;;; Segment privileged_functions (00000000)
00000000 2C 02 00 20 09 80 00 00 01 80 00 00 05 80 00 00 ,.. ............
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000020 00 00 00 00 00 00 00 00 00 00 00 00 15 17 00 00 ................
00000030 00 00 00 00 00 00 00 00 89 16 00 00 E5 16 00 00 ................
00000040 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000050 00 00 00 00 09 81 00 00                         ........       

;; prvUnlockQueue: 00000058
prvUnlockQueue proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
F008 FA8C     	bl	$00008574
F895 4045     	ldrb	r4,[r5,#&45]
B264           	sxtb	r4,r4
2C00           	cmps	r4,#0
DD16           	ble	$0000007E

l0000006A:
6A6B           	ldr	r3,[r5,#&24]
B1A3           	cbz	r3,$00000098

l0000006E:
F105 0624     	add	r6,r5,#&24
E005           	b	$0000007C
00000074             01 3C E3 B2 5C B2 6B B1                 .<..\.k.   

l0000007C:
6A6B           	ldr	r3,[r5,#&24]

l0000007E:
B15B           	cbz	r3,$00000098

l00000080:
4630           	mov	r0,r6
F000 FFCB     	bl	$00001018
2800           	cmps	r0,#0
D0F4           	beq	$00000270

l0000008A:
3C01           	subs	r4,#1
F001 F88E     	bl	$000011A8
B2E3           	uxtb	r3,r4
B25C           	sxtb	r4,r3
2B00           	cmps	r3,#0
D1F1           	bne	$00000278

l00000098:
23FF           	mov	r3,#&FF
F885 3045     	strb	r3,[r5,#&45]
F008 FA87     	bl	$000085AC
F008 FA69     	bl	$00008574
F895 4044     	ldrb	r4,[r5,#&44]
B264           	sxtb	r4,r4
2C00           	cmps	r4,#0
DD16           	ble	$000000C4

l000000B0:
692B           	ldr	r3,[r5,#&10]
B1A3           	cbz	r3,$000000DE

l000000B4:
F105 0610     	add	r6,r5,#&10
E005           	b	$000000C2
000000BA                               01 3C E3 B2 5C B2           .<..\.
000000C0 6B B1                                           k.             

l000000C2:
692B           	ldr	r3,[r5,#&10]

l000000C4:
B15B           	cbz	r3,$000000DE

l000000C6:
4630           	mov	r0,r6
F000 FFA8     	bl	$00001018
2800           	cmps	r0,#0
D0F4           	beq	$000002B6

l000000D0:
3C01           	subs	r4,#1
F001 F86B     	bl	$000011A8
B2E3           	uxtb	r3,r4
B25C           	sxtb	r4,r3
2B00           	cmps	r3,#0
D1F1           	bne	$000002BE

l000000DE:
23FF           	mov	r3,#&FF
F885 3044     	strb	r3,[r5,#&44]
E8BD 4070     	pop.w	{r4-r6,lr}
F008 BA62     	b	$00C085AC

;; prvCopyDataToQueue: 000000EC
prvCopyDataToQueue proc
B570           	push	{r4-r6,lr}
4604           	mov	r4,r0
6C00           	ldr	r0,[r0,#&40]
6BA5           	ldr	r5,[r4,#&38]
B928           	cbnz	r0,$00000102

l000000F6:
6826           	ldr	r6,[r4]
2E00           	cmps	r6,#0

;; fn000000FA: 000000FA
fn000000FA proc
D031           	beq	$0000015C

l000000FC:
3501           	adds	r5,#1
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}

l00000102:
4616           	mov	r6,r2
4602           	mov	r2,r0
B97E           	cbnz	r6,$00000128

l00000108:
68A0           	ldr	r0,[r4,#&8]
F00A FA5B     	bl	$0000A5C0
68A3           	ldr	r3,[r4,#&8]
6C21           	ldr	r1,[r4,#&40]
6862           	ldr	r2,[r4,#&4]
440B           	adds	r3,r1
4293           	cmps	r3,r2
60A3           	str	r3,[r4,#&8]
D319           	blo	$0000014C

l0000011C:
6823           	ldr	r3,[r4]
3501           	adds	r5,#1
4630           	mov	r0,r6
60A3           	str	r3,[r4,#&8]
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}

l00000128:
68E0           	ldr	r0,[r4,#&C]
F00A FA4B     	bl	$0000A5C0
6C22           	ldr	r2,[r4,#&40]
68E3           	ldr	r3,[r4,#&C]
4252           	rsbs	r2,r2
6821           	ldr	r1,[r4]
4413           	adds	r3,r2
428B           	cmps	r3,r1
60E3           	str	r3,[r4,#&C]
D202           	bhs	$00000140

l0000013E:
6863           	ldr	r3,[r4,#&4]

l00000140:
441A           	adds	r2,r3
60E2           	str	r2,[r4,#&C]
2E02           	cmps	r6,#2
D007           	beq	$00000154

l00000148:
3501           	adds	r5,#1
2000           	mov	r0,#0

l0000014C:
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}
00000150 01 35 30 46                                     .50F           

l00000154:
63A5           	str	r5,[r4,#&38]
BD70           	pop	{r4-r6,pc}
00000158                         05 B9 01 25                     ...%   

l0000015C:
2000           	mov	r0,#0
E7CE           	b	$000000FA
00000160 60 68 01 F0 75 F8 01 35                         `h..u..5       

;; fn00000168: 00000168
fn00000168 proc
6066           	str	r6,[r4,#&4]
E7C8           	b	$000000FA

;; prvCopyDataFromQueue: 0000016C
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
F00A BA1C     	b	$00C0A5C0

;; fn0000018C: 0000018C
fn0000018C proc
4770           	bx	lr
0000018E                                           00 BF               ..

;; xQueueGenericSend: 00000190
xQueueGenericSend proc
E92D 47F0     	push.w	{r4-r10,lr}
2500           	mov	r5,#0
B084           	sub	sp,#&10
4604           	mov	r4,r0
468A           	mov	r10,r1
9201           	str	r2,[sp,#&4]
461F           	mov	r7,r3
46A8           	mov	r8,r5
F8DF 90FC     	ldr	r9,[pc,#&FC]                             ; 000002A6
E027           	b	$000001F4
000001A8                         08 F0 02 FA 00 F0 2E FC         ........
000001B0 08 F0 E2 F9 94 F8 44 30 FF 2B 08 BF 84 F8 44 80 ......D0.+....D.
000001C0 94 F8 45 30 FF 2B 08 BF 84 F8 45 80 08 F0 F0 F9 ..E0.+....E.....
000001D0 01 A9 02 A8 00 F0 C0 FF 00 28 50 D1 08 F0 CC F9 .........(P.....
000001E0 A2 6B E3 6B 9A 42 17 D0 08 F0 E2 F9 20 46 FF F7 .k.k.B...... F..
000001F0 33 FF 00 F0                                     3...           

l000001F4:
FE3B 2501     	Invalid
F008 F9BE     	bl	$00008574
6BA2           	ldr	r2,[r4,#&38]
6BE3           	ldr	r3,[r4,#&3C]
429A           	cmps	r2,r3
D320           	blo	$00000242
2F02           	cmps	r7,#2
D01E           	beq	$00000242
9E01           	ldr	r6,[sp,#&4]
B396           	cbz	r6,$00000272
2D00           	cmps	r5,#0
D1CB           	bne	$000003A4
A802           	add	r0,sp,#8
F000 FF97     	bl	$00001140
E7C7           	b	$000001A4
F008 F9CA     	bl	$000085AC
9901           	ldr	r1,[sp,#&4]
F104 0010     	add	r0,r4,#&10
F000 FEDB     	bl	$00000FD8
4620           	mov	r0,r4
F7FF FF16     	bl	$00000054
F000 FE1E     	bl	$00000E68
2800           	cmps	r0,#0
D1E0           	bne	$000003F2
F04F 5380     	mov	r3,#&10000000
F8C9 3000     	str	r3,[r9]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7D7           	b	$000001F2
463A           	mov	r2,r7
4651           	mov	r1,r10
4620           	mov	r0,r4
F7FF FF4E     	bl	$000000E8
6A63           	ldr	r3,[r4,#&24]
B9EB           	cbnz	r3,$00000290
B138           	cbz	r0,$00000266
F04F 5280     	mov	r2,#&10000000
4B11           	ldr	r3,[pc,#&44]                            ; 000002A6
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
F008 F9A3     	bl	$000085AC
2001           	mov	r0,#1
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l00000270:
87F0           	strh	r0,[r6,#&7C]
F008 F99D     	bl	$000085AC
4630           	mov	r0,r6

l00000278:
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}
0000027E                                           20 46                F
00000280 FF F7 EA FE 00 F0 F2 FD 00 20 04 B0 BD E8 F0 87 ......... ......
00000290 04 F1 24 00 00 F0 C2 FE 00 28 DC D1 E3 E7 00 BF ..$......(......

;; fn000002A0: 000002A0
fn000002A0 proc
ED04 E000     	Invalid

;; xQueuePeekFromISR: 000002A4
xQueuePeekFromISR proc
B570           	push	{r4-r6,lr}
F3EF 8511     	mrs	r5,cpsr
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy

;; fn000002B6: 000002B6
fn000002B6 proc
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
F7FF FF4F     	bl	$00000168
60E6           	str	r6,[r4,#&C]
2001           	mov	r0,#1
F385 8811     	msr	cpsr,r5

;; fn000002D4: 000002D4
fn000002D4 proc
8811           	ldrh	r1,[r2]
BD70           	pop	{r4-r6,pc}

;; xQueueGenericReceive: 000002D8
xQueueGenericReceive proc
E92D 47F0     	push.w	{r4-r10,lr}
2500           	mov	r5,#0
B084           	sub	sp,#&10
4604           	mov	r4,r0
468A           	mov	r10,r1
9201           	str	r2,[sp,#&4]
4699           	mov	r9,r3
462F           	mov	r7,r5
F8DF 8138     	ldr	r8,[pc,#&138]                            ; 0000042A
E00C           	b	$00000306
000002F0 08 F0 42 F9 A3 6B 00 2B 37 D0 08 F0 59 F9 20 46 ..B..k.+7...Y. F
00000300 FF F7 AA FE 00 F0                               ......         

l00000306:
FDB2 2501     	Invalid
F008 F935     	bl	$00008574
6BA6           	ldr	r6,[r4,#&38]
2E00           	cmps	r6,#0
D14D           	bne	$000003AC
9B01           	ldr	r3,[sp,#&4]
2B00           	cmps	r3,#0
D044           	beq	$000003A0
2D00           	cmps	r5,#0
D03E           	beq	$00000398
F008 F947     	bl	$000085AC
F000 FB73     	bl	$00000A08
F008 F927     	bl	$00008574
F894 3044     	ldrb	r3,[r4,#&44]
2BFF           	cmps	r3,#&FF
BF08           	it	eq
F884 7044     	strbeq	r7,[r4,#&44]
F894 3045     	ldrb	r3,[r4,#&45]
2BFF           	cmps	r3,#&FF
BF08           	it	eq
F884 7045     	strbeq	r7,[r4,#&45]
F008 F935     	bl	$000085AC
A901           	add	r1,sp,#4
A802           	add	r0,sp,#8
F000 FF05     	bl	$00001154
2800           	cmps	r0,#0
D0CE           	beq	$000004EC
4620           	mov	r0,r4
F7FF FE80     	bl	$00000054
F000 FD88     	bl	$00000E68
F008 F90C     	bl	$00008574
6BA3           	ldr	r3,[r4,#&38]
B1FB           	cbz	r3,$000003A4
F008 F924     	bl	$000085AC
E7CE           	b	$00000304
F008 F921     	bl	$000085AC
6823           	ldr	r3,[r4]
B393           	cbz	r3,$000003D8
9901           	ldr	r1,[sp,#&4]
F104 0024     	add	r0,r4,#&24
F000 FE30     	bl	$00000FD8
4620           	mov	r0,r4
F7FF FE6B     	bl	$00000054
F000 FD73     	bl	$00000E68
2800           	cmps	r0,#0
D1BE           	bne	$00000504
F04F 5380     	mov	r3,#&10000000
F8C8 3000     	str	r3,[r8]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7B5           	b	$00000304
A802           	add	r0,sp,#8
F000 FED1     	bl	$00001140
E7BC           	b	$0000031A
F008 F904     	bl	$000085AC
2000           	mov	r0,#0
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}
4651           	mov	r1,r10
4620           	mov	r0,r4
68E5           	ldr	r5,[r4,#&C]
F7FF FED9     	bl	$00000168
F1B9 0F00     	cmp	r9,#0
D113           	bne	$000003E4
6823           	ldr	r3,[r4]
3E01           	subs	r6,#1
63A6           	str	r6,[r4,#&38]
B34B           	cbz	r3,$0000041C
6923           	ldr	r3,[r4,#&10]
BB03           	cbnz	r3,$0000040E
F008 F8F0     	bl	$000085AC
2001           	mov	r0,#1
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}
F008 F8CE     	bl	$00008574
6860           	ldr	r0,[r4,#&4]
F000 FEED     	bl	$000011B8
F008 F8E5     	bl	$000085AC
E7C4           	b	$0000036E
6A63           	ldr	r3,[r4,#&24]
60E5           	str	r5,[r4,#&C]
2B00           	cmps	r3,#0
D0ED           	beq	$000005C8
F104 0024     	add	r0,r4,#&24
F000 FE12     	bl	$00001018
2800           	cmps	r0,#0
D0E7           	beq	$000005C8
F04F 5280     	mov	r2,#&10000000
4B08           	ldr	r3,[pc,#&20]                            ; 00000428
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7DE           	b	$000003C8
F104 0010     	add	r0,r4,#&10
F000 FE03     	bl	$00001018
2800           	cmps	r0,#0
D1F0           	bne	$000005F8
E7D7           	b	$000003C8
F000 FF5A     	bl	$000012D0
6060           	str	r0,[r4,#&4]
E7D1           	b	$000003C4

;; fn00000424: 00000424
fn00000424 proc
ED04 E000     	Invalid

;; uxQueueMessagesWaiting: 00000428
uxQueueMessagesWaiting proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F008 F8A4     	bl	$00008574
6BA4           	ldr	r4,[r4,#&38]
F008 F8BD     	bl	$000085AC
4620           	mov	r0,r4

;; fn00000438: 00000438
fn00000438 proc
BD10           	pop	{r4,pc}
0000043A                               00 BF                       ..   

;; uxQueueSpacesAvailable: 0000043C
uxQueueSpacesAvailable proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F008 F89A     	bl	$00008574
6BA8           	ldr	r0,[r5,#&38]
6BEC           	ldr	r4,[r5,#&3C]
1A24           	sub	r4,r4,r0
F008 F8B1     	bl	$000085AC
4620           	mov	r0,r4

;; fn00000450: 00000450
fn00000450 proc
BD38           	pop	{r3-r5,pc}
00000452       00 BF                                       ..           

;; vQueueDelete: 00000454
vQueueDelete proc
F001 B994     	b	$00C0177C

;; xQueueGenericSendFromISR: 00000458
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
D305           	blo	$0000047E

l00000476:
2B02           	cmps	r3,#2
D003           	beq	$0000047E

l0000047A:
2000           	mov	r0,#0
F386 8811     	msr	cpsr,r6

l0000047E:
8811           	ldrh	r1,[r2]

l00000480:
BDF8           	pop	{r3-r7,pc}
00000482       90 F8 45 40 17 46 64 B2 1A 46 05 46 FF F7   ..E@.Fd..F.F..
00000490 2D FE 63 1C 07 D0 01 34 64 B2 85 F8 45 40 01 20 -.c....4d...E@. 
000004A0 86 F3 11 88 F8 BD 6B 6A 00 2B F8 D0 05 F1 24 00 ......kj.+....$.
000004B0 00 F0 B4 FD 00 28 F2 D0 00 2F F0 D0 01 20 38 60 .....(.../... 8`
000004C0 DC E7 00 BF                                     ....           

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
D20E           	bhs	$000004FC

l000004E2:
F890 3045     	ldrb	r3,[r0,#&45]
3201           	adds	r2,#1
B25B           	sxtb	r3,r3
6382           	str	r2,[r0,#&38]
1C5A           	add	r2,r3,#1
D00B           	beq	$00000504

l000004F0:
3301           	adds	r3,#1
B25B           	sxtb	r3,r3
F880 3045     	strb	r3,[r0,#&45]
2001           	mov	r0,#1
F384 8811     	msr	cpsr,r4

l000004FC:
8811           	ldrh	r1,[r2]

l000004FE:
BD38           	pop	{r3-r5,pc}
00000500 00 20 84 F3                                     . ..           

l00000504:
8811           	ldrh	r1,[r2]
BD38           	pop	{r3-r5,pc}
00000508                         43 6A 00 2B F4 D0 24 30         Cj.+..$0
00000510 0D 46 00 F0 83 FD 00 28 EE D0 00 2D EC D0 01 20 .F.....(...-... 
00000520 28 60 EA E7                                     (`..           

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
F386 8811     	msr	cpsr,r6
E8BD 81F0     	pop.w	{r4-r8,pc}

l0000054A:
4607           	mov	r7,r0
F890 5044     	ldrb	r5,[r0,#&44]
4690           	mov	r8,r2
B26D           	sxtb	r5,r5
F7FF FE0A     	bl	$00000168
3C01           	subs	r4,#1
1C6B           	add	r3,r5,#1
63BC           	str	r4,[r7,#&38]
D008           	beq	$0000056E

l00000560:
3501           	adds	r5,#1
B26D           	sxtb	r5,r5
F887 5044     	strb	r5,[r7,#&44]
2001           	mov	r0,#1
F386 8811     	msr	cpsr,r6

l0000056E:
E8BD 81F0     	pop.w	{r4-r8,pc}
00000572       3B 69 00 2B F7 D0 07 F1 10 00 00 F0 4E FD   ;i.+........N.
00000580 00 28 F1 D0 B8 F1 00 0F EE D0 01 20 C8 F8 00 00 .(......... ....
00000590 D7 E7 00 BF                                     ....           

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
xQueueGetMutexHolder proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F007 FFDE     	bl	$00008574
6823           	ldr	r3,[r4]
B923           	cbnz	r3,$000005CA

l000005C0:
6864           	ldr	r4,[r4,#&4]
F007 FFF5     	bl	$000085AC
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

l000005CA:
2400           	mov	r4,#0
F007 FFF0     	bl	$000085AC

;; fn000005D0: 000005D0
fn000005D0 proc
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

;; xQueueTakeMutexRecursive: 000005D4
xQueueTakeMutexRecursive proc
B570           	push	{r4-r6,lr}
6845           	ldr	r5,[r0,#&4]
4604           	mov	r4,r0
460E           	mov	r6,r1
F000 FDAC     	bl	$00001134
4285           	cmps	r5,r0
D00A           	beq	$000005F6

l000005E4:
2300           	mov	r3,#0
4632           	mov	r2,r6
4619           	mov	r1,r3
4620           	mov	r0,r4
F7FF FE74     	bl	$000002D4
B110           	cbz	r0,$000005F8

l000005F2:
68E3           	ldr	r3,[r4,#&C]
3301           	adds	r3,#1

l000005F6:
60E3           	str	r3,[r4,#&C]

l000005F8:
BD70           	pop	{r4-r6,pc}
000005FA                               01 20 E3 68 03 44           . .h.D

;; fn00000600: 00000600
fn00000600 proc
60E3           	str	r3,[r4,#&C]
BD70           	pop	{r4-r6,pc}

;; xQueueGiveMutexRecursive: 00000604
xQueueGiveMutexRecursive proc
B538           	push	{r3-r5,lr}
6845           	ldr	r5,[r0,#&4]
4604           	mov	r4,r0
F000 FD95     	bl	$00001134
4285           	cmps	r5,r0
D001           	beq	$00000612

l00000612:
2000           	mov	r0,#0
BD38           	pop	{r3-r5,pc}
00000616                   E3 68 01 3B E3 60 0B B1 01 20       .h.;.`... 
00000620 38 BD 20 46 1A 46 19 46 FF F7 B2 FD             8. F.F.F....   

;; fn0000062C: 0000062C
fn0000062C proc
2001           	mov	r0,#1
BD38           	pop	{r3-r5,pc}

;; xQueueGenericReset: 00000630
xQueueGenericReset proc
B570           	push	{r4-r6,lr}
4604           	mov	r4,r0
460E           	mov	r6,r1
25FF           	mov	r5,#&FF
F007 FF9E     	bl	$00008574
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
F007 FFA4     	bl	$000085AC
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}

l0000066C:
F104 0010     	add	r0,r4,#&10
F000 FCD4     	bl	$00001018
2800           	cmps	r0,#0
D0F5           	beq	$00000860

l00000678:
F04F 5280     	mov	r2,#&10000000
4B0A           	ldr	r3,[pc,#&28]                            ; 000006AC
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
F007 FF92     	bl	$000085AC
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}

l00000690:
F104 0010     	add	r0,r4,#&10
F007 FE1C     	bl	$000082CC
F104 0024     	add	r0,r4,#&24
F007 FE18     	bl	$000082CC
F007 FF86     	bl	$000085AC
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}

;; fn000006A8: 000006A8
fn000006A8 proc
ED04 E000     	Invalid

;; xQueueGenericCreate: 000006AC
xQueueGenericCreate proc
B570           	push	{r4-r6,lr}
4606           	mov	r6,r0
FB00 F001     	mul	r0,r0,r1
3048           	adds	r0,#&48
460D           	mov	r5,r1
F001 F838     	bl	$00001728
4604           	mov	r4,r0
B148           	cbz	r0,$000006D4

l000006C0:
B155           	cbz	r5,$000006D8

l000006C2:
F100 0348     	add	r3,r0,#&48

l000006C4:
0348           	lsls	r0,r1,#&D

;; fn000006C6: 000006C6
fn000006C6 proc
6003           	str	r3,[r0]
63E6           	str	r6,[r4,#&3C]
6425           	str	r5,[r4,#&40]
2101           	mov	r1,#1
4620           	mov	r0,r4
F7FF FFAE     	bl	$0000062C

l000006D4:
4620           	mov	r0,r4
BD70           	pop	{r4-r6,pc}

;; fn000006D8: 000006D8
fn000006D8 proc
6020           	str	r0,[r4]
E7F5           	b	$000006C4

;; xQueueCreateMutex: 000006DC
xQueueCreateMutex proc
B510           	push	{r4,lr}
4602           	mov	r2,r0
2100           	mov	r1,#0
2001           	mov	r0,#1
F7FF FFE2     	bl	$000006A8
4604           	mov	r4,r0
B138           	cbz	r0,$000006FC

l000006EC:
2300           	mov	r3,#0
6043           	str	r3,[r0,#&4]
6003           	str	r3,[r0]
60C3           	str	r3,[r0,#&C]
461A           	mov	r2,r3
4619           	mov	r1,r3
F7FF FD4A     	bl	$0000018C

;; fn000006FC: 000006FC
fn000006FC proc
4620           	mov	r0,r4
BD10           	pop	{r4,pc}

;; prvInitialiseNewTask: 00000700
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
785E           	ldrb	r6,[r3,#&1]
F800 6B01     	strb	r6,[r0],#&1
F813 6F01     	ldrb	r6,[r3,#&1]!
B10E           	cbz	r6,$0000073E

l0000073A:
428B           	cmps	r3,r1
D1F7           	bne	$0000092A

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
F007 FDC7     	bl	$000082E4
F104 0038     	add	r0,r4,#&38
F007 FDC3     	bl	$000082E4
F1C7 0302     	rsb	r3,r7,#2
63A3           	str	r3,[r4,#&38]
6D22           	ldr	r2,[r4,#&50]
465B           	mov	r3,fp
990D           	ldr	r1,[sp,#&34]
1D20           	add	r0,r4,#4
6324           	str	r4,[r4,#&30]
6464           	str	r4,[r4,#&44]
F000 FEEE     	bl	$00001550
6626           	str	r6,[r4,#&60]
4653           	mov	r3,r10
F884 6064     	strb	r6,[r4,#&64]
464A           	mov	r2,r9
4641           	mov	r1,r8
4628           	mov	r0,r5
F000 FDF9     	bl	$00001378
9B0B           	ldr	r3,[sp,#&2C]
6020           	str	r0,[r4]
B103           	cbz	r3,$00000792

l00000790:
601C           	str	r4,[r3]

l00000792:
E8BD 8FF8     	pop.w	{r3-fp,pc}

;; fn00000794: 00000794
fn00000794 proc
8FF8           	ldrh	r0,[r7,#&7C]
00000796                   00 BF                               ..       

;; prvAddNewTaskToReadyList: 00000798
prvAddNewTaskToReadyList proc
E92D 41F0     	push.w	{r4-r8,lr}
4C2D           	ldr	r4,[pc,#&B4]                            ; 00000858
4605           	mov	r5,r0
F007 FEEA     	bl	$00008574
6823           	ldr	r3,[r4]
3301           	adds	r3,#1
6023           	str	r3,[r4]
6863           	ldr	r3,[r4,#&4]
2B00           	cmps	r3,#0
D030           	beq	$0000080E

l000007B0:
6F63           	ldr	r3,[r4,#&74]
B32B           	cbz	r3,$00000800

l000007B4:
6CE8           	ldr	r0,[r5,#&4C]

l000007B6:
F104 0608     	add	r6,r4,#8
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
F007 FD8B     	bl	$000082EC
F007 FEE9     	bl	$000085AC
6F63           	ldr	r3,[r4,#&74]
B163           	cbz	r3,$000007FC

l000007E2:
6862           	ldr	r2,[r4,#&4]
6CEB           	ldr	r3,[r5,#&4C]
6CD2           	ldr	r2,[r2,#&4C]
429A           	cmps	r2,r3
D207           	bhs	$000007F8

l000007EC:
F04F 5280     	mov	r2,#&10000000
4B19           	ldr	r3,[pc,#&64]                            ; 0000085C
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy

l000007F8:
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

l0000080E:
6065           	str	r5,[r4,#&4]
E7D3           	b	$000007B6
00000812       65 60 23 68 01 2B CC D1 04 F1 08 06 30 46   e`#h.+......0F
00000820 07 F0 56 FD 04 F1 30 08 04 F1 1C 00 07 F0 50 FD ..V...0.......P.
00000830 04 F1 44 07 40 46 07 F0 4B FD 38 46 07 F0 48 FD ..D.@F..K.8F..H.
00000840 04 F1 58 00 07 F0 44 FD C4 F8 6C 80 E8 6C 27 67 ..X...D...l..l'g
00000850 B3 E7 00 BF C4 00 00 20                         .......        

;; fn00000858: 00000858
fn00000858 proc
ED04 E000     	Invalid

;; prvAddCurrentTaskToDelayedList.isra.0: 0000085C
prvAddCurrentTaskToDelayedList.isra.0 proc
B570           	push	{r4-r6,lr}
4C14           	ldr	r4,[pc,#&50]                            ; 000008B6

;; fn00000860: 00000860
fn00000860 proc
4605           	mov	r5,r0
F8D4 6080     	ldr	r6,[r4,#&80]
6860           	ldr	r0,[r4,#&4]
3024           	adds	r0,#&24
F007 FD69     	bl	$0000833C
B938           	cbnz	r0,$00000880

;; fn00000870: 00000870
fn00000870 proc
2201           	mov	r2,#1
6861           	ldr	r1,[r4,#&4]
6FE3           	ldr	r3,[r4,#&7C]
6CC9           	ldr	r1,[r1,#&4C]
408A           	lsls	r2,r1
EA23 0302     	bic.w	r3,r3,r2
67E3           	str	r3,[r4,#&7C]

;; fn00000880: 00000880
fn00000880 proc
4435           	adds	r5,r6
6863           	ldr	r3,[r4,#&4]
42AE           	cmps	r6,r5
625D           	str	r5,[r3,#&24]
D80B           	bhi	$0000089E

l0000088A:
6EE0           	ldr	r0,[r4,#&6C]
6861           	ldr	r1,[r4,#&4]
3124           	adds	r1,#&24
F007 FD3C     	bl	$00008308
F8D4 3084     	ldr	r3,[r4,#&84]
429D           	cmps	r5,r3
BF38           	it	lo
F8C4 5084     	strlo	r5,[r4,#&84]

l0000089E:
5084           	str	r4,[r0,r2]

l000008A0:
BD70           	pop	{r4-r6,pc}
000008A2       20 6F 61 68 BD E8 70 40 24 31 07 F0 2E BD    oah..p@$1....

;; fn000008B0: 000008B0
fn000008B0 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskCreate: 000008B4
xTaskCreate proc
E92D 47F0     	push.w	{r4-r10,lr}
4680           	mov	r8,r0
B084           	sub	sp,#&10
0090           	lsls	r0,r2,#2
4616           	mov	r6,r2
4689           	mov	r9,r1
469A           	mov	r10,r3
F000 FF32     	bl	$00001728
B1E0           	cbz	r0,$00000904

l000008CA:
4605           	mov	r5,r0
2068           	mov	r0,#&68
F000 FF2D     	bl	$00001728
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
F7FF FF05     	bl	$000006FC
4620           	mov	r0,r4
F7FF FF4E     	bl	$00000794

;; fn000008FA: 000008FA
fn000008FA proc
FF4E 2001     	vhadd.u8	d18,d14,d1

l000008FC:
2001           	mov	r0,#1
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l00000904:
F04F 30FF     	mov	r0,#&FFFFFFFF
B004           	add	sp,#&10
E8BD 87F0     	pop.w	{r4-r10,pc}

l0000090E:
4628           	mov	r0,r5
F000 FF36     	bl	$0000177C
F04F 30FF     	mov	r0,#&FFFFFFFF

;; fn00000918: 00000918
fn00000918 proc
E7F1           	b	$000008FA
0000091A                               00 BF                       ..   

;; xTaskCreateRestricted: 0000091C
xTaskCreateRestricted proc
6943           	ldr	r3,[r0,#&14]
B323           	cbz	r3,$0000096A

l00000920:
B5F0           	push	{r4-r7,lr}
4604           	mov	r4,r0
B085           	sub	sp,#&14
2068           	mov	r0,#&68
460F           	mov	r7,r1

;; fn0000092A: 0000092A
fn0000092A proc
F000 FEFF     	bl	$00001728
4605           	mov	r5,r0
B1C0           	cbz	r0,$00000964

;; fn00000932: 00000932
fn00000932 proc
2601           	mov	r6,#1
6961           	ldr	r1,[r4,#&14]
F880 6065     	strb	r6,[r0,#&65]
68E3           	ldr	r3,[r4,#&C]
8922           	ldrh	r2,[r4,#&10]
F8D4 E010     	ldr	lr,[r4,#&10]
6501           	str	r1,[r0,#&50]
6861           	ldr	r1,[r4,#&4]
9002           	str	r0,[sp,#&8]
9701           	str	r7,[sp,#&4]
F854 0B18     	ldr	r0,[r4],#&18
F8CD E000     	str	lr,[sp]
9403           	str	r4,[sp,#&C]
F7FF FED4     	bl	$000006FC
4628           	mov	r0,r5
F7FF FF1D     	bl	$00000794

l0000095C:
FF1D 4630     	vmin.u16	d4,d13,d16
B005           	add	sp,#&14
BDF0           	pop	{r4-r7,pc}

;; fn00000964: 00000964
fn00000964 proc
F04F 30FF     	mov	r0,#&FFFFFFFF
E7FA           	b	$0000095C

l0000096A:
F04F 30FF     	mov	r0,#&FFFFFFFF

;; fn0000096C: 0000096C
fn0000096C proc
30FF           	adds	r0,#&FF

l0000096E:
4770           	bx	lr

;; vTaskAllocateMPURegions: 00000970
vTaskAllocateMPURegions proc
B120           	cbz	r0,$0000097C

l00000972:
2300           	mov	r3,#0
3004           	adds	r0,#4
461A           	mov	r2,r3
F000 BDEC     	b	$00C01550

l0000097C:
4B03           	ldr	r3,[pc,#&C]                             ; 00000990
6858           	ldr	r0,[r3,#&4]
2300           	mov	r3,#0
3004           	adds	r0,#4
461A           	mov	r2,r3
F000 BDE5     	b	$00C01550
0000098A                               00 BF                       ..   

;; fn0000098C: 0000098C
fn0000098C proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskStartScheduler: 00000990
vTaskStartScheduler proc
F04F 4300     	mov	r3,#&80000000
B510           	push	{r4,lr}
4C12           	ldr	r4,[pc,#&48]                            ; 000009E6
B082           	sub	sp,#8
9300           	str	r3,[sp]
F104 0388     	add	r3,r4,#&88
9301           	str	r3,[sp,#&4]
223B           	mov	r2,#&3B
2300           	mov	r3,#0
490F           	ldr	r1,[pc,#&3C]                            ; 000009EA
480F           	ldr	r0,[pc,#&3C]                            ; 000009EC
F7FF FF83     	bl	$000008B0
2801           	cmps	r0,#1
D001           	beq	$000009B2

l000009B2:
B002           	add	sp,#8
BD10           	pop	{r4,pc}
000009B6                   4F F0 BF 03 83 F3 11 88 BF F3       O.........
000009C0 6F 8F BF F3 4F 8F 4F F0 FF 32 00 23 C4 F8 84 20 o...O.O..2.#... 
000009D0 60 67 C4 F8 80 30 02 B0 BD E8 10 40 00 F0 E8 BC `g...0.....@....
000009E0 C4 00 00 20 7C A2 00 00 2D 85 00 00             ... |...-...   

;; vTaskEndScheduler: 000009EC
vTaskEndScheduler proc
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
2200           	mov	r2,#0
4B02           	ldr	r3,[pc,#&8]                             ; 00000A0E
675A           	str	r2,[r3,#&74]
F000 BDA5     	b	$00C0154C
00000A06                   00 BF                               ..       

;; fn00000A08: 00000A08
fn00000A08 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskSuspendAll: 00000A0C
vTaskSuspendAll proc
4A03           	ldr	r2,[pc,#&C]                             ; 00000A20
F8D2 308C     	ldr	r3,[r2,#&8C]
3301           	adds	r3,#1
F8C2 308C     	str	r3,[r2,#&8C]
4770           	bx	lr
00000A1A                               00 BF                       ..   

;; fn00000A1C: 00000A1C
fn00000A1C proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskGetTickCount: 00000A20
xTaskGetTickCount proc
4B01           	ldr	r3,[pc,#&4]                             ; 00000A2C
F8D3 0080     	ldr	r0,[r3,#&80]
4770           	bx	lr
00000A28                         C4 00 00 20                     ...    

;; xTaskGetTickCountFromISR: 00000A2C
xTaskGetTickCountFromISR proc
4B01           	ldr	r3,[pc,#&4]                             ; 00000A38
F8D3 0080     	ldr	r0,[r3,#&80]
4770           	bx	lr

;; fn00000A34: 00000A34
fn00000A34 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; uxTaskGetNumberOfTasks: 00000A38
uxTaskGetNumberOfTasks proc
4B01           	ldr	r3,[pc,#&4]                             ; 00000A44
6818           	ldr	r0,[r3]
4770           	bx	lr
00000A3E                                           00 BF               ..

;; fn00000A40: 00000A40
fn00000A40 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; pcTaskGetName: 00000A44
pcTaskGetName proc
B108           	cbz	r0,$00000A4A

l00000A46:
3054           	adds	r0,#&54
4770           	bx	lr

l00000A4A:
4B02           	ldr	r3,[pc,#&8]                             ; 00000A5A
6858           	ldr	r0,[r3,#&4]
3054           	adds	r0,#&54
4770           	bx	lr
00000A52       00 BF                                       ..           

;; fn00000A54: 00000A54
fn00000A54 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskGenericNotify: 00000A58
xTaskGenericNotify proc
B5F8           	push	{r3-r7,lr}
461C           	mov	r4,r3
4606           	mov	r6,r0
460F           	mov	r7,r1
4615           	mov	r5,r2
F007 FD89     	bl	$00008574
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
D806           	bhi	$00000A88

l00000A7E:
E8DF F002     	tbb	[pc,-r2]                                 ; 00000A86

l00000A82:
0C3A           	lsrs	r2,r7,#&10
l00000A83	db	0x0C
l00000A84	db	0x04
l00000A85	db	0x02

l00000A86:
2C02           	cmps	r4,#2

l00000A88:
D039           	beq	$00000AFA

l00000A8A:
6637           	str	r7,[r6,#&60]
2C01           	cmps	r4,#1
D00A           	beq	$00000AA2

l00000A90:
2401           	mov	r4,#1
F007 FD8D     	bl	$000085AC

l00000A96:
4620           	mov	r0,r4
BDF8           	pop	{r3-r7,pc}
00000A9A                               33 6E 01 2C 03 F1           3n.,..
00000AA0 01 03                                           ..             

l00000AA2:
6633           	str	r3,[r6,#&60]
D1F4           	bne	$00000C8C

l00000AA6:
F106 0724     	add	r7,r6,#&24
4D16           	ldr	r5,[pc,#&58]                            ; 00000B0A
4638           	mov	r0,r7
F007 FC47     	bl	$0000833C
6CF0           	ldr	r0,[r6,#&4C]
F8D5 E07C     	ldr	lr,[r5,#&7C]
F105 0208     	add	r2,r5,#8
FA04 F300     	lsl	r3,r4,r0
EB00 0080     	add.w	r0,r0,r0,lsl #2
EA43 030E     	orr	r3,r3,lr
EB02 0080     	add.w	r0,r2,r0,lsl #2
4639           	mov	r1,r7
67EB           	str	r3,[r5,#&7C]
F007 FC0E     	bl	$000082EC
686B           	ldr	r3,[r5,#&4]
6CF2           	ldr	r2,[r6,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
D9D8           	bls	$00000C8C

l00000ADE:
F04F 5280     	mov	r2,#&10000000
4B09           	ldr	r3,[pc,#&24]                            ; 00000B0E
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
F007 FD5F     	bl	$000085AC

l00000AF2:
4620           	mov	r0,r4
BDF8           	pop	{r3-r7,pc}
00000AF6                   33 6E 1F 43                         3n.C     

l00000AFA:
6637           	str	r7,[r6,#&60]
E7C6           	b	$00000A88
00000AFE                                           00 24               .$
00000B00 C7 E7 00 BF C4 00 00 20 04 ED 00 E0             ....... ....   

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
D806           	bhi	$00000B46

l00000B3C:
E8DF F002     	tbb	[pc,-r2]                                 ; 00000B44

l00000B40:
0C2A           	lsrs	r2,r5,#&10
l00000B41	db	0x0C
l00000B42	db	0x04
l00000B43	db	0x02

l00000B44:
2C02           	cmps	r4,#2

l00000B46:
D03D           	beq	$00000BC0

l00000B48:
6601           	str	r1,[r0,#&60]
2C01           	cmps	r4,#1

l00000B4C:
D00A           	beq	$00000B60

l00000B4E:
2001           	mov	r0,#1
F385 8811     	msr	cpsr,r5

l00000B54:
E8BD 81F0     	pop.w	{r4-r8,pc}
00000B58                         03 6E 01 2C 03 F1 01 03         .n.,....

l00000B60:
6603           	str	r3,[r0,#&60]
D1F4           	bne	$00000D4A

l00000B64:
4E1A           	ldr	r6,[pc,#&68]                            ; 00000BD4
4607           	mov	r7,r0
F8D6 308C     	ldr	r3,[r6,#&8C]
B1B3           	cbz	r3,$00000B9C

l00000B6E:
F100 0138     	add	r1,r0,#&38
F106 0058     	add	r0,r6,#&58

l00000B76:
F007 FBBB     	bl	$000082EC
6873           	ldr	r3,[r6,#&4]
6CFA           	ldr	r2,[r7,#&4C]
6CDB           	ldr	r3,[r3,#&4C]
429A           	cmps	r2,r3
D9E4           	bls	$00000D4A

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
F007 FBCD     	bl	$0000833C
6CF8           	ldr	r0,[r7,#&4C]
6FF2           	ldr	r2,[r6,#&7C]
4084           	lsls	r4,r0
F106 0308     	add	r3,r6,#8
EB00 0080     	add.w	r0,r0,r0,lsl #2
4314           	orrs	r4,r2
4641           	mov	r1,r8
EB03 0080     	add.w	r0,r3,r0,lsl #2
67F4           	str	r4,[r6,#&7C]
F007 FB97     	bl	$000082EC

l00000BC0:
FB97 E7DA     	Invalid

l00000BC2:
E7DA           	b	$00000B76
00000BC4             00 20 C3 E7                             . ..       

l00000BC8:
F8C6 0090     	str	r0,[r6,#&90]
E7C0           	b	$00000B4C
00000BCE                                           00 BF               ..

;; fn00000BD0: 00000BD0
fn00000BD0 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskNotifyWait: 00000BD4
xTaskNotifyWait proc
E92D 41F0     	push.w	{r4-r8,lr}
4C1F           	ldr	r4,[pc,#&7C]                            ; 00000C5C
4615           	mov	r5,r2
4680           	mov	r8,r0
460E           	mov	r6,r1
461F           	mov	r7,r3
F007 FCC9     	bl	$00008574
6862           	ldr	r2,[r4,#&4]
F892 2064     	ldrb	r2,[r2,#&64]
2A02           	cmps	r2,#2
D009           	beq	$00000C00

l00000BF0:
2001           	mov	r0,#1
6861           	ldr	r1,[r4,#&4]
6E0A           	ldr	r2,[r1,#&60]
EA22 0208     	bic.w	r2,r2,r8
660A           	str	r2,[r1,#&60]
6863           	ldr	r3,[r4,#&4]
F883 0064     	strb	r0,[r3,#&64]

l00000C00:
0064           	lsls	r4,r4,#1

l00000C02:
B9DF           	cbnz	r7,$00000C3C

l00000C04:
F007 FCD4     	bl	$000085AC
F007 FCB6     	bl	$00008574
B115           	cbz	r5,$00000C14

l00000C0E:
6863           	ldr	r3,[r4,#&4]
6E1B           	ldr	r3,[r3,#&60]
602B           	str	r3,[r5]

l00000C14:
6863           	ldr	r3,[r4,#&4]
F893 3064     	ldrb	r3,[r3,#&64]
2B01           	cmps	r3,#1
D01A           	beq	$00000C50

l00000C1E:
2501           	mov	r5,#1
6863           	ldr	r3,[r4,#&4]
6E19           	ldr	r1,[r3,#&60]
EA21 0106     	bic.w	r1,r1,r6
6619           	str	r1,[r3,#&60]
2200           	mov	r2,#0
6863           	ldr	r3,[r4,#&4]
F883 2064     	strb	r2,[r3,#&64]
F007 FCBD     	bl	$000085AC
4628           	mov	r0,r5
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000C3C:
4638           	mov	r0,r7
F7FF FE0D     	bl	$00000858
F04F 5280     	mov	r2,#&10000000
4B05           	ldr	r3,[pc,#&14]                            ; 00000C62
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l00000C50:
8F6F           	ldrh	r7,[r5,#&74]
E7D7           	b	$00000C00
00000C54             00 25 E8 E7 C4 00 00 20 04 ED 00 E0     .%..... ....

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

;; fn00000C8C: 00000C8C
fn00000C8C proc
D003           	beq	$00000C92

;; fn00000C8E: 00000C8E
fn00000C8E proc
F386 8811     	msr	cpsr,r6

l00000C92:
E8BD 83F8     	pop.w	{r3-r9,pc}
00000C96                   19 4F 88 46 D7 F8 8C 30 04 46       .O.F...0.F
00000CA0 A3 B1 00 F1 38 01 07 F1 58 00 07 F0 21 FB 7B 68 ....8...X...!.{h
00000CB0 E2 6C DB 6C 9A 42 EA D9 01 23 B8 F1 00 0F 19 D0 .l.l.B...#......
00000CC0 C8 F8 00 30 86 F3 11 88 BD E8 F8 83 00 F1 24 09 ...0..........$.
00000CD0 48 46 07 F0 35 FB E0 6C FA 6F 85 40 07 F1 08 03 HF..5..l.o.@....
00000CE0 00 EB 80 00 15 43 49 46 03 EB 80 00 FD 67 07 F0 .....CIF.....g..
00000CF0 FF FA DC E7 C7 F8 90 30 C9 E7 00 BF             .......0....   

;; fn00000CFC: 00000CFC
fn00000CFC proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; ulTaskNotifyTake: 00000D00
ulTaskNotifyTake proc
B570           	push	{r4-r6,lr}
4C18           	ldr	r4,[pc,#&60]                            ; 00000D6A
4606           	mov	r6,r0
460D           	mov	r5,r1
F007 FC36     	bl	$00008574
6863           	ldr	r3,[r4,#&4]
6E1B           	ldr	r3,[r3,#&60]
B923           	cbnz	r3,$00000D1C

l00000D12:
2201           	mov	r2,#1
6863           	ldr	r3,[r4,#&4]
F883 2064     	strb	r2,[r3,#&64]

l00000D18:
2064           	mov	r0,#&64
B9B5           	cbnz	r5,$00000D4A

l00000D1C:
F007 FC48     	bl	$000085AC
F007 FC2A     	bl	$00008574
6863           	ldr	r3,[r4,#&4]
6E1D           	ldr	r5,[r3,#&60]
B11D           	cbz	r5,$00000D32

l00000D2A:
B956           	cbnz	r6,$00000D42

l00000D2C:
6863           	ldr	r3,[r4,#&4]

l00000D2E:
1E6A           	sub	r2,r5,#1
661A           	str	r2,[r3,#&60]

l00000D32:
2200           	mov	r2,#0
6863           	ldr	r3,[r4,#&4]
F883 2064     	strb	r2,[r3,#&64]
F007 FC39     	bl	$000085AC
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}

l00000D42:
2200           	mov	r2,#0
6863           	ldr	r3,[r4,#&4]
661A           	str	r2,[r3,#&60]
E7F3           	b	$00000D2E

;; fn00000D4A: 00000D4A
fn00000D4A proc
4628           	mov	r0,r5
F7FF FD86     	bl	$00000858
F04F 5280     	mov	r2,#&10000000
4B04           	ldr	r3,[pc,#&10]                            ; 00000D6C
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
E7DC           	b	$00000D18
00000D62       00 BF C4 00 00 20                           .....        

;; fn00000D68: 00000D68
fn00000D68 proc
ED04 E000     	Invalid

;; xTaskIncrementTick: 00000D6C
xTaskIncrementTick proc
E92D 47F0     	push.w	{r4-r10,lr}
4C3C           	ldr	r4,[pc,#&F0]                            ; 00000E68
F8D4 308C     	ldr	r3,[r4,#&8C]
2B00           	cmps	r3,#0
D15E           	bne	$00000E34

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
D152           	bne	$00000E42

l00000DA0:
F04F 33FF     	mov	r3,#&FFFFFFFF
F8C4 3084     	str	r3,[r4,#&84]

l00000DA8:
F8D4 3084     	ldr	r3,[r4,#&84]
2600           	mov	r6,#0
429F           	cmps	r7,r3
D330           	blo	$00000E10

l00000DB2:
F04F 0901     	mov	r9,#1
F8DF 80B0     	ldr	r8,[pc,#&B0]                             ; 00000E6E
E023           	b	$00000E00
00000DBC                                     E3 6E DB 68             .n.h
00000DC0 DD 68 6B 6A 05 F1 24 0A 9F 42 48 D3 50 46 07 F0 .hkj..$..BH.PF..
00000DD0 B7 FA AB 6C 05 F1 38 00 0B B1 07 F0 B1 FA E8 6C ...l..8........l
00000DE0 E2 6F 09 FA 00 F3 00 EB 80 00 13 43 51 46 08 EB .o.........CQF..
00000DF0 80 00 E3 67 07 F0 7C FA 63 68 EA 6C DB 6C 9A 42 ...g..|.ch.l.l.B

l00000E00:
BF28           	it	hs
2601           	movhs	r6,#1

l00000E04:
6EE3           	ldr	r3,[r4,#&6C]
681B           	ldr	r3,[r3]
2B00           	cmps	r3,#0
D1D7           	bne	$00000FB8

l00000E0C:
F04F 33FF     	mov	r3,#&FFFFFFFF

l00000E10:
F8C4 3084     	str	r3,[r4,#&84]
6863           	ldr	r3,[r4,#&4]
6CDB           	ldr	r3,[r3,#&4C]
EB03 0383     	add.w	r3,r3,r3,lsl #2
EB04 0383     	add.w	r3,r4,r3,lsl #2
689B           	ldr	r3,[r3,#&8]
2B02           	cmps	r3,#2

l00000E24:
BF28           	it	hs
2601           	movhs	r6,#1

l00000E28:
F8D4 3090     	ldr	r3,[r4,#&90]
2B00           	cmps	r3,#0
BF18           	it	ne
2601           	movne	r6,#1

l00000E32:
4630           	mov	r0,r6

l00000E34:
E8BD 87F0     	pop.w	{r4-r10,pc}
00000E38                         D4 F8 98 30 00 26 01 33         ...0.&.3
00000E40 C4 F8                                           ..             

l00000E42:
3098           	adds	r0,#&98
E7F0           	b	$00000E24
00000E46                   E3 6E 00 26 DB 68 DB 68 5B 6A       .n.&.h.h[j
00000E50 C4 F8 84 30 D4 F8 84 30 9F 42 DB D3 A9 E7 C4 F8 ...0...0.B......
00000E60 84 30 D7 E7 C4 00 00 20                         .0.....        

;; fn00000E68: 00000E68
fn00000E68 proc
00CC           	lsls	r4,r1,#3
2000           	mov	r0,#0

;; xTaskResumeAll: 00000E6C
xTaskResumeAll proc
E92D 41F0     	push.w	{r4-r8,lr}
4C33           	ldr	r4,[pc,#&CC]                            ; 00000F44
F007 FB81     	bl	$00008574
F8D4 308C     	ldr	r3,[r4,#&8C]
3B01           	subs	r3,#1
F8C4 308C     	str	r3,[r4,#&8C]
F8D4 508C     	ldr	r5,[r4,#&8C]
2D00           	cmps	r5,#0
D14E           	bne	$00000F22

l00000E88:
6823           	ldr	r3,[r4]
2B00           	cmps	r3,#0
D04B           	beq	$00000F22

l00000E8E:
2601           	mov	r6,#1
F104 0708     	add	r7,r4,#8
E01E           	b	$00000ED0
00000E96                   63 6E DD 68 05 F1 24 08 05 F1       cn.h..$...
00000EA0 38 00 07 F0 4D FA 40 46 07 F0 4A FA E8 6C E2 6F 8...M.@F..J..l.o
00000EB0 06 FA 00 F3 00 EB 80 00 13 43 41 46 07 EB 80 00 .........CAF....
00000EC0 E3 67 07 F0 15 FA 63 68 EA 6C DB 6C 9A 42 28 BF .g....ch.l.l.B(.

l00000ED0:
F8C4 6090     	str	r6,[r4,#&90]
6DA3           	ldr	r3,[r4,#&58]
2B00           	cmps	r3,#0
D1DD           	bne	$00001092

l00000EDA:
B135           	cbz	r5,$00000EEA

l00000EDC:
6EE3           	ldr	r3,[r4,#&6C]
681B           	ldr	r3,[r3]
BB3B           	cbnz	r3,$00000F32

l00000EE2:
F04F 33FF     	mov	r3,#&FFFFFFFF

l00000EE6:
F8C4 3084     	str	r3,[r4,#&84]

l00000EEA:
F8D4 5098     	ldr	r5,[r4,#&98]
B14D           	cbz	r5,$00000F04

l00000EF0:
2601           	mov	r6,#1
F7FF FF3B     	bl	$00000D68
B108           	cbz	r0,$00000EFC

l00000EF8:
F8C4 6090     	str	r6,[r4,#&90]

l00000EFC:
3D01           	subs	r5,#1
D1F8           	bne	$000010EE

l00000F00:
F8C4 5098     	str	r5,[r4,#&98]

l00000F04:
F8D4 3090     	ldr	r3,[r4,#&90]
B16B           	cbz	r3,$00000F26

l00000F0A:
F04F 5280     	mov	r2,#&10000000
4B0D           	ldr	r3,[pc,#&34]                            ; 00000F4A
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
2401           	mov	r4,#1
F007 FB48     	bl	$000085AC
4620           	mov	r0,r4

l00000F22:
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000F26:
2400           	mov	r4,#0
F007 FB42     	bl	$000085AC
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l00000F32:
6EE3           	ldr	r3,[r4,#&6C]
68DB           	ldr	r3,[r3,#&C]
68DB           	ldr	r3,[r3,#&C]
6A5B           	ldr	r3,[r3,#&24]
F8C4 3084     	str	r3,[r4,#&84]
E7D4           	b	$00000EE6
00000F40 C4 00 00 20                                     ...            

;; fn00000F44: 00000F44
fn00000F44 proc
ED04 E000     	Invalid

;; vTaskDelay: 00000F48
vTaskDelay proc
B508           	push	{r3,lr}
B940           	cbnz	r0,$00000F5E

l00000F4C:
F04F 5280     	mov	r2,#&10000000
4B09           	ldr	r3,[pc,#&24]                            ; 00000F7C
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
BD08           	pop	{r3,pc}

l00000F5E:
4A07           	ldr	r2,[pc,#&1C]                            ; 00000F82
F8D2 308C     	ldr	r3,[r2,#&8C]
3301           	adds	r3,#1
F8C2 308C     	str	r3,[r2,#&8C]
F7FF FC77     	bl	$00000858
F7FF FF7D     	bl	$00000E68
2800           	cmps	r0,#0
D0EA           	beq	$00001148

l00000F76:
BD08           	pop	{r3,pc}
00000F78                         04 ED 00 E0                     ....   

;; fn00000F7C: 00000F7C
fn00000F7C proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskDelayUntil: 00000F80
vTaskDelayUntil proc
4A14           	ldr	r2,[pc,#&50]                            ; 00000FD8
B510           	push	{r4,lr}
F8D2 408C     	ldr	r4,[r2,#&8C]
6803           	ldr	r3,[r0]
3401           	adds	r4,#1
F8C2 408C     	str	r4,[r2,#&8C]
F8D2 2080     	ldr	r2,[r2,#&80]
4419           	adds	r1,r3
429A           	cmps	r2,r3
D20E           	bhs	$00000FB4

l00000F9A:
428B           	cmps	r3,r1
D80E           	bhi	$00000FB8

l00000F9E:
6001           	str	r1,[r0]
F7FF FF64     	bl	$00000E68
B9A0           	cbnz	r0,$00000FD0

l00000FA6:
F04F 5280     	mov	r2,#&10000000
4B0B           	ldr	r3,[pc,#&2C]                            ; 00000FDE
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l00000FB4:
8F6F           	ldrh	r7,[r5,#&74]

l00000FB6:
BD10           	pop	{r4,pc}

;; fn00000FB8: 00000FB8
fn00000FB8 proc
428B           	cmps	r3,r1
D801           	bhi	$00000FBC

l00000FBC:
428A           	cmps	r2,r1
D2EE           	bhs	$0000119A

l00000FC0:
6001           	str	r1,[r0]
1A88           	sub	r0,r1,r2
F7FF FC4A     	bl	$00000858
F7FF FF50     	bl	$00000E68
2800           	cmps	r0,#0
D0EA           	beq	$000011A2

l00000FD0:
BD10           	pop	{r4,pc}
00000FD2       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; vTaskPlaceOnEventList: 00000FDC
vTaskPlaceOnEventList proc
B510           	push	{r4,lr}
460C           	mov	r4,r1
4B04           	ldr	r3,[pc,#&10]                            ; 00000FF8
6859           	ldr	r1,[r3,#&4]
3138           	adds	r1,#&38
F007 F991     	bl	$00008308
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
E434           	b	$00000858
00000FF2       00 BF                                       ..           

;; fn00000FF4: 00000FF4
fn00000FF4 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskPlaceOnUnorderedEventList: 00000FF8
vTaskPlaceOnUnorderedEventList proc
B538           	push	{r3-r5,lr}
4614           	mov	r4,r2
4B06           	ldr	r3,[pc,#&18]                            ; 0000101C
F041 4100     	orr	r1,r1,#&80000000
685D           	ldr	r5,[r3,#&4]
685B           	ldr	r3,[r3,#&4]
63A9           	str	r1,[r5,#&38]
F103 0138     	add	r1,r3,#&38
F007 F970     	bl	$000082EC
4620           	mov	r0,r4
E8BD 4038     	pop.w	{r3-r5,lr}
E421           	b	$00000858

;; fn00001018: 00001018
fn00001018 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskRemoveFromEventList: 0000101C
xTaskRemoveFromEventList proc
B5F8           	push	{r3-r7,lr}
68C3           	ldr	r3,[r0,#&C]
4C16           	ldr	r4,[pc,#&58]                            ; 00001080
68DD           	ldr	r5,[r3,#&C]
F105 0638     	add	r6,r5,#&38
4630           	mov	r0,r6
F007 F989     	bl	$0000833C
F8D4 308C     	ldr	r3,[r4,#&8C]
B9EB           	cbnz	r3,$00001070

l00001034:
F105 0624     	add	r6,r5,#&24
4630           	mov	r0,r6
F007 F981     	bl	$0000833C
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

l00001058:
F007 F94A     	bl	$000082EC
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
F007 F93B     	bl	$000082EC
E7EF           	b	$00001058

;; fn0000107C: 0000107C
fn0000107C proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskRemoveFromUnorderedEventList: 00001080
xTaskRemoveFromUnorderedEventList proc
B5F8           	push	{r3-r7,lr}
2501           	mov	r5,#1
68C6           	ldr	r6,[r0,#&C]
F041 4100     	orr	r1,r1,#&80000000
6001           	str	r1,[r0]
F106 0724     	add	r7,r6,#&24
F007 F956     	bl	$0000833C

;; fn00001092: 00001092
fn00001092 proc
F956 4C0F     	Invalid
4638           	mov	r0,r7
F007 F952     	bl	$0000833C
6CF3           	ldr	r3,[r6,#&4C]
F8D4 E07C     	ldr	lr,[r4,#&7C]
FA05 F203     	lsl	r2,r5,r3
F104 0008     	add	r0,r4,#8
EB03 0383     	add.w	r3,r3,r3,lsl #2
EB00 0083     	add.w	r0,r0,r3,lsl #2
EA42 020E     	orr	r2,r2,lr
4639           	mov	r1,r7
67E2           	str	r2,[r4,#&7C]
F007 F919     	bl	$000082EC
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
000010D2       00 BF                                       ..           

;; fn000010D4: 000010D4
fn000010D4 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskSwitchContext: 000010D8
vTaskSwitchContext proc
4A10           	ldr	r2,[pc,#&40]                            ; 00001120
F8D2 308C     	ldr	r3,[r2,#&8C]
B9C3           	cbnz	r3,$00001112

l000010E0:
F8C2 3090     	str	r3,[r2,#&90]
6FD3           	ldr	r3,[r2,#&7C]
FAB3 F383     	clz	r3,r3
B2DB           	uxtb	r3,r3
F1C3 031F     	rsb	r3,r3,#&1F

;; fn000010EE: 000010EE
fn000010EE proc
031F           	lsls	r7,r3,#&C
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

;; fn00001108: 00001108
fn00001108 proc
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
0000111A                               00 BF                       ..   

;; fn0000111C: 0000111C
fn0000111C proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; uxTaskResetEventItemValue: 00001120
uxTaskResetEventItemValue proc
4B04           	ldr	r3,[pc,#&10]                            ; 00001138
6859           	ldr	r1,[r3,#&4]
685A           	ldr	r2,[r3,#&4]
685B           	ldr	r3,[r3,#&4]
6B88           	ldr	r0,[r1,#&38]
6CDB           	ldr	r3,[r3,#&4C]
F1C3 0302     	rsb	r3,r3,#2
6393           	str	r3,[r2,#&38]
4770           	bx	lr

;; fn00001134: 00001134
fn00001134 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskGetCurrentTaskHandle: 00001138
xTaskGetCurrentTaskHandle proc
4B01           	ldr	r3,[pc,#&4]                             ; 00001144
6858           	ldr	r0,[r3,#&4]
4770           	bx	lr
0000113E                                           00 BF               ..

;; fn00001140: 00001140
fn00001140 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskSetTimeOutState: 00001144
vTaskSetTimeOutState proc
4B03           	ldr	r3,[pc,#&C]                             ; 00001158
F8D3 2094     	ldr	r2,[r3,#&94]

l00001148:
2094           	mov	r0,#&94
F8D3 3080     	ldr	r3,[r3,#&80]
E880 000C     	stm	r0,{r2-r3}
4770           	bx	lr

;; fn00001154: 00001154
fn00001154 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xTaskCheckForTimeOut: 00001158
xTaskCheckForTimeOut proc
B570           	push	{r4-r6,lr}
4604           	mov	r4,r0
460E           	mov	r6,r1
F007 FA0B     	bl	$00008574
4B11           	ldr	r3,[pc,#&44]                            ; 000011AE
6821           	ldr	r1,[r4]
F8D3 5080     	ldr	r5,[r3,#&80]
F8D3 2094     	ldr	r2,[r3,#&94]
6860           	ldr	r0,[r4,#&4]
4291           	cmps	r1,r2
D001           	beq	$00001174

l00001174:
4285           	cmps	r5,r0
D211           	bhs	$00001198

l00001178:
6832           	ldr	r2,[r6]
1A29           	sub	r1,r5,r0
4291           	cmps	r1,r2
D20D           	bhs	$00001198

l00001180:
1B52           	sub	r2,r2,r5
2500           	mov	r5,#0
F8D3 1094     	ldr	r1,[r3,#&94]
F8D3 3080     	ldr	r3,[r3,#&80]
4402           	adds	r2,r0
6032           	str	r2,[r6]
E884 000A     	stm	r4,{r1,r3}
F007 FA0C     	bl	$000085AC

l00001198:
4628           	mov	r0,r5

l0000119A:
BD70           	pop	{r4-r6,pc}
0000119C                                     01 25 07 F0             .%..
000011A0 07 FA                                           ..             

l000011A2:
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}
000011A6                   00 BF                               ..       

;; fn000011A8: 000011A8
fn000011A8 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; vTaskMissedYield: 000011AC
vTaskMissedYield proc
2201           	mov	r2,#1
4B02           	ldr	r3,[pc,#&8]                             ; 000011BE
F8C3 2090     	str	r2,[r3,#&90]
4770           	bx	lr
000011B6                   00 BF C4 00 00 20                   .....    

;; vTaskPriorityInherit: 000011BC
vTaskPriorityInherit proc
2800           	cmps	r0,#0
D042           	beq	$00001242

l000011C0:
B5F8           	push	{r3-r7,lr}
4C21           	ldr	r4,[pc,#&84]                            ; 0000124E
6CC3           	ldr	r3,[r0,#&4C]
6862           	ldr	r2,[r4,#&4]
6CD2           	ldr	r2,[r2,#&4C]
4293           	cmps	r3,r2
D212           	bhs	$000011F0

l000011CE:
6B82           	ldr	r2,[r0,#&38]
2A00           	cmps	r2,#0
DB04           	blt	$000011DA

l000011D4:
6862           	ldr	r2,[r4,#&4]
6CD2           	ldr	r2,[r2,#&4C]
F1C2 0202     	rsb	r2,r2,#2

l000011DA:
0202           	lsls	r2,r0,#8

l000011DC:
6382           	str	r2,[r0,#&38]
4D1B           	ldr	r5,[pc,#&6C]                            ; 00001252
EB03 0383     	add.w	r3,r3,r3,lsl #2
6B42           	ldr	r2,[r0,#&34]
EB05 0383     	add.w	r3,r5,r3,lsl #2
429A           	cmps	r2,r3
D003           	beq	$000011F2

l000011EE:
6863           	ldr	r3,[r4,#&4]

l000011F0:
6CDB           	ldr	r3,[r3,#&4C]

l000011F2:
64C3           	str	r3,[r0,#&4C]
BDF8           	pop	{r3-r7,pc}
000011F6                   00 F1 24 07 06 46 38 46 07 F0       ..$..F8F..
00001200 9F F8 68 B9 F2 6C 02 EB 82 03 04 EB 83 03 9B 68 ..h..l.........h
00001210 33 B9 01 21 E3 6F 01 FA 02 F2 23 EA 02 02 E2 67 3..!.o....#....g
00001220 01 23 62 68 D4 F8 7C E0 D2 6C 39 46 93 40 43 EA .#bh..|..l9F.@C.
00001230 0E 03 02 EB 82 00 F2 64 05 EB 80 00 E3 67 BD E8 .......d.....g..
00001240 F8 40                                           .@             

l00001242:
F007 B855     	b	$00C082EC
00001246                   70 47 C4 00 00 20 CC 00 00 20       pG... ... 

;; xTaskPriorityDisinherit: 00001250
xTaskPriorityDisinherit proc
2800           	cmps	r0,#0
D039           	beq	$000012C4

l00001254:
B5F8           	push	{r3-r7,lr}
6CC1           	ldr	r1,[r0,#&4C]
6DC3           	ldr	r3,[r0,#&5C]
6D82           	ldr	r2,[r0,#&58]
3B01           	subs	r3,#1
4291           	cmps	r1,r2
65C3           	str	r3,[r0,#&5C]

l00001262:
D000           	beq	$00001262

l00001264:
B10B           	cbz	r3,$0000126A

l00001266:
2000           	mov	r0,#0
BDF8           	pop	{r3-r7,pc}

l0000126A:
F100 0724     	add	r7,r0,#&24
4604           	mov	r4,r0
4638           	mov	r0,r7
F007 F865     	bl	$0000833C
B978           	cbnz	r0,$00001298

l00001278:
6CE1           	ldr	r1,[r4,#&4C]
4A14           	ldr	r2,[pc,#&50]                            ; 000012D2
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

l00001296:
E000           	b	$00001296

l00001298:
4A0C           	ldr	r2,[pc,#&30]                            ; 000012D0

l0000129A:
2501           	mov	r5,#1
6DA3           	ldr	r3,[r4,#&58]
F8D2 E07C     	ldr	lr,[r2,#&7C]
480B           	ldr	r0,[pc,#&2C]                            ; 000012D6
FA05 F603     	lsl	r6,r5,r3
4639           	mov	r1,r7
64E3           	str	r3,[r4,#&4C]
F1C3 0702     	rsb	r7,r3,#2
EB03 0383     	add.w	r3,r3,r3,lsl #2
EA46 060E     	orr	r6,r6,lr
EB00 0083     	add.w	r0,r0,r3,lsl #2
63A7           	str	r7,[r4,#&38]
67D6           	str	r6,[r2,#&7C]
F007 F816     	bl	$000082EC

l000012C4:
4628           	mov	r0,r5
BDF8           	pop	{r3-r7,pc}
000012C8                         00 20 70 47 C4 00 00 20         . pG... 
000012D0 CC 00 00 20                                     ...            

;; pvTaskIncrementMutexHeldCount: 000012D4
pvTaskIncrementMutexHeldCount proc
4B04           	ldr	r3,[pc,#&10]                            ; 000012EC
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
000012E6                   00 BF C4 00 00 20                   .....    

;; fn000012EC: 000012EC
fn000012EC proc
0000           	mov	r0,r0
0000           	mov	r0,r0

;; prvRestoreContextOfFirstTask: 000012F0
prvRestoreContextOfFirstTask proc
F8DF 0430     	ldr	r0,[pc,#&430]                            ; 00001728
6800           	ldr	r0,[r0]
6800           	ldr	r0,[r0]
F380 8808     	msr	cpsr,r0
4B0C           	ldr	r3,[pc,#&30]                            ; 00001334
6819           	ldr	r1,[r3]
6808           	ldr	r0,[r1]
F101 0104     	add	r1,r1,#4
F8DF 2420     	ldr	r2,[pc,#&420]                            ; 0000172E
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

;; fn00001330: 00001330
fn00001330 proc
00C8           	lsls	r0,r1,#3
2000           	mov	r0,#0

;; prvSVCHandler: 00001334
prvSVCHandler proc
6983           	ldr	r3,[r0,#&18]
F813 3C02     	ldrb.w	r3,[r3,-#&2]
2B01           	cmps	r3,#1
D010           	beq	$0000135C

l0000133E:
D309           	blo	$00001350

l00001340:
2B02           	cmps	r3,#2
D106           	bne	$0000134E

l00001344:
F3EF 8114     	mrs	r1,cpsr
F021 0101     	bic	r1,r1,#1
F381 8814     	msr	cpsr,r1

l0000134E:
8814           	ldrh	r4,[r2]

l00001350:
4770           	bx	lr
00001352       70 47 07 4A 13 68 43 F0 3E 43               pG.J.hC.>C   

l0000135C:
6013           	str	r3,[r2]
E7C7           	b	$000012EC
00001360 4F F0 80 52 04 4B 1A 60 BF F3 4F 8F BF F3 6F 8F O..R.K.`..O...o.
00001370 70 47 00 BF 1C ED 00 E0                         pG......       

;; fn00001378: 00001378
fn00001378 proc
ED04 E000     	Invalid

;; pxPortInitialiseStack: 0000137C
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
xPortStartScheduler proc
4B4D           	ldr	r3,[pc,#&134]                           ; 000014EC
B470           	push	{r4-r6}
681A           	ldr	r2,[r3]
494D           	ldr	r1,[pc,#&134]                           ; 000014F2
F442 027F     	orr	r2,r2,#&FF0000
601A           	str	r2,[r3]
681A           	ldr	r2,[r3]
F042 427F     	orr	r2,r2,#&FF000000
601A           	str	r2,[r3]
680B           	ldr	r3,[r1]
F5B3 6F00     	cmp	r3,#&800
D018           	beq	$000013FC

l000013CE:
F644 651F     	mov	r5,#&4E1F
2107           	mov	r1,#7
2000           	mov	r0,#0
4C46           	ldr	r4,[pc,#&118]                           ; 000014F6
4A46           	ldr	r2,[pc,#&118]                           ; 000014F8
4B47           	ldr	r3,[pc,#&11C]                           ; 000014FE
6025           	str	r5,[r4]
6011           	str	r1,[r2]
6018           	str	r0,[r3]
48D0           	ldr	r0,[pc,#&340]                           ; 0000172A
6800           	ldr	r0,[r0]
6800           	ldr	r0,[r0]
F380 8808     	msr	cpsr,r0
B662           	cps	#0
B661           	cps	#0
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy
DF00           	svc	#0
BF00           	nop

l000013FC:
BC70           	pop	{r4-r6}
4770           	bx	lr
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
4770           	bx	lr
00001552       00 BF                                       ..           

;; vPortStoreTaskMPUSettings: 00001554
vPortStoreTaskMPUSettings proc
B430           	push	{r4-r5}
2900           	cmps	r1,#0

l00001558:
D041           	beq	$000015DA

l0000155A:
BB4B           	cbnz	r3,$000015B0

l0000155C:
2505           	mov	r5,#5
684C           	ldr	r4,[r1,#&4]
B1FC           	cbz	r4,$000015A2

l00001562:
680B           	ldr	r3,[r1]
F045 0210     	orr	r2,r5,#&10
4313           	orrs	r3,r2
2C20           	cmps	r4,#&20
6083           	str	r3,[r0,#&8]
D96F           	bls	$0000164C

l00001570:
2240           	mov	r2,#&40
2305           	mov	r3,#5
E002           	b	$00001578
00001576                   01 33                               .3       

l00001578:
2B1F           	cmps	r3,#&1F
D017           	beq	$000015A8

l0000157C:
4294           	cmps	r4,r2
EA4F 0242     	mov.w	r2,r2,lsl #1

l00001582:
D8F8           	bhi	$00001772

l00001584:
005B           	lsls	r3,r3,#1
688A           	ldr	r2,[r1,#&8]
F042 0201     	orr	r2,r2,#1

l0000158C:
4313           	orrs	r3,r2
60C3           	str	r3,[r0,#&C]
3501           	adds	r5,#1
2D08           	cmps	r5,#8
F101 010C     	add	r1,r1,#&C
F100 0008     	add	r0,r0,#8
D1DF           	bne	$0000175A

l0000159E:
BC30           	pop	{r4-r5}
4770           	bx	lr

l000015A2:
F045 0310     	orr	r3,r5,#&10
60C4           	str	r4,[r0,#&C]

l000015A8:
6083           	str	r3,[r0,#&8]
E7F1           	b	$0000158C
000015AC                                     3E 23 EA E7             >#..

l000015B0:
009B           	lsls	r3,r3,#2
F042 0214     	orr	r2,r2,#&14
2B20           	cmps	r3,#&20
6002           	str	r2,[r0]
D94B           	bls	$00001650

l000015BC:
2240           	mov	r2,#&40
2405           	mov	r4,#5
E002           	b	$000015C4
000015C2       01 34                                       .4           

l000015C4:
2C1F           	cmps	r4,#&1F
D008           	beq	$000015D6

l000015C8:
4293           	cmps	r3,r2
EA4F 0242     	mov.w	r2,r2,lsl #1
D8F8           	bhi	$000017BE

l000015D0:
4B23           	ldr	r3,[pc,#&8C]                            ; 00001664

l000015D2:
EA43 0444     	orr	r4,r3,r4,lsl #1

l000015D6:
6044           	str	r4,[r0,#&4]
E7C0           	b	$00001558

l000015DA:
4C22           	ldr	r4,[pc,#&88]                            ; 0000166A
E7FB           	b	$000015D2
000015DE                                           22 4B               "K
000015E0 22 49 43 F0 14 02 C9 1A 20 29 02 60 36 D9 40 23 "IC..... ).`6.@#
000015F0 05 22 02 E0 01 32 1F 2A 26 D0 8B 42 4F EA 43 03 ."...2.*&..BO.C.
00001600 F8 D3 17 4B 43 EA 42 02 19 4B 1A 49 43 F0 15 04 ...KC.B..K.IC...
00001610 C9 1A 20 29 42 60 84 60 1E D9 05 22 40 23 02 E0 .. )B`.`..."@#..
00001620 01 32 1F 2A 12 D0 99 42 4F EA 43 03 F8 D8 12 4B .2.*...BO.C....K

l00001630:
EA43 0242     	orr	r2,r3,r2,lsl #1
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
00001648                         06 4A DD E7                     .J..   

l0000164C:
4A0B           	ldr	r2,[pc,#&2C]                            ; 00001680
E7F1           	b	$00001630

l00001650:
2308           	mov	r3,#8
E798           	b	$00001582
00001654             0A 4C BE E7 0A 4A EB E7 08 4A D3 E7     .L...J...J..
00001660 01 00 07 03 3F 00 07 03 00 00 00 20 00 20 00 20 ....?...... . . 
00001670 00 00 00 20 00 02 00 20 01 00 07 01 3F 00 07 01 ... ... ....?...
00001680 09 00 07 03 09 00 07 01                         ........       

;; xPortPendSVHandler: 00001688
xPortPendSVHandler proc
F3EF 8009     	mrs	r0,cpsr
4B14           	ldr	r3,[pc,#&50]                            ; 000016E4
681A           	ldr	r2,[r3]
F3EF 8114     	mrs	r1,cpsr
E920 0FF2     	stmdb	r0!,{r1,r4-fp}
6010           	str	r0,[r2]
E92D 4008     	push.w	{r3,lr}
F04F 00BF     	mov	r0,#&BF
F380 8811     	msr	cpsr,r0
F7FF FD17     	bl	$000010D4
F04F 0000     	mov	r0,#0
F380 8811     	msr	cpsr,r0
E8BD 4008     	pop.w	{r3,lr}
6819           	ldr	r1,[r3]
6808           	ldr	r0,[r1]
F101 0104     	add	r1,r1,#4
4A1A           	ldr	r2,[pc,#&68]                            ; 0000172E
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
F7FF FB37     	bl	$00000D68
B118           	cbz	r0,$00001708

l00001700:
F04F 5280     	mov	r2,#&10000000
4B02           	ldr	r3,[pc,#&8]                             ; 00001714
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
E607           	b	$00001330
00001724             08 ED 00 E0                             ....       

;; fn00001728: 00001728
fn00001728 proc
ED9C E000     	Invalid

;; pvPortMalloc: 0000172C
pvPortMalloc proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
0743           	lsls	r3,r0,#&1D
BF1C           	itt	ne
F020 0407     	bicne	r4,r0,#7

l00001738:
3408           	adds	r4,#8
F7FF F967     	bl	$00000A08
4B0F           	ldr	r3,[pc,#&3C]                            ; 00001782

;; fn00001740: 00001740
fn00001740 proc
681A           	ldr	r2,[r3]
B1AA           	cbz	r2,$00001770

l00001744:
F240 51B3     	mov	r1,#&5B3
F8D3 25C0     	ldr	r2,[r3,#&5C0]
4414           	adds	r4,r2
428C           	cmps	r4,r1
D809           	bhi	$00001762

l00001752:
42A2           	cmps	r2,r4
D207           	bhs	$00001762

l00001756:
6819           	ldr	r1,[r3]
F8C3 45C0     	str	r4,[r3,#&5C0]

;; fn0000175A: 0000175A
fn0000175A proc
45C0           	cmps	r8,r8
188C           	add	r4,r1,r2
F7FF FB85     	bl	$00000E68

l00001762:
4620           	mov	r0,r4
BD10           	pop	{r4,pc}
00001766                   00 24 FF F7 80 FB 20 46 10 BD       .$.... F..

l00001770:
F103 020C     	add	r2,r3,#&C

;; fn00001772: 00001772
fn00001772 proc
020C           	lsls	r4,r1,#8
F022 0207     	bic	r2,r2,#7
601A           	str	r2,[r3]
E7E3           	b	$00001740

;; fn0000177C: 0000177C
fn0000177C proc
0230           	lsls	r0,r6,#8
2000           	mov	r0,#0

;; vPortFree: 00001780
vPortFree proc
4770           	bx	lr
00001782       00 BF                                       ..           

;; vPortInitialiseBlocks: 00001784
vPortInitialiseBlocks proc
2200           	mov	r2,#0
4B02           	ldr	r3,[pc,#&8]                             ; 00001796
F8C3 25C0     	str	r2,[r3,#&5C0]
4770           	bx	lr
0000178E                                           00 BF               ..

;; fn00001790: 00001790
fn00001790 proc
0230           	lsls	r0,r6,#8
2000           	mov	r0,#0

;; xPortGetFreeHeapSize: 00001794
xPortGetFreeHeapSize proc
4B03           	ldr	r3,[pc,#&C]                             ; 000017A8
F8D3 05C0     	ldr	r0,[r3,#&5C0]
F5C0 60B6     	rsb	r0,r0,#&5B0
3004           	adds	r0,#4
4770           	bx	lr
000017A2       00 BF                                       ..           

;; fn000017A4: 000017A4
fn000017A4 proc
0230           	lsls	r0,r6,#8
2000           	mov	r0,#0

;; xEventGroupCreate: 000017A8
xEventGroupCreate proc
B510           	push	{r4,lr}
2018           	mov	r0,#&18
F7FF FFBE     	bl	$00001728
4604           	mov	r4,r0
B120           	cbz	r0,$000017BE

l000017B4:
2300           	mov	r3,#0
F840 3B04     	str	r3,[r0],#&4
F006 FD89     	bl	$000082CC

;; fn000017BE: 000017BE
fn000017BE proc
4620           	mov	r0,r4

;; fn000017C0: 000017C0
fn000017C0 proc
BD10           	pop	{r4,pc}
000017C2       00 BF                                       ..           

;; xEventGroupWaitBits: 000017C4
xEventGroupWaitBits proc
E92D 41F0     	push.w	{r4-r8,lr}
4606           	mov	r6,r0
461F           	mov	r7,r3
460D           	mov	r5,r1
4690           	mov	r8,r2
F7FF F91C     	bl	$00000A08
6834           	ldr	r4,[r6]
B967           	cbnz	r7,$000017F2

l000017D8:
422C           	adcs	r4,r5
D00D           	beq	$000017F4

l000017DC:
F1B8 0F00     	cmp	r8,#0
D002           	beq	$000017E4

l000017E2:
EA24 0505     	bic.w	r5,r4,r5

l000017E4:
0505           	lsls	r5,r0,#&14

l000017E6:
6035           	str	r5,[r6]
F7FF FB40     	bl	$00000E68
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l000017F2:
EA35 0304     	bics.w	r3,r5,r4

l000017F4:
0304           	lsls	r4,r0,#&C

l000017F6:
D0F1           	beq	$000019D8

l000017F8:
9B06           	ldr	r3,[sp,#&18]
2B00           	cmps	r3,#0
D0F4           	beq	$000019E4

l000017FE:
F1B8 0F00     	cmp	r8,#0
BF0C           	ite	eq
2100           	moveq	r1,#0

l00001806:
F04F 7180     	mov	r1,#&1000000

l00001808:
7180           	strb	r0,[r0,#&6]
B9C7           	cbnz	r7,$0000183E

l0000180C:
4329           	orrs	r1,r5
9A06           	ldr	r2,[sp,#&18]
1D30           	add	r0,r6,#4
F7FF FBF1     	bl	$00000FF4
F7FF FB29     	bl	$00000E68
B938           	cbnz	r0,$0000182C

l0000181C:
F04F 5280     	mov	r2,#&10000000
4B13           	ldr	r3,[pc,#&4C]                            ; 00001874
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l0000182C:
F7FF FC78     	bl	$0000111C
0183           	lsls	r3,r0,#6
4604           	mov	r4,r0
D506           	bpl	$00001840

l00001836:
F024 407F     	bic	r0,r4,#&FF000000
E8BD 81F0     	pop.w	{r4-r8,pc}

l0000183E:
F041 6180     	orr	r1,r1,#&4000000

l00001840:
6180           	str	r0,[r0,#&18]

l00001842:
E7E3           	b	$00001808
00001844             06 F0 98 FE 34 68 6F B9 25 42 05 D0     ....4ho.%B..
00001850 B8 F1 00 0F 02 D0 24 EA 05 05 35 60 06 F0 A8 FE ......$...5`....
00001860 24 F0 7F 40 BD E8 F0 81 35 EA 04 03 F6 D1 EF E7 $..@....5.......

;; fn00001870: 00001870
fn00001870 proc
ED04 E000     	Invalid

;; xEventGroupClearBits: 00001874
xEventGroupClearBits proc
B570           	push	{r4-r6,lr}
4606           	mov	r6,r0
460C           	mov	r4,r1
F006 FE7D     	bl	$00008574
6835           	ldr	r5,[r6]
EA25 0404     	bic.w	r4,r5,r4
6034           	str	r4,[r6]
F006 FE93     	bl	$000085AC
4628           	mov	r0,r5

;; fn0000188C: 0000188C
fn0000188C proc
BD70           	pop	{r4-r6,pc}
0000188E                                           00 BF               ..

;; xEventGroupSetBits: 00001890
xEventGroupSetBits proc
B5F8           	push	{r3-r7,lr}
4605           	mov	r5,r0
460C           	mov	r4,r1
F7FF F8B9     	bl	$00000A08
6829           	ldr	r1,[r5]
6928           	ldr	r0,[r5,#&10]
F105 060C     	add	r6,r5,#&C
4321           	orrs	r1,r4
4286           	cmps	r6,r0
6029           	str	r1,[r5]
D022           	beq	$000018EC

l000018AA:
2700           	mov	r7,#0
E00C           	b	$000018C4
000018AE                                           0A 42               .B
000018B0 07 D0 DB 01 00 D5 17 43 41 F0 00 71 FF F7 E0 FB .......CA..q....
000018C0 29 68 A6 42                                     )h.B           

l000018C4:
4620           	mov	r0,r4
D00C           	beq	$000018DE

l000018C8:
E890 0018     	ldm	r0,{r3-r4}
F013 6F80     	tst	r3,#&4000000
F023 427F     	bic	r2,r3,#&FF000000
D0EB           	beq	$00001AAA

l000018D6:
EA32 0E01     	bics.w	lr,r2,r1
D0EA           	beq	$00001AAE

l000018DC:
42A6           	cmps	r6,r4

l000018DE:
4620           	mov	r0,r4

;; fn000018E0: 000018E0
fn000018E0 proc
D1F2           	bne	$00001AC4

l000018E2:
43FF           	mvns	r7,r7
4039           	ands	r1,r7
6029           	str	r1,[r5]
F7FF FAC0     	bl	$00000E68

l000018EC:
6828           	ldr	r0,[r5]
BDF8           	pop	{r3-r7,pc}
000018F0 4F F0 FF 37                                     O..7           

;; fn000018F4: 000018F4
fn000018F4 proc
E7F6           	b	$000018E0
000018F6                   00 BF                               ..       

;; xEventGroupSync: 000018F8
xEventGroupSync proc
E92D 41F0     	push.w	{r4-r8,lr}
4688           	mov	r8,r1
4605           	mov	r5,r0
4616           	mov	r6,r2
461F           	mov	r7,r3
F7FF F882     	bl	$00000A08
4641           	mov	r1,r8
682C           	ldr	r4,[r5]
4628           	mov	r0,r5
430C           	orrs	r4,r1
F7FF FFBE     	bl	$0000188C
EA36 0304     	bics.w	r3,r6,r4
D021           	beq	$0000195A

l0000191A:
B92F           	cbnz	r7,$00001928

l0000191C:
682C           	ldr	r4,[r5]
F7FF FAA5     	bl	$00000E68
4620           	mov	r0,r4
E8BD 81F0     	pop.w	{r4-r8,pc}

l00001928:
463A           	mov	r2,r7
F046 61A0     	orr	r1,r6,#&5000000
1D28           	add	r0,r5,#4
F7FF FB62     	bl	$00000FF4
F7FF FA9A     	bl	$00000E68
B938           	cbnz	r0,$0000194A

l0000193A:
F04F 5280     	mov	r2,#&10000000
4B11           	ldr	r3,[pc,#&44]                            ; 0000198A
601A           	str	r2,[r3]
F3BF 8F4F     	dsb	sy
F3BF 8F6F     	isb	sy

l0000194A:
F7FF FBE9     	bl	$0000111C
0183           	lsls	r3,r0,#6
4604           	mov	r4,r0
D509           	bpl	$00001964

l00001954:
F024 447F     	bic	r4,r4,#&FF000000
4620           	mov	r0,r4

l0000195A:
E8BD 81F0     	pop.w	{r4-r8,pc}
0000195E                                           2B 68               +h
00001960 23 EA 06 06                                     #...           

l00001964:
602E           	str	r6,[r5]
E7DA           	b	$0000191A
00001968                         06 F0 06 FE 2C 68 36 EA         ....,h6.
00001970 04 03 04 BF 24 EA 06 06 2E 60 06 F0 19 FE 24 F0 ....$....`....$.
00001980 7F 44 E9 E7 04 ED 00 E0                         .D......       

;; xEventGroupGetBitsFromISR: 00001988
xEventGroupGetBitsFromISR proc
F3EF 8311     	mrs	r3,cpsr
F04F 02BF     	mov	r2,#&BF
F382 8811     	msr	cpsr,r2
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
F383 8811     	msr	cpsr,r3

;; fn000019A0: 000019A0
fn000019A0 proc
6800           	ldr	r0,[r0]
4770           	bx	lr

;; vEventGroupDelete: 000019A4
vEventGroupDelete proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F7FF F830     	bl	$00000A08
6863           	ldr	r3,[r4,#&4]
B13B           	cbz	r3,$000019C0

l000019B0:
F04F 7100     	mov	r1,#&2000000
6920           	ldr	r0,[r4,#&10]
F7FF FB63     	bl	$0000107C
6863           	ldr	r3,[r4,#&4]
2B00           	cmps	r3,#0
D1F7           	bne	$00001BAC

l000019C0:
4620           	mov	r0,r4
F7FF FEDD     	bl	$0000177C
E8BD 4010     	pop.w	{r4,lr}
F7FF BA4F     	b	$00000E68
000019CE                                           00 BF               ..

;; vEventGroupSetBitsCallback: 000019D0
vEventGroupSetBitsCallback proc
F7FF BF5E     	b	$0000188C

;; vEventGroupClearBitsCallback: 000019D4
vEventGroupClearBitsCallback proc
B538           	push	{r3-r5,lr}
4604           	mov	r4,r0

l000019D8:
460D           	mov	r5,r1
F006 FDCD     	bl	$00008574
6823           	ldr	r3,[r4]
EA23 0305     	bic.w	r3,r3,r5

l000019E4:
6023           	str	r3,[r4]
E8BD 4038     	pop.w	{r3-r5,lr}
F006 BDE1     	b	$00C085AC
000019EE                                           00 BF               ..
000019F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00001AA0 00 00 00 00 00 00 00 00 00 00                   ..........     

l00001AAA:
0000           	mov	r0,r0
0000           	mov	r0,r0

l00001AAE:
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0

;; fn00001AC4: 00001AC4
fn00001AC4 proc
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0

;; fn00001BAC: 00001BAC
fn00001BAC proc
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0
0000           	mov	r0,r0

;; fn00007FFC: 00007FFC
fn00007FFC proc
0000           	mov	r0,r0
0000           	mov	r0,r0
;;; Segment .text (00008000)

;; NmiSR: 00008000
NmiSR proc
E7FE           	b	$00007FFC
00008002       00 BF                                       ..           

;; FaultISR: 00008004
FaultISR proc
E7FE           	b	$00008000
00008006                   00 BF                               ..       

;; ResetISR: 00008008
ResetISR proc
4B08           	ldr	r3,[pc,#&20]                            ; 00008030
4809           	ldr	r0,[pc,#&24]                            ; 00008036
4283           	cmps	r3,r0
D20A           	bhs	$00008022

l00008010:
43DA           	mvns	r2,r3
2100           	mov	r1,#0
4402           	adds	r2,r0
F022 0203     	bic	r2,r2,#3
3204           	adds	r2,#4
441A           	adds	r2,r3
F843 1B04     	str	r1,[r3],#&4

l00008022:
4293           	cmps	r3,r2
D1FB           	bne	$0000821A

l00008026:
F000 B83B     	b	$00C0809C
0000802A                               00 BF 60 01 00 20           ..`.. 

l00008030:
0880           	lsrs	r0,r0,#2
2000           	mov	r0,#0

;; raise: 00008034
raise proc
E7FE           	b	$00008030
00008036                   00 BF                               ..       

;; vPrintTask: 00008038
vPrintTask proc
B530           	push	{r4-r5,lr}
2400           	mov	r4,#0

l0000803C:
4D09           	ldr	r5,[pc,#&24]                            ; 00008068
B083           	sub	sp,#&C
A901           	add	r1,sp,#4
3401           	adds	r4,#1
2300           	mov	r3,#0
F04F 32FF     	mov	r2,#&FFFFFFFF
6828           	ldr	r0,[r5]
F000 FD8E     	bl	$00008B68
F001 FB96     	bl	$0000977C
F004 0201     	and	r2,r4,#1
F004 013F     	and	r1,r4,#&3F
9801           	ldr	r0,[sp,#&4]
F001 FBB5     	bl	$000097C8
E7ED           	b	$0000803C
00008064             80 08 00 20                             ...        

;; vCheckTask: 00008068
vCheckTask proc
B530           	push	{r4-r5,lr}
4B0B           	ldr	r3,[pc,#&2C]                            ; 0000809E
B083           	sub	sp,#&C
9301           	str	r3,[sp,#&4]
F000 FC48     	bl	$00008900
AC02           	add	r4,sp,#8
4D09           	ldr	r5,[pc,#&24]                            ; 000080A2

l00008078:
F844 0D08     	str	r0,[r4,-#&8]!
4620           	mov	r0,r4
F241 3188     	mov	r1,#&1388
F000 FBF7     	bl	$00008870
2300           	mov	r3,#0
F04F 32FF     	mov	r2,#&FFFFFFFF
A901           	add	r1,sp,#4
6828           	ldr	r0,[r5]
F000 FD28     	bl	$00008AE0
E7F2           	b	$00008078
00008096                   00 BF 50 A2 00 00 80 08 00 20       ..P...... 

;; Main: 000080A0
Main proc
B500           	push	{lr}
2200           	mov	r2,#0
B083           	sub	sp,#&C
2104           	mov	r1,#4
2003           	mov	r0,#3
2400           	mov	r4,#0
F000 FCEC     	bl	$00008A84
4B0F           	ldr	r3,[pc,#&3C]                            ; 000080F4
6018           	str	r0,[r3]
4620           	mov	r0,r4
F001 FC1B     	bl	$000098EC
2203           	mov	r2,#3
4623           	mov	r3,r4
9200           	str	r2,[sp]
490C           	ldr	r1,[pc,#&30]                            ; 000080F8
223B           	mov	r2,#&3B
9401           	str	r4,[sp,#&4]
480C           	ldr	r0,[pc,#&30]                            ; 000080FE
F000 FB9E     	bl	$00008804
2202           	mov	r2,#2
490B           	ldr	r1,[pc,#&2C]                            ; 00008102
4623           	mov	r3,r4
9200           	str	r2,[sp]
9401           	str	r4,[sp,#&4]
223B           	mov	r2,#&3B
4809           	ldr	r0,[pc,#&24]                            ; 00008104
F000 FB95     	bl	$00008804
F7F8 FC57     	bl	$0000098C
4622           	mov	r2,r4
4621           	mov	r1,r4
4807           	ldr	r0,[pc,#&1C]                            ; 0000810A

l000080E8:
F001 FB70     	bl	$000097C8
E7FE           	b	$000080E8
000080EE                                           00 BF               ..
000080F0 80 08 00 20 58 A2 00 00 69 80 00 00 60 A2 00 00 ... X...i...`...
00008100 39 80 00 00 68 A2 00 00                         9...h...       

;; vUART_ISR: 00008108
vUART_ISR proc
B570           	push	{r4-r6,lr}
2600           	mov	r6,#0
4D19           	ldr	r5,[pc,#&64]                            ; 00008178
B082           	sub	sp,#8
2101           	mov	r1,#1
4628           	mov	r0,r5
9601           	str	r6,[sp,#&4]
F001 FFD9     	bl	$0000A0C8
4604           	mov	r4,r0
4601           	mov	r1,r0
4628           	mov	r0,r5
F001 FFDA     	bl	$0000A0D4
06E2           	lsls	r2,r4,#&1B
D503           	bpl	$0000812C

l00008128:
4B13           	ldr	r3,[pc,#&4C]                            ; 0000817C
681B           	ldr	r3,[r3]

l0000812C:
065B           	lsls	r3,r3,#&19
D416           	bmi	$0000815A

l00008130:
06A0           	lsls	r0,r4,#&1A
D503           	bpl	$00008138

l00008134:
4A11           	ldr	r2,[pc,#&44]                            ; 00008180
7813           	ldrb	r3,[r2]

l00008138:
2B7A           	cmps	r3,#&7A
D907           	bls	$00008148

l0000813C:
9B01           	ldr	r3,[sp,#&4]
B11B           	cbz	r3,$00008148

l00008140:
F04F 5280     	mov	r2,#&10000000
4B0E           	ldr	r3,[pc,#&38]                            ; 00008184
601A           	str	r2,[r3]

l00008148:
B002           	add	sp,#8
BD70           	pop	{r4-r6,pc}
0000814C                                     0A 49 09 68             .I.h
00008150 89 06 5C BF 07 49 0B 60 01 33                   ..\..I.`.3     

l0000815A:
7013           	strb	r3,[r2]
E7EE           	b	$00008138
0000815E                                           2D 68               -h
00008160 33 46 30 46 01 AA 0D F1 03 01 8D F8 03 50 F8 F7 3F0F.........P..
00008170 73 F9 DD E7 00 C0 00 40 18 C0 00 40 2C 02 00 20 s......@...@,.. 
00008180 04 ED 00 E0                                     ....           

;; vSetErrorLED: 00008184
vSetErrorLED proc
2101           	mov	r1,#1
2007           	mov	r0,#7

;; fn00008188: 00008188
fn00008188 proc
F000 BA34     	b	$00C085F0

;; prvSetAndCheckRegisters: 0000818C
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
D11C           	bne	$000081FC

l000081C6:
280B           	cmps	r0,#&B
D11A           	bne	$000081FC

l000081CA:
290C           	cmps	r1,#&C
D118           	bne	$000081FC

l000081CE:
2A0D           	cmps	r2,#&D
D116           	bne	$000081FC

l000081D2:
2B0E           	cmps	r3,#&E
D114           	bne	$000081FC

l000081D6:
2C0F           	cmps	r4,#&F
D112           	bne	$000081FC

l000081DA:
2D10           	cmps	r5,#&10
D110           	bne	$000081FC

l000081DE:
2E11           	cmps	r6,#&11
D10E           	bne	$000081FC

l000081E2:
2F12           	cmps	r7,#&12
D10C           	bne	$000081FC

l000081E6:
F1B8 0F13     	cmp	r8,#&13
D109           	bne	$000081FC

l000081EC:
F1B9 0F14     	cmp	r9,#&14
D106           	bne	$000081FC

l000081F2:
F1BA 0F15     	cmp	r10,#&15
D103           	bne	$000081FC

l000081F8:
F1BC 0F16     	cmp	ip,#&16

l000081FC:
D100           	bne	$000081FC

l000081FE:
4770           	bx	lr
00008200 00 B5 06 49 88 47 5D F8 04 EB 70 47             ...I.G]...pG   

;; fn0000820C: 0000820C
fn0000820C proc
4770           	bx	lr

l0000820E:
BF00           	nop

;; vApplicationIdleHook: 00008210
vApplicationIdleHook proc
B508           	push	{r3,lr}
F000 FE8B     	bl	$00008F28
F7FF FFB9     	bl	$00008188

;; fn0000821A: 0000821A
fn0000821A proc
E7FA           	b	$0000820E

;; fn0000821C: 0000821C
fn0000821C proc
8185           	strh	r5,[r0,#&18]
0000           	mov	r0,r0

;; PDCInit: 00008220
PDCInit proc
B530           	push	{r4-r5,lr}
481A           	ldr	r0,[pc,#&68]                            ; 00008292
B083           	sub	sp,#&C
F001 FCA9     	bl	$00009B78
4819           	ldr	r0,[pc,#&64]                            ; 00008296
F001 FCA6     	bl	$00009B78
2202           	mov	r2,#2
2134           	mov	r1,#&34
F04F 2040     	mov	r0,#&40004000
F000 FF68     	bl	$00009108
2201           	mov	r2,#1
2108           	mov	r1,#8
F04F 2040     	mov	r0,#&40004000
F000 FF62     	bl	$00009108
230A           	mov	r3,#&A
2202           	mov	r2,#2
2104           	mov	r1,#4
F04F 2040     	mov	r0,#&40004000
F000 FFB9     	bl	$000091C4
2408           	mov	r4,#8
2200           	mov	r2,#0
4D0E           	ldr	r5,[pc,#&38]                            ; 0000829A
4611           	mov	r1,r2
4B0E           	ldr	r3,[pc,#&38]                            ; 0000829E
4628           	mov	r0,r5
9400           	str	r4,[sp]
F001 FBC0     	bl	$000099E4
4628           	mov	r0,r5
F001 FBE3     	bl	$00009A30
4621           	mov	r1,r4
2200           	mov	r2,#0
F04F 2040     	mov	r0,#&40004000
F001 F8ED     	bl	$00009450
4622           	mov	r2,r4
4621           	mov	r1,r4
F04F 2040     	mov	r0,#&40004000
B003           	add	sp,#&C
E8BD 4030     	pop.w	{r4-r5,lr}
F001 B8E4     	b	$00C09450
0000828C                                     10 00 00 10             ....
00008290 01 00 00 20 00 80 00 40                         ... ...@       

;; fn00008298: 00008298
fn00008298 proc
4240           	rsbs	r0,r0
000F           	mov	r7,r1

;; PDCWrite: 0000829C
PDCWrite proc
B530           	push	{r4-r5,lr}
460D           	mov	r5,r1
4C0A           	ldr	r4,[pc,#&28]                            ; 000082D0
B083           	sub	sp,#&C
F000 010F     	and	r1,r0,#&F
4620           	mov	r0,r4
F001 FBF5     	bl	$00009A94
4629           	mov	r1,r5
4620           	mov	r0,r4
F001 FBF1     	bl	$00009A94
4620           	mov	r0,r4
A901           	add	r1,sp,#4
F001 FBFD     	bl	$00009AB4
A901           	add	r1,sp,#4
4620           	mov	r0,r4
F001 FBF9     	bl	$00009AB4
B003           	add	sp,#&C
BD30           	pop	{r4-r5,pc}
000082CA                               00 BF                       ..   

;; fn000082CC: 000082CC
fn000082CC proc
8000           	strh	r0,[r0]
4000           	ands	r0,r0

;; vListInitialise: 000082D0
vListInitialise proc
F04F 31FF     	mov	r1,#&FFFFFFFF
2200           	mov	r2,#0
F100 0308     	add	r3,r0,#8
6081           	str	r1,[r0,#&8]
E880 000C     	stm	r0,{r2-r3}
60C3           	str	r3,[r0,#&C]
6103           	str	r3,[r0,#&10]

;; fn000082E4: 000082E4
fn000082E4 proc
4770           	bx	lr
000082E6                   00 BF                               ..       

;; vListInitialiseItem: 000082E8
vListInitialiseItem proc
2300           	mov	r3,#0
6103           	str	r3,[r0,#&10]

;; fn000082EC: 000082EC
fn000082EC proc
4770           	bx	lr
000082EE                                           00 BF               ..

;; vListInsertEnd: 000082F0
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

;; fn00008308: 00008308
fn00008308 proc
6002           	str	r2,[r0]
4770           	bx	lr

;; vListInsert: 0000830C
vListInsert proc
B430           	push	{r4-r5}
680D           	ldr	r5,[r1]
1C6B           	add	r3,r5,#1
D011           	beq	$00008334

l00008314:
F100 0208     	add	r2,r0,#8

l00008318:
E000           	b	$00008318
0000831A                               1A 46 53 68 1C 68           .FSh.h

l00008320:
42A5           	cmps	r5,r4
D2FA           	bhs	$00008516

l00008324:
6804           	ldr	r4,[r0]
604B           	str	r3,[r1,#&4]
3401           	adds	r4,#1
6099           	str	r1,[r3,#&8]
608A           	str	r2,[r1,#&8]
6051           	str	r1,[r2,#&4]
6108           	str	r0,[r1,#&10]
6004           	str	r4,[r0]

l00008334:
BC30           	pop	{r4-r5}
4770           	bx	lr
00008338                         02 69 53 68                     .iSh   

;; fn0000833C: 0000833C
fn0000833C proc
E7F2           	b	$00008320
0000833E                                           00 BF               ..

;; uxListRemove: 00008340
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
xQueueCRSend proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4614           	mov	r4,r2
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
F000 F8FC     	bl	$00008574
6BAA           	ldr	r2,[r5,#&38]
6BEB           	ldr	r3,[r5,#&3C]
429A           	cmps	r2,r3
D014           	beq	$000083AE

l00008388:
F000 F912     	bl	$000085AC
2000           	mov	r0,#0
F380 8811     	msr	cpsr,r0
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6BAA           	ldr	r2,[r5,#&38]
6BEB           	ldr	r3,[r5,#&3C]

;; fn000083A6: 000083A6
fn000083A6 proc
429A           	cmps	r2,r3
D30A           	blo	$000083BC

l000083AA:
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3

l000083AE:
8811           	ldrh	r1,[r2]
BD70           	pop	{r4-r6,pc}
000083B2       00 F0 FD F8 7C B9 84 F3 11 88               ....|.....   

l000083BC:
4620           	mov	r0,r4
BD70           	pop	{r4-r6,pc}
000083C0 02 46 31 46 28 46 F7 F7 91 FE 6B 6A 7B B9 01 20 .F1F(F....kj{.. 
000083D0 00 23 83 F3 11 88 70 BD 05 F1 10 01 20 46 00 F0 .#....p..... F..
000083E0 87 FD 00 23 83 F3 11 88 6F F0 03 00 70 BD 05 F1 ...#....o...p...
000083F0 24 00 00 F0 4F FE 00 28 E9 D0 6F F0             $...O..(..o.   

;; fn000083FC: 000083FC
fn000083FC proc
0004           	mov	r4,r0
E7D4           	b	$000083A6

;; xQueueCRReceive: 00008400
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
D136           	bne	$00008486

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

l0000843C:
B922           	cbnz	r2,$00008448

l0000843E:
4610           	mov	r0,r2
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
F002 F8AD     	bl	$0000A5C0
6923           	ldr	r3,[r4,#&10]
B923           	cbnz	r3,$00008478

l0000846E:
2001           	mov	r0,#1
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
BD38           	pop	{r3-r5,pc}

l00008478:
F104 0010     	add	r0,r4,#&10
F000 FE0A     	bl	$00009090
2800           	cmps	r0,#0
D0F4           	beq	$0000866A

l00008484:
F06F 0004     	mvn	r0,#4

l00008486:
0004           	mov	r4,r0

l00008488:
E7DA           	b	$0000843C
0000848A                               00 F1 24 01 10 46           ..$..F
00008490 00 F0 2E FD 85 F3 11 88 6F F0 03 00 38 BD 00 BF ........o...8...

;; xQueueCRSendFromISR: 000084A0
xQueueCRSendFromISR proc
B570           	push	{r4-r6,lr}
6BC3           	ldr	r3,[r0,#&3C]
6B86           	ldr	r6,[r0,#&38]
4615           	mov	r5,r2
429E           	cmps	r6,r3
D301           	blo	$000084AC

l000084AC:
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}
000084B0 00 22 04 46 F7 F7 1A FE 00 2D F7 D1 63 6A 00 2B .".F.....-..cj.+
000084C0 F4 D0 04 F1 24 00 00 F0 E5 FD 05 1C 18 BF 01 25 ....$..........%
000084D0 EC E7 00 BF                                     ....           

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
F002 F85E     	bl	$0000A5C0
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

l00008516:
BDF8           	pop	{r3-r7,pc}

l00008518:
F104 0010     	add	r0,r4,#&10
F000 FDBA     	bl	$00009090
2800           	cmps	r0,#0
D0F5           	beq	$0000870C

l00008524:
2001           	mov	r0,#1
6028           	str	r0,[r5]
BDF8           	pop	{r3-r7,pc}

l0000852A:
BF00           	nop

;; prvIdleTask: 0000852C
prvIdleTask proc
B508           	push	{r3,lr}
F7FF FE6F     	bl	$0000820C

;; fn00008530: 00008530
fn00008530 proc
FE6F E7FC     	Invalid

;; fn00008532: 00008532
fn00008532 proc
E7FC           	b	$0000852A

;; xTaskNotifyStateClear: 00008534
xTaskNotifyStateClear proc
B538           	push	{r3-r5,lr}

l00008536:
B178           	cbz	r0,$00008558

l00008538:
4604           	mov	r4,r0
F000 F81D     	bl	$00008574
F894 3064     	ldrb	r3,[r4,#&64]
2B02           	cmps	r3,#2
BF05           	ittet	eq
2300           	moveq	r3,#0

l00008548:
2501           	mov	r5,#1
2500           	mov	r5,#0
F884 3064     	strb	r3,[r4,#&64]
F000 F82E     	bl	$000085AC
4628           	mov	r0,r5
BD38           	pop	{r3-r5,pc}

l00008558:
4B01           	ldr	r3,[pc,#&4]                             ; 00008564
685C           	ldr	r4,[r3,#&4]
E7ED           	b	$00008536
0000855E                                           00 BF               ..

;; fn00008560: 00008560
fn00008560 proc
00C4           	lsls	r4,r0,#3
2000           	mov	r0,#0

;; xPortRaisePrivilege: 00008564
xPortRaisePrivilege proc
F3EF 8014     	mrs	r0,cpsr
F010 0F01     	tst	r0,#1
BF1A           	itte	ne
2000           	movne	r0,#0

l00008570:
DF02           	svc	#2
2001           	mov	r0,#1

;; fn00008574: 00008574
fn00008574 proc
4770           	bx	lr
00008576                   00 20                               .        

;; vPortEnterCritical: 00008578
vPortEnterCritical proc
B508           	push	{r3,lr}
F7FF FFF3     	bl	$00008560
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
4A07           	ldr	r2,[pc,#&1C]                            ; 000085B2
2801           	cmps	r0,#1
6813           	ldr	r3,[r2]
F103 0301     	add	r3,r3,#1
6013           	str	r3,[r2]
D005           	beq	$000085A4

l0000859C:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000085A4:
F380 8814     	msr	cpsr,r0
BD08           	pop	{r3,pc}
000085AA                               00 BF                       ..   

;; fn000085AC: 000085AC
fn000085AC proc
00BC           	lsls	r4,r7,#2
2000           	mov	r0,#0

;; vPortExitCritical: 000085B0
vPortExitCritical proc
B508           	push	{r3,lr}
F7FF FFD7     	bl	$00008560
4A08           	ldr	r2,[pc,#&20]                            ; 000085DE
6813           	ldr	r3,[r2]
3B01           	subs	r3,#1
6013           	str	r3,[r2]
B90B           	cbnz	r3,$000085C4

l000085C0:
F383 8811     	msr	cpsr,r3

l000085C4:
2801           	cmps	r0,#1
D005           	beq	$000085D0

l000085C8:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000085D0:
F380 8814     	msr	cpsr,r0
BD08           	pop	{r3,pc}
000085D6                   00 BF BC 00 00 20                   .....    

;; vParTestInitialise: 000085DC
vParTestInitialise proc
B508           	push	{r3,lr}
F7FF FE1F     	bl	$0000821C
4B03           	ldr	r3,[pc,#&C]                             ; 000085F6
2005           	mov	r0,#5
7819           	ldrb	r1,[r3]
E8BD 4008     	pop.w	{r3,lr}
F7FF BE56     	b	$00008298
000085F0 F4 07 00 20                                     ...            

;; vParTestSetLED: 000085F4
vParTestSetLED proc
B538           	push	{r3-r5,lr}
4604           	mov	r4,r0
460D           	mov	r5,r1
F000 F961     	bl	$000088BC
2C07           	cmps	r4,#7
D80C           	bhi	$00008618

l00008602:
2301           	mov	r3,#1
FA03 F004     	lsl	r0,r3,r4
4B08           	ldr	r3,[pc,#&20]                            ; 00008630
B2C0           	uxtb	r0,r0
781A           	ldrb	r2,[r3]
B14D           	cbz	r5,$00008624

l00008610:
4310           	orrs	r0,r2
7018           	strb	r0,[r3]
7819           	ldrb	r1,[r3]
2005           	mov	r0,#5

l00008618:
F7FF FE40     	bl	$00008298
E8BD 4038     	pop.w	{r3-r5,lr}
F000 B95E     	b	$00C088DC

l00008624:
EA22 0000     	bic.w	r0,r2,r0
7018           	strb	r0,[r3]
E7F3           	b	$00008610

;; fn0000862C: 0000862C
fn0000862C proc
07F4           	lsls	r4,r6,#&1F
2000           	mov	r0,#0

;; vParTestToggleLED: 00008630
vParTestToggleLED proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F000 F944     	bl	$000088BC
2C07           	cmps	r4,#7
D80E           	bhi	$00008656

l0000863C:
2201           	mov	r2,#1
4B0B           	ldr	r3,[pc,#&2C]                            ; 00008672
FA02 F004     	lsl	r0,r2,r4
7819           	ldrb	r1,[r3]
B2C2           	uxtb	r2,r0
420A           	adcs	r2,r1
D10A           	bne	$0000865E

l0000864C:
7819           	ldrb	r1,[r3]

;; fn0000864E: 0000864E
fn0000864E proc
430A           	orrs	r2,r1
701A           	strb	r2,[r3]
7819           	ldrb	r1,[r3]
2005           	mov	r0,#5

;; fn00008656: 00008656
fn00008656 proc
F7FF FE21     	bl	$00008298
E8BD 4010     	pop.w	{r4,lr}

l0000865E:
F000 B93F     	b	$00C088DC
00008662       1A 78 22 EA 00 00 18 70                     .x"....p     

l0000866A:
E7F2           	b	$0000864E
0000866C                                     F4 07 00 20             ... 

;; prvFlashCoRoutine: 00008670
prvFlashCoRoutine proc
B570           	push	{r4-r6,lr}
8E83           	ldrh	r3,[r0,#&68]
B082           	sub	sp,#8
F5B3 7FE1     	cmp	r3,#&1C2
4604           	mov	r4,r0
D01B           	beq	$000086B2

l0000867E:
F240 12C3     	mov	r2,#&1C3
4293           	cmps	r3,r2
D002           	beq	$00008688

l00008686:
B323           	cbz	r3,$000086D2

l00008688:
B002           	add	sp,#8
BD70           	pop	{r4-r6,pc}
0000868C                                     14 4D 01 AE             .M..
00008690 01 98                                           ..             

l00008692:
F7FF FFCD     	bl	$0000862C
F04F 32FF     	mov	r2,#&FFFFFFFF
4631           	mov	r1,r6
6828           	ldr	r0,[r5]
F7FF FEAF     	bl	$000083FC
1D02           	add	r2,r0,#4
D018           	beq	$000086D4

l000086A6:
1D43           	add	r3,r0,#5
D00E           	beq	$000086C4

l000086AA:
2801           	cmps	r0,#1
D0F0           	beq	$0000888C

l000086AE:
2200           	mov	r2,#0
4B0C           	ldr	r3,[pc,#&30]                            ; 000086E8

l000086B2:
601A           	str	r2,[r3]
E7EF           	b	$00008692
000086B6                   0A 4D 01 AE 28 68 31 46 00 22       .M..(h1F."
000086C0 FF F7 9E FE                                     ....           

l000086C4:
1D43           	add	r3,r0,#5
D1F0           	bne	$000088A6

l000086C8:
F240 13C3     	mov	r3,#&1C3
86A3           	strh	r3,[r4,#&68]
B002           	add	sp,#8
BD70           	pop	{r4-r6,pc}

l000086D2:
4D03           	ldr	r5,[pc,#&C]                             ; 000086E6

l000086D4:
AE01           	add	r6,sp,#4
E7DE           	b	$00008692
000086D8                         4F F4 E1 73 A3 86 D3 E7         O..s....
000086E0 F8 07 00 20 C0 00 00 20                         ... ...        

;; prvFixedDelayCoRoutine: 000086E8
prvFixedDelayCoRoutine proc
B510           	push	{r4,lr}
8E83           	ldrh	r3,[r0,#&68]
B082           	sub	sp,#8
F5B3 7FC1     	cmp	r3,#&182
4604           	mov	r4,r0
9101           	str	r1,[sp,#&4]
D02B           	beq	$0000874C

l000086F8:
D926           	bls	$00008744

l000086FA:
F240 1283     	mov	r2,#&183
4293           	cmps	r3,r2
D109           	bne	$00008712

l00008702:
4B1D           	ldr	r3,[pc,#&74]                            ; 0000877E
9A01           	ldr	r2,[sp,#&4]
F853 0022     	ldr.w	r0,[r3,r2,lsl #2]

l00008708:
0022           	mov	r2,r4
BB40           	cbnz	r0,$0000875E

;; fn0000870C: 0000870C
fn0000870C proc
F44F 73CB     	mov	r3,#&196
86A3           	strh	r3,[r4,#&68]

l00008712:
B002           	add	sp,#8
BD10           	pop	{r4,pc}
00008716                   B3 F5 CB 7F FA D1 17 4B 00 22       .......K."
00008720 18 68 01 A9 FF F7 1E FE 02 1D 20 D0 43 1D 1A D0 .h........ .C...
00008730 01 28 E6 D0 00 22 12 4B 1A 60 0F 4B 01 9A 53 F8 .(...".K.`.K..S.
00008740 22 00 00 28                                     "..(           

l00008744:
D0E2           	beq	$00008908

l00008746:
E00A           	b	$0000875A
00008748                         00 2B E7 D0                     .+..   

l0000874C:
B002           	add	sp,#8
BD10           	pop	{r4,pc}
00008750 0A 4B 00 22 18 68 01 A9 FF F7                   .K.".h....     

l0000875A:
FE04 E7E6     	Invalid

l0000875E:
2100           	mov	r1,#0
F000 FBC6     	bl	$00008EEC
E7D2           	b	$00008708
00008766                   40 F2 83 13 A3 86 D1 E7 4F F4       @.......O.
00008770 C1 73 A3 86 CD E7 00 BF 84 A2 00 00 F8 07 00 20 .s............. 
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
F000 F978     	bl	$00008A84
4B0A           	ldr	r3,[pc,#&28]                            ; 000087C8
6018           	str	r0,[r3]
B188           	cbz	r0,$000087C2

l0000879E:
B14D           	cbz	r5,$000087B4

l000087A0:
2400           	mov	r4,#0
4E09           	ldr	r6,[pc,#&24]                            ; 000087CE
4622           	mov	r2,r4
2100           	mov	r1,#0
3401           	adds	r4,#1
4630           	mov	r0,r6
F000 FB48     	bl	$00008E3C
42AC           	cmps	r4,r5
D1F7           	bne	$000089A0

l000087B4:
2200           	mov	r2,#0
E8BD 4070     	pop.w	{r4-r6,lr}
2101           	mov	r1,#1
4803           	ldr	r0,[pc,#&C]                             ; 000087D0
F000 BB3F     	b	$00C08E3C

l000087C2:
BD70           	pop	{r4-r6,pc}
000087C4             F8 07 00 20 E9 86 00 00 71 86 00 00     ... ....q...

;; xAreFlashCoRoutinesStillRunning: 000087D0
xAreFlashCoRoutinesStillRunning proc
4B01           	ldr	r3,[pc,#&4]                             ; 000087DC
6818           	ldr	r0,[r3]
4770           	bx	lr
000087D6                   00 BF C0 00 00 20                   .....    

;; MPU_xTaskCreateRestricted: 000087DC
MPU_xTaskCreateRestricted proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FEBF     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F896     	bl	$00000918
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000087FE

l000087F6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000087FE:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3

;; fn00008804: 00008804
fn00008804 proc
BD70           	pop	{r4-r6,pc}
00008806                   00 BF                               ..       

;; MPU_xTaskCreate: 00008808
MPU_xTaskCreate proc
E92D 47F0     	push.w	{r4-r10,lr}
B082           	sub	sp,#8
4605           	mov	r5,r0
4688           	mov	r8,r1
4691           	mov	r9,r2
469A           	mov	r10,r3
9F0A           	ldr	r7,[sp,#&28]
9E0B           	ldr	r6,[sp,#&2C]
F7FF FEA3     	bl	$00008560
4653           	mov	r3,r10
4604           	mov	r4,r0
9700           	str	r7,[sp]
9601           	str	r6,[sp,#&4]
464A           	mov	r2,r9
4641           	mov	r1,r8
4628           	mov	r0,r5
F7F8 F842     	bl	$000008B0
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$0000883E

l00008836:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l0000883E:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
B002           	add	sp,#8
E8BD 87F0     	pop.w	{r4-r10,pc}
0000884A                               00 BF                       ..   

;; MPU_vTaskAllocateMPURegions: 0000884C
MPU_vTaskAllocateMPURegions proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FE87     	bl	$00008560
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F888     	bl	$0000096C
2C01           	cmps	r4,#1
D005           	beq	$0000886C

l00008864:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l0000886C:
F380 8814     	msr	cpsr,r0

;; fn00008870: 00008870
fn00008870 proc
BD70           	pop	{r4-r6,pc}
00008872       00 BF                                       ..           

;; MPU_vTaskDelayUntil: 00008874
MPU_vTaskDelayUntil proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FE73     	bl	$00008560
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FB7C     	bl	$00000F7C
2C01           	cmps	r4,#1
D005           	beq	$00008894

;; fn0000888C: 0000888C
fn0000888C proc
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008894:
F380 8814     	msr	cpsr,r0
BD70           	pop	{r4-r6,pc}
0000889A                               00 BF                       ..   

;; MPU_vTaskDelay: 0000889C
MPU_vTaskDelay proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FE60     	bl	$00008560
4604           	mov	r4,r0

;; fn000088A6: 000088A6
fn000088A6 proc
4628           	mov	r0,r5
F7F8 FB4E     	bl	$00000F44
2C01           	cmps	r4,#1
D005           	beq	$000088B8

;; fn000088B0: 000088B0
fn000088B0 proc
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000088B8:
F380 8814     	msr	cpsr,r0

;; fn000088BC: 000088BC
fn000088BC proc
BD38           	pop	{r3-r5,pc}
000088BE                                           00 BF               ..

;; MPU_vTaskSuspendAll: 000088C0
MPU_vTaskSuspendAll proc
B510           	push	{r4,lr}
F7FF FE4F     	bl	$00008560
4604           	mov	r4,r0
F7F8 F8A0     	bl	$00000A08
2C01           	cmps	r4,#1
D005           	beq	$000088D8

l000088D0:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000088D8:
F380 8814     	msr	cpsr,r0
BD10           	pop	{r4,pc}
000088DE                                           00 BF               ..

;; MPU_xTaskResumeAll: 000088E0
MPU_xTaskResumeAll proc
B510           	push	{r4,lr}
F7FF FE3F     	bl	$00008560
4604           	mov	r4,r0
F7F8 FAC0     	bl	$00000E68
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000088FA

l000088F2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000088FA:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3

;; fn00008900: 00008900
fn00008900 proc
BD10           	pop	{r4,pc}
00008902       00 BF                                       ..           

;; MPU_xTaskGetTickCount: 00008904
MPU_xTaskGetTickCount proc
B510           	push	{r4,lr}
F7FF FE2D     	bl	$00008560

;; fn00008908: 00008908
fn00008908 proc
FE2D 4604     	Invalid
F7F8 F888     	bl	$00000A1C
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$0000891E

l00008916:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l0000891E:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
00008926                   00 BF                               ..       

;; MPU_uxTaskGetNumberOfTasks: 00008928
MPU_uxTaskGetNumberOfTasks proc
B510           	push	{r4,lr}
F7FF FE1B     	bl	$00008560
4604           	mov	r4,r0
F7F8 F882     	bl	$00000A34
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008942

l0000893A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008942:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
0000894A                               00 BF                       ..   

;; MPU_pcTaskGetName: 0000894C
MPU_pcTaskGetName proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FE08     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F874     	bl	$00000A40
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$0000896A

l00008962:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l0000896A:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008972       00 BF                                       ..           

;; MPU_vTaskSetTimeOutState: 00008974
MPU_vTaskSetTimeOutState proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FDF4     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FBE0     	bl	$00001140
2C01           	cmps	r4,#1
D005           	beq	$00008990

l00008988:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008990:
F380 8814     	msr	cpsr,r0
BD38           	pop	{r3-r5,pc}
00008996                   00 BF                               ..       

;; MPU_xTaskCheckForTimeOut: 00008998
MPU_xTaskCheckForTimeOut proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FDE1     	bl	$00008560

;; fn000089A0: 000089A0
fn000089A0 proc
FDE1 4631     	Invalid
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FBD6     	bl	$00001154
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000089BA

l000089B2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000089BA:
F380 8814     	msr	cpsr,r0
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
F7FF FDC8     	bl	$00008560
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F83B     	bl	$00000A54
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000089F0

l000089E8:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l000089F0:
F380 8814     	msr	cpsr,r0
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
F7FF FDAC     	bl	$00008560
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F8DD     	bl	$00000BD0
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008A28

l00008A20:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008A28:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008A32       00 BF                                       ..           

;; MPU_ulTaskNotifyTake: 00008A34
MPU_ulTaskNotifyTake proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FD93     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F95C     	bl	$00000CFC
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008A56

l00008A4E:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008A56:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008A5E                                           00 BF               ..

;; MPU_xTaskNotifyStateClear: 00008A60
MPU_xTaskNotifyStateClear proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FD7E     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7FF FD62     	bl	$00008530
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008A7E

l00008A76:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008A7E:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3

;; fn00008A84: 00008A84
fn00008A84 proc
BD38           	pop	{r3-r5,pc}
00008A86                   00 BF                               ..       

;; MPU_xQueueGenericCreate: 00008A88
MPU_xQueueGenericCreate proc
B5F8           	push	{r3-r7,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
F7FF FD68     	bl	$00008560
463A           	mov	r2,r7
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FE06     	bl	$000006A8
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008AAE

l00008AA6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008AAE:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BDF8           	pop	{r3-r7,pc}
00008AB6                   00 BF                               ..       

;; MPU_xQueueGenericReset: 00008AB8
MPU_xQueueGenericReset proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FD51     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FDB2     	bl	$0000062C
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008ADA

l00008AD2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008ADA:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3

;; fn00008AE0: 00008AE0
fn00008AE0 proc
BD70           	pop	{r4-r6,pc}
00008AE2       00 BF                                       ..           

;; MPU_xQueueGenericSend: 00008AE4
MPU_xQueueGenericSend proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FD38     	bl	$00008560
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FB47     	bl	$0000018C
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B10

l00008B08:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008B10:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008B1A                               00 BF                       ..   

;; MPU_uxQueueMessagesWaiting: 00008B1C
MPU_uxQueueMessagesWaiting proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FD20     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FC7E     	bl	$00000424
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B3A

l00008B32:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008B3A:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008B42       00 BF                                       ..           

;; MPU_uxQueueSpacesAvailable: 00008B44
MPU_uxQueueSpacesAvailable proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FD0C     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FC74     	bl	$00000438
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B62

l00008B5A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008B62:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3

;; fn00008B68: 00008B68
fn00008B68 proc
BD38           	pop	{r3-r5,pc}
00008B6A                               00 BF                       ..   

;; MPU_xQueueGenericReceive: 00008B6C
MPU_xQueueGenericReceive proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FCF4     	bl	$00008560
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FBA7     	bl	$000002D4
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B98

l00008B90:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008B98:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008BA2       00 BF                                       ..           

;; MPU_xQueuePeekFromISR: 00008BA4
MPU_xQueuePeekFromISR proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FCDB     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FB76     	bl	$000002A0
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008BC6

l00008BBE:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008BC6:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008BCE                                           00 BF               ..

;; MPU_xQueueGetMutexHolder: 00008BD0
MPU_xQueueGetMutexHolder proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FCC6     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCEA     	bl	$000005B0
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008BEE

l00008BE6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008BEE:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008BF6                   00 BF                               ..       

;; MPU_xQueueCreateMutex: 00008BF8
MPU_xQueueCreateMutex proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FCB2     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FD6A     	bl	$000006D8
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008C16

l00008C0E:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008C16:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008C1E                                           00 BF               ..

;; MPU_xQueueTakeMutexRecursive: 00008C20
MPU_xQueueTakeMutexRecursive proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FC9D     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCD0     	bl	$000005D0
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008C42

l00008C3A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008C42:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008C4A                               00 BF                       ..   

;; MPU_xQueueGiveMutexRecursive: 00008C4C
MPU_xQueueGiveMutexRecursive proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC88     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCD4     	bl	$00000600
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008C6A

l00008C62:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008C6A:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008C72       00 BF                                       ..           

;; MPU_vQueueDelete: 00008C74
MPU_vQueueDelete proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC74     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FBE8     	bl	$00000450
2C01           	cmps	r4,#1
D005           	beq	$00008C90

l00008C88:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008C90:
F380 8814     	msr	cpsr,r0
BD38           	pop	{r3-r5,pc}
00008C96                   00 BF                               ..       

;; MPU_pvPortMalloc: 00008C98
MPU_pvPortMalloc proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC62     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD42     	bl	$00001728
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008CB6

l00008CAE:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008CB6:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008CBE                                           00 BF               ..

;; MPU_vPortFree: 00008CC0
MPU_vPortFree proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC4E     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD58     	bl	$0000177C
2C01           	cmps	r4,#1
D005           	beq	$00008CDC

l00008CD4:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008CDC:
F380 8814     	msr	cpsr,r0
BD38           	pop	{r3-r5,pc}
00008CE2       00 BF                                       ..           

;; MPU_vPortInitialiseBlocks: 00008CE4
MPU_vPortInitialiseBlocks proc
B510           	push	{r4,lr}
F7FF FC3D     	bl	$00008560
4604           	mov	r4,r0
F7F8 FD4A     	bl	$00001780
2C01           	cmps	r4,#1
D005           	beq	$00008CFC

l00008CF4:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008CFC:
F380 8814     	msr	cpsr,r0
BD10           	pop	{r4,pc}
00008D02       00 BF                                       ..           

;; MPU_xPortGetFreeHeapSize: 00008D04
MPU_xPortGetFreeHeapSize proc
B510           	push	{r4,lr}
F7FF FC2D     	bl	$00008560
4604           	mov	r4,r0
F7F8 FD42     	bl	$00001790
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008D1E

l00008D16:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008D1E:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
00008D26                   00 BF                               ..       

;; MPU_xEventGroupCreate: 00008D28
MPU_xEventGroupCreate proc
B510           	push	{r4,lr}
F7FF FC1B     	bl	$00008560
4604           	mov	r4,r0
F7F8 FD3A     	bl	$000017A4
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008D42

l00008D3A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008D42:
F380 8814     	msr	cpsr,r0
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
F7FF FC02     	bl	$00008560
464B           	mov	r3,r9
4604           	mov	r4,r0
9700           	str	r7,[sp]
4642           	mov	r2,r8
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FD2A     	bl	$000017C0
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008D7E

l00008D76:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008D7E:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
B003           	add	sp,#&C
E8BD 83F0     	pop.w	{r4-r9,pc}
00008D8A                               00 BF                       ..   

;; MPU_xEventGroupClearBits: 00008D8C
MPU_xEventGroupClearBits proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FBE7     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD6A     	bl	$00001870
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008DAE

l00008DA6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008DAE:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008DB6                   00 BF                               ..       

;; MPU_xEventGroupSetBits: 00008DB8
MPU_xEventGroupSetBits proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FBD1     	bl	$00008560
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD62     	bl	$0000188C
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008DDA

l00008DD2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008DDA:
F380 8814     	msr	cpsr,r0
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
F7FF FBB8     	bl	$00008560
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FD7B     	bl	$000018F4
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008E10

l00008E08:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008E10:
F380 8814     	msr	cpsr,r0
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008E1A                               00 BF                       ..   

;; MPU_vEventGroupDelete: 00008E1C
MPU_vEventGroupDelete proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FBA0     	bl	$00008560
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FDBC     	bl	$000019A0
2C01           	cmps	r4,#1
D005           	beq	$00008E38

l00008E30:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1

l00008E38:
F380 8814     	msr	cpsr,r0

;; fn00008E3C: 00008E3C
fn00008E3C proc
BD38           	pop	{r3-r5,pc}
00008E3E                                           00 BF               ..

;; xCoRoutineCreate: 00008E40
xCoRoutineCreate proc
E92D 4FF8     	push.w	{r3-fp,lr}
4681           	mov	r9,r0
2038           	mov	r0,#&38
460D           	mov	r5,r1
4692           	mov	r10,r2
F7F8 FC6E     	bl	$00001728
2800           	cmps	r0,#0
D047           	beq	$00008EE0

l00008E54:
4F25           	ldr	r7,[pc,#&94]                            ; 00008EF0
4604           	mov	r4,r0
683B           	ldr	r3,[r7]
B33B           	cbz	r3,$00008EAC

l00008E5C:
F107 0804     	add	r8,r7,#4
2D01           	cmps	r5,#1
BF28           	it	hs
2501           	movhs	r5,#1

l00008E66:
2300           	mov	r3,#0
4626           	mov	r6,r4
86A3           	strh	r3,[r4,#&68]
62E5           	str	r5,[r4,#&2C]
F8C4 A030     	str	r10,[r4,#&30]
F846 9B04     	str	r9,[r6],#&4
4630           	mov	r0,r6
F7FF FA36     	bl	$000082E4
F104 0018     	add	r0,r4,#&18
F7FF FA32     	bl	$000082E4
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
F7FF FA25     	bl	$000082EC
2001           	mov	r0,#1
E8BD 8FF8     	pop.w	{r3-fp,pc}

l00008EAC:
46B8           	mov	r8,r7
F848 0B04     	str	r0,[r8],#&4
4640           	mov	r0,r8
F7FF FA0C     	bl	$000082CC
F107 0B2C     	add	fp,r7,#&2C
F107 0018     	add	r0,r7,#&18
F7FF FA06     	bl	$000082CC
F107 0640     	add	r6,r7,#&40
4658           	mov	r0,fp
F7FF FA01     	bl	$000082CC
4630           	mov	r0,r6
F7FF F9FE     	bl	$000082CC
F107 0054     	add	r0,r7,#&54
F7FF F9FA     	bl	$000082CC
F8C7 B068     	str	fp,[r7,#&68]

l00008EE0:
66FE           	str	r6,[r7,#&6C]
E7BD           	b	$00008E5C
00008EE4             4F F0 FF 30 BD E8 F8 8F                 O..0....   

;; fn00008EEC: 00008EEC
fn00008EEC proc
07FC           	lsls	r4,r7,#&1F
2000           	mov	r0,#0

;; vCoRoutineAddToDelayedList: 00008EF0
vCoRoutineAddToDelayedList proc
B570           	push	{r4-r6,lr}
460E           	mov	r6,r1
4C0C           	ldr	r4,[pc,#&30]                            ; 00008F2C
6823           	ldr	r3,[r4]
6F65           	ldr	r5,[r4,#&74]
4405           	adds	r5,r0
1D18           	add	r0,r3,#4
F7FF FA1F     	bl	$0000833C
6F63           	ldr	r3,[r4,#&74]
6821           	ldr	r1,[r4]
429D           	cmps	r5,r3
604D           	str	r5,[r1,#&4]
BF34           	ite	lo
6EE0           	ldrlo	r0,[r4,#&6C]

l00008F0E:
6EA0           	ldr	r0,[r4,#&68]
3104           	adds	r1,#4
F7FF F9FB     	bl	$00008308
B136           	cbz	r6,$00008F26

l00008F18:
6821           	ldr	r1,[r4]
4630           	mov	r0,r6
E8BD 4070     	pop.w	{r4-r6,lr}
3118           	adds	r1,#&18
F7FF B9F3     	b	$00008308

l00008F26:
BD70           	pop	{r4-r6,pc}

;; fn00008F28: 00008F28
fn00008F28 proc
07FC           	lsls	r4,r7,#&1F
2000           	mov	r0,#0

;; vCoRoutineSchedule: 00008F2C
vCoRoutineSchedule proc
E92D 41F0     	push.w	{r4-r8,lr}
4D55           	ldr	r5,[pc,#&154]                           ; 0000908C
6D6B           	ldr	r3,[r5,#&54]
B32B           	cbz	r3,$00008F82

l00008F36:
2700           	mov	r7,#0
F105 0804     	add	r8,r5,#4
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6E2B           	ldr	r3,[r5,#&60]
68DC           	ldr	r4,[r3,#&C]
F104 0018     	add	r0,r4,#&18
F7FF F9F4     	bl	$0000833C
F387 8811     	msr	cpsr,r7
1D26           	add	r6,r4,#4
4630           	mov	r0,r6
F7FF F9EE     	bl	$0000833C
6AE3           	ldr	r3,[r4,#&2C]
6F2A           	ldr	r2,[r5,#&70]
EB03 0083     	add.w	r0,r3,r3,lsl #2
4293           	cmps	r3,r2
4631           	mov	r1,r6
EB08 0080     	add.w	r0,r8,r0,lsl #2
BF88           	it	hi
672B           	strhi	r3,[r5,#&70]

l00008F78:
F7FF F9BA     	bl	$000082EC
6D6B           	ldr	r3,[r5,#&54]
2B00           	cmps	r3,#0
D1DC           	bne	$00009138

l00008F82:
F7FF FCBF     	bl	$00008900
2700           	mov	r7,#0
6FAA           	ldr	r2,[r5,#&78]
6F6B           	ldr	r3,[r5,#&74]
1A80           	sub	r0,r0,r2
F8DF 8100     	ldr	r8,[pc,#&100]                            ; 00009096

l00008F90:
8100           	strh	r0,[r0,#&10]
67E8           	str	r0,[r5,#&7C]
2800           	cmps	r0,#0
D03D           	beq	$00009010

l00008F98:
3301           	adds	r3,#1
3801           	subs	r0,#1
676B           	str	r3,[r5,#&74]
67E8           	str	r0,[r5,#&7C]
2B00           	cmps	r3,#0
D053           	beq	$00009048

l00008FA4:
6EAA           	ldr	r2,[r5,#&68]
6811           	ldr	r1,[r2]
2900           	cmps	r1,#0
D0F3           	beq	$00009190

l00008FAC:
68D2           	ldr	r2,[r2,#&C]
68D4           	ldr	r4,[r2,#&C]
6862           	ldr	r2,[r4,#&4]
4293           	cmps	r3,r2
D206           	bhs	$00008FC0

l00008FB6:
E7ED           	b	$00008F90
00008FB8                         DA 68 6B 6F D4 68 62 68         .hko.hbh

l00008FC0:
429A           	cmps	r2,r3
D824           	bhi	$0000900A

l00008FC4:
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
1D26           	add	r6,r4,#4
4630           	mov	r0,r6
F7FF F9B2     	bl	$0000833C
6AA3           	ldr	r3,[r4,#&28]
F104 0018     	add	r0,r4,#&18
B10B           	cbz	r3,$00008FE8

l00008FE4:
F7FF F9AC     	bl	$0000833C

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
F7FF F976     	bl	$000082EC
6EAB           	ldr	r3,[r5,#&68]
681A           	ldr	r2,[r3]
2A00           	cmps	r2,#0

l0000900A:
D1D5           	bne	$000091B4

l0000900C:
6F6B           	ldr	r3,[r5,#&74]
6FE8           	ldr	r0,[r5,#&7C]

l00009010:
2800           	cmps	r0,#0
D1C1           	bne	$00009194

l00009014:
6F29           	ldr	r1,[r5,#&70]
67AB           	str	r3,[r5,#&78]
008B           	lsls	r3,r1,#2
185A           	add	r2,r3,r1
EB05 0282     	add.w	r2,r5,r2,lsl #2
6852           	ldr	r2,[r2,#&4]
2A00           	cmps	r2,#0
D12E           	bne	$00009080

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

l00009048:
E8BD 81F0     	pop.w	{r4-r8,pc}
0000904C                                     A9 6E EA 6E             .n.n
00009050 E9 66 AA 66 A7 E7                               .f.f..         

l00009056:
672A           	str	r2,[r5,#&70]
4413           	adds	r3,r2
009B           	lsls	r3,r3,#2
18E9           	add	r1,r5,r3
688A           	ldr	r2,[r1,#&8]
480A           	ldr	r0,[pc,#&28]                            ; 00009090
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
00009084             0A 46 E7 E7 FC 07 00 20 08 08 00 20     .F..... ... 

;; fn00009090: 00009090
fn00009090 proc
0800           	movs	r0,r0,#0
2000           	mov	r0,#0

;; xCoRoutineRemoveFromEventList: 00009094
xCoRoutineRemoveFromEventList proc
68C3           	ldr	r3,[r0,#&C]
B570           	push	{r4-r6,lr}
68DC           	ldr	r4,[r3,#&C]
4D09           	ldr	r5,[pc,#&24]                            ; 000090C6
F104 0618     	add	r6,r4,#&18
4630           	mov	r0,r6
F7FF F94D     	bl	$0000833C
F105 0054     	add	r0,r5,#&54
4631           	mov	r1,r6
F7FF F920     	bl	$000082EC
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
4B0F           	ldr	r3,[pc,#&3C]                            ; 00009108
4298           	cmps	r0,r3
D019           	beq	$000090FA

l000090CA:
D808           	bhi	$000090DA

l000090CC:
F1B0 2F40     	cmp	r0,#&40004000
D013           	beq	$000090F6

l000090D2:
F5A3 5380     	sub	r3,r3,#&1000
4298           	cmps	r0,r3
D10A           	bne	$000090EC

l000090DA:
2011           	mov	r0,#&11
4770           	bx	lr
000090DE                                           0A 4B               .K
000090E0 98 42 08 D0 03 F5 E8 33 98 42 01 D1             .B.....3.B..   

l000090EC:
2014           	mov	r0,#&14
4770           	bx	lr
000090F0 4F F0 FF 30 70 47                               O..0pG         

l000090F6:
2013           	mov	r0,#&13
4770           	bx	lr

l000090FA:
2010           	mov	r0,#&10
4770           	bx	lr
000090FE                                           12 20               . 
00009100 70 47 00 BF 00 60 00 40                         pG...`.@       

;; fn00009108: 00009108
fn00009108 proc
7000           	strb	r0,[r0]
4000           	ands	r0,r0

;; GPIODirModeSet: 0000910C
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

;; fn00009138: 00009138
fn00009138 proc
FA03 F101     	lsl	r1,r3,r1
F8D0 4400     	ldr	r4,[r0,#&400]
B2C9           	uxtb	r1,r1
F8D0 2420     	ldr	r2,[r0,#&420]
420C           	adcs	r4,r1
BF08           	it	eq
2300           	moveq	r3,#0

;; fn0000914C: 0000914C
fn0000914C proc
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

l00009190:
140C           	asrss	r4,r1,#&10
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

;; fn000091B4: 000091B4
fn000091B4 proc
2302           	mov	r3,#2

;; fn000091B6: 000091B6
fn000091B6 proc
2300           	mov	r3,#0
4208           	adcs	r0,r1
BF14           	ite	ne
2004           	movne	r0,#4

l000091BE:
2000           	mov	r0,#0
4313           	orrs	r3,r2
4318           	orrs	r0,r3

;; fn000091C4: 000091C4
fn000091C4 proc
4770           	bx	lr
000091C6                   00 BF                               ..       

;; GPIOPadConfigSet: 000091C8
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
4B24           	ldr	r3,[pc,#&90]                            ; 000093AC
B510           	push	{r4,lr}
4298           	cmps	r0,r3
D03C           	beq	$00009392

l0000931C:
D80F           	bhi	$0000933A

l0000931E:
F1B0 2F40     	cmp	r0,#&40004000
D02F           	beq	$00009380

l00009324:
F5A3 5380     	sub	r3,r3,#&1000
4298           	cmps	r0,r3
D118           	bne	$0000935A

l0000932C:
2411           	mov	r4,#&11
4620           	mov	r0,r4
F000 F8E8     	bl	$00009500
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}

l0000933A:
F000 B94F     	b	$00C095D8
0000933E                                           1B 4B               .K
00009340 98 42 16 D0 03 F5 E8 33 98 42 08 D1 14 24 20 46 .B.....3.B...$ F
00009350 00 F0 D8 F8 20 46 BD E8 10 40                   .... F...@     

l0000935A:
F000 B93F     	b	$00C095D8
0000935E                                           4F F0               O.
00009360 FF 34 20 46 00 F0 CE F8 20 46 BD E8 10 40 00 F0 .4 F.... F...@..
00009370 35 B9 13 24 20 46 00 F0 C5 F8 20 46 BD E8 10 40 5..$ F.... F...@

l00009380:
F000 B92C     	b	$00C095D8
00009384             10 24 20 46 00 F0 BC F8 20 46 BD E8     .$ F.... F..
00009390 10 40                                           .@             

l00009392:
F000 B923     	b	$00C095D8
00009396                   12 24 20 46 00 F0 B3 F8 20 46       .$ F.... F
000093A0 BD E8 10 40 00 F0 1A B9 00 60 00 40 00 70 00 40 ...@.....`.@.p.@

;; GPIOPortIntUnregister: 000093B0
GPIOPortIntUnregister proc
4B24           	ldr	r3,[pc,#&90]                            ; 00009448
B510           	push	{r4,lr}
4298           	cmps	r0,r3
D03C           	beq	$0000942E

l000093B8:
D80F           	bhi	$000093D6

l000093BA:
F1B0 2F40     	cmp	r0,#&40004000
D02F           	beq	$0000941C

l000093C0:
F5A3 5380     	sub	r3,r3,#&1000
4298           	cmps	r0,r3
D118           	bne	$000093F6

l000093C8:
2411           	mov	r4,#&11
4620           	mov	r0,r4
F000 F934     	bl	$00009634
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}

l000093D6:
F000 B8AF     	b	$00C09534
000093DA                               1B 4B 98 42 16 D0           .K.B..
000093E0 03 F5 E8 33 98 42 08 D1 14 24 20 46 00 F0 24 F9 ...3.B...$ F..$.
000093F0 20 46 BD E8 10 40                                F...@         

l000093F6:
F000 B89F     	b	$00C09534
000093FA                               4F F0 FF 34 20 46           O..4 F
00009400 00 F0 1A F9 20 46 BD E8 10 40 00 F0 95 B8 13 24 .... F...@.....$
00009410 20 46 00 F0 11 F9 20 46 BD E8 10 40              F.... F...@   

l0000941C:
F000 B88C     	b	$00C09534
00009420 10 24 20 46 00 F0 08 F9 20 46 BD E8 10 40       .$ F.... F...@ 

l0000942E:
F000 B883     	b	$00C09534
00009432       12 24 20 46 00 F0 FF F8 20 46 BD E8 10 40   .$ F.... F...@
00009440 00 F0 7A B8 00 60 00 40 00 70 00 40             ..z..`.@.p.@   

;; GPIOPinRead: 0000944C
GPIOPinRead proc
F850 0021     	ldr.w	r0,[r0,r1,lsl #2]

;; fn00009450: 00009450
fn00009450 proc
4770           	bx	lr
00009452       00 BF                                       ..           

;; GPIOPinWrite: 00009454
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
F7FF BEA5     	b	$000091C4

;; fn0000947C: 0000947C
fn0000947C proc
BEA5           	bkpt
0000947E                                           00 BF               ..

;; GPIOPinTypeI2C: 00009480
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
F7FF BE92     	b	$000091C4

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

;; fn000094C4: 000094C4
fn000094C4 proc
F7FF BE80     	b	$000091C4

;; GPIOPinTypeUART: 000094C8
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
F7FF BE6E     	b	$000091C4

;; GPIOPinTypeTimer: 000094EC
GPIOPinTypeTimer proc
F7FF BFEC     	b	$000094C4

;; GPIOPinTypeSSI: 000094F0
GPIOPinTypeSSI proc
F7FF BFEA     	b	$000094C4

;; GPIOPinTypePWM: 000094F4
GPIOPinTypePWM proc
F7FF BFE8     	b	$000094C4

;; IntDefaultHandler: 000094F8
IntDefaultHandler proc
E7FE           	b	$000094F4
000094FA                               00 BF                       ..   

;; IntMasterEnable: 000094FC
IntMasterEnable proc
F000 BDEE     	b	$00C0A0D8

;; IntMasterDisable: 00009500
IntMasterDisable proc
F000 BDF0     	b	$00C0A0E0

;; IntRegister: 00009504
IntRegister proc
4B0A           	ldr	r3,[pc,#&28]                            ; 00009534
B430           	push	{r4-r5}
681B           	ldr	r3,[r3]
4C0A           	ldr	r4,[pc,#&28]                            ; 0000953A
42A3           	cmps	r3,r4
D00A           	beq	$00009522

l00009510:
4623           	mov	r3,r4
F104 05B8     	add	r5,r4,#&B8
1B1A           	sub	r2,r3,r4
6812           	ldr	r2,[r2]
F843 2B04     	str	r2,[r3],#&4
42AB           	cmps	r3,r5
D1F9           	bne	$00009712

l00009522:
4B03           	ldr	r3,[pc,#&C]                             ; 00009536
601C           	str	r4,[r3]
F844 1020     	str.w	r1,[r4,r0,lsl #2]
BC30           	pop	{r4-r5}
4770           	bx	lr
0000952E                                           00 BF               ..
00009530 08 ED 00 E0                                     ....           

;; fn00009534: 00009534
fn00009534 proc
0000           	mov	r0,r0
2000           	mov	r0,#0

;; IntUnregister: 00009538
IntUnregister proc
4B02           	ldr	r3,[pc,#&8]                             ; 00009548
4A03           	ldr	r2,[pc,#&C]                             ; 0000954E
F843 2020     	str.w	r2,[r3,r0,lsl #2]
4770           	bx	lr
00009542       00 BF 00 00 00 20 F9 94 00 00               ..... ....   

;; IntPriorityGroupingSet: 0000954C
IntPriorityGroupingSet proc
4B04           	ldr	r3,[pc,#&10]                            ; 00009564
4A05           	ldr	r2,[pc,#&14]                            ; 0000956A
F853 3020     	ldr.w	r3,[r3,r0,lsl #2]
F043 63BF     	orr	r3,r3,#&5F80000
F443 3300     	orr	r3,r3,#&20000
6013           	str	r3,[r2]
4770           	bx	lr
00009560 A4 A2 00 00 0C ED 00 E0                         ........       

;; IntPriorityGroupingGet: 00009568
IntPriorityGroupingGet proc
F44F 63E0     	mov	r3,#&700
4906           	ldr	r1,[pc,#&18]                            ; 0000958C
2000           	mov	r0,#0
6809           	ldr	r1,[r1]
4A06           	ldr	r2,[pc,#&18]                            ; 00009592
4019           	ands	r1,r3
E001           	b	$00009578

l00009578:
F852 3B04     	ldr	r3,[r2],#&4
428B           	cmps	r3,r1
D002           	beq	$00009582

l00009580:
3001           	adds	r0,#1

l00009582:
2808           	cmps	r0,#8
D1F8           	bne	$00009774

l00009586:
4770           	bx	lr
00009588                         0C ED 00 E0 A8 A2 00 00         ........

;; IntPrioritySet: 00009590
IntPrioritySet proc
22FF           	mov	r2,#&FF
4B09           	ldr	r3,[pc,#&24]                            ; 000095BE
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
4B06           	ldr	r3,[pc,#&18]                            ; 000095DC
F020 0203     	bic	r2,r0,#3
4413           	adds	r3,r2
6A1B           	ldr	r3,[r3,#&20]
F000 0003     	and	r0,r0,#3
681B           	ldr	r3,[r3]
00C0           	lsls	r0,r0,#3
FA23 F000     	lsr	r0,r3,r0
B2C0           	uxtb	r0,r0
4770           	bx	lr
000095D6                   00 BF                               ..       

;; fn000095D8: 000095D8
fn000095D8 proc
A2A4           	adr	r2,$00009868
0000           	mov	r0,r0

;; IntEnable: 000095DC
IntEnable proc
2804           	cmps	r0,#4
D013           	beq	$00009604

l000095E0:
2805           	cmps	r0,#5
D017           	beq	$00009610

l000095E4:
2806           	cmps	r0,#6
D01B           	beq	$0000961C

l000095E8:
280F           	cmps	r0,#&F
D007           	beq	$000095F8

l000095EC:
D905           	bls	$000095F6

l000095EE:
2301           	mov	r3,#1
3810           	subs	r0,#&10
4A0E           	ldr	r2,[pc,#&38]                            ; 00009632
FA03 F000     	lsl	r0,r3,r0

l000095F6:
F000 6010     	and	r0,r0,#&9000000

l000095F8:
6010           	str	r0,[r2]

l000095FA:
4770           	bx	lr
000095FC                                     0C 4A 13 68             .J.h
00009600 43 F0 02 03                                     C...           

l00009604:
6013           	str	r3,[r2]
4770           	bx	lr
00009608                         0A 4A 13 68 43 F4 80 33         .J.hC..3

l00009610:
6013           	str	r3,[r2]
4770           	bx	lr
00009614             07 4A 13 68 43 F4 00 33                 .J.hC..3   

l0000961C:
6013           	str	r3,[r2]
4770           	bx	lr
00009620 04 4A 13 68 43 F4 80 23 13 60 70 47 00 E1 00 E0 .J.hC..#.`pG....
00009630 10 E0 00 E0                                     ....           

;; fn00009634: 00009634
fn00009634 proc
ED24 E000     	Invalid

;; IntDisable: 00009638
IntDisable proc
2804           	cmps	r0,#4
D013           	beq	$00009660

l0000963C:
2805           	cmps	r0,#5
D017           	beq	$0000966C

l00009640:
2806           	cmps	r0,#6
D01B           	beq	$00009678

l00009644:
280F           	cmps	r0,#&F
D007           	beq	$00009654

l00009648:
D905           	bls	$00009652

l0000964A:
2301           	mov	r3,#1
3810           	subs	r0,#&10
4A0E           	ldr	r2,[pc,#&38]                            ; 0000968E
FA03 F000     	lsl	r0,r3,r0

l00009652:
F000 6010     	and	r0,r0,#&9000000

l00009654:
6010           	str	r0,[r2]

l00009656:
4770           	bx	lr
00009658                         0C 4A 13 68 23 F0 02 03         .J.h#...

l00009660:
6013           	str	r3,[r2]
4770           	bx	lr
00009664             0A 4A 13 68 23 F4 80 33                 .J.h#..3   

l0000966C:
6013           	str	r3,[r2]
4770           	bx	lr
00009670 07 4A 13 68 23 F4 00 33                         .J.h#..3       

l00009678:
6013           	str	r3,[r2]
4770           	bx	lr
0000967C                                     04 4A 13 68             .J.h
00009680 23 F4 80 23 13 60 70 47 80 E1 00 E0 10 E0 00 E0 #..#.`pG........

;; fn00009690: 00009690
fn00009690 proc
ED24 E000     	Invalid

;; OSRAMDelay: 00009694
OSRAMDelay proc
3801           	subs	r0,#1
D1FD           	bne	$00009890

;; fn00009698: 00009698
fn00009698 proc
4770           	bx	lr
0000969A                               00 BF                       ..   

;; OSRAMWriteFirst: 0000969C
OSRAMWriteFirst proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
4C07           	ldr	r4,[pc,#&1C]                            ; 000096C4
2200           	mov	r2,#0
4620           	mov	r0,r4
213D           	mov	r1,#&3D
F000 FDAE     	bl	$0000A204
4629           	mov	r1,r5
4620           	mov	r0,r4
F000 FDC4     	bl	$0000A238
4620           	mov	r0,r4
E8BD 4038     	pop.w	{r3-r5,lr}
2103           	mov	r1,#3
F000 BDB0     	b	$00C0A21C

;; fn000096C0: 000096C0
fn000096C0 proc
0000           	mov	r0,r0
4002           	ands	r2,r0

;; OSRAMWriteArray: 000096C4
OSRAMWriteArray proc
B1C9           	cbz	r1,$000096FA

l000096C6:
B5F8           	push	{r3-r7,lr}
4605           	mov	r5,r0
4F0C           	ldr	r7,[pc,#&30]                            ; 00009702
4C0C           	ldr	r4,[pc,#&30]                            ; 00009704
1846           	add	r6,r0,r1
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD78     	bl	$0000A1C4
2800           	cmps	r0,#0
D0F9           	beq	$000098CC

l000096DC:
6838           	ldr	r0,[r7]
F7FF FFD9     	bl	$00009690
F815 1B01     	ldrb	r1,[r5],#&1
4620           	mov	r0,r4
F000 FDA8     	bl	$0000A238
2101           	mov	r1,#1
4620           	mov	r0,r4
F000 FD96     	bl	$0000A21C
42AE           	cmps	r6,r5
D1EB           	bne	$000098CC

l000096F8:
BDF8           	pop	{r3-r7,pc}

l000096FA:
4770           	bx	lr
000096FC                                     7C 08 00 20             |.. 

;; fn00009700: 00009700
fn00009700 proc
0000           	mov	r0,r0
4002           	ands	r2,r0

;; OSRAMWriteByte: 00009704
OSRAMWriteByte proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
2100           	mov	r1,#0
4809           	ldr	r0,[pc,#&24]                            ; 00009736
F000 FD5C     	bl	$0000A1C4
2800           	cmps	r0,#0

;; fn00009712: 00009712
fn00009712 proc
D0F9           	beq	$00009904

l00009714:
4B07           	ldr	r3,[pc,#&1C]                            ; 00009738
6818           	ldr	r0,[r3]
F7FF FFBC     	bl	$00009690
4621           	mov	r1,r4
4804           	ldr	r0,[pc,#&10]                            ; 00009736
F000 FD8C     	bl	$0000A238
E8BD 4010     	pop.w	{r4,lr}
2101           	mov	r1,#1
4801           	ldr	r0,[pc,#&4]                             ; 00009736
F000 BD78     	b	$00C0A21C
00009730 00 00 02 40                                     ...@           

;; fn00009734: 00009734
fn00009734 proc
087C           	lsrs	r4,r7,#1
2000           	mov	r0,#0

;; OSRAMWriteFinal: 00009738
OSRAMWriteFinal proc
B570           	push	{r4-r6,lr}
4606           	mov	r6,r0
4C0E           	ldr	r4,[pc,#&38]                            ; 0000977C
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD41     	bl	$0000A1C4
2800           	cmps	r0,#0
D0F9           	beq	$0000993A

l0000974A:
4D0C           	ldr	r5,[pc,#&30]                            ; 00009782
4C0A           	ldr	r4,[pc,#&28]                            ; 0000977C
6828           	ldr	r0,[r5]
F7FF FFA0     	bl	$00009690
4631           	mov	r1,r6
4620           	mov	r0,r4
F000 FD70     	bl	$0000A238
2105           	mov	r1,#5
4620           	mov	r0,r4
F000 FD5E     	bl	$0000A21C
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD2E     	bl	$0000A1C4
2800           	cmps	r0,#0
D0F9           	beq	$00009960

l00009770:
6828           	ldr	r0,[r5]
E8BD 4070     	pop.w	{r4-r6,lr}

;; fn00009774: 00009774
fn00009774 proc
4070           	eors	r0,r6
E78D           	b	$00009690
00009778                         00 00 02 40                     ...@   

;; fn0000977C: 0000977C
fn0000977C proc
087C           	lsrs	r4,r7,#1
2000           	mov	r0,#0

;; OSRAMClear: 00009780
OSRAMClear proc
B510           	push	{r4,lr}
2080           	mov	r0,#&80
F7FF FF8A     	bl	$00009698
2106           	mov	r1,#6
480E           	ldr	r0,[pc,#&38]                            ; 000097CA
F7FF FF9A     	bl	$000096C0
245F           	mov	r4,#&5F
2000           	mov	r0,#0
F7FF FFB6     	bl	$00009700
3C01           	subs	r4,#1
D1FA           	bne	$0000998E

l0000979C:
4620           	mov	r0,r4
F7FF FFCB     	bl	$00009734
2080           	mov	r0,#&80
F7FF FF7A     	bl	$00009698
2106           	mov	r1,#6
4807           	ldr	r0,[pc,#&1C]                            ; 000097CE
F7FF FF8A     	bl	$000096C0
245F           	mov	r4,#&5F
2000           	mov	r0,#0
F7FF FFA6     	bl	$00009700
3C01           	subs	r4,#1
D1FA           	bne	$000099AE

l000097BC:
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
E7B9           	b	$00009734
000097C4             F4 A2 00 00                             ....       

;; fn000097C8: 000097C8
fn000097C8 proc
A2FC           	adr	r2,$00009BB8
0000           	mov	r0,r0

;; OSRAMStringDraw: 000097CC
OSRAMStringDraw proc
B570           	push	{r4-r6,lr}
4616           	mov	r6,r2
460C           	mov	r4,r1
4605           	mov	r5,r0
2080           	mov	r0,#&80
F7FF FF61     	bl	$00009698
2E00           	cmps	r6,#0
BF0C           	ite	eq
20B0           	moveq	r0,#&B0

l000097E0:
20B1           	mov	r0,#&B1
F7FF FF8F     	bl	$00009700
F104 0624     	add	r6,r4,#&24
2080           	mov	r0,#&80
F7FF FF8A     	bl	$00009700
F006 000F     	and	r0,r6,#&F
F7FF FF86     	bl	$00009700
2080           	mov	r0,#&80
F7FF FF83     	bl	$00009700
F3C6 1003     	ubfx	r0,r6,#4,#4
F040 0010     	orr	r0,r0,#&10
F7FF FF7D     	bl	$00009700
2040           	mov	r0,#&40
F7FF FF7A     	bl	$00009700
782B           	ldrb	r3,[r5]
B383           	cbz	r3,$00009876

l00009814:
2C5A           	cmps	r4,#&5A
4E18           	ldr	r6,[pc,#&60]                            ; 0000987E
D90A           	bls	$0000982C

l0000981A:
E017           	b	$00009848
0000981C                                     15 F8 01 3F             ...?
00009820 06 34 83 B1 FF F7 6E FF 2B 78 1B B3             .4....n.+x..   

l0000982C:
2C5A           	cmps	r4,#&5A
D80D           	bhi	$00009848

l00009830:
3B20           	subs	r3,#&20
EB03 0383     	add.w	r3,r3,r3,lsl #2
18F0           	add	r0,r6,r3
2105           	mov	r1,#5
F7FF FF43     	bl	$000096C0
2C5A           	cmps	r4,#&5A
F04F 0000     	mov	r0,#0
D1EA           	bne	$00009A18

l00009846:
E8BD 4070     	pop.w	{r4-r6,lr}

l00009848:
4070           	eors	r0,r6

l0000984A:
E775           	b	$00009734
0000984C                                     20 3B 03 EB              ;..
00009850 83 03 C4 F1 5F 04 F0 18 21 46 FF F7 33 FF 2B 78 ...._...!F..3.+x
00009860 06 4A 20 3B 03 EB 83 03 13 44 23 44 18 7C BD E8 .J ;.....D#D.|..
00009870 70 40 61 E7 70 BD                               p@a.p.         

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

;; fn00009890: 00009890
fn00009890 proc
F3C1 1803     	ubfx	r8,r1,#4,#4
4416           	adds	r6,r2
F048 0810     	orr	r8,r8,#&10
F001 070F     	and	r7,r1,#&F
F103 3AFF     	add	r10,r3,#&FFFFFFFF
2080           	mov	r0,#&80
F7FF FEFA     	bl	$00009698
2C00           	cmps	r4,#0
BF14           	ite	ne
20B1           	movne	r0,#&B1

;; fn000098AE: 000098AE
fn000098AE proc
20B0           	mov	r0,#&B0
F7FF FF28     	bl	$00009700
2080           	mov	r0,#&80
F7FF FF25     	bl	$00009700
4638           	mov	r0,r7
F7FF FF22     	bl	$00009700
2080           	mov	r0,#&80
F7FF FF1F     	bl	$00009700
4640           	mov	r0,r8
F7FF FF1C     	bl	$00009700

;; fn000098CC: 000098CC
fn000098CC proc
2040           	mov	r0,#&40
F7FF FF19     	bl	$00009700
4628           	mov	r0,r5
4651           	mov	r1,r10
444D           	adds	r5,r9
F7FF FEF4     	bl	$000096C0
3401           	adds	r4,#1
F815 0C01     	ldrb.w	r0,[r5,-#&1]
F7FF FF29     	bl	$00009734
42A6           	cmps	r6,r4
D1DB           	bne	$00009A9E

l000098EA:
E8BD 87F0     	pop.w	{r4-r10,pc}

;; fn000098EC: 000098EC
fn000098EC proc
87F0           	strh	r0,[r6,#&7C]
000098EE                                           00 BF               ..

;; OSRAMInit: 000098F0
OSRAMInit proc
E92D 41F0     	push.w	{r4-r8,lr}
4604           	mov	r4,r0
F04F 2010     	mov	r0,#&10001000
F000 F93F     	bl	$00009B78
4818           	ldr	r0,[pc,#&60]                            ; 00009966
F000 F93C     	bl	$00009B78

;; fn00009904: 00009904
fn00009904 proc
210C           	mov	r1,#&C
4817           	ldr	r0,[pc,#&5C]                            ; 0000996A
F7FF FDBA     	bl	$0000947C
4621           	mov	r1,r4
4816           	ldr	r0,[pc,#&58]                            ; 0000996E
F000 FBF0     	bl	$0000A0F0
2201           	mov	r2,#1
4B15           	ldr	r3,[pc,#&54]                            ; 00009972
4F15           	ldr	r7,[pc,#&54]                            ; 00009974
26E3           	mov	r6,#&E3
2404           	mov	r4,#4
2080           	mov	r0,#&80
2500           	mov	r5,#0
601A           	str	r2,[r3]
F507 78F6     	add	r8,r7,#&1EC
E006           	b	$00009934
0000992A                               93 F8 EC 41 93 F8           ...A..
00009930 ED 01 23 44                                     ..#D           

;; fn00009934: 00009934
fn00009934 proc
F893 61EC     	ldrb	r6,[r3,#&EC]
F7FF FEB0     	bl	$00009698

;; fn0000993A: 0000993A
fn0000993A proc
FEB0 1CA8     	Invalid
1EA1           	sub	r1,r4,#2
4440           	adds	r0,r8
3401           	adds	r4,#1
F7FF FEBE     	bl	$000096C0
4425           	adds	r5,r4
4630           	mov	r0,r6
F7FF FEF4     	bl	$00009734
2D70           	cmps	r5,#&70
EB07 0305     	add.w	r3,r7,r5
D9E8           	bls	$00009B26

l00009958:
E8BD 41F0     	pop.w	{r4-r8,lr}
F7FF BF10     	b	$0000977C

l00009960:
0002           	mov	r2,r0
2000           	mov	r0,#0
5000           	str	r0,[r0,r0]
4000           	ands	r0,r0
0000           	mov	r0,r0
4002           	ands	r2,r0
087C           	lsrs	r4,r7,#1
2000           	mov	r0,#0
A2F4           	adr	r2,$00009D40
0000           	mov	r0,r0

;; OSRAMDisplayOn: 00009974
OSRAMDisplayOn proc
E92D 41F0     	push.w	{r4-r8,lr}
4F10           	ldr	r7,[pc,#&40]                            ; 000099C0
26E3           	mov	r6,#&E3
2404           	mov	r4,#4
2080           	mov	r0,#&80
2500           	mov	r5,#0
F507 78F6     	add	r8,r7,#&1EC
E006           	b	$00009992
00009988                         93 F8 EC 41 93 F8               ...A.. 

l0000998E:
01ED           	lsls	r5,r5,#7
4423           	adds	r3,r4

;; fn00009992: 00009992
fn00009992 proc
F893 61EC     	ldrb	r6,[r3,#&EC]
F7FF FE81     	bl	$00009698
1CA8           	add	r0,r5,#2
1EA1           	sub	r1,r4,#2
4440           	adds	r0,r8
3401           	adds	r4,#1
F7FF FE8F     	bl	$000096C0
4425           	adds	r5,r4
4630           	mov	r0,r6
F7FF FEC5     	bl	$00009734

;; fn000099AE: 000099AE
fn000099AE proc
2D70           	cmps	r5,#&70
EB07 0305     	add.w	r3,r7,r5
D9E8           	bls	$00009B84

l000099B6:
E8BD 81F0     	pop.w	{r4-r8,pc}
000099BA                               00 BF F4 A2 00 00           ......

;; OSRAMDisplayOff: 000099C0
OSRAMDisplayOff proc
B508           	push	{r3,lr}
2080           	mov	r0,#&80
F7FF FE6A     	bl	$00009698
20AE           	mov	r0,#&AE
F7FF FE9B     	bl	$00009700
2080           	mov	r0,#&80
F7FF FE98     	bl	$00009700
20AD           	mov	r0,#&AD
F7FF FE95     	bl	$00009700
2080           	mov	r0,#&80
F7FF FE92     	bl	$00009700
E8BD 4008     	pop.w	{r3,lr}

;; fn000099E4: 000099E4
fn000099E4 proc
208A           	mov	r0,#&8A
E6A7           	b	$00009734

;; SSIConfig: 000099E8
SSIConfig proc
E92D 41F0     	push.w	{r4-r8,lr}
4617           	mov	r7,r2
4606           	mov	r6,r0
4688           	mov	r8,r1
461C           	mov	r4,r3
9D06           	ldr	r5,[sp,#&18]
F000 F9FB     	bl	$00009DEC
2F02           	cmps	r7,#2
D018           	beq	$00009A2C

l000099FE:
2F00           	cmps	r7,#0

;; fn00009A00: 00009A00
fn00009A00 proc
BF18           	it	ne
2704           	movne	r7,#4

;; fn00009A04: 00009A04
fn00009A04 proc
FBB0 F3F4     	udiv	r3,r0,r4
2400           	mov	r4,#0
6077           	str	r7,[r6,#&4]
3402           	adds	r4,#2
FBB3 F2F4     	udiv	r2,r3,r4
3A01           	subs	r2,#1
2AFF           	cmps	r2,#&FF
D8F9           	bhi	$00009C08

;; fn00009A18: 00009A18
fn00009A18 proc
F008 0330     	and	r3,r8,#&30
3D01           	subs	r5,#1
EA43 1188     	orr	r1,r3,r8,lsl #6
430D           	orrs	r5,r1
EA45 2202     	orr	r2,r5,r2,lsl #8
6134           	str	r4,[r6,#&10]
6032           	str	r2,[r6]

l00009A2C:
E8BD 81F0     	pop.w	{r4-r8,pc}

;; fn00009A30: 00009A30
fn00009A30 proc
270C           	mov	r7,#&C
E7E7           	b	$00009A00

;; SSIEnable: 00009A34
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
F7FF FD58     	bl	$00009500
E8BD 4008     	pop.w	{r3,lr}
2017           	mov	r0,#&17
F7FF BDBF     	b	$000095D8
00009A5E                                           00 BF               ..

;; SSIIntUnregister: 00009A60
SSIIntUnregister proc
B508           	push	{r3,lr}
2017           	mov	r0,#&17
F7FF FDE8     	bl	$00009634
E8BD 4008     	pop.w	{r3,lr}
2017           	mov	r0,#&17
F7FF BD63     	b	$00009534
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
SSIDataPut proc
F100 020C     	add	r2,r0,#&C
6813           	ldr	r3,[r2]

;; fn00009A9E: 00009A9E
fn00009A9E proc
079B           	lsls	r3,r3,#&1E
D5FC           	bpl	$00009C98

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

;; fn00009AB4: 00009AB4
fn00009AB4 proc
4618           	mov	r0,r3
4770           	bx	lr

;; SSIDataGet: 00009AB8
SSIDataGet proc
F100 020C     	add	r2,r0,#&C
6813           	ldr	r3,[r2]
075B           	lsls	r3,r3,#&1D
D5FC           	bpl	$00009CB8

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
4B03           	ldr	r3,[pc,#&C]                             ; 00009AF0
4804           	ldr	r0,[pc,#&10]                            ; 00009AF6
681B           	ldr	r3,[r3]
EA00 2013     	and.w	r0,r0,r3,lsr #8
F500 7080     	add	r0,r0,#&100
4770           	bx	lr
00009AEC                                     08 E0 0F 40             ...@
00009AF0 00 FF FF 00                                     ....           

;; SysCtlFlashSizeGet: 00009AF4
SysCtlFlashSizeGet proc
4B03           	ldr	r3,[pc,#&C]                             ; 00009B08
4804           	ldr	r0,[pc,#&10]                            ; 00009B0E
681B           	ldr	r3,[r3]
EA00 20C3     	and.w	r0,r0,r3,lsl #&B
F500 6000     	add	r0,r0,#&800
4770           	bx	lr
00009B04             08 E0 0F 40 00 F8 FF 07                 ...@....   

;; SysCtlPinPresent: 00009B0C
SysCtlPinPresent proc
4B03           	ldr	r3,[pc,#&C]                             ; 00009B20
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
4B05           	ldr	r3,[pc,#&14]                            ; 00009B3C
0F02           	lsrs	r2,r0,#&1C
F853 3022     	ldr.w	r3,[r3,r2,lsl #2]

;; fn00009B26: 00009B26
fn00009B26 proc
3022           	adds	r0,#&22
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
4B0E           	ldr	r3,[pc,#&38]                            ; 00009B7E
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
D805           	bhi	$00009B66

l00009B5E:
9B01           	ldr	r3,[sp,#&4]
3301           	adds	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]

l00009B66:
2B0F           	cmps	r3,#&F
D9F9           	bls	$00009D5A

l00009B6A:
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
B003           	add	sp,#&C
BC10           	pop	{r4}
4770           	bx	lr

;; fn00009B78: 00009B78
fn00009B78 proc
A554           	adr	r5,$00009CC8
0000           	mov	r0,r0

;; SysCtlPeripheralEnable: 00009B7C
SysCtlPeripheralEnable proc
4B05           	ldr	r3,[pc,#&14]                            ; 00009B98
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2

l00009B84:
69DB           	ldr	r3,[r3,#&1C]
F020 4070     	bic	r0,r0,#&F0000000
681A           	ldr	r2,[r3]
4310           	orrs	r0,r2
6018           	str	r0,[r3]
4770           	bx	lr
00009B92       00 BF 54 A5 00 00                           ..T...       

;; SysCtlPeripheralDisable: 00009B98
SysCtlPeripheralDisable proc
4B05           	ldr	r3,[pc,#&14]                            ; 00009BB4
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
4B05           	ldr	r3,[pc,#&14]                            ; 00009BD0
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
4B05           	ldr	r3,[pc,#&14]                            ; 00009BEC
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
4B05           	ldr	r3,[pc,#&14]                            ; 00009C08
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
4B05           	ldr	r3,[pc,#&14]                            ; 00009C24
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
4A05           	ldr	r2,[pc,#&14]                            ; 00009C40
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
F7FF FC5D     	bl	$00009500
E8BD 4008     	pop.w	{r3,lr}
202C           	mov	r0,#&2C
F7FF BCC4     	b	$000095D8

;; SysCtlIntUnregister: 00009C54
SysCtlIntUnregister proc
B508           	push	{r3,lr}
202C           	mov	r0,#&2C
F7FF FCEE     	bl	$00009634
E8BD 4008     	pop.w	{r3,lr}
202C           	mov	r0,#&2C
F7FF BC69     	b	$00009534
00009C66                   00 BF                               ..       

;; SysCtlIntEnable: 00009C68
SysCtlIntEnable proc
4A02           	ldr	r2,[pc,#&8]                             ; 00009C78
6813           	ldr	r3,[r2]
4318           	orrs	r0,r3
6010           	str	r0,[r2]
4770           	bx	lr
00009C72       00 BF 54 E0 0F 40                           ..T..@       

;; SysCtlIntDisable: 00009C78
SysCtlIntDisable proc
4A02           	ldr	r2,[pc,#&8]                             ; 00009C88
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009C84             54 E0 0F 40                             T..@       

;; SysCtlIntClear: 00009C88
SysCtlIntClear proc
4B01           	ldr	r3,[pc,#&4]                             ; 00009C94
6018           	str	r0,[r3]
4770           	bx	lr
00009C8E                                           00 BF               ..
00009C90 58 E0 0F 40                                     X..@           

;; SysCtlIntStatus: 00009C94
SysCtlIntStatus proc
B910           	cbnz	r0,$00009C9C

l00009C96:
4B03           	ldr	r3,[pc,#&C]                             ; 00009CAA

l00009C98:
6818           	ldr	r0,[r3]
4770           	bx	lr

l00009C9C:
4B02           	ldr	r3,[pc,#&8]                             ; 00009CAC
6818           	ldr	r0,[r3]
4770           	bx	lr
00009CA2       00 BF 50 E0 0F 40 58 E0 0F 40               ..P..@X..@   

;; SysCtlLDOSet: 00009CAC
SysCtlLDOSet proc
4B01           	ldr	r3,[pc,#&4]                             ; 00009CB8
6018           	str	r0,[r3]
4770           	bx	lr
00009CB2       00 BF 34 E0 0F 40                           ..4..@       

;; SysCtlLDOGet: 00009CB8
SysCtlLDOGet proc
4B01           	ldr	r3,[pc,#&4]                             ; 00009CC4
6818           	ldr	r0,[r3]
4770           	bx	lr
00009CBE                                           00 BF               ..
00009CC0 34 E0 0F 40                                     4..@           

;; SysCtlLDOConfigSet: 00009CC4
SysCtlLDOConfigSet proc
4B01           	ldr	r3,[pc,#&4]                             ; 00009CD0
6018           	str	r0,[r3]
4770           	bx	lr
00009CCA                               00 BF 60 E1 0F 40           ..`..@

;; SysCtlReset: 00009CD0
SysCtlReset proc
4B01           	ldr	r3,[pc,#&4]                             ; 00009CDC

l00009CD2:
4A02           	ldr	r2,[pc,#&8]                             ; 00009CE2
601A           	str	r2,[r3]
E7FE           	b	$00009CD2
00009CD8                         0C ED 00 E0 04 00 FA 05         ........

;; SysCtlSleep: 00009CE0
SysCtlSleep proc
F000 BA04     	b	$00C0A0E8

;; SysCtlDeepSleep: 00009CE4
SysCtlDeepSleep proc
B510           	push	{r4,lr}
4C06           	ldr	r4,[pc,#&18]                            ; 00009D06
6823           	ldr	r3,[r4]
F043 0304     	orr	r3,r3,#4
6023           	str	r3,[r4]
F000 F9FC     	bl	$0000A0E8
6823           	ldr	r3,[r4]
F023 0304     	bic	r3,r3,#4
6023           	str	r3,[r4]
BD10           	pop	{r4,pc}
00009CFE                                           00 BF               ..
00009D00 10 ED 00 E0                                     ....           

;; SysCtlResetCauseGet: 00009D04
SysCtlResetCauseGet proc
4B01           	ldr	r3,[pc,#&4]                             ; 00009D10
6818           	ldr	r0,[r3]
4770           	bx	lr
00009D0A                               00 BF 5C E0 0F 40           ..\..@

;; SysCtlResetCauseClear: 00009D10
SysCtlResetCauseClear proc
4A02           	ldr	r2,[pc,#&8]                             ; 00009D20
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009D1C                                     5C E0 0F 40             \..@

;; SysCtlBrownOutConfigSet: 00009D20
SysCtlBrownOutConfigSet proc
4B02           	ldr	r3,[pc,#&8]                             ; 00009D30
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
4C29           	ldr	r4,[pc,#&A4]                            ; 00009DE6
4929           	ldr	r1,[pc,#&A4]                            ; 00009DE8
6823           	ldr	r3,[r4]
F060 0503     	orn	r5,r0,#3
4019           	ands	r1,r3
F441 6100     	orr	r1,r1,#&800
4029           	ands	r1,r5
4002           	ands	r2,r0
F423 0380     	bic	r3,r3,#&400000
4D25           	ldr	r5,[pc,#&94]                            ; 00009DEE
B082           	sub	sp,#8
F443 6300     	orr	r3,r3,#&800

;; fn00009D5A: 00009D5A
fn00009D5A proc
430A           	orrs	r2,r1
6023           	str	r3,[r4]
602F           	str	r7,[r5]
6022           	str	r2,[r4]
9601           	str	r6,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D805           	bhi	$00009D72

;; fn00009D6A: 00009D6A
fn00009D6A proc
9B01           	ldr	r3,[sp,#&4]
3301           	adds	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]

;; fn00009D72: 00009D72
fn00009D72 proc
2B0F           	cmps	r3,#&F
D9F9           	bls	$00009F66

l00009D76:
F000 0303     	and	r3,r0,#3
4C19           	ldr	r4,[pc,#&64]                            ; 00009DE6
F022 0203     	bic	r2,r2,#3
431A           	orrs	r2,r3
F022 63F8     	bic	r3,r2,#&7C00000
F000 61F8     	and	r1,r0,#&7C00000
6022           	str	r2,[r4]
0504           	lsls	r4,r0,#&14
EA41 0103     	orr	r1,r1,r3
D414           	bmi	$00009DBA

l00009D94:
F44F 4300     	mov	r3,#&8000
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
B16B           	cbz	r3,$00009DBA

l00009D9E:
4A13           	ldr	r2,[pc,#&4C]                            ; 00009DF2
6813           	ldr	r3,[r2]
0658           	lsls	r0,r3,#&19
D503           	bpl	$00009DAA

l00009DA6:
E008           	b	$00009DB6
00009DA8                         13 68                           .h     

l00009DAA:
065B           	lsls	r3,r3,#&19
D405           	bmi	$00009DB6

l00009DAE:
9B01           	ldr	r3,[sp,#&4]
3B01           	subs	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]

l00009DB6:
2B00           	cmps	r3,#0
D1F6           	bne	$00009FA4

l00009DBA:
F421 6100     	bic	r1,r1,#&800
2300           	mov	r3,#0
4A07           	ldr	r2,[pc,#&1C]                            ; 00009DE4
6011           	str	r1,[r2]
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D805           	bhi	$00009DD4

l00009DCC:
9B01           	ldr	r3,[sp,#&4]
3301           	adds	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]

l00009DD4:
2B0F           	cmps	r3,#&F
D9F9           	bls	$00009FC8

l00009DD8:
B002           	add	sp,#8
BCF0           	pop	{r4-r7}
4770           	bx	lr
00009DDE                                           00 BF               ..
00009DE0 60 E0 0F 40 0F CC BF FF 58 E0 0F 40             `..@....X..@   

;; fn00009DEC: 00009DEC
fn00009DEC proc
E050           	b	$00009E8C
00009DEE                                           0F 40               .@

;; SysCtlClockGet: 00009DF0
SysCtlClockGet proc
4B18           	ldr	r3,[pc,#&60]                            ; 00009E58
681B           	ldr	r3,[r3]
F003 0230     	and	r2,r3,#&30
2A10           	cmps	r2,#&10
D028           	beq	$00009E4A

l00009DFC:
2A20           	cmps	r2,#&20
D024           	beq	$00009E46

l00009E00:
B10A           	cbz	r2,$00009E06

l00009E02:
2000           	mov	r0,#0
4770           	bx	lr

l00009E06:
4A14           	ldr	r2,[pc,#&50]                            ; 00009E5E
F3C3 1183     	ubfx	r1,r3,#6,#4
EB02 0281     	add.w	r2,r2,r1,lsl #2

l00009E0E:
0281           	lsls	r1,r0,#&A

l00009E10:
6B10           	ldr	r0,[r2,#&30]
051A           	lsls	r2,r3,#&14
D411           	bmi	$00009E36

l00009E16:
4A11           	ldr	r2,[pc,#&44]                            ; 00009E62
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

l00009E36:
BF48           	it	mi
0880           	lsrsmi	r0,r0,#2

l00009E3A:
025A           	lsls	r2,r3,#9
D5E2           	bpl	$0000A000

l00009E3E:
F3C3 53C3     	ubfx	r3,r3,#&17,#4
3301           	adds	r3,#1
FBB0 F0F3     	udiv	r0,r0,r3

l00009E46:
F0F3 4770     	Invalid

l00009E48:
4770           	bx	lr

l00009E4A:
4805           	ldr	r0,[pc,#&14]                            ; 00009E66
E7E1           	b	$00009E0E
00009E4E                                           05 48               .H
00009E50 DF E7 00 BF 60 E0 0F 40 54 A5 00 00 64 E0 0F 40 ....`..@T...d..@
00009E60 70 38 39 00 C0 E1 E4 00                         p89.....       

;; SysCtlPWMClockSet: 00009E68
SysCtlPWMClockSet proc
4A03           	ldr	r2,[pc,#&C]                             ; 00009E7C
6813           	ldr	r3,[r2]
F423 13F0     	bic	r3,r3,#&1E0000
4318           	orrs	r0,r3
6010           	str	r0,[r2]
4770           	bx	lr
00009E76                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlPWMClockGet: 00009E7C
SysCtlPWMClockGet proc
4B02           	ldr	r3,[pc,#&8]                             ; 00009E8C
6818           	ldr	r0,[r3]
F400 10F0     	and	r0,r0,#&1E0000
4770           	bx	lr
00009E86                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlADCSpeedSet: 00009E8C
SysCtlADCSpeedSet proc
B410           	push	{r4}
4C0A           	ldr	r4,[pc,#&28]                            ; 00009EBE
490A           	ldr	r1,[pc,#&28]                            ; 00009EC0
6823           	ldr	r3,[r4]
4A0A           	ldr	r2,[pc,#&28]                            ; 00009EC4
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
4B02           	ldr	r3,[pc,#&8]                             ; 00009ED4
6818           	ldr	r0,[r3]
F400 6070     	and	r0,r0,#&F00
4770           	bx	lr
00009ECE                                           00 BF               ..
00009ED0 00 E1 0F 40                                     ...@           

;; SysCtlIOSCVerificationSet: 00009ED4
SysCtlIOSCVerificationSet proc
4A05           	ldr	r2,[pc,#&14]                            ; 00009EF0
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
4A05           	ldr	r2,[pc,#&14]                            ; 00009F0C
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
4A05           	ldr	r2,[pc,#&14]                            ; 00009F28
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
4B01           	ldr	r3,[pc,#&4]                             ; 00009F38
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
6804           	ldr	r4,[r0]
F014 0408     	ands	r4,r4,#8
D1FB           	bne	$0000A152

l00009F5E:
6AEB           	ldr	r3,[r5,#&2C]
F023 0310     	bic	r3,r3,#&10
62EB           	str	r3,[r5,#&2C]

l00009F66:
6B2A           	ldr	r2,[r5,#&30]
F422 7240     	bic	r2,r2,#&300
F022 0201     	bic	r2,r2,#1
632A           	str	r2,[r5,#&30]
F7FF FF3D     	bl	$00009DEC
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

l00009FA4:
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
F7FF FF1A     	bl	$00009DEC
EB05 1588     	add.w	r5,r5,r8,lsl #6
0080           	lsls	r0,r0,#2
FBB0 F0F5     	udiv	r0,r0,r5
6038           	str	r0,[r7]

l00009FC8:
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
6813           	ldr	r3,[r2]
071B           	lsls	r3,r3,#&1C
D4FC           	bmi	$0000A1EC

l00009FF6:
6AC3           	ldr	r3,[r0,#&2C]
F023 0310     	bic	r3,r3,#&10
62C3           	str	r3,[r0,#&2C]
6B03           	ldr	r3,[r0,#&30]

l0000A000:
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
6813           	ldr	r3,[r2]
06DB           	lsls	r3,r3,#&1B
D4FC           	bmi	$0000A234

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
6813           	ldr	r3,[r2]
069B           	lsls	r3,r3,#&1A
D4FC           	bmi	$0000A254

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
4C06           	ldr	r4,[pc,#&18]                            ; 0000A09A
42A0           	cmps	r0,r4
BF0C           	ite	eq
2415           	moveq	r4,#&15

l0000A082:
2416           	mov	r4,#&16
4620           	mov	r0,r4
F7FF FA3D     	bl	$00009500
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F7FF BAA4     	b	$000095D8
0000A094             00 C0 00 40                             ...@       

;; UARTIntUnregister: 0000A098
UARTIntUnregister proc
B510           	push	{r4,lr}
4C06           	ldr	r4,[pc,#&18]                            ; 0000A0BA
42A0           	cmps	r0,r4
BF0C           	ite	eq
2415           	moveq	r4,#&15

l0000A0A2:
2416           	mov	r4,#&16
4620           	mov	r0,r4
F7FF FAC7     	bl	$00009634
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F7FF BA42     	b	$00009534
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

;; fn0000A0C8: 0000A0C8
fn0000A0C8 proc
4770           	bx	lr
0000A0CA                               00 BF                       ..   

;; UARTIntStatus: 0000A0CC
UARTIntStatus proc
B909           	cbnz	r1,$0000A0D2

l0000A0CE:
6BC0           	ldr	r0,[r0,#&3C]
4770           	bx	lr

l0000A0D2:
6C00           	ldr	r0,[r0,#&40]

;; fn0000A0D4: 0000A0D4
fn0000A0D4 proc
4770           	bx	lr
0000A0D6                   00 BF                               ..       

;; UARTIntClear: 0000A0D8
UARTIntClear proc
6441           	str	r1,[r0,#&44]
4770           	bx	lr

;; CPUcpsie: 0000A0DC
CPUcpsie proc
B662           	cps	#0
4770           	bx	lr
0000A0E0 70 47 00 BF                                     pG..           

;; CPUcpsid: 0000A0E4
CPUcpsid proc
B672           	cps	#0
4770           	bx	lr

;; fn0000A0E8: 0000A0E8
fn0000A0E8 proc
4770           	bx	lr
0000A0EA                               00 BF                       ..   

;; CPUwfi: 0000A0EC
CPUwfi proc
BF30           	wfi
4770           	bx	lr

;; fn0000A0F0: 0000A0F0
fn0000A0F0 proc
4770           	bx	lr
0000A0F2       00 BF                                       ..           

;; I2CMasterInit: 0000A0F4
I2CMasterInit proc
B538           	push	{r3-r5,lr}
460D           	mov	r5,r1
6A02           	ldr	r2,[r0,#&20]
4604           	mov	r4,r0
F042 0210     	orr	r2,r2,#&10
6202           	str	r2,[r0,#&20]
F7FF FE75     	bl	$00009DEC
4B06           	ldr	r3,[pc,#&18]                            ; 0000A126
4A06           	ldr	r2,[pc,#&18]                            ; 0000A128
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

l0000A152:
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
F7FF F9BE     	bl	$00009500
E8BD 4008     	pop.w	{r3,lr}
2018           	mov	r0,#&18
F7FF BA25     	b	$000095D8
0000A192       00 BF                                       ..           

;; I2CIntUnregister: 0000A194
I2CIntUnregister proc
B508           	push	{r3,lr}
2018           	mov	r0,#&18
F7FF FA4E     	bl	$00009634
E8BD 4008     	pop.w	{r3,lr}
2018           	mov	r0,#&18
F7FF B9C9     	b	$00009534
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

;; fn0000A1C4: 0000A1C4
fn0000A1C4 proc
4770           	bx	lr
0000A1C6                   00 BF                               ..       

;; I2CMasterIntStatus: 0000A1C8
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

;; fn0000A1EC: 0000A1EC
fn0000A1EC proc
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

;; fn0000A204: 0000A204
fn0000A204 proc
4770           	bx	lr
0000A206                   00 BF                               ..       

;; I2CMasterSlaveAddrSet: 0000A208
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

;; fn0000A21C: 0000A21C
fn0000A21C proc
1080           	asrss	r0,r0,#2
4770           	bx	lr

;; I2CMasterControl: 0000A220
I2CMasterControl proc
6041           	str	r1,[r0,#&4]
4770           	bx	lr

;; I2CMasterErr: 0000A224
I2CMasterErr proc
6843           	ldr	r3,[r0,#&4]
07DA           	lsls	r2,r3,#&1F
D405           	bmi	$0000A232

l0000A22A:
F013 0002     	ands	r0,r3,#2
D003           	beq	$0000A234

l0000A230:
F003 001C     	and	r0,r3,#&1C

l0000A232:
001C           	mov	r4,r3

l0000A234:
4770           	bx	lr
0000A236                   00 20                               .        

;; fn0000A238: 0000A238
fn0000A238 proc
4770           	bx	lr
0000A23A                               00 BF                       ..   

;; I2CMasterDataPut: 0000A23C
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
0000A250 48 65 6C 6C                                     Hell           

l0000A254:
006F           	lsls	r7,r5,#1
0000           	mov	r0,r0
6843           	ldr	r3,[r0,#&4]
6365           	str	r5,[r4,#&34]
006B           	lsls	r3,r5,#1
0000           	mov	r0,r0
7250           	strb	r0,[r2,#&9]
6E69           	ldr	r1,[r5,#&64]
0074           	lsls	r4,r6,#1
0000           	mov	r0,r0
6853           	ldr	r3,[r2,#&4]
756F           	strb	r7,[r5,#&15]
646C           	str	r4,[r5,#&44]
6E20           	ldr	r0,[r4,#&60]
746F           	strb	r7,[r5,#&11]
6220           	str	r0,[r4,#&20]
2065           	mov	r0,#&65
6874           	ldr	r4,[r6,#&4]
7265           	strb	r5,[r4,#&9]
0065           	lsls	r5,r4,#1
4449           	adds	r1,r9
454C           	cmps	r4,r9
0000           	mov	r0,r0
0000           	mov	r0,r0
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
0000           	mov	r0,r0
007D           	lsls	r5,r7,#1
;;; Segment .text.memcpy (0000A5C4)

;; memcpy: 0000A5C4
memcpy proc
B5F0           	push	{r4-r7,lr}
0005           	mov	r5,r0
2A0F           	cmps	r2,#&F
D92F           	bls	$0000A628

l0000A5CC:
000B           	mov	r3,r1
4303           	orrs	r3,r0
079B           	lsls	r3,r3,#&1E
D136           	bne	$0000A63E

l0000A5D4:
0016           	mov	r6,r2
000C           	mov	r4,r1
0003           	mov	r3,r0
3E10           	subs	r6,#&10
0935           	lsrs	r5,r6,#4
3501           	adds	r5,#1
012D           	lsls	r5,r5,#4
1945           	add	r5,r0,r5
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
D1F3           	bne	$0000A7E0

l0000A5FC:
230F           	mov	r3,#&F
439E           	bics	r6,r3
3610           	adds	r6,#&10
1985           	add	r5,r0,r6
1989           	add	r1,r1,r6
4013           	ands	r3,r2
2B03           	cmps	r3,#3
D91C           	bls	$0000A642

l0000A60C:
1F1E           	sub	r6,r3,#4
2300           	mov	r3,#0
08B4           	lsrs	r4,r6,#2
3401           	adds	r4,#1
00A4           	lsls	r4,r4,#2
58CF           	ldr	r7,[r1,r3]
50EF           	str	r7,[r5,r3]
3304           	adds	r3,#4
42A3           	cmps	r3,r4
D1FA           	bne	$0000A812

l0000A620:
2403           	mov	r4,#3
43A6           	bics	r6,r4
1D33           	add	r3,r6,#4
4022           	ands	r2,r4

l0000A628:
18C9           	add	r1,r1,r3
18ED           	add	r5,r5,r3

l0000A62C:
2A00           	cmps	r2,#0
D005           	beq	$0000A638

l0000A630:
2300           	mov	r3,#0
5CCC           	ldrb	r4,[r1,r3]
54EC           	strb	r4,[r5,r3]
3301           	adds	r3,#1

l0000A638:
4293           	cmps	r3,r2
D1FA           	bne	$0000A82E

l0000A63C:
BCF0           	pop	{r4-r7}

l0000A63E:
BC02           	pop	{r1}
4708           	bx	r1

l0000A642:
0005           	mov	r5,r0
E7F4           	b	$0000A62C
0000A646                   1A 00 F0 E7 C0 46                   .....F   
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
