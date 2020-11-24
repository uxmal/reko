;;; Segment .text (004000F0)

;; acknowledge: 004000F0
acknowledge proc
	lwpc	r7,004544E4
	subu	r5,r7,r4
	andi	r6,r5,0000FFFF
	bbnezc	r5,0000000F,00400136
	lwpc	r5,00430078
	bltc	r6,r5,00400110
	addiu	r6,r6,00000001
	illegal
	addiu	r0,r2,0000E0C5
	lw	r0,28504(r28)
	illegal
	bc	001021C2
	addiu	r2,r8,0000A8A0
	illegal
	addiu	r5,r0,00007FFF
	subu	r7,r7,r8
	bgec	r5,r7,00400136
	sh	r4,0518(r6)
	jrc	ra

;; set_socket_option.isra.0.part.1: 00400138
set_socket_option.isra.0.part.1 proc
	save	00000010,ra,00000001
	balc	004049B0
0040013E                                           40 16               @.
00400140 00 2A A6 48 A1 04 84 03 C4 10 8B 60 A0 2D 01 00 .*.H.......`.-..
00400150 00 2A EC 81 02 D2 00 2A 00 00                   .*.....*..     

;; exit: 0040015A
;;   Called from:
;;     00400B66 (in fn00400B66)
;;     004030A0 (in fn004030A0)
exit proc
	save	00000020,ra,00000001
	sw	r4,000C(sp)
	balc	00404A20
00400162       00 2A BC 48 00 2A B6 E5 83 34 00 2A C0 B0   .*.H.*...4.*..

;; main: 00400170
main proc
	save	00000060,ra,00000007
	li	r6,00000020
	li	r19,00000002
	movep	r16,r18,r4,r5
	move	r5,r0
	addu	r4,sp,r6
	balc	0040A690
00400182       81 D3 EA B4 91 D3 EB B4 FF D3 E2 B4 E5 B4   ..............
00400190 03 B4 04 B4 06 B4 07 B4 68 B6 00 2A 18 1B A0 14 ........h..*....
004001A0 20 0A EC A6 18 B2 E4 A4 FF 90 F1 C8 22 A0 69 B6  ...........".i.
004001B0 C1 04 58 0A 50 BE 00 2A D0 57 FF D3 E4 88 5A 05 ..X.P..*.W....Z.
004001C0 84 80 34 80 8E C8 60 25 E1 04 94 1E CF 53 E0 D8 ..4...`%.....S..
004001D0 F1 C8 DD B7 8A D3 1A 18 E9 34 94 9B AB 60 0E 2D .........4...`.-
004001E0 01 00 81 04 DE 06 00 2A 06 82 02 D2 FF 2B 6B FF .......*.....+k.
004001F0 82 D3 E9 B4 BB 1B 81 D3 EF 60 2E 43 05 00 B1 1B .........`.C....
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
004002B0 8B 60 3A 2C 01 00 00 2A 86 80 2F 1B EB 60 2A 42 .`:,...*../..`*B
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
00400710 8F 60 6E F9 02 00 FF 29 97 FA EB 60 34 FB 02 00 .`n....)...`4...
00400720 81 B3 47 22 0F 94 04 B8 00 2A 22 02 F0 10 01 D2 ..G".....*".....
00400730 E1 60 FF FF FF 3F 47 22 47 A4 00 2A A4 15 09 35 .`...?G"G..*...5
00400740 00 C9 10 50 CA 34 08 81 02 60 81 D3 82 D2 42 72 ...P.4...`....Br
00400750 00 2A 12 02 E9 34 E0 C8 14 10 02 35 BA D3 CA 34 .*...4.....5...4
00400760 8A D2 09 91 45 72 08 81 01 50 00 2A F8 01 80 10 ....Er...P.*....
00400770 00 2A 6E 15 E9 34 8C BB C2 34 FF D3 C7 A8 2A 00 .*n..4...4....*.
00400780 8A D3 E9 B4 EB 60 A6 3D 05 00 A8 9B 82 34 FF D0 .....`.=.....4..
00400790 91 88 20 00 00 01 04 00 E5 04 94 3D 01 D3 A0 10 .. ........=....
004007A0 00 2A BC 75 C7 D8 FF 2B 8F F9 C5 34 C7 A8 D5 3F .*.u...+...4...?
004007B0 82 D3 CF 1B EB 60 4A 3D 05 00 9A 9B 85 34 FF D0 .....`J=.....4..
004007C0 1A DA 00 01 04 00 E5 04 3A 3D 43 D3 A9 D2 00 2A ........:=C....*
004007D0 8E 75 91 88 D1 3F C1 73 48 73 74 BC 00 2A 40 56 .u...?.sHst..*@V
004007E0 21 36 64 12 38 9A 00 2A 16 56 A0 04 22 FD 9C BC !6d.8..*.V.."...
004007F0 8B 60 FA 26 01 00 00 2A 46 7B FF 29 ED F9 C2 73 .`.&...*F{.)...s
00400800 32 BF 00 0A 64 03 C5 73 32 BF 00 0A 92 2D 64 12 2...d..s2....-d.
00400810 0A BA 81 34 00 2A D8 55 93 10 67 1F 97 14 F3 98 ...4.*.U..g.....
00400820 11 17 C0 C8 D9 17 C0 C8 DD 57 A1 04 16 04 FF 29 .........W.....)
00400830 7F FA                                           ..             

;; fn00400832: 00400832
fn00400832 proc
	bc	00409BD0
00400836                   00 00 00 00 00 00 00 00 00 00       ..........

;; _start: 00400840
_start proc
	move	r30,r0
	addiupc	r25,0000000D
	addiupc	r5,FFFFFBDB
	addiupc	r28,00017DB1
	move	r4,sp
	addiu	r1,r0,FFFFFFF0
	and	sp,sp,r1
	jalrc	ra,r25
	nop
	nop

;; _start_c: 00400860
;;   Called from:
;;     0040085C (in __errno_location)
_start_c proc
	save	00000010,ra,00000001
	lw	r5,0000(r4)
	addiu	r6,r4,00000004
	move	r9,r0
	addiupc	r8,0000FAD2
	addiupc	r7,FFFFF860
	addiupc	r4,FFFFFC7C
	balc	00404988
0040087C                                     00 00 00 00             ....

;; deregister_tm_clones: 00400880
;;   Called from:
;;     004008CC (in __do_global_dtors_aux)
deregister_tm_clones proc
	addiupc	r7,00017D96
	addiupc	r4,00017D94
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
	addiupc	r4,00017D88
	li	r6,00000002
	addiupc	r7,00017D85
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
004008D0 E3 60 2A F7 BF FF 86 9B 81 04 B4 2C F0 D8 81 D3 .`*........,....
004008E0 F0 84 C0 13                                     ....           

l004008E4:
	restore.jrc	00000010,ra,00000002

;; frame_dummy: 004008E6
frame_dummy proc
	save	00000010,ra,00000001
	addiupc	r7,FFBFF712
	beqzc	r7,004008FA

l004008F0:
	addiupc	r5,00017D68
	addiupc	r4,0000964C
	jalrc	ra,r7

l004008FA:
	addiupc	r4,00017B7F
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
usage proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,00007CFF
	balc	00400B5A
	li	r4,00000001
	balc	0040357C
	li	r4,00000002
	balc	00400B66

;; create_socket: 00400966
create_socket proc
	save	00000020,ra,00000006
	movep	r17,r19,r4,r5
	movep	r16,r20,r6,r7
	move	r18,r8
	balc	00400B56
00400970 40 94 90 BF 60 0A 08 74 FF D3 10 96 87 A8 30 00 @...`..t......0.
00400980 D4 39 C0 17 E3 C8 28 08 10 CA 24 08 EB 60 5A 3B .9....(...$..`Z;
00400990 05 00 03 D0 E4 C8 D7 47 BC 39 40 16 00 2A 4A 40 .......G.9@..*J@
004009A0 A0 04 D8 FA C4 10 8B 60 44 25 01 00 B0 39 BF 1B .......`D%...9..
004009B0 10 17 FF D3 C7 A8 32 00 1C 99 9A 39 40 16 00 2A ......2....9@..*
004009C0 28 40 A0 04 E2 FA C4 10 8B 60 22 25 01 00 8E 39 (@.......`"%...9
004009D0 1A 99 02 D2 90 39 7E 39 C0 17 F3 C8 DD 0F EB 60 .....9~9.......`
004009E0 08 3B 05 00 F4 C8 D3 47 26 1F 11 F7 26 1F       .;.....G&...&. 

;; pr_echo_reply: 004009EE
pr_echo_reply proc
	save	00000010,ra,00000001
	illegal
	balc	00407630
	restore	00000010,ra,00000001
	move	r5,r4
	addiupc	r4,00007D5E
	bc	004086C0

;; write_stdout: 00400A04
write_stdout proc
	save	00000010,ra,00000004
	move	r16,r0
	movep	r18,r17,r4,r5
	subu	r6,r17,r16
	addu	r5,r18,r16
	li	r4,00000001
	balc	0040B080
00400A14             00 B2 30 AA F1 FF 04 A8 ED BF 14 1F     ..0.........

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
	lh	r7,0004(r16)
	lwpc	r7,004544E4
	addiu	r7,r7,00000001
	sh	r4,0006(r16)
	andi	r6,r7,0000FFFF
	addiupc	r7,00028C78
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
	lwpc	r7,004544EC
	addiu	r17,r16,00000008
	bbeqzc	r7,0000000C,00400AEA
	move	r5,r0
	addiu	r4,sp,00000008
	balc	0040AF40
	li	r6,00000008
	addu	r5,sp,r6
	move.balc	r4,r17,0040A130
	lwpc	r17,00430074
	addiu	r17,r17,00000008
	movep	r5,r6,r17,r0
	move.balc	r4,r16,00400920
	lwpc	r7,00454508
	sh	r4,0002(r16)
	beqzc	r7,00400AD2
	lwpc	r7,004544EC
	bbnezc	r7,0000000C,00400AD2
	move	r5,r0
	addiu	r4,sp,00000008
	balc	0040AF40
	li	r6,00000008
	addu	r5,sp,r6
	addu	r4,r16,r6
	balc	00400B52
	illegal
	li	r5,00000008
	nor	r6,r0,r6
	addu	r4,sp,r5
	andi	r6,r6,0000FFFF
	balc	00400920
	sh	r4,0002(r16)
	lw	r4,0000(r18)
	addiu	r9,r0,00000010
	movep	r6,r7,r17,r0
	addiupc	r8,000184D5
	move.balc	r5,r16,00407D40
	xor	r17,r17,r4
	movz	r4,r0,r17
	restore.jrc	00000020,ra,00000004
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
	lwpc	r7,004544C4
	li	r16,00000001
	sw	r7,000C(sp)
	illegal
	addiu	r0,r5,0000849D
	illegal
	addiupc	r7,00017A78
	sb	r0,0012(r7)
	li	r6,00000015
	sb	r16,0013(r7)
	addiu	r8,r0,00000008
	sw	r4,0054(sp)
	addiu	r5,r0,0000FFFF
	lw	r4,0000(r17)
	sh	r6,0010(r7)
	addiupc	r7,00017A65
	li	r6,0000001A
	balc	00400B4A
	beqzc	r4,00400B48
	addiupc	r4,00007CD2
	restore	00000020,ra,00000003
	bc	00408630
	restore.jrc	00000020,ra,00000003

;; fn00400B4A: 00400B4A
;;   Called from:
;;     00400D28 (in fn00400D26)
fn00400B4A proc
	bc	00407D60
00400B4E                                           00 28               .(
00400B50 3E 9B                                           >.             

;; fn00400B52: 00400B52
;;   Called from:
;;     00400BDE (in ping4_run)
;;     00400C7C (in ping4_run)
fn00400B52 proc
	bc	0040A130

l00400B56:
	bc	004049B0

;; fn00400B5A: 00400B5A
fn00400B5A proc
	bc	004083F0

;; fn00400B5E: 00400B5E
fn00400B5E proc
	bc	00408340

;; fn00400B62: 00400B62
fn00400B62 proc
	bc	00406700

;; fn00400B66: 00400B66
;;   Called from:
;;     00400C98 (in fn00400CBE)
fn00400B66 proc
	bc	0040015A

;; ping4_run: 00400B6A
ping4_run proc
	save	00000180,ra,00000008
	movep	r17,r21,r4,r5
	movep	r18,r16,r6,r7
	bltic	r17,00000002,00400C24

l00400B76:
	lwpc	r7,004544EC
	bbeqzc	r7,00000005,00400B82
	balc	0040094E
	bbeqzc	r7,00000009,00400B96
	lwpc	r7,0045453C
	bneiuc	r7,00000003,00400B80
	bltic	r17,00000006,00400C24
	bc	00400B80
	bgeic	r17,0000000B,00400B80
	ori	r7,r7,00000400
	illegal
	addiu	r0,r5,0000187E

l00400BA6:
	sw	r0,0038(sp)
	bneiuc	r17,00000001,00400BAE

l00400BAC:
	bnezc	r18,00400BD2

l00400BAE:
	addiu	r7,sp,00000038
	addiupc	r6,00008B24
	movep	r4,r5,r20,r0
	balc	00405E20
00400BBA                               14 9A 00 2A 40 52           ...*@R
00400BC0 A0 04 4C F9 9C BC 8B 60 24 23 01 00 91 3B C6 18 ..L....`$#...;..
00400BD0 4E 36                                           N6             

l00400BD2:
	lw	r5,0014(r18)
	li	r6,00000010
	addiupc	r4,00018457
	addiu	r20,sp,00000060
	balc	00400B52
00400BE0 C0 00 FF 00 74 BC 67 3B A6 16 88 9A C0 00 FE 00 ....t.g;........
00400BF0 80 0A 3C 9D 8E 34 8F 62 C4 38 05 00 04 9A 00 2A ..<..4.b.8.....*
00400C00 EE 51 20 CA 1A 08 EB 60 2C 39 05 00 B1 16 C7 00 .Q ....`,9......
00400C10 01 00 CF 60 20 39 05 00 C3 04 7C 08 C7 20 C7 2C ...` 9....|.. .,
00400C20 3F 92 AC 92                                     ?...           

l00400C24:
	bgec	r0,r17,00400C6A

l00400C28:
	lw	r20,0000(r21)
	addiupc	r19,0001842D
	aluipc	r7,00000400
	li	r6,00000002
	sw	r0,0488(r7)
	addiupc	r5,00018428
	sw	r0,0044(sp)
	sw	r0,0048(sp)
	sw	r0,004C(sp)
	sh	r6,0488(r7)
	move.balc	r4,r20,004067A0
	bneiuc	r4,00000001,00400BA6

l00400C4E:
	illegal
	addiu	r0,r5,0000CA30
	illegal
	balc	00400CEA
	addiu	r0,r5,000080E7
	addiu	r0,r4,000060EF
	balc	00400CEA
	addiu	r0,r5,00001BB7

l00400C6A:
	addiupc	r18,000179EF
	lw	r7,0004(r18)
	bnec	r0,r7,00400E36

l00400C74:
	li	r6,00000010
	addiupc	r5,00018407
	addiu	r4,sp,00000028
	balc	00400B52
00400C7E                                           C0 10               ..
00400C80 81 D2 02 D2 00 2A F8 70 24 12 04 88 0C 80 80 04 .....*.p$.......
00400C90 8E F8                                           ..             

l00400C92:
	balc	00408630

l00400C96:
	li	r4,00000002
	balc	00400B66
00400C9A                               6B 62 88 38 05 00           kb.8..
00400CA0 60 8A 94 00 20 D3 A0 10 4E 72 A3 3A 0F D3 4E 72 `... ...Nr.:..Nr
00400CB0 60 0B 7C 9C 01 D2 00 2A 28 10 6B 62 68 38       `.|....*(.kbh8 

;; fn00400CBE: 00400CBE
fn00400CBE proc
	addiu	r0,r5,00000A60
	beqzc	r7,00400D10

l00400CC4:
	move	r7,r19
	addiu	r8,r4,00000001
	li	r6,00000019
	addiu	r5,r0,0000FFFF
	move.balc	r4,r17,00407D60
	move	r19,r4
	move	r4,r0
	balc	00401CE2
00400CDC                                     FF D3 67 AA             ..g.
00400CE0 56 00 8B 34 00 2A 38 69 E0 E0 01 0C 80 80 C0 E6 V..4.*8i........
00400CF0 87 A8 3E 00 4E 73 A0 00 33 89 20 0A 82 4E 04 88 ..>.Ns..3. ..N..
00400D00 14 80 CB 60 20 38 05 00 A0 04 1C F8 8B 60 DE 21 ...` 8.......`.!

l00400D10:
	addiu	r0,r1,00003A4B
	bc	00400C96
00400D16                   F2 34 00 01 0C 00 20 D3 07 B4       .4.... ...
00400D20 E9 B4 C7 73 71 BC                               ...sq.         

;; fn00400D26: 00400D26
fn00400D26 proc
	sw	r0,0020(sp)
	balc	00400B4A
00400D2A                               C6 D9 80 04 10 F8           ......
00400D30 61 1B 80 04 22 F8                               a...".         

;; fn00400D36: 00400D36
fn00400D36 proc
	bc	00400C92
00400D38                         EB 60 F2 37 05 00 9A 9B         .`.7....
00400D40 00 01 04 00 E5 04 E8 37 01 D3 71 BC FD 39 04 88 .......7..q..9..
00400D50 08 80 80 04 1A F8 00 2A D6 78 80 00 01 04 03 3A .......*.x.....:
00400D60 EB 60 D2 37 05 00 9D 84 2A 50 8A 9B E3 E0 02 10 .`.7....*P......
00400D70 E7 84 98 84 EB B4 10 D3 CA 72 20 0A 52 50 FF D3 .........r .RP..
00400D80 64 12 87 A8 50 00 CF 39 C0 17 F0 C8 42 68 EB 60 d...P..9....Bh.`
00400D90 5C 21 01 00 E3 B4 EB 60 90 37 05 00 A3 34 80 04 \!.....`.7...4..
00400DA0 F2 F7 E6 9B 80 04 14 F8 B1 39 00 01 04 00 E5 04 .........9......
00400DB0 7A 37 20 D3 A0 00 FF FF 20 0A A4 6F 04 88 06 80 z7 ..... ..o....
00400DC0 80 04 1C F8 CD 1A 10 D3 CA 72 20 0A 02 50 C3 D9 .........r ..P..
00400DD0 80 04 24 F8 BD 1A 90 D3 47 73 A2 04 6E F2 E7 B4 ..$.....Gs..n...
00400DE0 20 0A CC 58 FF D3 73 DA 80 04 14 F8 A5 1A EB 60  ..X..s........`
00400DF0 34 37 05 00 23 7C BA 9B 4E 72 00 2A A4 54 0E 9A 47..#|..Nr.*.T..
00400E00 AB 60 EA 20 01 00 80 04 02 F8 4F 39 89 1A AE 36 .`. ......O9...6
00400E10 CB 62 12 37 05 00 75 12 FC B9 A0 0A 74 54 92 B9 .b.7..u.....tT..
00400E20 CB 60 02 37 05 00 A0 04 FA F7 8B 60 C0 20 01 00 .`.7.......`. ..
00400E30 2D 39 20 0A 3C A1                               -9 .<.         

l00400E36:
	addiupc	r17,00018327
	lw	r7,0004(r17)
	bnezc	r7,00400E42

l00400E3E:
	lw	r7,0004(r18)
	sw	r7,0044(sp)

l00400E42:
	lwpc	r19,00454528
	beqzc	r19,00400E72
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
	li	r7,00000018
	illegal
	addiu	r0,r5,000060EB
	lw	r5,0050(sp)
	addiu	r0,r5,00009BCA
	lwpc	r7,00454514
	bnezc	r7,00400EFC
	lwpc	r7,00430048
	bgec	r7,r0,00400EE0
	li	r7,00000002
	illegal
	addiu	r0,r2,00001840
	lw	r20,0003(r19)
	beqc	r0,r20,00400EC0
	lhu	r7,0000(r20)
	bneiuc	r7,00000002,00400EC0
	lw	r4,0004(r19)
	li	r6,00000003
	move.balc	r5,r22,0040A8E0
	bnezc	r4,00400EC0
	li	r6,00000004
	addiupc	r5,000178CE
	addu	r4,r20,r6
	balc	0040A100
	beqc	r0,r4,00400E1A
	lw	r19,0000(r19)
	bc	00400E18
	lw	r4,0004(r17)
	balc	00407620
	lui	r7,FFFE0000
	ins	r4,r0,00000000,00000001
	beqc	r4,r7,00400E7A
	lwpc	r7,00430048
	bltc	r7,r0,00400F30
	lw	r4,0000(r16)
	addiupc	r7,000178B1
	addiu	r8,r0,00000004
	li	r6,0000000A
	move	r5,r0
	balc	00400B4A
	li	r7,FFFFFFFF
	bnec	r7,r4,00400F30
	addiupc	r4,00007BE9
	bc	00400C92
	lwpc	r7,0043008C
	addiu	r6,r0,000003E7
	bltc	r6,r7,00400F16
	lwpc	r5,00412EF0
	addiupc	r4,00007BAE
	bc	00400E0A
	lwpc	r7,00430048
	bltc	r7,r0,00400E82
	beqic	r7,00000002,00400E82
	lwpc	r5,00412EF0
	addiupc	r4,00007BB9
	bc	00400E0A
	lwpc	r7,004544EC
	bbeqzc	r7,0000000F,00400F50
	lw	r4,0000(r16)
	li	r6,00000010
	addiupc	r5,00017885
	balc	00405DB0
	li	r7,FFFFFFFF
	bnec	r4,r7,00400F50
	addiupc	r4,00007BCB
	bc	00400C92
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00400F78
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
	addiupc	r4,00007BBB
	balc	00401284
	li	r7,00000001
	lw	r4,0000(r16)
	sw	r7,001C(sp)
	addiu	r8,r0,00000004
	li	r6,0000000B
	move	r5,r0
	addiu	r7,sp,0000001C
	balc	00401280
	beqzc	r4,00400F9A
	lwpc	r5,00412EF0
	addiupc	r4,00007BBD
	balc	004083F0
	lw	r7,0004(r16)
	bneiuc	r7,00000001,00400FCC
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,sp,0000001C
	li	r6,0000000C
	move	r5,r0
	balc	00401280
	beqzc	r4,00400FB6
	addiupc	r4,00007BC8
	balc	00401284
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,sp,0000001C
	li	r6,00000007
	move	r5,r0
	balc	00401280
	beqzc	r4,00400FCC
	addiupc	r4,00007BCD
	balc	00401284
	lwpc	r7,004544EC
	bbeqzc	r7,00000005,00401012
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
	illegal
	addiu	r0,r5,00003A78
	bgec	r4,r0,00401012
	addiupc	r4,00007BBA
	bc	00400C92
	lwpc	r7,004544EC
	bbeqzc	r7,00000009,004010A8
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
	sb	r7,003B(sp)
	sb	r6,0039(sp)
	li	r6,00000005
	sb	r6,003A(sp)
	beqic	r7,00000003,0040107A
	lbu	r8,0039(sp)
	addiu	r7,sp,00000038
	lw	r4,0000(r16)
	li	r6,00000004
	move	r5,r0
	balc	00401280
	bgec	r4,r0,004010A0
	li	r7,00000002
	lbu	r8,0039(sp)
	lw	r4,0000(r16)
	li	r6,00000004
	sb	r7,003B(sp)
	move	r5,r0
	addiu	r7,sp,00000038
	balc	00401280
	bgec	r4,r0,004010A0
	addiupc	r4,00007B90
	bc	00400C92
	lwpc	r5,00454538
	addiu	r6,sp,00000038
	sll	r7,r5,00000003
	addiu	r7,r7,00000004
	sb	r7,0039(sp)
	move	r7,r0
	addiu	r6,r6,00000008
	bgec	r7,r5,0040104A
	addiupc	r4,00018201
	lwxs	r4,r7(r4)
	addiu	r7,r7,00000001
	sw	r4,-0004(r6)
	bc	0040108C
	li	r7,00000028
	illegal
	addiu	r0,r5,0000626B
	lw	r16,0078(sp)
	addiu	r0,r5,0000CA64
	illegal
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
	lwpc	r8,00454538
	move	r19,r7
	sll	r7,r8,00000002
	addiu	r7,r7,00000003
	move	r6,r0
	sb	r7,003A(sp)
	li	r7,00000004
	sb	r7,003B(sp)
	addiupc	r7,000181D4
	sb	r19,0039(sp)
	bltc	r6,r8,00401162
	addiu	r8,r8,00000001
	lw	r4,0000(r16)
	sll	r8,r8,00000002
	move	r7,r20
	li	r6,00000004
	move	r5,r0
	balc	00401280
	bltc	r4,r0,0040100C
	li	r7,00000028
	illegal
	addiu	r0,r5,000060EB
	illegal
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
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiupc	r7,000299EF
	li	r6,00000020
	addiu	r5,r0,0000FFFF
	balc	00401280
	bgec	r4,r0,0040116E
	addiupc	r4,00007B25
	bc	00400C92
	lw	r5,0000(r7)
	addiu	r6,r6,00000001
	addiu	r7,r7,00000004
	swxs	r5,r6(r20)
	bc	004010F4
	lwpc	r7,004544EC
	bbeqzc	r7,00000010,00401194
	lw	r4,0000(r16)
	addiu	r7,sp,00000028
	addiu	r8,r0,00000001
	li	r6,00000022
	move	r5,r0
	sw	r0,0028(sp)
	balc	00401280
	li	r7,FFFFFFFF
	bnec	r4,r7,00401194
	addiupc	r4,00007B1C
	bc	00400C92
	lwpc	r7,004544EC
	bbeqzc	r7,00000011,004011DA
	lwpc	r7,00454510
	lw	r4,0000(r16)
	sw	r7,0028(sp)
	addiu	r8,r0,00000001
	li	r6,00000021
	move	r5,r0
	addiupc	r7,000299AE
	li	r19,FFFFFFFF
	balc	00401280
	bnec	r19,r4,004011C2
	addiupc	r4,00007B19
	bc	00400C92
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,sp,00000028
	li	r6,00000002
	move	r5,r0
	balc	00401280
	bnec	r19,r4,004011DA
	addiupc	r4,00007B21
	bc	00400C92
	lwpc	r6,00454534
	addiu	r7,r0,0000FFE3
	subu	r7,r7,r6
	lwpc	r6,00430074
	bgec	r7,r6,004011F8
	addiupc	r5,00007B26
	bc	00400BC6
	bltiuc	r6,00000008,00401204
	li	r7,00000001
	illegal
	addiu	r0,r5,00000266
	addiu	r4,r8,00000A60
	addiu	r4,r28,00061284
	bnezc	r4,0040121E
	lwpc	r5,00412EF0
	addiupc	r4,00007B2D
	bc	00400E0A
	lw	r4,0004(r17)
	lwpc	r21,004544C0
	balc	00401590
	movep	r5,r6,r21,r4
	addiupc	r4,00007B2F
	balc	00401594
	lwpc	r7,00454528
	bnezc	r7,00401242
	lwpc	r7,004544EC
	bbeqzc	r7,0000000F,0040125C
	lw	r4,0004(r18)
	balc	00401590
	lwpc	r6,00454528
	addiupc	r7,00007914
	move	r5,r4
	movz	r6,r7,r6
	addiupc	r4,00007B21
	balc	00401594
	lwpc	r5,00430074
	lwpc	r6,00454534
	addu	r6,r5,r6
	addiupc	r4,00007B1F
	addiu	r6,r6,0000001C
	balc	00401594
	move.balc	r4,r16,004021B4
	movep	r6,r7,r20,r19
	addiupc	r4,000176F0
	move.balc	r5,r16,00402A9E
	bc	00407D60
	bc	00408630

;; pr_addr: 00401288
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
0040129E                                           03 BF               ..
004012A0 9D 00 14 01 1D 84 10 91 BC 38 C2 34 EB 60 6E 32 .........8.4.`n2
004012B0 05 00 77 DB 83 34 A3 04 26 01 00 2A 42 8E 80 88 ..w..4..&..*B...
004012C0 9C 00 E2 34 83 04 18 01 A3 34 C7 10 EF 60 4E 32 ...4.....4...`N2
004012D0 05 00 34 3B 85 04 68 30 00 2A A4 6B E0 00 FF 00 ..4;..h0.*.k....
004012E0 84 80 01 50 A2 34 8F 60 D4 01 03 00 83 34 40 01 ...P.4.`.....4@.
004012F0 01 00 20 11 00 11 DD 00 10 01 00 2A 04 50 EB 60 .. ........*.P.`
00401300 B4 31 05 00 9C 9B FD 84 10 20 B6 BB FD 00 10 01 .1....... ......
00401310 C0 04 5C F9 A0 00 00 10 82 04 C4 F0 00 2A 00 75 ..\..........*.u
00401320 36 18 EB 60 C4 31 05 00 F4 C8 DB 17 A2 34 40 11 6..`.1.......4@.
00401330 83 34 20 11 00 11 E0 00 FF 00 44 73 00 2A C2 4F .4 .......Ds.*.O
00401340 C5 1B 1D 01 10 01 C4 73 C0 04 1C F9 A0 00 00 10 .......s........
00401350 82 04 8C F0 00 2A C8 74 0F 60 62 01 03 00 82 04 .....*.t.`b.....
00401360 7E F0 E2 83 23 32                               ~...#2         

l00401366:
	bc	0040A690

;; pr_options: 0040136A
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
	addiupc	r4,00007C78
	addiu	r20,r20,FFFFFFFF
	addiu	r16,r16,00000001
	balc	00401594
0040138A                               E7 1B                       ..   

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
	addiupc	r4,00007CB9
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
	addiupc	r4,00007C5D
	movn	r7,r6,r5

l004013C6:
	move.balc	r5,r7,004086C0
	lbu	r21,0001(r16)
	bltic	r21,00000005,004013FA

l004013D2:
	addiu	r18,r16,00000003
	move	r17,r18
	move	r5,r17
	li	r6,00000004
	addiu	r4,sp,0000000C
	addiu	r17,r17,00000004
	balc	00401608
004013E2       E3 34 9C BB 80 04 9E F8 A8 39 0A D2 76 3B   .4.......9..v;
004013F0 35 22 D0 39 AE B3 E8 C8 DF 2F                   5".9...../     

l004013FA:
	subu	r20,r20,r19
	addu	r16,r16,r19
	bc	00401372
00401402       90 D2 02 D3 BD 20 50 21 04 B4 E5 B4 DD 84   ..... P!......
00401410 10 50 06 B4 07 B4 71 3A A4 10 80 04 76 F8 74 39 .P....q:....v.t9
00401420 CB 1B                                           ..             

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
	move	r6,r17
	addiupc	r5,0001800B
	move.balc	r4,r18,0040A100
	bnezc	r4,00401462
	lwpc	r7,004544EC
	bbnezc	r7,00000000,00401462
	addiupc	r4,00007C1D
	balc	00401594
	bc	004013FA
	movep	r5,r6,r18,r17
	addiupc	r4,00017FFC
	illegal
	addiu	r0,r5,00003998
	addiupc	r4,00007C1A
	addiu	r18,r16,00000003
	balc	00401594
	subu	r5,r21,r17
	li	r6,00000004
	addu	r5,r18,r5
	addiu	r4,sp,0000000C
	balc	00401608
	lw	r17,000C(sp)
	bnezc	r7,0040149C
	addiupc	r4,00007BFD
	balc	00401594
	addiu	r17,r17,FFFFFFFC
	li	r4,0000000A
	balc	00401766
	bltc	r0,r17,0040147A
	bc	004013FA
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
	addiupc	r4,00007BEE
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
	addiupc	r4,00007BEF
	lbu	r21,0003(r16)
	addiu	r30,r16,00000004
	move	r23,r0
	balc	00401594
004014DE                                           C0 12               ..
004014E0 42 18 90 D2 02 D3 BD 20 50 21 04 B4 E5 B4 DD 84 B...... P!......
004014F0 10 50 06 B4 07 B4 91 39 A4 10 80 04 96 F7 94 38 .P.....9.......8
00401500 3E 18 F1 22 D0 29 80 04 CA F7 64 18 C0 AA 66 00 >..".)....d...f.
00401510 B1 10 80 04 D2 F7 7C 38 D1 12 5C 92 0A D2 46 3A ......|8..\...F:
00401520 40 8A 5C 80 F5 80 0F 20 3E 12 9A 9B 04 D3 BE 10 @.\.... >.......
00401530 43 72 91 90 D2 38 E3 34 A9 BB 80 04 4A F7 54 38 Cr...8.4....J.T8
00401540 5C 92 40 8A 3A 80 18 5F D1 03 04 00 99 5F 60 33 \.@.:.._....._`3
00401550 7E B3 70 33 9A 5F 9B 5C 7E B3 F0 33 92 B3 11 88 ~.p3._.\~..3....
00401560 AB BF 31 82 80 F7 E0 AA 99 3F B1 10 80 04 48 F7 ..1......?....H.
00401570 22 38 F1 12 A5 1B D1 22 D0 29 80 04 7A F7 97 1B "8.....".)..z...
00401580 B5 80 44 C0 80 04 78 F7 A0 88 6F 3E             ..D...x...o>   

l0040158C:
	balc	00401594
0040158E                                           6B 1A               k.

;; fn00401590: 00401590
fn00401590 proc
	bc	00406830

;; fn00401594: 00401594
;;   Called from:
;;     00401388 (in pr_options)
;;     004014DC (in pr_options)
;;     0040158C (in pr_options)
;;     0040175E (in pr_icmph)
fn00401594 proc
	bc	004086C0

;; pr_iph: 00401598
pr_iph proc
	save	00000010,ra,00000004
	move	r16,r4
	addiupc	r4,00007BC6
	addiupc	r18,00007C04
	lw	r17,0000(r16)
	balc	00401762
004015A8                         80 16 80 04 C2 F7 30 85         ......0.
004015B0 04 60 9F F0 10 85 02 60 5F F3 89 5F A5 80 C4 F0 .`.....`_.._....
004015C0 92 30 D1 3B 8E 7E 80 04 C2 F7 C5 80 00 F3 A5 80 .0.;.~..........
004015D0 4D C0 C1 3B F0 84 0A 60 D0 84 09 20 80 04 B8 F7 M..;...`... ....
004015E0 B0 84 08 20 AF 3B 03 16 A7 3B A4 10 40 0A D0 70 ... .;...;..@..p
004015F0 04 16 9D 3B A4 10 40 0A C6 70 0A D2 68 39 B1 10 ...;..@..p..h9..
00401600 05 92 E4 83 12 30 63 19                         .....0c.       

l00401608:
	bc	0040A130

;; pr_icmph: 0040160C
pr_icmph proc
	save	00000020,ra,00000003
	movep	r17,r16,r6,r7
	bgeiuc	r4,00000013,00401758

l00401614:
	addiupc	r7,000085AC
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
	addiupc	r4,00007D03
	balc	00401594
00401760 23 1F                                           #.             

l00401762:
	bc	00408780

;; fn00401766: 00401766
fn00401766 proc
	bc	00408770

;; ping4_receive_error_msg: 0040176A
ping4_receive_error_msg proc
	save	00000270,ra,00000005
	move	r17,r4
	balc	004049B0
00401774             FD 80 B0 8D 07 12 C7 73 C0 15 C0 00     .......s....
00401780 40 20 F0 84 C4 9D 88 D3 F0 84 C8 9D C9 73 F0 84 @ ...........s..
00401790 E4 9D 90 D3 F0 84 E8 9D C5 73 F0 84 EC 9D 81 D3 .........s......
004017A0 F0 84 F0 9D D4 73 10 16 CD 72 F0 84 F4 9D E0 00 .....s...r......
004017B0 00 02 10 84 FC 9D F0 84 F8 9D 00 2A B2 5E 04 A8 ...........*.^..
004017C0 B8 80 D0 84 F8 8D E0 10 DC C8 04 60 F0 84 F4 8D ...........`....
004017D0 B1 34 40 12 5A B3 A8 9B 71 17 08 BB 72 17 D0 C8 .4@.Z...q...r...
004017E0 02 58 73 91 70 17 DC C8 12 60 CB 90 D1 B3 C0 80 .Xs.p....`......
004017F0 40 E0 06 01 0C 00 7E B3 08 AA DB FF E0 10 D7 1B @.....~.........
00401800 04 B9 00 2A FA 31 B2 84 04 20 B0 C8 62 08 EB 60 ...*.1... ..b..`
00401810 D8 2C 05 00 07 82 10 20 00 AA 1A 01 E4 C8 28 00 .,..... ......(.
00401820 80 04 54 F9 7A 3B EB 60 A4 2C 05 00 81 D0 E9 90 ..T.z;.`.,......
00401830 EF 60 9A 2C 05 00 00 2A 76 31 C0 95 04 B8 20 22 .`.,...*v1.... "
00401840 D0 81 90 10 E5 83 73 32 20 16 EB 60 A0 16 01 00 ......s2 ..`....
00401850 E3 B4 82 C8 12 D0 00 2A 90 31 A0 04 1E F9 C4 10 .......*.1......
00401860 83 34 00 2A DA 6A BF 1B A0 04 28 F9 22 17 F1 1B .4.*.j....(."...
00401870 B0 C8 06 10 88 C8 06 40 60 12 20 12 BA 18 E2 04 .......@`. .....
00401880 06 FC CA 34 F1 17 C7 A8 EF 3F FD 84 1C 20 F0 C8 ...4.....?... ..
00401890 E7 47 BD 84 20 60 20 0A 3C 14 5D 9A 9D 84 22 60 .G.. ` .<.]..."`
004018A0 FA 3A FF 2B 4B E8 91 17 F0 C8 2A 18 92 17 A6 BB .:.+K.....*.....
004018B0 01 D3 E0 80 32 80 10 16 00 01 04 00 E4 B4 A0 00 ....2...........
004018C0 FF 00 C4 73 12 97 00 2A 96 64 FF D3 74 DA 80 04 ...s...*.d..t...
004018D0 F2 F8 00 2A 5A 6D EB 60 F4 2B 05 00 0B 62 0A 2C ...*Zm.`.+...b.,
004018E0 05 00 E9 90 30 82 10 20 EF 60 E2 2B 05 00 C0 B8 ....0.. .`.+....
004018F0 01 F0 0A 98 82 D2 80 04 EE F8 A4 3A 39 1B 00 2A ...........:9..*
00401900 5A 05 90 D2 A8 B2 FF 2B 7F F9 24 12 9D 84 22 60 Z......+..$..."`
00401910 8A 3A 91 BD 80 04 D4 F8 7B 38 92 84 05 20 B2 84 .:......{8... ..
00401920 06 20 E0 10 22 17 E5 38 8B 60 C6 15 01 00 68 3A . .."..8.`....h:
00401930 20 12 01 D0 01 1B 25 12 00 12 FB 1A              .....%.....   

;; fn0040193C: 0040193C
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
	move	r4,r17
	restore.jrc	00000050,ra,00000009
00401974             90 D2 5F 0A 0F F9 A0 04 86 F8 9C BC     .._.........
00401980 8B 60 6A 15 01 00 00 2A B6 69 E3 1B             .`j....*.i..   

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
	illegal
	balc	00401B9C
	li	r5,00000010
	move	r19,r4
	move.balc	r4,r18,00401288
	addiupc	r7,FFFFF815
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
	lwpc	r4,00412EF4
	balc	00401B98
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
	bnec	r0,r19,00401AFA
	li	r5,00000002
	addiupc	r4,00007BF3
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
	addiupc	r7,00017CFA
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
	illegal
	balc	00401B9C
	balc	004000F0
	bc	004019E4

l00401ABA:
	lwpc	r7,004544EC
	andi	r7,r7,00000011
	bnec	r0,r7,0040196E
	balc	00401E5C
	li	r5,00000010
	move.balc	r4,r18,00401288
	move	r18,r4
	illegal
	balc	00401B9C
	movep	r5,r6,r18,r4
	addiupc	r4,00007BA9
	balc	00401B94
	beqzc	r19,00401AE8
	addiupc	r4,00007BB1
	balc	00401B94
	lw	r4,0004(r16)
	lbu	r17,0000(r16)
	lbu	r18,0001(r16)
	balc	0040193C
	movep	r6,r7,r4,r16
	movep	r4,r5,r17,r18
	balc	0040160C
	bc	0040196E
	li	r5,00000003
	addiupc	r4,00007BAE
	balc	00401BA0
	bc	004019E4
	bbeqzc	r7,00000008,004019E4
	lwpc	r6,00454514
	bnec	r0,r6,004019E4
	bbeqzc	r7,00000013,00401B28
	move	r5,r0
	addiu	r4,sp,00000018
	balc	0040AF40
	addiupc	r4,00007B9F
	lwm	r5,0018(sp),00000002
	balc	00401B94
	li	r5,00000010
	move.balc	r4,r18,00401288
	move	r5,r4
	addiupc	r4,00007B9C
	balc	00401B94
	beqzc	r19,00401B40
	addiupc	r4,00007B86
	balc	00401762
	bc	004019E4
	lw	r4,0004(r16)
	lbu	r18,0000(r16)
	lbu	r19,0001(r16)
	balc	0040193C
	movep	r6,r7,r4,r16
	movep	r4,r5,r18,r19
	balc	0040160C
	bc	00401970
	lwpc	r7,004544EC
	bbeqzc	r7,0000000D,00401B72
	li	r4,00000007
	balc	00401766
	lwpc	r7,004544EC
	bbeqzc	r7,00000000,00401B7C
	lwpc	r4,00412EF4
	balc	00401B98
	lwpc	r7,004544EC
	bbnezc	r7,00000000,004019E4
	addiu	r5,r22,00000014
	move.balc	r4,r21,0040136A
	li	r4,0000000A
	balc	00408770
	lwpc	r4,00412EF4
	balc	00401B98
	bc	00401970
	bc	004086C0
	bc	004081A0
	bc	00407630
	bc	00400A04
	nop
	nop
	nop

;; in_flight: 00401BB0
in_flight proc
	aluipc	r6,00000401
	lwpc	r7,004544E4
	lhu	r6,0518(r6)
	subu	r6,r7,r6
	andi	r4,r6,0000FFFF
	bbeqzc	r6,0000000F,00401BD6
	lwpc	r4,004544DC
	subu	r7,r7,r4
	lwpc	r4,004544D0
	subu	r4,r7,r4
	jrc	ra

;; advance_ntransmitted: 00401BD8
advance_ntransmitted proc
	lwpc	r6,004544E4
	aluipc	r5,00000401
	addiu	r7,r6,00000001
	lhu	r4,0518(r5)
	illegal
	addiu	r0,r5,0000F3FD
	subu	r7,r7,r4
	addiu	r4,r0,00007FFF
	bgec	r4,r7,00401C02
	addiu	r6,r6,00000002
	sh	r6,0518(r5)
	jrc	ra

;; sigstatus: 00401C04
sigstatus proc
	li	r7,00000001
	illegal
	addiu	r0,r5,0000DBE0

;; update_interval: 00401C0E
update_interval proc
	lwpc	r6,004544F4
	beqzc	r6,00401C52
	li	r5,00000008
	div	r7,r6,r5
	lwpc	r6,004544F8
	addiu	r5,r0,000003E8
	addu	r7,r7,r6
	addiu	r7,r7,000001F4
	div	r6,r7,r5
	lwpc	r7,00454514
	illegal
	addiu	r0,r2,00009B92
	addiu	r7,r0,000000C7
	bltc	r7,r6,00401C50
	addiu	r7,r0,000000C8
	illegal
	addiu	r0,r2,0000DBE0
	addiu	r6,r0,000003E8
	lwpc	r7,0043008C
	mul	r7,r7,r6
	bc	00401C1C

;; write_stdout: 00401C60
write_stdout proc
	save	00000010,ra,00000004
	move	r16,r0
	movep	r18,r17,r4,r5
	subu	r6,r17,r16
	addu	r5,r18,r16
	li	r4,00000001
	balc	0040B080
00401C70 00 B2 30 AA F1 FF 04 A8 ED BF 14 1F             ..0.........   

;; set_signal: 00401C7C
set_signal proc
	save	000000A0,ra,00000003
	addiu	r6,r0,0000008C
	movep	r16,r17,r4,r5
	move	r5,r0
	addiu	r4,sp,00000004
	balc	0040A690
00401C8C                                     C0 10 C1 72             ...r
00401C90 21 B6 00 0A 74 63 A3 1F                         !...tc..       

;; sigexit: 00401C98
sigexit proc
	save	00000010,ra,00000001
	li	r7,00000001
	illegal
	addiu	r0,r5,000060EB
	illegal
	addiu	r0,r2,0000BB82
	restore.jrc	00000010,ra,00000001
	move	r5,r0
	addiupc	r4,00029347
	balc	00407E60

;; limit_capabilities: 00401CB6
limit_capabilities proc
	save	00000010,ra,00000001
	balc	00401E82
00401CBA                               8F 60 54 28 05 00           .`T(..
00401CC0 00 2A DC 92 8F 60 EA 27 05 00 8B 60 44 28 05 00 .*...`.'...`D(..
00401CD0 00 2A 1C 93 02 BA 11 1F 80 04 9C F5 9C 39 7F D2 .*...........9..
00401CE0 9C 39                                           .9             

;; modify_capability: 00401CE2
;;   Called from:
;;     00400CD8 (in fn00400CBE)
modify_capability proc
	save	00000010,ra,00000001
	lwpc	r7,004544B4
	bnezc	r4,00401CF0
	balc	00401E82
	move	r7,r4
	move.balc	r4,r7,0040AFF0
	bnezc	r4,00401CF8
	restore.jrc	00000010,ra,00000001
	addiupc	r4,00007AC6
	balc	00401E7A
	li	r4,FFFFFFFF
	bc	00401CF6

;; drop_capabilities: 00401D02
drop_capabilities proc
	save	00000010,ra,00000001
	balc	00401E82
00401D06                   00 2A F6 92 02 BA 11 1F 80 04       .*........
00401D10 66 F5 66 39 7F D2 66 39                         f.f9..f9       

;; fill: 00401D18
fill proc
	save	00000090,ra,00000006
	movep	r20,r18,r4,r5
	move	r19,r6
	move	r16,r20
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
	addiupc	r5,00007AB0
	addiu	r7,sp,00000034
	move.balc	r4,r20,004088A0
	move	r6,r0
	move	r16,r4
	bltc	r0,r4,00401DC0

l00401D72:
	lwpc	r7,004544EC
	bbnezc	r7,00000004,00401D90
	addiupc	r4,00007ABC
	move	r17,r18
	balc	0040212A
	subu	r7,r17,r18
	bltc	r7,r16,00401DCC
	li	r4,0000000A
	balc	00408770
	restore.jrc	00000090,ra,00000006

l00401D92:
	balc	00404850
00401D96                   10 BA AB 60 52 11 01 00 80 04       ...`R.....
00401DA0 EE F4 82 3B 02 D2 D6 38 09 92 75 1B E0 10 7A B3 ...;...8..u...z.
00401DB0 F9 50 AA B2 E9 90 85 84 08 10 07 AA F1 3F 6C B0 .P...........?l.

l00401DC0:
	addiu	r7,r19,FFFFFFF8
	subu	r7,r7,r16
	bgeuc	r7,r6,00401DAC
	bc	00401D72
	lbu	r5,0008(r17)
	addiupc	r4,00007A98
	addiu	r17,r17,00000001
	balc	0040212A
	bc	00401D84

;; __schedule_exit: 00401DDA
__schedule_exit proc
	save	00000020,ra,00000002
	lwpc	r7,00454548
	move	r16,r4
	bnezc	r7,00401E4A
	lwpc	r7,004544DC
	beqzc	r7,00401E4E
	addiu	r5,r0,000003E8
	lwpc	r7,0043008C
	mul	r7,r7,r5
	lwpc	r6,0045450C
	sll	r6,r6,00000001
	illegal
	addiu	r0,r5,000088E6
	illegal
	illegal
	lwpc	r7,00454548
	addiu	r5,r0,000003E8
	illegal
	bltc	r16,r0,00401E28
	bgeuc	r16,r6,00401E2A
	move	r16,r6
	li	r6,000F4240
	move	r4,r0
	illegal
	sw	r5,0008(sp)
	modu	r5,r7,r6
	move	r6,r0
	sw	r5,000C(sp)
	move	r5,sp
	sw	r0,0000(sp)
	sw	r0,0004(sp)
	balc	00407F10
	move	r4,r16
	restore.jrc	00000020,ra,00000002
	addiu	r6,r0,000003E8
	lwpc	r7,00430084
	mul	r7,r7,r6
	bc	00401E0C

;; print_timestamp: 00401E5C
;;   Called from:
;;     004033EA (in fn004033EA)
print_timestamp proc
	save	00000020,ra,00000001
	lwpc	r7,004544EC
	bbeqzc	r7,00000013,00401E78
	move	r5,r0
	addiu	r4,sp,00000008
	balc	004021B0
	addiupc	r4,00007A4D
	lwm	r5,0008(sp),00000002
	balc	0040212A
	restore.jrc	00000020,ra,00000001
	bc	00408630
	bc	0040015A

;; fn00401E82: 00401E82
;;   Called from:
;;     00401CB8 (in limit_capabilities)
;;     00401D04 (in drop_capabilities)
fn00401E82 proc
	bc	0040AFC0

;; pinger: 00401E86
pinger proc
	save	00000030,ra,00000005
	lwpc	r7,004544B8
	movep	r16,r17,r4,r5
	addiu	r4,r0,000003E8
	bnec	r0,r7,004020FC
	lwpc	r7,004544D8
	beqzc	r7,00401EB4
	lwpc	r6,004544E4
	bltc	r6,r7,00401EB4
	lwpc	r7,004314C8
	beqc	r0,r7,004020FC
	aluipc	r19,00000401
	move	r5,r0
	lw	r7,0490(r19)
	move	r18,r19
	bnec	r0,r7,00401F8C
	addiupc	r4,000292E4
	balc	004021B0
	lwpc	r7,00430088
	addiu	r7,r7,FFFFFFFF
	lwpc	r6,0043008C
	mul	r7,r7,r6
	illegal
	addiu	r0,r5,000060EB
	illegal
	bbeqzc	r7,00000014,00401F2A
	lwpc	r6,004544E4
	bgec	r0,r6,00401F2A
	ext	r5,r6,00000005,0000000B
	li	r7,00000001
	sllv	r7,r7,r6
	addiupc	r6,0002821F
	lwxs	r6,r5(r6)
	and	r7,r7,r6
	bnezc	r7,00401F2A
	balc	00401E5C
	lwpc	r5,004544E4
	lui	r7,00000010
	addiupc	r4,00007A02
	illegal
	move.balc	r5,r6,004086C0
	lwpc	r4,00412EF4
	balc	004081A0
	lw	r7,0000(r16)
	li	r6,0001F400
	addiupc	r5,00031008
	move	r4,r17
	jalrc	ra,r7
	beqc	r0,r4,00402002
	bltc	r0,r4,00402054
	balc	004021AC
	lw	r7,0000(r4)
	beqic	r7,00000029,00402058
	balc	004021AC
	lw	r7,0000(r4)
	beqic	r7,0000000C,00402058
	balc	004021AC
	lw	r7,0000(r4)
	beqic	r7,0000000B,00402106
	lw	r7,0004(r16)
	move	r4,r17
	jalrc	ra,r7
	move	r18,r4
	bltc	r0,r4,004020C4
	bnezc	r4,00401F84
	lwpc	r7,00430080
	beqzc	r7,00401F84
	balc	004021AC
	lw	r7,0000(r4)
	bneiuc	r7,00000016,00401F84
	illegal
	addiu	r0,r2,00003A2A
	sw	r0,0000(sp)
	balc	004021AC
	lw	r7,0000(r4)
	beqzc	r7,00401F2A
	bc	004020C6
	addiu	r4,sp,00000008
	balc	004021B0
	lw	r8,0490(r19)
	lw	r17,0008(sp)
	addiu	r5,r0,000003E8
	addiupc	r7,00029279
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
	bgeic	r8,0000000A,00401FD4
	balc	00401BB0
	bltc	r4,r9,00401FD4
	li	r4,0000000A
	subu	r4,r4,r8
	restore.jrc	00000030,ra,00000005
	mul	r9,r9,r5
	lwpc	r4,00454544
	addu	r4,r4,r8
	slt	r8,r4,r9
	movz	r4,r9,r8
	bgec	r4,r5,00401FEE
	subu	r4,r5,r4
	restore.jrc	00000030,ra,00000005
	lw	r17,0008(sp)
	subu	r4,r4,r5
	illegal
	addiu	r0,r5,000084F2
	sw	r17,0040(sp)
	lw	r17,000C(sp)
	sw	r7,0001(r10)
	bc	00401EE0
	illegal
	addiu	r0,r5,00002BFF
	illegal
	lwpc	r7,004544EC
	andi	r7,r7,00000011
	bneiuc	r7,00000001,00402044
	lwpc	r5,00430070
	lwpc	r7,00430088
	bgec	r7,r5,00402034
	lwpc	r7,00430078
	bltc	r7,r5,0040203C
	balc	00401BB0
	bgec	r4,r5,00402044
	li	r5,00000001
	addiupc	r4,0000797D
	balc	00401C60
	lwpc	r7,00454544
	lwpc	r4,0043008C
	subu	r4,r4,r7
	restore.jrc	00000030,ra,00000005
	balc	00404A00
	lwpc	r7,004544F4
	li	r6,00061A7F
	illegal
	addiu	r0,r5,000088E6
	ori	r4,r16,000000C0
	illegal
	illegal
	addu	r7,r7,r6
	illegal
	addiu	r0,r5,000060EB
	illegal
	bbeqzc	r7,0000000E,0040208E
	balc	00401C0E
	lwpc	r7,0043008C
	li	r4,0000000A
	bltic	r7,00000016,004020AA
	sra	r4,r7,00000001
	addiu	r7,r0,000001F4
	slti	r6,r4,000001F5
	movz	r4,r7,r6
	lwpc	r7,00454540
	addiu	r7,r7,00000001
	lwpc	r6,00430084
	illegal
	addiu	r0,r5,00003CEC
	bltc	r7,r6,004020FC
	move	r18,r0
	balc	00401BD8
	bnezc	r18,004020E4
	lwpc	r7,004544EC
	bbnezc	r7,00000004,004020E4
	bbeqzc	r7,00000000,0040211E
	li	r5,00000001
	addiupc	r4,0000784C
	balc	00401C60
	lwpc	r7,0043008C
	li	r6,0000000A
	slti	r4,r7,0000000A
	illegal
	addiu	r0,r5,00002087
	sll	r4,r17,00000008
	move	r4,r6
	restore.jrc	00000030,ra,00000005
	li	r5,00000008
	div	r6,r7,r5
	bc	00402072
	lwpc	r7,00454544
	li	r4,0000000A
	lwpc	r6,0043008C
	addu	r7,r7,r6
	illegal
	addiu	r0,r5,00001F35
	addiupc	r4,0000790F
	balc	00401E7A
	bc	004020E4
	bc	004083F0
	bc	004086C0

;; sock_setbufs: 0040212E
sock_setbufs proc
	save	00000020,ra,00000003
	li	r7,00000004
	sw	r7,000C(sp)
	lwpc	r7,004544FC
	movep	r17,r16,r4,r5
	bnezc	r7,00402144
	illegal
	addiu	r0,r5,00001610
	addiupc	r7,000291D9
	addiu	r8,r0,00000004
	addiu	r6,r0,00001001
	addiu	r5,r0,0000FFFF
	balc	004023EE
	lwpc	r7,00430088
	mul	r16,r16,r7
	addiu	r7,r0,0000FFFF
	bgec	r7,r16,004021A6
	sw	r16,0008(sp)
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
	lw	r17,0008(sp)
	bgec	r7,r16,004021A4
	lwpc	r5,00412EF0
	addiupc	r4,000078D7
	balc	00402126
	restore.jrc	00000020,ra,00000003
	addiu	r7,r7,00000001
	sw	r7,0008(sp)
	bc	0040216A
	bc	004049B0
	bc	0040AF40

;; setup: 004021B4
setup proc
	save	000000B0,ra,00000003
	lwpc	r7,004544EC
	move	r16,r4
	andi	r6,r7,00000003
	bneiuc	r6,00000001,004021CA
	illegal
	addiu	r0,r2,000060AB
	illegal
	lwpc	r6,0043008C
	beqzc	r5,004021F4
	addiu	r5,r0,000000C7
	bltc	r5,r6,004021F4
	addiu	r6,r0,000000C8
	addiupc	r5,000078D2
	lwpc	r4,00412EF0
	balc	004023F2
	li	r4,00000002
	balc	00401E7E
	lwpc	r4,00430088
	li	r17,7FFFFFFF
	div	r5,r17,r4
	bltc	r6,r5,00402216
	lwpc	r5,00412EF0
	addiupc	r4,000078DD
	balc	00402126
	bc	004021F0
	li	r6,00000001
	sw	r6,0004(sp)
	bbeqzc	r7,00000006,0040222E
	addiu	r8,r0,00000004
	lw	r4,0000(r16)
	addu	r7,sp,r8
	addiu	r5,r0,0000FFFF
	balc	004023EE
	lwpc	r7,004544EC
	bbeqzc	r7,00000007,0040224A
	addiu	r8,r0,00000004
	lw	r4,0000(r16)
	addu	r7,sp,r8
	li	r6,00000010
	addiu	r5,r0,0000FFFF
	balc	004023EE
	lwpc	r7,004544EC
	bbnezc	r7,0000000C,00402276
	li	r7,00000001
	lw	r4,0000(r16)
	sw	r7,0020(sp)
	addiu	r8,r0,00000004
	li	r6,0000001D
	addiu	r5,r0,0000FFFF
	addiu	r7,sp,00000020
	balc	004023EE
	beqzc	r4,00402276
	lwpc	r5,00412EF0
	addiupc	r4,000078C0
	balc	00402126
	lwpc	r7,004544EC
	bbeqzc	r7,00000012,004022B6
	li	r4,00000001
	balc	00401CE2
	lw	r4,0000(r16)
	addiupc	r7,0002911E
	addiu	r8,r0,00000004
	li	r6,00000024
	addiu	r5,r0,0000FFFF
	balc	004023EE
	move	r17,r4
	move	r4,r0
	balc	00401CE2
	li	r7,FFFFFFFF
	bnec	r17,r7,004022B6
	lwpc	r6,004544C8
	addiupc	r5,000078C3
	lwpc	r4,00412EF0
	balc	004023F2
	li	r7,00000001
	lwpc	r6,0043008C
	sw	r7,0008(sp)
	addiu	r7,r0,000003E7
	sw	r0,000C(sp)
	bltc	r7,r6,004022E0
	slti	r7,r6,0000000A
	li	r5,0000000A
	movz	r5,r6,r7
	addiu	r6,r0,000003E8
	mul	r7,r5,r6
	sw	r0,0008(sp)
	sw	r7,000C(sp)
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
	addiu	r8,r0,00000008
	move	r7,r5
	addiu	r5,r0,000003E8
	div	r6,r7,r5
	sw	r6,0008(sp)
	illegal
	mul	r7,r6,r5
	lw	r4,0000(r16)
	addiu	r6,r0,00001006
	addiu	r5,r0,0000FFFF
	sw	r7,000C(sp)
	addu	r7,sp,r8
	balc	004023EE
	beqzc	r4,00402340
	lwpc	r7,004544EC
	ori	r7,r7,00000800
	illegal
	addiu	r0,r5,000060EB
	illegal
	andi	r7,r7,00000008
	lwpc	r5,00430074
	beqc	r0,r7,004023E8
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00402368
	balc	0040AFB0
	andi	r4,r4,0000FFFF
	balc	00406700
	illegal
	addiu	r0,r5,000004BF
	illegal
	li	r4,00000002
	balc	0040271C
	addiupc	r5,FFFFFC92
	li	r4,0000000E
	balc	0040271C
	addiupc	r5,FFFFFC44
	li	r4,00000003
	balc	0040271C
	addiu	r4,sp,00000020
	balc	00408030
	move	r6,r0
	addiu	r5,sp,00000020
	li	r4,00000002
	balc	00408040
	move	r5,r0
	addiupc	r4,00029081
	balc	004021B0
	lwpc	r7,004314C8
	beqzc	r7,004023B2
	move	r6,r0
	addiu	r5,sp,00000010
	move	r4,r0
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r7,0018(sp)
	sw	r0,001C(sp)
	balc	00407F10
	li	r4,00000001
	balc	0040AFD0
	beqzc	r4,004023D8
	addiu	r6,sp,00000010
	li	r5,40087468
	li	r4,00000001
	balc	00405B80
	li	r7,FFFFFFFF
	beqc	r4,r7,004023D8
	lhu	r7,0012(sp)
	beqzc	r7,004023D8
	illegal
	addiu	r0,r2,00001FB3
	addiupc	r6,00030B60
	addu	r6,r7,r6
	sb	r7,0008(r6)
	addiu	r7,r7,00000001
	bltc	r7,r5,004023DA
	bc	00402352
	bc	00407D60
	bc	00408340

;; gather_statistics: 004023F6
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
	illegal
	addiu	r0,r5,0000A920
	addiu	r2,r2,000060EB
	illegal
	subu	r5,r7,r16
	andi	r6,r5,0000FFFF
	bbnezc	r5,0000000F,0040245C
	lwpc	r5,00430078
	bltc	r6,r5,0040243A
	addiu	r6,r6,00000001
	illegal
	addiu	r0,r2,0000E0C5
	illegal
	illegal
	seh	r5,r5
	bltc	r0,r5,00402458
	andi	r7,r7,0000FFFF
	addiu	r5,r0,00007FFF
	subu	r7,r7,r4
	bgec	r5,r7,0040245C
	sh	r16,0518(r6)
	lwpc	r7,00454508
	move	r22,r0
	beqc	r0,r7,004025DC
	bltiuc	r19,00000010,004025DC
	li	r6,00000008
	addu	r4,sp,r6
	move.balc	r5,r20,0040A130
	bc	004024C2
	lw	r8,0000(r17)
	lw	r17,0008(sp)
	subu	r8,r8,r7
	li	r7,000F4240
	sw	r8,0000(r17)
	mul	r8,r8,r7
	lw	r7,0004(r17)
	addu	r22,r8,r7
	bgec	r22,r0,004024E0
	move	r6,r22
	addiupc	r5,000077DD
	lwpc	r4,00412EF0
	balc	004023F2
	lwpc	r7,004544EC
	bbnezc	r7,0000000C,004024DE
	movep	r4,r5,r17,r0
	balc	004021B0
	lwpc	r7,004544EC
	addiu	r6,r0,00001000
	or	r7,r7,r6
	illegal
	addiu	r0,r5,00001791
	lw	r17,000C(sp)
	subu	r7,r7,r6
	sw	r7,0044(sp)
	bgec	r7,r0,00402478
	lw	r6,0000(r17)
	illegal
	addiu	r0,r15,000090DF
	swm	r6,0000(r17),00000002
	bc	00402478
	move	r22,r0
	bnec	r0,r18,004025DE
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
	illegal
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
	illegal
	addiu	r0,r2,000060EB
	restore.jrc	000000C0,ra,00000008
	addiu	r0,r5,00008AC7
	illegal
	restore.jrc	000000B0,ra,0000000E
	addiu	r0,r5,000060CB
	restore.jrc	000000A0,ra,00000000
	addiu	r0,r5,000080F6
	illegal
	li	r5,00000008
	div	r7,r6,r5
	subu	r7,r22,r7
	addu	r7,r7,r6
	illegal
	addiu	r0,r5,000060EB
	restore.jrc	00000070,ra,0000000A
	addiu	r0,r5,0000C8E4
	illegal
	balc	00401C0E
	srl	r7,r16,00000005
	addiupc	r6,00027EE0
	lsa	r7,r7,r6,00000002
	li	r17,00000001
	sllv	r16,r17,r16
	lw	r6,0000(r7)
	and	r5,r6,r16
	beqzc	r5,004025FE
	lwpc	r7,004544E0
	addu	r7,r7,r17
	illegal
	addiu	r0,r5,000060EB
	restore.jrc	00000030,ra,00000006
	addiu	r0,r5,000090FF
	illegal
	addiu	r0,r5,000060EB
	beqc	r4,r5,004025CA
	addiu	r0,r2,0000D001
	illegal
	addiu	r0,r2,000060EB
	restore.jrc	00000020,ra,0000000A
	addiu	r0,r5,0000C8F4
	illegal
	beqzc	r16,0040260C
	bnezc	r18,00402604
	li	r5,00000003
	addiupc	r4,00007763
	balc	00401C60
	move	r16,r0
	move	r4,r16
	restore.jrc	00000040,r30,0000000A
	beqzc	r18,0040257A
	lwpc	r7,004544CC
	addiu	r7,r7,00000001
	illegal
	addiu	r0,r5,000060EB
	save	000000E0,ra,0000000A
	addiu	r0,r5,000090FF
	illegal
	addiu	r0,r5,00001220
	bc	004025AE
	or	r16,r16,r6
	sw	r16,0000(r7)
	bc	004025FA
	li	r5,00000002
	addiupc	r4,00007749
	bc	004025D2
	balc	00401E5C
	move	r6,r30
	addiupc	r4,00007745
	move.balc	r5,r19,004086C0
	lw	r17,0040(sp)
	beqzc	r7,00402622
	movep	r4,r5,r23,r19
	jalrc	ra,r7
	bltc	r21,r0,0040262E
	addiupc	r4,00007745
	move.balc	r5,r21,004086C0
	lwpc	r7,00430074
	addiu	r7,r7,00000007
	bltc	r7,r19,00402646
	addiupc	r4,0000773F
	li	r16,00000001
	balc	00408780
	bc	004025D8
	lwpc	r7,00454508
	beqzc	r7,00402668
	li	r7,0001869F
	bgec	r7,r22,004026CA
	addiu	r5,r0,000003E8
	addiupc	r4,00007736
	div	r7,r22,r5
	move.balc	r5,r7,004086C0
	beqzc	r17,00402670
	addiupc	r4,00007755
	balc	004029C6
	beqzc	r18,00402678
	addiupc	r4,00007755
	balc	004029C6
	lwpc	r4,00430074
	li	r5,00000008
	bgec	r5,r4,004025D8
	addiupc	r6,000308B6
	lbux	r7,r5(r20)
	addu	r6,r5,r6
	lbu	r6,0008(r6)
	beqc	r6,r7,00402718
	addiupc	r4,0000774C
	li	r17,00000008
	balc	004029C6
	lwpc	r7,00430074
	bgec	r17,r7,004025D8
	li	r6,00000020
	illegal
	bneiuc	r7,00000008,004026BC
	addiupc	r4,00007758
	move.balc	r5,r17,004086C0
	lbux	r5,r17(r20)
	addiupc	r4,00007756
	addiu	r17,r17,00000001
	balc	004029C6
	bc	004026A0
	addiu	r7,r0,0000270F
	bgec	r7,r22,004026EE
	addiu	r5,r0,000003E8
	li	r7,00000064
	illegal
	div	r4,r6,r7
	div	r7,r22,r5
	movep	r5,r6,r7,r4
	addiupc	r4,000076F9
	balc	004029C6
	bc	00402668
	addiu	r7,r0,000003E7
	bgec	r7,r22,00402710
	addiu	r5,r0,000003E8
	li	r7,0000000A
	illegal
	div	r4,r6,r7
	div	r7,r22,r5
	movep	r5,r6,r7,r4
	addiupc	r4,000076F1
	bc	004026EA
	movep	r5,r6,r0,r22
	addiupc	r4,000076F7
	bc	004026EA
	addiu	r5,r5,00000001
	bc	00402680
	bc	00401C7C

;; finish: 00402720
finish proc
	save	00000050,r30,0000000A
	aluipc	r7,00000402
	lw	r16,0490(r7)
	addiupc	r7,00028EB1
	lw	r20,0001(r7)
	addiupc	r7,00028EB2
	lw	r4,0004(r7)
	subu	r20,r20,r4
	bgec	r20,r0,00402746

l0040273E:
	addiu	r16,r16,FFFFFFFF
	illegal
	addiu	r0,r15,0000E0E5

l00402746:
	aluipc	r7,00000402
	li	r4,0000000A
	lw	r5,0498(r7)
	subu	r16,r16,r5
	balc	00402A96
00402754             8B 60 9A 07 01 00 00 2A 42 5A AB 60     .`.....*BZ.`
00402760 5C 1D 05 00 80 04 10 EE 5C 3A AB 60 74 1D 05 00 \.......\:.`t...
00402770 80 04 20 EE 50 3A AB 60 60 1D 05 00 80 04 30 EE .. .P:.``.....0.
00402780 44 3A AB 60 58 1D 05 00 86 9A 80 04 32 EE 36 3A D:.`X.......2.6:
00402790 AB 60 36 1D 05 00 86 9A 80 04 38 EE 28 3A AB 60 .`6.......8.(:.`
004027A0 2C 1D 05 00 86 9A 80 04 3E EE 1A 3A CB 60 32 1D ,.......>..:.`2.
004027B0 05 00 32 9B 8B 60 22 1D 05 00 E4 D0 69 B2 E6 80 ..2..`".....i...
004027C0 9F C0 24 22 58 28 99 3C D0 3A A4 10 80 04 28 EE ..$"X(.<.:....(.
004027D0 F4 39 E0 00 E8 03 F0 20 18 28 F4 20 18 31 80 04 .9..... .(. .1..
004027E0 2A EE 5A B3 E0 39 0A D2 40 06 8C DC A8 3A 2B 62 *.Z..9..@....:+b
004027F0 E8 1C 05 00 20 8A 10 01 EB 60 0A 1D 05 00 E0 88 .... ....`......
00402800 06 01 EB 60 D8 1C 05 00 45 E2 02 20 92 B3 92 84 ...`....E.. ....
00402810 80 84 B1 82 9F C0 B2 84 84 84 B9 BE 7C 3A D3 FE ............|:..
00402820 72 86 80 94 D2 86 84 94 45 E2 02 20 92 84 88 84 r.......E.. ....
00402830 B2 84 8C 84 B9 BE 62 3A 76 22 18 88 73 22 18 A8 ......b:v"..s"..
00402840 73 22 D8 38 92 84 88 94 B2 84 8C 94 A4 22 D0 A9 s".8........."..
00402850 F1 20 0F 8A A4 22 90 23 DB B0 53 B2 20 AA 08 80 . ...".#..S. ...
00402860 20 AA 5E 01 A0 8A 5A 01 55 12 F1 12 7F D2 E0 60  .^...Z.U......`
00402870 FF FF FF 7F 2E 18 FA BE 35 BE 1E 3A 4C B1 B7 3C ........5..:L..<
00402880 86 20 90 23 CA B2 92 10 E5 80 5F C0 7C B3 E6 20 . .#......_.|.. 
00402890 90 3B 69 33 FA B2 F7 10 C5 83 1F C0 E5 82 81 C0 .;i3............
004028A0 DE 20 90 92 F7 A8 CF BF E7 AA 04 00 92 A8 C7 FF . ..............
004028B0 20 02 E8 03 EB 60 52 1C 05 00 27 22 58 51 27 22  ....`R...'"XQ'"
004028C0 18 F1 C0 00 E8 03 E0 10 D3 BE 32 22 18 59 5D A5 ..........2".Y].
004028D0 18 2C 00 2A 5A 16 E4 12 C0 00 E8 03 E0 10 AB 62 .,.*Z..........b
004028E0 98 D7 02 00 D3 BE B2 39 32 22 58 39 3E 11 E0 B4 .......92"X9>...
004028F0 EC BF 80 04 26 ED 35 22 58 31 35 22 18 29 40 06 ....&.5"X15".)@.
00402900 72 EC 5D A5 18 24 BE 38 CB 60 6A D7 02 00 D8 C8 r.]..$.8.`j.....
00402910 0C 10 80 04 4A ED 40 0B A6 5D 40 06 56 EC EB 60 ....J.@..]@.V..`
00402920 B8 1B 05 00 F8 9B EB 60 60 D7 02 00 8E 9B C0 00 .......``.......
00402930 01 40 EB 60 B4 1B 05 00 E8 53 E2 9B CB 60 A2 1B .@.`.....S...`..
00402940 05 00 D8 C8 58 10 E0 60 40 42 0F 00 DF 90 F0 20 ....X..`@B..... 
00402950 18 28 F0 20 58 80 F4 80 9F C0 85 22 50 21 A4 20 .(. X......"P!. 
00402960 90 2B 80 B3 5A B0 E6 80 9F C0 2E 39 EB 60 82 1B .+..Z......9.`..
00402970 05 00 08 D3 00 01 40 1F C7 20 18 49 C0 00 E8 03 ......@.. .I....
00402980 C9 20 58 29 25 11 07 21 18 29 C4 20 58 39 05 11 . X)%..!.). X9..
00402990 C4 20 18 29 80 04 D4 EC C5 10 40 0B 22 5D 0A D2 . .)......@."]..
004029A0 F4 38 EB 60 34 1B 05 00 01 D2 92 9B 8B 60 16 EB .8.`4........`..
004029B0 02 00 0A 9A 8B 60 1E 1B 05 00 87 20 50 23 FF 2B .....`..... P#.+
004029C0 99 D7 55 12 EB 1A                               ..U...         

;; fn004029C6: 004029C6
fn004029C6 proc
	bc	004086C0

;; status: 004029CA
status proc
	save	00000030,ra,00000003
	lwpc	r16,004544E4
	move	r4,r0
	illegal
	addiu	r0,r5,0000622B
	bc	00402CDA
	addiu	r0,r5,00009814
	subu	r8,r16,r17
	li	r4,00000064
	illegal
	move	r6,r16
	mul	r4,r4,r8
	sra	r7,r16,0000001F
	balc	00402A9A
	lwpc	r7,00412EF0
	move	r8,r4
	sw	r7,001C(sp)
	movep	r6,r7,r17,r16
	lw	r17,001C(sp)
	addiupc	r5,00007642
	balc	00408340
	lwpc	r6,004544DC
	beqzc	r6,00402A8A
	lwpc	r7,00454508
	beqzc	r7,00402A8A
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
	illegal
	illegal
	div	r8,r4,r6
	sw	r17,0004(sp)
	div	r4,r5,r6
	div	r17,r16,r6
	illegal
	move	r11,r16
	div	r16,r7,r10
	illegal
	move	r6,r4
	lw	r17,001C(sp)
	move	r10,r16
	addiupc	r5,00007612
	sw	r17,0000(sp)
	balc	00408340
	lw	r17,001C(sp)
	li	r4,0000000A
	restore	00000030,ra,00000003
	bc	00408380

;; fn00402A96: 00402A96
;;   Called from:
;;     00402752 (in finish)
fn00402A96 proc
	bc	00408770

;; fn00402A9A: 00402A9A
fn00402A9A proc
	bc	00403CB0

;; main_loop: 00402A9E
main_loop proc
	save	00000020,ra,00000007
	illegal
	movep	r23,r23,r7,r8
	movep	r17,r16,r4,r5
	move	r18,r7
	sw	r6,0004(sp)
	lwpc	r7,004544B8
	bnec	r0,r7,00402C92
	lwpc	r6,004544D8
	beqzc	r6,00402AD0
	lwpc	r5,004544D0
	lwpc	r7,004544DC
	addu	r7,r7,r5
	bgec	r7,r6,00402C92
	lwpc	r7,004314C8
	beqzc	r7,00402AE2
	lwpc	r7,004544D0
	bnec	r0,r7,00402C92
	lwpc	r7,00454500
	beqzc	r7,00402AEC
	balc	004029CA
	movep	r4,r5,r17,r16
	balc	00401E86
	lwpc	r7,004544D8
	move	r20,r4
	beqzc	r7,00402B14
	lwpc	r6,004544E4
	bltc	r6,r7,00402B14
	lwpc	r7,004314C8
	bnezc	r7,00402B14
	balc	00401DDA
	move	r20,r4
	bgec	r0,r20,00402AEC
	addiu	r6,r0,00004800
	lwpc	r7,004544EC
	and	r7,r7,r6
	bnezc	r7,00402BA4
	lwpc	r6,0043008C
	li	r5,0000000A
	slti	r7,r6,0000000A
	movz	r5,r6,r7
	bltc	r20,r5,00402BA4
	move	r6,r0
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
	balc	00402CEA
	lw	r7,0000(r4)
	beqic	r7,0000000B,00402AAC
	balc	00402CEA
	lw	r7,0000(r4)
	beqic	r7,00000004,00402AAC
	lw	r7,0004(r17)
	move	r4,r16
	jalrc	ra,r7
	bnec	r0,r4,00402C74
	balc	00402CEA
	lw	r7,0000(r4)
	bnezc	r7,00402BFC
	lw	r7,0004(r16)
	bneiuc	r7,00000003,00402C74
	lw	r7,000C(r17)
	move	r4,r16
	jalrc	ra,r7
	bc	00402C74
	balc	00401BB0
	addiu	r21,r0,000003E8
	move	r19,r4
	li	r4,00000002
	balc	00402CEE
	illegal
	bnezc	r7,00402BDC
	li	r4,00000002
	balc	00402CEE
	div	r7,r21,r4
	slt	r4,r7,r20
	xori	r4,r4,00000001
	beqc	r0,r4,00402C96
	addiu	r20,r0,0000000A
	bnec	r0,r19,00402C96
	balc	00407E10
	li	r6,00000040
	bc	00402B3C
	li	r4,00000002
	balc	00402CEE
	li	r6,7FFFFFFF
	div	r7,r6,r4
	move	r4,r0
	bgec	r20,r7,00402BC8
	li	r4,00000002
	balc	00402CEE
	mul	r4,r4,r20
	slti	r4,r4,000003E9
	bc	00402BC8
	addiupc	r4,00007574
	balc	00408630
	bc	00402AAC
	lw	r17,0028(sp)
	move	r8,r0
	bltiuc	r4,0000000C,00402C4E
	lw	r5,0024(sp)
	move	r7,r20
	beqzc	r7,00402C4E
	lw	r21,0001(r7)
	addiu	r5,r0,0000FFFF
	lw	r6,0000(r7)
	bnec	r21,r5,00402C46
	lw	r5,0008(r7)
	bneiuc	r5,0000001D,00402C46
	bltiuc	r6,00000014,00402C46
	addiu	r8,r7,0000000C
	addiu	r6,r6,00000003
	addu	r5,r20,r4
	ins	r6,r0,00000000,00000001
	subu	r5,r5,r7
	addiu	r21,r6,0000000C
	addu	r7,r7,r6
	bltuc	r21,r5,00402C12
	bc	00402C4A
	bgeiuc	r6,0000000C,00402C2E
	move	r7,r0
	bc	00402C12
	lwpc	r7,004544EC
	bbeqzc	r7,0000000C,00402C7E
	move	r5,r0
	addiu	r4,sp,0000000C
	balc	0040AF40
	addiu	r7,sp,0000000C
	move	r8,r7
	lw	r20,0002(r17)
	addiu	r7,sp,00000030
	move	r6,r19
	addiu	r5,sp,00000014
	move	r4,r16
	jalrc	ra,r20
	bnec	r0,r4,00402B96
	balc	00401BB0
	bnec	r0,r4,00402BD8
	bc	00402AAC
	bnec	r0,r8,00402C64
	lw	r4,0000(r16)
	addiu	r6,sp,0000000C
	addiu	r5,r0,00008906
	balc	00405B80
	bnezc	r4,00402C58
	bc	00402C60
	balc	00402720
	addiu	r6,r0,00004800
	lwpc	r7,004544EC
	and	r7,r7,r6
	bnezc	r7,00402CAE
	lwpc	r7,0043008C
	beqc	r0,r7,00402B3A
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
	lhu	r7,0012(sp)
	andi	r7,r7,00000009
	beqc	r0,r7,00402AAC
	bc	00402BD8

;; is_ours: 00402CD6
;;   Called from:
;;     004019AE (in ping4_parse_reply)
;;     00401A9E (in ping4_parse_reply)
;;     004030D0 (in ping6_parse_reply)
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
00402CEA                               00 28 C2 1C 00 28           .(...(
00402CF0 3E 1A 08 90 00 80 00 C0 00 80 00 C0 00 80 00 C0 >...............

;; niquery_option_help_handler: 00402D00
;;   Called from:
;;     00403442 (in niquery_option_handler)
niquery_option_help_handler proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,000074F6
	balc	00403098
	li	r4,00000002
	balc	004030A0

;; niquery_option_subject_name_handler: 00402D12
niquery_option_subject_name_handler proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,0000755F
	balc	00403098
	li	r4,00000003
	balc	004030A0

;; niquery_set_qtype: 00402D24
;;   Called from:
;;     00402D4E (in niquery_option_ipv4_flag_handler)
;;     00402D7A (in niquery_option_ipv4_handler)
;;     00402D88 (in niquery_option_ipv6_flag_handler)
;;     00402DB4 (in niquery_option_ipv6_handler)
;;     00402DC0 (in niquery_option_name_handler)
niquery_set_qtype proc
	save	00000010,ra,00000001
	lwpc	r7,00430214
	bltc	r7,r0,00402D3E
	beqc	r4,r7,00402D3E
	addiupc	r4,0000756B
	balc	00408780
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000001
	illegal
	addiu	r0,r2,00001080
	restore.jrc	00000010,ra,00000001

;; niquery_option_ipv4_flag_handler: 00402D48
niquery_option_ipv4_flag_handler proc
	save	00000010,ra,00000002
	move	r16,r4
	li	r4,00000004
	balc	00402D24
00402D50 FF D3 04 A8 1C 80 14 D2 EB 60 7A E7 02 00 0C 3E .........`z....>
00402D60 82 04 6C D3 48 B0 43 17 EC 53 EF 60 68 E7 02 00 ..l.H.C..S.`h...
00402D70 E0 10 87 10 12 1F                               ......         

;; niquery_option_ipv4_handler: 00402D76
niquery_option_ipv4_handler proc
	save	00000010,ra,00000001
	li	r4,00000004
	balc	00402D24
00402D7C                                     84 80 9F C0             ....
00402D80 11 1F                                           ..             

;; niquery_option_ipv6_flag_handler: 00402D82
niquery_option_ipv6_flag_handler proc
	save	00000010,ra,00000002
	move	r16,r4
	li	r4,00000003
	balc	00402D24
00402D8A                               FF D3 04 A8 1C 80           ......
00402D90 14 D2 EB 60 40 E7 02 00 0C 3E 82 04 32 D3 48 B0 ...`@....>..2.H.
00402DA0 43 17 EC 53 EF 60 2E E7 02 00 E0 10 87 10 12 1F C..S.`..........

;; niquery_option_ipv6_handler: 00402DB0
niquery_option_ipv6_handler proc
	save	00000010,ra,00000001
	li	r4,00000003
	balc	00402D24
00402DB6                   84 80 9F C0 11 1F                   ......   

;; niquery_option_name_handler: 00402DBC
niquery_option_name_handler proc
	save	00000010,ra,00000001
	li	r4,00000002
	balc	00402D24
00402DC2       84 80 9F C0 11 1F                           ......       

;; pr_icmph: 00402DC8
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
	addiupc	r4,000075E7
	bc	00402E6E

l00402DEC:
	beqic	r4,00000003,00402E84

l00402DF0:
	bneiuc	r4,00000004,00402DE4

l00402DF4:
	addiupc	r4,00007590
	balc	0040309C
00402DFA                               80 04 2E EB 08 98           ......
00402E00 10 CA 9E 08 80 04 38 EB 92 3A B1 10 80 04 5C EB ......8..:....\.
00402E10 5C 18                                           \.             

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
	addiupc	r4,000075BB
	bc	00402E4A

l00402E34:
	addiupc	r4,000074F2
	balc	0040309C
00402E3A                               0C CA 2A 28 E0 04           ..*(..
00402E40 8E F4 8F 53 E0 D8 80 04 EE E9                   ...S......     

l00402E4A:
	balc	0040309C

l00402E4C:
	move	r4,r0
	restore.jrc	00000010,ra,00000003
00402E50 80 04 F0 E9 F5 1B 80 04 06 EA EF 1B 80 04 20 EA .............. .
00402E60 E9 1B 80 04 2E EA E3 1B B0 10 80 04 3A EA       ............:. 

l00402E6E:
	balc	0040309C
00402E70 DB 1B                                           ..             

l00402E72:
	addiupc	r4,00007521
	move.balc	r5,r17,004086C0
	beqzc	r16,00402E4C

l00402E7C:
	move	r5,r16
	addiupc	r4,00007527
	bc	00402E6E

l00402E84:
	addiupc	r4,0000752A
	balc	0040309C
00402E8A                               06 B8 80 04 5C EA           ....\.
00402E90 B9 1B 80 04 62 EA 00 CA B1 0F B0 10 80 04 70 EA ....b.........p.
00402EA0 CD 1B 80 04 AA EA 00 CA 5F 17 80 04 B2 EA 00 0B ........_.......
00402EB0 0E 58 57 1B                                     .XW.           

l00402EB4:
	addiupc	r4,0000755E
	bc	00402E4A

l00402EBA:
	addiupc	r4,00007563
	bc	00402E4A

l00402EC0:
	addiupc	r4,00007566
	bc	00402E4A

l00402EC6:
	addiupc	r4,00007569
	bc	00402E4A

;; pr_echo_reply: 00402ECC
pr_echo_reply proc
	save	00000010,ra,00000001
	illegal
	balc	004030A4
	restore	00000010,ra,00000001
	move	r5,r4
	addiupc	r4,00006AF0
	bc	004086C0

;; write_stdout: 00402EE0
write_stdout proc
	save	00000010,ra,00000004
	move	r16,r0
	movep	r18,r17,r4,r5
	subu	r6,r17,r16
	addu	r5,r18,r16
	li	r4,00000001
	balc	0040B080
00402EF0 00 B2 30 AA F1 FF 04 A8 ED BF 14 1F             ..0.........   

;; ping6_receive_error_msg: 00402EFC
ping6_receive_error_msg proc
	save	00000280,ra,00000005
	move	r17,r4
	balc	004032A0
00402F04             FD 80 A0 8D 07 12 C8 73 C0 15 C0 00     .......s....
00402F10 40 20 E6 B4 88 D3 E7 B4 D1 73 EA B4 9C D3 EB B4 @ .......s......
00402F20 C6 73 EC B4 81 D3 ED B4 D8 73 10 16 CA 72 EE B4 .s.......s...r..
00402F30 E0 00 00 02 10 B4 EF B4 00 2A 34 47 04 A8 B6 80 .........*4G....
00402F40 D0 84 DC 8D E0 10 DC C8 04 60 F0 84 D8 8D AE 34 .........`.....4
00402F50 40 12 5A B3 AA 9B 71 17 D1 C8 08 48 72 17 D0 C8 @.Z...q....Hr...
00402F60 02 C8 73 91 70 17 DC C8 12 60 CB 90 D1 B3 C0 80 ..s.p....`......
00402F70 40 E0 06 01 0C 00 7E B3 08 AA D9 FF E0 10 D5 1B @.....~.........
00402F80 04 B9 00 2A 7A 1A B2 84 04 20 B0 C8 5E 08 EB 60 ...*z.... ..^..`
00402F90 58 15 05 00 27 82 10 20 20 AA E4 00 E4 C8 26 00 X...'..  .....&.
00402FA0 80 04 D4 E1 3B 3B EB 60 24 15 05 00 01 D0 E9 90 ....;;.`$.......
00402FB0 EF 60 1A 15 05 00 E8 3A C0 95 84 B8 00 22 D0 89 .`.....:....."..
00402FC0 91 10 E5 83 83 32 20 16 EB 60 22 FF 00 00 E3 B4 .....2 ..`".....
00402FD0 82 C8 10 D0 00 2A 12 1A A0 04 A0 E1 C4 10 83 34 .....*.........4
00402FE0 1C 3B C3 1B A0 04 AC E1 22 17 F3 1B B0 C8 06 18 .;......".......
00402FF0 88 C8 06 40 60 12 00 12 88 18 10 D3 A2 04 00 F5 ...@`...........
00403000 53 72 00 2A FA 70 04 12 6B BA DD 84 20 20 E0 00 Sr.*.p..k...  ..
00403010 80 00 C7 A8 DF 3F BD 84 24 60 3F 0A B9 FC 55 9A .....?..$`?...U.
00403020 EB 60 AA 14 05 00 81 D0 E9 90 EF 60 A0 14 05 00 .`.........`....
00403030 EB 60 B6 14 05 00 F4 C8 7D 27 F8 50 8A 98 82 D2 .`......}'.P....
00403040 80 04 A4 E1 9B 3A 6F 1B A0 3B 9C D2 24 92 FF 2B .....:o..;..$..+
00403050 37 E2 04 12 9D 84 26 60 4A 38 90 BD 80 04 8C E1 7.....&`J8......
00403060 3A 38 B2 84 06 20 22 17 11 12 92 84 05 20 81 D0 :8... "...... ..
00403070 57 39 0A D2 78 3B 8B 60 78 FE 00 00 74 3B 37 1B W9..x;.`x...t;7.
00403080 05 12 20 12 31 1B                               .. .1.         

;; niquery_nonce.isra.0: 00403086
;;   Called from:
;;     00403174 (in ping6_parse_reply)
niquery_nonce.isra.0 proc
	save	00000010,ra,00000001
	lwpc	r5,00412EF0
	addiupc	r4,000073A5
	balc	00403098
	li	r4,00000003
	balc	004030A0
	bc	004083F0

;; fn0040309C: 0040309C
;;   Called from:
;;     00402DF8 (in pr_icmph)
;;     00402E38 (in pr_icmph)
;;     00402E4A (in pr_icmph)
;;     00402E6E (in pr_icmph)
;;     00402E88 (in pr_icmph)
fn0040309C proc
	bc	004086C0

;; fn004030A0: 004030A0
fn004030A0 proc
	bc	0040015A

;; fn004030A4: 004030A4
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
	illegal
	balc	004030A4
	li	r5,0000001C
	move	r17,r4
	lw	r4,001C(sp)
	move.balc	r4,r19,00401288
	addiupc	r7,FFFFFEF1
	move	r11,r4
	sw	r7,0000(sp)
	move	r10,r22
	move	r9,r0
	move	r8,r18
	movep	r6,r7,r21,r17
	li	r5,00000008
	move.balc	r4,r16,004023F6
	beqc	r0,r4,0040324A
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
	move	r4,r17
	restore.jrc	00000050,r30,0000000A
0040315E                                           D5 10               ..
00403160 A0 04 6C E8 8B 60 86 FD 00 00 92 39 EB 1B       ..l..`.....9.. 

l0040316E:
	addiu	r6,r0,0000008C
	bnec	r6,r7,00403176

l00403174:
	balc	00403086

l00403176:
	bltic	r21,00000038,00403158

l0040317A:
	li	r6,00000010
	addiupc	r5,000179C0
	addiu	r4,r16,00000020
	balc	0040A100
00403188                         4F BA F0 84 0E 20 50 02         O.... P.
00403190 30 00 F1 C8 08 60 F0 84 30 20 50 02 38 00 F1 C8 0....`..0 P.8...
004031A0 D8 D0 28 5F E0 00 80 00 C7 A8 AD 3F AC 7E 9F 0A ..(_.......?.~..
004031B0 25 FB 25 9A 2E 7E ED 3A EB 60 26 13 05 00 7B B2 %.%..~.:.`&...{.
004031C0 5D F3 B4 C8 34 78 AB 60 AC CE 02 00 A6 A8 08 80 ]...4x.`........
004031D0 C9 90 CF 60 A0 CE 02 00 C5 E0 02 10 26 86 18 65 ...`........&..e
004031E0 CB B0 A5 20 48 00 A0 A8 0C 80 FD F3 A0 00 FF 7F ... H...........
004031F0 FF B0 E5 88 04 80 86 84 18 55 3C 76 84 98 20 12 .........U<v.. .
00403200 59 1B EB 60 C8 12 05 00 E9 90 EF 60 C0 12 05 00 Y..`.......`....
00403210 EB 60 D6 12 05 00 E4 C8 0A 00 82 D2 80 04 C8 DF .`..............
00403220 BF 38 37 1B C4 39 9C D2 7F 0A 5D E0 24 12 2E 7E .87..9....].$..~
00403230 73 3A 91 BD 80 04 F8 DF 63 3A 01 16 88 5C 09 5D s:......c:...\.]
00403240 00 2A DC 43 92 BD 3F 0A 7F FB EB 60 9C 12 05 00 .*.C..?....`....
00403250 E4 C8 16 68 07 D2 96 39 EB 60 8E 12 05 00 E4 C8 ...h...9.`......
00403260 12 00 8B 60 8C FC 00 00 88 39 EB 60 7C 12 05 00 ...`.....9.`|...
00403270 F4 C8 8B 07 0A D2 76 39 87 1A EB 60 6C 12 05 00 ......v9...`l...
00403280 E4 C8 D5 46 EB 60 8A 12 05 00 E0 A8 CB 3E 5A 39 ...F.`.......>Z9
00403290 9C D2 7F 0A F3 DF A4 10 80 04 D0 DF FF 39 9B 1B .............9..

;; fn004032A0: 004032A0
;;   Called from:
;;     00402F02 (in ping6_receive_error_msg)
;;     0040344E (in hextoui)
fn004032A0 proc
	bc	004049B0

;; ping6_install_filter: 004032A4
ping6_install_filter proc
	save	00000020,ra,00000003
	lwpc	r7,0045454C
	move	r17,r4
	bnezc	r7,004032FC
	lwpc	r7,004544C4
	li	r16,00000001
	sw	r7,000C(sp)
	illegal
	addiu	r0,r5,0000849D
	illegal
	addiupc	r7,000166E7
	sb	r0,000A(r7)
	li	r6,00000015
	sb	r16,000B(r7)
	addiu	r8,r0,00000008
	sw	r4,004C(sp)
	addiu	r5,r0,0000FFFF
	lw	r4,0000(r17)
	sh	r6,0008(r7)
	addiupc	r7,000166D4
	li	r6,0000001A
	balc	00407D60
	beqzc	r4,004032FC
	addiupc	r4,000068F8
	restore	00000020,ra,00000003
	bc	00408630
	restore.jrc	00000020,ra,00000003
	bc	00408340

;; niquery_option_subject_addr_handler: 00403302
niquery_option_subject_addr_handler proc
	save	00000050,ra,00000006
	li	r6,00000020
	movep	r16,r19,r4,r5
	move	r5,r0
	addiu	r4,sp,00000010
	balc	0040A690
00403310 14 D2 0C 3E 82 D3 82 04 B6 CD E4 B4 CB 60 EE CE ...>.........`..
00403320 02 00 81 D3 E6 B4 48 B0 C3 17 06 A8 0A 80 AB 60 ......H........`
00403330 A0 E1 02 00 A0 A8 82 00 EF 60 D2 CE 02 00 88 9B .........`......
00403340 E0 C8 36 10 7F D0 0E 18 90 D3 08 D0 EF 60 7E E1 ..6..........`~.
00403350 02 00 8A D3 E5 B4 C3 73 44 73 73 BC 00 2A C0 2A .......sDss..*.*
00403360 43 36 24 12 40 9A 00 2A 96 2A A0 04 9E E6 9B BC C6$.@..*.*......
00403370 8B 60 7A FB 00 00 87 3B 4C 18 04 D0 E5 B4 0F 62 .`z....;L......b
00403380 4C E1 02 00 D1 1B A5 16 D4 10 5A B0 54 39 8B 60 L.........Z.T9.`
00403390 40 E1 02 00 00 2A 96 1B 6F 62 36 E1 02 00 83 34 @....*..ob6....4
004033A0 00 2A 4C 2A 22 18 77 99 8B 62 22 E1 02 00 80 0A .*L*".w..b".....
004033B0 E0 1E 64 12 51 BA 27 15 ED 1B E6 88 7B 3F 80 04 ..d.Q.'.....{?..
004033C0 32 E6 00 2A BA 53 FF D0 91 10 56 1F             2..*.S....V.   

;; if_name2index: 004033CC
if_name2index proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	00406760
004033D4             02 9A 12 1F D0 10 A0 04 4A D1 8B 60     ........J..`
004033E0 0C FB 00 00 19 3B 02 D2 B7 38                   .....;...8     

;; fn004033EA: 004033EA
fn004033EA proc
	bc	00401E5C

;; fn004033EE: 004033EE
fn004033EE proc
	bc	00408770

;; fn004033F2: 004033F2
fn004033F2 proc
	bc	004081A0

;; niquery_option_handler: 004033F6
niquery_option_handler proc
	save	00000020,ra,00000006
	li	r17,FFFFFFFF
	move	r20,r4
	move	r19,r0
	addiupc	r16,00016667
	bc	00403416
00403404             F1 C8 0A E8 B2 00 01 00 84 17 B4 3C     ...........<
00403410 20 18 69 92 05 90                                .i...         

l00403416:
	lw	r4,0000(r16)
	beqzc	r4,0040343C

l0040341A:
	lw	r18,0004(r16)
	movep	r5,r6,r20,r18
	balc	0040A8E0
00403422       6F BA 02 17 92 22 07 39 59 BB E5 BB 84 17   o....".9Y.....
00403430 A0 10 93 10 F0 D8 24 12 04 A8 D7 BF             ......$.....   

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
00403450 10 D3 40 94 C3 72 00 0A C8 6B E3 34 04 12 F8 5F ..@..r...k.4..._
00403460 8E 9B 3D 3A 7F D0 C0 17 86 BB 35 3A 96 D3 C0 97 ..=:......5:....
00403470 90 10 22 1F                                     ..".           

;; build_echo: 00403474
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
	lh	r7,0004(r16)
	lwpc	r7,00454508
	sh	r4,0006(r16)
	beqzc	r7,004034A8
	move	r5,r0
	addiu	r4,r16,00000008
	balc	0040AF40
	lwpc	r4,00430074
	addiu	r4,r4,00000008
	restore.jrc	00000010,ra,00000002

;; build_niquery: 004034B2
build_niquery proc
	save	00000020,ra,00000002
	addiu	r7,r0,FFFFFF8B
	move	r16,r4
	sb	r7,0000(r4)
	sh	r0,0002(r4)
	lwpc	r4,004544E4
	addiu	r4,r4,00000001
	illegal
	addiu	r0,r2,0000F24D
	balc	0040359C
	sb	r4,0008(r16)
	sh	r4,000E(sp)
	srl	r4,r4,00000008
	sb	r4,0009(r16)
	balc	00403086

;; fn004034E2: 004034E2
;;   Called from:
;;     0040312E (in ping6_parse_reply)
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
	addiupc	r6,00027723
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
	balc	00403474
	lwpc	r7,00454554
	move	r16,r4
	bnezc	r7,0040354E
	lw	r4,0000(r18)
	addiu	r9,r0,0000001C
	addiupc	r8,000177E3
	movep	r5,r6,r17,r16
	lwpc	r7,004314C4
	balc	00407D40
	xor	r16,r16,r4
	illegal
	addiu	r0,r2,00002200
	illegal
	balc	004034B2
	addiupc	r6,000177D3
	lw	r4,0000(r18)
	sw	r6,0014(sp)
	li	r6,0000001C
	sw	r6,0018(sp)
	addiu	r6,sp,0000000C
	sw	r6,001C(sp)
	addiupc	r6,00016FBD
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
ping6_usage proc
	save	00000010,ra,00000001
	addiupc	r7,00007251
	addiupc	r6,0000724B
	movz	r6,r7,r4

l0040358A:
	addiupc	r5,0000724F
	lwpc	r4,00412EF0
	balc	004032FE
	li	r4,00000002
	balc	0040015A
	bc	00406700

;; ping6_run: 004035A0
ping6_run proc
	save	00000070,r30,0000000A
	addiu	r30,sp,FFFFF070
	movep	r19,r16,r6,r7
	lwpc	r7,00430214
	movep	r17,r18,r4,r5
	sw	r0,0FA4(r30)
	bltc	r7,r0,004035E0
	move	r5,r0
	addiupc	r4,00028759
	balc	0040AF40
	balc	0040AFB0
	addiupc	r7,00028753
	sw	r4,0048(sp)
	lwpc	r7,00430210
	bltc	r7,r0,00403C7E
	lwpc	r7,004314D4
	beqc	r0,r7,00403C7E
	bltic	r17,00000002,004035F6
	lwpc	r5,00412EF0
	addiupc	r4,000072BB
	balc	004083F0
	move	r4,r0
	balc	0040357C
	bneiuc	r17,00000001,0040362C
	lw	r17,0000(r18)
	bnezc	r19,00403648
	addiu	r7,r30,00000FA4
	addiupc	r6,0000766F
	movep	r4,r5,r17,r0
	balc	00405E20
	lw	r19,0FA4(r30)
	beqzc	r4,00403648
	balc	00405E00
	addiupc	r5,000071F9
	movep	r6,r7,r17,r4
	lwpc	r4,00412EF0
	balc	00408340
	li	r4,00000002
	balc	0040015A
	lwpc	r7,00430214
	bgec	r7,r0,00403640
	lwpc	r7,00430210
	bneiuc	r7,00000001,004035F2
	lwpc	r17,004544D4
	bc	004035FC
	addiupc	r18,00017756
	lw	r5,0014(r19)
	li	r6,0000001C
	move.balc	r4,r18,0040A130
	li	r4,0000003A
	balc	0040359C
	sh	r4,0002(r18)
	lw	r4,0FA4(r30)
	beqzc	r4,00403664
	balc	00405DF0
	move.balc	r4,r17,0040A890
	li	r5,0000003A
	move	r6,r4
	move.balc	r4,r17,0040A050
	beqzc	r4,00403682
	lwpc	r7,004544EC
	ori	r7,r7,00000004
	illegal
	addiu	r0,r5,00000662
	illegal
	move	r20,r19
	bnezc	r7,004036CC
	lw	r7,000C(r19)
	bnezc	r7,004036CC
	lw	r7,0010(r19)
	bnezc	r7,004036CC
	lw	r7,0014(r19)
	bnezc	r7,004036CC
	li	r6,00000010
	addiupc	r5,00017731
	addiupc	r4,00017721
	balc	004034E2
	lw	r7,0018(r18)
	lwpc	r6,00454550
	sw	r7,0058(sp)
	beqzc	r7,004036C4
	beqzc	r6,004036C6
	beqc	r6,r7,004036CC
	lwpc	r5,00412EF0
	addiupc	r4,0000726D
	balc	004083F0
	bc	00403626
	bnezc	r6,004036CC
	illegal
	addiu	r0,r5,000004E2
	bgeiuc	r26,00000002,00403E44
	illegal
	addiu	r0,r5,00001227
	bnec	r0,r6,004038E0
	lw	r6,000C(r7)
	bnec	r0,r6,004038E0
	lw	r6,0010(r7)
	bnec	r0,r6,004038E0
	lw	r7,0014(r7)
	bnec	r0,r7,004038E0
	li	r5,00000001
	li	r4,0000000A
	balc	00407D80
	move	r19,r4
	addiupc	r4,00006711
	bltc	r19,r0,00403790
	lwpc	r4,004544B0
	beqc	r0,r4,0040379C
	balc	004033CC
	lbu	r7,0008(r20)
	addiu	r6,r0,000000FE
	sw	r0,0FB0(r30)
	sw	r0,0FB4(r30)
	sw	r0,0FB8(r30)
	sw	r0,0FBC(r30)
	sw	r4,0FC0(r30)
	bnec	r6,r7,00403740
	lbu	r7,0009(r20)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r6,r7,00403750
	sw	r4,0018(r20)
	bc	00403750
	addiu	r6,r0,000000FF
	bnec	r6,r7,00403750
	lbu	r7,0009(r20)
	andi	r7,r7,0000000F
	beqic	r7,00000002,0040373A
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
	lwpc	r22,004544B0
	move.balc	r4,r22,0040A890
	move	r7,r22
	addiu	r8,r4,00000001
	li	r6,00000019
	addiu	r5,r0,0000FFFF
	move.balc	r4,r19,00407D60
	bnec	r4,r21,00403796
	addiupc	r4,00007216
	balc	00408630
	bc	00403626
	move	r4,r0
	balc	00401CE2
	addiu	r4,r0,00000401
	balc	0040359C
	li	r6,0000001C
	sh	r4,0002(r20)
	addiupc	r5,00017698
	addiu	r20,r0,FFFFFFFF
	move.balc	r4,r19,00405DD0
	bnec	r4,r20,004037BE
	addiupc	r4,0000671E
	bc	00403790
	li	r7,0000001C
	addiu	r6,r30,00000FAC
	addiupc	r5,0001652A
	sw	r7,0FAC(r30)
	move.balc	r4,r19,004066B0
	bnec	r4,r20,004037DA
	addiupc	r4,00006714
	bc	00403790
	sh	r0,0002(r17)
	move.balc	r4,r19,0040AF72
	lwpc	r7,004544B0
	beqzc	r7,0040382A
	addiu	r4,r30,00000FB0
	balc	004062A2
	beqzc	r4,004037F8
	addiupc	r4,000071F1
	bc	00403790
	lw	r23,0010(r17)
	lwpc	r20,004544B0
	lw	r8,0014(r17)
	lwm	r21,0008(r17),00000002
	lw	r19,0FB0(r30)
	bnec	r0,r19,0040389E
	move	r6,r20
	addiupc	r5,000072C8
	lwpc	r4,00412EF0
	balc	00408340
	lw	r4,0FB0(r30)
	balc	00406292
	lwpc	r19,004544B0
	beqzc	r19,00403864
	lwpc	r7,00454554
	li	r6,00000014
	addiupc	r17,00016E4F
	move	r5,r0
	addu	r17,r7,r17
	addiu	r7,r7,00000020
	illegal
	addiu	r0,r5,0000D3A0
	sw	r7,0040(sp)
	li	r7,00000029
	sw	r7,0044(sp)
	li	r7,00000032
	sw	r7,0048(sp)
	addiu	r4,r17,0000000C
	balc	0040A690
	move.balc	r4,r19,004033CC
	sw	r4,005C(sp)
	lhu	r17,0008(r18)
	addiu	r4,r0,0000FF00
	balc	0040359C
	and	r17,r17,r4
	addiu	r4,r0,0000FF00
	balc	0040359C
	bnec	r4,r17,0040394C
	lwpc	r7,00454514
	beqc	r0,r7,0040393A
	lwpc	r7,0043008C
	addiu	r6,r0,000003E7
	bltc	r6,r7,00403920
	lwpc	r5,00412EF0
	addiupc	r4,000071A4
	bc	004036BE
	lw	r17,000C(r19)
	beqzc	r17,004038DC
	lhu	r7,0000(r17)
	bneiuc	r7,0000000A,004038DC
	lw	r4,0004(r19)
	li	r6,00000003
	sw	r8,0F9C(r30)
	move.balc	r5,r20,0040A8E0
	lw	r8,0F9C(r30)
	bnezc	r4,004038DC
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
	lw	r19,0000(r19)
	bc	0040380E
	lwpc	r4,004544B0
	beqc	r0,r4,0040382A
	lbu	r7,0008(r17)
	addiu	r6,r0,000000FE
	bnec	r6,r7,0040390C
	lbu	r7,0009(r17)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r7,r6,0040382A
	balc	004033CC
	sw	r4,0058(sp)
	bc	0040382A
	addiu	r6,r0,000000FF
	bnec	r7,r6,0040382A
	lbu	r7,0009(r17)
	andi	r7,r7,0000000F
	bneiuc	r7,00000002,0040382A
	bc	00403904
	lwpc	r7,00430218
	bltc	r7,r0,0040393A
	beqic	r7,00000002,0040393A
	lwpc	r5,00412EF0
	addiupc	r4,0000716E
	bc	004036BE
	lwpc	r7,00430218
	bgec	r7,r0,0040394C
	li	r7,00000002
	illegal
	addiu	r0,r2,000060EB
	bbeqzc	r6,00000000,00403954
	bltc	r7,r0,00403970
	lw	r4,0000(r16)
	addiupc	r7,0001645E
	addiu	r8,r0,00000004
	li	r6,00000017
	li	r5,00000029
	balc	00403C9A
	li	r7,FFFFFFFF
	bnec	r4,r7,00403970
	addiupc	r4,00007169
	bc	00403790
	lwpc	r7,004544EC
	bbeqzc	r7,0000000F,00403990
	lw	r4,0000(r16)
	li	r6,0000001C
	addiupc	r5,0001644D
	balc	00405DB0
	li	r7,FFFFFFFF
	bnec	r4,r7,00403990
	addiupc	r4,00007165
	bc	00403790
	lwpc	r17,00430074
	bltiuc	r17,00000008,004039AC
	lwpc	r7,00430214
	bgec	r7,r0,004039AC
	li	r7,00000001
	illegal
	addiu	r0,r5,00000231
	move	r1,r24
	move.balc	r4,r17,00405292
	move	r19,r4
	bnezc	r4,004039C4
	lwpc	r5,00412EF0
	addiupc	r4,00006759
	bc	004036BE
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
	lwpc	r5,00412EF0
	addiupc	r4,00006694
	balc	004083F0
	sw	r0,0008(sp)
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
	li	r7,00000002
	lw	r4,0000(r16)
	sw	r7,0FAC(r30)
	addiu	r8,r0,00000004
	li	r6,00000007
	addiu	r5,r0,000000FF
	addiu	r7,r30,00000FAC
	balc	00403C9A
	bgec	r4,r0,00403A42
	lwpc	r5,00412EF0
	addiupc	r4,00007119
	balc	004083F0
	li	r6,00000020
	addiu	r5,r0,000000FF
	addiu	r4,r30,00000FB0
	balc	0040A690
	lw	r7,0008(r16)
	bnezc	r7,00403A60
	lw	r7,0FB0(r30)
	ins	r7,r0,00000001,00000001
	sw	r7,0FB0(r30)
	lwpc	r6,00430214
	lw	r7,0FC0(r30)
	bltc	r6,r0,00403A90
	ins	r7,r0,0000000C,00000001
	lw	r4,0000(r16)
	addiu	r8,r0,00000020
	sw	r7,0FC0(r30)
	li	r6,00000001
	li	r5,0000003A
	addiu	r7,r30,00000FB0
	balc	00403C9A
	bgec	r4,r0,00403A96
	addiupc	r4,0000710B
	bc	00403790
	ins	r7,r0,00000001,00000001
	bc	00403A72
	lwpc	r7,004544EC
	bbeqzc	r7,00000010,00403ABE
	lw	r4,0000(r16)
	addiu	r7,r30,00000FAC
	addiu	r8,r0,00000004
	li	r6,00000013
	li	r5,00000029
	sw	r0,0FAC(r30)
	balc	00403C9A
	li	r7,FFFFFFFF
	bnec	r4,r7,00403ABE
	addiupc	r4,00007102
	bc	00403790
	lwpc	r7,004544EC
	bbeqzc	r7,00000011,00403B00
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiupc	r7,0002851F
	li	r6,00000012
	li	r5,00000029
	addiu	r20,r0,FFFFFFFF
	balc	00403C9A
	bnec	r4,r20,00403AE6
	addiupc	r4,00007100
	bc	00403790
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiupc	r7,00028510
	li	r6,00000010
	li	r5,00000029
	balc	00403C9A
	bnec	r4,r20,00403B00
	addiupc	r4,00007103
	bc	00403790
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
	lw	r4,0000(r16)
	addiu	r8,r0,00000004
	addiu	r7,r30,00000FAC
	li	r6,00000008
	li	r5,00000029
	balc	00403C9A
	bnec	r4,r20,00403B36
	addiupc	r4,000070F6
	bc	00403790
	lwpc	r7,004544EC
	bbeqzc	r7,0000000A,00403B5A
	lw	r4,0000(r16)
	addiupc	r7,000284DF
	addiu	r8,r0,00000004
	li	r6,00000043
	li	r5,00000029
	balc	00403C9A
	li	r7,FFFFFFFF
	bnec	r4,r7,00403B5A
	addiupc	r4,000070F0
	bc	00403790
	lwpc	r7,004544EC
	bbeqzc	r7,00000009,00403BF2
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
	addiupc	r5,000174B2
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
	addiupc	r4,000070C5
	bc	00403790
	lw	r17,0010(sp)
	addiu	r8,r0,00000004
	lw	r4,0000(r16)
	li	r6,00000021
	illegal
	addiu	r0,r5,000097A1
	li	r5,00000029
	addiu	r7,r30,00000FAC
	balc	00403C9A
	bnec	r4,r21,00403BF0
	addiupc	r4,000070BC
	bc	00403790
	move	sp,r20
	li	r5,0000001C
	addiupc	r4,00017480
	lwpc	r18,004544C0
	balc	00401288
	movep	r5,r6,r18,r4
	addiupc	r4,000070B8
	balc	00403C9E
	lwpc	r4,004544BC
	beqzc	r4,00403C1E
	balc	00407620
	move	r5,r4
	addiupc	r4,000070B6
	balc	00403C9E
	lwpc	r7,004544B0
	lwpc	r18,004544EC
	bnezc	r7,00403C30
	bbeqzc	r18,0000000F,00403C60
	ori	r7,r18,00000004
	li	r5,0000001C
	addiupc	r4,000162F1
	illegal
	addiu	r0,r5,00002BFF
	sw	r4,0114(r28)
	lwpc	r6,004544B0
	move	r5,r4
	addiupc	r7,00006414
	movz	r6,r7,r6
	addiupc	r4,00006622
	balc	00403C9E
	illegal
	addiu	r0,r5,000060AB
	illegal
	addiupc	r4,00007097
	balc	00403C9E
	move.balc	r4,r16,004021B4
	balc	00401D02
	movep	r6,r7,r19,r17
	addiupc	r4,000162DF
	move.balc	r5,r16,00402A9E
	addiupc	r7,0001743F
	illegal
	addiu	r0,r2,0000D390
	illegal
	addiu	r0,r2,0000600F
	illegal
	bc	004035E0
	bc	00407D60
	bc	004086C0
	nop
	nop
	nop
	nop

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
	clz	r6,r8
	beqzc	r6,00403D10
	subu	r5,r0,r6
	sllv	r7,r7,r6
	srlv	r5,r10,r5
	sllv	r9,r8,r6
	or	r7,r7,r5
	sllv	r10,r10,r6
	ext	r8,r9,00000000,00000010
	srl	r4,r9,00000010
	illegal
	mul	r5,r8,r3
	modu	r6,r7,r4
	srl	r7,r10,00000010
	sll	r6,r6,00000010
	or	r6,r6,r7
	move	r7,r3
	bgeuc	r6,r5,00403D46
	addu	r6,r6,r9
	addiu	r7,r7,FFFFFFFF
	bltuc	r6,r9,00403D46
	bgeuc	r6,r5,00403D46
	addiu	r7,r3,FFFFFFFE
	addu	r6,r6,r9
	subu	r6,r6,r5
	illegal
	mul	r8,r8,r3
	modu	r5,r6,r4
	ext	r10,r10,00000000,00000010
	sll	r6,r5,00000010
	or	r10,r6,r10
	move	r4,r3
	bgeuc	r10,r8,00403D76
	addu	r10,r10,r9
	addiu	r4,r4,FFFFFFFF
	bltuc	r10,r9,00403D76
	bgeuc	r10,r8,00403D76
	addiu	r4,r3,FFFFFFFE
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
00403D94             00 A9 06 00 81 D3 87 21 98 49 E9 20     .......!.I. 
00403DA0 3F 5B 8A BB 25 21 D0 39 60 01 01 00 63 1B 00 01 ?[..%!.9`...c...
00403DB0 20 00 E9 20 10 48 E8 20 D0 41 E4 20 10 50 05 21  .. .H. .A. .P.!
00403DC0 50 58 E5 20 10 28 04 21 50 40 E9 80 C0 F3 A8 20 PX. .(.!P@..... 
00403DD0 90 42 A9 80 50 C0 AB 20 98 19 67 20 18 20 AB 20 .B..P.. ..g . . 
00403DE0 D8 31 68 81 50 C0 C6 80 10 C0 66 21 90 32 63 11 .1h.P.....f!.2c.
00403DF0 86 88 12 C0 C1 3C 7F 91 26 A9 0A C0 86 88 06 C0 .....<..&.......
00403E00 63 81 02 80 C1 3C 69 B2 A4 20 98 19 67 20 18 38 c....<i.. ..g .8
00403E10 A4 20 D8 31 08 81 C0 F3 C6 80 10 C0 06 21 90 2A . .1.........!.*
00403E20 C3 10 E5 88 12 C0 A1 3C DF 90 25 A9 0A C0 E5 88 .......<..%.....
00403E30 06 C0 C3 80 02 80 A1 3C 6B 81 10 C0 DF B3 CB 20 .......<k...... 
00403E40 90 5A CD 1A                                     .Z..           

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
	illegal
	mul	r6,r10,r5
	srl	r9,r3,00000010
	sll	r11,r11,00000010
	sllv	r8,r8,r13
	or	r11,r11,r9
	move	r9,r5
	bgeuc	r11,r6,00403EC4
	addu	r11,r11,r12
	addiu	r9,r9,FFFFFFFF
	bltuc	r11,r12,00403EC4
	bgeuc	r11,r6,00403EC4
	addiu	r9,r5,FFFFFFFE
	addu	r11,r11,r12
	subu	r11,r11,r6
	illegal
	mul	r10,r10,r6
	modu	r5,r11,r7
	ext	r3,r3,00000000,00000010
	sll	r5,r5,00000010
	or	r5,r5,r3
	move	r7,r6
	bgeuc	r5,r10,00403EFA
	addu	r5,r5,r12
	addiu	r7,r7,FFFFFFFF
	bltuc	r5,r12,00403EFA
	bgeuc	r5,r10,00403EFA
	addiu	r7,r6,FFFFFFFE
	addu	r5,r5,r12
	sll	r9,r9,00000010
	subu	r10,r5,r10
	or	r7,r9,r7
	mul	r6,r7,r8
	illegal
	bltuc	r10,r8,00403F20
	move	r11,r0
	bnec	r10,r8,00403D7C
	sllv	r4,r4,r13
	bgeuc	r4,r6,00403D7C
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
	clz	r2,r2
	beqc	r0,r2,00403F8C
	subu	r5,r0,r2
	sllv	r7,r7,r2
	srlv	r5,r8,r5
	sllv	r9,r9,r2
	or	r7,r7,r5
	sllv	r8,r8,r2
	ext	r4,r9,00000000,00000010
	srl	r10,r9,00000010
	illegal
	mul	r5,r5,r4
	modu	r6,r7,r10
	sll	r7,r6,00000010
	srl	r6,r8,00000010
	or	r7,r7,r6
	bgeuc	r7,r5,00403FB8
	addu	r7,r7,r9
	bltuc	r7,r9,00403FB8
	bgeuc	r7,r5,00403FB8
	addu	r7,r7,r9
	subu	r7,r7,r5
	illegal
	mul	r4,r4,r5
	modu	r6,r7,r10
	ext	r8,r8,00000000,00000010
	sll	r7,r6,00000010
	or	r8,r7,r8
	bgeuc	r8,r4,00403FE0
	addu	r8,r8,r9
	bltuc	r8,r9,00403FE0
	bgeuc	r8,r4,00403FE0
	addu	r8,r8,r9
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
00404000 40 A8 06 00 81 D3 47 21 98 49 49 20 3F 5B 40 A8 @.....G!.II ?[@.
00404010 34 00 25 21 D0 29 89 80 C0 F3 49 81 50 C0 45 21 4.%!.)....I.P.E!
00404020 98 31 CC 3C 45 21 D8 39 A8 80 50 C0 E7 80 10 C0 .1.<E!.9..P.....
00404030 DC 53 C7 88 0C C0 E1 3C 27 A9 06 C0 C7 88 02 C0 .S.....<'.......
00404040 E1 3C 7F B3 75 1B 20 D3 49 20 10 48 46 20 D0 31 .<..u. .I .HF .1
00404050 69 80 50 C0 C5 20 50 60 C4 20 50 30 45 20 10 28 i.P.. P`. P0E .(
00404060 6C 20 98 51 A6 20 90 3A A9 80 C0 F3 4D 3C 44 20 l .Q. .:....M<D 
00404070 10 40 6C 20 D8 31 87 80 50 C0 C6 80 10 C0 4C 53 .@l .1..P.....LS
00404080 46 89 0C C0 C1 3C 26 A9 06 C0 46 89 02 C0 C1 3C F....<&...F....<
00404090 46 21 D0 51 6A 20 98 21 8D 3C 6A 20 D8 31 FD F2 F!.Qj .!.<j .1..
004040A0 C6 80 10 C0 EC 52 85 88 0C C0 A1 3C 25 A9 06 C0 .....R.....<%...
004040B0 85 88 02 C0 A1 3C 5B B2 5D 1B                   .....<[.].     

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
	illegal
	mul	r6,r7,r14
	srl	r10,r5,00000010
	sll	r2,r2,00000010
	sllv	r4,r4,r3
	or	r2,r2,r10
	move	r10,r14
	bgeuc	r2,r6,00404146
	addu	r2,r2,r12
	addiu	r10,r10,FFFFFFFF
	bltuc	r2,r12,00404146
	bgeuc	r2,r6,00404146
	addiu	r10,r14,FFFFFFFE
	addu	r2,r2,r12
	subu	r2,r2,r6
	modu	r6,r2,r13
	illegal
	mul	r2,r7,r14
	andi	r5,r5,0000FFFF
	sll	r6,r6,00000010
	or	r7,r6,r5
	move	r5,r14
	bgeuc	r7,r2,0040417C
	addu	r7,r7,r12
	addiu	r5,r5,FFFFFFFF
	bltuc	r7,r12,0040417C
	bgeuc	r7,r2,0040417C
	addiu	r5,r14,FFFFFFFE
	addu	r7,r7,r12
	sll	r6,r10,00000010
	subu	r7,r7,r2
	or	r6,r6,r5
	mul	r10,r6,r8
	illegal
	move	r2,r10
	move	r5,r6
	bltuc	r7,r6,0040419C
	bnec	r6,r7,004041AC
	bgeuc	r4,r10,004041AC
	subu	r2,r10,r8
	subu	r6,r6,r12
	sltu	r10,r10,r2
	subu	r5,r6,r10
	subu	r2,r4,r2
	subu	r7,r7,r5
	sltu	r4,r4,r2
	subu	r7,r7,r4
	srlv	r4,r2,r3
	sllv	r8,r7,r9
	srlv	r5,r7,r3
	or	r4,r8,r4
	bc	00403FEA
	sigrie	00000000
	addiu	r0,r0,00008145

;; __gtdf2: 004041D0
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
;;     0040C5B6 (in fn0040C5A8)
;;     0040CFCC (in scalbn)
;;     0040E17C (in fmod)
;;     0040E1AE (in fmod)
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
	addiupc	r7,00006FA6
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
	illegal
	mul	r13,r8,r9
	mul	r15,r4,r11
	illegal
	mul	r5,r9,r11
	illegal
	addu	r10,r3,r10
	illegal
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
	srl	r7,r8,00000001
	andi	r8,r8,00000001
	or	r8,r7,r8
	sll	r7,r11,0000001F
	or	r8,r8,r7
	srl	r11,r11,00000001
	addiu	r5,r12,000003FF
	bgec	r0,r5,004045EC
	andi	r7,r8,00000007
	beqzc	r7,00404564
	andi	r7,r8,0000000F
	beqic	r7,00000004,00404564
	addiu	r7,r8,00000004
	sltu	r8,r7,r8
	addu	r11,r11,r8
	move	r8,r7
	bbeqzc	r11,00000018,00404570
	ins	r11,r0,00000008,00000001
	addiu	r5,r12,00000400
	addiu	r7,r0,000007FE
	bltc	r7,r5,0040467C
	sll	r7,r11,0000001D
	srl	r8,r8,00000003
	or	r8,r7,r8
	srl	r7,r11,00000003
	move	r6,r0
	move	r9,r8
	ins	r6,r7,00000000,00000001
	ins	r6,r5,00000004,00000001
	ins	r6,r2,0000000F,00000001
	move	r7,r6
	movep	r4,r5,r9,r7
	jrc	ra
	or	r4,r11,r4
	lui	r7,00000080
	and	r4,r4,r7
	li	r7,000FFFFF
	movn	r5,r0,r4
	movn	r11,r7,r4
	li	r7,FFFFFFFF
	move	r2,r5
	movn	r8,r7,r4
	lui	r7,00000080
	or	r7,r11,r7
	addiu	r5,r0,000007FF
	bc	00404588
	move	r2,r5
	beqic	r13,00000002,0040467C
	beqic	r13,00000003,004045BE
	bneiuc	r13,00000001,00404542
	movep	r7,r8,r0,r0
	bc	00404642
	move	r2,r14
	move	r11,r4
	move	r8,r9
	move	r13,r3
	bc	004045CE
	move	r12,r6
	bc	00404542
	li	r7,00000001
	subu	r7,r7,r5
	bgeic	r7,00000039,004045DA
	bgeic	r7,00000020,00404646
	li	r5,00000020
	srlv	r4,r8,r7
	subu	r5,r5,r7
	srlv	r7,r11,r7
	sllv	r6,r11,r5
	sllv	r8,r8,r5
	or	r6,r6,r4
	sltu	r8,r0,r8
	or	r8,r6,r8
	andi	r6,r8,00000007
	beqzc	r6,00404630
	andi	r6,r8,0000000F
	beqic	r6,00000004,00404630
	addiu	r6,r8,00000004
	sltu	r8,r6,r8
	addu	r7,r7,r8
	move	r8,r6
	bbnezc	r7,00000017,00404680
	sll	r6,r7,0000001D
	srl	r8,r8,00000003
	or	r8,r6,r8
	srl	r7,r7,00000003
	move	r5,r0
	bc	00404588
	addiu	r6,r0,FFFFFFE1
	subu	r6,r6,r5
	move	r5,r0
	srlv	r6,r11,r6
	beqic	r7,00000020,0040465E
	subu	r7,r0,r7
	sllv	r5,r11,r7
	or	r8,r5,r8
	move	r7,r0
	sltu	r8,r0,r8
	or	r8,r6,r8
	bc	00404616
	li	r11,000FFFFF
	addiu	r8,r0,FFFFFFFF
	move	r2,r0
	bc	004045BE
	movep	r7,r8,r0,r0
	bc	004045C6
	movep	r7,r8,r0,r0
	li	r5,00000001
	bc	00404588
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00008125

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
	illegal
	illegal
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
sysconf proc
	save	00000150,ra,00000002
	addiu	r7,r0,000000F8
	move	r16,r4
	bltuc	r7,r4,00404748

l0040473E:
	addiupc	r7,00006E13
	illegal
	bnezc	r4,00404756

l00404748:
	balc	004049B0
0040474C                                     96 D3 C0 97             ....
00404750 7F D2 E2 83 53 31 FF D3 E4 88 F7 BF E7 80 FF 80 ....S1..........
00404760 E4 88 1E 80 C2 72 84 80 40 F3 00 2A C2 13 C3 34 .....r..@..*...4
00404770 E0 60 FF FF FF 7F 82 34 C0 A8 B4 00 87 88 D3 FF .`.....4........
00404780 AE 18 E4 80 01 80 FC F3 EC C8 C7 57 C0 04 B0 DB ...........W....
00404790 7F 53 E0 D8 82 E0 00 00 B9 1B 80 00 00 80 B3 1B .S..............
004047A0 E4 04 8C FC 79 16 AB 1B 42 70 C0 00 80 00 70 BC ....y...Bp....p.
004047B0 00 2A DC 5E 81 D3 FD 84 08 10 C0 00 80 00 F0 10 .*.^............
004047C0 A0 10 7B D2 00 2A 88 02 80 10 E0 10 0C 18 A6 80 ..{..*..........
004047D0 01 80 89 90 58 53 F0 20 87 30 07 22 07 31 6F BB ....XS. .0.".1o.
004047E0 E9 90 C0 00 80 00 C7 A8 F1 3F 67 1B 42 72 00 2A .........?g.Br.*
004047F0 7E 02 CF 34 FD 80 C0 8E 06 BB 01 D3 C7 84 FC 9E ~..4............
00404800 86 34 02 CA 06 A8 87 34 E9 34 C8 B3 0F 36 A0 10 .4.....4.4...6..
00404810 E4 04 1C FC 90 20 D8 40 B8 3C 98 3C 79 17 E0 10 ..... .@.<.<y...
00404820 A0 3C 00 2A 8A A2 E0 60 FF FF FF 7F A0 88 4D 3F .<.*...`......M?
00404830 80 60 FF FF FF 7F 1B 1B 80 10 17 1B 80 60 69 10 .`...........`i.
00404840 03 00 0F 1B 00 00 00 00 00 00 00 00 00 00 00 00 ................

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
00404884             2F 62 46 E5 02 00 80 10 CF 50 89 90     /bF......P..
00404890 FB BB 24 22 0F 24 24 06 96 FB 14 96 02 18 42 92 ..$".$$.......B.
004048A0 C0 17 92 9B ED C8 F7 37 70 73 C7 20 0F 3C 41 17 .......7ps. .<A.
004048B0 C7 A4 68 C8 E9 1B FA 34 EF 60 E2 FB 04 00 FD 84 ..h....4.`......
004048C0 A8 80 EF 60 E0 FB 04 00 F0 34 99 97 72 B8 4A 72 ...`.....4..r.Jr
004048D0 00 2A 7E 68 9D 84 8C 80 FF 2B 97 FF DD A4 54 24 .*~h.....+....T$
004048E0 76 DB DD A4 5C 24 73 DB FD 84 84 80 F8 9B 81 D3 v...\$s.........
004048F0 20 01 08 00 E6 B4 82 D3 E8 B4 00 11 03 D3 C4 72  ..............r
00404900 49 D2 3D 21 50 39 00 12 02 B4 03 B4 04 B4 05 B4 I.=!P9..........
00404910 07 B4 09 B4 00 2A 38 01 C4 73 7E B0 FE 7F E4 C8 .....*8..s~.....
00404920 3A 28 E0 00 02 80 C0 04 86 D5 A0 80 64 80 38 D2 :(..........d.8.
00404930 00 2A 1C 01 04 88 24 80 00 84 00 10 00 20 00 00 .*....$...... ..
00404940 0F 62 DA DB 02 00 0F 62 D8 DB 02 00 04 18 E1 C8 .b.....b........
00404950 F5 7F 09 92 F0 A4 FF 90 F5 BB 73 1B 02 90 10 CA ..........s.....
00404960 B7 C7 81 D3 92 97 D3 1F                         ........       

;; __libc_start_init: 00404968
;;   Called from:
;;     0040499C (in __libc_start_main)
__libc_start_init proc
	save	00000010,ra,00000002
	addiupc	r16,0002B684
	balc	004000D4
00404974             06 18 80 17 01 90 F0 D8 E3 60 76 B6     .........`v.
00404980 02 00 F0 A8 F1 FF 12 1F                         ........       

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
004049A0 92 10 11 BF 70 DA FF 2B B1 B7 00 00 00 00 00 00 ....p..+........

;; __errno_location: 004049B0
;;   Called from:
;;     0040013A (in set_socket_option.isra.0.part.1)
;;     00400B56 (in create_socket)
;;     00401770 (in ping4_receive_error_msg)
;;     004032A0 (in fn004032A0)
;;     00404748 (in sysconf)
;;     00405826 (in realloc)
;;     00405C00 (in mmap64)
;;     00405D9E (in mbtowc)
;;     0040686E (in inet_ntop)
;;     00406A28 (in inet_pton)
;;     00408024 (in __sigaction)
;;     00408D3A (in printf_core)
;;     00408D8E (in fn00408D8E)
;;     0040C736 (in __intscan)
;;     0040CC3E (in __syscall_ret)
;;     0040CEDC (in __expand_heap)
;;     0040DD42 (in handler)
;;     0040E3F4 (in mbrtowc)
;;     0040E558 (in mbsrtowcs)
;;     0040E7B6 (in sem_init)
;;     0040E7E2 (in sem_post)
;;     0040E938 (in sem_trywait)
__errno_location proc
	illegal
	addiu	r4,r3,FFFFFF78
	jrc	ra
	sigrie	00000000
	addiu	r0,r0,000010E0

;; strerror_l: 004049C0
strerror_l proc
	move	r7,r0

l004049C2:
	addiupc	r6,00007151
	lbux	r6,r7(r6)
	bnezc	r6,004049D8

l004049CC:
	addiupc	r4,00006DC6

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
strerror proc
	illegal
	lw	r5,-0038(r3)
	bc	004049C0
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00001E11

;; abort: 00404A00
abort proc
	save	00000010,ra,00000001
	li	r4,00000006
	balc	00407EE0
00404A08                         80 10 00 2A 92 34 00 84         ...*.4..
00404A10 00 10 00 20 00 00 00 00 00 00 00 00 00 00 00 00 ... ............

;; __funcs_on_exit: 00404A20
;;   Called from:
;;     0040015E (in exit)
__funcs_on_exit proc
	jrc	ra

;; __libc_exit_fini: 00404A22
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
;;     00404A78 (in sysinfo)
;;     00405B3C (in getrlimit64)
;;     00405BCC (in madvise)
;;     00405DC0 (in bind)
;;     004066C0 (in getsockname)
;;     004066E2 (in getsockopt)
;;     004077A6 (in cleanup)
;;     00407D72 (in setsockopt)
;;     00407D92 (in socket)
;;     00407E14 (in sched_yield)
;;     00407EB0 (in __block_all_sigs)
;;     00407EC4 (in __block_app_sigs)
;;     00407ED4 (in __restore_sigs)
;;     00407F1A (in setitimer)
;;     00407FE0 (in __libc_sigaction)
;;     00408120 (in __stdio_read)
;;     00408180 (in __stdio_seek)
;;     0040ADA0 (in __syscall_cp_c)
;;     0040AE10 (in __wait)
;;     0040AE86 (in pthread_sigmask)
;;     0040AFA4 (in geteuid)
;;     0040AFB4 (in getpid)
;;     0040AFC4 (in getuid)
;;     0040AFDE (in isatty)
;;     0040B048 (in do_setxid)
;;     0040B238 (in _Exit)
;;     0040D2F8 (in __stdio_write)
;;     0040D33E (in __stdout_write)
;;     0040DD36 (in __set_thread_area)
;;     0040E00C (in fn0040E00C)
;;     0040E02A (in readdir64)
;;     0040E870 (in lseek64)
__syscall proc
	move	r2,r4
	move	r4,r5
	move	r5,r6
	move	r6,r7
	move	r7,r8
	move	r8,r9
	move	r9,r10
	addiu	r0,r8,00000000
	jrc	ra
00404A64             00 80 00 C0 00 80 00 C0 00 80 00 C0     ............

;; sysinfo: 00404A70
sysinfo proc
	save	00000010,ra,00000001
	move	r5,r4
	addiu	r4,r0,000000B3
	balc	00404A50
00404A7C                                     E1 83 12 30             ...0
00404A80 00 28 AC 81 00 00 00 00 00 00 00 00 00 00 00 00 .(..............

;; __lctrans_impl: 00404A90
;;   Called from:
;;     00404A92 (in __lctrans)
__lctrans_impl proc
	jrc	ra

;; __lctrans: 00404A92
;;   Called from:
;;     004049D4 (in strerror_l)
__lctrans proc
	bc	00404A90

;; __lctrans_cur: 00404A96
;;   Called from:
;;     00405E10 (in gai_strerror)
__lctrans_cur proc
	illegal
	lw	r7,-0038(r3)
	lw	r5,0014(r7)
	bc	00404A90
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

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
	addiupc	r4,00016D35
	balc	0040AD30
00404AD2       EB 60 5C DA 02 00 E0 20 D0 31 68 50 12 B0   .`\.... .1hP..
00404AE0 CB 60 4A DA 02 00 ED B3 26 8A 24 C0 43 72 23 B6 .`J.....&.$.Cr#.
00404AF0 00 2A FC 82 6E 9A EB 60 34 DA 02 00 E4 88 5C 00 .*..n..`4.....\.
00404B00 13 B0 E4 10 00 12 C3 34 48 B3 8F 60 20 DA 02 00 .......4H..` ...
00404B10 F2 B0 70 B0 82 04 20 DA 2F 62 16 DA 02 00 00 2A ..p... ./b.....*
00404B20 3E 62 90 10 23 1F                               >b..#.         

l00404B26:
	addiupc	r4,00016D07
	move	r16,r0
	balc	0040AD30
00404B30 81 D0 EB 60 FC D9 02 00 A5 1B                   ...`......     

l00404B3A:
	addiupc	r4,00016CFD
	addiu	r16,r16,FFFFFFFF
	balc	0040AD30
00404B44             EB 60 EA D9 02 00 E0 20 D0 31 A0 60     .`..... .1.`
00404B50 0F 00 00 80 68 50 25 AA 87 FF 83 1B EB 60 D2 D9 ....hP%......`..
00404B60 02 00 A3 1B 82 04 D0 D9 00 12 00 2A F2 61 90 10 ...........*.a..
00404B70 23 1F                                           #.             

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
	lw	r18,0004(r19)
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
	addiupc	r7,00027C47
	lw	r7,000C(r7)
	addiupc	r21,00016CD4
	addu	r16,r16,r21
	bnezc	r7,00404C12

l00404BAC:
	addu	r7,r21,r20
	lw	r7,0010(r7)
	bnec	r0,r7,00404C8E

l00404BB6:
	move	r7,r18
	sll	r6,r22,00000004
	addu	r20,r20,r21
	addiu	r6,r6,00000008
	addu	r6,r6,r21
	sw	r6,0010(r20)
	sw	r6,0014(r20)
	beqc	r18,r7,00404C8E

l00404BCE:
	lw	r6,0000(r16)
	beqzc	r6,00404C44

l00404BD2:
	illegal
	sw	r0,0000(sp)
	illegal
	lw	r7,0004(r16)
	beqzc	r7,00404B84
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404B84
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r16,00404A50
	lw	r18,0004(r19)
	bc	00404B86
	illegal
	beqzc	r6,00404C4E
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r16,00000004
	move.balc	r4,r16,0040ADB0

l00404C12:
	illegal
	illegal
	li	r7,00000001
	illegal
	beqzc	r7,00404C16
	bc	00404C02

l00404C24:
	addiu	r6,r0,000001FF
	bltuc	r6,r7,00404C5C

l00404C2C:
	srl	r7,r7,00000003
	addiupc	r6,00007057
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
00404C4E                                           95 22               ."
00404C50 50 31 B1 17 64 17 C0 A8 71 3F 5D 1B             P1..d...q?].   

l00404C5C:
	addiu	r6,r0,00001C00
	bltuc	r6,r7,00404C7E

l00404C64:
	srl	r7,r7,00000007
	addiupc	r6,0000703B
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
	illegal
	sw	r0,0000(sp)
	illegal
	lw	r7,0004(r16)
	bnezc	r7,00404CCA
	li	r4,00000001
	restore.jrc	00000020,ra,00000008
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404CC6
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
	illegal
	aluipc	r5,00000404
	ori	r6,r5,00000550
	illegal
	and	r6,r6,r4
	ori	r17,r5,00000550
	illegal
	beqzc	r6,00404D1C
	illegal

l00404D34:
	li	r6,FFFFFFFF
	beqc	r6,r7,00404D4C

l00404D38:
	illegal
	illegal
	and	r6,r6,r7
	illegal
	beqzc	r6,00404D3C
	illegal

l00404D4C:
	lw	r18,0004(r19)
	lwm	r6,0008(r19),00000002
	bc	00404C96

;; alloc_rev: 00404D54
;;   Called from:
;;     0040500C (in free)
alloc_rev proc
	save	00000020,ra,00000008
	move	r19,r4
	lw	r18,0000(r19)
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
	addiupc	r7,00027B5D
	lw	r7,000C(r7)
	addiupc	r21,00016BEA
	addu	r16,r16,r21
	bnezc	r7,00404DE6

l00404D80:
	addu	r7,r21,r20
	lw	r7,0010(r7)
	bnec	r0,r7,00404E62

l00404D8A:
	move	r7,r18
	sll	r6,r22,00000004
	addu	r20,r20,r21
	addiu	r6,r6,00000008
	addu	r6,r6,r21
	sw	r6,0010(r20)
	sw	r6,0014(r20)
	beqc	r18,r7,00404E62

l00404DA2:
	lw	r6,0000(r16)
	beqzc	r6,00404E18

l00404DA6:
	illegal
	sw	r0,0000(sp)
	illegal
	lw	r7,0004(r16)
	beqzc	r7,00404D58
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404D58
	li	r7,00000001
	li	r6,00000001
	li	r4,00000062
	move.balc	r5,r16,00404A50
	lw	r18,0000(r19)
	bc	00404D5A
	illegal
	beqzc	r6,00404E22
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r16,00000004
	move.balc	r4,r16,0040ADB0

l00404DE6:
	illegal
	illegal
	li	r7,00000001
	illegal
	beqzc	r7,00404DEA
	bc	00404DD6

l00404DF8:
	addiu	r6,r0,000001FF
	bltuc	r6,r7,00404E30

l00404E00:
	srl	r7,r7,00000003
	addiupc	r6,00006F6D
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
00404E22       95 22 50 31 B0 17 64 17 C0 A8 71 3F 5D 1B   ."P1..d...q?].

l00404E30:
	addiu	r6,r0,00001C00
	bltuc	r6,r7,00404E52

l00404E38:
	srl	r7,r7,00000007
	addiupc	r6,00006F51
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
	illegal
	sw	r0,0000(sp)
	illegal
	lw	r7,0004(r16)
	bnezc	r7,00404EA6
	li	r4,00000001
	restore.jrc	00000020,ra,00000008
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,00404EA2
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
	illegal
	aluipc	r5,00000404
	ori	r6,r5,00000550
	illegal
	and	r6,r6,r4
	ori	r17,r5,00000550
	illegal
	beqzc	r6,00404EF8
	illegal

l00404F10:
	li	r6,FFFFFFFF
	beqc	r6,r7,00404F28

l00404F14:
	illegal
	illegal
	and	r6,r6,r7
	illegal
	beqzc	r6,00404F18
	illegal

l00404F28:
	lwm	r5,0008(r19),00000002
	bc	00404E70

;; free: 00404F2E
;;   Called from:
;;     00405910 (in realloc)
;;     00405DF0 (in freeaddrinfo)
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
	illegal

l00404F5C:
	lwx	r6,r22(r17)
	addu	r18,r17,r22
	beqc	r6,r7,00404F76

l00404F66:
	sb	r0,0000(r0)
	illegal

l00404F6E:
	restore	00000040,r30,0000000A
	bc	00405CC2

l00404F76:
	move	r19,r22
	addiupc	r30,00016AEA
	addiupc	r21,00027A58
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
	addiupc	r7,00006E9B
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
	illegal
	sw	r0,0408(r30)
	illegal
	lw	r7,040C(r30)
	bnec	r0,r7,00405118

l00404FF8:
	lw	r7,0000(r23)
	beqzc	r7,0040500C

l00404FFC:
	illegal
	sw	r11,0000(r23)
	illegal
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
	xor	r6,r6,r7
	lw	r17,0008(sp)
	sltu	r7,r7,r6
	li	r6,00000001
	movn	r5,r6,r7
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
	lw	r17,0008(sp)
	xor	r6,r6,r7
	sltu	r6,r7,r6
	li	r5,00000001
	movn	r4,r5,r6
	sw	r4,0008(sp)
	addu	r18,r18,r7
	bc	00404F82
	illegal
	beqc	r0,r6,00404FD4
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,00016C6F
	addiupc	r4,00016C6B
	balc	0040ADB0

l00405086:
	illegal
	addiu	r7,r30,00000408
	illegal
	li	r7,00000001
	addiu	r5,r30,00000408
	illegal
	beqzc	r7,0040508A
	bc	0040506E
	illegal
	beqc	r0,r6,00404FC4
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r23,00000004
	move.balc	r4,r23,0040ADB0

l004050B4:
	illegal
	illegal
	li	r7,00000001
	illegal
	beqzc	r7,004050B8
	bc	004050A0

l004050C6:
	addiu	r7,r0,00001C00
	bltuc	r7,r16,00405140

l004050CE:
	srl	r16,r16,00000007
	addiupc	r7,00006E06
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
004050F8                         81 D3 C0 00 81 00 62 D2         ......b.
00405100 FF 0B 4D F9 E0 80 26 80 87 A8 01 3F 81 D3 01 D3 ..M...&....?....
00405110 62 D2 FF 0B 3B F9 F5 1A 81 D3 C0 00 81 00 A2 04 b...;...........
00405120 36 D8 62 D2 FF 2B 29 F9 E0 80 26 80 87 A8 C9 3E 6.b..+)...&....>
00405130 81 D3 01 D3 A2 04 20 D8 62 D2 FF 2B 13 F9 B9 1A ...... .b..+....

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
	illegal
	sw	r0,0408(r30)
	illegal
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
	illegal
	sw	r11,0000(r23)
	illegal
	lw	r7,0001(r23)
	bnezc	r7,004051D4

l004051D2:
	restore.jrc	00000040,r30,0000000A
004051D4             81 D3 C0 00 81 00 62 D2 FF 0B 71 F8     ......b...q.
004051E0 E0 80 26 80 87 A8 EB 3F B7 10 81 D3 01 D3 62 D2 ..&....?......b.
004051F0 CA 83 42 30 FF 29 59 F8                         ..B0.)Y.       

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
00405216                   A9 1B                               ..       

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
	illegal
	ori	r7,r4,00000550
	illegal
	or	r7,r7,r5
	ori	r16,r4,00000550
	illegal
	beqzc	r7,00405236
	illegal

l0040524E:
	beqc	r0,r6,0040517E

l00405252:
	illegal
	illegal
	or	r7,r7,r6
	illegal
	beqzc	r7,00405256
	illegal
	bc	0040517E
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,00016B73
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040519C
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,00016B68
	li	r4,00000062
	balc	00404A50
	bc	0040519C

l00405290:
	jrc	ra

;; malloc: 00405292
;;   Called from:
;;     0040579E (in __malloc0)
;;     00405900 (in realloc)
;;     0040591A (in realloc)
malloc proc
	save	00000050,r30,0000000A
	addiupc	r22,000278CC
	li	r7,7FFFFFEF
	lw	r6,0024(r22)
	addiu	r5,r4,FFFFFFFF
	subu	r7,r7,r6
	bgeuc	r7,r5,00405472
	bnec	r0,r4,004056DC
	li	r7,00000010
	addiu	r21,r0,FFFFFFFF
	addiu	r20,r0,FFFFFFFF
	sw	r7,0008(sp)
	sw	r0,000C(sp)
	aluipc	r23,00000405
	lw	r4,0550(r23)
	lw	r5,0554(r23)
	and	r7,r4,r21
	and	r6,r5,r20
	or	r5,r7,r6
	beqc	r0,r5,00405370
	bnec	r0,r7,00405450
	subu	r7,r0,r6
	and	r7,r7,r6
	li	r6,076BE629
	mul	r7,r7,r6
	addiupc	r6,00006CE8
	srl	r7,r7,0000001B
	lbux	r19,r6(r7)
	addiu	r19,r19,00000020
	sll	r17,r19,00000004
	lw	r7,0003(r22)
	addiu	r18,r17,00000008
	addiupc	r16,00016925
	addu	r30,r16,r18
	bnec	r0,r7,0040543E
	addu	r17,r16,r17
	addu	r7,r16,r18
	lw	r9,0010(r17)
	beqc	r0,r9,0040546C
	bnec	r7,r9,00405514
	lwx	r7,r18(r16)
	beqzc	r7,004052C2
	illegal
	swx	r0,r18(r16)
	illegal
	lw	r7,0004(r30)
	beqzc	r7,004052C2
	li	r7,00000001
	addiu	r6,r0,00000081
	move	r5,r30
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,004052C2
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
	lw	r17,0008(sp)
	aluipc	r17,00000405
	lw	r7,0003(r22)
	addiu	r6,r6,00000010
	sw	r6,001C(sp)
	bnec	r0,r7,004055B4
	addiu	r4,sp,0000001C
	balc	0040CDF0
	move	r16,r4
	beqc	r0,r4,004056EA
	lwpc	r18,00432540
	lw	r17,001C(sp)
	beqc	r18,r4,004053A2
	addiu	r7,r7,FFFFFFF0
	li	r6,00000001
	addiu	r18,r4,00000010
	sw	r6,0008(sp)
	sw	r7,001C(sp)
	lw	r5,0544(r17)
	addu	r6,r18,r7
	li	r4,00000001
	ori	r7,r7,00000001
	sw	r7,-0008(r6)
	illegal
	addiu	r0,r2,0000A486
	bltiuc	r7,00000010,004054F0
	illegal
	bltiuc	r7,00000013,0040565C
	illegal
	sw	r0,0544(r17)
	illegal
	addiupc	r7,000168B7
	lw	r7,0004(r7)
	bnec	r0,r7,00405678
	beqc	r0,r9,004056D2
	sw	r9,000C(sp)
	move.balc	r4,r9,00404D54
	lw	r18,000C(sp)
	beqc	r0,r4,00405636
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
	ins	r7,r0,00000000,00000001
	lw	r17,0008(sp)
	addiu	r6,r7,FFFFFFF0
	bltuc	r5,r6,004055CE
	addiu	r16,r9,00000008
	move	r4,r16
	restore.jrc	00000050,r30,0000000A
	illegal
	beqc	r0,r6,0040530E
	li	r7,00000001
	li	r6,00000001
	addiu	r5,r30,00000004
	move	r4,r30
	balc	0040ADB0
	illegal
	illegal
	li	r7,00000001
	illegal
	beqzc	r7,00405442
	bc	00405428
	subu	r6,r0,r7
	and	r7,r7,r6
	li	r6,076BE629
	mul	r7,r7,r6
	addiupc	r6,00006C2F
	srl	r7,r7,0000001B
	lbux	r19,r6(r7)
	bc	004052FC
	sw	r7,0050(sp)
	sw	r7,0054(sp)
	bc	0040531E
	addiu	r4,r4,00000017
	lui	r7,0000001C
	ins	r4,r0,00000000,00000001
	sw	r4,0008(sp)
	bgeuc	r7,r4,004054BA
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
	addiu	r6,r16,FFFFFFF8
	li	r7,00000008
	addiu	r16,r4,00000010
	sw	r7,0008(sp)
	sw	r6,000C(sp)
	move	r4,r16
	restore.jrc	00000050,r30,0000000A
	srl	r7,r4,00000004
	addiu	r30,r7,FFFFFFFF
	bltiuc	r30,00000021,00405790
	addiu	r7,r7,FFFFFFFE
	addiu	r6,r0,000001FF
	bltuc	r6,r7,004056A6
	srl	r7,r7,00000003
	addiupc	r6,00006C06
	addu	r7,r7,r6
	li	r5,00000001
	lbu	r30,-0004(r7)
	addiu	r7,r30,00000001
	move	r4,r7
	sw	r7,000C(sp)
	subu	r7,r0,r7
	sllv	r6,r5,r4
	srlv	r7,r5,r7
	slti	r5,r4,00000020
	movz	r7,r0,r4
	movz	r7,r6,r5
	movz	r6,r0,r5
	subu	r6,r0,r6
	subu	r20,r0,r7
	sltu	r7,r0,r6
	move	r21,r6
	subu	r20,r20,r7
	bc	004052BE
	lw	r17,0002(r9)
	lw	r5,0003(r9)
	bltic	r19,00000028,00405610
	lw	r17,000C(sp)
	addiu	r7,r7,00000002
	bgec	r7,r19,004055FA
	lw	r6,0001(r9)
	lw	r17,0008(sp)
	ins	r6,r0,00000000,00000001
	subu	r18,r6,r7
	srl	r7,r18,00000004
	addiu	r7,r7,FFFFFFFF
	bltiuc	r7,00000021,00405610
	addiu	r4,r0,000001FF
	bltuc	r4,r7,004056BA
	srl	r7,r7,00000003
	addiupc	r4,00006BCE
	addu	r7,r7,r4
	lbu	r7,-0004(r7)
	bnec	r7,r19,00405610
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
	lw	r6,0000(r30)
	beqc	r0,r6,00405412
	illegal
	sw	r0,0000(r30)
	illegal
	lw	r7,0004(r30)
	bnec	r0,r7,0040564A
	lw	r7,0001(r9)
	ins	r7,r0,00000000,00000001
	lw	r17,0008(sp)
	addiu	r6,r7,FFFFFFF0
	bgeuc	r5,r6,00405420
	bc	004055CE
	illegal
	beqc	r0,r6,00405380
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,000167CE
	addiupc	r4,000167CA
	balc	0040ADB0
	illegal
	ori	r7,r17,00000544
	illegal
	li	r7,00000001
	ori	r5,r17,00000544
	illegal
	beqzc	r7,004055B8
	bc	0040559C
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
	bneiuc	r19,0000003F,00405610
	lw	r6,0001(r9)
	lui	r7,0000001C
	lw	r17,0008(sp)
	ins	r6,r0,00000000,00000001
	subu	r18,r6,r4
	bltuc	r7,r18,0040552E
	beqc	r17,r5,0040572E
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
	lw	r7,-0004(r18)
	lw	r17,0008(sp)
	ins	r7,r0,00000000,00000001
	addiu	r6,r7,FFFFFFF0
	bgeuc	r5,r6,00405420
	bc	004055CE
	li	r7,00000001
	addiu	r6,r0,00000081
	move	r5,r30
	li	r4,00000062
	sw	r9,000C(sp)
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	lw	r18,000C(sp)
	bnec	r7,r4,0040558A
	li	r7,00000001
	li	r6,00000001
	move	r5,r30
	li	r4,00000062
	sw	r9,000C(sp)
	balc	00404A50
	lw	r18,000C(sp)
	lw	r7,0001(r9)
	bc	0040558C
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,00016761
	li	r4,00000062
	sw	r9,000C(sp)
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	lw	r18,000C(sp)
	bnec	r7,r4,004053DC
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,00016754
	li	r4,00000062
	balc	00404A50
	lw	r18,000C(sp)
	bc	004053DC
	srl	r7,r7,00000007
	addiupc	r6,00006B1A
	addu	r7,r7,r6
	li	r5,00000001
	lbu	r30,-0004(r7)
	addiu	r7,r30,00000011
	bc	004054E0
	addiu	r4,r0,00001C00
	bltuc	r4,r7,004056D8
	srl	r7,r7,00000007
	addiupc	r4,00006B0C
	addu	r7,r7,r4
	lbu	r7,-0004(r7)
	addiu	r7,r7,00000010
	bc	0040554A
	move	r16,r0
	move	r4,r16
	restore.jrc	00000050,r30,0000000A
	li	r7,0000003F
	bc	0040554A
	balc	004049B0
	move	r16,r0
	li	r7,0000000C
	sw	r7,0000(sp)
	move	r4,r16
	restore.jrc	00000050,r30,0000000A
	lw	r7,0544(r17)
	beqzc	r7,004056D2
	illegal
	sw	r0,0544(r17)
	illegal
	addiupc	r7,00016722
	lw	r7,0004(r7)
	beqzc	r7,004056D2
	li	r7,00000001
	addiu	r6,r0,00000081
	addiupc	r5,0001671B
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,004056D2
	li	r7,00000001
	li	r6,00000001
	addiupc	r5,00016710
	li	r4,00000062
	balc	00404A50
	move	r4,r16
	restore.jrc	00000050,r30,0000000A
	li	r6,00000001
	subu	r7,r0,r19
	srlv	r7,r6,r7
	slti	r5,r19,00000020
	sllv	r6,r6,r19
	movz	r7,r0,r19
	movz	r7,r6,r5
	movz	r6,r0,r5
	nor	r7,r0,r7
	nor	r5,r0,r6
	beqzc	r6,00405772
	illegal
	ori	r6,r23,00000550
	illegal
	and	r6,r6,r5
	ori	r4,r23,00000550
	illegal
	beqzc	r6,0040575A
	illegal
	li	r6,FFFFFFFF
	beqc	r6,r7,0040578A
	illegal
	illegal
	and	r6,r6,r7
	illegal
	beqzc	r6,0040577A
	illegal
	lw	r17,0002(r9)
	lw	r5,0003(r9)
	bc	00405614
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
004057A2       2A 9A E4 A4 FC C0 E4 C8 22 00 B0 00 03 00   *.......".....
004057B0 DA 32 9A 9A 85 20 0F 2C E4 10 70 17 0A 9B 70 94 .2... .,..p...p.
004057C0 F1 93 A7 A8 F5 3F 12 1F F1 93 A7 A8 ED 3F 12 1F .....?.......?..

;; realloc: 004057D0
realloc proc
	beqc	r0,r4,00405918

l004057D4:
	save	00000030,ra,00000009
	addiupc	r23,0002762B
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
	illegal

l00405822:
	li	r19,00000010
	beqzc	r5,004057FA

l00405826:
	balc	004049B0
0040582A                               8C D3 40 12 C0 97           ..@...
00405830 92 10 39 1F                                     ..9.           

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
00405854             FF D3 E4 88 D8 00 96 3C D4 22 D0 B1     .......<."..
00405860 42 91 C4 F7 92 10 39 1F                         B.....9.       

l00405868:
	lwx	r6,r17(r21)
	addu	r18,r21,r17
	beqc	r6,r7,0040587A

l00405872:
	sb	r0,0000(r0)
	illegal

l0040587A:
	bltuc	r17,r19,004058C4

l0040587E:
	ori	r7,r17,00000001
	sw	r7,-0004(r16)
	swx	r7,r17(r21)
	ins	r7,r0,00000000,00000001
	addiu	r6,r7,FFFFFFF0
	bltuc	r19,r6,0040589C

l00405896:
	move	r18,r16
	move	r4,r18
	restore.jrc	00000030,ra,00000009
0040589C                                     FD B1 B3 80             ....
004058A0 01 00 C6 80 01 00 75 22 50 89 B3 22 87 2C 12 92 ......u"P..".,..
004058B0 11 97 50 12 A7 22 87 34 B0 A4 FC C8 FF 2B 6F F6 ..P..".4.....+o.
004058C0 92 10 39 1F                                     ..9.           

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
	addiu	r4,r19,FFFFFFF8
	balc	00405292
	move	r18,r4
	beqc	r0,r4,00405830
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
00405932       40 12 34 8A F9 FE 50 12 5D 1B 00 00 00 00   @.4...P.].....

;; __getopt_msg: 00405940
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
	move.balc	r4,r18,0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r18,0040857C
	beqzc	r4,00405980
	movep	r6,r7,r17,r16
	li	r5,00000001
	move.balc	r4,r19,0040857C
	bnec	r17,r4,00405980
	li	r4,0000000A
	move.balc	r5,r16,00408700
	move	r4,r16
	restore	00000020,ra,00000006
	bc	004084E0

;; __posix_getopt: 0040598A
__posix_getopt proc
	save	00000040,ra,00000006
	lwpc	r7,00430254
	movep	r19,r18,r4,r5
	move	r17,r6
	beqzc	r7,004059A0
	lwpc	r7,00432960
	beqzc	r7,004059B4
	li	r7,00000001
	illegal
	addiu	r0,r2,0000600F
	illegal
	illegal
	addiu	r0,r2,000060EB
	bnec	r26,r4,004059BC
	li	r4,FFFFFFFF
	bgec	r7,r19,00405B00
	lwxs	r5,r7(r18)
	beqc	r0,r5,00405B00
	lbu	r6,0000(r5)
	beqic	r6,0000002D,004059E4
	lbu	r6,0000(r17)
	bneiuc	r6,0000002D,00405B00
	addiu	r7,r7,00000001
	illegal
	addiu	r0,r4,000060EF
	bnec	r20,r3,004059E2
	li	r4,00000001
	restore.jrc	00000040,ra,00000006
	lbu	r6,0001(r5)
	li	r4,FFFFFFFF
	beqc	r0,r6,00405B00
	bneiuc	r6,0000002D,004059FE
	lbu	r6,0002(r5)
	bnezc	r6,004059FE
	addiu	r7,r7,00000001
	illegal
	addiu	r0,r2,00001F46
	lwpc	r7,004544A4
	bnezc	r7,00405A0E
	li	r7,00000001
	illegal
	addiu	r0,r4,000060EB
	illegal
	li	r6,00000004
	addu	r5,r5,r7
	addiu	r4,sp,00000018
	balc	00405CE0
	move	r7,r4
	bgec	r4,r0,00405A2C
	addiu	r7,r0,0000FFFD
	sw	r7,0018(sp)
	li	r7,00000001
	lwpc	r5,00430254
	lwxs	r4,r5(r18)
	lwpc	r6,004544A4
	addu	r20,r4,r6
	addu	r6,r7,r6
	illegal
	addiu	r0,r4,00002086
	sll	r18,r16,00000007
	bnezc	r6,00405A5A
	addiu	r5,r5,00000001
	illegal
	addiu	r0,r4,000060AF
	lb	ra,0002(r26)
	lbu	r6,0000(r17)
	addiu	r6,r6,FFFFFFD5
	andi	r6,r6,000000FD
	bnezc	r6,00405A68
	addiu	r17,r17,00000001
	move	r16,r0
	sw	r0,001C(sp)
	bc	00405A74
	addiu	r16,r16,00000001
	beqzc	r4,00405A8E
	beqc	r5,r6,00405A8E
	li	r6,00000004
	addu	r5,r17,r16
	addiu	r4,sp,0000001C
	sw	r7,000C(sp)
	balc	00405CE0
	lw	r17,000C(sp)
	lw	r17,0018(sp)
	lw	r17,001C(sp)
	bgec	r0,r4,00405A6E
	addu	r16,r16,r4
	bc	00405A72
	move	r4,r6
	beqc	r5,r6,00405ABA
	illegal
	addiu	r0,r4,00005F18
	bneiuc	r6,0000003A,00405AA4
	li	r4,0000003F
	restore.jrc	00000040,ra,00000006
	lwpc	r6,00430250
	beqzc	r6,00405AA0
	move	r6,r20
	addiupc	r5,00006205
	lw	r4,0000(r18)
	balc	00405940
	bc	00405AA0
	lbux	r6,r16(r17)
	bneiuc	r6,0000003A,00405B00
	addiu	r16,r16,00000001
	addu	r16,r17,r16
	lbu	r5,0000(r16)
	bneiuc	r5,0000003A,00405B02
	illegal
	addiu	r0,r4,00005F88
	lwpc	r6,004544A4
	bneiuc	r7,0000003A,00405AE0
	beqzc	r6,00405B00
	lwpc	r7,00430254
	addiu	r5,r7,00000001
	lwxs	r7,r7(r18)
	illegal
	addiu	r0,r2,0000B37E
	illegal
	addiu	r0,r4,000060EF
	illegal
	restore.jrc	00000040,ra,00000006
	lwpc	r5,00430254
	bltc	r5,r19,00405AD2
	illegal
	addiu	r0,r4,00001086
	lbu	r5,0000(r17)
	beqic	r5,0000003A,00405B00
	lwpc	r6,00430250
	beqc	r0,r6,00405AA0
	move	r6,r20
	addiupc	r5,000061D5
	bc	00405AB2
	sigrie	00000000

;; getrlimit64: 00405B30
getrlimit64 proc
	save	00000010,ra,00000002
	move	r16,r5
	movep	r7,r8,r0,r16
	movep	r5,r6,r0,r4
	addiu	r4,r0,00000105
	balc	00404A50
00405B40 00 2A EC 70 30 BA 81 17 8C BB 80 17 C0 60 FE FF .*.p0........`..
00405B50 FF 7F E6 88 08 C0 7F D3 FF D3 D0 A4 00 2C 83 17 .............,..
00405B60 8C BB 82 17 C0 60 FE FF FF 7F E6 88 08 C0 7F D3 .....`..........
00405B70 FF D3 D0 A4 08 2C 12 1F 00 00 00 00 00 00 00 00 .....,..........

;; ioctl: 00405B80
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
00405BD0 E1 83 12 30 00 28 58 70 00 00 00 00 00 00 00 00 ...0.(Xp........

;; __vm_wait: 00405BE0
;;   Called from:
;;     00405CC6 (in munmap)
__vm_wait proc
	jrc	ra

;; mmap64: 00405BE2
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
00405C04             96 D3 C0 97 7F D2 28 1F                 ......(.   

l00405C0C:
	li	r7,7FFFFFFE
	bgeuc	r7,r18,00405C1E
	balc	004049B0
	li	r7,0000000C
	bc	00405C06
	bbeqzc	r19,00000004,00405C26
	balc	00405BE0
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
	sigrie	00000000
	sigrie	00000000

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
	balc	004049B0
	li	r7,0000000C
	sw	r7,0000(sp)
	li	r4,FFFFFFFF
	restore	00000030,ra,00000005
	addiu	sp,sp,00000010
	jrc	ra
	move	r9,r0
	bbeqzc	r17,00000001,00405CA2
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
	movep	r7,r8,r16,r17
	movep	r5,r6,r18,r19
	addiu	r4,r0,000000D8
	balc	00404A50
	balc	0040CC30
	bc	00405C78
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

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
00405CCA                               80 00 D7 00 30 BF           ....0.
00405CD0 FF 2B 7D ED E3 83 12 30 00 28 54 6F 00 00 00 00 .+}....0.(To....

;; mbtowc: 00405CE0
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
	illegal
	lw	r8,-0038(r3)
	lw	r8,0000(r8)
	bnec	r0,r8,00405D1E
	lb	r7,0000(r5)
	addiu	r6,r0,0000DFFF
	and	r7,r7,r6
	sw	r7,0000(sp)
	li	r7,00000001
	bc	00405CFE
	addiu	r7,r7,FFFFFF3E
	bgeiuc	r7,00000033,00405D9E
	addiupc	r8,00006B23
	lwxs	r7,r7(r8)
	bgeiuc	r6,00000004,00405D42
	lsa	r6,r6,r6,00000001
	sll	r6,r6,00000001
	addiu	r6,r6,FFFFFFFA
	sllv	r6,r7,r6
	bltc	r6,r0,00405D9E
	lbu	r8,0001(r5)
	sra	r9,r7,0000001A
	srl	r6,r8,00000003
	addu	r9,r9,r6
	addiu	r6,r6,FFFFFFF0
	or	r9,r9,r6
	ins	r9,r0,00000000,00000001
	bnec	r0,r9,00405D9E
	sll	r7,r7,00000006
	addiu	r6,r8,FFFFFF80
	or	r6,r6,r7
	bltc	r6,r0,00405D72
	li	r7,00000002
	sw	r6,0000(sp)
	bc	00405CFE
	lbu	r7,0002(r5)
	addiu	r7,r7,FFFFFF80
	bgeiuc	r7,00000000,00405D9E
	sll	r6,r6,00000006
	or	r7,r7,r6
	bltc	r7,r0,00405D8A
	sw	r7,0000(sp)
	li	r7,00000003
	bc	00405CFE
	lbu	r6,0003(r5)
	addiu	r6,r6,FFFFFF80
	bgeiuc	r6,00000000,00405D9E
	sll	r7,r7,00000006
	or	r7,r7,r6
	sw	r7,0000(sp)
	li	r7,00000004
	bc	00405CFE

l00405D9E:
	balc	004049B0
00405DA2       D4 D3 C0 97 FF D3 55 1B 00 00 00 00 00 00   ......U.......

;; bind: 00405DB0
bind proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000C8
	balc	00404A50
00405DC4             E1 83 12 30 00 28 64 6E 00 00 00 00     ...0.(dn....

;; connect: 00405DD0
connect proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000CB
	balc	0040ADA4
00405DE4             E1 83 12 30 00 28 44 6E 00 00 00 00     ...0.(Dn....

;; freeaddrinfo: 00405DF0
freeaddrinfo proc
	bc	00404F2E
00405DF4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; gai_strerror: 00405E00
gai_strerror proc
	addiu	r7,r4,00000001
	addiupc	r4,0000678A

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
;;     00400BB6 (in ping4_run)
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
00405E70 C4 13 04 A8 E3 BF 19 BF D2 10 D0 72 9D 00 40 01 ...........r..@.
00405E80 00 2A 4A 11 64 12 04 A8 20 81 9E 20 18 80 50 72 .*J.d... .. ..Pr
00405E90 00 2A FC 49 BC D3 24 12 0F 3E B0 00 01 00 5A B2 .*.I..$..>....Z.
00405EA0 01 D2 00 2A 1A 6F 44 12 80 88 02 01 C0 12 8E 98 ...*.oD.........
00405EB0 04 22 50 B1 D1 00 01 00 D0 72 C0 0A 72 42 BC D3 ."P......r..rB..
00405EC0 3D 02 48 01 FE 20 18 38 E0 12 44 B6 E6 B4 77 8A =.H.. .8..D...w.
00405ED0 CA 00 CE 73 84 36 A7 12 03 B4 6C 18 51 A5 F8 C0 ...s.6....l.Q...
00405EE0 1C D3 90 D3 75 84 03 20 6A 81 02 10 55 84 02 20 ....u.. j...U.. 
00405EF0 66 21 10 3E 20 D3 67 11 F4 00 3C 00 74 BC 5D A4 f!.> .g...<.t.].
00405F00 28 2C E5 B4 68 B5 49 B5 00 2A 84 47 68 35 D4 00 (,..h.I..*.Gh5..
00405F10 20 00 49 35 F4 00 3C 00 5D A4 28 24 74 85 10 90  .I5..<.].($t...
00405F20 54 F5 74 84 08 90 54 84 0C 90 D4 84 14 90 D4 86 T.t...T.........
00405F30 18 90 F4 84 1C 90 40 C9 22 10 40 C9 3C 50 E3 34 ......@.".@.<P.4
00405F40 AC 92 85 36 E9 90 E3 B4 E3 34 C7 AB 8F 3F E4 34 ...6.....4...?.4
00405F50 E9 92 C6 34 97 90 7E B3 E4 B4 73 1B 54 85 20 50 ...4..~...s.T. P
00405F60 95 84 00 60 00 2A 98 07 04 D3 94 84 22 50 B1 10 ...`.*......"P..
00405F70 94 00 24 00 00 2A B8 41 C5 1B 54 85 20 50 95 84 ..$..*.A..T. P..
00405F80 00 60 00 2A 7A 07 B1 10 94 84 22 50 94 00 28 00 .`.*z....."P..(.
00405F90 D1 A4 FC C0 D4 84 38 90 10 D3 D9 1B E7 34 20 B0 ......8......4 .
00405FA0 10 A4 E0 C8 C0 13 70 95 AF 1A C4 13 AB 1A C0 83 ......p.........
00405FB0 0A 80 A5 1A 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; copy_addr: 00405FC0
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

l00405FD4:
	bltuc	r8,r6,00405FE0

l00405FD8:
	lh	r5,0000(r16)
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

l00405FFE:
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
	subu	r5,r6,r7
	bgeiuc	r5,00000004,0040603A

l00406036:
	move	r5,r0
	bc	0040604E

l0040603A:
	illegal
	lhu	r5,0000(r7)
	beqic	r4,00000007,0040604C
	addiu	r5,r5,00000003
	ins	r5,r0,00000000,00000001
	addu	r7,r7,r5
	bc	00406030
	addiu	r5,r5,FFFFFFFC

l0040604E:
	addiu	r5,r5,000000A4
	li	r4,00000001
	balc	0040CDC0
00406058                         04 12 7F D2 4C 98 9C 7F         ....L...
00406060 F0 C8 04 81 95 17 51 02 20 00 F0 84 8C 90 96 17 ......Q. .......
00406070 82 97 90 17 9E B3 7F B1 EC C8 4A 20 81 17 E0 88 ..........J ....
00406080 06 02 F0 84 8C 80 E7 80 3F 20 67 22 0F 3C 72 17 ........? g".<r.
00406090 07 97 0F F6 81 17 E0 88 EE 01 B0 17 82 BB 13 F6 ................
004060A0 B1 17 82 9B 07 F6 13 F7                         ........       

l004060A8:
	move	r4,r0
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
004060C6                   AA 7F 28 7F E0 C8 6A 10 EC C8       ..(...j...
004060D0 10 18 E0 C8 36 08 A8 7F EB 90 E0 80 40 E0 A4 B3 ....6.......@...
004060E0 91 1B E0 C8 12 18 F0 C8 ED 3F 90 00 A4 00 DC 90 .........?......
004060F0 A1 92 06 96 98 39 DF 1B DC 90 CC C8 D9 8F 90 02 .....9..........
00406100 90 00 A1 92 80 0A 28 40 90 F7 CB 1B DC 90 CC C8 ......(@........
00406110 C5 CF 95 16 11 D2 F1 84 12 60 90 02 20 00 90 84 .........`.. ...
00406120 20 50 90 00 2C 00 89 96 A1 92 F0 84 28 50 D0 84  P..,.......(P..
00406130 2B 10 5A 39 98 F7 9F 1B DC 90 CC C8 99 CF 95 16 +.Z9............
00406140 11 D2 F1 84 12 60 90 02 68 00 90 84 68 50 90 00 .....`..h...hP..
00406150 74 00 B0 84 6C 90 A1 92 F0 84 70 50 D0 84 73 10 t...l.....pP..s.
00406160 2C 39 90 86 14 90 6F 1B A1 17 81 97 A2 17 16 91 ,9....o.........
00406170 82 97 90 17 9E B3 7F B1 EC C8 68 20 83 17 E0 88 ..........h ....
00406180 13 3F F1 84 11 20 C0 00 80 00 B1 86 10 20 90 02 .?... ....... ..
00406190 10 00 D0 02 44 00 27 12 00 B4 01 B4 02 B4 03 B4 ....D.'.........
004061A0 E6 88 04 C0 20 82 80 80 9C F0 A0 00 FF 00 51 82 .... .........Q.
004061B0 83 C0 9D 10 D2 10 00 2A D6 44 40 CA 16 80 C4 73 .......*.D@....s
004061C0 97 F0 74 B1 88 D3 FF B0 20 02 FF 00 F1 20 10 88 ..t..... .... ..
004061D0 32 A6 F0 88 20 11 00 01 10 00 FD 10 D5 BF 9F 0A 2... ...........
004061E0 DF FD B1 1A AA 7F E0 C8 56 10 12 85 00 60 EC C8 ........V....`..
004061F0 10 18 E0 C8 26 08 A8 7F EB 90 E0 80 40 E0 A4 B3 ....&.......@...
00406200 71 1B E0 C8 6C 18 F0 C8 ED 27 1C 91 A1 93 D0 00 q...l....'......
00406210 68 00 B1 84 10 20 31 85 14 80 14 18 03 17 A1 93 h.... 1.........
00406220 B1 84 10 20 1C 91 31 85 14 80 08 9B D0 00 68 00 ... ..1.......h.
00406230 05 92 06 18 D0 00 20 00 03 92 FF 2B 83 FD B7 1B ...... ....+....
00406240 83 17 90 02 20 00 96 9B F0 00 68 00 24 D3 87 BE .... .....h.$...
00406250 3C 38 24 D3 E4 10 85 97 74 BC 00 2A 32 44 12 85 <8$.....t..*2D..
00406260 00 60 A1 93 D4 10 B1 84 10 20 1C 91 31 85 14 80 .`....... ..1...
00406270 C7 1B C8 80 04 80 CC C8 7D 8F 90 02 90 00 A1 92 ........}.......
00406280 80 0A AC 3E 90 F7 6F 1B 1F 0A A3 EC 1B 1A       ...>..o....... 

;; fn0040628E: 0040628E
fn0040628E proc
	bc	0040A130

;; freeifaddrs: 00406292
freeifaddrs proc
	save	00000010,ra,00000002
	bc	0040629E

l00406296:
	lw	r16,0000(r4)
	balc	00404F2E
0040629C                                     90 10                   .. 

l0040629E:
	bnezc	r4,00406296

l004062A0:
	restore.jrc	00000010,ra,00000002

;; getifaddrs: 004062A2
getifaddrs proc
	save	00000120,ra,00000003
	addiu	r6,r0,00000108
	move	r5,r0
	move	r17,r4
	addiu	r4,sp,00000008
	balc	0040A690
004062B4             C2 73 DF 04 65 FD 63 BC 00 2A 12 13     .s..e.c..*..
004062C0 04 12 0A BA E2 34 90 97 90 10 E3 83 23 31 82 34 .....4......#1.4
004062D0 FF 2B BF FF F3 1B 00 00 00 00 00 00 00 00 00 00 .+..............

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
004062F8                         80 A8 02 80 04 5C               .....\ 

l004062FE:
	move	r4,r0
	restore.jrc	00000010,ra,00000002

;; getnameinfo: 00406302
getnameinfo proc
	save	00000870,r30,0000000A
	lhu	r20,0000(r4)
	move	r16,r4
	move	r23,r6
	move	r30,r7
	move	r19,r10
	swm	r8,0014(sp),00000002
	beqic	r20,00000002,00406324
	beqic	r20,0000000A,004064E4
	addiu	r4,r0,FFFFFFFA
	bc	004065A0
	addiu	r17,r4,00000004
	addiu	r4,r0,FFFFFFFA
	bltiuc	r5,00000010,004065A0
	lbu	r9,0004(r16)
	addiupc	r5,00005DDF
	lbu	r8,0005(r16)
	addiu	r4,sp,00000058
	lbu	r7,0006(r16)
	move	r18,r0
	lbu	r6,0007(r16)
	balc	00408860
	beqc	r0,r23,004065BE
	beqc	r0,r30,004065BE
	andi	r7,r19,00000001
	sb	r0,0138(sp)
	sw	r7,001C(sp)
	bnec	r0,r7,0040644E
	addiu	r7,r0,00000408
	addiu	r6,sp,00000438
	addiu	r5,sp,000000A8
	addiupc	r4,00005DD8
	balc	00408070
	move	r21,r4
	beqc	r0,r4,0040644E
	sw	r17,0010(sp)
	bneiuc	r20,00000002,00406392
	li	r6,00000004
	addiu	r4,sp,00000038
	move.balc	r5,r17,0040A130
	li	r6,0000000C
	addiupc	r5,00006541
	addiu	r4,sp,0000002C
	balc	004066AA
	addiu	r7,sp,0000002C
	sw	r7,0010(sp)
	move	r6,r21
	addiu	r5,r0,00000200
	addiu	r4,sp,00000238
	balc	00408240
	beqc	r0,r4,0040644A
	li	r5,00000023
	addiu	r4,sp,00000238
	balc	0040A770
	beqzc	r4,004063B6
	li	r7,0000000A
	sb	r0,0001(r4)
	sb	r7,0000(r4)
	addiu	r6,sp,00000238
	lbu	r5,0000(r6)
	addiu	r22,r6,00000001
	andi	r4,r5,000000DF
	beqzc	r4,004063CE
	addiu	r5,r5,FFFFFFF7
	bgeiuc	r5,00000005,00406552
	sb	r0,0000(r6)
	addiu	r5,sp,00000238
	addiu	r4,sp,0000003C
	move	r6,r0
	balc	00406B50
	bgec	r0,r4,00406392
	lw	r17,003C(sp)
	bneiuc	r6,00000002,004063FA
	li	r6,00000004
	addiu	r5,sp,00000044
	addiu	r4,sp,00000050
	balc	004066AA
	li	r6,0000000C
	addiupc	r5,0000650C
	addiu	r4,sp,00000044
	balc	004066AA
	sw	r0,0040(sp)
	lw	r17,0010(sp)
	li	r6,00000010
	addiu	r5,sp,00000044
	balc	0040A100
	bnezc	r4,00406392
	lw	r17,0040(sp)
	bnec	r6,r18,00406392
	lbu	r6,0000(r22)
	beqc	r0,r6,00406556
	beqic	r6,00000020,004066A2
	addiu	r6,r6,FFFFFFF7
	bltiuc	r6,00000005,004066A2
	move	r6,r22
	lbu	r5,0000(r6)
	andi	r4,r5,000000DF
	beqzc	r4,00406432
	addiu	r5,r5,FFFFFFF7
	bgeiuc	r5,00000005,004066A6
	sb	r0,0000(r6)
	addiu	r5,r0,000000FF
	subu	r6,r6,r22
	bltc	r5,r6,00406392
	addiu	r6,r6,00000001
	addiu	r4,sp,00000138
	move.balc	r5,r22,0040A130
	move.balc	r4,r21,00408060
	lbu	r7,0138(sp)
	bnezc	r7,0040649E
	lw	r17,001C(sp)
	bnezc	r7,0040649E
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
	addiu	r7,sp,00000138
	addiupc	r6,FFFFFF25
	addiu	r4,sp,00000438
	balc	0040D0D0
	lbu	r7,0138(sp)
	bnec	r0,r7,00406590
	addiu	r4,r0,FFFFFFFE
	bbnezc	r19,00000003,004065A0
	addiu	r7,r0,00000100
	addiu	r6,sp,00000138
	movep	r4,r5,r20,r17
	balc	00406860
	beqc	r0,r18,00406590
	bbeqzc	r19,00000008,0040655A
	addiu	r4,sp,00000047
	sb	r0,0047(sp)
	li	r6,0000000A
	addiu	r4,r4,FFFFFFFF
	modu	r7,r18,r6
	addiu	r7,r7,00000030
	sb	r7,0000(r4)
	move	r7,r18
	illegal
	bnezc	r18,004064CC
	bc	0040657E
	addiu	r17,r4,00000008
	addiu	r4,r0,FFFFFFFA
	bltiuc	r5,0000001C,004065A0
	li	r6,0000000C
	addiupc	r5,0000648C
	move.balc	r4,r17,0040A100
	beqzc	r4,00406536
	addiu	r7,sp,00000058
	li	r6,0000000F
	lbux	r5,r6(r17)
	addiupc	r18,0000648B
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
	addiupc	r5,00005CF2
	addiu	r4,sp,00000098
	balc	0040A860
	lw	r18,0018(r16)
	bc	0040634A
	lbu	r9,0014(r16)
	addiupc	r5,00005CDB
	lbu	r8,0015(r16)
	addiu	r4,sp,00000058
	lbu	r7,0016(r16)
	lbu	r6,0017(r16)
	balc	00408860
	bc	00406532
	move	r6,r22
	bc	004063BA
	move	r6,r22
	bc	00406432
	lbu	r7,0000(r17)
	addiu	r6,r0,000000FE
	bnec	r7,r6,004065A4
	lbu	r7,0001(r17)
	addiu	r6,r0,00000080
	andi	r7,r7,000000C0
	bnec	r7,r6,004064C4
	addiu	r5,sp,00000439
	move.balc	r4,r18,00406710
	beqc	r0,r4,004064C4
	li	r7,00000025
	addiu	r5,r4,FFFFFFFF
	sb	r7,-0001(r4)
	addiu	r4,sp,00000138
	balc	0040A750
	addiu	r4,sp,00000138
	balc	0040A890
	bltuc	r4,r30,004065B6
	addiu	r4,r0,FFFFFFF4
	restore.jrc	00000870,r30,0000000A
	addiu	r6,r0,000000FF
	bnec	r7,r6,004064C4
	lbu	r7,0001(r17)
	andi	r7,r7,0000000F
	bneiuc	r7,00000002,004064C4
	bc	00406572
	addiu	r5,sp,00000138
	move.balc	r4,r23,0040A860
	lw	r17,0014(sp)
	move	r4,r0
	beqzc	r7,004065A0
	lw	r17,0018(sp)
	beqzc	r7,004065A0
	illegal
	balc	00407630
	sb	r0,0138(sp)
	move	r18,r4
	bbnezc	r19,00000001,004065F4
	addiu	r7,r0,00000408
	addiu	r6,sp,00000438
	addiu	r5,sp,00000238
	addiupc	r4,00005CA0
	balc	00408070
	andi	r19,r19,00000010
	move	r20,r4
	bnezc	r4,00406636
	lbu	r7,0138(sp)
	addiu	r16,sp,00000138
	bnezc	r7,0040661E
	move	r4,r18
	addiu	r16,sp,00000047
	sb	r0,0047(sp)
	li	r6,0000000A
	addiu	r16,r16,FFFFFFFF
	modu	r7,r4,r6
	addiu	r7,r7,00000030
	sb	r7,0000(r16)
	move	r7,r4
	illegal
	bnezc	r4,00406608
	move.balc	r4,r16,0040A890
	lw	r17,0018(sp)
	bgeuc	r4,r7,0040659C
	lw	r17,0014(sp)
	move.balc	r5,r16,0040A860
	move	r4,r0
	bc	004065A0
	move	r6,r16
	bc	00406654
	addiu	r17,sp,000000A8
	move	r6,r20
	addiu	r5,r0,00000080
	move.balc	r4,r17,00408240
	beqzc	r4,0040669C
	li	r5,00000023
	move.balc	r4,r17,0040A770
	beqzc	r4,00406652
	li	r7,0000000A
	sb	r0,0001(r4)
	sb	r7,0000(r4)
	move	r6,r17
	lbu	r7,0000(r6)
	beqzc	r7,00406636
	addiu	r16,r6,00000001
	beqic	r7,00000020,00406668
	addiu	r7,r7,FFFFFFF7
	bgeiuc	r7,00000005,00406632
	sb	r0,0000(r6)
	addiu	r5,sp,0000002C
	li	r6,0000000A
	move.balc	r4,r16,0040A022
	bnec	r4,r18,00406636
	lw	r17,002C(sp)
	beqc	r16,r4,00406636
	li	r6,00000004
	addiupc	r5,00005C5F
	beqzc	r19,00406688
	addiupc	r5,00005C58
	balc	0040A8E0
	bnezc	r4,00406636
	subu	r6,r16,r17
	bgeic	r6,00000021,00406636
	addiu	r4,sp,00000138
	move.balc	r5,r17,0040A130
	move.balc	r4,r20,00408060
	bc	004065F4
	addiu	r22,r22,00000001
	bc	0040640C
	addiu	r6,r6,00000001
	bc	00406422
	bc	0040A130
	addiu	r0,r0,00001E11

;; getsockname: 004066B0
getsockname proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000CC
	balc	00404A50
004066C4             E1 83 12 30 00 28 64 65 00 00 00 00     ...0.(de....

;; getsockopt: 004066D0
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
004066E6                   E1 83 12 30 00 28 42 65 00 00       ...0.(Be..

;; htonl: 004066F0
htonl proc
	illegal
	jrc	ra
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00008084

;; htons: 00406700
;;   Called from:
;;     00400B62 (in fn00400B62)
htons proc
	illegal
	andi	r4,r4,0000FFFF
	jrc	ra
	sigrie	00000000
	sigrie	00000000

;; if_indextoname: 00406710
if_indextoname proc
	save	00000030,ra,00000004
	move	r6,r0
	movep	r18,r17,r4,r5
	li	r5,00080001
	li	r4,00000001
	balc	00407D80
00406722       04 12 04 A8 2A 80 A0 00 10 89 DD 10 44 B6   ....*.......D.
00406730 FF 2B 4D F4 44 12 39 D2 1F 0B 15 E3 12 88 16 80 .+M.D.9.........
00406740 FF 2B 6D E2 C0 17 F0 C8 08 98 FF 2B 63 E2 86 D3 .+m........+c...
00406750 C0 97 80 10 34 1F 10 D3 BD 10 20 0A D2 41 34 1F ....4..... ..A4.

;; if_nametoindex: 00406760
;;   Called from:
;;     004033D0 (in if_name2index)
if_nametoindex proc
	save	00000030,ra,00000003
	move	r6,r0
	move	r17,r4
	li	r5,00080001
	li	r4,00000001
	balc	00407D80
00406772       04 12 04 88 04 80 80 10 33 1F 10 D3 9D 10   ........3.....
00406780 20 0B AC 41 A0 00 33 89 DD 10 1F 0A F3 F3 24 12  ..A..3.......$.
00406790 39 D2 1F 0B BB E2 11 A8 DF BF 84 34 33 1F 00 00 9..........43...

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
	addiu	r6,sp,00000010
	addiu	r5,r0,000000FF
	lwxs	r6,r7(r6)
	bltuc	r5,r6,004067C0

l00406820:
	illegal
	addiu	r7,r7,00000001
	bneiuc	r7,00000004,00406814
	li	r4,00000001
	restore.jrc	00000030,ra,00000004
	addiu	r0,r0,00001E11

;; inet_ntoa: 00406830
;;   Called from:
;;     00401590 (in fn00401590)
inet_ntoa proc
	save	00000010,ra,00000001
	addiupc	r6,00005B89
	srl	r10,r4,00000018
	ext	r9,r4,00000000,00000008
	ext	r8,r4,00000008,00000008
	andi	r7,r4,000000FF
	li	r5,00000010
	addiupc	r4,00016093
	balc	00408820
0040684E                                           82 04               ..
00406850 1E C1 11 1F 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; inet_ntop: 00406860
inet_ntop proc
	save	000000B0,ra,00000007
	movep	r16,r17,r5,r6
	move	r20,r7
	beqic	r4,00000002,0040687A

l0040686A:
	beqic	r4,0000000A,0040689E

l0040686E:
	balc	004049B0
00406872       E1 D3 20 12 C0 97 42 19                     .. ...B.     

l0040687A:
	lbu	r10,0003(r16)
	addiupc	r6,00005B63
	lbu	r9,0002(r16)
	lbu	r8,0001(r16)
	lbu	r7,0000(r16)
	movep	r4,r5,r17,r20
	balc	00408820
00406892       84 AA 26 C1 FF 2B 17 E1 9C D3 D7 1B         ..&..+...... 

l0040689E:
	li	r6,0000000C
	addiupc	r5,000062C6
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
	addiupc	r6,00005B18
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
	addiupc	r6,00005B09
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
	addiupc	r5,00005B0C
	addu	r4,r18,r16
	balc	0040A960
0040697A                               95 88 E5 BF 70 12           ....p.
00406980 A4 12 DF 1B                                     ....           

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
	movep	r4,r5,r17,r18
	balc	0040A860
	move	r4,r17
	restore.jrc	000000B0,ra,00000007

;; inet_pton: 004069C0
;;   Called from:
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
	illegal
	lbux	r5,r6(r16)
	bnezc	r5,00406A12
	illegal
	restore.jrc	00000030,ra,00000006
	bneiuc	r5,0000002E,004069EA
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
00406A2C                                     E1 D3 C0 97             ....
00406A30 7F D2 36 1F                                     ..6.           

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
	li	r6,00000007
	subu	r6,r6,r17
	bgec	r7,r6,00406B0E

l00406B00:
	illegal
	addiu	r7,r7,00000001
	bc	00406AF8

l00406B08:
	li	r18,00000001
	bnec	r7,r4,00406ADA

l00406B0E:
	movep	r6,r7,r19,r0
	illegal
	addiu	r7,r7,00000001
	srl	r4,r5,00000008
	sb	r4,0000(r6)
	addiu	r6,r6,00000002
	sb	r5,-0001(r6)
	bneiuc	r7,00000008,00406B10
	li	r4,00000001
	beqzc	r18,00406B44
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
00406B44             36 1F 00 00 00 00 00 00 00 00 00 00     6...........

;; __lookup_ipliteral: 00406B50
;;   Called from:
;;     004070D4 (in __lookup_name)
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
00406B72       82 D3 80 97 01 94 81 D3                     ........     

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
00406B96                   D8 73 F6 B1 13 A4 C0 88             .s...... 

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
00406BBA                               0A D3 00 97 E0 98           ......
00406BC0 99 5F 51 02 01 00 E7 80 30 80 C7 88 34 C0 C3 72 ._Q.....0...4..r
00406BD0 40 0A 38 34 E3 34 F8 5F C2 9B FD 84 10 20 C0 00 @.84.4._..... ..
00406BE0 FE 00 C7 A8 22 00 FD 84 11 20 C0 00 80 00 E7 80 ....".... ......
00406BF0 C0 20 C7 A8 6F 3F 5F 0A 67 FB 80 88 67 3F 01 96 . ..o?_.g...g?..
00406C00 77 1B 63 BC 23 B6 CD 1B C0 00 FF 00 C7 A8 55 3F w.c.#.........U?
00406C10 FD 84 11 20 FF F3 E0 C8 DD 17 49 1B E1 9A 45 1B ... ......I...E.
00406C20 80 10 DB 1B                                     ....           

l00406C24:
	bc	0040A130
00406C28                         00 00 00 00 00 00 00 00         ........

;; __isspace: 00406C30
;;   Called from:
;;     00406F46 (in fn00406F46)
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

l00406CA8:
	jrc	ra

;; addrcmp: 00406CAA
addrcmp proc
	lw	r4,0018(r4)
	lw	r7,0018(r5)
	subu	r4,r7,r4
	jrc	ra

;; name_from_dns: 00406CB2
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
00406D00 FF D3 90 84 B4 99 E4 88 82 00 01 D0 B0 CA 4C 10 ..............L.
00406D10 CB 71 C7 73 54 11 20 01 00 02 13 11 49 73 C5 72 .q.sT. .....Is.r
00406D20 00 0A 86 0A 20 12 E0 80 0B 80 04 A8 62 80 11 AA .... .......b...
00406D30 64 00 EF 34 DA BB CB 34 E0 80 03 80 D8 C8 50 20 d..4...4......P 
00406D40 DD 84 73 22 6F F3 C0 C8 46 10 40 9B C6 80 03 10 ..s"o...F.@.....
00406D50 E0 80 04 80 C0 20 10 3A 36 18                   ..... .:6.     

l00406D5A:
	move	r16,r0
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
00406D78                         FD 80 90 89 F0 20 0F 3C         ..... .<
00406D80 09 92 87 84 B4 99 FF D3 87 A8 85 3F E0 80 02 80 ...........?....
00406D90 87 10 E7 83 93 36 91 80 09 C0 9B 51 CD 73 C0 04 .....6.....Q.s..
00406DA0 A8 01 28 B2 29 92 00 2A 26 63 83 1B             ..(.)..*&c..   

;; policyof: 00406DAC
policyof proc
	save	00000020,ra,00000006
	addiupc	r16,0000604F
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
is_valid_hostname proc
	save	00000010,ra,00000002
	addiu	r5,r0,000000FF
	move	r16,r4
	balc	0040A940
00406DF4             E0 00 FD 00 9F 90 87 88 04 C0 80 10     ............
00406E00 12 1F 70 BD 80 10 00 2A D6 61 FF D3 72 DA EF 1B ..p....*.a..r...
00406E10 09 92 08 5E E4 20 08 00 07 A8 F5 BF E4 80 2D 80 ...^. ........-.
00406E20 FC F3 FC C8 EB 17 00 2A 76 42 65 BA 08 5E 84 80 .......*vBe..^..
00406E30 01 50 12 1F                                     .P..           

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
	addiupc	r4,00005865
	balc	00408070
00406E56                   64 12 20 9A 00 12 40 12 D3 10       d. ...@...
00406E60 A0 00 00 02 6A 72 00 2A D6 13 04 9A 59 CA 36 80 ....jr.*....Y.6.
00406E70 52 22 10 86 60 0A E8 11 26 18 FF 2B 33 DB 00 82 R"..`...&..+3...
00406E80 0B 80 C0 17 EC C8 18 A8 01 D0 F0 20 10 80 E0 60 ........... ...`
00406E90 04 20 10 00 78 50 E0 80 0B 80 00 22 10 3E 07 12 . ..xP.....".>..
00406EA0 90 10 CA 83 E3 36 A3 D2 6A 72 00 2A C2 38 06 9A .....6..jr.*.8..
00406EB0 8A D3 45 5C C4 5F 9D 00 A9 00 04 18 86 00 01 00 ..E\._..........
00406EC0 80 0B 64 3D C4 10 17 9A 84 A4 FF 90 78 38 6D 9A ..d=........x8m.
00406ED0 E3 34 C7 20 07 21 6E 38 63 9A 6A 73 68 5E 26 02 .4. .!n8c.jsh^&.
00406EE0 01 00 04 9A 60 38 48 9A 1C D2 64 5C 9A 3C EA 72 ....`8H...d\.<.r
00406EF0 DE 10 96 3C FF 2B 59 FC 80 88 63 3F 90 C8 40 08 ...<.+Y...c?..@.
00406F00 49 92 18 5E 36 9A 3E 38 2A BA B1 12 95 84 00 20 I..^6.>8*...... 
00406F10 04 9A 32 38 22 9A 15 84 00 10 3F 0A CB FE 80 88 ..28".....?.....
00406F20 3D 3F 35 22 D0 31 C9 90 37 BE 00 2A 02 32 2F 1B =?5".1..7..*.2/.
00406F30 D1 10 A9 1B 29 92 CB 1B A9 92 D1 1B B1 12 D7 1B ....)...........
00406F40 00 82 02 80 19 1B                               ......         

;; fn00406F46: 00406F46
fn00406F46 proc
	bc	00406C30

;; dns_parse_callback: 00406F4A
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
00406FB8                         80 88 C9 BF 9D 10 FF 2B         .......+
00406FC0 27 FE 41 9A 01 16 BD 10 00 2A 94 38 B7 1B       '.A......*.8.. 

;; __lookup_name: 00406FCE
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
00406FFE                                           C4 00               ..
00407000 01 00 D5 BE 88 3B                               .....;         

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
004070EA                               24 12 80 A8 BC 00           $.....
004070F0 C0 00 00 01 E0 72 47 72 FF D0 00 2A 04 0A 04 A8 .....rGr...*....
00407100 F7 BE E0 10 E0 12 D7 22 07 31 40 BB DE 34 C7 88 .......".1@..4..
00407110 0C C0 F6 22 50 39 E7 A4 FF 90 F1 C8 04 70 1D 84 ..."P9.......p..
00407120 80 10 E0 00 FF 00 20 82 02 80 E7 AA CB FE F6 BF ...... .........
00407130 F5 22 50 F1 A0 0A F8 2F AE D3 FE 84 00 10 E0 73 ."P..../.......s
00407140 87 12 F4 84 00 20 EC 9B B4 10 0C 18 D1 C8 02 70 ..... .........p
00407150 E9 90 E9 92 B1 1B A9 90 58 5E FF 2B D3 FA 77 BA ........X^.+..w.
00407160 85 12 02 18 89 92 94 84 00 20 06 9A FF 2B C1 FA ......... ...+..
00407170 73 9A 85 8A 3E 00 E0 00 FF 00 B4 20 D0 89 E7 22 s...>...... ..."
00407180 D0 39 F1 88 BD FF F7 00 01 00 D1 10 F5 20 50 21 .9........... P!
00407190 E3 B4 FA 39 E3 34 35 3E B5 BF 27 22 87 00 C7 73 ...9.45>..'"...s
004071A0 07 11 F3 10 1F 0A 0B FB 24 12 17 9A             ........$...   

l004071AC:
	move	r20,r17
	bltc	r0,r17,00407054

l004071B2:
	bc	00406FF8
004071B4             C7 73 1E 84 00 10 07 11 B0 BE 7E BE     .s........~.
004071C0 FF 2B EF FA 24 12 84 12 80 A8 89 BE 80 A8 29 3E .+..$.........)>
004071D0 23 1A                                           #.             

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
00407222       0C D3 A0 04 18 BC A0 0A 04 2F 8A D3 F2 A4   ........./....
00407230 EC C8                                           ..             

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
00407244             82 91 80 12                             ....       

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
	balc	0040738E
00407280 62 72 FF 2B 27 FB 44 12 62 72 FF 2B B5 F9 11 D3 br.+'.D.br.+....
00407290 F2 84 13 20 C4 12 A0 60 01 00 08 00 0A D2 B2 86 ... ...`........
004072A0 12 20 E3 B4 00 2A D8 0A E4 12 04 A8 DA 80 1C D3 . ...*..........
004072B0 E0 72 FF 2B 1B EB 44 12 80 A8 C6 00 9C D3 46 73 .r.+..D.......Fs
004072C0 FD 20 50 29 C0 E3 00 08 E6 B4 FF 0A E3 F3 5A BA . P)..........Z.
004072D0 49 72 FF 2B 6D F9 C0 E0 00 0C 96 20 10 23 86 20 Ir.+m...... .#. 
004072E0 10 F2 49 72 FF 2B C5 FA E3 34 C4 84 13 20 74 DB ..Ir.+...4... t.
004072F0 C0 E0 00 02 DE 20 90 F2 80 10 4B 33 FD 80 80 8E ..... ....K3....
00407300 74 B3 FD 00 80 01 7A B3 D2 84 A4 2E A5 A4 08 90 t.....z.........
00407310 40 02 80 00 54 53 C7 F2 6C F3 B2 20 90 28 58 53 @...TS..l.. .(XS
00407320 06 BB 89 90 44 AA D3 3F 44 12 E0 0A 44 3C 0F D2 ....D..?D...D<..
00407330 30 D3 C4 22 D0 21 86 22 D0 31 84 80 10 C0 B5 82 0..".!.".1......
00407340 14 C0 6C 52 89 92 A4 22 90 22 C4 23 90 3A 20 32 ..lR...".".#.: 2
00407350 7C 52                                           |R             

;; fn00407352: 00407352
fn00407352 proc
	sw	r4,0050(sp)
	addiu	r19,r19,0000001C
	bnec	r17,r20,00407248

l0040735A:
	movep	r4,r5,r16,r17
	addiupc	r7,FFFFFCA5
	li	r6,0000001C
	balc	00409E0E
00407366                   85 34 A0 10 00 2A E2 3A 89 18       .4...*.:..

l00407370:
	li	r6,0000000C
	addiupc	r5,00005D65
	addiu	r4,sp,00000088
	balc	0040738E
0040737A                               04 D3 B3 10 65 72           ....er
00407380 FD 1A 40 12 C0 13 A3 1B 40 12 C0 13 A1 1B       ..@.....@..... 

;; fn0040738E: 0040738E
;;   Called from:
;;     00407220 (in __lookup_name)
;;     0040727E (in fn00407352)
;;     00407378 (in fn00407352)
fn0040738E proc
	bc	0040A130
00407392       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; __lookup_serv: 004073A0
;;   Called from:
;;     00405E6C (in getaddrinfo)
__lookup_serv proc
	save	00000550,ra,00000009
	move	r19,r6
	addiupc	r6,00004867
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
	lh	r4,0000(r18)
	sb	r7,0002(r18)
	beqic	r19,00000006,00407424

l004073F0:
	lsa	r18,r16,r18,00000002
	li	r7,00000001
	addiu	r16,r16,00000001
	sb	r7,0003(r18)
	li	r7,00000011
	lh	r4,0000(r18)
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
	lh	r16,0000(r18)
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
	addiupc	r4,0000556C
	balc	00408070
00407454             A4 12 36 BA FF 2B 55 D5 00 82 0B 80     ..6..+U.....
00407460 40 17 CC C8 BF AF 81 D3 C7 20 10 38 C0 60 04 20 @........ .8.`. 
00407470 10 00 E8 53 93 BB AD 1B A3 D2 20 0A F2 32 06 9A ...S...... ..2..
00407480 8A D3 45 5C C4 5F 91 10 80 0B 9C 37 1C BA C2 70 ..E\._.....7...p
00407490 D5 10 A0 00 80 00 20 0A A6 0D 04 9A 18 CA D9 17 ...... .........
004074A0 A0 0A BC 0B 00 AA 7D 3F 5F 1B 91 88 10 C0 E4 A4 ......}?_.......
004074B0 FF 90 E1 C8 08 00 E7 80 09 80 EC C8 12 28 96 20 .............(. 
004074C0 07 39 C7 80 DF 20 0E 9B E7 80 09 80 FC C8 06 28 .9... .........(
004074D0 89 90 B5 1B 29 92 98 5F C7 80 DF 20 08 9B E7 80 ....).._... ....
004074E0 09 80 EC C8 EF 2F 0A D3 C1 72 20 0A 34 2B E0 00 ...../...r .4+..
004074F0 FF FF E4 12 87 A8 97 FF 81 34 91 88 91 3F 04 D3 .........4...?..
00407500 A0 04 34 AA 00 2A D8 33 16 BA 60 CA 81 37 50 22 ..4..*.3..`..7P"
00407510 0F 3C 01 D3 09 92 77 5F 11 D3 E7 86 00 50 76 5F .<....w_.....Pv_
00407520 81 34 04 D3 A0 04 18 AA 00 2A B4 33 80 A8 5F 3F .4.......*.3.._?
00407530 60 CA 5B 8F 50 22 0F 3C 02 D3 09 92 77 5F 06 D3 `.[.P".<....w_..
00407540 E7 86 00 50 76 5F 47 1B 00 00 00 00 00 00 00 00 ...Pv_G.........

;; __netlink_enumerate: 00407550
__netlink_enumerate proc
	save	00000020,ra,00000007
	illegal
	movep	r23,r23,r7,r8
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
	li	r7,00000040
	addiu	r6,r0,00002000
	move	r5,sp
	move.balc	r4,r17,00407640
	move	r20,r4
	bgec	r0,r4,004075C6
	move	r16,sp
	addu	r7,sp,r20
	subu	r7,r7,r16
	bltiuc	r7,00000010,0040758C
	lhu	r7,0004(r16)
	beqic	r7,00000003,004075CE
	beqic	r7,00000002,004075C6
	movep	r4,r5,r19,r16
	jalrc	ra,r18
	bnezc	r4,004075C8
	lw	r7,0000(r16)
	addiu	r7,r7,00000003
	ins	r7,r0,00000000,00000001
	addu	r16,r16,r7
	bc	004075A0
	li	r4,FFFFFFFF
	addiu	sp,sp,00002000
	restore.jrc	00000020,ra,00000007
	move	r4,r0
	bc	004075C8

;; __rtnetlink_enumerate: 004075D2
__rtnetlink_enumerate proc
	save	00000020,ra,00000007
	movep	r21,r20,r4,r5
	movep	r18,r19,r6,r7
	li	r5,00080003
	move	r6,r0
	li	r4,00000010
	li	r16,FFFFFFFF
	balc	00407D80
004075E8                         24 12 04 A8 24 80 33 11         $...$.3.
004075F0 12 D3 5D BF 81 D2 FF 2B 57 FF 04 12 0E BA 33 11 ..]....+W.....3.
00407600 16 D3 5C BF 82 D2 3F 0A 47 FF 04 12 39 D2 3F 0B ..\...?.G...9.?.
00407610 3F D4 90 10 27 1F 00 00 00 00 00 00 00 00 00 00 ?...'...........

;; ntohl: 00407620
;;   Called from:
;;     0040193C (in fn0040193C)
ntohl proc
	illegal
	jrc	ra
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00008084

;; ntohs: 00407630
;;   Called from:
;;     004030A4 (in fn004030A4)
ntohs proc
	illegal
	andi	r4,r4,0000FFFF
	jrc	ra
	sigrie	00000000
	sigrie	00000000

;; recv: 00407640
recv proc
	move	r9,r0
	move	r8,r0
	bc	00407650
00407648                         00 00 00 00 00 00 00 00         ........

;; recvfrom: 00407650
;;   Called from:
;;     00407644 (in recv)
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
00407666                   E1 83 12 30 00 28 C2 55 00 00       ...0.(.U..

;; recvmsg: 00407670
recvmsg proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000D4
	balc	0040ADA4
00407684             E1 83 12 30 00 28 A4 55 00 00 00 00     ...0.(.U....

;; res_mkquery: 00407690
;;   Called from:
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
004076AA                               24 12 10 9A C4 80           $.....
004076B0 01 80 C6 23 07 39 E7 80 2E 10 E6 20 10 8A 20 22 ...#.9..... .. "
004076C0 90 3B 11 02 11 00 80 B3 E0 00 FD 00 70 12 27 8A .;..........p.'.
004076D0 08 C0 FF D1 93 10 CA 83 53 31 12 AA F5 BF AC CA ........S1......
004076E0 F1 87 E0 00 FF 00 C7 AA E9 FF E7 AA E5 FF 42 71 ..............Bq
004076F0 B5 82 03 C0 03 BF A9 92 40 0A 94 2F 01 D3 DD 84 ........@../....
00407700 0D 10 BE 10 D1 10 9D 00 15 00 BD 86 0A 10 00 2A ...............*
00407710 1E 2A 0D D3 FD 80 E0 8E 7E B3 A7 84 E8 2E AA 9A .*......~.......
00407720 E6 10 02 18 E9 90 47 22 07 29 84 9A B1 C8 F5 77 ......G".).....w
00407730 7B B3 85 80 01 80 8D C8 99 FF 9D 80 E0 8E 4C B3 {.............L.
00407740 A6 84 E7 1E C7 00 01 00 CB 1B BD 10 80 10 E7 86 ................
00407750 E9 1E C7 86 EB 1E 00 2A 9A 37 C1 34 54 BE E6 80 .......*.7.4T...
00407760 50 C0 7E B3 FD F3 C7 80 88 C0 FD 84 09 10 DD 84 P.~.............
00407770 08 10 D0 10 00 2A B8 29 5B 1B 00 00 00 00 00 00 .....*.)[.......

;; mtime: 00407780
mtime proc
	save	00000020,ra,00000001
	move	r4,r0
	addiu	r5,sp,00000008
	balc	0040AEF4
0040778A                               E2 34 80 00 E8 03           .4....
00407790 C0 60 40 42 0F 00 EC 3C A3 34 C5 20 18 21 78 B2 .`@B...<.4. .!x.
004077A0 21 1F                                           !.             

;; cleanup: 004077A2
cleanup proc
	move	r5,r4
	li	r4,00000039
	bc	00404A50

;; __res_msend_rc: 004077AA
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
	lw	r7,0054(r30)
	bltuc	r20,r7,00407850
	move	r6,r0
	li	r5,00080081
	sh	r17,0050(sp)
	move.balc	r4,r17,00407D80
	move	r16,r4
	bgec	r4,r0,00407836
	bneiuc	r17,0000000A,00407846
	balc	004049B0
	lw	r7,0000(r4)
	bneiuc	r7,00000021,00407846
	move	r6,r0
	li	r5,00080081
	li	r4,00000002
	balc	00407D80
	move	r16,r4
	bltc	r4,r0,00407846
	li	r17,00000002
	move	r6,r19
	addiu	r5,sp,00000050
	move.balc	r4,r16,00405DB0
	bgec	r4,r0,00407898
	move.balc	r4,r16,0040AF72
	lw	r17,0030(sp)
	move	r5,r0
	balc	00407A82
	li	r4,FFFFFFFF
	restore.jrc	000000F0,r30,0000000A
	lw	r7,-0008(r23)
	bneiuc	r7,00000002,00407878
	li	r6,00000004
	addu	r4,r16,r6
	sw	r7,0018(sp)
	move.balc	r5,r23,0040A130
	li	r4,00000035
	balc	00406700
	lw	r17,0018(sp)
	sh	r4,0002(r16)
	lh	r7,0000(r16)
	addiu	r20,r20,00000001
	addiu	r16,r16,0000001C
	addiu	r23,r23,0000001C
	bc	004077F4
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
	lh	r7,0000(r16)
	bc	0040786E
	addiupc	r5,FFFFFF83
	addiu	r4,sp,00000044
	move	r6,r16
	balc	0040AE32
	lw	r17,0030(sp)
	move	r5,r0
	balc	00407A82
	beqic	r17,0000000A,0040791A
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
	lw	r17,000C(sp)
	subu	r7,r10,r22
	bgeuc	r7,r6,00407A76
	subu	r7,r10,r30
	bltuc	r7,r23,004078FE
	move	r21,r0
	bltc	r21,r18,0040795E
	lw	r5,001C(sp)
	move	r30,r10
	subu	r6,r23,r10
	li	r5,00000001
	addu	r6,r6,r30
	addiu	r4,sp,0000003C
	balc	00407E20
	bltc	r0,r4,004079EE
	balc	00407780
	move	r10,r4
	bc	004078E2
	addiu	r8,r0,00000004
	addiu	r7,sp,00000034
	li	r6,0000001A
	li	r5,00000029
	addiu	r17,sp,00000070
	move	r23,r0
	sw	r0,0034(sp)
	move.balc	r4,r16,00407D60
	beqc	r20,r23,004078AE
	lhu	r7,-0004(r17)
	bneiuc	r7,00000002,00407958
	li	r6,00000004
	addiu	r4,r17,00000010
	move.balc	r5,r17,0040A130
	li	r6,0000000C
	addiupc	r5,00005AC0
	addiu	r4,r17,00000004
	balc	0040A130
	li	r7,0000000A
	sh	r7,-0004(r17)
	sw	r0,0040(sp)
	sw	r0,0054(sp)
	addiu	r23,r23,00000001
	addiu	r17,r17,0000001C
	bc	0040792E
	lw	r17,0004(sp)
	lwxs	r7,r21(r7)
	beqzc	r7,00407992
	addiu	r21,r21,00000001
	bc	004078F6
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
	lw	r17,0018(sp)
	bnec	r7,r30,0040796A
	bc	00407966
	addiu	r7,sp,0000006C
	move	r30,r7
	bc	0040798A
	addiu	r8,r8,00000001
	bc	00407A18
	addiu	r7,r7,00000001
	bgec	r7,r18,004079EA
	lw	r17,0008(sp)
	lw	r5,0000(r10)
	lwxs	r6,r7(r6)
	lbu	r11,0000(r5)
	lbu	r4,0000(r6)
	bnec	r11,r4,0040799C
	lbu	r5,0001(r5)
	lbu	r6,0001(r6)
	bnec	r6,r5,0040799C
	lw	r17,0004(sp)
	sll	r4,r7,00000002
	addu	r11,r6,r4
	lw	r6,0000(r11)
	bnezc	r6,004079EE
	lw	r5,0000(r10)
	lbu	r6,0003(r5)
	andi	r6,r6,0000000F
	beqic	r6,00000002,00407A40
	beqic	r6,00000003,004079D6
	bnezc	r6,004079EE
	sw	r9,0000(r11)
	bnec	r7,r17,00407A66
	bgec	r17,r18,00407A72
	lw	r17,0004(sp)
	lwxs	r7,r17(r7)
	beqzc	r7,004079EE
	addiu	r17,r17,00000001
	bc	004079DC
	bnec	r7,r18,004079BA
	lw	r17,0014(sp)
	lsa	r10,r17,r7,00000002
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
	bltic	r9,00000004,004079F4
	move	r8,r0
	sw	r10,0028(sp)
	sw	r9,002C(sp)
	beqc	r20,r8,004079EE
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
	move	r7,r17
	bc	0040799E
	beqc	r0,r21,004079EE
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
	lw	r17,0014(sp)
	move	r6,r9
	lwx	r4,r4(r7)
	balc	0040A130
	bnec	r18,r17,004079EE
	addiu	r4,sp,00000044
	li	r5,00000001
	balc	0040AE3A
	move	r4,r0
	restore.jrc	000000F0,r30,0000000A
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
00407A9A                               7F D3 04 A8 12 80           ......
00407AA0 C7 73 21 35 47 11 02 35 E3 34 51 BF 1F 0A FB FC .s!5G..5.4Q.....
00407AB0 C4 10 86 10 94 1F 00 00 00 00 00 00 00 00 00 00 ................

;; res_send: 00407AC0
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
00407ADA                               04 A8 02 80 80 34           .....4
00407AE0 21 1F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 !...............

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
	addiupc	r4,0000522F
	balc	00408070
00407B36                   20 12 64 12 50 BA FF 2B 71 CE        .d.P..+q.
00407B40 40 17 7F D2 CC C8 22 A8 81 D3 C7 20 10 38 C0 60 @.....".... .8.`
00407B50 04 20 10 00 E8 53 92 9B C0 10 A0 04 46 A4 81 D0 . ...S......F...
00407B60 1F 0A ED EF 80 10 30 86 54 90 E8 83 C3 32 8A D2 ......0.T....2..
00407B70 68 72 00 2A FA 2B 2A BA B0 17 F4 C8 24 20 60 0A hr.*.+*.....$ `.
00407B80 3E 0A 80 C8 06 50 FF D3 87 A8 F3 3F D3 10 A0 00 >....P.....?....
00407B90 00 01 68 72 00 2A A8 06 55 BA 60 0A C2 04 C5 B8 ..hr.*..U.`.....
00407BA0 B7 1B 07 D3 A0 04 08 A4 68 72 54 39 80 A8 B6 00 ........hrT9....
00407BB0 9D 84 A7 20 FD 80 60 8D C7 12 4C 39 80 88 A6 00 ... ..`...L9....
00407BC0 A0 04 F4 A3 68 72 3C 39 2C 9A E4 84 06 20 E7 80 ....hr<9,.... ..
00407BD0 30 80 EC C8 20 50 44 02 06 00 0A D3 C3 72 40 0A 0... PD......r@.
00407BE0 40 24 F6 84 6C 8D A7 DB E4 80 10 50 0F D3 E6 20 @$..l......P... 
00407BF0 10 22 90 84 5C 90 A0 04 C6 A3 68 72 06 39 2A 9A ."..\.....hr.9*.
00407C00 E4 84 09 20 E7 80 30 80 EC C8 1E 50 44 02 09 00 ... ..0....PD...
00407C10 0A D3 C3 72 40 0A 0A 24 E3 34 A7 DB E4 80 0B 50 ...r@..$.4.....P
00407C20 0A D3 E6 20 10 22 90 84 58 90 A0 04 9E A3 68 72 ... ."..X.....hr
00407C30 D2 38 80 88 57 3F E4 84 08 20 C7 80 30 80 DC C8 .8..W?... ..0...
00407C40 04 50 F1 C8 47 77 42 91 0A D3 C3 72 40 0A D2 23 .P..GwB....r@..#
00407C50 E3 34 F2 88 37 3F E4 80 3D 50 3C D3 E6 20 10 22 .4..7?..=P<.. ."
00407C60 90 84 60 90 27 1B 0A D3 A0 04 6C A3 68 72 90 38 ..`.'.....l.hr.8
00407C70 42 BA 9D 84 AA 20 90 38 3A 9A BD 00 AB 00 28 CA B.... .8:.....(.
00407C80 0B 1F 58 5E 82 38 1E BA A3 B4 C3 34 68 5E 1A BA ..X^.8.....4h^..
00407C90 1C D2 64 5C 99 3C C0 10 08 B2 FF 2B B3 EE 80 88 ..d\.<.....+....
00407CA0 EB BE 29 92 E7 1A A9 90 D9 1B 5C 38 63 BA C9 90 ..).......\8c...
00407CB0 C3 B4 D7 1B 80 8A D5 3E 06 D3 A0 04 26 A3 68 72 .......>....&.hr
00407CC0 3E 38 28 BA 9D 84 A6 20 5D 02 A7 00 3A 38 80 88 >8(.... ]...:8..
00407CD0 BB 3E 28 5E 32 38 24 BA 40 0A B4 2B A4 8A AD FE .>(^28$.@..+....
00407CE0 C4 00 01 00 54 BE 00 2A 46 24 A1 1A 06 D3 A0 04 ....T..*F$......
00407CF0 FA A2 68 72 0A 38 80 A8 93 3E C9 1B 49 92 D3 1B ..hr.8...>..I...

;; fn00407D00: 00407D00
fn00407D00 proc
	bc	0040A8E0

;; fn00407D04: 00407D04
fn00407D04 proc
	bc	0040AC28

;; fn00407D08: 00407D08
fn00407D08 proc
	bc	00407AF0
00407D0C                                     00 00 00 00             ....

;; send: 00407D10
send proc
	move	r9,r0
	move	r8,r0
	bc	00407D40
00407D18                         00 00 00 00 00 00 00 00         ........

;; sendmsg: 00407D20
sendmsg proc
	save	00000010,ra,00000001
	move	r10,r0
	move	r9,r0
	movep	r7,r8,r6,r0
	move	r6,r5
	move	r5,r4
	addiu	r4,r0,000000D3
	balc	0040ADA4
00407D34             E1 83 12 30 00 28 F4 4E 00 00 00 00     ...0.(.N....

;; sendto: 00407D40
;;   Called from:
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
00407D56                   E1 83 12 30 00 28 D2 4E 00 00       ...0.(.N..

;; setsockopt: 00407D60
;;   Called from:
;;     00400B4A (in fn00400B4A)
;;     00400CD0 (in fn00400CBE)
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
00407D76                   E1 83 12 30 00 28 B2 4E 00 00       ...0.(.N..

;; socket: 00407D80
;;   Called from:
;;     0040671E (in if_indextoname)
;;     0040676E (in if_nametoindex)
;;     004075E4 (in __rtnetlink_enumerate)
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
00407D96                   00 2A 96 4E 04 12 04 88 60 80       .*.N....`.
00407DA0 FF 2B 0D CC C0 17 F0 C8 4C B0 E0 60 80 00 08 00 .+......L..`....
00407DB0 98 53 CC 9B C0 60 7F FF F7 FF 40 11 20 11 18 53 .S...`....@. ..S
00407DC0 7B BD 80 00 C6 00 5F 0B 87 CC 00 2A 62 4E 04 12 {....._....*bN..
00407DD0 04 A8 2C 80 24 CA 0C 98 A4 10 81 D3 02 D3 19 D2 ..,.$...........
00407DE0 FF 2B 6D CC 24 CA 18 38 E0 00 00 08 04 D3 19 D2 .+m.$..8........
00407DF0 1F 0B 5D CC 0A 18 FF 2B B7 CB C0 17 E2 C8 AB EF ..]....+........
00407E00 90 10 25 1F 00 00 00 00 00 00 00 00 00 00 00 00 ..%.............

;; sched_yield: 00407E10
sched_yield proc
	save	00000010,ra,00000001
	li	r4,0000007C
	balc	00404A50
00407E18                         E1 83 12 30 00 28 10 4E         ...0.(.N

;; poll: 00407E20
poll proc
	save	00000020,ra,00000001
	move	r7,r0
	bltc	r6,r0,00407E42

l00407E28:
	addiu	r8,r0,000003E8
	div	r7,r6,r8
	sw	r7,0008(sp)
	illegal
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
00407E54             00 2A D8 4D 21 1F 00 00 00 00 00 00     .*.M!.......

;; _longjmp: 00407E60
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
_setjmp proc
	sw	ra,0000(r4)
	sw	sp,0004(r4)
	swm	r16,0008(r4),00000008
	sw	r30,0028(r4)
	sw	r28,002C(r4)
	li	r4,00000000
	jrc	ra
	nop
	nop

;; __block_all_sigs: 00407EA0
__block_all_sigs proc
	move	r7,r4
	addiu	r8,r0,00000008
	addiupc	r6,0000581F
	move	r5,r0
	addiu	r4,r0,00000087
	bc	00404A50

;; __block_app_sigs: 00407EB4
;;   Called from:
;;     00407EE6 (in raise)
__block_app_sigs proc
	move	r7,r4
	addiu	r8,r0,00000008
	addiupc	r6,0000580D
	move	r5,r0
	addiu	r4,r0,00000087
	bc	00404A50

;; __restore_sigs: 00407EC8
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
00407EEA                               80 00 B2 00 FF 2B           .....+
00407EF0 5F CB 04 BF 80 00 82 00 FF 2B 55 CB 00 2A 30 4D _........+U..*0M
00407F00 04 12 9D 10 FF 2B C1 FF 90 10 92 1F 00 00 00 00 .....+..........

;; setitimer: 00407F10
setitimer proc
	save	00000010,ra,00000001
	move	r7,r6
	move	r6,r5
	move	r5,r4
	li	r4,00000067
	balc	00404A50
00407F1E                                           E1 83               ..
00407F20 12 30 00 28 0A 4D 00 00 00 00 00 00 00 00 00 00 .0.(.M..........

;; __get_handler_set: 00407F30
__get_handler_set proc
	li	r6,00000008
	addiupc	r5,00015525
	bc	0040A130

;; __libc_sigaction: 00407F3A
;;   Called from:
;;     00408020 (in __sigaction)
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
	addiupc	r5,00015516
	srl	r6,r7,00000005
	lsa	r6,r6,r5,00000002
	li	r5,00000001
	sllv	r5,r5,r7
	illegal
	illegal
	or	r7,r7,r5
	illegal
	beqzc	r7,00407F64
	illegal
	addiupc	r7,0002625C
	lw	r7,0004(r7)
	bnezc	r7,00407FA4
	lwpc	r7,00432988
	bnezc	r7,00407FA4
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
	illegal
	addiu	r0,r2,00001790

l00407FA4:
	lw	r7,0000(r17)
	lui	r6,00004000
	addiupc	r5,00002909
	addiu	r4,sp,00000010
	sw	r7,0008(sp)
	lw	r7,0084(r17)
	or	r6,r6,r7
	andi	r7,r7,00000004
	sw	r6,000C(sp)
	addiupc	r6,00002900
	movn	r6,r5,r7

l00407FC4:
	addiu	r5,r17,00000004
	move	r7,r6
	li	r6,00000008
	sw	r7,0018(sp)
	balc	0040A130
00407FD0 42 73                                           Bs             

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
00407FE8                         FF D3 1A BA E0 10 16 98         ........
00407FF0 E7 34 08 D3 C9 72 01 92 80 97 E8 34 F0 84 84 90 .4...r.....4....
00408000 00 2A 2C 21 E0 10 87 10 44 1F                   .*,!....D.     

;; __sigaction: 0040800A
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
00408028                         96 D3 C0 97 7F D2 11 1F         ........

;; sigemptyset: 00408030
sigemptyset proc
	sw	r0,0000(sp)
	sw	r0,0004(sp)
	move	r4,r0
	jrc	ra
00408038                         00 00 00 00 00 00 00 00         ........

;; sigprocmask: 00408040
sigprocmask proc
	save	00000010,ra,00000002
	balc	0040AE70
00408046                   04 12 04 BA 90 10 12 1F FF 2B       .........+
00408050 5F C9 04 F6 7F D0 F3 1B 00 00 00 00 00 00 00 00 _...............

;; __fclose_ca: 00408060
__fclose_ca proc
	lw	r7,000C(r4)
	jrc	r7
00408064             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __fopen_rb_ca: 00408070
;;   Called from:
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
00408080 A0 80 64 80 E8 E0 00 80 D3 10 38 D2 FF 2B C1 C9 ..d.......8..+..
00408090 00 2A 9C 4B A4 10 0F 96 04 A8 32 80 81 D3 02 D3 .*.K......2.....
004080A0 19 D2 22 91 FF 2B A9 C9 89 D3 80 97 E0 04 50 00 .."..+........P.
004080B0 88 97 E0 04 BA 00 8A 97 E0 04 26 00 38 92 83 97 ..........&.8...
004080C0 FF D3 0B 95 8C 94 F0 84 4C 90 90 10 25 1F 00 12 ........L...%...
004080D0 F9 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

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
004080EA                               A4 10 39 D2 FF 2B           ..9..+
004080F0 5F C9 E1 83 12 30 00 28 36 4B 00 00 00 00 00 00 _....0.(6K......

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
00408124             00 2A 08 4B 80 A8 12 80 E4 80 30 20     .*.K......0 
00408130 C7 80 10 10 80 17 EC 53 80 97 24 12 22 18 C1 34 .......S..$."..4
00408140 86 88 F7 FF 8B 17 49 B3 0C 17 78 B2 81 97 02 96 ......I...x.....
00408150 0E 9B C7 00 01 00 A4 B0 01 97 F8 5F F2 A4 FF 88 ..........._....
00408160 91 10 24 1F 00 00 00 00 00 00 00 00 00 00 00 00 ..$.............

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
00408184             00 2A A8 4A 04 88 08 80 7F D3 FF D3     .*.J........
00408190 DD A4 08 2C 9D A4 08 24 21 1F 00 00 00 00 00 00 ...,...$!.......

;; fflush_unlocked: 004081A0
;;   Called from:
;;     004033F2 (in fn004033F2)
fflush_unlocked proc
	save	00000010,ra,00000003
	move	r16,r4
	bnezc	r4,004081DE

l004081A6:
	lwpc	r7,00430300
	move	r17,r0
	beqzc	r7,004081BA
	lwpc	r4,00430300
	balc	004081A0
	move	r17,r4
	balc	00408610
	lw	r16,0000(r4)
	bc	004081C4
	lw	r16,0038(r16)
	beqzc	r16,004081D6
	lw	r7,0014(r16)
	lw	r6,001C(r16)
	bgeuc	r6,r7,004081C2
	move.balc	r4,r16,004081A0
	or	r17,r17,r4
	bc	004081C2
	balc	00408620
	move	r4,r17
	restore.jrc	00000010,ra,00000003

l004081DE:
	lw	r7,004C(r4)
	move	r17,r0
	bltc	r7,r0,004081EE

l004081E8:
	balc	0040D1D0
004081EC                                     24 12                   $. 

l004081EE:
	lw	r7,0014(r16)
	lw	r6,001C(r16)
	bgeuc	r6,r7,0040820C
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r0,r0
	jalrc	ra,r7
	lw	r7,0014(r16)
	bnezc	r7,0040820C
	beqzc	r17,00408208
	move.balc	r4,r16,0040D210
	li	r17,FFFFFFFF
	bc	004081DA
	lwm	r6,0004(r16),00000002
	bgeuc	r6,r7,00408224
	subu	r6,r6,r7
	lw	r5,0028(r16)
	addiu	r8,r0,00000001
	sra	r7,r6,0000001F
	move	r4,r16
	jalrc	ra,r5
	sw	r0,0004(sp)
	sw	r0,0008(sp)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,001C(sp)
	beqzc	r17,004081DA
	move	r17,r0
	move.balc	r4,r16,0040D210
	bc	004081DA
	sigrie	00000000
	sigrie	00000000

;; fgets_unlocked: 00408240
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
00408284             02 15 A5 B3 4C 18 00 0A 52 51 04 88     ....L...RQ..
00408290 1A 80 71 8A 76 00 80 17 E4 C8 70 20             ..q.v.....p    

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
004082AC                                     F1 00 01 00             ....
004082B0 4C F2 14 5E 9F 92 27 12 80 C8 E1 57             L..^..'....W   

l004082BC:
	beqc	r0,r20,0040829C

l004082C0:
	lw	r4,0004(r16)
	li	r5,0000000A
	lw	r6,0008(r16)
	subu	r6,r6,r4
	balc	0040A050
004082CC                                     81 17 C4 12             ....
004082D0 33 9A C5 B3 49 92 54 22 90 3B 81 16 F4 20 10 96 3...I.T".;... ..
004082E0 91 10 D2 10 12 B1 00 2A 46 1E 81 17 54 22 D0 A1 .......*F...T"..
004082F0 7E B1 81 97 C0 AA A5 3F 80 8A A1 3F 02 17 C7 88 ~......?...?....
00408300 89 FF C7 00 01 00 01 97 78 5E A1 1B 60 12 91 1B ........x^..`...

;; flockfile: 00408310
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
0040832E                                           E5 1B               ..

l00408330:
	restore.jrc	00000010,ra,00000002
00408332       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; fprintf: 00408340
;;   Called from:
;;     00400B5E (in fn00400B5E)
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
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00001E25

;; fputc: 00408380
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
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)
	move.balc	r4,r16,0040D210
	bc	004083D6

l004083C6:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,00408398
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)
	move	r4,r17
	restore.jrc	00000020,ra,00000005

l004083DA:
	movep	r4,r5,r16,r18
	balc	0040D250
004083E0 24 12 DD 1B 00 00 00 00 00 00 00 00 00 00 00 00 $...............

;; fputs_unlocked: 004083F0
;;   Called from:
;;     00400B5A (in fn00400B5A)
fputs_unlocked proc
	save	00000010,ra,00000004
	movep	r17,r18,r4,r5
	balc	0040A890
004083F8                         81 D2 04 12 58 BE 20 0A         ....X. .
00408400 7A 01 04 52 80 20 90 23 80 20 D0 21 14 1F 00 00 z..R. .#. .!....

;; __do_orphaned_stdio_locks: 00408410
__do_orphaned_stdio_locks proc
	illegal
	lw	r7,-000C(r3)
	bc	0040842E
	illegal
	lui	r6,00040000
	sw	r6,004C(r7)
	illegal
	lw	r7,0084(r7)
	bnezc	r7,0040841A
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
	illegal
	lw	r7,0040(r4)
	sw	r7,-000C(r3)

l0040845A:
	jrc	ra

;; ftrylockfile: 0040845C
;;   Called from:
;;     00408314 (in flockfile)
ftrylockfile proc
	move	r7,r4
	illegal
	lw	r6,004C(r4)
	lw	r8,-0094(r3)
	bnec	r8,r6,00408488
	lw	r6,0044(r4)
	li	r5,7FFFFFFF
	bnec	r5,r6,0040847E
	li	r4,FFFFFFFF
	jrc	ra
	addiu	r6,r6,00000001
	move	r4,r0
	sw	r6,0044(r7)
	jrc	ra
	lw	r6,004C(r4)
	bgec	r6,r0,00408494
	sw	r0,004C(r4)
	lw	r6,004C(r7)
	bnezc	r6,0040847A
	addiu	r5,r7,0000004C
	illegal
	illegal
	bnezc	r4,004084B0
	move	r6,r8
	illegal
	beqzc	r6,004084A2
	illegal
	bnezc	r4,0040847A
	li	r6,00000001
	sw	r0,0080(r7)
	sw	r6,0044(r7)
	lw	r6,-000C(r3)
	sw	r6,0084(r7)
	beqzc	r6,004084CE
	sw	r7,0080(r6)
	sw	r7,-000C(r3)
	jrc	ra
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

;; funlockfile: 004084E0
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
004084F8                         90 10 10 84 44 90 E2 83         ....D...
00408500 12 30 00 28 0A 4D 00 00 00 00 00 00 00 00 00 00 .0.(.M..........

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
0040853A                               F0 84 4B 00 32 12           ..K.2.
00408540 07 88 06 80 20 12 22 18 27 12 9E 98 F1 80 01 80 .... .".'.......
00408550 67 22 07 31 D0 C8 F1 57 89 17 90 10 33 BF F0 D8 g".1...W....3...
00408560 24 8A 02 C0 25 1F B6 B0 A5 B0 05 16 53 BF 00 2A $...%.......S..*
00408570 BE 1B 85 17 A8 B0 7E B1 85 97 25 1F             ......~...%.   

;; fwrite_unlocked: 0040857C
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
	illegal

l004085B2:
	move	r4,r16
	restore.jrc	00000020,ra,00000007
004085B6                   00 00 00 00 00 00 00 00 00 00       ..........

;; _IO_getc: 004085C0
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
004085E2       69 9A 81 17 02 17 C7 88 1A C0 C7 00 01 00   i.............
004085F0 01 97 F8 5C 00 0A 18 4C 08 18                   ...\...L..     

l004085FA:
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r17,0000(r7)
	move	r4,r17
	restore.jrc	00000010,ra,00000003
00408606                   00 0A D6 4D 24 12 E7 1B 00 00       ...M$.....

;; __ofl_lock: 00408610
;;   Called from:
;;     0040E722 (in __stdio_exit_needed)
__ofl_lock proc
	save	00000010,ra,00000001
	addiupc	r4,000151C5
	balc	0040AD30
0040861A                               82 04 8A A3 11 1F           ......

;; __ofl_unlock: 00408620
__ofl_unlock proc
	addiupc	r4,000151BE
	bc	0040AD60
00408628                         00 00 00 00 00 00 00 00         ........

;; perror: 00408630
;;   Called from:
;;     00400C92 (in fn00400D36)
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
	move.balc	r4,r16,0040D1D0
	move	r19,r4
	beqzc	r17,00408674
	lbu	r7,0000(r17)
	beqzc	r7,00408674
	move.balc	r4,r17,0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r17,0040857C
	li	r4,0000003A
	move.balc	r5,r16,00408380
	li	r4,00000020
	move.balc	r5,r16,00408380
	move.balc	r4,r18,0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r18,0040857C
	li	r4,0000000A
	move.balc	r5,r16,00408380
	beqzc	r19,004086B2
	move	r4,r16
	restore	00000020,ra,00000005
	bc	0040D210
	move	r19,r0
	bnezc	r17,00408656
	balc	0040A890
	move	r7,r16
	move	r5,r4
	li	r6,00000001
	move.balc	r4,r18,0040857C
	move	r5,r16
	li	r4,0000000A
	restore	00000020,ra,00000005
	bc	00408380
	restore.jrc	00000020,ra,00000005
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

;; printf: 004086C0
;;   Called from:
;;     004013C6 (in pr_options)
;;     00401594 (in fn00401594)
;;     004029C6 (in fn004029C6)
;;     00402E76 (in pr_icmph)
;;     0040309C (in fn0040309C)
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
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)
	move.balc	r4,r16,0040D210
	bc	00408756

l00408746:
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,00408718
	addiu	r6,r7,00000001
	sw	r6,0014(sp)
	sb	r19,0000(r7)
	move	r4,r17
	restore.jrc	00000020,ra,00000005

l0040875A:
	movep	r4,r5,r16,r18
	balc	0040D250
00408760 24 12 DD 1B 00 00 00 00 00 00 00 00 00 00 00 00 $...............

;; putchar: 00408770
;;   Called from:
;;     00401766 (in fn00401766)
;;     00402A96 (in fn00402A96)
;;     004033EE (in fn004033EE)
putchar proc
	lwpc	r5,00412EF4
	bc	00408380
	sigrie	00000000
	addiu	r0,r0,00001E14

;; puts: 00408780
;;   Called from:
;;     00401762 (in pr_iph)
puts proc
	save	00000010,ra,00000004
	lwpc	r16,00412EF4
	move	r17,r4
	lw	r7,004C(r16)
	move	r18,r0
	bltc	r7,r0,0040879A
	move.balc	r4,r16,0040D1D0
	move	r18,r4
	movep	r4,r5,r17,r16
	balc	004083F0
	li	r17,00000001
	bltc	r4,r0,004087C0
	lb	r7,004B(r16)
	beqic	r7,0000000A,004087CE
	lwm	r6,0010(r16),00000002
	bgeuc	r7,r6,004087CE
	addu	r6,r7,r17
	move	r17,r0
	sw	r6,0014(sp)
	li	r6,0000000A
	sb	r6,0000(r7)
	subu	r17,r0,r17
	beqzc	r18,004087CA
	move.balc	r4,r16,0040D210
	move	r4,r17
	restore.jrc	00000010,ra,00000004
	li	r5,0000000A
	move.balc	r4,r16,0040D250
	srl	r17,r4,0000001F
	bc	004087C0
	sigrie	00000000
	addiu	r0,r0,0000D302

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
;;     0040684A (in inet_ntoa)
;;     0040688E (in inet_ntop)
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
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

;; sprintf: 00408860
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
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,000083BD

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
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00001E14

;; fmt_u: 004088E0
;;   Called from:
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
004088F2       3F 92 84 00 30 00 14 5E 0A D3 50 BE E0 10   ?...0..^..P...
00408900 00 2A AC 61 50 FE                               .*.aP.         

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
	illegal

l0040891E:
	bnezc	r16,0040890A

l00408920:
	move	r4,r17
	restore.jrc	00000010,ra,00000004

;; getint: 00408924
;;   Called from:
;;     00408D84 (in printf_core)
;;     00408DD6 (in printf_core)
getint proc
	move	r7,r0
	bc	0040892E

l00408928:
	li	r7,FFFFFFFF
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
	illegal
	illegal
	bltc	r5,r6,00408928
	li	r5,0000000A
	mul	r7,r7,r5
	addu	r7,r6,r7
	bc	0040892A

l0040895E:
	move	r4,r7
	jrc	ra

;; out: 00408962
;;   Called from:
;;     00408C08 (in printf_core)
;;     00408FEA (in printf_core)
;;     0040900C (in printf_core)
;;     00409802 (in fn004097B0)
;;     004098E0 (in fn004098D8)
;;     004099A2 (in fn004099A0)
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
;;     00408E0E (in printf_core)
;;     004099BE (in printf_core)
pop_arg proc
	bgeiuc	r5,0000001D,00408A76

l0040897A:
	addiu	r5,r5,FFFFFFF7
	bgeiuc	r5,00000012,00408A76

l00408982:
	addiupc	r7,000052B9
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
;;     00408FE4 (in printf_core)
;;     00408FFC (in printf_core)
;;     00409006 (in printf_core)
;;     0040901C (in printf_core)
;;     004097FA (in fn004097B0)
;;     00409814 (in fn004097B0)
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
00408B50 0C 18 C9 90 BD 10 10 82 00 81 3F 0A 05 FE C0 00 ..........?.....
00408B60 FF 00 06 AA ED FF 2C F3 BD 10 3F 0A F5 FD       ......,...?... 

l00408B6E:
	restore.jrc	00000110,ra,00000004

;; printf_core: 00408B72
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

l00408B86:
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
	illegal
	illegal
	li	r5,0000000A
	swxs	r5,r6(r4)
	lbu	r6,0001(r7)
	addiu	r7,r7,00000003
	lw	r17,0028(sp)
	illegal
	restore.jrc	000000F0,ra,0000000F
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
	illegal
	illegal
	li	r5,0000000A
	swxs	r5,r6(r4)
	lbu	r6,0002(r7)
	addiu	r7,r7,00000004
	lw	r17,0028(sp)
	illegal
	restore.jrc	000000F0,ra,0000000F
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
	addiupc	r5,000051AC
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
00408D3E                                           96 D3               ..
00408D40 C0 97 FF D3 00 28 84 0C                         .....(..       

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
00408D88                         80 B4 04 88 09 BF               ...... 

;; fn00408D8E: 00408D8E
;;   Called from:
;;     00408B90 (in printf_core)
;;     00408C00 (in printf_core)
;;     00408FDA (in printf_core)
;;     004097C0 (in fn004097B0)
;;     004097D8 (in fn004097B0)
;;     004097EC (in fn004097B0)
fn00408D8E proc
	balc	004049B0
00408D92       CB D3 AB 1B                                 ....         

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
00408DDA                               C0 03 01 00 44 12           ....D.
00408DE0 01 1B                                           ..             

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
	addiupc	r6,00005081
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
004090C0 22 C0 00 01 00 20 C0 34 F3 10 11 21 10 43 A0 D2 ".... .4...!.C..
004090D0 1F 0A 4F FA E0 34 67 22 50 B3 D3 22 10 3E 50 1A ..O..4g"P..".>P.
004090E0 36 B2 AF 1B B5 74 DB 9A 54 72 AC 92 00 2A 00 3F 6....t..Tr...*.?
004090F0 84 3E C4 10 93 AA CB FF D4 72 1F 0A 65 F8 BF 1B .>.......r..e...
00409100 C0 8B 04 00 12 A8 87 BC F7 34 B6 36 C7 80 5F C0 .........4.6.._.
00409110 15 B4 C4 B4 0E 9B E1 60 00 00 00 80 C0 04 E8 8E .......`........
00409120 C6 B4 1C 18 34 CA 0E 5A 91 F2 A0 20 90 33 C4 B4 ....4..Z... .3..
00409130 C3 60 D3 8E 00 00 86 00 05 00 A6 20 10 22 86 B4 .`......... ."..
00409140 C7 80 80 F7 A0 60 FF FF EF 7F C5 A8 F4 C1 55 73 .....`........Us
00409150 F5 BC 00 2A DA 3D AC BC 00 2A 64 5E 6B BC E4 12 ...*.=...*d^k...
00409160 C5 13 95 FE 00 2A 08 69 80 A8 4C 02 F3 80 20 00 .....*.i..L... .
00409170 E9 B4 F3 C8 8A 0A F3 80 20 20 EC B4 88 9B E6 34 ........  .....4
00409180 E7 00 09 00 E6 B4 E4 34 EA 90 E2 B4 E1 E0 08 20 .......4....... 
00409190 E9 B4 4C CA 4E 60 E1 E0 08 20 80 02 0C 00 47 85 ..L.N`... ....G.
004091A0 38 82 54 22 D0 A1 67 85 3C 82 9F 92 FF D3 F4 A8 8.T"..g.<.......
004091B0 0E 02 E6 34 F8 5F F1 C8 1A 6A 80 E2 01 00 D7 12 ...4._...j......
004091C0 D4 23 10 AB CA 10 EB 10 5D A5 10 2C B6 BE 00 2A .#......]..,...*
004091D0 0E 69 5D A5 10 24 AC BC AB 10 40 08 E2 5D A4 12 .i]..$....@..]..
004091E0 B4 20 10 A3 F5 34 DD 02 78 00 D6 10 87 80 9F C0 . ...4..x.......
004091F0 C4 53 79 B2 A4 80 9F C0 FF 2B E5 F6 C4 AA 0A 00 .Sy......+......
00409200 B0 D3 9D 00 77 00 FD 84 77 10 F5 34 2D D3 AB D2 ....w...w..4-...
00409210 73 02 0F 00 E7 80 00 40 D6 13 E5 20 10 32 E4 80 s......@... .2..
00409220 02 80 64 A6 FE 88 C4 A4 FF 88 E4 B4 95 BE AD B6 ..d.............
00409230 FF 2B 8D B4 E0 04 08 9E CC 34 E4 20 07 39 7E 02 .+.......4. .9~.
00409240 01 00 EC 53 FE 84 00 10 00 2A 24 6F 6D 35 AC BC ...S.....*$om5..
00409250 8B 10 80 0B 8A 68 E9 34 C7 84 68 82 E7 84 6C 82 .....h.4..h...l.
00409260 FF 2B CD B0 D3 22 D0 39 E3 FE AB 12 97 12 F0 C8 .+...".9........
00409270 20 08 8B 10 6B BC 6D B5 E0 0B F4 67 6D 35 08 BA  ...k.m....gm5..
00409280 40 AA 04 80 24 CA 16 18 AE D3 7E 02 02 00 FE 84 @...$.....~.....
00409290 01 10 6B BC 8B 10 E0 0B D6 67 80 A8 58 01 E4 34 ..k......g..X..4
004092A0 C2 34 F6 20 D0 B9 E0 60 FD FF FF 7F E7 22 D0 39 .4. ...`.....".9
004092B0 7F B3 47 AA D9 BA D3 22 D0 99 40 8A 3C 01 F3 80 ..G...."..@.<...
004092C0 01 80 F2 A8 34 81 32 01 02 00 E9 22 50 A1 E2 34 ....4.2...."P..4
004092D0 C0 34 A0 D2 87 22 50 A9 3D BF 1F 0A 45 F8 C2 34 .4..."P.=...E..4
004092E0 A6 34 1F 0A 7D F6 C0 34 01 E1 00 00 F5 10 11 21 .4..}..4.......!
004092F0 10 43 B0 D2 1F 0A 2B F8 76 BF 1F 0A 65 F6 F4 22 .C....+.v...e.."
00409300 D0 31 6B BD ED B1 B0 D2 1F 0A 17 F8 A4 34 D7 10 .1k..........4..
00409310 1F 0A 4F F6 00 01 00 20 C0 34 F5 10 11 21 10 43 ..O.... .4...!.C
00409320 A0 D2 1F 0A FD F7 E0 34 A7 22 50 B3 D5 22 10 3E .......4."P..".>
00409330 C7 12 FF 29 51 F8 01 D3 C4 B4 C3 60 CB 8C 00 00 ...)Q......`....
00409340 DF 19 F5 BC C0 04 D4 8C 73 82 20 20 80 06 D0 8C ........s.  ....
00409350 B6 FE 66 22 10 A6 AC BC B6 BE 00 2A 12 67 0C 9A ..f".......*.g..
00409360 E0 04 C0 8C 80 06 C0 8C 67 22 10 A6 E4 34 11 11 ........g"...4..
00409370 C0 34 00 81 10 E4 67 02 03 00 A0 D2 F3 10 1F 0A .4....g.........
00409380 A1 F7 C4 34 A6 34 1F 0A D9 F5 03 D3 90 BE FF 2B ...4.4.........+
00409390 D1 F5 00 01 00 20 C0 34 F3 10 11 21 10 43 A0 D2 ..... .4...!.C..
004093A0 1F 0A 7F F7 E0 34 67 22 50 B3 D3 22 10 3E C7 12 .....4g"P..".>..
004093B0 07 88 D3 B7 FF 29 D7 F9 F5 34 FF 90 F5 B4 AD 19 .....)...4......
004093C0 E9 34 AB 10 C7 84 68 82 E7 84 6C 82 5F 08 61 AF .4....h...l._.a.
004093D0 62 FC D7 19 B7 12 9E 12 CA 10 EB 10 95 BE 5D A5 b.............].
004093E0 10 2C 00 2A DA 5B 5D A5 10 24 CA 10 EB 10 00 2A .,.*.[]..$.....*
004093F0 EE 66 95 FE EF 19 D3 13 33 1A 77 22 50 A1 CF 1A .f......3.w"P...
00409400 D2 80 00 40 86 D3 C7 20 10 96 6B BC BE 10 E0 0A ...@... ..k.....
00409410 5E 66 20 9A E1 E0 08 20 B7 12 C7 84 70 82 9E 12 ^f .... ....p...
00409420 E7 84 74 82 95 BE FF 2B 07 AF F5 34 95 FE E7 80 ..t....+...4....
00409430 1C 80 F5 B4 55 34 EE 73 E2 B4 02 A8 06 80 FD 00 ....U4.s........
00409440 D8 01 E2 B4 C2 37 95 BE 4D B4 AC B6 00 2A C0 6C .....7..M....*.l
00409450 9E 84 00 90 CC 93 00 2A 76 6D 6C 35 AC BC 8B 10 .......*vml5....
00409460 80 0B 7C 66 E1 E0 08 20 C7 84 78 82 E7 84 7C 82 ..|f... ..x...|.
00409470 FF 2B BD AE 03 FC 95 FE 6B BC 8B 10 00 09 F0 65 .+......k......e
00409480 4D 34 43 BA E2 36 E0 10 40 A8 E8 80 82 9B 55 B4 M4C..6..@.....U.
00409490 89 D3 D2 00 19 00 B5 34 E6 20 98 A1 89 92 E0 10 .......4. ......
004094A0 54 80 02 C0 05 A8 48 81 82 9B B5 B4 A0 12 D7 8B T.....H.........
004094B0 16 C0 E2 34 D7 74 E7 22 D0 A9 8A D3 B5 82 82 C0 ...4.t."........
004094C0 B5 22 0F AE E6 88 9C C1 E9 34 D5 10 A9 34 E7 80 .".......4...4..
004094D0 66 10 E0 20 10 32 2F B3 C0 10 B3 C8 04 38 40 22 f.. .2/......8@"
004094E0 90 33 7F B3 C2 34 DE 20 D0 31 C6 80 82 C0 DF 90 .3...4. .1......
004094F0 C6 20 0F 36 C7 88 0E 82 89 D2 C2 34 E7 00 00 24 . .6.......4...$
00409500 A7 20 18 A1 81 62 01 FC FF 3F 00 01 0A 00 D4 20 . ...b...?..... 
00409510 0F A4 A7 20 58 31 E6 00 01 00 F0 C8 4E 49 54 84 ... X1......NIT.
00409520 00 80 02 21 D8 39 88 BB D4 00 04 00 DE 88 CC 01 ...!.9..........
00409530 02 21 98 31 D4 C8 16 00 C0 60 00 CA 9A 3B C8 A8 .!.1.....`...;..
00409540 32 01 97 8A 2E C1 D4 A4 FC C0 C4 C8 26 01 C1 E0 2...........&...
00409550 08 20 01 D2 A6 84 4C 82 C8 80 81 C0 C7 A8 20 C1 . ....L....... .
00409560 74 DB D4 00 04 00 DE 88 DE 01 C1 E0 08 20 C6 84 t............ ..
00409570 64 82 14 19 62 80 1E 40 9D D3 62 20 10 3E BE 82 d...b..@..b .>..
00409580 04 80 67 10 80 12 F5 8A 1E C0 80 8A 06 00 97 A6 ..g.............
00409590 FC C8 FC 92 D7 8B 08 C0 FE 80 04 80 70 17 4C 9B ............p.L.
004095A0 62 20 D0 11 81 D3 E1 1A 95 74 C3 10 A0 10 6D B4 b .......t....m.
004095B0 4E B4 00 2A 9A 54 84 3E E0 10 94 20 90 23 C0 60 N..*.T.>... .#.`
004095C0 00 CA 9A 3B A4 20 50 41 14 BC 0C B5 00 2A 80 57 ...;. PA.....*.W
004095D0 0C 35 C0 60 00 CA 9A 3B 95 F4 E0 10 14 BC 00 2A .5.`...;.......*
004095E0 CE 54 BC 92 84 12 6D 34 4E 34 9B 1B C7 13 A5 1B .T....m4N4......
004095F0 E0 80 09 80 09 D2 E5 A8 04 80 A0 20 D0 21 01 D3 ........... .!..
00409600 60 60 00 CA 9A 3B 86 20 10 30 83 20 90 18 DF 90 ``...;. .0. ....
00409610 F7 10 A0 12 C7 AB 34 C0 F7 74 82 BB EC 92 A0 8A ......4..t......
00409620 06 00 BE 86 00 90 CC 93 E9 34 C2 34 E7 80 66 10 .........4.4..f.
00409630 F7 20 10 36 E6 10 DE 20 D0 31 C6 80 82 C0 D4 88 . .6... .1......
00409640 04 80 47 20 50 F1 5A B2 81 D3 59 1A 07 74 88 20 ..G P.Z...Y..t. 
00409650 50 60 AC 22 50 A9 A7 F6 C8 20 50 AA 75 20 18 A8 P`."P.... P.u ..
00409660 F1 93 B1 1B 8A D2 A9 92 ED 3C 59 1A 0A D3 E9 90 .........<Y.....
00409670 0E 3C A7 1A C1 E0 08 20 80 10 A6 84 44 82 D9 1A .<..... ....D...
00409680 C1 E0 08 20 C6 84 54 82 24 35 20 89 16 00 26 35 ... ..T.$5 ...&5
00409690 69 84 00 20 71 C8 0C 68 60 E0 01 00 65 20 10 2B i.. q..h`...e .+
004096A0 66 20 10 33 E2 20 D0 11 66 10 80 11 E3 10 CC 10 f .3. ..f.......
004096B0 54 84 00 90 AC B4 8D B4 4E B4 0F B5 00 2A 00 59 T.......N....*.Y
004096C0 6C 34 8D 35 AC BC 8C 10 A3 10 00 2A A2 63 2C 9A l4.5.......*.c,.
004096D0 4E 34 0F 35 48 20 50 39 F4 F4 C0 60 FF C9 9A 3B N4.5H P9...`...;
004096E0 F4 74 E6 A8 6C C0 E2 34 D7 74 E7 22 D0 A9 8A D3 .t..l..4.t."....
004096F0 B5 82 82 C0 B5 22 0F AE E6 88 6A C0 8C 92 D4 23 ....."....j....#
00409700 90 3B F4 20 10 F6 D7 8B 08 C0 FE 80 04 80 70 17 .;. ..........p.
00409710 5C 9B E9 34 F3 C8 98 38 81 D3 47 22 10 92 55 8A \..4...8..G"..U.
00409720 50 80 E0 80 04 80 F5 A8 48 80 F5 00 01 00 7F 92 P.......H.......
00409730 A5 B3 34 CA 7A 18 D7 8B 08 C0 DE A4 FC C0 8A D3 ..4.z...........
00409740 3C BB C0 02 09 00 3C 18 C1 E0 08 20 C6 84 5C 82 <.....<.... ..\.
00409750 37 1B 9C 92 74 F5 F4 8A 06 C0 17 A4 FC C8 FC 92 7...t...........
00409760 F4 74 E9 90 73 1B 8A D2 A9 92 ED 3C 8B 1B C7 13 .t..s......<....
00409770 95 1B 7E 92 5F 92 BB 1B 8A D2 C9 92 ED 3C E6 20 ..~._........<. 
00409780 D8 29 F5 9A E2 34 D3 80 20 00 FE 20 D0 39 E7 80 .)...4.. .. .9..
00409790 82 C0 FF 90 E7 20 0F 3E D3 C8 BC 30 C7 22 D0 B1 ..... .>...0."..
004097A0 F6 80 00 40 E0 20 10 B6 56 22 50 3B F6 20 10 96 ...@. ..V"P;. ..

;; fn004097B0: 004097B0
fn004097B0 proc
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

;; fn0040982A: 0040982A
;;   Called from:
;;     00409828 (in fn004097B0)
;;     004098E6 (in fn004098D8)
fn0040982A proc
	lw	r17,0008(sp)
	bgeuc	r7,r21,004098B2
	bnezc	r18,00409836
	bbeqzc	r17,00000003,00409840
	li	r6,00000001
	addiupc	r5,00003D80
	move.balc	r4,r16,00408962
	bgeuc	r21,r30,00409848
	bltc	r0,r18,004098E8
	li	r7,00000009
	move	r8,r0
	addu	r6,r18,r7
	li	r5,00000030
	move.balc	r4,r16,00408B22
	bc	004090C2
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
00409876                   09 35 EB 34 88 20 D0 31 D8 C8       .5.4. .1..
00409880 28 10 B5 82 00 40 2D D3 AB D2 64 A6 FE 88 A5 22 (....@-...d...."
00409890 10 32 A6 12 C4 80 02 80 C8 20 D0 41 A4 A6 FF 88 .2....... .A....
004098A0 CB B4 07 A9 E9 B4 C0 3E 39 1B 9F 90 30 D3 44 5F .......>9...0.D_
004098B0 C9 1B DD 02 81 00 95 74 C3 BF 2E 39 A4 10 B4 AA .......t...9....
004098C0 14 00 C4 AA 16 00 B0 D3 E0 72 FD 84 80 10 0C 18 .........r......

l004098D0:
	addiu	r5,r5,FFFFFFFF
	li	r7,00000030
	sb	r7,0000(r5)
	addiu	r7,sp,00000078

;; fn004098D8: 004098D8
fn004098D8 proc
	bltuc	r7,r5,004098D0

l004098DC:
	subu	r6,r22,r5
	move.balc	r4,r16,00408962
	addiu	r21,r21,00000004
	bc	0040982A
004098E8                         95 74 A0 10 DD 00 81 00         .t......
004098F0 F8 38 A4 10 DE 73 A7 A8 18 C0 D2 80 0A 40 89 D3 .8...s.......@..
00409900 D2 20 10 3E C7 10 AC 92 1F 0A 57 F0 52 82 09 80 . .>......W.R...
00409910 2F 1B BF 90 B0 D3 D4 5F DB 1B                   /......_..     

l0040991A:
	bltuc	r23,r30,00409922

l0040991E:
	addiu	r30,r23,00000004

l00409922:
	move	r21,r23

;; fn00409924: 00409924
;;   Called from:
;;     00409922 (in fn004097B0)
;;     004099AA (in fn004099A0)
fn00409924 proc
	bgeuc	r21,r30,0040992C
	bgec	r18,r0,00409948
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
	addiu	r20,sp,00000081
	lw	r4,0000(r21)
	movep	r5,r6,r0,r20
	balc	004099EA
	move	r5,r4
	bnec	r4,r20,00409960
	li	r7,00000030
	addiu	r5,sp,00000080
	sb	r7,0080(sp)
	move	r22,r5
	bnec	r21,r23,0040998A
	li	r6,00000001
	addiu	r22,r5,00000001
	move.balc	r4,r16,00408962
	bnezc	r18,00409976
	bbeqzc	r17,00000003,00409990
	li	r6,00000001
	addiupc	r5,00003CE0
	move.balc	r4,r16,00408962
	bc	00409990
	addiu	r22,r22,FFFFFFFF
	li	r7,00000030
	sb	r7,0000(r22)
	addiu	r7,sp,00000078
	bltuc	r7,r22,00409982
	subu	r20,r20,r22
	slt	r6,r20,r18
	move	r7,r20
	movz	r7,r18,r6
	addiu	r21,r21,00000004

;; fn004099A0: 004099A0
fn004099A0 proc
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
004099C2       00 CA 04 50 FF 29 E5 F1                     ...P.)..     

l004099CA:
	li	r7,00000001
	sw	r7,0004(sp)
	bc	00408E04

l004099D2:
	addiu	r19,sp,000000B8
	addiupc	r23,0000430E
	subu	r19,r19,r21
	slt	r30,r18,r19
	movn	r18,r19,r30

l004099E4:
	move	r30,r18
	bc	00408FC8

;; fn004099EA: 004099EA
;;   Called from:
;;     00409874 (in fn004097B0)
fn004099EA proc
	bc	004088E0

;; vfprintf: 004099EE
vfprintf proc
	save	00000110,ra,00000007
	movep	r16,r21,r4,r5
	move	r5,r6
	addiu	r4,sp,00000018
	li	r6,00000010
	li	r17,FFFFFFFF
	balc	0040A130
00409A00 28 D3 A0 10 DD 20 50 21 00 2A 84 0C 10 D3 C6 72 (.... P!.*.....r
00409A10 42 72 00 2A 1A 07 CA 73 07 11 42 73 D4 73 A3 BE Br.*...s..Bs.s..
00409A20 FF 2B 4F F1 04 A8 80 80 F0 84 4C 80 80 12 07 A8 .+O.......L.....
00409A30 06 80 00 0A 9A 37 84 12 80 17 D0 84 4A 00 47 82 .....7......J.G.
00409A40 20 20 C0 A8 06 80 E0 80 45 E1 80 97 8C 17 60 12   ......E.....`.
00409A50 92 BB E8 73 8B 15 85 97 87 97 8B 97 D0 D3 8C 97 ...s............
00409A60 FC 73 84 97 CA 73 42 73 07 11 D4 73 B0 BE FF 2B .s...sBs...s...+
00409A70 01 F1 24 12 9A 99 89 17 90 10                   ..$.......     

;; fn00409A7A: 00409A7A
fn00409A7A proc
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
00409AD4             80 17 7E B1 80 97 81 17 75 B1 01 95     ..~.....u...

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
00409AF6                   80 17 7E B1 80 97 81 17 75 B1       ..~.....u.
00409B00 01 95                                           ..             

l00409B02:
	lw	r7,0000(r16)
	move	r4,r19
	sb	r0,0000(r7)
	lw	r7,002C(r17)
	sw	r7,0054(sp)
	sw	r7,005C(sp)
	restore.jrc	00000020,ra,00000006

;; vsnprintf: 00409B10
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
00409B3E                                           FF 04               ..
00409B40 6F FF F1 B4 FF D3 7F D0 FD 84 6B 10 C2 73 B3 B7 o.........k..s..
00409B50 1B B6 FD B4 12 A8 0C 80 44 73 48 72 14 5C 7F 0B ........DsHr.\..
00409B60 8D FE D5 1F FF 2B 49 AE CB D3 C0 97 90 10 D5 1F .....+I.........

;; vsprintf: 00409B70
vsprintf proc
	save	00000020,ra,00000003
	movep	r16,r17,r4,r5
	move	r5,r6
	move	r4,sp
	li	r6,00000010
	balc	0040A130
00409B7E                                           FD 10               ..
00409B80 D1 10 A0 60 FF FF FF 7F 1F 0A 85 FF 23 1F 00 00 ...`........#...

;; do_read: 00409B90
do_read proc
	bc	0040D360

;; __isoc99_vsscanf: 00409B94
__isoc99_vsscanf proc
	save	000000B0,ra,00000003
	movep	r16,r17,r4,r5
	move	r5,r6
	move	r4,sp
	li	r6,00000010
	balc	0040A130
00409BA2       C0 00 90 00 A0 10 44 72 00 2A E2 0A FF 04   ......Dr.*....
00409BB0 DF FF EC B4 DD 10 FF D3 44 72 0F B6 19 B6 F7 B4 ........Dr......
00409BC0 20 0B E2 38 B3 1F 00 00 00 00 00 00 00 00 00 00  ..8............

;; atoi: 00409BD0
;;   Called from:
;;     00400832 (in fn00400832)
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
;;     00409F48 (in qsort)
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
;;     00409F4C (in qsort)
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
cycle proc
	save	00000120,ra,00000008
	movep	r18,r19,r4,r5
	move	r20,r6
	bltic	r6,00000002,00409C8E

l00409C84:
	lsa	r22,r6,r19,00000002
	sw	sp,0000(r22)
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
00409CAE                                           D0 10               ..
00409CB0 91 A4 00 24 00 2A 78 04 90 17 A9 92 7E B0 90 97 ...$.*x.....~...
00409CC0 91 90 B4 AA E9 3F 25 B0 C3 1B                   .....?%...     

;; sift: 00409CCA
;;   Called from:
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
00409CDA                               83 34 B3 10 90 DA           .4....
00409CE0 04 A8 3A 80                                     ..:.           

l00409CE4:
	move	r6,r18
	addiu	r5,sp,0000000C
	move.balc	r4,r21,00409C78
	restore.jrc	00000110,ra,00000008
00409CF0 C3 73 3F 92 F2 20 C7 84 49 92                   .s?.. ..I.     

l00409CFA:
	bltic	r17,00000002,00409CE4

l00409CFE:
	move	r6,r17
	subu	r7,r0,r21
	illegal
	illegal
	addu	r19,r16,r7
	lwxs	r6,r6(r22)
	lw	r17,000C(sp)
	subu	r7,r7,r6
	addu	r16,r16,r7
	move	r5,r16
	jalrc	ra,r20
	bgec	r4,r0,00409CDA
	movep	r4,r5,r16,r19
	jalrc	ra,r20
	bgec	r4,r0,00409CF0
	addiu	r7,sp,0000000C
	addiu	r17,r17,FFFFFFFE
	swxs	r19,r18(r7)
	move	r16,r19
	bc	00409CF8

;; pntz: 00409D32
;;   Called from:
;;     00409ECC (in qsort)
pntz proc
	lw	r5,0000(r4)
	li	r7,00000001
	li	r8,076BE629
	addiu	r6,r5,FFFFFFFF
	subu	r7,r7,r5
	and	r6,r6,r7
	addiupc	r5,00004A6C
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
	lwxs	r16,r18(r23)
	sll	r30,r18,00000002
	lw	r17,000C(sp)
	subu	r16,r17,r16
	move	r4,r16
	jalrc	ra,r21
	bgec	r0,r4,00409DD0
	beqzc	r19,00409DE6
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
	lw	r17,0004(sp)
	bneiuc	r7,00000001,00409D92
	lw	r17,0008(sp)
	bnezc	r7,00409D92
	bnezc	r19,00409DE2
	move	r6,r20
	addiu	r5,sp,0000000C
	move.balc	r4,r22,00409C78
	movep	r7,r8,r18,r23
	movep	r5,r6,r22,r21
	move.balc	r4,r17,00409CCA
	restore.jrc	00000120,r30,0000000A
	bltic	r18,00000002,00409DA8
	addu	r30,r23,r30
	subu	r4,r0,r22
	lw	r19,-0008(r30)
	move	r5,r16
	subu	r19,r4,r19
	addu	r4,r17,r4
	addu	r19,r17,r19
	jalrc	ra,r21
	bgec	r4,r0,00409DD2
	movep	r4,r5,r19,r16
	jalrc	ra,r21
	bltc	r4,r0,00409DA8
	bc	00409DD2

;; qsort: 00409E0E
;;   Called from:
;;     00407362 (in fn00407352)
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
00409EBE                                           01 D0               ..
00409EC0 B1 1B                                           ..             

l00409EC2:
	addu	r22,r18,r23
	bgeic	r16,00000002,00409EF0

l00409ECA:
	addiu	r4,sp,00000008
	balc	00409D32
00409ED0 64 12 A4 10 42 72 86 B1 72 38 57 3E 13 12       d...Br..r8W>.. 

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
00409EF6                   E2 34 42 72 81 D2 E7 80 07 10       .4Br......
00409F00 70 82 02 80 E2 B4 44 38 F4 73 55 11 F3 20 0F 3C p.....D8.sU.. .<
00409F10 10 81 01 80 91 BF 87 A4 40 C0 20 01 01 00 C2 73 ........@. ....s
00409F20 18 B2 29 B2 FF 2B 4D FE 42 72 81 D2 1A 38 E2 34 ..)..+M.Br...8.4
00409F30 55 11 20 01 01 00 E7 80 01 00 13 11 E2 B4 91 BF U. .............
00409F40 C2 73 DF 0A 2F FE 93 1B                         .s../...       

l00409F48:
	bc	00409C20

l00409F4C:
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
00409F62       FF D3 9D 10 E2 B4 F3 B4 6B BC 01 B6 0B B6   ........k.....
00409F70 00 2A CC 2B 01 D3 9D 10 40 0B E0 1D C2 34 E1 34 .*.+....@....4.4
00409F80 20 FC 7F B3 DE 34 9F 34 A7 80 9F C0 7C B3 5A B2  ....4.4....|.Z.
00409F90 E6 20 90 3B FE B2 88 98 EC 53 82 9B 00 B3 11 F6 . .;.....S......
00409FA0 20 BC A4 1F                                      ...           

;; strtof_l: 00409FA4
strtof_l proc
	save	00000010,ra,00000001
	move	r6,r0
	balc	00409F50
00409FAC                                     00 2A 70 62             .*pb
00409FB0 11 1F                                           ..             

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
	illegal
	illegal

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
	lw	r17,0018(sp)
	lw	r17,0014(sp)
	subu	r7,r7,r6
	lw	r6,0088(sp)
	addu	r7,r7,r6
	addu	r16,r16,r7
	sw	r16,0000(r17)
	restore.jrc	000000B0,ra,00000004

;; __strtoull_internal: 0040A00C
;;   Called from:
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
;;     004067B2 (in __inet_aton)
;;     004073C8 (in __lookup_serv)
__strtoul_internal proc
	save	00000010,ra,00000001
	addiu	r8,r0,FFFFFFFF
	move	r9,r0
	balc	00409FC0
0040A02E                                           11 1F               ..

;; __strtol_internal: 0040A030
__strtol_internal proc
	save	00000010,ra,00000001
	lui	r8,FFF80000
	move	r9,r0
	balc	00409FC0
0040A03C                                     11 1F                   .. 

;; __strtoimax_internal: 0040A03E
__strtoimax_internal proc
	bc	0040A018

;; __strtoumax_internal: 0040A042
__strtoumax_internal proc
	bc	0040A00C
0040A046                   00 00 00 00 00 00 00 00 00 00       ..........

;; memchr: 0040A050
;;   Called from:
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
	illegal
	movep	r22,r23,r6,r7
	and	r7,r8,r7
	li	r8,80808080
	and	r7,r7,r8
	bnezc	r7,0040A0E0
	move	r8,r4
	bc	0040A0CE
	lw	r7,0000(r8)
	xor	r7,r9,r7
	move	r4,r7
	nor	r7,r0,r7
	illegal
	movep	r22,r23,r6,r7
	and	r7,r7,r4
	li	r4,80808080
	and	r7,r7,r4
	bnezc	r7,0040A0D8
	addiu	r6,r6,FFFFFFFC
	addiu	r8,r8,00000004
	bgeiuc	r6,00000004,0040A0B0
	beqzc	r6,0040A0F8
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
;;     00403184 (in ping6_parse_reply)
;;     004068A6 (in inet_ntop)
;;     00406DC4 (in policyof)
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
;;     00400B52 (in fn00400B52)
;;     00401608 (in pr_options)
;;     004034E2 (in fn004034E2)
;;     0040590C (in realloc)
;;     00405FDA (in copy_addr)
;;     0040628E (in fn0040628E)
;;     00406C24 (in __lookup_ipliteral)
;;     00406F80 (in dns_parse_callback)
;;     004071FC (in __lookup_name)
;;     0040738E (in fn0040738E)
;;     00407F36 (in __get_handler_set)
;;     00407FCC (in __libc_sigaction)
;;     004099FC (in vfprintf)
;;     00409AD0 (in sn_write)
;;     00409AF2 (in sn_write)
;;     00409B22 (in vsnprintf)
;;     00409B7A (in vsprintf)
;;     00409B9E (in __isoc99_vsscanf)
;;     00409CAA (in cycle)
;;     0040B13C (in __copy_tls)
;;     0040D384 (in __string_read)
;;     0040D440 (in arg_n)
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
	addu	r7,r4,r6
	bgeuc	r5,r7,0040A616
	xor	r8,r5,r4
	andi	r8,r8,00000003
	bgeuc	r4,r5,0040A61A
	bnec	r0,r8,0040A666
	andi	r7,r4,00000003
	beqc	r0,r7,0040A67C
	addiu	r8,r6,FFFFFFFF
	move	r7,r4
	bnezc	r6,0040A546
	bc	0040A688
	addiu	r8,r8,FFFFFFFF
	beqc	r8,r10,0040A688
	addiu	r5,r5,00000001
	addiu	r7,r7,00000001
	lbu	r9,-0001(r5)
	andi	r6,r7,00000003
	addiu	r10,r0,FFFFFFFF
	sb	r9,-0001(r7)
	bnezc	r6,0040A540
	bltiuc	r8,00000004,0040A684
	addiu	r11,r8,FFFFFFFC
	move	r6,r0
	move	r10,r11
	ins	r10,r0,00000000,00000001
	addiu	r10,r10,00000004
	lwx	r9,r6(r5)
	swx	r9,r6(r7)
	addiu	r6,r6,00000004
	bnec	r6,r10,0040A56C
	ins	r11,r0,00000000,00000001
	andi	r6,r8,00000003
	addiu	r8,r11,00000004
	addu	r7,r7,r8
	addu	r5,r5,r8
	beqc	r0,r6,0040A688
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
	or	r8,r5,r7
	andi	r8,r8,00000003
	bnec	r0,r8,0040A66A
	addiu	r8,r6,FFFFFFFC
	move	r9,r0
	srl	r8,r8,00000002
	addiu	r8,r8,00000001
	sll	r10,r8,00000002
	lwxs	r11,r9(r5)
	swxs	r11,r9(r7)
	addiu	r9,r9,00000001
	bltuc	r9,r8,0040A5D6
	subu	r8,r6,r10
	addu	r9,r7,r10
	addu	r11,r5,r10
	beqc	r6,r10,0040A688
	lbux	r6,r10(r5)
	illegal
	beqic	r8,00000001,0040A688
	lbu	r7,0001(r11)
	sb	r7,0001(r9)
	beqic	r8,00000002,0040A688
	lbu	r7,0002(r11)
	sb	r7,0002(r9)
	jrc	ra
	bc	0040A130
	bnec	r0,r8,0040A658
	andi	r7,r7,00000003
	beqzc	r7,0040A680
	addiu	r7,r6,FFFFFFFF
	bnezc	r6,0040A630
	bc	0040A688
	addiu	r7,r7,FFFFFFFF
	beqc	r7,r9,0040A688
	lbux	r8,r7(r5)
	addu	r6,r4,r7
	andi	r6,r6,00000003
	addiu	r9,r0,FFFFFFFF
	illegal
	bnezc	r6,0040A62A
	move	r6,r7
	bltiuc	r7,00000004,0040A658
	addiu	r6,r6,FFFFFFFC
	lwx	r8,r6(r5)
	swx	r8,r6(r4)
	bgeiuc	r6,00000004,0040A648
	andi	r6,r7,00000003
	beqzc	r6,0040A688
	addiu	r6,r6,FFFFFFFF
	lbux	r7,r6(r5)
	illegal
	bc	0040A658
	move	r7,r4
	bc	0040A58A
	move	r8,r0
	lbux	r9,r8(r5)
	illegal
	addiu	r8,r8,00000001
	bnec	r6,r8,0040A66C
	jrc	ra
	movep	r7,r8,r4,r6
	bc	0040A55A
	move	r7,r6
	bc	0040A642
	move	r6,r8
	bc	0040A58A

l0040A688:
	jrc	ra
0040A68A                               00 00 00 00 00 00           ......

;; memset: 0040A690
;;   Called from:
;;     0040017E (in main)
;;     00401366 (in pr_addr)
;;     00401C88 (in set_signal)
;;     0040330C (in niquery_option_subject_addr_handler)
;;     00404880 (in __init_libc)
;;     004062B0 (in getifaddrs)
;;     0040807C (in __fopen_rb_ca)
;;     00408B4C (in pad)
;;     00409B3A (in vsnprintf)
;;     00409F5E (in strtox)
;;     0040DD1C (in stpncpy)
;;     0040E008 (in __synccall)
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
	sw	r6,0044(sp)
	sw	r6,0048(sp)
	sw	r6,-000C(r10)
	sw	r6,-0008(r10)
	bltiuc	r8,00000019,0040A74C
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
	addiu	r5,r8,FFFFFFE0
	ins	r5,r0,00000000,00000001
	addiu	r5,r5,00000020
	addu	r5,r7,r5
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
strcat proc
	save	00000020,ra,00000002
	move	r16,r4
	sw	r5,000C(sp)
	balc	0040A890
0040A75A                               A3 34 08 B2 00 2A           .4...*
0040A760 FE 00 90 10 22 1F 00 00 00 00 00 00 00 00 00 00 ...."...........

;; strchr: 0040A770
;;   Called from:
;;     00406B80 (in __lookup_ipliteral)
;;     0040AC32 (in strstr)
strchr proc
	save	00000010,ra,00000002
	move	r16,r5
	balc	0040A790
0040A778                         0C F0 C8 5F 84 53 E0 20         ..._.S. 
0040A780 10 26 12 1F 00 00 00 00 00 00 00 00 00 00 00 00 .&..............

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
0040A828                         08 B2 12 1F 00 00 00 00         ........

;; strcmp: 0040A830
;;   Called from:
;;     0040CD74 (in fn0040CCA4)
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
strcpy proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	0040DC10
0040A868                         90 10 12 1F 00 00 00 00         ........

;; __strdup: 0040A870
__strdup proc
	save	00000010,ra,00000003
	move	r17,r4
	balc	0040A890
0040A878                         04 02 01 00 1F 0A 13 AA         ........
0040A880 0A 9A 11 BF E3 83 12 30 FF 29 A5 F8 13 1F 00 00 .......0.)......

;; strlen: 0040A890
;;   Called from:
;;     004069AE (in inet_ntop)
;;     00406E3E (in name_from_hosts)
;;     0040743C (in __lookup_serv)
;;     004083F4 (in fputs_unlocked)
;;     0040A756 (in strcat)
;;     0040A824 (in strchrnul)
;;     0040A874 (in __strdup)
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
0040A8A8                         F1 93                           ..     

l0040A8AA:
	lw	r5,0000(r7)
	move	r6,r5
	nor	r5,r0,r5
	illegal
	movep	r22,r23,r6,r7
	and	r6,r6,r5
	li	r5,80808080
	and	r6,r6,r5
	beqzc	r6,0040A8A8
	move	r6,r7
	lbu	r7,0000(r7)
	beqzc	r7,0040A8D0
	addiu	r6,r6,00000001
	lbu	r5,0000(r6)
	bnezc	r5,0040A8CA
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
;;     0040341E (in niquery_option_handler)
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
strncpy proc
	save	00000010,ra,00000002
	move	r16,r4
	balc	0040DC90
0040A938                         90 10 12 1F 00 00 00 00         ........

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
0040A94A                               04 9A C9 B0 13 1F           ......
0040A950 90 10 13 1F 00 00 00 00 00 00 00 00 00 00 00 00 ................

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
	addu	r7,r16,r6
	addu	r5,r19,r18
	bgeuc	r7,r20,0040AAAE
	lbux	r5,r6(r5)
	lbux	r4,r7(r19)
	beqc	r4,r5,0040AB9A
	bgeuc	r4,r5,0040AB8E
	subu	r21,r7,r18
	move	r16,r7
	li	r6,00000001
	addu	r7,r16,r6
	addu	r5,r19,r18
	bltuc	r7,r20,0040AA8E
	addiu	r8,r18,00000001
	li	r17,00000001
	li	r6,00000001
	move	r16,r0
	addiu	r23,r0,FFFFFFFF
	addu	r7,r16,r6
	addu	r5,r19,r23
	bgeuc	r7,r20,0040AAE8
	lbux	r4,r6(r5)
	lbux	r5,r7(r19)
	beqc	r4,r5,0040ABAC
	bgeuc	r4,r5,0040ABA2
	subu	r17,r7,r23
	move	r16,r7
	li	r6,00000001
	addu	r7,r16,r6
	addu	r5,r19,r23
	bltuc	r7,r20,0040AAC6
	addiu	r30,r23,00000001
	bltuc	r8,r30,0040AAF6
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
	move	r4,r16
	restore.jrc	00000460,r30,0000000A
0040AB8E                                           50 12               P.
0040AB90 A0 02 01 00 09 92 01 D3 ED 1A A6 8A 05 3F C9 90 .............?..
0040ABA0 E5 1A F0 12 81 D0 09 92 01 D3 11 1B D1 88 2B 3F ..............+?
0040ABB0 C9 90 09 1B                                     ....           

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
	lbux	r5,r23(r16)
	move	r7,r23
	lbux	r6,r23(r19)
	bnec	r5,r6,0040AC0A
	bgeuc	r21,r7,0040AB88
	addiu	r7,r7,FFFFFFFF
	lbux	r5,r7(r19)
	lbux	r6,r7(r16)
	beqc	r5,r6,0040ABF8
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
;;     0040E076 (in rewinddir)
__lock proc
	save	00000010,ra,00000002
	addiupc	r7,00024B7D
	move	r16,r4
	lw	r7,000C(r7)
	bnezc	r7,0040AD46

l0040AD3C:
	restore.jrc	00000010,ra,00000002
0040AD3E                                           C7 10               ..
0040AD40 81 92 00 0A 6A 00                               ....j.         

l0040AD46:
	illegal
	illegal
	li	r7,00000001
	move	r6,r7
	illegal
	beqzc	r6,0040AD4A
	illegal
	bnezc	r5,0040AD3E
	restore.jrc	00000010,ra,00000002

;; __unlock: 0040AD60
;;   Called from:
;;     00408624 (in __ofl_unlock)
__unlock proc
	save	00000010,ra,00000002
	lw	r7,0000(r4)
	move	r16,r4
	beqzc	r7,0040AD98

l0040AD68:
	illegal
	sw	r0,0000(sp)
	illegal
	lw	r7,0004(r4)
	beqzc	r7,0040AD98
	li	r7,00000001
	move	r5,r4
	addiu	r6,r0,00000081
	li	r4,00000062
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r4,r7,0040AD98
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
;;     0040B08E (in write)
;;     0040E9E0 (in __timedwait_cp)
__syscall_cp proc
	bc	0040ADA0
0040ADA8                         00 00 00 00 00 00 00 00         ........

;; __wait: 0040ADB0
;;   Called from:
;;     0040832A (in flockfile)
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
	illegal

l0040ADCC:
	addiu	r7,r7,FFFFFFFF
	beqzc	r7,0040ADEC

l0040ADD0:
	beqzc	r16,0040ADC2

l0040ADD2:
	lw	r6,0000(r16)
	beqzc	r6,0040ADC2

l0040ADD6:
	illegal
	illegal
	addiu	r7,r7,00000001
	illegal
	beqzc	r7,0040ADDA
	illegal
	bc	0040ADEE

l0040ADEC:
	bnezc	r16,0040ADD6

l0040ADEE:
	lw	r7,0000(r18)
	beqc	r17,r7,0040AE0A

l0040ADF2:
	beqzc	r16,0040AE08

l0040ADF4:
	illegal
	illegal
	addiu	r7,r7,FFFFFFFF
	illegal
	beqzc	r7,0040ADF8
	illegal

l0040AE08:
	restore.jrc	00000020,ra,00000005

l0040AE0A:
	movep	r7,r8,r17,r0
	movep	r5,r6,r18,r19
	li	r4,00000062
	balc	00404A50
0040AE14             E0 80 26 80 87 A8 D3 3F 79 BD 72 BD     ..&....?y.r.
0040AE20 62 D2 FF 2B 2B 9C C7 1B 00 00 00 00 00 00 00 00 b..++...........

;; __do_cleanup_push: 0040AE30
;;   Called from:
;;     0040AE3E (in _pthread_cleanup_pop)
__do_cleanup_push proc
	jrc	ra

;; _pthread_cleanup_push: 0040AE32
_pthread_cleanup_push proc
	swm	r5,0000(r4),00000002
	bc	0040AE30

;; _pthread_cleanup_pop: 0040AE3A
_pthread_cleanup_pop proc
	save	00000010,ra,00000003
	movep	r16,r17,r4,r5
	balc	0040AE30
0040AE42       8A 98 80 17 01 16 E3 83 12 30 E0 D8 13 1F   .........0....

;; __pthread_setcancelstate: 0040AE50
;;   Called from:
;;     00407240 (in __lookup_name)
;;     0040EA22 (in __timedwait)
__pthread_setcancelstate proc
	li	r7,00000016
	bgeiuc	r4,00000003,0040AE68

l0040AE56:
	illegal
	beqzc	r5,0040AE62
	lw	r7,-0080(r3)
	sw	r7,0040(sp)
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
	addiupc	r5,000038BA
	addiupc	r4,000038C4
	balc	0040CC50
0040AEC0 BF 04 ED FF C4 10 00 80 06 C0 E2 04 D2 54 E7 A4 .............T..
0040AED0 00 51 F6 DA E6 10 82 04 C6 54 E4 A4 00 59 EB 9B .Q.......T...Y..
0040AEE0 00 80 06 C0 06 BB 80 80 26 80 13 1F 30 BE E3 83 ........&...0...
0040AEF0 12 30 C0 D8                                     .0..           

;; __clock_gettime: 0040AEF4
;;   Called from:
;;     00407786 (in mtime)
;;     0040AF4A (in gettimeofday)
__clock_gettime proc
	save	00000010,ra,00000003
	lwpc	r7,004303A0
	movep	r17,r16,r4,r5
	beqzc	r7,0040AF12
	jalrc	ra,r7
	beqzc	r4,0040AF3C
	addiu	r6,r0,FFFFFFEA
	bnec	r4,r6,0040AF12
	restore	00000010,ra,00000003
	bc	0040CC30
	movep	r5,r6,r17,r16
	li	r4,00000071
	balc	00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040AF0A
	addiu	r4,r0,FFFFFFEA
	bnezc	r17,0040AF0A
	movep	r5,r6,r16,r0
	addiu	r4,r0,000000A9
	balc	00404A50
	lw	r7,0004(r16)
	addiu	r6,r0,000003E8
	mul	r7,r7,r6
	sw	r7,0004(sp)
	move	r4,r0
	restore.jrc	00000010,ra,00000003

;; gettimeofday: 0040AF40
gettimeofday proc
	save	00000020,ra,00000002
	move	r16,r4
	beqzc	r4,0040AF5E

l0040AF46:
	addiu	r5,sp,00000008
	move	r4,r0
	balc	0040AEF4
0040AF4E                                           E2 34               .4
0040AF50 C0 00 E8 03 A3 34 80 97 C5 20 18 39 81 97       .....4... .9.. 

l0040AF5E:
	move	r4,r0
	restore.jrc	00000020,ra,00000002
0040AF62       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

;; dummy: 0040AF70
dummy proc
	jrc	ra

;; close: 0040AF72
close proc
	save	00000010,ra,00000001
	balc	004080E0
0040AF78                         40 11 6B BD 64 BD 20 11         @.k.d. .
0040AF80 39 D2 FF 2B 1F FE E0 80 04 80 C4 53 E1 83 12 30 9..+.......S...0
0040AF90 E0 20 10 22 00 28 98 1C 00 00 00 00 00 00 00 00 . .".(..........

;; geteuid: 0040AFA0
geteuid proc
	addiu	r4,r0,000000AF
	bc	00404A50
0040AFA8                         00 00 00 00 00 00 00 00         ........

;; getpid: 0040AFB0
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
isatty proc
	save	00000020,ra,00000001
	li	r6,40087468
	move	r5,r4
	addiu	r7,sp,00000008
	li	r4,0000001D
	balc	00404A50
0040AFE2       84 80 01 50 21 1F 00 00 00 00 00 00 00 00   ...P!.........

;; seteuid: 0040AFF0
seteuid proc
	li	r7,FFFFFFFF
	movep	r5,r6,r7,r4
	addiu	r4,r0,00000093
	bc	0040B04C
0040AFFC                                     00 00 00 00             ....

;; setuid: 0040B000
setuid proc
	move	r5,r4
	addiu	r4,r0,00000092
	movep	r6,r7,r0,r0
	bc	0040B04C
0040B00C                                     00 00 00 00             ....

;; do_setxid: 0040B010
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
0040B024             80 20 D0 89 1A 9A 84 17 96 BB 80 10     . ..........
0040B030 FF 2B 6D CE 80 00 AC 00 0E 38 09 D3 A4 10 80 00 .+m......8......
0040B040 81 00 04 38 84 94                               ...8..         

l0040B046:
	restore.jrc	00000010,ra,00000003

l0040B048:
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
	addiupc	r4,FFFFFFDA
	sw	r7,0014(sp)
	sw	r16,001C(sp)
	balc	0040DDF2
	lw	r17,001C(sp)
	beqzc	r4,0040B076
	bgec	r0,r4,0040B074
	balc	004049B0
	lw	r17,001C(sp)
	sw	r7,0000(sp)
	move	r4,r16
	restore.jrc	00000030,ra,00000002
	sigrie	00000000
	sigrie	00000000

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
0040B092       E1 83 12 30 00 28 96 1B 00 00 00 00 00 00   ...0.(........

;; isalnum: 0040B0A0
;;   Called from:
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
__init_tp proc
	save	00000010,ra,00000002
	move	r16,r4
	addiu	r4,r4,000000B0
	sw	r16,0000(r16)
	balc	0040DD30
0040B0CE                                           FF D3               ..
0040B0D0 04 A8 28 80 0A BA 01 D3 E4 E0 02 90 C7 84 30 94 ..(...........0.
0040B0E0 87 92 60 D2 FF 2B 69 99 E4 04 6C 93 F0 84 78 90 ..`..+i...l...x.
0040B0F0 F0 00 64 00 F0 84 64 90 E0 10 07 96 87 10 12 1F ..d...d.........

;; __copy_tls: 0040B100
__copy_tls proc
	save	00000020,ra,00000006
	addiupc	r7,00024995
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
0040B140 B0 15                                           ..             

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
	addiupc	r7,00013E22
	addiupc	r4,00024948
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
	addiupc	r4,00013E02
	bgeuc	r7,r6,0040B20E
	move	r10,r0
	addiu	r9,r0,FFFFFFFF
	addiu	r8,r0,00000802
	li	r7,00000003
	move	r5,r0
	addiu	r4,r0,000000DE
	balc	00404A50
	balc	0040B100
	balc	0040B0C0
	bgec	r4,r0,0040B222
	sb	r0,0000(r0)
	illegal
	restore.jrc	00000010,ra,00000001
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

;; _Exit: 0040B230
_Exit proc
	save	00000010,ra,00000002
	move	r16,r4
	move	r5,r4
	li	r4,0000005E
	balc	00404A50
0040B23C                                     B0 10 5D D2             ..].
0040B240 F7 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; scanexp: 0040B250
scanexp proc
	save	00000030,ra,00000006
	lw	r7,0004(r4)
	move	r19,r4
	lw	r9,0068(r4)
	bgeuc	r7,r9,0040B3BC
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r17,0000(r7)
	beqic	r17,0000002B,0040B39E
	beqic	r17,0000002D,0040B39E
	addiu	r7,r17,FFFFFFD0
	bgeiuc	r7,0000000A,0040B334
	move	r18,r0
	move	r16,r0
	bc	0040B292
	sw	r6,0044(sp)
	lbu	r17,0000(r7)
	li	r7,0CCCCCCB
	addiu	r10,r17,FFFFFFD0
	bgeiuc	r10,0000000A,0040B2C0
	bltc	r7,r16,0040B2C0
	lsa	r16,r16,r16,00000002
	lw	r7,0004(r19)
	lsa	r16,r16,r17,00000001
	addiu	r16,r16,FFFFFFD0
	addiu	r6,r7,00000001
	bltuc	r7,r9,0040B27C
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	move	r17,r4
	li	r7,0CCCCCCB
	addiu	r10,r17,FFFFFFD0
	bltiuc	r10,0000000A,0040B28E
	sra	r20,r16,0000001F
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
	bltc	r5,r20,0040B356
	beqc	r20,r5,0040B3CC
	lw	r5,0004(r19)
	addiu	r7,r7,FFFFFFFF
	move	r16,r17
	addu	r20,r6,r7
	bgeuc	r5,r9,0040B36E
	addiu	r7,r5,00000001
	sw	r7,0044(sp)
	lbu	r17,0000(r5)
	addiu	r10,r17,FFFFFFD0
	bc	0040B2C4
	bnec	r0,r5,0040B3D8
	beqc	r0,r9,0040B3F2
	lw	r7,0004(r19)
	addiu	r7,r7,FFFFFFFF
	move	r16,r0
	lui	r20,FFF80000
	sw	r7,0044(sp)
	movep	r4,r5,r16,r20
	restore.jrc	00000030,ra,00000006
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	move	r17,r4
	addiu	r10,r17,FFFFFFD0
	bgeiuc	r10,0000000A,0040B37E
	lw	r7,0004(r19)
	bgeuc	r7,r9,0040B348
	addiu	r6,r7,00000001
	sw	r6,0044(sp)
	lbu	r17,0000(r7)
	addiu	r10,r17,FFFFFFD0
	bc	0040B356
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	move	r17,r4
	addiu	r10,r17,FFFFFFD0
	bc	0040B2C4
	beqc	r0,r9,0040B388
	lw	r7,0004(r19)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0044(sp)
	beqzc	r18,0040B344
	subu	r16,r0,r16
	subu	r7,r0,r20
	sltu	r5,r0,r16
	subu	r20,r7,r5
	movep	r4,r5,r16,r20
	restore.jrc	00000030,ra,00000006
	lw	r7,0004(r19)
	bgeuc	r7,r9,0040B3E4
	addiu	r6,r7,00000001
	sw	r6,0044(sp)
	lbu	r4,0000(r7)
	addiu	r7,r4,FFFFFFD0
	bgeiuc	r7,0000000A,0040B330
	illegal
	move	r17,r4
	bc	0040B278
	sw	r5,000C(sp)
	balc	0040CB78
	move	r17,r4
	lw	r9,0068(r19)
	lw	r17,000C(sp)
	bc	0040B266
	li	r5,7AE147AD
	bgeuc	r5,r16,0040B314
	bc	0040B356
	beqc	r0,r9,0040B3F2
	lw	r7,0004(r19)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0044(sp)
	bc	0040B33A
	sw	r5,000C(sp)
	move.balc	r4,r19,0040CB78
	lw	r9,0068(r19)
	lw	r17,000C(sp)
	bc	0040B3AC
	move	r16,r0
	lui	r20,FFF80000
	bc	0040B344

;; decfloat: 0040B3FA
decfloat proc
	save	00000260,r30,0000000A
	move	r16,r4
	swm	r7,0004(sp),00000002
	sw	r6,0010(sp)
	sw	r9,0014(sp)
	beqic	r5,00000030,0040B410
	bc	0040BAE8
	lw	r6,0068(r16)
	lw	r7,0004(r16)
	bgeuc	r7,r6,0040B62E
	addiu	r5,r7,00000001
	sw	r5,0004(sp)
	lbu	r5,0000(r7)
	beqic	r5,00000030,0040B414
	li	r7,00000001
	sw	r7,0000(sp)
	beqic	r5,0000002E,0040B836
	move	r21,r0
	move	r23,r0
	move	r18,r0
	move	r19,r0
	move	r22,r0
	move	r17,r0
	move	r30,r0
	move	r20,r0
	sw	r0,0030(sp)
	addiu	r7,r5,FFFFFFD0
	bltiuc	r7,0000000A,0040B472
	bneiuc	r5,0000002E,0040B4CC
	bnec	r0,r21,0040B822
	move	r23,r22
	move	r18,r17
	addiu	r21,r0,00000001
	lw	r7,0004(r16)
	lw	r6,0068(r16)
	bgeuc	r7,r6,0040B4C4
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r5,0000(r7)
	addiu	r7,r5,FFFFFFD0
	bgeiuc	r7,0000000A,0040B448
	beqic	r5,0000002E,0040B44C
	addiu	r6,r22,00000001
	sltu	r4,r6,r22
	move	r22,r6
	addu	r17,r4,r17
	bgeic	r30,0000003D,0040B602
	xori	r5,r5,00000030
	addiu	r4,r20,00000001
	movn	r19,r6,r5
	beqc	r0,r20,0040B618
	addiu	r6,sp,FFFFF230
	lsa	r5,r30,r6,00000002
	lw	r6,0E00(r5)
	lsa	r6,r6,r6,00000002
	lsa	r7,r6,r7,00000001
	sw	r7,0E00(r5)
	bneiuc	r4,00000009,0040B63E
	li	r7,00000001
	lw	r6,0068(r16)
	sw	r7,0000(sp)
	addiu	r30,r30,00000001
	lw	r7,0004(r16)
	move	r20,r0
	bltuc	r7,r6,0040B462
	move.balc	r4,r16,0040CB78
	move	r5,r4
	bc	0040B440
	movz	r23,r22,r21
	movz	r18,r17,r21
	lw	r17,0000(sp)
	beqc	r0,r7,0040B646
	ori	r7,r5,00000020
	beqic	r7,00000025,0040BAEE
	bltc	r5,r0,0040B4F8
	lw	r7,0068(r16)
	beqzc	r7,0040B4F8
	lw	r7,0004(r16)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)
	lw	r17,0000(sp)
	beqc	r0,r7,0040B652
	lw	r4,0030(sp)
	beqc	r0,r16,0040B826
	beqc	r22,r23,0040BBA4
	lw	r17,0004(sp)
	srl	r7,r6,0000001F
	addu	r7,r7,r6
	sra	r7,r7,00000001
	subu	r7,r0,r7
	sra	r6,r7,0000001F
	bltc	r6,r18,0040BBF6
	beqc	r18,r6,0040BBF2
	lw	r17,0004(sp)
	addiu	r6,r7,FFFFFF96
	sra	r7,r6,0000001F
	bltc	r18,r7,0040BB0C
	beqc	r18,r7,0040BB08
	beqc	r0,r20,0040B59E
	bgeic	r20,00000009,0040B59C
	sll	r5,r30,00000002
	addiu	r7,sp,FFFFF230
	addu	r7,r7,r5
	lw	r7,0E00(r7)
	lsa	r7,r7,r7,00000002
	sll	r6,r7,00000001
	beqic	r20,00000008,0040B592
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000007,0040B592
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000006,0040B592
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000005,0040B592
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000004,0040B592
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	beqic	r20,00000003,0040B592
	lsa	r7,r7,r6,00000003
	sll	r6,r7,00000001
	bneiuc	r20,00000001,0040B592
	lsa	r6,r7,r6,00000003
	sll	r6,r6,00000001
	addiu	r7,sp,FFFFF230
	addu	r5,r7,r5
	sw	r6,0E00(r5)
	addiu	r30,r30,00000001
	lw	r17,0008(sp)
	move	r20,r23
	balc	00410170
	swm	r4,0008(sp),00000002
	bgeic	r19,00000009,0040B668
	bltc	r23,r19,0040B668
	bgeic	r23,00000012,0040B668
	lw	r4,0030(sp)
	bneiuc	r23,00000009,0040B5C0
	bc	0040BD24
	bgeic	r23,00000009,0040B5C8
	bc	0040BCF8
	addiu	r7,r23,FFFFFFF7
	sll	r6,r7,00000002
	subu	r7,r7,r6
	lw	r17,0010(sp)
	addu	r7,r7,r6
	bgeic	r7,0000001F,0040B5E0
	srlv	r7,r16,r7
	bnec	r0,r7,0040B668
	addiupc	r7,00003E2E
	addiu	r23,r23,FFFFFFF6
	lwxs	r4,r23(r7)
	balc	00410170
	movep	r18,r19,r4,r5
	move.balc	r4,r16,004101D0
	lwm	r6,0008(sp),00000002
	balc	00404330
	bc	0040BBD0
	beqic	r5,00000030,0040B458
	lw	r7,0220(sp)
	addiu	r19,r0,0000045C
	ori	r7,r7,00000001
	sw	r7,0220(sp)
	bc	0040B458
	li	r6,00000001
	addiu	r20,r0,00000001
	sw	r6,0000(sp)
	addiu	r6,sp,FFFFF230
	lsa	r6,r30,r6,00000002
	sw	r7,0E00(r6)
	bc	0040B458
	move.balc	r4,r16,0040CB78
	move	r5,r4
	beqic	r4,00000030,0040B410
	li	r7,00000001
	sw	r7,0000(sp)
	bc	0040B42A
	li	r7,00000001
	move	r20,r4
	sw	r7,0000(sp)
	bc	0040B458
	bltc	r5,r0,0040B652
	lw	r7,0068(r16)
	bnec	r0,r7,0040B4EC
	balc	004049B0
	li	r7,00000016
	sw	r7,0000(sp)
	movep	r6,r7,r0,r0
	move.balc	r4,r16,0040CB40
	movep	r4,r5,r0,r0
	restore.jrc	00000260,r30,0000000A
	move	r30,r7
	addiu	r7,r30,FFFFFFFF
	addiu	r6,sp,00000030
	lwxs	r6,r7(r6)
	beqzc	r6,0040B666
	li	r9,38E38E39
	sra	r7,r23,0000001F
	illegal
	sra	r9,r9,00000001
	subu	r17,r9,r7
	lsa	r9,r17,r17,00000003
	subu	r17,r23,r9
	beqzc	r17,0040B70E
	bltc	r23,r0,0040BCC6
	li	r7,00000008
	addiupc	r6,00003DD2
	subu	r7,r7,r17
	lwxs	r16,r7(r6)
	beqc	r0,r30,0040BD52
	li	r6,3B9ACA00
	move	r5,r0
	move	r18,r0
	move	r7,r0
	div	r19,r6,r16
	illegal
	bc	0040B6C0
	addiu	r7,r7,00000001
	beqc	r30,r7,0040B6F4
	addiu	r6,sp,00000030
	lwxs	r4,r7(r6)
	illegal
	illegal
	addu	r6,r6,r5
	modu	r5,r4,r16
	illegal
	addiu	r4,sp,00000030
	mul	r5,r5,r19
	swxs	r6,r7(r4)
	bnec	r7,r18,0040B6BA
	bnezc	r6,0040B6BA
	addiu	r18,r18,00000001
	addiu	r7,r7,00000001
	addiu	r20,r20,FFFFFFF7
	andi	r18,r18,0000007F
	bnec	r30,r7,0040B6C0
	beqzc	r5,0040B704
	addiu	r7,sp,FFFFF230
	lsa	r7,r30,r7,00000002
	addiu	r30,r30,00000001
	sw	r5,0E00(r7)
	subu	r10,r20,r17
	move	r17,r18
	addiu	r20,r10,00000009
	sw	r0,0000(sp)
	bltic	r20,00000012,0040B72E
	bneiuc	r20,00000012,0040B890
	addiu	r7,sp,FFFFF230
	li	r6,0089705E
	lsa	r7,r17,r7,00000002
	lw	r7,0E00(r7)
	bltuc	r6,r7,0040B890
	lw	r17,0000(sp)
	addiu	r16,r30,FFFFFFFF
	andi	r22,r16,0000007F
	move	r21,r0
	addiu	r7,r7,FFFFFFE3
	sw	r7,0000(sp)
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
	bgeuc	r3,r19,0040B7A8
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
	beqc	r22,r17,0040B7D6
	andi	r22,r6,0000007F
	bc	0040B740
	addiu	r6,sp,FFFFF230
	move	r4,r19
	addu	r23,r23,r6
	andi	r7,r16,0000007F
	move	r21,r0
	addiu	r6,r22,FFFFFFFF
	sw	r4,0E00(r23)
	bnec	r22,r7,0040B79E
	beqc	r22,r17,0040B7D4
	movz	r30,r7,r4
	andi	r22,r6,0000007F
	addiu	r16,r30,FFFFFFFF
	bc	0040B740
	move	r7,r17
	beqc	r0,r21,0040B710
	addiu	r9,r17,FFFFFFFF
	addiu	r20,r20,00000009
	andi	r17,r9,0000007F
	beqc	r30,r17,0040B7F8
	addiu	r7,sp,FFFFF230
	lsa	r7,r17,r7,00000002
	sw	r21,0E00(r7)
	bc	0040B710
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
	li	r5,0000002E
	bc	0040B4D4
	lw	r17,0008(sp)
	balc	00410170
	movep	r6,r7,r0,r0
	balc	00404330
	restore.jrc	00000260,r30,0000000A
	lw	r7,0004(r16)
	lw	r6,0068(r16)
	bgeuc	r7,r6,0040BC40
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r5,0000(r7)
	bneiuc	r5,00000030,0040BCEC
	lw	r6,0068(r16)
	move	r23,r0
	move	r18,r0
	addiu	r5,r23,FFFFFFFF
	lw	r7,0004(r16)
	sltu	r4,r5,r23
	addiu	r18,r18,FFFFFFFF
	move	r23,r5
	addu	r18,r4,r18
	addiu	r21,r7,00000001
	bgeuc	r7,r6,0040B880
	sw	r21,0001(r16)
	lbu	r5,0000(r7)
	beqic	r5,00000030,0040B854
	li	r7,00000001
	addiu	r21,r0,00000001
	sw	r7,0000(sp)
	bc	0040B434
	move.balc	r4,r16,0040CB78
	move	r5,r4
	bneiuc	r4,00000030,0040B874
	lw	r6,0068(r16)
	bc	0040B854
	andi	r6,r17,0000007F
	addiu	r18,r17,00000001
	beqc	r30,r6,0040B97C
	addiu	r7,sp,FFFFF230
	li	r5,0089705E
	lsa	r7,r6,r7,00000002
	lw	r7,0E00(r7)
	bgeuc	r5,r7,0040B97C
	li	r5,0089705F
	bnec	r7,r5,0040B8DA
	andi	r7,r18,0000007F
	beqc	r30,r7,0040B97C
	addiu	r5,sp,FFFFF230
	lsa	r7,r7,r5,00000002
	li	r5,0F2F09FF
	lw	r7,0E00(r7)
	bgeuc	r5,r7,0040B97C
	slti	r23,r20,0000001C
	li	r5,1DCD6500
	li	r2,001DCD65
	li	r7,00000001
	movn	r2,r5,r23
	addiu	r11,r0,00000009
	addiu	r5,r0,000001FF
	movn	r11,r7,r23
	movz	r7,r5,r23
	move	r23,r7
	lw	r17,0000(sp)
	addu	r7,r7,r11
	sw	r7,0000(sp)
	beqc	r17,r30,0040B898
	move	r7,r17
	move	r19,r0
	bc	0040B91A
	andi	r7,r22,0000007F
	beqc	r30,r7,0040B956
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
	bnezc	r5,0040B912
	andi	r17,r18,0000007F
	andi	r7,r22,0000007F
	addiu	r20,r20,FFFFFFF7
	addiu	r18,r17,00000001
	bnec	r30,r7,0040B91A
	beqc	r0,r19,0040BAB6
	addiu	r6,r30,00000001
	andi	r7,r6,0000007F
	beqc	r17,r7,0040BABC
	addiu	r6,sp,FFFFF230
	lsa	r5,r30,r6,00000002
	andi	r6,r17,0000007F
	move	r30,r7
	sw	r19,0E00(r5)
	bnec	r30,r6,0040B89C
	bneiuc	r20,00000012,0040B8DA
	beqc	r30,r6,0040BBDC
	addiu	r7,sp,FFFFF230
	andi	r18,r18,0000007F
	lsa	r6,r6,r7,00000002
	lw	r4,0E00(r6)
	balc	004101D0
	movep	r6,r7,r0,r0
	balc	0040EFC0
	beqc	r18,r30,0040BB8E
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
	bltc	r16,r0,0040BCAA
	li	r7,00000001
	sw	r16,0014(sp)
	sw	r7,0018(sp)
	move	r7,r16
	bltic	r7,00000035,0040BB3A
	move	r18,r0
	move	r19,r0
	sw	r0,0020(sp)
	sw	r0,0024(sp)
	addiu	r7,r17,00000002
	andi	r7,r7,0000007F
	beqc	r30,r7,0040BA7E
	addiu	r6,sp,FFFFF230
	lsa	r7,r7,r6,00000002
	li	r6,1DCD64FF
	lw	r7,0E00(r7)
	bltuc	r6,r7,0040BC24
	bnezc	r7,0040BA30
	addiu	r7,r17,00000003
	andi	r7,r7,0000007F
	beqc	r30,r7,0040BA4E
	lui	r7,00000412
	lwm	r4,0008(sp),00000002
	lw	r6,0288(r7)
	lw	r7,028C(r7)
	balc	00404330
	movep	r6,r7,r4,r5
	movep	r4,r5,r18,r19
	balc	0040EFC0
	movep	r18,r19,r4,r5
	lw	r17,0014(sp)
	li	r7,00000035
	subu	r7,r7,r6
	bltic	r7,00000002,0040BA7E
	lui	r7,00000412
	lw	r20,0258(r7)
	lw	r21,025C(r7)
	movep	r4,r5,r18,r19
	movep	r6,r7,r20,r21
	balc	0040CF20
	movep	r6,r7,r0,r0
	balc	0040FA70
	bnezc	r4,0040BA7E
	movep	r4,r5,r18,r19
	movep	r6,r7,r20,r21
	balc	0040EFC0
	movep	r18,r19,r4,r5
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
	lw	r17,0000(sp)
	movep	r4,r5,r20,r21
	restore	00000260,r30,0000000A
	bc	0040CFD0
	andi	r6,r17,0000007F
	bc	0040B898
	addiu	r7,r30,FFFFFFFF
	addiu	r5,sp,FFFFF230
	andi	r7,r7,0000007F
	andi	r6,r6,0000007F
	lsa	r7,r7,r5,00000002
	lw	r5,0E00(r7)
	ori	r5,r5,00000001
	sw	r5,0E00(r7)
	bc	0040B898
	sw	r7,0014(sp)
	sw	r0,0018(sp)
	bgeic	r7,00000035,0040B9F8
	bc	0040BB3A
	sw	r0,0000(sp)
	bc	0040B42A
	lw	r17,0014(sp)
	move.balc	r4,r16,0040B250
	beqc	r0,r4,0040BCCE
	addu	r4,r4,r23
	addu	r5,r18,r5
	sltu	r18,r4,r23
	move	r23,r4
	addu	r18,r18,r5
	bc	0040B4F8
	bgeuc	r23,r6,0040B530
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
	li	r6,00000069
	addiu	r10,r0,00000035
	subu	r6,r6,r7
	subu	r10,r10,r7
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
	addiu	r8,r30,00000001
	addiu	r7,sp,FFFFF230
	andi	r30,r8,0000007F
	lsa	r7,r30,r7,00000002
	sw	r0,0DFC(r7)
	bc	0040B9A2
	bnec	r18,r17,0040B502
	bltc	r0,r18,0040B502
	bnezc	r18,0040BBB6
	bltiuc	r23,0000000A,0040BBB6
	bc	0040B502
	lw	r17,0010(sp)
	bgeic	r7,0000001F,0040BBC4
	srlv	r7,r16,r7
	bnec	r0,r7,0040B502
	lw	r17,0008(sp)
	balc	00410170
	movep	r18,r19,r4,r5
	move.balc	r4,r16,004101D0
	movep	r6,r7,r4,r5
	movep	r4,r5,r18,r19
	balc	00404330
	bc	0040B662
	addiu	r8,r30,00000001
	addiu	r7,sp,FFFFF230
	andi	r30,r8,0000007F
	lsa	r7,r30,r7,00000002
	sw	r0,0DFC(r7)
	bc	0040B984
	bgeuc	r7,r23,0040B51E
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
	li	r6,1DCD6500
	beqc	r6,r7,0040BD34
	lwm	r4,0008(sp),00000002
	lui	r7,00000412
	lw	r6,0290(r7)
	lw	r7,0294(r7)
	bc	0040BA40
	move.balc	r4,r16,0040CB78
	move	r5,r4
	bc	0040B848
	balc	0040CF10
	lui	r7,00000412
	lw	r6,0240(r7)
	lw	r7,0244(r7)
	balc	004041D0
	bltc	r4,r0,0040BC82
	lw	r17,0018(sp)
	bnezc	r7,0040BCB8
	move	r17,r0
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
	lw	r17,0000(sp)
	addiu	r30,r30,00000003
	addiu	r7,r7,00000034
	bgec	r7,r30,0040BCA0
	lw	r17,0018(sp)
	beqc	r0,r7,0040BAAA
	movep	r6,r7,r0,r0
	movep	r4,r5,r18,r19
	balc	0040FA70
	beqc	r0,r4,0040BAAA
	balc	004049B0
	li	r7,00000022
	sw	r7,0000(sp)
	bc	0040BAAA
	li	r7,00000001
	addiu	r10,r0,00000035
	li	r6,00000069
	sw	r0,0014(sp)
	sw	r7,0018(sp)
	bc	0040BB46
	lw	r17,0014(sp)
	lw	r17,0028(sp)
	xor	r17,r7,r6
	sltu	r17,r0,r17
	bc	0040BC66
	addiu	r17,r17,00000009
	bc	0040B696
	lui	r7,FFF80000
	bnec	r7,r5,0040BAF8
	lw	r17,0014(sp)
	beqc	r0,r7,0040B65A
	lw	r7,0068(r16)
	beqzc	r7,0040BD58
	lw	r7,0004(r16)
	move	r5,r0
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)
	bc	0040BAF8
	addiu	r21,r0,00000001
	move	r23,r0
	move	r18,r0
	bc	0040B434
	move.balc	r4,r16,004101D0
	lwm	r6,0008(sp),00000002
	balc	00404330
	li	r7,00000008
	subu	r23,r7,r23
	addiupc	r7,00003A99
	movep	r16,r17,r4,r5
	lwxs	r4,r23(r7)
	balc	00410170
	movep	r6,r7,r4,r5
	movep	r4,r5,r16,r17
	balc	0040F5E0
	bc	0040B662
	move.balc	r4,r16,004101D0
	lwm	r6,0008(sp),00000002
	balc	00404330
	bc	0040B662
	addiu	r7,r17,00000003
	lwm	r4,0008(sp),00000002
	andi	r7,r7,0000007F
	bnec	r30,r7,0040BC32
	lui	r7,00000412
	lw	r6,0250(r7)
	lw	r7,0254(r7)
	bc	0040BA40
	move	r18,r0
	bc	0040B704
	move	r5,r0
	bc	0040BAF8

;; __floatscan: 0040BD5C
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
	sw	r5,0001(r21)
	lbu	r4,0000(r7)
	beqic	r4,00000020,0040BD90
	addiu	r7,r4,FFFFFFF7
	bltiuc	r7,00000005,0040BD90
	move	r5,r4
	beqic	r4,0000002B,0040C00A
	beqic	r5,0000002D,0040BEEA
	lui	r7,00000412
	addiu	r8,r0,00000001
	lw	r22,02A0(r7)
	lw	r23,02A4(r7)
	ori	r7,r5,00000020
	bneiuc	r7,00000029,0040BF0E
	lw	r6,0001(r21)
	lw	r7,0068(r21)
	bgeuc	r6,r7,0040BED6
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	bneiuc	r4,0000002E,0040BE78
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BEE0
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	bneiuc	r4,00000026,0040BE78
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BFB6
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	bneiuc	r4,00000029,0040C048
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BFC0
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	bneiuc	r4,0000002E,0040C0CC
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C01C
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	bneiuc	r4,00000029,0040C0D4
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040BECC
	addiu	r5,r6,00000001
	sw	r5,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	bneiuc	r4,00000034,0040C0DC
	lw	r6,0001(r21)
	bgeuc	r6,r7,0040C026
	addiu	r7,r6,00000001
	sw	r7,0001(r21)
	lbu	r4,0000(r6)
	ori	r4,r4,00000020
	beqic	r4,00000039,0040BEA4
	li	r5,00000007
	lw	r7,0068(r21)
	bnec	r0,r16,0040C04A
	beqzc	r7,0040BE80
	lw	r7,0001(r21)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0001(r21)
	balc	004049B0
	li	r7,00000016
	move	r22,r0
	move	r23,r0
	sw	r7,0000(sp)
	movep	r6,r7,r0,r0
	move.balc	r4,r21,0040CB40
	movep	r4,r5,r22,r23
	restore.jrc	00000080,r30,0000000A
	move.balc	r4,r21,0040CB78
	beqic	r4,00000020,0040BD90
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
0040BECC                                     A0 0A A8 0C             ....
0040BED0 F5 84 68 80 7B 1B A0 0A 9E 0C F5 84 68 80 03 1B ..h.{.......h...
0040BEE0 A0 0A 94 0C F5 84 68 80 0F 1B E1 E0 08 20 00 81 ......h...... ..
0040BEF0 01 80 C7 86 98 82 E7 86 9C 82 F5 75 D5 84 68 80 ...........u..h.
0040BF00 C7 88 FA C0 C7 00 01 00 D5 F5 F8 5E BB 1A F3 C8 ...........^....
0040BF10 B8 70 D5 75 F5 84 68 80 E6 88 A6 C1 A6 00 01 00 .p.u..h.........
0040BF20 B5 F5 68 5E C4 80 20 00 D3 C8 4D 0F D5 75 E6 88 ..h^.. ...M..u..
0040BF30 FA C0 A6 00 01 00 B5 F5 68 5E 84 80 20 00 93 C8 ........h^.. ...
0040BF40 37 77 D5 75 E6 88 68 C1 A6 00 01 00 B5 F5 68 5F 7w.u..h.......h_
0040BF50 C6 80 28 10 C0 20 90 33 C0 A8 3C 01 D5 75 81 D0 ..(.. .3..<..u..
0040BF60 E6 88 2E C1 E6 00 01 00 F5 F5 68 5E E4 10 C4 80 ..........h^....
0040BF70 30 80 E0 80 45 E1 DC C8 0C 51 E7 80 41 80 FC C8 0...E....Q..A...
0040BF80 04 D1 82 C8 00 F9 81 C8 1E 48 F5 84 68 80 E0 88 .........H..h...
0040BF90 52 01 F5 75 FF 90 F5 F5 06 B8 E5 1A FF 90 F5 F5 R..u............
0040BFA0 3F 92 7F D3 26 AA F5 3F E1 E0 08 20 C7 86 A8 82 ?...&..?... ....
0040BFB0 E7 86 AC 82 EF 1A A0 0A BE 0B F5 84 68 80 4F 1A ............h.O.
0040BFC0 A0 0A B4 0B F5 84 68 80 5B 1A B1 C8 22 80 D5 75 ......h.[..."..u
0040BFD0 F5 84 68 80 E6 88 5E C0 86 00 01 00 95 F5 68 5E ..h...^.......h^
0040BFE0 C4 80 20 00 C3 C8 02 C1 86 9B F5 75 FF 90 F5 F5 .. ........u....
0040BFF0 30 11 95 10 59 BE CA 83 82 30 FF 29 FD F3 0C B5 0...Y....0.)....
0040C000 A0 0A 74 0B 0C 35 A4 10 BF 19 E1 E0 08 20 00 01 ..t..5....... ..
0040C010 01 00 C7 86 A0 82 E7 86 A4 82 DF 1A A0 0A 58 0B ..............X.
0040C020 F5 84 68 80 15 1A A0 0A 4E 0B 3B 1A A0 0A 48 0B ..h.....N.;...H.
0040C030 F5 84 68 80 05 1B A8 B4 0C B5 A0 0A 3A 0B F5 84 ..h.........:...
0040C040 68 80 A8 34 0C 35 99 1B 83 D2 86 9B D5 75 DF 90 h..4.5.......u..
0040C050 D5 F5 00 8A 4F 3E A0 C8 4B 1E E0 88 47 3E F5 75 ....O>..K...G>.u
0040C060 C7 80 01 80 D5 F5 A0 C8 3B 26 C7 80 02 80 D5 F5 ........;&......
0040C070 A0 C8 31 2E C7 80 03 80 D5 F5 B0 C8 27 3E FC 90 ..1.........'>..
0040C080 F5 F5 F6 BE 8A 1D F5 84 68 80 29 92 D5 75 E6 A8 ........h.)..u..
0040C090 D3 FE A0 0A E2 0A D5 1A E0 88 0D 3F F5 75 C1 E0 ...........?.u..
0040C0A0 08 20 C6 86 A8 82 FF 90 E6 86 AC 82 F5 F5 F5 19 . ..............
0040C0B0 A0 0A C4 0A F5 84 68 80 C4 80 28 10 C0 20 90 33 ......h...(.. .3
0040C0C0 97 1A A0 0A B2 0A F5 84 68 80 59 1A 84 D2 00 AA ........h.Y.....
0040C0D0 79 3F A5 19 85 D2 00 AA 71 3F 9D 19 86 D2 00 AA y?......q?......
0040C0E0 69 3F 95 19 00 AA C1 3E 97 19 D5 75 E6 88 F2 C3 i?.....>...u....
0040C0F0 E6 00 01 00 F5 F5 68 5E 91 C8 F0 83 F5 84 68 80 ......h^......h.
0040C100 C8 12 D5 75 E6 88 3A C1 A6 00 01 00 B5 F5 68 5E ...u..:.......h^
0040C110 81 C8 EF 87 16 11 01 D3 81 C8 48 73 0C B4 0D B4 ..........Hs....
0040C120 0E B4 E1 E0 08 20 C0 12 60 11 E0 12 08 B4 09 B4 ..... ..`.......
0040C130 12 B4 F3 B4 47 84 58 82 67 84 5C 82 5D A4 28 2C ....G.X.g.\.].(,
0040C140 24 81 30 80 3C C9 38 50 A4 80 20 00 E5 80 61 80 $.0.<.8P.. ...a.
0040C150 FC C8 2C 30 91 C8 EA 72 EC 34 E0 A8 F2 00 81 D3 ..,0...r.4......
0040C160 CD B6 EC B4 6E B5 F5 75 B5 84 68 80 A7 88 C0 C0 ....n..u..h.....
0040C170 A7 00 01 00 B5 F5 78 5E 24 81 30 80 2C C9 C9 57 ......x^$.0.,..W
0040C180 81 C8 D5 77 99 C8 08 D0 84 80 20 00 24 81 57 80 ...w...... .$.W.
0040C190 60 A9 5A 80 60 A9 90 00 DC CA 8C 40 CC CA 4E 70 `.Z.`......@..Np
0040C1A0 E1 E0 08 20 10 B5 C7 84 B0 82 9D A4 28 24 E7 84 ... ........($..
0040C1B0 B4 82 71 B5 2F B5 FF 2B 77 81 2F 35 9D A4 28 2C ..q./..+w./5..(,
0040C1C0 20 08 AC 3F DD A4 28 24 FF 2B 65 81 AC BC 9D A4  ..?..($.+e.....
0040C1D0 20 24 00 2A EA 2D 10 35 71 35 9D A4 20 2C F6 00  $.*.-.5q5.. ,..
0040C1E0 01 00 01 D3 C7 22 90 2B C7 12 65 3C 79 1B 20 89 .....".+..e<y. .
0040C1F0 ED 3F F2 34 E9 BB E1 E0 08 20 0F B5 C7 84 50 82 .?.4..... ....P.
0040C200 00 01 01 00 E7 84 54 82 9D A4 28 24 70 B5 12 B5 ......T...($p...
0040C210 FF 2B 1D 81 AC BC 9D A4 20 24 00 2A A2 2D 0F 35 .+...... $.*.-.5
0040C220 70 35 9D A4 20 2C B7 1B F7 82 04 C0 E1 3E AF 1B p5.. ,.......>..
0040C230 0F B5 D0 B4 71 B5 A0 0A 3E 09 0F 35 D0 34 71 35 ....q...>..5.4q5
0040C240 FF 1A A0 0A 32 09 91 C8 CB 86 F5 84 68 80 B3 1A ....2.......h...
0040C250 C0 88 BC 01 60 A9 86 80 60 89 D0 03 AE D2 F6 00 ....`...`.......
0040C260 01 00 F7 82 04 C0 C7 22 90 33 C3 3C E0 C8 90 43 .......".3.<...C
0040C270 F6 00 02 00 F7 82 04 C0 C7 22 90 33 C3 3C E0 C8 .........".3.<..
0040C280 A4 43 F6 00 03 00 F7 82 04 C0 C7 22 90 33 C3 3C .C.........".3.<
0040C290 E0 C8 8C 43 F6 00 04 00 F7 82 04 C0 C7 22 90 33 ...C.........".3
0040C2A0 C3 3C E0 C8 74 43 F6 00 05 00 F7 82 04 C0 C7 22 .<..tC........."
0040C2B0 90 33 C3 3C E0 C8 5C 43 F6 00 06 00 F7 82 04 C0 .3.<..\C........
0040C2C0 C7 22 90 33 C3 3C E0 C8 30 43 76 21 90 5A F7 82 .".3.<..0Cv!.Z..
0040C2D0 04 C0 60 A9 04 00 F7 82 04 C0 A3 C8 F4 82 F5 84 ..`.............
0040C2E0 68 80 E0 88 E6 02 F5 75 63 BC FF 90 F5 F5 E0 8A h......uc.......
0040C2F0 40 01 ED 34 CE 34 07 82 08 80 26 81 01 80 F0 20 @..4.4....&.... 
0040C300 90 33 C1 3C 82 33 62 33 10 82 5E C0 78 B2 0C 53 .3.<.3b3..^.x..S
0040C310 E4 20 90 3B EC B2 7E B3 04 12 A7 12 FE A8 7A 82 . .;..~.......z.
0040C320 E0 88 70 02 87 AA F6 81 F4 88 EC 01 17 A8 60 80 ..p...........`.
0040C330 26 B6 30 12 C8 36 17 12 E9 36 81 E2 08 20 C8 13 &.0..6...6... ..
0040C340 D4 84 50 82 01 30 F4 84 54 82 F6 BE FF 2B 81 7E ..P..0..T....+.~
0040C350 FE BE 04 A8 14 80 F3 34 09 92 F6 BE C7 84 58 82 .......4......X.
0040C360 E7 84 5C 82 00 2A 78 37 AC BC F6 BE 00 2A 50 2C ..\..*x7.....*P,
0040C370 F1 80 01 80 27 22 90 33 BF 92 F6 FE 27 12 A6 3E ....'".3....'..>
0040C380 10 88 BD BF 26 36 DD A6 20 2C 1E 11 F0 12 07 12 ....&6.. ,......
0040C390 D0 00 20 00 06 22 90 3B E7 B1 F5 3C 66 22 90 33 .. ..".;...<f".3
0040C3A0 E9 90 7F B3 07 A8 0A 80 C5 34 78 DB E2 34 F3 88 .........4x..4..
0040C3B0 0A C0 03 B1 31 02 20 00 11 A8 94 81 39 CA 7C AA ....1. .....9.|.
0040C3C0 40 12 60 12                                     @.`.           

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
	move	r6,r16
	movep	r4,r5,r18,r19
	restore	00000080,r30,0000000A
	bc	0040CFD0
	lw	r7,0068(r21)
	beqc	r0,r7,0040C4F0
	lw	r7,0001(r21)
	addiu	r6,r7,FFFFFFFF
	sw	r6,0001(r21)
	beqc	r0,r16,0040C4F4
	addiu	r6,r7,FFFFFFFE
	sw	r6,0001(r21)
	lw	r17,0030(sp)
	beqzc	r6,0040C432
	addiu	r7,r7,FFFFFFFD
	sw	r7,0001(r21)
	move.balc	r4,r8,00410170
	movep	r6,r7,r0,r0
	balc	00404330
	movep	r22,r23,r4,r5
	bc	0040BEA4
	beqzc	r6,0040C410
	lwm	r6,0030(sp),00000002
	movz	r7,r22,r6
	sw	r7,0034(sp)
	lw	r17,0038(sp)
	movz	r7,r11,r6
	sw	r7,0038(sp)
	bltc	r0,r11,0040C2DA
	bnec	r0,r11,0040C25E
	bltiuc	r22,00000008,0040C25E
	bc	0040C2DA
	lw	r5,0001(r21)
	lw	r7,0068(r21)
	bgeuc	r5,r7,0040C50A
	addiu	r7,r5,00000001
	sw	r7,0001(r21)
	lbu	r4,0000(r5)
	bneiuc	r4,00000030,0040C500
	lw	r7,0068(r21)
	move	r22,r0
	sw	r30,0030(sp)
	move	r23,r0
	move	r30,r20
	move	r20,r19
	move	r19,r16
	move	r16,r8
	lw	r6,0001(r21)
	addiu	r5,r22,FFFFFFFF
	sltu	r9,r5,r22
	addiu	r4,r23,FFFFFFFF
	bgeuc	r6,r7,0040C4C6
	addiu	r23,r6,00000001
	move	r22,r5
	sw	r23,0001(r21)
	addu	r23,r9,r4
	lbu	r4,0000(r6)
	beqic	r4,00000030,0040C48C
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
	addiu	r23,r23,FFFFFFFF
	move.balc	r4,r21,0040CB78
	addiu	r7,r22,FFFFFFFF
	sltu	r6,r7,r22
	move	r22,r7
	addu	r23,r23,r6
	bneiuc	r4,00000030,0040C4B0
	lw	r7,0068(r21)
	bc	0040C48C
	sw	r8,0030(sp)
	move.balc	r4,r21,0040CB78
	lw	r18,0030(sp)
	bc	0040C0F8
	move	r6,r0
	bc	0040C118
	bnec	r0,r16,0040C432
	movep	r6,r7,r0,r0
	sw	r8,0008(sp)
	move.balc	r4,r21,0040CB40
	lw	r18,0008(sp)
	bc	0040C432
	li	r7,00000001
	sw	r0,0034(sp)
	sw	r7,0030(sp)
	sw	r0,0038(sp)
	bc	0040C122
	sw	r6,0020(sp)
	sw	r8,0030(sp)
	move.balc	r4,r21,0040CB78
	lw	r17,0020(sp)
	lw	r18,0030(sp)
	bc	0040C476
	lw	r17,0018(sp)
	bgeuc	r4,r7,0040C32C
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

;; fn0040C548: 0040C548
fn0040C548 proc
	sh	r19,0006(r6)
	movep	r22,r23,r4,r5
	bc	0040BEA4
0040C550 F3 34 54 D3 02 B5 87 84 58 82 A7 84 5C 82 00 2A .4T.....X...\..*
0040C560 DE 09 02 35 72 FE 00 08 06 3C AC BC 72 BE 00 2A ...5r....<..r..*
0040C570 8E 09 02 35 72 FE                               ...5r.         

l0040C576:
	movep	r6,r7,r0,r0
	lwm	r4,0020(sp),00000002

;; fn0040C57C: 0040C57C
fn0040C57C proc
	sw	r8,0008(sp)
	balc	0040FA70
0040C582       02 35 80 88 3D 3E F4 CA 39 06 E9 92 08 B4   .5..=>..9.....
0040C590 09 B4 31 1A C7 34 86 88 8B FD 02 B5 FF 2B 11 84 ..1..4.......+..
0040C5A0 02 35 A2 D3 01 E2 08 20                         .5.....        

;; fn0040C5A8: 0040C5A8
fn0040C5A8 proc
	sw	r7,0000(sp)
	move.balc	r4,r8,00410170
	lw	r6,0220(r16)
	lw	r7,0224(r16)
	balc	00404330
0040C5BA                               D0 84 20 82 F0 84           .. ...
0040C5C0 24 82 FF 2B 6B 7D F6 FE FF 29 D9 F8 80 10 A0 10 $..+k}...)......
0040C5D0 1D 19 15 BE 0C B5 FF 2B 77 EC 0C 35 80 A8 0F 3D .......+w..5...=
0040C5E0 E0 E0 01 00 A7 A8 07 3D 1C 98 F5 84 68 80 DF 9B .......=....h...
0040C5F0 F5 75 A0 10 FF 90 F5 F5 F5 18 C0 A8 CD 3C DB 18 .u...........<..
0040C600 C0 A8 6D 3C D5 18 6B BC A0 0A 34 05 C0 12 E0 12 ..m<..k...4.....
0040C610 FF 29 91 F8 C0 A8 A1 3C C1 18 C0 A8 89 3C BB 18 .).....<.....<..
0040C620 C0 A8 71 3C B5 18 C0 A8 59 3C AF 18 DC CA 2D 44 ..q<....Y<....-D
0040C630 AD 18 FF 2B 7B 83 A2 D3 C0 97 C9 19 F3 34 54 D3 ...+{........4T.
0040C640 ED B0 02 B5 87 84 58 82 A7 84 5C 82 00 2A F0 08 ......X...\..*..
0040C650 02 35 72 FE 00 08 18 3B AC BC 72 BE             .5r....;..r.   

;; fn0040C65C: 0040C65C
fn0040C65C proc
	balc	0040CF00
0040C660 02 35 72 FE                                     .5r.           

;; fn0040C664: 0040C664
fn0040C664 proc
	bltic	r17,00000020,0040C576

l0040C668:
	bc	0040C3C4
0040C66A                               00 00 00 00 00 00           ......

;; __intscan: 0040C670
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
	lw	r7,0004(r16)
	bgeuc	r7,r19,0040C708
	addiu	r6,r7,00000001
	sw	r6,0004(sp)
	lbu	r4,0000(r7)
	beqic	r4,00000020,0040C688
	addiu	r7,r4,FFFFFFF7
	bltiuc	r7,00000005,0040C688
	beqic	r4,0000002B,0040C742
	beqic	r4,0000002D,0040C742
	move	r23,r0
	beqc	r0,r30,0040C716
	beqic	r30,00000010,0040C95A
	addiupc	r22,00006BB3
	lbux	r7,r4(r22)
	bgeuc	r7,r30,0040C728
	addiupc	r22,00006BA5
	beqic	r30,0000000A,0040C75A
	addiu	r7,r30,FFFFFFFF
	lbux	r20,r4(r22)
	and	r7,r7,r30
	beqc	r0,r7,0040C9A2
	move	r17,r0
	mul	r6,r17,r30
	li	r7,071C71C6
	bgeuc	r20,r30,0040C8A2
	bltuc	r7,r17,0040C8A2
	lw	r7,0004(r16)
	addu	r17,r20,r6
	addiu	r5,r7,00000001
	bgeuc	r7,r19,0040C808
	sw	r5,0004(sp)
	lbu	r4,0000(r7)
	lbux	r20,r4(r22)
	bc	0040C6DE
	move.balc	r4,r16,0040CB78
	lw	r19,0068(r16)
	beqic	r4,00000020,0040C688
	bc	0040C69A
	beqic	r4,00000030,0040C95E
	addiupc	r7,00006B4D
	lbux	r7,r7(r4)
	bltiuc	r7,0000000A,0040C75A
	beqzc	r19,0040C730
	lw	r7,0004(r16)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(sp)
	movep	r6,r7,r0,r0
	move.balc	r4,r16,0040CB40

l0040C736:
	balc	004049B0
0040C73A                               96 D3 C0 97 63 BC           ....c.
0040C740 5A 1D 81 17 84 80 2D 60 80 20 D0 B9 67 8A 3A C2 Z.....-`. ..g.:.
0040C750 C7 00 01 00 01 97 78 5E 53 1B 20 12 31 22 0F 3C ......x^S. .1".<
0040C760 C4 80 30 80 A0 60 98 99 99 19 87 20 0F 3A CC C8 ..0..`..... .:..
0040C770 2C 50 25 AA 28 C0 01 17 27 82 30 80 66 8A A4 C0 ,P%.(...'.0.f...
0040C780 E6 00 01 00 A0 60 98 99 99 19 81 97 31 22 0F 3C .....`......1".<
0040C790 68 5E C4 80 30 80 87 20 0F 3A DC C8 D5 57 40 12 h^..0.. .:...W@.
0040C7A0 CC C8 A2 50 E0 60 99 99 99 19 47 AA 8A C0 F2 88 ...P.`....G.....
0040C7B0 7C 00 92 33 D1 83 5E C0 92 82 02 C0 FA B0 D4 23 |..3..^........#
0040C7C0 90 A2 E5 20 90 3B 92 3E 05 81 5F C0 F4 3C 86 82 ... .;.>.._..<..
0040C7D0 9F C0 F1 33 80 22 D0 F2 07 21 90 3A D1 32 C0 20 ...3."...!.:.2. 
0040C7E0 D0 42 FE A8 52 C0 C7 8B F2 02 52 B3 01 16 B1 20 .B..R.....R.... 
0040C7F0 90 2B F4 3C D4 B3 64 8A 1C C0 E4 00 01 00 81 97 .+.<..d.........
0040C800 48 5E C4 80 30 80 99 1B 00 0A 6C 03 70 86 68 80 H^..0.....l.p.h.
0040C810 C4 22 07 A1 C9 1A 00 0A 5E 03 70 86 68 80 C4 80 ."......^.p.h...
0040C820 30 80 7D 1B 00 0A 50 03 70 86 68 80 2F 1B E0 60 0.}...P.p.h./..`
0040C830 99 99 99 99 27 8A 7B FF C3 62 2F 6A 00 00 C4 22 ....'.{..b/j..."
0040C840 07 39 FC C8 B2 50 86 99 81 17 FF 90 81 97 E1 34 .9...P.........4
0040C850 F2 88 1A C0 37 22 10 8B 57 22 10 2B F1 22 D0 21 ....7"..W".+.".!
0040C860 E5 22 D0 B9 91 20 90 8B 37 22 D0 29 5A 1D 73 D9 ."... ..7".)Z.s.
0040C870 E2 34 F1 A8 DF FF E2 34 F4 C8 08 00 E0 8A 62 02 .4.....4......b.
0040C880 E0 82 01 80 E1 34 47 AA 0A C0 47 AA C7 3F E2 34 .....4G...G..?.4
0040C890 27 8A C1 FF FF 2B 19 81 A2 D3 C0 97 A1 34 82 34 '....+.......4.4
0040C8A0 5A 1D 40 12 DE 10 E0 10 D4 8B 9B FF 7F D2 FF D2 Z.@.............
0040C8B0 00 2A FC 21 B0 77 D2 23 18 38 40 81 01 80 D1 23 .*.!.w.#.8@....#
0040C8C0 D8 58 55 00 01 00 45 AA 34 C0 B2 88 CA 01 E3 3C .XU...E.4......<
0040C8D0 D1 23 18 30 80 22 D0 2A 47 89 C2 01 86 22 50 89 .#.0.".*G...."P.
0040C8E0 D1 20 90 93 A4 B3 75 8A 62 C0 50 84 04 90 95 84 . ....u.b.P.....
0040C8F0 00 20 C4 22 07 A1 AD 1B B0 77 C0 03 0A 00 75 8A . .".....w....u.
0040C900 18 C0 F5 00 01 00 81 97 F5 84 00 20 C7 22 07 39 ........... .".9
0040C910 C7 8B 1A C0 B0 77 75 AA E9 FF 00 0A 5A 02 C4 22 .....wu.....Z.."
0040C920 07 39 C7 8B 08 C0 70 86 68 80 B0 77 E9 1B FF 2B .9....p.h..w...+
0040C930 7F 80 A2 D3 C0 97 E2 34 E4 C8 58 00 F0 84 68 80 .......4..X...h.
0040C940 E0 12 41 36 22 36 E0 A8 FF 3E 09 1B 00 0A 28 02 ..A6"6...>....(.
0040C950 70 86 68 80 C4 22 07 A1 4B 1B 91 C8 2A 81 81 17 p.h.."..K...*...
0040C960 67 8A 40 C1 C7 00 01 00 01 97 78 5E E4 80 20 00 g.@.......x^.. .
0040C970 E3 C8 3A C1 C0 AB 4B 3D C3 62 EF 68 00 00 85 D3 ..:...K=.b.h....
0040C980 C4 22 07 A1 C0 03 08 00 26 18 00 0A EA 01 70 86 ."......&.....p.
0040C990 68 80 19 19 F0 84 68 80 E0 88 6C 01 41 36 22 36 h.....h...l.A6"6
0040C9A0 A7 1A DE 23 0F 3A F3 33 C7 23 D0 39 E7 80 85 F0 ...#.:.3.#.9....
0040C9B0 C0 04 AC 68 20 12 E6 20 07 29 E0 60 FF FF FF 07 ...h .. .).`....
0040C9C0 B1 20 10 30 D4 8B 2E C0 27 AA 2A C0 81 17 D4 20 . .0....'.*.... 
0040C9D0 90 8A 67 8A 0E C0 C7 00 01 00 01 97 78 5E C4 22 ..g.........x^."
0040C9E0 07 A1 D7 1B A3 B4 00 0A 8E 01 70 86 68 80 A3 34 ..........p.h..4
0040C9F0 C4 22 07 A1 C5 1B 60 80 01 80 A0 20 D0 11 43 20 ."....`.... ..C 
0040CA00 10 38 A3 20 50 18 A0 20 10 3A 43 11 65 81 20 40 .8. P.. .:C.e. @
0040CA10 E3 20                                           .              

;; fn0040CA12: 0040CA12
fn0040CA12 proc
	bc	0040CCA4
0040CA14             6A 21 10 1A 40 12 60 21 10 52 51 20     j!..@.`!.RQ 
0040CA20 50 20 B2 20 10 38 A0 20 10 22 B1 20 10 30 D4 8B P . .8. .". .0..
0040CA30 15 FE CC 53 B0 77 4A AA C5 FE 95 00 01 00 66 21 ...S.wJ.......f!
0040CA40 10 3A 52 89 3C 00 60 21 10 32 47 12 D4 20 90 8A .:R.<.`!.2G.. ..
0040CA50 75 8A 0C C0 01 96 95 84 00 20 C4 22 07 A1 BF 1B u........ ."....
0040CA60 A3 B4 64 B4 5D A5 14 2C 47 B4 00 0A 0A 01 C4 22 ..d.]..,G......"
0040CA70 07 A1 5D A5 14 24 70 86 68 80 A3 34 64 34 47 34 ..]..$p.h..4d4G4
0040CA80 9D 1B 23 8A C1 FF 77 1A C3 62 DF 67 00 00 C4 22 ..#...w..b.g..."
0040CA90 07 39 FC C8 37 84 91 18 24 8A 33 FE 61 1A C5 88 .9..7...$.3.a...
0040CAA0 3B FE 5B 1A 00 0A D0 00 70 86 68 80 BF 1A 81 17 ;.[.....p.h.....
0040CAB0 67 8A 62 C0 C7 00 01 00 01 97 78 5E C3 62 AB 67 g.b.......x^.b.g
0040CAC0 00 00 C4 22 07 A1 9C CA 36 80 E0 99 81 17 C7 80 ..."....6.......
0040CAD0 01 80 CC 98 FE 90 63 BC 81 97 5A 1D A8 88 0B FD ......c...Z.....
0040CAE0 57 19 FF 2B CB 7E C2 34 E6 10 FF 90 C7 20 90 2B W..+.~.4..... .+
0040CAF0 C1 34 A6 82 01 80 22 D3 40 97 B5 3C 87 10 5A 1D .4....".@..<..Z.
0040CB00 83 D3 C0 03 10 00 A9 1A E0 8A D7 3F E0 82 01 80 ...........?....
0040CB10 41 36 22 36 3F 19 00 0A 5E 00 70 86 68 80 9D 1B A6"6?...^.p.h...
0040CB20 01 97 6B BC 00 0A 18 00 63 BC 5A 1D 20 AA 0F 3C ..k.....c.Z. ..<
0040CB30 F1 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; __shlim: 0040CB40
;;   Called from:
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
	bltc	r7,r9,0040CB6E
	beqc	r9,r7,0040CB6A
	sw	r5,0068(r4)
	jrc	ra
	bgeuc	r6,r8,0040CB64
	addu	r5,r10,r6
	sw	r5,0068(r4)
	jrc	ra

;; __shgetc: 0040CB78
;;   Called from:
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
;;     00407FE4 (in __libc_sigaction)
__syscall_ret proc
	lui	r7,FFFFFFFF
	bltuc	r7,r4,0040CC3A

l0040CC38:
	jrc	ra

l0040CC3A:
	save	00000010,ra,00000002
	move	r16,r4
	balc	004049B0
0040CC42       00 22 D0 39 C0 97 7F D2 12 1F 00 00 00 00   .".9..........

;; __vdsosym: 0040CC50
;;   Called from:
;;     0040AEBC (in cgt_init)
__vdsosym proc
	save	00000030,r30,0000000A
	addiupc	r7,00023BED
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

;; fn0040CC9A: 0040CC9A
;;   Called from:
;;     0040CCA8 (in fn0040CCA8)
;;     0040CCA8 (in fn0040CCA8)
fn0040CC9A proc
	beqic	r8,00000002,0040CD24

l0040CC9E:
	addiu	r16,r16,00000001
	addu	r6,r6,r10
	beqc	r16,r9,0040CCBC

;; fn0040CCA4: 0040CCA4
;;   Called from:
;;     0040CA12 (in fn0040CA12)
;;     0040CCB8 (in fn0040CCAC)
fn0040CCA4 proc
	addiu	r0,r22,00007406

l0040CCA6:
	lw	r8,0000(r6)

;; fn0040CCA8: 0040CCA8
;;   Called from:
;;     0040CCA4 (in fn0040CCA4)
;;     0040CCA6 (in fn0040CCAC)
fn0040CCA8 proc
	bneiuc	r8,00000001,0040CC9A

;; fn0040CCAC: 0040CCAC
;;   Called from:
;;     0040CC62 (in __vdsosym)
;;     0040CC66 (in __vdsosym)
;;     0040CC7C (in __vdsosym)
;;     0040CC8A (in __vdsosym)
;;     0040CC98 (in __vdsosym)
;;     0040CCA4 (in fn0040CCA4)
;;     0040CCA4 (in fn0040CCA4)
;;     0040CCA8 (in fn0040CCA8)
;;     0040CCA8 (in fn0040CCA8)
fn0040CCAC proc
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
	illegal
	move	r7,r22
	ext	r5,r5,00000000,0000000F
	bc	0040CD8C
	addu	r7,r7,r6
	illegal
	bbnezc	r6,00000000,0040CD9A
	lhu	r6,0004(r7)
	ext	r6,r6,00000000,0000000F
	beqc	r5,r6,0040CDA6
	lw	r6,0010(r7)
	bnezc	r6,0040CD8A

l0040CD9E:
	lw	r8,0001(r23)
	bc	0040CD44

l0040CDA2:
	li	r16,00000004
	bc	0040CC78
0040CDA6                   73 17 E6 20 07 2C B5 3C 9F 0A       s.. .,.<..
0040CDB0 7F DA 6B BA                                     ..k.           

l0040CDB4:
	lw	r4,0004(r18)
	addu	r4,r17,r4
	restore.jrc	00000030,r30,0000000A
0040CDBA                               00 00 00 00 00 00           ......

;; calloc: 0040CDC0
;;   Called from:
;;     00406054 (in netlink_msg_to_ifaddr)
calloc proc
	beqzc	r5,0040CDD0

l0040CDC2:
	li	r6,FFFFFFFF
	illegal
	illegal
	bltuc	r7,r4,0040CDD6

l0040CDD0:
	mul	r4,r4,r5
	bc	0040579A
0040CDD6                   11 1E FF 2B D5 7B 8C D3 C0 97       ...+.{....
0040CDE0 80 10 11 1F 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; __expand_heap: 0040CDF0
__expand_heap proc
	save	00000020,ra,00000004
	addiupc	r18,00023B1D
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
	nor	r7,r0,r4
	bgeuc	r16,r7,0040CE42
	lw	r7,0010(r18)
	lui	r8,00000800
	addu	r5,r16,r4
	bgeuc	r8,r7,0040CED4
	move	r8,r7
	illegal
	movep	r8,r20,r5,r6
	bgeuc	r8,r5,0040CE82
	bgeuc	r4,r7,0040CE82
	lwpc	r5,00432EF0
	addiu	r7,r0,00000802
	srl	r5,r5,00000001
	move	r10,r0
	sllv	r5,r6,r5
	move	r11,r0
	sltu	r4,r5,r16
	addiu	r8,r0,FFFFFFFF
	movz	r16,r5,r4
	li	r6,00000003
	movep	r4,r5,r0,r16
	balc	00405BE2
	li	r7,FFFFFFFF
	beqc	r4,r7,0040CEF4
	lwpc	r7,00432EF0
	sw	r16,0000(r17)
	addiu	r7,r7,00000001
	illegal
	addiu	r0,r2,00001F24
	addiu	r7,sp,0000000C
	lui	r8,00000800
	bgeuc	r8,r7,0040CED8
	move	r8,r7
	illegal
	movep	r8,r20,r5,r6
	bgeuc	r8,r5,0040CE9C
	bltuc	r4,r7,0040CE42
	addiu	r4,r0,000000D6
	balc	00404A50
	lwpc	r7,00432EF4
	addu	r6,r16,r7
	beqc	r4,r6,0040CEE8
	lw	r6,0024(r18)
	bc	0040CE42
	move	r5,r0
	addiu	r4,r0,000000D6
	balc	00404A50
	lw	r6,0024(r18)
	subu	r7,r0,r4
	addiu	r5,r6,FFFFFFFF
	and	r7,r7,r5
	addu	r4,r7,r4
	illegal
	addiu	r0,r2,00001B4B
	move	r8,r0
	bc	0040CE3A
	move	r8,r0
	bc	0040CE94

l0040CEDC:
	balc	004049B0
0040CEE0 8C D3 C0 97 80 10 24 1F 8F 60 06 60 02 00 11 F6 ......$..`.`....
0040CEF0 87 10 24 1F 80 10 24 1F 00 00 00 00 00 00 00 00 ..$...$.........

;; copysignl: 0040CF00
;;   Called from:
;;     0040C65C (in fn0040C65C)
copysignl proc
	bc	0040E120
0040CF04             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; fabs: 0040CF10
fabs proc
	move	r6,r4
	ext	r7,r5,00000000,0000001F
	movep	r4,r5,r6,r7
	jrc	ra
0040CF1A                               00 00 00 00 00 00           ......

;; fmodl: 0040CF20
fmodl proc
	bc	0040E140
0040CF24             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; frexpl: 0040CF30
frexpl proc
	bc	0040E2C0
0040CF34             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; scalbn: 0040CF40
;;   Called from:
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
0040CF60 20 FC 12 8A 18 80 F3 84 BC 82 11 82 FE 87 D3 84  ...............
0040CF70 B8 82 58 38 F0 80 00 44 20 FC F2 20 10 82       ..X8...D .. .. 

l0040CF7E:
	addiu	r16,r16,000003FF
	move	r4,r0
	sll	r5,r16,00000014
	movep	r6,r7,r4,r5
	movep	r4,r5,r8,r9
	balc	0040CFCC
0040CF8E                                           25 1F               %.

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
0040CFAC                                     20 FC 50 8A              .P.
0040CFB0 CD BF F3 84 C4 82 11 02 92 07 D3 84 C0 82 0C 38 ...............8
0040CFC0 50 22 50 3B 20 FC F2 20 10 86 B3 1B             P"P; .. ....   

l0040CFCC:
	bc	00404330

;; scalbnl: 0040CFD0
scalbnl proc
	bc	0040CF40
0040CFD4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; mbstowcs: 0040CFE0
mbstowcs proc
	save	00000020,ra,00000001
	move	r7,r0
	sw	r5,000C(sp)
	addiu	r5,sp,0000000C
	balc	0040E410
0040CFEC                                     21 1F 00 00             !...

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
	illegal
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
	addiu	r0,r8,00000000
	nop
	nop

;; __lockfile: 0040D1D0
;;   Called from:
;;     004081E8 (in fflush_unlocked)
;;     00408250 (in fgets_unlocked)
;;     004083A2 (in fputc)
;;     00408594 (in fwrite_unlocked)
;;     004085DE (in _IO_getc)
;;     00408722 (in _IO_putc)
;;     0040D1CC (in __restore)
;;     0040D4D0 (in __isoc99_vfscanf)
;;     0040E6EE (in close_file)
__lockfile proc
	save	00000010,ra,00000003
	move	r16,r4
	illegal
	lw	r7,004C(r4)
	move	r4,r0
	lw	r17,-0094(r3)
	bnec	r17,r7,0040D1F0
	restore.jrc	00000010,ra,00000003
	li	r7,00000001
	addiu	r5,r16,00000050
	balc	0040ADB0
	addiu	r4,r16,0000004C
	illegal
	illegal
	bnezc	r6,0040D206
	move	r7,r17
	illegal
	beqzc	r7,0040D1F8
	illegal
	bnezc	r6,0040D1E6
	li	r4,00000001
	restore.jrc	00000010,ra,00000003

;; __unlockfile: 0040D210
;;   Called from:
;;     00408272 (in fgets_unlocked)
;;     004082A6 (in fgets_unlocked)
;;     004085A6 (in fwrite_unlocked)
;;     00409AA4 (in fn00409A7A)
;;     0040D4E6 (in __isoc99_vfscanf)
__unlockfile proc
	save	00000010,ra,00000002
	illegal
	sw	r0,004C(r4)
	illegal
	lw	r7,0050(r4)
	beqzc	r7,0040D248
	addiu	r16,r4,0000004C
	li	r7,00000001
	addiu	r6,r0,00000081
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r4,r7,0040D248
	li	r7,00000001
	li	r4,00000062
	movep	r5,r6,r16,r7
	restore	00000010,ra,00000002
	bc	00404A50
	restore.jrc	00000010,ra,00000002
	sigrie	00000000
	addiu	r0,r0,00001E22

;; __overflow: 0040D250
;;   Called from:
;;     0040839E (in fputc)
;;     004083DC (in fputc)
;;     0040871E (in _IO_putc)
;;     0040875C (in _IO_putc)
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
0040D27E                                           5D 9A               ].

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
0040D2BE                                           8B 17               ..
0040D2C0 0C 17 87 97 7C B3 D0 A4 10 2C 93 10 36 1F 80 17 ....|....,..6...
0040D2D0 04 94 E7 80 20 00 05 94 80 97 07 94 80 CA 38 10 .... .........8.
0040D2E0 91 17 B7 B3 E5 1B 90 17 7E B2 90 97 91 17 79 B2 ........~.....y.
0040D2F0 11 96                                           ..             

l0040D2F2:
	lw	r5,003C(r16)
	li	r4,00000042
	movep	r6,r7,r17,r20
	balc	00404A50
0040D2FC                                     FF 2B 31 F9             .+1.
0040D300 92 88 BB 3F 04 A8 C7 BF 91 17 25 B2 87 88 D7 FF ...?......%.....
0040D310 C9 B3 92 90 9F 92 CF 1B 60 12 AF 1B 00 00 00 00 ........`.......

;; __stdout_write: 0040D320
__stdout_write proc
	save	00000020,ra,00000004
	movep	r16,r17,r4,r5
	addiupc	r7,FFFFFFBC
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
0040D342       06 9A FF D3 F0 84 4B 10                     ......K.     

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
;;     004085DA (in _IO_getc)
;;     0040CB8E (in __shgetc)
__uflow proc
	save	00000020,ra,00000002
	move	r16,r4
	balc	0040E750
0040D3E8                         16 BA 88 17 01 D3 BD 00         ........
0040D3F0 0F 00 90 10 F0 D8 90 C8 06 08 9D 84 0F 20 22 1F ............. ".
0040D400 7F D2 22 1F 00 00 00 00 00 00 00 00 00 00 00 00 ..".............

;; store_int: 0040D410
store_int proc
	beqzc	r4,0040D432

l0040D412:
	addiu	r5,r5,00000002
	bgeiuc	r5,00000006,0040D432

l0040D418:
	addiupc	r8,00003010
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
0040D444             10 D3 DD 20 50 29 9D 10 FF 2B E1 CC     ... P)...+..
0040D450 FD 84 0C 00 C0 10 80 10 A0 34 06 18 D1 92 01 D2 .........4......
0040D460 1F 92 1C CA 12 10 E0 88 F3 BF FC 90 01 D3 E7 20 ............... 
0040D470 08 00 07 88 EB BF E5 1B 02 9A A0 B4 04 9B FD 84 ................
0040D480 0C 10 E0 88 18 80 C7 80 04 80 C6 20 08 00 DD 84 ........... ....
0040D490 0C 10 06 A8 08 80 C1 34 EF B3 70 16 32 1F E0 34 .......4..p.2..4
0040D4A0 71 93 C0 B4 F5 1B                               q.....         

;; __isoc99_vfscanf: 0040D4A6
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
0040D4D4             88 B4                                   ..         

l0040D4D6:
	move	r23,r0
	sw	r0,0008(sp)
	sw	r0,0010(sp)
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
	illegal
	move	r4,r30
	addu	r16,r16,r7
	movep	r6,r7,r0,r0
	balc	0040CB40
	lw	r7,0004(r30)
	lw	r6,0068(r30)
	bgeuc	r7,r6,0040D562
	addiu	r6,r7,00000001
	sw	r6,0004(r30)
	lbu	r4,0000(r7)
	lbu	r7,0000(r16)
	beqc	r4,r7,0040D56A
	lw	r7,0068(r30)
	beqzc	r7,0040D548
	lw	r7,0004(r30)
	addiu	r7,r7,FFFFFFFF
	sw	r7,0004(r30)
	bgec	r4,r0,0040D4E0
	lw	r17,0008(sp)
	li	r7,FFFFFFFF
	movz	r6,r7,r6
	sw	r6,0008(sp)
	bc	0040D4E0
	move	r4,r30
	balc	0040CB78
	bc	0040DB7C
	move	r4,r30
	balc	0040CB78
	bc	0040D532
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
	addiupc	r5,00002F4C
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
0040D5E4             44 12 F0 00 03 00 9B 1B                 D.......   

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
	bc	0040D588
0040D632       79 5F A0 82 01 80 D3 C8 08 40 07 02 02 00   y_.......@....
0040D640 A0 82 02 80 88 5C F1 80 2F 20 F0 C8 08 18 31 82 .....\../ ....1.
0040D650 20 00 A0 02 01 00 23 CA BE 18 23 CA CC 70 32 CA  .....#...#..p2.
0040D660 D4 D8 E3 34 9E 10 C3 34 E7 80 9F C0 EA B4 FF 2B ...4...4.......+
0040D670 CF F4 FE 84 04 80 DE 84 68 80 C7 88 2C C1 E9 90 ........h...,...
0040D680 FE 84 04 90 FE 84 68 80 8A 9B FE 84 04 80 FF 90 ......h.........
0040D690 FE 84 04 90 23 CA 4E 24 2B CA 9E 29 22 CA 00 C2 ....#.N$+..)"...
0040D6A0 2A CA 1C C9 22 CA 50 0C 3A CA 08 08 31 82 45 80 *...".P.:...1.E.
0040D6B0 3C CA 44 1C DE 84 08 80 FE 84 04 80 9E 84 7C 80 <.D...........|.
0040D6C0 7F B3 DE 84 78 80 A7 80 9F C0 7C B3 5A B2 E6 20 ....x.....|.Z.. 
0040D6D0 90 3B 84 34 FE B2 D7 3C E6 22 90 2B CE B3 DE B3 .;.4...<.".+....
0040D6E0 E6 12 E4 B4 40 8A E0 04 E2 34 E9 90 E2 B4 00 28 ....@....4.....(
0040D6F0 D6 04 79 5F A0 02 01 00 D3 C8 49 67 07 02 02 00 ..y_......Ig....
0040D700 A0 02 03 00 3F 1B A0 02 02 00 39 1B 07 12 A0 12 ....?.....9.....
0040D710 33 1B A0 02 01 00 2D 1B E3 34 A3 34 E0 20 50 33 3.....-..4.4. P3
0040D720 81 D3 C7 20 10 2A A3 B4 39 1B E4 34 F5 BF 5F 0A ... .*..9..4.._.
0040D730 DF FC 00 28 92 04 6B BC 9E 10 FF 2B 03 F4 FE 84 ...(..k....+....
0040D740 04 80 DE 84 68 80 C7 88 58 C0 C7 00 01 00 DE 84 ....h...X.......
0040D750 04 90 78 5E 81 C8 E7 07 84 80 09 80 9C C8 DF 2F ..x^.........../
0040D760 FE 84 68 80 8A 9B FE 84 04 80 FF 90 FE 84 04 90 ..h.............
0040D770 DE 84 08 80 FE 84 04 80 9E 84 7C 80 7F B3 DE 84 ..........|.....
0040D780 78 80 A7 80 9F C0 7C B3 5A B2 E6 20 90 3B 84 34 x.....|.Z.. .;.4
0040D790 FE B2 D7 3C E6 22 90 2B CE B3 DE B3 E6 12 E4 B4 ...<.".+........
0040D7A0 C1 1A 9E 10 FF 2B D1 F3 AB 1B 9E 10 FF 2B C9 F3 .....+.......+..
0040D7B0 04 88 D1 BE                                     ....           

l0040D7B4:
	lw	r17,0008(sp)
	li	r7,FFFFFFFF
	movz	r6,r7,r6

l0040D7BC:
	sw	r6,0008(sp)
	bc	0040D974
0040D7C0 23 CA 34 0B 23 CA E6 18 32 CA E9 DE 89 5F E2 C8 #.4.#...2...._..
0040D7D0 B4 F1 09 92 C0 12 1D 01 5C 00 C0 00 01 01 C0 BE ........\.......
0040D7E0 0B B5 FF 2B AB CE 08 5F FD 80 A0 8E 0B 35 07 84 ...+..._.....5..
0040D7F0 FC 1E D1 C8 98 69 01 D3 09 92 C6 22 D0 31 C7 84 .....i.....".1..
0040D800 2A 1F 88 5F E2 C8 D2 E8 AB 9B F1 C8 16 68 09 5F *.._.........h._
0040D810 12 9B C2 C8 0E E8 F0 A4 FF 90 B6 80 01 10 C7 A8 ................
0040D820 7E 81 09 92 88 5F DD 80 A0 8E 09 92 EE B3 01 D3 ~...._..........
0040D830 C6 22 D0 31 C7 84 FD 1E C9 1B 23 CA AC 7A 2B CA .".1......#..z+.
0040D840 52 80 3B CA B2 42 A0 10 33 CA 69 4E 00 81 01 80 R.;..B..3.iN....
0040D850 20 81 01 80 C0 10 9E 10 FF 2B 15 EE DE 86 04 80  ........+......
0040D860 AC BC BE 84 08 80 1E 85 78 80 B6 20 D0 B1 BE 84 ........x.. ....
0040D870 7C 80 36 81 9F C0 16 3C C8 22 90 B3 25 3C C1 3E |.6....<."..%<.>
0040D880 C8 22 90 B2 C0 8A EC 00 33 CA 62 82 40 8A 5E 02 ."......3.b.@.^.
0040D890 20 97 21 1A 23 CA 16 98 2B CA 08 A0 33 CA 15 86  .!.#...+...3...
0040D8A0 90 D2 A9 1B 23 CA 3E AA 23 CA F5 C7 07 1A C0 00 ....#.>.#.......
0040D8B0 01 01 FF D2 57 72 FF 2B D7 CD 1D 84 5C 10 33 CA ....Wr.+....\.3.
0040D8C0 E6 98 1D 84 66 10 1D 84 67 10 1D 84 68 10 1D 84 ....f...g...h...
0040D8D0 69 10 1D 84 6A 10 1D 84 7D 10 C0 02 1F 00 B0 CA i...j...}.......
0040D8E0 32 09 E5 34 92 12 8E 9B 96 80 02 C0 FF 2B A3 79 2..4.........+.y
0040D8F0 84 12 80 88 02 03 00 11 11 B4 12 B4 FE 84 04 80 ................
0040D900 DE 84 68 80 C7 88 02 C1 C7 00 01 00 DE 84 04 90 ..h.............
0040D910 78 5E FD 80 A0 8E 0B B5 7E B2 E7 84 FD 2E E0 A8 x^......~.......
0040D920 96 00 51 72 60 12 00 2A D6 0A 80 88 87 3E 0B 35 ..Qr`..*.....>.5
0040D930 DE 84 68 80 FE 84 04 80 06 9B FF 90 FE 84 04 90 ..h.............
0040D940 DE 84 08 80 FE 84 04 80 9E 84 7C 80 7F B3 DE 84 ..........|.....
0040D950 78 80 A7 80 9F C0 7C B3 5A B2 E6 20 90 3B FE B2 x.....|.Z.. .;..
0040D960 E6 20 90 2A 8E 9A 33 CA 64 1A A3 34 E3 DA CA 34 . .*..3.d..4...4
0040D970 E6 88 7C 02                                     ..|.           

l0040D974:
	lw	r17,0014(sp)
	beqc	r0,r7,0040D4E0

l0040D97A:
	move.balc	r4,r19,00404F2E
	move.balc	r4,r20,00404F2E
	bc	0040D4E0
0040D986                   0A 92 C0 02 01 00 49 1A D2 C8       ......I...
0040D990 71 EE 01 D3 09 92 C6 22 D0 31 C7 84 5A 1F 63 1A q......".1..Z.c.
0040D9A0 E9 90 E8 20 87 28 73 1A C0 02 1F 00 33 CA 2F 1F ... .(s.....3./.
0040D9B0 E3 34 C7 02 01 00 27 1B D1 73 9D 84 3F 10 01 D3 .4....'..s..?...
0040D9C0 BD 00 3F 00 50 72 00 2A 56 09 E0 80 02 80 0B 35 ..?.Pr.*V......5
0040D9D0 E4 88 29 3F FF D3 E4 88 70 01 80 8A 08 00 F0 34 ..)?....p......4
0040D9E0 88 22 C7 3C 09 91 E5 34 E0 88 11 3F C8 AA 0D 3F .".<...4...?...?
0040D9F0 C8 82 01 C0 C9 92 0B B5 B6 80 02 C0 9F 0A D1 7D ...............}
0040DA00 80 88 F6 01 84 12 0B 35 F3 1A 9E 10 0B B5 FC 39 .......5.......9
0040DA10 0B 35 FF 1A E5 34 DE 9B DF 0A 77 78 64 12 80 88 .5...4....wxd...
0040DA20 D6 01 00 11 FE 84 04 80 DE 84 68 80 C7 88 3C C0 ..........h...<.
0040DA30 C7 00 01 00 DE 84 04 90 78 5E FD 80 A0 8E 7E B2 ........x^....~.
0040DA40 E7 84 FD 2E 84 BB 80 12 E7 1A 88 02 01 00 13 21 ...............!
0040DA50 87 20 93 10 96 AA 0E 00 D6 82 01 C0 C9 92 DF 0B . ..............
0040DA60 6F 7D 80 88 A2 01 14 11 64 12 B9 1B 9E 10 0B B5 o}......d.......
0040DA70 9A 39 0B 35 C5 1B 60 12 30 B9 FE 84 04 80 DE 84 .9.5..`.0.......
0040DA80 68 80 C7 88 56 C0 C7 00 01 00 DE 84 04 90 78 5E h...V.........x^
0040DA90 FD 80 A0 8E 78 B2 E4 84 FD 2E DF BB 00 11 80 12 ....x...........
0040DAA0 60 12 8D 1A 72 22 87 20 69 92 FE 84 04 80 13 11 `...r". i.......
0040DAB0 DE 84 68 80 C7 88 1A C0 C7 00 01 00 DE 84 04 90 ..h.............
0040DAC0 78 5E FD 80 A0 8E 7E B2 E7 84 FD 2E D7 BB 72 12 x^....~.......r.
0040DAD0 75 1B 9E 10 6B B6 34 39 0B 35 E7 1B 9E 10 2C 39 u...k.49.5....,9
0040DAE0 AF 1B A0 95 F2 18 8A D2 63 19 88 D2 5F 19 B2 BE ........c..._...
0040DAF0 FF 2B 1D F9 FF 29 BD FB 75 BD 9E 10 FF 2B 5D E2 .+...)..u....+].
0040DB00 DE 84 08 80 FE 84 04 80 DE 86 7C 80 7F B3 DE 84 ..........|.....
0040DB10 78 80 27 82 9F C0 7C B3 36 3E E6 20 90 3B FE B0 x.'...|.6>. .;..
0040DB20 EC 53 20 FC E0 88 4D 3E 40 8A 89 3B A0 CA 12 08 .S ...M>@..;....
0040DB30 A0 CA 0E 10 A0 AA 7D 3B 00 2A E4 26 20 96 FF 29 ......};.*.& ..)
0040DB40 73 FB 12 A5 00 2C FF 29 6B FB 60 12 67 18       s....,.)k.`.g. 

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
0040DB66                   FE 84 04 80 DE 84 68 80 C7 88       ......h...
0040DB70 E7 F9 C7 00 01 00 DE 84 04 90 78 5E 81 C8 E7 07 ..........x^....
0040DB80 84 80 09 80 9C C8 DF 2F FE 84 68 80 8A 9B FE 84 ......./..h.....
0040DB90 04 80 FF 90 FE 84 04 90 DE 84 08 80 FE 84 04 80 ................
0040DBA0 9E 84 7C 80 7F B3 DE 84 78 80 A7 80 9F C0 7C B3 ..|.....x.....|.
0040DBB0 5A B2 E6 20 90 3B 84 34 FE B2 D7 3C E6 22 90 2B Z.. .;.4...<.".+
0040DBC0 CE B3 E6 12 DE B3 E4 B4 09 92 FF 29 0F F9 E5 34 ...........)...4
0040DBD0 8A 9B B0 CA 0D 0F 92 F6 23 CA D9 1A 80 8A 04 00 ........#.......
0040DBE0 88 22 C7 04 60 8A CD 3A 68 22 87 00 FF 29 C5 FA ."..`..:h"...)..
0040DBF0 E5 34 E0 88 BF 3A DB 1B 80 12 60 12 C2 34 FF D3 .4...:....`..4..
0040DC00 C7 20 10 32 C2 B4 73 19 80 12 F1 1B             . .2..s.....   

;; fn0040DC0C: 0040DC0C
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
	illegal
	movep	r22,r23,r6,r7
	and	r7,r7,r10
	li	r10,80808080
	and	r7,r7,r10
	bnezc	r7,0040DC80
	addiu	r8,r8,00000004
	addiu	r9,r9,00000004
	sw	r6,-0004(r8)
	lw	r6,0000(r9)
	move	r7,r6
	nor	r5,r0,r6
	illegal
	movep	r22,r23,r6,r7
	and	r7,r7,r5
	li	r5,80808080
	and	r7,r7,r5
	beqzc	r7,0040DC52
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
	illegal
	movep	r22,r23,r6,r7
	and	r7,r7,r8
	li	r8,80808080
	and	r7,r7,r8
	beqzc	r7,0040DCFE
	bc	0040DD24
	lw	r4,0000(r5)
	move	r7,r4
	nor	r8,r0,r4
	illegal
	movep	r22,r23,r6,r7
	and	r7,r7,r8
	li	r8,80808080
	and	r7,r7,r8
	bnezc	r7,0040DD0C
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
0040DD20 90 10 12 1F                                     ....           

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
0040DD46                   40 14 44 72 63 BD 00 2A 50 0A       @.Drc..*P.
0040DD50 48 72 63 BD 00 2A 48 0A 80 00 B2 00 FF 2B F1 6C Hrc..*H......+.l
0040DD60 83 B4 AB 60 B0 51 02 00 A2 B4 00 80 06 C0 E2 04 ...`.Q..........
0040DD70 A6 51 C7 A4 00 51 E6 DA C2 73 82 04 9A 51 E4 A4 .Q...Q...s...Q..
0040DD80 00 59 EB 9B 00 80 06 C0 E2 34 C7 A8 D5 3F A3 34 .Y.......4...?.4
0040DD90 00 80 06 C0 E2 04 74 51 C7 A4 00 51 C5 88 44 00 ......tQ...Q..D.
0040DDA0 00 80 06 C0 E3 34 A0 E0 01 00 DC 53 77 DB C0 00 .....4.....Sw...
0040DDB0 87 00 A2 04 56 51 62 D2 FF 2B 95 6C 44 72 00 2A ....VQb..+.lDr.*
0040DDC0 8E 0A EB 60 40 51 02 00 8B 60 36 51 02 00 F0 D8 ...`@Q...`6Q....
0040DDD0 48 72 00 2A FA 09 44 72 00 2A 74 0A FF 2B D1 6B Hr.*..Dr.*t..+.k
0040DDE0 04 F6 42 1F E0 10 82 04 22 51 E4 A4 00 59 A5 9B ..B....."Q...Y..
0040DDF0 AF 1B                                           ..             

;; __synccall: 0040DDF2
__synccall proc
	save	00000960,ra,00000007
	addiu	r6,r0,00000820
	addiupc	r21,00023319
	movep	r18,r19,r4,r5
	move	r5,r0
	addiu	r4,sp,00000120
	balc	0040E008
0040DE08                         C0 00 8C 00 A0 10 65 72         ......er
0040DE10 F6 39 FD 80 C0 86 07 12 FF 04 25 FF 45 72 F0 84 .9........%.Er..
0040DE20 54 97 E0 E0 00 02 F0 84 D8 97 FF 2B 87 A0 82 04 T..........+....
0040DE30 DE 50 FF 2B FB CE 80 10 FF 2B 65 A0 C2 72 01 D2 .P.+.....+e..r..
0040DE40 FF 2B 0D D0 F5 75 0F 60 CC 50 02 00 E0 88 2E 01 .+...u.`.P......
0040DE50 4F 62 B2 50 02 00 6F 62 A8 50 02 00 00 80 06 C0 Ob.P..ob.P......
0040DE60 81 D3 EF 60 98 50 02 00 00 80 06 C0 C0 00 80 00 ...`.P..........
0040DE70 FF D2 66 72 92 39 C0 10 E5 72 22 D2 FF 2B BB A0 ..fr.9...r"..+..
0040DE80 80 00 AC 00 86 39 24 12 80 00 B2 00 7E 39 A9 E0 .....9$.....~9..
0040DE90 00 00 84 12 80 04 B8 41 00 2A 04 02 90 84 E0 97 .......A.*......
0040DEA0 04 88 54 80 00 80 06 C0 0F 60 52 50 02 00 00 80 ..T......`RP....
0040DEB0 06 C0 E0 60 FF FF FF 7F C0 00 81 00 A3 60 3E 50 ...`.........`>P
0040DEC0 02 00 62 D2 46 39 E0 80 26 80 79 DA E0 60 FF FF ..b.F9..&.y..`..
0040DED0 FF 7F 01 D3 A3 60 26 50 02 00 62 D2 2E 39 82 34 .....`&P..b..9.4
0040DEE0 A0 10 FF 2B 6B CF 82 04 26 50 FF 2B 73 CE 45 72 ...+k...&P.+s.Er
0040DEF0 FF 2B D5 9F E7 83 63 39 1D 77 18 B8 00 12 9D 00 .+....c9.w......
0040DF00 20 01 00 2A 0A 01 1A BA 54 98 9D 00 20 01 00 2A  ..*....T... ..*
0040DF10 5E 01 E9 1B 22 D3 80 00 81 00 1F 92 3F 0B 31 6B ^...".......?.1k
0040DF20 D9 1B E4 84 13 20 E7 80 30 80 EC C8 D1 57 84 00 ..... ..0....W..
0040DF30 13 00 FF 2B 9B BC C4 10 84 8A C3 3F 41 9A 00 80 ...+.......?A...
0040DF40 06 C0 8F 60 C4 4F 02 00 00 80 06 C0 EB 60 C6 4F ...`.O.......`.O
0040DF50 02 00 D4 9B F1 16 C5 88 A5 3F F0 17 F5 1B 9D 84 .........?......
0040DF60 20 81 FF 2B 0D D0 0B 62 AC 4F 02 00 1C B8 81 D3  ..+...b.O......
0040DF70 C0 10 E5 72 22 D2 FD 84 94 90 FF 2B BD 9F 93 10 ...r"......+....
0040DF80 50 DA 8B 60 90 4F 02 00 1A 18 02 92 00 2A 40 08 P..`.O.......*@.
0040DF90 06 92 00 2A BA 08 00 14 D3 1B 40 14 42 92 00 2A ...*......@.B..*
0040DFA0 2E 08 90 10 75 BA FD 1A A2 D3 80 00 83 00 3F 0B ....u.........?.
0040DFB0 9F 6A E0 80 03 80 E4 88 45 3F C3 72 80 10 FF 2B .j......E?.r...+
0040DFC0 33 CF E4 34 A0 60 FF C9 9A 3B C7 10 C1 60 80 96 3..4.`...;...`..
0040DFD0 98 00 C4 B4 C5 88 0E 80 C3 34 E1 60 80 CC FD C4 .........4.`....
0040DFE0 C9 90 DD A4 0C 2C C3 73 C0 00 86 00 07 11 A2 04 .....,.s........
0040DFF0 1A 4F E0 10 62 D2 14 38 80 88 03 3F E0 80 03 80 .O..b..8...?....
0040E000 E4 88 FB 3E 09 92 F7 1A                         ...>....       

l0040E008:
	bc	0040A690

;; fn0040E00C: 0040E00C
fn0040E00C proc
	bc	00404A50

;; readdir64: 0040E010
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
0040E02E                                           44 12               D.
0040E030 80 A8 18 80 E4 00 02 00 20 12 E0 80 41 E0 A4 9B ........ ...A...
0040E040 FF 2B 6D 69 40 22 D0 91 40 95 18 18 04 94 05 96 .+mi@"..@.......

l0040E050:
	lw	r6,0010(r16)
	addu	r17,r17,r6
	lhu	r7,0010(r17)
	addu	r7,r7,r6
	lw	r6,0008(r17)
	sw	r7,0010(sp)
	lw	r7,000C(r17)
	swm	r6,0008(r16),00000002
	move	r4,r17
	restore.jrc	00000010,ra,00000004
	sigrie	00000000
	sigrie	00000000

;; rewinddir: 0040E070
rewinddir proc
	save	00000010,ra,00000003
	move	r16,r4
	addiu	r17,r4,00000018
	move.balc	r4,r17,0040AD30
	lw	r4,0000(r16)
	movep	r6,r7,r0,r0
	move	r8,r0
	balc	0040E860
0040E084             91 10 6B BC D0 A4 08 2C 04 94 05 94     ..k....,....
0040E090 E3 83 12 30 FF 29 C9 CC 00 00 00 00 00 00 00 00 ...0.)..........

;; open64: 0040E0A0
open64 proc
	addiu	sp,sp,FFFFFFE0
	save	00000020,ra,00000003
	move	r16,r5
	swm	r6,0028(sp),00000002
	swm	r8,0030(sp),00000002
	swm	r10,0038(sp),00000002
	bbnezc	r5,00000006,0040E0C4
	lui	r7,00000410
	move	r8,r0
	and	r6,r5,r7
	bnec	r6,r7,0040E0DA
	addiu	r7,sp,00000040
	lw	r18,0028(sp)
	sw	r7,0000(sp)
	sw	r7,0004(sp)
	sw	r7,0008(sp)
	li	r7,00000040
	sb	r7,000D(sp)
	li	r7,00000014
	sb	r7,000C(sp)
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
	bbeqzc	r16,00000013,0040E106
	move	r5,r4
	li	r7,00000001
	li	r6,00000002
	li	r4,00000019
	balc	00404A50
	move.balc	r4,r17,0040CC30
	restore	00000020,ra,00000003
	addiu	sp,sp,00000020
	jrc	ra
	sigrie	00000000
	sigrie	00000000
	sigrie	00000000

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
0040E180 AC BC 00 2A 5A 14 D7 FE                         ...*Z...       

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
0040E1B2       D3 1B                                       ..           

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
	illegal
	movep	r16,r23,r5,r6
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
0040E2D4             24 9A E1 E0 08 20 C7 84 C8 82 E7 84     $.... ......
0040E2E0 CC 82 11 BE FF 2B 49 60 D2 10 D5 3B A0 17 11 FE .....+I`...;....
0040E2F0 E7 80 40 80 A0 97                               ..@...         

l0040E2F6:
	movep	r4,r5,r17,r16
	restore.jrc	00000010,ra,00000004
0040E2FA                               20 94 F9 1B                  ... 

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
mbrtowc proc
	save	00000020,ra,00000001
	move	r2,r4
	addiupc	r4,000125FC
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
	illegal
	lw	r4,-0038(r3)
	lw	r4,0000(r4)
	bnezc	r4,0040E37E
	lb	r7,0000(r5)
	addiu	r6,r0,0000DFFF
	li	r4,00000001
	and	r7,r7,r6
	sw	r7,0000(r2)
	restore.jrc	00000020,ra,00000001
	addiu	r8,r8,FFFFFF3E
	bgeiuc	r8,00000033,0040E3F2
	addiupc	r4,000027F3
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
0040E3F8                         D4 D3 C0 97 7F D2               ...... 

l0040E3FE:
	restore.jrc	00000020,ra,00000001

;; mbsinit: 0040E400
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
	illegal
	lw	r7,-0038(r3)
	lw	r7,0000(r7)
	bnezc	r7,0040E49A
	move	r5,r6
	bnezc	r17,0040E48A
	move	r4,r16
	restore	00000010,ra,00000004
	bc	0040A890
	lbu	r7,0000(r16)
	beqzc	r7,0040E490
	seb	r7,r7
	addiu	r4,r0,0000DFFF
	addiu	r17,r17,00000004
	and	r7,r7,r4
	addiu	r16,r16,00000001
	addiu	r5,r5,FFFFFFFF
	sw	r7,-0004(r17)
	bnezc	r5,0040E472

l0040E48C:
	sw	r16,0000(r18)
	bc	0040E496
0040E490 ED B2 10 94 20 94                               .... .         

l0040E496:
	move	r4,r6
	restore.jrc	00000010,ra,00000004
0040E49A                               E6 10 A4 98                 .... 

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
	lw	r9,0000(r5)
	subu	r8,r7,r5
	addu	r8,r8,r16
	move	r4,r9
	illegal
	movep	r22,r23,r6,r7
	or	r4,r4,r9
	li	r9,80808080
	and	r4,r4,r9
	beqzc	r4,0040E504
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
0040E504             D1 92 C9 1B                             ....       

l0040E508:
	addiu	r5,r5,FFFFFF3E
	bgeiuc	r5,00000033,0040E536

l0040E510:
	addiupc	r4,0000272E
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
0040E55C                                     D4 D3 7F D3             ....
0040E560 C0 97 20 AA 27 3F 2F 1B                         .. .'?/.       

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
0040E57E                                           11 F4               ..
0040E580 FC 90 89 5E 91 96 8A 5E 01 90 92 96 94 90 B0 A4 ...^...^........
0040E590 FF 90 B1 A4 FC C8                               ......         

l0040E596:
	lbu	r8,0000(r16)
	bltiuc	r7,00000005,0040E4AE

l0040E59E:
	lw	r4,0000(r16)
	move	r5,r4
	illegal
	movep	r22,r23,r6,r7
	or	r5,r5,r4
	li	r4,80808080
	and	r5,r5,r4
	beqzc	r5,0040E57E
	bc	0040E4AE

l0040E5B6:
	addiu	r5,r5,FFFFFF3E
	bgeiuc	r5,00000033,0040E536

l0040E5BE:
	addiupc	r4,000026D7
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
	li	r7,00000001
	sb	r5,0000(r4)

l0040E612:
	move	r4,r7
	restore.jrc	00000010,ra,00000001
0040E616                   7D 20 C0 01 C3 A4 C8 C0 60 17       } ......`.
0040E620 18 BB C5 10 C1 60 80 20 FF FF E6 A8 E1 FF FF 2B .....`. .......+
0040E630 7F 63 D4 D3 C0 97 FF D3 D9 1B E0 00 FF 07 A7 A8 .c..............
0040E640 1C C0 E5 80 86 C0 C0 80 40 80 EC 53 A5 80 3F 20 ........@..S..? 
0040E650 C4 5F E0 80 80 80 FC 52 82 D3 C5 5E B5 1B E0 00 ._.....R...^....
0040E660 FF D7 A7 88 10 C0 E5 10 C0 00 FF 1F E1 60 00 20 .............`. 
0040E670 FF FF E6 A8 24 C0 E5 80 8C C0 C0 80 20 80 EC 53 ....$....... ..S
0040E680 C0 80 80 80 C4 5F E5 80 46 F1 A5 80 3F 20 EC 53 ....._..F...? .S
0040E690 EC 52 C5 5F C6 5E 83 D3 79 1B E5 10 C0 60 FF FF .R._.^..y....`..
0040E6A0 0F 00 E1 60 00 00 FF FF E6 A8 83 FF E5 80 92 C0 ...`............
0040E6B0 C0 80 10 80 EC 53 C5 80 4C F1 C4 5F E0 80 80 80 .....S..L.._....
0040E6C0 7C 53 45 5F C5 80 46 F1 A5 80 3F 20 7C 53 FC 52 |SE_..F...? |S.R
0040E6D0 46 5F 84 D3 C7 5E 3B 1B 00 00 00 00 00 00 00 00 F_...^;.........

;; close_file: 0040E6E0
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
	lw	r7,0024(r16)
	move	r4,r16
	movep	r5,r6,r0,r0
	jalrc	ra,r7
	lwm	r6,0004(r16),00000002
	bgeuc	r6,r7,0040E71E
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
;;     0040D3D2 (in __towrite_needs_stdio_exit)
;;     0040E79A (in __toread_needs_stdio_exit)
__stdio_exit_needed proc
	save	00000010,ra,00000002
	balc	00408610
0040E726                   40 14 06 18 1F 0A B3 FF 0E 14       @.........
0040E730 79 B8 8B 60 F8 47 02 00 FF 2B A5 FF 8B 60 BE 1B y..`.G...+...`..
0040E740 02 00 E2 83 12 30 FF 29 97 FF 00 00 00 00 00 00 .....0.)........

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
	lw	r7,0024(r4)
	movep	r5,r6,r0,r0
	jalrc	ra,r7
	lw	r4,0000(r16)
	sw	r0,0010(sp)
	sw	r0,0014(sp)
	sw	r0,001C(sp)
	bbnezc	r4,00000002,0040E790
	lw	r7,002C(r16)
	ext	r4,r4,00000004,00000001
	lw	r6,0030(r16)
	subu	r4,r0,r4
	addu	r7,r7,r6
	sw	r7,0004(sp)
	sw	r7,0008(sp)
	restore.jrc	00000010,ra,00000002
	ori	r4,r4,00000020
	sw	r4,0000(sp)
	li	r4,FFFFFFFF
	restore.jrc	00000010,ra,00000002

;; __toread_needs_stdio_exit: 0040E79A
__toread_needs_stdio_exit proc
	bc	0040E720
0040E79E                                           00 00               ..

;; sem_init: 0040E7A0
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
0040E7BA                               96 D3 C0 97 7F D2           ......
0040E7C0 11 1F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; sem_post: 0040E7D0
sem_post proc
	save	00000010,ra,00000002
	lw	r6,0008(r4)
	move	r16,r4
	lw	r7,0000(r16)
	li	r5,7FFFFFFF
	lw	r9,0001(r16)
	bnec	r5,r7,0040E7EE

l0040E7E2:
	balc	004049B0
0040E7E6                   CB D3 C0 97 7F D2 12 1F             ........ 

l0040E7EE:
	addiu	r5,r7,00000001
	srl	r4,r7,0000001F
	addu	r5,r5,r4
	illegal
	illegal
	bnec	r7,r8,0040E80C
	move	r4,r5
	illegal
	beqzc	r4,0040E7FC
	illegal
	bnec	r7,r8,0040E7D6
	bltc	r7,r0,0040E820
	bnec	r0,r9,0040E820
	move	r4,r0
	restore.jrc	00000010,ra,00000002
	addiu	r7,r0,00000080
	movn	r6,r7,r6
	li	r7,00000001
	or	r6,r6,r7
	li	r4,00000062
	move.balc	r5,r16,00404A50
	addiu	r7,r0,FFFFFFDA
	bnec	r7,r4,0040E81C
	li	r7,00000001
	li	r4,00000062
	movep	r5,r6,r16,r7
	balc	00404A50
	bc	0040E81C
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,000010A0

;; sem_wait: 0040E850
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
0040E874             FF 2B B9 E3 7F D3 FF D3 04 BA DD A4     .+..........
0040E880 08 24 E6 BC 21 1F 00 00 00 00 00 00 00 00 00 00 .$..!...........

;; cleanup: 0040E890
cleanup proc
	illegal
	illegal
	addiu	r7,r7,FFFFFFFF
	illegal
	beqzc	r7,0040E894
	illegal
	jrc	ra

;; sem_timedwait: 0040E8A6
;;   Called from:
;;     0040E852 (in sem_wait)
sem_timedwait proc
	save	00000020,ra,00000004
	movep	r16,r18,r4,r5
	balc	0040EA42
0040E8AE                                           00 0A               ..
0040E8B0 7E 00 E5 D3 08 BA 80 10 24 1F 00 80 06 C0 FF 90 ~.......$.......
0040E8C0 8A 9B 00 17 C0 A8 04 80 01 17 6F 9B 00 0A 60 00 ..........o...`.
0040E8D0 65 9A 01 93 00 80 06 C0 E6 A4 00 51 E9 90 E6 A4 e..........Q....
0040E8E0 00 59 F5 9B 00 80 06 C0 00 80 06 C0 F0 A4 00 51 .Y.............Q
0040E8F0 88 BB FF D3 F0 A4 00 59 F3 9B 00 80 06 C0 BF 04 .......Y........
0040E900 8F FF 41 72 FF 2B 2B C5 18 74 4B BE FF D2 00 0A ..Ar.++..tK.....
0040E910 6E 00 81 D2 24 12 41 72 FF 2B 1F C5 F1 10 E0 80 n...$.Ar.+......
0040E920 82 E0 A9 9B FF 2B 89 60 C0 94 7F D2 24 1F 00 00 .....+.`....$...

;; sem_trywait: 0040E930
sem_trywait proc
	save	00000010,ra,00000001
	lw	r7,0000(r4)
	bltc	r0,r7,0040E944

l0040E938:
	balc	004049B0
0040E93C                                     8B D3 C0 97             ....
0040E940 7F D2 11 1F                                     ....           

l0040E944:
	addiu	r5,r7,FFFFFFFF
	move	r6,r0
	bneiuc	r7,00000001,0040E954

l0040E94E:
	lw	r6,0004(r4)
	sltu	r6,r0,r6

l0040E954:
	subu	r5,r5,r6
	illegal
	illegal
	bnec	r7,r8,0040E96A
	move	r6,r5
	illegal
	beqzc	r6,0040E95A
	illegal
	bnec	r7,r8,0040E932
	move	r4,r0
	restore.jrc	00000010,ra,00000001
	sigrie	00000000
	sigrie	00000000
	addiu	r0,r0,00001E35

;; __timedwait_cp: 0040E980
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
	li	r4,00000016
	restore.jrc	00000030,ra,00000005
	addiu	r5,sp,00000008
	move.balc	r4,r6,0040AEF4
	bnezc	r4,0040E99C
	lw	r17,0008(sp)
	lw	r5,0000(r16)
	lw	r17,000C(sp)
	subu	r5,r5,r7
	lw	r7,0004(r16)
	sw	r5,0008(sp)
	subu	r7,r7,r6
	sw	r7,000C(sp)
	bgec	r7,r0,0040E9C8
	addiu	r5,r5,FFFFFFFF
	illegal
	balc	0040ED5E
	sw	r5,0008(sp)
	sw	r7,000C(sp)
	lw	r17,0008(sp)
	addiu	r16,sp,00000008
	bgec	r7,r0,0040E9D6
	li	r4,0000006E
	restore.jrc	00000030,ra,00000005

l0040E9D4:
	move	r16,r0
	movep	r7,r8,r18,r16
	movep	r5,r6,r17,r19
	move	r10,r0
	move	r9,r0
	li	r4,00000062
	balc	0040ADA4
0040E9E4             C0 80 26 80 E4 10 80 20 D0 21 79 DB     ..&.... .!y.
0040E9F0 40 11 20 11 1A BF 71 BD 62 D2 FF 2B A7 C3 80 20 @. ...q.b..+... 
0040EA00 D0 21 80 C8 0C 20 83 C8 C7 77 E4 80 7D 10 E0 20 .!... ...w..}.. 
0040EA10 10 26 35 1F                                     .&5.           

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
0040EA26                   01 35 E2 34 C3 34 30 BE FF 2B       .5.4.40..+
0040EA30 4F FF A0 10 04 12 87 34 FF 2B 15 C4 90 10 33 1F O......4.+....3.

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
__udivdi3 proc
	move	r8,r5
	move	r10,r4
	movep	r9,r5,r6,r7
	move	r11,r8
	bnec	r0,r5,0040EC5A

l0040EABC:
	bgeuc	r8,r9,0040EB4C
	clz	r7,r6
	beqzc	r7,0040EADE
	subu	r11,r0,r7
	sllv	r8,r8,r7
	srlv	r11,r4,r11
	sllv	r9,r6,r7
	or	r11,r11,r8
	sllv	r10,r4,r7
	ext	r8,r9,00000000,00000010
	srl	r2,r9,00000010
	illegal
	mul	r4,r8,r3
	srl	r7,r10,00000010
	modu	r6,r11,r2
	sll	r6,r6,00000010
	or	r6,r6,r7
	move	r7,r3
	bgeuc	r6,r4,0040EB14
	addu	r6,r6,r9
	addiu	r7,r7,FFFFFFFF
	bltuc	r6,r9,0040EB14
	bgeuc	r6,r4,0040EB14
	addiu	r7,r3,FFFFFFFE
	addu	r6,r6,r9
	subu	r6,r6,r4
	illegal
	mul	r8,r8,r11
	modu	r4,r6,r2
	ext	r10,r10,00000000,00000010
	sll	r6,r4,00000010
	or	r10,r6,r10
	move	r4,r11
	bgeuc	r10,r8,0040EB42
	addu	r10,r10,r9
	addiu	r4,r4,FFFFFFFF
	bltuc	r10,r9,0040EB42
	bgeuc	r10,r8,0040EB42
	addiu	r4,r11,FFFFFFFE
	sll	r7,r7,00000010
	or	r7,r7,r4

l0040EB48:
	move	r4,r7
	jrc	ra
0040EB4C                                     20 A9 06 00              ...
0040EB50 81 D3 C7 20 98 49 C9 20 3F 5B 6E BB 28 21 D0 41 ... .I. ?[n.(!.A
0040EB60 81 D2 69 81 C0 F3 49 80 50 C0 48 20 98 19 6B 20 ..i...I.P.H ..k 
0040EB70 18 20 EA 80 50 C0 48 20 D8 31 C6 80 10 C0 7C 53 . ..P.H .1....|S
0040EB80 E3 10 86 88 12 C0 C1 3C FF 90 26 A9 0A C0 86 88 .......<..&.....
0040EB90 06 C0 E3 80 02 80 C1 3C 6D B2 46 20 98 19 6B 20 .......<m.F ..k 
0040EBA0 18 40 46 20 D8 21 4A 81 C0 F3 C4 80 10 C0 46 21 .@F .!J.......F!
0040EBB0 90 52 83 10 0A 89 8B FF 41 3C 9F 90 2A A9 83 FF .R......A<..*...
0040EBC0 0A 89 7F FF 83 80 02 80 79 1B C9 20 10 48 A0 D3 ........y.. .H..
0040EBD0 7F B3 A9 80 C0 F3 E8 20 50 18 C4 20 10 50 C8 20 ....... P.. .P. 
0040EBE0 10 40 69 81 50 C0 63 21 98 11 45 20 18 30 E4 20 .@i.P.c!..E .0. 
0040EBF0 50 38 07 21 90 42 63 21 D8 39 88 80 50 C0 E7 80 P8.!.Bc!.9..P...
0040EC00 10 C0 CC 53 82 10 C7 88 12 C0 E1 3C 9F 90 27 A9 ...S.......<..'.
0040EC10 0A C0 C7 88 06 C0 82 80 02 80 E1 3C 7D B3 66 21 ...........<}.f!
0040EC20 D8 39 66 21 98 11 45 20 18 30 E7 80 10 C0 08 81 .9f!..E .0......
0040EC30 C0 F3 07 21 90 42 E2 10 C8 88 12 C0 01 3C FF 90 ...!.B.......<..
0040EC40 28 A9 0A C0 C8 88 06 C0 E2 80 02 80 01 3C A4 80 (............<..
0040EC50 10 C0 C8 20 D0 41 FC 52 09 1B                   ... .A.R..     

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
	illegal
	mul	r5,r9,r12
	srl	r8,r11,00000010
	modu	r10,r13,r7
	sll	r10,r10,00000010
	sllv	r6,r6,r3
	or	r10,r10,r8
	move	r8,r12
	bgeuc	r10,r5,0040ECD8
	addu	r10,r10,r2
	addiu	r8,r8,FFFFFFFF
	bltuc	r10,r2,0040ECD8
	bgeuc	r10,r5,0040ECD8
	addiu	r8,r12,FFFFFFFE
	addu	r10,r10,r2
	subu	r10,r10,r5
	illegal
	mul	r9,r9,r12
	modu	r5,r10,r7
	ext	r11,r11,00000000,00000010
	sll	r5,r5,00000010
	or	r5,r5,r11
	move	r7,r12
	bgeuc	r5,r9,0040ED10
	addu	r5,r5,r2
	addiu	r7,r7,FFFFFFFF
	bltuc	r5,r2,0040ED10
	bgeuc	r5,r9,0040ED10
	addiu	r7,r12,FFFFFFFE
	addu	r5,r5,r2
	sll	r8,r8,00000010
	subu	r9,r5,r9
	or	r7,r8,r7
	mul	r8,r7,r6
	illegal
	bltuc	r9,r6,0040ED36
	move	r5,r0
	bnec	r9,r6,0040EB48
	sllv	r6,r4,r3
	bgeuc	r6,r8,0040EB48
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
__umoddi3 proc
	movep	r9,r11,r6,r7
	movep	r8,r10,r4,r5
	bnec	r0,r11,0040EEA8

l0040ED58:
	bgeuc	r10,r9,0040EDDE
	clz	r11,r6
	beqc	r0,r11,0040ED7C
	subu	r10,r0,r11
	sllv	r5,r5,r11
	srlv	r10,r4,r10
	sllv	r9,r6,r11
	or	r10,r10,r5
	sllv	r8,r4,r11
	ext	r4,r9,00000000,00000010
	srl	r5,r9,00000010
	illegal
	mul	r6,r6,r4
	modu	r7,r10,r5
	srl	r10,r8,00000010
	sll	r7,r7,00000010
	or	r7,r7,r10
	bgeuc	r7,r6,0040EDAA
	addu	r7,r7,r9
	bltuc	r7,r9,0040EDAA
	bgeuc	r7,r6,0040EDAA
	addu	r7,r7,r9
	subu	r7,r7,r6
	illegal
	mul	r4,r4,r10
	modu	r6,r7,r5
	sll	r7,r6,00000010
	ext	r8,r8,00000000,00000010
	or	r8,r7,r8
	bgeuc	r8,r4,0040EDD2
	addu	r8,r8,r9
	bltuc	r8,r9,0040EDD2
	bgeuc	r8,r4,0040EDD2
	addu	r8,r8,r9
	subu	r8,r8,r4
	move	r5,r0
	srlv	r4,r8,r11

l0040EDDC:
	jrc	ra
0040EDDE                                           20 A9                .
0040EDE0 06 00 81 D3 C7 20 98 49 69 21 3F 5B 60 A9 3E 00 ..... .Ii!?[`.>.
0040EDF0 25 21 D0 29 89 80 C0 F3 49 81 50 C0 45 21 98 31 %!.)....I.P.E!.1
0040EE00 CC 3C 45 21 D8 39 A8 80 50 C0 E7 80 10 C0 DC 53 .<E!.9..P......S
0040EE10 C7 88 0C C0 E1 3C 27 A9 06 C0 C7 88 02 C0 E1 3C .....<'........<
0040EE20 7F B3 47 21 98 29 47 21 D8 31 8D 3C 89 1B 40 00 ..G!.)G!.1.<..@.
0040EE30 20 00 69 21 10 48 62 21 D0 11 64 21 10 40 45 20  .i!.Hb!..d!.@E 
0040EE40 50 18 65 21 10 28 44 20 50 10 89 80 50 C0 A2 20 P.e!.(D P...P.. 
0040EE50 90 12 A9 80 C0 F3 83 20 98 31 CD 3C 83 20 D8 39 ....... .1.<. .9
0040EE60 42 81 50 C0 E7 80 10 C0 47 21 90 3A C7 88 0C C0 B.P.....G!.:....
0040EE70 E1 3C 27 A9 06 C0 C7 88 02 C0 E1 3C 7D B3 86 20 .<'........<}.. 
0040EE80 98 51 4D 3C 86 20 D8 39 A2 80 C0 F3 E7 80 10 C0 .QM<. .9........
0040EE90 FC 52 45 89 0C C0 A1 3C 25 A9 06 C0 45 89 02 C0 .RE....<%...E...
0040EEA0 A1 3C 45 21 D0 29 4D 1B                         .<E!.)M.       

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
	illegal
	mul	r11,r10,r13
	srl	r2,r5,00000010
	sll	r3,r3,00000010
	sllv	r6,r6,r12
	or	r3,r3,r2
	sllv	r4,r4,r12
	move	r2,r13
	bgeuc	r3,r11,0040EF36
	addu	r3,r3,r8
	addiu	r2,r2,FFFFFFFF
	bltuc	r3,r8,0040EF36
	bgeuc	r3,r11,0040EF36
	addiu	r2,r13,FFFFFFFE
	addu	r3,r3,r8
	subu	r3,r3,r11
	modu	r11,r3,r7
	illegal
	mul	r7,r10,r13
	andi	r5,r5,0000FFFF
	sll	r11,r11,00000010
	or	r10,r11,r5
	move	r5,r13
	bgeuc	r10,r7,0040EF68
	addu	r10,r10,r8
	addiu	r5,r5,FFFFFFFF
	bltuc	r10,r8,0040EF68
	bgeuc	r10,r7,0040EF68
	addiu	r5,r13,FFFFFFFE
	addu	r10,r10,r8
	sll	r11,r2,00000010
	subu	r10,r10,r7
	or	r11,r11,r5
	mul	r2,r11,r6
	illegal
	move	r7,r2
	move	r5,r11
	bltuc	r10,r11,0040EF8C
	bnec	r10,r11,0040EF9C
	bgeuc	r4,r2,0040EF9C
	subu	r7,r2,r6
	subu	r11,r11,r8
	sltu	r2,r2,r7
	subu	r5,r11,r2
	subu	r7,r4,r7
	subu	r10,r10,r5
	sltu	r4,r4,r7
	subu	r10,r10,r4
	srlv	r4,r7,r12
	sllv	r8,r10,r9
	srlv	r5,r10,r12
	or	r4,r8,r4
	bc	0040EDDC
	sigrie	00000000

;; __adddf3: 0040EFC0
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
	addiupc	r2,00001F6C
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
	illegal
	mul	r6,r13,r14
	modu	r5,r8,r12
	sll	r8,r5,00000010
	srl	r5,r9,00000010
	move	r3,r14
	or	r5,r5,r8
	bgeuc	r5,r6,0040F7B6
	addu	r5,r5,r4
	addiu	r3,r3,FFFFFFFF
	bltuc	r5,r4,0040F7B6
	bgeuc	r5,r6,0040F7B6
	addiu	r3,r14,FFFFFFFE
	addu	r5,r5,r4
	subu	r5,r5,r6
	illegal
	mul	r8,r13,r14
	modu	r6,r5,r12
	ext	r9,r9,00000000,00000010
	sll	r6,r6,00000010
	or	r9,r9,r6
	move	r6,r14
	bgeuc	r9,r8,0040F7E8
	addu	r9,r9,r4
	addiu	r6,r6,FFFFFFFF
	bltuc	r9,r4,0040F7E8
	bgeuc	r9,r8,0040F7E8
	addiu	r6,r14,FFFFFFFE
	addu	r9,r9,r4
	sll	r3,r3,00000010
	subu	r9,r9,r8
	or	r3,r3,r6
	illegal
	mul	r5,r3,r7
	bltuc	r9,r6,0040F80A
	move	r8,r3
	bnec	r9,r6,0040F84A
	bgeuc	r2,r5,0040F84A
	addu	r2,r2,r7
	addiu	r8,r3,FFFFFFFF
	sltu	r14,r2,r7
	addu	r14,r14,r4
	addu	r9,r9,r14
	bltuc	r4,r9,0040F82A
	bnec	r4,r9,0040F84A
	bltuc	r2,r7,0040F84A
	bltuc	r9,r6,0040F836
	bnec	r6,r9,0040F84A
	bgeuc	r2,r5,0040F84A
	addu	r2,r2,r7
	addiu	r8,r3,FFFFFFFE
	sltu	r3,r2,r7
	addu	r3,r3,r4
	addu	r9,r9,r3
	subu	r5,r2,r5
	subu	r9,r9,r6
	sltu	r2,r2,r5
	subu	r6,r9,r2
	addiu	r9,r0,FFFFFFFF
	beqc	r4,r6,0040F91A
	illegal
	mul	r3,r13,r14
	modu	r2,r6,r12
	sll	r6,r2,00000010
	srl	r2,r5,00000010
	move	r9,r14
	or	r2,r2,r6
	bgeuc	r2,r3,0040F896
	addu	r2,r2,r4
	addiu	r9,r9,FFFFFFFF
	bltuc	r2,r4,0040F896
	bgeuc	r2,r3,0040F896
	addiu	r9,r14,FFFFFFFE
	addu	r2,r2,r4
	subu	r2,r2,r3
	illegal
	mul	r3,r13,r14
	modu	r6,r2,r12
	andi	r5,r5,0000FFFF
	sll	r6,r6,00000010
	or	r6,r6,r5
	move	r2,r14
	bgeuc	r6,r3,0040F8C6
	addu	r6,r6,r4
	addiu	r2,r2,FFFFFFFF
	bltuc	r6,r4,0040F8C6
	bgeuc	r6,r3,0040F8C6
	addiu	r2,r14,FFFFFFFE
	addu	r6,r6,r4
	sll	r5,r9,00000010
	subu	r6,r6,r3
	or	r5,r5,r2
	illegal
	mul	r3,r7,r5
	bltuc	r6,r2,0040F8E8
	move	r9,r5
	bnec	r6,r2,0040F916
	beqc	r0,r3,0040F91A
	addu	r6,r4,r6
	addiu	r9,r5,FFFFFFFF
	bltuc	r6,r4,0040F90E
	bltuc	r6,r2,0040F8FE
	bnec	r2,r6,0040F916
	bgeuc	r7,r3,0040F912
	addiu	r9,r5,FFFFFFFE
	sll	r5,r7,00000001
	sltu	r7,r5,r7
	addu	r4,r7,r4
	move	r7,r5
	addu	r6,r6,r4
	bnec	r6,r2,0040F916
	beqc	r3,r7,0040F91A
	ori	r9,r9,00000001
	addiu	r6,r11,000003FF
	bgec	r0,r6,0040F9C8
	andi	r7,r9,00000007
	beqzc	r7,0040F93C
	andi	r7,r9,0000000F
	beqic	r7,00000004,0040F93C
	addiu	r7,r9,00000004
	sltu	r9,r7,r9
	addu	r8,r8,r9
	move	r9,r7
	bbeqzc	r8,00000018,0040F948
	ins	r8,r0,00000008,00000001
	addiu	r6,r11,00000400
	addiu	r7,r0,000007FE
	bltc	r7,r6,0040FA5A
	sll	r7,r8,0000001D
	srl	r9,r9,00000003
	or	r9,r7,r9
	srl	r8,r8,00000003
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
0040F97C                                     88 20 90 22             . ."
0040F980 E8 E0 00 00 78 52 E0 60 FF FF 0F 00 80 20 10 2E ....xR.`..... ..
0040F990 87 20 10 46 FF D3 45 11 87 20 10 4E E8 E0 00 00 . .F..E.. .N....
0040F9A0 E8 20 90 42 C0 00 FF 07 B7 1B 4D 11 04 11 27 11 . .B......M...'.
0040F9B0 83 11 80 C9 A4 10 80 C9 E3 1F 90 C9 5D 0F 00 11 ............]...
0040F9C0 20 11 5C 18 45 11 EB 1B 81 D2 5B B3 A9 C8 EF CF  .\.E.....[.....
0040F9D0 A9 C8 50 00 20 D3 A9 20 50 20 ED B2 C8 20 10 38 ..P. .. P ... .8
0040F9E0 C9 20 10 48 CC 53 20 21 90 4B 27 21 90 4A A8 20 . .H.S !.K'!.J. 
0040F9F0 50 40 E9 80 07 20 94 9B E9 80 0F 20 E0 C8 0C 20 P@... ..... ... 
0040FA00 E9 00 04 00 27 21 90 4B 01 3C 27 11 14 C9 50 B8 ....'!.K.<'...P.
0040FA10 E8 80 1D C0 29 81 43 C0 27 21 90 4A 08 81 43 C0 ....).C.'!.J..C.
0040FA20 C0 10 3D 1B E0 80 1F 80 7F B3 C0 10 E8 20 50 38 ..=.......... P8
0040FA30 A1 C8 08 00 A0 20 D0 29 A8 20 10 30 26 21 90 4A ..... .). .0&!.J
0040FA40 00 11 20 21 90 4B 27 21 90 4A A7 1B 00 61 FF FF .. !.K'!.J...a..
0040FA50 0F 00 20 81 01 80 40 11 43 1B 00 11 20 11 45 1B .. ...@.C... .E.
0040FA60 00 11 20 11 01 D3 F9 1A 00 00 00 00 00 00 00 00 .. .............

;; __nedf2: 0040FA70
;;   Called from:
;;     0040C57E (in fn0040C664)
;;     0040C57E (in fn0040C57C)
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
