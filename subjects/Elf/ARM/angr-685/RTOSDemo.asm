;;; Segment privileged_functions (00000000)
00000000 2C 02 00 20 09 80 00 00 01 80 00 00 05 80 00 00 ,.. ............
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000020 00 00 00 00 00 00 00 00 00 00 00 00 15 17 00 00 ................
00000030 00 00 00 00 00 00 00 00 89 16 00 00 E5 16 00 00 ................
00000040 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000050 00 00 00 00 09 81 00 00                         ........       

;; prvUnlockQueue: 00000058
prvUnlockQueue proc
	push	{r4-r6,lr}
	mov	r5,r0
	bl	$00008574
	ldrb	r4,[r5,#&45]
	sxtb	r4,r4
	cmps	r4,#0
	ble	$0000007E

l0000006A:
	ldr	r3,[r5,#&24]
	cbz	r3,$00000098

l0000006E:
	add	r6,r5,#&24
	b	$0000007C
00000074             01 3C E3 B2 5C B2 6B B1                 .<..\.k.   

l0000007C:
	ldr	r3,[r5,#&24]

l0000007E:
	cbz	r3,$00000098

l00000080:
	mov	r0,r6
	bl	$00001018
	cmps	r0,#0
	beq	$00000270

l0000008A:
	subs	r4,#1
	bl	$000011A8
	uxtb	r3,r4
	sxtb	r4,r3
	cmps	r3,#0
	bne	$00000278

l00000098:
	mov	r3,#&FF
	strb	r3,[r5,#&45]
	bl	$000085AC
	bl	$00008574
	ldrb	r4,[r5,#&44]
	sxtb	r4,r4
	cmps	r4,#0
	ble	$000000C4

l000000B0:
	ldr	r3,[r5,#&10]
	cbz	r3,$000000DE

l000000B4:
	add	r6,r5,#&10
	b	$000000C2
000000BA                               01 3C E3 B2 5C B2           .<..\.
000000C0 6B B1                                           k.             

l000000C2:
	ldr	r3,[r5,#&10]

l000000C4:
	cbz	r3,$000000DE

l000000C6:
	mov	r0,r6
	bl	$00001018
	cmps	r0,#0
	beq	$000002B6

l000000D0:
	subs	r4,#1
	bl	$000011A8
	uxtb	r3,r4
	sxtb	r4,r3
	cmps	r3,#0
	bne	$000002BE

l000000DE:
	mov	r3,#&FF
	strb	r3,[r5,#&44]
	pop.w	{r4-r6,lr}
	b	$00C085AC

;; prvCopyDataToQueue: 000000EC
prvCopyDataToQueue proc
	push	{r4-r6,lr}
	mov	r4,r0
	ldr	r0,[r0,#&40]
	ldr	r5,[r4,#&38]
	cbnz	r0,$00000102

l000000F6:
	ldr	r6,[r4]
	cmps	r6,#0

l000000FA:
	beq	$0000015C

l000000FC:
	adds	r5,#1
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}

l00000102:
	mov	r6,r2
	mov	r2,r0
	cbnz	r6,$00000128

l00000108:
	ldr	r0,[r4,#&8]
	bl	$0000A5C0
	ldr	r3,[r4,#&8]
	ldr	r1,[r4,#&40]
	ldr	r2,[r4,#&4]
	adds	r3,r1
	cmps	r3,r2
	str	r3,[r4,#&8]
	blo	$0000014C

l0000011C:
	ldr	r3,[r4]
	adds	r5,#1
	mov	r0,r6
	str	r3,[r4,#&8]
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}

l00000128:
	ldr	r0,[r4,#&C]
	bl	$0000A5C0
	ldr	r2,[r4,#&40]
	ldr	r3,[r4,#&C]
	rsbs	r2,r2
	ldr	r1,[r4]
	adds	r3,r2
	cmps	r3,r1
	str	r3,[r4,#&C]
	bhs	$00000140

l0000013E:
	ldr	r3,[r4,#&4]

l00000140:
	adds	r2,r3
	str	r2,[r4,#&C]
	cmps	r6,#2
	beq	$00000154

l00000148:
	adds	r5,#1
	mov	r0,#0

l0000014C:
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}
00000150 01 35 30 46                                     .50F           

l00000154:
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}
00000158                         05 B9 01 25                     ...%   

l0000015C:
	mov	r0,#0
	b	$000000FA
00000160 60 68 01 F0 75 F8 01 35 66 60 C8 E7             `h..u..5f`..   

;; prvCopyDataFromQueue: 0000016C
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
	b	$00C0A5C0

;; fn0000018C: 0000018C
fn0000018C proc
	bx	lr
0000018E                                           00 BF               ..

;; xQueueGenericSend: 00000190
xQueueGenericSend proc
	push.w	{r4-r10,lr}
	mov	r5,#0
	sub	sp,#&10
	mov	r4,r0
	mov	r10,r1
	str	r2,[sp,#&4]
	mov	r7,r3
	mov	r8,r5
	ldr	r9,[pc,#&FC]                                           ; 000002A6
	b	$000001F4
000001A8                         08 F0 02 FA 00 F0 2E FC         ........
000001B0 08 F0 E2 F9 94 F8 44 30 FF 2B 08 BF 84 F8 44 80 ......D0.+....D.
000001C0 94 F8 45 30 FF 2B 08 BF 84 F8 45 80 08 F0 F0 F9 ..E0.+....E.....
000001D0 01 A9 02 A8 00 F0 C0 FF 00 28 50 D1 08 F0 CC F9 .........(P.....
000001E0 A2 6B E3 6B 9A 42 17 D0 08 F0 E2 F9 20 46 FF F7 .k.k.B...... F..
000001F0 33 FF 00 F0                                     3...           

l000001F4:
	Invalid
	bl	$00008574
	ldr	r2,[r4,#&38]
	ldr	r3,[r4,#&3C]
	cmps	r2,r3
	blo	$00000242
	cmps	r7,#2
	beq	$00000242
	ldr	r6,[sp,#&4]
	cbz	r6,$00000272
	cmps	r5,#0
	bne	$000003A4
	add	r0,sp,#8
	bl	$00001140
	b	$000001A4
	bl	$000085AC
	ldr	r1,[sp,#&4]
	add	r0,r4,#&10
	bl	$00000FD8
	mov	r0,r4
	bl	$00000054
	bl	$00000E68
	cmps	r0,#0
	bne	$000003F2
	mov	r3,#&10000000
	str	r3,[r9]
	dsb	sy
	isb	sy
	b	$000001F2
	mov	r2,r7
	mov	r1,r10
	mov	r0,r4
	bl	$000000E8
	ldr	r3,[r4,#&24]
	cbnz	r3,$00000290
	cbz	r0,$00000266
	mov	r2,#&10000000
	ldr	r3,[pc,#&44]                                           ; 000002A6
	str	r2,[r3]
	dsb	sy
	isb	sy
	bl	$000085AC
	mov	r0,#1
	add	sp,#&10
	pop.w	{r4-r10,pc}

l00000270:
	strh	r0,[r6,#&7C]
	bl	$000085AC
	mov	r0,r6

l00000278:
	add	sp,#&10
	pop.w	{r4-r10,pc}
0000027E                                           20 46                F
00000280 FF F7 EA FE 00 F0 F2 FD 00 20 04 B0 BD E8 F0 87 ......... ......
00000290 04 F1 24 00 00 F0 C2 FE 00 28 DC D1 E3 E7 00 BF ..$......(......

;; fn000002A0: 000002A0
fn000002A0 proc
	Invalid

;; xQueuePeekFromISR: 000002A4
xQueuePeekFromISR proc
	push	{r4-r6,lr}
	mrs	r5,cpsr
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy

l000002B6:
	dsb	sy
	ldr	r3,[r0,#&38]
	cbnz	r3,$000002C6

l000002BE:
	mov	r0,r3
	msr	cpsr,r5
	pop	{r4-r6,pc}
000002C6                   04 46 C6 68 FF F7 4F FF E6 60       .F.h..O..`
000002D0 01 20 85 F3                                     . ..           

;; fn000002D4: 000002D4
fn000002D4 proc
	ldrh	r1,[r2]
	pop	{r4-r6,pc}

;; xQueueGenericReceive: 000002D8
xQueueGenericReceive proc
	push.w	{r4-r10,lr}
	mov	r5,#0
	sub	sp,#&10
	mov	r4,r0
	mov	r10,r1
	str	r2,[sp,#&4]
	mov	r9,r3
	mov	r7,r5
	ldr	r8,[pc,#&138]                                          ; 0000042A
	b	$00000306
000002F0 08 F0 42 F9 A3 6B 00 2B 37 D0 08 F0 59 F9 20 46 ..B..k.+7...Y. F
00000300 FF F7 AA FE 00 F0                               ......         

l00000306:
	Invalid
	bl	$00008574
	ldr	r6,[r4,#&38]
	cmps	r6,#0
	bne	$000003AC
	ldr	r3,[sp,#&4]
	cmps	r3,#0
	beq	$000003A0
	cmps	r5,#0
	beq	$00000398
	bl	$000085AC
	bl	$00000A08
	bl	$00008574
	ldrb	r3,[r4,#&44]
	cmps	r3,#&FF
	it	eq
	strbeq	r7,[r4,#&44]
	ldrb	r3,[r4,#&45]
	cmps	r3,#&FF
	it	eq
	strbeq	r7,[r4,#&45]
	bl	$000085AC
	add	r1,sp,#4
	add	r0,sp,#8
	bl	$00001154
	cmps	r0,#0
	beq	$000004EC
	mov	r0,r4
	bl	$00000054
	bl	$00000E68
	bl	$00008574
	ldr	r3,[r4,#&38]
	cbz	r3,$000003A4
	bl	$000085AC
	b	$00000304
	bl	$000085AC
	ldr	r3,[r4]
	cbz	r3,$000003D8
	ldr	r1,[sp,#&4]
	add	r0,r4,#&24
	bl	$00000FD8
	mov	r0,r4
	bl	$00000054
	bl	$00000E68
	cmps	r0,#0
	bne	$00000504
	mov	r3,#&10000000
	str	r3,[r8]
	dsb	sy
	isb	sy
	b	$00000304
	add	r0,sp,#8
	bl	$00001140
	b	$0000031A
	bl	$000085AC
	mov	r0,#0
	add	sp,#&10
	pop.w	{r4-r10,pc}
	mov	r1,r10
	mov	r0,r4
	ldr	r5,[r4,#&C]
	bl	$00000168
	cmp	r9,#0
	bne	$000003E4
	ldr	r3,[r4]
	subs	r6,#1
	str	r6,[r4,#&38]
	cbz	r3,$0000041C
	ldr	r3,[r4,#&10]
	cbnz	r3,$0000040E
	bl	$000085AC
	mov	r0,#1
	add	sp,#&10
	pop.w	{r4-r10,pc}
	bl	$00008574
	ldr	r0,[r4,#&4]
	bl	$000011B8
	bl	$000085AC
	b	$0000036E
	ldr	r3,[r4,#&24]
	str	r5,[r4,#&C]
	cmps	r3,#0
	beq	$000005C8
	add	r0,r4,#&24
	bl	$00001018
	cmps	r0,#0
	beq	$000005C8
	mov	r2,#&10000000
	ldr	r3,[pc,#&20]                                           ; 00000428
	str	r2,[r3]
	dsb	sy
	isb	sy
	b	$000003C8
	add	r0,r4,#&10
	bl	$00001018
	cmps	r0,#0
	bne	$000005F8
	b	$000003C8
	bl	$000012D0
	str	r0,[r4,#&4]
	b	$000003C4

;; fn00000424: 00000424
fn00000424 proc
	Invalid

;; uxQueueMessagesWaiting: 00000428
uxQueueMessagesWaiting proc
	push	{r4,lr}
	mov	r4,r0
	bl	$00008574
	ldr	r4,[r4,#&38]
	bl	$000085AC
	mov	r0,r4

;; fn00000438: 00000438
fn00000438 proc
	pop	{r4,pc}
0000043A                               00 BF                       ..   

;; uxQueueSpacesAvailable: 0000043C
uxQueueSpacesAvailable proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008574
	ldr	r0,[r5,#&38]
	ldr	r4,[r5,#&3C]
	sub	r4,r4,r0
	bl	$000085AC
	mov	r0,r4

;; fn00000450: 00000450
fn00000450 proc
	pop	{r3-r5,pc}
00000452       00 BF                                       ..           

;; vQueueDelete: 00000454
vQueueDelete proc
	b	$00C0177C

;; xQueueGenericSendFromISR: 00000458
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
	blo	$0000047E
	cmps	r3,#2
	beq	$0000047E
	mov	r0,#0
	msr	cpsr,r6
	pop	{r3-r7,pc}
	ldrb	r4,[r0,#&45]
	mov	r7,r2
	sxtb	r4,r4
	mov	r2,r3
	mov	r5,r0
	bl	$000000E8
	add	r3,r4,#1
	beq	$000004A2
	adds	r4,#1
	sxtb	r4,r4
	strb	r4,[r5,#&45]
	mov	r0,#1
	msr	cpsr,r6
	pop	{r3-r7,pc}
	ldr	r3,[r5,#&24]
	cmps	r3,#0
	beq	$0000069A
	add	r0,r5,#&24
	bl	$00001018
	cmps	r0,#0
	beq	$0000069A
	cmps	r7,#0
	beq	$0000069A
	mov	r0,#1
	str	r0,[r7]
	b	$00000478
	nop

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
	bhs	$000004FC
	ldrb	r3,[r0,#&45]
	adds	r2,#1
	sxtb	r3,r3
	str	r2,[r0,#&38]
	add	r2,r3,#1
	beq	$00000504
	adds	r3,#1
	sxtb	r3,r3
	strb	r3,[r0,#&45]
	mov	r0,#1
	msr	cpsr,r4
	pop	{r3-r5,pc}
	mov	r0,#0
	msr	cpsr,r4
	pop	{r3-r5,pc}
	ldr	r3,[r0,#&24]
	cmps	r3,#0
	beq	$000006F4
	adds	r0,#&24
	mov	r5,r1
	bl	$00001018
	cmps	r0,#0
	beq	$000006F4
	cmps	r5,#0
	beq	$000006F4
	mov	r0,#1
	str	r0,[r5]
	b	$000004F6

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
	mov	r0,r4
	msr	cpsr,r6
	pop.w	{r4-r8,pc}
	mov	r7,r0
	ldrb	r5,[r0,#&44]
	mov	r8,r2
	sxtb	r5,r5
	bl	$00000168
	subs	r4,#1
	add	r3,r5,#1
	str	r4,[r7,#&38]
	beq	$0000056E
	adds	r5,#1
	sxtb	r5,r5
	strb	r5,[r7,#&44]
	mov	r0,#1
	msr	cpsr,r6
	pop.w	{r4-r8,pc}
	ldr	r3,[r7,#&10]
	cmps	r3,#0
	beq	$00000764
	add	r0,r7,#&10
	bl	$00001018
	cmps	r0,#0
	beq	$00000764
	cmp	r8,#0
	beq	$00000764
	mov	r0,#1
	str	r0,[r8]
	b	$0000053E
	nop

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
xQueueGetMutexHolder proc
	push	{r4,lr}
	mov	r4,r0
	bl	$00008574
	ldr	r3,[r4]
	cbnz	r3,$000005CA

l000005C0:
	ldr	r4,[r4,#&4]
	bl	$000085AC
	mov	r0,r4
	pop	{r4,pc}

l000005CA:
	mov	r4,#0
	bl	$000085AC

;; fn000005D0: 000005D0
fn000005D0 proc
	mov	r0,r4
	pop	{r4,pc}

;; xQueueTakeMutexRecursive: 000005D4
xQueueTakeMutexRecursive proc
	push	{r4-r6,lr}
	ldr	r5,[r0,#&4]
	mov	r4,r0
	mov	r6,r1
	bl	$00001134
	cmps	r5,r0
	beq	$000005F6

l000005E4:
	mov	r3,#0
	mov	r2,r6
	mov	r1,r3
	mov	r0,r4
	bl	$000002D4
	cbz	r0,$000005F8

l000005F2:
	ldr	r3,[r4,#&C]
	adds	r3,#1

l000005F6:
	str	r3,[r4,#&C]

l000005F8:
	pop	{r4-r6,pc}
000005FA                               01 20 E3 68 03 44           . .h.D

;; fn00000600: 00000600
fn00000600 proc
	str	r3,[r4,#&C]
	pop	{r4-r6,pc}

;; xQueueGiveMutexRecursive: 00000604
xQueueGiveMutexRecursive proc
	push	{r3-r5,lr}
	ldr	r5,[r0,#&4]
	mov	r4,r0
	bl	$00001134
	cmps	r5,r0
	beq	$00000612

l00000612:
	mov	r0,#0
	pop	{r3-r5,pc}
00000616                   E3 68 01 3B E3 60 0B B1 01 20       .h.;.`... 
00000620 38 BD 20 46 1A 46 19 46 FF F7 B2 FD             8. F.F.F....   

;; fn0000062C: 0000062C
fn0000062C proc
	mov	r0,#1
	pop	{r3-r5,pc}

;; xQueueGenericReset: 00000630
xQueueGenericReset proc
	push	{r4-r6,lr}
	mov	r4,r0
	mov	r6,r1
	mov	r5,#&FF
	bl	$00008574
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
	bl	$000085AC
	mov	r0,#1
	pop	{r4-r6,pc}

l0000066C:
	add	r0,r4,#&10
	bl	$00001018
	cmps	r0,#0
	beq	$00000860

l00000678:
	mov	r2,#&10000000
	ldr	r3,[pc,#&28]                                           ; 000006AC
	str	r2,[r3]
	dsb	sy
	isb	sy
	bl	$000085AC
	mov	r0,#1
	pop	{r4-r6,pc}

l00000690:
	add	r0,r4,#&10
	bl	$000082CC
	add	r0,r4,#&24
	bl	$000082CC
	bl	$000085AC
	mov	r0,#1
	pop	{r4-r6,pc}

;; fn000006A8: 000006A8
fn000006A8 proc
	Invalid

;; xQueueGenericCreate: 000006AC
xQueueGenericCreate proc
	push	{r4-r6,lr}
	mov	r6,r0
	mul	r0,r0,r1
	adds	r0,#&48
	mov	r5,r1
	bl	$00001728
	mov	r4,r0
	cbz	r0,$000006D4

l000006C0:
	cbz	r5,$000006D8

l000006C2:
	add	r3,r0,#&48

l000006C4:
	lsls	r0,r1,#&D

;; fn000006C6: 000006C6
fn000006C6 proc
	str	r3,[r0]
	str	r6,[r4,#&3C]
	str	r5,[r4,#&40]
	mov	r1,#1
	mov	r0,r4
	bl	$0000062C

l000006D4:
	mov	r0,r4
	pop	{r4-r6,pc}

;; fn000006D8: 000006D8
fn000006D8 proc
	str	r0,[r4]
	b	$000006C4

;; xQueueCreateMutex: 000006DC
xQueueCreateMutex proc
	push	{r4,lr}
	mov	r2,r0
	mov	r1,#0
	mov	r0,#1
	bl	$000006A8
	mov	r4,r0
	cbz	r0,$000006FC

l000006EC:
	mov	r3,#0
	str	r3,[r0,#&4]
	str	r3,[r0]
	str	r3,[r0,#&C]
	mov	r2,r3
	mov	r1,r3
	bl	$0000018C

;; fn000006FC: 000006FC
fn000006FC proc
	mov	r0,r4
	pop	{r4,pc}

;; prvInitialiseNewTask: 00000700
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
	ldrb	r6,[r3,#&1]
	strb	r6,[r0],#&1
	ldrb	r6,[r3,#&1]!
	cbz	r6,$0000073E

l0000073A:
	cmps	r3,r1
	bne	$0000092A

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
	bl	$000082E4
	add	r0,r4,#&38
	bl	$000082E4
	rsb	r3,r7,#2
	str	r3,[r4,#&38]
	ldr	r2,[r4,#&50]
	mov	r3,fp
	ldr	r1,[sp,#&34]
	add	r0,r4,#4
	str	r4,[r4,#&30]
	str	r4,[r4,#&44]
	bl	$00001550
	str	r6,[r4,#&60]
	mov	r3,r10
	strb	r6,[r4,#&64]
	mov	r2,r9
	mov	r1,r8
	mov	r0,r5
	bl	$00001378
	ldr	r3,[sp,#&2C]
	str	r0,[r4]
	cbz	r3,$00000792

l00000790:
	str	r4,[r3]

l00000792:
	pop.w	{r3-fp,pc}

;; fn00000794: 00000794
fn00000794 proc
	ldrh	r0,[r7,#&7C]
00000796                   00 BF                               ..       

;; prvAddNewTaskToReadyList: 00000798
prvAddNewTaskToReadyList proc
	push.w	{r4-r8,lr}
	ldr	r4,[pc,#&B4]                                           ; 00000858
	mov	r5,r0
	bl	$00008574
	ldr	r3,[r4]
	adds	r3,#1
	str	r3,[r4]
	ldr	r3,[r4,#&4]
	cmps	r3,#0
	beq	$0000080E

l000007B0:
	ldr	r3,[r4,#&74]
	cbz	r3,$00000800

l000007B4:
	ldr	r0,[r5,#&4C]

l000007B6:
	add	r6,r4,#8
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
	bl	$000082EC
	bl	$000085AC
	ldr	r3,[r4,#&74]
	cbz	r3,$000007FC

l000007E2:
	ldr	r2,[r4,#&4]
	ldr	r3,[r5,#&4C]
	ldr	r2,[r2,#&4C]
	cmps	r2,r3
	bhs	$000007F8

l000007EC:
	mov	r2,#&10000000
	ldr	r3,[pc,#&64]                                           ; 0000085C
	str	r2,[r3]
	dsb	sy

l000007F8:
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

l0000080E:
	str	r5,[r4,#&4]
	b	$000007B6
00000812       65 60 23 68 01 2B CC D1 04 F1 08 06 30 46   e`#h.+......0F
00000820 07 F0 56 FD 04 F1 30 08 04 F1 1C 00 07 F0 50 FD ..V...0.......P.
00000830 04 F1 44 07 40 46 07 F0 4B FD 38 46 07 F0 48 FD ..D.@F..K.8F..H.
00000840 04 F1 58 00 07 F0 44 FD C4 F8 6C 80 E8 6C 27 67 ..X...D...l..l'g
00000850 B3 E7 00 BF C4 00 00 20                         .......        

;; fn00000858: 00000858
fn00000858 proc
	Invalid

;; prvAddCurrentTaskToDelayedList.isra.0: 0000085C
prvAddCurrentTaskToDelayedList.isra.0 proc
	push	{r4-r6,lr}
	ldr	r4,[pc,#&50]                                           ; 000008B6

;; fn00000860: 00000860
fn00000860 proc
	mov	r5,r0
	ldr	r6,[r4,#&80]
	ldr	r0,[r4,#&4]
	adds	r0,#&24
	bl	$0000833C
	cbnz	r0,$00000880

;; fn00000870: 00000870
fn00000870 proc
	mov	r2,#1
	ldr	r1,[r4,#&4]
	ldr	r3,[r4,#&7C]
	ldr	r1,[r1,#&4C]
	lsls	r2,r1
	bic.w	r3,r3,r2
	str	r3,[r4,#&7C]

;; fn00000880: 00000880
fn00000880 proc
	adds	r5,r6
	ldr	r3,[r4,#&4]
	cmps	r6,r5
	str	r5,[r3,#&24]
	bhi	$0000089E

l0000088A:
	ldr	r0,[r4,#&6C]
	ldr	r1,[r4,#&4]
	adds	r1,#&24
	bl	$00008308
	ldr	r3,[r4,#&84]
	cmps	r5,r3
	it	lo
	strlo	r5,[r4,#&84]

l0000089E:
	str	r4,[r0,r2]

l000008A0:
	pop	{r4-r6,pc}
000008A2       20 6F 61 68 BD E8 70 40 24 31 07 F0 2E BD    oah..p@$1....

;; fn000008B0: 000008B0
fn000008B0 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskCreate: 000008B4
xTaskCreate proc
	push.w	{r4-r10,lr}
	mov	r8,r0
	sub	sp,#&10
	lsls	r0,r2,#2
	mov	r6,r2
	mov	r9,r1
	mov	r10,r3
	bl	$00001728
	cbz	r0,$00000904

l000008CA:
	mov	r5,r0
	mov	r0,#&68
	bl	$00001728
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
	bl	$000006FC
	mov	r0,r4
	bl	$00000794

;; fn000008FA: 000008FA
fn000008FA proc
	vhadd.u8	d18,d14,d1

l000008FC:
	mov	r0,#1
	add	sp,#&10
	pop.w	{r4-r10,pc}

l00000904:
	mov	r0,#&FFFFFFFF
	add	sp,#&10
	pop.w	{r4-r10,pc}

l0000090E:
	mov	r0,r5
	bl	$0000177C
	mov	r0,#&FFFFFFFF

;; fn00000918: 00000918
fn00000918 proc
	b	$000008FA
0000091A                               00 BF                       ..   

;; xTaskCreateRestricted: 0000091C
xTaskCreateRestricted proc
	ldr	r3,[r0,#&14]
	cbz	r3,$0000096A

l00000920:
	push	{r4-r7,lr}
	mov	r4,r0
	sub	sp,#&14
	mov	r0,#&68
	mov	r7,r1

;; fn0000092A: 0000092A
fn0000092A proc
	bl	$00001728
	mov	r5,r0
	cbz	r0,$00000964

;; fn00000932: 00000932
fn00000932 proc
	mov	r6,#1
	ldr	r1,[r4,#&14]
	strb	r6,[r0,#&65]
	ldr	r3,[r4,#&C]
	ldrh	r2,[r4,#&10]
	ldr	lr,[r4,#&10]
	str	r1,[r0,#&50]
	ldr	r1,[r4,#&4]
	str	r0,[sp,#&8]
	str	r7,[sp,#&4]
	ldr	r0,[r4],#&18
	str	lr,[sp]
	str	r4,[sp,#&C]
	bl	$000006FC
	mov	r0,r5
	bl	$00000794

l0000095C:
	vmin.u16	d4,d13,d16
	add	sp,#&14
	pop	{r4-r7,pc}

;; fn00000964: 00000964
fn00000964 proc
	mov	r0,#&FFFFFFFF
	b	$0000095C

l0000096A:
	mov	r0,#&FFFFFFFF

;; fn0000096C: 0000096C
fn0000096C proc
	adds	r0,#&FF

l0000096E:
	bx	lr

;; vTaskAllocateMPURegions: 00000970
vTaskAllocateMPURegions proc
	cbz	r0,$0000097C

l00000972:
	mov	r3,#0
	adds	r0,#4
	mov	r2,r3
	b	$00C01550

l0000097C:
	ldr	r3,[pc,#&C]                                            ; 00000990
	ldr	r0,[r3,#&4]
	mov	r3,#0
	adds	r0,#4
	mov	r2,r3
	b	$00C01550
0000098A                               00 BF                       ..   

;; fn0000098C: 0000098C
fn0000098C proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskStartScheduler: 00000990
vTaskStartScheduler proc
	mov	r3,#&80000000
	push	{r4,lr}
	ldr	r4,[pc,#&48]                                           ; 000009E6
	sub	sp,#8
	str	r3,[sp]
	add	r3,r4,#&88
	str	r3,[sp,#&4]
	mov	r2,#&3B
	mov	r3,#0
	ldr	r1,[pc,#&3C]                                           ; 000009EA
	ldr	r0,[pc,#&3C]                                           ; 000009EC
	bl	$000008B0
	cmps	r0,#1
	beq	$000009B2

l000009B2:
	add	sp,#8
	pop	{r4,pc}
000009B6                   4F F0 BF 03 83 F3 11 88 BF F3       O.........
000009C0 6F 8F BF F3 4F 8F 4F F0 FF 32 00 23 C4 F8 84 20 o...O.O..2.#... 
000009D0 60 67 C4 F8 80 30 02 B0 BD E8 10 40 00 F0 E8 BC `g...0.....@....
000009E0 C4 00 00 20 7C A2 00 00 2D 85 00 00             ... |...-...   

;; vTaskEndScheduler: 000009EC
vTaskEndScheduler proc
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	mov	r2,#0
	ldr	r3,[pc,#&8]                                            ; 00000A0E
	str	r2,[r3,#&74]
	b	$00C0154C
	nop

;; fn00000A08: 00000A08
fn00000A08 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskSuspendAll: 00000A0C
vTaskSuspendAll proc
	ldr	r2,[pc,#&C]                                            ; 00000A20
	ldr	r3,[r2,#&8C]
	adds	r3,#1
	str	r3,[r2,#&8C]
	bx	lr
00000A1A                               00 BF                       ..   

;; fn00000A1C: 00000A1C
fn00000A1C proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskGetTickCount: 00000A20
xTaskGetTickCount proc
	ldr	r3,[pc,#&4]                                            ; 00000A2C
	ldr	r0,[r3,#&80]
	bx	lr
00000A28                         C4 00 00 20                     ...    

;; xTaskGetTickCountFromISR: 00000A2C
xTaskGetTickCountFromISR proc
	ldr	r3,[pc,#&4]                                            ; 00000A38
	ldr	r0,[r3,#&80]
	bx	lr

;; fn00000A34: 00000A34
fn00000A34 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; uxTaskGetNumberOfTasks: 00000A38
uxTaskGetNumberOfTasks proc
	ldr	r3,[pc,#&4]                                            ; 00000A44
	ldr	r0,[r3]
	bx	lr
00000A3E                                           00 BF               ..

;; fn00000A40: 00000A40
fn00000A40 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; pcTaskGetName: 00000A44
pcTaskGetName proc
	cbz	r0,$00000A4A

l00000A46:
	adds	r0,#&54
	bx	lr

l00000A4A:
	ldr	r3,[pc,#&8]                                            ; 00000A5A
	ldr	r0,[r3,#&4]
	adds	r0,#&54
	bx	lr
00000A52       00 BF                                       ..           

;; fn00000A54: 00000A54
fn00000A54 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskGenericNotify: 00000A58
xTaskGenericNotify proc
	push	{r3-r7,lr}
	mov	r4,r3
	mov	r6,r0
	mov	r7,r1
	mov	r5,r2
	bl	$00008574
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
	bhi	$00000A88

l00000A7E:
	tbb	[pc,-r2]                                               ; 00000A86

l00000A82:
	lsrs	r2,r7,#&10
l00000A83	db	0x0C
l00000A84	db	0x04
l00000A85	db	0x02

l00000A86:
	cmps	r4,#2

l00000A88:
	beq	$00000AFA

l00000A8A:
	str	r7,[r6,#&60]
	cmps	r4,#1
	beq	$00000AA2

l00000A90:
	mov	r4,#1
	bl	$000085AC

l00000A96:
	mov	r0,r4
	pop	{r3-r7,pc}
00000A9A                               33 6E 01 2C 03 F1           3n.,..
00000AA0 01 03                                           ..             

l00000AA2:
	str	r3,[r6,#&60]
	bne	$00000C8C

l00000AA6:
	add	r7,r6,#&24
	ldr	r5,[pc,#&58]                                           ; 00000B0A
	mov	r0,r7
	bl	$0000833C
	ldr	r0,[r6,#&4C]
	ldr	lr,[r5,#&7C]
	add	r2,r5,#8
	lsl	r3,r4,r0
	add.w	r0,r0,r0,lsl #2
	orr	r3,r3,lr
	add.w	r0,r2,r0,lsl #2
	mov	r1,r7
	str	r3,[r5,#&7C]
	bl	$000082EC
	ldr	r3,[r5,#&4]
	ldr	r2,[r6,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	bls	$00000C8C

l00000ADE:
	mov	r2,#&10000000
	ldr	r3,[pc,#&24]                                           ; 00000B0E
	str	r2,[r3]
	dsb	sy
	isb	sy
	bl	$000085AC

l00000AF2:
	mov	r0,r4
	pop	{r3-r7,pc}
00000AF6                   33 6E 1F 43                         3n.C     

l00000AFA:
	str	r7,[r6,#&60]
	b	$00000A88
00000AFE                                           00 24               .$
00000B00 C7 E7 00 BF C4 00 00 20 04 ED 00 E0             ....... ....   

;; xTaskGenericNotifyFromISR: 00000B0C
xTaskGenericNotifyFromISR proc
	push.w	{r4-r8,lr}
	mrs	r5,cpsr
	mov	r4,#&BF
	msr	cpsr,r4
	isb	sy
	dsb	sy
	cbz	r3,$00000B2A
	ldr	r4,[r0,#&60]
	str	r4,[r3]
	mov	r3,#2
	ldrb	r4,[r0,#&64]
	subs	r2,#1
	strb	r3,[r0,#&64]
	uxtb	r4,r4
	cmps	r2,#3
	bhi	$00000B46
	tbb	[pc,-r2]                                               ; 00000B44
	lsrs	r2,r5,#&10
	lsls	r4,r0,#8
	cmps	r4,#2
	beq	$00000BC0
	str	r1,[r0,#&60]
	cmps	r4,#1
	beq	$00000B60
	mov	r0,#1
	msr	cpsr,r5
	pop.w	{r4-r8,pc}
	ldr	r3,[r0,#&60]
	cmps	r4,#1
	add	r3,r3,#1
	str	r3,[r0,#&60]
	bne	$00000D4A
	ldr	r6,[pc,#&68]                                           ; 00000BD4
	mov	r7,r0
	ldr	r3,[r6,#&8C]
	cbz	r3,$00000B9C
	add	r1,r0,#&38
	add	r0,r6,#&58
	bl	$000082EC
	ldr	r3,[r6,#&4]
	ldr	r2,[r7,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	bls	$00000D4A
	ldr	r3,[sp,#&18]
	mov	r0,#1
	cbz	r3,$00000BC8
	str	r0,[r3]
	msr	cpsr,r5
	pop.w	{r4-r8,pc}
	ldr	r3,[r0,#&60]
	orrs	r1,r3
	str	r1,[r0,#&60]
	b	$00000B46
	add	r8,r0,#&24
	mov	r0,r8
	bl	$0000833C
	ldr	r0,[r7,#&4C]
	ldr	r2,[r6,#&7C]
	lsls	r4,r0
	add	r3,r6,#8
	add.w	r0,r0,r0,lsl #2
	orrs	r4,r2
	mov	r1,r8
	add.w	r0,r3,r0,lsl #2
	str	r4,[r6,#&7C]
	bl	$000082EC
	b	$00000B76
	mov	r0,#0
	b	$00000B4C
	str	r0,[r6,#&90]
	b	$00000B4C
	nop

;; fn00000BD0: 00000BD0
fn00000BD0 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskNotifyWait: 00000BD4
xTaskNotifyWait proc
	push.w	{r4-r8,lr}
	ldr	r4,[pc,#&7C]                                           ; 00000C5C
	mov	r5,r2
	mov	r8,r0
	mov	r6,r1
	mov	r7,r3
	bl	$00008574
	ldr	r2,[r4,#&4]
	ldrb	r2,[r2,#&64]
	cmps	r2,#2
	beq	$00000C00

l00000BF0:
	mov	r0,#1
	ldr	r1,[r4,#&4]
	ldr	r2,[r1,#&60]
	bic.w	r2,r2,r8
	str	r2,[r1,#&60]
	ldr	r3,[r4,#&4]
	strb	r0,[r3,#&64]

l00000C00:
	lsls	r4,r4,#1

l00000C02:
	cbnz	r7,$00000C3C

l00000C04:
	bl	$000085AC
	bl	$00008574
	cbz	r5,$00000C14

l00000C0E:
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&60]
	str	r3,[r5]

l00000C14:
	ldr	r3,[r4,#&4]
	ldrb	r3,[r3,#&64]
	cmps	r3,#1
	beq	$00000C50

l00000C1E:
	mov	r5,#1
	ldr	r3,[r4,#&4]
	ldr	r1,[r3,#&60]
	bic.w	r1,r1,r6
	str	r1,[r3,#&60]
	mov	r2,#0
	ldr	r3,[r4,#&4]
	strb	r2,[r3,#&64]
	bl	$000085AC
	mov	r0,r5
	pop.w	{r4-r8,pc}

l00000C3C:
	mov	r0,r7
	bl	$00000858
	mov	r2,#&10000000
	ldr	r3,[pc,#&14]                                           ; 00000C62
	str	r2,[r3]
	dsb	sy
	isb	sy

l00000C50:
	ldrh	r7,[r5,#&74]
	b	$00000C00
00000C54             00 25 E8 E7 C4 00 00 20 04 ED 00 E0     .%..... ....

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

;; fn00000C8C: 00000C8C
fn00000C8C proc
	beq	$00000C92

l00000C8E:
	msr	cpsr,r6

l00000C92:
	pop.w	{r3-r9,pc}
00000C96                   19 4F 88 46 D7 F8 8C 30 04 46       .O.F...0.F
00000CA0 A3 B1 00 F1 38 01 07 F1 58 00 07 F0 21 FB 7B 68 ....8...X...!.{h
00000CB0 E2 6C DB 6C 9A 42 EA D9 01 23 B8 F1 00 0F 19 D0 .l.l.B...#......
00000CC0 C8 F8 00 30 86 F3 11 88 BD E8 F8 83 00 F1 24 09 ...0..........$.
00000CD0 48 46 07 F0 35 FB E0 6C FA 6F 85 40 07 F1 08 03 HF..5..l.o.@....
00000CE0 00 EB 80 00 15 43 49 46 03 EB 80 00 FD 67 07 F0 .....CIF.....g..
00000CF0 FF FA DC E7 C7 F8 90 30 C9 E7 00 BF             .......0....   

;; fn00000CFC: 00000CFC
fn00000CFC proc
	lsls	r4,r0,#3
	mov	r0,#0

;; ulTaskNotifyTake: 00000D00
ulTaskNotifyTake proc
	push	{r4-r6,lr}
	ldr	r4,[pc,#&60]                                           ; 00000D6A
	mov	r6,r0
	mov	r5,r1
	bl	$00008574
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&60]
	cbnz	r3,$00000D1C

l00000D12:
	mov	r2,#1
	ldr	r3,[r4,#&4]
	strb	r2,[r3,#&64]
	cbnz	r5,$00000D4A

l00000D1C:
	bl	$000085AC
	bl	$00008574
	ldr	r3,[r4,#&4]
	ldr	r5,[r3,#&60]
	cbz	r5,$00000D32

l00000D2A:
	cbnz	r6,$00000D42

l00000D2C:
	ldr	r3,[r4,#&4]

l00000D2E:
	sub	r2,r5,#1
	str	r2,[r3,#&60]

l00000D32:
	mov	r2,#0
	ldr	r3,[r4,#&4]
	strb	r2,[r3,#&64]
	bl	$000085AC
	mov	r0,r5
	pop	{r4-r6,pc}

l00000D42:
	mov	r2,#0
	ldr	r3,[r4,#&4]
	str	r2,[r3,#&60]
	b	$00000D2E

l00000D4A:
	mov	r0,r5
	bl	$00000858
	mov	r2,#&10000000
	ldr	r3,[pc,#&10]                                           ; 00000D6C
	str	r2,[r3]
	dsb	sy
	isb	sy
	b	$00000D18
	nop
	lsls	r4,r0,#3
	mov	r0,#0

;; fn00000D68: 00000D68
fn00000D68 proc
	Invalid

;; xTaskIncrementTick: 00000D6C
xTaskIncrementTick proc
	push.w	{r4-r10,lr}
	ldr	r4,[pc,#&F0]                                           ; 00000E68
	ldr	r3,[r4,#&8C]
	cmps	r3,#0
	bne	$00000E34

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
	bne	$00000E42

l00000DA0:
	mov	r3,#&FFFFFFFF
	str	r3,[r4,#&84]

l00000DA8:
	ldr	r3,[r4,#&84]
	mov	r6,#0
	cmps	r7,r3
	blo	$00000E10

l00000DB2:
	mov	r9,#1
	ldr	r8,[pc,#&B0]                                           ; 00000E6E
	b	$00000E00
00000DBC                                     E3 6E DB 68             .n.h
00000DC0 DD 68 6B 6A 05 F1 24 0A 9F 42 48 D3 50 46 07 F0 .hkj..$..BH.PF..
00000DD0 B7 FA AB 6C 05 F1 38 00 0B B1 07 F0 B1 FA E8 6C ...l..8........l
00000DE0 E2 6F 09 FA 00 F3 00 EB 80 00 13 43 51 46 08 EB .o.........CQF..
00000DF0 80 00 E3 67 07 F0 7C FA 63 68 EA 6C DB 6C 9A 42 ...g..|.ch.l.l.B

l00000E00:
	it	hs
	movhs	r6,#1

l00000E04:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3]
	cmps	r3,#0
	bne	$00000FB8

l00000E0C:
	mov	r3,#&FFFFFFFF

l00000E10:
	str	r3,[r4,#&84]
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&4C]
	add.w	r3,r3,r3,lsl #2
	add.w	r3,r4,r3,lsl #2
	ldr	r3,[r3,#&8]
	cmps	r3,#2

l00000E24:
	it	hs
	movhs	r6,#1

l00000E28:
	ldr	r3,[r4,#&90]
	cmps	r3,#0
	it	ne
	movne	r6,#1

l00000E32:
	mov	r0,r6

l00000E34:
	pop.w	{r4-r10,pc}
00000E38                         D4 F8 98 30 00 26 01 33         ...0.&.3
00000E40 C4 F8                                           ..             

l00000E42:
	adds	r0,#&98
	b	$00000E24
00000E46                   E3 6E 00 26 DB 68 DB 68 5B 6A       .n.&.h.h[j
00000E50 C4 F8 84 30 D4 F8 84 30 9F 42 DB D3 A9 E7 C4 F8 ...0...0.B......
00000E60 84 30 D7 E7 C4 00 00 20                         .0.....        

;; fn00000E68: 00000E68
fn00000E68 proc
	lsls	r4,r1,#3
	mov	r0,#0

;; xTaskResumeAll: 00000E6C
xTaskResumeAll proc
	push.w	{r4-r8,lr}
	ldr	r4,[pc,#&CC]                                           ; 00000F44
	bl	$00008574
	ldr	r3,[r4,#&8C]
	subs	r3,#1
	str	r3,[r4,#&8C]
	ldr	r5,[r4,#&8C]
	cmps	r5,#0
	bne	$00000F22

l00000E88:
	ldr	r3,[r4]
	cmps	r3,#0
	beq	$00000F22

l00000E8E:
	mov	r6,#1
	add	r7,r4,#8
	b	$00000ED0
00000E96                   63 6E DD 68 05 F1 24 08 05 F1       cn.h..$...
00000EA0 38 00 07 F0 4D FA 40 46 07 F0 4A FA E8 6C E2 6F 8...M.@F..J..l.o
00000EB0 06 FA 00 F3 00 EB 80 00 13 43 41 46 07 EB 80 00 .........CAF....
00000EC0 E3 67 07 F0 15 FA 63 68 EA 6C DB 6C 9A 42 28 BF .g....ch.l.l.B(.

l00000ED0:
	str	r6,[r4,#&90]
	ldr	r3,[r4,#&58]
	cmps	r3,#0
	bne	$00001092

l00000EDA:
	cbz	r5,$00000EEA

l00000EDC:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3]
	cbnz	r3,$00000F32

l00000EE2:
	mov	r3,#&FFFFFFFF

l00000EE6:
	str	r3,[r4,#&84]

l00000EEA:
	ldr	r5,[r4,#&98]
	cbz	r5,$00000F04

l00000EF0:
	mov	r6,#1
	bl	$00000D68
	cbz	r0,$00000EFC

l00000EF8:
	str	r6,[r4,#&90]

l00000EFC:
	subs	r5,#1
	bne	$000010EE

l00000F00:
	str	r5,[r4,#&98]

l00000F04:
	ldr	r3,[r4,#&90]
	cbz	r3,$00000F26

l00000F0A:
	mov	r2,#&10000000
	ldr	r3,[pc,#&34]                                           ; 00000F4A
	str	r2,[r3]
	dsb	sy
	isb	sy
	mov	r4,#1
	bl	$000085AC
	mov	r0,r4

l00000F22:
	pop.w	{r4-r8,pc}

l00000F26:
	mov	r4,#0
	bl	$000085AC
	mov	r0,r4
	pop.w	{r4-r8,pc}

l00000F32:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3,#&C]
	ldr	r3,[r3,#&C]
	ldr	r3,[r3,#&24]
	str	r3,[r4,#&84]
	b	$00000EE6
00000F40 C4 00 00 20                                     ...            

;; fn00000F44: 00000F44
fn00000F44 proc
	Invalid

;; vTaskDelay: 00000F48
vTaskDelay proc
	push	{r3,lr}
	cbnz	r0,$00000F5E

l00000F4C:
	mov	r2,#&10000000
	ldr	r3,[pc,#&24]                                           ; 00000F7C
	str	r2,[r3]
	dsb	sy
	isb	sy
	pop	{r3,pc}

l00000F5E:
	ldr	r2,[pc,#&1C]                                           ; 00000F82
	ldr	r3,[r2,#&8C]
	adds	r3,#1
	str	r3,[r2,#&8C]
	bl	$00000858
	bl	$00000E68
	cmps	r0,#0
	beq	$00001148

l00000F76:
	pop	{r3,pc}
00000F78                         04 ED 00 E0                     ....   

;; fn00000F7C: 00000F7C
fn00000F7C proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskDelayUntil: 00000F80
vTaskDelayUntil proc
	ldr	r2,[pc,#&50]                                           ; 00000FD8
	push	{r4,lr}
	ldr	r4,[r2,#&8C]
	ldr	r3,[r0]
	adds	r4,#1
	str	r4,[r2,#&8C]
	ldr	r2,[r2,#&80]
	adds	r1,r3
	cmps	r2,r3
	bhs	$00000FB4

l00000F9A:
	cmps	r3,r1
	bhi	$00000FB8

l00000F9E:
	str	r1,[r0]
	bl	$00000E68
	cbnz	r0,$00000FD0

l00000FA6:
	mov	r2,#&10000000
	ldr	r3,[pc,#&2C]                                           ; 00000FDE
	str	r2,[r3]
	dsb	sy
	isb	sy

l00000FB4:
	ldrh	r7,[r5,#&74]
	pop	{r4,pc}

;; fn00000FB8: 00000FB8
fn00000FB8 proc
	cmps	r3,r1
	bhi	$00000FBC

l00000FBC:
	cmps	r2,r1
	bhs	$0000119A

l00000FC0:
	str	r1,[r0]
	sub	r0,r1,r2
	bl	$00000858
	bl	$00000E68
	cmps	r0,#0
	beq	$000011A2

l00000FD0:
	pop	{r4,pc}
00000FD2       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; vTaskPlaceOnEventList: 00000FDC
vTaskPlaceOnEventList proc
	push	{r4,lr}
	mov	r4,r1
	ldr	r3,[pc,#&10]                                           ; 00000FF8
	ldr	r1,[r3,#&4]
	adds	r1,#&38
	bl	$00008308
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00000858
00000FF2       00 BF                                       ..           

;; fn00000FF4: 00000FF4
fn00000FF4 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskPlaceOnUnorderedEventList: 00000FF8
vTaskPlaceOnUnorderedEventList proc
	push	{r3-r5,lr}
	mov	r4,r2
	ldr	r3,[pc,#&18]                                           ; 0000101C
	orr	r1,r1,#&80000000
	ldr	r5,[r3,#&4]
	ldr	r3,[r3,#&4]
	str	r1,[r5,#&38]
	add	r1,r3,#&38
	bl	$000082EC
	mov	r0,r4
	pop.w	{r3-r5,lr}
	b	$00000858

;; fn00001018: 00001018
fn00001018 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskRemoveFromEventList: 0000101C
xTaskRemoveFromEventList proc
	push	{r3-r7,lr}
	ldr	r3,[r0,#&C]
	ldr	r4,[pc,#&58]                                           ; 00001080
	ldr	r5,[r3,#&C]
	add	r6,r5,#&38
	mov	r0,r6
	bl	$0000833C
	ldr	r3,[r4,#&8C]
	cbnz	r3,$00001070

l00001034:
	add	r6,r5,#&24
	mov	r0,r6
	bl	$0000833C
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

l00001058:
	bl	$000082EC
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
	bl	$000082EC
	b	$00001058

;; fn0000107C: 0000107C
fn0000107C proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskRemoveFromUnorderedEventList: 00001080
xTaskRemoveFromUnorderedEventList proc
	push	{r3-r7,lr}
	mov	r5,#1
	ldr	r6,[r0,#&C]
	orr	r1,r1,#&80000000
	str	r1,[r0]
	add	r7,r6,#&24
	bl	$0000833C

;; fn00001092: 00001092
fn00001092 proc
	Invalid
	mov	r0,r7
	bl	$0000833C
	ldr	r3,[r6,#&4C]
	ldr	lr,[r4,#&7C]
	lsl	r2,r5,r3
	add	r0,r4,#8
	add.w	r3,r3,r3,lsl #2
	add.w	r0,r0,r3,lsl #2
	orr	r2,r2,lr
	mov	r1,r7
	str	r2,[r4,#&7C]
	bl	$000082EC
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
000010D2       00 BF                                       ..           

;; fn000010D4: 000010D4
fn000010D4 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskSwitchContext: 000010D8
vTaskSwitchContext proc
	ldr	r2,[pc,#&40]                                           ; 00001120
	ldr	r3,[r2,#&8C]
	cbnz	r3,$00001112

l000010E0:
	str	r3,[r2,#&90]
	ldr	r3,[r2,#&7C]
	clz	r3,r3
	uxtb	r3,r3
	rsb	r3,r3,#&1F

;; fn000010EE: 000010EE
fn000010EE proc
	lsls	r7,r3,#&C
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

;; fn00001108: 00001108
fn00001108 proc
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
0000111A                               00 BF                       ..   

;; fn0000111C: 0000111C
fn0000111C proc
	lsls	r4,r0,#3
	mov	r0,#0

;; uxTaskResetEventItemValue: 00001120
uxTaskResetEventItemValue proc
	ldr	r3,[pc,#&10]                                           ; 00001138
	ldr	r1,[r3,#&4]
	ldr	r2,[r3,#&4]
	ldr	r3,[r3,#&4]
	ldr	r0,[r1,#&38]
	ldr	r3,[r3,#&4C]
	rsb	r3,r3,#2
	str	r3,[r2,#&38]
	bx	lr

;; fn00001134: 00001134
fn00001134 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskGetCurrentTaskHandle: 00001138
xTaskGetCurrentTaskHandle proc
	ldr	r3,[pc,#&4]                                            ; 00001144
	ldr	r0,[r3,#&4]
	bx	lr
0000113E                                           00 BF               ..

;; fn00001140: 00001140
fn00001140 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskSetTimeOutState: 00001144
vTaskSetTimeOutState proc
	ldr	r3,[pc,#&C]                                            ; 00001158
	ldr	r2,[r3,#&94]

l00001148:
	mov	r0,#&94
	ldr	r3,[r3,#&80]
	stm	r0,{r2-r3}
	bx	lr

;; fn00001154: 00001154
fn00001154 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xTaskCheckForTimeOut: 00001158
xTaskCheckForTimeOut proc
	push	{r4-r6,lr}
	mov	r4,r0
	mov	r6,r1
	bl	$00008574
	ldr	r3,[pc,#&44]                                           ; 000011AE
	ldr	r1,[r4]
	ldr	r5,[r3,#&80]
	ldr	r2,[r3,#&94]
	ldr	r0,[r4,#&4]
	cmps	r1,r2
	beq	$00001174

l00001174:
	cmps	r5,r0
	bhs	$00001198

l00001178:
	ldr	r2,[r6]
	sub	r1,r5,r0
	cmps	r1,r2
	bhs	$00001198

l00001180:
	sub	r2,r2,r5
	mov	r5,#0
	ldr	r1,[r3,#&94]
	ldr	r3,[r3,#&80]
	adds	r2,r0
	str	r2,[r6]
	stm	r4,{r1,r3}
	bl	$000085AC

l00001198:
	mov	r0,r5

l0000119A:
	pop	{r4-r6,pc}
0000119C                                     01 25 07 F0             .%..
000011A0 07 FA                                           ..             

l000011A2:
	mov	r0,r5
	pop	{r4-r6,pc}
000011A6                   00 BF                               ..       

;; fn000011A8: 000011A8
fn000011A8 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; vTaskMissedYield: 000011AC
vTaskMissedYield proc
	mov	r2,#1
	ldr	r3,[pc,#&8]                                            ; 000011BE
	str	r2,[r3,#&90]
	bx	lr
000011B6                   00 BF C4 00 00 20                   .....    

;; vTaskPriorityInherit: 000011BC
vTaskPriorityInherit proc
	cmps	r0,#0
	beq	$00001242

l000011C0:
	push	{r3-r7,lr}
	ldr	r4,[pc,#&84]                                           ; 0000124E
	ldr	r3,[r0,#&4C]
	ldr	r2,[r4,#&4]
	ldr	r2,[r2,#&4C]
	cmps	r3,r2
	bhs	$000011F0

l000011CE:
	ldr	r2,[r0,#&38]
	cmps	r2,#0
	blt	$000011DA

l000011D4:
	ldr	r2,[r4,#&4]
	ldr	r2,[r2,#&4C]
	rsb	r2,r2,#2

l000011DA:
	lsls	r2,r0,#8

l000011DC:
	str	r2,[r0,#&38]
	ldr	r5,[pc,#&6C]                                           ; 00001252
	add.w	r3,r3,r3,lsl #2
	ldr	r2,[r0,#&34]
	add.w	r3,r5,r3,lsl #2
	cmps	r2,r3
	beq	$000011F2

l000011EE:
	ldr	r3,[r4,#&4]

l000011F0:
	ldr	r3,[r3,#&4C]

l000011F2:
	str	r3,[r0,#&4C]
	pop	{r3-r7,pc}
000011F6                   00 F1 24 07 06 46 38 46 07 F0       ..$..F8F..
00001200 9F F8 68 B9 F2 6C 02 EB 82 03 04 EB 83 03 9B 68 ..h..l.........h
00001210 33 B9 01 21 E3 6F 01 FA 02 F2 23 EA 02 02 E2 67 3..!.o....#....g
00001220 01 23 62 68 D4 F8 7C E0 D2 6C 39 46 93 40 43 EA .#bh..|..l9F.@C.
00001230 0E 03 02 EB 82 00 F2 64 05 EB 80 00 E3 67 BD E8 .......d.....g..
00001240 F8 40                                           .@             

l00001242:
	b	$00C082EC
00001246                   70 47 C4 00 00 20 CC 00 00 20       pG... ... 

;; xTaskPriorityDisinherit: 00001250
xTaskPriorityDisinherit proc
	cmps	r0,#0
	beq	$000012C4

l00001254:
	push	{r3-r7,lr}
	ldr	r1,[r0,#&4C]
	ldr	r3,[r0,#&5C]
	ldr	r2,[r0,#&58]
	subs	r3,#1
	cmps	r1,r2
	str	r3,[r0,#&5C]

l00001262:
	beq	$00001262

l00001264:
	cbz	r3,$0000126A

l00001266:
	mov	r0,#0
	pop	{r3-r7,pc}

l0000126A:
	add	r7,r0,#&24
	mov	r4,r0
	mov	r0,r7
	bl	$0000833C
	cbnz	r0,$00001298

l00001278:
	ldr	r1,[r4,#&4C]
	ldr	r2,[pc,#&50]                                           ; 000012D2
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

l00001296:
	b	$00001296

l00001298:
	ldr	r2,[pc,#&30]                                           ; 000012D0

l0000129A:
	mov	r5,#1
	ldr	r3,[r4,#&58]
	ldr	lr,[r2,#&7C]
	ldr	r0,[pc,#&2C]                                           ; 000012D6
	lsl	r6,r5,r3
	mov	r1,r7
	str	r3,[r4,#&4C]
	rsb	r7,r3,#2
	add.w	r3,r3,r3,lsl #2
	orr	r6,r6,lr
	add.w	r0,r0,r3,lsl #2
	str	r7,[r4,#&38]
	str	r6,[r2,#&7C]
	bl	$000082EC

l000012C4:
	mov	r0,r5
	pop	{r3-r7,pc}
000012C8                         00 20 70 47 C4 00 00 20         . pG... 
000012D0 CC 00 00 20                                     ...            

;; pvTaskIncrementMutexHeldCount: 000012D4
pvTaskIncrementMutexHeldCount proc
	ldr	r3,[pc,#&10]                                           ; 000012EC
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
000012E6                   00 BF C4 00 00 20                   .....    

;; fn000012EC: 000012EC
fn000012EC proc
	mov	r0,r0
	mov	r0,r0

;; prvRestoreContextOfFirstTask: 000012F0
prvRestoreContextOfFirstTask proc
	ldr	r0,[pc,#&430]                                          ; 00001728
	ldr	r0,[r0]
	ldr	r0,[r0]
	msr	cpsr,r0
	ldr	r3,[pc,#&30]                                           ; 00001334
	ldr	r1,[r3]
	ldr	r0,[r1]
	add	r1,r1,#4
	ldr	r2,[pc,#&420]                                          ; 0000172E
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

;; fn00001330: 00001330
fn00001330 proc
	lsls	r0,r1,#3
	mov	r0,#0

;; prvSVCHandler: 00001334
prvSVCHandler proc
	ldr	r3,[r0,#&18]
	ldrb.w	r3,[r3,-#&2]
	cmps	r3,#1
	beq	$0000135C

l0000133E:
	blo	$00001350

l00001340:
	cmps	r3,#2
	bne	$0000134E

l00001344:
	mrs	r1,cpsr
	bic	r1,r1,#1
	msr	cpsr,r1

l0000134E:
	ldrh	r4,[r2]

l00001350:
	bx	lr
00001352       70 47 07 4A 13 68 43 F0 3E 43               pG.J.hC.>C   

l0000135C:
	str	r3,[r2]
	b	$000012EC
00001360 4F F0 80 52 04 4B 1A 60 BF F3 4F 8F BF F3 6F 8F O..R.K.`..O...o.
00001370 70 47 00 BF 1C ED 00 E0                         pG......       

;; fn00001378: 00001378
fn00001378 proc
	Invalid

;; pxPortInitialiseStack: 0000137C
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
xPortStartScheduler proc
	ldr	r3,[pc,#&134]                                          ; 000014EC
	push	{r4-r6}
	ldr	r2,[r3]
	ldr	r1,[pc,#&134]                                          ; 000014F2
	orr	r2,r2,#&FF0000
	str	r2,[r3]
	ldr	r2,[r3]
	orr	r2,r2,#&FF000000
	str	r2,[r3]
	ldr	r3,[r1]
	cmp	r3,#&800
	beq	$000013FC

l000013CE:
	mov	r5,#&4E1F
	mov	r1,#7
	mov	r0,#0
	ldr	r4,[pc,#&118]                                          ; 000014F6
	ldr	r2,[pc,#&118]                                          ; 000014F8
	ldr	r3,[pc,#&11C]                                          ; 000014FE
	str	r5,[r4]
	str	r1,[r2]
	str	r0,[r3]
	ldr	r0,[pc,#&340]                                          ; 0000172A
	ldr	r0,[r0]
	ldr	r0,[r0]
	msr	cpsr,r0
	Invalid
	Invalid
	dsb	sy
	isb	sy
	svc	#0
	nop

l000013FC:
	pop	{r4-r6}
	bx	lr
00001400 3E 48 3F 49 3F 4B 09 1A 40 F0 10 02 20 29 1A 60 >H?I?K..@... ).`
00001410 65 D9 40 23 05 22 02 E0 01 32 1F 2A 57 D0 99 42 e.@#."...2.*W..B
00001420 4F EA 43 03 F8 D8 38 4B 43 EA 42 02 37 49 38 4C O.C...8KC.B.7I8L
00001430 09 1A 34 4B 40 F0 11 00 20 29 22 60 18 60 4C D9 ..4K@... )"`.`L.
00001440 40 23 05 22 02 E0 01 32 1F 2A 42 D0 99 42 4F EA @#."...2.*B..BO.
00001450 43 03 F8 D8 2F 4B 43 EA 42 02 2F 4B 2F 49 2C 4D C.../KC.B./K/I,M
00001460 28 48 C9 1A 43 F0 12 04 20 29 2A 60 04 60 38 D9 (H..C... )*`.`8.
00001470 40 23 05 22 02 E0 01 32 1F 2A 2C D0 99 42 4F EA @#."...2.*,..BO.
00001480 43 03 F8 D8 26 48 40 EA 42 00 05 23 40 22 20 4E C...&H@.B..#@" N
00001490 1C 4C 24 4D 24 49 30 60 25 60 01 33 1F 2B 4F EA .L$M$I0`%`.3.+O.
000014A0 42 02 12 D0 8A 42 F8 D9 20 4A 42 EA 43 03 18 4A B....B.. JB.C..J
000014B0 1F 49 13 60 0B 68 43 F4 80 33 0B 60 52 F8 0C 3C .I.`.hC..3.`R..<
000014C0 43 F0 05 03 42 F8 0C 3C 81 E7 1A 4B EF E7 1A 4A C...B..<...K...J
000014D0 AC E7 1A 4A C1 E7 1A 48 D7 E7 1A 4A BD E7 1A 4A ...J...H...J...J
000014E0 A4 E7 1A 48 D1 E7 00 BF 20 ED 00 E0 90 ED 00 E0 ...H.... .......
000014F0 14 E0 00 E0 10 E0 00 E0 BC 00 00 20 00 00 00 00 ........... ....
00001500 00 00 02 00 9C ED 00 E0 01 00 07 06 00 80 00 00 ................
00001510 A0 ED 00 E0 01 00 07 05 00 00 00 20 00 02 00 20 ........... ... 
00001520 01 00 07 01 13 00 00 40 FE FF FF 1F 01 00 00 13 .......@........
00001530 24 ED 00 E0 3F 00 00 13 3F 00 07 06 3F 00 07 05 $...?...?...?...
00001540 3F 00 07 01 09 00 07 05 09 00 07 06 09 00 07 01 ?...............

;; vPortEndScheduler: 00001550
vPortEndScheduler proc
	bx	lr
00001552       00 BF                                       ..           

;; vPortStoreTaskMPUSettings: 00001554
vPortStoreTaskMPUSettings proc
	push	{r4-r5}
	cmps	r1,#0

l00001558:
	beq	$000015DA

l0000155A:
	cbnz	r3,$000015B0

l0000155C:
	mov	r5,#5
	ldr	r4,[r1,#&4]
	cbz	r4,$000015A2

l00001562:
	ldr	r3,[r1]
	orr	r2,r5,#&10
	orrs	r3,r2
	cmps	r4,#&20
	str	r3,[r0,#&8]
	bls	$0000164C

l00001570:
	mov	r2,#&40
	mov	r3,#5
	b	$00001578
00001576                   01 33                               .3       

l00001578:
	cmps	r3,#&1F
	beq	$000015A8

l0000157C:
	cmps	r4,r2
	mov.w	r2,r2,lsl #1

l00001582:
	bhi	$00001772

l00001584:
	lsls	r3,r3,#1
	ldr	r2,[r1,#&8]
	orr	r2,r2,#1

l0000158C:
	orrs	r3,r2
	str	r3,[r0,#&C]
	adds	r5,#1
	cmps	r5,#8
	add	r1,r1,#&C
	add	r0,r0,#8
	bne	$0000175A

l0000159E:
	pop	{r4-r5}
	bx	lr

l000015A2:
	orr	r3,r5,#&10
	str	r4,[r0,#&C]

l000015A8:
	str	r3,[r0,#&8]
	b	$0000158C
000015AC                                     3E 23 EA E7             >#..

l000015B0:
	lsls	r3,r3,#2
	orr	r2,r2,#&14
	cmps	r3,#&20
	str	r2,[r0]
	bls	$00001650

l000015BC:
	mov	r2,#&40
	mov	r4,#5
	b	$000015C4
000015C2       01 34                                       .4           

l000015C4:
	cmps	r4,#&1F
	beq	$000015D6

l000015C8:
	cmps	r3,r2
	mov.w	r2,r2,lsl #1
	bhi	$000017BE

l000015D0:
	ldr	r3,[pc,#&8C]                                           ; 00001664

l000015D2:
	orr	r4,r3,r4,lsl #1

l000015D6:
	str	r4,[r0,#&4]
	b	$00001558

l000015DA:
	ldr	r4,[pc,#&88]                                           ; 0000166A
	b	$000015D2
000015DE                                           22 4B               "K
000015E0 22 49 43 F0 14 02 C9 1A 20 29 02 60 36 D9 40 23 "IC..... ).`6.@#
000015F0 05 22 02 E0 01 32 1F 2A 26 D0 8B 42 4F EA 43 03 ."...2.*&..BO.C.
00001600 F8 D3 17 4B 43 EA 42 02 19 4B 1A 49 43 F0 15 04 ...KC.B..K.IC...
00001610 C9 1A 20 29 42 60 84 60 1E D9 05 22 40 23 02 E0 .. )B`.`..."@#..
00001620 01 32 1F 2A 12 D0 99 42 4F EA 43 03 F8 D8 12 4B .2.*...BO.C....K

l00001630:
	orr	r2,r3,r2,lsl #1
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
00001648                         06 4A DD E7                     .J..   

l0000164C:
	ldr	r2,[pc,#&2C]                                           ; 00001680
	b	$00001630

l00001650:
	mov	r3,#8
	b	$00001582
00001654             0A 4C BE E7 0A 4A EB E7 08 4A D3 E7     .L...J...J..
00001660 01 00 07 03 3F 00 07 03 00 00 00 20 00 20 00 20 ....?...... . . 
00001670 00 00 00 20 00 02 00 20 01 00 07 01 3F 00 07 01 ... ... ....?...
00001680 09 00 07 03 09 00 07 01                         ........       

;; xPortPendSVHandler: 00001688
xPortPendSVHandler proc
	mrs	r0,cpsr
	ldr	r3,[pc,#&50]                                           ; 000016E4
	ldr	r2,[r3]
	mrs	r1,cpsr
	stmdb	r0!,{r1,r4-fp}
	str	r0,[r2]
	push.w	{r3,lr}
	mov	r0,#&BF
	msr	cpsr,r0
	bl	$000010D4
	mov	r0,#0
	msr	cpsr,r0
	pop.w	{r3,lr}
	ldr	r1,[r3]
	ldr	r0,[r1]
	add	r1,r1,#4
	ldr	r2,[pc,#&68]                                           ; 0000172E
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
	bl	$00000D68
	cbz	r0,$00001708
	mov	r2,#&10000000
	ldr	r3,[pc,#&8]                                            ; 00001714
	str	r2,[r3]
	msr	cpsr,r4
	pop	{r4,pc}
	nop
	Invalid

;; vPortSVCHandler: 00001714
vPortSVCHandler proc
	tst	lr,#4
	ite	eq
	mrseq	r0,cpsr

l0000171E:
	mrs	r0,cpsr
	b	$00001330
00001724             08 ED 00 E0                             ....       

;; fn00001728: 00001728
fn00001728 proc
	Invalid

;; pvPortMalloc: 0000172C
pvPortMalloc proc
	push	{r4,lr}
	mov	r4,r0
	lsls	r3,r0,#&1D
	itt	ne
	bicne	r4,r0,#7

l00001738:
	adds	r4,#8
	bl	$00000A08
	ldr	r3,[pc,#&3C]                                           ; 00001782

;; fn00001740: 00001740
fn00001740 proc
	ldr	r2,[r3]
	cbz	r2,$00001770

l00001744:
	mov	r1,#&5B3
	ldr	r2,[r3,#&5C0]
	adds	r4,r2
	cmps	r4,r1
	bhi	$00001762

l00001752:
	cmps	r2,r4
	bhs	$00001762

l00001756:
	ldr	r1,[r3]
	str	r4,[r3,#&5C0]

;; fn0000175A: 0000175A
fn0000175A proc
	cmps	r8,r8
	add	r4,r1,r2
	bl	$00000E68

l00001762:
	mov	r0,r4
	pop	{r4,pc}
00001766                   00 24 FF F7 80 FB 20 46 10 BD       .$.... F..

l00001770:
	add	r2,r3,#&C

;; fn00001772: 00001772
fn00001772 proc
	lsls	r4,r1,#8
	bic	r2,r2,#7
	str	r2,[r3]
	b	$00001740

;; fn0000177C: 0000177C
fn0000177C proc
	lsls	r0,r6,#8
	mov	r0,#0

;; vPortFree: 00001780
vPortFree proc
	bx	lr
00001782       00 BF                                       ..           

;; vPortInitialiseBlocks: 00001784
vPortInitialiseBlocks proc
	mov	r2,#0
	ldr	r3,[pc,#&8]                                            ; 00001796
	str	r2,[r3,#&5C0]
	bx	lr
0000178E                                           00 BF               ..

;; fn00001790: 00001790
fn00001790 proc
	lsls	r0,r6,#8
	mov	r0,#0

;; xPortGetFreeHeapSize: 00001794
xPortGetFreeHeapSize proc
	ldr	r3,[pc,#&C]                                            ; 000017A8
	ldr	r0,[r3,#&5C0]
	rsb	r0,r0,#&5B0
	adds	r0,#4
	bx	lr
000017A2       00 BF                                       ..           

;; fn000017A4: 000017A4
fn000017A4 proc
	lsls	r0,r6,#8
	mov	r0,#0

;; xEventGroupCreate: 000017A8
xEventGroupCreate proc
	push	{r4,lr}
	mov	r0,#&18
	bl	$00001728
	mov	r4,r0
	cbz	r0,$000017BE

l000017B4:
	mov	r3,#0
	str	r3,[r0],#&4
	bl	$000082CC

;; fn000017BE: 000017BE
fn000017BE proc
	mov	r0,r4

;; fn000017C0: 000017C0
fn000017C0 proc
	pop	{r4,pc}
000017C2       00 BF                                       ..           

;; xEventGroupWaitBits: 000017C4
xEventGroupWaitBits proc
	push.w	{r4-r8,lr}
	mov	r6,r0
	mov	r7,r3
	mov	r5,r1
	mov	r8,r2
	bl	$00000A08
	ldr	r4,[r6]
	cbnz	r7,$000017F2

l000017D8:
	adcs	r4,r5
	beq	$000017F4

l000017DC:
	cmp	r8,#0
	beq	$000017E4

l000017E2:
	bic.w	r5,r4,r5

l000017E4:
	lsls	r5,r0,#&14

l000017E6:
	str	r5,[r6]
	bl	$00000E68
	mov	r0,r4
	pop.w	{r4-r8,pc}

l000017F2:
	bics.w	r3,r5,r4

l000017F4:
	lsls	r4,r0,#&C

l000017F6:
	beq	$000019D8

l000017F8:
	ldr	r3,[sp,#&18]
	cmps	r3,#0
	beq	$000019E4

l000017FE:
	cmp	r8,#0
	ite	eq
	moveq	r1,#0

l00001806:
	mov	r1,#&1000000

l00001808:
	strb	r0,[r0,#&6]
	cbnz	r7,$0000183E

l0000180C:
	orrs	r1,r5
	ldr	r2,[sp,#&18]
	add	r0,r6,#4
	bl	$00000FF4
	bl	$00000E68
	cbnz	r0,$0000182C

l0000181C:
	mov	r2,#&10000000
	ldr	r3,[pc,#&4C]                                           ; 00001874
	str	r2,[r3]
	dsb	sy
	isb	sy

l0000182C:
	bl	$0000111C
	lsls	r3,r0,#6
	mov	r4,r0
	bpl	$00001840

l00001836:
	bic	r0,r4,#&FF000000
	pop.w	{r4-r8,pc}

l0000183E:
	orr	r1,r1,#&4000000

l00001840:
	str	r0,[r0,#&18]

l00001842:
	b	$00001808
00001844             06 F0 98 FE 34 68 6F B9 25 42 05 D0     ....4ho.%B..
00001850 B8 F1 00 0F 02 D0 24 EA 05 05 35 60 06 F0 A8 FE ......$...5`....
00001860 24 F0 7F 40 BD E8 F0 81 35 EA 04 03 F6 D1 EF E7 $..@....5.......

;; fn00001870: 00001870
fn00001870 proc
	Invalid

;; xEventGroupClearBits: 00001874
xEventGroupClearBits proc
	push	{r4-r6,lr}
	mov	r6,r0
	mov	r4,r1
	bl	$00008574
	ldr	r5,[r6]
	bic.w	r4,r5,r4
	str	r4,[r6]
	bl	$000085AC
	mov	r0,r5

;; fn0000188C: 0000188C
fn0000188C proc
	pop	{r4-r6,pc}
0000188E                                           00 BF               ..

;; xEventGroupSetBits: 00001890
xEventGroupSetBits proc
	push	{r3-r7,lr}
	mov	r5,r0
	mov	r4,r1
	bl	$00000A08
	ldr	r1,[r5]
	ldr	r0,[r5,#&10]
	add	r6,r5,#&C
	orrs	r1,r4
	cmps	r6,r0
	str	r1,[r5]
	beq	$000018EC

l000018AA:
	mov	r7,#0
	b	$000018C4
000018AE                                           0A 42               .B
000018B0 07 D0 DB 01 00 D5 17 43 41 F0 00 71 FF F7 E0 FB .......CA..q....
000018C0 29 68 A6 42                                     )h.B           

l000018C4:
	mov	r0,r4
	beq	$000018DE

l000018C8:
	ldm	r0,{r3-r4}
	tst	r3,#&4000000
	bic	r2,r3,#&FF000000
	beq	$00001AAA

l000018D6:
	bics.w	lr,r2,r1
	beq	$00001AAE

l000018DC:
	cmps	r6,r4

l000018DE:
	mov	r0,r4

;; fn000018E0: 000018E0
fn000018E0 proc
	bne	$00001AC4

l000018E2:
	Invalid
	ands	r1,r7
	str	r1,[r5]
	bl	$00000E68

;; fn000018EC: 000018EC
fn000018EC proc
	ldr	r0,[r5]
	pop	{r3-r7,pc}
000018F0 4F F0 FF 37                                     O..7           

;; fn000018F4: 000018F4
fn000018F4 proc
	b	$000018E0
000018F6                   00 BF                               ..       

;; xEventGroupSync: 000018F8
xEventGroupSync proc
	push.w	{r4-r8,lr}
	mov	r8,r1
	mov	r5,r0
	mov	r6,r2
	mov	r7,r3
	bl	$00000A08
	mov	r1,r8
	ldr	r4,[r5]
	mov	r0,r5
	orrs	r4,r1
	bl	$0000188C
	bics.w	r3,r6,r4
	beq	$0000195A

l0000191A:
	cbnz	r7,$00001928

l0000191C:
	ldr	r4,[r5]
	bl	$00000E68
	mov	r0,r4
	pop.w	{r4-r8,pc}

l00001928:
	mov	r2,r7
	orr	r1,r6,#&5000000
	add	r0,r5,#4
	bl	$00000FF4
	bl	$00000E68
	cbnz	r0,$0000194A

l0000193A:
	mov	r2,#&10000000
	ldr	r3,[pc,#&44]                                           ; 0000198A
	str	r2,[r3]
	dsb	sy
	isb	sy

l0000194A:
	bl	$0000111C
	lsls	r3,r0,#6
	mov	r4,r0
	bpl	$00001964

l00001954:
	bic	r4,r4,#&FF000000
	mov	r0,r4

l0000195A:
	pop.w	{r4-r8,pc}
0000195E                                           2B 68               +h
00001960 23 EA 06 06                                     #...           

l00001964:
	str	r6,[r5]
	b	$0000191A
00001968                         06 F0 06 FE 2C 68 36 EA         ....,h6.
00001970 04 03 04 BF 24 EA 06 06 2E 60 06 F0 19 FE 24 F0 ....$....`....$.
00001980 7F 44 E9 E7 04 ED 00 E0                         .D......       

;; xEventGroupGetBitsFromISR: 00001988
xEventGroupGetBitsFromISR proc
	mrs	r3,cpsr
	mov	r2,#&BF
	msr	cpsr,r2
	isb	sy
	dsb	sy
	msr	cpsr,r3

;; fn000019A0: 000019A0
fn000019A0 proc
	ldr	r0,[r0]
	bx	lr

;; vEventGroupDelete: 000019A4
vEventGroupDelete proc
	push	{r4,lr}
	mov	r4,r0
	bl	$00000A08
	ldr	r3,[r4,#&4]
	cbz	r3,$000019C0

l000019B0:
	mov	r1,#&2000000
	ldr	r0,[r4,#&10]
	bl	$0000107C
	ldr	r3,[r4,#&4]
	cmps	r3,#0
	bne	$00001BAC

l000019C0:
	mov	r0,r4
	bl	$0000177C
	pop.w	{r4,lr}
	b	$00000E68
000019CE                                           00 BF               ..

;; vEventGroupSetBitsCallback: 000019D0
vEventGroupSetBitsCallback proc
	b	$0000188C

;; vEventGroupClearBitsCallback: 000019D4
vEventGroupClearBitsCallback proc
	push	{r3-r5,lr}
	mov	r4,r0

l000019D8:
	mov	r5,r1
	bl	$00008574
	ldr	r3,[r4]
	bic.w	r3,r3,r5

l000019E4:
	str	r3,[r4]
	pop.w	{r3-r5,lr}
	b	$00C085AC
000019EE                                           00 BF               ..
000019F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00001AA0 00 00 00 00 00 00 00 00 00 00                   ..........     

l00001AAA:
	mov	r0,r0
	mov	r0,r0

l00001AAE:
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0

;; fn00001AC4: 00001AC4
fn00001AC4 proc
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0

;; fn00001BAC: 00001BAC
fn00001BAC proc
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0
	mov	r0,r0

;; fn00007FFC: 00007FFC
fn00007FFC proc
	mov	r0,r0
	mov	r0,r0
;;; Segment .text (00008000)

;; NmiSR: 00008000
NmiSR proc
	b	$00007FFC
00008002       00 BF                                       ..           

;; FaultISR: 00008004
FaultISR proc
	b	$00008000
00008006                   00 BF                               ..       

;; ResetISR: 00008008
ResetISR proc
	ldr	r3,[pc,#&20]                                           ; 00008030
	ldr	r0,[pc,#&24]                                           ; 00008036
	cmps	r3,r0
	bhs	$00008022

l00008010:
	Invalid
	mov	r1,#0
	adds	r2,r0
	bic	r2,r2,#3
	adds	r2,#4
	adds	r2,r3
	str	r1,[r3],#&4

l00008022:
	cmps	r3,r2
	bne	$0000821A

l00008026:
	b	$00C0809C
0000802A                               00 BF 60 01 00 20           ..`.. 

l00008030:
	lsrs	r0,r0,#2
	mov	r0,#0

;; raise: 00008034
raise proc
	b	$00008030
00008036                   00 BF                               ..       

;; vPrintTask: 00008038
vPrintTask proc
	push	{r4-r5,lr}
	mov	r4,#0

l0000803C:
	ldr	r5,[pc,#&24]                                           ; 00008068
	sub	sp,#&C
	add	r1,sp,#4
	adds	r4,#1
	mov	r3,#0
	mov	r2,#&FFFFFFFF
	ldr	r0,[r5]
	bl	$00008B68
	bl	$0000977C
	and	r2,r4,#1
	and	r1,r4,#&3F
	ldr	r0,[sp,#&4]
	bl	$000097C8
	b	$0000803C
00008064             80 08 00 20                             ...        

;; vCheckTask: 00008068
vCheckTask proc
	push	{r4-r5,lr}
	ldr	r3,[pc,#&2C]                                           ; 0000809E
	sub	sp,#&C
	str	r3,[sp,#&4]
	bl	$00008900
	add	r4,sp,#8
	ldr	r5,[pc,#&24]                                           ; 000080A2

l00008078:
	str	r0,[r4,-#&8]!
	mov	r0,r4
	mov	r1,#&1388
	bl	$00008870
	mov	r3,#0
	mov	r2,#&FFFFFFFF
	add	r1,sp,#4
	ldr	r0,[r5]
	bl	$00008AE0
	b	$00008078
00008096                   00 BF 50 A2 00 00 80 08 00 20       ..P...... 

;; Main: 000080A0
Main proc
	push	{lr}
	mov	r2,#0
	sub	sp,#&C
	mov	r1,#4
	mov	r0,#3
	mov	r4,#0
	bl	$00008A84
	ldr	r3,[pc,#&3C]                                           ; 000080F4
	str	r0,[r3]
	mov	r0,r4
	bl	$000098EC
	mov	r2,#3
	mov	r3,r4
	str	r2,[sp]
	ldr	r1,[pc,#&30]                                           ; 000080F8
	mov	r2,#&3B
	str	r4,[sp,#&4]
	ldr	r0,[pc,#&30]                                           ; 000080FE
	bl	$00008804
	mov	r2,#2
	ldr	r1,[pc,#&2C]                                           ; 00008102
	mov	r3,r4
	str	r2,[sp]
	str	r4,[sp,#&4]
	mov	r2,#&3B
	ldr	r0,[pc,#&24]                                           ; 00008104
	bl	$00008804
	bl	$0000098C
	mov	r2,r4
	mov	r1,r4
	ldr	r0,[pc,#&1C]                                           ; 0000810A

l000080E8:
	bl	$000097C8
	b	$000080E8
000080EE                                           00 BF               ..
000080F0 80 08 00 20 58 A2 00 00 69 80 00 00 60 A2 00 00 ... X...i...`...
00008100 39 80 00 00 68 A2 00 00                         9...h...       

;; vUART_ISR: 00008108
vUART_ISR proc
	push	{r4-r6,lr}
	mov	r6,#0
	ldr	r5,[pc,#&64]                                           ; 00008178
	sub	sp,#8
	mov	r1,#1
	mov	r0,r5
	str	r6,[sp,#&4]
	bl	$0000A0C8
	mov	r4,r0
	mov	r1,r0
	mov	r0,r5
	bl	$0000A0D4
	lsls	r2,r4,#&1B
	bpl	$0000812C

l00008128:
	ldr	r3,[pc,#&4C]                                           ; 0000817C
	ldr	r3,[r3]

l0000812C:
	lsls	r3,r3,#&19
	bmi	$0000815A

l00008130:
	lsls	r0,r4,#&1A
	bpl	$00008138

l00008134:
	ldr	r2,[pc,#&44]                                           ; 00008180
	ldrb	r3,[r2]

l00008138:
	cmps	r3,#&7A
	bls	$00008148

l0000813C:
	ldr	r3,[sp,#&4]
	cbz	r3,$00008148

l00008140:
	mov	r2,#&10000000
	ldr	r3,[pc,#&38]                                           ; 00008184
	str	r2,[r3]

l00008148:
	add	sp,#8
	pop	{r4-r6,pc}
0000814C                                     0A 49 09 68             .I.h
00008150 89 06 5C BF 07 49 0B 60 01 33                   ..\..I.`.3     

l0000815A:
	strb	r3,[r2]
	b	$00008138
0000815E                                           2D 68               -h
00008160 33 46 30 46 01 AA 0D F1 03 01 8D F8 03 50 F8 F7 3F0F.........P..
00008170 73 F9 DD E7 00 C0 00 40 18 C0 00 40 2C 02 00 20 s......@...@,.. 
00008180 04 ED 00 E0                                     ....           

;; vSetErrorLED: 00008184
vSetErrorLED proc
	mov	r1,#1
	mov	r0,#7

;; fn00008188: 00008188
fn00008188 proc
	b	$00C085F0

;; prvSetAndCheckRegisters: 0000818C
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
	bne	$000081FC

l000081C6:
	cmps	r0,#&B
	bne	$000081FC

l000081CA:
	cmps	r1,#&C
	bne	$000081FC

l000081CE:
	cmps	r2,#&D
	bne	$000081FC

l000081D2:
	cmps	r3,#&E
	bne	$000081FC

l000081D6:
	cmps	r4,#&F
	bne	$000081FC

l000081DA:
	cmps	r5,#&10
	bne	$000081FC

l000081DE:
	cmps	r6,#&11
	bne	$000081FC

l000081E2:
	cmps	r7,#&12
	bne	$000081FC

l000081E6:
	cmp	r8,#&13
	bne	$000081FC

l000081EC:
	cmp	r9,#&14
	bne	$000081FC

l000081F2:
	cmp	r10,#&15
	bne	$000081FC

l000081F8:
	cmp	ip,#&16

l000081FC:
	bne	$000081FC

l000081FE:
	bx	lr
00008200 00 B5 06 49 88 47 5D F8 04 EB 70 47             ...I.G]...pG   

;; fn0000820C: 0000820C
fn0000820C proc
	bx	lr

l0000820E:
	nop

;; vApplicationIdleHook: 00008210
vApplicationIdleHook proc
	push	{r3,lr}
	bl	$00008F28
	bl	$00008188

;; fn0000821A: 0000821A
fn0000821A proc
	b	$0000820E

;; fn0000821C: 0000821C
fn0000821C proc
	strh	r5,[r0,#&18]
	mov	r0,r0

;; PDCInit: 00008220
PDCInit proc
	push	{r4-r5,lr}
	ldr	r0,[pc,#&68]                                           ; 00008292
	sub	sp,#&C
	bl	$00009B78
	ldr	r0,[pc,#&64]                                           ; 00008296
	bl	$00009B78
	mov	r2,#2
	mov	r1,#&34
	mov	r0,#&40004000
	bl	$00009108
	mov	r2,#1
	mov	r1,#8
	mov	r0,#&40004000
	bl	$00009108
	mov	r3,#&A
	mov	r2,#2
	mov	r1,#4
	mov	r0,#&40004000
	bl	$000091C4
	mov	r4,#8
	mov	r2,#0
	ldr	r5,[pc,#&38]                                           ; 0000829A
	mov	r1,r2
	ldr	r3,[pc,#&38]                                           ; 0000829E
	mov	r0,r5
	str	r4,[sp]
	bl	$000099E4
	mov	r0,r5
	bl	$00009A30
	mov	r1,r4
	mov	r2,#0
	mov	r0,#&40004000
	bl	$00009450
	mov	r2,r4
	mov	r1,r4
	mov	r0,#&40004000
	add	sp,#&C
	pop.w	{r4-r5,lr}
	b	$00C09450
0000828C                                     10 00 00 10             ....
00008290 01 00 00 20 00 80 00 40                         ... ...@       

;; fn00008298: 00008298
fn00008298 proc
	rsbs	r0,r0
	mov	r7,r1

;; PDCWrite: 0000829C
PDCWrite proc
	push	{r4-r5,lr}
	mov	r5,r1
	ldr	r4,[pc,#&28]                                           ; 000082D0
	sub	sp,#&C
	and	r1,r0,#&F
	mov	r0,r4
	bl	$00009A94
	mov	r1,r5
	mov	r0,r4
	bl	$00009A94
	mov	r0,r4
	add	r1,sp,#4
	bl	$00009AB4
	add	r1,sp,#4
	mov	r0,r4
	bl	$00009AB4
	add	sp,#&C
	pop	{r4-r5,pc}
000082CA                               00 BF                       ..   

;; fn000082CC: 000082CC
fn000082CC proc
	strh	r0,[r0]
	ands	r0,r0

;; vListInitialise: 000082D0
vListInitialise proc
	mov	r1,#&FFFFFFFF
	mov	r2,#0
	add	r3,r0,#8
	str	r1,[r0,#&8]
	stm	r0,{r2-r3}
	str	r3,[r0,#&C]
	str	r3,[r0,#&10]

;; fn000082E4: 000082E4
fn000082E4 proc
	bx	lr
000082E6                   00 BF                               ..       

;; vListInitialiseItem: 000082E8
vListInitialiseItem proc
	mov	r3,#0
	str	r3,[r0,#&10]

;; fn000082EC: 000082EC
fn000082EC proc
	bx	lr
000082EE                                           00 BF               ..

;; vListInsertEnd: 000082F0
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

;; fn00008308: 00008308
fn00008308 proc
	str	r2,[r0]
	bx	lr

;; vListInsert: 0000830C
vListInsert proc
	push	{r4-r5}
	ldr	r5,[r1]
	add	r3,r5,#1
	beq	$00008334

l00008314:
	add	r2,r0,#8

l00008318:
	b	$00008318
0000831A                               1A 46 53 68 1C 68           .FSh.h

l00008320:
	cmps	r5,r4
	bhs	$00008516

l00008324:
	ldr	r4,[r0]
	str	r3,[r1,#&4]
	adds	r4,#1
	str	r1,[r3,#&8]
	str	r2,[r1,#&8]
	str	r1,[r2,#&4]
	str	r0,[r1,#&10]
	str	r4,[r0]

l00008334:
	pop	{r4-r5}
	bx	lr
00008338                         02 69 53 68                     .iSh   

;; fn0000833C: 0000833C
fn0000833C proc
	b	$00008320
0000833E                                           00 BF               ..

;; uxListRemove: 00008340
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
xQueueCRSend proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r4,r2
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	bl	$00008574
	ldr	r2,[r5,#&38]
	ldr	r3,[r5,#&3C]
	cmps	r2,r3
	beq	$000083AE
	bl	$000085AC
	mov	r0,#0
	msr	cpsr,r0
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[r5,#&38]
	ldr	r3,[r5,#&3C]

l000083A6:
	cmps	r2,r3
	blo	$000083BC

l000083AA:
	mov	r3,#0
	msr	cpsr,r3
	pop	{r4-r6,pc}
000083B2       00 F0 FD F8 7C B9 84 F3 11 88               ....|.....   

l000083BC:
	mov	r0,r4
	pop	{r4-r6,pc}
000083C0 02 46 31 46 28 46 F7 F7 91 FE 6B 6A 7B B9 01 20 .F1F(F....kj{.. 
000083D0 00 23 83 F3 11 88 70 BD 05 F1 10 01 20 46 00 F0 .#....p..... F..
000083E0 87 FD 00 23 83 F3 11 88 6F F0 03 00 70 BD 05 F1 ...#....o...p...
000083F0 24 00 00 F0 4F FE 00 28 E9 D0 6F F0             $...O..(..o.   

;; fn000083FC: 000083FC
fn000083FC proc
	mov	r4,r0
	b	$000083A6

;; xQueueCRReceive: 00008400
xQueueCRReceive proc
	push	{r3-r5,lr}
	mov	r4,r0
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r5,[r0,#&38]
	cbnz	r5,$00008424
	cmps	r2,#0
	bne	$00008486
	msr	cpsr,r2
	mov	r0,r2
	pop	{r3-r5,pc}
	mov	r3,#0
	msr	cpsr,r3
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[r0,#&38]
	cbnz	r2,$00008448
	mov	r0,r2
	mov	r3,#0
	msr	cpsr,r3
	pop	{r3-r5,pc}
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
	add	r3,r3,#&FFFFFFFF
	str	r3,[r4,#&38]
	it	hs
	strhs	r1,[r4,#&C]
	bl	$0000A5C0
	ldr	r3,[r4,#&10]
	cbnz	r3,$00008478
	mov	r0,#1
	mov	r3,#0
	msr	cpsr,r3
	pop	{r3-r5,pc}
	add	r0,r4,#&10
	bl	$00009090
	cmps	r0,#0
	beq	$0000866A
	mvn	r0,#4
	b	$0000843C
	add	r1,r0,#&24
	mov	r0,r2
	bl	$00008EEC
	msr	cpsr,r5
	mvn	r0,#3
	pop	{r3-r5,pc}
	nop

;; xQueueCRSendFromISR: 000084A0
xQueueCRSendFromISR proc
	push	{r4-r6,lr}
	ldr	r3,[r0,#&3C]
	ldr	r6,[r0,#&38]
	mov	r5,r2
	cmps	r6,r3
	blo	$000084AC

l000084AC:
	mov	r0,r5
	pop	{r4-r6,pc}
000084B0 00 22 04 46 F7 F7 1A FE 00 2D F7 D1 63 6A 00 2B .".F.....-..cj.+
000084C0 F4 D0 04 F1 24 00 00 F0 E5 FD 05 1C 18 BF 01 25 ....$..........%
000084D0 EC E7 00 BF                                     ....           

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
	bl	$0000A5C0
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

l00008516:
	pop	{r3-r7,pc}

l00008518:
	add	r0,r4,#&10
	bl	$00009090
	cmps	r0,#0
	beq	$0000870C

l00008524:
	mov	r0,#1
	str	r0,[r5]
	pop	{r3-r7,pc}

l0000852A:
	nop

;; prvIdleTask: 0000852C
prvIdleTask proc
	push	{r3,lr}
	bl	$0000820C

;; fn00008530: 00008530
fn00008530 proc
	Invalid

;; fn00008532: 00008532
fn00008532 proc
	b	$0000852A

;; xTaskNotifyStateClear: 00008534
xTaskNotifyStateClear proc
	push	{r3-r5,lr}

l00008536:
	cbz	r0,$00008558

l00008538:
	mov	r4,r0
	bl	$00008574
	ldrb	r3,[r4,#&64]
	cmps	r3,#2
	ittet	eq
	moveq	r3,#0

l00008548:
	mov	r5,#1
	mov	r5,#0
	strb	r3,[r4,#&64]
	bl	$000085AC
	mov	r0,r5
	pop	{r3-r5,pc}

l00008558:
	ldr	r3,[pc,#&4]                                            ; 00008564
	ldr	r4,[r3,#&4]
	b	$00008536
0000855E                                           00 BF               ..

;; fn00008560: 00008560
fn00008560 proc
	lsls	r4,r0,#3
	mov	r0,#0

;; xPortRaisePrivilege: 00008564
xPortRaisePrivilege proc
	mrs	r0,cpsr
	tst	r0,#1
	itte	ne
	movne	r0,#0

l00008570:
	svc	#2
	mov	r0,#1

;; fn00008574: 00008574
fn00008574 proc
	bx	lr
00008576                   00 20                               .        

;; vPortEnterCritical: 00008578
vPortEnterCritical proc
	push	{r3,lr}
	bl	$00008560
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[pc,#&1C]                                           ; 000085B2
	cmps	r0,#1
	ldr	r3,[r2]
	add	r3,r3,#1
	str	r3,[r2]
	beq	$000085A4
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0
	pop	{r3,pc}
	nop

;; fn000085AC: 000085AC
fn000085AC proc
	lsls	r4,r7,#2
	mov	r0,#0

;; vPortExitCritical: 000085B0
vPortExitCritical proc
	push	{r3,lr}
	bl	$00008560
	ldr	r2,[pc,#&20]                                           ; 000085DE
	ldr	r3,[r2]
	subs	r3,#1
	str	r3,[r2]
	cbnz	r3,$000085C4

l000085C0:
	msr	cpsr,r3

l000085C4:
	cmps	r0,#1
	beq	$000085D0

l000085C8:
	mrs	r0,cpsr
	orr	r0,r0,#1

l000085D0:
	msr	cpsr,r0
	pop	{r3,pc}
000085D6                   00 BF BC 00 00 20                   .....    

;; vParTestInitialise: 000085DC
vParTestInitialise proc
	push	{r3,lr}
	bl	$0000821C
	ldr	r3,[pc,#&C]                                            ; 000085F6
	mov	r0,#5
	ldrb	r1,[r3]
	pop.w	{r3,lr}
	b	$00008298
000085F0 F4 07 00 20                                     ...            

;; vParTestSetLED: 000085F4
vParTestSetLED proc
	push	{r3-r5,lr}
	mov	r4,r0
	mov	r5,r1
	bl	$000088BC
	cmps	r4,#7
	bhi	$00008618

l00008602:
	mov	r3,#1
	lsl	r0,r3,r4
	ldr	r3,[pc,#&20]                                           ; 00008630
	uxtb	r0,r0
	ldrb	r2,[r3]
	cbz	r5,$00008624

l00008610:
	orrs	r0,r2
	strb	r0,[r3]
	ldrb	r1,[r3]
	mov	r0,#5

l00008618:
	bl	$00008298
	pop.w	{r3-r5,lr}
	b	$00C088DC

l00008624:
	bic.w	r0,r2,r0
	strb	r0,[r3]
	b	$00008610

;; fn0000862C: 0000862C
fn0000862C proc
	lsls	r4,r6,#&1F
	mov	r0,#0

;; vParTestToggleLED: 00008630
vParTestToggleLED proc
	push	{r4,lr}
	mov	r4,r0
	bl	$000088BC
	cmps	r4,#7
	bhi	$00008656

l0000863C:
	mov	r2,#1
	ldr	r3,[pc,#&2C]                                           ; 00008672
	lsl	r0,r2,r4
	ldrb	r1,[r3]
	uxtb	r2,r0
	adcs	r2,r1
	bne	$0000865E

l0000864C:
	ldrb	r1,[r3]
	orrs	r2,r1
	strb	r2,[r3]
	ldrb	r1,[r3]
	mov	r0,#5

l00008656:
	bl	$00008298
	pop.w	{r4,lr}

l0000865E:
	b	$00C088DC
00008662       1A 78 22 EA 00 00 18 70 F2 E7 F4 07 00 20   .x"....p..... 

;; prvFlashCoRoutine: 00008670
prvFlashCoRoutine proc
	push	{r4-r6,lr}
	ldrh	r3,[r0,#&68]
	sub	sp,#8
	cmp	r3,#&1C2
	mov	r4,r0
	beq	$000086B2

l0000867E:
	mov	r2,#&1C3
	cmps	r3,r2
	beq	$00008688

l00008686:
	cbz	r3,$000086D2

l00008688:
	add	sp,#8
	pop	{r4-r6,pc}
0000868C                                     14 4D 01 AE             .M..
00008690 01 98                                           ..             

l00008692:
	bl	$0000862C
	mov	r2,#&FFFFFFFF
	mov	r1,r6
	ldr	r0,[r5]
	bl	$000083FC
	add	r2,r0,#4
	beq	$000086D4

l000086A6:
	add	r3,r0,#5
	beq	$000086C4

l000086AA:
	cmps	r0,#1
	beq	$0000888C

l000086AE:
	mov	r2,#0
	ldr	r3,[pc,#&30]                                           ; 000086E8

l000086B2:
	str	r2,[r3]
	b	$00008692
000086B6                   0A 4D 01 AE 28 68 31 46 00 22       .M..(h1F."
000086C0 FF F7 9E FE                                     ....           

l000086C4:
	add	r3,r0,#5
	bne	$000088A6

l000086C8:
	mov	r3,#&1C3
	strh	r3,[r4,#&68]
	add	sp,#8
	pop	{r4-r6,pc}

l000086D2:
	ldr	r5,[pc,#&C]                                            ; 000086E6

l000086D4:
	add	r6,sp,#4
	b	$00008692
000086D8                         4F F4 E1 73 A3 86 D3 E7         O..s....
000086E0 F8 07 00 20 C0 00 00 20                         ... ...        

;; prvFixedDelayCoRoutine: 000086E8
prvFixedDelayCoRoutine proc
	push	{r4,lr}
	ldrh	r3,[r0,#&68]
	sub	sp,#8
	cmp	r3,#&182
	mov	r4,r0
	str	r1,[sp,#&4]
	beq	$0000874C

l000086F8:
	bls	$00008744

l000086FA:
	mov	r2,#&183
	cmps	r3,r2
	bne	$00008712

l00008702:
	ldr	r3,[pc,#&74]                                           ; 0000877E
	ldr	r2,[sp,#&4]
	ldr.w	r0,[r3,r2,lsl #2]

l00008708:
	mov	r2,r4
	cbnz	r0,$0000875E

;; fn0000870C: 0000870C
fn0000870C proc
	mov	r3,#&196
	strh	r3,[r4,#&68]

l00008712:
	add	sp,#8
	pop	{r4,pc}
00008716                   B3 F5 CB 7F FA D1 17 4B 00 22       .......K."
00008720 18 68 01 A9 FF F7 1E FE 02 1D 20 D0 43 1D 1A D0 .h........ .C...
00008730 01 28 E6 D0 00 22 12 4B 1A 60 0F 4B 01 9A 53 F8 .(...".K.`.K..S.
00008740 22 00 00 28                                     "..(           

l00008744:
	beq	$00008908

l00008746:
	b	$0000875A
00008748                         00 2B E7 D0                     .+..   

l0000874C:
	add	sp,#8
	pop	{r4,pc}
00008750 0A 4B 00 22 18 68 01 A9 FF F7                   .K.".h....     

l0000875A:
	Invalid

l0000875E:
	mov	r1,#0
	bl	$00008EEC
	b	$00008708
00008766                   40 F2 83 13 A3 86 D1 E7 4F F4       @.......O.
00008770 C1 73 A3 86 CD E7 00 BF 84 A2 00 00 F8 07 00 20 .s............. 
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
	bl	$00008A84
	ldr	r3,[pc,#&28]                                           ; 000087C8
	str	r0,[r3]
	cbz	r0,$000087C2

l0000879E:
	cbz	r5,$000087B4

l000087A0:
	mov	r4,#0
	ldr	r6,[pc,#&24]                                           ; 000087CE
	mov	r2,r4
	mov	r1,#0
	adds	r4,#1
	mov	r0,r6
	bl	$00008E3C
	cmps	r4,r5
	bne	$000089A0

l000087B4:
	mov	r2,#0
	pop.w	{r4-r6,lr}
	mov	r1,#1
	ldr	r0,[pc,#&C]                                            ; 000087D0
	b	$00C08E3C

l000087C2:
	pop	{r4-r6,pc}
000087C4             F8 07 00 20 E9 86 00 00 71 86 00 00     ... ....q...

;; xAreFlashCoRoutinesStillRunning: 000087D0
xAreFlashCoRoutinesStillRunning proc
	ldr	r3,[pc,#&4]                                            ; 000087DC
	ldr	r0,[r3]
	bx	lr
000087D6                   00 BF C0 00 00 20                   .....    

;; MPU_xTaskCreateRestricted: 000087DC
MPU_xTaskCreateRestricted proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00000918
	cmps	r4,#1
	mov	r3,r0
	beq	$000087FE

l000087F6:
	mrs	r0,cpsr
	orr	r0,r0,#1

l000087FE:
	msr	cpsr,r0
	mov	r0,r3

;; fn00008804: 00008804
fn00008804 proc
	pop	{r4-r6,pc}
00008806                   00 BF                               ..       

;; MPU_xTaskCreate: 00008808
MPU_xTaskCreate proc
	push.w	{r4-r10,lr}
	sub	sp,#8
	mov	r5,r0
	mov	r8,r1
	mov	r9,r2
	mov	r10,r3
	ldr	r7,[sp,#&28]
	ldr	r6,[sp,#&2C]
	bl	$00008560
	mov	r3,r10
	mov	r4,r0
	str	r7,[sp]
	str	r6,[sp,#&4]
	mov	r2,r9
	mov	r1,r8
	mov	r0,r5
	bl	$000008B0
	cmps	r4,#1
	mov	r3,r0
	beq	$0000883E

l00008836:
	mrs	r0,cpsr
	orr	r0,r0,#1

l0000883E:
	msr	cpsr,r0
	mov	r0,r3
	add	sp,#8
	pop.w	{r4-r10,pc}
0000884A                               00 BF                       ..   

;; MPU_vTaskAllocateMPURegions: 0000884C
MPU_vTaskAllocateMPURegions proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r4,r0
	mov	r1,r6
	mov	r0,r5
	bl	$0000096C
	cmps	r4,#1
	beq	$0000886C

l00008864:
	mrs	r0,cpsr
	orr	r0,r0,#1

l0000886C:
	msr	cpsr,r0

;; fn00008870: 00008870
fn00008870 proc
	pop	{r4-r6,pc}
00008872       00 BF                                       ..           

;; MPU_vTaskDelayUntil: 00008874
MPU_vTaskDelayUntil proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r4,r0
	mov	r1,r6
	mov	r0,r5
	bl	$00000F7C
	cmps	r4,#1
	beq	$00008894

;; fn0000888C: 0000888C
fn0000888C proc
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008894:
	msr	cpsr,r0
	pop	{r4-r6,pc}
0000889A                               00 BF                       ..   

;; MPU_vTaskDelay: 0000889C
MPU_vTaskDelay proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0

;; fn000088A6: 000088A6
fn000088A6 proc
	mov	r0,r5
	bl	$00000F44
	cmps	r4,#1
	beq	$000088B8

;; fn000088B0: 000088B0
fn000088B0 proc
	mrs	r0,cpsr
	orr	r0,r0,#1

l000088B8:
	msr	cpsr,r0

;; fn000088BC: 000088BC
fn000088BC proc
	pop	{r3-r5,pc}
000088BE                                           00 BF               ..

;; MPU_vTaskSuspendAll: 000088C0
MPU_vTaskSuspendAll proc
	push	{r4,lr}
	bl	$00008560
	mov	r4,r0
	bl	$00000A08
	cmps	r4,#1
	beq	$000088D8

l000088D0:
	mrs	r0,cpsr
	orr	r0,r0,#1

l000088D8:
	msr	cpsr,r0
	pop	{r4,pc}
000088DE                                           00 BF               ..

;; MPU_xTaskResumeAll: 000088E0
MPU_xTaskResumeAll proc
	push	{r4,lr}
	bl	$00008560
	mov	r4,r0
	bl	$00000E68
	cmps	r4,#1
	mov	r3,r0
	beq	$000088FA

l000088F2:
	mrs	r0,cpsr
	orr	r0,r0,#1

l000088FA:
	msr	cpsr,r0
	mov	r0,r3

;; fn00008900: 00008900
fn00008900 proc
	pop	{r4,pc}
00008902       00 BF                                       ..           

;; MPU_xTaskGetTickCount: 00008904
MPU_xTaskGetTickCount proc
	push	{r4,lr}
	bl	$00008560

;; fn00008908: 00008908
fn00008908 proc
	Invalid
	bl	$00000A1C
	cmps	r4,#1
	mov	r3,r0
	beq	$0000891E

l00008916:
	mrs	r0,cpsr
	orr	r0,r0,#1

l0000891E:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4,pc}
00008926                   00 BF                               ..       

;; MPU_uxTaskGetNumberOfTasks: 00008928
MPU_uxTaskGetNumberOfTasks proc
	push	{r4,lr}
	bl	$00008560
	mov	r4,r0
	bl	$00000A34
	cmps	r4,#1
	mov	r3,r0
	beq	$00008942

l0000893A:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008942:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4,pc}
0000894A                               00 BF                       ..   

;; MPU_pcTaskGetName: 0000894C
MPU_pcTaskGetName proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00000A40
	cmps	r4,#1
	mov	r3,r0
	beq	$0000896A

l00008962:
	mrs	r0,cpsr
	orr	r0,r0,#1

l0000896A:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r5,pc}
00008972       00 BF                                       ..           

;; MPU_vTaskSetTimeOutState: 00008974
MPU_vTaskSetTimeOutState proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00001140
	cmps	r4,#1
	beq	$00008990

l00008988:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008990:
	msr	cpsr,r0
	pop	{r3-r5,pc}
00008996                   00 BF                               ..       

;; MPU_xTaskCheckForTimeOut: 00008998
MPU_xTaskCheckForTimeOut proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560

;; fn000089A0: 000089A0
fn000089A0 proc
	Invalid
	mov	r4,r0
	mov	r0,r5
	bl	$00001154
	cmps	r4,#1
	mov	r3,r0
	beq	$000089BA

l000089B2:
	mrs	r0,cpsr
	orr	r0,r0,#1

l000089BA:
	msr	cpsr,r0
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
	bl	$00008560
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$00000A54
	cmps	r4,#1
	mov	r3,r0
	beq	$000089F0

l000089E8:
	mrs	r0,cpsr
	orr	r0,r0,#1

l000089F0:
	msr	cpsr,r0
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
	bl	$00008560
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$00000BD0
	cmps	r4,#1
	mov	r3,r0
	beq	$00008A28

l00008A20:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008A28:
	msr	cpsr,r0
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008A32       00 BF                                       ..           

;; MPU_ulTaskNotifyTake: 00008A34
MPU_ulTaskNotifyTake proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00000CFC
	cmps	r4,#1
	mov	r3,r0
	beq	$00008A56

l00008A4E:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008A56:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4-r6,pc}
00008A5E                                           00 BF               ..

;; MPU_xTaskNotifyStateClear: 00008A60
MPU_xTaskNotifyStateClear proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00008530
	cmps	r4,#1
	mov	r3,r0
	beq	$00008A7E

l00008A76:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008A7E:
	msr	cpsr,r0
	mov	r0,r3

;; fn00008A84: 00008A84
fn00008A84 proc
	pop	{r3-r5,pc}
00008A86                   00 BF                               ..       

;; MPU_xQueueGenericCreate: 00008A88
MPU_xQueueGenericCreate proc
	push	{r3-r7,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	bl	$00008560
	mov	r2,r7
	mov	r4,r0
	mov	r1,r6
	mov	r0,r5
	bl	$000006A8
	cmps	r4,#1
	mov	r3,r0
	beq	$00008AAE

l00008AA6:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008AAE:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r7,pc}
00008AB6                   00 BF                               ..       

;; MPU_xQueueGenericReset: 00008AB8
MPU_xQueueGenericReset proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$0000062C
	cmps	r4,#1
	mov	r3,r0
	beq	$00008ADA

l00008AD2:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008ADA:
	msr	cpsr,r0
	mov	r0,r3

;; fn00008AE0: 00008AE0
fn00008AE0 proc
	pop	{r4-r6,pc}
00008AE2       00 BF                                       ..           

;; MPU_xQueueGenericSend: 00008AE4
MPU_xQueueGenericSend proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008560
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$0000018C
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B10

l00008B08:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008B10:
	msr	cpsr,r0
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008B1A                               00 BF                       ..   

;; MPU_uxQueueMessagesWaiting: 00008B1C
MPU_uxQueueMessagesWaiting proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00000424
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B3A

l00008B32:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008B3A:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r5,pc}
00008B42       00 BF                                       ..           

;; MPU_uxQueueSpacesAvailable: 00008B44
MPU_uxQueueSpacesAvailable proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00000438
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B62

l00008B5A:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008B62:
	msr	cpsr,r0
	mov	r0,r3

;; fn00008B68: 00008B68
fn00008B68 proc
	pop	{r3-r5,pc}
00008B6A                               00 BF                       ..   

;; MPU_xQueueGenericReceive: 00008B6C
MPU_xQueueGenericReceive proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008560
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$000002D4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B98

l00008B90:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008B98:
	msr	cpsr,r0
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008BA2       00 BF                                       ..           

;; MPU_xQueuePeekFromISR: 00008BA4
MPU_xQueuePeekFromISR proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$000002A0
	cmps	r4,#1
	mov	r3,r0
	beq	$00008BC6

l00008BBE:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008BC6:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4-r6,pc}
00008BCE                                           00 BF               ..

;; MPU_xQueueGetMutexHolder: 00008BD0
MPU_xQueueGetMutexHolder proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$000005B0
	cmps	r4,#1
	mov	r3,r0
	beq	$00008BEE

l00008BE6:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008BEE:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r5,pc}
00008BF6                   00 BF                               ..       

;; MPU_xQueueCreateMutex: 00008BF8
MPU_xQueueCreateMutex proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$000006D8
	cmps	r4,#1
	mov	r3,r0
	beq	$00008C16

l00008C0E:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008C16:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r5,pc}
00008C1E                                           00 BF               ..

;; MPU_xQueueTakeMutexRecursive: 00008C20
MPU_xQueueTakeMutexRecursive proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$000005D0
	cmps	r4,#1
	mov	r3,r0
	beq	$00008C42

l00008C3A:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008C42:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4-r6,pc}
00008C4A                               00 BF                       ..   

;; MPU_xQueueGiveMutexRecursive: 00008C4C
MPU_xQueueGiveMutexRecursive proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00000600
	cmps	r4,#1
	mov	r3,r0
	beq	$00008C6A

l00008C62:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008C6A:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r5,pc}
00008C72       00 BF                                       ..           

;; MPU_vQueueDelete: 00008C74
MPU_vQueueDelete proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00000450
	cmps	r4,#1
	beq	$00008C90

l00008C88:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008C90:
	msr	cpsr,r0
	pop	{r3-r5,pc}
00008C96                   00 BF                               ..       

;; MPU_pvPortMalloc: 00008C98
MPU_pvPortMalloc proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$00001728
	cmps	r4,#1
	mov	r3,r0
	beq	$00008CB6

l00008CAE:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008CB6:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r3-r5,pc}
00008CBE                                           00 BF               ..

;; MPU_vPortFree: 00008CC0
MPU_vPortFree proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$0000177C
	cmps	r4,#1
	beq	$00008CDC

l00008CD4:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008CDC:
	msr	cpsr,r0
	pop	{r3-r5,pc}
00008CE2       00 BF                                       ..           

;; MPU_vPortInitialiseBlocks: 00008CE4
MPU_vPortInitialiseBlocks proc
	push	{r4,lr}
	bl	$00008560
	mov	r4,r0
	bl	$00001780
	cmps	r4,#1
	beq	$00008CFC

l00008CF4:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008CFC:
	msr	cpsr,r0
	pop	{r4,pc}
00008D02       00 BF                                       ..           

;; MPU_xPortGetFreeHeapSize: 00008D04
MPU_xPortGetFreeHeapSize proc
	push	{r4,lr}
	bl	$00008560
	mov	r4,r0
	bl	$00001790
	cmps	r4,#1
	mov	r3,r0
	beq	$00008D1E

l00008D16:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008D1E:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4,pc}
00008D26                   00 BF                               ..       

;; MPU_xEventGroupCreate: 00008D28
MPU_xEventGroupCreate proc
	push	{r4,lr}
	bl	$00008560
	mov	r4,r0
	bl	$000017A4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008D42

l00008D3A:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008D42:
	msr	cpsr,r0
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
	bl	$00008560
	mov	r3,r9
	mov	r4,r0
	str	r7,[sp]
	mov	r2,r8
	mov	r1,r6
	mov	r0,r5
	bl	$000017C0
	cmps	r4,#1
	mov	r3,r0
	beq	$00008D7E

l00008D76:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008D7E:
	msr	cpsr,r0
	mov	r0,r3
	add	sp,#&C
	pop.w	{r4-r9,pc}
00008D8A                               00 BF                       ..   

;; MPU_xEventGroupClearBits: 00008D8C
MPU_xEventGroupClearBits proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00001870
	cmps	r4,#1
	mov	r3,r0
	beq	$00008DAE

l00008DA6:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008DAE:
	msr	cpsr,r0
	mov	r0,r3
	pop	{r4-r6,pc}
00008DB6                   00 BF                               ..       

;; MPU_xEventGroupSetBits: 00008DB8
MPU_xEventGroupSetBits proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008560
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$0000188C
	cmps	r4,#1
	mov	r3,r0
	beq	$00008DDA

l00008DD2:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008DDA:
	msr	cpsr,r0
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
	bl	$00008560
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$000018F4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008E10

l00008E08:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008E10:
	msr	cpsr,r0
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008E1A                               00 BF                       ..   

;; MPU_vEventGroupDelete: 00008E1C
MPU_vEventGroupDelete proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008560
	mov	r4,r0
	mov	r0,r5
	bl	$000019A0
	cmps	r4,#1
	beq	$00008E38

l00008E30:
	mrs	r0,cpsr
	orr	r0,r0,#1

l00008E38:
	msr	cpsr,r0

;; fn00008E3C: 00008E3C
fn00008E3C proc
	pop	{r3-r5,pc}
00008E3E                                           00 BF               ..

;; xCoRoutineCreate: 00008E40
xCoRoutineCreate proc
	push.w	{r3-fp,lr}
	mov	r9,r0
	mov	r0,#&38
	mov	r5,r1
	mov	r10,r2
	bl	$00001728
	cmps	r0,#0
	beq	$00008EE0

l00008E54:
	ldr	r7,[pc,#&94]                                           ; 00008EF0
	mov	r4,r0
	ldr	r3,[r7]
	cbz	r3,$00008EAC

l00008E5C:
	add	r8,r7,#4
	cmps	r5,#1
	it	hs
	movhs	r5,#1

l00008E66:
	mov	r3,#0
	mov	r6,r4
	strh	r3,[r4,#&68]
	str	r5,[r4,#&2C]
	str	r10,[r4,#&30]
	str	r9,[r6],#&4
	mov	r0,r6
	bl	$000082E4
	add	r0,r4,#&18
	bl	$000082E4
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
	bl	$000082EC
	mov	r0,#1
	pop.w	{r3-fp,pc}

l00008EAC:
	mov	r8,r7
	str	r0,[r8],#&4
	mov	r0,r8
	bl	$000082CC
	add	fp,r7,#&2C
	add	r0,r7,#&18
	bl	$000082CC
	add	r6,r7,#&40
	mov	r0,fp
	bl	$000082CC
	mov	r0,r6
	bl	$000082CC
	add	r0,r7,#&54
	bl	$000082CC
	str	fp,[r7,#&68]

l00008EE0:
	str	r6,[r7,#&6C]
	b	$00008E5C
00008EE4             4F F0 FF 30 BD E8 F8 8F                 O..0....   

;; fn00008EEC: 00008EEC
fn00008EEC proc
	lsls	r4,r7,#&1F
	mov	r0,#0

;; vCoRoutineAddToDelayedList: 00008EF0
vCoRoutineAddToDelayedList proc
	push	{r4-r6,lr}
	mov	r6,r1
	ldr	r4,[pc,#&30]                                           ; 00008F2C
	ldr	r3,[r4]
	ldr	r5,[r4,#&74]
	adds	r5,r0
	add	r0,r3,#4
	bl	$0000833C
	ldr	r3,[r4,#&74]
	ldr	r1,[r4]
	cmps	r5,r3
	str	r5,[r1,#&4]
	ite	lo
	ldrlo	r0,[r4,#&6C]

l00008F0E:
	ldr	r0,[r4,#&68]
	adds	r1,#4
	bl	$00008308
	cbz	r6,$00008F26

l00008F18:
	ldr	r1,[r4]
	mov	r0,r6
	pop.w	{r4-r6,lr}
	adds	r1,#&18
	b	$00008308

l00008F26:
	pop	{r4-r6,pc}

;; fn00008F28: 00008F28
fn00008F28 proc
	lsls	r4,r7,#&1F
	mov	r0,#0

;; vCoRoutineSchedule: 00008F2C
vCoRoutineSchedule proc
	push.w	{r4-r8,lr}
	ldr	r5,[pc,#&154]                                          ; 0000908C
	ldr	r3,[r5,#&54]
	cbz	r3,$00008F82

l00008F36:
	mov	r7,#0
	add	r8,r5,#4
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r3,[r5,#&60]
	ldr	r4,[r3,#&C]
	add	r0,r4,#&18
	bl	$0000833C
	msr	cpsr,r7
	add	r6,r4,#4
	mov	r0,r6
	bl	$0000833C
	ldr	r3,[r4,#&2C]
	ldr	r2,[r5,#&70]
	add.w	r0,r3,r3,lsl #2
	cmps	r3,r2
	mov	r1,r6
	add.w	r0,r8,r0,lsl #2
	it	hi
	strhi	r3,[r5,#&70]
	bl	$000082EC
	ldr	r3,[r5,#&54]
	cmps	r3,#0
	bne	$00009138

l00008F82:
	bl	$00008900
	mov	r7,#0
	ldr	r2,[r5,#&78]
	ldr	r3,[r5,#&74]
	sub	r0,r0,r2
	ldr	r8,[pc,#&100]                                          ; 00009096

l00008F90:
	strh	r0,[r0,#&10]
	str	r0,[r5,#&7C]
	cmps	r0,#0
	beq	$00009010

l00008F98:
	adds	r3,#1
	subs	r0,#1
	str	r3,[r5,#&74]
	str	r0,[r5,#&7C]
	cmps	r3,#0
	beq	$00009048

l00008FA4:
	ldr	r2,[r5,#&68]
	ldr	r1,[r2]
	cmps	r1,#0
	beq	$00009190

l00008FAC:
	ldr	r2,[r2,#&C]
	ldr	r4,[r2,#&C]
	ldr	r2,[r4,#&4]
	cmps	r3,r2
	bhs	$00008FC0

l00008FB6:
	b	$00008F90
00008FB8                         DA 68 6B 6F D4 68 62 68         .hko.hbh

l00008FC0:
	cmps	r2,r3
	bhi	$0000900A

l00008FC4:
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	add	r6,r4,#4
	mov	r0,r6
	bl	$0000833C
	ldr	r3,[r4,#&28]
	add	r0,r4,#&18
	cbz	r3,$00008FE8
	bl	$0000833C
	msr	cpsr,r7
	ldr	r3,[r4,#&2C]
	ldr	r2,[r5,#&70]
	add.w	r0,r3,r3,lsl #2
	cmps	r3,r2
	mov	r1,r6
	add.w	r0,r8,r0,lsl #2
	it	hi
	strhi	r3,[r5,#&70]
	bl	$000082EC
	ldr	r3,[r5,#&68]
	ldr	r2,[r3]
	cmps	r2,#0

l0000900A:
	bne	$000091B4

l0000900C:
	ldr	r3,[r5,#&74]
	ldr	r0,[r5,#&7C]

l00009010:
	cmps	r0,#0
	bne	$00009194

l00009014:
	ldr	r1,[r5,#&70]
	str	r3,[r5,#&78]
	lsls	r3,r1,#2
	add	r2,r3,r1
	add.w	r2,r5,r2,lsl #2
	ldr	r2,[r2,#&4]
	cmps	r2,#0
	bne	$00009080

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

l00009048:
	pop.w	{r4-r8,pc}
0000904C                                     A9 6E EA 6E             .n.n
00009050 E9 66 AA 66 A7 E7                               .f.f..         

l00009056:
	str	r2,[r5,#&70]
	adds	r3,r2
	lsls	r3,r3,#2
	add	r1,r5,r3
	ldr	r2,[r1,#&8]
	ldr	r0,[pc,#&28]                                           ; 00009090
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
00009084             0A 46 E7 E7 FC 07 00 20 08 08 00 20     .F..... ... 

;; fn00009090: 00009090
fn00009090 proc
	movs	r0,r0,#0
	mov	r0,#0

;; xCoRoutineRemoveFromEventList: 00009094
xCoRoutineRemoveFromEventList proc
	ldr	r3,[r0,#&C]
	push	{r4-r6,lr}
	ldr	r4,[r3,#&C]
	ldr	r5,[pc,#&24]                                           ; 000090C6
	add	r6,r4,#&18
	mov	r0,r6
	bl	$0000833C
	add	r0,r5,#&54
	mov	r1,r6
	bl	$000082EC
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
	ldr	r3,[pc,#&3C]                                           ; 00009108
	cmps	r0,r3
	beq	$000090FA

l000090CA:
	bhi	$000090DA

l000090CC:
	cmp	r0,#&40004000
	beq	$000090F6

l000090D2:
	sub	r3,r3,#&1000
	cmps	r0,r3
	bne	$000090EC

l000090DA:
	mov	r0,#&11
	bx	lr
000090DE                                           0A 4B               .K
000090E0 98 42 08 D0 03 F5 E8 33 98 42 01 D1             .B.....3.B..   

l000090EC:
	mov	r0,#&14
	bx	lr
000090F0 4F F0 FF 30 70 47                               O..0pG         

l000090F6:
	mov	r0,#&13
	bx	lr

l000090FA:
	mov	r0,#&10
	bx	lr
000090FE                                           12 20               . 
00009100 70 47 00 BF 00 60 00 40                         pG...`.@       

;; fn00009108: 00009108
fn00009108 proc
	strb	r0,[r0]
	ands	r0,r0

;; GPIODirModeSet: 0000910C
GPIODirModeSet proc
	ldr	r3,[r0,#&400]
	tst	r2,#1
	ite	ne
	orrne	r3,r1

l00009118:
	Invalid
	str	r3,[r0,#&400]
	ldr	r3,[r0,#&420]
	lsls	r2,r2,#&1E
	ite	mi
	orrmi	r1,r3
	bicpl.w	r1,r3,r1
	str	r1,[r0,#&420]
	bx	lr
	nop

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
	Invalid
	str	r3,[r0,#&408]
	ldr	r3,[r0,#&404]
	tst	r2,#2
	ite	ne
	orrne	r3,r1
	Invalid
	str	r3,[r0,#&404]
	ldr	r3,[r0,#&40C]
	lsls	r2,r2,#&1D
	ite	mi
	orrmi	r1,r3
	bicpl.w	r1,r3,r1
	str	r1,[r0,#&40C]

;; fn00009190: 00009190
fn00009190 proc
	asrss	r4,r1,#&10
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

;; fn000091B4: 000091B4
fn000091B4 proc
	mov	r3,#2

;; fn000091B6: 000091B6
fn000091B6 proc
	mov	r3,#0
	adcs	r0,r1
	ite	ne
	movne	r0,#4

l000091BE:
	mov	r0,#0
	orrs	r3,r2
	orrs	r0,r3

;; fn000091C4: 000091C4
fn000091C4 proc
	bx	lr
000091C6                   00 BF                               ..       

;; GPIOPadConfigSet: 000091C8
GPIOPadConfigSet proc
	push	{r4}
	ldr	r4,[r0,#&500]
	tst	r2,#1
	ite	ne
	orrne	r4,r1

l000091D6:
	Invalid
	str	r4,[r0,#&500]
	ldr	r4,[r0,#&504]
	tst	r2,#2
	ite	ne
	orrne	r4,r1
	Invalid
	str	r4,[r0,#&504]
	ldr	r4,[r0,#&508]
	tst	r2,#4
	ite	ne
	orrne	r4,r1
	Invalid
	str	r4,[r0,#&508]
	tst	r2,#8
	ldr	r2,[r0,#&518]
	ite	ne
	orrne	r2,r1
	Invalid
	str	r2,[r0,#&518]
	ldr	r2,[r0,#&50C]
	lsls	r4,r3,#&1F
	ite	mi
	orrmi	r2,r1
	Invalid
	str	r2,[r0,#&50C]
	ldr	r2,[r0,#&510]
	lsls	r4,r3,#&1E
	ite	mi
	orrmi	r2,r1
	Invalid
	str	r2,[r0,#&510]
	ldr	r2,[r0,#&514]
	lsls	r4,r3,#&1D
	ite	mi
	orrmi	r2,r1
	Invalid
	str	r2,[r0,#&514]
	tst	r3,#8
	ldr	r3,[r0,#&51C]
	pop	{r4}
	ite	ne
	orrne	r1,r3
	biceq.w	r1,r3,r1
	str	r1,[r0,#&51C]
	bx	lr
	nop

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
	ldr	r3,[pc,#&90]                                           ; 000093AC
	push	{r4,lr}
	cmps	r0,r3
	beq	$00009392

l0000931C:
	bhi	$0000933A

l0000931E:
	cmp	r0,#&40004000
	beq	$00009380

l00009324:
	sub	r3,r3,#&1000
	cmps	r0,r3
	bne	$0000935A

l0000932C:
	mov	r4,#&11
	mov	r0,r4
	bl	$00009500
	mov	r0,r4
	pop.w	{r4,lr}

l0000933A:
	b	$00C095D8
0000933E                                           1B 4B               .K
00009340 98 42 16 D0 03 F5 E8 33 98 42 08 D1 14 24 20 46 .B.....3.B...$ F
00009350 00 F0 D8 F8 20 46 BD E8 10 40                   .... F...@     

l0000935A:
	b	$00C095D8
0000935E                                           4F F0               O.
00009360 FF 34 20 46 00 F0 CE F8 20 46 BD E8 10 40 00 F0 .4 F.... F...@..
00009370 35 B9 13 24 20 46 00 F0 C5 F8 20 46 BD E8 10 40 5..$ F.... F...@

l00009380:
	b	$00C095D8
00009384             10 24 20 46 00 F0 BC F8 20 46 BD E8     .$ F.... F..
00009390 10 40                                           .@             

l00009392:
	b	$00C095D8
00009396                   12 24 20 46 00 F0 B3 F8 20 46       .$ F.... F
000093A0 BD E8 10 40 00 F0 1A B9 00 60 00 40 00 70 00 40 ...@.....`.@.p.@

;; GPIOPortIntUnregister: 000093B0
GPIOPortIntUnregister proc
	ldr	r3,[pc,#&90]                                           ; 00009448
	push	{r4,lr}
	cmps	r0,r3
	beq	$0000942E

l000093B8:
	bhi	$000093D6

l000093BA:
	cmp	r0,#&40004000
	beq	$0000941C

l000093C0:
	sub	r3,r3,#&1000
	cmps	r0,r3
	bne	$000093F6

l000093C8:
	mov	r4,#&11
	mov	r0,r4
	bl	$00009634
	mov	r0,r4
	pop.w	{r4,lr}

l000093D6:
	b	$00C09534
000093DA                               1B 4B 98 42 16 D0           .K.B..
000093E0 03 F5 E8 33 98 42 08 D1 14 24 20 46 00 F0 24 F9 ...3.B...$ F..$.
000093F0 20 46 BD E8 10 40                                F...@         

l000093F6:
	b	$00C09534
000093FA                               4F F0 FF 34 20 46           O..4 F
00009400 00 F0 1A F9 20 46 BD E8 10 40 00 F0 95 B8 13 24 .... F...@.....$
00009410 20 46 00 F0 11 F9 20 46 BD E8 10 40              F.... F...@   

l0000941C:
	b	$00C09534
00009420 10 24 20 46 00 F0 08 F9 20 46 BD E8 10 40       .$ F.... F...@ 

l0000942E:
	b	$00C09534
00009432       12 24 20 46 00 F0 FF F8 20 46 BD E8 10 40   .$ F.... F...@
00009440 00 F0 7A B8 00 60 00 40 00 70 00 40             ..z..`.@.p.@   

;; GPIOPinRead: 0000944C
GPIOPinRead proc
	ldr.w	r0,[r0,r1,lsl #2]

;; fn00009450: 00009450
fn00009450 proc
	bx	lr
00009452       00 BF                                       ..           

;; GPIOPinWrite: 00009454
GPIOPinWrite proc
	str.w	r2,[r0,r1,lsl #2]
	bx	lr
0000945A                               00 BF                       ..   

;; GPIOPinTypeComparator: 0000945C
GPIOPinTypeComparator proc
	push	{r4-r6}
	Invalid
	ldr	r2,[r0,#&400]
	mov	r3,#0
	ands	r2,r5
	str	r2,[r0,#&400]
	ldr	r6,[r0,#&420]
	mov	r2,#1
	ands	r5,r6
	str	r5,[r0,#&420]
	pop	{r4-r6}
	b	$000091C4

;; fn0000947C: 0000947C
fn0000947C proc
	bkpt
	nop

;; GPIOPinTypeI2C: 00009480
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
	b	$000091C4

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

;; fn000094C4: 000094C4
fn000094C4 proc
	b	$000091C4

;; GPIOPinTypeUART: 000094C8
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
	b	$000091C4

;; GPIOPinTypeTimer: 000094EC
GPIOPinTypeTimer proc
	b	$000094C4

;; GPIOPinTypeSSI: 000094F0
GPIOPinTypeSSI proc
	b	$000094C4

;; GPIOPinTypePWM: 000094F4
GPIOPinTypePWM proc
	b	$000094C4

;; IntDefaultHandler: 000094F8
IntDefaultHandler proc
	b	$000094F4
000094FA                               00 BF                       ..   

;; IntMasterEnable: 000094FC
IntMasterEnable proc
	b	$00C0A0D8

;; IntMasterDisable: 00009500
IntMasterDisable proc
	b	$00C0A0E0

;; IntRegister: 00009504
IntRegister proc
	ldr	r3,[pc,#&28]                                           ; 00009534
	push	{r4-r5}
	ldr	r3,[r3]
	ldr	r4,[pc,#&28]                                           ; 0000953A
	cmps	r3,r4
	beq	$00009522

l00009510:
	mov	r3,r4
	add	r5,r4,#&B8
	sub	r2,r3,r4
	ldr	r2,[r2]
	str	r2,[r3],#&4
	cmps	r3,r5
	bne	$00009712

l00009522:
	ldr	r3,[pc,#&C]                                            ; 00009536
	str	r4,[r3]
	str.w	r1,[r4,r0,lsl #2]
	pop	{r4-r5}
	bx	lr
0000952E                                           00 BF               ..
00009530 08 ED 00 E0                                     ....           

;; fn00009534: 00009534
fn00009534 proc
	mov	r0,r0
	mov	r0,#0

;; IntUnregister: 00009538
IntUnregister proc
	ldr	r3,[pc,#&8]                                            ; 00009548
	ldr	r2,[pc,#&C]                                            ; 0000954E
	str.w	r2,[r3,r0,lsl #2]
	bx	lr
00009542       00 BF 00 00 00 20 F9 94 00 00               ..... ....   

;; IntPriorityGroupingSet: 0000954C
IntPriorityGroupingSet proc
	ldr	r3,[pc,#&10]                                           ; 00009564
	ldr	r2,[pc,#&14]                                           ; 0000956A
	ldr.w	r3,[r3,r0,lsl #2]
	orr	r3,r3,#&5F80000
	orr	r3,r3,#&20000
	str	r3,[r2]
	bx	lr
00009560 A4 A2 00 00 0C ED 00 E0                         ........       

;; IntPriorityGroupingGet: 00009568
IntPriorityGroupingGet proc
	mov	r3,#&700
	ldr	r1,[pc,#&18]                                           ; 0000958C
	mov	r0,#0
	ldr	r1,[r1]
	ldr	r2,[pc,#&18]                                           ; 00009592
	ands	r1,r3
	b	$00009578

l00009578:
	ldr	r3,[r2],#&4
	cmps	r3,r1
	beq	$00009582

l00009580:
	adds	r0,#1

l00009582:
	cmps	r0,#8
	bne	$00009774

l00009586:
	bx	lr
00009588                         0C ED 00 E0 A8 A2 00 00         ........

;; IntPrioritySet: 00009590
IntPrioritySet proc
	mov	r2,#&FF
	ldr	r3,[pc,#&24]                                           ; 000095BE
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
	ldr	r3,[pc,#&18]                                           ; 000095DC
	bic	r2,r0,#3
	adds	r3,r2
	ldr	r3,[r3,#&20]
	and	r0,r0,#3
	ldr	r3,[r3]
	lsls	r0,r0,#3
	lsr	r0,r3,r0
	uxtb	r0,r0
	bx	lr
000095D6                   00 BF                               ..       

;; fn000095D8: 000095D8
fn000095D8 proc
	adr	r2,$00009868
	mov	r0,r0

;; IntEnable: 000095DC
IntEnable proc
	cmps	r0,#4
	beq	$00009604

l000095E0:
	cmps	r0,#5
	beq	$00009610

l000095E4:
	cmps	r0,#6
	beq	$0000961C

l000095E8:
	cmps	r0,#&F
	beq	$000095F8

l000095EC:
	bls	$000095F6

l000095EE:
	mov	r3,#1
	subs	r0,#&10
	ldr	r2,[pc,#&38]                                           ; 00009632
	lsl	r0,r3,r0

l000095F6:
	and	r0,r0,#&9000000

l000095F8:
	str	r0,[r2]

l000095FA:
	bx	lr
000095FC                                     0C 4A 13 68             .J.h
00009600 43 F0 02 03                                     C...           

l00009604:
	str	r3,[r2]
	bx	lr
00009608                         0A 4A 13 68 43 F4 80 33         .J.hC..3

l00009610:
	str	r3,[r2]
	bx	lr
00009614             07 4A 13 68 43 F4 00 33                 .J.hC..3   

l0000961C:
	str	r3,[r2]
	bx	lr
00009620 04 4A 13 68 43 F4 80 23 13 60 70 47 00 E1 00 E0 .J.hC..#.`pG....
00009630 10 E0 00 E0                                     ....           

;; fn00009634: 00009634
fn00009634 proc
	Invalid

;; IntDisable: 00009638
IntDisable proc
	cmps	r0,#4
	beq	$00009660

l0000963C:
	cmps	r0,#5
	beq	$0000966C

l00009640:
	cmps	r0,#6
	beq	$00009678

l00009644:
	cmps	r0,#&F
	beq	$00009654

l00009648:
	bls	$00009652

l0000964A:
	mov	r3,#1
	subs	r0,#&10
	ldr	r2,[pc,#&38]                                           ; 0000968E
	lsl	r0,r3,r0

l00009652:
	and	r0,r0,#&9000000

l00009654:
	str	r0,[r2]

l00009656:
	bx	lr
00009658                         0C 4A 13 68 23 F0 02 03         .J.h#...

l00009660:
	str	r3,[r2]
	bx	lr
00009664             0A 4A 13 68 23 F4 80 33                 .J.h#..3   

l0000966C:
	str	r3,[r2]
	bx	lr
00009670 07 4A 13 68 23 F4 00 33                         .J.h#..3       

l00009678:
	str	r3,[r2]
	bx	lr
0000967C                                     04 4A 13 68             .J.h
00009680 23 F4 80 23 13 60 70 47 80 E1 00 E0 10 E0 00 E0 #..#.`pG........

;; fn00009690: 00009690
fn00009690 proc
	Invalid

;; OSRAMDelay: 00009694
OSRAMDelay proc
	subs	r0,#1
	bne	$00009890

;; fn00009698: 00009698
fn00009698 proc
	bx	lr
0000969A                               00 BF                       ..   

;; OSRAMWriteFirst: 0000969C
OSRAMWriteFirst proc
	push	{r3-r5,lr}
	mov	r5,r0
	ldr	r4,[pc,#&1C]                                           ; 000096C4
	mov	r2,#0
	mov	r0,r4
	mov	r1,#&3D
	bl	$0000A204
	mov	r1,r5
	mov	r0,r4
	bl	$0000A238
	mov	r0,r4
	pop.w	{r3-r5,lr}
	mov	r1,#3
	b	$00C0A21C

;; fn000096C0: 000096C0
fn000096C0 proc
	mov	r0,r0
	ands	r2,r0

;; OSRAMWriteArray: 000096C4
OSRAMWriteArray proc
	cbz	r1,$000096FA

l000096C6:
	push	{r3-r7,lr}
	mov	r5,r0
	ldr	r7,[pc,#&30]                                           ; 00009702
	ldr	r4,[pc,#&30]                                           ; 00009704
	add	r6,r0,r1
	mov	r1,#0
	mov	r0,r4
	bl	$0000A1C4
	cmps	r0,#0
	beq	$000098CC

l000096DC:
	ldr	r0,[r7]
	bl	$00009690
	ldrb	r1,[r5],#&1
	mov	r0,r4
	bl	$0000A238
	mov	r1,#1
	mov	r0,r4
	bl	$0000A21C
	cmps	r6,r5
	bne	$000098CC

l000096F8:
	pop	{r3-r7,pc}

l000096FA:
	bx	lr
000096FC                                     7C 08 00 20             |.. 

;; fn00009700: 00009700
fn00009700 proc
	mov	r0,r0
	ands	r2,r0

;; OSRAMWriteByte: 00009704
OSRAMWriteByte proc
	push	{r4,lr}
	mov	r4,r0
	mov	r1,#0
	ldr	r0,[pc,#&24]                                           ; 00009736
	bl	$0000A1C4
	cmps	r0,#0

;; fn00009712: 00009712
fn00009712 proc
	beq	$00009904

l00009714:
	ldr	r3,[pc,#&1C]                                           ; 00009738
	ldr	r0,[r3]
	bl	$00009690
	mov	r1,r4
	ldr	r0,[pc,#&10]                                           ; 00009736
	bl	$0000A238
	pop.w	{r4,lr}
	mov	r1,#1
	ldr	r0,[pc,#&4]                                            ; 00009736
	b	$00C0A21C
00009730 00 00 02 40                                     ...@           

;; fn00009734: 00009734
fn00009734 proc
	lsrs	r4,r7,#1
	mov	r0,#0

;; OSRAMWriteFinal: 00009738
OSRAMWriteFinal proc
	push	{r4-r6,lr}
	mov	r6,r0
	ldr	r4,[pc,#&38]                                           ; 0000977C
	mov	r1,#0
	mov	r0,r4
	bl	$0000A1C4
	cmps	r0,#0
	beq	$0000993A

l0000974A:
	ldr	r5,[pc,#&30]                                           ; 00009782
	ldr	r4,[pc,#&28]                                           ; 0000977C
	ldr	r0,[r5]
	bl	$00009690
	mov	r1,r6
	mov	r0,r4
	bl	$0000A238
	mov	r1,#5
	mov	r0,r4
	bl	$0000A21C
	mov	r1,#0
	mov	r0,r4
	bl	$0000A1C4
	cmps	r0,#0
	beq	$00009960

l00009770:
	ldr	r0,[r5]
	pop.w	{r4-r6,lr}

;; fn00009774: 00009774
fn00009774 proc
	eors	r0,r6
	b	$00009690
00009778                         00 00 02 40                     ...@   

;; fn0000977C: 0000977C
fn0000977C proc
	lsrs	r4,r7,#1
	mov	r0,#0

;; OSRAMClear: 00009780
OSRAMClear proc
	push	{r4,lr}
	mov	r0,#&80
	bl	$00009698
	mov	r1,#6
	ldr	r0,[pc,#&38]                                           ; 000097CA
	bl	$000096C0
	mov	r4,#&5F
	mov	r0,#0
	bl	$00009700
	subs	r4,#1
	bne	$0000998E

l0000979C:
	mov	r0,r4
	bl	$00009734
	mov	r0,#&80
	bl	$00009698
	mov	r1,#6
	ldr	r0,[pc,#&1C]                                           ; 000097CE
	bl	$000096C0
	mov	r4,#&5F
	mov	r0,#0
	bl	$00009700
	subs	r4,#1
	bne	$000099AE

l000097BC:
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009734
000097C4             F4 A2 00 00                             ....       

;; fn000097C8: 000097C8
fn000097C8 proc
	adr	r2,$00009BB8
	mov	r0,r0

;; OSRAMStringDraw: 000097CC
OSRAMStringDraw proc
	push	{r4-r6,lr}
	mov	r6,r2
	mov	r4,r1
	mov	r5,r0
	mov	r0,#&80
	bl	$00009698
	cmps	r6,#0
	ite	eq
	moveq	r0,#&B0

l000097E0:
	mov	r0,#&B1
	bl	$00009700
	add	r6,r4,#&24
	mov	r0,#&80
	bl	$00009700
	and	r0,r6,#&F
	bl	$00009700
	mov	r0,#&80
	bl	$00009700
	ubfx	r0,r6,#4,#4
	orr	r0,r0,#&10
	bl	$00009700
	mov	r0,#&40
	bl	$00009700
	ldrb	r3,[r5]
	cbz	r3,$00009876

l00009814:
	cmps	r4,#&5A
	ldr	r6,[pc,#&60]                                           ; 0000987E
	bls	$0000982C

l0000981A:
	b	$00009848
0000981C                                     15 F8 01 3F             ...?
00009820 06 34 83 B1 FF F7 6E FF 2B 78 1B B3             .4....n.+x..   

l0000982C:
	cmps	r4,#&5A
	bhi	$00009848

l00009830:
	subs	r3,#&20
	add.w	r3,r3,r3,lsl #2
	add	r0,r6,r3
	mov	r1,#5
	bl	$000096C0
	cmps	r4,#&5A
	mov	r0,#0
	bne	$00009A18

l00009846:
	pop.w	{r4-r6,lr}

l00009848:
	eors	r0,r6

l0000984A:
	b	$00009734
0000984C                                     20 3B 03 EB              ;..
00009850 83 03 C4 F1 5F 04 F0 18 21 46 FF F7 33 FF 2B 78 ...._...!F..3.+x
00009860 06 4A 20 3B 03 EB 83 03 13 44 23 44 18 7C BD E8 .J ;.....D#D.|..
00009870 70 40 61 E7 70 BD                               p@a.p.         

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

;; fn00009890: 00009890
fn00009890 proc
	ubfx	r8,r1,#4,#4
	adds	r6,r2
	orr	r8,r8,#&10
	and	r7,r1,#&F
	add	r10,r3,#&FFFFFFFF
	mov	r0,#&80
	bl	$00009698
	cmps	r4,#0
	ite	ne
	movne	r0,#&B1

;; fn000098AE: 000098AE
fn000098AE proc
	mov	r0,#&B0
	bl	$00009700
	mov	r0,#&80
	bl	$00009700
	mov	r0,r7
	bl	$00009700
	mov	r0,#&80
	bl	$00009700
	mov	r0,r8
	bl	$00009700

;; fn000098CC: 000098CC
fn000098CC proc
	mov	r0,#&40
	bl	$00009700
	mov	r0,r5
	mov	r1,r10
	adds	r5,r9
	bl	$000096C0
	adds	r4,#1
	ldrb.w	r0,[r5,-#&1]
	bl	$00009734
	cmps	r6,r4
	bne	$00009A9E

l000098EA:
	pop.w	{r4-r10,pc}

;; fn000098EC: 000098EC
fn000098EC proc
	strh	r0,[r6,#&7C]
000098EE                                           00 BF               ..

;; OSRAMInit: 000098F0
OSRAMInit proc
	push.w	{r4-r8,lr}
	mov	r4,r0
	mov	r0,#&10001000
	bl	$00009B78
	ldr	r0,[pc,#&60]                                           ; 00009966
	bl	$00009B78

;; fn00009904: 00009904
fn00009904 proc
	mov	r1,#&C
	ldr	r0,[pc,#&5C]                                           ; 0000996A
	bl	$0000947C
	mov	r1,r4
	ldr	r0,[pc,#&58]                                           ; 0000996E
	bl	$0000A0F0
	mov	r2,#1
	ldr	r3,[pc,#&54]                                           ; 00009972
	ldr	r7,[pc,#&54]                                           ; 00009974
	mov	r6,#&E3
	mov	r4,#4
	mov	r0,#&80
	mov	r5,#0
	str	r2,[r3]
	add	r8,r7,#&1EC
	b	$00009934
0000992A                               93 F8 EC 41 93 F8           ...A..
00009930 ED 01 23 44                                     ..#D           

;; fn00009934: 00009934
fn00009934 proc
	ldrb	r6,[r3,#&EC]
	bl	$00009698

;; fn0000993A: 0000993A
fn0000993A proc
	Invalid
	sub	r1,r4,#2
	adds	r0,r8
	adds	r4,#1
	bl	$000096C0
	adds	r5,r4
	mov	r0,r6
	bl	$00009734
	cmps	r5,#&70
	add.w	r3,r7,r5
	bls	$00009B26

l00009958:
	pop.w	{r4-r8,lr}
	b	$0000977C

l00009960:
	mov	r2,r0
	mov	r0,#0
	str	r0,[r0,r0]
	ands	r0,r0
	mov	r0,r0
	ands	r2,r0
	lsrs	r4,r7,#1
	mov	r0,#0
	adr	r2,$00009D40
	mov	r0,r0

;; OSRAMDisplayOn: 00009974
OSRAMDisplayOn proc
	push.w	{r4-r8,lr}
	ldr	r7,[pc,#&40]                                           ; 000099C0
	mov	r6,#&E3
	mov	r4,#4
	mov	r0,#&80
	mov	r5,#0
	add	r8,r7,#&1EC
	b	$00009992
00009988                         93 F8 EC 41 93 F8               ...A.. 

l0000998E:
	lsls	r5,r5,#7
	adds	r3,r4

;; fn00009992: 00009992
fn00009992 proc
	ldrb	r6,[r3,#&EC]
	bl	$00009698
	add	r0,r5,#2
	sub	r1,r4,#2
	adds	r0,r8
	adds	r4,#1
	bl	$000096C0
	adds	r5,r4
	mov	r0,r6
	bl	$00009734

;; fn000099AE: 000099AE
fn000099AE proc
	cmps	r5,#&70
	add.w	r3,r7,r5
	bls	$00009B84

l000099B6:
	pop.w	{r4-r8,pc}
000099BA                               00 BF F4 A2 00 00           ......

;; OSRAMDisplayOff: 000099C0
OSRAMDisplayOff proc
	push	{r3,lr}
	mov	r0,#&80
	bl	$00009698
	mov	r0,#&AE
	bl	$00009700
	mov	r0,#&80
	bl	$00009700
	mov	r0,#&AD
	bl	$00009700
	mov	r0,#&80
	bl	$00009700
	pop.w	{r3,lr}

;; fn000099E4: 000099E4
fn000099E4 proc
	mov	r0,#&8A
	b	$00009734

;; SSIConfig: 000099E8
SSIConfig proc
	push.w	{r4-r8,lr}
	mov	r7,r2
	mov	r6,r0
	mov	r8,r1
	mov	r4,r3
	ldr	r5,[sp,#&18]
	bl	$00009DEC
	cmps	r7,#2
	beq	$00009A2C

l000099FE:
	cmps	r7,#0

;; fn00009A00: 00009A00
fn00009A00 proc
	it	ne
	movne	r7,#4

;; fn00009A04: 00009A04
fn00009A04 proc
	udiv	r3,r0,r4
	mov	r4,#0
	str	r7,[r6,#&4]
	adds	r4,#2
	udiv	r2,r3,r4
	subs	r2,#1
	cmps	r2,#&FF
	bhi	$00009C08

;; fn00009A18: 00009A18
fn00009A18 proc
	and	r3,r8,#&30
	subs	r5,#1
	orr	r1,r3,r8,lsl #6
	orrs	r5,r1
	orr	r2,r5,r2,lsl #8
	str	r4,[r6,#&10]
	str	r2,[r6]

l00009A2C:
	pop.w	{r4-r8,pc}

;; fn00009A30: 00009A30
fn00009A30 proc
	mov	r7,#&C
	b	$00009A00

;; SSIEnable: 00009A34
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
	bl	$00009500
	pop.w	{r3,lr}
	mov	r0,#&17
	b	$000095D8
00009A5E                                           00 BF               ..

;; SSIIntUnregister: 00009A60
SSIIntUnregister proc
	push	{r3,lr}
	mov	r0,#&17
	bl	$00009634
	pop.w	{r3,lr}
	mov	r0,#&17
	b	$00009534
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
SSIDataPut proc
	add	r2,r0,#&C
	ldr	r3,[r2]

;; fn00009A9E: 00009A9E
fn00009A9E proc
	lsls	r3,r3,#&1E
	bpl	$00009C98

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

;; fn00009AB4: 00009AB4
fn00009AB4 proc
	mov	r0,r3
	bx	lr

;; SSIDataGet: 00009AB8
SSIDataGet proc
	add	r2,r0,#&C
	ldr	r3,[r2]
	lsls	r3,r3,#&1D
	bpl	$00009CB8

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
	ldr	r3,[pc,#&C]                                            ; 00009AF0
	ldr	r0,[pc,#&10]                                           ; 00009AF6
	ldr	r3,[r3]
	and.w	r0,r0,r3,lsr #8
	add	r0,r0,#&100
	bx	lr
00009AEC                                     08 E0 0F 40             ...@
00009AF0 00 FF FF 00                                     ....           

;; SysCtlFlashSizeGet: 00009AF4
SysCtlFlashSizeGet proc
	ldr	r3,[pc,#&C]                                            ; 00009B08
	ldr	r0,[pc,#&10]                                           ; 00009B0E
	ldr	r3,[r3]
	and.w	r0,r0,r3,lsl #&B
	add	r0,r0,#&800
	bx	lr
00009B04             08 E0 0F 40 00 F8 FF 07                 ...@....   

;; SysCtlPinPresent: 00009B0C
SysCtlPinPresent proc
	ldr	r3,[pc,#&C]                                            ; 00009B20
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
	ldr	r3,[pc,#&14]                                           ; 00009B3C
	lsrs	r2,r0,#&1C
	ldr.w	r3,[r3,r2,lsl #2]

;; fn00009B26: 00009B26
fn00009B26 proc
	adds	r0,#&22
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
	ldr	r3,[pc,#&38]                                           ; 00009B7E
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
	bhi	$00009B66

l00009B5E:
	ldr	r3,[sp,#&4]
	adds	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]

l00009B66:
	cmps	r3,#&F
	bls	$00009D5A

l00009B6A:
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	add	sp,#&C
	pop	{r4}
	bx	lr

;; fn00009B78: 00009B78
fn00009B78 proc
	adr	r5,$00009CC8
	mov	r0,r0

;; SysCtlPeripheralEnable: 00009B7C
SysCtlPeripheralEnable proc
	ldr	r3,[pc,#&14]                                           ; 00009B98
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2

l00009B84:
	ldr	r3,[r3,#&1C]
	bic	r0,r0,#&F0000000
	ldr	r2,[r3]
	orrs	r0,r2
	str	r0,[r3]
	bx	lr
00009B92       00 BF 54 A5 00 00                           ..T...       

;; SysCtlPeripheralDisable: 00009B98
SysCtlPeripheralDisable proc
	ldr	r3,[pc,#&14]                                           ; 00009BB4
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
	ldr	r3,[pc,#&14]                                           ; 00009BD0
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
	ldr	r3,[pc,#&14]                                           ; 00009BEC
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
	ldr	r3,[pc,#&14]                                           ; 00009C08
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
	ldr	r3,[pc,#&14]                                           ; 00009C24
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
	ldr	r2,[pc,#&14]                                           ; 00009C40
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
	bl	$00009500
	pop.w	{r3,lr}
	mov	r0,#&2C
	b	$000095D8

;; SysCtlIntUnregister: 00009C54
SysCtlIntUnregister proc
	push	{r3,lr}
	mov	r0,#&2C
	bl	$00009634
	pop.w	{r3,lr}
	mov	r0,#&2C
	b	$00009534
00009C66                   00 BF                               ..       

;; SysCtlIntEnable: 00009C68
SysCtlIntEnable proc
	ldr	r2,[pc,#&8]                                            ; 00009C78
	ldr	r3,[r2]
	orrs	r0,r3
	str	r0,[r2]
	bx	lr
00009C72       00 BF 54 E0 0F 40                           ..T..@       

;; SysCtlIntDisable: 00009C78
SysCtlIntDisable proc
	ldr	r2,[pc,#&8]                                            ; 00009C88
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009C84             54 E0 0F 40                             T..@       

;; SysCtlIntClear: 00009C88
SysCtlIntClear proc
	ldr	r3,[pc,#&4]                                            ; 00009C94
	str	r0,[r3]
	bx	lr
00009C8E                                           00 BF               ..
00009C90 58 E0 0F 40                                     X..@           

;; SysCtlIntStatus: 00009C94
SysCtlIntStatus proc
	cbnz	r0,$00009C9C

l00009C96:
	ldr	r3,[pc,#&C]                                            ; 00009CAA

l00009C98:
	ldr	r0,[r3]
	bx	lr

l00009C9C:
	ldr	r3,[pc,#&8]                                            ; 00009CAC
	ldr	r0,[r3]
	bx	lr
00009CA2       00 BF 50 E0 0F 40 58 E0 0F 40               ..P..@X..@   

;; SysCtlLDOSet: 00009CAC
SysCtlLDOSet proc
	ldr	r3,[pc,#&4]                                            ; 00009CB8
	str	r0,[r3]
	bx	lr
00009CB2       00 BF 34 E0 0F 40                           ..4..@       

;; SysCtlLDOGet: 00009CB8
SysCtlLDOGet proc
	ldr	r3,[pc,#&4]                                            ; 00009CC4
	ldr	r0,[r3]
	bx	lr
00009CBE                                           00 BF               ..
00009CC0 34 E0 0F 40                                     4..@           

;; SysCtlLDOConfigSet: 00009CC4
SysCtlLDOConfigSet proc
	ldr	r3,[pc,#&4]                                            ; 00009CD0
	str	r0,[r3]
	bx	lr
00009CCA                               00 BF 60 E1 0F 40           ..`..@

;; SysCtlReset: 00009CD0
SysCtlReset proc
	ldr	r3,[pc,#&4]                                            ; 00009CDC

l00009CD2:
	ldr	r2,[pc,#&8]                                            ; 00009CE2
	str	r2,[r3]
	b	$00009CD2
00009CD8                         0C ED 00 E0 04 00 FA 05         ........

;; SysCtlSleep: 00009CE0
SysCtlSleep proc
	b	$00C0A0E8

;; SysCtlDeepSleep: 00009CE4
SysCtlDeepSleep proc
	push	{r4,lr}
	ldr	r4,[pc,#&18]                                           ; 00009D06
	ldr	r3,[r4]
	orr	r3,r3,#4
	str	r3,[r4]
	bl	$0000A0E8
	ldr	r3,[r4]
	bic	r3,r3,#4
	str	r3,[r4]
	pop	{r4,pc}
00009CFE                                           00 BF               ..
00009D00 10 ED 00 E0                                     ....           

;; SysCtlResetCauseGet: 00009D04
SysCtlResetCauseGet proc
	ldr	r3,[pc,#&4]                                            ; 00009D10
	ldr	r0,[r3]
	bx	lr
00009D0A                               00 BF 5C E0 0F 40           ..\..@

;; SysCtlResetCauseClear: 00009D10
SysCtlResetCauseClear proc
	ldr	r2,[pc,#&8]                                            ; 00009D20
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009D1C                                     5C E0 0F 40             \..@

;; SysCtlBrownOutConfigSet: 00009D20
SysCtlBrownOutConfigSet proc
	ldr	r3,[pc,#&8]                                            ; 00009D30
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
	ldr	r4,[pc,#&A4]                                           ; 00009DE6
	ldr	r1,[pc,#&A4]                                           ; 00009DE8
	ldr	r3,[r4]
	orn	r5,r0,#3
	ands	r1,r3
	orr	r1,r1,#&800
	ands	r1,r5
	ands	r2,r0
	bic	r3,r3,#&400000
	ldr	r5,[pc,#&94]                                           ; 00009DEE
	sub	sp,#8
	orr	r3,r3,#&800

;; fn00009D5A: 00009D5A
fn00009D5A proc
	orrs	r2,r1
	str	r3,[r4]
	str	r7,[r5]
	str	r2,[r4]
	str	r6,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bhi	$00009D72

;; fn00009D6A: 00009D6A
fn00009D6A proc
	ldr	r3,[sp,#&4]
	adds	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]

;; fn00009D72: 00009D72
fn00009D72 proc
	cmps	r3,#&F
	bls	$00009F66

l00009D76:
	and	r3,r0,#3
	ldr	r4,[pc,#&64]                                           ; 00009DE6
	bic	r2,r2,#3
	orrs	r2,r3
	bic	r3,r2,#&7C00000
	and	r1,r0,#&7C00000
	str	r2,[r4]
	lsls	r4,r0,#&14
	orr	r1,r1,r3
	bmi	$00009DBA

l00009D94:
	mov	r3,#&8000
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cbz	r3,$00009DBA

l00009D9E:
	ldr	r2,[pc,#&4C]                                           ; 00009DF2
	ldr	r3,[r2]
	lsls	r0,r3,#&19
	bpl	$00009DAA

l00009DA6:
	b	$00009DB6
00009DA8                         13 68                           .h     

l00009DAA:
	lsls	r3,r3,#&19
	bmi	$00009DB6

l00009DAE:
	ldr	r3,[sp,#&4]
	subs	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]

l00009DB6:
	cmps	r3,#0
	bne	$00009FA4

l00009DBA:
	bic	r1,r1,#&800
	mov	r3,#0
	ldr	r2,[pc,#&1C]                                           ; 00009DE4
	str	r1,[r2]
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bhi	$00009DD4

l00009DCC:
	ldr	r3,[sp,#&4]
	adds	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]

l00009DD4:
	cmps	r3,#&F
	bls	$00009FC8

l00009DD8:
	add	sp,#8
	pop	{r4-r7}
	bx	lr
00009DDE                                           00 BF               ..
00009DE0 60 E0 0F 40 0F CC BF FF 58 E0 0F 40             `..@....X..@   

;; fn00009DEC: 00009DEC
fn00009DEC proc
	b	$00009E8C
00009DEE                                           0F 40               .@

;; SysCtlClockGet: 00009DF0
SysCtlClockGet proc
	ldr	r3,[pc,#&60]                                           ; 00009E58
	ldr	r3,[r3]
	and	r2,r3,#&30
	cmps	r2,#&10
	beq	$00009E4A

l00009DFC:
	cmps	r2,#&20
	beq	$00009E46

l00009E00:
	cbz	r2,$00009E06

l00009E02:
	mov	r0,#0
	bx	lr

l00009E06:
	ldr	r2,[pc,#&50]                                           ; 00009E5E
	ubfx	r1,r3,#6,#4
	add.w	r2,r2,r1,lsl #2

l00009E0E:
	lsls	r1,r0,#&A

l00009E10:
	ldr	r0,[r2,#&30]
	lsls	r2,r3,#&14
	bmi	$00009E36

l00009E16:
	ldr	r2,[pc,#&44]                                           ; 00009E62
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

l00009E36:
	it	mi
	lsrsmi	r0,r0,#2

l00009E3A:
	lsls	r2,r3,#9
	bpl	$0000A000

l00009E3E:
	ubfx	r3,r3,#&17,#4
	adds	r3,#1
	udiv	r0,r0,r3

l00009E46:
	Invalid

l00009E48:
	bx	lr

l00009E4A:
	ldr	r0,[pc,#&14]                                           ; 00009E66
	b	$00009E0E
00009E4E                                           05 48               .H
00009E50 DF E7 00 BF 60 E0 0F 40 54 A5 00 00 64 E0 0F 40 ....`..@T...d..@
00009E60 70 38 39 00 C0 E1 E4 00                         p89.....       

;; SysCtlPWMClockSet: 00009E68
SysCtlPWMClockSet proc
	ldr	r2,[pc,#&C]                                            ; 00009E7C
	ldr	r3,[r2]
	bic	r3,r3,#&1E0000
	orrs	r0,r3
	str	r0,[r2]
	bx	lr
00009E76                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlPWMClockGet: 00009E7C
SysCtlPWMClockGet proc
	ldr	r3,[pc,#&8]                                            ; 00009E8C
	ldr	r0,[r3]
	and	r0,r0,#&1E0000
	bx	lr
00009E86                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlADCSpeedSet: 00009E8C
SysCtlADCSpeedSet proc
	push	{r4}
	ldr	r4,[pc,#&28]                                           ; 00009EBE
	ldr	r1,[pc,#&28]                                           ; 00009EC0
	ldr	r3,[r4]
	ldr	r2,[pc,#&28]                                           ; 00009EC4
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
	ldr	r3,[pc,#&8]                                            ; 00009ED4
	ldr	r0,[r3]
	and	r0,r0,#&F00
	bx	lr
00009ECE                                           00 BF               ..
00009ED0 00 E1 0F 40                                     ...@           

;; SysCtlIOSCVerificationSet: 00009ED4
SysCtlIOSCVerificationSet proc
	ldr	r2,[pc,#&14]                                           ; 00009EF0
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
	ldr	r2,[pc,#&14]                                           ; 00009F0C
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
	ldr	r2,[pc,#&14]                                           ; 00009F28
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
	ldr	r3,[pc,#&4]                                            ; 00009F38
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
	ldr	r4,[r0]
	ands	r4,r4,#8
	bne	$0000A152

l00009F5E:
	ldr	r3,[r5,#&2C]
	bic	r3,r3,#&10
	str	r3,[r5,#&2C]

l00009F66:
	ldr	r2,[r5,#&30]
	bic	r2,r2,#&300
	bic	r2,r2,#1
	str	r2,[r5,#&30]
	bl	$00009DEC
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

l00009FA4:
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
	bl	$00009DEC
	add.w	r5,r5,r8,lsl #6
	lsls	r0,r0,#2
	udiv	r0,r0,r5
	str	r0,[r7]

l00009FC8:
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
	ldr	r3,[r2]
	lsls	r3,r3,#&1C
	bmi	$0000A1EC

l00009FF6:
	ldr	r3,[r0,#&2C]
	bic	r3,r3,#&10
	str	r3,[r0,#&2C]
	ldr	r3,[r0,#&30]

l0000A000:
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
	ldr	r3,[r2]
	lsls	r3,r3,#&1B
	bmi	$0000A234

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
	ldr	r3,[r2]
	lsls	r3,r3,#&1A
	bmi	$0000A254

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
	ldr	r4,[pc,#&18]                                           ; 0000A09A
	cmps	r0,r4
	ite	eq
	moveq	r4,#&15

l0000A082:
	mov	r4,#&16
	mov	r0,r4
	bl	$00009500
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095D8
0000A094             00 C0 00 40                             ...@       

;; UARTIntUnregister: 0000A098
UARTIntUnregister proc
	push	{r4,lr}
	ldr	r4,[pc,#&18]                                           ; 0000A0BA
	cmps	r0,r4
	ite	eq
	moveq	r4,#&15

l0000A0A2:
	mov	r4,#&16
	mov	r0,r4
	bl	$00009634
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009534
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

;; fn0000A0C8: 0000A0C8
fn0000A0C8 proc
	bx	lr
0000A0CA                               00 BF                       ..   

;; UARTIntStatus: 0000A0CC
UARTIntStatus proc
	cbnz	r1,$0000A0D2

l0000A0CE:
	ldr	r0,[r0,#&3C]
	bx	lr

l0000A0D2:
	ldr	r0,[r0,#&40]

;; fn0000A0D4: 0000A0D4
fn0000A0D4 proc
	bx	lr
0000A0D6                   00 BF                               ..       

;; UARTIntClear: 0000A0D8
UARTIntClear proc
	str	r1,[r0,#&44]
	bx	lr

;; CPUcpsie: 0000A0DC
CPUcpsie proc
	Invalid
	bx	lr
	bx	lr
	nop

;; CPUcpsid: 0000A0E4
CPUcpsid proc
	Invalid
	bx	lr

;; fn0000A0E8: 0000A0E8
fn0000A0E8 proc
	bx	lr
0000A0EA                               00 BF                       ..   

;; CPUwfi: 0000A0EC
CPUwfi proc
	wfi
	bx	lr

;; fn0000A0F0: 0000A0F0
fn0000A0F0 proc
	bx	lr
0000A0F2       00 BF                                       ..           

;; I2CMasterInit: 0000A0F4
I2CMasterInit proc
	push	{r3-r5,lr}
	mov	r5,r1
	ldr	r2,[r0,#&20]
	mov	r4,r0
	orr	r2,r2,#&10
	str	r2,[r0,#&20]
	bl	$00009DEC
	ldr	r3,[pc,#&18]                                           ; 0000A126
	ldr	r2,[pc,#&18]                                           ; 0000A128
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

l0000A152:
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
	bl	$00009500
	pop.w	{r3,lr}
	mov	r0,#&18
	b	$000095D8
0000A192       00 BF                                       ..           

;; I2CIntUnregister: 0000A194
I2CIntUnregister proc
	push	{r3,lr}
	mov	r0,#&18
	bl	$00009634
	pop.w	{r3,lr}
	mov	r0,#&18
	b	$00009534
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

;; fn0000A1C4: 0000A1C4
fn0000A1C4 proc
	bx	lr
0000A1C6                   00 BF                               ..       

;; I2CMasterIntStatus: 0000A1C8
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

;; fn0000A1EC: 0000A1EC
fn0000A1EC proc
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

;; fn0000A204: 0000A204
fn0000A204 proc
	bx	lr
0000A206                   00 BF                               ..       

;; I2CMasterSlaveAddrSet: 0000A208
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

;; fn0000A21C: 0000A21C
fn0000A21C proc
	asrss	r0,r0,#2
	bx	lr

;; I2CMasterControl: 0000A220
I2CMasterControl proc
	str	r1,[r0,#&4]
	bx	lr

;; I2CMasterErr: 0000A224
I2CMasterErr proc
	ldr	r3,[r0,#&4]
	lsls	r2,r3,#&1F
	bmi	$0000A232

l0000A22A:
	ands	r0,r3,#2
	beq	$0000A234

l0000A230:
	and	r0,r3,#&1C

l0000A232:
	mov	r4,r3

l0000A234:
	bx	lr
0000A236                   00 20                               .        

;; fn0000A238: 0000A238
fn0000A238 proc
	bx	lr
0000A23A                               00 BF                       ..   

;; I2CMasterDataPut: 0000A23C
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
0000A250 48 65 6C 6C                                     Hell           

l0000A254:
	lsls	r7,r5,#1
	mov	r0,r0
	ldr	r3,[r0,#&4]
	str	r5,[r4,#&34]
	lsls	r3,r5,#1
	mov	r0,r0
	strb	r0,[r2,#&9]
	ldr	r1,[r5,#&64]
	lsls	r4,r6,#1
	mov	r0,r0
	ldr	r3,[r2,#&4]
	strb	r7,[r5,#&15]
	str	r4,[r5,#&44]
	ldr	r0,[r4,#&60]
	strb	r7,[r5,#&11]
	str	r0,[r4,#&20]
	mov	r0,#&65
	ldr	r4,[r6,#&4]
	strb	r5,[r4,#&9]
	lsls	r5,r4,#1
	adds	r1,r9
	cmps	r4,r9
	mov	r0,r0
	mov	r0,r0
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

;; fn0000A5C0: 0000A5C0
fn0000A5C0 proc
	mov	r0,r0
	lsls	r5,r7,#1
;;; Segment .text.memcpy (0000A5C4)

;; memcpy: 0000A5C4
memcpy proc
	push	{r4-r7,lr}
	mov	r5,r0
	cmps	r2,#&F
	bls	$0000A628

l0000A5CC:
	mov	r3,r1
	orrs	r3,r0
	lsls	r3,r3,#&1E
	bne	$0000A63E

l0000A5D4:
	mov	r6,r2
	mov	r4,r1
	mov	r3,r0
	subs	r6,#&10
	lsrs	r5,r6,#4
	adds	r5,#1
	lsls	r5,r5,#4
	add	r5,r0,r5
	ldr	r7,[r4]
	str	r7,[r3]
	ldr	r7,[r4,#&4]
	str	r7,[r3,#&4]
	ldr	r7,[r4,#&8]
	str	r7,[r3,#&8]
	ldr	r7,[r4,#&C]
	str	r7,[r3,#&C]
	adds	r3,#&10
	adds	r4,#&10
	cmps	r5,r3
	bne	$0000A7E0

l0000A5FC:
	mov	r3,#&F
	Invalid
	adds	r6,#&10
	add	r5,r0,r6
	add	r1,r1,r6
	ands	r3,r2
	cmps	r3,#3
	bls	$0000A642
	sub	r6,r3,#4
	mov	r3,#0
	lsrs	r4,r6,#2
	adds	r4,#1
	lsls	r4,r4,#2
	ldr	r7,[r1,r3]
	str	r7,[r5,r3]
	adds	r3,#4
	cmps	r3,r4
	bne	$0000A812
	mov	r4,#3
	Invalid
	add	r3,r6,#4
	ands	r2,r4

l0000A628:
	add	r1,r1,r3
	add	r5,r5,r3
	cmps	r2,#0
	beq	$0000A638

l0000A630:
	mov	r3,#0
	ldrb	r4,[r1,r3]
	strb	r4,[r5,r3]
	adds	r3,#1

l0000A638:
	cmps	r3,r2
	bne	$0000A82E

l0000A63C:
	pop	{r4-r7}

l0000A63E:
	pop	{r1}
	bx	r1
0000A642       05 00 F4 E7 1A 00 F0 E7 C0 46               .........F   
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
