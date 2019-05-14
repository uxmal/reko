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
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
F008 FA8C     	bl	$00008578
F895 4045     	ldrb	r4,[r5,#&45]
B264           	sxtb	r4,r4
2C00           	cmps	r4,#0
DD16           	ble	$00000098

l0000006A:
6A6B           	ldr	r3,[r5,#&24]
B1A3           	cbz	r3,$00000098

l0000006E:
F105 0624     	add	r6,r5,#&24
E005           	b	$00000080

l00000074:
3C01           	subs	r4,#1
B2E3           	uxtb	r3,r4
B25C           	sxtb	r4,r3
B16B           	cbz	r3,$00000098

l0000007C:
6A6B           	ldr	r3,[r5,#&24]
B15B           	cbz	r3,$00000098

l00000080:
4630           	mov	r0,r6
F000 FFCB     	bl	$0000101C
2800           	cmps	r0,#0
D0F4           	beq	$00000074

l0000008A:
3C01           	subs	r4,#1
F001 F88E     	bl	$000011AC
B2E3           	uxtb	r3,r4
B25C           	sxtb	r4,r3
2B00           	cmps	r3,#0
D1F1           	bne	$0000007C

l00000098:
23FF           	mov	r3,#&FF
F885 3045     	strb	r3,[r5,#&45]
F008 FA87     	bl	$000085B0
F008 FA69     	bl	$00008578
F895 4044     	ldrb	r4,[r5,#&44]
B264           	sxtb	r4,r4
2C00           	cmps	r4,#0
DD16           	ble	$000000DE

l000000B0:
692B           	ldr	r3,[r5,#&10]
B1A3           	cbz	r3,$000000DE

l000000B4:
F105 0610     	add	r6,r5,#&10
E005           	b	$000000C6

l000000BA:
3C01           	subs	r4,#1
B2E3           	uxtb	r3,r4
B25C           	sxtb	r4,r3
B16B           	cbz	r3,$000000DE

l000000C2:
692B           	ldr	r3,[r5,#&10]
B15B           	cbz	r3,$000000DE

l000000C6:
4630           	mov	r0,r6
F000 FFA8     	bl	$0000101C
2800           	cmps	r0,#0
D0F4           	beq	$000000BA

l000000D0:
3C01           	subs	r4,#1
F001 F86B     	bl	$000011AC
B2E3           	uxtb	r3,r4
B25C           	sxtb	r4,r3
2B00           	cmps	r3,#0
D1F1           	bne	$000000C2

l000000DE:
23FF           	mov	r3,#&FF
F885 3044     	strb	r3,[r5,#&44]
E8BD 4070     	pop.w	{r4-r6,lr}
F008 BA62     	b	$000085B0

;; prvCopyDataToQueue: 000000EC
;;   Called from:
;;     0000024C (in xQueueGenericSend)
;;     0000048E (in xQueueGenericSendFromISR)
;;     000083C6 (in xQueueCRSend)
;;     000084B4 (in xQueueCRSendFromISR)
prvCopyDataToQueue proc
B570           	push	{r4-r6,lr}
4604           	mov	r4,r0
6C00           	ldr	r0,[r0,#&40]
6BA5           	ldr	r5,[r4,#&38]
B928           	cbnz	r0,$00000102

l000000F6:
6826           	ldr	r6,[r4]
2E00           	cmps	r6,#0
D031           	beq	$00000160

l000000FC:
3501           	adds	r5,#1

l000000FE:
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}

l00000102:
4616           	mov	r6,r2
4602           	mov	r2,r0
B97E           	cbnz	r6,$00000128

l00000108:
68A0           	ldr	r0,[r4,#&8]
F00A FA5B     	bl	$0000A5C4
68A3           	ldr	r3,[r4,#&8]
6C21           	ldr	r1,[r4,#&40]
6862           	ldr	r2,[r4,#&4]
440B           	adds	r3,r1
4293           	cmps	r3,r2
60A3           	str	r3,[r4,#&8]
D319           	blo	$00000150

l0000011C:
6823           	ldr	r3,[r4]
3501           	adds	r5,#1
4630           	mov	r0,r6
60A3           	str	r3,[r4,#&8]
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}

l00000128:
68E0           	ldr	r0,[r4,#&C]
F00A FA4B     	bl	$0000A5C4
6C22           	ldr	r2,[r4,#&40]
68E3           	ldr	r3,[r4,#&C]
4252           	rsbs	r2,r2
6821           	ldr	r1,[r4]
4413           	adds	r3,r2
428B           	cmps	r3,r1
60E3           	str	r3,[r4,#&C]
D202           	bhs	$00000144

l0000013E:
6863           	ldr	r3,[r4,#&4]
441A           	adds	r2,r3
60E2           	str	r2,[r4,#&C]

l00000144:
2E02           	cmps	r6,#2
D007           	beq	$00000158

l00000148:
3501           	adds	r5,#1
2000           	mov	r0,#0
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}

l00000150:
3501           	adds	r5,#1
4630           	mov	r0,r6
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}

l00000158:
B905           	cbnz	r5,$0000015C

l0000015A:
2501           	mov	r5,#1

l0000015C:
2000           	mov	r0,#0
E7CE           	b	$000000FE

l00000160:
6860           	ldr	r0,[r4,#&4]
F001 F875     	bl	$00001250
3501           	adds	r5,#1
6066           	str	r6,[r4,#&4]
E7C8           	b	$000000FE

;; prvCopyDataFromQueue: 0000016C
;;   Called from:
;;     000002CA (in xQueuePeekFromISR)
;;     000003B6 (in xQueueGenericReceive)
;;     00000554 (in xQueueReceiveFromISR)
prvCopyDataFromQueue proc
6C02           	ldr	r2,[r0,#&40]
B16A           	cbz	r2,$0000018C

l00000170:
460B           	mov	r3,r1
B410           	push	{r4}
68C1           	ldr	r1,[r0,#&C]
6844           	ldr	r4,[r0,#&4]
4411           	adds	r1,r2
42A1           	cmps	r1,r4
60C1           	str	r1,[r0,#&C]
BF24           	itt	hs
6801           	ldrhs	r1,[r0]

l00000182:
60C1           	str	r1,[r0,#&C]
BC10           	pop	{r4}
4618           	mov	r0,r3
F00A BA1C     	b	$0000A5C4

l0000018C:
4770           	bx	lr
0000018E                                           00 BF               ..

;; xQueueGenericSend: 00000190
;;   Called from:
;;     00000628 (in xQueueGiveMutexRecursive)
;;     000006F8 (in xQueueCreateMutex)
;;     00008AFE (in MPU_xQueueGenericSend)
xQueueGenericSend proc
E92D 47F0     	push.w	{r4-r10,lr}
2500           	mov	r5,#0
B084           	sub	sp,#&10
4604           	mov	r4,r0
468A           	mov	r10,r1
9201           	str	r2,[sp,#&4]
461F           	mov	r7,r3
46A8           	mov	r8,r5
F8DF 90FC     	ldr	r9,[000002A0]                            ; [pc,#&FC]
E027           	b	$000001F8

l000001A8:
F008 FA02     	bl	$000085B0
F000 FC2E     	bl	$00000A0C
F008 F9E2     	bl	$00008578
F894 3044     	ldrb	r3,[r4,#&44]
2BFF           	cmps	r3,#&FF
BF08           	it	eq
F884 8044     	strbeq	r8,[r4,#&44]

l000001C0:
F894 3045     	ldrb	r3,[r4,#&45]
2BFF           	cmps	r3,#&FF
BF08           	it	eq
F884 8045     	strbeq	r8,[r4,#&45]

l000001CC:
F008 F9F0     	bl	$000085B0
A901           	add	r1,sp,#4
A802           	add	r0,sp,#8
F000 FFC0     	bl	$00001158
2800           	cmps	r0,#0
D150           	bne	$0000027E

l000001DC:
F008 F9CC     	bl	$00008578
6BA2           	ldr	r2,[r4,#&38]
6BE3           	ldr	r3,[r4,#&3C]
429A           	cmps	r2,r3
D017           	beq	$00000218

l000001E8:
F008 F9E2     	bl	$000085B0
4620           	mov	r0,r4
F7FF FF33     	bl	$00000058
F000 FE3B     	bl	$00000E6C

l000001F6:
2501           	mov	r5,#1

l000001F8:
F008 F9BE     	bl	$00008578
6BA2           	ldr	r2,[r4,#&38]
6BE3           	ldr	r3,[r4,#&3C]
429A           	cmps	r2,r3
D320           	blo	$00000246

l00000204:
2F02           	cmps	r7,#2
D01E           	beq	$00000246

l00000208:
9E01           	ldr	r6,[sp,#&4]
B396           	cbz	r6,$00000272

l0000020C:
2D00           	cmps	r5,#0
D1CB           	bne	$000001A8

l00000210:
A802           	add	r0,sp,#8
F000 FF97     	bl	$00001144
E7C7           	b	$000001A8

l00000218:
F008 F9CA     	bl	$000085B0
9901           	ldr	r1,[sp,#&4]
F104 0010     	add	r0,r4,#&10
F000 FEDB     	bl	$00000FDC
4620           	mov	r0,r4
F7FF FF16     	bl	$00000058
F000 FE1E     	bl	$00000E6C
2800           	cmps	r0,#0
D1E0           	bne	$000001F6

l00000234:
F04F 5380     	mov	r3,#&10000000
F8C9 3000     	str	r3,[r9]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7D7           	b	$000001F6

l00000246:
463A           	mov	r2,r7
4651           	mov	r1,r10
4620           	mov	r0,r4
F7FF FF4E     	bl	$000000EC
6A63           	ldr	r3,[r4,#&24]
B9EB           	cbnz	r3,$00000290

l00000254:
B138           	cbz	r0,$00000266

l00000256:
F04F 5280     	mov	r2,#&10000000
4B11           	ldr	r3,[000002A0]                           ; [pc,#&44]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l00000266:
F008 F9A3     	bl	$000085B0
2001           	mov	r0,#1
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l00000272:
F008 F99D     	bl	$000085B0
4630           	mov	r0,r6
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l0000027E:
4620           	mov	r0,r4
F7FF FEEA     	bl	$00000058
F000 FDF2     	bl	$00000E6C
2000           	mov	r0,#0
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l00000290:
F104 0024     	add	r0,r4,#&24
F000 FEC2     	bl	$0000101C
2800           	cmps	r0,#0
D1DC           	bne	$00000256

l0000029C:
E7E3           	b	$00000266
0000029E                                           00 BF               ..
000002A0 04 ED 00 E0                                     ....           

;; xQueuePeekFromISR: 000002A4
;;   Called from:
;;     00008BB4 (in MPU_xQueuePeekFromISR)
xQueuePeekFromISR proc
B570           	push	{r4-r6,lr}
F3EF 8511     	mrs	r5,cpsr
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6B83           	ldr	r3,[r0,#&38]
B91B           	cbnz	r3,$000002C6

l000002BE:
4618           	mov	r0,r3
F385 8811     	msr	cpsr,r5
BD70           	pop	{r4-r6,pc}

l000002C6:
4604           	mov	r4,r0
68C6           	ldr	r6,[r0,#&C]
F7FF FF4F     	bl	$0000016C
60E6           	str	r6,[r4,#&C]
2001           	mov	r0,#1
F385 8811     	msr	cpsr,r5
BD70           	pop	{r4-r6,pc}

;; xQueueGenericReceive: 000002D8
;;   Called from:
;;     000005EC (in xQueueTakeMutexRecursive)
;;     00008B86 (in MPU_xQueueGenericReceive)
xQueueGenericReceive proc
E92D 47F0     	push.w	{r4-r10,lr}
2500           	mov	r5,#0
B084           	sub	sp,#&10
4604           	mov	r4,r0
468A           	mov	r10,r1
9201           	str	r2,[sp,#&4]
4699           	mov	r9,r3
462F           	mov	r7,r5
F8DF 8138     	ldr	r8,[00000424]                            ; [pc,#&138]
E00C           	b	$0000030A

l000002F0:
F008 F942     	bl	$00008578
6BA3           	ldr	r3,[r4,#&38]
2B00           	cmps	r3,#0
D037           	beq	$0000036A

l000002FA:
F008 F959     	bl	$000085B0
4620           	mov	r0,r4
F7FF FEAA     	bl	$00000058
F000 FDB2     	bl	$00000E6C

l00000308:
2501           	mov	r5,#1

l0000030A:
F008 F935     	bl	$00008578
6BA6           	ldr	r6,[r4,#&38]
2E00           	cmps	r6,#0
D14D           	bne	$000003B0

l00000314:
9B01           	ldr	r3,[sp,#&4]
2B00           	cmps	r3,#0
D044           	beq	$000003A4

l0000031A:
2D00           	cmps	r5,#0
D03E           	beq	$0000039C

l0000031E:
F008 F947     	bl	$000085B0
F000 FB73     	bl	$00000A0C
F008 F927     	bl	$00008578
F894 3044     	ldrb	r3,[r4,#&44]
2BFF           	cmps	r3,#&FF
BF08           	it	eq
F884 7044     	strbeq	r7,[r4,#&44]

l00000336:
F894 3045     	ldrb	r3,[r4,#&45]
2BFF           	cmps	r3,#&FF
BF08           	it	eq
F884 7045     	strbeq	r7,[r4,#&45]

l00000342:
F008 F935     	bl	$000085B0
A901           	add	r1,sp,#4
A802           	add	r0,sp,#8
F000 FF05     	bl	$00001158
2800           	cmps	r0,#0
D0CE           	beq	$000002F0

l00000352:
4620           	mov	r0,r4
F7FF FE80     	bl	$00000058
F000 FD88     	bl	$00000E6C
F008 F90C     	bl	$00008578
6BA3           	ldr	r3,[r4,#&38]
B1FB           	cbz	r3,$000003A4

l00000364:
F008 F924     	bl	$000085B0
E7CE           	b	$00000308

l0000036A:
F008 F921     	bl	$000085B0
6823           	ldr	r3,[r4]
B393           	cbz	r3,$000003D8

l00000372:
9901           	ldr	r1,[sp,#&4]
F104 0024     	add	r0,r4,#&24
F000 FE30     	bl	$00000FDC
4620           	mov	r0,r4
F7FF FE6B     	bl	$00000058
F000 FD73     	bl	$00000E6C
2800           	cmps	r0,#0
D1BE           	bne	$00000308

l0000038A:
F04F 5380     	mov	r3,#&10000000
F8C8 3000     	str	r3,[r8]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7B5           	b	$00000308

l0000039C:
A802           	add	r0,sp,#8
F000 FED1     	bl	$00001144
E7BC           	b	$0000031E

l000003A4:
F008 F904     	bl	$000085B0
2000           	mov	r0,#0
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l000003B0:
4651           	mov	r1,r10
4620           	mov	r0,r4
68E5           	ldr	r5,[r4,#&C]
F7FF FED9     	bl	$0000016C
F1B9 0F00     	cmp	r9,#0
D113           	bne	$000003E8

l000003C0:
6823           	ldr	r3,[r4]
3E01           	subs	r6,#1
63A6           	str	r6,[r4,#&38]
B34B           	cbz	r3,$0000041C

l000003C8:
6923           	ldr	r3,[r4,#&10]
BB03           	cbnz	r3,$0000040E

l000003CC:
F008 F8F0     	bl	$000085B0
2001           	mov	r0,#1
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l000003D8:
F008 F8CE     	bl	$00008578
6860           	ldr	r0,[r4,#&4]
F000 FEED     	bl	$000011BC
F008 F8E5     	bl	$000085B0
E7C4           	b	$00000372

l000003E8:
6A63           	ldr	r3,[r4,#&24]
60E5           	str	r5,[r4,#&C]
2B00           	cmps	r3,#0
D0ED           	beq	$000003CC

l000003F0:
F104 0024     	add	r0,r4,#&24
F000 FE12     	bl	$0000101C
2800           	cmps	r0,#0
D0E7           	beq	$000003CC

l000003FC:
F04F 5280     	mov	r2,#&10000000
4B08           	ldr	r3,[00000424]                           ; [pc,#&20]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7DE           	b	$000003CC

l0000040E:
F104 0010     	add	r0,r4,#&10
F000 FE03     	bl	$0000101C
2800           	cmps	r0,#0
D1F0           	bne	$000003FC

l0000041A:
E7D7           	b	$000003CC

l0000041C:
F000 FF5A     	bl	$000012D4
6060           	str	r0,[r4,#&4]
E7D1           	b	$000003C8
00000424             04 ED 00 E0                             ....       

;; uxQueueMessagesWaiting: 00000428
;;   Called from:
;;     00008B28 (in MPU_uxQueueMessagesWaiting)
uxQueueMessagesWaiting proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F008 F8A4     	bl	$00008578
6BA4           	ldr	r4,[r4,#&38]
F008 F8BD     	bl	$000085B0
4620           	mov	r0,r4
BD10           	pop	{r4,pc}
0000043A                               00 BF                       ..   

;; uxQueueSpacesAvailable: 0000043C
;;   Called from:
;;     00008B50 (in MPU_uxQueueSpacesAvailable)
uxQueueSpacesAvailable proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F008 F89A     	bl	$00008578
6BA8           	ldr	r0,[r5,#&38]
6BEC           	ldr	r4,[r5,#&3C]
1A24           	sub	r4,r4,r0
F008 F8B1     	bl	$000085B0
4620           	mov	r0,r4
BD38           	pop	{r3-r5,pc}
00000452       00 BF                                       ..           

;; vQueueDelete: 00000454
;;   Called from:
;;     00008C80 (in MPU_vQueueDelete)
vQueueDelete proc
F001 B994     	b	$00001780

;; xQueueGenericSendFromISR: 00000458
;;   Called from:
;;     0000816E (in vUART_ISR)
xQueueGenericSendFromISR proc
B5F8           	push	{r3-r7,lr}
F3EF 8611     	mrs	r6,cpsr
F04F 04BF     	mov	r4,#&BF
F384 8811     	msr	cpsr,r4
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6B85           	ldr	r5,[r0,#&38]
6BC4           	ldr	r4,[r0,#&3C]
42A5           	cmps	r5,r4
D305           	blo	$00000482

l00000476:
2B02           	cmps	r3,#2
D003           	beq	$00000482

l0000047A:
2000           	mov	r0,#0

l0000047C:
F386 8811     	msr	cpsr,r6
BDF8           	pop	{r3-r7,pc}

l00000482:
F890 4045     	ldrb	r4,[r0,#&45]
4617           	mov	r7,r2
B264           	sxtb	r4,r4
461A           	mov	r2,r3
4605           	mov	r5,r0
F7FF FE2D     	bl	$000000EC
1C63           	add	r3,r4,#1
D007           	beq	$000004A6

l00000496:
3401           	adds	r4,#1
B264           	sxtb	r4,r4
F885 4045     	strb	r4,[r5,#&45]

l0000049E:
2001           	mov	r0,#1
F386 8811     	msr	cpsr,r6
BDF8           	pop	{r3-r7,pc}

l000004A6:
6A6B           	ldr	r3,[r5,#&24]
2B00           	cmps	r3,#0
D0F8           	beq	$0000049E

l000004AC:
F105 0024     	add	r0,r5,#&24
F000 FDB4     	bl	$0000101C
2800           	cmps	r0,#0
D0F2           	beq	$0000049E

l000004B8:
2F00           	cmps	r7,#0
D0F0           	beq	$0000049E

l000004BC:
2001           	mov	r0,#1
6038           	str	r0,[r7]
E7DC           	b	$0000047C
000004C2       00 BF                                       ..           

;; xQueueGiveFromISR: 000004C4
xQueueGiveFromISR proc
B538           	push	{r3-r5,lr}
F3EF 8411     	mrs	r4,cpsr
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6B82           	ldr	r2,[r0,#&38]
6BC3           	ldr	r3,[r0,#&3C]
429A           	cmps	r2,r3
D20E           	bhs	$00000500

l000004E2:
F890 3045     	ldrb	r3,[r0,#&45]
3201           	adds	r2,#1
B25B           	sxtb	r3,r3
6382           	str	r2,[r0,#&38]
1C5A           	add	r2,r3,#1
D00B           	beq	$00000508

l000004F0:
3301           	adds	r3,#1
B25B           	sxtb	r3,r3
F880 3045     	strb	r3,[r0,#&45]

l000004F8:
2001           	mov	r0,#1

l000004FA:
F384 8811     	msr	cpsr,r4
BD38           	pop	{r3-r5,pc}

l00000500:
2000           	mov	r0,#0
F384 8811     	msr	cpsr,r4
BD38           	pop	{r3-r5,pc}

l00000508:
6A43           	ldr	r3,[r0,#&24]
2B00           	cmps	r3,#0
D0F4           	beq	$000004F8

l0000050E:
3024           	adds	r0,#&24
460D           	mov	r5,r1
F000 FD83     	bl	$0000101C
2800           	cmps	r0,#0
D0EE           	beq	$000004F8

l0000051A:
2D00           	cmps	r5,#0
D0EC           	beq	$000004F8

l0000051E:
2001           	mov	r0,#1
6028           	str	r0,[r5]
E7EA           	b	$000004FA

;; xQueueReceiveFromISR: 00000524
xQueueReceiveFromISR proc
E92D 41F0     	push.w	{r4-r8,lr}
F3EF 8611     	mrs	r6,cpsr
F04F 04BF     	mov	r4,#&BF
F384 8811     	msr	cpsr,r4
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6B84           	ldr	r4,[r0,#&38]
B924           	cbnz	r4,$0000054A

l00000540:
4620           	mov	r0,r4

l00000542:
F386 8811     	msr	cpsr,r6
E8BD 81F0     	pop.w	{r4-r8,pc}

l0000054A:
4607           	mov	r7,r0
F890 5044     	ldrb	r5,[r0,#&44]
4690           	mov	r8,r2
B26D           	sxtb	r5,r5
F7FF FE0A     	bl	$0000016C
3C01           	subs	r4,#1
1C6B           	add	r3,r5,#1
63BC           	str	r4,[r7,#&38]
D008           	beq	$00000572

l00000560:
3501           	adds	r5,#1
B26D           	sxtb	r5,r5
F887 5044     	strb	r5,[r7,#&44]

l00000568:
2001           	mov	r0,#1
F386 8811     	msr	cpsr,r6
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000572:
693B           	ldr	r3,[r7,#&10]
2B00           	cmps	r3,#0
D0F7           	beq	$00000568

l00000578:
F107 0010     	add	r0,r7,#&10
F000 FD4E     	bl	$0000101C
2800           	cmps	r0,#0
D0F1           	beq	$00000568

l00000584:
F1B8 0F00     	cmp	r8,#0
D0EE           	beq	$00000568

l0000058A:
2001           	mov	r0,#1
F8C8 0000     	str	r0,[r8]
E7D7           	b	$00000542
00000592       00 BF                                       ..           

;; xQueueIsQueueEmptyFromISR: 00000594
xQueueIsQueueEmptyFromISR proc
6B80           	ldr	r0,[r0,#&38]
FAB0 F080     	clz	r0,r0
0940           	lsrs	r0,r0,#5
4770           	bx	lr
0000059E                                           00 BF               ..

;; xQueueIsQueueFullFromISR: 000005A0
xQueueIsQueueFullFromISR proc
6B83           	ldr	r3,[r0,#&38]
6BC0           	ldr	r0,[r0,#&3C]
1AC0           	sub	r0,r0,r3
FAB0 F080     	clz	r0,r0
0940           	lsrs	r0,r0,#5
4770           	bx	lr
000005AE                                           00 BF               ..

;; uxQueueMessagesWaitingFromISR: 000005B0
uxQueueMessagesWaitingFromISR proc
6B80           	ldr	r0,[r0,#&38]
4770           	bx	lr

;; xQueueGetMutexHolder: 000005B4
;;   Called from:
;;     00008BDC (in MPU_xQueueGetMutexHolder)
xQueueGetMutexHolder proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F007 FFDE     	bl	$00008578
6823           	ldr	r3,[r4]
B923           	cbnz	r3,$000005CA

l000005C0:
6864           	ldr	r4,[r4,#&4]
F007 FFF5     	bl	$000085B0
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

l000005CA:
2400           	mov	r4,#0
F007 FFF0     	bl	$000085B0
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

;; xQueueTakeMutexRecursive: 000005D4
;;   Called from:
;;     00008C30 (in MPU_xQueueTakeMutexRecursive)
xQueueTakeMutexRecursive proc
B570           	push	{r4-r6,lr}
6845           	ldr	r5,[r0,#&4]
4604           	mov	r4,r0
460E           	mov	r6,r1
F000 FDAC     	bl	$00001138
4285           	cmps	r5,r0
D00A           	beq	$000005FA

l000005E4:
2300           	mov	r3,#0
4632           	mov	r2,r6
4619           	mov	r1,r3
4620           	mov	r0,r4
F7FF FE74     	bl	$000002D8
B110           	cbz	r0,$000005F8

l000005F2:
68E3           	ldr	r3,[r4,#&C]
3301           	adds	r3,#1
60E3           	str	r3,[r4,#&C]

l000005F8:
BD70           	pop	{r4-r6,pc}

l000005FA:
2001           	mov	r0,#1
68E3           	ldr	r3,[r4,#&C]
4403           	adds	r3,r0
60E3           	str	r3,[r4,#&C]
BD70           	pop	{r4-r6,pc}

;; xQueueGiveMutexRecursive: 00000604
;;   Called from:
;;     00008C58 (in MPU_xQueueGiveMutexRecursive)
xQueueGiveMutexRecursive proc
B538           	push	{r3-r5,lr}
6845           	ldr	r5,[r0,#&4]
4604           	mov	r4,r0
F000 FD95     	bl	$00001138
4285           	cmps	r5,r0
D001           	beq	$00000616

l00000612:
2000           	mov	r0,#0
BD38           	pop	{r3-r5,pc}

l00000616:
68E3           	ldr	r3,[r4,#&C]
3B01           	subs	r3,#1
60E3           	str	r3,[r4,#&C]
B10B           	cbz	r3,$00000622

l0000061E:
2001           	mov	r0,#1
BD38           	pop	{r3-r5,pc}

l00000622:
4620           	mov	r0,r4
461A           	mov	r2,r3
4619           	mov	r1,r3
F7FF FDB2     	bl	$00000190
2001           	mov	r0,#1
BD38           	pop	{r3-r5,pc}

;; xQueueGenericReset: 00000630
;;   Called from:
;;     000006D0 (in xQueueGenericCreate)
;;     00008AC8 (in MPU_xQueueGenericReset)
xQueueGenericReset proc
B570           	push	{r4-r6,lr}
4604           	mov	r4,r0
460E           	mov	r6,r1
25FF           	mov	r5,#&FF
F007 FF9E     	bl	$00008578
2100           	mov	r1,#0
6C23           	ldr	r3,[r4,#&40]
6BE2           	ldr	r2,[r4,#&3C]
6820           	ldr	r0,[r4]
FB02 F203     	mul	r2,r2,r3
1AD3           	sub	r3,r2,r3
4403           	adds	r3,r0
4402           	adds	r2,r0
63A1           	str	r1,[r4,#&38]
6062           	str	r2,[r4,#&4]
F884 5044     	strb	r5,[r4,#&44]
60E3           	str	r3,[r4,#&C]
60A0           	str	r0,[r4,#&8]
F884 5045     	strb	r5,[r4,#&45]
B9BE           	cbnz	r6,$00000690

l00000660:
6923           	ldr	r3,[r4,#&10]
B91B           	cbnz	r3,$0000066C

l00000664:
F007 FFA4     	bl	$000085B0
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}

l0000066C:
F104 0010     	add	r0,r4,#&10
F000 FCD4     	bl	$0000101C
2800           	cmps	r0,#0
D0F5           	beq	$00000664

l00000678:
F04F 5280     	mov	r2,#&10000000
4B0A           	ldr	r3,[000006A8]                           ; [pc,#&28]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
F007 FF92     	bl	$000085B0
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}

l00000690:
F104 0010     	add	r0,r4,#&10
F007 FE1C     	bl	$000082D0
F104 0024     	add	r0,r4,#&24
F007 FE18     	bl	$000082D0
F007 FF86     	bl	$000085B0
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}
000006A8                         04 ED 00 E0                     ....   

;; xQueueGenericCreate: 000006AC
;;   Called from:
;;     000006E4 (in xQueueCreateMutex)
;;     00008A9C (in MPU_xQueueGenericCreate)
xQueueGenericCreate proc
B570           	push	{r4-r6,lr}
4606           	mov	r6,r0
FB00 F001     	mul	r0,r0,r1
3048           	adds	r0,#&48
460D           	mov	r5,r1
F001 F838     	bl	$0000172C
4604           	mov	r4,r0
B148           	cbz	r0,$000006D4

l000006C0:
B155           	cbz	r5,$000006D8

l000006C2:
F100 0348     	add	r3,r0,#&48
6003           	str	r3,[r0]

l000006C8:
63E6           	str	r6,[r4,#&3C]
6425           	str	r5,[r4,#&40]
2101           	mov	r1,#1
4620           	mov	r0,r4
F7FF FFAE     	bl	$00000630

l000006D4:
4620           	mov	r0,r4
BD70           	pop	{r4-r6,pc}

l000006D8:
6020           	str	r0,[r4]
E7F5           	b	$000006C8

;; xQueueCreateMutex: 000006DC
;;   Called from:
;;     00008C04 (in MPU_xQueueCreateMutex)
xQueueCreateMutex proc
B510           	push	{r4,lr}
4602           	mov	r2,r0
2100           	mov	r1,#0
2001           	mov	r0,#1
F7FF FFE2     	bl	$000006AC
4604           	mov	r4,r0
B138           	cbz	r0,$000006FC

l000006EC:
2300           	mov	r3,#0
6043           	str	r3,[r0,#&4]
6003           	str	r3,[r0]
60C3           	str	r3,[r0,#&C]
461A           	mov	r2,r3
4619           	mov	r1,r3
F7FF FD4A     	bl	$00000190

l000006FC:
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

;; prvInitialiseNewTask: 00000700
;;   Called from:
;;     000008F2 (in xTaskCreate)
;;     00000954 (in xTaskCreateRestricted)
prvInitialiseNewTask proc
E92D 4FF8     	push.w	{r3-fp,lr}
9C0C           	ldr	r4,[sp,#&30]
4699           	mov	r9,r3
6D25           	ldr	r5,[r4,#&50]
F102 4380     	add	r3,r2,#&40000000
3B01           	subs	r3,#1
4693           	mov	fp,r2
9A0A           	ldr	r2,[sp,#&28]
EB05 0583     	add.w	r5,r5,r3,lsl #2
4680           	mov	r8,r0
1E4B           	sub	r3,r1,#1
EA4F 7AD2     	mov.w	r10,r2,lsr #&1F
F025 0507     	bic	r5,r5,#7
3102           	adds	r1,#2
F104 0054     	add	r0,r4,#&54
F022 4200     	bic	r2,r2,#&80000000

l0000072E:
785E           	ldrb	r6,[r3,#&1]
F800 6B01     	strb	r6,[r0],#&1
F813 6F01     	ldrb	r6,[r3,#&1]!
B10E           	cbz	r6,$0000073E

l0000073A:
428B           	cmps	r3,r1
D1F7           	bne	$0000072E

l0000073E:
2A01           	cmps	r2,#1
BF28           	it	hs
2201           	movhs	r2,#1

l00000744:
2600           	mov	r6,#0
4617           	mov	r7,r2
64E2           	str	r2,[r4,#&4C]
65A2           	str	r2,[r4,#&58]
F104 0024     	add	r0,r4,#&24
F884 6056     	strb	r6,[r4,#&56]
65E6           	str	r6,[r4,#&5C]
F007 FDC7     	bl	$000082E8
F104 0038     	add	r0,r4,#&38
F007 FDC3     	bl	$000082E8
F1C7 0302     	rsb	r3,r7,#2
63A3           	str	r3,[r4,#&38]
6D22           	ldr	r2,[r4,#&50]
465B           	mov	r3,fp
990D           	ldr	r1,[sp,#&34]
1D20           	add	r0,r4,#4
6324           	str	r4,[r4,#&30]
6464           	str	r4,[r4,#&44]
F000 FEEE     	bl	$00001554
6626           	str	r6,[r4,#&60]
4653           	mov	r3,r10
F884 6064     	strb	r6,[r4,#&64]
464A           	mov	r2,r9
4641           	mov	r1,r8
4628           	mov	r0,r5
F000 FDF9     	bl	$0000137C
9B0B           	ldr	r3,[sp,#&2C]
6020           	str	r0,[r4]
B103           	cbz	r3,$00000792

l00000790:
601C           	str	r4,[r3]

l00000792:
E8BD 8FF8     	pop.w	{r3-fp,pc}
00000796                   00 BF                               ..       

;; prvAddNewTaskToReadyList: 00000798
;;   Called from:
;;     000008F8 (in xTaskCreate)
;;     0000095A (in xTaskCreateRestricted)
prvAddNewTaskToReadyList proc
E92D 41F0     	push.w	{r4-r8,lr}
4C2D           	ldr	r4,[00000854]                           ; [pc,#&B4]
4605           	mov	r5,r0
F007 FEEA     	bl	$00008578
6823           	ldr	r3,[r4]
3301           	adds	r3,#1
6023           	str	r3,[r4]
6863           	ldr	r3,[r4,#&4]
2B00           	cmps	r3,#0
D030           	beq	$00000812

l000007B0:
6F63           	ldr	r3,[r4,#&74]
B32B           	cbz	r3,$00000800

l000007B4:
6CE8           	ldr	r0,[r5,#&4C]
F104 0608     	add	r6,r4,#8

l000007BA:
2301           	mov	r3,#1
6FE1           	ldr	r1,[r4,#&7C]
6FA2           	ldr	r2,[r4,#&78]
4083           	lsls	r3,r0
EB00 0080     	add.w	r0,r0,r0,lsl #2
430B           	orrs	r3,r1
3201           	adds	r2,#1
EB06 0080     	add.w	r0,r6,r0,lsl #2
F105 0124     	add	r1,r5,#&24
67E3           	str	r3,[r4,#&7C]
67A2           	str	r2,[r4,#&78]
F007 FD8B     	bl	$000082F0
F007 FEE9     	bl	$000085B0
6F63           	ldr	r3,[r4,#&74]
B163           	cbz	r3,$000007FC

l000007E2:
6862           	ldr	r2,[r4,#&4]
6CEB           	ldr	r3,[r5,#&4C]
6CD2           	ldr	r2,[r2,#&4C]
429A           	cmps	r2,r3
D207           	bhs	$000007FC

l000007EC:
F04F 5280     	mov	r2,#&10000000
4B19           	ldr	r3,[00000858]                           ; [pc,#&64]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l000007FC:
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000800:
6863           	ldr	r3,[r4,#&4]
6CE8           	ldr	r0,[r5,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
F104 0608     	add	r6,r4,#8
4283           	cmps	r3,r0
BF98           	it	ls
6065           	strls	r5,[r4,#&4]

l00000810:
E7D3           	b	$000007BA

l00000812:
6065           	str	r5,[r4,#&4]
6823           	ldr	r3,[r4]
2B01           	cmps	r3,#1
D1CC           	bne	$000007B4

l0000081A:
F104 0608     	add	r6,r4,#8
4630           	mov	r0,r6
F007 FD56     	bl	$000082D0
F104 0830     	add	r8,r4,#&30
F104 001C     	add	r0,r4,#&1C
F007 FD50     	bl	$000082D0
F104 0744     	add	r7,r4,#&44
4640           	mov	r0,r8
F007 FD4B     	bl	$000082D0
4638           	mov	r0,r7
F007 FD48     	bl	$000082D0
F104 0058     	add	r0,r4,#&58
F007 FD44     	bl	$000082D0
F8C4 806C     	str	r8,[r4,#&6C]
6CE8           	ldr	r0,[r5,#&4C]
6727           	str	r7,[r4,#&70]
E7B3           	b	$000007BA
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
B570           	push	{r4-r6,lr}
4C14           	ldr	r4,[000008B0]                           ; [pc,#&50]
4605           	mov	r5,r0
F8D4 6080     	ldr	r6,[r4,#&80]
6860           	ldr	r0,[r4,#&4]
3024           	adds	r0,#&24
F007 FD69     	bl	$00008340
B938           	cbnz	r0,$00000880

l00000870:
2201           	mov	r2,#1
6861           	ldr	r1,[r4,#&4]
6FE3           	ldr	r3,[r4,#&7C]
6CC9           	ldr	r1,[r1,#&4C]
408A           	lsls	r2,r1
EA23 0302     	bic.w	r3,r3,r2
67E3           	str	r3,[r4,#&7C]

l00000880:
4435           	adds	r5,r6
6863           	ldr	r3,[r4,#&4]
42AE           	cmps	r6,r5
625D           	str	r5,[r3,#&24]
D80B           	bhi	$000008A2

l0000088A:
6EE0           	ldr	r0,[r4,#&6C]
6861           	ldr	r1,[r4,#&4]
3124           	adds	r1,#&24
F007 FD3C     	bl	$0000830C
F8D4 3084     	ldr	r3,[r4,#&84]
429D           	cmps	r5,r3
BF38           	it	lo
F8C4 5084     	strlo	r5,[r4,#&84]

l000008A0:
BD70           	pop	{r4-r6,pc}

l000008A2:
6F20           	ldr	r0,[r4,#&70]
6861           	ldr	r1,[r4,#&4]
E8BD 4070     	pop.w	{r4-r6,lr}
3124           	adds	r1,#&24
F007 BD2E     	b	$0000830C
000008B0 C4 00 00 20                                     ...            

;; xTaskCreate: 000008B4
;;   Called from:
;;     000009AA (in vTaskStartScheduler)
;;     0000882C (in MPU_xTaskCreate)
xTaskCreate proc
E92D 47F0     	push.w	{r4-r10,lr}
4680           	mov	r8,r0
B084           	sub	sp,#&10
0090           	lsls	r0,r2,#2
4616           	mov	r6,r2
4689           	mov	r9,r1
469A           	mov	r10,r3
F000 FF32     	bl	$0000172C
B1E0           	cbz	r0,$00000904

l000008CA:
4605           	mov	r5,r0
2068           	mov	r0,#&68
F000 FF2D     	bl	$0000172C
4604           	mov	r4,r0
B1D8           	cbz	r0,$0000090E

l000008D6:
2700           	mov	r7,#0
6505           	str	r5,[r0,#&50]
9D0D           	ldr	r5,[sp,#&34]
F884 7065     	strb	r7,[r4,#&65]
9501           	str	r5,[sp,#&4]
9D0C           	ldr	r5,[sp,#&30]
4653           	mov	r3,r10
4632           	mov	r2,r6
4649           	mov	r1,r9
4640           	mov	r0,r8
9703           	str	r7,[sp,#&C]
9402           	str	r4,[sp,#&8]
9500           	str	r5,[sp]
F7FF FF05     	bl	$00000700
4620           	mov	r0,r4
F7FF FF4E     	bl	$00000798
2001           	mov	r0,#1

l000008FE:
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l00000904:
F04F 30FF     	mov	r0,#&FFFFFFFF
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l0000090E:
4628           	mov	r0,r5
F000 FF36     	bl	$00001780
F04F 30FF     	mov	r0,#&FFFFFFFF
E7F1           	b	$000008FE
0000091A                               00 BF                       ..   

;; xTaskCreateRestricted: 0000091C
;;   Called from:
;;     000087EC (in MPU_xTaskCreateRestricted)
xTaskCreateRestricted proc
6943           	ldr	r3,[r0,#&14]
B323           	cbz	r3,$0000096A

l00000920:
B5F0           	push	{r4-r7,lr}
4604           	mov	r4,r0
B085           	sub	sp,#&14
2068           	mov	r0,#&68
460F           	mov	r7,r1
F000 FEFF     	bl	$0000172C
4605           	mov	r5,r0
B1C0           	cbz	r0,$00000964

l00000932:
2601           	mov	r6,#1
6961           	ldr	r1,[r4,#&14]
F880 6065     	strb	r6,[r0,#&65]
68E3           	ldr	r3,[r4,#&C]
8922           	ldrh	r2,[r4,#&8]
F8D4 E010     	ldr	lr,[r4,#&10]
6501           	str	r1,[r0,#&50]
6861           	ldr	r1,[r4,#&4]
9002           	str	r0,[sp,#&8]
9701           	str	r7,[sp,#&4]
F854 0B18     	ldr	r0,[r4],#&18
F8CD E000     	str	lr,[sp]
9403           	str	r4,[sp,#&C]
F7FF FED4     	bl	$00000700
4628           	mov	r0,r5
F7FF FF1D     	bl	$00000798
4630           	mov	r0,r6

l00000960:
B005           	add	sp,#&14
BDF0           	pop	{r4-r7,pc}

l00000964:
F04F 30FF     	mov	r0,#&FFFFFFFF
E7FA           	b	$00000960

l0000096A:
F04F 30FF     	mov	r0,#&FFFFFFFF
4770           	bx	lr

;; vTaskAllocateMPURegions: 00000970
;;   Called from:
;;     0000885C (in MPU_vTaskAllocateMPURegions)
vTaskAllocateMPURegions proc
B120           	cbz	r0,$0000097C

l00000972:
2300           	mov	r3,#0
3004           	adds	r0,#4
461A           	mov	r2,r3
F000 BDEC     	b	$00001554

l0000097C:
4B03           	ldr	r3,[0000098C]                           ; [pc,#&C]
6858           	ldr	r0,[r3,#&4]
2300           	mov	r3,#0
3004           	adds	r0,#4
461A           	mov	r2,r3
F000 BDE5     	b	$00001554
0000098A                               00 BF C4 00 00 20           ..... 

;; vTaskStartScheduler: 00000990
;;   Called from:
;;     000080DE (in Main)
vTaskStartScheduler proc
F04F 4300     	mov	r3,#&80000000
B510           	push	{r4,lr}
4C12           	ldr	r4,[000009E0]                           ; [pc,#&48]
B082           	sub	sp,#8
9300           	str	r3,[sp]
F104 0388     	add	r3,r4,#&88
9301           	str	r3,[sp,#&4]
223B           	mov	r2,#&3B
2300           	mov	r3,#0
490F           	ldr	r1,[000009E4]                           ; [pc,#&3C]
480F           	ldr	r0,[000009E8]                           ; [pc,#&3C]
F7FF FF83     	bl	$000008B4
2801           	cmps	r0,#1
D001           	beq	$000009B6

l000009B2:
B002           	add	sp,#8
BD10           	pop	{r4,pc}

l000009B6:
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
F04F 32FF     	mov	r2,#&FFFFFFFF
2300           	mov	r3,#0
F8C4 2084     	str	r2,[r4,#&84]
6760           	str	r0,[r4,#&74]
F8C4 3080     	str	r3,[r4,#&80]
B002           	add	sp,#8
E8BD 4010     	pop.w	{r4,lr}
F000 BCE8     	b	$000013B0
000009E0 C4 00 00 20 7C A2 00 00 2D 85 00 00             ... |...-...   

;; vTaskEndScheduler: 000009EC
vTaskEndScheduler proc
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
2200           	mov	r2,#0
4B02           	ldr	r3,[00000A08]                           ; [pc,#&8]
675A           	str	r2,[r3,#&74]
F000 BDA5     	b	$00001550
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
4A03           	ldr	r2,[00000A1C]                           ; [pc,#&C]
F8D2 308C     	ldr	r3,[r2,#&8C]
3301           	adds	r3,#1
F8C2 308C     	str	r3,[r2,#&8C]
4770           	bx	lr
00000A1A                               00 BF C4 00 00 20           ..... 

;; xTaskGetTickCount: 00000A20
;;   Called from:
;;     0000890C (in MPU_xTaskGetTickCount)
xTaskGetTickCount proc
4B01           	ldr	r3,[00000A28]                           ; [pc,#&4]
F8D3 0080     	ldr	r0,[r3,#&80]
4770           	bx	lr
00000A28                         C4 00 00 20                     ...    

;; xTaskGetTickCountFromISR: 00000A2C
xTaskGetTickCountFromISR proc
4B01           	ldr	r3,[00000A34]                           ; [pc,#&4]
F8D3 0080     	ldr	r0,[r3,#&80]
4770           	bx	lr
00000A34             C4 00 00 20                             ...        

;; uxTaskGetNumberOfTasks: 00000A38
;;   Called from:
;;     00008930 (in MPU_uxTaskGetNumberOfTasks)
uxTaskGetNumberOfTasks proc
4B01           	ldr	r3,[00000A40]                           ; [pc,#&4]
6818           	ldr	r0,[r3]
4770           	bx	lr
00000A3E                                           00 BF               ..
00000A40 C4 00 00 20                                     ...            

;; pcTaskGetName: 00000A44
;;   Called from:
;;     00008958 (in MPU_pcTaskGetName)
pcTaskGetName proc
B108           	cbz	r0,$00000A4A

l00000A46:
3054           	adds	r0,#&54
4770           	bx	lr

l00000A4A:
4B02           	ldr	r3,[00000A54]                           ; [pc,#&8]
6858           	ldr	r0,[r3,#&4]
3054           	adds	r0,#&54
4770           	bx	lr
00000A52       00 BF C4 00 00 20                           .....        

;; xTaskGenericNotify: 00000A58
;;   Called from:
;;     000089DE (in MPU_xTaskGenericNotify)
xTaskGenericNotify proc
B5F8           	push	{r3-r7,lr}
461C           	mov	r4,r3
4606           	mov	r6,r0
460F           	mov	r7,r1
4615           	mov	r5,r2
F007 FD89     	bl	$00008578
B10C           	cbz	r4,$00000A6C

l00000A68:
6E33           	ldr	r3,[r6,#&60]
6023           	str	r3,[r4]

l00000A6C:
2302           	mov	r3,#2
F896 4064     	ldrb	r4,[r6,#&64]
1E6A           	sub	r2,r5,#1
F886 3064     	strb	r3,[r6,#&64]
B2E4           	uxtb	r4,r4
2A03           	cmps	r2,#3
D806           	bhi	$00000A8C

l00000A7E:
E8DF F002     	tbb	[pc,r2]                                  ; 00000A80

l00000A82:
0C3A           	lsrs	r2,r7,#&10
l00000A83	db	0x0C
l00000A84	db	0x04
l00000A85	db	0x02

l00000A86:
2C02           	cmps	r4,#2
D039           	beq	$00000AFE

l00000A8A:
6637           	str	r7,[r6,#&60]

l00000A8C:
2C01           	cmps	r4,#1
D00A           	beq	$00000AA6

l00000A90:
2401           	mov	r4,#1

l00000A92:
F007 FD8D     	bl	$000085B0

l00000A96:
4620           	mov	r0,r4
BDF8           	pop	{r3-r7,pc}
00000A9A                               33 6E 01 2C 03 F1           3n.,..
00000AA0 01 03 33 66 F4 D1                               ..3f..         

l00000AA6:
F106 0724     	add	r7,r6,#&24
4D16           	ldr	r5,[00000B04]                           ; [pc,#&58]
4638           	mov	r0,r7
F007 FC47     	bl	$00008340
6CF0           	ldr	r0,[r6,#&4C]
F8D5 E07C     	ldr	lr,[r5,#&7C]
F105 0208     	add	r2,r5,#8
FA04 F300     	lsl	r3,r4,r0
EB00 0080     	add.w	r0,r0,r0,lsl #2
EA43 030E     	orr	r3,r3,lr
EB02 0080     	add.w	r0,r2,r0,lsl #2
4639           	mov	r1,r7
67EB           	str	r3,[r5,#&7C]
F007 FC0E     	bl	$000082F0
686B           	ldr	r3,[r5,#&4]
6CF2           	ldr	r2,[r6,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
D9D8           	bls	$00000A90

l00000ADE:
F04F 5280     	mov	r2,#&10000000
4B09           	ldr	r3,[00000B08]                           ; [pc,#&24]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
F007 FD5F     	bl	$000085B0

l00000AF2:
4620           	mov	r0,r4
BDF8           	pop	{r3-r7,pc}
00000AF6                   33 6E 1F 43 37 66 C6 E7             3n.C7f.. 

l00000AFE:
2400           	mov	r4,#0
E7C7           	b	$00000A92
00000B02       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; xTaskGenericNotifyFromISR: 00000B0C
xTaskGenericNotifyFromISR proc
E92D 41F0     	push.w	{r4-r8,lr}
F3EF 8511     	mrs	r5,cpsr
F04F 04BF     	mov	r4,#&BF
F384 8811     	msr	cpsr,r4
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
B10B           	cbz	r3,$00000B2A

l00000B26:
6E04           	ldr	r4,[r0,#&60]
601C           	str	r4,[r3]

l00000B2A:
2302           	mov	r3,#2
F890 4064     	ldrb	r4,[r0,#&64]
3A01           	subs	r2,#1
F880 3064     	strb	r3,[r0,#&64]
B2E4           	uxtb	r4,r4
2A03           	cmps	r2,#3
D806           	bhi	$00000B4A

l00000B3C:
E8DF F002     	tbb	[pc,r2]                                  ; 00000B40

l00000B40:
0C2A           	lsrs	r2,r5,#&10
l00000B41	db	0x0C
l00000B42	db	0x04
l00000B43	db	0x02

l00000B44:
2C02           	cmps	r4,#2
D03D           	beq	$00000BC4

l00000B48:
6601           	str	r1,[r0,#&60]

l00000B4A:
2C01           	cmps	r4,#1
D00A           	beq	$00000B64

l00000B4E:
2001           	mov	r0,#1

l00000B50:
F385 8811     	msr	cpsr,r5

l00000B54:
E8BD 81F0     	pop.w	{r4-r8,pc}
00000B58                         03 6E 01 2C 03 F1 01 03         .n.,....
00000B60 03 66 F4 D1                                     .f..           

l00000B64:
4E1A           	ldr	r6,[00000BD0]                           ; [pc,#&68]
4607           	mov	r7,r0
F8D6 308C     	ldr	r3,[r6,#&8C]
B1B3           	cbz	r3,$00000B9C

l00000B6E:
F100 0138     	add	r1,r0,#&38
F106 0058     	add	r0,r6,#&58
F007 FBBB     	bl	$000082F0

l00000B7A:
6873           	ldr	r3,[r6,#&4]
6CFA           	ldr	r2,[r7,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
D9E4           	bls	$00000B4E

l00000B84:
9B06           	ldr	r3,[sp,#&18]
2001           	mov	r0,#1
B1F3           	cbz	r3,$00000BC8

l00000B8A:
6018           	str	r0,[r3]
F385 8811     	msr	cpsr,r5

l00000B90:
E8BD 81F0     	pop.w	{r4-r8,pc}
00000B94             03 6E 19 43 01 66 D6 E7                 .n.C.f..   

l00000B9C:
F100 0824     	add	r8,r0,#&24
4640           	mov	r0,r8
F007 FBCD     	bl	$00008340
6CF8           	ldr	r0,[r7,#&4C]
6FF2           	ldr	r2,[r6,#&7C]
4084           	lsls	r4,r0
F106 0308     	add	r3,r6,#8
EB00 0080     	add.w	r0,r0,r0,lsl #2
4314           	orrs	r4,r2
4641           	mov	r1,r8
EB03 0080     	add.w	r0,r3,r0,lsl #2
67F4           	str	r4,[r6,#&7C]
F007 FB97     	bl	$000082F0
E7DA           	b	$00000B7A

l00000BC4:
2000           	mov	r0,#0
E7C3           	b	$00000B50

l00000BC8:
F8C6 0090     	str	r0,[r6,#&90]
E7C0           	b	$00000B50
00000BCE                                           00 BF               ..
00000BD0 C4 00 00 20                                     ...            

;; xTaskNotifyWait: 00000BD4
;;   Called from:
;;     00008A16 (in MPU_xTaskNotifyWait)
xTaskNotifyWait proc
E92D 41F0     	push.w	{r4-r8,lr}
4C1F           	ldr	r4,[00000C58]                           ; [pc,#&7C]
4615           	mov	r5,r2
4680           	mov	r8,r0
460E           	mov	r6,r1
461F           	mov	r7,r3
F007 FCC9     	bl	$00008578
6862           	ldr	r2,[r4,#&4]
F892 2064     	ldrb	r2,[r2,#&64]
2A02           	cmps	r2,#2
D009           	beq	$00000C04

l00000BF0:
2001           	mov	r0,#1
6861           	ldr	r1,[r4,#&4]
6E0A           	ldr	r2,[r1,#&60]
EA22 0208     	bic.w	r2,r2,r8
660A           	str	r2,[r1,#&60]
6863           	ldr	r3,[r4,#&4]
F883 0064     	strb	r0,[r3,#&64]
B9DF           	cbnz	r7,$00000C3C

l00000C04:
F007 FCD4     	bl	$000085B0
F007 FCB6     	bl	$00008578
B115           	cbz	r5,$00000C14

l00000C0E:
6863           	ldr	r3,[r4,#&4]
6E1B           	ldr	r3,[r3,#&60]
602B           	str	r3,[r5]

l00000C14:
6863           	ldr	r3,[r4,#&4]
F893 3064     	ldrb	r3,[r3,#&64]
2B01           	cmps	r3,#1
D01A           	beq	$00000C54

l00000C1E:
2501           	mov	r5,#1
6863           	ldr	r3,[r4,#&4]
6E19           	ldr	r1,[r3,#&60]
EA21 0106     	bic.w	r1,r1,r6
6619           	str	r1,[r3,#&60]

l00000C2A:
2200           	mov	r2,#0
6863           	ldr	r3,[r4,#&4]
F883 2064     	strb	r2,[r3,#&64]
F007 FCBD     	bl	$000085B0
4628           	mov	r0,r5
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000C3C:
4638           	mov	r0,r7
F7FF FE0D     	bl	$0000085C
F04F 5280     	mov	r2,#&10000000
4B05           	ldr	r3,[00000C5C]                           ; [pc,#&14]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7D7           	b	$00000C04

l00000C54:
2500           	mov	r5,#0
E7E8           	b	$00000C2A
00000C58                         C4 00 00 20 04 ED 00 E0         ... ....

;; vTaskNotifyGiveFromISR: 00000C60
vTaskNotifyGiveFromISR proc
E92D 43F8     	push.w	{r3-r9,lr}
F3EF 8611     	mrs	r6,cpsr
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
2302           	mov	r3,#2
F890 5064     	ldrb	r5,[r0,#&64]
F880 3064     	strb	r3,[r0,#&64]
6E03           	ldr	r3,[r0,#&60]
B2ED           	uxtb	r5,r5
3301           	adds	r3,#1
2D01           	cmps	r5,#1
6603           	str	r3,[r0,#&60]
D003           	beq	$00000C96

l00000C8E:
F386 8811     	msr	cpsr,r6
E8BD 83F8     	pop.w	{r3-r9,pc}

l00000C96:
4F19           	ldr	r7,[00000CFC]                           ; [pc,#&64]
4688           	mov	r8,r1
F8D7 308C     	ldr	r3,[r7,#&8C]
4604           	mov	r4,r0
B1A3           	cbz	r3,$00000CCC

l00000CA2:
F100 0138     	add	r1,r0,#&38
F107 0058     	add	r0,r7,#&58
F007 FB21     	bl	$000082F0

l00000CAE:
687B           	ldr	r3,[r7,#&4]
6CE2           	ldr	r2,[r4,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
D9EA           	bls	$00000C8E

l00000CB8:
2301           	mov	r3,#1
F1B8 0F00     	cmp	r8,#0
D019           	beq	$00000CF4

l00000CC0:
F8C8 3000     	str	r3,[r8]
F386 8811     	msr	cpsr,r6
E8BD 83F8     	pop.w	{r3-r9,pc}

l00000CCC:
F100 0924     	add	r9,r0,#&24
4648           	mov	r0,r9
F007 FB35     	bl	$00008340
6CE0           	ldr	r0,[r4,#&4C]
6FFA           	ldr	r2,[r7,#&7C]
4085           	lsls	r5,r0
F107 0308     	add	r3,r7,#8
EB00 0080     	add.w	r0,r0,r0,lsl #2
4315           	orrs	r5,r2
4649           	mov	r1,r9
EB03 0080     	add.w	r0,r3,r0,lsl #2
67FD           	str	r5,[r7,#&7C]
F007 FAFF     	bl	$000082F0
E7DC           	b	$00000CAE

l00000CF4:
F8C7 3090     	str	r3,[r7,#&90]
E7C9           	b	$00000C8E
00000CFA                               00 BF C4 00 00 20           ..... 

;; ulTaskNotifyTake: 00000D00
;;   Called from:
;;     00008A44 (in MPU_ulTaskNotifyTake)
ulTaskNotifyTake proc
B570           	push	{r4-r6,lr}
4C18           	ldr	r4,[00000D64]                           ; [pc,#&60]
4606           	mov	r6,r0
460D           	mov	r5,r1
F007 FC36     	bl	$00008578
6863           	ldr	r3,[r4,#&4]
6E1B           	ldr	r3,[r3,#&60]
B923           	cbnz	r3,$00000D1C

l00000D12:
2201           	mov	r2,#1
6863           	ldr	r3,[r4,#&4]
F883 2064     	strb	r2,[r3,#&64]
B9B5           	cbnz	r5,$00000D4A

l00000D1C:
F007 FC48     	bl	$000085B0
F007 FC2A     	bl	$00008578
6863           	ldr	r3,[r4,#&4]
6E1D           	ldr	r5,[r3,#&60]
B11D           	cbz	r5,$00000D32

l00000D2A:
B956           	cbnz	r6,$00000D42

l00000D2C:
6863           	ldr	r3,[r4,#&4]
1E6A           	sub	r2,r5,#1
661A           	str	r2,[r3,#&60]

l00000D32:
2200           	mov	r2,#0
6863           	ldr	r3,[r4,#&4]
F883 2064     	strb	r2,[r3,#&64]
F007 FC39     	bl	$000085B0
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}

l00000D42:
2200           	mov	r2,#0
6863           	ldr	r3,[r4,#&4]
661A           	str	r2,[r3,#&60]
E7F3           	b	$00000D32

l00000D4A:
4628           	mov	r0,r5
F7FF FD86     	bl	$0000085C
F04F 5280     	mov	r2,#&10000000
4B04           	ldr	r3,[00000D68]                           ; [pc,#&10]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7DC           	b	$00000D1C
00000D62       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; xTaskIncrementTick: 00000D6C
;;   Called from:
;;     00000EF2 (in xTaskResumeAll)
;;     000016FA (in xPortSysTickHandler)
xTaskIncrementTick proc
E92D 47F0     	push.w	{r4-r10,lr}
4C3C           	ldr	r4,[00000E64]                           ; [pc,#&F0]
F8D4 308C     	ldr	r3,[r4,#&8C]
2B00           	cmps	r3,#0
D15E           	bne	$00000E38

l00000D7A:
F8D4 7080     	ldr	r7,[r4,#&80]
3701           	adds	r7,#1
F8C4 7080     	str	r7,[r4,#&80]
B987           	cbnz	r7,$00000DA8

l00000D86:
6EE3           	ldr	r3,[r4,#&6C]
6F22           	ldr	r2,[r4,#&70]
66E2           	str	r2,[r4,#&6C]
6723           	str	r3,[r4,#&70]
F8D4 3094     	ldr	r3,[r4,#&94]
3301           	adds	r3,#1
F8C4 3094     	str	r3,[r4,#&94]
6EE3           	ldr	r3,[r4,#&6C]
681B           	ldr	r3,[r3]
2B00           	cmps	r3,#0
D152           	bne	$00000E46

l00000DA0:
F04F 33FF     	mov	r3,#&FFFFFFFF
F8C4 3084     	str	r3,[r4,#&84]

l00000DA8:
F8D4 3084     	ldr	r3,[r4,#&84]
2600           	mov	r6,#0
429F           	cmps	r7,r3
D330           	blo	$00000E14

l00000DB2:
F04F 0901     	mov	r9,#1
F8DF 80B0     	ldr	r8,[00000E68]                            ; [pc,#&B0]
E023           	b	$00000E04

l00000DBC:
6EE3           	ldr	r3,[r4,#&6C]
68DB           	ldr	r3,[r3,#&C]
68DD           	ldr	r5,[r3,#&C]
6A6B           	ldr	r3,[r5,#&24]
F105 0A24     	add	r10,r5,#&24
429F           	cmps	r7,r3
D348           	blo	$00000E5E

l00000DCC:
4650           	mov	r0,r10
F007 FAB7     	bl	$00008340
6CAB           	ldr	r3,[r5,#&48]
F105 0038     	add	r0,r5,#&38
B10B           	cbz	r3,$00000DDE

l00000DDA:
F007 FAB1     	bl	$00008340

l00000DDE:
6CE8           	ldr	r0,[r5,#&4C]
6FE2           	ldr	r2,[r4,#&7C]
FA09 F300     	lsl	r3,r9,r0
EB00 0080     	add.w	r0,r0,r0,lsl #2
4313           	orrs	r3,r2
4651           	mov	r1,r10
EB08 0080     	add.w	r0,r8,r0,lsl #2
67E3           	str	r3,[r4,#&7C]
F007 FA7C     	bl	$000082F0
6863           	ldr	r3,[r4,#&4]
6CEA           	ldr	r2,[r5,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
BF28           	it	hs
2601           	movhs	r6,#1

l00000E04:
6EE3           	ldr	r3,[r4,#&6C]
681B           	ldr	r3,[r3]
2B00           	cmps	r3,#0
D1D7           	bne	$00000DBC

l00000E0C:
F04F 33FF     	mov	r3,#&FFFFFFFF
F8C4 3084     	str	r3,[r4,#&84]

l00000E14:
6863           	ldr	r3,[r4,#&4]
6CDB           	ldr	r3,[r3,#&4C]
EB03 0383     	add.w	r3,r3,r3,lsl #2
EB04 0383     	add.w	r3,r4,r3,lsl #2
689B           	ldr	r3,[r3,#&8]
2B02           	cmps	r3,#2
BF28           	it	hs
2601           	movhs	r6,#1

l00000E28:
F8D4 3090     	ldr	r3,[r4,#&90]
2B00           	cmps	r3,#0
BF18           	it	ne
2601           	movne	r6,#1

l00000E32:
4630           	mov	r0,r6
E8BD 87F0     	pop.w	{r4-r10,pc}

l00000E38:
F8D4 3098     	ldr	r3,[r4,#&98]
2600           	mov	r6,#0
3301           	adds	r3,#1
F8C4 3098     	str	r3,[r4,#&98]
E7F0           	b	$00000E28

l00000E46:
6EE3           	ldr	r3,[r4,#&6C]
2600           	mov	r6,#0
68DB           	ldr	r3,[r3,#&C]
68DB           	ldr	r3,[r3,#&C]
6A5B           	ldr	r3,[r3,#&24]
F8C4 3084     	str	r3,[r4,#&84]
F8D4 3084     	ldr	r3,[r4,#&84]
429F           	cmps	r7,r3
D3DB           	blo	$00000E14

l00000E5C:
E7A9           	b	$00000DB2

l00000E5E:
F8C4 3084     	str	r3,[r4,#&84]
E7D7           	b	$00000E14
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
E92D 41F0     	push.w	{r4-r8,lr}
4C33           	ldr	r4,[00000F40]                           ; [pc,#&CC]
F007 FB81     	bl	$00008578
F8D4 308C     	ldr	r3,[r4,#&8C]
3B01           	subs	r3,#1
F8C4 308C     	str	r3,[r4,#&8C]
F8D4 508C     	ldr	r5,[r4,#&8C]
2D00           	cmps	r5,#0
D14E           	bne	$00000F26

l00000E88:
6823           	ldr	r3,[r4]
2B00           	cmps	r3,#0
D04B           	beq	$00000F26

l00000E8E:
2601           	mov	r6,#1
F104 0708     	add	r7,r4,#8
E01E           	b	$00000ED4

l00000E96:
6E63           	ldr	r3,[r4,#&64]
68DD           	ldr	r5,[r3,#&C]
F105 0824     	add	r8,r5,#&24
F105 0038     	add	r0,r5,#&38
F007 FA4D     	bl	$00008340
4640           	mov	r0,r8
F007 FA4A     	bl	$00008340
6CE8           	ldr	r0,[r5,#&4C]
6FE2           	ldr	r2,[r4,#&7C]
FA06 F300     	lsl	r3,r6,r0
EB00 0080     	add.w	r0,r0,r0,lsl #2
4313           	orrs	r3,r2
4641           	mov	r1,r8
EB07 0080     	add.w	r0,r7,r0,lsl #2
67E3           	str	r3,[r4,#&7C]
F007 FA15     	bl	$000082F0
6863           	ldr	r3,[r4,#&4]
6CEA           	ldr	r2,[r5,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
BF28           	it	hs
F8C4 6090     	strhs	r6,[r4,#&90]

l00000ED4:
6DA3           	ldr	r3,[r4,#&58]
2B00           	cmps	r3,#0
D1DD           	bne	$00000E96

l00000EDA:
B135           	cbz	r5,$00000EEA

l00000EDC:
6EE3           	ldr	r3,[r4,#&6C]
681B           	ldr	r3,[r3]
BB3B           	cbnz	r3,$00000F32

l00000EE2:
F04F 33FF     	mov	r3,#&FFFFFFFF
F8C4 3084     	str	r3,[r4,#&84]

l00000EEA:
F8D4 5098     	ldr	r5,[r4,#&98]
B14D           	cbz	r5,$00000F04

l00000EF0:
2601           	mov	r6,#1

l00000EF2:
F7FF FF3B     	bl	$00000D6C
B108           	cbz	r0,$00000EFC

l00000EF8:
F8C4 6090     	str	r6,[r4,#&90]

l00000EFC:
3D01           	subs	r5,#1
D1F8           	bne	$00000EF2

l00000F00:
F8C4 5098     	str	r5,[r4,#&98]

l00000F04:
F8D4 3090     	ldr	r3,[r4,#&90]
B16B           	cbz	r3,$00000F26

l00000F0A:
F04F 5280     	mov	r2,#&10000000
4B0D           	ldr	r3,[00000F44]                           ; [pc,#&34]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
2401           	mov	r4,#1
F007 FB48     	bl	$000085B0
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000F26:
2400           	mov	r4,#0
F007 FB42     	bl	$000085B0
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000F32:
6EE3           	ldr	r3,[r4,#&6C]
68DB           	ldr	r3,[r3,#&C]
68DB           	ldr	r3,[r3,#&C]
6A5B           	ldr	r3,[r3,#&24]
F8C4 3084     	str	r3,[r4,#&84]
E7D4           	b	$00000EEA
00000F40 C4 00 00 20 04 ED 00 E0                         ... ....       

;; vTaskDelay: 00000F48
;;   Called from:
;;     000088A8 (in MPU_vTaskDelay)
vTaskDelay proc
B508           	push	{r3,lr}
B940           	cbnz	r0,$00000F5E

l00000F4C:
F04F 5280     	mov	r2,#&10000000
4B09           	ldr	r3,[00000F78]                           ; [pc,#&24]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
BD08           	pop	{r3,pc}

l00000F5E:
4A07           	ldr	r2,[00000F7C]                           ; [pc,#&1C]
F8D2 308C     	ldr	r3,[r2,#&8C]
3301           	adds	r3,#1
F8C2 308C     	str	r3,[r2,#&8C]
F7FF FC77     	bl	$0000085C
F7FF FF7D     	bl	$00000E6C
2800           	cmps	r0,#0
D0EA           	beq	$00000F4C

l00000F76:
BD08           	pop	{r3,pc}
00000F78                         04 ED 00 E0 C4 00 00 20         ....... 

;; vTaskDelayUntil: 00000F80
;;   Called from:
;;     00008884 (in MPU_vTaskDelayUntil)
vTaskDelayUntil proc
4A14           	ldr	r2,[00000FD4]                           ; [pc,#&50]
B510           	push	{r4,lr}
F8D2 408C     	ldr	r4,[r2,#&8C]
6803           	ldr	r3,[r0]
3401           	adds	r4,#1
F8C2 408C     	str	r4,[r2,#&8C]
F8D2 2080     	ldr	r2,[r2,#&80]
4419           	adds	r1,r3
429A           	cmps	r2,r3
D20E           	bhs	$00000FB8

l00000F9A:
428B           	cmps	r3,r1
D80E           	bhi	$00000FBC

l00000F9E:
6001           	str	r1,[r0]
F7FF FF64     	bl	$00000E6C
B9A0           	cbnz	r0,$00000FD0

l00000FA6:
F04F 5280     	mov	r2,#&10000000
4B0B           	ldr	r3,[00000FD8]                           ; [pc,#&2C]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
BD10           	pop	{r4,pc}

l00000FB8:
428B           	cmps	r3,r1
D801           	bhi	$00000FC0

l00000FBC:
428A           	cmps	r2,r1
D2EE           	bhs	$00000F9E

l00000FC0:
6001           	str	r1,[r0]
1A88           	sub	r0,r1,r2
F7FF FC4A     	bl	$0000085C
F7FF FF50     	bl	$00000E6C
2800           	cmps	r0,#0
D0EA           	beq	$00000FA6

l00000FD0:
BD10           	pop	{r4,pc}
00000FD2       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; vTaskPlaceOnEventList: 00000FDC
;;   Called from:
;;     00000222 (in xQueueGenericSend)
;;     00000378 (in xQueueGenericReceive)
vTaskPlaceOnEventList proc
B510           	push	{r4,lr}
460C           	mov	r4,r1
4B04           	ldr	r3,[00000FF4]                           ; [pc,#&10]
6859           	ldr	r1,[r3,#&4]
3138           	adds	r1,#&38
F007 F991     	bl	$0000830C
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
E434           	b	$0000085C
00000FF2       00 BF C4 00 00 20                           .....        

;; vTaskPlaceOnUnorderedEventList: 00000FF8
;;   Called from:
;;     00001812 (in xEventGroupWaitBits)
;;     00001930 (in xEventGroupSync)
vTaskPlaceOnUnorderedEventList proc
B538           	push	{r3-r5,lr}
4614           	mov	r4,r2
4B06           	ldr	r3,[00001018]                           ; [pc,#&18]
F041 4100     	orr	r1,r1,#&80000000
685D           	ldr	r5,[r3,#&4]
685B           	ldr	r3,[r3,#&4]
63A9           	str	r1,[r5,#&38]
F103 0138     	add	r1,r3,#&38
F007 F970     	bl	$000082F0
4620           	mov	r0,r4
E8BD 4038     	pop.w	{r3-r5,lr}
E421           	b	$0000085C
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
B5F8           	push	{r3-r7,lr}
68C3           	ldr	r3,[r0,#&C]
4C16           	ldr	r4,[0000107C]                           ; [pc,#&58]
68DD           	ldr	r5,[r3,#&C]
F105 0638     	add	r6,r5,#&38
4630           	mov	r0,r6
F007 F989     	bl	$00008340
F8D4 308C     	ldr	r3,[r4,#&8C]
B9EB           	cbnz	r3,$00001070

l00001034:
F105 0624     	add	r6,r5,#&24
4630           	mov	r0,r6
F007 F981     	bl	$00008340
2301           	mov	r3,#1
6CE8           	ldr	r0,[r5,#&4C]
6FE7           	ldr	r7,[r4,#&7C]
4083           	lsls	r3,r0
F104 0208     	add	r2,r4,#8
EB00 0080     	add.w	r0,r0,r0,lsl #2
433B           	orrs	r3,r7
4631           	mov	r1,r6
EB02 0080     	add.w	r0,r2,r0,lsl #2
67E3           	str	r3,[r4,#&7C]
F007 F94A     	bl	$000082F0

l0000105C:
6863           	ldr	r3,[r4,#&4]
6CEA           	ldr	r2,[r5,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
BF86           	itte	hi
2001           	movhi	r0,#1

l00001068:
F8C4 0090     	str	r0,[r4,#&90]
2000           	mov	r0,#0
BDF8           	pop	{r3-r7,pc}

l00001070:
4631           	mov	r1,r6
F104 0058     	add	r0,r4,#&58
F007 F93B     	bl	$000082F0
E7EF           	b	$0000105C
0000107C                                     C4 00 00 20             ... 

;; xTaskRemoveFromUnorderedEventList: 00001080
;;   Called from:
;;     000018BC (in xEventGroupSetBits)
;;     000019B6 (in vEventGroupDelete)
xTaskRemoveFromUnorderedEventList proc
B5F8           	push	{r3-r7,lr}
2501           	mov	r5,#1
68C6           	ldr	r6,[r0,#&C]
F041 4100     	orr	r1,r1,#&80000000
6001           	str	r1,[r0]
F106 0724     	add	r7,r6,#&24
F007 F956     	bl	$00008340
4C0F           	ldr	r4,[000010D4]                           ; [pc,#&3C]
4638           	mov	r0,r7
F007 F952     	bl	$00008340
6CF3           	ldr	r3,[r6,#&4C]
F8D4 E07C     	ldr	lr,[r4,#&7C]
FA05 F203     	lsl	r2,r5,r3
F104 0008     	add	r0,r4,#8
EB03 0383     	add.w	r3,r3,r3,lsl #2
EB00 0083     	add.w	r0,r0,r3,lsl #2
EA42 020E     	orr	r2,r2,lr
4639           	mov	r1,r7
67E2           	str	r2,[r4,#&7C]
F007 F919     	bl	$000082F0
6863           	ldr	r3,[r4,#&4]
6CF2           	ldr	r2,[r6,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
BF86           	itte	hi
4628           	movhi	r0,r5

l000010CA:
F8C4 5090     	str	r5,[r4,#&90]
2000           	mov	r0,#0
BDF8           	pop	{r3-r7,pc}
000010D2       00 BF C4 00 00 20                           .....        

;; vTaskSwitchContext: 000010D8
;;   Called from:
;;     000016A6 (in xPortPendSVHandler)
vTaskSwitchContext proc
4A10           	ldr	r2,[0000111C]                           ; [pc,#&40]
F8D2 308C     	ldr	r3,[r2,#&8C]
B9C3           	cbnz	r3,$00001112

l000010E0:
F8C2 3090     	str	r3,[r2,#&90]
6FD3           	ldr	r3,[r2,#&7C]
FAB3 F383     	clz	r3,r3
B2DB           	uxtb	r3,r3
F1C3 031F     	rsb	r3,r3,#&1F
EB03 0383     	add.w	r3,r3,r3,lsl #2
009B           	lsls	r3,r3,#2
18D0           	add	r0,r2,r3
4603           	mov	r3,r0
68C1           	ldr	r1,[r0,#&C]
3310           	adds	r3,#&10
6849           	ldr	r1,[r1,#&4]
4299           	cmps	r1,r3
60C1           	str	r1,[r0,#&C]
BF08           	it	eq
6849           	ldreq	r1,[r1,#&4]

l00001108:
68CB           	ldr	r3,[r1,#&C]
BF08           	it	eq
60C1           	streq	r1,[r0,#&C]

l0000110E:
6053           	str	r3,[r2,#&4]
4770           	bx	lr

l00001112:
2301           	mov	r3,#1
F8C2 3090     	str	r3,[r2,#&90]
4770           	bx	lr
0000111A                               00 BF C4 00 00 20           ..... 

;; uxTaskResetEventItemValue: 00001120
;;   Called from:
;;     0000182C (in xEventGroupWaitBits)
;;     0000194A (in xEventGroupSync)
uxTaskResetEventItemValue proc
4B04           	ldr	r3,[00001134]                           ; [pc,#&10]
6859           	ldr	r1,[r3,#&4]
685A           	ldr	r2,[r3,#&4]
685B           	ldr	r3,[r3,#&4]
6B88           	ldr	r0,[r1,#&38]
6CDB           	ldr	r3,[r3,#&4C]
F1C3 0302     	rsb	r3,r3,#2
6393           	str	r3,[r2,#&38]
4770           	bx	lr
00001134             C4 00 00 20                             ...        

;; xTaskGetCurrentTaskHandle: 00001138
;;   Called from:
;;     000005DC (in xQueueTakeMutexRecursive)
;;     0000060A (in xQueueGiveMutexRecursive)
xTaskGetCurrentTaskHandle proc
4B01           	ldr	r3,[00001140]                           ; [pc,#&4]
6858           	ldr	r0,[r3,#&4]
4770           	bx	lr
0000113E                                           00 BF               ..
00001140 C4 00 00 20                                     ...            

;; vTaskSetTimeOutState: 00001144
;;   Called from:
;;     00000212 (in xQueueGenericSend)
;;     0000039E (in xQueueGenericReceive)
;;     00008980 (in MPU_vTaskSetTimeOutState)
vTaskSetTimeOutState proc
4B03           	ldr	r3,[00001154]                           ; [pc,#&C]
F8D3 2094     	ldr	r2,[r3,#&94]
F8D3 3080     	ldr	r3,[r3,#&80]
E880 000C     	stm	r0,{r2-r3}
4770           	bx	lr
00001154             C4 00 00 20                             ...        

;; xTaskCheckForTimeOut: 00001158
;;   Called from:
;;     000001D4 (in xQueueGenericSend)
;;     0000034A (in xQueueGenericReceive)
;;     000089A8 (in MPU_xTaskCheckForTimeOut)
xTaskCheckForTimeOut proc
B570           	push	{r4-r6,lr}
4604           	mov	r4,r0
460E           	mov	r6,r1
F007 FA0B     	bl	$00008578
4B11           	ldr	r3,[000011A8]                           ; [pc,#&44]
6821           	ldr	r1,[r4]
F8D3 5080     	ldr	r5,[r3,#&80]
F8D3 2094     	ldr	r2,[r3,#&94]
6860           	ldr	r0,[r4,#&4]
4291           	cmps	r1,r2
D001           	beq	$00001178

l00001174:
4285           	cmps	r5,r0
D211           	bhs	$0000119C

l00001178:
6832           	ldr	r2,[r6]
1A29           	sub	r1,r5,r0
4291           	cmps	r1,r2
D20D           	bhs	$0000119C

l00001180:
1B52           	sub	r2,r2,r5
2500           	mov	r5,#0
F8D3 1094     	ldr	r1,[r3,#&94]
F8D3 3080     	ldr	r3,[r3,#&80]
4402           	adds	r2,r0
6032           	str	r2,[r6]
E884 000A     	stm	r4,{r1,r3}
F007 FA0C     	bl	$000085B0
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}

l0000119C:
2501           	mov	r5,#1
F007 FA07     	bl	$000085B0
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}
000011A6                   00 BF C4 00 00 20                   .....    

;; vTaskMissedYield: 000011AC
;;   Called from:
;;     0000008C (in prvUnlockQueue)
;;     000000D2 (in prvUnlockQueue)
vTaskMissedYield proc
2201           	mov	r2,#1
4B02           	ldr	r3,[000011B8]                           ; [pc,#&8]
F8C3 2090     	str	r2,[r3,#&90]
4770           	bx	lr
000011B6                   00 BF C4 00 00 20                   .....    

;; vTaskPriorityInherit: 000011BC
;;   Called from:
;;     000003DE (in xQueueGenericReceive)
vTaskPriorityInherit proc
2800           	cmps	r0,#0
D042           	beq	$00001246

l000011C0:
B5F8           	push	{r3-r7,lr}
4C21           	ldr	r4,[00001248]                           ; [pc,#&84]
6CC3           	ldr	r3,[r0,#&4C]
6862           	ldr	r2,[r4,#&4]
6CD2           	ldr	r2,[r2,#&4C]
4293           	cmps	r3,r2
D212           	bhs	$000011F4

l000011CE:
6B82           	ldr	r2,[r0,#&38]
2A00           	cmps	r2,#0
DB04           	blt	$000011DE

l000011D4:
6862           	ldr	r2,[r4,#&4]
6CD2           	ldr	r2,[r2,#&4C]
F1C2 0202     	rsb	r2,r2,#2
6382           	str	r2,[r0,#&38]

l000011DE:
4D1B           	ldr	r5,[0000124C]                           ; [pc,#&6C]
EB03 0383     	add.w	r3,r3,r3,lsl #2
6B42           	ldr	r2,[r0,#&34]
EB05 0383     	add.w	r3,r5,r3,lsl #2
429A           	cmps	r2,r3
D003           	beq	$000011F6

l000011EE:
6863           	ldr	r3,[r4,#&4]
6CDB           	ldr	r3,[r3,#&4C]
64C3           	str	r3,[r0,#&4C]

l000011F4:
BDF8           	pop	{r3-r7,pc}

l000011F6:
F100 0724     	add	r7,r0,#&24
4606           	mov	r6,r0
4638           	mov	r0,r7
F007 F89F     	bl	$00008340
B968           	cbnz	r0,$00001220

l00001204:
6CF2           	ldr	r2,[r6,#&4C]
EB02 0382     	add.w	r3,r2,r2,lsl #2
EB04 0383     	add.w	r3,r4,r3,lsl #2
689B           	ldr	r3,[r3,#&8]
B933           	cbnz	r3,$00001220

l00001212:
2101           	mov	r1,#1
6FE3           	ldr	r3,[r4,#&7C]
FA01 F202     	lsl	r2,r1,r2
EA23 0202     	bic.w	r2,r3,r2
67E2           	str	r2,[r4,#&7C]

l00001220:
2301           	mov	r3,#1
6862           	ldr	r2,[r4,#&4]
F8D4 E07C     	ldr	lr,[r4,#&7C]
6CD2           	ldr	r2,[r2,#&4C]
4639           	mov	r1,r7
4093           	lsls	r3,r2
EA43 030E     	orr	r3,r3,lr
EB02 0082     	add.w	r0,r2,r2,lsl #2
64F2           	str	r2,[r6,#&4C]
EB05 0080     	add.w	r0,r5,r0,lsl #2
67E3           	str	r3,[r4,#&7C]
E8BD 40F8     	pop.w	{r3-r7,lr}
F007 B855     	b	$000082F0

l00001246:
4770           	bx	lr
00001248                         C4 00 00 20 CC 00 00 20         ... ... 

;; xTaskPriorityDisinherit: 00001250
;;   Called from:
;;     00000162 (in prvCopyDataToQueue)
xTaskPriorityDisinherit proc
2800           	cmps	r0,#0
D039           	beq	$000012C8

l00001254:
B5F8           	push	{r3-r7,lr}
6CC1           	ldr	r1,[r0,#&4C]
6DC3           	ldr	r3,[r0,#&5C]
6D82           	ldr	r2,[r0,#&58]
3B01           	subs	r3,#1
4291           	cmps	r1,r2
65C3           	str	r3,[r0,#&5C]
D000           	beq	$00001266

l00001264:
B10B           	cbz	r3,$0000126A

l00001266:
2000           	mov	r0,#0
BDF8           	pop	{r3-r7,pc}

l0000126A:
F100 0724     	add	r7,r0,#&24
4604           	mov	r4,r0
4638           	mov	r0,r7
F007 F865     	bl	$00008340
B978           	cbnz	r0,$00001298

l00001278:
6CE1           	ldr	r1,[r4,#&4C]
4A14           	ldr	r2,[000012CC]                           ; [pc,#&50]
EB01 0381     	add.w	r3,r1,r1,lsl #2
EB02 0383     	add.w	r3,r2,r3,lsl #2
689B           	ldr	r3,[r3,#&8]
B943           	cbnz	r3,$0000129A

l00001288:
2001           	mov	r0,#1
6FD3           	ldr	r3,[r2,#&7C]
FA00 F101     	lsl	r1,r0,r1
EA23 0101     	bic.w	r1,r3,r1
67D1           	str	r1,[r2,#&7C]
E000           	b	$0000129A

l00001298:
4A0C           	ldr	r2,[000012CC]                           ; [pc,#&30]

l0000129A:
2501           	mov	r5,#1
6DA3           	ldr	r3,[r4,#&58]
F8D2 E07C     	ldr	lr,[r2,#&7C]
480B           	ldr	r0,[000012D0]                           ; [pc,#&2C]
FA05 F603     	lsl	r6,r5,r3
4639           	mov	r1,r7
64E3           	str	r3,[r4,#&4C]
F1C3 0702     	rsb	r7,r3,#2
EB03 0383     	add.w	r3,r3,r3,lsl #2
EA46 060E     	orr	r6,r6,lr
EB00 0083     	add.w	r0,r0,r3,lsl #2
63A7           	str	r7,[r4,#&38]
67D6           	str	r6,[r2,#&7C]
F007 F816     	bl	$000082F0
4628           	mov	r0,r5
BDF8           	pop	{r3-r7,pc}

l000012C8:
2000           	mov	r0,#0
4770           	bx	lr
000012CC                                     C4 00 00 20             ... 
000012D0 CC 00 00 20                                     ...            

;; pvTaskIncrementMutexHeldCount: 000012D4
;;   Called from:
;;     0000041C (in xQueueGenericReceive)
pvTaskIncrementMutexHeldCount proc
4B04           	ldr	r3,[000012E8]                           ; [pc,#&10]
685A           	ldr	r2,[r3,#&4]
B11A           	cbz	r2,$000012E2

l000012DA:
6859           	ldr	r1,[r3,#&4]
6DCA           	ldr	r2,[r1,#&5C]
3201           	adds	r2,#1
65CA           	str	r2,[r1,#&5C]

l000012E2:
6858           	ldr	r0,[r3,#&4]
4770           	bx	lr
000012E6                   00 BF C4 00 00 20 00 00 00 00       ..... ....

;; prvRestoreContextOfFirstTask: 000012F0
;;   Called from:
;;     0000135E (in prvSVCHandler)
prvRestoreContextOfFirstTask proc
F8DF 0430     	ldr	r0,[00001724]                            ; [pc,#&430]
6800           	ldr	r0,[r0]
6800           	ldr	r0,[r0]
F380 8808     	msr	cpsr,r0
4B0C           	ldr	r3,[00001330]                           ; [pc,#&30]
6819           	ldr	r1,[r3]
6808           	ldr	r0,[r1]
F101 0104     	add	r1,r1,#4
F8DF 2420     	ldr	r2,[00001728]                            ; [pc,#&420]
E8B1 0FF0     	ldm	r1!,{r4-fp}
E8A2 0FF0     	stm	r2!,{r4-fp}
E8B0 0FF8     	ldm	r0!,{r3-fp}
F383 8814     	msr	cpsr,r3
F380 8809     	msr	cpsr,r0
F04F 0000     	mov	r0,#0
F380 8811     	msr	cpsr,r0
F06F 0E02     	mvn	lr,#2
4770           	bx	lr
0000132C                                     AF F3 00 80             ....
00001330 C8 00 00 20                                     ...            

;; prvSVCHandler: 00001334
;;   Called from:
;;     00001722 (in vPortSVCHandler)
prvSVCHandler proc
6983           	ldr	r3,[r0,#&18]
F813 3C02     	ldrb.w	r3,[r3,-#&2]
2B01           	cmps	r3,#1
D010           	beq	$00001360

l0000133E:
D309           	blo	$00001354

l00001340:
2B02           	cmps	r3,#2
D106           	bne	$00001352

l00001344:
F3EF 8114     	mrs	r1,cpsr
F021 0101     	bic	r1,r1,#1
F381 8814     	msr	cpsr,r1
4770           	bx	lr

l00001352:
4770           	bx	lr

l00001354:
4A07           	ldr	r2,[00001374]                           ; [pc,#&1C]
6813           	ldr	r3,[r2]
F043 433E     	orr	r3,r3,#&BE000000
6013           	str	r3,[r2]
E7C7           	b	$000012F0

l00001360:
F04F 5280     	mov	r2,#&10000000
4B04           	ldr	r3,[00001378]                           ; [pc,#&10]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
4770           	bx	lr
00001372       00 BF 1C ED 00 E0 04 ED 00 E0               ..........   

;; pxPortInitialiseStack: 0000137C
;;   Called from:
;;     00000786 (in prvInitialiseNewTask)
pxPortInitialiseStack proc
2B01           	cmps	r3,#1
B430           	push	{r4-r5}
BF08           	it	eq
2302           	moveq	r3,#2

l00001384:
F04F 7580     	mov	r5,#&1000000
F04F 0400     	mov	r4,#0
BF18           	it	ne
2303           	movne	r3,#3

l00001390:
F840 2C20     	str.w	r2,[r0,-#&20]
F021 0101     	bic	r1,r1,#1
F1A0 0244     	sub	r2,r0,#&44
E900 0022     	stmdb	r0,{r1,r5}
F840 4C0C     	str.w	r4,[r0,-#&C]
F840 3C44     	str.w	r3,[r0,-#&44]
BC30           	pop	{r4-r5}
4610           	mov	r0,r2
4770           	bx	lr
000013AE                                           00 BF               ..

;; xPortStartScheduler: 000013B0
;;   Called from:
;;     000009DC (in vTaskStartScheduler)
xPortStartScheduler proc
4B4D           	ldr	r3,[000014E8]                           ; [pc,#&134]
B470           	push	{r4-r6}
681A           	ldr	r2,[r3]
494D           	ldr	r1,[000014EC]                           ; [pc,#&134]
F442 027F     	orr	r2,r2,#&FF0000
601A           	str	r2,[r3]
681A           	ldr	r2,[r3]
F042 427F     	orr	r2,r2,#&FF000000
601A           	str	r2,[r3]
680B           	ldr	r3,[r1]
F5B3 6F00     	cmp	r3,#&800
D018           	beq	$00001400

l000013CE:
F644 651F     	mov	r5,#&4E1F
2107           	mov	r1,#7
2000           	mov	r0,#0
4C46           	ldr	r4,[000014F0]                           ; [pc,#&118]
4A46           	ldr	r2,[000014F4]                           ; [pc,#&118]
4B47           	ldr	r3,[000014F8]                           ; [pc,#&11C]
6025           	str	r5,[r4]
6011           	str	r1,[r2]
6018           	str	r0,[r3]
48D0           	ldr	r0,[00001724]                           ; [pc,#&340]
6800           	ldr	r0,[r0]
6800           	ldr	r0,[r0]
F380 8808     	msr	cpsr,r0
B662           	cps	#0
B661           	cps	#0
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
DF00           	svc	#0
BF00           	nop
BC70           	pop	{r4-r6}
4770           	bx	lr

l00001400:
483E           	ldr	r0,[000014FC]                           ; [pc,#&F8]
493F           	ldr	r1,[00001500]                           ; [pc,#&FC]
4B3F           	ldr	r3,[00001504]                           ; [pc,#&FC]
1A09           	sub	r1,r1,r0
F040 0210     	orr	r2,r0,#&10
2920           	cmps	r1,#&20
601A           	str	r2,[r3]
D965           	bls	$000014DE

l00001412:
2340           	mov	r3,#&40
2205           	mov	r2,#5
E002           	b	$0000141E

l00001418:
3201           	adds	r2,#1
2A1F           	cmps	r2,#&1F
D057           	beq	$000014CE

l0000141E:
4299           	cmps	r1,r3
EA4F 0343     	mov.w	r3,r3,lsl #1
D8F8           	bhi	$00001418

l00001426:
4B38           	ldr	r3,[00001508]                           ; [pc,#&E0]
EA43 0242     	orr	r2,r3,r2,lsl #1

l0000142C:
4937           	ldr	r1,[0000150C]                           ; [pc,#&DC]
4C38           	ldr	r4,[00001510]                           ; [pc,#&E0]
1A09           	sub	r1,r1,r0
4B34           	ldr	r3,[00001504]                           ; [pc,#&D0]
F040 0011     	orr	r0,r0,#&11
2920           	cmps	r1,#&20
6022           	str	r2,[r4]
6018           	str	r0,[r3]
D94C           	bls	$000014DA

l00001440:
2340           	mov	r3,#&40
2205           	mov	r2,#5
E002           	b	$0000144C

l00001446:
3201           	adds	r2,#1
2A1F           	cmps	r2,#&1F
D042           	beq	$000014D2

l0000144C:
4299           	cmps	r1,r3
EA4F 0343     	mov.w	r3,r3,lsl #1
D8F8           	bhi	$00001446

l00001454:
4B2F           	ldr	r3,[00001514]                           ; [pc,#&BC]
EA43 0242     	orr	r2,r3,r2,lsl #1

l0000145A:
4B2F           	ldr	r3,[00001518]                           ; [pc,#&BC]
492F           	ldr	r1,[0000151C]                           ; [pc,#&BC]
4D2C           	ldr	r5,[00001510]                           ; [pc,#&B0]
4828           	ldr	r0,[00001504]                           ; [pc,#&A0]
1AC9           	sub	r1,r1,r3
F043 0412     	orr	r4,r3,#&12
2920           	cmps	r1,#&20
602A           	str	r2,[r5]
6004           	str	r4,[r0]
D938           	bls	$000014E2

l00001470:
2340           	mov	r3,#&40
2205           	mov	r2,#5
E002           	b	$0000147C

l00001476:
3201           	adds	r2,#1
2A1F           	cmps	r2,#&1F
D02C           	beq	$000014D6

l0000147C:
4299           	cmps	r1,r3
EA4F 0343     	mov.w	r3,r3,lsl #1
D8F8           	bhi	$00001476

l00001484:
4826           	ldr	r0,[00001520]                           ; [pc,#&98]
EA40 0042     	orr	r0,r0,r2,lsl #1

l0000148A:
2305           	mov	r3,#5
2240           	mov	r2,#&40
4E20           	ldr	r6,[00001510]                           ; [pc,#&80]
4C1C           	ldr	r4,[00001504]                           ; [pc,#&70]
4D24           	ldr	r5,[00001524]                           ; [pc,#&90]
4924           	ldr	r1,[00001528]                           ; [pc,#&90]
6030           	str	r0,[r6]
6025           	str	r5,[r4]

l0000149A:
3301           	adds	r3,#1
2B1F           	cmps	r3,#&1F
EA4F 0242     	mov.w	r2,r2,lsl #1
D012           	beq	$000014CA

l000014A4:
428A           	cmps	r2,r1
D9F8           	bls	$0000149A

l000014A8:
4A20           	ldr	r2,[0000152C]                           ; [pc,#&80]
EA42 0343     	orr	r3,r2,r3,lsl #1

l000014AE:
4A18           	ldr	r2,[00001510]                           ; [pc,#&60]
491F           	ldr	r1,[00001530]                           ; [pc,#&7C]
6013           	str	r3,[r2]
680B           	ldr	r3,[r1]
F443 3380     	orr	r3,r3,#&10000
600B           	str	r3,[r1]
F852 3C0C     	ldr.w	r3,[r2,-#&C]
F043 0305     	orr	r3,r3,#5
F842 3C0C     	str.w	r3,[r2,-#&C]
E781           	b	$000013CE

l000014CA:
4B1A           	ldr	r3,[00001534]                           ; [pc,#&68]
E7EF           	b	$000014AE

l000014CE:
4A1A           	ldr	r2,[00001538]                           ; [pc,#&68]
E7AC           	b	$0000142C

l000014D2:
4A1A           	ldr	r2,[0000153C]                           ; [pc,#&68]
E7C1           	b	$0000145A

l000014D6:
481A           	ldr	r0,[00001540]                           ; [pc,#&68]
E7D7           	b	$0000148A

l000014DA:
4A1A           	ldr	r2,[00001544]                           ; [pc,#&68]
E7BD           	b	$0000145A

l000014DE:
4A1A           	ldr	r2,[00001548]                           ; [pc,#&68]
E7A4           	b	$0000142C

l000014E2:
481A           	ldr	r0,[0000154C]                           ; [pc,#&68]
E7D1           	b	$0000148A
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
4770           	bx	lr
00001552       00 BF                                       ..           

;; vPortStoreTaskMPUSettings: 00001554
;;   Called from:
;;     00000774 (in prvInitialiseNewTask)
;;     00000978 (in vTaskAllocateMPURegions)
;;     00000986 (in vTaskAllocateMPURegions)
vPortStoreTaskMPUSettings proc
B430           	push	{r4-r5}
2900           	cmps	r1,#0
D041           	beq	$000015DE

l0000155A:
BB4B           	cbnz	r3,$000015B0

l0000155C:
2505           	mov	r5,#5

l0000155E:
684C           	ldr	r4,[r1,#&4]
B1FC           	cbz	r4,$000015A2

l00001562:
680B           	ldr	r3,[r1]
F045 0210     	orr	r2,r5,#&10
4313           	orrs	r3,r2
2C20           	cmps	r4,#&20
6083           	str	r3,[r0,#&8]
D96F           	bls	$00001650

l00001570:
2240           	mov	r2,#&40
2305           	mov	r3,#5
E002           	b	$0000157C

l00001576:
3301           	adds	r3,#1
2B1F           	cmps	r3,#&1F
D017           	beq	$000015AC

l0000157C:
4294           	cmps	r4,r2
EA4F 0242     	mov.w	r2,r2,lsl #1
D8F8           	bhi	$00001576

l00001584:
005B           	lsls	r3,r3,#1

l00001586:
688A           	ldr	r2,[r1,#&8]
F042 0201     	orr	r2,r2,#1
4313           	orrs	r3,r2
60C3           	str	r3,[r0,#&C]

l00001590:
3501           	adds	r5,#1
2D08           	cmps	r5,#8
F101 010C     	add	r1,r1,#&C
F100 0008     	add	r0,r0,#8
D1DF           	bne	$0000155E

l0000159E:
BC30           	pop	{r4-r5}
4770           	bx	lr

l000015A2:
F045 0310     	orr	r3,r5,#&10
60C4           	str	r4,[r0,#&C]
6083           	str	r3,[r0,#&8]
E7F1           	b	$00001590

l000015AC:
233E           	mov	r3,#&3E
E7EA           	b	$00001586

l000015B0:
009B           	lsls	r3,r3,#2
F042 0214     	orr	r2,r2,#&14
2B20           	cmps	r3,#&20
6002           	str	r2,[r0]
D94B           	bls	$00001654

l000015BC:
2240           	mov	r2,#&40
2405           	mov	r4,#5
E002           	b	$000015C8

l000015C2:
3401           	adds	r4,#1
2C1F           	cmps	r4,#&1F
D008           	beq	$000015DA

l000015C8:
4293           	cmps	r3,r2
EA4F 0242     	mov.w	r2,r2,lsl #1
D8F8           	bhi	$000015C2

l000015D0:
4B23           	ldr	r3,[00001660]                           ; [pc,#&8C]
EA43 0444     	orr	r4,r3,r4,lsl #1

l000015D6:
6044           	str	r4,[r0,#&4]
E7C0           	b	$0000155C

l000015DA:
4C22           	ldr	r4,[00001664]                           ; [pc,#&88]
E7FB           	b	$000015D6

l000015DE:
4B22           	ldr	r3,[00001668]                           ; [pc,#&88]
4922           	ldr	r1,[0000166C]                           ; [pc,#&88]
F043 0214     	orr	r2,r3,#&14
1AC9           	sub	r1,r1,r3
2920           	cmps	r1,#&20
6002           	str	r2,[r0]
D936           	bls	$0000165C

l000015EE:
2340           	mov	r3,#&40
2205           	mov	r2,#5
E002           	b	$000015FA

l000015F4:
3201           	adds	r2,#1
2A1F           	cmps	r2,#&1F
D026           	beq	$00001648

l000015FA:
428B           	cmps	r3,r1
EA4F 0343     	mov.w	r3,r3,lsl #1
D3F8           	blo	$000015F4

l00001602:
4B17           	ldr	r3,[00001660]                           ; [pc,#&5C]
EA43 0242     	orr	r2,r3,r2,lsl #1

l00001608:
4B19           	ldr	r3,[00001670]                           ; [pc,#&64]
491A           	ldr	r1,[00001674]                           ; [pc,#&68]
F043 0415     	orr	r4,r3,#&15
1AC9           	sub	r1,r1,r3
2920           	cmps	r1,#&20
6042           	str	r2,[r0,#&4]
6084           	str	r4,[r0,#&8]
D91E           	bls	$00001658

l0000161A:
2205           	mov	r2,#5
2340           	mov	r3,#&40
E002           	b	$00001626

l00001620:
3201           	adds	r2,#1
2A1F           	cmps	r2,#&1F
D012           	beq	$0000164C

l00001626:
4299           	cmps	r1,r3
EA4F 0343     	mov.w	r3,r3,lsl #1
D8F8           	bhi	$00001620

l0000162E:
4B12           	ldr	r3,[00001678]                           ; [pc,#&48]
EA43 0242     	orr	r2,r3,r2,lsl #1

l00001634:
2416           	mov	r4,#&16
2300           	mov	r3,#0
2117           	mov	r1,#&17
6104           	str	r4,[r0,#&10]
60C2           	str	r2,[r0,#&C]
6143           	str	r3,[r0,#&14]
61C3           	str	r3,[r0,#&1C]
6181           	str	r1,[r0,#&18]
BC30           	pop	{r4-r5}
4770           	bx	lr

l00001648:
4A06           	ldr	r2,[00001664]                           ; [pc,#&18]
E7DD           	b	$00001608

l0000164C:
4A0B           	ldr	r2,[0000167C]                           ; [pc,#&2C]
E7F1           	b	$00001634

l00001650:
2308           	mov	r3,#8
E798           	b	$00001586

l00001654:
4C0A           	ldr	r4,[00001680]                           ; [pc,#&28]
E7BE           	b	$000015D6

l00001658:
4A0A           	ldr	r2,[00001684]                           ; [pc,#&28]
E7EB           	b	$00001634

l0000165C:
4A08           	ldr	r2,[00001680]                           ; [pc,#&20]
E7D3           	b	$00001608
00001660 01 00 07 03 3F 00 07 03 00 00 00 20 00 20 00 20 ....?...... . . 
00001670 00 00 00 20 00 02 00 20 01 00 07 01 3F 00 07 01 ... ... ....?...
00001680 09 00 07 03 09 00 07 01                         ........       

;; xPortPendSVHandler: 00001688
xPortPendSVHandler proc
F3EF 8009     	mrs	r0,cpsr
4B14           	ldr	r3,[000016E0]                           ; [pc,#&50]
681A           	ldr	r2,[r3]
F3EF 8114     	mrs	r1,cpsr
E920 0FF2     	stmdb	r0!,{r1,r4-fp}
6010           	str	r0,[r2]
E92D 4008     	push.w	{r3,lr}
F04F 00BF     	mov	r0,#&BF
F380 8811     	msr	cpsr,r0
F7FF FD17     	bl	$000010D8
F04F 0000     	mov	r0,#0
F380 8811     	msr	cpsr,r0
E8BD 4008     	pop.w	{r3,lr}
6819           	ldr	r1,[r3]
6808           	ldr	r0,[r1]
F101 0104     	add	r1,r1,#4
4A1A           	ldr	r2,[00001728]                           ; [pc,#&68]
E8B1 0FF0     	ldm	r1!,{r4-fp}
E8A2 0FF0     	stm	r2!,{r4-fp}
E8B0 0FF8     	ldm	r0!,{r3-fp}
F383 8814     	msr	cpsr,r3
F380 8809     	msr	cpsr,r0
4770           	bx	lr
000016D6                   00 BF AF F3 00 80 AF F3 00 80       ..........
000016E0 C8 00 00 20                                     ...            

;; xPortSysTickHandler: 000016E4
xPortSysTickHandler proc
B510           	push	{r4,lr}
F3EF 8411     	mrs	r4,cpsr
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
F7FF FB37     	bl	$00000D6C
B118           	cbz	r0,$00001708

l00001700:
F04F 5280     	mov	r2,#&10000000
4B02           	ldr	r3,[00001710]                           ; [pc,#&8]
601A           	str	r2,[r3]

l00001708:
F384 8811     	msr	cpsr,r4
BD10           	pop	{r4,pc}
0000170E                                           00 BF               ..
00001710 04 ED 00 E0                                     ....           

;; vPortSVCHandler: 00001714
vPortSVCHandler proc
F01E 0F04     	tst	lr,#4
BF0C           	ite	eq
F3EF 8008     	mrseq	r0,cpsr

l0000171E:
F3EF 8009     	mrs	r0,cpsr
E607           	b	$00001334
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
B510           	push	{r4,lr}
4604           	mov	r4,r0
0743           	lsls	r3,r0,#&1D
BF1C           	itt	ne
F020 0407     	bicne	r4,r0,#7

l00001738:
3408           	adds	r4,#8
F7FF F967     	bl	$00000A0C
4B0F           	ldr	r3,[0000177C]                           ; [pc,#&3C]
681A           	ldr	r2,[r3]
B1AA           	cbz	r2,$00001770

l00001744:
F240 51B3     	mov	r1,#&5B3
F8D3 25C0     	ldr	r2,[r3,#&5C0]
4414           	adds	r4,r2
428C           	cmps	r4,r1
D809           	bhi	$00001766

l00001752:
42A2           	cmps	r2,r4
D207           	bhs	$00001766

l00001756:
6819           	ldr	r1,[r3]
F8C3 45C0     	str	r4,[r3,#&5C0]
188C           	add	r4,r1,r2
F7FF FB85     	bl	$00000E6C
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

l00001766:
2400           	mov	r4,#0
F7FF FB80     	bl	$00000E6C
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

l00001770:
F103 020C     	add	r2,r3,#&C
F022 0207     	bic	r2,r2,#7
601A           	str	r2,[r3]
E7E3           	b	$00001744
0000177C                                     30 02 00 20             0.. 

;; vPortFree: 00001780
;;   Called from:
;;     00000454 (in vQueueDelete)
;;     00000910 (in xTaskCreate)
;;     000019C2 (in vEventGroupDelete)
;;     00008CCC (in MPU_vPortFree)
vPortFree proc
4770           	bx	lr
00001782       00 BF                                       ..           

;; vPortInitialiseBlocks: 00001784
;;   Called from:
;;     00008CEC (in MPU_vPortInitialiseBlocks)
vPortInitialiseBlocks proc
2200           	mov	r2,#0
4B02           	ldr	r3,[00001790]                           ; [pc,#&8]
F8C3 25C0     	str	r2,[r3,#&5C0]
4770           	bx	lr
0000178E                                           00 BF               ..
00001790 30 02 00 20                                     0..            

;; xPortGetFreeHeapSize: 00001794
;;   Called from:
;;     00008D0C (in MPU_xPortGetFreeHeapSize)
xPortGetFreeHeapSize proc
4B03           	ldr	r3,[000017A4]                           ; [pc,#&C]
F8D3 05C0     	ldr	r0,[r3,#&5C0]
F5C0 60B6     	rsb	r0,r0,#&5B0
3004           	adds	r0,#4
4770           	bx	lr
000017A2       00 BF 30 02 00 20                           ..0..        

;; xEventGroupCreate: 000017A8
;;   Called from:
;;     00008D30 (in MPU_xEventGroupCreate)
xEventGroupCreate proc
B510           	push	{r4,lr}
2018           	mov	r0,#&18
F7FF FFBE     	bl	$0000172C
4604           	mov	r4,r0
B120           	cbz	r0,$000017BE

l000017B4:
2300           	mov	r3,#0
F840 3B04     	str	r3,[r0],#&4
F006 FD89     	bl	$000082D0

l000017BE:
4620           	mov	r0,r4
BD10           	pop	{r4,pc}
000017C2       00 BF                                       ..           

;; xEventGroupWaitBits: 000017C4
;;   Called from:
;;     00008D6C (in MPU_xEventGroupWaitBits)
xEventGroupWaitBits proc
E92D 41F0     	push.w	{r4-r8,lr}
4606           	mov	r6,r0
461F           	mov	r7,r3
460D           	mov	r5,r1
4690           	mov	r8,r2
F7FF F91C     	bl	$00000A0C
6834           	ldr	r4,[r6]
B967           	cbnz	r7,$000017F2

l000017D8:
422C           	adcs	r4,r5
D00D           	beq	$000017F8

l000017DC:
F1B8 0F00     	cmp	r8,#0
D002           	beq	$000017E8

l000017E2:
EA24 0505     	bic.w	r5,r4,r5
6035           	str	r5,[r6]

l000017E8:
F7FF FB40     	bl	$00000E6C
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l000017F2:
EA35 0304     	bics.w	r3,r5,r4
D0F1           	beq	$000017DC

l000017F8:
9B06           	ldr	r3,[sp,#&18]
2B00           	cmps	r3,#0
D0F4           	beq	$000017E8

l000017FE:
F1B8 0F00     	cmp	r8,#0
BF0C           	ite	eq
2100           	moveq	r1,#0

l00001806:
F04F 7180     	mov	r1,#&1000000
B9C7           	cbnz	r7,$0000183E

l0000180C:
4329           	orrs	r1,r5
9A06           	ldr	r2,[sp,#&18]
1D30           	add	r0,r6,#4
F7FF FBF1     	bl	$00000FF8
F7FF FB29     	bl	$00000E6C
B938           	cbnz	r0,$0000182C

l0000181C:
F04F 5280     	mov	r2,#&10000000
4B13           	ldr	r3,[00001870]                           ; [pc,#&4C]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l0000182C:
F7FF FC78     	bl	$00001120
0183           	lsls	r3,r0,#6
4604           	mov	r4,r0
D506           	bpl	$00001844

l00001836:
F024 407F     	bic	r0,r4,#&FF000000
E8BD 81F0     	pop.w	{r4-r8,pc}

l0000183E:
F041 6180     	orr	r1,r1,#&4000000
E7E3           	b	$0000180C

l00001844:
F006 FE98     	bl	$00008578
6834           	ldr	r4,[r6]
B96F           	cbnz	r7,$00001868

l0000184C:
4225           	adcs	r5,r4
D005           	beq	$0000185C

l00001850:
F1B8 0F00     	cmp	r8,#0
D002           	beq	$0000185C

l00001856:
EA24 0505     	bic.w	r5,r4,r5
6035           	str	r5,[r6]

l0000185C:
F006 FEA8     	bl	$000085B0
F024 407F     	bic	r0,r4,#&FF000000
E8BD 81F0     	pop.w	{r4-r8,pc}

l00001868:
EA35 0304     	bics.w	r3,r5,r4
D1F6           	bne	$0000185C

l0000186E:
E7EF           	b	$00001850
00001870 04 ED 00 E0                                     ....           

;; xEventGroupClearBits: 00001874
;;   Called from:
;;     00008D9C (in MPU_xEventGroupClearBits)
xEventGroupClearBits proc
B570           	push	{r4-r6,lr}
4606           	mov	r6,r0
460C           	mov	r4,r1
F006 FE7D     	bl	$00008578
6835           	ldr	r5,[r6]
EA25 0404     	bic.w	r4,r5,r4
6034           	str	r4,[r6]
F006 FE93     	bl	$000085B0
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}
0000188E                                           00 BF               ..

;; xEventGroupSetBits: 00001890
;;   Called from:
;;     00001910 (in xEventGroupSync)
;;     000019D0 (in vEventGroupSetBitsCallback)
;;     00008DC8 (in MPU_xEventGroupSetBits)
xEventGroupSetBits proc
B5F8           	push	{r3-r7,lr}
4605           	mov	r5,r0
460C           	mov	r4,r1
F7FF F8B9     	bl	$00000A0C
6829           	ldr	r1,[r5]
6928           	ldr	r0,[r5,#&10]
F105 060C     	add	r6,r5,#&C
4321           	orrs	r1,r4
4286           	cmps	r6,r0
6029           	str	r1,[r5]
D022           	beq	$000018F0

l000018AA:
2700           	mov	r7,#0
E00C           	b	$000018C8

l000018AE:
420A           	adcs	r2,r1
D007           	beq	$000018C2

l000018B2:
01DB           	lsls	r3,r3,#7
D500           	bpl	$000018B8

l000018B6:
4317           	orrs	r7,r2

l000018B8:
F041 7100     	orr	r1,r1,#&2000000
F7FF FBE0     	bl	$00001080
6829           	ldr	r1,[r5]

l000018C2:
42A6           	cmps	r6,r4
4620           	mov	r0,r4
D00C           	beq	$000018E2

l000018C8:
E890 0018     	ldm	r0,{r3-r4}
F013 6F80     	tst	r3,#&4000000
F023 427F     	bic	r2,r3,#&FF000000
D0EB           	beq	$000018AE

l000018D6:
EA32 0E01     	bics.w	lr,r2,r1
D0EA           	beq	$000018B2

l000018DC:
42A6           	cmps	r6,r4
4620           	mov	r0,r4
D1F2           	bne	$000018C8

l000018E2:
43FF           	mvns	r7,r7

l000018E4:
4039           	ands	r1,r7
6029           	str	r1,[r5]
F7FF FAC0     	bl	$00000E6C
6828           	ldr	r0,[r5]
BDF8           	pop	{r3-r7,pc}

l000018F0:
F04F 37FF     	mov	r7,#&FFFFFFFF
E7F6           	b	$000018E4
000018F6                   00 BF                               ..       

;; xEventGroupSync: 000018F8
;;   Called from:
;;     00008DFE (in MPU_xEventGroupSync)
xEventGroupSync proc
E92D 41F0     	push.w	{r4-r8,lr}
4688           	mov	r8,r1
4605           	mov	r5,r0
4616           	mov	r6,r2
461F           	mov	r7,r3
F7FF F882     	bl	$00000A0C
4641           	mov	r1,r8
682C           	ldr	r4,[r5]
4628           	mov	r0,r5
430C           	orrs	r4,r1
F7FF FFBE     	bl	$00001890
EA36 0304     	bics.w	r3,r6,r4
D021           	beq	$0000195E

l0000191A:
B92F           	cbnz	r7,$00001928

l0000191C:
682C           	ldr	r4,[r5]

l0000191E:
F7FF FAA5     	bl	$00000E6C
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l00001928:
463A           	mov	r2,r7
F046 61A0     	orr	r1,r6,#&5000000
1D28           	add	r0,r5,#4
F7FF FB62     	bl	$00000FF8
F7FF FA9A     	bl	$00000E6C
B938           	cbnz	r0,$0000194A

l0000193A:
F04F 5280     	mov	r2,#&10000000
4B11           	ldr	r3,[00001984]                           ; [pc,#&44]
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l0000194A:
F7FF FBE9     	bl	$00001120
0183           	lsls	r3,r0,#6
4604           	mov	r4,r0
D509           	bpl	$00001968

l00001954:
F024 447F     	bic	r4,r4,#&FF000000

l00001958:
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l0000195E:
682B           	ldr	r3,[r5]
EA23 0606     	bic.w	r6,r3,r6
602E           	str	r6,[r5]
E7DA           	b	$0000191E

l00001968:
F006 FE06     	bl	$00008578
682C           	ldr	r4,[r5]
EA36 0304     	bics.w	r3,r6,r4
BF04           	itt	eq
EA24 0606     	biceq.w	r6,r4,r6

l00001978:
602E           	str	r6,[r5]
F006 FE19     	bl	$000085B0
F024 447F     	bic	r4,r4,#&FF000000
E7E9           	b	$00001958
00001984             04 ED 00 E0                             ....       

;; xEventGroupGetBitsFromISR: 00001988
xEventGroupGetBitsFromISR proc
F3EF 8311     	mrs	r3,cpsr
F04F 02BF     	mov	r2,#&BF
F382 8811     	msr	cpsr,r2
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
F383 8811     	msr	cpsr,r3
6800           	ldr	r0,[r0]
4770           	bx	lr

;; vEventGroupDelete: 000019A4
;;   Called from:
;;     00008E28 (in MPU_vEventGroupDelete)
vEventGroupDelete proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F7FF F830     	bl	$00000A0C
6863           	ldr	r3,[r4,#&4]
B13B           	cbz	r3,$000019C0

l000019B0:
F04F 7100     	mov	r1,#&2000000
6920           	ldr	r0,[r4,#&10]
F7FF FB63     	bl	$00001080
6863           	ldr	r3,[r4,#&4]
2B00           	cmps	r3,#0
D1F7           	bne	$000019B0

l000019C0:
4620           	mov	r0,r4
F7FF FEDD     	bl	$00001780
E8BD 4010     	pop.w	{r4,lr}
F7FF BA4F     	b	$00000E6C
000019CE                                           00 BF               ..

;; vEventGroupSetBitsCallback: 000019D0
vEventGroupSetBitsCallback proc
F7FF BF5E     	b	$00001890

;; vEventGroupClearBitsCallback: 000019D4
vEventGroupClearBitsCallback proc
B538           	push	{r3-r5,lr}
4604           	mov	r4,r0
460D           	mov	r5,r1
F006 FDCD     	bl	$00008578
6823           	ldr	r3,[r4]
EA23 0305     	bic.w	r3,r3,r5
6023           	str	r3,[r4]
E8BD 4038     	pop.w	{r3-r5,lr}
F006 BDE1     	b	$000085B0
000019EE                                           00 BF               ..
000019F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
;;; Segment .text (00008000)

;; NmiSR: 00008000
NmiSR proc
E7FE           	b	$00008000
00008002       00 BF                                       ..           

;; FaultISR: 00008004
FaultISR proc
E7FE           	b	$00008004
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
F000 B83B     	b	$000080A0
0000802A                               00 BF 60 01 00 20           ..`.. 
00008030 80 08 00 20                                     ...            

;; raise: 00008034
raise proc
E7FE           	b	$00008034
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
F000 FD8E     	bl	$00008B6C
F001 FB96     	bl	$00009780
F004 0201     	and	r2,r4,#1
F004 013F     	and	r1,r4,#&3F
9801           	ldr	r0,[sp,#&4]
F001 FBB5     	bl	$000097CC
E7ED           	b	$00008040
00008064             80 08 00 20                             ...        

;; vCheckTask: 00008068
vCheckTask proc
B530           	push	{r4-r5,lr}
4B0B           	ldr	r3,[00008098]                           ; [pc,#&2C]
B083           	sub	sp,#&C
9301           	str	r3,[sp,#&4]
F000 FC48     	bl	$00008904
AC02           	add	r4,sp,#8
4D09           	ldr	r5,[0000809C]                           ; [pc,#&24]
F844 0D08     	str	r0,[r4,-#&8]!

l0000807C:
4620           	mov	r0,r4
F241 3188     	mov	r1,#&1388
F000 FBF7     	bl	$00008874
2300           	mov	r3,#0
F04F 32FF     	mov	r2,#&FFFFFFFF
A901           	add	r1,sp,#4
6828           	ldr	r0,[r5]
F000 FD28     	bl	$00008AE4
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
F000 FCEC     	bl	$00008A88
4B0F           	ldr	r3,[000080F0]                           ; [pc,#&3C]
6018           	str	r0,[r3]
4620           	mov	r0,r4
F001 FC1B     	bl	$000098F0
2203           	mov	r2,#3
4623           	mov	r3,r4
9200           	str	r2,[sp]
490C           	ldr	r1,[000080F4]                           ; [pc,#&30]
223B           	mov	r2,#&3B
9401           	str	r4,[sp,#&4]
480C           	ldr	r0,[000080F8]                           ; [pc,#&30]
F000 FB9E     	bl	$00008808
2202           	mov	r2,#2
490B           	ldr	r1,[000080FC]                           ; [pc,#&2C]
4623           	mov	r3,r4
9200           	str	r2,[sp]
9401           	str	r4,[sp,#&4]
223B           	mov	r2,#&3B
4809           	ldr	r0,[00008100]                           ; [pc,#&24]
F000 FB95     	bl	$00008808
F7F8 FC57     	bl	$00000990
4622           	mov	r2,r4
4621           	mov	r1,r4
4807           	ldr	r0,[00008104]                           ; [pc,#&1C]
F001 FB70     	bl	$000097CC

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
F001 FFD9     	bl	$0000A0CC
4604           	mov	r4,r0
4601           	mov	r1,r0
4628           	mov	r0,r5
F001 FFDA     	bl	$0000A0D8
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
F7F8 F973     	bl	$00000458
E7DD           	b	$00008130
00008174             00 C0 00 40 18 C0 00 40 2C 02 00 20     ...@...@,.. 
00008180 04 ED 00 E0                                     ....           

;; vSetErrorLED: 00008184
vSetErrorLED proc
2101           	mov	r1,#1
2007           	mov	r0,#7
F000 BA34     	b	$000085F4

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
4770           	bx	lr
BF00           	nop

;; vApplicationIdleHook: 00008210
;;   Called from:
;;     0000852E (in prvIdleTask)
vApplicationIdleHook proc
B508           	push	{r3,lr}

l00008212:
F000 FE8B     	bl	$00008F2C
F7FF FFB9     	bl	$0000818C
E7FA           	b	$00008212
0000821C                                     85 81 00 00             ....

;; PDCInit: 00008220
;;   Called from:
;;     000085DE (in vParTestInitialise)
PDCInit proc
B530           	push	{r4-r5,lr}
481A           	ldr	r0,[0000828C]                           ; [pc,#&68]
B083           	sub	sp,#&C
F001 FCA9     	bl	$00009B7C
4819           	ldr	r0,[00008290]                           ; [pc,#&64]
F001 FCA6     	bl	$00009B7C
2202           	mov	r2,#2
2134           	mov	r1,#&34
F04F 2040     	mov	r0,#&40004000
F000 FF68     	bl	$0000910C
2201           	mov	r2,#1
2108           	mov	r1,#8
F04F 2040     	mov	r0,#&40004000
F000 FF62     	bl	$0000910C
230A           	mov	r3,#&A
2202           	mov	r2,#2
2104           	mov	r1,#4
F04F 2040     	mov	r0,#&40004000
F000 FFB9     	bl	$000091C8
2408           	mov	r4,#8
2200           	mov	r2,#0
4D0E           	ldr	r5,[00008294]                           ; [pc,#&38]
4611           	mov	r1,r2
4B0E           	ldr	r3,[00008298]                           ; [pc,#&38]
4628           	mov	r0,r5
9400           	str	r4,[sp]
F001 FBC0     	bl	$000099E8
4628           	mov	r0,r5
F001 FBE3     	bl	$00009A34
4621           	mov	r1,r4
2200           	mov	r2,#0
F04F 2040     	mov	r0,#&40004000
F001 F8ED     	bl	$00009454
4622           	mov	r2,r4
4621           	mov	r1,r4
F04F 2040     	mov	r0,#&40004000
B003           	add	sp,#&C
E8BD 4030     	pop.w	{r4-r5,lr}
F001 B8E4     	b	$00009454
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
F001 FBF5     	bl	$00009A98
4629           	mov	r1,r5
4620           	mov	r0,r4
F001 FBF1     	bl	$00009A98
4620           	mov	r0,r4
A901           	add	r1,sp,#4
F001 FBFD     	bl	$00009AB8
A901           	add	r1,sp,#4
4620           	mov	r0,r4
F001 FBF9     	bl	$00009AB8
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
F000 F8FC     	bl	$00008578
6BAA           	ldr	r2,[r5,#&38]
6BEB           	ldr	r3,[r5,#&3C]
429A           	cmps	r2,r3
D014           	beq	$000083B2

l00008388:
F000 F912     	bl	$000085B0
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
F000 F8FD     	bl	$000085B0
B97C           	cbnz	r4,$000083D8

l000083B8:
F384 8811     	msr	cpsr,r4
4620           	mov	r0,r4
BD70           	pop	{r4-r6,pc}

l000083C0:
4602           	mov	r2,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FE91     	bl	$000000EC
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
F000 FD87     	bl	$00008EF0
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
F06F 0003     	mvn	r0,#3
BD70           	pop	{r4-r6,pc}

l000083EE:
F105 0024     	add	r0,r5,#&24
F000 FE4F     	bl	$00009094
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
F002 F8AD     	bl	$0000A5C4
6923           	ldr	r3,[r4,#&10]
B923           	cbnz	r3,$00008478

l0000846E:
2001           	mov	r0,#1
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
BD38           	pop	{r3-r5,pc}

l00008478:
F104 0010     	add	r0,r4,#&10
F000 FE0A     	bl	$00009094
2800           	cmps	r0,#0
D0F4           	beq	$0000846E

l00008484:
F06F 0004     	mvn	r0,#4
E7DA           	b	$00008440

l0000848A:
F100 0124     	add	r1,r0,#&24
4610           	mov	r0,r2
F000 FD2E     	bl	$00008EF0
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
F7F7 FE1A     	bl	$000000EC
2D00           	cmps	r5,#0
D1F7           	bne	$000084AC

l000084BC:
6A63           	ldr	r3,[r4,#&24]
2B00           	cmps	r3,#0
D0F4           	beq	$000084AC

l000084C2:
F104 0024     	add	r0,r4,#&24
F000 FDE5     	bl	$00009094
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
F002 F85E     	bl	$0000A5C4
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
F000 FDBA     	bl	$00009094
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
F7FF FE6F     	bl	$00008210
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
F000 F81D     	bl	$00008578
F894 3064     	ldrb	r3,[r4,#&64]
2B02           	cmps	r3,#2
BF05           	ittet	eq
2300           	moveq	r3,#0

l00008548:
2501           	mov	r5,#1
2500           	mov	r5,#0
F884 3064     	strb	r3,[r4,#&64]
F000 F82E     	bl	$000085B0
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
F7FF FFF3     	bl	$00008564
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
F7FF FFD7     	bl	$00008564
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
F7FF FE1F     	bl	$00008220
4B03           	ldr	r3,[000085F0]                           ; [pc,#&C]
2005           	mov	r0,#5
7819           	ldrb	r1,[r3]
E8BD 4008     	pop.w	{r3,lr}
F7FF BE56     	b	$0000829C
000085F0 F4 07 00 20                                     ...            

;; vParTestSetLED: 000085F4
;;   Called from:
;;     00008188 (in vSetErrorLED)
vParTestSetLED proc
B538           	push	{r3-r5,lr}
4604           	mov	r4,r0
460D           	mov	r5,r1
F000 F961     	bl	$000088C0
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
F7FF FE40     	bl	$0000829C

l0000861C:
E8BD 4038     	pop.w	{r3-r5,lr}
F000 B95E     	b	$000088E0

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
F000 F944     	bl	$000088C0
2C07           	cmps	r4,#7
D80E           	bhi	$0000865A

l0000863C:
2201           	mov	r2,#1
4B0B           	ldr	r3,[0000866C]                           ; [pc,#&2C]
FA02 F004     	lsl	r0,r2,r4
7819           	ldrb	r1,[r3]
B2C2           	uxtb	r2,r0
420A           	adcs	r2,r1
D10A           	bne	$00008662

l0000864C:
7819           	ldrb	r1,[r3]
430A           	orrs	r2,r1
701A           	strb	r2,[r3]

l00008652:
7819           	ldrb	r1,[r3]
2005           	mov	r0,#5
F7FF FE21     	bl	$0000829C

l0000865A:
E8BD 4010     	pop.w	{r4,lr}
F000 B93F     	b	$000088E0

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
F7FF FFCD     	bl	$00008630

l00008696:
F04F 32FF     	mov	r2,#&FFFFFFFF
4631           	mov	r1,r6
6828           	ldr	r0,[r5]
F7FF FEAF     	bl	$00008400
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
F7FF FE9E     	bl	$00008400
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
F7FF FE1E     	bl	$00008364
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
F7FF FE04     	bl	$00008364
E7E6           	b	$0000872C

l0000875E:
2100           	mov	r1,#0
F000 FBC6     	bl	$00008EF0
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
F000 F978     	bl	$00008A88
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
F000 FB48     	bl	$00008E40
42AC           	cmps	r4,r5
D1F7           	bne	$000087A4

l000087B4:
2200           	mov	r2,#0
E8BD 4070     	pop.w	{r4-r6,lr}
2101           	mov	r1,#1
4803           	ldr	r0,[000087CC]                           ; [pc,#&C]
F000 BB3F     	b	$00008E40

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
F7FF FEBF     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F896     	bl	$0000091C
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
F7FF FEA3     	bl	$00008564
4653           	mov	r3,r10
4604           	mov	r4,r0
9700           	str	r7,[sp]
9601           	str	r6,[sp,#&4]
464A           	mov	r2,r9
4641           	mov	r1,r8
4628           	mov	r0,r5
F7F8 F842     	bl	$000008B4
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
F7FF FE87     	bl	$00008564
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F888     	bl	$00000970
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
F7FF FE73     	bl	$00008564
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FB7C     	bl	$00000F80
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
F7FF FE60     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FB4E     	bl	$00000F48
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
F7FF FE4F     	bl	$00008564
4604           	mov	r4,r0
F7F8 F8A0     	bl	$00000A0C
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
F7FF FE3F     	bl	$00008564
4604           	mov	r4,r0
F7F8 FAC0     	bl	$00000E6C
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
F7FF FE2D     	bl	$00008564
4604           	mov	r4,r0
F7F8 F888     	bl	$00000A20
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
F7FF FE1B     	bl	$00008564
4604           	mov	r4,r0
F7F8 F882     	bl	$00000A38
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
F7FF FE08     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F874     	bl	$00000A44
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
F7FF FDF4     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FBE0     	bl	$00001144
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
F7FF FDE1     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FBD6     	bl	$00001158
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
F7FF FDC8     	bl	$00008564
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F83B     	bl	$00000A58
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
F7FF FDAC     	bl	$00008564
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F8DD     	bl	$00000BD4
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
F7FF FD93     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F95C     	bl	$00000D00
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
F7FF FD7E     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7FF FD62     	bl	$00008534
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
F7FF FD68     	bl	$00008564
463A           	mov	r2,r7
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FE06     	bl	$000006AC
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
F7FF FD51     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FDB2     	bl	$00000630
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
F7FF FD38     	bl	$00008564
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FB47     	bl	$00000190
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
F7FF FD20     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FC7E     	bl	$00000428
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
F7FF FD0C     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FC74     	bl	$0000043C
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
F7FF FCF4     	bl	$00008564
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FBA7     	bl	$000002D8
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
F7FF FCDB     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FB76     	bl	$000002A4
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
F7FF FCC6     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCEA     	bl	$000005B4
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
F7FF FCB2     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FD6A     	bl	$000006DC
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
F7FF FC9D     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCD0     	bl	$000005D4
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
F7FF FC88     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCD4     	bl	$00000604
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
F7FF FC74     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FBE8     	bl	$00000454
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
F7FF FC62     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD42     	bl	$0000172C
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
F7FF FC4E     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD58     	bl	$00001780
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
F7FF FC3D     	bl	$00008564
4604           	mov	r4,r0
F7F8 FD4A     	bl	$00001784
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
F7FF FC2D     	bl	$00008564
4604           	mov	r4,r0
F7F8 FD42     	bl	$00001794
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
F7FF FC1B     	bl	$00008564
4604           	mov	r4,r0
F7F8 FD3A     	bl	$000017A8
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
F7FF FC02     	bl	$00008564
464B           	mov	r3,r9
4604           	mov	r4,r0
9700           	str	r7,[sp]
4642           	mov	r2,r8
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FD2A     	bl	$000017C4
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
F7FF FBE7     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD6A     	bl	$00001874
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
F7FF FBD1     	bl	$00008564
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD62     	bl	$00001890
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
F7FF FBB8     	bl	$00008564
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FD7B     	bl	$000018F8
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
F7FF FBA0     	bl	$00008564
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FDBC     	bl	$000019A4
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
F7F8 FC6E     	bl	$0000172C
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
F7FF FA36     	bl	$000082E8
F104 0018     	add	r0,r4,#&18
F7FF FA32     	bl	$000082E8
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
F7FF FA25     	bl	$000082F0
2001           	mov	r0,#1
E8BD 8FF8     	pop.w	{r3-fp,pc}

l00008EAC:
46B8           	mov	r8,r7
F848 0B04     	str	r0,[r8],#&4
4640           	mov	r0,r8
F7FF FA0C     	bl	$000082D0
F107 0B2C     	add	fp,r7,#&2C
F107 0018     	add	r0,r7,#&18
F7FF FA06     	bl	$000082D0
F107 0640     	add	r6,r7,#&40
4658           	mov	r0,fp
F7FF FA01     	bl	$000082D0
4630           	mov	r0,r6
F7FF F9FE     	bl	$000082D0
F107 0054     	add	r0,r7,#&54
F7FF F9FA     	bl	$000082D0
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
F7FF FA1F     	bl	$00008340
6F63           	ldr	r3,[r4,#&74]
6821           	ldr	r1,[r4]
429D           	cmps	r5,r3
604D           	str	r5,[r1,#&4]
BF34           	ite	lo
6EE0           	ldrlo	r0,[r4,#&6C]

l00008F0E:
6EA0           	ldr	r0,[r4,#&68]
3104           	adds	r1,#4
F7FF F9FB     	bl	$0000830C
B136           	cbz	r6,$00008F26

l00008F18:
6821           	ldr	r1,[r4]
4630           	mov	r0,r6
E8BD 4070     	pop.w	{r4-r6,lr}
3118           	adds	r1,#&18
F7FF B9F3     	b	$0000830C

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
F7FF F9F4     	bl	$00008340
F387 8811     	msr	cpsr,r7
1D26           	add	r6,r4,#4
4630           	mov	r0,r6
F7FF F9EE     	bl	$00008340
6AE3           	ldr	r3,[r4,#&2C]
6F2A           	ldr	r2,[r5,#&70]
EB03 0083     	add.w	r0,r3,r3,lsl #2
4293           	cmps	r3,r2
4631           	mov	r1,r6
EB08 0080     	add.w	r0,r8,r0,lsl #2
BF88           	it	hi
672B           	strhi	r3,[r5,#&70]

l00008F78:
F7FF F9BA     	bl	$000082F0
6D6B           	ldr	r3,[r5,#&54]
2B00           	cmps	r3,#0
D1DC           	bne	$00008F3C

l00008F82:
F7FF FCBF     	bl	$00008904
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
F7FF F9B2     	bl	$00008340
6AA3           	ldr	r3,[r4,#&28]
F104 0018     	add	r0,r4,#&18
B10B           	cbz	r3,$00008FE8

l00008FE4:
F7FF F9AC     	bl	$00008340

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
F7FF F976     	bl	$000082F0
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
F7FF F94D     	bl	$00008340
F105 0054     	add	r0,r5,#&54
4631           	mov	r1,r6
F7FF F920     	bl	$000082F0
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
420C           	adcs	r4,r1
BF08           	it	eq
2300           	moveq	r3,#0

l0000914C:
420A           	adcs	r2,r1
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
420A           	adcs	r2,r1
F8D0 040C     	ldr	r0,[r0,#&40C]
BF14           	ite	ne
2201           	movne	r2,#1

l000091AE:
2200           	mov	r2,#0
420B           	adcs	r3,r1
BF14           	ite	ne
2302           	movne	r3,#2

l000091B6:
2300           	mov	r3,#0
4208           	adcs	r0,r1
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
420D           	adcs	r5,r1
F8D0 5508     	ldr	r5,[r0,#&508]
BF14           	ite	ne
2701           	movne	r7,#1

l00009278:
2700           	mov	r7,#0
420C           	adcs	r4,r1
F8D0 4518     	ldr	r4,[r0,#&518]
BF14           	ite	ne
2602           	movne	r6,#2

l00009284:
2600           	mov	r6,#0
420D           	adcs	r5,r1
BF14           	ite	ne
2504           	movne	r5,#4

l0000928C:
2500           	mov	r5,#0
420C           	adcs	r4,r1
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
4211           	adcs	r1,r2
F8D0 6514     	ldr	r6,[r0,#&514]
BF18           	it	ne
2501           	movne	r5,#1

l000092B0:
F8D0 251C     	ldr	r2,[r0,#&51C]
BF08           	it	eq
2500           	moveq	r5,#0

l000092B8:
4221           	adcs	r1,r4
BF14           	ite	ne
2402           	movne	r4,#2

l000092BE:
2400           	mov	r4,#0
4231           	adcs	r1,r6
BF14           	ite	ne
2004           	movne	r0,#4

l000092C6:
2000           	mov	r0,#0
4211           	adcs	r1,r2
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
F000 F8E8     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B94F     	b	$000095DC

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
F000 F8D8     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B93F     	b	$000095DC

l0000935E:
F04F 34FF     	mov	r4,#&FFFFFFFF
4620           	mov	r0,r4
F000 F8CE     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B935     	b	$000095DC

l00009372:
2413           	mov	r4,#&13
4620           	mov	r0,r4
F000 F8C5     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B92C     	b	$000095DC

l00009384:
2410           	mov	r4,#&10
4620           	mov	r0,r4
F000 F8BC     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B923     	b	$000095DC

l00009396:
2412           	mov	r4,#&12
4620           	mov	r0,r4
F000 F8B3     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B91A     	b	$000095DC
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
F000 F934     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B8AF     	b	$00009538

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
F000 F924     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B89F     	b	$00009538

l000093FA:
F04F 34FF     	mov	r4,#&FFFFFFFF
4620           	mov	r0,r4
F000 F91A     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B895     	b	$00009538

l0000940E:
2413           	mov	r4,#&13
4620           	mov	r0,r4
F000 F911     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B88C     	b	$00009538

l00009420:
2410           	mov	r4,#&10
4620           	mov	r0,r4
F000 F908     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B883     	b	$00009538

l00009432:
2412           	mov	r4,#&12
4620           	mov	r0,r4
F000 F8FF     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B87A     	b	$00009538
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
F7FF BEA5     	b	$000091C8
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
F7FF BE92     	b	$000091C8

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
F7FF BE80     	b	$000091C8

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
F7FF BE6E     	b	$000091C8

;; GPIOPinTypeTimer: 000094EC
GPIOPinTypeTimer proc
F7FF BFEC     	b	$000094C8

;; GPIOPinTypeSSI: 000094F0
GPIOPinTypeSSI proc
F7FF BFEA     	b	$000094C8

;; GPIOPinTypePWM: 000094F4
GPIOPinTypePWM proc
F7FF BFE8     	b	$000094C8

;; IntDefaultHandler: 000094F8
IntDefaultHandler proc
E7FE           	b	$000094F8
000094FA                               00 BF                       ..   

;; IntMasterEnable: 000094FC
IntMasterEnable proc
F000 BDEE     	b	$0000A0DC

;; IntMasterDisable: 00009500
IntMasterDisable proc
F000 BDF0     	b	$0000A0E4

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
D1FD           	bne	$00009694

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
F000 FDAE     	bl	$0000A208
4629           	mov	r1,r5
4620           	mov	r0,r4
F000 FDC4     	bl	$0000A23C
4620           	mov	r0,r4
E8BD 4038     	pop.w	{r3-r5,lr}
2103           	mov	r1,#3
F000 BDB0     	b	$0000A220
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
F000 FD78     	bl	$0000A1C8
2800           	cmps	r0,#0
D0F9           	beq	$000096D0

l000096DC:
6838           	ldr	r0,[r7]
F7FF FFD9     	bl	$00009694
F815 1B01     	ldrb	r1,[r5],#&1
4620           	mov	r0,r4
F000 FDA8     	bl	$0000A23C
2101           	mov	r1,#1
4620           	mov	r0,r4
F000 FD96     	bl	$0000A220
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
F000 FD5C     	bl	$0000A1C8
2800           	cmps	r0,#0
D0F9           	beq	$00009708

l00009714:
4B07           	ldr	r3,[00009734]                           ; [pc,#&1C]
6818           	ldr	r0,[r3]
F7FF FFBC     	bl	$00009694
4621           	mov	r1,r4
4804           	ldr	r0,[00009730]                           ; [pc,#&10]
F000 FD8C     	bl	$0000A23C
E8BD 4010     	pop.w	{r4,lr}
2101           	mov	r1,#1
4801           	ldr	r0,[00009730]                           ; [pc,#&4]
F000 BD78     	b	$0000A220
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
F000 FD41     	bl	$0000A1C8
2800           	cmps	r0,#0
D0F9           	beq	$0000973E

l0000974A:
4D0C           	ldr	r5,[0000977C]                           ; [pc,#&30]
4C0A           	ldr	r4,[00009778]                           ; [pc,#&28]
6828           	ldr	r0,[r5]
F7FF FFA0     	bl	$00009694
4631           	mov	r1,r6
4620           	mov	r0,r4
F000 FD70     	bl	$0000A23C
2105           	mov	r1,#5
4620           	mov	r0,r4
F000 FD5E     	bl	$0000A220

l00009764:
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD2E     	bl	$0000A1C8
2800           	cmps	r0,#0
D0F9           	beq	$00009764

l00009770:
6828           	ldr	r0,[r5]
E8BD 4070     	pop.w	{r4-r6,lr}
E78D           	b	$00009694
00009778                         00 00 02 40 7C 08 00 20         ...@|.. 

;; OSRAMClear: 00009780
;;   Called from:
;;     00008050 (in vPrintTask)
;;     0000995C (in OSRAMInit)
OSRAMClear proc
B510           	push	{r4,lr}
2080           	mov	r0,#&80
F7FF FF8A     	bl	$0000969C
2106           	mov	r1,#6
480E           	ldr	r0,[000097C4]                           ; [pc,#&38]
F7FF FF9A     	bl	$000096C4
245F           	mov	r4,#&5F

l00009792:
2000           	mov	r0,#0
F7FF FFB6     	bl	$00009704
3C01           	subs	r4,#1
D1FA           	bne	$00009792

l0000979C:
4620           	mov	r0,r4
F7FF FFCB     	bl	$00009738
2080           	mov	r0,#&80
F7FF FF7A     	bl	$0000969C
2106           	mov	r1,#6
4807           	ldr	r0,[000097C8]                           ; [pc,#&1C]
F7FF FF8A     	bl	$000096C4
245F           	mov	r4,#&5F

l000097B2:
2000           	mov	r0,#0
F7FF FFA6     	bl	$00009704
3C01           	subs	r4,#1
D1FA           	bne	$000097B2

l000097BC:
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
E7B9           	b	$00009738
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
F7FF FF61     	bl	$0000969C
2E00           	cmps	r6,#0
BF0C           	ite	eq
20B0           	moveq	r0,#&B0

l000097E0:
20B1           	mov	r0,#&B1
F7FF FF8F     	bl	$00009704
F104 0624     	add	r6,r4,#&24
2080           	mov	r0,#&80
F7FF FF8A     	bl	$00009704
F006 000F     	and	r0,r6,#&F
F7FF FF86     	bl	$00009704
2080           	mov	r0,#&80
F7FF FF83     	bl	$00009704
F3C6 1003     	ubfx	r0,r6,#4,#4
F040 0010     	orr	r0,r0,#&10
F7FF FF7D     	bl	$00009704
2040           	mov	r0,#&40
F7FF FF7A     	bl	$00009704
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
F7FF FF6E     	bl	$00009704
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
F7FF FF43     	bl	$000096C4
2C5A           	cmps	r4,#&5A
F04F 0000     	mov	r0,#0
D1EA           	bne	$0000981C

l00009846:
E8BD 4070     	pop.w	{r4-r6,lr}
E775           	b	$00009738

l0000984C:
3B20           	subs	r3,#&20
EB03 0383     	add.w	r3,r3,r3,lsl #2
F1C4 045F     	rsb	r4,r4,#&5F
18F0           	add	r0,r6,r3
4621           	mov	r1,r4
F7FF FF33     	bl	$000096C4
782B           	ldrb	r3,[r5]
4A06           	ldr	r2,[0000987C]                           ; [pc,#&18]
3B20           	subs	r3,#&20
EB03 0383     	add.w	r3,r3,r3,lsl #2
4413           	adds	r3,r2
4423           	adds	r3,r4
7C18           	ldrb	r0,[r3,#&10]
E8BD 4070     	pop.w	{r4-r6,lr}
E761           	b	$00009738

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
F7FF FEFA     	bl	$0000969C
2C00           	cmps	r4,#0
BF14           	ite	ne
20B1           	movne	r0,#&B1

l000098AE:
20B0           	mov	r0,#&B0
F7FF FF28     	bl	$00009704
2080           	mov	r0,#&80
F7FF FF25     	bl	$00009704
4638           	mov	r0,r7
F7FF FF22     	bl	$00009704
2080           	mov	r0,#&80
F7FF FF1F     	bl	$00009704
4640           	mov	r0,r8
F7FF FF1C     	bl	$00009704
2040           	mov	r0,#&40
F7FF FF19     	bl	$00009704
4628           	mov	r0,r5
4651           	mov	r1,r10
444D           	adds	r5,r9
F7FF FEF4     	bl	$000096C4
3401           	adds	r4,#1
F815 0C01     	ldrb.w	r0,[r5,-#&1]
F7FF FF29     	bl	$00009738
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
F000 F93F     	bl	$00009B7C
4818           	ldr	r0,[00009960]                           ; [pc,#&60]
F000 F93C     	bl	$00009B7C
210C           	mov	r1,#&C
4817           	ldr	r0,[00009964]                           ; [pc,#&5C]
F7FF FDBA     	bl	$00009480
4621           	mov	r1,r4
4816           	ldr	r0,[00009968]                           ; [pc,#&58]
F000 FBF0     	bl	$0000A0F4
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
F7FF FEB0     	bl	$0000969C
1CA8           	add	r0,r5,#2
1EA1           	sub	r1,r4,#2
4440           	adds	r0,r8
3401           	adds	r4,#1
F7FF FEBE     	bl	$000096C4
4425           	adds	r5,r4
4630           	mov	r0,r6
F7FF FEF4     	bl	$00009738
2D70           	cmps	r5,#&70
EB07 0305     	add.w	r3,r7,r5
D9E8           	bls	$0000992A

l00009958:
E8BD 41F0     	pop.w	{r4-r8,lr}
F7FF BF10     	b	$00009780
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
F7FF FE81     	bl	$0000969C
1CA8           	add	r0,r5,#2
1EA1           	sub	r1,r4,#2
4440           	adds	r0,r8
3401           	adds	r4,#1
F7FF FE8F     	bl	$000096C4
4425           	adds	r5,r4
4630           	mov	r0,r6
F7FF FEC5     	bl	$00009738
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
F7FF FE6A     	bl	$0000969C
20AE           	mov	r0,#&AE
F7FF FE9B     	bl	$00009704
2080           	mov	r0,#&80
F7FF FE98     	bl	$00009704
20AD           	mov	r0,#&AD
F7FF FE95     	bl	$00009704
2080           	mov	r0,#&80
F7FF FE92     	bl	$00009704
E8BD 4008     	pop.w	{r3,lr}
208A           	mov	r0,#&8A
E6A7           	b	$00009738

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
F000 F9FB     	bl	$00009DF0
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
F7FF FD58     	bl	$00009504
E8BD 4008     	pop.w	{r3,lr}
2017           	mov	r0,#&17
F7FF BDBF     	b	$000095DC
00009A5E                                           00 BF               ..

;; SSIIntUnregister: 00009A60
SSIIntUnregister proc
B508           	push	{r3,lr}
2017           	mov	r0,#&17
F7FF FDE8     	bl	$00009638
E8BD 4008     	pop.w	{r3,lr}
2017           	mov	r0,#&17
F7FF BD63     	b	$00009538
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
4203           	adcs	r3,r0
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
4218           	adcs	r0,r3
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
F7FF FC5D     	bl	$00009504
E8BD 4008     	pop.w	{r3,lr}
202C           	mov	r0,#&2C
F7FF BCC4     	b	$000095DC

;; SysCtlIntUnregister: 00009C54
SysCtlIntUnregister proc
B508           	push	{r3,lr}
202C           	mov	r0,#&2C
F7FF FCEE     	bl	$00009638
E8BD 4008     	pop.w	{r3,lr}
202C           	mov	r0,#&2C
F7FF BC69     	b	$00009538
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
F000 BA04     	b	$0000A0EC

;; SysCtlDeepSleep: 00009CE4
SysCtlDeepSleep proc
B510           	push	{r4,lr}
4C06           	ldr	r4,[00009D00]                           ; [pc,#&18]
6823           	ldr	r3,[r4]
F043 0304     	orr	r3,r3,#4
6023           	str	r3,[r4]
F000 F9FC     	bl	$0000A0EC
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
F7FF FF3D     	bl	$00009DF0
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
F7FF FF1A     	bl	$00009DF0
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
F7FF FA3D     	bl	$00009504
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F7FF BAA4     	b	$000095DC
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
F7FF FAC7     	bl	$00009638
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F7FF BA42     	b	$00009538
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
F7FF FE75     	bl	$00009DF0
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
F7FF F9BE     	bl	$00009504
E8BD 4008     	pop.w	{r3,lr}
2018           	mov	r0,#&18
F7FF BA25     	b	$000095DC
0000A192       00 BF                                       ..           

;; I2CIntUnregister: 0000A194
I2CIntUnregister proc
B508           	push	{r3,lr}
2018           	mov	r0,#&18
F7FF FA4E     	bl	$00009638
E8BD 4008     	pop.w	{r3,lr}
2018           	mov	r0,#&18
F7FF B9C9     	b	$00009538
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
;;; Segment .text.memcpy (0000A5C4)

;; memcpy: 0000A5C4
;;   Called from:
;;     0000010A (in prvCopyDataToQueue)
;;     0000012A (in prvCopyDataToQueue)
;;     00000188 (in prvCopyDataFromQueue)
;;     00008466 (in xQueueCRReceive)
;;     00008504 (in xQueueCRReceiveFromISR)
memcpy proc
B5F0           	push	{r4-r7,lr}
0005           	mov	r5,r0
2A0F           	cmps	r2,#&F
D92F           	bls	$0000A62C

l0000A5CC:
000B           	mov	r3,r1
4303           	orrs	r3,r0
079B           	lsls	r3,r3,#&1E
D136           	bne	$0000A642

l0000A5D4:
0016           	mov	r6,r2
000C           	mov	r4,r1
0003           	mov	r3,r0
3E10           	subs	r6,#&10
0935           	lsrs	r5,r6,#4
3501           	adds	r5,#1
012D           	lsls	r5,r5,#4
1945           	add	r5,r0,r5

l0000A5E4:
6827           	ldr	r7,[r4]
601F           	str	r7,[r3]
6867           	ldr	r7,[r4,#&4]
605F           	str	r7,[r3,#&4]
68A7           	ldr	r7,[r4,#&8]
609F           	str	r7,[r3,#&8]
68E7           	ldr	r7,[r4,#&C]
60DF           	str	r7,[r3,#&C]
3310           	adds	r3,#&10
3410           	adds	r4,#&10
429D           	cmps	r5,r3
D1F3           	bne	$0000A5E4

l0000A5FC:
230F           	mov	r3,#&F
439E           	bics	r6,r3
3610           	adds	r6,#&10
1985           	add	r5,r0,r6
1989           	add	r1,r1,r6
4013           	ands	r3,r2
2B03           	cmps	r3,#3
D91C           	bls	$0000A646

l0000A60C:
1F1E           	sub	r6,r3,#4
2300           	mov	r3,#0
08B4           	lsrs	r4,r6,#2
3401           	adds	r4,#1
00A4           	lsls	r4,r4,#2

l0000A616:
58CF           	ldr	r7,[r1,r3]
50EF           	str	r7,[r5,r3]
3304           	adds	r3,#4
42A3           	cmps	r3,r4
D1FA           	bne	$0000A616

l0000A620:
2403           	mov	r4,#3
43A6           	bics	r6,r4
1D33           	add	r3,r6,#4
4022           	ands	r2,r4
18C9           	add	r1,r1,r3
18ED           	add	r5,r5,r3

l0000A62C:
2A00           	cmps	r2,#0
D005           	beq	$0000A63C

l0000A630:
2300           	mov	r3,#0

l0000A632:
5CCC           	ldrb	r4,[r1,r3]
54EC           	strb	r4,[r5,r3]
3301           	adds	r3,#1
4293           	cmps	r3,r2
D1FA           	bne	$0000A632

l0000A63C:
BCF0           	pop	{r4-r7}
BC02           	pop	{r1}
4708           	bx	r1

l0000A642:
0005           	mov	r5,r0
E7F4           	b	$0000A630

l0000A646:
001A           	mov	r2,r3
E7F0           	b	$0000A62C
0000A64A                               C0 46                       .F   
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
