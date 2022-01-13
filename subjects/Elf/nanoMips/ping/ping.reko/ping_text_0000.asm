;;; Segment .text (004000F0)

;; acknowledge: 004000F0
;;   Called from:
;;     004018A2 (in ping4_receive_error_msg)
;;     00401AB4 (in ping4_parse_reply)
acknowledge proc
	lwpc	r7,004544E4
	subu	r5,r7,r4
	andi	r6,r5,0000FFFF
	bbnezc	r5,0000000F,00400136

l004000FE:
	lwpc	r5,00430078
	bltc	r6,r5,00400110

l00400108:
	addiu	r6,r6,00000001
	swpc	r6,00430078

l00400110:
	aluipc	r6,00000400
	lhu	r8,0518(r6)
	subu	r5,r4,r8
	seh	r5,r5
	bltc	r0,r5,00400132

l00400124:
	andi	r7,r7,0000FFFF
	addiu	r5,r0,00007FFF
	subu	r7,r7,r8
	bgec	r5,r7,00400136

l00400132:
	sh	r4,0518(r6)

l00400136:
	jrc	ra

;; set_socket_option.isra.0.part.1: 00400138
;;   Called from:
;;     004007A6 (in main)
set_socket_option.isra.0.part.1 proc
	save	00000010,ra,00000001
	balc	004049B0
	lw	r4,0000(r4)
	balc	004049EA
	addiupc	r5,00010384
	move	r6,r4
	lwpc	r4,00412EF0
	balc	00408340
	li	r4,00000002
	balc	0040015A

;; exit: 0040015A
;;   Called from:
;;     00400156 (in set_socket_option.isra.0.part.1)
;;     00400156 (in set_socket_option.isra.0.part.1)
;;     004001EC (in main)
;;     00400B66 (in fn00400B66)
;;     00401E7E (in fn00401E7E)
;;     004029BE (in finish)
;;     004030A0 (in fn004030A0)
;;     00403598 (in ping6_usage)
;;     00403628 (in ping6_run)
;;     004049A6 (in __libc_start_main)
exit proc
	save	00000020,ra,00000001
	sw	r4,000C(sp)
	balc	00404A20
	balc	00404A22
	balc	0040E720
	lw	r17,000C(sp)
	balc	0040B230

;; main: 00400170
;;   Called from:
;;     0040016C (in exit)
;;     004049A4 (in __libc_start_main)
main proc
	save	00000060,ra,00000007
	li	r6,00000020
	li	r19,00000002
	movep	r16,r18,r4,r5
	move	r5,r0
	addu	r4,sp,r6
	balc	0040A690
	li	r7,00000001
	sw	r7,0028(sp)
	li	r7,00000011
	sw	r7,002C(sp)
	li	r7,FFFFFFFF
	sw	r7,0008(sp)
	sw	r7,0014(sp)
	sw	r0,000C(sp)
	sw	r0,0010(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)
	sw	r19,0020(sp)
	balc	00401CB6
	lw	r17,0000(r18)
	move.balc	r4,r17,0040A890
	addu	r4,r17,r4
	lbu	r7,-0001(r4)
	bneiuc	r7,00000034,004001D0

l004001AE:
	sw	r19,0024(sp)

l004001B0:
	addiupc	r6,00010A58
	movep	r4,r5,r16,r18
	balc	0040598A
	li	r7,FFFFFFFF
	beqc	r4,r7,0040071A

l004001C0:
	addiu	r4,r4,FFFFFFCC
	bgeiuc	r4,00000004,00400728

l004001C8:
	addiupc	r7,00011E94
	lwxs	r7,r4(r7)
	jrc	r7

l004001D0:
	bneiuc	r7,00000036,004001B0

l004001D4:
	li	r7,0000000A
	bc	004001F2
004001D8                         E9 34 94 9B AB 60 0E 2D         .4...`.-
004001E0 01 00 81 04 DE 06 00 2A 06 82                   .......*..      

l004001EA:
	li	r4,00000002
	balc	0040015A
	li	r7,00000002

l004001F2:
	sw	r7,0024(sp)
	bc	004001B0
004001F6                   81 D3 EF 60 2E 43 05 00 B1 1B       ...`.C....
00400200 EB 60 E6 42 05 00 E4 C8 0C 48 AB 60 E0 2C 01 00 .`.B.....H.`.,..
00400210 81 04 E4 06 D1 1B E7 80 20 00 EF 60 CC 42 05 00 ........ ..`.B..
00400220 8F 1B EB 60 C4 42 05 00 F4 C8 DF 2F 2B 62 B6 42 ...`.B...../+b.B
00400230 05 00 E7 80 00 02 A1 04 E2 06 EF 60 AC 42 05 00 ...........`.B..
00400240 20 0A EC A5 08 BA 0F 60 F0 42 05 00 63 1B A1 04  ......`.B..c...
00400250 D2 06 20 0A DA A5 81 D3 0C 9A A1 04 D2 06 20 0A .. ........... .
00400260 CE A5 0A BA 83 D3 EF 60 D0 42 05 00 43 1B AB 60 .......`.B..C..`
00400270 7C 2C 01 00 81 04 C4 06 6D 1B E9 34 E0 88 55 3F |,......m..4..U?
00400280 5B 1B 8B 60 60 42 05 00 00 2A BE 31 8F 60 2A 42 [..``B...*.1.`*B
00400290 05 00 00 2A 1A 47 C0 17 8C BB EB 60 1C 42 05 00 ...*.G.....`.B..
004002A0 E0 80 C0 E4 96 9B CB 60 3C 42 05 00 A1 04 A4 06 .......`<B......

l004002B0:
	lwpc	r4,00412EF0
	balc	00408340
	bc	004001EA
004002BC                                     EB 60 2A 42             .`*B
004002C0 05 00 E7 80 00 02 53 1B 8B 60 1A 42 05 00 00 2A ......S..`.B...*
004002D0 24 31 04 88 08 80 80 10 00 2A A0 32 0D 1B 83 D3 $1.......*.2....
004002E0 EA B4 CD 1A C0 00 00 20 EB 60 FE 41 05 00 EC 53 ....... .`.A...S
004002F0 29 1B C0 00 00 40 F1 1B 8B 60 EA 41 05 00 00 2A )....@...`.A...*
00400300 CE 98 8F 60 D0 41 05 00 80 A8 A5 BE AB 60 DE 2B ...`.A.......`.+
00400310 01 00 81 04 5A 06 CF 1A EB 60 CE 41 05 00 E7 80 ....Z....`.A....
00400320 40 00 F7 1A C8 E0 00 00 BF 1B 00 2A 82 46 C1 72 @..........*.F.r
00400330 40 94 8B 60 B0 41 05 00 00 2A 76 9C 71 FE 00 2A @..`.A...*v.q..*
00400340 6E 46 C0 17 E2 BB E1 34 F8 5F DC BB 81 E2 08 20 nF.....4._..... 
00400350 B3 82 80 F7 31 11 15 11 D4 84 20 82 F4 84 24 82 ....1..... ...$.
00400360 01 BC 00 2A 2A 43 40 BA D4 84 20 82 11 11 F4 84 ...**C@... .....
00400370 24 82 95 12 80 BE 00 2A 56 3E 80 A8 2A 80 11 11 $......*V>..*...
00400380 93 12 6B BC 80 BE 00 2A F6 3E 04 A8 1A 80 E1 E0 ..k....*.>......
00400390 08 20 11 11 C7 84 28 82 93 12 E7 84 2C 82 80 BE . ....(.....,...
004003A0 00 2A 2C 3E 04 A8 0C 80 AB 60 42 2B 01 00 81 04 .*,>.....`B+....
004003B0 EA 05 33 1A E1 E0 08 20 11 11 C7 84 30 82 93 12 ..3.... ....0...
004003C0 E7 84 34 82 80 BE 00 2A 66 3F 00 2A F2 42 EB 60 ..4....*f?.*.B.`
004003D0 18 41 05 00 8F 60 B2 FC 02 00 E7 80 02 00 3B 1A .A...`........;.
004003E0 2B 62 02 41 05 00 BA D2 20 0A 84 A3 5C 9A 20 0A +b.A.... ...\. .
004003F0 7E A4 64 12 0C BA AB 60 F4 2A 01 00 81 04 B8 05 ~.d....`.*......
00400400 E5 19 A5 D2 00 2A 68 A3 0E 9A 44 5C C9 B1 89 90 .....*h...D\....
00400410 18 B2 8F 60 10 41 05 00 C2 04 08 FE 0A D2 60 0B ...`.A........`.
00400420 9E 65 80 A8 0C 80 CB 60 BC 40 05 00 A1 04 A0 05 .e.....`.@......
00400430 7F 1A C0 00 00 80 EB 60 B0 40 05 00 EC 53 EF 60 .......`.@...S.`
00400440 A8 40 05 00 60 0A E6 4A 67 19 C2 04 02 FC 02 D2 .@..`..Jg.......
00400450 20 0B 6C 65 80 88 06 80 C0 00 00 80 8B 1A EB 60  .le...........`
00400460 84 40 05 00 EF 60 BE 40 05 00 45 19 8B 60 76 40 .@...`.@..E..`v@
00400470 05 00 BE 3B 8F 60 0E FC 02 00 80 A8 0A 80 C1 E0 ...;.`..........
00400480 00 00 A1 04 6E 05 29 1A E1 E0 00 00 87 88 06 80 ....n.).........
00400490 EF 60 F2 FB 02 00 EB 60 78 40 05 00 E0 88 11 3D .`.....`x@.....=
004004A0 EB 60 E2 FB 02 00 F8 C8 07 25 AB 60 40 2A 01 00 .`.......%.`@*..
004004B0 81 04 6C 05 31 19 C1 E0 00 00 2D 1A 0A D3 C1 72 ..l.1.....-....r
004004C0 8B 60 22 40 05 00 00 2A 58 9B 8F 60 F8 3F 05 00 .`"@...*X..`.?..
004004D0 04 A8 06 80 E1 34 F8 5F 8C 9B AB 60 10 2A 01 00 .....4._...`.*..
004004E0 81 04 64 05 01 19 C4 E0 00 00 FD 19 2B 62 F6 3F ..d.........+b.?
004004F0 05 00 A1 04 6E 05 20 0A 36 A3 82 D3 1E 9A A1 04 ....n. .6.......
00400500 66 05 20 0A 2A A3 08 BA 0F 60 3A FB 02 00 A1 18 f. .*....`:.....
00400510 A1 04 5C 05 20 0A 18 A3 0A BA 81 D3 EF 60 26 FB ..\. ........`&.
00400520 02 00 8D 18 AB 60 C6 29 01 00 81 04 4A 05 B7 18 .....`.)....J...
00400530 EB 60 B6 3F 05 00 E7 80 04 00 DF 18 D0 E0 00 00 .`.?............
00400540 A7 19 EB 60 A4 3F 05 00 A0 10 E7 80 05 00 8B 60 ...`.?.........`
00400550 A0 29 01 00 EF 60 92 3F 05 00 00 2A 82 82 51 18 .)...`.?...*..Q.
00400560 EB 60 86 3F 05 00 C0 60 00 F4 01 00 E7 80 08 00 .`.?...`........
00400570 A3 60 CA 29 03 00 8B 60 6C 3F 05 00 EF 60 6A 3F .`.)...`l?...`j?
00400580 05 00 00 2A 92 17 29 18 EB 60 5E 3F 05 00 E7 80 ...*..)..`^?....
00400590 10 00 87 18 2B 62 4E 3F 05 00 98 5F F1 C8 24 80 ....+bN?..._..$.
004005A0 99 5F E7 80 DF 20 F2 C8 1A C0 10 D3 C1 72 91 00 ._... .......r..
004005B0 02 00 00 2A 7A 9A E1 34 F8 5F 90 9B D1 10 A1 04 ...*z..4._......
004005C0 F2 04 ED 18 0A D3 C1 72 91 10 E7 1B E0 00 FF 00 .......r........
004005D0 87 88 0C 80 AB 60 16 29 01 00 81 04 F6 04 07 18 .....`.)........
004005E0 8F 60 4A 3F 05 00 8B 60 FC 3E 05 00 00 2A 5A 2E .`J?...`.>...*Z.
004005F0 8F 60 0E 3F 05 00 00 2A B6 43 C0 17 8E BB EB 60 .`.?...*.C.....`
00400600 00 3F 05 00 E0 80 C0 E1 E0 88 A5 3B CB 60 D6 3E .?.........;.`.>
00400610 05 00 A1 04 FE 04 99 18 EB 60 CE 3E 05 00 E7 80 .........`.>....
00400620 80 00 FF 29 F5 FB 8B 60 BC 3E 05 00 04 3A C4 10 ...)...`.>...:..
00400630 8F 60 3E FA 02 00 04 88 06 80 A1 04 F2 04 71 18 .`>...........q.
00400640 E0 60 F8 F3 01 00 87 88 67 BB A1 04 0A 05 61 18 .`......g.....a.
00400650 8B 60 92 3E 05 00 DA 39 8F 60 9E 3E 05 00 80 A8 .`.>...9.`.>....
00400660 4F BB AB 60 88 28 01 00 81 04 10 05 FF 29 77 FB O..`.(.......)w.
00400670 C2 E0 00 00 EB 60 72 3E 05 00 EC 53 8B 60 66 3E .....`r>...S.`f>
00400680 05 00 EF 60 64 3E 05 00 A8 39 E0 00 FF 00 C4 10 ...`d>...9......
00400690 8F 60 7A 3E 05 00 87 88 17 FB A1 04 FA 04 11 18 .`z>............
004006A0 C0 00 00 10 43 18 EB 60 40 3E 05 00 E7 80 00 01 ....C..`@>......
004006B0 FF 29 67 FB A1 04 FC 04 81 04 04 05 00 2A 00 80 .)g..........*..
004006C0 80 10 FF 29 27 FB 8B 60 1C 3E 05 00 64 39 8F 60 ...)'..`.>..d9.`
004006D0 F4 0D 03 00 04 88 D9 BA AB 60 12 28 01 00 81 04 .........`.(....
004006E0 FA 04 FF 29 01 FB 8B 60 FC 3D 05 00 44 39 E0 00 ...)...`.=..D9..
004006F0 63 08 8F 60 8C F9 02 00 87 88 0E C0 AB 60 EE 27 c..`.........`.'
00400700 01 00 81 04 EE 04 FF 29 DD FA E0 00 E8 03 8F 3C .......).......<
00400710 8F 60 6E F9 02 00 FF 29 97 FA                   .`n....)..      

l0040071A:
	lwpc	r7,00430254
	subu	r16,r16,r7
	lsa	r18,r7,r18,00000002
	bnezc	r16,0040072C

l00400728:
	balc	0040094E

l0040072C:
	move	r7,r16
	li	r4,00000001
	addiu	r7,r7,3FFFFFFF
	lwxs	r20,r7(r18)
	balc	00401CE2
	lw	r18,0024(sp)
	beqic	r8,0000000A,00400754

l00400744:
	lw	r17,0028(sp)
	seqi	r8,r8,00000002
	li	r7,00000001
	li	r5,00000002
	addiu	r4,sp,00000008
	balc	00400966

l00400754:
	lw	r17,0024(sp)
	beqic	r7,00000002,0040076E

l0040075A:
	lw	r18,0008(sp)
	li	r7,0000003A
	lw	r17,0028(sp)
	li	r5,0000000A
	addiu	r8,r8,00000001
	addiu	r4,sp,00000014
	sltiu	r8,r8,00000001
	balc	00400966

l0040076E:
	move	r4,r0
	balc	00401CE2
	lw	r17,0024(sp)
	bnezc	r7,00400784

l00400778:
	lw	r17,0008(sp)
	li	r7,FFFFFFFF
	bnec	r7,r6,004007AA

l00400780:
	li	r7,0000000A

l00400782:
	sw	r7,0024(sp)

l00400784:
	lwpc	r7,00454530
	beqzc	r7,004007B4

l0040078C:
	lw	r17,0008(sp)
	li	r17,FFFFFFFF
	beqc	r17,r4,004007B4

l00400794:
	addiu	r8,r0,00000004
	addiupc	r7,00053D94
	li	r6,00000001
	move	r5,r0
	balc	00407D60
	bnec	r17,r4,004007B4

l004007A6:
	balc	00400138

l004007AA:
	lw	r17,0014(sp)
	bnec	r7,r6,00400784

l004007B0:
	li	r7,00000002
	bc	00400782

l004007B4:
	lwpc	r7,00454504
	beqzc	r7,004007D6

l004007BC:
	lw	r17,0014(sp)
	li	r17,FFFFFFFF
	beqc	r17,r4,004007D6

l004007C2:
	addiu	r8,r0,00000004
	addiupc	r7,00053D3A
	li	r6,00000043
	li	r5,00000029
	balc	00407D60
	beqc	r17,r4,004007A6

l004007D6:
	addiu	r7,sp,00000004
	addiu	r6,sp,00000020
	movep	r4,r5,r20,r0
	balc	00405E20
	lw	r4,0004(sp)
	move	r19,r4
	beqzc	r4,0040081E

l004007E6:
	balc	00405E00
	addiupc	r5,0000FD22
	movep	r6,r7,r20,r4
	lwpc	r4,00412EF0
	balc	00408340
	bc	004001EA

l004007FE:
	addiu	r7,sp,00000008
	movep	r5,r6,r18,r17
	move.balc	r4,r16,00400B6A

l00400806:
	addiu	r7,sp,00000014
	movep	r5,r6,r18,r17
	move.balc	r4,r16,004035A0
	move	r19,r4
	bnezc	r4,0040081C

l00400812:
	lw	r17,0004(sp)
	balc	00405DF0
	move	r4,r19
	restore.jrc	00000060,ra,00000007

l0040081C:
	lw	r17,001C(r17)

l0040081E:
	beqzc	r17,00400812

l00400820:
	lw	r6,0004(r17)
	beqic	r6,00000002,004007FE

l00400826:
	beqic	r6,0000000A,00400806

l0040082A:
	addiupc	r5,00010416
	bc	004002B0

;; fn00400832: 00400832
fn00400832 proc
	bc	00409BD0
00400836                   00 00 00 00 00 00 00 00 00 00       ..........

;; _start: 00400840
_start proc
	move	r30,r0
	addiupc	r25,0000001A
	addiupc	r5,FFFFF7B6
	addiupc	r28,0002FB62
	move	r4,sp
	addiu	r1,r0,FFFFFFF0
	and	sp,sp,r1
	jalrc	ra,r25
	nop
	nop

;; _start_c: 00400860
;;   Called from:
;;     00400858 (in _start)
;;     0040085C (in _start)
_start_c proc
	save	00000010,ra,00000001
	lw	r5,0000(r4)
	addiu	r6,r4,00000004
	move	r9,r0
	addiupc	r8,0000FAD2
	addiupc	r7,FFFFF860
	addiupc	r4,FFFFF8F8
	balc	00404988
	sigrie	00000000

;; deregister_tm_clones: 00400880
;;   Called from:
;;     004008CC (in __do_global_dtors_aux)
deregister_tm_clones proc
	addiupc	r7,0002FB2C
	addiupc	r4,0002FB28
	addiu	r7,r7,00000003
	subu	r7,r7,r4
	bltiuc	r7,00000007,0040089A

l00400890:
	addiupc	r7,FFBFF76A
	beqzc	r7,0040089A

l00400898:
	jrc	r7

l0040089A:
	jrc	ra

;; register_tm_clones: 0040089C
;;   Called from:
;;     00400906 (in frame_dummy)
register_tm_clones proc
	addiupc	r4,0002FB10
	li	r6,00000002
	addiupc	r7,0002FB0A
	subu	r7,r7,r4
	sra	r7,r7,00000002
	div	r5,r7,r6
	beqzc	r5,004008BC

l004008B2:
	addiupc	r7,FFBFF748
	beqzc	r7,004008BC

l004008BA:
	jrc	r7

l004008BC:
	jrc	ra

;; __do_global_dtors_aux: 004008BE
__do_global_dtors_aux proc
	save	00000010,ra,00000002
	aluipc	r7,00000400
	lbu	r6,03C0(r7)
	move	r16,r7
	bnezc	r6,004008E4

l004008CC:
	balc	00400880
	addiupc	r7,FFBFF72A
	beqzc	r7,004008DE

l004008D8:
	addiupc	r4,00012CB4
	jalrc	ra,r7

l004008DE:
	li	r7,00000001
	sb	r7,03C0(r16)

l004008E4:
	restore.jrc	00000010,ra,00000002

;; frame_dummy: 004008E6
frame_dummy proc
	save	00000010,ra,00000001
	addiupc	r7,FFBFF712
	beqzc	r7,004008FA

l004008F0:
	addiupc	r5,0002FAD0
	addiupc	r4,00012C98
	jalrc	ra,r7

l004008FA:
	addiupc	r4,0002F6FE
	lw	r7,0000(r4)
	bnezc	r7,0040090A

l00400902:
	restore	00000010,ra,00000001
	bc	0040089C

l0040090A:
	addiupc	r7,FFBFF6F0
	beqzc	r7,00400902

l00400912:
	jalrc	ra,r7
	bc	00400902
00400916                   00 00 00 00 00 00 00 00 00 00       ..........

;; in_cksum: 00400920
;;   Called from:
;;     00400A96 (in ping4_send_probe)
;;     00400ACE (in ping4_send_probe)
;;     004019A0 (in ping4_parse_reply)
in_cksum proc
	bc	0040092C

l00400922:
	addiu	r4,r4,00000002
	addiu	r5,r5,FFFFFFFE
	lhu	r7,-0002(r4)
	addu	r6,r6,r7

l0040092C:
	bgeic	r5,00000002,00400922

l00400930:
	bneiuc	r5,00000001,00400938

l00400934:
	lbu	r7,0000(r4)
	addu	r6,r6,r7

l00400938:
	sra	r4,r6,00000010
	andi	r6,r6,0000FFFF
	addu	r6,r4,r6
	sra	r4,r6,00000010
	addu	r6,r6,r4
	nor	r4,r0,r6
	andi	r4,r4,0000FFFF
	jrc	ra

;; usage: 0040094E
;;   Called from:
;;     00400728 (in main)
;;     00400B80 (in ping4_run)
usage proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,0000F9FE
	balc	00400B5A
	li	r4,00000001
	balc	0040357C
	li	r4,00000002
	balc	00400B66

;; create_socket: 00400966
;;   Called from:
;;     00400750 (in main)
;;     0040076A (in main)
;;     00400964 (in usage)
create_socket proc
	save	00000020,ra,00000006
	movep	r17,r19,r4,r5
	movep	r16,r20,r6,r7
	move	r18,r8

l0040096E:
	balc	00400B56
	sw	r0,0000(sp)
	movep	r5,r6,r16,r20
	move.balc	r4,r19,00407D80
	li	r7,FFFFFFFF
	sw	r4,0040(sp)
	bnec	r7,r4,004009B0

l00400980:
	balc	00400B56
	lw	r7,0000(r4)
	beqic	r7,00000021,004009B0

l00400988:
	bneiuc	r16,00000001,004009B0

l0040098C:
	lwpc	r7,004544EC
	li	r16,00000003
	bbeqzc	r7,00000008,0040096E

l00400998:
	balc	00400B56
	lw	r4,0000(r4)
	balc	004049EA
	addiupc	r5,0000FAD8
	move	r6,r4
	lwpc	r4,00412EF0
	balc	00400B5E
	bc	0040096E

l004009B0:
	lw	r6,0000(r17)
	li	r7,FFFFFFFF
	bnec	r7,r6,004009EA

l004009B8:
	beqzc	r18,004009D6

l004009BA:
	balc	00400B56
	lw	r4,0000(r4)
	balc	004049EA
	addiupc	r5,0000FAE2
	move	r6,r4
	lwpc	r4,00412EF0
	balc	00400B5E
	beqzc	r18,004009EC

l004009D2:
	li	r4,00000002
	balc	00400B66

l004009D6:
	balc	00400B56
	lw	r7,0000(r4)
	bneiuc	r7,00000021,004009BA

l004009DE:
	lwpc	r7,004544EC
	bbnezc	r7,00000008,004009BA

l004009E8:
	restore.jrc	00000020,ra,00000006

l004009EA:
	sw	r16,0001(r17)

l004009EC:
	restore.jrc	00000020,ra,00000006

;; pr_echo_reply: 004009EE
pr_echo_reply proc
	save	00000010,ra,00000001
	lhu	r4,0006(r4)
	balc	00407630
	restore	00000010,ra,00000001
	move	r5,r4
	addiupc	r4,0000FABC
	bc	004086C0

;; write_stdout: 00400A04
;;   Called from:
;;     00401BA0 (in fn00401BA0)
write_stdout proc
	save	00000010,ra,00000004
	move	r16,r0
	movep	r18,r17,r4,r5

l00400A0A:
	subu	r6,r17,r16
	addu	r5,r18,r16
	li	r4,00000001
	balc	0040B080
	addu	r16,r16,r4
	bltuc	r16,r17,00400A0A

l00400A1A:
	bltc	r4,r0,00400A0A

l00400A1E:
	restore.jrc	00000010,ra,00000004

;; ping4_send_probe: 00400A20
ping4_send_probe proc
	save	00000020,ra,00000004
	movep	r18,r16,r4,r5
	li	r7,00000008
	sb	r7,0000(r16)
	sb	r0,0001(r16)
	lwpc	r4,004544E4
	sh	r0,0002(r16)
	addiu	r4,r4,00000001
	andi	r4,r4,0000FFFF
	balc	00400B62
	lwpc	r7,004544C4
	sh	r7,0004(r16)
	lwpc	r7,004544E4
	addiu	r7,r7,00000001
	sh	r4,0006(r16)
	andi	r6,r7,0000FFFF
	addiupc	r7,000518F0
	srl	r5,r6,00000005
	lsa	r5,r5,r7,00000002
	li	r7,00000001
	sllv	r7,r7,r6
	lw	r6,0000(r5)
	nor	r7,r0,r7
	and	r7,r7,r6
	sw	r7,0040(sp)
	lwpc	r7,00454508
	beqzc	r7,00400A8C

l00400A6E:
	lwpc	r7,004544EC
	addiu	r17,r16,00000008
	bbeqzc	r7,0000000C,00400AEA

l00400A7A:
	move	r5,r0
	addiu	r4,sp,00000008
	balc	0040AF40
	li	r6,00000008
	addu	r5,sp,r6
	move.balc	r4,r17,0040A130

l00400A8C:
	lwpc	r17,00430074
	addiu	r17,r17,00000008
	movep	r5,r6,r17,r0
	move.balc	r4,r16,00400920
	lwpc	r7,00454508
	sh	r4,0002(r16)
	beqzc	r7,00400AD2

l00400AA4:
	lwpc	r7,004544EC
	bbnezc	r7,0000000C,00400AD2

l00400AAE:
	move	r5,r0
	addiu	r4,sp,00000008
	balc	0040AF40
	li	r6,00000008
	addu	r5,sp,r6
	addu	r4,r16,r6
	balc	00400B52
	lhu	r6,0002(r16)
	li	r5,00000008
	nor	r6,r0,r6
	addu	r4,sp,r5
	andi	r6,r6,0000FFFF
	balc	00400920
	sh	r4,0002(r16)

l00400AD2:
	lw	r4,0000(r18)
	addiu	r9,r0,00000010
	movep	r6,r7,r17,r0
	addiupc	r8,000309AA
	move.balc	r5,r16,00407D40
	xor	r17,r17,r4
	movz	r4,r0,r17

l00400AE8:
	restore.jrc	00000020,ra,00000004

l00400AEA:
	li	r6,00000008
	movep	r4,r5,r17,r0
	balc	00400B4E
	bc	00400A8C

;; ping4_install_filter: 00400AF2
ping4_install_filter proc
	save	00000020,ra,00000003
	lwpc	r7,0045451C
	move	r17,r4
	bnezc	r7,00400B48

l00400AFE:
	lwpc	r7,004544C4
	li	r16,00000001
	sw	r7,000C(sp)
	swpc	r16,0045451C
	lhu	r4,000C(sp)
	balc	00400B62
	addiupc	r7,0002F4F0
	sb	r0,0012(r7)
	li	r6,00000015
	sb	r16,0013(r7)
	addiu	r8,r0,00000008
	sw	r4,0054(sp)
	addiu	r5,r0,0000FFFF
	lw	r4,0000(r17)
	sh	r6,0010(r7)
	addiupc	r7,0002F4CA
	li	r6,0000001A
	balc	00400B4A
	beqzc	r4,00400B48

l00400B3C:
	addiupc	r4,0000F9A4
	restore	00000020,ra,00000003
	bc	00408630

l00400B48:
	restore.jrc	00000020,ra,00000003

;; fn00400B4A: 00400B4A
;;   Called from:
;;     00400B38 (in ping4_install_filter)
;;     00400D28 (in ping4_run)
;;     00400D4C (in ping4_run)
;;     00400EEE (in ping4_run)
fn00400B4A proc
	bc	00407D60

;; fn00400B4E: 00400B4E
;;   Called from:
;;     00400AEE (in ping4_send_probe)
;;     00400BE6 (in ping4_run)
;;     00400CAA (in ping4_run)
;;     00400E50 (in ping4_run)
fn00400B4E proc
	bc	0040A690

;; fn00400B52: 00400B52
;;   Called from:
;;     00400ABE (in ping4_send_probe)
;;     00400BDE (in ping4_run)
;;     00400C7C (in ping4_run)
fn00400B52 proc
	bc	0040A130

;; fn00400B56: 00400B56
;;   Called from:
;;     0040096E (in create_socket)
;;     00400980 (in create_socket)
;;     00400998 (in create_socket)
;;     004009BA (in create_socket)
;;     004009D6 (in create_socket)
;;     00400D86 (in ping4_run)
fn00400B56 proc
	bc	004049B0

;; fn00400B5A: 00400B5A
;;   Called from:
;;     0040095A (in usage)
;;     00400DA8 (in ping4_run)
;;     00400E0A (in ping4_run)
fn00400B5A proc
	bc	004083F0

;; fn00400B5E: 00400B5E
;;   Called from:
;;     004009AC (in create_socket)
;;     004009CE (in create_socket)
;;     00400BCC (in ping4_run)
;;     00400D12 (in ping4_run)
;;     00400E30 (in ping4_run)
fn00400B5E proc
	bc	00408340

;; fn00400B62: 00400B62
;;   Called from:
;;     00400A36 (in ping4_send_probe)
;;     00400B12 (in ping4_install_filter)
;;     00400D5E (in ping4_run)
fn00400B62 proc
	bc	00406700

;; fn00400B66: 00400B66
;;   Called from:
;;     00400964 (in usage)
;;     004009D4 (in create_socket)
;;     00400C98 (in ping4_run)
fn00400B66 proc
	bc	0040015A

;; ping4_run: 00400B6A
;;   Called from:
;;     00400802 (in main)
ping4_run proc
	save	00000180,ra,00000008
	movep	r17,r21,r4,r5
	movep	r18,r16,r6,r7
	bltic	r17,00000002,00400C24

l00400B76:
	lwpc	r7,004544EC
	bbeqzc	r7,00000005,00400B82

l00400B80:
	balc	0040094E

l00400B82:
	bbeqzc	r7,00000009,00400B96

l00400B86:
	lwpc	r7,0045453C
	bneiuc	r7,00000003,00400B80

l00400B90:
	bltic	r17,00000006,00400C24

l00400B94:
	bc	00400B80

l00400B96:
	bgeic	r17,0000000B,00400B80

l00400B9A:
	ori	r7,r7,00000400
	swpc	r7,004544EC
	bc	00400C24

l00400BA6:
	sw	r0,0038(sp)
	bneiuc	r17,00000001,00400BAE

l00400BAC:
	bnezc	r18,00400BD2

l00400BAE:
	addiu	r7,sp,00000038
	addiupc	r6,00011648
	movep	r4,r5,r20,r0
	balc	00405E20
	beqzc	r4,00400BD0

l00400BBC:
	balc	00405E00
	addiupc	r5,0000F94C
	movep	r6,r7,r20,r4

l00400BC6:
	lwpc	r4,00412EF0
	balc	00400B5E
	bc	00400C96

l00400BD0:
	lw	r4,0038(sp)

l00400BD2:
	lw	r5,0014(r18)
	li	r6,00000010
	addiupc	r4,000308AE
	addiu	r20,sp,00000060
	balc	00400B52
	addiu	r6,r0,000000FF
	movep	r4,r5,r20,r0
	balc	00400B4E
	lw	r5,0018(r18)
	beqzc	r5,00400BF4

l00400BEC:
	addiu	r6,r0,000000FE
	move.balc	r4,r20,0040A930

l00400BF4:
	lw	r17,0038(sp)
	swpc	r20,004544C0
	beqzc	r4,00400C02

l00400BFE:
	balc	00405DF0

l00400C02:
	beqic	r17,00000001,00400C20

l00400C06:
	lwpc	r7,00454538
	lw	r5,0004(r19)
	addiu	r6,r7,00000001
	swpc	r6,00454538
	addiupc	r6,0003087C
	swxs	r5,r7(r6)

l00400C20:
	addiu	r17,r17,FFFFFFFF
	addiu	r21,r21,00000004

l00400C24:
	bgec	r0,r17,00400C6A

l00400C28:
	lw	r20,0000(r21)
	addiupc	r19,0003085A
	aluipc	r7,00000400
	li	r6,00000002
	sw	r0,0488(r7)
	addiupc	r5,00030850
	sw	r0,0044(sp)
	sw	r0,0048(sp)
	sw	r0,004C(sp)
	sh	r6,0488(r7)
	move.balc	r4,r20,004067A0
	bneiuc	r4,00000001,00400BA6

l00400C4E:
	swpc	r20,004544C0
	bneiuc	r17,00000001,00400C06

l00400C58:
	lwpc	r7,004544EC
	ori	r7,r7,00000004
	swpc	r7,004544EC
	bc	00400C20

l00400C6A:
	addiupc	r18,0002F3DE
	lw	r7,0004(r18)
	bnec	r0,r7,00400E36

l00400C74:
	li	r6,00000010
	addiupc	r5,0003080E
	addiu	r4,sp,00000028
	balc	00400B52
	move	r6,r0
	li	r5,00000001
	li	r4,00000002
	balc	00407D80
	move	r17,r4
	bgec	r4,r0,00400C9A

l00400C8E:
	addiupc	r4,0000F88E

l00400C92:
	balc	00408630

l00400C96:
	li	r4,00000002
	balc	00400B66

l00400C9A:
	lwpc	r19,00454528
	beqc	r0,r19,00400D38

l00400CA4:
	li	r6,00000020
	move	r5,r0
	addiu	r4,sp,00000038
	balc	00400B4E
	li	r6,0000000F
	addiu	r4,sp,00000038
	move.balc	r5,r19,0040A930
	li	r4,00000001
	balc	00401CE2
	lwpc	r19,00454528
	move.balc	r4,r19,0040A890
	move	r7,r19
	addiu	r8,r4,00000001
	li	r6,00000019
	addiu	r5,r0,0000FFFF
	move.balc	r4,r17,00407D60
	move	r19,r4
	move	r4,r0
	balc	00401CE2
	li	r7,FFFFFFFF
	bnec	r7,r19,00400D38

l00400CE2:
	lw	r17,002C(sp)
	balc	00407620
	lui	r7,FFFE0000
	ins	r4,r0,00000000,00000001
	bnec	r7,r4,00400D32

l00400CF4:
	addiu	r6,sp,00000038
	addiu	r5,r0,00008933
	move.balc	r4,r17,00405B80
	bgec	r4,r0,00400D16

l00400D02:
	lwpc	r6,00454528
	addiupc	r5,0000F81C
	lwpc	r4,00412EF0
	balc	00400B5E
	bc	00400C96

l00400D16:
	lw	r17,0048(sp)
	addiu	r8,r0,0000000C
	li	r6,00000020
	sw	r0,001C(sp)
	sw	r7,0024(sp)
	addiu	r7,sp,0000001C
	movep	r4,r5,r17,r0
	sw	r0,0020(sp)
	balc	00400B4A
	bnec	r19,r4,00400D38

l00400D2C:
	addiupc	r4,0000F810
	bc	00400C92

l00400D32:
	addiupc	r4,0000F822
	bc	00400C92

l00400D38:
	lwpc	r7,00454530
	beqzc	r7,00400D5A

l00400D40:
	addiu	r8,r0,00000004
	addiupc	r7,000537E8
	li	r6,00000001
	movep	r4,r5,r17,r0
	balc	00400B4A
	bgec	r4,r0,00400D5A

l00400D52:
	addiupc	r4,0000F81A
	balc	00408630

l00400D5A:
	addiu	r4,r0,00000401
	balc	00400B62
	lwpc	r7,00454538
	sh	r4,002A(sp)
	beqzc	r7,00400D76

l00400D6C:
	aluipc	r7,00000400
	lw	r7,0498(r7)
	sw	r7,002C(sp)

l00400D76:
	li	r6,00000010
	addiu	r5,sp,00000028
	move.balc	r4,r17,00405DD0
	li	r7,FFFFFFFF
	move	r19,r4
	bnec	r7,r4,00400DD6

l00400D86:
	balc	00400B56
	lw	r7,0000(r4)
	bneiuc	r7,0000000D,00400DD0

l00400D8E:
	lwpc	r7,00412EF0
	sw	r7,000C(sp)
	lwpc	r7,0045452C
	lw	r17,000C(sp)
	addiupc	r4,0000F7F2
	beqzc	r7,00400E0A

l00400DA4:
	addiupc	r4,0000F814
	balc	00400B5A
	addiu	r8,r0,00000004
	addiupc	r7,0005377A
	li	r6,00000020
	addiu	r5,r0,0000FFFF
	move.balc	r4,r17,00407D60
	bgec	r4,r0,00400DC6

l00400DC0:
	addiupc	r4,0000F81C
	bc	00400C92

l00400DC6:
	li	r6,00000010
	addiu	r5,sp,00000028
	move.balc	r4,r17,00405DD0
	bnec	r19,r4,00400DD6

l00400DD0:
	addiupc	r4,0000F824
	bc	00400C92

l00400DD6:
	li	r7,00000010
	addiu	r6,sp,0000001C
	addiupc	r5,0002F26E
	sw	r7,001C(sp)
	move.balc	r4,r17,004066B0
	li	r7,FFFFFFFF
	bnec	r4,r7,00400DEE

l00400DE8:
	addiupc	r4,0000F814
	bc	00400C92

l00400DEE:
	lwpc	r7,00454528
	sh	r0,0002(r18)
	beqzc	r7,00400E32

l00400DF8:
	addiu	r4,sp,00000038
	balc	004062A2
	beqzc	r4,00400E0E

l00400E00:
	lwpc	r5,00412EF0
	addiupc	r4,0000F802

l00400E0A:
	balc	00400B5A
	bc	00400C96

l00400E0E:
	lw	r5,0038(sp)
	lwpc	r22,00454528
	move	r19,r21

l00400E18:
	bnezc	r19,00400E96

l00400E1A:
	move.balc	r4,r21,00406292
	bnezc	r19,00400E32

l00400E20:
	lwpc	r6,00454528
	addiupc	r5,0000F7FA
	lwpc	r4,00412EF0
	balc	00400B5E

l00400E32:
	move.balc	r4,r17,0040AF72

l00400E36:
	addiupc	r17,0003064E
	lw	r7,0004(r17)
	bnezc	r7,00400E42

l00400E3E:
	lw	r7,0004(r18)
	sw	r7,0044(sp)

l00400E42:
	lwpc	r19,00454528
	beqzc	r19,00400E72

l00400E4A:
	li	r6,00000020
	move	r5,r0
	addiu	r4,sp,00000038
	balc	00400B4E
	li	r6,0000000F
	addiu	r4,sp,00000038
	move.balc	r5,r19,0040A930
	lw	r4,0000(r16)
	addiu	r6,sp,00000038
	addiu	r5,r0,00008933
	balc	00405B80
	bltc	r4,r0,00400D02

l00400E6A:
	li	r7,00000018
	swpc	r7,004544AC

l00400E72:
	lwpc	r7,0045452C
	beqzc	r7,00400EC4

l00400E7A:
	lwpc	r7,00454514
	bnezc	r7,00400EFC

l00400E82:
	lwpc	r7,00430048
	bgec	r7,r0,00400EE0

l00400E8C:
	li	r7,00000002
	swpc	r7,00430048
	bc	00400ED6

l00400E96:
	lw	r20,0003(r19)
	beqc	r0,r20,00400EC0

l00400E9C:
	lhu	r7,0000(r20)
	bneiuc	r7,00000002,00400EC0

l00400EA4:
	lw	r4,0004(r19)
	li	r6,00000003
	move.balc	r5,r22,0040A8E0
	bnezc	r4,00400EC0

l00400EAE:
	li	r6,00000004
	addiupc	r5,0002F19C
	addu	r4,r20,r6
	balc	0040A100
	beqc	r0,r4,00400E1A

l00400EC0:
	lw	r19,0000(r19)
	bc	00400E18

l00400EC4:
	lw	r4,0004(r17)
	balc	00407620
	lui	r7,FFFE0000
	ins	r4,r0,00000000,00000001
	beqc	r4,r7,00400E7A

l00400ED6:
	lwpc	r7,00430048
	bltc	r7,r0,00400F30

l00400EE0:
	lw	r4,0000(r16)
	addiupc	r7,0002F162
	addiu	r8,r0,00000004
	li	r6,0000000A
	move	r5,r0
	balc	00400B4A
	li	r7,FFFFFFFF
	bnec	r7,r4,00400F30

l00400EF6:
	addiupc	r4,0000F7D2
	bc	00400C92

l00400EFC:
	lwpc	r7,0043008C
	addiu	r6,r0,000003E7
	bltc	r6,r7,00400F16

l00400F0A:
	lwpc	r5,00412EF0
	addiupc	r4,0000F75C
	bc	00400E0A

l00400F16:
	lwpc	r7,00430048
	bltc	r7,r0,00400E82

l00400F20:
	beqic	r7,00000002,00400E82

l00400F24:
	lwpc	r5,00412EF0
	addiupc	r4,0000F772
	bc	00400E0A

l00400F30:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000F,00400F50

l00400F3A:
	lw	r4,0000(r16)
	li	r6,00000010
	addiupc	r5,0002F10A
	balc	00405DB0
	li	r7,FFFFFFFF
	bnec	r4,r7,00400F50

l00400F4A:
	addiupc	r4,0000F796
	bc	00400C92

l00400F50:
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00400F78

l00400F56:
	li	r7,FFFFE7C6
	lw	r4,0000(r16)
	sw	r7,0038(sp)
	addiu	r8,r0,00000004
	addiu	r7,sp,00000038
	li	r6,00000001
	addiu	r5,r0,000000FF
	balc	00401280
	li	r7,FFFFFFFF
	bnec	r4,r7,00400F78

l00400F72:
	addiupc	r4,0000F776
	balc	00401284

l00400F78:
	li	r7,00000001
	lw	r4,0000(r16)
	sw	r7,001C(sp)
	addiu	r8,r0,00000004
	li	r6,0000000B
	move	r5,r0
	addiu	r7,sp,0000001C
	balc	00401280
	beqzc	r4,00400F9A

l00400F8C:
	lwpc	r5,00412EF0
	addiupc	r4,0000F77A
	balc	004083F0

l00400F9A:
	lw	r7,0004(r16)
	bneiuc	r7,00000001,00400FCC

l00400FA0:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,sp,0000001C
	li	r6,0000000C
	move	r5,r0
	balc	00401280
	beqzc	r4,00400FB6

l00400FB0:
	addiupc	r4,0000F790
	balc	00401284

l00400FB6:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,sp,0000001C
	li	r6,00000007
	move	r5,r0
	balc	00401280
	beqzc	r4,00400FCC

l00400FC6:
	addiupc	r4,0000F79A
	balc	00401284

l00400FCC:
	lwpc	r7,004544EC
	bbeqzc	r7,00000005,00401012

l00400FD6:
	li	r19,00000028
	addiu	r4,sp,00000038
	movep	r5,r6,r0,r19
	balc	00401366
	li	r7,00000001
	sb	r7,0038(sp)
	li	r7,00000007
	sb	r7,0039(sp)
	li	r7,00000027
	sb	r7,003A(sp)
	li	r7,00000004
	lw	r4,0000(r16)
	move	r8,r19
	sb	r7,003B(sp)
	li	r6,00000004
	move	r5,r0
	addiu	r7,sp,00000038
	swpc	r19,00454534
	balc	00401280
	bgec	r4,r0,00401012

l0040100C:
	addiupc	r4,0000F774
	bc	00400C92

l00401012:
	lwpc	r7,004544EC
	bbeqzc	r7,00000009,004010A8

l0040101C:
	li	r6,00000028
	move	r5,r0
	addiu	r4,sp,00000038
	balc	00401366
	li	r7,00000044
	li	r5,00000028
	li	r6,00000024
	sb	r7,0038(sp)
	lwpc	r7,0045453C
	movz	r6,r5,r7

l00401038:
	sb	r7,003B(sp)
	sb	r6,0039(sp)
	li	r6,00000005
	sb	r6,003A(sp)
	beqic	r7,00000003,0040107A

l0040104A:
	lbu	r8,0039(sp)
	addiu	r7,sp,00000038
	lw	r4,0000(r16)
	li	r6,00000004
	move	r5,r0
	balc	00401280
	bgec	r4,r0,004010A0

l0040105C:
	li	r7,00000002
	lbu	r8,0039(sp)
	lw	r4,0000(r16)
	li	r6,00000004
	sb	r7,003B(sp)
	move	r5,r0
	addiu	r7,sp,00000038
	balc	00401280
	bgec	r4,r0,004010A0

l00401074:
	addiupc	r4,0000F720
	bc	00400C92

l0040107A:
	lwpc	r5,00454538
	addiu	r6,sp,00000038
	sll	r7,r5,00000003
	addiu	r7,r7,00000004
	sb	r7,0039(sp)
	move	r7,r0

l0040108C:
	addiu	r6,r6,00000008
	bgec	r7,r5,0040104A

l00401092:
	addiupc	r4,00030402
	lwxs	r4,r7(r4)
	addiu	r7,r7,00000001
	sw	r4,-0004(r6)
	bc	0040108C

l004010A0:
	li	r7,00000028
	swpc	r7,00454534

l004010A8:
	lwpc	r19,004544EC
	bbeqzc	r19,0000000A,00401114

l004010B2:
	addiu	r20,sp,00000038
	li	r6,00000028
	movep	r4,r5,r20,r0
	balc	00401366
	li	r7,00000001
	andi	r19,r19,00000080
	addiu	r6,r0,00000083
	sb	r7,0038(sp)
	addiu	r7,r0,00000089
	movz	r7,r6,r19

l004010D2:
	lwpc	r8,00454538
	move	r19,r7
	sll	r7,r8,00000002
	addiu	r7,r7,00000003
	move	r6,r0
	sb	r7,003A(sp)
	li	r7,00000004
	sb	r7,003B(sp)
	addiupc	r7,000303A8
	sb	r19,0039(sp)

l004010F4:
	bltc	r6,r8,00401162

l004010F8:
	addiu	r8,r8,00000001
	lw	r4,0000(r16)
	sll	r8,r8,00000002
	move	r7,r20
	li	r6,00000004
	move	r5,r0
	balc	00401280
	bltc	r4,r0,0040100C

l0040110C:
	li	r7,00000028
	swpc	r7,00454534

l00401114:
	lwpc	r7,00430074
	addiu	r6,r0,00000200
	addiu	r4,r7,00000207
	div	r5,r4,r6
	lwpc	r6,00454534
	addiu	r7,r7,00000008
	addiu	r6,r6,00000104
	mul	r5,r5,r6
	addu	r5,r5,r7
	sw	r5,001C(sp)
	move.balc	r4,r16,0040212E
	lwpc	r7,0045452C
	beqzc	r7,0040116E

l00401144:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiupc	r7,000533DE
	li	r6,00000020
	addiu	r5,r0,0000FFFF
	balc	00401280
	bgec	r4,r0,0040116E

l0040115A:
	addiupc	r4,0000F64A
	bc	00400C92

l00401162:
	lw	r5,0000(r7)
	addiu	r6,r6,00000001
	addiu	r7,r7,00000004
	swxs	r5,r6(r20)
	bc	004010F4

l0040116E:
	lwpc	r7,004544EC
	bbeqzc	r7,00000010,00401194

l00401178:
	lw	r4,0000(r16)
	addiu	r7,sp,00000028
	addiu	r8,r0,00000001
	li	r6,00000022
	move	r5,r0
	sw	r0,0028(sp)
	balc	00401280
	li	r7,FFFFFFFF
	bnec	r4,r7,00401194

l0040118C:
	addiupc	r4,0000F638
	bc	00400C92

l00401194:
	lwpc	r7,004544EC
	bbeqzc	r7,00000011,004011DA

l0040119E:
	lwpc	r7,00454510
	lw	r4,0000(r16)
	sw	r7,0028(sp)
	addiu	r8,r0,00000001
	li	r6,00000021
	move	r5,r0
	addiupc	r7,0005335C
	li	r19,FFFFFFFF
	balc	00401280
	bnec	r19,r4,004011C2

l004011BA:
	addiupc	r4,0000F632
	bc	00400C92

l004011C2:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,sp,00000028
	li	r6,00000002
	move	r5,r0
	balc	00401280
	bnec	r19,r4,004011DA

l004011D2:
	addiupc	r4,0000F642
	bc	00400C92

l004011DA:
	lwpc	r6,00454534
	addiu	r7,r0,0000FFE3
	subu	r7,r7,r6
	lwpc	r6,00430074
	bgec	r7,r6,004011F8

l004011F0:
	addiupc	r5,0000F64C
	bc	00400BC6

l004011F8:
	bltiuc	r6,00000008,00401204

l004011FC:
	li	r7,00000001
	swpc	r7,00454508

l00401204:
	addiu	r19,r6,00000088
	move.balc	r4,r19,00405292
	move	r20,r4
	bnezc	r4,0040121E

l00401210:
	lwpc	r5,00412EF0
	addiupc	r4,0000F65A
	bc	00400E0A

l0040121E:
	lw	r4,0004(r17)
	lwpc	r21,004544C0
	balc	00401590
	movep	r5,r6,r21,r4
	addiupc	r4,0000F65E
	balc	00401594
	lwpc	r7,00454528
	bnezc	r7,00401242

l00401238:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000F,0040125C

l00401242:
	lw	r4,0004(r18)
	balc	00401590
	lwpc	r6,00454528
	addiupc	r7,0000F228
	move	r5,r4
	movz	r6,r7,r6

l00401256:
	addiupc	r4,0000F642
	balc	00401594

l0040125C:
	lwpc	r5,00430074
	lwpc	r6,00454534
	addu	r6,r5,r6
	addiupc	r4,0000F63E
	addiu	r6,r6,0000001C
	balc	00401594
	move.balc	r4,r16,004021B4
	movep	r6,r7,r20,r19
	addiupc	r4,0002EDE0
	move.balc	r5,r16,00402A9E

;; fn00401280: 00401280
;;   Called from:
;;     00400F6C (in ping4_run)
;;     00400F88 (in ping4_run)
;;     00400FAC (in ping4_run)
;;     00400FC2 (in ping4_run)
;;     00401006 (in ping4_run)
;;     00401056 (in ping4_run)
;;     0040106E (in ping4_run)
;;     00401106 (in ping4_run)
;;     00401154 (in ping4_run)
;;     00401186 (in ping4_run)
;;     004011B6 (in ping4_run)
;;     004011CE (in ping4_run)
;;     0040127C (in ping4_run)
fn00401280 proc
	bc	00407D60

;; fn00401284: 00401284
;;   Called from:
;;     00400F76 (in ping4_run)
;;     00400FB4 (in ping4_run)
;;     00400FCA (in ping4_run)
fn00401284 proc
	bc	00408630

;; pr_addr: 00401288
;;   Called from:
;;     00401416 (in pr_options)
;;     004014B0 (in pr_options)
;;     004014F6 (in pr_options)
;;     00401906 (in ping4_receive_error_msg)
;;     00401976 (in ping4_parse_reply)
;;     004019BC (in ping4_parse_reply)
;;     00401ACE (in ping4_parse_reply)
;;     00401B2A (in ping4_parse_reply)
;;     0040304E (in ping6_receive_error_msg)
;;     004030E2 (in ping6_parse_reply)
;;     00403228 (in ping6_parse_reply)
;;     00403292 (in ping6_parse_reply)
;;     00403BFE (in ping6_run)
;;     00403C40 (in ping6_run)
pr_addr proc
	save	00000220,ra,00000002
	addiu	r16,r0,000000FB
	sw	r5,0008(sp)
	move	r6,r16
	sw	r4,000C(sp)
	move	r5,r0
	addiu	r4,sp,00000014
	sw	r0,0010(sp)
	balc	00401366
	movep	r5,r6,r0,r16
	addiu	r4,sp,00000114
	sw	r0,0110(sp)
	balc	00401366
	lw	r17,0008(sp)
	lwpc	r7,00454520
	bnec	r6,r7,004012C2

l004012B4:
	lw	r17,000C(sp)
	addiupc	r5,00030126
	balc	0040A100
	beqc	r0,r4,0040135E

l004012C2:
	lw	r17,0008(sp)
	addiupc	r4,00030118
	lw	r17,000C(sp)
	move	r6,r7
	swpc	r7,00454520
	balc	00401608
	addiupc	r4,00053068
	balc	00407E80
	addiu	r7,r0,000000FF
	sltiu	r4,r4,00000001
	lw	r17,0008(sp)
	swpc	r4,004314C0
	lw	r17,000C(sp)
	addiu	r10,r0,00000001
	move	r9,r0
	move	r8,r0
	addiu	r6,sp,00000110
	balc	00406302
	lwpc	r7,004544B8
	beqzc	r7,00401322

l00401306:
	lbu	r7,0010(sp)
	bnezc	r7,00401342

l0040130C:
	addiu	r7,sp,00000110
	addiupc	r6,0000F95C
	addiu	r5,r0,00001000
	addiupc	r4,0002F0C4
	balc	00408820
	bc	00401358

l00401322:
	lwpc	r7,004544EC
	bbnezc	r7,00000002,00401306

l0040132C:
	lw	r17,0008(sp)
	move	r10,r0
	lw	r17,000C(sp)
	move	r9,r0
	move	r8,r0
	addiu	r7,r0,000000FF
	addiu	r6,sp,00000010
	balc	00406302
	bc	00401306

l00401342:
	addiu	r8,sp,00000110
	addiu	r7,sp,00000010
	addiupc	r6,0000F91C
	addiu	r5,r0,00001000
	addiupc	r4,0002F08C
	balc	00408820

l00401358:
	swpc	r0,004314C0

l0040135E:
	addiupc	r4,0002F07E
	restore.jrc	00000220,ra,00000002

;; fn00401366: 00401366
;;   Called from:
;;     00400FDC (in ping4_run)
;;     00401022 (in ping4_run)
;;     004010BA (in ping4_run)
;;     0040129C (in pr_addr)
;;     004012A8 (in pr_addr)
fn00401366 proc
	bc	0040A690

;; pr_options: 0040136A
;;   Called from:
;;     00401606 (in pr_iph)
;;     00401B80 (in ping4_parse_reply)
pr_options proc
	save	00000050,r30,0000000A
	move	r16,r4
	addiu	r20,r5,FFFFFFEC

l00401372:
	bltc	r0,r20,00401378

l00401376:
	restore.jrc	00000050,r30,0000000A

l00401378:
	lbu	r5,0000(r16)
	beqzc	r5,00401376

l0040137C:
	bneiuc	r5,00000001,0040138C

l00401380:
	addiupc	r4,0000F8F0
	addiu	r20,r20,FFFFFFFF
	addiu	r16,r16,00000001
	balc	00401594
	bc	00401372

l0040138C:
	lbu	r19,0001(r16)
	bltic	r19,00000002,00401376

l00401392:
	bltc	r20,r19,00401376

l00401396:
	beqic	r5,00000004,004014BC

l0040139A:
	bgeiuc	r5,00000005,004013A8

l0040139E:
	beqic	r5,00000007,00401422

l004013A2:
	addiupc	r4,0000F972
	bc	0040158C

l004013A8:
	addiu	r6,r0,00000083
	addiu	r7,r0,00000089
	beqc	r5,r6,004013B6

l004013B2:
	bnec	r7,r5,004013A2

l004013B6:
	xori	r5,r5,00000089
	li	r6,0000004C
	li	r7,00000053
	addiupc	r4,0000F8BA
	movn	r7,r6,r5

l004013C6:
	move.balc	r5,r7,004086C0
	lbu	r21,0001(r16)
	bltic	r21,00000005,004013FA

l004013D2:
	addiu	r18,r16,00000003
	move	r17,r18

l004013D8:
	move	r5,r17
	li	r6,00000004
	addiu	r4,sp,0000000C
	addiu	r17,r17,00000004
	balc	00401608
	lw	r17,000C(sp)
	bnezc	r7,00401402

l004013E6:
	addiupc	r4,0000F89E
	balc	00401594

l004013EC:
	li	r4,0000000A
	balc	00401766
	subu	r7,r21,r17
	addu	r7,r18,r7
	bgeic	r7,00000005,004013D8

l004013FA:
	subu	r20,r20,r19
	addu	r16,r16,r19
	bc	00401372

l00401402:
	li	r5,00000010
	li	r6,00000002
	addu	r4,sp,r5
	sw	r0,0010(sp)
	sw	r7,0014(sp)
	sh	r6,0010(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)
	balc	00401288
	move	r5,r4
	addiupc	r4,0000F876
	balc	00401594
	bc	004013EC

l00401422:
	lbu	r7,0002(r16)
	slt	r17,r7,r19
	movz	r7,r19,r17

l0040142C:
	move	r17,r7
	addiu	r17,r17,FFFFFFFC
	move	r21,r17
	bgec	r0,r17,004013FA

l00401436:
	lwpc	r7,00454524
	addiu	r18,r16,00000002
	bnec	r7,r17,00401462

l00401444:
	move	r6,r17
	addiupc	r5,00030016
	move.balc	r4,r18,0040A100
	bnezc	r4,00401462

l00401450:
	lwpc	r7,004544EC
	bbnezc	r7,00000000,00401462

l0040145A:
	addiupc	r4,0000F83A
	balc	00401594
	bc	004013FA

l00401462:
	movep	r5,r6,r18,r17
	addiupc	r4,0002FFF8
	swpc	r17,00454524
	balc	00401608
	addiupc	r4,0000F834
	addiu	r18,r16,00000003
	balc	00401594

l0040147A:
	subu	r5,r21,r17
	li	r6,00000004
	addu	r5,r18,r5
	addiu	r4,sp,0000000C
	balc	00401608
	lw	r17,000C(sp)
	bnezc	r7,0040149C

l0040148A:
	addiupc	r4,0000F7FA
	balc	00401594

l00401490:
	addiu	r17,r17,FFFFFFFC
	li	r4,0000000A
	balc	00401766
	bltc	r0,r17,0040147A

l0040149A:
	bc	004013FA

l0040149C:
	li	r5,00000010
	li	r6,00000002
	addu	r4,sp,r5
	sw	r0,0010(sp)
	sw	r7,0014(sp)
	sh	r6,0010(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)
	balc	00401288
	move	r5,r4
	addiupc	r4,0000F7DC
	balc	00401594
	bc	00401490

l004014BC:
	lbu	r7,0002(r16)
	slt	r18,r7,r19
	movz	r7,r19,r18

l004014C6:
	move	r18,r7
	addiu	r18,r18,FFFFFFFB
	bgec	r0,r18,004013FA

l004014CE:
	addiupc	r4,0000F7DE
	lbu	r21,0003(r16)
	addiu	r30,r16,00000004
	move	r23,r0
	balc	00401594
	move	r22,r0
	bc	00401524

l004014E2:
	li	r5,00000010
	li	r6,00000002
	addu	r4,sp,r5
	sw	r0,0010(sp)
	sw	r7,0014(sp)
	sh	r6,0010(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)
	balc	00401288
	move	r5,r4
	addiupc	r4,0000F796
	balc	00401594
	bc	00401540

l00401502:
	subu	r5,r17,r23
	addiupc	r4,0000F7CA
	bc	00401570

l0040150C:
	bnec	r0,r22,00401576

l00401510:
	move	r5,r17
	addiupc	r4,0000F7D2

l00401516:
	balc	00401594
	move	r22,r17

l0040151A:
	addiu	r18,r18,FFFFFFFC
	li	r4,0000000A
	balc	00401766
	bgec	r0,r18,00401580

l00401524:
	andi	r7,r21,0000000F
	move	r17,r30
	beqzc	r7,00401546

l0040152C:
	li	r6,00000004
	move	r5,r30
	addiu	r4,sp,0000000C
	addiu	r17,r17,00000004
	balc	00401608
	lw	r17,000C(sp)
	bnezc	r7,004014E2

l0040153A:
	addiupc	r4,0000F74A
	balc	00401594

l00401540:
	addiu	r18,r18,FFFFFFFC
	bgec	r0,r18,00401580

l00401546:
	lbu	r6,0000(r17)
	addiu	r30,r17,00000004
	lbu	r7,0001(r17)
	sll	r6,r6,00000008
	addu	r7,r7,r6
	sll	r6,r7,00000008
	lbu	r7,0002(r17)
	lbu	r17,0003(r17)
	addu	r7,r7,r6
	sll	r7,r7,00000008
	addu	r17,r17,r7
	bgec	r17,r0,0040150C

l00401562:
	ext	r17,r17,00000000,0000001F
	bnec	r0,r23,00401502

l0040156A:
	move	r5,r17
	addiupc	r4,0000F748

l00401570:
	balc	00401594
	move	r23,r17
	bc	0040151A

l00401576:
	subu	r5,r17,r22
	addiupc	r4,0000F77A
	bc	00401516

l00401580:
	srl	r5,r21,00000004
	addiupc	r4,0000F778
	beqc	r0,r5,004013FA

l0040158C:
	balc	00401594
	bc	004013FA

;; fn00401590: 00401590
;;   Called from:
;;     00401226 (in ping4_run)
;;     00401244 (in ping4_run)
;;     004015E8 (in pr_iph)
;;     004015F2 (in pr_iph)
fn00401590 proc
	bc	00406830

;; fn00401594: 00401594
;;   Called from:
;;     0040122E (in ping4_run)
;;     0040125A (in ping4_run)
;;     00401270 (in ping4_run)
;;     00401388 (in pr_options)
;;     004013EA (in pr_options)
;;     0040141E (in pr_options)
;;     0040145E (in pr_options)
;;     00401478 (in pr_options)
;;     0040148E (in pr_options)
;;     004014B8 (in pr_options)
;;     004014DC (in pr_options)
;;     004014FE (in pr_options)
;;     00401516 (in pr_options)
;;     0040153E (in pr_options)
;;     00401570 (in pr_options)
;;     0040158C (in pr_options)
;;     004015C2 (in pr_iph)
;;     004015D2 (in pr_iph)
;;     004015E4 (in pr_iph)
;;     0040175E (in pr_icmph)
;;     00401918 (in ping4_receive_error_msg)
fn00401594 proc
	bc	004086C0

;; pr_iph: 00401598
pr_iph proc
	save	00000010,ra,00000004
	move	r16,r4
	addiupc	r4,0000F78C
	addiupc	r18,0000F808
	lw	r17,0000(r16)
	balc	00401762
	lw	r5,0000(r16)
	addiupc	r4,0000F7C2
	lhu	r9,0004(r16)
	andi	r17,r17,0000000F
	lhu	r8,0002(r16)
	andi	r6,r5,0000000F
	lbu	r7,0001(r16)
	ext	r5,r5,00000004,00000004
	sll	r17,r17,00000002
	balc	00401594
	lhu	r5,0006(r16)
	addiupc	r4,0000F7C2
	ext	r6,r5,00000000,0000000D
	srl	r5,r5,0000000D
	balc	00401594
	lhu	r7,000A(r16)
	lbu	r6,0009(r16)
	addiupc	r4,0000F7B8
	lbu	r5,0008(r16)
	balc	00401594
	lw	r4,000C(r16)
	balc	00401590
	move	r5,r4
	move.balc	r4,r18,004086C0
	lw	r4,0010(r16)
	balc	00401590
	move	r5,r4
	move.balc	r4,r18,004086C0
	li	r4,0000000A
	balc	00401766
	move	r5,r17
	addiu	r4,r16,00000014
	restore	00000010,ra,00000004
	bc	0040136A

;; fn00401608: 00401608
;;   Called from:
;;     004012D2 (in pr_addr)
;;     004013E0 (in pr_options)
;;     0040146E (in pr_options)
;;     00401484 (in pr_options)
;;     00401534 (in pr_options)
fn00401608 proc
	bc	0040A130

;; pr_icmph: 0040160C
;;   Called from:
;;     00401926 (in ping4_receive_error_msg)
;;     00401AF4 (in ping4_parse_reply)
;;     00401B4C (in ping4_parse_reply)
pr_icmph proc
	save	00000020,ra,00000003
	movep	r17,r16,r6,r7
	bgeiuc	r4,00000013,00401758

l00401614:
	addiupc	r7,00010B58
	lwxs	r7,r4(r7)
	jrc	r7
0040161C                                     80 04 94 F7             ....
00401620 40 39 23 1F AC C8 80 80 E1 04 90 0B DF 53 E0 D8 @9#..........S..
00401630 80 04 8C F7 2C 39 1A 18 80 04 A0 F7 F7 1B 80 04 ....,9..........
00401640 BA F7 F1 1B 80 04 D8 F7 EB 1B B1 10 80 04 F0 F7 ................
00401650 43 3B 00 8A 0A 01 EB 60 90 2E 05 00 E4 C8 00 41 C;.....`.......A
00401660 02 92 35 3B 23 1F 80 04 FA F7 C9 1B 80 04 08 F8 ..5;#...........
00401670 C3 1B 80 04 1A F8 BD 1B 80 04 30 F8 B7 1B 80 04 ..........0.....
00401680 42 F8 B1 1B 80 04 58 F8 AB 1B 80 04 6E F8 A5 1B B.....X.....n...
00401690 80 04 98 F8 9F 1B 80 04 C6 F8 99 1B 80 04 D0 F8 ................
004016A0 93 1B 80 04 E2 F8 8D 1B 80 04 F0 F8 A3 1B 80 04 ................
004016B0 0A F9 81 1B A0 C8 3A 08 90 9A A0 C8 3A 10 A0 C8 ......:.....:...
004016C0 3C 18 80 04 76 F9 CD 3A 06 18 80 04 FE F8 C5 3A <...v..:.......:
004016D0 82 D3 00 B4 02 B4 FD 84 00 50 03 B4 02 98 81 14 .........P......
004016E0 90 D2 9D 10 21 B6 FF 2B 9F FB A4 10 80 04 64 F9 ....!..+......d.
004016F0 5F 1B 80 04 EA F8 D7 1B 80 04 F4 F8 D1 1B 80 04 _...............
00401700 16 F9 CB 1B 80 04 60 F9 17 1B 8A 9A A0 C8 0C 08 ......`.........
00401710 80 04 9C F9 3B 1B 80 04 5E F9 19 1B 80 04 70 F9 ....;...^.....p.
00401720 13 1B 08 98 01 16 14 3A 24 82 58 C0 B1 10 80 04 .......:$.X.....
00401730 9E F9 1D 1B 80 04 BC F9 E7 1A 80 04 C2 F9 E1 1A ................
00401740 80 04 CC F9 DB 1A 80 04 DA F9 D5 1A 80 04 E8 F9 ................
00401750 CF 1A 80 04 FA F9 C9 1A                         ........        

l00401758:
	move	r5,r4
	addiupc	r4,0000FA06
	balc	00401594
	restore.jrc	00000020,ra,00000003

;; fn00401762: 00401762
;;   Called from:
;;     004015A6 (in pr_iph)
;;     00401B3C (in ping4_parse_reply)
fn00401762 proc
	bc	00408780

;; fn00401766: 00401766
;;   Called from:
;;     004013EE (in pr_options)
;;     00401494 (in pr_options)
;;     0040151E (in pr_options)
;;     004015FC (in pr_iph)
;;     00401B5E (in ping4_parse_reply)
fn00401766 proc
	bc	00408770

;; ping4_receive_error_msg: 0040176A
ping4_receive_error_msg proc
	save	00000270,ra,00000005
	move	r17,r4
	balc	004049B0
	addiu	r7,sp,FFFFF250
	move	r16,r7
	addiu	r7,sp,0000001C
	lw	r19,0000(r4)
	addiu	r6,r0,00002040
	sw	r7,0DC4(r16)
	li	r7,00000008
	sw	r7,0DC8(r16)
	addiu	r7,sp,00000024
	sw	r7,0DE4(r16)
	li	r7,00000010
	sw	r7,0DE8(r16)
	addiu	r7,sp,00000014
	sw	r7,0DEC(r16)
	li	r7,00000001
	sw	r7,0DF0(r16)
	addiu	r7,sp,00000050
	lw	r4,0000(r17)
	addiu	r5,sp,00000034
	sw	r7,0DF4(r16)
	addiu	r7,r0,00000200
	sw	r0,0DFC(r16)
	sw	r7,0DF8(r16)
	balc	00407670
	bltc	r4,r0,0040187A

l004017C2:
	lw	r6,0DF8(r16)
	move	r7,r0
	bltiuc	r6,0000000C,004017D0

l004017CC:
	lw	r7,0DF4(r16)

l004017D0:
	lw	r17,0044(sp)
	move	r18,r0
	addu	r5,r5,r6

l004017D6:
	beqzc	r7,00401800

l004017D8:
	lw	r6,0004(r7)
	bnezc	r6,004017E4

l004017DC:
	lw	r6,0008(r7)
	bneiuc	r6,0000000B,004017E4

l004017E2:
	addiu	r18,r7,0000000C

l004017E4:
	lw	r6,0000(r7)
	bltiuc	r6,0000000C,004017FC

l004017EA:
	addiu	r6,r6,00000003
	subu	r16,r5,r7
	ins	r6,r0,00000000,00000001
	addiu	r8,r6,0000000C
	addu	r7,r7,r6
	bltuc	r8,r16,004017D6

l004017FC:
	move	r7,r0
	bc	004017D6

l00401800:
	bnezc	r18,00401806

l00401802:
	balc	00404A00

l00401806:
	lbu	r5,0004(r18)
	bneiuc	r5,00000001,00401870

l0040180E:
	lwpc	r7,004544EC
	andi	r16,r7,00000010
	bnec	r0,r16,00401936

l0040181C:
	bbeqzc	r7,00000000,00401848

l00401820:
	addiupc	r4,0000F954
	balc	00401BA0

l00401826:
	lwpc	r7,004544D0
	li	r17,00000001
	addiu	r7,r7,00000001
	swpc	r7,004544D0

l00401836:
	balc	004049B0
	sw	r19,0000(sp)
	bnezc	r16,00401842

l0040183E:
	subu	r16,r0,r17

l00401842:
	move	r4,r16
	restore.jrc	00000270,ra,00000005

l00401848:
	lw	r4,0000(r18)
	lwpc	r7,00412EF0
	sw	r7,000C(sp)
	beqic	r4,0000001A,00401868

l00401856:
	balc	004049EA
	addiupc	r5,0000F91E
	move	r6,r4

l00401860:
	lw	r17,000C(sp)
	balc	00408340
	bc	00401826

l00401868:
	addiupc	r5,0000F928
	lw	r6,0008(r18)
	bc	00401860

l00401870:
	bneiuc	r5,00000002,0040187A

l00401874:
	bgeic	r4,00000008,0040187E

l00401878:
	move	r19,r0

l0040187A:
	move	r17,r0
	bc	00401938

l0040187E:
	addiupc	r7,0002FC06
	lw	r17,0028(sp)
	lw	r7,0004(r7)
	bnec	r7,r6,00401878

l0040188A:
	lbu	r7,001C(sp)
	bneiuc	r7,00000008,00401878

l00401892:
	lhu	r5,0020(sp)
	move.balc	r4,r17,00402CD6
	beqzc	r4,00401878

l0040189C:
	lhu	r4,0022(sp)
	balc	00401B9C
	balc	004000F0
	lw	r7,0004(r17)
	bneiuc	r7,00000003,004018D6

l004018AC:
	lw	r7,0008(r17)
	bnezc	r7,004018D6

l004018B0:
	li	r6,00000001
	addiu	r7,r0,FFFFFFCE
	lw	r4,0000(r17)
	addiu	r8,r0,00000004
	sw	r7,0010(sp)
	addiu	r5,r0,000000FF
	addiu	r7,sp,00000010
	sw	r6,0048(sp)
	balc	00407D60
	li	r7,FFFFFFFF
	bnec	r4,r7,004018D6

l004018CE:
	addiupc	r4,0000F8F2
	balc	00408630

l004018D6:
	lwpc	r7,004544D0
	lwpc	r16,004544EC
	addiu	r7,r7,00000001
	andi	r17,r16,00000010
	swpc	r7,004544D0
	bnezc	r17,00401930

l004018F0:
	andi	r16,r16,00000001
	beqzc	r16,004018FE

l004018F4:
	li	r5,00000002
	addiupc	r4,0000F8EE
	balc	00401BA0
	bc	00401836

l004018FE:
	balc	00401E5C
	li	r5,00000010
	addu	r4,r18,r5
	balc	00401288
	move	r17,r4
	lhu	r4,0022(sp)
	balc	00401B9C
	movep	r5,r6,r17,r4
	addiupc	r4,0000F8D4
	balc	00401594
	lbu	r4,0005(r18)
	lbu	r5,0006(r18)
	move	r7,r0
	lw	r6,0008(r18)
	balc	0040160C
	lwpc	r4,00412EF4
	balc	00401B98

l00401930:
	move	r17,r0
	li	r16,00000001
	bc	00401836

l00401936:
	move	r17,r5

l00401938:
	move	r16,r0
	bc	00401836

;; fn0040193C: 0040193C
;;   Called from:
;;     00401AEE (in ping4_parse_reply)
;;     00401B46 (in ping4_parse_reply)
fn0040193C proc
	bc	00407620

;; ping4_parse_reply: 00401940
ping4_parse_reply proc
	save	00000050,ra,00000009
	movep	r20,r18,r6,r7
	lw	r7,0008(r5)
	move	r9,r4
	move	r17,r8
	lw	r16,0000(r7)
	lw	r7,0004(r4)
	bneiuc	r7,00000003,004019E8

l00401952:
	lw	r5,0000(r16)
	andi	r5,r5,0000000F
	sll	r7,r5,00000002
	addiu	r6,r7,00000007
	bgec	r6,r20,00401964

l00401960:
	bgeic	r5,00000005,0040198C

l00401964:
	lwpc	r7,004544EC
	bbnezc	r7,00000008,00401974

l0040196E:
	li	r17,00000001

l00401970:
	move	r4,r17
	restore.jrc	00000050,ra,00000009

l00401974:
	li	r5,00000010
	move.balc	r4,r18,00401288
	addiupc	r5,0000F886
	movep	r6,r7,r20,r4
	lwpc	r4,00412EF0
	balc	00408340
	bc	0040196E

l0040198C:
	lbu	r23,0008(r16)
	addiu	r21,r16,00000014
	addiu	r22,r7,FFFFFFEC

l00401998:
	subu	r20,r20,r7
	addu	r16,r16,r7
	movep	r5,r6,r20,r0
	move.balc	r4,r16,00400920
	lbu	r7,0000(r16)
	move	r19,r4
	bnec	r0,r7,00401A40

l004019AC:
	lhu	r5,0004(r16)
	move.balc	r4,r9,00402CD6
	beqzc	r4,0040196E

l004019B4:
	lhu	r4,0006(r16)
	balc	00401B9C
	li	r5,00000010
	move	r19,r4
	move.balc	r4,r18,00401288
	addiupc	r7,FFFFF02A
	move	r11,r4
	move	r10,r17
	sw	r7,0000(sp)
	move	r9,r0
	move	r8,r23
	movep	r6,r7,r20,r19
	li	r5,00000008
	move.balc	r4,r16,004023F6
	move	r17,r4
	beqc	r0,r4,00401B52

l004019DC:
	lwpc	r4,00412EF4
	balc	00401B98

l004019E4:
	move	r17,r0
	bc	00401970

l004019E8:
	lw	r19,0014(r5)
	bltiuc	r19,0000000C,00401A36

l004019EE:
	lw	r5,0010(r5)
	move	r21,r16
	move	r22,r0
	move	r23,r0
	move	r7,r5

l004019F8:
	beqzc	r7,00401998

l004019FA:
	lw	r6,0004(r7)
	lw	r4,0000(r7)
	bnezc	r6,00401A0E

l00401A00:
	lw	r6,0008(r7)
	bneiuc	r6,00000002,00401A2A

l00401A06:
	bltiuc	r4,00000004,00401A26

l00401A0A:
	lbu	r23,000C(r7)

l00401A0E:
	bltiuc	r4,0000000C,00401A26

l00401A12:
	addiu	r4,r4,00000003
	addu	r6,r5,r19
	ins	r4,r0,00000000,00000001
	subu	r6,r6,r7
	addiu	r8,r4,0000000C
	addu	r7,r7,r4
	bltuc	r8,r6,004019F8

l00401A26:
	move	r7,r0
	bc	004019F8

l00401A2A:
	bneiuc	r6,00000007,00401A0E

l00401A2E:
	addiu	r21,r7,0000000C
	move	r22,r4
	bc	00401A0E

l00401A36:
	move	r21,r16
	move	r22,r0
	move	r23,r0
	move	r7,r0
	bc	00401998

l00401A40:
	addiu	r7,r7,FFFFFFFD
	move	r17,r4
	andi	r7,r7,000000FF
	bgeiuc	r7,0000000A,00401A5A

l00401A4A:
	li	r6,00000001
	sllv	r7,r6,r7
	andi	r6,r7,00000307
	bnezc	r6,00401A76

l00401A56:
	bbnezc	r7,00000005,0040196E

l00401A5A:
	lwpc	r7,004544EC
	andi	r6,r7,00000111
	bneiuc	r6,00000001,00401B04

l00401A68:
	bnec	r0,r19,00401AFA

l00401A6C:
	li	r5,00000002
	addiupc	r4,0000F7E6
	balc	00401BA0
	bc	00401970

l00401A76:
	bltiuc	r20,00000024,0040196E

l00401A7A:
	lw	r17,0008(r16)
	andi	r17,r17,0000000F
	addiu	r17,r17,00000004
	sll	r17,r17,00000002
	bltc	r20,r17,0040196E

l00401A86:
	addiu	r17,r17,FFFFFFF8
	addu	r17,r16,r17
	lbu	r7,0000(r17)
	bneiuc	r7,00000008,0040196E

l00401A90:
	addiupc	r7,0002F9F4
	lw	r6,0018(r16)
	lw	r7,0004(r7)
	bnec	r7,r6,0040196E

l00401A9C:
	lhu	r5,0004(r17)
	move.balc	r4,r9,00402CD6
	beqc	r0,r4,0040196E

l00401AA6:
	lbu	r7,0000(r16)
	addiu	r7,r7,FFFFFFFC
	andi	r7,r7,000000FF
	bltiuc	r7,00000002,00401ABA

l00401AB0:
	lhu	r4,0006(r17)
	balc	00401B9C
	balc	004000F0
	bc	004019E4

l00401ABA:
	lwpc	r7,004544EC
	andi	r7,r7,00000011
	bnec	r0,r7,0040196E

l00401AC8:
	balc	00401E5C
	li	r5,00000010
	move.balc	r4,r18,00401288
	move	r18,r4
	lhu	r4,0006(r17)
	balc	00401B9C
	movep	r5,r6,r18,r4
	addiupc	r4,0000F752
	balc	00401B94
	beqzc	r19,00401AE8

l00401AE2:
	addiupc	r4,0000F762
	balc	00401B94

l00401AE8:
	lw	r4,0004(r16)
	lbu	r17,0000(r16)
	lbu	r18,0001(r16)
	balc	0040193C
	movep	r6,r7,r4,r16
	movep	r4,r5,r17,r18
	balc	0040160C
	bc	0040196E

l00401AFA:
	li	r5,00000003
	addiupc	r4,0000F75C
	balc	00401BA0
	bc	004019E4

l00401B04:
	bbeqzc	r7,00000008,004019E4

l00401B08:
	lwpc	r6,00454514
	bnec	r0,r6,004019E4

l00401B12:
	bbeqzc	r7,00000013,00401B28

l00401B16:
	move	r5,r0
	addiu	r4,sp,00000018
	balc	0040AF40
	addiupc	r4,0000F73E
	lwm	r5,0018(sp),00000002
	balc	00401B94

l00401B28:
	li	r5,00000010
	move.balc	r4,r18,00401288
	move	r5,r4
	addiupc	r4,0000F738
	balc	00401B94
	beqzc	r19,00401B40

l00401B38:
	addiupc	r4,0000F70C
	balc	00401762
	bc	004019E4

l00401B40:
	lw	r4,0004(r16)
	lbu	r18,0000(r16)
	lbu	r19,0001(r16)
	balc	0040193C
	movep	r6,r7,r4,r16
	movep	r4,r5,r18,r19
	balc	0040160C
	bc	00401970

l00401B52:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000D,00401B72

l00401B5C:
	li	r4,00000007
	balc	00401766
	lwpc	r7,004544EC
	bbeqzc	r7,00000000,00401B7C

l00401B6A:
	lwpc	r4,00412EF4
	balc	00401B98

l00401B72:
	lwpc	r7,004544EC
	bbnezc	r7,00000000,004019E4

l00401B7C:
	addiu	r5,r22,00000014
	move.balc	r4,r21,0040136A
	li	r4,0000000A
	balc	00408770
	lwpc	r4,00412EF4
	balc	00401B98
	bc	00401970

;; fn00401B94: 00401B94
;;   Called from:
;;     00401ADE (in ping4_parse_reply)
;;     00401AE6 (in ping4_parse_reply)
;;     00401B26 (in ping4_parse_reply)
;;     00401B34 (in ping4_parse_reply)
fn00401B94 proc
	bc	004086C0

;; fn00401B98: 00401B98
;;   Called from:
;;     0040192E (in ping4_receive_error_msg)
;;     004019E2 (in ping4_parse_reply)
;;     00401B70 (in ping4_parse_reply)
;;     00401B90 (in ping4_parse_reply)
fn00401B98 proc
	bc	004081A0

;; fn00401B9C: 00401B9C
;;   Called from:
;;     004018A0 (in ping4_receive_error_msg)
;;     00401910 (in ping4_receive_error_msg)
;;     004019B6 (in ping4_parse_reply)
;;     00401AB2 (in ping4_parse_reply)
;;     00401AD6 (in ping4_parse_reply)
fn00401B9C proc
	bc	00407630

;; fn00401BA0: 00401BA0
;;   Called from:
;;     00401824 (in ping4_receive_error_msg)
;;     004018FA (in ping4_receive_error_msg)
;;     00401A72 (in ping4_parse_reply)
;;     00401B00 (in ping4_parse_reply)
fn00401BA0 proc
	bc	00400A04
00401BA4             00 80 00 C0 00 80 00 C0 00 80 00 C0     ............

;; in_flight: 00401BB0
;;   Called from:
;;     00401FC4 (in pinger)
;;     00402034 (in pinger)
;;     00402BA4 (in main_loop)
;;     00402C74 (in main_loop)
in_flight proc
	aluipc	r6,00000401
	lwpc	r7,004544E4
	lhu	r6,0518(r6)
	subu	r6,r7,r6
	andi	r4,r6,0000FFFF
	bbeqzc	r6,0000000F,00401BD6

l00401BC6:
	lwpc	r4,004544DC
	subu	r7,r7,r4
	lwpc	r4,004544D0
	subu	r4,r7,r4

l00401BD6:
	jrc	ra

;; advance_ntransmitted: 00401BD8
;;   Called from:
;;     00402008 (in pinger)
;;     004020C6 (in pinger)
advance_ntransmitted proc
	lwpc	r6,004544E4
	aluipc	r5,00000401
	addiu	r7,r6,00000001
	lhu	r4,0518(r5)
	swpc	r7,004544E4
	andi	r7,r7,0000FFFF
	subu	r7,r7,r4
	addiu	r4,r0,00007FFF
	bgec	r4,r7,00401C02

l00401BFC:
	addiu	r6,r6,00000002
	sh	r6,0518(r5)

l00401C02:
	jrc	ra

;; sigstatus: 00401C04
sigstatus proc
	li	r7,00000001
	swpc	r7,00454500
	jrc	ra

;; update_interval: 00401C0E
;;   Called from:
;;     0040208A (in pinger)
;;     00402576 (in gather_statistics)
update_interval proc
	lwpc	r6,004544F4
	beqzc	r6,00401C52

l00401C16:
	li	r5,00000008
	div	r7,r6,r5

l00401C1C:
	lwpc	r6,004544F8
	addiu	r5,r0,000003E8
	addu	r7,r7,r6
	addiu	r7,r7,000001F4
	div	r6,r7,r5
	lwpc	r7,00454514
	swpc	r6,0043008C
	beqzc	r7,00401C50

l00401C3E:
	addiu	r7,r0,000000C7
	bltc	r7,r6,00401C50

l00401C46:
	addiu	r7,r0,000000C8
	swpc	r7,0043008C

l00401C50:
	jrc	ra

l00401C52:
	addiu	r6,r0,000003E8
	lwpc	r7,0043008C
	mul	r7,r7,r6
	bc	00401C1C

;; write_stdout: 00401C60
;;   Called from:
;;     00402042 (in pinger)
;;     004020E0 (in pinger)
;;     004025D2 (in gather_statistics)
write_stdout proc
	save	00000010,ra,00000004
	move	r16,r0
	movep	r18,r17,r4,r5

l00401C66:
	subu	r6,r17,r16
	addu	r5,r18,r16
	li	r4,00000001
	balc	0040B080
	addu	r16,r16,r4
	bltuc	r16,r17,00401C66

l00401C76:
	bltc	r4,r0,00401C66

l00401C7A:
	restore.jrc	00000010,ra,00000004

;; set_signal: 00401C7C
;;   Called from:
;;     0040271C (in fn0040271C)
set_signal proc
	save	000000A0,ra,00000003
	addiu	r6,r0,0000008C
	movep	r16,r17,r4,r5
	move	r5,r0
	addiu	r4,sp,00000004
	balc	0040A690
	move	r6,r0
	addiu	r5,sp,00000004
	sw	r17,0004(sp)
	move.balc	r4,r16,0040800A
	restore.jrc	000000A0,ra,00000003

;; sigexit: 00401C98
sigexit proc
	save	00000010,ra,00000001
	li	r7,00000001
	swpc	r7,004544B8
	lwpc	r7,004314C0
	bnezc	r7,00401CAC

l00401CAA:
	restore.jrc	00000010,ra,00000001

l00401CAC:
	move	r5,r0
	addiupc	r4,0005268E
	balc	00407E60

;; limit_capabilities: 00401CB6
;;   Called from:
;;     0040019A (in main)
;;     00401CB2 (in sigexit)
limit_capabilities proc
	save	00000010,ra,00000001
	balc	00401E82
	swpc	r4,00454514
	balc	0040AFA0
	swpc	r4,004544B4
	lwpc	r4,00454514
	balc	0040AFF0
	bnezc	r4,00401CD8

l00401CD6:
	restore.jrc	00000010,ra,00000001

l00401CD8:
	addiupc	r4,0000F59C
	balc	00401E7A
	li	r4,FFFFFFFF
	balc	00401E7E

;; modify_capability: 00401CE2
;;   Called from:
;;     0040073A (in main)
;;     00400770 (in main)
;;     00400CB6 (in ping4_run)
;;     00400CD8 (in ping4_run)
;;     00401CE0 (in limit_capabilities)
;;     00402282 (in setup)
;;     0040229C (in setup)
;;     00403752 (in ping6_run)
;;     00403798 (in ping6_run)
modify_capability proc
	save	00000010,ra,00000001
	lwpc	r7,004544B4
	bnezc	r4,00401CF0

l00401CEC:
	balc	00401E82
	move	r7,r4

l00401CF0:
	move.balc	r4,r7,0040AFF0
	bnezc	r4,00401CF8

l00401CF6:
	restore.jrc	00000010,ra,00000001

l00401CF8:
	addiupc	r4,0000F58C
	balc	00401E7A
	li	r4,FFFFFFFF
	bc	00401CF6

;; drop_capabilities: 00401D02
;;   Called from:
;;     00403C70 (in ping6_run)
drop_capabilities proc
	save	00000010,ra,00000001
	balc	00401E82
	balc	0040B000
	bnezc	r4,00401D0E

l00401D0C:
	restore.jrc	00000010,ra,00000001

l00401D0E:
	addiupc	r4,0000F566
	balc	00401E7A
	li	r4,FFFFFFFF
	balc	00401E7E

;; fill: 00401D18
;;   Called from:
;;     00401D16 (in drop_capabilities)
fill proc
	save	00000090,ra,00000006
	movep	r20,r18,r4,r5
	move	r19,r6
	move	r16,r20

l00401D20:
	lbu	r4,0000(r16)
	bnezc	r4,00401D92

l00401D24:
	addiu	r7,sp,0000006C
	addiu	r17,sp,00000030
	sw	r7,0024(sp)
	addiu	r7,sp,00000068
	sw	r7,0020(sp)
	addiu	r7,sp,00000064
	sw	r7,001C(sp)
	addiu	r7,sp,00000060
	sw	r7,0018(sp)
	addiu	r7,sp,0000005C
	sw	r7,0014(sp)
	addiu	r7,sp,00000058
	sw	r7,0010(sp)
	addiu	r7,sp,00000054
	sw	r7,000C(sp)
	addiu	r7,sp,00000050
	sw	r7,0008(sp)
	addiu	r7,sp,0000004C
	sw	r7,0004(sp)
	addiu	r7,sp,00000048
	move	r6,r17
	sw	r7,0000(sp)
	addiu	r11,sp,00000044
	addiu	r10,sp,00000040
	addiu	r9,sp,0000003C
	addiu	r8,sp,00000038
	addiupc	r5,0000F560
	addiu	r7,sp,00000034
	move.balc	r4,r20,004088A0
	move	r6,r0
	move	r16,r4
	bltc	r0,r4,00401DC0

l00401D72:
	lwpc	r7,004544EC
	bbnezc	r7,00000004,00401D90

l00401D7C:
	addiupc	r4,0000F578
	move	r17,r18
	balc	0040212A

l00401D84:
	subu	r7,r17,r18
	bltc	r7,r16,00401DCC

l00401D8A:
	li	r4,0000000A
	balc	00408770

l00401D90:
	restore.jrc	00000090,ra,00000006

l00401D92:
	balc	00404850
	bnezc	r4,00401DA8

l00401D98:
	lwpc	r5,00412EF0
	addiupc	r4,0000F4EE
	balc	00402126
	li	r4,00000002
	balc	00401E7E

l00401DA8:
	addiu	r16,r16,00000001
	bc	00401D20

l00401DAC:
	move	r7,r0

l00401DAE:
	addu	r5,r7,r6
	lwxs	r4,r7(r17)
	addu	r5,r18,r5
	addiu	r7,r7,00000001
	sb	r4,0008(r5)
	bnec	r7,r16,00401DAE

l00401DBE:
	addu	r6,r6,r16

l00401DC0:
	addiu	r7,r19,FFFFFFF8
	subu	r7,r7,r16
	bgeuc	r7,r6,00401DAC

l00401DCA:
	bc	00401D72

l00401DCC:
	lbu	r5,0008(r17)
	addiupc	r4,0000F530
	addiu	r17,r17,00000001
	balc	0040212A
	bc	00401D84

;; __schedule_exit: 00401DDA
;;   Called from:
;;     00402B0E (in main_loop)
__schedule_exit proc
	save	00000020,ra,00000002
	lwpc	r7,00454548
	move	r16,r4
	bnezc	r7,00401E4A

l00401DE6:
	lwpc	r7,004544DC
	beqzc	r7,00401E4E

l00401DEE:
	addiu	r5,r0,000003E8
	lwpc	r7,0043008C
	mul	r7,r7,r5
	lwpc	r6,0045450C
	sll	r6,r6,00000001
	swpc	r6,00454548
	bgeuc	r6,r7,00401E12

l00401E0C:
	swpc	r7,00454548

l00401E12:
	lwpc	r7,00454548
	addiu	r5,r0,000003E8
	divu	r6,r7,r5
	bltc	r16,r0,00401E28

l00401E24:
	bgeuc	r16,r6,00401E2A

l00401E28:
	move	r16,r6

l00401E2A:
	li	r6,000F4240
	move	r4,r0
	divu	r5,r7,r6
	sw	r5,0008(sp)
	modu	r5,r7,r6
	move	r6,r0
	sw	r5,000C(sp)
	move	r5,sp
	sw	r0,0000(sp)
	sw	r0,0004(sp)
	balc	00407F10

l00401E4A:
	move	r4,r16
	restore.jrc	00000020,ra,00000002

l00401E4E:
	addiu	r6,r0,000003E8
	lwpc	r7,00430084
	mul	r7,r7,r6
	bc	00401E0C

;; print_timestamp: 00401E5C
;;   Called from:
;;     004018FE (in ping4_receive_error_msg)
;;     00401AC8 (in ping4_parse_reply)
;;     00401F08 (in pinger)
;;     0040260C (in gather_statistics)
;;     004033EA (in if_name2index)
;;     004033EA (in fn004033EA)
print_timestamp proc
	save	00000020,ra,00000001
	lwpc	r7,004544EC
	bbeqzc	r7,00000013,00401E78

l00401E68:
	move	r5,r0
	addiu	r4,sp,00000008
	balc	004021B0
	addiupc	r4,0000F49A
	lwm	r5,0008(sp),00000002
	balc	0040212A

l00401E78:
	restore.jrc	00000020,ra,00000001

;; fn00401E7A: 00401E7A
;;   Called from:
;;     00401CDC (in limit_capabilities)
;;     00401CFC (in modify_capability)
;;     00401D12 (in drop_capabilities)
;;     00402122 (in pinger)
fn00401E7A proc
	bc	00408630

;; fn00401E7E: 00401E7E
;;   Called from:
;;     00401CE0 (in limit_capabilities)
;;     00401D16 (in drop_capabilities)
;;     00401DA6 (in fill)
;;     004021F2 (in setup)
fn00401E7E proc
	bc	0040015A

;; fn00401E82: 00401E82
;;   Called from:
;;     00401CB8 (in limit_capabilities)
;;     00401CEC (in modify_capability)
;;     00401D04 (in drop_capabilities)
fn00401E82 proc
	bc	0040AFC0

;; pinger: 00401E86
;;   Called from:
;;     00402AEE (in main_loop)
pinger proc
	save	00000030,ra,00000005
	lwpc	r7,004544B8
	movep	r16,r17,r4,r5
	addiu	r4,r0,000003E8
	bnec	r0,r7,004020FC

l00401E98:
	lwpc	r7,004544D8
	beqzc	r7,00401EB4

l00401EA0:
	lwpc	r6,004544E4
	bltc	r6,r7,00401EB4

l00401EAA:
	lwpc	r7,004314C8
	beqc	r0,r7,004020FC

l00401EB4:
	aluipc	r19,00000401
	move	r5,r0
	lw	r7,0490(r19)
	move	r18,r19
	bnec	r0,r7,00401F8C

l00401EC4:
	addiupc	r4,000525C8
	balc	004021B0
	lwpc	r7,00430088
	addiu	r7,r7,FFFFFFFF
	lwpc	r6,0043008C
	mul	r7,r7,r6
	swpc	r7,00454544

l00401EE0:
	lwpc	r7,004544EC
	bbeqzc	r7,00000014,00401F2A

l00401EEA:
	lwpc	r6,004544E4
	bgec	r0,r6,00401F2A

l00401EF4:
	ext	r5,r6,00000005,0000000B
	li	r7,00000001
	sllv	r7,r7,r6
	addiupc	r6,0005043E
	lwxs	r6,r5(r6)
	and	r7,r7,r6
	bnezc	r7,00401F2A

l00401F08:
	balc	00401E5C
	lwpc	r5,004544E4
	lui	r7,00000010
	addiupc	r4,0000F404
	mod	r6,r5,r7
	move.balc	r5,r6,004086C0
	lwpc	r4,00412EF4
	balc	004081A0

l00401F2A:
	lw	r7,0000(r16)
	li	r6,0001F400
	addiupc	r5,00031008
	move	r4,r17
	jalrc	ra,r7
	beqc	r0,r4,00402002

l00401F40:
	bltc	r0,r4,00402054

l00401F44:
	balc	004021AC
	lw	r7,0000(r4)
	beqic	r7,00000029,00402058

l00401F4C:
	balc	004021AC
	lw	r7,0000(r4)
	beqic	r7,0000000C,00402058

l00401F54:
	balc	004021AC
	lw	r7,0000(r4)
	beqic	r7,0000000B,00402106

l00401F5C:
	lw	r7,0004(r16)
	move	r4,r17
	jalrc	ra,r7
	move	r18,r4
	bltc	r0,r4,004020C4

l00401F68:
	bnezc	r4,00401F84

l00401F6A:
	lwpc	r7,00430080
	beqzc	r7,00401F84

l00401F72:
	balc	004021AC
	lw	r7,0000(r4)
	bneiuc	r7,00000016,00401F84

l00401F7A:
	swpc	r0,00430080
	balc	004021AC
	sw	r0,0000(sp)

l00401F84:
	balc	004021AC
	lw	r7,0000(r4)
	beqzc	r7,00401F2A

l00401F8A:
	bc	004020C6

l00401F8C:
	addiu	r4,sp,00000008
	balc	004021B0
	lw	r8,0490(r19)
	lw	r17,0008(sp)
	addiu	r5,r0,000003E8
	addiupc	r7,000524F2
	lw	r17,000C(sp)
	subu	r4,r4,r8
	lw	r19,0004(r7)
	mul	r4,r4,r5
	lwpc	r9,00430088
	subu	r6,r6,r19
	div	r8,r6,r5
	lwpc	r5,0043008C
	move	r10,r7
	addu	r8,r8,r4
	bnezc	r5,00401FD4

l00401FC0:
	bgeic	r8,0000000A,00401FD4

l00401FC4:
	balc	00401BB0
	bltc	r4,r9,00401FD4

l00401FCC:
	li	r4,0000000A
	subu	r4,r4,r8
	restore.jrc	00000030,ra,00000005

l00401FD4:
	mul	r9,r9,r5
	lwpc	r4,00454544
	addu	r4,r4,r8
	slt	r8,r4,r9
	movz	r4,r9,r8

l00401FE6:
	bgec	r4,r5,00401FEE

l00401FEA:
	subu	r4,r5,r4
	restore.jrc	00000030,ra,00000005

l00401FEE:
	lw	r17,0008(sp)
	subu	r4,r4,r5
	swpc	r4,00454544
	sw	r7,0490(r18)
	lw	r17,000C(sp)
	sw	r7,0001(r10)
	bc	00401EE0

l00402002:
	swpc	r0,00454540
	balc	00401BD8
	lwpc	r7,004544EC
	andi	r7,r7,00000011
	bneiuc	r7,00000001,00402044

l0040201A:
	lwpc	r5,00430070
	lwpc	r7,00430088
	bgec	r7,r5,00402034

l0040202A:
	lwpc	r7,00430078
	bltc	r7,r5,0040203C

l00402034:
	balc	00401BB0
	bgec	r4,r5,00402044

l0040203C:
	li	r5,00000001
	addiupc	r4,0000F2FA
	balc	00401C60

l00402044:
	lwpc	r7,00454544
	lwpc	r4,0043008C
	subu	r4,r4,r7
	restore.jrc	00000030,ra,00000005

l00402054:
	balc	00404A00

l00402058:
	lwpc	r7,004544F4
	li	r6,00061A7F
	swpc	r0,00454544
	bgec	r6,r7,004020FE

l0040206E:
	addiu	r6,r0,0000C350

l00402072:
	lwpc	r7,004544F8
	addu	r7,r7,r6
	swpc	r7,004544F8
	lwpc	r7,004544EC
	bbeqzc	r7,0000000E,0040208E

l0040208A:
	balc	00401C0E

l0040208E:
	lwpc	r7,0043008C
	li	r4,0000000A
	bltic	r7,00000016,004020AA

l0040209A:
	sra	r4,r7,00000001
	addiu	r7,r0,000001F4
	slti	r6,r4,000001F5
	movz	r4,r7,r6

l004020AA:
	lwpc	r7,00454540
	addiu	r7,r7,00000001
	lwpc	r6,00430084
	swpc	r7,00454540
	mul	r7,r7,r4
	bltc	r7,r6,004020FC

l004020C4:
	move	r18,r0

l004020C6:
	balc	00401BD8
	bnezc	r18,004020E4

l004020CC:
	lwpc	r7,004544EC
	bbnezc	r7,00000004,004020E4

l004020D6:
	bbeqzc	r7,00000000,0040211E

l004020DA:
	li	r5,00000001
	addiupc	r4,0000F098
	balc	00401C60

l004020E4:
	lwpc	r7,0043008C
	li	r6,0000000A
	slti	r4,r7,0000000A
	swpc	r0,00454544
	movz	r6,r7,r4

l004020FA:
	move	r4,r6

l004020FC:
	restore.jrc	00000030,ra,00000005

l004020FE:
	li	r5,00000008
	div	r6,r7,r5
	bc	00402072

l00402106:
	lwpc	r7,00454544
	li	r4,0000000A
	lwpc	r6,0043008C
	addu	r7,r7,r6
	swpc	r7,00454544
	restore.jrc	00000030,ra,00000005

l0040211E:
	addiupc	r4,0000F21E
	balc	00401E7A
	bc	004020E4

;; fn00402126: 00402126
;;   Called from:
;;     00401DA2 (in fill)
;;     004021A2 (in sock_setbufs)
;;     00402212 (in setup)
;;     00402274 (in setup)
fn00402126 proc
	bc	004083F0

;; fn0040212A: 0040212A
;;   Called from:
;;     00401D82 (in fill)
;;     00401DD6 (in fill)
;;     00401E76 (in print_timestamp)
fn0040212A proc
	bc	004086C0

;; sock_setbufs: 0040212E
;;   Called from:
;;     00401138 (in ping4_run)
;;     00403A0E (in ping6_run)
sock_setbufs proc
	save	00000020,ra,00000003
	li	r7,00000004
	sw	r7,000C(sp)
	lwpc	r7,004544FC
	movep	r17,r16,r4,r5
	bnezc	r7,00402144

l0040213E:
	swpc	r16,004544FC

l00402144:
	lw	r4,0000(r17)
	addiupc	r7,000523B2
	addiu	r8,r0,00000004
	addiu	r6,r0,00001001
	addiu	r5,r0,0000FFFF
	balc	004023EE
	lwpc	r7,00430088
	mul	r16,r16,r7
	addiu	r7,r0,0000FFFF
	bgec	r7,r16,004021A6

l00402168:
	sw	r16,0008(sp)

l0040216A:
	lw	r4,0000(r17)
	addiu	r8,r0,00000004
	addiu	r7,sp,00000008
	addiu	r6,r0,00001002
	addiu	r5,r0,0000FFFF
	balc	004023EE
	lw	r4,0000(r17)
	addiu	r8,sp,0000000C
	addiu	r7,sp,00000008
	addiu	r6,r0,00001002
	addiu	r5,r0,0000FFFF
	balc	004066D0
	bnezc	r4,004021A4

l00402192:
	lw	r17,0008(sp)
	bgec	r7,r16,004021A4

l00402198:
	lwpc	r5,00412EF0
	addiupc	r4,0000F1AE
	balc	00402126

l004021A4:
	restore.jrc	00000020,ra,00000003

l004021A6:
	addiu	r7,r7,00000001
	sw	r7,0008(sp)
	bc	0040216A

;; fn004021AC: 004021AC
;;   Called from:
;;     00401F44 (in pinger)
;;     00401F4C (in pinger)
;;     00401F54 (in pinger)
;;     00401F72 (in pinger)
;;     00401F80 (in pinger)
;;     00401F84 (in pinger)
fn004021AC proc
	bc	004049B0

;; fn004021B0: 004021B0
;;   Called from:
;;     00401E6C (in print_timestamp)
;;     00401EC8 (in pinger)
;;     00401F8E (in pinger)
;;     00402396 (in setup)
;;     004024AE (in gather_statistics)
fn004021B0 proc
	bc	0040AF40

;; setup: 004021B4
;;   Called from:
;;     00401272 (in ping4_run)
;;     00403C6C (in ping6_run)
setup proc
	save	000000B0,ra,00000003
	lwpc	r7,004544EC
	move	r16,r4
	andi	r6,r7,00000003
	bneiuc	r6,00000001,004021CA

l004021C4:
	swpc	r0,0043008C

l004021CA:
	lwpc	r5,00454514
	lwpc	r6,0043008C
	beqzc	r5,004021F4

l004021D8:
	addiu	r5,r0,000000C7
	bltc	r5,r6,004021F4

l004021E0:
	addiu	r6,r0,000000C8
	addiupc	r5,0000F1A4
	lwpc	r4,00412EF0
	balc	004023F2

l004021F0:
	li	r4,00000002
	balc	00401E7E

l004021F4:
	lwpc	r4,00430088
	li	r17,7FFFFFFF
	div	r5,r17,r4
	bltc	r6,r5,00402216

l00402208:
	lwpc	r5,00412EF0
	addiupc	r4,0000F1BA
	balc	00402126
	bc	004021F0

l00402216:
	li	r6,00000001
	sw	r6,0004(sp)
	bbeqzc	r7,00000006,0040222E

l0040221E:
	addiu	r8,r0,00000004
	lw	r4,0000(r16)
	addu	r7,sp,r8
	addiu	r5,r0,0000FFFF
	balc	004023EE

l0040222E:
	lwpc	r7,004544EC
	bbeqzc	r7,00000007,0040224A

l00402238:
	addiu	r8,r0,00000004
	lw	r4,0000(r16)
	addu	r7,sp,r8
	li	r6,00000010
	addiu	r5,r0,0000FFFF
	balc	004023EE

l0040224A:
	lwpc	r7,004544EC
	bbnezc	r7,0000000C,00402276

l00402254:
	li	r7,00000001
	lw	r4,0000(r16)
	sw	r7,0020(sp)
	addiu	r8,r0,00000004
	li	r6,0000001D
	addiu	r5,r0,0000FFFF
	addiu	r7,sp,00000020
	balc	004023EE
	beqzc	r4,00402276

l0040226A:
	lwpc	r5,00412EF0
	addiupc	r4,0000F180
	balc	00402126

l00402276:
	lwpc	r7,004544EC
	bbeqzc	r7,00000012,004022B6

l00402280:
	li	r4,00000001
	balc	00401CE2
	lw	r4,0000(r16)
	addiupc	r7,0005223C
	addiu	r8,r0,00000004
	li	r6,00000024
	addiu	r5,r0,0000FFFF
	balc	004023EE
	move	r17,r4
	move	r4,r0
	balc	00401CE2
	li	r7,FFFFFFFF
	bnec	r17,r7,004022B6

l004022A4:
	lwpc	r6,004544C8
	addiupc	r5,0000F186
	lwpc	r4,00412EF0
	balc	004023F2

l004022B6:
	li	r7,00000001
	lwpc	r6,0043008C
	sw	r7,0008(sp)
	addiu	r7,r0,000003E7
	sw	r0,000C(sp)
	bltc	r7,r6,004022E0

l004022CA:
	slti	r7,r6,0000000A
	li	r5,0000000A
	movz	r5,r6,r7

l004022D4:
	addiu	r6,r0,000003E8
	mul	r7,r5,r6
	sw	r0,0008(sp)
	sw	r7,000C(sp)

l004022E0:
	addiu	r8,r0,00000008
	lw	r4,0000(r16)
	addu	r7,sp,r8
	addiu	r6,r0,00001005
	addiu	r5,r0,0000FFFF
	balc	004023EE
	lwpc	r6,0043008C
	slti	r7,r6,0000000A
	li	r5,0000000A
	movz	r5,r6,r7

l00402304:
	addiu	r8,r0,00000008
	move	r7,r5
	addiu	r5,r0,000003E8
	div	r6,r7,r5
	sw	r6,0008(sp)
	mod	r6,r7,r5
	mul	r7,r6,r5
	lw	r4,0000(r16)
	addiu	r6,r0,00001006
	addiu	r5,r0,0000FFFF
	sw	r7,000C(sp)
	addu	r7,sp,r8
	balc	004023EE
	beqzc	r4,00402340

l00402330:
	lwpc	r7,004544EC
	ori	r7,r7,00000800
	swpc	r7,004544EC

l00402340:
	lwpc	r7,004544EC
	andi	r7,r7,00000008
	lwpc	r5,00430074
	beqc	r0,r7,004023E8

l00402352:
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00402368

l00402358:
	balc	0040AFB0
	andi	r4,r4,0000FFFF
	balc	00406700
	swpc	r4,004544C4

l00402368:
	addiupc	r5,FFFFF92C
	li	r4,00000002
	balc	0040271C
	addiupc	r5,FFFFF924
	li	r4,0000000E
	balc	0040271C
	addiupc	r5,FFFFF888
	li	r4,00000003
	balc	0040271C
	addiu	r4,sp,00000020
	balc	00408030
	move	r6,r0
	addiu	r5,sp,00000020
	li	r4,00000002
	balc	00408040
	move	r5,r0
	addiupc	r4,00052102
	balc	004021B0
	lwpc	r7,004314C8
	beqzc	r7,004023B2

l004023A0:
	move	r6,r0
	addiu	r5,sp,00000010
	move	r4,r0
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r7,0018(sp)
	sw	r0,001C(sp)
	balc	00407F10

l004023B2:
	li	r4,00000001
	balc	0040AFD0
	beqzc	r4,004023D8

l004023BA:
	addiu	r6,sp,00000010
	li	r5,40087468
	li	r4,00000001
	balc	00405B80
	li	r7,FFFFFFFF
	beqc	r4,r7,004023D8

l004023CC:
	lhu	r7,0012(sp)
	beqzc	r7,004023D8

l004023D2:
	swpc	r7,00430070

l004023D8:
	restore.jrc	000000B0,ra,00000003

l004023DA:
	addiupc	r6,00030B60
	addu	r6,r7,r6
	sb	r7,0008(r6)
	addiu	r7,r7,00000001

l004023E8:
	bltc	r7,r5,004023DA

l004023EC:
	bc	00402352

;; fn004023EE: 004023EE
;;   Called from:
;;     00402156 (in sock_setbufs)
;;     0040217A (in sock_setbufs)
;;     0040222C (in setup)
;;     00402248 (in setup)
;;     00402266 (in setup)
;;     00402296 (in setup)
;;     004022F2 (in setup)
;;     0040232C (in setup)
fn004023EE proc
	bc	00407D60

;; fn004023F2: 004023F2
;;   Called from:
;;     004021EE (in setup)
;;     004022B4 (in setup)
;;     004024A0 (in gather_statistics)
fn004023F2 proc
	bc	00408340

;; gather_statistics: 004023F6
;;   Called from:
;;     004019D2 (in ping4_parse_reply)
;;     004030F8 (in ping6_parse_reply)
gather_statistics proc
	save	00000040,r30,0000000A
	movep	r19,r16,r6,r7
	lwpc	r7,004544DC
	addiu	r7,r7,00000001
	move	r23,r4
	move	r21,r8
	move	r18,r9
	move	r17,r10
	move	r30,r11
	addu	r20,r4,r5
	swpc	r7,004544DC
	bnec	r0,r9,0040245C

l0040241A:
	lwpc	r7,004544E4
	subu	r5,r7,r16
	andi	r6,r5,0000FFFF
	bbnezc	r5,0000000F,0040245C

l00402428:
	lwpc	r5,00430078
	bltc	r6,r5,0040243A

l00402432:
	addiu	r6,r6,00000001
	swpc	r6,00430078

l0040243A:
	aluipc	r6,00000402
	lhu	r4,0518(r6)
	subu	r5,r16,r4
	seh	r5,r5
	bltc	r0,r5,00402458

l0040244C:
	andi	r7,r7,0000FFFF
	addiu	r5,r0,00007FFF
	subu	r7,r7,r4
	bgec	r5,r7,0040245C

l00402458:
	sh	r16,0518(r6)

l0040245C:
	lwpc	r7,00454508
	move	r22,r0
	beqc	r0,r7,004025DC

l00402468:
	bltiuc	r19,00000010,004025DC

l0040246C:
	li	r6,00000008
	addu	r4,sp,r6
	move.balc	r5,r20,0040A130
	bc	004024C2

l00402478:
	lw	r8,0000(r17)
	lw	r17,0008(sp)
	subu	r8,r8,r7
	li	r7,000F4240
	sw	r8,0000(r17)
	mul	r8,r8,r7
	lw	r7,0004(r17)
	addu	r22,r8,r7
	bgec	r22,r0,004024E0

l00402494:
	move	r6,r22
	addiupc	r5,0000EFBA
	lwpc	r4,00412EF0
	balc	004023F2
	lwpc	r7,004544EC
	bbnezc	r7,0000000C,004024DE

l004024AC:
	movep	r4,r5,r17,r0
	balc	004021B0
	lwpc	r7,004544EC
	addiu	r6,r0,00001000
	or	r7,r7,r6
	swpc	r7,004544EC

l004024C2:
	lw	r7,0004(r17)
	lw	r17,000C(sp)
	subu	r7,r7,r6
	sw	r7,0044(sp)
	bgec	r7,r0,00402478

l004024CE:
	lw	r6,0000(r17)
	addiu	r7,r7,000F4240
	addiu	r6,r6,FFFFFFFF
	swm	r6,0000(r17),00000002
	bc	00402478

l004024DE:
	move	r22,r0

l004024E0:
	bnec	r0,r18,004025DE

l004024E4:
	aluipc	r6,00000402
	sra	r5,r22,0000001F
	lw	r7,0480(r6)
	lw	r4,0484(r6)
	addu	r17,r7,r22
	addu	r5,r4,r5
	sltu	r7,r17,r7
	addu	r7,r7,r5
	sw	r17,0480(r6)
	sw	r7,0484(r6)
	aluipc	r5,00000402
	mul	r6,r22,r22
	muh	r4,r22,r22
	lw	r7,0488(r5)
	lw	r17,048C(r5)
	addu	r6,r7,r6
	sltu	r7,r6,r7
	addu	r4,r17,r4
	addu	r7,r7,r4
	sw	r6,0488(r5)
	sw	r7,048C(r5)
	lwpc	r7,0043007C
	bgec	r22,r7,0040253E

l00402538:
	swpc	r22,0043007C

l0040253E:
	lwpc	r7,0045450C
	bgec	r7,r22,0040254E

l00402548:
	swpc	r22,0045450C

l0040254E:
	lwpc	r6,004544F4
	sll	r7,r22,00000003
	beqzc	r6,00402566

l0040255A:
	li	r5,00000008
	div	r7,r6,r5
	subu	r7,r22,r7
	addu	r7,r7,r6

l00402566:
	swpc	r7,004544F4
	lwpc	r7,004544EC
	bbeqzc	r7,0000000E,0040257A

l00402576:
	balc	00401C0E

l0040257A:
	srl	r7,r16,00000005
	addiupc	r6,0004FDC0
	lsa	r7,r7,r6,00000002
	li	r17,00000001
	sllv	r16,r17,r16
	lw	r6,0000(r7)
	and	r5,r6,r16
	beqzc	r5,004025FE

l00402592:
	lwpc	r7,004544E0
	addu	r7,r7,r17
	swpc	r7,004544E0
	lwpc	r7,004544DC
	addiu	r7,r7,FFFFFFFF
	swpc	r7,004544DC

l004025AE:
	lwpc	r7,00430080
	li	r16,00000001
	swpc	r7,004314C4
	lwpc	r7,004544EC
	bbnezc	r7,00000004,004025D8

l004025C6:
	and	r16,r16,r7
	beqzc	r16,0040260C

l004025CA:
	bnezc	r18,00402604

l004025CC:
	li	r5,00000003
	addiupc	r4,0000EEC6

l004025D2:
	balc	00401C60
	move	r16,r0

l004025D8:
	move	r4,r16
	restore.jrc	00000040,r30,0000000A

l004025DC:
	beqzc	r18,0040257A

l004025DE:
	lwpc	r7,004544CC
	addiu	r7,r7,00000001
	swpc	r7,004544CC
	lwpc	r7,004544DC
	addiu	r7,r7,FFFFFFFF
	swpc	r7,004544DC

l004025FA:
	move	r17,r0
	bc	004025AE

l004025FE:
	or	r16,r16,r6
	sw	r16,0000(r7)
	bc	004025FA

l00402604:
	li	r5,00000002
	addiupc	r4,0000EE92
	bc	004025D2

l0040260C:
	balc	00401E5C
	move	r6,r30
	addiupc	r4,0000EE8A
	move.balc	r5,r19,004086C0
	lw	r17,0040(sp)
	beqzc	r7,00402622

l0040261E:
	movep	r4,r5,r23,r19
	jalrc	ra,r7

l00402622:
	bltc	r21,r0,0040262E

l00402626:
	addiupc	r4,0000EE8A
	move.balc	r5,r21,004086C0

l0040262E:
	lwpc	r7,00430074
	addiu	r7,r7,00000007
	bltc	r7,r19,00402646

l0040263A:
	addiupc	r4,0000EE7E
	li	r16,00000001
	balc	00408780
	bc	004025D8

l00402646:
	lwpc	r7,00454508
	beqzc	r7,00402668

l0040264E:
	li	r7,0001869F
	bgec	r7,r22,004026CA

l00402658:
	addiu	r5,r0,000003E8
	addiupc	r4,0000EE6C
	div	r7,r22,r5
	move.balc	r5,r7,004086C0

l00402668:
	beqzc	r17,00402670

l0040266A:
	addiupc	r4,0000EEAA
	balc	004029C6

l00402670:
	beqzc	r18,00402678

l00402672:
	addiupc	r4,0000EEAA
	balc	004029C6

l00402678:
	lwpc	r4,00430074
	li	r5,00000008

l00402680:
	bgec	r5,r4,004025D8

l00402684:
	addiupc	r6,000308B6
	lbux	r7,r5(r20)
	addu	r6,r5,r6
	lbu	r6,0008(r6)
	beqc	r6,r7,00402718

l00402698:
	addiupc	r4,0000EE98
	li	r17,00000008
	balc	004029C6

l004026A0:
	lwpc	r7,00430074
	bgec	r17,r7,004025D8

l004026AA:
	li	r6,00000020
	mod	r7,r17,r6
	bneiuc	r7,00000008,004026BC

l004026B4:
	addiupc	r4,0000EEB0
	move.balc	r5,r17,004086C0

l004026BC:
	lbux	r5,r17(r20)
	addiupc	r4,0000EEAC
	addiu	r17,r17,00000001
	balc	004029C6
	bc	004026A0

l004026CA:
	addiu	r7,r0,0000270F
	bgec	r7,r22,004026EE

l004026D2:
	addiu	r5,r0,000003E8
	li	r7,00000064
	mod	r6,r22,r5
	div	r4,r6,r7
	div	r7,r22,r5
	movep	r5,r6,r7,r4
	addiupc	r4,0000EDF2

l004026EA:
	balc	004029C6
	bc	00402668

l004026EE:
	addiu	r7,r0,000003E7
	bgec	r7,r22,00402710

l004026F6:
	addiu	r5,r0,000003E8
	li	r7,0000000A
	mod	r6,r22,r5
	div	r4,r6,r7
	div	r7,r22,r5
	movep	r5,r6,r7,r4
	addiupc	r4,0000EDE2
	bc	004026EA

l00402710:
	movep	r5,r6,r0,r22
	addiupc	r4,0000EDEE
	bc	004026EA

l00402718:
	addiu	r5,r5,00000001
	bc	00402680

;; fn0040271C: 0040271C
;;   Called from:
;;     0040236E (in setup)
;;     00402376 (in setup)
;;     0040237E (in setup)
fn0040271C proc
	bc	00401C7C

;; finish: 00402720
;;   Called from:
;;     00402C92 (in main_loop)
finish proc
	save	00000050,r30,0000000A
	aluipc	r7,00000402
	lw	r16,0490(r7)
	addiupc	r7,00051D62
	lw	r20,0001(r7)
	addiupc	r7,00051D64
	lw	r4,0004(r7)
	subu	r20,r20,r4
	bgec	r20,r0,00402746

l0040273E:
	addiu	r16,r16,FFFFFFFF
	addiu	r20,r20,000F4240

l00402746:
	aluipc	r7,00000402
	li	r4,0000000A
	lw	r5,0498(r7)
	subu	r16,r16,r5
	balc	00402A96
	lwpc	r4,00412EF4
	balc	004081A0
	lwpc	r5,004544C0
	addiupc	r4,0000EE10
	balc	004029C6
	lwpc	r5,004544E4
	addiupc	r4,0000EE20
	balc	004029C6
	lwpc	r5,004544DC
	addiupc	r4,0000EE30
	balc	004029C6
	lwpc	r5,004544E0
	beqzc	r5,00402790

l0040278A:
	addiupc	r4,0000EE32
	balc	004029C6

l00402790:
	lwpc	r5,004544CC
	beqzc	r5,0040279E

l00402798:
	addiupc	r4,0000EE38
	balc	004029C6

l0040279E:
	lwpc	r5,004544D0
	beqzc	r5,004027AC

l004027A6:
	addiupc	r4,0000EE3E
	balc	004029C6

l004027AC:
	lwpc	r6,004544E4
	beqzc	r6,004027E6

l004027B4:
	lwpc	r4,004544DC
	li	r17,00000064
	subu	r4,r6,r4
	sra	r7,r6,0000001F
	muh	r5,r4,r17
	mul	r4,r4,r17
	balc	00402A9A
	move	r5,r4
	addiupc	r4,0000EE28
	balc	004029C6
	addiu	r7,r0,000003E8
	mul	r5,r16,r7
	div	r6,r20,r7
	addiupc	r4,0000EE2A
	addu	r5,r5,r6
	balc	004029C6

l004027E6:
	li	r4,0000000A
	addiupc	r18,0000DC8C
	balc	00402A96
	lwpc	r17,004544DC
	beqc	r0,r17,00402908

l004027F8:
	lwpc	r7,00454508
	beqc	r0,r7,00402908

l00402802:
	lwpc	r7,004544E0
	aluipc	r18,00000402
	addu	r17,r17,r7
	lw	r4,0480(r18)
	sra	r21,r17,0000001F
	lw	r5,0484(r18)
	movep	r6,r7,r17,r21
	balc	00402A9A
	movep	r19,r22,r4,r5
	sw	r19,0480(r18)
	sw	r22,0484(r18)
	aluipc	r18,00000402
	lw	r4,0488(r18)
	lw	r5,048C(r18)
	movep	r6,r7,r17,r21
	balc	00402A9A
	mul	r17,r22,r19
	mul	r21,r19,r19
	muhu	r7,r19,r19
	sw	r4,0488(r18)
	sw	r5,048C(r18)
	subu	r21,r4,r21
	lsa	r17,r17,r7,00000001
	sltu	r4,r4,r21
	subu	r5,r5,r17
	subu	r17,r5,r4
	bltc	r0,r17,00402868

l00402860:
	bnec	r0,r17,004029C2

l00402864:
	beqc	r0,r21,004029C2

l00402868:
	move	r18,r21
	move	r23,r17
	li	r4,FFFFFFFF
	li	r7,7FFFFFFF
	bc	004028A4

l00402876:
	movep	r6,r7,r18,r23
	movep	r4,r5,r21,r17
	balc	00402A9A
	addu	r6,r4,r18
	addu	r5,r5,r23
	sltu	r4,r6,r4
	addu	r5,r4,r5
	move	r4,r18
	srl	r7,r5,0000001F
	addu	r6,r7,r6
	sltu	r7,r6,r7
	srl	r6,r6,00000001
	addu	r5,r7,r5
	move	r7,r23
	sll	r30,r5,0000001F
	sra	r23,r5,00000001
	or	r18,r30,r6

l004028A4:
	bltc	r23,r7,00402876

l004028A8:
	bnec	r7,r23,004028B0

l004028AC:
	bltuc	r18,r4,00402876

l004028B0:
	addiu	r17,r0,000003E8
	lwpc	r7,0045450C
	mod	r10,r7,r17
	div	r30,r7,r17
	addiu	r6,r0,000003E8
	move	r7,r0
	movep	r4,r5,r19,r22
	div	r11,r18,r17
	swm	r10,0018(sp),00000002
	balc	00403F30
	move	r23,r4
	addiu	r6,r0,000003E8
	move	r7,r0
	lwpc	r21,0043007C
	movep	r4,r5,r19,r22
	balc	00402A9A
	mod	r7,r18,r17
	move	r9,r30
	sw	r7,0000(sp)
	movep	r7,r8,r4,r23
	addiupc	r4,0000ED26
	mod	r6,r21,r17
	div	r5,r21,r17
	addiupc	r18,0000EC72
	lwm	r10,0018(sp),00000002
	balc	004029C6

l00402908:
	lwpc	r6,00430078
	bltic	r6,00000002,0040291E

l00402912:
	addiupc	r4,0000ED4A
	move.balc	r5,r18,004086C0
	addiupc	r18,0000EC56

l0040291E:
	lwpc	r7,004544DC
	beqzc	r7,0040299E

l00402926:
	lwpc	r7,0043008C
	beqzc	r7,0040293C

l0040292E:
	addiu	r6,r0,00004001
	lwpc	r7,004544EC
	and	r7,r7,r6
	beqzc	r7,0040299E

l0040293C:
	lwpc	r6,004544E4
	bltic	r6,00000002,0040299E

l00402946:
	li	r7,000F4240
	addiu	r6,r6,FFFFFFFF
	mul	r5,r16,r7
	muh	r16,r16,r7
	sra	r7,r20,0000001F
	addu	r4,r5,r20
	sltu	r5,r4,r5
	addu	r16,r16,r7
	addu	r5,r5,r16
	sra	r7,r6,0000001F
	balc	00402A9A
	lwpc	r7,004544F4
	li	r6,00000008
	addiu	r8,r0,00001F40
	div	r9,r7,r6
	addiu	r6,r0,000003E8
	mod	r5,r9,r6
	move	r9,r5
	div	r5,r7,r8
	mod	r7,r4,r6
	move	r8,r5
	div	r5,r4,r6
	addiupc	r4,0000ECD4
	move	r6,r5
	move.balc	r5,r18,004086C0

l0040299E:
	li	r4,0000000A
	balc	00402A96
	lwpc	r7,004544DC
	li	r4,00000001
	beqzc	r7,004029BE

l004029AC:
	lwpc	r4,004314C8
	beqzc	r4,004029BE

l004029B4:
	lwpc	r4,004544D8
	slt	r4,r7,r4

l004029BE:
	balc	0040015A

l004029C2:
	move	r18,r21
	bc	004028B0

;; fn004029C6: 004029C6
;;   Called from:
;;     0040266E (in gather_statistics)
;;     00402676 (in gather_statistics)
;;     0040269E (in gather_statistics)
;;     004026C6 (in gather_statistics)
;;     004026EA (in gather_statistics)
;;     00402768 (in finish)
;;     00402774 (in finish)
;;     00402780 (in finish)
;;     0040278E (in finish)
;;     0040279C (in finish)
;;     004027AA (in finish)
;;     004027D0 (in finish)
;;     004027E4 (in finish)
;;     00402906 (in finish)
fn004029C6 proc
	bc	004086C0

;; status: 004029CA
;;   Called from:
;;     00402AEA (in main_loop)
status proc
	save	00000030,ra,00000003
	lwpc	r16,004544E4
	move	r4,r0
	swpc	r0,00454500
	lwpc	r17,004544DC
	beqzc	r16,004029F6

l004029E2:
	subu	r8,r16,r17
	li	r4,00000064
	muh	r5,r8,r4
	move	r6,r16
	mul	r4,r4,r8
	sra	r7,r16,0000001F
	balc	00402A9A

l004029F6:
	lwpc	r7,00412EF0
	move	r8,r4
	sw	r7,001C(sp)
	movep	r6,r7,r17,r16
	lw	r17,001C(sp)
	addiupc	r5,0000EC84
	balc	00408340
	lwpc	r6,004544DC
	beqzc	r6,00402A8A

l00402A14:
	lwpc	r7,00454508
	beqzc	r7,00402A8A

l00402A1C:
	lwpc	r7,004544E0
	aluipc	r5,00000402
	addu	r6,r6,r7
	lw	r4,0480(r5)
	lw	r5,0484(r5)
	sra	r7,r6,0000001F
	balc	00402A9A
	lwpc	r7,004544F4
	li	r6,00000008
	lwpc	r5,0043007C
	div	r11,r7,r6
	lwpc	r16,0045450C
	addiu	r6,r0,000003E8
	addiu	r10,r0,00001F40
	mod	r17,r16,r6
	mod	r9,r4,r6
	div	r8,r4,r6
	sw	r17,0004(sp)
	div	r4,r5,r6
	div	r17,r16,r6
	mod	r16,r11,r6
	move	r11,r16
	div	r16,r7,r10
	mod	r7,r5,r6
	move	r6,r4
	lw	r17,001C(sp)
	move	r10,r16
	addiupc	r5,0000EC24
	sw	r17,0000(sp)
	balc	00408340

l00402A8A:
	lw	r17,001C(sp)
	li	r4,0000000A
	restore	00000030,ra,00000003
	bc	00408380

;; fn00402A96: 00402A96
;;   Called from:
;;     00402752 (in finish)
;;     004027EC (in finish)
;;     004029A0 (in finish)
fn00402A96 proc
	bc	00408770

;; fn00402A9A: 00402A9A
;;   Called from:
;;     004027C8 (in finish)
;;     0040281C (in finish)
;;     00402836 (in finish)
;;     0040287A (in finish)
;;     004028E6 (in finish)
;;     0040296A (in finish)
;;     004029F4 (in status)
;;     00402A34 (in status)
fn00402A9A proc
	bc	00403CB0

;; main_loop: 00402A9E
;;   Called from:
;;     0040127C (in ping4_run)
;;     00403C7A (in ping6_run)
main_loop proc
	save	00000020,ra,00000007
	addiu	sp,sp,FFFFEF50
	movep	r17,r16,r4,r5
	move	r18,r7
	sw	r6,0004(sp)

l00402AAC:
	lwpc	r7,004544B8
	bnec	r0,r7,00402C92

l00402AB6:
	lwpc	r6,004544D8
	beqzc	r6,00402AD0

l00402ABE:
	lwpc	r5,004544D0
	lwpc	r7,004544DC
	addu	r7,r7,r5
	bgec	r7,r6,00402C92

l00402AD0:
	lwpc	r7,004314C8
	beqzc	r7,00402AE2

l00402AD8:
	lwpc	r7,004544D0
	bnec	r0,r7,00402C92

l00402AE2:
	lwpc	r7,00454500
	beqzc	r7,00402AEC

l00402AEA:
	balc	004029CA

l00402AEC:
	movep	r4,r5,r17,r16
	balc	00401E86
	lwpc	r7,004544D8
	move	r20,r4
	beqzc	r7,00402B14

l00402AFC:
	lwpc	r6,004544E4
	bltc	r6,r7,00402B14

l00402B06:
	lwpc	r7,004314C8
	bnezc	r7,00402B14

l00402B0E:
	balc	00401DDA
	move	r20,r4

l00402B14:
	bgec	r0,r20,00402AEC

l00402B18:
	addiu	r6,r0,00004800
	lwpc	r7,004544EC
	and	r7,r7,r6
	bnezc	r7,00402BA4

l00402B26:
	lwpc	r6,0043008C
	li	r5,0000000A
	slti	r7,r6,0000000A
	movz	r5,r6,r7

l00402B36:
	bltc	r20,r5,00402BA4

l00402B3A:
	move	r6,r0

l00402B3C:
	li	r7,FFFFEF64
	addiu	r5,sp,000010B0
	addiu	r7,r7,0000001C
	lw	r4,0000(r16)
	addu	r7,r5,r7
	addiu	r5,sp,00000014
	sw	r7,0014(sp)
	addiu	r7,r0,00000080
	sw	r7,0018(sp)
	addiu	r7,sp,00000004
	sw	r7,001C(sp)
	li	r7,00000001
	sw	r7,0020(sp)
	addiu	r7,sp,000000B0
	sw	r7,0024(sp)
	addiu	r7,r0,00001000
	sw	r18,0008(sp)
	sw	r0,002C(sp)
	sw	r7,0028(sp)
	balc	00407670
	move	r19,r4
	bgec	r4,r0,00402C06

l00402B76:
	balc	00402CEA
	lw	r7,0000(r4)
	beqic	r7,0000000B,00402AAC

l00402B7E:
	balc	00402CEA
	lw	r7,0000(r4)
	beqic	r7,00000004,00402AAC

l00402B86:
	lw	r7,0004(r17)
	move	r4,r16
	jalrc	ra,r7
	bnec	r0,r4,00402C74

l00402B90:
	balc	00402CEA
	lw	r7,0000(r4)
	bnezc	r7,00402BFC

l00402B96:
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00402C74

l00402B9C:
	lw	r7,000C(r17)
	move	r4,r16
	jalrc	ra,r7
	bc	00402C74

l00402BA4:
	balc	00401BB0
	addiu	r21,r0,000003E8
	move	r19,r4
	li	r4,00000002
	balc	00402CEE
	mod	r7,r21,r4
	bnezc	r7,00402BDC

l00402BB8:
	li	r4,00000002
	balc	00402CEE
	div	r7,r21,r4
	slt	r4,r7,r20
	xori	r4,r4,00000001

l00402BC8:
	beqc	r0,r4,00402C96

l00402BCC:
	addiu	r20,r0,0000000A
	bnec	r0,r19,00402C96

l00402BD4:
	balc	00407E10

l00402BD8:
	li	r6,00000040
	bc	00402B3C

l00402BDC:
	li	r4,00000002
	balc	00402CEE
	li	r6,7FFFFFFF
	div	r7,r6,r4
	move	r4,r0
	bgec	r20,r7,00402BC8

l00402BF0:
	li	r4,00000002
	balc	00402CEE
	mul	r4,r4,r20
	slti	r4,r4,000003E9
	bc	00402BC8

l00402BFC:
	addiupc	r4,0000EAE8
	balc	00408630
	bc	00402AAC

l00402C06:
	lw	r17,0028(sp)
	move	r8,r0
	bltiuc	r4,0000000C,00402C4E

l00402C0E:
	lw	r5,0024(sp)
	move	r7,r20

l00402C12:
	beqzc	r7,00402C4E

l00402C14:
	lw	r21,0001(r7)
	addiu	r5,r0,0000FFFF
	lw	r6,0000(r7)
	bnec	r21,r5,00402C46

l00402C20:
	lw	r5,0008(r7)
	bneiuc	r5,0000001D,00402C46

l00402C26:
	bltiuc	r6,00000014,00402C46

l00402C2A:
	addiu	r8,r7,0000000C

l00402C2E:
	addiu	r6,r6,00000003
	addu	r5,r20,r4
	ins	r6,r0,00000000,00000001
	subu	r5,r5,r7
	addiu	r21,r6,0000000C
	addu	r7,r7,r6
	bltuc	r21,r5,00402C12

l00402C44:
	bc	00402C4A

l00402C46:
	bgeiuc	r6,0000000C,00402C2E

l00402C4A:
	move	r7,r0
	bc	00402C12

l00402C4E:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000C,00402C7E

l00402C58:
	move	r5,r0
	addiu	r4,sp,0000000C
	balc	0040AF40

l00402C60:
	addiu	r7,sp,0000000C
	move	r8,r7

l00402C64:
	lw	r20,0002(r17)
	addiu	r7,sp,00000030
	move	r6,r19
	addiu	r5,sp,00000014
	move	r4,r16
	jalrc	ra,r20
	bnec	r0,r4,00402B96

l00402C74:
	balc	00401BB0
	bnec	r0,r4,00402BD8

l00402C7C:
	bc	00402AAC

l00402C7E:
	bnec	r0,r8,00402C64

l00402C82:
	lw	r4,0000(r16)
	addiu	r6,sp,0000000C
	addiu	r5,r0,00008906
	balc	00405B80
	bnezc	r4,00402C58

l00402C90:
	bc	00402C60

l00402C92:
	balc	00402720

l00402C96:
	addiu	r6,r0,00004800
	lwpc	r7,004544EC
	and	r7,r7,r6
	bnezc	r7,00402CAE

l00402CA4:
	lwpc	r7,0043008C
	beqc	r0,r7,00402B3A

l00402CAE:
	lw	r7,0000(r16)
	move	r6,r20
	li	r5,00000001
	addiu	r4,sp,0000000C
	sw	r7,000C(sp)
	li	r7,00000009
	sh	r0,0012(sp)
	sh	r7,0010(sp)
	balc	00407E20
	bgec	r0,r4,00402AAC

l00402CCA:
	lhu	r7,0012(sp)
	andi	r7,r7,00000009
	beqc	r0,r7,00402AAC

l00402CD4:
	bc	00402BD8

;; is_ours: 00402CD6
;;   Called from:
;;     00401896 (in ping4_receive_error_msg)
;;     004019AE (in ping4_parse_reply)
;;     00401A9E (in ping4_parse_reply)
;;     0040301A (in ping6_receive_error_msg)
;;     004030D0 (in ping6_parse_reply)
;;     004031AE (in ping6_parse_reply)
is_ours proc
	lw	r7,0004(r4)
	li	r4,00000001
	beqc	r4,r7,00402CE8

l00402CDC:
	lwpc	r4,004544C4
	xor	r4,r4,r5
	sltiu	r4,r4,00000001

l00402CE8:
	jrc	ra

;; fn00402CEA: 00402CEA
;;   Called from:
;;     00402B76 (in main_loop)
;;     00402B7E (in main_loop)
;;     00402B90 (in main_loop)
fn00402CEA proc
	bc	004049B0

;; fn00402CEE: 00402CEE
;;   Called from:
;;     00402BB0 (in main_loop)
;;     00402BBA (in main_loop)
;;     00402BDE (in main_loop)
;;     00402BF2 (in main_loop)
fn00402CEE proc
	bc	00404730
00402CF2       08 90 00 80 00 C0 00 80 00 C0 00 80 00 C0   ..............

;; niquery_option_help_handler: 00402D00
;;   Called from:
;;     00403442 (in niquery_option_handler)
niquery_option_help_handler proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,0000E9EC
	balc	00403098
	li	r4,00000002
	balc	004030A0

;; niquery_option_subject_name_handler: 00402D12
;;   Called from:
;;     00402D10 (in niquery_option_help_handler)
niquery_option_subject_name_handler proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,0000EABE
	balc	00403098
	li	r4,00000003
	balc	004030A0

;; niquery_set_qtype: 00402D24
;;   Called from:
;;     00402D22 (in niquery_option_subject_name_handler)
;;     00402D4E (in niquery_option_ipv4_flag_handler)
;;     00402D7A (in niquery_option_ipv4_handler)
;;     00402D88 (in niquery_option_ipv6_flag_handler)
;;     00402DB4 (in niquery_option_ipv6_handler)
;;     00402DC0 (in niquery_option_name_handler)
niquery_set_qtype proc
	save	00000010,ra,00000001
	lwpc	r7,00430214
	bltc	r7,r0,00402D3E

l00402D30:
	beqc	r4,r7,00402D3E

l00402D32:
	addiupc	r4,0000EAD6
	balc	00408780
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000001

l00402D3E:
	swpc	r4,00430214
	move	r4,r0
	restore.jrc	00000010,ra,00000001

;; niquery_option_ipv4_flag_handler: 00402D48
niquery_option_ipv4_flag_handler proc
	save	00000010,ra,00000002
	move	r16,r4
	li	r4,00000004
	balc	00402D24
	li	r7,FFFFFFFF
	bltc	r4,r0,00402D72

l00402D56:
	li	r4,00000014
	lwpc	r7,004314D8
	mul	r16,r16,r4
	addiupc	r4,0002D36C
	addu	r4,r4,r16
	lw	r6,000C(r4)
	or	r7,r7,r6
	swpc	r7,004314D8
	move	r7,r0

l00402D72:
	move	r4,r7
	restore.jrc	00000010,ra,00000002

;; niquery_option_ipv4_handler: 00402D76
niquery_option_ipv4_handler proc
	save	00000010,ra,00000001
	li	r4,00000004
	balc	00402D24
	sra	r4,r4,0000001F
	restore.jrc	00000010,ra,00000001

;; niquery_option_ipv6_flag_handler: 00402D82
niquery_option_ipv6_flag_handler proc
	save	00000010,ra,00000002
	move	r16,r4
	li	r4,00000003
	balc	00402D24
	li	r7,FFFFFFFF
	bltc	r4,r0,00402DAC

l00402D90:
	li	r4,00000014
	lwpc	r7,004314D8
	mul	r16,r16,r4
	addiupc	r4,0002D332
	addu	r4,r4,r16
	lw	r6,000C(r4)
	or	r7,r7,r6
	swpc	r7,004314D8
	move	r7,r0

l00402DAC:
	move	r4,r7
	restore.jrc	00000010,ra,00000002

;; niquery_option_ipv6_handler: 00402DB0
niquery_option_ipv6_handler proc
	save	00000010,ra,00000001
	li	r4,00000003
	balc	00402D24
	sra	r4,r4,0000001F
	restore.jrc	00000010,ra,00000001

;; niquery_option_name_handler: 00402DBC
niquery_option_name_handler proc
	save	00000010,ra,00000001
	li	r4,00000002
	balc	00402D24
	sra	r4,r4,0000001F
	restore.jrc	00000010,ra,00000001

;; pr_icmph: 00402DC8
;;   Called from:
;;     00403070 (in ping6_receive_error_msg)
;;     00403246 (in ping6_parse_reply)
pr_icmph proc
	save	00000010,ra,00000003
	addiu	r7,r0,00000080
	movep	r16,r17,r5,r6
	beqc	r4,r7,00402EB4

l00402DD4:
	bltuc	r7,r4,00402E12

l00402DD8:
	beqic	r4,00000002,00402E72

l00402DDC:
	bgeiuc	r4,00000003,00402DEC

l00402DE0:
	beqic	r4,00000001,00402E34

l00402DE4:
	move	r5,r4
	addiupc	r4,0000EBCE
	bc	00402E6E

l00402DEC:
	beqic	r4,00000003,00402E84

l00402DF0:
	bneiuc	r4,00000004,00402DE4

l00402DF4:
	addiupc	r4,0000EB20
	balc	0040309C
	addiupc	r4,0000EB2E
	beqzc	r16,00402E08

l00402E00:
	bneiuc	r16,00000001,00402EA2

l00402E04:
	addiupc	r4,0000EB38

l00402E08:
	balc	0040309C

l00402E0A:
	move	r5,r17
	addiupc	r4,0000EB5C
	bc	00402E6E

l00402E12:
	addiu	r7,r0,00000082
	beqc	r4,r7,00402EC0

l00402E1A:
	bltuc	r4,r7,00402EBA

l00402E1E:
	addiu	r7,r0,00000083
	beqc	r4,r7,00402EC6

l00402E26:
	addiu	r7,r0,00000084
	bnec	r7,r4,00402DE4

l00402E2E:
	addiupc	r4,0000EB76
	bc	00402E4A

l00402E34:
	addiupc	r4,0000E9E4
	balc	0040309C
	bgeiuc	r16,00000005,00402E68

l00402E3E:
	addiupc	r7,0000F48E
	lwxs	r7,r16(r7)
	jrc	r7
00402E46                   80 04 EE E9                         ....      

l00402E4A:
	balc	0040309C

l00402E4C:
	move	r4,r0
	restore.jrc	00000010,ra,00000003
00402E50 80 04 F0 E9 F5 1B 80 04 06 EA EF 1B 80 04 20 EA .............. .
00402E60 E9 1B 80 04 2E EA E3 1B                         ........        

l00402E68:
	move	r5,r16
	addiupc	r4,0000EA3A

l00402E6E:
	balc	0040309C
	bc	00402E4C

l00402E72:
	addiupc	r4,0000EA42
	move.balc	r5,r17,004086C0
	beqzc	r16,00402E4C

l00402E7C:
	move	r5,r16
	addiupc	r4,0000EA4E
	bc	00402E6E

l00402E84:
	addiupc	r4,0000EA54
	balc	0040309C
	bnezc	r16,00402E92

l00402E8C:
	addiupc	r4,0000EA5C
	bc	00402E4A

l00402E92:
	addiupc	r4,0000EA62
	beqic	r16,00000001,00402E4A

l00402E9A:
	move	r5,r16
	addiupc	r4,0000EA70
	bc	00402E6E

l00402EA2:
	addiupc	r4,0000EAAA
	beqic	r16,00000002,00402E08

l00402EAA:
	addiupc	r4,0000EAB2
	move.balc	r5,r16,004086C0
	bc	00402E0A

l00402EB4:
	addiupc	r4,0000EABC
	bc	00402E4A

l00402EBA:
	addiupc	r4,0000EAC6
	bc	00402E4A

l00402EC0:
	addiupc	r4,0000EACC
	bc	00402E4A

l00402EC6:
	addiupc	r4,0000EAD2
	bc	00402E4A

;; pr_echo_reply: 00402ECC
pr_echo_reply proc
	save	00000010,ra,00000001
	lhu	r4,0006(r4)
	balc	004030A4
	restore	00000010,ra,00000001
	move	r5,r4
	addiupc	r4,0000D5E0
	bc	004086C0

;; write_stdout: 00402EE0
;;   Called from:
;;     00402FA4 (in ping6_receive_error_msg)
;;     00403044 (in ping6_receive_error_msg)
;;     00403220 (in ping6_parse_reply)
write_stdout proc
	save	00000010,ra,00000004
	move	r16,r0
	movep	r18,r17,r4,r5

l00402EE6:
	subu	r6,r17,r16
	addu	r5,r18,r16
	li	r4,00000001
	balc	0040B080
	addu	r16,r16,r4
	bltuc	r16,r17,00402EE6

l00402EF6:
	bltc	r4,r0,00402EE6

l00402EFA:
	restore.jrc	00000010,ra,00000004

;; ping6_receive_error_msg: 00402EFC
ping6_receive_error_msg proc
	save	00000280,ra,00000005
	move	r17,r4
	balc	004032A0
	addiu	r7,sp,FFFFF260
	move	r16,r7
	addiu	r7,sp,00000020
	lw	r19,0000(r4)
	addiu	r6,r0,00002040
	sw	r7,0018(sp)
	li	r7,00000008
	sw	r7,001C(sp)
	addiu	r7,sp,00000044
	sw	r7,0028(sp)
	li	r7,0000001C
	sw	r7,002C(sp)
	addiu	r7,sp,00000018
	sw	r7,0030(sp)
	li	r7,00000001
	sw	r7,0034(sp)
	addiu	r7,sp,00000060
	lw	r4,0000(r17)
	addiu	r5,sp,00000028
	sw	r7,0038(sp)
	addiu	r7,r0,00000200
	sw	r0,0040(sp)
	sw	r7,003C(sp)
	balc	00407670
	bltc	r4,r0,00402FF6

l00402F40:
	lw	r6,0DDC(r16)
	move	r7,r0
	bltiuc	r6,0000000C,00402F4E

l00402F4A:
	lw	r7,0DD8(r16)

l00402F4E:
	lw	r17,0038(sp)
	move	r18,r0
	addu	r5,r5,r6

l00402F54:
	beqzc	r7,00402F80

l00402F56:
	lw	r6,0004(r7)
	bneiuc	r6,00000029,00402F64

l00402F5C:
	lw	r6,0008(r7)
	bneiuc	r6,00000019,00402F64

l00402F62:
	addiu	r18,r7,0000000C

l00402F64:
	lw	r6,0000(r7)
	bltiuc	r6,0000000C,00402F7C

l00402F6A:
	addiu	r6,r6,00000003
	subu	r16,r5,r7
	ins	r6,r0,00000000,00000001
	addiu	r8,r6,0000000C
	addu	r7,r7,r6
	bltuc	r8,r16,00402F54

l00402F7C:
	move	r7,r0
	bc	00402F54

l00402F80:
	bnezc	r18,00402F86

l00402F82:
	balc	00404A00

l00402F86:
	lbu	r5,0004(r18)
	bneiuc	r5,00000001,00402FEC

l00402F8E:
	lwpc	r7,004544EC
	andi	r17,r7,00000010
	bnec	r0,r17,00403080

l00402F9C:
	bbeqzc	r7,00000000,00402FC6

l00402FA0:
	addiupc	r4,0000E1D4
	balc	00402EE0

l00402FA6:
	lwpc	r7,004544D0
	li	r16,00000001
	addiu	r7,r7,00000001
	swpc	r7,004544D0

l00402FB6:
	balc	004032A0
	sw	r19,0000(sp)
	bnezc	r17,00402FC0

l00402FBC:
	subu	r17,r0,r16

l00402FC0:
	move	r4,r17
	restore.jrc	00000280,ra,00000005

l00402FC6:
	lw	r4,0000(r18)
	lwpc	r7,00412EF0
	sw	r7,000C(sp)
	beqic	r4,0000001A,00402FE4

l00402FD4:
	balc	004049EA
	addiupc	r5,0000E1A0
	move	r6,r4

l00402FDE:
	lw	r17,000C(sp)
	balc	004032FE
	bc	00402FA6

l00402FE4:
	addiupc	r5,0000E1AC
	lw	r6,0008(r18)
	bc	00402FDE

l00402FEC:
	bneiuc	r5,00000003,00402FF6

l00402FF0:
	bgeic	r4,00000008,00402FFA

l00402FF4:
	move	r19,r0

l00402FF6:
	move	r16,r0
	bc	00403082

l00402FFA:
	li	r6,00000010
	addiupc	r5,0002F500
	addiu	r4,sp,0000004C
	balc	0040A100
	move	r16,r4
	bnezc	r4,00402FF4

l0040300A:
	lbu	r6,0020(sp)
	addiu	r7,r0,00000080
	bnec	r7,r6,00402FF4

l00403016:
	lhu	r5,0024(sp)
	move.balc	r4,r17,00402CD6
	beqzc	r4,00402FF4

l00403020:
	lwpc	r7,004544D0
	li	r17,00000001
	addiu	r7,r7,00000001
	swpc	r7,004544D0
	lwpc	r7,004544EC
	bbnezc	r7,00000004,00402FB6

l0040303A:
	and	r17,r17,r7
	beqzc	r17,00403048

l0040303E:
	li	r5,00000002
	addiupc	r4,0000E1A4
	balc	00402EE0
	bc	00402FB6

l00403048:
	balc	004033EA
	li	r5,0000001C
	addiu	r4,r18,00000010
	balc	00401288
	move	r16,r4
	lhu	r4,0026(sp)
	balc	004030A4
	movep	r5,r6,r16,r4
	addiupc	r4,0000E18C
	balc	0040309C
	lbu	r5,0006(r18)
	lw	r6,0008(r18)
	move	r16,r17
	lbu	r4,0005(r18)
	li	r17,00000001
	balc	00402DC8
	li	r4,0000000A
	balc	004033EE
	lwpc	r4,00412EF4
	balc	004033F2
	bc	00402FB6

l00403080:
	move	r16,r5

l00403082:
	move	r17,r0
	bc	00402FB6

;; niquery_nonce.isra.0: 00403086
;;   Called from:
;;     00403174 (in ping6_parse_reply)
;;     004034DE (in build_niquery)
niquery_nonce.isra.0 proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,0000E74A
	balc	00403098
	li	r4,00000003
	balc	004030A0

;; fn00403098: 00403098
;;   Called from:
;;     00402D0C (in niquery_option_help_handler)
;;     00402D1E (in niquery_option_subject_name_handler)
;;     00403092 (in niquery_nonce.isra.0)
;;     00403096 (in niquery_nonce.isra.0)
fn00403098 proc
	bc	004083F0

;; fn0040309C: 0040309C
;;   Called from:
;;     00402DF8 (in pr_icmph)
;;     00402E08 (in pr_icmph)
;;     00402E38 (in pr_icmph)
;;     00402E4A (in pr_icmph)
;;     00402E6E (in pr_icmph)
;;     00402E88 (in pr_icmph)
;;     00403060 (in ping6_receive_error_msg)
;;     00403238 (in ping6_parse_reply)
;;     0040329C (in ping6_parse_reply)
fn0040309C proc
	bc	004086C0

;; fn004030A0: 004030A0
;;   Called from:
;;     00402D10 (in niquery_option_help_handler)
;;     00402D22 (in niquery_option_subject_name_handler)
;;     00403096 (in niquery_nonce.isra.0)
;;     004033E8 (in if_name2index)
fn004030A0 proc
	bc	0040015A

;; fn004030A4: 004030A4
;;   Called from:
;;     00402ED0 (in pr_echo_reply)
;;     00403058 (in ping6_receive_error_msg)
;;     004030DA (in ping6_parse_reply)
;;     004031B6 (in ping6_parse_reply)
;;     00403230 (in ping6_parse_reply)
fn004030A4 proc
	bc	00407630

;; ping6_parse_reply: 004030A8
ping6_parse_reply proc
	save	00000050,r30,0000000A
	movep	r21,r19,r6,r7
	lw	r7,0008(r5)
	move	r20,r4
	lw	r23,0014(r5)
	move	r22,r8
	lw	r16,0000(r7)
	li	r7,FFFFFFFF
	sw	r7,001C(sp)
	bgeiuc	r23,0000000C,0040310A

l004030C0:
	bltic	r21,00000008,0040314E

l004030C4:
	lbu	r7,0000(r16)
	addiu	r6,r0,00000081
	bnec	r7,r6,0040316E

l004030CE:
	lhu	r5,0004(r16)
	move.balc	r4,r20,00402CD6
	beqc	r0,r4,00403158

l004030D8:
	lhu	r4,0006(r16)
	balc	004030A4
	li	r5,0000001C
	move	r17,r4
	lw	r4,001C(sp)
	move.balc	r4,r19,00401288
	addiupc	r7,FFFFFDE2
	move	r11,r4
	sw	r7,0000(sp)
	move	r10,r22
	move	r9,r0
	move	r8,r18
	movep	r6,r7,r21,r17
	li	r5,00000008
	move.balc	r4,r16,004023F6
	beqc	r0,r4,0040324A

l00403100:
	lwpc	r4,00412EF4
	balc	004033F2
	bc	004031FE

l0040310A:
	lw	r30,0010(r5)
	move	r17,r30

l00403110:
	beqzc	r17,004030C0

l00403112:
	lw	r7,0004(r17)
	lw	r18,0000(r17)
	bneiuc	r7,00000029,00403146

l0040311A:
	lw	r7,0008(r17)
	beqic	r7,00000008,00403124

l00403120:
	bneiuc	r7,00000034,00403146

l00403124:
	bltiuc	r18,00000010,00403146

l00403128:
	li	r6,00000004
	addiu	r5,r17,0000000C
	addiu	r4,sp,0000001C
	balc	004034E2

l00403130:
	addiu	r18,r18,00000003
	addu	r7,r30,r23
	ins	r18,r0,00000000,00000001
	subu	r7,r7,r17
	addiu	r6,r18,0000000C
	addu	r17,r17,r18
	bltuc	r6,r7,00403110

l00403144:
	bc	0040314A

l00403146:
	bgeiuc	r18,0000000C,00403130

l0040314A:
	move	r17,r0
	bc	00403110

l0040314E:
	lwpc	r7,004544EC
	bbnezc	r7,00000008,0040315E

l00403158:
	li	r17,00000001

l0040315A:
	move	r4,r17
	restore.jrc	00000050,r30,0000000A

l0040315E:
	move	r6,r21
	addiupc	r5,0000E86C
	lwpc	r4,00412EF0
	balc	004032FE
	bc	00403158

l0040316E:
	addiu	r6,r0,0000008C
	bnec	r6,r7,00403176

l00403174:
	balc	00403086

l00403176:
	bltic	r21,00000038,00403158

l0040317A:
	li	r6,00000010
	addiupc	r5,0002F380
	addiu	r4,r16,00000020
	balc	0040A100
	bnezc	r4,00403158

l0040318A:
	lbu	r7,000E(r16)
	addiu	r18,r16,00000030
	bneiuc	r7,0000002C,0040319E

l00403196:
	lbu	r7,0030(r16)
	addiu	r18,r16,00000038

l0040319E:
	bneiuc	r7,0000003A,0040327A

l004031A2:
	lbu	r6,0000(r18)
	addiu	r7,r0,00000080
	bnec	r7,r6,00403158

l004031AC:
	lhu	r5,0004(r18)
	move.balc	r4,r20,00402CD6
	beqzc	r4,00403158

l004031B4:
	lhu	r4,0006(r18)
	balc	004030A4
	lwpc	r7,004544E4
	subu	r5,r7,r4
	andi	r6,r5,0000FFFF
	bbnezc	r5,0000000F,004031FA

l004031C6:
	lwpc	r5,00430078
	bltc	r6,r5,004031D8

l004031D0:
	addiu	r6,r6,00000001
	swpc	r6,00430078

l004031D8:
	aluipc	r6,00000403
	lhu	r17,0518(r6)
	subu	r5,r4,r17
	seh	r5,r5
	bltc	r0,r5,004031F6

l004031EA:
	andi	r7,r7,0000FFFF
	addiu	r5,r0,00007FFF
	subu	r7,r7,r17
	bgec	r5,r7,004031FA

l004031F6:
	sh	r4,0518(r6)

l004031FA:
	lw	r17,0002(r20)
	beqzc	r17,00403202

l004031FE:
	move	r17,r0
	bc	0040315A

l00403202:
	lwpc	r7,004544D0
	addiu	r7,r7,00000001
	swpc	r7,004544D0
	lwpc	r7,004544EC
	bbeqzc	r7,00000000,00403224

l0040321A:
	li	r5,00000002
	addiupc	r4,0000DFC8
	balc	00402EE0
	bc	0040315A

l00403224:
	balc	004033EA
	li	r5,0000001C
	move.balc	r4,r19,00401288
	move	r17,r4
	lhu	r4,0006(r18)
	balc	004030A4
	movep	r5,r6,r17,r4
	addiupc	r4,0000DFF8
	balc	0040309C

l0040323A:
	lw	r4,0004(r16)
	lbu	r17,0000(r16)
	lbu	r18,0001(r16)
	balc	00407620
	movep	r5,r6,r18,r4
	move.balc	r4,r17,00402DC8

l0040324A:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000D,0040326A

l00403254:
	li	r4,00000007
	balc	004033EE
	lwpc	r7,004544EC
	bbeqzc	r7,00000000,00403274

l00403262:
	lwpc	r4,00412EF4
	balc	004033F2

l0040326A:
	lwpc	r7,004544EC
	bbnezc	r7,00000000,004031FE

l00403274:
	li	r4,0000000A
	balc	004033EE
	bc	00403100

l0040327A:
	lwpc	r7,004544EC
	bbeqzc	r7,00000008,00403158

l00403284:
	lwpc	r7,00454514
	bnec	r0,r7,00403158

l0040328E:
	balc	004033EA
	li	r5,0000001C
	move.balc	r4,r19,00401288
	move	r5,r4
	addiupc	r4,0000DFD0
	balc	0040309C
	bc	0040323A

;; fn004032A0: 004032A0
;;   Called from:
;;     00402F02 (in ping6_receive_error_msg)
;;     00402FB6 (in ping6_receive_error_msg)
;;     0040344E (in hextoui)
;;     00403462 (in hextoui)
;;     0040346A (in hextoui)
fn004032A0 proc
	bc	004049B0

;; ping6_install_filter: 004032A4
ping6_install_filter proc
	save	00000020,ra,00000003
	lwpc	r7,0045454C
	move	r17,r4
	bnezc	r7,004032FC

l004032B0:
	lwpc	r7,004544C4
	li	r16,00000001
	sw	r7,000C(sp)
	swpc	r16,0045454C
	lhu	r4,000C(sp)
	balc	0040359C
	addiupc	r7,0002CDCE
	sb	r0,000A(r7)
	li	r6,00000015
	sb	r16,000B(r7)
	addiu	r8,r0,00000008
	sw	r4,004C(sp)
	addiu	r5,r0,0000FFFF
	lw	r4,0000(r17)
	sh	r6,0008(r7)
	addiupc	r7,0002CDA8
	li	r6,0000001A
	balc	00407D60
	beqzc	r4,004032FC

l004032F0:
	addiupc	r4,0000D1F0
	restore	00000020,ra,00000003
	bc	00408630

l004032FC:
	restore.jrc	00000020,ra,00000003

;; fn004032FE: 004032FE
;;   Called from:
;;     00402FE0 (in ping6_receive_error_msg)
;;     0040316A (in ping6_parse_reply)
;;     00403376 (in niquery_option_subject_addr_handler)
;;     004033E4 (in if_name2index)
;;     00403594 (in ping6_usage)
fn004032FE proc
	bc	00408340

;; niquery_option_subject_addr_handler: 00403302
niquery_option_subject_addr_handler proc
	save	00000050,ra,00000006
	li	r6,00000020
	movep	r16,r19,r4,r5
	move	r5,r0
	addiu	r4,sp,00000010
	balc	0040A690
	li	r4,00000014
	mul	r16,r16,r4
	li	r7,00000002
	addiupc	r4,0002CDB6
	sw	r7,0010(sp)
	lwpc	r6,00430210
	li	r7,00000001
	sw	r7,0018(sp)
	addu	r4,r4,r16
	lw	r7,000C(r4)
	bltc	r6,r0,00403338

l0040332E:
	lwpc	r5,004314D4
	bnec	r0,r5,004033BA

l00403338:
	swpc	r7,00430210
	beqzc	r7,00403348

l00403340:
	beqic	r7,00000002,0040337A

l00403344:
	li	r16,FFFFFFFF
	bc	00403356

l00403348:
	li	r7,00000010
	li	r16,00000008
	swpc	r7,004314D0
	li	r7,0000000A
	sw	r7,0014(sp)

l00403356:
	addiu	r7,sp,0000000C
	addiu	r6,sp,00000010
	movep	r4,r5,r19,r0
	balc	00405E20
	lw	r4,000C(sp)
	move	r17,r4
	beqzc	r4,004033A6

l00403366:
	balc	00405E00
	addiupc	r5,0000E69E
	movep	r6,r7,r19,r4
	lwpc	r4,00412EF0
	balc	004032FE
	bc	004033C6

l0040337A:
	li	r16,00000004
	sw	r7,0014(sp)
	swpc	r16,004314D0
	bc	00403356

l00403386:
	lw	r5,0014(r18)
	move	r6,r20
	addu	r5,r5,r16
	balc	004034E2
	lwpc	r4,004314D4
	balc	00404F2E
	swpc	r19,004314D4

l0040339E:
	lw	r17,000C(sp)
	balc	00405DF0
	bc	004033C8

l004033A6:
	beqzc	r18,0040339E

l004033A8:
	lwpc	r20,004314D0
	move.balc	r4,r20,00405292
	move	r19,r4
	bnezc	r4,00403386

l004033B6:
	lw	r18,001C(r18)
	bc	004033A6

l004033BA:
	beqc	r6,r7,00403338

l004033BE:
	addiupc	r4,0000E632
	balc	00408780

l004033C6:
	li	r17,FFFFFFFF

l004033C8:
	move	r4,r17
	restore.jrc	00000050,ra,00000006

;; if_name2index: 004033CC
;;   Called from:
;;     0040370C (in ping6_run)
;;     0040385E (in ping6_run)
;;     00403904 (in ping6_run)
if_name2index proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	00406760
	beqzc	r4,004033D8

l004033D6:
	restore.jrc	00000010,ra,00000002

l004033D8:
	move	r6,r16
	addiupc	r5,0000D14A
	lwpc	r4,00412EF0
	balc	004032FE
	li	r4,00000002
	balc	004030A0

;; fn004033EA: 004033EA
;;   Called from:
;;     00403048 (in ping6_receive_error_msg)
;;     00403224 (in ping6_parse_reply)
;;     0040328E (in ping6_parse_reply)
fn004033EA proc
	bc	00401E5C

;; fn004033EE: 004033EE
;;   Called from:
;;     00403074 (in ping6_receive_error_msg)
;;     00403256 (in ping6_parse_reply)
;;     00403276 (in ping6_parse_reply)
fn004033EE proc
	bc	00408770

;; fn004033F2: 004033F2
;;   Called from:
;;     0040307C (in ping6_receive_error_msg)
;;     00403106 (in ping6_parse_reply)
;;     00403268 (in ping6_parse_reply)
fn004033F2 proc
	bc	004081A0

;; niquery_option_handler: 004033F6
niquery_option_handler proc
	save	00000020,ra,00000006
	li	r17,FFFFFFFF
	move	r20,r4
	move	r19,r0
	addiupc	r16,0002CCCE
	bc	00403416

l00403404:
	bneiuc	r7,0000003D,00403412

l00403408:
	addiu	r5,r18,00000001
	lw	r7,0010(r16)
	addu	r5,r5,r20
	bc	00403432

l00403412:
	addiu	r19,r19,00000001
	addiu	r16,r16,00000014

l00403416:
	lw	r4,0000(r16)
	beqzc	r4,0040343C

l0040341A:
	lw	r18,0004(r16)
	movep	r5,r6,r20,r18
	balc	0040A8E0
	bnezc	r4,00403412

l00403424:
	lw	r6,0008(r16)
	lbux	r7,r18(r20)
	bnezc	r6,00403404

l0040342C:
	bnezc	r7,00403412

l0040342E:
	lw	r7,0010(r16)
	move	r5,r0

l00403432:
	move	r4,r19
	jalrc	ra,r7
	move	r17,r4
	bltc	r4,r0,00403412

l0040343C:
	lw	r7,0000(r16)
	bnezc	r7,00403446

l00403440:
	movep	r4,r5,r0,r0
	balc	00402D00

l00403446:
	move	r4,r17
	restore.jrc	00000020,ra,00000006

;; hextoui: 0040344A
hextoui proc
	save	00000020,ra,00000002
	move	r16,r4
	balc	004032A0
	li	r6,00000010
	sw	r0,0000(sp)
	addiu	r5,sp,0000000C
	move.balc	r4,r16,0040A022
	lw	r17,000C(sp)
	move	r16,r4
	lbu	r7,0000(r7)
	beqzc	r7,00403470

l00403462:
	balc	004032A0
	li	r16,FFFFFFFF
	lw	r7,0000(r4)
	bnezc	r7,00403470

l0040346A:
	balc	004032A0
	li	r7,00000016
	sw	r7,0000(sp)

l00403470:
	move	r4,r16
	restore.jrc	00000020,ra,00000002

;; build_echo: 00403474
;;   Called from:
;;     0040351C (in ping6_send_probe)
build_echo proc
	save	00000010,ra,00000002
	addiu	r7,r0,FFFFFF80
	sb	r0,0001(r4)
	sb	r7,0000(r4)
	move	r16,r4
	sh	r0,0002(r4)
	lwpc	r4,004544E4
	addiu	r4,r4,00000001
	andi	r4,r4,0000FFFF
	balc	0040359C
	lwpc	r7,004544C4
	sh	r7,0004(r16)
	lwpc	r7,00454508
	sh	r4,0006(r16)
	beqzc	r7,004034A8

l004034A0:
	move	r5,r0
	addiu	r4,r16,00000008
	balc	0040AF40

l004034A8:
	lwpc	r4,00430074
	addiu	r4,r4,00000008
	restore.jrc	00000010,ra,00000002

;; build_niquery: 004034B2
;;   Called from:
;;     0040354C (in ping6_send_probe)
build_niquery proc
	save	00000020,ra,00000002
	addiu	r7,r0,FFFFFF8B
	move	r16,r4
	sb	r7,0000(r4)
	sh	r0,0002(r4)
	lwpc	r4,004544E4
	addiu	r4,r4,00000001
	swpc	r0,00430074
	andi	r4,r4,0000FFFF
	balc	0040359C
	sb	r4,0008(r16)
	sh	r4,000E(sp)
	srl	r4,r4,00000008
	sb	r4,0009(r16)
	balc	00403086

;; fn004034E2: 004034E2
;;   Called from:
;;     0040312E (in ping6_parse_reply)
;;     0040338C (in niquery_option_subject_addr_handler)
;;     004034DE (in build_niquery)
;;     004036A2 (in ping6_run)
fn004034E2 proc
	bc	0040A130

;; ping6_send_probe: 004034E6
ping6_send_probe proc
	save	00000050,ra,00000005
	lwpc	r7,004544E4
	movep	r18,r17,r4,r5
	addiu	r7,r7,00000001
	move	r5,r6
	andi	r7,r7,0000FFFF
	addiupc	r6,0004EE46
	srl	r4,r7,00000005
	li	r19,00000001
	lsa	r4,r4,r6,00000002
	sllv	r7,r19,r7
	nor	r7,r0,r7
	lw	r6,0000(r4)
	and	r7,r7,r6
	sw	r7,0000(sp)
	lwpc	r7,00430214
	move	r4,r17
	bgec	r7,r0,0040354C

l0040351C:
	balc	00403474
	lwpc	r7,00454554
	move	r16,r4
	bnezc	r7,0040354E

l00403528:
	lw	r4,0000(r18)
	addiu	r9,r0,0000001C
	addiupc	r8,0002EFC6
	movep	r5,r6,r17,r16
	lwpc	r7,004314C4
	balc	00407D40

l0040353E:
	xor	r16,r16,r4
	swpc	r0,004314C4
	movz	r4,r0,r16

l0040354A:
	restore.jrc	00000050,ra,00000005

l0040354C:
	balc	004034B2

l0040354E:
	addiupc	r6,0002EFA6
	lw	r4,0000(r18)
	sw	r6,0014(sp)
	li	r6,0000001C
	sw	r6,0018(sp)
	addiu	r6,sp,0000000C
	sw	r6,001C(sp)
	addiupc	r6,0002DF7A
	sw	r6,0024(sp)
	addiu	r5,sp,00000014
	lwpc	r6,004314C4
	sw	r17,000C(sp)
	sw	r16,0010(sp)
	sw	r19,0020(sp)
	sw	r7,0028(sp)
	sw	r0,002C(sp)
	balc	00407D20
	bc	0040353E

;; ping6_usage: 0040357C
;;   Called from:
;;     0040095E (in usage)
;;     004035F4 (in ping6_run)
ping6_usage proc
	save	00000010,ra,00000001
	addiupc	r7,0000E4A2
	addiupc	r6,0000E496
	movz	r6,r7,r4

l0040358A:
	addiupc	r5,0000E49E
	lwpc	r4,00412EF0
	balc	004032FE
	li	r4,00000002
	balc	0040015A

;; fn0040359C: 0040359C
;;   Called from:
;;     004032C4 (in ping6_install_filter)
;;     0040348C (in build_echo)
;;     004034CE (in build_niquery)
;;     00403656 (in ping6_run)
;;     004037A0 (in ping6_run)
;;     0040386C (in ping6_run)
;;     00403874 (in ping6_run)
fn0040359C proc
	bc	00406700

;; ping6_run: 004035A0
;;   Called from:
;;     0040080A (in main)
ping6_run proc
	save	00000070,r30,0000000A
	addiu	r30,sp,FFFFF070
	movep	r19,r16,r6,r7
	lwpc	r7,00430214
	movep	r17,r18,r4,r5
	sw	r0,0FA4(r30)
	bltc	r7,r0,004035E0

l004035B8:
	move	r5,r0
	addiupc	r4,00050EB2
	balc	0040AF40
	balc	0040AFB0
	addiupc	r7,00050EA6
	sw	r4,0048(sp)
	lwpc	r7,00430210
	bltc	r7,r0,00403C7E

l004035D6:
	lwpc	r7,004314D4
	beqc	r0,r7,00403C7E

l004035E0:
	bltic	r17,00000002,004035F6

l004035E4:
	lwpc	r5,00412EF0
	addiupc	r4,0000E576
	balc	004083F0

l004035F2:
	move	r4,r0
	balc	0040357C

l004035F6:
	bneiuc	r17,00000001,0040362C

l004035FA:
	lw	r17,0000(r18)

l004035FC:
	bnezc	r19,00403648

l004035FE:
	addiu	r7,r30,00000FA4
	addiupc	r6,0000ECDE
	movep	r4,r5,r17,r0
	balc	00405E20
	lw	r19,0FA4(r30)
	beqzc	r4,00403648

l00403612:
	balc	00405E00
	addiupc	r5,0000E3F2
	movep	r6,r7,r17,r4
	lwpc	r4,00412EF0
	balc	00408340

l00403626:
	li	r4,00000002
	balc	0040015A

l0040362C:
	lwpc	r7,00430214
	bgec	r7,r0,00403640

l00403636:
	lwpc	r7,00430210
	bneiuc	r7,00000001,004035F2

l00403640:
	lwpc	r17,004544D4
	bc	004035FC

l00403648:
	addiupc	r18,0002EEAC
	lw	r5,0014(r19)
	li	r6,0000001C
	move.balc	r4,r18,0040A130
	li	r4,0000003A
	balc	0040359C
	sh	r4,0002(r18)
	lw	r4,0FA4(r30)
	beqzc	r4,00403664

l00403660:
	balc	00405DF0

l00403664:
	move.balc	r4,r17,0040A890
	li	r5,0000003A
	move	r6,r4
	move.balc	r4,r17,0040A050
	beqzc	r4,00403682

l00403672:
	lwpc	r7,004544EC
	ori	r7,r7,00000004
	swpc	r7,004544EC

l00403682:
	addiupc	r19,0002EE56
	lw	r7,0008(r19)
	move	r20,r19
	bnezc	r7,004036CC

l0040368C:
	lw	r7,000C(r19)
	bnezc	r7,004036CC

l00403690:
	lw	r7,0010(r19)
	bnezc	r7,004036CC

l00403694:
	lw	r7,0014(r19)
	bnezc	r7,004036CC

l00403698:
	li	r6,00000010
	addiupc	r5,0002EE62
	addiupc	r4,0002EE42
	balc	004034E2
	lw	r7,0018(r18)
	lwpc	r6,00454550
	sw	r7,0058(sp)
	beqzc	r7,004036C4

l004036B0:
	beqzc	r6,004036C6

l004036B2:
	beqc	r6,r7,004036CC

l004036B4:
	lwpc	r5,00412EF0
	addiupc	r4,0000E4DA

l004036BE:
	balc	004083F0
	bc	00403626

l004036C4:
	bnezc	r6,004036CC

l004036C6:
	swpc	r7,00454550

l004036CC:
	addiupc	r7,0002CB4C
	lw	r6,0008(r7)
	swpc	r17,004544C0
	move	r17,r7
	bnec	r0,r6,004038E0

l004036DE:
	lw	r6,000C(r7)
	bnec	r0,r6,004038E0

l004036E4:
	lw	r6,0010(r7)
	bnec	r0,r6,004038E0

l004036EA:
	lw	r7,0014(r7)
	bnec	r0,r7,004038E0

l004036F0:
	li	r5,00000001
	li	r4,0000000A
	balc	00407D80
	move	r19,r4
	addiupc	r4,0000CE22
	bltc	r19,r0,00403790

l00403702:
	lwpc	r4,004544B0
	beqc	r0,r4,0040379C

l0040370C:
	balc	004033CC
	lbu	r7,0008(r20)
	addiu	r6,r0,000000FE
	sw	r0,0FB0(r30)
	sw	r0,0FB4(r30)
	sw	r0,0FB8(r30)
	sw	r0,0FBC(r30)
	sw	r4,0FC0(r30)
	bnec	r6,r7,00403740

l0040372C:
	lbu	r7,0009(r20)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r6,r7,00403750

l0040373A:
	sw	r4,0018(r20)
	bc	00403750

l00403740:
	addiu	r6,r0,000000FF
	bnec	r6,r7,00403750

l00403746:
	lbu	r7,0009(r20)
	andi	r7,r7,0000000F
	beqic	r7,00000002,0040373A

l00403750:
	li	r4,00000001
	balc	00401CE2
	addiu	r7,r30,00000FB0
	addiu	r8,r0,00000014
	li	r6,00000032
	li	r5,00000029
	move.balc	r4,r19,00407D60
	li	r7,FFFFFFFF
	move	r21,r4
	bnec	r7,r4,00403796

l0040376E:
	lwpc	r22,004544B0
	move.balc	r4,r22,0040A890
	move	r7,r22
	addiu	r8,r4,00000001
	li	r6,00000019
	addiu	r5,r0,0000FFFF
	move.balc	r4,r19,00407D60
	bnec	r4,r21,00403796

l0040378C:
	addiupc	r4,0000E42C

l00403790:
	balc	00408630
	bc	00403626

l00403796:
	move	r4,r0
	balc	00401CE2

l0040379C:
	addiu	r4,r0,00000401
	balc	0040359C
	li	r6,0000001C
	sh	r4,0002(r20)
	addiupc	r5,0002ED30
	addiu	r20,r0,FFFFFFFF
	move.balc	r4,r19,00405DD0
	bnec	r4,r20,004037BE

l004037B8:
	addiupc	r4,0000CE3C
	bc	00403790

l004037BE:
	li	r7,0000001C
	addiu	r6,r30,00000FAC
	addiupc	r5,0002CA54
	sw	r7,0FAC(r30)
	move.balc	r4,r19,004066B0
	bnec	r4,r20,004037DA

l004037D4:
	addiupc	r4,0000CE28
	bc	00403790

l004037DA:
	sh	r0,0002(r17)
	move.balc	r4,r19,0040AF72
	lwpc	r7,004544B0
	beqzc	r7,0040382A

l004037E8:
	addiu	r4,r30,00000FB0
	balc	004062A2
	beqzc	r4,004037F8

l004037F2:
	addiupc	r4,0000E3E2
	bc	00403790

l004037F8:
	lw	r23,0010(r17)
	lwpc	r20,004544B0
	lw	r8,0014(r17)
	lwm	r21,0008(r17),00000002
	lw	r19,0FB0(r30)

l0040380E:
	bnec	r0,r19,0040389E

l00403812:
	move	r6,r20
	addiupc	r5,0000E590
	lwpc	r4,00412EF0
	balc	00408340

l00403822:
	lw	r4,0FB0(r30)
	balc	00406292

l0040382A:
	lwpc	r19,004544B0
	beqzc	r19,00403864

l00403832:
	lwpc	r7,00454554
	li	r6,00000014
	addiupc	r17,0002DC9E
	move	r5,r0
	addu	r17,r7,r17
	addiu	r7,r7,00000020
	swpc	r7,00454554
	li	r7,00000020
	sw	r7,0040(sp)
	li	r7,00000029
	sw	r7,0044(sp)
	li	r7,00000032
	sw	r7,0048(sp)
	addiu	r4,r17,0000000C
	balc	0040A690
	move.balc	r4,r19,004033CC
	sw	r4,005C(sp)

l00403864:
	lhu	r17,0008(r18)
	addiu	r4,r0,0000FF00
	balc	0040359C
	and	r17,r17,r4
	addiu	r4,r0,0000FF00
	balc	0040359C
	bnec	r4,r17,0040394C

l0040387A:
	lwpc	r7,00454514
	beqc	r0,r7,0040393A

l00403884:
	lwpc	r7,0043008C
	addiu	r6,r0,000003E7
	bltc	r6,r7,00403920

l00403892:
	lwpc	r5,00412EF0
	addiupc	r4,0000E348
	bc	004036BE

l0040389E:
	lw	r17,000C(r19)
	beqzc	r17,004038DC

l004038A2:
	lhu	r7,0000(r17)
	bneiuc	r7,0000000A,004038DC

l004038A8:
	lw	r4,0004(r19)
	li	r6,00000003
	sw	r8,0F9C(r30)
	move.balc	r5,r20,0040A8E0
	lw	r8,0F9C(r30)
	bnezc	r4,004038DC

l004038BA:
	lw	r7,0008(r17)
	lw	r6,000C(r17)
	subu	r7,r7,r21
	subu	r6,r6,r22
	or	r7,r7,r6
	lw	r6,0010(r17)
	subu	r6,r6,r23
	or	r7,r7,r6
	lw	r6,0014(r17)
	subu	r6,r6,r8
	or	r7,r7,r6
	beqc	r0,r7,00403822

l004038DC:
	lw	r19,0000(r19)
	bc	0040380E

l004038E0:
	lwpc	r4,004544B0
	beqc	r0,r4,0040382A

l004038EA:
	lbu	r7,0008(r17)
	addiu	r6,r0,000000FE
	bnec	r6,r7,0040390C

l004038F4:
	lbu	r7,0009(r17)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r7,r6,0040382A

l00403904:
	balc	004033CC
	sw	r4,0058(sp)
	bc	0040382A

l0040390C:
	addiu	r6,r0,000000FF
	bnec	r7,r6,0040382A

l00403914:
	lbu	r7,0009(r17)
	andi	r7,r7,0000000F
	bneiuc	r7,00000002,0040382A

l0040391E:
	bc	00403904

l00403920:
	lwpc	r7,00430218
	bltc	r7,r0,0040393A

l0040392A:
	beqic	r7,00000002,0040393A

l0040392E:
	lwpc	r5,00412EF0
	addiupc	r4,0000E2DC
	bc	004036BE

l0040393A:
	lwpc	r7,00430218
	bgec	r7,r0,0040394C

l00403944:
	li	r7,00000002
	swpc	r7,00430218

l0040394C:
	lwpc	r7,00430218
	bltc	r7,r0,00403970

l00403956:
	lw	r4,0000(r16)
	addiupc	r7,0002C8BC
	addiu	r8,r0,00000004
	li	r6,00000017
	li	r5,00000029
	balc	00403C9A
	li	r7,FFFFFFFF
	bnec	r4,r7,00403970

l0040396A:
	addiupc	r4,0000E2D2
	bc	00403790

l00403970:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000F,00403990

l0040397A:
	lw	r4,0000(r16)
	li	r6,0000001C
	addiupc	r5,0002C89A
	balc	00405DB0
	li	r7,FFFFFFFF
	bnec	r4,r7,00403990

l0040398A:
	addiupc	r4,0000E2CA
	bc	00403790

l00403990:
	lwpc	r17,00430074
	bltiuc	r17,00000008,004039AC

l0040399A:
	lwpc	r7,00430214
	bgec	r7,r0,004039AC

l004039A4:
	li	r7,00000001
	swpc	r7,00454508

l004039AC:
	addiu	r17,r17,00001038
	move.balc	r4,r17,00405292
	move	r19,r4
	bnezc	r4,004039C4

l004039B8:
	lwpc	r5,00412EF0
	addiupc	r4,0000CEB2
	bc	004036BE

l004039C4:
	li	r7,00000001
	lw	r4,0000(r16)
	sw	r7,0008(sp)
	addiu	r8,r0,00000004
	sw	r7,0FA8(r30)
	li	r6,00000019
	li	r5,00000029
	addiu	r7,r30,00000FA8
	balc	00403C9A
	beqzc	r4,004039EE

l004039DE:
	lwpc	r5,00412EF0
	addiupc	r4,0000CD28
	balc	004083F0
	sw	r0,0008(sp)

l004039EE:
	lwpc	r7,00430074
	addiu	r6,r0,00000200
	addiu	r4,r7,00000207
	div	r5,r4,r6
	addiu	r6,r0,00000118
	addiu	r7,r7,00000008
	mul	r5,r5,r6
	addu	r5,r5,r7
	sw	r5,0FA8(r30)
	move.balc	r4,r16,0040212E
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00403A96

l00403A18:
	li	r7,00000002
	lw	r4,0000(r16)
	sw	r7,0FAC(r30)
	addiu	r8,r0,00000004
	li	r6,00000007
	addiu	r5,r0,000000FF
	addiu	r7,r30,00000FAC
	balc	00403C9A
	bgec	r4,r0,00403A42

l00403A34:
	lwpc	r5,00412EF0
	addiupc	r4,0000E232
	balc	004083F0

l00403A42:
	li	r6,00000020
	addiu	r5,r0,000000FF
	addiu	r4,r30,00000FB0
	balc	0040A690
	lw	r7,0008(r16)
	bnezc	r7,00403A60

l00403A54:
	lw	r7,0FB0(r30)
	ins	r7,r0,00000001,00000001
	sw	r7,0FB0(r30)

l00403A60:
	lwpc	r6,00430214
	lw	r7,0FC0(r30)
	bltc	r6,r0,00403A90

l00403A6E:
	ins	r7,r0,0000000C,00000001

l00403A72:
	lw	r4,0000(r16)
	addiu	r8,r0,00000020
	sw	r7,0FC0(r30)
	li	r6,00000001
	li	r5,0000003A
	addiu	r7,r30,00000FB0
	balc	00403C9A
	bgec	r4,r0,00403A96

l00403A8A:
	addiupc	r4,0000E216
	bc	00403790

l00403A90:
	ins	r7,r0,00000001,00000001
	bc	00403A72

l00403A96:
	lwpc	r7,004544EC
	bbeqzc	r7,00000010,00403ABE

l00403AA0:
	lw	r4,0000(r16)
	addiu	r7,r30,00000FAC
	addiu	r8,r0,00000004
	li	r6,00000013
	li	r5,00000029
	sw	r0,0FAC(r30)
	balc	00403C9A
	li	r7,FFFFFFFF
	bnec	r4,r7,00403ABE

l00403AB8:
	addiupc	r4,0000E204
	bc	00403790

l00403ABE:
	lwpc	r7,004544EC
	bbeqzc	r7,00000011,00403B00

l00403AC8:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiupc	r7,00050A3E
	li	r6,00000012
	li	r5,00000029
	addiu	r20,r0,FFFFFFFF
	balc	00403C9A
	bnec	r4,r20,00403AE6

l00403AE0:
	addiupc	r4,0000E200
	bc	00403790

l00403AE6:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiupc	r7,00050A20
	li	r6,00000010
	li	r5,00000029
	balc	00403C9A
	bnec	r4,r20,00403B00

l00403AFA:
	addiupc	r4,0000E206
	bc	00403790

l00403B00:
	li	r7,00000001
	lw	r4,0000(r16)
	sw	r7,0FAC(r30)
	addiu	r8,r0,00000004
	addiu	r7,r30,00000FAC
	li	r6,00000033
	li	r5,00000029
	balc	00403C9A
	li	r7,FFFFFFFF
	move	r20,r4
	bnec	r4,r7,00403B36

l00403B1C:
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,r30,00000FAC
	li	r6,00000008
	li	r5,00000029
	balc	00403C9A
	bnec	r4,r20,00403B36

l00403B30:
	addiupc	r4,0000E1EC
	bc	00403790

l00403B36:
	lwpc	r7,004544EC
	bbeqzc	r7,0000000A,00403B5A

l00403B40:
	lw	r4,0000(r16)
	addiupc	r7,000509BE
	addiu	r8,r0,00000004
	li	r6,00000043
	li	r5,00000029
	balc	00403C9A
	li	r7,FFFFFFFF
	bnec	r4,r7,00403B5A

l00403B54:
	addiupc	r4,0000E1E0
	bc	00403790

l00403B5A:
	lwpc	r7,004544EC
	bbeqzc	r7,00000009,00403BF2

l00403B64:
	lwpc	r7,00454554
	move	r20,sp
	addiu	r7,r7,0000002F
	li	r6,00000020
	ins	r7,r0,00000000,00000001
	move	r5,r0
	subu	sp,sp,r7
	addiu	r21,r0,FFFFFFFF
	move	r4,sp
	balc	0040A690
	lwpc	r4,004544BC
	ext	r4,r4,00000000,00000014
	balc	004066F0
	li	r7,00000001
	li	r6,00000010
	addiupc	r5,0002E964
	sw	r4,0010(sp)
	move	r4,sp
	sb	r7,0015(sp)
	sh	r7,0016(sp)
	sb	r0,0014(sp)
	balc	0040A130
	addiu	r8,r0,00000020
	lw	r4,0000(r16)
	move	r7,sp
	move	r6,r8
	li	r5,00000029
	balc	00403C9A
	bnec	r4,r21,00403BCA

l00403BC2:
	addiupc	r4,0000E18A
	bc	00403790

l00403BCA:
	lw	r17,0010(sp)
	addiu	r8,r0,00000004
	lw	r4,0000(r16)
	li	r6,00000021
	swpc	r7,004544BC
	sw	r7,0004(sp)
	li	r5,00000029
	addiu	r7,r30,00000FAC
	balc	00403C9A
	bnec	r4,r21,00403BF0

l00403BE8:
	addiupc	r4,0000E178
	bc	00403790

l00403BF0:
	move	sp,r20

l00403BF2:
	li	r5,0000001C
	addiupc	r4,0002E900
	lwpc	r18,004544C0
	balc	00401288
	movep	r5,r6,r18,r4
	addiupc	r4,0000E170
	balc	00403C9E
	lwpc	r4,004544BC
	beqzc	r4,00403C1E

l00403C12:
	balc	00407620
	move	r5,r4
	addiupc	r4,0000E16C
	balc	00403C9E

l00403C1E:
	lwpc	r7,004544B0
	lwpc	r18,004544EC
	bnezc	r7,00403C30

l00403C2C:
	bbeqzc	r18,0000000F,00403C60

l00403C30:
	ori	r7,r18,00000004
	li	r5,0000001C
	addiupc	r4,0002C5E2
	swpc	r7,004544EC
	balc	00401288
	lwpc	r6,004544B0
	move	r5,r4
	addiupc	r7,0000C828
	movz	r6,r7,r6

l00403C54:
	addiupc	r4,0000CC44
	balc	00403C9E
	swpc	r18,004544EC

l00403C60:
	lwpc	r5,00430074
	addiupc	r4,0000E12E
	balc	00403C9E
	move.balc	r4,r16,004021B4
	balc	00401D02
	movep	r6,r7,r19,r17
	addiupc	r4,0002C5BE
	move.balc	r5,r16,00402A9E

l00403C7E:
	addiupc	r7,0002E87E
	swpc	r7,004314D4
	li	r7,00000010
	swpc	r7,004314D0
	swpc	r0,00430210
	bc	004035E0

;; fn00403C9A: 00403C9A
;;   Called from:
;;     00403964 (in ping6_run)
;;     004039DA (in ping6_run)
;;     00403A2E (in ping6_run)
;;     00403A84 (in ping6_run)
;;     00403AB2 (in ping6_run)
;;     00403ADA (in ping6_run)
;;     00403AF4 (in ping6_run)
;;     00403B14 (in ping6_run)
;;     00403B2A (in ping6_run)
;;     00403B4E (in ping6_run)
;;     00403BBC (in ping6_run)
;;     00403BE2 (in ping6_run)
fn00403C9A proc
	bc	00407D60

;; fn00403C9E: 00403C9E
;;   Called from:
;;     00403C08 (in ping6_run)
;;     00403C1C (in ping6_run)
;;     00403C58 (in ping6_run)
;;     00403C6A (in ping6_run)
fn00403C9E proc
	bc	004086C0
00403CA2       08 90 00 80 00 C0 00 80 00 C0 00 80 00 C0   ..............

;; __divdi3: 00403CB0
;;   Called from:
;;     00402A9A (in fn00402A9A)
__divdi3 proc
	move	r8,r6
	move	r12,r7
	move	r2,r0
	bgec	r5,r0,00403CCE

l00403CBA:
	subu	r4,r0,r4
	subu	r5,r0,r5
	sltu	r9,r0,r4
	addiu	r2,r0,FFFFFFFF
	subu	r5,r5,r9

l00403CCE:
	bgec	r7,r0,00403CE6

l00403CD2:
	subu	r8,r0,r6
	subu	r7,r0,r7
	sltu	r12,r0,r8
	nor	r2,r0,r2
	subu	r12,r7,r12

l00403CE6:
	move	r9,r8
	move	r11,r12
	movep	r10,r7,r4,r5
	bnec	r0,r12,00403E44

l00403CF0:
	bgeuc	r7,r8,00403D94

l00403CF4:
	clz	r6,r8
	beqzc	r6,00403D10

l00403CFA:
	subu	r5,r0,r6
	sllv	r7,r7,r6
	srlv	r5,r10,r5
	sllv	r9,r8,r6
	or	r7,r7,r5
	sllv	r10,r10,r6

l00403D10:
	ext	r8,r9,00000000,00000010
	srl	r4,r9,00000010
	divu	r3,r7,r4
	mul	r5,r8,r3
	modu	r6,r7,r4
	srl	r7,r10,00000010
	sll	r6,r6,00000010
	or	r6,r6,r7
	move	r7,r3
	bgeuc	r6,r5,00403D46

l00403D34:
	addu	r6,r6,r9
	addiu	r7,r7,FFFFFFFF
	bltuc	r6,r9,00403D46

l00403D3C:
	bgeuc	r6,r5,00403D46

l00403D40:
	addiu	r7,r3,FFFFFFFE
	addu	r6,r6,r9

l00403D46:
	subu	r6,r6,r5
	divu	r3,r6,r4
	mul	r8,r8,r3
	modu	r5,r6,r4
	ext	r10,r10,00000000,00000010
	sll	r6,r5,00000010
	or	r10,r6,r10
	move	r4,r3
	bgeuc	r10,r8,00403D76

l00403D66:
	addu	r10,r10,r9
	addiu	r4,r4,FFFFFFFF
	bltuc	r10,r9,00403D76

l00403D6E:
	bgeuc	r10,r8,00403D76

l00403D72:
	addiu	r4,r3,FFFFFFFE

l00403D76:
	sll	r7,r7,00000010
	or	r7,r7,r4

l00403D7C:
	move	r4,r7
	move	r5,r11
	beqc	r0,r2,00403D92

l00403D84:
	subu	r4,r0,r7
	subu	r5,r0,r11
	sltu	r7,r0,r4
	subu	r5,r5,r7

l00403D92:
	jrc	ra

l00403D94:
	bnec	r0,r8,00403D9E

l00403D98:
	li	r7,00000001
	divu	r9,r7,r12

l00403D9E:
	clz	r7,r9
	bnezc	r7,00403DAE

l00403DA4:
	subu	r7,r5,r9
	addiu	r11,r0,00000001
	bc	00403D10

l00403DAE:
	addiu	r8,r0,00000020
	sllv	r9,r9,r7
	subu	r8,r8,r7
	sllv	r10,r4,r7
	srlv	r11,r5,r8
	sllv	r5,r5,r7
	srlv	r8,r4,r8
	ext	r7,r9,00000000,00000010
	or	r8,r8,r5
	srl	r5,r9,00000010
	divu	r3,r11,r5
	mul	r4,r7,r3
	modu	r6,r11,r5
	srl	r11,r8,00000010
	sll	r6,r6,00000010
	or	r6,r6,r11
	move	r11,r3
	bgeuc	r6,r4,00403E06

l00403DF4:
	addu	r6,r6,r9
	addiu	r11,r11,FFFFFFFF
	bltuc	r6,r9,00403E06

l00403DFC:
	bgeuc	r6,r4,00403E06

l00403E00:
	addiu	r11,r3,FFFFFFFE
	addu	r6,r6,r9

l00403E06:
	subu	r4,r6,r4
	divu	r3,r4,r5
	mul	r7,r7,r3
	modu	r6,r4,r5
	ext	r8,r8,00000000,00000010
	sll	r6,r6,00000010
	or	r5,r6,r8
	move	r6,r3
	bgeuc	r5,r7,00403E38

l00403E26:
	addu	r5,r5,r9
	addiu	r6,r6,FFFFFFFF
	bltuc	r5,r9,00403E38

l00403E2E:
	bgeuc	r5,r7,00403E38

l00403E32:
	addiu	r6,r3,FFFFFFFE
	addu	r5,r5,r9

l00403E38:
	sll	r11,r11,00000010
	subu	r7,r5,r7
	or	r11,r11,r6
	bc	00403D10

l00403E44:
	bltuc	r5,r12,00403F24

l00403E48:
	clz	r13,r12
	bnec	r0,r13,00403E60

l00403E50:
	bltuc	r12,r5,00403F2A

l00403E54:
	sltu	r7,r4,r8
	xori	r7,r7,00000001

l00403E5C:
	move	r11,r0
	bc	00403D7C

l00403E60:
	addiu	r9,r0,00000020
	sllv	r12,r12,r13
	subu	r9,r9,r13
	srlv	r7,r8,r9
	srlv	r6,r5,r9
	or	r12,r7,r12
	sllv	r5,r5,r13
	srlv	r9,r4,r9
	ext	r10,r12,00000000,00000010
	or	r3,r9,r5
	srl	r7,r12,00000010
	modu	r11,r6,r7
	divu	r5,r6,r7
	mul	r6,r10,r5
	srl	r9,r3,00000010
	sll	r11,r11,00000010
	sllv	r8,r8,r13
	or	r11,r11,r9
	move	r9,r5
	bgeuc	r11,r6,00403EC4

l00403EAE:
	addu	r11,r11,r12
	addiu	r9,r9,FFFFFFFF
	bltuc	r11,r12,00403EC4

l00403EB8:
	bgeuc	r11,r6,00403EC4

l00403EBC:
	addiu	r9,r5,FFFFFFFE
	addu	r11,r11,r12

l00403EC4:
	subu	r11,r11,r6
	divu	r6,r11,r7
	mul	r10,r10,r6
	modu	r5,r11,r7
	ext	r3,r3,00000000,00000010
	sll	r5,r5,00000010
	or	r5,r5,r3
	move	r7,r6
	bgeuc	r5,r10,00403EFA

l00403EE4:
	addu	r5,r5,r12
	addiu	r7,r7,FFFFFFFF
	bltuc	r5,r12,00403EFA

l00403EEE:
	bgeuc	r5,r10,00403EFA

l00403EF2:
	addiu	r7,r6,FFFFFFFE
	addu	r5,r5,r12

l00403EFA:
	sll	r9,r9,00000010
	subu	r10,r5,r10
	or	r7,r9,r7
	mul	r6,r7,r8
	muhu	r8,r7,r8
	bltuc	r10,r8,00403F20

l00403F12:
	move	r11,r0
	bnec	r10,r8,00403D7C

l00403F18:
	sllv	r4,r4,r13
	bgeuc	r4,r6,00403D7C

l00403F20:
	addiu	r7,r7,FFFFFFFF
	bc	00403E5C

l00403F24:
	move	r11,r0
	move	r7,r0
	bc	00403D7C

l00403F2A:
	move	r11,r0
	li	r7,00000001
	bc	00403D7C

;; __moddi3: 00403F30
;;   Called from:
;;     004028D2 (in finish)
__moddi3 proc
	move	r2,r6
	move	r10,r7
	move	r11,r0
	bgec	r5,r0,00403F4E

l00403F3A:
	subu	r4,r0,r4
	subu	r5,r0,r5
	sltu	r8,r0,r4
	addiu	r11,r0,FFFFFFFF
	subu	r5,r5,r8

l00403F4E:
	bgec	r7,r0,00403F62

l00403F52:
	subu	r2,r0,r6
	subu	r7,r0,r7
	sltu	r10,r0,r2
	subu	r10,r7,r10

l00403F62:
	move	r9,r2
	movep	r7,r8,r5,r4
	bnec	r0,r10,004040BA

l00403F6A:
	bgeuc	r7,r2,00404000

l00403F6E:
	clz	r2,r2
	beqc	r0,r2,00403F8C

l00403F76:
	subu	r5,r0,r2
	sllv	r7,r7,r2
	srlv	r5,r8,r5
	sllv	r9,r9,r2
	or	r7,r7,r5
	sllv	r8,r8,r2

l00403F8C:
	ext	r4,r9,00000000,00000010
	srl	r10,r9,00000010
	divu	r5,r7,r10
	mul	r5,r5,r4
	modu	r6,r7,r10
	sll	r7,r6,00000010
	srl	r6,r8,00000010
	or	r7,r7,r6
	bgeuc	r7,r5,00403FB8

l00403FAC:
	addu	r7,r7,r9
	bltuc	r7,r9,00403FB8

l00403FB2:
	bgeuc	r7,r5,00403FB8

l00403FB6:
	addu	r7,r7,r9

l00403FB8:
	subu	r7,r7,r5

l00403FBA:
	divu	r5,r7,r10
	mul	r4,r4,r5
	modu	r6,r7,r10
	ext	r8,r8,00000000,00000010
	sll	r7,r6,00000010
	or	r8,r7,r8
	bgeuc	r8,r4,00403FE0

l00403FD4:
	addu	r8,r8,r9
	bltuc	r8,r9,00403FE0

l00403FDA:
	bgeuc	r8,r4,00403FE0

l00403FDE:
	addu	r8,r8,r9

l00403FE0:
	subu	r8,r8,r4
	move	r5,r0
	srlv	r4,r8,r2

l00403FEA:
	beqc	r0,r11,00403FFE

l00403FEE:
	subu	r8,r0,r4
	subu	r5,r0,r5
	sltu	r7,r0,r8
	move	r4,r8
	subu	r5,r5,r7

l00403FFE:
	jrc	ra

l00404000:
	bnec	r0,r2,0040400A

l00404004:
	li	r7,00000001
	divu	r9,r7,r10

l0040400A:
	clz	r2,r9
	bnec	r0,r2,00404046

l00404012:
	subu	r5,r5,r9

l00404016:
	ext	r4,r9,00000000,00000010
	srl	r10,r9,00000010
	divu	r6,r5,r10
	mul	r6,r6,r4
	modu	r7,r5,r10
	srl	r5,r8,00000010
	sll	r7,r7,00000010
	or	r7,r7,r5
	bgeuc	r7,r6,00404042

l00404036:
	addu	r7,r7,r9
	bltuc	r7,r9,00404042

l0040403C:
	bgeuc	r7,r6,00404042

l00404040:
	addu	r7,r7,r9

l00404042:
	subu	r7,r7,r6
	bc	00403FBA

l00404046:
	li	r6,00000020
	sllv	r9,r9,r2
	subu	r6,r6,r2
	srl	r3,r9,00000010
	srlv	r12,r5,r6
	srlv	r6,r4,r6
	sllv	r5,r5,r2
	divu	r10,r12,r3
	or	r7,r6,r5
	ext	r5,r9,00000000,00000010
	mul	r10,r10,r5
	sllv	r8,r4,r2
	modu	r6,r12,r3
	srl	r4,r7,00000010
	sll	r6,r6,00000010
	or	r6,r6,r4
	bgeuc	r6,r10,00404090

l00404084:
	addu	r6,r6,r9
	bltuc	r6,r9,00404090

l0040408A:
	bgeuc	r6,r10,00404090

l0040408E:
	addu	r6,r6,r9

l00404090:
	subu	r10,r6,r10
	divu	r4,r10,r3
	mul	r4,r4,r5
	modu	r6,r10,r3
	andi	r5,r7,0000FFFF
	sll	r6,r6,00000010
	or	r5,r5,r6
	bgeuc	r5,r4,004040B6

l004040AA:
	addu	r5,r5,r9
	bltuc	r5,r9,004040B6

l004040B0:
	bgeuc	r5,r4,004040B6

l004040B4:
	addu	r5,r5,r9

l004040B6:
	subu	r5,r5,r4
	bc	00404016

l004040BA:
	bltuc	r5,r10,00403FEA

l004040BE:
	clz	r3,r10
	bnec	r0,r3,004040E0

l004040C6:
	bltuc	r10,r5,004040CE

l004040CA:
	bltuc	r4,r2,004040DC

l004040CE:
	subu	r8,r4,r2
	subu	r5,r5,r10
	sltu	r7,r4,r8
	subu	r7,r5,r7

l004040DC:
	movep	r4,r5,r8,r7
	bc	00403FEA

l004040E0:
	addiu	r9,r0,00000020
	sllv	r10,r10,r3
	subu	r9,r9,r3
	sllv	r8,r2,r3
	srlv	r6,r2,r9
	srlv	r7,r4,r9
	or	r12,r6,r10
	srlv	r6,r5,r9
	sllv	r5,r5,r3
	srl	r13,r12,00000010
	or	r5,r5,r7
	ext	r7,r12,00000000,00000010
	modu	r2,r6,r13
	divu	r14,r6,r13
	mul	r6,r7,r14
	srl	r10,r5,00000010
	sll	r2,r2,00000010
	sllv	r4,r4,r3
	or	r2,r2,r10
	move	r10,r14
	bgeuc	r2,r6,00404146

l00404130:
	addu	r2,r2,r12
	addiu	r10,r10,FFFFFFFF
	bltuc	r2,r12,00404146

l0040413A:
	bgeuc	r2,r6,00404146

l0040413E:
	addiu	r10,r14,FFFFFFFE
	addu	r2,r2,r12

l00404146:
	subu	r2,r2,r6
	modu	r6,r2,r13
	divu	r14,r2,r13
	mul	r2,r7,r14
	andi	r5,r5,0000FFFF
	sll	r6,r6,00000010
	or	r7,r6,r5
	move	r5,r14
	bgeuc	r7,r2,0040417C

l00404166:
	addu	r7,r7,r12
	addiu	r5,r5,FFFFFFFF
	bltuc	r7,r12,0040417C

l00404170:
	bgeuc	r7,r2,0040417C

l00404174:
	addiu	r5,r14,FFFFFFFE
	addu	r7,r7,r12

l0040417C:
	sll	r6,r10,00000010
	subu	r7,r7,r2
	or	r6,r6,r5
	mul	r10,r6,r8
	muhu	r6,r6,r8
	move	r2,r10
	move	r5,r6
	bltuc	r7,r6,0040419C

l00404196:
	bnec	r6,r7,004041AC

l00404198:
	bgeuc	r4,r10,004041AC

l0040419C:
	subu	r2,r10,r8
	subu	r6,r6,r12
	sltu	r10,r10,r2
	subu	r5,r6,r10

l004041AC:
	subu	r2,r4,r2
	subu	r7,r7,r5
	sltu	r4,r4,r2
	subu	r7,r7,r4
	srlv	r4,r2,r3
	sllv	r8,r7,r9
	srlv	r5,r7,r3
	or	r4,r8,r4
	bc	00403FEA
004041CA                               00 00 00 00 00 00           ......

;; __gtdf2: 004041D0
;;   Called from:
;;     0040BC58 (in decfloat)
;;     0040C34C (in __floatscan)
__gtdf2 proc
	ext	r10,r5,00000004,0000000B
	addiu	r2,r0,000007FF
	ext	r8,r5,00000000,00000014
	ext	r9,r7,00000000,00000014
	ext	r11,r7,00000004,0000000B
	move	r3,r4
	srl	r5,r5,0000001F
	move	r12,r6
	srl	r7,r7,0000001F
	bnec	r10,r2,004041FC

l004041F4:
	or	r13,r8,r4
	bnec	r0,r13,00404254

l004041FC:
	bnec	r11,r2,00404208

l00404200:
	or	r2,r9,r6
	bnec	r0,r2,00404254

l00404208:
	move	r2,r0
	bnec	r0,r10,00404216

l0040420E:
	or	r4,r8,r4
	sltiu	r2,r4,00000001

l00404216:
	bnec	r0,r11,00404220

l0040421A:
	or	r6,r9,r6
	beqzc	r6,0040425A

l00404220:
	beqc	r0,r2,00404260

l00404224:
	li	r5,FFFFFFFF
	movn	r5,r7,r7

l0040422A:
	move	r4,r5
	jrc	ra

l0040422E:
	bgec	r10,r11,0040423A

l00404232:
	li	r7,FFFFFFFF
	movz	r5,r7,r5

l00404238:
	bc	0040422A

l0040423A:
	bltuc	r9,r8,00404266

l0040423E:
	bnec	r8,r9,0040424C

l00404242:
	bltuc	r12,r3,00404266

l00404246:
	bltuc	r3,r12,00404232

l0040424A:
	bc	00404250

l0040424C:
	bltuc	r8,r9,00404232

l00404250:
	move	r5,r0
	bc	0040422A

l00404254:
	addiu	r5,r0,FFFFFFFE
	bc	0040422A

l0040425A:
	bnec	r0,r2,00404250

l0040425E:
	bc	00404266

l00404260:
	bnec	r5,r7,00404266

l00404262:
	bgec	r11,r10,0040422E

l00404266:
	li	r7,FFFFFFFF
	li	r6,00000001
	movz	r7,r6,r5

l0040426E:
	move	r5,r7
	bc	0040422A
00404272       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __ltdf2: 00404280
__ltdf2 proc
	ext	r11,r5,00000004,0000000B
	addiu	r14,r0,000007FF
	ext	r9,r5,00000000,00000014
	ext	r10,r7,00000000,00000014
	ext	r2,r7,00000004,0000000B
	move	r12,r4
	srl	r5,r5,0000001F
	move	r13,r6
	srl	r7,r7,0000001F
	bnec	r11,r14,004042B0

l004042A4:
	or	r3,r9,r4
	addiu	r8,r0,00000002
	bnec	r0,r3,004042E4

l004042B0:
	bnec	r2,r14,004042C0

l004042B4:
	or	r3,r10,r6
	addiu	r8,r0,00000002
	bnec	r0,r3,004042E4

l004042C0:
	move	r3,r0
	bnec	r0,r11,004042CE

l004042C6:
	or	r4,r9,r4
	sltiu	r3,r4,00000001

l004042CE:
	bnec	r0,r2,004042D8

l004042D2:
	or	r6,r10,r6
	beqzc	r6,00404312

l004042D8:
	beqc	r0,r3,0040431A

l004042DC:
	addiu	r8,r0,FFFFFFFF
	movn	r8,r7,r7

l004042E4:
	move	r4,r8
	jrc	ra

l004042E8:
	bgec	r11,r2,004042F6

l004042EC:
	addiu	r8,r0,FFFFFFFF
	movn	r8,r5,r5

l004042F4:
	bc	004042E4

l004042F6:
	bltuc	r10,r9,00404320

l004042FA:
	bnec	r9,r10,0040430A

l004042FE:
	bltuc	r13,r12,00404320

l00404302:
	move	r8,r0
	bltuc	r12,r13,004042EC

l00404308:
	bc	004042E4

l0040430A:
	move	r8,r0
	bltuc	r9,r10,004042EC

l00404310:
	bc	004042E4

l00404312:
	move	r8,r0
	bnec	r0,r3,004042E4

l00404318:
	bc	00404320

l0040431A:
	bnec	r5,r7,00404320

l0040431C:
	bgec	r2,r11,004042E8

l00404320:
	li	r4,FFFFFFFF
	addiu	r8,r0,00000001
	movn	r8,r4,r5

l0040432A:
	bc	004042E4
0040432C                                     00 00 00 00             ....

;; __muldf3: 00404330
;;   Called from:
;;     00409260 (in fn00409170)
;;     004093CC (in fn00409170)
;;     00409426 (in fn00409170)
;;     00409470 (in fn00409170)
;;     0040B5FA (in decfloat)
;;     0040B82E (in decfloat)
;;     0040B9BC (in decfloat)
;;     0040B9DC (in decfloat)
;;     0040BA40 (in decfloat)
;;     0040BB26 (in decfloat)
;;     0040BB32 (in decfloat)
;;     0040BBD4 (in decfloat)
;;     0040BC10 (in decfloat)
;;     0040BC1C (in decfloat)
;;     0040BC7C (in decfloat)
;;     0040BD00 (in decfloat)
;;     0040BD2C (in decfloat)
;;     0040C1B6 (in __floatscan)
;;     0040C1C8 (in __floatscan)
;;     0040C210 (in __floatscan)
;;     0040C3D2 (in __floatscan)
;;     0040C3E2 (in __floatscan)
;;     0040C438 (in __floatscan)
;;     0040C53A (in __floatscan)
;;     0040C546 (in __floatscan)
;;     0040C5B6 (in __floatscan)
;;     0040C5C2 (in __floatscan)
;;     0040CFCC (in fn0040CFCC)
;;     0040E17C (in fmod)
;;     0040E1AE (in fmod)
;;     0040E2E4 (in frexp)
__muldf3 proc
	ext	r10,r5,00000004,0000000B
	ext	r11,r5,00000000,00000014
	move	r12,r6
	srl	r5,r5,0000001F
	beqc	r0,r10,004043BE

l00404342:
	addiu	r6,r0,000007FF
	beqc	r10,r6,00404410

l0040434A:
	lui	r8,00000800
	srl	r6,r4,0000001D
	or	r6,r6,r8
	sll	r11,r11,00000003
	or	r11,r6,r11
	sll	r8,r4,00000003
	addiu	r10,r10,FFFFFC01

l00404366:
	move	r13,r0

l00404368:
	ext	r6,r7,00000004,0000000B
	ext	r4,r7,00000000,00000014
	srl	r14,r7,0000001F
	beqc	r0,r6,00404432

l00404378:
	addiu	r7,r0,000007FF
	beqc	r6,r7,00404482

l00404380:
	srl	r9,r12,0000001D
	lui	r7,00000800
	or	r9,r9,r7
	sll	r4,r4,00000003
	or	r4,r9,r4
	addiu	r6,r6,FFFFFC01
	sll	r9,r12,00000003

l0040439A:
	move	r3,r0

l0040439C:
	addu	r6,r6,r10
	sll	r10,r13,00000002
	or	r10,r10,r3
	xor	r2,r5,r14
	addiu	r10,r10,FFFFFFFF
	addiu	r12,r6,00000001
	bgeiuc	r10,0000000F,004044A4

l004043B4:
	addiupc	r7,0000DF4C
	lwxs	r7,r10(r7)
	jrc	r7

l004043BE:
	or	r8,r11,r4
	beqc	r0,r8,00404420

l004043C6:
	clz	r9,r11
	bnec	r0,r11,004043D6

l004043CE:
	clz	r10,r4
	addiu	r9,r10,00000020

l004043D6:
	addiu	r6,r9,FFFFFFF5
	bgeic	r6,0000001D,00404404

l004043DE:
	addiu	r10,r0,0000001D
	addiu	r8,r9,FFFFFFF8
	subu	r10,r10,r6
	sllv	r11,r11,r8
	srlv	r10,r4,r10
	sllv	r8,r4,r8
	or	r11,r10,r11

l004043FA:
	addiu	r10,r0,FFFFFC0D
	subu	r10,r10,r9
	bc	00404366

l00404404:
	addiu	r11,r9,FFFFFFD8
	move	r8,r0
	sllv	r11,r4,r11
	bc	004043FA

l00404410:
	or	r8,r11,r4
	beqc	r0,r8,0040442A

l00404418:
	move	r8,r4
	addiu	r13,r0,00000003
	bc	00404368

l00404420:
	move	r11,r0
	move	r10,r0
	addiu	r13,r0,00000001
	bc	00404368

l0040442A:
	move	r11,r0
	addiu	r13,r0,00000002
	bc	00404368

l00404432:
	or	r9,r4,r12
	beqc	r0,r9,00404492

l0040443A:
	clz	r2,r4
	bnezc	r4,00404448

l00404440:
	clz	r2,r12
	addiu	r2,r2,00000020

l00404448:
	addiu	r6,r2,FFFFFFF5
	bgeic	r6,0000001D,00404476

l00404450:
	addiu	r3,r0,0000001D
	addiu	r9,r2,FFFFFFF8
	subu	r3,r3,r6
	sllv	r4,r4,r9
	srlv	r3,r12,r3
	sllv	r9,r12,r9
	or	r4,r3,r4

l0040446C:
	addiu	r6,r0,FFFFFC0D
	subu	r6,r6,r2
	bc	0040439A

l00404476:
	addiu	r4,r2,FFFFFFD8
	move	r9,r0
	sllv	r4,r12,r4
	bc	0040446C

l00404482:
	or	r9,r4,r12
	beqc	r0,r9,0040449C

l0040448A:
	move	r9,r12
	addiu	r3,r0,00000003
	bc	0040439C

l00404492:
	move	r4,r0
	move	r6,r0
	addiu	r3,r0,00000001
	bc	0040439C

l0040449C:
	move	r4,r0
	addiu	r3,r0,00000002
	bc	0040439C

l004044A4:
	mul	r3,r8,r4
	muhu	r10,r8,r9
	mul	r13,r8,r9
	mul	r15,r4,r11
	muhu	r8,r8,r4
	mul	r5,r9,r11
	muhu	r9,r9,r11
	addu	r10,r3,r10
	muhu	r7,r4,r11
	sltu	r3,r10,r3
	addu	r15,r8,r15
	addu	r10,r10,r5
	addu	r25,r3,r15
	sltu	r5,r10,r5
	addu	r24,r9,r25
	addu	r14,r5,r24
	sltu	r8,r15,r8
	sltu	r3,r25,r3
	sltu	r9,r24,r9
	or	r4,r8,r3
	sltu	r5,r14,r5
	addu	r4,r4,r7
	or	r5,r9,r5
	sll	r8,r10,00000009
	addu	r4,r4,r5
	or	r8,r8,r13
	srl	r7,r14,00000017
	sll	r4,r4,00000009
	sltu	r8,r0,r8
	srl	r5,r10,00000017
	or	r11,r4,r7
	or	r8,r8,r5
	sll	r7,r14,00000009
	or	r8,r8,r7
	bbeqzc	r11,00000018,004045E8

l0040452A:
	srl	r7,r8,00000001
	andi	r8,r8,00000001
	or	r8,r7,r8
	sll	r7,r11,0000001F
	or	r8,r8,r7
	srl	r11,r11,00000001

l00404542:
	addiu	r5,r12,000003FF
	bgec	r0,r5,004045EC

l0040454A:
	andi	r7,r8,00000007
	beqzc	r7,00404564

l00404550:
	andi	r7,r8,0000000F
	beqic	r7,00000004,00404564

l00404558:
	addiu	r7,r8,00000004
	sltu	r8,r7,r8
	addu	r11,r11,r8
	move	r8,r7

l00404564:
	bbeqzc	r11,00000018,00404570

l00404568:
	ins	r11,r0,00000008,00000001
	addiu	r5,r12,00000400

l00404570:
	addiu	r7,r0,000007FE
	bltc	r7,r5,0040467C

l00404578:
	sll	r7,r11,0000001D
	srl	r8,r8,00000003
	or	r8,r7,r8
	srl	r7,r11,00000003

l00404588:
	move	r6,r0
	move	r9,r8
	ins	r6,r7,00000000,00000001
	ins	r6,r5,00000004,00000001
	ins	r6,r2,0000000F,00000001
	move	r7,r6
	movep	r4,r5,r9,r7
	jrc	ra
0040459E                                           8B 20               . 
004045A0 90 22 E8 E0 00 00 78 52 E0 60 FF FF 0F 00 80 20 ."....xR.`..... 
004045B0 10 2E 87 20 10 5E FF D3 45 10 87 20 10 46 E8 E0 ... .^..E.. .F..
004045C0 00 00 EB 20 90 3A                               ... .:          

l004045C6:
	addiu	r5,r0,000007FF
	bc	00404588
004045CC                                     45 10 A0 C9             E...
004045D0 AA 10 A0 C9 E9 1F B0 C9 69 0F                   ........i.      

l004045DA:
	movep	r7,r8,r0,r0
	bc	00404642
004045DE                                           4E 10               N.
004045E0 64 11 09 11 A3 11 E7 1B                         d.......        

l004045E8:
	move	r12,r6
	bc	00404542

l004045EC:
	li	r7,00000001
	subu	r7,r7,r5
	bgeic	r7,00000039,004045DA

l004045F4:
	bgeic	r7,00000020,00404646

l004045F8:
	li	r5,00000020
	srlv	r4,r8,r7
	subu	r5,r5,r7
	srlv	r7,r11,r7
	sllv	r6,r11,r5
	sllv	r8,r8,r5
	or	r6,r6,r4
	sltu	r8,r0,r8
	or	r8,r6,r8

l00404616:
	andi	r6,r8,00000007
	beqzc	r6,00404630

l0040461C:
	andi	r6,r8,0000000F
	beqic	r6,00000004,00404630

l00404624:
	addiu	r6,r8,00000004
	sltu	r8,r6,r8
	addu	r7,r7,r8
	move	r8,r6

l00404630:
	bbnezc	r7,00000017,00404680

l00404634:
	sll	r6,r7,0000001D
	srl	r8,r8,00000003
	or	r8,r6,r8
	srl	r7,r7,00000003

l00404642:
	move	r5,r0
	bc	00404588

l00404646:
	addiu	r6,r0,FFFFFFE1
	subu	r6,r6,r5
	move	r5,r0
	srlv	r6,r11,r6
	beqic	r7,00000020,0040465E

l00404656:
	subu	r7,r0,r7
	sllv	r5,r11,r7

l0040465E:
	or	r8,r5,r8
	move	r7,r0
	sltu	r8,r0,r8
	or	r8,r6,r8
	bc	00404616
0040466E                                           60 61               `a
00404670 FF FF 0F 00 00 81 01 80 40 10 43 1B             ........@.C.    

l0040467C:
	movep	r7,r8,r0,r0
	bc	004045C6

l00404680:
	movep	r7,r8,r0,r0
	li	r5,00000001
	bc	00404588
00404686                   00 00 00 00 00 00 00 00 00 00       ..........

;; __unorddf2: 00404690
__unorddf2 proc
	ext	r9,r5,00000000,00000014
	addiu	r10,r0,000007FF
	ext	r5,r5,00000004,0000000B
	ext	r8,r7,00000000,00000014
	ext	r7,r7,00000004,0000000B
	bnec	r5,r10,004046B0

l004046A8:
	or	r5,r9,r4
	li	r4,00000001
	bnezc	r5,004046BE

l004046B0:
	move	r4,r0
	bnec	r7,r10,004046BE

l004046B6:
	or	r4,r8,r6
	sltu	r4,r0,r4

l004046BE:
	jrc	ra

;; __fixdfsi: 004046C0
;;   Called from:
;;     00409230 (in fn00409170)
__fixdfsi proc
	ext	r6,r5,00000004,0000000B
	addiu	r7,r0,000003FE
	move	r9,r4
	ext	r8,r5,00000000,00000014
	move	r4,r0
	srl	r5,r5,0000001F
	bgec	r7,r6,00404714

l004046D8:
	addiu	r7,r0,0000041D
	bgec	r7,r6,004046EA

l004046E0:
	addiu	r5,r5,7FFFFFFF
	move	r4,r5
	jrc	ra

l004046EA:
	lui	r7,00000100
	or	r7,r8,r7
	addiu	r8,r0,00000433
	subu	r8,r8,r6
	bgeic	r8,00000020,00404716

l004046FE:
	addiu	r4,r6,FFFFFBED
	srlv	r8,r9,r8
	sllv	r4,r7,r4
	or	r4,r4,r8

l0040470E:
	beqzc	r5,00404714

l00404710:
	subu	r4,r0,r4

l00404714:
	jrc	ra

l00404716:
	addiu	r4,r0,00000413
	subu	r4,r4,r6
	srlv	r4,r7,r4
	bc	0040470E
00404722       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; sysconf: 00404730
;;   Called from:
;;     00402CEE (in fn00402CEE)
sysconf proc
	save	00000150,ra,00000002
	addiu	r7,r0,000000F8
	move	r16,r4
	bltuc	r7,r4,00404748

l0040473E:
	addiupc	r7,0000DC26
	lhxs	r4,r4(r7)
	bnezc	r4,00404756

l00404748:
	balc	004049B0
	li	r7,00000016
	sw	r7,0000(sp)
	li	r4,FFFFFFFF

l00404752:
	restore.jrc	00000150,ra,00000002

l00404756:
	li	r7,FFFFFFFF
	bgec	r4,r7,00404752

l0040475C:
	addiu	r7,r7,FFFFFF01
	bgec	r4,r7,00404782

l00404764:
	addiu	r5,sp,00000008
	ext	r4,r4,00000000,0000000E
	balc	00405B30
	lw	r17,000C(sp)
	li	r7,7FFFFFFF
	lw	r17,0008(sp)
	bnec	r0,r6,00404830

l0040477C:
	bgeuc	r7,r4,00404752

l00404780:
	bc	00404830

l00404782:
	addiu	r7,r4,FFFFFFFF
	andi	r7,r7,000000FF
	bgeiuc	r7,0000000A,00404752

l0040478C:
	addiupc	r6,0000DBB0
	lwxs	r7,r7(r6)
	jrc	r7

l00404794:
	lui	r4,00000020

l00404798:
	bc	00404752
0040479A                               80 00 00 80 B3 1B           ......

l004047A0:
	addiupc	r7,0004FC8C
	lw	r4,0024(r7)
	bc	00404752

l004047A8:
	addiu	r16,sp,00000008
	addiu	r6,r0,00000080
	movep	r4,r5,r16,r0
	balc	0040A690
	li	r7,00000001
	sb	r7,0008(sp)
	addiu	r6,r0,00000080
	move	r7,r16
	move	r5,r0
	li	r4,0000007B
	balc	00404A50
	move	r4,r0
	move	r7,r0
	bc	004047DA

l004047CE:
	addiu	r5,r6,FFFFFFFF
	addiu	r4,r4,00000001
	and	r6,r6,r5
	sbx	r6,r16(r7)

l004047DA:
	lbux	r6,r7(r16)
	bnezc	r6,004047CE

l004047E0:
	addiu	r7,r7,00000001
	addiu	r6,r0,00000080
	bnec	r7,r6,004047DA

l004047EA:
	bc	00404752

l004047EC:
	addiu	r4,sp,00000008
	balc	00404A70
	lw	r17,003C(sp)
	addiu	r7,sp,FFFFF140
	bnezc	r6,00404800

l004047FA:
	li	r6,00000001
	sw	r6,0EFC(r7)

l00404800:
	lw	r17,0018(sp)
	beqic	r16,00000015,0040480C

l00404806:
	lw	r17,001C(sp)
	lw	r17,0024(sp)
	addu	r4,r4,r7

l0040480C:
	lw	r4,003C(sp)
	move	r5,r0
	addiupc	r7,0004FC1C
	muhu	r8,r16,r4
	mul	r5,r5,r16
	mul	r4,r4,r16
	lw	r6,0024(r7)
	move	r7,r0
	addu	r5,r5,r8
	balc	0040EAB0
	li	r7,7FFFFFFF
	beqc	r0,r5,0040477C

l00404830:
	li	r4,7FFFFFFF
	bc	00404752

l00404838:
	move	r4,r0
	bc	00404752

l0040483C:
	li	r4,00031069
	bc	00404752
00404844             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; isxdigit: 00404850
;;   Called from:
;;     00401D92 (in fill)
;;     0040486A (in isxdigit_l)
isxdigit proc
	addiu	r6,r4,FFFFFFD0
	li	r7,00000001
	bltiuc	r6,0000000A,00404866

l0040485A:
	ori	r4,r4,00000020
	addiu	r4,r4,FFFFFF9F
	sltiu	r7,r4,00000006

l00404866:
	move	r4,r7
	jrc	ra

;; isxdigit_l: 0040486A
isxdigit_l proc
	bc	00404850
0040486E                                           00 00               ..

;; dummy: 00404870
dummy proc
	jrc	ra

;; __init_ssp: 00404872
;;   Called from:
;;     004048D8 (in __init_libc)
__init_ssp proc
	jrc	ra

;; __init_libc: 00404874
;;   Called from:
;;     00404998 (in __libc_start_main)
__init_libc proc
	save	000000D0,ra,00000003
	addiu	r6,r0,00000098
	movep	r17,r16,r4,r5
	move	r5,r0
	addiu	r4,sp,00000028
	balc	0040A690
	swpc	r17,00432DD0
	move	r4,r0

l0040488C:
	lwxs	r7,r4(r17)
	addiu	r4,r4,00000001
	bnezc	r7,0040488C

l00404892:
	lsa	r4,r4,r17,00000002
	addiupc	r17,0004FB96
	sw	r4,0050(sp)
	bc	004048A0

l0040489E:
	addiu	r4,r4,00000008

l004048A0:
	lw	r7,0000(r4)
	beqzc	r7,004048B6

l004048A4:
	bgeiuc	r7,00000026,0040489E

l004048A8:
	addiu	r6,sp,000000C0
	lsa	r7,r7,r6,00000002
	lw	r6,0004(r4)
	sw	r6,-0098(r7)
	bc	0040489E

l004048B6:
	lw	r17,0068(sp)
	swpc	r7,004544A0
	lw	r7,00A8(sp)
	swpc	r7,004544A8
	lw	r17,0040(sp)
	sw	r7,0064(sp)
	bnezc	r16,00404940

l004048CE:
	addiu	r4,sp,00000028
	balc	0040B152
	lw	r4,008C(sp)
	balc	00404872
	lwm	r6,0054(sp),00000002
	bnec	r6,r7,004048EE

l004048E2:
	lwm	r6,005C(sp),00000002
	bnec	r6,r7,004048EE

l004048E8:
	lw	r7,0084(sp)
	beqzc	r7,00404966

l004048EE:
	li	r7,00000001
	addiu	r9,r0,00000008
	sw	r7,0018(sp)
	li	r7,00000002
	sw	r7,0020(sp)
	move	r8,r0
	li	r6,00000003
	addiu	r5,sp,00000010
	li	r4,00000049
	addu	r7,sp,r9
	move	r16,r0
	sw	r0,0008(sp)
	sw	r0,000C(sp)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,001C(sp)
	sw	r0,0024(sp)
	balc	00404A50

l00404918:
	addiu	r7,sp,00000010
	addu	r7,r7,r16
	lhu	r7,0006(r7)
	bbeqzc	r7,00000005,0040495C

l00404922:
	addiu	r7,r0,00008002
	addiupc	r6,0000D586
	addiu	r5,r0,FFFFFF9C
	li	r4,00000038
	balc	00404A50
	bgec	r4,r0,0040495C

l00404938:
	sb	r0,0000(r0)
	teq	r0,r0,00000000

l00404940:
	swpc	r16,00432520

l00404946:
	swpc	r16,00432524
	bc	00404952

l0040494E:
	beqic	r7,0000002F,00404946

l00404952:
	addiu	r16,r16,00000001
	lbu	r7,-0001(r16)
	bnezc	r7,0040494E

l0040495A:
	bc	004048CE

l0040495C:
	addiu	r16,r16,00000008
	bneiuc	r16,00000018,00404918

l00404962:
	li	r7,00000001
	sw	r7,0048(sp)

l00404966:
	restore.jrc	000000D0,ra,00000003

;; __libc_start_init: 00404968
;;   Called from:
;;     0040499C (in __libc_start_main)
__libc_start_init proc
	save	00000010,ra,00000002
	addiupc	r16,0002B684
	balc	004000D4
	bc	0040497C

l00404976:
	lw	r7,0000(r16)
	addiu	r16,r16,00000004
	jalrc	ra,r7

l0040497C:
	addiupc	r7,0002B676
	bltuc	r16,r7,00404976

l00404986:
	restore.jrc	00000010,ra,00000002

;; __libc_start_main: 00404988
;;   Called from:
;;     00400878 (in _start_c)
__libc_start_main proc
	save	00000020,ra,00000005
	movep	r19,r18,r4,r5
	addiu	r16,r18,00000001
	lw	r5,0000(r6)
	lsa	r16,r16,r6,00000002
	move	r17,r6
	move.balc	r4,r16,00404874
	balc	00404968
	move	r4,r18
	movep	r5,r6,r17,r16
	jalrc	ra,r19
	balc	0040015A
	sigrie	00000000
004049AE                                           00 00               ..

;; __errno_location: 004049B0
;;   Called from:
;;     0040013A (in set_socket_option.isra.0.part.1)
;;     00400B56 (in fn00400B56)
;;     00401770 (in ping4_receive_error_msg)
;;     00401836 (in ping4_receive_error_msg)
;;     004021AC (in fn004021AC)
;;     00402CEA (in fn00402CEA)
;;     004032A0 (in fn004032A0)
;;     00404748 (in sysconf)
;;     004056DC (in malloc)
;;     00405826 (in realloc)
;;     00405C00 (in mmap64)
;;     00405C16 (in mmap64)
;;     00405C6E (in mremap)
;;     00405D9E (in mbtowc)
;;     00406740 (in if_indextoname)
;;     0040674A (in if_indextoname)
;;     0040686E (in inet_ntop)
;;     00406896 (in inet_ntop)
;;     00406A28 (in inet_pton)
;;     00406E7A (in name_from_hosts)
;;     00407458 (in __lookup_serv)
;;     00407816 (in __res_msend_rc)
;;     00407B3C (in __get_resolv_conf)
;;     00407DA0 (in socket)
;;     00407DF6 (in socket)
;;     00408024 (in __sigaction)
;;     0040804E (in sigprocmask)
;;     0040863A (in perror)
;;     00408D3A (in fn00408B86)
;;     00408D8E (in fn00408D8E)
;;     00409B64 (in vsnprintf)
;;     0040B06C (in __setxid)
;;     0040B652 (in decfloat)
;;     0040BB0C (in decfloat)
;;     0040BBF6 (in decfloat)
;;     0040BCA0 (in decfloat)
;;     0040BE80 (in __floatscan)
;;     0040C520 (in __floatscan)
;;     0040C59C (in __floatscan)
;;     0040C632 (in __floatscan)
;;     0040C736 (in __intscan)
;;     0040C894 (in __intscan)
;;     0040C92E (in __intscan)
;;     0040CAE2 (in __intscan)
;;     0040CC3E (in __syscall_ret)
;;     0040CDD8 (in calloc)
;;     0040CEDC (in __expand_heap)
;;     0040DD42 (in handler)
;;     0040DDDC (in handler)
;;     0040E040 (in readdir64)
;;     0040E3F4 (in mbrtowc)
;;     0040E558 (in mbsrtowcs)
;;     0040E62E (in wcrtomb)
;;     0040E7B6 (in sem_init)
;;     0040E7E2 (in sem_post)
;;     0040E924 (in sem_timedwait)
;;     0040E938 (in sem_trywait)
__errno_location proc
	rdhwr	r3,0000001D,00000000
	addiu	r4,r3,FFFFFF78
	jrc	ra
004049BA                               00 00 00 00 00 00           ......

;; strerror_l: 004049C0
;;   Called from:
;;     004049F2 (in strerror)
strerror_l proc
	move	r7,r0

l004049C2:
	addiupc	r6,0000E2A2
	lbux	r6,r7(r6)
	bnezc	r6,004049D8

l004049CC:
	addiupc	r4,0000DB8C

l004049D0:
	bnezc	r7,004049E0

l004049D2:
	lw	r5,0014(r5)
	bc	00404A92

l004049D8:
	beqc	r4,r6,004049CC

l004049DC:
	addiu	r7,r7,00000001
	bc	004049C2

l004049E0:
	lbu	r6,0000(r4)
	addiu	r4,r4,00000001
	bnezc	r6,004049E0

l004049E6:
	addiu	r7,r7,FFFFFFFF
	bc	004049D0

;; strerror: 004049EA
;;   Called from:
;;     00400140 (in set_socket_option.isra.0.part.1)
;;     0040099C (in create_socket)
;;     004009BE (in create_socket)
;;     00401856 (in ping4_receive_error_msg)
;;     00402FD4 (in ping6_receive_error_msg)
;;     00408640 (in perror)
strerror proc
	rdhwr	r3,0000001D,00000000
	lw	r5,-0038(r3)
	bc	004049C0
004049F6                   00 00 00 00 00 00 00 00 00 00       ..........

;; abort: 00404A00
;;   Called from:
;;     00401802 (in ping4_receive_error_msg)
;;     00402054 (in pinger)
;;     00402F82 (in ping6_receive_error_msg)
abort proc
	save	00000010,ra,00000001
	li	r4,00000006
	balc	00407EE0
	move	r4,r0
	balc	00407EA0
	sb	r0,0000(r0)
	teq	r0,r0,00000000
	sigrie	00000000
00404A1A                               00 00 00 00 00 00           ......

;; __funcs_on_exit: 00404A20
;;   Called from:
;;     0040015E (in exit)
__funcs_on_exit proc
	jrc	ra

;; __libc_exit_fini: 00404A22
;;   Called from:
;;     00400162 (in exit)
__libc_exit_fini proc
	save	00000010,ra,00000002
	addiupc	r16,0002B5D2
	bc	00404A32

l00404A2C:
	addiu	r16,r16,FFFFFFFC
	lw	r7,0000(r16)
	jalrc	ra,r7

l00404A32:
	addiupc	r7,0002B5C0
	bltuc	r7,r16,00404A2C

l00404A3C:
	restore	00000010,ra,00000002
	bc	00410340
00404A44             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __syscall: 00404A50
;;   Called from:
;;     004047C4 (in sysconf)
;;     00404914 (in __init_libc)
;;     00404930 (in __init_libc)
;;     00404A78 (in sysinfo)
;;     00404BE8 (in alloc_fwd)
;;     00404BFA (in alloc_fwd)
;;     00404CD2 (in alloc_fwd)
;;     00404CE4 (in alloc_fwd)
;;     00404DBC (in alloc_rev)
;;     00404DCE (in alloc_rev)
;;     00404EAE (in alloc_rev)
;;     00404EC0 (in alloc_rev)
;;     00405100 (in free)
;;     00405112 (in free)
;;     00405124 (in free)
;;     0040513A (in free)
;;     004051DC (in free)
;;     004051F4 (in free)
;;     00405274 (in free)
;;     0040528A (in free)
;;     00405340 (in malloc)
;;     00405354 (in malloc)
;;     00405656 (in malloc)
;;     0040566E (in malloc)
;;     00405686 (in malloc)
;;     0040569E (in malloc)
;;     00405710 (in malloc)
;;     00405726 (in malloc)
;;     00405B3C (in getrlimit64)
;;     00405BAE (in ioctl)
;;     00405BCC (in madvise)
;;     00405C3C (in mmap64)
;;     00405CAA (in mremap)
;;     00405CD0 (in munmap)
;;     00405DC0 (in bind)
;;     004066C0 (in getsockname)
;;     004066E2 (in getsockopt)
;;     00406738 (in if_indextoname)
;;     00406792 (in if_nametoindex)
;;     0040760E (in __rtnetlink_enumerate)
;;     004077A6 (in cleanup)
;;     00407D72 (in setsockopt)
;;     00407D92 (in socket)
;;     00407DC6 (in socket)
;;     00407DE0 (in socket)
;;     00407DF0 (in socket)
;;     00407E14 (in sched_yield)
;;     00407EB0 (in __block_all_sigs)
;;     00407EC4 (in __block_app_sigs)
;;     00407ED4 (in __restore_sigs)
;;     00407EEE (in raise)
;;     00407EF8 (in raise)
;;     00407F1A (in setitimer)
;;     00407F98 (in __libc_sigaction)
;;     00407FE0 (in __libc_sigaction)
;;     0040808C (in __fopen_rb_ca)
;;     004080A4 (in __fopen_rb_ca)
;;     004080EE (in __stdio_close)
;;     00408120 (in __stdio_read)
;;     00408180 (in __stdio_seek)
;;     0040AD80 (in __unlock)
;;     0040AD94 (in __unlock)
;;     0040ADA0 (in __syscall_cp_c)
;;     0040AE10 (in __wait)
;;     0040AE22 (in __wait)
;;     0040AE86 (in pthread_sigmask)
;;     0040AF16 (in __clock_gettime)
;;     0040AF2E (in __clock_gettime)
;;     0040AFA4 (in geteuid)
;;     0040AFB4 (in getpid)
;;     0040AFC4 (in getuid)
;;     0040AFDE (in isatty)
;;     0040B048 (in fn0040B048)
;;     0040B0E4 (in __init_tp)
;;     0040B20A (in __init_tls)
;;     0040B238 (in _Exit)
;;     0040CEA0 (in __expand_heap)
;;     0040CEBA (in __expand_heap)
;;     0040D230 (in __unlockfile)
;;     0040D244 (in __unlockfile)
;;     0040D2F8 (in __stdio_write)
;;     0040D33E (in __stdout_write)
;;     0040DD36 (in __set_thread_area)
;;     0040DD5C (in handler)
;;     0040DDB8 (in handler)
;;     0040DF1C (in __synccall)
;;     0040DFAE (in __synccall)
;;     0040E00C (in fn0040E00C)
;;     0040E02A (in readdir64)
;;     0040E102 (in open64)
;;     0040E82E (in sem_post)
;;     0040E840 (in sem_post)
;;     0040E870 (in lseek64)
__syscall proc
	move	r2,r4
	move	r4,r5
	move	r5,r6
	move	r6,r7
	move	r7,r8
	move	r8,r9
	move	r9,r10
	syscall	00000000
	jrc	ra
00404A64             00 80 00 C0 00 80 00 C0 00 80 00 C0     ............

;; sysinfo: 00404A70
;;   Called from:
;;     004047EE (in sysconf)
sysinfo proc
	save	00000010,ra,00000001
	move	r5,r4
	addiu	r4,r0,000000B3
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
00404A84             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __lctrans_impl: 00404A90
;;   Called from:
;;     00404A92 (in __lctrans)
;;     00404AA0 (in __lctrans_cur)
__lctrans_impl proc
	jrc	ra

;; __lctrans: 00404A92
;;   Called from:
;;     004049D4 (in strerror_l)
__lctrans proc
	bc	00404A90

;; __lctrans_cur: 00404A96
;;   Called from:
;;     0040594C (in __getopt_msg)
;;     00405E10 (in gai_strerror)
__lctrans_cur proc
	rdhwr	r3,0000001D,00000000
	lw	r7,-0038(r3)
	lw	r5,0014(r7)
	bc	00404A90
00404AA4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __simple_malloc: 00404AB0
__simple_malloc proc
	save	00000020,ra,00000003
	beqzc	r4,00404B26

l00404AB4:
	move	r17,r4
	bltiuc	r4,00000002,00404B72

l00404ABA:
	li	r16,00000001
	bc	00404AC2

l00404ABE:
	bgeiuc	r16,00000010,00404B3A

l00404AC2:
	sll	r16,r16,00000001
	bltuc	r16,r17,00404ABE

l00404AC8:
	addiu	r16,r16,FFFFFFFF

l00404ACA:
	addiupc	r4,0002DA6A
	balc	0040AD30
	lwpc	r7,00432534
	subu	r6,r0,r7
	and	r16,r16,r6

l00404ADE:
	addu	r17,r17,r16

l00404AE0:
	lwpc	r6,00432530
	subu	r6,r6,r7
	bgeuc	r6,r17,00404B10

l00404AEC:
	addiu	r4,sp,0000000C
	sw	r17,000C(sp)
	balc	0040CDF0
	beqzc	r4,00404B64

l00404AF6:
	lwpc	r7,00432530
	beqc	r4,r7,00404B5C

l00404B00:
	subu	r17,r17,r16
	move	r7,r4
	move	r16,r0

l00404B06:
	lw	r17,000C(sp)
	addu	r4,r4,r6
	swpc	r4,00432530

l00404B10:
	addu	r17,r7,r17
	addu	r16,r7,r16
	addiupc	r4,0002DA20
	swpc	r17,00432534
	balc	0040AD60
	move	r4,r16
	restore.jrc	00000020,ra,00000003

l00404B26:
	addiupc	r4,0002DA0E
	move	r16,r0
	balc	0040AD30
	li	r17,00000001
	lwpc	r7,00432534
	bc	00404ADE

l00404B3A:
	addiupc	r4,0002D9FA
	addiu	r16,r16,FFFFFFFF
	balc	0040AD30
	lwpc	r7,00432534
	subu	r6,r0,r7
	li	r5,8000000F
	and	r16,r16,r6
	bltuc	r5,r17,00404AE0

l00404B5A:
	bc	00404ADE

l00404B5C:
	lwpc	r7,00432534
	bc	00404B06

l00404B64:
	addiupc	r4,0002D9D0
	move	r16,r0
	balc	0040AD60
	move	r4,r16
	restore.jrc	00000020,ra,00000003

l00404B72:
	move	r16,r0
	li	r17,00000001
	bc	00404ACA
00404B78                         00 00 00 00 00 00 00 00         ........

;; alloc_fwd: 00404B80
;;   Called from:
;;     0040503E (in free)
;;     004058C4 (in realloc)
alloc_fwd proc
	save	00000020,ra,00000008
	move	r19,r4

l00404B84:
	lw	r18,0004(r19)

l00404B86:
	bbnezc	r18,00000000,00404C4A

l00404B8A:
	srl	r7,r18,00000004
	addiu	r7,r7,FFFFFFFF
	bgeiuc	r7,00000021,00404C24

l00404B92:
	sll	r20,r7,00000004
	move	r17,r7
	move	r22,r7
	addiu	r16,r20,00000008

l00404B9E:
	addiupc	r7,0004F88E
	lw	r7,000C(r7)
	addiupc	r21,0002D9A8
	addu	r16,r16,r21
	bnezc	r7,00404C12

l00404BAC:
	addu	r7,r21,r20
	lw	r7,0010(r7)
	bnec	r0,r7,00404C8E

l00404BB6:
	move	r7,r18

l00404BB8:
	sll	r6,r22,00000004
	addu	r20,r20,r21
	addiu	r6,r6,00000008
	addu	r6,r6,r21
	sw	r6,0010(r20)
	sw	r6,0014(r20)

l00404BCA:
	beqc	r18,r7,00404C8E

l00404BCE:
	lw	r6,0000(r16)
	beqzc	r6,00404C44

l00404BD2:
	sync	00000000
	sw	r0,0000(sp)
	sync	00000000
	lw	r7,0004(r16)
	beqzc	r7,00404B84

l00404BE0:
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404B84

l00404BF4:
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r16,00404A50
	lw	r18,0004(r19)
	bc	00404B86

l00404C02:
	sync	00000000
	beqzc	r6,00404C4E

l00404C08:
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r16,00000004
	move.balc	r4,r16,0040ADB0

l00404C12:
	sync	00000000

l00404C16:
	ll	r6,0000(r16)
	li	r7,00000001
	sc	r7,0000(r16)
	beqzc	r7,00404C16

l00404C22:
	bc	00404C02

l00404C24:
	addiu	r6,r0,000001FF
	bltuc	r6,r7,00404C5C

l00404C2C:
	srl	r7,r7,00000003
	addiupc	r6,0000E0AE
	addu	r7,r7,r6
	lbu	r17,-0004(r7)
	sll	r20,r17,00000004
	move	r22,r17
	addiu	r16,r20,00000008
	bc	00404B9E

l00404C44:
	move	r18,r7
	bbeqzc	r18,00000000,00404B8A

l00404C4A:
	move	r4,r0
	restore.jrc	00000020,ra,00000008

l00404C4E:
	addu	r6,r21,r20
	lw	r7,0004(r19)
	lw	r6,0010(r6)
	bnec	r0,r6,00404BCA

l00404C5A:
	bc	00404BB8

l00404C5C:
	addiu	r6,r0,00001C00
	bltuc	r6,r7,00404C7E

l00404C64:
	srl	r7,r7,00000007
	addiupc	r6,0000E076
	addu	r7,r7,r6
	lbu	r7,-0004(r7)
	addiu	r17,r7,00000010
	sll	r20,r17,00000004
	move	r22,r17
	addiu	r16,r20,00000008
	bc	00404B9E

l00404C7E:
	addiu	r16,r0,000003F8
	addiu	r22,r0,0000003F
	li	r17,0000003F
	addiu	r20,r0,000003F0
	bc	00404B9E

l00404C8E:
	lwm	r6,0008(r19),00000002
	beqc	r6,r7,00404CEC

l00404C96:
	move	r5,r18
	sw	r6,0048(sp)
	ins	r5,r0,00000000,00000001
	lw	r4,0008(r19)
	ori	r18,r18,00000001
	lwx	r6,r5(r19)
	sw	r7,000C(sp)
	ori	r6,r6,00000001
	sw	r18,0044(sp)
	swx	r6,r5(r19)
	lw	r7,0000(r16)
	beqzc	r7,00404CC6

l00404CB8:
	sync	00000000
	sw	r0,0000(sp)
	sync	00000000
	lw	r7,0004(r16)
	bnezc	r7,00404CCA

l00404CC6:
	li	r4,00000001
	restore.jrc	00000020,ra,00000008

l00404CCA:
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404CC6

l00404CDE:
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r16,00404A50
	li	r4,00000001
	restore.jrc	00000020,ra,00000008

l00404CEC:
	li	r6,00000001
	subu	r7,r0,r17
	srlv	r7,r6,r7
	slti	r5,r17,00000020
	sllv	r6,r6,r17
	movz	r7,r0,r17

l00404D02:
	movz	r7,r6,r5

l00404D06:
	movz	r6,r0,r5

l00404D0A:
	nor	r7,r0,r7
	nor	r4,r0,r6
	beqzc	r6,00404D34

l00404D14:
	sync	00000000
	aluipc	r5,00000404

l00404D1C:
	ori	r6,r5,00000550
	ll	r6,0000(r6)
	and	r6,r6,r4
	ori	r17,r5,00000550
	sc	r6,0000(r17)
	beqzc	r6,00404D1C

l00404D30:
	sync	00000000

l00404D34:
	li	r6,FFFFFFFF
	beqc	r6,r7,00404D4C

l00404D38:
	sync	00000000

l00404D3C:
	ll	r6,0004(r21)
	and	r6,r6,r7
	sc	r6,0004(r21)
	beqzc	r6,00404D3C

l00404D48:
	sync	00000000

l00404D4C:
	lw	r18,0004(r19)
	lwm	r6,0008(r19),00000002
	bc	00404C96

;; alloc_rev: 00404D54
;;   Called from:
;;     0040500C (in free)
;;     004053E2 (in malloc)
alloc_rev proc
	save	00000020,ra,00000008
	move	r19,r4

l00404D58:
	lw	r18,0000(r19)

l00404D5A:
	bbnezc	r18,00000000,00404E1E

l00404D5E:
	srl	r7,r18,00000004
	addiu	r7,r7,FFFFFFFF
	bgeiuc	r7,00000021,00404DF8

l00404D66:
	sll	r20,r7,00000004
	move	r17,r7
	move	r22,r7
	addiu	r16,r20,00000008

l00404D72:
	addiupc	r7,0004F6BA
	lw	r7,000C(r7)
	addiupc	r21,0002D7D4
	addu	r16,r16,r21
	bnezc	r7,00404DE6

l00404D80:
	addu	r7,r21,r20
	lw	r7,0010(r7)
	bnec	r0,r7,00404E62

l00404D8A:
	move	r7,r18

l00404D8C:
	sll	r6,r22,00000004
	addu	r20,r20,r21
	addiu	r6,r6,00000008
	addu	r6,r6,r21
	sw	r6,0010(r20)
	sw	r6,0014(r20)

l00404D9E:
	beqc	r18,r7,00404E62

l00404DA2:
	lw	r6,0000(r16)
	beqzc	r6,00404E18

l00404DA6:
	sync	00000000
	sw	r0,0000(sp)
	sync	00000000
	lw	r7,0004(r16)
	beqzc	r7,00404D58

l00404DB4:
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404D58

l00404DC8:
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r16,00404A50
	lw	r18,0000(r19)
	bc	00404D5A

l00404DD6:
	sync	00000000
	beqzc	r6,00404E22

l00404DDC:
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r16,00000004
	move.balc	r4,r16,0040ADB0

l00404DE6:
	sync	00000000

l00404DEA:
	ll	r6,0000(r16)
	li	r7,00000001
	sc	r7,0000(r16)
	beqzc	r7,00404DEA

l00404DF6:
	bc	00404DD6

l00404DF8:
	addiu	r6,r0,000001FF
	bltuc	r6,r7,00404E30

l00404E00:
	srl	r7,r7,00000003
	addiupc	r6,0000DEDA
	addu	r7,r7,r6
	lbu	r17,-0004(r7)
	sll	r20,r17,00000004
	move	r22,r17
	addiu	r16,r20,00000008
	bc	00404D72

l00404E18:
	move	r18,r7
	bbeqzc	r18,00000000,00404D5E

l00404E1E:
	move	r4,r0
	restore.jrc	00000020,ra,00000008

l00404E22:
	addu	r6,r21,r20
	lw	r7,0000(r19)
	lw	r6,0010(r6)
	bnec	r0,r6,00404D9E

l00404E2E:
	bc	00404D8C

l00404E30:
	addiu	r6,r0,00001C00
	bltuc	r6,r7,00404E52

l00404E38:
	srl	r7,r7,00000007
	addiupc	r6,0000DEA2
	addu	r7,r7,r6
	lbu	r7,-0004(r7)
	addiu	r17,r7,00000010
	sll	r20,r17,00000004
	move	r22,r17
	addiu	r16,r20,00000008
	bc	00404D72

l00404E52:
	addiu	r16,r0,000003F8
	addiu	r22,r0,0000003F
	li	r17,0000003F
	addiu	r20,r0,000003F0
	bc	00404D72

l00404E62:
	ins	r18,r0,00000000,00000001
	subu	r19,r19,r18
	lwm	r5,0008(r19),00000002
	beqc	r5,r6,00404EC8

l00404E70:
	lw	r7,0004(r19)
	sw	r5,0008(sp)
	move	r4,r7
	lw	r17,0008(r19)
	ins	r4,r0,00000000,00000001
	ori	r7,r7,00000001
	lwx	r5,r4(r19)
	sw	r6,004C(sp)
	ori	r5,r5,00000001
	sw	r7,0044(sp)
	swx	r5,r4(r19)
	lw	r7,0000(r16)
	beqzc	r7,00404EA2

l00404E94:
	sync	00000000
	sw	r0,0000(sp)
	sync	00000000
	lw	r7,0004(r16)
	bnezc	r7,00404EA6

l00404EA2:
	li	r4,00000001
	restore.jrc	00000020,ra,00000008

l00404EA6:
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404EA2

l00404EBA:
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r16,00404A50
	li	r4,00000001
	restore.jrc	00000020,ra,00000008

l00404EC8:
	li	r6,00000001
	subu	r7,r0,r17
	srlv	r7,r6,r7
	slti	r5,r17,00000020
	sllv	r6,r6,r17
	movz	r7,r0,r17

l00404EDE:
	movz	r7,r6,r5

l00404EE2:
	movz	r6,r0,r5

l00404EE6:
	nor	r7,r0,r7
	nor	r4,r0,r6
	beqzc	r6,00404F10

l00404EF0:
	sync	00000000
	aluipc	r5,00000404

l00404EF8:
	ori	r6,r5,00000550
	ll	r6,0000(r6)
	and	r6,r6,r4
	ori	r17,r5,00000550
	sc	r6,0000(r17)
	beqzc	r6,00404EF8

l00404F0C:
	sync	00000000

l00404F10:
	li	r6,FFFFFFFF
	beqc	r6,r7,00404F28

l00404F14:
	sync	00000000

l00404F18:
	ll	r6,0004(r21)
	and	r6,r6,r7
	sc	r6,0004(r21)
	beqzc	r6,00404F18

l00404F24:
	sync	00000000

l00404F28:
	lwm	r5,0008(r19),00000002
	bc	00404E70

;; free: 00404F2E
;;   Called from:
;;     00403394 (in niquery_option_subject_addr_handler)
;;     004055EE (in malloc)
;;     004058BC (in realloc)
;;     004058F8 (in realloc)
;;     00405910 (in realloc)
;;     00405DF0 (in freeaddrinfo)
;;     00406288 (in netlink_msg_to_ifaddr)
;;     00406298 (in freeifaddrs)
;;     0040D97A (in __isoc99_vfscanf)
;;     0040D97E (in __isoc99_vfscanf)
free proc
	beqc	r0,r4,00405290

l00404F32:
	save	00000040,r30,0000000A
	lw	r7,-0004(r4)
	addiu	r17,r4,FFFFFFF8
	move	r22,r7
	ins	r22,r0,00000000,00000001
	bbnezc	r7,00000000,00404F5C

l00404F46:
	lw	r7,-0008(r4)
	subu	r4,r17,r7
	addu	r5,r7,r22
	bbeqzc	r7,00000000,00404F6E

l00404F54:
	sb	r0,0000(r0)
	teq	r0,r0,00000000

l00404F5C:
	lwx	r6,r22(r17)
	addu	r18,r17,r22
	beqc	r6,r7,00404F76

l00404F66:
	sb	r0,0000(r0)
	teq	r0,r0,00000000

l00404F6E:
	restore	00000040,r30,0000000A
	bc	00405CC2

l00404F76:
	move	r19,r22
	addiupc	r30,0002D5D4
	addiupc	r21,0004F4B0
	sw	r0,0008(sp)

l00404F82:
	lw	r7,0000(r17)
	lw	r6,0004(r18)
	and	r7,r7,r6
	bbeqzc	r7,00000000,0040500C

l00404F8C:
	srl	r16,r19,00000004
	ori	r7,r19,00000001
	addiu	r16,r16,FFFFFFFF
	sw	r7,0044(sp)
	sw	r7,0000(sp)
	bltiuc	r16,00000021,00404FB0

l00404F9C:
	addiu	r7,r0,000001FF
	bltuc	r7,r16,004050C6

l00404FA4:
	srl	r16,r16,00000003
	addiupc	r7,0000DD36
	addu	r7,r16,r7
	lbu	r16,-0004(r7)

l00404FB0:
	sll	r20,r16,00000004
	sw	r16,000C(sp)
	addiu	r8,r20,00000008

l00404FBA:
	lw	r7,0003(r21)
	addu	r23,r30,r8
	bnec	r0,r7,004050B4

l00404FC4:
	addu	r7,r30,r20
	lw	r6,0010(r7)
	beqc	r0,r6,004050E8

l00404FCE:
	lw	r7,0003(r21)
	bnec	r0,r7,00405086

l00404FD4:
	lw	r7,0000(r17)
	lw	r6,0004(r18)
	and	r7,r7,r6
	bbnezc	r7,00000000,00405150

l00404FDE:
	lw	r7,0408(r30)
	beqzc	r7,00404FF8

l00404FE4:
	sync	00000000
	sw	r0,0408(r30)
	sync	00000000
	lw	r7,040C(r30)
	bnec	r0,r7,00405118

l00404FF8:
	lw	r7,0000(r23)
	beqzc	r7,0040500C

l00404FFC:
	sync	00000000
	sw	r11,0000(r23)
	sync	00000000
	lw	r7,0001(r23)
	bnec	r0,r7,004050F8

l0040500C:
	move.balc	r4,r17,00404D54
	beqzc	r4,0040503E

l00405012:
	lw	r7,0000(r17)
	lui	r5,00000028
	ins	r7,r0,00000000,00000001
	subu	r17,r17,r7
	lw	r7,0004(r17)
	ins	r7,r0,00000000,00000001
	addu	r6,r7,r22
	addu	r19,r19,r7
	bgeuc	r5,r6,0040503E

l0040502E:
	xor	r6,r6,r7
	lw	r17,0008(sp)
	sltu	r7,r7,r6
	li	r6,00000001
	movn	r5,r6,r7

l0040503C:
	sw	r5,0008(sp)

l0040503E:
	move.balc	r4,r18,00404B80
	beqc	r0,r4,00404F82

l00405046:
	lw	r7,0004(r18)
	lui	r5,00000028
	ins	r7,r0,00000000,00000001
	addu	r6,r7,r22
	addu	r19,r19,r7
	bgeuc	r5,r6,0040506A

l0040505A:
	lw	r17,0008(sp)
	xor	r6,r6,r7
	sltu	r6,r7,r6
	li	r5,00000001
	movn	r4,r5,r6

l00405068:
	sw	r4,0008(sp)

l0040506A:
	addu	r18,r18,r7
	bc	00404F82

l0040506E:
	sync	00000000
	beqc	r0,r6,00404FD4

l00405076:
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,0002D8DE
	addiupc	r4,0002D8D6
	balc	0040ADB0

l00405086:
	sync	00000000

l0040508A:
	addiu	r7,r30,00000408
	ll	r6,0000(r7)
	li	r7,00000001
	addiu	r5,r30,00000408
	sc	r7,0000(r5)
	beqzc	r7,0040508A

l0040509E:
	bc	0040506E

l004050A0:
	sync	00000000
	beqc	r0,r6,00404FC4

l004050A8:
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r23,00000004
	move.balc	r4,r23,0040ADB0

l004050B4:
	sync	00000000

l004050B8:
	ll	r6,0000(r23)
	li	r7,00000001
	sc	r7,0000(r23)
	beqzc	r7,004050B8

l004050C4:
	bc	004050A0

l004050C6:
	addiu	r7,r0,00001C00
	bltuc	r7,r16,00405140

l004050CE:
	srl	r16,r16,00000007
	addiupc	r7,0000DC0C
	addu	r7,r16,r7
	lbu	r16,-0004(r7)
	addiu	r16,r16,00000010
	sll	r20,r16,00000004
	sw	r16,000C(sp)
	addiu	r8,r20,00000008
	bc	00404FBA

l004050E8:
	lw	r17,000C(sp)
	sll	r6,r6,00000004
	addiu	r6,r6,00000008
	addu	r6,r30,r6
	sw	r6,0050(sp)
	sw	r6,0054(sp)
	bc	00404FCE

l004050F8:
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r23,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040500C

l0040510C:
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r23,00404A50
	bc	0040500C

l00405118:
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,0002D836
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404FF8

l00405130:
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,0002D820
	li	r4,00000062
	balc	00404A50
	bc	00404FF8

l00405140:
	li	r7,0000003F
	addiu	r8,r0,000003F8
	li	r16,0000003F
	addiu	r20,r0,000003F0
	sw	r7,000C(sp)
	bc	00404FBA

l00405150:
	aluipc	r4,00000405
	subu	r7,r0,r16
	lw	r10,0550(r4)
	slti	r22,r16,00000020
	lw	r11,0554(r4)
	srlv	r6,r10,r16
	sllv	r5,r11,r7
	srlv	r10,r11,r16
	movz	r5,r0,r16

l00405174:
	or	r6,r6,r5
	movz	r6,r10,r22

l0040517A:
	bbeqzc	r6,00000000,00405218

l0040517E:
	sw	r19,0044(sp)
	sw	r19,0000(sp)
	lw	r7,0408(r30)
	beqzc	r7,0040519C

l00405188:
	sync	00000000
	sw	r0,0408(r30)
	sync	00000000
	lw	r7,040C(r30)
	bnec	r0,r7,00405268

l0040519C:
	lw	r17,000C(sp)
	addu	r20,r30,r20
	lw	r6,0014(r20)
	addiu	r7,r7,00000001
	sll	r7,r7,00000004
	sw	r6,004C(sp)
	addu	r9,r30,r7
	addiu	r7,r9,FFFFFFF8
	sw	r7,0048(sp)
	sw	r17,0001(r9)
	lw	r7,000C(r17)
	sw	r17,0048(sp)
	lw	r17,0008(sp)
	bnezc	r7,004051F8

l004051C0:
	lw	r7,0000(r23)
	beqzc	r7,004051D2

l004051C4:
	sync	00000000
	sw	r11,0000(r23)
	sync	00000000
	lw	r7,0001(r23)
	bnezc	r7,004051D4

l004051D2:
	restore.jrc	00000040,r30,0000000A

l004051D4:
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r23,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,004051D2

l004051E8:
	move	r5,r23
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	restore	00000040,r30,0000000A
	bc	00404A50

l004051F8:
	lw	r7,0024(r21)
	addiu	r5,r18,FFFFFFF0
	li	r6,00000004
	addiu	r4,r7,0000000F
	subu	r7,r0,r7
	addu	r4,r17,r4
	and	r5,r5,r7
	and	r4,r4,r7
	subu	r5,r5,r4
	balc	00405BC0
	bc	004051C0

l00405218:
	li	r5,00000001
	srlv	r7,r5,r7
	sllv	r5,r5,r16
	movz	r7,r0,r16

l00405226:
	movz	r7,r5,r22

l0040522A:
	movz	r5,r0,r22

l0040522E:
	move	r6,r7
	beqzc	r5,0040524E

l00405232:
	sync	00000000

l00405236:
	ori	r7,r4,00000550
	ll	r7,0000(r7)
	or	r7,r7,r5
	ori	r16,r4,00000550
	sc	r7,0000(r16)
	beqzc	r7,00405236

l0040524A:
	sync	00000000

l0040524E:
	beqc	r0,r6,0040517E

l00405252:
	sync	00000000

l00405256:
	ll	r7,0004(r30)
	or	r7,r7,r6
	sc	r7,0004(r30)
	beqzc	r7,00405256

l00405262:
	sync	00000000
	bc	0040517E

l00405268:
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,0002D6E6
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040519C

l00405280:
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,0002D6D0
	li	r4,00000062
	balc	00404A50
	bc	0040519C

l00405290:
	jrc	ra

;; malloc: 00405292
;;   Called from:
;;     00401208 (in ping4_run)
;;     004033AE (in niquery_option_subject_addr_handler)
;;     004039B0 (in ping6_run)
;;     0040579E (in __malloc0)
;;     004058E6 (in realloc)
;;     00405900 (in realloc)
;;     0040591A (in realloc)
;;     0040A87C (in __strdup)
;;     0040D8EC (in __isoc99_vfscanf)
;;     0040DA18 (in __isoc99_vfscanf)
malloc proc
	save	00000050,r30,0000000A
	addiupc	r22,0004F198
	li	r7,7FFFFFEF
	lw	r6,0024(r22)
	addiu	r5,r4,FFFFFFFF
	subu	r7,r7,r6
	bgeuc	r7,r5,00405472

l004052AC:
	bnec	r0,r4,004056DC

l004052B0:
	li	r7,00000010
	addiu	r21,r0,FFFFFFFF
	addiu	r20,r0,FFFFFFFF
	sw	r7,0008(sp)
	sw	r0,000C(sp)

l004052BE:
	aluipc	r23,00000405

l004052C2:
	lw	r4,0550(r23)
	lw	r5,0554(r23)
	and	r7,r4,r21
	and	r6,r5,r20
	or	r5,r7,r6
	beqc	r0,r5,00405370

l004052DA:
	bnec	r0,r7,00405450

l004052DE:
	subu	r7,r0,r6
	and	r7,r7,r6
	li	r6,076BE629
	mul	r7,r7,r6
	addiupc	r6,0000D9D0
	srl	r7,r7,0000001B
	lbux	r19,r6(r7)
	addiu	r19,r19,00000020

l004052FC:
	sll	r17,r19,00000004
	lw	r7,0003(r22)
	addiu	r18,r17,00000008
	addiupc	r16,0002D24A
	addu	r30,r16,r18
	bnec	r0,r7,0040543E

l0040530E:
	addu	r17,r16,r17
	addu	r7,r16,r18
	lw	r9,0010(r17)
	beqc	r0,r9,0040546C

l0040531A:
	bnec	r7,r9,00405514

l0040531E:
	lwx	r7,r18(r16)
	beqzc	r7,004052C2

l00405324:
	sync	00000000
	swx	r0,r18(r16)
	sync	00000000
	lw	r7,0004(r30)
	beqzc	r7,004052C2

l00405336:
	li	r7,00000001
	addiu	r6,r0,00000081
	move	r5,r30
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,004052C2

l0040534C:
	li	r7,00000001
	li	r6,00000001
	move	r5,r30
	li	r4,00000062
	balc	00404A50
	lw	r4,0550(r23)
	lw	r5,0554(r23)
	and	r7,r4,r21
	and	r6,r5,r20
	or	r5,r7,r6
	bnec	r0,r5,004052DA

l00405370:
	lw	r17,0008(sp)
	aluipc	r17,00000405
	lw	r7,0003(r22)
	addiu	r6,r6,00000010
	sw	r6,001C(sp)
	bnec	r0,r7,004055B4

l00405380:
	addiu	r4,sp,0000001C
	balc	0040CDF0
	move	r16,r4
	beqc	r0,r4,004056EA

l0040538C:
	lwpc	r18,00432540
	lw	r17,001C(sp)
	beqc	r18,r4,004053A2

l00405396:
	addiu	r7,r7,FFFFFFF0
	li	r6,00000001
	addiu	r18,r4,00000010
	sw	r6,0008(sp)
	sw	r7,001C(sp)

l004053A2:
	lw	r5,0544(r17)
	addu	r6,r18,r7
	li	r4,00000001
	ori	r7,r7,00000001
	sw	r7,-0008(r6)
	swpc	r6,00432540
	sw	r4,-0004(r6)
	addiu	r9,r18,FFFFFFF8
	sw	r7,-0004(r18)
	beqzc	r5,004053DC

l004053C6:
	sync	00000000
	sw	r0,0544(r17)
	sync	00000000
	addiupc	r7,0002D16E
	lw	r7,0004(r7)
	bnec	r0,r7,00405678

l004053DC:
	beqc	r0,r9,004056D2

l004053E0:
	sw	r9,000C(sp)
	move.balc	r4,r9,00404D54
	lw	r18,000C(sp)
	beqc	r0,r4,00405636

l004053EC:
	lw	r6,-0008(r18)
	lw	r5,-0004(r18)
	ins	r6,r0,00000000,00000001
	subu	r6,r9,r6
	move	r4,r5
	lw	r7,0004(r6)
	ins	r4,r0,00000000,00000001
	ins	r7,r0,00000000,00000001
	addu	r7,r7,r5
	sw	r7,0004(sp)
	swx	r7,r4(r9)
	move	r9,r6

l00405412:
	ins	r7,r0,00000000,00000001
	lw	r17,0008(sp)
	addiu	r6,r7,FFFFFFF0
	bltuc	r5,r6,004055CE

l00405420:
	addiu	r16,r9,00000008

l00405424:
	move	r4,r16
	restore.jrc	00000050,r30,0000000A

l00405428:
	sync	00000000
	beqc	r0,r6,0040530E

l00405430:
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r30,00000004
	move	r4,r30
	balc	0040ADB0

l0040543E:
	sync	00000000

l00405442:
	ll	r6,0000(r30)
	li	r7,00000001
	sc	r7,0000(r30)
	beqzc	r7,00405442

l0040544E:
	bc	00405428

l00405450:
	subu	r6,r0,r7
	and	r7,r7,r6
	li	r6,076BE629
	mul	r7,r7,r6
	addiupc	r6,0000D85E
	srl	r7,r7,0000001B
	lbux	r19,r6(r7)
	bc	004052FC

l0040546C:
	sw	r7,0050(sp)
	sw	r7,0054(sp)
	bc	0040531E

l00405472:
	addiu	r4,r4,00000017
	lui	r7,0000001C
	ins	r4,r0,00000000,00000001
	sw	r4,0008(sp)
	bgeuc	r7,r4,004054BA

l00405484:
	addiu	r16,r6,00000007
	subu	r6,r0,r6
	addu	r16,r16,r4
	addiu	r7,r0,00000802
	and	r16,r16,r6
	move	r10,r0
	move	r11,r0
	addiu	r8,r0,FFFFFFFF
	li	r6,00000003
	movep	r4,r5,r0,r16
	balc	00405BE2
	li	r7,FFFFFFFF
	beqc	r4,r7,004056D2

l004054AA:
	addiu	r6,r16,FFFFFFF8
	li	r7,00000008
	addiu	r16,r4,00000010
	sw	r7,0008(sp)
	sw	r6,000C(sp)
	move	r4,r16
	restore.jrc	00000050,r30,0000000A

l004054BA:
	srl	r7,r4,00000004
	addiu	r30,r7,FFFFFFFF
	bltiuc	r30,00000021,00405790

l004054C4:
	addiu	r7,r7,FFFFFFFE
	addiu	r6,r0,000001FF
	bltuc	r6,r7,004056A6

l004054CE:
	srl	r7,r7,00000003
	addiupc	r6,0000D80C
	addu	r7,r7,r6
	li	r5,00000001
	lbu	r30,-0004(r7)
	addiu	r7,r30,00000001

l004054E0:
	move	r4,r7
	sw	r7,000C(sp)

l004054E4:
	subu	r7,r0,r7
	sllv	r6,r5,r4
	srlv	r7,r5,r7
	slti	r5,r4,00000020
	movz	r7,r0,r4

l004054F8:
	movz	r7,r6,r5

l004054FC:
	movz	r6,r0,r5

l00405500:
	subu	r6,r0,r6
	subu	r20,r0,r7
	sltu	r7,r0,r6
	move	r21,r6
	subu	r20,r20,r7
	bc	004052BE

l00405514:
	lw	r17,0002(r9)
	lw	r5,0003(r9)
	bltic	r19,00000028,00405610

l0040551C:
	lw	r17,000C(sp)
	addiu	r7,r7,00000002
	bgec	r7,r19,004055FA

l00405524:
	lw	r6,0001(r9)
	lw	r17,0008(sp)
	ins	r6,r0,00000000,00000001
	subu	r18,r6,r7

l0040552E:
	srl	r7,r18,00000004
	addiu	r7,r7,FFFFFFFF
	bltiuc	r7,00000021,00405610

l00405536:
	addiu	r4,r0,000001FF
	bltuc	r4,r7,004056BA

l0040553E:
	srl	r7,r7,00000003
	addiupc	r4,0000D79C
	addu	r7,r7,r4
	lbu	r7,-0004(r7)

l0040554A:
	bnec	r7,r19,00405610

l0040554E:
	lw	r4,0008(sp)
	addu	r4,r9,r16
	ori	r7,r16,00000001
	sw	r17,0008(sp)
	sw	r5,000C(sp)
	sw	r4,0048(sp)
	lw	r5,0008(r4)
	sw	r4,004C(sp)
	swx	r7,r16(r9)
	sw	r18,0004(sp)
	swx	r18,r6(r9)
	sw	r7,0001(r9)

l0040556E:
	lw	r6,0000(r30)
	beqc	r0,r6,00405412

l00405576:
	sync	00000000
	sw	r0,0000(r30)
	sync	00000000
	lw	r7,0004(r30)
	bnec	r0,r7,0040564A

l0040558A:
	lw	r7,0001(r9)

l0040558C:
	ins	r7,r0,00000000,00000001
	lw	r17,0008(sp)
	addiu	r6,r7,FFFFFFF0
	bgeuc	r5,r6,00405420

l0040559A:
	bc	004055CE

l0040559C:
	sync	00000000
	beqc	r0,r6,00405380

l004055A4:
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,0002CF9C
	addiupc	r4,0002CF94
	balc	0040ADB0

l004055B4:
	sync	00000000

l004055B8:
	ori	r7,r17,00000544
	ll	r6,0000(r7)
	li	r7,00000001
	ori	r5,r17,00000544
	sc	r7,0000(r5)
	beqzc	r7,004055B8

l004055CC:
	bc	0040559C

l004055CE:
	move	r4,r5
	subu	r6,r7,r5
	addu	r16,r9,r5
	ori	r6,r6,00000001
	ori	r5,r5,00000001
	sw	r9,0008(sp)
	swx	r5,r4(r9)
	addiu	r4,r16,00000008
	sw	r6,0004(sp)
	swx	r6,r7(r9)
	sw	r5,0001(r9)
	balc	00404F2E
	lw	r18,0008(sp)
	addiu	r16,r9,00000008
	bc	00405424

l004055FA:
	bneiuc	r19,0000003F,00405610

l004055FE:
	lw	r6,0001(r9)
	lui	r7,0000001C
	lw	r17,0008(sp)
	ins	r6,r0,00000000,00000001
	subu	r18,r6,r4
	bltuc	r7,r18,0040552E

l00405610:
	beqc	r17,r5,0040572E

l00405614:
	lw	r7,0001(r9)
	move	r4,r7
	ori	r7,r7,00000001
	ins	r4,r0,00000000,00000001
	lwx	r6,r4(r9)
	sw	r17,0048(sp)
	lw	r16,0002(r9)
	ori	r6,r6,00000001
	sw	r5,000C(sp)
	sw	r7,0001(r9)
	swx	r6,r4(r9)
	bc	0040556E

l00405636:
	lw	r7,-0004(r18)
	lw	r17,0008(sp)
	ins	r7,r0,00000000,00000001
	addiu	r6,r7,FFFFFFF0
	bgeuc	r5,r6,00405420

l00405648:
	bc	004055CE

l0040564A:
	li	r7,00000001
	addiu	r6,r0,00000081
	move	r5,r30
	li	r4,00000062
	sw	r9,000C(sp)
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	lw	r18,000C(sp)
	bnec	r7,r4,0040558A

l00405664:
	li	r7,00000001
	li	r6,00000001
	move	r5,r30
	li	r4,00000062
	sw	r9,000C(sp)
	balc	00404A50
	lw	r18,000C(sp)
	lw	r7,0001(r9)
	bc	0040558C

l00405678:
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,0002CEC2
	li	r4,00000062
	sw	r9,000C(sp)
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	lw	r18,000C(sp)
	bnec	r7,r4,004053DC

l00405694:
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,0002CEA8
	li	r4,00000062
	balc	00404A50
	lw	r18,000C(sp)
	bc	004053DC

l004056A6:
	srl	r7,r7,00000007
	addiupc	r6,0000D634
	addu	r7,r7,r6
	li	r5,00000001
	lbu	r30,-0004(r7)
	addiu	r7,r30,00000011
	bc	004054E0

l004056BA:
	addiu	r4,r0,00001C00
	bltuc	r4,r7,004056D8

l004056C2:
	srl	r7,r7,00000007
	addiupc	r4,0000D618
	addu	r7,r7,r4
	lbu	r7,-0004(r7)
	addiu	r7,r7,00000010
	bc	0040554A

l004056D2:
	move	r16,r0
	move	r4,r16
	restore.jrc	00000050,r30,0000000A

l004056D8:
	li	r7,0000003F
	bc	0040554A

l004056DC:
	balc	004049B0
	move	r16,r0
	li	r7,0000000C
	sw	r7,0000(sp)
	move	r4,r16
	restore.jrc	00000050,r30,0000000A

l004056EA:
	lw	r7,0544(r17)
	beqzc	r7,004056D2

l004056F0:
	sync	00000000
	sw	r0,0544(r17)
	sync	00000000
	addiupc	r7,0002CE44
	lw	r7,0004(r7)
	beqzc	r7,004056D2

l00405704:
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,0002CE36
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,004056D2

l0040571C:
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,0002CE20
	li	r4,00000062
	balc	00404A50
	move	r4,r16
	restore.jrc	00000050,r30,0000000A

l0040572E:
	li	r6,00000001
	subu	r7,r0,r19
	srlv	r7,r6,r7
	slti	r5,r19,00000020
	sllv	r6,r6,r19
	movz	r7,r0,r19

l00405744:
	movz	r7,r6,r5

l00405748:
	movz	r6,r0,r5

l0040574C:
	nor	r7,r0,r7
	nor	r5,r0,r6
	beqzc	r6,00405772

l00405756:
	sync	00000000

l0040575A:
	ori	r6,r23,00000550
	ll	r6,0000(r6)
	and	r6,r6,r5
	ori	r4,r23,00000550
	sc	r6,0000(r4)
	beqzc	r6,0040575A

l0040576E:
	sync	00000000

l00405772:
	li	r6,FFFFFFFF
	beqc	r6,r7,0040578A

l00405776:
	sync	00000000

l0040577A:
	ll	r6,0004(r16)
	and	r6,r6,r7
	sc	r6,0004(r16)
	beqzc	r6,0040577A

l00405786:
	sync	00000000

l0040578A:
	lw	r17,0002(r9)
	lw	r5,0003(r9)
	bc	00405614

l00405790:
	move	r4,r30
	li	r5,00000001
	move	r7,r30
	sw	r30,000C(sp)
	bc	004054E4

;; __malloc0: 0040579A
;;   Called from:
;;     0040CDD2 (in calloc)
__malloc0 proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	00405292
	beqzc	r4,004057CE

l004057A4:
	lw	r7,-0004(r4)
	bbeqzc	r7,00000000,004057CE

l004057AC:
	addiu	r5,r16,00000003
	srl	r5,r5,00000002
	beqzc	r5,004057CE

l004057B4:
	lsa	r5,r5,r4,00000002
	move	r7,r4

l004057BA:
	lw	r6,0000(r7)
	beqzc	r6,004057C8

l004057BE:
	sw	r0,0040(sp)
	addiu	r7,r7,00000004
	bnec	r7,r5,004057BA

l004057C6:
	restore.jrc	00000010,ra,00000002

l004057C8:
	addiu	r7,r7,00000004
	bnec	r7,r5,004057BA

l004057CE:
	restore.jrc	00000010,ra,00000002

;; realloc: 004057D0
;;   Called from:
;;     0040D9FC (in __isoc99_vfscanf)
;;     0040DA5E (in __isoc99_vfscanf)
realloc proc
	beqc	r0,r4,00405918

l004057D4:
	save	00000030,ra,00000009
	addiupc	r23,0004EC56
	move	r16,r4
	lw	r4,0024(r23)
	li	r7,7FFFFFEF
	addiu	r6,r5,FFFFFFFF
	subu	r7,r7,r4
	bltuc	r7,r6,00405822

l004057F0:
	addiu	r5,r5,00000017
	ins	r5,r0,00000000,00000001
	move	r19,r5

l004057FA:
	lw	r7,-0004(r16)
	addiu	r21,r16,FFFFFFF8
	move	r17,r7
	ins	r17,r0,00000000,00000001
	bbnezc	r7,00000000,00405868

l0040580C:
	lw	r22,-0008(r16)
	addu	r17,r17,r22
	addu	r20,r22,r19
	bbeqzc	r22,00000000,00405834

l0040581A:
	sb	r0,0000(r0)
	teq	r0,r0,00000000

l00405822:
	li	r19,00000010
	beqzc	r5,004057FA

l00405826:
	balc	004049B0
	li	r7,0000000C
	move	r18,r0
	sw	r7,0000(sp)

l00405830:
	move	r4,r18
	restore.jrc	00000030,ra,00000009

l00405834:
	bltuc	r20,r4,00405900

l00405838:
	addiu	r20,r20,FFFFFFFF
	subu	r7,r0,r4
	addu	r4,r4,r20
	and	r20,r7,r4
	beqc	r17,r20,00405896

l00405848:
	li	r7,00000001
	subu	r4,r21,r22
	movep	r5,r6,r17,r20
	balc	00405C52
	li	r7,FFFFFFFF
	beqc	r4,r7,00405932

l0040585A:
	addu	r4,r4,r22
	subu	r22,r20,r22
	addiu	r18,r4,00000008
	sw	r22,0001(r4)
	move	r4,r18
	restore.jrc	00000030,ra,00000009

l00405868:
	lwx	r6,r17(r21)
	addu	r18,r21,r17
	beqc	r6,r7,0040587A

l00405872:
	sb	r0,0000(r0)
	teq	r0,r0,00000000

l0040587A:
	bltuc	r17,r19,004058C4

l0040587E:
	ori	r7,r17,00000001
	sw	r7,-0004(r16)
	swx	r7,r17(r21)

l0040588A:
	ins	r7,r0,00000000,00000001
	addiu	r6,r7,FFFFFFF0
	bltuc	r19,r6,0040589C

l00405896:
	move	r18,r16

l00405898:
	move	r4,r18
	restore.jrc	00000030,ra,00000009

l0040589C:
	subu	r6,r7,r19
	ori	r5,r19,00000001
	ori	r6,r6,00000001
	addu	r17,r21,r19
	swx	r5,r19(r21)
	addiu	r4,r17,00000008
	sw	r6,0044(sp)
	move	r18,r16
	swx	r6,r7(r21)
	sw	r5,-0004(r16)
	balc	00404F2E
	move	r4,r18
	restore.jrc	00000030,ra,00000009

l004058C4:
	move.balc	r4,r18,00404B80
	beqzc	r4,0040591E

l004058CA:
	lw	r6,0004(r18)
	ins	r6,r0,00000000,00000001
	addu	r5,r17,r6
	ori	r7,r5,00000001
	sw	r7,-0004(r16)
	swx	r7,r6(r18)
	bgeuc	r5,r19,0040588A

l004058E2:
	addiu	r4,r19,FFFFFFF8
	balc	00405292
	move	r18,r4
	beqc	r0,r4,00405830

l004058F0:
	addiu	r6,r17,FFFFFFF8
	move.balc	r5,r16,0040A130
	move.balc	r4,r16,00404F2E
	move	r4,r18
	restore.jrc	00000030,ra,00000009

l00405900:
	move.balc	r4,r19,00405292
	move	r18,r4
	beqzc	r4,0040592C

l00405908:
	addiu	r6,r19,FFFFFFF8
	move.balc	r5,r16,0040A130
	move.balc	r4,r16,00404F2E
	move	r4,r18
	restore.jrc	00000030,ra,00000009

l00405918:
	move	r4,r5
	bc	00405292

l0040591E:
	ori	r7,r17,00000001
	sw	r7,-0004(r16)
	swx	r7,r17(r21)
	bc	004058E2

l0040592C:
	lw	r4,0024(r23)
	bc	00405838

l00405932:
	move	r18,r0
	bgeuc	r20,r17,00405830

l00405938:
	move	r18,r16
	bc	00405898
0040593C                                     00 00 00 00             ....

;; __getopt_msg: 00405940
;;   Called from:
;;     00405AB4 (in __posix_getopt)
__getopt_msg proc
	save	00000020,ra,00000006
	move	r20,r4
	lwpc	r16,00412EF0
	movep	r19,r17,r6,r7
	move.balc	r4,r5,00404A96
	move	r18,r4
	move.balc	r4,r16,00408310
	movep	r4,r5,r20,r16
	balc	004083F0
	bltc	r4,r0,00405980

l00405960:
	move.balc	r4,r18,0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r18,0040857C
	beqzc	r4,00405980

l00405970:
	movep	r6,r7,r17,r16
	li	r5,00000001
	move.balc	r4,r19,0040857C
	bnec	r17,r4,00405980

l0040597A:
	li	r4,0000000A
	move.balc	r5,r16,00408700

l00405980:
	move	r4,r16
	restore	00000020,ra,00000006
	bc	004084E0

;; __posix_getopt: 0040598A
;;   Called from:
;;     004001B6 (in main)
__posix_getopt proc
	save	00000040,ra,00000006
	lwpc	r7,00430254
	movep	r19,r18,r4,r5
	move	r17,r6
	beqzc	r7,004059A0

l00405998:
	lwpc	r7,00432960
	beqzc	r7,004059B4

l004059A0:
	li	r7,00000001
	swpc	r0,00432960
	swpc	r0,004544A4
	swpc	r7,00430254

l004059B4:
	lwpc	r7,00430254
	li	r4,FFFFFFFF
	bgec	r7,r19,00405B00

l004059C0:
	lwxs	r5,r7(r18)
	beqc	r0,r5,00405B00

l004059C6:
	lbu	r6,0000(r5)
	beqic	r6,0000002D,004059E4

l004059CC:
	lbu	r6,0000(r17)
	bneiuc	r6,0000002D,00405B00

l004059D2:
	addiu	r7,r7,00000001
	swpc	r5,004544E8
	swpc	r7,00430254
	li	r4,00000001
	restore.jrc	00000040,ra,00000006

l004059E4:
	lbu	r6,0001(r5)
	li	r4,FFFFFFFF
	beqc	r0,r6,00405B00

l004059EC:
	bneiuc	r6,0000002D,004059FE

l004059F0:
	lbu	r6,0002(r5)
	bnezc	r6,004059FE

l004059F4:
	addiu	r7,r7,00000001
	swpc	r7,00430254
	restore.jrc	00000040,ra,00000006

l004059FE:
	lwpc	r7,004544A4
	bnezc	r7,00405A0E

l00405A06:
	li	r7,00000001
	swpc	r7,004544A4

l00405A0E:
	lwpc	r7,004544A4
	li	r6,00000004
	addu	r5,r5,r7
	addiu	r4,sp,00000018
	balc	00405CE0
	move	r7,r4
	bgec	r4,r0,00405A2C

l00405A24:
	addiu	r7,r0,0000FFFD
	sw	r7,0018(sp)
	li	r7,00000001

l00405A2C:
	lwpc	r5,00430254
	lwxs	r4,r5(r18)
	lwpc	r6,004544A4
	addu	r20,r4,r6
	addu	r6,r7,r6
	swpc	r6,004544A4
	lbux	r6,r6(r4)
	bnezc	r6,00405A5A

l00405A4C:
	addiu	r5,r5,00000001
	swpc	r0,004544A4
	swpc	r5,00430254

l00405A5A:
	lbu	r6,0000(r17)
	addiu	r6,r6,FFFFFFD5
	andi	r6,r6,000000FD
	bnezc	r6,00405A68

l00405A66:
	addiu	r17,r17,00000001

l00405A68:
	move	r16,r0
	sw	r0,001C(sp)
	bc	00405A74

l00405A6E:
	addiu	r16,r16,00000001
	beqzc	r4,00405A8E

l00405A72:
	beqc	r5,r6,00405A8E

l00405A74:
	li	r6,00000004
	addu	r5,r17,r16
	addiu	r4,sp,0000001C
	sw	r7,000C(sp)
	balc	00405CE0
	lw	r17,000C(sp)
	lw	r17,0018(sp)
	lw	r17,001C(sp)
	bgec	r0,r4,00405A6E

l00405A8A:
	addu	r16,r16,r4
	bc	00405A72

l00405A8E:
	move	r4,r6
	beqc	r5,r6,00405ABA

l00405A94:
	swpc	r6,004544F0
	lbu	r6,0000(r17)
	bneiuc	r6,0000003A,00405AA4

l00405AA0:
	li	r4,0000003F
	restore.jrc	00000040,ra,00000006

l00405AA4:
	lwpc	r6,00430250
	beqzc	r6,00405AA0

l00405AAC:
	move	r6,r20
	addiupc	r5,0000C40A

l00405AB2:
	lw	r4,0000(r18)
	balc	00405940
	bc	00405AA0

l00405ABA:
	lbux	r6,r16(r17)
	bneiuc	r6,0000003A,00405B00

l00405AC2:
	addiu	r16,r16,00000001
	addu	r16,r17,r16
	lbu	r5,0000(r16)
	bneiuc	r5,0000003A,00405B02

l00405ACC:
	swpc	r0,004544E8

l00405AD2:
	lbu	r7,0000(r16)
	lwpc	r6,004544A4
	bneiuc	r7,0000003A,00405AE0

l00405ADE:
	beqzc	r6,00405B00

l00405AE0:
	lwpc	r7,00430254
	addiu	r5,r7,00000001
	lwxs	r7,r7(r18)
	swpc	r5,00430254
	addu	r7,r7,r6
	swpc	r0,004544A4
	swpc	r7,004544E8

l00405B00:
	restore.jrc	00000040,ra,00000006

l00405B02:
	lwpc	r5,00430254
	bltc	r5,r19,00405AD2

l00405B0C:
	swpc	r4,004544F0
	move	r4,r6
	lbu	r5,0000(r17)
	beqic	r5,0000003A,00405B00

l00405B1A:
	lwpc	r6,00430250
	beqc	r0,r6,00405AA0

l00405B24:
	move	r6,r20
	addiupc	r5,0000C3AA
	bc	00405AB2
00405B2C                                     00 00 00 00             ....

;; getrlimit64: 00405B30
;;   Called from:
;;     0040476A (in sysconf)
getrlimit64 proc
	save	00000010,ra,00000002
	move	r16,r5
	movep	r7,r8,r0,r16
	movep	r5,r6,r0,r4
	addiu	r4,r0,00000105
	balc	00404A50
	balc	0040CC30
	bnezc	r4,00405B76

l00405B46:
	lw	r7,0004(r16)
	bnezc	r7,00405B56

l00405B4A:
	lw	r7,0000(r16)
	li	r6,7FFFFFFE
	bgeuc	r6,r7,00405B5E

l00405B56:
	li	r6,FFFFFFFF
	li	r7,FFFFFFFF
	swm	r6,0000(r16),00000002

l00405B5E:
	lw	r7,000C(r16)
	bnezc	r7,00405B6E

l00405B62:
	lw	r7,0008(r16)
	li	r6,7FFFFFFE
	bgeuc	r6,r7,00405B76

l00405B6E:
	li	r6,FFFFFFFF
	li	r7,FFFFFFFF
	swm	r6,0008(r16),00000002

l00405B76:
	restore.jrc	00000010,ra,00000002
00405B78                         00 00 00 00 00 00 00 00         ........

;; ioctl: 00405B80
;;   Called from:
;;     00400CFA (in ping4_run)
;;     00400E62 (in ping4_run)
;;     004023C4 (in setup)
;;     00402C8A (in main_loop)
;;     00406730 (in if_indextoname)
;;     0040678A (in if_nametoindex)
ioctl proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000001
	swm	r6,0028(sp),00000002
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	swm	r8,0030(sp),00000002
	sw	r7,0004(sp)
	swm	r10,0038(sp),00000002
	sw	r7,0008(sp)
	li	r7,00000040
	sb	r7,000D(sp)
	li	r7,00000014
	sb	r7,000C(sp)
	move	r7,r6
	move	r6,r5
	move	r5,r4
	li	r4,0000001D
	balc	00404A50
	balc	0040CC30
	restore	00000020,ra,00000001
	addiu	sp,sp,00000020
	jrc	ra

;; madvise: 00405BC0
;;   Called from:
;;     00405212 (in free)
madvise proc
	save	00000010,ra,00000001
	move	r7,r6
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000E9
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
00405BD8                         00 00 00 00 00 00 00 00         ........

;; __vm_wait: 00405BE0
;;   Called from:
;;     00405C22 (in mmap64)
;;     00405C88 (in mremap)
;;     00405CC6 (in munmap)
__vm_wait proc
	jrc	ra

;; mmap64: 00405BE2
;;   Called from:
;;     004054A0 (in malloc)
;;     0040CE66 (in __expand_heap)
mmap64 proc
	save	00000020,ra,00000008
	movep	r21,r19,r6,r7
	move	r7,r11
	move	r16,r10
	ins	r7,r0,00000000,00000001
	andi	r10,r10,00000FFF
	or	r10,r10,r7
	move	r22,r8
	movep	r20,r18,r4,r5
	move	r17,r11
	beqc	r0,r10,00405C0C

l00405C00:
	balc	004049B0
	li	r7,00000016

l00405C06:
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000020,ra,00000008

l00405C0C:
	li	r7,7FFFFFFE
	bgeuc	r7,r18,00405C1E

l00405C16:
	balc	004049B0
	li	r7,0000000C
	bc	00405C06

l00405C1E:
	bbeqzc	r19,00000004,00405C26

l00405C22:
	balc	00405BE0

l00405C26:
	sll	r10,r17,00000014
	srl	r16,r16,0000000C
	or	r10,r10,r16
	move	r9,r22
	movep	r7,r8,r21,r19
	movep	r5,r6,r20,r18
	addiu	r4,r0,000000DE
	balc	00404A50
	restore	00000020,ra,00000008
	bc	0040CC30
00405C48                         00 00 00 00 00 00 00 00         ........

;; dummy: 00405C50
dummy proc
	jrc	ra

;; mremap: 00405C52
;;   Called from:
;;     00405850 (in realloc)
mremap proc
	addiu	sp,sp,FFFFFFF0
	save	00000030,ra,00000005
	movep	r16,r17,r6,r7
	swm	r8,0030(sp),00000002
	li	r7,7FFFFFFE
	swm	r10,0038(sp),00000002
	movep	r18,r19,r4,r5
	bgeuc	r7,r16,00405C82

l00405C6E:
	balc	004049B0
	li	r7,0000000C
	sw	r7,0000(sp)
	li	r4,FFFFFFFF

l00405C78:
	restore	00000030,ra,00000005
	addiu	sp,sp,00000010
	jrc	ra

l00405C82:
	move	r9,r0
	bbeqzc	r17,00000001,00405CA2

l00405C88:
	balc	00405BE0
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	sw	r7,0004(sp)
	sw	r7,0008(sp)
	li	r7,00000040
	sb	r7,000D(sp)
	li	r7,0000000C
	lw	r18,0030(sp)
	sb	r7,000C(sp)

l00405CA2:
	movep	r7,r8,r16,r17
	movep	r5,r6,r18,r19
	addiu	r4,r0,000000D8
	balc	00404A50
	balc	0040CC30
	bc	00405C78
00405CB4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; dummy: 00405CC0
dummy proc
	jrc	ra

;; munmap: 00405CC2
;;   Called from:
;;     00404F72 (in free)
munmap proc
	save	00000010,ra,00000003
	movep	r16,r17,r4,r5
	balc	00405BE0
	addiu	r4,r0,000000D7
	movep	r5,r6,r16,r17
	balc	00404A50
	restore	00000010,ra,00000003
	bc	0040CC30
00405CDC                                     00 00 00 00             ....

;; mbtowc: 00405CE0
;;   Called from:
;;     00405A1A (in __posix_getopt)
;;     00405A7C (in __posix_getopt)
mbtowc proc
	save	00000020,ra,00000001
	move	r7,r0
	beqzc	r5,00405CFE

l00405CE6:
	beqc	r0,r6,00405D9E

l00405CEA:
	bnezc	r4,00405CEE

l00405CEC:
	addiu	r4,sp,0000000C

l00405CEE:
	lbu	r7,0000(r5)
	seb	r8,r7
	bltc	r8,r0,00405D02

l00405CF8:
	sw	r7,0000(sp)
	sltu	r7,r0,r7

l00405CFE:
	move	r4,r7
	restore.jrc	00000020,ra,00000001

l00405D02:
	rdhwr	r3,0000001D,00000000
	lw	r8,-0038(r3)
	lw	r8,0000(r8)
	bnec	r0,r8,00405D1E

l00405D10:
	lb	r7,0000(r5)
	addiu	r6,r0,0000DFFF
	and	r7,r7,r6
	sw	r7,0000(sp)
	li	r7,00000001
	bc	00405CFE

l00405D1E:
	addiu	r7,r7,FFFFFF3E
	bgeiuc	r7,00000033,00405D9E

l00405D26:
	addiupc	r8,0000D646
	lwxs	r7,r7(r8)
	bgeiuc	r6,00000004,00405D42

l00405D32:
	lsa	r6,r6,r6,00000001
	sll	r6,r6,00000001
	addiu	r6,r6,FFFFFFFA
	sllv	r6,r7,r6
	bltc	r6,r0,00405D9E

l00405D42:
	lbu	r8,0001(r5)
	sra	r9,r7,0000001A
	srl	r6,r8,00000003
	addu	r9,r9,r6
	addiu	r6,r6,FFFFFFF0
	or	r9,r9,r6
	ins	r9,r0,00000000,00000001
	bnec	r0,r9,00405D9E

l00405D60:
	sll	r7,r7,00000006
	addiu	r6,r8,FFFFFF80
	or	r6,r6,r7
	bltc	r6,r0,00405D72

l00405D6C:
	li	r7,00000002
	sw	r6,0000(sp)
	bc	00405CFE

l00405D72:
	lbu	r7,0002(r5)
	addiu	r7,r7,FFFFFF80
	bgeiuc	r7,00000000,00405D9E

l00405D7C:
	sll	r6,r6,00000006
	or	r7,r7,r6
	bltc	r7,r0,00405D8A

l00405D84:
	sw	r7,0000(sp)
	li	r7,00000003
	bc	00405CFE

l00405D8A:
	lbu	r6,0003(r5)
	addiu	r6,r6,FFFFFF80
	bgeiuc	r6,00000000,00405D9E

l00405D94:
	sll	r7,r7,00000006
	or	r7,r7,r6
	sw	r7,0000(sp)
	li	r7,00000004
	bc	00405CFE

l00405D9E:
	balc	004049B0
	li	r7,00000054
	sw	r7,0000(sp)
	li	r7,FFFFFFFF
	bc	00405CFE
00405DAA                               00 00 00 00 00 00           ......

;; bind: 00405DB0
;;   Called from:
;;     00400F42 (in ping4_run)
;;     00403982 (in ping6_run)
;;     0040783A (in __res_msend_rc)
bind proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000C8
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
00405DCC                                     00 00 00 00             ....

;; connect: 00405DD0
;;   Called from:
;;     00400D7A (in ping4_run)
;;     00400DCA (in ping4_run)
;;     004037B0 (in ping6_run)
;;     004072B2 (in __lookup_name)
connect proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000CB
	balc	0040ADA4
	restore	00000010,ra,00000001
	bc	0040CC30
00405DEC                                     00 00 00 00             ....

;; freeaddrinfo: 00405DF0
;;   Called from:
;;     00400814 (in main)
;;     00400BFE (in ping4_run)
;;     004033A0 (in niquery_option_subject_addr_handler)
;;     00403660 (in ping6_run)
freeaddrinfo proc
	bc	00404F2E
00405DF4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; gai_strerror: 00405E00
;;   Called from:
;;     004007E6 (in main)
;;     00400BBC (in ping4_run)
;;     00403366 (in niquery_option_subject_addr_handler)
;;     00403612 (in ping6_run)
gai_strerror proc
	addiu	r7,r4,00000001
	addiupc	r4,0000CF14

l00405E08:
	lbu	r6,0000(r4)
	bnezc	r7,00405E14

l00405E0C:
	bnezc	r6,00405E10

l00405E0E:
	addiu	r4,r4,00000001

l00405E10:
	bc	00404A96

l00405E14:
	beqzc	r6,00405E0E

l00405E16:
	lbu	r6,0000(r4)
	addiu	r4,r4,00000001
	bnezc	r6,00405E16

l00405E1C:
	addiu	r7,r7,00000001
	bc	00405E08

;; getaddrinfo: 00405E20
;;   Called from:
;;     004007DC (in main)
;;     00400BB6 (in ping4_run)
;;     0040335C (in niquery_option_subject_addr_handler)
;;     00403608 (in ping6_run)
getaddrinfo proc
	save	000006B0,r30,0000000A
	move	r18,r4
	sw	r7,001C(sp)
	bnezc	r4,00405E30

l00405E2A:
	addiu	r30,r0,FFFFFFFE
	beqzc	r5,00405E58

l00405E30:
	beqzc	r6,00405E5E

l00405E32:
	lwm	r16,0000(r6),00000002
	lw	r7,0008(r6)
	lw	r19,000C(r6)
	andi	r6,r16,0000043F
	addiu	r30,r0,FFFFFFFF
	bnec	r16,r6,00405E58

l00405E44:
	addiu	r30,r0,FFFFFFFA
	bgeiuc	r17,0000000B,00405E58

l00405E4C:
	li	r4,00000001
	sllv	r4,r4,r17
	andi	r4,r4,00000405
	bnezc	r4,00405E66

l00405E58:
	move	r4,r30
	restore.jrc	000006B0,r30,0000000A

l00405E5E:
	move	r7,r0
	move	r19,r0
	move	r16,r0
	move	r17,r0

l00405E66:
	move	r8,r16
	move	r6,r19
	addiu	r4,sp,00000038
	balc	004073A0
	move	r30,r4
	bltc	r4,r0,00405E58

l00405E76:
	movep	r7,r8,r17,r16
	move	r6,r18
	addiu	r5,sp,00000040
	addiu	r4,sp,00000140
	balc	00406FCE
	move	r19,r4
	bltc	r4,r0,00405FAA

l00405E8A:
	mul	r16,r30,r4
	addiu	r4,sp,00000040
	balc	0040A890
	li	r7,0000003C
	move	r17,r4
	mul	r16,r16,r7
	addiu	r5,r16,00000001
	addu	r5,r5,r4
	li	r4,00000001
	balc	0040CDC0
	move	r18,r4
	beqc	r0,r4,00405FAE

l00405EAC:
	move	r22,r0
	beqzc	r17,00405EBE

l00405EB0:
	addu	r22,r4,r16
	addiu	r6,r17,00000001
	addiu	r5,sp,00000040
	move.balc	r4,r22,0040A130

l00405EBE:
	li	r7,0000003C
	addiu	r17,sp,00000148
	mul	r7,r30,r7
	move	r23,r0
	sw	r18,0010(sp)
	sw	r7,0018(sp)

l00405ECE:
	beqc	r23,r19,00405F9C

l00405ED2:
	addiu	r7,sp,00000038
	lw	r5,0010(sp)
	move	r21,r7
	sw	r0,000C(sp)
	bc	00405F48

l00405EDC:
	lw	r10,-0008(r17)
	li	r6,0000001C
	li	r7,00000010
	lbu	r3,0003(r21)
	xori	r11,r10,00000002
	lbu	r2,0002(r21)
	movn	r7,r6,r11

l00405EF4:
	li	r6,00000020
	move	r11,r7
	addiu	r7,r20,0000003C
	movep	r4,r5,r20,r0
	swm	r2,0028(sp),00000002
	sw	r7,0014(sp)
	sw	r11,0020(sp)
	sw	r10,0024(sp)
	balc	0040A690
	lw	r18,0020(sp)
	addiu	r6,r20,00000020
	lw	r18,0024(sp)
	addiu	r7,r20,0000003C
	lwm	r2,0028(sp),00000002
	sw	r11,0010(r20)
	sw	r10,0001(r20)
	sw	r3,0008(r20)
	sw	r2,000C(r20)
	sw	r6,0014(r20)
	sw	r22,0018(r20)
	sw	r7,001C(r20)
	beqic	r10,00000002,00405F5C

l00405F3A:
	beqic	r10,0000000A,00405F7A

l00405F3E:
	lw	r17,000C(sp)
	addiu	r21,r21,00000004
	lw	r5,0014(sp)
	addiu	r7,r7,00000001
	sw	r7,000C(sp)

l00405F48:
	lw	r17,000C(sp)
	bnec	r7,r30,00405EDC

l00405F4E:
	lw	r17,0010(sp)
	addiu	r23,r23,00000001
	lw	r17,0018(sp)
	addiu	r17,r17,0000001C
	addu	r7,r7,r6
	sw	r7,0010(sp)
	bc	00405ECE

l00405F5C:
	sh	r10,0020(r20)
	lhu	r4,0000(r21)
	balc	00406700
	li	r6,00000004
	sh	r4,0022(r20)
	move	r5,r17
	addiu	r4,r20,00000024

l00405F74:
	balc	0040A130
	bc	00405F3E

l00405F7A:
	sh	r10,0020(r20)
	lhu	r4,0000(r21)
	balc	00406700
	move	r5,r17
	sh	r4,0022(r20)
	addiu	r4,r20,00000028
	lw	r6,-0004(r17)
	sw	r6,0038(r20)
	li	r6,00000010
	bc	00405F74

l00405F9C:
	lw	r17,001C(sp)
	addu	r16,r18,r16
	sw	r0,-0020(r16)
	move	r30,r0
	sw	r18,0040(sp)
	bc	00405E58

l00405FAA:
	move	r30,r4
	bc	00405E58

l00405FAE:
	addiu	r30,r0,FFFFFFF6
	bc	00405E58
00405FB4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; copy_addr: 00405FC0
;;   Called from:
;;     004061DE (in netlink_msg_to_ifaddr)
;;     0040623A (in netlink_msg_to_ifaddr)
copy_addr proc
	save	00000010,ra,00000003
	move	r17,r4
	move	r16,r6
	beqic	r5,00000002,00405FD0

l00405FCA:
	beqic	r5,0000000A,00405FE2

l00405FCE:
	restore.jrc	00000010,ra,00000003

l00405FD0:
	addiu	r4,r6,00000004
	li	r6,00000004

;; fn00405FD4: 00405FD4
;;   Called from:
;;     00405FD2 (in copy_addr)
;;     00406000 (in fn00405FFE)
;;     00406008 (in copy_addr)
;;     00406014 (in copy_addr)
fn00405FD4 proc
	bltuc	r8,r6,00405FE0

l00405FD8:
	sh	r5,0000(r16)
	move.balc	r5,r7,0040A130
	sw	r16,0000(r17)

l00405FE0:
	restore.jrc	00000010,ra,00000003

l00405FE2:
	lbu	r10,0000(r7)
	addiu	r4,r6,00000008
	addiu	r6,r0,000000FE
	bnec	r10,r6,00406002

l00405FF0:
	lbu	r6,0001(r7)
	addiu	r10,r0,00000080
	andi	r6,r6,000000C0
	beqc	r6,r10,00406018

;; fn00405FFE: 00405FFE
;;   Called from:
;;     00405FD4 (in fn00405FD4)
;;     00405FFA (in copy_addr)
;;     0040601C (in fn0040601C)
fn00405FFE proc
	li	r6,00000010
	bc	00405FD4

l00406002:
	addiu	r11,r0,000000FF
	li	r6,00000010
	bnec	r10,r11,00405FD4

l0040600C:
	lbu	r10,0001(r7)
	andi	r10,r10,0000000F
	bneiuc	r10,00000002,00405FD4

l00406018:
	sw	r9,0018(r16)

;; fn0040601C: 0040601C
;;   Called from:
;;     00406018 (in copy_addr)
;;     004075B6 (in __netlink_enumerate)
fn0040601C proc
	bc	00405FFE

;; netlink_msg_to_ifaddr: 0040601E
netlink_msg_to_ifaddr proc
	save	00000030,ra,00000008
	movep	r19,r17,r4,r5
	lhu	r7,0004(r17)
	bneiuc	r7,00000010,004060AC

l00406028:
	lw	r6,0000(r17)
	addiu	r7,r17,00000020
	addu	r6,r17,r6

l00406030:
	subu	r5,r6,r7
	bgeiuc	r5,00000004,0040603A

l00406036:
	move	r5,r0
	bc	0040604E

l0040603A:
	lhu	r4,0002(r7)
	lhu	r5,0000(r7)
	beqic	r4,00000007,0040604C

l00406042:
	addiu	r5,r5,00000003
	ins	r5,r0,00000000,00000001
	addu	r7,r7,r5
	bc	00406030

l0040604C:
	addiu	r5,r5,FFFFFFFC

l0040604E:
	addiu	r5,r5,000000A4
	li	r4,00000001
	balc	0040CDC0
	move	r16,r4
	li	r4,FFFFFFFF
	beqzc	r16,004060AA

l0040605E:
	lhu	r7,0004(r17)
	bneiuc	r7,00000010,00406168

l00406064:
	lw	r7,0014(r17)
	addiu	r18,r17,00000020
	sw	r7,008C(r16)
	lw	r7,0018(r17)
	sw	r7,0008(sp)

l00406072:
	lw	r7,0000(r17)
	addu	r7,r17,r7
	subu	r7,r7,r18
	bgeiuc	r7,00000004,004060C6

l0040607C:
	lw	r7,0004(r16)
	beqc	r0,r7,00406288

l00406082:
	lw	r7,008C(r16)
	andi	r7,r7,0000003F
	lsa	r7,r7,r19,00000002
	lw	r6,0008(r7)
	sw	r6,001C(sp)
	sw	r16,0002(r7)

l00406094:
	lw	r7,0004(r16)
	beqc	r0,r7,00406288

l0040609A:
	lw	r7,0000(r19)
	bnezc	r7,004060A0

l0040609E:
	sw	r16,0000(r19)

l004060A0:
	lw	r7,0004(r19)
	beqzc	r7,004060A6

l004060A4:
	sw	r16,0000(r7)

l004060A6:
	sw	r16,0001(r19)

l004060A8:
	move	r4,r0

l004060AA:
	restore.jrc	00000030,ra,00000008

l004060AC:
	lw	r6,0014(r17)
	andi	r7,r6,0000003F
	lsa	r7,r7,r19,00000002
	lw	r18,0008(r7)

l004060B8:
	beqzc	r18,004060A8

l004060BA:
	lw	r7,008C(r18)
	beqc	r6,r7,00406036

l004060C2:
	lw	r18,001C(r18)
	bc	004060B8

l004060C6:
	lhu	r7,0002(r18)
	lhu	r6,0000(r18)
	beqic	r7,00000002,00406138

l004060CE:
	bgeiuc	r7,00000003,004060E2

l004060D2:
	beqic	r7,00000001,0040610C

l004060D6:
	lhu	r7,0000(r18)
	addiu	r7,r7,00000003
	ins	r7,r0,00000000,00000001
	addu	r18,r18,r7
	bc	00406072

l004060E2:
	beqic	r7,00000003,004060F8

l004060E6:
	bneiuc	r7,00000007,004060D6

l004060EA:
	addiu	r4,r16,000000A4
	addiu	r6,r6,FFFFFFFC
	addiu	r5,r18,00000004
	sw	r4,0018(sp)
	balc	0040628E
	bc	004060D6

l004060F8:
	addiu	r6,r6,FFFFFFFC
	bgeiuc	r6,00000011,004060D6

l004060FE:
	addiu	r20,r16,00000090
	addiu	r5,r18,00000004
	move.balc	r4,r20,0040A130
	sw	r20,0001(r16)
	bc	004060D6

l0040610C:
	addiu	r6,r6,FFFFFFFC
	bgeiuc	r6,00000019,004060D6

l00406112:
	lw	r5,0014(r17)
	li	r4,00000011
	lhu	r7,0012(r17)
	addiu	r20,r16,00000020
	sh	r4,0020(r16)
	addiu	r4,r16,0000002C
	sw	r5,0024(sp)
	addiu	r5,r18,00000004
	sh	r7,0028(r16)
	sb	r6,002B(r16)
	balc	0040628E
	sw	r20,0003(r16)
	bc	004060D6

l00406138:
	addiu	r6,r6,FFFFFFFC
	bgeiuc	r6,00000019,004060D6

l0040613E:
	lw	r5,0014(r17)
	li	r4,00000011
	lhu	r7,0012(r17)
	addiu	r20,r16,00000068
	sh	r4,0068(r16)
	addiu	r4,r16,00000074
	sw	r5,006C(r16)
	addiu	r5,r18,00000004
	sh	r7,0070(r16)
	sb	r6,0073(r16)
	balc	0040628E
	sw	r20,0014(r16)
	bc	004060D6

l00406168:
	lw	r7,0004(r18)
	sw	r7,0004(sp)
	lw	r7,0008(r18)
	addiu	r18,r17,00000018
	sw	r7,0008(sp)

l00406172:
	lw	r7,0000(r17)
	addu	r7,r17,r7
	subu	r7,r7,r18
	bgeiuc	r7,00000004,004061E4

l0040617C:
	lw	r7,000C(r16)
	beqc	r0,r7,00406094

l00406182:
	lbu	r7,0011(r17)
	addiu	r6,r0,00000080
	lbu	r21,0010(r17)
	addiu	r20,r16,00000010
	addiu	r22,r16,00000044
	move	r17,r7
	sw	r0,0000(sp)
	sw	r0,0004(sp)
	sw	r0,0008(sp)
	sw	r0,000C(sp)
	bgeuc	r6,r7,004061A8

l004061A4:
	addiu	r17,r0,FFFFFF80

l004061A8:
	andi	r17,r17,000000FF
	addiu	r5,r0,000000FF
	sra	r18,r17,00000003
	move	r4,sp
	move	r6,r18
	balc	0040A690
	beqic	r18,00000010,004061D4

l004061BE:
	addiu	r7,sp,00000010
	andi	r17,r17,00000007
	addu	r18,r7,r18
	li	r7,00000008
	subu	r7,r7,r17
	addiu	r17,r0,000000FF
	sllv	r17,r17,r7
	sb	r17,-0010(r18)

l004061D4:
	move	r9,r0
	addiu	r8,r0,00000010
	move	r7,sp
	movep	r5,r6,r21,r22
	move.balc	r4,r20,00405FC0
	bc	00406094

l004061E4:
	lhu	r7,0002(r18)
	beqic	r7,00000002,00406240

l004061EA:
	lhu	r8,0000(r18)
	bgeiuc	r7,00000003,00406202

l004061F2:
	beqic	r7,00000001,0040621C

l004061F6:
	lhu	r7,0000(r18)
	addiu	r7,r7,00000003
	ins	r7,r0,00000000,00000001
	addu	r18,r18,r7
	bc	00406172

l00406202:
	beqic	r7,00000003,00406272

l00406206:
	bneiuc	r7,00000004,004061F6

l0040620A:
	addiu	r8,r8,FFFFFFFC
	addiu	r7,r18,00000004
	addiu	r6,r16,00000068
	lbu	r5,0010(r17)
	lw	r9,0014(r17)
	bc	00406230

l0040621C:
	lw	r6,000C(r16)
	addiu	r7,r18,00000004
	lbu	r5,0010(r17)
	addiu	r8,r8,FFFFFFFC
	lw	r9,0014(r17)
	beqzc	r6,00406234

l0040622C:
	addiu	r6,r16,00000068

l00406230:
	addiu	r4,r16,00000014
	bc	0040623A

l00406234:
	addiu	r6,r16,00000020

l00406238:
	addiu	r4,r16,0000000C

l0040623A:
	balc	00405FC0
	bc	004061F6

l00406240:
	lw	r7,000C(r16)
	addiu	r20,r16,00000020
	beqzc	r7,0040625E

l00406248:
	addiu	r7,r16,00000068
	li	r6,00000024
	movep	r4,r5,r7,r20
	balc	0040628E
	li	r6,00000024
	move	r7,r4
	sw	r7,0014(sp)
	movep	r4,r5,r20,r0
	balc	0040A690

l0040625E:
	lhu	r8,0000(r18)
	addiu	r7,r18,00000004
	move	r6,r20
	lbu	r5,0010(r17)
	addiu	r8,r8,FFFFFFFC
	lw	r9,0014(r17)
	bc	00406238

l00406272:
	addiu	r6,r8,FFFFFFFC
	bgeiuc	r6,00000011,004061F6

l0040627A:
	addiu	r20,r16,00000090
	addiu	r5,r18,00000004
	move.balc	r4,r20,0040A130
	sw	r20,0001(r16)
	bc	004061F6

l00406288:
	move.balc	r4,r16,00404F2E
	bc	004060A8

;; fn0040628E: 0040628E
;;   Called from:
;;     004060F4 (in netlink_msg_to_ifaddr)
;;     00406132 (in netlink_msg_to_ifaddr)
;;     00406160 (in netlink_msg_to_ifaddr)
;;     00406250 (in netlink_msg_to_ifaddr)
fn0040628E proc
	bc	0040A130

;; freeifaddrs: 00406292
;;   Called from:
;;     00400E1A (in ping4_run)
;;     00403826 (in ping6_run)
;;     004062D0 (in getifaddrs)
freeifaddrs proc
	save	00000010,ra,00000002
	bc	0040629E

l00406296:
	lw	r16,0000(r4)
	balc	00404F2E
	move	r4,r16

l0040629E:
	bnezc	r4,00406296

l004062A0:
	restore.jrc	00000010,ra,00000002

;; getifaddrs: 004062A2
;;   Called from:
;;     00400DFA (in ping4_run)
;;     004037EC (in ping6_run)
getifaddrs proc
	save	00000120,ra,00000003
	addiu	r6,r0,00000108
	move	r5,r0
	move	r17,r4
	addiu	r4,sp,00000008
	balc	0040A690
	addiu	r7,sp,00000008
	addiupc	r6,FFFFFD64
	movep	r4,r5,r0,r0
	balc	004075D2
	move	r16,r4
	bnezc	r4,004062CE

l004062C4:
	lw	r17,0008(sp)
	sw	r7,0040(sp)

l004062C8:
	move	r4,r16
	restore.jrc	00000120,ra,00000003

l004062CE:
	lw	r17,0008(sp)
	balc	00406292
	bc	004062C8
004062D6                   00 00 00 00 00 00 00 00 00 00       ..........

;; dns_parse_callback: 004062E0
dns_parse_callback proc
	save	00000010,ra,00000002
	move	r16,r4
	move	r4,r8
	bneiuc	r5,0000000C,004062FE

l004062EA:
	addiu	r8,r0,00000100
	move	r7,r16
	addiu	r5,r4,00000200
	balc	0040D000
	bltc	r0,r4,004062FE

l004062FC:
	sb	r0,0000(r16)

l004062FE:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

;; getnameinfo: 00406302
;;   Called from:
;;     004012FA (in pr_addr)
;;     0040133C (in pr_addr)
getnameinfo proc
	save	00000870,r30,0000000A
	lhu	r20,0000(r4)
	move	r16,r4
	move	r23,r6
	move	r30,r7
	move	r19,r10
	swm	r8,0014(sp),00000002
	beqic	r20,00000002,00406324

l0040631A:
	beqic	r20,0000000A,004064E4

l0040631E:
	addiu	r4,r0,FFFFFFFA
	bc	004065A0

l00406324:
	addiu	r17,r4,00000004
	addiu	r4,r0,FFFFFFFA
	bltiuc	r5,00000010,004065A0

l0040632E:
	lbu	r9,0004(r16)
	addiupc	r5,0000BBBE
	lbu	r8,0005(r16)
	addiu	r4,sp,00000058
	lbu	r7,0006(r16)
	move	r18,r0
	lbu	r6,0007(r16)
	balc	00408860

l0040634A:
	beqc	r0,r23,004065BE

l0040634E:
	beqc	r0,r30,004065BE

l00406352:
	andi	r7,r19,00000001
	sb	r0,0138(sp)
	sw	r7,001C(sp)
	bnec	r0,r7,0040644E

l0040635E:
	addiu	r7,r0,00000408
	addiu	r6,sp,00000438
	addiu	r5,sp,000000A8
	addiupc	r4,0000BBB0
	balc	00408070
	move	r21,r4
	beqc	r0,r4,0040644E

l00406376:
	sw	r17,0010(sp)
	bneiuc	r20,00000002,00406392

l0040637C:
	li	r6,00000004
	addiu	r4,sp,00000038
	move.balc	r5,r17,0040A130
	li	r6,0000000C
	addiupc	r5,0000CA82
	addiu	r4,sp,0000002C
	balc	004066AA
	addiu	r7,sp,0000002C
	sw	r7,0010(sp)

l00406392:
	move	r6,r21
	addiu	r5,r0,00000200
	addiu	r4,sp,00000238
	balc	00408240
	beqc	r0,r4,0040644A

l004063A4:
	li	r5,00000023
	addiu	r4,sp,00000238
	balc	0040A770
	beqzc	r4,004063B6

l004063B0:
	li	r7,0000000A
	sb	r0,0001(r4)
	sb	r7,0000(r4)

l004063B6:
	addiu	r6,sp,00000238

l004063BA:
	lbu	r5,0000(r6)
	addiu	r22,r6,00000001
	andi	r4,r5,000000DF
	beqzc	r4,004063CE

l004063C6:
	addiu	r5,r5,FFFFFFF7
	bgeiuc	r5,00000005,00406552

l004063CE:
	sb	r0,0000(r6)
	addiu	r5,sp,00000238
	addiu	r4,sp,0000003C
	move	r6,r0
	balc	00406B50
	bgec	r0,r4,00406392

l004063E0:
	lw	r17,003C(sp)
	bneiuc	r6,00000002,004063FA

l004063E6:
	li	r6,00000004
	addiu	r5,sp,00000044
	addiu	r4,sp,00000050
	balc	004066AA
	li	r6,0000000C
	addiupc	r5,0000CA18
	addiu	r4,sp,00000044
	balc	004066AA
	sw	r0,0040(sp)

l004063FA:
	lw	r17,0010(sp)
	li	r6,00000010
	addiu	r5,sp,00000044
	balc	0040A100
	bnezc	r4,00406392

l00406406:
	lw	r17,0040(sp)
	bnec	r6,r18,00406392

l0040640C:
	lbu	r6,0000(r22)
	beqc	r0,r6,00406556

l00406414:
	beqic	r6,00000020,004066A2

l00406418:
	addiu	r6,r6,FFFFFFF7
	bltiuc	r6,00000005,004066A2

l00406420:
	move	r6,r22

l00406422:
	lbu	r5,0000(r6)
	andi	r4,r5,000000DF
	beqzc	r4,00406432

l0040642A:
	addiu	r5,r5,FFFFFFF7
	bgeiuc	r5,00000005,004066A6

l00406432:
	sb	r0,0000(r6)
	addiu	r5,r0,000000FF
	subu	r6,r6,r22
	bltc	r5,r6,00406392

l00406440:
	addiu	r6,r6,00000001
	addiu	r4,sp,00000138
	move.balc	r5,r22,0040A130

l0040644A:
	move.balc	r4,r21,00408060

l0040644E:
	lbu	r7,0138(sp)
	bnezc	r7,0040649E

l00406454:
	lw	r17,001C(sp)
	bnezc	r7,0040649E

l00406458:
	li	r7,00000060
	addiu	r11,sp,00000238
	move	r10,r0
	move	r9,r0
	move	r8,r0
	li	r6,00000001
	addiu	r5,sp,00000058
	sw	r7,0000(sp)
	move	r4,r0
	li	r7,0000000C
	balc	00407690
	addiu	r7,r0,00000200
	move	r5,r4
	addiu	r6,sp,00000438
	addiu	r4,sp,00000238
	balc	00407AC0
	sb	r0,0138(sp)
	move	r5,r4
	bgec	r0,r4,0040649E

l0040648E:
	addiu	r7,sp,00000138
	addiupc	r6,FFFFFE4A
	addiu	r4,sp,00000438
	balc	0040D0D0

l0040649E:
	lbu	r7,0138(sp)
	bnec	r0,r7,00406590

l004064A6:
	addiu	r4,r0,FFFFFFFE
	bbnezc	r19,00000003,004065A0

l004064AE:
	addiu	r7,r0,00000100
	addiu	r6,sp,00000138
	movep	r4,r5,r20,r17
	balc	00406860
	beqc	r0,r18,00406590

l004064C0:
	bbeqzc	r19,00000008,0040655A

l004064C4:
	addiu	r4,sp,00000047
	sb	r0,0047(sp)

l004064CC:
	li	r6,0000000A
	addiu	r4,r4,FFFFFFFF
	modu	r7,r18,r6
	addiu	r7,r7,00000030
	sb	r7,0000(r4)
	move	r7,r18
	divu	r18,r7,r6
	bnezc	r18,004064CC

l004064E2:
	bc	0040657E

l004064E4:
	addiu	r17,r4,00000008
	addiu	r4,r0,FFFFFFFA
	bltiuc	r5,0000001C,004065A0

l004064EE:
	li	r6,0000000C
	addiupc	r5,0000C918
	move.balc	r4,r17,0040A100
	beqzc	r4,00406536

l004064FA:
	addiu	r7,sp,00000058
	li	r6,0000000F

l004064FE:
	lbux	r5,r6(r17)
	addiupc	r18,0000C916
	addiu	r6,r6,FFFFFFFF
	andi	r4,r5,0000000F
	srl	r5,r5,00000004
	lbux	r4,r4(r18)
	lbux	r5,r5(r18)
	sb	r4,0000(r7)
	li	r4,0000002E
	sb	r5,0002(r7)
	li	r5,FFFFFFFF
	sb	r4,0001(r7)
	addiu	r7,r7,00000004
	sb	r4,-0001(r7)
	bnec	r6,r5,004064FE

l00406528:
	addiupc	r5,0000B9E4
	addiu	r4,sp,00000098
	balc	0040A860

l00406532:
	lw	r18,0018(r16)
	bc	0040634A

l00406536:
	lbu	r9,0014(r16)
	addiupc	r5,0000B9B6
	lbu	r8,0015(r16)
	addiu	r4,sp,00000058
	lbu	r7,0016(r16)
	lbu	r6,0017(r16)
	balc	00408860
	bc	00406532

l00406552:
	move	r6,r22
	bc	004063BA

l00406556:
	move	r6,r22
	bc	00406432

l0040655A:
	lbu	r7,0000(r17)
	addiu	r6,r0,000000FE
	bnec	r7,r6,004065A4

l00406564:
	lbu	r7,0001(r17)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r7,r6,004064C4

l00406572:
	addiu	r5,sp,00000439
	move.balc	r4,r18,00406710
	beqc	r0,r4,004064C4

l0040657E:
	li	r7,00000025
	addiu	r5,r4,FFFFFFFF
	sb	r7,-0001(r4)
	addiu	r4,sp,00000138
	balc	0040A750

l00406590:
	addiu	r4,sp,00000138
	balc	0040A890
	bltuc	r4,r30,004065B6

l0040659C:
	addiu	r4,r0,FFFFFFF4

l004065A0:
	restore.jrc	00000870,r30,0000000A

l004065A4:
	addiu	r6,r0,000000FF
	bnec	r7,r6,004064C4

l004065AC:
	lbu	r7,0001(r17)
	andi	r7,r7,0000000F
	bneiuc	r7,00000002,004064C4

l004065B4:
	bc	00406572

l004065B6:
	addiu	r5,sp,00000138
	move.balc	r4,r23,0040A860

l004065BE:
	lw	r17,0014(sp)
	move	r4,r0
	beqzc	r7,004065A0

l004065C4:
	lw	r17,0018(sp)
	beqzc	r7,004065A0

l004065C8:
	lhu	r4,0002(r16)
	balc	00407630
	sb	r0,0138(sp)
	move	r18,r4
	bbnezc	r19,00000001,004065F4

l004065D8:
	addiu	r7,r0,00000408
	addiu	r6,sp,00000438
	addiu	r5,sp,00000238
	addiupc	r4,0000B940
	balc	00408070
	andi	r19,r19,00000010
	move	r20,r4
	bnezc	r4,00406636

l004065F4:
	lbu	r7,0138(sp)
	addiu	r16,sp,00000138
	bnezc	r7,0040661E

l004065FE:
	move	r4,r18
	addiu	r16,sp,00000047
	sb	r0,0047(sp)

l00406608:
	li	r6,0000000A
	addiu	r16,r16,FFFFFFFF
	modu	r7,r4,r6
	addiu	r7,r7,00000030
	sb	r7,0000(r16)
	move	r7,r4
	divu	r4,r7,r6
	bnezc	r4,00406608

l0040661E:
	move.balc	r4,r16,0040A890
	lw	r17,0018(sp)
	bgeuc	r4,r7,0040659C

l00406628:
	lw	r17,0014(sp)
	move.balc	r5,r16,0040A860
	move	r4,r0
	bc	004065A0

l00406632:
	move	r6,r16
	bc	00406654

l00406636:
	addiu	r17,sp,000000A8
	move	r6,r20
	addiu	r5,r0,00000080
	move.balc	r4,r17,00408240
	beqzc	r4,0040669C

l00406644:
	li	r5,00000023
	move.balc	r4,r17,0040A770
	beqzc	r4,00406652

l0040664C:
	li	r7,0000000A
	sb	r0,0001(r4)
	sb	r7,0000(r4)

l00406652:
	move	r6,r17

l00406654:
	lbu	r7,0000(r6)
	beqzc	r7,00406636

l00406658:
	addiu	r16,r6,00000001
	beqic	r7,00000020,00406668

l00406660:
	addiu	r7,r7,FFFFFFF7
	bgeiuc	r7,00000005,00406632

l00406668:
	sb	r0,0000(r6)
	addiu	r5,sp,0000002C
	li	r6,0000000A
	move.balc	r4,r16,0040A022
	bnec	r4,r18,00406636

l00406676:
	lw	r17,002C(sp)
	beqc	r16,r4,00406636

l0040667C:
	li	r6,00000004
	addiupc	r5,0000B8BE
	beqzc	r19,00406688

l00406684:
	addiupc	r5,0000B8B0

l00406688:
	balc	0040A8E0
	bnezc	r4,00406636

l0040668E:
	subu	r6,r16,r17
	bgeic	r6,00000021,00406636

l00406694:
	addiu	r4,sp,00000138
	move.balc	r5,r17,0040A130

l0040669C:
	move.balc	r4,r20,00408060
	bc	004065F4

l004066A2:
	addiu	r22,r22,00000001
	bc	0040640C

l004066A6:
	addiu	r6,r6,00000001
	bc	00406422

;; fn004066AA: 004066AA
;;   Called from:
;;     0040638C (in getnameinfo)
;;     004063EC (in getnameinfo)
;;     004063F6 (in getnameinfo)
fn004066AA proc
	bc	0040A130
004066AE                                           00 00               ..

;; getsockname: 004066B0
;;   Called from:
;;     00400DE0 (in ping4_run)
;;     004037CC (in ping6_run)
;;     004072CA (in __lookup_name)
getsockname proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000CC
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
004066CC                                     00 00 00 00             ....

;; getsockopt: 004066D0
;;   Called from:
;;     0040218C (in sock_setbufs)
getsockopt proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r8
	move	r8,r7
	move	r7,r6
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000D1
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
004066EE                                           00 00               ..

;; htonl: 004066F0
;;   Called from:
;;     00403B90 (in ping6_run)
htonl proc
	rotx	r4,r4,00000008,00000018,00000000
	jrc	ra
004066F6                   00 00 00 00 00 00 00 00 00 00       ..........

;; htons: 00406700
;;   Called from:
;;     00400B62 (in fn00400B62)
;;     0040235E (in setup)
;;     0040359C (in ping6_usage)
;;     0040359C (in fn0040359C)
;;     00405F64 (in getaddrinfo)
;;     00405F82 (in getaddrinfo)
;;     00407864 (in __res_msend_rc)
;;     00407884 (in __res_msend_rc)
htons proc
	rotx	r4,r4,00000018,00000008,00000000
	andi	r4,r4,0000FFFF
	jrc	ra
00406708                         00 00 00 00 00 00 00 00         ........

;; if_indextoname: 00406710
;;   Called from:
;;     00406576 (in getnameinfo)
if_indextoname proc
	save	00000030,ra,00000004
	move	r6,r0
	movep	r18,r17,r4,r5
	li	r5,00080001
	li	r4,00000001
	balc	00407D80
	move	r16,r4
	bltc	r4,r0,00406752

l00406728:
	addiu	r5,r0,00008910
	move	r6,sp
	sw	r18,0010(sp)
	balc	00405B80
	move	r18,r4
	li	r4,00000039
	move.balc	r5,r16,00404A50
	bgec	r18,r0,00406756

l00406740:
	balc	004049B0
	lw	r7,0000(r4)
	bneiuc	r7,00000013,00406752

l0040674A:
	balc	004049B0
	li	r7,00000006
	sw	r7,0000(sp)

l00406752:
	move	r4,r0
	restore.jrc	00000030,ra,00000004

l00406756:
	li	r6,00000010
	move	r5,sp
	move.balc	r4,r17,0040A930
	restore.jrc	00000030,ra,00000004

;; if_nametoindex: 00406760
;;   Called from:
;;     004033D0 (in if_name2index)
;;     00406BF6 (in __lookup_ipliteral)
if_nametoindex proc
	save	00000030,ra,00000003
	move	r6,r0
	move	r17,r4
	li	r5,00080001
	li	r4,00000001
	balc	00407D80
	move	r16,r4
	bgec	r4,r0,0040677C

l00406778:
	move	r4,r0
	restore.jrc	00000030,ra,00000003

l0040677C:
	li	r6,00000010
	move	r4,sp
	move.balc	r5,r17,0040A930
	addiu	r5,r0,00008933
	move	r6,sp
	move.balc	r4,r16,00405B80
	move	r17,r4
	li	r4,00000039
	move.balc	r5,r16,00404A50
	bltc	r17,r0,00406778

l0040679A:
	lw	r17,0010(sp)
	restore.jrc	00000030,ra,00000003
0040679E                                           00 00               ..

;; __inet_aton: 004067A0
;;   Called from:
;;     00400C46 (in ping4_run)
;;     00406B58 (in __lookup_ipliteral)
__inet_aton proc
	save	00000030,ra,00000004
	move	r16,r0
	sw	r0,0010(sp)
	movep	r17,r18,r4,r5
	sw	r0,0014(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)

l004067AE:
	move	r6,r0
	addiu	r5,sp,0000000C
	move.balc	r4,r17,0040A022
	addiu	r7,sp,00000010
	swxs	r4,r16(r7)
	lw	r17,000C(sp)
	bnec	r17,r7,004067C4

l004067C0:
	move	r4,r0
	restore.jrc	00000030,ra,00000004

l004067C4:
	lbu	r6,0000(r7)
	beqzc	r6,004067CC

l004067C8:
	bneiuc	r6,0000002E,004067C0

l004067CC:
	lbu	r5,0000(r17)
	addiu	r5,r5,FFFFFFD0
	bgeiuc	r5,0000000A,004067C0

l004067D6:
	beqzc	r6,004067E4

l004067D8:
	addiu	r16,r16,00000001
	addiu	r17,r7,00000001
	bneiuc	r16,00000004,004067AE

l004067E2:
	bc	004067C0

l004067E4:
	beqic	r16,00000001,004067FC

l004067E8:
	beqic	r16,00000002,00406808

l004067EC:
	bnezc	r16,00406812

l004067EE:
	lw	r17,0010(sp)
	ext	r6,r7,00000000,00000018
	srl	r7,r7,00000018
	sw	r7,0010(sp)
	sw	r6,0014(sp)

l004067FC:
	lw	r17,0014(sp)
	andi	r6,r7,0000FFFF
	srl	r7,r7,00000010
	sw	r7,0014(sp)
	sw	r6,0018(sp)

l00406808:
	lw	r17,0018(sp)
	andi	r6,r7,000000FF
	srl	r7,r7,00000008
	sw	r7,0018(sp)
	sw	r6,001C(sp)

l00406812:
	move	r7,r0

l00406814:
	addiu	r6,sp,00000010
	addiu	r5,r0,000000FF
	lwxs	r6,r7(r6)
	bltuc	r5,r6,004067C0

l00406820:
	sbx	r6,r18(r7)
	addiu	r7,r7,00000001
	bneiuc	r7,00000004,00406814

l0040682A:
	li	r4,00000001
	restore.jrc	00000030,ra,00000004
0040682E                                           00 00               ..

;; inet_ntoa: 00406830
;;   Called from:
;;     00401590 (in fn00401590)
inet_ntoa proc
	save	00000010,ra,00000001
	addiupc	r6,0000B712
	srl	r10,r4,00000018
	ext	r9,r4,00000000,00000008
	ext	r8,r4,00000008,00000008
	andi	r7,r4,000000FF
	li	r5,00000010
	addiupc	r4,0002C126
	balc	00408820
	addiupc	r4,0002C11E
	restore.jrc	00000010,ra,00000001
00406854             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; inet_ntop: 00406860
;;   Called from:
;;     004064B8 (in getnameinfo)
inet_ntop proc
	save	000000B0,ra,00000007
	movep	r16,r17,r5,r6
	move	r20,r7
	beqic	r4,00000002,0040687A

l0040686A:
	beqic	r4,0000000A,0040689E

l0040686E:
	balc	004049B0
	li	r7,00000061

l00406874:
	move	r17,r0
	sw	r7,0000(sp)
	bc	004069BC

l0040687A:
	lbu	r10,0003(r16)
	addiupc	r6,0000B6C6
	lbu	r9,0002(r16)
	lbu	r8,0001(r16)
	lbu	r7,0000(r16)
	movep	r4,r5,r17,r20
	balc	00408820
	bltuc	r4,r20,004069BC

l00406896:
	balc	004049B0
	li	r7,0000001C
	bc	00406874

l0040689E:
	li	r6,0000000C
	addiupc	r5,0000C58C
	addiu	r18,sp,0000002C
	move.balc	r4,r16,0040A100
	lbu	r5,0002(r16)
	lbu	r6,0000(r16)
	lbu	r2,0008(r16)
	sll	r5,r5,00000008
	lbu	r21,0006(r16)
	sll	r7,r6,00000008
	lbu	r19,0004(r16)
	sll	r2,r2,00000008
	lbu	r8,0003(r16)
	sll	r21,r21,00000008
	lbu	r13,0001(r16)
	sll	r19,r19,00000008
	lbu	r11,0009(r16)
	lbu	r10,0007(r16)
	lbu	r9,0005(r16)
	lbu	r12,000E(r16)
	lbu	r15,000F(r16)
	lbu	r3,000C(r16)
	lbu	r14,000D(r16)
	beqzc	r4,00406932

l004068EE:
	sll	r12,r12,00000008
	sll	r3,r3,00000008
	addu	r15,r12,r15
	addu	r14,r3,r14
	swm	r14,0004(sp),00000002
	lbu	r4,000A(r16)
	lbu	r6,000B(r16)
	addu	r8,r8,r5
	sll	r4,r4,00000008
	addu	r11,r2,r11
	addu	r4,r4,r6
	addu	r10,r10,r21
	sw	r4,0000(sp)
	addu	r9,r9,r19
	addu	r7,r7,r13
	li	r5,00000064
	addiupc	r6,0000B630
	move.balc	r4,r18,00408820

l00406928:
	move	r19,r0
	addiu	r21,r0,00000002
	move	r16,r0
	bc	00406964

l00406932:
	sw	r3,0004(sp)
	addu	r8,r8,r5
	sw	r14,0008(sp)
	addu	r11,r2,r11
	sw	r12,000C(sp)
	addu	r10,r10,r21
	sw	r15,0010(sp)
	addu	r9,r9,r19
	lbu	r4,000A(r16)
	addu	r7,r7,r13
	lbu	r6,000B(r16)
	li	r5,00000064
	sll	r4,r4,00000008
	addu	r4,r4,r6
	addiupc	r6,0000B612
	sw	r4,0000(sp)
	move.balc	r4,r18,00408820
	bc	00406928

l00406962:
	addiu	r16,r16,00000001

l00406964:
	lbux	r7,r16(r18)
	beqzc	r7,00406984

l0040696A:
	beqzc	r16,00406970

l0040696C:
	bneiuc	r7,0000003A,00406962

l00406970:
	addiupc	r5,0000B618
	addu	r4,r18,r16
	balc	0040A960
	bgec	r21,r4,00406962

l0040697E:
	move	r19,r16
	move	r21,r4
	bc	00406962

l00406984:
	bltic	r21,00000003,004069AE

l00406988:
	addiu	r7,sp,00000090
	li	r6,0000003A
	addu	r7,r7,r19
	addu	r5,r21,r19
	sb	r6,-0064(r7)
	addiu	r4,r19,00000002
	sb	r6,-0063(r7)
	subu	r6,r16,r19
	subu	r6,r6,r21
	addu	r5,r18,r5
	addu	r4,r18,r4
	addiu	r6,r6,00000001
	balc	0040A510

l004069AE:
	move.balc	r4,r18,0040A890
	bgeuc	r4,r20,00406896

l004069B6:
	movep	r4,r5,r17,r18
	balc	0040A860

l004069BC:
	move	r4,r17
	restore.jrc	000000B0,ra,00000007

;; inet_pton: 004069C0
;;   Called from:
;;     00406B2C (in inet_pton)
;;     00406BA2 (in __lookup_ipliteral)
inet_pton proc
	save	00000030,ra,00000006
	movep	r16,r19,r5,r6
	bneiuc	r4,00000002,00406A24

l004069C8:
	move	r7,r0
	bc	00406A20

l004069CC:
	li	r18,0000000A
	addiu	r6,r6,00000001
	mul	r5,r5,r18
	addu	r5,r5,r17
	addiu	r5,r5,FFFFFFD0
	beqic	r6,00000003,004069F4

l004069DC:
	lbux	r17,r6(r16)
	addiu	r18,r17,FFFFFFD0
	bltiuc	r18,0000000A,004069CC

l004069E8:
	bnezc	r6,004069EE

l004069EA:
	move	r4,r0
	restore.jrc	00000030,ra,00000006

l004069EE:
	beqic	r6,00000001,004069FA

l004069F2:
	move	r6,r4

l004069F4:
	lbu	r17,0000(r16)
	beqic	r17,00000030,004069EA

l004069FA:
	addiu	r17,r0,000000FF
	bltc	r17,r5,004069EA

l00406A02:
	sbx	r5,r19(r7)
	lbux	r5,r6(r16)
	bnezc	r5,00406A12

l00406A0C:
	seqi	r4,r7,00000003
	restore.jrc	00000030,ra,00000006

l00406A12:
	bneiuc	r5,0000002E,004069EA

l00406A16:
	addiu	r6,r6,00000001
	addiu	r7,r7,00000001
	addu	r16,r16,r6
	beqic	r7,00000004,004069EA

l00406A20:
	movep	r5,r6,r0,r0
	bc	004069DC

l00406A24:
	beqic	r4,0000000A,00406A34

l00406A28:
	balc	004049B0
	li	r7,00000061
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000030,ra,00000006

l00406A34:
	lbu	r7,0000(r16)
	bneiuc	r7,0000003A,00406A42

l00406A3A:
	lbu	r7,0001(r16)
	bneiuc	r7,0000003A,004069EA

l00406A40:
	addiu	r16,r16,00000001

l00406A42:
	li	r4,FFFFFFFF
	move	r17,r0

l00406A46:
	lbu	r7,0000(r16)
	bneiuc	r7,0000003A,00406A74

l00406A4C:
	li	r7,FFFFFFFF
	bnec	r7,r4,00406A74

l00406A52:
	addiu	r6,sp,00000010
	andi	r7,r17,00000007
	lsa	r7,r7,r6,00000001
	lbu	r6,0001(r16)
	sh	r0,-0010(r7)
	addiu	r7,r16,00000001
	beqc	r0,r6,00406B3C

l00406A68:
	beqic	r17,00000007,004069EA

l00406A6C:
	move	r4,r17

l00406A6E:
	addiu	r17,r17,00000001
	move	r16,r7
	bc	00406A46

l00406A74:
	move	r5,r0
	move	r7,r0

l00406A78:
	lbux	r18,r7(r16)
	addiu	r6,r18,FFFFFFD0
	bltiuc	r6,0000000A,00406A94

l00406A84:
	ori	r6,r18,00000020
	addiu	r18,r6,FFFFFF9F
	bgeiuc	r18,00000006,00406AA0

l00406A90:
	addiu	r6,r6,FFFFFFA9

l00406A94:
	sll	r5,r5,00000004
	addiu	r7,r7,00000001
	addu	r5,r5,r6
	bneiuc	r7,00000004,00406A78

l00406A9E:
	bc	00406AA4

l00406AA0:
	beqc	r0,r7,004069EA

l00406AA4:
	andi	r6,r17,00000007
	addiu	r18,sp,00000010
	lsa	r6,r6,r18,00000001
	sh	r5,-0010(r6)
	lbux	r6,r7(r16)
	bnezc	r6,00406AC4

l00406AB6:
	li	r7,FFFFFFFF
	bnec	r7,r4,00406B40

l00406ABC:
	bneiuc	r17,00000007,004069EA

l00406AC0:
	move	r18,r0
	bc	00406B0E

l00406AC4:
	beqic	r17,00000007,004069EA

l00406AC8:
	beqic	r6,0000003A,00406B36

l00406ACC:
	bneiuc	r6,0000002E,004069EA

l00406AD0:
	li	r7,FFFFFFFF
	bgeic	r17,00000006,00406B08

l00406AD6:
	beqc	r4,r7,004069EA

l00406ADA:
	addiu	r17,r17,00000001
	li	r18,00000001

l00406ADE:
	addiu	r6,r17,00000001
	lsa	r20,r4,sp,00000001
	subu	r6,r6,r4
	addiu	r4,r4,00000007
	subu	r4,r4,r17
	sll	r6,r6,00000001
	lsa	r4,r4,sp,00000001
	move.balc	r5,r20,0040A510
	move	r7,r0

l00406AF8:
	li	r6,00000007
	subu	r6,r6,r17
	bgec	r7,r6,00406B0E

l00406B00:
	shxs	r0,r7(r20)
	addiu	r7,r7,00000001
	bc	00406AF8

l00406B08:
	li	r18,00000001
	bnec	r7,r4,00406ADA

l00406B0E:
	movep	r6,r7,r19,r0

l00406B10:
	lhuxs	r5,r7(sp)
	addiu	r7,r7,00000001
	srl	r4,r5,00000008
	sb	r4,0000(r6)
	addiu	r6,r6,00000002
	sb	r5,-0001(r6)
	bneiuc	r7,00000008,00406B10

l00406B24:
	li	r4,00000001
	beqzc	r18,00406B44

l00406B28:
	addiu	r6,r19,0000000C
	li	r4,00000002
	move.balc	r5,r16,004069C0
	slt	r4,r0,r4
	restore.jrc	00000030,ra,00000006

l00406B36:
	addiu	r7,r7,00000001
	addu	r7,r16,r7
	bc	00406A6E

l00406B3C:
	move	r4,r17
	move	r16,r7

l00406B40:
	move	r18,r0
	bc	00406ADE

l00406B44:
	restore.jrc	00000030,ra,00000006
00406B46                   00 00 00 00 00 00 00 00 00 00       ..........

;; __lookup_ipliteral: 00406B50
;;   Called from:
;;     004063D8 (in getnameinfo)
;;     00406EF4 (in name_from_hosts)
;;     004070D4 (in __lookup_name)
;;     00407B60 (in __get_resolv_conf)
;;     00407C9A (in __get_resolv_conf)
__lookup_ipliteral proc
	save	00000080,ra,00000006
	movep	r16,r18,r4,r5
	addiu	r5,sp,00000008
	move	r20,r6
	move.balc	r4,r18,004067A0
	bgec	r0,r4,00406B7E

l00406B60:
	bneiuc	r20,0000000A,00406B6A

l00406B64:
	addiu	r7,r0,FFFFFFFE
	bc	00406B7A

l00406B6A:
	li	r6,00000004
	addiu	r5,sp,00000008
	addiu	r4,r16,00000008
	balc	00406C24
	li	r7,00000002
	sw	r7,0000(sp)
	sw	r0,0004(sp)

l00406B78:
	li	r7,00000001

l00406B7A:
	move	r4,r7
	restore.jrc	00000080,ra,00000006

l00406B7E:
	li	r5,00000025
	move.balc	r4,r18,0040A770
	move	r17,r4
	beqzc	r4,00406B9E

l00406B88:
	subu	r19,r4,r18
	bgeic	r19,00000000,00406B9E

l00406B8E:
	movep	r5,r6,r18,r19
	addiu	r4,sp,00000020
	addiu	r18,sp,00000020
	balc	00406C24
	addiu	r7,sp,00000060
	addu	r19,r7,r19
	sb	r0,-0040(r19)

l00406B9E:
	addiu	r6,sp,00000010
	li	r4,0000000A
	move.balc	r5,r18,004069C0
	move	r7,r0
	bgec	r0,r4,00406B7A

l00406BAC:
	beqic	r20,00000002,00406B64

l00406BB0:
	li	r6,00000010
	addiu	r4,r16,00000008
	addu	r5,sp,r6
	balc	00406C24
	li	r6,0000000A
	sw	r6,0000(sp)
	beqzc	r17,00406C20

l00406BC0:
	lbu	r7,0001(r17)
	addiu	r18,r17,00000001
	addiu	r7,r7,FFFFFFD0
	bgeuc	r7,r6,00406C02

l00406BCE:
	addiu	r5,sp,0000000C
	move.balc	r4,r18,0040A00C

l00406BD4:
	lw	r17,000C(sp)
	lbu	r7,0000(r7)
	beqzc	r7,00406C1C

l00406BDA:
	lbu	r7,0010(sp)
	addiu	r6,r0,000000FE
	bnec	r7,r6,00406C08

l00406BE6:
	lbu	r7,0011(sp)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r7,r6,00406B64

l00406BF6:
	move.balc	r4,r18,00406760
	beqc	r0,r4,00406B64

l00406BFE:
	sw	r4,0004(sp)
	bc	00406B78

l00406C02:
	movep	r4,r5,r0,r0
	sw	r17,000C(sp)
	bc	00406BD4

l00406C08:
	addiu	r6,r0,000000FF
	bnec	r7,r6,00406B64

l00406C10:
	lbu	r7,0011(sp)
	andi	r7,r7,0000000F
	beqic	r7,00000002,00406BF6

l00406C1A:
	bc	00406B64

l00406C1C:
	beqzc	r5,00406BFE

l00406C1E:
	bc	00406B64

l00406C20:
	move	r4,r0
	bc	00406BFE

;; fn00406C24: 00406C24
;;   Called from:
;;     00406B70 (in __lookup_ipliteral)
;;     00406B94 (in __lookup_ipliteral)
;;     00406BB8 (in __lookup_ipliteral)
fn00406C24 proc
	bc	0040A130
00406C28                         00 00 00 00 00 00 00 00         ........

;; __isspace: 00406C30
;;   Called from:
;;     00406F46 (in fn00406F46)
;;     0040715A (in __lookup_name)
;;     0040716C (in __lookup_name)
__isspace proc
	li	r7,00000001
	beqic	r4,00000020,00406C3E

l00406C36:
	addiu	r4,r4,FFFFFFF7
	sltiu	r7,r4,00000005

l00406C3E:
	move	r4,r7
	jrc	ra

;; scopeof: 00406C42
;;   Called from:
;;     0040728A (in __lookup_name)
;;     004072D2 (in __lookup_name)
scopeof proc
	lbu	r6,0000(r4)
	addiu	r5,r0,000000FF
	move	r7,r4
	bnec	r5,r6,00406C52

l00406C4C:
	lbu	r4,0001(r4)
	andi	r4,r4,0000000F
	jrc	ra

l00406C52:
	addiu	r5,r0,000000FE
	bnec	r5,r6,00406C68

l00406C58:
	lbu	r5,0001(r4)
	addiu	r8,r0,00000080
	li	r4,00000002
	andi	r5,r5,000000C0
	beqc	r5,r8,00406CA8

l00406C68:
	lw	r5,0000(r7)
	bnezc	r5,00406C90

l00406C6C:
	lw	r5,0004(r7)
	bnezc	r5,00406C90

l00406C70:
	lw	r5,0008(r7)
	bnezc	r5,00406C90

l00406C74:
	lbu	r5,000C(r7)
	bnezc	r5,00406C90

l00406C7A:
	lbu	r5,000D(r7)
	bnezc	r5,00406C90

l00406C80:
	lbu	r5,000E(r7)
	bnezc	r5,00406C90

l00406C86:
	lbu	r5,000F(r7)
	li	r4,00000002
	beqic	r5,00000001,00406CA8

l00406C90:
	addiu	r5,r0,000000FE
	li	r4,0000000E
	bnec	r5,r6,00406CA8

l00406C98:
	lbu	r7,0001(r7)
	li	r6,00000005
	andi	r7,r7,000000C0
	xori	r7,r7,000000C0
	movz	r4,r6,r7

;; fn00406CA8: 00406CA8
;;   Called from:
;;     00406C64 (in scopeof)
;;     00406C8C (in scopeof)
;;     00406C96 (in scopeof)
;;     00406CA4 (in scopeof)
;;     00406CA4 (in scopeof)
;;     00409CDE (in sift)
;;     00409D18 (in sift)
;;     00409D20 (in sift)
;;     00409DA0 (in trinkle)
;;     00409DFE (in trinkle)
;;     00409E06 (in trinkle)
fn00406CA8 proc
	jrc	ra

;; addrcmp: 00406CAA
addrcmp proc
	lw	r4,0018(r4)
	lw	r7,0018(r5)
	subu	r4,r7,r4
	jrc	ra

;; name_from_dns: 00406CB2
;;   Called from:
;;     004071A4 (in __lookup_name)
;;     004071C0 (in __lookup_name)
name_from_dns proc
	save	00000690,ra,00000007
	movep	r19,r21,r6,r7
	addiu	r7,sp,FFFFF670
	addiu	r17,sp,00000040
	movep	r16,r20,r7,r8
	addiu	r7,sp,00000158
	addiu	r18,sp,00000270
	sw	r7,09A8(r16)
	addiu	r7,sp,00000470
	sw	r18,09AC(r16)
	sw	r4,09C4(r16)
	sw	r5,09C8(r16)
	sw	r17,0014(sp)
	sw	r0,003C(sp)
	sw	r7,09B0(r16)
	beqic	r21,0000000A,00406D5A

l00406CE8:
	addiu	r7,r0,00000118
	move	r11,r17
	sw	r7,0000(sp)
	li	r7,00000001
	move	r6,r7
	move	r10,r0
	move	r9,r0
	move	r8,r0
	movep	r4,r5,r0,r19
	balc	00407690
	li	r7,FFFFFFFF
	sw	r4,09B4(r16)
	beqc	r4,r7,00406D8C

l00406D0A:
	li	r16,00000001
	bneiuc	r21,00000002,00406D5C

l00406D10:
	addiu	r19,sp,0000002C
	addiu	r7,sp,0000001C
	move	r10,r20
	addiu	r9,r0,00000200
	move	r8,r19
	addiu	r6,sp,00000024
	addiu	r5,sp,00000014
	move.balc	r4,r16,004077AA
	move	r17,r0
	addiu	r7,r0,FFFFFFF5
	bltc	r4,r0,00406D90

l00406D2E:
	bnec	r17,r16,00406D96

l00406D32:
	lw	r17,003C(sp)
	bnezc	r7,00406D90

l00406D36:
	lw	r17,002C(sp)
	addiu	r7,r0,FFFFFFFD
	bltic	r6,00000004,00406D90

l00406D40:
	lbu	r6,0273(sp)
	andi	r6,r6,0000000F
	beqic	r6,00000002,00406D90

l00406D4A:
	beqzc	r6,00406D8C

l00406D4C:
	xori	r6,r6,00000003
	addiu	r7,r0,FFFFFFFC
	movz	r7,r0,r6

l00406D58:
	bc	00406D90

l00406D5A:
	move	r16,r0

l00406D5C:
	addiu	r7,r0,00000118
	move	r10,r0
	mul	r11,r16,r7
	move	r9,r0
	sw	r7,0000(sp)
	move	r8,r0
	li	r7,0000001C
	li	r6,00000001
	movep	r4,r5,r0,r19
	addu	r11,r11,r17
	balc	00407690
	addiu	r7,sp,FFFFF670
	lsa	r7,r16,r7,00000002
	addiu	r16,r16,00000001
	sw	r4,09B4(r7)
	li	r7,FFFFFFFF
	bnec	r7,r4,00406D10

l00406D8C:
	addiu	r7,r0,FFFFFFFE

l00406D90:
	move	r4,r7
	restore.jrc	00000690,ra,00000007

l00406D96:
	sll	r4,r17,00000009
	lwxs	r5,r17(r19)
	addiu	r7,sp,00000034
	addiupc	r6,000001A8
	addu	r4,r18,r4
	addiu	r17,r17,00000001
	balc	0040D0D0
	bc	00406D2E

;; policyof: 00406DAC
;;   Called from:
;;     00407282 (in __lookup_name)
;;     004072E4 (in __lookup_name)
policyof proc
	save	00000020,ra,00000006
	addiupc	r16,0000C09E
	move	r19,r4
	move	r17,r0
	move	r20,r16
	bc	00406DBE

l00406DBA:
	addiu	r17,r17,00000001
	addiu	r16,r16,00000014

l00406DBE:
	lbu	r18,0010(r16)
	movep	r5,r6,r16,r18
	move.balc	r4,r19,0040A100
	bnezc	r4,00406DBA

l00406DCA:
	li	r7,00000014
	lbu	r4,0011(r16)
	mul	r7,r7,r17
	lbux	r6,r18(r19)
	addu	r7,r7,r20
	lbux	r5,r18(r7)
	and	r7,r6,r4
	bnec	r7,r5,00406DBA

l00406DE4:
	move	r4,r16
	restore.jrc	00000020,ra,00000006

;; is_valid_hostname: 00406DE8
;;   Called from:
;;     00406F1A (in name_from_hosts)
;;     00406FBE (in dns_parse_callback)
is_valid_hostname proc
	save	00000010,ra,00000002
	addiu	r5,r0,000000FF
	move	r16,r4
	balc	0040A940
	addiu	r7,r0,000000FD
	addiu	r4,r4,FFFFFFFF
	bgeuc	r7,r4,00406E02

l00406DFE:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

l00406E02:
	movep	r5,r6,r16,r0
	move	r4,r0
	balc	0040CFE0
	li	r7,FFFFFFFF
	bnec	r4,r7,00406E12

l00406E0E:
	bc	00406DFE

l00406E10:
	addiu	r16,r16,00000001

l00406E12:
	lbu	r4,0000(r16)
	seb	r7,r4
	bltc	r7,r0,00406E10

l00406E1C:
	addiu	r7,r4,FFFFFFD3
	andi	r7,r7,000000FF
	bltiuc	r7,00000002,00406E10

l00406E26:
	balc	0040B0A0
	bnezc	r4,00406E10

l00406E2C:
	lbu	r4,0000(r16)
	sltiu	r4,r4,00000001
	restore.jrc	00000010,ra,00000002

;; name_from_hosts: 00406E34
;;   Called from:
;;     004070E6 (in __lookup_name)
name_from_hosts proc
	save	000006E0,r30,0000000A
	movep	r22,r23,r4,r5
	move	r20,r6
	move	r30,r7
	move.balc	r4,r6,0040A890
	addiu	r7,r0,00000408
	sw	r4,000C(sp)
	addiu	r6,sp,000002A8
	addiu	r5,sp,00000018
	addiupc	r4,0000B0CA
	balc	00408070
	move	r19,r4
	beqzc	r4,00406E7A

l00406E5A:
	move	r16,r0
	move	r18,r0

l00406E5E:
	move	r6,r19
	addiu	r5,r0,00000200
	addiu	r4,sp,000000A8
	balc	00408240
	beqzc	r4,00406E70

l00406E6C:
	bltic	r18,00000030,00406EA6

l00406E70:
	movn	r16,r18,r18

l00406E74:
	move.balc	r4,r19,00408060
	bc	00406EA0

l00406E7A:
	balc	004049B0
	addiu	r16,r0,FFFFFFF5
	lw	r7,0000(r4)
	bgeiuc	r7,00000015,00406EA0

l00406E88:
	li	r16,00000001
	sllv	r16,r16,r7
	li	r7,00102004
	and	r16,r16,r7
	addiu	r7,r0,FFFFFFF5
	movn	r7,r0,r16

l00406E9E:
	move	r16,r7

l00406EA0:
	move	r4,r16
	restore.jrc	000006E0,r30,0000000A

l00406EA6:
	li	r5,00000023
	addiu	r4,sp,000000A8
	balc	0040A770
	beqzc	r4,00406EB6

l00406EB0:
	li	r7,0000000A
	sb	r0,0001(r4)
	sb	r7,0000(r4)

l00406EB6:
	addiu	r4,sp,000000A9
	bc	00406EC0

l00406EBC:
	addiu	r4,r6,00000001

l00406EC0:
	move.balc	r5,r20,0040AC28
	move	r6,r4
	beqzc	r4,00406E5E

l00406EC8:
	lbu	r4,-0001(r4)
	balc	00406F46
	beqzc	r4,00406EBC

l00406ED0:
	lw	r17,000C(sp)
	lbux	r4,r7(r6)
	balc	00406F46
	beqzc	r4,00406EBC

l00406EDA:
	addiu	r6,sp,000000A8

l00406EDC:
	lbu	r4,0000(r6)
	addiu	r17,r6,00000001
	beqzc	r4,00406EE8

l00406EE4:
	balc	00406F46
	beqzc	r4,00406F30

l00406EE8:
	li	r4,0000001C
	sb	r0,0000(r6)
	mul	r4,r4,r18
	addiu	r5,sp,000000A8
	move	r6,r30
	addu	r4,r4,r22
	balc	00406B50
	beqc	r0,r4,00406E5E

l00406EFC:
	bneiuc	r4,00000001,00406F40

l00406F00:
	addiu	r18,r18,00000001

l00406F02:
	lbu	r4,0000(r17)
	beqzc	r4,00406F3C

l00406F06:
	balc	00406F46
	bnezc	r4,00406F34

l00406F0A:
	move	r21,r17

l00406F0C:
	lbu	r4,0000(r21)
	beqzc	r4,00406F16

l00406F12:
	balc	00406F46
	beqzc	r4,00406F38

l00406F16:
	sb	r0,0000(r21)
	move.balc	r4,r17,00406DE8
	beqc	r0,r4,00406E5E

l00406F22:
	subu	r6,r21,r17
	addiu	r6,r6,00000001
	movep	r4,r5,r23,r17
	balc	0040A130
	bc	00406E5E

l00406F30:
	move	r6,r17
	bc	00406EDC

l00406F34:
	addiu	r17,r17,00000001
	bc	00406F02

l00406F38:
	addiu	r21,r21,00000001
	bc	00406F0C

l00406F3C:
	move	r21,r17
	bc	00406F16

l00406F40:
	addiu	r16,r0,FFFFFFFE
	bc	00406E5E

;; fn00406F46: 00406F46
;;   Called from:
;;     00406ECC (in name_from_hosts)
;;     00406ED6 (in name_from_hosts)
;;     00406EE4 (in name_from_hosts)
;;     00406F06 (in name_from_hosts)
;;     00406F12 (in name_from_hosts)
fn00406F46 proc
	bc	00406C30

;; fn00406F48: 00406F48
;;   Called from:
;;     0040D1AE (in __dns_parse)
fn00406F48 proc
	movep	r7,r7,r4,r5

;; dns_parse_callback: 00406F4A
;;   Called from:
;;     00406F48 (in fn00406F48)
dns_parse_callback proc
	save	00000110,ra,00000002
	movep	r16,r9,r4,r5
	movep	r4,r5,r8,r6
	beqic	r9,00000005,00406FAA

l00406F56:
	beqic	r9,0000001C,00406F8A

l00406F5A:
	bneiuc	r9,00000001,00406F84

l00406F5E:
	li	r4,FFFFFFFF
	bneiuc	r7,00000004,00406F86

l00406F64:
	lw	r6,0008(r16)
	li	r4,0000001C
	mul	r8,r6,r4
	addiu	r6,r6,00000001
	lw	r4,0000(r16)
	addu	r4,r4,r8
	addiu	r8,r0,00000002
	sw	r8,0000(r4)
	sw	r0,0004(sp)
	addiu	r4,r4,00000008
	sw	r6,0008(sp)
	move	r6,r7

l00406F80:
	balc	0040A130

l00406F84:
	move	r4,r0

l00406F86:
	restore.jrc	00000110,ra,00000002

l00406F8A:
	li	r4,FFFFFFFF
	bneiuc	r7,00000010,00406F86

l00406F90:
	lw	r6,0008(r16)
	lw	r4,0000(r16)
	mul	r9,r9,r6
	addiu	r6,r6,00000001
	addu	r9,r9,r4
	li	r4,0000000A
	sw	r4,0000(r9)
	addiu	r4,r9,00000008
	sw	r11,0001(r9)
	sw	r6,0008(sp)
	move	r6,r7
	bc	00406F80

l00406FAA:
	addiu	r8,r0,00000100
	move	r7,sp
	addiu	r5,r4,00000200
	balc	0040D000
	bgec	r0,r4,00406F84

l00406FBC:
	move	r4,sp
	balc	00406DE8
	beqzc	r4,00406F84

l00406FC4:
	lw	r4,0004(r16)
	move	r5,sp
	balc	0040A860
	bc	00406F84

;; __lookup_name: 00406FCE
;;   Called from:
;;     00405E80 (in getaddrinfo)
__lookup_name proc
	save	000001B0,r30,0000000A
	movep	r16,r21,r4,r5
	movep	r22,r19,r6,r7
	move	r18,r8
	sb	r0,0000(r21)
	beqc	r0,r22,00407006

l00406FE0:
	addiu	r5,r0,000000FF
	move.balc	r4,r22,0040A940
	addiu	r6,r0,000000FD
	addiu	r7,r4,FFFFFFFF
	bgeuc	r6,r7,00406FFE

l00406FF4:
	addiu	r17,r0,FFFFFFFE

l00406FF8:
	move	r4,r17
	restore.jrc	000001B0,r30,0000000A

l00406FFE:
	addiu	r6,r4,00000001
	movep	r4,r5,r21,r22
	balc	0040738E

l00407006:
	bbeqzc	r18,00000003,00407010

l0040700A:
	beqic	r19,0000000A,00407034

l0040700E:
	addiu	r18,r18,FFFFFFF8

l00407010:
	bnec	r0,r22,004070D2

l00407014:
	andi	r7,r18,00000001
	beqzc	r7,00407088

l00407018:
	beqic	r19,0000000A,00407038

l0040701C:
	li	r6,00000002
	sw	r0,0004(sp)
	sw	r6,0000(sp)
	sw	r0,0008(sp)
	sw	r0,000C(sp)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,0018(sp)
	bnec	r19,r6,0040703A

l0040702E:
	addiu	r20,r0,00000001
	bc	00407054

l00407034:
	move	r19,r0
	bc	00407010

l00407038:
	move	r7,r0

l0040703A:
	li	r6,0000001C
	addiu	r20,r7,00000001
	mul	r7,r7,r6
	li	r6,0000000A
	addu	r7,r16,r7
	sw	r6,0040(sp)
	sw	r0,0044(sp)
	sw	r0,0048(sp)
	sw	r0,004C(sp)
	sw	r0,0050(sp)
	sw	r0,0054(sp)
	sw	r0,0058(sp)

l00407054:
	move	r17,r20
	bbeqzc	r18,00000003,0040706A

l0040705A:
	andi	r18,r18,00000010
	beqc	r0,r18,0040720C

l00407062:
	addiu	r18,r16,00000014
	move	r17,r0

l00407066:
	bnec	r20,r17,00407210

l0040706A:
	bltic	r17,00000002,00406FF8

l0040706E:
	beqic	r19,00000002,00406FF8

l00407072:
	move	r7,r0

l00407074:
	li	r6,0000001C
	mul	r6,r6,r7
	lwx	r6,r6(r16)
	bneiuc	r6,00000002,00407238

l00407080:
	addiu	r7,r7,00000001
	bnec	r7,r17,00407074

l00407086:
	bc	00406FF8

l00407088:
	beqic	r19,0000000A,004070B0

l0040708C:
	li	r7,00000002
	sw	r0,0008(sp)
	sw	r7,0000(sp)
	addiu	r7,r0,0000007F
	sb	r7,0008(r16)
	li	r7,00000001
	sw	r0,0004(sp)
	sw	r0,000C(sp)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,0018(sp)
	sb	r7,000B(r16)
	beqic	r19,00000002,0040702E

l004070AE:
	li	r7,00000001

l004070B0:
	li	r6,0000001C
	addiu	r20,r7,00000001
	mul	r7,r7,r6
	li	r6,0000000A
	addu	r7,r16,r7
	sw	r6,0040(sp)
	li	r6,00000001
	sw	r0,0054(sp)
	sw	r0,0044(sp)
	sw	r0,0048(sp)
	sw	r0,004C(sp)
	sw	r0,0050(sp)
	sw	r0,0058(sp)
	sb	r6,0017(r7)
	bc	00407054

l004070D2:
	movep	r5,r6,r22,r19
	move.balc	r4,r16,00406B50
	move	r17,r4
	bnec	r0,r4,004071AC

l004070DE:
	bbnezc	r18,00000002,00406FF4

l004070E2:
	movep	r6,r7,r22,r19
	movep	r4,r5,r16,r21
	balc	00406E34
	move	r17,r4
	bnec	r0,r4,004071AC

l004070F0:
	addiu	r6,r0,00000100
	addiu	r5,sp,00000080
	addiu	r4,sp,0000001C
	li	r17,FFFFFFFF
	balc	00407B02
	bltc	r4,r0,00406FF8

l00407102:
	move	r7,r0
	move	r23,r0

l00407106:
	lbux	r6,r23(r22)
	bnezc	r6,0040714C

l0040710C:
	lw	r17,0078(sp)
	bgeuc	r7,r6,0040711E

l00407112:
	addu	r7,r22,r23
	lbu	r7,-0001(r7)
	bneiuc	r7,0000002E,00407122

l0040711E:
	sb	r0,0080(sp)

l00407122:
	addiu	r7,r0,000000FF
	addiu	r17,r0,FFFFFFFE
	bltuc	r7,r23,00406FF8

l0040712E:
	movep	r5,r6,r22,r23
	addu	r30,r21,r23
	move.balc	r4,r21,0040A130
	li	r7,0000002E
	sb	r7,0000(r30)
	addiu	r7,sp,00000080
	move	r20,r7

l00407142:
	lbu	r7,0000(r20)
	beqzc	r7,004071B4

l00407148:
	move	r5,r20
	bc	00407158

l0040714C:
	bneiuc	r6,0000002E,00407152

l00407150:
	addiu	r7,r7,00000001

l00407152:
	addiu	r23,r23,00000001
	bc	00407106

l00407156:
	addiu	r5,r5,00000001

l00407158:
	lbu	r4,0000(r5)
	balc	00406C30
	bnezc	r4,00407156

l00407160:
	move	r20,r5
	bc	00407166

l00407164:
	addiu	r20,r20,00000001

l00407166:
	lbu	r4,0000(r20)
	beqzc	r4,00407172

l0040716C:
	balc	00406C30
	beqzc	r4,00407164

l00407172:
	beqc	r5,r20,004071B4

l00407176:
	addiu	r7,r0,000000FF
	subu	r17,r20,r5
	subu	r7,r7,r23
	bgeuc	r17,r7,00407142

l00407186:
	addiu	r7,r23,00000001
	move	r6,r17
	addu	r4,r21,r7
	sw	r7,000C(sp)
	balc	0040738E
	lw	r17,000C(sp)
	addu	r17,r17,r21
	movep	r5,r6,r21,r21
	sbx	r0,r7(r17)
	addiu	r7,sp,0000001C
	move	r8,r7
	move	r7,r19
	move.balc	r4,r16,00406CB2
	move	r17,r4
	beqzc	r4,00407142

l004071AC:
	move	r20,r17
	bltc	r0,r17,00407054

l004071B2:
	bc	00406FF8

l004071B4:
	addiu	r7,sp,0000001C
	sb	r0,0000(r30)
	move	r8,r7
	movep	r4,r5,r16,r21
	movep	r6,r7,r22,r19
	balc	00406CB2
	move	r17,r4
	move	r20,r4
	bltc	r0,r4,00407054

l004071CC:
	bnec	r0,r4,00406FF8

l004071D0:
	bc	00406FF4

l004071D2:
	addiu	r21,r21,00000001
	bgec	r21,r20,00407062

l004071D8:
	li	r17,0000001C
	mul	r17,r17,r21
	lwx	r7,r17(r16)
	bneiuc	r7,0000000A,004071D2

l004071E4:
	bgec	r21,r20,00407062

l004071E8:
	addu	r17,r16,r17

l004071EA:
	lw	r7,0000(r17)
	bneiuc	r7,0000000A,00407200

l004071F0:
	li	r6,0000001C
	addiu	r22,r18,00000001
	mul	r18,r18,r6
	addu	r4,r16,r18
	move	r18,r22
	move.balc	r5,r17,0040A130

l00407200:
	addiu	r21,r21,00000001
	addiu	r17,r17,0000001C
	bnec	r21,r20,004071EA

l00407208:
	move	r20,r18
	bc	00407062

l0040720C:
	move	r21,r0
	bc	004071D8

l00407210:
	lw	r7,-0014(r18)
	bneiuc	r7,00000002,00407232

l00407218:
	addiu	r21,r18,FFFFFFF4
	li	r6,00000004
	movep	r4,r5,r18,r21
	balc	0040738E
	li	r6,0000000C
	addiupc	r5,0000BC18
	move.balc	r4,r21,0040A130
	li	r7,0000000A
	sw	r7,-0014(r18)

l00407232:
	addiu	r17,r17,00000001
	addiu	r18,r18,0000001C
	bc	00407066

l00407238:
	beqc	r17,r7,00406FF8

l0040723C:
	addiu	r5,sp,00000014
	li	r4,00000001
	balc	0040AE50
	addiu	r19,r16,00000008
	move	r20,r0

l00407248:
	li	r7,0000000A
	sw	r0,0084(sp)
	sh	r7,0080(sp)
	li	r7,FFFFFFFF
	sh	r7,0082(sp)
	lw	r7,-0004(r19)
	sw	r0,0088(sp)
	sw	r7,0098(sp)
	lw	r7,-0008(r19)
	sw	r0,008C(sp)
	sw	r0,0090(sp)
	sw	r0,0094(sp)
	bneiuc	r7,0000000A,00407370

l00407278:
	li	r6,00000010
	move	r5,r19
	addiu	r4,sp,00000088

l0040727E:
	balc	0040738E
	addiu	r4,sp,00000088
	balc	00406DAC
	move	r18,r4
	addiu	r4,sp,00000088
	balc	00406C42
	li	r6,00000011
	lbu	r7,0013(r18)
	move	r22,r4
	li	r5,00080001
	li	r4,0000000A
	lbu	r21,0012(r18)
	sw	r7,000C(sp)
	balc	00407D80
	move	r23,r4
	bltc	r4,r0,00407388

l004072AE:
	li	r6,0000001C
	addiu	r5,sp,00000080
	balc	00405DD0
	move	r18,r4
	bnec	r0,r4,00407382

l004072BC:
	li	r7,0000001C
	addiu	r6,sp,00000018
	addu	r5,sp,r7
	lui	r30,00040000
	sw	r7,0018(sp)
	move.balc	r4,r23,004066B0
	bnezc	r4,0040732A

l004072D0:
	addiu	r4,sp,00000024
	balc	00406C42
	lui	r6,00060000
	xor	r4,r22,r4
	movz	r30,r6,r4

l004072E2:
	addiu	r4,sp,00000024
	balc	00406DAC
	lw	r17,000C(sp)
	lbu	r6,0013(r4)
	bnec	r6,r7,004072F8

l004072F0:
	lui	r6,00010000
	or	r30,r30,r6

l004072F8:
	move	r4,r0

l004072FA:
	srl	r6,r4,00000003
	addiu	r7,sp,FFFFF180
	addu	r18,r7,r6
	addiu	r7,sp,00000180
	addu	r5,r7,r6
	lbu	r6,0EA4(r18)
	lbu	r5,-00F8(r5)
	addiu	r18,r0,00000080
	xor	r6,r6,r5
	andi	r5,r4,00000007
	andi	r6,r6,000000FF
	srav	r5,r18,r5
	and	r6,r6,r5
	bnezc	r6,00407328

l00407322:
	addiu	r4,r4,00000001
	bnec	r4,r18,004072FA

l00407328:
	move	r18,r4

l0040732A:
	move.balc	r4,r23,0040AF72

l0040732E:
	li	r4,0000000F
	li	r6,00000030
	subu	r4,r4,r22
	subu	r6,r6,r20
	sll	r4,r4,00000010
	sll	r21,r21,00000014
	or	r4,r4,r6
	addiu	r20,r20,00000001
	or	r4,r4,r21
	or	r7,r4,r30
	sll	r4,r18,00000008
	or	r4,r4,r7
	sw	r4,0050(sp)
	addiu	r19,r19,0000001C
	bnec	r17,r20,00407248

l0040735A:
	movep	r4,r5,r16,r17
	addiupc	r7,FFFFF94A
	li	r6,0000001C
	balc	00409E0E
	lw	r17,0014(sp)
	move	r5,r0
	balc	0040AE50
	bc	00406FF8

l00407370:
	li	r6,0000000C
	addiupc	r5,0000BACA
	addiu	r4,sp,00000088
	balc	0040738E
	li	r6,00000004
	move	r5,r19
	addiu	r4,sp,00000094
	bc	0040727E

l00407382:
	move	r18,r0
	move	r30,r0
	bc	0040732A

l00407388:
	move	r18,r0
	move	r30,r0
	bc	0040732E

;; fn0040738E: 0040738E
;;   Called from:
;;     00407004 (in __lookup_name)
;;     00407192 (in __lookup_name)
;;     00407220 (in __lookup_name)
;;     0040727E (in __lookup_name)
;;     00407378 (in __lookup_name)
fn0040738E proc
	bc	0040A130
00407392       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __lookup_serv: 004073A0
;;   Called from:
;;     00405E6C (in getaddrinfo)
__lookup_serv proc
	save	00000550,ra,00000009
	move	r19,r6
	addiupc	r6,000090CE
	movep	r18,r20,r4,r5
	move	r16,r8
	sw	r6,0004(sp)
	beqic	r7,00000001,0040740E

l004073B4:
	beqic	r7,00000002,00407402

l004073B8:
	bnezc	r7,00407418

l004073BA:
	beqc	r0,r20,0040742E

l004073BE:
	lbu	r7,0000(r20)
	beqzc	r7,00407408

l004073C4:
	li	r6,0000000A
	addiu	r5,sp,00000004
	move.balc	r4,r20,0040A022

l004073CC:
	lw	r17,0004(sp)
	lbu	r7,0000(r7)
	bnezc	r7,00407436

l004073D2:
	addiu	r7,r0,0000FFFF
	bltuc	r7,r4,00407408

l004073DA:
	andi	r4,r4,0000FFFF
	beqic	r19,00000011,00407432

l004073E0:
	li	r7,00000002
	li	r16,00000001
	sb	r7,0003(r18)
	li	r7,00000006
	sh	r4,0000(r18)
	sb	r7,0002(r18)
	beqic	r19,00000006,00407424

l004073F0:
	lsa	r18,r16,r18,00000002
	li	r7,00000001
	addiu	r16,r16,00000001
	sb	r7,0003(r18)
	li	r7,00000011
	sh	r4,0000(r18)
	sb	r7,0002(r18)
	bc	00407424

l00407402:
	beqzc	r19,0040742A

l00407404:
	beqic	r19,00000006,0040742A

l00407408:
	addiu	r16,r0,FFFFFFF8
	bc	00407424

l0040740E:
	beqzc	r19,00407414

l00407410:
	bneiuc	r19,00000011,00407408

l00407414:
	li	r19,00000011
	bc	004073BA

l00407418:
	bnec	r0,r20,00407408

l0040741C:
	li	r16,00000001
	sh	r0,0000(r18)
	sb	r19,0002(r18)
	sb	r7,0003(r18)

l00407424:
	move	r4,r16
	restore.jrc	00000550,ra,00000009

l0040742A:
	li	r19,00000006
	bc	004073BA

l0040742E:
	move	r4,r0
	bc	004073CC

l00407432:
	move	r16,r0
	bc	004073F0

l00407436:
	andi	r16,r16,00000400
	bnezc	r16,00407408

l0040743C:
	move.balc	r4,r20,0040A890
	addiu	r7,r0,00000408
	move	r22,r4
	addiu	r6,sp,00000118
	addiu	r5,sp,00000088
	addiupc	r4,0000AAD8
	balc	00408070
	move	r21,r4
	bnezc	r4,0040748E

l00407458:
	balc	004049B0
	addiu	r16,r0,FFFFFFF5
	lw	r6,0000(r4)
	bgeiuc	r6,00000015,00407424

l00407466:
	li	r7,00000001
	sllv	r7,r7,r6
	li	r6,00102004
	and	r7,r7,r6
	bnezc	r7,00407408

l00407476:
	bc	00407424

l00407478:
	li	r5,00000023
	move.balc	r4,r17,0040A770
	beqzc	r4,00407486

l00407480:
	li	r7,0000000A
	sb	r0,0001(r4)
	sb	r7,0000(r4)

l00407486:
	move	r4,r17

l00407488:
	move.balc	r5,r20,0040AC28
	bnezc	r4,004074AA

l0040748E:
	addiu	r17,sp,00000008
	move	r6,r21
	addiu	r5,r0,00000080
	move.balc	r4,r17,00408240
	beqzc	r4,004074A0

l0040749C:
	bltic	r16,00000002,00407478

l004074A0:
	move.balc	r4,r21,00408060
	bnec	r0,r16,00407424

l004074A8:
	bc	00407408

l004074AA:
	bgeuc	r17,r4,004074BE

l004074AE:
	lbu	r7,-0001(r4)
	beqic	r7,00000020,004074BE

l004074B6:
	addiu	r7,r7,FFFFFFF7
	bgeiuc	r7,00000005,004074D0

l004074BE:
	lbux	r7,r22(r4)
	andi	r6,r7,000000DF
	beqzc	r6,004074D6

l004074C8:
	addiu	r7,r7,FFFFFFF7
	bltiuc	r7,00000005,004074D6

l004074D0:
	addiu	r4,r4,00000001
	bc	00407488

l004074D4:
	addiu	r17,r17,00000001

l004074D6:
	lbu	r7,0000(r17)
	andi	r6,r7,000000DF
	beqzc	r6,004074E6

l004074DE:
	addiu	r7,r7,FFFFFFF7
	bgeiuc	r7,00000005,004074D4

l004074E6:
	li	r6,0000000A
	addiu	r5,sp,00000004
	move.balc	r4,r17,0040A022
	addiu	r7,r0,0000FFFF
	move	r23,r4
	bltuc	r7,r4,0040748E

l004074F8:
	lw	r17,0004(sp)
	beqc	r17,r4,0040748E

l004074FE:
	li	r6,00000004
	addiupc	r5,0000AA34
	balc	0040A8E0
	bnezc	r4,00407520

l0040750A:
	beqic	r19,00000006,0040748E

l0040750E:
	lsa	r7,r16,r18,00000002
	li	r6,00000001
	addiu	r16,r16,00000001
	sb	r6,0003(r7)
	li	r6,00000011
	sh	r23,0000(r7)
	sb	r6,0002(r7)

l00407520:
	lw	r17,0004(sp)
	li	r6,00000004
	addiupc	r5,0000AA18
	balc	0040A8E0
	bnec	r0,r4,0040748E

l00407530:
	beqic	r19,00000011,0040748E

l00407534:
	lsa	r7,r16,r18,00000002
	li	r6,00000002
	addiu	r16,r16,00000001
	sb	r6,0003(r7)
	li	r6,00000006
	sh	r23,0000(r7)
	sb	r6,0002(r7)
	bc	0040748E
00407548                         00 00 00 00 00 00 00 00         ........

;; __netlink_enumerate: 00407550
;;   Called from:
;;     004075F6 (in __rtnetlink_enumerate)
;;     00407606 (in __rtnetlink_enumerate)
__netlink_enumerate proc
	save	00000020,ra,00000007
	addiu	sp,sp,FFFFE000
	movep	r21,r16,r6,r7
	movep	r17,r20,r4,r5
	li	r6,00000014
	move	r5,r0
	move	r4,sp
	move	r18,r8
	move	r19,r9
	balc	0040A690
	addiu	r7,r0,00000301
	li	r6,00000014
	sh	r7,0006(sp)
	move	r5,sp
	move	r7,r0
	sw	r6,0000(sp)
	sh	r21,0004(sp)
	sw	r20,0008(sp)
	sb	r16,0010(sp)
	move.balc	r4,r17,00407D10
	bltc	r4,r0,004075C8

l0040758C:
	li	r7,00000040
	addiu	r6,r0,00002000
	move	r5,sp
	move.balc	r4,r17,00407640
	move	r20,r4
	bgec	r0,r4,004075C6

l0040759E:
	move	r16,sp

l004075A0:
	addu	r7,sp,r20
	subu	r7,r7,r16
	bltiuc	r7,00000010,0040758C

l004075AA:
	lhu	r7,0004(r16)
	beqic	r7,00000003,004075CE

l004075B0:
	beqic	r7,00000002,004075C6

l004075B4:
	movep	r4,r5,r19,r16
	jalrc	ra,r18
	bnezc	r4,004075C8

l004075BA:
	lw	r7,0000(r16)
	addiu	r7,r7,00000003
	ins	r7,r0,00000000,00000001
	addu	r16,r16,r7
	bc	004075A0

l004075C6:
	li	r4,FFFFFFFF

l004075C8:
	addiu	sp,sp,00002000
	restore.jrc	00000020,ra,00000007

l004075CE:
	move	r4,r0
	bc	004075C8

;; __rtnetlink_enumerate: 004075D2
;;   Called from:
;;     004062BC (in getifaddrs)
__rtnetlink_enumerate proc
	save	00000020,ra,00000007
	movep	r21,r20,r4,r5
	movep	r18,r19,r6,r7
	li	r5,00080003
	move	r6,r0
	li	r4,00000010
	li	r16,FFFFFFFF
	balc	00407D80
	move	r17,r4
	bltc	r4,r0,00407612

l004075EE:
	move	r9,r19
	li	r6,00000012
	movep	r7,r8,r21,r18
	li	r5,00000001
	balc	00407550
	move	r16,r4
	bnezc	r4,0040760C

l004075FE:
	move	r9,r19
	li	r6,00000016
	movep	r7,r8,r20,r18
	li	r5,00000002
	move.balc	r4,r17,00407550
	move	r16,r4

l0040760C:
	li	r4,00000039
	move.balc	r5,r17,00404A50

l00407612:
	move	r4,r16
	restore.jrc	00000020,ra,00000007
00407616                   00 00 00 00 00 00 00 00 00 00       ..........

;; ntohl: 00407620
;;   Called from:
;;     00400CE4 (in ping4_run)
;;     00400EC6 (in ping4_run)
;;     0040193C (in fn0040193C)
;;     00403240 (in ping6_parse_reply)
;;     00403C12 (in ping6_run)
ntohl proc
	rotx	r4,r4,00000008,00000018,00000000
	jrc	ra
00407626                   00 00 00 00 00 00 00 00 00 00       ..........

;; ntohs: 00407630
;;   Called from:
;;     004009F2 (in pr_echo_reply)
;;     00401B9C (in fn00401B9C)
;;     004030A4 (in fn004030A4)
;;     004065CA (in getnameinfo)
ntohs proc
	rotx	r4,r4,00000018,00000008,00000000
	andi	r4,r4,0000FFFF
	jrc	ra
00407638                         00 00 00 00 00 00 00 00         ........

;; recv: 00407640
;;   Called from:
;;     00407594 (in __netlink_enumerate)
recv proc
	move	r9,r0
	move	r8,r0
	bc	00407650
00407648                         00 00 00 00 00 00 00 00         ........

;; recvfrom: 00407650
;;   Called from:
;;     00407644 (in recv)
;;     00407A06 (in __res_msend_rc)
recvfrom proc
	save	00000010,ra,00000001
	move	r10,r9
	move	r9,r8
	move	r8,r7
	move	r7,r6
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000CF
	balc	0040ADA4
	restore	00000010,ra,00000001
	bc	0040CC30
0040766E                                           00 00               ..

;; recvmsg: 00407670
;;   Called from:
;;     004017BA (in ping4_receive_error_msg)
;;     00402B6C (in main_loop)
;;     00402F38 (in ping6_receive_error_msg)
recvmsg proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000D4
	balc	0040ADA4
	restore	00000010,ra,00000001
	bc	0040CC30
0040768C                                     00 00 00 00             ....

;; res_mkquery: 00407690
;;   Called from:
;;     0040646E (in getnameinfo)
;;     00406CFC (in name_from_dns)
;;     00406D74 (in name_from_dns)
res_mkquery proc
	save	00000150,r30,0000000A
	move	r30,r5
	move	r21,r4
	addiu	r5,r0,000000FF
	move	r4,r30
	movep	r22,r23,r6,r7
	move	r20,r11
	lw	r18,0150(sp)
	balc	0040A940
	move	r17,r4
	beqzc	r4,004076BE

l004076AE:
	addiu	r6,r4,FFFFFFFF
	lbux	r7,r6(r30)
	xori	r7,r7,0000002E
	movz	r17,r6,r7

l004076BE:
	sltu	r7,r0,r17
	addiu	r16,r17,00000011
	addu	r16,r16,r7
	addiu	r7,r0,000000FD
	move	r19,r16
	bgeuc	r7,r17,004076DA

l004076D2:
	li	r19,FFFFFFFF

l004076D4:
	move	r4,r19
	restore.jrc	00000150,r30,0000000A

l004076DA:
	bltc	r18,r16,004076D2

l004076DE:
	bgeiuc	r21,00000010,004076D2

l004076E2:
	addiu	r7,r0,000000FF
	bltuc	r7,r22,004076D2

l004076EA:
	bltuc	r7,r23,004076D2

l004076EE:
	addiu	r18,sp,00000008
	sll	r21,r21,00000003
	movep	r5,r6,r0,r16
	addiu	r21,r21,00000001
	move.balc	r4,r18,0040A690
	li	r6,00000001
	sb	r6,000D(sp)
	move	r5,r30
	move	r6,r17
	addiu	r4,sp,00000015
	sb	r21,000A(sp)
	balc	0040A130
	li	r6,0000000D

l00407714:
	addiu	r7,sp,FFFFF120
	addu	r7,r7,r6
	lbu	r5,0EE8(r7)
	beqzc	r5,0040774A

l00407720:
	move	r7,r6
	bc	00407726

l00407724:
	addiu	r7,r7,00000001

l00407726:
	lbux	r5,r7(r18)
	beqzc	r5,00407730

l0040772C:
	bneiuc	r5,0000002E,00407724

l00407730:
	subu	r5,r7,r6
	addiu	r4,r5,FFFFFFFF
	bgeiuc	r4,0000003F,004076D2

l0040773A:
	addiu	r4,sp,FFFFF120
	addu	r6,r4,r6
	sb	r5,0EE7(r6)
	addiu	r6,r7,00000001
	bc	00407714

l0040774A:
	move	r5,sp
	move	r4,r0
	sb	r23,0EE9(r7)
	sb	r22,0EEB(r7)
	balc	0040AEF4
	lw	r17,0004(sp)
	movep	r4,r5,r20,r18
	srl	r7,r6,00000010
	addu	r7,r7,r6
	andi	r7,r7,0000FFFF
	sra	r6,r7,00000008
	sb	r7,0009(sp)
	sb	r6,0008(sp)
	move	r6,r16
	balc	0040A130
	bc	004076D4
0040777A                               00 00 00 00 00 00           ......

;; mtime: 00407780
;;   Called from:
;;     004078C8 (in __res_msend_rc)
;;     00407912 (in __res_msend_rc)
mtime proc
	save	00000020,ra,00000001
	move	r4,r0
	addiu	r5,sp,00000008
	balc	0040AEF4
	lw	r17,0008(sp)
	addiu	r4,r0,000003E8
	li	r6,000F4240
	mul	r7,r7,r4
	lw	r17,000C(sp)
	div	r4,r5,r6
	addu	r4,r7,r4
	restore.jrc	00000020,ra,00000001

;; cleanup: 004077A2
cleanup proc
	move	r5,r4
	li	r4,00000039
	bc	00404A50

;; __res_msend_rc: 004077AA
;;   Called from:
;;     00406D20 (in name_from_dns)
;;     00407AAC (in __res_msend)
__res_msend_rc proc
	save	000000F0,r30,0000000A
	move	r30,r10
	swm	r6,0010(sp),00000002
	sw	r5,0008(sp)
	li	r6,00000054
	move	r5,r0
	move	r18,r4
	addiu	r4,sp,0000006C
	addiu	r23,r10,00000008
	sw	r8,0004(sp)
	addiu	r16,sp,0000006C
	sw	r9,0020(sp)
	li	r17,00000002
	sw	r0,0050(sp)
	move	r20,r0
	sw	r0,0054(sp)
	li	r19,00000010
	sw	r0,0058(sp)
	sw	r0,005C(sp)
	sw	r0,0060(sp)
	sw	r0,0064(sp)
	sw	r0,0068(sp)
	balc	0040A690
	addiu	r5,sp,00000030
	li	r4,00000001
	balc	00407A82
	lw	r6,0060(r30)
	addiu	r7,r0,000003E8
	lw	r22,0058(r30)
	mul	r7,r7,r6
	sw	r7,000C(sp)

l004077F4:
	lw	r7,0054(r30)
	bltuc	r20,r7,00407850

l004077FC:
	move	r6,r0
	li	r5,00080081
	sh	r17,0050(sp)
	move.balc	r4,r17,00407D80
	move	r16,r4
	bgec	r4,r0,00407836

l00407812:
	bneiuc	r17,0000000A,00407846

l00407816:
	balc	004049B0
	lw	r7,0000(r4)
	bneiuc	r7,00000021,00407846

l00407820:
	move	r6,r0
	li	r5,00080081
	li	r4,00000002
	balc	00407D80
	move	r16,r4
	bltc	r4,r0,00407846

l00407834:
	li	r17,00000002

l00407836:
	move	r6,r19
	addiu	r5,sp,00000050
	move.balc	r4,r16,00405DB0
	bgec	r4,r0,00407898

l00407842:
	move.balc	r4,r16,0040AF72

l00407846:
	lw	r17,0030(sp)
	move	r5,r0
	balc	00407A82
	li	r4,FFFFFFFF
	restore.jrc	000000F0,r30,0000000A

l00407850:
	lw	r7,-0008(r23)
	bneiuc	r7,00000002,00407878

l00407858:
	li	r6,00000004
	addu	r4,r16,r6
	sw	r7,0018(sp)
	move.balc	r5,r23,0040A130
	li	r4,00000035
	balc	00406700
	lw	r17,0018(sp)
	sh	r4,0002(r16)
	sh	r7,0000(r16)

l0040786E:
	addiu	r20,r20,00000001
	addiu	r16,r16,0000001C
	addiu	r23,r23,0000001C
	bc	004077F4

l00407878:
	li	r6,00000010
	addiu	r4,r16,00000008
	li	r17,0000000A
	move.balc	r5,r23,0040A130
	li	r4,00000035
	balc	00406700
	lw	r7,-0004(r23)
	li	r19,0000001C
	sh	r4,0002(r16)
	sw	r7,0018(sp)
	li	r7,0000000A
	sh	r7,0000(r16)
	bc	0040786E

l00407898:
	addiupc	r5,FFFFFF06
	addiu	r4,sp,00000044
	move	r6,r16
	balc	0040AE32
	lw	r17,0030(sp)
	move	r5,r0
	balc	00407A82
	beqic	r17,0000000A,0040791A

l004078AE:
	lw	r17,0004(sp)
	sll	r6,r18,00000002
	move	r5,r0
	move	r17,r0
	balc	0040A690
	li	r7,00000001
	sh	r7,0040(sp)
	sw	r16,003C(sp)
	lw	r17,000C(sp)
	div	r23,r7,r22
	balc	00407780
	sll	r7,r18,00000001
	sw	r7,001C(sp)
	li	r7,0000001C
	mul	r7,r7,r20
	addiu	r6,sp,0000006C
	move	r22,r4
	subu	r30,r4,r23
	move	r10,r4
	addu	r7,r6,r7
	sw	r7,0018(sp)

l004078E2:
	lw	r17,000C(sp)
	subu	r7,r10,r22
	bgeuc	r7,r6,00407A76

l004078EC:
	subu	r7,r10,r30
	bltuc	r7,r23,004078FE

l004078F4:
	move	r21,r0

l004078F6:
	bltc	r21,r18,0040795E

l004078FA:
	lw	r5,001C(sp)
	move	r30,r10

l004078FE:
	subu	r6,r23,r10
	li	r5,00000001
	addu	r6,r6,r30
	addiu	r4,sp,0000003C
	balc	00407E20
	bltc	r0,r4,004079EE

l00407912:
	balc	00407780
	move	r10,r4
	bc	004078E2

l0040791A:
	addiu	r8,r0,00000004
	addiu	r7,sp,00000034
	li	r6,0000001A
	li	r5,00000029
	addiu	r17,sp,00000070
	move	r23,r0
	sw	r0,0034(sp)
	move.balc	r4,r16,00407D60

l0040792E:
	beqc	r20,r23,004078AE

l00407932:
	lhu	r7,-0004(r17)
	bneiuc	r7,00000002,00407958

l0040793A:
	li	r6,00000004
	addiu	r4,r17,00000010
	move.balc	r5,r17,0040A130
	li	r6,0000000C
	addiupc	r5,0000B580
	addiu	r4,r17,00000004
	balc	0040A130
	li	r7,0000000A
	sh	r7,-0004(r17)
	sw	r0,0040(sp)
	sw	r0,0054(sp)

l00407958:
	addiu	r23,r23,00000001
	addiu	r17,r17,0000001C
	bc	0040792E

l0040795E:
	lw	r17,0004(sp)
	lwxs	r7,r21(r7)
	beqzc	r7,00407992

l00407966:
	addiu	r21,r21,00000001
	bc	004078F6

l0040796A:
	lw	r17,0008(sp)
	move	r8,r30
	lw	r17,0010(sp)
	move	r9,r19
	addiu	r7,r0,00004000
	lwxs	r5,r21(r5)
	addiu	r30,r30,0000001C
	lwxs	r6,r21(r6)
	sw	r10,0024(sp)
	move.balc	r4,r16,00407D40
	lw	r18,0024(sp)

l0040798A:
	lw	r17,0018(sp)
	bnec	r7,r30,0040796A

l00407990:
	bc	00407966

l00407992:
	addiu	r7,sp,0000006C
	move	r30,r7
	bc	0040798A

l00407998:
	addiu	r8,r8,00000001
	bc	00407A18

l0040799C:
	addiu	r7,r7,00000001

l0040799E:
	bgec	r7,r18,004079EA

l004079A2:
	lw	r17,0008(sp)
	lw	r5,0000(r10)
	lwxs	r6,r7(r6)
	lbu	r11,0000(r5)
	lbu	r4,0000(r6)
	bnec	r11,r4,0040799C

l004079B2:
	lbu	r5,0001(r5)
	lbu	r6,0001(r6)
	bnec	r6,r5,0040799C

l004079BA:
	lw	r17,0004(sp)
	sll	r4,r7,00000002
	addu	r11,r6,r4
	lw	r6,0000(r11)
	bnezc	r6,004079EE

l004079C6:
	lw	r5,0000(r10)
	lbu	r6,0003(r5)
	andi	r6,r6,0000000F
	beqic	r6,00000002,00407A40

l004079D0:
	beqic	r6,00000003,004079D6

l004079D4:
	bnezc	r6,004079EE

l004079D6:
	sw	r9,0000(r11)
	bnec	r7,r17,00407A66

l004079DC:
	bgec	r17,r18,00407A72

l004079E0:
	lw	r17,0004(sp)
	lwxs	r7,r17(r7)
	beqzc	r7,004079EE

l004079E6:
	addiu	r17,r17,00000001
	bc	004079DC

l004079EA:
	bnec	r7,r18,004079BA

l004079EE:
	lw	r17,0014(sp)
	lsa	r10,r17,r7,00000002

l004079F4:
	addiu	r7,sp,00000050
	lw	r5,0000(r10)
	lw	r17,0020(sp)
	addiu	r9,sp,00000038
	move	r8,r7
	move	r7,r0
	sw	r10,0024(sp)
	sw	r19,0038(sp)
	move.balc	r4,r16,00407650
	move	r9,r4
	lw	r18,0024(sp)
	bltc	r4,r0,00407912

l00407A12:
	bltic	r9,00000004,004079F4

l00407A16:
	move	r8,r0

l00407A18:
	sw	r10,0028(sp)
	sw	r9,002C(sp)
	beqc	r20,r8,004079EE

l00407A20:
	li	r4,0000001C
	addiu	r7,sp,0000006C
	mul	r4,r4,r8
	move	r6,r19
	addiu	r5,sp,00000050
	sw	r8,0024(sp)
	addu	r4,r7,r4
	balc	0040A100
	lw	r18,0024(sp)
	lw	r18,0028(sp)
	lw	r18,002C(sp)
	bnec	r0,r4,00407998

l00407A3C:
	move	r7,r17
	bc	0040799E

l00407A40:
	beqc	r0,r21,004079EE

l00407A44:
	li	r7,0000001C
	lw	r17,0008(sp)
	mul	r8,r8,r7
	addiu	r7,sp,0000006C
	lw	r17,0010(sp)
	move	r9,r19
	lwx	r5,r4(r5)
	addiu	r21,r21,FFFFFFFF
	lwx	r6,r4(r6)
	addu	r8,r8,r7
	addiu	r7,r0,00004000
	move.balc	r4,r16,00407D40
	bc	004079EE

l00407A66:
	lw	r17,0014(sp)
	move	r6,r9
	lwx	r4,r4(r7)
	balc	0040A130

l00407A72:
	bnec	r18,r17,004079EE

l00407A76:
	addiu	r4,sp,00000044
	li	r5,00000001
	balc	0040AE3A
	move	r4,r0
	restore.jrc	000000F0,r30,0000000A

;; fn00407A82: 00407A82
;;   Called from:
;;     004077E2 (in __res_msend_rc)
;;     0040784A (in __res_msend_rc)
;;     004078A8 (in __res_msend_rc)
fn00407A82 proc
	bc	0040AE50

;; __res_msend: 00407A86
;;   Called from:
;;     00407AD6 (in res_send)
__res_msend proc
	save	00000090,ra,00000004
	movep	r16,r17,r4,r5
	move	r18,r6
	addiu	r4,sp,0000001C
	movep	r5,r6,r0,r0
	sw	r9,0004(sp)
	sw	r8,0008(sp)
	sw	r7,000C(sp)
	balc	00407B02
	li	r6,FFFFFFFF
	bltc	r4,r0,00407AB2

l00407AA0:
	addiu	r7,sp,0000001C
	lw	r18,0004(sp)
	move	r10,r7
	lw	r18,0008(sp)
	lw	r17,000C(sp)
	movep	r5,r6,r17,r18
	move.balc	r4,r16,004077AA
	move	r6,r4

l00407AB2:
	move	r4,r6
	restore.jrc	00000090,ra,00000004
00407AB6                   00 00 00 00 00 00 00 00 00 00       ..........

;; res_send: 00407AC0
;;   Called from:
;;     00406480 (in getnameinfo)
res_send proc
	save	00000020,ra,00000001
	move	r9,r7
	sw	r7,0000(sp)
	sw	r6,0004(sp)
	move	r8,sp
	sw	r5,0008(sp)
	addiu	r7,sp,00000004
	sw	r4,000C(sp)
	addiu	r6,sp,00000008
	addiu	r5,sp,0000000C
	li	r4,00000001
	balc	00407A86
	bltc	r4,r0,00407AE0

l00407ADE:
	lw	r17,0000(sp)

l00407AE0:
	restore.jrc	00000020,ra,00000001
00407AE2       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __isspace: 00407AF0
;;   Called from:
;;     00407D08 (in fn00407D08)
__isspace proc
	li	r7,00000001
	beqic	r4,00000020,00407AFE

l00407AF6:
	addiu	r4,r4,FFFFFFF7
	sltiu	r7,r4,00000005

l00407AFE:
	move	r4,r7
	jrc	ra

;; __get_resolv_conf: 00407B02
;;   Called from:
;;     004070FA (in __lookup_name)
;;     00407A96 (in __res_msend)
__get_resolv_conf proc
	save	000002C0,ra,00000008
	movep	r16,r20,r4,r5
	li	r7,00000001
	move	r21,r6
	sw	r7,005C(r16)
	li	r7,00000005
	sw	r7,0060(r16)
	li	r7,00000002
	sw	r7,0058(r16)
	beqc	r0,r20,00407B24

l00407B20:
	sb	r0,0000(r20)

l00407B24:
	addiu	r7,r0,00000100
	addiu	r6,sp,000001A0
	addiu	r5,sp,00000010
	addiupc	r4,0000A45E
	balc	00408070
	move	r17,r0
	move	r19,r4
	bnezc	r4,00407B8C

l00407B3C:
	balc	004049B0
	lw	r6,0000(r4)
	li	r4,FFFFFFFF
	bgeiuc	r6,00000015,00407B6A

l00407B48:
	li	r7,00000001
	sllv	r7,r7,r6
	li	r6,00102004
	and	r7,r7,r6
	beqzc	r7,00407B6A

l00407B58:
	move	r6,r0
	addiupc	r5,0000A446
	li	r17,00000001
	move.balc	r4,r16,00406B50

l00407B64:
	move	r4,r0
	sw	r17,0054(r16)

l00407B6A:
	restore.jrc	000002C0,ra,00000008

l00407B6E:
	li	r5,0000000A
	addiu	r4,sp,000000A0
	balc	0040A770
	bnezc	r4,00407BA2

l00407B78:
	lw	r7,0000(r19)
	bbnezc	r7,00000004,00407BA2

l00407B7E:
	move.balc	r4,r19,004085C0
	beqic	r4,0000000A,00407B8C

l00407B86:
	li	r7,FFFFFFFF
	bnec	r7,r4,00407B7E

l00407B8C:
	move	r6,r19
	addiu	r5,r0,00000100
	addiu	r4,sp,000000A0
	balc	00408240
	bnezc	r4,00407B6E

l00407B9A:
	move.balc	r4,r19,00408060
	bnezc	r17,00407B64

l00407BA0:
	bc	00407B58

l00407BA2:
	li	r6,00000007
	addiupc	r5,0000A408
	addiu	r4,sp,000000A0
	balc	00407D00
	bnec	r0,r4,00407C66

l00407BB0:
	lbu	r4,00A7(sp)
	addiu	r7,sp,FFFFF2A0
	move	r22,r7
	balc	00407D08
	beqc	r0,r4,00407C66

l00407BC0:
	addiupc	r5,0000A3F4
	addiu	r4,sp,000000A0
	balc	00407D04
	beqzc	r4,00407BF6

l00407BCA:
	lbu	r7,0006(r4)
	addiu	r7,r7,FFFFFFD0
	bgeiuc	r7,0000000A,00407BF6

l00407BD6:
	addiu	r18,r4,00000006
	li	r6,0000000A
	addiu	r5,sp,0000000C
	move.balc	r4,r18,0040A022
	lw	r7,0D6C(r22)
	beqc	r18,r7,00407BF6

l00407BE8:
	sltiu	r7,r4,00000010
	li	r6,0000000F
	movz	r4,r6,r7

l00407BF2:
	sw	r4,005C(r16)

l00407BF6:
	addiupc	r5,0000A3C6
	addiu	r4,sp,000000A0
	balc	00407D04
	beqzc	r4,00407C2A

l00407C00:
	lbu	r7,0009(r4)
	addiu	r7,r7,FFFFFFD0
	bgeiuc	r7,0000000A,00407C2A

l00407C0C:
	addiu	r18,r4,00000009
	li	r6,0000000A
	addiu	r5,sp,0000000C
	move.balc	r4,r18,0040A022
	lw	r17,000C(sp)
	beqc	r18,r7,00407C2A

l00407C1C:
	sltiu	r7,r4,0000000B
	li	r6,0000000A
	movz	r4,r6,r7

l00407C26:
	sw	r4,0058(r16)

l00407C2A:
	addiupc	r5,0000A39E
	addiu	r4,sp,000000A0
	balc	00407D04
	beqc	r0,r4,00407B8C

l00407C36:
	lbu	r7,0008(r4)
	addiu	r6,r7,FFFFFFD0
	bltiuc	r6,0000000A,00407C46

l00407C42:
	bneiuc	r7,0000002E,00407B8C

l00407C46:
	addiu	r18,r4,00000008
	li	r6,0000000A
	addiu	r5,sp,0000000C
	move.balc	r4,r18,0040A022
	lw	r17,000C(sp)
	beqc	r18,r7,00407B8C

l00407C56:
	sltiu	r7,r4,0000003D
	li	r6,0000003C
	movz	r4,r6,r7

l00407C60:
	sw	r4,0060(r16)
	bc	00407B8C

l00407C66:
	li	r6,0000000A
	addiupc	r5,0000A36C
	addiu	r4,sp,000000A0
	balc	00407D00
	bnezc	r4,00407CB4

l00407C72:
	lbu	r4,00AA(sp)
	balc	00407D08
	beqzc	r4,00407CB4

l00407C7A:
	addiu	r5,sp,000000AB
	bgeic	r17,00000003,00407B8C

l00407C82:
	lbu	r4,0000(r5)
	balc	00407D08
	bnezc	r4,00407CA6

l00407C88:
	sw	r5,000C(sp)

l00407C8A:
	lw	r17,000C(sp)
	lbu	r4,0000(r6)
	bnezc	r4,00407CAA

l00407C90:
	li	r4,0000001C
	sb	r0,0000(r6)
	mul	r4,r4,r17
	move	r6,r0
	addu	r4,r16,r4
	balc	00406B50
	bgec	r0,r4,00407B8C

l00407CA2:
	addiu	r17,r17,00000001
	bc	00407B8C

l00407CA6:
	addiu	r5,r5,00000001
	bc	00407C82

l00407CAA:
	balc	00407D08
	bnezc	r4,00407C90

l00407CAE:
	addiu	r6,r6,00000001
	sw	r6,000C(sp)
	bc	00407C8A

l00407CB4:
	beqc	r0,r20,00407B8C

l00407CB8:
	li	r6,00000006
	addiupc	r5,0000A326
	addiu	r4,sp,000000A0
	balc	00407D00
	bnezc	r4,00407CEC

l00407CC4:
	lbu	r4,00A6(sp)
	addiu	r18,sp,000000A7
	balc	00407D08
	beqc	r0,r4,00407B8C

l00407CD2:
	lbu	r4,0000(r18)
	balc	00407D08
	bnezc	r4,00407CFC

l00407CD8:
	move.balc	r4,r18,0040A890
	bgeuc	r4,r21,00407B8C

l00407CE0:
	addiu	r6,r4,00000001
	movep	r4,r5,r20,r18
	balc	0040A130
	bc	00407B8C

l00407CEC:
	li	r6,00000006
	addiupc	r5,0000A2FA
	addiu	r4,sp,000000A0
	balc	00407D00
	bnec	r0,r4,00407B8C

l00407CFA:
	bc	00407CC4

l00407CFC:
	addiu	r18,r18,00000001
	bc	00407CD2

;; fn00407D00: 00407D00
;;   Called from:
;;     00407BAA (in __get_resolv_conf)
;;     00407C6E (in __get_resolv_conf)
;;     00407CC0 (in __get_resolv_conf)
;;     00407CF4 (in __get_resolv_conf)
fn00407D00 proc
	bc	0040A8E0

;; fn00407D04: 00407D04
;;   Called from:
;;     00407BC6 (in __get_resolv_conf)
;;     00407BFC (in __get_resolv_conf)
;;     00407C30 (in __get_resolv_conf)
fn00407D04 proc
	bc	0040AC28

;; fn00407D08: 00407D08
;;   Called from:
;;     00407BBA (in __get_resolv_conf)
;;     00407C76 (in __get_resolv_conf)
;;     00407C84 (in __get_resolv_conf)
;;     00407CAA (in __get_resolv_conf)
;;     00407CCC (in __get_resolv_conf)
;;     00407CD4 (in __get_resolv_conf)
fn00407D08 proc
	bc	00407AF0
00407D0C                                     00 00 00 00             ....

;; send: 00407D10
;;   Called from:
;;     00407584 (in __netlink_enumerate)
send proc
	move	r9,r0
	move	r8,r0
	bc	00407D40
00407D18                         00 00 00 00 00 00 00 00         ........

;; sendmsg: 00407D20
;;   Called from:
;;     00403576 (in ping6_send_probe)
sendmsg proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000D3
	balc	0040ADA4
	restore	00000010,ra,00000001
	bc	0040CC30
00407D3C                                     00 00 00 00             ....

;; sendto: 00407D40
;;   Called from:
;;     00400ADE (in ping4_send_probe)
;;     0040353A (in ping6_send_probe)
;;     00407984 (in __res_msend_rc)
;;     00407A60 (in __res_msend_rc)
;;     00407D14 (in send)
sendto proc
	save	00000010,ra,00000001
	move	r10,r9
	move	r9,r8
	move	r8,r7
	move	r7,r6
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000CE
	balc	0040ADA4
	restore	00000010,ra,00000001
	bc	0040CC30
00407D5E                                           00 00               ..

;; setsockopt: 00407D60
;;   Called from:
;;     004007A0 (in main)
;;     004007CE (in main)
;;     00400B4A (in fn00400B4A)
;;     00400CD0 (in ping4_run)
;;     00400DB8 (in ping4_run)
;;     00401280 (in fn00401280)
;;     004018C6 (in ping4_receive_error_msg)
;;     004023EE (in fn004023EE)
;;     004032EA (in ping6_install_filter)
;;     00403762 (in ping6_run)
;;     00403784 (in ping6_run)
;;     00403C9A (in fn00403C9A)
;;     0040792A (in __res_msend_rc)
setsockopt proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r8
	move	r8,r7
	move	r7,r6
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000D0
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
00407D7E                                           00 00               ..

;; socket: 00407D80
;;   Called from:
;;     00400974 (in create_socket)
;;     00400C84 (in ping4_run)
;;     004036F4 (in ping6_run)
;;     0040671E (in if_indextoname)
;;     0040676E (in if_nametoindex)
;;     004072A4 (in __lookup_name)
;;     004075E4 (in __rtnetlink_enumerate)
;;     00407808 (in __res_msend_rc)
;;     0040782A (in __res_msend_rc)
socket proc
	save	00000020,ra,00000005
	movep	r18,r17,r4,r5
	move	r19,r6
	move	r10,r0
	move	r9,r0
	addiu	r4,r0,000000C6
	movep	r7,r8,r19,r0
	movep	r5,r6,r18,r17
	balc	00404A50
	balc	0040CC30
	move	r16,r4
	bgec	r4,r0,00407E00

l00407DA0:
	balc	004049B0
	lw	r7,0000(r4)
	bneiuc	r7,00000016,00407DF6

l00407DAA:
	li	r7,00080080
	and	r7,r7,r17
	beqzc	r7,00407E00

l00407DB4:
	li	r6,FFF7FF7F
	move	r10,r0
	move	r9,r0
	and	r6,r6,r17
	movep	r7,r8,r19,r0
	addiu	r4,r0,000000C6
	move.balc	r5,r18,00404A50
	balc	0040CC30
	move	r16,r4
	bltc	r4,r0,00407E00

l00407DD4:
	bbeqzc	r17,00000013,00407DE4

l00407DD8:
	move	r5,r4
	li	r7,00000001
	li	r6,00000002
	li	r4,00000019
	balc	00404A50

l00407DE4:
	bbeqzc	r17,00000007,00407E00

l00407DE8:
	addiu	r7,r0,00000800
	li	r6,00000004
	li	r4,00000019
	move.balc	r5,r16,00404A50
	bc	00407E00

l00407DF6:
	balc	004049B0
	lw	r7,0000(r4)
	beqic	r7,0000001D,00407DAA

l00407E00:
	move	r4,r16
	restore.jrc	00000020,ra,00000005
00407E04             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; sched_yield: 00407E10
;;   Called from:
;;     00402BD4 (in main_loop)
sched_yield proc
	save	00000010,ra,00000001
	li	r4,0000007C
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30

;; poll: 00407E20
;;   Called from:
;;     00402CC2 (in main_loop)
;;     0040790A (in __res_msend_rc)
poll proc
	save	00000020,ra,00000001
	move	r7,r0
	bltc	r6,r0,00407E42

l00407E28:
	addiu	r8,r0,000003E8
	div	r7,r6,r8
	sw	r7,0008(sp)
	mod	r7,r6,r8
	li	r6,000F4240
	mul	r7,r7,r6
	sw	r7,000C(sp)
	addiu	r7,sp,00000008

l00407E42:
	move	r6,r5
	move	r10,r0
	move	r5,r4
	addiu	r9,r0,00000008
	move	r8,r0
	li	r4,00000049
	balc	0040ADA4
	balc	0040CC30
	restore.jrc	00000020,ra,00000001
00407E5A                               00 00 00 00 00 00           ......

;; _longjmp: 00407E60
;;   Called from:
;;     00401CB2 (in sigexit)
_longjmp proc
	lw	ra,0000(r4)
	lw	sp,0004(r4)
	lwm	r16,0008(r4),00000008
	lw	r30,0028(r4)
	lw	r28,002C(r4)
	move	r4,r5
	bnezc	r4,00407E7A

l00407E78:
	addiu	r4,r4,00000001

l00407E7A:
	jrc	ra
00407E7C                                     00 80 00 C0             ....

;; _setjmp: 00407E80
;;   Called from:
;;     004012D8 (in pr_addr)
_setjmp proc
	sw	ra,0000(r4)
	sw	sp,0004(r4)
	swm	r16,0008(r4),00000008
	sw	r30,0028(r4)
	sw	r28,002C(r4)
	li	r4,00000000
	jrc	ra
00407E98                         00 80 00 C0 00 80 00 C0         ........

;; __block_all_sigs: 00407EA0
;;   Called from:
;;     00404A0A (in abort)
;;     0040B030 (in do_setxid)
;;     0040DE38 (in __synccall)
__block_all_sigs proc
	move	r7,r4
	addiu	r8,r0,00000008
	addiupc	r6,0000B03E
	move	r5,r0
	addiu	r4,r0,00000087
	bc	00404A50

;; __block_app_sigs: 00407EB4
;;   Called from:
;;     00407EE6 (in raise)
;;     0040DE2A (in __synccall)
__block_app_sigs proc
	move	r7,r4
	addiu	r8,r0,00000008
	addiupc	r6,0000B01A
	move	r5,r0
	addiu	r4,r0,00000087
	bc	00404A50

;; __restore_sigs: 00407EC8
;;   Called from:
;;     00407F04 (in raise)
;;     0040DEF0 (in __synccall)
__restore_sigs proc
	movep	r6,r7,r4,r0
	addiu	r8,r0,00000008
	li	r5,00000002
	addiu	r4,r0,00000087
	bc	00404A50
00407ED8                         00 00 00 00 00 00 00 00         ........

;; raise: 00407EE0
;;   Called from:
;;     00404A04 (in abort)
raise proc
	save	00000090,ra,00000002
	move	r16,r4
	move	r4,sp
	balc	00407EB4
	addiu	r4,r0,000000B2
	balc	00404A50
	movep	r5,r6,r4,r16
	addiu	r4,r0,00000082
	balc	00404A50
	balc	0040CC30
	move	r16,r4
	move	r4,sp
	balc	00407EC8
	move	r4,r16
	restore.jrc	00000090,ra,00000002
00407F0C                                     00 00 00 00             ....

;; setitimer: 00407F10
;;   Called from:
;;     00401E46 (in __schedule_exit)
;;     004023AE (in setup)
setitimer proc
	save	00000010,ra,00000001
	move	r7,r6
	move	r6,r5
	move	r5,r4
	li	r4,00000067
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
00407F26                   00 00 00 00 00 00 00 00 00 00       ..........

;; __get_handler_set: 00407F30
__get_handler_set proc
	li	r6,00000008
	addiupc	r5,0002AA4A
	bc	0040A130

;; __libc_sigaction: 00407F3A
;;   Called from:
;;     00408020 (in __sigaction)
;;     0040DE7C (in __synccall)
;;     0040DF7A (in __synccall)
__libc_sigaction proc
	save	00000040,ra,00000004
	movep	r18,r17,r4,r5
	move	r16,r6
	move	r6,r0
	beqc	r0,r17,00407FD2

l00407F46:
	lw	r7,0000(r17)
	bltiuc	r7,00000002,00407FA4

l00407F4C:
	addiu	r7,r18,FFFFFFFF
	addiupc	r5,0002AA2C
	srl	r6,r7,00000005
	lsa	r6,r6,r5,00000002
	li	r5,00000001
	sllv	r5,r5,r7
	sync	00000000

l00407F64:
	ll	r7,0000(r6)
	or	r7,r7,r5
	sc	r7,0000(r6)
	beqzc	r7,00407F64

l00407F70:
	sync	00000000
	addiupc	r7,0004C4B8
	lw	r7,0004(r7)
	bnezc	r7,00407FA4

l00407F7C:
	lwpc	r7,00432988
	bnezc	r7,00407FA4

l00407F84:
	li	r7,00000003
	addiu	r8,r0,00000008
	sw	r7,0004(sp)
	move	r6,sp
	move	r7,r0
	li	r5,00000001
	addiu	r4,r0,00000087
	sw	r0,0000(sp)
	balc	00404A50
	li	r7,00000001
	swpc	r7,00432988

l00407FA4:
	lw	r7,0000(r17)
	lui	r6,00004000
	addiupc	r5,00005212
	addiu	r4,sp,00000010
	sw	r7,0008(sp)
	lw	r7,0084(r17)
	or	r6,r6,r7
	andi	r7,r7,00000004
	sw	r6,000C(sp)
	addiupc	r6,00005200
	movn	r6,r5,r7

l00407FC4:
	addiu	r5,r17,00000004
	move	r7,r6
	li	r6,00000008
	sw	r7,0018(sp)
	balc	0040A130
	addiu	r6,sp,00000008

l00407FD2:
	move	r7,r0
	beqzc	r16,00407FD8

l00407FD6:
	addiu	r7,sp,0000001C

l00407FD8:
	addiu	r8,r0,00000008
	addiu	r4,r0,00000086
	move.balc	r5,r18,00404A50
	balc	0040CC30
	li	r7,FFFFFFFF
	bnezc	r4,00408006

l00407FEC:
	move	r7,r0
	beqzc	r16,00408006

l00407FF0:
	lw	r17,001C(sp)
	li	r6,00000008
	addiu	r5,sp,00000024
	addiu	r4,r16,00000004
	sw	r7,0000(sp)
	lw	r17,0020(sp)
	sw	r7,0084(r16)
	balc	0040A130
	move	r7,r0

l00408006:
	move	r4,r7
	restore.jrc	00000040,ra,00000004

;; __sigaction: 0040800A
;;   Called from:
;;     00401C92 (in set_signal)
__sigaction proc
	save	00000010,ra,00000001
	addiu	r8,r4,FFFFFFE0
	move	r7,r4
	bltiuc	r8,00000003,00408024

l00408016:
	addiu	r7,r7,FFFFFFFF
	bgeiuc	r7,0000003F,00408024

l0040801C:
	restore	00000010,ra,00000001
	bc	00407F3A

l00408024:
	balc	004049B0
	li	r7,00000016
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000001

;; sigemptyset: 00408030
;;   Called from:
;;     00402382 (in setup)
sigemptyset proc
	sw	r0,0000(sp)
	sw	r0,0004(sp)
	move	r4,r0
	jrc	ra
00408038                         00 00 00 00 00 00 00 00         ........

;; sigprocmask: 00408040
;;   Called from:
;;     0040238C (in setup)
sigprocmask proc
	save	00000010,ra,00000002
	balc	0040AE70
	move	r16,r4
	bnezc	r4,0040804E

l0040804A:
	move	r4,r16
	restore.jrc	00000010,ra,00000002

l0040804E:
	balc	004049B0
	sw	r16,0000(r4)
	li	r16,FFFFFFFF
	bc	0040804A
00408058                         00 00 00 00 00 00 00 00         ........

;; __fclose_ca: 00408060
;;   Called from:
;;     0040644A (in getnameinfo)
;;     0040669C (in getnameinfo)
;;     00406E74 (in name_from_hosts)
;;     004074A0 (in __lookup_serv)
;;     00407B9A (in __get_resolv_conf)
__fclose_ca proc
	lw	r7,000C(r4)
	jrc	r7
00408064             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __fopen_rb_ca: 00408070
;;   Called from:
;;     0040636C (in getnameinfo)
;;     004065E8 (in getnameinfo)
;;     00406E52 (in name_from_hosts)
;;     00407450 (in __lookup_serv)
;;     00407B32 (in __get_resolv_conf)
__fopen_rb_ca proc
	save	00000020,ra,00000005
	movep	r19,r16,r4,r5
	movep	r18,r17,r6,r7
	movep	r4,r5,r16,r0
	addiu	r6,r0,00000090
	balc	0040A690
	addiu	r5,r0,FFFFFF9C
	lui	r7,00000088
	move	r6,r19
	li	r4,00000038
	balc	00404A50
	balc	0040CC30
	move	r5,r4
	sw	r4,003C(sp)
	bltc	r4,r0,004080CE

l0040809C:
	li	r7,00000001
	li	r6,00000002
	li	r4,00000019
	addiu	r18,r18,00000008
	balc	00404A50
	li	r7,00000009
	sw	r7,0000(sp)
	addiupc	r7,00000050
	sw	r7,0020(sp)
	addiupc	r7,000000BA
	sw	r7,0028(sp)
	addiupc	r7,00000026
	addiu	r17,r17,FFFFFFF8
	sw	r7,000C(sp)
	li	r7,FFFFFFFF
	sw	r18,002C(sp)
	sw	r17,0030(sp)
	sw	r7,004C(r16)

l004080CA:
	move	r4,r16
	restore.jrc	00000020,ra,00000005

l004080CE:
	move	r16,r0
	bc	004080CA
004080D2       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __aio_close: 004080E0
;;   Called from:
;;     004080E6 (in __stdio_close)
;;     0040AF74 (in close)
__aio_close proc
	jrc	ra

;; __stdio_close: 004080E2
__stdio_close proc
	save	00000010,ra,00000001
	lw	r4,003C(r4)
	balc	004080E0
	move	r5,r4
	li	r4,00000039
	balc	00404A50
	restore	00000010,ra,00000001
	bc	0040CC30
004080FA                               00 00 00 00 00 00           ......

;; __stdio_read: 00408100
__stdio_read proc
	save	00000020,ra,00000004
	movep	r16,r18,r4,r5
	move	r17,r6
	li	r4,00000041
	lw	r6,0030(r16)
	lw	r5,003C(r16)
	sltu	r7,r0,r6
	sw	r6,000C(sp)
	subu	r7,r17,r7
	move	r6,sp
	sw	r7,0004(sp)
	lw	r7,002C(r16)
	sw	r18,0000(sp)
	sw	r7,0008(sp)
	li	r7,00000002
	balc	00404A50
	balc	0040CC30
	bltc	r0,r4,0040813E

l0040812C:
	andi	r7,r4,00000030
	xori	r6,r7,00000010
	lw	r7,0000(r16)
	or	r7,r7,r6
	sw	r7,0000(sp)

l0040813A:
	move	r17,r4
	bc	00408160

l0040813E:
	lw	r17,0004(sp)
	bgeuc	r6,r4,0040813A

l00408144:
	lw	r7,002C(r16)
	subu	r4,r4,r6
	lw	r6,0030(r16)
	addu	r4,r7,r4
	sw	r7,0004(sp)
	sw	r4,0008(sp)
	beqzc	r6,00408160

l00408152:
	addiu	r6,r7,00000001
	addu	r18,r18,r17
	sw	r6,0004(sp)
	lbu	r7,0000(r7)
	sb	r7,-0001(r18)

l00408160:
	move	r4,r17
	restore.jrc	00000020,ra,00000004
00408164             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __stdio_seek: 00408170
__stdio_seek proc
	save	00000020,ra,00000001
	movep	r5,r9,r7,r8
	move	r7,r6
	move	r6,r5
	lw	r5,003C(r4)
	addiu	r8,sp,00000008
	li	r4,0000003E
	balc	00404A50
	balc	0040CC30
	bgec	r4,r0,00408194

l0040818C:
	li	r6,FFFFFFFF
	li	r7,FFFFFFFF
	swm	r6,0008(sp),00000002

l00408194:
	lwm	r4,0008(sp),00000002
	restore.jrc	00000020,ra,00000001
0040819A                               00 00 00 00 00 00           ......

;; fflush_unlocked: 004081A0
;;   Called from:
;;     00401B98 (in fn00401B98)
;;     00401F26 (in pinger)
;;     0040275A (in finish)
;;     004033F2 (in fn004033F2)
;;     004081B6 (in fflush_unlocked)
;;     004081CE (in fflush_unlocked)
fflush_unlocked proc
	save	00000010,ra,00000003
	move	r16,r4
	bnezc	r4,004081DE

l004081A6:
	lwpc	r7,00430300
	move	r17,r0
	beqzc	r7,004081BA

l004081B0:
	lwpc	r4,00430300
	balc	004081A0
	move	r17,r4

l004081BA:
	balc	00408610
	lw	r16,0000(r4)
	bc	004081C4

l004081C2:
	lw	r16,0038(r16)

l004081C4:
	beqzc	r16,004081D6

l004081C6:
	lw	r7,0014(r16)
	lw	r6,001C(r16)
	bgeuc	r6,r7,004081C2

l004081CE:
	move.balc	r4,r16,004081A0
	or	r17,r17,r4
	bc	004081C2

l004081D6:
	balc	00408620

l004081DA:
	move	r4,r17
	restore.jrc	00000010,ra,00000003

l004081DE:
	lw	r7,004C(r4)
	move	r17,r0
	bltc	r7,r0,004081EE

l004081E8:
	balc	0040D1D0
	move	r17,r4

l004081EE:
	lw	r7,0014(r16)
	lw	r6,001C(r16)
	bgeuc	r6,r7,0040820C

l004081F6:
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r0,r0
	jalrc	ra,r7
	lw	r7,0014(r16)
	bnezc	r7,0040820C

l00408202:
	beqzc	r17,00408208

l00408204:
	move.balc	r4,r16,0040D210

l00408208:
	li	r17,FFFFFFFF
	bc	004081DA

l0040820C:
	lwm	r6,0004(r16),00000002
	bgeuc	r6,r7,00408224

l00408214:
	subu	r6,r6,r7
	lw	r5,0028(r16)
	addiu	r8,r0,00000001
	sra	r7,r6,0000001F
	move	r4,r16
	jalrc	ra,r5

l00408224:
	sw	r0,0004(sp)
	sw	r0,0008(sp)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,001C(sp)
	beqzc	r17,004081DA

l00408230:
	move	r17,r0
	move.balc	r4,r16,0040D210
	bc	004081DA
00408238                         00 00 00 00 00 00 00 00         ........

;; fgets_unlocked: 00408240
;;   Called from:
;;     0040639C (in getnameinfo)
;;     0040663E (in getnameinfo)
;;     00406E66 (in name_from_hosts)
;;     00407496 (in __lookup_serv)
;;     00407B94 (in __get_resolv_conf)
fgets_unlocked proc
	save	00000020,ra,00000008
	lw	r7,004C(r6)
	move	r16,r6
	movep	r19,r18,r4,r5
	move	r21,r0
	bltc	r7,r0,00408256

l00408250:
	move.balc	r4,r6,0040D1D0
	move	r21,r4

l00408256:
	addiu	r20,r18,FFFFFFFF
	move	r17,r19
	bgeic	r18,00000002,004082BC

l00408260:
	lb	r7,004A(r16)
	addiu	r6,r7,FFFFFFFF
	or	r7,r7,r6
	sb	r7,004A(r16)
	beqc	r0,r21,00408276

l00408272:
	move.balc	r4,r16,0040D210

l00408276:
	move	r17,r0
	bnec	r0,r20,00408280

l0040827C:
	move	r17,r19
	sb	r0,0000(r19)

l00408280:
	move	r4,r17
	restore.jrc	00000020,ra,00000008

l00408284:
	lw	r18,0008(r16)
	subu	r18,r18,r7
	bc	004082D6

l0040828A:
	move.balc	r4,r16,0040D3E0
	bgec	r4,r0,004082AC

l00408292:
	beqc	r17,r19,0040830C

l00408296:
	lw	r7,0000(r16)
	bbeqzc	r7,00000004,0040830C

l0040829C:
	beqzc	r19,004082A0

l0040829E:
	sb	r0,0000(r17)

l004082A0:
	move	r17,r19
	beqc	r0,r21,00408280

l004082A6:
	move.balc	r4,r16,0040D210
	bc	00408280

l004082AC:
	addiu	r7,r17,00000001
	andi	r4,r4,000000FF
	sb	r4,0000(r17)
	addiu	r20,r20,FFFFFFFF
	move	r17,r7
	beqic	r4,0000000A,0040829C

l004082BC:
	beqc	r0,r20,0040829C

l004082C0:
	lw	r4,0004(r16)
	li	r5,0000000A
	lw	r6,0008(r16)
	subu	r6,r6,r4
	balc	0040A050
	lw	r7,0004(r16)
	move	r22,r4
	beqzc	r4,00408284

l004082D2:
	subu	r18,r4,r7
	addiu	r18,r18,00000001

l004082D6:
	sltu	r7,r20,r18
	lw	r5,0004(r16)
	movn	r18,r20,r7

l004082E0:
	move	r4,r17
	move	r6,r18
	addu	r17,r17,r18
	balc	0040A130
	lw	r7,0004(r16)
	subu	r20,r20,r18
	addu	r7,r7,r18
	sw	r7,0004(sp)
	bnec	r0,r22,0040829C

l004082F8:
	beqc	r0,r20,0040829C

l004082FC:
	lw	r6,0008(r16)
	bgeuc	r7,r6,0040828A

l00408302:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)
	bc	004082AC

l0040830C:
	move	r19,r0
	bc	004082A0

;; flockfile: 00408310
;;   Called from:
;;     00405952 (in __getopt_msg)
flockfile proc
	save	00000010,ra,00000002
	move	r16,r4

l00408314:
	move.balc	r4,r16,0040845C
	beqzc	r4,00408330

l0040831A:
	lw	r6,004C(r16)
	beqzc	r6,00408314

l00408320:
	li	r7,00000001
	addiu	r5,r16,00000050
	addiu	r4,r16,0000004C
	balc	0040ADB0
	bc	00408314

l00408330:
	restore.jrc	00000010,ra,00000002
00408332       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; fprintf: 00408340
;;   Called from:
;;     00400150 (in set_socket_option.isra.0.part.1)
;;     004002B6 (in main)
;;     004007F6 (in main)
;;     00400B5E (in fn00400B5E)
;;     00401862 (in ping4_receive_error_msg)
;;     00401986 (in ping4_parse_reply)
;;     004023F2 (in fn004023F2)
;;     00402A08 (in status)
;;     00402A86 (in status)
;;     004032FE (in fn004032FE)
;;     00403622 (in ping6_run)
;;     0040381E (in ping6_run)
fprintf proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000001
	swm	r6,0028(sp),00000002
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	move	r6,sp
	sw	r7,0004(sp)
	swm	r8,0030(sp),00000002
	sw	r7,0008(sp)
	li	r7,00000018
	sb	r7,000C(sp)
	li	r7,00000040
	swm	r10,0038(sp),00000002
	sb	r7,000D(sp)
	balc	004099EE
	restore	00000020,ra,00000001
	addiu	sp,sp,00000020
	jrc	ra
00408376                   00 00 00 00 00 00 00 00 00 00       ..........

;; fputc: 00408380
;;   Called from:
;;     00402A92 (in status)
;;     0040866A (in perror)
;;     00408670 (in perror)
;;     00408684 (in perror)
;;     004086AE (in perror)
;;     00408776 (in putchar)
fputc proc
	save	00000020,ra,00000005
	movep	r18,r16,r4,r5
	andi	r19,r18,000000FF
	andi	r17,r18,000000FF
	lw	r7,004C(r16)
	bgec	r7,r0,004083A2

l00408390:
	lb	r7,004B(r16)
	bnec	r7,r17,004083C6

l00408398:
	movep	r4,r5,r16,r18
	restore	00000020,ra,00000005
	bc	0040D250

l004083A2:
	move.balc	r4,r16,0040D1D0
	beqzc	r4,00408390

l004083A8:
	lb	r7,004B(r16)
	beqc	r17,r7,004083DA

l004083B0:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,004083DA

l004083B8:
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)

l004083C0:
	move.balc	r4,r16,0040D210
	bc	004083D6

l004083C6:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,00408398

l004083CE:
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)

l004083D6:
	move	r4,r17
	restore.jrc	00000020,ra,00000005

l004083DA:
	movep	r4,r5,r16,r18
	balc	0040D250
	move	r17,r4
	bc	004083C0
004083E4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; fputs_unlocked: 004083F0
;;   Called from:
;;     00400B5A (in fn00400B5A)
;;     00400F96 (in ping4_run)
;;     00402126 (in fn00402126)
;;     00403098 (in fn00403098)
;;     004035EE (in ping6_run)
;;     004036BE (in ping6_run)
;;     004039E8 (in ping6_run)
;;     00403A3E (in ping6_run)
;;     00405958 (in __getopt_msg)
;;     0040879C (in puts)
fputs_unlocked proc
	save	00000010,ra,00000004
	movep	r17,r18,r4,r5
	balc	0040A890
	li	r5,00000001
	move	r16,r4
	movep	r6,r7,r16,r18
	move.balc	r4,r17,0040857C
	xor	r4,r4,r16
	sltu	r4,r0,r4
	subu	r4,r0,r4
	restore.jrc	00000010,ra,00000004
0040840E                                           00 00               ..

;; __do_orphaned_stdio_locks: 00408410
__do_orphaned_stdio_locks proc
	rdhwr	r3,0000001D,00000000
	lw	r7,-000C(r3)
	bc	0040842E

l0040841A:
	sync	00000000
	lui	r6,00040000
	sw	r6,004C(r7)
	sync	00000000
	lw	r7,0084(r7)

l0040842E:
	bnezc	r7,0040841A

l00408430:
	jrc	ra

;; __unlist_locked_file: 00408432
;;   Called from:
;;     004084F4 (in funlockfile)
__unlist_locked_file proc
	addiu	r4,r4,00000044
	lw	r7,0000(r4)
	beqzc	r7,0040845A

l0040843A:
	lwm	r6,003C(r4),00000002
	beqzc	r7,00408444

l00408440:
	sw	r6,0080(r7)

l00408444:
	lw	r6,003C(r4)
	beqzc	r6,0040844E

l00408448:
	sw	r7,0084(r6)
	jrc	ra

l0040844E:
	rdhwr	r3,0000001D,00000000
	lw	r7,0040(r4)
	sw	r7,-000C(r3)

l0040845A:
	jrc	ra

;; ftrylockfile: 0040845C
;;   Called from:
;;     00408314 (in flockfile)
ftrylockfile proc
	move	r7,r4
	rdhwr	r3,0000001D,00000000
	lw	r6,004C(r4)
	lw	r8,-0094(r3)
	bnec	r8,r6,00408488

l0040846E:
	lw	r6,0044(r4)
	li	r5,7FFFFFFF
	bnec	r5,r6,0040847E

l0040847A:
	li	r4,FFFFFFFF
	jrc	ra

l0040847E:
	addiu	r6,r6,00000001
	move	r4,r0
	sw	r6,0044(r7)
	jrc	ra

l00408488:
	lw	r6,004C(r4)
	bgec	r6,r0,00408494

l00408490:
	sw	r0,004C(r4)

l00408494:
	lw	r6,004C(r7)
	bnezc	r6,0040847A

l0040849A:
	addiu	r5,r7,0000004C
	sync	00000000

l004084A2:
	ll	r4,0000(r5)
	bnezc	r4,004084B0

l004084A8:
	move	r6,r8
	sc	r6,0000(r5)
	beqzc	r6,004084A2

l004084B0:
	sync	00000000
	bnezc	r4,0040847A

l004084B6:
	li	r6,00000001
	sw	r0,0080(r7)
	sw	r6,0044(r7)
	lw	r6,-000C(r3)
	sw	r6,0084(r7)
	beqzc	r6,004084CE

l004084CA:
	sw	r7,0080(r6)

l004084CE:
	sw	r7,-000C(r3)
	jrc	ra
004084D4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; funlockfile: 004084E0
;;   Called from:
;;     00405986 (in __getopt_msg)
funlockfile proc
	save	00000010,ra,00000002
	lw	r7,0044(r4)
	move	r16,r4
	beqic	r7,00000001,004084F4

l004084EC:
	addiu	r7,r7,FFFFFFFF
	sw	r7,0044(r4)
	restore.jrc	00000010,ra,00000002

l004084F4:
	balc	00408432
	move	r4,r16
	sw	r0,0044(r16)
	restore	00000010,ra,00000002
	bc	0040D210
00408506                   00 00 00 00 00 00 00 00 00 00       ..........

;; __fwritex: 00408510
;;   Called from:
;;     0040859C (in fwrite_unlocked)
;;     00408970 (in out)
__fwritex proc
	save	00000020,ra,00000005
	lw	r7,0010(r6)
	move	r16,r6
	movep	r19,r18,r4,r5
	beqzc	r7,00408530

l0040851A:
	lw	r7,0010(r16)
	lw	r6,0014(r16)
	subu	r7,r7,r6
	bgeuc	r7,r18,0040853A

l00408524:
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r19,r18
	restore	00000020,ra,00000005
	jrc	r7

l00408530:
	move.balc	r4,r6,0040D3A0
	beqzc	r4,0040851A

l00408536:
	move	r4,r0
	restore.jrc	00000020,ra,00000005

l0040853A:
	lb	r7,004B(r16)
	move	r17,r18
	bgec	r7,r0,0040854A

l00408544:
	move	r17,r0
	bc	0040856A

l00408548:
	move	r17,r7

l0040854A:
	beqzc	r17,0040856A

l0040854C:
	addiu	r7,r17,FFFFFFFF
	lbux	r6,r7(r19)
	bneiuc	r6,0000000A,00408548

l00408558:
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r19,r17
	jalrc	ra,r7
	bgeuc	r4,r17,00408566

l00408564:
	restore.jrc	00000020,ra,00000005

l00408566:
	addu	r19,r19,r17
	subu	r18,r18,r17

l0040856A:
	lw	r4,0014(r16)
	movep	r5,r6,r19,r18
	balc	0040A130
	lw	r7,0014(r16)
	addu	r4,r18,r17
	addu	r7,r7,r18
	sw	r7,0014(sp)
	restore.jrc	00000020,ra,00000005

;; fwrite_unlocked: 0040857C
;;   Called from:
;;     0040596A (in __getopt_msg)
;;     00405974 (in __getopt_msg)
;;     004083FE (in fputs_unlocked)
;;     00408664 (in perror)
;;     0040867E (in perror)
;;     004086A2 (in perror)
fwrite_unlocked proc
	save	00000020,ra,00000007
	movep	r16,r17,r6,r7
	movep	r19,r18,r4,r5
	lw	r7,004C(r17)
	move	r21,r0
	mul	r20,r18,r16
	movz	r16,r0,r18

l00408590:
	bltc	r7,r0,0040859A

l00408594:
	move.balc	r4,r17,0040D1D0
	move	r21,r4

l0040859A:
	movep	r5,r6,r20,r17
	move.balc	r4,r19,00408510
	move	r19,r4
	beqc	r0,r21,004085AA

l004085A6:
	move.balc	r4,r17,0040D210

l004085AA:
	beqc	r20,r19,004085B2

l004085AE:
	divu	r16,r19,r18

l004085B2:
	move	r4,r16
	restore.jrc	00000020,ra,00000007
004085B6                   00 00 00 00 00 00 00 00 00 00       ..........

;; _IO_getc: 004085C0
;;   Called from:
;;     00407B7E (in __get_resolv_conf)
_IO_getc proc
	save	00000010,ra,00000003
	lw	r7,004C(r4)
	move	r16,r4
	bgec	r7,r0,004085DE

l004085CC:
	lw	r7,0004(r16)
	lw	r6,0008(r16)
	bltuc	r7,r6,004085FA

l004085D4:
	move	r4,r16
	restore	00000010,ra,00000003
	bc	0040D3E0

l004085DE:
	balc	0040D1D0
	beqzc	r4,004085CC

l004085E4:
	lw	r7,0004(r16)
	lw	r6,0008(r16)
	bgeuc	r7,r6,00408606

l004085EC:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r17,0000(r7)

l004085F4:
	move.balc	r4,r16,0040D210
	bc	00408602

l004085FA:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r17,0000(r7)

l00408602:
	move	r4,r17
	restore.jrc	00000010,ra,00000003

l00408606:
	move.balc	r4,r16,0040D3E0
	move	r17,r4
	bc	004085F4
0040860E                                           00 00               ..

;; __ofl_lock: 00408610
;;   Called from:
;;     004081BA (in fflush_unlocked)
;;     0040E722 (in __stdio_exit_needed)
__ofl_lock proc
	save	00000010,ra,00000001
	addiupc	r4,0002A38A
	balc	0040AD30
	addiupc	r4,0002A38A
	restore.jrc	00000010,ra,00000001

;; __ofl_unlock: 00408620
;;   Called from:
;;     004081D6 (in fflush_unlocked)
__ofl_unlock proc
	addiupc	r4,0002A37C
	bc	0040AD60
00408628                         00 00 00 00 00 00 00 00         ........

;; perror: 00408630
;;   Called from:
;;     00400B44 (in ping4_install_filter)
;;     00400C92 (in ping4_run)
;;     00400D56 (in ping4_run)
;;     00401284 (in fn00401284)
;;     004018D2 (in ping4_receive_error_msg)
;;     00401E7A (in fn00401E7A)
;;     00402C00 (in main_loop)
;;     004032F8 (in ping6_install_filter)
;;     00403790 (in ping6_run)
perror proc
	save	00000020,ra,00000005
	lwpc	r16,00412EF0
	move	r17,r4
	balc	004049B0
	lw	r4,0000(r4)
	balc	004049EA
	lw	r7,004C(r16)
	move	r18,r4
	bltc	r7,r0,00408694

l0040864E:
	move.balc	r4,r16,0040D1D0
	move	r19,r4
	beqzc	r17,00408674

l00408656:
	lbu	r7,0000(r17)
	beqzc	r7,00408674

l0040865A:
	move.balc	r4,r17,0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r17,0040857C
	li	r4,0000003A
	move.balc	r5,r16,00408380
	li	r4,00000020
	move.balc	r5,r16,00408380

l00408674:
	move.balc	r4,r18,0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r18,0040857C
	li	r4,0000000A
	move.balc	r5,r16,00408380
	beqzc	r19,004086B2

l0040868A:
	move	r4,r16
	restore	00000020,ra,00000005
	bc	0040D210

l00408694:
	move	r19,r0
	bnezc	r17,00408656

l00408698:
	balc	0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r18,0040857C
	move	r5,r16
	li	r4,0000000A
	restore	00000020,ra,00000005
	bc	00408380

l004086B2:
	restore.jrc	00000020,ra,00000005
004086B4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; printf: 004086C0
;;   Called from:
;;     00400A00 (in pr_echo_reply)
;;     004013C6 (in pr_options)
;;     00401594 (in fn00401594)
;;     004015EC (in pr_iph)
;;     004015F6 (in pr_iph)
;;     00401B94 (in fn00401B94)
;;     00401F1C (in pinger)
;;     0040212A (in fn0040212A)
;;     00402616 (in gather_statistics)
;;     0040262A (in gather_statistics)
;;     00402664 (in gather_statistics)
;;     004026B8 (in gather_statistics)
;;     00402916 (in finish)
;;     0040299A (in finish)
;;     004029C6 (in fn004029C6)
;;     00402E76 (in pr_icmph)
;;     00402EAE (in pr_icmph)
;;     00402EDC (in pr_echo_reply)
;;     0040309C (in fn0040309C)
;;     00403C9E (in fn00403C9E)
printf proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000001
	swm	r7,002C(sp),00000002
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	swm	r5,0024(sp),00000002
	sw	r7,0004(sp)
	move	r6,sp
	sw	r7,0008(sp)
	li	r7,0000001C
	move	r5,r4
	sb	r7,000C(sp)
	lwpc	r4,00412EF4
	li	r7,00000040
	swm	r9,0034(sp),00000002
	sw	r11,003C(sp)
	sb	r7,000D(sp)
	balc	004099EE
	restore	00000020,ra,00000001
	addiu	sp,sp,00000020
	jrc	ra

;; _IO_putc: 00408700
;;   Called from:
;;     0040597C (in __getopt_msg)
_IO_putc proc
	save	00000020,ra,00000005
	movep	r18,r16,r4,r5
	andi	r19,r18,000000FF
	andi	r17,r18,000000FF
	lw	r7,004C(r16)
	bgec	r7,r0,00408722

l00408710:
	lb	r7,004B(r16)
	bnec	r7,r17,00408746

l00408718:
	movep	r4,r5,r16,r18
	restore	00000020,ra,00000005
	bc	0040D250

l00408722:
	move.balc	r4,r16,0040D1D0
	beqzc	r4,00408710

l00408728:
	lb	r7,004B(r16)
	beqc	r17,r7,0040875A

l00408730:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,0040875A

l00408738:
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)

l00408740:
	move.balc	r4,r16,0040D210
	bc	00408756

l00408746:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,00408718

l0040874E:
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)

l00408756:
	move	r4,r17
	restore.jrc	00000020,ra,00000005

l0040875A:
	movep	r4,r5,r16,r18
	balc	0040D250
	move	r17,r4
	bc	00408740
00408764             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; putchar: 00408770
;;   Called from:
;;     00401766 (in fn00401766)
;;     00401B86 (in ping4_parse_reply)
;;     00401D8C (in fill)
;;     00402A96 (in fn00402A96)
;;     004033EE (in fn004033EE)
putchar proc
	lwpc	r5,00412EF4
	bc	00408380
0040877A                               00 00 00 00 00 00           ......

;; puts: 00408780
;;   Called from:
;;     00401762 (in fn00401762)
;;     00402640 (in gather_statistics)
;;     00402D36 (in niquery_set_qtype)
;;     004033C2 (in niquery_option_subject_addr_handler)
puts proc
	save	00000010,ra,00000004
	lwpc	r16,00412EF4
	move	r17,r4
	lw	r7,004C(r16)
	move	r18,r0
	bltc	r7,r0,0040879A

l00408794:
	move.balc	r4,r16,0040D1D0
	move	r18,r4

l0040879A:
	movep	r4,r5,r17,r16
	balc	004083F0
	li	r17,00000001
	bltc	r4,r0,004087C0

l004087A6:
	lb	r7,004B(r16)
	beqic	r7,0000000A,004087CE

l004087AE:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,004087CE

l004087B6:
	addu	r6,r7,r17
	move	r17,r0
	sw	r6,0014(sp)
	li	r6,0000000A
	sb	r6,0000(r7)

l004087C0:
	subu	r17,r0,r17
	beqzc	r18,004087CA

l004087C6:
	move.balc	r4,r16,0040D210

l004087CA:
	move	r4,r17
	restore.jrc	00000010,ra,00000004

l004087CE:
	li	r5,0000000A
	move.balc	r4,r16,0040D250
	srl	r17,r4,0000001F
	bc	004087C0
004087DA                               00 00 00 00 00 00           ......

;; setbuf: 004087E0
setbuf proc
	li	r6,00000002
	addiu	r7,r0,00000400
	movn	r6,r0,r5

l004087EA:
	bc	004087F0
004087EE                                           00 00               ..

;; setvbuf: 004087F0
;;   Called from:
;;     004087EA (in setbuf)
setvbuf proc
	li	r7,FFFFFFFF
	sb	r7,004B(r4)
	bneiuc	r6,00000002,00408808

l004087FA:
	sw	r0,0030(sp)

l004087FC:
	lw	r7,0000(r4)
	ori	r7,r7,00000040
	sw	r7,0000(sp)
	move	r4,r0
	jrc	ra

l00408808:
	bneiuc	r6,00000001,004087FC

l0040880C:
	li	r7,0000000A
	sb	r7,004B(r4)
	bc	004087FC
00408814             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; snprintf: 00408820
;;   Called from:
;;     0040131C (in pr_addr)
;;     00401354 (in pr_addr)
;;     0040684A (in inet_ntoa)
;;     0040688E (in inet_ntop)
;;     00406924 (in inet_ntop)
;;     0040695C (in inet_ntop)
snprintf proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000001
	swm	r7,002C(sp),00000002
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	swm	r9,0034(sp),00000002
	sw	r7,0004(sp)
	sw	r7,0008(sp)
	li	r7,00000014
	sb	r7,000C(sp)
	li	r7,00000040
	sb	r7,000D(sp)
	move	r7,sp
	sw	r11,003C(sp)
	balc	00409B10
	restore	00000020,ra,00000001
	addiu	sp,sp,00000020
	jrc	ra
00408854             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; sprintf: 00408860
;;   Called from:
;;     00406346 (in getnameinfo)
;;     0040654C (in getnameinfo)
sprintf proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000001
	swm	r6,0028(sp),00000002
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	move	r6,sp
	sw	r7,0004(sp)
	swm	r8,0030(sp),00000002
	sw	r7,0008(sp)
	li	r7,00000018
	sb	r7,000C(sp)
	li	r7,00000040
	swm	r10,0038(sp),00000002
	sb	r7,000D(sp)
	balc	00409B70
	restore	00000020,ra,00000001
	addiu	sp,sp,00000020
	jrc	ra
00408896                   00 00 00 00 00 00 00 00 00 00       ..........

;; __isoc99_sscanf: 004088A0
;;   Called from:
;;     00401D66 (in fill)
__isoc99_sscanf proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000001
	swm	r6,0028(sp),00000002
	addiu	r7,sp,00000040
	sw	r7,0000(sp)
	move	r6,sp
	sw	r7,0004(sp)
	swm	r8,0030(sp),00000002
	sw	r7,0008(sp)
	li	r7,00000018
	sb	r7,000C(sp)
	li	r7,00000040
	swm	r10,0038(sp),00000002
	sb	r7,000D(sp)
	balc	00409B94
	restore	00000020,ra,00000001
	addiu	sp,sp,00000020
	jrc	ra
004088D6                   00 00 00 00 00 00 00 00 00 00       ..........

;; fmt_u: 004088E0
;;   Called from:
;;     004091F8 (in fn00409170)
;;     004099EA (in fn004099EA)
fmt_u proc
	save	00000010,ra,00000004
	movep	r16,r18,r4,r5
	move	r17,r6
	bc	00408906

l004088E8:
	move	r7,r0
	li	r6,0000000A
	movep	r4,r5,r16,r18
	balc	0040ED50
	addiu	r17,r17,FFFFFFFF
	addiu	r4,r4,00000030
	sb	r4,0000(r17)
	li	r6,0000000A
	movep	r4,r5,r16,r18
	move	r7,r0
	balc	0040EAB0
	movep	r16,r18,r4,r5

l00408906:
	bnezc	r18,004088E8

l00408908:
	bc	0040891E

l0040890A:
	li	r6,0000000A
	addiu	r17,r17,FFFFFFFF
	modu	r7,r16,r6
	addiu	r7,r7,00000030
	sb	r7,0000(r17)
	move	r7,r16
	divu	r16,r7,r6

l0040891E:
	bnezc	r16,0040890A

l00408920:
	move	r4,r17
	restore.jrc	00000010,ra,00000004

;; getint: 00408924
;;   Called from:
;;     00408D84 (in fn00408B86)
;;     00408DD6 (in fn00408B86)
getint proc
	move	r7,r0
	bc	0040892E

l00408928:
	li	r7,FFFFFFFF

l0040892A:
	addiu	r8,r8,00000001
	sw	r8,0000(r4)

l0040892E:
	lw	r8,0000(r4)
	lbu	r6,0000(r8)
	addiu	r6,r6,FFFFFFD0
	bgeiuc	r6,0000000A,0040895E

l0040893C:
	li	r5,0CCCCCCC
	bltuc	r5,r7,00408928

l00408946:
	addiu	r5,r0,FFFFFFF6
	mul	r5,r5,r7
	addiu	r5,r5,7FFFFFFF
	bltc	r5,r6,00408928

l00408956:
	li	r5,0000000A
	mul	r7,r7,r5
	addu	r7,r6,r7
	bc	0040892A

l0040895E:
	move	r4,r7
	jrc	ra

;; out: 00408962
;;   Called from:
;;     00408B5A (in pad)
;;     00408B6A (in pad)
;;     00408C08 (in fn00408B86)
;;     00408FEA (in fn00408B86)
;;     0040900C (in fn00408B86)
;;     004092E2 (in fn00409170)
;;     004092FA (in fn00409170)
;;     00409310 (in fn00409170)
;;     00409802 (in fn00409170)
;;     0040983C (in fn00409170)
;;     004098E0 (in fn00409170)
;;     00409908 (in fn00409170)
;;     00409940 (in fn00409170)
;;     0040996C (in fn00409170)
;;     0040997C (in fn00409170)
;;     004099A2 (in fn00409170)
out proc
	move	r7,r4
	move	r4,r5
	move	r5,r6
	lw	r6,0000(r7)
	bbnezc	r6,00000005,00408974

l0040896E:
	move	r6,r7
	bc	00408510

l00408974:
	jrc	ra

;; pop_arg: 00408976
;;   Called from:
;;     00408E0E (in fn00408B86)
;;     004099BE (in fn00408B86)
pop_arg proc
	bgeiuc	r5,0000001D,00408A76

l0040897A:
	addiu	r5,r5,FFFFFFF7
	bgeiuc	r5,00000012,00408A76

l00408982:
	addiupc	r7,0000A572
	lwxs	r5,r5(r7)
	lb	r7,000C(r6)
	jrc	r5
0040898E                                           E0 88               ..
00408990 1A 80 A7 80 04 80 A5 20 08 00 A6 84 0C 10 05 A8 ....... ........
004089A0 0A 80 61 17 EF B3 F0 17 C0 97 E0 DB E0 17 F1 92 ..a.............
004089B0 E0 96 F3 1B E0 88 1E 80 A7 80 04 80 A5 20 08 00 ............. ..
004089C0 A6 84 0C 10 05 A8 0E 80 61 17 EF B3 F0 7F C0 97 ........a.......
004089D0 E7 80 9F C0 9E 18 E0 17 F1 92 E0 96 EF 1B E0 88 ................
004089E0 1C 80 A7 80 04 80 A5 20 08 00 A6 84 0C 10 05 A8 ....... ........
004089F0 0C 80 61 17 EF B3 F8 7F C0 97 41 94 E0 DB E0 17 ..a.......A.....
00408A00 F1 92 E0 96 F1 1B E0 88 18 80 A7 80 04 80 A5 20 ............... 
00408A10 08 00 A6 84 0C 10 05 A8 08 80 61 17 EF B3 F0 5F ..........a...._
00408A20 AD 1B E0 17 F1 92 E0 96 F5 1B E0 88 18 80 A7 80 ................
00408A30 04 80 A5 20 08 00 A6 84 0C 10 05 A8 08 80 61 17 ... ..........a.
00408A40 EF B3 F8 5F B3 1B E0 17 F1 92 E0 96 F5 1B E0 88 ..._............
00408A50 26 80 E0 80 80 E0 A7 80 08 80 07 11 A5 20 08 00 &............ ..
00408A60 A6 84 0C 10 05 A8 10 80 E1 17 07 21 D0 39 C7 A4 ...........!.9..
00408A70 00 24 40 97 C1 97                               .$@...          

l00408A76:
	jrc	ra
00408A78                         E0 17 EF 90 E0 80 80 E0         ........
00408A80 F2 92 E0 96 E9 1B E0 88 18 80 A7 80 04 80 A5 20 ............... 
00408A90 08 00 A6 84 0C 10 05 A8 08 80 61 17 EF B3 F0 17 ..........a.....
00408AA0 2D 1B E0 17 F1 92 E0 96 F5 1B E0 88 18 80 A7 80 -...............
00408AB0 04 80 A5 20 08 00 A6 84 0C 10 05 A8 08 80 61 17 ... ..........a.
00408AC0 EF B3 F0 17 33 1B E0 17 F1 92 E0 96 F5 1B E0 88 ....3...........
00408AD0 42 80 E0 80 80 E0 A7 80 08 80 07 11 A5 20 08 00 B............ ..
00408AE0 A6 84 0C 10 05 A8 2C 80 E1 17 07 21 D0 39 C7 A4 ......,....!.9..
00408AF0 00 24 40 97 7F 1B E0 88 1A 80 E0 80 80 E0 A7 80 .$@.............
00408B00 08 80 A5 20 08 00 A6 84 0C 10 05 A8 06 80 61 17 ... ..........a.
00408B10 EF B3 DB 1B E0 17 EF 90 E0 80 80 E0 F2 92 E0 96 ................
00408B20 CD 1B                                           ..              

;; pad: 00408B22
;;   Called from:
;;     00408FE4 (in fn00408B86)
;;     00408FFC (in fn00408B86)
;;     00409006 (in fn00408B86)
;;     0040901C (in fn00408B86)
;;     004090D0 (in fn00409170)
;;     004092DA (in fn00409170)
;;     004092F4 (in fn00409170)
;;     00409308 (in fn00409170)
;;     00409322 (in fn00409170)
;;     004097FA (in fn00409170)
;;     00409814 (in fn00409170)
;;     00409850 (in fn00409170)
;;     00409934 (in fn00409170)
pad proc
	save	00000110,ra,00000004
	move	r17,r4
	lui	r4,00000012
	and	r8,r8,r4
	bnec	r0,r8,00408B6E

l00408B34:
	bgec	r7,r6,00408B6E

l00408B38:
	subu	r16,r6,r7
	addiu	r7,r0,00000100
	sltiu	r6,r16,00000101
	move	r4,sp
	movn	r7,r16,r6

l00408B48:
	move	r18,r16
	move	r6,r7
	balc	0040A690
	bc	00408B5E

l00408B52:
	addiu	r6,r6,00000001
	move	r5,sp
	addiu	r16,r16,FFFFFF00
	move.balc	r4,r17,00408962

l00408B5E:
	addiu	r6,r0,000000FF
	bltuc	r6,r16,00408B52

l00408B66:
	andi	r6,r18,000000FF
	move	r5,sp
	move.balc	r4,r17,00408962

l00408B6E:
	restore.jrc	00000110,ra,00000004

;; printf_core: 00408B72
;;   Called from:
;;     00409A20 (in vfprintf)
;;     00409A6E (in vfprintf)
printf_core proc
	save	000002E0,r30,0000000A
	move	r22,r0
	move	r16,r4
	sw	r0,0004(sp)
	sw	r6,000C(sp)
	sw	r0,001C(sp)
	sw	r8,0020(sp)
	sw	r7,0028(sp)
	sw	r5,004C(sp)

;; fn00408B86: 00408B86
;;   Called from:
;;     00408B84 (in printf_core)
;;     00408D8E (in fn00408D8E)
;;     00409332 (in fn00409170)
fn00408B86 proc
	lw	r17,0004(sp)
	li	r7,7FFFFFFF
	subu	r7,r7,r6
	bltc	r7,r22,00408D8E

l00408B94:
	lw	r5,004C(sp)
	addu	r7,r6,r22
	sw	r7,0004(sp)
	lbu	r7,0000(r21)
	bnezc	r7,00408BCC

l00408BA2:
	bnec	r0,r16,00408E04

l00408BA6:
	lw	r17,001C(sp)
	li	r16,00000001
	beqc	r0,r7,00408E02

l00408BAE:
	lw	r17,0020(sp)
	lwxs	r5,r16(r7)
	bnec	r0,r5,004099B4

l00408BB6:
	lw	r17,0020(sp)
	lwxs	r7,r16(r7)
	bnec	r0,r7,00408D3A

l00408BBE:
	addiu	r16,r16,00000001
	bneiuc	r16,0000000A,00408BB6

l00408BC4:
	bc	004099CA

l00408BC8:
	addiu	r22,r22,00000001
	sw	r22,004C(sp)

l00408BCC:
	lw	r5,004C(sp)
	lbu	r7,0000(r22)
	beqzc	r7,00408BEE

l00408BD4:
	bneiuc	r7,00000025,00408BC8

l00408BD8:
	bc	00408BE0

l00408BDA:
	addiu	r7,r7,00000002
	addiu	r22,r22,00000001
	sw	r7,004C(sp)

l00408BE0:
	lw	r17,004C(sp)
	lbu	r6,0000(r7)
	bneiuc	r6,00000025,00408BEE

l00408BE8:
	lbu	r6,0001(r7)
	beqic	r6,00000025,00408BDA

l00408BEE:
	lw	r17,0004(sp)
	li	r20,7FFFFFFF
	subu	r22,r22,r21
	subu	r7,r20,r7
	sw	r7,0008(sp)
	bltc	r7,r22,00408D8E

l00408C04:
	beqzc	r16,00408C0C

l00408C06:
	movep	r5,r6,r21,r22
	move.balc	r4,r16,00408962

l00408C0C:
	bnec	r0,r22,00408B86

l00408C10:
	lw	r17,004C(sp)
	lbu	r19,0001(r7)
	addiu	r19,r19,FFFFFFD0
	bgeiuc	r19,0000000A,00408D20

l00408C1C:
	lbu	r6,0002(r7)
	bneiuc	r6,00000024,00408D20

l00408C22:
	addiu	r7,r7,00000003
	sw	r7,004C(sp)
	li	r7,00000001
	sw	r7,001C(sp)

l00408C2A:
	move	r17,r0

l00408C2C:
	lw	r17,004C(sp)
	lbu	r4,0000(r7)
	addiu	r5,r4,FFFFFFE0
	bgeiuc	r5,00000020,00408C46

l00408C38:
	li	r6,00012889
	srlv	r6,r6,r5
	bbnezc	r6,00000000,00408D28

l00408C46:
	bneiuc	r4,0000002A,00408D82

l00408C4A:
	lbu	r6,0001(r7)
	addiu	r5,r6,FFFFFFD0
	bgeiuc	r5,0000000A,00408D36

l00408C54:
	lbu	r5,0002(r7)
	bneiuc	r5,00000024,00408D36

l00408C5A:
	lw	r17,0020(sp)
	addiu	r6,r6,3FFFFFD0
	li	r5,0000000A
	swxs	r5,r6(r4)
	lbu	r6,0001(r7)
	addiu	r7,r7,00000003
	lw	r17,0028(sp)
	addiu	r6,r6,1FFFFFD0
	sw	r7,004C(sp)
	lsa	r6,r6,r5,00000003
	li	r7,00000001
	sw	r7,001C(sp)
	lw	r6,0000(r6)
	sw	r6,0000(sp)

l00408C82:
	lw	r17,0000(sp)
	bgec	r7,r0,00408C96

l00408C88:
	addiu	r7,r0,00002000
	or	r17,r17,r7
	lw	r17,0000(sp)
	subu	r7,r0,r7
	sw	r7,0000(sp)

l00408C96:
	lw	r17,004C(sp)
	move	r30,r0
	li	r18,FFFFFFFF
	lbu	r6,0000(r7)
	bneiuc	r6,0000002E,00408CE2

l00408CA2:
	lbu	r6,0001(r7)
	bneiuc	r6,0000002A,00408DD0

l00408CA8:
	lbu	r6,0002(r7)
	addiu	r5,r6,FFFFFFD0
	bgeiuc	r5,0000000A,00408D96

l00408CB2:
	lbu	r5,0003(r7)
	bneiuc	r5,00000024,00408D96

l00408CB8:
	lw	r17,0020(sp)
	addiu	r6,r6,3FFFFFD0
	li	r5,0000000A
	swxs	r5,r6(r4)
	lbu	r6,0002(r7)
	addiu	r7,r7,00000004
	lw	r17,0028(sp)
	addiu	r6,r6,1FFFFFD0
	lsa	r6,r6,r5,00000003
	lw	r18,0000(r6)

l00408CD8:
	nor	r30,r0,r18
	sw	r7,004C(sp)
	srl	r30,r30,0000001F

l00408CE2:
	move	r23,r0

l00408CE4:
	lw	r17,004C(sp)
	lbu	r6,0000(r7)
	addiu	r6,r6,FFFFFFBF
	bgeiuc	r6,0000003A,00408D3A

l00408CF0:
	addiu	r6,r7,00000001
	addiupc	r5,0000A358
	sw	r6,004C(sp)
	lbu	r6,0000(r7)
	li	r7,0000003A
	mul	r7,r7,r23
	addu	r7,r7,r5
	addu	r7,r7,r6
	lbu	r5,-0041(r7)
	addiu	r7,r5,FFFFFFFF
	bltiuc	r7,00000008,00408DE2

l00408D10:
	beqzc	r5,00408D3A

l00408D12:
	bneiuc	r5,0000001B,00408DE6

l00408D16:
	bgec	r19,r0,00408D3A

l00408D1A:
	bnec	r0,r16,00408E12

l00408D1E:
	bc	00408B86

l00408D20:
	addiu	r7,r7,00000001
	li	r19,FFFFFFFF
	sw	r7,004C(sp)
	bc	00408C2A

l00408D28:
	li	r6,00000001
	sllv	r5,r6,r5
	addu	r7,r7,r6
	or	r17,r17,r5
	sw	r7,004C(sp)
	bc	00408C2C

l00408D36:
	lw	r17,001C(sp)
	beqzc	r7,00408D48

l00408D3A:
	balc	004049B0
	li	r7,00000016

l00408D40:
	sw	r7,0000(sp)
	li	r7,FFFFFFFF
	bc	004099CC

l00408D48:
	sw	r0,0000(sp)
	beqzc	r16,00408D6E

l00408D4C:
	lw	r17,000C(sp)
	lb	r5,000C(r7)
	bgec	r0,r5,00408D76

l00408D56:
	addiu	r6,r5,FFFFFFFC
	seb	r6,r6
	sb	r6,000C(r7)
	bltc	r6,r0,00408D76

l00408D66:
	lw	r7,0004(r7)
	subu	r7,r7,r5

l00408D6A:
	lw	r7,0000(r7)
	sw	r7,0000(sp)

l00408D6E:
	lw	r17,004C(sp)
	addiu	r7,r7,00000001
	sw	r7,004C(sp)
	bc	00408C82

l00408D76:
	lw	r17,000C(sp)
	lw	r17,000C(sp)
	lw	r7,0000(r7)
	addiu	r6,r7,00000004
	sw	r6,0040(sp)
	bc	00408D6A

l00408D82:
	addiu	r4,sp,0000004C
	balc	00408924
	sw	r4,0000(sp)
	bgec	r4,r0,00408C96

;; fn00408D8E: 00408D8E
;;   Called from:
;;     00408B90 (in fn00408B86)
;;     00408BA2 (in fn00408B86)
;;     00408C00 (in fn00408B86)
;;     00408D3E (in fn00408B86)
;;     00408D8A (in fn00408B86)
;;     00408E02 (in fn00408B86)
;;     00408FDA (in fn00408B86)
;;     004092B2 (in fn00409170)
;;     004097C0 (in fn00409170)
;;     004097D8 (in fn00409170)
;;     004097EC (in fn00409170)
;;     004098A2 (in fn00409170)
;;     004099CA (in fn00408B86)
fn00408D8E proc
	balc	004049B0
	li	r7,0000004B
	bc	00408D40

l00408D96:
	lw	r17,001C(sp)
	bnezc	r7,00408D3A

l00408D9A:
	move	r18,r0
	beqzc	r16,00408DBE

l00408D9E:
	lw	r17,000C(sp)
	lb	r5,000C(r7)
	bgec	r0,r5,00408DC4

l00408DA8:
	addiu	r6,r5,FFFFFFFC
	seb	r6,r6
	sb	r6,000C(r7)
	bltc	r6,r0,00408DC4

l00408DB8:
	lw	r7,0004(r7)
	subu	r7,r7,r5

l00408DBC:
	lw	r18,0000(r7)

l00408DBE:
	lw	r17,004C(sp)
	addiu	r7,r7,00000002
	bc	00408CD8

l00408DC4:
	lw	r17,000C(sp)
	lw	r17,000C(sp)
	lw	r7,0000(r7)
	addiu	r6,r7,00000004
	sw	r6,0040(sp)
	bc	00408DBC

l00408DD0:
	addiu	r7,r7,00000001
	addiu	r4,sp,0000004C
	sw	r7,004C(sp)
	balc	00408924
	addiu	r30,r0,00000001
	move	r18,r4
	bc	00408CE2

l00408DE2:
	move	r23,r5
	bc	00408CE4

l00408DE6:
	bltc	r19,r0,00408E00

l00408DEA:
	lw	r17,0020(sp)
	swxs	r5,r19(r7)
	lw	r17,0028(sp)
	lsa	r19,r19,r7,00000003
	lwm	r6,0000(r19),00000002
	swm	r6,0058(sp),00000002
	bc	00408D1A

l00408E00:
	bnezc	r16,00408E0A

l00408E02:
	sw	r0,0004(sp)

l00408E04:
	lw	r17,0004(sp)
	restore.jrc	000002E0,r30,0000000A

l00408E0A:
	lw	r17,000C(sp)
	addiu	r4,sp,00000058
	balc	00408976

l00408E12:
	lw	r17,004C(sp)
	lbu	r19,-0001(r7)
	beqc	r0,r23,00408E26

l00408E1C:
	andi	r7,r19,0000000F
	bneiuc	r7,00000003,00408E26

l00408E22:
	ins	r19,r0,00000005,00000001

l00408E26:
	bbeqzc	r17,0000000D,00408E2E

l00408E2A:
	ins	r17,r0,00000000,00000001

l00408E2E:
	addiu	r7,r19,FFFFFFBF
	bltiuc	r7,00000038,00408E3A

l00408E36:
	bc	004099D2

l00408E3A:
	addiupc	r6,0000A102
	lwxs	r7,r7(r6)
	jrc	r7
00408E42       EC CA 41 45 E0 04 D6 A1 F7 20 47 3C E0 D8   ..AE..... G<..
00408E50 F6 34 DD 84 04 60 71 7F 2D 19 F6 34 DD 84 04 20 .4...`q.-..4... 
00408E60 74 5F 23 19 F6 34 C1 34 70 97 1B 19 F6 34 C1 34 t_#..4.4p....4.4
00408E70 70 97 C6 80 9F C0 71 97 0D 19 88 D3 D2 80 08 50 p.....q........P
00408E80 C7 20 10 96 FC 50 F8 D1 D6 34 13 81 20 20 F7 36 . ...P...4..  .6
00408E90 BD 02 B8 00 E6 10 B7 10 A7 20 90 22 2A BA E6 22 ......... ."*.."
00408EA0 90 32 E0 04 4E 91 E7 12 10 9B 24 CA 0C 18 73 82 .2..N.....$...s.
00408EB0 84 C0 C0 02 02 00 67 22 50 B9 C0 8B C8 00 12 A8 ......g"P.......
00408EC0 CD BE 20 82 10 E4 C2 18 7F F2 40 05 72 A1 44 21 .. .......@.r.D!
00408ED0 07 21 BF 92 FC 33 88 20 90 22 95 84 00 10 85 80 .!...3. ."......
00408EE0 1C C0 CC 53 DC 32 B1 1B BD 02 B8 00 D6 34 B7 34 ...S.2.......4.4
00408EF0 F5 10 A6 20 90 22 1E BA E0 06 F8 90 24 CA BB 1F ... ."......$...
00408F00 A7 22 D0 B1 77 12 56 22 50 B3 ED 92 D3 22 10 BE ."..w.V"P...."..
00408F10 D6 82 01 50 A5 1B 67 F2 BF 92 84 00 30 00 6B 33 ...P..g.....0.k3
00408F20 95 84 00 10 85 80 1D C0 4C 53 DB 32 C5 1B DD A4 ........LS.2....
00408F30 58 24 07 88 1C 80 C0 20 D0 31 E0 20 D0 39 C0 20 X$..... .1. .9. 
00408F40 90 2B C0 02 01 00 FF B2 DD A4 58 2C E0 06 A4 90 .+........X,....
00408F50 1A 18 34 CA 24 58 E0 04 9A 90 F1 82 01 20 67 02 ..4.$X....... g.
00408F60 02 00 E0 22 90 B3 E7 22 10 9A F3 12 6E 73 9D A4 ..."..."....ns..
00408F70 58 24 FF 2B 6B F9 A4 12 41 1B C0 02 01 00 E3 62 X$.+k...A......b
00408F80 71 90 00 00 E7 1B 12 88 39 BF DD A4 58 24 7C 53 q.......9...X$|S
00408F90 EE 73 04 BB 40 8A 14 0A C6 80 01 50 A7 22 D0 39 .s..@......P.".9
00408FA0 7E B3 EE 71 F2 20 50 33 C7 20 10 96 B3 22 D0 99 ~..q. P3. ..."..
00408FB0 E0 60 FF FF FF 7F 72 22 50 F3 C7 22 D0 39 D3 23 .`....r"P..".9.#
00408FC0 10 96 D2 13 47 AA C7 BD                         ....G...        

l00408FC8:
	lw	r17,0000(sp)
	addu	r20,r30,r22
	lw	r4,0000(sp)
	slt	r7,r7,r20
	movn	r18,r20,r7

l00408FD8:
	lw	r17,0008(sp)
	bltc	r7,r18,00408D8E

l00408FDE:
	movep	r7,r8,r20,r17
	move	r6,r18
	li	r5,00000020
	move.balc	r4,r16,00408B22
	movep	r5,r6,r23,r22
	move.balc	r4,r16,00408962
	lui	r8,00000010
	movep	r6,r7,r18,r20
	xor	r8,r17,r8
	li	r5,00000030
	move	r22,r18
	move.balc	r4,r16,00408B22
	move	r6,r30
	movep	r7,r8,r19,r0
	li	r5,00000030
	move.balc	r4,r16,00408B22
	movep	r5,r6,r21,r19
	move.balc	r4,r16,00408962
	addiu	r8,r0,00002000
	xor	r8,r17,r8
	li	r5,00000020
	movep	r6,r7,r18,r20
	move.balc	r4,r16,00408B22
	bc	00408B86
00409024             F6 34 20 82 10 E4 E0 06 C6 8F 01 D1     .4 .........
00409030 EE 71 BD 02 B7 00 FD 84 B7 10 71 1B FF 2B 71 B9 .q........q..+q.
00409040 40 16 FF 2B A5 B9 A4 12 B2 80 00 40 E0 60 FF FF @..+.......@.`..
00409050 FF 7F B2 20 10 3A F5 BC 00 2A E4 18 95 20 50 99 ... .:...*... P.
00409060 12 88 06 80 B8 5F E0 A8 25 3D 44 12 20 82 10 E4 ....._..%=D. ...
00409070 E0 06 80 8F 37 1B B6 36 E0 04 84 8F A7 22 10 AA ....7..6....."..
00409080 C7 1B F6 34 7F D1 1A B4 F9 B4 D9 73 F6 B4 96 36 ...4.......s...6
00409090 60 12 53 8A 16 C0 B4 74 92 9A 54 72 8C 92 00 2A `.S....t..Tr...*
004090A0 4E 3F 04 A8 9D BC AF B1 87 88 34 C0 13 A8 DF BC N?........4.....
004090B0 C0 34 A0 D2 3B BF 80 12 1F 0A 67 FA B6 36 74 AA .4..;.....g..6t.
004090C0 22 C0                                           ".              

l004090C2:
	addiu	r8,r0,00002000
	lw	r17,0000(sp)
	move	r7,r19
	xor	r8,r17,r8
	li	r5,00000020
	move.balc	r4,r16,00408B22
	lw	r17,0000(sp)
	slt	r22,r7,r19
	movn	r7,r19,r22

l004090DE:
	bc	00409330
004090E0 36 B2 AF 1B B5 74 DB 9A 54 72 AC 92 00 2A 00 3F 6....t..Tr...*.?
004090F0 84 3E C4 10 93 AA CB FF D4 72 1F 0A 65 F8 BF 1B .>.......r..e...
00409100 C0 8B 04 00 12 A8 87 BC F7 34 B6 36 C7 80 5F C0 .........4.6.._.
00409110 15 B4 C4 B4 0E 9B E1 60 00 00 00 80 C0 04 E8 8E .......`........
00409120 C6 B4 1C 18 34 CA 0E 5A 91 F2 A0 20 90 33 C4 B4 ....4..Z... .3..
00409130 C3 60 D3 8E 00 00 86 00 05 00 A6 20 10 22 86 B4 .`......... ."..
00409140 C7 80 80 F7 A0 60 FF FF EF 7F C5 A8 F4 C1 55 73 .....`........Us
00409150 F5 BC 00 2A DA 3D AC BC 00 2A 64 5E 6B BC E4 12 ...*.=...*d^k...
00409160 C5 13 95 FE 00 2A 08 69 80 A8 4C 02 F3 80 20 00 .....*.i..L... .

;; fn00409170: 00409170
fn00409170 proc
	sw	r7,0024(sp)
	bneiuc	r7,00000021,00409400

l00409176:
	andi	r7,r19,00000020
	sw	r7,0030(sp)
	beqzc	r7,00409186

l0040917E:
	lw	r17,0018(sp)
	addiu	r7,r7,00000009
	sw	r7,0018(sp)

l00409186:
	lw	r17,0010(sp)
	addiu	r7,r7,00000002
	sw	r7,0008(sp)
	lui	r7,00000412
	sw	r7,0024(sp)
	bgeiuc	r18,0000000C,004091E4

l00409196:
	lui	r7,00000412
	addiu	r20,r0,0000000C
	lw	r10,0238(r7)
	subu	r20,r20,r18
	lw	r11,023C(r7)

l004091AA:
	addiu	r20,r20,FFFFFFFF
	li	r7,FFFFFFFF
	bnec	r20,r7,004093C0

l004091B2:
	lw	r17,0018(sp)
	lbu	r7,0000(r7)
	bneiuc	r7,0000002D,004093D4

l004091BA:
	lui	r20,FFF80000
	move	r22,r23
	xor	r21,r20,r30
	move	r6,r10
	move	r7,r11
	swm	r10,0010(sp),00000002
	movep	r4,r5,r22,r21
	balc	0040FAE0
	lwm	r10,0010(sp),00000002
	movep	r6,r7,r4,r5
	move	r5,r11
	move.balc	r4,r10,0040EFC0
	move	r21,r4
	xor	r20,r20,r5

l004091E4:
	lw	r17,0054(sp)
	addiu	r22,sp,00000078
	move	r6,r22
	sra	r4,r7,0000001F
	xor	r7,r7,r4
	subu	r4,r7,r4
	sra	r5,r4,0000001F
	balc	004088E0
	bnec	r4,r22,0040920A

l00409200:
	li	r7,00000030
	addiu	r4,sp,00000077
	sb	r7,0077(sp)

l0040920A:
	lw	r17,0054(sp)
	li	r6,0000002D
	li	r5,0000002B
	addiu	r19,r19,0000000F
	slti	r7,r7,00000000
	move	r30,r22
	movz	r6,r5,r7

l0040921E:
	addiu	r7,r4,FFFFFFFE
	sb	r19,-0002(r4)
	sb	r6,-0001(r4)
	sw	r7,0010(sp)

l0040922C:
	movep	r4,r5,r21,r20
	sw	r21,0034(sp)
	balc	004046C0
	addiupc	r7,00009E08
	lw	r17,0030(sp)
	lbux	r7,r4(r7)
	addiu	r19,r30,00000001
	or	r7,r7,r6
	sb	r7,0000(r30)
	balc	00410170
	lw	r18,0034(sp)
	movep	r6,r7,r4,r5
	move	r4,r11
	move.balc	r5,r20,0040FAE0
	lw	r17,0024(sp)
	lw	r6,0268(r7)
	lw	r7,026C(r7)
	balc	00404330
	subu	r7,r19,r22
	movep	r11,r23,r4,r5
	move	r21,r11
	move	r20,r23
	bneiuc	r7,00000001,00409292

l00409272:
	move	r4,r11
	movep	r6,r7,r0,r0
	sw	r11,0034(sp)
	move.balc	r5,r23,0040FA70
	lw	r18,0034(sp)
	bnezc	r4,00409288

l00409280:
	bltc	r0,r18,00409288

l00409284:
	bbeqzc	r17,00000003,0040929E

l00409288:
	li	r7,0000002E
	addiu	r19,r30,00000002
	sb	r7,0001(r30)

l00409292:
	movep	r6,r7,r0,r0
	move	r4,r11
	move.balc	r5,r23,0040FA70
	bnec	r0,r4,004093F6

l0040929E:
	lw	r17,0010(sp)
	lw	r17,0008(sp)
	subu	r23,r22,r7
	li	r7,7FFFFFFD
	subu	r7,r7,r23
	subu	r7,r7,r6
	bltc	r7,r18,00408D8E

l004092B6:
	subu	r19,r19,r22
	beqc	r0,r18,004093FA

l004092BE:
	addiu	r7,r19,FFFFFFFF
	bltc	r18,r7,004093FA

l004092C6:
	addiu	r9,r18,00000002
	addu	r20,r9,r23

l004092CE:
	lw	r17,0008(sp)
	lw	r17,0000(sp)
	li	r5,00000020
	addu	r21,r7,r20
	movep	r7,r8,r21,r17
	move.balc	r4,r16,00408B22
	lw	r17,0008(sp)
	lw	r17,0018(sp)
	move.balc	r4,r16,00408962
	lw	r17,0000(sp)
	lui	r8,00000010
	move	r7,r21
	xor	r8,r17,r8
	li	r5,00000030
	move.balc	r4,r16,00408B22
	movep	r5,r6,r22,r19
	move.balc	r4,r16,00408962
	subu	r6,r20,r23
	movep	r7,r8,r0,r0
	subu	r6,r6,r19
	li	r5,00000030
	move.balc	r4,r16,00408B22
	lw	r17,0010(sp)
	move	r6,r23
	move.balc	r4,r16,00408962
	addiu	r8,r0,00002000
	lw	r17,0000(sp)
	move	r7,r21
	xor	r8,r17,r8
	li	r5,00000020
	move.balc	r4,r16,00408B22
	lw	r17,0000(sp)
	slt	r22,r7,r21
	movn	r7,r21,r22

l00409330:
	move	r22,r7
	bc	00408B86
00409336                   01 D3 C4 B4 C3 60 CB 8C 00 00       .....`....
00409340 DF 19 F5 BC C0 04 D4 8C 73 82 20 20 80 06 D0 8C ........s.  ....
00409350 B6 FE 66 22 10 A6 AC BC B6 BE 00 2A 12 67 0C 9A ..f".......*.g..
00409360 E0 04 C0 8C 80 06 C0 8C 67 22 10 A6 E4 34 11 11 ........g"...4..
00409370 C0 34 00 81 10 E4 67 02 03 00 A0 D2 F3 10 1F 0A .4....g.........
00409380 A1 F7 C4 34 A6 34 1F 0A D9 F5 03 D3 90 BE FF 2B ...4.4.........+
00409390 D1 F5 00 01 00 20 C0 34 F3 10 11 21 10 43 A0 D2 ..... .4...!.C..
004093A0 1F 0A 7F F7 E0 34 67 22 50 B3 D3 22 10 3E C7 12 .....4g"P..".>..
004093B0 07 88 D3 B7 FF 29 D7 F9 F5 34 FF 90 F5 B4 AD 19 .....)...4......

l004093C0:
	lw	r17,0024(sp)
	move	r5,r11
	lw	r6,0268(r7)
	lw	r7,026C(r7)
	move.balc	r4,r10,00404330
	movep	r10,r11,r4,r5
	bc	004091AA

l004093D4:
	move	r21,r23
	move	r20,r30
	move	r6,r10
	move	r7,r11
	movep	r4,r5,r21,r20
	swm	r10,0010(sp),00000002
	balc	0040EFC0
	lwm	r10,0010(sp),00000002
	move	r6,r10
	move	r7,r11
	balc	0040FAE0
	movep	r21,r20,r4,r5
	bc	004091E4

l004093F6:
	move	r30,r19
	bc	0040922C

l004093FA:
	addu	r20,r23,r19
	bc	004092CE

l00409400:
	slti	r6,r18,00000000
	li	r7,00000006
	movn	r18,r7,r6

l0040940A:
	movep	r6,r7,r0,r0
	move	r5,r30
	move.balc	r4,r23,0040FA70
	beqzc	r4,00409434

l00409414:
	lui	r7,00000412
	move	r21,r23
	lw	r6,0270(r7)
	move	r20,r30
	lw	r7,0274(r7)
	movep	r4,r5,r21,r20
	balc	00404330
	lw	r17,0054(sp)
	movep	r21,r20,r4,r5
	addiu	r7,r7,FFFFFFE4
	sw	r7,0054(sp)

l00409434:
	lw	r16,0054(sp)
	addiu	r7,sp,000000B8
	sw	r7,0008(sp)
	bltc	r2,r0,00409444

l0040943E:
	addiu	r7,sp,000001D8
	sw	r7,0008(sp)

l00409444:
	lw	r7,0008(sp)

l00409446:
	movep	r4,r5,r21,r20
	sw	r2,0034(sp)
	sw	r21,0030(sp)
	balc	00410110
	sw	r4,0000(r30)
	addiu	r30,r30,00000004
	balc	004101D0
	lw	r18,0030(sp)
	movep	r6,r7,r4,r5
	move	r4,r11
	move.balc	r5,r20,0040FAE0
	lui	r7,00000412
	lw	r6,0278(r7)
	lw	r7,027C(r7)
	balc	00404330
	movep	r11,r8,r4,r5
	movep	r21,r20,r4,r5
	movep	r6,r7,r0,r0
	move	r4,r11
	move.balc	r5,r8,0040FA70
	lw	r16,0034(sp)
	bnezc	r4,00409446

l00409484:
	lw	r5,0008(sp)
	move	r7,r0

l00409488:
	bltc	r0,r2,00409574

l0040948C:
	beqzc	r7,00409490

l0040948E:
	sw	r2,0054(sp)

l00409490:
	li	r7,00000009
	addiu	r6,r18,00000019
	lw	r17,0054(sp)
	divu	r20,r6,r7
	addiu	r20,r20,00000001
	move	r7,r0
	sll	r2,r20,00000002

l004094A4:
	bltc	r5,r0,004095F0

l004094A8:
	beqzc	r7,004094AC

l004094AA:
	sw	r5,0054(sp)

l004094AC:
	move	r21,r0
	bgeuc	r23,r30,004094C8

l004094B2:
	lw	r17,0008(sp)
	lw	r6,0000(r23)
	subu	r21,r7,r23
	li	r7,0000000A
	sra	r21,r21,00000002
	lsa	r21,r21,r21,00000003

l004094C4:
	bgeuc	r6,r7,00409664

l004094C8:
	lw	r17,0024(sp)
	move	r6,r21
	lw	r17,0024(sp)
	xori	r7,r7,00000066
	movz	r6,r0,r7

l004094D6:
	subu	r7,r18,r6
	move	r6,r0
	bneiuc	r5,00000027,004094E2

l004094DE:
	sltu	r6,r0,r18

l004094E2:
	subu	r7,r7,r6
	lw	r17,0008(sp)
	subu	r6,r30,r6
	sra	r6,r6,00000002
	addiu	r6,r6,FFFFFFFF
	lsa	r6,r6,r6,00000003
	bgec	r7,r6,00409706

l004094F8:
	li	r5,00000009
	lw	r17,0008(sp)
	addiu	r7,r7,00002400
	div	r20,r7,r5
	addiu	r20,r20,3FFFFC01
	addiu	r8,r0,0000000A
	lsa	r20,r20,r6,00000002
	mod	r6,r7,r5
	addiu	r7,r6,00000001

l0040951A:
	bneiuc	r7,00000009,0040966C

l0040951E:
	lw	r2,0000(r20)
	modu	r7,r2,r8
	bnezc	r7,00409530

l00409528:
	addiu	r6,r20,00000004
	beqc	r30,r6,004096FC

l00409530:
	divu	r6,r2,r8
	bbnezc	r6,00000000,0040954E

l00409538:
	li	r6,3B9ACA00
	bnec	r8,r6,00409674

l00409542:
	bgeuc	r23,r20,00409674

l00409546:
	lw	r6,-0004(r20)
	bbeqzc	r6,00000000,00409674

l0040954E:
	lui	r6,00000412
	li	r4,00000001
	lw	r5,024C(r6)

l00409558:
	sra	r6,r8,00000001
	bltuc	r7,r6,00409680

l00409560:
	bnec	r6,r7,0040956A

l00409562:
	addiu	r6,r20,00000004
	beqc	r30,r6,00409748

l0040956A:
	lui	r6,00000412
	lw	r6,0264(r6)
	bc	00409688

l00409574:
	slti	r3,r2,0000001E
	li	r7,0000001D
	movn	r7,r2,r3

l0040957E:
	addiu	r21,r30,FFFFFFFC
	move	r3,r7
	move	r20,r0

l00409586:
	bgeuc	r21,r23,004095A8

l0040958A:
	beqc	r0,r20,00409594

l0040958E:
	sw	r20,-0004(r23)
	addiu	r23,r23,FFFFFFFC

l00409594:
	bgeuc	r23,r30,004095A0

l00409598:
	addiu	r7,r30,FFFFFFFC
	lw	r6,0000(r7)
	beqzc	r6,004095EC

l004095A0:
	subu	r2,r2,r3
	li	r7,00000001
	bc	00409488

l004095A8:
	lw	r4,0000(r21)
	move	r6,r3
	move	r5,r0
	sw	r3,0034(sp)
	sw	r2,0038(sp)
	balc	0040EA50
	addu	r20,r20,r4
	move	r7,r0
	sltu	r4,r20,r4
	li	r6,3B9ACA00
	addu	r8,r4,r5
	movep	r4,r5,r20,r8
	sw	r8,0030(sp)
	balc	0040ED50
	lw	r18,0030(sp)
	li	r6,3B9ACA00
	sw	r4,0000(r21)
	move	r7,r0
	movep	r4,r5,r20,r8
	balc	0040EAB0
	addiu	r21,r21,FFFFFFFC
	move	r20,r4
	lw	r16,0034(sp)
	lw	r16,0038(sp)
	bc	00409586

l004095EC:
	move	r30,r7
	bc	00409594

l004095F0:
	addiu	r7,r0,FFFFFFF7
	li	r4,00000009
	bltc	r5,r7,004095FE

l004095FA:
	subu	r4,r0,r5

l004095FE:
	li	r6,00000001
	li	r3,3B9ACA00
	sllv	r6,r6,r4
	srav	r3,r3,r4
	addiu	r6,r6,FFFFFFFF
	move	r7,r23
	move	r21,r0

l00409614:
	bltuc	r7,r30,0040964C

l00409618:
	lw	r7,0000(r23)
	bnezc	r7,0040961E

l0040961C:
	addiu	r23,r23,00000004

l0040961E:
	beqc	r0,r21,00409628

l00409622:
	sw	r21,0000(r30)
	addiu	r30,r30,00000004

l00409628:
	lw	r17,0024(sp)
	lw	r17,0008(sp)
	xori	r7,r7,00000066
	movn	r6,r23,r7

l00409634:
	move	r7,r6
	subu	r6,r30,r6
	sra	r6,r6,00000002
	bgec	r20,r6,00409646

l00409642:
	addu	r30,r7,r2

l00409646:
	addu	r5,r5,r4
	li	r7,00000001
	bc	004094A4

l0040964C:
	lw	r8,0000(r7)
	srlv	r12,r8,r4
	addu	r21,r12,r21
	sw	r21,0000(r7)
	and	r21,r8,r6
	mul	r21,r21,r3
	addiu	r7,r7,00000004
	bc	00409614

l00409664:
	li	r5,0000000A
	addiu	r21,r21,00000001
	mul	r7,r7,r5
	bc	004094C4

l0040966C:
	li	r6,0000000A
	addiu	r7,r7,00000001
	mul	r8,r8,r6
	bc	0040951A

l00409674:
	lui	r6,00000412
	move	r4,r0
	lw	r5,0244(r6)
	bc	00409558

l00409680:
	lui	r6,00000412
	lw	r6,0254(r6)

l00409688:
	lw	r18,0010(sp)
	beqc	r0,r9,004096A4

l0040968E:
	lw	r18,0018(sp)
	lbu	r3,0000(r9)
	bneiuc	r3,0000002D,004096A4

l00409698:
	lui	r3,FFF80000
	xor	r5,r5,r3
	xor	r6,r6,r3

l004096A4:
	subu	r2,r2,r7
	move	r3,r6
	move	r12,r0
	move	r7,r3
	move	r6,r12
	sw	r2,0000(r20)
	sw	r5,0030(sp)
	sw	r4,0034(sp)
	sw	r2,0038(sp)
	sw	r8,003C(sp)
	balc	0040EFC0
	lw	r16,0030(sp)
	lw	r19,0034(sp)
	movep	r6,r7,r4,r5
	move	r4,r12
	move	r5,r3
	balc	0040FA70
	beqzc	r4,004096FC

l004096D0:
	lw	r16,0038(sp)
	lw	r18,003C(sp)
	addu	r7,r8,r2

l004096D8:
	sw	r7,0000(r20)
	li	r6,3B9AC9FF
	lw	r7,0000(r20)
	bltuc	r6,r7,00409752

l004096E6:
	lw	r17,0008(sp)
	lw	r6,0000(r23)
	subu	r21,r7,r23
	li	r7,0000000A
	sra	r21,r21,00000002
	lsa	r21,r21,r21,00000003

l004096F8:
	bgeuc	r6,r7,00409766

l004096FC:
	addiu	r20,r20,00000004
	sltu	r7,r20,r30
	movn	r30,r20,r7

l00409706:
	bgeuc	r23,r30,00409712

l0040970A:
	addiu	r7,r30,FFFFFFFC
	lw	r6,0000(r7)
	beqzc	r6,0040976E

l00409712:
	lw	r17,0024(sp)
	bneiuc	r7,00000027,004097B0

l00409718:
	li	r7,00000001
	movz	r18,r7,r18

l0040971E:
	bgec	r21,r18,00409772

l00409722:
	addiu	r7,r0,FFFFFFFC
	bltc	r21,r7,00409772

l0040972A:
	addiu	r7,r21,00000001
	addiu	r19,r19,FFFFFFFF
	subu	r18,r18,r7

l00409732:
	bbnezc	r17,00000003,004097B0

l00409736:
	bgeuc	r23,r30,00409742

l0040973A:
	lw	r6,-0004(r30)
	li	r7,0000000A
	bnezc	r6,0040977E

l00409742:
	addiu	r22,r0,00000009
	bc	00409784

l00409748:
	lui	r6,00000412
	lw	r6,025C(r6)
	bc	00409688

l00409752:
	addiu	r20,r20,FFFFFFFC
	sw	r11,0001(r20)
	bgeuc	r20,r23,00409760

l0040975A:
	sw	r0,-0004(r23)
	addiu	r23,r23,FFFFFFFC

l00409760:
	lw	r7,0000(r20)
	addiu	r7,r7,00000001
	bc	004096D8

l00409766:
	li	r5,0000000A
	addiu	r21,r21,00000001
	mul	r7,r7,r5
	bc	004096F8

l0040976E:
	move	r30,r7
	bc	00409706

l00409772:
	addiu	r19,r19,FFFFFFFE
	addiu	r18,r18,FFFFFFFF
	bc	00409732

l00409778:
	li	r5,0000000A
	addiu	r22,r22,00000001
	mul	r7,r7,r5

l0040977E:
	modu	r5,r6,r7
	beqzc	r5,00409778

l00409784:
	lw	r17,0008(sp)
	ori	r6,r19,00000020
	subu	r7,r30,r7
	sra	r7,r7,00000002
	addiu	r7,r7,FFFFFFFF
	lsa	r7,r7,r7,00000003
	bneiuc	r6,00000026,00409858

l0040979C:
	subu	r22,r7,r22
	slti	r7,r22,00000000
	movn	r22,r0,r7

l004097A8:
	slt	r7,r22,r18
	movn	r18,r22,r7

l004097B0:
	li	r6,00000001
	bnezc	r18,004097B8

l004097B4:
	ext	r6,r17,00000003,00000001

l004097B8:
	li	r7,7FFFFFFE
	subu	r5,r7,r6
	bltc	r5,r18,00408D8E

l004097C4:
	addiu	r22,r18,00000001
	addiu	r7,r7,00000001
	addu	r22,r22,r6
	ori	r20,r19,00000020
	subu	r7,r7,r22
	bneiuc	r20,00000026,0040985C

l004097D8:
	bltc	r7,r21,00408D8E

l004097DC:
	bgec	r0,r21,004097E2

l004097E0:
	addu	r22,r22,r21

l004097E2:
	lw	r17,0010(sp)
	li	r7,7FFFFFFF
	subu	r7,r7,r6
	bltc	r7,r22,00408D8E

l004097F0:
	addu	r19,r6,r22
	lw	r17,0000(sp)
	movep	r7,r8,r19,r17
	li	r5,00000020
	move.balc	r4,r16,00408B22
	lw	r17,0010(sp)
	lw	r17,0018(sp)
	move.balc	r4,r16,00408962
	lui	r8,00000010
	lw	r17,0000(sp)
	xor	r8,r17,r8
	move	r7,r19
	li	r5,00000030
	move.balc	r4,r16,00408B22
	bneiuc	r20,00000026,0040991A

l0040981C:
	lw	r17,0008(sp)
	sltu	r20,r23,r7
	movn	r7,r23,r20

l00409826:
	move	r20,r7
	move	r21,r7

l0040982A:
	lw	r17,0008(sp)
	bgeuc	r7,r21,004098B2

l00409830:
	bnezc	r18,00409836

l00409832:
	bbeqzc	r17,00000003,00409840

l00409836:
	li	r6,00000001
	addiupc	r5,00007B00
	move.balc	r4,r16,00408962

l00409840:
	bgeuc	r21,r30,00409848

l00409844:
	bltc	r0,r18,004098E8

l00409848:
	li	r7,00000009
	move	r8,r0
	addu	r6,r18,r7
	li	r5,00000030
	move.balc	r4,r16,00408B22
	bc	004090C2

l00409858:
	addu	r7,r7,r21
	bc	0040979C

l0040985C:
	sra	r6,r21,0000001F
	addiu	r8,sp,00000078
	xor	r4,r6,r21
	sw	r8,0024(sp)
	subu	r4,r4,r6
	move	r6,r8
	sra	r5,r4,0000001F
	sw	r7,002C(sp)
	balc	004099EA
	lw	r18,0024(sp)
	lw	r17,002C(sp)

l0040987A:
	subu	r6,r8,r4
	bltic	r6,00000002,004098AA

l00409882:
	slti	r21,r21,00000000
	li	r6,0000002D
	li	r5,0000002B
	sb	r19,-0002(r4)
	movz	r6,r5,r21

l00409892:
	move	r21,r6
	addiu	r6,r4,FFFFFFFE
	subu	r8,r8,r6
	sb	r21,-0001(r4)
	sw	r6,002C(sp)
	bltc	r7,r8,00408D8E

l004098A6:
	addu	r22,r22,r8
	bc	004097E2

l004098AA:
	addiu	r4,r4,FFFFFFFF
	li	r6,00000030
	sb	r6,0000(r4)
	bc	0040987A

l004098B2:
	addiu	r22,sp,00000081
	lw	r4,0000(r21)
	movep	r5,r6,r0,r22
	balc	004099EA
	move	r5,r4
	bnec	r20,r21,004098D6

l004098C2:
	bnec	r4,r22,004098DC

l004098C6:
	li	r7,00000030
	addiu	r5,sp,00000080
	sb	r7,0080(sp)
	bc	004098DC

l004098D0:
	addiu	r5,r5,FFFFFFFF
	li	r7,00000030
	sb	r7,0000(r5)

l004098D6:
	addiu	r7,sp,00000078
	bltuc	r7,r5,004098D0

l004098DC:
	subu	r6,r22,r5
	move.balc	r4,r16,00408962
	addiu	r21,r21,00000004
	bc	0040982A

l004098E8:
	lw	r4,0000(r21)
	move	r5,r0
	addiu	r6,sp,00000081
	balc	004099EA
	move	r5,r4

l004098F4:
	addiu	r7,sp,00000078
	bltuc	r7,r5,00409912

l004098FA:
	slti	r6,r18,0000000A
	li	r7,00000009
	movn	r7,r18,r6

l00409904:
	move	r6,r7
	addiu	r21,r21,00000004
	move.balc	r4,r16,00408962
	addiu	r18,r18,FFFFFFF7
	bc	00409840

l00409912:
	addiu	r5,r5,FFFFFFFF
	li	r7,00000030
	sb	r7,0000(r5)
	bc	004098F4

l0040991A:
	bltuc	r23,r30,00409922

l0040991E:
	addiu	r30,r23,00000004

l00409922:
	move	r21,r23

l00409924:
	bgeuc	r21,r30,0040992C

l00409928:
	bgec	r18,r0,00409948

l0040992C:
	li	r7,00000012
	addu	r6,r18,r7
	move	r8,r0
	li	r5,00000030
	move.balc	r4,r16,00408B22
	lw	r17,002C(sp)
	addiu	r6,sp,00000078
	movep	r4,r5,r16,r7
	subu	r6,r6,r7
	balc	00408962
	bc	004090C2

l00409948:
	addiu	r20,sp,00000081
	lw	r4,0000(r21)
	movep	r5,r6,r0,r20
	balc	004099EA
	move	r5,r4
	bnec	r4,r20,00409960

l00409958:
	li	r7,00000030
	addiu	r5,sp,00000080
	sb	r7,0080(sp)

l00409960:
	move	r22,r5
	bnec	r21,r23,0040998A

l00409966:
	li	r6,00000001
	addiu	r22,r5,00000001
	move.balc	r4,r16,00408962
	bnezc	r18,00409976

l00409972:
	bbeqzc	r17,00000003,00409990

l00409976:
	li	r6,00000001
	addiupc	r5,000079C0
	move.balc	r4,r16,00408962
	bc	00409990

l00409982:
	addiu	r22,r22,FFFFFFFF
	li	r7,00000030
	sb	r7,0000(r22)

l0040998A:
	addiu	r7,sp,00000078
	bltuc	r7,r22,00409982

l00409990:
	subu	r20,r20,r22
	slt	r6,r20,r18
	move	r7,r20
	movz	r7,r18,r6

l0040999E:
	addiu	r21,r21,00000004
	movep	r5,r6,r22,r7
	move.balc	r4,r16,00408962
	subu	r18,r18,r20
	bc	00409924
004099AC                                     A7 12 67 12             ..g.
004099B0 FF 29 F9 F5                                     .)..            

l004099B4:
	lw	r17,0028(sp)
	lw	r17,000C(sp)
	lsa	r4,r16,r7,00000003
	addiu	r16,r16,00000001
	balc	00408976
	beqic	r16,0000000A,004099CA

l004099C6:
	bc	00408BAE

l004099CA:
	li	r7,00000001

l004099CC:
	sw	r7,0004(sp)
	bc	00408E04

l004099D2:
	addiu	r19,sp,000000B8
	addiupc	r23,0000861C
	subu	r19,r19,r21
	slt	r30,r18,r19
	movn	r18,r19,r30

l004099E4:
	move	r30,r18
	bc	00408FC8

;; fn004099EA: 004099EA
;;   Called from:
;;     00409874 (in fn00409170)
;;     004098BA (in fn00409170)
;;     004098F0 (in fn00409170)
;;     00409950 (in fn00409170)
fn004099EA proc
	bc	004088E0

;; vfprintf: 004099EE
;;   Called from:
;;     00408368 (in fprintf)
;;     004086F2 (in printf)
;;     00409B5E (in vsnprintf)
vfprintf proc
	save	00000110,ra,00000007
	movep	r16,r21,r4,r5
	move	r5,r6
	addiu	r4,sp,00000018
	li	r6,00000010
	li	r17,FFFFFFFF
	balc	0040A130
	li	r6,00000028
	move	r5,r0
	addu	r4,sp,r6
	balc	0040A690
	li	r6,00000010
	addiu	r5,sp,00000018
	addiu	r4,sp,00000008
	balc	0040A130
	addiu	r7,sp,00000028
	move	r8,r7
	addiu	r6,sp,00000008
	addiu	r7,sp,00000050
	movep	r4,r5,r0,r21
	balc	00408B72
	bltc	r4,r0,00409AA8

l00409A28:
	lw	r7,004C(r16)
	move	r20,r0
	bltc	r7,r0,00409A38

l00409A32:
	move.balc	r4,r16,0040D1D0
	move	r20,r4

l00409A38:
	lw	r7,0000(r16)
	lb	r6,004A(r16)
	andi	r18,r7,00000020
	bltc	r0,r6,00409A4C

l00409A46:
	ins	r7,r0,00000005,00000001
	sw	r7,0000(sp)

l00409A4C:
	lw	r7,0030(r16)
	move	r19,r0
	bnezc	r7,00409A64

l00409A52:
	addiu	r7,sp,000000A0
	lw	r19,002C(r16)
	sw	r7,0014(sp)
	sw	r7,001C(sp)
	sw	r7,002C(sp)
	li	r7,00000050
	sw	r7,0030(sp)
	addiu	r7,sp,000000F0
	sw	r7,0010(sp)

l00409A64:
	addiu	r7,sp,00000028
	addiu	r6,sp,00000008
	move	r8,r7
	addiu	r7,sp,00000050
	movep	r4,r5,r16,r21
	balc	00408B72
	move	r17,r4
	beqzc	r19,00409A90

l00409A76:
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r0,r0
	jalrc	ra,r7
	lw	r6,0014(r16)
	li	r7,FFFFFFFF
	sw	r0,0010(sp)
	movz	r17,r7,r6

l00409A88:
	sw	r0,0014(sp)
	sw	r0,001C(sp)
	sw	r19,002C(sp)
	sw	r0,0030(sp)

l00409A90:
	lw	r7,0000(r16)
	li	r6,FFFFFFFF
	andi	r5,r7,00000020
	or	r18,r18,r7
	movn	r17,r6,r5

l00409A9E:
	sw	r18,0000(sp)
	beqc	r0,r20,00409AA8

l00409AA4:
	move.balc	r4,r16,0040D210

l00409AA8:
	move	r4,r17
	restore.jrc	00000110,ra,00000007
00409AAE                                           00 00               ..

;; sn_write: 00409AB0
sn_write proc
	save	00000020,ra,00000006
	movep	r17,r20,r4,r5
	move	r19,r6
	lw	r16,0054(r17)
	lw	r18,0014(r17)
	lw	r5,001C(r17)
	lw	r6,0004(r16)
	subu	r7,r18,r5
	sltu	r18,r6,r7
	movz	r6,r7,r18

l00409ACA:
	move	r18,r6
	beqzc	r6,00409AE0

l00409ACE:
	lw	r4,0000(r16)
	balc	0040A130
	lw	r7,0000(r16)
	addu	r7,r7,r18
	sw	r7,0000(sp)
	lw	r7,0004(r16)
	subu	r18,r7,r18
	sw	r18,0004(sp)

l00409AE0:
	lw	r7,0004(r16)
	sltu	r18,r19,r7
	movn	r7,r19,r18

l00409AEA:
	move	r18,r7
	beqzc	r7,00409B02

l00409AEE:
	lw	r4,0000(r16)
	movep	r5,r6,r20,r18
	balc	0040A130
	lw	r7,0000(r16)
	addu	r7,r7,r18
	sw	r7,0000(sp)
	lw	r7,0004(r16)
	subu	r18,r7,r18
	sw	r18,0004(sp)

l00409B02:
	lw	r7,0000(r16)
	move	r4,r19
	sb	r0,0000(r7)
	lw	r7,002C(r17)
	sw	r7,0054(sp)
	sw	r7,005C(sp)
	restore.jrc	00000020,ra,00000006

;; vsnprintf: 00409B10
;;   Called from:
;;     00408846 (in snprintf)
;;     00409B88 (in vsprintf)
vsnprintf proc
	save	000000D0,ra,00000005
	movep	r16,r18,r4,r5
	move	r19,r6
	addiu	r17,sp,00000004
	li	r6,00000010
	addu	r4,sp,r6
	movn	r17,r16,r18

l00409B22:
	move.balc	r5,r7,0040A130
	move	r7,r0
	sw	r17,0008(sp)
	beqzc	r18,00409B30

l00409B2C:
	addiu	r7,r18,FFFFFFFF

l00409B30:
	addiu	r6,r0,00000090
	move	r5,r0
	addiu	r4,sp,00000020
	sw	r7,000C(sp)
	balc	0040A690
	addiupc	r7,FFFFFF6E
	sw	r7,0044(sp)
	li	r7,FFFFFFFF
	li	r16,FFFFFFFF
	sb	r7,006B(sp)
	addiu	r7,sp,00000008
	sw	sp,004C(sp)
	sw	r16,006C(sp)
	sw	r7,0074(sp)
	bltc	r18,r0,00409B64

l00409B58:
	addiu	r6,sp,00000010
	addiu	r4,sp,00000020
	sb	r0,0000(r17)
	move.balc	r5,r19,004099EE
	restore.jrc	000000D0,ra,00000005

l00409B64:
	balc	004049B0
	li	r7,0000004B
	sw	r7,0000(sp)
	move	r4,r16
	restore.jrc	000000D0,ra,00000005

;; vsprintf: 00409B70
;;   Called from:
;;     00408888 (in sprintf)
vsprintf proc
	save	00000020,ra,00000003
	movep	r16,r17,r4,r5
	move	r5,r6
	move	r4,sp
	li	r6,00000010
	balc	0040A130
	move	r7,sp
	move	r6,r17
	li	r5,7FFFFFFF
	move.balc	r4,r16,00409B10
	restore.jrc	00000020,ra,00000003
00409B8E                                           00 00               ..

;; do_read: 00409B90
do_read proc
	bc	0040D360

;; __isoc99_vsscanf: 00409B94
;;   Called from:
;;     004088C8 (in __isoc99_sscanf)
__isoc99_vsscanf proc
	save	000000B0,ra,00000003
	movep	r16,r17,r4,r5
	move	r5,r6
	move	r4,sp
	li	r6,00000010
	balc	0040A130
	addiu	r6,r0,00000090
	move	r5,r0
	addiu	r4,sp,00000010
	balc	0040A690
	addiupc	r7,FFFFFFDE
	sw	r7,0030(sp)
	move	r6,sp
	li	r7,FFFFFFFF
	addiu	r4,sp,00000010
	sw	r16,003C(sp)
	sw	r16,0064(sp)
	sw	r7,005C(sp)
	move.balc	r5,r17,0040D4A6
	restore.jrc	000000B0,ra,00000003
00409BC6                   00 00 00 00 00 00 00 00 00 00       ..........

;; atoi: 00409BD0
;;   Called from:
;;     00400832 (in fn00400832)
;;     0040DF32 (in __synccall)
atoi proc
	move	r7,r4
	bc	00409BFE

l00409BD4:
	li	r5,00000001

l00409BD6:
	addiu	r7,r7,00000001

l00409BD8:
	move	r4,r0
	bc	00409BEA

l00409BDC:
	move	r5,r0
	bc	00409BD6

l00409BE0:
	addiu	r8,r0,0000000A
	addiu	r7,r7,00000001
	mul	r4,r4,r8
	subu	r4,r4,r6

l00409BEA:
	lbu	r6,0000(r7)
	addiu	r6,r6,FFFFFFD0
	bltiuc	r6,0000000A,00409BE0

l00409BF4:
	bnezc	r5,00409C18

l00409BF6:
	subu	r4,r0,r4
	jrc	ra

l00409BFC:
	addiu	r7,r7,00000001

l00409BFE:
	lbu	r6,0000(r7)
	beqic	r6,00000020,00409BFC

l00409C04:
	addiu	r5,r6,FFFFFFF7
	bltiuc	r5,00000005,00409BFC

l00409C0C:
	beqic	r6,0000002B,00409BDC

l00409C10:
	beqic	r6,0000002D,00409BD4

l00409C14:
	move	r5,r0
	bc	00409BD8

l00409C18:
	jrc	ra
00409C1A                               00 00 00 00 00 00           ......

;; shl: 00409C20
;;   Called from:
;;     00409EA6 (in qsort)
;;     00409F48 (in fn00409F48)
shl proc
	bltiuc	r5,00000020,00409C2E

l00409C24:
	lw	r7,0000(r4)
	addiu	r5,r5,FFFFFFE0
	sw	r0,0000(sp)
	sw	r7,0004(sp)

l00409C2E:
	subu	r8,r0,r5
	lwm	r6,0000(r4),00000002
	sllv	r7,r7,r5
	srlv	r8,r6,r8
	or	r7,r7,r8
	sllv	r5,r6,r5
	sw	r5,0000(sp)
	sw	r7,0004(sp)
	jrc	ra

;; shr: 00409C4C
;;   Called from:
;;     00409DC0 (in trinkle)
;;     00409F4C (in fn00409F4C)
shr proc
	bltiuc	r5,00000020,00409C5A

l00409C50:
	lw	r7,0004(r4)
	addiu	r5,r5,FFFFFFE0
	sw	r0,0004(sp)
	sw	r7,0000(sp)

l00409C5A:
	lw	r6,0004(r4)
	subu	r8,r0,r5
	lw	r7,0000(r4)
	sllv	r8,r6,r8
	srlv	r7,r7,r5
	srlv	r5,r6,r5
	or	r7,r7,r8
	sw	r5,0004(sp)
	sw	r7,0000(sp)
	jrc	ra

;; cycle: 00409C78
;;   Called from:
;;     00409CE8 (in sift)
;;     00409DD6 (in trinkle)
cycle proc
	save	00000120,ra,00000008
	movep	r18,r19,r4,r5
	move	r20,r6
	bltic	r6,00000002,00409C8E

l00409C84:
	lsa	r22,r6,r19,00000002
	sw	sp,0000(r22)

l00409C8C:
	bnezc	r18,00409C92

l00409C8E:
	restore.jrc	00000120,ra,00000008

l00409C92:
	sltiu	r16,r18,00000101
	addiu	r7,r0,00000100
	movn	r7,r18,r16

l00409C9E:
	lw	r5,0000(r19)
	lw	r4,0000(r22)
	move	r6,r7
	move	r16,r7
	move	r17,r19
	move	r21,r0
	balc	0040A130

l00409CAE:
	move	r6,r16
	lwm	r4,0000(r17),00000002
	balc	0040A130
	lw	r7,0000(r17)
	addiu	r21,r21,00000001
	addu	r7,r7,r16
	sw	r7,0040(sp)
	addiu	r17,r17,00000004
	bnec	r20,r21,00409CAE

l00409CC6:
	subu	r18,r18,r16
	bc	00409C8C

;; sift: 00409CCA
;;   Called from:
;;     00409DDE (in trinkle)
;;     00409E68 (in qsort)
;;     00409EB2 (in qsort)
sift proc
	save	00000110,ra,00000008
	movep	r16,r21,r4,r5
	movep	r20,r17,r6,r7
	move	r22,r8
	li	r18,00000001
	sw	r16,000C(sp)
	bc	00409CFA

l00409CDA:
	lw	r17,000C(sp)
	move	r5,r19
	jalrc	ra,r20
	bltc	r4,r0,00409D1E

l00409CE4:
	move	r6,r18
	addiu	r5,sp,0000000C
	move.balc	r4,r21,00409C78
	restore.jrc	00000110,ra,00000008

l00409CF0:
	addiu	r7,sp,0000000C
	addiu	r17,r17,FFFFFFFF
	swxs	r16,r18(r7)

l00409CF8:
	addiu	r18,r18,00000001

l00409CFA:
	bltic	r17,00000002,00409CE4

l00409CFE:
	move	r6,r17
	subu	r7,r0,r21
	addiu	r6,r6,3FFFFFFE
	addu	r19,r16,r7
	lwxs	r6,r6(r22)
	lw	r17,000C(sp)
	subu	r7,r7,r6
	addu	r16,r16,r7
	move	r5,r16
	jalrc	ra,r20
	bgec	r4,r0,00409CDA

l00409D1E:
	movep	r4,r5,r16,r19
	jalrc	ra,r20
	bgec	r4,r0,00409CF0

l00409D26:
	addiu	r7,sp,0000000C
	addiu	r17,r17,FFFFFFFE
	swxs	r19,r18(r7)
	move	r16,r19
	bc	00409CF8

;; pntz: 00409D32
;;   Called from:
;;     00409DB2 (in trinkle)
;;     00409ECC (in qsort)
pntz proc
	lw	r5,0000(r4)
	li	r7,00000001
	li	r8,076BE629
	addiu	r6,r5,FFFFFFFF
	subu	r7,r7,r5
	and	r6,r6,r7
	addiupc	r5,000094D8
	mul	r6,r6,r8
	srl	r6,r6,0000001B
	lbux	r6,r6(r5)
	bnezc	r6,00409D70

l00409D54:
	lw	r6,0004(r4)
	subu	r7,r0,r6
	and	r7,r7,r6
	mul	r7,r7,r8
	srl	r7,r7,0000001B
	lbux	r6,r7(r5)
	addiu	r4,r6,00000020
	movz	r4,r0,r6

l00409D6E:
	move	r6,r4

l00409D70:
	move	r4,r6
	jrc	ra

;; trinkle: 00409D74
;;   Called from:
;;     00409E54 (in qsort)
;;     00409E9C (in qsort)
;;     00409F24 (in qsort)
;;     00409F42 (in qsort)
trinkle proc
	save	00000120,r30,0000000A
	movep	r17,r22,r4,r5
	move	r21,r6
	move	r18,r8
	lwm	r6,0000(r7),00000002
	move	r19,r9
	move	r23,r10
	addiu	r20,r0,00000001
	swm	r6,0004(sp),00000002
	sw	r17,000C(sp)
	bc	00409DC6

l00409D92:
	lwxs	r16,r18(r23)
	sll	r30,r18,00000002
	lw	r17,000C(sp)
	subu	r16,r17,r16
	move	r4,r16
	jalrc	ra,r21
	bgec	r0,r4,00409DD0

l00409DA6:
	beqzc	r19,00409DE6

l00409DA8:
	addiu	r7,sp,0000000C
	addiu	r4,sp,00000004
	swxs	r16,r20(r7)
	move	r19,r0
	balc	00409D32
	addiu	r20,r20,00000001
	move	r17,r4
	move	r5,r4
	addiu	r4,sp,00000004
	addu	r18,r18,r17
	balc	00409C4C
	move	r17,r16

l00409DC6:
	lw	r17,0004(sp)
	bneiuc	r7,00000001,00409D92

l00409DCC:
	lw	r17,0008(sp)
	bnezc	r7,00409D92

l00409DD0:
	bnezc	r19,00409DE2

l00409DD2:
	move	r6,r20
	addiu	r5,sp,0000000C
	move.balc	r4,r22,00409C78
	movep	r7,r8,r18,r23
	movep	r5,r6,r22,r21
	move.balc	r4,r17,00409CCA

l00409DE2:
	restore.jrc	00000120,r30,0000000A

l00409DE6:
	bltic	r18,00000002,00409DA8

l00409DEA:
	addu	r30,r23,r30
	subu	r4,r0,r22
	lw	r19,-0008(r30)
	move	r5,r16
	subu	r19,r4,r19
	addu	r4,r17,r4
	addu	r19,r17,r19
	jalrc	ra,r21
	bgec	r4,r0,00409DD2

l00409E04:
	movep	r4,r5,r19,r16
	jalrc	ra,r21
	bltc	r4,r0,00409DA8

l00409E0C:
	bc	00409DD2

;; qsort: 00409E0E
;;   Called from:
;;     00407362 (in __lookup_name)
qsort proc
	save	00000100,r30,0000000A
	movep	r17,r20,r6,r7
	mul	r5,r5,r17
	li	r7,00000001
	sw	r7,0008(sp)
	sw	r0,000C(sp)
	beqc	r0,r5,00409EEC

l00409E20:
	subu	r19,r5,r17
	addiu	r6,sp,00000010
	addu	r22,r4,r19
	move	r21,r6
	sw	r17,0010(sp)
	sw	r17,0014(sp)

l00409E2E:
	lw	r7,0000(r6)
	lw	r16,0004(r6)
	addu	r7,r7,r16
	addu	r7,r7,r17
	sw	r7,0008(sp)
	addiu	r6,r6,00000004
	bltuc	r7,r5,00409E2E

l00409E3E:
	subu	r23,r0,r17
	move	r18,r4
	li	r16,00000001

l00409E46:
	bltuc	r18,r22,00409E5A

l00409E4A:
	move	r10,r21
	move	r9,r0
	move	r8,r16
	addiu	r7,sp,00000008
	movep	r5,r6,r17,r20
	move.balc	r4,r18,00409D74
	bc	00409EDE

l00409E5A:
	lw	r17,0008(sp)
	andi	r7,r7,00000003
	bneiuc	r7,00000003,00409E80

l00409E62:
	movep	r7,r8,r16,r21
	movep	r5,r6,r17,r20
	addiu	r16,r16,00000002
	move.balc	r4,r18,00409CCA
	li	r5,00000002
	addiu	r4,sp,00000008
	balc	00409F4C

l00409E72:
	lw	r17,0008(sp)
	addu	r18,r18,r17
	subu	r19,r19,r17
	ori	r7,r7,00000001
	sw	r7,0008(sp)
	bc	00409E46

l00409E80:
	addiu	r30,r16,FFFFFFFF
	addiu	r7,sp,000000D0
	lsa	r7,r30,r7,00000002
	lw	r7,-00C0(r7)
	bltuc	r7,r19,00409EAE

l00409E92:
	move	r10,r21
	move	r9,r0
	move	r8,r16
	addiu	r7,sp,00000008
	movep	r5,r6,r17,r20
	move.balc	r4,r18,00409D74

l00409EA0:
	bneiuc	r16,00000001,00409EB8

l00409EA4:
	addiu	r4,sp,00000008
	move.balc	r5,r16,00409C20
	move	r16,r0
	bc	00409E72

l00409EAE:
	movep	r7,r8,r16,r21
	movep	r5,r6,r17,r20
	move.balc	r4,r18,00409CCA
	bc	00409EA0

l00409EB8:
	move	r5,r30
	addiu	r4,sp,00000008
	balc	00409F48
	li	r16,00000001
	bc	00409E72

l00409EC2:
	addu	r22,r18,r23
	bgeic	r16,00000002,00409EF0

l00409ECA:
	addiu	r4,sp,00000008
	balc	00409D32
	move	r19,r4
	move	r5,r4
	addiu	r4,sp,00000008
	addu	r19,r16,r19
	balc	00409F4C

l00409EDA:
	addu	r18,r18,r23
	move	r16,r19

l00409EDE:
	bneiuc	r16,00000001,00409EC2

l00409EE2:
	lw	r17,0008(sp)
	bneiuc	r7,00000001,00409ECA

l00409EE8:
	lw	r17,000C(sp)
	bnezc	r7,00409ECA

l00409EEC:
	restore.jrc	00000100,r30,0000000A

l00409EF0:
	addiu	r4,sp,00000008
	li	r5,00000002
	balc	00409F48
	lw	r17,0008(sp)
	addiu	r4,sp,00000008
	li	r5,00000001
	xori	r7,r7,00000007
	addiu	r19,r16,FFFFFFFE
	sw	r7,0008(sp)
	balc	00409F4C
	addiu	r7,sp,000000D0
	move	r10,r21
	lsa	r7,r19,r7,00000002
	addiu	r8,r16,FFFFFFFF
	movep	r5,r6,r17,r20
	lw	r4,-00C0(r7)
	addiu	r9,r0,00000001
	addiu	r7,sp,00000008
	addu	r4,r17,r4
	subu	r4,r18,r4
	balc	00409D74
	addiu	r4,sp,00000008
	li	r5,00000001
	balc	00409F48
	lw	r17,0008(sp)
	move	r10,r21
	addiu	r9,r0,00000001
	ori	r7,r7,00000001
	move	r8,r19
	sw	r7,0008(sp)
	movep	r5,r6,r17,r20
	addiu	r7,sp,00000008
	move.balc	r4,r22,00409D74
	bc	00409EDA

;; fn00409F48: 00409F48
;;   Called from:
;;     00409EBC (in qsort)
;;     00409EF4 (in qsort)
;;     00409F2C (in qsort)
fn00409F48 proc
	bc	00409C20

;; fn00409F4C: 00409F4C
;;   Called from:
;;     00409E70 (in qsort)
;;     00409ED8 (in qsort)
;;     00409F06 (in qsort)
fn00409F4C proc
	bc	00409C4C

;; strtox: 00409F50
;;   Called from:
;;     00409FA8 (in strtof_l)
;;     00409FB4 (in strtod_l)
;;     00409FBA (in strtold_l)
strtox proc
	save	000000A0,ra,00000004
	movep	r16,r17,r4,r5
	move	r18,r6
	move	r5,r0
	addiu	r6,r0,00000090
	move	r4,sp
	balc	0040A690
	li	r7,FFFFFFFF
	move	r4,sp
	sw	r7,0008(sp)
	sw	r7,004C(sp)
	movep	r6,r7,r0,r0
	sw	r16,0004(sp)
	sw	r16,002C(sp)
	balc	0040CB40
	li	r6,00000001
	move	r4,sp
	move.balc	r5,r18,0040BD5C
	lw	r17,0008(sp)
	lw	r17,0004(sp)
	movep	r8,r9,r4,r5
	subu	r7,r7,r6
	lw	r17,0078(sp)
	lw	r17,007C(sp)
	sra	r5,r7,0000001F
	addu	r6,r7,r6
	addu	r5,r5,r4
	sltu	r7,r6,r7
	addu	r7,r7,r5
	beqzc	r17,00409FA0

l00409F98:
	or	r7,r7,r6
	beqzc	r7,00409F9E

l00409F9C:
	addu	r16,r16,r6

l00409F9E:
	sw	r16,0000(r17)

l00409FA0:
	movep	r4,r5,r8,r9
	restore.jrc	000000A0,ra,00000004

;; strtof_l: 00409FA4
strtof_l proc
	save	00000010,ra,00000001
	move	r6,r0
	balc	00409F50
	balc	00410220
	restore.jrc	00000010,ra,00000001

;; strtod_l: 00409FB2
strtod_l proc
	li	r6,00000001
	bc	00409F50

;; strtold_l: 00409FB8
strtold_l proc
	li	r6,00000002
	bc	00409F50
00409FBE                                           00 00               ..

;; strtox: 00409FC0
;;   Called from:
;;     0040A014 (in __strtoull_internal)
;;     0040A01E (in __strtoll_internal)
;;     0040A02A (in __strtoul_internal)
;;     0040A038 (in __strtol_internal)
strtox proc
	save	000000B0,ra,00000004
	movep	r16,r17,r4,r5
	move	r18,r6
	sw	r0,0010(sp)
	sw	r16,0014(sp)
	li	r7,FFFFFFFF
	sw	r16,003C(sp)
	bltc	r16,r0,00409FDA

l00409FD2:
	move	r7,r16
	addiu	r7,r7,7FFFFFFF

l00409FDA:
	sw	r7,0018(sp)
	li	r7,FFFFFFFF
	addiu	r4,sp,00000010
	sw	r7,005C(sp)
	movep	r6,r7,r0,r0
	swm	r8,0008(sp),00000002
	balc	0040CB40
	li	r6,00000001
	addiu	r4,sp,00000010
	lwm	r8,0008(sp),00000002
	move.balc	r5,r18,0040C670
	beqzc	r17,0040A00A

l00409FFA:
	lw	r17,0018(sp)
	lw	r17,0014(sp)
	subu	r7,r7,r6
	lw	r6,0088(sp)
	addu	r7,r7,r6
	addu	r16,r16,r7
	sw	r16,0000(r17)

l0040A00A:
	restore.jrc	000000B0,ra,00000004

;; __strtoull_internal: 0040A00C
;;   Called from:
;;     00406BD0 (in __lookup_ipliteral)
;;     0040A042 (in __strtoumax_internal)
__strtoull_internal proc
	addiu	r8,r0,FFFFFFFF
	addiu	r9,r0,FFFFFFFF
	bc	00409FC0

;; __strtoll_internal: 0040A018
;;   Called from:
;;     0040A03E (in __strtoimax_internal)
__strtoll_internal proc
	move	r8,r0
	lui	r9,FFF80000
	bc	00409FC0

;; __strtoul_internal: 0040A022
;;   Called from:
;;     00403456 (in hextoui)
;;     0040666E (in getnameinfo)
;;     004067B2 (in __inet_aton)
;;     004073C8 (in __lookup_serv)
;;     004074EA (in __lookup_serv)
;;     00407BDE (in __get_resolv_conf)
;;     00407C14 (in __get_resolv_conf)
;;     00407C4C (in __get_resolv_conf)
__strtoul_internal proc
	save	00000010,ra,00000001
	addiu	r8,r0,FFFFFFFF
	move	r9,r0
	balc	00409FC0
	restore.jrc	00000010,ra,00000001

;; __strtol_internal: 0040A030
__strtol_internal proc
	save	00000010,ra,00000001
	lui	r8,FFF80000
	move	r9,r0
	balc	00409FC0
	restore.jrc	00000010,ra,00000001

;; __strtoimax_internal: 0040A03E
__strtoimax_internal proc
	bc	0040A018

;; __strtoumax_internal: 0040A042
__strtoumax_internal proc
	bc	0040A00C
0040A046                   00 00 00 00 00 00 00 00 00 00       ..........

;; memchr: 0040A050
;;   Called from:
;;     0040366C (in ping6_run)
;;     004082C8 (in fgets_unlocked)
;;     0040A946 (in strnlen)
;;     0040AB74 (in twoway_strstr)
;;     0040D370 (in __string_read)
memchr proc
	andi	r7,r4,00000003
	andi	r5,r5,000000FF
	beqzc	r7,0040A072

l0040A056:
	beqc	r0,r6,0040A0F8

l0040A05A:
	lbu	r7,0000(r4)
	bnec	r5,r7,0040A06A

l0040A05E:
	bc	0040A0FC

l0040A060:
	beqc	r0,r6,0040A0F8

l0040A064:
	lbu	r7,0000(r4)
	beqc	r5,r7,0040A0FC

l0040A06A:
	addiu	r4,r4,00000001
	addiu	r6,r6,FFFFFFFF
	andi	r7,r4,00000003
	bnezc	r7,0040A060

l0040A072:
	beqc	r0,r6,0040A0F8

l0040A076:
	lbu	r7,0000(r4)
	beqc	r5,r7,0040A0FC

l0040A07C:
	sll	r9,r5,00000008
	addu	r9,r9,r5
	sll	r7,r9,00000010
	addu	r9,r9,r7
	bltiuc	r6,00000004,0040A0E0

l0040A08C:
	lw	r7,0000(r4)
	xor	r7,r9,r7
	nor	r8,r0,r7
	addiu	r7,r7,FEFEFEFF
	and	r7,r8,r7
	li	r8,80808080
	and	r7,r7,r8
	bnezc	r7,0040A0E0

l0040A0AC:
	move	r8,r4
	bc	0040A0CE

l0040A0B0:
	lw	r7,0000(r8)
	xor	r7,r9,r7
	move	r4,r7
	nor	r7,r0,r7
	addiu	r4,r4,FEFEFEFF
	and	r7,r7,r4
	li	r4,80808080
	and	r7,r7,r4
	bnezc	r7,0040A0D8

l0040A0CE:
	addiu	r6,r6,FFFFFFFC
	addiu	r8,r8,00000004
	bgeiuc	r6,00000004,0040A0B0

l0040A0D6:
	beqzc	r6,0040A0F8

l0040A0D8:
	lbu	r7,0000(r8)
	move	r4,r8
	beqc	r5,r7,0040A0FC

l0040A0E0:
	addiu	r7,r4,00000001
	addu	r6,r4,r6
	bc	0040A0F2

l0040A0E8:
	lbu	r8,0000(r7)
	addiu	r7,r7,00000001
	beqc	r5,r8,0040A0FC

l0040A0F2:
	move	r4,r7
	bnec	r7,r6,0040A0E8

l0040A0F8:
	move	r4,r0
	jrc	ra

l0040A0FC:
	jrc	ra
0040A0FE                                           00 00               ..

;; memcmp: 0040A100
;;   Called from:
;;     00400EB8 (in ping4_run)
;;     004012BA (in pr_addr)
;;     0040144A (in pr_options)
;;     00403002 (in ping6_receive_error_msg)
;;     00403184 (in ping6_parse_reply)
;;     00406400 (in getnameinfo)
;;     004064F4 (in getnameinfo)
;;     004068A6 (in inet_ntop)
;;     00406DC4 (in policyof)
;;     00407A2E (in __res_msend_rc)
;;     0040AAFA (in twoway_strstr)
memcmp proc
	beqzc	r6,0040A12A

l0040A102:
	lbu	r8,0000(r4)
	lbu	r9,0000(r5)
	bnec	r9,r8,0040A124

l0040A10E:
	li	r7,00000001

l0040A110:
	beqc	r6,r7,0040A12A

l0040A112:
	lbux	r8,r7(r4)
	addiu	r7,r7,00000001
	addu	r9,r5,r7
	lbu	r9,-0001(r9)
	beqc	r8,r9,0040A110

l0040A124:
	subu	r4,r8,r9
	jrc	ra

l0040A12A:
	move	r4,r0
	jrc	ra
0040A12E                                           00 00               ..

;; memcpy: 0040A130
;;   Called from:
;;     00400A88 (in ping4_send_probe)
;;     00400B52 (in fn00400B52)
;;     00401608 (in fn00401608)
;;     00402472 (in gather_statistics)
;;     004034E2 (in fn004034E2)
;;     00403650 (in ping6_run)
;;     00403BAC (in ping6_run)
;;     004058F4 (in realloc)
;;     0040590C (in realloc)
;;     00405EBA (in getaddrinfo)
;;     00405F74 (in getaddrinfo)
;;     00405FDA (in fn00405FD4)
;;     00406104 (in netlink_msg_to_ifaddr)
;;     00406280 (in netlink_msg_to_ifaddr)
;;     0040628E (in fn0040628E)
;;     00406380 (in getnameinfo)
;;     00406446 (in getnameinfo)
;;     00406698 (in getnameinfo)
;;     004066AA (in fn004066AA)
;;     00406C24 (in fn00406C24)
;;     00406F2A (in name_from_hosts)
;;     00406F80 (in dns_parse_callback)
;;     00407134 (in __lookup_name)
;;     004071FC (in __lookup_name)
;;     00407228 (in __lookup_name)
;;     0040738E (in fn0040738E)
;;     0040770E (in res_mkquery)
;;     00407774 (in res_mkquery)
;;     0040785E (in __res_msend_rc)
;;     0040787E (in __res_msend_rc)
;;     0040793E (in __res_msend_rc)
;;     0040794A (in __res_msend_rc)
;;     00407A6E (in __res_msend_rc)
;;     00407CE6 (in __get_resolv_conf)
;;     00407F36 (in __get_handler_set)
;;     00407FCC (in __libc_sigaction)
;;     00408000 (in __libc_sigaction)
;;     004082E6 (in fgets_unlocked)
;;     0040856E (in __fwritex)
;;     004099FC (in vfprintf)
;;     00409A12 (in vfprintf)
;;     00409AD0 (in sn_write)
;;     00409AF2 (in sn_write)
;;     00409B22 (in vsnprintf)
;;     00409B7A (in vsprintf)
;;     00409B9E (in __isoc99_vsscanf)
;;     00409CAA (in cycle)
;;     00409CB4 (in cycle)
;;     0040A616 (in memmove)
;;     0040A888 (in __strdup)
;;     0040B13C (in __copy_tls)
;;     0040D384 (in __string_read)
;;     0040D440 (in arg_n)
;;     0040D44C (in arg_n)
;;     0040D4B2 (in __isoc99_vfscanf)
memcpy proc
	andi	r7,r5,00000003
	beqc	r0,r7,0040A4F2

l0040A136:
	beqc	r0,r6,0040A1EE

l0040A13A:
	move	r13,r4
	bc	0040A142

l0040A13E:
	beqc	r0,r6,0040A286

l0040A142:
	addiu	r5,r5,00000001
	addiu	r13,r13,00000001
	lbu	r8,-0001(r5)
	andi	r7,r5,00000003
	addiu	r6,r6,FFFFFFFF
	sb	r8,-0001(r13)
	bnezc	r7,0040A13E

l0040A154:
	save	00000010,r16,00000002
	andi	r8,r13,00000003
	bnec	r0,r8,0040A1F0

l0040A160:
	bltiuc	r6,00000010,0040A506

l0040A164:
	addiu	r12,r6,FFFFFFF0
	move	r8,r5
	move	r3,r12
	move	r7,r13
	ins	r3,r0,00000000,00000001
	addiu	r3,r3,00000010
	addu	r3,r13,r3

l0040A17A:
	lw	r2,0000(r8)
	lw	r11,0001(r8)
	lw	r10,0002(r8)
	lw	r9,0003(r8)
	addiu	r8,r8,00000010
	sw	r2,0000(r7)
	sw	r11,0004(r7)
	sw	r10,0002(r7)
	sw	r9,0003(r7)
	addiu	r7,r7,00000010
	bnec	r7,r3,0040A17A

l0040A19A:
	move	r7,r12
	andi	r17,r6,00000008
	ins	r7,r0,00000000,00000001
	andi	r16,r6,00000004
	addiu	r7,r7,00000010
	addu	r5,r5,r7
	addu	r13,r13,r7
	andi	r7,r6,00000002
	andi	r6,r6,00000001

l0040A1B0:
	beqzc	r17,0040A1C4

l0040A1B2:
	lw	r8,0000(r5)
	lw	r17,0004(r5)
	addiu	r5,r5,00000008
	sw	r8,0000(r13)
	sw	r17,0004(r13)
	addiu	r13,r13,00000008

l0040A1C4:
	beqzc	r16,0040A1D0

l0040A1C6:
	lw	r16,0000(r5)
	addiu	r5,r5,00000004
	sw	r16,0000(r13)
	addiu	r13,r13,00000004

l0040A1D0:
	beqzc	r7,0040A1E2

l0040A1D2:
	lbu	r16,0000(r5)
	lbu	r7,0001(r5)
	addiu	r5,r5,00000002
	sb	r16,0000(r13)
	sb	r7,0001(r13)
	addiu	r13,r13,00000002

l0040A1E2:
	beqzc	r6,0040A1EA

l0040A1E4:
	lbu	r7,0000(r5)
	sb	r7,0000(r13)

l0040A1EA:
	restore	00000010,r16,00000002

l0040A1EE:
	jrc	ra

l0040A1F0:
	bltiuc	r6,00000020,0040A322

l0040A1F4:
	lbu	r9,0000(r5)
	lw	r7,0000(r5)
	beqic	r8,00000002,0040A288

l0040A1FE:
	bneiuc	r8,00000003,0040A44A

l0040A202:
	addiu	r16,r6,FFFFFFEC
	addiu	r24,r13,00000001
	srl	r17,r16,00000004
	addiu	r5,r5,00000001
	sll	r25,r17,00000004
	move	r2,r5
	addiu	r25,r25,00000011
	move	r8,r24
	addu	r25,r13,r25
	sb	r9,0000(r13)

l0040A222:
	lw	r11,0003(r2)
	srl	r3,r7,00000008
	lw	r10,0007(r2)
	lw	r9,000B(r2)
	sll	r15,r11,00000018
	lw	r7,000F(r2)
	sll	r14,r10,00000018
	sll	r13,r9,00000018
	srl	r11,r11,00000008
	srl	r10,r10,00000008
	srl	r9,r9,00000008
	sll	r12,r7,00000018
	or	r3,r3,r15
	or	r11,r11,r14
	or	r10,r10,r13
	or	r9,r9,r12
	sw	r3,0000(r8)
	sw	r11,0004(r8)
	addiu	r2,r2,00000010
	sw	r10,0002(r8)
	sw	r9,0003(r8)
	addiu	r8,r8,00000010
	bnec	r8,r25,0040A222

l0040A27A:
	ins	r16,r0,00000000,00000001
	addiu	r6,r6,FFFFFFEF
	move	r13,r16
	bc	0040A312

l0040A286:
	jrc	ra

l0040A288:
	addiu	r16,r6,FFFFFFEC
	lbu	r8,0001(r5)
	srl	r17,r16,00000004
	addiu	r24,r13,00000002
	addiu	r25,r17,00000001
	addiu	r5,r5,00000002
	sll	r25,r25,00000004
	sb	r8,0001(r13)
	addu	r25,r24,r25
	move	r2,r5
	sb	r9,0000(r13)
	move	r8,r24

l0040A2B0:
	lw	r11,0002(r2)
	srl	r3,r7,00000010
	lw	r10,0006(r2)
	lw	r9,000A(r2)
	sll	r15,r11,00000010
	lw	r7,000E(r2)
	sll	r14,r10,00000010
	sll	r13,r9,00000010
	srl	r11,r11,00000010
	srl	r10,r10,00000010
	srl	r9,r9,00000010
	sll	r12,r7,00000010
	or	r3,r3,r15
	or	r11,r11,r14
	or	r10,r10,r13
	or	r9,r9,r12
	sw	r3,0000(r8)
	sw	r11,0004(r8)
	addiu	r2,r2,00000010
	sw	r10,0002(r8)
	sw	r9,0003(r8)
	addiu	r8,r8,00000010
	bnec	r8,r25,0040A2B0

l0040A308:
	ins	r16,r0,00000000,00000001
	addiu	r6,r6,FFFFFFEE
	move	r13,r16

l0040A312:
	addiu	r13,r13,00000010
	sll	r17,r17,00000004
	addu	r5,r5,r13
	subu	r6,r6,r17
	addu	r13,r24,r13

l0040A322:
	andi	r8,r6,00000008
	andi	r10,r6,00000004
	andi	r17,r6,00000002
	andi	r16,r6,00000001
	bbeqzc	r6,00000004,0040A4F6

l0040A332:
	lbu	r9,0002(r5)
	lbu	r6,0001(r5)
	sb	r9,0002(r13)
	lbu	r9,0003(r5)
	lbu	r7,0000(r5)
	sb	r9,0003(r13)
	lbu	r9,0004(r5)
	lbu	r25,0006(r5)
	sb	r9,0004(r13)
	lbu	r9,0005(r5)
	lbu	r24,0007(r5)
	lbu	r15,0008(r5)
	lbu	r14,0009(r5)
	lbu	r12,000A(r5)
	lbu	r3,000B(r5)
	lbu	r2,000C(r5)
	lbu	r11,000D(r5)
	sb	r6,0001(r13)
	addiu	r6,r5,00000010
	sb	r9,0005(r13)
	lbu	r9,000E(r5)
	lbu	r5,000F(r5)
	sb	r7,0000(r13)
	addiu	r7,r13,00000010
	sb	r25,0006(r13)
	sb	r24,0007(r13)
	sb	r15,0008(r13)
	sb	r14,0009(r13)
	sb	r12,000A(r13)
	sb	r3,000B(r13)
	sb	r2,000C(r13)
	sb	r11,000D(r13)
	sb	r9,000E(r13)
	sb	r5,000F(r13)

l0040A3B4:
	beqc	r0,r8,0040A4FC

l0040A3B8:
	lbu	r14,0000(r6)
	addiu	r9,r6,00000008
	lbu	r13,0001(r6)
	addiu	r8,r7,00000008
	lbu	r12,0002(r6)
	lbu	r3,0003(r6)
	lbu	r2,0004(r6)
	lbu	r11,0005(r6)
	lbu	r5,0006(r6)
	lbu	r6,0007(r6)
	sb	r14,0000(r7)
	sb	r13,0001(r7)
	sb	r12,0002(r7)
	sb	r3,0003(r7)
	sb	r2,0004(r7)
	sb	r11,0005(r7)
	sb	r5,0006(r7)
	sb	r6,0007(r7)

l0040A400:
	beqc	r0,r10,0040A500

l0040A404:
	lbu	r11,0000(r9)
	addiu	r13,r8,00000004
	lbu	r10,0001(r9)
	addiu	r5,r9,00000004
	lbu	r6,0002(r9)
	lbu	r7,0003(r9)
	sb	r11,0000(r8)
	sb	r10,0001(r8)
	sb	r6,0002(r8)
	sb	r7,0003(r8)

l0040A42C:
	beqzc	r17,0040A43E

l0040A42E:
	lbu	r6,0000(r5)
	lbu	r7,0001(r5)
	addiu	r5,r5,00000002
	sb	r6,0000(r13)
	sb	r7,0001(r13)
	addiu	r13,r13,00000002

l0040A43E:
	beqc	r0,r16,0040A1EA

l0040A442:
	lbu	r7,0000(r5)
	sb	r7,0000(r13)
	bc	0040A1EA

l0040A44A:
	addiu	r24,r6,FFFFFFEC
	lbu	r8,0001(r5)
	srl	r25,r24,00000004
	lbu	r17,0002(r5)
	addiu	r15,r25,00000001
	addiu	r16,r13,00000003
	addiu	r5,r5,00000003
	sll	r15,r15,00000004
	sb	r8,0001(r13)
	addu	r15,r16,r15
	move	r2,r5
	sb	r9,0000(r13)
	sb	r17,0002(r13)
	move	r8,r16

l0040A47A:
	lw	r11,0001(r2)
	srl	r3,r7,00000018
	lw	r10,0005(r2)
	lw	r9,0009(r2)
	sll	r17,r11,00000008
	lw	r7,000D(r2)
	sll	r14,r10,00000008
	sll	r13,r9,00000008
	srl	r11,r11,00000018
	srl	r10,r10,00000018
	srl	r9,r9,00000018
	sll	r12,r7,00000008
	or	r3,r3,r17
	or	r11,r11,r14
	or	r10,r10,r13
	or	r9,r9,r12
	sw	r3,0000(r8)
	sw	r11,0004(r8)
	addiu	r2,r2,00000010
	sw	r10,0002(r8)
	sw	r9,0003(r8)
	addiu	r8,r8,00000010
	bnec	r8,r15,0040A47A

l0040A4D2:
	move	r13,r24
	addiu	r6,r6,FFFFFFED
	ins	r13,r0,00000000,00000001
	sll	r25,r25,00000004
	addiu	r13,r13,00000010
	subu	r6,r6,r25
	addu	r5,r5,r13
	addu	r13,r16,r13
	bc	0040A322

l0040A4F2:
	move	r13,r4
	bc	0040A154

l0040A4F6:
	move	r6,r5
	move	r7,r13
	bc	0040A3B4

l0040A4FC:
	movep	r9,r8,r6,r7
	bc	0040A400

l0040A500:
	move	r5,r9
	move	r13,r8
	bc	0040A42C

l0040A506:
	andi	r17,r6,00000008
	andi	r16,r6,00000004
	andi	r7,r6,00000002
	andi	r6,r6,00000001
	bc	0040A1B0

;; memmove: 0040A510
;;   Called from:
;;     004069AA (in inet_ntop)
;;     00406AF2 (in inet_pton)
memmove proc
	beqc	r4,r5,0040A688

l0040A514:
	addu	r7,r5,r6
	bgeuc	r4,r7,0040A616

l0040A51A:
	addu	r7,r4,r6
	bgeuc	r5,r7,0040A616

l0040A520:
	xor	r8,r5,r4
	andi	r8,r8,00000003
	bgeuc	r4,r5,0040A61A

l0040A52C:
	bnec	r0,r8,0040A666

l0040A530:
	andi	r7,r4,00000003
	beqc	r0,r7,0040A67C

l0040A536:
	addiu	r8,r6,FFFFFFFF
	move	r7,r4
	bnezc	r6,0040A546

l0040A53E:
	bc	0040A688

l0040A540:
	addiu	r8,r8,FFFFFFFF
	beqc	r8,r10,0040A688

l0040A546:
	addiu	r5,r5,00000001
	addiu	r7,r7,00000001
	lbu	r9,-0001(r5)
	andi	r6,r7,00000003
	addiu	r10,r0,FFFFFFFF
	sb	r9,-0001(r7)
	bnezc	r6,0040A540

l0040A55A:
	bltiuc	r8,00000004,0040A684

l0040A55E:
	addiu	r11,r8,FFFFFFFC
	move	r6,r0
	move	r10,r11
	ins	r10,r0,00000000,00000001
	addiu	r10,r10,00000004

l0040A56C:
	lwx	r9,r6(r5)
	swx	r9,r6(r7)
	addiu	r6,r6,00000004
	bnec	r6,r10,0040A56C

l0040A57A:
	ins	r11,r0,00000000,00000001
	andi	r6,r8,00000003
	addiu	r8,r11,00000004
	addu	r7,r7,r8
	addu	r5,r5,r8

l0040A58A:
	beqc	r0,r6,0040A688

l0040A58E:
	addiu	r8,r5,00000004
	addiu	r10,r7,00000004
	sltu	r8,r7,r8
	sltu	r10,r5,r10
	xori	r8,r8,00000001
	xori	r10,r10,00000001
	sltiu	r9,r6,0000000A
	or	r8,r8,r10
	xori	r9,r9,00000001
	and	r8,r9,r8
	beqc	r0,r8,0040A66A

l0040A5BA:
	or	r8,r5,r7
	andi	r8,r8,00000003
	bnec	r0,r8,0040A66A

l0040A5C6:
	addiu	r8,r6,FFFFFFFC
	move	r9,r0
	srl	r8,r8,00000002
	addiu	r8,r8,00000001
	sll	r10,r8,00000002

l0040A5D6:
	lwxs	r11,r9(r5)
	swxs	r11,r9(r7)
	addiu	r9,r9,00000001
	bltuc	r9,r8,0040A5D6

l0040A5E4:
	subu	r8,r6,r10
	addu	r9,r7,r10
	addu	r11,r5,r10
	beqc	r6,r10,0040A688

l0040A5F4:
	lbux	r6,r10(r5)
	sbx	r6,r7(r10)
	beqic	r8,00000001,0040A688

l0040A600:
	lbu	r7,0001(r11)
	sb	r7,0001(r9)
	beqic	r8,00000002,0040A688

l0040A60C:
	lbu	r7,0002(r11)
	sb	r7,0002(r9)
	jrc	ra

l0040A616:
	bc	0040A130

l0040A61A:
	bnec	r0,r8,0040A658

l0040A61E:
	andi	r7,r7,00000003
	beqzc	r7,0040A680

l0040A622:
	addiu	r7,r6,FFFFFFFF
	bnezc	r6,0040A630

l0040A628:
	bc	0040A688

l0040A62A:
	addiu	r7,r7,FFFFFFFF
	beqc	r7,r9,0040A688

l0040A630:
	lbux	r8,r7(r5)
	addu	r6,r4,r7
	andi	r6,r6,00000003
	addiu	r9,r0,FFFFFFFF
	sbx	r8,r4(r7)
	bnezc	r6,0040A62A

l0040A642:
	move	r6,r7
	bltiuc	r7,00000004,0040A658

l0040A648:
	addiu	r6,r6,FFFFFFFC
	lwx	r8,r6(r5)
	swx	r8,r6(r4)
	bgeiuc	r6,00000004,0040A648

l0040A656:
	andi	r6,r7,00000003

l0040A658:
	beqzc	r6,0040A688

l0040A65A:
	addiu	r6,r6,FFFFFFFF
	lbux	r7,r6(r5)
	sbx	r7,r4(r6)
	bc	0040A658

l0040A666:
	move	r7,r4
	bc	0040A58A

l0040A66A:
	move	r8,r0

l0040A66C:
	lbux	r9,r8(r5)
	sbx	r9,r7(r8)
	addiu	r8,r8,00000001
	bnec	r6,r8,0040A66C

l0040A67A:
	jrc	ra

l0040A67C:
	movep	r7,r8,r4,r6
	bc	0040A55A

l0040A680:
	move	r7,r6
	bc	0040A642

l0040A684:
	move	r6,r8
	bc	0040A58A

l0040A688:
	jrc	ra
0040A68A                               00 00 00 00 00 00           ......

;; memset: 0040A690
;;   Called from:
;;     0040017E (in main)
;;     00400B4E (in fn00400B4E)
;;     00401366 (in fn00401366)
;;     00401C88 (in set_signal)
;;     0040330C (in niquery_option_subject_addr_handler)
;;     0040385A (in ping6_run)
;;     00403A4C (in ping6_run)
;;     00403B82 (in ping6_run)
;;     004047B0 (in sysconf)
;;     00404880 (in __init_libc)
;;     00405F08 (in getaddrinfo)
;;     004061B6 (in netlink_msg_to_ifaddr)
;;     0040625A (in netlink_msg_to_ifaddr)
;;     004062B0 (in getifaddrs)
;;     00407566 (in __netlink_enumerate)
;;     004076F8 (in res_mkquery)
;;     004077DA (in __res_msend_rc)
;;     004078B6 (in __res_msend_rc)
;;     0040807C (in __fopen_rb_ca)
;;     00408B4C (in pad)
;;     00409A08 (in vfprintf)
;;     00409B3A (in vsnprintf)
;;     00409BAA (in __isoc99_vsscanf)
;;     00409F5E (in strtox)
;;     0040D7E2 (in __isoc99_vfscanf)
;;     0040D8B6 (in __isoc99_vfscanf)
;;     0040DD1C (in stpncpy)
;;     0040E008 (in fn0040E008)
memset proc
	beqc	r0,r6,0040A74C

l0040A694:
	andi	r7,r5,000000FF
	addu	r8,r4,r6
	sb	r7,-0001(r8)
	sb	r7,0000(r4)
	bltiuc	r6,00000003,0040A74C

l0040A6A4:
	sb	r7,-0002(r8)
	sb	r7,0001(r4)
	sb	r7,-0003(r8)
	sb	r7,0002(r4)
	bltiuc	r6,00000007,0040A74C

l0040A6B4:
	sb	r7,-0004(r8)
	sb	r7,0003(r4)
	bltiuc	r6,00000009,0040A74C

l0040A6BE:
	subu	r9,r0,r4
	andi	r7,r5,000000FF
	andi	r9,r9,00000003
	sll	r5,r7,00000008
	subu	r6,r6,r9
	addu	r5,r5,r7
	ins	r6,r0,00000000,00000001
	addu	r7,r4,r9
	move	r8,r6
	sll	r6,r5,00000010
	addu	r6,r5,r6
	addu	r10,r7,r8
	swx	r6,r9(r4)
	sw	r6,-0004(r10)
	bltiuc	r8,00000009,0040A74C

l0040A6F0:
	sw	r6,0044(sp)
	sw	r6,0048(sp)
	sw	r6,-000C(r10)
	sw	r6,-0008(r10)
	bltiuc	r8,00000019,0040A74C

l0040A700:
	andi	r5,r7,00000004
	sw	r6,004C(sp)
	addiu	r5,r5,00000018
	sw	r6,0050(sp)
	subu	r8,r8,r5
	sw	r6,0054(sp)
	sw	r6,0058(sp)
	addu	r7,r7,r5
	sw	r6,-001C(r10)
	sw	r6,-0018(r10)
	sw	r6,-0014(r10)
	sw	r6,-0010(r10)
	bltiuc	r8,00000020,0040A74C

l0040A726:
	addiu	r5,r8,FFFFFFE0
	ins	r5,r0,00000000,00000001
	addiu	r5,r5,00000020
	addu	r5,r7,r5

l0040A734:
	sw	r6,0040(sp)
	sw	r6,0044(sp)
	sw	r6,0048(sp)
	sw	r6,004C(sp)
	sw	r6,0050(sp)
	sw	r6,0054(sp)
	sw	r6,0058(sp)
	sw	r6,005C(sp)
	addiu	r7,r7,00000020
	bnec	r7,r5,0040A734

l0040A74C:
	jrc	ra
0040A74E                                           00 00               ..

;; strcat: 0040A750
;;   Called from:
;;     0040658C (in getnameinfo)
strcat proc
	save	00000020,ra,00000002
	move	r16,r4
	sw	r5,000C(sp)
	balc	0040A890
	lw	r17,000C(sp)
	addu	r4,r16,r4
	balc	0040A860
	move	r4,r16
	restore.jrc	00000020,ra,00000002
0040A766                   00 00 00 00 00 00 00 00 00 00       ..........

;; strchr: 0040A770
;;   Called from:
;;     004063AA (in getnameinfo)
;;     00406646 (in getnameinfo)
;;     00406B80 (in __lookup_ipliteral)
;;     00406EAA (in name_from_hosts)
;;     0040747A (in __lookup_serv)
;;     00407B72 (in __get_resolv_conf)
;;     0040AC32 (in strstr)
strchr proc
	save	00000010,ra,00000002
	move	r16,r5
	balc	0040A790
	andi	r16,r16,000000FF
	lbu	r7,0000(r4)
	xor	r7,r7,r16
	movn	r4,r0,r7

l0040A782:
	restore.jrc	00000010,ra,00000002
0040A784             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; strchrnul: 0040A790
;;   Called from:
;;     0040A774 (in strchr)
strchrnul proc
	save	00000010,ra,00000002
	andi	r5,r5,000000FF
	move	r16,r4
	bnezc	r5,0040A7A6

l0040A798:
	bc	0040A824

l0040A79A:
	lbu	r7,0000(r16)
	beqc	r0,r7,0040A820

l0040A7A0:
	beqc	r5,r7,0040A820

l0040A7A4:
	addiu	r16,r16,00000001

l0040A7A6:
	andi	r7,r16,00000003
	bnezc	r7,0040A79A

l0040A7AA:
	sll	r10,r5,00000008
	lw	r7,0000(r16)
	addu	r10,r10,r5
	li	r4,FEFEFEFF
	sll	r8,r10,00000010
	addu	r6,r7,r4
	addu	r10,r10,r8
	nor	r8,r0,r7
	xor	r7,r10,r7
	and	r6,r6,r8
	addu	r4,r7,r4
	nor	r7,r0,r7
	and	r7,r7,r4
	or	r7,r7,r6
	li	r6,80808080
	and	r7,r7,r6
	bnezc	r7,0040A814

l0040A7E0:
	move	r4,r16

l0040A7E2:
	addiu	r4,r4,00000004
	li	r6,FEFEFEFF
	lw	r8,0000(r4)
	xor	r9,r10,r8
	addu	r7,r8,r6
	addu	r6,r6,r9
	nor	r8,r0,r8
	nor	r9,r0,r9
	and	r7,r7,r8
	and	r6,r6,r9
	or	r7,r7,r6
	li	r6,80808080
	and	r7,r7,r6
	beqzc	r7,0040A7E2

l0040A812:
	move	r16,r4

l0040A814:
	lbu	r7,0000(r16)
	beqzc	r7,0040A820

l0040A818:
	beqc	r5,r7,0040A820

l0040A81A:
	addiu	r16,r16,00000001
	lbu	r7,0000(r16)
	bnezc	r7,0040A818

l0040A820:
	move	r4,r16
	restore.jrc	00000010,ra,00000002

l0040A824:
	balc	0040A890
	addu	r4,r16,r4
	restore.jrc	00000010,ra,00000002
0040A82C                                     00 00 00 00             ....

;; strcmp: 0040A830
;;   Called from:
;;     0040CD74 (in __vdsosym)
;;     0040CDAE (in __vdsosym)
strcmp proc
	lbu	r7,0000(r4)
	lbu	r6,0000(r5)
	bnec	r7,r6,0040A858

l0040A838:
	beqzc	r6,0040A854

l0040A83A:
	li	r7,00000001
	bc	0040A840

l0040A83E:
	beqzc	r6,0040A854

l0040A840:
	lbux	r6,r7(r4)
	lbux	r8,r7(r5)
	addiu	r7,r7,00000001
	beqc	r6,r8,0040A83E

l0040A84E:
	subu	r4,r6,r8
	jrc	ra

l0040A854:
	move	r4,r0
	jrc	ra

l0040A858:
	subu	r4,r7,r6
	jrc	ra
0040A85C                                     00 00 00 00             ....

;; strcpy: 0040A860
;;   Called from:
;;     0040652E (in getnameinfo)
;;     004065BA (in getnameinfo)
;;     0040662A (in getnameinfo)
;;     004069B8 (in inet_ntop)
;;     00406FC8 (in dns_parse_callback)
;;     0040A75E (in strcat)
strcpy proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	0040DC10
	move	r4,r16
	restore.jrc	00000010,ra,00000002
0040A86C                                     00 00 00 00             ....

;; __strdup: 0040A870
__strdup proc
	save	00000010,ra,00000003
	move	r17,r4
	balc	0040A890
	addiu	r16,r4,00000001
	move.balc	r4,r16,00405292
	beqzc	r4,0040A88C

l0040A882:
	movep	r5,r6,r17,r16
	restore	00000010,ra,00000003
	bc	0040A130

l0040A88C:
	restore.jrc	00000010,ra,00000003
0040A88E                                           00 00               ..

;; strlen: 0040A890
;;   Called from:
;;     004001A0 (in main)
;;     00400CC0 (in ping4_run)
;;     00403664 (in ping6_run)
;;     00403774 (in ping6_run)
;;     00405960 (in __getopt_msg)
;;     00405E90 (in getaddrinfo)
;;     00406594 (in getnameinfo)
;;     0040661E (in getnameinfo)
;;     004069AE (in inet_ntop)
;;     00406E3E (in name_from_hosts)
;;     0040743C (in __lookup_serv)
;;     00407CD8 (in __get_resolv_conf)
;;     004083F4 (in fputs_unlocked)
;;     0040865A (in perror)
;;     00408674 (in perror)
;;     00408698 (in perror)
;;     0040A756 (in strcat)
;;     0040A824 (in strchrnul)
;;     0040A874 (in __strdup)
;;     0040E46E (in mbsrtowcs)
strlen proc
	andi	r6,r4,00000003
	move	r7,r4
	beqzc	r6,0040A8D8

l0040A896:
	lbu	r6,0000(r4)
	bnezc	r6,0040A8A0

l0040A89A:
	bc	0040A8DC

l0040A89C:
	lbu	r6,0000(r7)
	beqzc	r6,0040A8D4

l0040A8A0:
	addiu	r7,r7,00000001
	andi	r6,r7,00000003
	bnezc	r6,0040A89C

l0040A8A6:
	bc	0040A8AA

l0040A8A8:
	addiu	r7,r7,00000004

l0040A8AA:
	lw	r5,0000(r7)
	move	r6,r5
	nor	r5,r0,r5
	addiu	r6,r6,FEFEFEFF
	and	r6,r6,r5
	li	r5,80808080
	and	r6,r6,r5
	beqzc	r6,0040A8A8

l0040A8C4:
	move	r6,r7
	lbu	r7,0000(r7)
	beqzc	r7,0040A8D0

l0040A8CA:
	addiu	r6,r6,00000001
	lbu	r5,0000(r6)
	bnezc	r5,0040A8CA

l0040A8D0:
	subu	r4,r6,r4
	jrc	ra

l0040A8D4:
	subu	r4,r7,r4
	jrc	ra

l0040A8D8:
	move	r7,r4
	bc	0040A8AA

l0040A8DC:
	move	r4,r0
	jrc	ra

;; strncmp: 0040A8E0
;;   Called from:
;;     00400EA8 (in ping4_run)
;;     0040341E (in niquery_option_handler)
;;     004038B0 (in ping6_run)
;;     00406688 (in getnameinfo)
;;     00407504 (in __lookup_serv)
;;     00407528 (in __lookup_serv)
;;     00407D00 (in fn00407D00)
strncmp proc
	beqzc	r6,0040A91E

l0040A8E2:
	lbu	r7,0000(r4)
	lbu	r9,0000(r5)
	beqzc	r7,0040A926

l0040A8EA:
	beqc	r0,r9,0040A922

l0040A8EE:
	addiu	r6,r6,FFFFFFFF
	beqzc	r6,0040A922

l0040A8F2:
	bnec	r9,r7,0040A922

l0040A8F6:
	li	r7,00000001
	bc	0040A908

l0040A8FA:
	move	r5,r10
	beqc	r0,r9,0040A918

l0040A900:
	beqc	r6,r7,0040A918

l0040A902:
	addiu	r7,r7,00000001
	bnec	r8,r9,0040A918

l0040A908:
	lbux	r8,r7(r4)
	addiu	r10,r5,00000001
	lbu	r9,0001(r5)
	bnec	r0,r8,0040A8FA

l0040A918:
	subu	r4,r8,r9
	jrc	ra

l0040A91E:
	move	r4,r0
	jrc	ra

l0040A922:
	move	r8,r7
	bc	0040A918

l0040A926:
	move	r8,r0
	bc	0040A918
0040A92A                               00 00 00 00 00 00           ......

;; strncpy: 0040A930
;;   Called from:
;;     00400BF0 (in ping4_run)
;;     00400CB0 (in ping4_run)
;;     00400E56 (in ping4_run)
;;     0040675A (in if_indextoname)
;;     00406780 (in if_nametoindex)
strncpy proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	0040DC90
	move	r4,r16
	restore.jrc	00000010,ra,00000002
0040A93C                                     00 00 00 00             ....

;; strnlen: 0040A940
;;   Called from:
;;     00406DF0 (in is_valid_hostname)
;;     00406FE4 (in __lookup_name)
;;     004076A6 (in res_mkquery)
strnlen proc
	save	00000010,ra,00000003
	movep	r17,r16,r4,r5
	movep	r5,r6,r0,r16
	balc	0040A050
	beqzc	r4,0040A950

l0040A94C:
	subu	r4,r4,r17
	restore.jrc	00000010,ra,00000003

l0040A950:
	move	r4,r16
	restore.jrc	00000010,ra,00000003
0040A954             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; strspn: 0040A960
;;   Called from:
;;     00406976 (in inet_ntop)
strspn proc
	save	00000030,r16,00000001
	lbu	r7,0000(r5)
	sw	r0,0000(sp)
	sw	r0,0004(sp)
	sw	r0,0008(sp)
	sw	r0,000C(sp)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)
	beqc	r0,r7,0040AA18

l0040A97A:
	move	r6,r4
	lbu	r4,0001(r5)
	move	r10,r6
	lbu	r9,0000(r6)
	beqc	r0,r4,0040AA0A

l0040A988:
	srl	r4,r7,00000005
	addiu	r16,sp,00000020
	lsa	r4,r4,r16,00000002
	addiu	r8,r0,00000001
	sllv	r7,r8,r7
	sw	r7,-0020(r4)
	bc	0040A9B2

l0040A99E:
	addiu	r16,sp,00000020
	lsa	r4,r4,r16,00000002
	lw	r8,-0020(r4)
	or	r7,r7,r8
	sw	r7,-0020(r4)
	beqzc	r7,0040A9C6

l0040A9B2:
	addiu	r5,r5,00000001
	li	r7,00000001
	lbu	r8,0000(r5)
	srl	r4,r8,00000005
	sllv	r7,r7,r8
	bnec	r0,r8,0040A99E

l0040A9C6:
	beqc	r0,r9,0040AA18

l0040A9CA:
	srl	r7,r9,00000005
	addiu	r5,sp,00000020
	lsa	r7,r7,r5,00000002
	li	r4,00000001
	sllv	r9,r4,r9
	lw	r4,-0020(r7)
	and	r4,r9,r4
	bnezc	r4,0040A9F4

l0040A9E4:
	bc	0040AA06

l0040A9E6:
	addiu	r5,sp,00000020
	lsa	r4,r4,r5,00000002
	lw	r5,-0020(r4)
	and	r7,r7,r5
	beqzc	r7,0040AA02

l0040A9F4:
	addiu	r6,r6,00000001
	li	r7,00000001
	lbu	r5,0000(r6)
	srl	r4,r5,00000005
	sllv	r7,r7,r5
	bnezc	r5,0040A9E6

l0040AA02:
	subu	r4,r6,r10

l0040AA06:
	restore.jrc	00000030,r16,00000001

l0040AA0A:
	bnec	r9,r7,0040AA18

l0040AA0E:
	addiu	r6,r6,00000001
	lbu	r7,0000(r6)
	beqc	r7,r9,0040AA0E

l0040AA16:
	bc	0040AA02

l0040AA18:
	move	r4,r0
	restore.jrc	00000030,r16,00000001
0040AA1E                                           00 00               ..

;; twoway_strstr: 0040AA20
;;   Called from:
;;     0040AC72 (in strstr)
twoway_strstr proc
	save	00000460,r30,0000000A
	lbu	r6,0000(r5)
	movep	r22,r19,r4,r5
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,0018(sp)
	sw	r0,001C(sp)
	sw	r0,0020(sp)
	sw	r0,0024(sp)
	sw	r0,0028(sp)
	sw	r0,002C(sp)
	beqc	r0,r6,0040AC1C

l0040AA3C:
	lbu	r7,0000(r22)
	move	r20,r0
	bnezc	r7,0040AA4E

l0040AA44:
	bc	0040AB86

l0040AA46:
	lbux	r7,r20(r22)
	beqc	r0,r7,0040AB86

l0040AA4E:
	srl	r5,r6,00000005
	addiu	r7,sp,FFFFF430
	lsa	r5,r5,r7,00000002
	addiu	r20,r20,00000001
	li	r7,00000001
	addiu	r4,sp,FFFFF430
	lw	r16,0BE0(r5)
	sllv	r7,r7,r6
	lsa	r4,r6,r4,00000002
	lbux	r6,r20(r19)
	or	r7,r7,r16
	sw	r20,0C00(r4)
	sw	r7,0BE0(r5)
	bnezc	r6,0040AA46

l0040AA7C:
	addiu	r21,r0,00000001
	li	r6,00000001
	move	r16,r0
	li	r18,FFFFFFFF

l0040AA86:
	addu	r7,r16,r6
	addu	r5,r19,r18
	bgeuc	r7,r20,0040AAAE

l0040AA8E:
	lbux	r5,r6(r5)
	lbux	r4,r7(r19)
	beqc	r4,r5,0040AB9A

l0040AA9A:
	bgeuc	r4,r5,0040AB8E

l0040AA9E:
	subu	r21,r7,r18

l0040AAA2:
	move	r16,r7
	li	r6,00000001
	addu	r7,r16,r6
	addu	r5,r19,r18
	bltuc	r7,r20,0040AA8E

l0040AAAE:
	addiu	r8,r18,00000001
	li	r17,00000001
	li	r6,00000001
	move	r16,r0
	addiu	r23,r0,FFFFFFFF

l0040AABC:
	addu	r7,r16,r6
	addu	r5,r19,r23
	bgeuc	r7,r20,0040AAE8

l0040AAC6:
	lbux	r4,r6(r5)
	lbux	r5,r7(r19)
	beqc	r4,r5,0040ABAC

l0040AAD2:
	bgeuc	r4,r5,0040ABA2

l0040AAD6:
	subu	r17,r7,r23

l0040AADA:
	move	r16,r7
	li	r6,00000001
	addu	r7,r16,r6
	addu	r5,r19,r23
	bltuc	r7,r20,0040AAC6

l0040AAE8:
	addiu	r30,r23,00000001
	bltuc	r8,r30,0040AAF6

l0040AAF0:
	move	r30,r8
	move	r23,r18
	move	r17,r21

l0040AAF6:
	move	r6,r30
	addu	r5,r19,r17
	move.balc	r4,r19,0040A100
	beqc	r0,r4,0040AC14

l0040AB02:
	addiu	r7,r20,FFFFFFFF
	sw	r0,000C(sp)
	subu	r7,r7,r23
	sltu	r17,r7,r23
	movn	r7,r23,r17

l0040AB14:
	addiu	r17,r7,00000001

l0040AB18:
	move	r16,r22
	move	r21,r0

l0040AB1C:
	subu	r7,r22,r16
	bltuc	r7,r20,0040AB6E

l0040AB24:
	addu	r4,r16,r20
	addiu	r18,sp,FFFFF430
	lbu	r6,-0001(r4)
	li	r7,00000001
	srl	r5,r6,00000005
	sllv	r7,r7,r6
	lsa	r5,r5,r18,00000002
	lw	r5,0BE0(r5)
	and	r7,r7,r5
	beqzc	r7,0040ABB4

l0040AB44:
	lsa	r6,r6,r18,00000002
	lw	r7,0C00(r6)
	subu	r7,r20,r7
	beqzc	r7,0040ABBA

l0040AB52:
	lw	r17,000C(sp)
	beqzc	r6,0040AB62

l0040AB56:
	beqc	r0,r21,0040AB62

l0040AB5A:
	bgeuc	r7,r17,0040AB62

l0040AB5E:
	subu	r7,r20,r17

l0040AB62:
	addu	r16,r16,r7
	move	r21,r0
	subu	r7,r22,r16
	bgeuc	r7,r20,0040AB24

l0040AB6E:
	ori	r18,r20,0000003F
	movep	r5,r6,r0,r18
	move.balc	r4,r22,0040A050
	beqc	r0,r4,0040AC10

l0040AB7C:
	subu	r7,r4,r16
	bltuc	r7,r20,0040AB86

l0040AB82:
	move	r22,r4
	bc	0040AB24

l0040AB86:
	move	r16,r0

l0040AB88:
	move	r4,r16
	restore.jrc	00000460,r30,0000000A

l0040AB8E:
	move	r18,r16
	addiu	r21,r0,00000001
	addiu	r16,r16,00000001
	li	r6,00000001
	bc	0040AA86

l0040AB9A:
	beqc	r6,r21,0040AAA2

l0040AB9E:
	addiu	r6,r6,00000001
	bc	0040AA86

l0040ABA2:
	move	r23,r16
	li	r17,00000001
	addiu	r16,r16,00000001
	li	r6,00000001
	bc	0040AABC

l0040ABAC:
	beqc	r17,r6,0040AADA

l0040ABB0:
	addiu	r6,r6,00000001
	bc	0040AABC

l0040ABB4:
	move	r16,r4
	move	r21,r0
	bc	0040AB1C

l0040ABBA:
	sltu	r7,r21,r30
	move	r6,r30
	movz	r6,r21,r7

l0040ABC4:
	move	r7,r6
	lbux	r6,r6(r19)
	bnezc	r6,0040ABD6

l0040ABCC:
	bc	0040ABE8

l0040ABCE:
	addiu	r7,r7,00000001
	lbux	r6,r7(r19)
	beqzc	r6,0040ABE8

l0040ABD6:
	lbux	r5,r7(r16)
	beqc	r5,r6,0040ABCE

l0040ABDE:
	subu	r7,r7,r23
	move	r21,r0
	addu	r16,r16,r7
	bc	0040AB1C

l0040ABE8:
	bgeuc	r21,r30,0040AB88

l0040ABEC:
	lbux	r5,r23(r16)
	move	r7,r23
	lbux	r6,r23(r19)
	bnec	r5,r6,0040AC0A

l0040ABF8:
	bgeuc	r21,r7,0040AB88

l0040ABFC:
	addiu	r7,r7,FFFFFFFF
	lbux	r5,r7(r19)
	lbux	r6,r7(r16)
	beqc	r5,r6,0040ABF8

l0040AC0A:
	addu	r16,r16,r17
	lw	r5,000C(sp)
	bc	0040AB1C

l0040AC10:
	addu	r22,r22,r18
	bc	0040AB24

l0040AC14:
	subu	r7,r20,r17
	sw	r7,000C(sp)
	bc	0040AB18

l0040AC1C:
	move	r20,r0
	move	r30,r0
	addiu	r23,r0,FFFFFFFF
	li	r17,00000001
	bc	0040AAF6

;; strstr: 0040AC28
;;   Called from:
;;     00406EC0 (in name_from_hosts)
;;     00407488 (in __lookup_serv)
;;     00407D04 (in fn00407D04)
strstr proc
	lbu	r6,0000(r5)
	bnezc	r6,0040AC2E

l0040AC2C:
	jrc	ra

l0040AC2E:
	save	00000010,ra,00000002
	move	r16,r5
	move.balc	r5,r6,0040A770
	move	r7,r4
	beqzc	r4,0040ACB2

l0040AC3A:
	lbu	r5,0001(r16)
	beqzc	r5,0040ACB6

l0040AC3E:
	lbu	r8,0001(r4)
	beqc	r0,r8,0040ACB2

l0040AC46:
	lbu	r9,0002(r16)
	beqc	r0,r9,0040ACB8

l0040AC4E:
	lbu	r10,0002(r4)
	beqc	r0,r10,0040ACB2

l0040AC56:
	lbu	r11,0003(r16)
	beqc	r0,r11,0040AC76

l0040AC5E:
	lbu	r2,0003(r4)
	beqc	r0,r2,0040ACB2

l0040AC66:
	lbu	r6,0004(r16)
	beqzc	r6,0040ACDC

l0040AC6C:
	move	r5,r16
	restore	00000010,ra,00000002
	bc	0040AA20

l0040AC76:
	lbu	r4,0000(r16)
	sll	r5,r5,00000010
	lbu	r6,0000(r7)
	sll	r8,r8,00000010
	sll	r4,r4,00000018
	sll	r9,r9,00000008
	sll	r6,r6,00000018
	or	r5,r5,r4
	or	r6,r6,r8
	sll	r10,r10,00000008
	or	r9,r5,r9
	or	r6,r6,r10
	addiu	r4,r7,00000002

l0040ACA4:
	beqc	r9,r6,0040AD2A

l0040ACA8:
	addiu	r4,r4,00000001
	lbu	r7,0000(r4)
	or	r6,r6,r7
	sll	r6,r6,00000008
	bnezc	r7,0040ACA4

l0040ACB2:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

l0040ACB6:
	restore.jrc	00000010,ra,00000002

l0040ACB8:
	lbu	r6,0000(r16)
	addiu	r4,r4,00000001
	lbu	r7,0000(r7)
	sll	r6,r6,00000008
	sll	r7,r7,00000008
	or	r5,r5,r6
	or	r7,r7,r8

l0040ACC8:
	beqc	r5,r7,0040AD22

l0040ACCC:
	addiu	r4,r4,00000001
	sll	r7,r7,00000008
	lbu	r6,0000(r4)
	or	r7,r7,r6
	andi	r7,r7,0000FFFF
	bnezc	r6,0040ACC8

l0040ACD8:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

l0040ACDC:
	lbu	r4,0000(r16)
	sll	r5,r5,00000010
	lbu	r6,0000(r7)
	sll	r8,r8,00000010
	sll	r4,r4,00000018
	sll	r9,r9,00000008
	sll	r6,r6,00000018
	or	r5,r5,r4
	or	r6,r6,r8
	or	r5,r5,r11
	or	r6,r6,r2
	sll	r10,r10,00000008
	or	r5,r5,r9
	or	r6,r6,r10
	addiu	r4,r7,00000003

l0040AD12:
	beqc	r5,r6,0040AD26

l0040AD14:
	addiu	r4,r4,00000001
	sll	r6,r6,00000008
	lbu	r7,0000(r4)
	or	r6,r6,r7
	bnezc	r7,0040AD12

l0040AD1E:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

l0040AD22:
	addiu	r4,r4,FFFFFFFF
	restore.jrc	00000010,ra,00000002

l0040AD26:
	addiu	r4,r4,FFFFFFFD
	restore.jrc	00000010,ra,00000002

l0040AD2A:
	addiu	r4,r4,FFFFFFFE
	restore.jrc	00000010,ra,00000002
0040AD2E                                           00 00               ..

;; __lock: 0040AD30
;;   Called from:
;;     00404ACE (in __simple_malloc)
;;     00404B2C (in __simple_malloc)
;;     00404B40 (in __simple_malloc)
;;     00408616 (in __ofl_lock)
;;     0040DE32 (in __synccall)
;;     0040E076 (in rewinddir)
__lock proc
	save	00000010,ra,00000002
	addiupc	r7,000496FA
	move	r16,r4
	lw	r7,000C(r7)
	bnezc	r7,0040AD46

l0040AD3C:
	restore.jrc	00000010,ra,00000002

l0040AD3E:
	move	r6,r7
	addiu	r5,r16,00000004
	move.balc	r4,r16,0040ADB0

l0040AD46:
	sync	00000000

l0040AD4A:
	ll	r5,0000(r16)
	li	r7,00000001
	move	r6,r7
	sc	r6,0000(r16)
	beqzc	r6,0040AD4A

l0040AD58:
	sync	00000000
	bnezc	r5,0040AD3E

l0040AD5E:
	restore.jrc	00000010,ra,00000002

;; __unlock: 0040AD60
;;   Called from:
;;     00404B1E (in __simple_malloc)
;;     00404B6A (in __simple_malloc)
;;     00408624 (in __ofl_unlock)
;;     0040DEEA (in __synccall)
;;     0040E094 (in rewinddir)
__unlock proc
	save	00000010,ra,00000002
	lw	r7,0000(r4)
	move	r16,r4
	beqzc	r7,0040AD98

l0040AD68:
	sync	00000000
	sw	r0,0000(sp)
	sync	00000000
	lw	r7,0004(r4)
	beqzc	r7,0040AD98

l0040AD76:
	li	r7,00000001
	move	r5,r4
	addiu	r6,r0,00000081
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r4,r7,0040AD98

l0040AD8A:
	li	r7,00000001
	li	r4,00000062
	movep	r5,r6,r16,r7
	restore	00000010,ra,00000002
	bc	00404A50

l0040AD98:
	restore.jrc	00000010,ra,00000002
0040AD9A                               00 00 00 00 00 00           ......

;; __syscall_cp_c: 0040ADA0
;;   Called from:
;;     0040ADA4 (in __syscall_cp)
__syscall_cp_c proc
	bc	00404A50

;; __syscall_cp: 0040ADA4
;;   Called from:
;;     00405DE0 (in connect)
;;     00407662 (in recvfrom)
;;     00407680 (in recvmsg)
;;     00407D30 (in sendmsg)
;;     00407D52 (in sendto)
;;     00407E50 (in poll)
;;     0040AF82 (in close)
;;     0040B08E (in write)
;;     0040E0EC (in open64)
;;     0040E9E0 (in __timedwait_cp)
;;     0040E9FA (in __timedwait_cp)
__syscall_cp proc
	bc	0040ADA0
0040ADA8                         00 00 00 00 00 00 00 00         ........

;; __wait: 0040ADB0
;;   Called from:
;;     00404C0E (in alloc_fwd)
;;     00404DE2 (in alloc_rev)
;;     00405082 (in free)
;;     004050B0 (in free)
;;     0040543A (in malloc)
;;     004055B0 (in malloc)
;;     0040832A (in flockfile)
;;     0040AD42 (in __lock)
;;     0040D1EC (in __lockfile)
__wait proc
	save	00000020,ra,00000005
	addiu	r19,r0,00000080
	move	r17,r6
	movz	r19,r0,r7

l0040ADBC:
	li	r7,00000065
	movep	r18,r16,r4,r5
	bc	0040ADCC

l0040ADC2:
	lw	r6,0000(r18)
	bnec	r6,r17,0040AE08

l0040ADC8:
	sync	00000000

l0040ADCC:
	addiu	r7,r7,FFFFFFFF
	beqzc	r7,0040ADEC

l0040ADD0:
	beqzc	r16,0040ADC2

l0040ADD2:
	lw	r6,0000(r16)
	beqzc	r6,0040ADC2

l0040ADD6:
	sync	00000000

l0040ADDA:
	ll	r7,0000(r16)
	addiu	r7,r7,00000001
	sc	r7,0000(r16)
	beqzc	r7,0040ADDA

l0040ADE6:
	sync	00000000
	bc	0040ADEE

l0040ADEC:
	bnezc	r16,0040ADD6

l0040ADEE:
	lw	r7,0000(r18)
	beqc	r17,r7,0040AE0A

l0040ADF2:
	beqzc	r16,0040AE08

l0040ADF4:
	sync	00000000

l0040ADF8:
	ll	r7,0000(r16)
	addiu	r7,r7,FFFFFFFF
	sc	r7,0000(r16)
	beqzc	r7,0040ADF8

l0040AE04:
	sync	00000000

l0040AE08:
	restore.jrc	00000020,ra,00000005

l0040AE0A:
	movep	r7,r8,r17,r0
	movep	r5,r6,r18,r19
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040ADEE

l0040AE1C:
	movep	r7,r8,r17,r0
	movep	r5,r6,r18,r0
	li	r4,00000062
	balc	00404A50
	bc	0040ADEE
0040AE28                         00 00 00 00 00 00 00 00         ........

;; __do_cleanup_push: 0040AE30
;;   Called from:
;;     0040AE36 (in _pthread_cleanup_push)
;;     0040AE3E (in _pthread_cleanup_pop)
__do_cleanup_push proc
	jrc	ra

;; _pthread_cleanup_push: 0040AE32
;;   Called from:
;;     004078A0 (in __res_msend_rc)
;;     0040E904 (in sem_timedwait)
_pthread_cleanup_push proc
	swm	r5,0000(r4),00000002
	bc	0040AE30

;; _pthread_cleanup_pop: 0040AE3A
;;   Called from:
;;     00407A7A (in __res_msend_rc)
;;     0040E918 (in sem_timedwait)
_pthread_cleanup_pop proc
	save	00000010,ra,00000003
	movep	r16,r17,r4,r5
	balc	0040AE30
	beqzc	r17,0040AE4E

l0040AE44:
	lw	r7,0000(r16)
	lw	r4,0004(r16)
	restore	00000010,ra,00000003
	jrc	r7

l0040AE4E:
	restore.jrc	00000010,ra,00000003

;; __pthread_setcancelstate: 0040AE50
;;   Called from:
;;     00407240 (in __lookup_name)
;;     0040736A (in __lookup_name)
;;     00407A82 (in fn00407A82)
;;     0040DE40 (in __synccall)
;;     0040DEE2 (in __synccall)
;;     0040EA22 (in __timedwait)
;;     0040EA38 (in __timedwait)
__pthread_setcancelstate proc
	li	r7,00000016
	bgeiuc	r4,00000003,0040AE68

l0040AE56:
	rdhwr	r3,0000001D,00000000
	beqzc	r5,0040AE62

l0040AE5C:
	lw	r7,-0080(r3)
	sw	r7,0040(sp)

l0040AE62:
	move	r7,r0
	sw	r4,-0080(r3)

l0040AE68:
	move	r4,r7
	jrc	ra
0040AE6C                                     00 00 00 00             ....

;; pthread_sigmask: 0040AE70
;;   Called from:
;;     00408042 (in sigprocmask)
pthread_sigmask proc
	save	00000010,ra,00000002
	move	r9,r4
	move	r16,r6
	li	r4,00000016
	bgeiuc	r9,00000003,0040AEA6

l0040AE7C:
	movep	r6,r7,r5,r16
	addiu	r8,r0,00000008
	addiu	r4,r0,00000087
	move.balc	r5,r9,00404A50
	beqzc	r4,0040AE92

l0040AE8C:
	subu	r4,r0,r4
	restore.jrc	00000010,ra,00000002

l0040AE92:
	move	r4,r0
	beqzc	r16,0040AEA6

l0040AE96:
	lw	r7,0000(r16)
	ext	r7,r7,00000000,0000001F
	sw	r7,0000(sp)
	lw	r7,0004(r16)
	ins	r7,r0,00000000,00000001
	sw	r7,0004(sp)

l0040AEA6:
	restore.jrc	00000010,ra,00000002
0040AEA8                         00 00 00 00 00 00 00 00         ........

;; cgt_init: 0040AEB0
cgt_init proc
	save	00000010,ra,00000003
	movep	r16,r17,r4,r5
	addiupc	r5,00007174
	addiupc	r4,00007188
	balc	0040CC50
	addiupc	r5,FFFFFFEC
	move	r6,r4
	sync	00000000

l0040AECA:
	addiupc	r7,000254D2
	ll	r7,0000(r7)
	bnec	r5,r7,0040AEE0

l0040AED4:
	move	r7,r6
	addiupc	r4,000254C6
	sc	r7,0000(r4)
	beqzc	r7,0040AECA

l0040AEE0:
	sync	00000000
	bnezc	r6,0040AEEC

l0040AEE6:
	addiu	r4,r0,FFFFFFDA
	restore.jrc	00000010,ra,00000003

l0040AEEC:
	movep	r4,r5,r16,r17
	restore	00000010,ra,00000003
	jrc	r6

;; __clock_gettime: 0040AEF4
;;   Called from:
;;     00407756 (in res_mkquery)
;;     00407786 (in mtime)
;;     0040AF4A (in gettimeofday)
;;     0040DFBE (in __synccall)
;;     0040E9A2 (in __timedwait_cp)
__clock_gettime proc
	save	00000010,ra,00000003
	lwpc	r7,004303A0
	movep	r17,r16,r4,r5
	beqzc	r7,0040AF12

l0040AF00:
	jalrc	ra,r7
	beqzc	r4,0040AF3C

l0040AF04:
	addiu	r6,r0,FFFFFFEA
	bnec	r4,r6,0040AF12

l0040AF0A:
	restore	00000010,ra,00000003
	bc	0040CC30

l0040AF12:
	movep	r5,r6,r17,r16
	li	r4,00000071
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040AF0A

l0040AF22:
	addiu	r4,r0,FFFFFFEA
	bnezc	r17,0040AF0A

l0040AF28:
	movep	r5,r6,r16,r0
	addiu	r4,r0,000000A9
	balc	00404A50
	lw	r7,0004(r16)
	addiu	r6,r0,000003E8
	mul	r7,r7,r6
	sw	r7,0004(sp)

l0040AF3C:
	move	r4,r0
	restore.jrc	00000010,ra,00000003

;; gettimeofday: 0040AF40
;;   Called from:
;;     00400A7E (in ping4_send_probe)
;;     00400AB2 (in ping4_send_probe)
;;     00401B1A (in ping4_parse_reply)
;;     004021B0 (in fn004021B0)
;;     00402C5C (in main_loop)
;;     004034A4 (in build_echo)
;;     004035BE (in ping6_run)
gettimeofday proc
	save	00000020,ra,00000002
	move	r16,r4
	beqzc	r4,0040AF5E

l0040AF46:
	addiu	r5,sp,00000008
	move	r4,r0
	balc	0040AEF4
	lw	r17,0008(sp)
	addiu	r6,r0,000003E8
	lw	r17,000C(sp)
	sw	r7,0000(sp)
	div	r7,r5,r6
	sw	r7,0004(sp)

l0040AF5E:
	move	r4,r0
	restore.jrc	00000020,ra,00000002
0040AF62       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; dummy: 0040AF70
dummy proc
	jrc	ra

;; close: 0040AF72
;;   Called from:
;;     00400E32 (in ping4_run)
;;     004037DC (in ping6_run)
;;     0040732A (in __lookup_name)
;;     00407842 (in __res_msend_rc)
;;     0040DF62 (in __synccall)
close proc
	save	00000010,ra,00000001
	balc	004080E0
	move	r10,r0
	movep	r7,r8,r0,r0
	movep	r5,r6,r4,r0
	move	r9,r0
	li	r4,00000039
	balc	0040ADA4
	addiu	r7,r0,FFFFFFFC
	xor	r7,r7,r4
	restore	00000010,ra,00000001
	movz	r4,r0,r7

l0040AF94:
	bc	0040CC30
0040AF98                         00 00 00 00 00 00 00 00         ........

;; geteuid: 0040AFA0
;;   Called from:
;;     00401CC0 (in limit_capabilities)
geteuid proc
	addiu	r4,r0,000000AF
	bc	00404A50
0040AFA8                         00 00 00 00 00 00 00 00         ........

;; getpid: 0040AFB0
;;   Called from:
;;     00402358 (in setup)
;;     004035C2 (in ping6_run)
getpid proc
	addiu	r4,r0,000000AC
	bc	00404A50
0040AFB8                         00 00 00 00 00 00 00 00         ........

;; getuid: 0040AFC0
;;   Called from:
;;     00401E82 (in fn00401E82)
getuid proc
	addiu	r4,r0,000000AE
	bc	00404A50
0040AFC8                         00 00 00 00 00 00 00 00         ........

;; isatty: 0040AFD0
;;   Called from:
;;     004023B4 (in setup)
isatty proc
	save	00000020,ra,00000001
	li	r6,40087468
	move	r5,r4
	addiu	r7,sp,00000008
	li	r4,0000001D
	balc	00404A50
	sltiu	r4,r4,00000001
	restore.jrc	00000020,ra,00000001
0040AFE8                         00 00 00 00 00 00 00 00         ........

;; seteuid: 0040AFF0
;;   Called from:
;;     00401CD0 (in limit_capabilities)
;;     00401CF0 (in modify_capability)
seteuid proc
	li	r7,FFFFFFFF
	movep	r5,r6,r7,r4
	addiu	r4,r0,00000093
	bc	0040B04C
0040AFFC                                     00 00 00 00             ....

;; setuid: 0040B000
;;   Called from:
;;     00401D06 (in drop_capabilities)
setuid proc
	move	r5,r4
	addiu	r4,r0,00000092
	movep	r6,r7,r0,r0
	bc	0040B04C
0040B00C                                     00 00 00 00             ....

;; do_setxid: 0040B010
;;   Called from:
;;     0040DF80 (in __synccall)
do_setxid proc
	save	00000010,ra,00000003
	lw	r7,0010(r4)
	move	r16,r4
	bltc	r0,r7,0040B046

l0040B01A:
	lwm	r5,0000(r4),00000002
	lw	r7,0008(r4)
	lw	r4,000C(r4)
	balc	0040B048
	subu	r17,r0,r4
	beqzc	r4,0040B044

l0040B02A:
	lw	r7,0010(r16)
	bnezc	r7,0040B044

l0040B02E:
	move	r4,r0
	balc	00407EA0
	addiu	r4,r0,000000AC
	balc	0040B048
	li	r6,00000009
	move	r5,r4
	addiu	r4,r0,00000081
	balc	0040B048

l0040B044:
	sw	r17,0010(sp)

l0040B046:
	restore.jrc	00000010,ra,00000003

;; fn0040B048: 0040B048
;;   Called from:
;;     0040B022 (in do_setxid)
;;     0040B038 (in do_setxid)
;;     0040B042 (in do_setxid)
fn0040B048 proc
	bc	00404A50

;; __setxid: 0040B04C
;;   Called from:
;;     0040AFF8 (in seteuid)
;;     0040B008 (in setuid)
__setxid proc
	save	00000030,ra,00000002
	li	r16,FFFFFFFF
	swm	r5,000C(sp),00000002
	sw	r4,0018(sp)
	addiu	r5,sp,0000000C
	addiupc	r4,FFFFFFB4
	sw	r7,0014(sp)
	sw	r16,001C(sp)
	balc	0040DDF2
	lw	r17,001C(sp)
	beqzc	r4,0040B076

l0040B068:
	bgec	r0,r4,0040B074

l0040B06C:
	balc	004049B0
	lw	r17,001C(sp)
	sw	r7,0000(sp)

l0040B074:
	move	r4,r16

l0040B076:
	restore.jrc	00000030,ra,00000002
0040B078                         00 00 00 00 00 00 00 00         ........

;; write: 0040B080
;;   Called from:
;;     00400A10 (in write_stdout)
;;     00401C6C (in write_stdout)
;;     00402EEC (in write_stdout)
write proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	li	r4,00000040
	balc	0040ADA4
	restore	00000010,ra,00000001
	bc	0040CC30
0040B09A                               00 00 00 00 00 00           ......

;; isalnum: 0040B0A0
;;   Called from:
;;     00406E26 (in is_valid_hostname)
;;     0040B0BA (in isalnum_l)
isalnum proc
	ori	r7,r4,00000020
	li	r6,00000001
	addiu	r7,r7,FFFFFF9F
	bltiuc	r7,0000001A,0040B0B6

l0040B0AE:
	addiu	r4,r4,FFFFFFD0
	sltiu	r6,r4,0000000A

l0040B0B6:
	move	r4,r6
	jrc	ra

;; isalnum_l: 0040B0BA
isalnum_l proc
	bc	0040B0A0
0040B0BE                                           00 00               ..

;; __init_tp: 0040B0C0
;;   Called from:
;;     0040B212 (in __init_tls)
__init_tp proc
	save	00000010,ra,00000002
	move	r16,r4
	addiu	r4,r4,000000B0
	sw	r16,0000(r16)
	balc	0040DD30
	li	r7,FFFFFFFF
	bltc	r4,r0,0040B0FC

l0040B0D4:
	bnezc	r4,0040B0E0

l0040B0D6:
	li	r6,00000001
	aluipc	r7,0000040B
	sw	r6,0430(r7)

l0040B0E0:
	addiu	r5,r16,0000001C
	li	r4,00000060
	balc	00404A50
	addiupc	r7,0004936C
	sw	r7,0078(r16)
	addiu	r7,r16,00000064
	sw	r7,0064(r16)
	move	r7,r0
	sw	r4,001C(sp)

l0040B0FC:
	move	r4,r7
	restore.jrc	00000010,ra,00000002

;; __copy_tls: 0040B100
;;   Called from:
;;     0040B20E (in __init_tls)
__copy_tls proc
	save	00000020,ra,00000006
	addiupc	r7,0004932A
	lw	r16,0020(r7)
	move	r18,r7
	lw	r6,0018(r7)
	lw	r17,001C(r7)
	nor	r16,r0,r16
	lsa	r16,r16,r6,00000002
	addiu	r6,r0,FFFFFF50
	addiu	r17,r17,FFFFFFFF
	subu	r6,r6,r4
	addu	r16,r4,r16
	and	r17,r17,r6
	lw	r19,0014(r7)
	addu	r17,r4,r17
	addiu	r20,r16,00000004
	bc	0040B142

l0040B12C:
	lw	r4,0014(r19)
	lw	r6,0008(r19)
	addiu	r4,r4,000000B0
	addu	r4,r17,r4
	sw	r4,0000(r20)
	addiu	r20,r20,00000004
	lw	r5,0004(r19)
	balc	0040A130
	lw	r19,0000(r19)

l0040B142:
	bnezc	r19,0040B12C

l0040B144:
	lw	r7,0020(r18)
	move	r4,r17
	sw	r7,0000(sp)
	sw	r16,0001(r17)
	sw	r16,00AC(r17)
	restore.jrc	00000020,ra,00000006

;; __init_tls: 0040B152
;;   Called from:
;;     004048D0 (in __init_libc)
__init_tls proc
	save	00000010,ra,00000001
	lw	r10,0003(r4)
	movep	r5,r6,r0,r0
	lw	r9,0014(r4)
	move	r7,r10
	bc	0040B17C

l0040B160:
	bneiuc	r8,00000002,0040B18E

l0040B164:
	addiupc	r8,FFBF4E96
	beqc	r0,r8,0040B174

l0040B16E:
	lw	r5,0008(r7)
	subu	r5,r8,r5

l0040B174:
	lw	r8,0010(r4)
	addiu	r9,r9,FFFFFFFF
	addu	r7,r7,r8

l0040B17C:
	beqc	r0,r9,0040B198

l0040B180:
	lw	r8,0000(r7)
	bneiuc	r8,00000006,0040B160

l0040B186:
	lw	r5,0008(r7)
	subu	r5,r10,r5
	bc	0040B174

l0040B18E:
	xori	r8,r8,00000007
	movz	r6,r7,r8

l0040B196:
	bc	0040B174

l0040B198:
	addiupc	r7,00027C44
	addiupc	r4,00049290
	beqzc	r6,0040B1BA

l0040B1A2:
	lw	r8,0002(r6)
	addu	r5,r5,r8
	sw	r5,0044(sp)
	lw	r5,0010(r6)
	sw	r5,0048(sp)
	lw	r5,0014(r6)
	lw	r6,001C(r6)
	swm	r5,000C(r7),00000002
	li	r6,00000001
	sw	r7,0014(sp)
	sw	r6,0020(sp)

l0040B1BA:
	lw	r5,000C(r7)
	lw	r6,0004(r7)
	lw	r8,0010(r7)
	addu	r6,r6,r5
	subu	r6,r0,r6
	addiu	r9,r8,FFFFFFFF
	and	r6,r6,r9
	addu	r5,r6,r5
	sw	r5,004C(sp)
	bgeiuc	r8,00000004,0040B1DC

l0040B1D8:
	li	r6,00000004
	sw	r6,0050(sp)

l0040B1DC:
	lw	r7,0010(r7)
	addiu	r6,r7,000000BB
	addu	r6,r6,r5
	ins	r6,r0,00000000,00000001
	swm	r6,0018(r4),00000002
	addiu	r7,r0,000000F4
	addiupc	r4,00027C04
	bgeuc	r7,r6,0040B20E

l0040B1F8:
	move	r10,r0
	addiu	r9,r0,FFFFFFFF
	addiu	r8,r0,00000802
	li	r7,00000003
	move	r5,r0
	addiu	r4,r0,000000DE
	balc	00404A50

l0040B20E:
	balc	0040B100
	balc	0040B0C0
	bgec	r4,r0,0040B222

l0040B21A:
	sb	r0,0000(r0)
	teq	r0,r0,00000000

l0040B222:
	restore.jrc	00000010,ra,00000001
0040B224             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; _Exit: 0040B230
;;   Called from:
;;     0040016C (in exit)
_Exit proc
	save	00000010,ra,00000002
	move	r16,r4
	move	r5,r4
	li	r4,0000005E

l0040B238:
	balc	00404A50
	move	r5,r16
	li	r4,0000005D
	bc	0040B238
0040B242       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; scanexp: 0040B250
;;   Called from:
;;     0040BAF0 (in decfloat)
;;     0040C5D6 (in __floatscan)
scanexp proc
	save	00000030,ra,00000006
	lw	r7,0004(r4)
	move	r19,r4
	lw	r9,0068(r4)
	bgeuc	r7,r9,0040B3BC

l0040B25E:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r17,0000(r7)

l0040B266:
	beqic	r17,0000002B,0040B39E

l0040B26A:
	beqic	r17,0000002D,0040B39E

l0040B26E:
	addiu	r7,r17,FFFFFFD0
	bgeiuc	r7,0000000A,0040B334

l0040B276:
	move	r18,r0

l0040B278:
	move	r16,r0
	bc	0040B292

l0040B27C:
	sw	r6,0044(sp)
	lbu	r17,0000(r7)
	li	r7,0CCCCCCB
	addiu	r10,r17,FFFFFFD0
	bgeiuc	r10,0000000A,0040B2C0

l0040B28E:
	bltc	r7,r16,0040B2C0

l0040B292:
	lsa	r16,r16,r16,00000002
	lw	r7,0004(r19)
	lsa	r16,r16,r17,00000001
	addiu	r16,r16,FFFFFFD0
	addiu	r6,r7,00000001
	bltuc	r7,r9,0040B27C

l0040B2A8:
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	move	r17,r4
	li	r7,0CCCCCCB
	addiu	r10,r17,FFFFFFD0
	bltiuc	r10,0000000A,0040B28E

l0040B2C0:
	sra	r20,r16,0000001F

l0040B2C4:
	sll	r6,r16,00000002
	srl	r7,r16,0000001E
	sll	r4,r20,00000002
	addu	r8,r6,r16
	or	r4,r4,r7
	sltu	r6,r8,r6
	addu	r7,r4,r20
	sll	r4,r8,00000001
	addu	r7,r6,r7
	srl	r8,r8,0000001F
	sll	r7,r7,00000001
	addu	r6,r4,r17
	or	r7,r7,r8
	sra	r17,r17,0000001F
	sltu	r4,r6,r4
	addu	r7,r7,r17
	addiu	r17,r6,FFFFFFD0
	addu	r7,r4,r7
	li	r5,0147AE14
	sltu	r6,r17,r6
	bgeiuc	r10,0000000A,0040B37E

l0040B30C:
	bltc	r5,r20,0040B356

l0040B310:
	beqc	r20,r5,0040B3CC

l0040B314:
	lw	r5,0004(r19)
	addiu	r7,r7,FFFFFFFF
	move	r16,r17
	addu	r20,r6,r7
	bgeuc	r5,r9,0040B36E

l0040B322:
	addiu	r7,r5,00000001
	sw	r7,0044(sp)
	lbu	r17,0000(r5)
	addiu	r10,r17,FFFFFFD0
	bc	0040B2C4

l0040B330:
	bnec	r0,r5,0040B3D8

l0040B334:
	beqc	r0,r9,0040B3F2

l0040B338:
	lw	r7,0004(r19)

l0040B33A:
	addiu	r7,r7,FFFFFFFF
	move	r16,r0
	lui	r20,FFF80000
	sw	r7,0044(sp)

l0040B344:
	movep	r4,r5,r16,r20
	restore.jrc	00000030,ra,00000006

l0040B348:
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	move	r17,r4
	addiu	r10,r17,FFFFFFD0

l0040B356:
	bgeiuc	r10,0000000A,0040B37E

l0040B35A:
	lw	r7,0004(r19)
	bgeuc	r7,r9,0040B348

l0040B360:
	addiu	r6,r7,00000001
	sw	r6,0044(sp)
	lbu	r17,0000(r7)
	addiu	r10,r17,FFFFFFD0
	bc	0040B356

l0040B36E:
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	move	r17,r4
	addiu	r10,r17,FFFFFFD0
	bc	0040B2C4

l0040B37E:
	beqc	r0,r9,0040B388

l0040B382:
	lw	r7,0004(r19)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0044(sp)

l0040B388:
	beqzc	r18,0040B344

l0040B38A:
	subu	r16,r0,r16
	subu	r7,r0,r20
	sltu	r5,r0,r16
	subu	r20,r7,r5
	movep	r4,r5,r16,r20
	restore.jrc	00000030,ra,00000006

l0040B39E:
	lw	r7,0004(r19)
	bgeuc	r7,r9,0040B3E4

l0040B3A4:
	addiu	r6,r7,00000001
	sw	r6,0044(sp)
	lbu	r4,0000(r7)

l0040B3AC:
	addiu	r7,r4,FFFFFFD0
	bgeiuc	r7,0000000A,0040B330

l0040B3B4:
	seqi	r18,r17,0000002D
	move	r17,r4
	bc	0040B278

l0040B3BC:
	sw	r5,000C(sp)
	balc	0040CB78
	move	r17,r4
	lw	r9,0068(r19)
	lw	r17,000C(sp)
	bc	0040B266

l0040B3CC:
	li	r5,7AE147AD
	bgeuc	r5,r16,0040B314

l0040B3D6:
	bc	0040B356

l0040B3D8:
	beqc	r0,r9,0040B3F2

l0040B3DC:
	lw	r7,0004(r19)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0044(sp)
	bc	0040B33A

l0040B3E4:
	sw	r5,000C(sp)
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	lw	r17,000C(sp)
	bc	0040B3AC

l0040B3F2:
	move	r16,r0
	lui	r20,FFF80000
	bc	0040B344

;; decfloat: 0040B3FA
;;   Called from:
;;     0040BFFA (in __floatscan)
decfloat proc
	save	00000260,r30,0000000A
	move	r16,r4
	swm	r7,0004(sp),00000002
	sw	r6,0010(sp)
	sw	r9,0014(sp)
	beqic	r5,00000030,0040B410

l0040B40C:
	bc	0040BAE8

l0040B410:
	lw	r6,0068(r16)

l0040B414:
	lw	r7,0004(r16)
	bgeuc	r7,r6,0040B62E

l0040B41A:
	addiu	r5,r7,00000001
	sw	r5,0004(sp)
	lbu	r5,0000(r7)
	beqic	r5,00000030,0040B414

l0040B426:
	li	r7,00000001
	sw	r7,0000(sp)

l0040B42A:
	beqic	r5,0000002E,0040B836

l0040B42E:
	move	r21,r0
	move	r23,r0
	move	r18,r0

l0040B434:
	move	r19,r0
	move	r22,r0
	move	r17,r0
	move	r30,r0
	move	r20,r0
	sw	r0,0030(sp)

l0040B440:
	addiu	r7,r5,FFFFFFD0
	bltiuc	r7,0000000A,0040B472

l0040B448:
	bneiuc	r5,0000002E,0040B4CC

l0040B44C:
	bnec	r0,r21,0040B822

l0040B450:
	move	r23,r22
	move	r18,r17
	addiu	r21,r0,00000001

l0040B458:
	lw	r7,0004(r16)
	lw	r6,0068(r16)
	bgeuc	r7,r6,0040B4C4

l0040B462:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r5,0000(r7)
	addiu	r7,r5,FFFFFFD0
	bgeiuc	r7,0000000A,0040B448

l0040B472:
	beqic	r5,0000002E,0040B44C

l0040B476:
	addiu	r6,r22,00000001
	sltu	r4,r6,r22
	move	r22,r6
	addu	r17,r4,r17
	bgeic	r30,0000003D,0040B602

l0040B486:
	xori	r5,r5,00000030
	addiu	r4,r20,00000001
	movn	r19,r6,r5

l0040B492:
	beqc	r0,r20,0040B618

l0040B496:
	addiu	r6,sp,FFFFF230
	lsa	r5,r30,r6,00000002
	lw	r6,0E00(r5)
	lsa	r6,r6,r6,00000002
	lsa	r7,r6,r7,00000001
	sw	r7,0E00(r5)
	bneiuc	r4,00000009,0040B63E

l0040B4B2:
	li	r7,00000001
	lw	r6,0068(r16)
	sw	r7,0000(sp)
	addiu	r30,r30,00000001
	lw	r7,0004(r16)
	move	r20,r0
	bltuc	r7,r6,0040B462

l0040B4C4:
	move.balc	r4,r16,0040CB78
	move	r5,r4
	bc	0040B440

l0040B4CC:
	movz	r23,r22,r21

l0040B4D0:
	movz	r18,r17,r21

l0040B4D4:
	lw	r17,0000(sp)
	beqc	r0,r7,0040B646

l0040B4DA:
	ori	r7,r5,00000020
	beqic	r7,00000025,0040BAEE

l0040B4E2:
	bltc	r5,r0,0040B4F8

l0040B4E6:
	lw	r7,0068(r16)
	beqzc	r7,0040B4F8

l0040B4EC:
	lw	r7,0004(r16)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)
	lw	r17,0000(sp)
	beqc	r0,r7,0040B652

l0040B4F8:
	lw	r4,0030(sp)
	beqc	r0,r16,0040B826

l0040B4FE:
	beqc	r22,r23,0040BBA4

l0040B502:
	lw	r17,0004(sp)
	srl	r7,r6,0000001F
	addu	r7,r7,r6
	sra	r7,r7,00000001
	subu	r7,r0,r7
	sra	r6,r7,0000001F
	bltc	r6,r18,0040BBF6

l0040B51A:
	beqc	r18,r6,0040BBF2

l0040B51E:
	lw	r17,0004(sp)
	addiu	r6,r7,FFFFFF96
	sra	r7,r6,0000001F
	bltc	r18,r7,0040BB0C

l0040B52C:
	beqc	r18,r7,0040BB08

l0040B530:
	beqc	r0,r20,0040B59E

l0040B534:
	bgeic	r20,00000009,0040B59C

l0040B538:
	sll	r5,r30,00000002
	addiu	r7,sp,FFFFF230
	addu	r7,r7,r5
	lw	r7,0E00(r7)
	lsa	r7,r7,r7,00000002
	sll	r6,r7,00000001
	beqic	r20,00000008,0040B592

l0040B550:
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000007,0040B592

l0040B55A:
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000006,0040B592

l0040B564:
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000005,0040B592

l0040B56E:
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000004,0040B592

l0040B578:
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000003,0040B592

l0040B582:
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	bneiuc	r20,00000001,0040B592

l0040B58C:
	lsa	r6,r7,r6,00000003
	sll	r6,r6,00000001

l0040B592:
	addiu	r7,sp,FFFFF230
	addu	r5,r7,r5
	sw	r6,0E00(r5)

l0040B59C:
	addiu	r30,r30,00000001

l0040B59E:
	lw	r17,0008(sp)
	move	r20,r23
	balc	00410170
	swm	r4,0008(sp),00000002
	bgeic	r19,00000009,0040B668

l0040B5AE:
	bltc	r23,r19,0040B668

l0040B5B2:
	bgeic	r23,00000012,0040B668

l0040B5B6:
	lw	r4,0030(sp)
	bneiuc	r23,00000009,0040B5C0

l0040B5BC:
	bc	0040BD24

l0040B5C0:
	bgeic	r23,00000009,0040B5C8

l0040B5C4:
	bc	0040BCF8

l0040B5C8:
	addiu	r7,r23,FFFFFFF7
	sll	r6,r7,00000002
	subu	r7,r7,r6
	lw	r17,0010(sp)
	addu	r7,r7,r6
	bgeic	r7,0000001F,0040B5E0

l0040B5D8:
	srlv	r7,r16,r7
	bnec	r0,r7,0040B668

l0040B5E0:
	addiupc	r7,00007C5C
	addiu	r23,r23,FFFFFFF6
	lwxs	r4,r23(r7)
	balc	00410170
	movep	r18,r19,r4,r5
	move.balc	r4,r16,004101D0
	lwm	r6,0008(sp),00000002
	balc	00404330
	bc	0040BBD0

l0040B602:
	beqic	r5,00000030,0040B458

l0040B606:
	lw	r7,0220(sp)
	addiu	r19,r0,0000045C
	ori	r7,r7,00000001
	sw	r7,0220(sp)
	bc	0040B458

l0040B618:
	li	r6,00000001
	addiu	r20,r0,00000001
	sw	r6,0000(sp)
	addiu	r6,sp,FFFFF230
	lsa	r6,r30,r6,00000002
	sw	r7,0E00(r6)
	bc	0040B458

l0040B62E:
	move.balc	r4,r16,0040CB78
	move	r5,r4
	beqic	r4,00000030,0040B410

l0040B638:
	li	r7,00000001
	sw	r7,0000(sp)
	bc	0040B42A

l0040B63E:
	li	r7,00000001
	move	r20,r4
	sw	r7,0000(sp)
	bc	0040B458

l0040B646:
	bltc	r5,r0,0040B652

l0040B64A:
	lw	r7,0068(r16)
	bnec	r0,r7,0040B4EC

l0040B652:
	balc	004049B0
	li	r7,00000016
	sw	r7,0000(sp)

l0040B65A:
	movep	r6,r7,r0,r0
	move.balc	r4,r16,0040CB40
	movep	r4,r5,r0,r0

l0040B662:
	restore.jrc	00000260,r30,0000000A

l0040B666:
	move	r30,r7

l0040B668:
	addiu	r7,r30,FFFFFFFF
	addiu	r6,sp,00000030
	lwxs	r6,r7(r6)
	beqzc	r6,0040B666

l0040B672:
	li	r9,38E38E39
	sra	r7,r23,0000001F
	muh	r9,r23,r9
	sra	r9,r9,00000001
	subu	r17,r9,r7
	lsa	r9,r17,r17,00000003
	subu	r17,r23,r9
	beqzc	r17,0040B70E

l0040B692:
	bltc	r23,r0,0040BCC6

l0040B696:
	li	r7,00000008
	addiupc	r6,00007BA4
	subu	r7,r7,r17
	lwxs	r16,r7(r6)
	beqc	r0,r30,0040BD52

l0040B6A4:
	li	r6,3B9ACA00
	move	r5,r0
	move	r18,r0
	move	r7,r0
	div	r19,r6,r16
	teq	r16,r0,00000007

l0040B6B8:
	bc	0040B6C0

l0040B6BA:
	addiu	r7,r7,00000001
	beqc	r30,r7,0040B6F4

l0040B6C0:
	addiu	r6,sp,00000030
	lwxs	r4,r7(r6)
	divu	r6,r4,r16
	teq	r16,r0,00000007

l0040B6CC:
	addu	r6,r6,r5
	modu	r5,r4,r16
	teq	r16,r0,00000007

l0040B6D6:
	addiu	r4,sp,00000030
	mul	r5,r5,r19
	swxs	r6,r7(r4)
	bnec	r7,r18,0040B6BA

l0040B6E2:
	bnezc	r6,0040B6BA

l0040B6E4:
	addiu	r18,r18,00000001
	addiu	r7,r7,00000001
	addiu	r20,r20,FFFFFFF7
	andi	r18,r18,0000007F
	bnec	r30,r7,0040B6C0

l0040B6F4:
	beqzc	r5,0040B704

l0040B6F6:
	addiu	r7,sp,FFFFF230
	lsa	r7,r30,r7,00000002
	addiu	r30,r30,00000001
	sw	r5,0E00(r7)

l0040B704:
	subu	r10,r20,r17
	move	r17,r18
	addiu	r20,r10,00000009

l0040B70E:
	sw	r0,0000(sp)

l0040B710:
	bltic	r20,00000012,0040B72E

l0040B714:
	bneiuc	r20,00000012,0040B890

l0040B718:
	addiu	r7,sp,FFFFF230
	li	r6,0089705E
	lsa	r7,r17,r7,00000002
	lw	r7,0E00(r7)
	bltuc	r6,r7,0040B890

l0040B72E:
	lw	r17,0000(sp)
	addiu	r16,r30,FFFFFFFF
	andi	r22,r16,0000007F
	move	r21,r0
	addiu	r7,r7,FFFFFFE3
	sw	r7,0000(sp)

l0040B740:
	sll	r23,r22,00000002
	addiu	r5,sp,FFFFF230
	addu	r5,r5,r23
	li	r6,3B9ACA00
	lw	r5,0E00(r5)
	move	r7,r0
	li	r3,3B9ACA00
	sll	r18,r5,0000001D
	srl	r5,r5,00000003
	addu	r19,r18,r21
	sltu	r18,r19,r18
	move	r4,r19
	addu	r18,r18,r5
	bnezc	r18,0040B774

l0040B770:
	bgeuc	r3,r19,0040B7A8

l0040B774:
	move.balc	r5,r18,0040EAB0
	li	r6,3B9ACA00
	move	r7,r0
	move	r21,r4
	movep	r4,r5,r19,r18
	balc	0040ED50
	addiu	r6,sp,FFFFF230
	addu	r23,r23,r6
	andi	r7,r16,0000007F
	addiu	r6,r22,FFFFFFFF
	sw	r4,0E00(r23)
	beqc	r22,r7,0040B7C2

l0040B79E:
	beqc	r22,r17,0040B7D6

l0040B7A2:
	andi	r22,r6,0000007F
	bc	0040B740

l0040B7A8:
	addiu	r6,sp,FFFFF230
	move	r4,r19
	addu	r23,r23,r6
	andi	r7,r16,0000007F
	move	r21,r0
	addiu	r6,r22,FFFFFFFF
	sw	r4,0E00(r23)
	bnec	r22,r7,0040B79E

l0040B7C2:
	beqc	r22,r17,0040B7D4

l0040B7C6:
	movz	r30,r7,r4

l0040B7CA:
	andi	r22,r6,0000007F
	addiu	r16,r30,FFFFFFFF
	bc	0040B740

l0040B7D4:
	move	r7,r17

l0040B7D6:
	beqc	r0,r21,0040B710

l0040B7DA:
	addiu	r9,r17,FFFFFFFF
	addiu	r20,r20,00000009
	andi	r17,r9,0000007F
	beqc	r30,r17,0040B7F8

l0040B7EA:
	addiu	r7,sp,FFFFF230
	lsa	r7,r17,r7,00000002
	sw	r21,0E00(r7)
	bc	0040B710

l0040B7F8:
	addiu	r6,r7,FFFFFFFF
	addiu	r5,sp,FFFFF230
	lsa	r5,r7,r5,00000002
	andi	r6,r6,0000007F
	move	r30,r7
	addiu	r7,sp,FFFFF230
	lsa	r7,r6,r7,00000002
	lw	r5,0E00(r5)
	lw	r6,0E00(r7)
	or	r6,r6,r5
	sw	r6,0E00(r7)
	bc	0040B7EA

l0040B822:
	li	r5,0000002E
	bc	0040B4D4

l0040B826:
	lw	r17,0008(sp)
	balc	00410170
	movep	r6,r7,r0,r0
	balc	00404330
	restore.jrc	00000260,r30,0000000A

l0040B836:
	lw	r7,0004(r16)
	lw	r6,0068(r16)
	bgeuc	r7,r6,0040BC40

l0040B840:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r5,0000(r7)

l0040B848:
	bneiuc	r5,00000030,0040BCEC

l0040B84C:
	lw	r6,0068(r16)
	move	r23,r0
	move	r18,r0

l0040B854:
	addiu	r5,r23,FFFFFFFF
	lw	r7,0004(r16)
	sltu	r4,r5,r23
	addiu	r18,r18,FFFFFFFF
	move	r23,r5
	addu	r18,r4,r18
	addiu	r21,r7,00000001
	bgeuc	r7,r6,0040B880

l0040B86C:
	sw	r21,0001(r16)
	lbu	r5,0000(r7)
	beqic	r5,00000030,0040B854

l0040B874:
	li	r7,00000001
	addiu	r21,r0,00000001
	sw	r7,0000(sp)
	bc	0040B434

l0040B880:
	move.balc	r4,r16,0040CB78
	move	r5,r4
	bneiuc	r4,00000030,0040B874

l0040B88A:
	lw	r6,0068(r16)
	bc	0040B854

l0040B890:
	andi	r6,r17,0000007F
	addiu	r18,r17,00000001

l0040B898:
	beqc	r30,r6,0040B97C

l0040B89C:
	addiu	r7,sp,FFFFF230
	li	r5,0089705E
	lsa	r7,r6,r7,00000002
	lw	r7,0E00(r7)
	bgeuc	r5,r7,0040B97C

l0040B8B2:
	li	r5,0089705F
	bnec	r7,r5,0040B8DA

l0040B8BC:
	andi	r7,r18,0000007F
	beqc	r30,r7,0040B97C

l0040B8C4:
	addiu	r5,sp,FFFFF230
	lsa	r7,r7,r5,00000002
	li	r5,0F2F09FF
	lw	r7,0E00(r7)
	bgeuc	r5,r7,0040B97C

l0040B8DA:
	slti	r23,r20,0000001C
	li	r5,1DCD6500
	li	r2,001DCD65
	li	r7,00000001
	movn	r2,r5,r23

l0040B8F0:
	addiu	r11,r0,00000009
	addiu	r5,r0,000001FF
	movn	r11,r7,r23

l0040B8FC:
	movz	r7,r5,r23

l0040B900:
	move	r23,r7
	lw	r17,0000(sp)
	addu	r7,r7,r11
	sw	r7,0000(sp)
	beqc	r17,r30,0040B898

l0040B90C:
	move	r7,r17
	move	r19,r0
	bc	0040B91A

l0040B912:
	andi	r7,r22,0000007F
	beqc	r30,r7,0040B956

l0040B91A:
	addiu	r6,sp,FFFFF230
	addiu	r22,r7,00000001
	lsa	r4,r7,r6,00000002
	lw	r6,0E00(r4)
	srlv	r5,r6,r11
	and	r6,r6,r23
	addu	r5,r5,r19
	mul	r19,r6,r2
	sw	r5,0E00(r4)
	bnec	r7,r17,0040B912

l0040B940:
	bnezc	r5,0040B912

l0040B942:
	andi	r17,r18,0000007F
	andi	r7,r22,0000007F
	addiu	r20,r20,FFFFFFF7
	addiu	r18,r17,00000001
	bnec	r30,r7,0040B91A

l0040B956:
	beqc	r0,r19,0040BAB6

l0040B95A:
	addiu	r6,r30,00000001
	andi	r7,r6,0000007F
	beqc	r17,r7,0040BABC

l0040B966:
	addiu	r6,sp,FFFFF230
	lsa	r5,r30,r6,00000002
	andi	r6,r17,0000007F
	move	r30,r7
	sw	r19,0E00(r5)
	bnec	r30,r6,0040B89C

l0040B97C:
	bneiuc	r20,00000012,0040B8DA

l0040B980:
	beqc	r30,r6,0040BBDC

l0040B984:
	addiu	r7,sp,FFFFF230
	andi	r18,r18,0000007F
	lsa	r6,r6,r7,00000002
	lw	r4,0E00(r6)
	balc	004101D0
	movep	r6,r7,r0,r0
	balc	0040EFC0
	beqc	r18,r30,0040BB8E

l0040B9A2:
	lw	r4,0000(sp)
	lui	r7,00000412
	lw	r4,0004(sp)
	lw	r6,0278(r7)
	addiu	r16,r16,00000035
	lw	r7,027C(r7)
	sw	r16,001C(sp)
	subu	r16,r16,r19
	sw	r16,0028(sp)
	balc	00404330
	addiu	r7,sp,FFFFF230
	lsa	r18,r18,r7,00000002
	movep	r22,r23,r4,r5
	lw	r4,0E00(r18)
	balc	004101D0
	movep	r6,r7,r22,r23
	balc	0040EFC0
	lwm	r6,0008(sp),00000002
	balc	00404330
	lw	r17,0010(sp)
	movep	r22,r23,r4,r5
	bgec	r16,r7,0040BADE

l0040B9E8:
	bltc	r16,r0,0040BCAA

l0040B9EC:
	li	r7,00000001
	sw	r16,0014(sp)
	sw	r7,0018(sp)
	move	r7,r16
	bltic	r7,00000035,0040BB3A

l0040B9F8:
	move	r18,r0
	move	r19,r0
	sw	r0,0020(sp)
	sw	r0,0024(sp)

l0040BA00:
	addiu	r7,r17,00000002
	andi	r7,r7,0000007F
	beqc	r30,r7,0040BA7E

l0040BA0C:
	addiu	r6,sp,FFFFF230
	lsa	r7,r7,r6,00000002
	li	r6,1DCD64FF
	lw	r7,0E00(r7)
	bltuc	r6,r7,0040BC24

l0040BA22:
	bnezc	r7,0040BA30

l0040BA24:
	addiu	r7,r17,00000003
	andi	r7,r7,0000007F
	beqc	r30,r7,0040BA4E

l0040BA30:
	lui	r7,00000412
	lwm	r4,0008(sp),00000002
	lw	r6,0288(r7)
	lw	r7,028C(r7)

l0040BA40:
	balc	00404330
	movep	r6,r7,r4,r5
	movep	r4,r5,r18,r19
	balc	0040EFC0
	movep	r18,r19,r4,r5

l0040BA4E:
	lw	r17,0014(sp)
	li	r7,00000035
	subu	r7,r7,r6
	bltic	r7,00000002,0040BA7E

l0040BA58:
	lui	r7,00000412
	lw	r20,0258(r7)
	lw	r21,025C(r7)
	movep	r4,r5,r18,r19
	movep	r6,r7,r20,r21
	balc	0040CF20
	movep	r6,r7,r0,r0
	balc	0040FA70
	bnezc	r4,0040BA7E

l0040BA74:
	movep	r4,r5,r18,r19
	movep	r6,r7,r20,r21
	balc	0040EFC0
	movep	r18,r19,r4,r5

l0040BA7E:
	lw	r17,0004(sp)
	movep	r4,r5,r22,r23
	subu	r17,r0,r7
	movep	r6,r7,r18,r19
	balc	0040EFC0
	lw	r4,0010(sp)
	lwm	r6,0020(sp),00000002
	balc	0040FAE0
	lw	r17,001C(sp)
	subu	r30,r17,r16
	addiu	r6,r30,FFFFFFFF
	ext	r7,r7,00000000,0000001F
	movep	r20,r21,r4,r5
	bgec	r7,r6,0040BC48

l0040BAAA:
	lw	r17,0000(sp)
	movep	r4,r5,r20,r21
	restore	00000260,r30,0000000A
	bc	0040CFD0

l0040BAB6:
	andi	r6,r17,0000007F
	bc	0040B898

l0040BABC:
	addiu	r7,r30,FFFFFFFF
	addiu	r5,sp,FFFFF230
	andi	r7,r7,0000007F
	andi	r6,r6,0000007F
	lsa	r7,r7,r5,00000002
	lw	r5,0E00(r7)
	ori	r5,r5,00000001
	sw	r5,0E00(r7)
	bc	0040B898

l0040BADE:
	sw	r7,0014(sp)
	sw	r0,0018(sp)
	bgeic	r7,00000035,0040B9F8

l0040BAE6:
	bc	0040BB3A

l0040BAE8:
	sw	r0,0000(sp)
	bc	0040B42A

l0040BAEE:
	lw	r17,0014(sp)
	move.balc	r4,r16,0040B250
	beqc	r0,r4,0040BCCE

l0040BAF8:
	addu	r4,r4,r23
	addu	r5,r18,r5
	sltu	r18,r4,r23
	move	r23,r4
	addu	r18,r18,r5
	bc	0040B4F8

l0040BB08:
	bgeuc	r23,r6,0040B530

l0040BB0C:
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	lui	r16,00000412
	lw	r17,0008(sp)
	balc	00410170
	lw	r6,0280(r16)
	lw	r7,0284(r16)
	balc	00404330
	lw	r6,0280(r16)
	lw	r7,0284(r16)
	balc	00404330
	bc	0040B662

l0040BB3A:
	li	r6,00000069
	addiu	r10,r0,00000035
	subu	r6,r6,r7
	subu	r10,r10,r7

l0040BB46:
	lui	r7,00000412
	sw	r10,002C(sp)
	lw	r18,0258(r7)
	lw	r19,025C(r7)
	movep	r4,r5,r18,r19
	balc	0040CF40
	movep	r6,r7,r22,r23
	balc	0040CF00
	lw	r18,002C(sp)
	movep	r20,r21,r4,r5
	swm	r20,0020(sp),00000002
	move	r6,r10
	movep	r4,r5,r18,r19
	balc	0040CF40
	movep	r6,r7,r4,r5
	movep	r4,r5,r22,r23
	balc	0040CF20
	movep	r18,r19,r4,r5
	movep	r6,r7,r18,r19
	movep	r4,r5,r22,r23
	balc	0040FAE0
	movep	r6,r7,r4,r5
	movep	r4,r5,r20,r21
	balc	0040EFC0
	movep	r22,r23,r4,r5
	bc	0040BA00

l0040BB8E:
	addiu	r8,r30,00000001
	addiu	r7,sp,FFFFF230
	andi	r30,r8,0000007F
	lsa	r7,r30,r7,00000002
	sw	r0,0DFC(r7)
	bc	0040B9A2

l0040BBA4:
	bnec	r18,r17,0040B502

l0040BBA8:
	bltc	r0,r18,0040B502

l0040BBAC:
	bnezc	r18,0040BBB6

l0040BBAE:
	bltiuc	r23,0000000A,0040BBB6

l0040BBB2:
	bc	0040B502

l0040BBB6:
	lw	r17,0010(sp)
	bgeic	r7,0000001F,0040BBC4

l0040BBBC:
	srlv	r7,r16,r7
	bnec	r0,r7,0040B502

l0040BBC4:
	lw	r17,0008(sp)
	balc	00410170
	movep	r18,r19,r4,r5
	move.balc	r4,r16,004101D0

l0040BBD0:
	movep	r6,r7,r4,r5
	movep	r4,r5,r18,r19
	balc	00404330
	bc	0040B662

l0040BBDC:
	addiu	r8,r30,00000001
	addiu	r7,sp,FFFFF230
	andi	r30,r8,0000007F
	lsa	r7,r30,r7,00000002
	sw	r0,0DFC(r7)
	bc	0040B984

l0040BBF2:
	bgeuc	r7,r23,0040B51E

l0040BBF6:
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	lui	r16,00000412
	lw	r17,0008(sp)
	balc	00410170
	lw	r6,0220(r16)
	lw	r7,0224(r16)
	balc	00404330
	lw	r6,0220(r16)
	lw	r7,0224(r16)
	balc	00404330
	restore.jrc	00000260,r30,0000000A

l0040BC24:
	li	r6,1DCD6500
	beqc	r6,r7,0040BD34

l0040BC2E:
	lwm	r4,0008(sp),00000002

l0040BC32:
	lui	r7,00000412
	lw	r6,0290(r7)
	lw	r7,0294(r7)
	bc	0040BA40

l0040BC40:
	move.balc	r4,r16,0040CB78
	move	r5,r4
	bc	0040B848

l0040BC48:
	balc	0040CF10
	lui	r7,00000412
	lw	r6,0240(r7)
	lw	r7,0244(r7)
	balc	004041D0
	bltc	r4,r0,0040BC82

l0040BC60:
	lw	r17,0018(sp)
	bnezc	r7,0040BCB8

l0040BC64:
	move	r17,r0

l0040BC66:
	lui	r7,00000412
	lw	r4,0000(sp)
	lw	r6,0250(r7)
	lw	r7,0254(r7)
	addiu	r16,r16,00000001
	movep	r4,r5,r20,r21
	sw	r16,0000(sp)
	sw	r17,0018(sp)
	balc	00404330
	movep	r20,r21,r4,r5

l0040BC82:
	lw	r17,0000(sp)
	addiu	r30,r30,00000003
	addiu	r7,r7,00000034
	bgec	r7,r30,0040BCA0

l0040BC8E:
	lw	r17,0018(sp)
	beqc	r0,r7,0040BAAA

l0040BC94:
	movep	r6,r7,r0,r0
	movep	r4,r5,r18,r19
	balc	0040FA70
	beqc	r0,r4,0040BAAA

l0040BCA0:
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	bc	0040BAAA

l0040BCAA:
	li	r7,00000001
	addiu	r10,r0,00000035
	li	r6,00000069
	sw	r0,0014(sp)
	sw	r7,0018(sp)
	bc	0040BB46

l0040BCB8:
	lw	r17,0014(sp)
	lw	r17,0028(sp)
	xor	r17,r7,r6
	sltu	r17,r0,r17
	bc	0040BC66

l0040BCC6:
	addiu	r17,r17,00000009
	bc	0040B696

l0040BCCE:
	lui	r7,FFF80000
	bnec	r7,r5,0040BAF8

l0040BCD6:
	lw	r17,0014(sp)
	beqc	r0,r7,0040B65A

l0040BCDC:
	lw	r7,0068(r16)
	beqzc	r7,0040BD58

l0040BCE2:
	lw	r7,0004(r16)
	move	r5,r0
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)
	bc	0040BAF8

l0040BCEC:
	addiu	r21,r0,00000001
	move	r23,r0
	move	r18,r0
	bc	0040B434

l0040BCF8:
	move.balc	r4,r16,004101D0
	lwm	r6,0008(sp),00000002
	balc	00404330
	li	r7,00000008
	subu	r23,r7,r23
	addiupc	r7,00007532
	movep	r16,r17,r4,r5
	lwxs	r4,r23(r7)
	balc	00410170
	movep	r6,r7,r4,r5
	movep	r4,r5,r16,r17
	balc	0040F5E0
	bc	0040B662

l0040BD24:
	move.balc	r4,r16,004101D0
	lwm	r6,0008(sp),00000002
	balc	00404330
	bc	0040B662

l0040BD34:
	addiu	r7,r17,00000003
	lwm	r4,0008(sp),00000002
	andi	r7,r7,0000007F
	bnec	r30,r7,0040BC32

l0040BD44:
	lui	r7,00000412
	lw	r6,0250(r7)
	lw	r7,0254(r7)
	bc	0040BA40

l0040BD52:
	move	r18,r0
	bc	0040B704

l0040BD58:
	move	r5,r0
	bc	0040BAF8

;; __floatscan: 0040BD5C
;;   Called from:
;;     00409F78 (in strtox)
;;     0040DAFC (in __isoc99_vfscanf)
__floatscan proc
	save	00000080,r30,0000000A
	move	r21,r4
	move	r16,r6
	beqc	r0,r5,0040BEA8

l0040BD66:
	bltc	r5,r0,0040BEA0

l0040BD6A:
	bgeic	r5,00000003,0040BEA0

l0040BD6E:
	li	r7,00000035
	addiu	r19,r0,FFFFFBCE
	sw	r7,0008(sp)
	addiu	r7,r0,FFFFFB64
	sw	r7,0018(sp)
	addiu	r7,r0,00000432
	addiu	r20,r0,FFFFFFFF
	move	r30,r0
	addiu	r18,r0,FFFFFBCE
	li	r17,00000035
	sw	r0,0014(sp)
	sw	r7,001C(sp)

l0040BD90:
	lw	r7,0001(r21)
	lw	r6,0068(r21)
	addiu	r5,r7,00000001
	bgeuc	r7,r6,0040BE96

l0040BD9E:
	sw	r5,0001(r21)
	lbu	r4,0000(r7)
	beqic	r4,00000020,0040BD90

l0040BDA6:
	addiu	r7,r4,FFFFFFF7
	bltiuc	r7,00000005,0040BD90

l0040BDAE:
	move	r5,r4
	beqic	r4,0000002B,0040C00A

l0040BDB4:
	beqic	r5,0000002D,0040BEEA

l0040BDB8:
	lui	r7,00000412
	addiu	r8,r0,00000001
	lw	r22,02A0(r7)
	lw	r23,02A4(r7)

l0040BDC8:
	ori	r7,r5,00000020
	bneiuc	r7,00000029,0040BF0E

l0040BDD0:
	lw	r6,0001(r21)
	lw	r7,0068(r21)
	bgeuc	r6,r7,0040BED6

l0040BDDA:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BDE2:
	ori	r4,r4,00000020
	bneiuc	r4,0000002E,0040BE78

l0040BDEA:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BEE0

l0040BDF0:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BDF8:
	ori	r4,r4,00000020
	bneiuc	r4,00000026,0040BE78

l0040BE00:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BFB6

l0040BE06:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BE0E:
	ori	r4,r4,00000020
	bneiuc	r4,00000029,0040C048

l0040BE16:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BFC0

l0040BE1C:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BE24:
	ori	r4,r4,00000020
	bneiuc	r4,0000002E,0040C0CC

l0040BE2C:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C01C

l0040BE32:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BE3A:
	ori	r4,r4,00000020
	bneiuc	r4,00000029,0040C0D4

l0040BE42:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BECC

l0040BE48:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BE50:
	ori	r4,r4,00000020
	bneiuc	r4,00000034,0040C0DC

l0040BE58:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C026

l0040BE5E:
	addiu	r7,r6,00000001
	sw	r7,0001(r21)
	lbu	r4,0000(r6)

l0040BE66:
	ori	r4,r4,00000020
	beqic	r4,00000039,0040BEA4

l0040BE6E:
	li	r5,00000007
	lw	r7,0068(r21)
	bnec	r0,r16,0040C04A

l0040BE78:
	beqzc	r7,0040BE80

l0040BE7A:
	lw	r7,0001(r21)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)

l0040BE80:
	balc	004049B0
	li	r7,00000016
	move	r22,r0
	move	r23,r0
	sw	r7,0000(sp)
	movep	r6,r7,r0,r0
	move.balc	r4,r21,0040CB40
	movep	r4,r5,r22,r23
	restore.jrc	00000080,r30,0000000A

l0040BE96:
	move.balc	r4,r21,0040CB78
	beqic	r4,00000020,0040BD90

l0040BE9E:
	bc	0040BDA6

l0040BEA0:
	move	r22,r0
	move	r23,r0

l0040BEA4:
	movep	r4,r5,r22,r23
	restore.jrc	00000080,r30,0000000A

l0040BEA8:
	li	r7,00000018
	addiu	r19,r0,FFFFFF6B
	sw	r7,0008(sp)
	addiu	r7,r0,FFFFFF01
	sw	r7,0018(sp)
	addiu	r7,r0,00000095
	addiu	r20,r0,FFFFFFFF
	move	r30,r0
	addiu	r18,r0,FFFFFF6B
	li	r17,00000018
	sw	r0,0014(sp)
	sw	r7,001C(sp)
	bc	0040BD90

l0040BECC:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BE50

l0040BED6:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BDE2

l0040BEE0:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BDF8

l0040BEEA:
	lui	r7,00000412
	addiu	r8,r0,FFFFFFFF
	lw	r22,0298(r7)
	lw	r23,029C(r7)

l0040BEFA:
	lw	r7,0001(r21)
	lw	r6,0068(r21)
	bgeuc	r7,r6,0040BFFE

l0040BF04:
	addiu	r6,r7,00000001
	sw	r6,0001(r21)
	lbu	r5,0000(r7)
	bc	0040BDC8

l0040BF0E:
	bneiuc	r7,0000002E,0040BFCA

l0040BF12:
	lw	r6,0001(r21)
	lw	r7,0068(r21)
	bgeuc	r6,r7,0040C0C2

l0040BF1C:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BF24:
	ori	r6,r4,00000020
	bneiuc	r6,00000021,0040BE78

l0040BF2C:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C02C

l0040BF32:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)

l0040BF3A:
	ori	r4,r4,00000020
	bneiuc	r4,0000002E,0040BE78

l0040BF42:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C0B0

l0040BF48:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r6,0000(r6)
	xori	r6,r6,00000028
	sltu	r6,r0,r6

l0040BF58:
	bnec	r0,r6,0040C098

l0040BF5C:
	lw	r6,0001(r21)
	li	r17,00000001
	bgeuc	r6,r7,0040C092

l0040BF64:
	addiu	r7,r6,00000001
	sw	r7,0001(r21)
	lbu	r4,0000(r6)

l0040BF6C:
	move	r7,r4
	addiu	r6,r4,FFFFFFD0
	ins	r7,r0,00000005,00000001
	bltiuc	r6,0000000A,0040C086

l0040BF7A:
	addiu	r7,r7,FFFFFFBF
	bltiuc	r7,0000001A,0040C086

l0040BF82:
	beqic	r4,0000001F,0040C086

l0040BF86:
	beqic	r4,00000029,0040BFA8

l0040BF8A:
	lw	r7,0068(r21)
	beqc	r0,r7,0040C0E4

l0040BF92:
	lw	r7,0001(r21)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)
	bnezc	r16,0040BFA0

l0040BF9A:
	bc	0040BE80

l0040BF9C:
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)

l0040BFA0:
	addiu	r17,r17,FFFFFFFF
	li	r6,FFFFFFFF
	bnec	r6,r17,0040BF9C

l0040BFA8:
	lui	r7,00000412
	lw	r22,02A8(r7)
	lw	r23,02AC(r7)
	bc	0040BEA4

l0040BFB6:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BE0E

l0040BFC0:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BE24

l0040BFCA:
	bneiuc	r5,00000030,0040BFF0

l0040BFCE:
	lw	r6,0001(r21)
	lw	r7,0068(r21)
	bgeuc	r6,r7,0040C036

l0040BFD8:
	addiu	r4,r6,00000001
	sw	r4,0001(r21)
	lbu	r4,0000(r6)

l0040BFE0:
	ori	r6,r4,00000020
	beqic	r6,00000038,0040C0EA

l0040BFE8:
	beqzc	r7,0040BFF0

l0040BFEA:
	lw	r7,0001(r21)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)

l0040BFF0:
	move	r9,r16
	move	r4,r21
	movep	r6,r7,r17,r18
	restore	00000080,r30,0000000A
	bc	0040B3FA

l0040BFFE:
	sw	r8,0030(sp)
	move.balc	r4,r21,0040CB78
	lw	r18,0030(sp)
	move	r5,r4
	bc	0040BDC8

l0040C00A:
	lui	r7,00000412
	addiu	r8,r0,00000001
	lw	r22,02A0(r7)
	lw	r23,02A4(r7)
	bc	0040BEFA

l0040C01C:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BE3A

l0040C026:
	move.balc	r4,r21,0040CB78
	bc	0040BE66

l0040C02C:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BF3A

l0040C036:
	sw	r5,0020(sp)
	sw	r8,0030(sp)
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	lw	r17,0020(sp)
	lw	r18,0030(sp)
	bc	0040BFE0

l0040C048:
	li	r5,00000003

l0040C04A:
	beqzc	r7,0040C052

l0040C04C:
	lw	r6,0001(r21)
	addiu	r6,r6,FFFFFFFF
	sw	r6,0001(r21)

l0040C052:
	beqc	r0,r16,0040BEA4

l0040C056:
	beqic	r5,00000003,0040BEA4

l0040C05A:
	beqc	r0,r7,0040BEA4

l0040C05E:
	lw	r7,0001(r21)
	addiu	r6,r7,FFFFFFFF
	sw	r6,0001(r21)
	beqic	r5,00000004,0040BEA4

l0040C06A:
	addiu	r6,r7,FFFFFFFE
	sw	r6,0001(r21)
	beqic	r5,00000005,0040BEA4

l0040C074:
	addiu	r6,r7,FFFFFFFD
	sw	r6,0001(r21)
	bneiuc	r5,00000007,0040BEA4

l0040C07E:
	addiu	r7,r7,FFFFFFFC
	sw	r7,0001(r21)
	movep	r4,r5,r22,r23
	restore.jrc	00000080,r30,0000000A

l0040C086:
	lw	r7,0068(r21)
	addiu	r17,r17,00000001
	lw	r6,0001(r21)
	bltuc	r6,r7,0040BF64

l0040C092:
	move.balc	r4,r21,0040CB78
	bc	0040BF6C

l0040C098:
	beqc	r0,r7,0040BFA8

l0040C09C:
	lw	r7,0001(r21)
	lui	r6,00000412
	lw	r22,02A8(r6)
	addiu	r7,r7,FFFFFFFF
	lw	r23,02AC(r6)
	sw	r7,0001(r21)
	bc	0040BEA4

l0040C0B0:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	xori	r6,r4,00000028
	sltu	r6,r0,r6
	bc	0040BF58

l0040C0C2:
	move.balc	r4,r21,0040CB78
	lw	r7,0068(r21)
	bc	0040BF24

l0040C0CC:
	li	r5,00000004
	bnec	r0,r16,0040C04A

l0040C0D2:
	bc	0040BE78

l0040C0D4:
	li	r5,00000005
	bnec	r0,r16,0040C04A

l0040C0DA:
	bc	0040BE78

l0040C0DC:
	li	r5,00000006
	bnec	r0,r16,0040C04A

l0040C0E2:
	bc	0040BE78

l0040C0E4:
	bnec	r0,r16,0040BFA8

l0040C0E8:
	bc	0040BE80

l0040C0EA:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C4E2

l0040C0F0:
	addiu	r7,r6,00000001
	sw	r7,0001(r21)
	lbu	r4,0000(r6)

l0040C0F8:
	bneiuc	r4,00000030,0040C4EC

l0040C0FC:
	lw	r7,0068(r21)
	move	r22,r8

l0040C102:
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C242

l0040C108:
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	beqic	r4,00000030,0040C102

l0040C114:
	move	r8,r22
	li	r6,00000001

l0040C118:
	beqic	r4,0000002E,0040C464

l0040C11C:
	sw	r0,0030(sp)
	sw	r0,0034(sp)
	sw	r0,0038(sp)

l0040C122:
	lui	r7,00000412
	move	r22,r0
	move	r11,r0
	move	r23,r0
	sw	r0,0020(sp)
	sw	r0,0024(sp)
	sw	r0,0048(sp)
	sw	r7,004C(sp)
	lw	r2,0258(r7)
	lw	r3,025C(r7)
	swm	r2,0028(sp),00000002

l0040C140:
	addiu	r9,r4,FFFFFFD0
	bltiuc	r9,0000000A,0040C180

l0040C148:
	ori	r5,r4,00000020
	addiu	r7,r5,FFFFFF9F
	bltiuc	r7,00000006,0040C180

l0040C154:
	bneiuc	r4,0000002E,0040C442

l0040C158:
	lw	r17,0030(sp)
	bnec	r0,r7,0040C250

l0040C15E:
	li	r7,00000001
	sw	r22,0034(sp)
	sw	r7,0030(sp)
	sw	r11,0038(sp)

l0040C166:
	lw	r7,0001(r21)
	lw	r5,0068(r21)
	bgeuc	r7,r5,0040C230

l0040C170:
	addiu	r5,r7,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r7)
	addiu	r9,r4,FFFFFFD0
	bgeiuc	r9,0000000A,0040C148

l0040C180:
	beqic	r4,0000002E,0040C158

l0040C184:
	bltic	r4,0000003A,0040C190

l0040C188:
	ori	r4,r4,00000020
	addiu	r9,r4,FFFFFFA9

l0040C190:
	bltc	r0,r11,0040C1EE

l0040C194:
	bnec	r0,r11,0040C228

l0040C198:
	bltiuc	r22,00000008,0040C228

l0040C19C:
	bgeiuc	r22,0000000E,0040C1EE

l0040C1A0:
	lui	r7,00000412
	sw	r8,0040(sp)
	lw	r6,02B0(r7)
	lwm	r4,0028(sp),00000002
	lw	r7,02B4(r7)
	sw	r11,0044(sp)
	sw	r9,003C(sp)
	balc	00404330
	lw	r18,003C(sp)
	swm	r4,0028(sp),00000002
	move.balc	r4,r9,00410170
	lwm	r6,0028(sp),00000002
	balc	00404330
	movep	r6,r7,r4,r5
	lwm	r4,0020(sp),00000002
	balc	0040EFC0
	lw	r18,0040(sp)
	lw	r18,0044(sp)
	swm	r4,0020(sp),00000002

l0040C1DE:
	addiu	r7,r22,00000001
	li	r6,00000001
	sltu	r5,r7,r22
	move	r22,r7
	addu	r11,r11,r5
	bc	0040C166

l0040C1EE:
	beqc	r0,r9,0040C1DE

l0040C1F2:
	lw	r17,0048(sp)
	bnezc	r7,0040C1DE

l0040C1F6:
	lui	r7,00000412
	sw	r8,003C(sp)
	lw	r6,0250(r7)
	addiu	r8,r0,00000001
	lw	r7,0254(r7)
	lwm	r4,0028(sp),00000002
	sw	r11,0040(sp)
	sw	r8,0048(sp)
	balc	00404330
	movep	r6,r7,r4,r5
	lwm	r4,0020(sp),00000002
	balc	0040EFC0
	lw	r18,003C(sp)
	lw	r18,0040(sp)
	swm	r4,0020(sp),00000002
	bc	0040C1DE

l0040C228:
	sll	r23,r23,00000004
	addu	r23,r23,r9
	bc	0040C1DE

l0040C230:
	sw	r8,003C(sp)
	sw	r6,0040(sp)
	sw	r11,0044(sp)
	move.balc	r4,r21,0040CB78
	lw	r18,003C(sp)
	lw	r17,0040(sp)
	lw	r18,0044(sp)
	bc	0040C140

l0040C242:
	move.balc	r4,r21,0040CB78
	bneiuc	r4,00000030,0040C114

l0040C24A:
	lw	r7,0068(r21)
	bc	0040C102

l0040C250:
	beqc	r0,r6,0040C410

l0040C254:
	bltc	r0,r11,0040C2DE

l0040C258:
	beqc	r0,r11,0040C62C

l0040C25C:
	li	r5,0000002E

l0040C25E:
	addiu	r7,r22,00000001
	sll	r23,r23,00000004
	sltu	r6,r7,r22
	addu	r6,r6,r11
	beqic	r7,00000008,0040C600

l0040C270:
	addiu	r7,r22,00000002
	sll	r23,r23,00000004
	sltu	r6,r7,r22
	addu	r6,r6,r11
	beqic	r7,00000008,0040C626

l0040C282:
	addiu	r7,r22,00000003
	sll	r23,r23,00000004
	sltu	r6,r7,r22
	addu	r6,r6,r11
	beqic	r7,00000008,0040C620

l0040C294:
	addiu	r7,r22,00000004
	sll	r23,r23,00000004
	sltu	r6,r7,r22
	addu	r6,r6,r11
	beqic	r7,00000008,0040C61A

l0040C2A6:
	addiu	r7,r22,00000005
	sll	r23,r23,00000004
	sltu	r6,r7,r22
	addu	r6,r6,r11
	beqic	r7,00000008,0040C614

l0040C2B8:
	addiu	r7,r22,00000006
	sll	r23,r23,00000004
	sltu	r6,r7,r22
	addu	r6,r6,r11
	beqic	r7,00000008,0040C5FA

l0040C2CA:
	or	r11,r22,r11
	sll	r23,r23,00000004
	bnec	r0,r11,0040C2DA

l0040C2D6:
	sll	r23,r23,00000004

l0040C2DA:
	beqic	r5,00000030,0040C5D2

l0040C2DE:
	lw	r7,0068(r21)
	beqc	r0,r7,0040C5CC

l0040C2E6:
	lw	r7,0001(r21)
	movep	r4,r5,r0,r0
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)

l0040C2EE:
	beqc	r0,r23,0040C432

l0040C2F2:
	lw	r17,0034(sp)
	lw	r17,0038(sp)
	addiu	r16,r7,FFFFFFF8
	addiu	r9,r6,FFFFFFFF
	sltu	r6,r16,r7
	addu	r6,r6,r9
	sll	r7,r16,00000002
	sll	r6,r6,00000002
	srl	r16,r16,0000001E
	addu	r4,r7,r4
	or	r6,r6,r16
	sltu	r7,r4,r7
	addu	r6,r6,r5
	addu	r7,r7,r6
	move	r16,r4
	move	r21,r7
	bltc	r30,r7,0040C59A

l0040C320:
	beqc	r0,r7,0040C594

l0040C324:
	bltc	r7,r20,0040C51E

l0040C328:
	beqc	r20,r7,0040C518

l0040C32C:
	bltc	r23,r0,0040C390

l0040C330:
	sw	r17,0018(sp)
	move	r17,r16
	lw	r5,0020(sp)
	move	r16,r23
	lw	r5,0024(sp)
	lui	r20,00000412
	move	r30,r8

l0040C340:
	lw	r6,0250(r20)
	sll	r16,r16,00000001
	lw	r7,0254(r20)
	movep	r4,r5,r22,r23
	balc	004041D0
	movep	r6,r7,r22,r23
	bltc	r4,r0,0040C36A

l0040C356:
	lw	r17,004C(sp)
	addiu	r16,r16,00000001
	movep	r4,r5,r22,r23
	lw	r6,0258(r7)
	lw	r7,025C(r7)
	balc	0040FAE0
	movep	r6,r7,r4,r5

l0040C36A:
	movep	r4,r5,r22,r23
	balc	0040EFC0
	addiu	r7,r17,FFFFFFFF
	sltu	r6,r7,r17
	addiu	r21,r21,FFFFFFFF
	movep	r22,r23,r4,r5
	move	r17,r7
	addu	r21,r21,r6
	bgec	r16,r0,0040C340

l0040C384:
	lw	r4,0018(sp)
	swm	r22,0020(sp),00000002
	move	r8,r30
	move	r23,r16
	move	r16,r7

l0040C390:
	addiu	r6,r16,00000020
	sltu	r7,r6,r16
	subu	r19,r6,r19
	addu	r7,r7,r21
	sltu	r6,r6,r19
	addiu	r7,r7,00000001
	subu	r7,r7,r6
	bltc	r7,r0,0040C3B2

l0040C3A8:
	lw	r17,0014(sp)
	bnec	r6,r7,0040C3BC

l0040C3AC:
	lw	r17,0008(sp)
	bgeuc	r19,r7,0040C3BC

l0040C3B2:
	subu	r17,r16,r18
	addiu	r17,r17,00000020
	bltc	r17,r0,0040C550

l0040C3BC:
	bltic	r17,00000035,0040C63C

l0040C3C0:
	move	r18,r0
	move	r19,r0

l0040C3C4:
	move.balc	r4,r8,00410170
	lwm	r6,0020(sp),00000002
	movep	r8,r9,r4,r5
	swm	r8,0008(sp),00000002
	balc	00404330
	movep	r20,r21,r4,r5
	move.balc	r4,r23,004101D0
	lwm	r8,0008(sp),00000002
	movep	r6,r7,r8,r9
	balc	00404330
	movep	r6,r7,r18,r19
	balc	0040EFC0
	movep	r6,r7,r20,r21
	balc	0040EFC0
	movep	r6,r7,r18,r19
	balc	0040FAE0
	movep	r6,r7,r0,r0
	movep	r18,r19,r4,r5
	balc	0040FA70
	beqc	r0,r4,0040C632

l0040C404:
	move	r6,r16
	movep	r4,r5,r18,r19
	restore	00000080,r30,0000000A
	bc	0040CFD0

l0040C410:
	lw	r7,0068(r21)
	beqc	r0,r7,0040C4F0

l0040C418:
	lw	r7,0001(r21)
	addiu	r6,r7,FFFFFFFF
	sw	r6,0001(r21)
	beqc	r0,r16,0040C4F4

l0040C424:
	addiu	r6,r7,FFFFFFFE
	sw	r6,0001(r21)
	lw	r17,0030(sp)
	beqzc	r6,0040C432

l0040C42E:
	addiu	r7,r7,FFFFFFFD
	sw	r7,0001(r21)

l0040C432:
	move.balc	r4,r8,00410170
	movep	r6,r7,r0,r0
	balc	00404330
	movep	r22,r23,r4,r5
	bc	0040BEA4

l0040C442:
	beqzc	r6,0040C410

l0040C444:
	lwm	r6,0030(sp),00000002
	movz	r7,r22,r6

l0040C44C:
	sw	r7,0034(sp)
	lw	r17,0038(sp)
	movz	r7,r11,r6

l0040C454:
	sw	r7,0038(sp)
	bltc	r0,r11,0040C2DA

l0040C45A:
	bnec	r0,r11,0040C25E

l0040C45E:
	bltiuc	r22,00000008,0040C25E

l0040C462:
	bc	0040C2DA

l0040C464:
	lw	r5,0001(r21)
	lw	r7,0068(r21)
	bgeuc	r5,r7,0040C50A

l0040C46E:
	addiu	r7,r5,00000001
	sw	r7,0001(r21)
	lbu	r4,0000(r5)

l0040C476:
	bneiuc	r4,00000030,0040C500

l0040C47A:
	lw	r7,0068(r21)
	move	r22,r0
	sw	r30,0030(sp)
	move	r23,r0
	move	r30,r20
	move	r20,r19
	move	r19,r16
	move	r16,r8

l0040C48C:
	lw	r6,0001(r21)
	addiu	r5,r22,FFFFFFFF
	sltu	r9,r5,r22
	addiu	r4,r23,FFFFFFFF
	bgeuc	r6,r7,0040C4C6

l0040C49E:
	addiu	r23,r6,00000001
	move	r22,r5
	sw	r23,0001(r21)
	addu	r23,r9,r4
	lbu	r4,0000(r6)
	beqic	r4,00000030,0040C48C

l0040C4B0:
	li	r7,00000001
	move	r8,r16
	li	r6,00000001
	move	r16,r19
	swm	r22,0034(sp),00000002
	move	r19,r20
	move	r20,r30
	lw	r7,0030(sp)
	sw	r7,0030(sp)
	bc	0040C122

l0040C4C6:
	addiu	r23,r23,FFFFFFFF
	move.balc	r4,r21,0040CB78
	addiu	r7,r22,FFFFFFFF
	sltu	r6,r7,r22
	move	r22,r7
	addu	r23,r23,r6
	bneiuc	r4,00000030,0040C4B0

l0040C4DC:
	lw	r7,0068(r21)
	bc	0040C48C

l0040C4E2:
	sw	r8,0030(sp)
	move.balc	r4,r21,0040CB78
	lw	r18,0030(sp)
	bc	0040C0F8

l0040C4EC:
	move	r6,r0
	bc	0040C118

l0040C4F0:
	bnec	r0,r16,0040C432

l0040C4F4:
	movep	r6,r7,r0,r0
	sw	r8,0008(sp)
	move.balc	r4,r21,0040CB40
	lw	r18,0008(sp)
	bc	0040C432

l0040C500:
	li	r7,00000001
	sw	r0,0034(sp)
	sw	r7,0030(sp)
	sw	r0,0038(sp)
	bc	0040C122

l0040C50A:
	sw	r6,0020(sp)
	sw	r8,0030(sp)
	move.balc	r4,r21,0040CB78
	lw	r17,0020(sp)
	lw	r18,0030(sp)
	bc	0040C476

l0040C518:
	lw	r17,0018(sp)
	bgeuc	r4,r7,0040C32C

l0040C51E:
	sw	r8,0008(sp)
	balc	004049B0
	lw	r18,0008(sp)
	li	r7,00000022
	lui	r16,00000412
	sw	r7,0000(sp)
	move.balc	r4,r8,00410170
	lw	r6,0280(r16)
	lw	r7,0284(r16)
	balc	00404330
	lw	r6,0280(r16)
	lw	r7,0284(r16)
	balc	00404330
	movep	r22,r23,r4,r5
	bc	0040BEA4

l0040C550:
	lw	r17,004C(sp)
	li	r6,00000054
	sw	r8,0008(sp)
	lw	r4,0258(r7)
	lw	r5,025C(r7)
	balc	0040CF40
	lw	r18,0008(sp)
	movep	r18,r19,r4,r5
	move.balc	r4,r8,00410170
	movep	r6,r7,r4,r5
	movep	r4,r5,r18,r19
	balc	0040CF00
	lw	r18,0008(sp)
	movep	r18,r19,r4,r5

l0040C576:
	movep	r6,r7,r0,r0
	lwm	r4,0020(sp),00000002
	sw	r8,0008(sp)
	balc	0040FA70
	lw	r18,0008(sp)
	beqc	r0,r4,0040C3C4

l0040C588:
	bbnezc	r23,00000000,0040C3C4

l0040C58C:
	addiu	r23,r23,00000001
	sw	r0,0020(sp)
	sw	r0,0024(sp)
	bc	0040C3C4

l0040C594:
	lw	r17,001C(sp)
	bgeuc	r6,r4,0040C324

l0040C59A:
	sw	r8,0008(sp)
	balc	004049B0
	lw	r18,0008(sp)
	li	r7,00000022
	lui	r16,00000412
	sw	r7,0000(sp)
	move.balc	r4,r8,00410170
	lw	r6,0220(r16)
	lw	r7,0224(r16)
	balc	00404330
	lw	r6,0220(r16)
	lw	r7,0224(r16)
	balc	00404330
	movep	r22,r23,r4,r5
	bc	0040BEA4

l0040C5CC:
	move	r4,r0

l0040C5CE:
	move	r5,r0
	bc	0040C2EE

l0040C5D2:
	movep	r4,r5,r21,r16
	sw	r8,0030(sp)
	balc	0040B250
	lw	r18,0030(sp)
	bnec	r0,r4,0040C2EE

l0040C5E0:
	lui	r7,FFF80000
	bnec	r7,r5,0040C2EE

l0040C5E8:
	beqzc	r16,0040C606

l0040C5EA:
	lw	r7,0068(r21)
	beqzc	r7,0040C5CE

l0040C5F0:
	lw	r7,0001(r21)
	move	r5,r0
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)
	bc	0040C2EE

l0040C5FA:
	bnec	r0,r6,0040C2CA

l0040C5FE:
	bc	0040C2DA

l0040C600:
	bnec	r0,r6,0040C270

l0040C604:
	bc	0040C2DA

l0040C606:
	movep	r6,r7,r0,r0
	move.balc	r4,r21,0040CB40
	move	r22,r0
	move	r23,r0
	bc	0040BEA4

l0040C614:
	bnec	r0,r6,0040C2B8

l0040C618:
	bc	0040C2DA

l0040C61A:
	bnec	r0,r6,0040C2A6

l0040C61E:
	bc	0040C2DA

l0040C620:
	bnec	r0,r6,0040C294

l0040C624:
	bc	0040C2DA

l0040C626:
	bnec	r0,r6,0040C282

l0040C62A:
	bc	0040C2DA

l0040C62C:
	bltiuc	r22,00000008,0040C25C

l0040C630:
	bc	0040C2DE

l0040C632:
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	bc	0040C404

l0040C63C:
	lw	r17,004C(sp)
	li	r6,00000054
	subu	r6,r6,r17
	sw	r8,0008(sp)
	lw	r4,0258(r7)
	lw	r5,025C(r7)
	balc	0040CF40
	lw	r18,0008(sp)
	movep	r18,r19,r4,r5
	move.balc	r4,r8,00410170
	movep	r6,r7,r4,r5
	movep	r4,r5,r18,r19
	balc	0040CF00
	lw	r18,0008(sp)
	movep	r18,r19,r4,r5
	bltic	r17,00000020,0040C576

l0040C668:
	bc	0040C3C4
0040C66A                               00 00 00 00 00 00           ......

;; __intscan: 0040C670
;;   Called from:
;;     00409FF4 (in strtox)
;;     0040D858 (in __isoc99_vfscanf)
__intscan proc
	save	00000050,r30,0000000A
	sw	r9,0004(sp)
	sw	r8,0008(sp)
	bgeiuc	r5,00000025,0040C736

l0040C67A:
	move	r30,r5
	beqic	r5,00000001,0040C736

l0040C680:
	lw	r19,0068(r4)
	move	r16,r4
	move	r17,r6

l0040C688:
	lw	r7,0004(r16)
	bgeuc	r7,r19,0040C708

l0040C68E:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)
	beqic	r4,00000020,0040C688

l0040C69A:
	addiu	r7,r4,FFFFFFF7
	bltiuc	r7,00000005,0040C688

l0040C6A2:
	beqic	r4,0000002B,0040C742

l0040C6A6:
	beqic	r4,0000002D,0040C742

l0040C6AA:
	move	r23,r0

l0040C6AC:
	beqc	r0,r30,0040C716

l0040C6B0:
	beqic	r30,00000010,0040C95A

l0040C6B4:
	addiupc	r22,00006BB3
	lbux	r7,r4(r22)
	bgeuc	r7,r30,0040C728

l0040C6C2:
	addiupc	r22,00006BA5
	beqic	r30,0000000A,0040C75A

l0040C6CC:
	addiu	r7,r30,FFFFFFFF
	lbux	r20,r4(r22)
	and	r7,r7,r30
	beqc	r0,r7,0040C9A2

l0040C6DC:
	move	r17,r0

l0040C6DE:
	mul	r6,r17,r30
	li	r7,071C71C6
	bgeuc	r20,r30,0040C8A2

l0040C6EC:
	bltuc	r7,r17,0040C8A2

l0040C6F0:
	lw	r7,0004(r16)
	addu	r17,r20,r6
	addiu	r5,r7,00000001
	bgeuc	r7,r19,0040C808

l0040C6FE:
	sw	r5,0004(sp)
	lbu	r4,0000(r7)
	lbux	r20,r4(r22)
	bc	0040C6DE

l0040C708:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	beqic	r4,00000020,0040C688

l0040C714:
	bc	0040C69A

l0040C716:
	beqic	r4,00000030,0040C95E

l0040C71A:
	addiupc	r7,00006B4D
	lbux	r7,r7(r4)
	bltiuc	r7,0000000A,0040C75A

l0040C728:
	beqzc	r19,0040C730

l0040C72A:
	lw	r7,0004(r16)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)

l0040C730:
	movep	r6,r7,r0,r0
	move.balc	r4,r16,0040CB40

l0040C736:
	balc	004049B0
	li	r7,00000016
	sw	r7,0000(sp)

l0040C73E:
	movep	r4,r5,r0,r0
	restore.jrc	00000050,r30,0000000A

l0040C742:
	lw	r7,0004(r16)
	seqi	r4,r4,0000002D
	subu	r23,r0,r4
	bgeuc	r7,r19,0040C98A

l0040C750:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)
	bc	0040C6AC

l0040C75A:
	move	r17,r0

l0040C75C:
	lsa	r7,r17,r17,00000002
	addiu	r6,r4,FFFFFFD0
	li	r5,19999998
	lsa	r7,r7,r4,00000001
	bgeiuc	r6,0000000A,0040C79E

l0040C772:
	bltuc	r5,r17,0040C79E

l0040C776:
	lw	r6,0004(r16)
	addiu	r17,r7,FFFFFFD0
	bgeuc	r6,r19,0040C824

l0040C780:
	addiu	r7,r6,00000001
	li	r5,19999998
	sw	r7,0004(sp)
	lsa	r7,r17,r17,00000002
	lbu	r4,0000(r6)
	addiu	r6,r4,FFFFFFD0
	lsa	r7,r7,r4,00000001
	bltiuc	r6,0000000A,0040C772

l0040C79E:
	move	r18,r0

l0040C7A0:
	bgeiuc	r6,0000000A,0040C846

l0040C7A4:
	li	r7,19999999
	bltuc	r7,r18,0040C838

l0040C7AE:
	beqc	r18,r7,0040C82E

l0040C7B2:
	sll	r7,r17,00000002
	srl	r30,r17,0000001E
	sll	r20,r18,00000002
	addu	r5,r7,r17
	or	r20,r20,r30
	sltu	r7,r5,r7
	addu	r20,r20,r18
	srl	r8,r5,0000001F
	addu	r7,r7,r20
	sra	r20,r6,0000001F
	sll	r7,r7,00000001
	nor	r30,r0,r20
	or	r7,r7,r8
	sll	r5,r5,00000001
	nor	r8,r0,r6
	bltuc	r30,r7,0040C838

l0040C7E6:
	beqc	r7,r30,0040CADC

l0040C7EA:
	addu	r17,r5,r6
	lw	r4,0004(r16)
	sltu	r5,r17,r5
	addu	r7,r7,r20
	addu	r18,r5,r7
	bgeuc	r4,r19,0040C816

l0040C7FA:
	addiu	r7,r4,00000001
	sw	r7,0004(sp)
	lbu	r4,0000(r4)
	addiu	r6,r4,FFFFFFD0
	bc	0040C7A0

l0040C808:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	lbux	r20,r4(r22)
	bc	0040C6DE

l0040C816:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	addiu	r6,r4,FFFFFFD0
	bc	0040C7A0

l0040C824:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	bc	0040C75C

l0040C82E:
	li	r7,99999999
	bgeuc	r7,r17,0040C7B2

l0040C838:
	addiupc	r22,00006A2F
	lbux	r7,r4(r22)
	bltiuc	r7,0000000A,0040C8F8

l0040C846:
	beqzc	r19,0040C84E

l0040C848:
	lw	r7,0004(r16)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)

l0040C84E:
	lw	r17,0004(sp)
	bgeuc	r18,r7,0040C86E

l0040C854:
	xor	r17,r23,r17
	xor	r5,r23,r18
	subu	r4,r17,r23
	subu	r23,r5,r23
	sltu	r17,r17,r4
	subu	r5,r23,r17
	restore.jrc	00000050,r30,0000000A

l0040C86E:
	bnec	r18,r7,0040C876

l0040C870:
	lw	r17,0008(sp)
	bltuc	r17,r7,0040C854

l0040C876:
	lw	r17,0008(sp)
	bbnezc	r7,00000000,0040C884

l0040C87C:
	beqc	r0,r23,0040CAE2

l0040C880:
	addiu	r23,r0,FFFFFFFF

l0040C884:
	lw	r17,0004(sp)
	bltuc	r7,r18,0040C894

l0040C88A:
	bnec	r7,r18,0040C854

l0040C88E:
	lw	r17,0008(sp)
	bgeuc	r7,r17,0040C854

l0040C894:
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	lw	r17,0004(sp)
	lw	r17,0008(sp)
	restore.jrc	00000050,r30,0000000A

l0040C8A2:
	move	r18,r0

l0040C8A4:
	move	r6,r30
	move	r7,r0
	bgeuc	r20,r30,0040C846

l0040C8AC:
	li	r4,FFFFFFFF
	li	r5,FFFFFFFF
	balc	0040EAB0
	lw	r21,0001(r16)
	mul	r7,r18,r30
	addiu	r10,r0,FFFFFFFF
	muhu	r11,r17,r30
	addiu	r2,r21,00000001
	bltuc	r5,r18,0040C8FE

l0040C8CA:
	beqc	r18,r5,0040CA98

l0040C8CE:
	addu	r7,r7,r11
	mul	r6,r17,r30
	nor	r5,r0,r20
	beqc	r7,r10,0040CA9E

l0040C8DC:
	addu	r17,r6,r20
	sltu	r18,r17,r6
	addu	r18,r18,r7
	bgeuc	r21,r19,0040C94C

l0040C8EA:
	sw	r2,0004(r16)
	lbu	r4,0000(r21)
	lbux	r20,r4(r22)
	bc	0040C8A4

l0040C8F8:
	lw	r21,0001(r16)
	addiu	r30,r0,0000000A

l0040C8FE:
	bgeuc	r21,r19,0040C91A

l0040C902:
	addiu	r7,r21,00000001
	sw	r7,0004(sp)
	lbu	r7,0000(r21)
	lbux	r7,r7(r22)
	bgeuc	r7,r30,0040C92E

l0040C914:
	lw	r21,0001(r16)

l0040C916:
	bltuc	r21,r19,0040C902

l0040C91A:
	move.balc	r4,r16,0040CB78
	lbux	r7,r4(r22)
	bgeuc	r7,r30,0040C92E

l0040C926:
	lw	r19,0068(r16)
	lw	r21,0001(r16)
	bc	0040C916

l0040C92E:
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	lw	r17,0008(sp)
	bbeqzc	r7,00000000,0040C994

l0040C93C:
	lw	r7,0068(r16)
	move	r23,r0
	lw	r4,0004(sp)
	lw	r4,0008(sp)
	bnec	r0,r7,0040C848

l0040C94A:
	bc	0040C854

l0040C94C:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	lbux	r20,r4(r22)
	bc	0040C8A4

l0040C95A:
	bneiuc	r4,00000030,0040CA88

l0040C95E:
	lw	r7,0004(r16)
	bgeuc	r7,r19,0040CAA4

l0040C964:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)

l0040C96C:
	ori	r7,r4,00000020
	beqic	r7,00000038,0040CAAE

l0040C974:
	bnec	r0,r30,0040C6C2

l0040C978:
	addiupc	r22,000068EF
	li	r7,00000005
	lbux	r20,r4(r22)
	addiu	r30,r0,00000008
	bc	0040C9B0

l0040C98A:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	bc	0040C6AC

l0040C994:
	lw	r7,0068(r16)
	beqc	r0,r7,0040CB08

l0040C99C:
	lw	r4,0004(sp)
	lw	r4,0008(sp)
	bc	0040C848

l0040C9A2:
	lsa	r7,r30,r30,00000001
	sll	r7,r7,00000003
	subu	r7,r7,r30
	ext	r7,r7,00000005,00000003

l0040C9B0:
	addiupc	r6,000068AC
	move	r17,r0
	lbux	r5,r6(r7)

l0040C9BA:
	li	r7,07FFFFFF
	sllv	r6,r17,r5
	bgeuc	r20,r30,0040C9F6

l0040C9C8:
	bltuc	r7,r17,0040C9F6

l0040C9CC:
	lw	r7,0004(r16)
	or	r17,r20,r6
	bgeuc	r7,r19,0040C9E4

l0040C9D6:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)
	lbux	r20,r4(r22)
	bc	0040C9BA

l0040C9E4:
	sw	r5,000C(sp)
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	lw	r17,000C(sp)
	lbux	r20,r4(r22)
	bc	0040C9BA

l0040C9F6:
	addiu	r3,r0,FFFFFFFF
	subu	r2,r0,r5
	sllv	r7,r3,r2
	srlv	r3,r3,r5
	movz	r7,r0,r5

l0040CA0A:
	move	r10,r3
	slti	r11,r5,00000020
	or	r3,r3,r7
	movz	r3,r10,r11

l0040CA18:
	move	r18,r0
	movz	r10,r0,r11

l0040CA1E:
	srlv	r4,r17,r2
	sllv	r7,r18,r5
	movz	r4,r0,r5

l0040CA2A:
	sllv	r6,r17,r5
	bgeuc	r20,r30,0040C846

l0040CA32:
	or	r7,r7,r4
	lw	r21,0001(r16)
	bltuc	r10,r18,0040C8FE

l0040CA3A:
	addiu	r4,r21,00000001
	movz	r7,r6,r11

l0040CA42:
	beqc	r18,r10,0040CA82

l0040CA46:
	movz	r6,r0,r11

l0040CA4A:
	move	r18,r7
	or	r17,r20,r6
	bgeuc	r21,r19,0040CA60

l0040CA54:
	sw	r4,0004(sp)
	lbu	r4,0000(r21)
	lbux	r20,r4(r22)
	bc	0040CA1E

l0040CA60:
	sw	r5,000C(sp)
	sw	r3,0010(sp)
	swm	r10,0014(sp),00000002
	sw	r2,001C(sp)
	move.balc	r4,r16,0040CB78
	lbux	r20,r4(r22)
	lwm	r10,0014(sp),00000002
	lw	r19,0068(r16)
	lw	r17,000C(sp)
	lw	r16,0010(sp)
	lw	r16,001C(sp)
	bc	0040CA1E

l0040CA82:
	bgeuc	r3,r17,0040CA46

l0040CA86:
	bc	0040C8FE

l0040CA88:
	addiupc	r22,000067DF
	lbux	r7,r4(r22)
	bltiuc	r7,00000010,0040C6CC

l0040CA96:
	bc	0040C728

l0040CA98:
	bgeuc	r4,r17,0040C8CE

l0040CA9C:
	bc	0040C8FE

l0040CA9E:
	bgeuc	r5,r6,0040C8DC

l0040CAA2:
	bc	0040C8FE

l0040CAA4:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	bc	0040C96C

l0040CAAE:
	lw	r7,0004(r16)
	bgeuc	r7,r19,0040CB16

l0040CAB4:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)

l0040CABC:
	addiupc	r22,000067AB
	lbux	r20,r4(r22)
	bltiuc	r20,00000010,0040CB00

l0040CACA:
	beqzc	r19,0040CB2C

l0040CACC:
	lw	r7,0004(r16)
	addiu	r6,r7,FFFFFFFF
	beqzc	r17,0040CB20

l0040CAD4:
	addiu	r7,r7,FFFFFFFE
	movep	r4,r5,r0,r0
	sw	r7,0004(sp)
	restore.jrc	00000050,r30,0000000A

l0040CADC:
	bgeuc	r8,r5,0040C7EA

l0040CAE0:
	bc	0040C838

l0040CAE2:
	balc	004049B0
	lw	r17,0008(sp)
	move	r7,r6
	addiu	r7,r7,FFFFFFFF
	sltu	r5,r7,r6
	lw	r17,0004(sp)
	addiu	r21,r6,FFFFFFFF
	li	r6,00000022
	sw	r6,0000(sp)
	addu	r5,r5,r21
	move	r4,r7
	restore.jrc	00000050,r30,0000000A

l0040CB00:
	li	r7,00000003
	addiu	r30,r0,00000010
	bc	0040C9B0

l0040CB08:
	beqc	r0,r23,0040CAE2

l0040CB0C:
	addiu	r23,r0,FFFFFFFF
	lw	r4,0004(sp)
	lw	r4,0008(sp)
	bc	0040C854

l0040CB16:
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	bc	0040CABC

l0040CB20:
	sw	r6,0004(sp)

l0040CB22:
	movep	r6,r7,r0,r0
	move.balc	r4,r16,0040CB40
	movep	r4,r5,r0,r0
	restore.jrc	00000050,r30,0000000A

l0040CB2C:
	bnec	r0,r17,0040C73E

l0040CB30:
	bc	0040CB22
0040CB32       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __shlim: 0040CB40
;;   Called from:
;;     00409F70 (in strtox)
;;     00409FE8 (in strtox)
;;     0040B65C (in decfloat)
;;     0040BE8E (in __floatscan)
;;     0040C4F8 (in __floatscan)
;;     0040C608 (in __floatscan)
;;     0040C732 (in __intscan)
;;     0040CB24 (in __intscan)
;;     0040D518 (in __isoc99_vfscanf)
;;     0040D66E (in __isoc99_vfscanf)
;;     0040D73A (in __isoc99_vfscanf)
;;     0040DB62 (in __isoc99_vfscanf)
__shlim proc
	lw	r10,0001(r4)
	or	r11,r6,r7
	lw	r5,0008(r4)
	swm	r6,0070(r4),00000002
	subu	r8,r5,r10
	sra	r9,r8,0000001F
	swm	r8,0078(r4),00000002
	beqc	r0,r11,0040CB64

l0040CB5C:
	bltc	r7,r9,0040CB6E

l0040CB60:
	beqc	r9,r7,0040CB6A

l0040CB64:
	sw	r5,0068(r4)
	jrc	ra

l0040CB6A:
	bgeuc	r6,r8,0040CB64

l0040CB6E:
	addu	r5,r10,r6
	sw	r5,0068(r4)
	jrc	ra

;; __shgetc: 0040CB78
;;   Called from:
;;     0040B2A8 (in scanexp)
;;     0040B348 (in scanexp)
;;     0040B36E (in scanexp)
;;     0040B3BE (in scanexp)
;;     0040B3E6 (in scanexp)
;;     0040B4C4 (in decfloat)
;;     0040B62E (in decfloat)
;;     0040B880 (in decfloat)
;;     0040BC40 (in decfloat)
;;     0040BE96 (in __floatscan)
;;     0040BECC (in __floatscan)
;;     0040BED6 (in __floatscan)
;;     0040BEE0 (in __floatscan)
;;     0040BFB6 (in __floatscan)
;;     0040BFC0 (in __floatscan)
;;     0040C000 (in __floatscan)
;;     0040C01C (in __floatscan)
;;     0040C026 (in __floatscan)
;;     0040C02C (in __floatscan)
;;     0040C03A (in __floatscan)
;;     0040C092 (in __floatscan)
;;     0040C0B0 (in __floatscan)
;;     0040C0C2 (in __floatscan)
;;     0040C236 (in __floatscan)
;;     0040C242 (in __floatscan)
;;     0040C4C8 (in __floatscan)
;;     0040C4E4 (in __floatscan)
;;     0040C50E (in __floatscan)
;;     0040C708 (in __intscan)
;;     0040C808 (in __intscan)
;;     0040C816 (in __intscan)
;;     0040C824 (in __intscan)
;;     0040C91A (in __intscan)
;;     0040C94C (in __intscan)
;;     0040C98A (in __intscan)
;;     0040C9E6 (in __intscan)
;;     0040CA6A (in __intscan)
;;     0040CAA4 (in __intscan)
;;     0040CB16 (in __intscan)
;;     0040D55A (in __isoc99_vfscanf)
;;     0040D564 (in __isoc99_vfscanf)
;;     0040D7A4 (in __isoc99_vfscanf)
;;     0040D7AC (in __isoc99_vfscanf)
;;     0040DC0C (in fn0040DC0C)
__shgetc proc
	save	00000010,ra,00000002
	lwm	r6,0070(r4),00000002
	or	r5,r6,r7
	move	r16,r4
	beqzc	r5,0040CB8E

l0040CB86:
	lw	r5,007C(r4)
	bgec	r5,r7,0040CC1C

l0040CB8E:
	move.balc	r4,r16,0040D3E0
	bltc	r4,r0,0040CC26

l0040CB96:
	lw	r6,0070(r16)
	lw	r5,0074(r16)
	lw	r8,0001(r16)
	or	r9,r6,r5
	lw	r7,0008(r16)
	bnec	r0,r9,0040CBE4

l0040CBAA:
	move	r6,r7

l0040CBAC:
	sw	r6,0068(r16)
	beqzc	r7,0040CBD8

l0040CBB2:
	lw	r6,0078(r16)
	subu	r7,r7,r8
	addiu	r7,r7,00000001
	lw	r5,007C(r16)
	addu	r9,r6,r7
	sra	r7,r7,0000001F
	sltu	r6,r9,r6
	addu	r7,r5,r7
	addu	r7,r6,r7
	sw	r9,0078(r16)
	sw	r7,007C(r16)

l0040CBD8:
	lbu	r7,-0001(r8)
	beqc	r4,r7,0040CBE2

l0040CBDE:
	sb	r4,-0001(r8)

l0040CBE2:
	restore.jrc	00000010,ra,00000002

l0040CBE4:
	lw	r9,0078(r16)
	subu	r2,r7,r8
	lw	r12,007C(r16)
	sra	r10,r2,0000001F
	subu	r11,r6,r9
	sltu	r3,r6,r11
	subu	r5,r5,r12
	subu	r5,r5,r3
	bltc	r10,r5,0040CBAA

l0040CC08:
	beqc	r5,r10,0040CC16

l0040CC0C:
	addiu	r6,r6,FFFFFFFF
	subu	r6,r6,r9
	addu	r6,r6,r8
	bc	0040CBAC

l0040CC16:
	bltuc	r2,r11,0040CBAA

l0040CC1A:
	bc	0040CC0C

l0040CC1C:
	bnec	r5,r7,0040CC26

l0040CC1E:
	lw	r7,0078(r4)
	bltuc	r7,r6,0040CB8E

l0040CC26:
	li	r4,FFFFFFFF
	sw	r0,0068(r16)
	restore.jrc	00000010,ra,00000002
0040CC2E                                           00 00               ..

;; __syscall_ret: 0040CC30
;;   Called from:
;;     00404A80 (in sysinfo)
;;     00405B40 (in getrlimit64)
;;     00405BB2 (in ioctl)
;;     00405BD4 (in madvise)
;;     00405C44 (in mmap64)
;;     00405CAE (in mremap)
;;     00405CD8 (in munmap)
;;     00405DC8 (in bind)
;;     00405DE8 (in connect)
;;     004066C8 (in getsockname)
;;     004066EA (in getsockopt)
;;     0040766A (in recvfrom)
;;     00407688 (in recvmsg)
;;     00407D38 (in sendmsg)
;;     00407D5A (in sendto)
;;     00407D7A (in setsockopt)
;;     00407D96 (in socket)
;;     00407DCA (in socket)
;;     00407E1C (in sched_yield)
;;     00407E54 (in poll)
;;     00407EFC (in raise)
;;     00407F22 (in setitimer)
;;     00407FE4 (in __libc_sigaction)
;;     00408090 (in __fopen_rb_ca)
;;     004080F6 (in __stdio_close)
;;     00408124 (in __stdio_read)
;;     00408184 (in __stdio_seek)
;;     0040AF0E (in __clock_gettime)
;;     0040AF94 (in close)
;;     0040B096 (in write)
;;     0040D2FC (in __stdio_write)
;;     0040E106 (in open64)
;;     0040E874 (in lseek64)
__syscall_ret proc
	lui	r7,FFFFFFFF
	bltuc	r7,r4,0040CC3A

l0040CC38:
	jrc	ra

l0040CC3A:
	save	00000010,ra,00000002
	move	r16,r4
	balc	004049B0
	subu	r7,r0,r16
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000002
0040CC4C                                     00 00 00 00             ....

;; __vdsosym: 0040CC50
;;   Called from:
;;     0040AEBC (in cgt_init)
__vdsosym proc
	save	00000030,r30,0000000A
	addiupc	r7,000477DA
	lw	r17,0010(r7)
	lw	r7,0000(r17)
	beqic	r7,00000021,0040CDA2

l0040CC5E:
	move	r16,r0
	bnezc	r7,0040CC6A

l0040CC62:
	bc	0040CD20

l0040CC64:
	move	r16,r6
	beqc	r0,r7,0040CD20

l0040CC6A:
	addiu	r6,r16,00000002
	lwxs	r7,r6(r17)
	bneiuc	r7,00000021,0040CC64

l0040CC74:
	addiu	r16,r16,00000003
	sll	r16,r16,00000002

l0040CC78:
	lwx	r11,r16(r17)
	beqc	r0,r11,0040CD20

l0040CC80:
	lw	r6,001C(r11)
	lhu	r9,002C(r11)
	addu	r6,r6,r11
	beqc	r0,r9,0040CD20

l0040CC8E:
	lhu	r10,002A(r11)
	li	r17,FFFFFFFF
	move	r7,r0
	move	r16,r0
	bc	0040CCA6

l0040CC9A:
	beqic	r8,00000002,0040CD24

l0040CC9E:
	addiu	r16,r16,00000001
	addu	r6,r6,r10
	beqc	r16,r9,0040CCBC

l0040CCA6:
	lw	r8,0000(r6)
	bneiuc	r8,00000001,0040CC9A

l0040CCAC:
	lwm	r17,0004(r6),00000002
	addiu	r16,r16,00000001
	addu	r17,r17,r11
	addu	r6,r6,r10
	subu	r17,r17,r18
	bnec	r16,r9,0040CCA6

l0040CCBC:
	beqzc	r7,0040CD20

l0040CCBE:
	li	r6,FFFFFFFF
	beqc	r17,r6,0040CD20

l0040CCC4:
	lw	r16,0000(r7)
	beqzc	r16,0040CD20

l0040CCC8:
	addiu	r7,r7,00000004
	move	r22,r0
	move	r30,r0
	move	r23,r0
	move	r18,r0
	move	r21,r0
	bc	0040CCEA

l0040CCD6:
	beqic	r16,00000004,0040CD2E

l0040CCDA:
	xori	r16,r16,00000005
	movz	r21,r6,r16

l0040CCE2:
	addiu	r7,r7,00000008
	lw	r16,-0004(r7)
	beqzc	r16,0040CD16

l0040CCEA:
	lw	r6,0000(r7)
	li	r8,6FFFFFF0
	addu	r6,r17,r6
	beqic	r16,00000006,0040CD2A

l0040CCF8:
	bltiuc	r16,00000007,0040CCD6

l0040CCFC:
	beqc	r16,r8,0040CD32

l0040CD00:
	li	r8,6FFFFFFC
	addiu	r7,r7,00000008
	xor	r16,r16,r8
	movz	r22,r6,r16

l0040CD10:
	lw	r16,-0004(r7)
	bnezc	r16,0040CCEA

l0040CD16:
	beqc	r0,r21,0040CD20

l0040CD1A:
	beqzc	r18,0040CD20

l0040CD1C:
	bnec	r0,r23,0040CD36

l0040CD20:
	move	r4,r0
	restore.jrc	00000030,r30,0000000A

l0040CD24:
	lw	r7,0004(r6)
	addu	r7,r7,r11
	bc	0040CC9E

l0040CD2A:
	move	r18,r6
	bc	0040CCE2

l0040CD2E:
	move	r23,r6
	bc	0040CCE2

l0040CD32:
	move	r30,r6
	bc	0040CCE2

l0040CD36:
	lw	r8,0001(r23)
	movz	r30,r0,r22

l0040CD3C:
	beqc	r0,r8,0040CD20

l0040CD40:
	movep	r20,r19,r4,r5
	bc	0040CD4C

l0040CD44:
	addiu	r16,r16,00000001
	addiu	r18,r18,00000010
	bgeuc	r16,r8,0040CD20

l0040CD4C:
	lbu	r6,000C(r18)
	li	r7,00000027
	andi	r5,r6,0000000F
	srav	r7,r7,r5
	bbeqzc	r7,00000000,0040CD44

l0040CD5C:
	srl	r6,r6,00000004
	addiu	r7,r0,00000406
	srav	r6,r7,r6
	bbeqzc	r6,00000000,0040CD44

l0040CD6A:
	lhu	r7,000E(r18)
	beqzc	r7,0040CD44

l0040CD70:
	lw	r5,0000(r18)
	addu	r5,r5,r21
	move.balc	r4,r19,0040A830
	bnezc	r4,0040CD9E

l0040CD7A:
	beqc	r0,r30,0040CDB4

l0040CD7E:
	lhuxs	r5,r16(r30)
	move	r7,r22
	ext	r5,r5,00000000,0000000F
	bc	0040CD8C

l0040CD8A:
	addu	r7,r7,r6

l0040CD8C:
	lhu	r6,0002(r7)
	bbnezc	r6,00000000,0040CD9A

l0040CD92:
	lhu	r6,0004(r7)
	ext	r6,r6,00000000,0000000F
	beqc	r5,r6,0040CDA6

l0040CD9A:
	lw	r6,0010(r7)
	bnezc	r6,0040CD8A

l0040CD9E:
	lw	r8,0001(r23)
	bc	0040CD44

l0040CDA2:
	li	r16,00000004
	bc	0040CC78

l0040CDA6:
	lw	r6,000C(r7)
	lwx	r5,r6(r7)
	addu	r5,r5,r21
	move.balc	r4,r20,0040A830
	bnezc	r4,0040CD9E

l0040CDB4:
	lw	r4,0004(r18)
	addu	r4,r17,r4
	restore.jrc	00000030,r30,0000000A
0040CDBA                               00 00 00 00 00 00           ......

;; calloc: 0040CDC0
;;   Called from:
;;     00405EA2 (in getaddrinfo)
;;     00406054 (in netlink_msg_to_ifaddr)
calloc proc
	beqzc	r5,0040CDD0

l0040CDC2:
	li	r6,FFFFFFFF
	divu	r7,r6,r5
	teq	r5,r0,00000007

l0040CDCC:
	bltuc	r7,r4,0040CDD6

l0040CDD0:
	mul	r4,r4,r5
	bc	0040579A

l0040CDD6:
	save	00000010,ra,00000001
	balc	004049B0
	li	r7,0000000C
	sw	r7,0000(sp)
	move	r4,r0
	restore.jrc	00000010,ra,00000001
0040CDE4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __expand_heap: 0040CDF0
;;   Called from:
;;     00404AF0 (in __simple_malloc)
;;     00405382 (in malloc)
__expand_heap proc
	save	00000020,ra,00000004
	addiupc	r18,0004763A
	li	r7,7FFFFFFF
	lw	r6,0024(r18)
	lw	r16,0000(r4)
	subu	r7,r7,r6
	bltuc	r7,r16,0040CEDC

l0040CE06:
	subu	r7,r0,r16
	addiu	r5,r6,FFFFFFFF
	move	r17,r4
	and	r7,r7,r5
	lwpc	r4,00432EF4
	addu	r16,r16,r7
	beqc	r0,r4,0040CEB4

l0040CE1E:
	nor	r7,r0,r4
	bgeuc	r16,r7,0040CE42

l0040CE26:
	lw	r7,0010(r18)
	lui	r8,00000800
	addu	r5,r16,r4
	bgeuc	r8,r7,0040CED4

l0040CE32:
	move	r8,r7
	addiu	r8,r8,FF800000

l0040CE3A:
	bgeuc	r8,r5,0040CE82

l0040CE3E:
	bgeuc	r4,r7,0040CE82

l0040CE42:
	lwpc	r5,00432EF0
	addiu	r7,r0,00000802
	srl	r5,r5,00000001
	move	r10,r0
	sllv	r5,r6,r5
	move	r11,r0
	sltu	r4,r5,r16
	addiu	r8,r0,FFFFFFFF
	movz	r16,r5,r4

l0040CE62:
	li	r6,00000003
	movep	r4,r5,r0,r16
	balc	00405BE2
	li	r7,FFFFFFFF
	beqc	r4,r7,0040CEF4

l0040CE70:
	lwpc	r7,00432EF0
	sw	r16,0000(r17)
	addiu	r7,r7,00000001
	swpc	r7,00432EF0
	restore.jrc	00000020,ra,00000004

l0040CE82:
	addiu	r7,sp,0000000C
	lui	r8,00000800
	bgeuc	r8,r7,0040CED8

l0040CE8C:
	move	r8,r7
	addiu	r8,r8,FF800000

l0040CE94:
	bgeuc	r8,r5,0040CE9C

l0040CE98:
	bltuc	r4,r7,0040CE42

l0040CE9C:
	addiu	r4,r0,000000D6
	balc	00404A50
	lwpc	r7,00432EF4
	addu	r6,r16,r7
	beqc	r4,r6,0040CEE8

l0040CEB0:
	lw	r6,0024(r18)
	bc	0040CE42

l0040CEB4:
	move	r5,r0
	addiu	r4,r0,000000D6
	balc	00404A50
	lw	r6,0024(r18)
	subu	r7,r0,r4
	addiu	r5,r6,FFFFFFFF
	and	r7,r7,r5
	addu	r4,r7,r4
	swpc	r4,00432EF4
	bc	0040CE1E

l0040CED4:
	move	r8,r0
	bc	0040CE3A

l0040CED8:
	move	r8,r0
	bc	0040CE94

l0040CEDC:
	balc	004049B0
	li	r7,0000000C
	sw	r7,0000(sp)
	move	r4,r0
	restore.jrc	00000020,ra,00000004

l0040CEE8:
	swpc	r4,00432EF4
	sw	r16,0000(r17)
	move	r4,r7
	restore.jrc	00000020,ra,00000004

l0040CEF4:
	move	r4,r0
	restore.jrc	00000020,ra,00000004
0040CEF8                         00 00 00 00 00 00 00 00         ........

;; copysignl: 0040CF00
;;   Called from:
;;     0040BB5C (in decfloat)
;;     0040C56E (in __floatscan)
;;     0040C65C (in __floatscan)
copysignl proc
	bc	0040E120
0040CF04             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; fabs: 0040CF10
;;   Called from:
;;     0040BC48 (in decfloat)
fabs proc
	move	r6,r4
	ext	r7,r5,00000000,0000001F
	movep	r4,r5,r6,r7
	jrc	ra
0040CF1A                               00 00 00 00 00 00           ......

;; fmodl: 0040CF20
;;   Called from:
;;     0040BA68 (in decfloat)
;;     0040BB74 (in decfloat)
fmodl proc
	bc	0040E140
0040CF24             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; frexpl: 0040CF30
frexpl proc
	bc	0040E2C0
0040CF34             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; scalbn: 0040CF40
;;   Called from:
;;     0040BB56 (in decfloat)
;;     0040BB6C (in decfloat)
;;     0040C55E (in __floatscan)
;;     0040C64C (in __floatscan)
;;     0040CFD0 (in scalbnl)
scalbn proc
	save	00000020,ra,00000005
	addiu	r18,r0,000003FF
	move	r17,r6
	movep	r8,r9,r4,r5
	bgec	r18,r6,0040CF90

l0040CF4E:
	lui	r19,00000412
	addiu	r16,r17,FFFFFC01
	lw	r6,02B8(r19)
	lw	r7,02BC(r19)
	balc	0040CFCC
	movep	r8,r9,r4,r5
	bgec	r18,r16,0040CF7E

l0040CF66:
	lw	r7,02BC(r19)
	addiu	r16,r17,FFFFF802
	lw	r6,02B8(r19)
	balc	0040CFCC
	slti	r7,r16,00000400
	movep	r8,r9,r4,r5
	movz	r16,r18,r7

l0040CF7E:
	addiu	r16,r16,000003FF
	move	r4,r0
	sll	r5,r16,00000014
	movep	r6,r7,r4,r5
	movep	r4,r5,r8,r9
	balc	0040CFCC
	restore.jrc	00000020,ra,00000005

l0040CF90:
	addiu	r18,r0,FFFFFC02
	move	r16,r6
	bgec	r6,r18,0040CF7E

l0040CF9A:
	lui	r19,00000412
	addiu	r16,r17,000003C9
	lw	r6,02C0(r19)
	lw	r7,02C4(r19)
	balc	0040CFCC
	movep	r8,r9,r4,r5
	bgec	r16,r18,0040CF7E

l0040CFB2:
	lw	r7,02C4(r19)
	addiu	r16,r17,00000792
	lw	r6,02C0(r19)
	balc	0040CFCC
	slt	r7,r16,r18
	movep	r8,r9,r4,r5
	movn	r16,r18,r7

l0040CFCA:
	bc	0040CF7E

;; fn0040CFCC: 0040CFCC
;;   Called from:
;;     0040CF5E (in scalbn)
;;     0040CF72 (in scalbn)
;;     0040CF8C (in scalbn)
;;     0040CFAA (in scalbn)
;;     0040CFBE (in scalbn)
fn0040CFCC proc
	bc	00404330

;; scalbnl: 0040CFD0
;;   Called from:
;;     0040BAB2 (in decfloat)
;;     0040C40C (in __floatscan)
scalbnl proc
	bc	0040CF40
0040CFD4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; mbstowcs: 0040CFE0
;;   Called from:
;;     00406E06 (in is_valid_hostname)
mbstowcs proc
	save	00000020,ra,00000001
	move	r7,r0
	sw	r5,000C(sp)
	addiu	r5,sp,0000000C
	balc	0040E410
	restore.jrc	00000020,ra,00000001
0040CFEE                                           00 00               ..

;; wctomb: 0040CFF0
wctomb proc
	beqzc	r4,0040CFF8

l0040CFF2:
	move	r6,r0
	bc	0040E600

l0040CFF8:
	jrc	ra
0040CFFA                               00 00 00 00 00 00           ......

;; dn_expand: 0040D000
;;   Called from:
;;     004062F4 (in dns_parse_callback)
;;     00406FB4 (in dns_parse_callback)
dn_expand proc
	bnec	r5,r6,0040D00A

l0040D002:
	addiu	r9,r0,FFFFFFFF

l0040D006:
	move	r4,r9
	jrc	ra

l0040D00A:
	bgec	r0,r8,0040D002

l0040D00E:
	slti	r9,r8,000000FF
	addiu	r10,r0,000000FE
	movz	r8,r10,r9

l0040D01A:
	move	r13,r6
	addu	r8,r8,r7
	move	r11,r7
	move	r14,r0
	addiu	r9,r0,FFFFFFFF

l0040D026:
	subu	r2,r5,r4
	bgec	r14,r2,0040D002

l0040D02E:
	lbu	r10,0000(r13)
	andi	r3,r10,000000C0
	beqc	r0,r3,0040D06E

l0040D03A:
	addiu	r3,r13,00000001
	beqc	r5,r3,0040D002

l0040D042:
	addiu	r3,r0,00003F00
	sll	r10,r10,00000008
	and	r10,r10,r3
	lbu	r3,0001(r13)
	or	r10,r10,r3
	bgec	r9,r0,0040D062

l0040D05A:
	addiu	r9,r13,00000002
	subu	r9,r9,r6

l0040D062:
	bgec	r10,r2,0040D002

l0040D066:
	addu	r13,r4,r10

l0040D06A:
	addiu	r14,r14,00000002
	bc	0040D026

l0040D06E:
	beqc	r0,r10,0040D0BC

l0040D072:
	move	r2,r7
	beqc	r11,r7,0040D084

l0040D078:
	addiu	r10,r0,0000002E
	addiu	r2,r11,00000001
	sb	r10,0000(r11)

l0040D084:
	addiu	r3,r13,00000001
	lbu	r10,0000(r13)
	subu	r11,r5,r3
	bgec	r10,r11,0040D002

l0040D094:
	subu	r11,r8,r2
	bgec	r10,r11,0040D002

l0040D09C:
	move	r11,r0
	bc	0040D0AE

l0040D0A0:
	addu	r12,r13,r11
	lbu	r12,0001(r12)
	sbx	r12,r2(r11)
	addiu	r11,r11,00000001

l0040D0AE:
	bnec	r10,r11,0040D0A0

l0040D0B2:
	addu	r11,r2,r10
	addu	r13,r3,r10
	bc	0040D06A

l0040D0BC:
	sb	r0,0000(r11)
	bgec	r9,r0,0040D006

l0040D0C4:
	addiu	r9,r13,00000001
	subu	r9,r9,r6
	bc	0040D006
0040D0CE                                           00 00               ..

;; __dns_parse: 0040D0D0
;;   Called from:
;;     0040649A (in getnameinfo)
;;     00406DA6 (in name_from_dns)
__dns_parse proc
	save	00000020,ra,00000008
	movep	r18,r20,r4,r5
	movep	r21,r22,r6,r7
	bgeic	r20,0000000C,0040D0DE

l0040D0DA:
	li	r4,FFFFFFFF
	restore.jrc	00000020,ra,00000008

l0040D0DE:
	lbu	r7,0003(r18)
	andi	r7,r7,0000000F
	beqzc	r7,0040D0E8

l0040D0E4:
	move	r4,r0
	restore.jrc	00000020,ra,00000008

l0040D0E8:
	lbu	r6,0004(r18)
	addiu	r7,r18,0000000C
	lbu	r5,0005(r18)
	sll	r6,r6,00000008
	lbu	r19,0006(r18)
	addu	r6,r6,r5
	lbu	r5,0007(r18)
	sll	r19,r19,00000008
	addu	r19,r19,r5
	addu	r5,r6,r19
	bgeic	r5,00000001,0040D0DA

l0040D108:
	addiu	r6,r6,FFFFFFFF
	li	r5,FFFFFFFF
	bnec	r5,r6,0040D11A

l0040D10E:
	addiu	r19,r19,FFFFFFFF
	li	r6,FFFFFFFF
	bnec	r6,r19,0040D156

l0040D116:
	bc	0040D0E4

l0040D118:
	addiu	r7,r7,00000001

l0040D11A:
	subu	r4,r7,r18
	lbu	r5,0000(r7)
	bgec	r4,r20,0040D12A

l0040D122:
	addiu	r4,r5,FFFFFFFF
	bltiuc	r4,0000003F,0040D118

l0040D12A:
	addiu	r4,r0,000000C1
	bltuc	r4,r5,0040D0DA

l0040D132:
	bnec	r4,r5,0040D13E

l0040D134:
	lbu	r16,0001(r7)
	addiu	r4,r0,000000FF
	beqc	r16,r4,0040D0DA

l0040D13E:
	addiu	r4,r20,FFFFFFFA
	addu	r4,r18,r4
	bltuc	r4,r7,0040D0DA

l0040D148:
	li	r4,00000006
	li	r16,00000005
	movz	r4,r16,r5

l0040D150:
	addu	r7,r7,r4
	bc	0040D108

l0040D154:
	addiu	r7,r7,00000001

l0040D156:
	subu	r6,r7,r18
	lbu	r16,0000(r7)
	bgec	r6,r20,0040D166

l0040D15E:
	addiu	r6,r16,FFFFFFFF
	bltiuc	r6,0000003F,0040D154

l0040D166:
	addiu	r6,r0,000000C1
	bltuc	r6,r16,0040D0DA

l0040D16E:
	bnec	r16,r6,0040D17A

l0040D170:
	lbu	r5,0001(r7)
	addiu	r6,r0,000000FF
	beqc	r5,r6,0040D0DA

l0040D17A:
	addiu	r6,r20,FFFFFFFA
	addu	r6,r18,r6
	bltuc	r6,r7,0040D0DA

l0040D184:
	li	r6,00000002
	li	r5,00000001
	movz	r6,r5,r16

l0040D18C:
	addu	r16,r7,r6
	addu	r6,r18,r20
	lbu	r17,0008(r16)
	lbu	r7,0009(r16)
	sll	r17,r17,00000008
	addu	r17,r17,r7
	addu	r7,r16,r17
	bltuc	r6,r7,0040D0DA

l0040D1A4:
	lbu	r5,0001(r16)
	addiu	r6,r16,0000000A
	movep	r7,r8,r17,r18
	move	r4,r22
	jalrc	ra,r21
	bltc	r4,r0,0040D0DA

l0040D1B4:
	addiu	r7,r17,0000000A
	addu	r7,r16,r7
	bc	0040D10E
0040D1BC                                     00 00 00 00             ....

;; __restore: 0040D1C0
__restore proc
	addiu	r2,r0,0000008B
	syscall	00000000
	nop
	nop

;; __lockfile: 0040D1D0
;;   Called from:
;;     004081E8 (in fflush_unlocked)
;;     00408250 (in fgets_unlocked)
;;     004083A2 (in fputc)
;;     00408594 (in fwrite_unlocked)
;;     004085DE (in _IO_getc)
;;     0040864E (in perror)
;;     00408722 (in _IO_putc)
;;     00408794 (in puts)
;;     00409A32 (in vfprintf)
;;     0040D1CC (in __restore)
;;     0040D4D0 (in __isoc99_vfscanf)
;;     0040E6EE (in close_file)
__lockfile proc
	save	00000010,ra,00000003
	move	r16,r4
	rdhwr	r3,0000001D,00000000
	lw	r7,004C(r4)
	move	r4,r0
	lw	r17,-0094(r3)
	bnec	r17,r7,0040D1F0

l0040D1E4:
	restore.jrc	00000010,ra,00000003

l0040D1E6:
	li	r7,00000001
	addiu	r5,r16,00000050
	balc	0040ADB0

l0040D1F0:
	addiu	r4,r16,0000004C
	sync	00000000

l0040D1F8:
	ll	r6,0000(r4)
	bnezc	r6,0040D206

l0040D1FE:
	move	r7,r17
	sc	r7,0000(r4)
	beqzc	r7,0040D1F8

l0040D206:
	sync	00000000
	bnezc	r6,0040D1E6

l0040D20C:
	li	r4,00000001
	restore.jrc	00000010,ra,00000003

;; __unlockfile: 0040D210
;;   Called from:
;;     00408204 (in fflush_unlocked)
;;     00408232 (in fflush_unlocked)
;;     00408272 (in fgets_unlocked)
;;     004082A6 (in fgets_unlocked)
;;     004083C0 (in fputc)
;;     00408502 (in funlockfile)
;;     004085A6 (in fwrite_unlocked)
;;     004085F4 (in _IO_getc)
;;     00408690 (in perror)
;;     00408740 (in _IO_putc)
;;     004087C6 (in puts)
;;     00409AA4 (in vfprintf)
;;     0040D4E6 (in __isoc99_vfscanf)
__unlockfile proc
	save	00000010,ra,00000002
	sync	00000000
	sw	r0,004C(r4)
	sync	00000000
	lw	r7,0050(r4)
	beqzc	r7,0040D248

l0040D224:
	addiu	r16,r4,0000004C
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r4,r7,0040D248

l0040D23A:
	li	r7,00000001
	li	r4,00000062
	movep	r5,r6,r16,r7
	restore	00000010,ra,00000002
	bc	00404A50

l0040D248:
	restore.jrc	00000010,ra,00000002
0040D24A                               00 00 00 00 00 00           ......

;; __overflow: 0040D250
;;   Called from:
;;     0040839E (in fputc)
;;     004083DC (in fputc)
;;     0040871E (in _IO_putc)
;;     0040875C (in _IO_putc)
;;     004087D0 (in puts)
__overflow proc
	save	00000020,ra,00000002
	lw	r7,0010(r4)
	move	r16,r4
	sb	r5,000F(sp)
	beqzc	r7,0040D27A

l0040D25C:
	lwm	r6,0010(r16),00000002
	bltuc	r7,r6,0040D284

l0040D264:
	lw	r7,0024(r16)
	li	r6,00000001
	addiu	r5,sp,0000000F
	move	r4,r16
	jalrc	ra,r7
	bneiuc	r4,00000001,0040D280

l0040D274:
	lbu	r4,000F(sp)
	restore.jrc	00000020,ra,00000002

l0040D27A:
	balc	0040D3A0
	beqzc	r4,0040D25C

l0040D280:
	li	r4,FFFFFFFF
	restore.jrc	00000020,ra,00000002

l0040D284:
	lbu	r4,000F(sp)
	lb	r6,004B(r16)
	beqc	r4,r6,0040D264

l0040D290:
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r4,0000(r7)
	restore.jrc	00000020,ra,00000002
0040D29A                               00 00 00 00 00 00           ......

;; __stdio_write: 0040D2A0
;;   Called from:
;;     0040D34C (in __stdout_write)
__stdio_write proc
	save	00000030,ra,00000006
	lw	r7,001C(r4)
	move	r16,r4
	lw	r18,0014(r4)
	move	r19,r6
	addiu	r20,r0,00000002
	move	r17,sp
	subu	r18,r18,r7
	sw	r7,0000(sp)
	sw	r18,0004(sp)
	addu	r18,r18,r6
	sw	r5,0008(sp)
	sw	r6,000C(sp)
	bc	0040D2F2

l0040D2BE:
	lw	r7,002C(r16)
	lw	r6,0030(r16)
	sw	r7,001C(sp)
	addu	r6,r7,r6
	swm	r6,0010(r16),00000002

l0040D2CA:
	move	r4,r19
	restore.jrc	00000030,ra,00000006

l0040D2CE:
	lw	r7,0000(r16)
	sw	r0,0010(sp)
	ori	r7,r7,00000020
	sw	r0,0014(sp)
	sw	r7,0000(sp)
	sw	r0,001C(sp)
	beqic	r20,00000002,0040D318

l0040D2E0:
	lw	r7,0004(r17)
	subu	r19,r19,r7
	bc	0040D2CA

l0040D2E6:
	lw	r7,0000(r17)
	addu	r7,r7,r4
	sw	r7,0040(sp)
	lw	r7,0004(r17)
	subu	r4,r7,r4
	sw	r4,0044(sp)

l0040D2F2:
	lw	r5,003C(r16)
	li	r4,00000042
	movep	r6,r7,r17,r20
	balc	00404A50
	balc	0040CC30
	beqc	r18,r4,0040D2BE

l0040D304:
	bltc	r4,r0,0040D2CE

l0040D308:
	lw	r7,0004(r17)
	subu	r18,r18,r4
	bgeuc	r7,r4,0040D2E6

l0040D310:
	subu	r4,r4,r7
	addiu	r17,r17,00000008
	addiu	r20,r20,FFFFFFFF
	bc	0040D2E6

l0040D318:
	move	r19,r0
	bc	0040D2CA
0040D31C                                     00 00 00 00             ....

;; __stdout_write: 0040D320
__stdout_write proc
	save	00000020,ra,00000004
	movep	r16,r17,r4,r5
	addiupc	r7,FFFFFF78
	move	r18,r6
	sw	r7,0024(sp)
	lw	r7,0000(r16)
	bbnezc	r7,00000006,0040D34A

l0040D332:
	lw	r5,003C(r16)
	addiu	r7,sp,00000008
	li	r6,40087468
	li	r4,0000001D
	balc	00404A50
	beqzc	r4,0040D34A

l0040D344:
	li	r7,FFFFFFFF
	sb	r7,004B(r16)

l0040D34A:
	movep	r5,r6,r17,r18
	move.balc	r4,r16,0040D2A0
	restore.jrc	00000020,ra,00000004
0040D352       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __string_read: 0040D360
;;   Called from:
;;     00409B90 (in do_read)
__string_read proc
	save	00000020,ra,00000007
	movep	r19,r21,r4,r5
	addiu	r18,r6,00000100
	move	r20,r6
	lw	r16,0054(r19)
	movep	r5,r6,r0,r18
	move.balc	r4,r16,0040A050
	beqzc	r4,0040D378

l0040D376:
	subu	r18,r4,r16

l0040D378:
	sltu	r17,r20,r18
	movz	r20,r18,r17

l0040D380:
	move	r17,r20
	movep	r5,r6,r16,r17
	move.balc	r4,r21,0040A130
	addu	r7,r16,r17
	addu	r16,r16,r18
	move	r4,r20
	sw	r7,0044(sp)
	sw	r16,0002(r19)
	sw	r16,0054(r19)
	restore.jrc	00000020,ra,00000007
0040D398                         00 00 00 00 00 00 00 00         ........

;; __towrite: 0040D3A0
;;   Called from:
;;     00408530 (in __fwritex)
;;     0040D27A (in __overflow)
__towrite proc
	lb	r7,004A(r4)
	addiu	r6,r7,FFFFFFFF
	or	r7,r7,r6
	sb	r7,004A(r4)
	lw	r7,0000(r4)
	bbnezc	r7,00000003,0040D3C8

l0040D3B4:
	lw	r7,002C(r4)
	lw	r6,0030(r4)
	sw	r7,0014(sp)
	sw	r7,001C(sp)
	addu	r7,r7,r6
	sw	r0,0004(sp)
	sw	r0,0008(sp)
	sw	r7,0010(sp)
	move	r4,r0
	jrc	ra

l0040D3C8:
	ori	r7,r7,00000020
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	jrc	ra

;; __towrite_needs_stdio_exit: 0040D3D2
__towrite_needs_stdio_exit proc
	bc	0040E720
0040D3D6                   00 00 00 00 00 00 00 00 00 00       ..........

;; __uflow: 0040D3E0
;;   Called from:
;;     0040828A (in fgets_unlocked)
;;     004085DA (in _IO_getc)
;;     00408606 (in _IO_getc)
;;     0040CB8E (in __shgetc)
__uflow proc
	save	00000020,ra,00000002
	move	r16,r4
	balc	0040E750
	bnezc	r4,0040D400

l0040D3EA:
	lw	r7,0020(r16)
	li	r6,00000001
	addiu	r5,sp,0000000F
	move	r4,r16
	jalrc	ra,r7
	bneiuc	r4,00000001,0040D400

l0040D3FA:
	lbu	r4,000F(sp)
	restore.jrc	00000020,ra,00000002

l0040D400:
	li	r4,FFFFFFFF
	restore.jrc	00000020,ra,00000002
0040D404             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; store_int: 0040D410
;;   Called from:
;;     0040D72E (in __isoc99_vfscanf)
;;     0040DAF0 (in __isoc99_vfscanf)
store_int proc
	beqzc	r4,0040D432

l0040D412:
	addiu	r5,r5,00000002
	bgeiuc	r5,00000006,0040D432

l0040D418:
	addiupc	r8,00006020
	lwxs	r5,r5(r8)
	jrc	r5
0040D422       44 5F E0 DB 41 7F E0 DB 40 97 E0 DB C4 A4   D_..A...@.....
0040D430 00 2C                                           .,              

l0040D432:
	jrc	ra

;; arg_n: 0040D434
;;   Called from:
;;     0040D5E0 (in __isoc99_vfscanf)
arg_n proc
	save	00000030,ra,00000002
	li	r6,00000010
	move	r16,r5
	move	r5,r4
	addu	r4,sp,r6
	balc	0040A130
	li	r6,00000010
	addu	r5,sp,r6
	move	r4,sp
	balc	0040A130
	lb	r7,000C(sp)
	move	r6,r0
	move	r4,r0
	lw	r17,0000(sp)
	bc	0040D462

l0040D45C:
	addiu	r5,r5,00000004
	li	r4,00000001

l0040D460:
	addiu	r16,r16,FFFFFFFF

l0040D462:
	bltiuc	r16,00000002,0040D478

l0040D466:
	bgec	r0,r7,0040D45C

l0040D46A:
	addiu	r7,r7,FFFFFFFC
	li	r6,00000001
	seb	r7,r7
	bgec	r7,r0,0040D460

l0040D476:
	bc	0040D45C

l0040D478:
	beqzc	r4,0040D47C

l0040D47A:
	sw	r5,0000(sp)

l0040D47C:
	beqzc	r6,0040D482

l0040D47E:
	sb	r7,000C(sp)

l0040D482:
	bgec	r0,r7,0040D49E

l0040D486:
	addiu	r6,r7,FFFFFFFC
	seb	r6,r6
	sb	r6,000C(sp)
	bltc	r6,r0,0040D49E

l0040D496:
	lw	r17,0004(sp)
	subu	r7,r6,r7

l0040D49A:
	lw	r4,0000(r7)
	restore.jrc	00000030,ra,00000002

l0040D49E:
	lw	r17,0000(sp)
	addiu	r6,r7,00000004
	sw	r6,0000(sp)
	bc	0040D49A

;; __isoc99_vfscanf: 0040D4A6
;;   Called from:
;;     00409BC0 (in __isoc99_vsscanf)
__isoc99_vfscanf proc
	save	00000190,r30,0000000A
	movep	r16,r17,r5,r6
	move	r30,r4
	li	r6,00000010
	addiu	r4,sp,0000004C
	move.balc	r5,r17,0040A130
	lw	r7,0000(r17)
	sw	r0,0020(sp)
	sw	r7,001C(sp)
	lw	r7,0004(r17)
	sw	r7,0024(sp)
	lb	r7,000C(r17)
	sw	r7,0018(sp)
	lw	r7,004C(r30)
	bltc	r7,r0,0040D4D6

l0040D4CE:
	move	r4,r30
	balc	0040D1D0
	sw	r4,0020(sp)

l0040D4D6:
	move	r23,r0
	sw	r0,0008(sp)
	sw	r0,0010(sp)

l0040D4DC:
	lbu	r7,0000(r16)
	bnezc	r7,0040D4F0

l0040D4E0:
	lw	r17,0020(sp)
	beqzc	r7,0040D4EA

l0040D4E4:
	move	r4,r30
	balc	0040D210

l0040D4EA:
	lw	r17,0008(sp)
	restore.jrc	00000190,r30,0000000A

l0040D4F0:
	bneiuc	r7,00000020,0040D4F8

l0040D4F4:
	bc	0040DB50

l0040D4F8:
	addiu	r6,r7,FFFFFFF7
	bgeiuc	r6,00000005,0040D504

l0040D500:
	bc	0040DB50

l0040D504:
	bneiuc	r7,00000025,0040D50E

l0040D508:
	lbu	r5,0001(r16)
	bneiuc	r5,00000025,0040D57C

l0040D50E:
	seqi	r7,r7,00000025
	move	r4,r30
	addu	r16,r16,r7
	movep	r6,r7,r0,r0
	balc	0040CB40
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040D562

l0040D528:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040D532:
	lbu	r7,0000(r16)
	beqc	r4,r7,0040D56A

l0040D538:
	lw	r7,0068(r30)
	beqzc	r7,0040D548

l0040D53E:
	lw	r7,0004(r30)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(r30)

l0040D548:
	bgec	r4,r0,0040D4E0

l0040D54C:
	lw	r17,0008(sp)
	li	r7,FFFFFFFF
	movz	r6,r7,r6

l0040D554:
	sw	r6,0008(sp)
	bc	0040D4E0

l0040D558:
	move	r4,r30
	balc	0040CB78
	bc	0040DB7C

l0040D562:
	move	r4,r30
	balc	0040CB78
	bc	0040D532

l0040D56A:
	addiu	r7,r23,00000001
	sltu	r6,r7,r23
	move	r23,r7
	lw	r17,0010(sp)
	addu	r7,r6,r7
	bc	0040DBC6

l0040D57C:
	bneiuc	r5,0000002A,0040D5C0

l0040D580:
	addiu	r7,r16,00000002
	move	r18,r0

l0040D586:
	sw	r0,000C(sp)

l0040D588:
	lbu	r6,0000(r7)
	addiu	r5,r7,00000001
	addiu	r4,r6,FFFFFFD0
	bltiuc	r4,0000000A,0040D620

l0040D596:
	sw	r0,0014(sp)
	bneiuc	r6,0000002D,0040D5A8

l0040D59C:
	sltu	r7,r0,r18
	move	r20,r0
	sw	r7,0014(sp)
	move	r19,r0
	move	r7,r5

l0040D5A8:
	lbu	r6,0000(r7)
	addiu	r16,r7,00000001
	addiu	r6,r6,FFFFFFBF
	andi	r6,r6,000000FF
	bgeiuc	r6,0000003A,0040D7B4

l0040D5B8:
	addiupc	r5,00005E98
	lwxs	r6,r6(r5)
	jrc	r6

l0040D5C0:
	addiu	r5,r5,FFFFFFD0
	bgeiuc	r5,0000000A,0040D5EC

l0040D5C8:
	lbu	r7,0002(r16)
	bneiuc	r7,00000024,0040D5EC

l0040D5CE:
	lw	r17,001C(sp)
	addiu	r4,sp,0000004C
	sw	r7,004C(sp)
	lw	r17,0024(sp)
	sw	r7,0050(sp)
	lbu	r7,0018(sp)
	sb	r7,0058(sp)
	balc	0040D434
	move	r18,r4
	addiu	r7,r16,00000003
	bc	0040D586

l0040D5EC:
	lw	r17,0018(sp)
	bgec	r0,r7,0040D618

l0040D5F2:
	addiu	r6,r7,FFFFFFFC
	seb	r6,r6
	bltc	r6,r0,0040D616

l0040D5FE:
	lw	r17,0024(sp)
	lwm	r4,0018(sp),00000002
	subu	r7,r7,r4
	sw	r7,001C(sp)

l0040D608:
	lw	r17,001C(sp)
	addiu	r7,r16,00000001
	sw	r6,0018(sp)
	sw	r5,001C(sp)
	lw	r18,0000(r4)
	bc	0040D586

l0040D616:
	sw	r6,0018(sp)

l0040D618:
	lwm	r6,0018(sp),00000002
	addiu	r5,r7,00000004
	bc	0040D608

l0040D620:
	lw	r17,000C(sp)
	li	r7,0000000A
	mul	r7,r7,r4
	addu	r7,r7,r6
	addiu	r7,r7,FFFFFFD0
	sw	r7,000C(sp)
	move	r7,r5

l0040D630:
	bc	0040D588
0040D632       79 5F A0 82 01 80 D3 C8 08 40 07 02 02 00   y_.......@....
0040D640 A0 82 02 80                                     ....            

l0040D644:
	lbu	r17,0000(r16)
	andi	r7,r17,0000002F
	bneiuc	r7,00000003,0040D656

l0040D64E:
	ori	r17,r17,00000020
	addiu	r21,r0,00000001

l0040D656:
	beqic	r17,00000023,0040D718

l0040D65A:
	beqic	r17,0000002E,0040D72A

l0040D65E:
	bneiuc	r17,0000001B,0040D736

l0040D662:
	lw	r17,000C(sp)
	move	r4,r30
	lw	r17,000C(sp)
	sra	r7,r7,0000001F
	sw	r7,0028(sp)
	balc	0040CB40
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040D7AA

l0040D67E:
	addiu	r7,r7,00000001
	sw	r7,0004(r30)

l0040D684:
	lw	r7,0068(r30)
	beqzc	r7,0040D694

l0040D68A:
	lw	r7,0004(r30)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(r30)

l0040D694:
	beqic	r17,00000024,0040DAE6

l0040D698:
	bgeic	r17,00000025,0040D83A

l0040D69C:
	beqic	r17,00000018,0040D8A0

l0040D6A0:
	bgeic	r17,00000019,0040D7C0

l0040D6A4:
	beqic	r17,00000001,0040DAF8

l0040D6A8:
	bltic	r17,00000001,0040D6B4

l0040D6AC:
	addiu	r17,r17,FFFFFFBB
	bltiuc	r17,00000003,0040DAF8

l0040D6B4:
	lw	r6,0008(r30)
	lw	r7,0004(r30)
	lw	r4,007C(r30)
	subu	r7,r7,r6
	lw	r6,0078(r30)
	sra	r5,r7,0000001F
	addu	r6,r7,r6
	addu	r5,r5,r4
	sltu	r7,r6,r7
	lw	r17,0010(sp)
	addu	r7,r7,r5
	addu	r6,r6,r23
	sltu	r5,r6,r23
	addu	r7,r4,r7
	addu	r7,r5,r7
	move	r23,r6
	sw	r7,0010(sp)
	beqc	r0,r18,0040DBC8

l0040D6E8:
	lw	r17,0008(sp)
	addiu	r7,r7,00000001
	sw	r7,0008(sp)
	bc	0040DBC8

l0040D6F0:
	addiupc	r6,FFF65F78
0040D6F2       79 5F A0 02 01 00 D3 C8 49 67               y_......Ig    

l0040D6FC:
	addiu	r16,r7,00000002

l0040D700:
	addiu	r21,r0,00000003

l0040D704:
	bc	0040D644
0040D706                   A0 02 02 00 39 1B                   ....9.    

l0040D70C:
	move	r16,r7
	move	r21,r0

l0040D710:
	bc	0040D644
0040D712       A0 02 01 00 2D 1B                           ....-.        

l0040D718:
	lw	r17,000C(sp)
	lw	r17,000C(sp)
	slt	r6,r0,r7
	li	r7,00000001
	movz	r5,r7,r6

l0040D726:
	sw	r5,000C(sp)
	bc	0040D662

l0040D72A:
	lw	r17,0010(sp)
	movep	r5,r6,r21,r23
	move.balc	r4,r18,0040D410
	bc	0040DBC8

l0040D736:
	movep	r6,r7,r0,r0
	move	r4,r30
	balc	0040CB40

l0040D73E:
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040D7A2

l0040D74A:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040D754:
	beqic	r4,00000020,0040D73E

l0040D758:
	addiu	r4,r4,FFFFFFF7
	bltiuc	r4,00000005,0040D73E

l0040D760:
	lw	r7,0068(r30)
	beqzc	r7,0040D770

l0040D766:
	lw	r7,0004(r30)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(r30)

l0040D770:
	lw	r6,0008(r30)
	lw	r7,0004(r30)
	lw	r4,007C(r30)
	subu	r7,r7,r6
	lw	r6,0078(r30)
	sra	r5,r7,0000001F
	addu	r6,r7,r6
	addu	r5,r5,r4
	sltu	r7,r6,r7
	lw	r17,0010(sp)
	addu	r7,r7,r5
	addu	r6,r6,r23
	sltu	r5,r6,r23
	addu	r7,r4,r7
	addu	r7,r5,r7
	move	r23,r6
	sw	r7,0010(sp)
	bc	0040D662

l0040D7A2:
	move	r4,r30
	balc	0040CB78
	bc	0040D754

l0040D7AA:
	move	r4,r30
	balc	0040CB78
	bgec	r4,r0,0040D684

l0040D7B4:
	lw	r17,0008(sp)
	li	r7,FFFFFFFF
	movz	r6,r7,r6

l0040D7BC:
	sw	r6,0008(sp)
	bc	0040D974

l0040D7C0:
	beqic	r17,00000021,0040DAF8

l0040D7C4:
	beqic	r17,00000023,0040D8AE

l0040D7C8:
	bneiuc	r17,0000001B,0040D6B4

l0040D7CC:
	lbu	r7,0001(r16)
	beqic	r7,0000001E,0040D986

l0040D7D2:
	addiu	r16,r16,00000001
	move	r22,r0

l0040D7D6:
	addiu	r8,sp,0000005C
	addiu	r6,r0,00000101
	movep	r4,r5,r8,r22
	sw	r8,002C(sp)
	balc	0040A690
	lbu	r6,0000(r16)
	addiu	r7,sp,FFFFF160
	lw	r18,002C(sp)
	sb	r0,0EFC(r7)
	bneiuc	r6,0000002D,0040D98E

l0040D7F6:
	li	r6,00000001
	addiu	r16,r16,00000001
	subu	r6,r6,r22
	sb	r6,0F2A(r7)

l0040D802:
	lbu	r7,0000(r16)
	beqic	r7,0000001D,0040D8DA

l0040D808:
	beqzc	r7,0040D7B4

l0040D80A:
	bneiuc	r7,0000002D,0040D824

l0040D80E:
	lbu	r6,0001(r16)
	beqzc	r6,0040D824

l0040D812:
	beqic	r6,0000001D,0040D824

l0040D816:
	lbu	r7,-0001(r16)

l0040D81A:
	xori	r5,r22,00000001
	bltc	r7,r6,0040D9A0

l0040D822:
	addiu	r16,r16,00000001

l0040D824:
	lbu	r7,0000(r16)
	addiu	r6,sp,FFFFF160
	addiu	r16,r16,00000001
	addu	r7,r6,r7
	li	r6,00000001
	subu	r6,r6,r22
	sb	r6,0EFD(r7)
	bc	0040D802

l0040D83A:
	beqic	r17,0000002F,0040DAEA

l0040D83E:
	bgeic	r17,00000030,0040D894

l0040D842:
	bltic	r17,00000028,0040DAF8

l0040D846:
	move	r5,r0
	bneiuc	r17,00000029,0040D6B4

l0040D84C:
	addiu	r8,r0,FFFFFFFF
	addiu	r9,r0,FFFFFFFF
	move	r6,r0
	move	r4,r30
	balc	0040C670
	lw	r22,0004(r30)
	movep	r6,r7,r4,r5
	lw	r5,0008(r30)
	lw	r8,0078(r30)
	subu	r22,r22,r5
	lw	r5,007C(r30)
	sra	r9,r22,0000001F
	addu	r8,r8,r22
	sltu	r22,r8,r22
	addu	r9,r9,r5
	addu	r22,r22,r9
	or	r22,r8,r22
	beqc	r0,r22,0040D974

l0040D888:
	bneiuc	r17,00000030,0040DAEE

l0040D88C:
	beqc	r0,r18,0040DAEE

l0040D890:
	sw	r6,0000(sp)
	bc	0040D6B4

l0040D894:
	beqic	r17,00000033,0040D8AE

l0040D898:
	bgeic	r17,00000034,0040D8A4

l0040D89C:
	bneiuc	r17,00000030,0040D6B4

l0040D8A0:
	li	r5,00000010
	bc	0040D84C

l0040D8A4:
	beqic	r17,00000035,0040DAE6

l0040D8A8:
	beqic	r17,00000038,0040D8A0

l0040D8AC:
	bc	0040D6B4

l0040D8AE:
	addiu	r6,r0,00000101
	li	r5,FFFFFFFF
	addiu	r4,sp,0000005C
	balc	0040A690
	sb	r0,005C(sp)
	bneiuc	r17,00000033,0040D9A8

l0040D8C2:
	sb	r0,0066(sp)
	sb	r0,0067(sp)
	sb	r0,0068(sp)
	sb	r0,0069(sp)
	sb	r0,006A(sp)
	sb	r0,007D(sp)

l0040D8DA:
	addiu	r22,r0,0000001F

l0040D8DE:
	bneiuc	r21,00000001,0040DA14

l0040D8E2:
	lw	r17,0014(sp)
	move	r20,r18
	beqzc	r7,0040D8F6

l0040D8E8:
	sll	r4,r22,00000002
	balc	00405292
	move	r20,r4
	beqc	r0,r4,0040DBF8

l0040D8F6:
	move	r8,r0
	sw	r0,0044(sp)
	sw	r0,0048(sp)

l0040D8FC:
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040DA0A

l0040D908:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040D912:
	addiu	r7,sp,FFFFF160
	sw	r8,002C(sp)
	addu	r7,r7,r4
	lbu	r7,0EFD(r7)
	bnec	r0,r7,0040D9B8

l0040D922:
	addiu	r4,sp,00000044
	move	r19,r0
	balc	0040E400
	beqc	r0,r4,0040D7B4

l0040D92E:
	lw	r18,002C(sp)

l0040D930:
	lw	r6,0068(r30)
	lw	r7,0004(r30)
	beqzc	r6,0040D940

l0040D93A:
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(r30)

l0040D940:
	lw	r6,0008(r30)
	lw	r7,0004(r30)
	lw	r4,007C(r30)
	subu	r7,r7,r6
	lw	r6,0078(r30)
	sra	r5,r7,0000001F
	addu	r6,r7,r6
	addu	r5,r5,r4
	sltu	r7,r6,r7
	addu	r7,r7,r5
	or	r5,r6,r7
	beqzc	r5,0040D974

l0040D966:
	bneiuc	r17,00000023,0040DBCE

l0040D96A:
	lw	r17,000C(sp)
	bnec	r5,r6,0040D974

l0040D96E:
	lw	r17,0028(sp)
	beqc	r6,r7,0040DBF0

l0040D974:
	lw	r17,0014(sp)
	beqc	r0,r7,0040D4E0

l0040D97A:
	move.balc	r4,r19,00404F2E
	move.balc	r4,r20,00404F2E
	bc	0040D4E0

l0040D986:
	addiu	r16,r16,00000002
	addiu	r22,r0,00000001
	bc	0040D7D6

l0040D98E:
	bneiuc	r6,0000001D,0040D802

l0040D992:
	li	r6,00000001
	addiu	r16,r16,00000001
	subu	r6,r6,r22
	sb	r6,0F5A(r7)
	bc	0040D802

l0040D9A0:
	addiu	r7,r7,00000001
	sbx	r5,r8(r7)
	bc	0040D81A

l0040D9A8:
	addiu	r22,r0,0000001F
	bneiuc	r17,00000023,0040D8DE

l0040D9B0:
	lw	r17,000C(sp)
	addiu	r22,r7,00000001
	bc	0040D8DE

l0040D9B8:
	addiu	r7,sp,00000044
	sb	r4,003F(sp)
	li	r6,00000001
	addiu	r5,sp,0000003F
	addiu	r4,sp,00000040
	balc	0040E320
	addiu	r7,r0,FFFFFFFE
	lw	r18,002C(sp)
	beqc	r4,r7,0040D8FC

l0040D9D4:
	li	r7,FFFFFFFF
	beqc	r4,r7,0040DB4A

l0040D9DA:
	beqc	r0,r20,0040D9E6

l0040D9DE:
	lw	r17,0040(sp)
	swxs	r7,r8(r20)
	addiu	r8,r8,00000001

l0040D9E6:
	lw	r17,0014(sp)
	beqc	r0,r7,0040D8FC

l0040D9EC:
	bnec	r8,r22,0040D8FC

l0040D9F0:
	sll	r22,r8,00000001
	addiu	r22,r22,00000001
	sw	r8,002C(sp)
	sll	r5,r22,00000002
	move.balc	r4,r20,004057D0
	beqc	r0,r4,0040DBFA

l0040DA04:
	move	r20,r4
	lw	r18,002C(sp)
	bc	0040D8FC

l0040DA0A:
	move	r4,r30
	sw	r8,002C(sp)
	balc	0040DC0C
	lw	r18,002C(sp)
	bc	0040D912

l0040DA14:
	lw	r17,0014(sp)
	beqzc	r7,0040DA76

l0040DA18:
	move.balc	r4,r22,00405292
	move	r19,r4
	beqc	r0,r4,0040DBF8

l0040DA22:
	move	r8,r0

l0040DA24:
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040DA6C

l0040DA30:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040DA3A:
	addiu	r7,sp,FFFFF160
	addu	r7,r7,r4
	lbu	r7,0EFD(r7)
	bnezc	r7,0040DA4A

l0040DA46:
	move	r20,r0
	bc	0040D930

l0040DA4A:
	addiu	r20,r8,00000001
	sbx	r4,r19(r8)
	move	r4,r19
	bnec	r22,r20,0040DA66

l0040DA58:
	sll	r22,r22,00000001
	addiu	r22,r22,00000001
	move.balc	r5,r22,004057D0
	beqc	r0,r4,0040DC08

l0040DA66:
	move	r8,r20
	move	r19,r4
	bc	0040DA24

l0040DA6C:
	move	r4,r30
	sw	r8,002C(sp)
	balc	0040DC0C
	lw	r18,002C(sp)
	bc	0040DA3A

l0040DA76:
	move	r19,r0
	bnezc	r18,0040DAAA

l0040DA7A:
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040DADC

l0040DA86:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040DA90:
	addiu	r7,sp,FFFFF160
	addu	r4,r7,r4
	lbu	r7,0EFD(r4)
	bnezc	r7,0040DA7A

l0040DA9C:
	move	r8,r0
	move	r20,r0
	move	r19,r0
	bc	0040D930

l0040DAA4:
	sbx	r4,r18(r19)
	addiu	r19,r19,00000001

l0040DAAA:
	lw	r7,0004(r30)
	move	r8,r19
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040DAD2

l0040DAB8:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040DAC2:
	addiu	r7,sp,FFFFF160
	addu	r7,r7,r4
	lbu	r7,0EFD(r7)
	bnezc	r7,0040DAA4

l0040DACE:
	move	r19,r18
	bc	0040DA46

l0040DAD2:
	move	r4,r30
	sw	r19,002C(sp)
	balc	0040DC0C
	lw	r18,002C(sp)
	bc	0040DAC2

l0040DADC:
	move	r4,r30
	balc	0040DC0C
	bc	0040DA90

l0040DAE2:
	sw	r19,0000(sp)
	bc	0040DBD8

l0040DAE6:
	li	r5,0000000A
	bc	0040D84C

l0040DAEA:
	li	r5,00000008
	bc	0040D84C

l0040DAEE:
	movep	r4,r5,r18,r21
	balc	0040D410
	bc	0040D6B4

l0040DAF8:
	movep	r5,r6,r21,r0
	move	r4,r30
	balc	0040BD5C
	lw	r6,0008(r30)
	lw	r7,0004(r30)
	lw	r22,007C(r30)
	subu	r7,r7,r6
	lw	r6,0078(r30)
	sra	r17,r7,0000001F
	addu	r6,r7,r6
	addu	r17,r17,r22
	sltu	r7,r6,r7
	addu	r7,r7,r17
	or	r7,r7,r6
	movep	r8,r9,r4,r5
	beqc	r0,r7,0040D974

l0040DB28:
	beqc	r0,r18,0040D6B4

l0040DB2C:
	beqic	r21,00000001,0040DB42

l0040DB30:
	beqic	r21,00000002,0040DB42

l0040DB34:
	bnec	r0,r21,0040D6B4

l0040DB38:
	balc	00410220
	sw	r4,0000(sp)
	bc	0040D6B4

l0040DB42:
	swm	r8,0000(r18),00000002
	bc	0040D6B4

l0040DB4A:
	move	r19,r0
	bc	0040D7B4

l0040DB4E:
	addiu	r16,r16,00000001

l0040DB50:
	lbu	r7,0001(r16)
	beqic	r7,00000020,0040DB4E

l0040DB56:
	addiu	r7,r7,FFFFFFF7
	bltiuc	r7,00000005,0040DB4E

l0040DB5E:
	movep	r6,r7,r0,r0
	move	r4,r30
	balc	0040CB40

l0040DB66:
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040D558

l0040DB72:
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)

l0040DB7C:
	beqic	r4,00000020,0040DB66

l0040DB80:
	addiu	r4,r4,FFFFFFF7
	bltiuc	r4,00000005,0040DB66

l0040DB88:
	lw	r7,0068(r30)
	beqzc	r7,0040DB98

l0040DB8E:
	lw	r7,0004(r30)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(r30)

l0040DB98:
	lw	r6,0008(r30)
	lw	r7,0004(r30)
	lw	r4,007C(r30)
	subu	r7,r7,r6
	lw	r6,0078(r30)
	sra	r5,r7,0000001F
	addu	r6,r7,r6
	addu	r5,r5,r4
	sltu	r7,r6,r7
	lw	r17,0010(sp)
	addu	r7,r7,r5
	addu	r6,r6,r23
	sltu	r5,r6,r23
	addu	r7,r4,r7
	move	r23,r6
	addu	r7,r5,r7

l0040DBC6:
	sw	r7,0010(sp)

l0040DBC8:
	addiu	r16,r16,00000001
	bc	0040D4DC

l0040DBCE:
	lw	r17,0014(sp)
	beqzc	r7,0040DBDC

l0040DBD2:
	bneiuc	r21,00000001,0040DAE2

l0040DBD6:
	sw	r20,0000(r18)

l0040DBD8:
	beqic	r17,00000023,0040D6B4

l0040DBDC:
	beqc	r0,r20,0040DBE4

l0040DBE0:
	swxs	r0,r8(r20)

l0040DBE4:
	beqc	r0,r19,0040D6B4

l0040DBE8:
	sbx	r0,r8(r19)
	bc	0040D6B4

l0040DBF0:
	lw	r17,0014(sp)
	beqc	r0,r7,0040D6B4

l0040DBF6:
	bc	0040DBD2

l0040DBF8:
	move	r20,r0

l0040DBFA:
	move	r19,r0

l0040DBFC:
	lw	r17,0008(sp)
	li	r7,FFFFFFFF
	movz	r6,r7,r6

l0040DC04:
	sw	r6,0008(sp)
	bc	0040D97A

l0040DC08:
	move	r20,r0
	bc	0040DBFC

;; fn0040DC0C: 0040DC0C
;;   Called from:
;;     0040DA0E (in __isoc99_vfscanf)
;;     0040DA70 (in __isoc99_vfscanf)
;;     0040DAD6 (in __isoc99_vfscanf)
;;     0040DADE (in __isoc99_vfscanf)
fn0040DC0C proc
	bc	0040CB78

;; stpcpy: 0040DC10
;;   Called from:
;;     0040A864 (in strcpy)
stpcpy proc
	xor	r7,r4,r5
	andi	r7,r7,00000003
	bnezc	r7,0040DC80

l0040DC18:
	andi	r7,r5,00000003
	beqzc	r7,0040DC32

l0040DC1C:
	lbu	r7,0000(r5)
	sb	r7,0000(r4)
	bnezc	r7,0040DC2A

l0040DC22:
	bc	0040DC88

l0040DC24:
	lbu	r7,0000(r5)
	sb	r7,0000(r4)
	beqzc	r7,0040DC88

l0040DC2A:
	addiu	r5,r5,00000001
	addiu	r4,r4,00000001
	andi	r7,r5,00000003
	bnezc	r7,0040DC24

l0040DC32:
	lw	r6,0000(r5)
	movep	r8,r9,r4,r5
	move	r7,r6
	nor	r10,r0,r6
	addiu	r7,r7,FEFEFEFF
	and	r7,r7,r10
	li	r10,80808080
	and	r7,r7,r10
	bnezc	r7,0040DC80

l0040DC52:
	addiu	r8,r8,00000004
	addiu	r9,r9,00000004
	sw	r6,-0004(r8)
	lw	r6,0000(r9)
	move	r7,r6
	nor	r5,r0,r6
	addiu	r7,r7,FEFEFEFF
	and	r7,r7,r5
	li	r5,80808080
	and	r7,r7,r5
	beqzc	r7,0040DC52

l0040DC74:
	movep	r4,r5,r8,r9
	lbu	r7,0000(r5)
	sb	r7,0000(r4)
	beqzc	r7,0040DC88

l0040DC7C:
	addiu	r5,r5,00000001
	addiu	r4,r4,00000001

l0040DC80:
	lbu	r7,0000(r5)
	sb	r7,0000(r4)
	bnezc	r7,0040DC7C

l0040DC86:
	jrc	ra

l0040DC88:
	jrc	ra
0040DC8A                               00 00 00 00 00 00           ......

;; stpncpy: 0040DC90
;;   Called from:
;;     0040A934 (in strncpy)
stpncpy proc
	save	00000010,ra,00000002
	xor	r7,r4,r5
	move	r16,r4
	andi	r7,r7,00000003
	bnezc	r7,0040DD0A

l0040DC9C:
	andi	r7,r5,00000003
	beqzc	r7,0040DCB2

l0040DCA0:
	beqzc	r6,0040DD1A

l0040DCA2:
	lbu	r7,0000(r5)
	sb	r7,0000(r16)
	beqzc	r7,0040DD1A

l0040DCA8:
	addiu	r5,r5,00000001
	addiu	r6,r6,FFFFFFFF
	andi	r7,r5,00000003
	addiu	r16,r16,00000001
	bnezc	r7,0040DCA0

l0040DCB2:
	beqzc	r6,0040DD1A

l0040DCB4:
	lbu	r9,0000(r5)
	beqc	r0,r9,0040DD1A

l0040DCBC:
	bltiuc	r6,00000004,0040DD24

l0040DCC0:
	lw	r4,0000(r5)
	move	r8,r4
	nor	r7,r0,r4
	addiu	r8,r8,FEFEFEFF
	and	r7,r7,r8
	li	r8,80808080
	and	r7,r7,r8
	beqzc	r7,0040DCFE

l0040DCDE:
	bc	0040DD24

l0040DCE0:
	lw	r4,0000(r5)
	move	r7,r4
	nor	r8,r0,r4
	addiu	r7,r7,FEFEFEFF
	and	r7,r7,r8
	li	r8,80808080
	and	r7,r7,r8
	bnezc	r7,0040DD0C

l0040DCFE:
	addiu	r6,r6,FFFFFFFC
	sw	r4,0000(sp)
	addiu	r5,r5,00000004
	addiu	r16,r16,00000004
	bgeiuc	r6,00000004,0040DCE0

l0040DD0A:
	beqzc	r6,0040DD1A

l0040DD0C:
	lbu	r7,0000(r5)
	sb	r7,0000(r16)
	beqzc	r7,0040DD1A

l0040DD12:
	addiu	r6,r6,FFFFFFFF
	addiu	r5,r5,00000001
	addiu	r16,r16,00000001
	bnezc	r6,0040DD0C

l0040DD1A:
	movep	r4,r5,r16,r0
	balc	0040A690
	move	r4,r16
	restore.jrc	00000010,ra,00000002

l0040DD24:
	sb	r9,0000(r16)
	bc	0040DD12
0040DD2A                               00 00 00 00 00 00           ......

;; __set_thread_area: 0040DD30
;;   Called from:
;;     0040B0CA (in __init_tp)
__set_thread_area proc
	move	r5,r4
	addiu	r4,r0,000000F4
	bc	00404A50
0040DD3A                               00 00 00 00 00 00           ......

;; handler: 0040DD40
handler proc
	save	00000040,ra,00000002
	balc	004049B0
	lw	r16,0000(r4)
	addiu	r4,sp,00000010
	movep	r5,r6,r0,r0
	balc	0040E7A0
	addiu	r4,sp,00000020
	movep	r5,r6,r0,r0
	balc	0040E7A0
	addiu	r4,r0,000000B2
	balc	00404A50
	sw	r4,000C(sp)

l0040DD62:
	lwpc	r5,00432F18
	sw	r5,0008(sp)
	sync	00000000

l0040DD6E:
	addiupc	r7,000251A6
	ll	r6,0000(r7)
	bnec	r5,r6,0040DD84

l0040DD78:
	addiu	r7,sp,00000008
	addiupc	r4,0002519A
	sc	r7,0000(r4)
	beqzc	r7,0040DD6E

l0040DD84:
	sync	00000000
	lw	r17,0008(sp)
	bnec	r7,r6,0040DD62

l0040DD8E:
	lw	r17,000C(sp)
	sync	00000000

l0040DD94:
	addiupc	r7,00025174
	ll	r6,0000(r7)
	beqc	r5,r6,0040DDE4

l0040DDA0:
	sync	00000000
	lw	r17,000C(sp)
	lui	r5,FFF80000
	or	r7,r7,r5
	bnec	r6,r7,0040DDBC

l0040DDAE:
	addiu	r6,r0,00000087
	addiupc	r5,00025156
	li	r4,00000062
	balc	00404A50

l0040DDBC:
	addiu	r4,sp,00000010
	balc	0040E850
	lwpc	r7,00432F08
	lwpc	r4,00432F04
	jalrc	ra,r7
	addiu	r4,sp,00000020
	balc	0040E7D0
	addiu	r4,sp,00000010
	balc	0040E850
	balc	004049B0
	sw	r16,0000(r4)
	restore.jrc	00000040,ra,00000002

l0040DDE4:
	move	r7,r0
	addiupc	r4,00025122
	sc	r7,0000(r4)
	beqzc	r7,0040DD94

l0040DDF0:
	bc	0040DDA0

;; __synccall: 0040DDF2
;;   Called from:
;;     0040B060 (in __setxid)
__synccall proc
	save	00000960,ra,00000007
	addiu	r6,r0,00000820
	addiupc	r21,00046632
	movep	r18,r19,r4,r5
	move	r5,r0
	addiu	r4,sp,00000120
	balc	0040E008
	addiu	r6,r0,0000008C
	move	r5,r0
	addiu	r4,sp,00000094
	balc	0040E008
	addiu	r7,sp,FFFFF940
	move	r16,r7
	addiupc	r7,FFFFFF24
	addiu	r4,sp,00000014
	sw	r7,0754(r16)
	lui	r7,00010000
	sw	r7,07D8(r16)
	balc	00407EB4
	addiupc	r4,000250DE
	balc	0040AD30
	move	r4,r0
	balc	00407EA0
	addiu	r5,sp,00000008
	li	r4,00000001
	balc	0040AE50
	lw	r7,0001(r21)
	swpc	r0,00432F18
	beqc	r0,r7,0040DF7E

l0040DE50:
	swpc	r18,00432F08
	swpc	r19,00432F04
	sync	00000000
	li	r7,00000001
	swpc	r7,00432F00
	sync	00000000
	addiu	r6,r0,00000080
	li	r5,FFFFFFFF
	addiu	r4,sp,00000098
	balc	0040E008
	move	r6,r0
	addiu	r5,sp,00000094
	li	r4,00000022
	balc	00407F3A
	addiu	r4,r0,000000AC
	balc	0040E00C
	move	r17,r4
	addiu	r4,r0,000000B2
	balc	0040E00C
	lui	r5,00000090
	move	r20,r4
	addiupc	r4,000041B8
	balc	0040E0A0
	sw	r4,07E0(r16)
	bgec	r4,r0,0040DEF8

l0040DEA4:
	sync	00000000
	swpc	r0,00432F00
	sync	00000000
	li	r7,7FFFFFFF
	addiu	r6,r0,00000081
	addiupc	r5,0002503E
	li	r4,00000062
	balc	0040E00C
	addiu	r7,r0,FFFFFFDA
	bnec	r4,r7,0040DEDE

l0040DECC:
	li	r7,7FFFFFFF
	li	r6,00000001
	addiupc	r5,00025026
	li	r4,00000062
	balc	0040E00C

l0040DEDE:
	lw	r17,0008(sp)
	move	r5,r0
	balc	0040AE50
	addiupc	r4,00025026
	balc	0040AD60
	addiu	r4,sp,00000014
	balc	00407EC8
	restore.jrc	00000960,ra,00000007

l0040DEF8:
	lw	r16,0003(r21)

l0040DEFA:
	bnezc	r16,0040DF14

l0040DEFC:
	move	r16,r0

l0040DEFE:
	addiu	r4,sp,00000120
	balc	0040E010
	bnezc	r4,0040DF22

l0040DF08:
	beqzc	r16,0040DF5E

l0040DF0A:
	addiu	r4,sp,00000120
	balc	0040E070
	bc	0040DEFC

l0040DF14:
	li	r6,00000022
	addiu	r4,r0,00000081
	addiu	r16,r16,FFFFFFFF
	move.balc	r5,r17,00404A50
	bc	0040DEFA

l0040DF22:
	lbu	r7,0013(r4)
	addiu	r7,r7,FFFFFFD0
	bgeiuc	r7,0000000A,0040DEFE

l0040DF2E:
	addiu	r4,r4,00000013
	balc	00409BD0
	move	r6,r4
	beqc	r4,r20,0040DEFE

l0040DF3C:
	beqzc	r4,0040DEFE

l0040DF3E:
	sync	00000000
	swpc	r4,00432F0C
	sync	00000000
	lwpc	r7,00432F18

l0040DF52:
	beqzc	r7,0040DFA8

l0040DF54:
	lw	r5,0004(r7)
	beqc	r5,r6,0040DEFE

l0040DF5A:
	lw	r7,0000(r7)
	bc	0040DF52

l0040DF5E:
	lw	r4,0120(sp)
	balc	0040AF72
	lwpc	r16,00432F18

l0040DF6C:
	bnezc	r16,0040DF8A

l0040DF6E:
	li	r7,00000001
	move	r6,r0
	addiu	r5,sp,00000094
	li	r4,00000022
	sw	r7,0094(sp)
	balc	00407F3A

l0040DF7E:
	move	r4,r19
	jalrc	ra,r18
	lwpc	r4,00432F18
	bc	0040DFA4

l0040DF8A:
	addiu	r4,r16,00000008
	balc	0040E7D0
	addiu	r4,r16,00000018
	balc	0040E850
	lw	r16,0000(r16)
	bc	0040DF6C

l0040DF9A:
	lw	r16,0000(r4)
	addiu	r4,r4,00000008
	balc	0040E7D0
	move	r4,r16

l0040DFA4:
	bnezc	r4,0040DF9A

l0040DFA6:
	bc	0040DEA4

l0040DFA8:
	li	r7,00000022
	addiu	r4,r0,00000083
	move.balc	r5,r17,00404A50
	addiu	r7,r0,FFFFFFFD
	beqc	r4,r7,0040DEFE

l0040DFBA:
	addiu	r5,sp,0000000C
	move	r4,r0
	balc	0040AEF4
	lw	r17,0010(sp)
	li	r5,3B9AC9FF
	move	r6,r7
	addiu	r6,r6,00989680
	sw	r6,0010(sp)
	bgec	r5,r6,0040DFE6

l0040DFD8:
	lw	r17,000C(sp)
	addiu	r7,r7,C4FDCC80
	addiu	r6,r6,00000001
	swm	r6,000C(sp),00000002

l0040DFE6:
	addiu	r7,sp,0000000C
	addiu	r6,r0,00000086
	move	r8,r7
	addiupc	r5,00024F1A
	move	r7,r0
	li	r4,00000062
	balc	0040E00C
	beqc	r0,r4,0040DEFE

l0040DFFC:
	addiu	r7,r0,FFFFFFFD
	beqc	r4,r7,0040DEFE

l0040E004:
	addiu	r16,r16,00000001
	bc	0040DEFE

;; fn0040E008: 0040E008
;;   Called from:
;;     0040DE06 (in __synccall)
;;     0040DE10 (in __synccall)
;;     0040DE74 (in __synccall)
fn0040E008 proc
	bc	0040A690

;; fn0040E00C: 0040E00C
;;   Called from:
;;     0040DE84 (in __synccall)
;;     0040DE8C (in __synccall)
;;     0040DEC4 (in __synccall)
;;     0040DEDC (in __synccall)
;;     0040DFF6 (in __synccall)
fn0040E00C proc
	bc	00404A50

;; readdir64: 0040E010
;;   Called from:
;;     0040DF02 (in __synccall)
readdir64 proc
	save	00000010,ra,00000004
	move	r16,r4
	addiu	r17,r4,00000020
	lwm	r6,0010(r4),00000002
	bltc	r6,r7,0040E050

l0040E020:
	lw	r5,0000(r16)
	addiu	r7,r0,00000800
	move	r6,r17
	li	r4,0000003D
	balc	00404A50
	move	r18,r4
	bltc	r0,r4,0040E04C

l0040E034:
	addiu	r7,r4,00000002
	move	r17,r0
	ins	r7,r0,00000001,00000001
	beqzc	r7,0040E064

l0040E040:
	balc	004049B0
	subu	r18,r0,r18
	sw	r18,0000(sp)
	bc	0040E064

l0040E04C:
	sw	r0,0010(sp)
	sw	r4,0014(sp)

l0040E050:
	lw	r6,0010(r16)
	addu	r17,r17,r6
	lhu	r7,0010(r17)
	addu	r7,r7,r6
	lw	r6,0008(r17)
	sw	r7,0010(sp)
	lw	r7,000C(r17)
	swm	r6,0008(r16),00000002

l0040E064:
	move	r4,r17
	restore.jrc	00000010,ra,00000004
0040E068                         00 00 00 00 00 00 00 00         ........

;; rewinddir: 0040E070
;;   Called from:
;;     0040DF0E (in __synccall)
rewinddir proc
	save	00000010,ra,00000003
	move	r16,r4
	addiu	r17,r4,00000018
	move.balc	r4,r17,0040AD30
	lw	r4,0000(r16)
	movep	r6,r7,r0,r0
	move	r8,r0
	balc	0040E860
	move	r4,r17
	movep	r6,r7,r0,r0
	swm	r6,0008(r16),00000002
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	restore	00000010,ra,00000003
	bc	0040AD60
0040E098                         00 00 00 00 00 00 00 00         ........

;; open64: 0040E0A0
;;   Called from:
;;     0040DE98 (in __synccall)
open64 proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000003
	move	r16,r5
	swm	r6,0028(sp),00000002
	swm	r8,0030(sp),00000002
	swm	r10,0038(sp),00000002
	bbnezc	r5,00000006,0040E0C4

l0040E0B8:
	lui	r7,00000410
	move	r8,r0
	and	r6,r5,r7
	bnec	r6,r7,0040E0DA

l0040E0C4:
	addiu	r7,sp,00000040
	lw	r18,0028(sp)
	sw	r7,0000(sp)
	sw	r7,0004(sp)
	sw	r7,0008(sp)
	li	r7,00000040
	sb	r7,000D(sp)
	li	r7,00000014
	sb	r7,000C(sp)

l0040E0DA:
	addiu	r7,r0,00008000
	move	r6,r4
	move	r10,r0
	move	r9,r0
	or	r7,r7,r16
	addiu	r5,r0,FFFFFF9C
	li	r4,00000038
	balc	0040ADA4
	move	r17,r4
	bltc	r4,r0,0040E106

l0040E0F6:
	bbeqzc	r16,00000013,0040E106

l0040E0FA:
	move	r5,r4
	li	r7,00000001
	li	r6,00000002
	li	r4,00000019
	balc	00404A50

l0040E106:
	move.balc	r4,r17,0040CC30
	restore	00000020,ra,00000003
	addiu	sp,sp,00000020
	jrc	ra
0040E114             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; copysign: 0040E120
;;   Called from:
;;     0040CF00 (in copysignl)
copysign proc
	ext	r5,r5,00000000,0000001F
	ins	r7,r0,00000000,00000001
	move	r8,r4
	or	r6,r5,r7
	movep	r4,r5,r8,r6
	jrc	ra
0040E132       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; fmod: 0040E140
;;   Called from:
;;     0040CF20 (in fmodl)
fmod proc
	save	00000030,r30,0000000A
	movep	r20,r19,r6,r7
	srl	r6,r20,0000001F
	sll	r7,r19,00000001
	movep	r23,r22,r4,r5
	or	r7,r7,r6
	sll	r5,r20,00000001
	or	r6,r5,r7
	move	r4,r23
	move	r21,r22
	beqzc	r6,0040E178

l0040E15C:
	ext	r16,r19,00000000,0000001F
	lui	r6,0007FF00
	bltuc	r6,r16,0040E178

l0040E168:
	bnec	r16,r6,0040E16E

l0040E16A:
	bnec	r0,r20,0040E178

l0040E16E:
	ext	r16,r21,00000004,0000000B
	addiu	r6,r0,000007FF
	bnec	r16,r6,0040E18C

l0040E178:
	movep	r6,r7,r20,r19
	movep	r4,r5,r23,r22
	balc	00404330
	movep	r6,r7,r4,r5
	balc	0040F5E0

l0040E186:
	movep	r23,r22,r4,r5

l0040E188:
	movep	r4,r5,r23,r22
	restore.jrc	00000030,r30,0000000A

l0040E18C:
	srl	r17,r4,0000001F
	sll	r6,r21,00000001
	or	r6,r6,r17
	sll	r17,r4,00000001
	bltuc	r7,r6,0040E1B4

l0040E19C:
	bnec	r6,r7,0040E1A2

l0040E19E:
	bltuc	r5,r17,0040E1B4

l0040E1A2:
	bnec	r5,r17,0040E188

l0040E1A6:
	bnec	r7,r6,0040E188

l0040E1AA:
	movep	r6,r7,r0,r0
	movep	r4,r5,r23,r22
	balc	00404330
	bc	0040E186

l0040E1B4:
	bnezc	r16,0040E20C

l0040E1B6:
	srl	r6,r4,00000014
	sll	r7,r21,0000000C
	or	r7,r7,r6
	sll	r5,r4,0000000C

l0040E1C4:
	bgec	r7,r0,0040E1FC

l0040E1C8:
	li	r6,00000001
	subu	r6,r6,r16
	move.balc	r5,r21,0040EA50
	movep	r17,r18,r4,r5

l0040E1D2:
	ext	r30,r19,00000004,0000000B
	bnec	r0,r30,0040E22A

l0040E1DA:
	srl	r6,r20,00000014
	sll	r7,r19,0000000C
	or	r7,r7,r6
	sll	r5,r20,0000000C

l0040E1E8:
	bgec	r7,r0,0040E21A

l0040E1EC:
	li	r6,00000001
	subu	r6,r6,r30
	movep	r4,r5,r20,r19
	balc	0040EA50

l0040E1F8:
	move	r19,r4
	bc	0040E244

l0040E1FC:
	srl	r17,r5,0000001F
	sll	r6,r7,00000001
	addiu	r16,r16,FFFFFFFF
	or	r7,r6,r17
	sll	r5,r5,00000001
	bc	0040E1C4

l0040E20C:
	ext	r18,r21,00000000,00000014
	lui	r5,00000100
	move	r17,r4
	or	r18,r18,r5
	bc	0040E1D2

l0040E21A:
	srl	r4,r5,0000001F
	sll	r6,r7,00000001
	addiu	r30,r30,FFFFFFFF
	or	r7,r6,r4
	sll	r5,r5,00000001
	bc	0040E1E8

l0040E22A:
	ext	r19,r19,00000000,00000014
	lui	r5,00000100
	move	r4,r20
	or	r5,r5,r19
	bc	0040E1F8

l0040E238:
	srl	r7,r17,0000001F
	sll	r18,r18,00000001
	or	r18,r18,r7
	sll	r17,r17,00000001
	addiu	r16,r16,FFFFFFFF

l0040E244:
	subu	r20,r17,r19
	subu	r6,r18,r5
	sltu	r7,r17,r20
	move	r4,r20
	subu	r6,r6,r7
	move	r7,r6
	bgec	r30,r16,0040E26A

l0040E258:
	bltc	r6,r0,0040E238

l0040E25C:
	or	r7,r20,r6
	beqc	r0,r7,0040E1AA

l0040E264:
	move	r17,r20
	move	r18,r6
	bc	0040E238

l0040E26A:
	bltc	r6,r0,0040E29A

l0040E26E:
	or	r6,r20,r6
	beqc	r0,r6,0040E1AA

l0040E276:
	srl	r6,r7,00000014
	beqzc	r6,0040E2A0

l0040E27C:
	bgec	r0,r16,0040E2B0

l0040E280:
	addiu	r7,r7,FFF00000
	sll	r5,r16,00000014
	or	r5,r5,r7

l0040E28C:
	move	r6,r21
	move	r23,r4
	ins	r6,r0,00000000,00000001
	or	r22,r6,r5
	bc	0040E188

l0040E29A:
	move	r4,r17
	move	r7,r18
	bc	0040E276

l0040E2A0:
	srl	r6,r4,0000001F
	sll	r5,r7,00000001
	or	r7,r5,r6
	sll	r4,r4,00000001
	addiu	r16,r16,FFFFFFFF
	bc	0040E276

l0040E2B0:
	li	r6,00000001
	subu	r6,r6,r16
	move.balc	r5,r7,0040EA80
	bc	0040E28C
0040E2BA                               00 00 00 00 00 00           ......

;; frexp: 0040E2C0
;;   Called from:
;;     0040CF30 (in frexpl)
;;     0040E2EA (in frexp)
frexp proc
	save	00000010,ra,00000004
	movep	r17,r16,r4,r5
	ext	r7,r16,00000004,0000000B
	move	r18,r6
	move	r6,r16
	bnezc	r7,0040E2FE

l0040E2CE:
	movep	r6,r7,r0,r0
	balc	0040FA70
	beqzc	r4,0040E2FA

l0040E2D6:
	lui	r7,00000412
	lw	r6,02C8(r7)
	lw	r7,02CC(r7)
	movep	r4,r5,r17,r16
	balc	00404330
	move	r6,r18
	balc	0040E2C0
	lw	r7,0000(r18)
	movep	r17,r16,r4,r5
	addiu	r7,r7,FFFFFFC0
	sw	r7,0000(sp)

l0040E2F6:
	movep	r4,r5,r17,r16
	restore.jrc	00000010,ra,00000004

l0040E2FA:
	sw	r0,0000(sp)
	bc	0040E2F6

l0040E2FE:
	addiu	r5,r0,000007FF
	beqc	r5,r7,0040E2F6

l0040E306:
	addiu	r7,r7,FFFFFC02
	ins	r6,r0,00000004,00000001
	lui	r16,0003FE00
	sw	r7,0000(sp)
	or	r16,r16,r6
	bc	0040E2F6
0040E318                         00 00 00 00 00 00 00 00         ........

;; mbrtowc: 0040E320
;;   Called from:
;;     0040D9C6 (in __isoc99_vfscanf)
mbrtowc proc
	save	00000020,ra,00000001
	move	r2,r4
	addiupc	r4,00024BF8
	movz	r7,r4,r7

l0040E32C:
	lw	r9,0000(r7)
	bnezc	r5,0040E338

l0040E330:
	bnec	r0,r9,0040E3F2

l0040E334:
	move	r4,r0
	restore.jrc	00000020,ra,00000001

l0040E338:
	bnec	r0,r2,0040E340

l0040E33C:
	addiu	r2,sp,0000000C

l0040E340:
	addiu	r4,r0,FFFFFFFE
	beqc	r0,r6,0040E3FE

l0040E348:
	bnec	r0,r9,0040E3A0

l0040E34C:
	lbu	r8,0000(r5)
	seb	r4,r8
	bltc	r4,r0,0040E362

l0040E358:
	sltu	r4,r0,r8
	sw	r8,0000(r2)
	restore.jrc	00000020,ra,00000001

l0040E362:
	rdhwr	r3,0000001D,00000000
	lw	r4,-0038(r3)
	lw	r4,0000(r4)
	bnezc	r4,0040E37E

l0040E36E:
	lb	r7,0000(r5)
	addiu	r6,r0,0000DFFF
	li	r4,00000001
	and	r7,r7,r6
	sw	r7,0000(r2)
	restore.jrc	00000020,ra,00000001

l0040E37E:
	addiu	r8,r8,FFFFFF3E
	bgeiuc	r8,00000033,0040E3F2

l0040E386:
	addiupc	r4,00004FE6
	addiu	r5,r5,00000001
	lwxs	r9,r8(r4)
	addiu	r8,r6,FFFFFFFF
	bnec	r0,r8,0040E3A2

l0040E398:
	addiu	r4,r0,FFFFFFFE
	sw	r9,0000(r7)
	restore.jrc	00000020,ra,00000001

l0040E3A0:
	move	r8,r6

l0040E3A2:
	lbu	r11,0000(r5)
	sra	r10,r9,0000001A
	srl	r11,r11,00000003
	addu	r10,r10,r11
	addiu	r11,r11,FFFFFFF0
	or	r10,r10,r11
	ins	r10,r0,00000000,00000001
	bnec	r0,r10,0040E3F2

l0040E3C0:
	addiu	r5,r5,00000001
	sll	r9,r9,00000006
	lbu	r4,-0001(r5)
	addiu	r8,r8,FFFFFFFF
	addiu	r4,r4,FFFFFF80
	or	r9,r4,r9
	bltc	r9,r0,0040E3E4

l0040E3D8:
	subu	r4,r6,r8
	sw	r9,0000(r2)
	sw	r0,0040(sp)
	restore.jrc	00000020,ra,00000001

l0040E3E4:
	beqc	r0,r8,0040E398

l0040E3E8:
	lbu	r4,0000(r5)
	addiu	r4,r4,FFFFFF80
	bltiuc	r4,00000000,0040E3C0

l0040E3F2:
	sw	r0,0040(sp)
	balc	004049B0
	li	r7,00000054
	sw	r7,0000(sp)
	li	r4,FFFFFFFF

l0040E3FE:
	restore.jrc	00000020,ra,00000001

;; mbsinit: 0040E400
;;   Called from:
;;     0040D926 (in __isoc99_vfscanf)
mbsinit proc
	li	r7,00000001
	beqzc	r4,0040E40A

l0040E404:
	lw	r7,0000(r4)
	sltiu	r7,r7,00000001

l0040E40A:
	move	r4,r7
	jrc	ra
0040E40E                                           00 00               ..

;; mbsrtowcs: 0040E410
;;   Called from:
;;     0040CFE8 (in mbstowcs)
mbsrtowcs proc
	save	00000010,ra,00000004
	movep	r17,r18,r4,r5
	lw	r16,0000(r18)
	beqzc	r7,0040E458

l0040E418:
	lw	r8,0000(r7)
	beqc	r0,r8,0040E458

l0040E41E:
	beqc	r0,r17,0040E544

l0040E422:
	sw	r0,0040(sp)
	move	r7,r6

l0040E426:
	lbu	r9,0000(r16)
	sra	r4,r8,0000001A
	srl	r5,r9,00000003
	addu	r4,r4,r5
	addiu	r5,r5,FFFFFFF0
	or	r4,r4,r5
	ins	r4,r0,00000000,00000001
	bnec	r0,r4,0040E530

l0040E442:
	sll	r4,r8,00000006
	addiu	r8,r9,FFFFFF80
	or	r8,r8,r4
	bltc	r8,r0,0040E5CA

l0040E452:
	addiu	r16,r16,00000001

l0040E454:
	sw	r8,0000(r17)
	bc	0040E4BC

l0040E458:
	rdhwr	r3,0000001D,00000000
	lw	r7,-0038(r3)
	lw	r7,0000(r7)
	bnezc	r7,0040E49A

l0040E464:
	move	r5,r6
	bnezc	r17,0040E48A

l0040E468:
	move	r4,r16
	restore	00000010,ra,00000004
	bc	0040A890

l0040E472:
	lbu	r7,0000(r16)
	beqzc	r7,0040E490

l0040E476:
	seb	r7,r7
	addiu	r4,r0,0000DFFF
	addiu	r17,r17,00000004
	and	r7,r7,r4
	addiu	r16,r16,00000001
	addiu	r5,r5,FFFFFFFF
	sw	r7,-0004(r17)

l0040E48A:
	bnezc	r5,0040E472

l0040E48C:
	sw	r16,0000(r18)
	bc	0040E496

l0040E490:
	subu	r6,r6,r5
	sw	r0,0040(sp)
	sw	r0,0000(sp)

l0040E496:
	move	r4,r6
	restore.jrc	00000010,ra,00000004

l0040E49A:
	move	r7,r6
	beqzc	r17,0040E4C2

l0040E49E:
	beqzc	r7,0040E48C

l0040E4A0:
	lbu	r5,0000(r16)
	addiu	r5,r5,FFFFFFFF
	bgeiuc	r5,0000003F,0040E4AE

l0040E4A8:
	andi	r5,r16,00000003
	beqc	r0,r5,0040E596

l0040E4AE:
	lbu	r5,0000(r16)
	addiu	r4,r5,FFFFFFFF
	bgeiuc	r4,0000003F,0040E5B6

l0040E4B8:
	addiu	r16,r16,00000001
	sw	r5,0040(sp)

l0040E4BC:
	addiu	r7,r7,FFFFFFFF
	addiu	r17,r17,00000004
	bc	0040E49E

l0040E4C2:
	lbu	r5,0000(r16)
	addiu	r5,r5,FFFFFFFF
	bgeiuc	r5,0000003F,0040E4F4

l0040E4CA:
	andi	r5,r16,00000003
	bnezc	r5,0040E4F4

l0040E4CE:
	move	r5,r16

l0040E4D0:
	lw	r9,0000(r5)
	subu	r8,r7,r5
	addu	r8,r8,r16
	move	r4,r9
	addiu	r4,r4,FEFEFEFF
	or	r4,r4,r9
	li	r9,80808080
	and	r4,r4,r9
	beqzc	r4,0040E504

l0040E4F0:
	move	r16,r5
	move	r7,r8

l0040E4F4:
	lbu	r5,0000(r16)
	addiu	r4,r5,FFFFFFFF
	bgeiuc	r4,0000003F,0040E508

l0040E4FE:
	addiu	r16,r16,00000001

l0040E500:
	addiu	r7,r7,FFFFFFFF
	bc	0040E4C2

l0040E504:
	addiu	r5,r5,00000004
	bc	0040E4D0

l0040E508:
	addiu	r5,r5,FFFFFF3E
	bgeiuc	r5,00000033,0040E536

l0040E510:
	addiupc	r4,00004E5C
	addiu	r16,r16,00000001
	lwxs	r8,r5(r4)

l0040E51A:
	lbu	r5,0000(r16)
	sra	r4,r8,0000001A
	srl	r5,r5,00000003
	addu	r4,r4,r5
	addiu	r5,r5,FFFFFFF0
	or	r4,r4,r5
	ins	r4,r0,00000000,00000001
	beqzc	r4,0040E548

l0040E530:
	addiu	r16,r16,FFFFFFFF
	bnec	r0,r8,0040E558

l0040E536:
	lbu	r5,0000(r16)
	bnezc	r5,0040E558

l0040E53A:
	beqzc	r17,0040E540

l0040E53C:
	sw	r0,0040(sp)
	sw	r0,0000(sp)

l0040E540:
	subu	r6,r6,r7
	bc	0040E496

l0040E544:
	move	r7,r6
	bc	0040E51A

l0040E548:
	bbeqzc	r8,00000019,0040E4FE

l0040E54C:
	lbu	r5,0001(r16)
	addiu	r5,r5,FFFFFF80
	bltiuc	r5,00000000,0040E568

l0040E556:
	addiu	r16,r16,FFFFFFFF

l0040E558:
	balc	004049B0
	li	r7,00000054
	li	r6,FFFFFFFF
	sw	r7,0000(sp)
	bnec	r0,r17,0040E48C

l0040E566:
	bc	0040E496

l0040E568:
	bbnezc	r8,00000013,0040E570

l0040E56C:
	addiu	r16,r16,00000002
	bc	0040E500

l0040E570:
	lbu	r5,0002(r16)
	addiu	r5,r5,FFFFFF80
	bgeiuc	r5,00000000,0040E556

l0040E57A:
	addiu	r16,r16,00000003
	bc	0040E500

l0040E57E:
	sw	r8,0000(r17)
	addiu	r7,r7,FFFFFFFC
	lbu	r5,0001(r16)
	sw	r5,0044(sp)
	lbu	r5,0002(r16)
	addiu	r16,r16,00000004
	sw	r5,0048(sp)
	addiu	r17,r17,00000010
	lbu	r5,-0001(r16)
	sw	r5,-0004(r17)

l0040E596:
	lbu	r8,0000(r16)
	bltiuc	r7,00000005,0040E4AE

l0040E59E:
	lw	r4,0000(r16)
	move	r5,r4
	addiu	r5,r5,FEFEFEFF
	or	r5,r5,r4
	li	r4,80808080
	and	r5,r5,r4
	beqzc	r5,0040E57E

l0040E5B4:
	bc	0040E4AE

l0040E5B6:
	addiu	r5,r5,FFFFFF3E
	bgeiuc	r5,00000033,0040E536

l0040E5BE:
	addiupc	r4,00004DAE
	addiu	r16,r16,00000001
	lwxs	r8,r5(r4)
	bc	0040E426

l0040E5CA:
	lbu	r5,0001(r16)
	addiu	r5,r5,FFFFFF80
	bgeiuc	r5,00000000,0040E556

l0040E5D4:
	sll	r8,r8,00000006
	or	r8,r5,r8
	bltc	r8,r0,0040E5E4

l0040E5E0:
	addiu	r16,r16,00000002
	bc	0040E454

l0040E5E4:
	lbu	r5,0002(r16)
	addiu	r5,r5,FFFFFF80
	bgeiuc	r5,00000000,0040E556

l0040E5EE:
	sll	r8,r8,00000006
	addiu	r16,r16,00000003
	or	r8,r5,r8
	bc	0040E454
0040E5FA                               00 00 00 00 00 00           ......

;; wcrtomb: 0040E600
;;   Called from:
;;     0040CFF4 (in wctomb)
wcrtomb proc
	save	00000010,ra,00000001
	li	r7,00000001
	beqzc	r4,0040E612

l0040E606:
	addiu	r7,r0,00000080
	bgeuc	r5,r7,0040E616

l0040E60E:
	li	r7,00000001
	sb	r5,0000(r4)

l0040E612:
	move	r4,r7
	restore.jrc	00000010,ra,00000001

l0040E616:
	rdhwr	r3,0000001D,00000000
	lw	r6,-0038(r3)
	lw	r6,0000(r6)
	bnezc	r6,0040E63A

l0040E622:
	move	r6,r5
	addiu	r6,r6,FFFF2080
	bltuc	r6,r7,0040E60E

l0040E62E:
	balc	004049B0
	li	r7,00000054
	sw	r7,0000(sp)
	li	r7,FFFFFFFF
	bc	0040E612

l0040E63A:
	addiu	r7,r0,000007FF
	bltuc	r7,r5,0040E65E

l0040E642:
	sra	r7,r5,00000006
	addiu	r6,r0,FFFFFFC0
	or	r7,r7,r6
	andi	r5,r5,0000003F
	sb	r7,0000(r4)
	addiu	r7,r0,FFFFFF80
	or	r5,r5,r7
	li	r7,00000002
	sb	r5,0001(r4)
	bc	0040E612

l0040E65E:
	addiu	r7,r0,0000D7FF
	bgeuc	r7,r5,0040E676

l0040E666:
	move	r7,r5
	addiu	r6,r0,00001FFF
	addiu	r7,r7,FFFF2000
	bltuc	r6,r7,0040E69A

l0040E676:
	sra	r7,r5,0000000C
	addiu	r6,r0,FFFFFFE0
	or	r7,r7,r6
	addiu	r6,r0,FFFFFF80
	sb	r7,0000(r4)
	ext	r7,r5,00000006,00000006
	andi	r5,r5,0000003F
	or	r7,r7,r6
	or	r5,r5,r6
	sb	r7,0001(r4)
	sb	r5,0002(r4)
	li	r7,00000003
	bc	0040E612

l0040E69A:
	move	r7,r5
	li	r6,000FFFFF
	addiu	r7,r7,FFFF0000
	bltuc	r6,r7,0040E62E

l0040E6AC:
	sra	r7,r5,00000012
	addiu	r6,r0,FFFFFFF0
	or	r7,r7,r6
	ext	r6,r5,0000000C,00000006
	sb	r7,0000(r4)
	addiu	r7,r0,FFFFFF80
	or	r6,r6,r7
	sb	r6,0001(r4)
	ext	r6,r5,00000006,00000006
	andi	r5,r5,0000003F
	or	r6,r6,r7
	or	r5,r5,r7
	sb	r6,0002(r4)
	li	r7,00000004
	sb	r5,0003(r4)
	bc	0040E612
0040E6D8                         00 00 00 00 00 00 00 00         ........

;; close_file: 0040E6E0
;;   Called from:
;;     0040E72A (in __stdio_exit_needed)
;;     0040E738 (in __stdio_exit_needed)
;;     0040E746 (in __stdio_exit_needed)
close_file proc
	save	00000010,ra,00000002
	move	r16,r4
	beqzc	r4,0040E71E

l0040E6E6:
	lw	r7,004C(r4)
	bltc	r7,r0,0040E6F2

l0040E6EE:
	balc	0040D1D0

l0040E6F2:
	lw	r7,0014(r16)
	lw	r6,001C(r16)
	bgeuc	r6,r7,0040E702

l0040E6FA:
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r0,r0
	jalrc	ra,r7

l0040E702:
	lwm	r6,0004(r16),00000002
	bgeuc	r6,r7,0040E71E

l0040E70A:
	subu	r6,r6,r7
	lw	r5,0028(r16)
	move	r4,r16
	addiu	r8,r0,00000001
	sra	r7,r6,0000001F
	restore	00000010,ra,00000002
	jrc	r5

l0040E71E:
	restore.jrc	00000010,ra,00000002

;; __stdio_exit_needed: 0040E720
;;   Called from:
;;     00400166 (in exit)
;;     0040D3D2 (in __towrite_needs_stdio_exit)
;;     0040E79A (in __toread_needs_stdio_exit)
__stdio_exit_needed proc
	save	00000010,ra,00000002
	balc	00408610
	lw	r16,0000(r4)
	bc	0040E730

l0040E72A:
	move.balc	r4,r16,0040E6E0
	lw	r16,0038(r16)

l0040E730:
	bnezc	r16,0040E72A

l0040E732:
	lwpc	r4,00432F30
	balc	0040E6E0
	lwpc	r4,00430300
	restore	00000010,ra,00000002
	bc	0040E6E0
0040E74A                               00 00 00 00 00 00           ......

;; __toread: 0040E750
;;   Called from:
;;     0040D3E4 (in __uflow)
__toread proc
	save	00000010,ra,00000002
	lb	r7,004A(r4)
	move	r16,r4
	addiu	r6,r7,FFFFFFFF
	or	r7,r7,r6
	lw	r6,001C(r4)
	sb	r7,004A(r4)
	lw	r7,0014(r4)
	bgeuc	r6,r7,0040E770

l0040E76A:
	lw	r7,0024(r4)
	movep	r5,r6,r0,r0
	jalrc	ra,r7

l0040E770:
	lw	r4,0000(r16)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,001C(sp)
	bbnezc	r4,00000002,0040E790

l0040E77C:
	lw	r7,002C(r16)
	ext	r4,r4,00000004,00000001
	lw	r6,0030(r16)
	subu	r4,r0,r4
	addu	r7,r7,r6
	sw	r7,0004(sp)
	sw	r7,0008(sp)
	restore.jrc	00000010,ra,00000002

l0040E790:
	ori	r4,r4,00000020
	sw	r4,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000002

;; __toread_needs_stdio_exit: 0040E79A
__toread_needs_stdio_exit proc
	bc	0040E720
0040E79E                                           00 00               ..

;; sem_init: 0040E7A0
;;   Called from:
;;     0040DD4C (in handler)
;;     0040DD54 (in handler)
sem_init proc
	save	00000010,ra,00000001
	bltc	r6,r0,0040E7B6

l0040E7A6:
	sltiu	r5,r5,00000001
	sw	r6,0000(sp)
	sll	r5,r5,00000007
	sw	r0,0004(sp)
	sw	r5,0008(sp)
	move	r4,r0
	restore.jrc	00000010,ra,00000001

l0040E7B6:
	balc	004049B0
	li	r7,00000016
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000001
0040E7C2       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; sem_post: 0040E7D0
;;   Called from:
;;     0040DDD2 (in handler)
;;     0040DF8C (in __synccall)
;;     0040DF9E (in __synccall)
sem_post proc
	save	00000010,ra,00000002
	lw	r6,0008(r4)
	move	r16,r4

l0040E7D6:
	lw	r7,0000(r16)
	li	r5,7FFFFFFF
	lw	r9,0001(r16)
	bnec	r5,r7,0040E7EE

l0040E7E2:
	balc	004049B0
	li	r7,0000004B
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000002

l0040E7EE:
	addiu	r5,r7,00000001
	srl	r4,r7,0000001F
	addu	r5,r5,r4
	sync	00000000

l0040E7FC:
	ll	r8,0000(r16)
	bnec	r7,r8,0040E80C

l0040E804:
	move	r4,r5
	sc	r4,0000(r16)
	beqzc	r4,0040E7FC

l0040E80C:
	sync	00000000
	bnec	r7,r8,0040E7D6

l0040E814:
	bltc	r7,r0,0040E820

l0040E818:
	bnec	r0,r9,0040E820

l0040E81C:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

l0040E820:
	addiu	r7,r0,00000080
	movn	r6,r7,r6

l0040E828:
	li	r7,00000001
	or	r6,r6,r7
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040E81C

l0040E83A:
	li	r7,00000001
	li	r4,00000062
	movep	r5,r6,r16,r7
	balc	00404A50
	bc	0040E81C
0040E846                   00 00 00 00 00 00 00 00 00 00       ..........

;; sem_wait: 0040E850
;;   Called from:
;;     0040DDBE (in handler)
;;     0040DDD8 (in handler)
;;     0040DF92 (in __synccall)
sem_wait proc
	move	r5,r0
	bc	0040E8A6
0040E856                   00 00 00 00 00 00 00 00 00 00       ..........

;; lseek64: 0040E860
;;   Called from:
;;     0040E080 (in rewinddir)
lseek64 proc
	save	00000020,ra,00000001
	movep	r5,r9,r7,r8
	move	r7,r6
	addiu	r8,sp,00000008
	move	r6,r5
	move	r5,r4
	li	r4,0000003E
	balc	00404A50
	balc	0040CC30
	li	r6,FFFFFFFF
	li	r7,FFFFFFFF
	bnezc	r4,0040E882

l0040E87E:
	lwm	r6,0008(sp),00000002

l0040E882:
	movep	r4,r5,r6,r7
	restore.jrc	00000020,ra,00000001
0040E886                   00 00 00 00 00 00 00 00 00 00       ..........

;; cleanup: 0040E890
cleanup proc
	sync	00000000

l0040E894:
	ll	r7,0000(r4)
	addiu	r7,r7,FFFFFFFF
	sc	r7,0000(r4)
	beqzc	r7,0040E894

l0040E8A0:
	sync	00000000
	jrc	ra

;; sem_timedwait: 0040E8A6
;;   Called from:
;;     0040E852 (in sem_wait)
sem_timedwait proc
	save	00000020,ra,00000004
	movep	r16,r18,r4,r5
	balc	0040EA42
	move.balc	r4,r16,0040E930
	li	r7,00000065
	bnezc	r4,0040E8BE

l0040E8B6:
	move	r4,r0
	restore.jrc	00000020,ra,00000004

l0040E8BA:
	sync	00000000

l0040E8BE:
	addiu	r7,r7,FFFFFFFF
	beqzc	r7,0040E8CC

l0040E8C2:
	lw	r6,0000(r16)
	bltc	r0,r6,0040E8CC

l0040E8C8:
	lw	r6,0004(r16)
	beqzc	r6,0040E8BA

l0040E8CC:
	move.balc	r4,r16,0040E930
	beqzc	r4,0040E8B6

l0040E8D2:
	addiu	r6,r16,00000004
	sync	00000000

l0040E8D8:
	ll	r7,0000(r6)
	addiu	r7,r7,00000001
	sc	r7,0000(r6)
	beqzc	r7,0040E8D8

l0040E8E4:
	sync	00000000
	sync	00000000

l0040E8EC:
	ll	r7,0000(r16)
	bnezc	r7,0040E8FA

l0040E8F2:
	li	r7,FFFFFFFF
	sc	r7,0000(r16)
	beqzc	r7,0040E8EC

l0040E8FA:
	sync	00000000
	addiupc	r5,FFFFFF8E
	addiu	r4,sp,00000004
	balc	0040AE32
	lw	r8,0002(r16)
	movep	r6,r7,r0,r18
	li	r5,FFFFFFFF
	move.balc	r4,r16,0040E980
	li	r5,00000001
	move	r17,r4
	addiu	r4,sp,00000004
	balc	0040AE3A
	move	r7,r17
	ins	r7,r0,00000002,00000001
	beqzc	r7,0040E8CC

l0040E924:
	balc	004049B0
	sw	r17,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000020,ra,00000004
0040E92E                                           00 00               ..

;; sem_trywait: 0040E930
;;   Called from:
;;     0040E8AE (in sem_timedwait)
;;     0040E8CC (in sem_timedwait)
sem_trywait proc
	save	00000010,ra,00000001

l0040E932:
	lw	r7,0000(r4)
	bltc	r0,r7,0040E944

l0040E938:
	balc	004049B0
	li	r7,0000000B
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000001

l0040E944:
	addiu	r5,r7,FFFFFFFF
	move	r6,r0
	bneiuc	r7,00000001,0040E954

l0040E94E:
	lw	r6,0004(r4)
	sltu	r6,r0,r6

l0040E954:
	subu	r5,r5,r6
	sync	00000000

l0040E95A:
	ll	r8,0000(r4)
	bnec	r7,r8,0040E96A

l0040E962:
	move	r6,r5
	sc	r6,0000(r4)
	beqzc	r6,0040E95A

l0040E96A:
	sync	00000000
	bnec	r7,r8,0040E932

l0040E972:
	move	r4,r0
	restore.jrc	00000010,ra,00000001
0040E976                   00 00 00 00 00 00 00 00 00 00       ..........

;; __timedwait_cp: 0040E980
;;   Called from:
;;     0040E90E (in sem_timedwait)
;;     0040EA2E (in __timedwait)
__timedwait_cp proc
	save	00000030,ra,00000005
	addiu	r19,r0,00000080
	move	r16,r7
	movep	r17,r18,r4,r5
	movz	r19,r0,r8

l0040E98E:
	beqzc	r7,0040E9D4

l0040E990:
	lw	r7,0004(r7)
	li	r5,3B9AC9FF
	bgeuc	r5,r7,0040E9A0

l0040E99C:
	li	r4,00000016
	restore.jrc	00000030,ra,00000005

l0040E9A0:
	addiu	r5,sp,00000008
	move.balc	r4,r6,0040AEF4
	bnezc	r4,0040E99C

l0040E9A8:
	lw	r17,0008(sp)
	lw	r5,0000(r16)
	lw	r17,000C(sp)
	subu	r5,r5,r7
	lw	r7,0004(r16)
	sw	r5,0008(sp)
	subu	r7,r7,r6
	sw	r7,000C(sp)
	bgec	r7,r0,0040E9C8

l0040E9BC:
	addiu	r5,r5,FFFFFFFF
	addiu	r7,r7,3B9ACA00
	sw	r5,0008(sp)
	sw	r7,000C(sp)

l0040E9C8:
	lw	r17,0008(sp)
	addiu	r16,sp,00000008
	bgec	r7,r0,0040E9D6

l0040E9D0:
	li	r4,0000006E
	restore.jrc	00000030,ra,00000005

l0040E9D4:
	move	r16,r0

l0040E9D6:
	movep	r7,r8,r18,r16
	movep	r5,r6,r17,r19
	move	r10,r0
	move	r9,r0
	li	r4,00000062
	balc	0040ADA4
	addiu	r6,r0,FFFFFFDA
	move	r7,r4
	subu	r4,r0,r4
	bnec	r6,r7,0040EA02

l0040E9F0:
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r18,r16
	movep	r5,r6,r17,r0
	li	r4,00000062
	balc	0040ADA4
	subu	r4,r0,r4

l0040EA02:
	beqic	r4,00000004,0040EA12

l0040EA06:
	beqic	r4,0000002E,0040E9D0

l0040EA0A:
	xori	r7,r4,0000007D
	movn	r4,r0,r7

l0040EA12:
	restore.jrc	00000030,ra,00000005

;; __timedwait: 0040EA14
__timedwait proc
	save	00000030,ra,00000003
	movep	r16,r17,r4,r5
	addiu	r5,sp,0000001C
	li	r4,00000001
	sw	r8,0004(sp)
	sw	r7,0008(sp)
	sw	r6,000C(sp)
	balc	0040AE50
	lw	r18,0004(sp)
	lw	r17,0008(sp)
	lw	r17,000C(sp)
	movep	r4,r5,r16,r17
	balc	0040E980
	move	r5,r0
	move	r16,r4
	lw	r17,001C(sp)
	balc	0040AE50
	move	r4,r16
	restore.jrc	00000030,ra,00000003

;; __testcancel: 0040EA40
;;   Called from:
;;     0040EA42 (in __pthread_testcancel)
__testcancel proc
	jrc	ra

;; __pthread_testcancel: 0040EA42
;;   Called from:
;;     0040E8AA (in sem_timedwait)
__pthread_testcancel proc
	bc	0040EA40
0040EA46                   00 00 00 00 00 00 00 00 00 00       ..........

;; __ashldi3: 0040EA50
;;   Called from:
;;     004095B2 (in fn00409170)
;;     0040E1CC (in fmod)
;;     0040E1F4 (in fmod)
__ashldi3 proc
	subu	r7,r0,r6
	sllv	r5,r5,r6
	srlv	r7,r4,r7
	sllv	r4,r4,r6
	movz	r7,r0,r6

l0040EA64:
	slti	r6,r6,00000020
	or	r5,r5,r7
	movz	r5,r4,r6

l0040EA6E:
	movz	r4,r0,r6

l0040EA72:
	jrc	ra
0040EA74             00 80 00 C0 00 80 00 C0 00 80 00 C0     ............

;; __lshrdi3: 0040EA80
;;   Called from:
;;     0040E2B4 (in fmod)
__lshrdi3 proc
	subu	r7,r0,r6
	srlv	r4,r4,r6
	sllv	r7,r5,r7
	srlv	r5,r5,r6
	movz	r7,r0,r6

l0040EA94:
	slti	r6,r6,00000020
	or	r4,r4,r7
	movz	r4,r5,r6

l0040EA9E:
	movz	r5,r0,r6

l0040EAA2:
	jrc	ra
0040EAA4             00 80 00 C0 00 80 00 C0 00 80 00 C0     ............

;; __udivdi3: 0040EAB0
;;   Called from:
;;     00404822 (in sysconf)
;;     00408900 (in fmt_u)
;;     004095DE (in fn00409170)
;;     0040B774 (in decfloat)
;;     0040C8B0 (in __intscan)
__udivdi3 proc
	move	r8,r5
	move	r10,r4
	movep	r9,r5,r6,r7
	move	r11,r8
	bnec	r0,r5,0040EC5A

l0040EABC:
	bgeuc	r8,r9,0040EB4C

l0040EAC0:
	clz	r7,r6
	beqzc	r7,0040EADE

l0040EAC6:
	subu	r11,r0,r7
	sllv	r8,r8,r7
	srlv	r11,r4,r11
	sllv	r9,r6,r7
	or	r11,r11,r8
	sllv	r10,r4,r7

l0040EADE:
	ext	r8,r9,00000000,00000010
	srl	r2,r9,00000010
	divu	r3,r11,r2
	mul	r4,r8,r3
	srl	r7,r10,00000010
	modu	r6,r11,r2
	sll	r6,r6,00000010
	or	r6,r6,r7
	move	r7,r3
	bgeuc	r6,r4,0040EB14

l0040EB02:
	addu	r6,r6,r9
	addiu	r7,r7,FFFFFFFF
	bltuc	r6,r9,0040EB14

l0040EB0A:
	bgeuc	r6,r4,0040EB14

l0040EB0E:
	addiu	r7,r3,FFFFFFFE
	addu	r6,r6,r9

l0040EB14:
	subu	r6,r6,r4
	divu	r11,r6,r2
	mul	r8,r8,r11
	modu	r4,r6,r2
	ext	r10,r10,00000000,00000010
	sll	r6,r4,00000010
	or	r10,r6,r10
	move	r4,r11
	bgeuc	r10,r8,0040EB42

l0040EB32:
	addu	r10,r10,r9
	addiu	r4,r4,FFFFFFFF
	bltuc	r10,r9,0040EB42

l0040EB3A:
	bgeuc	r10,r8,0040EB42

l0040EB3E:
	addiu	r4,r11,FFFFFFFE

l0040EB42:
	sll	r7,r7,00000010
	or	r7,r7,r4

l0040EB48:
	move	r4,r7
	jrc	ra

l0040EB4C:
	bnec	r0,r9,0040EB56

l0040EB50:
	li	r7,00000001
	divu	r9,r7,r6

l0040EB56:
	clz	r6,r9
	bnezc	r6,0040EBCA

l0040EB5C:
	subu	r8,r8,r9
	li	r5,00000001

l0040EB62:
	ext	r11,r9,00000000,00000010
	srl	r2,r9,00000010
	divu	r3,r8,r2
	mul	r4,r11,r3
	srl	r7,r10,00000010
	modu	r6,r8,r2
	sll	r6,r6,00000010
	or	r6,r6,r7
	move	r7,r3
	bgeuc	r6,r4,0040EB98

l0040EB86:
	addu	r6,r6,r9
	addiu	r7,r7,FFFFFFFF
	bltuc	r6,r9,0040EB98

l0040EB8E:
	bgeuc	r6,r4,0040EB98

l0040EB92:
	addiu	r7,r3,FFFFFFFE
	addu	r6,r6,r9

l0040EB98:
	subu	r6,r6,r4
	divu	r3,r6,r2
	mul	r8,r11,r3
	modu	r4,r6,r2
	ext	r10,r10,00000000,00000010
	sll	r6,r4,00000010
	or	r10,r6,r10
	move	r4,r3
	bgeuc	r10,r8,0040EB42

l0040EBB8:
	addu	r10,r10,r9
	addiu	r4,r4,FFFFFFFF
	bltuc	r10,r9,0040EB42

l0040EBC0:
	bgeuc	r10,r8,0040EB42

l0040EBC4:
	addiu	r4,r3,FFFFFFFE
	bc	0040EB42

l0040EBCA:
	sllv	r9,r9,r6
	li	r7,00000020
	subu	r7,r7,r6
	ext	r5,r9,00000000,00000010
	srlv	r3,r8,r7
	sllv	r10,r4,r6
	sllv	r8,r8,r6
	srl	r11,r9,00000010
	divu	r2,r3,r11
	mul	r6,r5,r2
	srlv	r7,r4,r7
	or	r8,r7,r8
	modu	r7,r3,r11
	srl	r4,r8,00000010
	sll	r7,r7,00000010
	or	r7,r7,r4
	move	r4,r2
	bgeuc	r7,r6,0040EC1C

l0040EC0A:
	addu	r7,r7,r9
	addiu	r4,r4,FFFFFFFF
	bltuc	r7,r9,0040EC1C

l0040EC12:
	bgeuc	r7,r6,0040EC1C

l0040EC16:
	addiu	r4,r2,FFFFFFFE
	addu	r7,r7,r9

l0040EC1C:
	subu	r6,r7,r6
	modu	r7,r6,r11
	divu	r2,r6,r11
	mul	r6,r5,r2
	sll	r7,r7,00000010
	ext	r8,r8,00000000,00000010
	or	r8,r7,r8
	move	r7,r2
	bgeuc	r8,r6,0040EC4E

l0040EC3C:
	addu	r8,r8,r9
	addiu	r7,r7,FFFFFFFF
	bltuc	r8,r9,0040EC4E

l0040EC44:
	bgeuc	r8,r6,0040EC4E

l0040EC48:
	addiu	r7,r2,FFFFFFFE
	addu	r8,r8,r9

l0040EC4E:
	sll	r5,r4,00000010
	subu	r8,r8,r6
	or	r5,r5,r7
	bc	0040EB62

l0040EC5A:
	bltuc	r8,r5,0040ED3A

l0040EC5E:
	clz	r3,r7
	bnec	r0,r3,0040EC76

l0040EC66:
	bltuc	r5,r8,0040ED40

l0040EC6A:
	sltu	r6,r4,r6
	xori	r7,r6,00000001

l0040EC72:
	move	r5,r0
	bc	0040EB48

l0040EC76:
	li	r5,00000020
	sllv	r7,r7,r3
	subu	r5,r5,r3
	srlv	r2,r6,r5
	srlv	r13,r8,r5
	or	r2,r2,r7
	sllv	r8,r8,r3
	srlv	r5,r4,r5
	ext	r9,r2,00000000,00000010
	or	r11,r5,r8
	srl	r7,r2,00000010
	divu	r12,r13,r7
	mul	r5,r9,r12
	srl	r8,r11,00000010
	modu	r10,r13,r7
	sll	r10,r10,00000010
	sllv	r6,r6,r3
	or	r10,r10,r8
	move	r8,r12
	bgeuc	r10,r5,0040ECD8

l0040ECC2:
	addu	r10,r10,r2
	addiu	r8,r8,FFFFFFFF
	bltuc	r10,r2,0040ECD8

l0040ECCC:
	bgeuc	r10,r5,0040ECD8

l0040ECD0:
	addiu	r8,r12,FFFFFFFE
	addu	r10,r10,r2

l0040ECD8:
	subu	r10,r10,r5
	divu	r12,r10,r7
	mul	r9,r9,r12
	modu	r5,r10,r7
	ext	r11,r11,00000000,00000010
	sll	r5,r5,00000010
	or	r5,r5,r11
	move	r7,r12
	bgeuc	r5,r9,0040ED10

l0040ECFA:
	addu	r5,r5,r2
	addiu	r7,r7,FFFFFFFF
	bltuc	r5,r2,0040ED10

l0040ED04:
	bgeuc	r5,r9,0040ED10

l0040ED08:
	addiu	r7,r12,FFFFFFFE
	addu	r5,r5,r2

l0040ED10:
	sll	r8,r8,00000010
	subu	r9,r5,r9
	or	r7,r8,r7
	mul	r8,r7,r6
	muhu	r6,r7,r6
	bltuc	r9,r6,0040ED36

l0040ED28:
	move	r5,r0
	bnec	r9,r6,0040EB48

l0040ED2E:
	sllv	r6,r4,r3
	bgeuc	r6,r8,0040EB48

l0040ED36:
	addiu	r7,r7,FFFFFFFF
	bc	0040EC72

l0040ED3A:
	move	r5,r0
	move	r7,r0
	bc	0040EB48

l0040ED40:
	move	r5,r0
	li	r7,00000001
	bc	0040EB48
0040ED46                   00 00 00 00 00 00 00 00 00 00       ..........

;; __umoddi3: 0040ED50
;;   Called from:
;;     004088EE (in fmt_u)
;;     004095CC (in fn00409170)
;;     0040B784 (in decfloat)
__umoddi3 proc
	movep	r9,r11,r6,r7
	movep	r8,r10,r4,r5
	bnec	r0,r11,0040EEA8

l0040ED58:
	bgeuc	r10,r9,0040EDDE

l0040ED5C:
	clz	r11,r6
	beqc	r0,r11,0040ED7C

l0040ED64:
	subu	r10,r0,r11
	sllv	r5,r5,r11
	srlv	r10,r4,r10
	sllv	r9,r6,r11
	or	r10,r10,r5
	sllv	r8,r4,r11

l0040ED7C:
	ext	r4,r9,00000000,00000010
	srl	r5,r9,00000010
	divu	r6,r10,r5
	mul	r6,r6,r4
	modu	r7,r10,r5
	srl	r10,r8,00000010
	sll	r7,r7,00000010
	or	r7,r7,r10
	bgeuc	r7,r6,0040EDAA

l0040ED9E:
	addu	r7,r7,r9
	bltuc	r7,r9,0040EDAA

l0040EDA4:
	bgeuc	r7,r6,0040EDAA

l0040EDA8:
	addu	r7,r7,r9

l0040EDAA:
	subu	r7,r7,r6
	divu	r10,r7,r5
	mul	r4,r4,r10
	modu	r6,r7,r5

l0040EDB6:
	sll	r7,r6,00000010
	ext	r8,r8,00000000,00000010
	or	r8,r7,r8
	bgeuc	r8,r4,0040EDD2

l0040EDC6:
	addu	r8,r8,r9
	bltuc	r8,r9,0040EDD2

l0040EDCC:
	bgeuc	r8,r4,0040EDD2

l0040EDD0:
	addu	r8,r8,r9

l0040EDD2:
	subu	r8,r8,r4
	move	r5,r0
	srlv	r4,r8,r11

l0040EDDC:
	jrc	ra

l0040EDDE:
	bnec	r0,r9,0040EDE8

l0040EDE2:
	li	r7,00000001
	divu	r9,r7,r6

l0040EDE8:
	clz	r11,r9
	bnec	r0,r11,0040EE2E

l0040EDF0:
	subu	r5,r5,r9

l0040EDF4:
	ext	r4,r9,00000000,00000010
	srl	r10,r9,00000010
	divu	r6,r5,r10
	mul	r6,r6,r4
	modu	r7,r5,r10
	srl	r5,r8,00000010
	sll	r7,r7,00000010
	or	r7,r7,r5
	bgeuc	r7,r6,0040EE20

l0040EE14:
	addu	r7,r7,r9
	bltuc	r7,r9,0040EE20

l0040EE1A:
	bgeuc	r7,r6,0040EE20

l0040EE1E:
	addu	r7,r7,r9

l0040EE20:
	subu	r7,r7,r6
	divu	r5,r7,r10
	modu	r6,r7,r10
	mul	r4,r4,r5
	bc	0040EDB6

l0040EE2E:
	addiu	r2,r0,00000020
	sllv	r9,r9,r11
	subu	r2,r2,r11
	sllv	r8,r4,r11
	srlv	r3,r5,r2
	sllv	r5,r5,r11
	srlv	r2,r4,r2
	srl	r4,r9,00000010
	or	r2,r2,r5
	ext	r5,r9,00000000,00000010
	divu	r6,r3,r4
	mul	r6,r6,r5
	modu	r7,r3,r4
	srl	r10,r2,00000010
	sll	r7,r7,00000010
	or	r7,r7,r10
	bgeuc	r7,r6,0040EE7C

l0040EE70:
	addu	r7,r7,r9
	bltuc	r7,r9,0040EE7C

l0040EE76:
	bgeuc	r7,r6,0040EE7C

l0040EE7A:
	addu	r7,r7,r9

l0040EE7C:
	subu	r6,r7,r6
	divu	r10,r6,r4
	mul	r10,r10,r5
	modu	r7,r6,r4
	ext	r5,r2,00000000,00000010
	sll	r7,r7,00000010
	or	r5,r5,r7
	bgeuc	r5,r10,0040EEA2

l0040EE96:
	addu	r5,r5,r9
	bltuc	r5,r9,0040EEA2

l0040EE9C:
	bgeuc	r5,r10,0040EEA2

l0040EEA0:
	addu	r5,r5,r9

l0040EEA2:
	subu	r5,r5,r10
	bc	0040EDF4

l0040EEA8:
	bltuc	r5,r11,0040EDDC

l0040EEAC:
	clz	r12,r7
	bnec	r0,r12,0040EECE

l0040EEB4:
	bltuc	r11,r10,0040EEBC

l0040EEB8:
	bltuc	r8,r9,0040EECA

l0040EEBC:
	subu	r8,r4,r6
	subu	r5,r5,r7
	sltu	r10,r4,r8
	subu	r10,r5,r10

l0040EECA:
	movep	r4,r5,r8,r10
	bc	0040EDDC

l0040EECE:
	addiu	r9,r0,00000020
	sllv	r7,r7,r12
	subu	r9,r9,r12
	srlv	r11,r6,r9
	srlv	r10,r4,r9
	or	r8,r11,r7
	srlv	r11,r5,r9
	sllv	r5,r5,r12
	srl	r7,r8,00000010
	or	r5,r10,r5
	ext	r10,r8,00000000,00000010
	modu	r3,r11,r7
	divu	r13,r11,r7
	mul	r11,r10,r13
	srl	r2,r5,00000010
	sll	r3,r3,00000010
	sllv	r6,r6,r12
	or	r3,r3,r2
	sllv	r4,r4,r12
	move	r2,r13
	bgeuc	r3,r11,0040EF36

l0040EF20:
	addu	r3,r3,r8
	addiu	r2,r2,FFFFFFFF
	bltuc	r3,r8,0040EF36

l0040EF2A:
	bgeuc	r3,r11,0040EF36

l0040EF2E:
	addiu	r2,r13,FFFFFFFE
	addu	r3,r3,r8

l0040EF36:
	subu	r3,r3,r11
	modu	r11,r3,r7
	divu	r13,r3,r7
	mul	r7,r10,r13
	andi	r5,r5,0000FFFF
	sll	r11,r11,00000010
	or	r10,r11,r5
	move	r5,r13
	bgeuc	r10,r7,0040EF68

l0040EF56:
	addu	r10,r10,r8
	addiu	r5,r5,FFFFFFFF
	bltuc	r10,r8,0040EF68

l0040EF5E:
	bgeuc	r10,r7,0040EF68

l0040EF62:
	addiu	r5,r13,FFFFFFFE
	addu	r10,r10,r8

l0040EF68:
	sll	r11,r2,00000010
	subu	r10,r10,r7
	or	r11,r11,r5
	mul	r2,r11,r6
	muhu	r11,r11,r6
	move	r7,r2
	move	r5,r11
	bltuc	r10,r11,0040EF8C

l0040EF84:
	bnec	r10,r11,0040EF9C

l0040EF88:
	bgeuc	r4,r2,0040EF9C

l0040EF8C:
	subu	r7,r2,r6
	subu	r11,r11,r8
	sltu	r2,r2,r7
	subu	r5,r11,r2

l0040EF9C:
	subu	r7,r4,r7
	subu	r10,r10,r5
	sltu	r4,r4,r7
	subu	r10,r10,r4
	srlv	r4,r7,r12
	sllv	r8,r10,r9
	srlv	r5,r10,r12
	or	r4,r8,r4
	bc	0040EDDC
0040EFBC                                     00 00 00 00             ....

;; __adddf3: 0040EFC0
;;   Called from:
;;     004091DA (in fn00409170)
;;     004093E2 (in fn00409170)
;;     004096BC (in fn00409170)
;;     0040B99A (in decfloat)
;;     0040B9D4 (in decfloat)
;;     0040BA48 (in decfloat)
;;     0040BA78 (in decfloat)
;;     0040BA88 (in decfloat)
;;     0040BB86 (in decfloat)
;;     0040C1D2 (in __floatscan)
;;     0040C21A (in __floatscan)
;;     0040C36C (in __floatscan)
;;     0040C3E8 (in __floatscan)
;;     0040C3EE (in __floatscan)
__adddf3 proc
	ext	r10,r5,00000000,00000014
	ext	r9,r7,00000000,00000014
	sll	r10,r10,00000003
	srl	r8,r4,0000001D
	ext	r13,r5,00000004,0000000B
	or	r8,r8,r10
	srl	r3,r7,0000001F
	ext	r10,r7,00000004,0000000B
	srl	r5,r5,0000001F
	sll	r7,r9,00000003
	srl	r9,r6,0000001D
	sll	r12,r4,00000003
	or	r9,r9,r7
	sll	r6,r6,00000003
	subu	r11,r13,r10
	addiu	r2,r0,000007FF
	bnec	r5,r3,0040F2C4

l0040F002:
	move	r3,r11
	bgec	r0,r11,0040F152

l0040F008:
	bnec	r0,r10,0040F0CE

l0040F00C:
	or	r7,r9,r6
	bnezc	r7,0040F074

l0040F012:
	move	r10,r11
	bnec	r11,r2,0040F23A

l0040F018:
	or	r7,r8,r12
	bnec	r0,r7,0040F23A

l0040F020:
	move	r8,r0
	move	r12,r0

l0040F024:
	bbeqzc	r8,00000017,0040F03A

l0040F028:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	ins	r8,r0,00000007,00000001
	bnec	r10,r7,0040F03A

l0040F036:
	move	r8,r0
	move	r12,r0

l0040F03A:
	sll	r6,r8,0000001D
	srl	r7,r12,00000003
	or	r7,r7,r6
	addiu	r6,r0,000007FF
	srl	r8,r8,00000003
	bnec	r10,r6,0040F060

l0040F050:
	or	r6,r7,r8
	beqc	r0,r6,0040F5D2

l0040F058:
	lui	r6,00000080
	or	r8,r8,r6

l0040F060:
	move	r6,r0
	ins	r6,r8,00000000,00000001
	ins	r6,r10,00000004,00000001
	ins	r6,r5,0000000F,00000001
	movep	r8,r9,r6,r7
	movep	r4,r5,r9,r8
	jrc	ra

l0040F074:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,0040F0BA

l0040F07A:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	addiu	r10,r0,00000001
	addu	r8,r8,r7
	move	r12,r6

l0040F08C:
	bbeqzc	r8,00000017,0040F23A

l0040F090:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	beqc	r10,r7,0040F020

l0040F09A:
	move	r6,r8
	srl	r2,r12,00000001
	ins	r6,r0,00000007,00000001
	andi	r12,r12,00000001
	sll	r8,r6,0000001F
	or	r12,r2,r12
	or	r12,r8,r12
	srl	r8,r6,00000001
	bc	0040F23A

l0040F0BA:
	bnec	r11,r2,0040F0EC

l0040F0BE:
	or	r7,r8,r12
	bnec	r0,r7,0040F292

l0040F0C6:
	move	r8,r0
	move	r12,r0
	move	r10,r11
	bc	0040F024

l0040F0CE:
	bnec	r13,r2,0040F0E2

l0040F0D2:
	or	r7,r8,r12
	bnec	r0,r7,0040F292

l0040F0DA:
	move	r8,r0
	move	r12,r0
	move	r10,r13
	bc	0040F024

l0040F0E2:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11

l0040F0EC:
	bgeic	r7,00000039,0040F148

l0040F0F0:
	bgeic	r7,00000020,0040F128

l0040F0F4:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040F116:
	addu	r6,r6,r12
	addu	r7,r7,r8
	sltu	r8,r6,r12
	move	r10,r13
	addu	r8,r8,r7
	move	r12,r6
	bc	0040F08C

l0040F128:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0040F13A

l0040F132:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0040F13A:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040F144:
	move	r7,r0
	bc	0040F116

l0040F148:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	0040F144

l0040F152:
	beqc	r0,r11,0040F200

l0040F156:
	bnec	r0,r13,0040F1C2

l0040F15A:
	or	r7,r8,r12
	bnezc	r7,0040F174

l0040F160:
	bnec	r10,r2,0040F16E

l0040F164:
	or	r12,r9,r6
	move	r8,r0
	beqc	r0,r12,0040F024

l0040F16E:
	move	r8,r9
	move	r12,r6
	bc	0040F23A

l0040F174:
	nor	r11,r0,r11
	bnec	r0,r11,0040F18A

l0040F17C:
	addu	r12,r12,r6
	addu	r8,r8,r9

l0040F182:
	sltu	r6,r12,r6
	addu	r8,r8,r6
	bc	0040F08C

l0040F18A:
	beqc	r10,r2,0040F164

l0040F18E:
	bgeic	r11,00000039,0040F1F6

l0040F192:
	bgeic	r11,00000020,0040F1D4

l0040F196:
	li	r7,00000020
	srlv	r2,r12,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r4,r8,r7
	sllv	r12,r12,r7
	or	r4,r4,r2
	sltu	r12,r0,r12
	or	r12,r4,r12

l0040F1B8:
	addu	r12,r12,r6
	addu	r8,r11,r9
	bc	0040F182

l0040F1C2:
	beqc	r10,r2,0040F164

l0040F1C6:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	0040F18E

l0040F1D4:
	srlv	r4,r8,r11
	move	r7,r0
	beqic	r11,00000020,0040F1E6

l0040F1DE:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l0040F1E6:
	or	r12,r7,r12
	sltu	r12,r0,r12
	or	r12,r4,r12

l0040F1F2:
	move	r11,r0
	bc	0040F1B8

l0040F1F6:
	or	r12,r8,r12
	sltu	r12,r0,r12
	bc	0040F1F2

l0040F200:
	addiu	r10,r13,00000001
	andi	r7,r10,000007FF
	bgeic	r7,00000002,0040F2A2

l0040F20C:
	or	r7,r8,r12
	bnec	r0,r13,0040F258

l0040F214:
	beqc	r0,r7,0040F5AC

l0040F218:
	or	r7,r9,r6
	move	r10,r0
	beqzc	r7,0040F23A

l0040F220:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	move	r12,r6
	addu	r8,r8,r7
	bbeqzc	r8,00000017,0040F23A

l0040F232:
	ins	r8,r0,00000007,00000001
	addiu	r10,r0,00000001

l0040F23A:
	andi	r7,r12,00000007
	beqc	r0,r7,0040F024

l0040F242:
	andi	r7,r12,0000000F
	beqic	r7,00000004,0040F024

l0040F24A:
	addiu	r6,r12,00000004
	sltu	r7,r6,r12
	move	r12,r6
	addu	r8,r8,r7
	bc	0040F024

l0040F258:
	beqc	r0,r7,0040F5B2

l0040F25C:
	or	r6,r9,r6
	move	r10,r2
	beqzc	r6,0040F23A

l0040F264:
	srl	r10,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r10
	bbnezc	r9,00000013,0040F298

l0040F274:
	ext	r4,r4,00000000,0000001D
	sll	r7,r8,0000001D
	or	r7,r7,r4
	move	r3,r5

l0040F280:
	sll	r10,r10,00000003
	srl	r8,r7,0000001D
	or	r8,r8,r10
	sll	r12,r7,00000003

l0040F290:
	move	r5,r3

l0040F292:
	addiu	r10,r0,000007FF
	bc	0040F23A

l0040F298:
	li	r10,000FFFFF
	li	r7,FFFFFFFF
	bc	0040F280

l0040F2A2:
	beqc	r10,r2,0040F020

l0040F2A6:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	srl	r6,r6,00000001
	addu	r2,r8,r7
	sll	r8,r2,0000001F
	or	r12,r8,r6
	srl	r8,r2,00000001
	bc	0040F23A

l0040F2C4:
	move	r14,r11
	bgec	r0,r11,0040F37E

l0040F2CA:
	bnec	r0,r10,0040F344

l0040F2CE:
	or	r7,r9,r6
	beqc	r0,r7,0040F012

l0040F2D6:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,0040F300

l0040F2DC:
	subu	r6,r12,r6
	subu	r8,r8,r9
	sltu	r7,r12,r6
	addiu	r10,r0,00000001
	subu	r8,r8,r7
	move	r12,r6

l0040F2F2:
	bbeqzc	r8,00000017,0040F23A

l0040F2F6:
	ext	r4,r8,00000000,00000017
	move	r11,r12
	move	r13,r10
	bc	0040F508

l0040F300:
	beqc	r11,r2,0040F0BE

l0040F304:
	bgeic	r7,00000039,0040F374

l0040F308:
	bgeic	r7,00000020,0040F354

l0040F30C:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040F32E:
	subu	r6,r12,r6
	subu	r7,r8,r7
	sltu	r8,r12,r6
	move	r10,r13
	subu	r8,r7,r8
	move	r12,r6
	bc	0040F2F2

l0040F344:
	beqc	r13,r2,0040F0D2

l0040F348:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11
	bc	0040F304

l0040F354:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0040F366

l0040F35E:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0040F366:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040F370:
	move	r7,r0
	bc	0040F32E

l0040F374:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	0040F370

l0040F37E:
	beqc	r0,r11,0040F42E

l0040F382:
	bnec	r0,r13,0040F3F0

l0040F386:
	or	r7,r8,r12
	bnezc	r7,0040F39E

l0040F38C:
	bnec	r10,r2,0040F398

l0040F390:
	or	r12,r9,r6
	beqc	r0,r12,0040F5B8

l0040F398:
	move	r8,r9
	move	r12,r6
	bc	0040F482

l0040F39E:
	nor	r11,r0,r11
	bnec	r0,r11,0040F3BA

l0040F3A6:
	subu	r12,r6,r12
	subu	r8,r9,r8

l0040F3AE:
	sltu	r6,r6,r12
	move	r5,r3
	subu	r8,r8,r6
	bc	0040F2F2

l0040F3BA:
	beqc	r10,r2,0040F390

l0040F3BE:
	bgeic	r11,00000039,0040F424

l0040F3C2:
	bgeic	r11,00000020,0040F402

l0040F3C6:
	li	r7,00000020
	srlv	r4,r12,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r5,r8,r7
	sllv	r12,r12,r7
	or	r5,r5,r4
	sltu	r12,r0,r12
	or	r12,r5,r12

l0040F3E6:
	subu	r12,r6,r12
	subu	r8,r9,r11
	bc	0040F3AE

l0040F3F0:
	beqc	r10,r2,0040F390

l0040F3F4:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	0040F3BE

l0040F402:
	srlv	r5,r8,r11
	move	r7,r0
	beqic	r11,00000020,0040F414

l0040F40C:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l0040F414:
	or	r12,r7,r12
	sltu	r12,r0,r12
	or	r12,r5,r12

l0040F420:
	move	r11,r0
	bc	0040F3E6

l0040F424:
	or	r12,r8,r12
	sltu	r12,r0,r12
	bc	0040F420

l0040F42E:
	addiu	r7,r13,00000001
	andi	r7,r7,000007FF
	bgeic	r7,00000002,0040F4E4

l0040F43A:
	or	r10,r8,r12
	or	r7,r9,r6
	bnec	r0,r13,0040F496

l0040F446:
	bnec	r0,r10,0040F45A

l0040F44A:
	bnec	r0,r7,0040F398

l0040F44E:
	move	r8,r0
	move	r12,r0

l0040F452:
	move	r10,r0
	move	r5,r0
	bc	0040F024

l0040F45A:
	beqzc	r7,0040F492

l0040F45C:
	subu	r4,r12,r6
	subu	r7,r8,r9
	sltu	r10,r12,r4
	subu	r7,r7,r10
	bbeqzc	r7,00000017,0040F486

l0040F470:
	subu	r12,r6,r12
	subu	r8,r9,r8
	sltu	r6,r6,r12
	move	r10,r0
	subu	r8,r8,r6

l0040F482:
	move	r5,r3
	bc	0040F23A

l0040F486:
	or	r12,r4,r7
	beqc	r0,r12,0040F56C

l0040F48E:
	move	r8,r7
	move	r12,r4

l0040F492:
	move	r10,r0
	bc	0040F23A

l0040F496:
	bnec	r0,r10,0040F4A4

l0040F49A:
	beqc	r0,r7,0040F5C0

l0040F49E:
	move	r8,r9
	move	r12,r6
	bc	0040F290

l0040F4A4:
	move	r10,r2
	beqc	r0,r7,0040F23A

l0040F4AA:
	srl	r7,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r7
	bbnezc	r9,00000013,0040F4DA

l0040F4BA:
	ext	r4,r4,00000000,0000001D
	sll	r12,r8,0000001D
	or	r4,r12,r4
	move	r14,r5

l0040F4C8:
	sll	r7,r7,00000003
	srl	r8,r4,0000001D
	or	r8,r8,r7
	sll	r12,r4,00000003
	move	r5,r14
	bc	0040F292

l0040F4DA:
	li	r7,000FFFFF
	li	r4,FFFFFFFF
	bc	0040F4C8

l0040F4E4:
	subu	r11,r12,r6
	subu	r4,r8,r9
	sltu	r7,r12,r11
	subu	r4,r4,r7
	bbeqzc	r4,00000017,0040F564

l0040F4F6:
	subu	r11,r6,r12
	subu	r8,r9,r8
	sltu	r6,r6,r11
	move	r5,r3
	subu	r4,r8,r6

l0040F508:
	clz	r7,r4
	bnezc	r4,0040F516

l0040F50E:
	clz	r7,r11
	addiu	r7,r7,00000020

l0040F516:
	addiu	r10,r7,FFFFFFF8
	bgeic	r10,00000020,0040F570

l0040F51E:
	subu	r8,r0,r10
	sllv	r4,r4,r10
	srlv	r8,r11,r8
	sllv	r12,r11,r10
	or	r8,r8,r4

l0040F532:
	bltc	r10,r13,0040F5A2

l0040F536:
	subu	r10,r10,r13
	addiu	r6,r10,00000001
	bgeic	r6,00000020,0040F57C

l0040F542:
	li	r7,00000020
	srlv	r9,r12,r6
	subu	r7,r7,r6
	sllv	r4,r8,r7
	sllv	r7,r12,r7
	or	r4,r4,r9
	sltu	r7,r0,r7
	or	r12,r4,r7
	srlv	r8,r8,r6
	bc	0040F492

l0040F564:
	or	r12,r11,r4
	bnec	r0,r12,0040F508

l0040F56C:
	move	r8,r0
	bc	0040F452

l0040F570:
	addiu	r8,r7,FFFFFFD8
	move	r12,r0
	sllv	r8,r11,r8
	bc	0040F532

l0040F57C:
	addiu	r10,r10,FFFFFFE1
	move	r7,r0
	srlv	r10,r8,r10
	beqic	r6,00000020,0040F592

l0040F58A:
	subu	r6,r0,r6
	sllv	r7,r8,r6

l0040F592:
	or	r8,r12,r7
	sltu	r8,r0,r8
	or	r12,r10,r8
	move	r8,r0
	bc	0040F492

l0040F5A2:
	subu	r10,r13,r10
	ins	r8,r0,00000007,00000001
	bc	0040F23A

l0040F5AC:
	move	r8,r9
	move	r12,r6
	bc	0040F492

l0040F5B2:
	move	r8,r9
	move	r12,r6
	bc	0040F292

l0040F5B8:
	move	r8,r0
	move	r5,r3
	bc	0040F024

l0040F5C0:
	move	r5,r0
	li	r8,007FFFFF
	addiu	r12,r0,FFFFFFF8
	move	r10,r2
	bc	0040F024

l0040F5D2:
	move	r8,r0
	bc	0040F060
0040F5D8                         00 00 00 00 00 00 00 00         ........

;; __divdf3: 0040F5E0
;;   Called from:
;;     0040BD1C (in decfloat)
;;     0040E182 (in fmod)
__divdf3 proc
	ext	r11,r5,00000004,0000000B
	ext	r10,r5,00000000,00000014
	move	r9,r4
	srl	r5,r5,0000001F
	beqc	r0,r11,0040F66A

l0040F5F2:
	addiu	r8,r0,000007FF
	beqc	r11,r8,0040F6BC

l0040F5FA:
	lui	r9,00000800
	srl	r8,r4,0000001D
	or	r8,r8,r9
	sll	r10,r10,00000003
	or	r8,r8,r10
	sll	r9,r4,00000003
	addiu	r2,r11,FFFFFC01

l0040F616:
	move	r12,r0

l0040F618:
	ext	r11,r7,00000004,0000000B
	ext	r4,r7,00000000,00000014
	srl	r13,r7,0000001F
	beqc	r0,r11,0040F6E2

l0040F628:
	addiu	r7,r0,000007FF
	beqc	r11,r7,0040F730

l0040F630:
	srl	r10,r6,0000001D
	lui	r7,00000800
	or	r10,r10,r7
	sll	r4,r4,00000003
	sll	r7,r6,00000003
	or	r4,r10,r4
	addiu	r6,r11,FFFFFC01

l0040F648:
	move	r3,r0

l0040F64A:
	subu	r11,r2,r6
	sll	r6,r12,00000002
	or	r6,r6,r3
	xor	r10,r5,r13
	addiu	r6,r6,FFFFFFFF
	bgeiuc	r6,0000000F,0040F754

l0040F660:
	addiupc	r2,00003ED8
	lwxs	r6,r6(r2)
	jrc	r6

l0040F66A:
	or	r8,r10,r4
	beqc	r0,r8,0040F6CE

l0040F672:
	clz	r11,r10
	bnec	r0,r10,0040F682

l0040F67A:
	clz	r11,r4
	addiu	r11,r11,00000020

l0040F682:
	addiu	r2,r11,FFFFFFF5
	bgeic	r2,0000001D,0040F6B0

l0040F68A:
	addiu	r8,r0,0000001D
	addiu	r9,r11,FFFFFFF8
	subu	r8,r8,r2
	sllv	r10,r10,r9
	srlv	r8,r4,r8
	sllv	r9,r4,r9
	or	r8,r8,r10

l0040F6A6:
	addiu	r2,r0,FFFFFC0D
	subu	r2,r2,r11
	bc	0040F616

l0040F6B0:
	addiu	r8,r11,FFFFFFD8
	move	r9,r0
	sllv	r8,r4,r8
	bc	0040F6A6

l0040F6BC:
	or	r8,r10,r4
	beqc	r0,r8,0040F6D8

l0040F6C4:
	move	r8,r10
	move	r2,r11
	addiu	r12,r0,00000003
	bc	0040F618

l0040F6CE:
	move	r9,r0
	move	r2,r0
	addiu	r12,r0,00000001
	bc	0040F618

l0040F6D8:
	move	r9,r0
	move	r2,r11
	addiu	r12,r0,00000002
	bc	0040F618

l0040F6E2:
	or	r7,r4,r6
	beqzc	r7,0040F740

l0040F6E8:
	clz	r11,r4
	bnezc	r4,0040F6F6

l0040F6EE:
	clz	r11,r6
	addiu	r11,r11,00000020

l0040F6F6:
	addiu	r3,r11,FFFFFFF5
	bgeic	r3,0000001D,0040F724

l0040F6FE:
	addiu	r10,r0,0000001D
	addiu	r7,r11,FFFFFFF8
	subu	r10,r10,r3
	sllv	r4,r4,r7
	srlv	r10,r6,r10
	sllv	r7,r6,r7
	or	r4,r10,r4

l0040F71A:
	addiu	r6,r0,FFFFFC0D
	subu	r6,r6,r11
	bc	0040F648

l0040F724:
	addiu	r4,r11,FFFFFFD8
	move	r7,r0
	sllv	r4,r6,r4
	bc	0040F71A

l0040F730:
	or	r7,r4,r6
	beqzc	r7,0040F74A

l0040F736:
	move	r7,r6
	addiu	r3,r0,00000003
	move	r6,r11
	bc	0040F64A

l0040F740:
	move	r4,r0
	move	r6,r0
	addiu	r3,r0,00000001
	bc	0040F64A

l0040F74A:
	move	r4,r0
	move	r6,r11
	addiu	r3,r0,00000002
	bc	0040F64A

l0040F754:
	bltuc	r4,r8,0040F760

l0040F758:
	bnec	r8,r4,0040F976

l0040F75C:
	bltuc	r9,r7,0040F976

l0040F760:
	sll	r5,r8,0000001F
	srl	r6,r9,00000001
	sll	r2,r9,0000001F
	srl	r8,r8,00000001
	or	r9,r5,r6

l0040F774:
	srl	r6,r7,00000018
	sll	r4,r4,00000008
	or	r4,r4,r6
	sll	r7,r7,00000008
	ext	r13,r4,00000000,00000010
	srl	r12,r4,00000010
	divu	r14,r8,r12
	mul	r6,r13,r14
	modu	r5,r8,r12
	sll	r8,r5,00000010
	srl	r5,r9,00000010
	move	r3,r14
	or	r5,r5,r8
	bgeuc	r5,r6,0040F7B6

l0040F7A4:
	addu	r5,r5,r4
	addiu	r3,r3,FFFFFFFF
	bltuc	r5,r4,0040F7B6

l0040F7AC:
	bgeuc	r5,r6,0040F7B6

l0040F7B0:
	addiu	r3,r14,FFFFFFFE
	addu	r5,r5,r4

l0040F7B6:
	subu	r5,r5,r6
	divu	r14,r5,r12
	mul	r8,r13,r14
	modu	r6,r5,r12
	ext	r9,r9,00000000,00000010
	sll	r6,r6,00000010
	or	r9,r9,r6
	move	r6,r14
	bgeuc	r9,r8,0040F7E8

l0040F7D6:
	addu	r9,r9,r4
	addiu	r6,r6,FFFFFFFF
	bltuc	r9,r4,0040F7E8

l0040F7DE:
	bgeuc	r9,r8,0040F7E8

l0040F7E2:
	addiu	r6,r14,FFFFFFFE
	addu	r9,r9,r4

l0040F7E8:
	sll	r3,r3,00000010
	subu	r9,r9,r8
	or	r3,r3,r6
	muhu	r6,r3,r7
	mul	r5,r3,r7
	bltuc	r9,r6,0040F80A

l0040F800:
	move	r8,r3
	bnec	r9,r6,0040F84A

l0040F806:
	bgeuc	r2,r5,0040F84A

l0040F80A:
	addu	r2,r2,r7
	addiu	r8,r3,FFFFFFFF
	sltu	r14,r2,r7
	addu	r14,r14,r4
	addu	r9,r9,r14
	bltuc	r4,r9,0040F82A

l0040F822:
	bnec	r4,r9,0040F84A

l0040F826:
	bltuc	r2,r7,0040F84A

l0040F82A:
	bltuc	r9,r6,0040F836

l0040F82E:
	bnec	r6,r9,0040F84A

l0040F832:
	bgeuc	r2,r5,0040F84A

l0040F836:
	addu	r2,r2,r7
	addiu	r8,r3,FFFFFFFE
	sltu	r3,r2,r7
	addu	r3,r3,r4
	addu	r9,r9,r3

l0040F84A:
	subu	r5,r2,r5
	subu	r9,r9,r6
	sltu	r2,r2,r5
	subu	r6,r9,r2
	addiu	r9,r0,FFFFFFFF
	beqc	r4,r6,0040F91A

l0040F862:
	divu	r14,r6,r12
	mul	r3,r13,r14
	modu	r2,r6,r12
	sll	r6,r2,00000010
	srl	r2,r5,00000010
	move	r9,r14
	or	r2,r2,r6
	bgeuc	r2,r3,0040F896

l0040F880:
	addu	r2,r2,r4
	addiu	r9,r9,FFFFFFFF
	bltuc	r2,r4,0040F896

l0040F88A:
	bgeuc	r2,r3,0040F896

l0040F88E:
	addiu	r9,r14,FFFFFFFE
	addu	r2,r2,r4

l0040F896:
	subu	r2,r2,r3
	divu	r14,r2,r12
	mul	r3,r13,r14
	modu	r6,r2,r12
	andi	r5,r5,0000FFFF
	sll	r6,r6,00000010
	or	r6,r6,r5
	move	r2,r14
	bgeuc	r6,r3,0040F8C6

l0040F8B4:
	addu	r6,r6,r4
	addiu	r2,r2,FFFFFFFF
	bltuc	r6,r4,0040F8C6

l0040F8BC:
	bgeuc	r6,r3,0040F8C6

l0040F8C0:
	addiu	r2,r14,FFFFFFFE
	addu	r6,r6,r4

l0040F8C6:
	sll	r5,r9,00000010
	subu	r6,r6,r3
	or	r5,r5,r2
	muhu	r2,r7,r5
	mul	r3,r7,r5
	bltuc	r6,r2,0040F8E8

l0040F8DE:
	move	r9,r5
	bnec	r6,r2,0040F916

l0040F8E4:
	beqc	r0,r3,0040F91A

l0040F8E8:
	addu	r6,r4,r6
	addiu	r9,r5,FFFFFFFF
	bltuc	r6,r4,0040F90E

l0040F8F2:
	bltuc	r6,r2,0040F8FE

l0040F8F6:
	bnec	r2,r6,0040F916

l0040F8FA:
	bgeuc	r7,r3,0040F912

l0040F8FE:
	addiu	r9,r5,FFFFFFFE
	sll	r5,r7,00000001
	sltu	r7,r5,r7
	addu	r4,r7,r4
	move	r7,r5
	addu	r6,r6,r4

l0040F90E:
	bnec	r6,r2,0040F916

l0040F912:
	beqc	r3,r7,0040F91A

l0040F916:
	ori	r9,r9,00000001

;; fn0040F91A: 0040F91A
;;   Called from:
;;     0040F85E (in __divdf3)
;;     0040F8E4 (in __divdf3)
;;     0040F912 (in __divdf3)
;;     0040F916 (in __divdf3)
;;     0040F9A4 (in fn0040F9A4)
;;     0040F9BA (in __isoc99_vfscanf)
;;     0040F9BE (in fn0040F9BE)
;;     0040FA5A (in fn0040FA5A)
fn0040F91A proc
	addiu	r6,r11,000003FF
	bgec	r0,r6,0040F9C8

l0040F922:
	andi	r7,r9,00000007
	beqzc	r7,0040F93C

l0040F928:
	andi	r7,r9,0000000F
	beqic	r7,00000004,0040F93C

l0040F930:
	addiu	r7,r9,00000004
	sltu	r9,r7,r9
	addu	r8,r8,r9
	move	r9,r7

l0040F93C:
	bbeqzc	r8,00000018,0040F948

l0040F940:
	ins	r8,r0,00000008,00000001
	addiu	r6,r11,00000400

l0040F948:
	addiu	r7,r0,000007FE
	bltc	r7,r6,0040FA5A

l0040F950:
	sll	r7,r8,0000001D
	srl	r9,r9,00000003
	or	r9,r7,r9
	srl	r8,r8,00000003

l0040F960:
	move	r7,r0
	ins	r7,r8,00000000,00000001
	move	r8,r9
	ins	r7,r6,00000004,00000001
	ins	r7,r10,0000000F,00000001
	movep	r6,r4,r7,r8
	move	r5,r6
	jrc	ra

l0040F976:
	addiu	r11,r11,FFFFFFFF
	move	r2,r0
	bc	0040F774

l0040F97C:
	or	r4,r8,r4
	lui	r7,00000080
	and	r4,r4,r7
	li	r7,000FFFFF
	movn	r5,r0,r4

l0040F990:
	movn	r8,r7,r4

l0040F994:
	li	r7,FFFFFFFF
	move	r10,r5
	movn	r9,r7,r4

l0040F99C:
	lui	r7,00000080
	or	r8,r8,r7

;; fn0040F9A4: 0040F9A4
;;   Called from:
;;     0040F95C (in fn0040F91A)
;;     0040F9A0 (in __isoc99_vfscanf)
;;     0040FA22 (in fn0040F9BE)
;;     0040FA5E (in fn0040FA5A)
;;     0040FA66 (in fn0040F91A)
fn0040F9A4 proc
	addiu	r6,r0,000007FF

;; fn0040F9A8: 0040F9A8
;;   Called from:
;;     0040D5BE (in __isoc99_vfscanf)
;;     0040D5BE (in __isoc99_vfscanf)
;;     0040D5BE (in __isoc99_vfscanf)
;;     0040F9A4 (in fn0040F9A4)
fn0040F9A8 proc
	bc	0040F960
0040F9AA                               4D 11 04 11 27 11           M...'.
0040F9B0 83 11                                           ..              

l0040F9B2:
	beqic	r12,00000002,0040FA5A

l0040F9B6:
	beqic	r12,00000003,0040F99C

l0040F9BA:
	bneiuc	r12,00000001,0040F91A

l0040F9BC:
	illegal

;; fn0040F9BE: 0040F9BE
;;   Called from:
;;     0040F9BA (in __isoc99_vfscanf)
;;     0040F9CC (in fn0040F91A)
;;     0040FA1C (in fn0040F91A)
fn0040F9BE proc
	move	r8,r0
	move	r9,r0
	bc	0040FA20

l0040F9C4:
	move	r10,r5
	bc	0040F9B2

l0040F9C8:
	li	r5,00000001
	subu	r5,r5,r6
	bgeic	r5,00000039,0040F9BE

l0040F9D0:
	bgeic	r5,00000020,0040FA24

l0040F9D4:
	li	r6,00000020
	srlv	r4,r9,r5
	subu	r6,r6,r5
	sllv	r7,r8,r6
	sllv	r9,r9,r6
	or	r7,r7,r4
	sltu	r9,r0,r9
	or	r9,r7,r9
	srlv	r8,r8,r5

l0040F9F2:
	andi	r7,r9,00000007
	beqzc	r7,0040FA0C

l0040F9F8:
	andi	r7,r9,0000000F
	beqic	r7,00000004,0040FA0C

l0040FA00:
	addiu	r7,r9,00000004
	sltu	r9,r7,r9
	addu	r8,r8,r9
	move	r9,r7

l0040FA0C:
	bbnezc	r8,00000017,0040FA60

l0040FA10:
	sll	r7,r8,0000001D
	srl	r9,r9,00000003
	or	r9,r7,r9
	srl	r8,r8,00000003

l0040FA20:
	move	r6,r0
	bc	0040F960

l0040FA24:
	addiu	r7,r0,FFFFFFE1
	subu	r7,r7,r6
	move	r6,r0
	srlv	r7,r8,r7
	beqic	r5,00000020,0040FA3C

l0040FA34:
	subu	r5,r0,r5
	sllv	r6,r8,r5

l0040FA3C:
	or	r9,r6,r9
	move	r8,r0
	sltu	r9,r0,r9
	or	r9,r7,r9
	bc	0040F9F2

l0040FA4C:
	li	r8,000FFFFF
	addiu	r9,r0,FFFFFFFF
	move	r10,r0

l0040FA58:
	bc	0040F99C

;; fn0040FA5A: 0040FA5A
;;   Called from:
;;     0040F94C (in fn0040F91A)
;;     0040F9B2 (in __isoc99_vfscanf)
fn0040FA5A proc
	move	r8,r0
	move	r9,r0
	bc	0040F9A4

l0040FA60:
	move	r8,r0
	move	r9,r0
	li	r6,00000001
	bc	0040F960
0040FA68                         00 00 00 00 00 00 00 00         ........

;; __nedf2: 0040FA70
;;   Called from:
;;     00409278 (in fn00409170)
;;     00409296 (in fn00409170)
;;     0040940E (in fn00409170)
;;     0040947C (in fn00409170)
;;     004096CA (in fn00409170)
;;     0040BA6E (in decfloat)
;;     0040BC98 (in decfloat)
;;     0040C3FC (in __floatscan)
;;     0040C57E (in __floatscan)
;;     0040E2D0 (in frexp)
__nedf2 proc
	move	r11,r4
	move	r12,r4
	ext	r9,r5,00000004,0000000B
	addiu	r4,r0,000007FF
	ext	r8,r5,00000000,00000014
	ext	r3,r7,00000000,00000014
	ext	r10,r7,00000004,0000000B
	srl	r5,r5,0000001F
	move	r13,r6
	srl	r7,r7,0000001F
	bnec	r9,r4,0040FAA6

l0040FA96:
	or	r2,r8,r11
	li	r4,00000001
	bnec	r0,r2,0040FAD2

l0040FAA0:
	beqc	r10,r9,0040FAAA

l0040FAA4:
	jrc	ra

l0040FAA6:
	bnec	r10,r4,0040FAB2

l0040FAAA:
	or	r6,r3,r6
	li	r4,00000001
	bnezc	r6,0040FAD2

l0040FAB2:
	li	r4,00000001
	bnec	r9,r10,0040FAD2

l0040FAB8:
	bnec	r8,r3,0040FAD2

l0040FABC:
	bnec	r12,r13,0040FAD2

l0040FAC0:
	beqc	r5,r7,0040FAD0

l0040FAC2:
	bnec	r0,r9,0040FAD2

l0040FAC6:
	or	r4,r8,r11
	sltu	r4,r0,r4
	jrc	ra

l0040FAD0:
	move	r4,r0

l0040FAD2:
	jrc	ra
0040FAD4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __subdf3: 0040FAE0
;;   Called from:
;;     004091CE (in fn00409170)
;;     00409252 (in fn00409170)
;;     004093EE (in fn00409170)
;;     00409460 (in fn00409170)
;;     0040BA92 (in decfloat)
;;     0040BB7E (in decfloat)
;;     0040C364 (in __floatscan)
;;     0040C3F4 (in __floatscan)
__subdf3 proc
	ext	r10,r5,00000000,00000014
	ext	r9,r7,00000000,00000014
	sll	r10,r10,00000003
	srl	r8,r4,0000001D
	or	r8,r8,r10
	srl	r12,r7,0000001F
	ext	r10,r7,00000004,0000000B
	sll	r7,r9,00000003
	srl	r9,r6,0000001D
	ext	r13,r5,00000004,0000000B
	or	r9,r9,r7
	addiu	r7,r0,000007FF
	srl	r5,r5,0000001F
	sll	r3,r4,00000003
	sll	r6,r6,00000003
	bnec	r10,r7,0040FB24

l0040FB1E:
	or	r7,r9,r6
	bnezc	r7,0040FB28

l0040FB24:
	xori	r12,r12,00000001

l0040FB28:
	subu	r11,r13,r10
	addiu	r2,r0,000007FF
	bnec	r12,r5,0040FDF2

l0040FB34:
	bgec	r0,r11,0040FC82

l0040FB38:
	bnec	r0,r10,0040FBFE

l0040FB3C:
	or	r7,r9,r6
	bnezc	r7,0040FBA4

l0040FB42:
	move	r10,r11
	bnec	r11,r2,0040FD6A

l0040FB48:
	or	r7,r8,r3
	bnec	r0,r7,0040FD6A

l0040FB50:
	move	r8,r0
	move	r3,r0

l0040FB54:
	bbeqzc	r8,00000017,0040FB6A

l0040FB58:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	ins	r8,r0,00000007,00000001
	bnec	r10,r7,0040FB6A

l0040FB66:
	move	r8,r0
	move	r3,r0

l0040FB6A:
	sll	r6,r8,0000001D
	srl	r7,r3,00000003
	or	r7,r7,r6
	addiu	r6,r0,000007FF
	srl	r8,r8,00000003
	bnec	r10,r6,0040FB90

l0040FB80:
	or	r6,r7,r8
	beqc	r0,r6,004100FE

l0040FB88:
	lui	r6,00000080
	or	r8,r8,r6

l0040FB90:
	move	r6,r0
	ins	r6,r8,00000000,00000001
	ins	r6,r10,00000004,00000001
	ins	r6,r5,0000000F,00000001
	movep	r8,r9,r6,r7
	movep	r4,r5,r9,r8
	jrc	ra

l0040FBA4:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,0040FBEA

l0040FBAA:
	addu	r6,r3,r6
	addu	r8,r8,r9
	sltu	r7,r6,r3
	addiu	r10,r0,00000001
	addu	r8,r8,r7
	move	r3,r6

l0040FBBC:
	bbeqzc	r8,00000017,0040FD6A

l0040FBC0:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	beqc	r10,r7,0040FB50

l0040FBCA:
	move	r6,r8
	srl	r11,r3,00000001
	ins	r6,r0,00000007,00000001
	andi	r3,r3,00000001
	sll	r8,r6,0000001F
	or	r3,r11,r3
	or	r3,r8,r3
	srl	r8,r6,00000001
	bc	0040FD6A

l0040FBEA:
	bnec	r11,r2,0040FC1C

l0040FBEE:
	or	r7,r8,r3
	bnec	r0,r7,0040FDC0

l0040FBF6:
	move	r8,r0
	move	r3,r0
	move	r10,r11
	bc	0040FB54

l0040FBFE:
	bnec	r13,r2,0040FC12

l0040FC02:
	or	r7,r8,r3
	bnec	r0,r7,0040FDC0

l0040FC0A:
	move	r8,r0
	move	r3,r0
	move	r10,r13
	bc	0040FB54

l0040FC12:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11

l0040FC1C:
	bgeic	r7,00000039,0040FC78

l0040FC20:
	bgeic	r7,00000020,0040FC58

l0040FC24:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040FC46:
	addu	r6,r6,r3
	addu	r7,r7,r8
	sltu	r8,r6,r3
	move	r10,r13
	addu	r8,r8,r7
	move	r3,r6
	bc	0040FBBC

l0040FC58:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0040FC6A

l0040FC62:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0040FC6A:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040FC74:
	move	r7,r0
	bc	0040FC46

l0040FC78:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	0040FC74

l0040FC82:
	beqc	r0,r11,0040FD30

l0040FC86:
	bnec	r0,r13,0040FCF2

l0040FC8A:
	or	r7,r8,r3
	bnezc	r7,0040FCA4

l0040FC90:
	bnec	r10,r2,0040FC9E

l0040FC94:
	or	r3,r9,r6
	move	r8,r0
	beqc	r0,r3,0040FB54

l0040FC9E:
	move	r8,r9
	move	r3,r6
	bc	0040FD6A

l0040FCA4:
	nor	r11,r0,r11
	bnec	r0,r11,0040FCBA

l0040FCAC:
	addu	r3,r3,r6
	addu	r8,r8,r9

l0040FCB2:
	sltu	r6,r3,r6
	addu	r8,r8,r6
	bc	0040FBBC

l0040FCBA:
	beqc	r10,r2,0040FC94

l0040FCBE:
	bgeic	r11,00000039,0040FD26

l0040FCC2:
	bgeic	r11,00000020,0040FD04

l0040FCC6:
	li	r7,00000020
	srlv	r2,r3,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r4,r8,r7
	sllv	r3,r3,r7
	or	r4,r4,r2
	sltu	r3,r0,r3
	or	r3,r4,r3

l0040FCE8:
	addu	r3,r3,r6
	addu	r8,r11,r9
	bc	0040FCB2

l0040FCF2:
	beqc	r10,r2,0040FC94

l0040FCF6:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	0040FCBE

l0040FD04:
	srlv	r4,r8,r11
	move	r7,r0
	beqic	r11,00000020,0040FD16

l0040FD0E:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l0040FD16:
	or	r3,r7,r3
	sltu	r3,r0,r3
	or	r3,r4,r3

l0040FD22:
	move	r11,r0
	bc	0040FCE8

l0040FD26:
	or	r3,r8,r3
	sltu	r3,r0,r3
	bc	0040FD22

l0040FD30:
	addiu	r10,r13,00000001
	andi	r7,r10,000007FF
	bgeic	r7,00000002,0040FDD0

l0040FD3C:
	or	r7,r8,r3
	bnec	r0,r13,0040FD88

l0040FD44:
	beqc	r0,r7,004100D8

l0040FD48:
	or	r7,r9,r6
	move	r10,r0
	beqzc	r7,0040FD6A

l0040FD50:
	addu	r6,r3,r6
	addu	r8,r8,r9
	sltu	r7,r6,r3
	move	r3,r6
	addu	r8,r8,r7
	bbeqzc	r8,00000017,0040FD6A

l0040FD62:
	ins	r8,r0,00000007,00000001
	addiu	r10,r0,00000001

l0040FD6A:
	andi	r7,r3,00000007
	beqc	r0,r7,0040FB54

l0040FD72:
	andi	r7,r3,0000000F
	beqic	r7,00000004,0040FB54

l0040FD7A:
	addiu	r6,r3,00000004
	sltu	r7,r6,r3
	move	r3,r6
	addu	r8,r8,r7
	bc	0040FB54

l0040FD88:
	beqc	r0,r7,004100DE

l0040FD8C:
	or	r6,r9,r6
	move	r10,r2
	beqzc	r6,0040FD6A

l0040FD94:
	srl	r5,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r5
	bbnezc	r9,00000013,0040FDC6

l0040FDA4:
	ext	r4,r4,00000000,0000001D
	sll	r7,r8,0000001D
	or	r7,r7,r4
	move	r11,r12

l0040FDB0:
	sll	r5,r5,00000003
	srl	r8,r7,0000001D
	or	r8,r8,r5
	sll	r3,r7,00000003

l0040FDBE:
	move	r5,r11

l0040FDC0:
	addiu	r10,r0,000007FF
	bc	0040FD6A

l0040FDC6:
	li	r5,000FFFFF
	li	r7,FFFFFFFF
	bc	0040FDB0

l0040FDD0:
	beqc	r10,r2,0040FB50

l0040FDD4:
	addu	r6,r3,r6
	addu	r8,r8,r9
	sltu	r7,r6,r3
	srl	r6,r6,00000001
	addu	r11,r8,r7
	sll	r8,r11,0000001F
	or	r3,r8,r6
	srl	r8,r11,00000001
	bc	0040FD6A

l0040FDF2:
	bgec	r0,r11,0040FEAA

l0040FDF6:
	bnec	r0,r10,0040FE70

l0040FDFA:
	or	r7,r9,r6
	beqc	r0,r7,0040FB42

l0040FE02:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,0040FE2C

l0040FE08:
	subu	r6,r3,r6
	subu	r8,r8,r9
	sltu	r7,r3,r6
	addiu	r10,r0,00000001
	subu	r8,r8,r7
	move	r3,r6

l0040FE1E:
	bbeqzc	r8,00000017,0040FD6A

l0040FE22:
	ext	r4,r8,00000000,00000017
	move	r11,r3
	move	r13,r10
	bc	00410034

l0040FE2C:
	beqc	r11,r2,0040FBEE

l0040FE30:
	bgeic	r7,00000039,0040FEA0

l0040FE34:
	bgeic	r7,00000020,0040FE80

l0040FE38:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040FE5A:
	subu	r6,r3,r6
	subu	r7,r8,r7
	sltu	r8,r3,r6
	move	r10,r13
	subu	r8,r7,r8
	move	r3,r6
	bc	0040FE1E

l0040FE70:
	beqc	r13,r2,0040FC02

l0040FE74:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11
	bc	0040FE30

l0040FE80:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0040FE92

l0040FE8A:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0040FE92:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l0040FE9C:
	move	r7,r0
	bc	0040FE5A

l0040FEA0:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	0040FE9C

l0040FEAA:
	beqc	r0,r11,0040FF5A

l0040FEAE:
	bnec	r0,r13,0040FF1C

l0040FEB2:
	or	r7,r8,r3
	bnezc	r7,0040FECA

l0040FEB8:
	bnec	r10,r2,0040FEC4

l0040FEBC:
	or	r3,r9,r6
	beqc	r0,r3,004100E4

l0040FEC4:
	move	r8,r9
	move	r3,r6
	bc	0040FFAE

l0040FECA:
	nor	r11,r0,r11
	bnec	r0,r11,0040FEE6

l0040FED2:
	subu	r3,r6,r3
	subu	r8,r9,r8

l0040FEDA:
	sltu	r6,r6,r3
	move	r5,r12
	subu	r8,r8,r6
	bc	0040FE1E

l0040FEE6:
	beqc	r10,r2,0040FEBC

l0040FEEA:
	bgeic	r11,00000039,0040FF50

l0040FEEE:
	bgeic	r11,00000020,0040FF2E

l0040FEF2:
	li	r7,00000020
	srlv	r4,r3,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r5,r8,r7
	sllv	r3,r3,r7
	or	r5,r5,r4
	sltu	r3,r0,r3
	or	r3,r5,r3

l0040FF12:
	subu	r3,r6,r3
	subu	r8,r9,r11
	bc	0040FEDA

l0040FF1C:
	beqc	r10,r2,0040FEBC

l0040FF20:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	0040FEEA

l0040FF2E:
	srlv	r5,r8,r11
	move	r7,r0
	beqic	r11,00000020,0040FF40

l0040FF38:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l0040FF40:
	or	r3,r7,r3
	sltu	r3,r0,r3
	or	r3,r5,r3

l0040FF4C:
	move	r11,r0
	bc	0040FF12

l0040FF50:
	or	r3,r8,r3
	sltu	r3,r0,r3
	bc	0040FF4C

l0040FF5A:
	addiu	r7,r13,00000001
	andi	r7,r7,000007FF
	bgeic	r7,00000002,00410010

l0040FF66:
	or	r10,r8,r3
	or	r7,r9,r6
	bnec	r0,r13,0040FFC2

l0040FF72:
	bnec	r0,r10,0040FF86

l0040FF76:
	bnec	r0,r7,0040FEC4

l0040FF7A:
	move	r8,r0
	move	r3,r0

l0040FF7E:
	move	r10,r0
	move	r5,r0
	bc	0040FB54

l0040FF86:
	beqzc	r7,0040FFBE

l0040FF88:
	subu	r4,r3,r6
	subu	r7,r8,r9
	sltu	r10,r3,r4
	subu	r7,r7,r10
	bbeqzc	r7,00000017,0040FFB2

l0040FF9C:
	subu	r3,r6,r3
	subu	r8,r9,r8
	sltu	r6,r6,r3
	move	r10,r0
	subu	r8,r8,r6

l0040FFAE:
	move	r5,r12
	bc	0040FD6A

l0040FFB2:
	or	r3,r4,r7
	beqc	r0,r3,00410098

l0040FFBA:
	move	r8,r7
	move	r3,r4

l0040FFBE:
	move	r10,r0
	bc	0040FD6A

l0040FFC2:
	bnec	r0,r10,0040FFD2

l0040FFC6:
	beqc	r0,r7,004100EC

l0040FFCA:
	move	r8,r9
	move	r3,r6
	move	r5,r12
	bc	0040FDC0

l0040FFD2:
	move	r10,r2
	beqc	r0,r7,0040FD6A

l0040FFD8:
	srl	r7,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r7
	bbnezc	r9,00000013,00410006

l0040FFE8:
	ext	r4,r4,00000000,0000001D
	sll	r3,r8,0000001D
	or	r4,r3,r4
	move	r11,r5

l0040FFF6:
	sll	r7,r7,00000003
	srl	r8,r4,0000001D
	or	r8,r8,r7
	sll	r3,r4,00000003
	bc	0040FDBE

l00410006:
	li	r7,000FFFFF
	li	r4,FFFFFFFF
	bc	0040FFF6

l00410010:
	subu	r11,r3,r6
	subu	r4,r8,r9
	sltu	r7,r3,r11
	subu	r4,r4,r7
	bbeqzc	r4,00000017,00410090

l00410022:
	subu	r11,r6,r3
	subu	r8,r9,r8
	sltu	r6,r6,r11
	move	r5,r12
	subu	r4,r8,r6

l00410034:
	clz	r7,r4
	bnezc	r4,00410042

l0041003A:
	clz	r7,r11
	addiu	r7,r7,00000020

l00410042:
	addiu	r10,r7,FFFFFFF8
	bgeic	r10,00000020,0041009C

l0041004A:
	subu	r8,r0,r10
	sllv	r4,r4,r10
	srlv	r8,r11,r8
	sllv	r3,r11,r10
	or	r8,r8,r4

l0041005E:
	bltc	r10,r13,004100CE

l00410062:
	subu	r10,r10,r13
	addiu	r6,r10,00000001
	bgeic	r6,00000020,004100A8

l0041006E:
	li	r7,00000020
	srlv	r9,r3,r6
	subu	r7,r7,r6
	sllv	r4,r8,r7
	sllv	r7,r3,r7
	or	r4,r4,r9
	sltu	r7,r0,r7
	or	r3,r4,r7
	srlv	r8,r8,r6
	bc	0040FFBE

l00410090:
	or	r3,r11,r4
	bnec	r0,r3,00410034

l00410098:
	move	r8,r0
	bc	0040FF7E

l0041009C:
	addiu	r8,r7,FFFFFFD8
	move	r3,r0
	sllv	r8,r11,r8
	bc	0041005E

l004100A8:
	addiu	r10,r10,FFFFFFE1
	move	r7,r0
	srlv	r10,r8,r10
	beqic	r6,00000020,004100BE

l004100B6:
	subu	r6,r0,r6
	sllv	r7,r8,r6

l004100BE:
	or	r8,r3,r7
	sltu	r8,r0,r8
	or	r3,r10,r8
	move	r8,r0
	bc	0040FFBE

l004100CE:
	subu	r10,r13,r10
	ins	r8,r0,00000007,00000001
	bc	0040FD6A

l004100D8:
	move	r8,r9
	move	r3,r6
	bc	0040FFBE

l004100DE:
	move	r8,r9
	move	r3,r6
	bc	0040FDC0

l004100E4:
	move	r8,r0
	move	r5,r12
	bc	0040FB54

l004100EC:
	move	r5,r0
	li	r8,007FFFFF
	addiu	r3,r0,FFFFFFF8
	move	r10,r2
	bc	0040FB54
