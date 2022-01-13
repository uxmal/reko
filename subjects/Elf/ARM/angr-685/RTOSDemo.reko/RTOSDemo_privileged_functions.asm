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
l00000A82	db	0x3A
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
4620           	mov	r0,r4
BDF8           	pop	{r3-r7,pc}

l00000A9A:
6E33           	ldr	r3,[r6,#&60]
2C01           	cmps	r4,#1
F103 0301     	add	r3,r3,#1
6633           	str	r3,[r6,#&60]
D1F4           	bne	$00000A90

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
4620           	mov	r0,r4
BDF8           	pop	{r3-r7,pc}

l00000AF6:
6E33           	ldr	r3,[r6,#&60]
431F           	orrs	r7,r3
6637           	str	r7,[r6,#&60]
E7C6           	b	$00000A8C

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
l00000B40	db	0x2A
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
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000B58:
6E03           	ldr	r3,[r0,#&60]
2C01           	cmps	r4,#1
F103 0301     	add	r3,r3,#1
6603           	str	r3,[r0,#&60]
D1F4           	bne	$00000B4E

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
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000B94:
6E03           	ldr	r3,[r0,#&60]
4319           	orrs	r1,r3
6601           	str	r1,[r0,#&60]
E7D6           	b	$00000B4A

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
