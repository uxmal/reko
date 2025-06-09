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
