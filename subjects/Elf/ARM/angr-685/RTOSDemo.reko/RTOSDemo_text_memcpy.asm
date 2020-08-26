;;; Segment .text.memcpy (0000A5C4)

;; memcpy: 0000A5C4
;;   Called from:
;;     0000010A (in prvCopyDataToQueue)
;;     0000012A (in prvCopyDataToQueue)
;;     00000188 (in prvCopyDataFromQueue)
;;     00008466 (in xQueueCRReceive)
;;     00008504 (in xQueueCRReceiveFromISR)
memcpy proc
	push	{r4-r7,lr}
	mov	r5,r0
	cmps	r2,#&F
	bls	$0000A62C

l0000A5CC:
	mov	r3,r1
	orrs	r3,r0
	lsls	r3,r3,#&1E
	bne	$0000A642

l0000A5D4:
	mov	r6,r2
	mov	r4,r1
	mov	r3,r0
	subs	r6,#&10
	lsrs	r5,r6,#4
	adds	r5,#1
	lsls	r5,r5,#4
	add	r5,r0,r5

l0000A5E4:
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
	bne	$0000A5E4

l0000A5FC:
	mov	r3,#&F
	bics	r6,r3
	adds	r6,#&10
	add	r5,r0,r6
	add	r1,r1,r6
	ands	r3,r2
	cmps	r3,#3
	bls	$0000A646

l0000A60C:
	sub	r6,r3,#4
	mov	r3,#0
	lsrs	r4,r6,#2
	adds	r4,#1
	lsls	r4,r4,#2

l0000A616:
	ldr	r7,[r1,r3]
	str	r7,[r5,r3]
	adds	r3,#4
	cmps	r3,r4
	bne	$0000A616

l0000A620:
	mov	r4,#3
	bics	r6,r4
	add	r3,r6,#4
	ands	r2,r4
	add	r1,r1,r3
	add	r5,r5,r3

l0000A62C:
	cmps	r2,#0
	beq	$0000A63C

l0000A630:
	mov	r3,#0

l0000A632:
	ldrb	r4,[r1,r3]
	strb	r4,[r5,r3]
	adds	r3,#1
	cmps	r3,r2
	bne	$0000A632

l0000A63C:
	pop	{r4-r7}
	pop	{r1}
	bx	r1

l0000A642:
	mov	r5,r0
	b	$0000A630

l0000A646:
	mov	r2,r3
	b	$0000A62C
0000A64A                               C0 46                       .F   
