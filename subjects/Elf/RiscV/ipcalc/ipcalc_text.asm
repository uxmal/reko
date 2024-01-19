;;; Segment .text (0000000000015180)

;; fn0000000000015180: 0000000000015180
fn0000000000015180 proc
	addi	sp,sp,-0x1C0
	sd	s5,0x188(sp)
	ld	a5,-0x790(gp)
	sd	s1,0x1A8(sp)
	lui	s1,0x11
	sd	a5,0x148(sp)
	lui	a5,0x10
	addiw	a5,a5,0x8
	sd	s0,0x1B0(sp)
	sd	s2,0x1A0(sp)
	sd	s3,0x198(sp)
	sd	s4,0x190(sp)
	sd	s6,0x180(sp)
	sd	s7,0x178(sp)
	sd	s8,0x170(sp)
	sd	s9,0x168(sp)
	sd	s10,0x160(sp)
	sd	s11,0x158(sp)
	sd	ra,0x1B8(sp)
	lui	s8,0x1
	addi	s3,a0,0x0
	addi	s2,a1,0x0
	sw	zero,0x1C(sp)
	addi	s0,zero,0x0
	addi	s6,zero,0x0
	addi	s9,zero,0x0
	addi	s10,zero,0x0
	sw	zero,(sp)
	addi	s1,s1,-0x720
	lui	s4,0x12
	addi	s7,zero,0x1
	sw	a5,0x8(sp)

l0000000000015200:
	addi	a4,zero,0x0
	addi	a3,s1,0x0
	addi	a2,s4,-0x658
	addi	a1,s2,0x0
	addi	a0,s3,0x0
	jal	ra,getopt_long
	addi	a4,zero,-0x1
	addi	a5,a0,0x0
	beq	a0,a4,0x0000000000015348

l0000000000015224:
	addi	a4,zero,0x76
	bltu	a4,a0,0x0000000000015200

l000000000001522C:
	slli	a5,a5,0x20
	lui	a4,0x10
	srli	a5,a5,0x1E
	addi	a4,a4,0x2A0
	add	a5,a5,a4
	lw	a5,(a5)
	jalr	zero,a5,0x0
0000000000015248                         23 20 71 01 6F F0 5F FB         # q.o._.
0000000000015250 B7 07 04 00 33 64 F4 00 6F F0 9F FA B7 07 02 00 ....3d..o.......
0000000000015260 33 64 F4 00 6F F0 DF F9 13 05 00 00 EF 00 10 56 3d..o..........V
0000000000015270 13 05 00 00 EF F0 DF D6 B7 27 00 00 33 64 F4 00 .........'..3d..
0000000000015280 6F F0 1F F8 33 64 84 01 6F F0 9F F7 B7 17 00 00 o...3d..o.......
0000000000015290 9B 87 07 80 33 64 F4 00 6F F0 9F F6 13 64 04 40 ....3d..o....d.@
00000000000152A0 6F F0 1F F6 83 27 81 00 33 64 F4 00 6F F0 5F F5 o....'..3d..o._.
00000000000152B0 13 8B 0B 00 6F F0 DF F4 23 AA 71 81 6F F0 5F F4 ....o...#.q.o._.
00000000000152C0 03 B5 81 87 EF F0 DF C6 13 0D 05 00 E3 1A 05 F2 ................
00000000000152D0 13 05 10 00 EF F0 DF D0 13 64 04 20 6F F0 5F F2 .........d. o._.
00000000000152E0 03 B5 81 87 EF F0 DF C4 93 0C 05 00 E3 1A 05 F0 ................
00000000000152F0 6F F0 1F FE 13 64 04 10 6F F0 9F F0 13 64 04 08 o....d..o....d..
0000000000015300 6F F0 1F F0 13 64 84 00 6F F0 9F EF 13 64 14 00 o....d..o....d..
0000000000015310 6F F0 1F EF B7 C7 00 00 33 64 F4 00 6F F0 5F EE o.......3d..o._.
0000000000015320 13 64 44 00 6F F0 DF ED 13 64 04 04 6F F0 5F ED .dD.o....d..o._.
0000000000015330 13 05 10 00 EF 00 90 49 13 05 00 00 EF F0 5F CA .......I......_.
0000000000015340 23 2E 71 01 6F F0 DF EB                         #.q.o...        

l0000000000015348:
	lw	a4,-0x770(gp)
	bge	a4,s3,0x0000000000015428

l0000000000015350:
	addiw	a1,a4,0x1
	slli	a3,a4,0x3
	add	s2,s2,a3
	sw	a1,-0x770(gp)
	ld	s1,(s2)
	addi	s4,zero,0x0
	bge	a1,s3,0x0000000000015378

l000000000001536C:
	ld	s4,0x8(s2)
	addiw	a4,a4,0x2
	sw	a4,-0x770(gp)

l0000000000015378:
	bne	s6,zero,0x0000000000015588

l000000000001537C:
	lw	a4,0x1C(sp)
	beq	a4,zero,0x00000000000153B4

l0000000000015384:
	lw	a4,(sp)
	beq	a4,zero,0x00000000000153B4

l000000000001538C:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x00000000000153D8

l0000000000015394:
	ld	a3,-0x780(gp)
	lui	a0,0x12
	addi	a2,zero,0x2E
	addi	a1,zero,0x1
	addi	a0,a0,-0x628
	jal	ra,fwrite
	lw	s6,(sp)
	jal	zero,0x00000000000153DC

l00000000000153B4:
	sd	a5,0x8(sp)
	beq	s9,zero,0x0000000000015924

l00000000000153BC:
	jal	ra,fn0000000000017924
	ori	s2,s0,0x2
	ld	a5,0x8(sp)
	beq	a0,zero,0x0000000000015434

l00000000000153CC:
	beq	s10,zero,0x00000000000155A8

l00000000000153D0:
	lw	a5,-0x7EC(gp)
	beq	a5,zero,0x000000000001556C

l00000000000153D8:
	addi	s6,zero,0x1

l00000000000153DC:
	ld	a4,0x148(sp)
	ld	a5,-0x790(gp)
	addi	a0,s6,0x0
	bne	a4,a5,0x0000000000015E8C

l00000000000153EC:
	ld	ra,0x1B8(sp)
	ld	s0,0x1B0(sp)
	ld	s1,0x1A8(sp)
	ld	s2,0x1A0(sp)
	ld	s3,0x198(sp)
	ld	s4,0x190(sp)
	ld	s5,0x188(sp)
	ld	s6,0x180(sp)
	ld	s7,0x178(sp)
	ld	s8,0x170(sp)
	ld	s9,0x168(sp)
	ld	s10,0x160(sp)
	ld	s11,0x158(sp)
	addi	sp,sp,0x1C0
	jalr	zero,ra,0x0

l0000000000015428:
	addi	s4,zero,0x0
	addi	s1,zero,0x0
	jal	zero,0x0000000000015378

l0000000000015434:
	lui	a4,0x10
	addiw	a4,a4,0x8
	and	s0,s0,a4
	bne	s0,a4,0x00000000000153CC

l0000000000015444:
	addi	s0,s2,0x0

l0000000000015448:
	lui	s2,0x4
	or	s2,s0,s2
	bne	s9,zero,0x00000000000153CC

l0000000000015454:
	addi	s0,s2,0x0

l0000000000015458:
	beq	s10,zero,0x0000000000015CE4

l000000000001545C:
	addi	a2,zero,0x1
	addi	a1,s10,0x0
	addi	a0,sp,0x1C
	jal	ra,fn00000000000166F4
	addi	a5,a0,0x0
	blt	a0,zero,0x0000000000015BC8

l0000000000015474:
	addi	a1,sp,0x28
	addi	a0,zero,0x2
	sd	a5,0x8(sp)
	lw	s1,0x1C(sp)
	sd	zero,0x20(sp)
	jal	ra,clock_gettime
	ld	a5,0x8(sp)
	blt	a0,zero,0x0000000000015CB0

l0000000000015494:
	beq	s1,zero,0x0000000000015C48

l0000000000015498:
	ld	a4,0x30(sp)
	addi	a1,zero,0xF
	addi	a0,sp,0x39
	andi	a4,a4,0x1
	ori	a4,a4,-0x4
	sb	a4,0x38(sp)
	jal	ra,fn00000000000164D0
	ld	a5,0x8(sp)
	blt	a0,zero,0x0000000000015CB0

l00000000000154BC:
	addi	s1,sp,0x108
	addi	a3,zero,0x40
	addi	a2,s1,0x0
	addi	a1,sp,0x38
	addi	a0,zero,0xA
	jal	ra,inet_ntop
	ld	a5,0x8(sp)
	beq	a0,zero,0x0000000000015CB0

l00000000000154DC:
	lui	a2,0x12
	addi	a4,a5,0x0
	addi	a3,s1,0x0
	addi	a2,a2,-0x570
	addi	a1,zero,0x1
	addi	a0,sp,0x20
	sd	a5,0x8(sp)
	jal	ra,__asprintf_chk
	addi	a4,zero,-0x1
	ld	a5,0x8(sp)
	beq	a0,a4,0x0000000000015CB0

l0000000000015508:
	ld	s1,0x20(sp)
	beq	s1,zero,0x0000000000015CB0

l0000000000015510:
	lw	a4,(sp)
	bne	a4,zero,0x00000000000155D8

l0000000000015518:
	addi	a1,zero,0x3A
	addi	a0,s1,0x0
	sd	a5,(sp)
	jal	ra,strchr
	ld	a5,(sp)
	beq	a0,zero,0x00000000000155D8

l0000000000015530:
	addi	a4,zero,0x1
	sw	a4,0x1C(sp)
	beq	s4,zero,0x0000000000015BEC

l000000000001553C:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x00000000000153D8

l0000000000015544:
	ld	a0,-0x780(gp)
	lui	a2,0x12
	addi	a3,s4,0x0
	addi	a2,a2,-0x530
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	a0,zero,0x1
	jal	ra,fn0000000000015FCC
	addi	s6,zero,0x1
	jal	zero,0x00000000000153DC

l000000000001556C:
	ld	a3,-0x780(gp)
	lui	a0,0x12
	addi	a2,zero,0x25
	addi	a1,zero,0x1
	addi	a0,a0,-0x5F8
	jal	ra,fwrite
	jal	zero,0x00000000000153D8

l0000000000015588:
	lui	a2,0x12
	lui	a1,0x12
	addi	a2,a2,-0x640
	addi	a1,a1,-0x638
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	addi	s6,zero,0x0
	jal	zero,0x00000000000153DC

l00000000000155A8:
	lw	a4,0x1C(sp)
	bne	a4,zero,0x0000000000015944

l00000000000155B0:
	lw	a4,(sp)
	beq	a4,zero,0x000000000001598C

l00000000000155B8:
	addi	a1,s9,0x0
	addi	a0,zero,0x2
	sd	a5,(sp)
	jal	ra,fn00000000000169D8
	addi	s1,a0,0x0
	beq	a0,zero,0x0000000000015964

l00000000000155D0:
	ld	a5,(sp)
	addi	s0,s2,0x0

l00000000000155D8:
	beq	s4,zero,0x0000000000015BA0

l00000000000155DC:
	lw	a5,0x1C(sp)
	bne	a5,zero,0x000000000001553C

l00000000000155E4:
	addi	a2,zero,0x0
	addi	a1,s4,0x0
	addi	a0,sp,0x1C
	jal	ra,fn00000000000166F4
	addi	a5,a0,0x0
	blt	a0,zero,0x0000000000015B6C

l00000000000155FC:
	lw	a4,0x1C(sp)
	beq	a4,zero,0x00000000000159B0

l0000000000015604:
	addi	a3,s0,0x0
	addi	a2,sp,0x38
	addi	a1,a5,0x0
	addi	a0,s1,0x0
	jal	ra,fn00000000000175BC

l0000000000015618:
	blt	a0,zero,0x00000000000153D8

l000000000001561C:
	lui	a5,0xFFFAC
	addiw	a5,a5,-0x1
	and	a5,s0,a5
	bne	a5,zero,0x0000000000015630

l000000000001562C:
	ori	s0,s0,0x8

l0000000000015630:
	addi	a0,zero,0x1
	jal	ra,isatty
	beq	a0,zero,0x0000000000015644

l000000000001563C:
	addi	a4,zero,0x1
	sw	a4,-0x7F0(gp)

l0000000000015644:
	andi	a5,s0,0x8
	beq	a5,zero,0x00000000000159C8

l000000000001564C:
	lw	a5,0x1C(sp)
	lw	s2,0xD8(sp)
	bne	a5,zero,0x000000000001590C

l0000000000015658:
	addi	a5,zero,0x20
	beq	s2,a5,0x0000000000015914

l0000000000015660:
	beq	s10,zero,0x0000000000015860

l0000000000015664:
	lui	s1,0x12

l0000000000015668:
	ld	a3,0x48(sp)
	lui	s3,0x12
	beq	a3,zero,0x0000000000015690

l0000000000015674:
	lui	a1,0x12
	addi	a4,s2,0x0
	addi	a2,s3,-0x4E0
	addi	a1,a1,-0x4D8
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	lw	s2,0xD8(sp)

l0000000000015690:
	ld	a3,0x58(sp)
	lui	a1,0x12
	addi	a4,s2,0x0
	addi	a2,s3,-0x4E0
	addi	a1,a1,-0x4C8
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	ld	a3,0x68(sp)
	lw	a4,0xD8(sp)
	lui	a2,0x12
	lui	a1,0x12
	addi	a2,a2,-0x4B8
	addi	a1,a1,-0x4A8
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	ld	a3,0x60(sp)
	beq	a3,zero,0x00000000000156EC

l00000000000156D4:
	lui	s2,0x12
	lui	a1,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x498
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C

l00000000000156EC:
	lui	a5,0x10
	addiw	a5,a5,0x8
	and	s0,s0,a5
	beq	s0,a5,0x0000000000015C1C

l00000000000156FC:
	addi	a0,zero,0xA
	jal	ra,putchar
	ld	a3,0xF0(sp)
	beq	a3,zero,0x0000000000015728

l000000000001570C:
	lui	a2,0x12
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x478
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C

l0000000000015728:
	ld	a3,0xF8(sp)
	beq	a3,zero,0x000000000001574C

l0000000000015730:
	lui	a2,0x12
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x460
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C

l000000000001574C:
	ld	a3,0xE0(sp)
	beq	a3,zero,0x000000000001576C

l0000000000015754:
	lui	a2,0x12
	lui	a1,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x450
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C

l000000000001576C:
	ld	a3,0xE8(sp)
	beq	a3,zero,0x000000000001578C

l0000000000015774:
	lui	a2,0x12
	lui	a1,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x440
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C

l000000000001578C:
	lw	a5,0x1C(sp)
	beq	a5,zero,0x00000000000157A0

l0000000000015794:
	lw	a5,0xD8(sp)
	addi	a4,zero,0x6F
	bgeu	a4,a5,0x0000000000015E3C

l00000000000157A0:
	lui	a2,0x12
	lui	a1,0x12
	addi	a3,sp,0x98
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x420
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C

l00000000000157BC:
	ld	a5,0x78(sp)
	beq	a5,zero,0x0000000000015C08

l00000000000157C4:
	addi	a0,zero,0xA
	jal	ra,putchar
	ld	a3,0x80(sp)
	beq	a3,zero,0x00000000000157F0

l00000000000157D4:
	lui	a2,0x12
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x410
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C

l00000000000157F0:
	ld	a3,0x78(sp)
	beq	a3,zero,0x0000000000015814

l00000000000157F8:
	lui	a2,0x12
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x400
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C

l0000000000015814:
	ld	a3,0x88(sp)
	beq	a3,zero,0x0000000000015838

l000000000001581C:
	lui	a2,0x12
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x3F0
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C

l0000000000015838:
	ld	a3,0x90(sp)
	beq	a3,zero,0x00000000000153DC

l0000000000015840:
	lui	a2,0x12
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,a2,-0x3A0
	addi	a1,a1,-0x3E8
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C
	jal	zero,0x00000000000153DC

l0000000000015860:
	ld	s3,0x38(sp)
	ld	a0,0x58(sp)
	addi	a1,s3,0x0
	jal	ra,strcmp
	beq	a0,zero,0x0000000000015664

l0000000000015874:
	ld	a3,0x40(sp)
	addi	s4,zero,0x0
	beq	a3,zero,0x0000000000015E14

l0000000000015880:
	lui	s2,0x12
	lui	s1,0x12
	lui	a1,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x508
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	ld	a3,0x38(sp)
	lui	a1,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x4F0
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	beq	s4,zero,0x0000000000015E74

l00000000000158B8:
	lui	a5,0x10
	addiw	a5,a5,0x8
	and	s0,s0,a5
	beq	s0,a5,0x0000000000015E64

l00000000000158C8:
	ld	a3,0xF0(sp)
	beq	a3,zero,0x00000000000158E8

l00000000000158D0:
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x478
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C

l00000000000158E8:
	ld	a3,0xF8(sp)
	beq	a3,zero,0x00000000000157BC

l00000000000158F0:
	lui	a1,0x12
	lui	a0,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x460
	addi	a0,a0,-0x468
	jal	ra,fn000000000001654C
	jal	zero,0x00000000000157BC

l000000000001590C:
	addi	a5,zero,0x80
	bne	s2,a5,0x0000000000015660

l0000000000015914:
	ld	a3,0x40(sp)
	beq	a3,zero,0x0000000000015DDC

l000000000001591C:
	addi	s4,zero,0x1
	jal	zero,0x0000000000015880

l0000000000015924:
	jal	ra,fn0000000000017924
	ld	a5,0x8(sp)
	bne	a0,zero,0x0000000000015458

l0000000000015930:
	lui	a4,0x10
	addiw	a4,a4,0x8
	and	a3,s0,a4
	bne	a3,a4,0x0000000000015458

l0000000000015940:
	jal	zero,0x0000000000015448

l0000000000015944:
	addi	a1,s9,0x0
	addi	a0,zero,0xA
	sd	a5,0x8(sp)
	jal	ra,fn00000000000169D8
	addi	s1,a0,0x0
	addi	s0,s2,0x0
	ld	a5,0x8(sp)
	bne	a0,zero,0x0000000000015510

l0000000000015964:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x00000000000153D8

l000000000001596C:
	ld	a0,-0x780(gp)
	lui	a2,0x12
	addi	a3,s9,0x0
	addi	a2,a2,-0x5B0
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	s6,zero,0x1
	jal	zero,0x00000000000153DC

l000000000001598C:
	addi	a1,s9,0x0
	addi	a0,zero,0x0
	sd	a5,(sp)
	jal	ra,fn00000000000169D8
	addi	s1,a0,0x0
	beq	a0,zero,0x0000000000015964

l00000000000159A4:
	addi	s0,s2,0x0
	ld	a5,(sp)
	jal	zero,0x0000000000015518

l00000000000159B0:
	addi	a3,s0,0x0
	addi	a2,sp,0x38
	addi	a1,a5,0x0
	addi	a0,s1,0x0
	jal	ra,fn0000000000016D74
	jal	zero,0x0000000000015618

l00000000000159C8:
	andi	a5,s0,0x80
	bne	a5,zero,0x0000000000015D18

l00000000000159D0:
	andi	a5,s0,0x200
	bne	a5,zero,0x0000000000015D48

l00000000000159D8:
	andi	a5,s0,0x40
	beq	a5,zero,0x00000000000159FC

l00000000000159E0:
	lw	a5,0x1C(sp)
	bne	a5,zero,0x00000000000159FC

l00000000000159E8:
	ld	a2,0x60(sp)
	lui	a1,0x12
	addi	a1,a1,-0x3B8
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l00000000000159FC:
	andi	a5,s0,0x100
	bne	a5,zero,0x0000000000015D30

l0000000000015A04:
	slli	a5,s0,0x2E
	blt	a5,zero,0x0000000000015D78

l0000000000015A0C:
	andi	a5,s0,0x400
	beq	a5,zero,0x0000000000015A2C

l0000000000015A14:
	ld	a2,0xE0(sp)
	beq	a2,zero,0x0000000000015A2C

l0000000000015A1C:
	lui	a1,0x12
	addi	a1,a1,-0x388
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015A2C:
	slli	a5,s0,0x34
	bge	a5,zero,0x0000000000015A4C

l0000000000015A34:
	ld	a2,0xE8(sp)
	beq	a2,zero,0x0000000000015A4C

l0000000000015A3C:
	lui	a1,0x12
	addi	a1,a1,-0x378
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015A4C:
	slli	a5,s0,0x32
	bge	a5,zero,0x0000000000015A80

l0000000000015A54:
	ld	s2,0xF0(sp)
	beq	s2,zero,0x0000000000015A80

l0000000000015A5C:
	addi	a1,zero,0x20
	addi	a0,s2,0x0
	jal	ra,strchr
	addi	a2,s2,0x0
	beq	a0,zero,0x0000000000015DA4

l0000000000015A70:
	lui	a1,0x12
	addi	a1,a1,-0x368
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015A80:
	slli	a5,s0,0x33
	bge	a5,zero,0x0000000000015AAC

l0000000000015A88:
	addi	a1,zero,0x20
	addi	a0,sp,0x98
	jal	ra,strchr
	addi	a2,sp,0x98
	beq	a0,zero,0x0000000000015D90

l0000000000015A9C:
	lui	a1,0x12
	addi	a1,a1,-0x348
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015AAC:
	andi	a5,s0,0x1
	beq	a5,zero,0x0000000000015ACC

l0000000000015AB4:
	ld	a2,0x70(sp)
	beq	a2,zero,0x0000000000015ACC

l0000000000015ABC:
	lui	a1,0x12
	addi	a1,a1,-0x328
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015ACC:
	andi	a5,s0,0x2
	bne	a5,zero,0x0000000000015D60

l0000000000015AD4:
	lui	a5,0xC
	and	s0,s0,a5
	bne	s0,a5,0x00000000000153DC

l0000000000015AE0:
	ld	a2,0x80(sp)
	beq	a2,zero,0x0000000000015AF8

l0000000000015AE8:
	lui	a1,0x12
	addi	a1,a1,-0x308
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015AF8:
	ld	s0,0x78(sp)
	beq	s0,zero,0x0000000000015B24

l0000000000015B00:
	addi	a1,zero,0x20
	addi	a0,s0,0x0
	jal	ra,strchr
	addi	a2,s0,0x0
	beq	a0,zero,0x0000000000015E90

l0000000000015B14:
	lui	a1,0x12
	addi	a1,a1,-0x2F8
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015B24:
	ld	s0,0x88(sp)
	beq	s0,zero,0x0000000000015B50

l0000000000015B2C:
	addi	a1,zero,0x20
	addi	a0,s0,0x0
	jal	ra,strchr
	addi	a2,s0,0x0
	beq	a0,zero,0x0000000000015EA4

l0000000000015B40:
	lui	a1,0x12
	addi	a1,a1,-0x2D8
	addi	a0,zero,0x1
	jal	ra,__printf_chk

l0000000000015B50:
	ld	a2,0x90(sp)
	beq	a2,zero,0x00000000000153DC

l0000000000015B58:
	lui	a1,0x12
	addi	a1,a1,-0x2B8
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x00000000000153DC

l0000000000015B6C:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x00000000000153D8

l0000000000015B74:
	lw	a5,0x1C(sp)
	ld	a0,-0x780(gp)
	bne	a5,zero,0x0000000000015CD8

l0000000000015B80:
	lui	a3,0x12
	addi	a3,a3,-0x660

l0000000000015B88:
	addi	a4,s4,0x0

l0000000000015B8C:
	lui	a2,0x12
	addi	a2,a2,-0x590
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	jal	zero,0x00000000000153D8

l0000000000015BA0:
	addi	a1,zero,0x2F
	addi	a0,s1,0x0
	sd	a5,(sp)
	jal	ra,strchr
	ld	a5,(sp)
	beq	a0,zero,0x00000000000155FC

l0000000000015BB8:
	sb	zero,(a0)
	addi	s4,a0,0x1
	beq	s4,zero,0x00000000000155FC

l0000000000015BC4:
	jal	zero,0x00000000000155E4

l0000000000015BC8:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x00000000000153D8

l0000000000015BD0:
	lw	a5,0x1C(sp)
	ld	a0,-0x780(gp)
	bne	a5,zero,0x0000000000015E7C

l0000000000015BDC:
	lui	a3,0x12
	addi	a3,a3,-0x660
	addi	a4,s10,0x0
	jal	zero,0x0000000000015B8C

l0000000000015BEC:
	addi	a1,zero,0x2F
	addi	a0,s1,0x0
	sd	a5,(sp)
	jal	ra,strchr
	ld	a5,(sp)
	bne	a0,zero,0x0000000000015BB8

l0000000000015C04:
	jal	zero,0x0000000000015604

l0000000000015C08:
	ld	a5,0x88(sp)
	bne	a5,zero,0x00000000000157C4

l0000000000015C10:
	ld	a5,0x90(sp)
	bne	a5,zero,0x00000000000157C4

l0000000000015C18:
	jal	zero,0x00000000000153DC

l0000000000015C1C:
	ld	a3,0x50(sp)
	beq	a3,zero,0x00000000000156FC

l0000000000015C24:
	addi	s0,zero,0x0
	lui	s2,0x12

l0000000000015C2C:
	lui	a1,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x488
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	bne	s0,zero,0x00000000000158C8

l0000000000015C44:
	jal	zero,0x00000000000156FC

l0000000000015C48:
	addi	a1,zero,0x4
	addi	a0,sp,0x100
	sd	a5,0x8(sp)
	ld	s1,0x30(sp)
	jal	ra,fn00000000000164D0
	ld	a5,0x8(sp)
	blt	a0,zero,0x0000000000015CB0

l0000000000015C64:
	addi	a4,zero,0xF
	bge	a4,a5,0x0000000000015C7C

l0000000000015C6C:
	addi	a4,zero,0x4
	rem	a4,s1,a4
	addi	a3,zero,0x1
	bgeu	a3,a4,0x0000000000015DB8

l0000000000015C7C:
	addi	a4,zero,0xA
	sb	a4,0x100(sp)

l0000000000015C84:
	lw	a4,0x100(sp)
	addi	s1,sp,0x108
	addi	a3,zero,0x40
	addi	a2,s1,0x0
	addi	a1,sp,0x18
	addi	a0,zero,0x2
	sd	a5,0x8(sp)
	sw	a4,0x18(sp)
	jal	ra,inet_ntop
	ld	a5,0x8(sp)
	bne	a0,zero,0x00000000000154DC

l0000000000015CB0:
	lw	a4,-0x7EC(gp)
	bne	a4,zero,0x00000000000153D8

l0000000000015CB8:
	ld	a0,-0x780(gp)
	lui	a2,0x12
	addi	a3,a5,0x0
	addi	a2,a2,-0x568
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	s6,zero,0x1
	jal	zero,0x00000000000153DC

l0000000000015CD8:
	lui	a3,0x12
	addi	a3,a3,-0x668
	jal	zero,0x0000000000015B88

l0000000000015CE4:
	bne	s1,zero,0x0000000000015510

l0000000000015CE8:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x00000000000153D8

l0000000000015CF0:
	ld	a3,-0x780(gp)
	lui	a0,0x12
	addi	a2,zero,0x1C
	addi	a1,zero,0x1
	addi	a0,a0,-0x5D0
	jal	ra,fwrite
	addi	a0,zero,0x1
	jal	ra,fn0000000000015FCC
	addi	s6,zero,0x1
	jal	zero,0x00000000000153DC

l0000000000015D18:
	ld	a2,0x68(sp)
	lui	a1,0x12
	addi	a1,a1,-0x3D8
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x00000000000159D0

l0000000000015D30:
	ld	a2,0x58(sp)
	lui	a1,0x12
	addi	a1,a1,-0x3A8
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015A04

l0000000000015D48:
	lw	a2,0xD8(sp)
	lui	a1,0x12
	addi	a1,a1,-0x3C8
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x00000000000159D8

l0000000000015D60:
	lui	a1,0x12
	addi	a2,s1,0x0
	addi	a1,a1,-0x318
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015AD4

l0000000000015D78:
	ld	a2,0x50(sp)
	lui	a1,0x12
	addi	a1,a1,-0x398
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015A0C

l0000000000015D90:
	lui	a1,0x12
	addi	a1,a1,-0x338
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015AAC

l0000000000015DA4:
	lui	a1,0x12
	addi	a1,a1,-0x358
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015A80

l0000000000015DB8:
	beq	a4,a3,0x0000000000015E00

l0000000000015DBC:
	addi	a4,zero,-0x54
	sb	a4,0x100(sp)
	ld	a4,0x30(sp)
	srai	a4,a4,0x4
	andi	a4,a4,0xF
	ori	a4,a4,0x10
	sb	a4,0x101(sp)
	jal	zero,0x0000000000015C84

l0000000000015DDC:
	ld	a3,0x38(sp)
	lui	s2,0x12
	lui	s1,0x12
	lui	a1,0x12
	addi	a2,s2,-0x3A0
	addi	a1,a1,-0x4F0
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	jal	zero,0x00000000000158B8

l0000000000015E00:
	addi	a4,zero,-0x40
	sb	a4,0x100(sp)
	addi	a4,zero,-0x58
	sb	a4,0x101(sp)
	jal	zero,0x0000000000015C84

l0000000000015E14:
	lui	s2,0x12
	lui	s1,0x12
	lui	a1,0x12
	addi	a2,s2,-0x3A0
	addi	a3,s3,0x0
	addi	a1,a1,-0x4F0
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	lw	s2,0xD8(sp)
	jal	zero,0x0000000000015668

l0000000000015E3C:
	addi	a3,zero,0x80
	lui	a2,0x12
	lui	a1,0x12
	addi	a4,sp,0x98
	subw	a3,a3,a5
	addi	a2,a2,-0x430
	addi	a1,a1,-0x420
	addi	a0,s1,-0x4F8
	jal	ra,fn000000000001654C
	jal	zero,0x00000000000157BC

l0000000000015E64:
	ld	a3,0x50(sp)
	addi	s0,zero,0x1
	bne	a3,zero,0x0000000000015C2C

l0000000000015E70:
	jal	zero,0x00000000000158C8

l0000000000015E74:
	lw	s2,0xD8(sp)
	jal	zero,0x0000000000015668

l0000000000015E7C:
	lui	a3,0x12
	addi	a3,a3,-0x668
	addi	a4,s10,0x0
	jal	zero,0x0000000000015B8C

l0000000000015E8C:
	jal	ra,__stack_chk_fail

l0000000000015E90:
	lui	a1,0x12
	addi	a1,a1,-0x2E8
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015B24

l0000000000015EA4:
	lui	a1,0x12
	addi	a1,a1,-0x2C8
	addi	a0,zero,0x1
	jal	ra,__printf_chk
	jal	zero,0x0000000000015B50

;; fn0000000000015EB8: 0000000000015EB8
fn0000000000015EB8 proc
	auipc	gp,0xFFFFD
	addi	gp,gp,0x590
	addi	a5,a0,0x0
	auipc	a0,0xFFFFF
	addi	a0,a0,0x2BC
	ld	a1,(sp)
	addi	a2,sp,0x8
	andi	sp,sp,-0x10
	auipc	a3,0x2
	addi	a3,a3,0x410
	auipc	a4,0x2
	addi	a4,a4,0x498
	addi	a6,sp,0x0
	jal	zero,__libc_start_main

;; fn0000000000015EF0: 0000000000015EF0
;;   Called from:
;;     0000000000015F78 (in fn0000000000015F64)
fn0000000000015EF0 proc
	lui	a0,0x10
	lui	a5,0x10
	addi	a4,a0,0x2A0
	addi	a5,a5,0x2A7
	sub	a5,a5,a4
	addi	a4,zero,0xE
	bgeu	a4,a5,0x0000000000015F20

l0000000000015F0C:
	lui	t1,0x0
	addi	t1,t1,0x0
	beq	t1,zero,0x0000000000015F20

l0000000000015F18:
	addi	a0,a0,0x2A0
	jalr	zero,t1,0x0

l0000000000015F20:
	jalr	zero,ra,0x0

;; fn0000000000015F24: 0000000000015F24
;;   Called from:
;;     0000000000015FA4 (in fn0000000000015F94)
;;     0000000000015FC8 (in fn0000000000015F94)
fn0000000000015F24 proc
	lui	a0,0x10
	lui	a5,0x10
	addi	a1,a0,0x2A0
	addi	a5,a5,0x2A0
	sub	a5,a5,a1
	srai	a5,a5,0x3
	srli	a1,a5,0x3F
	add	a1,a1,a5
	srai	a1,a1,0x1
	beq	a1,zero,0x0000000000015F60

l0000000000015F4C:
	lui	t1,0x0
	addi	t1,t1,0x0
	beq	t1,zero,0x0000000000015F60

l0000000000015F58:
	addi	a0,a0,0x2A0
	jalr	zero,t1,0x0

l0000000000015F60:
	jalr	zero,ra,0x0

;; fn0000000000015F64: 0000000000015F64
fn0000000000015F64 proc
	addi	sp,sp,-0x10
	sd	s0,(sp)
	lbu	a5,-0x76C(gp)
	sd	ra,0x8(sp)
	bne	a5,zero,0x0000000000015F84

l0000000000015F78:
	jal	ra,fn0000000000015EF0
	addi	a5,zero,0x1
	sb	a5,-0x76C(gp)

l0000000000015F84:
	ld	ra,0x8(sp)
	ld	s0,(sp)
	addi	sp,sp,0x10
	jalr	zero,ra,0x0

;; fn0000000000015F94: 0000000000015F94
fn0000000000015F94 proc
	lui	a5,0x1A
	addi	a0,a5,-0x1D8
	ld	a5,(a0)
	bne	a5,zero,0x0000000000015FA8

l0000000000015FA4:
	jal	zero,fn0000000000015F24

l0000000000015FA8:
	lui	a5,0x0
	addi	a5,a5,0x0
	beq	a5,zero,0x0000000000015FA4

l0000000000015FB4:
	addi	sp,sp,-0x10
	sd	ra,0x8(sp)
	jalr	ra,a5,0x0
	ld	ra,0x8(sp)
	addi	sp,sp,0x10
	jal	zero,fn0000000000015F24

;; fn0000000000015FCC: 0000000000015FCC
;;   Called from:
;;     0000000000015560 (in fn0000000000015180)
;;     0000000000015D0C (in fn0000000000015180)
fn0000000000015FCC proc
	addi	sp,sp,-0x10
	sd	s0,(sp)
	sd	ra,0x8(sp)
	ld	a3,-0x780(gp)
	bne	a0,zero,0x0000000000016090

l0000000000015FE0:
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,0x408
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x3E
	addi	a1,zero,0x1
	addi	a0,a0,0x458
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x41
	addi	a1,zero,0x1
	addi	a0,a0,0x498
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4C
	addi	a1,zero,0x1
	addi	a0,a0,0x4E0
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x41
	addi	a1,zero,0x1
	addi	a0,a0,0x530
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x29
	addi	a1,zero,0x1
	addi	a0,a0,0x578
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	ld	ra,0x8(sp)
	ld	s0,(sp)
	lui	a0,0x11
	addi	a2,zero,0x1E
	addi	a1,zero,0x1
	addi	a0,a0,0x5A8
	addi	sp,sp,0x10
	jal	zero,fwrite

l0000000000016090:
	lui	a0,0x11
	addi	a2,zero,0x1A
	addi	a1,zero,0x1
	addi	a0,a0,-0x420
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x36
	addi	a1,zero,0x1
	addi	a0,a0,-0x400
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,-0x3C8
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x3E
	addi	a1,zero,0x1
	addi	a0,a0,-0x378
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4F
	addi	a1,zero,0x1
	addi	a0,a0,-0x338
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4F
	addi	a1,zero,0x1
	addi	a0,a0,-0x2E8
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x2A
	addi	a1,zero,0x1
	addi	a0,a0,-0x298
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4C
	addi	a1,zero,0x1
	addi	a0,a0,-0x268
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,-0x218
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,-0x1C8
	jal	ra,fwrite
	ld	a1,-0x780(gp)
	addi	a0,zero,0xA
	jal	ra,fputc
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x17
	addi	a1,zero,0x1
	addi	a0,a0,-0x178
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x47
	addi	a1,zero,0x1
	addi	a0,a0,-0x160
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x39
	addi	a1,zero,0x1
	addi	a0,a0,-0x118
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x3A
	addi	a1,zero,0x1
	addi	a0,a0,-0xD8
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x39
	addi	a1,zero,0x1
	addi	a0,a0,-0x98
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,-0x58
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,-0x8
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x4D
	addi	a1,zero,0x1
	addi	a0,a0,0x48
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x2E
	addi	a1,zero,0x1
	addi	a0,a0,0x98
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x48
	addi	a1,zero,0x1
	addi	a0,a0,0xC8
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x2D
	addi	a1,zero,0x1
	addi	a0,a0,0x118
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x43
	addi	a1,zero,0x1
	addi	a0,a0,0x148
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x40
	addi	a1,zero,0x1
	addi	a0,a0,0x190
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x48
	addi	a1,zero,0x1
	addi	a0,a0,0x1D8
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x2E
	addi	a1,zero,0x1
	addi	a0,a0,0x228
	jal	ra,fwrite
	ld	a1,-0x780(gp)
	addi	a0,zero,0xA
	jal	ra,fputc
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0xF
	addi	a1,zero,0x1
	addi	a0,a0,0x258
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x57
	addi	a1,zero,0x1
	addi	a0,a0,0x268
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x3C
	addi	a1,zero,0x1
	addi	a0,a0,0x2C0
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x44
	addi	a1,zero,0x1
	addi	a0,a0,0x300
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x3A
	addi	a1,zero,0x1
	addi	a0,a0,0x348
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x39
	addi	a1,zero,0x1
	addi	a0,a0,0x388
	jal	ra,fwrite
	ld	a3,-0x780(gp)
	ld	ra,0x8(sp)
	ld	s0,(sp)
	lui	a0,0x11
	addi	a2,zero,0x3E
	addi	a1,zero,0x1
	addi	a0,a0,0x3C8
	addi	sp,sp,0x10
	jal	zero,fwrite

;; fn00000000000163B0: 00000000000163B0
;;   Called from:
;;     000000000001762C (in fn00000000000175BC)
;;     00000000000176E4 (in fn00000000000175BC)
fn00000000000163B0 proc
	addi	sp,sp,-0xF0
	sd	s0,0xE0(sp)
	sd	s1,0xD8(sp)
	sd	s2,0xD0(sp)
	sd	s3,0xC8(sp)
	sd	s4,0xC0(sp)
	sd	s5,0xB8(sp)
	sd	s6,0xB0(sp)
	sd	s7,0xA8(sp)
	sd	s8,0xA0(sp)
	sd	s9,0x98(sp)
	sd	ra,0xE8(sp)
	ld	a5,-0x790(gp)
	lbu	a4,(a0)
	lui	s4,0x11
	addi	s0,zero,0x0
	addi	s2,a0,0x0
	addi	a3,s4,0x5C8
	addi	a2,zero,0x80
	addi	a1,zero,0x1
	addi	a0,sp,0x8
	addi	s3,zero,0x10
	addiw	s0,s0,0x1
	sd	a5,0x88(sp)
	addi	s2,s2,0x1
	jal	ra,__sprintf_chk
	addi	s1,sp,0xA
	addi	s6,zero,0x80
	addi	s5,zero,0x1
	addi	s9,zero,0xF
	addi	s7,zero,0x3A
	beq	s0,s3,0x000000000001646C

l0000000000016430:
	lbu	a4,(s2)
	addi	a3,s4,0x5C8
	addi	a2,s6,0x0
	addi	a1,s5,0x0
	addi	a0,s1,0x0
	jal	ra,__sprintf_chk
	andi	a5,s0,0x1
	addi	a4,s1,0x2
	beq	a5,zero,0x00000000000164B8

l0000000000016454:
	beq	s0,s9,0x00000000000164C4

l0000000000016458:
	sb	s7,0x2(s1)
	addi	s2,s2,0x1
	addi	s1,s1,0x3

l0000000000016464:
	addiw	s0,s0,0x1
	bne	s0,s3,0x0000000000016430

l000000000001646C:
	sb	zero,(s1)
	addi	a0,sp,0x8
	jal	ra,__strdup
	ld	a4,0x88(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x00000000000164CC

l0000000000016484:
	ld	ra,0xE8(sp)
	ld	s0,0xE0(sp)
	ld	s1,0xD8(sp)
	ld	s2,0xD0(sp)
	ld	s3,0xC8(sp)
	ld	s4,0xC0(sp)
	ld	s5,0xB8(sp)
	ld	s6,0xB0(sp)
	ld	s7,0xA8(sp)
	ld	s8,0xA0(sp)
	ld	s9,0x98(sp)
	addi	sp,sp,0xF0
	jalr	zero,ra,0x0

l00000000000164B8:
	addi	s1,a4,0x0
	addi	s2,s2,0x1
	jal	zero,0x0000000000016464

l00000000000164C4:
	addi	s1,a4,0x0
	jal	zero,0x000000000001646C

l00000000000164CC:
	jal	ra,__stack_chk_fail

;; fn00000000000164D0: 00000000000164D0
;;   Called from:
;;     00000000000154B0 (in fn0000000000015180)
;;     0000000000015C58 (in fn0000000000015180)
fn00000000000164D0 proc
	addi	sp,sp,-0x20
	sd	s0,0x10(sp)
	addi	s0,a0,0x0
	lui	a0,0x11
	sd	s2,(sp)
	addi	a0,a0,0x5D0
	addi	s2,a1,0x0
	addi	a1,zero,0x0
	sd	ra,0x18(sp)
	sd	s1,0x8(sp)
	jal	ra,open
	blt	a0,zero,0x0000000000016544

l0000000000016500:
	slli	a2,s2,0x20
	addi	a1,s0,0x0
	srli	a2,a2,0x20
	addi	s1,a0,0x0
	jal	ra,read
	addi	s0,a0,0x0
	addi	a0,s1,0x0
	jal	ra,close
	subw	a0,s0,s2
	sltu	a0,zero,a0
	subw	a0,zero,a0

l000000000001652C:
	ld	ra,0x18(sp)
	ld	s0,0x10(sp)
	ld	s1,0x8(sp)
	ld	s2,(sp)
	addi	sp,sp,0x20
	jalr	zero,ra,0x0

l0000000000016544:
	addi	a0,zero,-0x1
	jal	zero,0x000000000001652C

;; fn000000000001654C: 000000000001654C
;;   Called from:
;;     0000000000015688 (in fn0000000000015180)
;;     00000000000156A8 (in fn0000000000015180)
;;     00000000000156C8 (in fn0000000000015180)
;;     00000000000156E8 (in fn0000000000015180)
;;     0000000000015724 (in fn0000000000015180)
;;     0000000000015748 (in fn0000000000015180)
;;     0000000000015768 (in fn0000000000015180)
;;     0000000000015788 (in fn0000000000015180)
;;     00000000000157B8 (in fn0000000000015180)
;;     00000000000157EC (in fn0000000000015180)
;;     0000000000015810 (in fn0000000000015180)
;;     0000000000015834 (in fn0000000000015180)
;;     0000000000015858 (in fn0000000000015180)
;;     0000000000015898 (in fn0000000000015180)
;;     00000000000158B0 (in fn0000000000015180)
;;     00000000000158E4 (in fn0000000000015180)
;;     0000000000015904 (in fn0000000000015180)
;;     0000000000015C3C (in fn0000000000015180)
;;     0000000000015DF8 (in fn0000000000015180)
;;     0000000000015E30 (in fn0000000000015180)
;;     0000000000015E5C (in fn0000000000015180)
fn000000000001654C proc
	addi	sp,sp,-0x80
	sd	s0,0x40(sp)
	ld	t3,-0x790(gp)
	addi	t1,sp,0x58
	sd	s2,0x30(sp)
	sd	s3,0x28(sp)
	addi	s2,a1,0x0
	addi	s3,a0,0x0
	sd	a3,0x58(sp)
	addi	a1,zero,0x1
	addi	a3,t1,0x0
	addi	a0,sp,0x10
	sd	ra,0x48(sp)
	sd	s1,0x38(sp)
	sd	a4,0x60(sp)
	sd	a5,0x68(sp)
	sd	a6,0x70(sp)
	sd	a7,0x78(sp)
	sd	t3,0x18(sp)
	sd	zero,0x10(sp)
	sd	t1,0x8(sp)
	jal	ra,__vasprintf_chk
	blt	a0,zero,0x00000000000165D8

l00000000000165A8:
	ld	a1,-0x778(gp)
	addi	a0,s2,0x0
	jal	ra,fputs
	lw	a5,-0x7F0(gp)
	bne	a5,zero,0x000000000001661C

l00000000000165BC:
	ld	a1,-0x778(gp)
	ld	a0,0x10(sp)
	jal	ra,fputs
	lw	a5,-0x7F0(gp)
	bne	a5,zero,0x0000000000016600

l00000000000165D0:
	ld	a0,0x10(sp)
	jal	ra,free

l00000000000165D8:
	ld	a4,0x18(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x000000000001662C

l00000000000165E4:
	ld	ra,0x48(sp)
	ld	s0,0x40(sp)
	ld	s1,0x38(sp)
	ld	s2,0x30(sp)
	ld	s3,0x28(sp)
	addi	sp,sp,0x80
	jalr	zero,ra,0x0

l0000000000016600:
	ld	a3,-0x778(gp)
	lui	a0,0x11
	addi	a2,zero,0x4
	addi	a1,zero,0x1
	addi	a0,a0,0x5E0
	jal	ra,fwrite
	jal	zero,0x00000000000165D0

l000000000001661C:
	ld	a1,-0x778(gp)
	addi	a0,s3,0x0
	jal	ra,fputs
	jal	zero,0x00000000000165BC

l000000000001662C:
	jal	ra,__stack_chk_fail

;; fn0000000000016630: 0000000000016630
;;   Called from:
;;     00000000000167D0 (in fn00000000000166F4)
fn0000000000016630 proc
	addi	sp,sp,-0x40
	sd	s0,0x30(sp)
	ld	a5,-0x790(gp)
	sd	ra,0x38(sp)
	sd	s1,0x28(sp)
	sd	a5,0x8(sp)
	sd	s2,0x20(sp)
	sd	s3,0x18(sp)
	addi	s2,a0,0x0
	addi	s3,a1,0x0
	sd	zero,(sp)
	jal	ra,__errno_location
	sw	zero,(a0)
	addi	s1,a0,0x0
	addi	a2,zero,0x0
	addi	a1,sp,0x0
	addi	a0,s2,0x0
	jal	ra,strtol
	ld	a5,(sp)
	beq	a5,zero,0x000000000001668C

l0000000000016680:
	beq	s2,a5,0x000000000001668C

l0000000000016684:
	lbu	a5,(a5)
	beq	a5,zero,0x00000000000166C4

l000000000001668C:
	lw	a5,(s1)

l0000000000016690:
	bge	zero,a5,0x00000000000166E8

l0000000000016694:
	subw	a5,zero,a5

l0000000000016698:
	ld	a3,0x8(sp)
	ld	a4,-0x790(gp)
	addi	a0,a5,0x0
	bne	a3,a4,0x00000000000166F0

l00000000000166A8:
	ld	ra,0x38(sp)
	ld	s0,0x30(sp)
	ld	s1,0x28(sp)
	ld	s2,0x20(sp)
	ld	s3,0x18(sp)
	addi	sp,sp,0x40
	jalr	zero,ra,0x0

l00000000000166C4:
	lw	a5,(s1)
	bne	a5,zero,0x0000000000016690

l00000000000166CC:
	addiw	a4,a0,0x0
	addi	a3,a4,0x0
	bne	a0,a3,0x00000000000166E0

l00000000000166D8:
	sw	a4,(s3)
	jal	zero,0x0000000000016698

l00000000000166E0:
	addi	a5,zero,-0x22
	jal	zero,0x0000000000016698

l00000000000166E8:
	addi	a5,zero,-0x16
	jal	zero,0x0000000000016698

l00000000000166F0:
	jal	ra,__stack_chk_fail

;; fn00000000000166F4: 00000000000166F4
;;   Called from:
;;     0000000000015468 (in fn0000000000015180)
;;     00000000000155F0 (in fn0000000000015180)
fn00000000000166F4 proc
	addi	sp,sp,-0x40
	sd	s2,0x20(sp)
	ld	a5,-0x790(gp)
	sd	s0,0x30(sp)
	lw	s0,(a0)
	sd	s1,0x28(sp)
	sd	s3,0x18(sp)
	sd	s4,0x10(sp)
	sd	ra,0x38(sp)
	sd	a5,0x8(sp)
	addi	s1,a0,0x0
	addi	s3,a1,0x0
	addi	s4,a2,0x0
	bne	s0,zero,0x00000000000167C8

l000000000001672C:
	addi	a1,zero,0x2E
	addi	a0,s3,0x0
	jal	ra,strchr
	beq	a0,zero,0x00000000000167C8

l000000000001673C:
	addi	a2,sp,0x0
	addi	a1,s3,0x0
	addi	a0,zero,0x2
	jal	ra,inet_pton
	beq	a0,zero,0x000000000001678C

l0000000000016750:
	lw	a0,(sp)
	jal	ra,fn00000000000182B4
	beq	a0,zero,0x0000000000016790

l000000000001675C:
	andi	a5,a0,0x1
	bne	a5,zero,0x0000000000016778

l0000000000016764:
	srliw	a5,a0,0x1
	beq	a5,zero,0x0000000000016790

l000000000001676C:
	andi	a4,a5,0x1
	beq	a4,zero,0x0000000000016820

l0000000000016774:
	addi	a0,a5,0x0

l0000000000016778:
	srliw	a0,a0,0x1
	addiw	s0,s0,0x1
	beq	a0,zero,0x0000000000016790

l0000000000016784:
	andi	a5,a0,0x1
	bne	a5,zero,0x0000000000016778

l000000000001678C:
	addi	s0,zero,-0x1

l0000000000016790:
	sw	s0,(sp)

l0000000000016794:
	lw	a5,(sp)
	beq	s4,zero,0x00000000000167DC

l000000000001679C:
	addi	a4,zero,0x20
	addi	a0,a5,0x0
	bge	a4,a5,0x00000000000167DC

l00000000000167A8:
	lw	a4,(s1)
	bne	a4,zero,0x00000000000167B8

l00000000000167B0:
	addi	a4,zero,0x1
	sw	a4,(s1)

l00000000000167B8:
	addi	a4,zero,0x80
	bge	a4,a5,0x00000000000167F4

l00000000000167C0:
	addi	a0,zero,-0x1
	jal	zero,0x00000000000167F4

l00000000000167C8:
	addi	a1,sp,0x0
	addi	a0,s3,0x0
	jal	ra,fn0000000000016630
	beq	a0,zero,0x0000000000016794

l00000000000167D8:
	jal	zero,0x00000000000167C0

l00000000000167DC:
	addi	a0,a5,0x0
	blt	a5,zero,0x00000000000167C0

l00000000000167E4:
	lw	a4,(s1)
	bne	a4,zero,0x00000000000167B8

l00000000000167EC:
	addi	a5,zero,0x20
	blt	a5,a0,0x00000000000167C0

l00000000000167F4:
	ld	a4,0x8(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x000000000001682C

l0000000000016800:
	ld	ra,0x38(sp)
	ld	s0,0x30(sp)
	ld	s1,0x28(sp)
	ld	s2,0x20(sp)
	ld	s3,0x18(sp)
	ld	s4,0x10(sp)
	addi	sp,sp,0x40
	jalr	zero,ra,0x0

l0000000000016820:
	srliw	a0,a0,0x2
	bne	a0,zero,0x000000000001675C

l0000000000016828:
	jal	zero,0x0000000000016790

l000000000001682C:
	jal	ra,__stack_chk_fail

;; fn0000000000016830: 0000000000016830
;;   Called from:
;;     00000000000168CC (in fn00000000000168B8)
;;     0000000000016EE4 (in fn0000000000016D74)
;;     0000000000016F3C (in fn0000000000016D74)
;;     0000000000016F98 (in fn0000000000016D74)
fn0000000000016830 proc
	addi	sp,sp,-0x20
	sd	s0,0x10(sp)
	ld	a5,-0x790(gp)
	sd	ra,0x18(sp)
	sw	zero,(sp)
	sd	a5,0x8(sp)
	beq	a0,zero,0x0000000000016864

l000000000001684C:
	addi	a5,zero,0x20
	subw	a0,a5,a0
	addi	a5,zero,-0x1
	sllw	a0,a5,a0
	jal	ra,fn00000000000182B4
	sw	a0,(sp)

l0000000000016864:
	ld	a4,0x8(sp)
	ld	a5,-0x790(gp)
	lw	a0,(sp)
	bne	a4,a5,0x0000000000016884

l0000000000016874:
	ld	ra,0x18(sp)
	ld	s0,0x10(sp)
	addi	sp,sp,0x20
	jalr	zero,ra,0x0

l0000000000016884:
	jal	ra,__stack_chk_fail
0000000000016888                         13 01 01 FF 23 30 81 00         ....#0..
0000000000016890 13 04 05 00 13 85 05 00 23 34 11 00 EF F0 5F F9 ........#4...._.
00000000000168A0 83 30 81 00 13 45 F5 FF 33 65 85 00 03 34 01 00 .0...E..3e...4..
00000000000168B0 13 01 01 01 67 80 00 00                         ....g...        

;; fn00000000000168B8: 00000000000168B8
fn00000000000168B8 proc
	addi	sp,sp,-0x10
	sd	s0,(sp)
	addi	s0,a0,0x0
	addi	a0,a1,0x0
	sd	ra,0x8(sp)
	jal	ra,fn0000000000016830
	ld	ra,0x8(sp)
	and	a0,s0,a0
	ld	s0,(sp)
	addi	sp,sp,0x10
	jalr	zero,ra,0x0

;; fn00000000000168E4: 00000000000168E4
;;   Called from:
;;     00000000000171D0 (in fn0000000000016D74)
;;     000000000001782C (in fn00000000000175BC)
fn00000000000168E4 proc
	addi	sp,sp,-0x60
	sd	s0,0x50(sp)
	ld	a4,-0x790(gp)
	sd	ra,0x58(sp)
	sd	s1,0x48(sp)
	addi	a5,zero,0x2
	sd	a4,0x38(sp)
	addi	a7,a0,0x0
	beq	a0,a5,0x0000000000016984

l0000000000016908:
	addi	a5,zero,0xA
	beq	a0,a5,0x0000000000016934

l0000000000016910:
	addi	a0,zero,0x0

l0000000000016914:
	ld	a4,0x38(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x00000000000169D4

l0000000000016920:
	ld	ra,0x58(sp)
	ld	s0,0x50(sp)
	ld	s1,0x48(sp)
	addi	sp,sp,0x60
	jalr	zero,ra,0x0

l0000000000016934:
	sd	zero,0x18(sp)
	addi	a2,zero,0x10
	addi	a0,sp,0x20
	sd	zero,0x20(sp)
	sd	zero,0x28(sp)
	sw	zero,0x30(sp)
	sh	a7,0x18(sp)
	jal	ra,memcpy
	addi	a6,zero,0x0
	addi	a5,zero,0x0
	addi	a4,zero,0x0
	addi	a3,zero,0x401
	addi	a2,gp,-0x768
	addi	a1,zero,0x1C
	addi	a0,sp,0x18
	jal	ra,getnameinfo

l0000000000016974:
	bne	a0,zero,0x0000000000016910

l0000000000016978:
	addi	a0,gp,-0x768
	jal	ra,__strdup
	jal	zero,0x0000000000016914

l0000000000016984:
	lbu	t5,(a1)
	lbu	t4,0x1(a1)
	lbu	t3,0x2(a1)
	lbu	t1,0x3(a1)
	sd	zero,0x8(sp)
	addi	a6,zero,0x0
	addi	a5,zero,0x0
	addi	a4,zero,0x0
	addi	a3,zero,0x401
	addi	a2,gp,-0x768
	addi	a1,zero,0x10
	addi	a0,sp,0x8
	sd	zero,0x10(sp)
	sh	a7,0x8(sp)
	sb	t5,0xC(sp)
	sb	t4,0xD(sp)
	sb	t3,0xE(sp)
	sb	t1,0xF(sp)
	jal	ra,getnameinfo
	jal	zero,0x0000000000016974

l00000000000169D4:
	jal	ra,__stack_chk_fail

;; fn00000000000169D8: 00000000000169D8
;;   Called from:
;;     00000000000155C4 (in fn0000000000015180)
;;     0000000000015950 (in fn0000000000015180)
;;     0000000000015998 (in fn0000000000015180)
fn00000000000169D8 proc
	addi	sp,sp,-0x70
	sd	s4,0x40(sp)
	ld	a5,-0x790(gp)
	addi	a4,a1,0x0
	sd	zero,0x8(sp)
	sw	a0,0xC(sp)
	addi	a3,sp,0x0
	addi	a2,sp,0x8
	addi	a1,zero,0x0
	addi	a0,a4,0x0
	sd	ra,0x68(sp)
	sd	s0,0x60(sp)
	sd	s1,0x58(sp)
	sd	s2,0x50(sp)
	sd	s3,0x48(sp)
	sd	a5,0x38(sp)
	sd	zero,0x10(sp)
	sd	zero,0x18(sp)
	sd	zero,0x20(sp)
	sd	zero,0x28(sp)
	sd	zero,0x30(sp)
	jal	ra,getaddrinfo
	bne	a0,zero,0x0000000000016AD8

l0000000000016A34:
	ld	s0,(sp)
	beq	s0,zero,0x0000000000016AA0

l0000000000016A3C:
	addi	s1,gp,-0x360
	addi	s3,zero,0x2
	addi	s2,zero,0x40
	jal	zero,0x0000000000016A68

l0000000000016A4C:
	addi	a1,a1,0x8
	addi	a3,s2,0x0
	addi	a2,s1,0x0
	jal	ra,inet_ntop
	bne	a0,zero,0x0000000000016A88

l0000000000016A60:
	ld	s0,0x28(s0)
	beq	s0,zero,0x0000000000016A9C

l0000000000016A68:
	lw	a0,0x4(s0)
	ld	a1,0x18(s0)
	bne	a0,s3,0x0000000000016A4C

l0000000000016A74:
	addi	a1,a1,0x4
	addi	a3,s2,0x0
	addi	a2,s1,0x0
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000016A60

l0000000000016A88:
	ld	a0,(sp)
	jal	ra,freeaddrinfo
	addi	a0,s1,0x0
	jal	ra,__strdup
	jal	zero,0x0000000000016AAC

l0000000000016A9C:
	ld	s0,(sp)

l0000000000016AA0:
	addi	a0,s0,0x0
	jal	ra,freeaddrinfo
	addi	a0,zero,0x0

l0000000000016AAC:
	ld	a4,0x38(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x0000000000016AE0

l0000000000016AB8:
	ld	ra,0x68(sp)
	ld	s0,0x60(sp)
	ld	s1,0x58(sp)
	ld	s2,0x50(sp)
	ld	s3,0x48(sp)
	ld	s4,0x40(sp)
	addi	sp,sp,0x70
	jalr	zero,ra,0x0

l0000000000016AD8:
	addi	a0,zero,0x0
	jal	zero,0x0000000000016AAC

l0000000000016AE0:
	jal	ra,__stack_chk_fail
0000000000016AE4             93 07 05 00 63 84 07 04 13 F7 17 00     ....c.......
0000000000016AF0 63 02 07 02 13 05 00 00 9B D7 17 00 13 F7 17 00 c...............
0000000000016B00 1B 05 15 00 63 88 07 02 E3 18 07 FE 13 05 F0 FF ....c...........
0000000000016B10 67 80 00 00 1B D7 17 00 93 76 17 00 9B D7 27 00 g........v....'.
0000000000016B20 63 08 07 00 E3 82 06 FC 93 07 07 00 6F F0 9F FC c...........o...
0000000000016B30 13 05 00 00 67 80 00 00                         ....g...        

;; fn0000000000016B38: 0000000000016B38
fn0000000000016B38 proc
	addi	a5,zero,0x7F
	bltu	a5,a0,0x0000000000016B5C

l0000000000016B40:
	slli	a0,a0,0x20
	lui	a5,0x10
	addi	a5,a5,0x480
	srli	a0,a0,0x1D
	add	a0,a5,a0
	ld	a0,(a0)
	jalr	zero,ra,0x0

l0000000000016B5C:
	lui	a0,0x11
	addi	a0,a0,0x1D0
	jalr	zero,ra,0x0

;; fn0000000000016B68: 0000000000016B68
;;   Called from:
;;     000000000001700C (in fn0000000000016D74)
fn0000000000016B68 proc
	addi	sp,sp,-0x10
	sd	ra,0x8(sp)
	jal	ra,fn00000000000182B4
	srliw	a5,a0,0x18
	beq	a5,zero,0x0000000000016BD8

l0000000000016B7C:
	addi	a4,zero,0xA
	beq	a5,a4,0x0000000000016C2C

l0000000000016B84:
	addi	a3,zero,0x64
	srliw	a4,a0,0x10
	beq	a5,a3,0x0000000000016C58

l0000000000016B90:
	addi	a3,zero,0x7F
	beq	a5,a3,0x0000000000016C70

l0000000000016B98:
	srliw	a2,a0,0x8
	addi	a1,zero,0xA9
	andi	a3,a4,0xFF
	andi	a6,a2,0xFF
	beq	a5,a1,0x0000000000016C44

l0000000000016BAC:
	addi	a1,zero,0xAC
	bne	a5,a1,0x0000000000016BF0

l0000000000016BB4:
	andi	a4,a4,0xF0
	addi	a3,zero,0x10
	beq	a4,a3,0x0000000000016C2C

l0000000000016BC0:
	addiw	a4,a5,-0xE0
	addi	a3,zero,0xF
	bltu	a3,a4,0x0000000000016CB8

l0000000000016BCC:
	lui	a4,0x11
	addi	a4,a4,0x748
	jal	zero,0x0000000000016BE0

l0000000000016BD8:
	lui	a4,0x11
	addi	a4,a4,0x5E8

l0000000000016BE0:
	ld	ra,0x8(sp)
	addi	a0,a4,0x0
	addi	sp,sp,0x10
	jalr	zero,ra,0x0

l0000000000016BF0:
	addi	a1,zero,0xC0
	bne	a5,a1,0x0000000000016C7C

l0000000000016BF8:
	or	a4,a4,a2
	andi	a4,a4,0xFF
	beq	a4,zero,0x0000000000016CD0

l0000000000016C04:
	addi	a4,zero,0x2
	beq	a3,a4,0x0000000000016CDC

l0000000000016C0C:
	addi	a4,zero,0x33
	beq	a3,a4,0x0000000000016C9C

l0000000000016C14:
	addi	a4,zero,0x58
	beq	a3,a4,0x0000000000016CB0

l0000000000016C1C:
	addi	a4,zero,0x34
	beq	a3,a4,0x0000000000016CEC

l0000000000016C24:
	addi	a4,zero,0xA8
	bne	a3,a4,0x0000000000016BC0

l0000000000016C2C:
	ld	ra,0x8(sp)
	lui	a4,0x11
	addi	a4,a4,0x608
	addi	a0,a4,0x0
	addi	sp,sp,0x10
	jalr	zero,ra,0x0

l0000000000016C44:
	addi	a4,zero,0xFE
	bne	a3,a4,0x0000000000016BC0

l0000000000016C4C:
	lui	a4,0x11
	addi	a4,a4,0x640
	jal	zero,0x0000000000016BE0

l0000000000016C58:
	andi	a4,a4,0xC0
	addi	a3,zero,0x40
	bne	a4,a3,0x0000000000016BC0

l0000000000016C64:
	lui	a4,0x11
	addi	a4,a4,0x618
	jal	zero,0x0000000000016BE0

l0000000000016C70:
	lui	a4,0x11
	addi	a4,a4,0x630
	jal	zero,0x0000000000016BE0

l0000000000016C7C:
	addi	a2,zero,0xCB
	bne	a5,a2,0x0000000000016D18

l0000000000016C84:
	bne	a3,zero,0x0000000000016BC0

l0000000000016C88:
	addi	a4,zero,0x71
	bne	a6,a4,0x0000000000016BC0

l0000000000016C90:
	lui	a4,0x11
	addi	a4,a4,0x6C0
	jal	zero,0x0000000000016BE0

l0000000000016C9C:
	addi	a4,zero,0x64
	bne	a6,a4,0x0000000000016BC0

l0000000000016CA4:
	lui	a4,0x11
	addi	a4,a4,0x6A0
	jal	zero,0x0000000000016BE0

l0000000000016CB0:
	addi	a4,zero,0x63
	beq	a6,a4,0x0000000000016D00

l0000000000016CB8:
	andi	a5,a5,0xF0
	addi	a4,zero,0xF0
	beq	a5,a4,0x0000000000016D0C

l0000000000016CC4:
	lui	a4,0x11
	addi	a4,a4,0x650
	jal	zero,0x0000000000016BE0

l0000000000016CD0:
	lui	a4,0x11
	addi	a4,a4,0x660
	jal	zero,0x0000000000016BE0

l0000000000016CDC:
	bne	a6,zero,0x0000000000016BC0

l0000000000016CE0:
	lui	a4,0x11
	addi	a4,a4,0x680
	jal	zero,0x0000000000016BE0

l0000000000016CEC:
	addi	a4,zero,0xC1
	bne	a6,a4,0x0000000000016BC0

l0000000000016CF4:
	lui	a4,0x11
	addi	a4,a4,0x708
	jal	zero,0x0000000000016BE0

l0000000000016D00:
	lui	a4,0x11
	addi	a4,a4,0x6E0
	jal	zero,0x0000000000016BE0

l0000000000016D0C:
	lui	a4,0x11
	addi	a4,a4,0x710
	jal	zero,0x0000000000016BE0

l0000000000016D18:
	addi	a2,zero,0xFF
	bne	a5,a2,0x0000000000016D4C

l0000000000016D20:
	lui	a4,0x11
	addi	a4,a4,0x710
	bne	a3,a5,0x0000000000016BE0

l0000000000016D2C:
	lui	a4,0x11
	addi	a4,a4,0x710
	bne	a6,a2,0x0000000000016BE0

l0000000000016D38:
	and	a0,a0,a2
	bne	a0,a6,0x0000000000016BE0

l0000000000016D40:
	lui	a4,0x11
	addi	a4,a4,0x720
	jal	zero,0x0000000000016BE0

l0000000000016D4C:
	addi	a3,zero,0xC6
	bne	a5,a3,0x0000000000016BC0

l0000000000016D54:
	andi	a5,a4,0xFE
	addi	a3,zero,0x12
	lui	a4,0x11
	addi	a4,a4,0x650
	bne	a5,a3,0x0000000000016BE0

l0000000000016D68:
	lui	a4,0x11
	addi	a4,a4,0x738
	jal	zero,0x0000000000016BE0

;; fn0000000000016D74: 0000000000016D74
;;   Called from:
;;     00000000000159C0 (in fn0000000000015180)
fn0000000000016D74 proc
	addi	sp,sp,-0x1C0
	sd	s7,0x178(sp)
	ld	a5,-0x790(gp)
	sd	s6,0x180(sp)
	addi	s6,a2,0x0
	sd	s1,0x1A8(sp)
	sd	s5,0x188(sp)
	addi	s1,a0,0x0
	addi	a2,zero,0xC8
	addi	s5,a1,0x0
	addi	a0,s6,0x0
	addi	a1,zero,0x0
	sd	s8,0x170(sp)
	sd	ra,0x1B8(sp)
	sd	s0,0x1B0(sp)
	sd	s2,0x1A0(sp)
	sd	s3,0x198(sp)
	sd	s4,0x190(sp)
	addi	s8,a3,0x0
	sd	a5,0x168(sp)
	jal	ra,memset
	addi	a2,sp,0x8
	addi	a1,s1,0x0
	addi	a0,zero,0x2
	jal	ra,inet_pton
	bge	zero,a0,0x0000000000016E60

l0000000000016DDC:
	blt	s5,zero,0x0000000000016E88

l0000000000016DE0:
	sd	s1,0x30(sp)
	addi	a0,s1,0x0
	addi	s0,zero,0x3
	addi	s2,zero,0x2E

l0000000000016DF0:
	addi	a1,s2,0x0
	jal	ra,strchr
	sd	a0,0x30(sp)
	beq	a0,zero,0x0000000000017194

l0000000000016E00:
	addi	a0,a0,0x1
	sd	a0,0x30(sp)
	addiw	s0,s0,-0x1
	bne	s0,zero,0x0000000000016DF0

l0000000000016E10:
	addi	a5,zero,0x20
	bge	a5,s5,0x0000000000016EBC

l0000000000016E18:
	lw	a5,-0x7EC(gp)
	beq	a5,zero,0x0000000000017054

l0000000000016E20:
	addi	a0,zero,-0x1

l0000000000016E24:
	ld	a4,0x168(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x00000000000172D8

l0000000000016E30:
	ld	ra,0x1B8(sp)
	ld	s0,0x1B0(sp)
	ld	s1,0x1A8(sp)
	ld	s2,0x1A0(sp)
	ld	s3,0x198(sp)
	ld	s4,0x190(sp)
	ld	s5,0x188(sp)
	ld	s6,0x180(sp)
	ld	s7,0x178(sp)
	ld	s8,0x170(sp)
	addi	sp,sp,0x1C0
	jalr	zero,ra,0x0

l0000000000016E60:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000016E20

l0000000000016E68:
	ld	a0,-0x780(gp)
	lui	a2,0x11
	addi	a3,s1,0x0
	addi	a2,a2,0x788
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	a0,zero,-0x1
	jal	zero,0x0000000000016E24

l0000000000016E88:
	slli	a5,s8,0x2D
	addi	s5,zero,0x20
	bge	a5,zero,0x0000000000016EBC

l0000000000016E94:
	lw	a0,0x8(sp)
	addi	s5,zero,0x8
	jal	ra,fn00000000000182B4
	srliw	a5,a0,0x18
	bge	a0,zero,0x0000000000016EBC

l0000000000016EA8:
	addiw	a5,a5,-0x80
	addi	a4,zero,0x3F
	addi	s5,zero,0x18
	bltu	a4,a5,0x0000000000016EBC

l0000000000016EB8:
	addi	s5,zero,0x10

l0000000000016EBC:
	addi	a3,zero,0x2F
	addi	a2,sp,0x38
	addi	a1,sp,0x8
	addi	a0,zero,0x2
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017074

l0000000000016ED4:
	addi	a0,sp,0x38
	jal	ra,__strdup
	sd	a0,(s6)
	addi	a0,s5,0x0
	jal	ra,fn0000000000016830
	sw	a0,0x10(sp)
	addi	a3,zero,0x10
	addi	a2,sp,0x38
	addi	a1,sp,0x10
	addi	a0,zero,0x2
	sd	zero,0x38(sp)
	sd	zero,0x40(sp)
	sd	zero,0x48(sp)
	sd	zero,0x50(sp)
	sd	zero,0x58(sp)
	sw	zero,0x60(sp)
	sh	zero,0x64(sp)
	sb	zero,0x66(sp)
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017300

l0000000000016F24:
	addi	a0,sp,0x38
	jal	ra,__strdup
	sd	a0,0x30(s6)
	lw	s0,0x8(sp)
	sw	s5,0xA0(s6)
	addi	a0,s5,0x0
	jal	ra,fn0000000000016830
	xori	a5,a0,-0x1
	or	a5,a5,s0
	addi	a3,zero,0x10
	addi	a2,sp,0x38
	addi	a1,sp,0x20
	addi	a0,zero,0x2
	sw	a5,0x20(sp)
	sd	zero,0x38(sp)
	sd	zero,0x40(sp)
	sd	zero,0x48(sp)
	sd	zero,0x50(sp)
	sd	zero,0x58(sp)
	sw	zero,0x60(sp)
	sh	zero,0x64(sp)
	sb	zero,0x66(sp)
	jal	ra,inet_ntop
	beq	a0,zero,0x00000000000172F4

l0000000000016F84:
	addi	a0,sp,0x38
	jal	ra,__strdup
	sd	a0,0x28(s6)
	lw	s0,0x8(sp)
	addi	a0,s5,0x0
	jal	ra,fn0000000000016830
	lw	a3,0x20(sp)
	and	s0,a0,s0
	addi	a2,s0,0x0
	addi	a1,s5,0x0
	addi	a0,s0,0x0
	sw	s0,0x18(sp)
	jal	ra,fn0000000000017F04
	sd	a0,0x18(s6)
	addi	a3,zero,0x10
	addi	a2,sp,0x38
	addi	a1,sp,0x18
	addi	a0,zero,0x2
	sd	zero,0x38(sp)
	sd	zero,0x40(sp)
	sd	zero,0x48(sp)
	sd	zero,0x50(sp)
	sd	zero,0x58(sp)
	sw	zero,0x60(sp)
	sh	zero,0x64(sp)
	sb	zero,0x66(sp)
	jal	ra,inet_ntop
	beq	a0,zero,0x00000000000172DC

l0000000000016FF4:
	addi	a0,sp,0x38
	jal	ra,__strdup
	lw	s0,0x18(sp)
	sd	a0,0x20(s6)
	addi	s3,a0,0x0
	addi	a0,s0,0x0
	jal	ra,fn0000000000016B68
	sd	a0,0xB8(s6)
	addi	a0,s0,0x0
	jal	ra,fn00000000000182B4
	srliw	a5,a0,0x18
	bge	a0,zero,0x0000000000017098

l0000000000017024:
	addiw	a4,a5,-0x80
	addi	a3,zero,0x3F
	bgeu	a3,a4,0x000000000001727C

l0000000000017030:
	addiw	a4,a5,-0xC0
	addi	a3,zero,0x1F
	bgeu	a3,a4,0x00000000000172A4

l000000000001703C:
	addiw	a5,a5,-0xE0
	addi	a4,zero,0xE
	bltu	a4,a5,0x00000000000172B0

l0000000000017048:
	lui	a5,0x11
	addi	a5,a5,0x770
	jal	zero,0x00000000000170A0

l0000000000017054:
	ld	a0,-0x780(gp)
	lui	a2,0x11
	addi	a3,s5,0x0
	addi	a2,a2,0x7D8
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	a0,zero,-0x1
	jal	zero,0x0000000000016E24

l0000000000017074:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000016E20

l000000000001707C:
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x2B
	addi	a1,zero,0x1
	addi	a0,a0,0x7F8
	jal	ra,fwrite
	jal	zero,0x0000000000016E20

l0000000000017098:
	lui	a5,0x11
	addi	a5,a5,0x758

l00000000000170A0:
	sd	a5,0xC0(s6)
	addi	s2,zero,0x20
	beq	s5,s2,0x0000000000017158

l00000000000170AC:
	sw	s0,0x28(sp)
	addi	a5,zero,0x1F
	beq	s5,a5,0x0000000000017210

l00000000000170B8:
	lui	a5,0x1000
	or	s0,s0,a5
	addi	a3,zero,0x10
	addi	a2,sp,0x38
	addi	a1,sp,0x28
	addi	a0,zero,0x2
	sw	s0,0x28(sp)
	jal	ra,inet_ntop
	beq	a0,zero,0x00000000000172E8

l00000000000170DC:
	addi	a0,sp,0x38
	jal	ra,__strdup
	lw	a5,0x10(sp)
	sd	a0,0xA8(s6)
	lw	a0,0x18(sp)
	xori	a5,a5,-0x1
	or	a0,a5,a0
	jal	ra,fn00000000000182B4
	addiw	a0,a0,-0x1
	jal	ra,fn00000000000182B4
	sw	a0,0x30(sp)
	addi	a3,zero,0x2F
	addi	a2,sp,0x38
	addi	a1,sp,0x30
	addi	a0,zero,0x2
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017074

l0000000000017120:
	addi	a0,sp,0x38
	jal	ra,__strdup
	addi	a2,zero,0x1
	subw	a5,s2,s5
	sllw	a5,a2,a5
	addi	a3,zero,0x40
	lui	a4,0x12
	sd	a0,0xB0(s6)
	addiw	a5,a5,-0x2
	addi	a4,a4,-0x7D8
	addi	a1,a3,0x0
	addi	a0,s6,0x60
	jal	ra,__snprintf_chk
	jal	zero,0x000000000001717C

l0000000000017158:
	lui	a5,0x11
	sd	s3,0xA8(s6)
	sd	s3,0xB0(s6)
	addi	a4,s6,0x60
	addi	a5,a5,0x780

l000000000001716C:
	lbu	a3,(a5)
	sb	a3,(a4)
	lbu	a5,0x1(a5)
	sb	a5,0x1(a4)

l000000000001717C:
	slli	a5,s8,0x31
	blt	a5,zero,0x0000000000017288

l0000000000017184:
	andi	s8,s8,0x1
	bne	s8,zero,0x00000000000171C8

l000000000001718C:
	addi	a0,zero,0x0
	jal	zero,0x0000000000016E24

l0000000000017194:
	lui	s4,0x11
	addi	s3,zero,0x1
	addi	s2,zero,-0x1

l00000000000171A0:
	addi	a3,s1,0x0
	addi	a2,s4,0x7A8
	addi	a1,s3,0x0
	addi	a0,sp,0x30
	jal	ra,__asprintf_chk
	beq	a0,s2,0x00000000000172BC

l00000000000171B8:
	addiw	s0,s0,-0x1
	ld	s1,0x30(sp)
	bne	s0,zero,0x00000000000171A0

l00000000000171C4:
	jal	zero,0x0000000000016E10

l00000000000171C8:
	addi	a1,sp,0x8
	addi	a0,zero,0x2
	jal	ra,fn00000000000168E4
	sd	a0,0x38(s6)
	bne	a0,zero,0x000000000001718C

l00000000000171DC:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000016E20

l00000000000171E4:
	lui	a3,0x12
	addi	a4,s1,0x0
	addi	a3,a3,-0x7D0
	addi	a2,zero,0xFA
	addi	a1,zero,0x1
	addi	a0,sp,0x68
	jal	ra,__sprintf_chk
	addi	a0,sp,0x68
	jal	ra,herror
	addi	a0,zero,-0x1
	jal	zero,0x0000000000016E24

l0000000000017210:
	addi	a3,zero,0x10
	addi	a2,sp,0x38
	addi	a1,sp,0x28
	addi	a0,zero,0x2
	jal	ra,inet_ntop
	beq	a0,zero,0x00000000000172E8

l0000000000017228:
	addi	a0,sp,0x38
	jal	ra,__strdup
	lw	a5,0x10(sp)
	lw	a4,0x18(sp)
	sd	a0,0xA8(s6)
	xori	a5,a5,-0x1
	or	a5,a5,a4
	addi	a3,zero,0x2F
	addi	a2,sp,0x38
	addi	a1,sp,0x30
	addi	a0,zero,0x2
	sw	a5,0x30(sp)
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017074

l0000000000017260:
	addi	a0,sp,0x38
	jal	ra,__strdup
	lui	a5,0x12
	sd	a0,0xB0(s6)
	addi	a4,s6,0x60
	addi	a5,a5,0x660
	jal	zero,0x000000000001716C

l000000000001727C:
	lui	a5,0x11
	addi	a5,a5,0x760
	jal	zero,0x00000000000170A0

l0000000000017288:
	lw	a0,0x8(sp)
	addi	a4,s6,0x58
	addi	a3,s6,0x50
	addi	a2,s6,0x48
	addi	a1,s6,0x40
	jal	ra,fn0000000000017B40
	jal	zero,0x0000000000017184

l00000000000172A4:
	lui	a5,0x11
	addi	a5,a5,0x768
	jal	zero,0x00000000000170A0

l00000000000172B0:
	lui	a5,0x11
	addi	a5,a5,0x778
	jal	zero,0x00000000000170A0

l00000000000172BC:
	ld	a0,-0x780(gp)
	addi	a3,zero,0x265

l00000000000172C4:
	lui	a2,0x11
	addi	a2,a2,0x7B0
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	jal	ra,abort

l00000000000172D8:
	jal	ra,__stack_chk_fail

l00000000000172DC:
	ld	a0,-0x780(gp)
	addi	a3,zero,0x29B
	jal	zero,0x00000000000172C4

l00000000000172E8:
	ld	a0,-0x780(gp)
	addi	a3,zero,0x2AC
	jal	zero,0x00000000000172C4

l00000000000172F4:
	ld	a0,-0x780(gp)
	addi	a3,zero,0x28F
	jal	zero,0x00000000000172C4

l0000000000017300:
	ld	a0,-0x780(gp)
	addi	a3,zero,0x284
	jal	zero,0x00000000000172C4

;; fn000000000001730C: 000000000001730C
;;   Called from:
;;     000000000001767C (in fn00000000000175BC)
fn000000000001730C proc
	addi	sp,sp,-0xC0
	sd	s0,0xB0(sp)
	ld	a4,-0x790(gp)
	sd	ra,0xB8(sp)
	sd	s1,0xA8(sp)
	addi	a5,zero,0x80
	sd	a4,0x98(sp)
	bltu	a5,a0,0x00000000000173BC

l000000000001732C:
	addi	a4,zero,0x8
	sd	zero,0x8(sp)
	sd	zero,0x10(sp)
	addi	s1,a1,0x0
	addi	a5,sp,0x8
	subw	a4,a4,a0
	addi	a3,zero,0x7
	addi	a2,zero,-0x1
	addi	a1,zero,0xFF
	beq	a0,zero,0x000000000001736C

l0000000000017354:
	bge	a3,a0,0x00000000000173C4

l0000000000017358:
	sb	a2,(a5)

l000000000001735C:
	addiw	a0,a0,-0x8
	addi	a5,a5,0x1
	addiw	a4,a4,0x8
	blt	zero,a0,0x0000000000017354

l000000000001736C:
	addi	a3,zero,0x80
	addi	a2,sp,0x18
	addi	a1,sp,0x8
	addi	a0,zero,0xA
	jal	ra,inet_ntop
	beq	a0,zero,0x00000000000173BC

l0000000000017384:
	addi	a2,zero,0x10
	addi	a1,sp,0x8
	addi	a0,s1,0x0
	jal	ra,memcpy
	addi	a0,sp,0x18
	jal	ra,__strdup

l000000000001739C:
	ld	a4,0x98(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x00000000000173D0

l00000000000173A8:
	ld	ra,0xB8(sp)
	ld	s0,0xB0(sp)
	ld	s1,0xA8(sp)
	addi	sp,sp,0xC0
	jalr	zero,ra,0x0

l00000000000173BC:
	addi	a0,zero,0x0
	jal	zero,0x000000000001739C

l00000000000173C4:
	sllw	a6,a1,a4
	sb	a6,(a5)
	jal	zero,0x000000000001735C

l00000000000173D0:
	jal	ra,__stack_chk_fail

;; fn00000000000173D4: 00000000000173D4
;;   Called from:
;;     00000000000176F4 (in fn00000000000175BC)
fn00000000000173D4 proc
	addi	sp,sp,-0x30
	sd	s0,0x20(sp)
	sd	ra,0x28(sp)
	sd	s1,0x18(sp)
	sd	s2,0x10(sp)
	sd	s3,0x8(sp)
	addi	a5,zero,0x80
	addi	s0,a0,0x0
	lbu	s2,(a0)
	lbu	s3,0x1(a0)
	beq	a1,a5,0x00000000000174DC

l0000000000017400:
	addi	a5,zero,0x5F
	blt	a5,a1,0x0000000000017474

l0000000000017408:
	slli	a5,s2,0x8
	or	a5,a5,s3
	lui	a4,0xFFFFE
	and	a4,a4,a5
	lui	a3,0x2
	beq	a4,a3,0x0000000000017580

l0000000000017420:
	andi	a4,s2,0xFE
	addi	a2,zero,0xFC
	beq	a4,a2,0x000000000001758C

l000000000001742C:
	lui	a4,0x10
	andi	a2,a5,-0x40
	addiw	a4,a4,-0x180
	beq	a2,a4,0x0000000000017598

l000000000001743C:
	addi	a4,zero,0xFF
	beq	s2,a4,0x00000000000175A4

l0000000000017444:
	lui	a0,0x11
	andi	a5,a5,-0x2
	addiw	a3,a3,0x2
	addi	a0,a0,0x710
	beq	a5,a3,0x00000000000175B0

l0000000000017458:
	ld	ra,0x28(sp)
	ld	s0,0x20(sp)
	ld	s1,0x18(sp)
	ld	s2,0x10(sp)
	ld	s3,0x8(sp)
	addi	sp,sp,0x30
	jalr	zero,ra,0x0

l0000000000017474:
	lui	s1,0x10
	addi	s1,s1,0x480

l000000000001747C:
	addi	a2,zero,0xC
	addi	a1,s1,0x430
	addi	a0,s0,0x0
	jal	ra,memcmp
	beq	a0,zero,0x000000000001755C

l0000000000017490:
	addi	a2,zero,0xC
	addi	a1,s1,0x440
	addi	a0,s0,0x0
	jal	ra,memcmp
	beq	a0,zero,0x0000000000017514

l00000000000174A4:
	addi	a2,zero,0xC
	addi	a1,s1,0x450
	addi	a0,s0,0x0
	jal	ra,memcmp
	bne	a0,zero,0x0000000000017408

l00000000000174B8:
	ld	ra,0x28(sp)
	lui	a0,0x12
	addi	a0,a0,-0x748
	ld	s0,0x20(sp)
	ld	s1,0x18(sp)
	ld	s2,0x10(sp)
	ld	s3,0x8(sp)
	addi	sp,sp,0x30
	jalr	zero,ra,0x0

l00000000000174DC:
	lui	s1,0x10
	addi	s1,s1,0x480
	addi	a2,zero,0x10
	addi	a1,s1,0x400
	jal	ra,memcmp
	beq	a0,zero,0x0000000000017538

l00000000000174F4:
	addi	a2,zero,0x10
	addi	a1,s1,0x418
	addi	a0,s0,0x0
	jal	ra,memcmp
	bne	a0,zero,0x000000000001747C

l0000000000017508:
	lui	a0,0x12
	addi	a0,a0,-0x790
	jal	zero,0x0000000000017458

l0000000000017514:
	ld	ra,0x28(sp)
	lui	a0,0x12
	addi	a0,a0,-0x760
	ld	s0,0x20(sp)
	ld	s1,0x18(sp)
	ld	s2,0x10(sp)
	ld	s3,0x8(sp)
	addi	sp,sp,0x30
	jalr	zero,ra,0x0

l0000000000017538:
	ld	ra,0x28(sp)
	lui	a0,0x12
	addi	a0,a0,-0x7A8
	ld	s0,0x20(sp)
	ld	s1,0x18(sp)
	ld	s2,0x10(sp)
	ld	s3,0x8(sp)
	addi	sp,sp,0x30
	jalr	zero,ra,0x0

l000000000001755C:
	ld	ra,0x28(sp)
	lui	a0,0x12
	addi	a0,a0,-0x778
	ld	s0,0x20(sp)
	ld	s1,0x18(sp)
	ld	s2,0x10(sp)
	ld	s3,0x8(sp)
	addi	sp,sp,0x30
	jalr	zero,ra,0x0

l0000000000017580:
	lui	a0,0x12
	addi	a0,a0,-0x728
	jal	zero,0x0000000000017458

l000000000001758C:
	lui	a0,0x12
	addi	a0,a0,-0x718
	jal	zero,0x0000000000017458

l0000000000017598:
	lui	a0,0x12
	addi	a0,a0,-0x700
	jal	zero,0x0000000000017458

l00000000000175A4:
	lui	a0,0x11
	addi	a0,a0,0x748
	jal	zero,0x0000000000017458

l00000000000175B0:
	lui	a0,0x12
	addi	a0,a0,-0x6E8
	jal	zero,0x0000000000017458

;; fn00000000000175BC: 00000000000175BC
;;   Called from:
;;     0000000000015614 (in fn0000000000015180)
fn00000000000175BC proc
	addi	sp,sp,-0x1A0
	sd	s1,0x188(sp)
	ld	a5,-0x790(gp)
	sd	s0,0x190(sp)
	addi	s0,a2,0x0
	sd	s2,0x180(sp)
	sd	s4,0x170(sp)
	addi	a2,zero,0xC8
	addi	s4,a0,0x0
	addi	s2,a1,0x0
	addi	a0,s0,0x0
	addi	a1,zero,0x0
	sd	s6,0x160(sp)
	sd	ra,0x198(sp)
	sd	s3,0x178(sp)
	sd	s5,0x168(sp)
	sd	s7,0x158(sp)
	sd	s8,0x150(sp)
	sd	s9,0x148(sp)
	addi	s6,a3,0x0
	sd	a5,0x138(sp)
	jal	ra,memset
	addi	a2,sp,0x8
	addi	a1,s4,0x0
	addi	a0,zero,0xA
	jal	ra,inet_pton
	bge	zero,a0,0x00000000000177B4

l0000000000017628:
	addi	a0,sp,0x8
	jal	ra,fn00000000000163B0
	addi	s3,sp,0x38
	sd	a0,0x8(s0)
	addi	a3,zero,0xFA
	addi	a2,s3,0x0
	addi	a1,sp,0x8
	addi	a0,zero,0xA
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017768

l0000000000017650:
	addi	a0,s3,0x0
	jal	ra,__strdup
	sd	a0,(s0)
	addi	s5,zero,0x80
	blt	s5,s2,0x00000000000177DC

l0000000000017664:
	blt	s2,zero,0x0000000000017868

l0000000000017668:
	addi	s5,s2,0x0

l000000000001766C:
	addi	s7,sp,0x18
	sw	s5,0xA0(s0)
	addi	a1,s7,0x0
	addi	a0,s5,0x0
	jal	ra,fn000000000001730C
	sd	a0,0x30(s0)
	beq	a0,zero,0x00000000000178F8

l0000000000017688:
	addi	s8,sp,0x28
	addi	a5,sp,0x8
	addi	s9,s7,0x0
	addi	a2,s8,0x0
	addi	a3,s7,0x0

l000000000001769C:
	lbu	a4,(a5)
	lbu	a1,(a3)
	addi	a5,a5,0x1
	addi	a3,a3,0x1
	and	a4,a4,a1
	sb	a4,(a2)
	addi	a2,a2,0x1
	bne	s7,a5,0x000000000001769C

l00000000000176BC:
	addi	a3,zero,0xFA
	addi	a2,s3,0x0
	addi	a1,sp,0x28
	addi	a0,zero,0xA
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017768

l00000000000176D4:
	addi	a0,s3,0x0
	jal	ra,__strdup
	sd	a0,0x20(s0)
	addi	a0,sp,0x28
	jal	ra,fn00000000000163B0
	sd	a0,0x10(s0)
	addi	a1,s2,0x0
	addi	a0,sp,0x28
	jal	ra,fn00000000000173D4
	sd	a0,0xB8(s0)
	addi	a1,s5,0x0
	addi	a0,sp,0x28
	jal	ra,fn0000000000018118
	sd	a0,0x18(s0)
	addi	a5,zero,0x80
	bne	s2,a5,0x0000000000017870

l0000000000017714:
	ld	a5,0x20(s0)
	addi	a0,s0,0x60
	addi	s2,zero,0x0
	sd	a5,0xA8(s0)
	sd	a5,0xB0(s0)

l0000000000017728:
	slli	s2,s2,0x20
	lui	a5,0x10
	srli	s2,s2,0x1D
	addi	a5,a5,0x480
	add	s2,a5,s2
	ld	a3,(s2)

l0000000000017740:
	lui	a2,0x12
	addi	a2,a2,-0x670
	addi	a1,zero,0x40
	jal	ra,snprintf
	slli	a5,s6,0x31
	blt	a5,zero,0x00000000000178DC

l0000000000017758:
	andi	s6,s6,0x1
	bne	s6,zero,0x0000000000017824

l0000000000017760:
	addi	a0,zero,0x0
	jal	zero,0x0000000000017774

l0000000000017768:
	lw	a5,-0x7EC(gp)
	beq	a5,zero,0x0000000000017804

l0000000000017770:
	addi	a0,zero,-0x1

l0000000000017774:
	ld	a4,0x138(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x0000000000017920

l0000000000017780:
	ld	ra,0x198(sp)
	ld	s0,0x190(sp)
	ld	s1,0x188(sp)
	ld	s2,0x180(sp)
	ld	s3,0x178(sp)
	ld	s4,0x170(sp)
	ld	s5,0x168(sp)
	ld	s6,0x160(sp)
	ld	s7,0x158(sp)
	ld	s8,0x150(sp)
	ld	s9,0x148(sp)
	addi	sp,sp,0x1A0
	jalr	zero,ra,0x0

l00000000000177B4:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000017770

l00000000000177BC:
	ld	a0,-0x780(gp)
	lui	a2,0x12
	addi	a3,s4,0x0
	addi	a2,a2,-0x6E0
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	a0,zero,-0x1
	jal	zero,0x0000000000017774

l00000000000177DC:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000017770

l00000000000177E4:
	ld	a0,-0x780(gp)
	lui	a2,0x12
	addi	a3,s2,0x0
	addi	a2,a2,-0x6C0
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	a0,zero,-0x1
	jal	zero,0x0000000000017774

l0000000000017804:
	ld	a3,-0x780(gp)
	lui	a0,0x11
	addi	a2,zero,0x2B
	addi	a1,zero,0x1
	addi	a0,a0,0x7F8
	jal	ra,fwrite
	addi	a0,zero,-0x1
	jal	zero,0x0000000000017774

l0000000000017824:
	addi	a1,sp,0x8
	addi	a0,zero,0xA
	jal	ra,fn00000000000168E4
	sd	a0,0x38(s0)
	bne	a0,zero,0x0000000000017760

l0000000000017838:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000017770

l0000000000017840:
	lui	a3,0x12
	addi	a4,s4,0x0
	addi	a3,a3,-0x7D0
	addi	a2,zero,0xFA
	addi	a1,zero,0x1
	addi	a0,s3,0x0
	jal	ra,__sprintf_chk
	addi	a0,s3,0x0
	jal	ra,herror
	jal	zero,0x0000000000017770

l0000000000017868:
	addi	s2,s5,0x0
	jal	zero,0x000000000001766C

l0000000000017870:
	addi	a0,s3,0x0
	jal	ra,__strdup
	sd	a0,0xA8(s0)

l000000000001787C:
	lbu	a5,(s9)
	lbu	a4,(s8)
	addi	s8,s8,0x1
	xori	a5,a5,-0x1
	or	a5,a5,a4
	sb	a5,-0x1(s8)
	addi	s9,s9,0x1
	bne	s3,s8,0x000000000001787C

l000000000001789C:
	addi	a3,zero,0xFA
	addi	a2,s3,0x0
	addi	a1,sp,0x28
	addi	a0,zero,0xA
	jal	ra,inet_ntop
	beq	a0,zero,0x0000000000017768

l00000000000178B4:
	addi	a0,s3,0x0
	jal	ra,__strdup
	addi	a5,zero,0x80
	sd	a0,0xB0(s0)
	subw	s2,a5,s2
	addi	a0,s0,0x60
	bne	s2,a5,0x0000000000017728

l00000000000178D0:
	lui	a3,0x11
	addi	a3,a3,0x1D0
	jal	zero,0x0000000000017740

l00000000000178DC:
	addi	a4,s0,0x58
	addi	a3,s0,0x50
	addi	a2,s0,0x48
	addi	a1,s0,0x40
	addi	a0,sp,0x8
	jal	ra,fn0000000000017D10
	jal	zero,0x0000000000017758

l00000000000178F8:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x0000000000017770

l0000000000017900:
	ld	a0,-0x780(gp)
	lui	a2,0x12
	addi	a3,s2,0x0
	addi	a2,a2,-0x6A0
	addi	a1,zero,0x1
	jal	ra,__fprintf_chk
	addi	a0,zero,-0x1
	jal	zero,0x0000000000017774

l0000000000017920:
	jal	ra,__stack_chk_fail

;; fn0000000000017924: 0000000000017924
;;   Called from:
;;     00000000000153BC (in fn0000000000015180)
;;     0000000000015924 (in fn0000000000015180)
;;     0000000000017B7C (in fn0000000000017B40)
;;     0000000000017D4C (in fn0000000000017D10)
fn0000000000017924 proc
	addi	sp,sp,-0x60
	sd	s2,0x40(sp)
	ld	a5,-0x7E0(gp)
	sd	s1,0x48(sp)
	sd	ra,0x58(sp)
	sd	s0,0x50(sp)
	sd	s3,0x38(sp)
	sd	s4,0x30(sp)
	sd	s5,0x28(sp)
	sd	s6,0x20(sp)
	sd	s7,0x18(sp)
	sd	s8,0x10(sp)
	sd	s9,0x8(sp)
	beq	a5,zero,0x00000000000179B8

l000000000001795C:
	lw	a5,-0x7EC(gp)
	bne	a5,zero,0x000000000001796C

l0000000000017964:
	lbu	a5,-0x320(gp)
	bne	a5,zero,0x00000000000179A8

l000000000001796C:
	lw	s0,-0x7E8(gp)

l0000000000017970:
	ld	ra,0x58(sp)
	addi	a0,s0,0x0
	ld	s1,0x48(sp)
	ld	s0,0x50(sp)
	ld	s2,0x40(sp)
	ld	s3,0x38(sp)
	ld	s4,0x30(sp)
	ld	s5,0x28(sp)
	ld	s6,0x20(sp)
	ld	s7,0x18(sp)
	ld	s8,0x10(sp)
	ld	s9,0x8(sp)
	addi	sp,sp,0x60
	jalr	zero,ra,0x0

l00000000000179A8:
	ld	a1,-0x780(gp)
	addi	a0,gp,-0x320
	jal	ra,fputs
	jal	zero,0x000000000001796C

l00000000000179B8:
	lw	s0,-0x7E8(gp)
	bne	s0,zero,0x000000000001795C

l00000000000179C0:
	lui	s4,0x13
	addi	a1,zero,0x1
	addi	a0,s4,-0x5C8
	jal	ra,dlopen
	sd	a0,-0x7E0(gp)
	addi	s3,a0,0x0
	beq	a0,zero,0x0000000000017B14

l00000000000179DC:
	lui	a1,0x13
	addi	a1,a1,-0x588
	jal	ra,dlsym
	lui	a1,0x13
	sd	a0,-0x798(gp)
	addi	a1,a1,-0x570
	addi	a0,s3,0x0
	jal	ra,dlsym
	lui	a1,0x13
	addi	s2,a0,0x0
	addi	a1,a1,-0x560
	addi	a0,s3,0x0
	sd	s2,-0x7A0(gp)
	jal	ra,dlsym
	lui	a1,0x13
	addi	s4,a0,0x0
	addi	a1,a1,-0x540
	addi	a0,s3,0x0
	sd	s4,-0x7A8(gp)
	jal	ra,dlsym
	lui	a1,0x13
	addi	s5,a0,0x0
	addi	a1,a1,-0x530
	addi	a0,s3,0x0
	sd	s5,-0x7B8(gp)
	jal	ra,dlsym
	lui	a1,0x13
	addi	s6,a0,0x0
	addi	a1,a1,-0x518
	addi	a0,s3,0x0
	sd	s6,-0x7C0(gp)
	jal	ra,dlsym
	lui	a1,0x13
	addi	s7,a0,0x0
	addi	a1,a1,-0x500
	addi	a0,s3,0x0
	sd	s7,-0x7C8(gp)
	jal	ra,dlsym
	lui	a1,0x13
	addi	s8,a0,0x0
	addi	a1,a1,-0x4E8
	addi	a0,s3,0x0
	sd	s8,-0x7D0(gp)
	jal	ra,dlsym
	lui	a1,0x13
	addi	s9,a0,0x0
	addi	a1,a1,-0x4C8
	addi	a0,s3,0x0
	sd	s9,-0x7D8(gp)
	jal	ra,dlsym
	sd	a0,-0x7B0(gp)
	beq	s2,zero,0x0000000000017ACC

l0000000000017AAC:
	beq	s4,zero,0x0000000000017ACC

l0000000000017AB0:
	beq	s5,zero,0x0000000000017ACC

l0000000000017AB4:
	beq	s6,zero,0x0000000000017ACC

l0000000000017AB8:
	beq	s7,zero,0x0000000000017ACC

l0000000000017ABC:
	beq	s8,zero,0x0000000000017ACC

l0000000000017AC0:
	beq	s9,zero,0x0000000000017ACC

l0000000000017AC4:
	sw	zero,-0x7E8(gp)
	jal	zero,0x0000000000017970

l0000000000017ACC:
	lui	a4,0x13
	addi	a5,a4,-0x4B0
	ld	a6,-0x4B0(a4)
	ld	a0,0x8(a5)
	ld	a1,0x10(a5)
	ld	a2,0x18(a5)
	ld	a3,0x20(a5)
	lw	a4,0x28(a5)
	addi	a5,gp,-0x320
	addi	s0,zero,-0x1
	sd	a6,(a5)
	sd	a0,0x8(a5)
	sd	a1,0x10(a5)
	sd	a2,0x18(a5)
	sd	a3,0x20(a5)
	sw	a4,0x28(a5)
	sw	s0,-0x7E8(gp)
	jal	zero,0x0000000000017970

l0000000000017B14:
	addi	a3,zero,0x100
	lui	a4,0x13
	addi	a5,s4,-0x5C8
	addi	a4,a4,-0x5A8
	addi	a2,zero,0x1
	addi	a1,a3,0x0
	addi	a0,gp,-0x320
	addi	s0,zero,-0x1
	jal	ra,__snprintf_chk
	sw	s0,-0x7E8(gp)
	jal	zero,0x0000000000017970

;; fn0000000000017B40: 0000000000017B40
;;   Called from:
;;     000000000001729C (in fn0000000000016D74)
fn0000000000017B40 proc
	addi	sp,sp,-0x50
	sd	s0,0x40(sp)
	sd	s1,0x38(sp)
	sd	s2,0x30(sp)
	sd	s3,0x28(sp)
	sd	s4,0x20(sp)
	sd	ra,0x48(sp)
	sd	s5,0x18(sp)
	sd	s6,0x10(sp)
	sd	s7,0x8(sp)
	addi	s0,a0,0x0
	addi	s3,a1,0x0
	addi	s2,a2,0x0
	addi	s1,a3,0x0
	addi	s4,a4,0x0
	jal	ra,fn0000000000017924
	beq	a0,zero,0x0000000000017BB0

l0000000000017B84:
	ld	ra,0x48(sp)
	ld	s0,0x40(sp)
	ld	s1,0x38(sp)
	ld	s2,0x30(sp)
	ld	s3,0x28(sp)
	ld	s4,0x20(sp)
	ld	s5,0x18(sp)
	ld	s6,0x10(sp)
	ld	s7,0x8(sp)
	addi	sp,sp,0x50
	jalr	zero,ra,0x0

l0000000000017BB0:
	addi	a0,s0,0x0
	jal	ra,fn00000000000182B4
	ld	a5,-0x798(gp)
	addi	s7,a0,0x0
	jalr	ra,a5,0x0
	ld	a5,-0x7A0(gp)
	addi	a1,zero,0x10
	addi	a0,zero,0x1
	jalr	ra,a5,0x0
	addi	s0,a0,0x0
	beq	a0,zero,0x0000000000017C3C

l0000000000017BDC:
	ld	a5,-0x7C8(gp)
	addi	a4,zero,0x1
	slli	a1,s7,0x20
	sw	a4,0x4C(a0)
	srli	a1,a1,0x20
	jalr	ra,a5,0x0
	addi	s6,a0,0x0
	blt	a0,zero,0x0000000000017B84

l0000000000017BFC:
	ld	a5,-0x7A8(gp)
	addi	a1,a0,0x0
	addi	a0,s0,0x0
	jalr	ra,a5,0x0
	beq	a0,zero,0x0000000000017C18

l0000000000017C10:
	jal	ra,__strdup
	sd	a0,(s3)

l0000000000017C18:
	ld	a5,-0x7B0(gp)
	addi	a0,s6,0x0
	jalr	ra,a5,0x0
	beq	a0,zero,0x0000000000017C30

l0000000000017C28:
	jal	ra,__strdup
	sd	a0,(s2)

l0000000000017C30:
	ld	a5,-0x7B8(gp)
	addi	a0,s0,0x0
	jalr	ra,a5,0x0

l0000000000017C3C:
	ld	a5,-0x7A0(gp)
	addi	a1,zero,0x10
	addi	a0,zero,0x2
	jalr	ra,a5,0x0
	addi	s0,a0,0x0
	beq	a0,zero,0x0000000000017CF4

l0000000000017C54:
	ld	a5,-0x7C0(gp)
	addi	a4,zero,0x1
	slli	a1,s7,0x20
	sw	a4,0x4C(s0)
	srli	a1,a1,0x20
	jalr	ra,a5,0x0
	addi	s2,a0,0x0
	beq	a0,zero,0x0000000000017C94

l0000000000017C74:
	ld	a0,0x20(a0)
	beq	a0,zero,0x0000000000017C84

l0000000000017C7C:
	jal	ra,__strdup
	sd	a0,(s1)

l0000000000017C84:
	flw	fa4,0x34(s2)
	fmv.w.x	fa5,zero
	feq.s	a5,fa4,fa5
	beq	a5,zero,0x0000000000017CC8

l0000000000017C94:
	addi	a0,s0,0x0
	ld	ra,0x48(sp)
	ld	s0,0x40(sp)
	ld	s1,0x38(sp)
	ld	s2,0x30(sp)
	ld	s3,0x28(sp)
	ld	s4,0x20(sp)
	ld	s5,0x18(sp)
	ld	s6,0x10(sp)
	ld	s7,0x8(sp)
	ld	t1,-0x7B8(gp)
	addi	sp,sp,0x50
	jalr	zero,t1,0x0

l0000000000017CC8:
	flw	fa5,0x30(s2)
	fcvt.d.s	fa4,fa4
	lui	a2,0x13
	fcvt.d.s	fa5,fa5
	fmv.x.d	a4,fa4
	addi	a2,a2,-0x480
	fmv.x.d	a3,fa5
	addi	a1,zero,0x1
	addi	a0,s4,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017C94

l0000000000017CF4:
	ld	a5,-0x7A0(gp)
	addi	a1,zero,0x10
	addi	a0,zero,0x6
	jalr	ra,a5,0x0
	addi	s0,a0,0x0
	bne	a0,zero,0x0000000000017C54

l0000000000017D0C:
	jal	zero,0x0000000000017B84

;; fn0000000000017D10: 0000000000017D10
;;   Called from:
;;     00000000000178F0 (in fn00000000000175BC)
fn0000000000017D10 proc
	addi	sp,sp,-0x50
	sd	s0,0x40(sp)
	sd	s2,0x30(sp)
	sd	s3,0x28(sp)
	sd	s4,0x20(sp)
	sd	s5,0x18(sp)
	sd	ra,0x48(sp)
	sd	s1,0x38(sp)
	sd	s6,0x10(sp)
	sd	s7,0x8(sp)
	addi	s0,a0,0x0
	addi	s4,a1,0x0
	addi	s3,a2,0x0
	addi	s2,a3,0x0
	addi	s5,a4,0x0
	jal	ra,fn0000000000017924
	beq	a0,zero,0x0000000000017D80

l0000000000017D54:
	ld	ra,0x48(sp)
	ld	s0,0x40(sp)
	ld	s1,0x38(sp)
	ld	s2,0x30(sp)
	ld	s3,0x28(sp)
	ld	s4,0x20(sp)
	ld	s5,0x18(sp)
	ld	s6,0x10(sp)
	ld	s7,0x8(sp)
	addi	sp,sp,0x50
	jalr	zero,ra,0x0

l0000000000017D80:
	ld	a5,-0x798(gp)
	jalr	ra,a5,0x0
	ld	a5,-0x7A0(gp)
	addi	a1,zero,0x10
	addi	a0,zero,0xC
	jalr	ra,a5,0x0
	addi	s1,a0,0x0
	beq	a0,zero,0x0000000000017E18

l0000000000017DA0:
	lwu	a4,0x4(s0)
	lwu	a5,0xC(s0)
	lwu	a1,(s0)
	lwu	a2,0x8(s0)
	ld	a3,-0x7D0(gp)
	slli	a4,a4,0x20
	slli	a5,a5,0x20
	addi	a6,zero,0x1
	sw	a6,0x4C(a0)
	or	a1,a4,a1
	or	a2,a5,a2
	jalr	ra,a3,0x0
	addi	s7,a0,0x0
	blt	a0,zero,0x0000000000017D54

l0000000000017DD8:
	ld	a5,-0x7A8(gp)
	addi	a1,a0,0x0
	addi	a0,s1,0x0
	jalr	ra,a5,0x0
	beq	a0,zero,0x0000000000017DF4

l0000000000017DEC:
	jal	ra,__strdup
	sd	a0,(s4)

l0000000000017DF4:
	ld	a5,-0x7B0(gp)
	addi	a0,s7,0x0
	jalr	ra,a5,0x0
	beq	a0,zero,0x0000000000017E0C

l0000000000017E04:
	jal	ra,__strdup
	sd	a0,(s3)

l0000000000017E0C:
	ld	a5,-0x7B8(gp)
	addi	a0,s1,0x0
	jalr	ra,a5,0x0

l0000000000017E18:
	ld	a5,-0x7A0(gp)
	addi	a1,zero,0x10
	addi	a0,zero,0x1E
	jalr	ra,a5,0x0
	addi	s1,a0,0x0
	beq	a0,zero,0x0000000000017EE8

l0000000000017E30:
	lwu	a4,0x4(s0)
	lwu	a5,0xC(s0)
	lwu	a1,(s0)
	lwu	a2,0x8(s0)
	ld	a3,-0x7D8(gp)
	slli	a4,a4,0x20
	slli	a5,a5,0x20
	addi	a6,zero,0x1
	sw	a6,0x4C(s1)
	or	a1,a4,a1
	or	a2,a5,a2
	jalr	ra,a3,0x0
	addi	s0,a0,0x0
	beq	a0,zero,0x0000000000017E88

l0000000000017E68:
	ld	a0,0x20(a0)
	beq	a0,zero,0x0000000000017E78

l0000000000017E70:
	jal	ra,__strdup
	sd	a0,(s2)

l0000000000017E78:
	flw	fa4,0x34(s0)
	fmv.w.x	fa5,zero
	feq.s	a5,fa4,fa5
	beq	a5,zero,0x0000000000017EBC

l0000000000017E88:
	addi	a0,s1,0x0
	ld	ra,0x48(sp)
	ld	s0,0x40(sp)
	ld	s1,0x38(sp)
	ld	s2,0x30(sp)
	ld	s3,0x28(sp)
	ld	s4,0x20(sp)
	ld	s5,0x18(sp)
	ld	s6,0x10(sp)
	ld	s7,0x8(sp)
	ld	t1,-0x7B8(gp)
	addi	sp,sp,0x50
	jalr	zero,t1,0x0

l0000000000017EBC:
	flw	fa5,0x30(s0)
	fcvt.d.s	fa4,fa4
	lui	a2,0x13
	fcvt.d.s	fa5,fa5
	fmv.x.d	a4,fa4
	addi	a2,a2,-0x480
	fmv.x.d	a3,fa5
	addi	a1,zero,0x1
	addi	a0,s5,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017E88

l0000000000017EE8:
	ld	a5,-0x7A0(gp)
	addi	a1,zero,0x10
	addi	a0,zero,0x1F
	jalr	ra,a5,0x0
	addi	s1,a0,0x0
	bne	a0,zero,0x0000000000017E30

l0000000000017F00:
	jal	zero,0x0000000000017D54

;; fn0000000000017F04: 0000000000017F04
;;   Called from:
;;     0000000000016FB4 (in fn0000000000016D74)
fn0000000000017F04 proc
	addi	sp,sp,-0x50
	sd	s3,0x28(sp)
	ld	a5,-0x790(gp)
	sd	s0,0x40(sp)
	sd	s1,0x38(sp)
	sd	s2,0x30(sp)
	sd	s4,0x20(sp)
	sd	s5,0x18(sp)
	sd	s6,0x10(sp)
	addi	s0,a1,0x0
	sd	ra,0x48(sp)
	addi	s5,a2,0x0
	addi	s4,a3,0x0
	sd	a5,0x8(sp)
	sd	zero,(sp)
	jal	ra,fn00000000000182B4
	srliw	a1,a0,0x10
	srliw	t1,a0,0x8
	addi	t3,zero,0x20
	srliw	s6,a0,0x18
	andi	s1,a1,0xFF
	andi	s2,t1,0xFF
	beq	s0,t3,0x0000000000018048

l0000000000017F60:
	addi	a1,zero,0x18
	beq	s0,a1,0x0000000000018070

l0000000000017F68:
	addi	a0,zero,0x10
	beq	s0,a0,0x00000000000180D8

l0000000000017F70:
	addi	a5,zero,0x8
	beq	s0,a5,0x00000000000180F8

l0000000000017F78:
	bltu	a1,s0,0x0000000000017FBC

l0000000000017F7C:
	bltu	a0,s0,0x0000000000018094

l0000000000017F80:
	bltu	a5,s0,0x0000000000018008

l0000000000017F84:
	addi	a0,zero,0x0

l0000000000017F88:
	ld	a4,0x8(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x0000000000018114

l0000000000017F94:
	ld	ra,0x48(sp)
	ld	s0,0x40(sp)
	ld	s1,0x38(sp)
	ld	s2,0x30(sp)
	ld	s3,0x28(sp)
	ld	s4,0x20(sp)
	ld	s5,0x18(sp)
	ld	s6,0x10(sp)
	addi	sp,sp,0x50
	jalr	zero,ra,0x0

l0000000000017FBC:
	addi	a0,s5,0x0
	jal	ra,fn00000000000182B4
	addi	s0,a0,0x0
	addi	a0,s4,0x0
	jal	ra,fn00000000000182B4
	lui	a2,0x13
	andi	a4,a0,0xFF
	addi	a7,s6,0x0
	addi	a6,s1,0x0
	addi	a5,s2,0x0
	andi	a3,s0,0xFF
	addi	a2,a2,-0x410
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk

l0000000000017FF8:
	addi	a5,zero,-0x1
	beq	a0,a5,0x0000000000017F84

l0000000000018000:
	ld	a0,(sp)
	jal	zero,0x0000000000017F88

l0000000000018008:
	addi	a0,s5,0x0
	jal	ra,fn00000000000182B4
	addi	s0,a0,0x0
	addi	a0,s4,0x0
	jal	ra,fn00000000000182B4
	srliw	a4,a0,0x10
	srliw	a3,s0,0x10
	lui	a2,0x13
	addi	a5,s6,0x0
	andi	a4,a4,0xFF
	andi	a3,a3,0xFF
	addi	a2,a2,-0x3D0
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017FF8

l0000000000018048:
	lui	a2,0x13
	andi	a3,a0,0xFF
	addi	a6,s6,0x0
	addi	a5,s1,0x0
	addi	a4,s2,0x0
	addi	a2,a2,-0x478
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017FF8

l0000000000018070:
	lui	a2,0x13
	addi	a5,s6,0x0
	addi	a4,s1,0x0
	addi	a3,s2,0x0
	addi	a2,a2,-0x458
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017FF8

l0000000000018094:
	addi	a0,s5,0x0
	jal	ra,fn00000000000182B4
	addi	s0,a0,0x0
	addi	a0,s4,0x0
	jal	ra,fn00000000000182B4
	srliw	a4,a0,0x8
	srliw	a3,s0,0x8
	lui	a2,0x13
	addi	a6,s6,0x0
	addi	a5,s1,0x0
	andi	a4,a4,0xFF
	andi	a3,a3,0xFF
	addi	a2,a2,-0x3F0
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017FF8

l00000000000180D8:
	lui	a2,0x13
	addi	a4,s6,0x0
	addi	a3,s1,0x0
	addi	a2,a2,-0x440
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017FF8

l00000000000180F8:
	lui	a2,0x13
	addi	a3,s6,0x0
	addi	a2,a2,-0x428
	addi	a1,zero,0x1
	addi	a0,sp,0x0
	jal	ra,__asprintf_chk
	jal	zero,0x0000000000017FF8

l0000000000018114:
	jal	ra,__stack_chk_fail

;; fn0000000000018118: 0000000000018118
;;   Called from:
;;     0000000000017704 (in fn00000000000175BC)
fn0000000000018118 proc
	addi	sp,sp,-0x120
	sd	s0,0x110(sp)
	ld	a5,-0x790(gp)
	sd	ra,0x118(sp)
	andi	t4,a1,0x3
	sd	a5,0x108(sp)
	bne	t4,zero,0x00000000000182A8

l0000000000018134:
	andi	a5,a1,0x7
	addi	a4,zero,0x4
	srliw	t5,a1,0x3
	beq	a5,a4,0x0000000000018264

l0000000000018144:
	beq	t5,zero,0x00000000000181FC

l0000000000018148:
	addiw	a1,t5,-0x1
	addi	a4,t4,0x0
	addi	t1,zero,0x9
	addi	a7,zero,0x2E
	addi	t3,zero,-0x1
	jal	zero,0x0000000000018190

l0000000000018160:
	slli	a2,a2,0x20
	addi	a5,sp,0x110
	srli	a2,a2,0x20
	slli	a3,a3,0x20
	add	a2,a5,a2
	srli	a3,a3,0x20
	sb	a6,-0x108(a2)
	add	a3,a5,a3
	sb	a7,-0x108(a3)
	addiw	a1,a1,-0x1
	addiw	a4,a4,0x4
	beq	a1,t3,0x00000000000181F4

l0000000000018190:
	slli	a5,a1,0x20
	srli	a5,a5,0x20
	add	a5,a0,a5
	lbu	a5,(a5)
	addiw	a3,a4,0x1
	andi	a2,a5,0xF
	addi	a6,a2,0x57
	bltu	t1,a2,0x00000000000181B4

l00000000000181B0:
	addi	a6,a2,0x30

l00000000000181B4:
	slli	a2,a4,0x20
	addi	t6,sp,0x110
	srli	a2,a2,0x20
	slli	a3,a3,0x20
	add	a2,t6,a2
	srli	a3,a3,0x20
	sb	a6,-0x108(a2)
	add	a3,t6,a3
	srli	a5,a5,0x4
	sb	a7,-0x108(a3)
	addiw	a2,a4,0x2
	addiw	a3,a4,0x3
	addi	a6,a5,0x30
	bgeu	t1,a5,0x0000000000018160

l00000000000181EC:
	addi	a6,a5,0x57
	jal	zero,0x0000000000018160

l00000000000181F4:
	slliw	t5,t5,0x2
	addw	t4,t4,t5

l00000000000181FC:
	slli	t4,t4,0x20
	lui	a5,0x7
	addi	a4,sp,0x8
	srli	t4,t4,0x20
	add	t4,a4,t4
	addiw	a4,a5,0x69
	addiw	a5,a5,0x261
	sh	a5,0x4(t4)
	lui	a5,0x6
	addiw	a5,a5,0x170
	sh	a4,(t4)
	lui	a4,0x3
	addiw	a4,a4,-0x1CA
	sh	a5,0x6(t4)
	addi	a5,zero,0x2E
	addi	a0,sp,0x8
	sh	a4,0x2(t4)
	sh	a5,0x8(t4)
	jal	ra,__strdup

l0000000000018248:
	ld	a4,0x108(sp)
	ld	a5,-0x790(gp)
	bne	a4,a5,0x00000000000182B0

l0000000000018254:
	ld	ra,0x118(sp)
	ld	s0,0x110(sp)
	addi	sp,sp,0x120
	jalr	zero,ra,0x0

l0000000000018264:
	addw	a5,a1,a4
	srliw	a5,a5,0x3
	addiw	a5,a5,-0x1
	slli	a5,a5,0x20
	srli	a5,a5,0x20
	add	a5,a0,a5
	lbu	a5,(a5)
	addi	a3,zero,0x9
	srli	a5,a5,0x4
	addi	a4,a5,0x57
	bltu	a3,a5,0x0000000000018294

l0000000000018290:
	addi	a4,a5,0x30

l0000000000018294:
	addi	a5,zero,0x2E
	sb	a4,0x8(sp)
	sb	a5,0x9(sp)
	addi	t4,zero,0x2
	jal	zero,0x0000000000018144

l00000000000182A8:
	addi	a0,zero,0x0
	jal	zero,0x0000000000018248

l00000000000182B0:
	jal	ra,__stack_chk_fail

;; fn00000000000182B4: 00000000000182B4
;;   Called from:
;;     0000000000016754 (in fn00000000000166F4)
;;     000000000001685C (in fn0000000000016830)
;;     0000000000016B70 (in fn0000000000016B68)
;;     0000000000016E9C (in fn0000000000016D74)
;;     0000000000017018 (in fn0000000000016D74)
;;     00000000000170F8 (in fn0000000000016D74)
;;     0000000000017100 (in fn0000000000016D74)
;;     0000000000017BB4 (in fn0000000000017B40)
;;     0000000000017F40 (in fn0000000000017F04)
;;     0000000000017FC0 (in fn0000000000017F04)
;;     0000000000017FCC (in fn0000000000017F04)
;;     000000000001800C (in fn0000000000017F04)
;;     0000000000018018 (in fn0000000000017F04)
;;     0000000000018098 (in fn0000000000017F04)
;;     00000000000180A4 (in fn0000000000017F04)
fn00000000000182B4 proc
	slliw	a5,a0,0x18
	srliw	a3,a0,0x18
	or	a3,a3,a5
	lui	a4,0xFF0
	lui	a5,0x10
	and	a4,a0,a4
	addiw	a5,a5,-0x100
	sraiw	a4,a4,0x8
	and	a0,a0,a5
	slliw	a0,a0,0x8
	or	a5,a3,a4
	or	a0,a5,a0
	jalr	zero,ra,0x0

;; fn00000000000182E8: 00000000000182E8
fn00000000000182E8 proc
	addi	sp,sp,-0x40
	sd	s0,0x30(sp)
	sd	s2,0x20(sp)
	auipc	s0,0x2
	addi	s0,s0,-0x4DC
	auipc	s2,0x2
	addi	s2,s2,-0x4DC
	sub	s2,s2,s0
	sd	ra,0x38(sp)
	sd	s1,0x28(sp)
	sd	s3,0x18(sp)
	sd	s4,0x10(sp)
	sd	s5,0x8(sp)
	srai	s2,s2,0x3
	beq	s2,zero,0x0000000000018354

l0000000000018324:
	addi	s5,a0,0x0
	addi	s4,a1,0x0
	addi	s3,a2,0x0
	addi	s1,zero,0x0

l0000000000018334:
	ld	a5,(s0)
	addi	a2,s3,0x0
	addi	a1,s4,0x0
	addi	a0,s5,0x0
	addi	s1,s1,0x1
	jalr	ra,a5,0x0
	addi	s0,s0,0x8
	bne	s2,s1,0x0000000000018334

l0000000000018354:
	ld	ra,0x38(sp)
	ld	s0,0x30(sp)
	ld	s1,0x28(sp)
	ld	s2,0x20(sp)
	ld	s3,0x18(sp)
	ld	s4,0x10(sp)
	ld	s5,0x8(sp)
	addi	sp,sp,0x40
	jalr	zero,ra,0x0

;; fn0000000000018378: 0000000000018378
fn0000000000018378 proc
	jalr	zero,ra,0x0
