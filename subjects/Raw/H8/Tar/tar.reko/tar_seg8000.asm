;;; Segment seg8000 (8000)

;; fn8000: 8000
fn8000 proc
	mov.w	r0,@-sp
	mov.w	r1,@-sp
	mov.w	r2,@-sp
	mov.w	r3,@-sp
	jsr	@fn00009AF8
	mov.w	@sp+,r3
	mov.w	@sp+,r2
	mov.w	@sp+,r1
	mov.w	@sp+,r0
	rts
8016                   44 6F 20 79 6F 75 20 62 79 74       Do you byt
8020 65 2C 20 77 68 65 6E 20 49 20 6B 6E 6F 63 6B 3F e, when I knock?
8030 00 00                                           ..              

;; fn00008032: 00008032
;;   Called from:
;;     000088FE (in fn00008866)
fn00008032 proc
	mov.w	#0x002E,r3
	sub.w	r3,r7
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	sub.w	r5,r5
	mov.w	#0x0004,r6
	sub.w	r0,r0

l00008046:
	sub.w	r1,r1
	mov.w	r1,@(6:16,sp)
	mov.w	r5,r3
	add.w	r3,r3

l00008050:
	mov.w	r3,r2
	mov.w	#0x9E42,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	r0,@er2
	add.b	#0x0C,r3l
	addx.b	#0x00,r3h
	mov.w	@(6:16,sp),r1
	adds	#0x00000001,er1
	mov.w	r1,@(6:16,sp)
	cmp.w	r6,r1
	ble	00008050

l0000806E:
	mov.w	#0x0005,r2
	adds	#0x00000001,er5
	cmp.w	r2,r5
	ble	00008046

l00008078:
	sub.w	r5,r5

l0000807A:
	sub.w	r4,r4
	mov.w	r4,@(6:16,sp)
	mov.w	r5,@(48:16,sp)
	mov.w	r5,r1
	add.w	r1,r1
	mov.w	r1,@(48:16,sp)

l0000808C:
	mov.w	@(6:16,sp),r2
	add.w	r2,r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(48:16,sp),r1
	mov.w	r1,@(46:16,sp)
	mov.w	r1,r4
	add.w	r2,r4
	mov.w	r4,@(46:16,sp)
	mov.w	@(46:16,sp),r2
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,r2
	mov.w	@er2,r2
	mov.w	r2,@(50:16,sp)
	mov.w	@(6:16,sp),r4
	mov.w	r4,@(44:16,sp)
	mov.w	r4,r1
	adds	#0x00000001,er1
	mov.w	r1,@(44:16,sp)
	mov.w	@(44:16,sp),r2
	add.w	r2,r2
	add.w	r1,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(48:16,sp),r4
	mov.w	r4,@(42:16,sp)
	mov.w	r4,r1
	add.w	r2,r1
	mov.w	r1,@(42:16,sp)
	mov.w	@(42:16,sp),r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r6
	mov.w	@(6:16,sp),r3
	adds	#0x00000002,er3
	mov.w	r3,r2
	add.w	r2,r2
	add.w	r3,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(48:16,sp),r1
	mov.w	r1,@(6:16,sp)
	mov.w	r1,r4
	add.w	r2,r4
	mov.w	r4,@(6:16,sp)
	mov.w	@(6:16,sp),r2
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,r2
	mov.w	@er2,r2
	mov.w	r2,@(8:16,sp)
	mov.w	@(50:16,sp),r0
	bge	00008136

l00008130:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l00008136:
	mov.w	r6,r3
	mov.w	r6,r6
	bge	00008142

l0000813C:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l00008142:
	mov.w	r0,r2
	add.w	r3,r2
	mov.w	@(8:16,sp),r3
	bge	00008152

l0000814C:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l00008152:
	add.w	r2,r3
	mov.w	@(50:16,sp),r2
	add.w	r6,r2
	mov.w	@(8:16,sp),r4
	add.w	r4,r2
	mov.w	r2,r2
	bge	0000816A

l00008164:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l0000816A:
	cmp.w	r2,r3
	bne	0000819E

l0000816E:
	mov.w	@(46:16,sp),r1
	mov.w	#0x9E42,r3
	add.w	r1,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	@(42:16,sp),r4
	mov.w	#0x9E42,r3
	add.w	r4,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	@(6:16,sp),r1
	mov.w	#0x9E42,r3
	add.w	r1,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3

l0000819E:
	mov.w	@(44:16,sp),r4
	mov.w	r4,@(6:16,sp)
	mov.w	#0x0002,r2
	cmp.w	r2,r4
	bgt	000081B2

l000081AE:
	jmp	@0x808C:24

l000081B2:
	adds	#0x00000001,er5
	mov.w	#0x0005,r2
	cmp.w	r2,r5
	bgt	000081C0

l000081BC:
	jmp	@0x807A:24

l000081C0:
	sub.w	r5,r5

l000081C2:
	sub.w	r1,r1
	mov.w	r1,@(6:16,sp)
	mov.w	r5,@(18:16,sp)
	mov.w	r5,r4
	adds	#0x00000001,er4
	mov.w	r4,@(18:16,sp)
	mov.w	r5,@(16:16,sp)
	mov.w	r5,r1
	adds	#0x00000002,er1
	mov.w	r1,@(16:16,sp)
	mov.w	r5,r2
	add.w	r2,r2
	mov.w	r2,@(14:16,sp)
	mov.w	#0x9E42,r4
	add.w	r2,r4
	mov.w	r4,@(14:16,sp)
	mov.w	r2,@(12:16,sp)
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,@(12:16,sp)
	sub.w	r4,r4
	mov.w	r4,@(10:16,sp)

l00008206:
	mov.w	@(12:16,sp),r1
	mov.w	@er1,r1
	mov.w	r1,@(40:16,sp)
	mov.w	@(12:16,sp),r4
	add.b	#0x0C,r4l
	addx.b	#0x00,r4h
	mov.w	r4,@(12:16,sp)
	mov.w	@(18:16,sp),r3
	add.w	r3,r3
	mov.w	@(10:16,sp),r1
	add.w	r1,r3
	mov.w	r3,r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	mov.w	r2,@(38:16,sp)
	mov.w	@(16:16,sp),r5
	add.w	r5,r5
	add.w	r1,r5
	mov.w	r5,r2
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,r2
	mov.w	@er2,r6
	mov.w	@(40:16,sp),r2
	bge	00008258

l00008252:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l00008258:
	mov.w	@(38:16,sp),r0
	bge	00008264

l0000825E:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l00008264:
	add.w	r0,r2
	mov.w	r6,r0
	mov.w	r6,r6
	bge	00008272

l0000826C:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l00008272:
	add.w	r2,r0
	mov.w	@(38:16,sp),r4
	mov.w	@(40:16,sp),r2
	add.w	r4,r2
	add.w	r6,r2
	mov.w	r2,r2
	bge	0000828A

l00008284:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l0000828A:
	cmp.w	r2,r0
	bne	000082B2

l0000828E:
	mov.w	@(14:16,sp),r1
	mov.w	@er1,r2
	adds	#0x00000001,er2
	mov.w	r2,@er1
	mov.w	#0x9E42,r4
	add.w	r3,r4
	mov.w	r4,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	#0x9E42,r3
	add.w	r5,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3

l000082B2:
	mov.w	@(14:16,sp),r1
	add.b	#0x0C,r1l
	addx.b	#0x00,r1h
	mov.w	r1,@(14:16,sp)
	mov.w	@(10:16,sp),r4
	add.b	#0x0C,r4l
	addx.b	#0x00,r4h
	mov.w	r4,@(10:16,sp)
	mov.w	@(6:16,sp),r1
	adds	#0x00000001,er1
	mov.w	r1,@(6:16,sp)
	mov.w	#0x0004,r2
	cmp.w	r2,r1
	bgt	000082E0

l000082DC:
	jmp	@0x8206:24

l000082E0:
	mov.w	@(18:16,sp),r5
	mov.w	#0x0003,r2
	cmp.w	r2,r5
	bgt	000082F0

l000082EC:
	jmp	@0x81C2:24

l000082F0:
	sub.w	r5,r5

l000082F2:
	sub.w	r4,r4
	mov.w	r4,@(6:16,sp)
	mov.w	r5,@(18:16,sp)
	mov.w	r5,r1
	adds	#0x00000001,er1
	mov.w	r1,@(18:16,sp)
	mov.w	r5,@(16:16,sp)
	mov.w	r5,r4
	adds	#0x00000002,er4
	mov.w	r4,@(16:16,sp)
	add.w	r5,r5
	mov.w	r5,@(34:16,sp)

l00008316:
	mov.w	@(6:16,sp),r2
	add.w	r2,r2
	mov.w	@(6:16,sp),r1
	add.w	r1,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(34:16,sp),r4
	mov.w	r4,@(32:16,sp)
	mov.w	r4,r1
	add.w	r2,r1
	mov.w	r1,@(32:16,sp)
	mov.w	@(32:16,sp),r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	mov.w	r2,@(36:16,sp)
	mov.w	@(18:16,sp),r6
	add.w	r6,r6
	mov.w	@(6:16,sp),r1
	mov.w	r1,@(28:16,sp)
	mov.w	r1,r4
	adds	#0x00000001,er4
	mov.w	r4,@(28:16,sp)
	mov.w	@(28:16,sp),r2
	add.w	r2,r2
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r6
	mov.w	r6,r2
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,r2
	mov.w	@er2,r2
	mov.w	r2,@(30:16,sp)
	mov.w	@(16:16,sp),r5
	add.w	r5,r5
	mov.w	@(6:16,sp),r3
	adds	#0x00000002,er3
	mov.w	r3,r2
	add.w	r2,r2
	add.w	r3,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r5
	mov.w	r5,r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	mov.w	r2,@(6:16,sp)
	mov.w	@(36:16,sp),r0
	bge	000083B0

l000083AA:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l000083B0:
	mov.w	@(30:16,sp),r3
	bge	000083BC

l000083B6:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l000083BC:
	mov.w	r0,r2
	add.w	r3,r2
	mov.w	@(6:16,sp),r3
	bge	000083CC

l000083C6:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l000083CC:
	add.w	r2,r3
	mov.w	@(30:16,sp),r1
	mov.w	@(36:16,sp),r2
	add.w	r1,r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	mov.w	r2,r2
	bge	000083E8

l000083E2:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l000083E8:
	cmp.w	r2,r3
	bne	00008414

l000083EC:
	mov.w	@(32:16,sp),r1
	mov.w	#0x9E42,r3
	add.w	r1,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	#0x9E42,r3
	add.w	r6,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	#0x9E42,r3
	add.w	r5,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3

l00008414:
	mov.w	@(28:16,sp),r4
	mov.w	r4,@(6:16,sp)
	mov.w	#0x0002,r2
	cmp.w	r2,r4
	bgt	00008428

l00008424:
	jmp	@0x8316:24

l00008428:
	mov.w	@(18:16,sp),r5
	mov.w	#0x0003,r2
	cmp.w	r2,r5
	bgt	00008438

l00008434:
	jmp	@0x82F2:24

l00008438:
	sub.w	r5,r5

l0000843A:
	sub.w	r1,r1
	mov.w	r1,@(6:16,sp)
	mov.w	r5,@(18:16,sp)
	mov.w	r5,r4
	adds	#0x00000001,er4
	mov.w	r4,@(18:16,sp)
	mov.w	r5,@(16:16,sp)
	mov.w	r5,r1
	adds	#0x00000002,er1
	mov.w	r1,@(16:16,sp)
	add.w	r5,r5
	mov.w	r5,@(24:16,sp)

l0000845E:
	mov.w	@(6:16,sp),r3
	adds	#0x00000002,er3
	mov.w	r3,r2
	add.w	r2,r2
	add.w	r3,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(24:16,sp),r4
	mov.w	r4,@(22:16,sp)
	mov.w	r4,r1
	add.w	r2,r1
	mov.w	r1,@(22:16,sp)
	mov.w	@(22:16,sp),r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	mov.w	r2,@(26:16,sp)
	mov.w	@(18:16,sp),r6
	add.w	r6,r6
	mov.w	@(6:16,sp),r1
	mov.w	r1,@(20:16,sp)
	mov.w	r1,r4
	adds	#0x00000001,er4
	mov.w	r4,@(20:16,sp)
	mov.w	@(20:16,sp),r2
	add.w	r2,r2
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r6
	mov.w	r6,r2
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,r2
	mov.w	@er2,r3
	mov.w	@(16:16,sp),r5
	add.w	r5,r5
	mov.w	@(6:16,sp),r2
	add.w	r2,r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r5
	mov.w	r5,r2
	mov.w	#0x9EB0,r1
	add.w	r2,r1
	mov.w	r1,r2
	mov.w	@er2,r2
	mov.w	r2,@(6:16,sp)
	mov.w	@(26:16,sp),r2
	bge	000084F4

l000084EE:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l000084F4:
	mov.w	r3,r0
	mov.w	r3,r3
	bge	00008500

l000084FA:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l00008500:
	add.w	r0,r2
	mov.w	@(6:16,sp),r0
	bge	0000850E

l00008508:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l0000850E:
	add.w	r2,r0
	mov.w	@(26:16,sp),r2
	add.w	r3,r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	mov.w	r2,r2
	bge	00008526

l00008520:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l00008526:
	cmp.w	r2,r0
	bne	00008552

l0000852A:
	mov.w	@(22:16,sp),r1
	mov.w	#0x9E42,r3
	add.w	r1,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	#0x9E42,r3
	add.w	r6,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3
	mov.w	#0x9E42,r3
	add.w	r5,r3
	mov.w	@er3,r2
	adds	#0x00000001,er2
	mov.w	r2,@er3

l00008552:
	mov.w	@(20:16,sp),r4
	mov.w	r4,@(6:16,sp)
	mov.w	#0x0002,r2
	cmp.w	r2,r4
	bgt	00008566

l00008562:
	jmp	@0x845E:24

l00008566:
	mov.w	@(18:16,sp),r5
	mov.w	#0x0003,r2
	cmp.w	r2,r5
	bgt	00008576

l00008572:
	jmp	@0x843A:24

l00008576:
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x002E,r3
	add.w	r3,r7
	rts

;; fn00008584: 00008584
;;   Called from:
;;     000088DC (in fn00008866)
;;     00009562 (in fn00009478)
;;     00009574 (in fn00009478)
;;     00009590 (in fn00009478)
;;     00009598 (in fn00009478)
;;     00009648 (in fn00009478)
;;     0000965A (in fn00009478)
;;     00009676 (in fn00009478)
;;     0000967E (in fn00009478)
;;     0000971E (in fn00009478)
;;     00009730 (in fn00009478)
;;     0000974C (in fn00009478)
;;     00009754 (in fn00009478)
fn00008584 proc
	mov.w	#0x000A,r3
	sub.w	r3,r7
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	sub.w	r4,r4
	mov.w	r4,@(12:16,sp)
	sub.w	r5,r5
	mov.w	r5,@(14:16,sp)
	mov.w	#0x9EB0,r6

l000085A0:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r4
	mov.w	r4,@(8:16,sp)
	mov.w	r4,r5
	add.w	r5,r5
	mov.w	r5,@(8:16,sp)

l000085B2:
	mov.w	r1,r2
	add.w	r2,r2
	add.w	r1,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(8:16,sp),r0
	add.w	r2,r0
	add.w	r6,r0
	mov.w	r1,@(6:16,sp)
	mov.w	r1,r4
	adds	#0x00000001,er4
	mov.w	r4,@(6:16,sp)
	mov.w	@(6:16,sp),r2
	add.w	r2,r2
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(8:16,sp),r3
	add.w	r2,r3
	add.w	r6,r3
	mov.w	@er0,r0
	mov.w	@er3,r2
	cmp.w	r2,r0
	bne	0000861C

l000085EC:
	mov.w	r1,r2
	adds	#0x00000002,er2
	mov.w	r2,r3
	add.w	r3,r3
	add.w	r2,r3
	add.w	r3,r3
	add.w	r3,r3
	mov.w	@(8:16,sp),r2
	add.w	r3,r2
	mov.w	#0x9EB0,r5
	add.w	r2,r5
	mov.w	r5,r2
	mov.w	@er2,r2
	cmp.w	r2,r0
	bne	0000861C

l0000860E:
	mov.w	r0,r0
	beq	0000861C

l00008612:
	mov.w	@(12:16,sp),r4
	add.w	r0,r4
	mov.w	r4,@(12:16,sp)

l0000861C:
	mov.w	@(6:16,sp),r1
	mov.w	#0x0002,r2
	cmp.w	r2,r1
	ble	000085B2

l00008628:
	mov.w	@(14:16,sp),r5
	adds	#0x00000001,er5
	mov.w	r5,@(14:16,sp)
	mov.w	#0x0005,r2
	cmp.w	r2,r5
	bgt	0000863E

l0000863A:
	jmp	@0x85A0:24

l0000863E:
	sub.w	r4,r4
	mov.w	r4,@(14:16,sp)

l00008644:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r6
	adds	#0x00000001,er6
	mov.w	@(14:16,sp),r0
	add.w	r0,r0
	mov.w	#0x9EB0,r5
	add.w	r0,r5
	mov.w	r5,r0
	sub.w	r4,r4
	mov.w	r4,@(6:16,sp)
	mov.w	r6,@(8:16,sp)
	mov.w	r6,r5
	add.w	r5,r5
	mov.w	r5,@(8:16,sp)

l0000866C:
	mov.w	@(8:16,sp),r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	mov.w	#0x9EB0,r5
	add.w	r2,r5
	mov.w	r5,r2
	mov.w	@er0,r3
	add.b	#0x0C,r0l
	addx.b	#0x00,r0h
	mov.w	@er2,r2
	cmp.w	r2,r3
	bne	000086B0

l0000868A:
	mov.w	@(14:16,sp),r2
	adds	#0x00000002,er2
	add.w	r2,r2
	add.w	r4,r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	cmp.w	r2,r3
	bne	000086B0

l000086A2:
	mov.w	r3,r3
	beq	000086B0

l000086A6:
	mov.w	@(12:16,sp),r5
	add.w	r3,r5
	mov.w	r5,@(12:16,sp)

l000086B0:
	mov.w	@(6:16,sp),r4
	add.b	#0x0C,r4l
	addx.b	#0x00,r4h
	mov.w	r4,@(6:16,sp)
	adds	#0x00000001,er1
	mov.w	#0x0004,r2
	cmp.w	r2,r1
	ble	0000866C

l000086C6:
	mov.w	r6,@(14:16,sp)
	mov.w	#0x0003,r2
	mov.w	r6,r5
	cmp.w	r2,r5
	bgt	000086D8

l000086D4:
	jmp	@0x8644:24

l000086D8:
	sub.w	r4,r4
	mov.w	r4,@(14:16,sp)

l000086DE:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r6
	adds	#0x00000001,er6
	mov.w	@(14:16,sp),r5
	mov.w	r5,@(10:16,sp)
	mov.w	r5,r4
	add.w	r4,r4
	mov.w	r4,@(10:16,sp)

l000086F6:
	mov.w	r1,r2
	add.w	r2,r2
	add.w	r1,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(10:16,sp),r0
	add.w	r2,r0
	mov.w	#0x9EB0,r5
	add.w	r0,r5
	mov.w	r5,r0
	mov.w	r6,r3
	add.w	r3,r3
	mov.w	r1,@(6:16,sp)
	mov.w	r1,r4
	adds	#0x00000001,er4
	mov.w	r4,@(6:16,sp)
	mov.w	@(6:16,sp),r2
	add.w	r2,r2
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r3
	mov.w	#0x9EB0,r5
	add.w	r3,r5
	mov.w	r5,r3
	mov.w	@er0,r0
	mov.w	r0,@(8:16,sp)
	mov.w	@er3,r2
	cmp.w	r2,r0
	bne	00008778

l00008740:
	mov.w	@(14:16,sp),r3
	adds	#0x00000002,er3
	add.w	r3,r3
	mov.w	r1,r0
	adds	#0x00000002,er0
	mov.w	r0,r2
	add.w	r2,r2
	add.w	r0,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r3
	mov.w	#0x9EB0,r4
	add.w	r3,r4
	mov.w	r4,r3
	mov.w	@er3,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r2,r5
	bne	00008778

l0000876A:
	mov.w	r5,r5
	beq	00008778

l0000876E:
	mov.w	@(12:16,sp),r4
	add.w	r5,r4
	mov.w	r4,@(12:16,sp)

l00008778:
	mov.w	@(6:16,sp),r1
	mov.w	#0x0002,r2
	cmp.w	r2,r1
	bgt	00008788

l00008784:
	jmp	@0x86F6:24

l00008788:
	mov.w	r6,@(14:16,sp)
	mov.w	#0x0003,r2
	mov.w	r6,r5
	cmp.w	r2,r5
	bgt	0000879A

l00008796:
	jmp	@0x86DE:24

l0000879A:
	sub.w	r4,r4
	mov.w	r4,@(14:16,sp)

l000087A0:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r6
	adds	#0x00000001,er6
	mov.w	@(14:16,sp),r5
	mov.w	r5,@(8:16,sp)
	mov.w	r5,r4
	add.w	r4,r4
	mov.w	r4,@(8:16,sp)

l000087B8:
	mov.w	r1,r3
	adds	#0x00000002,er3
	mov.w	r3,r2
	add.w	r2,r2
	add.w	r3,r2
	add.w	r2,r2
	add.w	r2,r2
	mov.w	@(8:16,sp),r0
	add.w	r2,r0
	mov.w	#0x9EB0,r5
	add.w	r0,r5
	mov.w	r5,r0
	mov.w	r6,r3
	add.w	r3,r3
	mov.w	r1,@(6:16,sp)
	mov.w	r1,r4
	adds	#0x00000001,er4
	mov.w	r4,@(6:16,sp)
	mov.w	@(6:16,sp),r2
	add.w	r2,r2
	add.w	r4,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r3
	mov.w	#0x9EB0,r5
	add.w	r3,r5
	mov.w	r5,r3
	mov.w	@er0,r0
	mov.w	@er3,r2
	cmp.w	r2,r0
	bne	00008832

l00008802:
	mov.w	@(14:16,sp),r2
	adds	#0x00000002,er2
	add.w	r2,r2
	mov.w	r1,r3
	add.w	r3,r3
	add.w	r1,r3
	add.w	r3,r3
	add.w	r3,r3
	add.w	r3,r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	cmp.w	r2,r0
	bne	00008832

l00008824:
	mov.w	r0,r0
	beq	00008832

l00008828:
	mov.w	@(12:16,sp),r5
	add.w	r0,r5
	mov.w	r5,@(12:16,sp)

l00008832:
	mov.w	@(6:16,sp),r1
	mov.w	#0x0002,r2
	cmp.w	r2,r1
	bgt	00008842

l0000883E:
	jmp	@0x87B8:24

l00008842:
	mov.w	r6,@(14:16,sp)
	mov.w	#0x0003,r2
	mov.w	r6,r4
	cmp.w	r2,r4
	bgt	00008854

l00008850:
	jmp	@0x87A0:24

l00008854:
	mov.w	@(12:16,sp),r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x000A,r3
	add.w	r3,r7
	rts

;; fn00008866: 00008866
;;   Called from:
;;     000089AC (in fn00008866)
;;     000096D6 (in fn00009478)
fn00008866 proc
	mov.w	#0x0018,r3
	sub.w	r3,r7
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	r0,@(24:16,sp)
	mov.w	r1,@(22:16,sp)
	mov.w	r2,@(20:16,sp)
	mov.w	#0xFFFF,r2
	mov.w	r2,@er0
	mov.w	@(22:16,sp),r4
	mov.w	r2,@er4
	sub.w	r5,r5
	mov.w	r5,@(18:16,sp)
	mov.w	#0x9EF0,r4
	mov.w	r4,@(14:16,sp)
	mov.w	r5,@(12:16,sp)

l0000889C:
	mov.w	@(18:16,sp),r2
	add.w	r2,r2
	mov.w	#0x9EF0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	bgt	000088B2

l000088AE:
	jmp	@0x8A7A:24

l000088B2:
	mov.w	@(14:16,sp),r5
	mov.w	@er5,r2
	subs	#0x00000001,er2
	mov.w	r2,@er5
	add.w	r2,r2
	mov.w	@(12:16,sp),r4
	add.w	r4,r2
	mov.w	@(20:16,sp),r5
	mov.w	r5,@(-24912:16,er2)
	mov.w	@0x9E40:16,r2
	mov.w	@(32:16,sp),r4
	cmp.w	r2,r4
	beq	000088DC

l000088D8:
	jmp	@0x8974:24

l000088DC:
	jsr	@fn00008584
	mov.w	r0,@(6:16,sp)
	mov.w	r0,r5
	add.w	r5,r5
	mov.w	r5,r4
	add.w	r0,r4
	add.w	r4,r4
	add.w	r4,r4
	add.w	r4,r4
	mov.w	r4,r5
	add.w	r0,r5
	add.w	r5,r5
	add.w	r5,r5
	mov.w	r5,@(6:16,sp)
	jsr	@fn00008032
	sub.w	r4,r4
	mov.w	r4,@(16:16,sp)

l00008908:
	sub.w	r3,r3
	mov.w	@(16:16,sp),r2
	add.w	r2,r2
	mov.w	r2,r6
	mov.w	#0x9E42,r5
	add.w	r6,r5
	mov.w	r5,r6
	mov.w	r2,@(8:16,sp)
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,@(8:16,sp)

l00008928:
	mov.w	@(8:16,sp),r5
	mov.w	@er5,r0
	add.b	#0x0C,r5l
	addx.b	#0x00,r5h
	mov.w	r5,@(8:16,sp)
	mov.w	@er6,r1
	add.b	#0x0C,r6l
	addx.b	#0x00,r6h
	mov.w	r3,@(10:16,sp)
	jsr	@fn00009E08
	mov.w	@(6:16,sp),r4
	add.w	r0,r4
	mov.w	r4,@(6:16,sp)
	mov.w	@(10:16,sp),r3
	adds	#0x00000001,er3
	mov.w	#0x0004,r2
	cmp.w	r2,r3
	ble	00008928

l0000895C:
	mov.w	@(16:16,sp),r5
	adds	#0x00000001,er5
	mov.w	r5,@(16:16,sp)
	mov.w	#0x0005,r2
	cmp.w	r2,r5
	ble	00008908

l0000896E:
	mov.w	r4,@(28:16,sp)
	bra	000089B4

l00008974:
	mov.w	@(20:16,sp),r2
	not.b	r2l
	not.b	r2h
	mov.w	@(22:16,sp),r4
	mov.w	@er4,r3
	mov.w	r3,@-sp
	mov.w	@(34:16,sp),r5
	adds	#0x00000001,er5
	mov.w	r5,@(34:16,sp)
	mov.w	@(34:16,sp),r4
	mov.w	r4,@-sp
	mov.w	@(36:16,sp),r5
	subs	#0x00000001,er5
	mov.w	r5,@(36:16,sp)
	adds	#0x00000001,er2
	mov.w	#0x0020,r1
	add.w	r7,r1
	mov.w	#0x001E,r0
	add.w	r7,r0
	jsr	@fn00008866
	adds	#0x00000002,sp
	adds	#0x00000002,sp

l000089B4:
	mov.w	@(14:16,sp),r4
	mov.w	@er4,r2
	mov.w	r2,r3
	add.w	r3,r3
	mov.w	@(12:16,sp),r5
	add.w	r5,r3
	sub.w	r0,r0
	mov.w	r0,@(-24912:16,er3)
	adds	#0x00000001,er2
	mov.w	r2,@er4
	mov.w	#0xFFFF,r6
	mov.w	@(20:16,sp),r4
	cmp.w	r6,r4
	bne	000089E8

l000089DA:
	mov.w	@(28:16,sp),r3
	mov.w	@(22:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r2,r3
	blt	00008A22

l000089E8:
	mov.w	#0x0001,r2
	mov.w	@(20:16,sp),r4
	cmp.w	r2,r4
	bne	00008A02

l000089F4:
	mov.w	@(28:16,sp),r3
	mov.w	@(22:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r2,r3
	bgt	00008A22

l00008A02:
	jsr	@fn00009B66
	btst	#0x00,r0l
	beq	00008A18

l00008A0A:
	mov.w	@(28:16,sp),r3
	mov.w	@(22:16,sp),r4
	mov.w	@er4,r2
	cmp.w	r2,r3
	beq	00008A22

l00008A18:
	mov.w	@(24:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r6,r2
	bne	00008A36

l00008A22:
	mov.w	@(18:16,sp),r5
	mov.w	@(24:16,sp),r4
	mov.w	r5,@er4
	mov.w	@(28:16,sp),r2
	mov.w	@(22:16,sp),r4
	mov.w	r2,@er4

l00008A36:
	mov.w	#0xFFFF,r2
	mov.w	@(20:16,sp),r5
	cmp.w	r2,r5
	bne	00008A58

l00008A42:
	mov.w	@(34:16,sp),r4
	cmp.w	r5,r4
	beq	00008A58

l00008A4A:
	mov.w	@(22:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r2,r4
	ble	00008A58

l00008A54:
	jmp	@0x8ADC:24

l00008A58:
	mov.w	#0x0001,r2
	mov.w	@(20:16,sp),r4
	cmp.w	r2,r4
	bne	00008A7A

l00008A64:
	mov.w	#0xFFFF,r2
	mov.w	@(34:16,sp),r5
	cmp.w	r2,r5
	beq	00008A7A

l00008A70:
	mov.w	@(22:16,sp),r4
	mov.w	@er4,r2
	cmp.w	r2,r5
	blt	00008ADC

l00008A7A:
	mov.w	@(32:16,sp),r5
	bne	00008AB0

l00008A80:
	mov.w	#0x3002,r2
	mov.w	r2,@-sp
	mov.w	@(24:16,sp),r4
	mov.w	@er4,r2
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6

l00008AB0:
	mov.w	@(14:16,sp),r5
	adds	#0x00000002,er5
	mov.w	r5,@(14:16,sp)
	mov.w	@(12:16,sp),r4
	add.b	#0x0C,r4l
	addx.b	#0x00,r4h
	mov.w	r4,@(12:16,sp)
	mov.w	@(18:16,sp),r5
	adds	#0x00000001,er5
	mov.w	r5,@(18:16,sp)
	mov.w	#0x0004,r2
	cmp.w	r2,r5
	bgt	00008ADC

l00008AD8:
	jmp	@0x889C:24

l00008ADC:
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x0018,r3
	add.w	r3,r7
	rts

;; fn00008AEA: 00008AEA
;;   Called from:
;;     000096EC (in fn00009478)
fn00008AEA proc
	mov.w	r4,@-sp
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x001E,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2001,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x00C8,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2001,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x00D2,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2001,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x001E,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	mov.w	@sp+,r4
	rts

;; fn00008BB0: 00008BB0
;;   Called from:
;;     00009070 (in fn00008F4E)
;;     00009136 (in fn00008F4E)
;;     00009210 (in fn00008F4E)
;;     0000931E (in fn00008F4E)
;;     000093BC (in fn00009370)
;;     000096E8 (in fn00009478)
;;     0000977C (in fn00009478)
fn00008BB0 proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	r0,@(8:16,sp)
	mov.w	r1,@(6:16,sp)
	mov.w	@0x9E36:16,r2
	cmp.w	r1,r2
	bne	00008BCE

l00008BCA:
	jmp	@0x8DA2:24

l00008BCE:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008BE6:
	mov.w	#0x0001,r0
	jsr	@fn00009C92
	mov.b	r0l,r0l
	beq	00008BE6

l00008BF2:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E34:16
	mov.w	@0x9E36:16,r2
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	blt	00008C22

l00008C1E:
	jmp	@0x8CD2:24

l00008C22:
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	cmp.w	r5,r2
	blt	00008C42

l00008C3E:
	jmp	@0x8D8A:24

l00008C42:
	mov.w	#0x000A,r6

l00008C46:
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	r0,r4
	mov.w	@0x9E3A:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	00008C92

l00008C60:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	00008C92

l00008C68:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	adds	#0x00000001,er2
	mov.w	r2,@0x9E36:16
	mov.w	@0x9E7E:16,r3

l00008C92:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	00008CBE

l00008C9A:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	00008CBE

l00008CA2:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008CBE:
	mov.w	@0x9E36:16,r2
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	bge	00008CCE

l00008CCA:
	jmp	@0x8C46:24

l00008CCE:
	jmp	@0x8D8A:24

l00008CD2:
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	bgt	00008CDE

l00008CDA:
	jmp	@0x8DA2:24

l00008CDE:
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	cmp.w	r5,r2
	bgt	00008CFE

l00008CFA:
	jmp	@0x8D8A:24

l00008CFE:
	mov.w	#0x000A,r6

l00008D02:
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	r0,r4
	mov.w	@0x9E3A:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	00008D4E

l00008D1C:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	00008D4E

l00008D24:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	subs	#0x00000001,er2
	mov.w	r2,@0x9E36:16
	mov.w	@0x9E7E:16,r3

l00008D4E:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	00008D7A

l00008D56:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	00008D7A

l00008D5E:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008D7A:
	mov.w	@0x9E36:16,r2
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	ble	00008D8A

l00008D86:
	jmp	@0x8D02:24

l00008D8A:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008DA2:
	mov.w	@0x9E34:16,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	blt	00008DB2

l00008DAE:
	jmp	@0x8E6E:24

l00008DB2:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0014,r0
	jsr	@fn00009C18
	mov.w	@0x9E34:16,r2
	cmp.w	r5,r2
	blt	00008DDE

l00008DDA:
	jmp	@0x8F2A:24

l00008DDE:
	mov.w	#0x000A,r6

l00008DE2:
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	r0,r4
	mov.w	@0x9E38:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	00008E2E

l00008DFC:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	00008E2E

l00008E04:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E34:16,r2
	adds	#0x00000001,er2
	mov.w	r2,@0x9E34:16
	mov.w	@0x9E7E:16,r3

l00008E2E:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	00008E5A

l00008E36:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	00008E5A

l00008E3E:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008E5A:
	mov.w	@0x9E34:16,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	bge	00008E6A

l00008E66:
	jmp	@0x8DE2:24

l00008E6A:
	jmp	@0x8F2A:24

l00008E6E:
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	bgt	00008E7A

l00008E76:
	jmp	@0x8F2A:24

l00008E7A:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E34:16,r2
	cmp.w	r5,r2
	bgt	00008E9E

l00008E9A:
	jmp	@0x8F2A:24

l00008E9E:
	mov.w	#0x000A,r6

l00008EA2:
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	r0,r4
	mov.w	@0x9E38:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	00008EEE

l00008EBC:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	00008EEE

l00008EC4:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@0x9E34:16,r2
	subs	#0x00000001,er2
	mov.w	r2,@0x9E34:16
	mov.w	@0x9E7E:16,r3

l00008EEE:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	00008F1A

l00008EF6:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	00008F1A

l00008EFE:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008F1A:
	mov.w	@0x9E34:16,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	ble	00008F2A

l00008F26:
	jmp	@0x8EA2:24

l00008F2A:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts

;; fn00008F4E: 00008F4E
;;   Called from:
;;     0000954E (in fn00009478)
fn00008F4E proc
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E34:16
	mov.w	r2,@0x9E36:16
	mov.w	#0x1000,r1
	mov.w	#0x1946,r0
	jsr	@fn00009B9A
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp

l00008F86:
	mov.w	#0x0001,r0
	jsr	@fn00009C92
	mov.b	r0l,r0l
	beq	00008F86

l00008F92:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E34:16
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x01C2,r0
	jsr	@fn00009C18
	sub.w	r6,r6
	mov.w	#0x07D0,r5
	mov.w	r5,@(6:16,sp)
	mov.w	#0x00C8,r1
	sub.w	r0,r0
	jsr	@fn00009BF4
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	bra	00009010

l00008FF8:
	sub.w	r0,r0
	jsr	@fn00009C6E
	cmp.w	r6,r0
	ble	00009004

l00009002:
	mov.w	r0,r6

l00009004:
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r0
	bge	00009010

l0000900C:
	mov.w	r0,@(6:16,sp)

l00009010:
	sub.w	r0,r0
	jsr	@fn00009BEC
	mov.w	r0,r0
	bgt	00008FF8

l0000901A:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@(6:16,sp),r3
	add.w	r6,r3
	mov.w	r3,r2
	shll.b	r2l
	rotxl.b	r2h
	bst	#0x00,r2l
	and.b	#0x01,r2l
	and.b	#0x00,r2h
	add.w	r3,r2
	shar.b	r2h
	rotxr.b	r2l
	mov.w	r2,@0x9E3A:16
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp
	sub.w	r1,r1
	mov.w	#0xFFFF,r0
	jsr	@fn00008BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0096,r0
	jsr	@fn00009C18
	sub.w	r6,r6
	mov.w	#0x07D0,r5
	mov.w	r5,@(6:16,sp)
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	bra	000090C8

l000090B0:
	sub.w	r0,r0
	jsr	@fn00009C6E
	cmp.w	r6,r0
	ble	000090BC

l000090BA:
	mov.w	r0,r6

l000090BC:
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r0
	bge	000090C8

l000090C4:
	mov.w	r0,@(6:16,sp)

l000090C8:
	mov.w	#0x0001,r0
	jsr	@fn00009C92
	mov.b	r0l,r0l
	beq	000090B0

l000090D4:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@(6:16,sp),r3
	add.w	r6,r3
	mov.w	r3,r2
	shll.b	r2l
	rotxl.b	r2h
	bst	#0x00,r2l
	and.b	#0x01,r2l
	and.b	#0x00,r2h
	add.w	r3,r2
	shar.b	r2h
	rotxr.b	r2l
	mov.w	r2,@0x9E38:16
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp
	sub.w	r2,r2
	mov.w	r2,@0x9E3E:16
	mov.w	#0x07D0,r2
	mov.w	r2,@0x9E3C:16
	sub.w	r1,r1
	sub.w	r0,r0
	jsr	@fn00008BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0064,r1
	sub.w	r0,r0
	jsr	@fn00009BF4
	bra	000091B0

l00009192:
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	@0x9E3E:16,r2
	cmp.w	r2,r0
	ble	000091A4

l000091A0:
	mov.w	r0,@0x9E3E:16

l000091A4:
	mov.w	@0x9E3C:16,r2
	cmp.w	r2,r0
	bge	000091B0

l000091AC:
	mov.w	r0,@0x9E3C:16

l000091B0:
	sub.w	r0,r0
	jsr	@fn00009BEC
	mov.w	r0,r0
	bgt	00009192

l000091BA:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	mov.w	#0x0005,r1
	sub.w	r0,r0
	jsr	@fn00008BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0064,r1
	sub.w	r0,r0
	jsr	@fn00009BF4
	bra	0000928A

l0000926C:
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	@0x9E3E:16,r2
	cmp.w	r2,r0
	ble	0000927E

l0000927A:
	mov.w	r0,@0x9E3E:16

l0000927E:
	mov.w	@0x9E3C:16,r2
	cmp.w	r2,r0
	bge	0000928A

l00009286:
	mov.w	r0,@0x9E3C:16

l0000928A:
	sub.w	r0,r0
	jsr	@fn00009BEC
	mov.w	r0,r0
	bgt	0000926C

l00009294:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	mov.w	@0x9E3E:16,r2
	add.b	#0x28,r2l
	addx.b	#0x00,r2h
	mov.w	r2,@0x9E3E:16
	mov.w	@0x9E3C:16,r3
	add.b	#0xFB,r3l
	addx.b	#0xFF,r3h
	mov.w	r3,@0x9E3C:16
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp
	sub.w	r1,r1
	mov.w	#0xFFFF,r0
	jsr	@fn00008BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x012C,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E36:16
	mov.w	#0x1000,r1
	mov.w	#0x1946,r0
	jsr	@fn00009B9A
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	rts

;; fn00009370: 00009370
;;   Called from:
;;     000095CE (in fn00009478)
fn00009370 proc
	mov.w	#0x0006,r3
	sub.w	r3,r7
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	#0xFFFF,r3
	mov.w	r3,@(10:16,sp)
	sub.w	r3,r3
	mov.w	r3,@(8:16,sp)
	sub.w	r4,r4
	sub.w	r6,r6
	mov.w	#0x9EF0,r3
	mov.w	r3,@(6:16,sp)

l00009396:
	mov.w	@(6:16,sp),r3
	mov.w	@er3+,r1
	mov.w	r3,@(6:16,sp)
	subs	#0x00000001,er1
	mov.w	r1,r1
	bge	000093AA

l000093A6:
	jmp	@0x944E:24

l000093AA:
	mov.w	r1,r2
	add.w	r2,r2
	add.w	r6,r2
	mov.w	@(-24912:16,er2),r2
	beq	000093BA

l000093B6:
	jmp	@0x944E:24

l000093BA:
	mov.w	r4,r0
	jsr	@fn00008BB0
	mov.w	#0x0007,r5
	mov.w	r5,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn00009C18
	mov.w	r5,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18
	sub.w	r0,r0
	jsr	@fn00009C6E
	mov.w	@0x9E3E:16,r2
	cmp.w	r2,r0
	ble	00009416

l0000940A:
	mov.w	r4,@(10:16,sp)
	mov.w	#0x0001,r3
	mov.w	r3,@(8:16,sp)

l00009416:
	mov.w	r5,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn00009C18
	mov.w	r5,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn00009C18

l0000944E:
	add.b	#0x0C,r6l
	addx.b	#0x00,r6h
	adds	#0x00000001,er4
	mov.w	#0x0004,r2
	cmp.w	r2,r4
	bgt	00009466

l0000945C:
	mov.w	@(8:16,sp),r3
	bne	00009466

l00009462:
	jmp	@0x9396:24

l00009466:
	mov.w	@(10:16,sp),r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x0006,r3
	add.w	r3,r7
	rts

;; fn00009478: 00009478
;;   Called from:
;;     00009B3A (in fn00009AF8)
fn00009478 proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	sub.w	r0,r0
	jsr	@fn00009BFC
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp
	sub.w	r0,r0
	jsr	@fn00009BFC
	mov.w	r0,r1
	bld	#0x07,r1h
	subx.b	r0l,r0l
	subx.b	r0h,r0h
	jsr	@fn00009B54
	sub.b	r2l,r2l
	mov.b	#0x03,r1l
	sub.w	r0,r0
	jsr	@fn00009C34
	mov.b	#0x20,r2l
	mov.b	#0x01,r1l
	mov.w	#0x0001,r0
	jsr	@fn00009C34
	mov.w	#0x1001,r1
	mov.w	#0x19C4,r0
	jsr	@fn00009B9A

l000094DA:
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6
	sub.w	r0,r0
	sub.w	r1,r1
	mov.w	#0x0005,r4

l000094F0:
	sub.w	r3,r3
	mov.w	r0,r2
	add.w	r2,r2
	mov.w	#0x9EB0,r5
	add.w	r2,r5
	mov.w	r5,r2

l000094FE:
	mov.w	r1,@er2
	add.b	#0x0C,r2l
	addx.b	#0x00,r2h
	adds	#0x00000001,er3
	mov.w	#0x0004,r5
	cmp.w	r5,r3
	ble	000094FE

l0000950E:
	adds	#0x00000001,er0
	cmp.w	r4,r0
	ble	000094F0

l00009514:
	sub.w	r3,r3
	mov.w	#0x0004,r1
	mov.w	#0x0006,r0
	mov.w	#0x9EF0,r2

l00009522:
	mov.w	r0,@er2
	adds	#0x00000002,er2
	adds	#0x00000001,er3
	cmp.w	r1,r3
	ble	00009522

l0000952C:
	mov.w	#0xFFFF,r6
	jmp	@0x97D6:24

l00009534:
	jsr	@fn00009CBC
	mov.b	#0x00,r0h
	mov.w	#0x0004,r5
	cmp.w	r5,r0
	bne	00009546

l00009542:
	jmp	@0x9800:24

l00009546:
	mov.w	#0x0002,r2
	cmp.w	r2,r0
	bne	00009556

l0000954E:
	jsr	@fn00008F4E
	jmp	@0x97D6:24

l00009556:
	cmp.w	r4,r0
	beq	0000955E

l0000955A:
	jmp	@0x95BE:24

l0000955E:
	jsr	@fn00009D6A
	jsr	@fn00008584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	ble	00009574

l0000956E:
	mov.w	#0x0004,r2
	bra	00009584

l00009574:
	jsr	@fn00008584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	bge	00009590

l00009580:
	mov.w	#0x0005,r2

l00009584:
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6

l00009590:
	jsr	@fn00008584
	mov.w	r0,@0x9E80:16
	jsr	@fn00008584
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp
	jmp	@0x97D6:24

l000095BE:
	mov.w	#0x1000,r1
	mov.w	#0x1946,r0
	jsr	@fn00009B9A
	jsr	@fn00009DF2
	jsr	@fn00009370
	mov.w	r0,@(6:16,sp)
	mov.w	r0,r0
	bge	000095DE

l000095DA:
	jmp	@0x96A0:24

l000095DE:
	mov.w	#0x0014,r2
	mov.w	r2,@-sp
	mov.w	#0x09C4,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x000A,r4
	mov.w	r4,@-sp
	mov.w	#0x05DC,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0DAC,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	@(6:16,sp),r1
	mov.w	r1,r2
	add.w	r2,r2
	mov.w	r2,r0
	mov.w	#0x9EF0,r5
	add.w	r0,r5
	mov.w	r5,r0
	mov.w	@er0,r3
	subs	#0x00000001,er3
	mov.w	r3,@er0
	add.w	r3,r3
	add.w	r1,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r3
	mov.w	r6,@(-24912:16,er3)
	jsr	@fn00008584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	ble	0000965A

l00009654:
	mov.w	#0x0004,r2
	bra	0000966A

l0000965A:
	jsr	@fn00008584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	bge	00009676

l00009666:
	mov.w	#0x0005,r2

l0000966A:
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6

l00009676:
	jsr	@fn00008584
	mov.w	r0,@0x9E80:16
	jsr	@fn00008584
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp

l000096A0:
	mov.w	#0x0001,r0
	sub.w	r3,r3
	mov.w	#0x0004,r4
	mov.w	#0x9EF0,r1

l000096AE:
	mov.w	@er1+,r2
	ble	000096B4

l000096B2:
	sub.w	r0,r0

l000096B4:
	adds	#0x00000001,er3
	cmp.w	r4,r3
	ble	000096AE

l000096BA:
	mov.w	r0,r0
	beq	000096C2

l000096BE:
	jmp	@0x9776:24

l000096C2:
	mov.w	r6,@-sp
	mov.w	r0,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x000C,r1
	add.w	r7,r1
	mov.w	#0x000A,r0
	add.w	r7,r0
	jsr	@fn00008866
	mov.w	@(10:16,sp),r0
	mov.w	r0,r2
	add.w	r2,r2
	mov.w	@(-24848:16,er2),r1
	subs	#0x00000001,er1
	jsr	@fn00008BB0
	jsr	@fn00008AEA
	mov.w	@(10:16,sp),r1
	mov.w	#0x0001,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	mov.w	r1,r2
	add.w	r2,r2
	mov.w	r2,r0
	mov.w	#0x9EF0,r5
	add.w	r0,r5
	mov.w	r5,r0
	mov.w	@er0,r3
	subs	#0x00000001,er3
	mov.w	r3,@er0
	add.w	r3,r3
	add.w	r1,r2
	add.w	r2,r2
	add.w	r2,r2
	add.w	r2,r3
	mov.w	r4,@(-24912:16,er3)
	jsr	@fn00008584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	ble	00009730

l0000972A:
	mov.w	#0x0004,r2
	bra	00009740

l00009730:
	jsr	@fn00008584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	bge	0000974C

l0000973C:
	mov.w	#0x0005,r2

l00009740:
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6

l0000974C:
	jsr	@fn00008584
	mov.w	r0,@0x9E80:16
	jsr	@fn00008584
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn00009BB6
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	adds	#0x00000002,sp

l00009776:
	sub.w	r1,r1
	mov.w	#0xFFFF,r0
	jsr	@fn00008BB0
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	#0x012C,r0
	jsr	@fn00009C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn00009BB6
	adds	#0x00000002,sp
	mov.w	r6,@0x9E36:16
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6
	mov.w	#0x1000,r1
	mov.w	#0x19C4,r0
	jsr	@fn00009B9A
	jsr	@fn00009DDC

l000097D6:
	sub.w	r5,r5
	bne	00009800

l000097DA:
	mov.w	#0x0001,r0
	sub.w	r3,r3
	mov.w	#0x0004,r4
	mov.w	#0x9EF0,r1

l000097E8:
	mov.w	@er1+,r2
	ble	000097EE

l000097EC:
	sub.w	r0,r0

l000097EE:
	adds	#0x00000001,er3
	cmp.w	r4,r3
	ble	000097E8

l000097F4:
	mov.w	#0x0008,r4
	mov.w	r0,r0
	bne	00009800

l000097FC:
	jmp	@0x9534:24

l00009800:
	mov.w	#0x0002,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6
	jsr	@fn00009CBC
	cmp.b	#0x08,r0l
	beq	0000981C

l00009818:
	jmp	@0x94DA:24

l0000981C:
	sub.w	r0,r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts
0000982A                               79 03 00 06 19 37           y....7
00009830 6D F4 6D F5 6D F6 5E 00 85 84 0D 06 09 66 09 06 m.m.m.^......f..
00009840 09 66 09 66 09 66 09 06 09 66 09 66 5E 00 80 32 .f.f.f...f.f^..2
00009850 19 33 6F F3 00 0A 19 33 6F F3 00 06 6F 72 00 0A .3o....3o...or..
00009860 09 22 0D 25 79 03 9E 42 09 53 0D 35 0D 24 79 03 .".%y..B.S.5.$y.
00009870 9E B0 09 43 0D 34 69 40 8C 0C 94 00 69 51 8D 0C ...C.4i@....iQ..
00009880 95 00 5E 00 9E 08 09 06 6F 73 00 06 0B 03 6F F3 ..^.....os....o.
00009890 00 06 79 02 00 04 1D 23 4F DC 6F 73 00 0A 0B 03 ..y....#O.os....
000098A0 6F F3 00 0A 79 02 00 05 1D 23 4F AA 0D 60 6D 76 o...y....#O..`mv
000098B0 6D 75 6D 74 79 03 00 06 09 37 54 70 6D F4 6D F5 mumty....7Tpm.m.
000098C0 0D 03 09 33 0D 34 79 05 9E F0 09 45 0D 54 69 42 ...3.4y....E.TiB
000098D0 1B 02 69 C2 09 22 09 03 09 33 09 33 09 32 6F A1 ..i.."...3.3.2o.
000098E0 9E B0 6D 75 6D 74 54 70 6D F4 6D F5 0D 02 09 22 ..mumtTpm.m...."
000098F0 0D 24 79 05 9E F0 09 45 0D 54 69 41 0D 13 09 33 .$y....E.TiA...3
00009900 09 02 09 22 09 22 09 23 19 22 6F B2 9E B0 0B 01 ...".".#."o.....
00009910 69 C1 6D 75 6D 74 54 70 6D F4 6D F5 6D F6 19 11 i.mumtTpm.m.m...
00009920 79 06 00 04 79 05 9E B0 19 44 19 00 0D 13 09 33 y...y....D.....3
00009930 0D 32 09 52 69 A4 8B 0C 93 00 0B 00 1D 60 4F F0 .2.Ri........`O.
00009940 79 02 00 05 79 03 00 04 0B 01 1D 21 4F DC 19 00 y...y......!O...
00009950 0D 31 79 03 00 06 79 02 9E F0 69 A3 0B 82 0B 00 .1y...y...i.....
00009960 1D 10 4F F6 6D 76 6D 75 6D 74 54 70 6D F4 79 00 ..O.mvmumtTpm.y.
00009970 00 01 19 33 79 04 00 04 79 01 9E F0 6D 12 4F 02 ...3y...y...m.O.
00009980 19 00 0B 03 1D 43 4F F4 6D 74 54 70 6D F4 0D 02 .....CO.mtTpm...
00009990 79 04 00 07 6D F4 79 01 20 02 79 00 1A 4E 5E 00 y...m.y. .y..N^.
000099A0 9B B6 0B 87 79 00 00 2A 5E 00 9C 18 6D F4 79 02 ....y..*^...m.y.
000099B0 00 03 79 01 20 02 79 00 1A 4E 5E 00 9B B6 0B 87 ..y. .y..N^.....
000099C0 79 00 00 55 5E 00 9C 18 6D 74 54 70 6D F4 0D 02 y..U^...mtTpm...
000099D0 79 04 00 07 6D F4 79 01 20 02 79 00 1A 4E 5E 00 y...m.y. .y..N^.
000099E0 9B B6 0B 87 79 00 00 1E 5E 00 9C 18 6D F4 79 02 ....y...^...m.y.
000099F0 00 03 79 01 20 02 79 00 1A 4E 5E 00 9B B6 0B 87 ..y. .y..N^.....
00009A00 79 00 00 55 5E 00 9C 18 6D 74 54 70 79 02 00 07 y..U^...mtTpy...
00009A10 6D F2 79 02 00 01 79 01 20 00 79 00 1A 4E 5E 00 m.y...y. .y..N^.
00009A20 9B B6 0B 87 79 00 00 01 5E 00 9C 92 0C 88 47 F4 ....y...^.....G.
00009A30 79 02 00 07 6D F2 79 02 00 03 79 01 20 00 79 00 y...m.y...y. .y.
00009A40 1A 4E 5E 00 9B B6 0B 87 79 02 FF FF 6B 82 9E 34 .N^.....y...k..4
00009A50 54 70 6D F4 19 11 79 00 FF FF 5E 00 8B B0 79 04 Tpm...y...^...y.
00009A60 00 07 6D F4 79 02 00 02 79 01 20 02 79 00 1A 4E ..m.y...y. .y..N
00009A70 5E 00 9B B6 0B 87 79 00 01 2C 5E 00 9C 18 6D F4 ^.....y..,^...m.
00009A80 79 02 00 03 79 01 20 02 79 00 1A 4E 5E 00 9B B6 y...y. .y..N^...
00009A90 0B 87 79 02 FF FF 6B 82 9E 36 6D 74 54 70 5E 00 ..y...k..6mtTp^.
00009AA0 85 84 6B 02 9E 80 1D 02 4F 06 79 02 00 04 40 10 ..k.....O.y...@.
00009AB0 5E 00 85 84 6B 02 9E 80 1D 02 4C 10 79 02 00 05 ^...k.....L.y...
00009AC0 79 01 40 04 79 00 29 9A 5E 00 9B A6 5E 00 85 84 y.@.y.).^...^...
00009AD0 6B 80 9E 80 5E 00 85 84 0D 02 79 03 30 02 6D F3 k...^.....y.0.m.
00009AE0 79 01 30 01 79 00 1F F2 5E 00 9B B6 79 00 27 C8 y.0.y...^...y.'.
00009AF0 5E 00 9B 90 0B 87 54 70                         ^.....Tp        

;; fn00009AF8: 00009AF8
;;   Called from:
;;     8008 (in fn8000)
fn00009AF8 proc
	mov.w	#0x9E90,r2
	mov.w	#0x9F38,r3
	cmp.w	r3,r2
	beq	00009B0E

l00009B04:
	sub.b	r0l,r0l

l00009B06:
	mov.b	r0l,@er2
	adds	#0x00000001,er2
	cmp.w	r3,r2
	bne	00009B06

l00009B0E:
	mov.w	#0x9F10,r1
	mov.w	#0x9F00,r2
	mov.w	#0x3B9A,r0
	jsr	@fn00009BA6
	mov.w	#0x2964,r0
	jsr	@fn00009B90
	mov.w	#0x1498,r0
	jsr	@fn00009B90
	mov.w	#0x1ABA,r0
	jsr	@fn00009B90
	jsr	@fn00009DDC
	jsr	@fn00009478
	mov.w	#0x3ED4,r0
	jsr	@fn00009B90
	mov.b	#0x01,r2l
	mov.b	r2l,@0xFFCC:16
	mov.w	@0x0:16,r2
	jsr	@er2
	rts

;; fn00009B54: 00009B54
;;   Called from:
;;     000094B4 (in fn00009478)
fn00009B54 proc
	xor.b	#0x7B,r1l
	xor.b	#0xDB,r1h
	xor.b	#0x68,r0l
	xor.b	#0x16,r0h
	mov.w	r0,@0x9E90:16
	mov.w	r1,@0x9E92:16
	rts

;; fn00009B66: 00009B66
;;   Called from:
;;     00008A02 (in fn00008866)
fn00009B66 proc
	mov.w	@0x9E90:16,r0
	mov.w	@0x9E92:16,r1
	mov.w	#0x0001,r2
	mov.w	#0x0DCD,r3
	jsr	@fn00009E18
	add.b	#0x01,r1l
	addx.b	#0x00,r1h
	addx.b	#0x00,r0l
	addx.b	#0x00,r0h
	mov.w	r0,@0x9E90:16
	mov.w	r1,@0x9E92:16
	mov.w	@0x9E92:16,r0
	rts

;; fn00009B90: 00009B90
;;   Called from:
;;     00008A9C (in fn00008866)
;;     00009064 (in fn00008F4E)
;;     0000911E (in fn00008F4E)
;;     00009312 (in fn00008F4E)
;;     000094A0 (in fn00009478)
;;     000095B4 (in fn00009478)
;;     0000969A (in fn00009478)
;;     00009770 (in fn00009478)
;;     00009B22 (in fn00009AF8)
;;     00009B2A (in fn00009AF8)
;;     00009B32 (in fn00009AF8)
;;     00009B42 (in fn00009AF8)
;;     00009D6E (in fn00009D6A)
;;     00009D76 (in fn00009D6A)
;;     00009D90 (in fn00009D6A)
;;     00009DA8 (in fn00009D6A)
;;     00009DEC (in fn00009DDC)
;;     00009E02 (in fn00009DF2)
fn00009B90 proc
	mov.w	r6,@-sp
	jsr	@er0
	mov.w	r6,r0
	mov.w	@sp+,r6
	rts

;; fn00009B9A: 00009B9A
;;   Called from:
;;     00008F6A (in fn00008F4E)
;;     00009362 (in fn00008F4E)
;;     000094D6 (in fn00009478)
;;     000095C6 (in fn00009478)
;;     000097CE (in fn00009478)
;;     00009DE4 (in fn00009DDC)
;;     00009DFA (in fn00009DF2)
fn00009B9A proc
	mov.w	r6,@-sp
	mov.w	r1,r6
	jsr	@er0
	mov.w	r6,r0
	mov.w	@sp+,r6
	rts

;; fn00009BA6: 00009BA6
;;   Called from:
;;     00008AAC (in fn00008866)
;;     000094E4 (in fn00009478)
;;     0000958C (in fn00009478)
;;     00009672 (in fn00009478)
;;     00009748 (in fn00009478)
;;     000097C2 (in fn00009478)
;;     0000980C (in fn00009478)
;;     00009B1A (in fn00009AF8)
;;     00009C86 (in fn00009C6E)
;;     00009CAE (in fn00009C92)
;;     00009CD8 (in fn00009CBC)
;;     00009CE6 (in fn00009CBC)
;;     00009D44 (in fn00009D34)
;;     00009D52 (in fn00009D34)
;;     00009D84 (in fn00009D6A)
;;     00009DA0 (in fn00009D6A)
;;     00009DB6 (in fn00009D6A)
;;     00009DCE (in fn00009DC0)
fn00009BA6 proc
	mov.w	r6,@-sp
	mov.w	r2,@-sp
	mov.w	r1,r6
	jsr	@er0
	mov.w	r6,r0
	adds	#0x00000002,sp
	mov.w	@sp+,r6
	rts

;; fn00009BB6: 00009BB6
;;   Called from:
;;     00008A94 (in fn00008866)
;;     00008AFE (in fn00008AEA)
;;     00008B1A (in fn00008AEA)
;;     00008B36 (in fn00008AEA)
;;     00008B52 (in fn00008AEA)
;;     00008B6E (in fn00008AEA)
;;     00008B82 (in fn00008AEA)
;;     00008B9E (in fn00008AEA)
;;     00008BE0 (in fn00008BB0)
;;     00008C04 (in fn00008BB0)
;;     00008C30 (in fn00008BB0)
;;     00008C7E (in fn00008BB0)
;;     00008CB8 (in fn00008BB0)
;;     00008CEC (in fn00008BB0)
;;     00008D3A (in fn00008BB0)
;;     00008D74 (in fn00008BB0)
;;     00008D9C (in fn00008BB0)
;;     00008DC4 (in fn00008BB0)
;;     00008E1A (in fn00008BB0)
;;     00008E54 (in fn00008BB0)
;;     00008E8C (in fn00008BB0)
;;     00008EDA (in fn00008BB0)
;;     00008F14 (in fn00008BB0)
;;     00008F3C (in fn00008BB0)
;;     00008F80 (in fn00008F4E)
;;     00008FA4 (in fn00008F4E)
;;     00008FC0 (in fn00008F4E)
;;     00008FF0 (in fn00008F4E)
;;     0000902C (in fn00008F4E)
;;     0000905C (in fn00008F4E)
;;     00009082 (in fn00008F4E)
;;     000090A8 (in fn00008F4E)
;;     000090E6 (in fn00008F4E)
;;     00009116 (in fn00008F4E)
;;     00009148 (in fn00008F4E)
;;     00009164 (in fn00008F4E)
;;     00009180 (in fn00008F4E)
;;     000091CC (in fn00008F4E)
;;     000091E0 (in fn00008F4E)
;;     000091FC (in fn00008F4E)
;;     00009222 (in fn00008F4E)
;;     0000923E (in fn00008F4E)
;;     0000925A (in fn00008F4E)
;;     000092A6 (in fn00008F4E)
;;     000092BA (in fn00008F4E)
;;     000092D6 (in fn00008F4E)
;;     0000930A (in fn00008F4E)
;;     00009330 (in fn00008F4E)
;;     0000934C (in fn00008F4E)
;;     000093D2 (in fn00009370)
;;     000093EE (in fn00009370)
;;     00009424 (in fn00009370)
;;     00009440 (in fn00009370)
;;     00009498 (in fn00009478)
;;     000095AC (in fn00009478)
;;     000095F0 (in fn00009478)
;;     00009608 (in fn00009478)
;;     0000961C (in fn00009478)
;;     00009692 (in fn00009478)
;;     00009768 (in fn00009478)
;;     00009792 (in fn00009478)
;;     000097AE (in fn00009478)
fn00009BB6 proc
	mov.w	r6,@-sp
	mov.w	@(4:16,sp),r3
	mov.w	r3,@-sp
	mov.w	r2,@-sp
	mov.w	r1,r6
	jsr	@er0
	mov.w	r6,r0
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	mov.w	@sp+,r6
	rts
00009BCE                                           6D F6               m.
00009BD0 6F 73 00 06 6D F3 6F 73 00 06 6D F3 6D F2 0D 16 os..m.os..m.m...
00009BE0 5D 00 0D 60 8F 06 97 00 6D 76 54 70             ]..`....mvTp    

;; fn00009BEC: 00009BEC
;;   Called from:
;;     00009012 (in fn00008F4E)
;;     000091B2 (in fn00008F4E)
;;     0000928C (in fn00008F4E)
fn00009BEC proc
	add.w	r0,r0
	mov.w	@(-24802:16,er0),r0
	rts

;; fn00009BF4: 00009BF4
;;   Called from:
;;     00008FDE (in fn00008F4E)
;;     0000918C (in fn00008F4E)
;;     00009266 (in fn00008F4E)
fn00009BF4 proc
	add.w	r0,r0
	mov.w	r1,@(-24802:16,er0)
	rts

;; fn00009BFC: 00009BFC
;;   Called from:
;;     00009484 (in fn00009478)
;;     000094A8 (in fn00009478)
fn00009BFC proc
	add.w	r0,r0
	mov.w	@(-24814:16,er0),r2
	mov.w	r2,r0
	add.w	r0,r0
	add.w	r0,r0
	add.w	r2,r0
	add.w	r0,r0
	rts
00009C0E                                           09 00               ..
00009C10 19 22 6F 82 9F 12 54 70                         ."o...Tp        

;; fn00009C18: 00009C18
;;   Called from:
;;     00008B08 (in fn00008AEA)
;;     00008B24 (in fn00008AEA)
;;     00008B40 (in fn00008AEA)
;;     00008B5C (in fn00008AEA)
;;     00008B8C (in fn00008AEA)
;;     00008BA8 (in fn00008AEA)
;;     00008DCE (in fn00008BB0)
;;     00008FCA (in fn00008F4E)
;;     0000908C (in fn00008F4E)
;;     00009152 (in fn00008F4E)
;;     0000916E (in fn00008F4E)
;;     000091EA (in fn00008F4E)
;;     00009206 (in fn00008F4E)
;;     0000922C (in fn00008F4E)
;;     00009248 (in fn00008F4E)
;;     000092C4 (in fn00008F4E)
;;     000092E0 (in fn00008F4E)
;;     0000933A (in fn00008F4E)
;;     000093DC (in fn00009370)
;;     000093F8 (in fn00009370)
;;     0000942E (in fn00009370)
;;     0000944A (in fn00009370)
;;     0000979C (in fn00009478)
fn00009C18 proc
	mov.w	r4,@-sp
	mov.w	#0x9F1E,r4
	mov.w	r0,@er4
	mov.w	r0,r0
	ble	00009C30

l00009C24:
	jsr	@fn00009D34
	mov.w	r0,r0
	bne	00009C30

l00009C2C:
	mov.w	@er4,r2
	bgt	00009C24

l00009C30:
	mov.w	@sp+,r4
	rts

;; fn00009C34: 00009C34
;;   Called from:
;;     000094BE (in fn00009478)
;;     000094CA (in fn00009478)
fn00009C34 proc
	add.w	r0,r0
	add.w	r0,r0
	add.w	r0,r0
	mov.w	#0x9E94,r3
	add.w	r0,r3
	mov.w	r3,r0
	mov.b	r1l,@er0
	mov.b	r2l,@(1:16,er0)
	rts
00009C4A                               6D F4 0D 04 09 44           m....D
00009C50 09 44 09 44 C0 10 79 02 9E 94 09 42 0D 01 79 00 .D.D..y....B..y.
00009C60 14 C0 5E 00 9B A6 6F 40 9E 98 6D 74 54 70       ..^...o@..mtTp  

;; fn00009C6E: 00009C6E
;;   Called from:
;;     00008C48 (in fn00008BB0)
;;     00008D04 (in fn00008BB0)
;;     00008DE4 (in fn00008BB0)
;;     00008EA4 (in fn00008BB0)
;;     00008FFA (in fn00008F4E)
;;     000090B2 (in fn00008F4E)
;;     00009194 (in fn00008F4E)
;;     0000926E (in fn00008F4E)
;;     000093FE (in fn00009370)
fn00009C6E proc
	mov.w	r4,@-sp
	mov.w	r0,r4
	add.w	r4,r4
	add.w	r4,r4
	add.w	r4,r4
	or.b	#0x10,r0h
	mov.w	#0x9E94,r2
	add.w	r4,r2
	mov.w	r0,r1
	mov.w	#0x14C0,r0
	jsr	@fn00009BA6
	mov.w	@(-24938:16,er4),r0
	mov.w	@sp+,r4
	rts

;; fn00009C92: 00009C92
;;   Called from:
;;     00008BEA (in fn00008BB0)
;;     00008F8A (in fn00008F4E)
;;     000090CC (in fn00008F4E)
fn00009C92 proc
	mov.w	r4,@-sp
	mov.w	r0,r4
	add.w	r4,r4
	add.w	r4,r4
	add.w	r4,r4
	mov.w	#0x9E94,r3
	add.w	r4,r3
	mov.w	r3,r4
	or.b	#0x10,r0h
	mov.w	r4,r2
	mov.w	r0,r1
	mov.w	#0x14C0,r0
	jsr	@fn00009BA6
	mov.b	@(6:16,er4),r0l
	mov.b	#0x00,r0h
	mov.w	@sp+,r4
	rts

;; fn00009CBC: 00009CBC
;;   Called from:
;;     00009534 (in fn00009478)
;;     00009810 (in fn00009478)
fn00009CBC proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r7,r5
	adds	#0x00000002,er5
	adds	#0x00000002,er5

l00009CCA:
	mov.w	#0x0006,r2
	add.w	r7,r2
	mov.w	#0x4000,r1
	mov.w	#0x29F2,r0
	jsr	@fn00009BA6
	mov.w	r5,r2
	mov.w	#0x3000,r1
	mov.w	#0x1FB6,r0
	jsr	@fn00009BA6
	mov.w	@(6:16,sp),r3
	mov.w	#0x0002,r2
	cmp.w	r2,r3
	bne	00009CFC

l00009CF6:
	mov.w	@(4:16,sp),r2
	beq	00009CCA

l00009CFC:
	sub.w	r4,r4

l00009CFE:
	jsr	@fn00009D34
	mov.w	r0,r0
	beq	00009D08

l00009D06:
	sub.w	r4,r4

l00009D08:
	adds	#0x00000001,er4
	mov.w	#0x03E7,r2
	cmp.w	r2,r4
	ble	00009CFE

l00009D12:
	mov.w	@(6:16,sp),r2
	beq	00009D20

l00009D18:
	mov.b	@(1:16,er5),r0l
	mov.b	#0x00,r0h
	bra	00009D2A

l00009D20:
	mov.b	@(1:16,er5),r2l
	or.b	#0x08,r2l
	mov.b	r2l,r0l
	mov.b	#0x00,r0h

l00009D2A:
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts

;; fn00009D34: 00009D34
;;   Called from:
;;     00009C24 (in fn00009C18)
;;     00009CFE (in fn00009CBC)
fn00009D34 proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r7,r2
	adds	#0x00000002,er2
	mov.w	#0x4000,r1
	mov.w	#0x29F2,r0
	jsr	@fn00009BA6
	mov.w	r7,r2
	mov.w	#0x3000,r1
	mov.w	#0x1FB6,r0
	jsr	@fn00009BA6
	mov.w	@(2:16,sp),r2
	beq	00009D60

l00009D5C:
	mov.w	@sp,r0
	bra	00009D64

l00009D60:
	mov.w	@sp,r0
	or.b	#0x08,r0l

l00009D64:
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts

;; fn00009D6A: 00009D6A
;;   Called from:
;;     0000955E (in fn00009478)
fn00009D6A proc
	mov.w	#0x27AC,r0
	jsr	@fn00009B90
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6
	jsr	@fn00009DC0
	mov.w	#0x2A62,r0
	jsr	@fn00009B90
	mov.w	#0x9F10,r1
	mov.w	#0x9F00,r2
	mov.w	#0x3B9A,r0
	jsr	@fn00009BA6
	mov.w	#0x1498,r0
	jsr	@fn00009B90
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn00009BA6
	jsr	@fn00009DC0
	rts

;; fn00009DC0: 00009DC0
;;   Called from:
;;     00009D88 (in fn00009D6A)
;;     00009DBA (in fn00009D6A)
fn00009DC0 proc
	subs	#0x00000002,sp

l00009DC2:
	mov.w	r7,r2
	adds	#0x00000001,er2
	mov.w	#0x700C,r1
	mov.w	#0x3CCC,r0
	jsr	@fn00009BA6
	mov.b	@(1:16,sp),r2l
	bne	00009DC2

l00009DD8:
	adds	#0x00000002,sp
	rts

;; fn00009DDC: 00009DDC
;;   Called from:
;;     000097D2 (in fn00009478)
;;     00009B36 (in fn00009AF8)
fn00009DDC proc
	mov.w	#0x3006,r1
	mov.w	#0x1B62,r0
	jsr	@fn00009B9A
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	rts

;; fn00009DF2: 00009DF2
;;   Called from:
;;     000095CA (in fn00009478)
fn00009DF2 proc
	mov.w	#0x3007,r1
	mov.w	#0x1B62,r0
	jsr	@fn00009B9A
	mov.w	#0x27C8,r0
	jsr	@fn00009B90
	rts

;; fn00009E08: 00009E08
;;   Called from:
;;     00008940 (in fn00008866)
fn00009E08 proc
	mov.w	r0,r2
	mulxu.b	r1h,r2h
	mov.b	r0h,r2h
	mulxu.b	r1l,r0h
	add.b	r2l,r0h
	mulxu.b	r2h,r1h
	add.b	r1l,r0h
	rts

;; fn00009E18: 00009E18
;;   Called from:
;;     00009B76 (in fn00009B66)
fn00009E18 proc
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	r1,r6
	mov.w	r0,r5
	mov.w	r3,r4
	mov.w	r2,r3
	jsr	@@0x54:8
	mov.w	r6,r1
	mov.w	r5,r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	rts
00009E34             FF FF FF FF 03 13 03 0C 02 D0 03 3B     ...........;
00009E40 00 04 00 03 00 04 00 06 00 06 00 04 00 03 00 04 ................
00009E50 00 07 00 09 00 09 00 07 00 04 00 06 00 09 00 0C ................
00009E60 00 0C 00 09 00 06 00 04 00 07 00 09 00 09 00 07 ................
00009E70 00 04 00 03 00 04 00 06 00 06 00 04 00 03 00 64 ...............d
00009E80 00 00                                           ..              
