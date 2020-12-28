;;; Segment code (00000000)
00000000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000100 00 00 3E 79 15 00 00 00 00 00 00 00 00 00 00 00 ..>y............
00000110 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000200 00 00 3E 84 15 00 00 00 00 00 00 00 00 00 00 00 ..>.............
00000210 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000300 00 00 3E 4D 15 00 00 00 00 00 00 00 00 00 00 00 ..>M............
00000310 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000400 00 00 3E 16 15 00 00 00 00 00 00 00 00 00 00 00 ..>.............
00000410 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000500 00 00 3D DF 15 00 00 00 00 00 00 00 00 00 00 00 ..=.............
00000510 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000600 00 00 3D A8 15 00 00 00 00 00 00 00 00 00 00 00 ..=.............
00000610 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000700 00 00 3D 71 15 00 00 00 00 00 00 00 00 00 00 00 ..=q............
00000710 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000800 00 00 3D 3A 15 00 00 00 00 00 00 00 00 00 00 00 ..=:............
00000810 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000900 00 00 3D 03 15 00 00 00 00 00 00 00 00 00 00 00 ..=.............
00000910 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000A00 00 00 3C CC 15 00 00 00 00 00 00 00 00 00 00 00 ..<.............
00000A10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000B00 00 00 3C 95 15 00 00 00 00 00 00 00 00 00 00 00 ..<.............
00000B10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000C00 00 00 3C 5E 15 00 00 00 00 00 00 00 00 00 00 00 ..<^............
00000C10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000D00 00 00 3C 27 15 00 00 00 00 00 00 00 00 00 00 00 ..<'............
00000D10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000E00 00 00 3B F0 15 00 00 00 00 00 00 00 00 00 00 00 ..;.............
00000E10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...

;; fn000043D0: 000043D0
;;   Called from:
;;     0000495C (in fn0000490C)
;;     00004A8C (in fn00004A6C)
;;     00004BC4 (in fn00004B44)
;;     00004D28 (in fn00004C48)
fn000043D0 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.sfeqi	r5,00000000
	l.addi	r1,r1,-00000008
	l.slli	r3,r3,00000004
	l.bf	00004480
	l.slli	r4,r4,00000002

l000043EC:
	l.movhi	r5,000001F0
	l.ori	r5,r5,00001540
	l.add	r2,r3,r5
	l.add	r2,r2,r4
	l.lwz	r3,0(r2)
	l.sfeqi	r3,00000000
	l.bf	000044BC
	l.addi	r3,r0,+000000FE

l0000440C:
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00000014
	l.addi	r3,r0,+000000F8
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+000000E0
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+000000C0
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+00000080
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+00000000
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00000014

l00004468:
	l.lwz	r3,0(r2)
	l.sfnei	r3,00000000
	l.bf	00004468
	l.nop

l00004478:
	l.j	000044C0
	l.addi	r1,r1,+00000008

l00004480:
	l.movhi	r5,000001F0
	l.ori	r5,r5,00001540
	l.add	r2,r3,r5
	l.add	r2,r2,r4
	l.lwz	r3,0(r2)
	l.sfeqi	r3,000000FF
	l.bf	000044BC
	l.addi	r3,r0,+000000FF

l000044A0:
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000001E

l000044AC:
	l.lwz	r3,0(r2)
	l.sfnei	r3,000000FF
	l.bf	000044AC
	l.nop

l000044BC:
	l.addi	r1,r1,+00000008

l000044C0:
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn000044D0: 000044D0
;;   Called from:
;;     0000EA44 (in fn0000E98C)
fn000044D0 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r2
	l.ori	r3,r3,00003158
	l.addi	r2,r0,+00000000
	l.addi	r1,r1,-00000004
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000004
	l.ori	r11,r2,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)
000044F8                         18 60 00 01 D7 E1 17 FC         .`......
00004500 A8 63 31 58 9C 40 00 00 9C 21 FF FC D4 03 10 00 .c1X.@...!......
00004510 9C 21 00 04 A9 62 00 00 44 00 48 00 84 41 FF FC .!...b..D.H..A..
00004520 18 60 01 F0 18 80 16 AA A8 63 1D A0 A8 84 EF E8 .`.......c......
00004530 9D 60 00 00 D4 03 20 00 18 80 AA 16 A8 84 EF E8 .`.... .........
00004540 D4 03 20 00 44 00 48 00 15 00 00 00 18 60 01 F0 .. .D.H......`..
00004550 18 80 16 AA A8 63 1D A0 9D 60 00 00 D4 03 20 00 .....c...`.... .
00004560 18 80 AA 16 D4 03 20 00 44 00 48 00 15 00 00 00 ...... .D.H.....

;; fn00004570: 00004570
;;   Called from:
;;     0000EC2C (in fn0000EC18)
;;     0000EC4C (in fn0000EC18)
;;     0000F1E8 (in fn0000F194)
fn00004570 proc
	l.movhi	r5,00000001
	l.sw	-4(r1),r2
	l.ori	r5,r5,00003158
	l.addi	r1,r1,-00000008
	l.lwz	r6,0(r5)
	l.sfgtsi	r6,+00000003
	l.bf	00004600
	l.addi	r11,r0,-0000001C

l00004590:
	l.movhi	r6,000001F0
	l.addi	r11,r0,+00000001
	l.ori	r6,r6,00001C0C
	l.movhi	r2,0000003E
	l.lwz	r7,0(r6)
	l.ori	r2,r2,00000382
	l.sw	0(r1),r7
	l.lwz	r8,0(r1)
	l.lwz	r7,0(r5)
	l.sll	r7,r11,r7
	l.or	r7,r7,r8
	l.sw	0(r1),r7
	l.lwz	r7,0(r1)
	l.sw	0(r6),r7
	l.lwz	r6,0(r5)
	l.add	r6,r6,r2
	l.movhi	r2,000001F0
	l.slli	r6,r6,00000003
	l.ori	r2,r2,00001C14
	l.sw	0(r6),r3
	l.lwz	r3,0(r5)
	l.slli	r3,r3,00000003
	l.add	r3,r3,r2
	l.sw	0(r3),r4
	l.lwz	r3,0(r5)
	l.add	r3,r3,r11
	l.addi	r11,r0,+00000000
	l.sw	0(r5),r3

l00004600:
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000460C: 0000460C
;;   Called from:
;;     0000489C (in fn00004844)
;;     000048CC (in fn00004844)
;;     000048D8 (in fn00004844)
;;     000048E4 (in fn00004844)
;;     000048F0 (in fn00004844)
;;     000049A0 (in fn00004978)
;;     00004AC8 (in fn00004A6C)
fn0000460C proc
	l.srli	r5,r3,00000002
	l.addi	r1,r1,-00000004
	l.sfeqi	r5,00000000
	l.bf	00004640
	l.andi	r4,r4,00000001

l00004620:
	l.movhi	r5,00000170
	l.addi	r3,r3,-00000004
	l.ori	r5,r5,00000084
	l.lwz	r6,0(r5)
	l.sw	0(r1),r6
	l.lwz	r7,0(r1)
	l.j	00004658
	l.addi	r6,r0,+00000001

l00004640:
	l.movhi	r5,00000170
	l.ori	r5,r5,00000080
	l.lwz	r6,0(r5)
	l.sw	0(r1),r6
	l.lwz	r7,0(r1)
	l.addi	r6,r0,+00000001

l00004658:
	l.addi	r11,r0,+00000000
	l.sll	r6,r6,r3
	l.sll	r3,r4,r3
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r3,r3,r6
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r5),r3
	l.jr	r9
	l.addi	r1,r1,+00000004
0000468C                                     18 60 01 F0             .`..
00004690 9C 21 FF FC A8 63 1E 80 9D 60 00 00 84 83 00 00 .!...c...`......
000046A0 D4 01 20 00 84 81 00 00 A8 84 00 01 D4 01 20 00 .. ........... .
000046B0 84 81 00 00 D4 03 20 00 44 00 48 00 9C 21 00 04 ...... .D.H..!..

;; fn000046C0: 000046C0
;;   Called from:
;;     0000ACE0 (in fn0000AC84)
;;     0000ACF4 (in fn0000AC84)
;;     0000AD14 (in fn0000AC84)
;;     0000AD28 (in fn0000AC84)
;;     0000C83C (in fn0000C8A0)
;;     0000C868 (in fn0000C8A0)
fn000046C0 proc
	l.movhi	r3,000001F0
	l.addi	r1,r1,-00000008
	l.ori	r3,r3,00001E80
	l.lwz	r4,0(r3)
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.ori	r4,r4,00000002
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4

l000046E8:
	l.lwz	r5,0(r3)
	l.andi	r5,r5,00000002
	l.sfnei	r5,00000000
	l.bf	000046E8
	l.movhi	r4,000001F0

l000046FC:
	l.ori	r3,r4,00001E84
	l.ori	r4,r4,00001E88
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.lwz	r3,0(r4)
	l.sw	4(r1),r3
	l.lwz	r11,4(r1)
	l.lwz	r12,0(r1)
	l.jr	r9
	l.addi	r1,r1,+00000008
00004724             18 80 01 F0 9D 60 00 00 A8 84 1D A8     .....`......
00004730 D4 04 18 00 44 00 48 00 15 00 00 00             ....D.H.....   

;; fn0000473C: 0000473C
;;   Called from:
;;     00004854 (in fn00004844)
;;     00004AF0 (in fn00004A6C)
fn0000473C proc
	l.movhi	r4,00000170
	l.sw	-4(r1),r2
	l.ori	r4,r4,00000004
	l.addi	r1,r1,-00000008
	l.lwz	r5,0(r4)
	l.addi	r2,r0,-00000002
	l.sw	0(r1),r5
	l.andi	r3,r3,00000001
	l.lwz	r5,0(r1)
	l.and	r5,r5,r2
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.or	r3,r3,r5
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r4),r3
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00004788: 00004788
;;   Called from:
;;     00004864 (in fn00004844)
fn00004788 proc
	l.movhi	r3,00000170
	l.ori	r3,r3,00000030

l00004790:
	l.lwz	r4,0(r3)
	l.andi	r4,r4,00000001
	l.sfeqi	r4,00000000
	l.bf	00004790
	l.nop

l000047A4:
	l.jr	r9
	l.nop

;; fn000047AC: 000047AC
;;   Called from:
;;     00004AE0 (in fn00004A6C)
fn000047AC proc
	l.movhi	r4,00000170
	l.sw	-4(r1),r2
	l.lwz	r5,0(r4)
	l.addi	r1,r1,-00000008
	l.addi	r2,r0,-00000020
	l.sw	0(r1),r5
	l.andi	r3,r3,0000001F
	l.lwz	r5,0(r1)
	l.and	r5,r5,r2
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.or	r3,r3,r5
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r4),r3
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn000047F4: 000047F4
;;   Called from:
;;     0000491C (in fn0000490C)
;;     00004AD0 (in fn00004A6C)
;;     00004B00 (in fn00004A6C)
fn000047F4 proc
	l.sfeqi	r3,00000000
	l.bf	00004814
	l.movhi	r4,00000170

l00004800:
	l.sfeqi	r3,00000001
	l.bnf	0000483C
	l.movhi	r3,00000170

l0000480C:
	l.j	00004824
	l.movhi	r5,00001110

l00004814:
	l.ori	r4,r4,00000080
	l.sw	0(r4),r3
	l.j	0000483C
	l.nop

l00004824:
	l.ori	r4,r3,00000080
	l.ori	r5,r5,00001100
	l.ori	r3,r3,00000020
	l.sw	0(r4),r5
	l.addi	r4,r0,+0000000F
	l.sw	0(r3),r4

l0000483C:
	l.jr	r9
	l.nop

;; fn00004844: 00004844
;;   Called from:
;;     0000AC54 (in fn0000AC14)
;;     00010710 (in fn00010570)
fn00004844 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r3,r0,+00000001
	l.addi	r1,r1,-0000000C
	l.jal	0000473C
	l.addi	r2,r0,-00000002
	l.jal	0000AEDC
	l.ori	r3,r0,0000B000
	l.jal	00004788
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B001
	l.movhi	r3,00000170
	l.ori	r3,r3,00000020
	l.lwz	r4,0(r3)
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.and	r4,r4,r2
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r3,r0,+00000000
	l.jal	0000460C
	l.ori	r4,r3,00000000
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001C30
	l.lwz	r4,0(r3)
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.and	r4,r4,r2
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r3,r0,+00000000
	l.jal	0000460C
	l.ori	r4,r3,00000000
	l.addi	r3,r0,+00000001
	l.jal	0000460C
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000002
	l.jal	0000460C
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000003
	l.jal	0000460C
	l.addi	r4,r0,+00000000
	l.addi	r1,r1,+0000000C
	l.ori	r3,r0,0000B002
	l.lwz	r9,-4(r1)
	l.j	0000AEDC
	l.lwz	r2,-8(r1)

;; fn0000490C: 0000490C
;;   Called from:
;;     0000AC5C (in fn0000AC14)
;;     00010810 (in fn00010570)
fn0000490C proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r3,r0,+00000000
	l.addi	r1,r1,-00000008
	l.jal	000047F4
	l.addi	r2,r0,+00000000
	l.jal	0000AEDC
	l.ori	r3,r0,0000B003
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001D40
	l.sw	0(r3),r2
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+00000001
	l.jal	0000B950
	l.ori	r4,r3,00000000
	l.jal	0000AEDC
	l.ori	r3,r0,0000B004
	l.ori	r3,r2,00000000
	l.ori	r4,r2,00000000
	l.jal	000043D0
	l.ori	r5,r2,00000000
	l.addi	r1,r1,+00000008
	l.ori	r3,r0,0000B005
	l.lwz	r9,-4(r1)
	l.j	0000AEDC
	l.lwz	r2,-8(r1)

;; fn00004978: 00004978
;;   Called from:
;;     000120F0 (in fn00010570)
fn00004978 proc
	l.ori	r3,r0,0000B00C
	l.sw	-4(r1),r9
	l.jal	0000AEDC
	l.addi	r1,r1,-00000004
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001C30
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000001
	l.sw	0(r3),r4
	l.addi	r3,r0,+00000000
	l.jal	0000460C
	l.addi	r4,r0,+00000003
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00D
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn000049C0: 000049C0
;;   Called from:
;;     00004B0C (in fn00004A6C)
;;     0001223C (in fn00012214)
fn000049C0 proc
	l.sw	-4(r1),r2
	l.movhi	r2,0000002E
	l.addi	r1,r1,-00000004
	l.ori	r2,r2,00000014
	l.add	r5,r4,r2
	l.movhi	r2,00000170
	l.slli	r4,r4,00000003
	l.ori	r2,r2,000000A4
	l.slli	r5,r5,00000003
	l.add	r4,r4,r2
	l.addi	r2,r0,+00000000
	l.sw	0(r5),r3
	l.sw	0(r4),r2
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00004A00: 00004A00
;;   Called from:
;;     00004B1C (in fn00004A6C)
;;     00004BB4 (in fn00004B44)
fn00004A00 proc
	l.slli	r3,r3,00000004
	l.sw	-4(r1),r2
	l.movhi	r2,00000170
	l.addi	r1,r1,-00000008
	l.add	r3,r3,r2
	l.addi	r4,r4,+00000018
	l.lwz	r6,0(r3)
	l.slli	r5,r5,00000018
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000001
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r4
	l.srai	r5,r5,00000018
	l.xori	r6,r6,0000FFFF
	l.sll	r4,r5,r4
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r4,r4,r6
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00004A6C: 00004A6C
;;   Called from:
;;     00011F24 (in fn00010570)
fn00004A6C proc
	l.sw	-8(r1),r14
	l.ori	r14,r3,00000000
	l.addi	r3,r0,+00000000
	l.sw	-4(r1),r9
	l.addi	r5,r0,+00000001
	l.ori	r4,r3,00000000
	l.sw	-12(r1),r2
	l.addi	r1,r1,-0000000C
	l.jal	000043D0
	l.addi	r2,r0,+00000001
	l.jal	0000AEDC
	l.ori	r3,r0,0000B006
	l.addi	r4,r0,+00000000
	l.jal	0000B950
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.jal	0000AEDC
	l.ori	r3,r0,0000B007
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001D40
	l.sw	0(r3),r2
	l.addi	r3,r0,+00000000
	l.jal	0000460C
	l.ori	r4,r3,00000000
	l.jal	000047F4
	l.addi	r3,r0,+00000000
	l.jal	0000AEDC
	l.ori	r3,r0,0000B008
	l.jal	000047AC
	l.addi	r3,r0,+00000000
	l.jal	0000AEDC
	l.ori	r3,r0,0000B009
	l.jal	0000473C
	l.addi	r3,r0,+00000000
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00A
	l.jal	000047F4
	l.ori	r3,r2,00000000
	l.ori	r3,r14,00000000
	l.jal	000049C0
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000000
	l.ori	r5,r2,00000000
	l.jal	00004A00
	l.ori	r4,r3,00000000
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00B
	l.addi	r1,r1,+0000000C
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn00004B44: 00004B44
;;   Called from:
;;     00012248 (in fn00012214)
fn00004B44 proc
	l.sw	-36(r1),r2
	l.movhi	r2,00000170
	l.sw	-16(r1),r22
	l.slli	r22,r3,00000002
	l.sw	-24(r1),r18
	l.ori	r2,r2,00000080
	l.addi	r18,r0,+00000001
	l.sw	-32(r1),r14
	l.add	r14,r22,r2
	l.sll	r2,r18,r4
	l.lwz	r5,0(r14)
	l.sw	-20(r1),r20
	l.xori	r20,r2,0000FFFF
	l.sw	-4(r1),r9
	l.and	r5,r20,r5
	l.sw	-28(r1),r16
	l.sw	-12(r1),r24
	l.sw	-8(r1),r26
	l.sw	0(r14),r5
	l.movhi	r5,000001F0
	l.addi	r1,r1,-00000024
	l.ori	r5,r5,00001C30
	l.ori	r26,r3,00000000
	l.add	r16,r22,r5
	l.ori	r24,r4,00000000
	l.lwz	r5,0(r16)
	l.and	r5,r20,r5
	l.sw	0(r16),r5
	l.jal	00004A00
	l.ori	r5,r18,00000000
	l.ori	r3,r26,00000000
	l.ori	r4,r24,00000000
	l.jal	000043D0
	l.ori	r5,r18,00000000
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001500
	l.add	r22,r22,r3
	l.lwz	r3,0(r22)
	l.and	r20,r20,r3
	l.sw	0(r22),r20
	l.csync
	l.jal	0000C8A0
	l.ori	r3,r18,00000000
	l.lwz	r3,0(r16)
	l.or	r3,r2,r3
	l.sw	0(r16),r3
	l.lwz	r3,0(r14)
	l.or	r3,r3,r2
	l.sw	0(r14),r3
	l.movhi	r3,00000170
	l.ori	r3,r3,00000020
	l.lwz	r4,0(r3)
	l.or	r2,r2,r4
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000024
	l.lwz	r9,-4(r1)
	l.lwz	r2,-36(r1)
	l.lwz	r14,-32(r1)
	l.lwz	r16,-28(r1)
	l.lwz	r18,-24(r1)
	l.lwz	r20,-20(r1)
	l.lwz	r22,-16(r1)
	l.lwz	r24,-12(r1)
	l.jr	r9
	l.lwz	r26,-8(r1)

;; fn00004C48: 00004C48
;;   Called from:
;;     00012280 (in fn00012214)
fn00004C48 proc
	l.sw	-16(r1),r14
	l.sw	-8(r1),r18
	l.slli	r14,r3,00000002
	l.ori	r18,r3,00000000
	l.movhi	r3,00000170
	l.sw	-20(r1),r2
	l.ori	r3,r3,00000030
	l.ori	r2,r4,00000000
	l.add	r5,r14,r3
	l.addi	r3,r4,+00000010
	l.addi	r4,r0,+00000001
	l.sw	-4(r1),r9
	l.sll	r3,r4,r3
	l.sw	-12(r1),r16
	l.addi	r1,r1,-00000014

l00004C84:
	l.lwz	r4,0(r5)
	l.and	r4,r3,r4
	l.sfeqi	r4,00000000
	l.bf	00004C84
	l.movhi	r4,00000170

l00004C98:
	l.addi	r3,r0,+00000001
	l.sll	r5,r3,r2
	l.ori	r4,r4,00000020
	l.xori	r16,r5,0000FFFF
	l.lwz	r6,0(r4)
	l.and	r6,r16,r6
	l.sw	0(r4),r6
	l.movhi	r6,000001F0
	l.ori	r6,r6,00001500
	l.add	r4,r14,r6
	l.lwz	r6,0(r4)
	l.or	r5,r6,r5
	l.sw	0(r4),r5
	l.csync
	l.jal	0000C8A0
	l.nop
	l.movhi	r4,00000170
	l.movhi	r6,000001F0
	l.ori	r4,r4,00000080
	l.ori	r6,r6,00001C30
	l.add	r3,r14,r4
	l.add	r14,r14,r6
	l.lwz	r4,0(r3)
	l.addi	r5,r0,+00000000
	l.and	r4,r4,r16
	l.sw	0(r3),r4
	l.ori	r4,r2,00000000
	l.lwz	r3,0(r14)
	l.and	r16,r3,r16
	l.ori	r3,r18,00000000
	l.sw	0(r14),r16
	l.addi	r1,r1,+00000014
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.j	000043D0
	l.lwz	r18,-8(r1)
00004D30 D7 E1 4F FC 9C 21 FF FC 9C 21 00 04 85 21 FF FC ..O..!...!...!..
00004D40 00 00 1E D8 15 00 00 00 84 C3 00 00 AC 84 FF FF ................
00004D50 E0 84 30 03 E0 A4 28 04 44 00 48 00 D4 03 28 00 ..0...(.D.H...(.

;; fn00004D60: 00004D60
;;   Called from:
;;     0000694C (in fn000073EC)
fn00004D60 proc
	l.lwz	r5,84(r3)
	l.lwz	r4,88(r3)
	l.sw	-12(r1),r14
	l.ori	r14,r3,00000000
	l.andi	r3,r5,0000000F
	l.sw	-16(r1),r2
	l.andi	r2,r4,0000000F
	l.slli	r3,r3,00000009
	l.add	r2,r2,r2
	l.sw	-4(r1),r9
	l.or	r3,r2,r3
	l.movhi	r2,000001C6
	l.sw	-8(r1),r16
	l.ori	r2,r2,00003310
	l.addi	r1,r1,-00000010

l00004D9C:
	l.lwz	r6,0(r2)
	l.or	r6,r6,r3
	l.sw	0(r2),r6
	l.movhi	r6,000001C6
	l.addi	r2,r2,+00000004
	l.ori	r6,r6,00003334
	l.sfne	r2,r6
	l.bf	00004D9C
	l.nop

l00004DC0:
	l.andi	r3,r4,000000F0
	l.andi	r2,r5,000000F0
	l.srli	r3,r3,00000003
	l.slli	r2,r2,00000005
	l.or	r3,r3,r2
	l.movhi	r2,000001C6
	l.ori	r2,r2,00003390

l00004DDC:
	l.lwz	r6,0(r2)
	l.movhi	r8,000001C6
	l.or	r6,r6,r3
	l.ori	r8,r8,000033B4
	l.sw	0(r2),r6
	l.addi	r2,r2,+00000004
	l.sfne	r2,r8
	l.bf	00004DDC
	l.nop

l00004E00:
	l.andi	r3,r4,00000F00
	l.andi	r2,r5,00000F00
	l.srli	r3,r3,00000007
	l.slli	r2,r2,00000001
	l.or	r3,r3,r2
	l.movhi	r2,000001C6
	l.ori	r2,r2,00003410

l00004E1C:
	l.lwz	r6,0(r2)
	l.movhi	r13,000001C6
	l.or	r6,r6,r3
	l.ori	r13,r13,00003434
	l.sw	0(r2),r6
	l.addi	r2,r2,+00000004
	l.sfne	r2,r13
	l.bf	00004E1C
	l.movhi	r6,000001C6

l00004E40:
	l.andi	r3,r4,0000F000
	l.andi	r2,r5,0000F000
	l.srli	r3,r3,0000000B
	l.srli	r2,r2,00000003
	l.or	r2,r3,r2
	l.ori	r6,r6,00003490

l00004E58:
	l.lwz	r3,0(r6)
	l.movhi	r15,000001C6
	l.or	r3,r3,r2
	l.ori	r15,r15,000034B4
	l.sw	0(r6),r3
	l.addi	r6,r6,+00000004
	l.sfne	r6,r15
	l.bf	00004E58
	l.movhi	r8,0000000F

l00004E7C:
	l.movhi	r2,000001C6
	l.ori	r7,r2,00003100
	l.and	r12,r5,r8
	l.movhi	r16,0000FBFF
	l.lwz	r3,0(r7)
	l.ori	r16,r16,0000FFFF
	l.srli	r12,r12,00000010
	l.and	r8,r4,r8
	l.and	r3,r3,r16
	l.slli	r13,r12,00000009
	l.srli	r8,r8,00000010
	l.sw	0(r7),r3
	l.ori	r3,r2,00003334
	l.or	r8,r13,r8
	l.lwz	r11,0(r3)
	l.movhi	r15,000000F0
	l.or	r11,r8,r11
	l.movhi	r16,000000F0
	l.sw	0(r3),r11
	l.ori	r3,r2,00003338
	l.slli	r12,r12,00000019
	l.lwz	r11,0(r3)
	l.or	r8,r8,r11
	l.and	r11,r5,r15
	l.sw	0(r3),r8
	l.srli	r11,r11,00000014
	l.and	r8,r4,r16
	l.ori	r3,r2,000033B4
	l.slli	r15,r11,00000009
	l.srli	r8,r8,00000014
	l.lwz	r13,0(r3)
	l.slli	r11,r11,00000019
	l.or	r8,r15,r8
	l.or	r13,r8,r13
	l.sw	0(r3),r13
	l.ori	r3,r2,000033B8
	l.lwz	r13,0(r3)
	l.or	r8,r8,r13
	l.movhi	r13,00000F00
	l.sw	0(r3),r8
	l.and	r8,r5,r13
	l.and	r13,r4,r13
	l.srli	r8,r8,00000018
	l.srli	r13,r13,00000018
	l.ori	r3,r2,00003434
	l.slli	r17,r8,00000009
	l.lwz	r15,0(r3)
	l.srli	r5,r5,0000001C
	l.or	r13,r17,r13
	l.srli	r4,r4,0000001C
	l.or	r15,r13,r15
	l.slli	r8,r8,00000019
	l.sw	0(r3),r15
	l.ori	r3,r2,00003438
	l.lwz	r15,0(r3)
	l.or	r13,r13,r15
	l.movhi	r15,00000400
	l.sw	0(r3),r13
	l.slli	r13,r5,00000009
	l.lwz	r3,0(r6)
	l.slli	r5,r5,00000019
	l.or	r4,r13,r4
	l.or	r3,r4,r3
	l.sw	0(r6),r3
	l.ori	r3,r2,000034B8
	l.lwz	r6,0(r3)
	l.or	r4,r4,r6
	l.sw	0(r3),r4
	l.ori	r3,r2,0000333C
	l.lwz	r4,0(r3)
	l.or	r12,r12,r4
	l.sw	0(r3),r12
	l.ori	r3,r2,000033BC
	l.lwz	r4,0(r3)
	l.or	r11,r11,r4
	l.sw	0(r3),r11
	l.ori	r3,r2,0000343C
	l.lwz	r4,0(r3)
	l.or	r8,r8,r4
	l.sw	0(r3),r8
	l.ori	r3,r2,000034BC
	l.ori	r2,r2,00003240
	l.lwz	r4,0(r3)
	l.or	r5,r5,r4
	l.sw	0(r3),r5
	l.lwz	r3,0(r7)
	l.or	r3,r3,r15
	l.sw	0(r7),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00000001
	l.lwz	r3,80(r14)
	l.andi	r4,r3,000000F0
	l.slli	r4,r4,00000004

l00004FF0:
	l.lwz	r5,0(r2)
	l.movhi	r16,000001C6
	l.or	r5,r5,r4
	l.ori	r16,r16,00003268
	l.sw	0(r2),r5
	l.addi	r2,r2,+00000004
	l.sfne	r2,r16
	l.bf	00004FF0
	l.andi	r5,r3,0000000F

l00005014:
	l.movhi	r2,000001C6
	l.ori	r4,r2,00003218
	l.slli	r5,r5,00000008
	l.lwz	r6,0(r4)
	l.or	r5,r5,r6
	l.andi	r6,r3,00000F00
	l.sw	0(r4),r5
	l.ori	r4,r2,0000321C
	l.andi	r3,r3,0000F000
	l.lwz	r5,0(r4)
	l.ori	r2,r2,00003280
	l.or	r5,r6,r5
	l.srli	r3,r3,00000004
	l.sw	0(r4),r5
	l.lwz	r4,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.addi	r1,r1,+00000010
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)
00005070 44 00 48 00 9D 60 00 01                         D.H..`..       

;; fn00005078: 00005078
;;   Called from:
;;     00007C84 (in fn00007AE0)
fn00005078 proc
	l.movhi	r3,000001C6
	l.addi	r5,r0,+0000018F
	l.ori	r4,r3,0000200C
	l.movhi	r8,00000096
	l.sw	0(r4),r5
	l.ori	r4,r3,00002090
	l.movhi	r5,00000001
	l.movhi	r7,00000258
	l.sw	0(r4),r5
	l.ori	r4,r3,00002098
	l.addi	r5,r0,+00000001
	l.ori	r7,r7,0000000D
	l.sw	0(r4),r5
	l.movhi	r5,0000012C
	l.ori	r4,r3,00002010
	l.ori	r5,r5,0000000D
	l.movhi	r6,000000C8
	l.sw	0(r4),r5
	l.ori	r4,r3,00002014
	l.ori	r5,r8,00000104
	l.ori	r6,r6,00000190
	l.sw	0(r4),r5
	l.ori	r4,r3,00002018
	l.movhi	r5,00000200
	l.sw	0(r4),r7
	l.ori	r4,r3,0000201C
	l.ori	r5,r5,0000000D
	l.sw	0(r4),r6
	l.ori	r4,r3,00002020
	l.ori	r11,r3,00002028
	l.sw	0(r4),r5
	l.movhi	r5,00000060
	l.ori	r4,r3,00002024
	l.ori	r5,r5,00000100
	l.movhi	r12,00000020
	l.sw	0(r4),r5
	l.movhi	r4,00000100
	l.ori	r12,r12,00000080
	l.ori	r5,r4,0000000D
	l.ori	r8,r8,0000000D
	l.sw	0(r11),r5
	l.ori	r11,r3,0000202C
	l.ori	r4,r4,00000009
	l.sw	0(r11),r12
	l.movhi	r12,0000076C
	l.ori	r11,r3,00002030
	l.ori	r12,r12,0000000D
	l.sw	0(r11),r12
	l.movhi	r12,000003E8
	l.ori	r11,r3,00002034
	l.ori	r12,r12,000005DC
	l.sw	0(r11),r12
	l.ori	r11,r3,00002038
	l.sw	0(r11),r8
	l.movhi	r11,00000064
	l.ori	r8,r3,0000203C
	l.ori	r11,r11,00000078
	l.sw	0(r8),r11
	l.ori	r8,r3,00002040
	l.ori	r11,r3,00002044
	l.sw	0(r8),r4
	l.movhi	r8,00000040
	l.ori	r4,r8,00000080
	l.ori	r8,r8,00000100
	l.sw	0(r11),r4
	l.ori	r11,r3,00002048
	l.sw	0(r11),r5
	l.ori	r11,r3,0000204C
	l.sw	0(r11),r4
	l.ori	r11,r3,00002050
	l.sw	0(r11),r5
	l.ori	r5,r3,00002054
	l.sw	0(r5),r4
	l.movhi	r4,00000400
	l.ori	r5,r3,00002058
	l.ori	r11,r4,00000009
	l.ori	r4,r4,00000960
	l.sw	0(r5),r11
	l.ori	r5,r3,0000205C
	l.sw	0(r5),r8
	l.movhi	r8,00000D48
	l.ori	r5,r3,00002060
	l.ori	r8,r8,0000030D
	l.sw	0(r5),r8
	l.ori	r5,r3,00002064
	l.sw	0(r5),r4
	l.ori	r4,r3,00002068
	l.ori	r3,r3,0000206C
	l.sw	0(r4),r7
	l.sw	0(r3),r6
	l.jr	r9
	l.nop

;; fn000051E8: 000051E8
;;   Called from:
;;     00007408 (in fn000073EC)
fn000051E8 proc
	l.lwz	r4,92(r3)
	l.sw	-44(r1),r2
	l.sw	-40(r1),r14
	l.sw	-20(r1),r24
	l.sw	-12(r1),r28
	l.sw	-4(r1),r9
	l.sw	-36(r1),r16
	l.sw	-32(r1),r18
	l.sw	-28(r1),r20
	l.sw	-24(r1),r22
	l.sw	-16(r1),r26
	l.sw	-8(r1),r30
	l.lwz	r14,0(r3)
	l.andi	r4,r4,00000002
	l.addi	r1,r1,-00000060
	l.ori	r28,r3,00000000
	l.srli	r2,r14,00000001
	l.sfeqi	r4,00000000
	l.bf	000052D0
	l.lwz	r24,4(r3)

l00005238:
	l.lwz	r4,44(r3)
	l.lwz	r5,40(r3)
	l.srli	r7,r4,00000017
	l.srli	r18,r5,0000000B
	l.lwz	r8,48(r3)
	l.andi	r7,r7,0000001F
	l.andi	r18,r18,0000000F
	l.sw	0(r1),r7
	l.srli	r7,r4,00000014
	l.srli	r20,r5,00000015
	l.srli	r11,r5,0000000F
	l.srli	r6,r5,00000006
	l.srli	r29,r4,0000000F
	l.srli	r12,r8,0000000C
	l.sw	8(r1),r18
	l.andi	r18,r7,00000007
	l.srli	r7,r4,00000006
	l.srli	r16,r4,0000000B
	l.andi	r20,r20,00000007
	l.andi	r11,r11,0000003F
	l.andi	r6,r6,0000001F
	l.andi	r5,r5,0000003F
	l.andi	r29,r29,0000001F
	l.andi	r7,r7,0000001F
	l.andi	r4,r4,0000003F
	l.andi	r12,r12,000001FF
	l.andi	r8,r8,00000FFF
	l.sw	48(r1),r20
	l.sw	20(r1),r11
	l.sw	4(r1),r6
	l.sw	24(r1),r5
	l.sw	16(r1),r29
	l.andi	r16,r16,0000000F
	l.sw	12(r1),r7
	l.sw	28(r1),r4
	l.sw	36(r1),r12
	l.j	00005D6C
	l.sw	32(r1),r8

l000052D0:
	l.sfnei	r24,00000003
	l.bf	000054E8
	l.sfnei	r24,00000002

l000052DC:
	l.addi	r16,r0,+00000032
	l.addi	r4,r0,+000003E8
	l.mul	r16,r2,r16
	l.jal	0001003C
	l.ori	r3,r16,00000000
	l.sub	r4,r0,r11
	l.ori	r3,r16,00000000
	l.or	r11,r4,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r18,r11,0000001F
	l.slli	r5,r2,00000003
	l.add	r16,r2,r2
	l.add	r11,r18,r11
	l.add	r16,r16,r5
	l.andi	r11,r11,000000FF
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.sw	20(r1),r11
	l.sub	r5,r0,r11
	l.ori	r3,r16,00000000
	l.or	r5,r5,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r18,r5,0000001F
	l.add	r5,r18,r11
	l.andi	r5,r5,000000FF
	l.sfleui	r5,00000001
	l.bnf	00005360
	l.sw	8(r1),r5

l00005358:
	l.addi	r3,r0,+00000002
	l.sw	8(r1),r3

l00005360:
	l.slli	r16,r2,00000004
	l.addi	r4,r0,+000003E8
	l.sub	r16,r16,r2
	l.jal	0001003C
	l.ori	r3,r16,00000000
	l.ori	r3,r16,00000000
	l.addi	r16,r0,+00000035
	l.sub	r6,r0,r11
	l.mul	r16,r2,r16
	l.or	r6,r6,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r20,r6,0000001F
	l.ori	r3,r16,00000000
	l.add	r6,r20,r11
	l.addi	r4,r0,+000003E8
	l.andi	r6,r6,000000FF
	l.jal	0001003C
	l.sw	4(r1),r6
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r5,r7,r11
	l.addi	r4,r0,+000003E8
	l.srli	r22,r5,0000001F
	l.jal	0000FEDC
	l.slli	r16,r2,00000003
	l.add	r5,r22,r11
	l.ori	r3,r16,00000000
	l.andi	r5,r5,000000FF
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.sw	24(r1),r5
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r6,r7,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r6,0000001F
	l.add	r6,r22,r11
	l.andi	r6,r6,000000FF
	l.sfleui	r6,00000001
	l.bf	00006280
	l.sw	0(r1),r6

l0000540C:
	l.ori	r18,r6,00000000
	l.sw	16(r1),r6

l00005414:
	l.lwz	r4,4(r1)
	l.sfleui	r4,00000001
	l.bf	00005428
	l.addi	r16,r0,+00000002

l00005424:
	l.ori	r16,r4,00000000

l00005428:
	l.slli	r7,r2,00000005
	l.add	r8,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r7,r8,r7
	l.addi	r20,r0,+00001E78
	l.add	r3,r7,r2
	l.jal	0001003C
	l.mul	r20,r2,r20
	l.addi	r7,r0,+00000026
	l.sub	r4,r0,r11
	l.mul	r3,r2,r7
	l.or	r4,r4,r11
	l.srli	r22,r4,0000001F
	l.jal	0000FEDC
	l.addi	r4,r0,+000003E8
	l.add	r4,r22,r11
	l.ori	r3,r20,00000000
	l.andi	r4,r4,000000FF
	l.sw	28(r1),r4
	l.jal	0001003C
	l.addi	r4,r0,+000003E8
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+0000015E
	l.sub	r8,r0,r11
	l.mul	r20,r2,r20
	l.or	r8,r8,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r8,0000001F
	l.ori	r3,r20,00000000
	l.add	r8,r22,r11
	l.addi	r4,r0,+000003E8
	l.srli	r8,r8,00000005
	l.andi	r8,r8,0000FFFF
	l.jal	0001003C
	l.sw	32(r1),r8
	l.sub	r12,r0,r11
	l.ori	r3,r20,00000000
	l.or	r12,r12,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r12,0000001F
	l.lwz	r6,4(r1)
	l.add	r12,r22,r11
	l.sw	12(r1),r6
	l.andi	r12,r12,0000FFFF
	l.j	00005CE0
	l.sw	36(r1),r12

l000054E8:
	l.bf	000056F8
	l.sfnei	r24,00000006

l000054F0:
	l.addi	r16,r0,+00000032
	l.addi	r4,r0,+000003E8
	l.mul	r16,r2,r16
	l.ori	r3,r16,00000000
	l.jal	0001003C
	l.add	r20,r2,r2
	l.sub	r4,r0,r11
	l.ori	r3,r16,00000000
	l.or	r11,r4,r11
	l.slli	r16,r2,00000003
	l.srli	r18,r11,0000001F
	l.jal	0000FEDC
	l.addi	r4,r0,+000003E8
	l.add	r11,r18,r11
	l.add	r18,r20,r16
	l.andi	r11,r11,000000FF
	l.ori	r3,r18,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.sw	20(r1),r11
	l.sub	r5,r0,r11
	l.ori	r3,r18,00000000
	l.or	r11,r5,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r11,0000001F
	l.slli	r6,r2,00000002
	l.add	r11,r22,r11
	l.slli	r22,r2,00000004
	l.andi	r11,r11,000000FF
	l.addi	r4,r0,+000003E8
	l.add	r18,r6,r22
	l.sw	8(r1),r11
	l.ori	r3,r18,00000000
	l.jal	0001003C
	l.sub	r22,r22,r2
	l.sub	r6,r0,r11
	l.ori	r3,r18,00000000
	l.or	r11,r6,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r26,r11,0000001F
	l.slli	r7,r2,00000006
	l.add	r11,r26,r11
	l.addi	r4,r0,+000003E8
	l.add	r18,r7,r2
	l.andi	r11,r11,000000FF
	l.ori	r3,r18,00000000
	l.jal	0001003C
	l.sw	4(r1),r11
	l.sub	r7,r0,r11
	l.ori	r3,r18,00000000
	l.or	r11,r7,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r26,r11,0000001F
	l.ori	r3,r16,00000000
	l.add	r11,r26,r11
	l.addi	r4,r0,+000003E8
	l.andi	r11,r11,000000FF
	l.add	r20,r20,r2
	l.jal	0001003C
	l.sw	24(r1),r11
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r18,r7,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r18,r18,0000001F
	l.ori	r3,r22,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.add	r18,r18,r11
	l.ori	r3,r22,00000000
	l.sub	r16,r0,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.or	r16,r16,r11
	l.slli	r7,r20,00000004
	l.srli	r16,r16,0000001F
	l.addi	r4,r0,+000003E8
	l.sub	r20,r7,r20
	l.add	r16,r16,r11
	l.ori	r3,r20,00000000
	l.jal	0001003C
	l.andi	r18,r18,000000FF
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+00001E78
	l.sub	r4,r0,r11
	l.mul	r20,r2,r20
	l.or	r4,r4,r11
	l.andi	r16,r16,000000FF
	l.srli	r22,r4,0000001F
	l.jal	0000FEDC
	l.addi	r4,r0,+000003E8
	l.add	r4,r22,r11
	l.ori	r3,r20,00000000
	l.andi	r4,r4,000000FF
	l.sw	28(r1),r4
	l.jal	0001003C
	l.addi	r4,r0,+000003E8
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+00000148
	l.sub	r8,r0,r11
	l.mul	r20,r2,r20
	l.or	r8,r8,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r8,0000001F
	l.ori	r3,r20,00000000
	l.add	r8,r22,r11
	l.addi	r4,r0,+000003E8
	l.srli	r8,r8,00000005
	l.andi	r8,r8,0000FFFF
	l.jal	0000FEDC
	l.sw	32(r1),r8
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r13,r0,r11
	l.sw	16(r1),r18
	l.or	r8,r13,r11
	l.sw	12(r1),r16
	l.srli	r13,r8,0000001F
	l.sw	0(r1),r24
	l.add	r12,r22,r13
	l.andi	r12,r12,0000FFFF
	l.j	00005CE0
	l.sw	36(r1),r12

l000056F8:
	l.bf	000059C4
	l.sfnei	r24,00000007

l00005700:
	l.addi	r16,r0,+00000032
	l.addi	r4,r0,+000003E8
	l.mul	r16,r2,r16
	l.jal	0000FEDC
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.ori	r3,r16,00000000
	l.jal	0001003C
	l.ori	r18,r11,00000000
	l.sub	r4,r0,r11
	l.or	r4,r4,r11
	l.srli	r4,r4,0000001F
	l.add	r11,r18,r4
	l.andi	r11,r11,000000FF
	l.sfleui	r11,00000003
	l.bnf	0000574C
	l.sw	20(r1),r11

l00005744:
	l.addi	r3,r0,+00000004
	l.sw	20(r1),r3

l0000574C:
	l.slli	r5,r2,00000003
	l.add	r16,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r16,r16,r5
	l.jal	0000FEDC
	l.ori	r3,r16,00000000
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r18,r11,00000000
	l.sub	r5,r0,r11
	l.or	r5,r5,r11
	l.srli	r5,r5,0000001F
	l.add	r5,r18,r5
	l.andi	r5,r5,000000FF
	l.sfeqi	r5,00000000
	l.bnf	0000579C
	l.sw	8(r1),r5

l00005794:
	l.addi	r4,r0,+00000001
	l.sw	8(r1),r4

l0000579C:
	l.add	r16,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r16,r16,r2
	l.slli	r16,r16,00000003
	l.jal	0000FEDC
	l.ori	r3,r16,00000000
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r20,r11,00000000
	l.sub	r6,r0,r11
	l.or	r6,r6,r11
	l.srli	r6,r6,0000001F
	l.add	r6,r20,r6
	l.andi	r6,r6,000000FF
	l.sfleui	r6,00000001
	l.bnf	000057EC
	l.sw	4(r1),r6

l000057E4:
	l.addi	r6,r0,+00000002
	l.sw	4(r1),r6

l000057EC:
	l.addi	r16,r0,+00000046
	l.addi	r4,r0,+000003E8
	l.mul	r16,r2,r16
	l.jal	0001003C
	l.ori	r3,r16,00000000
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r11,r7,r11
	l.addi	r4,r0,+000003E8
	l.srli	r22,r11,0000001F
	l.jal	0000FEDC
	l.slli	r16,r2,00000003
	l.add	r11,r22,r11
	l.ori	r3,r16,00000000
	l.andi	r11,r11,000000FF
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.sw	24(r1),r11
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r6,r7,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r6,0000001F
	l.add	r6,r22,r11
	l.andi	r6,r6,000000FF
	l.sfeqi	r6,00000000
	l.bf	00006294
	l.sw	0(r1),r6

l00005860:
	l.sfleui	r6,00000001
	l.bf	000062A0
	l.addi	r18,r0,+00000002

l0000586C:
	l.ori	r18,r6,00000000
	l.sw	16(r1),r6

l00005874:
	l.slli	r20,r2,00000004
	l.addi	r4,r0,+000003E8
	l.sub	r20,r20,r2
	l.jal	0000FEDC
	l.ori	r3,r20,00000000
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r16,r0,r11
	l.or	r6,r16,r11
	l.srli	r16,r6,0000001F
	l.add	r16,r22,r16
	l.andi	r16,r16,000000FF
	l.sfleui	r16,00000001
	l.bnf	000058BC
	l.add	r20,r2,r2

l000058B8:
	l.addi	r16,r0,+00000002

l000058BC:
	l.addi	r4,r0,+000003E8
	l.add	r20,r20,r2
	l.slli	r7,r20,00000003
	l.add	r20,r20,r7
	l.jal	0000FEDC
	l.ori	r3,r20,00000000
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r7,r0,r11
	l.or	r6,r7,r11
	l.srli	r7,r6,0000001F
	l.add	r7,r22,r7
	l.andi	r7,r7,000000FF
	l.sfleui	r7,00000001
	l.bnf	0000590C
	l.sw	12(r1),r7

l00005904:
	l.addi	r3,r0,+00000002
	l.sw	12(r1),r3

l0000590C:
	l.add	r20,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r20,r20,r2
	l.slli	r8,r20,00000003
	l.sub	r20,r8,r20
	l.add	r20,r20,r20
	l.jal	0000FEDC
	l.ori	r3,r20,00000000
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+00000F3C
	l.addi	r4,r0,+000003E8
	l.mul	r20,r2,r20
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r8,r0,r11
	l.ori	r3,r20,00000000
	l.or	r7,r8,r11
	l.srli	r8,r7,0000001F
	l.add	r4,r22,r8
	l.andi	r4,r4,000000FF
	l.sw	28(r1),r4
	l.jal	0001003C
	l.addi	r4,r0,+000003E8
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+000000D2
	l.sub	r8,r0,r11
	l.mul	r20,r2,r20
	l.or	r7,r8,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r7,0000001F
	l.ori	r3,r20,00000000
	l.add	r7,r22,r11
	l.addi	r4,r0,+000003E8
	l.srli	r8,r7,00000005
	l.andi	r8,r8,0000FFFF
	l.jal	0000FEDC
	l.sw	32(r1),r8
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r13,r0,r11
	l.or	r7,r13,r11
	l.j	00005C80
	l.srli	r13,r7,0000001F

l000059C4:
	l.bf	00005C90
	l.addi	r3,r0,+00000062

l000059CC:
	l.addi	r16,r0,+00000032
	l.addi	r4,r0,+000003E8
	l.mul	r16,r2,r16
	l.jal	0000FEDC
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.ori	r3,r16,00000000
	l.jal	0001003C
	l.ori	r18,r11,00000000
	l.sub	r4,r0,r11
	l.or	r4,r4,r11
	l.srli	r4,r4,0000001F
	l.add	r11,r18,r4
	l.andi	r11,r11,000000FF
	l.sfleui	r11,00000003
	l.bnf	00005A18
	l.sw	20(r1),r11

l00005A10:
	l.addi	r4,r0,+00000004
	l.sw	20(r1),r4

l00005A18:
	l.slli	r5,r2,00000003
	l.add	r16,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r16,r16,r5
	l.jal	0000FEDC
	l.ori	r3,r16,00000000
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r18,r11,00000000
	l.sub	r5,r0,r11
	l.or	r5,r5,r11
	l.srli	r5,r5,0000001F
	l.add	r5,r18,r5
	l.andi	r5,r5,000000FF
	l.sfeqi	r5,00000000
	l.bnf	00005A68
	l.sw	8(r1),r5

l00005A60:
	l.addi	r6,r0,+00000001
	l.sw	8(r1),r6

l00005A68:
	l.add	r16,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r16,r16,r2
	l.slli	r16,r16,00000003
	l.jal	0000FEDC
	l.ori	r3,r16,00000000
	l.ori	r3,r16,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r20,r11,00000000
	l.sub	r6,r0,r11
	l.or	r5,r6,r11
	l.srli	r6,r5,0000001F
	l.add	r6,r20,r6
	l.andi	r6,r6,000000FF
	l.sfleui	r6,00000001
	l.bnf	00005AB8
	l.sw	4(r1),r6

l00005AB0:
	l.addi	r3,r0,+00000002
	l.sw	4(r1),r3

l00005AB8:
	l.addi	r16,r0,+00000046
	l.addi	r4,r0,+000003E8
	l.mul	r16,r2,r16
	l.jal	0001003C
	l.ori	r3,r16,00000000
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r11,r7,r11
	l.addi	r4,r0,+000003E8
	l.srli	r22,r11,0000001F
	l.jal	0000FEDC
	l.slli	r16,r2,00000003
	l.add	r11,r22,r11
	l.ori	r3,r16,00000000
	l.andi	r11,r11,000000FF
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.sw	24(r1),r11
	l.sub	r7,r0,r11
	l.ori	r3,r16,00000000
	l.or	r11,r7,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r11,0000001F
	l.add	r11,r22,r11
	l.andi	r11,r11,000000FF
	l.sfleui	r11,00000001
	l.bf	000062A8
	l.sw	0(r1),r11

l00005B2C:
	l.ori	r18,r11,00000000
	l.sw	16(r1),r11

l00005B34:
	l.slli	r20,r2,00000004
	l.addi	r4,r0,+000003E8
	l.sub	r20,r20,r2
	l.jal	0000FEDC
	l.ori	r3,r20,00000000
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r16,r0,r11
	l.or	r6,r16,r11
	l.srli	r16,r6,0000001F
	l.add	r16,r22,r16
	l.andi	r16,r16,000000FF
	l.sfleui	r16,00000001
	l.bnf	00005B7C
	l.add	r20,r2,r2

l00005B78:
	l.addi	r16,r0,+00000002

l00005B7C:
	l.addi	r4,r0,+000003E8
	l.add	r20,r20,r2
	l.slli	r7,r20,00000003
	l.add	r20,r20,r7
	l.jal	0000FEDC
	l.ori	r3,r20,00000000
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r7,r0,r11
	l.or	r6,r7,r11
	l.srli	r7,r6,0000001F
	l.add	r7,r22,r7
	l.andi	r7,r7,000000FF
	l.sfleui	r7,00000001
	l.bnf	00005BCC
	l.sw	12(r1),r7

l00005BC4:
	l.addi	r4,r0,+00000002
	l.sw	12(r1),r4

l00005BCC:
	l.add	r20,r2,r2
	l.addi	r4,r0,+000003E8
	l.add	r20,r20,r2
	l.slli	r7,r20,00000003
	l.sub	r20,r7,r20
	l.add	r20,r20,r20
	l.jal	0000FEDC
	l.ori	r3,r20,00000000
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+00000F3C
	l.addi	r4,r0,+000003E8
	l.mul	r20,r2,r20
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r7,r0,r11
	l.ori	r3,r20,00000000
	l.or	r6,r7,r11
	l.srli	r7,r6,0000001F
	l.add	r4,r22,r7
	l.andi	r4,r4,000000FF
	l.sw	28(r1),r4
	l.jal	0001003C
	l.addi	r4,r0,+000003E8
	l.ori	r3,r20,00000000
	l.addi	r20,r0,+000000D2
	l.sub	r8,r0,r11
	l.mul	r20,r2,r20
	l.or	r11,r8,r11
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.srli	r22,r11,0000001F
	l.ori	r3,r20,00000000
	l.add	r11,r22,r11
	l.addi	r4,r0,+000003E8
	l.srli	r8,r11,00000005
	l.andi	r8,r8,0000FFFF
	l.jal	0000FEDC
	l.sw	32(r1),r8
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0001003C
	l.ori	r22,r11,00000000
	l.sub	r13,r0,r11
	l.or	r6,r13,r11
	l.srli	r13,r6,0000001F

l00005C80:
	l.add	r12,r22,r13
	l.andi	r12,r12,0000FFFF
	l.j	00005CE0
	l.sw	36(r1),r12

l00005C90:
	l.addi	r6,r0,+00000080
	l.addi	r4,r0,+0000000A
	l.sw	32(r1),r3
	l.addi	r3,r0,+00000006
	l.sw	36(r1),r6
	l.sw	12(r1),r3
	l.addi	r6,r0,+00000003
	l.sw	0(r1),r4
	l.addi	r4,r0,+0000000E
	l.ori	r18,r6,00000000
	l.sw	16(r1),r6
	l.sw	28(r1),r4
	l.addi	r6,r0,+00000010
	l.addi	r3,r0,+00000014
	l.lwz	r4,12(r1)
	l.addi	r16,r0,+00000008
	l.sw	20(r1),r6
	l.sw	24(r1),r3
	l.sw	4(r1),r4
	l.sw	8(r1),r18

l00005CE0:
	l.lwz	r4,4(r1)
	l.lwz	r6,24(r1)
	l.movhi	r3,00000040
	l.slli	r13,r4,00000006
	l.or	r15,r6,r3
	l.lwz	r6,8(r1)
	l.or	r13,r15,r13
	l.slli	r15,r6,0000000B
	l.lwz	r3,20(r1)
	l.lwz	r4,12(r1)
	l.or	r13,r13,r15
	l.slli	r15,r3,0000000F
	l.lwz	r3,16(r1)
	l.lwz	r6,28(r1)
	l.or	r13,r13,r15
	l.slli	r15,r4,00000006
	l.sw	40(r28),r13
	l.slli	r13,r16,0000000B
	l.lwz	r4,0(r1)
	l.or	r13,r15,r13
	l.slli	r15,r3,0000000F
	l.or	r13,r13,r6
	l.lwz	r6,36(r1)
	l.or	r13,r13,r15
	l.slli	r15,r18,00000014
	l.lwz	r3,32(r1)
	l.or	r13,r13,r15
	l.slli	r15,r4,00000017
	l.addi	r4,r0,+00000002
	l.or	r13,r13,r15
	l.sw	48(r1),r4
	l.sw	44(r28),r13
	l.slli	r13,r6,0000000C
	l.or	r13,r13,r3
	l.sw	48(r28),r13

l00005D6C:
	l.sfeqi	r24,00000003
	l.bf	00005E3C
	l.sfgtui	r24,00000003

l00005D78:
	l.bf	00005D94
	l.sfeqi	r24,00000006

l00005D80:
	l.sfeqi	r24,00000002
	l.bnf	00006010
	l.ori	r3,r2,00000000

l00005D8C:
	l.j	00005DAC
	l.addi	r2,r0,+00000190

l00005D94:
	l.bf	00005ED4
	l.sfeqi	r24,00000007

l00005D9C:
	l.bnf	00006010
	l.ori	r3,r2,00000000

l00005DA4:
	l.j	00005F70
	l.addi	r2,r0,+000000C8

l00005DAC:
	l.addi	r4,r0,+0000000F
	l.mul	r2,r14,r2
	l.jal	0000FEDC
	l.addi	r2,r2,+00000001
	l.sw	40(r1),r2
	l.addi	r2,r0,+000001F4
	l.andi	r11,r11,000000FF
	l.mul	r3,r14,r2
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.sw	44(r1),r11
	l.addi	r13,r0,+000000C8
	l.addi	r19,r0,+00000003
	l.mul	r13,r14,r13
	l.addi	r15,r18,+00000005
	l.addi	r22,r0,+00000000
	l.addi	r12,r13,+00000001
	l.addi	r16,r16,+00000005
	l.addi	r13,r0,+00000001
	l.andi	r18,r15,000000FF
	l.ori	r4,r24,00000000
	l.lwz	r26,28(r28)
	l.addi	r8,r11,+00000001
	l.addi	r14,r14,+00000001
	l.andi	r16,r16,000000FF
	l.addi	r17,r0,+00000005
	l.addi	r7,r0,+00000004
	l.ori	r15,r19,00000000
	l.addi	r2,r0,+0000000C
	l.ori	r27,r19,00000000
	l.ori	r25,r19,00000000
	l.ori	r5,r13,00000000
	l.ori	r20,r22,00000000
	l.ori	r24,r22,00000000
	l.j	0000606C
	l.addi	r21,r0,+00000263

l00005E3C:
	l.ori	r3,r2,00000000
	l.addi	r2,r0,+000001F4
	l.addi	r4,r0,+0000000F
	l.mul	r2,r14,r2
	l.jal	0000FEDC
	l.addi	r2,r2,+00000001
	l.sw	40(r1),r2
	l.addi	r2,r0,+00000168
	l.andi	r11,r11,000000FF
	l.mul	r3,r14,r2
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.sw	44(r1),r11
	l.addi	r13,r0,+000000C8
	l.addi	r7,r0,+00000004
	l.mul	r13,r14,r13
	l.addi	r15,r18,+00000006
	l.addi	r16,r16,+00000006
	l.addi	r17,r0,+00000005
	l.addi	r22,r0,+00000000
	l.addi	r12,r13,+00000001
	l.andi	r18,r15,000000FF
	l.lwz	r26,28(r28)
	l.ori	r15,r24,00000000
	l.addi	r8,r11,+00000001
	l.addi	r14,r14,+00000001
	l.andi	r16,r16,000000FF
	l.ori	r19,r17,00000000
	l.addi	r2,r0,+0000000C
	l.ori	r4,r7,00000000
	l.ori	r27,r7,00000000
	l.addi	r25,r0,+00000006
	l.addi	r13,r0,+00000002
	l.ori	r5,r7,00000000
	l.ori	r20,r22,00000000
	l.addi	r24,r0,+00000018
	l.j	0000606C
	l.addi	r21,r0,+00001C70

l00005ED4:
	l.ori	r3,r2,00000000
	l.addi	r2,r0,+000000C8
	l.addi	r4,r0,+0000001E
	l.mul	r2,r14,r2
	l.jal	0000FEDC
	l.addi	r2,r2,+00000001
	l.sw	40(r1),r2
	l.add	r2,r14,r14
	l.andi	r11,r11,000000FF
	l.add	r6,r2,r14
	l.addi	r4,r0,+000003E8
	l.slli	r13,r6,00000005
	l.sw	44(r1),r11
	l.addi	r16,r16,+00000005
	l.add	r6,r6,r13
	l.addi	r22,r0,+00000003
	l.add	r3,r6,r14
	l.jal	0000FEDC
	l.lwz	r20,36(r28)
	l.slli	r13,r14,00000003
	l.addi	r17,r0,+00000005
	l.addi	r15,r0,+00000002
	l.add	r13,r2,r13
	l.addi	r8,r11,+00000001
	l.add	r13,r13,r14
	l.andi	r16,r16,000000FF
	l.addi	r12,r13,+00000001
	l.addi	r13,r18,+00000005
	l.addi	r14,r14,+00000001
	l.andi	r18,r13,000000FF
	l.ori	r7,r17,00000000
	l.addi	r19,r0,+0000000A
	l.ori	r2,r17,00000000
	l.ori	r4,r17,00000000
	l.ori	r27,r15,00000000
	l.addi	r25,r0,+00000004
	l.addi	r13,r0,+00000001
	l.j	00006004
	l.ori	r5,r22,00000000

l00005F70:
	l.addi	r4,r0,+0000001E
	l.mul	r2,r14,r2
	l.jal	0000FEDC
	l.addi	r2,r2,+00000001
	l.sw	40(r1),r2
	l.add	r2,r14,r14
	l.andi	r11,r11,000000FF
	l.add	r6,r2,r14
	l.addi	r4,r0,+000003E8
	l.slli	r13,r6,00000005
	l.sw	44(r1),r11
	l.addi	r24,r18,+00000008
	l.add	r6,r6,r13
	l.addi	r16,r16,+00000008
	l.add	r3,r6,r14
	l.jal	0000FEDC
	l.andi	r18,r24,000000FF
	l.slli	r13,r14,00000003
	l.addi	r17,r0,+00000005
	l.addi	r15,r0,+00000003
	l.add	r2,r2,r13
	l.lwz	r20,36(r28)
	l.add	r2,r2,r14
	l.addi	r8,r11,+00000001
	l.addi	r12,r2,+00000001
	l.addi	r14,r14,+00000001
	l.andi	r16,r16,000000FF
	l.ori	r7,r17,00000000
	l.addi	r19,r0,+0000000D
	l.addi	r2,r0,+0000000C
	l.ori	r4,r17,00000000
	l.ori	r22,r17,00000000
	l.ori	r27,r15,00000000
	l.addi	r25,r0,+00000006
	l.addi	r13,r0,+00000002
	l.ori	r5,r17,00000000
	l.addi	r24,r0,+0000000A

l00006004:
	l.addi	r26,r0,+000000C3
	l.j	0000606C
	l.addi	r21,r0,+00000000

l00006010:
	l.addi	r22,r0,+00000000
	l.addi	r7,r0,+00000003
	l.addi	r17,r0,+00000004
	l.addi	r6,r0,+0000001B
	l.addi	r15,r0,+00000002
	l.addi	r13,r0,+00000001
	l.ori	r19,r17,00000000
	l.addi	r18,r0,+00000008
	l.sw	44(r1),r6
	l.addi	r16,r0,+0000000C
	l.addi	r2,r0,+00000006
	l.ori	r4,r15,00000000
	l.ori	r27,r7,00000000
	l.ori	r25,r7,00000000
	l.ori	r5,r13,00000000
	l.ori	r20,r22,00000000
	l.ori	r24,r22,00000000
	l.ori	r26,r22,00000000
	l.ori	r21,r22,00000000
	l.ori	r14,r22,00000000
	l.ori	r12,r22,00000000
	l.ori	r8,r22,00000000
	l.sw	40(r1),r22

l0000606C:
	l.lhz	r30,24(r28)
	l.sfnei	r30,00000000
	l.bf	00006080
	l.nop

l0000607C:
	l.sw	24(r28),r21

l00006080:
	l.lhz	r30,28(r28)
	l.sfnei	r30,00000000
	l.bf	00006094
	l.nop

l00006090:
	l.sw	28(r28),r26

l00006094:
	l.lhz	r30,32(r28)
	l.sfnei	r30,00000000
	l.bf	000060A8
	l.nop

l000060A4:
	l.sw	32(r28),r24

l000060A8:
	l.lhz	r30,36(r28)
	l.sfnei	r30,00000000
	l.bf	000060BC
	l.nop

l000060B8:
	l.sw	36(r28),r20

l000060BC:
	l.movhi	r30,000001C6
	l.lhz	r23,26(r28)
	l.ori	r21,r30,00003030
	l.lwz	r6,20(r1)
	l.sw	0(r21),r23
	l.lhz	r23,30(r28)
	l.ori	r21,r30,00003034
	l.slli	r3,r6,00000010
	l.sw	0(r21),r23
	l.lhz	r23,34(r28)
	l.ori	r21,r30,00003038
	l.lwz	r6,28(r1)
	l.sw	0(r21),r23
	l.lhz	r23,38(r28)
	l.ori	r21,r30,0000303C
	l.or	r3,r3,r6
	l.sw	0(r21),r23
	l.lwz	r21,12(r28)
	l.slli	r28,r16,00000018
	l.srli	r21,r21,00000004
	l.lwz	r6,0(r1)
	l.or	r28,r3,r28
	l.andi	r23,r21,00000003
	l.lwz	r3,44(r1)
	l.ori	r21,r30,0000302C
	l.slli	r26,r3,00000008
	l.sw	0(r21),r23
	l.lwz	r3,16(r1)
	l.slli	r21,r4,0000000C
	l.slli	r22,r22,00000010
	l.slli	r29,r3,00000008
	l.slli	r31,r6,00000010
	l.or	r22,r22,r21
	l.lwz	r6,24(r1)
	l.lwz	r3,48(r1)
	l.or	r22,r22,r2
	l.or	r31,r31,r29
	l.lwz	r2,4(r1)
	l.slli	r27,r27,00000018
	l.slli	r25,r25,00000010
	l.or	r31,r31,r6
	l.slli	r20,r3,00000010
	l.slli	r6,r2,00000018
	l.or	r25,r27,r25
	l.slli	r23,r19,00000008
	l.or	r20,r6,r20
	l.lwz	r6,8(r1)
	l.lwz	r4,12(r1)
	l.or	r28,r28,r26
	l.or	r24,r25,r18
	l.ori	r26,r30,00003058
	l.slli	r18,r6,00000008
	l.ori	r29,r30,0000305C
	l.sw	0(r26),r28
	l.or	r24,r24,r23
	l.or	r19,r20,r4
	l.ori	r23,r30,00003060
	l.sw	0(r29),r31
	l.ori	r21,r30,00003064
	l.sw	0(r23),r24
	l.or	r19,r19,r18
	l.ori	r18,r30,00003068
	l.sw	0(r21),r22
	l.sw	0(r18),r19
	l.slli	r18,r17,00000018
	l.slli	r17,r17,00000010
	l.ori	r2,r30,00003078
	l.movhi	r4,00000200
	l.or	r17,r18,r17
	l.movhi	r3,0000FFFF
	l.or	r16,r17,r15
	l.slli	r15,r7,00000008
	l.ori	r4,r4,00000100
	l.slli	r5,r5,00000010
	l.or	r16,r16,r15
	l.ori	r15,r30,0000306C
	l.or	r13,r13,r4
	l.sw	0(r15),r16
	l.lwz	r6,40(r1)
	l.lwz	r15,0(r2)
	l.or	r13,r13,r5
	l.and	r15,r15,r3
	l.slli	r3,r8,00000014
	l.ori	r15,r15,00003310
	l.ori	r5,r30,00003080
	l.slli	r14,r14,00000014
	l.sw	0(r2),r15
	l.or	r4,r3,r6
	l.ori	r3,r30,00003050
	l.lwz	r2,32(r1)
	l.sw	0(r5),r13
	l.sw	0(r3),r4
	l.or	r14,r14,r12
	l.ori	r3,r30,00003054
	l.slli	r8,r2,00000010
	l.sw	0(r3),r14
	l.lwz	r3,36(r1)
	l.ori	r2,r30,00003090
	l.or	r12,r8,r3
	l.sw	0(r2),r12
	l.addi	r1,r1,+00000060
	l.lwz	r9,-4(r1)
	l.lwz	r2,-44(r1)
	l.lwz	r14,-40(r1)
	l.lwz	r16,-36(r1)
	l.lwz	r18,-32(r1)
	l.lwz	r20,-28(r1)
	l.lwz	r22,-24(r1)
	l.lwz	r24,-20(r1)
	l.lwz	r26,-16(r1)
	l.lwz	r28,-12(r1)
	l.jr	r9
	l.lwz	r30,-8(r1)

l00006280:
	l.addi	r4,r0,+00000002
	l.sw	0(r1),r4
	l.ori	r18,r4,00000000
	l.j	00005414
	l.sw	16(r1),r4

l00006294:
	l.addi	r6,r0,+00000001
	l.sw	0(r1),r6
	l.addi	r18,r0,+00000002

l000062A0:
	l.j	00005874
	l.sw	16(r1),r18

l000062A8:
	l.addi	r3,r0,+00000002
	l.sw	0(r1),r3
	l.ori	r18,r3,00000000
	l.j	00005B34
	l.sw	16(r1),r3

;; fn000062BC: 000062BC
;;   Called from:
;;     000064B8 (in fn00006394)
fn000062BC proc
	l.lwz	r3,92(r3)
	l.sw	-4(r1),r9
	l.srli	r3,r3,00000014
	l.sw	-8(r1),r2
	l.andi	r3,r3,00000007
	l.sfgtui	r3,00000005
	l.bf	00006324
	l.addi	r1,r1,-00000008

l000062DC:
	l.movhi	r2,00000001
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00002478
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.jr	r3
	l.nop

l000062F8:
	l.movhi	r4,0000E486
	l.j	0000632C
	l.ori	r4,r4,0000CCCC

l00006304:
	l.movhi	r4,0000E906
	l.j	0000632C
	l.ori	r4,r4,00009999

l00006310:
	l.movhi	r4,0000ED86
	l.j	0000632C
	l.ori	r4,r4,00006666

l0000631C:
	l.j	0000632C
	l.movhi	r4,0000F586

l00006324:
	l.movhi	r4,0000F206
	l.ori	r4,r4,00003333

l0000632C:
	l.movhi	r3,000001C2
	l.movhi	r2,00000100
	l.ori	r3,r3,00000290
	l.sw	0(r3),r4
	l.movhi	r3,000001C2
	l.ori	r3,r3,00000020
	l.lwz	r4,0(r3)
	l.or	r4,r4,r2
	l.movhi	r2,00000010
	l.sw	0(r3),r4
	l.lwz	r4,0(r3)
	l.or	r4,r4,r2
	l.sw	0(r3),r4

l00006360:
	l.lwz	r4,0(r3)
	l.movhi	r2,00000010
	l.and	r4,r4,r2
	l.sfnei	r4,00000000
	l.bf	00006360
	l.nop

l00006378:
	l.jal	0000C8A0
	l.addi	r3,r0,+00000014

l00006380:
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000001
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00006394: 00006394
;;   Called from:
;;     000065A8 (in fn000064DC)
fn00006394 proc
	l.sw	-16(r1),r2
	l.lwz	r2,0(r3)
	l.sw	-8(r1),r16
	l.ori	r16,r3,00000000
	l.xori	r3,r2,00000378
	l.sw	-4(r1),r9
	l.sub	r4,r0,r3
	l.sw	-12(r1),r14
	l.or	r3,r4,r3
	l.sfgesi	r3,+00000000
	l.bf	000063DC
	l.addi	r1,r1,-00000010

l000063C4:
	l.xori	r3,r2,00000348
	l.sub	r4,r0,r3
	l.or	r3,r4,r3
	l.sfltsi	r3,+00000000
	l.bf	000063E4
	l.nop

l000063DC:
	l.addi	r2,r2,-00000018
	l.sw	0(r16),r2

l000063E4:
	l.lwz	r3,0(r16)
	l.addi	r4,r0,+00000018
	l.add	r3,r3,r3
	l.jal	0000FEDC
	l.addi	r2,r0,+00000001
	l.sfleui	r11,00000020
	l.bf	0000646C
	l.ori	r14,r11,00000000

l00006404:
	l.sfleui	r11,00000040
	l.bf	0000645C
	l.addi	r2,r0,+00000002

l00006410:
	l.ori	r3,r11,00000000
	l.addi	r4,r0,+00000003
	l.jal	0001003C
	l.addi	r2,r0,+00000003
	l.sfeqi	r11,00000000
	l.bf	00006460
	l.ori	r3,r14,00000000

l0000642C:
	l.and	r3,r14,r2
	l.sfeqi	r3,00000000
	l.bf	0000645C
	l.addi	r2,r0,+00000004

l0000643C:
	l.ori	r3,r14,00000000
	l.addi	r4,r0,+00000005
	l.jal	0001003C
	l.addi	r2,r0,+00000005
	l.sfeqi	r11,00000000
	l.bf	0000645C
	l.nop

l00006458:
	l.addi	r2,r0,+00000000

l0000645C:
	l.ori	r3,r14,00000000

l00006460:
	l.jal	0000FEDC
	l.ori	r4,r2,00000000
	l.ori	r14,r11,00000000

l0000646C:
	l.movhi	r3,000001C2
	l.addi	r5,r14,-00000001
	l.ori	r3,r3,00000020
	l.slli	r5,r5,00000008
	l.lwz	r4,0(r3)
	l.addi	r4,r2,-00000001
	l.mul	r2,r14,r2
	l.slli	r4,r4,00000004
	l.movhi	r6,00008000
	l.add	r14,r2,r2
	l.or	r4,r5,r4
	l.add	r2,r14,r2
	l.or	r5,r4,r6
	l.sw	0(r3),r5
	l.movhi	r5,00008010
	l.or	r4,r4,r5
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+00000014
	l.jal	000062BC
	l.ori	r3,r16,00000000
	l.addi	r1,r1,+00000010
	l.slli	r11,r2,00000003
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn000064DC: 000064DC
;;   Called from:
;;     000073F8 (in fn000073EC)
fn000064DC proc
	l.sw	-32(r1),r2
	l.movhi	r2,000001C2
	l.sw	-28(r1),r14
	l.movhi	r4,00007FFF
	l.ori	r14,r2,0000015C
	l.sw	-8(r1),r24
	l.ori	r4,r4,0000FFFF
	l.ori	r24,r3,00000000
	l.lwz	r3,0(r14)
	l.sw	-4(r1),r9
	l.and	r3,r3,r4
	l.sw	-24(r1),r16
	l.sw	-20(r1),r18
	l.sw	-16(r1),r20
	l.sw	-12(r1),r22
	l.sw	0(r14),r3
	l.ori	r16,r2,000000FC
	l.ori	r18,r2,00000060
	l.lwz	r3,0(r16)
	l.ori	r20,r2,000002C0
	l.and	r3,r3,r4
	l.addi	r4,r0,-00004001
	l.sw	0(r16),r3
	l.movhi	r5,00007FFF
	l.lwz	r3,0(r18)
	l.ori	r5,r5,0000FFFF
	l.and	r3,r3,r4
	l.addi	r1,r1,-00000020
	l.sw	0(r18),r3
	l.lwz	r3,0(r20)
	l.and	r3,r3,r4
	l.ori	r4,r2,00000020
	l.sw	0(r20),r3
	l.ori	r2,r2,000000F4
	l.lwz	r3,0(r4)
	l.and	r3,r3,r5
	l.movhi	r5,00000010
	l.sw	0(r4),r3
	l.or	r3,r3,r5
	l.sw	0(r4),r3
	l.movhi	r4,00007FFF
	l.lwz	r3,0(r2)
	l.ori	r4,r4,0000FFFF
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.movhi	r5,0000FFCF
	l.lwz	r22,0(r2)
	l.ori	r5,r5,0000FFFF
	l.ori	r3,r24,00000000
	l.jal	00006394
	l.and	r22,r22,r5
	l.srli	r11,r11,00000001
	l.addi	r3,r0,+000003E8
	l.jal	0000C8A0
	l.sw	0(r24),r11
	l.addi	r3,r0,+0000000A
	l.sw	0(r2),r22
	l.jal	0000C8A0
	l.nop
	l.movhi	r3,00000001
	l.addi	r4,r0,+00000001
	l.or	r22,r22,r3
	l.sw	0(r2),r22
	l.lwz	r3,0(r20)
	l.ori	r3,r3,00004000
	l.sw	0(r20),r3
	l.lwz	r3,0(r18)
	l.ori	r3,r3,00004000
	l.sw	0(r18),r3
	l.movhi	r18,000001C6
	l.ori	r3,r18,00002094
	l.ori	r18,r18,0000300C
	l.sw	0(r3),r4
	l.movhi	r4,00008000
	l.lwz	r3,0(r16)
	l.or	r3,r3,r4
	l.sw	0(r16),r3
	l.lwz	r3,0(r14)
	l.or	r3,r3,r4
	l.sw	0(r14),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.addi	r3,r0,+000007D0
	l.jal	0000C8A0
	l.ori	r2,r0,00008000
	l.addi	r3,r0,+0000000A
	l.sw	0(r18),r2
	l.jal	0000C8A0
	l.nop
	l.addi	r1,r1,+00000020
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-32(r1)
	l.lwz	r14,-28(r1)
	l.lwz	r16,-24(r1)
	l.lwz	r18,-20(r1)
	l.lwz	r20,-16(r1)
	l.lwz	r22,-12(r1)
	l.jr	r9
	l.lwz	r24,-8(r1)

;; fn00006678: 00006678
;;   Called from:
;;     00007400 (in fn000073EC)
fn00006678 proc
	l.movhi	r4,000001C6
	l.sw	-4(r1),r2
	l.ori	r4,r4,00002000
	l.movhi	r2,0000FF00
	l.lwz	r6,4(r3)
	l.lwz	r5,0(r4)
	l.ori	r2,r2,00000FFF
	l.andi	r4,r6,00000007
	l.and	r5,r5,r2
	l.movhi	r2,00000040
	l.slli	r4,r4,00000010
	l.or	r5,r5,r2
	l.addi	r1,r1,-00000004
	l.or	r5,r5,r4
	l.lwz	r4,20(r3)
	l.andi	r12,r4,00000001
	l.sfeqi	r12,00000000
	l.bf	000066C8
	l.addi	r7,r0,+00001000

l000066C4:
	l.addi	r7,r0,+00000000

l000066C8:
	l.addi	r6,r6,-00000006
	l.sfgtui	r6,00000001
	l.bf	000066E4
	l.or	r5,r5,r7

l000066D8:
	l.movhi	r2,00000008
	l.j	000066F8
	l.or	r5,r5,r2

l000066E4:
	l.lwz	r6,92(r3)
	l.movhi	r2,00000008
	l.slli	r6,r6,0000000E
	l.and	r6,r6,r2
	l.or	r5,r5,r6

l000066F8:
	l.movhi	r6,000001C6
	l.addi	r13,r0,+00000001
	l.ori	r6,r6,00002000
	l.sw	0(r6),r5
	l.andi	r5,r4,00000100
	l.sfeqi	r5,00000000
	l.bf	0000671C
	l.nop

l00006718:
	l.addi	r13,r0,+00000002

l0000671C:
	l.srli	r4,r4,0000000C
	l.movhi	r7,000001C6
	l.lwz	r11,16(r3)
	l.andi	r15,r4,00000001
	l.addi	r4,r0,+00000000
	l.ori	r7,r7,00002000
	l.ori	r8,r4,00000000

l00006738:
	l.addi	r5,r4,+0000000C
	l.lwz	r6,0(r7)
	l.srl	r5,r11,r5
	l.addi	r2,r0,-00001000
	l.andi	r5,r5,00000001
	l.and	r6,r6,r2
	l.slli	r5,r5,00000002
	l.or	r6,r6,r15
	l.or	r5,r6,r5
	l.addi	r6,r4,+00000004
	l.srl	r6,r11,r6
	l.andi	r6,r6,000000FF
	l.addi	r6,r6,-00000001
	l.slli	r6,r6,00000004
	l.andi	r6,r6,000000FF
	l.or	r5,r5,r6
	l.srl	r6,r11,r4
	l.andi	r6,r6,0000000F
	l.sfeqi	r6,00000002
	l.bf	000067C8
	l.sfgtui	r6,00000002

l0000678C:
	l.bf	000067A8
	l.sfeqi	r6,00000004

l00006794:
	l.sfeqi	r6,00000001
	l.bnf	000067D0
	l.nop

l000067A0:
	l.j	000067D4
	l.ori	r5,r5,00000700

l000067A8:
	l.bf	000067C0
	l.sfeqi	r6,00000008

l000067B0:
	l.bnf	000067D0
	l.nop

l000067B8:
	l.j	000067D4
	l.ori	r5,r5,00000A00

l000067C0:
	l.j	000067D4
	l.ori	r5,r5,00000900

l000067C8:
	l.j	000067D4
	l.ori	r5,r5,00000800

l000067D0:
	l.ori	r5,r5,00000600

l000067D4:
	l.sw	0(r7),r5
	l.addi	r8,r8,+00000001
	l.addi	r7,r7,+00000004
	l.sfltu	r8,r13
	l.bf	00006738
	l.addi	r4,r4,+00000010

l000067EC:
	l.movhi	r4,000001C6
	l.addi	r5,r0,+00000201
	l.ori	r4,r4,00002000
	l.lwz	r4,0(r4)
	l.andi	r4,r4,00000001
	l.sfeqi	r4,00000000
	l.bf	00006810
	l.movhi	r4,000001C6

l0000680C:
	l.addi	r5,r0,+00000303

l00006810:
	l.sfeqi	r12,00000000
	l.ori	r6,r4,00003120
	l.sw	0(r6),r5
	l.bf	00006834
	l.ori	r5,r4,00003444

l00006824:
	l.addi	r2,r0,+00000000
	l.ori	r4,r4,000034C4
	l.sw	0(r5),r2
	l.sw	0(r4),r2

l00006834:
	l.lwz	r4,56(r3)
	l.movhi	r5,000001C6
	l.andi	r3,r4,00000003
	l.ori	r6,r5,00002000
	l.slli	r3,r3,00000019
	l.lwz	r7,0(r6)
	l.movhi	r2,0000001F
	l.or	r3,r3,r7
	l.slli	r4,r4,0000000A
	l.sw	0(r6),r3
	l.ori	r2,r2,0000F000
	l.ori	r3,r5,00002004
	l.and	r4,r4,r2
	l.lwz	r5,0(r3)
	l.or	r4,r4,r5
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

l00006880:
	l.movhi	r3,000001C6
	l.sw	-24(r1),r2
	l.ori	r2,r4,00000000
	l.sw	-20(r1),r14
	l.lwz	r14,92(r4)
	l.ori	r4,r3,00003108
	l.lwz	r7,12(r2)
	l.lwz	r5,0(r4)
	l.addi	r6,r0,-00000F01
	l.andi	r7,r7,00000001
	l.and	r5,r5,r6
	l.srli	r14,r14,00000002
	l.xori	r7,r7,00000001
	l.ori	r5,r5,00000300
	l.sw	-4(r1),r9
	l.sw	-16(r1),r16
	l.sw	-12(r1),r18
	l.sw	-8(r1),r20
	l.slli	r7,r7,00000005
	l.addi	r1,r1,-00000018
	l.andi	r14,r14,00000003
	l.sw	0(r4),r5
	l.lwz	r6,0(r2)
	l.ori	r3,r3,00003344
	l.addi	r8,r0,-00000031

l000068E4:
	l.lwz	r5,0(r3)
	l.sfleui	r6,00000258
	l.and	r5,r5,r8
	l.movhi	r8,0000FFFF
	l.or	r5,r5,r7
	l.ori	r8,r8,00000FF1
	l.bf	00006914
	l.and	r4,r5,r8

l00006904:
	l.movhi	r8,0000FFFF
	l.ori	r8,r8,000009F1
	l.and	r4,r5,r8
	l.ori	r4,r4,00000400

l00006914:
	l.sw	0(r3),r4
	l.movhi	r4,000001C6
	l.addi	r3,r3,+00000080
	l.ori	r4,r4,00003544
	l.sfne	r3,r4
	l.bf	000068E4
	l.addi	r8,r0,-00000031

l00006930:
	l.movhi	r16,000001C6
	l.addi	r5,r0,-00000801
	l.ori	r3,r16,00003208
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000002
	l.and	r4,r4,r5
	l.sw	0(r3),r4
	l.jal	00004D60
	l.ori	r3,r2,00000000
	l.sfeqi	r14,00000001
	l.bf	00006970
	l.sfeqi	r14,00000002

l00006960:
	l.bnf	00006A04
	l.ori	r3,r16,00003108

l00006968:
	l.j	00006998
	l.nop

l00006970:
	l.ori	r3,r16,00003108
	l.addi	r6,r0,-000000C1
	l.lwz	r4,0(r3)
	l.ori	r16,r16,000030BC
	l.and	r4,r4,r6
	l.addi	r7,r0,-00000108
	l.sw	0(r3),r4
	l.lwz	r3,0(r16)
	l.j	000069F8
	l.and	r3,r3,r7

l00006998:
	l.lwz	r4,0(r3)
	l.addi	r8,r0,-000000C1
	l.addi	r6,r0,-00000108
	l.and	r4,r4,r8
	l.movhi	r7,00000800
	l.ori	r4,r4,00000080
	l.movhi	r8,00007FFF
	l.sw	0(r3),r4
	l.ori	r3,r16,00003060
	l.ori	r8,r8,0000FFFF
	l.lwz	r4,0(r3)
	l.ori	r3,r16,000030BC
	l.srli	r4,r4,00000010
	l.lwz	r5,0(r3)
	l.ori	r16,r16,0000311C
	l.and	r5,r5,r6
	l.andi	r4,r4,0000001F
	l.ori	r5,r5,00000100
	l.addi	r4,r4,-00000002
	l.or	r4,r5,r4
	l.sw	0(r3),r4
	l.lwz	r3,0(r16)
	l.or	r3,r3,r7
	l.and	r3,r3,r8

l000069F8:
	l.sw	0(r16),r3
	l.j	00006A24
	l.lwz	r4,4(r2)

l00006A04:
	l.lwz	r4,0(r3)
	l.addi	r5,r0,-00000041
	l.and	r4,r4,r5
	l.sw	0(r3),r4
	l.lwz	r4,0(r3)
	l.ori	r4,r4,000000C0
	l.sw	0(r3),r4
	l.lwz	r4,4(r2)

l00006A24:
	l.addi	r3,r4,-00000006
	l.sfgtui	r3,00000001
	l.bf	00006A54
	l.movhi	r3,000001C6

l00006A34:
	l.movhi	r6,000088FF
	l.ori	r3,r3,0000311C
	l.ori	r6,r6,0000FFFF
	l.lwz	r5,0(r3)
	l.movhi	r7,00002200
	l.and	r5,r5,r6
	l.or	r5,r5,r7
	l.sw	0(r3),r5

l00006A54:
	l.lwz	r3,20(r2)
	l.andi	r3,r3,00001000
	l.sfeqi	r3,00000000
	l.bf	00006A8C
	l.movhi	r7,0000F000

l00006A68:
	l.movhi	r3,000001C6
	l.movhi	r8,0000F000
	l.ori	r3,r3,000030C0
	l.movhi	r6,00000300
	l.lwz	r5,0(r3)
	l.ori	r6,r6,00003087
	l.and	r5,r5,r8
	l.j	00006AA8
	l.or	r5,r5,r6

l00006A8C:
	l.movhi	r3,000001C6
	l.ori	r3,r3,000030C0
	l.movhi	r8,00000100
	l.lwz	r5,0(r3)
	l.ori	r8,r8,00003087
	l.and	r5,r5,r7
	l.or	r5,r5,r8

l00006AA8:
	l.sw	0(r3),r5
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001510
	l.lwz	r5,0(r3)
	l.andi	r5,r5,00000003
	l.sfeqi	r5,00000000
	l.bf	00006C98
	l.movhi	r8,0000FF00

l00006AC8:
	l.lwz	r4,0(r3)
	l.addi	r5,r0,-00000003
	l.and	r4,r4,r5
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.movhi	r4,000001C6
	l.movhi	r8,000000FF
	l.ori	r3,r4,00003140
	l.lwz	r5,8(r2)
	l.lwz	r6,0(r3)
	l.movhi	r7,0000FF00
	l.ori	r8,r8,0000FFFF
	l.and	r6,r6,r7
	l.and	r5,r5,r8
	l.sfnei	r14,00000001
	l.or	r5,r5,r6
	l.sw	0(r3),r5
	l.bf	00006B50
	l.addi	r3,r0,+00000062

l00006B18:
	l.ori	r3,r4,00003000
	l.addi	r5,r0,+00000052
	l.ori	r4,r4,00003010
	l.sw	0(r3),r5
	l.addi	r5,r0,+00000053
	l.sw	0(r3),r5

l00006B30:
	l.lwz	r3,0(r4)
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00006B30
	l.nop

l00006B44:
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+00000020

l00006B50:
	l.movhi	r16,000001C6
	l.ori	r4,r16,00003000
	l.sw	0(r4),r3
	l.ori	r3,r3,00000001
	l.sw	0(r4),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.ori	r3,r16,00003010

l00006B70:
	l.lwz	r4,0(r3)
	l.andi	r4,r4,00000001
	l.sfeqi	r4,00000000
	l.bf	00006B70
	l.movhi	r16,000001C6

l00006B84:
	l.ori	r3,r16,0000310C
	l.movhi	r5,0000F9FF
	l.lwz	r4,0(r3)
	l.ori	r5,r5,0000FFFF
	l.movhi	r6,00000400
	l.and	r4,r4,r5
	l.or	r4,r4,r6
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.ori	r3,r16,00003004
	l.ori	r16,r16,00003018
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000001
	l.sw	0(r3),r4

l00006BC0:
	l.lwz	r3,0(r16)
	l.andi	r3,r3,00000007
	l.sfnei	r3,00000003
	l.bf	00006BC0
	l.movhi	r18,000001C6

l00006BD4:
	l.movhi	r3,000001F0
	l.addi	r7,r0,-00000002
	l.ori	r3,r3,00001510
	l.lwz	r4,0(r3)
	l.and	r4,r4,r7
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.ori	r3,r18,00003004
	l.addi	r8,r0,-00000002
	l.lwz	r4,0(r3)
	l.ori	r18,r18,00003018
	l.and	r4,r4,r8
	l.sw	0(r3),r4

l00006C0C:
	l.lwz	r3,0(r18)
	l.andi	r3,r3,00000007
	l.sfnei	r3,00000001
	l.bf	00006C0C
	l.movhi	r16,000001C6

l00006C20:
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000F
	l.sfnei	r14,00000001
	l.bf	00006D58
	l.ori	r3,r16,00003108

l00006C34:
	l.addi	r5,r0,-000000C1
	l.lwz	r4,0(r3)
	l.movhi	r6,0000F9FF
	l.and	r4,r4,r5
	l.ori	r6,r6,0000FFFF
	l.sw	0(r3),r4
	l.ori	r3,r16,0000310C
	l.movhi	r7,00000200
	l.lwz	r4,0(r3)
	l.and	r4,r4,r6
	l.or	r4,r4,r7
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.ori	r3,r14,00000000
	l.ori	r3,r16,00003000
	l.addi	r4,r0,+00000401
	l.ori	r16,r16,00003010
	l.sw	0(r3),r4

l00006C7C:
	l.lwz	r3,0(r16)
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00006C7C
	l.movhi	r3,000001C6

l00006C90:
	l.j	00006D60
	l.ori	r4,r3,00003010

l00006C98:
	l.movhi	r3,000001C6
	l.ori	r5,r3,00003140
	l.lwz	r6,8(r2)
	l.lwz	r7,0(r5)
	l.sfnei	r14,00000001
	l.and	r7,r7,r8
	l.movhi	r8,000000FF
	l.ori	r8,r8,0000FFFF
	l.and	r6,r6,r8
	l.or	r6,r6,r7
	l.sw	0(r5),r6
	l.bf	00006D18
	l.sfeqi	r4,00000003

l00006CCC:
	l.ori	r4,r3,00003000
	l.addi	r5,r0,+00000052
	l.ori	r3,r3,00003010
	l.sw	0(r4),r5
	l.addi	r5,r0,+00000053
	l.sw	0(r4),r5

l00006CE4:
	l.lwz	r4,0(r3)
	l.andi	r4,r4,00000001
	l.sfeqi	r4,00000000
	l.bf	00006CE4
	l.nop

l00006CF8:
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.lwz	r4,4(r2)
	l.sfeqi	r4,00000003
	l.bf	00006D24
	l.addi	r3,r0,+000005A0

l00006D10:
	l.j	00006D24
	l.addi	r3,r0,+00000520

l00006D18:
	l.bf	00006D24
	l.addi	r3,r0,+000005F2

l00006D20:
	l.addi	r3,r0,+00000572

l00006D24:
	l.movhi	r16,000001C6
	l.ori	r4,r16,00003000
	l.ori	r16,r16,00003010
	l.sw	0(r4),r3
	l.ori	r3,r3,00000001
	l.sw	0(r4),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A

l00006D44:
	l.lwz	r3,0(r16)
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00006D44
	l.nop

l00006D58:
	l.movhi	r3,000001C6
	l.ori	r4,r3,00003010

l00006D60:
	l.ori	r3,r3,00003018
	l.lwz	r4,0(r4)
	l.srli	r4,r4,00000014
	l.andi	r4,r4,000000FF
	l.sub	r20,r0,r4
	l.or	r20,r20,r4
	l.xori	r20,r20,0000FFFF
	l.srli	r20,r20,0000001F

l00006D80:
	l.lwz	r4,0(r3)
	l.andi	r4,r4,00000001
	l.sfeqi	r4,00000000
	l.bf	00006D80
	l.movhi	r16,000001C6

l00006D94:
	l.ori	r18,r16,0000308C
	l.movhi	r4,00008000
	l.lwz	r3,0(r18)
	l.or	r3,r3,r4
	l.sw	0(r18),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.movhi	r5,00007FFF
	l.lwz	r3,0(r18)
	l.ori	r5,r5,0000FFFF
	l.and	r3,r3,r5
	l.sw	0(r18),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.ori	r3,r16,000020D0
	l.movhi	r6,00008000
	l.lwz	r4,0(r3)
	l.or	r4,r4,r6
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.ori	r3,r16,0000310C
	l.movhi	r7,0000F9FF
	l.lwz	r4,0(r3)
	l.ori	r7,r7,0000FFFF
	l.lwz	r2,4(r2)
	l.and	r4,r4,r7
	l.addi	r2,r2,-00000006
	l.sw	0(r3),r4
	l.sfgtui	r2,00000001
	l.bf	00006E30
	l.sfnei	r14,00000001

l00006E14:
	l.bf	00006E30
	l.ori	r16,r16,0000311C

l00006E1C:
	l.addi	r8,r0,-000000C1
	l.lwz	r2,0(r16)
	l.and	r2,r2,r8
	l.ori	r2,r2,00000040
	l.sw	0(r16),r2

l00006E30:
	l.addi	r1,r1,+00000018
	l.ori	r11,r20,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-24(r1)
	l.lwz	r14,-20(r1)
	l.lwz	r16,-16(r1)
	l.lwz	r18,-12(r1)
	l.jr	r9
	l.lwz	r20,-8(r1)

;; fn00006E54: 00006E54
;;   Called from:
;;     00007B64 (in fn00007AE0)
fn00006E54 proc
	l.movhi	r4,000001C6
	l.ori	r3,r4,00002000
	l.lwz	r3,0(r3)
	l.srli	r11,r3,00000008
	l.srli	r5,r3,00000004
	l.andi	r11,r11,0000000F
	l.andi	r5,r5,0000000F
	l.add	r11,r11,r5
	l.srli	r5,r3,00000002
	l.addi	r11,r11,-0000000E
	l.andi	r3,r3,00000003
	l.andi	r5,r5,00000003
	l.sfeqi	r3,00000000
	l.add	r11,r11,r5
	l.addi	r5,r0,+00000001
	l.bf	00006ED8
	l.sll	r11,r5,r11

l00006E98:
	l.ori	r4,r4,00002004
	l.ori	r3,r11,00000000
	l.lwz	r4,0(r4)
	l.andi	r6,r4,00000003
	l.sfeqi	r6,00000000
	l.bf	00006ED8
	l.srli	r6,r4,00000008

l00006EB4:
	l.srli	r3,r4,00000004
	l.srli	r4,r4,00000002
	l.andi	r6,r6,0000000F
	l.andi	r3,r3,0000000F
	l.andi	r4,r4,00000003
	l.add	r3,r6,r3
	l.addi	r3,r3,-0000000E
	l.add	r3,r3,r4
	l.sll	r3,r5,r3

l00006ED8:
	l.jr	r9
	l.add	r11,r3,r11

;; fn00006EE0: 00006EE0
;;   Called from:
;;     000074EC (in fn00007428)
;;     00007570 (in fn00007428)
;;     000075F4 (in fn00007428)
;;     000076D4 (in fn00007634)
fn00006EE0 proc
	l.movhi	r4,000001C6
	l.sw	-4(r1),r2
	l.ori	r5,r4,00003010
	l.movhi	r2,00000040
	l.lwz	r5,0(r5)
	l.and	r5,r5,r2
	l.sfeqi	r5,00000000
	l.bf	00006F50
	l.addi	r1,r1,-00000004

l00006F04:
	l.ori	r5,r4,00003348
	l.ori	r6,r4,000033C8
	l.lwz	r5,0(r5)
	l.lwz	r6,0(r6)
	l.ori	r7,r4,00003448
	l.srli	r6,r6,00000018
	l.ori	r4,r4,000034C8
	l.srli	r5,r5,00000018
	l.andi	r6,r6,00000003
	l.lwz	r7,0(r7)
	l.lwz	r4,0(r4)
	l.sfnei	r6,00000002
	l.bf	00006FB0
	l.andi	r5,r5,00000003

l00006F3C:
	l.sfeqi	r5,00000002
	l.bf	00006F64
	l.or	r5,r6,r5

l00006F48:
	l.j	00006FB8
	l.addi	r11,r0,+00000000

l00006F50:
	l.lwz	r4,20(r3)
	l.addi	r2,r0,-00000010
	l.and	r4,r4,r2
	l.j	00006FD4
	l.ori	r4,r4,00001000

l00006F64:
	l.srli	r4,r4,00000018
	l.andi	r4,r4,00000003
	l.sfnei	r4,00000002
	l.bf	00006F9C
	l.movhi	r2,0000FFFF

l00006F78:
	l.srli	r4,r7,00000018
	l.andi	r4,r4,00000003
	l.sfnei	r4,00000002
	l.bf	00006F9C
	l.nop

l00006F8C:
	l.lwz	r4,20(r3)
	l.ori	r2,r2,00000FF0
	l.j	00006FD4
	l.and	r4,r4,r2

l00006F9C:
	l.lwz	r4,20(r3)
	l.ori	r2,r2,00000FF0
	l.and	r4,r4,r2
	l.j	00006FD4
	l.ori	r4,r4,00000001

l00006FB0:
	l.or	r5,r6,r5
	l.addi	r11,r0,+00000000

l00006FB8:
	l.sfne	r5,r11
	l.bf	00006FDC
	l.nop

l00006FC4:
	l.lwz	r4,20(r3)
	l.addi	r2,r0,-00001010
	l.and	r4,r4,r2
	l.ori	r4,r4,00001001

l00006FD4:
	l.sw	20(r3),r4
	l.addi	r11,r0,+00000001

l00006FDC:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00006FE8: 00006FE8
;;   Called from:
;;     000074DC (in fn00007428)
;;     00007560 (in fn00007428)
;;     000075E4 (in fn00007428)
;;     000076C4 (in fn00007634)
fn00006FE8 proc
	l.sw	-24(r1),r2
	l.movhi	r2,000001C6
	l.sw	-20(r1),r14
	l.ori	r14,r3,00000000
	l.ori	r3,r2,00003100
	l.movhi	r5,00000200
	l.lwz	r4,0(r3)
	l.sw	-4(r1),r9
	l.or	r4,r4,r5
	l.sw	-16(r1),r16
	l.sw	-12(r1),r18
	l.sw	-8(r1),r20
	l.sw	0(r3),r4
	l.movhi	r4,00001234
	l.movhi	r3,00004000
	l.ori	r4,r4,00005678
	l.addi	r1,r1,-00000028
	l.sw	0(r3),r4
	l.lwz	r3,0(r3)
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.ori	r3,r2,00003010
	l.lwz	r3,0(r3)
	l.srli	r3,r3,0000000D
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	000070C0
	l.ori	r3,r2,00003348

l00007058:
	l.ori	r5,r2,00003448
	l.lwz	r4,0(r3)
	l.ori	r3,r2,000033C8
	l.srli	r4,r4,0000001C
	l.lwz	r3,0(r3)
	l.ori	r2,r2,000034C8
	l.srli	r3,r3,0000001C
	l.andi	r4,r4,00000001
	l.lwz	r5,0(r5)
	l.andi	r3,r3,00000001
	l.lwz	r2,0(r2)
	l.addi	r2,r0,+00000000
	l.sw	0(r1),r4
	l.sfne	r4,r2
	l.bf	000070E0
	l.sw	4(r1),r3

l00007098:
	l.sfne	r3,r2
	l.bf	000070E4
	l.movhi	r18,000001C6

l000070A4:
	l.lwz	r2,20(r14)
	l.addi	r3,r0,-00000010
	l.and	r2,r2,r3
	l.ori	r2,r2,00000001
	l.sw	20(r14),r2
	l.j	000070E4
	l.addi	r2,r0,+00000001

l000070C0:
	l.lwz	r3,20(r14)
	l.andi	r4,r3,0000000F
	l.sfnei	r4,00000000
	l.bf	000070E0
	l.addi	r2,r0,+00000001

l000070D4:
	l.addi	r4,r0,-00000010
	l.and	r3,r3,r4
	l.sw	20(r14),r3

l000070E0:
	l.movhi	r18,000001C6

l000070E4:
	l.movhi	r5,0000FBFF
	l.ori	r16,r18,00003100
	l.ori	r5,r5,0000FFFF
	l.lwz	r20,0(r16)
	l.addi	r3,r0,+00000064
	l.and	r20,r20,r5
	l.sw	0(r16),r20
	l.jal	0000C8A0
	l.nop
	l.movhi	r3,00000400
	l.movhi	r4,00000100
	l.or	r20,r20,r3
	l.addi	r5,r0,-00002001
	l.sw	0(r16),r20
	l.ori	r20,r18,00003010
	l.lwz	r3,0(r16)
	l.or	r3,r3,r4
	l.sw	0(r16),r3
	l.lwz	r3,0(r20)
	l.and	r3,r3,r5
	l.sw	0(r20),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.sfnei	r2,00000000
	l.bf	00007164
	l.movhi	r4,0000FDFF

l0000714C:
	l.lwz	r3,0(r16)
	l.ori	r4,r4,0000FFFF
	l.and	r3,r3,r4
	l.sw	0(r16),r3
	l.j	00007318
	l.addi	r1,r1,+00000028

l00007164:
	l.lwz	r3,20(r14)
	l.srli	r3,r3,0000000C
	l.andi	r3,r3,0000000F
	l.sfeqi	r3,00000000
	l.bf	000072F8
	l.addi	r2,r0,+00000001

l0000717C:
	l.lwz	r3,16(r14)
	l.ori	r16,r2,00000000
	l.srli	r5,r3,0000000C
	l.srli	r4,r3,00000004
	l.andi	r3,r3,0000000F
	l.andi	r5,r5,0000000F
	l.andi	r4,r4,000000FF
	l.add	r3,r3,r3
	l.add	r4,r5,r4
	l.movhi	r5,00004000
	l.addi	r4,r4,+0000000B
	l.add	r3,r4,r3
	l.movhi	r4,00001234
	l.sll	r3,r2,r3
	l.ori	r4,r4,0000ABCD
	l.add	r3,r3,r5
	l.sw	0(r3),r4
	l.lwz	r3,0(r3)
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.lwz	r3,0(r20)
	l.srli	r3,r3,0000000D
	l.and	r3,r3,r2
	l.sfeqi	r3,00000000
	l.bf	0000728C
	l.ori	r4,r18,00003448

l000071E4:
	l.ori	r2,r18,00003348
	l.ori	r3,r18,000033C8
	l.lwz	r2,0(r2)
	l.lwz	r3,0(r3)
	l.lwz	r4,0(r4)
	l.srli	r2,r2,0000001C
	l.srli	r3,r3,0000001C
	l.srli	r4,r4,0000001C
	l.ori	r18,r18,000034C8
	l.and	r2,r2,r16
	l.lwz	r5,0(r18)
	l.and	r3,r3,r16
	l.and	r4,r4,r16
	l.srli	r5,r5,0000001C
	l.sw	0(r1),r2
	l.sw	4(r1),r3
	l.sw	8(r1),r4
	l.sfeqi	r2,00000000
	l.bf	0000726C
	l.and	r5,r5,r16

l00007234:
	l.sfeqi	r3,00000000
	l.bf	00007298
	l.ori	r2,r3,00000000

l00007240:
	l.sfeqi	r4,00000000
	l.bf	00007298
	l.ori	r2,r4,00000000

l0000724C:
	l.sfeqi	r5,00000000
	l.bf	00007298
	l.ori	r2,r5,00000000

l00007258:
	l.movhi	r3,0000FFFF
	l.lwz	r2,20(r14)
	l.ori	r3,r3,00000FFF
	l.j	00007280
	l.and	r2,r2,r3

l0000726C:
	l.sfnei	r3,00000000
	l.bf	00007298
	l.nop

l00007278:
	l.lwz	r2,20(r14)
	l.ori	r2,r2,00001000

l00007280:
	l.sw	20(r14),r2
	l.j	00007298
	l.ori	r2,r16,00000000

l0000728C:
	l.lwz	r3,20(r14)
	l.ori	r3,r3,00001000
	l.sw	20(r14),r3

l00007298:
	l.movhi	r16,000001C6
	l.movhi	r4,0000FBFF
	l.ori	r14,r16,00003100
	l.ori	r4,r4,0000FFFF
	l.lwz	r18,0(r14)
	l.addi	r3,r0,+00000064
	l.and	r18,r18,r4
	l.sw	0(r14),r18
	l.jal	0000C8A0
	l.nop
	l.movhi	r5,00000400
	l.movhi	r4,00000100
	l.or	r18,r18,r5
	l.addi	r5,r0,-00002001
	l.sw	0(r14),r18
	l.lwz	r3,0(r14)
	l.or	r3,r3,r4
	l.sw	0(r14),r3
	l.ori	r3,r16,00003010
	l.lwz	r4,0(r3)
	l.and	r4,r4,r5
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A

l000072F8:
	l.movhi	r3,000001C6
	l.movhi	r5,0000FDFF
	l.ori	r3,r3,00003100
	l.ori	r5,r5,0000FFFF
	l.lwz	r4,0(r3)
	l.and	r4,r4,r5
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000028

l00007318:
	l.ori	r11,r2,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-24(r1)
	l.lwz	r14,-20(r1)
	l.lwz	r16,-16(r1)
	l.lwz	r18,-12(r1)
	l.jr	r9
	l.lwz	r20,-8(r1)

;; fn00007338: 00007338
;;   Called from:
;;     00007CC4 (in fn00007AE0)
fn00007338 proc
	l.srli	r3,r3,00000001
	l.sw	-4(r1),r2
	l.movhi	r6,00004000
	l.addi	r1,r1,-00000004
	l.slli	r3,r3,00000014
	l.j	0000737C
	l.addi	r5,r0,+00000000

l00007354:
	l.ori	r2,r2,00004567
	l.add	r7,r5,r2
	l.movhi	r2,0000FEDC
	l.sw	0(r6),r7
	l.ori	r2,r2,0000BA98
	l.add	r7,r6,r3
	l.add	r8,r5,r2
	l.addi	r6,r6,+00000004
	l.sw	0(r7),r8
	l.addi	r5,r5,+00000001

l0000737C:
	l.sfltu	r5,r4
	l.bf	00007354
	l.movhi	r2,00000123

l00007388:
	l.movhi	r6,00004000
	l.j	000073D0
	l.addi	r5,r0,+00000000

l00007394:
	l.movhi	r2,0000FEDC
	l.ori	r2,r2,0000BA98
	l.lwz	r8,0(r7)
	l.add	r7,r5,r2
	l.sfne	r8,r7
	l.bf	000073E0
	l.addi	r11,r0,+00000001

l000073B0:
	l.movhi	r2,00000123
	l.lwz	r8,0(r6)
	l.ori	r2,r2,00004567
	l.add	r7,r5,r2
	l.sfne	r8,r7
	l.bf	000073E0
	l.addi	r6,r6,+00000004

l000073CC:
	l.addi	r5,r5,+00000001

l000073D0:
	l.sfltu	r5,r4
	l.bf	00007394
	l.add	r7,r6,r3

l000073DC:
	l.addi	r11,r0,+00000000

l000073E0:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn000073EC: 000073EC
;;   Called from:
;;     000074C0 (in fn00007428)
;;     00007544 (in fn00007428)
;;     000075C8 (in fn00007428)
;;     00007684 (in fn00007634)
;;     00007724 (in fn0000770C)
;;     00007B38 (in fn00007AE0)
fn000073EC proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r1,r1,-00000008
	l.jal	000064DC
	l.ori	r2,r3,00000000
	l.jal	00006678
	l.ori	r3,r2,00000000
	l.jal	000051E8
	l.ori	r3,r2,00000000
	l.addi	r1,r1,+00000008
	l.ori	r4,r2,00000000
	l.addi	r3,r0,+00000000
	l.lwz	r9,-4(r1)
	l.j	00006880
	l.lwz	r2,-8(r1)

;; fn00007428: 00007428
;;   Called from:
;;     00007A5C (in fn00007A3C)
fn00007428 proc
	l.movhi	r4,000000B0
	l.sw	-20(r1),r2
	l.sw	-16(r1),r14
	l.ori	r2,r3,00000000
	l.lwz	r14,92(r3)
	l.ori	r4,r4,000000B0
	l.sw	-12(r1),r16
	l.sw	-8(r1),r18
	l.sw	-4(r1),r9
	l.lwz	r18,16(r3)
	l.lwz	r16,20(r3)
	l.sw	16(r2),r4
	l.ori	r3,r14,00000001
	l.addi	r4,r0,+00000000
	l.sw	92(r2),r3
	l.sw	20(r2),r4
	l.andi	r3,r3,00000010
	l.sfeq	r3,r4
	l.bf	00007480
	l.addi	r1,r1,-00000014

l00007478:
	l.ori	r3,r14,00000005
	l.sw	92(r2),r3

l00007480:
	l.lwz	r3,68(r2)
	l.andi	r3,r3,000003FF
	l.sw	0(r2),r3
	l.addi	r3,r0,+00000007
	l.sw	4(r2),r3
	l.movhi	r3,0000003B
	l.ori	r3,r3,00003BF9
	l.sw	8(r2),r3
	l.addi	r3,r0,+00005505
	l.sw	80(r2),r3
	l.movhi	r3,00003333
	l.sw	84(r2),r3
	l.ori	r3,r0,0000CCCC
	l.sw	88(r2),r3
	l.addi	r3,r0,+00000002
	l.sw	36(r2),r3
	l.jal	000073EC
	l.ori	r3,r2,00000000
	l.lwz	r3,92(r2)
	l.andi	r3,r3,00000010
	l.sfnei	r3,00000000
	l.bf	000074EC
	l.nop

l000074DC:
	l.jal	00006FE8
	l.ori	r3,r2,00000000
	l.j	000074F8
	l.sfnei	r11,00000000

l000074EC:
	l.jal	00006EE0
	l.ori	r3,r2,00000000
	l.sfnei	r11,00000000

l000074F8:
	l.bf	00007608
	l.nop

l00007500:
	l.lwz	r3,68(r2)
	l.srli	r3,r3,0000000A
	l.andi	r3,r3,000003FF
	l.sw	0(r2),r3
	l.addi	r3,r0,+00000006
	l.sw	4(r2),r3
	l.movhi	r3,0000003B
	l.ori	r3,r3,00003BF9
	l.sw	8(r2),r3
	l.addi	r3,r0,+00005505
	l.sw	80(r2),r3
	l.movhi	r3,00003333
	l.sw	84(r2),r3
	l.ori	r3,r0,0000CCCC
	l.sw	88(r2),r3
	l.addi	r3,r0,+00000002
	l.sw	36(r2),r3
	l.jal	000073EC
	l.ori	r3,r2,00000000
	l.lwz	r3,92(r2)
	l.andi	r3,r3,00000010
	l.sfnei	r3,00000000
	l.bf	00007570
	l.nop

l00007560:
	l.jal	00006FE8
	l.ori	r3,r2,00000000
	l.j	0000757C
	l.sfnei	r11,00000000

l00007570:
	l.jal	00006EE0
	l.ori	r3,r2,00000000
	l.sfnei	r11,00000000

l0000757C:
	l.bf	00007608
	l.nop

l00007584:
	l.lwz	r3,68(r2)
	l.srli	r3,r3,00000014
	l.andi	r3,r3,000003FF
	l.sw	0(r2),r3
	l.addi	r3,r0,+00000003
	l.sw	4(r2),r3
	l.movhi	r3,0000003B
	l.ori	r3,r3,00003BF9
	l.sw	8(r2),r3
	l.addi	r3,r0,+00005505
	l.sw	80(r2),r3
	l.movhi	r3,00003333
	l.sw	84(r2),r3
	l.ori	r3,r0,0000BBBB
	l.sw	88(r2),r3
	l.addi	r3,r0,+00000040
	l.sw	28(r2),r3
	l.jal	000073EC
	l.ori	r3,r2,00000000
	l.lwz	r3,92(r2)
	l.andi	r3,r3,00000010
	l.sfnei	r3,00000000
	l.bf	000075F4
	l.nop

l000075E4:
	l.jal	00006FE8
	l.ori	r3,r2,00000000
	l.j	00007600
	l.sfeqi	r11,00000000

l000075F4:
	l.jal	00006EE0
	l.ori	r3,r2,00000000
	l.sfeqi	r11,00000000

l00007600:
	l.bf	00007618
	l.nop

l00007608:
	l.sw	92(r2),r14
	l.sw	16(r2),r18
	l.sw	20(r2),r16
	l.addi	r11,r0,+00000001

l00007618:
	l.addi	r1,r1,+00000014
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)

;; fn00007634: 00007634
;;   Called from:
;;     00007A84 (in fn00007A3C)
fn00007634 proc
	l.movhi	r4,000000B0
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.ori	r2,r3,00000000
	l.lwz	r14,92(r3)
	l.ori	r4,r4,000000B0
	l.sw	-8(r1),r16
	l.sw	-4(r1),r9
	l.lwz	r16,16(r3)
	l.ori	r3,r14,00000001
	l.sw	16(r2),r4
	l.addi	r4,r0,+00001000
	l.sw	92(r2),r3
	l.sw	20(r2),r4
	l.andi	r3,r3,00000010
	l.sfeqi	r3,00000000
	l.bf	00007684
	l.addi	r1,r1,-00000010

l0000767C:
	l.ori	r3,r14,00000005
	l.sw	92(r2),r3

l00007684:
	l.jal	000073EC
	l.ori	r3,r2,00000000
	l.movhi	r3,000001C6
	l.movhi	r4,00000010
	l.ori	r3,r3,00003010
	l.addi	r11,r0,+00000000
	l.lwz	r3,0(r3)
	l.and	r3,r3,r4
	l.sfne	r3,r11
	l.bf	000076F4
	l.nop

l000076B0:
	l.lwz	r3,92(r2)
	l.andi	r3,r3,00000010
	l.sfne	r3,r11
	l.bf	000076D4
	l.nop

l000076C4:
	l.jal	00006FE8
	l.ori	r3,r2,00000000
	l.j	000076E0
	l.sfeqi	r11,00000000

l000076D4:
	l.jal	00006EE0
	l.ori	r3,r2,00000000
	l.sfeqi	r11,00000000

l000076E0:
	l.bf	000076F4
	l.nop

l000076E8:
	l.sw	92(r2),r14
	l.sw	16(r2),r16
	l.addi	r11,r0,+00000001

l000076F4:
	l.addi	r1,r1,+00000010
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn0000770C: 0000770C
;;   Called from:
;;     00007A98 (in fn00007A3C)
fn0000770C proc
	l.sw	-20(r1),r2
	l.sw	-4(r1),r9
	l.sw	-16(r1),r14
	l.sw	-12(r1),r16
	l.sw	-8(r1),r18
	l.addi	r1,r1,-00000014
	l.jal	000073EC
	l.ori	r2,r3,00000000
	l.sfeqi	r11,00000000
	l.bf	00007A20
	l.addi	r7,r0,+00000000

l00007738:
	l.lwz	r23,20(r2)
	l.srli	r11,r23,00000008
	l.movhi	r3,000001C6
	l.movhi	r5,00004000
	l.andi	r11,r11,0000000F
	l.ori	r19,r7,00000000
	l.addi	r11,r11,+00000001
	l.ori	r17,r7,00000000
	l.ori	r3,r3,00002000
	l.addi	r13,r0,+00000001
	l.addi	r31,r0,+000000FF
	l.j	000079E0
	l.addi	r25,r0,+0000000F

l0000776C:
	l.andi	r14,r4,00000001
	l.sfnei	r14,00000000
	l.bf	00007780
	l.ori	r12,r8,00000000

l0000777C:
	l.ori	r12,r6,00000000

l00007780:
	l.sw	0(r8),r12
	l.addi	r4,r4,+00000001
	l.addi	r8,r8,+00000004
	l.sfnei	r4,00000040
	l.bf	0000776C
	l.addi	r6,r6,-00000004

l00007798:
	l.lwz	r4,0(r3)
	l.addi	r6,r0,-00000F0D
	l.and	r4,r4,r6
	l.addi	r6,r0,+0000000B
	l.ori	r4,r4,000006F0
	l.sw	0(r3),r4
	l.addi	r14,r6,+0000000B

l000077B4:
	l.addi	r8,r0,+00000000
	l.sll	r14,r13,r14
	l.ori	r12,r15,00000000
	l.ori	r4,r8,00000000
	l.add	r14,r14,r5

l000077C8:
	l.andi	r16,r4,00000001
	l.sfeqi	r16,00000000
	l.bf	000077DC
	l.ori	r21,r12,00000000

l000077D8:
	l.add	r21,r8,r5

l000077DC:
	l.add	r16,r8,r14
	l.lwz	r16,0(r16)
	l.sfeq	r21,r16
	l.bf	0000780C
	l.addi	r4,r4,+00000001

l000077F0:
	l.addi	r4,r4,-00000001
	l.sfeqi	r4,00000040
	l.bnf	00007824
	l.addi	r6,r6,+00000001

l00007800:
	l.addi	r6,r6,-00000001
	l.j	00007838
	l.sfgtui	r6,0000000F

l0000780C:
	l.addi	r8,r8,+00000004
	l.sfnei	r4,00000040
	l.bf	000077C8
	l.addi	r12,r12,-00000004

l0000781C:
	l.j	00007838
	l.sfgtui	r6,0000000F

l00007824:
	l.sfnei	r6,00000011
	l.bf	000077B4
	l.addi	r14,r6,+0000000B

l00007830:
	l.j	00007844
	l.addi	r6,r0,+00000010

l00007838:
	l.bnf	00007848
	l.addi	r4,r7,+00000004

l00007840:
	l.addi	r6,r0,+00000010

l00007844:
	l.addi	r4,r7,+00000004

l00007848:
	l.lwz	r14,16(r2)
	l.sll	r8,r6,r4
	l.sll	r4,r31,r4
	l.ori	r12,r15,00000000
	l.xori	r4,r4,0000FFFF
	l.and	r14,r4,r14
	l.lwz	r4,0(r3)
	l.or	r14,r8,r14
	l.addi	r8,r0,-00000FFD
	l.and	r4,r4,r8
	l.ori	r8,r5,00000000
	l.ori	r4,r4,000006A4
	l.sw	0(r3),r4
	l.addi	r4,r0,+00000000

l00007880:
	l.andi	r16,r4,00000001
	l.sfnei	r16,00000000
	l.bf	00007894
	l.ori	r21,r8,00000000

l00007890:
	l.ori	r21,r12,00000000

l00007894:
	l.lwz	r16,2048(r8)
	l.sfne	r21,r16
	l.bf	000078B4
	l.addi	r8,r8,+00000004

l000078A4:
	l.addi	r4,r4,+00000001
	l.sfnei	r4,00000040
	l.bf	00007880
	l.addi	r12,r12,-00000004

l000078B4:
	l.xori	r4,r4,00000040
	l.sub	r27,r0,r4
	l.or	r4,r27,r4
	l.srli	r27,r4,0000001F
	l.addi	r4,r7,+0000000C
	l.sll	r8,r27,r4
	l.sll	r4,r25,r4
	l.xori	r4,r4,0000FFFF
	l.and	r14,r14,r4
	l.lwz	r4,0(r3)
	l.or	r14,r8,r14
	l.addi	r8,r0,-00000FFD
	l.and	r4,r4,r8
	l.ori	r4,r4,00000AA0
	l.sw	0(r3),r4
	l.addi	r4,r0,+00000009
	l.sll	r16,r13,r4

l000078F8:
	l.addi	r12,r0,+00000000
	l.ori	r21,r15,00000000
	l.add	r16,r16,r5
	l.ori	r8,r12,00000000

l00007908:
	l.andi	r18,r8,00000001
	l.sfeqi	r18,00000000
	l.bf	0000791C
	l.ori	r29,r21,00000000

l00007918:
	l.add	r29,r12,r5

l0000791C:
	l.add	r18,r12,r16
	l.lwz	r18,0(r18)
	l.sfeq	r29,r18
	l.bf	0000794C
	l.addi	r8,r8,+00000001

l00007930:
	l.addi	r8,r8,-00000001
	l.sfeqi	r8,00000040
	l.bnf	00007964
	l.addi	r4,r4,+00000001

l00007940:
	l.addi	r4,r4,-00000001
	l.j	00007978
	l.sfgtui	r4,0000000C

l0000794C:
	l.addi	r12,r12,+00000004
	l.sfnei	r8,00000040
	l.bf	00007908
	l.addi	r21,r21,-00000004

l0000795C:
	l.j	00007978
	l.sfgtui	r4,0000000C

l00007964:
	l.sfnei	r4,0000000E
	l.bf	000078F8
	l.sll	r16,r13,r4

l00007970:
	l.j	00007994
	l.addi	r4,r0,+0000000D

l00007978:
	l.bf	00007990
	l.sfeqi	r4,00000009

l00007980:
	l.bf	0000799C
	l.addi	r12,r0,+00000000

l00007988:
	l.j	00007998
	l.addi	r12,r4,-0000000A

l00007990:
	l.addi	r4,r0,+0000000D

l00007994:
	l.addi	r12,r4,-0000000A

l00007998:
	l.sll	r12,r13,r12

l0000799C:
	l.sll	r15,r25,r7
	l.sll	r12,r12,r7
	l.addi	r17,r17,+00000001
	l.xori	r15,r15,0000FFFF
	l.addi	r7,r7,+00000010
	l.and	r8,r14,r15
	l.sfgeu	r17,r11
	l.or	r8,r12,r8
	l.bf	000079F4
	l.sw	16(r2),r8

l000079C4:
	l.addi	r19,r19,+00000002
	l.add	r19,r19,r6
	l.add	r19,r19,r27
	l.add	r19,r19,r4
	l.addi	r4,r19,+00000001
	l.sll	r4,r13,r4
	l.add	r5,r5,r4

l000079E0:
	l.xori	r15,r5,0000FFFF
	l.ori	r8,r5,00000000
	l.ori	r6,r15,00000000
	l.j	0000776C
	l.addi	r4,r0,+00000000

l000079F4:
	l.sfeqi	r11,00000001
	l.bf	00007A20
	l.srli	r3,r8,00000010

l00007A00:
	l.addi	r4,r0,-00000F01
	l.andi	r8,r8,0000FFFF
	l.sfne	r3,r8
	l.bnf	00007A18
	l.and	r3,r23,r4

l00007A14:
	l.ori	r3,r3,00000100

l00007A18:
	l.sw	20(r2),r3
	l.addi	r11,r0,+00000001

l00007A20:
	l.addi	r1,r1,+00000014
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)

;; fn00007A3C: 00007A3C
;;   Called from:
;;     00007B24 (in fn00007AE0)
fn00007A3C proc
	l.lwz	r4,92(r3)
	l.sw	-8(r1),r2
	l.sw	-4(r1),r9
	l.andi	r4,r4,00002000
	l.addi	r1,r1,-00000008
	l.sfnei	r4,00000000
	l.bf	00007A70
	l.ori	r2,r3,00000000

l00007A5C:
	l.jal	00007428
	l.nop
	l.sfeqi	r11,00000000
	l.bf	00007AD0
	l.nop

l00007A70:
	l.lwz	r3,92(r2)
	l.andi	r3,r3,00004000
	l.sfnei	r3,00000000
	l.bf	00007A98
	l.nop

l00007A84:
	l.jal	00007634
	l.ori	r3,r2,00000000
	l.sfeqi	r11,00000000
	l.bf	00007AD0
	l.nop

l00007A98:
	l.jal	0000770C
	l.ori	r3,r2,00000000
	l.sfeqi	r11,00000000
	l.bf	00007AD0
	l.nop

l00007AAC:
	l.lwz	r3,92(r2)
	l.andi	r4,r3,00008000
	l.sfnei	r4,00000000
	l.bf	00007AD0
	l.addi	r11,r0,+00000001

l00007AC0:
	l.ori	r3,r3,00000003
	l.addi	r4,r0,-00006001
	l.and	r3,r3,r4
	l.sw	92(r2),r3

l00007AD0:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00007AE0: 00007AE0
;;   Called from:
;;     00007FC8 (in fn00007FAC)
fn00007AE0 proc
	l.movhi	r3,000001F0
	l.sw	-8(r1),r16
	l.ori	r3,r3,00001510
	l.sw	-16(r1),r2
	l.lwz	r16,0(r3)
	l.sw	-4(r1),r9
	l.sw	-12(r1),r14
	l.andi	r16,r16,00000003
	l.addi	r1,r1,-00000010
	l.sfnei	r16,00000000
	l.bf	00007B38
	l.ori	r2,r4,00000000

l00007B10:
	l.lwz	r3,92(r4)
	l.andi	r3,r3,00000001
	l.sfnei	r3,00000000
	l.bf	00007B38
	l.nop

l00007B24:
	l.jal	00007A3C
	l.ori	r3,r4,00000000
	l.sfeqi	r11,00000000
	l.bf	00007CE0
	l.ori	r4,r11,00000000

l00007B38:
	l.jal	000073EC
	l.ori	r3,r2,00000000
	l.sfeqi	r11,00000000
	l.bf	00007CE0
	l.ori	r4,r11,00000000

l00007B4C:
	l.lwz	r14,20(r2)
	l.sfgesi	r14,+00000000
	l.bf	00007B64
	l.srli	r14,r14,00000010

l00007B5C:
	l.j	00007B80
	l.andi	r14,r14,00007FFF

l00007B64:
	l.jal	00006E54
	l.nop
	l.lhz	r3,22(r2)
	l.slli	r4,r11,00000010
	l.ori	r14,r11,00000000
	l.or	r3,r4,r3
	l.sw	20(r2),r3

l00007B80:
	l.lwz	r3,92(r2)
	l.andi	r4,r3,00000200
	l.sfeqi	r4,00000000
	l.bf	00007BB4
	l.movhi	r7,0000FFFF

l00007B94:
	l.movhi	r4,000001C6
	l.movhi	r6,0000FFFF
	l.ori	r4,r4,00003100
	l.ori	r6,r6,00000FFF
	l.lwz	r5,0(r4)
	l.and	r5,r5,r6
	l.j	00007BC8
	l.ori	r5,r5,00005000

l00007BB4:
	l.movhi	r4,000001C6
	l.ori	r4,r4,00003100
	l.ori	r7,r7,00000FFF
	l.lwz	r5,0(r4)
	l.and	r5,r5,r7

l00007BC8:
	l.sw	0(r4),r5
	l.movhi	r4,000001C6
	l.movhi	r7,00008000
	l.ori	r5,r4,00003140
	l.lwz	r6,0(r5)
	l.or	r6,r6,r7
	l.sw	0(r5),r6
	l.andi	r5,r3,00000100
	l.sfeqi	r5,00000000
	l.bf	00007C08
	l.movhi	r5,00000400

l00007BF4:
	l.ori	r4,r4,000030B8
	l.lwz	r5,0(r4)
	l.ori	r5,r5,00000300
	l.sw	0(r4),r5
	l.movhi	r5,00000400

l00007C08:
	l.and	r4,r3,r5
	l.sfeqi	r4,00000000
	l.bf	00007C2C
	l.movhi	r4,000001C6

l00007C18:
	l.addi	r6,r0,-00002001
	l.ori	r4,r4,00003108
	l.lwz	r5,0(r4)
	l.j	00007C38
	l.and	r5,r5,r6

l00007C2C:
	l.ori	r4,r4,00003108
	l.lwz	r5,0(r4)
	l.ori	r5,r5,00002000

l00007C38:
	l.sw	0(r4),r5
	l.lwz	r4,4(r2)
	l.sfnei	r4,00000007
	l.bf	00007C74
	l.movhi	r7,00000800

l00007C4C:
	l.movhi	r4,000001C6
	l.movhi	r7,0000FFF0
	l.ori	r4,r4,0000307C
	l.ori	r7,r7,0000FFFF
	l.lwz	r5,0(r4)
	l.movhi	r6,00000001
	l.and	r5,r5,r7
	l.or	r5,r5,r6
	l.sw	0(r4),r5
	l.movhi	r7,00000800

l00007C74:
	l.and	r3,r3,r7
	l.sfnei	r3,00000000
	l.bf	00007C90
	l.sfeqi	r16,00000000

l00007C84:
	l.jal	00005078
	l.nop
	l.sfeqi	r16,00000000

l00007C90:
	l.bf	00007CB0
	l.nop

l00007C98:
	l.lwz	r2,92(r2)
	l.movhi	r3,00001000
	l.and	r2,r2,r3
	l.sfeqi	r2,00000000
	l.bf	00007CE0
	l.ori	r4,r14,00000000

l00007CB0:
	l.movhi	r2,000001C6
	l.addi	r3,r0,-00000001
	l.ori	r2,r2,00002094
	l.addi	r4,r0,+00000100
	l.sw	0(r2),r3
	l.jal	00007338
	l.ori	r3,r14,00000000
	l.addi	r4,r0,+00000000
	l.sfne	r11,r4
	l.bf	00007CE0
	l.nop

l00007CDC:
	l.ori	r4,r14,00000000

l00007CE0:
	l.addi	r1,r1,+00000010
	l.ori	r11,r4,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)
00007CFC                                     D7 E1 4F FC             ..O.
00007D00 D7 E1 17 F8 9C 60 02 58 9C 21 FF 98 9C 40 00 00 .....`.X.!...@..
00007D10 D4 01 18 00 9C 60 00 03 A8 81 00 00 D4 01 18 04 .....`..........
00007D20 18 60 00 3B D4 01 10 14 A8 63 3B F9 D4 01 10 1C .`.;.....c;.....
00007D30 D4 01 18 08 9C 60 00 01 D4 01 10 38 D4 01 18 0C .....`.....8....
00007D40 18 60 10 E4 D4 01 10 3C A8 63 10 E4 D4 01 10 40 .`.....<.c.....@
00007D50 D4 01 18 10 9C 60 08 40 D4 01 10 48 D4 01 18 18 .....`.@...H....
00007D60 9C 60 00 08 D4 01 10 4C D4 01 18 20 9C 60 00 02 .`.....L... .`..
00007D70 D4 01 18 24 18 60 00 47 A8 63 19 4F D4 01 18 28 ...$.`.G.c.O...(
00007D80 18 60 01 B1 A8 63 A9 4B D4 01 18 2C 18 60 00 06 .`...c.K...,.`..
00007D90 A8 63 10 43 D4 01 18 30 18 60 B4 78 A8 63 78 96 .c.C...0.`.x.cx.
00007DA0 D4 01 18 34 18 60 1E 08 A8 63 A1 E0 D4 01 18 44 ...4.`...c.....D
00007DB0 9C 60 55 05 D4 01 18 50 18 60 33 33 D4 01 18 54 .`U....P.`33...T
00007DC0 A8 60 BB BB D4 01 18 58 18 60 10 00 A8 63 81 D0 .`.....X.`...c..
00007DD0 D4 01 18 5C 07 FF FF 43 A8 62 00 00 9C 21 00 68 ...\...C.b...!.h
00007DE0 85 21 FF FC 44 00 48 00 84 41 FF F8 D7 E1 4F FC .!..D.H..A....O.
00007DF0 9C 21 FF FC 9C 21 00 04 85 21 FF FC 00 00 12 A9 .!...!...!......
00007E00 15 00 00 00                                     ....           

;; fn00007E04: 00007E04
;;   Called from:
;;     000107FC (in fn00010570)
fn00007E04 proc
	l.sw	-12(r1),r2
	l.movhi	r2,000001C6
	l.addi	r4,r0,+00000000
	l.ori	r3,r2,00002094
	l.sw	-4(r1),r9
	l.sw	-8(r1),r14
	l.sw	0(r3),r4
	l.addi	r1,r1,-0000000C
	l.jal	0000C8A0
	l.addi	r3,r0,+00000001
	l.ori	r3,r2,00003004
	l.ori	r2,r2,00003018
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000101
	l.sw	0(r3),r4

l00007E40:
	l.lwz	r3,0(r2)
	l.andi	r3,r3,00000007
	l.sfnei	r3,00000003
	l.bf	00007E40
	l.movhi	r14,000001C6

l00007E54:
	l.addi	r3,r0,+00000001
	l.jal	0000C8A0
	l.ori	r2,r14,0000310C
	l.movhi	r5,0000FF00
	l.lwz	r3,0(r2)
	l.ori	r5,r5,0000FFFF
	l.and	r3,r3,r5
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00000001
	l.ori	r3,r14,00003344
	l.movhi	r4,0000FF3F

l00007E84:
	l.lwz	r2,0(r3)
	l.ori	r4,r4,00000FF4
	l.movhi	r5,00000040
	l.and	r2,r2,r4
	l.ori	r5,r5,0000500A
	l.or	r2,r2,r5
	l.sw	0(r3),r2
	l.movhi	r2,000001C6
	l.addi	r3,r3,+00000080
	l.ori	r2,r2,00003544
	l.sfne	r3,r2
	l.bf	00007E84
	l.movhi	r4,0000FF3F

l00007EB8:
	l.movhi	r2,000001C6
	l.addi	r5,r0,-000003DC
	l.ori	r3,r2,00003208
	l.ori	r2,r2,0000300C
	l.lwz	r4,0(r3)
	l.and	r4,r4,r5
	l.ori	r4,r4,00000313
	l.sw	0(r3),r4
	l.movhi	r3,000001F0
	l.ori	r3,r3,00001510
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000003
	l.sw	0(r3),r4
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+00000000
	l.movhi	r5,00007FFF
	l.sw	0(r2),r3
	l.movhi	r3,000001C2
	l.ori	r5,r5,0000FFFF
	l.ori	r4,r3,00000020
	l.lwz	r2,0(r4)
	l.and	r2,r2,r5
	l.movhi	r5,00000010
	l.sw	0(r4),r2
	l.or	r2,r2,r5
	l.movhi	r5,00007FFF
	l.sw	0(r4),r2
	l.ori	r4,r3,000000F4
	l.ori	r5,r5,0000FFFF
	l.lwz	r2,0(r4)
	l.and	r2,r2,r5
	l.movhi	r5,00000001
	l.sw	0(r4),r2
	l.lwz	r2,0(r4)
	l.or	r2,r2,r5
	l.addi	r5,r0,-00004001
	l.sw	0(r4),r2
	l.ori	r2,r3,00000060
	l.lwz	r4,0(r2)
	l.and	r4,r4,r5
	l.movhi	r5,00007FFF
	l.sw	0(r2),r4
	l.ori	r2,r3,0000015C
	l.ori	r5,r5,0000FFFF
	l.lwz	r4,0(r2)
	l.ori	r3,r3,000002C0
	l.and	r4,r4,r5
	l.sw	0(r2),r4
	l.addi	r4,r0,-00004001
	l.lwz	r2,0(r3)
	l.and	r2,r2,r4
	l.sw	0(r3),r2
	l.jal	0000C8A0
	l.addi	r3,r0,+00000001
	l.addi	r1,r1,+0000000C
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn00007FAC: 00007FAC
;;   Called from:
;;     00011FEC (in fn00010570)
fn00007FAC proc
	l.movhi	r4,00000001
	l.sw	-4(r1),r9
	l.ori	r4,r4,00002E8C
	l.addi	r1,r1,-00000004
	l.lwz	r4,0(r4)
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	00007AE0
	l.addi	r3,r0,+00000000

;; fn00007FD0: 00007FD0
;;   Called from:
;;     00012064 (in fn00010570)
fn00007FD0 proc
	l.movhi	r3,000001C6
	l.addi	r4,r0,-00000001
	l.ori	r3,r3,00002094
	l.sw	-4(r1),r9
	l.sw	0(r3),r4
	l.addi	r1,r1,-00000004
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	0000AEDC
	l.ori	r3,r0,00009071

;; fn00008000: 00008000
;;   Called from:
;;     00010770 (in fn00010570)
fn00008000 proc
	l.movhi	r3,000001C6
	l.movhi	r4,00000001
	l.ori	r3,r3,00002094
	l.sw	-4(r1),r9
	l.sw	0(r3),r4
	l.addi	r1,r1,-00000004
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	0000AEDC
	l.ori	r3,r0,00009072

;; fn00008030: 00008030
;;   Called from:
;;     00012014 (in fn00010570)
fn00008030 proc
	l.slli	r4,r4,00000018
	l.addi	r6,r0,+00000001
	l.movhi	r5,000001C6
	l.sll	r6,r6,r3
	l.ori	r5,r5,00002094
	l.srai	r4,r4,00000018
	l.lwz	r7,0(r5)
	l.xori	r6,r6,0000FFFF
	l.sll	r3,r4,r3
	l.and	r6,r6,r7
	l.sw	-4(r1),r9
	l.or	r3,r6,r3
	l.addi	r1,r1,-00000004
	l.sw	0(r5),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+0000000A
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	0000AEDC
	l.ori	r3,r0,00009073

;; fn00008080: 00008080
;;   Called from:
;;     0000832C (in fn000082FC)
;;     00008344 (in fn000082FC)
;;     0000835C (in fn000082FC)
;;     00008374 (in fn000082FC)
;;     0000838C (in fn000082FC)
;;     000083A4 (in fn000082FC)
fn00008080 proc
	l.sw	-4(r1),r2
	l.addi	r2,r0,-00000004
	l.addi	r1,r1,-00000008
	l.and	r5,r3,r2
	l.movhi	r2,000001C1
	l.andi	r3,r3,00000003
	l.ori	r2,r2,00007000
	l.slli	r3,r3,00000003
	l.add	r5,r5,r2
	l.addi	r11,r0,+00000000
	l.lwz	r6,0(r5)
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000001
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r3
	l.sll	r3,r4,r3
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r3,r3,r6
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r5),r3
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn000080EC: 000080EC
;;   Called from:
;;     00008320 (in fn000082FC)
;;     00008338 (in fn000082FC)
;;     00008350 (in fn000082FC)
;;     00008368 (in fn000082FC)
;;     00008380 (in fn000082FC)
;;     00008398 (in fn000082FC)
fn000080EC proc
	l.sw	-4(r1),r2
	l.addi	r2,r0,-00000004
	l.addi	r1,r1,-00000008
	l.and	r5,r3,r2
	l.movhi	r2,000001C1
	l.andi	r3,r3,00000003
	l.ori	r2,r2,00007000
	l.slli	r3,r3,00000003
	l.add	r5,r5,r2
	l.addi	r11,r0,+00000000
	l.lwz	r6,0(r5)
	l.addi	r3,r3,+00000004
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000001
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r3
	l.sll	r3,r4,r3
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r3,r3,r6
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r5),r3
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)
0000815C                                     D7 E1 17 F0             ....
00008160 18 40 01 C1 D7 E1 77 F4 D7 E1 87 F8 D7 E1 4F FC .@....w.......O.
00008170 AA 02 71 44 9C 21 FF EC 00 00 00 1C A9 C2 71 84 ..qD.!........q.
00008180 84 4E 00 00 D4 01 10 00 84 61 00 00 04 00 1D BA .N.......a......
00008190 15 00 00 00 A8 6B 00 00 04 00 1B EF A8 4B 00 00 .....k.......K..
000081A0 BC 0B 00 00 10 00 00 11 15 00 00 00 8C 62 00 00 .............b..
000081B0 BC 23 00 05 10 00 00 0A 9C 80 00 03 9C 60 00 06 .#...........`..
000081C0 9C 80 00 01 D8 02 18 00 9C 60 00 00 D4 02 18 20 .........`..... 
000081D0 D4 02 20 24 00 00 00 05 15 00 00 00 A8 62 00 00 .. $.........b..
000081E0 04 00 1C 99 D8 02 20 00 84 70 00 00 BC 23 00 00 ...... ..p...#..
000081F0 13 FF FF E4 18 40 01 C1 A8 62 70 50 9C 80 00 04 .....@...bpP....
00008200 D4 03 20 00 A8 62 71 50 84 63 00 00 BC 03 00 00 .. ..bqP.c......
00008210 10 00 00 1C A8 42 71 90 84 42 00 00 D4 01 10 00 .....Bq..B......
00008220 84 61 00 00 04 00 1D 94 15 00 00 00 A8 6B 00 00 .a...........k..
00008230 04 00 1B C9 A8 4B 00 00 BC 0B 00 00 10 00 00 11 .....K..........
00008240 15 00 00 00 8C 62 00 00 BC 23 00 05 10 00 00 0A .....b...#......
00008250 9C 80 00 03 9C 60 00 06 9C 80 00 01 D8 02 18 00 .....`..........
00008260 9C 60 00 00 D4 02 18 20 D4 02 20 24 00 00 00 06 .`..... .. $....
00008270 18 40 01 C1 A8 62 00 00 04 00 1C 73 D8 02 20 00 .@...b.....s.. .
00008280 18 40 01 C1 9C 60 01 00 A8 42 70 50 9D 60 00 01 .@...`...BpP.`..
00008290 D4 02 18 00 9C 21 00 14 85 21 FF FC 84 41 FF F0 .....!...!...A..
000082A0 85 C1 FF F4 44 00 48 00 86 01 FF F8             ....D.H.....   

;; fn000082AC: 000082AC
;;   Called from:
;;     000083D8 (in fn000082FC)
;;     000083E0 (in fn000082FC)
fn000082AC proc
	l.movhi	r4,000001C1
	l.addi	r6,r0,+00000001
	l.ori	r4,r4,00007040
	l.add	r3,r3,r3
	l.lwz	r5,0(r4)
	l.addi	r1,r1,-00000004
	l.sll	r3,r6,r3
	l.sw	0(r1),r5
	l.addi	r11,r0,+00000000
	l.lwz	r5,0(r1)
	l.xori	r6,r3,0000FFFF
	l.and	r5,r6,r5
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.or	r3,r3,r5
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r4),r3
	l.jr	r9
	l.addi	r1,r1,+00000004

;; fn000082FC: 000082FC
;;   Called from:
;;     00008794 (in fn00008788)
;;     0000EA34 (in fn0000E98C)
fn000082FC proc
	l.sw	-4(r1),r9
	l.addi	r3,r0,+00000011
	l.addi	r1,r1,-00000004
	l.jal	0000B320
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000011
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000000
	l.jal	000080EC
	l.ori	r4,r3,00000000
	l.addi	r3,r0,+00000000
	l.jal	00008080
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000001
	l.jal	000080EC
	l.ori	r4,r3,00000000
	l.addi	r3,r0,+00000001
	l.jal	00008080
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000002
	l.jal	000080EC
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000002
	l.jal	00008080
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000003
	l.jal	000080EC
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000003
	l.jal	00008080
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000004
	l.jal	000080EC
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000004
	l.jal	00008080
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000005
	l.jal	000080EC
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000005
	l.jal	00008080
	l.addi	r4,r0,+00000001
	l.movhi	r3,00000001
	l.ori	r3,r3,0000315C
	l.lwz	r5,0(r3)
	l.sfnei	r5,00000000
	l.bf	000083D8
	l.movhi	r4,00000000

l000083C4:
	l.addi	r3,r0,+00000011
	l.jal	00008BB8
	l.ori	r4,r4,0000815C
	l.jal	00008B60
	l.addi	r3,r0,+00000011

l000083D8:
	l.jal	000082AC
	l.addi	r3,r0,+00000001
	l.jal	000082AC
	l.addi	r3,r0,+00000004
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn000083F8: 000083F8
;;   Called from:
;;     00008770 (in fn0000875C)
fn000083F8 proc
	l.sw	-4(r1),r9
	l.addi	r3,r0,+00000011
	l.addi	r1,r1,-00000004
	l.jal	0000BC38
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000011
	l.jal	0000B320
	l.addi	r4,r0,+00000000
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn00008428: 00008428
;;   Called from:
;;     0000EAF4 (in fn0000E98C)
fn00008428 proc
	l.sw	-20(r1),r2
	l.sw	-16(r1),r14
	l.sw	-4(r1),r9
	l.sw	-12(r1),r16
	l.sw	-8(r1),r18
	l.ori	r14,r3,00000000
	l.lbz	r3,1(r3)
	l.andi	r3,r3,00000002
	l.addi	r1,r1,-00000018
	l.sfnei	r3,00000000
	l.bf	0000847C
	l.ori	r2,r4,00000000

l00008458:
	l.j	000084FC
	l.movhi	r16,000001C1

l00008460:
	l.sfeqi	r2,00000000
	l.bf	00008558
	l.addi	r11,r0,-00000023

l0000846C:
	l.jal	0000C768
	l.addi	r2,r2,-00000001
	l.j	00008484
	l.nop

l0000847C:
	l.movhi	r18,000001C1
	l.ori	r18,r18,00007108

l00008484:
	l.lwz	r3,0(r18)
	l.sfeqi	r3,00000001
	l.bf	00008460
	l.movhi	r16,000001C1

l00008494:
	l.ori	r3,r14,00000000
	l.jal	0000F888
	l.ori	r2,r16,00007188
	l.sw	0(r1),r11
	l.ori	r16,r16,0000714C
	l.lwz	r3,0(r1)
	l.sw	0(r2),r3

l000084B0:
	l.lwz	r2,0(r16)
	l.sfeqi	r2,00000000
	l.bf	000084B0
	l.movhi	r2,000001C1

l000084C0:
	l.lwz	r3,0(r1)
	l.ori	r2,r2,0000718C
	l.addi	r11,r0,+00000000
	l.lwz	r2,0(r2)
	l.sfeq	r3,r2
	l.bf	00008558
	l.nop

l000084DC:
	l.j	00008558
	l.addi	r11,r0,-0000000E

l000084E4:
	l.bf	00008558
	l.addi	r11,r0,-00000023

l000084EC:
	l.jal	0000C768
	l.addi	r2,r2,-00000001
	l.j	00008500
	l.nop

l000084FC:
	l.ori	r16,r16,00007100

l00008500:
	l.lwz	r3,0(r16)
	l.sfeqi	r3,00000001
	l.bf	000084E4
	l.sfeqi	r2,00000000

l00008510:
	l.movhi	r2,000001C1
	l.ori	r3,r14,00000000
	l.jal	0000F888
	l.ori	r2,r2,00007180
	l.sw	0(r2),r11
	l.lbz	r11,1(r14)
	l.andi	r11,r11,00000001
	l.sfeqi	r11,00000000
	l.bf	00008558
	l.nop

l00008538:
	l.lwz	r2,32(r14)
	l.lwz	r3,36(r14)
	l.ori	r4,r2,00000000
	l.or	r2,r4,r3
	l.sfeqi	r2,00000000
	l.bf	00008538
	l.nop

l00008554:
	l.addi	r11,r0,+00000000

l00008558:
	l.addi	r1,r1,+00000018
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)

;; fn00008574: 00008574
;;   Called from:
;;     0000F818 (in fn0000F444)
;;     0000F838 (in fn0000F444)
;;     00012170 (in fn00010570)
fn00008574 proc
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-4(r1),r9
	l.sw	-8(r1),r16
	l.ori	r14,r3,00000000
	l.lbz	r3,1(r3)
	l.ori	r2,r4,00000000
	l.andi	r4,r3,00000002
	l.sfeqi	r4,00000000
	l.bf	000085EC
	l.addi	r1,r1,-00000010

l000085A0:
	l.j	000085C4
	l.movhi	r16,000001C1

l000085A8:
	l.jal	0000C768
	l.nop
	l.sfeqi	r2,00000000
	l.bf	00008658
	l.addi	r2,r2,-00000001

l000085BC:
	l.j	000085C8
	l.nop

l000085C4:
	l.ori	r16,r16,00007114

l000085C8:
	l.lwz	r3,0(r16)
	l.sfeqi	r3,00000001
	l.bf	000085A8
	l.nop

l000085D8:
	l.ori	r3,r14,00000000
	l.jal	0000F888
	l.movhi	r2,000001C1
	l.j	0000864C
	l.ori	r2,r2,00007194

l000085EC:
	l.andi	r4,r3,00000001
	l.sfnei	r4,00000000
	l.bf	00008628
	l.movhi	r16,000001C1

l000085FC:
	l.sfnei	r3,00000000
	l.bnf	00008628
	l.addi	r11,r0,-00000003

l00008608:
	l.j	00008660
	l.addi	r1,r1,+00000010

l00008610:
	l.bf	0000865C
	l.addi	r11,r0,-00000023

l00008618:
	l.jal	0000C768
	l.addi	r2,r2,-00000001
	l.j	0000862C
	l.nop

l00008628:
	l.ori	r16,r16,00007100

l0000862C:
	l.lwz	r3,0(r16)
	l.sfeqi	r3,00000001
	l.bf	00008610
	l.sfeqi	r2,00000000

l0000863C:
	l.movhi	r2,000001C1
	l.ori	r3,r14,00000000
	l.jal	0000F888
	l.ori	r2,r2,00007180

l0000864C:
	l.sw	0(r2),r11
	l.j	0000865C
	l.addi	r11,r0,+00000000

l00008658:
	l.addi	r11,r0,-00000023

l0000865C:
	l.addi	r1,r1,+00000010

l00008660:
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn00008674: 00008674
;;   Called from:
;;     00012110 (in fn00010570)
fn00008674 proc
	l.movhi	r3,000001C1
	l.sw	-8(r1),r2
	l.ori	r2,r3,00007144
	l.sw	-4(r1),r9
	l.lwz	r2,0(r2)
	l.sfeqi	r2,00000000
	l.bf	000086DC
	l.addi	r1,r1,-00000008

l00008694:
	l.ori	r3,r3,00007184
	l.lwz	r3,0(r3)
	l.jal	0000F874
	l.nop
	l.ori	r3,r11,00000000
	l.jal	0000F154
	l.ori	r2,r11,00000000
	l.sfeqi	r11,00000000
	l.bf	00008744
	l.nop

l000086BC:
	l.lbz	r3,0(r2)
	l.sfnei	r3,00000005
	l.bnf	000086D0
	l.addi	r3,r0,+00000006

l000086CC:
	l.addi	r3,r0,+00000003

l000086D0:
	l.sb	0(r2),r3
	l.j	00008730
	l.addi	r4,r0,+00000004

l000086DC:
	l.ori	r2,r3,00007150
	l.lwz	r2,0(r2)
	l.sfeqi	r2,00000000
	l.bf	00008748
	l.ori	r3,r3,00007190

l000086F0:
	l.lwz	r3,0(r3)
	l.jal	0000F874
	l.nop
	l.ori	r3,r11,00000000
	l.jal	0000F154
	l.ori	r2,r11,00000000
	l.sfeqi	r11,00000000
	l.bf	00008744
	l.nop

l00008714:
	l.lbz	r3,0(r2)
	l.sfnei	r3,00000005
	l.bnf	00008728
	l.addi	r3,r0,+00000006

l00008724:
	l.addi	r3,r0,+00000003

l00008728:
	l.sb	0(r2),r3
	l.addi	r4,r0,+00000100

l00008730:
	l.movhi	r3,000001C1
	l.ori	r3,r3,00007050
	l.sw	0(r3),r4
	l.j	0000874C
	l.addi	r1,r1,+00000008

l00008744:
	l.ori	r2,r11,00000000

l00008748:
	l.addi	r1,r1,+00000008

l0000874C:
	l.ori	r11,r2,00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000875C: 0000875C
;;   Called from:
;;     000106F8 (in fn00010570)
fn0000875C proc
	l.movhi	r3,00000001
	l.addi	r4,r0,+00000001
	l.ori	r3,r3,0000315C
	l.sw	-4(r1),r9
	l.sw	0(r3),r4
	l.jal	000083F8
	l.addi	r1,r1,-00000004
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn00008788: 00008788
;;   Called from:
;;     000120D8 (in fn00010570)
fn00008788 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r1,r1,-00000008
	l.jal	000082FC
	l.addi	r2,r0,+00000000
	l.movhi	r3,00000001
	l.ori	r11,r2,00000000
	l.ori	r3,r3,0000315C
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn000087BC: 000087BC
;;   Called from:
;;     0000E9C4 (in fn0000E98C)
fn000087BC proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r3,r0,+00000010
	l.addi	r1,r1,-00000008
	l.addi	r4,r0,+00000001
	l.jal	0000B320
	l.addi	r2,r0,+00000000
	l.addi	r3,r0,+00000010
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.movhi	r3,00000001
	l.ori	r11,r2,00000000
	l.ori	r3,r3,000037E8
	l.sw	4(r3),r2
	l.sw	12(r3),r2
	l.sw	20(r3),r2
	l.sw	28(r3),r2
	l.sw	36(r3),r2
	l.sw	44(r3),r2
	l.sw	52(r3),r2
	l.sw	60(r3),r2
	l.movhi	r3,00000001
	l.ori	r3,r3,00003160
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000882C                                     44 00 48 00             D.H.
00008830 9D 60 00 00                                     .`..           

;; fn00008834: 00008834
;;   Called from:
;;     0000F2D8 (in fn0000F250)
;;     0000F3F0 (in fn0000F368)
fn00008834 proc
	l.sw	-12(r1),r2
	l.sw	-8(r1),r14
	l.sw	-4(r1),r9
	l.addi	r1,r1,-0000000C
	l.ori	r2,r3,00000000
	l.jal	0000E718
	l.ori	r14,r4,00000000
	l.movhi	r5,00000001
	l.slli	r4,r2,00000003
	l.ori	r5,r5,000037E8
	l.ori	r3,r11,00000000
	l.add	r4,r4,r5
	l.addi	r5,r0,-0000000D
	l.sw	0(r4),r11
	l.movhi	r4,00000001
	l.ori	r4,r4,00003160
	l.lwz	r4,0(r4)
	l.sfnei	r4,00000000
	l.bf	000088E4
	l.movhi	r6,00000070

l00008884:
	l.movhi	r5,0000001E
	l.ori	r5,r5,00008480
	l.ori	r6,r6,00006040
	l.mul	r4,r14,r5
	l.add	r5,r2,r6
	l.j	000088B8
	l.slli	r5,r5,00000002

l000088A0:
	l.bf	000088B8
	l.addi	r4,r4,-00000001

l000088A8:
	l.jal	0000E740
	l.addi	r4,r4,+00000001
	l.j	000088E4
	l.addi	r5,r0,-00000023

l000088B8:
	l.lwz	r6,0(r5)
	l.sfeqi	r6,00000001
	l.bf	000088A0
	l.sfnei	r4,00000000

l000088C8:
	l.movhi	r3,00000001
	l.slli	r2,r2,00000003
	l.ori	r3,r3,000037E8
	l.addi	r5,r0,+00000000
	l.add	r2,r2,r3
	l.addi	r3,r0,+00000001
	l.sw	4(r2),r3

l000088E4:
	l.addi	r1,r1,+0000000C
	l.ori	r11,r5,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn000088FC: 000088FC
;;   Called from:
;;     0000F32C (in fn0000F250)
;;     0000F410 (in fn0000F368)
fn000088FC proc
	l.sw	-8(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.ori	r2,r2,00003160
	l.addi	r1,r1,-00000008
	l.lwz	r2,0(r2)
	l.sfeqi	r2,00000000
	l.bf	0000893C
	l.slli	r4,r3,00000003

l00008920:
	l.movhi	r2,00000001
	l.ori	r2,r2,000037E8
	l.add	r4,r4,r2
	l.jal	0000E740
	l.lwz	r3,0(r4)
	l.j	0000896C
	l.addi	r11,r0,-0000000D

l0000893C:
	l.movhi	r5,00000070
	l.ori	r5,r5,00006040
	l.add	r3,r3,r5
	l.movhi	r5,00000001
	l.slli	r3,r3,00000002
	l.ori	r5,r5,000037E8
	l.sw	0(r3),r2
	l.add	r3,r4,r5
	l.sw	4(r3),r2
	l.jal	0000E740
	l.lwz	r3,0(r3)
	l.ori	r11,r2,00000000

l0000896C:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000897C: 0000897C
;;   Called from:
;;     0000A968 (in fn0000A92C)
;;     00010700 (in fn00010570)
fn0000897C proc
	l.movhi	r3,00000001
	l.addi	r4,r0,+00000001
	l.ori	r3,r3,00003160
	l.addi	r11,r0,+00000000
	l.sw	0(r3),r4
	l.jr	r9
	l.nop

;; fn00008998: 00008998
;;   Called from:
;;     000120E0 (in fn00010570)
fn00008998 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r3,r0,+00000010
	l.addi	r1,r1,-00000008
	l.addi	r4,r0,+00000001
	l.jal	0000B320
	l.addi	r2,r0,+00000000
	l.addi	r3,r0,+00000010
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.movhi	r3,00000001
	l.ori	r11,r2,00000000
	l.ori	r3,r3,00003160
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn000089E0: 000089E0
;;   Called from:
;;     00008AF8 (in fn00008AF0)
fn000089E0 proc
	l.movhi	r3,000001F0
	l.movhi	r4,00000001
	l.ori	r3,r3,00000C00
	l.ori	r4,r4,00003828
	l.sw	-4(r1),r2
	l.addi	r2,r0,+00000000
	l.sw	0(r4),r3
	l.sw	64(r3),r2
	l.addi	r4,r0,-00000001
	l.sw	80(r3),r2
	l.addi	r1,r1,-00000004
	l.sw	16(r3),r4
	l.addi	r1,r1,+00000004
	l.ori	r11,r2,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00008A20: 00008A20
fn00008A20 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r2
	l.ori	r3,r3,00003828
	l.addi	r2,r0,+00000000
	l.addi	r1,r1,-00000004
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000004
	l.ori	r11,r2,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)

l00008A48:
	l.sfnei	r3,00000000
	l.bf	00008A64
	l.movhi	r4,00000001

l00008A54:
	l.addi	r5,r0,+00000001
	l.ori	r4,r4,00003828
	l.lwz	r4,0(r4)
	l.sw	16(r4),r5

l00008A64:
	l.movhi	r4,00000001
	l.addi	r6,r0,+00000001
	l.ori	r4,r4,00003828
	l.sll	r3,r6,r3
	l.lwz	r4,0(r4)
	l.addi	r11,r0,+00000000
	l.lwz	r5,64(r4)
	l.or	r3,r3,r5
	l.sw	64(r4),r3
	l.jr	r9
	l.nop

l00008A90:
	l.movhi	r4,00000001
	l.addi	r6,r0,+00000001
	l.ori	r4,r4,00003828
	l.sll	r3,r6,r3
	l.lwz	r4,0(r4)
	l.addi	r11,r0,+00000000
	l.lwz	r5,64(r4)
	l.xori	r3,r3,0000FFFF
	l.and	r3,r3,r5
	l.sw	64(r4),r3
	l.jr	r9
	l.nop

;; fn00008AC0: 00008AC0
;;   Called from:
;;     00008C40 (in fn0000E758)
fn00008AC0 proc
	l.movhi	r3,00000001
	l.addi	r1,r1,-00000004
	l.ori	r3,r3,00003828
	l.lwz	r3,0(r3)
	l.lwz	r3,0(r3)
	l.srli	r3,r3,00000002
	l.sw	0(r1),r3
	l.lwz	r11,0(r1)
	l.jr	r9
	l.addi	r1,r1,+00000004
00008AE8                         44 00 48 00 9D 60 00 01         D.H..`..

;; fn00008AF0: 00008AF0
;;   Called from:
;;     0000E9D4 (in fn0000E98C)
fn00008AF0 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.jal	000089E0
	l.addi	r1,r1,-00000008
	l.movhi	r3,00000001
	l.movhi	r4,00000000
	l.ori	r3,r3,00003834
	l.ori	r4,r4,00008AE8

l00008B10:
	l.addi	r2,r0,+00000000
	l.sw	0(r3),r4
	l.sw	4(r3),r2
	l.movhi	r2,00000001
	l.addi	r3,r3,+00000008
	l.ori	r2,r2,00003934
	l.sfne	r3,r2
	l.bf	00008B10
	l.addi	r11,r0,+00000000

l00008B34:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
00008B44             D7 E1 4F FC 07 FF FF B6 9C 21 FF FC     ..O......!..
00008B50 9C 21 00 04 85 21 FF FC 44 00 48 00 9D 60 00 00 .!...!..D.H..`..

;; fn00008B60: 00008B60
;;   Called from:
;;     000083D0 (in fn000082FC)
;;     00009140 (in fn00009290)
;;     0000A680 (in fn0000A664)
;;     0000C62C (in fn0000C588)
;;     0000F6B0 (in fn0000F444)
;;     000106D4 (in fn00010570)
fn00008B60 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	00008A48
	l.nop

;; fn00008B78: 00008B78
;;   Called from:
;;     0000F698 (in fn0000F444)
fn00008B78 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	00008A90
	l.nop

;; fn00008B90: 00008B90
;;   Called from:
;;     0000F6D0 (in fn0000F444)
fn00008B90 proc
	l.movhi	r4,00000001
	l.addi	r11,r0,+00000000
	l.ori	r4,r4,00003828
	l.lwz	r4,0(r4)
	l.sw	12(r4),r3
	l.lwz	r3,80(r4)
	l.ori	r3,r3,00000001
	l.sw	80(r4),r3
	l.jr	r9
	l.nop

;; fn00008BB8: 00008BB8
;;   Called from:
;;     000083C8 (in fn000082FC)
;;     0000C538 (in fn0000C48C)
fn00008BB8 proc
	l.sw	-4(r1),r2
	l.movhi	r2,00000001
	l.slli	r3,r3,00000003
	l.ori	r2,r2,00003834
	l.addi	r1,r1,-00000004
	l.add	r3,r3,r2
	l.addi	r11,r0,+00000000
	l.sw	0(r3),r4
	l.sw	4(r3),r5
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)
00008BE8                         D7 E1 17 FC 18 40 00 01         .....@..
00008BF0 B8 63 00 03 A8 42 38 34 9C 21 FF FC E0 63 10 00 .c...B84.!...c..
00008C00 84 A3 00 00 E4 25 20 00 10 00 00 08 9D 60 FF DC .....% ......`..
00008C10 18 80 00 00 9C 40 00 00 A8 84 8A E8 D4 03 10 04 .....@..........
00008C20 D4 03 20 00 A9 62 00 00 9C 21 00 04 44 00 48 00 .. ..b...!..D.H.
00008C30 84 41 FF FC                                     .A..           

l00008C34:
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r1,r1,-00000008
	l.jal	00008AC0
	l.movhi	r2,00000001
	l.slli	r3,r11,00000003
	l.ori	r2,r2,00003834
	l.add	r3,r3,r2
	l.lwz	r11,0(r3)
	l.jalr	r11
	l.lwz	r3,4(r3)
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00008C74: 00008C74
;;   Called from:
;;     00011534 (in fn00010570)
;;     00011580 (in fn00010570)
;;     00011594 (in fn00010570)
;;     000115A8 (in fn00010570)
;;     000115F0 (in fn00010570)
;;     00011638 (in fn00010570)
;;     00011664 (in fn00010570)
;;     000116D0 (in fn00010570)
;;     000116E4 (in fn00010570)
;;     00011710 (in fn00010570)
;;     00012350 (in fn00012320)
;;     000123A8 (in fn00012320)
fn00008C74 proc
	l.movhi	r4,00000001
	l.addi	r5,r0,+00000001
	l.ori	r4,r4,00003828
	l.sll	r3,r5,r3
	l.lwz	r4,0(r4)
	l.addi	r1,r1,-00000004
	l.lwz	r4,16(r4)
	l.and	r3,r3,r4
	l.sw	0(r1),r3
	l.lwz	r11,0(r1)
	l.jr	r9
	l.addi	r1,r1,+00000004

;; fn00008CA4: 00008CA4
;;   Called from:
;;     00009138 (in fn00009290)
;;     0000F6A8 (in fn0000F444)
;;     0000F6C0 (in fn0000F444)
;;     00011548 (in fn00010570)
;;     00011604 (in fn00010570)
;;     00012364 (in fn00012320)
;;     000123BC (in fn00012320)
fn00008CA4 proc
	l.movhi	r4,00000001
	l.addi	r5,r0,+00000001
	l.ori	r4,r4,00003828
	l.sll	r3,r5,r3
	l.lwz	r4,0(r4)
	l.addi	r11,r0,+00000000
	l.sw	16(r4),r3
	l.jr	r9
	l.nop

;; fn00008CC8: 00008CC8
;;   Called from:
;;     0001068C (in fn00010570)
fn00008CC8 proc
	l.movhi	r3,00000001
	l.movhi	r4,00000001
	l.ori	r3,r3,00003828
	l.ori	r4,r4,0000382C
	l.lwz	r3,0(r3)
	l.sw	-4(r1),r2
	l.lwz	r5,64(r3)
	l.addi	r2,r0,+00000000
	l.sw	0(r4),r5
	l.movhi	r4,00000001
	l.lwz	r5,80(r3)
	l.ori	r4,r4,00003830
	l.sw	64(r3),r2
	l.addi	r1,r1,-00000004
	l.sw	80(r3),r2
	l.sw	0(r4),r5
	l.addi	r1,r1,+00000004
	l.ori	r11,r2,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)
00008D18                         18 60 00 01 9C 80 FF FF         .`......
00008D20 A8 63 38 28 9D 60 00 00 84 63 00 00 D4 03 20 10 .c8(.`...c.... .
00008D30 18 80 00 01 A8 84 38 2C 84 84 00 00 D4 03 20 40 ......8,...... @
00008D40 18 80 00 01 A8 84 38 30 84 84 00 00 D4 03 20 50 ......80...... P
00008D50 44 00 48 00 15 00 00 00                         D.H.....       

;; fn00008D58: 00008D58
;;   Called from:
;;     00009304 (in fn000092CC)
;;     00009334 (in fn000092CC)
fn00008D58 proc
	l.movhi	r6,000001F0
	l.sw	-4(r1),r2
	l.ori	r3,r6,00002030
	l.addi	r1,r1,-00000004
	l.lwz	r7,0(r3)
	l.movhi	r3,00000001
	l.srli	r7,r7,00000008
	l.ori	r3,r3,00003374
	l.andi	r7,r7,0000007F
	l.lwz	r4,0(r3)
	l.add	r5,r7,r4
	l.sfgtui	r5,000000FF
	l.bf	00008DCC
	l.addi	r11,r0,-0000001C

l00008D90:
	l.addi	r4,r4,+00000004
	l.ori	r6,r6,00002020
	l.add	r4,r4,r3
	l.j	00008DB4
	l.addi	r3,r0,+00000000

l00008DA4:
	l.lwz	r8,0(r6)
	l.addi	r3,r3,+00000001
	l.sb	0(r4),r8
	l.addi	r4,r4,+00000001

l00008DB4:
	l.sfne	r3,r7
	l.bf	00008DA4
	l.addi	r11,r0,+00000000

l00008DC0:
	l.movhi	r3,00000001
	l.ori	r3,r3,00003374
	l.sw	0(r3),r5

l00008DCC:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00008DD8: 00008DD8
;;   Called from:
;;     00009374 (in fn000092CC)
fn00008DD8 proc
	l.sw	-4(r1),r2
	l.addi	r1,r1,-00000130
	l.addi	r5,r1,+00000104
	l.addi	r6,r1,+0000012C
	l.ori	r4,r5,00000000
	l.addi	r2,r0,+00000000

l00008DF0:
	l.sb	0(r4),r2
	l.addi	r4,r4,+00000001
	l.sfne	r4,r6
	l.bf	00008DF0
	l.nop

l00008E04:
	l.addi	r4,r0,+00000000
	l.j	00008E2C
	l.addi	r6,r1,+00000005

l00008E10:
	l.add	r7,r6,r4
	l.ori	r2,r2,00003374
	l.add	r8,r4,r2
	l.addi	r4,r4,+00000001
	l.lbz	r8,4(r8)
	l.andi	r4,r4,000000FF
	l.sb	0(r7),r8

l00008E2C:
	l.sfltu	r4,r3
	l.bf	00008E10
	l.movhi	r2,00000001

l00008E38:
	l.addi	r2,r0,+00000014
	l.addi	r4,r0,+00000000
	l.sb	4(r1),r2
	l.addi	r3,r3,+00000001
	l.ori	r7,r4,00000000
	l.addi	r15,r1,+00000004
	l.j	00008F18
	l.addi	r6,r1,+00000104

l00008E58:
	l.lbz	r11,0(r8)
	l.andi	r8,r11,0000007F
	l.sflesi	r8,+00000013
	l.bf	00008E7C
	l.addi	r12,r0,+00000000

l00008E6C:
	l.sflesi	r8,+00000028
	l.bf	00008E7C
	l.addi	r12,r0,+00000001

l00008E78:
	l.addi	r12,r0,+00000000

l00008E7C:
	l.sfeqi	r12,00000000
	l.bf	00008EB0
	l.sflesi	r8,+00000027

l00008E88:
	l.slli	r11,r11,00000018
	l.sfgesi	r11,+00000000
	l.bf	00008EA4
	l.nop

l00008E98:
	l.add	r8,r6,r4
	l.j	00008F04
	l.addi	r2,r0,+00000001

l00008EA4:
	l.add	r8,r6,r4
	l.j	00008F04
	l.addi	r2,r0,+00000000

l00008EB0:
	l.bf	00008EC8
	l.ori	r13,r12,00000000

l00008EB8:
	l.sflesi	r8,+00000050
	l.bf	00008EC8
	l.addi	r13,r0,+00000001

l00008EC4:
	l.ori	r13,r12,00000000

l00008EC8:
	l.slli	r13,r13,00000018
	l.sfeqi	r13,00000000
	l.bf	00008F10
	l.slli	r11,r11,00000018

l00008ED8:
	l.sfgesi	r11,+00000000
	l.bf	00008EF0
	l.addi	r8,r4,+00000001

l00008EE4:
	l.add	r4,r6,r4
	l.j	00008EF8
	l.addi	r2,r0,+00000001

l00008EF0:
	l.add	r4,r6,r4
	l.addi	r2,r0,+00000000

l00008EF8:
	l.sb	0(r4),r2
	l.andi	r4,r8,000000FF
	l.add	r8,r6,r4

l00008F04:
	l.addi	r4,r4,+00000001
	l.sb	0(r8),r2
	l.andi	r4,r4,000000FF

l00008F10:
	l.addi	r7,r7,+00000001
	l.andi	r7,r7,000000FF

l00008F18:
	l.sfltu	r7,r3
	l.bf	00008F28
	l.addi	r8,r0,+00000001

l00008F24:
	l.addi	r8,r0,+00000000

l00008F28:
	l.andi	r8,r8,000000FF
	l.sfeqi	r8,00000000
	l.bf	00008F54
	l.sfleui	r7,00000027

l00008F38:
	l.bf	00008F44
	l.addi	r8,r0,+00000001

l00008F40:
	l.addi	r8,r0,+00000000

l00008F44:
	l.andi	r8,r8,000000FF
	l.sfnei	r8,00000000
	l.bf	00008E58
	l.add	r8,r15,r7

l00008F54:
	l.sfleui	r4,0000001A
	l.bf	00008FC4
	l.addi	r11,r0,-00000001

l00008F60:
	l.addi	r4,r5,+0000001C
	l.addi	r3,r0,+00000000

l00008F68:
	l.lbz	r6,0(r5)
	l.sfnei	r6,00000000
	l.bf	00008F7C
	l.add	r3,r3,r3

l00008F78:
	l.ori	r3,r3,00000001

l00008F7C:
	l.addi	r5,r5,+00000002
	l.sfne	r5,r4
	l.bf	00008F68
	l.nop

l00008F8C:
	l.andi	r11,r3,000007C0
	l.andi	r5,r3,0000003F
	l.andi	r3,r3,00001000
	l.srli	r11,r11,00000006
	l.sfeqi	r3,00000000
	l.bf	00008FAC
	l.addi	r4,r0,+00000040

l00008FA8:
	l.addi	r4,r0,+00000000

l00008FAC:
	l.add	r3,r4,r5
	l.slli	r4,r3,00000010
	l.xori	r3,r3,0000FFFF
	l.slli	r3,r3,00000018
	l.or	r11,r4,r11
	l.or	r11,r11,r3

l00008FC4:
	l.addi	r1,r1,+00000130
	l.jr	r9
	l.lwz	r2,-4(r1)

l00008FD0:
	l.movhi	r3,000001F0
	l.sw	-24(r1),r2
	l.movhi	r4,00000001
	l.ori	r2,r3,00002000
	l.ori	r4,r4,00003484
	l.lwz	r5,0(r2)
	l.sw	-4(r1),r9
	l.sw	-20(r1),r14
	l.sw	-16(r1),r16
	l.sw	-12(r1),r18
	l.sw	-8(r1),r20
	l.sw	0(r4),r5
	l.ori	r18,r3,00002010
	l.movhi	r4,00000001
	l.lwz	r5,0(r18)
	l.ori	r4,r4,00003488
	l.ori	r14,r3,0000202C
	l.sw	0(r4),r5
	l.movhi	r4,00000001
	l.lwz	r5,0(r14)
	l.ori	r4,r4,0000348C
	l.ori	r16,r3,00002030
	l.sw	0(r4),r5
	l.movhi	r4,00000001
	l.lwz	r5,0(r16)
	l.ori	r4,r4,00003490
	l.ori	r20,r3,00002034
	l.sw	0(r4),r5
	l.movhi	r3,00000001
	l.lwz	r4,0(r20)
	l.ori	r3,r3,00003494
	l.addi	r1,r1,-00000018
	l.sw	0(r3),r4
	l.jal	0000AF1C
	l.addi	r3,r0,+0000001F
	l.movhi	r3,00000001
	l.ori	r3,r3,0000347C
	l.sw	0(r3),r11
	l.jal	0000B250
	l.addi	r3,r0,+0000001F
	l.movhi	r3,00000001
	l.addi	r4,r0,+0000000B
	l.ori	r3,r3,00003480
	l.addi	r5,r0,+00000002
	l.sw	0(r3),r11
	l.jal	00009744
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+0000000B
	l.jal	000097BC
	l.ori	r5,r3,00000000
	l.addi	r5,r0,+00000000
	l.addi	r3,r0,+00000001
	l.jal	00009834
	l.addi	r4,r0,+0000000B
	l.addi	r3,r0,+0000001F
	l.jal	0000B610
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+0000001F
	l.jal	0000B320
	l.addi	r4,r0,+00000001
	l.addi	r4,r0,+00000001
	l.jal	0000B0B8
	l.addi	r3,r0,+0000001F
	l.jal	0000BDC8
	l.addi	r3,r0,+0000001F
	l.addi	r3,r0,+0000001F
	l.jal	0000B320
	l.addi	r4,r0,+00000001
	l.addi	r4,r0,+00000001
	l.jal	0000B320
	l.addi	r3,r0,+00000017
	l.addi	r3,r0,+00000030
	l.sw	0(r2),r3
	l.movhi	r3,00000100
	l.ori	r3,r3,0000032C
	l.sw	0(r20),r3
	l.addi	r3,r0,+00000004
	l.sw	0(r18),r3
	l.addi	r3,r0,+000000FF
	l.sw	0(r16),r3
	l.addi	r3,r0,+00001F13
	l.sw	0(r14),r3
	l.lwz	r3,0(r2)
	l.ori	r3,r3,00000003
	l.sw	0(r2),r3
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000000
	l.ori	r2,r2,00003374
	l.sw	0(r2),r3
	l.jal	00008CA4
	l.addi	r3,r0,+00000005
	l.jal	00008B60
	l.addi	r3,r0,+00000005
	l.addi	r1,r1,+00000018
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-24(r1)
	l.lwz	r14,-20(r1)
	l.lwz	r16,-16(r1)
	l.lwz	r18,-12(r1)
	l.jr	r9
	l.lwz	r20,-8(r1)
0000916C                                     18 80 00 01             ....
00009170 D7 E1 4F FC A8 84 34 7C 9C 21 FF FC 9C 60 00 1F ..O...4|.!...`..
00009180 04 00 09 24 84 84 00 00 18 80 00 01 9C 60 00 1F ...$.........`..
00009190 A8 84 34 80 04 00 07 C9 84 84 00 00 04 00 0B 0B ..4.............
000091A0 9C 60 00 1F 18 60 00 01 9D 60 00 00 A8 63 34 84 .`...`...`...c4.
000091B0 84 A3 00 00 18 60 01 F0 A8 83 20 00 D4 04 28 00 .....`.... ...(.
000091C0 18 80 00 01 A8 84 34 88 84 A4 00 00 A8 83 20 10 ......4....... .
000091D0 D4 04 28 00 18 80 00 01 A8 84 34 8C 84 A4 00 00 ..(.......4.....
000091E0 A8 83 20 2C D4 04 28 00 18 80 00 01 A8 84 34 90 .. ,..(.......4.
000091F0 84 A4 00 00 A8 83 20 30 A8 63 20 34 D4 04 28 00 ...... 0.c 4..(.
00009200 18 80 00 01 A8 84 34 94 84 84 00 00 D4 03 20 00 ......4....... .
00009210 9C 21 00 04 85 21 FF FC 44 00 48 00 15 00 00 00 .!...!..D.H.....

l00009220:
	l.sw	-8(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.ori	r2,r2,0000316C
	l.ori	r4,r3,00000000
	l.addi	r1,r1,-00000008
	l.ori	r3,r2,00000000
	l.jal	0000DF10
	l.addi	r5,r0,+00000204
	l.lwz	r3,0(r2)
	l.movhi	r4,00001000
	l.addi	r11,r0,+00000000
	l.and	r5,r3,r4
	l.movhi	r4,00000001
	l.ori	r4,r4,00003164
	l.sw	0(r4),r5
	l.movhi	r4,00000FFF
	l.ori	r4,r4,0000FFFF
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
00009280 18 60 00 00 A8 63 40 00 44 00 48 00 85 63 03 B0 .`...c@.D.H..c..

;; fn00009290: 00009290
;;   Called from:
;;     00012330 (in fn00012320)
fn00009290 proc
	l.movhi	r3,00000000
	l.sw	-4(r1),r9
	l.ori	r3,r3,00004000
	l.lwz	r3,944(r3)
	l.sfeqi	r3,00000000
	l.bf	000092BC
	l.addi	r1,r1,-00000004

l000092AC:
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	00008FD0
	l.nop

l000092BC:
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.nop

;; fn000092CC: 000092CC
;;   Called from:
;;     0001160C (in fn00010570)
;;     000123C4 (in fn00012320)
fn000092CC proc
	l.sw	-12(r1),r2
	l.movhi	r3,000001F0
	l.movhi	r2,00000001
	l.ori	r3,r3,00002030
	l.ori	r2,r2,00003164
	l.sw	-8(r1),r14
	l.lwz	r14,0(r2)
	l.lwz	r2,0(r3)
	l.sw	-4(r1),r9
	l.sw	0(r3),r2
	l.andi	r3,r2,00000010
	l.sfeqi	r3,00000000
	l.bf	00009324
	l.addi	r1,r1,-0000000C

l00009304:
	l.jal	00008D58
	l.nop
	l.sfeqi	r11,00000000
	l.bf	00009324
	l.addi	r3,r0,+00000000

l00009318:
	l.movhi	r2,00000001
	l.j	00009678
	l.ori	r2,r2,00003374

l00009324:
	l.andi	r3,r2,00000002
	l.sfeqi	r3,00000000
	l.bf	00009668
	l.andi	r11,r2,00000001

l00009334:
	l.jal	00008D58
	l.nop
	l.sfeqi	r11,00000000
	l.bf	00009360
	l.movhi	r7,00001000

l00009348:
	l.movhi	r2,00000001
	l.addi	r5,r0,+00000000
	l.ori	r2,r2,00003374
	l.ori	r11,r5,00000000
	l.j	000096E4
	l.sw	0(r2),r5

l00009360:
	l.movhi	r2,00000001
	l.sfne	r14,r7
	l.bf	00009388
	l.ori	r2,r2,00003374

l00009370:
	l.lwz	r3,0(r2)
	l.jal	00008DD8
	l.movhi	r2,00000001
	l.ori	r2,r2,00003478
	l.j	00009574
	l.sw	0(r2),r11

l00009388:
	l.lwz	r7,0(r2)
	l.ori	r3,r11,00000000
	l.addi	r2,r2,+00000004
	l.j	000093D0
	l.ori	r5,r11,00000000

l0000939C:
	l.lbz	r3,0(r2)
	l.andi	r4,r3,00000080
	l.sfeqi	r4,00000000
	l.bf	000093BC
	l.sfgtui	r5,00000070

l000093B0:
	l.andi	r4,r3,0000007F
	l.j	000093C8
	l.add	r5,r5,r4

l000093BC:
	l.bf	000093DC
	l.nop

l000093C4:
	l.ori	r5,r4,00000000

l000093C8:
	l.addi	r11,r11,+00000001
	l.addi	r2,r2,+00000001

l000093D0:
	l.sfltu	r11,r7
	l.bf	0000939C
	l.nop

l000093DC:
	l.srli	r2,r3,00000007
	l.andi	r2,r2,00000001
	l.sfnei	r2,00000000
	l.bf	00009548
	l.sfleui	r5,00000070

l000093F0:
	l.bf	000093FC
	l.addi	r4,r0,+00000001

l000093F8:
	l.ori	r4,r2,00000000

l000093FC:
	l.andi	r4,r4,000000FF
	l.sfnei	r4,00000000
	l.bf	00009548
	l.sfltu	r11,r7

l0000940C:
	l.j	00009450
	l.nop

l00009414:
	l.ori	r8,r8,00003378
	l.add	r2,r11,r8
	l.lbz	r3,0(r2)
	l.andi	r2,r3,00000080
	l.sfeqi	r2,00000000
	l.bf	00009440
	l.sfgtui	r4,00000038

l00009430:
	l.bf	00009458
	l.nop

l00009438:
	l.j	00009448
	l.addi	r4,r0,+00000000

l00009440:
	l.andi	r2,r3,0000007F
	l.add	r4,r4,r2

l00009448:
	l.addi	r11,r11,+00000001
	l.sfltu	r11,r7

l00009450:
	l.bf	00009414
	l.movhi	r8,00000001

l00009458:
	l.andi	r3,r3,00000080
	l.sub	r2,r0,r3
	l.or	r3,r2,r3
	l.xori	r3,r3,0000FFFF
	l.srli	r3,r3,0000001F
	l.sfnei	r3,00000000
	l.bf	00009548
	l.sfleui	r4,00000038

l00009478:
	l.bf	00009484
	l.addi	r5,r0,+00000001

l00009480:
	l.ori	r5,r3,00000000

l00009484:
	l.andi	r5,r5,000000FF
	l.sfnei	r5,00000000
	l.bf	00009548
	l.addi	r4,r0,+00000001

l00009494:
	l.ori	r2,r5,00000000
	l.ori	r3,r5,00000000
	l.j	0000952C
	l.ori	r12,r4,00000000

l000094A4:
	l.movhi	r8,00000001
	l.ori	r8,r8,00003378
	l.add	r6,r11,r8
	l.lbz	r6,0(r6)
	l.bf	000094E4
	l.andi	r8,r6,00000080

l000094BC:
	l.sfeqi	r8,00000000
	l.bf	000094D4
	l.sfgtui	r3,00000024

l000094C8:
	l.andi	r6,r6,0000007F
	l.j	00009518
	l.add	r3,r3,r6

l000094D4:
	l.bf	00009550
	l.ori	r4,r8,00000000

l000094DC:
	l.j	00009528
	l.andi	r3,r6,0000007F

l000094E4:
	l.sfeqi	r8,00000000
	l.bf	00009520
	l.sfgtui	r3,0000004A

l000094F0:
	l.bf	00009558
	l.sfleui	r3,00000024

l000094F8:
	l.bf	00009504
	l.sll	r3,r12,r5

l00009500:
	l.or	r2,r2,r3

l00009504:
	l.addi	r5,r5,+00000001
	l.sfeqi	r5,00000020
	l.bf	00009568
	l.nop

l00009514:
	l.andi	r3,r6,0000007F

l00009518:
	l.j	00009528
	l.addi	r4,r0,+00000001

l00009520:
	l.andi	r6,r6,0000007F
	l.add	r3,r3,r6

l00009528:
	l.addi	r11,r11,+00000001

l0000952C:
	l.sfltu	r11,r7
	l.bf	000094A4
	l.sfeqi	r4,00000000

l00009538:
	l.j	0000956C
	l.movhi	r3,00000001

l00009540:
	l.j	0000955C
	l.addi	r3,r3,+00000001

l00009548:
	l.j	0000955C
	l.addi	r3,r0,+00000000

l00009550:
	l.j	0000955C
	l.ori	r3,r8,00000000

l00009558:
	l.ori	r3,r4,00000000

l0000955C:
	l.sfltu	r3,r7
	l.bf	00009540
	l.addi	r2,r0,-00000001

l00009568:
	l.movhi	r3,00000001

l0000956C:
	l.ori	r3,r3,00003478
	l.sw	0(r3),r2

l00009574:
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000000
	l.ori	r2,r2,00003374
	l.movhi	r5,000000FF
	l.sw	0(r2),r3
	l.movhi	r2,00000001
	l.movhi	r7,0000FF00
	l.ori	r2,r2,00003478
	l.movhi	r3,00000001
	l.lwz	r4,0(r2)
	l.ori	r3,r3,0000316C
	l.and	r6,r4,r5
	l.movhi	r5,00000001
	l.and	r12,r4,r7
	l.ori	r5,r5,00003370
	l.srli	r12,r12,00000008
	l.lwz	r8,0(r5)
	l.movhi	r5,00000001
	l.lwz	r11,0(r3)
	l.ori	r5,r5,00003168
	l.addi	r3,r3,+00000008
	l.andi	r2,r4,0000FFFF
	l.lwz	r7,0(r5)
	l.xor	r12,r12,r6
	l.ori	r5,r3,00000000
	l.j	00009640
	l.addi	r4,r0,+00000000

l000095E0:
	l.lwz	r8,0(r5)
	l.andi	r13,r8,0000FFFF
	l.sfne	r2,r13
	l.bf	00009634
	l.nop

l000095F4:
	l.movhi	r7,000000FF
	l.sfne	r12,r7
	l.bf	00009634
	l.addi	r7,r0,+00000000

l00009604:
	l.movhi	r4,00000001
	l.srli	r6,r6,00000010
	l.ori	r4,r4,00003370
	l.movhi	r5,00000001
	l.sw	0(r4),r2
	l.movhi	r4,00000001
	l.ori	r5,r5,00003170
	l.ori	r4,r4,00003168
	l.ori	r8,r13,00000000
	l.sw	0(r4),r6
	l.j	000096CC
	l.addi	r4,r0,+00000000

l00009634:
	l.addi	r4,r4,+00000001
	l.addi	r5,r5,+00000008
	l.ori	r8,r2,00000000

l00009640:
	l.sfltu	r4,r11
	l.bf	000095E0
	l.nop

l0000964C:
	l.movhi	r2,00000001
	l.ori	r2,r2,00003370
	l.sw	0(r2),r8
	l.movhi	r2,00000001
	l.ori	r2,r2,00003168
	l.j	000096D8
	l.sw	0(r2),r7

l00009668:
	l.sfeqi	r11,00000000
	l.bf	000096E4
	l.movhi	r2,00000001

l00009674:
	l.ori	r2,r2,00003374

l00009678:
	l.sw	0(r2),r3
	l.j	000096E4
	l.ori	r11,r3,00000000

l00009684:
	l.bf	000096A4
	l.lwz	r7,0(r5)

l0000968C:
	l.sfne	r6,r7
	l.bf	000096C0
	l.nop

l00009698:
	l.lwz	r7,0(r3)
	l.j	000096B8
	l.sfeq	r8,r7

l000096A4:
	l.sfne	r6,r7
	l.bf	000096C0
	l.nop

l000096B0:
	l.lwz	r7,0(r3)
	l.sfeq	r2,r7

l000096B8:
	l.bf	000096E0
	l.nop

l000096C0:
	l.addi	r4,r4,+00000001
	l.addi	r3,r3,+00000008
	l.addi	r5,r5,+00000008

l000096CC:
	l.sfltu	r4,r11
	l.bf	00009684
	l.sfeqi	r14,00000000

l000096D8:
	l.j	000096E4
	l.addi	r11,r0,+00000000

l000096E0:
	l.addi	r11,r0,+00000001

l000096E4:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn000096F8: 000096F8
;;   Called from:
;;     0000E9B4 (in fn0000E98C)
fn000096F8 proc
	l.sw	-4(r1),r9
	l.addi	r3,r0,+00000020
	l.addi	r1,r1,-00000004
	l.jal	0000B320
	l.addi	r4,r0,+00000001
	l.movhi	r4,000001F0
	l.addi	r3,r0,+00000001
	l.ori	r5,r4,00002E18
	l.ori	r4,r4,00002E38
	l.sw	0(r5),r3
	l.sw	0(r4),r3
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000
00009734             44 00 48 00 9D 60 00 00 44 00 48 00     D.H..`..D.H.
00009740 9D 60 00 00                                     .`..           

;; fn00009744: 00009744
;;   Called from:
;;     00009084 (in fn00009290)
;;     00009AC0 (in fn0000998C)
;;     00009AD0 (in fn0000998C)
;;     00009AE0 (in fn0000998C)
;;     0000A690 (in fn0000A664)
;;     0000AB64 (in fn0000AADC)
;;     0000AB74 (in fn0000AADC)
;;     0000CB80 (in fn0000CB40)
;;     0000CBB0 (in fn0000CB40)
;;     0000D69C (in fn0000D640)
;;     0000D6AC (in fn0000D640)
;;     0000D7C0 (in fn0000D76C)
;;     0000D7D0 (in fn0000D76C)
fn00009744 proc
	l.slli	r6,r3,00000003
	l.sw	-4(r1),r2
	l.movhi	r2,0000007C
	l.add	r3,r6,r3
	l.ori	r2,r2,00000AF7
	l.srli	r6,r4,00000003
	l.add	r3,r3,r2
	l.andi	r4,r4,00000007
	l.add	r3,r3,r6
	l.addi	r1,r1,-00000008
	l.slli	r3,r3,00000002
	l.slli	r4,r4,00000002
	l.addi	r11,r0,+00000000
	l.lwz	r6,0(r3)
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000007
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r4
	l.sll	r4,r5,r4
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r4,r4,r6
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn000097BC: 000097BC
;;   Called from:
;;     00009094 (in fn00009290)
;;     0000A6A8 (in fn0000A664)
;;     0000CB90 (in fn0000CB40)
;;     0000CBC4 (in fn0000CB40)
fn000097BC proc
	l.slli	r6,r3,00000003
	l.sw	-4(r1),r2
	l.movhi	r2,0000007C
	l.add	r3,r6,r3
	l.ori	r2,r2,00000AFE
	l.srli	r6,r4,00000004
	l.add	r3,r3,r2
	l.addi	r1,r1,-00000008
	l.add	r3,r3,r6
	l.andi	r4,r4,0000000F
	l.slli	r3,r3,00000002
	l.add	r4,r4,r4
	l.addi	r11,r0,+00000000
	l.lwz	r6,0(r3)
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000003
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r4
	l.sll	r4,r5,r4
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r4,r4,r6
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00009834: 00009834
;;   Called from:
;;     000090A4 (in fn00009290)
;;     00009AF0 (in fn0000998C)
;;     00009B00 (in fn0000998C)
;;     00009B10 (in fn0000998C)
;;     0000AB84 (in fn0000AADC)
;;     0000AB94 (in fn0000AADC)
;;     0000CBA0 (in fn0000CB40)
;;     0000CBD4 (in fn0000CB40)
fn00009834 proc
	l.slli	r6,r3,00000003
	l.sw	-4(r1),r2
	l.movhi	r2,0000007C
	l.add	r3,r6,r3
	l.ori	r2,r2,00000AFC
	l.srli	r6,r4,00000004
	l.add	r3,r3,r2
	l.addi	r1,r1,-00000008
	l.add	r3,r3,r6
	l.andi	r4,r4,0000000F
	l.slli	r3,r3,00000002
	l.add	r4,r4,r4
	l.addi	r11,r0,+00000000
	l.lwz	r6,0(r3)
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000003
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r4
	l.sll	r4,r5,r4
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r4,r4,r6
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn000098AC: 000098AC
;;   Called from:
;;     00009A8C (in fn0000998C)
;;     00009AA0 (in fn0000998C)
;;     00009AB0 (in fn0000998C)
;;     0000A048 (in fn00009ED0)
;;     0000A78C (in fn0000A708)
;;     0000A79C (in fn0000A708)
;;     0000A7AC (in fn0000A708)
;;     0000A7C4 (in fn0000A708)
;;     0000A7D4 (in fn0000A708)
;;     0000A7EC (in fn0000A708)
;;     0000AA28 (in fn0000A92C)
;;     0000AA38 (in fn0000A92C)
;;     0000AA48 (in fn0000A92C)
;;     0000ABA4 (in fn0000AADC)
;;     0000ABB4 (in fn0000AADC)
fn000098AC proc
	l.slli	r6,r3,00000002
	l.slli	r3,r3,00000005
	l.sw	-4(r1),r2
	l.movhi	r2,000001F0
	l.add	r3,r6,r3
	l.ori	r2,r2,00002BEC
	l.addi	r1,r1,-00000008
	l.add	r3,r3,r2
	l.andi	r5,r5,00000001
	l.lwz	r6,0(r3)
	l.addi	r11,r0,+00000000
	l.sw	0(r1),r6
	l.addi	r6,r0,+00000001
	l.lwz	r7,0(r1)
	l.sll	r6,r6,r4
	l.sll	r4,r5,r4
	l.xori	r6,r6,0000FFFF
	l.and	r6,r6,r7
	l.sw	0(r1),r6
	l.lwz	r6,0(r1)
	l.or	r4,r4,r6
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn00009918: 00009918
;;   Called from:
;;     0000A150 (in fn0000A070)
;;     0000A170 (in fn0000A070)
;;     0000A6BC (in fn0000A6B0)
;;     0000A6E0 (in fn0000A6B0)
fn00009918 proc
	l.slli	r5,r3,00000002
	l.slli	r3,r3,00000005
	l.sw	-4(r1),r2
	l.movhi	r2,000001F0
	l.add	r3,r5,r3
	l.ori	r2,r2,00002BEC
	l.addi	r1,r1,-00000008
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.addi	r1,r1,+00000008
	l.srl	r4,r3,r4
	l.lwz	r2,-4(r1)
	l.jr	r9
	l.andi	r11,r4,00000001

;; fn00009958: 00009958
;;   Called from:
;;     000115C4 (in fn00010570)
fn00009958 proc
	l.sw	-4(r1),r2
	l.movhi	r2,000001F0
	l.slli	r3,r3,00000005
	l.ori	r2,r2,00002DF4
	l.addi	r1,r1,-00000008
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.lwz	r11,0(r1)
	l.addi	r1,r1,+00000008
	l.and	r11,r4,r11
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000998C: 0000998C
;;   Called from:
;;     0000EA24 (in fn0000E98C)
fn0000998C proc
	l.sw	-16(r1),r14
	l.sw	-12(r1),r16
	l.sw	-4(r1),r9
	l.sw	-20(r1),r2
	l.sw	-8(r1),r18
	l.addi	r1,r1,-00000018
	l.jal	0000D628
	l.movhi	r16,00000001
	l.movhi	r3,00000001
	l.movhi	r14,00000001
	l.sfnei	r11,00000000
	l.ori	r3,r3,000034AC
	l.ori	r16,r16,00003938
	l.bf	00009A78
	l.ori	r14,r14,00003934

l000099C8:
	l.addi	r2,r0,+00000001
	l.addi	r18,r0,+0000000F
	l.sw	0(r3),r2
	l.sw	0(r16),r11
	l.sw	0(r14),r18
	l.addi	r16,r1,+00000003
	l.addi	r14,r1,+00000002
	l.addi	r3,r0,+00000032
	l.ori	r4,r14,00000000
	l.sb	3(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D214
	l.ori	r3,r16,00000000
	l.lbz	r3,2(r1)
	l.ori	r3,r3,00000010
	l.ori	r5,r2,00000000
	l.sb	2(r1),r3
	l.ori	r4,r14,00000000
	l.jal	0000D298
	l.ori	r3,r16,00000000
	l.movhi	r4,00000001
	l.ori	r3,r18,00000000
	l.jal	0000EDF4
	l.ori	r4,r4,00002C08
	l.addi	r4,r0,+0000001A
	l.ori	r3,r16,00000000
	l.sb	3(r1),r4
	l.ori	r5,r2,00000000
	l.jal	0000D214
	l.ori	r4,r14,00000000
	l.lbz	r3,2(r1)
	l.ori	r3,r3,00000007
	l.ori	r4,r14,00000000
	l.sb	2(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D298
	l.ori	r3,r16,00000000
	l.movhi	r3,00000001
	l.ori	r3,r3,00003010
	l.sb	7(r3),r2
	l.sb	15(r3),r2
	l.sb	23(r3),r2
	l.j	00009B48
	l.sb	31(r3),r2

l00009A78:
	l.addi	r2,r0,+00000000
	l.addi	r4,r0,+00000005
	l.sw	0(r3),r2
	l.ori	r5,r2,00000000
	l.addi	r3,r0,+00000001
	l.jal	000098AC
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000008
	l.ori	r5,r3,00000000
	l.jal	000098AC
	l.ori	r2,r2,00003498
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000009
	l.jal	000098AC
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000005
	l.jal	00009744
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000008
	l.jal	00009744
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000009
	l.jal	00009744
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000005
	l.jal	00009834
	l.addi	r5,r0,+00000003
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000008
	l.jal	00009834
	l.addi	r5,r0,+00000003
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000009
	l.jal	00009834
	l.addi	r5,r0,+00000003
	l.lwz	r3,0(r2)
	l.movhi	r4,0000001A
	l.or	r3,r3,r4
	l.movhi	r4,00000001
	l.sw	0(r2),r3
	l.addi	r2,r0,+00000011
	l.addi	r3,r0,+0000000F
	l.sw	0(r16),r2
	l.addi	r2,r0,+00000016
	l.ori	r4,r4,00002C16
	l.jal	0000EDF4
	l.sw	0(r14),r2

l00009B48:
	l.addi	r1,r1,+00000018
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)
00009B68                         44 00 48 00 9D 60 00 00         D.H..`..

;; fn00009B70: 00009B70
;;   Called from:
;;     0000E130 (in fn0000E110)
;;     000101DC (in fn000101A8)
;;     000106B0 (in fn00010570)
fn00009B70 proc
	l.movhi	r3,00000001
	l.ori	r3,r3,000034AC
	l.jr	r9
	l.lwz	r11,0(r3)

;; fn00009B80: 00009B80
;;   Called from:
;;     0000A7FC (in fn0000A708)
;;     0000AAD4 (in fn0000AA80)
fn00009B80 proc
	l.sw	-8(r1),r2
	l.movhi	r2,000001F0
	l.addi	r4,r0,+00000000
	l.ori	r3,r2,00001000
	l.sw	-4(r1),r9
	l.sw	0(r3),r4
	l.ori	r3,r2,00001014
	l.addi	r4,r0,+00000101
	l.ori	r2,r2,00001018
	l.sw	0(r3),r4
	l.addi	r3,r0,+00000000
	l.addi	r1,r1,-00000008
	l.sw	0(r2),r3
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.lwz	r3,0(r2)
	l.ori	r3,r3,00000001
	l.sw	0(r2),r3

l00009BC8:
	l.j	00009BC8
	l.nop
00009BD0 E0 83 18 00 D7 E1 17 F8 E0 64 18 00 18 40 00 01 .........d...@..
00009BE0 B8 63 00 03 A8 42 2E 90 D7 E1 4F FC E0 63 10 00 .c...B....O..c..
00009BF0 9C 21 FF F4 8C 63 00 03 9C 40 00 00 D8 01 18 03 .!...c...@......
00009C00 9C 81 00 02 9C 61 00 03 9C A0 00 01 04 00 0D 82 .....a..........
00009C10 D8 01 10 02 91 61 00 02 9C 21 00 0C B9 6B 00 5F .....a...!...k._
00009C20 85 21 FF FC 44 00 48 00 84 41 FF F8             .!..D.H..A..   

;; fn00009C2C: 00009C2C
;;   Called from:
;;     0000E22C (in fn0000E1AC)
;;     0000E32C (in fn0000E1AC)
;;     0000F6F4 (in fn0000F444)
;;     000102C4 (in fn000101A8)
;;     00010470 (in fn000103C0)
fn00009C2C proc
	l.sw	-16(r1),r2
	l.sw	-4(r1),r9
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.ori	r2,r3,00000000
	l.addi	r1,r1,-00000014
	l.ori	r5,r4,00000000
	l.sfgtui	r3,00000016
	l.bf	00009DC4
	l.addi	r11,r0,-00000016

l00009C54:
	l.sfgtui	r3,00000010
	l.bf	00009DC4
	l.addi	r11,r0,+00000000

l00009C60:
	l.add	r4,r3,r3
	l.add	r4,r4,r3
	l.movhi	r3,00000001
	l.slli	r4,r4,00000003
	l.ori	r3,r3,00002E90
	l.add	r4,r4,r3
	l.addi	r7,r4,+00000004
	l.lhz	r3,0(r7)
	l.sfltu	r5,r3
	l.bf	00009DC4
	l.addi	r11,r0,-00000016

l00009C8C:
	l.addi	r6,r4,+0000000C
	l.lhz	r8,2(r6)
	l.sfgtu	r5,r8
	l.bf	00009DC4
	l.nop

l00009CA0:
	l.lhz	r7,2(r7)
	l.sfgtu	r5,r7
	l.bf	00009CCC
	l.nop

l00009CB0:
	l.lhz	r4,8(r4)
	l.addi	r6,r4,-00000001
	l.sub	r3,r6,r3
	l.jal	0000FEDC
	l.add	r3,r3,r5
	l.j	00009D0C
	l.andi	r11,r11,0000007F

l00009CCC:
	l.lhz	r3,0(r6)
	l.sfgtu	r5,r3
	l.bf	00009CEC
	l.addi	r14,r4,+00000008

l00009CDC:
	l.lbz	r3,3(r14)
	l.andi	r3,r3,0000007F
	l.j	00009D10
	l.sb	2(r1),r3

l00009CEC:
	l.lhz	r4,16(r4)
	l.addi	r6,r4,-00000001
	l.sub	r3,r6,r3
	l.jal	0000FEDC
	l.add	r3,r3,r5
	l.lbz	r3,3(r14)
	l.add	r11,r11,r3
	l.andi	r11,r11,0000007F

l00009D0C:
	l.sb	2(r1),r11

l00009D10:
	l.add	r3,r2,r2
	l.movhi	r4,00000001
	l.add	r3,r3,r2
	l.ori	r4,r4,00002E90
	l.slli	r3,r3,00000003
	l.addi	r14,r1,+00000003
	l.addi	r5,r0,+00000001
	l.add	r3,r3,r4
	l.slli	r2,r2,00000003
	l.lbz	r4,3(r3)
	l.sb	3(r1),r4
	l.lwz	r4,20(r3)
	l.lbz	r3,2(r1)
	l.and	r3,r4,r3
	l.addi	r4,r1,+00000001
	l.sb	2(r1),r3
	l.jal	0000D214
	l.ori	r3,r14,00000000
	l.ori	r3,r14,00000000
	l.addi	r4,r1,+00000002
	l.addi	r5,r0,+00000001
	l.jal	0000D298
	l.ori	r16,r11,00000000
	l.movhi	r3,00000001
	l.or	r11,r16,r11
	l.ori	r3,r3,00003010
	l.add	r2,r2,r3
	l.lbz	r2,7(r2)
	l.sfeqi	r2,00000000
	l.bf	00009DC0
	l.andi	r14,r11,000000FF

l00009D8C:
	l.lbz	r2,2(r1)
	l.lbz	r3,1(r1)
	l.sfltu	r2,r3
	l.bf	00009DA8
	l.nop

l00009DA0:
	l.j	00009DAC
	l.sub	r2,r2,r3

l00009DA8:
	l.sub	r2,r3,r2

l00009DAC:
	l.sb	2(r1),r2
	l.lbz	r3,2(r1)
	l.slli	r3,r3,00000004
	l.jal	0000C8A0
	l.addi	r3,r3,+00000064

l00009DC0:
	l.ori	r11,r14,00000000

l00009DC4:
	l.addi	r1,r1,+00000014
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn00009DDC: 00009DDC
;;   Called from:
;;     0000E160 (in fn0000E110)
;;     0000E268 (in fn0000E1AC)
;;     0000E348 (in fn0000E1AC)
;;     0000F704 (in fn0000F444)
;;     000102B4 (in fn000101A8)
fn00009DDC proc
	l.sw	-8(r1),r2
	l.sw	-4(r1),r9
	l.addi	r2,r0,+00000000
	l.addi	r1,r1,-0000000C
	l.addi	r11,r0,-00000016
	l.sfgtui	r3,00000016
	l.bf	00009EC0
	l.sb	2(r1),r2

l00009DFC:
	l.sfeqi	r3,00000011
	l.bf	00009EC0
	l.addi	r11,r0,+00000CE4

l00009E08:
	l.sfeqi	r3,00000012
	l.bf	00009EC0
	l.addi	r11,r0,+000004B0

l00009E14:
	l.sfeqi	r3,00000013
	l.bf	00009EC0
	l.sfeqi	r3,00000014

l00009E20:
	l.bf	00009EC0
	l.addi	r11,r0,+0000044C

l00009E28:
	l.sfeqi	r3,00000015
	l.bf	00009EC0
	l.addi	r11,r0,+00000CE4

l00009E34:
	l.sfeqi	r3,00000016
	l.bf	00009EC0
	l.addi	r4,r1,+00000002

l00009E40:
	l.add	r2,r3,r3
	l.add	r2,r2,r3
	l.movhi	r3,00000001
	l.slli	r2,r2,00000003
	l.ori	r3,r3,00002E90
	l.addi	r5,r0,+00000001
	l.add	r2,r2,r3
	l.lbz	r3,3(r2)
	l.sb	3(r1),r3
	l.jal	0000D214
	l.addi	r3,r1,+00000003
	l.lbz	r3,2(r1)
	l.lwz	r11,20(r2)
	l.addi	r4,r2,+00000008
	l.and	r11,r11,r3
	l.lhz	r3,2(r4)
	l.sfles	r3,r11
	l.bf	00009E9C
	l.sfne	r11,r3

l00009E8C:
	l.lhz	r3,0(r4)
	l.lhz	r2,4(r2)
	l.j	00009EBC
	l.mul	r11,r11,r3

l00009E9C:
	l.bf	00009EAC
	l.addi	r4,r2,+0000000C

l00009EA4:
	l.j	00009EC0
	l.lhz	r11,0(r4)

l00009EAC:
	l.lhz	r2,16(r2)
	l.sub	r11,r11,r3
	l.mul	r11,r11,r2
	l.lhz	r2,0(r4)

l00009EBC:
	l.add	r11,r11,r2

l00009EC0:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00009ED0: 00009ED0
;;   Called from:
;;     0000F718 (in fn0000F444)
;;     00010364 (in fn000101A8)
;;     00010514 (in fn000103C0)
fn00009ED0 proc
	l.sw	-20(r1),r2
	l.sw	-4(r1),r9
	l.sw	-16(r1),r14
	l.sw	-12(r1),r16
	l.sw	-8(r1),r18
	l.ori	r6,r3,00000000
	l.addi	r1,r1,-00000018
	l.sfgtui	r3,0000000F
	l.bf	00009F7C
	l.ori	r2,r4,00000000

l00009EF8:
	l.slli	r6,r3,00000003
	l.movhi	r3,00000001
	l.addi	r16,r1,+00000003
	l.ori	r3,r3,00003010
	l.addi	r14,r1,+00000002
	l.add	r6,r6,r3
	l.addi	r5,r0,+00000001
	l.lbz	r3,3(r6)
	l.addi	r6,r6,+00000004
	l.sb	3(r1),r3
	l.sb	2(r6),r4
	l.ori	r3,r16,00000000
	l.ori	r4,r14,00000000
	l.jal	0000D214
	l.lhz	r18,0(r6)
	l.addi	r5,r0,+00000001
	l.lbz	r4,2(r1)
	l.sll	r3,r5,r18
	l.sll	r18,r2,r18
	l.xori	r3,r3,000000FF
	l.and	r3,r3,r4
	l.ori	r4,r14,00000000
	l.or	r18,r3,r18
	l.ori	r3,r16,00000000
	l.jal	0000D298
	l.sb	2(r1),r18
	l.sfnei	r2,00000001
	l.bf	0000A050
	l.nop

l00009F6C:
	l.jal	0000C768
	l.ori	r3,r2,00000000
	l.j	0000A054
	l.addi	r1,r1,+00000018

l00009F7C:
	l.xori	r3,r3,00000013
	l.sub	r4,r0,r3
	l.or	r3,r4,r3
	l.sfgesi	r3,+00000000
	l.bf	00009FB8
	l.addi	r3,r0,+00000001

l00009F94:
	l.xori	r3,r6,00000011
	l.sub	r4,r0,r3
	l.or	r3,r4,r3
	l.sfgesi	r3,+00000000
	l.bf	00009FB4
	l.sfnei	r6,00000014

l00009FAC:
	l.bf	0000A000
	l.sfnei	r6,00000012

l00009FB4:
	l.addi	r3,r0,+00000001

l00009FB8:
	l.movhi	r4,00000001
	l.sll	r5,r3,r6
	l.ori	r4,r4,00003498
	l.and	r2,r2,r3
	l.lwz	r7,0(r4)
	l.xori	r5,r5,0000FFFF
	l.sll	r6,r2,r6
	l.and	r5,r5,r7
	l.sw	0(r4),r5
	l.lwz	r5,0(r4)
	l.or	r6,r6,r5
	l.sw	0(r4),r6
	l.lwz	r2,0(r4)
	l.sub	r4,r0,r2
	l.or	r2,r4,r2
	l.addi	r4,r0,+00000008
	l.j	0000A048
	l.srli	r5,r2,0000001F

l0000A000:
	l.bf	0000A014
	l.sfnei	r6,00000015

l0000A008:
	l.addi	r3,r0,+00000001
	l.j	0000A024
	l.addi	r4,r0,+00000009

l0000A014:
	l.bf	0000A02C
	l.sfnei	r6,00000016

l0000A01C:
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000007

l0000A024:
	l.j	0000A048
	l.ori	r5,r2,00000000

l0000A02C:
	l.bf	0000A050
	l.sub	r5,r0,r2

l0000A034:
	l.addi	r3,r0,+00000001
	l.or	r2,r5,r2
	l.addi	r4,r0,+00000005
	l.xori	r5,r2,0000FFFF
	l.srli	r5,r5,0000001F

l0000A048:
	l.jal	000098AC
	l.nop

l0000A050:
	l.addi	r1,r1,+00000018

l0000A054:
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)

;; fn0000A070: 0000A070
;;   Called from:
;;     0000F728 (in fn0000F444)
fn0000A070 proc
	l.sw	-4(r1),r9
	l.sw	-12(r1),r2
	l.sw	-8(r1),r14
	l.sfgtui	r3,0000000F
	l.bf	0000A0E4
	l.addi	r1,r1,-00000010

l0000A088:
	l.slli	r2,r3,00000003
	l.movhi	r3,00000001
	l.addi	r4,r1,+00000002
	l.ori	r3,r3,00003010
	l.addi	r5,r0,+00000001
	l.add	r2,r2,r3
	l.lbz	r3,3(r2)
	l.addi	r2,r2,+00000004
	l.sb	3(r1),r3
	l.addi	r3,r1,+00000003
	l.jal	0000D214
	l.lhz	r14,0(r2)
	l.lbz	r11,2(r1)
	l.sra	r11,r11,r14
	l.andi	r11,r11,00000001
	l.sfeqi	r11,00000000
	l.bf	0000A0DC
	l.addi	r3,r0,+00000001

l0000A0D0:
	l.addi	r11,r0,+00000001
	l.j	0000A188
	l.sb	2(r2),r3

l0000A0DC:
	l.j	0000A188
	l.sb	2(r2),r11

l0000A0E4:
	l.xori	r2,r3,00000013
	l.sub	r4,r0,r2
	l.or	r2,r4,r2
	l.sfgesi	r2,+00000000
	l.bf	0000A118
	l.xori	r2,r3,00000011

l0000A0FC:
	l.sub	r4,r0,r2
	l.or	r2,r4,r2
	l.sfgesi	r2,+00000000
	l.bf	0000A118
	l.sfnei	r3,00000014

l0000A110:
	l.bf	0000A130
	l.sfnei	r3,00000012

l0000A118:
	l.movhi	r2,00000001
	l.ori	r2,r2,00003498
	l.lwz	r11,0(r2)
	l.srl	r11,r11,r3
	l.j	0000A188
	l.andi	r11,r11,00000001

l0000A130:
	l.bf	0000A144
	l.sfnei	r3,00000015

l0000A138:
	l.addi	r3,r0,+00000001
	l.j	0000A150
	l.addi	r4,r0,+00000009

l0000A144:
	l.bf	0000A160
	l.addi	r4,r0,+00000007

l0000A14C:
	l.addi	r3,r0,+00000001

l0000A150:
	l.jal	00009918
	l.nop
	l.j	0000A18C
	l.addi	r1,r1,+00000010

l0000A160:
	l.sfnei	r3,00000016
	l.bf	0000A188
	l.addi	r11,r0,-00000016

l0000A16C:
	l.addi	r3,r0,+00000001
	l.jal	00009918
	l.addi	r4,r0,+00000005
	l.sub	r2,r0,r11
	l.or	r11,r2,r11
	l.xori	r11,r11,0000FFFF
	l.srli	r11,r11,0000001F

l0000A188:
	l.addi	r1,r1,+00000010

l0000A18C:
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)
0000A19C                                     D7 E1 4F FC             ..O.
0000A1A0 9C 21 FF FC A8 64 00 00 9C 21 00 04 A8 85 00 00 .!...d...!......
0000A1B0 85 21 FF FC 00 00 0C 39 A8 A6 00 00 D7 E1 4F FC .!.....9......O.
0000A1C0 9C 21 FF FC A8 64 00 00 9C 21 00 04 A8 85 00 00 .!...d...!......
0000A1D0 85 21 FF FC 00 00 0C 10 A8 A6 00 00 D7 E1 4F FC .!............O.
0000A1E0 A8 A3 00 00 9C 21 FF FC 84 85 00 08 84 63 00 04 .....!.......c..
0000A1F0 84 A5 00 0C 9C 21 00 04 85 21 FF FC 00 00 0C 27 .....!...!.....'
0000A200 15 00 00 00 D7 E1 4F FC A8 A3 00 00 9C 21 FF FC ......O......!..
0000A210 84 85 00 08 84 63 00 04 84 A5 00 0C 9C 21 00 04 .....c.......!..
0000A220 85 21 FF FC 00 00 0B FC 15 00 00 00             .!..........   

;; fn0000A22C: 0000A22C
;;   Called from:
;;     00011554 (in fn00010570)
;;     0001236C (in fn00012320)
fn0000A22C proc
	l.sw	-8(r1),r2
	l.ori	r2,r3,00000000
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,000034AC
	l.lwz	r3,0(r3)
	l.sfnei	r3,00000001
	l.bf	0000A2EC
	l.addi	r1,r1,-00000018

l0000A250:
	l.addi	r3,r0,+00000048
	l.addi	r5,r0,+00000049
	l.sb	8(r1),r3
	l.sb	9(r1),r5
	l.addi	r3,r1,+00000008
	l.ori	r4,r1,00000000
	l.jal	0000D214
	l.addi	r5,r0,+00000006
	l.lbz	r3,1(r1)
	l.andi	r4,r3,00000001
	l.sfeqi	r4,00000000
	l.bf	0000A298
	l.andi	r4,r3,00000002

l0000A284:
	l.lwz	r4,0(r2)
	l.movhi	r5,00000004
	l.or	r4,r4,r5
	l.sw	0(r2),r4
	l.andi	r4,r3,00000002

l0000A298:
	l.sfeqi	r4,00000000
	l.bf	0000A2B8
	l.andi	r4,r3,00000020

l0000A2A4:
	l.lwz	r4,0(r2)
	l.movhi	r5,00000002
	l.or	r4,r4,r5
	l.sw	0(r2),r4
	l.andi	r4,r3,00000020

l0000A2B8:
	l.sfeqi	r4,00000000
	l.bf	0000A2D0
	l.movhi	r5,00000001

l0000A2C4:
	l.lwz	r4,0(r2)
	l.or	r4,r4,r5
	l.sw	0(r2),r4

l0000A2D0:
	l.andi	r3,r3,00000040
	l.sfeqi	r3,00000000
	l.bf	0000A2EC
	l.nop

l0000A2E0:
	l.lwz	r3,0(r2)
	l.ori	r3,r3,00008000
	l.sw	0(r2),r3

l0000A2EC:
	l.addi	r1,r1,+00000018
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000A300: 0000A300
;;   Called from:
;;     00012390 (in fn00012320)
;;     000123A0 (in fn00012320)
fn0000A300 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,000034AC
	l.sw	-8(r1),r2
	l.lwz	r3,0(r3)
	l.addi	r1,r1,-0000000C
	l.sfnei	r3,00000001
	l.bf	0000A354
	l.addi	r11,r0,+00000000

l0000A324:
	l.addi	r2,r0,+00000048
	l.addi	r3,r1,+00000002
	l.sb	2(r1),r2
	l.addi	r2,r0,-00000001
	l.ori	r4,r1,00000000
	l.sb	0(r1),r2
	l.addi	r2,r0,+00000049
	l.addi	r5,r0,+00000002
	l.sb	3(r1),r2
	l.addi	r2,r0,-00000001
	l.jal	0000D298
	l.sb	1(r1),r2

l0000A354:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000A364: 0000A364
;;   Called from:
;;     0000F6E0 (in fn0000F444)
fn0000A364 proc
	l.sw	-12(r1),r2
	l.ori	r2,r3,00000000
	l.movhi	r3,00000001
	l.sw	-8(r1),r14
	l.ori	r3,r3,000034AC
	l.sw	-4(r1),r9
	l.lwz	r5,0(r3)
	l.addi	r1,r1,-00000050
	l.sfnei	r5,00000001
	l.bf	0000A5B8
	l.addi	r14,r0,+00000000

l0000A390:
	l.addi	r3,r0,-00000001
	l.addi	r14,r1,+00000018
	l.sb	24(r1),r3
	l.addi	r4,r1,+00000043
	l.ori	r3,r14,00000000
	l.jal	0000D298
	l.sb	67(r1),r5
	l.ori	r3,r14,00000000
	l.addi	r4,r0,+00000020

l0000A3B4:
	l.sb	0(r3),r4
	l.addi	r4,r4,+00000001
	l.andi	r4,r4,000000FF
	l.sfnei	r4,00000037
	l.bf	0000A3B4
	l.addi	r3,r3,+00000001

l0000A3CC:
	l.addi	r3,r1,+00000018
	l.ori	r4,r1,00000000
	l.jal	0000D214
	l.addi	r5,r0,+00000017
	l.lbz	r3,0(r1)
	l.sb	63(r1),r3
	l.lbz	r3,1(r1)
	l.sb	62(r1),r3
	l.lbz	r3,2(r1)
	l.sb	61(r1),r3
	l.lbz	r3,3(r1)
	l.sb	60(r1),r3
	l.lbz	r3,4(r1)
	l.sb	59(r1),r3
	l.lbz	r3,6(r1)
	l.lbz	r4,5(r1)
	l.andi	r3,r3,0000003F
	l.andi	r4,r4,0000003F
	l.slli	r3,r3,00000006
	l.lbz	r5,9(r1)
	l.or	r4,r4,r3
	l.lbz	r3,7(r1)
	l.andi	r3,r3,0000003F
	l.andi	r5,r5,0000003F
	l.slli	r3,r3,0000000C
	l.slli	r5,r5,00000018
	l.lbz	r7,11(r1)
	l.or	r4,r4,r3
	l.lbz	r3,8(r1)
	l.andi	r3,r3,0000003F
	l.slli	r7,r7,00000004
	l.slli	r3,r3,00000012
	l.ori	r14,r11,00000000
	l.or	r4,r4,r3
	l.lbz	r3,10(r1)
	l.andi	r3,r3,0000003F
	l.or	r4,r4,r5
	l.slli	r5,r3,0000001E
	l.srli	r6,r3,00000002
	l.or	r4,r4,r5
	l.lbz	r5,12(r1)
	l.andi	r5,r5,00000003
	l.or	r3,r7,r6
	l.slli	r5,r5,0000000C
	l.lbz	r6,20(r1)
	l.or	r3,r3,r5
	l.lbz	r5,16(r1)
	l.andi	r5,r5,0000003F
	l.andi	r6,r6,0000003F
	l.slli	r5,r5,0000000E
	l.slli	r6,r6,00000006
	l.sb	58(r1),r4
	l.or	r3,r3,r5
	l.lbz	r5,17(r1)
	l.andi	r5,r5,0000003F
	l.slli	r5,r5,00000014
	l.or	r3,r3,r5
	l.lbz	r5,18(r1)
	l.slli	r5,r5,0000001A
	l.or	r3,r3,r5
	l.lbz	r5,21(r1)
	l.andi	r5,r5,0000003F
	l.sb	54(r1),r3
	l.slli	r5,r5,0000000C
	l.or	r5,r6,r5
	l.lbz	r6,19(r1)
	l.andi	r6,r6,0000003F
	l.or	r5,r5,r6
	l.lbz	r6,22(r1)
	l.andi	r6,r6,0000003F
	l.slli	r6,r6,00000012
	l.or	r5,r5,r6
	l.srli	r6,r4,00000008
	l.sb	57(r1),r6
	l.srli	r6,r4,00000010
	l.srli	r4,r4,00000018
	l.sb	56(r1),r6
	l.sb	55(r1),r4
	l.srli	r4,r3,00000008
	l.sb	53(r1),r4
	l.srli	r4,r3,00000010
	l.srli	r3,r3,00000018
	l.sb	52(r1),r4
	l.sb	51(r1),r3
	l.sb	50(r1),r5
	l.srli	r3,r5,00000008
	l.srli	r5,r5,00000010
	l.addi	r4,r1,+00000030
	l.sb	49(r1),r3
	l.sb	48(r1),r5
	l.addi	r3,r0,+00000000
	l.lwz	r8,40(r2)
	l.lwz	r7,44(r2)
	l.lwz	r6,48(r2)
	l.lwz	r5,52(r2)

l0000A548:
	l.lbz	r11,0(r4)
	l.sll	r11,r11,r3
	l.or	r8,r8,r11
	l.lbz	r11,4(r4)
	l.sll	r11,r11,r3
	l.or	r7,r7,r11
	l.lbz	r11,8(r4)
	l.sll	r11,r11,r3
	l.or	r6,r6,r11
	l.lbz	r11,12(r4)
	l.sll	r11,r11,r3
	l.addi	r3,r3,+00000008
	l.addi	r4,r4,+00000001
	l.sfnei	r3,00000020
	l.bf	0000A548
	l.or	r5,r5,r11

l0000A588:
	l.addi	r3,r0,-00000001
	l.sw	52(r2),r5
	l.sw	40(r2),r8
	l.sw	44(r2),r7
	l.sw	48(r2),r6
	l.sb	24(r1),r3
	l.addi	r2,r0,+00000000
	l.addi	r3,r1,+00000018
	l.addi	r4,r1,+00000043
	l.addi	r5,r0,+00000001
	l.jal	0000D298
	l.sb	67(r1),r2

l0000A5B8:
	l.addi	r1,r1,+00000050
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn0000A5D0: 0000A5D0
;;   Called from:
;;     0000F74C (in fn0000F444)
fn0000A5D0 proc
	l.lwz	r4,48(r3)
	l.sfnei	r4,00000001
	l.lwz	r4,40(r3)
	l.bf	0000A5FC
	l.lwz	r3,44(r3)

l0000A5E4:
	l.movhi	r5,00000001
	l.ori	r5,r5,0000349C
	l.sw	0(r5),r4
	l.movhi	r4,00000001
	l.j	0000A628
	l.ori	r4,r4,000034A0

l0000A5FC:
	l.add	r5,r4,r4
	l.slli	r4,r4,00000003
	l.add	r4,r5,r4
	l.movhi	r5,00000001
	l.ori	r5,r5,000034A4
	l.sw	0(r5),r4
	l.add	r4,r3,r3
	l.slli	r3,r3,00000003
	l.add	r3,r4,r3
	l.movhi	r4,00000001
	l.ori	r4,r4,000034A8

l0000A628:
	l.sw	0(r4),r3
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn0000A634: 0000A634
;;   Called from:
;;     0000F75C (in fn0000F444)
fn0000A634 proc
	l.addi	r4,r3,+00000028
	l.movhi	r3,00000000
	l.sw	-4(r1),r9
	l.ori	r3,r3,00004000
	l.addi	r1,r1,-00000004
	l.addi	r3,r3,+00000358
	l.jal	0000DF10
	l.addi	r5,r0,+00000050
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn0000A664: 0000A664
;;   Called from:
;;     00012328 (in fn00012320)
fn0000A664 proc
	l.movhi	r3,00000000
	l.sw	-4(r1),r9
	l.ori	r3,r3,00004000
	l.lwz	r3,948(r3)
	l.sfeqi	r3,00000000
	l.bf	0000A688
	l.addi	r1,r1,-00000004

l0000A680:
	l.jal	00008B60
	l.addi	r3,r0,+00000000

l0000A688:
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000006
	l.jal	00009744
	l.addi	r5,r0,+00000000
	l.addi	r3,r0,+00000001
	l.addi	r1,r1,+00000004
	l.addi	r4,r0,+00000006
	l.lwz	r9,-4(r1)
	l.j	000097BC
	l.ori	r5,r3,00000000

;; fn0000A6B0: 0000A6B0
;;   Called from:
;;     000123D8 (in fn00012320)
fn0000A6B0 proc
	l.sw	-4(r1),r9
	l.addi	r3,r0,+00000001
	l.addi	r1,r1,-00000004
	l.jal	00009918
	l.addi	r4,r0,+00000006
	l.addi	r3,r0,+00000000
	l.sfne	r11,r3
	l.bf	0000A6F8
	l.nop

l0000A6D4:
	l.jal	0000DB28
	l.addi	r3,r0,+0000000A
	l.addi	r3,r0,+00000001
	l.jal	00009918
	l.addi	r4,r0,+00000006
	l.sub	r3,r0,r11
	l.or	r11,r3,r11
	l.xori	r3,r11,0000FFFF
	l.srli	r3,r3,0000001F

l0000A6F8:
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.ori	r11,r3,00000000

;; fn0000A708: 0000A708
;;   Called from:
;;     0000AAC4 (in fn0000AA80)
;;     00012410 (in fn00012320)
fn0000A708 proc
	l.sw	-16(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.ori	r2,r2,000034AC
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.lwz	r2,0(r2)
	l.sfnei	r2,00000001
	l.bf	0000A774
	l.addi	r1,r1,-00000014

l0000A730:
	l.addi	r3,r0,+00000032
	l.addi	r16,r1,+00000003
	l.addi	r14,r1,+00000002
	l.sb	3(r1),r3
	l.ori	r4,r14,00000000
	l.ori	r3,r16,00000000
	l.jal	0000D214
	l.ori	r5,r2,00000000
	l.lbz	r3,2(r1)
	l.ori	r3,r3,00000040
	l.ori	r4,r14,00000000
	l.sb	2(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D298
	l.ori	r3,r16,00000000

l0000A76C:
	l.j	0000A76C
	l.nop

l0000A774:
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002C28
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000005
	l.jal	000098AC
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000009
	l.jal	000098AC
	l.addi	r5,r0,+00000000
	l.addi	r4,r0,+00000008
	l.addi	r5,r0,+00000000
	l.jal	000098AC
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.addi	r3,r0,+000003E8
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000008
	l.jal	000098AC
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000009
	l.jal	000098AC
	l.ori	r5,r3,00000000
	l.jal	0000C768
	l.addi	r3,r0,+0000000A
	l.addi	r4,r0,+00000005
	l.addi	r5,r0,+00000000
	l.jal	000098AC
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.addi	r3,r0,+00000064
	l.jal	00009B80
	l.nop
	l.addi	r1,r1,+00000014
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)
0000A81C                                     D7 E1 17 F8             ....
0000A820 D7 E1 4F FC 9C 21 FF F4 18 40 00 07 A8 61 00 00 ..O..!...@...a..
0000A830 07 FF FE 7F A8 42 80 00 84 61 00 00 E0 63 10 03 .....B...a...c..
0000A840 BC 03 00 00 10 00 00 07 18 60 01 F0 9C 80 00 0F .........`......
0000A850 04 00 01 AD A8 63 01 08 07 FF FF AC 15 00 00 00 .....c..........
0000A860 07 FF FE A8 15 00 00 00 07 FF F9 0F 9C 60 00 00 .............`..
0000A870 9C 21 00 0C 9D 60 00 01 85 21 FF FC 44 00 48 00 .!...`...!..D.H.
0000A880 84 41 FF F8 D7 E1 17 EC A8 43 00 00 18 60 00 01 .A.......C...`..
0000A890 D7 E1 77 F0 A8 63 34 AC D7 E1 4F FC D7 E1 87 F4 ..w..c4...O.....
0000A8A0 D7 E1 97 F8 85 C3 00 00 BC 2E 00 01 10 00 00 18 ................
0000A8B0 9C 21 FF E8 9C 60 00 36 BD 42 00 04 10 00 00 13 .!...`.6.B......
0000A8C0 D8 01 18 03 9E 41 00 03 9E 01 00 02 A8 72 00 00 .....A.......r..
0000A8D0 A8 90 00 00 04 00 0A 50 A8 AE 00 00 8C 61 00 02 .......P.....a..
0000A8E0 B8 82 00 06 A4 63 00 3F A8 AE 00 00 E0 64 18 04 .....c.?.....d..
0000A8F0 A8 90 00 00 D8 01 18 02 04 00 0A 68 A8 72 00 00 ...........h.r..
0000A900 00 00 00 04 9C 21 00 18 9C 40 FF EA 9C 21 00 18 .....!...@...!..
0000A910 A9 62 00 00 85 21 FF FC 84 41 FF EC 85 C1 FF F0 .b...!...A......
0000A920 86 01 FF F4 44 00 48 00 86 41 FF F8             ....D.H..A..   

;; fn0000A92C: 0000A92C
;;   Called from:
;;     0000AAA0 (in fn0000AA80)
;;     0000ABBC (in fn0000AADC)
;;     0000AC6C (in fn0000AC14)
fn0000A92C proc
	l.sw	-4(r1),r9
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.addi	r1,r1,-00000014
	l.jal	0000AC84
	l.movhi	r2,00000001
	l.addi	r4,r0,+00000002
	l.addi	r3,r0,+00000001
	l.jal	0000B610
	l.ori	r2,r2,000034AC
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.jal	0000BAC8
	l.nop
	l.jal	0000897C
	l.nop
	l.addi	r3,r0,+0000000B
	l.jal	0000B950
	l.addi	r4,r0,+00000001
	l.addi	r4,r0,+00000001
	l.jal	0000B950
	l.addi	r3,r0,+0000000C
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+00000021
	l.jal	0000BC38
	l.addi	r4,r0,+00000000
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.lwz	r2,0(r2)
	l.sfnei	r2,00000001
	l.bf	0000AA20
	l.addi	r14,r1,+00000002

l0000A9B4:
	l.addi	r16,r1,+00000003
	l.addi	r3,r0,+00000010
	l.ori	r4,r14,00000000
	l.sb	3(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D214
	l.ori	r3,r16,00000000
	l.addi	r3,r0,+00000020
	l.ori	r4,r14,00000000
	l.sb	2(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D298
	l.ori	r3,r16,00000000
	l.addi	r3,r0,+00000011
	l.ori	r4,r14,00000000
	l.sb	3(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D214
	l.ori	r3,r16,00000000
	l.addi	r3,r0,+00000010
	l.ori	r4,r14,00000000
	l.sb	2(r1),r3
	l.ori	r5,r2,00000000
	l.jal	0000D298
	l.ori	r3,r16,00000000
	l.j	0000AA6C
	l.addi	r1,r1,+00000014

l0000AA20:
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000005
	l.jal	000098AC
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000009
	l.jal	000098AC
	l.addi	r5,r0,+00000000
	l.addi	r4,r0,+00000008
	l.addi	r3,r0,+00000001
	l.jal	000098AC
	l.addi	r5,r0,+00000000
	l.jal	0000C768
	l.addi	r3,r0,+000005DC
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002C37
	l.addi	r1,r1,+00000014

l0000AA6C:
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn0000AA80: 0000AA80
;;   Called from:
;;     00012304 (in fn000122B8)
fn0000AA80 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,000034AC
	l.sw	-8(r1),r2
	l.lwz	r3,0(r3)
	l.sfeqi	r3,00000001
	l.bf	0000AACC
	l.addi	r1,r1,-00000008

l0000AAA0:
	l.jal	0000A92C
	l.addi	r2,r0,-00000002
	l.movhi	r3,000001F0
	l.ori	r3,r3,0000142C
	l.lwz	r4,0(r3)
	l.and	r4,r4,r2
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.j	0000A708
	l.lwz	r2,-8(r1)

l0000AACC:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.j	00009B80
	l.lwz	r2,-8(r1)

;; fn0000AADC: 0000AADC
;;   Called from:
;;     000122EC (in fn000122B8)
fn0000AADC proc
	l.sw	-16(r1),r2
	l.sw	-4(r1),r9
	l.addi	r2,r0,+00000032
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.addi	r1,r1,-00000014
	l.sb	3(r1),r2
	l.movhi	r2,00000001
	l.ori	r2,r2,000034AC
	l.lwz	r2,0(r2)
	l.sfnei	r2,00000001
	l.bf	0000AB60
	l.addi	r4,r0,+00000002

l0000AB10:
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002C41
	l.addi	r16,r1,+00000003
	l.jal	0000EDF4
	l.addi	r14,r1,+00000002
	l.ori	r3,r16,00000000
	l.ori	r4,r14,00000000
	l.jal	0000D214
	l.ori	r5,r2,00000000
	l.lbz	r3,2(r1)
	l.addi	r4,r0,-00000080
	l.ori	r5,r2,00000000
	l.or	r3,r3,r4
	l.ori	r4,r14,00000000
	l.sb	2(r1),r3
	l.jal	0000D298
	l.ori	r3,r16,00000000
	l.j	0000ABF4
	l.nop

l0000AB60:
	l.addi	r3,r0,+00000001
	l.jal	00009744
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000007
	l.jal	00009744
	l.ori	r5,r3,00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000002
	l.jal	00009834
	l.addi	r5,r0,+00000003
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000007
	l.jal	00009834
	l.addi	r5,r0,+00000003
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000002
	l.jal	000098AC
	l.addi	r5,r0,+00000000
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000007
	l.jal	000098AC
	l.addi	r5,r0,+00000000
	l.jal	0000A92C
	l.nop
	l.movhi	r3,000001F0
	l.addi	r4,r0,+000007A4
	l.ori	r2,r3,00000180
	l.addi	r5,r0,-00000002
	l.sw	0(r2),r4
	l.ori	r2,r3,0000142C
	l.ori	r3,r3,00001C00
	l.lwz	r4,0(r2)
	l.and	r4,r4,r5
	l.sw	0(r2),r4
	l.addi	r2,r0,+00000000
	l.sw	0(r3),r2

l0000ABF4:
	l.j	0000ABF4
	l.nop
0000ABFC                                     D7 E1 4F FC             ..O.
0000AC00 9C 21 FF FC 9C 21 00 04 85 21 FF FC 03 FF FF B4 .!...!...!......
0000AC10 15 00 00 00                                     ....           

;; fn0000AC14: 0000AC14
;;   Called from:
;;     00012348 (in fn00012320)
fn0000AC14 proc
	l.sw	-8(r1),r2
	l.movhi	r2,00000170
	l.sw	-4(r1),r9
	l.ori	r2,r2,00000030
	l.addi	r1,r1,-0000000C

l0000AC28:
	l.lwz	r3,0(r2)
	l.movhi	r4,00000001
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	0000AC74
	l.movhi	r4,00000001

l0000AC40:
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002C4B
	l.addi	r2,r0,+00000000
	l.jal	0000EDF4
	l.sw	0(r1),r2
	l.jal	00004844
	l.nop
	l.jal	0000490C
	l.nop
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.j	0000A92C
	l.lwz	r2,-8(r1)

l0000AC74:
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.j	0000AC28
	l.nop

;; fn0000AC84: 0000AC84
;;   Called from:
;;     0000A940 (in fn0000A92C)
;;     00010624 (in fn00010570)
fn0000AC84 proc
	l.sw	-32(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.ori	r2,r2,000034B0
	l.sw	-28(r1),r14
	l.sw	-24(r1),r16
	l.sw	-20(r1),r18
	l.sw	-16(r1),r20
	l.sw	-12(r1),r22
	l.sw	-8(r1),r24
	l.lwz	r3,0(r2)
	l.sfnei	r3,00000000
	l.bf	0000AD94
	l.addi	r1,r1,-00000020

l0000ACBC:
	l.addi	r3,r0,+00000001
	l.jal	0000AF1C
	l.movhi	r14,0000000F
	l.addi	r4,r0,+00000003
	l.addi	r3,r0,+00000001
	l.jal	0000B610
	l.ori	r24,r11,00000000
	l.jal	0000DB28
	l.addi	r3,r0,+00000640
	l.jal	000046C0
	l.nop
	l.ori	r3,r14,00004240
	l.jal	0000DB28
	l.ori	r20,r12,00000000
	l.jal	000046C0
	l.nop
	l.addi	r4,r0,+00000002
	l.addi	r3,r0,+00000001
	l.jal	0000B610
	l.ori	r22,r12,00000000
	l.jal	0000DB28
	l.addi	r3,r0,+00000640
	l.jal	000046C0
	l.nop
	l.ori	r3,r14,00004240
	l.jal	0000DB28
	l.ori	r16,r12,00000000
	l.jal	000046C0
	l.nop
	l.addi	r3,r0,+00000001
	l.ori	r4,r24,00000000
	l.jal	0000B610
	l.ori	r18,r12,00000000
	l.sub	r3,r22,r20
	l.jal	0000FEDC
	l.addi	r4,r0,+00005DC0
	l.addi	r4,r0,+00005DC0
	l.sub	r3,r18,r16
	l.jal	0000FEDC
	l.mul	r14,r11,r4
	l.ori	r3,r14,00000000
	l.jal	0000FEDC
	l.ori	r4,r11,00000000
	l.addi	r3,r0,+000003E8
	l.mul	r11,r11,r3
	l.movhi	r3,00000001
	l.ori	r3,r3,00003098
	l.sw	0(r3),r11
	l.movhi	r3,00000001
	l.srli	r11,r11,00000009
	l.ori	r3,r3,0000309C
	l.sw	0(r3),r11
	l.addi	r3,r0,+00000001
	l.sw	0(r2),r3

l0000AD94:
	l.addi	r1,r1,+00000020
	l.lwz	r9,-4(r1)
	l.lwz	r2,-32(r1)
	l.lwz	r14,-28(r1)
	l.lwz	r16,-24(r1)
	l.lwz	r18,-20(r1)
	l.lwz	r20,-16(r1)
	l.lwz	r22,-12(r1)
	l.jr	r9
	l.lwz	r24,-8(r1)

;; fn0000ADBC: 0000ADBC
;;   Called from:
;;     0000E9A4 (in fn0000E98C)
fn0000ADBC proc
	l.movhi	r4,000001F0
	l.movhi	r3,00000001
	l.ori	r4,r4,00001400
	l.ori	r3,r3,00003948
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.sw	0(r3),r4
	l.movhi	r4,00000001
	l.movhi	r3,000001C2
	l.ori	r4,r4,00003944
	l.addi	r1,r1,-00000008
	l.sw	0(r4),r3
	l.movhi	r4,00000001
	l.ori	r3,r3,00000028
	l.ori	r4,r4,0000393C
	l.sw	0(r4),r3
	l.jal	0000BF88
	l.addi	r3,r0,+0000000B
	l.movhi	r4,00000BEB
	l.ori	r3,r11,00000000
	l.jal	0000FFD8
	l.ori	r4,r4,0000C200
	l.sfeqi	r11,00000000
	l.bnf	0000AE24
	l.nop

l0000AE20:
	l.addi	r11,r0,+00000001

l0000AE24:
	l.movhi	r3,00000001
	l.addi	r4,r11,-00000001
	l.ori	r3,r3,00003948
	l.andi	r4,r4,0000001F
	l.lwz	r3,0(r3)
	l.addi	r2,r0,-00001F01
	l.lwz	r11,0(r3)
	l.slli	r4,r4,00000008
	l.and	r11,r11,r2
	l.addi	r2,r0,+00000000
	l.or	r11,r11,r4
	l.addi	r4,r0,+0000000B
	l.sw	0(r3),r11
	l.jal	0000B610
	l.addi	r3,r0,+00000001
	l.movhi	r3,00000001
	l.addi	r11,r0,+00000000
	l.ori	r3,r3,00003940
	l.sw	0(r3),r2
	l.movhi	r3,000001C2
	l.movhi	r2,0000FFFF
	l.ori	r3,r3,00000204
	l.lwz	r4,0(r3)
	l.and	r4,r4,r2
	l.ori	r4,r4,00000400
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000AE9C                                     18 60 00 01             .`..
0000AEA0 D7 E1 17 FC A8 63 39 44 9C 40 00 00 9C 21 FF FC .....c9D.@...!..
0000AEB0 D4 03 10 00 18 60 00 01 A9 62 00 00 A8 63 39 48 .....`...b...c9H
0000AEC0 D4 03 10 00 18 60 00 01 A8 63 39 3C D4 03 10 00 .....`...c9<....
0000AED0 9C 21 00 04 44 00 48 00 84 41 FF FC             .!..D.H..A..   

;; fn0000AEDC: 0000AEDC
;;   Called from:
;;     0000485C (in fn00004844)
;;     0000486C (in fn00004844)
;;     00004904 (in fn00004844)
;;     00004924 (in fn0000490C)
;;     0000494C (in fn0000490C)
;;     00004970 (in fn0000490C)
;;     00004980 (in fn00004978)
;;     000049A8 (in fn00004978)
;;     00004A94 (in fn00004A6C)
;;     00004AB0 (in fn00004A6C)
;;     00004AD8 (in fn00004A6C)
;;     00004AE8 (in fn00004A6C)
;;     00004AF8 (in fn00004A6C)
;;     00004B24 (in fn00004A6C)
;;     00007FF8 (in fn00007FD0)
;;     00008028 (in fn00008000)
;;     00008078 (in fn00008030)
;;     0000E99C (in fn0000E98C)
;;     0000E9AC (in fn0000E98C)
;;     0000E9BC (in fn0000E98C)
;;     0000E9CC (in fn0000E98C)
;;     0000E9DC (in fn0000E98C)
;;     0000E9EC (in fn0000E98C)
;;     0000EA0C (in fn0000E98C)
;;     0000EA1C (in fn0000E98C)
;;     0000EA2C (in fn0000E98C)
;;     0000EA3C (in fn0000E98C)
;;     0000EA4C (in fn0000E98C)
;;     0000EA74 (in fn0000E98C)
;;     0000EA84 (in fn0000E98C)
;;     0000EB40 (in fn0000E98C)
;;     0000F660 (in fn0000F444)
;;     00010590 (in fn00010570)
;;     000105EC (in fn00010570)
;;     0001067C (in fn00010570)
;;     00010684 (in fn00010570)
;;     000106F0 (in fn00010570)
;;     00010708 (in fn00010570)
;;     00010718 (in fn00010570)
;;     00010748 (in fn00010570)
;;     00010768 (in fn00010570)
;;     00010808 (in fn00010570)
;;     00010824 (in fn00010570)
;;     00010838 (in fn00010570)
;;     00010864 (in fn00010570)
;;     00010894 (in fn00010570)
;;     0001094C (in fn00010570)
;;     00010A08 (in fn00010570)
;;     00010AAC (in fn00010570)
;;     00010B50 (in fn00010570)
;;     00010C28 (in fn00010570)
;;     00010CD8 (in fn00010570)
;;     00010D90 (in fn00010570)
;;     00010E34 (in fn00010570)
;;     00010ED8 (in fn00010570)
;;     00010F88 (in fn00010570)
;;     0001102C (in fn00010570)
;;     000110D0 (in fn00010570)
;;     0001119C (in fn00010570)
;;     00011438 (in fn00010570)
;;     000114BC (in fn00010570)
;;     000114CC (in fn00010570)
;;     000114E4 (in fn00010570)
;;     00011500 (in fn00010570)
;;     00011738 (in fn00010570)
;;     00011740 (in fn00010570)
;;     00011754 (in fn00010570)
;;     000117C8 (in fn00010570)
;;     00011F1C (in fn00010570)
;;     00011F30 (in fn00010570)
;;     00012044 (in fn00010570)
;;     00012070 (in fn00010570)
;;     00012094 (in fn00010570)
;;     000120C8 (in fn00010570)
;;     000120D0 (in fn00010570)
;;     000120E8 (in fn00010570)
;;     000120F8 (in fn00010570)
;;     00012198 (in fn00010570)
;;     000121B0 (in fn00010570)
;;     000121BC (in fn00010570)
;;     000122E4 (in fn000122B8)
;;     000122FC (in fn000122B8)
fn0000AEDC proc
	l.movhi	r4,000001F0
	l.ori	r4,r4,0000010C
	l.sw	0(r4),r3
	l.jr	r9
	l.nop
0000AEF0 18 60 01 F0 A8 63 01 0C 85 63 00 00 44 00 48 00 .`...c...c..D.H.
0000AF00 15 00 00 00                                     ....           

;; fn0000AF04: 0000AF04
;;   Called from:
;;     00012340 (in fn00012320)
fn0000AF04 proc
	l.sw	0(r3),r4
	l.jr	r9
	l.nop
0000AF10 85 63 00 00 44 00 48 00 15 00 00 00             .c..D.H.....   

;; fn0000AF1C: 0000AF1C
;;   Called from:
;;     00009054 (in fn00009290)
;;     0000ACC0 (in fn0000AC84)
;;     0000B670 (in fn0000B610)
;;     00010630 (in fn00010570)
fn0000AF1C proc
	l.sw	-4(r1),r2
	l.addi	r3,r3,-00000001
	l.sfgtui	r3,0000001F
	l.bf	0000B01C
	l.addi	r1,r1,-00000004

l0000AF30:
	l.movhi	r2,00000001
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00002490
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.jr	r3
	l.nop

;; fn0000AF4C: 0000AF4C
;;   Called from:
;;     000062F0 (in fn000062BC)
fn0000AF4C proc
	l.j	0000B028
	l.addi	r11,r0,+00000011

;; fn0000AF54: 0000AF54
;;   Called from:
;;     000062F0 (in fn000062BC)
fn0000AF54 proc
	l.movhi	r3,00000001
	l.movhi	r2,00000001
	l.ori	r3,r3,00003948
	l.ori	r2,r2,000025DC
	l.lwz	r3,0(r3)
	l.lwz	r3,0(r3)
	l.srli	r3,r3,0000000E
	l.j	0000B010
	l.andi	r3,r3,0000000C
0000AF78                         18 60 01 C2 18 40 00 01         .`...@..
0000AF80 A8 63 00 50 A8 42 25 EC 84 63 00 00 B8 63 00 4E .c.P.B%..c...c.N
0000AF90 00 00 00 20 A4 63 00 0C 18 60 01 C2 18 40 00 01 ... .c...`...@..
0000AFA0 A8 63 00 54 A8 42 25 FC 84 63 00 00 B8 63 00 4A .c.T.B%..c...c.J
0000AFB0 00 00 00 18 A4 63 00 0C 18 60 01 C2 18 40 00 01 .....c...`...@..
0000AFC0 A8 63 00 58 A8 42 26 0C 84 63 00 00 B8 63 00 56 .c.X.B&..c...c.V
0000AFD0 00 00 00 10 A4 63 00 0C 00 00 00 14 9D 60 00 19 .....c.......`..
0000AFE0 18 60 00 01 9D 60 00 00 A8 63 39 48 84 63 00 00 .`...`...c9H.c..
0000AFF0 84 63 00 54 B8 63 00 58 A4 63 00 03 BC 43 00 01 .c.T.c.X.c...C..
0000B000 10 00 00 0A B8 63 00 02 18 40 00 01 A8 42 26 1C .....c...@...B&.

l0000B010:
	l.add	r3,r3,r2
	l.j	0000B028
	l.lwz	r11,0(r3)

l0000B01C:
	l.j	0000B028
	l.addi	r11,r0,+00000000
0000B024             9D 60 00 16                             .`..       

l0000B028:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000B034: 0000B034
;;   Called from:
;;     0000B174 (in fn0000B0B8)
fn0000B034 proc
	l.sw	-4(r1),r2
	l.sfgtui	r3,00000010
	l.bf	0000B058
	l.addi	r1,r1,-00000004

l0000B044:
	l.addi	r2,r0,+00000000
	l.sw	0(r4),r2
	l.sw	0(r5),r3
	l.j	0000B0AC
	l.ori	r11,r2,00000000

l0000B058:
	l.sfgtui	r3,00000020
	l.bf	0000B074
	l.sfgtui	r3,00000040

l0000B064:
	l.addi	r6,r0,+00000001
	l.sw	0(r4),r6
	l.j	0000B0A0
	l.add	r3,r3,r6

l0000B074:
	l.bf	0000B088
	l.addi	r6,r0,+00000002

l0000B07C:
	l.addi	r3,r3,+00000003
	l.j	0000B0A0
	l.sw	0(r4),r6

l0000B088:
	l.sfgtui	r3,00000080
	l.bf	0000B0AC
	l.addi	r11,r0,-00000016

l0000B094:
	l.addi	r6,r0,+00000003
	l.addi	r3,r3,+00000007
	l.sw	0(r4),r6

l0000B0A0:
	l.srl	r3,r3,r6
	l.addi	r11,r0,+00000000
	l.sw	0(r5),r3

l0000B0AC:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000B0B8: 0000B0B8
;;   Called from:
;;     000090C8 (in fn00009290)
;;     0000E2AC (in fn0000E1AC)
;;     0000E2F0 (in fn0000E1AC)
;;     000100E0 (in fn000100B8)
;;     000111DC (in fn00010570)
;;     000118E4 (in fn00010570)
fn0000B0B8 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.sfeqi	r3,00000003
	l.bf	0000B144
	l.addi	r1,r1,-00000010

l0000B0CC:
	l.sfgtui	r3,00000003
	l.bf	0000B0EC
	l.sfeqi	r3,00000014

l0000B0D8:
	l.sfeqi	r3,00000001
	l.bnf	0000B238
	l.movhi	r3,00000001

l0000B0E4:
	l.j	0000B104
	l.ori	r3,r3,00003948

l0000B0EC:
	l.bf	0000B1F4
	l.sfeqi	r3,0000001F

l0000B0F4:
	l.bnf	0000B238
	l.ori	r3,r4,00000000

l0000B0FC:
	l.j	0000B174
	l.ori	r5,r1,00000000

l0000B104:
	l.lwz	r5,0(r3)
	l.j	0000B114
	l.addi	r3,r0,+00000000

l0000B110:
	l.addi	r3,r3,+00000001

l0000B114:
	l.sfgtui	r4,00000001
	l.bf	0000B110
	l.srli	r4,r4,00000001

l0000B120:
	l.andi	r3,r3,00000003
	l.lwz	r4,0(r5)
	l.addi	r2,r0,-00000031
	l.slli	r3,r3,00000004
	l.and	r4,r4,r2
	l.or	r3,r4,r3
	l.sw	0(r5),r3
	l.j	0000B23C
	l.addi	r3,r0,+00000000

l0000B144:
	l.movhi	r3,00000001
	l.addi	r4,r4,-00000001
	l.ori	r3,r3,00003948
	l.addi	r2,r0,-00000004
	l.lwz	r3,0(r3)
	l.andi	r4,r4,00000003
	l.lwz	r5,12(r3)
	l.and	r5,r5,r2
	l.or	r4,r5,r4
	l.sw	12(r3),r4
	l.j	0000B23C
	l.addi	r3,r0,+00000000

l0000B174:
	l.jal	0000B034
	l.addi	r4,r1,+00000004
	l.sfnei	r11,00000000
	l.bf	0000B23C
	l.addi	r3,r0,-00000024

l0000B188:
	l.movhi	r3,00000001
	l.lwz	r4,4(r1)
	l.ori	r3,r3,00003948
	l.j	0000B1A0
	l.lwz	r3,0(r3)

l0000B19C:
	l.addi	r11,r11,+00000001

l0000B1A0:
	l.sfgtui	r4,00000001
	l.bf	0000B19C
	l.srli	r4,r4,00000001

l0000B1AC:
	l.movhi	r2,0000FFFC
	l.andi	r11,r11,00000003
	l.lwz	r4,84(r3)
	l.ori	r2,r2,0000FFFF
	l.slli	r11,r11,00000010
	l.and	r4,r4,r2
	l.addi	r2,r0,-00000010
	l.or	r11,r4,r11
	l.sw	84(r3),r11
	l.lwz	r5,0(r1)
	l.lwz	r4,84(r3)
	l.addi	r5,r5,-00000001
	l.and	r4,r4,r2
	l.andi	r5,r5,0000000F
	l.or	r4,r4,r5
	l.sw	84(r3),r4
	l.j	0000B23C
	l.addi	r3,r0,+00000000

l0000B1F4:
	l.movhi	r3,000001C2
	l.addi	r2,r0,-00000004
	l.ori	r3,r3,00000050
	l.addi	r4,r4,-00000001
	l.lwz	r5,0(r3)
	l.andi	r4,r4,00000003
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.and	r5,r5,r2
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.j	0000B23C
	l.addi	r3,r0,+00000000

l0000B238:
	l.addi	r3,r0,-00000016

l0000B23C:
	l.addi	r1,r1,+00000010
	l.ori	r11,r3,00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000B250: 0000B250
;;   Called from:
;;     00009068 (in fn00009290)
;;     0000C0BC (in fn0000BF88)
;;     0000E170 (in fn0000E110)
;;     0000E2B4 (in fn0000E1AC)
;;     0000E2F8 (in fn0000E1AC)
fn0000B250 proc
	l.sw	-4(r1),r2
	l.addi	r3,r3,-00000001
	l.sfgtui	r3,0000001F
	l.bf	0000B310
	l.addi	r1,r1,-00000008

l0000B264:
	l.movhi	r2,00000001
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00002510
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.jr	r3
	l.nop
0000B280 00 00 00 25 9D 60 00 01 18 60 00 01 A8 63 39 48 ...%.`...`...c9H
0000B290 84 63 00 00 85 63 00 00 9C 60 00 01 B9 6B 00 44 .c...c...`...k.D
0000B2A0 A5 6B 00 03 00 00 00 1C E1 63 58 08 18 60 00 01 .k.......cX..`..
0000B2B0 A8 63 39 48 84 63 00 00 85 63 00 0C 00 00 00 13 .c9H.c...c......
0000B2C0 A5 6B 00 03 18 60 00 01 A8 63 39 48 84 63 00 00 .k...`...c9H.c..
0000B2D0 85 63 00 54 84 63 00 54 A5 6B 00 0F B8 63 00 50 .c.T.c.T.k...c.P
0000B2E0 9D 6B 00 01 A4 63 00 03 00 00 00 0B E1 6B 18 08 .k...c.......k..
0000B2F0 18 60 01 C2 A8 63 00 50 84 63 00 00 D4 01 18 00 .`...c.P.c......
0000B300 85 61 00 00 A5 6B 00 03 00 00 00 03 9D 6B 00 01 .a...k.......k..

l0000B310:
	l.addi	r11,r0,-00000016
	l.addi	r1,r1,+00000008
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000B320: 0000B320
;;   Called from:
;;     00008308 (in fn000082FC)
;;     00008410 (in fn000083F8)
;;     000087D0 (in fn000087BC)
;;     000089AC (in fn00008998)
;;     000090BC (in fn00009290)
;;     000090DC (in fn00009290)
;;     000090E8 (in fn00009290)
;;     00009704 (in fn000096F8)
;;     0000C350 (in fn0000C32C)
;;     0000CBE0 (in fn0000CB40)
;;     0000D6C4 (in fn0000D640)
;;     0000D7DC (in fn0000D76C)
fn0000B320 proc
	l.sw	-4(r1),r2
	l.addi	r3,r3,-0000000E
	l.sfgtui	r3,00000012
	l.bf	0000B4C4
	l.addi	r1,r1,-00000004

l0000B334:
	l.movhi	r2,00000001
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00002590
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.jr	r3
	l.nop
0000B350 18 60 00 01 A4 84 00 01 A8 63 39 48 B8 84 00 06 .`.......c9H....
0000B360 84 63 00 00 9C 40 FF BF 84 A3 00 28 00 00 00 0A .c...@.....(....
0000B370 E0 A5 10 03 18 60 00 01 A4 84 00 01 A8 63 39 48 .....`.......c9H
0000B380 B8 84 00 04 84 63 00 00 9C 40 FF EF 84 A3 00 28 .....c...@.....(
0000B390 E0 A5 10 03 E0 85 20 04 D4 03 20 28 00 00 00 4B ...... ... (...K
0000B3A0 9D 60 00 00 18 60 00 01 A4 84 00 01 A8 63 39 48 .`...`.......c9H
0000B3B0 B8 84 00 02 84 63 00 00 9C 40 FF FB 84 A3 00 28 .....c...@.....(
0000B3C0 03 FF FF F5 E0 A5 10 03 18 60 00 01 A4 84 00 01 .........`......
0000B3D0 A8 63 39 48 B8 84 00 03 84 63 00 00 9C 40 FF F7 .c9H.....c...@..
0000B3E0 84 A3 00 28 03 FF FF EC E0 A5 10 03 18 60 00 01 ...(.........`..
0000B3F0 A4 84 00 01 A8 63 39 48 E0 84 20 00 84 63 00 00 .....c9H.. ..c..
0000B400 9C 40 FF FD 84 A3 00 28 03 FF FF E3 E0 A5 10 03 .@.....(........
0000B410 18 60 00 01 A4 84 00 01 A8 63 39 48 9C 40 FF FE .`.......c9H.@..
0000B420 84 63 00 00 84 A3 00 28 03 FF FF DB E0 A5 10 03 .c.....(........
0000B430 18 60 00 01 18 40 7F FF A8 63 39 48 A8 42 FF FF .`...@...c9H.B..
0000B440 84 63 00 00 B8 84 00 1F 84 A3 00 54 E0 A5 10 03 .c.........T....
0000B450 E0 85 20 04 D4 03 20 54 00 00 00 1C 9D 60 00 00 .. ... T.....`..
0000B460 18 60 01 C2 B8 84 00 06 A8 63 00 60 9C 40 FF BF .`.......c.`.@..
0000B470 84 A3 00 00 00 00 00 10 E0 A5 10 03 18 60 01 C2 .............`..
0000B480 B8 84 00 16 A8 63 00 64 18 40 FF BF 84 A3 00 00 .....c.d.@......
0000B490 00 00 00 08 A8 42 FF FF 18 60 01 C2 B8 84 00 15 .....B...`......
0000B4A0 A8 63 00 64 18 40 FF DF 84 A3 00 00 A8 42 FF FF .c.d.@.......B..
0000B4B0 E0 A5 10 03 E0 84 28 04 D4 03 20 00 00 00 00 03 ......(... .....
0000B4C0 9D 60 00 00                                     .`..           

l0000B4C4:
	l.addi	r11,r0,-00000016
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000B4D4: 0000B4D4
;;   Called from:
;;     0000CC6C (in fn0000CB40)
;;     0000D740 (in fn0000D640)
fn0000B4D4 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.movhi	r3,00000001
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	0000F8E8
	l.ori	r3,r3,00003940

;; fn0000B4F0: 0000B4F0
;;   Called from:
;;     0000D7B0 (in fn0000D76C)
fn0000B4F0 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.movhi	r3,00000001
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	0000F9A0
	l.ori	r3,r3,00003940

l0000B50C:
	l.sw	-4(r1),r9
	l.sw	-12(r1),r2
	l.sw	-8(r1),r14
	l.sfeqi	r3,00000002
	l.bf	0000B578
	l.addi	r1,r1,-0000000C

l0000B524:
	l.sfgtui	r3,00000002
	l.bf	0000B544
	l.sfeqi	r3,00000003

l0000B530:
	l.sfeqi	r3,00000001
	l.bnf	0000B5F8
	l.movhi	r2,00000001

l0000B53C:
	l.j	0000B56C
	l.addi	r14,r0,+00000000

l0000B544:
	l.bf	0000B55C
	l.sfeqi	r3,0000000B

l0000B54C:
	l.bnf	0000B5F8
	l.movhi	r2,00000BEB

l0000B554:
	l.j	0000B58C
	l.addi	r14,r0,+00000002

l0000B55C:
	l.movhi	r2,0000016E
	l.addi	r14,r0,+00000001
	l.j	0000B590
	l.ori	r2,r2,00003600

l0000B56C:
	l.ori	r2,r2,0000309C
	l.j	0000B590
	l.lwz	r2,0(r2)

l0000B578:
	l.movhi	r2,00000001
	l.addi	r14,r0,+00000003
	l.ori	r2,r2,00003098
	l.j	0000B590
	l.lwz	r2,0(r2)

l0000B58C:
	l.ori	r2,r2,0000C200

l0000B590:
	l.movhi	r3,00000001
	l.addi	r4,r0,+00000000
	l.ori	r3,r3,00003940
	l.ori	r5,r2,00000000
	l.jal	0000FA38
	l.slli	r14,r14,00000010
	l.movhi	r3,00000001
	l.addi	r5,r0,-00000004
	l.ori	r3,r3,00003948
	l.lwz	r3,0(r3)
	l.lwz	r4,12(r3)
	l.and	r4,r4,r5
	l.movhi	r5,0000FFFC
	l.sw	12(r3),r4
	l.ori	r5,r5,0000FFFF
	l.lwz	r4,0(r3)
	l.and	r4,r4,r5
	l.ori	r5,r2,00000000
	l.or	r14,r4,r14
	l.addi	r4,r0,+00000001
	l.sw	0(r3),r14
	l.movhi	r3,00000001
	l.jal	0000FA38
	l.ori	r3,r3,00003940
	l.j	0000B5FC
	l.addi	r11,r0,+00000000

l0000B5F8:
	l.addi	r11,r0,-00000016

l0000B5FC:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn0000B610: 0000B610
;;   Called from:
;;     000090B0 (in fn00009290)
;;     0000A950 (in fn0000A92C)
;;     0000ACD0 (in fn0000AC84)
;;     0000AD04 (in fn0000AC84)
;;     0000AD38 (in fn0000AC84)
;;     0000AE58 (in fn0000ADBC)
;;     000100CC (in fn000100B8)
;;     000100EC (in fn000100B8)
;;     00010128 (in fn000100B8)
;;     0001015C (in fn000100B8)
;;     0001081C (in fn00010570)
;;     00010870 (in fn00010570)
;;     0001087C (in fn00010570)
;;     00010888 (in fn00010570)
;;     000111B8 (in fn00010570)
;;     00011234 (in fn00010570)
;;     000112CC (in fn00010570)
;;     000112F0 (in fn00010570)
;;     00011360 (in fn00010570)
;;     000113BC (in fn00010570)
;;     000117F8 (in fn00010570)
;;     000118D0 (in fn00010570)
;;     0001190C (in fn00010570)
;;     00011964 (in fn00010570)
;;     0001198C (in fn00010570)
;;     000121D0 (in fn00010570)
fn0000B610 proc
	l.sw	-8(r1),r2
	l.sw	-4(r1),r9
	l.ori	r2,r4,00000000
	l.sfeqi	r3,0000000A
	l.bf	0000B700
	l.addi	r1,r1,-00000008

l0000B628:
	l.sfgtui	r3,0000000A
	l.bf	0000B650
	l.sfeqi	r3,0000000D

l0000B634:
	l.sfeqi	r3,00000001
	l.bf	0000B670
	l.sfeqi	r3,00000004

l0000B640:
	l.bnf	0000B8C8
	l.addi	r11,r0,-00000016

l0000B648:
	l.j	0000B694
	l.sfeqi	r4,00000003

l0000B650:
	l.bf	0000B7DC
	l.sfeqi	r3,0000001F

l0000B658:
	l.bf	0000B85C
	l.sfeqi	r3,0000000B

l0000B660:
	l.bnf	0000B8C8
	l.addi	r11,r0,-00000016

l0000B668:
	l.j	0000B79C
	l.sfeqi	r4,0000000B

l0000B670:
	l.jal	0000AF1C
	l.nop
	l.sfeq	r11,r2
	l.bf	0000B8BC
	l.ori	r3,r2,00000000

l0000B684:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.j	0000B50C
	l.lwz	r2,-8(r1)

l0000B694:
	l.bf	0000B6C0
	l.sfeqi	r4,00000006

l0000B69C:
	l.bf	0000B6E0
	l.sfeqi	r4,00000001

l0000B6A4:
	l.bnf	0000B8C4
	l.movhi	r4,0000FFFC

l0000B6AC:
	l.movhi	r2,000001C2
	l.ori	r2,r2,00000050
	l.lwz	r3,0(r2)
	l.j	0000B808
	l.ori	r4,r4,0000FFFF

l0000B6C0:
	l.movhi	r2,000001C2
	l.movhi	r4,0000FFFC
	l.ori	r2,r2,00000050
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.j	0000B84C
	l.movhi	r4,00000001

l0000B6E0:
	l.movhi	r2,000001C2
	l.movhi	r4,0000FFFC
	l.ori	r2,r2,00000050
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.j	0000B84C
	l.movhi	r4,00000002

l0000B700:
	l.sfeqi	r4,00000003
	l.bf	0000B754
	l.sfgtui	r4,00000003

l0000B70C:
	l.bf	0000B728
	l.sfeqi	r4,00000004

l0000B714:
	l.sfeqi	r4,00000001
	l.bnf	0000B8C4
	l.nop

l0000B720:
	l.j	0000B740
	l.movhi	r2,000001C2

l0000B728:
	l.bf	0000B770
	l.sfeqi	r4,0000000B

l0000B730:
	l.bnf	0000B8C4
	l.nop

l0000B738:
	l.j	0000B78C
	l.movhi	r2,000001C2

l0000B740:
	l.addi	r4,r0,-00003001
	l.ori	r2,r2,00000054
	l.lwz	r3,0(r2)
	l.j	0000B850
	l.and	r3,r3,r4

l0000B754:
	l.movhi	r2,000001C2
	l.addi	r4,r0,-00003001
	l.ori	r2,r2,00000054
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.j	0000B850
	l.ori	r3,r3,00001000

l0000B770:
	l.movhi	r2,000001C2
	l.addi	r4,r0,-00003001
	l.ori	r2,r2,00000054
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.j	0000B850
	l.ori	r3,r3,00002000

l0000B78C:
	l.ori	r2,r2,00000054
	l.lwz	r3,0(r2)
	l.j	0000B850
	l.ori	r3,r3,00003000

l0000B79C:
	l.bf	0000B7C0
	l.sfeqi	r4,00000017

l0000B7A4:
	l.bnf	0000B8C4
	l.addi	r4,r0,-00000004

l0000B7AC:
	l.movhi	r2,000001C2
	l.ori	r2,r2,0000005C
	l.lwz	r3,0(r2)
	l.j	0000B850
	l.and	r3,r3,r4

l0000B7C0:
	l.movhi	r2,000001C2
	l.addi	r4,r0,-00000004
	l.ori	r2,r2,0000005C
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.j	0000B850
	l.ori	r3,r3,00000001

l0000B7DC:
	l.sfeqi	r4,00000003
	l.bf	0000B810
	l.sfeqi	r4,0000000B

l0000B7E8:
	l.bf	0000B830
	l.sfeqi	r4,00000001

l0000B7F0:
	l.bnf	0000B8C4
	l.movhi	r4,0000FCFF

l0000B7F8:
	l.movhi	r2,000001C2
	l.ori	r2,r2,00000058
	l.lwz	r3,0(r2)
	l.ori	r4,r4,0000FFFF

l0000B808:
	l.j	0000B850
	l.and	r3,r3,r4

l0000B810:
	l.movhi	r2,000001C2
	l.movhi	r4,0000FCFF
	l.ori	r2,r2,00000058
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.j	0000B84C
	l.movhi	r4,00000100

l0000B830:
	l.movhi	r2,000001C2
	l.movhi	r4,0000FCFF
	l.ori	r2,r2,00000058
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.movhi	r4,00000200

l0000B84C:
	l.or	r3,r3,r4

l0000B850:
	l.sw	0(r2),r3
	l.j	0000B8C8
	l.addi	r11,r0,+00000000

l0000B85C:
	l.sfeqi	r4,00000001
	l.bf	0000B878
	l.sfeqi	r4,00000003

l0000B868:
	l.bnf	0000B8C4
	l.nop

l0000B870:
	l.j	0000B898
	l.movhi	r2,00000001

l0000B878:
	l.movhi	r2,00000001
	l.movhi	r4,0000FCFF
	l.ori	r2,r2,00003948
	l.ori	r4,r4,0000FFFF
	l.lwz	r2,0(r2)
	l.lwz	r3,84(r2)
	l.j	0000B8B8
	l.and	r3,r3,r4

l0000B898:
	l.movhi	r4,0000FCFF
	l.ori	r2,r2,00003948
	l.ori	r4,r4,0000FFFF
	l.lwz	r2,0(r2)
	l.lwz	r3,84(r2)
	l.and	r3,r3,r4
	l.movhi	r4,00000100
	l.or	r3,r3,r4

l0000B8B8:
	l.sw	84(r2),r3

l0000B8BC:
	l.j	0000B8C8
	l.addi	r11,r0,+00000000

l0000B8C4:
	l.addi	r11,r0,-00000016

l0000B8C8:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000B8D8: 0000B8D8
;;   Called from:
;;     0000BE24 (in fn0000BE00)
fn0000B8D8 proc
	l.sw	-8(r1),r2
	l.ori	r2,r3,00000000
	l.movhi	r3,00007829
	l.sw	-4(r1),r9
	l.ori	r3,r3,0000B800
	l.sfleu	r4,r3
	l.bf	0000B8FC
	l.addi	r1,r1,-00000008

l0000B8F8:
	l.ori	r4,r3,00000000

l0000B8FC:
	l.ori	r3,r4,00000000
	l.movhi	r4,0000005B
	l.jal	0000FEDC
	l.ori	r4,r4,00008D80
	l.movhi	r3,00000001
	l.slli	r11,r11,00000002
	l.ori	r3,r3,00002624
	l.add	r11,r11,r3
	l.lbz	r3,0(r11)
	l.sb	0(r2),r3
	l.lbz	r3,1(r11)
	l.sb	1(r2),r3
	l.lbz	r3,2(r11)
	l.lbz	r11,3(r11)
	l.sb	2(r2),r3
	l.sb	3(r2),r11
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000B950: 0000B950
;;   Called from:
;;     00004944 (in fn0000490C)
;;     00004AA0 (in fn00004A6C)
;;     0000A974 (in fn0000A92C)
;;     0000A980 (in fn0000A92C)
;;     00011458 (in fn00010570)
;;     00011464 (in fn00010570)
;;     0001148C (in fn00010570)
;;     00011774 (in fn00010570)
;;     00011780 (in fn00010570)
fn0000B950 proc
	l.sw	-4(r1),r2
	l.addi	r3,r3,-00000001
	l.sfgtui	r3,0000000E
	l.bf	0000BA98
	l.addi	r1,r1,-00000004

l0000B964:
	l.movhi	r2,00000001
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00002B68
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.jr	r3
	l.nop
0000B980 18 60 00 01 A4 84 00 01 A8 63 39 48 9C 40 FF FE .`.......c9H.@..
0000B990 84 63 00 00 84 A3 01 00 00 00 00 0A E0 A5 10 03 .c..............
0000B9A0 18 60 00 01 A4 84 00 01 A8 63 39 48 E0 84 20 00 .`.......c9H.. .
0000B9B0 84 63 00 00 9C 40 FF FD 84 A3 01 00 E0 A5 10 03 .c...@..........
0000B9C0 E0 85 20 04 D4 03 21 00 00 00 00 35 9D 60 00 00 .. ...!....5.`..
0000B9D0 18 60 00 01 A4 84 00 01 A8 63 39 48 B8 84 00 02 .`.......c9H....
0000B9E0 84 63 00 00 9C 40 FF FB 84 A3 01 00 03 FF FF F5 .c...@..........
0000B9F0 E0 A5 10 03 18 60 00 01 A4 84 00 01 A8 63 39 48 .....`.......c9H
0000BA00 B8 84 00 03 84 63 00 00 9C 40 FF F7 84 A3 01 00 .....c...@......
0000BA10 03 FF FF EC E0 A5 10 03 18 60 00 01 A4 84 00 01 .........`......
0000BA20 A8 63 39 48 B8 84 00 03 84 63 00 00 9C 40 FF F7 .c9H.....c...@..
0000BA30 84 A3 01 10 00 00 00 0A E0 A5 10 03 18 60 00 01 .............`..
0000BA40 A4 84 00 01 A8 63 39 48 B8 84 00 02 84 63 00 00 .....c9H.....c..
0000BA50 9C 40 FF FB 84 A3 01 10 E0 A5 10 03 E0 85 20 04 .@............ .
0000BA60 D4 03 21 10 00 00 00 0E 9D 60 00 00 18 60 00 01 ..!......`...`..
0000BA70 9C 40 FF FE A8 63 39 48 A4 84 00 01 84 63 00 00 .@...c9H.....c..
0000BA80 84 A3 01 18 E0 A5 10 03 E0 85 20 04 D4 03 21 18 .......... ...!.
0000BA90 00 00 00 03 9D 60 00 00                         .....`..       

l0000BA98:
	l.addi	r11,r0,-00000016
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000BAA8: 0000BAA8
;;   Called from:
;;     0000C410 (in fn0000C32C)
fn0000BAA8 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.ori	r4,r3,00000000
	l.addi	r1,r1,+00000004
	l.movhi	r3,00000001
	l.lwz	r9,-4(r1)
	l.j	0000F8E8
	l.ori	r3,r3,000034B4

;; fn0000BAC8: 0000BAC8
;;   Called from:
;;     0000A960 (in fn0000A92C)
;;     000113CC (in fn00010570)
fn0000BAC8 proc
	l.sw	-8(r1),r2
	l.movhi	r2,00000001
	l.addi	r4,r0,+00000001
	l.ori	r2,r2,000034B8
	l.sw	-4(r1),r9
	l.movhi	r3,00000001
	l.sw	0(r2),r4
	l.movhi	r2,000001F0
	l.addi	r1,r1,-00000008
	l.ori	r3,r3,000034B4
	l.addi	r5,r0,+00000000
	l.jal	0000FA38
	l.ori	r2,r2,00001444
	l.lwz	r3,0(r2)
	l.movhi	r4,0000A700
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.addi	r4,r0,-00000004
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.jal	0000DB28
	l.addi	r3,r0,+00000014
	l.lwz	r3,0(r2)
	l.movhi	r4,0000A700
	l.addi	r11,r0,+00000000
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.addi	r4,r0,-00000005
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000BB60: 0000BB60
;;   Called from:
;;     000117E4 (in fn00010570)
fn0000BB60 proc
	l.sw	-8(r1),r2
	l.movhi	r2,000001F0
	l.movhi	r4,0000A700
	l.ori	r2,r2,00001444
	l.sw	-4(r1),r9
	l.lwz	r3,0(r2)
	l.addi	r1,r1,-00000008
	l.or	r3,r3,r4
	l.ori	r4,r4,00000004
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.jal	0000C768
	l.addi	r3,r0,+00000002
	l.lwz	r3,0(r2)
	l.movhi	r4,0000A700
	l.or	r3,r3,r4
	l.ori	r4,r4,00000003
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.jal	0000C768
	l.addi	r3,r0,+00000002
	l.lwz	r3,0(r2)
	l.movhi	r4,0000A700
	l.or	r3,r3,r4
	l.movhi	r4,000058F0
	l.sw	0(r2),r3
	l.ori	r4,r4,00000FFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.movhi	r4,0000A707
	l.or	r3,r3,r4
	l.addi	r4,r0,+00000000
	l.sw	0(r2),r3
	l.movhi	r3,00000001
	l.ori	r5,r4,00000000
	l.ori	r3,r3,000034B4
	l.jal	0000FA38
	l.movhi	r2,00000001
	l.ori	r2,r2,000034B8
	l.addi	r3,r0,+00000000
	l.sw	0(r2),r3
	l.addi	r1,r1,+00000008
	l.ori	r11,r3,00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000BC28: 0000BC28
;;   Called from:
;;     0000C8B8 (in fn0000C8A0)
fn0000BC28 proc
	l.movhi	r3,00000001
	l.ori	r3,r3,000034B8
	l.jr	r9
	l.lwz	r11,0(r3)

;; fn0000BC38: 0000BC38
;;   Called from:
;;     00008314 (in fn000082FC)
;;     00008404 (in fn000083F8)
;;     000087DC (in fn000087BC)
;;     000089B8 (in fn00008998)
;;     0000A994 (in fn0000A92C)
;;     0000BDD8 (in fn0000BDC8)
;;     0000BDE4 (in fn0000BDC8)
;;     0000C35C (in fn0000C32C)
;;     0000CBEC (in fn0000CB40)
;;     0000D6B8 (in fn0000D640)
;;     0000D7E8 (in fn0000D76C)
;;     000114B0 (in fn00010570)
;;     000117AC (in fn00010570)
fn0000BC38 proc
	l.sw	-4(r1),r2
	l.addi	r3,r3,-0000000E
	l.sfgtui	r3,00000013
	l.bf	0000BDB8
	l.addi	r1,r1,-00000004

l0000BC4C:
	l.movhi	r2,00000001
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00002BA4
	l.add	r3,r3,r2
	l.lwz	r3,0(r3)
	l.jr	r3
	l.nop
0000BC68                         18 60 00 01 A4 84 00 01         .`......
0000BC70 A8 63 39 48 B8 84 00 06 84 63 00 00 9C 40 FF BF .c9H.....c...@..
0000BC80 84 A3 00 B0 00 00 00 0A E0 A5 10 03 18 60 00 01 .............`..
0000BC90 A4 84 00 01 A8 63 39 48 B8 84 00 04 84 63 00 00 .....c9H.....c..
0000BCA0 9C 40 FF EF 84 A3 00 B0 E0 A5 10 03 E0 85 20 04 .@............ .
0000BCB0 D4 03 20 B0 00 00 00 42 9D 60 00 00 18 60 00 01 .. ....B.`...`..
0000BCC0 A4 84 00 01 A8 63 39 48 B8 84 00 02 84 63 00 00 .....c9H.....c..
0000BCD0 9C 40 FF FB 84 A3 00 B0 03 FF FF F5 E0 A5 10 03 .@..............
0000BCE0 18 60 00 01 A4 84 00 01 A8 63 39 48 B8 84 00 03 .`.......c9H....
0000BCF0 84 63 00 00 9C 40 FF F7 84 A3 00 B0 03 FF FF EC .c...@..........
0000BD00 E0 A5 10 03 18 60 00 01 A4 84 00 01 A8 63 39 48 .....`.......c9H
0000BD10 E0 84 20 00 84 63 00 00 9C 40 FF FD 84 A3 00 B0 .. ..c...@......
0000BD20 03 FF FF E3 E0 A5 10 03 18 60 00 01 9C 40 FF FE .........`...@..
0000BD30 A8 63 39 48 A4 84 00 01 84 63 00 00 84 A3 01 20 .c9H.....c..... 
0000BD40 E0 A5 10 03 E0 85 20 04 D4 03 21 20 00 00 00 1C ...... ...! ....
0000BD50 9D 60 00 00 18 60 01 C2 B8 84 00 06 A8 63 02 C0 .`...`.......c..
0000BD60 9C 40 FF BF 84 A3 00 00 00 00 00 10 E0 A5 10 03 .@..............
0000BD70 18 60 01 C2 B8 84 00 16 A8 63 02 C4 18 40 FF BF .`.......c...@..
0000BD80 84 A3 00 00 00 00 00 08 A8 42 FF FF 18 60 01 C2 .........B...`..
0000BD90 B8 84 00 15 A8 63 02 C4 18 40 FF DF 84 A3 00 00 .....c...@......
0000BDA0 A8 42 FF FF E0 A5 10 03 E0 84 28 04 D4 03 20 00 .B........(... .
0000BDB0 00 00 00 03 9D 60 00 00                         .....`..       

l0000BDB8:
	l.addi	r11,r0,-00000016
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000BDC8: 0000BDC8
;;   Called from:
;;     000090D0 (in fn00009290)
fn0000BDC8 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r4,r0,+00000000
	l.addi	r1,r1,-00000008
	l.jal	0000BC38
	l.ori	r2,r3,00000000
	l.ori	r3,r2,00000000
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000BE00: 0000BE00
;;   Called from:
;;     0000E2C8 (in fn0000E1AC)
fn0000BE00 proc
	l.sw	-8(r1),r14
	l.sw	-4(r1),r9
	l.sw	-12(r1),r2
	l.addi	r14,r0,-00000016
	l.sfeqi	r3,00000006
	l.bnf	0000BF70
	l.addi	r1,r1,-00000010

l0000BE1C:
	l.movhi	r2,00000001
	l.ori	r3,r1,00000000
	l.jal	0000B8D8
	l.ori	r2,r2,00003944
	l.lwz	r3,0(r2)
	l.lbz	r4,3(r1)
	l.lwz	r2,0(r3)
	l.srli	r5,r2,00000010
	l.andi	r5,r5,00000003
	l.sfges	r5,r4
	l.bf	0000BE6C
	l.movhi	r5,0000FFFC

l0000BE4C:
	l.andi	r4,r4,00000003
	l.ori	r5,r5,0000FFFF
	l.slli	r4,r4,00000010
	l.and	r2,r2,r5
	l.or	r2,r2,r4
	l.sw	0(r3),r2
	l.jal	0000DB28
	l.addi	r3,r0,+000007D0

l0000BE6C:
	l.lbz	r3,2(r1)
	l.andi	r4,r2,00000003
	l.sfges	r4,r3
	l.bf	0000BEA4
	l.addi	r4,r0,-00000004

l0000BE80:
	l.andi	r3,r3,00000003
	l.and	r2,r2,r4
	l.or	r2,r2,r3
	l.movhi	r3,00000001
	l.ori	r3,r3,00003944
	l.lwz	r3,0(r3)
	l.sw	0(r3),r2
	l.jal	0000DB28
	l.addi	r3,r0,+000007D0

l0000BEA4:
	l.lbz	r3,0(r1)
	l.andi	r3,r3,0000001F
	l.addi	r5,r0,-00001F01
	l.slli	r3,r3,00000008
	l.and	r2,r2,r5
	l.addi	r4,r0,-00000031
	l.or	r2,r2,r3
	l.lbz	r3,1(r1)
	l.andi	r3,r3,00000003
	l.movhi	r14,00000001
	l.slli	r3,r3,00000004
	l.and	r2,r2,r4
	l.ori	r14,r14,00003944
	l.or	r2,r2,r3
	l.lwz	r3,0(r14)
	l.sw	0(r3),r2
	l.jal	0000DB28
	l.addi	r3,r0,+00000014
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.lbz	r3,2(r1)
	l.andi	r4,r2,00000003
	l.sfles	r4,r3
	l.bf	0000BF2C
	l.srli	r4,r2,00000010

l0000BF08:
	l.addi	r5,r0,-00000004
	l.andi	r3,r3,00000003
	l.and	r2,r2,r5
	l.or	r2,r2,r3
	l.lwz	r3,0(r14)
	l.sw	0(r3),r2
	l.jal	0000DB28
	l.addi	r3,r0,+000007D0
	l.srli	r4,r2,00000010

l0000BF2C:
	l.lbz	r3,3(r1)
	l.andi	r4,r4,00000003
	l.sfles	r4,r3
	l.bf	0000BF70
	l.addi	r14,r0,+00000000

l0000BF40:
	l.movhi	r4,0000FFFC
	l.andi	r3,r3,00000003
	l.ori	r4,r4,0000FFFF
	l.slli	r3,r3,00000010
	l.and	r2,r2,r4
	l.or	r2,r2,r3
	l.movhi	r3,00000001
	l.ori	r3,r3,00003944
	l.lwz	r3,0(r3)
	l.sw	0(r3),r2
	l.jal	0000DB28
	l.addi	r3,r0,+000007D0

l0000BF70:
	l.addi	r1,r1,+00000010
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn0000BF88: 0000BF88
;;   Called from:
;;     0000ADFC (in fn0000ADBC)
;;     0000C07C (in fn0000BF88)
;;     0000C0B0 (in fn0000BF88)
;;     0000C8CC (in fn0000C8A0)
;;     0000C9E8 (in fn0000C9C8)
;;     0000D6CC (in fn0000D640)
;;     0000D9A4 (in fn0000D95C)
;;     0000E124 (in fn0000E110)
;;     0000E2D4 (in fn0000E1AC)
fn0000BF88 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.sfeqi	r3,0000000B
	l.bf	0000C0DC
	l.addi	r1,r1,-00000008

l0000BF9C:
	l.sfgtui	r3,0000000B
	l.bf	0000BFCC
	l.sfeqi	r3,00000016

l0000BFA8:
	l.sfeqi	r3,00000003
	l.bf	0000C124
	l.sfeqi	r3,00000006

l0000BFB4:
	l.bf	0000BFEC
	l.sfeqi	r3,00000001

l0000BFBC:
	l.bnf	0000C11C
	l.nop

l0000BFC4:
	l.j	0000C074
	l.movhi	r2,00000001

l0000BFCC:
	l.bf	0000C03C
	l.sfeqi	r3,00000019

l0000BFD4:
	l.bf	0000C0B0
	l.sfeqi	r3,00000011

l0000BFDC:
	l.bnf	0000C12C
	l.addi	r11,r0,+00000000

l0000BFE4:
	l.j	0000C040
	l.movhi	r2,00000001

l0000BFEC:
	l.movhi	r2,00000001
	l.ori	r2,r2,00003944
	l.lwz	r2,0(r2)
	l.lwz	r3,0(r2)
	l.srli	r5,r3,00000008
	l.srli	r6,r3,00000004
	l.andi	r4,r3,00000003
	l.srli	r3,r3,00000010
	l.andi	r5,r5,0000001F
	l.andi	r6,r6,00000003
	l.andi	r2,r3,00000003
	l.addi	r5,r5,+00000001
	l.addi	r3,r6,+00000001
	l.addi	r4,r4,+00000001
	l.mul	r5,r5,r3
	l.movhi	r3,0000016E
	l.sll	r4,r4,r2
	l.ori	r3,r3,00003600
	l.j	0000C0CC
	l.mul	r3,r5,r3

l0000C03C:
	l.movhi	r2,00000001

l0000C040:
	l.ori	r2,r2,00003948
	l.lwz	r3,0(r2)
	l.lwz	r3,0(r3)
	l.srli	r3,r3,00000010
	l.andi	r3,r3,00000003
	l.sfeqi	r3,00000002
	l.bf	0000C07C
	l.sfeqi	r3,00000003

l0000C060:
	l.bf	0000C0A0
	l.sfeqi	r3,00000001

l0000C068:
	l.bf	0000C124
	l.nop

l0000C070:
	l.movhi	r2,00000001

l0000C074:
	l.j	0000C0A8
	l.ori	r2,r2,0000309C

l0000C07C:
	l.jal	0000BF88
	l.addi	r3,r0,+0000000B
	l.lwz	r2,0(r2)
	l.ori	r3,r11,00000000
	l.lwz	r4,0(r2)
	l.srli	r4,r4,00000008
	l.andi	r4,r4,0000001F
	l.j	0000C0CC
	l.addi	r4,r4,+00000001

l0000C0A0:
	l.movhi	r2,00000001
	l.ori	r2,r2,00003098

l0000C0A8:
	l.j	0000C12C
	l.lwz	r11,0(r2)

l0000C0B0:
	l.jal	0000BF88
	l.addi	r3,r0,+00000016
	l.addi	r3,r0,+00000003
	l.jal	0000B250
	l.ori	r2,r11,00000000
	l.ori	r3,r2,00000000
	l.ori	r4,r11,00000000

l0000C0CC:
	l.jal	0000FFD8
	l.nop
	l.j	0000C130
	l.addi	r1,r1,+00000008

l0000C0DC:
	l.movhi	r2,00000001
	l.movhi	r3,0000016E
	l.ori	r2,r2,0000393C
	l.ori	r3,r3,00003600
	l.lwz	r2,0(r2)
	l.lwz	r2,0(r2)
	l.srli	r11,r2,00000008
	l.srli	r2,r2,00000004
	l.andi	r11,r11,0000001F
	l.andi	r2,r2,00000003
	l.addi	r11,r11,+00000001
	l.addi	r2,r2,+00000001
	l.mul	r11,r11,r2
	l.mul	r11,r11,r3
	l.j	0000C12C
	l.srai	r11,r11,00000001

l0000C11C:
	l.j	0000C12C
	l.addi	r11,r0,+00000000

l0000C124:
	l.movhi	r11,0000016E
	l.ori	r11,r11,00003600

l0000C12C:
	l.addi	r1,r1,+00000008

l0000C130:
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000C13C                                     D7 E1 17 F0             ....
0000C140 A8 43 00 00 18 60 00 01 D7 E1 4F FC A8 63 30 A0 .C...`....O..c0.
0000C150 D7 E1 77 F4 D7 E1 87 F8 84 63 00 00 9C 21 FF F0 ..w......c...!..
0000C160 BC 23 00 00 10 00 00 18 9D 60 FF F3 1A 00 00 01 .#.......`......
0000C170 9D C0 00 01 AA 10 39 58 85 62 00 00 84 70 00 00 ......9X.b...p..
0000C180 E1 6E 58 08 84 63 00 04 E1 6B 18 03 BC 0B 00 00 .nX..c...k......
0000C190 10 00 00 0D 15 00 00 00 85 62 00 10 BC 0B 00 00 .........b......
0000C1A0 10 00 00 09 15 00 00 00 48 00 58 00 84 62 00 14 ........H.X..b..
0000C1B0 84 42 00 00 84 70 00 00 E0 4E 10 08 A9 6E 00 00 .B...p...N...n..
0000C1C0 D4 03 10 04 9C 21 00 10 85 21 FF FC 84 41 FF F0 .....!...!...A..
0000C1D0 85 C1 FF F4 44 00 48 00 86 01 FF F8 D7 E1 17 EC ....D.H.........
0000C1E0 18 40 00 01 D7 E1 87 F4 A8 42 30 A0 D7 E1 4F FC .@.......B0...O.
0000C1F0 D7 E1 77 F0 D7 E1 97 F8 86 02 00 00 9C 21 FF EC ..w..........!..
0000C200 BC 30 00 00 10 00 00 43 9D 60 FF F3 BC 03 00 00 .0.....C.`......
0000C210 10 00 00 22 18 80 00 01 BC 03 00 01 0C 00 00 3C ..."...........<
0000C220 19 C0 00 01 9E 40 00 20 A9 CE 30 A4 9C 80 02 EE .....@. ..0.....
0000C230 84 4E 00 18 84 62 00 00 E0 63 58 03 D4 02 18 00 .N...b...cX.....
0000C240 84 62 00 00 D4 02 18 00 D4 0E 90 0C 84 62 00 04 .b...........b..
0000C250 04 00 0F 23 15 00 00 00 D4 02 58 04 84 4E 00 34 ...#......X..N.4
0000C260 9C E0 FF F3 84 62 00 00 9C 80 02 EE E0 63 38 03 .....b.......c8.
0000C270 D4 02 18 00 84 62 00 00 D4 02 18 00 D4 0E 90 28 .....b.........(
0000C280 84 62 00 04 04 00 0F 16 15 00 00 00 D4 02 58 04 .b............X.
0000C290 00 00 00 20 A9 70 00 00 9C C0 02 EE A8 84 30 A4 ... .p........0.
0000C2A0 9C E0 FF F3 84 44 00 18 A9 63 00 00 84 A2 00 04 .....D...c......
0000C2B0 E0 A5 33 06 D4 02 28 04 84 A2 00 00 E0 A5 38 03 ..3...(.......8.
0000C2C0 D4 02 28 00 84 A2 00 00 A8 A5 00 04 D4 02 28 00 ..(...........(.
0000C2D0 84 44 00 34 9C A0 5D C0 84 E2 00 04 D4 04 28 0C .D.4..].......(.
0000C2E0 E0 C7 33 06 D4 02 30 04 9C E0 FF F3 84 C2 00 00 ..3...0.........
0000C2F0 E0 C6 38 03 D4 02 30 00 84 C2 00 00 A8 C6 00 04 ..8...0.........
0000C300 D4 02 30 00 00 00 00 03 D4 04 28 28 9D 60 FF FD ..0.......((.`..
0000C310 9C 21 00 14 85 21 FF FC 84 41 FF EC 85 C1 FF F0 .!...!...A......
0000C320 86 01 FF F4 44 00 48 00 86 41 FF F8             ....D.H..A..   

;; fn0000C32C: 0000C32C
;;   Called from:
;;     0000EA5C (in fn0000E98C)
fn0000C32C proc
	l.sw	-8(r1),r2
	l.movhi	r3,000001F0
	l.movhi	r2,00000001
	l.ori	r3,r3,00000800
	l.ori	r2,r2,00003958
	l.sw	-4(r1),r9
	l.sw	0(r2),r3
	l.addi	r1,r1,-00000008
	l.addi	r3,r0,+0000001C
	l.jal	0000B320
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+0000001C
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.movhi	r4,00000001
	l.lwz	r6,0(r2)
	l.ori	r4,r4,000030BC
	l.addi	r3,r0,+00000000
	l.addi	r5,r0,+00005DC0

l0000C378:
	l.addi	r2,r3,+00000001
	l.addi	r7,r0,+00000000
	l.slli	r3,r2,00000005
	l.addi	r8,r0,-0000000D
	l.sfnei	r2,00000002
	l.add	r3,r6,r3
	l.sw	0(r4),r3
	l.sw	0(r3),r7
	l.lwz	r7,0(r3)
	l.and	r7,r7,r8
	l.addi	r8,r0,-00000071
	l.sw	0(r3),r7
	l.lwz	r7,0(r3)
	l.ori	r7,r7,00000004
	l.sw	0(r3),r7
	l.lwz	r7,0(r3)
	l.and	r7,r7,r8
	l.sw	0(r3),r7
	l.lwz	r7,0(r3)
	l.sw	0(r3),r7
	l.sw	-12(r4),r5
	l.ori	r3,r2,00000000
	l.bf	0000C378
	l.addi	r4,r4,+0000001C

l0000C3D8:
	l.movhi	r3,00000001
	l.addi	r2,r0,+00000001
	l.ori	r3,r3,000030A4
	l.sw	4(r3),r2
	l.movhi	r2,00000001
	l.ori	r2,r2,00003954
	l.sw	0(r2),r3
	l.lwz	r3,24(r3)
	l.lwz	r2,0(r3)
	l.ori	r2,r2,00000080
	l.sw	0(r3),r2
	l.movhi	r3,00000000
	l.movhi	r2,00000001
	l.ori	r3,r3,0000C1DC
	l.jal	0000BAA8
	l.ori	r2,r2,000030A0
	l.addi	r3,r0,+00000000
	l.sw	0(r2),r3
	l.addi	r1,r1,+00000008
	l.ori	r11,r3,00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000C434             D7 E1 4F FC D7 E1 17 F8 9C 60 00 1C     ..O......`..
0000C440 9C 21 FF F8 9C 80 00 00 07 FF FD FC 9C 40 00 00 .!...........@..
0000C450 9C 60 00 1C 07 FF FB B3 9C 80 00 00 18 60 00 01 .`...........`..
0000C460 9C 80 00 01 A8 63 39 58 A9 62 00 00 D4 03 10 00 .....c9X.b......
0000C470 18 60 00 01 A8 63 30 A0 D4 03 20 00 9C 21 00 08 .`...c0... ..!..
0000C480 85 21 FF FC 44 00 48 00 84 41 FF F8             .!..D.H..A..   

;; fn0000C48C: 0000C48C
;;   Called from:
;;     0000E918 (in fn0000E904)
fn0000C48C proc
	l.sw	-20(r1),r2
	l.movhi	r2,00000001
	l.sw	-16(r1),r14
	l.ori	r2,r2,000030A0
	l.sw	-12(r1),r16
	l.sw	-8(r1),r18
	l.lwz	r14,0(r2)
	l.sw	-4(r1),r9
	l.addi	r2,r0,+00000000
	l.addi	r1,r1,-00000014
	l.ori	r18,r3,00000000
	l.sfne	r14,r2
	l.bf	0000C540
	l.ori	r16,r4,00000000

l0000C4C4:
	l.jal	0000E718
	l.movhi	r2,00000001
	l.ori	r2,r2,000030A4
	l.lwz	r5,4(r2)
	l.sfeqi	r5,00000000
	l.bf	0000C4F8
	l.slli	r3,r5,00000002

l0000C4E0:
	l.lwz	r3,32(r2)
	l.sfnei	r3,00000000
	l.bf	0000C51C
	l.ori	r2,r14,00000000

l0000C4F0:
	l.addi	r5,r0,+00000001
	l.slli	r3,r5,00000002

l0000C4F8:
	l.slli	r2,r5,00000005
	l.sub	r2,r2,r3
	l.movhi	r3,00000001
	l.ori	r3,r3,000030A4
	l.add	r2,r2,r3
	l.addi	r3,r0,+00000001
	l.sw	16(r2),r18
	l.sw	4(r2),r3
	l.sw	20(r2),r16

l0000C51C:
	l.jal	0000E740
	l.ori	r3,r11,00000000
	l.sfeqi	r2,00000000
	l.bf	0000C540
	l.movhi	r4,00000000

l0000C530:
	l.lwz	r3,8(r2)
	l.ori	r4,r4,0000C13C
	l.jal	00008BB8
	l.ori	r5,r2,00000000

l0000C540:
	l.addi	r1,r1,+00000014
	l.ori	r11,r2,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)
0000C560 D7 E1 17 FC 9C 21 FF FC 9C 40 00 00 9C 21 00 04 .....!...@...!..
0000C570 A9 62 00 00 D4 03 10 04 D4 03 10 10 D4 03 10 14 .b..............
0000C580 44 00 48 00 84 41 FF FC                         D.H..A..       

;; fn0000C588: 0000C588
;;   Called from:
;;     0000E950 (in fn0000E904)
fn0000C588 proc
	l.sw	-12(r1),r2
	l.ori	r2,r3,00000000
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,000030A0
	l.sw	-8(r1),r14
	l.lwz	r3,0(r3)
	l.addi	r1,r1,-0000000C
	l.sfnei	r3,00000000
	l.bf	0000C648
	l.addi	r11,r0,-0000000D

l0000C5B4:
	l.lwz	r3,12(r2)
	l.lwz	r6,24(r2)
	l.mul	r4,r4,r3
	l.sw	4(r6),r4
	l.lwz	r3,0(r6)
	l.ori	r3,r3,00000002
	l.sw	0(r6),r3

l0000C5D0:
	l.lwz	r14,0(r6)
	l.andi	r14,r14,00000002
	l.sfnei	r14,00000000
	l.bf	0000C5D0
	l.nop

l0000C5E4:
	l.lwz	r3,0(r6)
	l.addi	r4,r0,-00000081
	l.slli	r5,r5,00000007
	l.and	r3,r3,r4
	l.addi	r4,r0,+00000001
	l.sw	0(r6),r3
	l.lwz	r3,0(r6)
	l.or	r5,r5,r3
	l.movhi	r3,00000001
	l.sw	0(r6),r5
	l.lwz	r5,0(r2)
	l.ori	r3,r3,00003958
	l.sll	r4,r4,r5
	l.lwz	r3,0(r3)
	l.sw	4(r3),r4
	l.lwz	r5,0(r3)
	l.or	r4,r4,r5
	l.sw	0(r3),r4
	l.jal	00008B60
	l.lwz	r3,8(r2)
	l.lwz	r2,24(r2)
	l.ori	r11,r14,00000000
	l.lwz	r3,0(r2)
	l.ori	r3,r3,00000001
	l.sw	0(r2),r3

l0000C648:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)
0000C65C                                     D7 E1 17 EC             ....
0000C660 A8 43 00 00 18 60 00 01 D7 E1 87 F4 A8 63 30 A0 .C...`.......c0.
0000C670 D7 E1 4F FC D7 E1 77 F0 D7 E1 97 F8 86 03 00 00 ..O...w.........
0000C680 9C 21 FF EC BC 30 00 00 10 00 00 18 9D 60 FF F3 .!...0.......`..
0000C690 84 62 00 18 9C A0 FF FE 84 83 00 00 19 C0 00 01 .b..............
0000C6A0 E0 84 28 03 9E 40 00 01 D4 03 20 00 84 82 00 00 ..(..@.... .....
0000C6B0 A9 CE 39 58 E0 92 20 08 84 6E 00 00 AC 84 FF FF ..9X.. ..n......
0000C6C0 84 A3 00 00 E0 84 28 03 D4 03 20 00 07 FF F1 2B ......(... ....+
0000C6D0 84 62 00 08 84 42 00 00 84 6E 00 00 E2 52 10 08 .b...B...n...R..
0000C6E0 A9 70 00 00 D4 03 90 04 9C 21 00 14 85 21 FF FC .p.......!...!..
0000C6F0 84 41 FF EC 85 C1 FF F0 86 01 FF F4 44 00 48 00 .A..........D.H.
0000C700 86 41 FF F8 18 80 00 01 D7 E1 17 FC A8 84 30 A0 .A............0.
0000C710 9C 21 FF FC 84 A4 00 00 BC 25 00 00 10 00 00 10 .!.......%......
0000C720 9D 60 FF F3 18 C0 00 01 A8 C6 34 BC 84 86 00 00 .`........4.....
0000C730 BC 44 00 01 10 00 00 0A 9D 60 FF EA 18 40 00 01 .D.......`...@..
0000C740 B8 E4 00 02 A8 42 39 4C 9C 84 00 01 E0 E7 10 00 .....B9L........
0000C750 D4 06 20 00 D4 07 18 00 A9 65 00 00 9C 21 00 04 .. ......e...!..
0000C760 44 00 48 00 84 41 FF FC                         D.H..A..       

;; fn0000C768: 0000C768
;;   Called from:
;;     00004938 (in fn0000490C)
;;     00004AA8 (in fn00004A6C)
;;     0000846C (in fn00008428)
;;     000084EC (in fn00008428)
;;     000085A8 (in fn00008574)
;;     00008618 (in fn00008574)
;;     00009BB4 (in fn00009B80)
;;     00009F6C (in fn00009ED0)
;;     0000A7B4 (in fn0000A708)
;;     0000A7DC (in fn0000A708)
;;     0000A7F4 (in fn0000A708)
;;     0000A958 (in fn0000A92C)
;;     0000A988 (in fn0000A92C)
;;     0000A99C (in fn0000A92C)
;;     0000AA50 (in fn0000A92C)
;;     0000AC74 (in fn0000AC14)
;;     0000BB94 (in fn0000BB60)
;;     0000BBBC (in fn0000BB60)
;;     0000BEEC (in fn0000BE00)
;;     0000E260 (in fn0000E1AC)
;;     0000E340 (in fn0000E1AC)
;;     000100D4 (in fn000100B8)
;;     000100F4 (in fn000100B8)
;;     0001011C (in fn000100B8)
;;     00010130 (in fn000100B8)
;;     00010740 (in fn00010570)
;;     00011240 (in fn00010570)
;;     000112C0 (in fn00010570)
;;     000113C4 (in fn00010570)
;;     000116A4 (in fn00010570)
;;     00011790 (in fn00010570)
;;     000117B4 (in fn00010570)
;;     000117EC (in fn00010570)
;;     00011858 (in fn00010570)
;;     000118D8 (in fn00010570)
;;     00011918 (in fn00010570)
;;     0001193C (in fn00010570)
;;     00011998 (in fn00010570)
;;     000120C0 (in fn00010570)
fn0000C768 proc
	l.movhi	r4,00000001
	l.sw	-4(r1),r2
	l.ori	r4,r4,000030A0
	l.addi	r1,r1,-00000004
	l.lwz	r4,0(r4)
	l.sfnei	r4,00000000
	l.bf	0000C814
	l.sfeqi	r3,00000000

l0000C788:
	l.bf	0000C814
	l.movhi	r4,00000001

l0000C790:
	l.ori	r4,r4,00003954
	l.lwz	r5,0(r4)
	l.lwz	r6,12(r5)
	l.lwz	r4,24(r5)
	l.mul	r3,r3,r6
	l.sw	4(r4),r3
	l.lwz	r3,0(r4)
	l.ori	r3,r3,00000002
	l.sw	0(r4),r3

l0000C7B4:
	l.lwz	r3,0(r4)
	l.andi	r3,r3,00000002
	l.sfnei	r3,00000000
	l.bf	0000C7B4
	l.movhi	r3,00000001

l0000C7C8:
	l.lwz	r5,0(r5)
	l.addi	r6,r0,+00000001
	l.ori	r3,r3,00003958
	l.sll	r5,r6,r5
	l.lwz	r3,0(r3)
	l.sw	4(r3),r5
	l.lwz	r6,0(r4)
	l.ori	r6,r6,00000001
	l.sw	0(r4),r6

l0000C7EC:
	l.lwz	r6,4(r3)
	l.and	r6,r5,r6
	l.sfeqi	r6,00000000
	l.bf	0000C7EC
	l.nop

l0000C800:
	l.lwz	r6,0(r4)
	l.addi	r2,r0,-00000002
	l.and	r6,r6,r2
	l.sw	0(r4),r6
	l.sw	4(r3),r5

l0000C814:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

l0000C820:
	l.sw	-12(r1),r2
	l.sw	-4(r1),r9
	l.sw	-8(r1),r14
	l.ori	r2,r3,00000000
	l.sfeqi	r3,00000000
	l.bf	0000C88C
	l.addi	r1,r1,-0000000C

l0000C83C:
	l.jal	000046C0
	l.addi	r14,r0,+00000001
	l.add	r3,r2,r2
	l.add	r2,r3,r2
	l.slli	r2,r2,00000003
	l.add	r2,r12,r2
	l.sfltu	r2,r12
	l.bf	0000C868
	l.add	r14,r14,r11

l0000C860:
	l.addi	r14,r0,+00000000
	l.add	r14,r14,r11

l0000C868:
	l.jal	000046C0
	l.nop
	l.sfgtu	r14,r11
	l.bf	0000C868
	l.sfne	r14,r11

l0000C87C:
	l.bf	0000C88C
	l.sfgtu	r2,r12

l0000C884:
	l.bf	0000C868
	l.nop

l0000C88C:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn0000C8A0: 0000C8A0
;;   Called from:
;;     00004410 (in fn000043D0)
;;     00004420 (in fn000043D0)
;;     00004430 (in fn000043D0)
;;     00004440 (in fn000043D0)
;;     00004450 (in fn000043D0)
;;     00004460 (in fn000043D0)
;;     000044A4 (in fn000043D0)
;;     00004BE8 (in fn00004B44)
;;     00004CD0 (in fn00004C48)
;;     00004FDC (in fn00004D60)
;;     00006378 (in fn000062BC)
;;     000064B0 (in fn00006394)
;;     00006590 (in fn000064DC)
;;     000065B8 (in fn000064DC)
;;     000065C8 (in fn000064DC)
;;     00006634 (in fn000064DC)
;;     00006644 (in fn000064DC)
;;     00006AD8 (in fn000073EC)
;;     00006B44 (in fn000073EC)
;;     00006B64 (in fn000073EC)
;;     00006BA4 (in fn000073EC)
;;     00006BEC (in fn000073EC)
;;     00006C20 (in fn000073EC)
;;     00006C64 (in fn000073EC)
;;     00006CF8 (in fn000073EC)
;;     00006D3C (in fn000073EC)
;;     00006DA8 (in fn000073EC)
;;     00006DC4 (in fn000073EC)
;;     00006DE0 (in fn000073EC)
;;     00007034 (in fn00006FE8)
;;     00007100 (in fn00006FE8)
;;     00007138 (in fn00006FE8)
;;     000071C4 (in fn00006FE8)
;;     000072B8 (in fn00006FE8)
;;     000072F0 (in fn00006FE8)
;;     00007E24 (in fn00007E04)
;;     00007E58 (in fn00007E04)
;;     00007E74 (in fn00007E04)
;;     00007EEC (in fn00007E04)
;;     00007F8C (in fn00007E04)
;;     00007FE8 (in fn00007FD0)
;;     00008018 (in fn00008000)
;;     00008068 (in fn00008030)
;;     00009DB8 (in fn00009C2C)
;;     0000D5D4 (in fn0000D49C)
;;     0000F658 (in fn0000F444)
;;     00011F80 (in fn00010570)
;;     00011F9C (in fn00010570)
;;     00011FB0 (in fn00010570)
;;     00011FCC (in fn00010570)
;;     00011FE4 (in fn00010570)
fn0000C8A0 proc
	l.sw	-8(r1),r2
	l.sw	-4(r1),r9
	l.ori	r2,r3,00000000
	l.sfeqi	r3,00000000
	l.bf	0000C914
	l.addi	r1,r1,-00000008

l0000C8B8:
	l.jal	0000BC28
	l.nop
	l.sfeqi	r11,00000000
	l.bf	0000C904
	l.ori	r3,r2,00000000

l0000C8CC:
	l.jal	0000BF88
	l.addi	r3,r0,+00000011
	l.mul	r2,r11,r2
	l.movhi	r4,0000000F
	l.ori	r4,r4,0000423F
	l.add	r3,r2,r4
	l.movhi	r4,0000000F
	l.jal	0000FEDC
	l.ori	r4,r4,00004240
	l.addi	r1,r1,+00000008
	l.ori	r3,r11,00000000
	l.lwz	r9,-4(r1)
	l.j	0000DB28
	l.lwz	r2,-8(r1)

l0000C904:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.j	0000C820
	l.lwz	r2,-8(r1)

l0000C914:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000C924             44 00 48 00 9D 60 00 00 44 00 48 00     D.H..`..D.H.
0000C930 9D 60 00 00 44 00 48 00 9D 60 00 00             .`..D.H..`..   

;; fn0000C93C: 0000C93C
;;   Called from:
;;     0000CDE4 (in fn0000CCA8)
;;     0000CE4C (in fn0000CCA8)
;;     0000CEC4 (in fn0000CCA8)
;;     0000CF54 (in fn0000CCA8)
;;     0000CFC4 (in fn0000CCA8)
;;     0000D044 (in fn0000CCA8)
;;     0000D12C (in fn0000CCA8)
fn0000C93C proc
	l.movhi	r3,000001F0
	l.ori	r3,r3,0000240C
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000008
	l.sw	0(r3),r4
	l.lwz	r4,0(r3)

l0000C954:
	l.lwz	r4,0(r3)
	l.andi	r4,r4,00000008
	l.sfnei	r4,00000000
	l.bf	0000C954
	l.nop

l0000C968:
	l.jr	r9
	l.nop

;; fn0000C970: 0000C970
;;   Called from:
;;     0000CC08 (in fn0000CB40)
;;     0000D1A4 (in fn0000CCA8)
fn0000C970 proc
	l.movhi	r3,000001F0
	l.addi	r4,r0,+00000001
	l.ori	r3,r3,00002418
	l.sw	0(r3),r4

l0000C980:
	l.lwz	r4,0(r3)
	l.sfnei	r4,00000000
	l.bf	0000C980
	l.nop

l0000C990:
	l.jr	r9
	l.nop

;; fn0000C998: 0000C998
;;   Called from:
;;     0000CD70 (in fn0000CCA8)
;;     0000CF4C (in fn0000CCA8)
fn0000C998 proc
	l.movhi	r3,000001F0
	l.sw	-4(r1),r2
	l.ori	r3,r3,0000240C
	l.addi	r2,r0,-00000009
	l.lwz	r4,0(r3)
	l.addi	r1,r1,-00000004
	l.ori	r4,r4,00000020
	l.and	r4,r4,r2
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000C9C8: 0000C9C8
;;   Called from:
;;     0000CBF4 (in fn0000CB40)
fn0000C9C8 proc
	l.sw	-4(r1),r9
	l.sw	-16(r1),r18
	l.sw	-12(r1),r20
	l.sw	-28(r1),r2
	l.sw	-24(r1),r14
	l.sw	-20(r1),r16
	l.sw	-8(r1),r22
	l.addi	r3,r0,+00000019
	l.jal	0000BF88
	l.addi	r1,r1,-0000001C
	l.addi	r4,r0,+0000000A
	l.jal	0000FFD8
	l.ori	r3,r11,00000000
	l.movhi	r4,00000006
	l.ori	r3,r11,00000000
	l.ori	r4,r4,00001A80
	l.jal	0000FEDC
	l.ori	r20,r11,00000000
	l.sfeqi	r11,00000000
	l.bf	0000CA94
	l.ori	r18,r11,00000000

l0000CA1C:
	l.addi	r2,r0,+00000000
	l.addi	r16,r0,+00000001

l0000CA24:
	l.ori	r3,r18,00000000
	l.jal	0000FEDC
	l.ori	r4,r16,00000000
	l.addi	r11,r11,-00000001
	l.j	0000CA6C
	l.andi	r14,r11,000000FF

l0000CA3C:
	l.ori	r3,r20,00000000
	l.jal	0000FEDC
	l.ori	r4,r22,00000000
	l.ori	r4,r16,00000000
	l.jal	0000FEDC
	l.ori	r3,r11,00000000
	l.movhi	r3,00000006
	l.ori	r3,r3,00001A80
	l.sfleu	r11,r3
	l.bf	0000CAA0
	l.slli	r14,r14,00000003

l0000CA68:
	l.andi	r14,r22,000000FF

l0000CA6C:
	l.sfleui	r14,0000000F
	l.bf	0000CA3C
	l.addi	r22,r14,+00000001

l0000CA78:
	l.addi	r2,r2,+00000001
	l.andi	r2,r2,000000FF
	l.sfeqi	r2,00000008
	l.bf	0000CA9C
	l.add	r16,r16,r16

l0000CA8C:
	l.j	0000CA24
	l.andi	r16,r16,000000FF

l0000CA94:
	l.ori	r2,r11,00000000
	l.addi	r14,r0,+00000001

l0000CA9C:
	l.slli	r14,r14,00000003

l0000CAA0:
	l.movhi	r3,000001F0
	l.or	r2,r14,r2
	l.ori	r3,r3,00002414
	l.sw	0(r3),r2
	l.addi	r1,r1,+0000001C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-28(r1)
	l.lwz	r14,-24(r1)
	l.lwz	r16,-20(r1)
	l.lwz	r18,-16(r1)
	l.lwz	r20,-12(r1)
	l.jr	r9
	l.lwz	r22,-8(r1)
0000CAD4             D7 E1 4F FC D7 E1 17 F8 BC 03 00 00     ..O.........
0000CAE0 10 00 00 07 9C 21 FF F8 BC 03 00 01 0C 00 00 11 .....!..........
0000CAF0 9D 60 FF DC 00 00 00 09 15 00 00 00 18 80 00 01 .`..............
0000CB00 9C 40 00 01 A8 84 34 C0 A9 63 00 00 D8 04 10 00 .@....4..c......
0000CB10 00 00 00 09 9C 21 00 08 07 FF FF AC 9C 40 00 00 .....!.......@..
0000CB20 18 60 00 01 A8 63 34 C0 9D 60 00 00 D8 03 10 00 .`...c4..`......
0000CB30 9C 21 00 08 85 21 FF FC 44 00 48 00 84 41 FF F8 .!...!..D.H..A..

;; fn0000CB40: 0000CB40
;;   Called from:
;;     0000EA14 (in fn0000E98C)
fn0000CB40 proc
	l.sw	-8(r1),r2
	l.movhi	r2,00000000
	l.sw	-4(r1),r9
	l.ori	r2,r2,00004000
	l.lwz	r2,32(r2)
	l.sfeqi	r2,00000000
	l.bf	0000CB78
	l.addi	r1,r1,-00000008

l0000CB60:
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000001
	l.ori	r2,r2,000034C0
	l.sb	0(r2),r3
	l.j	0000CC90
	l.addi	r1,r1,+00000008

l0000CB78:
	l.ori	r4,r2,00000000
	l.addi	r3,r0,+00000001
	l.jal	00009744
	l.addi	r5,r0,+00000002
	l.ori	r4,r2,00000000
	l.ori	r5,r2,00000000
	l.jal	000097BC
	l.addi	r3,r0,+00000001
	l.ori	r4,r2,00000000
	l.addi	r3,r0,+00000001
	l.jal	00009834
	l.addi	r5,r0,+00000002
	l.addi	r3,r0,+00000001
	l.addi	r5,r0,+00000002
	l.jal	00009744
	l.ori	r4,r3,00000000
	l.addi	r3,r0,+00000001
	l.ori	r5,r2,00000000
	l.ori	r4,r3,00000000
	l.jal	000097BC
	l.movhi	r2,000001F0
	l.addi	r3,r0,+00000001
	l.addi	r5,r0,+00000002
	l.jal	00009834
	l.ori	r4,r3,00000000
	l.addi	r3,r0,+00000019
	l.jal	0000B320
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+00000019
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.jal	0000C9C8
	l.nop
	l.ori	r3,r2,0000240C
	l.addi	r4,r0,+00000044
	l.sw	0(r3),r4
	l.jal	0000C970
	l.nop
	l.ori	r3,r2,00002420
	l.lwz	r2,0(r3)
	l.sfeqi	r2,0000003A
	l.bf	0000CC60
	l.addi	r4,r0,+0000000A

l0000CC24:
	l.j	0000CC30
	l.addi	r2,r0,+0000000B

l0000CC2C:
	l.sw	0(r3),r4

l0000CC30:
	l.addi	r2,r2,-00000001
	l.sfnei	r2,00000000
	l.bf	0000CC2C
	l.nop

l0000CC40:
	l.movhi	r2,000001F0
	l.addi	r4,r0,+0000000A
	l.ori	r3,r2,00002420
	l.ori	r2,r2,00002418
	l.sw	0(r3),r4
	l.lwz	r3,0(r3)
	l.addi	r3,r0,+00000001
	l.sw	0(r2),r3

l0000CC60:
	l.movhi	r4,00000000
	l.addi	r3,r0,+00000019
	l.ori	r4,r4,0000CAD4
	l.jal	0000B4D4
	l.movhi	r2,00000001
	l.ori	r2,r2,000034C0
	l.addi	r3,r0,+00000000
	l.sb	0(r2),r3
	l.movhi	r2,000001F0
	l.ori	r2,r2,00002420
	l.lwz	r2,0(r2)
	l.addi	r1,r1,+00000008

l0000CC90:
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000CCA0 44 00 48 00 9D 60 00 00                         D.H..`..       

;; fn0000CCA8: 0000CCA8
;;   Called from:
;;     0000D25C (in fn0000D214)
;;     0000D2E0 (in fn0000D298)
fn0000CCA8 proc
	l.sw	-40(r1),r2
	l.movhi	r2,00000001
	l.sw	-36(r1),r14
	l.sw	-32(r1),r16
	l.sw	-20(r1),r22
	l.sw	-12(r1),r26
	l.sw	-8(r1),r28
	l.sw	-4(r1),r9
	l.sw	-28(r1),r18
	l.sw	-24(r1),r20
	l.sw	-16(r1),r24
	l.ori	r2,r2,000034C0
	l.addi	r1,r1,-0000002C
	l.lbz	r20,0(r2)
	l.slli	r20,r20,00000018
	l.ori	r26,r3,00000000
	l.ori	r16,r6,00000000
	l.srai	r20,r20,00000018
	l.ori	r14,r7,00000000
	l.andi	r22,r4,000000FF
	l.andi	r28,r5,000000FF
	l.sfnei	r20,00000000
	l.bf	0000D1E0
	l.addi	r2,r0,-0000000D

l0000CD08:
	l.sfgtui	r7,00000004
	l.bf	0000D1E0
	l.addi	r2,r0,-00000016

l0000CD14:
	l.movhi	r2,000001F0
	l.addi	r4,r0,-00000009
	l.ori	r18,r2,0000240C
	l.lwz	r3,0(r18)
	l.ori	r3,r3,00000004
	l.and	r3,r3,r4
	l.sw	0(r18),r3
	l.lwz	r3,0(r18)
	l.lwz	r3,0(r18)
	l.jal	0000E718
	l.nop
	l.ori	r3,r2,0000241C
	l.ori	r2,r2,00002410
	l.sw	0(r3),r20
	l.ori	r24,r11,00000000
	l.lwz	r2,0(r2)
	l.andi	r2,r2,000000FF
	l.sw	0(r1),r2
	l.addi	r2,r0,-00000001
	l.lwz	r3,0(r1)
	l.sfnei	r3,000000F8
	l.bf	0000D0F0
	l.movhi	r4,000001F0

l0000CD70:
	l.jal	0000C998
	l.nop
	l.addi	r6,r0,+000007FF

l0000CD7C:
	l.lwz	r2,0(r18)
	l.andi	r2,r2,00000008
	l.sfnei	r2,00000000
	l.bf	0000CDA8
	l.movhi	r20,000001F0

l0000CD90:
	l.addi	r6,r6,-00000001
	l.sfnei	r6,00000000
	l.bf	0000CD7C
	l.addi	r2,r0,-00000001

l0000CDA0:
	l.j	0000D0F0
	l.movhi	r4,000001F0

l0000CDA8:
	l.sfeqi	r6,00000000
	l.bf	0000D0EC
	l.addi	r2,r0,-00000001

l0000CDB4:
	l.ori	r3,r20,00002410
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sfnei	r3,00000008
	l.bf	0000D0F0
	l.movhi	r4,000001F0

l0000CDD0:
	l.add	r22,r22,r22
	l.ori	r2,r20,00002408
	l.andi	r3,r22,000000FF
	l.ori	r20,r20,0000240C
	l.sw	0(r2),r3
	l.jal	0000C93C
	l.nop
	l.addi	r6,r0,+000007FF

l0000CDF0:
	l.lwz	r2,0(r20)
	l.andi	r2,r2,00000008
	l.sfnei	r2,00000000
	l.bf	0000CE1C
	l.movhi	r18,000001F0

l0000CE04:
	l.addi	r6,r6,-00000001
	l.sfnei	r6,00000000
	l.bf	0000CDF0
	l.addi	r2,r0,-00000001

l0000CE14:
	l.j	0000D0F0
	l.movhi	r4,000001F0

l0000CE1C:
	l.sfeqi	r6,00000000
	l.bf	0000D0EC
	l.addi	r2,r0,-00000001

l0000CE28:
	l.ori	r3,r18,00002410
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sfnei	r3,00000018
	l.bf	0000D0F0
	l.movhi	r4,000001F0

l0000CE44:
	l.ori	r2,r18,00002408
	l.sw	0(r2),r28
	l.jal	0000C93C
	l.ori	r2,r18,0000240C
	l.addi	r5,r0,+000007FF

l0000CE58:
	l.lwz	r3,0(r2)
	l.andi	r3,r3,00000008
	l.sfnei	r3,00000000
	l.bf	0000CE84
	l.movhi	r18,000001F0

l0000CE6C:
	l.addi	r5,r5,-00000001
	l.sfnei	r5,00000000
	l.bf	0000CE58
	l.nop

l0000CE7C:
	l.j	0000D0EC
	l.addi	r2,r0,-00000001

l0000CE84:
	l.sfeqi	r5,00000000
	l.bf	0000D0EC
	l.addi	r2,r0,-00000001

l0000CE90:
	l.ori	r4,r18,00002410
	l.lwz	r3,0(r4)
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sfnei	r3,00000028
	l.bf	0000D0EC
	l.sfnei	r26,00000000

l0000CEAC:
	l.bnf	0000CF30
	l.ori	r20,r4,00000000

l0000CEB4:
	l.j	0000CF4C
	l.nop

l0000CEBC:
	l.lbz	r2,0(r16)
	l.sw	0(r22),r2
	l.jal	0000C93C
	l.addi	r2,r0,+000007FF

l0000CECC:
	l.lwz	r3,0(r18)
	l.andi	r3,r3,00000008
	l.sfeqi	r3,00000000
	l.bf	0000CEF8
	l.addi	r2,r2,-00000001

l0000CEE0:
	l.addi	r2,r2,+00000001
	l.sfeqi	r2,FFFFFFFF
	l.bnf	0000CF0C
	l.movhi	r4,000001F0

l0000CEF0:
	l.j	0000D0F4
	l.addi	r5,r0,-00000009

l0000CEF8:
	l.sfnei	r2,FFFFFFFF
	l.bf	0000CECC
	l.movhi	r4,000001F0

l0000CF04:
	l.j	0000D0F4
	l.addi	r5,r0,-00000009

l0000CF0C:
	l.lwz	r2,0(r20)
	l.addi	r14,r14,-00000001
	l.sw	0(r1),r2
	l.lwz	r2,0(r1)
	l.sfnei	r2,00000028
	l.bf	0000D0E8
	l.addi	r16,r16,+00000001

l0000CF28:
	l.j	0000CF3C
	l.sfnei	r14,00000000

l0000CF30:
	l.ori	r22,r18,00002408
	l.ori	r18,r18,0000240C
	l.sfnei	r14,00000000

l0000CF3C:
	l.bf	0000CEBC
	l.nop

l0000CF44:
	l.j	0000D0EC
	l.ori	r2,r14,00000000

l0000CF4C:
	l.jal	0000C998
	l.ori	r18,r18,0000240C
	l.jal	0000C93C
	l.nop
	l.addi	r5,r0,+000007FF

l0000CF60:
	l.lwz	r2,0(r18)
	l.andi	r2,r2,00000008
	l.sfnei	r2,00000000
	l.bf	0000CF8C
	l.movhi	r20,000001F0

l0000CF74:
	l.addi	r5,r5,-00000001
	l.sfnei	r5,00000000
	l.bf	0000CF60
	l.addi	r2,r0,-00000001

l0000CF84:
	l.j	0000D0F0
	l.movhi	r4,000001F0

l0000CF8C:
	l.sfeqi	r5,00000000
	l.bf	0000D0EC
	l.addi	r2,r0,-00000001

l0000CF98:
	l.ori	r3,r20,00002410
	l.lwz	r3,0(r3)
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sfnei	r3,00000010
	l.bf	0000D0F0
	l.movhi	r4,000001F0

l0000CFB4:
	l.ori	r2,r20,00002408
	l.ori	r22,r22,00000001
	l.ori	r20,r20,0000240C
	l.sw	0(r2),r22
	l.jal	0000C93C
	l.nop
	l.addi	r4,r0,+000007FF

l0000CFD0:
	l.lwz	r2,0(r20)
	l.andi	r2,r2,00000008
	l.sfnei	r2,00000000
	l.bf	0000CFFC
	l.movhi	r5,000001F0

l0000CFE4:
	l.addi	r4,r4,-00000001
	l.sfnei	r4,00000000
	l.bf	0000CFD0
	l.addi	r2,r0,-00000001

l0000CFF4:
	l.j	0000D0F0
	l.movhi	r4,000001F0

l0000CFFC:
	l.sfeqi	r4,00000000
	l.bf	0000D0EC
	l.addi	r2,r0,-00000001

l0000D008:
	l.ori	r4,r5,00002410
	l.lwz	r3,0(r4)
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sfnei	r3,00000040
	l.bf	0000D0EC
	l.ori	r18,r5,0000240C

l0000D024:
	l.j	0000D0CC
	l.ori	r22,r5,00002408

l0000D02C:
	l.sfnei	r14,00000000
	l.bf	0000D044
	l.nop

l0000D038:
	l.lwz	r2,0(r18)
	l.andi	r2,r2,000000F3
	l.sw	0(r18),r2

l0000D044:
	l.jal	0000C93C
	l.addi	r2,r0,+000007FF

l0000D04C:
	l.lwz	r3,0(r18)
	l.andi	r3,r3,00000008
	l.sfeqi	r3,00000000
	l.bf	0000D078
	l.addi	r2,r2,-00000001

l0000D060:
	l.addi	r2,r2,+00000001
	l.sfeqi	r2,FFFFFFFF
	l.bnf	0000D08C
	l.movhi	r4,000001F0

l0000D070:
	l.j	0000D0F4
	l.addi	r5,r0,-00000009

l0000D078:
	l.sfnei	r2,FFFFFFFF
	l.bf	0000D04C
	l.movhi	r4,000001F0

l0000D084:
	l.j	0000D0F4
	l.addi	r5,r0,-00000009

l0000D08C:
	l.lwz	r2,0(r22)
	l.sfeqi	r14,00000000
	l.sb	0(r16),r2
	l.lwz	r2,0(r20)
	l.sw	0(r1),r2
	l.bf	0000D0B4
	l.nop

l0000D0A8:
	l.lwz	r2,0(r1)
	l.j	0000D0BC
	l.sfnei	r2,00000050

l0000D0B4:
	l.lwz	r2,0(r1)
	l.sfnei	r2,00000058

l0000D0BC:
	l.bf	0000D0E8
	l.addi	r16,r16,+00000001

l0000D0C4:
	l.j	0000D0D4
	l.sfnei	r14,00000000

l0000D0CC:
	l.ori	r20,r4,00000000
	l.sfnei	r14,00000000

l0000D0D4:
	l.bf	0000D02C
	l.addi	r14,r14,-00000001

l0000D0DC:
	l.addi	r14,r14,+00000001
	l.j	0000D0EC
	l.ori	r2,r14,00000000

l0000D0E8:
	l.addi	r2,r0,-00000001

l0000D0EC:
	l.movhi	r4,000001F0

l0000D0F0:
	l.addi	r5,r0,-00000009

l0000D0F4:
	l.ori	r4,r4,0000240C
	l.lwz	r3,0(r4)
	l.ori	r3,r3,00000010
	l.and	r3,r3,r5
	l.addi	r5,r0,+000007FF
	l.sw	0(r4),r3

l0000D10C:
	l.lwz	r3,0(r4)
	l.andi	r3,r3,00000010
	l.sfeqi	r3,00000000
	l.bf	0000D12C
	l.addi	r5,r5,-00000001

l0000D120:
	l.sfnei	r5,00000000
	l.bf	0000D10C
	l.nop

l0000D12C:
	l.jal	0000C93C
	l.nop
	l.movhi	r4,000001F0
	l.addi	r5,r0,+000007FF
	l.ori	r4,r4,0000240C
	l.lwz	r3,0(r4)
	l.lwz	r3,0(r4)

l0000D148:
	l.lwz	r3,0(r4)
	l.andi	r3,r3,00000010
	l.sfeqi	r3,00000000
	l.bf	0000D168
	l.addi	r5,r5,-00000001

l0000D15C:
	l.sfnei	r5,00000000
	l.bf	0000D148
	l.nop

l0000D168:
	l.movhi	r3,000001F0
	l.addi	r4,r0,+000007FF
	l.ori	r3,r3,00002410

l0000D174:
	l.lwz	r5,0(r3)
	l.sfeqi	r5,000000F8
	l.bf	0000D19C
	l.sfnei	r4,00000000

l0000D184:
	l.addi	r4,r4,-00000001
	l.sfnei	r4,00000000
	l.bf	0000D174
	l.nop

l0000D194:
	l.j	0000D1A4
	l.nop

l0000D19C:
	l.bf	0000D1B4
	l.addi	r5,r0,+000007FF

l0000D1A4:
	l.jal	0000C970
	l.nop
	l.j	0000D1D8
	l.nop

l0000D1B4:
	l.movhi	r3,000001F0
	l.ori	r3,r3,00002420

l0000D1BC:
	l.lwz	r4,0(r3)
	l.sfeqi	r4,0000003A
	l.bf	0000D1D8
	l.addi	r5,r5,-00000001

l0000D1CC:
	l.sfnei	r5,00000000
	l.bf	0000D1BC
	l.nop

l0000D1D8:
	l.jal	0000E740
	l.ori	r3,r24,00000000

l0000D1E0:
	l.addi	r1,r1,+0000002C
	l.ori	r11,r2,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-40(r1)
	l.lwz	r14,-36(r1)
	l.lwz	r16,-32(r1)
	l.lwz	r18,-28(r1)
	l.lwz	r20,-24(r1)
	l.lwz	r22,-20(r1)
	l.lwz	r24,-16(r1)
	l.lwz	r26,-12(r1)
	l.jr	r9
	l.lwz	r28,-8(r1)

;; fn0000D214: 0000D214
;;   Called from:
;;     000099F4 (in fn0000998C)
;;     00009A38 (in fn0000998C)
;;     00009D50 (in fn00009C2C)
;;     00009E64 (in fn00009DDC)
;;     00009F2C (in fn00009ED0)
;;     0000A0B0 (in fn0000A070)
;;     0000A268 (in fn0000A22C)
;;     0000A3D4 (in fn0000A364)
;;     0000A748 (in fn0000A708)
;;     0000A9C8 (in fn0000A92C)
;;     0000A9F8 (in fn0000A92C)
;;     0000AB30 (in fn0000AADC)
;;     0000D38C (in fn0000D31C)
;;     0000D584 (in fn0000D49C)
fn0000D214 proc
	l.sw	-20(r1),r14
	l.addi	r14,r0,+00000000
	l.sw	-24(r1),r2
	l.sw	-16(r1),r16
	l.sw	-12(r1),r18
	l.sw	-8(r1),r20
	l.sw	-4(r1),r9
	l.ori	r20,r3,00000000
	l.addi	r1,r1,-00000018
	l.ori	r18,r4,00000000
	l.ori	r16,r5,00000000
	l.j	0000D268
	l.ori	r2,r14,00000000

l0000D248:
	l.add	r5,r20,r2
	l.addi	r3,r0,+00000001
	l.lbz	r5,0(r5)
	l.addi	r4,r0,+00000036
	l.ori	r7,r3,00000000
	l.jal	0000CCA8
	l.addi	r2,r2,+00000001
	l.or	r14,r14,r11

l0000D268:
	l.sfltu	r2,r16
	l.bf	0000D248
	l.add	r6,r18,r2

l0000D274:
	l.addi	r1,r1,+00000018
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-24(r1)
	l.lwz	r14,-20(r1)
	l.lwz	r16,-16(r1)
	l.lwz	r18,-12(r1)
	l.jr	r9
	l.lwz	r20,-8(r1)

;; fn0000D298: 0000D298
;;   Called from:
;;     00009A10 (in fn0000998C)
;;     00009A54 (in fn0000998C)
;;     00009D64 (in fn00009C2C)
;;     00009F58 (in fn00009ED0)
;;     0000A34C (in fn0000A300)
;;     0000A3A4 (in fn0000A364)
;;     0000A5B0 (in fn0000A364)
;;     0000A764 (in fn0000A708)
;;     0000A9E0 (in fn0000A92C)
;;     0000AA10 (in fn0000A92C)
;;     0000AB50 (in fn0000AADC)
;;     0000D484 (in fn0000D408)
;;     0000D5B8 (in fn0000D49C)
fn0000D298 proc
	l.sw	-20(r1),r14
	l.addi	r14,r0,+00000000
	l.sw	-24(r1),r2
	l.sw	-16(r1),r16
	l.sw	-12(r1),r18
	l.sw	-8(r1),r20
	l.sw	-4(r1),r9
	l.ori	r20,r3,00000000
	l.addi	r1,r1,-00000018
	l.ori	r18,r4,00000000
	l.ori	r16,r5,00000000
	l.j	0000D2EC
	l.ori	r2,r14,00000000

l0000D2CC:
	l.add	r5,r20,r2
	l.addi	r3,r0,+00000000
	l.lbz	r5,0(r5)
	l.addi	r4,r0,+00000036
	l.addi	r7,r0,+00000001
	l.jal	0000CCA8
	l.addi	r2,r2,+00000001
	l.or	r14,r14,r11

l0000D2EC:
	l.sfltu	r2,r16
	l.bf	0000D2CC
	l.add	r6,r18,r2

l0000D2F8:
	l.addi	r1,r1,+00000018
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-24(r1)
	l.lwz	r14,-20(r1)
	l.lwz	r16,-16(r1)
	l.lwz	r18,-12(r1)
	l.jr	r9
	l.lwz	r20,-8(r1)

;; fn0000D31C: 0000D31C
;;   Called from:
;;     0000F76C (in fn0000F444)
fn0000D31C proc
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-4(r1),r9
	l.sw	-8(r1),r16
	l.ori	r2,r3,00000000
	l.addi	r1,r1,-00000020
	l.lwz	r14,40(r3)
	l.addi	r3,r0,+00000000
	l.addi	r6,r1,+00000008
	l.j	0000D378
	l.ori	r4,r3,00000000

l0000D348:
	l.bf	0000D35C
	l.nop

l0000D350:
	l.lwz	r5,44(r2)
	l.j	0000D368
	l.srl	r5,r5,r3

l0000D35C:
	l.addi	r5,r3,-00000020
	l.lwz	r7,48(r2)
	l.srl	r5,r7,r5

l0000D368:
	l.sb	0(r6),r5
	l.addi	r4,r4,+00000001
	l.addi	r3,r3,+00000008
	l.addi	r6,r6,+00000001

l0000D378:
	l.sfltu	r4,r14
	l.bf	0000D348
	l.sfgtui	r4,00000003

l0000D384:
	l.addi	r3,r1,+00000008
	l.ori	r4,r1,00000000
	l.jal	0000D214
	l.ori	r5,r14,00000000
	l.addi	r3,r0,+00000000
	l.ori	r4,r1,00000000
	l.j	0000D3E4
	l.ori	r6,r3,00000000

l0000D3A4:
	l.sfgtui	r6,00000003
	l.bf	0000D3C4
	l.lbz	r5,0(r4)

l0000D3B0:
	l.sll	r5,r5,r3
	l.lwz	r7,52(r2)
	l.or	r5,r7,r5
	l.j	0000D3D8
	l.sw	52(r2),r5

l0000D3C4:
	l.addi	r7,r3,-00000020
	l.sll	r5,r5,r7
	l.lwz	r7,56(r2)
	l.or	r5,r7,r5
	l.sw	56(r2),r5

l0000D3D8:
	l.addi	r6,r6,+00000001
	l.addi	r4,r4,+00000001
	l.addi	r3,r3,+00000008

l0000D3E4:
	l.sfltu	r6,r14
	l.bf	0000D3A4
	l.nop

l0000D3F0:
	l.addi	r1,r1,+00000020
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn0000D408: 0000D408
;;   Called from:
;;     0000F77C (in fn0000F444)
fn0000D408 proc
	l.sw	-4(r1),r9
	l.addi	r4,r0,+00000000
	l.addi	r1,r1,-00000014
	l.lwz	r5,40(r3)
	l.addi	r8,r1,+00000008
	l.ori	r7,r1,00000000
	l.j	0000D474
	l.ori	r6,r4,00000000

l0000D428:
	l.bf	0000D448
	l.nop

l0000D430:
	l.lwz	r11,44(r3)
	l.srl	r11,r11,r4
	l.sb	0(r8),r11
	l.lwz	r11,52(r3)
	l.j	0000D460
	l.srl	r11,r11,r4

l0000D448:
	l.addi	r11,r4,-00000020
	l.lwz	r12,48(r3)
	l.srl	r12,r12,r11
	l.sb	0(r8),r12
	l.lwz	r12,56(r3)
	l.srl	r11,r12,r11

l0000D460:
	l.sb	0(r7),r11
	l.addi	r6,r6,+00000001
	l.addi	r4,r4,+00000008
	l.addi	r8,r8,+00000001
	l.addi	r7,r7,+00000001

l0000D474:
	l.sfltu	r6,r5
	l.bf	0000D428
	l.sfgtui	r6,00000003

l0000D480:
	l.addi	r3,r1,+00000008
	l.jal	0000D298
	l.ori	r4,r1,00000000
	l.addi	r1,r1,+00000014
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.nop

;; fn0000D49C: 0000D49C
;;   Called from:
;;     0000F78C (in fn0000F444)
fn0000D49C proc
	l.sw	-44(r1),r2
	l.sw	-32(r1),r18
	l.sw	-8(r1),r30
	l.sw	-4(r1),r9
	l.sw	-40(r1),r14
	l.sw	-36(r1),r16
	l.sw	-28(r1),r20
	l.sw	-24(r1),r22
	l.sw	-20(r1),r24
	l.sw	-16(r1),r26
	l.sw	-12(r1),r28
	l.addi	r4,r0,+00000000
	l.addi	r1,r1,-0000004C
	l.lwz	r18,40(r3)
	l.lwz	r30,68(r3)
	l.addi	r8,r1,+00000018
	l.addi	r7,r1,+00000010
	l.addi	r6,r1,+00000008
	l.j	0000D554
	l.ori	r2,r4,00000000

l0000D4EC:
	l.bf	0000D518
	l.nop

l0000D4F4:
	l.lwz	r5,44(r3)
	l.srl	r5,r5,r4
	l.sb	0(r8),r5
	l.lwz	r5,52(r3)
	l.srl	r5,r5,r4
	l.sb	0(r7),r5
	l.lwz	r5,60(r3)
	l.j	0000D53C
	l.srl	r5,r5,r4

l0000D518:
	l.addi	r5,r4,-00000020
	l.lwz	r11,48(r3)
	l.srl	r11,r11,r5
	l.sb	0(r8),r11
	l.lwz	r11,56(r3)
	l.srl	r11,r11,r5
	l.sb	0(r7),r11
	l.lwz	r11,64(r3)
	l.srl	r5,r11,r5

l0000D53C:
	l.sb	0(r6),r5
	l.addi	r2,r2,+00000001
	l.addi	r4,r4,+00000008
	l.addi	r8,r8,+00000001
	l.addi	r7,r7,+00000001
	l.addi	r6,r6,+00000001

l0000D554:
	l.sfltu	r2,r18
	l.bf	0000D4EC
	l.sfgtui	r2,00000003

l0000D560:
	l.addi	r16,r0,+00000000
	l.ori	r2,r1,00000000
	l.ori	r14,r16,00000000
	l.addi	r20,r1,+00000018
	l.addi	r28,r1,+00000010
	l.addi	r24,r1,+00000008
	l.j	0000D5E4
	l.addi	r22,r0,+000003E8

l0000D580:
	l.add	r3,r20,r14
	l.jal	0000D214
	l.addi	r5,r0,+00000001
	l.sfnei	r30,00000000
	l.bf	0000D5B4
	l.ori	r3,r20,00000000

l0000D598:
	l.add	r3,r28,r14
	l.lbz	r4,0(r2)
	l.lbz	r3,0(r3)
	l.xori	r3,r3,000000FF
	l.and	r3,r3,r4
	l.sb	0(r2),r3
	l.ori	r3,r20,00000000

l0000D5B4:
	l.ori	r5,r18,00000000
	l.jal	0000D298
	l.ori	r4,r1,00000000
	l.add	r3,r24,r14
	l.lbz	r5,0(r3)
	l.sfeqi	r5,00000000
	l.bf	0000D5DC
	l.ori	r16,r11,00000000

l0000D5D4:
	l.jal	0000C8A0
	l.mul	r3,r5,r22

l0000D5DC:
	l.addi	r14,r14,+00000001
	l.addi	r2,r2,+00000001

l0000D5E4:
	l.sfltu	r14,r18
	l.bf	0000D580
	l.ori	r4,r2,00000000

l0000D5F0:
	l.addi	r1,r1,+0000004C
	l.ori	r11,r16,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-44(r1)
	l.lwz	r14,-40(r1)
	l.lwz	r16,-36(r1)
	l.lwz	r18,-32(r1)
	l.lwz	r20,-28(r1)
	l.lwz	r22,-24(r1)
	l.lwz	r24,-20(r1)
	l.lwz	r26,-16(r1)
	l.lwz	r28,-12(r1)
	l.jr	r9
	l.lwz	r30,-8(r1)

;; fn0000D628: 0000D628
;;   Called from:
;;     000099A4 (in fn0000998C)
fn0000D628 proc
	l.movhi	r3,00000001
	l.ori	r3,r3,000034C0
	l.lbz	r11,0(r3)
	l.slli	r11,r11,00000018
	l.jr	r9
	l.srai	r11,r11,00000018

;; fn0000D640: 0000D640
;;   Called from:
;;     0000ED7C (in fn0000ED78)
fn0000D640 proc
	l.sw	-12(r1),r2
	l.sw	-4(r1),r9
	l.sw	-8(r1),r14
	l.jal	0000ECE8
	l.addi	r1,r1,-0000000C
	l.sub	r2,r0,r11
	l.or	r11,r2,r11
	l.movhi	r2,00000001
	l.xori	r11,r11,0000FFFF
	l.ori	r2,r2,000034C4
	l.srli	r11,r11,0000001F
	l.sw	0(r2),r11
	l.addi	r11,r0,-0000000D
	l.lwz	r2,0(r2)
	l.sfnei	r2,00000000
	l.bf	0000D758
	l.movhi	r3,00000001

l0000D684:
	l.movhi	r14,00000001
	l.ori	r14,r14,000034C8
	l.addi	r4,r0,+00000002
	l.ori	r3,r3,0000C200
	l.ori	r5,r4,00000000
	l.sw	0(r14),r3
	l.jal	00009744
	l.addi	r3,r0,+00000001
	l.addi	r5,r0,+00000002
	l.addi	r3,r0,+00000001
	l.jal	00009744
	l.addi	r4,r0,+00000003
	l.addi	r3,r0,+0000001B
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.addi	r4,r0,+00000001
	l.jal	0000B320
	l.addi	r3,r0,+0000001B
	l.jal	0000BF88
	l.addi	r3,r0,+00000019
	l.lwz	r3,0(r14)
	l.lwz	r4,0(r14)
	l.slli	r3,r3,00000003
	l.slli	r4,r4,00000004
	l.jal	0000FEDC
	l.add	r3,r11,r3
	l.movhi	r3,000001F0
	l.andi	r6,r11,000000FF
	l.ori	r4,r3,0000280C
	l.srli	r11,r11,00000008
	l.lwz	r5,0(r4)
	l.ori	r5,r5,00000080
	l.andi	r11,r11,000000FF
	l.sw	0(r4),r5
	l.ori	r5,r3,00002800
	l.sw	0(r5),r6
	l.ori	r5,r3,00002804
	l.sw	0(r5),r11
	l.ori	r5,r3,000028A4
	l.ori	r3,r3,00002808
	l.sw	0(r5),r2
	l.addi	r5,r0,+00000003
	l.sw	0(r4),r5
	l.addi	r4,r0,+00000007
	l.sw	0(r3),r4
	l.movhi	r4,00000000
	l.addi	r3,r0,+0000001B
	l.jal	0000B4D4
	l.ori	r4,r4,0000DA70
	l.movhi	r3,00000001
	l.ori	r11,r2,00000000
	l.ori	r3,r3,000030DC
	l.sw	0(r3),r2

l0000D758:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn0000D76C: 0000D76C
;;   Called from:
;;     0000EC8C (in fn0000EC54)
fn0000D76C proc
	l.movhi	r3,00000001
	l.sw	-8(r1),r2
	l.ori	r3,r3,000030DC
	l.addi	r2,r0,+00000001
	l.sw	-4(r1),r9
	l.sw	0(r3),r2
	l.jal	0000ECE8
	l.addi	r1,r1,-00000008
	l.sub	r3,r0,r11
	l.movhi	r4,00000000
	l.or	r11,r3,r11
	l.movhi	r3,00000001
	l.xori	r11,r11,0000FFFF
	l.ori	r3,r3,000034C4
	l.srli	r11,r11,0000001F
	l.ori	r4,r4,0000DA70
	l.sw	0(r3),r11
	l.jal	0000B4F0
	l.addi	r3,r0,+0000001B
	l.ori	r3,r2,00000000
	l.addi	r4,r0,+00000002
	l.jal	00009744
	l.addi	r5,r0,+00000007
	l.ori	r3,r2,00000000
	l.addi	r5,r0,+00000007
	l.jal	00009744
	l.addi	r4,r0,+00000003
	l.addi	r3,r0,+0000001B
	l.jal	0000B320
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+0000001B
	l.jal	0000BC38
	l.addi	r4,r0,+00000000
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000D804: 0000D804
;;   Called from:
;;     0000D928 (in fn0000D8D8)
;;     0000D934 (in fn0000D8D8)
fn0000D804 proc
	l.movhi	r4,00000001
	l.slli	r3,r3,00000018
	l.ori	r4,r4,000030DC
	l.addi	r11,r0,-0000000D
	l.lwz	r4,0(r4)
	l.sfnei	r4,00000000
	l.bf	0000D864
	l.srai	r3,r3,00000018

l0000D824:
	l.movhi	r4,00000001
	l.ori	r4,r4,000034C4
	l.lwz	r4,0(r4)
	l.sfnei	r4,00000000
	l.bf	0000D864
	l.movhi	r4,000001F0

l0000D83C:
	l.ori	r4,r4,0000287C

l0000D840:
	l.lwz	r5,0(r4)
	l.andi	r5,r5,00000002
	l.sfeqi	r5,00000000
	l.bf	0000D840
	l.nop

l0000D854:
	l.movhi	r4,000001F0
	l.addi	r11,r0,+00000000
	l.ori	r4,r4,00002800
	l.sw	0(r4),r3

l0000D864:
	l.jr	r9
	l.nop
0000D86C                                     18 80 00 01             ....
0000D870 9D 60 FF F3 A8 84 30 DC 84 84 00 00 BC 24 00 00 .`....0......$..
0000D880 10 00 00 14 15 00 00 00 18 80 00 01 A8 84 34 C4 ..............4.
0000D890 84 84 00 00 BC 24 00 00 10 00 00 0E 15 00 00 00 .....$..........
0000D8A0 A9 64 00 00 18 80 01 F0 A8 A4 28 84 00 00 00 05 .d........(.....
0000D8B0 A8 84 28 00 84 E4 00 00 9D 6B 00 01 D8 06 38 00 ..(......k....8.
0000D8C0 84 C5 00 00 BC 26 00 00 13 FF FF FB E0 C3 58 00 .....&........X.
0000D8D0 44 00 48 00 15 00 00 00                         D.H.....       

;; fn0000D8D8: 0000D8D8
;;   Called from:
;;     0000EF44 (in fn0000EDF4)
;;     0000F0FC (in fn0000EDF4)
fn0000D8D8 proc
	l.sw	-8(r1),r2
	l.ori	r2,r3,00000000
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,000030DC
	l.addi	r1,r1,-00000008
	l.lwz	r3,0(r3)
	l.sfnei	r3,00000000
	l.bf	0000D94C
	l.addi	r11,r0,-0000000D

l0000D900:
	l.movhi	r3,00000001
	l.ori	r3,r3,000034C4
	l.lwz	r3,0(r3)
	l.sfnei	r3,00000000
	l.bnf	0000D93C
	l.nop

l0000D918:
	l.j	0000D950
	l.addi	r1,r1,+00000008

l0000D920:
	l.bf	0000D930
	l.nop

l0000D928:
	l.jal	0000D804
	l.addi	r3,r0,+0000000D

l0000D930:
	l.lbs	r3,0(r2)
	l.jal	0000D804
	l.addi	r2,r2,+00000001

l0000D93C:
	l.lbs	r11,0(r2)
	l.sfnei	r11,00000000
	l.bf	0000D920
	l.sfnei	r11,0000000A

l0000D94C:
	l.addi	r1,r1,+00000008

l0000D950:
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000D95C: 0000D95C
;;   Called from:
;;     0000F7AC (in fn0000F444)
fn0000D95C proc
	l.movhi	r4,00000001
	l.sw	-4(r1),r9
	l.ori	r4,r4,000034C4
	l.sw	-8(r1),r2
	l.lwz	r4,0(r4)
	l.addi	r1,r1,-00000008
	l.sfnei	r4,00000000
	l.bf	0000DA60
	l.addi	r11,r0,-0000000D

l0000D980:
	l.movhi	r4,00000001
	l.ori	r4,r4,000034C8
	l.sw	0(r4),r3
	l.movhi	r3,000001F0
	l.ori	r3,r3,00002880

l0000D994:
	l.lwz	r4,0(r3)
	l.sfnei	r4,00000000
	l.bf	0000D994
	l.nop

l0000D9A4:
	l.jal	0000BF88
	l.addi	r3,r0,+00000019
	l.movhi	r4,00000001
	l.ori	r4,r4,000034C8
	l.lwz	r3,0(r4)
	l.lwz	r4,0(r4)
	l.slli	r3,r3,00000003
	l.slli	r4,r4,00000004
	l.jal	0000FEDC
	l.add	r3,r11,r3
	l.movhi	r3,000001F0
	l.sfeqi	r11,00000000
	l.ori	r3,r3,0000280C
	l.lwz	r6,0(r3)
	l.bnf	0000D9E8
	l.movhi	r4,000001F0

l0000D9E4:
	l.addi	r11,r0,+00000001

l0000D9E8:
	l.ori	r7,r6,00000080
	l.ori	r3,r4,000028A4
	l.andi	r8,r11,000000FF
	l.lwz	r5,0(r3)
	l.srli	r11,r11,00000008
	l.ori	r5,r5,00000002
	l.addi	r2,r0,-00000081
	l.sw	0(r3),r5
	l.ori	r5,r4,0000280C
	l.andi	r11,r11,000000FF
	l.sw	0(r5),r7
	l.ori	r7,r4,00002800
	l.ori	r4,r4,00002804
	l.sw	0(r7),r8
	l.and	r6,r6,r2
	l.sw	0(r4),r11
	l.sw	0(r5),r6
	l.lwz	r4,0(r3)
	l.ori	r4,r4,00000004
	l.sw	0(r3),r4

l0000DA38:
	l.lwz	r11,0(r3)
	l.andi	r11,r11,00000004
	l.sfnei	r11,00000000
	l.bf	0000DA38
	l.nop

l0000DA4C:
	l.lwz	r4,0(r3)
	l.addi	r2,r0,-00000003
	l.ori	r4,r4,00000004
	l.and	r4,r4,r2
	l.sw	0(r3),r4

l0000DA60:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000DA70 D7 E1 17 F8 18 40 00 01 D7 E1 4F FC A8 42 34 C4 .....@....O..B4.
0000DA80 9C 21 FF F8 84 42 00 00 BC 22 00 00 10 00 00 1E .!...B..."......
0000DA90 9D 60 FF F3 BC 03 00 00 10 00 00 06 BC 03 00 01 .`..............
0000DAA0 0C 00 00 19 9D 60 FF FD 00 00 00 0F 18 60 00 01 .....`.......`..
0000DAB0 18 40 01 F0 A8 42 28 7C 84 62 00 00 A4 63 00 04 .@...B(|.b...c..
0000DAC0 BC 03 00 00 13 FF FF FD 9C 60 00 01 18 40 00 01 .........`...@..
0000DAD0 A8 42 30 DC 9D 60 00 00 D4 02 18 00 00 00 00 0B .B0..`..........
0000DAE0 9C 21 00 08 A8 63 34 C8 84 63 00 00 07 FF FF 9C .!...c4..c......
0000DAF0 15 00 00 00 18 60 00 01 A9 62 00 00 A8 63 30 DC .....`...b...c0.
0000DB00 D4 03 10 00 9C 21 00 08 85 21 FF FC 44 00 48 00 .....!...!..D.H.
0000DB10 84 41 FF F8 18 60 00 01 A8 63 34 C8 85 63 00 00 .A...`...c4..c..
0000DB20 44 00 48 00 15 00 00 00                         D.H.....       

;; fn0000DB28: 0000DB28
;;   Called from:
;;     0000A6D4 (in fn0000A6B0)
;;     0000ACD8 (in fn0000AC84)
;;     0000ACEC (in fn0000AC84)
;;     0000AD0C (in fn0000AC84)
;;     0000AD20 (in fn0000AC84)
;;     0000BB20 (in fn0000BAC8)
;;     0000BE64 (in fn0000BE00)
;;     0000BE9C (in fn0000BE00)
;;     0000BEE4 (in fn0000BE00)
;;     0000BF20 (in fn0000BE00)
;;     0000BF68 (in fn0000BE00)
;;     0000C8FC (in fn0000C8A0)
;;     0001082C (in fn00010570)
fn0000DB28 proc
	l.sfnei	r3,00000000
	l.bf	0000DB28
	l.addi	r3,r3,-00000001

l0000DB34:
	l.addi	r3,r3,+00000001
	l.jr	r9
	l.nop

;; fn0000DB40: 0000DB40
;;   Called from:
;;     0000EFE4 (in fn0000EDF4)
fn0000DB40 proc
	l.sw	-32(r1),r2
	l.sw	-28(r1),r14
	l.sw	-16(r1),r20
	l.sw	-4(r1),r9
	l.sw	-24(r1),r16
	l.sw	-20(r1),r18
	l.sw	-12(r1),r22
	l.sw	-8(r1),r24
	l.ori	r2,r3,00000000
	l.addi	r1,r1,-00000030
	l.ori	r14,r4,00000000
	l.sfnei	r3,00000000
	l.bf	0000DB88
	l.ori	r20,r5,00000000

l0000DB78:
	l.addi	r3,r0,+00000030
	l.sb	1(r4),r2
	l.j	0000DC44
	l.sb	0(r4),r3

l0000DB88:
	l.addi	r18,r0,+00000000
	l.sfges	r3,r18
	l.bf	0000DBA0
	l.nop

l0000DB98:
	l.sub	r2,r0,r3
	l.addi	r18,r0,+00000001

l0000DBA0:
	l.addi	r16,r0,+00000000
	l.j	0000DBB0
	l.ori	r22,r1,00000000

l0000DBAC:
	l.ori	r16,r3,00000000

l0000DBB0:
	l.ori	r3,r2,00000000
	l.ori	r4,r20,00000000
	l.jal	0001005C
	l.add	r24,r22,r16
	l.movhi	r6,00000001
	l.ori	r3,r2,00000000
	l.ori	r6,r6,00002BF4
	l.ori	r4,r20,00000000
	l.add	r11,r11,r6
	l.lbz	r11,0(r11)
	l.jal	0000FFD8
	l.sb	0(r24),r11
	l.addi	r3,r16,+00000001
	l.sfgtsi	r11,+00000000
	l.bf	0000DBAC
	l.ori	r2,r11,00000000

l0000DBF0:
	l.sfeqi	r18,00000000
	l.bf	0000DC08
	l.ori	r2,r18,00000000

l0000DBFC:
	l.addi	r2,r0,+0000002D
	l.sb	0(r14),r2
	l.addi	r2,r0,+00000001

l0000DC08:
	l.add	r3,r1,r16
	l.add	r4,r14,r2
	l.j	0000DC28
	l.addi	r5,r1,-00000001

l0000DC18:
	l.lbz	r6,0(r3)
	l.addi	r3,r3,-00000001
	l.sb	0(r4),r6
	l.addi	r4,r4,+00000001

l0000DC28:
	l.sfne	r3,r5
	l.bf	0000DC18
	l.nop

l0000DC34:
	l.add	r2,r14,r2
	l.add	r16,r2,r16
	l.addi	r2,r0,+00000000
	l.sb	1(r16),r2

l0000DC44:
	l.addi	r1,r1,+00000030
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-32(r1)
	l.lwz	r14,-28(r1)
	l.lwz	r16,-24(r1)
	l.lwz	r18,-20(r1)
	l.lwz	r20,-16(r1)
	l.lwz	r22,-12(r1)
	l.jr	r9
	l.lwz	r24,-8(r1)

;; fn0000DC70: 0000DC70
;;   Called from:
;;     0000F004 (in fn0000EDF4)
fn0000DC70 proc
	l.sw	-28(r1),r2
	l.sw	-24(r1),r14
	l.sw	-20(r1),r16
	l.sw	-12(r1),r20
	l.sw	-8(r1),r22
	l.sw	-4(r1),r9
	l.sw	-16(r1),r18
	l.addi	r16,r0,+00000000
	l.addi	r1,r1,-0000002C
	l.ori	r2,r3,00000000
	l.ori	r14,r4,00000000
	l.ori	r20,r5,00000000
	l.sfne	r3,r16
	l.bf	0000DCC0
	l.ori	r22,r1,00000000

l0000DCAC:
	l.addi	r3,r0,+00000030
	l.sb	1(r4),r2
	l.j	0000DD38
	l.sb	0(r4),r3

l0000DCBC:
	l.ori	r16,r3,00000000

l0000DCC0:
	l.ori	r3,r2,00000000
	l.ori	r4,r20,00000000
	l.jal	0001003C
	l.add	r18,r22,r16
	l.movhi	r5,00000001
	l.ori	r3,r2,00000000
	l.ori	r5,r5,00002BF4
	l.ori	r4,r20,00000000
	l.add	r11,r11,r5
	l.lbz	r11,0(r11)
	l.jal	0000FEDC
	l.sb	0(r18),r11
	l.addi	r3,r16,+00000001
	l.sfnei	r11,00000000
	l.bf	0000DCBC
	l.ori	r2,r11,00000000

l0000DD00:
	l.ori	r2,r18,00000000
	l.ori	r3,r14,00000000
	l.j	0000DD20
	l.addi	r4,r1,-00000001

l0000DD10:
	l.lbz	r5,0(r2)
	l.addi	r2,r2,-00000001
	l.sb	0(r3),r5
	l.addi	r3,r3,+00000001

l0000DD20:
	l.sfne	r2,r4
	l.bf	0000DD10
	l.nop

l0000DD2C:
	l.add	r16,r14,r16
	l.addi	r2,r0,+00000000
	l.sb	1(r16),r2

l0000DD38:
	l.addi	r1,r1,+0000002C
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-28(r1)
	l.lwz	r14,-24(r1)
	l.lwz	r16,-20(r1)
	l.lwz	r18,-16(r1)
	l.lwz	r20,-12(r1)
	l.jr	r9
	l.lwz	r22,-8(r1)

;; fn0000DD60: 0000DD60
;;   Called from:
;;     0000EC6C (in fn0000EC54)
;;     0000F7F0 (in fn0000F444)
fn0000DD60 proc
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.sw	-4(r1),r9
	l.ori	r2,r4,00000000
	l.addi	r1,r1,-00000014
	l.movhi	r4,00000001
	l.sw	0(r1),r3
	l.ori	r4,r4,00002C6D
	l.addi	r3,r0,+0000000F
	l.ori	r16,r5,00000000
	l.jal	0000EDF4
	l.addi	r14,r0,+00000000
	l.j	0000DDDC
	l.sfltu	r14,r16

l0000DD9C:
	l.sfnei	r3,00000000
	l.bf	0000DDB8
	l.movhi	r4,00000001

l0000DDA8:
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002C73
	l.jal	0000EDF4
	l.sw	0(r1),r2

l0000DDB8:
	l.lwz	r3,0(r2)
	l.movhi	r4,00000001
	l.sw	0(r1),r3
	l.ori	r4,r4,00002C7D
	l.addi	r3,r0,+0000000F
	l.addi	r14,r14,+00000004
	l.jal	0000EDF4
	l.addi	r2,r2,+00000004
	l.sfltu	r14,r16

l0000DDDC:
	l.bf	0000DD9C
	l.andi	r3,r14,0000000F

l0000DDE4:
	l.addi	r1,r1,+00000014
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002E0B
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.j	0000EDF4
	l.lwz	r16,-8(r1)

;; fn0000DE08: 0000DE08
;;   Called from:
;;     0000F00C (in fn0000EDF4)
;;     0000F07C (in fn0000EDF4)
fn0000DE08 proc
	l.ori	r4,r3,00000000

l0000DE0C:
	l.lbs	r5,0(r4)
	l.sfnei	r5,00000000
	l.bf	0000DE0C
	l.addi	r4,r4,+00000001

l0000DE1C:
	l.addi	r4,r4,-00000001
	l.jr	r9
	l.sub	r11,r4,r3

;; fn0000DE28: 0000DE28
;;   Called from:
;;     0000EAE8 (in fn0000E98C)
;;     0000F034 (in fn0000EDF4)
;;     0000F074 (in fn0000EDF4)
fn0000DE28 proc
	l.addi	r5,r0,+00000000

l0000DE2C:
	l.add	r6,r4,r5
	l.add	r7,r3,r5
	l.lbs	r6,0(r6)
	l.sb	0(r7),r6
	l.sfnei	r6,00000000
	l.bf	0000DE2C
	l.addi	r5,r5,+00000001

l0000DE48:
	l.jr	r9
	l.ori	r11,r3,00000000

;; fn0000DE50: 0000DE50
;;   Called from:
;;     0000ED4C (in fn0000ECF8)
fn0000DE50 proc
	l.sw	-4(r1),r2
	l.ori	r6,r3,00000000
	l.sfeqi	r5,00000000
	l.bnf	0000DE6C
	l.addi	r1,r1,-00000004

l0000DE64:
	l.j	0000DEBC
	l.addi	r1,r1,+00000004

l0000DE6C:
	l.lbs	r7,0(r6)
	l.sfnei	r7,00000000
	l.bf	0000DE6C
	l.addi	r6,r6,+00000001

l0000DE7C:
	l.addi	r6,r6,-00000001
	l.j	0000DEA8
	l.add	r8,r4,r7

l0000DE88:
	l.addi	r7,r7,+00000001
	l.sfne	r5,r7
	l.bf	0000DEA4
	l.addi	r6,r6,+00000001

l0000DE98:
	l.addi	r2,r0,+00000000
	l.j	0000DEB8
	l.sb	0(r6),r2

l0000DEA4:
	l.add	r8,r4,r7

l0000DEA8:
	l.lbs	r8,0(r8)
	l.sfnei	r8,00000000
	l.bf	0000DE88
	l.sb	0(r6),r8

l0000DEB8:
	l.addi	r1,r1,+00000004

l0000DEBC:
	l.ori	r11,r3,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)
0000DEC8                         9C C0 00 00 E0 A3 30 00         ......0.
0000DED0 E0 E4 30 00 8C A5 00 00 8C E7 00 00 E4 05 38 00 ..0...........8.
0000DEE0 10 00 00 07 BC 25 00 00 E4 65 38 00 10 00 00 07 .....%...e8.....
0000DEF0 9D 60 00 01 00 00 00 05 9D 60 FF FF 13 FF FF F4 .`.......`......
0000DF00 9C C6 00 01 A9 65 00 00 44 00 48 00 15 00 00 00 .....e..D.H.....

;; fn0000DF10: 0000DF10
;;   Called from:
;;     0000923C (in fn0000EC54)
;;     0000A64C (in fn0000A634)
;;     0000E538 (in fn0000E50C)
;;     0000E54C (in fn0000E50C)
;;     0000ED30 (in fn0000ECF8)
;;     0000F648 (in fn0000F444)
;;     000105B0 (in fn00010570)
;;     000105DC (in fn00010570)
fn0000DF10 proc
	l.or	r7,r4,r3
	l.sw	-4(r1),r2
	l.andi	r7,r7,00000003
	l.addi	r6,r0,+00000000
	l.sfne	r7,r6
	l.bf	0000DF48
	l.addi	r1,r1,-00000004

l0000DF2C:
	l.j	0000DF5C
	l.srli	r6,r5,00000002

l0000DF34:
	l.add	r7,r3,r6
	l.lbz	r8,0(r8)
	l.addi	r6,r6,+00000001
	l.sb	0(r7),r8
	l.addi	r5,r5,-00000001

l0000DF48:
	l.sfnei	r5,00000000
	l.bf	0000DF34
	l.add	r8,r4,r6

l0000DF54:
	l.j	0000DFC0
	l.addi	r1,r1,+00000004

l0000DF5C:
	l.j	0000DF7C
	l.sfnei	r6,00000000

l0000DF64:
	l.add	r11,r3,r7
	l.lwz	r8,0(r8)
	l.addi	r7,r7,+00000004
	l.sw	0(r11),r8
	l.addi	r6,r6,-00000001
	l.sfnei	r6,00000000

l0000DF7C:
	l.bf	0000DF64
	l.add	r8,r4,r7

l0000DF84:
	l.addi	r2,r0,-00000004
	l.and	r7,r5,r2
	l.andi	r5,r5,00000003
	l.add	r4,r4,r7
	l.j	0000DFB0
	l.add	r7,r3,r7

l0000DF9C:
	l.add	r8,r7,r6
	l.lbz	r11,0(r11)
	l.addi	r6,r6,+00000001
	l.sb	0(r8),r11
	l.addi	r5,r5,-00000001

l0000DFB0:
	l.sfnei	r5,00000000
	l.bf	0000DF9C
	l.add	r11,r4,r6

l0000DFBC:
	l.addi	r1,r1,+00000004

l0000DFC0:
	l.ori	r11,r3,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000DFCC: 0000DFCC
;;   Called from:
;;     0000F1FC (in fn0000F194)
;;     00010250 (in fn000101A8)
fn0000DFCC proc
	l.j	0000DFE0
	l.ori	r6,r3,00000000

l0000DFD4:
	l.sb	0(r6),r4
	l.addi	r5,r5,-00000001
	l.addi	r6,r6,+00000001

l0000DFE0:
	l.sfnei	r5,00000000
	l.bf	0000DFD4
	l.nop

l0000DFEC:
	l.jr	r9
	l.ori	r11,r3,00000000
0000DFF4             D7 E1 17 E4 D7 E1 77 E8 D7 E1 87 EC     ......w.....
0000E000 D7 E1 97 F0 D7 E1 A7 F4 D7 E1 4F FC D7 E1 B7 F8 ..........O.....
0000E010 9C 21 FF C4 AA 03 00 00 A9 C4 00 00 9C 61 00 3C .!...........a.<
0000E020 9C 40 00 00 AA 41 00 00 00 00 00 2A 9E 81 00 20 .@...A.....*... 
0000E030 9C 80 00 00 D8 06 20 00 9C C6 00 01 E4 26 A0 00 ...... ......&..
0000E040 13 FF FF FD BC 05 00 25 10 00 00 05 E0 90 10 00 .......%........
0000E050 9C 42 00 01 00 00 00 1E D8 04 28 00 9D CE 00 01 .B........(.....
0000E060 90 8E 00 00 BC 04 00 64 0C 00 00 07 9E C3 00 04 .......d........
0000E070 A8 81 00 00 84 63 00 00 07 FF FE B2 9C A0 00 0A .....c..........
0000E080 A8 76 00 00 00 00 00 05 9C A0 00 00 9C A5 00 01 .v..............
0000E090 D8 06 20 00 9C 42 00 01 E0 92 28 00 9C C0 00 01 .. ..B....(.....
0000E0A0 BC A5 00 1F 10 00 00 03 90 84 00 00 9C C0 00 00 ................
0000E0B0 A4 C6 00 FF BC 06 00 00 10 00 00 05 A4 C4 00 FF ................
0000E0C0 BC 26 00 00 13 FF FF F2 E0 D0 10 00 9D CE 00 01 .&..............
0000E0D0 90 AE 00 00 BC 05 00 00 0F FF FF D6 A8 D2 00 00 ................
0000E0E0 E2 10 10 00 A9 62 00 00 D8 10 28 00 9C 21 00 3C .....b....(..!.<
0000E0F0 85 21 FF FC 84 41 FF E4 85 C1 FF E8 86 01 FF EC .!...A..........
0000E100 86 41 FF F0 86 81 FF F4 44 00 48 00 86 C1 FF F8 .A......D.H.....

;; fn0000E110: 0000E110
;;   Called from:
;;     0000EA64 (in fn0000E98C)
fn0000E110 proc
	l.sw	-8(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.addi	r3,r0,+00000006
	l.addi	r1,r1,-00000008
	l.jal	0000BF88
	l.ori	r2,r2,00003960
	l.sw	0(r2),r11
	l.jal	00009B70
	l.movhi	r2,00000001
	l.sfnei	r11,00000001
	l.bf	0000E14C
	l.ori	r2,r2,0000395C

l0000E144:
	l.j	0000E150
	l.addi	r3,r0,+00000000

l0000E14C:
	l.addi	r3,r0,+00000014

l0000E150:
	l.sw	0(r2),r3
	l.movhi	r2,00000001
	l.ori	r2,r2,0000395C
	l.lwz	r3,0(r2)
	l.jal	00009DDC
	l.movhi	r2,00000001
	l.ori	r2,r2,00003960
	l.addi	r3,r0,+00000014
	l.jal	0000B250
	l.sw	4(r2),r11
	l.movhi	r3,00000000
	l.sw	8(r2),r11
	l.movhi	r2,00000001
	l.ori	r3,r3,00004298
	l.ori	r2,r2,000034CC
	l.addi	r11,r0,+00000000
	l.sw	0(r2),r3
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)
0000E1A4             44 00 48 00 9D 60 00 00                 D.H..`..   

;; fn0000E1AC: 0000E1AC
;;   Called from:
;;     0000F5F4 (in fn0000F444)
fn0000E1AC proc
	l.sw	-28(r1),r2
	l.sw	-16(r1),r18
	l.addi	r2,r0,+000003E8
	l.lwz	r18,40(r3)
	l.sw	-4(r1),r9
	l.mul	r18,r18,r2
	l.movhi	r2,00000001
	l.sw	-24(r1),r14
	l.ori	r2,r2,000034CC
	l.sw	-20(r1),r16
	l.sw	-12(r1),r20
	l.sw	-8(r1),r22
	l.lwz	r2,0(r2)
	l.j	0000E1EC
	l.addi	r1,r1,-0000001C

l0000E1E8:
	l.ori	r2,r3,00000000

l0000E1EC:
	l.addi	r3,r2,+0000000C
	l.lwz	r4,0(r3)
	l.sfgeu	r4,r18
	l.bf	0000E1E8
	l.nop

l0000E200:
	l.movhi	r3,00000001
	l.lwz	r20,4(r2)
	l.ori	r3,r3,00003960
	l.lwz	r22,4(r3)
	l.sfleu	r20,r22
	l.bf	0000E280
	l.addi	r14,r0,+00000000

l0000E21C:
	l.movhi	r14,00000001
	l.ori	r4,r20,00000000
	l.ori	r14,r14,0000395C
	l.addi	r16,r0,-00000024
	l.jal	00009C2C
	l.lwz	r3,0(r14)
	l.sfnei	r11,00000000
	l.bf	0000E380
	l.sub	r3,r20,r22

l0000E240:
	l.jal	0000FEDC
	l.addi	r4,r0,+00000014
	l.slli	r3,r11,00000003
	l.slli	r11,r11,00000005
	l.addi	r4,r0,+000003E8
	l.add	r3,r3,r11
	l.jal	0000FEDC
	l.addi	r3,r3,+000003E7
	l.jal	0000C768
	l.ori	r3,r11,00000000
	l.jal	00009DDC
	l.lwz	r3,0(r14)
	l.lwz	r3,4(r2)
	l.sfne	r11,r3
	l.bf	0000E380
	l.ori	r14,r11,00000000

l0000E280:
	l.movhi	r16,00000001
	l.ori	r16,r16,00003960
	l.lwz	r3,0(r16)
	l.sfeq	r18,r3
	l.bf	0000E308
	l.movhi	r3,00000001

l0000E298:
	l.lwz	r4,8(r2)
	l.lwz	r3,8(r16)
	l.sfleu	r4,r3
	l.bf	0000E2C0
	l.nop

l0000E2AC:
	l.jal	0000B0B8
	l.addi	r3,r0,+00000014
	l.jal	0000B250
	l.addi	r3,r0,+00000014
	l.sw	8(r16),r11

l0000E2C0:
	l.ori	r4,r18,00000000
	l.addi	r3,r0,+00000006
	l.jal	0000BE00
	l.movhi	r16,00000001
	l.addi	r3,r0,+00000006
	l.jal	0000BF88
	l.ori	r16,r16,00003960
	l.lwz	r4,8(r2)
	l.lwz	r3,8(r16)
	l.sfgeu	r4,r3
	l.bf	0000E304
	l.sw	0(r16),r11

l0000E2F0:
	l.jal	0000B0B8
	l.addi	r3,r0,+00000014
	l.jal	0000B250
	l.addi	r3,r0,+00000014
	l.sw	8(r16),r11

l0000E304:
	l.movhi	r3,00000001

l0000E308:
	l.lwz	r4,4(r2)
	l.ori	r3,r3,00003960
	l.lwz	r3,4(r3)
	l.sfgeu	r4,r3
	l.bf	0000E364
	l.movhi	r3,00000001

l0000E320:
	l.movhi	r14,00000001
	l.addi	r16,r0,-00000024
	l.ori	r14,r14,0000395C
	l.jal	00009C2C
	l.lwz	r3,0(r14)
	l.sfnei	r11,00000000
	l.bf	0000E380
	l.nop

l0000E340:
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.jal	00009DDC
	l.lwz	r3,0(r14)
	l.lwz	r3,4(r2)
	l.sfne	r11,r3
	l.bf	0000E380
	l.ori	r14,r11,00000000

l0000E360:
	l.movhi	r3,00000001

l0000E364:
	l.lwz	r2,4(r2)
	l.ori	r3,r3,00003960
	l.lwz	r4,4(r3)
	l.sfeq	r2,r4
	l.bf	0000E380
	l.addi	r16,r0,+00000000

l0000E37C:
	l.sw	4(r3),r14

l0000E380:
	l.addi	r1,r1,+0000001C
	l.ori	r11,r16,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-28(r1)
	l.lwz	r14,-24(r1)
	l.lwz	r16,-20(r1)
	l.lwz	r18,-16(r1)
	l.lwz	r20,-12(r1)
	l.jr	r9
	l.lwz	r22,-8(r1)

;; fn0000E3A8: 0000E3A8
;;   Called from:
;;     0000EA6C (in fn0000E98C)
fn0000E3A8 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r2
	l.ori	r3,r3,0000396C
	l.addi	r2,r0,+00000000
	l.addi	r1,r1,-00000004
	l.sw	0(r3),r2
	l.movhi	r3,00000001
	l.ori	r11,r2,00000000
	l.ori	r3,r3,00003970
	l.sw	0(r3),r2
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)
0000E3DC                                     44 00 48 00             D.H.
0000E3E0 9D 60 00 00 18 80 01 C2 9D 60 00 00 A8 A4 00 08 .`.......`......
0000E3F0 84 A5 00 00 D4 03 28 00 A8 A4 00 10 84 A5 00 00 ......(.........
0000E400 D4 03 28 04 A8 A4 00 18 84 A5 00 00 D4 03 28 08 ..(...........(.
0000E410 A8 A4 00 28 84 A5 00 00 D4 03 28 0C A8 A4 00 2C ...(......(....,
0000E420 84 A5 00 00 D4 03 28 10 A8 A4 00 30 84 A5 00 00 ......(....0....
0000E430 D4 03 28 14 A8 A4 00 38 84 A5 00 00 D4 03 28 18 ..(....8......(.
0000E440 A8 A4 00 40 84 A5 00 00 D4 03 28 1C A8 A4 00 44 ...@......(....D
0000E450 A8 84 00 48 84 A5 00 00 D4 03 28 20 84 84 00 00 ...H......( ....
0000E460 44 00 48 00 D4 03 20 24 18 80 01 C2 84 C3 00 00 D.H... $........
0000E470 A8 A4 00 08 9D 60 00 00 D4 05 30 00 A8 A4 00 10 .....`....0.....
0000E480 84 C3 00 04 D4 05 30 00 A8 A4 00 18 84 C3 00 08 ......0.........
0000E490 D4 05 30 00 A8 A4 00 28 84 C3 00 0C D4 05 30 00 ..0....(......0.
0000E4A0 A8 A4 00 2C 84 C3 00 10 D4 05 30 00 A8 A4 00 30 ...,......0....0
0000E4B0 84 C3 00 14 D4 05 30 00 A8 A4 00 38 84 C3 00 18 ......0....8....
0000E4C0 D4 05 30 00 A8 A4 00 40 84 C3 00 1C D4 05 30 00 ..0....@......0.
0000E4D0 A8 A4 00 44 84 C3 00 20 A8 84 00 48 D4 05 30 00 ...D... ...H..0.
0000E4E0 84 63 00 24 D4 04 18 00 44 00 48 00 15 00 00 00 .c.$....D.H.....

;; fn0000E4F0: 0000E4F0
;;   Called from:
;;     000106CC (in fn00010570)
;;     000106E8 (in fn00010570)
;;     0001075C (in fn00010570)
;;     00012088 (in fn00010570)
;;     000121A8 (in fn00010570)
fn0000E4F0 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.ori	r5,r3,00000000
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r5
	l.ori	r3,r4,00000000

;; fn0000E50C: 0000E50C
;;   Called from:
;;     0000F688 (in fn0000F444)
fn0000E50C proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.lwz	r4,32(r3)
	l.lwz	r5,36(r3)
	l.sfnei	r5,00000001
	l.bf	0000E55C
	l.addi	r1,r1,-00000008

l0000E528:
	l.addi	r2,r3,+00000028
	l.movhi	r3,00000001
	l.ori	r4,r2,00000000
	l.ori	r3,r3,00003974
	l.jal	0000DF10
	l.addi	r5,r0,+0000000C
	l.movhi	r3,00000001
	l.ori	r4,r2,00000000
	l.ori	r3,r3,000030E0
	l.jal	0000DF10
	l.addi	r5,r0,+0000000C
	l.j	0000E580
	l.addi	r1,r1,+00000008

l0000E55C:
	l.movhi	r4,00000001
	l.ori	r4,r4,000030E0
	l.lwz	r2,0(r4)
	l.sw	40(r3),r2
	l.lwz	r2,4(r4)
	l.sw	44(r3),r2
	l.lwz	r2,8(r4)
	l.sw	48(r3),r2
	l.addi	r1,r1,+00000008

l0000E580:
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000E590: 0000E590
;;   Called from:
;;     0000F7C0 (in fn0000F444)
fn0000E590 proc
	l.movhi	r6,00000001
	l.addi	r11,r0,+00000000
	l.ori	r6,r6,0000375C
	l.sw	0(r6),r3
	l.movhi	r3,00000001
	l.ori	r3,r3,00003104
	l.sw	0(r3),r4
	l.movhi	r3,00000001
	l.ori	r3,r3,00003108
	l.jr	r9
	l.sw	0(r3),r5

;; fn0000E5BC: 0000E5BC
;;   Called from:
;;     000107B0 (in fn00010570)
;;     00011FF4 (in fn00010570)
fn0000E5BC proc
	l.movhi	r3,00000001
	l.ori	r3,r3,0000375C
	l.jr	r9
	l.lwz	r11,0(r3)

;; fn0000E5CC: 0000E5CC
;;   Called from:
;;     000107C4 (in fn00010570)
;;     0001201C (in fn00010570)
fn0000E5CC proc
	l.sw	-12(r1),r14
	l.movhi	r4,00000001
	l.movhi	r14,00000001
	l.sw	-4(r1),r9
	l.ori	r14,r14,00003104
	l.sw	-16(r1),r2
	l.sw	-8(r1),r16
	l.addi	r3,r0,+0000000F
	l.addi	r1,r1,-00000018
	l.ori	r4,r4,00002C82
	l.movhi	r16,00000001
	l.jal	0000EDF4
	l.lwz	r2,0(r14)
	l.lwz	r3,0(r14)
	l.ori	r16,r16,00003108
	l.sw	0(r1),r3
	l.movhi	r4,00000001
	l.lwz	r3,0(r16)
	l.ori	r4,r4,00002C90
	l.sw	4(r1),r3
	l.jal	0000EDF4
	l.addi	r3,r0,+0000000F
	l.lwz	r3,0(r14)
	l.lwz	r4,0(r16)
	l.addi	r14,r0,+00000000
	l.j	0000E644
	l.add	r3,r4,r3

l0000E638:
	l.lwz	r4,0(r2)
	l.addi	r2,r2,+00000004
	l.add	r14,r14,r4

l0000E644:
	l.sfltu	r2,r3
	l.bf	0000E638
	l.movhi	r4,00000001

l0000E650:
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002C9F
	l.addi	r1,r1,+00000018
	l.ori	r11,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)
0000E678                         18 60 00 01 D7 E1 4F FC         .`....O.
0000E680 D7 E1 17 F8 A8 63 3A 18 9C 21 FF F4 00 00 00 03 .....c:..!......
0000E690 9C 80 00 00 9C 84 00 04 18 40 00 01 A8 42 3E 18 .........@...B>.
0000E6A0 E4 63 10 00 10 00 00 06 15 00 00 00 84 A3 00 00 .c..............
0000E6B0 BC 05 00 00 13 FF FF F8 9C 63 00 04 D4 01 20 00 .........c.... .
0000E6C0 18 80 00 01 9C 60 00 0F 04 00 01 CB A8 84 2C AE .....`........,.
0000E6D0 9C 21 00 0C 9D 60 00 00 85 21 FF FC 44 00 48 00 .!...`...!..D.H.
0000E6E0 84 41 FF F8                                     .A..           

;; fn0000E6E4: 0000E6E4
;;   Called from:
;;     0000EB5C (in fn0000E98C)
fn0000E6E4 proc
	l.movhi	r3,00000000
	l.sw	-4(r1),r9
	l.ori	r3,r3,0000E678
	l.jal	0000E96C
	l.addi	r1,r1,-00000004
	l.addi	r3,r0,+00000011
	l.mfspr	r4,r3,VR
	l.ori	r4,r4,00000004
	l.mtspr	r3,r4,VR
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.nop

;; fn0000E718: 0000E718
;;   Called from:
;;     00008848 (in fn00008834)
;;     0000C4C4 (in fn0000C48C)
;;     0000CD38 (in fn0000CCA8)
;;     0000EE40 (in fn0000EDF4)
;;     0000F258 (in fn0000F250)
;;     0000F38C (in fn0000F368)
;;     0000F45C (in fn0000F444)
;;     0000F904 (in fn0000F8E8)
fn0000E718 proc
	l.sw	-4(r1),r2
	l.addi	r2,r0,-00000007
	l.addi	r3,r0,+00000011
	l.addi	r1,r1,-00000004
	l.mfspr	r11,r3,VR
	l.and	r4,r11,r2
	l.mtspr	r3,r4,VR
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000E740: 0000E740
;;   Called from:
;;     000088A8 (in fn00008834)
;;     0000892C (in fn000088FC)
;;     00008960 (in fn000088FC)
;;     0000C51C (in fn0000C48C)
;;     0000D1D8 (in fn0000CCA8)
;;     0000F104 (in fn0000EDF4)
;;     0000F2BC (in fn0000F250)
;;     0000F428 (in fn0000F368)
;;     0000F850 (in fn0000F444)
;;     0000F960 (in fn0000F8E8)
fn0000E740 proc
	l.addi	r4,r0,+00000011
	l.mtspr	r4,r3,VR
	l.jr	r9
	l.nop
0000E750 00 00 00 00 15 00 00 00                         ........       

;; fn0000E758: 0000E758
fn0000E758 proc
	l.sw	-8(r1),r14
	l.sw	-4(r1),r9
	l.sw	-12(r1),r2
	l.ori	r14,r5,00000000
	l.sfnei	r3,00000008
	l.bf	0000E788
	l.addi	r1,r1,-00000018

l0000E774:
	l.addi	r1,r1,+00000018
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.j	00008C34
	l.lwz	r14,-8(r1)

l0000E788:
	l.movhi	r2,00000001
	l.sw	0(r1),r3
	l.slli	r3,r3,00000002
	l.ori	r2,r2,00003110
	l.add	r3,r3,r2
	l.lwz	r2,0(r3)
	l.sw	8(r1),r4
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002CC1
	l.jal	0000EDF4
	l.sw	4(r1),r2
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002CE8
	l.jal	0000EDF4
	l.addi	r2,r0,+00000002

l0000E7CC:
	l.sw	0(r1),r2
	l.movhi	r4,00000001
	l.lwz	r3,0(r14)
	l.ori	r4,r4,00002CF8
	l.sw	4(r1),r3
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.addi	r2,r2,+00000001
	l.sfnei	r2,00000020
	l.bf	0000E7CC
	l.addi	r14,r14,+00000004

l0000E7F8:
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002D08
	l.nop

l0000E80C:
	l.j	0000E80C
	l.nop
0000E814             D7 E1 17 F8 18 40 00 01 D7 E1 4F FC     .....@....O.
0000E820 A8 42 37 6C 9C 21 FF F8 84 62 00 00 9C 63 00 01 .B7l.!...b...c..
0000E830 D4 02 18 00 18 40 00 01 00 00 00 18 A8 42 39 4C .....@.......B9L
0000E840 84 83 00 04 BD A4 00 00 10 00 00 05 BC 24 00 00 .............$..
0000E850 9C 84 FF FF 00 00 00 0B D4 03 20 04 10 00 00 09 .......... .....
0000E860 15 00 00 00 84 83 00 00 85 63 00 08 BC 0B 00 00 .........c......
0000E870 10 00 00 04 D4 03 20 04 48 00 58 00 84 63 00 0C ...... .H.X..c..
0000E880 18 60 00 01 9C 42 00 04 A8 63 39 54 E4 02 18 00 .`...B...c9T....
0000E890 10 00 00 06 15 00 00 00 84 62 00 00 BC 23 00 00 .........b...#..
0000E8A0 13 FF FF E8 15 00 00 00 9C 21 00 08 9D 60 00 00 .........!...`..
0000E8B0 85 21 FF FC 44 00 48 00 84 41 FF F8             .!..D.H..A..   

;; fn0000E8BC: 0000E8BC
;;   Called from:
;;     0000EB64 (in fn0000E98C)
;;     0000EE60 (in fn0000EDF4)
fn0000E8BC proc
	l.movhi	r3,00000001
	l.ori	r3,r3,0000376C
	l.lwz	r11,0(r3)
	l.jr	r9
	l.nop
0000E8D0 E0 83 18 00 D7 E1 4F FC E0 84 18 00 9C 21 FF FC ......O......!..
0000E8E0 B8 A4 00 05 E0 84 28 00 E0 64 18 00 04 00 05 7C ......(..d.....|
0000E8F0 9C 80 03 E8 9C 21 00 04 85 21 FF FC 44 00 48 00 .....!...!..D.H.
0000E900 15 00 00 00                                     ....           

;; fn0000E904: 0000E904
;;   Called from:
;;     0000EA7C (in fn0000E98C)
fn0000E904 proc
	l.movhi	r3,00000000
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.ori	r3,r3,0000E814
	l.addi	r4,r0,+00000000
	l.jal	0000C48C
	l.addi	r1,r1,-00000008
	l.movhi	r4,00000001
	l.ori	r3,r11,00000000
	l.ori	r4,r4,00003768
	l.sfeqi	r3,00000000
	l.sw	0(r4),r11
	l.bf	0000E95C
	l.addi	r11,r0,-00000024

l0000E93C:
	l.movhi	r4,00000001
	l.addi	r2,r0,+00000000
	l.ori	r4,r4,0000376C
	l.ori	r5,r2,00000000
	l.sw	0(r4),r2
	l.jal	0000C588
	l.addi	r4,r0,+0000000A
	l.ori	r11,r2,00000000

l0000E95C:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000E96C: 0000E96C
;;   Called from:
;;     0000E6F0 (in fn0000E6E4)
fn0000E96C proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.ori	r4,r3,00000000
	l.addi	r1,r1,+00000004
	l.movhi	r3,00000001
	l.lwz	r9,-4(r1)
	l.j	0000F8E8
	l.ori	r3,r3,00003770

;; fn0000E98C: 0000E98C
fn0000E98C proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.jal	0000F89C
	l.addi	r1,r1,-00000010
	l.jal	0000AEDC
	l.ori	r3,r0,0000B000
	l.jal	0000ADBC
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B001
	l.jal	000096F8
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B002
	l.jal	000087BC
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B003
	l.jal	00008AF0
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B004
	l.jal	0000EC18
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B005
	l.jal	0000ED78
	l.nop
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002E02
	l.jal	0000AEDC
	l.ori	r3,r0,0000B006
	l.jal	0000CB40
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B007
	l.jal	0000998C
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B008
	l.jal	000082FC
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B009
	l.jal	000044D0
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00A
	l.jal	0000F194
	l.nop
	l.jal	0000C32C
	l.nop
	l.jal	0000E110
	l.nop
	l.jal	0000E3A8
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00B
	l.jal	0000E904
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00C
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002E0D
	l.jal	0000F250
	l.nop
	l.sfeqi	r11,00000000
	l.bf	0000EB14
	l.ori	r2,r11,00000000

l0000EAB0:
	l.addi	r3,r0,-00000070
	l.addi	r4,r0,+00000002
	l.sb	2(r11),r3
	l.sb	1(r11),r4
	l.addi	r3,r0,+00000000
	l.addi	r4,r0,+00000000
	l.sw	32(r11),r3
	l.sw	36(r11),r4
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000078
	l.sb	3(r11),r4
	l.movhi	r4,00000001
	l.sw	40(r11),r3
	l.ori	r4,r4,00002E16
	l.jal	0000DE28
	l.addi	r3,r11,+0000002C
	l.ori	r3,r2,00000000
	l.jal	00008428
	l.addi	r4,r0,+00000FA0
	l.movhi	r3,00000001
	l.lwz	r4,40(r2)
	l.ori	r3,r3,00003980
	l.sw	0(r3),r4
	l.jal	0000F368
	l.ori	r3,r2,00000000

l0000EB14:
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002E1E
	l.jal	0000EDF4
	l.addi	r2,r0,+00000078
	l.jal	0000EC54
	l.nop
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002E2A
	l.jal	0000AEDC
	l.ori	r3,r0,0000B00D
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002E38
	l.jal	0000EDF4
	l.sw	0(r1),r2
	l.jal	0000E6E4
	l.nop

l0000EB64:
	l.jal	0000E8BC
	l.nop
	l.addi	r4,r0,+000001F4
	l.jal	0001003C
	l.ori	r3,r11,00000000
	l.sfnei	r11,00000000
	l.bf	0000EBDC
	l.ori	r2,r11,00000000

l0000EB84:
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002E4C
	l.movhi	r3,00000001
	l.addi	r4,r0,+00000100
	l.ori	r3,r3,00003770
	l.jal	0000FA38
	l.ori	r5,r2,00000000
	l.j	0000EBC0
	l.nop

l0000EBB0:
	l.lwz	r2,4(r1)
	l.sw	4(r1),r2
	l.lwz	r2,4(r1)
	l.addi	r2,r2,+00000001

l0000EBC0:
	l.sw	4(r1),r2
	l.movhi	r3,00000003
	l.lwz	r2,4(r1)
	l.ori	r3,r3,00000D3F
	l.sfles	r2,r3
	l.bf	0000EBB0
	l.nop

l0000EBDC:
	l.addi	r4,r0,+00000000
	l.sw	4(r1),r4
	l.j	0000EC00
	l.nop

l0000EBEC:
	l.lwz	r2,4(r1)
	l.sw	4(r1),r2
	l.lwz	r2,4(r1)
	l.addi	r2,r2,+00000001
	l.sw	4(r1),r2

l0000EC00:
	l.lwz	r2,4(r1)
	l.sflesi	r2,+0000270F
	l.bnf	0000EB64
	l.nop

l0000EC10:
	l.j	0000EBEC
	l.nop

;; fn0000EC18: 0000EC18
;;   Called from:
;;     0000E9E4 (in fn0000E98C)
fn0000EC18 proc
	l.movhi	r3,00000000
	l.movhi	r4,00000000
	l.sw	-4(r1),r9
	l.ori	r3,r3,00004000
	l.addi	r1,r1,-00000004
	l.jal	00004570
	l.ori	r4,r4,000043C4
	l.movhi	r4,00000000
	l.ori	r4,r4,00004000
	l.lwz	r3,8(r4)
	l.lwz	r4,12(r4)
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	00004570
	l.add	r4,r3,r4

;; fn0000EC54: 0000EC54
;;   Called from:
;;     0000EB28 (in fn0000E98C)
;;     0000F7D0 (in fn0000F444)
fn0000EC54 proc
	l.movhi	r3,00000001
	l.movhi	r4,00000000
	l.sw	-4(r1),r9
	l.ori	r3,r3,00002E6C
	l.addi	r1,r1,-00000004
	l.ori	r4,r4,00004000
	l.jal	0000DD60
	l.addi	r5,r0,+000003C4
	l.movhi	r3,00000000
	l.ori	r3,r3,00004000
	l.lwz	r3,16(r3)
	l.sfnei	r3,00000000
	l.bf	0000EC98
	l.movhi	r3,00000000

l0000EC8C:
	l.jal	0000D76C
	l.nop
	l.movhi	r3,00000000

l0000EC98:
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.j	00009220
	l.ori	r3,r3,00004034

;; fn0000ECA8: 0000ECA8
;;   Called from:
;;     0000F1B8 (in fn0000F194)
fn0000ECA8 proc
	l.movhi	r5,00000000
	l.addi	r11,r0,+00000000
	l.ori	r5,r5,00004000
	l.lwz	r6,0(r5)
	l.sw	0(r3),r6
	l.lwz	r3,4(r5)
	l.jr	r9
	l.sw	0(r4),r3

;; fn0000ECC8: 0000ECC8
;;   Called from:
;;     0001059C (in fn00010570)
fn0000ECC8 proc
	l.movhi	r5,00000000
	l.addi	r11,r0,+00000000
	l.ori	r5,r5,00004000
	l.lwz	r6,8(r5)
	l.sw	0(r3),r6
	l.lwz	r3,12(r5)
	l.jr	r9
	l.sw	0(r4),r3

;; fn0000ECE8: 0000ECE8
;;   Called from:
;;     0000D64C (in fn0000D640)
;;     0000D784 (in fn0000D76C)
fn0000ECE8 proc
	l.movhi	r3,00000000
	l.ori	r3,r3,00004000
	l.jr	r9
	l.lwz	r11,16(r3)

;; fn0000ECF8: 0000ECF8
;;   Called from:
;;     0000F020 (in fn0000EDF4)
fn0000ECF8 proc
	l.sw	-24(r1),r2
	l.sw	-20(r1),r14
	l.sw	-12(r1),r18
	l.sw	-8(r1),r20
	l.sw	-4(r1),r9
	l.sw	-16(r1),r16
	l.ori	r18,r4,00000000
	l.addi	r1,r1,-0000002C
	l.movhi	r4,00000001
	l.ori	r20,r5,00000000
	l.ori	r14,r3,00000000
	l.ori	r4,r4,00002E71
	l.ori	r3,r1,00000000
	l.addi	r5,r0,+00000011
	l.jal	0000DF10
	l.addi	r2,r0,+00000000
	l.sfges	r18,r20
	l.bf	0000ED54
	l.ori	r3,r14,00000000

l0000ED44:
	l.sub	r2,r20,r18
	l.ori	r4,r1,00000000
	l.jal	0000DE50
	l.ori	r5,r2,00000000

l0000ED54:
	l.addi	r1,r1,+0000002C
	l.ori	r11,r2,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-24(r1)
	l.lwz	r14,-20(r1)
	l.lwz	r16,-16(r1)
	l.lwz	r18,-12(r1)
	l.jr	r9
	l.lwz	r20,-8(r1)

;; fn0000ED78: 0000ED78
;;   Called from:
;;     0000E9F4 (in fn0000E98C)
fn0000ED78 proc
	l.sw	-4(r1),r9
	l.jal	0000D640
	l.addi	r1,r1,-00000004
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000
0000ED94             44 00 48 00 9D 60 00 00 B8 63 00 18     D.H..`...c..
0000EDA0 D7 E1 4F FC 9C 21 FF FC 07 FF FA 97 B8 63 00 98 ..O..!.......c..
0000EDB0 9C 21 00 04 85 21 FF FC 44 00 48 00 9D 60 00 00 .!...!..D.H..`..
0000EDC0 D7 E1 4F FC 9C 21 FF FC 9C 21 00 04 85 21 FF FC ..O..!...!...!..
0000EDD0 03 FF FA A7 15 00 00 00 D7 E1 4F FC 07 FF FA BF ..........O.....
0000EDE0 9C 21 FF FC 9C 21 00 04 85 21 FF FC 44 00 48 00 .!...!...!..D.H.
0000EDF0 9D 60 00 00                                     .`..           

;; fn0000EDF4: 0000EDF4
;;   Called from:
;;     00009A20 (in fn0000998C)
;;     00009B40 (in fn0000998C)
;;     0000A77C (in fn0000A708)
;;     0000AA60 (in fn0000A92C)
;;     0000AB20 (in fn0000AADC)
;;     0000AC4C (in fn0000AC14)
;;     0000DD8C (in fn0000DD60)
;;     0000DDB0 (in fn0000DD60)
;;     0000DDD0 (in fn0000DD60)
;;     0000DE00 (in fn0000DD60)
;;     0000E5F8 (in fn0000E5CC)
;;     0000E61C (in fn0000E5CC)
;;     0000E654 (in fn0000E5CC)
;;     0000E7B0 (in fn0000E758)
;;     0000E7C4 (in fn0000E758)
;;     0000E7E4 (in fn0000E758)
;;     0000E800 (in fn0000E758)
;;     0000EA04 (in fn0000E98C)
;;     0000EA94 (in fn0000E98C)
;;     0000EB20 (in fn0000E98C)
;;     0000EB38 (in fn0000E98C)
;;     0000EB54 (in fn0000E98C)
;;     0000EB8C (in fn0000E98C)
;;     00010674 (in fn00010570)
;;     000114F4 (in fn00010570)
;;     00012054 (in fn00010570)
;;     00012108 (in fn00010570)
;;     000121EC (in fn00010570)
;;     000123F4 (in fn00012320)
fn0000EDF4 proc
	l.sw	-32(r1),r2
	l.movhi	r2,00000001
	l.sw	-24(r1),r16
	l.ori	r2,r2,00003150
	l.sw	-4(r1),r9
	l.lwz	r11,0(r2)
	l.addi	r2,r0,+000000F0
	l.addi	r11,r11,+00000001
	l.sw	-28(r1),r14
	l.sra	r11,r2,r11
	l.sw	-20(r1),r18
	l.sw	-16(r1),r20
	l.sw	-12(r1),r22
	l.sw	-8(r1),r24
	l.and	r11,r11,r3
	l.addi	r1,r1,-0000003C
	l.sfeqi	r11,00000000
	l.bf	0000F118
	l.ori	r16,r4,00000000

l0000EE40:
	l.jal	0000E718
	l.movhi	r2,00000001
	l.ori	r2,r2,00003154
	l.ori	r22,r11,00000000
	l.lwz	r2,0(r2)
	l.sfeqi	r2,00000000
	l.bf	0000EF4C
	l.nop

l0000EE60:
	l.jal	0000E8BC
	l.addi	r2,r0,+0000005B
	l.add	r20,r11,r11
	l.slli	r11,r11,00000003
	l.addi	r3,r0,+0000002E
	l.add	r20,r20,r11
	l.sb	16(r1),r2
	l.sb	21(r1),r3
	l.addi	r2,r0,+0000005D
	l.addi	r3,r0,+00000020
	l.sb	24(r1),r2
	l.sb	25(r1),r3
	l.addi	r2,r0,+00000000
	l.ori	r3,r20,00000000
	l.addi	r4,r0,+000003E8
	l.jal	0000FEDC
	l.sb	26(r1),r2
	l.addi	r4,r0,+00002710
	l.ori	r3,r11,00000000
	l.jal	0001003C
	l.addi	r14,r1,+00000011
	l.addi	r24,r1,+00000015
	l.ori	r18,r11,00000000
	l.addi	r2,r0,+000003E8

l0000EEC0:
	l.ori	r3,r18,00000000
	l.jal	0000FEDC
	l.ori	r4,r2,00000000
	l.addi	r4,r11,+00000030
	l.ori	r3,r18,00000000
	l.sb	0(r14),r4
	l.ori	r4,r2,00000000
	l.jal	0001003C
	l.addi	r14,r14,+00000001
	l.ori	r3,r2,00000000
	l.addi	r4,r0,+0000000A
	l.jal	0000FEDC
	l.ori	r18,r11,00000000
	l.sfne	r14,r24
	l.bf	0000EEC0
	l.ori	r2,r11,00000000

l0000EF00:
	l.ori	r3,r20,00000000
	l.jal	0001003C
	l.addi	r4,r0,+000003E8
	l.addi	r4,r0,+00000064
	l.ori	r3,r11,00000000
	l.jal	0000FEDC
	l.ori	r2,r11,00000000
	l.addi	r4,r11,+00000030
	l.ori	r3,r2,00000000
	l.sb	22(r1),r4
	l.jal	0001003C
	l.addi	r4,r0,+00000064
	l.addi	r4,r0,+0000000A
	l.jal	0000FEDC
	l.ori	r3,r11,00000000
	l.addi	r3,r1,+00000010
	l.addi	r2,r11,+00000030
	l.jal	0000D8D8
	l.sb	23(r1),r2

l0000EF4C:
	l.movhi	r2,00000001
	l.addi	r6,r1,+0000003C
	l.j	0000F0B4
	l.ori	r2,r2,00003984

l0000EF5C:
	l.bf	0000F0A8
	l.nop

l0000EF64:
	l.addi	r16,r16,+00000001
	l.lbs	r4,0(r16)
	l.addi	r3,r4,-00000031
	l.andi	r3,r3,000000FF
	l.sfgtui	r3,00000008
	l.bf	0000EF8C
	l.addi	r18,r0,+00000000

l0000EF80:
	l.addi	r18,r4,-00000030
	l.addi	r16,r16,+00000001
	l.lbs	r4,0(r16)

l0000EF8C:
	l.sfeqi	r4,00000070
	l.bf	0000EFF4
	l.sfgtsi	r4,+00000070

l0000EF98:
	l.bf	0000EFBC
	l.sfeqi	r4,00000075

l0000EFA0:
	l.sfeqi	r4,00000063
	l.bf	0000F05C
	l.sfeqi	r4,00000064

l0000EFAC:
	l.bnf	0000F090
	l.nop

l0000EFB4:
	l.j	0000EFDC
	l.lwz	r3,0(r6)

l0000EFBC:
	l.bf	0000F048
	l.sfeqi	r4,00000078

l0000EFC4:
	l.bf	0000EFF4
	l.sfeqi	r4,00000073

l0000EFCC:
	l.bnf	0000F090
	l.nop

l0000EFD4:
	l.j	0000F06C
	l.lwz	r18,0(r6)

l0000EFDC:
	l.ori	r4,r1,00000000
	l.addi	r5,r0,+0000000A
	l.jal	0000DB40
	l.addi	r24,r6,+00000004
	l.j	0000F00C
	l.nop

l0000EFF4:
	l.addi	r24,r6,+00000004
	l.lwz	r3,0(r6)
	l.ori	r4,r1,00000000
	l.addi	r5,r0,+00000010

l0000F004:
	l.jal	0000DC70
	l.nop

l0000F00C:
	l.jal	0000DE08
	l.ori	r3,r1,00000000
	l.ori	r5,r18,00000000
	l.ori	r3,r1,00000000
	l.ori	r4,r11,00000000
	l.jal	0000ECF8
	l.ori	r20,r11,00000000
	l.ori	r18,r11,00000000
	l.ori	r3,r2,00000000
	l.ori	r4,r1,00000000
	l.jal	0000DE28
	l.add	r20,r20,r18
	l.add	r2,r2,r20
	l.j	0000F0B0
	l.ori	r6,r24,00000000

l0000F048:
	l.addi	r24,r6,+00000004
	l.lwz	r3,0(r6)
	l.ori	r4,r1,00000000
	l.j	0000F004
	l.addi	r5,r0,+0000000A

l0000F05C:
	l.lwz	r3,0(r6)
	l.addi	r6,r6,+00000004
	l.j	0000F0B0
	l.sb	0(r2),r3

l0000F06C:
	l.ori	r3,r2,00000000
	l.ori	r4,r18,00000000
	l.jal	0000DE28
	l.addi	r20,r6,+00000004
	l.jal	0000DE08
	l.ori	r3,r18,00000000
	l.ori	r6,r20,00000000
	l.j	0000F0B0
	l.add	r2,r2,r11

l0000F090:
	l.addi	r3,r0,+00000025
	l.sb	0(r2),r3
	l.lbz	r3,0(r16)
	l.sb	1(r2),r3
	l.j	0000F0B0
	l.addi	r2,r2,+00000002

l0000F0A8:
	l.sb	0(r2),r4
	l.addi	r2,r2,+00000001

l0000F0B0:
	l.addi	r16,r16,+00000001

l0000F0B4:
	l.lbs	r4,0(r16)
	l.sfnei	r4,00000000
	l.bf	0000EF5C
	l.sfnei	r4,00000025

l0000F0C4:
	l.lbs	r3,-1(r2)
	l.movhi	r5,00000001
	l.sfnei	r3,0000000A
	l.bf	0000F0E8
	l.ori	r5,r5,00003154

l0000F0D8:
	l.addi	r3,r0,+00000001
	l.sw	0(r5),r3
	l.j	0000F0F0
	l.addi	r3,r0,+00000000

l0000F0E8:
	l.sw	0(r5),r4
	l.addi	r3,r0,+00000000

l0000F0F0:
	l.sb	0(r2),r3
	l.movhi	r3,00000001
	l.addi	r2,r2,+00000001
	l.jal	0000D8D8
	l.ori	r3,r3,00003984
	l.jal	0000E740
	l.ori	r3,r22,00000000
	l.movhi	r3,00000001
	l.ori	r3,r3,00003984
	l.sub	r11,r2,r3

l0000F118:
	l.addi	r1,r1,+0000003C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-32(r1)
	l.lwz	r14,-28(r1)
	l.lwz	r16,-24(r1)
	l.lwz	r18,-20(r1)
	l.lwz	r20,-16(r1)
	l.lwz	r22,-12(r1)
	l.jr	r9
	l.lwz	r24,-8(r1)

;; fn0000F140: 0000F140
;;   Called from:
;;     0000F79C (in fn0000F444)
fn0000F140 proc
	l.movhi	r4,00000001
	l.addi	r11,r0,+00000000
	l.ori	r4,r4,00003150
	l.jr	r9
	l.sw	0(r4),r3

;; fn0000F154: 0000F154
;;   Called from:
;;     000086A8 (in fn00008674)
;;     00008700 (in fn00008674)
;;     0000F2C4 (in fn0000F250)
;;     0000F378 (in fn0000F368)
fn0000F154 proc
	l.movhi	r4,00000001
	l.ori	r4,r4,0000377C
	l.lwz	r4,0(r4)
	l.sfltu	r3,r4
	l.bf	0000F18C
	l.addi	r11,r0,+00000000

l0000F16C:
	l.movhi	r5,00000001
	l.ori	r5,r5,00003780
	l.lwz	r5,0(r5)
	l.sfltu	r3,r5
	l.bf	0000F188
	l.addi	r4,r0,+00000001

l0000F184:
	l.ori	r4,r11,00000000

l0000F188:
	l.ori	r11,r4,00000000

l0000F18C:
	l.jr	r9
	l.nop

;; fn0000F194: 0000F194
;;   Called from:
;;     0000EA54 (in fn0000E98C)
fn0000F194 proc
	l.movhi	r3,00000001
	l.movhi	r4,00000001
	l.sw	-12(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.sw	-8(r1),r14
	l.ori	r3,r3,00003774
	l.addi	r1,r1,-0000000C
	l.ori	r4,r4,00003784
	l.jal	0000ECA8
	l.ori	r2,r2,00003774
	l.lwz	r3,0(r2)
	l.movhi	r2,00000001
	l.movhi	r5,00000001
	l.ori	r2,r2,00003784
	l.ori	r5,r5,00003780
	l.lwz	r4,0(r2)
	l.movhi	r14,00000001
	l.add	r4,r3,r4
	l.ori	r14,r14,0000377C
	l.sw	0(r5),r4
	l.jal	00004570
	l.sw	0(r14),r3
	l.lwz	r3,0(r14)
	l.lwz	r5,0(r2)
	l.addi	r4,r0,+00000000
	l.jal	0000DFCC
	l.addi	r2,r0,+00000000
	l.movhi	r3,00000001
	l.ori	r3,r3,00003A04
	l.sw	4(r3),r2
	l.sw	8(r3),r2
	l.sw	12(r3),r2
	l.sw	16(r3),r2
	l.sw	0(r3),r2
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000000
	l.ori	r2,r2,00003778
	l.ori	r11,r3,00000000
	l.sw	0(r2),r3
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)
0000F248                         44 00 48 00 9D 60 00 00         D.H..`..

;; fn0000F250: 0000F250
;;   Called from:
;;     0000EA9C (in fn0000E98C)
fn0000F250 proc
	l.sw	-8(r1),r2
	l.sw	-4(r1),r9
	l.jal	0000E718
	l.addi	r1,r1,-00000008
	l.movhi	r4,00000001
	l.ori	r3,r11,00000000
	l.ori	r4,r4,00003A04
	l.lwz	r2,0(r4)
	l.sfeqi	r2,00000000
	l.bf	0000F2BC
	l.nop

l0000F27C:
	l.lwz	r2,0(r4)
	l.movhi	r5,00000001
	l.addi	r2,r2,-00000001
	l.ori	r5,r5,00003A04
	l.sw	0(r4),r2
	l.lwz	r2,0(r4)
	l.addi	r2,r2,+00000001
	l.slli	r2,r2,00000002
	l.add	r2,r2,r4
	l.lwz	r2,0(r2)
	l.lwz	r4,0(r4)
	l.addi	r4,r4,+00000001
	l.slli	r4,r4,00000002
	l.add	r4,r4,r5
	l.addi	r5,r0,+00000000
	l.sw	0(r4),r5

l0000F2BC:
	l.jal	0000E740
	l.nop
	l.jal	0000F154
	l.ori	r3,r2,00000000
	l.sfnei	r11,00000000
	l.bf	0000F334
	l.ori	r3,r11,00000000

l0000F2D8:
	l.jal	00008834
	l.addi	r4,r0,+00000064
	l.movhi	r3,00000001
	l.movhi	r4,00000001
	l.ori	r3,r3,0000377C
	l.ori	r4,r4,00003780
	l.lwz	r3,0(r3)
	l.j	0000F320
	l.lwz	r4,0(r4)

l0000F2FC:
	l.lbz	r5,0(r3)
	l.sfnei	r5,00000000
	l.bf	0000F320
	l.addi	r3,r3,+00000080

l0000F30C:
	l.addi	r3,r3,-00000080
	l.addi	r2,r0,+00000001
	l.sb	0(r3),r2
	l.j	0000F32C
	l.ori	r2,r3,00000000

l0000F320:
	l.sfltu	r3,r4
	l.bf	0000F2FC
	l.nop

l0000F32C:
	l.jal	000088FC
	l.addi	r3,r0,+00000000

l0000F334:
	l.addi	r4,r0,+00000000
	l.addi	r3,r0,+00000000
	l.ori	r11,r2,00000000
	l.sw	8(r2),r3
	l.sw	12(r2),r4
	l.sw	32(r2),r3
	l.sw	36(r2),r4
	l.addi	r4,r0,+00000000
	l.sb	1(r2),r4
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn0000F368: 0000F368
;;   Called from:
;;     0000EB0C (in fn0000E98C)
;;     0000F848 (in fn0000F444)
;;     00012180 (in fn00010570)
fn0000F368 proc
	l.sw	-12(r1),r2
	l.sw	-4(r1),r9
	l.sw	-8(r1),r14
	l.addi	r1,r1,-0000000C
	l.jal	0000F154
	l.ori	r2,r3,00000000
	l.sfeqi	r11,00000000
	l.bf	0000F430
	l.nop

l0000F38C:
	l.jal	0000E718
	l.nop
	l.movhi	r4,00000001
	l.ori	r14,r11,00000000
	l.ori	r4,r4,00003A04
	l.lwz	r3,0(r4)
	l.sfgtui	r3,00000003
	l.bf	0000F3EC
	l.addi	r6,r0,+00000000

l0000F3B0:
	l.addi	r5,r0,+00000000
	l.sw	8(r2),r5
	l.sw	12(r2),r6
	l.addi	r6,r0,+00000001
	l.sb	0(r2),r6
	l.lwz	r3,0(r4)
	l.addi	r3,r3,+00000001
	l.slli	r3,r3,00000002
	l.add	r3,r3,r4
	l.sw	0(r3),r2
	l.lwz	r2,0(r4)
	l.addi	r2,r2,+00000001
	l.sw	0(r4),r2
	l.j	0000F41C
	l.addi	r1,r1,+0000000C

l0000F3EC:
	l.addi	r3,r0,+00000000
	l.jal	00008834
	l.addi	r4,r0,+00000064
	l.addi	r3,r0,+00000000
	l.addi	r4,r0,+00000000
	l.sb	0(r2),r3
	l.addi	r5,r0,+00000000
	l.sw	8(r2),r4
	l.sw	12(r2),r5
	l.jal	000088FC
	l.addi	r3,r0,+00000000
	l.addi	r1,r1,+0000000C

l0000F41C:
	l.ori	r3,r14,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.j	0000E740
	l.lwz	r14,-8(r1)

l0000F430:
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn0000F444: 0000F444
fn0000F444 proc
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000010
	l.ori	r14,r3,00000000
	l.jal	0000E718
	l.addi	r2,r0,+00000004
	l.lbz	r4,2(r14)
	l.sb	0(r14),r2
	l.sfeqi	r4,00000059
	l.bf	0000F75C
	l.ori	r16,r11,00000000

l0000F478:
	l.sfgtui	r4,00000059
	l.bf	0000F52C
	l.sfeqi	r4,00000061

l0000F484:
	l.sfeqi	r4,00000030
	l.bf	0000F5F4
	l.sfgtui	r4,00000030

l0000F490:
	l.bf	0000F4E0
	l.sfeqi	r4,00000053

l0000F498:
	l.sfeqi	r4,00000020
	l.bf	0000F688
	l.sfgtui	r4,00000020

l0000F4A4:
	l.bf	0000F4C8
	l.sfeqi	r4,00000022

l0000F4AC:
	l.sfeqi	r4,00000016
	l.bf	0000F604
	l.sfeqi	r4,00000019

l0000F4B8:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F4C0:
	l.j	0000F678
	l.nop

l0000F4C8:
	l.bf	0000F5D4
	l.sfeqi	r4,00000024

l0000F4D0:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F4D8:
	l.j	0000F5E4
	l.nop

l0000F4E0:
	l.bf	0000F6E0
	l.sfgtui	r4,00000053

l0000F4E8:
	l.bf	0000F50C
	l.sfeqi	r4,00000055

l0000F4F0:
	l.sfeqi	r4,00000051
	l.bf	0000F698
	l.sfeqi	r4,00000052

l0000F4FC:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F504:
	l.j	0000F6A8
	l.nop

l0000F50C:
	l.bf	0000F6F0
	l.sfltui	r4,00000055

l0000F514:
	l.bf	0000F74C
	l.sfeqi	r4,00000056

l0000F51C:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F524:
	l.j	0000F704
	l.nop

l0000F52C:
	l.bf	0000F7F8
	l.addi	r2,r0,+00000000

l0000F534:
	l.sfgtui	r4,00000061
	l.bf	0000F588
	l.sfeqi	r4,00000067

l0000F540:
	l.sfeqi	r4,0000005C
	l.bf	0000F714
	l.sfgtui	r4,0000005C

l0000F54C:
	l.bf	0000F570
	l.sfeqi	r4,0000005D

l0000F554:
	l.sfeqi	r4,0000005A
	l.bf	0000F6C0
	l.sfeqi	r4,0000005B

l0000F560:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F568:
	l.j	0000F6D0
	l.nop

l0000F570:
	l.bf	0000F728
	l.sfeqi	r4,00000060

l0000F578:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F580:
	l.j	0000F79C
	l.nop

l0000F588:
	l.bf	0000F7D0
	l.sfgtui	r4,00000067

l0000F590:
	l.bf	0000F5B4
	l.sfeqi	r4,00000081

l0000F598:
	l.sfeqi	r4,00000062
	l.bf	0000F7AC
	l.sfeqi	r4,00000064

l0000F5A4:
	l.bnf	0000F7E0
	l.nop

l0000F5AC:
	l.j	0000F7BC
	l.lwz	r3,40(r14)

l0000F5B4:
	l.bf	0000F77C
	l.sfeqi	r4,00000082

l0000F5BC:
	l.bf	0000F78C
	l.sfeqi	r4,00000080

l0000F5C4:
	l.bnf	0000F7E4
	l.movhi	r3,00000001

l0000F5CC:
	l.j	0000F76C
	l.nop

l0000F5D4:
	l.jal	00012214
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F5E4:
	l.jal	000122B8
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F5F4:
	l.jal	0000E1AC
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F604:
	l.movhi	r2,00000001
	l.ori	r2,r2,00003778
	l.lwz	r3,0(r2)
	l.sfeqi	r3,00000003
	l.bf	0000F660
	l.addi	r3,r0,+00000003

l0000F61C:
	l.movhi	r5,00000001
	l.sw	0(r2),r3
	l.movhi	r2,00000001
	l.movhi	r3,00000001
	l.ori	r2,r2,00003980
	l.ori	r5,r5,00002476
	l.lwz	r4,0(r2)
	l.movhi	r2,00000001
	l.ori	r3,r3,000000B8
	l.ori	r2,r2,000000B8
	l.add	r4,r4,r2
	l.jal	0000DF10
	l.sub	r5,r5,r2
	l.jal	0000FA94
	l.nop
	l.jal	0000C8A0
	l.addi	r3,r0,+000003E8

l0000F660:
	l.jal	0000AEDC
	l.movhi	r3,0000F3F3
	l.jal	00010570
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F678:
	l.jal	00012320
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F688:
	l.jal	0000E50C
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F698:
	l.jal	00008B78
	l.addi	r3,r0,+00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F6A8:
	l.jal	00008CA4
	l.addi	r3,r0,+00000000
	l.jal	00008B60
	l.addi	r3,r0,+00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F6C0:
	l.jal	00008CA4
	l.ori	r3,r2,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F6D0:
	l.jal	00008B90
	l.lwz	r3,40(r14)
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F6E0:
	l.jal	0000A364
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F6F0:
	l.lwz	r3,40(r14)
	l.jal	00009C2C
	l.lwz	r4,44(r14)
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F704:
	l.jal	00009DDC
	l.lwz	r3,40(r14)
	l.j	0000F734
	l.ori	r2,r11,00000000

l0000F714:
	l.lwz	r3,40(r14)
	l.jal	00009ED0
	l.lwz	r4,44(r14)
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F728:
	l.jal	0000A070
	l.lwz	r3,40(r14)
	l.ori	r2,r11,00000000

l0000F734:
	l.sfltsi	r11,+00000000
	l.bf	0000F7FC
	l.addi	r3,r0,+00000005

l0000F740:
	l.sw	44(r14),r11
	l.j	0000F7FC
	l.addi	r2,r0,+00000000

l0000F74C:
	l.jal	0000A5D0
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F75C:
	l.jal	0000A634
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F76C:
	l.jal	0000D31C
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F77C:
	l.jal	0000D408
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F78C:
	l.jal	0000D49C
	l.ori	r3,r14,00000000
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F79C:
	l.jal	0000F140
	l.lwz	r3,40(r14)
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F7AC:
	l.jal	0000D95C
	l.lwz	r3,40(r14)
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F7BC:
	l.lwz	r4,44(r14)
	l.jal	0000E590
	l.lwz	r5,48(r14)
	l.j	0000F7F8
	l.ori	r2,r11,00000000

l0000F7D0:
	l.jal	0000EC54
	l.nop
	l.j	0000F7FC
	l.addi	r3,r0,+00000005

l0000F7E0:
	l.movhi	r3,00000001

l0000F7E4:
	l.ori	r4,r14,00000000
	l.ori	r3,r3,00002E82
	l.addi	r5,r0,+00000080
	l.jal	0000DD60
	l.addi	r2,r0,-00000003

l0000F7F8:
	l.addi	r3,r0,+00000005

l0000F7FC:
	l.sb	3(r14),r2
	l.sb	0(r14),r3
	l.lbz	r3,1(r14)
	l.andi	r3,r3,00000003
	l.sfeqi	r3,00000000
	l.bf	0000F820
	l.ori	r3,r14,00000000

l0000F818:
	l.jal	00008574
	l.addi	r4,r0,+00000FA0

l0000F820:
	l.lbz	r3,1(r14)
	l.sfnei	r3,00000000
	l.bf	0000F850
	l.sfeqi	r2,00000000

l0000F830:
	l.bf	0000F848
	l.ori	r3,r14,00000000

l0000F838:
	l.jal	00008574
	l.addi	r4,r0,+00000FA0
	l.j	0000F850
	l.nop

l0000F848:
	l.jal	0000F368
	l.ori	r3,r14,00000000

l0000F850:
	l.jal	0000E740
	l.ori	r3,r16,00000000
	l.addi	r1,r1,+00000010
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn0000F874: 0000F874
;;   Called from:
;;     0000869C (in fn00008674)
;;     000086F4 (in fn00008674)
fn0000F874 proc
	l.movhi	r4,00000001
	l.ori	r4,r4,00003774
	l.lwz	r11,0(r4)
	l.jr	r9
	l.add	r11,r3,r11

;; fn0000F888: 0000F888
;;   Called from:
;;     00008498 (in fn00008428)
;;     00008518 (in fn00008428)
;;     000085DC (in fn00008574)
;;     00008644 (in fn00008574)
fn0000F888 proc
	l.movhi	r4,00000001
	l.ori	r4,r4,00003774
	l.lwz	r11,0(r4)
	l.jr	r9
	l.sub	r11,r3,r11

;; fn0000F89C: 0000F89C
;;   Called from:
;;     0000E994 (in fn0000E98C)
fn0000F89C proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r2
	l.ori	r3,r3,00003788
	l.addi	r2,r0,+00000000
	l.addi	r1,r1,-00000004
	l.sw	0(r3),r2
	l.sw	12(r3),r2
	l.sw	24(r3),r2
	l.sw	36(r3),r2
	l.sw	48(r3),r2
	l.sw	60(r3),r2
	l.sw	72(r3),r2
	l.sw	84(r3),r2
	l.addi	r1,r1,+00000004
	l.ori	r11,r2,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)
0000F8E0 44 00 48 00 9D 60 00 00                         D.H..`..       

;; fn0000F8E8: 0000F8E8
;;   Called from:
;;     0000B4E8 (in fn0000B4D4)
;;     0000BAC0 (in fn0000BAA8)
;;     0000E984 (in fn0000E96C)
fn0000F8E8 proc
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000010
	l.movhi	r2,00000001
	l.ori	r14,r3,00000000
	l.jal	0000E718
	l.ori	r16,r4,00000000
	l.ori	r2,r2,00003788
	l.ori	r3,r11,00000000
	l.addi	r5,r0,+00000000

l0000F918:
	l.lwz	r4,0(r2)
	l.sfnei	r4,00000000
	l.bf	0000F950
	l.addi	r5,r5,+00000001

l0000F928:
	l.addi	r5,r5,-00000001
	l.add	r2,r5,r5
	l.movhi	r4,00000001
	l.add	r2,r2,r5
	l.ori	r4,r4,00003788
	l.slli	r2,r2,00000002
	l.add	r2,r2,r4
	l.addi	r4,r0,+00000001
	l.j	0000F960
	l.sw	0(r2),r4

l0000F950:
	l.sfnei	r5,00000008
	l.bf	0000F918
	l.addi	r2,r2,+0000000C

l0000F95C:
	l.addi	r2,r0,+00000000

l0000F960:
	l.jal	0000E740
	l.nop
	l.sfeqi	r2,00000000
	l.bf	0000F988
	l.addi	r11,r0,-0000001C

l0000F974:
	l.lwz	r3,0(r14)
	l.sw	4(r2),r16
	l.sw	8(r2),r3
	l.sw	0(r14),r2
	l.addi	r11,r0,+00000000

l0000F988:
	l.addi	r1,r1,+00000010
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

l0000F9A0:
	l.sw	-4(r1),r2
	l.lwz	r6,0(r3)
	l.addi	r1,r1,-00000004
	l.sfeqi	r6,00000000
	l.bf	0000FA2C
	l.addi	r11,r0,-00000013

l0000F9B8:
	l.lwz	r5,8(r6)
	l.sfnei	r5,00000000
	l.bf	0000F9E8
	l.nop

l0000F9C8:
	l.lwz	r7,4(r6)
	l.sfne	r7,r4
	l.bf	0000FA2C
	l.addi	r11,r0,-00000024

l0000F9D8:
	l.sw	0(r3),r5
	l.j	0000FA1C
	l.ori	r5,r6,00000000

l0000F9E4:
	l.ori	r5,r3,00000000

l0000F9E8:
	l.lwz	r3,4(r5)
	l.sfne	r3,r4
	l.bf	0000FA04
	l.nop

l0000F9F8:
	l.lwz	r3,8(r5)
	l.j	0000FA1C
	l.sw	8(r6),r3

l0000FA04:
	l.lwz	r3,8(r5)
	l.sfnei	r3,00000000
	l.bf	0000F9E4
	l.ori	r6,r5,00000000

l0000FA14:
	l.j	0000FA2C
	l.addi	r11,r0,-00000024

l0000FA1C:
	l.addi	r2,r0,+00000000
	l.sw	0(r5),r2
	l.sw	8(r5),r2
	l.ori	r11,r2,00000000

l0000FA2C:
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn0000FA38: 0000FA38
;;   Called from:
;;     0000B5A0 (in fn0000B610)
;;     0000B5E8 (in fn0000B610)
;;     0000BAF4 (in fn0000BAC8)
;;     0000BC00 (in fn0000BB60)
;;     0000EBA0 (in fn0000E98C)
fn0000FA38 proc
	l.sw	-16(r1),r2
	l.sw	-12(r1),r14
	l.sw	-8(r1),r16
	l.sw	-4(r1),r9
	l.ori	r16,r4,00000000
	l.addi	r1,r1,-00000010
	l.ori	r14,r5,00000000
	l.j	0000FA6C
	l.lwz	r2,0(r3)

l0000FA5C:
	l.lwz	r11,4(r2)
	l.jalr	r11
	l.ori	r4,r14,00000000
	l.lwz	r2,8(r2)

l0000FA6C:
	l.sfnei	r2,00000000
	l.bf	0000FA5C
	l.ori	r3,r16,00000000

l0000FA78:
	l.addi	r1,r1,+00000010
	l.ori	r11,r2,00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-16(r1)
	l.lwz	r14,-12(r1)
	l.jr	r9
	l.lwz	r16,-8(r1)

;; fn0000FA94: 0000FA94
;;   Called from:
;;     0000F650 (in fn0000F444)
fn0000FA94 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r3
	l.sw	-12(r1),r4
	l.sw	-16(r1),r5
	l.msync
	l.csync
	l.addi	r3,r0,+00000010
	l.addi	r4,r0,+00000000
	l.addi	r5,r0,+00001000

l0000FAB8:
	l.mtspr	r0,r4,00002002
	l.sfne	r4,r5
	l.bf	0000FAB8
	l.add	r4,r4,r3

l0000FAC8:
	l.psync
	l.lwz	r9,-4(r1)
	l.lwz	r3,-8(r1)
	l.lwz	r4,-12(r1)
	l.lwz	r5,-16(r1)
	l.jr	r9
	l.nop
0000FAE4             18 20 00 00 18 40 00 00 18 60 00 00     . ...@...`..
0000FAF0 18 80 00 00 18 A0 00 00 18 C0 00 00 18 E0 00 00 ................
0000FB00 19 00 00 00 19 20 00 00 19 40 00 00 19 60 00 00 ..... ...@...`..
0000FB10 19 80 00 00 19 A0 00 00 19 C0 00 00 19 E0 00 00 ................
0000FB20 1A 00 00 00 1A 20 00 00 1A 40 00 00 1A 60 00 00 ..... ...@...`..
0000FB30 1A 80 00 00 1A A0 00 00 1A C0 00 00 1A E0 00 00 ................
0000FB40 1B 00 00 00 1B 20 00 00 1B 40 00 00 1B 60 00 00 ..... ...@...`..
0000FB50 1B 80 00 00 1B A0 00 00 1B C0 00 00 1B E0 00 00 ................
0000FB60 A8 20 00 01 A8 21 00 80 C0 00 08 11 C1 40 00 00 . ...!.......@..
0000FB70 18 20 00 01 A8 21 3E 18 9C 40 FF FD E0 21 10 03 . ...!>..@...!..
0000FB80 B4 C0 00 11 9C A0 FF FF AC A5 00 10 E0 A6 28 03 ..............(.
0000FB90 C0 00 28 11 9D C0 00 10 9C C0 00 00 9C A0 10 00 ..(.............
0000FBA0 C0 80 30 02 E4 26 28 00 13 FF FF FE E0 C6 70 00 ..0..&(.......p.
0000FBB0 15 00 00 00 15 00 00 00 15 00 00 00 15 00 00 00 ................
0000FBC0 15 00 00 00 15 00 00 00 15 00 00 00 15 00 00 00 ................
0000FBD0 15 00 00 00 15 00 00 00 15 00 00 00 18 A0 00 01 ................
0000FBE0 A8 A5 31 58 18 C0 00 01 A8 C6 3A 18 D4 05 00 00 ..1X......:.....
0000FBF0 E4 85 30 00 13 FF FF FE 9C A5 00 04 07 FF FB 64 ..0............d
0000FC00 15 00 00 00 9C 6B 00 00 07 FF FA D2 15 00 00 00 .....k..........
0000FC10 9C 21 FF 00 D4 01 18 04 D4 01 20 08 D4 01 28 0C .!........ ...(.
0000FC20 9C 60 00 02 B4 80 00 20 9C A1 00 00 00 00 00 6E .`..... .......n
0000FC30 15 00 00 00 9C 21 FF 00 D4 01 18 04 D4 01 20 08 .....!........ .
0000FC40 D4 01 28 0C 9C 60 00 03 B4 80 00 20 9C A1 00 00 ..(..`..... ....
0000FC50 00 00 00 65 15 00 00 00 9C 21 FF 00 D4 01 18 04 ...e.....!......
0000FC60 D4 01 20 08 D4 01 28 0C 9C 60 00 04 B4 80 00 20 .. ...(..`..... 
0000FC70 9C A1 00 00 00 00 00 5C 15 00 00 00 9C 21 FF 00 .......\.....!..
0000FC80 D4 01 18 04 D4 01 20 08 D4 01 28 0C 9C 60 00 05 ...... ...(..`..
0000FC90 B4 80 00 20 9C A1 00 00 00 00 00 53 15 00 00 00 ... .......S....
0000FCA0 9C 21 FF 00 D4 01 18 04 D4 01 20 08 D4 01 28 0C .!........ ...(.
0000FCB0 9C 60 00 06 B4 80 00 20 9C A1 00 00 00 00 00 4A .`..... .......J
0000FCC0 15 00 00 00 9C 21 FF 00 D4 01 18 04 D4 01 20 08 .....!........ .
0000FCD0 D4 01 28 0C 9C 60 00 07 B4 80 00 20 9C A1 00 00 ..(..`..... ....
0000FCE0 00 00 00 41 15 00 00 00 9C 21 FF 00 D4 01 18 04 ...A.....!......
0000FCF0 D4 01 20 08 D4 01 28 0C 9C 60 00 08 B4 80 00 20 .. ...(..`..... 
0000FD00 9C A1 00 00 00 00 00 38 15 00 00 00 9C 21 FF 00 .......8.....!..
0000FD10 D4 01 18 04 D4 01 20 08 D4 01 28 0C 9C 60 00 09 ...... ...(..`..
0000FD20 B4 80 00 20 9C A1 00 00 00 00 00 2F 15 00 00 00 ... ......./....
0000FD30 9C 21 FF 00 D4 01 18 04 D4 01 20 08 D4 01 28 0C .!........ ...(.
0000FD40 9C 60 00 0A B4 80 00 20 9C A1 00 00 00 00 00 26 .`..... .......&
0000FD50 15 00 00 00 9C 21 FF 00 D4 01 18 04 D4 01 20 08 .....!........ .
0000FD60 D4 01 28 0C 9C 60 00 0B B4 80 00 20 9C A1 00 00 ..(..`..... ....
0000FD70 00 00 00 1D 15 00 00 00 9C 21 FF 00 D4 01 18 04 .........!......
0000FD80 D4 01 20 08 D4 01 28 0C 9C 60 00 0C B4 80 00 20 .. ...(..`..... 
0000FD90 9C A1 00 00 00 00 00 14 15 00 00 00 9C 21 FF 00 .............!..
0000FDA0 D4 01 18 04 D4 01 20 08 D4 01 28 0C 9C 60 00 0D ...... ...(..`..
0000FDB0 B4 80 00 20 9C A1 00 00 00 00 00 0B 15 00 00 00 ... ............
0000FDC0 9C 21 FF 00 D4 01 18 04 D4 01 20 08 D4 01 28 0C .!........ ...(.
0000FDD0 9C 60 00 0E B4 80 00 20 9C A1 00 00 00 00 00 02 .`..... ........
0000FDE0 15 00 00 00 D4 01 10 00 D4 01 30 10 D4 01 38 14 ..........0...8.
0000FDF0 D4 01 40 18 D4 01 48 1C D4 01 50 20 D4 01 58 24 ..@...H...P ..X$
0000FE00 D4 01 60 28 D4 01 68 2C D4 01 70 30 D4 01 78 34 ..`(..h,..p0..x4
0000FE10 D4 01 80 38 D4 01 88 3C D4 01 90 40 D4 01 98 44 ...8...<...@...D
0000FE20 D4 01 A0 48 D4 01 A8 4C D4 01 B0 50 D4 01 B8 54 ...H...L...P...T
0000FE30 D4 01 C0 58 D4 01 C8 5C D4 01 D0 60 D4 01 D8 64 ...X...\...`...d
0000FE40 D4 01 E0 68 D4 01 E8 6C D4 01 F0 70 D4 01 F8 74 ...h...l...p...t
0000FE50 07 FF FA 42 15 00 00 00 84 41 00 00 84 61 00 04 ...B.....A...a..
0000FE60 84 81 00 08 84 A1 00 0C 84 C1 00 10 84 E1 00 14 ................
0000FE70 85 01 00 18 85 21 00 1C 85 41 00 20 85 61 00 24 .....!...A. .a.$
0000FE80 85 81 00 28 85 A1 00 2C 85 C1 00 30 85 E1 00 34 ...(...,...0...4
0000FE90 86 01 00 38 86 21 00 3C 86 41 00 40 86 61 00 44 ...8.!.<.A.@.a.D
0000FEA0 86 81 00 48 86 A1 00 4C 86 C1 00 50 86 E1 00 54 ...H...L...P...T
0000FEB0 87 01 00 58 87 21 00 5C 87 41 00 60 87 61 00 64 ...X.!.\.A.`.a.d
0000FEC0 87 81 00 68 87 A1 00 6C 87 C1 00 70 87 E1 00 74 ...h...l...p...t
0000FED0 9C 21 01 00 24 00 00 00 15 00 00 00             .!..$.......   

;; fn0000FEDC: 0000FEDC
;;   Called from:
;;     00005300 (in fn000051E8)
;;     0000533C (in fn000051E8)
;;     0000538C (in fn000051E8)
;;     000053C0 (in fn000051E8)
;;     000053F0 (in fn000051E8)
;;     0000545C (in fn000051E8)
;;     00005494 (in fn000051E8)
;;     000054C8 (in fn000051E8)
;;     0000551C (in fn000051E8)
;;     00005550 (in fn000051E8)
;;     00005590 (in fn000051E8)
;;     000055C8 (in fn000051E8)
;;     000055FC (in fn000051E8)
;;     00005620 (in fn000051E8)
;;     00005664 (in fn000051E8)
;;     0000569C (in fn000051E8)
;;     000056B8 (in fn000051E8)
;;     0000570C (in fn000051E8)
;;     0000575C (in fn000051E8)
;;     000057AC (in fn000051E8)
;;     00005814 (in fn000051E8)
;;     00005844 (in fn000051E8)
;;     00005880 (in fn000051E8)
;;     000058CC (in fn000051E8)
;;     00005924 (in fn000051E8)
;;     00005980 (in fn000051E8)
;;     0000599C (in fn000051E8)
;;     000059D8 (in fn000051E8)
;;     00005A28 (in fn000051E8)
;;     00005A78 (in fn000051E8)
;;     00005AE0 (in fn000051E8)
;;     00005B10 (in fn000051E8)
;;     00005B40 (in fn000051E8)
;;     00005B8C (in fn000051E8)
;;     00005BE4 (in fn000051E8)
;;     00005C40 (in fn000051E8)
;;     00005C5C (in fn000051E8)
;;     00005DB4 (in fn000051E8)
;;     00005DD0 (in fn000051E8)
;;     00005E4C (in fn000051E8)
;;     00005E68 (in fn000051E8)
;;     00005EE4 (in fn000051E8)
;;     00005F18 (in fn000051E8)
;;     00005F78 (in fn000051E8)
;;     00005FAC (in fn000051E8)
;;     000063F0 (in fn00006394)
;;     00006460 (in fn00006394)
;;     00009CBC (in fn00009C2C)
;;     00009CF8 (in fn00009C2C)
;;     0000AD44 (in fn0000AC84)
;;     0000AD54 (in fn0000AC84)
;;     0000AD60 (in fn0000AC84)
;;     0000B904 (in fn0000B8D8)
;;     0000C8E8 (in fn0000C8A0)
;;     0000CA08 (in fn0000C9C8)
;;     0000CA28 (in fn0000C9C8)
;;     0000CA40 (in fn0000C9C8)
;;     0000CA4C (in fn0000C9C8)
;;     0000D6E4 (in fn0000D640)
;;     0000D9C4 (in fn0000D95C)
;;     0000DCE8 (in fn0000DC70)
;;     0000E240 (in fn0000E1AC)
;;     0000E258 (in fn0000E1AC)
;;     0000EE9C (in fn0000EDF4)
;;     0000EEC4 (in fn0000EDF4)
;;     0000EEEC (in fn0000EDF4)
;;     0000EF14 (in fn0000EDF4)
;;     0000EF34 (in fn0000EDF4)
;;     00010014 (in fn0000FFD8)
;;     00010044 (in fn0001003C)
;;     00010090 (in fn0001005C)
fn0000FEDC proc
	l.addi	r1,r1,-00000004
	l.sw	0(r1),r9
	l.addi	r11,r0,+00000000
	l.addi	r8,r4,+00000000
	l.addi	r5,r3,+00000000
	l.sfne	r8,r11
	l.bnf	0000FFCC
	l.addi	r7,r0,+00000000

l0000FEFC:
	l.sfgtu	r8,r5
	l.bf	0000FFC8
	l.sfeq	r8,r5

l0000FF08:
	l.bf	0000FFC0
	l.sfltu	r11,r8

l0000FF10:
	l.bnf	0000FF44
	l.addi	r13,r0,+00000020

l0000FF18:
	l.movhi	r9,00008000
	l.addi	r6,r0,-00000001

l0000FF20:
	l.and	r3,r5,r9
	l.slli	r4,r7,00000001
	l.addi	r15,r5,+00000000
	l.srli	r3,r3,0000001F
	l.add	r13,r13,r6
	l.or	r7,r4,r3
	l.sfltu	r7,r8
	l.bf	0000FF20
	l.slli	r5,r5,00000001

l0000FF44:
	l.srli	r7,r7,00000001
	l.addi	r13,r13,+00000001
	l.addi	r9,r0,+00000000
	l.sfltu	r9,r13
	l.bnf	0000FFCC
	l.addi	r5,r15,+00000000

l0000FF5C:
	l.movhi	r15,00008000
	l.addi	r17,r0,+00000000

l0000FF64:
	l.and	r3,r5,r15
	l.slli	r4,r7,00000001
	l.srli	r3,r3,0000001F
	l.or	r7,r4,r3
	l.sub	r6,r7,r8
	l.and	r3,r6,r15
	l.srli	r3,r3,0000001F
	l.addi	r4,r0,+00000000
	l.sfne	r3,r4
	l.bf	0000FF94
	l.slli	r3,r11,00000001

l0000FF90:
	l.addi	r4,r0,+00000001

l0000FF94:
	l.slli	r5,r5,00000001
	l.sfne	r4,r17
	l.bnf	0000FFA8
	l.or	r11,r3,r4

l0000FFA4:
	l.addi	r7,r6,+00000000

l0000FFA8:
	l.addi	r9,r9,+00000001
	l.sfltu	r9,r13
	l.bf	0000FF64
	l.nop

l0000FFB8:
	l.j	0000FFCC
	l.nop

l0000FFC0:
	l.j	0000FFCC
	l.addi	r11,r0,+00000001

l0000FFC8:
	l.addi	r7,r5,+00000000

l0000FFCC:
	l.lwz	r9,0(r1)
	l.jr	r9
	l.addi	r1,r1,+00000004

;; fn0000FFD8: 0000FFD8
;;   Called from:
;;     0000AE0C (in fn0000ADBC)
;;     0000C0CC (in fn0000BF88)
;;     0000C9F4 (in fn0000C9C8)
;;     0000DBD8 (in fn0000DB40)
fn0000FFD8 proc
	l.addi	r1,r1,-00000008
	l.sw	0(r1),r9
	l.sw	4(r1),r14
	l.addi	r5,r3,+00000000
	l.addi	r14,r0,+00000000
	l.sflts	r5,r0
	l.bnf	00010000
	l.addi	r3,r0,+00000000

l0000FFF8:
	l.addi	r14,r0,+00000001
	l.sub	r5,r0,r5

l00010000:
	l.sflts	r4,r0
	l.bnf	00010014
	l.nop

l0001000C:
	l.addi	r14,r14,+00000001
	l.sub	r4,r0,r4

l00010014:
	l.jal	0000FEDC
	l.addi	r3,r5,+00000000
	l.sfeqi	r14,00000001
	l.bnf	0001002C
	l.nop

l00010028:
	l.sub	r11,r0,r11

l0001002C:
	l.lwz	r9,0(r1)
	l.lwz	r14,4(r1)
	l.jr	r9
	l.addi	r1,r1,+00000008

;; fn0001003C: 0001003C
;;   Called from:
;;     000052E8 (in fn000051E8)
;;     00005324 (in fn000051E8)
;;     0000536C (in fn000051E8)
;;     000053A4 (in fn000051E8)
;;     000053D8 (in fn000051E8)
;;     00005440 (in fn000051E8)
;;     00005474 (in fn000051E8)
;;     000054B0 (in fn000051E8)
;;     00005500 (in fn000051E8)
;;     00005538 (in fn000051E8)
;;     00005578 (in fn000051E8)
;;     000055B0 (in fn000051E8)
;;     000055E4 (in fn000051E8)
;;     0000560C (in fn000051E8)
;;     00005640 (in fn000051E8)
;;     0000567C (in fn000051E8)
;;     000056C8 (in fn000051E8)
;;     0000571C (in fn000051E8)
;;     0000576C (in fn000051E8)
;;     000057BC (in fn000051E8)
;;     000057F8 (in fn000051E8)
;;     0000582C (in fn000051E8)
;;     00005890 (in fn000051E8)
;;     000058DC (in fn000051E8)
;;     0000593C (in fn000051E8)
;;     00005960 (in fn000051E8)
;;     000059AC (in fn000051E8)
;;     000059E8 (in fn000051E8)
;;     00005A38 (in fn000051E8)
;;     00005A88 (in fn000051E8)
;;     00005AC4 (in fn000051E8)
;;     00005AF8 (in fn000051E8)
;;     00005B50 (in fn000051E8)
;;     00005B9C (in fn000051E8)
;;     00005BFC (in fn000051E8)
;;     00005C20 (in fn000051E8)
;;     00005C6C (in fn000051E8)
;;     00006418 (in fn00006394)
;;     00006444 (in fn00006394)
;;     0000DCC8 (in fn0000DC70)
;;     0000EB70 (in fn0000E98C)
;;     0000EEAC (in fn0000EDF4)
;;     0000EEDC (in fn0000EDF4)
;;     0000EF04 (in fn0000EDF4)
;;     0000EF28 (in fn0000EDF4)
fn0001003C proc
	l.addi	r1,r1,-00000004
	l.sw	0(r1),r9
	l.jal	0000FEDC
	l.nop
	l.addi	r11,r7,+00000000
	l.lwz	r9,0(r1)
	l.jr	r9
	l.addi	r1,r1,+00000004

;; fn0001005C: 0001005C
;;   Called from:
;;     0000DBB8 (in fn0000DB40)
fn0001005C proc
	l.addi	r1,r1,-00000008
	l.sw	0(r1),r9
	l.sw	4(r1),r14
	l.addi	r14,r0,+00000000
	l.sflts	r3,r0
	l.bnf	00010080
	l.nop

l00010078:
	l.addi	r14,r0,+00000001
	l.sub	r3,r0,r3

l00010080:
	l.sflts	r4,r0
	l.bnf	00010090
	l.nop

l0001008C:
	l.sub	r4,r0,r4

l00010090:
	l.jal	0000FEDC
	l.nop
	l.sfeqi	r14,00000001
	l.bnf	000100A8
	l.addi	r11,r7,+00000000

l000100A4:
	l.sub	r11,r0,r11

l000100A8:
	l.lwz	r9,0(r1)
	l.lwz	r14,4(r1)
	l.jr	r9
	l.addi	r1,r1,+00000008

;; fn000100B8: 000100B8
;;   Called from:
;;     00010858 (in fn00010570)
;;     00011F10 (in fn00010570)
fn000100B8 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r4,r0,+00000003
	l.addi	r1,r1,-0000000C
	l.addi	r3,r0,+00000004
	l.jal	0000B610
	l.movhi	r2,000001C2
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+00000014
	l.jal	0000B0B8
	l.addi	r4,r0,+00000001
	l.addi	r4,r0,+00000003
	l.jal	0000B610
	l.addi	r3,r0,+0000000A
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.ori	r3,r2,00000054
	l.addi	r5,r0,-00001000
	l.lwz	r4,0(r3)
	l.and	r4,r4,r5
	l.ori	r4,r4,00000010
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r4,r0,+00000003
	l.jal	0000B610
	l.addi	r3,r0,+0000000D
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.ori	r3,r2,00000058
	l.movhi	r5,0000FF00
	l.lwz	r4,0(r3)
	l.ori	r2,r2,000000CC
	l.and	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	0(r3),r4
	l.addi	r3,r0,+0000000B
	l.jal	0000B610
	l.addi	r4,r0,+00000017
	l.lwz	r3,0(r2)
	l.movhi	r4,0000FF0F
	l.sw	0(r1),r3
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r1)
	l.movhi	r5,000000A0
	l.and	r3,r3,r4
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.or	r3,r3,r5
	l.sw	0(r1),r3
	l.lwz	r3,0(r1)
	l.sw	0(r2),r3
	l.addi	r1,r1,+0000000C
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn000101A8: 000101A8
;;   Called from:
;;     000114C4 (in fn00010570)
fn000101A8 proc
	l.sw	-40(r1),r2
	l.movhi	r2,00000000
	l.sw	-4(r1),r9
	l.sw	-36(r1),r14
	l.sw	-32(r1),r16
	l.sw	-28(r1),r18
	l.sw	-24(r1),r20
	l.sw	-20(r1),r22
	l.sw	-16(r1),r24
	l.sw	-12(r1),r26
	l.sw	-8(r1),r28
	l.ori	r2,r2,00004358
	l.addi	r1,r1,-00000028

l000101DC:
	l.jal	00009B70
	l.nop
	l.sfnei	r11,00000000
	l.bf	00010200
	l.movhi	r3,00000000

l000101F0:
	l.lwz	r3,0(r2)
	l.slli	r3,r3,00000011
	l.sw	0(r2),r3
	l.movhi	r3,00000000

l00010200:
	l.addi	r2,r2,+00000004
	l.ori	r3,r3,000043A8
	l.sfne	r2,r3
	l.bf	000101DC
	l.movhi	r3,00000001

l00010214:
	l.addi	r16,r2,-00000004
	l.movhi	r14,00000001
	l.movhi	r22,00000001
	l.movhi	r26,00000001
	l.movhi	r28,00000001
	l.movhi	r24,00000001
	l.ori	r3,r3,00003744
	l.addi	r4,r0,+00000000
	l.addi	r5,r0,+00000017
	l.ori	r14,r14,00003544
	l.ori	r22,r22,00003742
	l.ori	r20,r16,00000000
	l.addi	r18,r0,+00000013
	l.ori	r26,r26,000034D0
	l.ori	r28,r28,00003938
	l.jal	0000DFCC
	l.ori	r24,r24,00003934

l00010258:
	l.lwz	r2,0(r26)
	l.srl	r2,r2,r18
	l.andi	r2,r2,00000001
	l.sfeqi	r2,00000000
	l.bf	000102E0
	l.nop

l00010270:
	l.j	000102D0
	l.lwz	r2,0(r28)

l00010278:
	l.lwz	r3,0(r20)
	l.srl	r3,r3,r2
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	000102CC
	l.nop

l00010290:
	l.movhi	r4,00000001
	l.ori	r4,r4,00003744
	l.add	r3,r2,r4
	l.addi	r4,r0,+00000001
	l.sb	0(r3),r4
	l.lwz	r3,0(r14)
	l.sfeqi	r3,00000000
	l.bf	000102CC
	l.nop

l000102B4:
	l.jal	00009DDC
	l.ori	r3,r2,00000000
	l.ori	r3,r2,00000000
	l.sh	0(r22),r11
	l.jal	00009C2C
	l.lwz	r4,0(r14)

l000102CC:
	l.addi	r2,r2,+00000001

l000102D0:
	l.lwz	r3,0(r24)
	l.sfleu	r2,r3
	l.bf	00010278
	l.nop

l000102E0:
	l.addi	r18,r18,-00000001
	l.addi	r20,r20,-00000004
	l.addi	r14,r14,-00000004
	l.sfnei	r18,FFFFFFFF
	l.bf	00010258
	l.addi	r22,r22,-00000002

l000102F8:
	l.movhi	r20,00000001
	l.movhi	r22,00000001
	l.movhi	r18,00000001
	l.addi	r2,r0,+00000013
	l.ori	r20,r20,000034D4
	l.ori	r22,r22,00003938
	l.ori	r18,r18,00003934

l00010314:
	l.lwz	r3,0(r20)
	l.srl	r3,r3,r2
	l.andi	r3,r3,00000001
	l.sfnei	r3,00000000
	l.bf	00010380
	l.nop

l0001032C:
	l.j	00010370
	l.lwz	r14,0(r22)

l00010334:
	l.lwz	r3,0(r16)
	l.srl	r3,r3,r14
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	0001036C
	l.movhi	r4,00000001

l0001034C:
	l.ori	r4,r4,00003744
	l.add	r3,r14,r4
	l.lbs	r4,0(r3)
	l.sfnei	r4,00000000
	l.bf	0001036C
	l.nop

l00010364:
	l.jal	00009ED0
	l.ori	r3,r14,00000000

l0001036C:
	l.addi	r14,r14,+00000001

l00010370:
	l.lwz	r3,0(r18)
	l.sfleu	r14,r3
	l.bf	00010334
	l.nop

l00010380:
	l.addi	r2,r2,-00000001
	l.sfnei	r2,FFFFFFFF
	l.bf	00010314
	l.addi	r16,r16,-00000004

l00010390:
	l.addi	r1,r1,+00000028
	l.lwz	r9,-4(r1)
	l.lwz	r2,-40(r1)
	l.lwz	r14,-36(r1)
	l.lwz	r16,-32(r1)
	l.lwz	r18,-28(r1)
	l.lwz	r20,-24(r1)
	l.lwz	r22,-20(r1)
	l.lwz	r24,-16(r1)
	l.lwz	r26,-12(r1)
	l.jr	r9
	l.lwz	r28,-8(r1)

;; fn000103C0: 000103C0
;;   Called from:
;;     00011748 (in fn00010570)
fn000103C0 proc
	l.sw	-36(r1),r14
	l.movhi	r14,00000000
	l.sw	-24(r1),r20
	l.sw	-20(r1),r22
	l.sw	-16(r1),r24
	l.sw	-12(r1),r26
	l.sw	-8(r1),r28
	l.movhi	r20,00000001
	l.ori	r14,r14,00004358
	l.movhi	r22,00000001
	l.movhi	r26,00000001
	l.movhi	r28,00000001
	l.movhi	r24,00000001
	l.sw	-32(r1),r16
	l.sw	-28(r1),r18
	l.sw	-4(r1),r9
	l.sw	-40(r1),r2
	l.ori	r20,r20,000034F8
	l.addi	r1,r1,-00000028
	l.ori	r22,r22,0000371C
	l.ori	r18,r14,00000000
	l.addi	r16,r0,+00000000
	l.ori	r26,r26,000034D0
	l.ori	r28,r28,00003938
	l.ori	r24,r24,00003934

l00010424:
	l.lwz	r2,0(r26)
	l.srl	r2,r2,r16
	l.andi	r2,r2,00000001
	l.sfeqi	r2,00000000
	l.bf	0001048C
	l.nop

l0001043C:
	l.lwz	r2,0(r20)
	l.sfeqi	r2,00000000
	l.bf	0001048C
	l.nop

l0001044C:
	l.j	0001047C
	l.lwz	r2,0(r28)

l00010454:
	l.lwz	r3,0(r18)
	l.srl	r3,r3,r2
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00010478
	l.nop

l0001046C:
	l.lhz	r4,0(r22)
	l.jal	00009C2C
	l.ori	r3,r2,00000000

l00010478:
	l.addi	r2,r2,+00000001

l0001047C:
	l.lwz	r3,0(r24)
	l.sfleu	r2,r3
	l.bf	00010454
	l.nop

l0001048C:
	l.addi	r16,r16,+00000001
	l.addi	r20,r20,+00000004
	l.addi	r18,r18,+00000004
	l.sfnei	r16,00000014
	l.bf	00010424
	l.addi	r22,r22,+00000002

l000104A4:
	l.movhi	r20,00000001
	l.movhi	r22,00000001
	l.movhi	r18,00000001
	l.addi	r16,r0,+00000000
	l.ori	r20,r20,000034D4
	l.ori	r22,r22,00003938
	l.ori	r18,r18,00003934

l000104C0:
	l.lwz	r2,0(r20)
	l.srl	r2,r2,r16
	l.andi	r2,r2,00000001
	l.sfnei	r2,00000000
	l.bf	00010530
	l.nop

l000104D8:
	l.j	00010520
	l.lwz	r2,0(r22)

l000104E0:
	l.lwz	r3,0(r14)
	l.srl	r3,r3,r2
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	0001051C
	l.nop

l000104F8:
	l.movhi	r4,00000001
	l.ori	r4,r4,00003744
	l.add	r3,r2,r4
	l.lbs	r3,0(r3)
	l.sfnei	r3,00000000
	l.bf	0001051C
	l.ori	r3,r2,00000000

l00010514:
	l.jal	00009ED0
	l.addi	r4,r0,+00000001

l0001051C:
	l.addi	r2,r2,+00000001

l00010520:
	l.lwz	r3,0(r18)
	l.sfleu	r2,r3
	l.bf	000104E0
	l.nop

l00010530:
	l.addi	r16,r16,+00000001
	l.sfnei	r16,00000014
	l.bf	000104C0
	l.addi	r14,r14,+00000004

l00010540:
	l.addi	r1,r1,+00000028
	l.lwz	r9,-4(r1)
	l.lwz	r2,-40(r1)
	l.lwz	r14,-36(r1)
	l.lwz	r16,-32(r1)
	l.lwz	r18,-28(r1)
	l.lwz	r20,-24(r1)
	l.lwz	r22,-20(r1)
	l.lwz	r24,-16(r1)
	l.lwz	r26,-12(r1)
	l.jr	r9
	l.lwz	r28,-8(r1)

;; fn00010570: 00010570
;;   Called from:
;;     0000F668 (in fn0000F444)
;;     0001226C (in fn00012214)
fn00010570 proc
	l.sw	-16(r1),r14
	l.movhi	r14,0000F3F3
	l.sw	-4(r1),r9
	l.sw	-12(r1),r16
	l.sw	-8(r1),r18
	l.sw	-20(r1),r2
	l.ori	r16,r3,00000000
	l.addi	r1,r1,-00000044
	l.jal	0000AEDC
	l.ori	r3,r14,00001000
	l.addi	r3,r1,+0000002C
	l.jal	0000ECC8
	l.addi	r4,r1,+00000028
	l.lwz	r18,44(r1)
	l.addi	r3,r1,+00000004
	l.ori	r4,r18,00000000
	l.jal	0000DF10
	l.addi	r5,r0,+00000020
	l.lwz	r4,40(r1)
	l.sfleui	r4,00000447
	l.bf	000121F8
	l.addi	r11,r0,-0000000C

l000105C8:
	l.movhi	r2,00000001
	l.srli	r4,r4,00000001
	l.ori	r2,r2,000034E4
	l.addi	r5,r0,+00000224
	l.ori	r3,r2,00000000
	l.jal	0000DF10
	l.add	r4,r4,r18
	l.lwz	r3,44(r16)
	l.sw	16(r1),r3
	l.jal	0000AEDC
	l.ori	r3,r14,00001001
	l.lwz	r4,16(r2)
	l.movhi	r5,00000001
	l.lwz	r2,12(r2)
	l.ori	r5,r5,000034D0
	l.and	r3,r2,r4
	l.xori	r4,r4,0000FFFF
	l.sw	0(r5),r3
	l.or	r3,r3,r4
	l.movhi	r5,00000001
	l.or	r2,r3,r2
	l.ori	r5,r5,000034D4
	l.sw	0(r5),r2
	l.jal	0000AC84
	l.movhi	r2,00000001
	l.ori	r2,r2,000034D8
	l.jal	0000AF1C
	l.addi	r3,r0,+00000001
	l.sw	0(r2),r11
	l.movhi	r2,00000170
	l.ori	r2,r2,00000030

l00010644:
	l.lwz	r3,0(r2)
	l.movhi	r4,00000001
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	00010644
	l.movhi	r4,00000001

l0001065C:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.ori	r4,r4,00002420
	l.lwz	r2,0(r2)
	l.addi	r3,r0,+0000000F
	l.sw	0(r1),r2
	l.jal	0000EDF4
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,00002000
	l.jal	0000AEDC
	l.ori	r3,r2,00003001
	l.jal	00008CC8
	l.nop
	l.movhi	r5,00003087
	l.lwz	r2,4(r1)
	l.ori	r5,r5,0000F000
	l.and	r2,r2,r5
	l.sfeqi	r2,00000000
	l.bf	000106E0
	l.movhi	r3,00004810

l000106B0:
	l.jal	00009B70
	l.nop
	l.sfnei	r11,00000001
	l.bf	000106DC
	l.nop

l000106C4:
	l.movhi	r3,00004810
	l.addi	r4,r1,+00000004
	l.jal	0000E4F0
	l.ori	r3,r3,00000FCC
	l.jal	00008B60
	l.addi	r3,r0,+00000000

l000106DC:
	l.movhi	r3,00004810

l000106E0:
	l.addi	r4,r1,+00000004
	l.ori	r3,r3,00000C94
	l.jal	0000E4F0
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,00003002
	l.jal	0000875C
	l.nop
	l.jal	0000897C
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r2,00003003
	l.jal	00004844
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r2,00003004
	l.movhi	r3,000001F0
	l.movhi	r4,00000001
	l.ori	r3,r3,00002E18
	l.ori	r4,r4,00003708
	l.lwz	r5,0(r3)
	l.addi	r6,r0,+00000000
	l.sw	0(r4),r5
	l.sw	0(r3),r6
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.jal	0000AEDC
	l.ori	r3,r2,00003005
	l.movhi	r3,00004810
	l.movhi	r4,00000001
	l.ori	r3,r3,00000F2C
	l.jal	0000E4F0
	l.ori	r4,r4,000034E4
	l.ori	r3,r2,00003006
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.jal	00008000
	l.ori	r2,r2,000034E4
	l.lwz	r3,100(r2)
	l.sfeqi	r3,00000000
	l.bf	00010804
	l.movhi	r3,00000001

l00010788:
	l.lwz	r4,104(r2)
	l.ori	r3,r3,0000375C
	l.sw	0(r3),r4
	l.movhi	r3,00000001
	l.lwz	r4,108(r2)
	l.ori	r3,r3,00003104
	l.sw	0(r3),r4
	l.lwz	r3,112(r2)
	l.movhi	r2,00000001
	l.ori	r2,r2,00003108
	l.jal	0000E5BC
	l.sw	0(r2),r3
	l.sfeqi	r11,00000000
	l.bf	000107D4
	l.nop

l000107C4:
	l.jal	0000E5CC
	l.movhi	r2,00000001
	l.ori	r2,r2,0000396C
	l.sw	0(r2),r11

l000107D4:
	l.movhi	r2,000001C2
	l.ori	r3,r2,00000028
	l.ori	r2,r2,0000015C
	l.lwz	r4,0(r3)
	l.movhi	r3,00000001
	l.ori	r3,r3,0000370C
	l.sw	0(r3),r4
	l.lwz	r3,0(r2)
	l.movhi	r2,00000001
	l.ori	r2,r2,00003710
	l.jal	00007E04
	l.sw	0(r2),r3

l00010804:
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,00003007
	l.jal	0000490C
	l.nop
	l.addi	r4,r0,+00000002
	l.jal	0000B610
	l.addi	r3,r0,+00000001
	l.jal	0000AEDC
	l.ori	r3,r2,00003008
	l.jal	0000DB28
	l.addi	r3,r0,+00000640
	l.ori	r3,r2,00003009
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.ori	r2,r2,000034D0
	l.lwz	r2,0(r2)
	l.andi	r2,r2,00000010
	l.sfeqi	r2,00000000
	l.bf	00010860
	l.nop

l00010858:
	l.jal	000100B8
	l.nop

l00010860:
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,00003091
	l.addi	r3,r0,+0000000A
	l.jal	0000B610
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+0000000B
	l.jal	0000B610
	l.addi	r4,r0,+00000017
	l.addi	r3,r0,+0000000D
	l.jal	0000B610
	l.addi	r4,r0,+00000001
	l.ori	r3,r2,00003092
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000001
	l.sfnei	r2,00000000
	l.bf	000108D0
	l.movhi	r2,00000001

l000108B4:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.lwz	r3,0(r2)
	l.ori	r4,r4,0000FFFF
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l000108D0:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00010944
	l.movhi	r3,0000F3F3

l000108E8:
	l.movhi	r4,000001C2
	l.movhi	r6,0000FFFC
	l.lwz	r5,0(r4)
	l.movhi	r3,00000001
	l.and	r5,r5,r6
	l.ori	r3,r3,00003718
	l.lwz	r6,160(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,172(r2)
	l.slli	r5,r5,00000010
	l.or	r6,r6,r5
	l.lwz	r5,168(r2)
	l.slli	r5,r5,00000008
	l.or	r6,r6,r5
	l.lwz	r5,164(r2)
	l.slli	r5,r5,00000004
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010944:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003921
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000004
	l.sfnei	r2,00000000
	l.bf	00010988
	l.movhi	r2,00000001

l00010968:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000008
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010988:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000004
	l.sfeqi	r3,00000000
	l.bf	00010A00
	l.movhi	r3,0000F3F3

l000109A0:
	l.movhi	r4,000001C2
	l.movhi	r6,0000FFF0
	l.ori	r4,r4,00000008
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,192(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,204(r2)
	l.slli	r5,r5,00000012
	l.or	r6,r6,r5
	l.lwz	r5,200(r2)
	l.slli	r5,r5,00000010
	l.or	r6,r6,r5
	l.lwz	r5,196(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010A00:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003922
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000008
	l.sfnei	r2,00000000
	l.bf	00010A44
	l.movhi	r2,00000001

l00010A24:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000010
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010A44:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000008
	l.sfeqi	r3,00000000
	l.bf	00010AA4
	l.movhi	r3,0000F3F3

l00010A5C:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r4,r4,00000010
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,208(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,212(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010AA4:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003923
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000010
	l.sfnei	r2,00000000
	l.bf	00010AE8
	l.movhi	r2,00000001

l00010AC8:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000018
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010AE8:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000010
	l.sfeqi	r3,00000000
	l.bf	00010B48
	l.movhi	r3,0000F3F3

l00010B00:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r4,r4,00000018
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,224(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,228(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010B48:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003924
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000020
	l.sfnei	r2,00000000
	l.bf	00010BA0
	l.movhi	r2,00000001

l00010B6C:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000020
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.movhi	r5,0000FFEF
	l.and	r3,r3,r4
	l.ori	r5,r5,0000FFFF
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.and	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010BA0:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000020
	l.sfeqi	r3,00000000
	l.bf	00010C20
	l.movhi	r3,0000F3F3

l00010BB8:
	l.movhi	r3,000001C2
	l.addi	r6,r0,-00002000
	l.ori	r3,r3,00000020
	l.movhi	r4,00000001
	l.lwz	r5,0(r3)
	l.ori	r4,r4,00003718
	l.and	r5,r5,r6
	l.lwz	r6,240(r2)
	l.sw	0(r4),r5
	l.lwz	r5,0(r4)
	l.or	r6,r5,r6
	l.lwz	r5,248(r2)
	l.slli	r5,r5,00000008
	l.or	r6,r6,r5
	l.lwz	r5,244(r2)
	l.slli	r5,r5,00000004
	l.or	r2,r6,r5
	l.movhi	r5,0000FFEF
	l.sw	0(r4),r2
	l.ori	r5,r5,0000FFFF
	l.lwz	r2,0(r3)
	l.and	r2,r2,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r4)
	l.sw	0(r3),r2
	l.movhi	r3,0000F3F3

l00010C20:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003925
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000040
	l.sfnei	r2,00000000
	l.bf	00010C64
	l.movhi	r2,00000001

l00010C44:
	l.movhi	r2,000001C2
	l.movhi	r6,00007FFF
	l.ori	r2,r2,00000028
	l.ori	r6,r6,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010C64:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000040
	l.sfeqi	r3,00000000
	l.bf	00010CD0
	l.movhi	r3,0000F3F3

l00010C7C:
	l.movhi	r4,000001C2
	l.movhi	r3,0000FFFF
	l.ori	r4,r4,00000028
	l.lwz	r6,256(r2)
	l.lwz	r5,0(r4)
	l.and	r5,r5,r3
	l.movhi	r3,00000001
	l.ori	r3,r3,00003718
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,264(r2)
	l.slli	r5,r5,00000008
	l.or	r6,r6,r5
	l.lwz	r5,260(r2)
	l.slli	r5,r5,00000004
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010CD0:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003926
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000800
	l.sfnei	r2,00000000
	l.bf	00010D14
	l.movhi	r2,00000001

l00010CF4:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,0000002C
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010D14:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000800
	l.sfeqi	r3,00000000
	l.bf	00010D88
	l.movhi	r3,0000F3F3

l00010D2C:
	l.movhi	r4,000001C2
	l.movhi	r6,0000FFFC
	l.ori	r4,r4,0000002C
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r7,336(r2)
	l.sw	0(r3),r5
	l.lwz	r5,344(r2)
	l.lwz	r6,0(r3)
	l.or	r6,r6,r7
	l.slli	r7,r5,00000010
	l.slli	r5,r5,00000008
	l.or	r6,r6,r7
	l.or	r6,r6,r5
	l.lwz	r5,340(r2)
	l.slli	r5,r5,00000004
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010D88:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003927
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000400
	l.sfnei	r2,00000000
	l.bf	00010DCC
	l.movhi	r2,00000001

l00010DAC:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000030
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010DCC:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000400
	l.sfeqi	r3,00000000
	l.bf	00010E2C
	l.movhi	r3,0000F3F3

l00010DE4:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r4,r4,00000030
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,320(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,324(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010E2C:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003928
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000080
	l.sfnei	r2,00000000
	l.bf	00010E70
	l.movhi	r2,00000001

l00010E50:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000038
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010E70:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000080
	l.sfeqi	r3,00000000
	l.bf	00010ED0
	l.movhi	r3,0000F3F3

l00010E88:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r4,r4,00000038
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,272(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,276(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010ED0:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003929
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00002000
	l.sfnei	r2,00000000
	l.bf	00010F14
	l.movhi	r2,00000001

l00010EF4:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000040
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010F14:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00002000
	l.sfeqi	r3,00000000
	l.bf	00010F80
	l.movhi	r3,0000F3F3

l00010F2C:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00001000
	l.ori	r4,r4,00000040
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,368(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,376(r2)
	l.slli	r5,r5,00000008
	l.or	r6,r6,r5
	l.lwz	r5,372(r2)
	l.slli	r5,r5,00000004
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00010F80:
	l.movhi	r2,00000001
	l.ori	r3,r3,0000392A
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000100
	l.sfnei	r2,00000000
	l.bf	00010FC4
	l.movhi	r2,00000001

l00010FA4:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000044
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00010FC4:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000100
	l.sfeqi	r3,00000000
	l.bf	00011024
	l.movhi	r3,0000F3F3

l00010FDC:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r4,r4,00000044
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,288(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,292(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l00011024:
	l.movhi	r2,00000001
	l.ori	r3,r3,0000392B
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00000200
	l.sfnei	r2,00000000
	l.bf	00011068
	l.movhi	r2,00000001

l00011048:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,00000048
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011068:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00000200
	l.sfeqi	r3,00000000
	l.bf	000110C8
	l.movhi	r3,0000F3F3

l00011080:
	l.movhi	r4,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r4,r4,00000048
	l.movhi	r3,00000001
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00003718
	l.and	r5,r5,r6
	l.lwz	r6,304(r2)
	l.sw	0(r3),r5
	l.lwz	r5,0(r3)
	l.or	r6,r5,r6
	l.lwz	r5,308(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r3)
	l.sw	0(r4),r2
	l.movhi	r3,0000F3F3

l000110C8:
	l.movhi	r2,00000001
	l.ori	r3,r3,0000392C
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,148(r2)
	l.andi	r2,r2,00001000
	l.sfnei	r2,00000000
	l.bf	00011120
	l.movhi	r2,00000001

l000110EC:
	l.movhi	r2,000001C2
	l.movhi	r4,00007FFF
	l.ori	r2,r2,0000004C
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.movhi	r5,0000BFFF
	l.and	r3,r3,r4
	l.ori	r5,r5,0000FFFF
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.and	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011120:
	l.ori	r2,r2,000034E4
	l.lwz	r3,156(r2)
	l.andi	r3,r3,00001000
	l.sfeqi	r3,00000000
	l.bf	00011194
	l.movhi	r3,0000F3F3

l00011138:
	l.movhi	r3,000001C2
	l.addi	r6,r0,-00008000
	l.ori	r3,r3,0000004C
	l.movhi	r4,00000001
	l.lwz	r5,0(r3)
	l.ori	r4,r4,00003718
	l.and	r5,r5,r6
	l.lwz	r6,352(r2)
	l.sw	0(r4),r5
	l.lwz	r5,0(r4)
	l.or	r6,r5,r6
	l.lwz	r5,356(r2)
	l.slli	r5,r5,00000008
	l.or	r2,r6,r5
	l.movhi	r5,0000BFFF
	l.sw	0(r4),r2
	l.ori	r5,r5,0000FFFF
	l.lwz	r2,0(r3)
	l.and	r2,r2,r5
	l.sw	0(r3),r2
	l.lwz	r2,0(r4)
	l.sw	0(r3),r2
	l.movhi	r3,0000F3F3

l00011194:
	l.movhi	r2,00000001
	l.ori	r3,r3,00003093
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r3,384(r2)
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	000111C0
	l.addi	r3,r0,+00000004

l000111B8:
	l.jal	0000B610
	l.lwz	r4,388(r2)

l000111C0:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r3,384(r2)
	l.andi	r3,r3,00000004
	l.sfeqi	r3,00000000
	l.bf	000111E4
	l.addi	r3,r0,+00000014

l000111DC:
	l.jal	0000B0B8
	l.lwz	r4,436(r2)

l000111E4:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r3,384(r2)
	l.andi	r3,r3,00000010
	l.sfeqi	r3,00000000
	l.bf	000112D4
	l.nop

l00011200:
	l.lwz	r4,468(r2)
	l.xori	r3,r4,00000003
	l.sub	r5,r0,r3
	l.or	r3,r5,r3
	l.sfgesi	r3,+00000000
	l.bf	00011230
	l.xori	r3,r4,00000001

l0001121C:
	l.sub	r5,r0,r3
	l.or	r3,r5,r3
	l.sfltsi	r3,+00000000
	l.bf	00011280
	l.sfnei	r4,0000000B

l00011230:
	l.addi	r3,r0,+0000000A
	l.jal	0000B610
	l.movhi	r2,000001C2
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.ori	r2,r2,00000054
	l.movhi	r4,00000001
	l.lwz	r3,0(r2)
	l.ori	r4,r4,000034E4
	l.addi	r6,r0,-00000031
	l.lwz	r4,476(r4)
	l.and	r3,r3,r6
	l.slli	r4,r4,00000004
	l.or	r3,r4,r3
	l.movhi	r4,00000001
	l.ori	r4,r4,00003714
	l.sw	0(r4),r3
	l.sw	0(r2),r3
	l.j	000112D8
	l.movhi	r2,00000001

l00011280:
	l.bf	000112D4
	l.movhi	r3,000001C2

l00011288:
	l.lwz	r6,476(r2)
	l.lwz	r4,472(r2)
	l.slli	r6,r6,00000004
	l.slli	r4,r4,00000006
	l.ori	r3,r3,00000054
	l.or	r4,r6,r4
	l.lwz	r5,0(r3)
	l.addi	r6,r0,-000000F1
	l.and	r5,r5,r6
	l.or	r4,r4,r5
	l.movhi	r5,00000001
	l.ori	r5,r5,00003714
	l.sw	0(r5),r4
	l.sw	0(r3),r4
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+0000000A
	l.jal	0000B610
	l.lwz	r4,468(r2)

l000112D4:
	l.movhi	r2,00000001

l000112D8:
	l.ori	r2,r2,000034E4
	l.lwz	r3,384(r2)
	l.andi	r3,r3,00000020
	l.sfeqi	r3,00000000
	l.bf	000112F8
	l.addi	r3,r0,+0000000B

l000112F0:
	l.jal	0000B610
	l.lwz	r4,488(r2)

l000112F8:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r3,384(r2)
	l.andi	r3,r3,00000040
	l.sfeqi	r3,00000000
	l.bf	00011340
	l.movhi	r3,000001C2

l00011314:
	l.lwz	r4,516(r2)
	l.ori	r3,r3,00000054
	l.addi	r6,r0,-00000301
	l.lwz	r5,0(r3)
	l.slli	r4,r4,00000008
	l.and	r2,r5,r6
	l.or	r2,r4,r2
	l.movhi	r4,00000001
	l.ori	r4,r4,00003714
	l.sw	0(r4),r2
	l.sw	0(r3),r2

l00011340:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r3,384(r2)
	l.andi	r3,r3,00000080
	l.sfeqi	r3,00000000
	l.bf	000113A0
	l.nop

l0001135C:
	l.lwz	r4,528(r2)
	l.jal	0000B610
	l.addi	r3,r0,+0000000D
	l.movhi	r3,000001C2
	l.movhi	r4,0000FFFC
	l.ori	r3,r3,00000058
	l.lwz	r5,0(r3)
	l.and	r5,r5,r4
	l.lwz	r4,544(r2)
	l.or	r5,r5,r4
	l.lwz	r4,540(r2)
	l.slli	r4,r4,00000010
	l.or	r2,r5,r4
	l.movhi	r4,00000001
	l.ori	r4,r4,00003714
	l.sw	0(r4),r2
	l.sw	0(r3),r2

l000113A0:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r2,144(r2)
	l.andi	r3,r2,00000008
	l.sfnei	r3,00000000
	l.bf	000113DC
	l.addi	r4,r0,+00000002

l000113BC:
	l.jal	0000B610
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.jal	0000BAC8
	l.nop
	l.j	00011430
	l.movhi	r3,0000F3F3

l000113DC:
	l.andi	r2,r2,00000003
	l.sfnei	r2,00000000
	l.bf	0001142C
	l.movhi	r5,0000A700

l000113EC:
	l.movhi	r2,000001F0
	l.ori	r2,r2,00001444
	l.addi	r6,r0,-00000004
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.and	r3,r3,r6
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3

l0001142C:
	l.movhi	r3,0000F3F3

l00011430:
	l.movhi	r2,00000001
	l.ori	r3,r3,0000300A
	l.jal	0000AEDC
	l.ori	r2,r2,000034D4
	l.lwz	r2,0(r2)
	l.andi	r2,r2,00000010
	l.sfnei	r2,00000000
	l.bf	00011470
	l.movhi	r2,00000001

l00011454:
	l.addi	r3,r0,+0000000B
	l.jal	0000B950
	l.addi	r4,r0,+00000001
	l.addi	r3,r0,+0000000C
	l.jal	0000B950
	l.addi	r4,r0,+00000001
	l.movhi	r2,00000001

l00011470:
	l.ori	r2,r2,000034D4
	l.lwz	r2,0(r2)
	l.andi	r2,r2,00000008
	l.sfnei	r2,00000000
	l.bf	00011498
	l.movhi	r2,00000001

l00011488:
	l.addi	r3,r0,+0000000F
	l.jal	0000B950
	l.addi	r4,r0,+00000001
	l.movhi	r2,00000001

l00011498:
	l.ori	r2,r2,000034D4
	l.lwz	r4,0(r2)
	l.andi	r4,r4,00000010
	l.sfnei	r4,00000000
	l.bf	000114B8
	l.nop

l000114B0:
	l.jal	0000BC38
	l.addi	r3,r0,+00000021

l000114B8:
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,0000300B
	l.jal	000101A8
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r2,0000300C
	l.movhi	r3,00000001
	l.addi	r4,r0,+00000000
	l.ori	r3,r3,000034DC
	l.sw	0(r3),r4
	l.jal	0000AEDC
	l.ori	r3,r2,00004000
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,0000242A
	l.ori	r3,r2,00005000
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E0
	l.addi	r5,r0,+00000000
	l.sw	0(r2),r5
	l.ori	r2,r5,00000000
	l.movhi	r6,00003087

l0001151C:
	l.lwz	r3,4(r1)
	l.ori	r6,r6,0000F000
	l.and	r3,r3,r6
	l.sfeqi	r3,00000000
	l.bf	0001156C
	l.lwz	r3,4(r1)

l00011534:
	l.jal	00008C74
	l.addi	r3,r0,+00000000
	l.sfeqi	r11,00000000
	l.bf	00011568
	l.nop

l00011548:
	l.jal	00008CA4
	l.addi	r3,r0,+00000000
	l.movhi	r3,00000001
	l.jal	0000A22C
	l.ori	r3,r3,000034E0
	l.sfeqi	r11,00000000
	l.bf	00011734
	l.nop

l00011568:
	l.lwz	r3,4(r1)

l0001156C:
	l.movhi	r4,00000080
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	000115DC
	l.lwz	r3,4(r1)

l00011580:
	l.jal	00008C74
	l.addi	r3,r0,+0000000D
	l.sfnei	r11,00000000
	l.bf	000115C0
	l.lwz	r4,24(r1)

l00011594:
	l.jal	00008C74
	l.addi	r3,r0,+00000014
	l.sfnei	r11,00000000
	l.bf	000115C0
	l.lwz	r4,24(r1)

l000115A8:
	l.jal	00008C74
	l.addi	r3,r0,+00000015
	l.sfeqi	r11,00000000
	l.bf	000115DC
	l.lwz	r3,4(r1)

l000115BC:
	l.lwz	r4,24(r1)

l000115C0:
	l.addi	r3,r0,+00000001
	l.jal	00009958
	l.andi	r4,r4,00000FFF
	l.sfeqi	r11,00000000
	l.bnf	00011728
	l.movhi	r3,00000080

l000115D8:
	l.lwz	r3,4(r1)

l000115DC:
	l.movhi	r5,00000008
	l.and	r3,r3,r5
	l.sfeqi	r3,00000000
	l.bf	00011624
	l.lwz	r3,4(r1)

l000115F0:
	l.jal	00008C74
	l.addi	r3,r0,+00000005
	l.sfeqi	r11,00000000
	l.bf	00011624
	l.lwz	r3,4(r1)

l00011604:
	l.jal	00008CA4
	l.addi	r3,r0,+00000005
	l.jal	000092CC
	l.nop
	l.sfnei	r11,00000001
	l.bnf	00011728
	l.movhi	r3,00000008

l00011620:
	l.lwz	r3,4(r1)

l00011624:
	l.movhi	r6,00000010
	l.and	r3,r3,r6
	l.sfeqi	r3,00000000
	l.bf	00011650
	l.lwz	r3,4(r1)

l00011638:
	l.jal	00008C74
	l.addi	r3,r0,+00000008
	l.sfeqi	r11,00000000
	l.bnf	00011728
	l.movhi	r3,00000010

l0001164C:
	l.lwz	r3,4(r1)

l00011650:
	l.movhi	r4,00000020
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	0001167C
	l.lwz	r3,4(r1)

l00011664:
	l.jal	00008C74
	l.addi	r3,r0,+00000009
	l.sfeqi	r11,00000000
	l.bnf	00011728
	l.movhi	r3,00000020

l00011678:
	l.lwz	r3,4(r1)

l0001167C:
	l.movhi	r5,00000040
	l.and	r3,r3,r5
	l.sfeqi	r3,00000000
	l.bf	000116BC
	l.lwz	r3,4(r1)

l00011690:
	l.lwz	r3,20(r1)
	l.sfgeu	r2,r3
	l.bf	000116B4
	l.nop

l000116A0:
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.addi	r2,r2,+00000001
	l.j	000116BC
	l.lwz	r3,4(r1)

l000116B4:
	l.j	00011728
	l.movhi	r3,00000040

l000116BC:
	l.movhi	r6,00000100
	l.and	r3,r3,r6
	l.sfeqi	r3,00000000
	l.bf	000116FC
	l.lwz	r3,4(r1)

l000116D0:
	l.jal	00008C74
	l.addi	r3,r0,+0000001B
	l.sfnei	r11,00000000
	l.bf	00011728
	l.movhi	r3,00000100

l000116E4:
	l.jal	00008C74
	l.addi	r3,r0,+0000001C
	l.sfeqi	r11,00000000
	l.bnf	00011728
	l.movhi	r3,00000100

l000116F8:
	l.lwz	r3,4(r1)

l000116FC:
	l.movhi	r4,00000A00
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	0001151C
	l.movhi	r6,00003087

l00011710:
	l.jal	00008C74
	l.addi	r3,r0,+00000016
	l.sfeqi	r11,00000000
	l.bf	0001151C
	l.movhi	r6,00003087

l00011724:
	l.movhi	r3,00000A00

l00011728:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E0
	l.sw	0(r2),r3

l00011734:
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,00006000
	l.jal	0000AEDC
	l.ori	r3,r2,00007001
	l.jal	000103C0
	l.nop
	l.ori	r3,r2,00007002
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.ori	r2,r2,000034D4
	l.lwz	r2,0(r2)
	l.andi	r2,r2,00000010
	l.sfnei	r2,00000000
	l.bf	00011788
	l.addi	r3,r0,+0000000B

l00011774:
	l.jal	0000B950
	l.ori	r4,r2,00000000
	l.addi	r3,r0,+0000000C
	l.jal	0000B950
	l.ori	r4,r2,00000000

l00011788:
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.ori	r2,r2,000034D4
	l.lwz	r2,0(r2)
	l.andi	r2,r2,00000010
	l.sfnei	r2,00000000
	l.bf	000117BC
	l.addi	r3,r0,+00000021

l000117AC:
	l.jal	0000BC38
	l.addi	r4,r0,+00000001
	l.jal	0000C768
	l.addi	r3,r0,+00000001

l000117BC:
	l.movhi	r3,0000F3F3
	l.movhi	r2,00000001
	l.ori	r3,r3,00007003
	l.jal	0000AEDC
	l.ori	r2,r2,000034E4
	l.lwz	r2,144(r2)
	l.andi	r3,r2,00000008
	l.sfnei	r3,00000000
	l.bf	00011808
	l.nop

l000117E4:
	l.jal	0000BB60
	l.movhi	r2,00000001
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+00000001
	l.jal	0000B610
	l.addi	r4,r0,+00000003
	l.j	000118B8
	l.ori	r2,r2,000034E4

l00011808:
	l.andi	r2,r2,00000003
	l.sfnei	r2,00000000
	l.bf	000118B4
	l.movhi	r2,00000001

l00011818:
	l.movhi	r2,000001F0
	l.movhi	r5,0000A700
	l.ori	r2,r2,00001444
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.ori	r3,r3,00000001
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3
	l.jal	0000C768
	l.addi	r3,r0,+00000002
	l.lwz	r3,0(r2)
	l.movhi	r6,0000A700
	l.movhi	r4,0000FFF8
	l.or	r3,r3,r6
	l.ori	r4,r4,0000FFFF
	l.sw	36(r1),r3
	l.movhi	r5,00000004
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.and	r3,r3,r4
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.or	r3,r3,r5
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l000118B4:
	l.ori	r2,r2,000034E4

l000118B8:
	l.lwz	r2,384(r2)
	l.andi	r2,r2,00000005
	l.sfeqi	r2,00000000
	l.bf	000118F0
	l.movhi	r2,00000001

l000118CC:
	l.addi	r4,r0,+00000003
	l.jal	0000B610
	l.addi	r3,r0,+00000004
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.addi	r3,r0,+00000014
	l.jal	0000B0B8
	l.addi	r4,r0,+00000001
	l.movhi	r2,00000001

l000118F0:
	l.ori	r2,r2,000034E4
	l.lwz	r2,384(r2)
	l.andi	r2,r2,00000050
	l.sfeqi	r2,00000000
	l.bf	00011944
	l.addi	r4,r0,+00000003

l00011908:
	l.addi	r3,r0,+0000000A
	l.jal	0000B610
	l.movhi	r2,000001C2
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.ori	r2,r2,00000054
	l.lwz	r3,0(r2)
	l.addi	r6,r0,-00001000
	l.and	r3,r3,r6
	l.ori	r3,r3,00000010
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3
	l.jal	0000C768
	l.addi	r3,r0,+00000001

l00011944:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r2,384(r2)
	l.andi	r2,r2,00000020
	l.sfeqi	r2,00000000
	l.bf	00011970
	l.movhi	r2,00000001

l00011960:
	l.addi	r3,r0,+0000000B
	l.jal	0000B610
	l.addi	r4,r0,+00000017
	l.movhi	r2,00000001

l00011970:
	l.ori	r2,r2,000034E4
	l.lwz	r2,384(r2)
	l.andi	r2,r2,00000080
	l.sfeqi	r2,00000000
	l.bf	000119B8
	l.addi	r4,r0,+00000003

l00011988:
	l.addi	r3,r0,+0000000D
	l.jal	0000B610
	l.movhi	r2,000001C2
	l.addi	r3,r0,+00000001
	l.jal	0000C768
	l.ori	r2,r2,00000058
	l.lwz	r3,0(r2)
	l.movhi	r4,0000FF00
	l.and	r3,r3,r4
	l.sw	36(r1),r3
	l.lwz	r3,36(r1)
	l.sw	0(r2),r3

l000119B8:
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000001
	l.sfeqi	r2,00000000
	l.bf	000119E4
	l.movhi	r2,00000001

l000119D4:
	l.addi	r3,r0,+00001000
	l.movhi	r2,000001C2
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l000119E4:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000001
	l.sfeqi	r2,00000000
	l.bf	00011A14
	l.movhi	r2,00000001

l000119FC:
	l.movhi	r2,000001C2
	l.movhi	r5,00008000
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011A14:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000004
	l.sfeqi	r2,00000000
	l.bf	00011A44
	l.movhi	r2,00000001

l00011A2C:
	l.movhi	r3,00000003
	l.movhi	r2,000001C2
	l.ori	r3,r3,00005514
	l.ori	r2,r2,00000008
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011A44:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000004
	l.sfeqi	r2,00000000
	l.bf	00011A78
	l.movhi	r2,00000001

l00011A5C:
	l.movhi	r2,000001C2
	l.movhi	r6,00008000
	l.ori	r2,r2,00000008
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011A78:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000008
	l.sfeqi	r2,00000000
	l.bf	00011AA8
	l.movhi	r2,00000001

l00011A90:
	l.movhi	r3,00000300
	l.movhi	r2,000001C2
	l.ori	r3,r3,00006207
	l.ori	r2,r2,00000010
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011AA8:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000008
	l.sfeqi	r2,00000000
	l.bf	00011ADC
	l.movhi	r2,00000001

l00011AC0:
	l.movhi	r2,000001C2
	l.movhi	r4,00008000
	l.ori	r2,r2,00000010
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011ADC:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000010
	l.sfeqi	r2,00000000
	l.bf	00011B0C
	l.movhi	r2,00000001

l00011AF4:
	l.movhi	r3,00000300
	l.movhi	r2,000001C2
	l.ori	r3,r3,00006207
	l.ori	r2,r2,00000018
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011B0C:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000010
	l.sfeqi	r2,00000000
	l.bf	00011B40
	l.movhi	r2,00000001

l00011B24:
	l.movhi	r2,000001C2
	l.movhi	r5,00008000
	l.ori	r2,r2,00000018
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011B40:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000020
	l.sfeqi	r2,00000000
	l.bf	00011B7C
	l.movhi	r2,00000001

l00011B58:
	l.movhi	r2,000001C2
	l.addi	r3,r0,+00001000
	l.ori	r2,r2,00000020
	l.movhi	r6,00004000
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011B7C:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000020
	l.sfeqi	r2,00000000
	l.bf	00011BC0
	l.movhi	r2,00000001

l00011B94:
	l.movhi	r2,000001C2
	l.movhi	r4,00008000
	l.ori	r2,r2,00000020
	l.movhi	r5,00004000
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011BC0:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000040
	l.sfeqi	r2,00000000
	l.bf	00011BF0
	l.movhi	r2,00000001

l00011BD8:
	l.movhi	r3,00000004
	l.movhi	r2,000001C2
	l.ori	r3,r3,00001811
	l.ori	r2,r2,00000028
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011BF0:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000040
	l.sfeqi	r2,00000000
	l.bf	00011C24
	l.movhi	r2,00000001

l00011C08:
	l.movhi	r2,000001C2
	l.movhi	r6,00008000
	l.ori	r2,r2,00000028
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011C24:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000800
	l.sfeqi	r2,00000000
	l.bf	00011C54
	l.movhi	r2,00000001

l00011C3C:
	l.movhi	r3,00000004
	l.movhi	r2,000001C2
	l.ori	r3,r3,00001811
	l.ori	r2,r2,0000002C
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011C54:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000800
	l.sfeqi	r2,00000000
	l.bf	00011C88
	l.movhi	r2,00000001

l00011C6C:
	l.movhi	r2,000001C2
	l.movhi	r4,00008000
	l.ori	r2,r2,0000002C
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011C88:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000400
	l.sfeqi	r2,00000000
	l.bf	00011CB8
	l.movhi	r2,00000001

l00011CA0:
	l.movhi	r3,00000300
	l.movhi	r2,000001C2
	l.ori	r3,r3,00006207
	l.ori	r2,r2,00000030
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011CB8:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000400
	l.sfeqi	r2,00000000
	l.bf	00011CEC
	l.movhi	r2,00000001

l00011CD0:
	l.movhi	r2,000001C2
	l.movhi	r5,00008000
	l.ori	r2,r2,00000030
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011CEC:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000080
	l.sfeqi	r2,00000000
	l.bf	00011D1C
	l.movhi	r2,00000001

l00011D04:
	l.movhi	r3,00000300
	l.movhi	r2,000001C2
	l.ori	r3,r3,00006207
	l.ori	r2,r2,00000038
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011D1C:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000080
	l.sfeqi	r2,00000000
	l.bf	00011D50
	l.movhi	r2,00000001

l00011D34:
	l.movhi	r2,000001C2
	l.movhi	r6,00008000
	l.ori	r2,r2,00000038
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011D50:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00002000
	l.sfeqi	r2,00000000
	l.bf	00011D7C
	l.movhi	r2,00000001

l00011D68:
	l.movhi	r2,000001C2
	l.addi	r3,r0,+00000515
	l.ori	r2,r2,00000040
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011D7C:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00002000
	l.sfeqi	r2,00000000
	l.bf	00011DB0
	l.movhi	r2,00000001

l00011D94:
	l.movhi	r2,000001C2
	l.movhi	r4,00008000
	l.ori	r2,r2,00000040
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011DB0:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000100
	l.sfeqi	r2,00000000
	l.bf	00011DE0
	l.movhi	r2,00000001

l00011DC8:
	l.movhi	r3,00000300
	l.movhi	r2,000001C2
	l.ori	r3,r3,00001300
	l.ori	r2,r2,00000044
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011DE0:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000100
	l.sfeqi	r2,00000000
	l.bf	00011E14
	l.movhi	r2,00000001

l00011DF8:
	l.movhi	r2,000001C2
	l.movhi	r5,00008000
	l.ori	r2,r2,00000044
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011E14:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00000200
	l.sfeqi	r2,00000000
	l.bf	00011E44
	l.movhi	r2,00000001

l00011E2C:
	l.movhi	r3,00000300
	l.movhi	r2,000001C2
	l.ori	r3,r3,00006207
	l.ori	r2,r2,00000048
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011E44:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00000200
	l.sfeqi	r2,00000000
	l.bf	00011E78
	l.movhi	r2,00000001

l00011E5C:
	l.movhi	r2,000001C2
	l.movhi	r6,00008000
	l.ori	r2,r2,00000048
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011E78:
	l.ori	r2,r2,000034E4
	l.lwz	r2,156(r2)
	l.andi	r2,r2,00001000
	l.sfeqi	r2,00000000
	l.bf	00011EB4
	l.movhi	r2,00000001

l00011E90:
	l.movhi	r2,000001C2
	l.addi	r3,r0,+00001000
	l.ori	r2,r2,0000004C
	l.movhi	r4,00004000
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011EB4:
	l.ori	r2,r2,000034E4
	l.lwz	r2,152(r2)
	l.andi	r2,r2,00001000
	l.sfeqi	r2,00000000
	l.bf	00011EF8
	l.movhi	r2,00000001

l00011ECC:
	l.movhi	r2,000001C2
	l.movhi	r5,00008000
	l.ori	r2,r2,0000004C
	l.movhi	r6,00004000
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.movhi	r2,00000001

l00011EF8:
	l.ori	r2,r2,000034D0
	l.lwz	r2,0(r2)
	l.andi	r2,r2,00000010
	l.sfeqi	r2,00000000
	l.bf	00011F18
	l.nop

l00011F10:
	l.jal	000100B8
	l.nop

l00011F18:
	l.movhi	r2,0000F3F3
	l.jal	0000AEDC
	l.ori	r3,r2,00007004
	l.jal	00004A6C
	l.lwz	r3,16(r1)
	l.ori	r3,r2,00007005
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.lwz	r2,100(r2)
	l.sfeqi	r2,00000000
	l.bf	00012064
	l.movhi	r16,000001C2

l00011F4C:
	l.movhi	r4,00007FFF
	l.ori	r2,r16,00000028
	l.ori	r4,r4,0000FFFF
	l.lwz	r3,0(r2)
	l.movhi	r14,00000001
	l.and	r3,r3,r4
	l.ori	r14,r14,00003710
	l.sw	0(r2),r3
	l.movhi	r3,00000001
	l.ori	r3,r3,0000370C
	l.lwz	r3,0(r3)
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00000064
	l.movhi	r6,00008000
	l.lwz	r3,0(r2)
	l.or	r3,r3,r6
	l.sw	0(r2),r3
	l.addi	r3,r0,+00000014
	l.jal	0000C8A0
	l.ori	r2,r16,0000015C
	l.lwz	r3,0(r14)
	l.andi	r3,r3,00000007
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+000000C8
	l.movhi	r4,00000300
	l.lwz	r3,0(r14)
	l.ori	r4,r4,00000007
	l.and	r3,r3,r4
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00000014
	l.movhi	r5,00008000
	l.lwz	r3,0(r2)
	l.or	r3,r3,r5
	l.sw	0(r2),r3
	l.jal	0000C8A0
	l.addi	r3,r0,+00002710
	l.jal	00007FAC
	l.nop
	l.jal	0000E5BC
	l.nop
	l.sfeqi	r11,00000000
	l.bf	00012064
	l.nop

l00012008:
	l.movhi	r2,00000001
	l.addi	r3,r0,+00000010
	l.addi	r4,r0,+00000001
	l.jal	00008030
	l.ori	r2,r2,00003970
	l.jal	0000E5CC
	l.nop
	l.sw	0(r2),r11
	l.movhi	r2,00000001
	l.ori	r2,r2,0000396C
	l.lwz	r2,0(r2)
	l.sfeq	r11,r2
	l.bf	00012064
	l.nop

l00012040:
	l.movhi	r3,0000F1F1
	l.jal	0000AEDC
	l.ori	r3,r3,0000900F
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002430

l0001205C:
	l.j	0001205C
	l.nop

l00012064:
	l.jal	00007FD0
	l.movhi	r2,0000F3F3
	l.ori	r3,r2,00007006
	l.jal	0000AEDC
	l.movhi	r18,00000001
	l.movhi	r3,00004810
	l.movhi	r4,00000001
	l.ori	r3,r3,00000F7C
	l.ori	r4,r4,000034E4
	l.jal	0000E4F0
	l.movhi	r16,00000001
	l.ori	r3,r2,00007007
	l.jal	0000AEDC
	l.movhi	r14,00000001
	l.movhi	r3,00000001
	l.ori	r18,r18,000034E0
	l.ori	r3,r3,00003708
	l.ori	r16,r16,00003970
	l.lwz	r4,0(r3)
	l.movhi	r3,000001F0
	l.ori	r14,r14,0000396C
	l.ori	r3,r3,00002E18
	l.sw	0(r3),r4
	l.jal	0000C768
	l.addi	r3,r0,+00000001
	l.jal	0000AEDC
	l.ori	r3,r2,00007008
	l.jal	0000AEDC
	l.ori	r3,r2,00007009
	l.jal	00008788
	l.nop
	l.jal	00008998
	l.nop
	l.jal	0000AEDC
	l.ori	r3,r2,0000700A
	l.jal	00004978
	l.lwz	r3,16(r1)
	l.jal	0000AEDC
	l.ori	r3,r2,0000700B
	l.movhi	r4,00000001
	l.addi	r3,r0,+0000000F
	l.jal	0000EDF4
	l.ori	r4,r4,00002443

l00012110:
	l.jal	00008674
	l.nop
	l.sfeqi	r11,00000000
	l.bf	00012110
	l.ori	r2,r11,00000000

l00012124:
	l.lbz	r3,2(r11)
	l.sfnei	r3,00000011
	l.bf	0001215C
	l.addi	r6,r0,+00000005

l00012134:
	l.lwz	r3,0(r18)
	l.lwz	r4,0(r16)
	l.sw	40(r11),r3
	l.lwz	r3,0(r14)
	l.xor	r3,r4,r3
	l.sb	0(r11),r6
	l.sub	r4,r0,r3
	l.or	r3,r4,r3
	l.srli	r3,r3,0000001F
	l.sw	44(r11),r3

l0001215C:
	l.lbz	r3,1(r2)
	l.andi	r3,r3,00000003
	l.sfeqi	r3,00000000
	l.bf	00012180
	l.ori	r3,r2,00000000

l00012170:
	l.jal	00008574
	l.addi	r4,r0,+00000FA0
	l.j	0001218C
	l.lbz	r2,0(r2)

l00012180:
	l.jal	0000F368
	l.ori	r3,r2,00000000
	l.lbz	r2,0(r2)

l0001218C:
	l.sfnei	r2,00000005
	l.bf	00012110
	l.movhi	r2,0000F3F3

l00012198:
	l.jal	0000AEDC
	l.ori	r3,r2,0000700C
	l.movhi	r3,00004810
	l.addi	r4,r1,+00000004
	l.jal	0000E4F0
	l.ori	r3,r3,00000DF4
	l.jal	0000AEDC
	l.ori	r3,r2,0000700D
	l.ori	r3,r2,00008000
	l.jal	0000AEDC
	l.movhi	r2,00000001
	l.ori	r2,r2,000034D8
	l.addi	r3,r0,+00000001
	l.lwz	r4,0(r2)
	l.jal	0000B610
	l.movhi	r2,00000001
	l.ori	r2,r2,000034E4
	l.movhi	r4,00000001
	l.lwz	r2,0(r2)
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002459
	l.jal	0000EDF4
	l.sw	0(r1),r2
	l.addi	r11,r0,+00000000

l000121F8:
	l.addi	r1,r1,+00000044
	l.lwz	r9,-4(r1)
	l.lwz	r2,-20(r1)
	l.lwz	r14,-16(r1)
	l.lwz	r16,-12(r1)
	l.jr	r9
	l.lwz	r18,-8(r1)

;; fn00012214: 00012214
;;   Called from:
;;     0000F5D4 (in fn0000F444)
fn00012214 proc
	l.sw	-12(r1),r2
	l.sw	-8(r1),r14
	l.sw	-4(r1),r9
	l.lwz	r14,48(r3)
	l.addi	r1,r1,-0000000C
	l.lwz	r2,40(r3)
	l.sfnei	r14,00000000
	l.bf	00012258
	l.lwz	r5,44(r3)

l00012238:
	l.ori	r3,r5,00000000
	l.jal	000049C0
	l.ori	r4,r2,00000000
	l.ori	r3,r14,00000000
	l.jal	00004B44
	l.ori	r4,r2,00000000
	l.j	0001228C
	l.addi	r1,r1,+0000000C

l00012258:
	l.sfnei	r14,00000003
	l.bf	00012288
	l.sfnei	r2,00000000

l00012264:
	l.bf	0001227C
	l.nop

l0001226C:
	l.jal	00010570
	l.nop
	l.j	0001228C
	l.addi	r1,r1,+0000000C

l0001227C:
	l.addi	r3,r0,+00000000
	l.jal	00004C48
	l.ori	r4,r2,00000000

l00012288:
	l.addi	r1,r1,+0000000C

l0001228C:
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)
000122A0 D7 E1 4F FC 9C 21 FF FC 9C 21 00 04 85 21 FF FC ..O..!...!...!..
000122B0 03 FF E2 0B 15 00 00 00                         ........       

;; fn000122B8: 000122B8
;;   Called from:
;;     0000F5E4 (in fn0000F444)
fn000122B8 proc
	l.sw	-8(r1),r2
	l.sw	-4(r1),r9
	l.lwz	r2,40(r3)
	l.sfeqi	r2,00000000
	l.bf	000122E4
	l.addi	r1,r1,-00000008

l000122D0:
	l.sfgtui	r2,00000002
	l.bf	00012310
	l.addi	r11,r0,-00000016

l000122DC:
	l.j	000122FC
	l.nop

l000122E4:
	l.jal	0000AEDC
	l.ori	r3,r0,0000A101
	l.jal	0000AADC
	l.nop
	l.j	00012310
	l.ori	r11,r2,00000000

l000122FC:
	l.jal	0000AEDC
	l.ori	r3,r0,0000A102
	l.jal	0000AA80
	l.nop
	l.addi	r11,r0,+00000000

l00012310:
	l.addi	r1,r1,+00000008
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00012320: 00012320
;;   Called from:
;;     0000F678 (in fn0000F444)
fn00012320 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.jal	0000A664
	l.addi	r1,r1,-0000000C
	l.jal	00009290
	l.nop
	l.movhi	r3,000001F0
	l.addi	r4,r0,+0000000F
	l.jal	0000AF04
	l.ori	r3,r3,00000108
	l.jal	0000AC14
	l.nop

l00012350:
	l.jal	00008C74
	l.addi	r3,r0,+00000000
	l.sfeqi	r11,00000000
	l.bf	000123A8
	l.nop

l00012364:
	l.jal	00008CA4
	l.addi	r3,r0,+00000000
	l.jal	0000A22C
	l.ori	r3,r1,00000000
	l.movhi	r4,00000007
	l.lwz	r3,0(r1)
	l.ori	r4,r4,00008000
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	000123A0
	l.nop

l00012390:
	l.jal	0000A300
	l.nop
	l.j	000123EC
	l.movhi	r4,00000001

l000123A0:
	l.jal	0000A300
	l.nop

l000123A8:
	l.jal	00008C74
	l.addi	r3,r0,+00000005
	l.sfeqi	r11,00000000
	l.bf	000123D8
	l.nop

l000123BC:
	l.jal	00008CA4
	l.addi	r3,r0,+00000005
	l.jal	000092CC
	l.nop
	l.sfeqi	r11,00000001
	l.bf	000123EC
	l.movhi	r4,00000001

l000123D8:
	l.jal	0000A6B0
	l.nop
	l.sfnei	r11,00000001
	l.bf	00012350
	l.movhi	r4,00000001

l000123EC:
	l.addi	r3,r0,+0000000F
	l.ori	r4,r4,00002464
	l.jal	0000EDF4
	l.addi	r2,r0,-00000002
	l.movhi	r3,000001F0
	l.ori	r3,r3,0000142C
	l.lwz	r4,0(r3)
	l.and	r4,r4,r2
	l.sw	0(r3),r4
	l.jal	0000A708
	l.nop

l00012418:
	l.j	00012418
	l.nop
00012420 25 78 20 65 6E 74 65 72 0A 00 77 61 69 74 0A 00 %x enter..wait..
00012430 64 72 61 6D 20 63 72 63 20 65 72 72 6F 72 2E 2E dram crc error..
00012440 2E 0A 00 77 61 69 74 20 61 63 33 32 37 20 72 65 ...wait ac327 re
00012450 73 75 6D 65 2E 2E 2E 0A 00 25 78 20 72 65 74 75 sume.....%x retu
00012460 72 6E 0A 00 72 65 73 65 74 20 73 79 73 74 65 6D rn..reset system
00012470 20 6E 6F 77 0A 00 00 00                          now....       
l00012478	dd	0x00006380
l0001247C	dd	0x000062F8
l00012480	dd	0x00006304
l00012484	dd	0x00006310
l00012488	dd	0x00006324
l0001248C	dd	0x0000631C
l00012490	dd	0x0000AF54
l00012494	dd	0x0000AF4C
00012498                         00 00 B0 24 00 00 AF 78         ...$...x
000124A0 00 00 B0 1C 00 00 B0 1C 00 00 B0 1C 00 00 B0 1C ................
000124B0 00 00 B0 1C 00 00 AF 98 00 00 B0 1C 00 00 AF 98 ................
000124C0 00 00 AF B8 00 00 B0 1C 00 00 B0 1C 00 00 B0 1C ................
000124D0 00 00 B0 1C 00 00 B0 1C 00 00 B0 1C 00 00 B0 1C ................
000124E0 00 00 B0 1C 00 00 B0 1C 00 00 AF D8 00 00 B0 1C ................
000124F0 00 00 AF D8 00 00 AF D8 00 00 AF D8 00 00 AF D8 ................
00012500 00 00 B0 1C 00 00 AF D8 00 00 AF E0 00 00 AF D8 ................
00012510 00 00 B2 88 00 00 B2 80 00 00 B2 AC 00 00 B3 10 ................
00012520 00 00 B3 10 00 00 B3 10 00 00 B3 10 00 00 B3 10 ................
00012530 00 00 B3 10 00 00 B3 10 00 00 B3 10 00 00 B3 10 ................
00012540 00 00 B3 10 00 00 B3 10 00 00 B3 10 00 00 B3 10 ................
00012550 00 00 B3 10 00 00 B3 10 00 00 B3 10 00 00 B2 F0 ................
00012560 00 00 B3 10 00 00 B3 10 00 00 B2 80 00 00 B3 10 ................
00012570 00 00 B2 80 00 00 B3 10 00 00 B2 80 00 00 B2 80 ................
00012580 00 00 B3 10 00 00 B2 80 00 00 B2 C4 00 00 B2 80 ................
00012590 00 00 B4 60 00 00 B4 C4 00 00 B4 7C 00 00 B4 98 ...`.......|....
000125A0 00 00 B4 C4 00 00 B4 C4 00 00 B4 C4 00 00 B4 C4 ................
000125B0 00 00 B4 C4 00 00 B3 EC 00 00 B4 C4 00 00 B3 50 ...............P
000125C0 00 00 B4 C4 00 00 B3 74 00 00 B3 A4 00 00 B4 C4 .......t........
000125D0 00 00 B3 C8 00 00 B4 30 00 00 B4 10 00 00 00 01 .......0........
000125E0 00 00 00 03 00 00 00 0B 00 00 00 02 00 00 00 01 ................
000125F0 00 00 00 03 00 00 00 06 00 00 00 06 00 00 00 01 ................
00012600 00 00 00 03 00 00 00 04 00 00 00 0B 00 00 00 01 ................
00012610 00 00 00 03 00 00 00 0B 00 00 00 0B 00 00 00 01 ................
00012620 00 00 00 03 09 00 00 02 09 00 00 02 09 00 00 02 ................
00012630 09 00 00 02 09 00 00 02 09 00 00 02 09 00 00 02 ................
00012640 09 00 00 02 09 00 00 02 09 00 00 02 09 00 00 02 ................
00012650 0A 00 00 02 0B 00 00 02 0C 00 00 02 0D 00 00 02 ................
00012660 0E 00 00 02 0F 00 00 02 10 00 00 02 11 00 00 02 ................
00012670 12 00 00 02 09 00 00 01 0A 00 00 01 0A 00 00 01 ................
00012680 0B 00 00 01 0B 00 00 01 0C 00 00 01 0C 00 00 01 ................
00012690 0D 00 00 01 0D 00 00 01 0E 00 00 01 0E 00 00 01 ................
000126A0 0F 00 00 01 0F 00 00 01 10 00 00 01 10 00 00 01 ................
000126B0 11 00 00 01 11 00 00 01 12 00 00 01 12 00 00 01 ................
000126C0 09 00 00 00 09 00 00 00 0A 00 00 00 0A 00 00 00 ................
000126D0 0A 00 00 00 0A 00 00 00 0B 00 00 00 0B 00 00 00 ................
000126E0 0B 00 00 00 0B 00 00 00 0C 00 00 00 0C 00 00 00 ................
000126F0 0C 00 00 00 0C 00 00 00 0D 00 00 00 0D 00 00 00 ................
00012700 0D 00 00 00 0D 00 00 00 0E 00 00 00 0E 00 00 00 ................
00012710 0E 00 00 00 0E 00 00 00 0F 00 00 00 0F 00 00 00 ................
00012720 0F 00 00 00 0F 00 00 00 10 00 00 00 10 00 00 00 ................
00012730 10 00 00 00 10 00 00 00 11 00 00 00 11 00 00 00 ................
00012740 11 00 00 00 11 00 00 00 12 00 00 00 12 00 00 00 ................
00012750 12 00 00 00 12 00 00 00 13 00 00 00 13 00 00 00 ................
00012760 13 00 00 00 13 00 00 00 14 00 00 00 14 00 00 00 ................
00012770 14 00 00 00 14 00 00 00 15 00 00 00 15 00 00 00 ................
00012780 15 00 00 00 15 00 00 00 16 00 00 00 16 00 00 00 ................
00012790 16 00 00 00 16 00 00 00 17 00 00 00 17 00 00 00 ................
000127A0 17 00 00 00 17 00 00 00 18 00 00 00 18 00 00 00 ................
000127B0 18 00 00 00 18 00 00 00 19 00 00 00 19 00 00 00 ................
000127C0 19 00 00 00 19 00 00 00 1A 00 00 00 1A 00 00 00 ................
000127D0 1A 00 00 00 1A 00 00 00 1B 00 00 00 1B 00 00 00 ................
000127E0 1B 00 00 00 1B 00 00 00 1C 00 00 00 1C 00 00 00 ................
000127F0 1C 00 00 00 1C 00 00 00 1D 00 00 00 1D 00 00 00 ................
00012800 1D 00 00 00 1D 00 00 00 0F 01 00 00 0F 01 00 00 ................
00012810 0F 01 00 00 0F 01 00 00 0F 01 00 00 0F 01 00 00 ................
00012820 0F 01 00 00 0F 01 00 00 0A 02 00 00 0A 02 00 00 ................
00012830 0A 02 00 00 0A 02 00 00 10 01 00 00 10 01 00 00 ................
00012840 10 01 00 00 10 01 00 00 11 01 00 00 11 01 00 00 ................
00012850 11 01 00 00 11 01 00 00 11 01 00 00 11 01 00 00 ................
00012860 11 01 00 00 11 01 00 00 12 01 00 00 12 01 00 00 ................
00012870 12 01 00 00 12 01 00 00 12 01 00 00 12 01 00 00 ................
00012880 12 01 00 00 12 01 00 00 0C 02 00 00 0C 02 00 00 ................
00012890 0C 02 00 00 0C 02 00 00 13 01 00 00 13 01 00 00 ................
000128A0 13 01 00 00 13 01 00 00 14 01 00 00 14 01 00 00 ................
000128B0 14 01 00 00 14 01 00 00 14 01 00 00 14 01 00 00 ................
000128C0 14 01 00 00 14 01 00 00 15 01 00 00 15 01 00 00 ................
000128D0 15 01 00 00 15 01 00 00 15 01 00 00 15 01 00 00 ................
000128E0 15 01 00 00 15 01 00 00 0E 02 00 00 0E 02 00 00 ................
000128F0 0E 02 00 00 0E 02 00 00 16 01 00 00 16 01 00 00 ................
00012900 16 01 00 00 16 01 00 00 17 01 00 00 17 01 00 00 ................
00012910 17 01 00 00 17 01 00 00 17 01 00 00 17 01 00 00 ................
00012920 17 01 00 00 17 01 00 00 18 01 00 00 18 01 00 00 ................
00012930 18 01 00 00 18 01 00 00 18 01 00 00 18 01 00 00 ................
00012940 18 01 00 00 18 01 00 00 10 02 00 00 10 02 00 00 ................
00012950 10 02 00 00 10 02 00 00 19 01 00 00 19 01 00 00 ................
00012960 19 01 00 00 19 01 00 00 1A 01 00 00 1A 01 00 00 ................
00012970 1A 01 00 00 1A 01 00 00 1A 01 00 00 1A 01 00 00 ................
00012980 1A 01 00 00 1A 01 00 00 1B 01 00 00 1B 01 00 00 ................
00012990 1B 01 00 00 1B 01 00 00 1B 01 00 00 1B 01 00 00 ................
000129A0 1B 01 00 00 1B 01 00 00 12 02 00 00 12 02 00 00 ................
000129B0 12 02 00 00 12 02 00 00 13 02 00 00 13 02 00 00 ................
000129C0 13 02 00 00 13 02 00 00 13 02 00 00 13 02 00 00 ................
000129D0 13 02 00 00 13 02 00 00 13 02 00 00 13 02 00 00 ................
000129E0 13 02 00 00 13 02 00 00 14 02 00 00 14 02 00 00 ................
000129F0 14 02 00 00 14 02 00 00 14 02 00 00 14 02 00 00 ................
00012A00 14 02 00 00 14 02 00 00 14 02 00 00 14 02 00 00 ................
00012A10 14 02 00 00 14 02 00 00 0F 03 00 00 0F 03 00 00 ................
00012A20 0F 03 00 00 0F 03 00 00 15 02 00 00 15 02 00 00 ................
00012A30 15 02 00 00 15 02 00 00 15 02 00 00 15 02 00 00 ................
00012A40 15 02 00 00 15 02 00 00 10 03 00 00 10 03 00 00 ................
00012A50 10 03 00 00 10 03 00 00 10 03 00 00 10 03 00 00 ................
00012A60 10 03 00 00 10 03 00 00 16 02 00 00 16 02 00 00 ................
00012A70 16 02 00 00 16 02 00 00 17 02 00 00 17 02 00 00 ................
00012A80 17 02 00 00 17 02 00 00 17 02 00 00 17 02 00 00 ................
00012A90 17 02 00 00 17 02 00 00 17 02 00 00 17 02 00 00 ................
00012AA0 17 02 00 00 17 02 00 00 18 02 00 00 18 02 00 00 ................
00012AB0 18 02 00 00 18 02 00 00 18 02 00 00 18 02 00 00 ................
00012AC0 18 02 00 00 18 02 00 00 18 02 00 00 18 02 00 00 ................
00012AD0 18 02 00 00 18 02 00 00 19 02 00 00 19 02 00 00 ................
00012AE0 19 02 00 00 19 02 00 00 19 02 00 00 19 02 00 00 ................
00012AF0 19 02 00 00 19 02 00 00 19 02 00 00 19 02 00 00 ................
00012B00 19 02 00 00 19 02 00 00 1A 02 00 00 1A 02 00 00 ................
00012B10 1A 02 00 00 1A 02 00 00 1A 02 00 00 1A 02 00 00 ................
00012B20 1A 02 00 00 1A 02 00 00 1A 02 00 00 1A 02 00 00 ................
00012B30 1A 02 00 00 1A 02 00 00 1B 02 00 00 1B 02 00 00 ................
00012B40 1B 02 00 00 1B 02 00 00 1B 02 00 00 1B 02 00 00 ................
00012B50 1B 02 00 00 1B 02 00 00 1B 02 00 00 1B 02 00 00 ................
00012B60 1B 02 00 00 1B 02 00 00 00 00 B9 80 00 00 B9 A0 ................
00012B70 00 00 B9 D0 00 00 B9 F4 00 00 BA 98 00 00 BA 98 ................
00012B80 00 00 BA 98 00 00 BA 98 00 00 BA 98 00 00 BA 98 ................
00012B90 00 00 BA 18 00 00 BA 3C 00 00 BA 98 00 00 BA 98 .......<........
00012BA0 00 00 BA 6C 00 00 BD 54 00 00 BD B8 00 00 BD 70 ...l...T.......p
00012BB0 00 00 BD 8C 00 00 BD B8 00 00 BD B8 00 00 BD B8 ................
00012BC0 00 00 BD B8 00 00 BD B8 00 00 BD B8 00 00 BD B8 ................
00012BD0 00 00 BC 68 00 00 BD B8 00 00 BC 8C 00 00 BC BC ...h............
00012BE0 00 00 BD B8 00 00 BC E0 00 00 BD 04 00 00 BD B8 ................
00012BF0 00 00 BD 28 30 31 32 33 34 35 36 37 38 39 41 42 ...(0123456789AB
00012C00 43 44 45 46 00 00 00 00 70 6D 75 20 69 73 20 65 CDEF....pmu is e
00012C10 78 69 73 74 0A 00 70 6D 75 20 69 73 20 6E 6F 74 xist..pmu is not
00012C20 20 65 78 69 73 74 0A 00 72 65 73 65 74 31 20 73  exist..reset1 s
00012C30 79 73 74 65 6D 0A 00 70 6F 77 65 72 6F 66 66 0A ystem..poweroff.
00012C40 00 73 68 75 74 64 6F 77 6E 0A 00 63 70 75 30 20 .shutdown..cpu0 
00012C50 65 6E 74 65 72 20 77 66 69 20 6E 6F 77 2C 20 77 enter wfi now, w
00012C60 61 69 74 20 5B 25 64 5D 20 6D 73 0A 00 25 73 20 ait [%d] ms..%s 
00012C70 3A 0A 00 0A 30 78 25 38 70 20 3A 20 00 25 38 78 :...0x%8p : .%8x
00012C80 20 00 63 72 63 20 62 65 67 69 6E 2E 2E 2E 0A 00  .crc begin.....
00012C90 73 72 63 3A 25 78 20 6C 65 6E 3A 25 78 0A 00 63 src:%x len:%x..c
00012CA0 72 63 20 66 69 6E 69 73 68 2E 2E 2E 0A 00 73 74 rc finish.....st
00012CB0 61 63 6B 20 66 72 65 65 3A 25 64 62 79 74 65 0A ack free:%dbyte.
00012CC0 00 65 78 63 65 70 74 69 6F 6E 20 5B 25 78 2C 20 .exception [%x, 
00012CD0 25 73 5D 20 63 6F 6D 69 6E 67 2C 20 5B 65 70 63 %s] coming, [epc
00012CE0 20 3D 20 25 78 5D 0A 00 72 65 67 69 73 74 65 72  = %x]..register
00012CF0 20 6C 69 73 74 3A 0A 00 72 65 67 69 73 74 65 72  list:..register
00012D00 25 78 3A 20 25 78 0A 00 63 70 75 20 61 62 6F 72 %x: %x..cpu abor
00012D10 74 20 65 6E 74 65 72 2E 2E 2E 0A 00 41 6E 20 75 t enter.....An u
00012D20 6E 6B 6E 6F 77 6E 00 41 20 72 65 73 65 74 00 41 nknown.A reset.A
00012D30 20 42 75 73 20 45 72 72 6F 72 00 41 20 44 61 74  Bus Error.A Dat
00012D40 61 20 50 61 67 65 20 46 61 75 6C 74 00 41 6E 20 a Page Fault.An 
00012D50 49 6E 73 74 72 75 63 74 69 6F 6E 20 50 61 67 65 Instruction Page
00012D60 20 46 61 75 6C 74 00 41 20 54 69 63 6B 2D 54 69  Fault.A Tick-Ti
00012D70 6D 65 72 00 41 6E 20 41 6C 69 67 6E 6D 65 6E 74 mer.An Alignment
00012D80 00 41 6E 20 49 6C 6C 65 67 61 6C 20 49 6E 73 74 .An Illegal Inst
00012D90 72 75 63 74 69 6F 6E 00 41 6E 20 45 78 74 65 72 ruction.An Exter
00012DA0 6E 61 6C 20 49 6E 74 65 72 72 75 70 74 00 41 20 nal Interrupt.A 
00012DB0 44 2D 54 4C 42 20 4D 69 73 73 00 41 6E 20 49 2D D-TLB Miss.An I-
00012DC0 54 4C 42 20 4D 69 73 73 00 41 20 52 61 6E 67 65 TLB Miss.A Range
00012DD0 00 41 20 53 79 73 74 65 6D 20 43 61 6C 6C 00 41 .A System Call.A
00012DE0 20 46 6C 6F 61 74 69 6E 67 2D 50 6F 69 6E 74 00  Floating-Point.
00012DF0 41 20 54 72 61 70 00 41 20 52 65 73 65 72 76 65 A Trap.A Reserve
00012E00 64 00 41 52 49 53 43 20 55 50 20 0A 00 6E 6F 74 d.ARISC UP ..not
00012E10 69 66 79 20 0A 00 76 30 2E 32 2E 32 33 00 73 65 ify ..v0.2.23.se
00012E20 74 20 70 61 72 61 73 20 0A 00 6E 6F 74 69 66 79 t paras ..notify
00012E30 20 6F 76 65 72 20 0A 00 61 72 31 30 30 20 76 65  over ..ar100 ve
00012E40 72 73 69 6F 6E 20 3A 20 25 64 0A 00 2D 2D 2D 2D rsion : %d..----
00012E50 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D ----------------
00012E60 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 0A 00 70 61 72 61 ----------..para
00012E70 00 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 .               
00012E80 20 00 6D 73 67 00 00 00 00 00 00 01 00 00 42 38  .msg.........B8
00012E90 00 36 00 12 02 58 04 4C 00 0A 00 33 04 60 05 F0 .6...X.L...3.`..
00012EA0 00 14 00 15 00 00 00 7F 00 36 00 13 03 E8 08 CA .........6......
00012EB0 00 32 00 1A 08 CA 08 CA 00 00 00 00 00 00 00 1F .2..............
00012EC0 00 36 00 14 02 58 04 4C 00 0A 00 33 04 60 05 F0 .6...X.L...3.`..
00012ED0 00 14 00 15 00 00 00 7F 00 36 00 15 02 58 05 DC .........6...X..
00012EE0 00 14 00 2E 06 40 0C E4 00 64 00 12 00 00 00 3F .....@...d.....?
00012EF0 00 36 00 16 04 4C 0D 48 00 64 00 18 0D 48 0D 48 .6...L.H.d...H.H
00012F00 00 00 00 00 00 00 00 1F 00 36 00 17 02 BC 0C E4 .........6......
00012F10 00 64 00 1B 0C E4 0C E4 00 00 00 00 00 00 00 1F .d..............
00012F20 00 36 00 18 02 BC 0D 48 00 64 00 1C 0D 48 0D 48 .6.....H.d...H.H
00012F30 00 00 00 00 00 00 00 1F 00 36 00 19 02 BC 0C E4 .........6......
00012F40 00 64 00 1B 0C E4 0C E4 00 00 00 00 00 00 00 1F .d..............
00012F50 00 36 00 20 02 BC 07 6C 00 64 00 0D 07 6C 07 6C .6. ...l.d...l.l
00012F60 00 00 00 00 00 00 00 0F 00 36 00 21 02 BC 07 6C .........6.!...l
00012F70 00 64 00 0D 07 6C 07 6C 00 00 00 00 00 00 00 0F .d...l.l........
00012F80 00 36 00 22 02 BC 07 6C 00 64 00 0D 07 6C 07 6C .6."...l.d...l.l
00012F90 00 00 00 00 00 00 00 0F 00 36 00 23 02 BC 07 6C .........6.#...l
00012FA0 00 64 00 0D 07 6C 07 6C 00 00 00 00 00 00 00 0F .d...l.l........
00012FB0 00 36 00 24 02 BC 0C E4 00 64 00 1B 0C E4 0C E4 .6.$.....d......
00012FC0 00 00 00 00 00 00 00 1F 00 36 00 25 02 BC 0D 48 .........6.%...H
00012FD0 00 64 00 1C 0E 10 10 68 00 C8 00 04 00 00 00 1F .d.....h........
00012FE0 00 36 00 26 02 BC 0C E4 00 64 00 1B 0C E4 0C E4 .6.&.....d......
00012FF0 00 00 00 00 00 00 00 1F 00 00 01 00 00 00 00 00 ................
00013000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00013010 00 36 00 10 00 00 00 00 00 36 00 10 00 01 00 00 .6.......6......
00013020 00 36 00 10 00 02 00 00 00 36 00 10 00 03 00 00 .6.......6......
00013030 00 36 00 10 00 04 00 00 00 36 00 10 00 05 00 00 .6.......6......
00013040 00 36 00 10 00 06 00 00 00 36 00 10 00 07 00 00 .6.......6......
00013050 00 36 00 11 00 00 00 00 00 36 00 11 00 01 00 00 .6.......6......
00013060 00 36 00 11 00 02 00 00 00 36 00 11 00 03 00 00 .6.......6......
00013070 00 36 00 11 00 04 00 00 00 36 00 11 00 05 00 00 .6.......6......
00013080 00 36 00 11 00 06 00 00 00 36 00 11 00 07 00 00 .6.......6......
00013090 00 36 01 00 00 00 00 00 00 F4 24 00 00 00 7A 12 .6........$...z.
000130A0 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 01 ................
000130B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
000130C0 00 00 00 01 00 00 00 00 00 00 00 02 00 00 00 00 ................
000130D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 ................
000130E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
000130F0 40 00 00 00 00 10 00 00 00 00 00 00 00 00 00 00 @...............
00013100 00 00 00 00 40 00 00 00 00 10 00 00 48 10 30 00 ....@.......H.0.
00013110 00 01 2D 1C 00 01 2D 27 00 01 2D 2F 00 01 2D 3B ..-...-'..-/..-;
00013120 00 01 2D 4D 00 01 2D 67 00 01 2D 74 00 01 2D 81 ..-M..-g..-t..-.
00013130 00 01 2D 98 00 01 2D AE 00 01 2D BB 00 01 2D C9 ..-...-...-...-.
00013140 00 01 2D D1 00 01 2D DF 00 01 2D F0 00 01 2D F7 ..-...-...-...-.
00013150 00 00 00 02 00 00 00 01 00 00 00 00 00 00 00 00 ................
00013160 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...

;; fn00018000: 00018000
fn00018000 proc
	l.movhi	r3,00000001
	l.movhi	r4,000001C2
	l.ori	r3,r3,0000310C
	l.lwz	r5,540(r4)
	l.lwz	r3,0(r3)
	l.addi	r11,r0,+00000000
	l.sw	2044(r3),r5
	l.sw	1500(r3),r4
	l.lwz	r5,544(r4)
	l.sw	2048(r3),r5
	l.lwz	r5,548(r4)
	l.sw	2052(r3),r5
	l.lwz	r5,552(r4)
	l.sw	2056(r3),r5
	l.lwz	r5,556(r4)
	l.sw	2060(r3),r5
	l.lwz	r5,564(r4)
	l.sw	2068(r3),r5
	l.lwz	r5,572(r4)
	l.sw	2076(r3),r5
	l.lwz	r5,584(r4)
	l.sw	2088(r3),r5
	l.lwz	r5,592(r4)
	l.sw	2096(r3),r5
	l.lwz	r5,636(r4)
	l.sw	2140(r3),r5
	l.lwz	r5,640(r4)
	l.sw	2144(r3),r5
	l.lwz	r5,644(r4)
	l.sw	2148(r3),r5
	l.lwz	r5,648(r4)
	l.sw	2152(r3),r5
	l.lwz	r5,652(r4)
	l.sw	2156(r3),r5
	l.lwz	r5,668(r4)
	l.sw	2172(r3),r5
	l.lwz	r5,680(r4)
	l.sw	2184(r3),r5
	l.lwz	r5,0(r4)
	l.sw	1504(r3),r5
	l.lwz	r5,8(r4)
	l.sw	1512(r3),r5
	l.lwz	r5,16(r4)
	l.sw	1520(r3),r5
	l.lwz	r5,24(r4)
	l.sw	1528(r3),r5
	l.lwz	r5,40(r4)
	l.sw	1544(r3),r5
	l.lwz	r5,68(r4)
	l.sw	1572(r3),r5
	l.lwz	r5,56(r4)
	l.sw	1560(r3),r5
	l.lwz	r5,72(r4)
	l.sw	1576(r3),r5
	l.lwz	r5,80(r4)
	l.sw	1584(r3),r5
	l.lwz	r5,84(r4)
	l.sw	1588(r3),r5
	l.lwz	r5,88(r4)
	l.sw	1592(r3),r5
	l.lwz	r5,92(r4)
	l.sw	1596(r3),r5
	l.lwz	r5,96(r4)
	l.sw	1600(r3),r5
	l.lwz	r5,100(r4)
	l.sw	1604(r3),r5
	l.lwz	r5,104(r4)
	l.sw	1608(r3),r5
	l.lwz	r5,108(r4)
	l.sw	1612(r3),r5
	l.lwz	r5,112(r4)
	l.sw	1616(r3),r5
	l.lwz	r5,116(r4)
	l.sw	1620(r3),r5
	l.lwz	r5,128(r4)
	l.sw	1632(r3),r5
	l.lwz	r5,136(r4)
	l.sw	1640(r3),r5
	l.lwz	r5,140(r4)
	l.sw	1644(r3),r5
	l.lwz	r5,144(r4)
	l.sw	1648(r3),r5
	l.lwz	r5,152(r4)
	l.sw	1656(r3),r5
	l.lwz	r5,156(r4)
	l.sw	1660(r3),r5
	l.lwz	r5,160(r4)
	l.sw	1664(r3),r5
	l.lwz	r5,164(r4)
	l.sw	1668(r3),r5
	l.lwz	r5,176(r4)
	l.sw	1680(r3),r5
	l.lwz	r5,180(r4)
	l.sw	1684(r3),r5
	l.lwz	r5,184(r4)
	l.sw	1688(r3),r5
	l.lwz	r5,192(r4)
	l.sw	1696(r3),r5
	l.lwz	r5,204(r4)
	l.sw	1708(r3),r5
	l.lwz	r5,256(r4)
	l.sw	1760(r3),r5
	l.lwz	r5,260(r4)
	l.sw	1764(r3),r5
	l.lwz	r5,280(r4)
	l.sw	1784(r3),r5
	l.lwz	r5,288(r4)
	l.sw	1792(r3),r5
	l.lwz	r5,292(r4)
	l.sw	1796(r3),r5
	l.lwz	r5,304(r4)
	l.sw	1808(r3),r5
	l.lwz	r5,308(r4)
	l.sw	1812(r3),r5
	l.lwz	r5,316(r4)
	l.sw	1820(r3),r5
	l.lwz	r5,320(r4)
	l.sw	1824(r3),r5
	l.lwz	r5,324(r4)
	l.sw	1828(r3),r5
	l.lwz	r5,336(r4)
	l.sw	1840(r3),r5
	l.lwz	r5,340(r4)
	l.sw	1844(r3),r5
	l.lwz	r5,416(r4)
	l.sw	1920(r3),r5
	l.lwz	r5,512(r4)
	l.sw	2016(r3),r5
	l.lwz	r5,516(r4)
	l.sw	2020(r3),r5
	l.lwz	r5,704(r4)
	l.sw	2208(r3),r5
	l.lwz	r5,708(r4)
	l.sw	2212(r3),r5
	l.lwz	r5,712(r4)
	l.sw	2216(r3),r5
	l.lwz	r5,720(r4)
	l.sw	2224(r3),r5
	l.lwz	r5,728(r4)
	l.sw	2232(r3),r5
	l.lwz	r5,752(r4)
	l.sw	2256(r3),r5
	l.lwz	r5,768(r4)
	l.sw	2272(r3),r5
	l.lwz	r4,772(r4)
	l.sw	2276(r3),r4
	l.jr	r9
	l.nop

;; fn00018250: 00018250
fn00018250 proc
	l.sw	-8(r1),r2
	l.movhi	r2,00000001
	l.sw	-4(r1),r9
	l.ori	r2,r2,0000310C
	l.addi	r1,r1,-0000000C
	l.lwz	r4,0(r2)
	l.lwz	r5,2016(r4)
	l.lwz	r3,1500(r4)
	l.sw	512(r3),r5
	l.lwz	r5,2020(r4)
	l.sw	516(r3),r5
	l.lwz	r5,2044(r4)
	l.sw	540(r3),r5
	l.lwz	r5,2048(r4)
	l.sw	544(r3),r5
	l.lwz	r5,2052(r4)
	l.sw	548(r3),r5
	l.lwz	r5,2056(r4)
	l.sw	552(r3),r5
	l.lwz	r5,2060(r4)
	l.sw	556(r3),r5
	l.lwz	r5,2068(r4)
	l.sw	564(r3),r5
	l.lwz	r5,2076(r4)
	l.sw	572(r3),r5
	l.lwz	r5,2088(r4)
	l.sw	584(r3),r5
	l.lwz	r5,2096(r4)
	l.sw	592(r3),r5
	l.lwz	r5,2140(r4)
	l.sw	636(r3),r5
	l.lwz	r5,2144(r4)
	l.sw	640(r3),r5
	l.lwz	r5,2148(r4)
	l.sw	644(r3),r5
	l.lwz	r5,2152(r4)
	l.sw	648(r3),r5
	l.lwz	r5,2156(r4)
	l.sw	652(r3),r5
	l.lwz	r5,2172(r4)
	l.sw	668(r3),r5
	l.lwz	r5,2184(r4)
	l.sw	680(r3),r5
	l.lwz	r5,1504(r4)
	l.sw	0(r3),r5
	l.lwz	r5,1512(r4)
	l.sw	8(r3),r5
	l.lwz	r5,1520(r4)
	l.sw	16(r3),r5
	l.lwz	r5,1528(r4)
	l.sw	24(r3),r5
	l.lwz	r5,1544(r4)
	l.sw	40(r3),r5
	l.lwz	r5,1572(r4)
	l.sw	68(r3),r5
	l.lwz	r5,1560(r4)
	l.sw	56(r3),r5
	l.lwz	r4,1576(r4)
	l.sw	72(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+0000000A
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r2,0(r2)
	l.lwz	r2,1500(r2)
	l.lwz	r3,0(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018404
	l.movhi	r3,00000000

l0001836C:
	l.lwz	r3,8(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018404
	l.movhi	r3,00000000

l00018384:
	l.lwz	r3,16(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018404
	l.movhi	r3,00000000

l0001839C:
	l.lwz	r3,24(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018404
	l.movhi	r3,00000000

l000183B4:
	l.lwz	r3,40(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018404
	l.movhi	r3,00000000

l000183CC:
	l.lwz	r3,68(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018404
	l.movhi	r3,00000000

l000183E4:
	l.lwz	r3,56(r2)
	l.srli	r3,r3,0000001C
	l.andi	r3,r3,00000001
	l.sfeqi	r3,00000000
	l.bf	00018400
	l.nop

l000183FC:
	l.lwz	r2,72(r2)

l00018400:
	l.movhi	r3,00000000

l00018404:
	l.movhi	r2,00000001
	l.ori	r3,r3,0000C8A0
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r2,r2,0000310C
	l.lwz	r4,0(r2)
	l.addi	r6,r0,-00000004
	l.lwz	r3,1500(r4)
	l.lwz	r5,80(r3)
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.and	r5,r5,r6
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.lwz	r4,1584(r4)
	l.andi	r4,r4,00000003
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	80(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r4,0(r2)
	l.movhi	r6,0000FFFC
	l.lwz	r3,1500(r4)
	l.ori	r6,r6,0000FFFF
	l.lwz	r5,80(r3)
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.and	r5,r5,r6
	l.movhi	r6,00000003
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.lwz	r4,1584(r4)
	l.and	r4,r4,r6
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	80(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r3,0(r1)
	l.addi	r4,r0,-00000301
	l.and	r3,r3,r4
	l.sw	0(r1),r3
	l.lwz	r3,0(r2)
	l.lwz	r5,0(r1)
	l.lwz	r4,1584(r3)
	l.lwz	r3,1500(r3)
	l.andi	r4,r4,00000300
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	80(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r4,0(r2)
	l.addi	r6,r0,-00000400
	l.lwz	r3,1500(r4)
	l.lwz	r5,84(r3)
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.and	r5,r5,r6
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.lwz	r4,1588(r4)
	l.andi	r4,r4,000003FF
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	84(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r3,0(r1)
	l.addi	r4,r0,-00003001
	l.and	r3,r3,r4
	l.sw	0(r1),r3
	l.lwz	r3,0(r2)
	l.lwz	r5,0(r1)
	l.lwz	r4,1588(r3)
	l.lwz	r3,1500(r3)
	l.andi	r4,r4,00003000
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	84(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r4,0(r2)
	l.movhi	r6,0000FFFE
	l.lwz	r3,1500(r4)
	l.lwz	r5,88(r3)
	l.sw	0(r1),r5
	l.lwz	r5,0(r1)
	l.and	r5,r5,r6
	l.movhi	r6,00000001
	l.sw	0(r1),r5
	l.ori	r6,r6,0000FFFF
	l.lwz	r5,0(r1)
	l.lwz	r4,1592(r4)
	l.and	r4,r4,r6
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	88(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.movhi	r4,0000FCFF
	l.lwz	r3,0(r1)
	l.ori	r4,r4,0000FFFF
	l.movhi	r6,00000300
	l.and	r3,r3,r4
	l.sw	0(r1),r3
	l.lwz	r3,0(r2)
	l.lwz	r5,0(r1)
	l.lwz	r4,1592(r3)
	l.lwz	r3,1500(r3)
	l.and	r4,r4,r6
	l.or	r4,r4,r5
	l.sw	0(r1),r4
	l.lwz	r4,0(r1)
	l.sw	88(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000002
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r3,0(r2)
	l.lwz	r5,1596(r3)
	l.lwz	r4,1500(r3)
	l.sw	92(r4),r5
	l.lwz	r5,2208(r3)
	l.sw	704(r4),r5
	l.lwz	r5,2212(r3)
	l.sw	708(r4),r5
	l.lwz	r5,2216(r3)
	l.sw	712(r4),r5
	l.lwz	r5,2224(r3)
	l.sw	720(r4),r5
	l.lwz	r3,2232(r3)
	l.sw	728(r4),r3
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000001
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r4,0(r2)
	l.lwz	r5,1620(r4)
	l.lwz	r3,1500(r4)
	l.sw	116(r3),r5
	l.lwz	r5,1632(r4)
	l.sw	128(r3),r5
	l.lwz	r5,1640(r4)
	l.sw	136(r3),r5
	l.lwz	r5,1644(r4)
	l.sw	140(r3),r5
	l.lwz	r5,1648(r4)
	l.sw	144(r3),r5
	l.lwz	r5,1656(r4)
	l.sw	152(r3),r5
	l.lwz	r5,1660(r4)
	l.sw	156(r3),r5
	l.lwz	r5,1664(r4)
	l.sw	160(r3),r5
	l.lwz	r5,1668(r4)
	l.sw	164(r3),r5
	l.lwz	r5,1680(r4)
	l.sw	176(r3),r5
	l.lwz	r5,1684(r4)
	l.sw	180(r3),r5
	l.lwz	r5,1688(r4)
	l.sw	184(r3),r5
	l.lwz	r5,1696(r4)
	l.sw	192(r3),r5
	l.lwz	r5,1708(r4)
	l.sw	204(r3),r5
	l.lwz	r5,256(r3)
	l.sw	1760(r4),r5
	l.lwz	r5,1764(r4)
	l.sw	260(r3),r5
	l.lwz	r5,1784(r4)
	l.sw	280(r3),r5
	l.lwz	r5,1792(r4)
	l.sw	288(r3),r5
	l.lwz	r5,1796(r4)
	l.sw	292(r3),r5
	l.lwz	r5,1808(r4)
	l.sw	304(r3),r5
	l.lwz	r5,1812(r4)
	l.sw	308(r3),r5
	l.lwz	r5,1820(r4)
	l.sw	316(r3),r5
	l.lwz	r5,1824(r4)
	l.sw	320(r3),r5
	l.lwz	r5,1828(r4)
	l.sw	324(r3),r5
	l.lwz	r5,1840(r4)
	l.sw	336(r3),r5
	l.lwz	r5,1844(r4)
	l.sw	340(r3),r5
	l.lwz	r4,1920(r4)
	l.sw	416(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000001
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r4,0(r2)
	l.lwz	r5,1600(r4)
	l.lwz	r3,1500(r4)
	l.sw	96(r3),r5
	l.lwz	r5,1604(r4)
	l.sw	100(r3),r5
	l.lwz	r5,1608(r4)
	l.sw	104(r3),r5
	l.lwz	r5,1612(r4)
	l.sw	108(r3),r5
	l.lwz	r4,1616(r4)
	l.sw	112(r3),r4
	l.movhi	r3,00000000
	l.addi	r4,r0,+0000000A
	l.jal	00018F10
	l.ori	r3,r3,0000C8A0
	l.lwz	r2,0(r2)
	l.lwz	r3,1500(r2)
	l.lwz	r4,2256(r2)
	l.sw	752(r3),r4
	l.lwz	r4,2272(r2)
	l.sw	768(r3),r4
	l.addi	r4,r0,+00000001
	l.lwz	r2,2276(r2)
	l.sw	772(r3),r2
	l.movhi	r3,00000000
	l.jal	00018F10
	l.ori	r3,r3,0000C768
	l.addi	r1,r1,+0000000C
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn000187E8: 000187E8
fn000187E8 proc
	l.or	r7,r4,r3
	l.sw	-4(r1),r2
	l.andi	r7,r7,00000003
	l.addi	r6,r0,+00000000
	l.sfne	r7,r6
	l.bf	00018820
	l.addi	r1,r1,-00000004

l00018804:
	l.j	00018834
	l.srli	r6,r5,00000002

l0001880C:
	l.add	r7,r3,r6
	l.lbz	r8,0(r8)
	l.addi	r6,r6,+00000001
	l.sb	0(r7),r8
	l.addi	r5,r5,-00000001

l00018820:
	l.sfnei	r5,00000000
	l.bf	0001880C
	l.add	r8,r4,r6

l0001882C:
	l.j	00018898
	l.addi	r1,r1,+00000004

l00018834:
	l.j	00018854
	l.sfnei	r6,00000000

l0001883C:
	l.add	r11,r3,r7
	l.lwz	r8,0(r8)
	l.addi	r7,r7,+00000004
	l.sw	0(r11),r8
	l.addi	r6,r6,-00000001
	l.sfnei	r6,00000000

l00018854:
	l.bf	0001883C
	l.add	r8,r4,r7

l0001885C:
	l.addi	r2,r0,-00000004
	l.and	r7,r5,r2
	l.andi	r5,r5,00000003
	l.add	r4,r4,r7
	l.j	00018888
	l.add	r7,r3,r7

l00018874:
	l.add	r8,r7,r6
	l.lbz	r11,0(r11)
	l.addi	r6,r6,+00000001
	l.sb	0(r8),r11
	l.addi	r5,r5,-00000001

l00018888:
	l.sfnei	r5,00000000
	l.bf	00018874
	l.add	r11,r4,r6

l00018894:
	l.addi	r1,r1,+00000004

l00018898:
	l.ori	r11,r3,00000000
	l.jr	r9
	l.lwz	r2,-4(r1)

;; fn000188A4: 000188A4
fn000188A4 proc
	l.j	000188B8
	l.ori	r6,r3,00000000

l000188AC:
	l.sb	0(r6),r4
	l.addi	r5,r5,-00000001
	l.addi	r6,r6,+00000001

l000188B8:
	l.sfnei	r5,00000000
	l.bf	000188AC
	l.nop

l000188C4:
	l.jr	r9
	l.ori	r11,r3,00000000

;; fn000188CC: 000188CC
;;   Called from:
;;     00018AF4 (in fn00018AC4)
;;     00019194 (in fn00019168)
;;     000191AC (in fn00019168)
;;     00019254 (in fn00019230)
;;     000192C8 (in fn000192A8)
fn000188CC proc
	l.slli	r6,r6,00000002
	l.addi	r7,r5,-00000001
	l.mul	r7,r6,r7
	l.add	r4,r4,r7
	l.slli	r7,r5,00000002
	l.sub	r8,r0,r6
	l.addi	r7,r7,-00000004
	l.j	00018904
	l.add	r3,r3,r7

l000188F0:
	l.add	r7,r4,r6
	l.addi	r5,r5,-00000001
	l.lwz	r7,0(r7)
	l.sw	0(r3),r7
	l.addi	r3,r3,-00000004

l00018904:
	l.sfnei	r5,00000000
	l.bf	000188F0
	l.add	r4,r4,r8

l00018910:
	l.jr	r9
	l.nop

;; fn00018918: 00018918
;;   Called from:
;;     00018C0C (in fn00018BDC)
;;     000191F8 (in fn000191CC)
;;     00019210 (in fn000191CC)
;;     00019290 (in fn0001926C)
;;     00019300 (in fn000192E0)
fn00018918 proc
	l.slli	r7,r5,00000002
	l.slli	r6,r6,00000002
	l.addi	r7,r7,-00000004
	l.sub	r8,r0,r6
	l.add	r4,r4,r7
	l.addi	r7,r5,-00000001
	l.mul	r7,r6,r7
	l.j	00018950
	l.add	r3,r3,r7

l0001893C:
	l.lwz	r11,0(r4)
	l.add	r7,r3,r6
	l.addi	r5,r5,-00000001
	l.sw	0(r7),r11
	l.addi	r4,r4,-00000004

l00018950:
	l.sfnei	r5,00000000
	l.bf	0001893C
	l.add	r3,r3,r8

l0001895C:
	l.jr	r9
	l.nop

;; fn00018964: 00018964
fn00018964 proc
	l.sw	-8(r1),r2
	l.movhi	r4,00000080
	l.ori	r2,r3,00000000
	l.lwz	r3,0(r3)
	l.sw	-4(r1),r9
	l.and	r3,r3,r4
	l.sfeqi	r3,00000000
	l.bf	00018A4C
	l.addi	r1,r1,-00000008

l00018988:
	l.jal	00018C24
	l.addi	r3,r0,+0000000D
	l.lwz	r3,24(r2)
	l.sfeqi	r3,00000000
	l.bf	00018A4C
	l.nop

l000189A0:
	l.andi	r3,r3,00000002
	l.sfeqi	r3,00000000
	l.bf	000189C4
	l.movhi	r3,000001F0

l000189B0:
	l.movhi	r5,00000008
	l.ori	r3,r3,00000CC4
	l.lwz	r4,0(r3)
	l.or	r4,r4,r5
	l.sw	0(r3),r4

l000189C4:
	l.lwz	r3,24(r2)
	l.andi	r3,r3,00000020
	l.sfeqi	r3,00000000
	l.bf	000189EC
	l.movhi	r3,000001F0

l000189D8:
	l.movhi	r5,00000020
	l.ori	r3,r3,00000CC4
	l.lwz	r4,0(r3)
	l.or	r4,r4,r5
	l.sw	0(r3),r4

l000189EC:
	l.lwz	r3,24(r2)
	l.andi	r3,r3,00000040
	l.sfeqi	r3,00000000
	l.bf	00018A14
	l.movhi	r3,000001F0

l00018A00:
	l.movhi	r5,00000040
	l.ori	r3,r3,00000CC4
	l.lwz	r4,0(r3)
	l.or	r4,r4,r5
	l.sw	0(r3),r4

l00018A14:
	l.lwz	r2,24(r2)
	l.andi	r2,r2,00000080
	l.sfeqi	r2,00000000
	l.bf	00018A3C
	l.movhi	r4,00000800

l00018A28:
	l.movhi	r2,000001F0
	l.ori	r2,r2,00000CC4
	l.lwz	r3,0(r2)
	l.or	r3,r3,r4
	l.sw	0(r2),r3

l00018A3C:
	l.jal	00018C24
	l.addi	r3,r0,+00000019
	l.jal	00018C24
	l.addi	r3,r0,+0000001A

l00018A4C:
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00018A60: 00018A60
fn00018A60 proc
	l.sw	-8(r1),r2
	l.lwz	r4,0(r3)
	l.movhi	r2,00000080
	l.sw	-4(r1),r9
	l.and	r4,r4,r2
	l.sfeqi	r4,00000000
	l.bf	00018AB0
	l.addi	r1,r1,-00000008

l00018A80:
	l.lwz	r3,24(r3)
	l.sfeqi	r3,00000000
	l.bf	00018AB0
	l.nop

l00018A90:
	l.movhi	r3,000001F0
	l.addi	r2,r0,+00000000
	l.ori	r3,r3,00000CC4
	l.sw	0(r3),r2
	l.jal	00018C68
	l.addi	r3,r0,+00000019
	l.jal	00018C68
	l.addi	r3,r0,+00000019

l00018AB0:
	l.addi	r1,r1,+00000008
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00018AC4: 00018AC4
fn00018AC4 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r1,r1,-00000010
	l.ori	r2,r3,00000000
	l.addi	r3,r0,+00000000
	l.movhi	r4,000001C2
	l.sw	4(r1),r3
	l.movhi	r3,00000001
	l.ori	r4,r4,00000800
	l.ori	r3,r3,0000310C
	l.addi	r5,r0,+00000097
	l.lwz	r3,0(r3)
	l.jal	000188CC
	l.addi	r6,r0,+00000001
	l.movhi	r3,00000000
	l.addi	r4,r0,+00000001
	l.jal	00018F10
	l.ori	r3,r3,0000C768
	l.j	00018BB8
	l.addi	r3,r0,+00000000

l00018B14:
	l.lwz	r3,4(r1)
	l.add	r4,r3,r3
	l.add	r3,r4,r3
	l.slli	r3,r3,00000002
	l.add	r3,r2,r3
	l.lwz	r3,120(r3)
	l.lwz	r6,0(r3)
	l.lwz	r5,4(r1)
	l.lwz	r3,4(r1)
	l.add	r7,r5,r5
	l.lwz	r4,4(r1)
	l.add	r5,r7,r5
	l.addi	r3,r3,+00000001
	l.slli	r5,r5,00000002
	l.add	r5,r2,r5
	l.lwz	r5,124(r5)
	l.xori	r5,r5,0000FFFF
	l.and	r5,r6,r5
	l.add	r6,r4,r4
	l.add	r4,r6,r4
	l.add	r6,r3,r3
	l.slli	r4,r4,00000002
	l.add	r3,r6,r3
	l.slli	r3,r3,00000002
	l.add	r4,r2,r4
	l.add	r3,r2,r3
	l.lwz	r4,124(r4)
	l.lwz	r3,116(r3)
	l.and	r4,r4,r3
	l.or	r3,r5,r4
	l.sw	0(r1),r3
	l.lwz	r3,4(r1)
	l.add	r4,r3,r3
	l.add	r3,r4,r3
	l.lwz	r4,0(r1)
	l.slli	r3,r3,00000002
	l.add	r3,r2,r3
	l.lwz	r3,120(r3)
	l.sw	0(r3),r4
	l.lwz	r3,4(r1)
	l.addi	r3,r3,+00000001

l00018BB8:
	l.sw	4(r1),r3
	l.lwz	r3,4(r1)
	l.sflesi	r3,+00000001
	l.bf	00018B14
	l.addi	r11,r0,+00000000

l00018BCC:
	l.addi	r1,r1,+00000010
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00018BDC: 00018BDC
fn00018BDC proc
	l.movhi	r3,000001C2
	l.sw	-4(r1),r9
	l.ori	r4,r3,00000068
	l.addi	r1,r1,-00000004
	l.lwz	r5,0(r4)
	l.ori	r3,r3,00000800
	l.ori	r5,r5,00000020
	l.addi	r6,r0,+00000001
	l.sw	0(r4),r5
	l.movhi	r4,00000001
	l.addi	r5,r0,+00000097
	l.ori	r4,r4,0000310C
	l.jal	00018918
	l.lwz	r4,0(r4)
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn00018C24: 00018C24
;;   Called from:
;;     00018988 (in fn00018964)
;;     00018A3C (in fn00018964)
;;     00018A44 (in fn00018964)
fn00018C24 proc
	l.sfnei	r3,00000000
	l.bf	00018C40
	l.movhi	r4,000001F0

l00018C30:
	l.ori	r4,r4,00000C10
	l.lwz	r5,0(r4)
	l.addi	r5,r0,+00000001
	l.sw	0(r4),r5

l00018C40:
	l.movhi	r4,000001F0
	l.addi	r6,r0,+00000001
	l.ori	r4,r4,00000C40
	l.sll	r3,r6,r3
	l.lwz	r5,0(r4)
	l.addi	r11,r0,+00000000
	l.or	r3,r3,r5
	l.sw	0(r4),r3
	l.jr	r9
	l.nop

;; fn00018C68: 00018C68
;;   Called from:
;;     00018AA0 (in fn00018A60)
;;     00018AA8 (in fn00018A60)
fn00018C68 proc
	l.addi	r6,r0,+00000001
	l.movhi	r4,000001F0
	l.sll	r3,r6,r3
	l.ori	r4,r4,00000C40
	l.addi	r11,r0,+00000000
	l.lwz	r5,0(r4)
	l.xori	r3,r3,0000FFFF
	l.and	r3,r3,r5
	l.sw	0(r4),r3
	l.jr	r9
	l.nop
00018C94             D7 E1 17 F8 D7 E1 4F FC 9C 21 FF F8     ......O..!..
00018CA0 07 FF FF 31 A8 43 00 00 84 62 00 00 18 80 00 08 ...1.C...b......
00018CB0 E0 63 20 03 BC 03 00 00 10 00 00 07 18 60 00 00 .c ..........`..
00018CC0 9C 80 00 00 04 00 00 93 A8 63 8F D0 07 FF FF D6 .........c......
00018CD0 9C 60 00 05 84 62 00 00 18 A0 00 10 E0 63 28 03 .`...b.......c(.
00018CE0 BC 03 00 00 10 00 00 04 15 00 00 00 07 FF FF CE ................
00018CF0 9C 60 00 08 84 62 00 00 18 80 00 20 E0 63 20 03 .`...b..... .c .
00018D00 BC 03 00 00 10 00 00 04 15 00 00 00 07 FF FF C6 ................
00018D10 9C 60 00 09 84 62 00 00 18 A0 00 40 E0 63 28 03 .`...b.....@.c(.
00018D20 BC 03 00 00 10 00 00 05 18 60 00 00 9C 80 00 00 .........`......
00018D30 04 00 00 78 A8 63 C9 24 84 62 00 00 18 80 01 00 ...x.c.$.b......
00018D40 E0 63 20 03 BC 03 00 00 10 00 00 0A 18 60 01 F0 .c ..........`..
00018D50 A8 63 0C C8 84 83 00 00 A8 84 3F 80 D4 03 20 00 .c........?... .
00018D60 07 FF FF B1 9C 60 00 1B 07 FF FF AF 9C 60 00 1C .....`.......`..
00018D70 84 62 00 00 18 A0 0A 00 E0 83 28 03 BC 04 00 00 .b........(.....
00018D80 10 00 00 18 18 80 08 00 E0 63 20 03 BC 03 00 00 .........c .....
00018D90 10 00 00 07 18 60 01 F0 18 A0 20 00 A8 63 0C C0 .....`.... ..c..
00018DA0 84 83 00 00 E0 84 28 04 D4 03 20 00 84 42 00 00 ......(... ..B..
00018DB0 18 60 02 00 E0 42 18 03 BC 02 00 00 10 00 00 07 .`...B..........
00018DC0 18 80 40 00 18 40 01 F0 A8 42 0C C0 84 62 00 00 ..@..@...B...b..
00018DD0 E0 63 20 04 D4 02 18 00 07 FF FF 93 9C 60 00 16 .c ..........`..
00018DE0 9C 21 00 08 9D 60 00 00 85 21 FF FC 44 00 48 00 .!...`...!..D.H.
00018DF0 84 41 FF F8 D7 E1 17 F8 18 80 00 08 A8 43 00 00 .A...........C..
00018E00 84 63 00 00 D7 E1 4F FC E0 63 20 03 BC 03 00 00 .c....O..c .....
00018E10 10 00 00 08 9C 21 FF F8 18 60 00 00 9C 80 00 00 .....!...`......
00018E20 04 00 00 3C A8 63 91 6C 07 FF FF 90 9C 60 00 05 ...<.c.l.....`..
00018E30 84 62 00 00 18 A0 00 40 E0 63 28 03 BC 03 00 00 .b.....@.c(.....
00018E40 10 00 00 05 18 60 00 00 9C 80 00 00 04 00 00 31 .....`.........1
00018E50 A8 63 C9 2C 07 FF FF 03 A8 62 00 00 18 80 30 87 .c.,.....b....0.
00018E60 84 62 00 00 A8 84 F0 00 E0 63 20 03 BC 03 00 00 .b.......c .....
00018E70 10 00 00 04 15 00 00 00 04 00 00 A0 15 00 00 00 ................
00018E80 84 62 00 00 18 A0 01 00 E0 63 28 03 BC 03 00 00 .b.......c(.....
00018E90 10 00 00 0B 18 60 01 F0 9C A0 C0 7F A8 63 0C C8 .....`.......c..
00018EA0 84 83 00 00 E0 84 28 03 D4 03 20 00 07 FF FF 6F ......(... ....o
00018EB0 9C 60 00 1B 07 FF FF 6D 9C 60 00 1C 84 42 00 00 .`.....m.`...B..
00018EC0 18 60 0A 00 E0 42 18 03 BC 02 00 00 10 00 00 08 .`...B..........
00018ED0 9C 80 00 00 18 40 01 F0 A8 42 0C C0 9C 60 00 16 .....@...B...`..
00018EE0 D4 02 20 00 07 FF FF 61 15 00 00 00 18 60 00 00 .. ....a.....`..
00018EF0 9C 80 00 00 04 00 00 07 A8 63 8D 18 9C 21 00 08 .........c...!..
00018F00 9D 60 00 00 85 21 FF FC 44 00 48 00 84 41 FF F8 .`...!..D.H..A..

;; fn00018F10: 00018F10
;;   Called from:
;;     00018344 (in fn00018250)
;;     00018410 (in fn00018250)
;;     0001845C (in fn00018250)
;;     000184B0 (in fn00018250)
;;     000184F4 (in fn00018250)
;;     00018540 (in fn00018250)
;;     00018584 (in fn00018250)
;;     000185D8 (in fn00018250)
;;     00018624 (in fn00018250)
;;     0001866C (in fn00018250)
;;     0001875C (in fn00018250)
;;     0001879C (in fn00018250)
;;     000187CC (in fn00018250)
;;     00018B04 (in fn00018AC4)
;;     0001914C (in fn000190F8)
fn00018F10 proc
	l.sw	-4(r1),r9
	l.addi	r1,r1,-00000004
	l.ori	r5,r3,00000000
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r5
	l.ori	r3,r4,00000000
00018F2C                                     D7 E1 4F FC             ..O.
00018F30 D7 E1 17 F8 9C 21 FF F8 04 00 00 DC A8 43 00 00 .....!.......C..
00018F40 04 00 00 8A 15 00 00 00 04 00 00 BA 15 00 00 00 ................
00018F50 07 FF FE DD A8 62 00 00 07 FF FC 2A 15 00 00 00 .....b.....*....
00018F60 04 00 00 EE 15 00 00 00 9C 21 00 08 9D 60 00 00 .........!...`..
00018F70 85 21 FF FC 44 00 48 00 84 41 FF F8 D7 E1 4F FC .!..D.H..A....O.
00018F80 D7 E1 17 F8 9C 21 FF F8 04 00 00 F4 A8 43 00 00 .....!.......C..
00018F90 07 FF FC B0 15 00 00 00 07 FF FF 11 A8 62 00 00 .............b..
00018FA0 04 00 00 B3 15 00 00 00 04 00 00 89 15 00 00 00 ................
00018FB0 04 00 00 CC 15 00 00 00 9C 21 00 08 9D 60 00 00 .........!...`..
00018FC0 85 21 FF FC 44 00 48 00 84 41 FF F8 D7 E1 4F FC .!..D.H..A....O.
00018FD0 D7 E1 17 EC D7 E1 77 F0 D7 E1 87 F4 D7 E1 97 F8 ......w.........
00018FE0 84 43 00 00 9C 21 FF D4 9C 60 00 36 9C 80 00 40 .C...!...`.6...@
00018FF0 D8 01 18 16 D8 01 18 17 9C 60 00 41 9E 01 00 12 .........`.A....
00019000 D8 01 18 15 9C 61 00 16 9D C0 00 02 D4 01 18 00 .....a..........
00019010 9C 61 00 14 D8 01 20 14 D4 01 18 04 18 60 00 00 .a.... ......`..
00019020 A8 81 00 00 A8 63 A2 04 D4 01 80 08 07 FF FF B9 .....c..........
00019030 D4 01 70 0C 18 60 00 01 A8 90 00 00 A8 63 37 60 ..p..`.......c7`
00019040 A8 AE 00 00 07 FF FD E9 AA 41 00 00 A8 70 00 00 .........A...p..
00019050 9C 80 00 00 07 FF FE 14 A8 AE 00 00 18 80 00 04 ................
00019060 E0 62 20 03 BC 03 00 00 10 00 00 05 18 80 00 02 .b .............
00019070 8C 61 00 13 A8 63 00 01 D8 01 18 13 E0 62 20 03 .a...c.......b .
00019080 BC 03 00 00 10 00 00 05 18 80 00 01 8C 61 00 13 .............a..
00019090 A8 63 00 02 D8 01 18 13 E0 62 20 03 BC 03 00 00 .c.......b .....
000190A0 10 00 00 04 8C 61 00 13 A8 63 00 20 D8 01 18 13 .....a...c. ....
000190B0 A4 42 80 00 BC 02 00 00 10 00 00 04 8C 41 00 13 .B...........A..
000190C0 A8 42 00 40 D8 01 10 13 18 60 00 00 A8 92 00 00 .B.@.....`......
000190D0 07 FF FF 90 A8 63 A1 DC 9C 21 00 2C 9D 60 00 00 .....c...!.,.`..
000190E0 85 21 FF FC 84 41 FF EC 85 C1 FF F0 86 01 FF F4 .!...A..........
000190F0 44 00 48 00 86 41 FF F8                         D.H..A..       

;; fn000190F8: 000190F8
fn000190F8 proc
	l.sw	-4(r1),r9
	l.sw	-8(r1),r2
	l.addi	r3,r0,+00000036
	l.addi	r1,r1,-0000001C
	l.addi	r2,r0,+00000040
	l.sb	18(r1),r3
	l.sb	19(r1),r3
	l.addi	r3,r1,+00000012
	l.sb	16(r1),r2
	l.sw	0(r1),r3
	l.addi	r3,r1,+00000010
	l.addi	r2,r0,+00000041
	l.sw	4(r1),r3
	l.movhi	r3,00000001
	l.ori	r4,r1,00000000
	l.ori	r3,r3,00003760
	l.sb	17(r1),r2
	l.sw	8(r1),r3
	l.addi	r3,r0,+00000002
	l.sw	12(r1),r3
	l.movhi	r3,00000000
	l.jal	00018F10
	l.ori	r3,r3,0000A1DC
	l.addi	r1,r1,+0000001C
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.lwz	r2,-8(r1)

;; fn00019168: 00019168
fn00019168 proc
	l.sw	-12(r1),r2
	l.movhi	r2,00000001
	l.sw	-8(r1),r14
	l.ori	r2,r2,0000310C
	l.movhi	r14,000001C1
	l.lwz	r3,0(r2)
	l.sw	-4(r1),r9
	l.ori	r4,r14,0000E000
	l.addi	r1,r1,-0000000C
	l.addi	r3,r3,+00000264
	l.addi	r5,r0,+0000001C
	l.jal	000188CC
	l.addi	r6,r0,+00000001
	l.lwz	r3,0(r2)
	l.ori	r4,r14,0000E100
	l.addi	r3,r3,+000002D4
	l.addi	r5,r0,+0000003F
	l.jal	000188CC
	l.addi	r6,r0,+00000001
	l.addi	r1,r1,+0000000C
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn000191CC: 000191CC
fn000191CC proc
	l.sw	-12(r1),r2
	l.movhi	r2,00000001
	l.sw	-8(r1),r14
	l.ori	r2,r2,0000310C
	l.movhi	r14,000001C1
	l.lwz	r4,0(r2)
	l.sw	-4(r1),r9
	l.ori	r3,r14,0000E000
	l.addi	r1,r1,-0000000C
	l.addi	r4,r4,+00000264
	l.addi	r5,r0,+0000001C
	l.jal	00018918
	l.addi	r6,r0,+00000001
	l.lwz	r4,0(r2)
	l.ori	r3,r14,0000E100
	l.addi	r4,r4,+000002D4
	l.addi	r5,r0,+0000003F
	l.jal	00018918
	l.addi	r6,r0,+00000001
	l.addi	r1,r1,+0000000C
	l.addi	r11,r0,+00000000
	l.lwz	r9,-4(r1)
	l.lwz	r2,-12(r1)
	l.jr	r9
	l.lwz	r14,-8(r1)

;; fn00019230: 00019230
fn00019230 proc
	l.movhi	r3,00000001
	l.movhi	r4,000001C2
	l.ori	r3,r3,0000310C
	l.sw	-4(r1),r9
	l.lwz	r3,0(r3)
	l.addi	r1,r1,-00000004
	l.addi	r3,r3,+000003D8
	l.ori	r4,r4,00003404
	l.addi	r5,r0,+00000006
	l.jal	000188CC
	l.addi	r6,r0,+00000003
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn0001926C: 0001926C
fn0001926C proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,0000310C
	l.addi	r1,r1,-00000004
	l.lwz	r4,0(r3)
	l.movhi	r3,000001C2
	l.addi	r4,r4,+000003D8
	l.ori	r3,r3,00003408
	l.addi	r5,r0,+00000006
	l.jal	00018918
	l.addi	r6,r0,+00000003
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn000192A8: 000192A8
fn000192A8 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,0000310C
	l.addi	r1,r1,-00000004
	l.lwz	r3,0(r3)
	l.movhi	r4,000001C0
	l.addi	r3,r3,+000004BC
	l.addi	r5,r0,+0000003D
	l.jal	000188CC
	l.addi	r6,r0,+00000001
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn000192E0: 000192E0
fn000192E0 proc
	l.movhi	r3,00000001
	l.sw	-4(r1),r9
	l.ori	r3,r3,0000310C
	l.addi	r1,r1,-00000004
	l.lwz	r4,0(r3)
	l.addi	r5,r0,+0000003D
	l.movhi	r3,000001C0
	l.addi	r4,r4,+000004BC
	l.jal	00018918
	l.addi	r6,r0,+00000001
	l.addi	r1,r1,+00000004
	l.lwz	r9,-4(r1)
	l.jr	r9
	l.addi	r11,r0,+00000000

;; fn00019318: 00019318
fn00019318 proc
	l.movhi	r4,00000001
	l.movhi	r3,00000172
	l.ori	r4,r4,00003764
	l.lwz	r5,8(r3)
	l.sw	0(r4),r3
	l.movhi	r4,00000001
	l.addi	r11,r0,+00000000
	l.ori	r4,r4,0000310C
	l.lwz	r4,0(r4)
	l.sw	1472(r4),r5
	l.lwz	r5,12(r3)
	l.sw	1476(r4),r5
	l.lwz	r3,32(r3)
	l.sw	1476(r4),r3
	l.jr	r9
	l.nop

;; fn00019358: 00019358
fn00019358 proc
	l.movhi	r3,00000001
	l.movhi	r4,00000001
	l.ori	r3,r3,00003764
	l.sw	-4(r1),r2
	l.lwz	r3,0(r3)
	l.addi	r2,r0,+00000000
	l.ori	r4,r4,0000310C
	l.sw	0(r3),r2
	l.lwz	r4,0(r4)
	l.addi	r1,r1,-00000004
	l.lwz	r5,1496(r4)
	l.ori	r11,r2,00000000
	l.sw	32(r3),r5
	l.lwz	r5,1472(r4)
	l.sw	8(r3),r5
	l.lwz	r4,1476(r4)
	l.sw	12(r3),r4
	l.addi	r4,r0,+00000001
	l.sw	0(r3),r4
	l.addi	r1,r1,+00000004
	l.jr	r9
	l.lwz	r2,-4(r1)
