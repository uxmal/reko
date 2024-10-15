;;; Segment .text (00011000)

;; fn00011000: 00011000
;;   Called from:
;;     00011120 (in fn000110E8)
fn00011000 proc
	addiu	sp,sp,-00000028
	sw	ra,0020(sp)
	sw	r7,0034(sp)
	sw	r6,0030(sp)
	sw	r5,002C(sp)
	sw	r4,0028(sp)
	addiu	r6,r0,+00000005
	or	r5,r0,r0
	addiu	r4,sp,+00000014
	jal	0001176C
	nop
	addiu	r5,r0,+00000005
	addiu	r4,r0,+00000001
	jal	0001175C
	nop
	sw	r2,0010(sp)
	addiu	r6,r0,+00000005
	addiu	r5,sp,+00000014
	lw	r4,0010(sp)
	jal	0001174C
	nop
	addiu	r4,r0,+0000000C
	lw	r8,0010(sp)
	sb	r4,0000(r8)
	lw	r8,0010(sp)
	addiu	r8,r8,+00000001
	swl	r0,0003(r8)
	swr	r0,0000(r8)
	addiu	r8,r0,+00000042
	sb	r8,0014(sp)
	lw	r8,0010(sp)
	swl	r8,0018(sp)
	swr	r8,0015(sp)
	or	r8,r0,r0
	sw	r8,001C(sp)
	lw	r2,001C(sp)
	lw	ra,0020(sp)
	addiu	sp,sp,+00000028
	jr	ra
	nop

;; Win32CrtStartup: 000110A0
Win32CrtStartup proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	sw	r7,0024(sp)
	sw	r6,0020(sp)
	sw	r5,001C(sp)
	sw	r4,0018(sp)
	jal	fn000111C4
	nop
	lw	r7,0024(sp)
	lw	r6,0020(sp)
	lw	r5,001C(sp)
	lw	r4,0018(sp)
	jal	fn000110E8
	nop
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn000110E8: 000110E8
;;   Called from:
;;     000110D0 (in Win32CrtStartup)
fn000110E8 proc
	addiu	sp,sp,-00000028
	sw	ra,0024(sp)
	sw	r30,0020(sp)
	or	r30,sp,r0
	sw	r7,0034(r30)
	sw	r6,0030(r30)
	sw	r5,002C(r30)
	sw	r4,0028(r30)
	jal	fn000114E4
	nop
	lw	r7,0034(r30)
	lw	r6,0030(r30)
	lw	r5,002C(r30)
	lw	r4,0028(r30)
	jal	fn00011000
	nop
	sw	r2,0010(r30)
	lw	r4,0010(r30)
	jal	fn00011278
	nop
	beq	r0,r0,00011154
	nop
00011140 10 00 C4 8F F6 44 00 0C 00 00 00 00 01 00 00 10 .....D..........
00011150 00 00 00 00                                     ....            

l00011154:
	or	sp,r30,r0
	lw	r30,0020(sp)
	lw	ra,0024(sp)
	addiu	sp,sp,+00000028
	jr	ra
	nop

;; fn0001116C: 0001116C
fn0001116C proc
	addiu	sp,sp,-00000018
	sw	r30,0010(sp)
	sw	ra,0014(sp)
	addiu	r30,r2,-00000028
	sw	r4,0014(r30)
	lw	r8,0014(r30)
	sw	r8,0018(r30)
	lw	r8,0014(r30)
	lw	r8,0000(r8)
	lw	r8,0000(r8)
	sw	r8,001C(r30)
	lw	r8,001C(r30)
	sw	r8,0010(r30)
	lw	r5,0018(r30)
	lw	r4,0010(r30)
	jal	0001177C
	nop
	lw	r30,0010(sp)
	lw	ra,0014(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn000111C4: 000111C4
;;   Called from:
;;     000110B8 (in Win32CrtStartup)
fn000111C4 proc
	addiu	sp,sp,-00000020
	sw	ra,0018(sp)
	lui	r8,%hi(00013030)
	lw	r8,%lo(00013030)(r8)
	sw	r8,0014(sp)
	lw	r8,0014(sp)
	beq	r8,r0,0001120C
	nop

l000111E4:
	lw	r9,0014(sp)
	ori	r8,r0,0000B064
	beq	r9,r8,0001120C
	nop

l000111F4:
	lw	r8,0014(sp)
	nor	r9,r8,r0
	lui	r8,%hi(00013034)
	sw	r9,%lo(00013034)(r8)
	beq	r0,r0,00011268
	nop

l0001120C:
	jal	0001179C
	nop
	sw	r2,0014(sp)
	lw	r8,0014(sp)
	andi	r8,r8,0000FFFF
	sw	r8,0010(sp)
	lw	r8,0014(sp)
	srl	r9,r8,10
	lw	r8,0010(sp)
	xor	r8,r9,r8
	sw	r8,0014(sp)
	lw	r8,0014(sp)
	bne	r8,r0,0001124C
	nop

l00011244:
	ori	r8,r0,0000B064
	sw	r8,0014(sp)

l0001124C:
	lw	r9,0014(sp)
	lui	r8,%hi(00013030)
	sw	r9,%lo(00013030)(r8)
	lw	r8,0014(sp)
	nor	r9,r8,r0
	lui	r8,%hi(00013034)
	sw	r9,%lo(00013034)(r8)

l00011268:
	lw	ra,0018(sp)
	addiu	sp,sp,+00000020
	jr	ra
	nop

;; fn00011278: 00011278
;;   Called from:
;;     00011130 (in fn000110E8)
fn00011278 proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	sw	r4,0018(sp)
	or	r6,r0,r0
	or	r5,r0,r0
	lw	r4,0018(sp)
	jal	fn000112A8
	nop
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn000112A8: 000112A8
;;   Called from:
;;     00011290 (in fn00011278)
;;     000113F0 (in fn000113D8)
;;     0001141C (in fn00011408)
;;     00011448 (in fn00011434)
fn000112A8 proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	sw	r6,0020(sp)
	sw	r5,001C(sp)
	sw	r4,0018(sp)
	lw	r8,0020(sp)
	sll	r9,r8,18
	sra	r9,r9,18
	lui	r8,%hi(00013038)
	sb	r9,%lo(00013038)(r8)
	lw	r8,001C(sp)
	bne	r8,r0,0001138C
	nop

l000112DC:
	lui	r8,%hi(00013040)
	lw	r8,%lo(00013040)(r8)
	beq	r8,r0,00011374
	nop

l000112EC:
	lui	r8,%hi(0001303C)
	lw	r8,%lo(0001303C)(r8)
	addiu	r9,r8,-00000004
	lui	r8,%hi(0001303C)
	sw	r9,%lo(0001303C)(r8)
	lui	r8,%hi(0001303C)
	lw	r9,%lo(0001303C)(r8)
	lui	r8,%hi(00013040)
	lw	r8,%lo(00013040)(r8)
	sltu	r8,r9,r8
	bne	r8,r0,0001134C
	nop

l0001131C:
	lui	r8,%hi(0001303C)
	lw	r8,%lo(0001303C)(r8)
	lw	r8,0000(r8)
	beq	r8,r0,00011344
	nop

l00011330:
	lui	r8,%hi(0001303C)
	lw	r8,%lo(0001303C)(r8)
	lw	r8,0000(r8)
	jalr	ra,r8
	nop

l00011344:
	beq	r0,r0,000112EC
	nop

l0001134C:
	lui	r8,%hi(00013040)
	lw	r4,%lo(00013040)(r8)
	jal	000117AC
	nop
	lui	r4,%hi(0001303C)
	sw	r0,%lo(0001303C)(r4)
	lui	r8,%hi(0001303C)
	lw	r9,%lo(0001303C)(r8)
	lui	r8,%hi(00013040)
	sw	r9,%lo(00013040)(r8)

l00011374:
	lui	r8,+0001
	addiu	r5,r8,+00002014
	lui	r8,+0001
	addiu	r4,r8,+00002010
	jal	fn0001147C
	nop

l0001138C:
	lui	r8,+0001
	addiu	r5,r8,+0000201C
	lui	r8,+0001
	addiu	r4,r8,+00002018
	jal	fn0001147C
	nop
	lw	r8,0020(sp)
	bne	r8,r0,000113C8
	nop

l000113B0:
	jal	fn00011460
	nop
	lw	r5,0018(sp)
	or	r4,r2,r0
	jal	000117EC
	nop

l000113C8:
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn000113D8: 000113D8
fn000113D8 proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	sw	r4,0018(sp)
	or	r6,r0,r0
	addiu	r5,r0,+00000001
	lw	r4,0018(sp)
	jal	fn000112A8
	nop
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn00011408: 00011408
fn00011408 proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	addiu	r6,r0,+00000001
	or	r5,r0,r0
	or	r4,r0,r0
	jal	fn000112A8
	nop
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn00011434: 00011434
fn00011434 proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	addiu	r6,r0,+00000001
	addiu	r5,r0,+00000001
	or	r4,r0,r0
	jal	fn000112A8
	nop
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn00011460: 00011460
;;   Called from:
;;     000113B0 (in fn000112A8)
fn00011460 proc
	addiu	sp,sp,-00000008
	addiu	r8,r0,+00000042
	sw	r8,0000(sp)
	lw	r2,0000(sp)
	addiu	sp,sp,+00000008
	jr	ra
	nop

;; fn0001147C: 0001147C
;;   Called from:
;;     00011384 (in fn000112A8)
;;     0001139C (in fn000112A8)
;;     000114FC (in fn000114E4)
;;     00011514 (in fn000114E4)
fn0001147C proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	sw	r5,001C(sp)
	sw	r4,0018(sp)

l0001148C:
	lw	r9,0018(sp)
	lw	r8,001C(sp)
	sltu	r8,r9,r8
	beq	r8,r0,000114D4
	nop

l000114A0:
	lw	r8,0018(sp)
	lw	r8,0000(r8)
	beq	r8,r0,000114C0
	nop

l000114B0:
	lw	r8,0018(sp)
	lw	r8,0000(r8)
	jalr	ra,r8
	nop

l000114C0:
	lw	r8,0018(sp)
	addiu	r8,r8,+00000004
	sw	r8,0018(sp)
	beq	r0,r0,0001148C
	nop

l000114D4:
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn000114E4: 000114E4
;;   Called from:
;;     00011108 (in fn000110E8)
fn000114E4 proc
	addiu	sp,sp,-00000018
	sw	ra,0010(sp)
	lui	r8,+0001
	addiu	r5,r8,+0000200C
	lui	r8,+0001
	addiu	r4,r8,+00002008
	jal	fn0001147C
	nop
	lui	r4,+0001
	addiu	r5,r4,+00002004
	lui	r8,+0001
	addiu	r4,r8,+00002000
	jal	fn0001147C
	nop
	lw	ra,0010(sp)
	addiu	sp,sp,+00000018
	jr	ra
	nop

;; fn0001152C: 0001152C
;;   Called from:
;;     0001170C (in fn000116FC)
fn0001152C proc
	addiu	sp,sp,-00000038
	sw	ra,0030(sp)
	sw	r4,0038(sp)
	sw	r0,0014(sp)
	lui	r8,%hi(00013040)
	lw	r8,%lo(00013040)(r8)
	sw	r8,001C(sp)
	lui	r8,%hi(0001303C)
	lw	r8,%lo(0001303C)(r8)
	sw	r8,0028(sp)
	sw	r0,0018(sp)
	sw	r0,0024(sp)
	lw	r9,0028(sp)
	lw	r8,001C(sp)
	subu	r8,r9,r8
	sw	r8,0010(sp)
	lw	r8,0010(sp)
	bgez	r8,00011588
	nop

l00011578:
	or	r8,r0,r0
	sw	r8,002C(sp)
	beq	r0,r0,000116E8
	nop

l00011588:
	lw	r8,0010(sp)
	addiu	r8,r8,+00000004
	sw	r8,0024(sp)
	lw	r8,001C(sp)
	beq	r8,r0,000115B0
	nop

l000115A0:
	lw	r4,001C(sp)
	jal	000117DC
	nop
	sw	r2,0018(sp)

l000115B0:
	lw	r9,0018(sp)
	lw	r8,0024(sp)
	sltu	r8,r9,r8
	beq	r8,r0,000116B0
	nop

l000115C4:
	lw	r8,001C(sp)
	bne	r8,r0,000115E8
	nop

l000115D0:
	addiu	r4,r0,+00000010
	jal	000117CC
	nop
	sw	r2,0014(sp)
	beq	r0,r0,0001166C
	nop

l000115E8:
	lw	r8,0018(sp)
	sll	r8,r8,01
	sw	r8,0020(sp)
	lw	r8,0018(sp)
	sltiu	r8,r8,+00000201
	bne	r8,r0,00011610
	nop

l00011604:
	lw	r8,0018(sp)
	addiu	r8,r8,+00000200
	sw	r8,0020(sp)

l00011610:
	lw	r9,0020(sp)
	lw	r8,0018(sp)
	sltu	r8,r8,r9
	beq	r8,r0,00011638
	nop

l00011624:
	lw	r5,0020(sp)
	lw	r4,001C(sp)
	jal	000117BC
	nop
	sw	r2,0014(sp)

l00011638:
	lw	r8,0014(sp)
	bne	r8,r0,0001166C
	nop

l00011644:
	lw	r9,0024(sp)
	lw	r8,0018(sp)
	sltu	r8,r8,r9
	beq	r8,r0,0001166C
	nop

l00011658:
	lw	r5,0024(sp)
	lw	r4,001C(sp)
	jal	000117BC
	nop
	sw	r2,0014(sp)

l0001166C:
	lw	r8,0014(sp)
	bne	r8,r0,00011688
	nop

l00011678:
	or	r8,r0,r0
	sw	r8,002C(sp)
	beq	r0,r0,000116E8
	nop

l00011688:
	lw	r9,0028(sp)
	lw	r8,001C(sp)
	subu	r8,r9,r8
	sra	r8,r8,02
	sll	r9,r8,02
	lw	r8,0014(sp)
	addu	r8,r8,r9
	sw	r8,0028(sp)
	lw	r8,0014(sp)
	sw	r8,001C(sp)

l000116B0:
	lw	r9,0038(sp)
	lw	r8,0028(sp)
	sw	r9,0000(r8)
	lw	r8,0028(sp)
	addiu	r8,r8,+00000004
	sw	r8,0028(sp)
	lw	r9,0028(sp)
	lui	r8,%hi(0001303C)
	sw	r9,%lo(0001303C)(r8)
	lw	r9,001C(sp)
	lui	r8,%hi(00013040)
	sw	r9,%lo(00013040)(r8)
	lw	r8,0038(sp)
	sw	r8,002C(sp)

l000116E8:
	lw	r2,002C(sp)
	lw	ra,0030(sp)
	addiu	sp,sp,+00000038
	jr	ra
	nop

;; fn000116FC: 000116FC
fn000116FC proc
	addiu	sp,sp,-00000020
	sw	ra,0018(sp)
	sw	r4,0020(sp)
	lw	r4,0020(sp)
	jal	fn0001152C
	nop
	beq	r2,r0,00011728
	nop

l0001171C:
	sw	r0,0014(sp)
	beq	r0,r0,00011730
	nop

l00011728:
	addiu	r8,r0,-00000001
	sw	r8,0014(sp)

l00011730:
	lw	r8,0014(sp)
	sw	r8,0010(sp)
	lw	r2,0010(sp)
	lw	ra,0018(sp)
	addiu	sp,sp,+00000020
	jr	ra
	nop
0001174C                                     01 00 08 3C             ...<
00011750 00 30 08 8D 08 00 00 01 00 00 00 00 01 00 08 3C .0.............<
00011760 04 30 08 8D 08 00 00 01 00 00 00 00 01 00 08 3C .0.............<
00011770 08 30 08 8D 08 00 00 01 00 00 00 00 01 00 08 3C .0.............<
00011780 0C 30 08 8D 08 00 00 01 00 00 00 00 01 00 08 3C .0.............<
00011790 10 30 08 8D 08 00 00 01 00 00 00 00             .0..........    
0001179C                                     01 00 08 3C             ...<
000117A0 14 30 08 8D 08 00 00 01 00 00 00 00 01 00 08 3C .0.............<
000117B0 18 30 08 8D 08 00 00 01 00 00 00 00             .0..........    
000117BC                                     01 00 08 3C             ...<
000117C0 1C 30 08 8D 08 00 00 01 00 00 00 00             .0..........    
000117CC                                     01 00 08 3C             ...<
000117D0 20 30 08 8D 08 00 00 01 00 00 00 00              0..........    
000117DC                                     01 00 08 3C             ...<
000117E0 24 30 08 8D 08 00 00 01 00 00 00 00 01 00 08 3C $0.............<
000117F0 28 30 08 8D 08 00 00 01 00 00 00 00             (0..........    
