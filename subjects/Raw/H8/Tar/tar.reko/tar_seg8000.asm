;;; Segment seg8000 (8000)

;; fn8000: 8000
fn8000 proc
	mov.w	r0,@-sp
	mov.w	r1,@-sp
	mov.w	r2,@-sp
	mov.w	r3,@-sp
	jsr	@fn9AF8
	mov.w	@sp+,r3
	mov.w	@sp+,r2
	mov.w	@sp+,r1
	mov.w	@sp+,r0
	rts
8016                   44 6F 20 79 6F 75 20 62 79 74       Do you byt
8020 65 2C 20 77 68 65 6E 20 49 20 6B 6E 6F 63 6B 3F e, when I knock?
8030 00 00                                           ..              

;; fn8032: 8032
;;   Called from:
;;     88FE (in fn8866)
fn8032 proc
	mov.w	#0x002E,r3
	sub.w	r3,r7
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	sub.w	r5,r5
	mov.w	#0x0004,r6
	sub.w	r0,r0

l8046:
	sub.w	r1,r1
	mov.w	r1,@(6:16,sp)
	mov.w	r5,r3
	add.w	r3,r3

l8050:
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
	ble	8050

l806E:
	mov.w	#0x0005,r2
	adds	#0x00000001,er5
	cmp.w	r2,r5
	ble	8046

l8078:
	sub.w	r5,r5

l807A:
	sub.w	r4,r4
	mov.w	r4,@(6:16,sp)
	mov.w	r5,@(48:16,sp)
	mov.w	r5,r1
	add.w	r1,r1
	mov.w	r1,@(48:16,sp)

l808C:
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
	bge	8136

l8130:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l8136:
	mov.w	r6,r3
	mov.w	r6,r6
	bge	8142

l813C:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l8142:
	mov.w	r0,r2
	add.w	r3,r2
	mov.w	@(8:16,sp),r3
	bge	8152

l814C:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l8152:
	add.w	r2,r3
	mov.w	@(50:16,sp),r2
	add.w	r6,r2
	mov.w	@(8:16,sp),r4
	add.w	r4,r2
	mov.w	r2,r2
	bge	816A

l8164:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l816A:
	cmp.w	r2,r3
	bne	819E

l816E:
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

l819E:
	mov.w	@(44:16,sp),r4
	mov.w	r4,@(6:16,sp)
	mov.w	#0x0002,r2
	cmp.w	r2,r4
	bgt	81B2

l81AE:
	jmp	@0x808C:24

l81B2:
	adds	#0x00000001,er5
	mov.w	#0x0005,r2
	cmp.w	r2,r5
	bgt	81C0

l81BC:
	jmp	@0x807A:24

l81C0:
	sub.w	r5,r5

l81C2:
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

l8206:
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
	bge	8258

l8252:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l8258:
	mov.w	@(38:16,sp),r0
	bge	8264

l825E:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l8264:
	add.w	r0,r2
	mov.w	r6,r0
	mov.w	r6,r6
	bge	8272

l826C:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l8272:
	add.w	r2,r0
	mov.w	@(38:16,sp),r4
	mov.w	@(40:16,sp),r2
	add.w	r4,r2
	add.w	r6,r2
	mov.w	r2,r2
	bge	828A

l8284:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l828A:
	cmp.w	r2,r0
	bne	82B2

l828E:
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

l82B2:
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
	bgt	82E0

l82DC:
	jmp	@0x8206:24

l82E0:
	mov.w	@(18:16,sp),r5
	mov.w	#0x0003,r2
	cmp.w	r2,r5
	bgt	82F0

l82EC:
	jmp	@0x81C2:24

l82F0:
	sub.w	r5,r5

l82F2:
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

l8316:
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
	bge	83B0

l83AA:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l83B0:
	mov.w	@(30:16,sp),r3
	bge	83BC

l83B6:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l83BC:
	mov.w	r0,r2
	add.w	r3,r2
	mov.w	@(6:16,sp),r3
	bge	83CC

l83C6:
	not.b	r3l
	not.b	r3h
	adds	#0x00000001,er3

l83CC:
	add.w	r2,r3
	mov.w	@(30:16,sp),r1
	mov.w	@(36:16,sp),r2
	add.w	r1,r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	mov.w	r2,r2
	bge	83E8

l83E2:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l83E8:
	cmp.w	r2,r3
	bne	8414

l83EC:
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

l8414:
	mov.w	@(28:16,sp),r4
	mov.w	r4,@(6:16,sp)
	mov.w	#0x0002,r2
	cmp.w	r2,r4
	bgt	8428

l8424:
	jmp	@0x8316:24

l8428:
	mov.w	@(18:16,sp),r5
	mov.w	#0x0003,r2
	cmp.w	r2,r5
	bgt	8438

l8434:
	jmp	@0x82F2:24

l8438:
	sub.w	r5,r5

l843A:
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

l845E:
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
	bge	84F4

l84EE:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l84F4:
	mov.w	r3,r0
	mov.w	r3,r3
	bge	8500

l84FA:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l8500:
	add.w	r0,r2
	mov.w	@(6:16,sp),r0
	bge	850E

l8508:
	not.b	r0l
	not.b	r0h
	adds	#0x00000001,er0

l850E:
	add.w	r2,r0
	mov.w	@(26:16,sp),r2
	add.w	r3,r2
	mov.w	@(6:16,sp),r4
	add.w	r4,r2
	mov.w	r2,r2
	bge	8526

l8520:
	not.b	r2l
	not.b	r2h
	adds	#0x00000001,er2

l8526:
	cmp.w	r2,r0
	bne	8552

l852A:
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

l8552:
	mov.w	@(20:16,sp),r4
	mov.w	r4,@(6:16,sp)
	mov.w	#0x0002,r2
	cmp.w	r2,r4
	bgt	8566

l8562:
	jmp	@0x845E:24

l8566:
	mov.w	@(18:16,sp),r5
	mov.w	#0x0003,r2
	cmp.w	r2,r5
	bgt	8576

l8572:
	jmp	@0x843A:24

l8576:
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x002E,r3
	add.w	r3,r7
	rts

;; fn8584: 8584
;;   Called from:
;;     88DC (in fn8866)
;;     9562 (in fn9478)
;;     9574 (in fn9478)
;;     9590 (in fn9478)
;;     9598 (in fn9478)
;;     9648 (in fn9478)
;;     965A (in fn9478)
;;     9676 (in fn9478)
;;     967E (in fn9478)
;;     971E (in fn9478)
;;     9730 (in fn9478)
;;     974C (in fn9478)
;;     9754 (in fn9478)
fn8584 proc
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

l85A0:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r4
	mov.w	r4,@(8:16,sp)
	mov.w	r4,r5
	add.w	r5,r5
	mov.w	r5,@(8:16,sp)

l85B2:
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
	bne	861C

l85EC:
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
	bne	861C

l860E:
	mov.w	r0,r0
	beq	861C

l8612:
	mov.w	@(12:16,sp),r4
	add.w	r0,r4
	mov.w	r4,@(12:16,sp)

l861C:
	mov.w	@(6:16,sp),r1
	mov.w	#0x0002,r2
	cmp.w	r2,r1
	ble	85B2

l8628:
	mov.w	@(14:16,sp),r5
	adds	#0x00000001,er5
	mov.w	r5,@(14:16,sp)
	mov.w	#0x0005,r2
	cmp.w	r2,r5
	bgt	863E

l863A:
	jmp	@0x85A0:24

l863E:
	sub.w	r4,r4
	mov.w	r4,@(14:16,sp)

l8644:
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

l866C:
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
	bne	86B0

l868A:
	mov.w	@(14:16,sp),r2
	adds	#0x00000002,er2
	add.w	r2,r2
	add.w	r4,r2
	mov.w	#0x9EB0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	cmp.w	r2,r3
	bne	86B0

l86A2:
	mov.w	r3,r3
	beq	86B0

l86A6:
	mov.w	@(12:16,sp),r5
	add.w	r3,r5
	mov.w	r5,@(12:16,sp)

l86B0:
	mov.w	@(6:16,sp),r4
	add.b	#0x0C,r4l
	addx.b	#0x00,r4h
	mov.w	r4,@(6:16,sp)
	adds	#0x00000001,er1
	mov.w	#0x0004,r2
	cmp.w	r2,r1
	ble	866C

l86C6:
	mov.w	r6,@(14:16,sp)
	mov.w	#0x0003,r2
	mov.w	r6,r5
	cmp.w	r2,r5
	bgt	86D8

l86D4:
	jmp	@0x8644:24

l86D8:
	sub.w	r4,r4
	mov.w	r4,@(14:16,sp)

l86DE:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r6
	adds	#0x00000001,er6
	mov.w	@(14:16,sp),r5
	mov.w	r5,@(10:16,sp)
	mov.w	r5,r4
	add.w	r4,r4
	mov.w	r4,@(10:16,sp)

l86F6:
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
	bne	8778

l8740:
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
	bne	8778

l876A:
	mov.w	r5,r5
	beq	8778

l876E:
	mov.w	@(12:16,sp),r4
	add.w	r5,r4
	mov.w	r4,@(12:16,sp)

l8778:
	mov.w	@(6:16,sp),r1
	mov.w	#0x0002,r2
	cmp.w	r2,r1
	bgt	8788

l8784:
	jmp	@0x86F6:24

l8788:
	mov.w	r6,@(14:16,sp)
	mov.w	#0x0003,r2
	mov.w	r6,r5
	cmp.w	r2,r5
	bgt	879A

l8796:
	jmp	@0x86DE:24

l879A:
	sub.w	r4,r4
	mov.w	r4,@(14:16,sp)

l87A0:
	sub.w	r1,r1
	mov.w	@(14:16,sp),r6
	adds	#0x00000001,er6
	mov.w	@(14:16,sp),r5
	mov.w	r5,@(8:16,sp)
	mov.w	r5,r4
	add.w	r4,r4
	mov.w	r4,@(8:16,sp)

l87B8:
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
	bne	8832

l8802:
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
	bne	8832

l8824:
	mov.w	r0,r0
	beq	8832

l8828:
	mov.w	@(12:16,sp),r5
	add.w	r0,r5
	mov.w	r5,@(12:16,sp)

l8832:
	mov.w	@(6:16,sp),r1
	mov.w	#0x0002,r2
	cmp.w	r2,r1
	bgt	8842

l883E:
	jmp	@0x87B8:24

l8842:
	mov.w	r6,@(14:16,sp)
	mov.w	#0x0003,r2
	mov.w	r6,r4
	cmp.w	r2,r4
	bgt	8854

l8850:
	jmp	@0x87A0:24

l8854:
	mov.w	@(12:16,sp),r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x000A,r3
	add.w	r3,r7
	rts

;; fn8866: 8866
;;   Called from:
;;     89AC (in fn8866)
;;     96D6 (in fn9478)
fn8866 proc
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

l889C:
	mov.w	@(18:16,sp),r2
	add.w	r2,r2
	mov.w	#0x9EF0,r4
	add.w	r2,r4
	mov.w	r4,r2
	mov.w	@er2,r2
	bgt	88B2

l88AE:
	jmp	@0x8A7A:24

l88B2:
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
	beq	88DC

l88D8:
	jmp	@0x8974:24

l88DC:
	jsr	@fn8584
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
	jsr	@fn8032
	sub.w	r4,r4
	mov.w	r4,@(16:16,sp)

l8908:
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

l8928:
	mov.w	@(8:16,sp),r5
	mov.w	@er5,r0
	add.b	#0x0C,r5l
	addx.b	#0x00,r5h
	mov.w	r5,@(8:16,sp)
	mov.w	@er6,r1
	add.b	#0x0C,r6l
	addx.b	#0x00,r6h
	mov.w	r3,@(10:16,sp)
	jsr	@fn9E08
	mov.w	@(6:16,sp),r4
	add.w	r0,r4
	mov.w	r4,@(6:16,sp)
	mov.w	@(10:16,sp),r3
	adds	#0x00000001,er3
	mov.w	#0x0004,r2
	cmp.w	r2,r3
	ble	8928

l895C:
	mov.w	@(16:16,sp),r5
	adds	#0x00000001,er5
	mov.w	r5,@(16:16,sp)
	mov.w	#0x0005,r2
	cmp.w	r2,r5
	ble	8908

l896E:
	mov.w	r4,@(28:16,sp)
	bra	89B4

l8974:
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
	jsr	@fn8866
	adds	#0x00000002,sp
	adds	#0x00000002,sp

l89B4:
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
	bne	89E8

l89DA:
	mov.w	@(28:16,sp),r3
	mov.w	@(22:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r2,r3
	blt	8A22

l89E8:
	mov.w	#0x0001,r2
	mov.w	@(20:16,sp),r4
	cmp.w	r2,r4
	bne	8A02

l89F4:
	mov.w	@(28:16,sp),r3
	mov.w	@(22:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r2,r3
	bgt	8A22

l8A02:
	jsr	@fn9B66
	btst	#0x00,r0l
	beq	8A18

l8A0A:
	mov.w	@(28:16,sp),r3
	mov.w	@(22:16,sp),r4
	mov.w	@er4,r2
	cmp.w	r2,r3
	beq	8A22

l8A18:
	mov.w	@(24:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r6,r2
	bne	8A36

l8A22:
	mov.w	@(18:16,sp),r5
	mov.w	@(24:16,sp),r4
	mov.w	r5,@er4
	mov.w	@(28:16,sp),r2
	mov.w	@(22:16,sp),r4
	mov.w	r2,@er4

l8A36:
	mov.w	#0xFFFF,r2
	mov.w	@(20:16,sp),r5
	cmp.w	r2,r5
	bne	8A58

l8A42:
	mov.w	@(34:16,sp),r4
	cmp.w	r5,r4
	beq	8A58

l8A4A:
	mov.w	@(22:16,sp),r5
	mov.w	@er5,r2
	cmp.w	r2,r4
	ble	8A58

l8A54:
	jmp	@0x8ADC:24

l8A58:
	mov.w	#0x0001,r2
	mov.w	@(20:16,sp),r4
	cmp.w	r2,r4
	bne	8A7A

l8A64:
	mov.w	#0xFFFF,r2
	mov.w	@(34:16,sp),r5
	cmp.w	r2,r5
	beq	8A7A

l8A70:
	mov.w	@(22:16,sp),r4
	mov.w	@er4,r2
	cmp.w	r2,r5
	blt	8ADC

l8A7A:
	mov.w	@(32:16,sp),r5
	bne	8AB0

l8A80:
	mov.w	#0x3002,r2
	mov.w	r2,@-sp
	mov.w	@(24:16,sp),r4
	mov.w	@er4,r2
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6

l8AB0:
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
	bgt	8ADC

l8AD8:
	jmp	@0x889C:24

l8ADC:
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x0018,r3
	add.w	r3,r7
	rts

;; fn8AEA: 8AEA
;;   Called from:
;;     96EC (in fn9478)
fn8AEA proc
	mov.w	r4,@-sp
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x001E,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2001,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x00C8,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2001,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x00D2,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2001,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x001E,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
	mov.w	@sp+,r4
	rts

;; fn8BB0: 8BB0
;;   Called from:
;;     9070 (in fn8F4E)
;;     9136 (in fn8F4E)
;;     9210 (in fn8F4E)
;;     931E (in fn8F4E)
;;     93BC (in fn9370)
;;     96E8 (in fn9478)
;;     977C (in fn9478)
fn8BB0 proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	r0,@(8:16,sp)
	mov.w	r1,@(6:16,sp)
	mov.w	@0x9E36:16,r2
	cmp.w	r1,r2
	bne	8BCE

l8BCA:
	jmp	@0x8DA2:24

l8BCE:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8BE6:
	mov.w	#0x0001,r0
	jsr	@fn9C92
	mov.b	r0l,r0l
	beq	8BE6

l8BF2:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E34:16
	mov.w	@0x9E36:16,r2
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	blt	8C22

l8C1E:
	jmp	@0x8CD2:24

l8C22:
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	cmp.w	r5,r2
	blt	8C42

l8C3E:
	jmp	@0x8D8A:24

l8C42:
	mov.w	#0x000A,r6

l8C46:
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	r0,r4
	mov.w	@0x9E3A:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	8C92

l8C60:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	8C92

l8C68:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	adds	#0x00000001,er2
	mov.w	r2,@0x9E36:16
	mov.w	@0x9E7E:16,r3

l8C92:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	8CBE

l8C9A:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	8CBE

l8CA2:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8CBE:
	mov.w	@0x9E36:16,r2
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	bge	8CCE

l8CCA:
	jmp	@0x8C46:24

l8CCE:
	jmp	@0x8D8A:24

l8CD2:
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	bgt	8CDE

l8CDA:
	jmp	@0x8DA2:24

l8CDE:
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	cmp.w	r5,r2
	bgt	8CFE

l8CFA:
	jmp	@0x8D8A:24

l8CFE:
	mov.w	#0x000A,r6

l8D02:
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	r0,r4
	mov.w	@0x9E3A:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	8D4E

l8D1C:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	8D4E

l8D24:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E36:16,r2
	subs	#0x00000001,er2
	mov.w	r2,@0x9E36:16
	mov.w	@0x9E7E:16,r3

l8D4E:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	8D7A

l8D56:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	8D7A

l8D5E:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8D7A:
	mov.w	@0x9E36:16,r2
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r2
	ble	8D8A

l8D86:
	jmp	@0x8D02:24

l8D8A:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8DA2:
	mov.w	@0x9E34:16,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	blt	8DB2

l8DAE:
	jmp	@0x8E6E:24

l8DB2:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0014,r0
	jsr	@fn9C18
	mov.w	@0x9E34:16,r2
	cmp.w	r5,r2
	blt	8DDE

l8DDA:
	jmp	@0x8F2A:24

l8DDE:
	mov.w	#0x000A,r6

l8DE2:
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	r0,r4
	mov.w	@0x9E38:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	8E2E

l8DFC:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	8E2E

l8E04:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E34:16,r2
	adds	#0x00000001,er2
	mov.w	r2,@0x9E34:16
	mov.w	@0x9E7E:16,r3

l8E2E:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	8E5A

l8E36:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	8E5A

l8E3E:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8E5A:
	mov.w	@0x9E34:16,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	bge	8E6A

l8E66:
	jmp	@0x8DE2:24

l8E6A:
	jmp	@0x8F2A:24

l8E6E:
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	bgt	8E7A

l8E76:
	jmp	@0x8F2A:24

l8E7A:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E34:16,r2
	cmp.w	r5,r2
	bgt	8E9E

l8E9A:
	jmp	@0x8F2A:24

l8E9E:
	mov.w	#0x000A,r6

l8EA2:
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	r0,r4
	mov.w	@0x9E38:16,r2
	sub.w	r2,r4
	mov.w	@0x9E7E:16,r3
	mov.w	#0x0064,r2
	cmp.w	r2,r3
	bne	8EEE

l8EBC:
	mov.w	#0x0008,r2
	cmp.w	r2,r4
	ble	8EEE

l8EC4:
	mov.w	#0x00C8,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x07D0,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@0x9E34:16,r2
	subs	#0x00000001,er2
	mov.w	r2,@0x9E34:16
	mov.w	@0x9E7E:16,r3

l8EEE:
	mov.w	#0x00C8,r2
	cmp.w	r2,r3
	bne	8F1A

l8EF6:
	mov.w	#0xFFF8,r2
	cmp.w	r2,r4
	bge	8F1A

l8EFE:
	mov.w	#0x0064,r2
	mov.w	r2,@0x9E7E:16
	mov.w	r6,@-sp
	mov.w	#0x01B8,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8F1A:
	mov.w	@0x9E34:16,r2
	mov.w	@(8:16,sp),r5
	cmp.w	r5,r2
	ble	8F2A

l8F26:
	jmp	@0x8EA2:24

l8F2A:
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts

;; fn8F4E: 8F4E
;;   Called from:
;;     954E (in fn9478)
fn8F4E proc
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E34:16
	mov.w	r2,@0x9E36:16
	mov.w	#0x1000,r1
	mov.w	#0x1946,r0
	jsr	@fn9B9A
	mov.w	#0x0007,r2
	mov.w	r2,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp

l8F86:
	mov.w	#0x0001,r0
	jsr	@fn9C92
	mov.b	r0l,r0l
	beq	8F86

l8F92:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E34:16
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x01C2,r0
	jsr	@fn9C18
	sub.w	r6,r6
	mov.w	#0x07D0,r5
	mov.w	r5,@(6:16,sp)
	mov.w	#0x00C8,r1
	sub.w	r0,r0
	jsr	@fn9BF4
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	bra	9010

l8FF8:
	sub.w	r0,r0
	jsr	@fn9C6E
	cmp.w	r6,r0
	ble	9004

l9002:
	mov.w	r0,r6

l9004:
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r0
	bge	9010

l900C:
	mov.w	r0,@(6:16,sp)

l9010:
	sub.w	r0,r0
	jsr	@fn9BEC
	mov.w	r0,r0
	bgt	8FF8

l901A:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
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
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp
	sub.w	r1,r1
	mov.w	#0xFFFF,r0
	jsr	@fn8BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0096,r0
	jsr	@fn9C18
	sub.w	r6,r6
	mov.w	#0x07D0,r5
	mov.w	r5,@(6:16,sp)
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	bra	90C8

l90B0:
	sub.w	r0,r0
	jsr	@fn9C6E
	cmp.w	r6,r0
	ble	90BC

l90BA:
	mov.w	r0,r6

l90BC:
	mov.w	@(6:16,sp),r5
	cmp.w	r5,r0
	bge	90C8

l90C4:
	mov.w	r0,@(6:16,sp)

l90C8:
	mov.w	#0x0001,r0
	jsr	@fn9C92
	mov.b	r0l,r0l
	beq	90B0

l90D4:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
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
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp
	sub.w	r2,r2
	mov.w	r2,@0x9E3E:16
	mov.w	#0x07D0,r2
	mov.w	r2,@0x9E3C:16
	sub.w	r1,r1
	sub.w	r0,r0
	jsr	@fn8BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0064,r1
	sub.w	r0,r0
	jsr	@fn9BF4
	bra	91B0

l9192:
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	@0x9E3E:16,r2
	cmp.w	r2,r0
	ble	91A4

l91A0:
	mov.w	r0,@0x9E3E:16

l91A4:
	mov.w	@0x9E3C:16,r2
	cmp.w	r2,r0
	bge	91B0

l91AC:
	mov.w	r0,@0x9E3C:16

l91B0:
	sub.w	r0,r0
	jsr	@fn9BEC
	mov.w	r0,r0
	bgt	9192

l91BA:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
	mov.w	#0x0005,r1
	sub.w	r0,r0
	jsr	@fn8BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0064,r1
	sub.w	r0,r0
	jsr	@fn9BF4
	bra	928A

l926C:
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	@0x9E3E:16,r2
	cmp.w	r2,r0
	ble	927E

l927A:
	mov.w	r0,@0x9E3E:16

l927E:
	mov.w	@0x9E3C:16,r2
	cmp.w	r2,r0
	bge	928A

l9286:
	mov.w	r0,@0x9E3C:16

l928A:
	sub.w	r0,r0
	jsr	@fn9BEC
	mov.w	r0,r0
	bgt	926C

l9294:
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2000,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
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
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp
	sub.w	r1,r1
	mov.w	#0xFFFF,r0
	jsr	@fn8BB0
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x012C,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0xFFFF,r2
	mov.w	r2,@0x9E36:16
	mov.w	#0x1000,r1
	mov.w	#0x1946,r0
	jsr	@fn9B9A
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	rts

;; fn9370: 9370
;;   Called from:
;;     95CE (in fn9478)
fn9370 proc
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

l9396:
	mov.w	@(6:16,sp),r3
	mov.w	@er3+,r1
	mov.w	r3,@(6:16,sp)
	subs	#0x00000001,er1
	mov.w	r1,r1
	bge	93AA

l93A6:
	jmp	@0x944E:24

l93AA:
	mov.w	r1,r2
	add.w	r2,r2
	add.w	r6,r2
	mov.w	@(-24912:16,er2),r2
	beq	93BA

l93B6:
	jmp	@0x944E:24

l93BA:
	mov.w	r4,r0
	jsr	@fn8BB0
	mov.w	#0x0007,r5
	mov.w	r5,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn9C18
	mov.w	r5,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18
	sub.w	r0,r0
	jsr	@fn9C6E
	mov.w	@0x9E3E:16,r2
	cmp.w	r2,r0
	ble	9416

l940A:
	mov.w	r4,@(10:16,sp)
	mov.w	#0x0001,r3
	mov.w	r3,@(8:16,sp)

l9416:
	mov.w	r5,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x002A,r0
	jsr	@fn9C18
	mov.w	r5,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x0055,r0
	jsr	@fn9C18

l944E:
	add.b	#0x0C,r6l
	addx.b	#0x00,r6h
	adds	#0x00000001,er4
	mov.w	#0x0004,r2
	cmp.w	r2,r4
	bgt	9466

l945C:
	mov.w	@(8:16,sp),r3
	bne	9466

l9462:
	jmp	@0x9396:24

l9466:
	mov.w	@(10:16,sp),r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	mov.w	#0x0006,r3
	add.w	r3,r7
	rts

;; fn9478: 9478
;;   Called from:
;;     9B3A (in fn9AF8)
fn9478 proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r6,@-sp
	sub.w	r0,r0
	jsr	@fn9BFC
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp
	sub.w	r0,r0
	jsr	@fn9BFC
	mov.w	r0,r1
	bld	#0x07,r1h
	subx.b	r0l,r0l
	subx.b	r0h,r0h
	jsr	@fn9B54
	sub.b	r2l,r2l
	mov.b	#0x03,r1l
	sub.w	r0,r0
	jsr	@fn9C34
	mov.b	#0x20,r2l
	mov.b	#0x01,r1l
	mov.w	#0x0001,r0
	jsr	@fn9C34
	mov.w	#0x1001,r1
	mov.w	#0x19C4,r0
	jsr	@fn9B9A

l94DA:
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6
	sub.w	r0,r0
	sub.w	r1,r1
	mov.w	#0x0005,r4

l94F0:
	sub.w	r3,r3
	mov.w	r0,r2
	add.w	r2,r2
	mov.w	#0x9EB0,r5
	add.w	r2,r5
	mov.w	r5,r2

l94FE:
	mov.w	r1,@er2
	add.b	#0x0C,r2l
	addx.b	#0x00,r2h
	adds	#0x00000001,er3
	mov.w	#0x0004,r5
	cmp.w	r5,r3
	ble	94FE

l950E:
	adds	#0x00000001,er0
	cmp.w	r4,r0
	ble	94F0

l9514:
	sub.w	r3,r3
	mov.w	#0x0004,r1
	mov.w	#0x0006,r0
	mov.w	#0x9EF0,r2

l9522:
	mov.w	r0,@er2
	adds	#0x00000002,er2
	adds	#0x00000001,er3
	cmp.w	r1,r3
	ble	9522

l952C:
	mov.w	#0xFFFF,r6
	jmp	@0x97D6:24

l9534:
	jsr	@fn9CBC
	mov.b	#0x00,r0h
	mov.w	#0x0004,r5
	cmp.w	r5,r0
	bne	9546

l9542:
	jmp	@0x9800:24

l9546:
	mov.w	#0x0002,r2
	cmp.w	r2,r0
	bne	9556

l954E:
	jsr	@fn8F4E
	jmp	@0x97D6:24

l9556:
	cmp.w	r4,r0
	beq	955E

l955A:
	jmp	@0x95BE:24

l955E:
	jsr	@fn9D6A
	jsr	@fn8584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	ble	9574

l956E:
	mov.w	#0x0004,r2
	bra	9584

l9574:
	jsr	@fn8584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	bge	9590

l9580:
	mov.w	#0x0005,r2

l9584:
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6

l9590:
	jsr	@fn8584
	mov.w	r0,@0x9E80:16
	jsr	@fn8584
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp
	jmp	@0x97D6:24

l95BE:
	mov.w	#0x1000,r1
	mov.w	#0x1946,r0
	jsr	@fn9B9A
	jsr	@fn9DF2
	jsr	@fn9370
	mov.w	r0,@(6:16,sp)
	mov.w	r0,r0
	bge	95DE

l95DA:
	jmp	@0x96A0:24

l95DE:
	mov.w	#0x0014,r2
	mov.w	r2,@-sp
	mov.w	#0x09C4,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x000A,r4
	mov.w	r4,@-sp
	mov.w	#0x05DC,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	#0x0DAC,r2
	mov.w	#0x1773,r1
	mov.w	#0x327C,r0
	jsr	@fn9BB6
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
	jsr	@fn8584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	ble	965A

l9654:
	mov.w	#0x0004,r2
	bra	966A

l965A:
	jsr	@fn8584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	bge	9676

l9666:
	mov.w	#0x0005,r2

l966A:
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6

l9676:
	jsr	@fn8584
	mov.w	r0,@0x9E80:16
	jsr	@fn8584
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp

l96A0:
	mov.w	#0x0001,r0
	sub.w	r3,r3
	mov.w	#0x0004,r4
	mov.w	#0x9EF0,r1

l96AE:
	mov.w	@er1+,r2
	ble	96B4

l96B2:
	sub.w	r0,r0

l96B4:
	adds	#0x00000001,er3
	cmp.w	r4,r3
	ble	96AE

l96BA:
	mov.w	r0,r0
	beq	96C2

l96BE:
	jmp	@0x9776:24

l96C2:
	mov.w	r6,@-sp
	mov.w	r0,@-sp
	mov.w	#0x0001,r2
	mov.w	#0x000C,r1
	add.w	r7,r1
	mov.w	#0x000A,r0
	add.w	r7,r0
	jsr	@fn8866
	mov.w	@(10:16,sp),r0
	mov.w	r0,r2
	add.w	r2,r2
	mov.w	@(-24848:16,er2),r1
	subs	#0x00000001,er1
	jsr	@fn8BB0
	jsr	@fn8AEA
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
	jsr	@fn8584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	ble	9730

l972A:
	mov.w	#0x0004,r2
	bra	9740

l9730:
	jsr	@fn8584
	mov.w	@0x9E80:16,r2
	cmp.w	r0,r2
	bge	974C

l973C:
	mov.w	#0x0005,r2

l9740:
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6

l974C:
	jsr	@fn8584
	mov.w	r0,@0x9E80:16
	jsr	@fn8584
	mov.w	r0,r2
	mov.w	#0x3002,r3
	mov.w	r3,@-sp
	mov.w	#0x3001,r1
	mov.w	#0x1FF2,r0
	jsr	@fn9BB6
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	adds	#0x00000002,sp

l9776:
	sub.w	r1,r1
	mov.w	#0xFFFF,r0
	jsr	@fn8BB0
	mov.w	#0x0007,r4
	mov.w	r4,@-sp
	mov.w	#0x0002,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	#0x012C,r0
	jsr	@fn9C18
	mov.w	r4,@-sp
	mov.w	#0x0003,r2
	mov.w	#0x2002,r1
	mov.w	#0x1A4E,r0
	jsr	@fn9BB6
	adds	#0x00000002,sp
	mov.w	r6,@0x9E36:16
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6
	mov.w	#0x1000,r1
	mov.w	#0x19C4,r0
	jsr	@fn9B9A
	jsr	@fn9DDC

l97D6:
	sub.w	r5,r5
	bne	9800

l97DA:
	mov.w	#0x0001,r0
	sub.w	r3,r3
	mov.w	#0x0004,r4
	mov.w	#0x9EF0,r1

l97E8:
	mov.w	@er1+,r2
	ble	97EE

l97EC:
	sub.w	r0,r0

l97EE:
	adds	#0x00000001,er3
	cmp.w	r4,r3
	ble	97E8

l97F4:
	mov.w	#0x0008,r4
	mov.w	r0,r0
	bne	9800

l97FC:
	jmp	@0x9534:24

l9800:
	mov.w	#0x0002,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6
	jsr	@fn9CBC
	cmp.b	#0x08,r0l
	beq	981C

l9818:
	jmp	@0x94DA:24

l981C:
	sub.w	r0,r0
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts
982A                               79 03 00 06 19 37           y....7
9830 6D F4 6D F5 6D F6 5E 00 85 84 0D 06 09 66 09 06 m.m.m.^......f..
9840 09 66 09 66 09 66 09 06 09 66 09 66 5E 00 80 32 .f.f.f...f.f^..2
9850 19 33 6F F3 00 0A 19 33 6F F3 00 06 6F 72 00 0A .3o....3o...or..
9860 09 22 0D 25 79 03 9E 42 09 53 0D 35 0D 24 79 03 .".%y..B.S.5.$y.
9870 9E B0 09 43 0D 34 69 40 8C 0C 94 00 69 51 8D 0C ...C.4i@....iQ..
9880 95 00 5E 00 9E 08 09 06 6F 73 00 06 0B 03 6F F3 ..^.....os....o.
9890 00 06 79 02 00 04 1D 23 4F DC 6F 73 00 0A 0B 03 ..y....#O.os....
98A0 6F F3 00 0A 79 02 00 05 1D 23 4F AA 0D 60 6D 76 o...y....#O..`mv
98B0 6D 75 6D 74 79 03 00 06 09 37 54 70 6D F4 6D F5 mumty....7Tpm.m.
98C0 0D 03 09 33 0D 34 79 05 9E F0 09 45 0D 54 69 42 ...3.4y....E.TiB
98D0 1B 02 69 C2 09 22 09 03 09 33 09 33 09 32 6F A1 ..i.."...3.3.2o.
98E0 9E B0 6D 75 6D 74 54 70 6D F4 6D F5 0D 02 09 22 ..mumtTpm.m...."
98F0 0D 24 79 05 9E F0 09 45 0D 54 69 41 0D 13 09 33 .$y....E.TiA...3
9900 09 02 09 22 09 22 09 23 19 22 6F B2 9E B0 0B 01 ...".".#."o.....
9910 69 C1 6D 75 6D 74 54 70 6D F4 6D F5 6D F6 19 11 i.mumtTpm.m.m...
9920 79 06 00 04 79 05 9E B0 19 44 19 00 0D 13 09 33 y...y....D.....3
9930 0D 32 09 52 69 A4 8B 0C 93 00 0B 00 1D 60 4F F0 .2.Ri........`O.
9940 79 02 00 05 79 03 00 04 0B 01 1D 21 4F DC 19 00 y...y......!O...
9950 0D 31 79 03 00 06 79 02 9E F0 69 A3 0B 82 0B 00 .1y...y...i.....
9960 1D 10 4F F6 6D 76 6D 75 6D 74 54 70 6D F4 79 00 ..O.mvmumtTpm.y.
9970 00 01 19 33 79 04 00 04 79 01 9E F0 6D 12 4F 02 ...3y...y...m.O.
9980 19 00 0B 03 1D 43 4F F4 6D 74 54 70 6D F4 0D 02 .....CO.mtTpm...
9990 79 04 00 07 6D F4 79 01 20 02 79 00 1A 4E 5E 00 y...m.y. .y..N^.
99A0 9B B6 0B 87 79 00 00 2A 5E 00 9C 18 6D F4 79 02 ....y..*^...m.y.
99B0 00 03 79 01 20 02 79 00 1A 4E 5E 00 9B B6 0B 87 ..y. .y..N^.....
99C0 79 00 00 55 5E 00 9C 18 6D 74 54 70 6D F4 0D 02 y..U^...mtTpm...
99D0 79 04 00 07 6D F4 79 01 20 02 79 00 1A 4E 5E 00 y...m.y. .y..N^.
99E0 9B B6 0B 87 79 00 00 1E 5E 00 9C 18 6D F4 79 02 ....y...^...m.y.
99F0 00 03 79 01 20 02 79 00 1A 4E 5E 00 9B B6 0B 87 ..y. .y..N^.....
9A00 79 00 00 55 5E 00 9C 18 6D 74 54 70 79 02 00 07 y..U^...mtTpy...
9A10 6D F2 79 02 00 01 79 01 20 00 79 00 1A 4E 5E 00 m.y...y. .y..N^.
9A20 9B B6 0B 87 79 00 00 01 5E 00 9C 92 0C 88 47 F4 ....y...^.....G.
9A30 79 02 00 07 6D F2 79 02 00 03 79 01 20 00 79 00 y...m.y...y. .y.
9A40 1A 4E 5E 00 9B B6 0B 87 79 02 FF FF 6B 82 9E 34 .N^.....y...k..4
9A50 54 70 6D F4 19 11 79 00 FF FF 5E 00 8B B0 79 04 Tpm...y...^...y.
9A60 00 07 6D F4 79 02 00 02 79 01 20 02 79 00 1A 4E ..m.y...y. .y..N
9A70 5E 00 9B B6 0B 87 79 00 01 2C 5E 00 9C 18 6D F4 ^.....y..,^...m.
9A80 79 02 00 03 79 01 20 02 79 00 1A 4E 5E 00 9B B6 y...y. .y..N^...
9A90 0B 87 79 02 FF FF 6B 82 9E 36 6D 74 54 70 5E 00 ..y...k..6mtTp^.
9AA0 85 84 6B 02 9E 80 1D 02 4F 06 79 02 00 04 40 10 ..k.....O.y...@.
9AB0 5E 00 85 84 6B 02 9E 80 1D 02 4C 10 79 02 00 05 ^...k.....L.y...
9AC0 79 01 40 04 79 00 29 9A 5E 00 9B A6 5E 00 85 84 y.@.y.).^...^...
9AD0 6B 80 9E 80 5E 00 85 84 0D 02 79 03 30 02 6D F3 k...^.....y.0.m.
9AE0 79 01 30 01 79 00 1F F2 5E 00 9B B6 79 00 27 C8 y.0.y...^...y.'.
9AF0 5E 00 9B 90 0B 87 54 70                         ^.....Tp        

;; fn9AF8: 9AF8
;;   Called from:
;;     8008 (in fn8000)
fn9AF8 proc
	mov.w	#0x9E90,r2
	mov.w	#0x9F38,r3
	cmp.w	r3,r2
	beq	9B0E

l9B04:
	sub.b	r0l,r0l

l9B06:
	mov.b	r0l,@er2
	adds	#0x00000001,er2
	cmp.w	r3,r2
	bne	9B06

l9B0E:
	mov.w	#0x9F10,r1
	mov.w	#0x9F00,r2
	mov.w	#0x3B9A,r0
	jsr	@fn9BA6
	mov.w	#0x2964,r0
	jsr	@fn9B90
	mov.w	#0x1498,r0
	jsr	@fn9B90
	mov.w	#0x1ABA,r0
	jsr	@fn9B90
	jsr	@fn9DDC
	jsr	@fn9478
	mov.w	#0x3ED4,r0
	jsr	@fn9B90
	mov.b	#0x01,r2l
	mov.b	r2l,@0xFFCC:16
	mov.w	@0x0:16,r2
	jsr	@er2
	rts

;; fn9B54: 9B54
;;   Called from:
;;     94B4 (in fn9478)
fn9B54 proc
	xor.b	#0x7B,r1l
	xor.b	#0xDB,r1h
	xor.b	#0x68,r0l
	xor.b	#0x16,r0h
	mov.w	r0,@0x9E90:16
	mov.w	r1,@0x9E92:16
	rts

;; fn9B66: 9B66
;;   Called from:
;;     8A02 (in fn8866)
fn9B66 proc
	mov.w	@0x9E90:16,r0
	mov.w	@0x9E92:16,r1
	mov.w	#0x0001,r2
	mov.w	#0x0DCD,r3
	jsr	@fn9E18
	add.b	#0x01,r1l
	addx.b	#0x00,r1h
	addx.b	#0x00,r0l
	addx.b	#0x00,r0h
	mov.w	r0,@0x9E90:16
	mov.w	r1,@0x9E92:16
	mov.w	@0x9E92:16,r0
	rts

;; fn9B90: 9B90
;;   Called from:
;;     8A9C (in fn8866)
;;     9064 (in fn8F4E)
;;     911E (in fn8F4E)
;;     9312 (in fn8F4E)
;;     94A0 (in fn9478)
;;     95B4 (in fn9478)
;;     969A (in fn9478)
;;     9770 (in fn9478)
;;     9B22 (in fn9AF8)
;;     9B2A (in fn9AF8)
;;     9B32 (in fn9AF8)
;;     9B42 (in fn9AF8)
;;     9D6E (in fn9D6A)
;;     9D76 (in fn9D6A)
;;     9D90 (in fn9D6A)
;;     9DA8 (in fn9D6A)
;;     9DEC (in fn9DDC)
;;     9E02 (in fn9DF2)
fn9B90 proc
	mov.w	r6,@-sp
	jsr	@er0
	mov.w	r6,r0
	mov.w	@sp+,r6
	rts

;; fn9B9A: 9B9A
;;   Called from:
;;     8F6A (in fn8F4E)
;;     9362 (in fn8F4E)
;;     94D6 (in fn9478)
;;     95C6 (in fn9478)
;;     97CE (in fn9478)
;;     9DE4 (in fn9DDC)
;;     9DFA (in fn9DF2)
fn9B9A proc
	mov.w	r6,@-sp
	mov.w	r1,r6
	jsr	@er0
	mov.w	r6,r0
	mov.w	@sp+,r6
	rts

;; fn9BA6: 9BA6
;;   Called from:
;;     8AAC (in fn8866)
;;     94E4 (in fn9478)
;;     958C (in fn9478)
;;     9672 (in fn9478)
;;     9748 (in fn9478)
;;     97C2 (in fn9478)
;;     980C (in fn9478)
;;     9B1A (in fn9AF8)
;;     9C86 (in fn9C6E)
;;     9CAE (in fn9C92)
;;     9CD8 (in fn9CBC)
;;     9CE6 (in fn9CBC)
;;     9D44 (in fn9D34)
;;     9D52 (in fn9D34)
;;     9D84 (in fn9D6A)
;;     9DA0 (in fn9D6A)
;;     9DB6 (in fn9D6A)
;;     9DCE (in fn9DC0)
fn9BA6 proc
	mov.w	r6,@-sp
	mov.w	r2,@-sp
	mov.w	r1,r6
	jsr	@er0
	mov.w	r6,r0
	adds	#0x00000002,sp
	mov.w	@sp+,r6
	rts

;; fn9BB6: 9BB6
;;   Called from:
;;     8A94 (in fn8866)
;;     8AFE (in fn8AEA)
;;     8B1A (in fn8AEA)
;;     8B36 (in fn8AEA)
;;     8B52 (in fn8AEA)
;;     8B6E (in fn8AEA)
;;     8B82 (in fn8AEA)
;;     8B9E (in fn8AEA)
;;     8BE0 (in fn8BB0)
;;     8C04 (in fn8BB0)
;;     8C30 (in fn8BB0)
;;     8C7E (in fn8BB0)
;;     8CB8 (in fn8BB0)
;;     8CEC (in fn8BB0)
;;     8D3A (in fn8BB0)
;;     8D74 (in fn8BB0)
;;     8D9C (in fn8BB0)
;;     8DC4 (in fn8BB0)
;;     8E1A (in fn8BB0)
;;     8E54 (in fn8BB0)
;;     8E8C (in fn8BB0)
;;     8EDA (in fn8BB0)
;;     8F14 (in fn8BB0)
;;     8F3C (in fn8BB0)
;;     8F80 (in fn8F4E)
;;     8FA4 (in fn8F4E)
;;     8FC0 (in fn8F4E)
;;     8FF0 (in fn8F4E)
;;     902C (in fn8F4E)
;;     905C (in fn8F4E)
;;     9082 (in fn8F4E)
;;     90A8 (in fn8F4E)
;;     90E6 (in fn8F4E)
;;     9116 (in fn8F4E)
;;     9148 (in fn8F4E)
;;     9164 (in fn8F4E)
;;     9180 (in fn8F4E)
;;     91CC (in fn8F4E)
;;     91E0 (in fn8F4E)
;;     91FC (in fn8F4E)
;;     9222 (in fn8F4E)
;;     923E (in fn8F4E)
;;     925A (in fn8F4E)
;;     92A6 (in fn8F4E)
;;     92BA (in fn8F4E)
;;     92D6 (in fn8F4E)
;;     930A (in fn8F4E)
;;     9330 (in fn8F4E)
;;     934C (in fn8F4E)
;;     93D2 (in fn9370)
;;     93EE (in fn9370)
;;     9424 (in fn9370)
;;     9440 (in fn9370)
;;     9498 (in fn9478)
;;     95AC (in fn9478)
;;     95F0 (in fn9478)
;;     9608 (in fn9478)
;;     961C (in fn9478)
;;     9692 (in fn9478)
;;     9768 (in fn9478)
;;     9792 (in fn9478)
;;     97AE (in fn9478)
fn9BB6 proc
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
9BCE                                           6D F6               m.
9BD0 6F 73 00 06 6D F3 6F 73 00 06 6D F3 6D F2 0D 16 os..m.os..m.m...
9BE0 5D 00 0D 60 8F 06 97 00 6D 76 54 70             ]..`....mvTp    

;; fn9BEC: 9BEC
;;   Called from:
;;     9012 (in fn8F4E)
;;     91B2 (in fn8F4E)
;;     928C (in fn8F4E)
fn9BEC proc
	add.w	r0,r0
	mov.w	@(-24802:16,er0),r0
	rts

;; fn9BF4: 9BF4
;;   Called from:
;;     8FDE (in fn8F4E)
;;     918C (in fn8F4E)
;;     9266 (in fn8F4E)
fn9BF4 proc
	add.w	r0,r0
	mov.w	r1,@(-24802:16,er0)
	rts

;; fn9BFC: 9BFC
;;   Called from:
;;     9484 (in fn9478)
;;     94A8 (in fn9478)
fn9BFC proc
	add.w	r0,r0
	mov.w	@(-24814:16,er0),r2
	mov.w	r2,r0
	add.w	r0,r0
	add.w	r0,r0
	add.w	r2,r0
	add.w	r0,r0
	rts
9C0E                                           09 00               ..
9C10 19 22 6F 82 9F 12 54 70                         ."o...Tp        

;; fn9C18: 9C18
;;   Called from:
;;     8B08 (in fn8AEA)
;;     8B24 (in fn8AEA)
;;     8B40 (in fn8AEA)
;;     8B5C (in fn8AEA)
;;     8B8C (in fn8AEA)
;;     8BA8 (in fn8AEA)
;;     8DCE (in fn8BB0)
;;     8FCA (in fn8F4E)
;;     908C (in fn8F4E)
;;     9152 (in fn8F4E)
;;     916E (in fn8F4E)
;;     91EA (in fn8F4E)
;;     9206 (in fn8F4E)
;;     922C (in fn8F4E)
;;     9248 (in fn8F4E)
;;     92C4 (in fn8F4E)
;;     92E0 (in fn8F4E)
;;     933A (in fn8F4E)
;;     93DC (in fn9370)
;;     93F8 (in fn9370)
;;     942E (in fn9370)
;;     944A (in fn9370)
;;     979C (in fn9478)
fn9C18 proc
	mov.w	r4,@-sp
	mov.w	#0x9F1E,r4
	mov.w	r0,@er4
	mov.w	r0,r0
	ble	9C30

l9C24:
	jsr	@fn9D34
	mov.w	r0,r0
	bne	9C30

l9C2C:
	mov.w	@er4,r2
	bgt	9C24

l9C30:
	mov.w	@sp+,r4
	rts

;; fn9C34: 9C34
;;   Called from:
;;     94BE (in fn9478)
;;     94CA (in fn9478)
fn9C34 proc
	add.w	r0,r0
	add.w	r0,r0
	add.w	r0,r0
	mov.w	#0x9E94,r3
	add.w	r0,r3
	mov.w	r3,r0
	mov.b	r1l,@er0
	mov.b	r2l,@(1:16,er0)
	rts
9C4A                               6D F4 0D 04 09 44           m....D
9C50 09 44 09 44 C0 10 79 02 9E 94 09 42 0D 01 79 00 .D.D..y....B..y.
9C60 14 C0 5E 00 9B A6 6F 40 9E 98 6D 74 54 70       ..^...o@..mtTp  

;; fn9C6E: 9C6E
;;   Called from:
;;     8C48 (in fn8BB0)
;;     8D04 (in fn8BB0)
;;     8DE4 (in fn8BB0)
;;     8EA4 (in fn8BB0)
;;     8FFA (in fn8F4E)
;;     90B2 (in fn8F4E)
;;     9194 (in fn8F4E)
;;     926E (in fn8F4E)
;;     93FE (in fn9370)
fn9C6E proc
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
	jsr	@fn9BA6
	mov.w	@(-24938:16,er4),r0
	mov.w	@sp+,r4
	rts

;; fn9C92: 9C92
;;   Called from:
;;     8BEA (in fn8BB0)
;;     8F8A (in fn8F4E)
;;     90CC (in fn8F4E)
fn9C92 proc
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
	jsr	@fn9BA6
	mov.b	@(6:16,er4),r0l
	mov.b	#0x00,r0h
	mov.w	@sp+,r4
	rts

;; fn9CBC: 9CBC
;;   Called from:
;;     9534 (in fn9478)
;;     9810 (in fn9478)
fn9CBC proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r4,@-sp
	mov.w	r5,@-sp
	mov.w	r7,r5
	adds	#0x00000002,er5
	adds	#0x00000002,er5

l9CCA:
	mov.w	#0x0006,r2
	add.w	r7,r2
	mov.w	#0x4000,r1
	mov.w	#0x29F2,r0
	jsr	@fn9BA6
	mov.w	r5,r2
	mov.w	#0x3000,r1
	mov.w	#0x1FB6,r0
	jsr	@fn9BA6
	mov.w	@(6:16,sp),r3
	mov.w	#0x0002,r2
	cmp.w	r2,r3
	bne	9CFC

l9CF6:
	mov.w	@(4:16,sp),r2
	beq	9CCA

l9CFC:
	sub.w	r4,r4

l9CFE:
	jsr	@fn9D34
	mov.w	r0,r0
	beq	9D08

l9D06:
	sub.w	r4,r4

l9D08:
	adds	#0x00000001,er4
	mov.w	#0x03E7,r2
	cmp.w	r2,r4
	ble	9CFE

l9D12:
	mov.w	@(6:16,sp),r2
	beq	9D20

l9D18:
	mov.b	@(1:16,er5),r0l
	mov.b	#0x00,r0h
	bra	9D2A

l9D20:
	mov.b	@(1:16,er5),r2l
	or.b	#0x08,r2l
	mov.b	r2l,r0l
	mov.b	#0x00,r0h

l9D2A:
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts

;; fn9D34: 9D34
;;   Called from:
;;     9C24 (in fn9C18)
;;     9CFE (in fn9CBC)
fn9D34 proc
	subs	#0x00000002,sp
	subs	#0x00000002,sp
	mov.w	r7,r2
	adds	#0x00000002,er2
	mov.w	#0x4000,r1
	mov.w	#0x29F2,r0
	jsr	@fn9BA6
	mov.w	r7,r2
	mov.w	#0x3000,r1
	mov.w	#0x1FB6,r0
	jsr	@fn9BA6
	mov.w	@(2:16,sp),r2
	beq	9D60

l9D5C:
	mov.w	@sp,r0
	bra	9D64

l9D60:
	mov.w	@sp,r0
	or.b	#0x08,r0l

l9D64:
	adds	#0x00000002,sp
	adds	#0x00000002,sp
	rts

;; fn9D6A: 9D6A
;;   Called from:
;;     955E (in fn9478)
fn9D6A proc
	mov.w	#0x27AC,r0
	jsr	@fn9B90
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6
	jsr	@fn9DC0
	mov.w	#0x2A62,r0
	jsr	@fn9B90
	mov.w	#0x9F10,r1
	mov.w	#0x9F00,r2
	mov.w	#0x3B9A,r0
	jsr	@fn9BA6
	mov.w	#0x1498,r0
	jsr	@fn9B90
	sub.w	r2,r2
	mov.w	#0x4004,r1
	mov.w	#0x299A,r0
	jsr	@fn9BA6
	jsr	@fn9DC0
	rts

;; fn9DC0: 9DC0
;;   Called from:
;;     9D88 (in fn9D6A)
;;     9DBA (in fn9D6A)
fn9DC0 proc
	subs	#0x00000002,sp

l9DC2:
	mov.w	r7,r2
	adds	#0x00000001,er2
	mov.w	#0x700C,r1
	mov.w	#0x3CCC,r0
	jsr	@fn9BA6
	mov.b	@(1:16,sp),r2l
	bne	9DC2

l9DD8:
	adds	#0x00000002,sp
	rts

;; fn9DDC: 9DDC
;;   Called from:
;;     97D2 (in fn9478)
;;     9B36 (in fn9AF8)
fn9DDC proc
	mov.w	#0x3006,r1
	mov.w	#0x1B62,r0
	jsr	@fn9B9A
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	rts

;; fn9DF2: 9DF2
;;   Called from:
;;     95CA (in fn9478)
fn9DF2 proc
	mov.w	#0x3007,r1
	mov.w	#0x1B62,r0
	jsr	@fn9B9A
	mov.w	#0x27C8,r0
	jsr	@fn9B90
	rts

;; fn9E08: 9E08
;;   Called from:
;;     8940 (in fn8866)
fn9E08 proc
	mov.w	r0,r2
	mulxu.b	r1h,r2h
	mov.b	r0h,r2h
	mulxu.b	r1l,r0h
	add.b	r2l,r0h
	mulxu.b	r2h,r1h
	add.b	r1l,r0h
	rts

;; fn9E18: 9E18
;;   Called from:
;;     9B76 (in fn9B66)
fn9E18 proc
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
9E34             FF FF FF FF 03 13 03 0C 02 D0 03 3B     ...........;
9E40 00 04 00 03 00 04 00 06 00 06 00 04 00 03 00 04 ................
9E50 00 07 00 09 00 09 00 07 00 04 00 06 00 09 00 0C ................
9E60 00 0C 00 09 00 06 00 04 00 07 00 09 00 09 00 07 ................
9E70 00 04 00 03 00 04 00 06 00 06 00 04 00 03 00 64 ...............d
9E80 00 00                                           ..              
