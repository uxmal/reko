;;; Segment .text (00009840)

;; hexagon_nn_skel_invoke: 00009840
hexagon_nn_skel_invoke proc
	{ allocframe(00000078); memd(r29+496) = r17:r16; r2 = add(PC,000218D0) }
	{ memd(r29+80) = r25:r24; memd(r29+88) = r23:r22; r17:r16 = combine(r0,r1) }
	{ memd(r29+96) = r21:r20; memd(r29+104) = r19:r18; r18 = 00000014; r3 = extractu(r17,00000005,0000001D) }
	{ memd(r29+72) = r27:r26 }
	{ r20 = memw(r2-336); r21 = memw(r2-352) }
	{ r19 = memw(r2-320); p0 = cmp.gtu(r3,00000014) }
	{ r2 = memw(r2-304); if (p0) jump:nt 0000A644 }

l00009890:
	{ r4 = add(PC,00000DD8) }
	{ r3 = memw(r14+r3<<#2) }
	{ r3 = add(r3,r4) }
	{ jumpr r3 }
000098A4             6D 1E 74 3E 5F 1E 66 3E 58 41 DD 91     m.t>_.f>XA..
000098B0 3A C1 DD 91 46 4D 00 58 1E C0 1E 90 6D 1E 74 3E :...FM.X....m.t>
000098C0 5F 1E 66 3E 58 41 DD 91 3A C1 DD 91 62 4A 00 58 _.f>XA..:...bJ.X
000098D0 1E C0 1E 90 60 48 00 00 02 DE 49 6A FE 7F FF 0F ....`H....Ij....
000098E0 02 F7 82 97 83 44 11 8D 04 48 51 8D E6 C1 11 76 .....D...HQ....v
000098F0 05 C8 31 8D 05 C6 05 F3 23 C4 05 EF 08 40 83 10 ..1.....#....@..
00009900 23 C0 90 43 A0 46 00 58 12 C0 07 7A 04 41 00 78 #..C.F.X...z.A.x
00009910 08 E3 42 21 F8 7F FF 59 E0 C0 83 75 03 C0 90 91 ..B!...Y...u....
00009920 34 15 35 12 31 14 36 13 C5 48 46 8E C1 48 44 8E 4.5.1.6..HF..HD.
00009930 C7 40 23 91 E8 C0 23 91 C7 48 48 8E 30 10 36 11 .@#...#..HH.0.6.
00009940 C0 48 46 8E C1 D0 47 8E C0 50 45 8E 00 C0 A2 50 .HF...G..PE....P
00009950 7C C6 00 58 5E 48 00 00 02 DE 49 6A FE 7F FF 0F |..X^H....Ij....
00009960 93 F7 82 97 82 44 11 8D 04 48 51 8D E5 C1 11 76 .....D...HQ....v
00009970 03 C8 31 8D 03 C5 03 F3 22 C4 03 EF C4 42 F2 10 ..1....."....B..
00009980 22 C0 90 43 83 41 00 78 08 E2 42 21 BC 7F FF 59 "..C.A.x..B!...Y
00009990 60 C1 82 75 64 50 04 C4 85 03 83 00 31 16 36 15 `..udP......1.6.
000099A0 37 17 32 14 C2 48 46 8E C1 C8 47 8E C2 D0 41 8E 7.2..HF...G...A.
000099B0 08 45 C2 14 21 C0 84 47 46 46 00 58 D2 C1 00 7A .E..!..GFF.X...z
000099C0 36 1B 35 1A C5 48 46 8E 37 19 39 18 D1 C8 47 8E 6.5..HF.7.9...G.
000099D0 D1 D0 45 8E F2 41 B9 14 00 C0 00 7F EE 49 B2 14 ..E..A.......I..
000099E0 75 C0 23 47 3A 10 81 02 3C 12 3E 11 10 40 84 91 u.#G:...<.>..@..
000099F0 1A C1 02 20 16 C0 C2 10 31 40 00 00 48 C8 01 15 ... ....1@..H...
00009A00 03 41 30 F3 31 40 00 00 38 C2 82 20 28 40 03 5C .A0.1@..8.. (@.\
00009A10 03 C0 90 74 08 C0 02 60 22 80 21 9B 08 C2 A3 AB ...t...`".!.....
00009A20 D2 48 56 8E D4 48 55 8E 02 C0 71 70 D2 D0 54 8E .HV..HU...qp..T.
00009A30 00 52 10 F5 00 C0 B3 50 08 C6 00 58 82 44 11 8D .R.....P...X.D..
00009A40 03 48 51 8D E5 C1 11 76 04 C8 31 8D 04 C5 04 F3 .HQ....v..1.....
00009A50 22 C3 04 EF 58 42 F2 10 22 C0 90 43 E0 42 82 75 "...XB.."..C.B.u
00009A60 03 43 00 78 54 E2 32 21 82 03 84 00 22 43 02 8C .C.xT.2!...."C..
00009A70 43 42 24 91 67 C2 24 91 C3 48 47 8E 26 42 24 91 CB$.g.$..HG.&B$.
00009A80 05 C2 24 91 C5 C8 46 8E C5 50 43 8E E3 C2 24 91 ..$...F..PC...$.
00009A90 94 42 B5 14 C2 42 24 47 86 C2 24 47 A7 42 24 91 .B...B$G..$G.B$.
00009AA0 00 C0 24 91 C2 48 43 8E A8 C0 90 91 C6 48 47 8E ..$..HC......HG.
00009AB0 21 C3 08 8C C6 D0 42 8E 80 41 B6 14 83 41 24 47 !.....B..A...A$G
00009AC0 28 C0 24 47 42 13 47 12 C7 48 42 8E A9 41 24 91 (.$GB.G..HB..A$.
00009AD0 D2 C1 24 91 C0 48 48 8E EC 41 24 91 02 C1 24 91 ..$..HH..A$...$.
00009AE0 C3 48 49 8E D2 48 4C 8E 2D 41 24 91 4E C1 24 91 .HI..HL.-A$.N.$.
00009AF0 C0 50 47 8E C2 48 4D 8E 6F 41 24 91 81 C0 24 91 .PG..HM.oA$...$.
00009B00 CE 48 4F 8E A8 40 24 91 D1 C0 24 91 C3 50 52 8E .HO..@$...$..PR.
00009B10 C1 48 48 8E FC 40 24 91 8A C0 90 91 D1 48 5C 8E .HH..@$......H\.
00009B20 C2 50 4E 8E 44 C0 90 91 C1 50 51 8E 01 46 9D A1 .PN.D....PQ..F..
00009B30 00 CA 9D A1 06 CA 00 5A 88 C5 00 58 82 44 11 8D .......Z...X.D..
00009B40 03 48 51 8D E5 C1 11 76 04 C8 31 8D 04 C5 04 F3 .HQ....v..1.....
00009B50 22 C3 04 EF D8 41 E2 10 22 C0 90 43 60 43 82 75 "....A.."..C`C.u
00009B60 83 43 00 78 D4 E2 22 21 08 40 90 91 62 C0 90 91 .C.x.."!.@..b...
00009B70 23 43 28 91 47 C3 28 91 06 43 28 91 64 C3 28 91 #C(.G.(..C(.d.(.
00009B80 C6 48 43 8E C7 C8 44 8E C6 50 47 8E C7 C2 28 91 .HC...D..PG...(.
00009B90 14 42 B6 14 00 40 28 47 22 C0 28 47 4C 40 28 91 .B...@(G".(GL@(.
00009BA0 63 C0 28 91 C0 48 42 8E CC 48 43 8E E9 42 28 91 c.(..HB..HC..B(.
00009BB0 81 C0 28 91 AD 40 28 91 CE C0 28 91 C7 48 49 8E ..(..@(...(..HI.
00009BC0 C1 48 4D 8E EF 40 28 91 3C C1 28 91 C0 50 4C 8E .HM..@(.<.(..PL.
00009BD0 CE 48 4F 8E 02 41 28 91 4A C1 28 91 C1 50 4E 8E .HO..A(.J.(..PN.
00009BE0 6B 41 28 91 83 C1 28 91 C2 48 5C 8E CA 48 4B 8E kA(...(..H\..HK.
00009BF0 B1 41 28 91 D2 C1 28 91 C3 48 51 8E F3 41 28 91 .A(...(..HQ..A(.
00009C00 04 C2 28 91 D2 48 53 8E 34 42 28 91 55 C2 28 91 ..(..HS.4B(.U.(.
00009C10 C2 50 4A 8E C4 48 54 8E 76 C2 28 91 D5 48 56 8E .PJ..HT.v.(..HV.
00009C20 C3 50 52 8E 85 42 28 91 A8 C2 28 91 C5 48 48 8E .PR..B(...(..HH.
00009C30 C4 50 55 8E 5C 40 90 91 01 C6 9D A1 C5 50 47 8E .PU.\@.......PG.
00009C40 00 DC 9D A1 A8 C9 00 5A 00 C5 00 58 53 48 00 00 .......Z...XSH..
00009C50 02 C2 49 6A FF 7F FF 0F 14 F8 82 97 82 44 11 8D ..Ij.........D..
00009C60 03 48 51 8D E5 C1 11 76 04 C8 31 8D 04 C5 04 F3 .HQ....v..1.....
00009C70 22 C3 04 EF 48 40 E2 10 22 C0 90 43 83 40 00 78 "...H@.."..C.@.x
00009C80 08 E2 42 21 40 7E FF 59 60 C0 82 75 02 C0 90 91 ..B!@~.Y`..u....
00009C90 23 C0 22 91 24 12 20 10 C0 48 43 8E 62 C0 22 91 #.".$. ..HC.b.".
00009CA0 C4 C8 42 8E C0 50 44 8E 00 C0 B4 50 CE C4 00 58 ..B..PD....P...X
00009CB0 83 44 11 8D 02 48 51 8D E5 C1 11 76 04 C8 31 8D .D...HQ....v..1.
00009CC0 04 C5 04 F3 23 C2 04 EF 1E 43 E3 10 23 C0 90 43 ....#....C..#..C
00009CD0 60 43 83 75 84 43 00 78 08 E3 42 21 14 FE FF 59 `C.u.C.x..B!...Y
00009CE0 6B 50 02 C4 84 C2 00 78 23 40 8B 91 08 E4 42 22 kP.....x#@....B"
00009CF0 0A 7E FF 59 60 C2 83 75 83 03 89 00 A4 42 31 91 .~.Y`..u.....B1.
00009D00 C7 C2 31 91 86 42 31 91 E5 C2 31 91 C6 48 44 8E ..1..B1...1..HD.
00009D10 C7 C8 45 8E C6 D0 47 8E 50 43 A6 14 64 43 31 47 ..E...G.PC..dC1G
00009D20 43 C3 31 47 42 43 02 8C 8C 46 1D B0 0E 43 31 91 C.1GBC...F...C1.
00009D30 25 C3 31 91 C3 48 44 8E 09 47 1D B0 CD 40 31 91 %.1..HD..G...@1.
00009D40 67 C0 8B 91 CE C8 45 8E CE D0 43 8E 36 48 DF 5C g.....E...C.6H.\
00009D50 00 47 4E F2 04 42 31 47 45 C0 90 47 12 46 1D B0 .GN..B1GE..G.F..
00009D60 93 11 90 10 10 41 02 E2 C0 48 43 8E 3C 42 31 91 .....A...HC.<B1.
00009D70 83 C1 31 91 91 1D 92 17 CD 48 42 8E C3 48 41 8E ..1......HB..HA.
00009D80 4F 42 31 91 6A C2 31 91 C4 48 5C 8E CF 48 4A 8E OB1.j.1..H\..HJ.
00009D90 48 40 31 91 33 C1 31 91 92 18 97 13 5C 41 31 91 H@1.3.1.....\A1.
00009DA0 B5 C0 31 91 C8 48 47 8E 87 47 1D B0 91 14 9C 1B ..1..HG..G......
00009DB0 C2 48 53 8E C4 50 4F 8E CA C1 31 91 DC 48 54 8E .HS..PO...1..HT.
00009DC0 C0 50 48 8E F6 41 31 91 11 C0 8B 91 C1 48 55 8E .PH..A1......HU.
00009DD0 CA 48 56 8E 0B 40 90 91 07 D2 9D A1 C2 50 5C 8E .HV..@.......P\.
00009DE0 0E 48 1D B0 06 4E 9D A1 05 CB 9D A1 C1 50 4D 8E .H...N.......PM.
00009DF0 C3 50 4A 8E 04 4C 9D A1 01 CE 9D A1 03 49 9D A1 .PJ..L.......I..
00009E00 02 C7 9D A1 4C 49 00 5A 00 C6 9D A1 12 40 60 70 ....LI.Z.....@`p
00009E10 20 40 00 00 70 E0 42 24 12 40 00 78 02 42 9D 91  @..p.B$.@.x.B..
00009E20 00 C2 B1 A1 23 48 02 8C 24 D8 02 8C 22 50 02 8C ....#H..$..."P..
00009E30 94 13 93 B1 02 C2 11 A1 E7 C1 9D 91 22 58 07 8C ............"X..
00009E40 23 48 07 8C 04 C7 11 A1 24 50 07 8C 93 15 92 B7 #H......$P......
00009E50 06 C4 11 A1 C7 C1 9D 91 22 58 07 8C 23 48 07 8C ........"X..#H..
00009E60 08 C7 11 A1 24 50 07 8C 93 19 92 BB 0A C4 11 A1 ....$P..........
00009E70 A7 C1 9D 91 26 58 07 8C 22 48 07 8C 0C C7 11 A1 ....&X.."H......
00009E80 23 50 07 8C 96 1F 92 BD 0E C3 11 A1 86 C1 9D 91 #P..............
00009E90 23 58 06 8C 27 50 06 8C 10 C6 11 A1 22 48 06 8C #X..'P......"H..
00009EA0 13 43 11 A1 12 C7 11 A1 CE 43 00 58 11 C2 11 A1 .C.......C.X....
00009EB0 49 48 00 00 02 D0 49 6A D2 7E FF 59 FF 7F FF 0F IH....Ij.~.Y....
00009EC0 94 F8 82 97 82 44 11 8D 03 48 51 8D E5 C1 11 76 .....D...HQ....v
00009ED0 04 C8 31 8D 04 C5 04 F3 22 C3 04 EF E8 7F FF 0F ..1.....".......
00009EE0 50 42 C2 10 22 C0 90 43 E0 40 82 75 04 41 00 78 PB.."..C.@.u.A.x
00009EF0 08 E2 42 21 08 FD FF 59 64 50 03 C4 85 C0 00 78 ..B!...YdP.....x
00009F00 22 40 84 91 08 E5 42 22 FE 7C FF 59 60 C0 82 75 "@....B".|.Y`..u
00009F10 43 43 03 8C 46 03 85 00 26 44 06 8C 52 14 57 16 CC..F...&D..R.W.
00009F20 E9 40 25 91 A8 C0 25 91 C7 48 49 8E C2 C8 48 8E .@%...%..HI...H.
00009F30 C2 D0 47 8E EA 7F FF 0F 08 46 82 14 11 C0 84 47 ..G......F.....G
00009F40 57 13 56 12 10 41 03 E2 03 48 1D B0 55 11 50 10 W.V..A...H..U.P.
00009F50 C6 48 47 8E C0 48 45 8E 01 C0 90 91 C0 50 46 8E .HG..HE......PF.
00009F60 10 C9 00 5A 12 40 60 70 1B 40 00 00 40 E0 42 24 ...Z.@`p.@..@.B$
00009F70 12 40 00 78 02 42 9D 91 00 C2 B1 A1 23 58 02 8C .@.x.B......#X..
00009F80 27 D0 02 8C 22 48 02 8C 97 12 93 B3 5C 43 00 58 '..."H......\C.X
00009F90 01 C2 11 A1 83 44 11 8D 02 48 51 8D E5 C1 11 76 .....D...HQ....v
00009FA0 04 C8 31 8D 04 C5 04 F3 23 C2 04 EF E5 7F FF 0F ..1.....#.......
00009FB0 30 41 C3 10 23 C0 90 43 60 40 83 75 84 40 00 78 0A..#..C`@.u.@.x
00009FC0 08 E3 42 21 A0 FC FF 59 63 50 02 C4 04 C1 00 78 ..B!...YcP.....x
00009FD0 22 40 83 91 08 E4 42 22 96 7C FF 59 E0 C0 82 75 "@....B".|.Y...u
00009FE0 82 47 1D B0 01 48 1D B0 38 00 84 00 25 C0 24 91 .G...H..8...%.$.
00009FF0 43 12 40 10 C0 48 45 8E 64 C0 24 91 C3 C8 44 8E C.@..HE.d.$...D.
0000A000 C0 50 43 8E 00 C9 00 5A 12 40 60 70 18 40 00 00 .PC....Z.@`p.@..
0000A010 78 E0 42 24 12 40 00 78 02 42 9D 91 00 C2 B0 A1 x.B$.@.x.B......
0000A020 23 48 02 8C 27 D0 02 8C 22 58 02 8C 87 12 83 B1 #H..'..."X......
0000A030 03 C2 10 A1 E7 C1 9D 91 23 58 07 8C 26 50 07 8C ........#X..&P..
0000A040 04 C7 10 A1 22 48 07 8C 86 16 83 B7 FC 42 00 58 ...."H.......B.X
0000A050 05 C2 10 A1 42 48 00 00 02 DE 49 6A FF 7F FF 0F ....BH....Ij....
0000A060 15 F9 82 97 83 44 11 8D 02 48 51 8D E5 C1 11 76 .....D...HQ....v
0000A070 04 C8 31 8D 04 C5 04 F3 23 42 04 EF 62 50 02 C4 ..1.....#B..bP..
0000A080 84 C0 00 78 E2 7F FF 0F 00 C0 C3 10 23 40 82 91 ...x........#@..
0000A090 08 E4 42 22 38 7C FF 59 60 C0 83 75 00 40 B5 50 ..B"8|.Y`..u.@.P
0000A0A0 28 00 00 4D 12 40 60 70 16 40 00 00 40 E0 42 24 (..M.@`p.@..@.B$
0000A0B0 12 40 00 78 02 42 9D 91 00 C2 B0 A1 23 58 02 8C .@.x.B......#X..
0000A0C0 27 D0 02 8C 22 48 02 8C 87 12 83 B3 BC 42 00 58 '..."H.......B.X
0000A0D0 01 C2 10 A1 83 44 11 8D 02 48 51 8D E5 C1 11 76 .....D...HQ....v
0000A0E0 04 C8 31 8D 04 C5 04 F3 23 C2 04 EF E0 7F FF 0F ..1.....#.......
0000A0F0 30 42 C3 10 24 C0 90 43 83 40 00 78 08 E4 42 21 0B..$..C.@.x..B!
0000A100 02 7C FF 59 60 C0 84 75 62 D0 02 C4 24 40 82 91 .|.Y`..ub...$@..
0000A110 FA E3 32 22 85 03 84 00 26 C0 24 91 47 13 43 10 ..2"....&.$.G.C.
0000A120 C3 48 46 8E 44 C0 24 91 C4 C8 47 8E C3 D0 44 8E .HF.D.$...G...D.
0000A130 E2 7F FF 0F 10 C5 83 14 14 40 00 00 18 40 03 10 .........@...@..
0000A140 D2 41 00 78 40 C0 90 47 03 C0 03 F3 E3 FF 03 97 .A.x@..G........
0000A150 00 40 03 DD 08 C8 00 5C D6 7B FF 59 00 C0 03 75 .@.....\.{.Y...u
0000A160 78 48 00 5A 28 00 01 4D 9E FF FF 59 83 44 11 8D xH.Z(..M...Y.D..
0000A170 02 48 51 8D E5 C1 11 76 04 C8 31 8D 04 C5 04 F3 .HQ....v..1.....
0000A180 23 C2 04 EF DE 7F FF 0F 00 41 C3 10 23 C0 90 43 #........A..#..C
0000A190 04 41 00 78 DE 7F FF 0F 08 E3 02 21 63 50 02 C4 .A.x.......!cP..
0000A1A0 04 C0 90 91 45 17 31 01 46 15 47 16 C7 48 45 8E ....E.1.F.G..HE.
0000A1B0 82 C0 24 91 C2 C8 46 8E C2 D0 47 8E DF 7F FF 0F ..$...F...G.....
0000A1C0 78 41 82 14 58 41 DD 47 76 C1 DD 47 46 13 45 12 xA..XA.Gv..GF.E.
0000A1D0 C5 48 46 8E 44 11 40 10 C0 48 44 8E 74 3E 31 00 .HF.D.@..HD.t>1.
0000A1E0 66 1E 6D 3E C0 50 45 8E 5A 48 00 58 3A 41 DD 91 f.m>.PE.ZH.X:A..
0000A1F0 1E C0 1E 90 6D 1E 74 3E 5F 1E 66 3E 58 41 DD 91 ....m.t>_.f>XA..
0000A200 3A C1 DD 91 72 48 00 58 1E C0 1E 90 82 44 11 8D :...rH.X.....D..
0000A210 03 48 51 8D E5 C1 11 76 04 C8 31 8D 04 C5 04 F3 .HQ....v..1.....
0000A220 22 C3 04 EF DB 7F FF 0F 40 41 C2 10 22 C0 90 43 ".......@A.."..C
0000A230 83 40 00 78 E9 7F FF 0F 28 E2 02 21 83 03 82 00 .@.x....(..!....
0000A240 24 C0 22 91 25 13 21 10 C1 48 44 8E 42 C0 22 91 $.".%.!..HD.B.".
0000A250 C2 C8 45 8E C1 D0 42 8E DD 7F FF 0F 40 43 81 14 ..E...B.....@C..
0000A260 58 41 DD 47 76 C1 DD 47 74 3E 80 02 66 1E 6D 3E XA.Gv..Gt>..f.m>
0000A270 EA 47 00 58 3A 41 DD 91 1E C0 1E 90 82 44 11 8D .G.X:A.......D..
0000A280 18 48 51 8D E4 C1 11 76 03 C8 31 8D 03 C4 03 F3 .HQ....v..1.....
0000A290 22 D8 03 EF D9 7F FF 0F 60 43 C2 10 19 60 10 74 ".......`C...`.t
0000A2A0 22 C0 90 43 83 41 00 78 DB 7F FF 0F 50 E2 02 21 "..C.A.x....P..!
0000A2B0 24 49 49 02 A3 44 00 78 82 03 0A 5A 22 42 02 8C $II..D.x...Z"B..
0000A2C0 5B C0 99 9B 22 43 42 ED A4 40 3B 91 93 C0 3B 91 [..."CB..@;...;.
0000A2D0 D3 48 44 8E C5 40 3B 91 E7 C0 3B 91 C5 C8 47 8E .HD..@;...;...G.
0000A2E0 D3 D0 45 8E DB 7F FF 0F 28 C2 8B 14 42 45 13 8C ..E.....(...BE..
0000A2F0 AC C0 D2 26 15 40 00 78 16 40 00 78 14 40 3B 91 ...&.@.x.@.x.@;.
0000A300 5A C0 3B 91 00 40 02 75 00 61 82 74 77 40 3B 91 Z.;..@.u.a.tw@;.
0000A310 23 C0 3B 91 10 40 00 5C 0B C3 9D A1 F2 F8 FF 5B #.;..@.\.......[
0000A320 16 40 60 70 92 C0 12 24 02 40 00 78 E1 30 ED 70 .@`p...$.@.x.0.p
0000A330 10 C2 95 AB 86 40 CB 10 AF 28 8F 70 00 41 13 60 .....@...(.p.A.`
0000A340 02 40 75 70 04 40 70 70 03 C0 99 91 17 40 79 70 .@up.@pp.....@yp
0000A350 36 13 35 10 47 40 23 91 28 C0 23 91 25 10 26 B3 6.5.G@#.(.#.%.&.
0000A360 02 47 02 A1 01 C8 02 A1 31 14 35 17 A7 40 23 91 .G......1.5..@#.
0000A370 C8 C0 23 91 21 14 25 B7 06 48 02 A1 05 C7 02 A1 ..#.!.%..H......
0000A380 36 18 31 1B 27 41 23 91 48 C1 23 91 26 18 21 BB 6.1.'A#.H.#.&.!.
0000A390 0A 48 02 A1 09 C7 02 A1 36 1C 35 1F A7 41 23 91 .H......6.5..A#.
0000A3A0 C8 C1 23 91 26 1C 25 BF 0E 48 02 A1 0D C7 02 A1 ..#.&.%..H......
0000A3B0 41 42 23 91 66 C2 23 91 27 42 23 91 08 C2 23 91 AB#.f.#.'B#...#.
0000A3C0 C1 48 46 8E 15 47 02 A1 16 C1 02 A1 17 C6 02 A1 .HF..G..........
0000A3D0 C8 48 47 8E 14 C8 02 A1 A9 C0 84 91 C8 50 41 8E .HG..........PA.
0000A3E0 06 E9 C2 20 24 CE 1A 16 19 C2 04 B0 04 40 99 91 ... $........@..
0000A3F0 04 D2 A2 A1 04 40 77 70 E1 42 23 91 85 C2 23 91 .....@wp.B#...#.
0000A400 A6 42 23 91 C7 C2 23 91 18 45 02 A1 19 C6 02 A1 .B#...#..E......
0000A410 1A 47 02 A1 1B C1 02 A1 00 43 23 91 26 C3 23 91 .G.......C#.&.#.
0000A420 83 43 03 B0 47 43 23 91 68 C3 23 91 1C 40 02 A1 .C..GC#.h.#..@..
0000A430 1D C6 02 A1 1E C7 02 A1 02 84 02 B0 1F C8 02 A1 ................
0000A440 42 41 3B 91 63 C1 3B 91 C2 48 43 8E 19 41 3B 91 BA;.c.;..HC..A;.
0000A450 24 C1 3B 91 7B 50 18 C4 D9 48 44 8E A5 C0 97 91 $.;.{P...HD.....
0000A460 D9 50 42 8E 27 C2 05 8C 0A 48 20 5C 00 47 59 F2 .PB.'....H \.GY.
0000A470 23 C0 9B 47 DC 40 00 58 D2 C1 00 7A AA 6A AA 0A #..G.@.X...z.j..
0000A480 62 C5 00 78 22 C2 43 ED 22 C4 02 8C F4 68 DF 5C b..x".C."....h.\
0000A490 00 42 59 F2 F2 FF 8F 7E 42 45 19 8C CA C0 C2 26 .BY....~BE.....&
0000A4A0 18 40 00 78 00 40 02 75 00 61 82 74 48 D6 9D 42 .@.x.@.u.a.tH..B
0000A4B0 04 40 20 5C 0E C0 00 58 24 F8 FF 5B B8 40 00 10 .@ \...X$..[.@..
0000A4C0 18 C0 60 70 90 08 01 F0 10 D6 98 AB 00 40 59 75 ..`p.........@Yu
0000A4D0 83 E2 18 74 00 40 40 89 32 40 20 5C 08 D4 BD A1 ...t.@@.2@ \....
0000A4E0 08 41 19 60 1F 40 00 00 90 6D 31 DE 82 C0 97 91 .A.`.@...m1.....
0000A4F0 10 C1 11 E2 25 10 24 12 27 11 26 13 C5 48 47 8E ....%.$.'.&..HG.
0000A500 28 40 90 91 00 C5 03 A1 36 13 37 B1 C4 48 46 8E (@......6.7..HF.
0000A510 02 C4 03 A1 C5 50 44 8E 82 40 02 B0 0A E8 C4 20 .....PD..@..... 
0000A520 86 4E 0A 16 36 C1 9D 91 44 40 90 9B FF F2 A3 A7 .N..6...D@......
0000A530 03 84 03 B0 00 C0 00 7F 04 40 79 70 A7 1C B2 3C .........@yp...<
0000A540 D4 48 42 8E 02 D3 18 F5 DA C8 47 8E D4 D0 5A 8E .HB.......G...Z.
0000A550 4E 45 00 5A 00 D4 15 F5 12 40 60 70 08 E0 02 24 NE.Z.....@`p...$
0000A560 66 40 00 58 36 C1 9D 91 00 C1 9D 91 00 40 40 85 f@.X6........@@.
0000A570 60 58 20 5C 36 41 9D 47 02 C0 9B 43 08 C0 19 60 `X \6A.G...C...`
0000A580 03 40 38 91 64 C0 38 91 45 40 38 91 26 C0 38 91 .@8.d.8.E@8.&.8.
0000A590 23 10 24 B3 26 11 25 B2 E3 40 38 91 87 C0 38 91 #.$.&.%..@8...8.
0000A5A0 A5 40 38 91 C6 C0 38 91 27 14 23 B7 25 15 26 B6 .@8...8.'.#.%.&.
0000A5B0 67 41 38 91 04 C1 38 91 25 41 38 91 46 C1 38 91 gA8...8.%A8.F.8.
0000A5C0 24 18 27 BB 25 19 26 BA 83 41 38 91 A4 C1 38 91 $.'.%.&..A8...8.
0000A5D0 C5 41 38 91 E6 C1 38 91 24 1D 23 BC 26 1F 25 BE .A8...8.$.#.&.%.
0000A5E0 07 43 38 91 24 C3 38 91 45 43 38 91 66 C3 38 91 .C8.$.8.EC8.f.8.
0000A5F0 10 47 02 A1 11 C4 02 A1 12 45 02 A1 13 C6 02 A1 .G.......E......
0000A600 83 43 38 91 A4 C3 38 91 18 44 18 B0 C5 43 38 91 .C8...8..D...C8.
0000A610 E6 C3 38 91 14 43 02 A1 15 C4 02 A1 16 C5 02 A1 ..8..C..........
0000A620 02 83 02 B0 17 C6 02 A1 9E 3C 0A 48 0C C0 0E 10 .........<.H....
0000A630 70 77 FF 5B E8 00 E0 50 80 39 8E 30 FA 60 FF 5C pw.[...P.9.0.`.\
0000A640 00 C0 00 7F                                     ....            

l0000A644:
	{ r0 = r18 }
	{ r19:r18 = memd(r29+104); r17:r16 = memd(r29+112) }
	{ r23:r22 = memd(r29+88); r21:r20 = memd(r29+96) }
	{ r27:r26 = memd(r29+72); r25:r24 = memd(r29+80) }
	{ dealloc_return }
0000A65C                                     62 77 FF 5B             bw.[
0000A660 00 C0 70 70 DE F9 FF 59 3C F2 FF FF 54 F2 FF FF ..pp...Y<...T...
0000A670 7C F2 FF FF FC F2 FF FF EC F2 FF FF D4 F3 FF FF |...............
0000A680 D4 F4 FF FF F4 F5 FF FF 48 F6 FF FF E4 F5 FF FF ........H.......
0000A690 48 F8 FF FF 5C F8 FF FF 6C F2 FF FF 2C F9 FF FF H...\...l...,...
0000A6A0 FC F9 FF FF 6C FA FF FF 04 FB FF FF 8C FB FF FF ....l...........
0000A6B0 EC F9 FF FF A4 FB FF FF 14 FC FF FF 00 00 00 00 ................

;; allocate_graph_storage: 0000A6C0
;;   Called from:
;;     0000C65C (in do_prepare)
allocate_graph_storage proc
	{ allocframe(00000020); r3 = 60000000; r2 = 00000080 }
	{ memd(r29+16) = r19:r18; memd(r29+24) = r17:r16; r16 = r0; r4 = 00000080 }
	{ r1 = add(r16,00000028) }
	{ memw(r16+48) = r4; memd(r29+8) = r21:r20; call prefree }
	{ r17 = memw(r16) }

l0000A6EC:
	{ if (p0.new) r0 = add(r16,00000000); if (p0.new) jump:nt 0000A7C0; p0 = cmp.eq(r9,00000000) }

l0000A6F4:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000A7B8; r2 = memw(r17+20) }

l0000A700:
	{ r3 = memw(r17+8) }

l0000A704:
	{ r3 = memw(r29+r20) }
	{ if (cmp.eq(r4.new,00000000)) jump:nt 0000A7AC; r4 = memw(r3+20) }

l0000A714:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 0000A7B0 }

l0000A71C:
	{ r1:r0 = combine(r17,r16); r2 = r18 }
	{ r5:r4 = combine(00000000,00000000); r19 = r0; r2 = add(PC,000209EC) }
	{ r1:r0 = combine(00000000,r16) }
	{ r6 = memw(r2-208); r3:r2 = combine(00000000,0000000D) }
	{ r6 = memw(r6+52) }
	{ memw(r29+4) = FFFFFF80; r6 = memw(r6+8) }
	{ memw(r29) = 00000000; callr r6 }
	{ if (!p0.new) r2 = memw(r17+8); if (!p0.new) jump:nt 0000A794; p0 = cmp.eq(r0,00000000) }

l0000A75C:
	{ r2 = r16; r1 = 000000F9; r3 = add(PC,0001AC02) }
	{ call errlog_function }
	{ r2 = r16; r1 = 00000110; r3 = add(PC,0001ABE1) }
	{ call errlog_function }
	{ r1 = 0000014C; jump 0000A8E0; r3 = add(PC,0001AA48) }

l0000A794:
	{ r2 = memw(r13+r20) }
	{ memb(r0+2) = r2.new; r2 = add(r2,0000001C) }
	{ memw(r19+36) = r0; memw(r0+36) = r7 }
	{ r2 = memw(r17+20) }

l0000A7AC:
	{ if (cmp.gtu(r2,r18.new)) jump:t 0000A700; r18 = add(r18,00000001); r20 = add(r20,00000004) }

l0000A7B0:
	{ if (cmp.gtu(r2,r18.new)) jump:t 0000A704; r18 = add(r18,00000001) }

l0000A7B8:
	{ r17 = memw(r17+36); jump 0000A6EC }

l0000A7BC:
	{ r17 = memw(r17+36) }

l0000A7C0:
	{ call allocate_and_free }
	{ p0 = cmp.eq(r0,00000000); r1 = 0000014E; r3 = add(PC,0001AA16) }
	{ if (p0) r2 = memw(r16); if (!p0) jump:nt 0000A8E0 }

l0000A7DC:
	{ if (!p0.new) r3 = memw(r2+24); if (p0.new) r0 = add(r16,00000000); if (p0.new) jump:nt 0000A800; p0 = cmp.eq(r2,00000000) }

l0000A7E8:
	{ p0 = cmp.eq(r3,0000000D) }
	{ if (p0) r3 = memw(r2+8) }
	{ if (p0) r3 = memw(r3) }
	{ if (p0) memw(r3+16) = 00000000 }
	{ r2 = memw(r2+36); jump 0000A7DC }

l0000A800:
	{ call check_allocations }
	{ p0 = cmp.eq(r0,00000000); r1 = 00000155; r3 = add(PC,0001A9E1) }
	{ if (p0) r0 = add(r16,00000000); if (!p0) jump:nt 0000A8E0 }

l0000A81C:
	{ call check_allocations }
	{ r1 = 000000CA; if (p0.new) jump:nt 0000A848; p0 = cmp.eq(r0,00000000) }

l0000A828:
	{ r3 = add(PC,0001A9EB) }

l0000A830:
	{ r2 = r16; call errlog_function }

l0000A834:
	{ r2 = r16 }
	{ r1 = 00000156; jump 0000A8E0; r3 = add(PC,0001A9BD) }

l0000A848:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000A860; r2 = memw(r16+44); r1 = 000000CB }

l0000A858:
	{ jump 0000A834; r3 = add(PC,00000005) }

l0000A860:
	{ r17 = memw(r16+48) }
	{ r0 = r17; call fn00009500 }
	{ memw(r16+44) = r0; if (p0.new) r1 = 000000CD; if (!p0.new) jump:nt 0000A884; p0 = cmp.eq(r0,00000000) }

l0000A878:
	{ jump 0000A830; r3 = add(PC,0001A9BA) }

l0000A884:
	{ memw(r29+4) = r0; r3:r2 = combine(00000002,r16); r4 = add(PC,0001A9BF) }
	{ memw(r29) = r17; r1 = 000000D1 }
	{ call logmsg_function }
	{ r3 = memw(r16+8); r2 = memw(r16+12); r0 = r16 }
	{ r4 = add(r2,0000007F) }
	{ memb(r3+1) = r4.new; r4 = and(r4,FFFFFF80) }
	{ r4 = memw(r3+4); r2 = sub(r7,r2) }
	{ memb(r3+2) = r2.new; r2 = add(r2,r4); call allocate_and_free }
	{ p0 = cmp.eq(r0,00000000); r1 = 00000158; r3 = add(PC,00000037) }
	{ if (p0) jump:nt 0000A8F4 }

l0000A8E0:
	{ r17 = -00000001; r2 = r16; call errlog_function }

l0000A8E8:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r17 }
	{ dealloc_return; r21:r20 = memd(r29+8) }

l0000A8F4:
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000A8E8; r0 = memw(r16); r17 = 00000000; r18 = r16 }

l0000A908:
	{ r18 = add(r0,00000024) }

l0000A90C:
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000A8E8; r0 = memw(r18) }

l0000A918:
	{ if (!cmp.eq(r2.new,0000000D)) jump:nt 0000A90C }

l0000A920:
	{ memb(r18) = r7.new; r7 = memw(r0+36) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ jump 0000A90C }
0000A938                         00 40 00 7F 00 C0 00 7F         .@......

;; errlog_function: 0000A940
;;   Called from:
;;     0000A76C (in allocate_graph_storage)
;;     0000A780 (in allocate_graph_storage)
;;     0000A830 (in allocate_graph_storage)
;;     0000A8E0 (in allocate_graph_storage)
;;     0000A9E8 (in allocate_and_free)
;;     0000AB48 (in check_allocations)
;;     0000AC40 (in prefree)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0001A875) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; allocate_and_free: 0000A964
;;   Called from:
;;     0000A7C0 (in allocate_graph_storage)
;;     0000A8C0 (in allocate_graph_storage)
allocate_and_free proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r17:r16 = combine(r0,00000000) }
	{ memd(r29+24) = r19:r18; r19 = memw(r17) }
	{ memd(r29+16) = r21:r20 }
	{ memd(r29+8) = r23:r22; if (!p0.new) r18 = add(r17,00000028); if (p0.new) jump:nt 0000AAD4; p0 = cmp.eq(r11,00000000) }

l0000A980:
	{ if (cmp.eq(r2.new,0000000D)) jump:t 0000A9C8; r2 = memw(r19+24); r1:r0 = combine(r18,r17) }

l0000A984:
	{ if (cmp.eq(r2.new,0000000D)) jump:t 0000A9CC; r2 = memw(r19+24) }

l0000A990:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000AAD0; r2 = memw(r19+20) }

l0000A998:
	{ r3 = memw(r19+8) }

l0000A99C:
	{ r21 = memw(r31+r20<<#2) }
	{ if (cmp.eq(r3.new,00000000)) jump:nt 0000AAC4; r3 = memw(r21+20) }

l0000A9AC:
	{ if (!cmp.eq(r4.new,00000000)) jump:t 0000AAC8 }

l0000A9B4:
	{ if (cmp.eq(r4.new,00000000)) jump:nt 0000AAE8; r4 = memw(r18) }

l0000A9C0:
	{ r2 = and(r2,FFFFFF80); jump 0000AA30 }

l0000A9C8:
	{ r2 = memw(r19+8) }

l0000A9CC:
	{ r3 = memw(r2) }
	{ r3 = memw(r3+20); r2 = memw(r3+16) }
	{ call prefree }
	{ if (!p0.new) r1 = 00000123; if (p0.new) jump:nt 0000A9F4; p0 = cmp.eq(r0,00000000) }

l0000A9E0:
	{ r3 = add(PC,0001A8F7) }

l0000A9E8:
	{ r16 = -00000001; r2 = r17; call errlog_function }
	{ jump 0000AAD4 }

l0000A9F4:
	{ r2 = memw(r19+8); r1 = 00000127; r4 = add(PC,0001A8F1) }
	{ r5 = memw(r2); r3:r2 = combine(00000003,r17) }
	{ r6 = memw(r5+20) }
	{ memb(r29+1) = r5.new; r5 = memw(r5+16) }
	{ memw(r29) = r6 }
	{ jump 0000AACC }

l0000AA24:
	{ r4 = memw(r0); r3 = r0 }
	{ nop; if (p0.new) jump:nt 0000AAE4; p0 = cmp.eq(r4,00000000) }

l0000AA30:
	{ r0 = r4 }
	{ if (cmp.gtu(r2,r4.new)) jump:t 0000AA24; r4 = memw(r0+8) }

l0000AA40:
	{ if (!p0.new) r3 = sub(r4,r2); if (!p0.new) jump:nt 0000AA58; p0 = cmp.eq(r4,r2) }

l0000AA48:
	{ memb(r3) = r2.new; r2 = memw(r0); call fn00009510 }

l0000AA58:
	{ memb(r0+1) = r2.new; r2 = add(r22,r2) }
	{ if (!cmp.gtu(r2,r3.new)) jump:t 0000AA8C; r3 = memw(r17+48) }

l0000AA70:
	{ memw(r17+48) = r2; r3:r2 = combine(00000003,r17); r4 = add(PC,0000002A) }
	{ memb(r29) = r5.new; r5 = memw(r0+4); r1 = 00000066; call logmsg_function }

l0000AA8C:
	{ memw(r21+16) = r22; if (!p0.new) r1 = 00000135; if (!p0.new) jump:nt 0000AAA8; p0 = cmp.eq(r14,00000000) }

l0000AA90:
	{ memw(r21+16) = r22; if (!p0.new) r1 = 00000135 }

l0000AA98:
	{ r1 = 00000131; jump 0000A9E8; r3 = add(PC,0001A85D) }

l0000AAA8:
	{ memw(r29+4) = r22; r5 = memw(r21+20); r4 = add(PC,0001A85A) }
	{ memw(r29) = r5; r3:r2 = combine(00000003,r17) }
	{ call logmsg_function }
	{ r2 = memw(r19+20) }

l0000AAC4:
	{ if (cmp.gtu(r2,r20.new)) jump:t 0000A998; r20 = add(r20,00000001) }

l0000AAC8:
	{ if (cmp.gtu(r2,r20.new)) jump:t 0000A99C }

l0000AACC:
	{ if (!cmp.eq(r19.new,00000000)) jump:t 0000A980; r19 = memw(r19+36) }

l0000AAD0:
	{ if (!cmp.eq(r19.new,00000000)) jump:t 0000A984 }

l0000AAD4:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32); r0 = r16 }

l0000AAD8:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

l0000AAE4:
	{ memw(r21+16) = 00000000; jump 0000AA98 }

l0000AAE8:
	{ memw(r21+16) = 00000000 }

;; check_allocations: 0000AAEC
;;   Called from:
;;     0000A800 (in allocate_graph_storage)
;;     0000A81C (in allocate_graph_storage)
;;     0000AAE8 (in allocate_and_free)
check_allocations proc
	{ allocframe(+00000018); r1 = 000000B8; r4 = add(PC,0001A7C6) }
	{ memd(r29+16) = r17:r16; r16 = r0 }
	{ r5 = memw(r16+40); r3:r2 = combine(00000003,r16) }
	{ r7 = memw(r5+4); r6 = memw(r5+8) }
	{ memw(r29+4) = r7; memw(r29+8) = r6 }
	{ memw(r29) = r5; call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000AB38; r2 = memw(r16+40) }

l0000AB24:
	{ if (!cmp.eq(r4.new,00000000)) jump:t 0000AB24; r4 = memw(r4); r3 = add(r3,FFFFFFFF) }

l0000AB34:
	{ if (p0.new) r2 = memw(r2+8) }

l0000AB38:
	{ r2 = r16; r1 = 000000BE; r3 = add(PC,0001A739) }
	{ call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+16); r0 = FFFFFFFF }
0000AB54             18 48 00 5C 00 40 00 06 00 40 02 75     .H.\.@...@.u
0000AB60 61 F8 00 7E 9C 46 00 00 03 4C 49 6A 21 58 00 78 a..~.F...LIj!X.x
0000AB70 00 C2 9D A1 02 40 70 70 00 40 00 06 80 C0 5D 3C .....@pp.@....]<
0000AB80 E4 FF FF 59 9C 46 00 00 84 46 49 6A 42 E0 30 73 ...Y.F...FIjB.0s
0000AB90 26 40 00 5A 85 41 90 91 00 D2 BD A1 00 C0 00 78 &@.Z.A.........x
0000ABA0 40 1F 14 3E                                     @..>            

;; allocator_teardown: 0000ABA4
;;   Called from:
;;     0000B7FC (in do_teardown)
allocator_teardown proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0 }
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000ABC4; r0 = memw(r16+40) }

l0000ABB4:
	{ r17 = memw(r0); call fn00009510 }

l0000ABB8:
	{ r17 = memw(r0) }

l0000ABBC:
	{ p0 = cmp.eq(r17,00000000); r0 = r17 }
	{ if (!p0) jump:nt 0000ABB4 }

l0000ABC4:
	{ memw(r16+40) = 00000000; r0 = memw(r16+12) }
	{ nop; if (!p0.new) jump:nt 0000ABD4; p0 = cmp.eq(r0,00000000) }

l0000ABD0:
	{ dealloc_return; r17:r16 = memd(r29) }

l0000ABD4:
	{ deallocframe; r17:r16 = memd(r29); jump fn00009510 }

;; logmsg_function: 0000ABDC
;;   Called from:
;;     0000A89C (in allocate_graph_storage)
;;     0000AA7C (in allocate_and_free)
;;     0000AABC (in allocate_and_free)
;;     0000AB10 (in check_allocations)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000ABFC; r5 = memw(r2+16) }

l0000ABEC:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000000D) }
	{ memw(r29+4) = r6; call logv }

l0000ABFC:
	{ dealloc_return }

;; prefree: 0000AC00
;;   Called from:
;;     0000A6DC (in allocate_graph_storage)
;;     0000A9D4 (in allocate_and_free)
prefree proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r2,r1); r3 = add(r3,0000007F) }
	{ memd(r29) = r19:r18; r4 = memw(r16); r19 = and(r3,FFFFFF80); r18 = r0 }

l0000AC18:
	{ r2 = r4; if (!p0.new) jump:nt 0000AC50; p0 = cmp.eq(r4,00000000) }

l0000AC20:
	{ r0 = 0000000C; call fn00009500 }
	{ if (p0.new) r17 = FFFFFFFF; if (!p0.new) jump:nt 0000ACB4; p0 = cmp.eq(r0,00000000) }

l0000AC30:
	{ r2 = r18; r1 = 000000A0; r3 = add(PC,0001A70D) }
	{ call errlog_function }
	{ jump 0000ACC4 }

l0000AC48:
	{ r4 = memw(r2); jump 0000AC18; r8 = r2 }

l0000AC50:
	{ r4 = memw(r2+4); r3 = memw(r2+8) }
	{ if (cmp.gtu(r17,r5.new)) jump:t 0000AC48; r5 = add(r3,r4) }

l0000AC60:
	{ memb(r2+2) = r3.new; r0 = memw(r2); r17 = 00000000; r3 = add(r3,r19) }
	{ if (!p0.new) r5 = memw(r0+4) }
	{ if (!cmp.eq(r4.new,r5)) jump:t 0000ACC4; r4 = add(r4,r3) }

l0000AC84:
	{ memb(r2+2) = r3.new; r3 = add(r4,r3) }
	{ memw(r2) = r3; call fn00009510 }
	{ jump 0000ACC4 }
0000AC9C                                     05 51 13 F3             .Q..
0000ACA0 C2 E4 72 20 11 40 00 78 29 01 B3 78 0C 40 00 58 ..r .@.x)..x.@.X
0000ACB0 02 C3 82 A1                                     ....            

l0000ACB4:
	{ memw(r0+8) = r19; memw(r0+4) = r17; r17 = 00000000 }
	{ r2 = memw(r16) }
	{ memw(r16) = r0; memw(r0) = r2 }

l0000ACC4:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r17 }
	{ dealloc_return }

;; do_execute: 0000ACD0
;;   Called from:
;;     0000B060 (in hexagon_nn_execute_new)
;;     0000B120 (in hexagon_nn_execute)
do_execute proc
	{ allocframe(00000020); memd(r29+496) = r17:r16; r16 = r0 }
	{ memd(r29+8) = r21:r20; memd(r29+16) = r19:r18 }
	{ memw(r16+28) = r2; memd(r29) = r23:r22 }
	{ memw(r16+24) = r3; memw(r16+20) = r1 }
	{ memw(r16+32) = r4; call nn_os_hvx_power_on }
	{ r0 = r16; call nn_os_vector_workers_acquire }
	{ call fn00009530 }
	{ if (cmp.eq(r17.new,00000000)) jump:nt 0000AD58; r17 = memw(r16); r19:r18 = combine(r1,r0) }

l0000AD08:
	{ if (p0.new) r20 = 00000000; if (p0.new) jump:nt 0000AD58; p0 = cmp.eq(r9,00000000) }

l0000AD0C:
	{ if (p0.new) r20 = 00000000 }

l0000AD10:
	{ r0 = r16; call nn_os_get_perfcount }
	{ r2 = memw(r17); r1:r0 = combine(r16,r17); r23:r22 = combine(r1,r0) }
	{ r2 = memw(r2) }
	{ callr r2 }
	{ if (!cmp.eq(r20.new,00000000)) jump:nt 0000AD58; r20 = r0 }

l0000AD38:
	{ r0 = r16 }
	{ memw(r17+44) += 00000001; r3:r2 = memd(r17+48); r1:r0 = sub(r1:r0,r23:r22) }
	{ r1:r0 = add(r3:r2,r1:r0) }
	{ memd(r17+48) = r1:r0; r17 = memw(r17+36); jump 0000AD08 }

l0000AD58:
	{ call fn00009530 }
	{ r0 = r16; r3:r2 = sub(r1:r0,r19:r18) }
	{ memd(r16+80) = r3:r2; call nn_os_vector_workers_release }
	{ r0 = r16; call nn_os_hvx_power_off }
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r20 }
	{ r23:r22 = memd(r29); r21:r20 = memd(r29+8) }
	{ dealloc_return }
0000AD84             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; hexagon_nn_init: 0000AD90
hexagon_nn_init proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r1:r0 = combine(00000058,00000001) }
	{ memd(r29) = r19:r18; call fn00009540 }
	{ r17:r16 = combine(r0,00000000) }
	{ if (!p0.new) memb(r17-28) = 01; if (p0.new) jump:nt 0000AE1C; p0 = cmp.eq(r9,00000000) }

l0000ADAC:
	{ r1 = 02000000; r0 = 00000080 }
	{ call fn00009550 }
	{ memb(r17+1) = r18.new; r18 = r0 }
	{ if (!p0.new) memw(r17+8) = 02000000 }
	{ r0 = r17; call fn00009510 }
	{ jump 0000AE1C }
0000ADDC                                     00 40 02 00             .@..
0000ADE0 00 E0 00 7C AE F3 FF 5B 0C 40 40 10 00 60 91 74 ...|...[.@@..`.t
0000ADF0 0E C0 91 A1 8E 73 FF 5B 00 C0 72 70 EA FF FF 59 .....s.[..rp...Y
0000AE00 FF 7F 01 00 BF 47 51 3C 00 C8 51 3C A2 CE 00 5A .....GQ<..Q<...Z
0000AE10 10 C0 60 70 00 40 10 75 10 E0 11 74             ..`p.@.u...t    

l0000AE1C:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
	{ dealloc_return }
0000AE28                         00 40 00 7F 00 C0 00 7F         .@......

;; hexagon_nn_getlog: 0000AE30
hexagon_nn_getlog proc
	{ allocframe(00000010); r4 = 0000000E; r3 = add(PC,0001A541) }
	{ memd(r29) = r19:r18; memd(r29+8) = r17:r16; r18 = r2; r17:r16 = combine(r1,r0) }
	{ r1:r0 = combine(r3,r17); call fn00009560; r2 = min(r4,r18) }
	{ r0 = FFFFFFFF; if (p0.new) jump:nt 0000AE90; p0 = cmp.eq(r8,00000000); r2 = add(r17,add(r18,0000003F)) }

l0000AE60:
	{ memb(r2) = 00 }
	{ r19 = memw(r16+56) }
	{ r0 = r19; call fn00009570 }
	{ r2 = r18; r3 = r0; r1:r0 = combine(r19,r17) }
	{ call fn00009560; r2 = min(r2,r3) }
	{ memw(r16+64) = 00000000; r2 = memw(r16+56); r0 = 00000000 }
	{ memb(r2) = 00 }

l0000AE90:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
	{ dealloc_return }

;; hexagon_nn_snpprint: 0000AE98
hexagon_nn_snpprint proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r3 = add(PC,0001A4D9) }
	{ r17:r16 = combine(r1,r2) }
	{ memd(r29) = r19:r18; r18 = r0; r1:r0 = combine(r3,r17); call fn00009580 }
	{ if (!p0.new) r2 = add(r16,00000000); r0 = FFFFFFFF; if (p0.new) jump:nt 0000AECC; p0 = cmp.eq(r10,00000000) }

l0000AEC0:
	{ r1:r0 = combine(r17,r18); call do_snpprint }
	{ r0 = 00000000 }

l0000AECC:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
	{ dealloc_return }

;; hexagon_nn_set_debug_level: 0000AED4
hexagon_nn_set_debug_level proc
	{ p0 = cmp.eq(r0,00000000); r3 = 00000000; if (p0.new) r2 = FFFFFFFF }
	{ if (!p0) r2 = add(r3,00000000); r3 = max(r1,r3) }
	{ jumpr r31; r0 = r2 }

;; hexagon_nn_prepare: 0000AEF0
hexagon_nn_prepare proc
	{ allocframe(+00000008); if (!p0.new) jump:nt 0000AF14; p0 = cmp.eq(r0,00000000) }

l0000AEF8:
	{ r1 = 00000087; r2 = 00000000; r3 = add(PC,0001A49F) }
	{ memw(r29) = 00000000; call errlog_function }
	{ dealloc_return; r0 = -00000001 }

l0000AF14:
	{ deallocframe; jump do_prepare }

;; errlog_function: 0000AF1C
;;   Called from:
;;     0000AF08 (in hexagon_nn_prepare)
;;     0000AF78 (in hexagon_nn_append_node)
;;     0000AFCC (in hexagon_nn_append_const_node)
;;     0000B034 (in hexagon_nn_execute_new)
;;     0000B100 (in hexagon_nn_execute)
;;     0000B168 (in hexagon_nn_teardown)
;;     0000B1BC (in hexagon_nn_get_perfinfo)
;;     0000B1E0 (in hexagon_nn_reset_perfinfo)
;;     0000B218 (in hexagon_nn_last_execution_cycles)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0001A463) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; hexagon_nn_append_node: 0000AF40
hexagon_nn_append_node proc
	{ allocframe(00000008); r6 = r4; if (!p0.new) jump:nt 0000AF60; p0 = cmp.eq(r0,00000000) }

l0000AF48:
	{ r1 = 00000098; r2 = 00000000; r3 = add(PC,0001A44F) }
	{ memw(r29) = 00000000; jump 0000AF78 }

l0000AF60:
	{ if (cmp.eq(r4.new,00000001)) jump:t 0000AF80; r4 = memb(r0+36) }

l0000AF6C:
	{ r2 = r0; r1 = 0000009B; r3 = add(PC,00000002) }

l0000AF78:
	{ call errlog_function }
	{ dealloc_return; r0 = -00000001 }

l0000AF80:
	{ r7 = memd(r29+20); r4 = memd(r29+16) }
	{ memw(r29) = r6; memw(r29+4) = r4; r5:r4 = combine(r7,r5); call do_append_node }
	{ dealloc_return }

;; hexagon_nn_append_const_node: 0000AF94
hexagon_nn_append_const_node proc
	{ allocframe(+00000008); if (!p0.new) jump:nt 0000AFB4; p0 = cmp.eq(r0,00000000) }

l0000AF9C:
	{ r1 = 000000B4; r2 = 00000000; r3 = add(PC,0001A3FB) }
	{ memw(r29) = 00000000; jump 0000AFCC }

l0000AFB4:
	{ if (cmp.eq(r6.new,00000001)) jump:t 0000AFD4; r6 = memb(r0+36) }

l0000AFC0:
	{ r2 = r0; r1 = 000000B7; r3 = add(PC,0000002E) }

l0000AFCC:
	{ call errlog_function }
	{ dealloc_return; r0 = -00000001 }

l0000AFD4:
	{ memb(r29+1) = r7.new; r7 = memw(r29+20) }
	{ memw(r29) = r6; call do_append_const_node }
	{ dealloc_return }

;; hexagon_nn_execute_new: 0000AFEC
hexagon_nn_execute_new proc
	{ allocframe(00000020); memd(r29+496) = r17:r16 }
	{ memd(r29+16) = r19:r18; r19:r18 = combine(r3,r2); r17:r16 = combine(r0,r1) }
	{ memd(r29+8) = r21:r20; r20 = r4; if (!p0.new) jump:nt 0000B01C; p0 = cmp.eq(r9,00000000) }

l0000B004:
	{ r1 = 000000D5; r2 = 00000000; r3 = add(PC,0001A393) }
	{ memw(r29) = 00000000; jump 0000B034 }

l0000B01C:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0000B044; r2 = memb(r17+36) }

l0000B028:
	{ r2 = r17; r1 = 000000D8; r3 = add(PC,0000002B) }

l0000B034:
	{ call errlog_function }
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = FFFFFFFF }
	{ dealloc_return; r21:r20 = memd(r29+8) }

l0000B044:
	{ memw(r29) = r18; memw(r29+4) = r20; r2 = r17; call logmsg_function }
	{ r1:r0 = combine(r16,r17); r3:r2 = combine(r19,r18); r4 = r20 }
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }
	{ deallocframe; r21:r20 = memd(r29+8); jump do_execute }

;; logmsg_function: 0000B068
;;   Called from:
;;     0000B044 (in hexagon_nn_execute_new)
logmsg_function proc
	{ allocframe(00000008); r4 = 00000002 }
	{ if (cmp.gtu(r4,r3.new)) jump:t 0000B098; r3 = memw(r2+16) }

l0000B078:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000000B) }
	{ r6 = add(r29,00000010); r1 = 000000DA; r4 = add(PC,0001A362) }
	{ memw(r29+4) = r6; call logv }

l0000B098:
	{ dealloc_return }

;; hexagon_nn_execute: 0000B09C
hexagon_nn_execute proc
	{ allocframe(+00000060); memw(r29-52) = r4; p0 = cmp.eq(r0,00000000) }
	{ memw(r29+44) = r2; r6 = memd(r29+104) }
	{ memw(r29+48) = r3; r4 = memd(r29+124) }
	{ memw(r29+60) = r6; r3 = memw(r29+128) }
	{ memw(r29+24) = r4; memw(r29+40) = r1 }
	{ memw(r29+64) = r6; memw(r29+28) = r3 }
	{ memd(r29+80) = r19:r18; memd(r29+88) = r17:r16 }
	{ memw(r29+32) = r3; memd(r29+72) = r21:r20 }
	{ if (p0) memw(r29) = 00000000; memw(r29+56) = r5; if (!p0) jump:nt 0000B0E8 }

l0000B0D4:
	{ r1 = 000000F9; r2 = 00000000; r3 = add(PC,0001A2C3) }
	{ jump 0000B100 }

l0000B0E8:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0000B10C; r2 = memb(r0+36) }

l0000B0F4:
	{ r2 = r0; r1 = 000000FC; r3 = add(PC,0000001F) }

l0000B100:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0000B144 }

l0000B10C:
	{ r1 = add(r29,00000028); r2 = 00000001; r3 = add(r29,00000008); r4 = 00000001 }
	{ r17 = memd(r29+116); r16 = memd(r29+108) }
	{ r19 = memd(r29+120); r18 = memd(r29+112) }
	{ r20 = memw(r29+132); call do_execute }
	{ memb(r16) = r2.new; r2 = memw(r29+8) }
	{ memw(r18) = r5; r4 = memd(r29+16) }
	{ memw(r19) = r3; memw(r17) = r4 }
	{ memb(r20) = r2.new; r2 = memw(r29+32) }

l0000B144:
	{ r19:r18 = memd(r29+80); r17:r16 = memd(r29+88) }

l0000B148:
	{ dealloc_return; r21:r20 = memd(r29+72) }
0000B14C                                     00 C0 00 7F             ....

;; hexagon_nn_teardown: 0000B150
hexagon_nn_teardown proc
	{ allocframe(+00000008); if (!p0.new) jump:nt 0000B174; p0 = cmp.eq(r0,00000000) }

l0000B158:
	{ r1 = 0000010B; r2 = 00000000; r3 = add(PC,0001A23F) }
	{ memw(r29) = 00000000; call errlog_function }
	{ dealloc_return; r0 = -00000001 }

l0000B174:
	{ deallocframe; jump do_teardown }

;; hexagon_nn_get_perfinfo: 0000B17C
hexagon_nn_get_perfinfo proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r3; p0 = cmp.eq(r0,00000000) }
	{ if (p0) memw(r29) = 00000000; if (p0) jump:nt 0000B1AC }

l0000B190:
	{ call do_perfinfo_get }
	{ memb(r16) = r2.new; r17:r16 = memd(r29+8); r0 = FFFFFFFF; r2 = r0 }
	{ dealloc_return; r0 = -00000001 }

l0000B1AC:
	{ r1 = 00000117; r2 = 00000000; r3 = add(PC,0001A1EB) }
	{ call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

;; hexagon_nn_reset_perfinfo: 0000B1C8
hexagon_nn_reset_perfinfo proc
	{ allocframe(+00000008); if (!p0.new) jump:nt 0000B1EC; p0 = cmp.eq(r0,00000000) }

l0000B1D0:
	{ r1 = 00000124; r2 = 00000000; r3 = add(PC,0001A1C7) }
	{ memw(r29) = 00000000; call errlog_function }
	{ dealloc_return; r0 = -00000001 }

l0000B1EC:
	{ deallocframe; jump do_perfinfo_reset }

;; hexagon_nn_version: 0000B1F4
hexagon_nn_version proc
	{ r2 = r0; r0 = 00000000 }
	{ memw(r2) = 0000005A; jumpr r31 }

;; hexagon_nn_last_execution_cycles: 0000B200
hexagon_nn_last_execution_cycles proc
	{ allocframe(+00000008); if (!p0.new) jump:nt 0000B224; p0 = cmp.eq(r0,00000000) }

l0000B208:
	{ r1 = 00000134; r2 = 00000000; r3 = add(PC,0001A18F) }
	{ memw(r29) = 00000000; call errlog_function }
	{ dealloc_return; r0 = -00000001 }

l0000B224:
	{ r5:r4 = memd(r0+80); r0 = 00000000 }
	{ r7:r6 = lsr(r5:r4,00000020) }
	{ memw(r1) = r4; memw(r2) = r6 }
	{ dealloc_return }

;; hexagon_nn_GetHexagonBinaryVersion: 0000B238
hexagon_nn_GetHexagonBinaryVersion proc
	{ r2 = r0; r0 = 00000000 }
	{ memw(r2) = 0000005A; jumpr r31 }

;; hexagon_nn_PrintLog: 0000B244
hexagon_nn_PrintLog proc
	{ jumpr r31; r0 = 00000000 }
0000B248                         00 40 00 7F 00 C0 00 7F         .@......

;; hexagon_nn_op_name_to_id: 0000B250
hexagon_nn_op_name_to_id proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r2 = add(PC,0001FEC0) }
	{ memd(r29) = r19:r18; r18 = 00000000; r17:r16 = combine(r0,r1) }
	{ r19 = memw(r2-192) }
	{ r1 = memw(r19); r0 = r17; call fn00009590 }
	{ if (p0.new) jump:nt 0000B28C; p0 = cmp.eq(r0,00000000) }

l0000B278:
	{ r18 = r18; r19 = add(r19,00000004) }
	{ if (p0.new) r0 = FFFFFFFF; p0 = cmp.gt(r18,00000072); jump 0000B290; if (!p0.new) jump:nt 0000B26C }

l0000B28C:
	{ memw(r16) = r18; r0 = 00000000 }

l0000B290:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
	{ dealloc_return }

;; hexagon_nn_op_id_to_name: 0000B298
hexagon_nn_op_id_to_name proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; p0 = cmp.gtu(r0,00000072) }
	{ memd(r29) = r19:r18; r18 = r2; r17:r16 = combine(r1,FFFFFFFF); if (p0) jump:nt 0000B2DC }

l0000B2AC:
	{ r2 = add(PC,0001FE64) }
	{ r2 = memw(r2-192) }
	{ r19 = memw(r14+r0<<#2) }
	{ r0 = r19; call fn00009570 }
	{ if (cmp.gtu(r2.new,r18)) jump:t 0000B2DC; r2 = add(r0,00000001) }

l0000B2D4:
	{ r16 = 00000000; r1:r0 = combine(r19,r17) }

l0000B2DC:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
	{ dealloc_return }

;; hexagon_nn_disable_dcvs: 0000B2E8
hexagon_nn_disable_dcvs proc
	{ allocframe(00000028); r7 = 00000005; r0 = 00000000 }
	{ memb(r29) = r7; r2 = add(r29,00000000); r1 = add(r29,00000000) }
	{ memuh(r2+8) = 0100; call fn000095B0 }
	{ dealloc_return }
0000B30C                                     00 C0 00 7F             ....

;; hexagon_nn_set_powersave_level: 0000B310
hexagon_nn_set_powersave_level proc
	{ allocframe(00000028); r2 = 00000000; if (!p0.new) jump:nt 0000B33C; p0 = cmp.eq(r0,00000000) }

l0000B318:
	{ memb(r29) = r7.new; r0 = 00000000; r2 = add(r29,00000000); r7 = 00000005 }
	{ memuh(r2+8) = 0100; r1 = add(r29,00000000) }
	{ r2 = r0 }

l0000B33C:
	{ dealloc_return; r0 = r2 }

;; hexagon_nn_config: 0000B340
hexagon_nn_config proc
	{ r1 = 00000000; r0 = 01000000; r3:r2 = combine(FFFFFFFF,FFFFFFFF) }
	{ allocframe(+00000000); call fn000095C0 }
	{ dealloc_return; r0 = 00000000 }
0000B358                         00 00 00 00 00 00 00 00         ........

;; logv: 0000B360
;;   Called from:
;;     0000ABF4 (in logmsg_function)
;;     0000B090 (in logmsg_function)
;;     0000B6CC (in logmsg_function)
;;     0000BEFC (in logmsg_function)
;;     0000CAEC (in logmsg_function)
;;     0000CD20 (in errlog_function)
;;     0000EB9C (in logmsg_function)
;;     0000F1F8 (in logmsg_function)
;;     0000F450 (in logmsg_function)
;;     0000F570 (in logmsg_function)
;;     0000F704 (in logmsg_function)
;;     0000FAF0 (in logmsg_function)
;;     00010060 (in logmsg_function)
;;     000104B4 (in errlog_function)
;;     000108E4 (in logmsg_function)
;;     000108FC (in errlog_function)
;;     00010D24 (in errlog_function)
;;     00010F5C (in logmsg_function)
;;     000110C0 (in logmsg_function)
;;     000115AC (in errlog_function)
;;     00011D70 (in logmsg_function)
;;     00012574 (in logmsg_function)
;;     00012720 (in logmsg_function)
;;     00012D64 (in logmsg_function)
;;     00013608 (in logmsg_function)
;;     00013974 (in logmsg_function)
;;     000147D8 (in logmsg_function)
;;     00014B10 (in logmsg_function)
;;     00014CCC (in logmsg_function)
;;     000153F0 (in logmsg_function)
;;     00015D64 (in logmsg_function)
;;     00016568 (in errlog_function)
;;     00016568 (in fn00016564)
;;     00016CB4 (in logmsg_function)
;;     00016E5C (in logmsg_function)
;;     00016E88 (in errlog_function)
;;     00017014 (in logmsg_function)
;;     000173A4 (in logmsg_function)
;;     000174C0 (in logmsg_function)
;;     00017A18 (in logmsg_function)
;;     00017FD4 (in logmsg_function)
;;     0001897C (in logmsg_function)
;;     00019D00 (in logmsg_function)
;;     0001AD20 (in logmsg_function)
;;     0001AF90 (in logmsg_function)
;;     0001B294 (in logmsg_function)
;;     0001B84C (in logmsg_function)
;;     0001BB20 (in logmsg_function)
;;     0001C070 (in logmsg_function)
;;     0001C364 (in logmsg_function)
;;     0001C5F0 (in logmsg_function)
;;     0001CA68 (in logmsg_function)
;;     0001D03C (in logmsg_function)
;;     0001D4E8 (in logmsg_function)
;;     0001D85C (in logmsg_function)
;;     0001DA44 (in logmsg_function)
;;     0001DE54 (in logmsg_function)
;;     0001E17C (in logmsg_function)
;;     0001E32C (in logmsg_function)
;;     0001E990 (in logmsg_function)
;;     0001F000 (in logmsg_function)
;;     0001F454 (in errlog_function)
;;     0001F880 (in errlog_function)
;;     0001F9B4 (in logmsg_function)
;;     0001FE04 (in errlog_function)
;;     0001FF34 (in logmsg_function)
;;     000200EC (in logmsg_function)
;;     000204C0 (in logmsg_function)
;;     000206F4 (in logmsg_function)
;;     00020E10 (in logmsg_function)
;;     00021234 (in errlog_function)
;;     00021640 (in logmsg_function)
;;     00021838 (in logmsg_function)
;;     00021C84 (in logmsg_function)
;;     00021C9C (in errlog_function)
logv proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r16 = r2 }
	{ memd(r29+8) = r23:r22; r22 = memw(r16+64); r2 = add(PC,0001A0A2) }
	{ r7 = memw(r16+28); r17 = memw(r16+24) }
	{ memw(r29+4) = r1; memd(r29+16) = r21:r20; r20 = r5; r3 = add(r17,r22) }
	{ r21 = sub(r7,r22) }
	{ memw(r29) = r0; memd(r29+24) = r19:r18; r1:r0 = combine(r21,r3); r19 = r4 }
	{ call fn000095D0 }
	{ if (!cmp.gtu(r21,r18.new)) jump:t 0000B3E8; r18 = r0; r3:r2 = combine(r20,r19) }

l0000B3AC:
	{ r1:r0 = combine(r21,r17) }
	{ call fn000095E0; r0 += add(r18,r22) }
	{ if (!cmp.gtu(r21,r19.new)) jump:t 0000B3E8; r19 = r0; r3 = add(r18,r22) }

l0000B3C8:
	{ r20 = sub(r21,r19); r2 = add(PC,0000000D) }
	{ r17 += add(r19,r3) }
	{ r1:r0 = combine(r20,r17); call fn000095D0 }
	{  }
	{ r0 += add(r19,r18) }
	{ memw(r16+64) += r0 }

l0000B3E8:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }
0000B3F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; alloc_node: 0000B400
;;   Called from:
;;     000137EC (in hexagon_nn_const_ctor)
alloc_node proc
	{ allocframe(00000010); memd(r29+488) = r19:r18; r0 = 00000038; r18 = r0 }
	{ memd(r29+8) = r17:r16; r17:r16 = combine(r1,r2); call fn00009500 }
	{ if (p0.new) memw(r0+28) = r18; if (p0.new) jump:nt 0000B444; p0 = cmp.eq(r0,00000000) }

l0000B420:
	{ r2 = add(PC,0001FCF0) }
	{ r2 = memw(r2-208) }
	{ memb(r0+32) = r16; r2 = memw(r7+r17<<#2) }
	{ memw(r0+24) = r17; memw(r0) = r2 }
	{ memw(r0+52) = 00000000; memw(r0+44) = 00000000 }
	{ memw(r0+40) = 00000000; memw(r0+48) = 00000000 }

l0000B444:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
	{ dealloc_return }

;; node_alloc_common: 0000B44C
;;   Called from:
;;     000141F8 (in conv2d_ctor)
;;     00014FD8 (in matmul_ctor)
;;     00016E08 (in nop_ctor)
;;     00017464 (in prefree_ctor)
;;     0001D3C4 (in variable_ctor)
node_alloc_common proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r0 = 00000038; r16 = r0 }
	{ memd(r29+32) = r21:r20; memd(r29+40) = r19:r18 }
	{ memd(r29+24) = r23:r22; r22 = r1; r19:r18 = combine(r4,r5); r21:r20 = combine(r2,r3) }
	{ call fn00009500 }
	{ if (cmp.eq(r17.new,00000000)) jump:nt 0000B5E0; r17 = r0; r1 = 000000A0 }

l0000B47C:
	{ r2 = add(PC,00000018) }
	{ memw(r17+28) = r22; p0 = cmp.eq(r19,00000000) }
	{ r2 = memw(r2-208) }
	{ r2 = memw(r15+r21<<#2) }
	{ memw(r17+24) = r21; memw(r17) = r2 }
	{ r21 = memw(r29+68) }
	{ memw(r17+44) = FFFFFF80; memb(r17+32) = r20 }
	{ memw(r17+16) = r19; memw(r17+48) = 00000000 }
	{ memw(r17+52) = 00000000; memw(r17+40) = 00000000 }
	{ if (!p0) jump:nt 0000B4B8 }

l0000B4B0:
	{ memw(r17+12) = 00000000; memw(r17+4) = 00000000; jump 0000B538 }

l0000B4B8:
	{ call fn00009500; r0 = asl(r19,00000003) }
	{ memb(r17+3) = r20.new; r20 = r0 }
	{ r1 = 00000057 }
	{ r3 = add(PC,0001A01A) }
	{ r2 = r16; call errlog_function }
	{ r1 = 000000A4; jump 0000B5EC; r3 = add(PC,00019F68) }
0000B4F0 40 42 13 8C 08 F0 FF 5B 80 40 00 10 41 6B 00 7E @B.....[.@..Ak.~
0000B500 01 C0 91 A1 10 40 13 60 02 C2 9D 91 03 40 82 91 .....@.`.....@..
0000B510 0C E0 42 24 80 46 00 00 83 46 49 6A E2 7F FF 59 ..B$.F...FIj...Y
0000B520 21 CC 00 78 23 C0 82 91 47 40 82 9B 01 C3 94 A1 !..x#...G@......
0000B530 00 80 00 7F 10 C7 94 AB                         ........        

l0000B538:
	{ memw(r17+20) = r18; r1:r0 = combine(00000000,00000000); p0 = cmp.eq(r18,00000000) }
	{ memd(r29+8) = r1:r0; memd(r29+16) = r1:r0; if (!p0) jump:nt 0000B550 }

l0000B548:
	{ memw(r17+8) = 00000000; jump 0000B5D0 }

l0000B550:
	{ call fn00009500; r0 = asl(r18,00000002) }
	{ memb(r17+2) = r19.new; r19 = r0 }
	{ r1 = 00000084 }
	{ r22 = 00000000; r20 = 00000000 }

l0000B56C:
	{ r0 = add(r29,00000008); r1 = 00000000; call tensor_alloc }
	{ memw(r19) = r0; if (p0.new) r1 = 0000008D; if (!p0.new) jump:nt 0000B5AC; p0 = cmp.eq(r0,00000000) }

l0000B580:
	{ jump 0000B594; r3 = add(PC,00019F4E) }
0000B58C                                     7C 46 00 00             |F..
0000B590 03 D1 49 6A                                     ..Ij            

l0000B594:
	{ r2 = r16; call errlog_function }
	{ r1 = 000000A8; jump 0000B5EC; r3 = add(PC,00019EBF) }

l0000B5AC:
	{ r3 = memw(r21++#8); r2 = memw(r17+8); r19 = add(r19,00000004); r22 = add(r22,00000001) }
	{ p0 = cmp.gtu(r18,r22) }
	{ r2 = memw(r13+r20); r20 = add(r20,00000004) }
	{ memw(r2+20) = r3; if (p0) jump:nt 0000B56C }

l0000B5D0:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48); r0 = r17 }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ dealloc_return }

l0000B5E0:
	{ memw(r29) = r22; r3 = add(PC,00019E49) }

l0000B5EC:
	{ r2 = r16; r17 = 00000000; call errlog_function }
	{ jump 0000B5D0 }
0000B5F8                         7C 46 00 00 03 45 49 6A         |F...EIj
0000B600 70 FF FF 59                                     p..Y            

;; errlog_function: 0000B604
;;   Called from:
;;     0000B4D8 (in node_alloc_common)
;;     0000B594 (in node_alloc_common)
;;     0000B5EC (in node_alloc_common)
;;     0000B748 (in do_append_node)
;;     0000B7A4 (in do_append_const_node)
;;     0000B7F0 (in do_teardown)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00019E0F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; node_free_common: 0000B628
;;   Called from:
;;     00016E3C (in nop_dtor)
;;     000174A0 (in prefree_dtor)
;;     000194BC (in supernode_dtor)
;;     0001D4D0 (in variable_dtor)
node_free_common proc
	{ allocframe(00000018); memd(r29+496) = r17:r16; r16 = r0; r2 = r1 }
	{ memw(r29) = r16; memd(r29+8) = r19:r18 }
	{ call logmsg_function }
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000B648; r0 = memw(r16+4) }

l0000B648:
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000B654; r0 = memw(r16+12) }

l0000B654:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B684; r2 = memw(r16+20) }

l0000B660:
	{ r2 = memw(r16+8) }
	{ r2 = memw(r13+r17) }
	{ memw(r2+16) = 00000000 }
	{ r2 = memw(r16+8) }
	{ r0 = memw(r13+r17); call tensor_free }
	{ r2 = memw(r16+20); r17 = add(r17,00000004) }
	{ if (cmp.gtu(r2,r18.new)) jump:t 0000B660; r18 = add(r18,00000001) }

l0000B684:
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000B690; r0 = memw(r16+8) }

l0000B688:
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000B694 }

l0000B690:
	{ r0 = r16; call fn00009510 }

l0000B694:
	{ r0 = r16 }

l0000B698:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16); r0 = 00000000 }
	{ dealloc_return }

;; logmsg_function: 0000B6A4
;;   Called from:
;;     0000B638 (in node_free_common)
logmsg_function proc
	{ allocframe(00000008); r4 = 00000003 }
	{ if (cmp.gtu(r4,r3.new)) jump:t 0000B6D4; r3 = memw(r2+16) }

l0000B6B4:
	{ r5 = add(r29,00000010); r3 = 00000003; r0 = add(PC,00000023) }
	{ r6 = add(r29,00000010); r1 = 000000B0; r4 = add(PC,00019DB3) }
	{ memw(r29+4) = r6; call logv }

l0000B6D4:
	{ dealloc_return }
0000B6D8                         00 40 00 7F 00 C0 00 7F         .@......

;; do_append_node: 0000B6E0
;;   Called from:
;;     0000AF84 (in hexagon_nn_append_node)
do_append_node proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r6 = add(PC,0001FA30) }
	{ r6 = memw(r6-208); r17:r16 = combine(r1,r0) }
	{ r6 = memw(r14+r2<<#2); r7 = memw(r29+28) }
	{ memw(r29+4) = r7; r6 = memw(r6+8) }
	{ r7 = memw(r29+24) }
	{ memw(r29) = r7; callr r6 }
	{ if (p0.new) memw(r29) = r17; if (p0.new) jump:nt 0000B738; p0 = cmp.eq(r0,00000000) }

l0000B718:
	{ memw(r0+36) = FFFFFF80 }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B72C; r2 = memw(r16) }

l0000B728:
	{ r16 = add(r2,00000024) }

l0000B72C:
	{ memw(r16) = r0; r17:r16 = memd(r29+8); r0 = 00000000 }
	{ dealloc_return }

l0000B738:
	{ r2 = r16; r1 = 000000D5; r3 = add(PC,00019D47) }
	{ call errlog_function }
	{ r0 = FFFFFFFF }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; do_append_const_node: 0000B754
;;   Called from:
;;     0000AFE0 (in hexagon_nn_append_const_node)
do_append_const_node proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ r6 = memw(r29+28) }
	{ memw(r29+4) = r6; r6 = memd(r29+24) }
	{ memw(r29) = r6; call hexagon_nn_const_ctor }
	{ if (p0.new) memw(r29) = r17; if (p0.new) jump:nt 0000B794; p0 = cmp.eq(r0,00000000) }

l0000B774:
	{ memw(r0+36) = FFFFFF80 }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B788; r2 = memw(r16) }

l0000B784:
	{ r16 = add(r2,00000024) }

l0000B788:
	{ memw(r16) = r0; r17:r16 = memd(r29+8); r0 = 00000000 }
	{ dealloc_return }

l0000B794:
	{ r2 = r16; r1 = 000000FC; r3 = add(PC,00019CEB) }
	{ call errlog_function }
	{ r0 = FFFFFFFF }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; do_teardown: 0000B7B0
;;   Called from:
;;     0000B174 (in hexagon_nn_teardown)
do_teardown proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0; call nn_os_workers_kill }
	{ memb(r16-28) = 00; r2 = memw(r16) }

l0000B7C4:
	{ r1:r0 = combine(r16,r2); if (p0.new) jump:nt 0000B7FC; p0 = cmp.eq(r2,00000000) }

l0000B7CC:
	{ r17 = memw(r2+4); r3 = memw(r2) }
	{ r2 = memw(r3+12) }
	{ callr r2 }
	{ r2 = r17; if (p0.new) jump:nt 0000B7C4; p0 = cmp.eq(r0,00000000) }

l0000B7E0:
	{ r2 = r16; r1 = 0000010E; r3 = add(PC,00019CB6) }
	{ call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29); r0 = FFFFFFFF }

l0000B7FC:
	{ r0 = r16; call allocator_teardown }
	{ r0 = memw(r16+4); call fn00009510 }
	{ r0 = r16; call fn00009510 }
	{ dealloc_return; r17:r16 = memd(r29); r0 = 00000000 }
0000B81C                                     00 00 00 00             ....

;; do_snpprint: 0000B820
;;   Called from:
;;     0000AEC0 (in hexagon_nn_snpprint)
do_snpprint proc
	{ allocframe(00000040); memd(r29+488) = r19:r18; r3 = add(PC,00019D26) }
	{ memd(r29+40) = r21:r20; r18 = r1 }
	{ memd(r29+56) = r17:r16; r16 = r0; r17 = add(r2,FFFFFFFF); r4 = add(r18,add(r2,0000003F)) }
	{ memb(r4) = 00000000; memd(r29+32) = r23:r22; r1:r0 = combine(r17,r18) }
	{ r7 = memw(r16+16); r2 = memw(r16+12) }
	{ memw(r29) = r16; memw(r29+8) = r7 }
	{ memw(r29+4) = r2; r2 = r3 }
	{ call fn000095D0 }
	{ if (!p0.new) r17 = sub(r17,r0); if (p0.new) jump:nt 0000B990; p0 = cmp.eq(r9,r0) }

l0000B85C:
	{ r20 = memw(r16); r18 = add(r18,r0); r19 = 00000000 }

l0000B864:
	{ if (!p0.new) r7 = memb(r20-32); if (!p0.new) r8 = memw(r20+16); r1:r0 = combine(r17,r18); if (p0.new) jump:nt 0000B97C; p0 = cmp.eq(r12,00000000) }

l0000B874:
	{ r5 = memw(r20+28); r4 = memw(r20+24); r3 = add(PC,0001F89C) }
	{ r3 = memw(r3-192); r6 = add(PC,0001FE08) }
	{ r9 = memw(r20+20); r2 = add(PC,00019CD8) }
	{ memb(r29+7) = r6.new; r6 = memw(r6+r7<<#2) }
	{ memw(r29+24) = r7 }
	{ memw(r29+16) = r8; memw(r29+20) = r9 }
	{ memw(r29+8) = r4; memw(r29+12) = r3 }
	{ memw(r29) = r20; memw(r29+4) = r5 }
	{ call fn000095D0 }
	{ if (!p0.new) r17 = sub(r17,r0); if (p0.new) jump:nt 0000B990; p0 = cmp.eq(r9,r0) }

l0000B8C8:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B974; r2 = memw(r16+16); r18 = add(r18,r0) }

l0000B8D8:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B938 }

l0000B8E0:
	{ r3 = memw(r20+12); r4 = memw(r20+4); r2 = add(PC,00019CD2) }
	{ r4 = memw(r13+r22); r3 = memw(r21+r21); r5 = add(r3,r21); r1:r0 = combine(r17,r18) }
	{ memw(r29+12) = r3; r5 = memw(r5-4) }
	{ memw(r29+4) = r4; memw(r29+8) = r5 }
	{ memw(r29) = r23; call fn000095D0 }
	{ if (!p0.new) r17 = sub(r17,r0); if (p0.new) jump:nt 0000B990; p0 = cmp.eq(r9,r0) }

l0000B918:
	{ r2 = memw(r20+16); r21 = add(r21,00000008); r18 = add(r18,r0); r22 = add(r22,00000004) }
	{ if (cmp.gtu(r2,r23.new)) jump:t 0000B8E0; r23 = add(r23,00000001) }

l0000B930:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B978 }

l0000B938:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000B978 }

l0000B940:
	{ r3 = memw(r20+8); r1:r0 = combine(r17,r18); r2 = add(PC,00019C9C) }
	{ r3 = memw(r29+r21) }
	{ memw(r29) = r22; memw(r29+4) = r3; call fn000095D0 }
	{ if (!p0.new) r17 = sub(r17,r0); if (p0.new) jump:nt 0000B990; p0 = cmp.eq(r9,r0) }

l0000B964:
	{ r2 = memw(r20+20); r21 = add(r21,00000004); r18 = add(r18,r0) }
	{ if (cmp.gtu(r2,r22.new)) jump:t 0000B940; r22 = add(r22,00000001) }

l0000B974:
	{ r20 = memw(r20+4); r19 = r19; jump 0000B864 }

l0000B978:
	{ r20 = memw(r20+4); r19 = r19 }

l0000B97C:
	{ memw(r29) = r19; r1:r0 = combine(r17,r18); r2 = add(PC,00019C74) }
	{ call fn000095D0 }

l0000B990:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ dealloc_return }
0000B99C                                     00 00 00 00             ....

;; const_depth_extend_8: 0000B9A0
const_depth_extend_8 proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+16) = r23:r22; r3 = memw(r16+8) }
	{ memd(r29+24) = r21:r20; r21 = r2 }
	{ memd(r29+32) = r19:r18 }
	{ memd(r29+8) = r25:r24; r23 = memw(r3) }
	{ memd(r29) = r27:r26 }
	{ r7 = memw(r23+8); r2 = memw(r23) }
	{ r18 = memw(r23+12); r4 = memw(r23+4) }
	{ r19 = memw(r23+16); r24 = add(r18,r17); r2 = mpyi(r7,r2) }
	{ r25 = mpyi(r2,r4) }
	{ call fn00009500; r0 = add(00000080,mpyi(r24,r25)) }
	{ if (cmp.eq(r20.new,00000000)) jump:nt 0000BA4C; r20 = r0; r2 = FFFFFFFF }

l0000B9F4:
	{ p0 = cmp.gt(r25,00000000); r26 = r25 }
	{ r21 = and(r21,000000FF); r22 = r20 }

l0000BA00:
	{ r1:r0 = combine(r19,r22); r2 = r18; call fn00009560 }
	{ r19 = add(r19,r18); r2 = r17; r1 = r21; r0 = add(r22,r18) }
	{ r22 = add(r22,r24); call fn000095F0 }
	{ if (!cmp.eq(r26.new,00000000)) jump:t 0000BA00; r26 = add(r26,FFFFFFFF) }

l0000BA2C:
	{ r0 = memw(r23+16); call fn00009514 }
	{ memw(r23+12) = r24; memw(r23+16) = r20; r2 = 00000000 }
	{ r3 = memw(r16+8) }
	{ r3 = memw(r3) }
	{ memw(r3+20) = r17 }

l0000BA4C:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40); r0 = r2 }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ r27:r26 = memd(r29); r25:r24 = memd(r29+8) }
	{ dealloc_return }

;; const_width_extend_8: 0000BA64
;;   Called from:
;;     0000BE78 (in try_pad_bad_supernodes)
const_width_extend_8 proc
	{ allocframe(+00000038); memd(r29-56) = r27:r26; r27 = r0 }
	{ memd(r29+24) = r23:r22; r3 = memw(r27+8); r22 = r2 }
	{ memd(r29+40) = r19:r18; r19 = r1 }
	{ memd(r29+48) = r17:r16 }
	{ memd(r29+32) = r21:r20; r23 = memw(r3) }
	{ memd(r29+16) = r25:r24 }
	{ r16 = memw(r23); r21 = memw(r23+8) }
	{ r26 = memw(r23+12); r20 = memw(r23+4); r24 = add(r21,r19) }
	{ r17 = memw(r23+16); r3 = mpyi(r24,r16) }
	{ r25 = mpyi(r3,r20) }
	{ call fn00009500; r0 = add(00000080,mpyi(r25,r26)) }
	{ if (cmp.eq(r18.new,00000000)) jump:nt 0000BB28; r18 = r0; r2 = FFFFFFFF }

l0000BAC4:
	{ if (!cmp.gt(r27.new,00000000)) jump:nt 0000BB04; r27 = mpyi(r20,r16); r19 = mpyi(r26,r19) }

l0000BAD4:
	{ r22 = and(r22,000000FF); r20 = r18 }
	{ r21 = mpyi(r26,r21) }

l0000BADC:
	{ r1:r0 = combine(r17,r20); r2 = r21; call fn00009560 }
	{ r17 = add(r17,r21); r2 = r19; r1 = r22; r0 = add(r20,r21) }
	{ r20 = add(r20,r16); call fn000095F0 }
	{ if (!cmp.eq(r27.new,00000000)) jump:t 0000BADC; r27 = add(r27,FFFFFFFF) }

l0000BB04:
	{ r0 = memw(r23+16); call fn00009510; r16 = mpyi(r25,r26) }

l0000BB08:
	{ r0 = memw(r23+16); call fn00009514 }

l0000BB10:
	{ memw(r23+16) = r18; r3 = memd(r29+4); r2 = 00000000 }
	{ memw(r23+8) = r24 }
	{ r3 = memw(r3+8) }
	{ r3 = memw(r3) }
	{ memw(r3+20) = r16 }

l0000BB28:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48); r0 = r2 }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }

;; check_same_inputs: 0000BB40
check_same_inputs proc
	{ if (cmp.gtu(r3,r4.new)) jump:t 0000BB90; r4 = memw(r1+16) }

l0000BB4C:
	{ if (cmp.gtu(r3,r4.new)) jump:t 0000BB94; r4 = memw(r2+16) }

l0000BB58:
	{ if (!p0.new) jumpr:nt r31 }
0000BB5C                                     00 41 03 60             .A.`
0000BB60 25 03 14 03 82 40 04 B0 83 C0 05 B0 E4 FF 82 97 %....@..........
0000BB70 E5 7F 83 97 10 C4 42 20 04 C0 82 91 05 40 83 91 ......B .....@..
0000BB80 0A C4 42 20 00 80 00 7F 83 20 82 20 C0 3F 00 48 ..B ..... . .?.H

l0000BB90:
	{ r0 = FFFFFFFF }

l0000BB94:
	{ jumpr r31 }

;; try_pad_bad_supernodes: 0000BB98
;;   Called from:
;;     0000C640 (in do_prepare)
try_pad_bad_supernodes proc
	{ allocframe(00000060); memd(r29+496) = r17:r16 }
	{ r16 = memw(r1); r17 = r0 }
	{ memd(r29+72) = r21:r20; memd(r29+80) = r19:r18; r19 = 00000000 }
	{ r2 = memw(r16+24) }
	{ memd(r29+56) = r25:r24; memd(r29+64) = r23:r22; if (!p0) r1:r0 = combine(r16,r17); p0 = cmp.eq(r2,00000031) }
	{ memd(r29+48) = r27:r26; if (!p0) jump:nt 0000BECC }

l0000BBC4:
	{ call find_first_consumer.1.2_i32_0 }
	{ if (cmp.eq(r18.new,r16)) jump:nt 0000BECC; r18 = r0 }

l0000BBD4:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000BC14 }

l0000BBDC:
	{ r19 = 00000000 }

l0000BBE0:
	{ if (!cmp.gtu(r2,r20.new)) jump:nt 0000BC10; r20 = add(r20,00000001) }

l0000BBEC:
	{ r1:r0 = combine(r16,r17); r2 = r20 }
	{ if (!p0) r1:r0 = combine(r16,r17); if (p0.new) r2 = add(r20,00000000); if (!p0.new) jump:nt 0000BECC; p0 = cmp.eq(r0,r10) }

l0000BC00:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r16+20); if (p0.new) jump:nt 0000BBE0; p0 = cmp.eq(r0,r10) }

l0000BC0C:
	{ jump 0000BECC }

l0000BC10:
	{ r2 = memw(r18+24); r19 = 00000000 }

l0000BC14:
	{ p0 = cmp.eq(r2,00000031); if (!p0.new) jump:nt 0000BECC }

l0000BC1C:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000BECC; r2 = memw(r17) }

l0000BC28:
	{ r3 = memw(r4+56); jump 0000BC38 }

l0000BC30:
	{ if (cmp.eq(r5.new,00000000)) jump:nt 0000BECC; r5 = memw(r5+36) }

l0000BC34:
	{ if (cmp.eq(r5.new,00000000)) jump:nt 0000BED0 }

l0000BC38:
	{ if (!cmp.eq(r6.new,r3)) jump:t 0000BC30; r6 = memw(r5+28) }

l0000BC3C:
	{ if (!cmp.eq(r6.new,r3)) jump:t 0000BC34 }

l0000BC44:
	{ r5 = memw(r5) }
	{ r24 = memw(r5+12) }
	{ if (!p0.new) r26 = add(r2,00000000); r5 = add(r24,0000001F); if (p0.new) jump:nt 0000BECC; p0 = bitsclr(r24,0000001F) }

l0000BC5C:
	{ r4 = memw(r4+8); r22 = and(r5,FFFFFFE0) }
	{ r20 = sub(r22,r24); jump 0000BC74 }

l0000BC6C:
	{ if (cmp.eq(r26.new,00000000)) jump:nt 0000BECC; r26 = memw(r26+36) }

l0000BC70:
	{ if (cmp.eq(r26.new,00000000)) jump:nt 0000BED0 }

l0000BC74:
	{ if (!cmp.eq(r5.new,r4)) jump:t 0000BC6C; r5 = memw(r26+28) }

l0000BC78:
	{ if (!cmp.eq(r5.new,r4)) jump:t 0000BC70 }

l0000BC80:
	{ r25 = r2 }
	{ if (cmp.eq(r25.new,00000000)) jump:nt 0000BECC; r25 = memw(r25+36) }

l0000BC88:
	{ if (cmp.eq(r25.new,00000000)) jump:nt 0000BED0 }

l0000BC90:
	{ if (!cmp.eq(r4.new,r3)) jump:t 0000BC88 }

l0000BC98:
	{ r4 = memw(r3+8); jump 0000BCA8 }

l0000BCA0:
	{ if (cmp.eq(r21.new,00000000)) jump:nt 0000BECC; r21 = memw(r21+36) }

l0000BCA4:
	{ if (cmp.eq(r21.new,00000000)) jump:nt 0000BED0 }

l0000BCA8:
	{ if (!cmp.eq(r5.new,r4)) jump:t 0000BCA0; r5 = memw(r21+28) }

l0000BCAC:
	{ if (!cmp.eq(r5.new,r4)) jump:t 0000BCA4 }

l0000BCB4:
	{ r5 = memw(r3+32) }
	{ if (cmp.eq(r4.new,00000000)) jump:nt 0000BECC; r4 = memw(r4+36) }

l0000BCBC:
	{ if (cmp.eq(r4.new,00000000)) jump:nt 0000BED0 }

l0000BCC4:
	{ if (!cmp.eq(r6.new,r5)) jump:t 0000BCBC }

l0000BCCC:
	{ r3 = memw(r3+40) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000BECC; r2 = memw(r2+36) }

l0000BCD4:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000BED0 }

l0000BCDC:
	{ if (!cmp.eq(r5.new,r3)) jump:t 0000BCD4 }

l0000BCE4:
	{ r2 = memw(r2+8); r3 = memw(r4+8); r0 = 00000017 }
	{ r3 = memw(r3) }
	{ r2 = memw(r2) }
	{ r3 = memw(r3+16); r2 = memw(r2+16) }
	{ r19 = memw(r3) }
	{ r2 = memw(r2) }
	{ call fn00009600; r1 = sfsub(r2,r19) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = 00000000 }
	{ r2 = sfsub(r2,r19) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r5 = memw(r16+28); r2 = convert_uw2sf(r0):chop; r4 = add(PC,0001991F) }
	{ memw(r29+12) = r22; p1 = cmp.gt(r2,FFFFFFFF); r1 = 000001C1 }
	{ memw(r29+8) = r20; if (!p0.new) r3 = zxtb(r2); if (p0.new) r3 = 000000FF; p0 = cmp.gt(r2,000000FF) }
	{ memw(r29) = r24; memw(r29+4) = r5; if (!p1) r3 = 00000000 }
	{ memw(r29+16) = r3; memw(r29+28) = r3; r3:r2 = combine(00000002,r17) }
	{ call logmsg_function }
	{ r2 = memw(r25+8); r19 = FFFFFFFF }
	{ r23 = memw(r2) }
	{ r6 = memw(r23+12); r7 = memw(r23) }
	{ r4 = memw(r23+4); r1 = memw(r23+8); r3 = add(r6,r20) }
	{ r5 = memw(r23+16); r2 = mpyi(r1,r7) }
	{ memw(r29+40) = r5; memw(r29+32) = r6 }
	{ memw(r29+44) = r3; r2 = mpyi(r2,r4) }
	{ memw(r29+36) = r2; r0 = add(00000080,mpyi(r3,r2)) }
	{ call fn00009500 }
	{ if (cmp.eq(r27.new,00000000)) jump:t 0000BECC; r27 = r0 }

l0000BDB8:
	{ r0 = memd(r29+36); r3 = memd(r29+44); r2 = r20 }
	{ r5 = memd(r29+40); r1 = memd(r29+32) }
	{ memb(r29+10) = r6.new; call try_pad_bad_supernodes.extracted_region; r6 = mpyi(r3,r0) }
	{ r0 = memw(r23+16) }
	{ memw(r23+16) = r27; r2 = memw(r29+44) }
	{ memw(r23+12) = r2 }
	{ r3 = memw(r29+40); r2 = memw(r25+8) }
	{ r2 = memw(r2) }
	{ memw(r2+20) = r3 }
	{ r7 = memw(r26+8) }
	{ r25 = memw(r7) }
	{ r5 = memw(r25+12); r2 = memw(r25+8) }
	{ r4 = memw(r25+4); r7 = memw(r25) }
	{ r3 = memw(r25+16); r23 = add(r5,r20); r2 = mpyi(r2,r7) }
	{ memw(r29+44) = r3; memw(r29+36) = r5; r2 = mpyi(r2,r4) }
	{ memw(r29+40) = r2; r0 = add(00000080,mpyi(r23,r2)) }
	{ call fn00009500 }
	{ if (cmp.eq(r27.new,00000000)) jump:nt 0000BECC; r27 = r0; r3:r2 = combine(r23,r20) }

l0000BE40:
	{ r1 = memd(r29+36); r0 = memd(r29+40) }
	{ r5 = memw(r29+44) }
	{ memb(r29+11) = r6.new; call try_pad_bad_supernodes.extracted_region; r6 = mpyi(r23,r0) }
	{ r0 = memw(r25+16) }
	{ memw(r25+12) = r23; memw(r25+16) = r27; r1:r0 = combine(r20,r21) }
	{ r2 = memw(r29+28); r3 = memw(r26+8) }
	{ r4 = memw(r29+44) }
	{ r3 = memw(r3) }
	{ memw(r3+20) = r4; call const_width_extend_8 }
	{ if (p0.new) r2 = memw(r16+8); if (!p0.new) jump:nt 0000BECC; p0 = cmp.eq(r0,00000000) }

l0000BE88:
	{ r1:r0 = sxtw(r22) }
	{ r19 = memw(r2); r3:r2 = sxtw(r24) }
	{ r4 = memw(r19+20) }
	{ r7:r6 = mpyu(r4,r0) }
	{ r7 += mpyi(r4,r1) }
	{ r1:r0 = combine(r7,r6); call fn00009630 }
	{ memw(r19+20) = r0; r3:r2 = combine(00000002,r17); r4 = add(PC,000197E1) }
	{ r6 = memw(r18+28); r5 = memw(r16+28); r1 = 000001C6 }
	{ memw(r29+4) = r6 }
	{ memw(r29) = r5; r19 = 00000000; call logmsg_function }

l0000BECC:
	{ r27:r26 = memd(r29+48); r25:r24 = memd(r29+56); r0 = r19 }

l0000BED0:
	{ r27:r26 = memd(r29+48); r25:r24 = memd(r29+56) }
	{ r19:r18 = memd(r29+80); r17:r16 = memd(r29+88) }
	{ r23:r22 = memd(r29+64); r21:r20 = memd(r29+72) }
	{ dealloc_return }

;; logmsg_function: 0000BEE4
;;   Called from:
;;     0000BD6C (in try_pad_bad_supernodes)
;;     0000BEC4 (in try_pad_bad_supernodes)
;;     0000BF80 (in do_prepare)
;;     0000C03C (in do_prepare)
;;     0000C0E8 (in do_prepare)
;;     0000C1C0 (in do_prepare)
;;     0000C298 (in do_prepare)
;;     0000C2D8 (in do_prepare)
;;     0000C34C (in do_prepare)
;;     0000C3BC (in do_prepare)
;;     0000C430 (in do_prepare)
;;     0000C4A0 (in do_prepare)
;;     0000C614 (in do_prepare)
;;     0000C6EC (in do_prepare)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000BF04; r5 = memw(r2+16) }

l0000BEF4:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,00000005) }
	{ memw(r29+4) = r6; call logv }

l0000BF04:
	{ dealloc_return }

;; do_prepare: 0000BF08
;;   Called from:
;;     0000AF14 (in hexagon_nn_prepare)
do_prepare proc
	{ allocframe(000000A8); memd(r29+496) = r17:r16; r16 = r0 }
	{ memd(r29+144) = r21:r20; memd(r29+152) = r19:r18 }
	{ memd(r29+128) = r25:r24; memd(r29+136) = r23:r22 }
	{ memd(r29+120) = r27:r26; call nn_os_hvx_power_on }
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000BF48; r2 = memb(r16+36); r1 = 000001E5 }

l0000BF34:
	{ r3 = add(PC,00000006) }

l0000BF38:
	{ r2 = r16 }

l0000BF3C:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0000C734 }

l0000BF48:
	{ if (cmp.eq(r22.new,00000000)) jump:nt 0000BF98; r21 = add(r29,00000708); r17 = memw(r16) }

l0000BF5C:
	{ r23 = FFFFFFCC }

l0000BF64:
	{ r2 = memw(r17+24) }
	{ if (p0.new) r1 = 000000D1; p0 = cmp.eq(r2,0000004D); if (!p0.new) jump:nt 0000C0F4 }

l0000BF74:
	{ r3:r2 = combine(00000009,r16); r4 = add(PC,000198B3) }
	{ call logmsg_function }
	{ r1:r0 = combine(r17,r16); call find_first_consumer.1.2_i32_0 }
	{ if (cmp.eq(r18.new,r17)) jump:nt 0000C0F4; r18 = r0 }

l0000BF98:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000BFD8; r2 = memw(r17+20) }

l0000BFA4:
	{ if (!cmp.gtu(r2,r19.new)) jump:nt 0000BFD4; r19 = add(r19,00000001) }

l0000BFB0:
	{ r1:r0 = combine(r17,r16); r2 = r19 }
	{ if (!p0) r1:r0 = combine(r17,r16); if (p0.new) r2 = add(r19,00000000); if (!p0.new) jump:nt 0000C0F4; p0 = cmp.eq(r0,r10) }

l0000BFC4:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r17+20); if (p0.new) jump:nt 0000BFA4; p0 = cmp.eq(r0,r10) }

l0000BFD0:
	{ jump 0000C0F4 }

l0000BFD4:
	{ r2 = memw(r18+24) }

l0000BFD8:
	{ p0 = cmp.eq(r2,0000004B); if (!p0.new) jump:nt 0000C0F4 }

l0000BFE0:
	{ if (cmp.gtu(r20,r2.new)) jump:t 0000C0F4; r2 = memw(r17+16) }

l0000BFEC:
	{ if (cmp.gtu(r20,r2.new)) jump:t 0000C0F8 }

l0000BFF4:
	{ r3 = memw(r18+12); r4 = memw(r17+12) }
	{ r4 = add(r4,00000004); r3 = add(r3,00000004); jump 0000C010 }

l0000C000:
	{ if (cmp.gt(r2.new,00000002)) jump:nt 0000C02C; r2 = add(r2,00000001); r3 = add(r3,00000008); r4 = add(r4,00000008) }

l0000C010:
	{ r5 = memw(r4-4) }

l0000C014:
	{ if (!cmp.eq(r6.new,r5)) jump:nt 0000C0F4; r6 = memw(r3-4) }

l0000C020:
	{ if (cmp.eq(r6.new,r5)) jump:t 0000C000; r6 = memw(r3) }

l0000C02C:
	{ r1 = 000000D8; r3:r2 = combine(00000009,r16); r4 = add(PC,00019815) }
	{ call logmsg_function }
	{ r2 = memw(r18+8); r3 = setbit(r21,00000004); r4 = add(PC,0001F0D0) }
	{ r0 = r16; r5 = 00000003 }
	{ r6 = memw(r2); r2 = 00000013 }
	{ r6 = memw(r6+20) }
	{ memw(r3) = 00000000; memw(r29+8) = r6 }
	{ r7 = memw(r18+8) }
	{ r1 = memw(r7+4) }
	{ memb(r29+4) = r3.new; r3 = memw(r1+20) }
	{ r7 = memw(r18+8) }
	{ r3 = memw(r7+8) }
	{ r3 = memw(r3+20) }
	{ memw(r21+20) = 00000000; memw(r29+24) = r3 }
	{ r6 = memw(r13+r23) }
	{ r3 = memb(r18+32); r6 = memw(r6+76) }
	{ r1 = memw(r18+28); r4 = memw(r17+16) }
	{ r7 = memw(r17+12); r6 = memw(r6+8) }
	{ memw(r29+4) = r21 }
	{ memw(r29) = r7; callr r6 }
	{ if (cmp.eq(r19.new,00000000)) jump:nt 0000C0F4; r19 = r0 }

l0000C0B0:
	{ memw(r22) = r19 }
	{ memb(r19+9) = r7.new; r7 = memw(r18+36) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r2 = memw(r18); r1:r0 = combine(r16,r18) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1 = 000000EB; r3:r2 = combine(00000003,r16); r4 = add(PC,00019783) }
	{ memb(r29) = r5.new; r5 = memw(r19+28); call logmsg_function }

l0000C0F4:
	{ r2 = memw(r22) }

l0000C0F8:
	{ r22 = add(r2,00000024) }
	{ if (!cmp.eq(r17.new,00000000)) jump:t 0000BF64; r17 = memw(r2+36) }

l0000C108:
	{ if (cmp.eq(r22.new,00000000)) jump:nt 0000C168; r21 = add(r29,00000560) }

l0000C118:
	{ r2 = memw(r17+24) }
	{ if (p0.new) r3 = memw(r17+12); p0 = cmp.eq(r2,0000004B); if (!p0.new) jump:nt 0000C29C }

l0000C128:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0000C29C; r2 = memw(r3+36) }

l0000C134:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C2A0 }

l0000C13C:
	{ r3 = memw(r3+32) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C29C; r2 = memw(r2+36) }

l0000C144:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C2A0 }

l0000C14C:
	{ if (!cmp.eq(r4.new,r3)) jump:t 0000C144 }

l0000C154:
	{ if (!cmp.eq(r2.new,00000003)) jump:t 0000C2A0 }

l0000C15C:
	{ r1:r0 = combine(r17,r16) }
	{ if (cmp.eq(r18.new,r17)) jump:nt 0000C29C; r18 = r0 }

l0000C168:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C1A8; r2 = memw(r17+20); r19 = 00000000 }

l0000C16C:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C1AC; r2 = memw(r17+20) }

l0000C178:
	{ if (!cmp.gtu(r2,r19.new)) jump:nt 0000C1A8; r19 = add(r19,00000001) }

l0000C184:
	{ r1:r0 = combine(r17,r16); r2 = r19 }
	{ if (!p0) r1:r0 = combine(r17,r16); if (p0.new) r2 = add(r19,00000000); if (!p0.new) jump:nt 0000C29C; p0 = cmp.eq(r0,r10) }

l0000C198:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r17+20); if (p0.new) jump:nt 0000C178; p0 = cmp.eq(r0,r10) }

l0000C1A4:
	{ jump 0000C29C }

l0000C1A8:
	{ if (!cmp.eq(r2.new,00000015)) jump:t 0000C29C; r2 = memw(r18+24); r1 = 00000106 }

l0000C1AC:
	{ if (!cmp.eq(r2.new,00000015)) jump:t 0000C2A0; r2 = memw(r18+24) }

l0000C1B8:
	{ r3:r2 = combine(00000009,r16); r4 = add(PC,00000029) }
	{ call logmsg_function }
	{ r2 = memw(r18+8); r3 = setbit(r20,00000004); r6 = add(PC,0001EF4C) }
	{ r2 = memw(r2); r5:r4 = combine(00000003,00000004) }
	{ r2 = memw(r2+20) }
	{ memw(r3) = 00000000; memw(r29+8) = r2; r2 = 00000017 }
	{ r3 = memw(r18+12) }
	{ r0 = memw(r3) }
	{ r1 = memw(r3+4) }
	{ memd(r29+32) = r1:r0 }
	{ r7 = memw(r18+8) }
	{ r3 = memw(r7+4) }
	{ r3 = memw(r3+20) }
	{ memw(r20+12) = 00000000; memw(r29+16) = r3 }
	{ r7 = memw(r18+12) }
	{ r0 = memw(r7+8) }
	{ r1 = memw(r7+12) }
	{ memd(r29+40) = r1:r0 }
	{ r3 = memw(r18+8) }
	{ r3 = memw(r3+8) }
	{ r3 = memw(r3+20) }
	{ memw(r20+20) = 00000000; memw(r29+24) = r3 }
	{ r7 = memw(r18+12) }
	{ r0 = memw(r7+16) }
	{ r1 = memw(r7+20) }
	{ memd(r29+48) = r1:r0 }
	{ r3 = memw(r17+12) }
	{ r0 = memw(r3+32) }
	{ r1 = memw(r3+36) }
	{ memd(r29+56) = r1:r0 }
	{ r6 = memw(r13+r23); r0 = r16 }
	{ r3 = memb(r17+32); r6 = memw(r6+92) }
	{ r1 = memw(r18+28) }
	{ memw(r29+4) = r20; r6 = memw(r6+8) }
	{ memw(r29) = r21; callr r6 }
	{ if (p0.new) memw(r17+36) = r0; if (p0.new) jump:nt 0000C29C; p0 = cmp.eq(r0,00000000) }

l0000C270:
	{ memb(r0+9) = r7.new; r7 = memw(r18+36); r1:r0 = combine(r16,r18) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1 = 0000011A; r3:r2 = combine(00000003,r16); r4 = add(PC,00019569) }
	{ call logmsg_function }

l0000C29C:
	{ r2 = memw(r22) }

l0000C2A0:
	{ r22 = add(r2,00000024) }
	{ if (!cmp.eq(r17.new,00000000)) jump:t 0000C118; r17 = memw(r2+36) }

l0000C2B0:
	{ if (cmp.eq(r17.new,00000000)) jump:nt 0000C660; r17 = memw(r16) }

l0000C2BC:
	{ r25 = add(r29,00000020) }

l0000C2C0:
	{ if (!cmp.eq(r2.new,0000000F)) jump:t 0000C620; r2 = memw(r17+24); r1 = 00000133 }

l0000C2D0:
	{ r3:r2 = combine(00000009,r16); r4 = add(PC,00000026) }
	{ call logmsg_function }
	{ r1:r0 = combine(r17,r16); call find_first_consumer.1.2_i32_0 }
	{ if (cmp.eq(r18.new,r17)) jump:nt 0000C620; r18 = r0 }

l0000C2F0:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C330; r2 = memw(r17+20) }

l0000C2FC:
	{ if (!cmp.gtu(r2,r19.new)) jump:nt 0000C32C; r19 = add(r19,00000001) }

l0000C308:
	{ r1:r0 = combine(r17,r16); r2 = r19 }
	{ if (!p0) r1:r0 = combine(r17,r16); if (p0.new) r2 = add(r19,00000000); if (!p0.new) jump:nt 0000C620; p0 = cmp.eq(r0,r10) }

l0000C31C:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r17+20); if (p0.new) jump:nt 0000C2FC; p0 = cmp.eq(r0,r10) }

l0000C328:
	{ jump 0000C620 }

l0000C32C:
	{ if (cmp.eq(r2.new,00000013)) jump:t 0000C33C; r2 = memw(r18+24) }

l0000C330:
	{ if (cmp.eq(r2.new,00000013)) jump:t 0000C340 }

l0000C338:
	{ p0 = cmp.eq(r2,0000004B) }

l0000C33C:
	{ r1 = 0000013B; r3:r2 = combine(00000009,r16); r4 = add(PC,00019444) }

l0000C340:
	{ r1 = 0000013B; r3:r2 = combine(00000009,r16); r4 = add(PC,00000004) }

l0000C34C:
	{ call logmsg_function }
	{ r1:r0 = combine(r18,r16); call find_first_consumer.1.2_i32_0 }
	{ if (cmp.eq(r19.new,r18)) jump:nt 0000C620; r19 = r0 }

l0000C364:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C3A4; r2 = memw(r18+20) }

l0000C370:
	{ if (!cmp.gtu(r2,r20.new)) jump:nt 0000C3A0; r20 = add(r20,00000001) }

l0000C37C:
	{ r1:r0 = combine(r18,r16); r2 = r20 }
	{ if (!p0) r1:r0 = combine(r18,r16); if (p0.new) r2 = add(r20,00000000); if (!p0.new) jump:nt 0000C620; p0 = cmp.eq(r0,r11) }

l0000C390:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r18+20); if (p0.new) jump:nt 0000C370; p0 = cmp.eq(r0,r11) }

l0000C39C:
	{ jump 0000C620 }

l0000C3A0:
	{ r2 = memw(r19+24) }

l0000C3A4:
	{ if (p0.new) r1 = 00000140; p0 = cmp.eq(r2,00000023); if (!p0.new) jump:nt 0000C620 }

l0000C3B0:
	{ r3:r2 = combine(00000009,r16); r4 = add(PC,000193DE) }
	{ call logmsg_function }
	{ r1:r0 = combine(r19,r16); call find_first_consumer.1.2_i32_0 }
	{ if (cmp.eq(r20.new,r19)) jump:nt 0000C620; r20 = r0 }

l0000C3D4:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C414; r2 = memw(r19+20) }

l0000C3E0:
	{ if (!cmp.gtu(r2,r21.new)) jump:nt 0000C410; r21 = add(r21,00000001) }

l0000C3EC:
	{ r1:r0 = combine(r19,r16); r2 = r21 }
	{ if (!p0) r1:r0 = combine(r19,r16); if (p0.new) r2 = add(r21,00000000); if (!p0.new) jump:nt 0000C620; p0 = cmp.eq(r0,r12) }

l0000C400:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r19+20); if (p0.new) jump:nt 0000C3E0; p0 = cmp.eq(r0,r12) }

l0000C40C:
	{ jump 0000C620 }

l0000C410:
	{ if (cmp.eq(r2.new,00000013)) jump:t 0000C420; r2 = memw(r18+24) }

l0000C414:
	{ if (cmp.eq(r2.new,00000013)) jump:t 0000C424 }

l0000C41C:
	{ p0 = cmp.eq(r2,0000004B) }

l0000C420:
	{ r1 = 00000145; r3:r2 = combine(00000009,r16); r4 = add(PC,0001937D) }

l0000C424:
	{ r1 = 00000145; r3:r2 = combine(00000009,r16); r4 = add(PC,0000003D) }

l0000C430:
	{ call logmsg_function }
	{ r1:r0 = combine(r20,r16); call find_first_consumer.1.2_i32_0 }
	{ if (cmp.eq(r21.new,r20)) jump:nt 0000C620; r21 = r0 }

l0000C448:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C488; r2 = memw(r20+20) }

l0000C454:
	{ if (!cmp.gtu(r2,r22.new)) jump:nt 0000C484; r22 = add(r22,00000001) }

l0000C460:
	{ r1:r0 = combine(r20,r16); r2 = r22 }
	{ if (!p0) r1:r0 = combine(r20,r16); if (p0.new) r2 = add(r22,00000000); if (!p0.new) jump:nt 0000C620; p0 = cmp.eq(r0,r13) }

l0000C474:
	{ call find_last_consumer }
	{ if (p0.new) r2 = memw(r20+20); if (p0.new) jump:nt 0000C454; p0 = cmp.eq(r0,r13) }

l0000C480:
	{ jump 0000C620 }

l0000C484:
	{ r2 = memw(r21+24) }

l0000C488:
	{ if (!cmp.eq(r2.new,00000017)) jump:nt 0000C620; r2 = setbit(r2,00000002) }

l0000C494:
	{ r1 = 0000014B; r3:r2 = combine(00000009,r16); r4 = add(PC,0000001B) }
	{ call logmsg_function }
	{ r2 = memw(r17+12) }
	{ r1 = memw(r2+4); r0 = memw(r2) }
	{ memd(r29+32) = r1:r0 }
	{ r7 = memw(r17+12) }
	{ r5 = memw(r7+12); r4 = memw(r7+8) }
	{ memd(r29+40) = r5:r4 }
	{ r2 = memw(r17+12) }
	{ r1 = memw(r2+20); r0 = memw(r2+16) }
	{ memd(r29+48) = r1:r0 }
	{ r7 = memw(r17+12) }
	{ r5 = memw(r7+28); r4 = memw(r7+24) }
	{ memd(r29+56) = r5:r4 }
	{ r2 = memw(r17+12) }
	{ r1 = memw(r2+4); r0 = memw(r2) }
	{ memd(r29+64) = r1:r0 }
	{ r7 = memw(r17+12) }
	{ r5 = memw(r7+12); r4 = memw(r7+8) }
	{ memd(r29+72) = r5:r4 }
	{ r2 = memw(r17+12) }
	{ r1 = memw(r2+20); r0 = memw(r2+16) }
	{ memd(r29+80) = r1:r0 }
	{ r7 = memw(r19+12) }
	{ r5 = memw(r7+12); r4 = memw(r7+8) }
	{ memd(r29+88) = r5:r4; r4 = 0000000A }
	{ r2 = memw(r19+12) }
	{ r1 = memw(r2+4); r0 = memw(r2) }
	{ memd(r29+96) = r1:r0 }
	{ r5 = memw(r19+12) }
	{ r7 = memw(r5+12); r6 = memw(r5+8) }
	{ memd(r29+104) = r7:r6 }
	{ if (!cmp.eq(r2.new,00000017)) jump:t 0000C530; r2 = memw(r21+24) }

l0000C528:
	{ r1 = memw(r2+28); r0 = memw(r2+24) }
	{ memd(r29+112) = r1:r0 }

l0000C530:
	{ r2 = memw(r21+8); r3 = setbit(r24,00000004); r6 = add(PC,0001EBE0) }
	{ r2 = memw(r2); r5 = 00000003 }
	{ memb(r29+2) = r7.new; r7 = memw(r2+20); r2 = 00000031 }
	{ r1 = memw(r21+8) }
	{ r0 = memw(r1+4) }
	{ memb(r29+4) = r3.new; r3 = memw(r0+20); r0 = r16 }
	{ r7 = memw(r21+8) }
	{ r3 = memw(r7+8) }
	{ r3 = memw(r3+20) }
	{ memw(r24+20) = FFFFFF80; memw(r29+24) = r3 }
	{ r6 = memw(r13+r23) }
	{ r1 = memw(r21+28); r6 = memw(r6+196) }
	{ r3 = memb(r17+32) }
	{ memw(r29+4) = r24; r6 = memw(r6+8) }
	{ memw(r29) = r25; callr r6 }
	{ if (cmp.eq(r22.new,00000000)) jump:nt 0000C620; r22 = r0 }

l0000C5AC:
	{ memw(r26) = r22 }
	{ memb(r22+9) = r7.new; r7 = memw(r21+36) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r2 = memw(r18); r1:r0 = combine(r16,r18) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r2 = memw(r19); r1:r0 = combine(r16,r19) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r2 = memw(r20); r1:r0 = combine(r16,r20) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r2 = memw(r21); r1:r0 = combine(r16,r21) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1 = 00000176; r3:r2 = combine(00000003,r16); r4 = add(PC,000191B3) }
	{ memb(r29) = r5.new; r5 = memw(r22+28); call logmsg_function }

l0000C620:
	{ r2 = memw(r26) }

l0000C624:
	{ r26 = add(r2,00000024) }
	{ if (!cmp.eq(r17.new,00000000)) jump:t 0000C2C0; r17 = memw(r2+36) }

l0000C634:
	{ r2 = memw(r17) }

l0000C638:
	{ if (!p0.new) r1:r0 = combine(r17,r16); if (p0.new) jump:nt 0000C65C; p0 = cmp.eq(r2,00000000) }

l0000C640:
	{ call try_pad_bad_supernodes }
	{ if (p0.new) r2 = memw(r17); if (!p0.new) r1 = 000001CE; if (!p0.new) jump:nt 0000C748; p0 = cmp.eq(r0,00000000) }

l0000C650:
	{ r17 = add(r2,00000024) }
	{ r2 = memw(r2+36); jump 0000C638 }

l0000C65C:
	{ r0 = r16; call allocate_graph_storage }

l0000C660:
	{ r0 = r16 }

l0000C664:
	{ if (!p0.new) jump:nt 0000C734; p0 = cmp.eq(r0,00000000) }

l0000C668:
	{ if (cmp.eq(r17.new,00000000)) jump:nt 0000C714; r17 = memw(r16) }

l0000C674:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C700; r2 = memw(r17+16) }

l0000C67C:
	{ r3 = memw(r17+12) }

l0000C680:
	{ if (cmp.eq(r4.new,00000000)) jump:nt 0000C6DC; r4 = memw(r23+r18<<#3) }

l0000C68C:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C758; r2 = memw(r16) }

l0000C698:
	{ r5 = memw(r3+4) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C754; r2 = memw(r2+36) }

l0000C6A0:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000C758 }

l0000C6A8:
	{ if (!cmp.eq(r3.new,r4)) jump:t 0000C6A0 }

l0000C6B0:
	{ if (cmp.gtu(r3.new,r5)) jump:t 0000C6D0 }

l0000C6B8:
	{ memw(r29+4) = r4; r1 = 0000007E; r3 = add(PC,0000001A) }
	{ memw(r29) = r5; r2 = r16 }
	{ jump 0000BF3C }
0000C6CC                                     93 01 22 02             ..".

l0000C6D0:
	{ r2 = memw(r14+r5<<#2) }
	{ memw(r31+r18<<#2) = r2; jump 0000C6F0 }

l0000C6DC:
	{ r1 = 00000083; r3:r2 = combine(00000001,r16); r4 = add(PC,00019058) }
	{ call logmsg_function }

l0000C6F0:
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r2,r18.new)) jump:t 0000C67C; r18 = add(r18,00000001) }

l0000C700:
	{ if (!cmp.eq(r17.new,00000000)) jump:t 0000C674 }

l0000C708:
	{ if (!p0.new) r2 = memw(r17); r1:r0 = combine(r16,r17); if (!p0.new) jump:nt 0000C724; p0 = cmp.eq(r9,00000000) }

l0000C714:
	{ r0 = r16; call nn_os_hvx_power_off }
	{ memb(r16-28) = 02; jump 0000C734; r0 = 00000000 }

l0000C724:
	{ r2 = memw(r2+4) }
	{ callr r2 }
	{ r17 = memw(r17+36); if (p0.new) jump:nt 0000C708; p0 = cmp.eq(r0,00000000) }

l0000C734:
	{ r19:r18 = memd(r29+152); r17:r16 = memd(r29+160) }
	{ r23:r22 = memd(r29+136); r21:r20 = memd(r29+144) }
	{ r27:r26 = memd(r29+120); r25:r24 = memd(r29+128) }
	{ dealloc_return }

l0000C748:
	{ jump 0000BF38; r3 = add(PC,0001901E) }

l0000C754:
	{ memb(r29+1) = r2.new; r2 = memw(r17+28); r3 = add(PC,00018F88) }

l0000C758:
	{ memb(r29+1) = r2.new; r2 = memw(r17+28); r3 = add(PC,00000008) }

l0000C768:
	{ r2 = r16 }
	{ memw(r29) = r4; jump 0000BF3C }

;; errlog_function: 0000C774
;;   Called from:
;;     0000BF3C (in do_prepare)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00018EC1) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; try_pad_bad_supernodes.extracted_region: 0000C798
;;   Called from:
;;     0000BDC4 (in try_pad_bad_supernodes)
;;     0000BE48 (in try_pad_bad_supernodes)
try_pad_bad_supernodes.extracted_region proc
	{ allocframe(00000018); memd(r29+496) = r17:r16 }
	{ memd(r29+8) = r19:r18; r19:r18 = combine(r4,r3); r17:r16 = combine(r2,r1) }
	{ memd(r29) = r21:r20; r21:r20 = combine(r0,r5) }
	{ if (!p0.new) jump:nt 0000C7DC; p0 = cmp.gt(r13,00000000) }

l0000C7B4:
	{ r1:r0 = combine(r20,r19); r2 = r16; call fn00009560 }
	{ r20 = add(r20,r16); r1 = 00000000; r2 = r17; r0 = add(r19,r16) }
	{ r19 = add(r19,r18); call fn000095F0 }
	{ if (!cmp.eq(r21.new,00000000)) jump:t 0000C7B4; r21 = add(r21,FFFFFFFF) }

l0000C7DC:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }

l0000C7E0:
	{ dealloc_return; r21:r20 = memd(r29) }

;; find_first_consumer.1.2_i32_0: 0000C7E4
;;   Called from:
;;     0000BBC4 (in try_pad_bad_supernodes)
;;     0000BF84 (in do_prepare)
;;     0000C2DC (in do_prepare)
;;     0000C350 (in do_prepare)
;;     0000C3C0 (in do_prepare)
;;     0000C434 (in do_prepare)
find_first_consumer.1.2_i32_0 proc
	{ jump 0000C854; r0 = 00000240 }
0000C7EC                                     00 00 00 00             ....

;; tensor_alloc: 0000C7F0
;;   Called from:
;;     0000B56C (in node_alloc_common)
tensor_alloc proc
	{ allocframe(00000010); memd(r29+488) = r19:r18; r0 = 00000020; r18 = r0 }
	{ memd(r29+8) = r17:r16; r17 = r1; call fn00009500 }
	{ if (!cmp.eq(r16.new,00000000)) jump:t 0000C810; r16 = r0 }

l0000C810:
	{ if (p0.new) memw(r16+16) = 00000000; if (p0.new) jump:nt 0000C838; p0 = cmp.eq(r9,00000000) }

l0000C818:
	{ r1:r0 = combine(r17,00000080); call fn00009550 }
	{ memw(r16+16) = r0; if (!p0.new) jump:nt 0000C838; p0 = cmp.eq(r0,00000000) }

l0000C82C:
	{ r0 = r16; r16 = 00000000; call fn00009510 }
	{ jump 0000C850 }

l0000C838:
	{ r3 = memw(r18+12); r2 = memw(r18) }
	{ r4 = memw(r18+8); r7 = memw(r18+4) }
	{ memw(r16+12) = r3; memw(r16) = r2 }
	{ memw(r16+8) = r4; memw(r16+4) = r7 }
	{ memw(r16+20) = r17; memw(r16+24) = r17 }
	{ memw(r16+28) = r16 }

l0000C850:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }

l0000C854:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
	{ dealloc_return }

;; tensor_dup: 0000C85C
;;   Called from:
;;     000137C0 (in hexagon_nn_const_ctor)
tensor_dup proc
	{ allocframe(00000010); memd(r29+488) = r19:r18; r0 = 00000020; r18 = r0 }
	{ memd(r29+8) = r17:r16; r17 = memw(r18+24); call fn00009500 }
	{ if (!cmp.eq(r16.new,00000000)) jump:t 0000C87C; r16 = r0 }

l0000C87C:
	{ if (p0.new) jump:nt 0000C8A8; p0 = cmp.eq(r9,00000000) }

l0000C880:
	{ r1:r0 = combine(r17,00000080); call fn00009550 }
	{ memw(r16+16) = r0; if (p0.new) jump:nt 0000C89C; p0 = cmp.eq(r0,00000000) }

l0000C894:
	{ r2 = memw(r18+24); jump 0000C8B0 }

l0000C89C:
	{ r0 = r16; r16 = 00000000; call fn00009510 }
	{ jump 0000C8CC }

l0000C8A8:
	{ memw(r16+16) = 00000000; r2 = 00000000; r0 = 00000000 }

l0000C8B0:
	{ r4 = memw(r18+4); r3 = memw(r18) }
	{ r5 = memw(r18+12); r7 = memw(r18+8) }
	{ memw(r16+4) = r4; memw(r16) = r3 }
	{ memw(r16+12) = r5; memw(r16+8) = r7 }
	{ memw(r16+24) = r17; r1 = memw(r18+16) }
	{ memw(r16+28) = r16; memw(r16+20) = r17 }
	{ call fn00009560 }

l0000C8CC:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
	{ dealloc_return }

;; tensor_free: 0000C8D8
;;   Called from:
;;     0000B670 (in node_free_common)
;;     00013944 (in const_dtor)
tensor_free proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0 }
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0000C8EC; r0 = memw(r16+16) }

l0000C8EC:
	{ deallocframe; r17:r16 = memd(r29); jump 0000C97C; r0 = r8 }
0000C8F8                         00 00 00 00 00 00 00 00         ........

;; do_perfinfo_reset: 0000C900
;;   Called from:
;;     0000B1EC (in hexagon_nn_reset_perfinfo)
do_perfinfo_reset proc
	{ allocframe(00000000); r2 = r0 }
	{ if (cmp.eq(r3.new,00000000)) jump:nt 0000C91C; r3 = memw(r2); r5:r4 = combine(00000000,00000000) }

l0000C910:
	{ memd(r3+48) = r5:r4 }

l0000C914:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 0000C910; r3 = memw(r3+36) }

l0000C91C:
	{ memw(r2+52) = r1; r3 = and(r1,000000FF); r0 = 00000005 }

l0000C920:
	{ memw(r2+52) = r1; r3 = and(r1,000000FF) }

l0000C924:
	{ call fn00009640; r1 = setbit(r3,00000010) }
	{ dealloc_return; r0 = 00000000 }

;; do_perfinfo_get: 0000C930
;;   Called from:
;;     0000B190 (in hexagon_nn_get_perfinfo)
do_perfinfo_get proc
	{ r3 = memw(r0); r0 = 00000000; r4 = add(r1,00000008) }
	{ if (!p0.new) r0 = 00000000; if (p0.new) jump:nt 0000C96C; p0 = cmp.eq(r3,00000000) }

l0000C940:
	{  }
	{ memb(r4-2) = r5.new; r5 = memw(r3+28); r0 = add(r0,00000001) }
	{ memw(r4-4) = r1 }
	{ r7:r6 = memd(r3+48) }
	{ memd(r4) = r7:r6; r4 = add(r4,00000010) }
	{ if (!cmp.eq(r3.new,00000000)) jump:t 0000C940; r3 = memw(r3+36) }

l0000C96C:
	{ jumpr r31 }

l0000C970:
	{ jumpr r31; r0 = -00000001 }
0000C974             00 00 00 00 00 00 00 00                 ........    

l0000C97C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; find_last_consumer: 0000C980
;;   Called from:
;;     0000BC00 (in try_pad_bad_supernodes)
;;     0000BFC4 (in do_prepare)
;;     0000C198 (in do_prepare)
;;     0000C31C (in do_prepare)
;;     0000C390 (in do_prepare)
;;     0000C400 (in do_prepare)
;;     0000C474 (in do_prepare)
;;     0000C97C (in tensor_free)
find_last_consumer proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+40) = r19:r18; r19 = memw(r16); r18 = r2 }
	{ memd(r29+24) = r23:r22; memd(r29+32) = r21:r20; if (p0.new) r0 = add(r17,00000000); p0 = cmp.eq(r19,00000000) }
	{ memd(r29+16) = r25:r24; jump 0000CA2C; if (!p0) jump:nt 0000C9A8 }
0000C9A8                         15 40 00 78 9C 07 90 50         .@.x...P
0000C9B0 82 40 93 91 36 C0 02 24 96 C0 00 7C 18 40 60 70 .@..6..$...|.@`p
0000C9C0 63 C0 93 91 04 D7 03 F3 E4 7F 84 97 08 F4 02 20 c.............. 
0000C9D0 20 40 00 58 00 C0 78 70 03 D7 83 3A 10 52 03 F2  @.X..xp...:.R..
0000C9E0 01 52 03 F2 00 60 18 74 00 E0 93 74 12 C1 20 5C .R...`.t...t.. \
0000C9F0 10 C0 4D 10 92 07 B3 07 02 40 70 70 29 08 32 E8 ..M......@pp).2.
0000CA00 0B 08 13 E8 68 C0 00 5A 00 40 78 70 82 C0 93 91 ....h..Z.@xp....
0000CA10 17 41 17 B0 36 40 16 B0 D6 E2 32 22 00 51 13 F2 .A..6@....2".Q..
0000CA20 35 60 00 7E 33 41 93 91 CA E0 72 24             5`.~3A....r$    

l0000CA2C:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ dealloc_return; r25:r24 = memd(r29+16) }
0000CA3C                                     00 C0 00 7F             ....

;; find_first_consumer: 0000CA40
find_first_consumer proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+32) = r19:r18; r19 = memw(r17); r18 = r2 }
	{ memd(r29+24) = r21:r20 }
	{ memd(r29+16) = r23:r22; if (p0.new) jump:nt 0000CAC4; p0 = cmp.eq(r11,00000000) }

l0000CA5C:
	{ r20 = memw(r16+28); r21 = 00000000 }

l0000CA60:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000CAB4; r2 = memw(r19+16) }

l0000CA64:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000CAB8 }

l0000CA6C:
	{ r3 = memw(r19+12) }

l0000CA70:
	{ r4 = add(r3,r23) }
	{ if (!cmp.eq(r4.new,r20)) jump:t 0000CAA8; r4 = memw(r4-4) }

l0000CA80:
	{ if (!cmp.eq(r3.new,r18)) jump:t 0000CAAC }

l0000CA88:
	{ if (!p0.new) r16 = add(r19,00000000); jump 0000CAC8 }
0000CA90 82 07 B3 07 02 40 71 70 28 08 32 E8 0B 08 13 E8 .....@qp(.2.....
0000CAA0 1A C0 00 5A 82 C0 93 91                         ...Z....        

l0000CAA8:
	{ if (cmp.gtu(r2,r22.new)) jump:t 0000CA6C; r22 = add(r22,00000001); r23 = add(r23,00000008) }

l0000CAAC:
	{ if (cmp.gtu(r2,r22.new)) jump:t 0000CA70; r22 = add(r22,00000001) }

l0000CAB4:
	{ if (!cmp.eq(r19.new,00000000)) jump:t 0000CA60; r19 = memw(r19+36); if (p0.new) r21 = 00000001; p0 = cmp.eq(r19,r16) }

l0000CAB8:
	{ if (!cmp.eq(r19.new,00000000)) jump:t 0000CA64; r19 = memw(r19+36); if (p0.new) r21 = 00000001 }

l0000CAC4:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40); r0 = r16 }

l0000CAC8:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }

l0000CACC:
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ dealloc_return }

;; logmsg_function: 0000CAD4
logmsg_function proc
	{ allocframe(00000008); r3 = 00000000; r0 = add(PC,00018DC0) }
	{ r5 = add(r29,00000010); r1 = 0000003C; r4 = add(PC,00018DCB) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }

;; nn_os_workers_kill: 0000CB00
;;   Called from:
;;     0000B7B0 (in do_teardown)
;;     0000CAFC (in logmsg_function)
nn_os_workers_kill proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0 }
	{ r0 = memw(r16+68); call qurt_pipe_send.1.1_i64_0 }
	{ r0 = memw(r16+72); call qurt_pipe_send.1.1_i64_0 }
	{ r17:r16 = memd(r29); r0 = memw(r16+72) }
	{ deallocframe; jump qurt_pipe_send.1.1_i64_0 }

;; nn_os_work_for_vector: 0000CB28
;;   Called from:
;;     000118F4 (in avgpool_execute)
;;     000132B4 (in concat_execute)
;;     00015924 (in maxpool_execute)
;;     00019104 (in supernode_execute_hvx)
;;     00019114 (in supernode_execute_hvx)
nn_os_work_for_vector proc
	{ r0 = memw(r0+68); r3:r2 = combine(r2,r1); jump fn00009650 }

;; nn_os_work_for_scalar: 0000CB34
nn_os_work_for_scalar proc
	{ r0 = memw(r0+72); r3:r2 = combine(r2,r1); jump fn00009650 }

;; nn_os_vector_acquire: 0000CB40
;;   Called from:
;;     00014F04 (in matmul_check_ref)
;;     00019428 (in supernode_check_ref)
nn_os_vector_acquire proc
	{ allocframe(+00000000); call dspCV_hvx_lock.1.0_i8_2.1_i32_0 }
	{ dealloc_return; r0 = 00000000 }

;; nn_os_vector_release: 0000CB4C
;;   Called from:
;;     00014F54 (in matmul_check_ref)
;;     00019458 (in supernode_check_ref)
nn_os_vector_release proc
	{ jump fn00009660 }

;; nn_os_workers_spawn: 0000CB50
nn_os_workers_spawn proc
	{ allocframe(+000000E8) }
	{ memd(r29+224) = r17:r16; r2 = add(r29,00000090) }
	{ memd(r29+192) = r25:r24; memd(r29+200) = r23:r22; r16 = add(r2,00000008) }
	{ memd(r29+208) = r21:r20; memd(r29+216) = r19:r18; r0 = r16; r18 = r0 }
	{ memd(r29+184) = r27:r26; call qurt_sem_init_val.1.1_i16_0 }
	{ r19 = add(r29,00000004); r0 = 00000020 }
	{ memb(r19+8) = 00; memw(r29+144) = r18; r17 = add(r18,00000044); call fn00009500 }
	{ memw(r19+4) = FFFFFF84; memw(r29+4) = r0; r0 = r17; r1 = add(r29,00000004) }
	{ call fn00009670 }
	{ memb(r19+8) = 00000000; r0 = 00000020; r18 = add(r18,00000048); call fn00009500 }
	{ memw(r19+4) = FFFFFF84; memw(r29+4) = r0; r0 = r18; r1 = add(r29,00000004) }
	{ call fn00009670 }
	{ r3 = add(r29,000000A8); r2 = add(r29,00000010); r23 = 00000100; r22 = 000000FF }
	{ r27:r26 = combine(00002000,00000001); r25:r24 = combine(00000081,00000000) }
	{ r21 = add(r2,0000003C); r19 = setbit(r3,00000004) }

l0000CBE4:
	{ memb(r21-28) = r24; r3 = 0000FFFE; r20 = add(r21,FFFFFFE4) }
	{ memb(r21-11) = r22; memb(r21-8) = r24; r1 = add(PC,00018D26) }
	{ memuh(r21-10) = r23; memb(r21-12) = r24; r0 = r20; r2 = 00000010 }
	{ memuh(r21-6) = r3; memb(r21-7) = r22 }
	{ memw(r21) = 00000000; memw(r21-4) = r24 }
	{ call fn00009680 }
	{ memb(r21-13) = r24; r0 = 00002000; call fn00009500 }
	{ memw(r21) = r0; p0 = cmp.gt(r26,00000001); r2 = add(PC,00000060) }
	{ memuh(r21-10) = r25; memw(r21-4) = r27; r1:r0 = combine(r20,r19); r3 = add(r29,00000090) }
	{ r4 = mux(p0,r18,r17) }
	{ memb(r29+37) = r4.new; r4 = memw(r4); call fn00009690 }
	{ r0 = r16; r19 = add(r19,00000004); r25 = add(r25,00000001) }
	{ if (!cmp.eq(r26.new,00000004)) jump:t 0000CBE4; r26 = add(r26,00000001); r21 = add(r21,00000020) }

l0000CC80:
	{ r19:r18 = memd(r29+216); r17:r16 = memd(r29+224) }
	{ r23:r22 = memd(r29+200); r21:r20 = memd(r29+208) }
	{ r27:r26 = memd(r29+184); r25:r24 = memd(r29+192) }
	{ dealloc_return }

;; qurt_worker: 0000CC94
qurt_worker proc
	{ allocframe(00000010); memd(r29+496) = r17:r16 }
	{ r17 = memw(r0++#8); r16 = memw(r0+4) }
	{ memd(r29) = r19:r18; call qurt_sem_add.1.1_i32_1 }
	{ r0 = r16; call fn000096B0 }
	{ r19:r18 = combine(r1,r0) }
	{ if (p0.new) jump:nt 0000CCD8; p0 = cmp.eq(r10,00000000); r3:r2 = lsr(r19:r18,00000020) }

l0000CCBC:
	{ callr r18; r1:r0 = combine(r2,r17) }
	{ r0 = r16; call fn000096B0 }
	{ r19:r18 = combine(r1,r0) }
	{ if (!p0.new) jump:nt 0000CCBC; p0 = cmp.eq(r10,00000000); r3:r2 = lsr(r19:r18,00000020) }

l0000CCD8:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = 00000000 }
	{ deallocframe; jump 000096C0 }

;; nn_os_hvx_power_on: 0000CCE8
;;   Called from:
;;     0000ACE8 (in do_execute)
;;     0000BF1C (in do_prepare)
;;     00014EFC (in matmul_check_ref)
nn_os_hvx_power_on proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0; call fn000096D0 }
	{ if (!p0.new) r2 = add(r16,00000000); if (p0.new) jump:nt 0000CD00; p0 = cmp.eq(r0,00000000) }

l0000CCFC:
	{ call errlog_function }

l0000CD00:
	{ dealloc_return; r17:r16 = memd(r29) }

;; errlog_function: 0000CD04
;;   Called from:
;;     0000CCFC (in nn_os_hvx_power_on)
errlog_function proc
	{ allocframe(00000008); r3 = 00000000; r0 = add(PC,00018C20) }
	{ r5 = add(r29,00000010); r1 = 000000C2; r4 = add(PC,00018C28) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

;; nn_os_hvx_power_off: 0000CD30
;;   Called from:
;;     0000AD6C (in do_execute)
;;     0000C714 (in do_prepare)
;;     0000CD20 (in errlog_function)
;;     00014F5C (in matmul_check_ref)
nn_os_hvx_power_off proc
	{ jump 000096E0 }

;; nn_os_get_perfcount: 0000CD34
;;   Called from:
;;     0000AD10 (in do_execute)
;;     0000AD10 (in do_execute)
nn_os_get_perfcount proc
	{ allocframe(+00000008) }
	{ memd(r29) = r17:r16; r2 = memw(r0+20) }
	{ nop; if (p0.new) jump:nt 0000CD54; p0 = cmp.eq(r2,00000005) }

l0000CD44:
	{ if (!p0.new) r0 = 00000000; if (!p0.new) jump:nt 0000CD60; p0 = cmp.eq(r2,00000000) }

l0000CD4C:
	{ deallocframe; r17:r16 = memd(r29); jump fn00009530 }

l0000CD54:
	{ call fn000096F0 }
	{ deallocframe; r17:r16 = memd(r29); jump 00009700 }

l0000CD60:
	{ call fn00009710 }
	{ r16 = r0; r0 = 00000001; call fn00009710 }
	{ r17 = r0 }
	{ dealloc_return; r17:r16 = memd(r29); r1:r0 = combine(r17,r16) }
0000CD78                         00 40 00 7F 00 C0 00 7F         .@......

;; nn_os_vector_workers_acquire: 0000CD80
;;   Called from:
;;     0000ACF0 (in do_execute)
nn_os_vector_workers_acquire proc
	{ allocframe(00000010); memd(r29+488) = r19:r18; r18 = add(PC,0001E390) }
	{ memd(r29+8) = r17:r16; r16 = r0 }
	{ r17 = memw(r18-176) }
	{ r0 = r17; call qurt_sem_init_val.1.1_i16_0 }
	{ r18 = memw(r18-160) }
	{ r0 = r18; call qurt_sem_init_val.1.1_i16_0; r19 = add(PC,0001E8F8) }
	{ r0 = memw(r16+68); r3 = add(r19,00000004); r2 = add(PC,00000030) }
	{ call fn00009650 }
	{ r0 = r17; call fn000096A0 }
	{ r0 = r18; call qurt_sem_add.1.1_i32_1 }
	{ call dspCV_hvx_lock.1.0_i8_2.1_i32_0 }
	{ memw(r19) = 00000000; r17:r16 = memd(r29+8) }
	{ dealloc_return; r19:r18 = memd(r29) }

;; worker_acquire: 0000CDE8
worker_acquire proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r1; call dspCV_hvx_lock.1.0_i8_2.1_i32_0 }
	{ memw(r16) = 00000000; r17 = add(PC,0001E31C) }
	{ r0 = memw(r17-176); call qurt_sem_add.1.1_i32_1 }
	{ r17:r16 = memd(r29); r0 = memw(r17-160) }
	{ deallocframe; jump fn000096A0 }

;; nn_os_vector_workers_release: 0000CE20
;;   Called from:
;;     0000AD64 (in do_execute)
nn_os_vector_workers_release proc
	{ allocframe(00000010); memd(r29+488) = r19:r18; r18 = add(PC,0001E2F0) }
	{ memd(r29+8) = r17:r16; r16 = r0 }
	{ r17 = memw(r18-176) }
	{ r0 = r17; call qurt_sem_init_val.1.1_i16_0 }
	{ r18 = memw(r18-160) }
	{ r0 = r18; call qurt_sem_init_val.1.1_i16_0 }
	{ r0 = memw(r16+68); r4 = add(PC,0001E850) }
	{ r3 = add(r4,00000004); call fn00009650; r2 = add(PC,0000002C) }
	{ r0 = r17; call fn000096A0 }
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r18; call qurt_sem_add.1.1_i32_1 }
	{ deallocframe; jump fn00009660 }

;; worker_release: 0000CE88
worker_release proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; call fn00009660 }
	{ r16 = add(PC,0001E280) }
	{ r0 = memw(r16-176); call qurt_sem_add.1.1_i32_1 }
	{ r17:r16 = memd(r29); r0 = memw(r16-160) }
	{ deallocframe; jump fn000096A0 }

;; dspCV_hvx_lock.1.0_i8_2.1_i32_0: 0000CEB8
;;   Called from:
;;     0000CB40 (in nn_os_vector_acquire)
;;     0000CDDC (in nn_os_vector_workers_acquire)
;;     0000CDE8 (in worker_acquire)
dspCV_hvx_lock.1.0_i8_2.1_i32_0 proc
	{ r1:r0 = combine(00000000,00000002); jump 00009720 }

;; qurt_sem_add.1.1_i32_1: 0000CEC0
;;   Called from:
;;     0000CCA0 (in qurt_worker)
;;     0000CDD4 (in nn_os_vector_workers_acquire)
;;     0000CE00 (in worker_acquire)
;;     0000CE74 (in nn_os_vector_workers_release)
;;     0000CE98 (in worker_release)
qurt_sem_add.1.1_i32_1 proc
	{ jump fn0000CF80; r1 = FFFFC841 }

;; qurt_sem_init_val.1.1_i16_0: 0000CEC8
;;   Called from:
;;     0000CB70 (in nn_os_workers_spawn)
;;     0000CD98 (in nn_os_vector_workers_acquire)
;;     0000CDA8 (in nn_os_vector_workers_acquire)
;;     0000CE38 (in nn_os_vector_workers_release)
;;     0000CE48 (in nn_os_vector_workers_release)
qurt_sem_init_val.1.1_i16_0 proc
	{ jump fn0000CFA8; r0 = FFFFC840 }

;; qurt_pipe_send.1.1_i64_0: 0000CED0
;;   Called from:
;;     0000CB08 (in nn_os_workers_kill)
;;     0000CB10 (in nn_os_workers_kill)
;;     0000CB20 (in nn_os_workers_kill)
qurt_pipe_send.1.1_i64_0 proc
	{ r3:r2 = combine(00000000,00000000); jump fn00009650 }
0000CED8                         00 00 00 00 00 00 00 00         ........

;; transpack: 0000CEE0
;;   Called from:
;;     00013EA0 (in conv2d_execute_hvx)
;;     00014F50 (in matmul_check_ref)
;;     00019454 (in supernode_check_ref)
transpack proc
	{ if (!p0.new) jump:nt 0000CF74; p0 = cmp.gt(r2,00000000); r4 = asl(r1,00000003) }

l0000CEE8:
	{ r5 = 00000000; r3 = add(r3,00000003) }

l0000CEEC:
	{ r9 = add(r1,00000003); r8 = add(r5,r2); if (!p0.new) jump:nt 0000CF68; p0 = cmp.gt(r1,00000000) }

l0000CEF8:
	{ r7:r6 = combine(00000000,r3); r9 = lsr(r9,00000002) }
	{ loop1(0000CF04,r9) }
	{ r9 = r6; r13 = add(r7,00000003); r12 = add(r7,00000002); r14 = mpyi(r7,r2) }
	{ r13:r12 = combine(r0,r0); r10 = mpyi(r13,r2); r28 = mpyi(r12,r2) }
	{ r15:r14 = combine(r0,r0); r13 += add(r14,r5); r12 += add(r8,r14) }
	{ r15 += add(r10,r5); r14 += add(r28,r5) }
	{ loop0(0000CF38,00000010) }
	{ memb(r9-3) = r28.new; r28 = memb(r13++#1) }
	{ memb(r9-2) = r10 }
	{ r28 = memb(r14++#1) }
	{ memb(r9-1) = r28 }
	{ r10 = memb(r15++#1) }
	{ memb(r9) = r10; r9 = add(r9,00000004) }
	{ nop; r6 = add(r6,00000080); r7 = add(r7,00000004) }

l0000CF68:
	{ if (cmp.gtu(r2,r5.new)) jump:t 0000CEEC; r5 = add(r5,00000020); r3 = addasl(r3,r4,00000002) }

l0000CF74:
	{ jumpr r31 }

;; pad2d: 0000CF78
;;   Called from:
;;     0000CF68 (in transpack)
;;     00013E88 (in conv2d_execute_hvx)
;;     00014F3C (in matmul_check_ref)
;;     0001943C (in supernode_check_ref)
pad2d proc
	{ allocframe(00000030); memd(r29+488) = r19:r18 }
	{ memd(r29+40) = r17:r16; r19 = r4; r17:r16 = combine(r5,r3) }

;; fn0000CF80: 0000CF80
;;   Called from:
;;     0000CEC0 (in qurt_sem_add.1.1_i32_1)
;;     0000CF7C (in pad2d)
fn0000CF80 proc
	{ memd(r29+40) = r17:r16; r19 = r4 }
	{ memd(r29+8) = r25:r24; memd(r29+24) = r21:r20 }
	{ memd(r29+16) = r23:r22; r22 = r2; r21:r20 = combine(r0,r1) }
	{ memd(r29) = r27:r26; r18 = memw(r29+56); if (p0.new) r23 = add(r16,00000000); p0 = cmp.gt(r20,00000000) }
	{ if (p0) r24 = sub(r17,r22); if (p0) r25 = add(r20,00000000); if (!p0) jump:nt fn0000CFE0 }

;; fn0000CFA8: 0000CFA8
;;   Called from:
;;     0000CEC8 (in qurt_sem_init_val.1.1_i16_0)
fn0000CFA8 proc
	{ if (p0) r24 = sub(r17,r22); if (p0) r25 = add(r20,00000000) }

;; fn0000CFB0: 0000CFB0
;;   Called from:
;;     0000CFA4 (in fn0000CF80)
;;     0000CFA8 (in fn0000CFA8)
;;     0000CFA8 (in fn0000CFA8)
;;     0000CFA8 (in fn0000CFA8)
;;     0000CFA8 (in fn0000CFA8)
fn0000CFB0 proc
	{ r26 = mpyi(r17,r20) }

l0000CFB4:
	{ r1:r0 = combine(r21,r23); r2 = r22; call vmemcpy_asm }
	{ r21 = add(r21,r22); r1 = r18; r2 = r24; r0 = add(r23,r22) }
	{ r23 = add(r23,r17); call vmemset_asm }
	{ if (!cmp.eq(r25.new,00000000)) jump:t 0000CFB4; r25 = add(r25,FFFFFFFF) }

;; fn0000CFE0: 0000CFE0
;;   Called from:
;;     0000CFA4 (in fn0000CF80)
;;     0000CFD4 (in fn0000CFB0)
fn0000CFE0 proc
	{ if (!cmp.gt(r19.new,00000000)) jump:nt 0000CFFC; r19 = sub(r19,r20) }

l0000CFE8:
	{ r16 = add(r16,r17); r2 = r17; r1:r0 = combine(r18,r16); call vmemset_asm }

l0000CFEC:
	{ r16 = add(r16,r17); r2 = r17; r1:r0 = combine(r18,r16) }

l0000CFF4:
	{ if (!cmp.eq(r19.new,00000000)) jump:t 0000CFE8; r19 = add(r19,FFFFFFFF) }

l0000CFFC:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }

l0000D000:
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ r27:r26 = memd(r29); r25:r24 = memd(r29+8) }
	{ dealloc_return }

;; unpad2d: 0000D010
;;   Called from:
;;     00013FC0 (in conv2d_execute_hvx)
unpad2d proc
	{ allocframe(00000018); memd(r29+496) = r17:r16 }
	{ memd(r29) = r21:r20; memd(r29+8) = r19:r18; r19:r18 = combine(r5,r3); r17:r16 = combine(r2,r0) }
	{ if (!cmp.gt(r20.new,00000000)) jump:nt 0000D048; r20 = r4; r21 = asl(r19,00000002) }

l0000D02C:
	{ r1:r0 = combine(r16,r18); r2 = r21; call vmemcpy_asm; r16 = addasl(r16,r17,00000002) }

l0000D030:
	{ r1:r0 = combine(r16,r18); r2 = r21; call fn00022964 }

l0000D03C:
	{ if (!cmp.eq(r20.new,00000000)) jump:t 0000D02C; r20 = add(r20,FFFFFFFF); r18 = addasl(r18,r19,00000002) }

l0000D048:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }

l0000D04C:
	{ dealloc_return; r21:r20 = memd(r29) }

;; unpad2d_bytes: 0000D050
;;   Called from:
;;     0001A744 (in supernode_execute_hvx_slice)
unpad2d_bytes proc
	{ allocframe(00000018); memd(r29+496) = r17:r16 }
	{ memd(r29) = r21:r20; memd(r29+8) = r19:r18; r19:r18 = combine(r5,r3); r17:r16 = combine(r2,r0) }
	{ if (!cmp.gt(r20.new,00000000)) jump:nt 0000D080; r20 = r4 }

l0000D068:
	{ r16 = add(r16,r17); r2 = r19; r1:r0 = combine(r16,r18); call vmemcpy_asm }

l0000D06C:
	{ r16 = add(r16,r17); r2 = r19; r1:r0 = combine(r16,r18) }

l0000D074:
	{ if (!cmp.eq(r20.new,00000000)) jump:t 0000D068; r20 = add(r20,FFFFFFFF); r18 = add(r18,r19) }

l0000D080:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }

l0000D084:
	{ dealloc_return; r21:r20 = memd(r29) }
0000D088                         00 00 00 00 00 00 00 00         ........
0000D090 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemm_cn: 0000D0A0
gemm_cn proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; if (!p0.new) jump:nt 0000D12C; p0 = cmp.gt(r5,00000000) }

l0000D0A8:
	{ r7 = memd(r29+20); r6 = memd(r29+16); r8 = 00000000 }

l0000D0B0:
	{ r12 = r8; if (!p0.new) jump:nt 0000D124; p0 = cmp.gt(r6,00000000); r13 = mpyi(r8,r6) }

l0000D0BC:
	{ r9 = r2; loop1(0000D0CC,r6) }
	{ r12 = add(r0,mpyi(r12,r7)) }
	{ r13 = addasl(r4,r13,00000002) }
	{ r14 = 00000000; r11 = add(r7,FFFFFFFF); if (!p0.new) jump:nt 0000D118; p0 = cmp.gt(r7,00000000) }

l0000D0D8:
	{ r10 = memb(r9); p0 = cmp.gtu(r7,00000001); r28 = add(r9,r6); r15:r14 = combine(r12,00000000) }
	{ r16 = memb(r15++#1); r10 = add(r10,r3); loop0(0000D0FC,r11) }
	{ r11 = add(r16,r1); if (!p0) jump:nt 0000D114 }

l0000D0FC:
	{ r17 = memb(r15++#1); r16 = memb(r28); r28 = add(r28,r6); r14 += mpyi(r10,r11) }
	{ r11 = add(r17,r1); r10 = add(r16,r3) }

l0000D114:
	{ r14 += mpyi(r10,r11) }

l0000D118:
	{ memw(r13++#4) = r14; nop; r9 = add(r9,00000001) }

l0000D124:
	{ if (!cmp.eq(r8.new,r5)) jump:t 0000D0B0; r8 = add(r8,00000001) }

l0000D12C:
	{ dealloc_return; r17:r16 = memd(r29) }

;; gemm_co: 0000D130
;;   Called from:
;;     0000D124 (in gemm_cn)
gemm_co proc
	{ allocframe(+00000018) }
	{ memd(r29+16) = r17:r16; r6 = memd(r29+32) }
	{ memd(r29) = r21:r20; memd(r29+8) = r19:r18; if (p0.new) r12 = 00000000; p0 = cmp.gt(r6,00000000) }
	{ if (p0) r7 = memw(r29+36); if (p0) r9 = memw(r29+40); if (!p0) jump:nt 0000D2D8 }

l0000D150:
	{ r8 = memw(r29+44); p0 = cmp.gt(r5,00000000); r13 = mpyi(r3,r1) }
	{ r13 = mpyi(r13,r7) }

l0000D160:
	{ r15:r14 = combine(00000000,r2); p1 = !cmp.eq(r12,00000000) }
	{ if (!p1.new) r14 = 00000000; if (p1.new) jump:nt 0000D1B0; p1 = or(p1,!p0) }

l0000D174:
	{ loop1(0000D178,r5) }
	{  }
	{ memw(r30+r14<<#2) = r13; if (p1.new) r10 = add(r14,00000000) }
	{ r28 = r13; r15 = addasl(r9,r14,00000002); loop0(0000D194,r7) }
	{ r10 = add(r0,mpyi(r10,r7)) }
	{ r11 = memb(r10++#1) }
	{ r28 += mpyi(r11,r3) }
	{ nop; nop }
	{ r15:r14 = combine(00000000,r2) }

l0000D1B0:
	{ if (p1.new) r10 = add(r14,00000000); p1 = cmp.gt(r7,00000000); r28 = 00000000; r11 = add(r15,r12) }
	{ memw(r14+r11<<#2) = r28; if (!p1) jump:nt 0000D1E0 }

l0000D1C8:
	{ r11 = addasl(r8,r11,00000002); loop0(0000D1D0,r7) }
	{ r16 = memb(r10); r10 = add(r10,r6) }
	{ r28 += mpyi(r16,r1) }

l0000D1E0:
	{ r14 = add(r14,00000001); r15 = add(r15,00000001) }

l0000D1E4:
	{ r14 = add(r14,00000001) }

l0000D1E8:
	{ if (p2.new) r14 = 00000000; p2 = cmp.eq(r15,00000020); if (!p0) jump:nt 0000D2CC; if (!p2.new) jump:nt 0000D1B0 }

l0000D1F8:
	{ r28 = 00000000; r15 = r2; r11 = r14; r10 = r12 }
	{ r11 = add(r0,mpyi(r11,r7)); r10 += mpyi(r14,r6) }
	{ if (p1) r18 = add(r11,00000000); if (p1) r17 = add(r15,00000000); r16 = 00000000; r19 = add(r10,r28) }
	{ memw(r15+r19<<#2) = r16; if (!p1) jump:nt 0000D244 }
	{ r19 = addasl(r4,r19,00000002); loop0(0000D230,r7) }
	{ r21 = memb(r18++#1); r20 = memb(r17); r17 = add(r17,r6) }
	{ r16 += mpyi(r20,r21) }
	{ r15 = add(r15,00000001) }
	{ p2 = cmp.eq(r28,00000020); if (!p2.new) jump:nt 0000D210 }
	{ if (!cmp.eq(r14.new,r5)) jump:t 0000D1F8; r14 = add(r14,00000001) }
	{ r15 = 00000000 }
	{ r11 = r14; r10 = r12; r28 = addasl(r9,r15,00000002); loop0(0000D2A0,0000000F) }
	{ r16 = memw(r28); r18 = memw(r11++#4); r10 += mpyi(r15,r6) }
	{ r10 = addasl(r4,r10,00000002) }
	{ r17 = memw(r10) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r19 = memw(r10+4) }
	{ r17 += add(r16,r18) }
	{ r16 = memw(r28); r18 = memw(r11++#4) }
	{ r15 = add(r15,00000001) }
	{ p1 = cmp.eq(r15,r5); if (!p1.new) jump:nt 0000D264; r19 += add(r16,r18) }

l0000D2CC:
	{ if (cmp.gtu(r6,r12.new)) jump:t 0000D160; r12 = add(r12,00000020); r2 = add(r2,00000020) }

l0000D2D8:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }

l0000D2DC:
	{ dealloc_return; r21:r20 = memd(r29) }

;; gemsuma_cn: 0000D2E0
gemsuma_cn proc
	{ r6 = 00000000; r12 = add(r2,FFFFFFFF); if (!p0.new) jump:nt 0000D334; p0 = cmp.gt(r1,00000000) }

l0000D2EC:
	{ loop1(0000D2F0,r1) }
	{ r7 = 00000000; r8 = r6; if (!p0.new) jump:nt 0000D320; p0 = cmp.gt(r2,00000000) }

l0000D2FC:
	{ p0 = cmp.gtu(r2,00000001); r7 = 00000000; loop0(0000D314,r12) }
	{ r8 = add(r0,mpyi(r8,r2)) }
	{ r9 = memb(r8++#1); if (!p0) jump:nt 0000D31C }

l0000D314:
	{ r9 = memb(r8++#1); r7 = add(r9,r7) }

l0000D31C:
	{ r7 = add(r9,r7) }

l0000D320:
	{ r6 = add(r6,00000001); r8 = r5 }
	{ nop; r8 += mpyi(r7,r4) }

l0000D334:
	{ jumpr r31 }
0000D338                         00 40 00 7F 00 C0 00 7F         .@......

;; gemsumb_cn: 0000D340
gemsumb_cn proc
	{ r6 = r0; r2 = 00000000; p0 = cmp.gt(r3,00000000); r5 = 00000000 }
	{ loop1(0000D350,00000010) }
	{ memw(r30+r5<<#2) = r2; if (p0) r12 = add(r3,00000003); if (!p0) jump:nt 0000D3B8 }

l0000D35C:
	{ r9:r8 = combine(00000000,r6); r7 = addasl(r1,r5,00000002) }
	{ r12 = lsr(r12,00000002) }
	{ loop0(0000D36C,r12) }
	{ r12 = memb(r8-1) }
	{ r9 += mpyi(r12,r4) }
	{ r0 = memb(r8) }
	{ r12 = mpyi(r0,r4); r13 += mpyi(r0,r4) }
	{ r14 = mpyi(r14,r4) }
	{ r15 = r14 }
	{ r15 += add(r12,r9) }
	{ r9 = memb(r8+2) }
	{ r9 = mpyi(r9,r4) }
	{ r9 += add(r13,r14) }
	{ memw(r7) = r9; nop }

l0000D3B8:
	{ r5 = r5; r6 = add(r6,00000004); nop; nop }
	{ jumpr r31 }

;; gemmpybbw_cn: 0000D3C8
gemmpybbw_cn proc
	{ if (!p0.new) jumpr:nt r31; p0 = cmp.gt(r3,00000000) }
0000D3D0 16 31 07 28 08 41 21 69 09 44 07 ED 0C 60 07 73 .1.(.A!i.D...`.s
0000D3E0 08 C0 66 70 00 CD 05 E3 0F 49 0C F3 0E 40 00 78 ..fp.....I...@.x
0000D3F0 00 40 45 75 6A E0 05 74 32 40 20 5C 1C 40 08 74 .@Euj..t2@ \.@.t
0000D400 01 40 0D 74 0E EF 82 3B 4F C2 0F C4 2A C2 0A 8C .@.t...;O...*...
0000D410 08 C0 0A 60 0A 40 21 91 EB FF 3C 97 0E 4A 0B EF ...`.@!...<..J..
0000D420 00 D2 AF A1 0A 40 3C 91 2B C0 21 91 0E 4B 0A EF .....@<.+.!..K..
0000D430 00 D2 AF A1 2A 40 3C 91 4B C0 21 91 0E 4B 0A EF ....*@<.K.!..K..
0000D440 00 D2 AF A1 1C 50 1C B0 81 40 01 B0 4A 40 3C 91 .....P...@..J@<.
0000D450 6B C0 21 91 0E 8B 0A EF 00 D2 AF A1 2C 40 0C B0 k.!.........,@..
0000D460 88 80 08 B0 00 C0 00 7F 27 C0 07 B0 B4 C3 77 14 ........'.....w.
0000D470 00 C0 9F 52                                     ...R            

;; gemaddvvm_cn: 0000D474
gemaddvvm_cn proc
	{ r5 = 00000000; if (!p0.new) jump:nt 0000D4C8; p0 = cmp.gt(r3,00000000); loop1(0000D480,r3) }

l0000D480:
	{ r7 = r1; r6 = addasl(r0,r5,00000002); r3 = mpyi(r5,r4) }
	{ r8 = memw(r6); r12 = memw(r7++#4); r3 = addasl(r2,r3,00000002); loop0(0000D4A0,0000000F) }
	{ r9 = memw(r3) }
	{ r13 = memw(r3+4) }
	{ r9 += add(r12,r8) }
	{ r12 = memw(r7++#4); r8 = memw(r6) }
	{ r5 = add(r5,00000001) }
	{ nop; r13 += add(r12,r8) }

l0000D4C8:
	{ jumpr r31 }

;; gemm_asm: 0000D4CC
;;   Called from:
;;     0000D4BC (in gemaddvvm_cn)
;;     00013FAC (in conv2d_execute_hvx)
gemm_asm proc
	{ allocframe(00000058); memd(r29+496) = r17:r16 }
	{ r16 = memd(r29+96); r17 = r0 }
	{ memd(r29+64) = r21:r20; memd(r29+72) = r19:r18; p0 = cmp.gt(r16,00000000); r19 = r2 }
	{ memw(r29+36) = r1; memd(r29+56) = r23:r22 }
	{ memd(r29+40) = r27:r26; memd(r29+48) = r25:r24 }
	{ memw(r29+32) = r4; memw(r29+12) = r3 }
	{ if (p0) r23 = memw(r29+104); if (p0) r25 = memw(r29+112); if (!p0) jump:nt 0000D5A4 }

l0000D4FC:
	{ r4 = memd(r29+12); r3 = memd(r29+36); r21 = 00000000; r18 = 00000000 }
	{ r2 = memd(r29+100); r22 = memd(r29+108) }
	{ r4 = memw(r29+120); r26 = combine(r25.l,r2.l); r7 = mpyi(r22,r2); r3 = mpyi(r4,r3) }
	{ memw(r29+28) = r4; r4 = memd(r29+124); r2 = mpyi(r3,r2) }
	{ memw(r29+24) = r4; r24 = memw(r29+116); r4 = asl(r22,00000002) }
	{ memw(r29+16) = r4; memw(r29+20) = r7 }
	{ memw(r29+8) = r2 }

l0000D538:
	{ r3:r2 = combine(r24,r26); r1:r0 = combine(r23,r17); if (!p0.new) jump:nt 0000D54C; p0 = cmp.eq(r13,00000000) }

l0000D544:
	{ r4 = memd(r29+12); r5 = memd(r29+8) }
	{ call gemsuma_asm }

l0000D54C:
	{ r3 = memd(r29+36); r2 = memd(r29+28) }
	{ r2 = r25; r20 = add(r2,r18) }
	{ r1:r0 = combine(r20,r19); call gemsumb_asm }
	{ r2 = memw(r29+32); r1:r0 = combine(r19,r17); r5:r4 = combine(r26,r16) }
	{ r27 = add(r2,r18) }
	{ r3:r2 = combine(r23,r27); call gemmpybbw_asm }
	{ r5 = memd(r29+24); r4 = r16; r1:r0 = combine(r20,r24); r3:r2 = combine(r23,r27) }
	{ memb(r29) = r6.new; r6 = cmp.eq(r21,00000000); call gemaddvvm_asm }
	{ r7 = memd(r29+20); r18 = add(r18,r2) }
	{ if (cmp.gtu(r16,r21.new)) jump:t 0000D538; r21 = add(r21,r22); r19 = add(r19,r7) }

l0000D5A4:
	{ r19:r18 = memd(r29+72); r17:r16 = memd(r29+80) }

l0000D5A8:
	{ r23:r22 = memd(r29+56); r21:r20 = memd(r29+64) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+48) }
	{ dealloc_return }

;; im2col_co: 0000D5B8
;;   Called from:
;;     00013F7C (in conv2d_execute_hvx)
im2col_co proc
	{ allocframe(+000000C0); r8 = 00000020 }
	{ memd(r29+144) = r27:r26; r7 = memw(r29+204) }
	{ r9 = memw(r29+216); r12 = memw(r29+200) }
	{ memd(r29+176) = r19:r18; memw(r29+120) = r5; p0 = cmp.gt(r9,00000000); r26 = mpyi(r7,r3) }
	{ memd(r29+184) = r17:r16; r16 = r4; r6 = add(0000000F,mpyi(r26,r12)) }
	{ memd(r29+152) = r25:r24; r4 = memw(r29+212) }
	{ memd(r29+160) = r23:r22; memd(r29+168) = r21:r20; r5 = and(r6,FFFFFFF0) }
	{ memw(r29+100) = r26; memw(r29+36) = r9; if (p0) r9 = add(r2,00000000); r19 = max(r5,r8) }
	{ memw(r29+8) = r4; memw(r29+56) = r0 }
	{ memw(r29+32) = r19; if (p0) r0 = memw(r29+36); if (!p0) jump:nt 0000D954 }

l0000D614:
	{ r13 = memw(r29+220); r24 = memw(r29+208); r4 = mpyi(r26,r12); r6 = mpyi(r7,r12) }
	{ memw(r29+112) = r12; r12 = memw(r29+8); r14 = sub(r19,r4); r2 = add(0000000F,mpyi(r6,r3)) }
	{ memw(r29+68) = r3; memw(r29+4) = r4; p1 = cmp.gt(r14,00000000); p0 = cmp.gt(r12,00000000) }
	{ r6 = memw(r29+224); r2 = and(r2,FFFFFFF0); p0 = not(p0); r4 = mpyi(r3,r9) }
	{ memw(r29+16) = r13; memw(r29+92) = r6; p0 = or(p0,!p1); r2 = max(r2,r8) }
	{ memw(r29+28) = r14; r3 = sub(r7,r9); r23 = mpyi(r19,r0); r21 = mpyi(r4,r24) }
	{ memw(r29+52) = r9; memw(r29+12) = r3; r22 = mpyi(r0,r2) }
	{ memw(r29+48) = r13; r3 = add(r24,FFFFFFFF) }
	{ memw(r29+80) = r3; memw(r29+20) = r7; r7 = add(r24,r1); r5 = sub(r7,r13) }
	{ memw(r29+96) = r1; r6 = 00000000; r4 = sub(r5,r9); r1 += add(r24,FFFFFFFF) }
	{ memw(r29+88) = r7; memw(r29+72) = r1; r1 = p0; r3 = mpyi(r12,r24) }
	{ memw(r29+24) = r1; memw(r29+84) = r3 }
	{ memw(r29+44) = r4 }

l0000D6B0:
	{ memw(r29+40) = r6; r7 = memd(r29+112); r8 = 00000000; r2 = mpyi(r6,r19) }
	{ memw(r29+116) = r2; p0 = cmp.gt(r7,00000000) }
	{ if (!p0) jump:nt 0000D904 }

l0000D6C8:
	{ r2 = memd(r29+40); r3 = memd(r29+44) }
	{ r4 = memd(r29+48); r5 = memd(r29+20); r2 = mpyi(r2,r24); r3 = max(r3,r8) }
	{ r7 = memd(r29+68); r1 = memd(r29+12); r4 = max(r4,r8) }
	{ r5 = memw(r29+16); r3 = sub(r5,r3); r4 = mpyi(r7,r4) }
	{ r2 = sub(r2,r5); r3 = mpyi(r7,r3) }
	{ r1 = memw(r29+120); r6 = sub(00000000,r2); r0 = add(r1,r2) }
	{ r4 = add(r1,r4); r8 = 00000000; r6 = max(r6,r8); r5 = max(r0,r8) }
	{ memb(r29+16) = r2.new; r27 = mpyi(r6,r7); r2 = max(r2,r8) }
	{ memw(r29+104) = r27; memw(r29+60) = r4; r2 = add(r1,r3) }
	{ memw(r29+108) = r0; memw(r29+76) = r2; r2 = add(r5,r6) }
	{ r2 = mpyi(r2,r7) }
	{ r20 = sub(r26,r2) }

l0000D73C:
	{ r2 = memw(r29+92) }
	{ r4 = sub(r8,r2); r2 = mpyi(r8,r26) }
	{ if (p0.new) r3 = memw(r29+116); if (p0.new) r25 = add(r21,00000000); if (p0.new) jump:nt 0000D764; p0 = cmp.gt(r4,-00000001) }

l0000D754:
	{ memw(r29+128) = r8; r2 = add(r2,r3) }
	{ memw(r29+136) = r2; jump 0000D7A0 }

l0000D764:
	{ r18 = memd(r29+120); r3 = memd(r29+116); r17 = r4; r25 = r21 }
	{ memw(r29+128) = r8; r4 = add(r2,r3); r19 = r4; r18 += add(r2,r3) }
	{ memw(r29+136) = r4 }

l0000D784:
	{ r18 = add(r18,r23); r1:r0 = combine(r16,r18); r2 = r26; call vmemset_asm }
	{ if (!cmp.gt(r17.new,-00000001)) jump:t 0000D784; r17 = add(r17,r24) }

l0000D7A0:
	{ memw(r29+124) = r4; r7 = memd(r29+84); r1 = r24; r2 = 00000000 }
	{ r2 = memw(r29+96); r19 = add(r4,r7); r17 = max(r4,r2) }
	{ r0 = sub(r17,r4); r26 = min(r2,r19) }
	{ call fn00009750 }
	{ memw(r29+132) = r0; p0 = cmp.gt(r27,00000000); if (!p0.new) jump:nt 0000D804 }

l0000D7D0:
	{ if (p0.new) r2 = memw(r29-124); if (p0.new) r3 = memw(r29-120); p0 = cmp.gt(r26,r17); if (!p0.new) jump:nt 0000D804 }

l0000D7E0:
	{ r18 = memd(r29+120); r21 = r17 }
	{ r2 = mpyi(r2,r23) }
	{ r18 += add(r3,r2) }

l0000D7EC:
	{ r18 = add(r18,r23); r1:r0 = combine(r16,r18); r2 = r27; call vmemset_asm }
	{ if (cmp.gtu(r26,r21.new)) jump:t 0000D7EC; r21 = add(r21,r24) }

l0000D804:
	{ r27 = r25; r1 = r24; if (!p0.new) jump:nt 0000D870; p0 = cmp.gt(r12,00000000) }

l0000D808:
	{ r27 = r25; r1 = r24 }

l0000D810:
	{ r2 = memw(r29+80) }
	{ r2 = sub(r2,r17) }
	{ r0 = add(r2,r26); call fn00009750 }
	{ if (!cmp.gt(r21.new,00000000)) jump:nt 0000D870; r21 = r0 }

l0000D82C:
	{ r18 = memw(r29+64) }
	{ r25 = memw(r29+60); r3 = memw(r29+52); r2 = mpyi(r22,r2) }
	{ r7 = memw(r29+68); r6 = memw(r29+136) }
	{ r3 = memw(r29+56); r25 += add(r6,r2); r18 += mpyi(r17,r3) }
	{ r18 = add(r3,mpyi(r18,r7)) }

l0000D854:
	{ r25 = add(r25,r22); r1:r0 = combine(r18,r25); r2 = r20; call vmemcpy_asm }
	{ if (!cmp.eq(r21.new,00000000)) jump:t 0000D854; r21 = add(r21,FFFFFFFF); r18 = add(r18,r27) }

l0000D870:
	{ if (!cmp.gt(r21.new,00000000)) jump:nt 0000D8A8; r21 = memw(r29+108) }

l0000D874:
	{ if (!cmp.gt(r21.new,00000000)) jump:nt 0000D8AC }

l0000D87C:
	{ if (p0.new) r2 = memw(r29-124); if (p0.new) r18 = memw(r29+76); p0 = cmp.gt(r26,r17) }
	{ r3 = memw(r29+136) }
	{ r2 = mpyi(r22,r2) }
	{ r18 += add(r3,r2) }

l0000D894:
	{ r18 = add(r18,r22); r2 = r21; r1:r0 = combine(r16,r18); call vmemset_asm }
	{ if (cmp.gtu(r26,r17.new)) jump:t 0000D894; r17 = add(r17,r24) }

l0000D8A8:
	{ r26 = memw(r29+100); r2 = memw(r29+88); r21 = r27; r1 = r24 }

l0000D8AC:
	{ r26 = memw(r29+100); r2 = memw(r29+88); r21 = r27 }

l0000D8B8:
	{ if (p0.new) jump:nt 0000D8F0; p0 = cmp.gt(r2,r11) }

l0000D8BC:
	{ r2 = memd(r29+124); r17 = memd(r29+72) }
	{ r0 = sub(r17,r2); call fn00009750 }
	{ r3 = memw(r29+136); r18 = memw(r29+120); r2 = mpyi(r0,r23) }
	{ r18 += add(r3,r2) }

l0000D8D8:
	{ r18 = add(r18,r23); r1:r0 = combine(r16,r18); r2 = r26; call vmemset_asm }
	{ if (cmp.gtu(r19,r17.new)) jump:t 0000D8D8; r17 = add(r17,r24) }

l0000D8F0:
	{ r2 = memw(r29+112); r8 = memw(r29+128) }

l0000D8F4:
	{ r2 = memw(r29+112) }

l0000D8F8:
	{ if (!cmp.eq(r8.new,r2)) jump:t 0000D73C; r27 = memw(r29+104); r8 = add(r8,00000001) }

l0000D904:
	{ r19 = memd(r29+28); r0 = memd(r29+24) }

l0000D908:
	{ if (p0.new) jump:nt 0000D930; p0 = r0 }

l0000D910:
	{ r2 = memd(r29+4); r17 = memd(r29+120) }
	{ r18 = memd(r29+8); r3 = memd(r29+116) }
	{ r17 += add(r3,r2) }

l0000D91C:
	{ r17 = add(r17,r23); r2 = r19; r1:r0 = combine(r16,r17); call vmemset_asm }
	{ if (!cmp.eq(r18.new,00000000)) jump:t 0000D91C; r18 = add(r18,FFFFFFFF) }

l0000D930:
	{ r5 = memd(r29+36); r6 = memd(r29+40) }

l0000D934:
	{ r7 = memd(r29+48); r2 = memd(r29+44); r6 = add(r6,00000001) }
	{ r19 = memw(r29+32); p0 = cmp.eq(r6,r5); r4 = sub(r7,r24); r2 = add(r2,r24) }
	{ memw(r29+48) = r4; memw(r29+44) = r2; if (!p0) jump:nt 0000D6B0 }

l0000D954:
	{ r4 = memd(r29+8); r2 = memd(r29+36) }
	{ r2 = mpyi(r2,r4); r3 = add(00000007,mpyi(r2,r4)) }
	{ r3 = and(r3,FFFFFFF8) }
	{ if (!cmp.gt(r18.new,00000000)) jump:nt 0000D988; r18 = sub(r3,r2) }

l0000D970:
	{ r17 = add(r3,mpyi(r17,r2)) }

l0000D974:
	{ r17 = add(r17,r19); r2 = r19; r1:r0 = combine(r16,r17); call vmemset_asm }
	{ if (!cmp.eq(r18.new,00000000)) jump:t 0000D974; r18 = add(r18,FFFFFFFF) }

l0000D988:
	{ r19:r18 = memd(r29+176); r17:r16 = memd(r29+184) }

l0000D98C:
	{ r23:r22 = memd(r29+160); r21:r20 = memd(r29+168) }
	{ r27:r26 = memd(r29+144); r25:r24 = memd(r29+152) }
	{ dealloc_return }

;; im2col_cn: 0000D99C
im2col_cn proc
	{ allocframe(+000000B8) }
	{ memd(r29+168) = r19:r18; r7 = memw(r29+196); r18 = r4 }
	{ r12 = memw(r29+208); r9 = memw(r29+192) }
	{ memd(r29+176) = r17:r16; r6 = memw(r29+204); r16 = r5; r19 = mpyi(r7,r3) }
	{ memd(r29+152) = r23:r22; memd(r29+160) = r21:r20; p0 = cmp.gt(r6,00000000); r8 = add(0000000F,mpyi(r19,r9)) }
	{ memw(r29+92) = r9; memd(r29+144) = r25:r24; if (p0) r28 = sub(r7,r2) }
	{ memw(r29+16) = r6; memd(r29+136) = r27:r26; r17 = and(r8,FFFFFFF0) }
	{ memw(r29+40) = r0; memw(r29+72) = r16 }
	{ memw(r29+64) = r12; memw(r29+132) = r1 }
	{ memw(r29+68) = r17; if (p0) r13 = memw(r29-44); if (!p0) jump:nt 0000DC30 }

l0000DA00:
	{ r15 = memw(r29+200); r4 = memw(r29+216); r14 = mpyi(r19,r9); r1 = mpyi(r7,r9) }
	{ memw(r29+96) = r15; r5 = sub(r7,r13); r6 = mpyi(r4,r2); r20 = mpyi(r3,r2) }
	{ memw(r29+32) = r3; memw(r29+36) = r7; r3 = 00000000; r8 = add(0000000F,mpyi(r1,r3)) }
	{ memw(r29+28) = r28; memw(r29+24) = r16; r7 = sub(00000000,r4) }
	{ memw(r29+52) = r7; memw(r29+20) = r3; r3 = sub(00000000,r13); r0 = lsr(r8,00000004) }
	{ memw(r29+60) = r14; memw(r29+44) = r13; r7 = sub(r17,r14) }
	{ memw(r29+56) = r7; memw(r29+8) = r3; r3 = mpyi(r15,r2); r1 = mpyi(r12,r0) }
	{ memw(r29+12) = r3; r7 = sub(00000000,r6); r2 = sub(r5,r2) }
	{ memw(r29+48) = r7; memw(r29+4) = r2; r1:r0 = vaslw(r1:r0,00000002) }
	{ memd(r29+80) = r1:r0 }

l0000DA74:
	{ if (p0.new) r7 = memw(r29+24); p0 = cmp.gt(r12,00000000); if (!p0.new) jump:nt 0000DC00 }

l0000DA80:
	{ memb(r29+29) = r2.new; r2 = memw(r29+44) }
	{ memw(r29+108) = r2; r2 = memd(r29+20) }
	{ r7 = memw(r29+8); r2 = mpyi(r2,r12) }
	{ memw(r29+104) = r7; r7 = 00000000 }
	{ memw(r29+100) = r7; memw(r29+76) = r2 }

l0000DAA0:
	{ if (p0.new) r8 = memw(r29+28); if (p0.new) r17 = memw(r29+52); p0 = cmp.gt(r9,00000000); if (!p0.new) jump:nt 0000DB9C }

l0000DAB0:
	{ r3 = memd(r29+100); r2 = memd(r29+96); r7 = 00000000 }
	{ r4 = memd(r29+44); r6 = memd(r29+104); r2 = mpyi(r3,r2) }
	{ r1 = memd(r29+116); r5 = memd(r29+108); r2 = sub(r2,r4); r3 = max(r6,r7) }
	{ r1 = memw(r29+32); r2 = add(r8,r2); r6 = sub(00000000,r2); r0 = max(r1,r7) }
	{ r6 = memw(r29+36); r24 = r1; r5 = max(r5,r7); r16 = max(r6,r7) }
	{ r5 = memw(r29+40); r2 = sub(r6,r5); r7 = mpyi(r1,r0); r22 = max(r2,r7) }
	{ memb(r29+32) = r2.new; r23 = memw(r29+112); r26 = mpyi(r22,r1); r2 = mpyi(r1,r2) }
	{ memw(r29+124) = r7; r21 = memd(r29+92); r2 = add(r2,r3); r7 = mpyi(r16,r1) }
	{ memw(r29+120) = r7; r2 = sub(r6,r16); r24 = add(r5,mpyi(r24,r2)) }
	{ r25 = sub(r2,r22) }
	{ r27 = mpyi(r25,r1) }

l0000DB30:
	{ nop; if (p0.new) jump:nt 0000DB40; p0 = cmp.gt(r9,-00000001) }

l0000DB38:
	{ if (cmp.gt(r2.new,r17)) jump:t 0000DB48; r2 = memw(r29+132) }

l0000DB40:
	{ r1 = and(r18,000000FF); r0 = r23; jump 0000DB88; r2 = r11 }

l0000DB44:
	{ r1 = and(r18,000000FF); r0 = r23 }

l0000DB48:
	{ if (!p0.new) jump:nt 0000DB58; p0 = cmp.gt(r8,00000000) }

l0000DB4C:
	{ r2 = memd(r29+120); r1 = and(r18,000000FF); r0 = r23; call fn000095F0 }

l0000DB58:
	{ if (p0.new) r2 = memw(r29+124); p0 = cmp.gt(r25,00000000); r1 = r24; if (!p0.new) jump:nt 0000DB74 }

l0000DB68:
	{ r2 = r27; r0 = add(r23,r2); call vmemcpy_asm }

l0000DB74:
	{ if (p0.new) r2 = memw(r29-128); r1 = and(r18,000000FF); if (!p0.new) jump:nt 0000DB8C; p0 = cmp.gt(r14,00000000) }

l0000DB80:
	{ r2 = r26; r0 = add(r23,r2) }

l0000DB88:
	{ call fn000095F0 }

l0000DB8C:
	{ r23 = add(r23,r19); r17 = r17; r24 = add(r24,r20) }
	{ if (!cmp.eq(r21.new,00000000)) jump:t 0000DB30; r21 = add(r21,FFFFFFFF) }

l0000DB9C:
	{ r21 = memd(r29+100); r2 = memd(r29+76); r1 = and(r18,000000FF) }

l0000DBA0:
	{ r21 = memd(r29+100); r2 = memd(r29+76) }
	{ r16 = memd(r29+72); r17 = memd(r29+68); r3 = add(r21,r2) }
	{ r2 = memd(r29+56); r4 = memd(r29+60); r0 = r16; r3 = mpyi(r3,r17) }
	{ call fn000095F0; r0 += add(r3,r4) }
	{ r3 = memd(r29+108); r2 = memd(r29+96); r21 = add(r21,00000001) }
	{ r1:r0 = memd(r29+80); r6 = memd(r29+112); r3 = add(r3,r2) }
	{ memw(r29+108) = r3; r12 = memw(r29+64); r3 = addasl(r6,r0,00000002) }
	{ r9 = memw(r29+92); r7 = memw(r29+116); p0 = cmp.eq(r21,r12) }
	{ memw(r29+112) = r3; r6 = memd(r29+104); r5 = sub(r7,r2) }
	{ r3 = add(r6,r2) }
	{ memw(r29+116) = r5; memw(r29+100) = r21 }
	{ memw(r29+104) = r3; if (!p0) jump:nt 0000DAA0 }

l0000DC00:
	{ r1:r0 = memd(r29+80); r2 = memd(r29+24) }
	{ r4 = memd(r29+48); r7 = memd(r29+12); r2 = addasl(r2,r1,00000002) }
	{ r7 = memd(r29+16); r3 = memd(r29+20); r4 = add(r4,r7) }
	{ r6 = memd(r29+52); r3 = r3 }
	{ memw(r29+24) = r2; r2 = memd(r29+96); p0 = cmp.eq(r3,r7) }
	{ memw(r29+20) = r3; memw(r29+48) = r4; r4 = add(r6,r2) }
	{ memw(r29+52) = r4; if (!p0) jump:nt 0000DA74 }

l0000DC30:
	{ r3 = memd(r29+16); r1 = and(r18,000000FF) }
	{ r0 = mpyi(r12,r3); r2 = add(00000007,mpyi(r12,r3)) }
	{ r2 = and(r2,FFFFFFF8) }
	{ r2 = sub(r2,r0); r0 = add(r16,mpyi(r0,r17)) }
	{ r2 = mpyi(r2,r17) }
	{ r19:r18 = memd(r29+168); r17:r16 = memd(r29+176); call fn000095F0 }
	{ r23:r22 = memd(r29+152); r21:r20 = memd(r29+160) }
	{ r27:r26 = memd(r29+136); r25:r24 = memd(r29+144) }
	{ dealloc_return }

;; im2col_slice_v0_co: 0000DC64
im2col_slice_v0_co proc
	{ allocframe(000000C8); memd(r29+472) = r23:r22 }
	{ memd(r29+184) = r19:r18; r23 = memw(r29+212) }
	{ memd(r29+152) = r27:r26; r7 = memw(r29+208); r27 = r5 }
	{ memw(r29+104) = r7; r26 = memw(r29+224) }
	{ memd(r29+160) = r25:r24; r18 = memw(r29+220); r19 = mpyi(r23,r3) }
	{ memw(r29+72) = r3; memd(r29+192) = r17:r16; r17 = r2; r8 = mpyi(r19,r7) }
	{ r6 = memw(r29+240); r2 = memw(r29+232); r7 = add(0000000F,mpyi(r19,r7)); r3 = add(00000007,mpyi(r26,r18)) }
	{ memw(r29+144) = r1; memd(r29+176) = r21:r20; r20 = mpyi(r26,r18) }
	{ memw(r29+92) = r8; memw(r29+140) = r2; r2 = and(r3,FFFFFFF8) }
	{ memw(r29+44) = r26; r24 = memw(r29+236) }
	{ r2 = sub(r2,r20); r3 = and(r7,FFFFFFF0) }
	{ memw(r29+68) = r0; r21 = r4; r1:r0 = combine(r26,r24) }
	{ memw(r29+96) = r3; memw(r29+4) = r2; r2 = sub(r3,r8) }
	{ memw(r29+88) = r2; r22 = add(r6,r24) }
	{ r25 = memw(r29+216) }
	{ r16 = memw(r29+228) }
	{ call fn00009750 }
	{ memw(r29+40) = r0; if (!p0.new) r7 = add(r18,FFFFFFFF); p0 = cmp.gt(r20,r22); r2 = mpyi(r0,r26) }
	{ r20 = sub(r24,r2) }
	{ memw(r29+36) = r20; r2 = p0 }
	{ memw(r29+8) = r2; jump 0000DD40; if (p0) jump:nt 0000DD24 }
0000DD24             16 5D FF 5B 12 40 60 70 00 D6 1A F5     .].[.@`p....
0000DD30 A0 30 07 30 02 DA 07 ED 02 D6 22 F3 03 C2 9D A1 .0.0......".....

l0000DD40:
	{ memw(r29+32) = r7; if (!p0.new) r6 = add(r25,00000000); if (p0.new) jump:nt 0000DFB0; p0 = cmp.gt(r0,r7) }

l0000DD4C:
	{ r5 = memd(r29+72); r2 = memd(r29+104); r4 = sub(r23,r16); r3 = mpyi(r0,r6) }
	{ memw(r29+108) = r6; r1 = memw(r29+140); r9 = sub(00000000,r16); r2 = mpyi(r23,r2) }
	{ r8 = r0; r3 = sub(r3,r1); r2 = add(0000000F,mpyi(r2,r5)); r6 = mpyi(r6,r17) }
	{ memw(r29+84) = r3; memw(r29+28) = r9; r9 = sub(r23,r17); r25 = mpyi(r5,r17) }
	{ memw(r29+24) = r6; memw(r29+60) = r9; r4 = sub(r4,r17); r3 = mpyi(r17,r3) }
	{ memw(r29+76) = r16; memw(r29+64) = r23; r2 = lsr(r2,00000004) }
	{ memw(r29+80) = r3; memw(r29+20) = r4 }
	{ memw(r29+16) = r2 }
	{ p0 = cmp.eq(r8,r0); r9 = r20; r2 = r26; if (p0.new) jump:nt 0000DDC8 }

l0000DDB8:
	{ if (!p0.new) r2 = add(r26,00000000); r9 = 00000000; p0 = cmp.eq(r8,r7) }
	{ if (p0) r2 = memw(r29+12) }

l0000DDC8:
	{ if (p0.new) memw(r29+52) = r8; memw(r29+100) = r2; p0 = cmp.gt(r2,r9); if (!p0.new) jump:nt 0000DF8C }

l0000DDD8:
	{ r5 = memd(r29+16); r4 = memd(r29+108); r2 = sub(r2,r9) }
	{ r7 = memd(r29+20); r6 = r4; r3 = mpyi(r4,r9) }
	{ memw(r29+124) = r27; r5 = memw(r29+76); r6 = add(r7,mpyi(r6,r9)); r2 = mpyi(r2,r5) }
	{ memw(r29+56) = r27; r7 = memw(r29+28); r3 = sub(r5,r3) }
	{ memw(r29+120) = r3; memw(r29+128) = r6; r2 = asl(r2,00000004); r4 = add(r7,mpyi(r4,r9)) }
	{ memw(r29+48) = r2; memw(r29+116) = r4 }

l0000DE18:
	{ memw(r29+112) = r9; r2 = memw(r29+104); r8 = 00000000 }
	{ if (p0.new) r6 = memw(r29-128); if (p0.new) r7 = memw(r29+120); if (!p0.new) jump:nt 0000DF1C; p0 = cmp.gt(r2,00000000) }

l0000DE30:
	{ r3 = memd(r29+112); r2 = memd(r29+108) }
	{ r1 = memd(r29+72); r4 = memd(r29+76); r3 = max(r6,r8); r2 = mpyi(r3,r2) }
	{ r16 = memd(r29+104); r7 = memd(r29+60); r2 = sub(r2,r4); r0 = max(r7,r8) }
	{ r4 = memd(r29+68); r5 = memd(r29+116); r2 = add(r7,r2); r6 = sub(00000000,r2) }
	{ r6 = memw(r29+64); r27 = r1; r20 = max(r2,r8); r23 = max(r6,r8) }
	{ r18 = memw(r29+84); r2 = sub(r6,r3); r7 = mpyi(r1,r0); r5 = max(r5,r8) }
	{ memb(r29+35) = r2.new; r26 = memw(r29+124); r22 = mpyi(r20,r1); r2 = mpyi(r1,r2) }
	{ memw(r29+136) = r7; r2 = add(r2,r5); r7 = mpyi(r23,r1) }
	{ memw(r29+132) = r7; r2 = sub(r6,r23); r27 = add(r4,mpyi(r27,r2)) }
	{ r24 = sub(r2,r20) }
	{ r17 = mpyi(r24,r1) }

l0000DEAC:
	{ nop; if (p0.new) jump:nt 0000DEBC; p0 = cmp.gt(r10,-00000001) }

l0000DEB4:
	{ if (cmp.gt(r2.new,r18)) jump:t 0000DEC4; r2 = memw(r29+144) }

l0000DEBC:
	{ r1:r0 = combine(r21,r26); jump 0000DF04; r2 = r11 }

l0000DEC0:
	{ r1:r0 = combine(r21,r26) }

l0000DEC4:
	{ if (p0.new) r2 = memw(r29-124); if (!p0) r1:r0 = combine(r21,r26); if (!p0.new) jump:nt 0000DED4; p0 = cmp.gt(r15,00000000) }

l0000DED0:
	{ call vmemset_asm }

l0000DED4:
	{ if (p0.new) r2 = memw(r29-120); if (p0.new) r1 = add(r27,00000000); p0 = cmp.gt(r24,00000000); if (!p0.new) jump:nt 0000DEF0 }

l0000DEE4:
	{ r2 = r17; r0 = add(r26,r2); call vmemcpy_asm }

l0000DEF0:
	{ if (p0.new) r2 = memw(r29-116); if (p0.new) r1 = add(r21,00000000); if (!p0.new) jump:nt 0000DF08; p0 = cmp.gt(r12,00000000) }

l0000DEFC:
	{ r2 = r22; r0 = add(r26,r2) }

l0000DF04:
	{ call vmemset_asm }

l0000DF08:
	{ r26 = add(r26,r19); r18 = add(r18,00000001); r27 = add(r27,r25) }
	{ if (!cmp.eq(r16.new,00000000)) jump:t 0000DEAC; r16 = add(r16,FFFFFFFF) }

l0000DF1C:
	{ r16 = memd(r29+124); r2 = memd(r29+92); r1 = r21 }

l0000DF20:
	{ r16 = memd(r29+124); r2 = memd(r29+92) }

l0000DF24:
	{ r2 = memw(r29+88); r0 = add(r16,r2); call vmemset_asm }
	{ r3 = memd(r29+116); r2 = memd(r29+108) }
	{ r6 = memw(r29+128); r7 = memw(r29+120); r3 = add(r3,r2) }
	{ memw(r29+116) = r3; r9 = memw(r29+112); r3 = add(r6,r2); r5 = sub(r7,r2) }
	{ r7 = memd(r29+96); r2 = memd(r29+100); r9 = add(r9,00000001) }
	{ memw(r29+128) = r3; memw(r29+120) = r5; p0 = cmp.eq(r9,r2); r16 = add(r16,r7) }
	{ memw(r29+124) = r16; if (p0) r2 = memw(r29+48); if (!p0) jump:nt 0000DE18 }

l0000DF74:
	{ r26 = memw(r29+44); r27 = memw(r29+56) }
	{ r6 = memd(r29+24); r0 = memd(r29+40); r27 = add(r27,r2) }
	{ r7 = memd(r29+32); r20 = memd(r29+36) }
	{ r8 = memw(r29+52) }

l0000DF8C:
	{ r2 = memw(r29+80) }
	{ r3 = memd(r29+84); r5 = memd(r29+108); r8 = add(r8,00000001); p0 = cmp.gt(r7,r8) }
	{ memb(r29+20) = r2.new; r3 = add(r3,r5); r2 = add(r2,r6) }
	{ memw(r29+84) = r3 }

l0000DFB0:
	{ r0 = memw(r29+8) }
	{ if (!p0.new) r25:r24 = memd(r29+160); if (!p0.new) r1:r0 = combine(r21,r27); if (!p0.new) jump:nt 0000DFD8; p0 = r0 }

l0000DFC4:
	{ r19:r18 = memd(r29+184); r17:r16 = memd(r29+192) }
	{ r23:r22 = memd(r29+168); r21:r20 = memd(r29+176) }
	{ r27:r26 = memd(r29+152); r25:r24 = memd(r29+160) }
	{ dealloc_return }

l0000DFD8:
	{ r3 = memd(r29+4); r2 = memd(r29+96) }
	{ r19:r18 = memd(r29+184); r17:r16 = memd(r29+192); r2 = mpyi(r3,r2) }
	{ r23:r22 = memd(r29+168); r21:r20 = memd(r29+176) }
	{ r27:r26 = memd(r29+152) }
	{ deallocframe; jump vmemset_asm }

;; im2col_slice_co: 0000DFF4
im2col_slice_co proc
	{ allocframe(+00000088) }
	{ memd(r29+112) = r21:r20; r6 = memw(r29+144); r21 = r2 }
	{ memd(r29+88) = r27:r26; r2 = memw(r29+164) }
	{ memw(r29+48) = r2; r7 = memw(r29+168) }
	{ memd(r29+96) = r25:r24; r27 = memw(r29+148); r2 = mpyi(r6,r3) }
	{ memd(r29+120) = r19:r18; memd(r29+104) = r23:r22; r22 = r1 }
	{ r19 = memw(r29+160) }
	{ r20 = memw(r29+176); r23 = memw(r29+172); r2 = add(0000000F,mpyi(r2,r27)) }
	{ memd(r29+128) = r17:r16; memw(r29+80) = r0; r2 = and(r2,FFFFFFF0); r1:r0 = combine(r19,r23) }
	{ memw(r29+24) = r19; r16 = r5; r18 = add(r20,r23) }
	{ memw(r29+84) = r3; memw(r29+72) = r6 }
	{ memw(r29+64) = r2; r17 = memw(r29+156) }
	{ memw(r29+68) = r7; memw(r29+76) = r4 }
	{ r24 = memw(r29+152) }
	{ call fn00009750 }
	{ r25 = r0; r2 = mpyi(r19,r17) }
	{ memw(r29+20) = r25; r3 = mpyi(r25,r19) }
	{ r23 = r18; r26 = sub(r23,r3) }
	{ memw(r29+16) = r26; if (!p0) r1:r0 = combine(r19,r23); if (p0.new) jump:nt 0000E090; p0 = cmp.gt(r2,r15) }

l0000E088:
	{ memw(r29+40) = r19; r17 = r17; jump 0000E0A4 }

l0000E090:
	{ call fn00009750 }
	{ r17 = r0 }
	{ r2 = mpyi(r17,r19) }
	{ r2 = sub(r23,r2) }
	{ memw(r29+40) = r2 }

l0000E0A4:
	{ r1 = memd(r29+76); r2 = memd(r29+64); r0 = r16 }
	{ memw(r29+12) = r17; r2 = mpyi(r2,r20) }
	{ call vmemset_asm }
	{ p0 = cmp.gt(r25,r17); r5 = r22; if (p0.new) jump:nt 0000E224; r3 = mpyi(r25,r24) }

l0000E0C8:
	{ r0 = memd(r29+84); r2 = memd(r29+72); r4 = sub(r27,r21); r7 = mpyi(r24,r21) }
	{ r1 = memd(r29+68); r23 = 00000000; r6 = r25; r2 = mpyi(r27,r2) }
	{ memb(r29+15) = r3.new; r3 = sub(r3,r1); r2 = add(0000000F,mpyi(r2,r0)) }
	{ memw(r29+8) = r7; memw(r29+52) = r24; r2 = lsr(r2,00000004) }
	{ memw(r29+56) = r3; memw(r29+44) = r4 }
	{ memw(r29+4) = r2 }
	{ r2 = memw(r29+40); p0 = cmp.eq(r6,r17); p1 = cmp.eq(r6,r25) }
	{ r4 = mux(p1,r26,00000000); if (!p0) r2 = add(r19,00000000) }
	{ if (p1) r2 = add(r19,00000000) }
	{ if (p0.new) memw(r29+36) = r16; memw(r29+68) = r2; if (p0.new) jump:nt 0000E200; p0 = cmp.gt(r2,r4) }

l0000E128:
	{ memw(r29+32) = r6; r3 = memd(r29+4); r22 = r16; r2 = sub(r2,r4) }
	{ r2 = mpyi(r2,r3) }
	{ r2 = asl(r2,00000004) }
	{ memw(r29+28) = r2 }

l0000E140:
	{ memw(r29+76) = r4; r2 = memd(r29+72) }
	{ if (p0.new) r7 = memw(r29+48); if (p0.new) r25 = memw(r29+60); if (!p0.new) jump:nt 0000E1D4; p0 = cmp.gt(r2,00000000) }

l0000E150:
	{ r3 = memd(r29+76); r2 = memd(r29+52); r17 = 00000000 }
	{ r20 = memw(r29+72); r26 = memw(r29+56); r2 = mpyi(r3,r2) }
	{ r2 = memw(r29+44); r24 = sub(r2,r7) }
	{ r18 = add(r2,r24); r19 = sub(00000000,r24) }

l0000E174:
	{ p0 = cmp.gt(r25,FFFFFFFF); if (!p0.new) jump:nt 0000E1C0 }

l0000E17C:
	{ p0 = cmp.gt(r5,r25); if (!p0.new) jump:nt 0000E1C0; r3 = max(r23,r19) }

l0000E188:
	{ r4 = sub(r27,r3); r2 = max(r23,r18) }
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 0000E1C0; r2 = sub(r4,r2) }

l0000E19C:
	{ r3 = memd(r29+80); r6 = memd(r29+84); r0 = add(r3,r17) }
	{ r16 = r5; r1 = add(r4,r26); r0 = add(r22,mpyi(r0,r6)) }
	{ r1 = add(r3,mpyi(r1,r6)); r2 = mpyi(r2,r6) }
	{ call vmemcpy_asm }
	{ r5 = r16 }

l0000E1C0:
	{ r17 = add(r17,r27); r25 = add(r25,00000001); r26 = add(r26,r21) }
	{ if (!cmp.eq(r20.new,00000000)) jump:t 0000E174; r20 = add(r20,FFFFFFFF) }

l0000E1D4:
	{ r2 = memd(r29+64); r4 = memd(r29+76) }

l0000E1D8:
	{ r7 = memd(r29+68); r22 = add(r22,r2) }
	{ if (!cmp.eq(r4.new,r7)) jump:t 0000E140; r4 = add(r4,00000001) }

l0000E1E8:
	{ r25 = memw(r29+20); r19 = memw(r29+24); r16 = add(r16,r2) }
	{ r17 = memw(r29+12); r26 = memw(r29+16) }
	{ r7 = memd(r29+8); r6 = memd(r29+32) }

l0000E200:
	{ r2 = memw(r29+56) }
	{ r3 = memd(r29+60); r4 = memd(r29+52); r6 = add(r6,00000001); p0 = cmp.gt(r17,r6) }
	{ memb(r29+14) = r2.new; r3 = add(r3,r4); r2 = add(r2,r7) }
	{ memw(r29+60) = r3 }

l0000E224:
	{ r19:r18 = memd(r29+120); r17:r16 = memd(r29+128) }
	{ r23:r22 = memd(r29+104); r21:r20 = memd(r29+112) }
	{ r27:r26 = memd(r29+88); r25:r24 = memd(r29+96) }
	{ dealloc_return }

;; fast_im2col_co: 0000E238
;;   Called from:
;;     0001A564 (in supernode_execute_hvx_slice)
fast_im2col_co proc
	{ allocframe(+00000068) }
	{ r6 = memw(r29+112); r9 = memw(r29+124) }
	{ memd(r29+96) = r17:r16; r7 = memw(r29+128); r17 = r3; r6 = add(r6,r9) }
	{ memd(r29+72) = r23:r22; memd(r29+88) = r19:r18; r19:r18 = combine(r5,r4); r7 += add(r6,FFFFFFFF) }
	{ memw(r29+36) = r7; memd(r29+80) = r21:r20; if (!p0.new) r3 = add(r17,0000000F); p0 = cmp.gt(r9,r7) }
	{ memd(r29+56) = r27:r26; memd(r29+64) = r25:r24 }
	{ memw(r29+52) = r1; if (!p0) r8 = memw(r29-124); if (p0) jump:nt 0000E414 }

l0000E280:
	{ r5 = memw(r29+136); r12 = memw(r29+120); r4 = lsr(r3,00000004); r20 = mpyi(r17,r2) }
	{ r13 = memw(r29+140); r27 = and(r3,FFFFFFF0); r6 = mpyi(r9,r12); r4 = mpyi(r4,r8) }
	{ r14 = memw(r29+144); r26 = r20; r15 = add(r5,r2); r7 = mpyi(r12,r17) }
	{ r23 = sub(r27,r17); r5 = sub(r6,r13); r4 = asl(r4,00000004); r21 = mpyi(r5,r17) }
	{ memw(r29+28) = r14; r14 = memw(r29+52); r3 = sub(r13,r6); r26 = add(r0,mpyi(r26,r5)) }
	{ memw(r29+32) = r12; memw(r29+4) = r4; r4 = sub(FFFFFFFF,r3); r22 = mpyi(r15,r17) }
	{ memw(r29+12) = r8; r16 = add(r13,r14); r8 = sub(00000000,r12); r2 = mpyi(r7,r2) }
	{ memw(r29+8) = r8; memw(r29+20) = r13 }
	{ memw(r29+24) = r2; memw(r29+16) = r16 }

l0000E2FC:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0000E36C; r2 = memw(r29+28) }

l0000E308:
	{ if (p0.new) r3 = memw(r29+20); p0 = cmp.gt(r12,00000000); if (p0.new) jump:nt 0000E324 }

l0000E314:
	{ memw(r29+44) = r5; memw(r29+40) = r4 }
	{ memw(r29+48) = r9; jump 0000E3E8 }
0000E320 19 C2 23 F3                                     ..#.            

l0000E324:
	{ if (cmp.gtu(r25,r2.new)) jump:nt 0000E314; r2 = memw(r29+16) }

l0000E330:
	{ memw(r29+48) = r9; r3 = memw(r29+8); r24 = r26 }
	{ r2 = sub(r4,r2) }
	{ memw(r29+40) = r4; memw(r29+44) = r5; r2 = maxu(r3,r2) }
	{ r16 = sub(00000000,r2) }

l0000E34C:
	{ p0 = cmp.gt(r25,FFFFFFFF); if (!p0.new) jump:nt 0000E35C }

l0000E354:
	{ if (cmp.gt(r2.new,r25)) jump:t 0000E3B8; r2 = memw(r29+52) }

l0000E35C:
	{ r1:r0 = combine(r18,r19); r2 = r22; call vmemset_asm }

l0000E360:
	{ r1:r0 = combine(r18,r19); r2 = r22 }

l0000E368:
	{ jump 0000E3D4 }

l0000E36C:
	{ memw(r29+40) = r4; r2 = memd(r29+12); r25:r24 = combine(r19,r26) }
	{ memw(r29+44) = r5; r16 = r2; p0 = cmp.gt(r2,00000000) }
	{ memw(r29+48) = r9; if (!p0) jump:nt 0000E3E8 }

l0000E384:
	{ r1:r0 = combine(r24,r25); r2 = r17; call vmemcpy_asm }
	{ r1 = r18; r2 = r23; r0 = add(r25,r17); call vmemset_asm }
	{ if (!cmp.eq(r16.new,00000000)) jump:t 0000E384; r16 = add(r16,FFFFFFFF); r25 = add(r25,r27); r24 = add(r24,r17) }

l0000E3B0:
	{ r19 = add(r19,r2); jump 0000E3E8 }

l0000E3B8:
	{ r1:r0 = combine(r18,r19); r2 = r21; call vmemset_asm }
	{ r1 = r24; r2 = r20; r0 = add(r19,r21); call vmemcpy_asm }

l0000E3D4:
	{ r19 = add(r19,r22); r25 = add(r25,00000001); r24 = add(r24,r20) }
	{ if (!cmp.eq(r16.new,00000000)) jump:t 0000E34C; r16 = add(r16,FFFFFFFF) }

l0000E3E8:
	{ r2 = memw(r29+36); r9 = memw(r29+48) }

l0000E3EC:
	{ r2 = memw(r29+36) }
	{ r12 = memw(r29+32); r7 = memw(r29+24); r9 = add(r9,00000001); p0 = cmp.gt(r2,r9) }
	{ r4 = memd(r29+40); r5 = memd(r29+44); r26 = add(r26,r7) }
	{ r4 = add(r4,r12); r5 = add(r5,r12); if (p0) jump:nt 0000E2FC }

l0000E414:
	{ r19:r18 = memd(r29+88); r17:r16 = memd(r29+96) }
	{ r23:r22 = memd(r29+72); r21:r20 = memd(r29+80) }
	{ r27:r26 = memd(r29+56); r25:r24 = memd(r29+64) }
	{ dealloc_return }
0000E428                         00 00 00 00 00 00 00 00         ........

;; deconv_execute_ref: 0000E430
deconv_execute_ref proc
	{ allocframe(+000000B8) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+152) = r23:r22; memd(r29+176) = r17:r16 }
	{ r16 = memw(r2); r22 = memb(r0+32) }
	{ r7 = memw(r3); r5 = memw(r2+24) }
	{ memd(r29+168) = r19:r18; r17 = memw(r2+4); p0 = cmp.eq(r22,00000000) }
	{ memd(r29+144) = r25:r24; memd(r29+160) = r21:r20 }
	{ memw(r29+104) = r7; r7 = memw(r16+4) }
	{ r20 = memw(r2+8); r19 = memw(r2+12) }
	{ r24 = memw(r2+16); r21 = memw(r2+20) }
	{ memd(r29+136) = r27:r26; r2 = memw(r16+12) }
	{ memw(r29+88) = r0; r0 = memw(r16+8) }
	{ memw(r29+92) = r7; r26 = memw(r16) }
	{ memw(r29+124) = r2; r7 = memw(r17+12) }
	{ r18 = memw(r5+8); r2 = memw(r17) }
	{ r4 = memw(r3+8); r27 = memw(r5+4) }
	{ memw(r29+16) = r1; r3 = memw(r3+4); r1 = p0 }
	{ memw(r29+100) = r7 }
	{ memw(r29+120) = r2; r7 = memw(r17+4) }
	{ r2 = memw(r17+8) }
	{ memw(r29+128) = r0; memw(r29+80) = r5 }
	{ memw(r29+72) = r3; memw(r29+76) = r4 }
	{ memw(r29+32) = r7; memw(r29+20) = r26 }
	{ memw(r29+108) = r1; memw(r29+84) = r2 }
	{ if (p0) jump:nt 0000E4E4 }

l0000E4BC:
	{ if (p0.new) r2 = memw(r29-128); if (p0.new) r3 = memw(r29+32); if (p0.new) jump:nt 0000E4DC; p0 = cmp.eq(r14,00000002) }

l0000E4C8:
	{ r0 = r18; r23 = 00000000; if (!p0.new) jump:nt 0000E4F0; p0 = cmp.eq(r14,00000001) }

l0000E4D0:
	{ r2 = memw(r29+128) }
	{ jump 0000E4E4; r0 += add(r2,FFFFFFFF) }

l0000E4DC:
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r18) }

l0000E4E4:
	{ r1 = r18; call fn00009760 }
	{ r23 = r0 }

l0000E4F0:
	{ if (!p0.new) r1 = add(r27,00000000); p0 = cmp.eq(r22,00000002) }
	{ memb(r29+29) = r0.new; if (p0) r1 = add(r27,00000000); if (p0) jump:nt 0000E53C; r0 = p0 }

l0000E50C:
	{ if (p0.new) r2 = memw(r29+92); if (!p0.new) r1 = add(r27,00000000) }
	{ memb(r29+11) = r2.new; r0 = memw(r29+108); r2 = 00000000 }
	{ if (p0.new) r0 = memw(r29+92); if (!p0.new) jump:nt 0000E554 }

l0000E52C:
	{ jump 0000E548 }
0000E530 00 C0 61 70 E0 5F 02 E2 0A C0 00 58             ..ap._.....X    

l0000E53C:
	{ r3 = memd(r29+120); r2 = memd(r29+92) }
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r1) }

l0000E548:
	{ call fn00009760 }
	{ memw(r29+44) = r0 }
	{ r3 = memw(r20+16); r2 = memw(r19+16); r20 = 437F0000 }

l0000E554:
	{ r3 = memw(r20+16); r2 = memw(r19+16); r20 = 00000000 }

l0000E55C:
	{ r4 = memw(r24+16); r6 = memw(r17+16) }
	{ r17 = memw(r3); r2 = memw(r2) }
	{ memw(r29+56) = r6; r5 = memw(r21+16) }
	{ r6 = memd(r29+104); r7 = memw(r16+16); r21 = sfsub(r2,r17) }
	{ r1:r0 = combine(r20,r21) }
	{ memw(r29+96) = r7; r16 = memw(r5) }
	{ r6 = memw(r6+16) }
	{ memw(r29+52) = r6; r19 = memw(r4); call fn00009610 }
	{ r25 = r20; r16 = r0; r24 = sfsub(r16,r19) }
	{ r1:r0 = combine(r20,r24); call fn00009610 }
	{ r3 = 38D1B717; r16 = sfmpy(r16,r0) }
	{ r21 = r3; r2 = 4F000000; r1:r0 = combine(r21,r3) }
	{ memb(r29+17) = r2.new; call fn00009600; r2 = sfmpy(r16,r2) }
	{ r20 = 00000000; r1:r0 = combine(r0,r25) }
	{ r2 = sfsub(r20,r17) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = r21; r24 = r0; r1 = r24; call fn00009600 }
	{ r1:r0 = combine(r0,r25); call fn00009610 }
	{ r2 = sfsub(r20,r19) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r2 = memd(r29+32); r21 = r0; r19 = add(r18,FFFFFFFF) }
	{ r17 = sub(r18,r2) }

l0000E60C:
	{ r0 = memd(r29+116); r3 = r23 }
	{ if (p0.new) r0 = add(r17,r3); if (p0.new) jump:nt 0000E638; p0 = r0 }

l0000E61C:
	{ if (p0.new) r0 = add(r19,r3); if (p0.new) jump:nt 0000E638; p0 = cmp.eq(r14,00000001) }

l0000E624:
	{ r1 = memd(r29+108); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r18,r3); if (!p0.new) jump:nt 0000E648; p0 = r1 }

l0000E634:
	{ jump 0000E63C }

l0000E638:
	{ r1 = r18 }

l0000E63C:
	{ r20 = r3; call fn00009760 }
	{ r3 = r20 }

l0000E648:
	{ if (!cmp.eq(r2.new,r0)) jump:t 0000E60C; r2 = memw(r29+128); r23 = add(r3,00000001) }

l0000E658:
	{ memw(r29+64) = r3; r3 = memd(r29+120) }
	{ r2 = CF000000; r21 = convert_uw2sf(r24):chop; r23 = convert_uw2sf(r21):chop }
	{ memw(r29+112) = r20; r19 = memd(r29+44); r17 = sub(r20,r3); r2 = sfmpy(r16,r2) }
	{ memw(r29+60) = r2 }

l0000E67C:
	{ r0 = memw(r29+116) }
	{ if (!p0.new) r1:r0 = combine(r20,r19); if (p0.new) jump:nt 0000E6AC; p0 = r0 }

l0000E68C:
	{ if (p0.new) jump:nt 0000E6A4; p0 = cmp.eq(r14,00000001) }

l0000E690:
	{ r1 = memd(r29+108); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r20,r19); if (!p0.new) jump:nt 0000E6B8; p0 = r1 }

l0000E6A0:
	{ jump 0000E6B4 }

l0000E6A4:
	{ jump 0000E6B4; r0 += add(r20,FFFFFFFF) }

l0000E6AC:
	{ r1 = r20; r0 = add(r17,r19) }

l0000E6B4:
	{ call fn00009760 }

l0000E6B8:
	{ if (!cmp.eq(r2.new,r0)) jump:t 0000E67C; r2 = memw(r29+92); r19 = add(r19,00000001) }

l0000E6C8:
	{ r24 = memw(r29+16); r16 = memw(r29+88); r4 = add(PC,0000000A) }
	{ r22 = add(r19,FFFFFFFF); r1 = 0000008B }
	{ memb(r29+1) = r2.new; r2 = memw(r16+28) }
	{ memw(r29) = r16 }
	{ call logmsg_function }
	{ r7 = memw(r29+128); r17 = memw(r29+124); r4 = add(PC,000172FC) }
	{ memw(r29+12) = r17; r2 = r24; r1 = 0000008C }
	{ memw(r29+8) = r7; r3 = memd(r29+92) }
	{ memw(r29) = r26; memw(r29+4) = r3 }
	{ call logmsg_function }
	{ r3 = memw(r29+32); r25 = memw(r29+84); r2 = r24; r1 = 0000008D }
	{ memw(r29+12) = r25; r27 = memw(r29+100); r4 = add(PC,000172D9) }
	{ memw(r29+8) = r3; r7 = memd(r29+120) }
	{ memw(r29) = r27; memw(r29+4) = r7 }
	{ call logmsg_function }
	{ memw(r29+4) = r18; r1 = 0000008E; r4 = add(PC,000172D1) }
	{ memw(r29) = r20; r2 = r24 }
	{ call logmsg_function }
	{ r2 = r24; r1 = 0000008F; r4 = add(PC,000172C9) }
	{ memb(r29) = r3.new; r3 = memb(r16+32); r16 = r24; call logmsg_function }
	{ memw(r29+12) = r27; r24 = memw(r29+64); r4 = add(PC,0000003B) }
	{ memw(r29) = r26; memw(r29+8) = r24; r1 = 00000090 }
	{ memw(r29+4) = r22; r2 = r16 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r17,r25); r12 = r22; if (p0.new) jump:nt 0000E7D8; r2 = mpyi(r26,r27) }

l0000E7BC:
	{ r1 = 00000092; r3 = add(PC,000172A6) }
	{ r2 = r16 }

l0000E7CC:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0000EAF0 }

l0000E7D8:
	{ memw(r29+24) = r12; r3 = memw(r29+104); r9:r8 = combine(r24,r27) }
	{ r4 = memw(r3+20); r2 = mpyi(r2,r24) }
	{ r2 = mpyi(r2,r22) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 0000E810; r2 = asl(r2,00000002) }

l0000E7FC:
	{ memw(r29+4) = r2; r1 = 00000094; r3 = add(PC,0000003F) }
	{ memw(r29) = r4; r2 = r16 }
	{ jump 0000E7CC }

l0000E810:
	{ r4 = memw(r29+80) }
	{ if (cmp.eq(r3.new,00000001)) jump:t 0000E82C; r3 = memw(r4) }

l0000E820:
	{ r1 = 00000096; jump 0000E7CC; r3 = add(PC,00000035) }

l0000E82C:
	{ if (cmp.eq(r3.new,00000001)) jump:t 0000E84C; r3 = memw(r4+12); r27 = r18; p0 = cmp.gt(r26,00000000) }

l0000E840:
	{ r1 = 00000097; jump 0000E7CC; r3 = add(PC,00000026) }

l0000E84C:
	{ r7 = memd(r29+72); r3 = memd(r29+104) }
	{ r4 = memw(r29+60) }
	{ memw(r3+12) = r8; memw(r3+4) = r12 }
	{ memw(r3+24) = r2; memw(r3+8) = r9 }
	{ memw(r3) = r26 }
	{ r3 = memd(r29+76); r2 = memw(r7+16) }
	{ memw(r7) = 00000001; memw(r7+12) = 00000001 }
	{ memw(r7+8) = 00000001; memw(r7+4) = 00000001 }
	{ memw(r7+24) = 00000004; memw(r2) = r4 }
	{ memw(r3) = 00000001; r2 = memw(r3+16) }
	{ memw(r3+8) = 00000001; memw(r3+4) = 00000001 }
	{ memw(r3+12) = 00000001; r4 = memd(r29+68) }
	{ memw(r3+24) = 00000004; memw(r2) = r4; if (!p0) jump:nt 0000EAC8 }

l0000E894:
	{ r6 = memd(r29+120); r3 = memd(r29+92); r2 = r20 }
	{ r5 = memw(r29+128); r7 = memw(r29+32); r3 = add(r3,FFFFFFFF) }
	{ r18 = r7; r4 = r7; r5 = add(r5,FFFFFFFF); r2 = add(r6,mpyi(r2,r3)) }
	{ memb(r29+7) = r5.new; r5 = 00000000; r1 = mpyi(r7,r17); r4 += mpyi(r27,r5) }
	{ r4 = sub(r4,r9); r17 = mpyi(r17,r8) }
	{ memb(r29+27) = r3.new; r4 += lsr(r4,0000001F); r3 = mpyi(r1,r8) }
	{ memb(r29+12) = r7.new; r2 = asr(r2,00000001); r7 = asr(r4,00000001) }

l0000E8F0:
	{ p0 = cmp.gt(r12,00000000); if (!p0.new) jump:nt 0000EAAC }

l0000E8F8:
	{ r3 = memd(r29+28); r2 = memd(r29+92); r6 = 00000000 }
	{ memw(r29+116) = r6; r7 = memd(r29+24); r2 = mpyi(r3,r2) }
	{ memw(r29+84) = r2; r5 = mpyi(r3,r7) }
	{ memw(r29+40) = r5 }

l0000E914:
	{ memw(r29+44) = r19; p0 = cmp.gt(r9,00000000); if (!p0.new) jump:nt 0000EA98 }

l0000E920:
	{ r4 = memd(r29+116); r2 = memd(r29+40); r7 = 00000000 }
	{ memw(r29+68) = r7; r3 = memd(r29+36); r2 = add(r4,r2) }
	{ r3 = add(r4,r3); r2 = mpyi(r2,r9) }
	{ memw(r29+60) = r2; memw(r29+88) = r3 }

l0000E93C:
	{ p0 = cmp.gt(r8,00000000); if (!p0.new) jump:nt 0000EA80 }

l0000E944:
	{ r2 = memd(r29+60); r4 = memd(r29+68) }
	{ r5 = memd(r29+56); r3 = memd(r29+48); r2 = add(r4,r2) }
	{ r3 = memd(r29+52); r4 = 00000000; r19 = add(r4,r3); r2 = mpyi(r2,r8) }
	{ r2 = addasl(r3,r2,00000002) }

l0000E960:
	{ memw(r29+72) = r2; r2 = memd(r29+120); r26 = 00000000; r25 = r5 }
	{ p0 = cmp.gt(r2,00000000); r16 = 00000000 }
	{ memw(r29+80) = r5; memw(r29+76) = r4; if (!p0) jump:nt 0000EA60 }

l0000E97C:
	{ r2 = memd(r29+116); r1 = r20 }
	{ r0 = sub(r2,r26); call fn00009770 }
	{ if (!p0.new) jump:nt 0000EA4C; p0 = cmp.eq(r0,00000000) }

l0000E98C:
	{ r2 = memd(r29+88); r1 = r20 }
	{ r0 = sub(r2,r26); call fn00009750 }
	{ if (!cmp.gt(r2.new,r0)) jump:nt 0000EA4C; r2 = memw(r29+92) }

l0000E9A4:
	{ if (p0.new) r20 = add(r25,00000000); if (p0.new) r24 = 00000000 }
	{ if (p0.new) r3 = memw(r29-128); if (p0.new) r2 = memw(r29+84); if (!p0.new) jump:nt 0000EA4C; p0 = cmp.gt(r10,00000000) }

l0000E9B8:
	{ r2 = add(r0,r2) }
	{ memb(r29+26) = r2.new; r2 = mpyi(r2,r3) }

l0000E9C4:
	{ r22 = sub(r19,r24) }
	{ r1:r0 = combine(r27,r22); call fn00009770 }
	{ if (!p0) r1:r0 = combine(r27,r22); if (!p0.new) jump:nt 0000EA40; p0 = cmp.eq(r0,00000000) }

l0000E9D8:
	{ call fn00009750 }
	{ if (!cmp.gt(r2.new,r0)) jump:nt 0000EA40; r2 = memw(r29+128) }

l0000E9E8:
	{ if (p0.new) r3 = memw(r29+124) }
	{ if (p0.new) r8 = memw(r29+100); if (!p0.new) jump:nt 0000EA40; p0 = cmp.gt(r3,00000000) }

l0000E9F4:
	{ r4 = memd(r29+96); r2 = memd(r29+104); p0 = cmp.gtu(r3,00000001); r6 = add(r3,FFFFFFFF) }
	{ r2 = add(r0,r2); loop0(0000EA24,r6) }
	{ r4 = memb(r20); r3 = add(r20,r8); r2 = add(r4,mpyi(r2,r3)) }
	{ r5 = memb(r2++#1); r4 = sub(r4,r23) }
	{ r5 = sub(r5,r21); if (!p0) jump:nt 0000EA3C }

l0000EA24:
	{ r7 = memb(r2++#1); r6 = memb(r3); r3 = add(r3,r8); r16 += mpyi(r4,r5) }
	{ r5 = sub(r7,r21); r4 = sub(r6,r23) }

l0000EA3C:
	{ r16 += mpyi(r4,r5) }

l0000EA40:
	{ if (!cmp.eq(r24.new,r18)) jump:t 0000E9C4; r24 = add(r24,00000001); r20 = add(r20,r17) }

l0000EA4C:
	{ r7 = memd(r29+120); r2 = memd(r29+108) }

l0000EA50:
	{ if (!cmp.eq(r26.new,r7)) jump:t 0000E97C; r20 = memw(r29+112); r26 = add(r26,00000001); r25 = add(r25,r2) }

l0000EA60:
	{ r5 = memd(r29+80); r4 = memd(r29+76) }

l0000EA64:
	{ r8 = memw(r29+100); r2 = memw(r29+72); r5 = add(r5,00000001); r4 = add(r4,00000001) }
	{ memw(r2++#4) = r16; p0 = cmp.eq(r4,r8) }
	{ if (!p0) jump:nt 0000E960 }

l0000EA80:
	{ r9 = memw(r29+64); r2 = memw(r29+68) }
	{ r2 = add(r2,00000001) }
	{ memw(r29+68) = r2; p0 = cmp.eq(r9,r2); if (!p0.new) jump:nt 0000E93C }

l0000EA98:
	{ r19 = memd(r29+44); r3 = memd(r29+116) }
	{ r3 = add(r3,00000001); r2 = add(r3,00000002) }
	{ memw(r29+116) = r3; if (!p0.new) jump:nt 0000E914; p0 = cmp.eq(r2,r11) }

l0000EAAC:
	{ r26 = memw(r29+20); r2 = memw(r29+28) }
	{ r12 = memw(r29+24); r2 = add(r2,00000001) }
	{ memw(r29+28) = r2; p0 = cmp.eq(r2,r26); if (!p0.new) jump:nt 0000E8F0 }

l0000EAC8:
	{ memw(r29+12) = r8; r1 = 000000D3; r4 = add(PC,00016FEB) }
	{ memw(r29+8) = r9; r2 = memw(r29+16) }
	{ memw(r29) = r26; memw(r29+4) = r12 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0000EAF0:
	{ r19:r18 = memd(r29+168); r17:r16 = memd(r29+176) }
	{ r23:r22 = memd(r29+152); r21:r20 = memd(r29+160) }
	{ r27:r26 = memd(r29+136); r25:r24 = memd(r29+144) }
	{ dealloc_return }

;; deconv_check_ref: 0000EB04
deconv_check_ref proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00016E67) }
	{ r17 = r0; r16 = r1; r1 = 00000202 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000007)) jump:t 0000EB40; r2 = memw(r17+16) }

l0000EB2C:
	{ r2 = memw(r17+28); r1 = 00000203; r3 = add(PC,0000001B) }
	{ memw(r29) = r2; jump 0000EB54 }

l0000EB40:
	{ if (cmp.eq(r2.new,00000003)) jump:t 0000EB64; r2 = memw(r17+20); r1 = 00000204 }

l0000EB50:
	{ r3 = add(PC,00000013) }

l0000EB54:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0000EB64:
	{ r2 = r16; r1 = 00000205; r4 = add(PC,00016E52) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0000EB84
;;   Called from:
;;     0000E6EC (in deconv_execute_ref)
;;     0000E718 (in deconv_execute_ref)
;;     0000E748 (in deconv_execute_ref)
;;     0000E764 (in deconv_execute_ref)
;;     0000E778 (in deconv_execute_ref)
;;     0000E7A8 (in deconv_execute_ref)
;;     0000EAE8 (in deconv_execute_ref)
;;     0000EB18 (in deconv_check_ref)
;;     0000EB74 (in deconv_check_ref)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000EBA8; r5 = memw(r2+16) }

l0000EB94:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000003F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0000EBA8:
	{ dealloc_return }

l0000EBAC:
	{ nop }

;; errlog_function: 0000EBB0
;;   Called from:
;;     0000E7CC (in deconv_execute_ref)
;;     0000EB54 (in deconv_check_ref)
;;     0000EBAC (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00016D9F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0000EBD4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; deconv_f_execute_ref: 0000EBE0
deconv_f_execute_ref proc
	{ allocframe(000000B0); memd(r29+472) = r23:r22; r22 = r0 }
	{ memd(r29+168) = r17:r16; r2 = memw(r22+4) }
	{ r17 = memb(r22+32) }
	{ memd(r29+160) = r19:r18; r3 = memw(r22+8) }
	{ r4 = memw(r2+8); r16 = memw(r2); p0 = cmp.eq(r17,00000000) }
	{ memd(r29+152) = r21:r20; r19 = memw(r2+4); r21 = r1 }
	{ memd(r29+128) = r27:r26 }
	{ memd(r29+136) = r25:r24; r0 = memw(r16+8); r1 = p0 }
	{ r2 = memw(r19+8); r24 = memw(r16) }
	{ r20 = memw(r3); r7 = memw(r19+4) }
	{ r18 = memw(r4+8); r26 = memw(r19+12) }
	{ memw(r29+88) = r2; r27 = memw(r4+4) }
	{ memw(r29+32) = r7; r2 = memw(r19) }
	{ r7 = memw(r16+4); r6 = memw(r16+12) }
	{ memw(r29+120) = r0; memw(r29+80) = r4 }
	{ memw(r29+116) = r2; memw(r29+20) = r24 }
	{ memw(r29+92) = r7; memw(r29+100) = r6 }
	{ memw(r29+104) = r1; if (p0) jump:nt 0000EC74 }

l0000EC50:
	{ if (p0.new) jump:nt 0000EC68; p0 = cmp.eq(r9,00000002) }

l0000EC54:
	{ r23 = 00000000; if (!p0.new) jump:nt 0000EC80; p0 = cmp.eq(r9,00000001) }

l0000EC5C:
	{ r2 = memd(r29+120); r0 = r18 }
	{ jump 0000EC74; r0 += add(r2,FFFFFFFF) }

l0000EC68:
	{ r3 = memd(r29+32); r2 = memd(r29+120) }
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r18) }

l0000EC74:
	{ r1 = r18; call fn00009760 }
	{ r23 = r0 }

l0000EC80:
	{ p0 = cmp.eq(r17,00000002) }
	{ memb(r29+28) = r0.new; if (p0) jump:nt 0000ECC0; r0 = p0 }

l0000EC94:
	{ if (p0.new) r2 = memw(r29+92); if (p0.new) r1 = add(r27,00000000) }
	{ r1 = memd(r29+104); r0 = 00000000 }
	{ if (p0.new) r0 = memw(r29+92); if (p0.new) r1 = add(r27,00000000); if (!p0.new) jump:nt 0000ECD4; p0 = r1 }

l0000ECB0:
	{ jump 0000ECD0 }
0000ECB4             00 C0 61 70 E0 5F 02 E2 0C C0 00 58     ..ap._.....X

l0000ECC0:
	{ r3 = memd(r29+92); r2 = memd(r29+116); r1 = r27 }
	{ r2 = sub(r3,r2) }
	{ r0 = add(r2,r1) }

l0000ECD0:
	{ call fn00009760 }

l0000ECD4:
	{ r7 = memd(r29+32); r2 = memw(r20+16); r25 = r0 }
	{ memw(r29+84) = r20; r7 = memw(r16+16); r16 = add(r18,FFFFFFFF); r20 = sub(r18,r7) }
	{ memw(r29+56) = r2; r2 = memw(r19+16) }
	{ memw(r29+96) = r7; memw(r29+52) = r2 }

l0000ECF0:
	{ r0 = memd(r29+112); r3 = r23 }
	{ if (p0.new) r0 = add(r20,r3); if (p0.new) jump:nt 0000ED1C; p0 = r0 }

l0000ED00:
	{ if (p0.new) r0 = add(r16,r3); if (p0.new) jump:nt 0000ED1C; p0 = cmp.eq(r9,00000001) }

l0000ED08:
	{ r1 = memd(r29+104); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r18,r3); if (!p0.new) jump:nt 0000ED2C; p0 = r1 }

l0000ED18:
	{ jump 0000ED20 }

l0000ED1C:
	{ r1 = r18 }

l0000ED20:
	{ r19 = r3; call fn00009760 }
	{ r3 = r19 }

l0000ED2C:
	{ if (!cmp.eq(r2.new,r0)) jump:t 0000ECF0; r2 = memw(r29+120); r23 = add(r3,00000001) }

l0000ED3C:
	{ r2 = memd(r29+116); r23 = r3; r20 = r25 }
	{ memw(r29+108) = r19; r16 = sub(r19,r2) }

l0000ED4C:
	{ r0 = memw(r29+112) }
	{ if (!p0.new) r1:r0 = combine(r19,r20); if (p0.new) jump:nt 0000ED7C; p0 = r0 }

l0000ED5C:
	{ if (p0.new) jump:nt 0000ED74; p0 = cmp.eq(r9,00000001) }

l0000ED60:
	{ r1 = memd(r29+104); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r19,r20); if (!p0.new) jump:nt 0000ED88; p0 = r1 }

l0000ED70:
	{ jump 0000ED84 }

l0000ED74:
	{ jump 0000ED84; r0 += add(r19,FFFFFFFF) }

l0000ED7C:
	{ r1 = r19; r0 = add(r16,r20) }

l0000ED84:
	{ call fn00009760 }

l0000ED88:
	{ if (!cmp.eq(r2.new,r0)) jump:t 0000ED4C; r2 = memw(r29+92); r20 = add(r20,00000001) }

l0000ED98:
	{ r2 = memw(r22+28); r4 = add(PC,00000007) }
	{ memw(r29+4) = r2; r1 = 00000076 }
	{ r17 = r20; r2 = r21 }
	{ memw(r29) = r22; memw(r29+24) = r17; call logmsg_function }
	{ r7 = memw(r29+120); r25 = memw(r29+100); r1 = 00000077 }
	{ memw(r29) = r24; memw(r29+12) = r25; r4 = add(PC,00016DB9) }
	{ memw(r29+8) = r7; r3 = memd(r29+92); r2 = r21 }
	{ memw(r29+4) = r3 }
	{ call logmsg_function }
	{ r7 = memw(r29+32); r27 = memw(r29+88); r1 = 00000078 }
	{ memw(r29) = r26; memw(r29+12) = r27; r4 = add(PC,00016DA6) }
	{ memw(r29+8) = r7; r3 = memd(r29+116); r2 = r21 }
	{ memw(r29+4) = r3 }
	{ call logmsg_function }
	{ memw(r29+4) = r18; r1 = 00000079; r4 = add(PC,00016D9E) }
	{ memw(r29) = r19; r2 = r21 }
	{ call logmsg_function }
	{ r2 = r21; r1 = 0000007A; r4 = add(PC,00016D9A) }
	{ memb(r29) = r3.new; r3 = memb(r22+32); r16 = r25; call logmsg_function }
	{ memw(r29+12) = r26; r1 = 0000007B; r4 = add(PC,0000000C) }
	{ memw(r29) = r24; r2 = r21 }
	{ memw(r29+4) = r17; memw(r29+8) = r23 }
	{ call logmsg_function }
	{ if (!p0.new) r1 = 0000007D; p0 = cmp.eq(r16,r27); if (p0.new) jump:nt 0000EE8C; r2 = mpyi(r24,r26) }

l0000EE74:
	{ r3 = add(PC,00016D7B) }
	{ r2 = r21 }

l0000EE80:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0000F148 }

l0000EE8C:
	{ memb(r29+16) = r7.new; r3 = memw(r29+84); r7 = r23 }
	{ r4 = memw(r3+20) }
	{ r2 = mpyi(r2,r17) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 0000EEC4; r2 = asl(r2,00000002) }

l0000EEB0:
	{ memw(r29+4) = r2; r1 = 0000007F; r3 = add(PC,00000018) }
	{ memw(r29) = r4; r2 = r21 }
	{ jump 0000EE80 }

l0000EEC4:
	{ r4 = memw(r29+80) }
	{ if (cmp.eq(r3.new,00000001)) jump:t 0000EEE0; r3 = memw(r4) }

l0000EED4:
	{ r1 = 00000081; jump 0000EE80; r3 = add(PC,0000000E) }

l0000EEE0:
	{ if (cmp.eq(r3.new,00000001)) jump:t 0000EEFC; r3 = memw(r4+12); p0 = cmp.gt(r24,00000000) }

l0000EEF0:
	{ r1 = 00000082; jump 0000EE80; r3 = add(PC,00000003) }

l0000EEFC:
	{ memw(r29+16) = r21; r3 = memd(r29+84) }
	{ memw(r3+4) = r17; memw(r3+24) = r2 }
	{ memw(r3+8) = r7; memw(r3) = r24 }
	{ memw(r3+12) = r26; if (!p0) jump:nt 0000F124 }

l0000EF14:
	{ r6 = memd(r29+116); r3 = memd(r29+92); r2 = r19; r23 = mpyi(r16,r26) }
	{ r5 = memw(r29+120); r27 = memw(r29+32); r3 = add(r3,FFFFFFFF) }
	{ r5 = add(r5,FFFFFFFF); r4 = r27; r1 = mpyi(r27,r16); r2 = add(r6,mpyi(r2,r3)) }
	{ memb(r29+7) = r5.new; r5 = 00000000; r2 = add(r2,sub(00000041,r20)); r4 += mpyi(r18,r5) }
	{ r4 = sub(r4,r7) }
	{ memb(r29+26) = r3.new; r4 += lsr(r4,0000001F); r3 = mpyi(r1,r26) }
	{ memb(r29+12) = r6.new; r2 = asr(r2,00000001) }

l0000EF70:
	{ if (!p0.new) jump:nt 0000F10C; p0 = cmp.gt(r9,00000000) }

l0000EF74:
	{ r3 = memd(r29+28); r2 = memd(r29+92); r5 = 00000000 }
	{ memw(r29+112) = r5; r6 = memd(r29+24); r2 = mpyi(r3,r2) }
	{ memw(r29+84) = r2; r4 = mpyi(r3,r6) }
	{ memw(r29+40) = r4 }

l0000EF90:
	{ memw(r29+44) = r20; if (!p0.new) jump:nt 0000F0F8; p0 = cmp.gt(r7,00000000) }

l0000EF98:
	{ r4 = memd(r29+112); r2 = memd(r29+40); r6 = 00000000 }
	{ memw(r29+68) = r6; r3 = memd(r29+36); r2 = add(r4,r2) }
	{ r3 = add(r4,r3); r2 = mpyi(r2,r7) }
	{ memw(r29+60) = r2; memw(r29+88) = r3 }

l0000EFB4:
	{ p0 = cmp.gt(r26,00000000); if (!p0.new) jump:nt 0000F0E8 }

l0000EFBC:
	{ r2 = memd(r29+60); r4 = memd(r29+68) }
	{ r5 = memd(r29+52); r3 = memd(r29+48); r2 = add(r4,r2) }
	{ r3 = memd(r29+56); r4 = 00000000; r20 = add(r4,r3); r2 = mpyi(r2,r26) }
	{ r2 = addasl(r3,r2,00000002) }

l0000EFD8:
	{ memw(r29+72) = r2; r2 = memd(r29+116); r24 = 00000000 }
	{ r17 = r5; r21 = 00000000; p0 = cmp.gt(r2,00000000) }
	{ memw(r29+80) = r5; memw(r29+76) = r4; if (!p0) jump:nt 0000F0CC }

l0000EFF4:
	{ r2 = memd(r29+112); r1 = r19 }
	{ r0 = sub(r2,r21); call fn00009770 }
	{ if (!p0.new) jump:nt 0000F0B8; p0 = cmp.eq(r0,00000000) }

l0000F004:
	{ r2 = memd(r29+88); r1 = r19 }
	{ r0 = sub(r2,r21); call fn00009750 }
	{ if (!cmp.gt(r2.new,r0)) jump:nt 0000F0B8; r2 = memw(r29+92) }

l0000F01C:
	{ if (p0.new) r25 = 00000000 }
	{ p0 = cmp.gt(r27,00000000); if (!p0.new) jump:nt 0000F0B8 }

l0000F028:
	{ r3 = memd(r29+120); r2 = memd(r29+84); r19 = r17 }
	{ r2 = add(r0,r2) }
	{ memb(r29+25) = r2.new; r2 = mpyi(r2,r3) }

l0000F03C:
	{ r22 = sub(r20,r25) }
	{ r1:r0 = combine(r18,r22); call fn00009770 }
	{ if (!p0) r1:r0 = combine(r18,r22); if (!p0.new) jump:nt 0000F0AC; p0 = cmp.eq(r0,00000000) }

l0000F050:
	{ call fn00009750 }
	{ if (!cmp.gt(r2.new,r0)) jump:nt 0000F0AC; r2 = memw(r29+120) }

l0000F060:
	{ if (!p0.new) jump:nt 0000F0AC; p0 = cmp.gt(r8,00000000); r4 = addasl(r19,r26,00000002) }

l0000F068:
	{ r3 = memd(r29+96); r2 = memd(r29+100); p0 = cmp.gtu(r16,00000001); r6 = add(r16,FFFFFFFF) }
	{ r2 = add(r0,r2); loop0(0000F094,r6) }
	{ r2 = mpyi(r2,r16) }
	{ r3 = memw(r19); r2 = addasl(r3,r2,00000002) }
	{ r2 = memw(r2); r5 = add(r2,00000004) }
	{ if (!p0) jump:nt 0000F0A8 }

l0000F094:
	{ r3 = memw(r4); r2 = memw(r5); r4 = addasl(r4,r26,00000002); r24 += sfmpy(r2,r3) }
	{ nop; r5 = add(r5,00000004) }

l0000F0A8:
	{ r24 += sfmpy(r2,r3) }

l0000F0AC:
	{ if (!cmp.eq(r25.new,r27)) jump:t 0000F03C; r25 = add(r25,00000001); r19 = addasl(r19,r23,00000002) }

l0000F0B8:
	{ r7 = memd(r29+116); r2 = memd(r29+104) }

l0000F0BC:
	{ r19 = memw(r29+108) }
	{ if (!cmp.eq(r21.new,r7)) jump:t 0000EFF4; r21 = add(r21,00000001); r17 = addasl(r17,r2,00000002) }

l0000F0CC:
	{ r4 = memd(r29+76); r2 = memd(r29+72) }

l0000F0D0:
	{ r5 = memd(r29+80); r4 = r4 }
	{ memw(r2) = r24; p0 = cmp.eq(r4,r26); r2 = add(r2,00000004); r5 = add(r5,00000004) }
	{ if (!p0) jump:nt 0000EFD8 }

l0000F0E8:
	{ r7 = memd(r29+64); r2 = memd(r29+68) }
	{ r2 = add(r2,00000001) }
	{ memw(r29+68) = r2; if (!p0.new) jump:nt 0000EFB4; p0 = cmp.eq(r7,r2) }

l0000F0F8:
	{ r20 = memd(r29+44); r3 = memd(r29+112) }
	{ r3 = add(r3,00000001); r2 = add(r3,00000002) }
	{ memw(r29+112) = r3; if (!p0.new) jump:nt 0000EF90; p0 = cmp.eq(r2,r12) }

l0000F10C:
	{ r24 = memw(r29+20); r2 = memw(r29+28) }
	{ r17 = memd(r29+24); r2 = r2 }
	{ memw(r29+28) = r2; p0 = cmp.eq(r2,r24); if (!p0.new) jump:nt 0000EF70 }

l0000F124:
	{ memw(r29+12) = r26; r1 = 000000B3; r4 = add(PC,00016B1C) }
	{ memw(r29) = r24; r2 = memw(r29+16) }
	{ memw(r29+4) = r17; memw(r29+8) = r7 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0000F148:
	{ r19:r18 = memd(r29+160); r17:r16 = memd(r29+168) }
	{ r23:r22 = memd(r29+144); r21:r20 = memd(r29+152) }
	{ r27:r26 = memd(r29+128); r25:r24 = memd(r29+136) }
	{ dealloc_return }
0000F15C                                     00 C0 00 7F             ....

;; deconv_check_ref: 0000F160
deconv_check_ref proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00016998) }
	{ r17 = r0; r16 = r1; r1 = 000000BA }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000003)) jump:t 0000F19C; r2 = memw(r17+16) }

l0000F188:
	{ r2 = memw(r17+28); r1 = 000000BB; r3 = add(PC,0000000C) }
	{ memw(r29) = r2; jump 0000F1B0 }

l0000F19C:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000F1C0; r2 = memw(r17+20); r1 = 000000BC }

l0000F1AC:
	{ r3 = add(PC,00000004) }

l0000F1B0:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0000F1C0:
	{ r2 = r16; r1 = 000000BD; r4 = add(PC,00016983) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0000F1E0
;;   Called from:
;;     0000EDAC (in deconv_f_execute_ref)
;;     0000EDDC (in deconv_f_execute_ref)
;;     0000EE08 (in deconv_f_execute_ref)
;;     0000EE20 (in deconv_f_execute_ref)
;;     0000EE34 (in deconv_f_execute_ref)
;;     0000EE60 (in deconv_f_execute_ref)
;;     0000F140 (in deconv_f_execute_ref)
;;     0000F174 (in deconv_check_ref)
;;     0000F1D0 (in deconv_check_ref)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000F204; r5 = memw(r2+16) }

l0000F1F0:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000002E) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0000F204:
	{ dealloc_return }

;; errlog_function: 0000F208
;;   Called from:
;;     0000EE80 (in deconv_f_execute_ref)
;;     0000F1B0 (in deconv_check_ref)
;;     0000F1F8 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000168D2) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0000F22C                                     00 00 00 00             ....

;; logsoftmax_execute: 0000F230
logsoftmax_execute proc
	{ allocframe(+00000048) }
	{ r3 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+64) = r17:r16; memd(r29+56) = r19:r18 }
	{ r19 = memw(r3); r5 = memw(r2); r2 = r1 }
	{ memd(r29+40) = r23:r22; memd(r29+48) = r21:r20 }
	{ r4 = memw(r19+24); r7 = memw(r5+20) }
	{ memd(r29+24) = r27:r26; memd(r29+32) = r25:r24; if (!p0.new) r26 = 00000000; p0 = cmp.gtu(r4,r7) }
	{ if (p0) r1 = 00000039; if (p0) jump:nt 0000F3AC }

l0000F264:
	{ r2 = memw(r19+4); r4 = memw(r19) }
	{ memw(r29+8) = r2; r3 = memw(r19+8); r2 = mpyi(r2,r4) }
	{ r22 = memw(r19+12) }
	{ memw(r29) = r4; memw(r29+12) = r5 }
	{ memw(r29+4) = r3 }
	{ if (!cmp.gt(r24.new,00000000)) jump:nt 0000F37C; r25 = memw(r19+16); r24 = mpyi(r2,r3) }

l0000F28C:
	{ r2 = memd(r29+12); r16 = 00000000 }
	{ r27 = memw(r2+16) }

l0000F294:
	{ r2 = memw(r25); p0 = cmp.gt(r22,00000000) }
	{ memb(r29+4) = r0.new; if (!p0) jump:nt 0000F2FC; r0 = p0 }

l0000F2AC:
	{ r21 = r2; r4 = r2; r3 = r20 }
	{ r5 = add(r3,00000004); r21 = sfmax(r4,r21) }
	{ jump 0000F2C8 }
0000F2C0 04 C0 83 91 F8 C3 35 17                         ......5.        

l0000F2C8:
	{ r0 = memd(r29+16); r18 = r20; r17 = r16; r23 = r22 }
	{ if (!p0.new) jump:nt 0000F2FC; p0 = r0 }

l0000F2DC:
	{ call fn00009780; r0 = sfsub(r2,r21) }
	{ if (cmp.eq(r23.new,00000000)) jump:nt 0000F308; r23 = add(r23,FFFFFFFF); r3 = add(r18,00000004); r17 = sfadd(r17,r0) }

l0000F2F8:
	{ jump 0000F2DC; r10 = r3 }

l0000F2FC:
	{ r0 = r16; call fn00009790 }
	{ jump 0000F368 }

l0000F308:
	{ r0 = r17; call fn00009790 }
	{ r2 = memw(r29+16) }
	{ if (!p0.new) jump:nt 0000F368; p0 = r2 }

l0000F31C:
	{ r5 = r22; r3 = 00000000; p0 = cmp.gtu(r22,00000001) }
	{ r3 = add(r27,r3); r2 = add(r3,00000004); r4 = add(r25,r3); loop0(0000F340,r5) }
	{ r4 = memw(r4) }
	{ if (!p0) jump:nt 0000F360; r4 = sfsub(r4,r21) }

l0000F340:
	{ memb(r3) = r4.new; r6 = add(r2,00000004); r5 = add(r25,r2); r4 = sfsub(r4,r0) }
	{ r7 = memw(r5); r2 = r6 }
	{ nop; r4 = sfsub(r7,r21) }

l0000F360:
	{ memb(r3) = r2.new; r2 = sfsub(r4,r0) }

l0000F368:
	{ r20 = addasl(r20,r22,00000002); r27 = addasl(r27,r22,00000002) }

l0000F36C:
	{ r20 = addasl(r20,r22,00000002) }
	{ if (!cmp.eq(r26.new,r24)) jump:t 0000F294; r26 = add(r26,00000001); r25 = addasl(r25,r22,00000002) }

l0000F37C:
	{ r2 = memd(r29); r3 = memd(r29+12); r0 = 00000000 }

l0000F380:
	{ r2 = memd(r29); r3 = memd(r29+12) }

l0000F384:
	{ r6 = memd(r29+8); r7 = memd(r29+4) }
	{ memw(r3) = r2; memw(r3+8) = r7 }
	{ memw(r3+4) = r6; memw(r3+12) = r22 }
	{ r2 = memw(r19+24) }
	{ memw(r3+24) = r2 }

l0000F398:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }

l0000F3AC:
	{ call errlog_function; r3 = add(PC,00016924) }
	{ r0 = FFFFFFFF; jump 0000F398 }

;; logsoftmax_check: 0000F3C0
logsoftmax_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000168C9) }
	{ r17 = r0; r16 = r1; r1 = 00000053 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000F400; r2 = memw(r17+16); r1 = 00000054 }

l0000F3EC:
	{ r3 = add(PC,0000003D) }
	{ r2 = r16; call errlog_function }

l0000F3F4:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0000F400:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000F418; r2 = memw(r17+20); r1 = 00000055 }

l0000F410:
	{ jump 0000F3F4; r3 = add(PC,00000019) }

l0000F418:
	{ r2 = r16; r1 = 00000056; r4 = add(PC,0001689C) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0000F438
;;   Called from:
;;     0000F3D4 (in logsoftmax_check)
;;     0000F428 (in logsoftmax_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000F45C; r5 = memw(r2+16) }

l0000F448:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000025) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0000F45C:
	{ dealloc_return }

;; errlog_function: 0000F460
;;   Called from:
;;     0000F3AC (in logsoftmax_execute)
;;     0000F3F0 (in logsoftmax_check)
;;     0000F450 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00016809) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0000F484             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; expanddims_execute: 0000F490
expanddims_execute proc
	{ allocframe(+00000000) }
	{ r2 = memw(r0+4); r4 = memw(r0+8) }
	{ r4 = memw(r4); r3 = memw(r2) }
	{ r5 = memw(r3+4); r7 = memw(r3) }
	{ memw(r4) = r7; memw(r4+4) = r5 }
	{ r0 = memw(r3+12); r2 = memw(r3+8) }
	{ memw(r4+12) = r0 }
	{ memw(r4+8) = r2; r2 = r1 }
	{ r5 = memw(r3+24) }
	{ if (cmp.gtu(r5,r6.new)) jump:t 0000F4CC; r6 = memw(r4+20) }

l0000F4C0:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }
	{ dealloc_return; r0 = 00000000 }

l0000F4CC:
	{ r1 = 00000030; call errlog_function; r3 = add(PC,0001688B) }
	{ dealloc_return; r0 = -00000001 }

;; expanddims_check: 0000F4E0
expanddims_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0001681E) }
	{ r17 = r0; r16 = r1; r1 = 00000037 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 0000F524; r3 = 00000004 }

l0000F50C:
	{ r1 = 00000038; r3 = add(PC,00000013) }
	{ r2 = r16; call errlog_function }

l0000F518:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0000F524:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000F53C; r2 = memw(r17+20); r1 = 00000039 }

l0000F534:
	{ jump 0000F518; r3 = add(PC,0000003A) }

l0000F53C:
	{ r2 = r16; r1 = 0000003A; r4 = add(PC,000167FE) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0000F558
;;   Called from:
;;     0000F4F4 (in expanddims_check)
;;     0000F548 (in expanddims_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000F57C; r5 = memw(r2+16) }

l0000F568:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000003A) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0000F57C:
	{ dealloc_return }

;; errlog_function: 0000F580
;;   Called from:
;;     0000F4CC (in expanddims_execute)
;;     0000F514 (in expanddims_check)
;;     0000F570 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0001675E) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0000F5A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; sslice_execute_4b: 0000F5B0
sslice_execute_4b proc
	{ jump strided_slice_impl; r4 = 00000004 }

;; sslice_check: 0000F5B4
sslice_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000167D3) }
	{ r17 = r0; r16 = r1; r1 = 00000070 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0000F5E8; r2 = memw(r17+16); r1 = 00000071 }

l0000F5E0:
	{ jump 0000F604; r3 = add(PC,00000002) }

l0000F5E8:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000F60C; r2 = memw(r17+20); r0 = 00000000; r1 = 00000072 }

l0000F5FC:
	{ r3 = add(PC,00000031) }
	{ r2 = r16; call errlog_function }

l0000F604:
	{ r2 = r16 }

l0000F608:
	{ r0 = FFFFFFFF }

l0000F60C:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; sslice_execute_1b: 0000F610
sslice_execute_1b proc
	{ jump strided_slice_impl; r1 = 00000001 }

;; sslice_execute_q8: 0000F614
sslice_execute_q8 proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ r4 = memw(r16+8); r2 = memw(r16+4) }
	{ r2 = memw(r4+4); r3 = memw(r2+16) }
	{ r5 = memw(r3+4); r7 = memw(r3) }
	{ memw(r2) = r7; memw(r2+4) = r5 }
	{ r0 = memw(r3+12); r4 = memw(r3+8) }
	{ memw(r2+8) = r4; memw(r2+12) = r0 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0000F64C; r6 = memw(r2+20) }

l0000F644:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l0000F64C:
	{ r3 = memw(r16+4); r2 = memw(r16+8) }
	{ r2 = memw(r2+8); r3 = memw(r3+20) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0000F67C; r6 = memw(r2+20) }

l0000F674:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l0000F67C:
	{ deallocframe; r17:r16 = memd(r29); r2 = 00000001; r1:r0 = combine(r17,r16) }
	{ jump strided_slice_impl }
0000F68C                                     00 C0 00 7F             ....

;; sslice_check_q8: 0000F690
sslice_check_q8 proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000166F7) }
	{ r17 = r0; r16 = r1; r1 = 00000078 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0000F6C4; r2 = memw(r17+16); r1 = 00000079 }

l0000F6BC:
	{ jump 0000F6E0; r3 = add(PC,00000026) }

l0000F6C4:
	{ if (cmp.eq(r2.new,00000003)) jump:t 0000F6E8; r2 = memw(r17+20); r0 = 00000000; r1 = 0000007A }

l0000F6D8:
	{ r3 = add(PC,00000015) }
	{ r2 = r16; call errlog_function }

l0000F6E0:
	{ r2 = r16 }

l0000F6E4:
	{ r0 = FFFFFFFF }

l0000F6E8:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0000F6EC
;;   Called from:
;;     0000F5C8 (in sslice_check)
;;     0000F6A4 (in sslice_check_q8)
;;     0000F840 (in strided_slice_impl)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000F710; r5 = memw(r2+16) }

l0000F6FC:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000002D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0000F710:
	{ dealloc_return }

;; errlog_function: 0000F714
;;   Called from:
;;     0000F600 (in sslice_check)
;;     0000F6DC (in sslice_check_q8)
;;     0000F704 (in logmsg_function)
;;     0000F810 (in strided_slice_impl)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00016651) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; strided_slice_impl: 0000F738
;;   Called from:
;;     0000F5B0 (in sslice_execute_4b)
;;     0000F610 (in sslice_execute_1b)
;;     0000F688 (in sslice_execute_q8)
strided_slice_impl proc
	{ allocframe(00000048); memd(r29+488) = r19:r18; r19 = r0 }
	{ memd(r29+64) = r17:r16; r3 = memw(r19+4); r17 = r2 }
	{ memd(r29+48) = r21:r20; r21 = r1 }
	{ memd(r29+32) = r25:r24 }
	{ memd(r29+40) = r23:r22; r5 = memw(r3+8) }
	{ r7 = memw(r3+12); r6 = memw(r3+4) }
	{ r0 = memw(r5+16); r22 = memw(r3) }
	{ r6 = memw(r7+16); r5 = memw(r6+16) }
	{ r4 = memw(r19+8) }
	{ r24 = memw(r0); r16 = memw(r5) }
	{ memd(r29+24) = r27:r26; r20 = memw(r6); r2 = sub(FFFFFFFF,r16) }
	{ r23 = memw(r4); r3 = memw(r22+12); r1:r0 = combine(r20,r20) }
	{ r27 = memw(r22+8); r26 = memw(r22+4); r0 += add(r24,r2) }
	{ memw(r29+20) = r3; call fn00009750 }
	{ r2 = memw(r23+20); r18 = r0; r3 = add(PC,00016621) }
	{ if (cmp.gtu(r25.new,r2)) jump:t 0000F810; r1 = 00000047; r25 = mpyi(r18,r17) }

l0000F7B0:
	{ r1 = 00000048; p0 = cmp.gt(r26,00000001); r3 = add(PC,00000017) }
	{ if (cmp.gt(r2.new,00000001)) jump:t 0000F810; r2 = memw(r22) }

l0000F7C8:
	{ r1 = 00000049; r3 = add(PC,0000003F) }
	{ if (!p0) r1 = 0000004A; if (p0) jump:nt 0000F810 }

l0000F7D8:
	{ if (!p0.new) r1 = 0000004B; p0 = cmp.gt(r27,00000001); r3 = add(PC,000165EB) }
	{ if (p0) jump:nt 0000F810 }

l0000F7EC:
	{ r3 = add(PC,000165F7) }
	{ if (!cmp.gt(r2.new,r16)) jump:t 0000F810; r2 = memw(r29+20) }

l0000F800:
	{ r1 = 0000004C; r3 = add(PC,00000031) }
	{ if (!cmp.gtu(r24,r2.new)) jump:t 0000F81C; r2 = memw(r29+20) }

l0000F810:
	{ r19 = -00000001; r2 = r21; call errlog_function }

l0000F814:
	{ r19 = -00000001; r2 = r21 }

l0000F818:
	{ jump 0000F884 }

l0000F81C:
	{ r22 = memw(r23+16); r26 = memw(r22+16); r4 = add(PC,000165DA) }
	{ memw(r29+8) = r24; memw(r29+12) = r20; r2 = r21; r1 = 0000004F }
	{ memw(r29+4) = r16 }
	{ memw(r29) = r19; r19 = 00000000; call logmsg_function }
	{ memw(r23) = 00000001; memw(r23+24) = r25; p0 = cmp.gt(r18,00000000) }
	{ memw(r23+4) = FFFFFF81 }
	{ memw(r23+12) = r18; memw(r23+8) = 00000001; if (!p0) jump:nt 0000F884 }

l0000F864:
	{ r20 = mpyi(r20,r17); r16 = add(r26,mpyi(r16,r17)) }

l0000F86C:
	{ r22 = add(r22,r17); r2 = r17; r1:r0 = combine(r16,r22); call fn00009560 }
	{ if (!cmp.eq(r18.new,00000000)) jump:t 0000F86C; r18 = add(r18,FFFFFFFF); r16 = add(r16,r20) }

l0000F884:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64); r0 = r19 }

l0000F888:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }
0000F89C                                     00 00 00 00             ....

;; resizenear_f_execute: 0000F8A0
resizenear_f_execute proc
	{ allocframe(+00000068) }
	{ r3 = memw(r0+8); r7 = memw(r0+4) }
	{ memd(r29+64) = r25:r24; memd(r29+80) = r21:r20 }
	{ r21 = memw(r7); r4 = memw(r7+4) }
	{ memd(r29+88) = r19:r18; memd(r29+56) = r27:r26; r27 = r1 }
	{ r25 = memw(r21+8); r2 = memw(r4+16) }
	{ memd(r29+96) = r17:r16; memd(r29+72) = r23:r22 }
	{ r20 = memw(r3); r24 = memw(r2+4); r0 = convert_w2sf(r25) }
	{ r26 = memw(r21+4); r18 = memw(r21) }
	{ r19 = memw(r2); r23 = memw(r21+12); call fn00009610; r1 = convert_w2sf(r24) }
	{ r16 = r0; r2 = convert_w2sf(r26) }
	{ r0 = r2; call fn00009610; r1 = convert_w2sf(r19) }
	{ r3 = memw(r20+20); r1 = 00000044; r17 = asl(r23,00000002); r2 = mpyi(r24,r19) }
	{ r2 = mpyi(r2,r18) }
	{ if (!cmp.gtu(r22.new,r3)) jump:t 0000F934; r22 = mpyi(r2,r17) }

l0000F920:
	{ r2 = r27; r3 = add(PC,00000022) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0000FA44 }

l0000F934:
	{ r21 = memw(r20+16); r26 = memw(r21+16); r2 = r27; r3 = r26 }
	{ memw(r29+28) = r23; r1 = 00000045; r4 = add(PC,00016548) }
	{ memw(r29+24) = r24; memw(r29+48) = r0 }
	{ memw(r29+20) = r19; memw(r29+36) = r18 }
	{ memw(r29+16) = r18; memw(r29+32) = r3 }
	{ memw(r29+4) = r3; memw(r29+12) = r23 }
	{ memw(r29) = r18; memw(r29+8) = r25 }
	{ call logmsg_function }
	{ memw(r20) = r18; memw(r20+24) = r22; r3 = r19; r0 = 00000000 }
	{ memw(r20+12) = r23; memw(r20+4) = r19; p0 = cmp.gt(r18,00000000) }
	{ memw(r20+8) = r24 }
	{ if (!p0) jump:nt 0000FA44 }

l0000F990:
	{ r4 = memd(r29+48); r5 = 00000000; r2 = mpyi(r24,r23) }
	{ r2 = asl(r2,00000002) }
	{ memw(r29+44) = r2 }

l0000F9A0:
	{ if (p0.new) memw(r29+40) = r5; jump 0000FA34; if (p0.new) jump:nt 0000F9AC; p0 = cmp.gt(r3,00000000) }
0000F9AC                                     14 40 00 78             .@.x
0000F9B0 A5 28 82 DC 02 C2 05 ED 0D C2 9D A1 02 40 54 8B .(...........@T.
0000F9C0 1B 40 75 70 00 C0 58 75 02 C2 44 EB 22 40 62 8B .@up..Xu..D."@b.
0000F9D0 30 C0 20 5C 13 40 75 70 D3 3C 3E 50 03 40 79 70 0. \.@up.<>P.@yp
0000F9E0 32 38 0D 28 19 43 02 ED 12 C0 63 70 02 40 55 8B 28.(.C....cp.@U.
0000F9F0 00 C0 73 70 02 C2 50 EB 23 40 62 8B 02 C0 71 70 ..sp..P.#@b...qp
0000FA00 03 D9 03 F3 03 D7 03 ED 41 5A 03 C4 AC 4D FF 5B ........AZ...M.[
0000FA10 13 D1 13 F3 35 40 15 B0 EC F8 72 20 15 40 7B 70 ....5@....r .@{p
0000FA20 19 40 72 70 B2 3C E3 50 C4 3C 2D 58 34 40 14 B0 .@rp.<.P.<-X4@..
0000FA30 C8 E3 72 20                                     ..r             

l0000FA34:
	{ r2 = memd(r29+36); r5 = memd(r29+40) }
	{ if (!cmp.eq(r5.new,r2)) jump:t 0000F9A0; r5 = add(r5,00000001) }

l0000FA44:
	{ r19:r18 = memd(r29+88); r17:r16 = memd(r29+96) }
	{ r23:r22 = memd(r29+72); r21:r20 = memd(r29+80) }
	{ r27:r26 = memd(r29+56); r25:r24 = memd(r29+64) }
	{ dealloc_return }
0000FA58                         00 40 00 7F 00 C0 00 7F         .@......

;; resizenear_f_check: 0000FA60
resizenear_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000163D5) }
	{ r17 = r0; r16 = r1; r1 = 0000005B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0000FAA0; r2 = memw(r17+16); r1 = 0000005C }

l0000FA8C:
	{ r3 = add(PC,00000000) }
	{ r2 = r16; call errlog_function }

l0000FA94:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0000FAA0:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0000FAB8; r2 = memw(r17+20); r1 = 0000005D }

l0000FAB0:
	{ jump 0000FA94; r3 = add(PC,0000002B) }

l0000FAB8:
	{ r2 = r16; r1 = 0000005E; r4 = add(PC,000163AF) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0000FAD8
;;   Called from:
;;     0000F970 (in resizenear_f_execute)
;;     0000FA74 (in resizenear_f_check)
;;     0000FAC8 (in resizenear_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0000FAFC; r5 = memw(r2+16) }

l0000FAE8:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000031) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0000FAFC:
	{ dealloc_return }

;; errlog_function: 0000FB00
;;   Called from:
;;     0000F928 (in resizenear_f_execute)
;;     0000FA90 (in resizenear_f_check)
;;     0000FAF0 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00016315) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0000FB24             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; mirrorpad_f_execute: 0000FB30
mirrorpad_f_execute proc
	{ allocframe(+000000C0); r4 = add(PC,000163F2) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+176) = r19:r18; memd(r29+152) = r25:r24 }
	{ r2 = memw(r2); r24 = memw(r2+4) }
	{ memd(r29+144) = r27:r26; r18 = memw(r3) }
	{ r7 = memw(r2+16); r6 = memw(r24+16) }
	{ r25 = memw(r2+12); r5 = memw(r2) }
	{ r27 = memw(r2+4); r19 = memw(r2+8) }
	{ memd(r29+184) = r17:r16; r2 = memw(r6+20); r1 = 00000085; r17 = r1 }
	{ memd(r29+160) = r23:r22; memd(r29+168) = r21:r20 }
	{ memw(r29+124) = r7; r7 = memw(r18+16) }
	{ memw(r29+136) = r2; r2 = memw(r6+16) }
	{ r21 = memw(r6+28); r20 = memw(r6+24) }
	{ r26 = memw(r6+8); r16 = memw(r6+12) }
	{ r23 = memw(r6); r22 = memw(r6+4) }
	{ memw(r29+140) = r2; memw(r29+12) = r25; r2 = r17 }
	{ memw(r29+120) = r0; memw(r29+8) = r19 }
	{ memw(r29+132) = r5; memw(r29+4) = r27 }
	{ memw(r29) = r5; memw(r29+128) = r7 }
	{ call logmsg_function }
	{ r1 = 00000086; r3 = add(PC,00016379) }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0000FCC4; r2 = memw(r24+12) }

l0000FBD8:
	{ r1 = 00000087; r3 = add(PC,00000025) }
	{ if (!cmp.eq(r2.new,00000004)) jump:t 0000FCC4; r2 = memw(r24+8) }

l0000FBEC:
	{ r1 = 00000088; r3 = add(PC,0000001F) }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0000FCC4; r2 = or(r21,r20) }

l0000FC00:
	{ r1 = 00000089; r3 = add(PC,0000001D) }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0000FCC4; r2 = or(r22,r23) }

l0000FC14:
	{ if (p0.new) memw(r29) = r26; if (!p0.new) r2 = add(r17,00000000); p0 = cmp.gt(r19,r26) }
	{ memw(r29+4) = r19; r1 = 0000008A; r3 = add(PC,00016349) }
	{ jump 0000FCC8 }
0000FC34             12 48 8B 14 0B 40 7A 70 09 C0 70 70     .H...@zp..pp
0000FC40 8C 45 00 00 83 54 49 6A 61 51 00 78 01 D3 9D A1 .E...TIjaQ.x....
0000FC50 08 28 92 70 3A C0 00 58 81 D1 00 78 8C 45 00 00 .(.p:..X...x.E..
0000FC60 03 53 49 6A 62 44 9D 91 34 FB C2 21 8C 45 00 00 .SIjbD..4..!.E..
0000FC70 03 4B 49 6A 04 59 13 F5 A1 D1 00 78 25 4B 09 EF .KIj.Y.....x%K..
0000FC80 24 57 16 EF 42 44 9D 91 24 FB C2 21 03 45 04 ED $W..BD..$..!.E..
0000FC90 C1 51 00 78 4A 44 9D 91 62 C4 9D 91 2A 42 1B EF .Q.xJD..b...*B..
0000FCA0 26 44 9D 91 A8 C0 92 91 26 D4 15 EF 8B 45 00 00 &D......&....E..
0000FCB0 83 53 49 6A 07 CA 03 ED 07 C6 07 ED 47 42 07 8C .SIj........GB..
0000FCC0 0C E8 42 21                                     ..B!            

l0000FCC4:
	{ r2 = r17 }

l0000FCC8:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0000FF10 }
0000FCD4             00 40 00 78 23 44 9D 91 00 C6 92 A1     .@.x#D......
0000FCE0 00 40 43 75 83 7F 0F 7E A5 02 A7 A6 07 49 13 FB .@Cu...~.....I..
0000FCF0 0F 40 00 7E 01 4A 92 A1 03 C4 92 A1 0A 60 20 5C .@.~.J.......` \
0000FD00 E8 5F 0F 7E 6D 44 9D 41 E0 C9 9D 40 0E 59 13 ED ._.~mD.A...@.Y..
0000FD10 51 42 19 8C 06 4D 1B F3 0C C4 9D 91 02 51 05 ED QB...M.......Q..
0000FD20 04 44 05 ED 1A 40 59 76 E9 C3 9D 91 16 45 19 ED .D...@Yv.....E..
0000FD30 45 43 13 C4 17 40 6C 70 1C C0 00 78 07 51 07 ED EC...@lp...x.Q..
0000FD40 43 43 06 C4 E0 7F EB BF 1D DB 9D A1 01 4B 11 ED CC...........K..
0000FD50 06 46 02 ED 1A 4E 9D A1 14 C9 9D A1 12 53 11 ED .F...N.......S..
0000FD60 47 42 0E 8C 90 08 77 E9 21 48 0D C4 18 41 9D A1 GB....w.!H...A..
0000FD70 1B CB 9D A1 4F 42 19 8E 47 42 16 8C 19 C7 9D A1 ....OB..GB......
0000FD80 04 4A 04 ED 0C 56 03 E3 06 40 56 76 07 C6 9D A1 .J...V...@Vv....
0000FD90 09 59 05 E3 0A 4F 9D A1 0C C4 9D A1 01 5B 0E ED .Y...O.......[..
0000FDA0 81 28 E8 DD 07 4D 02 ED 02 40 42 76 E2 08 D7 E8 .(...M...@Bv....
0000FDB0 67 08 F1 E8 52 08 B6 E8 02 5C 04 ED 03 C4 30 91 g...R....\....0.
0000FDC0 53 29 D7 DD 00 40 47 75 1B 60 11 74 03 44 9D 91 S)...@Gu.`.t.D..
0000FDD0 12 DC 9D A1 42 43 02 C4 18 40 00 7E 1E 09 0F E9 ....BC...@.~....
0000FDE0 13 59 9D A1 16 C2 9D A1 28 C0 20 5C 27 1D 52 3D .Y......(. \'.R=
0000FDF0 60 40 02 DD A4 1C F3 3C 66 1D 65 3C 02 47 03 ED `@.....<f.e<.G..
0000FE00 23 40 00 7A F7 3D 74 5A 15 C5 06 F3 54 47 02 C4 #@.z.=tZ....TG..
0000FE10 37 1D 92 3C 13 44 07 F3 43 3D 32 58 03 DB 02 E3 7..<.D..C=2X....
0000FE20 16 5B 15 F5 62 C3 9D 91 19 C0 62 70 7C 40 82 10 .[..b.....bp|@..
0000FE30 90 40 00 58 16 E0 95 74 62 44 9D 91 17 C2 9D 91 .@.X...tbD......
0000FE40 00 40 42 75 D8 41 9D 91 BB C1 9D 91 24 40 20 5C .@Bu.A......$@ \
0000FE50 15 40 7B 70 76 41 9D 91 D4 C2 9D 41 83 1C 52 3D .@{pvA.....A..R=
0000FE60 73 C4 9D 91 60 40 02 DD 22 60 00 7E 02 E0 80 7E s...`@.."`.~...~
0000FE70 02 C2 03 F3 17 D5 02 E3 74 4B FF 5B 00 54 15 F5 ........tK.[.T..
0000FE80 02 40 78 70 F3 FF F3 BF 55 55 16 C4 F8 40 BB 10 .@xp....UU...@..
0000FE90 14 D8 14 F3 42 44 9D 91 79 C2 9D 91 36 C2 9D 91 ....BD..y...6...
0000FEA0 1E 40 C2 10 53 C4 9D 43 57 1C 52 3D 63 1D 76 3C .@..S..CW.R=c.v<
0000FEB0 60 40 02 DD 14 46 03 F3 07 E0 80 7E 15 C7 16 F3 `@...F.....~....
0000FEC0 50 4B FF 5B 02 40 78 70 00 54 15 F5 15 D5 3B F3 PK.[.@xp.T....;.
0000FED0 14 58 14 F3 F3 7F F3 BF F8 E0 72 24 F3 1C C4 3C .X........r$...<
0000FEE0 5C 42 9D 91 82 C2 9D 91 56 56 04 C4 57 57 04 C4 \B......VV..WW..
0000FEF0 3C 40 1C B0 27 C4 9D 91 59 59 03 C4 42 42 03 C4 <@..'...YY..BB..
0000FF00 00 47 1C F2 14 D4 BD A1 58 60 FF 5C 00 C0 00 7E .G......X`.\...~

l0000FF10:
	{ r19:r18 = memd(r29+176); r17:r16 = memd(r29+184) }
	{ r23:r22 = memd(r29+160); r21:r20 = memd(r29+168) }
	{ r27:r26 = memd(r29+144); r25:r24 = memd(r29+152) }
	{ dealloc_return }
0000FF24             1E 4B FF 5B 00 57 16 F5 02 40 71 70     .K.[.W...@qp
0000FF30 F9 FF F9 BF 56 56 1A C4 F8 68 DF 5C 17 51 17 F3 ....VV...h.\.Q..
0000FF40 00 C0 59 75 02 C3 9D 91 16 C2 15 F3 0A 4B FF 5B ..Yu.........K.[
0000FF50 02 40 72 70 00 D6 14 F5 15 52 16 F3 C2 3D BF 50 .@rp.....R...=.P
0000FF60 19 40 62 70 00 C0 42 75 14 C0 20 5C FA 4A FF 5B .@bp..Bu.. \.J.[
0000FF70 02 40 71 70 00 55 17 F5 17 D7 31 F3 15 51 15 F3 .@qp.U....1..Q..
0000FF80 F9 7F F9 BF F8 E0 72 24 E2 C2 9D 91 15 C2 16 F3 ......r$........
0000FF90 38 40 18 B0 14 52 14 F3 A7 1D 92 3D 13 C2 13 F3 8@...R.....=....
0000FFA0 5B 5B 07 C4 A2 43 9D 91 40 F8 72 20 46 FF FF 59 [[...C..@.r F..Y

;; mirrorpad_f_check: 0000FFB0
mirrorpad_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00015F17) }
	{ r17 = r0; r16 = r1; r1 = 000000A7 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memb(r17+32) }
	{ r2 = add(r2,FFFFFFFD) }
	{ if (p0.new) r1 = 000000AA; if (!p0.new) jump:nt 0000FFF8; p0 = cmpb.gtu(r2,01) }

l0000FFE0:
	{ r3 = add(PC,00015EF9) }
	{ r2 = r16; call errlog_function }

l0000FFEC:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0000FFF8:
	{ if (cmp.eq(r2.new,00000002)) jump:t 00010010; r2 = memw(r17+16); r1 = 000000AC }

l00010008:
	{ jump 0000FFEC; r3 = add(PC,00000029) }

l00010010:
	{ if (cmp.eq(r2.new,00000001)) jump:t 00010028; r2 = memw(r17+20); r1 = 000000AD }

l00010020:
	{ jump 0000FFEC; r3 = add(PC,00000020) }

l00010028:
	{ r2 = r16; r1 = 000000AE; r4 = add(PC,00015EE4) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00010048
;;   Called from:
;;     0000FBBC (in mirrorpad_f_execute)
;;     0000FFC4 (in mirrorpad_f_check)
;;     00010038 (in mirrorpad_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001006C; r5 = memw(r2+16) }

l00010058:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000014) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001006C:
	{ dealloc_return }

;; errlog_function: 00010070
;;   Called from:
;;     0000FCC8 (in mirrorpad_f_execute)
;;     0000FFE8 (in mirrorpad_f_check)
;;     00010060 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00015E38) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00010094             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; add_int32_execute: 000100A0
add_int32_execute proc
	{ allocframe(000000A0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = 00000000 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,00000001); p0 = cmp.eq(r6,00000001) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,00000001); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,00000001); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,00000002) }
	{ if (p0) jump:nt 0001014C }

l00010148:
	{ r22 = mpyi(r7,r12) }

l0001014C:
	{ r2 = r6; r1 = 000000BD; r0 = add(PC,00015EA9) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,00015EB8) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 000101B4; r2 = memw(r25+20); r1 = 000000BD }

l00010198:
	{ r2 = r24; r0 = add(PC,00000021) }
	{ r3 = add(PC,00015E92) }

l000101A8:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000103E0 }

l000101B4:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 000101FC }

l000101F0:
	{ if (p0.new) jump:nt 000101FC; p0 = cmp.eq(r5,00000001) }

l000101F4:
	{ p0 = cmp.eq(r13,00000001); if (!p0.new) jump:nt 00010238 }

l000101FC:
	{ if (p0.new) jump:nt 0001020C; p0 = cmp.eq(r6,r3) }

l00010200:
	{ if (p0.new) jump:nt 0001020C; p0 = cmp.eq(r6,00000001) }

l00010204:
	{ nop; if (!p0.new) jump:nt 00010238; p0 = cmp.eq(r3,00000001) }

l0001020C:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 00010220 }

l00010214:
	{ if (p0.new) jump:nt 00010220; p0 = cmp.eq(r7,00000001) }

l00010218:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 00010238 }

l00010220:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001026C }

l00010228:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 0001026C }

l00010230:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 0001026C }

l00010238:
	{ memw(r29+28) = r12; r1 = 000000BD; r0 = add(PC,00015DBD) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,00015DF0) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 000101A8 }

l0001026C:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,00015D7D) }
	{ memw(r29+40) = r20; r1 = 000000BD; r4 = add(PC,00015DEC) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = 00000000; p0 = cmp.gt(r24,00000000) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 000103B8 }

l000102E8:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,00000001); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,00000001); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,00000000); p2 = cmp.eq(r7,00000001); r2 = mux(p2,00000000,r12); r4 = mpyi(r13,r5) }
	{ r5 = !cmp.eq(r12,00000001); r3 = mux(p0,00000000,r13); p0 = r0; r8 = mpyi(r8,r12) }
	{ if (p0) r8 = add(r14,00000000); if (p2) r4 = add(r14,00000000); r7 = 00000000 }

l00010340:
	{ r14 = 00000000; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 000103A8; p0 = cmp.gt(r10,00000000) }

l00010344:
	{ r14 = 00000000; r13:r12 = combine(r25,r26) }

l0001034C:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 00010398; p0 = cmp.gt(r12,00000000) }

l00010358:
	{ loop1(0001035C,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001038C; p0 = cmp.gt(r13,00000000) }

l00010368:
	{ loop0(0001036C,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,00000002); r0 = addasl(r0,r6,00000002) }
	{ r11 = add(r16,r11) }
	{ memw(r10++#4) = r11; nop }
	{ r23 = addasl(r23,r21,00000002) }

l0001038C:
	{ nop; r15 = addasl(r15,r2,00000002); r28 = addasl(r28,r3,00000002) }

l00010398:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001034C; r14 = add(r14,00000001); r12 = addasl(r12,r4,00000002); r13 = addasl(r13,r22,00000002) }

l000103A8:
	{ if (!cmp.eq(r7.new,r24)) jump:t 00010340; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002); r25 = addasl(r25,r8,00000002) }

l000103AC:
	{ if (!cmp.eq(r7.new,r24)) jump:t 00010344; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002) }

l000103B8:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,00015C3D) }

l000103BC:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,0000003D) }

l000103CC:
	{ r2 = r19; r1 = 000000BD; r4 = add(PC,00000018) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l000103E0:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; add_int32_check: 000103F4
add_int32_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00015BC6) }
	{ r16 = r1; r1 = 00000037; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,00015B99) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 00010444; r2 = memw(r17+16); r1 = 00000038 }

l00010428:
	{ r0 = add(PC,0000003D) }
	{ r3 = add(PC,00015B9A) }

l00010434:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00010444:
	{ if (cmp.eq(r2.new,00000001)) jump:t 00010464; r2 = memw(r17+20); r1 = 00000039 }

l00010454:
	{ r0 = add(PC,00000011) }
	{ jump 00010434; r3 = add(PC,00015B7D) }

l00010464:
	{ r2 = r16; r1 = 0000003A; r0 = add(PC,00015B3D) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,00015B75) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00010488
;;   Called from:
;;     00010180 (in add_int32_execute)
;;     000102B0 (in add_int32_execute)
;;     000103D8 (in add_int32_execute)
;;     00010414 (in add_int32_check)
;;     00010470 (in add_int32_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000104A8; r5 = memw(r2+16) }

l00010498:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l000104A8:
	{ dealloc_return }

;; errlog_function: 000104AC
;;   Called from:
;;     000101A8 (in add_int32_execute)
;;     00010434 (in add_int32_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; prod_f_execute: 000104D0
;;   Called from:
;;     000104CC (in close_check__merged)
prod_f_execute proc
	{ allocframe(+00000030); r28 = 00000001 }
	{ r8 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+40) = r17:r16; r6 = memw(r0+16) }
	{ r14 = memw(r2); r7 = memw(r8); r2 = r1 }
	{ memd(r29+24) = r21:r20; memd(r29+32) = r19:r18; if (!p0.new) r17 = 00000001; p0 = cmp.eq(r6,00000003) }
	{ r3 = memw(r7+12); r15 = memw(r7); if (!p0) r10 = 00000001; if (!p0) r1 = 00000001 }
	{ r5 = memw(r7+4); r4 = memw(r7+8); if (p0) r13:r12 = combine(r3,r15); if (p0) r18 = 00000000 }
	{ r7 = memw(r14+16); r6 = memw(r7+16) }
	{ memd(r29+8) = r25:r24; memd(r29+16) = r23:r22 }
	{ memd(r29) = r27:r26; if (p0) jump:nt 0001053C }

l00010530:
	{ r9:r8 = combine(00000001,00000001); r13:r12 = combine(00000001,00000001) }
	{ jump 0001067C }

l0001053C:
	{ r1 = memw(r8+4); r10 = memw(r8+8); r9:r8 = combine(r4,r5) }
	{ r11 = memw(r1+12); r10 = memw(r10+16) }
	{ r10 = memw(r10); r1 = memw(r1+16) }
	{ if (!p0.new) r16 = add(r1,00000000); if (!p0.new) r9:r8 = combine(r4,r5); p0 = cmp.eq(r11,00000000); if (p0.new) jump:nt 000105B8 }

l00010568:
	{ r13:r12 = combine(r3,r15); loop0(00010570,r11) }
	{ r17 = memw(r16++#4) }
	{ if (!cmp.gt(r17.new,00000003)) jump:t 0001058C; r17 = add(r10,sub(0000007F,r17)) }

l00010580:
	{ r12 = 00000001; r13 = 00000001; r9:r8 = combine(00000001,00000001) }

l0001058C:
	{ if (p0.new) r13 = 00000001; if (p0.new) jump:t 000105AC; p0 = cmp.eq(r9,00000000) }

l00010594:
	{ if (p0.new) r9 = 00000001; if (p0.new) jump:t 000105AC; p0 = cmp.eq(r9,00000001) }

l0001059C:
	{ if (p0.new) r8 = 00000001; if (p0.new) jump:t 000105AC; p0 = cmp.eq(r9,00000002) }

l000105A4:
	{ if (p0.new) r12 = 00000001; p0 = cmp.eq(r17,00000003) }

l000105AC:
	{ nop; nop }
	{ r18 = r11 }

l000105B8:
	{ if (cmp.eq(r0.new,00000002)) jump:t 000105DC; r0 = memb(r0+32); r11 = r9; p0 = cmp.eq(r18,00000000) }

l000105CC:
	{ r1 = r13; r28 = r12; r17 = r8 }
	{ jump 0001067C }

l000105DC:
	{ r0 = r13; r17:r16 = combine(r8,r12); if (!p0) r11 = add(r9,00000000) }
	{ if (p0.new) r17:r16 = combine(r8,r12); if (!p0) r0 = add(r13,00000000); if (p0) jump:nt 00010638 }

l000105F4:
	{ loop0(000105F8,r18) }
	{ r18 = memw(r1++#4) }
	{ if (!cmp.gt(r18.new,00000003)) jump:t 00010614; r18 = add(r10,sub(0000007F,r18)) }

l00010608:
	{ r16 = 00000000; r0 = 00000000; r17 = 00000000 }
	{ jump 00010630 }

l00010614:
	{ if (p0.new) r0 = 00000000; if (p0.new) jump:t 00010630; p0 = cmp.eq(r10,00000000) }

l0001061C:
	{ if (p0.new) r11 = 00000000; if (p0.new) jump:t 00010630; p0 = cmp.eq(r10,00000001) }

l00010624:
	{ if (p0.new) r17 = 00000000; if (p0.new) jump:t 00010630; p0 = cmp.eq(r10,00000002) }

l0001062C:
	{ r16 = -00000001; p0 = cmp.eq(r18,00000003) }

l00010630:
	{ nop; nop }

l00010638:
	{ p3 = cmp.eq(r0,00000000); p0 = cmp.eq(r11,00000000); p2 = cmp.eq(r17,00000000); p1 = cmp.eq(r16,00000000) }
	{ r1 = mux(p1,00000001,r16); p1 = or(p1,p2) }
	{ r10 = mux(p1,00000001,r16); if (!p2) r1 = add(r17,00000000); p2 = or(p1,p0) }
	{ r17 = mux(p2,00000001,r16); if (!p0) r1 = add(r11,00000000); if (!p0) r10 = add(r1,00000000); if (p3) jump:nt 0001067C }

l0001066C:
	{ r1 = r0; r10 = r1; r17 = r10; r28 = r17 }

l0001067C:
	{ r11 = memw(r14+20); p0 = cmp.gt(r12,00000000); r0 = mpyi(r13,r9) }
	{ r0 = mpyi(r0,r8) }
	{ r0 = mpyi(r0,r12) }
	{ if (!cmp.gtu(r0.new,r11)) jump:t 000106B8; r0 = asl(r0,00000002) }

l0001069C:
	{ r1 = 000000C6; r0 = add(PC,0000003A) }
	{ call errlog_function; r3 = add(PC,00015A8D) }
	{ r0 = FFFFFFFF; jump 00010814 }

l000106B8:
	{ memw(r14+24) = r0; memw(r14+12) = r1; r0 = 00000000 }
	{ memw(r14+4) = r17; memw(r14+8) = r10; if (p0) r0 = 00000000 }
	{ memw(r14) = r28; if (p0) r1 = 3F800000; if (!p0) jump:nt 00010814 }

l000106E0:
	{ p2 = cmp.eq(r8,00000001); p1 = cmp.eq(r13,00000001); p3 = cmp.eq(r9,00000001); p0 = cmp.eq(r12,00000001) }
	{ r14 = mux(p1,r3,00000001); r28 = mux(p3,r4,00000001); r2 = mux(p0,r15,00000001) }
	{ r15 = mux(p2,r5,00000001) }

l00010700:
	{ p0 = cmp.gt(r8,00000000); r10 = 00000000; if (!p0.new) jump:nt 00010808 }

l00010704:
	{ p0 = cmp.gt(r8,00000000); r10 = 00000000 }

l0001070C:
	{ p0 = cmp.gt(r9,00000000); r11 = 00000000; if (!p0.new) jump:nt 00010800 }

l00010710:
	{ p0 = cmp.gt(r9,00000000); r11 = 00000000 }

l00010718:
	{ p0 = cmp.gt(r13,00000000); r17:r16 = combine(00000000,r7); if (!p0.new) jump:nt 000107F8 }

l00010724:
	{ r18 = r1; if (!p0.new) jump:nt 000107E4; p0 = cmp.gt(r2,00000000) }

l0001072C:
	{ r19:r18 = combine(00000000,r1) }

l00010730:
	{ p0 = cmp.gt(r15,00000000); r22 = add(r19,r0); if (!p0.new) jump:nt 000107DC }

l00010734:
	{ p0 = cmp.gt(r15,00000000); r22 = add(r19,r0) }

l0001073C:
	{ r21:r20 = combine(r10,00000000) }
	{ r21 += mpyi(r22,r5) }

l00010744:
	{ p0 = cmp.gt(r28,00000000); r24 = add(r21,r20); if (!p0.new) jump:nt 000107D4 }

l00010750:
	{ r23:r22 = combine(r11,00000000); loop1(0001075C,r28) }
	{ r23 += mpyi(r24,r4) }
	{ p0 = cmp.gt(r14,00000000); r24 = r17; r25 = add(r23,r22); if (!p0.new) jump:nt 000107C8 }

l0001076C:
	{ p0 = cmp.gtu(r14,00000001); r26 = add(r14,FFFFFFFF) }
	{ if (p0) r27 = add(r26,FFFFFFFF); r24 += mpyi(r25,r3) }
	{ r25 = addasl(r6,r24,00000002) }
	{ r24 = add(r25,00000004) }
	{ r25 = memw(r25); if (p0) jump:nt 00010794 }

l0001078C:
	{ r24 = r25; jump 000107C4 }

l00010794:
	{ r24 = memw(r24); r26 = add(r24,00000004); p0 = cmp.gtu(r26,00000001); loop0(000107A8,r27) }
	{ if (!p0) jump:nt 000107C0 }

l000107A8:
	{ r24 = memw(r26); r26 = add(r26,00000004); r27 = r24; r18 = sfmpy(r18,r25) }
	{ nop; r25 = r27 }

l000107C0:
	{ r18 = sfmpy(r18,r25) }

l000107C4:
	{ r18 = sfmpy(r18,r24) }

l000107C8:
	{ nop; nop; r22 = add(r22,00000001) }

l000107D4:
	{ if (!cmp.eq(r20.new,r15)) jump:t 00010744; r20 = add(r20,00000001) }

l000107DC:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00010730; r19 = add(r19,00000001) }

l000107E0:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00010734 }

l000107E4:
	{ memw(r16) = r18; r17 = r17; r16 = add(r16,00000004) }

l000107E8:
	{ memw(r16) = r18; r17 = r17 }

l000107EC:
	{ p0 = cmp.eq(r17,r13); if (!p0.new) jump:nt 00010724 }

l000107F4:
	{ r7 = addasl(r7,r13,00000002) }

l000107F8:
	{ if (!cmp.eq(r11.new,r9)) jump:t 00010718; r11 = add(r11,00000001) }

l00010800:
	{ if (!cmp.eq(r10.new,r8)) jump:t 0001070C; r10 = add(r10,00000001) }

l00010804:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00010710 }

l00010808:
	{ if (!cmp.eq(r0.new,r12)) jump:t 00010700; r0 = add(r0,00000001) }

l0001080C:
	{ if (!cmp.eq(r0.new,r12)) jump:t 00010704 }

l00010814:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ r27:r26 = memd(r29); r25:r24 = memd(r29+8) }
	{ dealloc_return }
00010828                         00 40 00 7F 00 C0 00 7F         .@......

;; prod_f_check: 00010830
prod_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000158A7) }
	{ r17 = r0; r16 = r1; r1 = 00000037 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 0001087C; r3 = 00000004 }

l0001085C:
	{ r1 = 00000038; r0 = add(PC,00000023) }

l00010864:
	{ r3 = add(PC,0001587F) }

l0001086C:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001087C:
	{ r1 = 00000039; if (!p0.new) jump:nt 00010890; p0 = cmp.eq(r2,00000000) }

l00010884:
	{ jump 00010864; r0 = add(PC,00015837) }

l00010890:
	{ if (cmp.eq(r2.new,00000001)) jump:t 000108B0; r2 = memw(r17+20); r1 = 0000003A }

l000108A0:
	{ r0 = add(PC,0000001F) }
	{ jump 0001086C; r3 = add(PC,0001584E) }

l000108B0:
	{ r2 = r16; r1 = 0000003B; r4 = add(PC,00015852) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 000108CC
;;   Called from:
;;     00010844 (in prod_f_check)
;;     000108BC (in prod_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000108F0; r5 = memw(r2+16) }

l000108DC:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000023) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l000108F0:
	{ dealloc_return }

;; errlog_function: 000108F4
;;   Called from:
;;     000106A4 (in prod_f_execute)
;;     0001086C (in prod_f_check)
;;     000108E4 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }

;; mul_int32_execute: 00010910
;;   Called from:
;;     000108FC (in errlog_function)
mul_int32_execute proc
	{ allocframe(000000A0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = 00000000 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,00000001); p0 = cmp.eq(r6,00000001) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,00000001); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,00000001); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,00000002) }
	{ if (p0) jump:nt 000109BC }

l000109B8:
	{ r22 = mpyi(r7,r12) }

l000109BC:
	{ r2 = r6; r1 = 000000BD; r0 = add(PC,000157D7) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,000157E6) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 00010A24; r2 = memw(r25+20); r1 = 000000BD }

l00010A08:
	{ r2 = r24; r0 = add(PC,0000000F) }
	{ r3 = add(PC,000157C0) }

l00010A18:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00010C50 }

l00010A24:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 00010A6C }

l00010A60:
	{ if (p0.new) jump:nt 00010A6C; p0 = cmp.eq(r5,00000001) }

l00010A64:
	{ p0 = cmp.eq(r13,00000001); if (!p0.new) jump:nt 00010AA8 }

l00010A6C:
	{ if (p0.new) jump:nt 00010A7C; p0 = cmp.eq(r6,r3) }

l00010A70:
	{ if (p0.new) jump:nt 00010A7C; p0 = cmp.eq(r6,00000001) }

l00010A74:
	{ nop; if (!p0.new) jump:nt 00010AA8; p0 = cmp.eq(r3,00000001) }

l00010A7C:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 00010A90 }

l00010A84:
	{ if (p0.new) jump:nt 00010A90; p0 = cmp.eq(r7,00000001) }

l00010A88:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 00010AA8 }

l00010A90:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 00010ADC }

l00010A98:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 00010ADC }

l00010AA0:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 00010ADC }

l00010AA8:
	{ memw(r29+28) = r12; r1 = 000000BD; r0 = add(PC,000156EB) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,0001571E) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 00010A18 }

l00010ADC:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,000156AB) }
	{ memw(r29+40) = r20; r1 = 000000BD; r4 = add(PC,0001571A) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = 00000000; p0 = cmp.gt(r24,00000000) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 00010C28 }

l00010B58:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,00000001); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,00000001); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,00000000); p2 = cmp.eq(r7,00000001); r2 = mux(p2,00000000,r12); r4 = mpyi(r13,r5) }
	{ r5 = !cmp.eq(r12,00000001); r3 = mux(p0,00000000,r13); p0 = r0; r8 = mpyi(r8,r12) }
	{ if (p0) r8 = add(r14,00000000); if (p2) r4 = add(r14,00000000); r7 = 00000000 }

l00010BB0:
	{ r14 = 00000000; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 00010C18; p0 = cmp.gt(r10,00000000) }

l00010BB4:
	{ r14 = 00000000; r13:r12 = combine(r25,r26) }

l00010BBC:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 00010C08; p0 = cmp.gt(r12,00000000) }

l00010BC8:
	{ loop1(00010BCC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 00010BFC; p0 = cmp.gt(r13,00000000) }

l00010BD8:
	{ loop0(00010BDC,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,00000002); r0 = addasl(r0,r6,00000002) }
	{ r11 = mpyi(r16,r11) }
	{ memw(r10++#4) = r11; nop }
	{ r23 = addasl(r23,r21,00000002) }

l00010BFC:
	{ nop; r15 = addasl(r15,r2,00000002); r28 = addasl(r28,r3,00000002) }

l00010C08:
	{ if (!cmp.eq(r14.new,r18)) jump:t 00010BBC; r14 = add(r14,00000001); r12 = addasl(r12,r4,00000002); r13 = addasl(r13,r22,00000002) }

l00010C18:
	{ if (!cmp.eq(r7.new,r24)) jump:t 00010BB0; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002); r25 = addasl(r25,r8,00000002) }

l00010C1C:
	{ if (!cmp.eq(r7.new,r24)) jump:t 00010BB4; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002) }

l00010C28:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,0001556B) }

l00010C2C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,0000002B) }

l00010C3C:
	{ r2 = r19; r1 = 000000BD; r4 = add(PC,00000006) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00010C50:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; mul_int32_check: 00010C64
mul_int32_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000154F4) }
	{ r16 = r1; r1 = 00000037; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,000154C7) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 00010CB4; r2 = memw(r17+16); r1 = 00000038 }

l00010C98:
	{ r0 = add(PC,0000002B) }
	{ r3 = add(PC,000154C8) }

l00010CA4:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00010CB4:
	{ if (cmp.eq(r2.new,00000001)) jump:t 00010CD4; r2 = memw(r17+20); r1 = 00000039 }

l00010CC4:
	{ r0 = add(PC,0000003F) }
	{ jump 00010CA4; r3 = add(PC,000154AB) }

l00010CD4:
	{ r2 = r16; r1 = 0000003A; r0 = add(PC,0001546B) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,000154A3) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00010CF8
;;   Called from:
;;     000109F0 (in mul_int32_execute)
;;     00010B20 (in mul_int32_execute)
;;     00010C48 (in mul_int32_execute)
;;     00010C84 (in mul_int32_check)
;;     00010CE0 (in mul_int32_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00010D18; r5 = memw(r2+16) }

l00010D08:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l00010D18:
	{ dealloc_return }

;; errlog_function: 00010D1C
;;   Called from:
;;     00010A18 (in mul_int32_execute)
;;     00010CA4 (in mul_int32_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; pack_execute: 00010D40
;;   Called from:
;;     00010D3C (in errlog_function)
pack_execute proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r17 = r0; r7 = r1 }
	{ memd(r29+8) = r27:r26; r3 = memw(r17+4) }
	{ r2 = memw(r17+8) }
	{ memd(r29+32) = r21:r20; memd(r29+40) = r19:r18 }
	{ r19 = memw(r2); r26 = memw(r3) }
	{ memd(r29+24) = r23:r22; r20 = memw(r17+16) }
	{ r2 = memw(r26+24); if (p0.new) r4 = add(r3,00000004); p0 = cmp.gt(r20,00000001) }
	{ memd(r29+16) = r25:r24; r21 = memw(r26+8) }
	{ r23 = memw(r26+4); r25 = memw(r26); r24 = r2 }
	{ r18 = memw(r19+16); r22 = memw(r26+12); r5 = add(r20,FFFFFFFF); if (!p0) jump:nt 00010E24 }

l00010D98:
	{ r24 = r2; loop0(00010DA0,r5) }
	{ r5 = memw(r4); r3 = add(PC,00015500) }
	{ if (cmp.eq(r6.new,r25)) jump:t 00010DBC; r6 = memw(r5) }

l00010DB8:
	{ r1 = 00000042 }

l00010DBC:
	{ if (cmp.eq(r6.new,r23)) jump:t 00010DD4; r6 = memw(r5+4); r3 = add(PC,000154E4) }

l00010DD0:
	{ r1 = 00000043 }

l00010DD4:
	{ if (cmp.eq(r6.new,r21)) jump:t 00010DEC; r6 = memw(r5+8); r3 = add(PC,000154CC) }

l00010DE8:
	{ r1 = 00000044 }

l00010DEC:
	{ if (cmp.eq(r6.new,r22)) jump:t 00010E04; r6 = memw(r5+12); r3 = add(PC,000154B4) }

l00010E00:
	{ r1 = 00000045 }

l00010E04:
	{ if (cmp.eq(r5.new,r2)) jump:t 00010E1C; r5 = memw(r5+24); r3 = add(PC,000154A6) }

l00010E18:
	{ r1 = 00000046 }

l00010E1C:
	{ r24 = add(r24,r2); r4 = add(r4,00000004) }

l00010E24:
	{ r1 = 0000004A; r3 = add(PC,0001548F) }
	{ if (cmp.gtu(r24,r4.new)) jump:t 00010EC0; r4 = memw(r19+20) }

l00010E3C:
	{ memw(r29+4) = r7; if (p0.new) r16 = 00000004 }
	{ r27 = r20 }

l00010E48:
	{ r1 = memw(r26+16); r0 = r18; call fn00009560 }
	{ if (cmp.eq(r27.new,00000000)) jump:nt 00010E74; r3 = memw(r26+24); r27 = add(r27,FFFFFFFF) }

l00010E64:
	{ r26 = memw(r13+r16); r16 = add(r16,00000004) }
	{ r2 = memw(r26+24); jump 00010E48 }

l00010E74:
	{ if (p0.new) r22 = add(r20,00000000); if (!p0.new) jump:nt 00010E98; p0 = cmp.eq(r14,00000001) }

l00010E7C:
	{ r20 = r25 }

l00010E80:
	{ memw(r19+4) = r23; memw(r19+24) = r24; r0 = 00000000 }
	{ memw(r19+8) = r21; memw(r19) = r20 }
	{ memw(r19+12) = r22; jump 00010ECC }

l00010E98:
	{ if (p0.new) r21 = add(r20,00000000); if (p0.new) jump:t 00010E7C; p0 = cmp.eq(r13,00000001) }

l00010EA0:
	{ if (p0.new) r23 = add(r20,00000000); if (p0.new) jump:t 00010E7C; p0 = cmp.eq(r15,00000001) }

l00010EA8:
	{ r7 = memw(r29+4); r1 = 00000056; r3 = add(PC,00015419) }
	{ p0 = cmp.eq(r25,00000001); if (p0.new) jump:nt 00010E80 }

l00010EC0:
	{ r2 = r7; call errlog_function }
	{ r0 = FFFFFFFF }

l00010ECC:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }

;; pack_check: 00010EE0
pack_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r17 = r0; r16 = r1 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 00010F0C; r2 = memw(r17+16); r1 = 0000005F }

l00010F04:
	{ jump 00010F28; r3 = add(PC,00000009) }

l00010F0C:
	{ if (cmp.eq(r2.new,00000001)) jump:t 00010F30; r2 = memw(r17+20); r0 = 00000000; r1 = 00000060 }

l00010F20:
	{ r3 = add(PC,00000038) }
	{ r2 = r16; call errlog_function }

l00010F28:
	{ r2 = r16 }

l00010F2C:
	{ r0 = FFFFFFFF }

l00010F30:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00010F34
;;   Called from:
;;     00010EEC (in pack_check)
logmsg_function proc
	{ allocframe(00000008); r4 = 00000002 }
	{ if (cmp.gtu(r4,r3.new)) jump:t 00010F64; r3 = memw(r2+16) }

l00010F44:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000019) }
	{ r6 = add(r29,00000010); r1 = 0000005E; r4 = add(PC,00015327) }
	{ memw(r29+4) = r6; call logv }

l00010F64:
	{ dealloc_return }

;; errlog_function: 00010F68
;;   Called from:
;;     00010EC0 (in pack_execute)
;;     00010F24 (in pack_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000152F1) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00010F8C                                     00 00 00 00             ....

;; shape_execute: 00010F90
shape_execute proc
	{ allocframe(00000018); memd(r29+496) = r17:r16; r4 = add(PC,000153B7) }
	{ r1 = 00000031; r17:r16 = combine(r1,r0) }
	{ memd(r29+8) = r19:r18; r3 = memw(r16+4) }
	{ r5 = memw(r16+8); r2 = r17 }
	{ r19 = memw(r3) }
	{ memw(r29) = r16; r18 = memw(r5); call logmsg_function }
	{ if (cmp.gtu(r2.new,0000000F)) jump:t 00010FD8; r2 = memw(r18+20) }

l00010FC4:
	{ r2 = r17; r1 = 00000033; r3 = add(PC,0000001F) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00011024 }

l00010FD8:
	{ memw(r18+4) = 00000001; r2 = memw(r18+16); r4 = add(PC,00015395) }
	{ memw(r18+12) = FFFFFF84; memw(r18) = 00000001; r1 = 0000003B }
	{ memw(r18+8) = 00000001 }
	{ memb(r2) = r3.new; r3 = memw(r19) }
	{ memw(r7+4) = r6 }
	{ r2 = memw(r19+8); r3 = memw(r18+16) }
	{ memw(r3+8) = r2; r2 = r17 }
	{ r7 = memw(r19+12); r5 = memw(r18+16) }
	{ memw(r18+24) = 00000010; memw(r5+12) = r7 }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }

l00011024:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }
	{ dealloc_return }
0001102C                                     00 C0 00 7F             ....

;; shape_check: 00011030
shape_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000152CF) }
	{ r17 = r0; r16 = r1; r1 = 00000041 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000001)) jump:t 00011070; r2 = memw(r17+16); r1 = 00000042 }

l0001105C:
	{ r3 = add(PC,0000003E) }
	{ r2 = r16; call errlog_function }

l00011064:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00011070:
	{ if (cmp.eq(r2.new,00000001)) jump:t 00011088; r2 = memw(r17+20); r1 = 00000043 }

l00011080:
	{ jump 00011064; r3 = add(PC,00000029) }

l00011088:
	{ r2 = r16; r1 = 00000044; r4 = add(PC,000152AD) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 000110A8
;;   Called from:
;;     00010FB0 (in shape_execute)
;;     00011018 (in shape_execute)
;;     00011044 (in shape_check)
;;     00011098 (in shape_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000110CC; r5 = memw(r2+16) }

l000110B8:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000030) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l000110CC:
	{ dealloc_return }

;; errlog_function: 000110D0
;;   Called from:
;;     00010FCC (in shape_execute)
;;     00011060 (in shape_check)
;;     000110C0 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00015214) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
000110F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; and_int32_execute: 00011100
and_int32_execute proc
	{ jump broadcast_elementwise_execute_int32; r2 = add(PC,000004C8) }

;; logical_int32_check: 0001110C
logical_int32_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00015352) }
	{ r16 = r1; r1 = 0000003E; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,00015321) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001115C; r2 = memw(r17+16); r1 = 0000003F }

l00011140:
	{ r0 = add(PC,00000005) }
	{ r3 = add(PC,0001532D) }

l0001114C:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001115C:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0001117C; r2 = memw(r17+20); r1 = 00000040 }

l0001116C:
	{ r0 = add(PC,00000019) }
	{ jump 0001114C; r3 = add(PC,00015310) }

l0001117C:
	{ r2 = r16; r1 = 00000041; r0 = add(PC,000152C5) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,00015304) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; ior_int32_execute: 000111A4
ior_int32_execute proc
	{ jump broadcast_elementwise_execute_int32; r2 = add(PC,0000041C) }

;; xor_int32_execute: 000111B0
xor_int32_execute proc
	{ jump broadcast_elementwise_execute_int32; r2 = add(PC,000003C8) }
000111BC                                     00 C0 00 7F             ....

;; broadcast_elementwise_execute_int32: 000111C0
;;   Called from:
;;     00011100 (in and_int32_execute)
;;     000111A4 (in ior_int32_execute)
;;     000111B0 (in xor_int32_execute)
broadcast_elementwise_execute_int32 proc
	{ allocframe(+000000A8); memd(r29-48) = r25:r24; r25:r24 = combine(r1,r0) }
	{ memd(r29+136) = r23:r22; r3 = memw(r24+4) }
	{ memd(r29+152) = r19:r18; r18 = r2; r2 = 00000000 }
	{ memd(r29+144) = r21:r20 }
	{ memd(r29+160) = r17:r16; r23 = memw(r3) }
	{ r4 = memw(r24+8); r19 = memw(r3+4) }
	{ memd(r29+120) = r27:r26; r12 = memw(r23+4) }
	{ r7 = memw(r19); r0 = memw(r23); p0 = cmp.eq(r12,00000001) }
	{ r21 = memw(r23+8); r5 = memw(r19+4); p1 = cmp.eq(r0,00000001) }
	{ memw(r29+104) = r5; r9 = memw(r19+8); p2 = cmp.eq(r21,00000001); r6 = mux(p0,r5,r12) }
	{ r8 = memw(r23+12); r5 = mux(p1,r7,r0); r10 = p1 }
	{ memw(r29+72) = r7; r0 = memw(r19+12); p1 = cmp.eq(r8,00000001); r3 = mpyi(r5,r6) }
	{ r17 = memw(r4); r7 = mux(p2,r9,r21) }
	{ memw(r29+60) = r12; memw(r29+100) = r10; r10 = p2; r3 = mpyi(r3,r7) }
	{ memw(r29+88) = r6; memw(r29+68) = r5; r16 = mux(p1,r0,r8) }
	{ memw(r29+108) = r10; memw(r29+64) = r9; r3 = mpyi(r3,r16) }
	{ memw(r29+92) = r0; memw(r29+112) = r7 }
	{ memw(r29+84) = r2; r22 = asl(r3,00000002) }
	{ if (p0) jump:nt 00011270 }

l00011268:
	{ r2 = mpyi(r8,r21) }
	{ memw(r29+84) = r2 }

l00011270:
	{ r2 = r25; r1 = 000000BD; r0 = add(PC,0001510B) }
	{ memw(r29+96) = r8; r27 = memw(r17+16); r4 = add(PC,0001511A) }
	{ r26 = memw(r23+16); r20 = memw(r19+16) }
	{ memw(r29) = r24; call logmsg_function }
	{ if (!cmp.gtu(r22,r2.new)) jump:t 000112CC; r2 = memw(r17+20); r1 = 000000BD }

l000112B0:
	{ r2 = r25; r0 = add(PC,0000000F) }
	{ r3 = add(PC,00015100) }

l000112C0:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00011564 }

l000112CC:
	{ r5 = memw(r23); r2 = memw(r19) }
	{ r7 = memw(r23+8); r8 = memw(r23+12); p0 = cmp.eq(r5,r2) }
	{ r12 = memw(r19+12); r6 = memw(r23+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+76) = r26; memw(r29+56) = r22 }
	{ memw(r29+80) = r20; if (p0) jump:nt 00011304 }

l000112FC:
	{ if (p0.new) jump:nt 00011304; p0 = cmp.eq(r5,00000001) }

l00011300:
	{ if (!p0.new) jump:nt 00011340; p0 = cmp.eq(r2,00000001) }

l00011304:
	{ nop; if (p0.new) jump:nt 00011314; p0 = cmp.eq(r6,r3) }

l0001130C:
	{ if (p0.new) jump:nt 00011314; p0 = cmp.eq(r6,00000001) }

l00011310:
	{ if (!p0.new) jump:nt 00011340; p0 = cmp.eq(r3,00000001) }

l00011314:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 00011328 }

l0001131C:
	{ if (p0.new) jump:nt 00011328; p0 = cmp.eq(r7,00000001) }

l00011320:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 00011340 }

l00011328:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 00011374 }

l00011330:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 00011374 }

l00011338:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 00011374 }

l00011340:
	{ memw(r29+28) = r12; r1 = 000000BD; r0 = add(PC,0001503B) }
	{ memw(r29+12) = r8; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,0001506E) }
	{ memw(r29+4) = r6; memw(r29+16) = r2; r2 = r25 }
	{ memw(r29) = r5; jump 000112C0 }

l00011374:
	{ memw(r29+44) = r16; r23 = memd(r29+112); r0 = add(PC,00015007) }
	{ r19 = memw(r29+68); r1 = 000000BD; r4 = add(PC,00015076) }
	{ memw(r29+40) = r23; r20 = memd(r29+88) }
	{ memw(r29+32) = r19; memw(r29+36) = r20 }
	{ memw(r29+28) = r12; memw(r29+52) = r24 }
	{ memw(r29+24) = r9; memw(r29+48) = r25 }
	{ memw(r29+8) = r7; memw(r29+20) = r3 }
	{ memw(r29+4) = r6; memw(r29+16) = r2; r2 = r25 }
	{ memw(r29) = r5; memw(r29+12) = r8 }
	{ call logmsg_function }
	{ r4 = memw(r29+80); p0 = cmp.gt(r19,00000000) }
	{ r2 = memd(r29+56); r5 = memd(r29+76) }
	{ memw(r17) = r19; memw(r17+24) = r2 }
	{ memw(r17+8) = r23; memw(r17+4) = r20 }
	{ memw(r17+12) = r16; if (!p0) jump:nt 0001153C }

l000113DC:
	{ r9 = memw(r29+64); r2 = memw(r29+60); r1 = 00000000 }
	{ r0 = memd(r29+108); r6 = memd(r29+104); p0 = cmp.eq(r9,00000001); r2 = mpyi(r21,r2) }
	{ r8 = memw(r29+92); r7 = memw(r29+72); r3 = mpyi(r9,r6) }
	{ r0 = memw(r29+100); r13 = mux(p0,00000000,r8); p2 = r0; r9 = mpyi(r8,r9) }
	{ memw(r29+64) = r1; r7 = memd(r29+96); p1 = cmp.eq(r7,00000001); r3 = mpyi(r3,r8) }
	{ r25 = !cmp.eq(r7,00000001); r6 = 00000000; p2 = cmp.eq(r6,00000001); r12 = mux(p2,00000000,r7) }
	{ if (p2) r9 = add(r6,00000000); r26 = !cmp.eq(r8,00000001); p0 = r0; r2 = mpyi(r2,r7) }
	{ memw(r29+72) = r9; memw(r29+108) = r12; if (p0) r2 = add(r6,00000000); if (p1) r3 = add(r6,00000000) }
	{ memw(r29+60) = r2; memw(r29+104) = r13 }
	{ memw(r29+56) = r3 }

l00011458:
	{ memw(r29+76) = r5; r2 = memd(r29+88); r7:r6 = combine(00000000,r5); r3 = r4 }
	{ memw(r29+80) = r4; p0 = cmp.gt(r2,00000000) }
	{ if (!p0) jump:nt 0001151C }

l00011470:
	{ if (cmp.gt(r2.new,00000000)) jump:t 00011484; r2 = memw(r29+112) }

l0001147C:
	{ memw(r29+100) = r3; jump 000114FC }

l00011484:
	{ r21 = r6; r23 = 00000000; r19 = r3 }
	{ memw(r29+96) = r6; memw(r29+92) = r7 }
	{ memw(r29+100) = r3 }

l00011494:
	{ r22 = r16; r17 = r19; r24 = r27; r20 = r21 }
	{ if (!p0.new) jump:nt 000114E4; p0 = cmp.gt(r8,00000000) }

l000114A4:
	{ nop; nop; nop }
	{ nop; nop; nop; nop }

l000114C0:
	{ r1 = memw(r17); r0 = memw(r20); callr r18; r22 = add(r22,FFFFFFFF) }
	{ p0 = cmp.eq(r22,00000000); r17 = addasl(r17,r26,00000002); r20 = addasl(r20,r25,00000002) }
	{ memw(r24++#4) = r0; if (!p0) jump:nt 000114C0 }

l000114E0:
	{ r27 = addasl(r27,r16,00000002) }

l000114E4:
	{ r7 = memd(r29+104); r2 = memd(r29+108); r23 = add(r23,00000001) }
	{ r21 = addasl(r21,r2,00000002) }
	{ if (!cmp.eq(r2.new,r23)) jump:t 00011494; r2 = memw(r29+112); r19 = addasl(r19,r7,00000002) }

l000114FC:
	{ r6 = memd(r29+96); r2 = memd(r29+84) }

l00011500:
	{ r3 = memd(r29+100); r5 = memd(r29+72) }
	{ r2 = memd(r29+88); r7 = memd(r29+92); r6 = addasl(r6,r2,00000002) }
	{ r5 = memd(r29+76); r4 = memd(r29+80); r3 = addasl(r3,r5,00000002) }
	{ if (!cmp.eq(r7.new,r2)) jump:t 00011470; r7 = add(r7,00000001) }

l0001151C:
	{ r7 = memd(r29+56); r2 = memd(r29+60) }

l00011520:
	{ r3 = memw(r29+64) }
	{ r2 = memd(r29+68); r3 = r3; r4 = addasl(r4,r7,00000002); r5 = addasl(r5,r2,00000002) }
	{ memw(r29+64) = r3; p0 = cmp.eq(r3,r2) }
	{ if (!p0) jump:nt 00011458 }

l0001153C:
	{ memb(r29) = r2.new; r2 = memw(r29+52); r0 = add(PC,00014E3F) }
	{ r2 = memw(r29+48); r1 = 000000BD; r4 = add(PC,0000001A) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00011564:
	{ r19:r18 = memd(r29+152); r17:r16 = memd(r29+160) }
	{ r23:r22 = memd(r29+136); r21:r20 = memd(r29+144) }
	{ r27:r26 = memd(r29+120); r25:r24 = memd(r29+128) }
	{ dealloc_return }

;; xor_helper: 00011578
xor_helper proc
	{ jumpr r31; r0 = xor(r1,r0) }

;; logmsg_function: 00011580
;;   Called from:
;;     0001112C (in logical_int32_check)
;;     0001118C (in logical_int32_check)
;;     00011298 (in broadcast_elementwise_execute_int32)
;;     000113BC (in broadcast_elementwise_execute_int32)
;;     0001155C (in broadcast_elementwise_execute_int32)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000115A0; r5 = memw(r2+16) }

l00011590:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l000115A0:
	{ dealloc_return }

;; errlog_function: 000115A4
;;   Called from:
;;     0001114C (in logical_int32_check)
;;     000112C0 (in broadcast_elementwise_execute_int32)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }

;; ior_helper: 000115C0
;;   Called from:
;;     000115AC (in errlog_function)
ior_helper proc
	{ jumpr r31; r0 = or(r1,r0) }

;; and_helper: 000115C8
and_helper proc
	{ jumpr r31; r0 = and(r1,r0) }

;; avgpool_execute_asm: 000115D0
avgpool_execute_asm proc
	{ jump avgpool_execute; r2 = add(PC,000007D0) }

;; avgpool_check: 000115DC
avgpool_check proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r4 = add(PC,00014F4A) }
	{ r17 = r0; r16 = r1; r1 = 00000186 }
	{ memw(r29) = r17; memd(r29+32) = r19:r18; r3:r2 = combine(00000002,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,00000005)) jump:t 00011620; r2 = memw(r17+16); r1 = 00000187 }

l0001160C:
	{ r3 = add(PC,00000037) }

l00011610:
	{ r2 = r16; call errlog_function }

l00011614:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 00011718 }

l00011620:
	{ if (cmp.eq(r2.new,00000003)) jump:t 0001163C; r2 = memw(r17+20); r1 = 0000018B }

l00011630:
	{ r1 = 00000188; jump 00011614; r3 = add(PC,0000002A) }

l0001163C:
	{ memw(r29) = r17; r3:r2 = combine(00000003,r16); r4 = add(PC,00014F2F) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000116C4; r2 = memw(r17+16); r19:r18 = combine(00000000,00000000) }

l0001165C:
	{ r2 = memw(r17+4) }

l00011660:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001167C; r2 = memw(r5+r18) }

l0001166C:
	{ memw(r29) = r19; r1 = 0000018E; r3 = add(PC,00000000) }
	{ jump 00011610 }

l0001167C:
	{ r6 = memw(r2+4); r5 = memw(r2); r4 = add(PC,00014F72) }
	{ r8 = memw(r2+12); r7 = memw(r2+8); r1 = 00000180 }
	{ r2 = memw(r2+16); r9 = memw(r2+24) }
	{ memw(r29+12) = r7; memw(r29+24) = r2; r3:r2 = combine(00000003,r16) }
	{ memw(r29+16) = r8; memw(r29+20) = r9 }
	{ memw(r29+4) = r5; memw(r29+8) = r6 }
	{ memw(r29) = r19; call logmsg_function }
	{ r2 = memw(r17+16); r18 = add(r18,00000004) }
	{ if (cmp.gtu(r2,r19.new)) jump:t 0001165C; r19 = add(r19,00000001) }

l000116C4:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000116FC; r2 = memw(r17+20) }

l000116C8:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00011700 }

l000116D0:
	{ r3 = memw(r17+8) }
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 000116FC; r4 = add(r4,00000001); r3 = add(r3,00000004) }

l000116D8:
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 00011700; r4 = add(r4,00000001) }

l000116E4:
	{ if (!cmp.eq(r5.new,00000000)) jump:t 000116D8 }

l000116EC:
	{ memw(r29) = r4; r1 = 00000194; r3 = add(PC,00000016) }
	{ jump 00011610 }

l000116FC:
	{ r1 = 00000197; r3:r2 = combine(00000002,r16); r4 = add(PC,00014ED9) }

l00011700:
	{ r1 = 00000197; r3:r2 = combine(00000002,r16); r4 = add(PC,00000019) }

l0001170C:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l00011718:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ dealloc_return }

;; avgpool_execute_ref: 00011720
avgpool_execute_ref proc
	{ jump avgpool_execute; r2 = add(PC,00000290) }
0001172C                                     00 C0 00 7F             ....

;; avgpool_execute: 00011730
;;   Called from:
;;     000115D0 (in avgpool_execute_asm)
;;     00011720 (in avgpool_execute_ref)
avgpool_execute proc
	{ allocframe(00000088); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+112) = r21:r20; r3 = memw(r17+4) }
	{ r4 = memw(r17+8); r21 = memb(r17+32) }
	{ memd(r29+96) = r25:r24; memd(r29+88) = r27:r26 }
	{ r25 = memw(r3+16); r5 = memw(r3); p0 = cmp.eq(r21,00000000) }
	{ memw(r29+8) = r2; r2 = memw(r3+8) }
	{ r26 = memw(r3+12); r0 = p0 }
	{ memw(r29+24) = r2; r2 = memw(r3+4) }
	{ memd(r29+120) = r19:r18; memd(r29+104) = r23:r22 }
	{ memw(r29+12) = r2; r27 = memw(r4) }
	{ r2 = memw(r5+8); r23 = memw(r5) }
	{ r24 = memw(r5+12); r20 = memw(r5+4) }
	{ r22 = memw(r25+4); r1 = memw(r25+8) }
	{ r6 = memw(r4+4); r18 = memw(r26+4) }
	{ r7 = memw(r4+8) }
	{ memw(r29+20) = r7; memw(r29+16) = r6 }
	{ memw(r29+28) = r0; if (!p0) jump:nt 000117A4 }

l000117A0:
	{ jump 000117C8; r0 = r2 }

l000117A4:
	{ if (p0.new) r3 = memw(r26+8); if (p0.new) r2 = add(r1,r2); if (p0.new) jump:nt 000117C4; p0 = cmp.eq(r13,00000002) }

l000117B0:
	{ if (p0.new) r0 = add(r1,00000000); r19 = 00000000; if (!p0.new) jump:nt 000117D0; p0 = cmp.eq(r13,00000001) }

l000117BC:
	{ jump 000117C8; r0 += add(r2,FFFFFFFF) }

l000117C4:
	{ r0 = sub(r2,r3) }

l000117C8:
	{ call fn00009760 }
	{ r19 = r0 }

l000117D0:
	{ if (p0.new) r1 = add(r22,00000000); r2 = add(r22,r20); if (p0.new) jump:nt 00011800; p0 = cmp.eq(r13,00000002) }

l000117DC:
	{ if (!p0) r1:r0 = combine(r22,r22); if (p0.new) jump:nt 000117F8; p0 = cmp.eq(r13,00000001) }

l000117E4:
	{ r0 = memd(r29+28); r21 = 00000000 }
	{ if (!p0) r1:r0 = combine(r22,r20); if (!p0.new) jump:nt 0001180C; p0 = r0 }

l000117F4:
	{ jump 00011804 }

l000117F8:
	{ jump 00011804; r0 += add(r20,FFFFFFFF) }

l00011800:
	{ r0 = sub(r2,r18) }

l00011804:
	{ call fn00009760 }
	{ r21 = r0 }

l0001180C:
	{ memw(r29+32) = r17; r3:r2 = combine(00000000,00000000); r5 = add(r29,00000038); r4 = add(r29,00000020) }
	{ memw(r4+4) = FFFFFF81; r22 = add(r5,00000008); r20 = add(r4,00000008) }
	{ memd(r29+56) = r3:r2; memd(r29+64) = r3:r2; r1:r0 = combine(00000000,r20) }
	{ memw(r4+20) = 00000000; memd(r29+72) = r3:r2 }
	{ memw(r4+12) = 00000000; memw(r4+16) = 00000000 }
	{ memw(r29+56) = r17; memw(r4+8) = 00000000; call fn00009740 }
	{ r1:r0 = combine(00000000,r22); call fn00009740 }
	{ r1 = 00000151; r3:r2 = combine(00000002,r16); r4 = add(PC,00014C80) }
	{ memw(r29) = r17; call logmsg_function }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 0001187C; r2 = memw(r26) }

l00011868:
	{ if (!cmp.eq(r2.new,00000001)) jump:t 00011880 }

l00011870:
	{ if (!cmp.eq(r2.new,00000001)) jump:t 00011880 }

l00011878:
	{ if (cmp.eq(r2.new,00000001)) jump:t 0001189C }

l0001187C:
	{ r1 = 00000156; r3 = add(PC,00014C62) }

l00011880:
	{ r1 = 00000156; r3 = add(PC,00000022) }

l00011888:
	{ r2 = r16; call errlog_function }

l0001188C:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 0001199C }
00011898                         02 57 18 ED                     .W..    

l0001189C:
	{ r4 = memw(r27+20); r1 = 00000158 }
	{ r2 = mpyi(r2,r19) }
	{ if (!cmp.gtu(r3.new,r4)) jump:t 000118BC; r3 = mpyi(r2,r21) }

l000118B4:
	{ jump 0001188C; r3 = add(PC,00000006) }

l000118BC:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 000118D4; r2 = memb(r17+32); r1 = 00000159 }

l000118CC:
	{ jump 0001188C; r3 = add(PC,0000003C) }

l000118D4:
	{ memw(r27+4) = r21; r18 = memw(r29+8); r2 = add(r29,00000020) }
	{ memw(r27+12) = r24; memw(r27) = r23; r1:r0 = combine(r18,r16) }
	{ memw(r27+24) = r3; memw(r27+8) = r19 }
	{ call nn_os_work_for_vector }
	{ r0 = r16; r1 = add(r29,00000038); callr r18 }
	{ r0 = r20; call fn000096A0 }
	{ r4 = memd(r29+16); r5 = memd(r29+12) }
	{ r2 = memw(r5) }
	{ memb(r4+1) = r3.new; r3 = memw(r5+4) }
	{ r2 = memw(r5+8) }
	{ memw(r4+8) = r2 }
	{ r7 = memw(r5+12) }
	{ memw(r4+12) = r7 }
	{ r2 = memw(r5+24) }
	{ if (cmp.gtu(r2,r6.new)) jump:t 00011944; r6 = memw(r4+20) }

l0001193C:
	{ r1 = memw(r5+16); r2 = memw(r5+24); call fn00009560 }

l00011944:
	{ r5 = memd(r29+20); r4 = memd(r29+24) }
	{ r2 = memw(r4) }
	{ memb(r5+1) = r3.new; r3 = memw(r4+4) }
	{ r2 = memw(r4+8) }
	{ memw(r5+8) = r2 }
	{ r7 = memw(r4+12) }
	{ memw(r5+12) = r7 }
	{ r2 = memw(r4+24) }
	{ if (cmp.gtu(r2,r6.new)) jump:t 00011980; r6 = memw(r5+20) }

l00011978:
	{ r1 = memw(r4+16); r2 = memw(r4+24); call fn00009560 }

l00011980:
	{ r1 = 00000165; r3:r2 = combine(00000002,r16); r4 = add(PC,00014B96) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001199C:
	{ r19:r18 = memd(r29+120); r17:r16 = memd(r29+128) }
	{ r23:r22 = memd(r29+104); r21:r20 = memd(r29+112) }
	{ r27:r26 = memd(r29+88); r25:r24 = memd(r29+96) }
	{ dealloc_return }

;; avgpool_execute_slice_ref: 000119B0
avgpool_execute_slice_ref proc
	{ allocframe(+000000B8) }
	{ r3 = memw(r1+4); r2 = memw(r1) }
	{ memd(r29+176) = r17:r16; memd(r29+168) = r19:r18 }
	{ r19 = memb(r2+32); r7 = memw(r2+4) }
	{ memw(r29+24) = r3; r2 = memw(r2+8) }
	{ r4 = memw(r7+16); r16 = memw(r7); p0 = cmp.eq(r19,00000000) }
	{ memd(r29+152) = r23:r22; r3 = memw(r7+12) }
	{ r5 = memw(r16+8) }
	{ memd(r29+144) = r25:r24; memd(r29+160) = r21:r20 }
	{ memd(r29+136) = r27:r26; r20 = r5; r0 = r5 }
	{ memw(r29+4) = r1; r27 = memw(r2); r1 = p0 }
	{ r21 = memw(r4+4); r17 = memw(r4+8) }
	{ r22 = memw(r16+4); r25 = memw(r16+12); if (!p0) r2 = add(r17,r20) }
	{ r24 = memw(r3+8); r23 = memw(r3+4) }
	{ memb(r29+5) = r7.new; r7 = memw(r16) }
	{ memw(r29+116) = r1 }
	{ if (p0.new) r0 = sub(r2,r24); if (!p0.new) r26 = 00000000; if (p0.new) jump:nt 00011A38; p0 = cmp.eq(r11,00000002) }

l00011A2C:
	{ r0 = r17; if (!p0.new) jump:nt 00011A44; p0 = cmp.eq(r11,00000001) }

l00011A34:
	{ r0 += add(r20,FFFFFFFF) }

l00011A38:
	{ r1 = r17; call fn00009760 }
	{ r26 = r0 }

l00011A44:
	{ if (p0.new) r1 = add(r21,00000000); if (p0.new) jump:nt 00011A7C; p0 = cmp.eq(r11,00000002) }

l00011A4C:
	{ if (p0.new) r1 = add(r21,00000000); if (p0.new) jump:nt 00011A70; p0 = cmp.eq(r11,00000001) }

l00011A54:
	{ memb(r29+18) = r2.new; r0 = memw(r29+116); r2 = 00000000 }
	{ if (!p0) r1:r0 = combine(r21,r22); if (!p0.new) jump:nt 00011A90 }

l00011A6C:
	{ jump 00011A84 }

l00011A70:
	{ r0 = r1 }
	{ jump 00011A84; r0 += add(r22,FFFFFFFF) }

l00011A7C:
	{ r2 = add(r1,r22) }
	{ r0 = sub(r2,r23) }

l00011A84:
	{ call fn00009760 }
	{ memw(r29+72) = r0 }
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 00011D34; r2 = memw(r29+20); r3 = sub(r23,r22) }

l00011A90:
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 00011D38; r2 = memw(r29+20) }

l00011A9C:
	{ r2 = memw(r29+72); r5 = sub(r24,r20); r4 = add(r26,FFFFFFFF) }
	{ r7 = memd(r29+24); r2 = r2; r5 = mpyi(r25,r20); r4 = add(r5,mpyi(r4,r17)) }
	{ memw(r29+44) = r6; memw(r29+128) = r26; r4 += lsr(r4,0000001F); r2 = add(r3,mpyi(r2,r21)) }
	{ r7 = sub(FFFFFFFF,r22); r2 += lsr(r2,0000001F); r3 = mpyi(r7,r21) }
	{ memw(r29+40) = r7; r7 = memw(r16+16); r4 = asr(r4,00000001); r6 = asl(r21,00000001) }
	{ memw(r29+108) = r7; r26 = memw(r27+16); r7 = 00000000; r1 = asr(r2,00000001) }
	{ memw(r29+52) = r22; memw(r29+112) = r5; r5 = mpyi(r5,r22) }
	{ memw(r29+48) = r23; memw(r29+60) = r6; r6 = sub(FFFFFFFF,r20) }
	{ memw(r29+124) = r20; memw(r29+28) = r7; r3 = sub(r3,r1); r2 = add(r1,sub(0000007F,r3)) }
	{ memw(r29+12) = r3; r2 = sub(r2,r23); r3 = sub(00000000,r4); r7 = add(r4,sub(0000007F,r24)) }
	{ memw(r29+80) = r21; memw(r29+120) = r24 }
	{ memw(r29+16) = r5; memw(r29+96) = r6 }
	{ memw(r29+116) = r4; memw(r29+76) = r1 }
	{ memw(r29+32) = r3; memw(r29+36) = r7 }
	{ memw(r29+8) = r2 }

l00011B30:
	{ r2 = memw(r29+24) }
	{ if (!cmp.gt(r3.new,r2)) jump:t 00011D24; r3 = memw(r29+72) }

l00011B40:
	{ memw(r29+92) = r3; r7 = memd(r29+12) }
	{ r3 = memd(r29+72); r4 = memd(r29+112) }
	{ memw(r29+88) = r7; r5 = memd(r29+28); r2 = combine(r4.l,r2.l) }
	{ r6 = memd(r29+24); r7 = memd(r29+16); r1:r0 = combine(r4,r2) }
	{ memb(r29+14) = r3.new; r3 = mpyi(r5,r3) }
	{ memd(r29+64) = r1:r0 }
	{ memw(r29+100) = r3; jump 00011B90 }

l00011B70:
	{ r5 = memd(r29+72); r6 = memd(r29+84) }
	{ r3 = memd(r29+88); r2 = memd(r29+60); p0 = cmp.gt(r5,r6) }
	{ r7 = memd(r29+92); r3 = add(r3,r2) }
	{ memw(r29+88) = r3; r4 = sub(r7,r2) }
	{ memw(r29+92) = r4; if (!p0) jump:nt 00011D24 }

l00011B90:
	{ r2 = r6 }
	{ r7 = memd(r29+76); r3 = memd(r29+80); r4 = add(r2,00000002) }
	{ memw(r29+84) = r4; r5 = memd(r29+108); r3 = mpyi(r4,r3) }
	{ r1:r0 = memd(r29+64); r4 = memd(r29+112); r3 = sub(r3,r7) }
	{ r3 = add(r5,mpyi(r3,r4)) }
	{ l2fetch(r3,r1:r0) }
	{ if (!cmp.gt(r3.new,00000000)) jump:nt 00011B70; r3 = memw(r29+128); r27 = 00000000 }

l00011BC4:
	{ r4 = memd(r29+40); r3 = memd(r29+80); r18 = 00000000 }
	{ r7 = memd(r29+56); r5 = memd(r29+92) }
	{ r1 = memd(r29+48); r7 = memd(r29+88); r2 = add(r2,r7); r3 = mpyi(r2,r3) }
	{ r16 = memd(r29+32); r5 = memd(r29+76); r4 = max(r5,r4) }
	{ r7 = memw(r29+44); r4 = sub(FFFFFFFF,r4); r3 = sub(r3,r5); r5 = max(r7,r6) }
	{ r21 = memd(r29+36); r3 = memd(r29+52); r0 = add(r3,r1); r6 = max(r3,r6) }
	{ r7 = memw(r29+124); r22 = sub(r4,r5); r23 = mpyi(r7,r2) }
	{ r2 = min(r3,r0) }
	{ memb(r29+26) = r1.new; r20 = sub(r2,r6); r1 = mpyi(r6,r7) }
	{ r3 = memd(r29+116); r0 = 00008000 }
	{ r2 = memd(r29+124); r3 = memd(r29+120); r7 = sub(r2,r3) }
	{ r3 = add(r7,r3); r19 = max(r7,r18) }
	{ r2 = min(r2,r3) }
	{ r24 = sub(r2,r19) }
	{ call fn00009760; r1 = mpyi(r24,r20) }
	{ p0 = cmp.gt(r25,00000000); if (!p0.new) jump:nt 00011D0C; r8 = mpyi(r24,r25) }

l00011C54:
	{ r4 = memd(r29+104); r2 = memd(r29+96); r3 = mpyi(r27,r25); r7 = max(r16,r18) }
	{ r1 = memd(r29+112); r4 = 00000000; r6 = add(r19,r4) }
	{ r2 = memw(r29+108); r9 = mpyi(r6,r25); r5 = max(r21,r2) }
	{ r8 = memw(r29+100); r5 = sub(r1,r8); r6 = sub(FFFFFFFF,r5) }
	{ r6 = sub(r6,r7); r12 = sub(r6,r7); r2 += add(r9,r8) }
	{ r7 = mpyi(r25,r12) }

l00011C90:
	{ r8 = 00000000; if (!p0.new) jump:nt 00011CE4; p0 = cmp.gt(r12,00000000) }

l00011C98:
	{ r9:r8 = combine(r2,00000000); loop1(00011CA0,r22) }
	{ p0 = cmp.gt(r24,00000000); r12 = add(r9,r4); r14 = add(r6,FFFFFFFF); if (!p0.new) jump:nt 00011CD8 }

l00011CB0:
	{ p0 = cmp.gtu(r6,00000001); loop0(00011CC4,r14) }
	{ r12 = memb(r12); r13 = add(r12,r25); if (!p0) jump:nt 00011CD0 }

l00011CC4:
	{ r12 = memb(r13); r13 = add(r13,r25); r8 = add(r12,r8) }

l00011CD0:
	{ r9 = add(r9,r7); r8 = add(r12,r8) }

l00011CD8:
	{ nop; nop; r9 = add(r9,r5) }

l00011CE4:
	{ r12 = r26; r9 = add(r4,r3); r8 = add(00004000,mpyi(r8,r0)) }
	{ r4 = add(r4,00000001); r12 += add(r9,r23) }
	{ p0 = cmp.eq(r4,r25); r8 = lsr(r8,0000000F) }
	{ memb(r12) = r8; if (!p0) jump:nt 00011C90 }

l00011D0C:
	{ r2 = memw(r29+128); r21 = sub(r21,r17); r16 = add(r16,r17) }
	{ if (cmp.eq(r27.new,r2)) jump:nt 00011B70; r27 = add(r27,00000001) }

l00011D24:
	{ r2 = memd(r29+20); r3 = memd(r29+28) }
	{ r3 = add(r3,00000001) }
	{ memw(r29+28) = r3; if (!p0.new) jump:nt 00011B30; p0 = cmp.eq(r3,r2) }

l00011D34:
	{ r17:r16 = memd(r29+176); r2 = memd(r29+4); r1 = 00000001 }

l00011D38:
	{ r17:r16 = memd(r29+176); r2 = memd(r29+4) }

l00011D3C:
	{ r21:r20 = memd(r29+160); r19:r18 = memd(r29+168); r0 = add(r2,00000008) }
	{ r25:r24 = memd(r29+144); r23:r22 = memd(r29+152) }
	{ deallocframe; r27:r26 = memd(r29+136); jump 00009730 }

;; logmsg_function: 00011D58
;;   Called from:
;;     000115F0 (in avgpool_check)
;;     0001164C (in avgpool_check)
;;     000116B0 (in avgpool_check)
;;     0001170C (in avgpool_check)
;;     00011854 (in avgpool_execute)
;;     00011990 (in avgpool_execute)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00011D78; r5 = memw(r2+16) }

l00011D68:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,00000003) }
	{ memw(r29+4) = r6; call logv }

l00011D78:
	{ dealloc_return }

;; errlog_function: 00011D7C
;;   Called from:
;;     00011610 (in avgpool_check)
;;     00011888 (in avgpool_execute)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0001472B) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; avgpool_execute_slice_asm: 00011DA0
avgpool_execute_slice_asm proc
	{ allocframe(+00000098) }
	{ r3 = memw(r1+4); r2 = memw(r1) }
	{ memd(r29+112) = r25:r24; memd(r29+136) = r19:r18 }
	{ r18 = memb(r2+32); r7 = memw(r2+4) }
	{ memd(r29+144) = r17:r16; r2 = memw(r2+8) }
	{ r4 = memw(r7+16); r24 = memw(r7); p0 = cmp.eq(r18,00000000) }
	{ memw(r29+28) = r3; r3 = memw(r7+12) }
	{ r17 = memw(r24+8) }
	{ memd(r29+104) = r27:r26; memd(r29+128) = r21:r20 }
	{ r7 = memw(r24); r16 = memw(r2); r0 = r17 }
	{ r26 = memw(r4+8); r2 = memw(r4+4) }
	{ r19 = memw(r24+4); r20 = memw(r24+12) }
	{ memw(r29+24) = r7; memw(r29+12) = r1; r1 = p0 }
	{ memw(r29+80) = r2; r2 = memw(r3+4) }
	{ r7 = memw(r3+8) }
	{ memw(r29+48) = r2; memd(r29+120) = r23:r22; if (!p0) r2 = add(r26,r17) }
	{ memw(r29+92) = r1; memw(r29+96) = r7 }
	{ if (p0) jump:nt 00011E34 }

l00011E14:
	{ if (p0.new) r3 = memw(r29+96); if (!p0.new) r0 = add(r26,00000000); if (p0.new) jump:nt 00011E30; p0 = cmp.eq(r10,00000002) }

l00011E20:
	{ r21 = 00000000; if (!p0.new) jump:nt 00011E40; p0 = cmp.eq(r10,00000001) }

l00011E28:
	{ jump 00011E34; r0 += add(r17,FFFFFFFF) }

l00011E30:
	{ r0 = sub(r2,r3) }

l00011E34:
	{ r1 = r26; call fn00009760 }
	{ r21 = r0 }

l00011E40:
	{ nop; if (p0.new) jump:nt 00011E70; p0 = cmp.eq(r10,00000002) }

l00011E48:
	{ if (p0.new) r1 = memw(r29+80); if (p0.new) jump:nt 00011E64; p0 = cmp.eq(r10,00000001) }

l00011E50:
	{ r1 = memd(r29+92); r0 = 00000000 }
	{ if (!p0.new) jump:nt 00011E80; p0 = r1 }

l00011E5C:
	{ r1 = memw(r29+80); jump 00011E7C; r0 = r11 }

l00011E64:
	{ r0 = r1 }
	{ jump 00011E7C; r0 += add(r19,FFFFFFFF) }

l00011E70:
	{ r3 = memd(r29+48); r1 = memd(r29+80) }
	{ r2 = add(r1,r19) }
	{ r0 = sub(r2,r3) }

l00011E7C:
	{ call fn00009760 }

l00011E80:
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 00012050; r2 = memw(r29+24); r6 = mpyi(r21,r20); r7 = mpyi(r20,r17) }

l00011E94:
	{ r5 = memd(r29+48); r3 = memd(r29+96); r4 = add(r0,FFFFFFFF) }
	{ r1 = memw(r29+80); r8 = and(r20,0000007F); r5 = sub(r5,r19); r3 = sub(r3,r17) }
	{ r16 = memw(r24+16); r3 = memw(r16+16); r9 = 00000000; r2 = add(r3,mpyi(r2,r26)) }
	{ memw(r29+64) = r7; memw(r29+76) = r21; r5 = mpyi(r0,r21); r4 = add(r5,mpyi(r4,r1)) }
	{ memb(r29+13) = r1.new; r1 = asl(r6,00000001); r2 += lsr(r2,0000001F) }
	{ memw(r29+44) = r19; r1 = memd(r29+28); r4 += lsr(r4,0000001F) }
	{ memw(r29+68) = r0; memw(r29+20) = r5; r2 = asr(r2,00000001); r5 = mpyi(r7,r19) }
	{ memw(r29+92) = r8; r2 = sub(00000000,r2); r3 = asr(r4,00000001); r6 = add(r3,mpyi(r6,r1)) }
	{ memw(r29+16) = r5; memw(r29+32) = r9 }
	{ memw(r29+72) = r3; memw(r29+36) = r6 }
	{ memw(r29+40) = r2 }

l00011F0C:
	{ r2 = memw(r29+28) }
	{ if (!cmp.gt(r3.new,r2)) jump:t 00012034; r3 = memw(r29+68) }

l00011F1C:
	{ r5 = memd(r29+16); r7 = memd(r29+32); r2 = combine(r3.l,r2.l) }
	{ memb(r29+22) = r4.new; r4 = memw(r29+36); r1:r0 = combine(r3,r2); r22 = mpyi(r5,r7) }
	{ jump 00011F54 }

l00011F3C:
	{ r7 = memd(r29+68); r6 = memd(r29+84) }
	{ r3 = memd(r29+88); r2 = memd(r29+52); p0 = cmp.gt(r7,r6) }
	{ r3 = add(r3,r2) }
	{ memw(r29+88) = r3; if (!p0) jump:nt 00012034 }

l00011F54:
	{ r2 = r6 }
	{ r7 = memd(r29+72); r3 = memd(r29+80); r4 = add(r2,00000002) }
	{ memw(r29+84) = r4; r1:r0 = memd(r29+56); r3 = mpyi(r4,r3) }
	{ r4 = memw(r29+64); r3 = sub(r3,r7) }
	{ r3 = add(r16,mpyi(r3,r4)) }
	{ l2fetch(r3,r1:r0) }
	{ if (!cmp.gt(r3.new,00000000)) jump:nt 00011F3C; r3 = memw(r29+76) }

l00011F84:
	{ r19 = memd(r29+76); r4 = memd(r29+48) }
	{ r21 = memw(r29+40); r24 = memw(r29+88); r3 = 00000000; r2 = mpyi(r2,r3) }
	{ r7 = memw(r29+44); r2 = sub(r2,r7) }
	{ r2 = add(r2,r4); r3 = max(r2,r3) }
	{ r18 = mpyi(r3,r17); r2 = min(r7,r2) }
	{ r25 = sub(r2,r3) }
	{ r2 = memd(r29+96); r3 = 00000000; r23 = r26 }
	{ r2 = add(r2,r21); r0 = 00008000; r27 = max(r21,r3) }
	{ r2 = min(r17,r2) }
	{ r26 = sub(r2,r27) }
	{ call fn00009760; r1 = mpyi(r26,r25) }
	{ r1 = r16; r2 = add(r27,r18); r5:r4 = combine(r17,r25) }
	{ r2 = mpyi(r2,r20) }
	{ if (!cmp.eq(r3.new,00000000)) jump:t 0001200C; r3 = memw(r29+92); r1 += add(r2,r22) }

l00011FF8:
	{ memw(r29) = r0; r0 = r24; r3:r2 = combine(r26,r20) }
	{ call avgpool_aligned_hvx }
	{ jump 0001201C }

l0001200C:
	{ memw(r29) = r0; r0 = r24; r3:r2 = combine(r26,r20) }
	{ call avgpool_nonaligned_hvx }

l0001201C:
	{ r24 = add(r24,r20); r26 = r23 }
	{ if (cmp.eq(r19.new,00000000)) jump:nt 00011F3C; r19 = add(r19,FFFFFFFF); r21 = add(r21,r26) }

l00012034:
	{ r7 = memd(r29+24); r3 = memd(r29+32) }
	{ r4 = memd(r29+20); r2 = memd(r29+36); r3 = add(r3,00000001) }
	{ memw(r29+32) = r3; r2 = add(r2,r4); p0 = cmp.eq(r3,r7) }
	{ memw(r29+36) = r2; if (!p0) jump:nt 00011F0C }

l00012050:
	{ r17:r16 = memd(r29+144); r2 = memd(r29+12); r1 = 00000001 }
	{ r21:r20 = memd(r29+128); r19:r18 = memd(r29+136); r0 = add(r2,00000008) }
	{ r25:r24 = memd(r29+112); r23:r22 = memd(r29+120) }
	{ deallocframe; r27:r26 = memd(r29+104); jump 00009730 }
00012074             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; biasadd_8p8to32_execute: 00012080
biasadd_8p8to32_execute proc
	{ allocframe(+00000090) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memw(r29+92) = r1; memd(r29+136) = r17:r16 }
	{ r4 = memw(r2+8); r5 = memw(r2+12) }
	{ r6 = memw(r2); r8 = memw(r2+16) }
	{ r17 = memw(r2+4); r1 = memw(r2+20) }
	{ r5 = memw(r5+16); r2 = memw(r4+16) }
	{ memd(r29+96) = r27:r26; memd(r29+120) = r21:r20 }
	{ r27 = memw(r5); r21 = memw(r2) }
	{ memw(r29+76) = r0; r0 = memw(r1+16) }
	{ memd(r29+128) = r19:r18 }
	{ memd(r29+112) = r23:r22; r18 = 437F0000; r23 = sfsub(r27,r21) }
	{ r2 = memw(r3+8); r7 = memw(r8+16) }
	{ r22 = memw(r0); r19 = memw(r3); r1:r0 = combine(r18,r23) }
	{ memw(r29+72) = r2; r5 = memw(r3+4) }
	{ r2 = memw(r6+8) }
	{ memw(r29+68) = r5; memd(r29+104) = r25:r24 }
	{ memw(r29+84) = r2; r5 = memw(r6+4) }
	{ r26 = memw(r6+16); r2 = memw(r17+16) }
	{ r20 = memw(r6); r24 = memw(r6+12) }
	{ memw(r29+88) = r2; memw(r29+80) = r5 }
	{ r25 = memw(r19+16) }
	{ memw(r29+64) = r22; r16 = memw(r7); call fn00009610 }
	{ r18 = r0; r1 = r18; r22 = sfsub(r22,r16) }
	{ r0 = r22; call fn00009610 }
	{ r1 = 00000066; r3 = add(PC,000145AC) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 000121C4; r2 = memw(r17+4) }

l00012130:
	{ r1 = 00000067; r3 = add(PC,00000018) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 000121C4; r2 = memw(r17) }

l00012144:
	{ r1 = 00000068; r3 = add(PC,00000004) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 000121C4; r2 = memw(r17+8) }

l00012158:
	{ memw(r29+60) = r0; r4 = memw(r17+12) }
	{ if (p0.new) memw(r29+4) = r24; if (!p0.new) r2 = memw(r29+92); p0 = cmp.eq(r4,r24); if (p0.new) jump:nt 00012180 }

l0001216C:
	{ memw(r29) = r4; r1 = 0000006A; r3 = add(PC,00014563) }
	{ jump 000121C8 }

l00012180:
	{ r3 = memd(r29+84); r2 = memd(r29+80); r4 = add(PC,00014567) }
	{ r2 = mpyi(r20,r2) }
	{ r2 = memw(r29+76); r5 = mpyi(r2,r3) }
	{ memw(r29) = r2; r2 = memd(r29+92); r3 = 00000002; r5 = mpyi(r5,r24) }
	{ call logmsg_function; r17 = asl(r5,00000002) }
	{ r1 = 0000006E; r0 = clrbit(r21,0000001E); r3 = add(PC,00014555) }
	{ if (!cmp.gtu(r17,r2.new)) jump:t 000121D4; r2 = memw(r19+20) }

l000121C4:
	{ r2 = memw(r29+92) }

l000121C8:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00012470 }

l000121D4:
	{ r7 = memd(r29+84); r3 = memd(r29+80); r1 = r27 }
	{ memw(r29+48) = r19; memw(r29+36) = r1 }
	{ memw(r19+24) = r17 }
	{ memw(r29+52) = r20 }
	{ memw(r19+4) = r3; memw(r19) = r20 }
	{ memw(r29+56) = r24; memw(r19+8) = r7 }
	{ memw(r19+12) = r24; call fn00009600 }
	{ r20 = memd(r29+64); r19 = r0; r2 = clrbit(r16,0000001E) }
	{ r1:r0 = combine(r20,r2); call fn00009600 }
	{ r1:r0 = combine(r0,r19); call fn00009600 }
	{ r5 = C8000000; r3 = 48000000 }
	{ r4 = 2F800000; r2 = 38D1B717 }
	{ memb(r29+10) = r5.new; r1:r0 = combine(r23,r2); r5 = sfmpy(r0,r5); r3 = sfmpy(r0,r3) }
	{ memw(r29+32) = r3 }
	{ call fn00009600; r19 = sfmpy(r2,r4) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ memw(r29+44) = r17; r17 = 00000000 }
	{ r2 = sfsub(r17,r21) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = 38D1B717; r27 = r18; r2 = convert_uw2sf(r0):chop }
	{ r1 = r22; r23 = 00000000; p1 = cmp.gt(r2,FFFFFFFF) }
	{ if (!p0.new) r18 = zxtb(r2); if (p0.new) r18 = 000000FF; p0 = cmp.gt(r2,000000FF) }
	{ if (!p1) r18 = add(r23,00000000); call fn00009600 }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r17,r16) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r1:r0 = combine(r19,r27); r2 = convert_uw2sf(r0):chop }
	{ call fn00009610; r17 = max(r2,r23) }
	{ r19 = memd(r29+60); r1 = r19; r22 = r0; r24 = r19 }
	{ r0 = r19; call fn00009610 }
	{ r16 = memd(r29+92); r2 = memd(r29+36); r9:r8 = convert_sf2df(r16); r7:r6 = convert_sf2df(r20) }
	{ r1 = 0000008C; r15:r14 = convert_sf2df(r21); r4 = add(PC,00014423) }
	{ memd(r29+24) = r7:r6; r20 = r0; r3:r2 = combine(00000009,r16); r13:r12 = convert_sf2df(r2) }
	{ memd(r29+8) = r13:r12; memd(r29+16) = r9:r8 }
	{ memd(r29) = r15:r14; call logmsg_function }
	{ r1 = 0000008D; r3:r2 = combine(00000009,r16); r9:r8 = convert_sf2df(r19); r7:r6 = convert_sf2df(r24) }
	{ memd(r29+16) = r7:r6; r13:r12 = convert_sf2df(r27); r4 = add(PC,00014415) }
	{ memd(r29) = r13:r12; memd(r29+8) = r9:r8 }
	{ call logmsg_function }
	{ r12 = memw(r29+68); r9:r8 = convert_sf2df(r22); r7:r6 = convert_sf2df(r20) }
	{ r3 = memd(r29+40); r5 = memd(r29+72); r4 = add(PC,00014417) }
	{ r0 = memw(r29+48); r1 = 00000099 }
	{ memw(r12+12) = FFFFFF81; r2 = memw(r12+16) }
	{ memw(r12+4) = FFFFFF81; memw(r12+8) = 00000001 }
	{ memw(r2) = r3; memw(r12) = 00000001 }
	{ r13 = memw(r29+32); r2 = memw(r5+16) }
	{ memw(r5+8) = 00000001; memw(r5+4) = 00000001 }
	{ memw(r5+12) = 00000001; memw(r5) = 00000001 }
	{ memw(r2) = r13; r21 = memw(r29+52); r3:r2 = combine(00000009,r16) }
	{ r27 = memw(r29+80); r19 = memw(r29+84) }
	{ r24 = memw(r29+56); r13 = memw(r29+44) }
	{ memw(r0+8) = r19; memw(r0) = r21 }
	{ memw(r0+24) = r13; memw(r0+4) = r27 }
	{ memw(r12+24) = 00000004; memw(r0+12) = r24 }
	{ memd(r29+24) = r7:r6; memw(r5+24) = 00000004 }
	{ memd(r29+8) = r9:r8; memw(r29+16) = r17 }
	{ memw(r29) = r18; call logmsg_function }
	{ r16 = r24; r2 = mpyi(r27,r21) }
	{ r2 = mpyi(r2,r19) }
	{ memw(r29+84) = r2; p0 = cmp.eq(r2,00000000) }
	{ nop; if (p0) jump:nt 0001244C }

l000123EC:
	{ r19 = memw(r29+88); r24 = r25; r27 = r16; r21 = r26 }
	{ if (p0.new) jump:nt 00012438; p0 = cmp.eq(r8,00000000) }

l00012400:
	{ r3 = memb(r21++#1); r2 = memb(r19++#1) }
	{ r3 = sub(r3,r18); r2 = sub(r2,r17) }
	{ r3 = convert_w2sf(r3); r2 = convert_w2sf(r2) }
	{ r0 = sfmpy(r20,r2) }
	{ r27 = add(r27,FFFFFFFF); call fn00009620; r0 += sfmpy(r22,r3) }
	{ memw(r24++#4) = r2.new; p0 = cmp.eq(r27,00000000); if (!p0.new) jump:nt 00012400; r2 = convert_uw2sf(r0):chop }

l00012438:
	{ r2 = memw(r29+84); r26 = add(r26,r16); r25 = addasl(r25,r16,00000002) }

l0001243C:
	{ r2 = memw(r29+84); r26 = add(r26,r16) }

l00012444:
	{ if (!cmp.eq(r23.new,r2)) jump:t 000123EC; r23 = add(r23,00000001) }

l0001244C:
	{ memb(r29) = r2.new; r2 = memw(r29+76); r4 = add(PC,0001434A) }

l00012450:
	{ memb(r29) = r2.new; r2 = memw(r29+76); r4 = add(PC,0000000A) }

l00012460:
	{ r1 = 000000AC }
	{ r2 = memw(r29+92); call logmsg_function }
	{ r0 = 00000000 }

l00012470:
	{ r19:r18 = memd(r29+128); r17:r16 = memd(r29+136) }
	{ r23:r22 = memd(r29+112); r21:r20 = memd(r29+120) }
	{ r27:r26 = memd(r29+96); r25:r24 = memd(r29+104) }
	{ dealloc_return }

;; biasadd_check: 00012484
biasadd_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000141B5) }
	{ r17 = r0; r16 = r1; r1 = 000000B3 }
	{ memw(r29) = r17; r3:r2 = combine(00000002,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 000124C8; r2 = memw(r17+16) }

l000124B0:
	{ r1 = 000000B4; r3 = add(PC,00000026) }

l000124B8:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l000124C8:
	{ r2 = memw(r17+20) }
	{ if (!p0.new) r1 = 000000B5; if (!p0.new) jump:nt 000124FC; p0 = cmp.eq(r2,00000003) }

l000124D4:
	{ r2 = memw(r17+4); r4 = 00000000; loop0(000124E0,00000006) }
	{ r3 = r2 }
	{ if (!cmp.eq(r5.new,00000000)) jump:t 00012508; r5 = memw(r3) }

l000124EC:
	{ memw(r29) = r4; r1 = 000000B8; r3 = add(PC,00000016) }
	{ jump 000124B8 }

l000124FC:
	{ jump 000124B8; r3 = add(PC,0001416D) }

l00012508:
	{ r4 = r4; r3 = add(r3,00000004); nop }
	{ jump 00012520; r0 = 00000000 }

l00012514:
	{ if (cmp.gtu(r4.new,00000005)) jump:nt 0001253C; r4 = add(r4,00000001); r2 = add(r2,00000004) }

l00012518:
	{ if (cmp.gtu(r4.new,00000005)) jump:nt 00012540; r4 = add(r4,00000001) }

l00012520:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 00012514; r3 = memw(r2) }

l00012524:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 00012518 }

l0001252C:
	{ memw(r29) = r4; r1 = 000000BD; r3 = add(PC,0000002C) }
	{ jump 000124B8 }

l0001253C:
	{ r1 = 000000C0; r3:r2 = combine(00000002,r16); r4 = add(PC,0001416F) }

l00012540:
	{ r1 = 000000C0; r3:r2 = combine(00000002,r16); r4 = add(PC,0000002F) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001255C
;;   Called from:
;;     000121A4 (in biasadd_8p8to32_execute)
;;     00012310 (in biasadd_8p8to32_execute)
;;     00012340 (in biasadd_8p8to32_execute)
;;     000123CC (in biasadd_8p8to32_execute)
;;     00012464 (in biasadd_8p8to32_execute)
;;     00012498 (in biasadd_check)
;;     0001254C (in biasadd_check)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001257C; r5 = memw(r2+16) }

l0001256C:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,00000034) }
	{ memw(r29+4) = r6; call logv }

l0001257C:
	{ dealloc_return }

;; errlog_function: 00012580
;;   Called from:
;;     000121C8 (in biasadd_8p8to32_execute)
;;     000124B8 (in biasadd_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0001409C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
000125A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; check_execute: 000125B0
check_execute proc
	{ allocframe(00000018); memd(r29+496) = r17:r16; r4 = add(PC,0001426C) }
	{ r1 = 00000036; r17:r16 = combine(r0,r1) }
	{ memd(r29+8) = r19:r18; r3 = memw(r17+4); r2 = r16 }
	{ r18 = memw(r3) }
	{ memw(r29) = r17; r19 = memw(r3+4); call logmsg_function }
	{ r4 = memw(r18); r1 = 00000038; r3 = add(PC,0001425C) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 0001263C; r2 = memw(r19) }

l000125F0:
	{ r4 = memw(r18+4); r1 = 00000039; r3 = add(PC,0000002B) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 0001263C; r2 = memw(r19+4) }

l00012604:
	{ r4 = memw(r18+8); r1 = 0000003A; r3 = add(PC,00000039) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 0001263C; r2 = memw(r19+8) }

l00012618:
	{ r4 = memw(r18+12); r1 = 0000003B; r3 = add(PC,00000006) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 0001263C; r2 = memw(r19+12) }

l0001262C:
	{ r4 = memw(r18+24); r1 = 0000003C; r3 = add(PC,00000013) }
	{ if (cmp.eq(r2.new,r4)) jump:t 00012644; r2 = memw(r19+24) }

l0001263C:
	{ memw(r29) = r4; memw(r29+4) = r2; jump 00012664; r2 = r8 }

l00012640:
	{ memw(r29) = r4; memw(r29+4) = r2 }

l00012644:
	{ r1 = memw(r19+16); r0 = memw(r18+16); r2 = r4; call fn000097A0 }
	{ if (p0.new) memw(r29) = r17; if (p0.new) jump:nt 00012670; p0 = cmp.eq(r0,00000000) }

l00012658:
	{ r2 = r16; r1 = 0000003E; r3 = add(PC,00014282) }

l00012664:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00012688 }

l00012670:
	{ r2 = r16; r1 = 00000040; r4 = add(PC,00014278) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00012688:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }
	{ dealloc_return }

;; check_check: 00012690
check_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00014131) }
	{ r17 = r0; r16 = r1; r1 = 00000046 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 000126D0; r2 = memw(r17+16); r1 = 00000047 }

l000126BC:
	{ r3 = add(PC,00000020) }
	{ r2 = r16; call errlog_function }

l000126C4:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l000126D0:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000126E8; r2 = memw(r17+20); r1 = 00000048 }

l000126E0:
	{ jump 000126C4; r3 = add(PC,00000012) }

l000126E8:
	{ r2 = r16; r1 = 00000049; r4 = add(PC,0001411D) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00012708
;;   Called from:
;;     000125D0 (in check_execute)
;;     00012680 (in check_execute)
;;     000126A4 (in check_check)
;;     000126F8 (in check_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001272C; r5 = memw(r2+16) }

l00012718:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000012) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001272C:
	{ dealloc_return }

;; errlog_function: 00012730
;;   Called from:
;;     00012664 (in check_execute)
;;     000126C0 (in check_check)
;;     00012720 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00014076) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00012754             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; close_execute_f: 00012760
close_execute_f proc
	{ allocframe(00000078); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1); r4 = 00000003 }
	{ memd(r29+104) = r19:r18; r2 = memw(r17+4) }
	{ r3 = memw(r17+16) }
	{ memd(r29+88) = r23:r22; memd(r29+96) = r21:r20; p0 = cmp.gtu(r4,r3) }
	{ r19 = memw(r2); r18 = memw(r2+4) }
	{ memd(r29+72) = r27:r26; memd(r29+80) = r25:r24; if (p0) r24 = 3D4CCCCD }
	{ r22 = memw(r18+16); r5 = memw(r18+24) }
	{ if (!p0) r2 = memw(r2+8) }
	{ r20 = memw(r19+16); if (!p0) r2 = memw(r2+16) }
	{ if (p0) jump:nt 000127AC; r21 = lsr(r5,00000002) }

l000127A8:
	{ r24 = memw(r2) }

l000127AC:
	{ r1 = 00000061; r3:r2 = combine(00000002,r16); r4 = add(PC,00014377) }
	{ memw(r29) = r17; call logmsg_function }
	{ r4 = memw(r19); r1 = 00000064; r3 = add(PC,000141C9) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012868; r2 = memw(r18) }

l000127E0:
	{ r4 = memw(r19+4); r1 = 00000065; r3 = add(PC,00000015) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012868; r2 = memw(r18+4) }

l000127F8:
	{ r4 = memw(r19+8); r1 = 00000066; r3 = add(PC,00000020) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012868; r2 = memw(r18+8) }

l00012810:
	{ r4 = memw(r19+12); r1 = 00000067; r3 = add(PC,0000002A) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012868; r2 = memw(r18+12) }

l00012828:
	{ r4 = memw(r19+24); r1 = 00000068; r3 = add(PC,00000034) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012868; r2 = memw(r18+24); r25 = 00000000; p0 = cmp.eq(r21,00000000) }

l00012848:
	{ if (!p0) memw(r29+68) = r26.new; if (!p0) r27 = add(r22,00000000); r26 = 00000000 }
	{ if (p0) r19:r18 = combine(r26,r26); if (p0.new) r19:r18 = combine(r26,r26) }
	{ memw(r29+68) = r20; memw(r29+64) = r22; jump 000128C0 }

l00012868:
	{ memw(r29) = r4; memw(r29+4) = r2; r2 = r16 }

l00012870:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00012940 }
0001287C                                     0E 29 C7 7D             .).}
00012880 14 C0 9B 91 BE 76 FE 5B 00 D4 13 F5 92 77 FE 5B .....v.[.....w.[
00012890 13 40 60 70 00 D4 12 F5 9B 40 1B B0 96 40 16 B0 .@`p.....@...@..
000128A0 E2 00 0A 50 22 D4 02 EB 22 DF C2 8C 1A 5A 82 EB ...P"..."....Z..
000128B0 80 5A E2 C7 19 E0 17 74 37 40 17 B0 E4 F5 B2 21 .Z.....t7@.....!

l000128C0:
	{ r2 = sfsub(r19,r18) }
	{ r3 = sfmpy(r24,r2) }
	{ if (p0.new) memw(r29) = r17; if (!p0.new) r1 = 0000007C; if (!p0.new) jump:nt 0001292C; p0 = sfcmp.gt(r26,r3) }

l000128D8:
	{ r5 = memd(r29+68); r4 = memd(r29+64); r9:r8 = convert_sf2df(r2); r7:r6 = convert_sf2df(r3) }
	{ r1 = 0000007A; r13:r12 = convert_sf2df(r26); r3 = add(PC,00014263) }
	{ r5 = addasl(r5,r25,00000002); r4 = addasl(r4,r25,00000002) }
	{ r4 = memw(r4) }
	{ memd(r29+56) = r7:r6; r2 = memw(r5); r5:r4 = convert_sf2df(r4) }
	{ memd(r29+24) = r5:r4; memd(r29+48) = r9:r8 }
	{ memw(r29+4) = r21; r2 = r16; r7:r6 = convert_sf2df(r2) }
	{ memd(r29+32) = r5:r4; memd(r29+40) = r13:r12 }
	{ memd(r29+8) = r7:r6; memd(r29+16) = r7:r6 }
	{ memw(r29) = r25; jump 00012870 }

l0001292C:
	{ r3:r2 = combine(00000002,r16); r4 = add(PC,000141B8) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00012940:
	{ r19:r18 = memd(r29+104); r17:r16 = memd(r29+112) }
	{ r23:r22 = memd(r29+88); r21:r20 = memd(r29+96) }
	{ r27:r26 = memd(r29+72); r25:r24 = memd(r29+80) }
	{ dealloc_return }

;; close_check_f: 00012954
close_check_f proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000141A1) }
	{ r17 = r0; r16 = r1; r1 = 000000F6 }
	{ memw(r29) = r17; r3:r2 = combine(00000002,r16); call logmsg_function }
	{ if (cmp.gtu(r2.new,00000001)) jump:t 00012998; r2 = memw(r17+16); r1 = 000000F7 }

l00012984:
	{ r3 = add(PC,0000002D) }
	{ r2 = r16; call errlog_function }

l0001298C:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00012998:
	{ if (cmp.gtu(r3.new,r2)) jump:t 000129B0; r3 = 00000004; r1 = 000000F8 }

l000129A8:
	{ jump 0001298C; r3 = add(PC,00000009) }

l000129B0:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000129C8; r2 = memw(r17+20); r1 = 000000F9 }

l000129C0:
	{ jump 0001298C; r3 = add(PC,00000007) }

l000129C8:
	{ r1 = 000000FA; r3:r2 = combine(00000002,r16); r4 = add(PC,00014144) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; close_execute_i32: 000129E8
close_execute_i32 proc
	{ jump close_execute; r2 = add(PC,0000059C) }

;; close_check: 000129F4
close_check proc
	{ jump close_check__merged; r0 = 00000000 }
000129F8                         00 40 00 7F 00 C0 00 7F         .@......

;; close_check__merged: 00012A00
;;   Called from:
;;     000129F4 (in close_check)
;;     00012D48 (in close_check_q)
close_check__merged proc
	{ allocframe(00000020); p0 = cmp.eq(r2,00000001); r4 = add(PC,00013F14) }
	{ r6 = 00000006; r5 = 000000ED; r7 = add(PC,000140E9) }
	{ memd(r29+16) = r19:r18; memd(r29+24) = r17:r16; r16 = r1; r8 = 00000002 }
	{ r1 = mux(p0,00000100,r5); if (!p0) r4 = add(r7,00000000); r3:r2 = combine(00000002,r16) }
	{ memb(r29+2) = r19.new; r17 = r0; r19 = p0 }
	{ memw(r29) = r17; r18 = mux(p0,r6,r8) }
	{ if (cmp.eq(r2.new,r18)) jump:t 00012A74; r2 = memw(r17+16) }

l00012A5C:
	{ r0 = memd(r29+8); r2 = r16; r3 = add(PC,00000015) }
	{ if (!p0.new) r1 = 000000EE; if (p0.new) r1 = 00000101; jump 00012A9C; p0 = r0 }

l00012A74:
	{ r0 = memw(r29+8) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00012AB4; r2 = memw(r17+20); p1 = r0 }

l00012A88:
	{ if (!p1) r1 = 000000EF; if (p1) r1 = 00000102 }
	{ r3 = add(PC,00013EB3) }
	{ r2 = r16 }

l00012A9C:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00012AE8 }
00012AA8                         FA 44 00 00 83 4D 49 6A         .D...MIj
00012AB0 F8 FF FF 59                                     ...Y            

l00012AB4:
	{ if (p1) jump:nt 00012ACC }

l00012AB8:
	{ r1 = 000000F0; r3 = 00000002; r4 = add(PC,00014054) }
	{ jump 00012ADC }

l00012ACC:
	{ r1 = 00000103; r3 = 00000002; r4 = add(PC,00013E8E) }

l00012ADC:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r0 = 00000000 }

l00012AE8:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }
	{ dealloc_return }

;; close_execute_u8: 00012AF0
close_execute_u8 proc
	{ jump close_execute; r2 = add(PC,0000039C) }
00012AFC                                     00 C0 00 7F             ....

;; close_execute_q_u8: 00012B00
close_execute_q_u8 proc
	{ allocframe(00000070); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+80) = r23:r22; r2 = memw(r17+4) }
	{ memd(r29+72) = r25:r24 }
	{ memd(r29+96) = r19:r18; r19 = 437F0000 }
	{ memd(r29+88) = r21:r20; r3 = memw(r2+4) }
	{ r5 = memw(r2+20); r4 = memw(r2+8); r1 = r19 }
	{ r3 = memw(r3+16); r6 = memw(r2+16) }
	{ r5 = memw(r5+16); r4 = memw(r4+16) }
	{ r6 = memw(r6+16) }
	{ r22 = memw(r3); r24 = memw(r4) }
	{ r23 = memw(r6); r25 = memw(r5) }
	{ memd(r29+64) = r27:r26; r21 = memw(r2); r0 = sfsub(r24,r22) }
	{ r20 = memw(r2+12); call fn00009610; r18 = sfsub(r25,r23) }
	{ memw(r29+60) = r0; r1:r0 = combine(r19,r18); call fn00009610 }
	{ r1 = 000000D0; r3:r2 = combine(00000002,r16); r4 = add(PC,00013E0F) }
	{ memw(r29+56) = r0; r26 = memw(r21+16) }
	{ r19 = memw(r20+24); r27 = memw(r20+16) }
	{ memw(r29) = r17; call logmsg_function }
	{ r4 = memw(r21); r1 = 000000D1; r3 = add(PC,00013E01) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012C20; r2 = memw(r20) }

l00012BA8:
	{ r4 = memw(r21+4); r1 = 000000D2; r3 = add(PC,0000000D) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012C20; r2 = memw(r20+4) }

l00012BC0:
	{ r4 = memw(r21+8); r1 = 000000D3; r3 = add(PC,00000018) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012C20; r2 = memw(r20+8) }

l00012BD8:
	{ r4 = memw(r21+12); r1 = 000000D4; r3 = add(PC,00000022) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012C20; r2 = memw(r20+12) }

l00012BF0:
	{ r4 = memw(r21+24); r1 = 000000D5; r3 = add(PC,0000002C) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012C20; r2 = memw(r20+24); p0 = cmp.gt(r19,00000000) }

l00012C0C:
	{ memw(r29+48) = r24; memw(r29+44) = r25 }
	{ memw(r29+52) = r16; jump 00012C3C; if (!p0) jump:nt 00012D18 }

l00012C20:
	{ memw(r29) = r4; memw(r29+4) = r2; r2 = r16 }

l00012C28:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00012D34 }

l00012C34:
	{ if (!cmp.gt(r19.new,r21)) jump:nt 00012D18; r19 = r25 }

l00012C3C:
	{ r16 = memb(r27); r19 = memb(r26); r20 = r22; r25:r24 = combine(r19,r23) }

l00012C40:
	{ r16 = memb(r27); r19 = memb(r26); r20 = r22 }
	{ r2 = memd(r29+60); r4 = memd(r29+56); r1 = r18; r7 = convert_w2sf(r16) }
	{ r3 = convert_w2sf(r19); r24 += sfmpy(r4,r7) }
	{ r20 += sfmpy(r2,r3) }
	{ call fn00009610; r0 = sfsub(r20,r24) }
	{ r2 = 9999999A; r26 = add(r26,00000001); r4 = clrbit(r0,0000001E) }
	{ r3 = 3FA99999; r27 = add(r27,00000001); r1:r0 = convert_sf2df(r4) }
	{ if (p0.new) memw(r29+32) = r19; if (!p0.new) r21 = add(r21,00000001); if (!p0.new) jump:nt 00012C34; p0 = dfcmp.gt(r1:r0,r3:r2) }

l00012C9C:
	{ memw(r29+36) = r16; r2 = memd(r29+44); r13:r12 = convert_sf2df(r22); r7:r6 = convert_sf2df(r23) }
	{ r16 = memw(r29+52); r1 = 000000E1; r5:r4 = convert_sf2df(r2) }
	{ memd(r29+24) = r5:r4; r2 = memd(r29+48); r4 = add(PC,00013D84) }
	{ memd(r29) = r13:r12; memd(r29+16) = r7:r6; r3:r2 = combine(00000001,r16); r9:r8 = convert_sf2df(r2) }
	{ memd(r29+8) = r9:r8 }
	{ call logmsg_function }
	{ r1 = 000000E2; r3:r2 = combine(00000001,r16); r9:r8 = convert_sf2df(r20); r7:r6 = convert_sf2df(r24) }
	{ memd(r29+16) = r7:r6; memw(r29+4) = r25; r4 = add(PC,00013D8C) }
	{ memw(r29) = r21; memd(r29+8) = r9:r8 }
	{ call logmsg_function }
	{ r2 = r16; r1 = 000000E3; r3 = add(PC,00013D88) }
	{ jump 00012C28 }

l00012D18:
	{ r1 = 000000E7; r3 = 00000002; r4 = add(PC,00013D82) }
	{ memw(r29) = r17; r2 = memd(r29+52); call logmsg_function }
	{ r0 = 00000000 }

l00012D34:
	{ r19:r18 = memd(r29+96); r17:r16 = memd(r29+104) }
	{ r23:r22 = memd(r29+80); r21:r20 = memd(r29+88) }
	{ r27:r26 = memd(r29+64); r25:r24 = memd(r29+72) }
	{ dealloc_return }

;; close_check_q: 00012D48
close_check_q proc
	{ jump close_check__merged; r1 = 00000001 }

;; logmsg_function: 00012D4C
;;   Called from:
;;     000127BC (in close_execute_f)
;;     00012938 (in close_execute_f)
;;     00012968 (in close_check_f)
;;     000129D8 (in close_check_f)
;;     00012ADC (in close_check__merged)
;;     00012B84 (in close_execute_q_u8)
;;     00012CD4 (in close_execute_q_u8)
;;     00012D00 (in close_execute_q_u8)
;;     00012D28 (in close_execute_q_u8)
;;     00012DBC (in close_execute)
;;     00012E7C (in close_execute)
;;     00012F68 (in check_u8vals)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00012D6C; r5 = memw(r2+16) }

l00012D5C:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,00000021) }
	{ memw(r29+4) = r6; call logv }

l00012D6C:
	{ dealloc_return }

;; errlog_function: 00012D70
;;   Called from:
;;     00012870 (in close_execute_f)
;;     00012988 (in close_check_f)
;;     00012A9C (in close_check__merged)
;;     00012C28 (in close_execute_q_u8)
;;     00012E64 (in close_execute)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00013B89) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; close_execute: 00012D94
;;   Called from:
;;     000129E8 (in close_execute_i32)
;;     00012AF0 (in close_execute_u8)
close_execute proc
	{ allocframe(00000020); memd(r29+496) = r17:r16; r4 = add(PC,00013D38) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r19:r18; r5 = memw(r17+4); r1 = 0000003B; r18 = r2 }
	{ r3:r2 = combine(00000002,r16) }
	{ memd(r29+8) = r21:r20; r19 = memw(r5) }
	{ r20 = memw(r5+4) }
	{ memw(r29) = r17; call logmsg_function }
	{ r4 = memw(r19); r1 = 0000003D; r3 = add(PC,00013BC9) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012E30; r2 = memw(r20) }

l00012DDC:
	{ r4 = memw(r19+4); r1 = 0000003E; r3 = add(PC,00000019) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012E30; r2 = memw(r20+4) }

l00012DF0:
	{ r4 = memw(r19+8); r1 = 0000003F; r3 = add(PC,00000028) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012E30; r2 = memw(r20+8) }

l00012E04:
	{ r4 = memw(r19+12); r1 = 00000040; r3 = add(PC,00000036) }
	{ if (!cmp.eq(r2.new,r4)) jump:t 00012E30; r2 = memw(r20+12) }

l00012E1C:
	{ r4 = memw(r19+24); r1 = 00000041; r3 = add(PC,00000000) }
	{ if (cmp.eq(r2.new,r4)) jump:t 00012E38; r2 = memw(r20+24) }

l00012E30:
	{ memw(r29) = r4; memw(r29+4) = r2; jump 00012E64; r2 = r8 }

l00012E34:
	{ memw(r29) = r4; memw(r29+4) = r2 }

l00012E38:
	{ r2 = memw(r20+16); r1 = memw(r19+16); r0 = r16; r3 = r4 }
	{ callr r18 }
	{ r3:r2 = combine(00000002,r16); r1 = 00000045; if (p0.new) jump:nt 00012E70; p0 = cmp.eq(r0,00000000) }

l00012E54:
	{ r2 = r16; r1 = 00000043; r3 = add(PC,00013C38) }

l00012E64:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00012E84 }

l00012E70:
	{ memw(r29) = r17; r4 = add(PC,00013C74) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00012E84:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }
	{ dealloc_return; r21:r20 = memd(r29+8) }

;; check_u8vals: 00012E8C
check_u8vals proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r3) }
	{ memd(r29+40) = r19:r18; r19:r18 = combine(00000000,00000001); p0 = cmp.gt(r16,00000000) }
	{ memd(r29+32) = r21:r20; r20 = r1 }
	{ memd(r29+16) = r25:r24; memd(r29+24) = r23:r22 }
	{ if (!p0) jump:nt 00012F70 }

l00012EAC:
	{ r2 = r16; r3 = r19; p1 = cmp.gtu(r16,00000001) }
	{ r4 = memb(r3++#1); r5 = add(r2,FFFFFFFF); if (p1) jump:nt 00012ED8 }

l00012EC0:
	{ r2 = r4 }

l00012EC4:
	{ r18 = 00000000; r22 = 00000000; if (!p0) jump:nt 00012F70; r2 = maxu(r18,r2) }

l00012ED0:
	{ jump 00012F14; r21 = convert_uw2sf(r2) }

l00012ED8:
	{ r2 = memb(r3++#1); p1 = cmp.gtu(r2,00000001); loop0(00012EE8,r5) }
	{ if (!p1) jump:nt 00012EFC }

l00012EE8:
	{ r2 = memb(r3++#1); r5 = r2; r18 = maxu(r18,r4) }
	{ nop; r4 = r5 }

l00012EFC:
	{ jump 00012EC4; r18 = maxu(r18,r4) }

l00012F04:
	{ if (!cmp.gtu(r16,r22.new)) jump:nt 00012F70; r22 = add(r22,00000001); r19 = add(r19,00000001); r20 = add(r20,00000001) }

l00012F14:
	{ r24 = memb(r19); r23 = memb(r20); r1 = r21 }

l00012F18:
	{ r24 = memb(r19); r23 = memb(r20) }
	{ r3 = convert_uw2sf(r23); r2 = convert_uw2sf(r24) }
	{ call fn00009610; r0 = sfsub(r3,r2) }
	{ r2 = 9999999A; r4 = clrbit(r0,0000001E) }
	{ r3 = 3FA99999; r1:r0 = convert_sf2df(r4) }
	{ if (p0.new) memw(r29+12) = r24; if (p0.new) r1 = 000000A2; if (!p0.new) jump:nt 00012F04; p0 = dfcmp.gt(r1:r0,r3:r2) }

l00012F58:
	{ r3:r2 = combine(00000000,r17); r4 = add(PC,00013B55) }
	{ memw(r29+4) = r16; memw(r29+8) = r23 }
	{ memw(r29) = r22; r18 = 00000001; call logmsg_function }

l00012F70:
	{ r0 = r18 }
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ dealloc_return; r25:r24 = memd(r29+16) }

;; check_i32vals: 00012F84
check_i32vals proc
	{ allocframe(00000030); memd(r29+480) = r21:r20; r20 = lsr(r3,00000002) }
	{ memd(r29+16) = r23:r22; memd(r29+40) = r17:r16; r17:r16 = combine(r2,r0) }
	{ memd(r29+32) = r19:r18; r18 = r1; p0 = cmp.eq(r20,00000000); r0 = 00000000 }
	{ if (!p0) r2 = add(r17,00000000); if (p0) jump:nt 00013038 }

l00012FA8:
	{ loop0(00012FAC,r20) }
	{ r3 = memw(r2++#4) }
	{ r3 = abs(r3) }
	{ nop; r0 = maxu(r0,r3) }
	{ r21 = -00000001; r0 = -00000001; jump 00013038; if (!p0) jump:nt 00012FDC }
00012FC8                         92 40 12 B0 91 40 11 B0         .@...@..
00012FD0 35 40 15 B0 0C F4 82 21 30 C0 00 16 13 C0 20 8B 5@.....!0..... .
00012FE0 01 40 73 70 9F 00 AE 00 02 40 57 8B 03 C0 56 8B .@sp.....@W...V.
00012FF0 20 42 03 EB 10 F3 FE 5B 24 5F C0 8C 66 66 99 09  B.....[$_..ff..
00013000 42 C3 00 78 00 40 84 84 66 66 FA 03 23 C3 00 78 B..x.@..ff..#..x
00013010 20 42 E0 D2 DC 68 FF 5C E1 71 00 7E 00 D5 9D 42  B...h.\.q.~...B
00013020 EA 44 00 00 84 46 49 6A 3F 28 81 7D 1C 08 2E E8 .D...FIj?(.}....
00013030 8E FE FF 5B 20 C0 00 78                         ...[ ..x        

l00013038:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ dealloc_return }
00013044             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; concat_execute_asm: 00013050
concat_execute_asm proc
	{ jump concat_execute; r2 = add(PC,000005EC) }

;; concat_check: 0001305C
concat_check proc
	{ allocframe(00000018); memd(r29+496) = r17:r16; r4 = add(PC,00013BF7) }
	{ r17 = r0; r16 = r1; r1 = 00000123 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16); r3 = AAAAAAAB; r1 = 00000124 }
	{ r4 = add(r2,FFFFFFFF) }
	{ r5 = r4; r3 = mpyu(r4,r3) }
	{ r3 = lsr(r3,00000001) }
	{ r5 -= mpyi(r3,00000003) }
	{ r2 = memw(r17+28); r5 = memw(r17+24); r3 = add(PC,0000000F) }
	{ memw(r29+4) = r5; memw(r29+8) = r2; r2 = r16 }
	{ memw(r29) = r4; jump 00013138 }
000130B8                         EF 44 00 00 83 4A 49 6A         .D...JIj
000130C0 A1 E4 00 78 84 40 00 78 38 E2 02 21 EF 44 00 00 ...x.@.x8..!.D..
000130D0 03 49 49 6A C1 E4 00 78 A4 40 91 91 2E E3 42 24 .IIj...x.@....B$
000130E0 12 40 02 10 24 C0 91 47 08 C0 02 60 EF 44 00 00 .@..$..G...`.D..
000130F0 03 C1 49 6A 02 40 84 91 0C C0 02 24 84 80 04 B0 ..Ij.@.....$....
00013100 00 C0 00 7F 0E 40 04 16 42 C0 91 91 14 40 00 58 .....@..B....@.X
00013110 01 E5 00 78 82 40 02 B0 24 40 04 B0 18 C2 02 25 ...x.@..$@.....%
00013120 EE 44 00 00 83 4C 49 6A 05 40 82 91 FA E0 72 24 .D...LIj.@....r$
00013130 61 E5 00 78 02 C0 70 70                         a..x..pp        

l00013138:
	{ call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+16); r0 = FFFFFFFF }
00013144             EE 44 00 00 84 40 49 6A A1 65 00 78     .D...@Ij.e.x
00013150 02 C0 70 70 4E 42 00 5A 00 D1 9D A1 00 C0 00 78 ..ppNB.Z.......x
00013160 40 1F 14 3E                                     @..>            

;; concat_execute_ref: 00013164
concat_execute_ref proc
	{ jump concat_execute; r2 = add(PC,000002D0) }

;; concat_execute: 00013170
;;   Called from:
;;     00013050 (in concat_execute_asm)
;;     00013164 (in concat_execute_ref)
concat_execute proc
	{ allocframe(000000A8); memd(r29+472) = r23:r22; r3 = AAAAAAAB }
	{ r23 = r0 }
	{ memd(r29+152) = r19:r18; r4 = memw(r23+16) }
	{ memd(r29+120) = r27:r26; r27 = memw(r23+4); r18 = add(r4,FFFFFFFF) }
	{ memd(r29+160) = r17:r16; r5 = memw(r23+8); r17 = r1; r3 = mpyu(r18,r3) }
	{ memd(r29+128) = r25:r24; r0 = memw(r27+4); r25 = r2 }
	{ r2 = r17; r1 = 000000E5; r4 = add(PC,00013A1C) }
	{ r26 = memw(r5+4); r24 = memw(r5); r22 = lsr(r3,00000001) }
	{ memb(r29+10) = r3.new; r3 = memw(r0+4); r16 = r22; r6 = addasl(r27,r22,00000002) }
	{ r3 = memw(r0) }
	{ memw(r29+36) = r3; r6 = memw(r6+4) }
	{ r7 = memw(r13+r27) }
	{ r5 = memw(r5+8); r7 = memw(r7+16) }
	{ memd(r29+144) = r21:r20; r3 = memw(r6+16) }
	{ memw(r29+28) = r5; r5 = memw(r0+8) }
	{ r19 = memw(r27) }
	{ memw(r29+44) = r5; r20 = memw(r7) }
	{ r21 = memw(r3) }
	{ memw(r29) = r23; call logmsg_function }
	{ r2 = memw(r19+16) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 00013300; r2 = memw(r2) }

l00013214:
	{ memw(r29+24) = r24; r24 = 00000000 }
	{ if (!cmp.gtu(r2.new,r18)) jump:t 0001331C; r2 = 00000003 }

l00013228:
	{ memw(r26+12) = FFFFFF81; r6 = memw(r29+28); r2 = 00000000 }
	{ memw(r26+4) = FFFFFF81; memw(r26+8) = 00000001; r3 = add(r29,00000030); r2 = sfmin(r2,r21) }
	{ memw(r6+8) = 00000001; memw(r26) = 00000001; r21 = add(r3,00000014) }
	{ memw(r6+4) = 00000001; memw(r6) = 00000001; r16 = add(r29,00000054); r1:r0 = combine(00000000,r21) }
	{ memw(r6+12) = FFFFFF81 }
	{ r4 = memw(r26+16) }
	{ memw(r4) = r2; r4 = memd(r29+36) }
	{ r7 = memw(r6+16) }
	{ memw(r26+24) = 00000004; memw(r7) = r20 }
	{ memw(r5) = r4; memw(r6+24) = 00000004 }
	{ r4 = memd(r29+44); r7 = memd(r29+40) }
	{ memw(r5+8) = r4; memw(r5+4) = r7 }
	{ memw(r5+24) = r18 }
	{ memw(r29+48) = r23; memw(r5+12) = r19 }
	{ memw(r29+64) = r19; memw(r29+84) = r23 }
	{ memw(r29+92) = r2; memw(r29+56) = r2 }
	{ memw(r29+96) = r20; memw(r29+60) = r20 }
	{ memw(r3+4) = 00000001; memw(r29+100) = r19 }
	{ memw(r16+4) = FFFFFF80; call fn00009740 }
	{ r0 = add(r16,00000014); r1 = 00000000; call fn00009740 }
	{ r1:r0 = combine(r25,r17); r2 = add(r29,00000030); call nn_os_work_for_vector }
	{ r0 = r17; r1 = add(r29,00000054); callr r25 }
	{ r0 = r21; call fn000096A0 }
	{ r2 = r17; r1 = 00000112; r4 = add(PC,00013974) }
	{ memw(r29) = r23; call logmsg_function }

l000132E8:
	{ r27:r26 = memd(r29+120); r0 = r24 }
	{ r19:r18 = memd(r29+152); r17:r16 = memd(r29+160) }
	{ r23:r22 = memd(r29+136); r21:r20 = memd(r29+144) }
	{ r25:r24 = memd(r29+128) }
	{ dealloc_return }

l00013300:
	{ r1 = 000000E6; r3 = add(PC,000138DD) }
	{ r2 = r17 }

l00013310:
	{ r24 = FFFFFFFF; call errlog_function }
	{ jump 000132E8 }

l0001331C:
	{ r3 = memd(r29+40); r2 = memd(r29+44); r24 = add(r27,00000004); r19:r18 = combine(00000000,00000000) }
	{ memw(r29+16) = r23; memw(r29+8) = r26; r16 = r27; r26 = add(r16,r27) }
	{ memw(r29+20) = r17; memw(r29+12) = r25; r2 = mpyi(r3,r2) }
	{ r3 = memd(r29+36); r23 = 00000000; r16 += add(r7,00000004) }
	{ r2 = mpyi(r2,r3) }
	{ memw(r29+32) = r2 }

l00013354:
	{ r3 = memw(r29+44); r25 = memw(r24) }
	{ if (cmp.eq(r2.new,r3)) jump:t 00013378; r2 = memw(r25+8) }

l00013368:
	{ r1 = 000000E9; r3 = add(PC,00000004) }
	{ memw(r29) = r23; r2 = memd(r29+20); jump 00013310 }

l00013374:
	{ memw(r29) = r23; r2 = memd(r29+20) }

l00013378:
	{ r2 = memw(r25+4) }
	{ if (cmp.eq(r3.new,r2)) jump:t 00013394; r3 = memw(r29+40) }

l00013388:
	{ r1 = 000000EC; jump 00013374; r3 = add(PC,0000003D) }

l00013394:
	{ r2 = memw(r25) }
	{ if (cmp.eq(r3.new,r2)) jump:t 000133B0; r3 = memw(r29+36) }

l000133A4:
	{ r1 = 000000EF; jump 00013374; r3 = add(PC,0000003B) }

l000133B0:
	{ r2 = memw(r16++#4); r23 = add(r23,00000001); r17 = add(r27,00000008); r0 = r21 }
	{ r2 = memw(r2+16) }
	{ r1 = memw(r2); call fn000097B0 }
	{ r2 = memw(r26++#4); r0 = r20; r21 = r0 }
	{ r2 = memw(r2+16) }
	{ r1 = memw(r2); call fn00009600 }
	{ r3 = memw(r29+32); r2 = memw(r25+12); r20 = r0; r27 = r24 }
	{ if (!p0.new) r26 = memw(r29+8); p0 = cmp.gtu(r22,r23); r19 = add(r2,r19); r24 = r17 }
	{ if (!p0) r23 = memw(r29+16); if (!p0) r24 = 00000000; if (p0) jump:nt 00013354; r18 += mpyi(r3,r2) }

l00013414:
	{ r17 = memd(r29+20); r5 = memd(r29+24); r1 = 000000F6 }
	{ r25 = memw(r29+12) }
	{ if (!cmp.gtu(r18,r2.new)) jump:t 00013228; r2 = memw(r5+20) }

l0001342C:
	{ jump 00013310; r3 = add(PC,0000000E) }

;; concat_execute_slice_ref: 00013434
concat_execute_slice_ref proc
	{ allocframe(00000048); r0 = 437F0000 }
	{ r2 = memw(r1+12); r4 = memw(r1+8) }
	{ memd(r29+64) = r17:r16; r16 = memw(r1) }
	{ memd(r29+40) = r23:r22 }
	{ memw(r29+4) = r1; r3 = memw(r16+16) }
	{ r23 = memw(r1+4); r22 = memw(r1+16); r1 = sfsub(r2,r4) }
	{ memd(r29+56) = r19:r18; memd(r29+32) = r25:r24; r17 = add(r3,FFFFFFFF) }
	{ memd(r29+24) = r27:r26; memd(r29+48) = r21:r20 }
	{ memw(r29+20) = r4; r24 = memw(r16+4); call fn00009610 }
	{ memw(r29+16) = r0; r2 = 00000003 }
	{ if (!p0.new) r26 = add(r24,00000000); if (p0) jump:nt 000135CC; p0 = cmp.gtu(r2,r9) }

l00013480:
	{ r2 = memw(r16+8); r3 = AAAAAAAB }
	{ r2 = memw(r2); r16 = 00000000; r3 = mpyu(r17,r3) }
	{ memb(r29+2) = r4.new; r4 = setbit(r3,00000000); r18 = lsr(r3,00000001) }
	{ r27 = memw(r2+16) }
	{ memw(r29+12) = r3 }

l000134A8:
	{ r21 = memw(r26+4); r26 = add(r26,00000004); r2 = r16 }
	{ r3 = r2; r16 = r2; r17 = add(r21,0000000C) }
	{ if (!p0.new) jump:t 000135BC; p0 = cmp.eq(r3,r15) }

l000134C0:
	{ r4 = memd(r29+8); r3 = memd(r29+12); r1 = 437F0000 }
	{ r3 = add(r3,r2); r4 = 00000000; r2 = add(r4,r2) }
	{ r2 = memw(r14+r2<<#2); r3 = memw(r6+r3<<#2) }
	{ r2 = memw(r2+16); r3 = memw(r3+16) }
	{ r3 = memw(r3) }
	{ r2 = memw(r2); r25 = sfmin(r4,r3) }
	{ call fn00009610; r0 = sfsub(r2,r25) }
	{ r3 = memw(r21+4); r2 = memw(r21+8); r17 = add(r21,0000000C); r20 = r0 }
	{ r4 = memw(r21+12); r5 = memw(r21) }
	{ r3 = memw(r21+16); r2 = mpyi(r3,r2) }
	{ r19 = mpyi(r2,r5) }
	{ r7 = combine(r4.l,r19.l) }
	{ r1:r0 = combine(r4,r7) }
	{ l2fetch(r3,r1:r0) }
	{ r2 = memd(r29+20); r1 = r20 }
	{ call fn00009610; r0 = sfsub(r25,r2) }
	{ r3 = memd(r29+16); r2 = r0; r0 = 41700000 }
	{ r20 = convert_uw2sf(r2):chop; r25 = sfmpy(r3,r20) }
	{ call fn000097C0 }
	{ p0 = cmp.eq(r19,00000000); r2 = sfmpy(r25,r0) }
	{ if (!p0) r2 = add(r27,00000000); if (p0) jump:nt 000135BC; r5 = convert_uw2sf(r2):chop }

l00013554:
	{ r3 = memw(r21+16); r7 = memw(r21+12); r4 = 00007FFF }
	{ if (!p0.new) r4 = sxth(r5); p0 = cmp.gt(r5,r4); loop0(00013568,r19) }
	{ r7 = 00000000; if (p0.new) jump:nt 000135B4; p0 = cmp.eq(r7,00000000) }

l00013570:
	{ r5 = r3; r6 = 00000000 }

l00013574:
	{ r8 = memb(r5++#1); r7 = 000000FF }
	{ r8 = add(r8,r20) }
	{ r8 = add(00004000,mpyi(r8,r4)) }
	{ r8 = asr(r8,0000000F) }
	{ if (!p0.new) r7 = add(r8,00000000); p0 = cmp.gt(r8,000000FF); if (p0.new) jump:nt 000135A0 }

l00013598:
	{ if (!p0.new) r7 = 00000000; p0 = cmp.gt(r8,FFFFFFFF) }

l000135A0:
	{ r6 = add(r6,00000001); r8 = add(r2,r6) }
	{ memb(r8) = r7 }
	{ if (cmp.gtu(r7.new,r6)) jump:t 00013574; r7 = memw(r17) }

l000135B4:
	{ r2 = add(r2,r22); r3 = add(r3,r7); nop }

l000135B8:
	{ r2 = add(r2,r22); r3 = add(r3,r7) }

l000135BC:
	{ r2 = memw(r17); p0 = cmp.gtu(r18,r16) }
	{ r27 = add(r27,r2); if (p0) jump:nt 000134A8 }

l000135CC:
	{ r17:r16 = memd(r29+64); r2 = memd(r29+4); r1 = 00000001 }
	{ r21:r20 = memd(r29+48); r19:r18 = memd(r29+56); r0 = add(r2,00000014) }
	{ r25:r24 = memd(r29+32); r23:r22 = memd(r29+40) }
	{ deallocframe; r27:r26 = memd(r29+24); jump 00009730 }

;; logmsg_function: 000135F0
;;   Called from:
;;     00013070 (in concat_check)
;;     000131FC (in concat_execute)
;;     000132E0 (in concat_execute)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00013614; r5 = memw(r2+16) }

l00013600:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000002C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00013614:
	{ dealloc_return }

;; errlog_function: 00013618
;;   Called from:
;;     00013138 (in concat_check)
;;     00013310 (in concat_execute)
;;     00013608 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00013590) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; concat_execute_slice_asm: 0001363C
concat_execute_slice_asm proc
	{ allocframe(00000048); r0 = 437F0000 }
	{ r2 = memw(r1+12); r4 = memw(r1+8) }
	{ memd(r29+64) = r17:r16; r17 = memw(r1) }
	{ r3 = memw(r1+16) }
	{ memw(r29+16) = r3; memd(r29+32) = r25:r24 }
	{ memw(r29+8) = r1; r24 = memw(r1+4); r1 = sfsub(r2,r4) }
	{ r3 = memw(r17+16) }
	{ memd(r29+48) = r21:r20; memd(r29+56) = r19:r18; r19 = add(r3,FFFFFFFF) }
	{ memd(r29+24) = r27:r26; memd(r29+40) = r23:r22 }
	{ memw(r29+20) = r4; r16 = memw(r17+4); call fn00009610 }
	{ memw(r29+12) = r0; r2 = 00000003 }
	{ if (!p0.new) r25 = add(r16,00000004); if (!p0.new) r27 = 00000000; if (p0) jump:nt 00013770; p0 = cmp.gtu(r2,r11) }

l00013690:
	{ r3 = memw(r17+8); r2 = AAAAAAAB }
	{ r7 = memw(r3); r2 = mpyu(r19,r2) }
	{ r20 = memw(r7+16); r19 = lsr(r2,00000001) }
	{ r16 = asl(r19,00000003); r26 = asl(r19,00000002) }

l000136B0:
	{ r17 = memw(r25); r27 = add(r27,00000001); r2 = and(r27,00000001) }
	{ if (p0.new) r2 = memw(r25+r26); if (p0.new) r3 = memw(r25+r16); p0 = cmp.eq(r2,r24); if (p0.new) jump:nt 000136D4 }

l000136CC:
	{ r17 = add(r17,0000000C); jump 0001375C }

l000136D4:
	{ r21 = memw(r17+16); r4 = 00000000 }
	{ r3 = memw(r3+16); r2 = memw(r2+16); r1 = 437F0000 }
	{ r3 = memw(r3); r2 = memw(r2) }
	{ r23 = sfmin(r4,r2) }
	{ call fn00009610; r0 = sfsub(r3,r23) }
	{ r3 = memw(r17+4); r2 = memw(r17+8); r22 = r0 }
	{ r4 = memw(r17+12); r5 = memw(r17) }
	{ r2 = mpyi(r3,r2) }
	{ r18 = mpyi(r2,r5) }
	{ r7 = combine(r4.l,r18.l) }
	{ r1:r0 = combine(r4,r7) }
	{ l2fetch(r21,r1:r0) }
	{ r2 = memd(r29+20); r1 = r22 }
	{ call fn00009610; r0 = sfsub(r23,r2) }
	{ r2 = memd(r29+12); r3 = 47000000; r6 = 00007FFF }
	{ r5 = memw(r29+16); r1:r0 = combine(r21,r20); r7 = convert_uw2sf(r0):chop }
	{ r2 = sfmpy(r2,r22) }
	{ r2 = memw(r17+12); r3 = sxth(r7); r17 = add(r17,0000000C); r4 = sfmpy(r2,r3) }
	{ memw(r29) = r18; r4 = convert_uw2sf(r4):chop }
	{ r4 = min(r6,r4) }
	{ r4 = sxth(r4); call memconvert_hvx }

l0001375C:
	{ r2 = memw(r17); p0 = cmp.gtu(r19,r27); r25 = add(r25,00000004) }
	{ r20 = add(r20,r2); if (p0) jump:nt 000136B0 }

l00013770:
	{ r17:r16 = memd(r29+64); r2 = memd(r29+8); r1 = 00000001 }
	{ r21:r20 = memd(r29+48); r19:r18 = memd(r29+56); r0 = add(r2,00000014) }
	{ r25:r24 = memd(r29+32); r23:r22 = memd(r29+40) }
	{ deallocframe; r27:r26 = memd(r29+24); jump 00009730 }
00013794             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; hexagon_nn_const_ctor: 000137A0
;;   Called from:
;;     0000B764 (in do_append_const_node)
hexagon_nn_const_ctor proc
	{ allocframe(+00000030); memw(r29-44) = r5 }
	{ memd(r29+40) = r17:r16; r0 = add(r29,00000000); r17:r16 = combine(r1,r0) }
	{ memw(r29) = r2; memw(r29+8) = r4 }
	{ memd(r29+32) = r19:r18; r4 = memd(r29+56) }
	{ memw(r29+4) = r3; r3 = memd(r29+60) }
	{ memw(r29+24) = r3; memw(r29+16) = r4 }
	{ memw(r29+20) = r3; call tensor_dup }
	{ if (!cmp.eq(r18.new,00000000)) jump:t 000137EC; r18 = r0 }

l000137D4:
	{ r2 = r16; r1 = 00000054; r3 = add(PC,00000028) }
	{ r17 = 00000000; call errlog_function }
	{ jump 00013828 }

l000137EC:
	{ r17 = 00000000; r2 = 00000000; r1:r0 = combine(00000003,r17); call alloc_node }
	{ r2 = add(r18,0000001C); if (!p0.new) jump:nt 00013818; p0 = cmp.eq(r0,00000000) }

l00013800:
	{ r2 = r16; r1 = 00000058; r3 = add(PC,0001350B) }
	{ call errlog_function }
	{ jump 00013828 }

l00013818:
	{ memw(r0+4) = 00000000; memw(r0+20) = 00000001; r17 = r0 }
	{ memw(r0+12) = 00000000; memw(r0+16) = 00000000 }
	{ memw(r0+8) = r2 }

l00013828:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40); r0 = r17 }
	{ dealloc_return }

;; errlog_function: 00013834
;;   Called from:
;;     000137E0 (in hexagon_nn_const_ctor)
;;     00013810 (in hexagon_nn_const_ctor)
;;     00013898 (in const_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000134A9) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; const_execute: 00013858
const_execute proc
	{ jumpr r31; r0 = 00000000 }
0001385C                                     00 C0 00 7F             ....

;; const_check: 00013860
const_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000134FE) }
	{ r17 = r0; r16 = r1; r1 = 00000036 }
	{ memw(r29) = r17; r3:r2 = combine(00000002,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000138A4; r2 = memw(r17+4) }

l0001388C:
	{ r3 = 00000000; r1 = 00000038; r4 = add(PC,0000002D) }

l00013894:
	{ memw(r29) = r4; r2 = r16 }
	{ call errlog_function }
	{ r0 = FFFFFFFF }
	{ dealloc_return; r17:r16 = memd(r29+8) }

l000138A4:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 000138BC; r2 = memw(r17+8) }

l000138B0:
	{ r3 = 00000000; r1 = 0000003B; r4 = add(PC,0000001E) }
	{ jump 00013894 }

l000138BC:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 000138D8; r2 = memw(r2); r1 = 00000040 }

l000138CC:
	{ r3 = 00000000; r1 = 0000003E; r4 = add(PC,0000001D) }
	{ jump 00013894 }

l000138D8:
	{ memw(r29) = r17; r3:r2 = combine(00000002,r16); r4 = add(PC,000134E9) }
	{ call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
000138F4             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......

;; const_ctor: 00013900
const_ctor proc
	{ r1 = 0000006D; r3:r2 = combine(00000000,r0); r4 = add(PC,0001342E) }
	{ allocframe(+00000000); call logmsg_function }
	{ dealloc_return; r0 = 00000000 }
0001391C                                     00 C0 00 7F             ....

;; const_dtor: 00013920
const_dtor proc
	{ allocframe(+00000010); r3:r2 = combine(00000002,r1); r4 = add(PC,000133FB) }
	{ memd(r29+8) = r17:r16; r16 = r0; r1 = 00000073 }
	{ memw(r29) = r16; call logmsg_function }
	{ r2 = memw(r16+8) }
	{ r0 = memw(r2); call tensor_free }
	{ r0 = r16; call fn00009510 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = 00000000 }

;; logmsg_function: 0001395C
;;   Called from:
;;     00013874 (in const_check)
;;     000138E8 (in const_check)
;;     00013910 (in const_ctor)
;;     00013938 (in const_dtor)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001397C; r5 = memw(r2+16) }

l0001396C:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,00000035) }
	{ memw(r29+4) = r6; call logv }

l0001397C:
	{ dealloc_return }

;; conv2d_execute_hvx: 00013980
conv2d_execute_hvx proc
	{ allocframe(+000000B8); memd(r29-56) = r27:r26; r27 = r0 }
	{ memd(r29+160) = r21:r20; r2 = memw(r27+4) }
	{ memd(r29+152) = r23:r22; r3 = memw(r27+8) }
	{ r22 = memw(r2+4); r21 = memb(r27+32) }
	{ r5 = memw(r2+24); r20 = memw(r2) }
	{ memd(r29+168) = r19:r18; memd(r29+176) = r17:r16; p0 = cmp.eq(r21,00000000) }
	{ memd(r29+144) = r25:r24; r4 = memw(r3+8) }
	{ r3 = memw(r3+4); r16 = memw(r3) }
	{ r23 = memw(r2+8); r19 = memw(r2+12) }
	{ r18 = memw(r2+16); r25 = memw(r2+20) }
	{ memw(r29+100) = r1; r2 = memw(r22+12) }
	{ r0 = memw(r20+8) }
	{ memw(r29+80) = r3; r7 = memw(r20+12) }
	{ memw(r29+116) = r2; r2 = memw(r22+8) }
	{ r3 = memw(r22+4); r17 = memw(r20+4) }
	{ r24 = memw(r5+4); r1 = memw(r5+8) }
	{ memw(r29+108) = r7; r6 = memw(r20) }
	{ r7 = memw(r22) }
	{ memw(r29+88) = r5; memw(r29+92) = r2; r2 = p0 }
	{ memw(r29+76) = r4; memw(r29+128) = r0 }
	{ memw(r29+124) = r7; memw(r29+104) = r6 }
	{ memw(r29+84) = r2; if (p0) jump:nt 00013A3C }

l00013A08:
	{ if (p0.new) r2 = memw(r29-128); if (p0.new) jump:nt 00013A34; p0 = cmp.eq(r13,00000002) }

l00013A10:
	{ memw(r29+120) = r3; memw(r29+112) = r1; p0 = cmp.eq(r21,00000001); r0 = 00000000 }
	{ if (p0) r1 = memw(r29+112); if (p0) r2 = memw(r29-128); if (!p0) jump:nt 00013A44 }

l00013A28:
	{ r3 = memd(r29+120); r0 = r1 }
	{ jump 00013A3C; r0 += add(r2,FFFFFFFF) }

l00013A34:
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r1) }

l00013A3C:
	{ memw(r29+120) = r3; memw(r29+112) = r1; call fn00009760 }

l00013A44:
	{ if (p0.new) r2 = memw(r29+124); if (p0.new) r1 = add(r24,00000000); if (p0.new) jump:nt 00013A80; p0 = cmp.eq(r13,00000002) }

l00013A50:
	{ memw(r29+96) = r0; if (!p0.new) r26 = add(r24,00000000); if (p0.new) jump:nt 00013A74; p0 = cmp.eq(r13,00000001) }

l00013A5C:
	{ r1 = memd(r29+84); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r26,r17); if (!p0.new) jump:nt 00013A94; p0 = r1 }

l00013A6C:
	{ call fn00009760 }
	{ jump 00013A94 }

l00013A74:
	{ r1:r0 = combine(r24,r24) }
	{ jump 00013A8C; r0 += add(r17,FFFFFFFF) }

l00013A80:
	{ memw(r29+96) = r0; r2 = sub(r17,r2) }
	{ r0 = add(r2,r24) }

l00013A8C:
	{ r26 = r24; call fn00009760 }

l00013A94:
	{ r3 = memw(r23+16); r2 = memw(r19+16); r19 = 437F0000 }
	{ r4 = memw(r18+16); r6 = memw(r22+16); r24 = r0 }
	{ r21 = memw(r3); r2 = memw(r2) }
	{ memw(r29+68) = r6; r5 = memw(r25+16) }
	{ r6 = memw(r16+16); r7 = memw(r20+16); r22 = sfsub(r2,r21) }
	{ memw(r29+84) = r7; memw(r29+48) = r26; r1:r0 = combine(r19,r22) }
	{ memw(r29+72) = r6; r18 = memw(r5) }
	{ r25 = memw(r4); call fn00009610 }
	{ r23 = r0; r20 = sfsub(r18,r25) }
	{ r1:r0 = combine(r19,r20); call fn00009610 }
	{ r2 = CF000000; r4 = sfmpy(r23,r0) }
	{ r0 = r22; r3 = 4F000000; r2 = sfmpy(r4,r2) }
	{ memb(r29+15) = r3.new; r3 = sfmpy(r4,r3) }
	{ memw(r29+56) = r2 }
	{ r18 = 00000000; r1:r0 = combine(r0,r19); call fn00009610 }
	{ r2 = sfsub(r18,r21) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = r20; r2 = r0 }
	{ memb(r29+13) = r2.new; call fmaxf.1.0; r2 = convert_uw2sf(r2):chop }
	{ r2 = 00000000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r18,r25) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r5 = memw(r27+28); r22 = memw(r29+100); r4 = add(PC,000132A0) }
	{ memw(r29+4) = r5; r3:r2 = combine(00000002,r22); r1 = 00000207; r7 = convert_uw2sf(r0):chop }
	{ memw(r29) = r27; memw(r29+64) = r7 }
	{ call logmsg_function }
	{ r21 = memd(r29+104); r19 = memd(r29+108); r4 = add(PC,00013292) }
	{ memw(r29+12) = r19; r5 = memw(r29+128); r3:r2 = combine(00000002,r22) }
	{ memw(r29+4) = r17; memw(r29+8) = r5; r1 = 00000208 }
	{ memw(r29) = r21; call logmsg_function }
	{ r5 = memw(r29+120); r25 = memw(r29+92); r4 = add(PC,00013283) }
	{ memw(r29+12) = r25; r3:r2 = combine(00000002,r22) }
	{ memw(r29+8) = r5; r20 = memd(r29+124); r1 = 00000209 }
	{ memw(r29+4) = r20 }
	{ r18 = memw(r29+116) }
	{ memw(r29) = r18; call logmsg_function }
	{ r5 = memw(r29+112); r1 = 0000020A; r4 = add(PC,0001326B) }
	{ memw(r29) = r26; memw(r29+4) = r5; r3:r2 = combine(00000002,r22) }
	{ call logmsg_function }
	{ r1 = 0000020B; r3:r2 = combine(00000002,r22); r4 = add(PC,0001325F) }
	{ memb(r29) = r5.new; r5 = memb(r27+32); call logmsg_function }
	{ memw(r29) = r21; memw(r29+12) = r18; r4 = add(PC,00000015) }
	{ memw(r29+4) = r24; r23 = memw(r29+96); r3:r2 = combine(00000002,r22) }
	{ memw(r29+8) = r23; r1 = 0000020C }
	{ call logmsg_function }
	{ r9 = r17; r1 = 0000020D; r3 = add(PC,00013250) }
	{ if (!cmp.eq(r12.new,r25)) jump:t 00013CEC; r12 = r19; r2 = mpyi(r21,r18) }

l00013C58:
	{ r2 = mpyi(r2,r23) }
	{ r2 = mpyi(r2,r24) }
	{ if (!cmp.gtu(r4.new,r5)) jump:t 00013C84; r4 = asl(r2,00000002) }

l00013C6C:
	{ memb(r29+2) = r2.new; r2 = memw(r27+28); r3 = add(PC,00000021) }
	{ memw(r29) = r5; memw(r29+4) = r4; jump 00013CF0; r2 = r14 }

l00013C84:
	{ r2 = memd(r29+88); r19 = r23; r1 = 00000211 }
	{ r23 = r24; r3 = add(PC,0001322B) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 00013CEC; r2 = memw(r2) }

l00013CA4:
	{ r2 = memw(r29+88); r1 = 00000212; r3 = add(PC,00000028) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 00013CEC; r2 = memw(r2+12) }

l00013CBC:
	{ r2 = memd(r29+80); r5 = 00000004; r3 = add(PC,00000021) }
	{ r1 = 00000213 }
	{ if (cmp.gtu(r5,r2.new)) jump:t 00013CEC; r2 = memw(r2+20) }

l00013CD4:
	{ r2 = memw(r29+76); r1 = 00000214; r3 = add(PC,00000017) }
	{ if (cmp.gtu(r2.new,00000003)) jump:t 00013CFC; r2 = memw(r2+20); r6 = add(00000007,mpyi(r23,r19)) }

l00013CEC:
	{ r2 = r22 }

l00013CF0:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00014018 }

l00013CFC:
	{ memw(r7+12) = r18; r2 = memd(r29+120); r27 = and(r6,FFFFFFF8); r1 = 00000020 }
	{ memw(r7) = r21; memw(r7+8) = r19; r15 = 00000000; r3 = mpyi(r2,r20) }
	{ memw(r7+4) = r23; r17 = r22; r3 = mpyi(r3,r12); r5 = add(0000000F,mpyi(r3,r12)) }
	{ memw(r7+24) = r4; r7 = memd(r29+80); r5 = and(r5,FFFFFFF0); r6 = add(r18,0000001F) }
	{ r2 = memw(r29+56); r8 = memw(r29+76); r24 = and(r6,FFFFFFE0); r5 = max(r5,r1) }
	{ r13 = memw(r29+60); r6 = asl(r24,00000002) }
	{ memw(r7+4) = 00000001; r4 = memw(r7+16) }
	{ memw(r7+12) = 00000001; memw(r7+8) = 00000001 }
	{ memw(r4) = r2; memw(r7) = 00000001; r0 = r5; r4 = asl(r27,00000002) }
	{ memw(r8+4) = FFFFFF81; memw(r7+24) = 00000004; r2 = mpyi(r0,r24); r28 = mpyi(r27,r0) }
	{ memw(r8+8) = 00000001; r1 = memw(r8+16); r7 = or(r2,0000007F); r22 = r28 }
	{ memw(r8) = 00000001; memw(r8+12) = FFFFFF81 }
	{ memw(r8+24) = 00000004; memw(r1) = r13; r13 = 000000FF }
	{ memw(r29+32) = r5; r26 = memw(r17+4); r5 = r7 }
	{ memw(r29+60) = r3; r14 = memw(r29+64); r8 = r26 }
	{ memw(r29+56) = r2; p0 = cmp.gt(r14,000000FF); r8 += add(r22,0000007F); r2 = mpyi(r27,r24) }
	{ memw(r29+44) = r9; p1 = cmp.gt(r14,FFFFFFFF); r8 = and(r8,FFFFFF80); r3 = asl(r2,00000002) }
	{ memw(r29+64) = r3; memw(r29+92) = r8; r8 = add(r8,0000017F); r2 = add(00000080,asl(r2,00000004)) }
	{ r8 = and(r8,FFFFFF80); if (p0) r18 = add(r13,00000000) }
	{ memw(r29+88) = r8; if (!p0) r18 = zxtb(r14); r8 += add(r4,0000007F) }
	{ r0 = and(r8,FFFFFF80); if (!p1) r18 = add(r15,00000000) }
	{ memw(r29+80) = r0; r20 = addasl(r0,r24,00000002); r5 += add(r6,r0) }
	{ r25 = and(r5,FFFFFF80) }
	{ memw(r29+76) = r25; r3 = add(r25,r7) }
	{ r21 = and(r3,FFFFFF80) }
	{ r1:r0 = combine(00000000,r21); call fn000095F0 }
	{ memw(r29+4) = r26; r3:r2 = combine(00000002,r17); r4 = add(PC,00013250) }
	{ memw(r29) = r22; r16 = r26; r1 = 0000023B }
	{ call logmsg_function }
	{ r1 = 0000023C; r3:r2 = combine(00000002,r17); r4 = add(PC,0001324B) }
	{ memw(r29+4) = r20; r5 = memd(r29+56) }
	{ memw(r29) = r5; call logmsg_function }
	{ r1 = 0000023D; r3:r2 = combine(00000002,r17); r4 = add(PC,00013243) }
	{ memw(r29+4) = r21; r5 = memd(r29+64); r22 = r21 }
	{ memw(r29) = r5; r17 = memw(r29+128) }
	{ call logmsg_function }
	{ r2 = memd(r29+116); r21 = memd(r29+32); r3 = r20 }
	{ r0 = memd(r29+68); r1 = memd(r29+60); r5:r4 = combine(r24,r21) }
	{ memw(r29) = r18; call pad2d }
	{ r24 = r21; r26 = r24; r1:r0 = combine(r21,r20); r3:r2 = combine(r25,r24) }
	{ call transpack }
	{ r7 = memd(r29+120); r2 = memd(r29+104); r12 = r23; r9 = r19 }
	{ r8 = memw(r29+112); r3 = memw(r29+108); r4 = add(r12,FFFFFFFF); r6 = sub(r7,r17) }
	{ r20 = memd(r29+72); r19 = memd(r29+84); if (!p0.new) jump:nt 00013FE8; p0 = cmp.gt(r2,00000000) }

l00013EC8:
	{ r5 = memw(r29+124); r13 = memw(r29+44); r2 = add(r9,FFFFFFFF); r15 = mpyi(r12,r9) }
	{ r1 = memd(r29+52); r0 = memd(r29+48); r14 = sub(00000000,r18); r5 = sub(r5,r13) }
	{ p0 = cmp.gt(r1,FFFFFFFF); p1 = cmp.gt(r1,000000FF); r6 = 000000FF; r2 = add(r6,mpyi(r2,r8)) }
	{ memw(r29+60) = r14; memw(r29+64) = r15; r5 = mpyi(r13,r17); r4 = add(r5,mpyi(r4,r0)) }
	{ r17 = memd(r29+104); r1 = memd(r29+116); if (!p1) r6 = zxtb(r1); r2 += lsr(r2,0000001F) }
	{ memb(r29+13) = r5.new; r4 += lsr(r4,0000001F); r5 = mpyi(r5,r3) }
	{ r5 = 00000000; r2 = asr(r2,00000001) }
	{ memw(r29+40) = r2; memw(r29+56) = r0; if (!p0) r6 = add(r5,00000000) }
	{ memw(r29+68) = r6; r1 = sub(00000000,r6); r6 = asr(r4,00000001) }
	{ memw(r29+36) = r6; memw(r29+48) = r1 }

l00013F40:
	{ memb(r29+6) = r2.new; r2 = memw(r29+36); r0 = r19; r5 = r16 }
	{ r1 = memd(r29+44); r6 = memd(r29+40); r18 = r12 }
	{ memw(r29+16) = r9; memw(r29+20) = r6; r21 = r8 }
	{ r4 = memd(r29+68); r23 = r7 }
	{ memw(r29+12) = r12; r2 = memw(r29+128) }
	{ memw(r29+8) = r8 }
	{ memw(r29+4) = r7; r6 = memd(r29+124) }
	{ memw(r29) = r6; call im2col_co }
	{ r1 = memd(r29+48); r2 = memd(r29+92); r5:r4 = combine(r27,r22) }
	{ memw(r29+28) = r2; r7 = memd(r29+80); r0 = r16 }
	{ memw(r29+24) = r7; r2 = memd(r29+88) }
	{ memw(r29+16) = r24; memw(r29+20) = r2 }
	{ r2 = memd(r29+76); r3 = memd(r29+60) }
	{ memw(r29+4) = r24; memw(r29+8) = r27 }
	{ memw(r29+12) = FFFFFFA0; memw(r29) = r26; call gemm_asm }
	{ r4 = memd(r29+64); r5 = memd(r29+116); r1:r0 = combine(r27,r22) }
	{ r3:r2 = combine(r20,r26); call unpad2d }
	{ r6 = memd(r29+52); r2 = memd(r29+56); r9:r8 = combine(r25,r21); r12 = r18 }
	{ r3 = memd(r29+108); r7 = r23; r19 = add(r19,r6) }
	{ if (!cmp.eq(r17.new,00000000)) jump:t 00013F40; r17 = add(r17,FFFFFFFF); r20 = addasl(r20,r2,00000002) }

l00013FE8:
	{ memb(r29+3) = r2.new; r2 = memw(r29+116); r4 = add(PC,000130C2) }

l00013FEC:
	{ memb(r29+3) = r2.new; r2 = memw(r29+116); r4 = add(PC,00000002) }

l00013FFC:
	{ memw(r29+8) = r9; r2 = memw(r29+100); r1 = 00000259 }
	{ memw(r29) = r5; memw(r29+4) = r12 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00014018:
	{ r19:r18 = memd(r29+168); r17:r16 = memd(r29+176) }
	{ r23:r22 = memd(r29+152); r21:r20 = memd(r29+160) }
	{ r27:r26 = memd(r29+136); r25:r24 = memd(r29+144) }
	{ dealloc_return }

;; conv2d_check_ref: 0001402C
conv2d_check_ref proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r4 = add(PC,00012F07) }
	{ r17 = r0; r16 = r1; r1 = 00000271 }
	{ memw(r29) = r17; memd(r29+32) = r19:r18; r3:r2 = combine(00000002,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,00000007)) jump:t 0001406C; r2 = memw(r17+16) }

l00014058:
	{ r2 = memw(r17+28); r1 = 00000272; r3 = add(PC,00000037) }
	{ memw(r29) = r2; jump 00014080 }

l0001406C:
	{ if (cmp.eq(r2.new,00000003)) jump:t 00014090; r2 = memw(r17+20); r1 = 00000273 }

l0001407C:
	{ r3 = add(PC,0000002F) }

l00014080:
	{ r2 = r16; call errlog_function }

l00014084:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 000141A8 }

l00014090:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 000140A8; r3 = memw(r17+4); r1 = 00000274 }

l000140A0:
	{ jump 00014084; r3 = add(PC,00000022) }

l000140A8:
	{ r2 = memw(r17+8) }
	{ if (!p0.new) r4 = 00000000; if (p0.new) r1 = 00000275; if (p0.new) jump:nt 000140D8; p0 = cmp.eq(r2,00000000) }

l000140B8:
	{ loop0(000140BC,00000007) }
	{ if (!cmp.eq(r5.new,00000000)) jump:t 000140E4; r5 = memw(r3) }

l000140C8:
	{ memw(r29) = r4; r1 = 00000278; r3 = add(PC,00000013) }
	{ jump 00014080 }

l000140D8:
	{ jump 00014080; r3 = add(PC,00012EB2) }

l000140E4:
	{ r4 = r4; r3 = add(r3,00000004); nop }
	{ jump 000140FC; r0 = 00000000 }

l000140F0:
	{ if (cmp.gtu(r4.new,00000002)) jump:nt 00014118; r4 = add(r4,00000001); r2 = add(r2,00000004) }

l000140F4:
	{ if (cmp.gtu(r4.new,00000002)) jump:nt 0001411C; r4 = add(r4,00000001) }

l000140FC:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 000140F0; r3 = memw(r2) }

l00014100:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 000140F4 }

l00014108:
	{ memw(r29) = r4; r1 = 0000027D; r3 = add(PC,00000021) }
	{ jump 00014080 }

l00014118:
	{ r1 = 00000282; r3:r2 = combine(00000003,r16); r4 = add(PC,00012E9C) }

l0001411C:
	{ r1 = 00000282; r3:r2 = combine(00000003,r16); r4 = add(PC,0000001C) }
	{ memw(r29) = r17; call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001418C; r2 = memw(r17+16); r19:r18 = combine(00000000,00000000) }

l0001413C:
	{ r2 = memw(r17+4); r1 = 0000026B; r4 = add(PC,00012EDF) }

l00014140:
	{ r2 = memw(r17+4); r1 = 0000026B; r4 = add(PC,0000001F) }

l0001414C:
	{ r2 = memw(r13+r18) }
	{ r6 = memw(r2); r5 = memw(r2+16) }
	{ r8 = memw(r2+8); r7 = memw(r2+4) }
	{ r2 = memw(r2+24); r9 = memw(r2+12) }
	{ memw(r29+8) = r7; memw(r29+24) = r5 }
	{ memw(r29+4) = r6; memw(r29+20) = r2; r3:r2 = combine(00000003,r16) }
	{ memw(r29+12) = r8; memw(r29+16) = r9 }
	{ memw(r29) = r19; call logmsg_function }
	{ r2 = memw(r17+16); r18 = add(r18,00000004) }
	{ if (cmp.gtu(r2,r19.new)) jump:t 0001413C; r19 = add(r19,00000001) }

l0001418C:
	{ r1 = 00000286; r3:r2 = combine(00000002,r16); r4 = add(PC,00012E77) }

l00014190:
	{ r1 = 00000286; r3:r2 = combine(00000002,r16); r4 = add(PC,00000037) }

l0001419C:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l000141A8:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ dealloc_return }

;; conv2d_ctor: 000141B0
conv2d_ctor proc
	{ allocframe(00000028); memd(r29+488) = r19:r18; r6 = add(PC,00012D6C) }
	{ r19:r18 = combine(r0,r4) }
	{ memd(r29+16) = r21:r20; memd(r29+32) = r17:r16; r21:r20 = combine(r2,r5); r17:r16 = combine(r3,r1) }
	{ r4 = r6; r3:r2 = combine(00000002,r19); r1 = 00000295 }
	{ memd(r29+8) = r23:r22; r22 = memd(r29+48) }
	{ r23 = memw(r29+52) }
	{ memw(r29) = r16; call logmsg_function }
	{ memw(r29+4) = r23; r1:r0 = combine(r16,r19); r3:r2 = combine(r17,r21); r5:r4 = combine(r20,r18) }
	{ memw(r29) = r22; call node_alloc_common }
	{ r17:r16 = memd(r29+32) }
	{ r21:r20 = memd(r29+16); r19:r18 = memd(r29+24) }
	{ dealloc_return; r23:r22 = memd(r29+8) }

;; conv2d_execute_ref: 0001420C
conv2d_execute_ref proc
	{ allocframe(+00000088) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+120) = r19:r18; memd(r29+96) = r25:r24 }
	{ r18 = memw(r2); r25 = memb(r0+32) }
	{ memd(r29+104) = r23:r22; r23 = memw(r2+4); p0 = cmp.eq(r25,00000000) }
	{ r7 = memw(r2+8); r5 = memw(r2+24) }
	{ memd(r29+88) = r27:r26; memd(r29+112) = r21:r20 }
	{ r4 = memw(r2+12) }
	{ memw(r29+60) = r7; r7 = memw(r3+8) }
	{ r3 = memw(r3+4); r20 = memw(r3) }
	{ memd(r29+128) = r17:r16; r27 = memw(r18+8) }
	{ memw(r29+32) = r1; memw(r29+72) = r0; r0 = r27 }
	{ memw(r29+64) = r4; r4 = memw(r2+20) }
	{ memw(r29+36) = r3; r24 = memw(r2+16) }
	{ r17 = memw(r18+4); r2 = memw(r23+8) }
	{ r16 = memw(r23+12); r19 = memw(r18+12) }
	{ r22 = memw(r23+4); r21 = memw(r23) }
	{ r3 = memw(r5+4); r1 = memw(r5+8) }
	{ memw(r29+24) = r7; r7 = memw(r18) }
	{ memw(r29+40) = r5; memw(r29+80) = r2; r2 = p0 }
	{ memw(r29+28) = r7; memw(r29+56) = r4 }
	{ memw(r29+48) = r2; if (!p0) r2 = sub(r27,r22); if (p0) jump:nt 000142B0 }

l0001428C:
	{ if (p0.new) r0 = add(r2,r1); p0 = cmp.eq(r25,00000002); if (p0.new) jump:nt 000142B0 }

l00014298:
	{ memw(r29+68) = r1; r0 = 00000000; p0 = cmp.eq(r25,00000001); if (!p0.new) jump:nt 000142C0 }

l000142A4:
	{ r1 = memw(r29+68) }
	{ r0 = r1 }
	{ r0 += add(r27,FFFFFFFF) }

l000142B0:
	{ memw(r29+68) = r1; r26 = r3; call fn00009760 }
	{ r3 = r26 }

l000142C0:
	{ if (p0.new) memw(r29+76) = r0; p0 = cmp.eq(r25,00000002); r2 = sub(r17,r21); if (p0.new) jump:nt 000142FC }

l000142D0:
	{ if (!p0) r1:r0 = combine(r3,r3); if (!p0.new) r25 = add(r3,00000000); p0 = cmp.eq(r25,00000001); if (p0.new) jump:nt 000142F4 }

l000142E0:
	{ r1 = memd(r29+48); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r25,r17); if (!p0.new) jump:nt 0001430C; p0 = r1 }

l000142F0:
	{ jump 00014308 }

l000142F4:
	{ jump 00014304; r0 += add(r17,FFFFFFFF) }

l000142FC:
	{ memw(r29+76) = r0; r1 = r3; r0 = add(r2,r3) }

l00014304:
	{ r25 = r3 }

l00014308:
	{ call fn00009760 }

l0001430C:
	{ r3 = memd(r29+60); r2 = memd(r29+64); r26 = 437F0000 }
	{ r5 = memd(r29+56); r6 = memw(r23+16) }
	{ r3 = memw(r3+16); r2 = memw(r2+16) }
	{ memw(r29+64) = r6; r6 = memw(r20+16) }
	{ memw(r29+52) = r20; r2 = memw(r2) }
	{ r4 = memw(r24+16); r20 = memw(r3) }
	{ memw(r29+48) = r0; r5 = memw(r5+16) }
	{ r24 = sfsub(r2,r20) }
	{ memw(r29+44) = r25; r25 = memw(r18+16); r1:r0 = combine(r26,r24) }
	{ memw(r29+60) = r6; r18 = memw(r5) }
	{ r23 = memw(r4); call fn00009610 }
	{ memw(r29+56) = r0; r18 = r26; r2 = sfsub(r18,r23) }
	{ r26 = r2; r1:r0 = combine(r26,r2); call fn00009610 }
	{ r4 = memd(r29+56); r2 = CF000000 }
	{ r3 = 4F000000 }
	{ r0 = r24; r4 = sfmpy(r4,r0) }
	{ r2 = sfmpy(r4,r2); r3 = sfmpy(r4,r3) }
	{ memw(r29+20) = r2; memw(r29+56) = r3 }
	{ call fmaxf.1.0 }
	{ r1:r0 = combine(r0,r18); call fn00009610 }
	{ r3 = 00000000 }
	{ r20 = r3; r2 = sfsub(r3,r20) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = r26; r2 = r0 }
	{ call fmaxf.1.0; r24 = convert_uw2sf(r2):chop }
	{ r1:r0 = combine(r0,r18); call fn00009610 }
	{ r2 = sfsub(r20,r23) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r20 = memw(r29+32); r26 = memw(r29+72); r4 = add(PC,00012A20) }
	{ r3:r2 = combine(00000002,r20); r1 = 00000174; r18 = convert_uw2sf(r0):chop }
	{ memb(r29+1) = r5.new; r5 = memw(r26+28) }
	{ memw(r29) = r26 }
	{ memw(r29+12) = r19; r3:r2 = combine(00000002,r20); r4 = add(PC,00012A12) }
	{ memw(r29+4) = r17; r1 = 00000175 }
	{ memw(r29+8) = r27; r23 = memw(r29+28) }
	{ memw(r29) = r23; call logmsg_function }
	{ memw(r29+4) = r21; r2 = memd(r29+80); r4 = add(PC,00012A03) }
	{ memw(r29+12) = r2; r3:r2 = combine(00000002,r20); r1 = 00000176 }
	{ memw(r29) = r16; memw(r29+8) = r22 }
	{ call logmsg_function }
	{ r7 = memw(r29+68); r3:r2 = combine(00000002,r20); r4 = add(PC,000129FB) }
	{ r1 = 00000177 }
	{ memw(r29+4) = r7; r5 = memd(r29+44) }
	{ memw(r29) = r5; call logmsg_function }
	{ r1 = 00000178; r3:r2 = combine(00000002,r20); r4 = add(PC,000129EF) }
	{ r26 = memw(r29+48); r5 = memb(r26+32) }
	{ memw(r29) = r5; call logmsg_function }
	{ memw(r29) = r23; memw(r29+12) = r16; r3:r2 = combine(00000002,r20) }
	{ memw(r29+4) = r26; r20 = memw(r29+76); r4 = add(PC,000129D9) }
	{ memw(r29+8) = r20; r1 = 00000179 }
	{ call logmsg_function }
	{ r1 = 0000017A; r7:r6 = combine(r20,r23); r3 = add(PC,000129DC) }
	{ if (!cmp.eq(r2.new,r19)) jump:t 00014560; r2 = memw(r29+80); r5 = r26 }

l000144CC:
	{ r3 = memw(r29+52) }
	{ r4 = memw(r3+20); r2 = mpyi(r2,r20) }
	{ r2 = mpyi(r2,r26) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 000144FC; r2 = asl(r2,00000002) }

l000144E8:
	{ memw(r29+4) = r2; r1 = 0000017C; r3 = add(PC,00000039) }
	{ memw(r29) = r4; r2 = memd(r29+32) }
	{ jump 00014564 }

l000144FC:
	{ r4 = memw(r29+40); r1 = 0000017E; r9:r8 = combine(r5,r3) }
	{ r3 = add(PC,000129AF) }
	{ if (!cmp.eq(r4.new,00000001)) jump:t 00014560; r4 = memw(r4) }

l0001451C:
	{ r3 = memw(r29+40) }
	{ r4 = memw(r3+12); r3 = add(PC,000129A8) }
	{ if (!p0.new) jump:nt 00014560; p0 = cmp.eq(r4,00000001) }

l00014530:
	{ r3 = memd(r29+36); r5 = 00000004; r1 = 00000180 }
	{ r4 = memw(r3+20); r3 = add(PC,000129A1) }
	{ if (p0) jump:nt 00014560; p0 = cmp.gtu(r5,r4) }

l00014548:
	{ r3 = memw(r29+24); r1 = 00000181 }
	{ r4 = memw(r3+20); r3 = add(PC,00012997) }
	{ if (p0.new) jump:nt 00014570; p0 = cmp.gtu(r4,00000003) }

l00014560:
	{ r2 = memw(r29+32) }

l00014564:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000147AC }

l00014570:
	{ memw(r8+4) = r9; r3 = memw(r29+36); p0 = cmp.gt(r6,00000000); r5 = r6 }
	{ memw(r8+8) = r7; memw(r8+12) = r16; if (p0) r5 = add(r9,FFFFFFFF) }
	{ memw(r8+24) = r2 }
	{ memw(r8) = r6; r8 = r7 }
	{ r1 = memd(r29+24); r2 = memw(r3+16) }
	{ memw(r3) = 00000001; memw(r3+12) = 00000001 }
	{ memw(r3+8) = 00000001; memw(r3+4) = 00000001 }
	{ memb(r2) = r4.new; r4 = memw(r29+20) }
	{ memw(r1) = 00000001; memw(r3+24) = 00000004 }
	{ memw(r1+4) = 00000001; r2 = memw(r1+16) }
	{ memw(r1+12) = 00000001; memw(r1+8) = 00000001 }
	{ memb(r2) = r4.new; r4 = memw(r29+56); if (p0) r2 = sub(r22,r27) }
	{ memw(r1+24) = 00000004; if (p0) r4 = sub(r21,r17) }
	{ r7 = memd(r29+44); r6 = memd(r29+68); r1 = mpyi(r22,r19) }
	{ memb(r29+6) = r6.new; r6 = 00000000; r5 = add(r4,mpyi(r5,r7)); r3 = add(r2,mpyi(r3,r6)) }
	{ r4 = mpyi(r1,r16) }
	{ r5 += lsr(r5,0000001F); r3 += lsr(r3,0000001F) }
	{ r7 = asr(r5,00000001); r3 = asr(r3,00000001) }
	{ memw(r29+36) = r7; memw(r29+56) = r3 }

l0001460C:
	{ p0 = cmp.gt(r9,00000000); if (!p0.new) jump:nt 00014774 }

l00014614:
	{ memb(r29+13) = r6.new; r3 = memw(r29+24); r6 = 00000000 }
	{ memb(r29+10) = r3.new; r3 = mpyi(r3,r9) }

l0001462C:
	{ p0 = cmp.gt(r8,00000000); if (!p0.new) jump:nt 0001475C }
	{ r6 = memd(r29+52); r3 = memd(r29+44); r1 = 00000000 }
	{ memw(r29+80) = r1; r5 = memd(r29+40); r3 = mpyi(r6,r3) }
	{ r6 = memw(r29+36); r5 = add(r6,r5) }
	{ memb(r29+18) = r1.new; r13 = sub(r3,r6); r1 = mpyi(r5,r8) }

l00014658:
	{ if (p0.new) r3 = memw(r29+72); r28 = 00000000; if (!p0.new) jump:nt 00014744; p0 = cmp.gt(r8,00000000) }
	{ r5 = memd(r29+68); r6 = memd(r29+80) }
	{ r15 = memw(r29+64) }
	{ r6 = memw(r29+56); r3 = add(r6,r3); r5 = mpyi(r6,r5) }
	{ r5 = memw(r29+60); r0 = sub(r5,r6); r3 = mpyi(r3,r16) }
	{ r1 = addasl(r5,r3,00000002) }

l00014688:
	{ p0 = cmp.gt(r21,00000000); r10 = 00000000; r3 = 00000000; r6 = r15 }
	{ if (!p0) jump:nt 00014730 }

l0001469C:
	{ if (!cmp.gtu(r17,r5.new)) jump:nt 00014724; r5 = add(r3,r13) }

l000146A8:
	{ if (p0.new) r11 = add(r6,00000000) }
	{ if (p0.new) r8 = 00000000; r5 = add(r5,r7); if (!p0.new) jump:nt 00014724; p0 = cmp.gt(r14,00000000) }

l000146B8:
	{ r26 = mpyi(r5,r27); loop1(000146C0,r22) }
	{ if (!cmp.gtu(r27,r5.new)) jump:nt 00014718; r5 = add(r8,r0) }

l000146CC:
	{ if (p0.new) r23 = add(r5,r26); if (p0.new) r14 = add(r19,FFFFFFFF) }
	{ if (p0.new) r20 = memb(r11); if (p0.new) r9 = add(r11,r16); if (!p0.new) jump:nt 00014718; p0 = cmp.gt(r11,00000000) }

l000146E0:
	{ p0 = cmp.gtu(r19,00000001); r20 = sub(r20,r18); r23 = add(r25,mpyi(r23,r19)); loop0(000146FC,r14) }
	{ r5 = memb(r23++#1) }
	{ r5 = sub(r5,r24); if (!p0) jump:nt 00014714 }

l000146FC:
	{ r12 = memb(r23++#1); r14 = memb(r9); r9 = add(r9,r16); r10 += mpyi(r20,r5) }
	{ r5 = sub(r12,r24); r20 = sub(r14,r18) }

l00014714:
	{ r10 += mpyi(r20,r5) }

l00014718:
	{ nop; r11 = add(r11,r2); r8 = add(r8,00000001) }

l00014724:
	{ if (!cmp.eq(r3.new,r21)) jump:t 0001469C; r3 = add(r3,00000001); r6 = add(r6,r4) }

l00014730:
	{ memw(r1++#4) = r10; r15 = add(r15,00000001); r28 = add(r28,00000001) }

l00014734:
	{ memw(r1++#4) = r10; r15 = add(r15,00000001) }

l0001473C:
	{ p0 = cmp.eq(r28,r16); if (!p0.new) jump:nt 00014688 }

l00014744:
	{ r8 = memw(r29+76); r3 = memw(r29+80) }
	{ r3 = add(r3,00000001) }
	{ memw(r29+80) = r3; p0 = cmp.eq(r3,r8); if (!p0.new) jump:nt 00014658 }

l0001475C:
	{ r9 = memw(r29+48); r3 = memw(r29+52) }
	{ r3 = add(r3,00000001) }
	{ memw(r29+52) = r3; p0 = cmp.eq(r3,r9); if (!p0.new) jump:nt 0001462C }

l00014774:
	{ r5 = memd(r29+28); r3 = memd(r29+24) }
	{ r3 = add(r3,00000001) }
	{ memw(r29+24) = r3; if (!p0.new) jump:nt 0001460C; p0 = cmp.eq(r3,r5) }

l00014784:
	{ memw(r29+12) = r16; r3 = 00000002; r4 = add(PC,00012771) }
	{ r2 = memw(r29+32); r1 = 000001B4 }
	{ memw(r29+4) = r9; memw(r29+8) = r8 }
	{ memw(r29) = r5; call logmsg_function }
	{ r0 = 00000000 }

l000147AC:
	{ r19:r18 = memd(r29+120); r17:r16 = memd(r29+128) }
	{ r23:r22 = memd(r29+104); r21:r20 = memd(r29+112) }
	{ r27:r26 = memd(r29+88); r25:r24 = memd(r29+96) }
	{ dealloc_return }

;; logmsg_function: 000147C0
;;   Called from:
;;     00013B7C (in conv2d_execute_hvx)
;;     00013BA0 (in conv2d_execute_hvx)
;;     00013BD0 (in conv2d_execute_hvx)
;;     00013BF4 (in conv2d_execute_hvx)
;;     00013C08 (in conv2d_execute_hvx)
;;     00013C34 (in conv2d_execute_hvx)
;;     00013E34 (in conv2d_execute_hvx)
;;     00013E4C (in conv2d_execute_hvx)
;;     00013E74 (in conv2d_execute_hvx)
;;     00014010 (in conv2d_execute_hvx)
;;     00014040 (in conv2d_check_ref)
;;     00014128 (in conv2d_check_ref)
;;     00014178 (in conv2d_check_ref)
;;     0001419C (in conv2d_check_ref)
;;     000141E0 (in conv2d_ctor)
;;     00014420 (in conv2d_execute_ref)
;;     00014444 (in conv2d_execute_ref)
;;     00014460 (in conv2d_execute_ref)
;;     00014480 (in conv2d_execute_ref)
;;     000144A8 (in conv2d_execute_ref)
;;     000147A0 (in conv2d_execute_ref)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000147E0; r5 = memw(r2+16) }

l000147D0:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000000C) }
	{ memw(r29+4) = r6; call logv }

l000147E0:
	{ dealloc_return }

;; errlog_function: 000147E4
;;   Called from:
;;     00013CF0 (in conv2d_execute_hvx)
;;     00014080 (in conv2d_check_ref)
;;     00014564 (in conv2d_execute_ref)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000125F4) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; fmaxf.1.0: 00014808
;;   Called from:
;;     00013B2C (in conv2d_execute_hvx)
;;     0001438C (in conv2d_execute_ref)
;;     000143B8 (in conv2d_execute_ref)
fmaxf.1.0 proc
	{ r2 = 38D1B717 }
	{ r1:r0 = combine(r0,r2); jump fn00009600 }
00014818                         00 00 00 00 00 00 00 00         ........

;; flatten_execute: 00014820
flatten_execute proc
	{ allocframe(+00000028); r4 = add(PC,000129B2) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+24) = r19:r18; memd(r29+32) = r17:r16; r1 = 00000037; r16 = r1 }
	{ r17 = memw(r2); r2 = r16 }
	{ memd(r29+8) = r23:r22; memd(r29+16) = r21:r20 }
	{ r19 = memw(r17+8); r18 = memw(r3) }
	{ r22 = memw(r17); r21 = memw(r17+4) }
	{ memw(r29) = r0; r20 = memw(r17+12); call logmsg_function }
	{ r2 = memw(r17+24) }
	{ if (!cmp.gtu(r2,r3.new)) jump:t 00014878; r3 = memw(r18+20) }

l00014864:
	{ r2 = r16; r1 = 00000039; r3 = add(PC,0000003C) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000148BC }

l00014878:
	{ memw(r18+8) = 00000001; memw(r18) = 00000001; r2 = mpyi(r21,r22) }
	{ memw(r18+4) = 00000001; r0 = memw(r18+16); r6 = mpyi(r2,r19) }
	{ memb(r18+3) = r7.new; r7 = mpyi(r6,r20) }
	{ memw(r18+24) = r2 }
	{ r2 = memw(r17+24); r1 = memw(r17+16); call fn00009560 }
	{ r2 = r16; r1 = 0000003F; r4 = add(PC,000128CA) }
	{ memb(r29) = r3.new; r3 = memw(r17+24); call logmsg_function }

l000148BC:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; flatten_check: 000148C8
flatten_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000128C1) }
	{ r17 = r0; r16 = r1; r1 = 0000005C }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 0000005D; r3 = add(PC,00012824) }
	{ if (cmp.gtu(r2.new,00000002)) jump:t 0001493C; r2 = memw(r17+16) }

l000148FC:
	{ r1 = 0000005E; r3 = add(PC,0000001F) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 0001493C; r2 = memw(r17+20) }

l00014910:
	{ r2 = memw(r17+4); r1 = 0000005F; r3 = add(PC,00000016) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001493C; r2 = memw(r2) }

l00014928:
	{ r2 = memw(r17+8); r1 = 00000060; r3 = add(PC,00000009) }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001494C; r2 = memw(r2) }

l0001493C:
	{ r2 = r16; call errlog_function }

l00014940:
	{ r2 = r16 }

l00014944:
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001494C:
	{ r2 = r16; r1 = 00000061; r4 = add(PC,0001286D) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; qflatten_execute: 0001496C
qflatten_execute proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r4 = add(PC,000127D5) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+24) = r19:r18; r2 = memw(r17+4); r1 = 0000004C }
	{ r3 = memw(r17+8) }
	{ memd(r29+8) = r23:r22; memd(r29+16) = r21:r20 }
	{ r19 = memw(r3); r18 = memw(r2); r2 = r16 }
	{ r20 = memw(r18+8) }
	{ r23 = memw(r18); r22 = memw(r18+4) }
	{ memw(r29) = r17; r21 = memw(r18+12); call logmsg_function }
	{ r2 = memw(r18+24) }
	{ if (!cmp.gtu(r2,r3.new)) jump:t 000149CC; r3 = memw(r19+20) }

l000149B4:
	{ r2 = r16; r1 = 0000004E; r3 = add(PC,0000002C) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00014A70 }

l000149CC:
	{ memw(r19+8) = 00000001; memw(r19) = 00000001; r2 = mpyi(r22,r23) }
	{ memw(r19+4) = 00000001; r0 = memw(r19+16); r6 = mpyi(r2,r20) }
	{ memb(r19+3) = r7.new; r7 = mpyi(r6,r21) }
	{ memw(r19+24) = r2 }
	{ r2 = memw(r18+24); r1 = memw(r18+16); call fn00009560 }
	{ r3 = memw(r17+4); r2 = memw(r17+8) }
	{ r2 = memw(r2+4); r3 = memw(r3+4) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 00014A24; r6 = memw(r2+20) }

l00014A1C:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l00014A24:
	{ r3 = memw(r17+4); r2 = memw(r17+8) }
	{ r2 = memw(r2+8); r3 = memw(r3+8) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 00014A54; r6 = memw(r2+20) }

l00014A4C:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l00014A54:
	{ r2 = memw(r18+24); r4 = add(PC,00012716) }
	{ memw(r29) = r2; r2 = r16; r1 = 00000056 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00014A70:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }
00014A7C                                     00 C0 00 7F             ....

;; qflatten_check: 00014A80
qflatten_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0001266E) }
	{ r17 = r0; r16 = r1; r1 = 00000067 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000003)) jump:t 00014AC0; r2 = memw(r17+16); r1 = 00000068 }

l00014AAC:
	{ r3 = add(PC,00000020) }
	{ r2 = r16; call errlog_function }

l00014AB4:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00014AC0:
	{ if (cmp.eq(r2.new,00000003)) jump:t 00014AD8; r2 = memw(r17+20); r1 = 00000069 }

l00014AD0:
	{ jump 00014AB4; r3 = add(PC,0000000B) }

l00014AD8:
	{ r2 = r16; r1 = 0000006A; r4 = add(PC,0001264F) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00014AF8
;;   Called from:
;;     0001484C (in flatten_execute)
;;     000148AC (in flatten_execute)
;;     000148DC (in flatten_check)
;;     0001495C (in flatten_check)
;;     0001499C (in qflatten_execute)
;;     00014A68 (in qflatten_execute)
;;     00014A94 (in qflatten_check)
;;     00014AE8 (in qflatten_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00014B1C; r5 = memw(r2+16) }

l00014B08:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000000D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00014B1C:
	{ dealloc_return }

;; errlog_function: 00014B20
;;   Called from:
;;     0001486C (in flatten_execute)
;;     0001493C (in flatten_check)
;;     000149C0 (in qflatten_execute)
;;     00014AB0 (in qflatten_check)
;;     00014B10 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000125B1) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00014B44             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; input_execute: 00014B50
input_execute proc
	{ allocframe(00000020); memd(r29+496) = r17:r16; r4 = add(PC,000126FF) }
	{ r16 = r1; r1 = 00000031 }
	{ memd(r29+8) = r21:r20; memd(r29+16) = r19:r18; r18 = r0; r2 = r16 }
	{ memw(r29) = r18; call logmsg_function }
	{ r2 = memw(r16+28) }
	{ if (!cmp.eq(r3.new,r2)) jump:t 00014BB0; r3 = memw(r18+20) }

l00014B84:
	{ r2 = 00000000; r17 = 00000000 }
	{ r21:r20 = combine(00000000,00000000); r19 = 00000000 }

l00014B8C:
	{ r4 = memw(r16+20); r3 = memw(r18+8) }
	{ r3 = memw(r29+r19); r2 = add(r4,r20) }
	{ r5 = memw(r2+20) }
	{ if (!cmp.gtu(r5,r6.new)) jump:t 00014BC8; r6 = memw(r3+20) }

l00014BA8:
	{ jump 00014BC0; r11 = 0000003B; r3 = add(PC,00000012) }

l00014BB0:
	{ r1 = 00000036; r3 = add(PC,000126B8) }
	{ r17 = -00000001; r2 = r16; call errlog_function }

l00014BC0:
	{ r17 = -00000001; r2 = r16 }

l00014BC4:
	{ jump 00014C28 }

l00014BC8:
	{ memb(r3) = r4.new; r4 = memw(r5+r20) }
	{ memw(r3+4) = r7 }
	{ r4 = memw(r2+8) }
	{ memw(r3+8) = r4 }
	{ r7 = memw(r2+12) }
	{ memw(r3+12) = r7 }
	{ r4 = memw(r2+20) }
	{ memw(r3+24) = r4 }
	{ r1 = memw(r2+16) }
	{ r2 = memw(r2+20); r0 = memw(r3+16); call vmemcpy_asm }
	{ r2 = memw(r18+20); r20 = add(r20,00000020); r19 = add(r19,00000004) }
	{ if (cmp.gtu(r2,r21.new)) jump:t 00014B8C; r21 = add(r21,00000001) }

l00014C10:
	{ memw(r29) = r2; r1 = 00000040; r4 = add(PC,00012674) }
	{ r2 = r16; call logmsg_function }

l00014C28:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r17 }
	{ dealloc_return; r21:r20 = memd(r29+8) }

;; input_check: 00014C34
input_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000125D3) }
	{ r17 = r0; r16 = r1; r1 = 00000046 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00014C94; r2 = memw(r17+20) }

l00014C5C:
	{ r3 = memw(r17+8) }
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 00014C94; r4 = add(r4,00000001); r3 = add(r3,00000004) }

l00014C64:
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 00014C98; r4 = add(r4,00000001) }

l00014C70:
	{ if (!cmp.eq(r5.new,00000000)) jump:t 00014C64 }

l00014C78:
	{ r3:r2 = combine(00000000,r16); r1 = 0000004A; r4 = add(PC,0000002A) }
	{ memw(r29) = r4; call errlog_function }
	{ r0 = FFFFFFFF }
	{ dealloc_return; r17:r16 = memd(r29+8) }

l00014C94:
	{ r2 = r16; r1 = 0000004D; r4 = add(PC,000125A4) }

l00014C98:
	{ r2 = r16; r1 = 0000004D; r4 = add(PC,00000024) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00014CB4
;;   Called from:
;;     00014B6C (in input_execute)
;;     00014C20 (in input_execute)
;;     00014C48 (in input_check)
;;     00014CA4 (in input_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00014CD8; r5 = memw(r2+16) }

l00014CC4:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000002C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00014CD8:
	{ dealloc_return }

;; errlog_function: 00014CDC
;;   Called from:
;;     00014BBC (in input_execute)
;;     00014C84 (in input_check)
;;     00014CCC (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00012510) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; matmul_execute_asm: 00014D00
matmul_execute_asm proc
	{ jump matmul_execute; r2 = add(PC,00000724) }

;; matmul_check_ref: 00014D0C
matmul_check_ref proc
	{ allocframe(00000048); memd(r29+496) = r17:r16; r4 = add(PC,000126DD) }
	{ r17 = r0; r16 = r1; r1 = 0000010F }
	{ memd(r29+48) = r21:r20; memd(r29+56) = r19:r18; r3:r2 = combine(00000002,r16) }
	{ memd(r29+32) = r25:r24; memd(r29+40) = r23:r22 }
	{ memw(r29) = r17; call logmsg_function }
	{ r1 = 00000110; r3 = add(PC,000126C9) }
	{ if (!cmp.eq(r2.new,00000006)) jump:t 00014EE0; r2 = memw(r17+16) }

l00014D50:
	{ r1 = 00000111; r3 = add(PC,0000000B) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 00014EE0; r2 = memw(r17+20) }

l00014D64:
	{ r1 = 00000112; r3 = add(PC,0000000E) }
	{ if (cmp.eq(r4.new,00000000)) jump:nt 00014EE0; r4 = memw(r17+4) }

l00014D78:
	{ r2 = memw(r17+8); r1 = 00000113; r3 = add(PC,00000006) }
	{ r5 = 00000000; if (p0.new) jump:nt 00014EE0; p0 = cmp.eq(r2,00000000); loop0(00014D90,00000006) }

l00014D90:
	{ if (cmp.eq(r3.new,00000000)) jump:nt 00014DA4; r3 = memw(r4) }

l00014D9C:
	{ r5 = r5; r4 = add(r4,00000004) }
	{ jump 00014DC4; r0 = 00000000 }

l00014DA4:
	{ memw(r29) = r5; r1 = 00000116; r3 = add(PC,000126A3) }
	{ jump 00014EE0 }

l00014DB8:
	{ if (cmp.gtu(r4.new,00000002)) jump:nt 00014DE0; r4 = add(r4,00000001); r2 = add(r2,00000004) }

l00014DBC:
	{ if (cmp.gtu(r4.new,00000002)) jump:nt 00014DE4; r4 = add(r4,00000001) }

l00014DC4:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 00014DB8; r3 = memw(r2) }

l00014DC8:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 00014DBC }

l00014DD0:
	{ memw(r29) = r4; r1 = 0000011B; r3 = add(PC,00000009) }
	{ jump 00014EE0 }

l00014DE0:
	{ r1 = 00000120; r3:r2 = combine(00000003,r16); r4 = add(PC,00012684) }

l00014DE4:
	{ r1 = 00000120; r3:r2 = combine(00000003,r16); r4 = add(PC,00000004) }

l00014DF0:
	{ memw(r29) = r17; call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00014E58; r2 = memw(r17+16); r19:r18 = combine(00000000,00000000) }

l00014E04:
	{ r2 = memw(r17+4); r1 = 00000109; r4 = add(PC,00012723) }

l00014E08:
	{ r2 = memw(r17+4); r1 = 00000109; r4 = add(PC,00000023) }

l00014E14:
	{ r2 = memw(r13+r18) }
	{ r6 = memw(r2+24); r5 = memw(r2+16) }
	{ r8 = memw(r2+4); r7 = memw(r2+12) }
	{ r12 = memw(r2); r9 = memw(r2+8); r3:r2 = combine(00000003,r16) }
	{ memw(r29+20) = r6; memw(r29+24) = r5 }
	{ memw(r29+12) = r9; memw(r29+16) = r7 }
	{ memw(r29+4) = r12; memw(r29+8) = r8 }
	{ memw(r29) = r19; call logmsg_function }
	{ r2 = memw(r17+16); r18 = add(r18,00000004) }
	{ if (cmp.gtu(r2,r19.new)) jump:t 00014E04; r19 = add(r19,00000001) }

l00014E58:
	{ r2 = memw(r17+4) }

l00014E5C:
	{ r3 = memw(r2+20) }
	{ r7 = memw(r2+4); r4 = memw(r2+16) }
	{ r3 = memw(r3+16); r2 = memw(r4+16) }
	{ r20 = memw(r7+8); r18 = memw(r7+16) }
	{ r2 = memw(r3); r21 = memw(r2) }
	{ r19 = memw(r7+12); r0 = sfsub(r2,r21) }
	{ call fmaxf.1.0 }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = 00000000 }
	{ r2 = sfsub(r2,r21) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r4 = add(r19,0000001F); r2 = add(r20,0000000F) }
	{ r0 = 00000080; r22 = and(r4,FFFFFFE0); r3:r2 = combine(00000020,r0); r5 = and(r2,FFFFFFF0) }
	{ r24 = convert_uw2sf(r2):chop; r23 = maxu(r5,r3) }
	{ call fn00009550; r1 = mpyi(r23,r22) }
	{ memw(r17+40) = r0; r1 = 0000013A; r3 = add(PC,000125D7) }
	{ p1 = cmp.gt(r24,FFFFFFFF); if (!p0.new) jump:nt 00014EEC; p0 = cmp.eq(r0,00000000) }

l00014EE0:
	{ r21 = -00000001; r2 = r16; call errlog_function }
	{ jump 00014F7C }

l00014EEC:
	{ r0 = r16; r21 = 00000000; p0 = cmp.gt(r24,000000FF) }
	{ if (!p0) r25 = zxtb(r24); if (p0) r25 = 000000FF }
	{ if (!p1) r25 = add(r21,00000000); call nn_os_hvx_power_on }
	{ call nn_os_vector_acquire }
	{ memw(r29+16) = r25; r3:r2 = combine(00000002,r16); r4 = add(PC,000125C8) }
	{ memw(r29+4) = r19; r1 = 0000013E }
	{ memw(r29+8) = r23; memw(r29+12) = r22; r24 = r0 }
	{ memw(r29) = r20; call logmsg_function }
	{ r3 = memw(r16+4); r2 = r19; r1:r0 = combine(r20,r18); r5:r4 = combine(r22,r23) }
	{ memw(r29) = r25; call pad2d }
	{ r3 = memw(r17+8); r0 = memw(r16+4); r1 = r23; r2 = r22 }
	{ call transpack }
	{ r0 = r24; call nn_os_vector_release }
	{ r0 = r16; call nn_os_hvx_power_off }
	{ r1 = 00000143; r3:r2 = combine(00000002,r16); r4 = add(PC,000125AB) }
	{ memw(r29) = r17; call logmsg_function }

l00014F7C:
	{ r0 = r21 }
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ dealloc_return; r25:r24 = memd(r29+32) }

;; matmul_ctor: 00014F90
matmul_ctor proc
	{ allocframe(00000028); memd(r29+488) = r19:r18; r6 = add(PC,00012442) }
	{ r19:r18 = combine(r0,r4) }
	{ memd(r29+16) = r21:r20; memd(r29+32) = r17:r16; r21:r20 = combine(r2,r5); r17:r16 = combine(r3,r1) }
	{ r4 = r6; r3:r2 = combine(00000002,r19); r1 = 00000152 }
	{ memd(r29+8) = r23:r22; r22 = memd(r29+48) }
	{ r23 = memw(r29+52) }
	{ memw(r29) = r16; call logmsg_function }
	{ memw(r29+4) = r23; r1:r0 = combine(r16,r19); r3:r2 = combine(r17,r21); r5:r4 = combine(r20,r18) }
	{ memw(r29) = r22; call node_alloc_common }
	{ r17:r16 = memd(r29+32) }
	{ r21:r20 = memd(r29+16); r19:r18 = memd(r29+24) }
	{ dealloc_return; r23:r22 = memd(r29+8) }

;; matmul_execute_ref: 00014FEC
matmul_execute_ref proc
	{ jump matmul_execute; r2 = add(PC,000002F4) }

;; matmul_execute: 00014FF8
;;   Called from:
;;     00014D00 (in matmul_execute_asm)
;;     00014FEC (in matmul_execute_ref)
matmul_execute proc
	{ allocframe(00000078); memd(r29+496) = r17:r16; r16 = r0 }
	{ memw(r29+40) = r2; r3 = memw(r16+4) }
	{ r4 = memw(r16+8) }
	{ memd(r29+72) = r27:r26; memw(r29+68) = r1 }
	{ r2 = memw(r4+4); r7 = memw(r3) }
	{ r5 = memw(r3+12) }
	{ memw(r29+48) = r2; r2 = memw(r4) }
	{ r6 = memw(r3+8) }
	{ memw(r29+52) = r2; r2 = memw(r7+8) }
	{ r9 = memw(r3+4) }
	{ memw(r29+60) = r2; r2 = memw(r6+16) }
	{ r1 = memw(r4+8); r5 = memw(r5+16) }
	{ r8 = memw(r3+20); r4 = memw(r9) }
	{ r3 = memw(r3+16) }
	{ memw(r29+56) = r4; r4 = memw(r5) }
	{ r26 = memw(r2) }
	{ memd(r29+96) = r21:r20; memd(r29+104) = r19:r18; r19 = 437F0000 }
	{ r8 = memw(r8+16) }
	{ memw(r29+44) = r1; r1 = memw(r7+12); r20 = sfsub(r4,r26) }
	{ r3 = memw(r3+16) }
	{ memd(r29+80) = r25:r24; memw(r29+64) = r1; r1:r0 = combine(r19,r20) }
	{ memd(r29+88) = r23:r22; r18 = memw(r7+4) }
	{ r25 = memw(r9+8); r17 = memw(r7) }
	{ r23 = memw(r9+12); r24 = memw(r9+4) }
	{ r27 = memw(r3); r21 = memw(r8); call fn00009610 }
	{ r22 = r0; r21 = sfsub(r21,r27) }
	{ r1:r0 = combine(r19,r21); call fn00009610 }
	{ r3 = 4F000000; r4 = sfmpy(r22,r0) }
	{ r0 = r20; r2 = CF000000 }
	{ memb(r29+9) = r3.new; r22 = sfmpy(r4,r2); r3 = sfmpy(r4,r3) }
	{ r20 = 00000000; r1:r0 = combine(r0,r19); call fn00009610 }
	{ r24 = r17; r26 = r24; r2 = sfsub(r20,r26) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = r21; r2 = r0 }
	{ call fmaxf.1.0; r19 = convert_uw2sf(r2):chop }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r16 = memd(r29+68); r20 = r16; r2 = sfsub(r20,r27) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r1 = 00000072; r3:r2 = combine(00000002,r16); r4 = add(PC,000121EC) }
	{ memw(r29) = r20; call logmsg_function; r21 = convert_uw2sf(r0):chop }
	{ memw(r29+20) = r26; memw(r29+28) = r23; r4 = add(PC,000121E8) }
	{ r2 = memw(r29+64); r1 = 00000075 }
	{ memw(r29+24) = r25; r25 = memw(r29+56) }
	{ memw(r29+16) = r25; r17 = memw(r29+60) }
	{ memw(r29) = r24; memw(r29+12) = r2; r3:r2 = combine(00000002,r16) }
	{ memw(r29+4) = r18; memw(r29+8) = r17 }
	{ call logmsg_function }
	{ memw(r29+12) = r23; r3:r2 = combine(00000002,r16); r4 = add(PC,000121DE) }
	{ memw(r29) = r24; r1 = 00000077 }
	{ memw(r29+4) = r18; memw(r29+8) = r17 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r18,00000001); r1 = 00000078; r3 = add(PC,000121DF) }
	{ if (p0) r1 = 00000079; if (!p0) jump:nt 00015240 }

l000151A0:
	{ if (p0.new) r1 = 0000007A; p0 = cmp.eq(r26,00000001); r3 = add(PC,000121C7) }
	{ if (!p0) jump:nt 00015240 }

l000151B4:
	{ if (p0.new) r1 = 0000007B; p0 = cmp.eq(r24,00000001); r3 = add(PC,000121C5) }
	{ if (!p0) jump:nt 00015240 }

l000151C8:
	{ p0 = cmp.eq(r25,00000001); r2 = mpyi(r24,r17); r3 = add(PC,000121B1) }
	{ if (!p0) jump:nt 00015240 }

l000151DC:
	{ r3 = memw(r29+52); r1 = 0000007C }
	{ r2 = mpyi(r2,r18) }
	{ r4 = memw(r3+20); r3 = add(PC,000121A8) }
	{ r2 = mpyi(r2,r23) }
	{ if (cmp.gtu(r2.new,r4)) jump:t 00015240; r2 = asl(r2,00000002) }

l00015204:
	{ r3 = memd(r29+48); r5 = 00000004; r1 = 0000007D }
	{ r4 = memw(r3+20); p2 = cmp.gt(r19,FFFFFFFF); r3 = add(PC,00012195) }
	{ p3 = cmp.gt(r21,FFFFFFFF); if (p0) jump:nt 00015240; p0 = cmp.gtu(r5,r4) }

l00015224:
	{ r3 = memd(r29+44); r5 = 00000000; r1 = 0000007E }
	{ r4 = memw(r3+20); r3 = add(PC,00012183) }
	{ if (!p0) r1:r0 = combine(r16,r20); if (p0.new) jump:nt 00015250; p0 = cmp.gtu(r4,00000003) }

l00015240:
	{ r2 = r16; call errlog_function }
	{ r0 = FFFFFFFF; jump 000152C4 }

l00015250:
	{ r6 = memd(r29+48); r7 = memd(r29+52); r3 = 000000FF; p0 = cmp.gt(r19,000000FF) }
	{ memw(r7+12) = r23; memw(r7) = 00000001 }
	{ memw(r7+4) = 00000001; memw(r7+8) = r17 }
	{ memw(r7+24) = r2; if (!p1) r3 = zxtb(r21); if (p0) r2 = add(r3,00000000) }
	{ r7 = memd(r29+44); r4 = memw(r6+16); if (!p3) r3 = add(r5,00000000); if (!p0) r2 = zxtb(r19) }
	{ memw(r6+12) = 00000001; memw(r6+8) = 00000001; if (!p2) r2 = add(r5,00000000) }
	{ memw(r6+4) = 00000001; memw(r6) = 00000001 }
	{ memw(r4) = r22 }
	{ memw(r7+8) = 00000001; r4 = memw(r7+16) }
	{ memw(r7) = 00000001; memw(r7+4) = 00000001 }
	{ memw(r7+12) = 00000001; r5 = memd(r29+36) }
	{ memw(r4) = r5; r4 = memd(r29+40) }
	{ memw(r7+24) = 00000004; memw(r6+24) = 00000004; callr r4 }
	{ r1 = 0000008C; r3:r2 = combine(00000002,r16); r4 = add(PC,00012111) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l000152C4:
	{ r19:r18 = memd(r29+104); r17:r16 = memd(r29+112) }
	{ r23:r22 = memd(r29+88); r21:r20 = memd(r29+96) }
	{ r27:r26 = memd(r29+72); r25:r24 = memd(r29+80) }
	{ dealloc_return }
000152D8                         00 40 00 7F 00 C0 00 7F         .@......

;; matmul_ref: 000152E0
matmul_ref proc
	{ allocframe(+00000038); r4 = add(PC,00011FD1) }
	{ r5 = memw(r0+4); r6 = memw(r0+8) }
	{ memd(r29+48) = r17:r16; memd(r29+40) = r19:r18; r17:r16 = combine(r1,r3); r18 = r2 }
	{ r5 = memw(r5+4); r2 = memw(r5); r1 = 000000AD }
	{ memd(r29+32) = r21:r20; r6 = memw(r6) }
	{ memd(r29+24) = r23:r22 }
	{ memd(r29+16) = r25:r24; r19 = memw(r2+12) }
	{ r23 = memw(r2+16); r20 = memw(r2+8) }
	{ r22 = memw(r5+12); r21 = memw(r5+16); r3:r2 = combine(00000002,r17) }
	{ memw(r29+12) = r16; r24 = memw(r6+16) }
	{ memw(r29+4) = r19; memw(r29+8) = r18 }
	{ memw(r29) = r20; call logmsg_function }
	{ r2 = 00000000; if (p0.new) jump:nt 000153B4; p0 = cmp.eq(r12,00000000) }

l0001533C:
	{ if (p0.new) jump:nt 000153AC; p0 = cmp.eq(r14,00000000); r5 = mpyi(r2,r22) }

l00015344:
	{ r3 = r21; r4 = r2; loop1(00015354,r22) }
	{ r4 = add(r23,mpyi(r4,r19)) }
	{ r5 = addasl(r24,r5,00000002) }
	{ r6 = 00000000; r12 = add(r19,FFFFFFFF); if (p0.new) jump:nt 000153A0; p0 = cmp.eq(r11,00000000) }

l00015360:
	{ r9 = memb(r3); p0 = cmp.gtu(r19,00000001); r8 = add(r3,r22); r7:r6 = combine(r4,00000000) }
	{ r13 = memb(r7++#1); r9 = sub(r9,r16); loop0(00015384,r12) }
	{ r12 = sub(r13,r18); if (!p0) jump:nt 0001539C }

l00015384:
	{ r14 = memb(r7++#1); r13 = memb(r8); r8 = add(r8,r22); r6 += mpyi(r9,r12) }
	{ r12 = sub(r14,r18); r9 = sub(r13,r16) }

l0001539C:
	{ r6 += mpyi(r9,r12) }

l000153A0:
	{ memw(r5++#4) = r6; nop; r3 = add(r3,00000001) }

l000153AC:
	{ if (!cmp.eq(r2.new,r20)) jump:t 0001533C; r2 = add(r2,00000001) }

l000153B4:
	{ r1 = 000000BA; r3:r2 = combine(00000002,r17); r4 = add(PC,00011F2F) }

l000153B8:
	{ r1 = 000000BA; r3:r2 = combine(00000002,r17); r4 = add(PC,0000002F) }
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48); call logmsg_function }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ dealloc_return; r25:r24 = memd(r29+16) }

;; logmsg_function: 000153D8
;;   Called from:
;;     00014D30 (in matmul_check_ref)
;;     00014DF0 (in matmul_check_ref)
;;     00014E44 (in matmul_check_ref)
;;     00014F28 (in matmul_check_ref)
;;     00014F74 (in matmul_check_ref)
;;     00014FC0 (in matmul_ctor)
;;     00015120 (in matmul_execute)
;;     00015164 (in matmul_execute)
;;     00015184 (in matmul_execute)
;;     000152BC (in matmul_execute)
;;     0001532C (in matmul_ref)
;;     000153C4 (in matmul_ref)
;;     000153C4 (in matmul_ref)
;;     00015490 (in matmul_asm)
;;     0001550C (in matmul_asm)
;;     000155A4 (in matmul_asm)
;;     000155B8 (in matmul_asm)
;;     000155D0 (in matmul_asm)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000153F8; r5 = memw(r2+16) }

l000153E8:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,00000031) }
	{ memw(r29+4) = r6; call logv }

l000153F8:
	{ dealloc_return }
000153FC                                     00 C0 00 7F             ....

;; errlog_function: 00015400
;;   Called from:
;;     00014EE0 (in matmul_check_ref)
;;     00015240 (in matmul_execute)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00011E95) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; matmul_asm: 00015424
matmul_asm proc
	{ allocframe(00000050); memd(r29+480) = r21:r20; r21:r20 = combine(r0,r3) }
	{ memd(r29+40) = r25:r24; r4 = memw(r21+4) }
	{ r5 = memw(r21+8) }
	{ memd(r29+64) = r19:r18; memd(r29+72) = r17:r16; r16 = r1; r19 = r2 }
	{ r5 = memw(r5); r6 = memw(r4) }
	{ memd(r29+48) = r23:r22; r4 = memw(r4+4) }
	{ r17 = memw(r6+16); r24 = memw(r6+8) }
	{ r18 = memw(r5+16); r23 = memw(r6+12); if (p0.new) r1 = 000000DB; p0 = cmp.eq(r24,00000001) }
	{ memd(r29+32) = r27:r26; r22 = memw(r4+12); if (p0) r25 = add(r23,0000000F); if (!p0) jump:nt 000154F0 }

l00015470:
	{ memw(r29+4) = r23; memw(r29+20) = r20; r4 = add(PC,000120E5) }
	{ memw(r29+28) = r16; memw(r29+16) = r19; r3:r2 = combine(00000002,r16); r24 = and(r25,FFFFFFF0) }
	{ memw(r29+8) = 00000001; memw(r29+12) = r24 }
	{ memw(r29) = 00000001; call logmsg_function }
	{ r2 = r25; r23 = FFFFFE00; if (p0.new) jump:nt 000155C0; p0 = cmp.eq(r14,00000000) }

l000154A4:
	{ r16 = 00000000; r27:r26 = combine(00000000,00000020); r25 = r22 }
	{ r20 = sub(00000000,r20); r19 = sub(00000000,r19); r23 &= asl(r2,00000005) }

l000154BC:
	{ r2 = memw(r21+8); r4 = r18; r1:r0 = combine(r19,r17); r5 = min(r26,r25) }
	{ memw(r29) = r24; r2 = add(r2,r16); r3 = r20; call gemvmpybbw_asm }
	{ r18 = add(r18,00000080); r16 = add(r16,r23); r25 = add(r25,FFFFFFE0) }
	{ if (cmp.gtu(r22,r27.new)) jump:t 000154BC; r27 = add(r27,00000020) }

l000154F0:
	{ memw(r29+12) = r20; r21 = memw(r4+16); r4 = add(PC,00011DC1) }
	{ memw(r29) = r24; r3:r2 = combine(00000002,r16) }
	{ memw(r29+4) = r23; memw(r29+8) = r19; r1 = 000000AD }
	{ call logmsg_function }
	{ p0 = cmp.eq(r24,00000000); r2 = 00000000; if (p0.new) jump:nt 00015594 }

l0001551C:
	{ if (p0.new) jump:nt 0001558C; p0 = cmp.eq(r14,00000000); r5 = mpyi(r2,r22) }

l00015524:
	{ r3 = r21; r4 = r2; loop1(00015534,r22) }
	{ r4 = add(r17,mpyi(r4,r23)) }
	{ r5 = addasl(r18,r5,00000002) }
	{ r6 = 00000000; r12 = add(r23,FFFFFFFF); if (p0.new) jump:nt 00015580; p0 = cmp.eq(r15,00000000) }

l00015540:
	{ r9 = memb(r3); p0 = cmp.gtu(r23,00000001); r8 = add(r3,r22); r7:r6 = combine(r4,00000000) }
	{ r13 = memb(r7++#1); r9 = sub(r9,r20); loop0(00015564,r12) }
	{ r12 = sub(r13,r19); if (!p0) jump:nt 0001557C }

l00015564:
	{ r14 = memb(r7++#1); r13 = memb(r8); r8 = add(r8,r22); r6 += mpyi(r9,r12) }
	{ r12 = sub(r14,r19); r9 = sub(r13,r20) }

l0001557C:
	{ r6 += mpyi(r9,r12) }

l00015580:
	{ memw(r5++#4) = r6; nop; r3 = add(r3,00000001) }

l0001558C:
	{ if (!cmp.eq(r2.new,r24)) jump:t 0001551C; r2 = add(r2,00000001) }

l00015594:
	{ r1 = 000000BA; r3:r2 = combine(00000002,r16); r4 = add(PC,00011D4F) }

l00015598:
	{ r1 = 000000BA; r3:r2 = combine(00000002,r16); r4 = add(PC,0000000F) }

l000155A4:
	{ call logmsg_function }
	{ r1 = 000000EB; r3:r2 = combine(00000002,r16); r4 = add(PC,00012002) }
	{ memw(r29+28) = r16; call logmsg_function }

l000155C0:
	{ r1 = 000000EE; r3 = 00000002; r4 = add(PC,0001203B) }
	{ r17:r16 = memd(r29+72); r2 = memd(r29+28); call logmsg_function }
	{ r21:r20 = memd(r29+56); r19:r18 = memd(r29+64) }
	{ r25:r24 = memd(r29+40); r23:r22 = memd(r29+48) }
	{ dealloc_return; r27:r26 = memd(r29+32) }

;; fmaxf.1.0: 000155EC
;;   Called from:
;;     00014E78 (in matmul_check_ref)
;;     000150E8 (in matmul_execute)
fmaxf.1.0 proc
	{ r2 = 38D1B717 }
	{ r1:r0 = combine(r0,r2); jump fn00009600 }
000155FC                                     00 00 00 00             ....

;; maxpool_execute_asm: 00015600
maxpool_execute_asm proc
	{ jump maxpool_execute; r2 = add(PC,00000794) }

;; maxpool_check: 0001560C
maxpool_check proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r4 = add(PC,00012087) }
	{ r17 = r0; r16 = r1; r1 = 0000016C }
	{ memw(r29) = r17; memd(r29+32) = r19:r18; r3:r2 = combine(00000002,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,00000005)) jump:t 00015650; r2 = memw(r17+16); r1 = 0000016D }

l0001563C:
	{ r3 = add(PC,00000034) }

l00015640:
	{ r2 = r16; call errlog_function }

l00015644:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 00015748 }

l00015650:
	{ if (cmp.eq(r2.new,00000003)) jump:t 0001566C; r2 = memw(r17+20); r1 = 00000171 }

l00015660:
	{ r1 = 0000016E; jump 00015644; r3 = add(PC,00000027) }

l0001566C:
	{ memw(r29) = r17; r3:r2 = combine(00000003,r16); r4 = add(PC,0001206C) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000156F4; r2 = memw(r17+16); r19:r18 = combine(00000000,00000000) }

l0001568C:
	{ r2 = memw(r17+4) }

l00015690:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 000156AC; r2 = memw(r5+r18) }

l0001569C:
	{ memw(r29) = r19; r1 = 00000174; r3 = add(PC,0000003D) }
	{ jump 00015640 }

l000156AC:
	{ r6 = memw(r2+4); r5 = memw(r2); r4 = add(PC,000120AF) }
	{ r8 = memw(r2+12); r7 = memw(r2+8); r1 = 00000166 }
	{ r2 = memw(r2+16); r9 = memw(r2+24) }
	{ memw(r29+12) = r7; memw(r29+24) = r2; r3:r2 = combine(00000003,r16) }
	{ memw(r29+16) = r8; memw(r29+20) = r9 }
	{ memw(r29+4) = r5; memw(r29+8) = r6 }
	{ memw(r29) = r19; call logmsg_function }
	{ r2 = memw(r17+16); r18 = add(r18,00000004) }
	{ if (cmp.gtu(r2,r19.new)) jump:t 0001568C; r19 = add(r19,00000001) }

l000156F4:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001572C; r2 = memw(r17+20) }

l000156F8:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00015730 }

l00015700:
	{ r3 = memw(r17+8) }
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 0001572C; r4 = add(r4,00000001); r3 = add(r3,00000004) }

l00015708:
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 00015730; r4 = add(r4,00000001) }

l00015714:
	{ if (!cmp.eq(r5.new,00000000)) jump:t 00015708 }

l0001571C:
	{ memw(r29) = r4; r1 = 0000017A; r3 = add(PC,00000013) }
	{ jump 00015640 }

l0001572C:
	{ r1 = 0000017D; r3:r2 = combine(00000002,r16); r4 = add(PC,00012016) }

l00015730:
	{ r1 = 0000017D; r3:r2 = combine(00000002,r16); r4 = add(PC,00000016) }

l0001573C:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l00015748:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ dealloc_return }

;; maxpool_execute_ref: 00015750
maxpool_execute_ref proc
	{ jump maxpool_execute; r2 = add(PC,00000290) }
0001575C                                     00 C0 00 7F             ....

;; maxpool_execute: 00015760
;;   Called from:
;;     00015600 (in maxpool_execute_asm)
;;     00015750 (in maxpool_execute_ref)
maxpool_execute proc
	{ allocframe(00000088); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+112) = r21:r20; r3 = memw(r17+4) }
	{ r4 = memw(r17+8); r21 = memb(r17+32) }
	{ memd(r29+96) = r25:r24; memd(r29+88) = r27:r26 }
	{ r25 = memw(r3+16); r5 = memw(r3); p0 = cmp.eq(r21,00000000) }
	{ memw(r29+8) = r2; r2 = memw(r3+8) }
	{ r26 = memw(r3+12); r0 = p0 }
	{ memw(r29+24) = r2; r2 = memw(r3+4) }
	{ memd(r29+120) = r19:r18; memd(r29+104) = r23:r22 }
	{ memw(r29+12) = r2; r27 = memw(r4) }
	{ r2 = memw(r5+8); r23 = memw(r5) }
	{ r24 = memw(r5+12); r20 = memw(r5+4) }
	{ r22 = memw(r25+4); r1 = memw(r25+8) }
	{ r6 = memw(r4+4); r18 = memw(r26+4) }
	{ r7 = memw(r4+8) }
	{ memw(r29+20) = r7; memw(r29+16) = r6 }
	{ memw(r29+28) = r0; if (!p0) jump:nt 000157D4 }

l000157D0:
	{ jump 000157F8; r0 = r2 }

l000157D4:
	{ if (p0.new) r3 = memw(r26+8); if (p0.new) r2 = add(r1,r2); if (p0.new) jump:nt 000157F4; p0 = cmp.eq(r13,00000002) }

l000157E0:
	{ if (p0.new) r0 = add(r1,00000000); r19 = 00000000; if (!p0.new) jump:nt 00015800; p0 = cmp.eq(r13,00000001) }

l000157EC:
	{ jump 000157F8; r0 += add(r2,FFFFFFFF) }

l000157F4:
	{ r0 = sub(r2,r3) }

l000157F8:
	{ call fn00009760 }
	{ r19 = r0 }

l00015800:
	{ if (p0.new) r1 = add(r22,00000000); r2 = add(r22,r20); if (p0.new) jump:nt 00015830; p0 = cmp.eq(r13,00000002) }

l0001580C:
	{ if (!p0) r1:r0 = combine(r22,r22); if (p0.new) jump:nt 00015828; p0 = cmp.eq(r13,00000001) }

l00015814:
	{ r0 = memd(r29+28); r21 = 00000000 }
	{ if (!p0) r1:r0 = combine(r22,r20); if (!p0.new) jump:nt 0001583C; p0 = r0 }

l00015824:
	{ jump 00015834 }

l00015828:
	{ jump 00015834; r0 += add(r20,FFFFFFFF) }

l00015830:
	{ r0 = sub(r2,r18) }

l00015834:
	{ call fn00009760 }
	{ r21 = r0 }

l0001583C:
	{ memw(r29+32) = r17; r3:r2 = combine(00000000,00000000); r5 = add(r29,00000038); r4 = add(r29,00000020) }
	{ memw(r4+4) = FFFFFF81; r22 = add(r5,00000008); r20 = add(r4,00000008) }
	{ memd(r29+56) = r3:r2; memd(r29+64) = r3:r2; r1:r0 = combine(00000000,r20) }
	{ memw(r4+20) = 00000000; memd(r29+72) = r3:r2 }
	{ memw(r4+12) = 00000000; memw(r4+16) = 00000000 }
	{ memw(r29+56) = r17; memw(r4+8) = 00000000; call fn00009740 }
	{ r1:r0 = combine(00000000,r22); call fn00009740 }
	{ r1 = 00000137; r3:r2 = combine(00000002,r16); r4 = add(PC,00011DBD) }
	{ memw(r29) = r17; call logmsg_function }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 000158AC; r2 = memw(r26) }

l00015898:
	{ if (!cmp.eq(r2.new,00000001)) jump:t 000158B0 }

l000158A0:
	{ if (!cmp.eq(r2.new,00000001)) jump:t 000158B0 }

l000158A8:
	{ if (cmp.eq(r2.new,00000001)) jump:t 000158CC }

l000158AC:
	{ r1 = 0000013C; r3 = add(PC,00011D9F) }

l000158B0:
	{ r1 = 0000013C; r3 = add(PC,0000001F) }

l000158B8:
	{ r2 = r16; call errlog_function }

l000158BC:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 000159CC }
000158C8                         02 57 18 ED                     .W..    

l000158CC:
	{ r4 = memw(r27+20); r1 = 0000013E }
	{ r2 = mpyi(r2,r19) }
	{ if (!cmp.gtu(r3.new,r4)) jump:t 000158EC; r3 = mpyi(r2,r21) }

l000158E4:
	{ jump 000158BC; r3 = add(PC,00000003) }

l000158EC:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 00015904; r2 = memb(r17+32); r1 = 0000013F }

l000158FC:
	{ jump 000158BC; r3 = add(PC,00000039) }

l00015904:
	{ memw(r27+4) = r21; r18 = memw(r29+8); r2 = add(r29,00000020) }
	{ memw(r27+12) = r24; memw(r27) = r23; r1:r0 = combine(r18,r16) }
	{ memw(r27+24) = r3; memw(r27+8) = r19 }
	{ call nn_os_work_for_vector }
	{ r0 = r16; r1 = add(r29,00000038); callr r18 }
	{ r0 = r20; call fn000096A0 }
	{ r4 = memd(r29+16); r5 = memd(r29+12) }
	{ r2 = memw(r5) }
	{ memb(r4+1) = r3.new; r3 = memw(r5+4) }
	{ r2 = memw(r5+8) }
	{ memw(r4+8) = r2 }
	{ r7 = memw(r5+12) }
	{ memw(r4+12) = r7 }
	{ r2 = memw(r5+24) }
	{ if (cmp.gtu(r2,r6.new)) jump:t 00015974; r6 = memw(r4+20) }

l0001596C:
	{ r1 = memw(r5+16); r2 = memw(r5+24); call fn00009560 }

l00015974:
	{ r5 = memd(r29+20); r4 = memd(r29+24) }
	{ r2 = memw(r4) }
	{ memb(r5+1) = r3.new; r3 = memw(r4+4) }
	{ r2 = memw(r4+8) }
	{ memw(r5+8) = r2 }
	{ r7 = memw(r4+12) }
	{ memw(r5+12) = r7 }
	{ r2 = memw(r4+24) }
	{ if (cmp.gtu(r2,r6.new)) jump:t 000159B0; r6 = memw(r5+20) }

l000159A8:
	{ r1 = memw(r4+16); r2 = memw(r4+24); call fn00009560 }

l000159B0:
	{ r1 = 0000014B; r3:r2 = combine(00000002,r16); r4 = add(PC,00011CD3) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l000159CC:
	{ r19:r18 = memd(r29+120); r17:r16 = memd(r29+128) }
	{ r23:r22 = memd(r29+104); r21:r20 = memd(r29+112) }
	{ r27:r26 = memd(r29+88); r25:r24 = memd(r29+96) }
	{ dealloc_return }

;; maxpool_execute_slice_ref: 000159E0
maxpool_execute_slice_ref proc
	{ allocframe(+00000098) }
	{ r3 = memw(r1+4); r2 = memw(r1) }
	{ memd(r29+144) = r17:r16; memd(r29+136) = r19:r18 }
	{ r18 = memb(r2+32); r7 = memw(r2+4) }
	{ memw(r29+16) = r3; r2 = memw(r2+8) }
	{ r4 = memw(r7+16); r16 = memw(r7); p0 = cmp.eq(r18,00000000) }
	{ memd(r29+120) = r23:r22; r3 = memw(r7+12) }
	{ r5 = memw(r16+8) }
	{ memd(r29+128) = r21:r20; memd(r29+112) = r25:r24 }
	{ memd(r29+104) = r27:r26; r21 = r5; r0 = r5 }
	{ memw(r29) = r1; r22 = memw(r4+4); r1 = p0 }
	{ r17 = memw(r4+8); r19 = memw(r2) }
	{ r23 = memw(r16+4); r24 = memw(r16+12); if (!p0) r2 = add(r17,r21) }
	{ r26 = memw(r3+8); r25 = memw(r3+4) }
	{ memb(r29+3) = r7.new; r7 = memw(r16) }
	{ if (p0) jump:nt 00015A64 }

l00015A4C:
	{ if (p0.new) r0 = sub(r2,r26); if (p0.new) jump:nt 00015A64; p0 = cmp.eq(r10,00000002) }

l00015A54:
	{ if (p0.new) r0 = add(r17,00000000); r20 = 00000000; if (!p0.new) jump:nt 00015A70; p0 = cmp.eq(r10,00000001) }

l00015A60:
	{ r0 += add(r21,FFFFFFFF) }

l00015A64:
	{ r1 = r17; call fn00009760 }
	{ r20 = r0 }

l00015A70:
	{ if (p0.new) r1 = add(r22,00000000); r2 = add(r22,r23); if (p0.new) jump:nt 00015AA8; p0 = cmp.eq(r10,00000002) }

l00015A7C:
	{ if (!p0) r1:r0 = combine(r22,r22); if (p0.new) jump:nt 00015AA0; p0 = cmp.eq(r10,00000001) }

l00015A84:
	{ memb(r29+17) = r2.new; r0 = memw(r29+96); r2 = 00000000 }
	{ if (!p0) r1:r0 = combine(r22,r23); if (!p0.new) jump:nt 00015AB8 }

l00015A9C:
	{ jump 00015AAC }

l00015AA0:
	{ jump 00015AAC; r0 += add(r23,FFFFFFFF) }

l00015AA8:
	{ r0 = sub(r2,r25) }

l00015AAC:
	{ call fn00009760 }
	{ memw(r29+68) = r0 }
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 00015D28; r2 = memw(r29+12); r3 = sub(r25,r23) }

l00015AB8:
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 00015D2C; r2 = memw(r29+12) }

l00015AC4:
	{ r2 = memd(r29+68); r6 = r20; r4 = sub(r26,r21) }
	{ r5 = memd(r29+16); r2 = r2; r4 = sub(FFFFFFFF,r21); r6 = add(r4,mpyi(r6,r17)) }
	{ r3 = 00000000; r1 = mpyi(r24,r21); r2 = add(r3,mpyi(r2,r22)) }
	{ memw(r29+52) = r8; r8 = sub(FFFFFFFF,r23); r6 += lsr(r6,0000001F); r7 = mpyi(r5,r22) }
	{ r15 = asr(r6,00000001); r2 += lsr(r2,0000001F) }
	{ memw(r29+96) = r4; memw(r29+48) = r3; r4 = r21; r3 = 00000000 }
	{ r9 = memw(r19+16); r2 = asr(r2,00000001); r5 = mpyi(r21,r24) }
	{ memw(r29+64) = r1; memw(r29+32) = r8; r8 = r26; r1 = sub(r7,r2) }
	{ r12 = memw(r16+16); r6 = add(r15,sub(0000007F,r26)) }
	{ memw(r29+92) = r9; memw(r29+72) = r2; r9 = 00000000; r2 = add(r2,sub(0000007F,r7)) }
	{ memw(r29+44) = r23; memw(r29+8) = r1; r2 = sub(r2,r25); r1 = sub(00000000,r15) }
	{ memw(r29+20) = r9; memw(r29+36) = r25 }
	{ memw(r29+24) = r1; memw(r29+28) = r6 }
	{ memw(r29+4) = r2 }

l00015B58:
	{ r2 = memw(r29+16) }
	{ if (!cmp.gt(r6.new,r2)) jump:t 00015D0C; r6 = memw(r29+68) }

l00015B68:
	{ memw(r29+88) = r6; r1 = memd(r29+8); r2 = combine(r7.l,r22.l) }
	{ r13 = memw(r29+16); r6 = memw(r29+68) }
	{ memw(r29+84) = r1; r9 = memw(r29+20); r1:r0 = combine(r7,r2) }
	{ memb(r29+10) = r6.new; r6 = mpyi(r9,r6) }
	{ memd(r29+56) = r1:r0 }

l00015B94:
	{ r6 = memd(r29+84); r2 = memd(r29+52) }
	{ r2 = memd(r29+88); r22 = memd(r29+76); r6 = add(r6,r2) }
	{ r7 = memw(r29+68); r13 = memw(r29+80); r2 -= asl(r22,00000001) }
	{ memw(r29+88) = r2; memw(r29+84) = r6; p0 = cmp.gt(r7,r13) }
	{ if (!p0) jump:nt 00015D0C }

l00015BB8:
	{ r2 = r13 }
	{ r1:r0 = memd(r29+56); r7 = memd(r29+72); r6 = add(r2,00000002) }
	{ memw(r29+80) = r6; r6 = mpyi(r6,r22) }
	{ r7 = memw(r29+64); r6 = sub(r6,r7) }
	{ r6 = add(r12,mpyi(r6,r7)) }
	{ l2fetch(r6,r1:r0) }
	{ if (p0.new) r9 = memw(r29+40); if (p0.new) r13 = memw(r29+88); if (!p0.new) jump:nt 00015B94; p0 = cmp.gt(r12,00000000) }

l00015BE8:
	{ r7 = memd(r29+84); r6 = memd(r29+76) }
	{ r7 = memw(r29+32); r2 = add(r2,r9); r0 = max(r7,r3); r6 = mpyi(r2,r6) }
	{ r2 = memw(r29+48); r14 = memw(r29+72); r19 = mpyi(r2,r20) }
	{ r7 = memd(r29+36); r2 = add(r2,r0); r6 = sub(r6,r14); r9 = max(r13,r7) }
	{ r0 = memd(r29+44); r6 = add(r6,r7); r16 = mpyi(r4,r2); r25 = max(r6,r3) }
	{ r1 = memd(r29+28); r23 = memd(r29+24); r9 = sub(FFFFFFFF,r9); r13 = 00000000 }
	{ r2 = min(r0,r6) }
	{ p0 = cmp.gt(r24,00000000); r0 = add(r13,r19); if (!p0.new) jump:nt 00015CF8; r7 = mpyi(r13,r17) }

l00015C40:
	{ r10 = memw(r29+92); r6 = memw(r29+96); r21 = r24; r14 = max(r23,r3) }
	{ r18 = add(r16,r14); r7 = sub(r7,r15) }
	{ r6 = 00000000; r0 = add(r10,mpyi(r0,r24)); r28 = max(r1,r6) }
	{ r7 = add(r7,r8); r28 = sub(FFFFFFFF,r28); r21 = add(r12,mpyi(r21,r18)); r10 = max(r7,r3) }
	{ r11 = sub(r28,r14); r14 = min(r4,r7) }

l00015C7C:
	{ p0 = cmp.gt(r2,r25); r18 = 00000000; r28 = sub(r9,r25); if (!p0.new) jump:nt 00015CE4 }

l00015C8C:
	{ r7 = r21; r18 = 00000000; loop1(00015C94,r28) }
	{ p0 = cmp.gt(r14,r10); r22 = and(r18,000000FF); r26 = add(r11,FFFFFFFF); if (!p0.new) jump:nt 00015CD8 }

l00015CA4:
	{ r28 = memb(r7); p1 = cmp.gtu(r11,00000001); r27 = add(r7,r24) }
	{ p0 = cmp.gtu(r28,r22); loop0(00015CBC,r26) }
	{ if (!p1) jump:nt 00015CD4 }

l00015CBC:
	{ r28 = memb(r27); r27 = add(r27,r24); if (p0) r18 = add(r28,00000000) }
	{ r22 = and(r18,000000FF) }
	{ nop; p0 = cmp.gtu(r28,r22) }

l00015CD4:
	{ if (p0) r18 = add(r28,00000000) }

l00015CD8:
	{ nop; nop; r7 = add(r7,r5) }

l00015CE4:
	{ memb(r0++#1) = r18; r21 = add(r21,00000001); r6 = add(r6,00000001) }
	{ p0 = cmp.eq(r6,r24); if (!p0.new) jump:nt 00015C7C }

l00015CF8:
	{ if (cmp.eq(r13.new,r20)) jump:nt 00015B94; r13 = add(r13,00000001); r1 = sub(r1,r17); r23 = add(r23,r17) }

l00015D0C:
	{ r1 = memd(r29+12); r7 = memd(r29+20) }
	{ r6 = memd(r29+48); r2 = memd(r29+44); r7 = add(r7,00000001) }
	{ memw(r29+20) = r7; r6 = add(r6,r2); p0 = cmp.eq(r7,r1) }
	{ memw(r29+48) = r6; if (!p0) jump:nt 00015B58 }

l00015D28:
	{ r17:r16 = memd(r29+144); r2 = memd(r29); r1 = 00000001 }

l00015D2C:
	{ r17:r16 = memd(r29+144); r2 = memd(r29) }

l00015D30:
	{ r21:r20 = memd(r29+128); r19:r18 = memd(r29+136); r0 = add(r2,00000008) }
	{ r25:r24 = memd(r29+112); r23:r22 = memd(r29+120) }
	{ deallocframe; r27:r26 = memd(r29+104); jump 00009730 }

;; logmsg_function: 00015D4C
;;   Called from:
;;     00015620 (in maxpool_check)
;;     0001567C (in maxpool_check)
;;     000156E0 (in maxpool_check)
;;     0001573C (in maxpool_check)
;;     00015884 (in maxpool_execute)
;;     000159C0 (in maxpool_execute)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00015D6C; r5 = memw(r2+16) }

l00015D5C:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000003C) }
	{ memw(r29+4) = r6; call logv }

l00015D6C:
	{ dealloc_return }

;; errlog_function: 00015D70
;;   Called from:
;;     00015640 (in maxpool_check)
;;     000158B8 (in maxpool_execute)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000118A4) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; maxpool_execute_slice_asm: 00015D94
maxpool_execute_slice_asm proc
	{ allocframe(+00000088) }
	{ r3 = memw(r1+4); r2 = memw(r1) }
	{ memd(r29+96) = r25:r24; memd(r29+120) = r19:r18 }
	{ r19 = memb(r2+32); r7 = memw(r2+4) }
	{ memw(r29+20) = r3; r2 = memw(r2+8) }
	{ r4 = memw(r7+16); r24 = memw(r7); p0 = cmp.eq(r19,00000000) }
	{ memd(r29+128) = r17:r16; r3 = memw(r7+12) }
	{ r17 = memw(r24+8) }
	{ memd(r29+88) = r27:r26; memd(r29+112) = r21:r20 }
	{ r2 = memw(r4+4); r16 = memw(r2); r0 = r17 }
	{ r20 = memw(r24+12); r18 = memw(r4+8) }
	{ r27 = memw(r3+8); r21 = memw(r24+4) }
	{ memw(r29+4) = r1; r7 = memw(r24); r1 = p0 }
	{ memw(r29+72) = r2; r2 = memw(r3+4) }
	{ memw(r29+16) = r7; memd(r29+104) = r23:r22 }
	{ memw(r29+80) = r1; memw(r29+40) = r2; if (!p0) r2 = add(r18,r17) }
	{ if (p0) jump:nt 00015E18 }

l00015E04:
	{ if (p0.new) r0 = sub(r2,r27); if (p0.new) jump:nt 00015E18; p0 = cmp.eq(r11,00000002) }

l00015E0C:
	{ r0 = r18; r22 = 00000000; if (!p0.new) jump:nt 00015E24; p0 = cmp.eq(r11,00000001) }

l00015E14:
	{ r0 += add(r17,FFFFFFFF) }

l00015E18:
	{ r1 = r18; call fn00009760 }
	{ r22 = r0 }

l00015E24:
	{ nop; if (p0.new) jump:nt 00015E54; p0 = cmp.eq(r11,00000002) }

l00015E2C:
	{ if (p0.new) r1 = memw(r29+72); if (p0.new) jump:nt 00015E48; p0 = cmp.eq(r11,00000001) }

l00015E34:
	{ r1 = memd(r29+80); r0 = 00000000 }
	{ if (!p0.new) jump:nt 00015E64; p0 = r1 }

l00015E40:
	{ r1 = memw(r29+72); jump 00015E60; r0 = r13 }

l00015E48:
	{ r0 = r1 }
	{ jump 00015E60; r0 += add(r21,FFFFFFFF) }

l00015E54:
	{ r3 = memd(r29+40); r1 = memd(r29+72) }
	{ r2 = add(r1,r21) }
	{ r0 = sub(r2,r3) }

l00015E60:
	{ call fn00009760 }

l00015E64:
	{ if (!cmp.gt(r2.new,00000000)) jump:nt 0001600C; r2 = memw(r29+16); r3 = sub(r27,r17); r4 = add(r0,FFFFFFFF) }

l00015E78:
	{ r1 = memd(r29+72); r5 = memd(r29+40); r2 = add(r22,FFFFFFFF) }
	{ r26 = and(r20,0000007F); r5 = sub(r5,r21); r7 = mpyi(r20,r17); r2 = add(r3,mpyi(r2,r18)) }
	{ r3 = memw(r16+16); r8 = 00000000; r5 = mpyi(r0,r22); r4 = add(r5,mpyi(r4,r1)) }
	{ memb(r29+11) = r1.new; r23 = memw(r24+16); r1 = asl(r6,00000001); r2 += lsr(r2,0000001F) }
	{ memw(r29+36) = r21; r1 = memd(r29+20); r2 = asr(r2,00000001) }
	{ memb(r29+3) = r5.new; r2 = sub(00000000,r2); r6 = add(r3,mpyi(r6,r1)); r5 = mpyi(r5,r20) }
	{ memw(r29+56) = r7; memw(r29+68) = r22 }
	{ memw(r29+8) = r5; memw(r29+60) = r0; r3 = asr(r4,00000001) }
	{ memw(r29+28) = r6; memw(r29+24) = r8 }
	{ memw(r29+32) = r2; memw(r29+64) = r3 }

l00015EE8:
	{ r2 = memw(r29+20) }
	{ if (!cmp.gt(r3.new,r2)) jump:t 00015FF0; r3 = memw(r29+60) }

l00015EF8:
	{ r5 = memd(r29+8); r7 = memd(r29+24); r2 = combine(r3.l,r2.l) }
	{ memb(r29+20) = r4.new; r4 = memw(r29+28); r1:r0 = combine(r3,r2); r19 = mpyi(r5,r7) }
	{ jump 00015F30 }

l00015F18:
	{ r7 = memd(r29+60); r6 = memd(r29+76) }
	{ r3 = memd(r29+80); r2 = memd(r29+44); p0 = cmp.gt(r7,r6) }
	{ r3 = add(r3,r2) }
	{ memw(r29+80) = r3; if (!p0) jump:nt 00015FF0 }

l00015F30:
	{ r2 = r6 }
	{ r7 = memd(r29+64); r3 = memd(r29+72); r4 = add(r2,00000002) }
	{ memw(r29+76) = r4; r1:r0 = memd(r29+48); r3 = mpyi(r4,r3) }
	{ r4 = memw(r29+56); r3 = sub(r3,r7) }
	{ r3 = add(r23,mpyi(r3,r4)) }
	{ l2fetch(r3,r1:r0) }
	{ if (!cmp.gt(r3.new,00000000)) jump:nt 00015F18; r3 = memw(r29+68) }

l00015F60:
	{ r16 = memd(r29+68); r4 = memd(r29+40) }
	{ r22 = memw(r29+32); r24 = memw(r29+80); r3 = 00000000; r2 = mpyi(r2,r3) }
	{ r7 = memw(r29+36); r2 = sub(r2,r7) }
	{ r2 = add(r2,r4); r3 = max(r2,r3) }
	{ r21 = mpyi(r3,r17); r2 = min(r7,r2) }
	{ r25 = sub(r2,r3) }
	{ r1 = r23; r2 = 00000000; p0 = cmp.eq(r26,00000000); r3 = add(r27,r22) }
	{ if (!p0) r0 = add(r24,00000000); r3 = min(r17,r3); r2 = max(r22,r2) }
	{ if (p0) r0 = add(r24,00000000); if (!p0) r2 = add(r20,00000000); r3 = sub(r3,r2); r4 = add(r2,r21) }
	{ if (p0) r2 = add(r20,00000000); if (p0.new) r5:r4 = combine(r17,r25); r7 = mpyi(r4,r20) }
	{ if (p0) r5:r4 = combine(r17,r25); if (!p0) jump:nt 00015FD8; r1 += add(r7,r19) }

l00015FD0:
	{ call maxpool_aligned_hvx }
	{ jump 00015FDC }

l00015FD8:
	{ call maxpool_nonaligned_hvx }

l00015FDC:
	{ if (cmp.eq(r16.new,00000000)) jump:nt 00015F18; r16 = add(r16,FFFFFFFF); r22 = add(r22,r18); r24 = add(r24,r20) }

l00015FF0:
	{ r7 = memd(r29+16); r3 = memd(r29+24) }
	{ r4 = memd(r29+12); r2 = memd(r29+28); r3 = add(r3,00000001) }
	{ memw(r29+24) = r3; r2 = add(r2,r4); p0 = cmp.eq(r3,r7) }
	{ memw(r29+28) = r2; if (!p0) jump:nt 00015EE8 }

l0001600C:
	{ r17:r16 = memd(r29+128); r2 = memd(r29+4); r1 = 00000001 }
	{ r21:r20 = memd(r29+112); r19:r18 = memd(r29+120); r0 = add(r2,00000008) }
	{ r25:r24 = memd(r29+96); r23:r22 = memd(r29+104) }
	{ deallocframe; r27:r26 = memd(r29+88); jump 00009730 }
00016030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; min_execute: 00016040
min_execute proc
	{ r3 = 7F800000; r2 = add(PC,000150D0) }
	{ r2 = memw(r2-144); jump nn_reduction_float }
0001605C                                     00 C0 00 7F             ....

;; minmax_check: 00016060
minmax_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0001182A) }
	{ r17 = r0; r16 = r1; r1 = 00000075 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,000117FA) }
	{ call logmsg_function }
	{ r1 = 00000076; r3 = add(PC,0001181F) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000160FC; r2 = memw(r17+4) }

l0001609C:
	{ r1 = 00000077; r3 = add(PC,00000017) }
	{ if (cmp.eq(r4.new,00000000)) jump:nt 000160FC; r4 = memw(r17+8) }

l000160B0:
	{ r1 = 00000078; r3 = add(PC,00000010) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000160FC; r2 = memw(r2) }

l000160C4:
	{ r1 = 00000079; r3 = add(PC,00000009) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 000160FC; r2 = memw(r4) }

l000160D8:
	{ r1 = 0000007A; r3 = add(PC,00000003) }
	{ if (cmp.gtu(r2.new,00000003)) jump:t 000160FC; r2 = memw(r17+16) }

l000160EC:
	{ r1 = 0000007B; r3 = add(PC,0000002F) }
	{ if (cmp.eq(r2.new,00000001)) jump:t 00016114; r2 = memw(r17+20) }

l000160FC:
	{ r2 = r16; call errlog_function; r0 = add(PC,00011772) }

l00016100:
	{ r2 = r16; call fn00016564; r0 = add(PC,00000032) }

l0001610C:
	{ r17:r16 = memd(r29+8); r0 = -00000001 }
	{ dealloc_return }

l00016114:
	{ r2 = r16; r1 = 0000007C; r0 = add(PC,0001175A) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,000117C2) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
0001613C                                     00 C0 00 7F             ....

;; max_execute: 00016140
max_execute proc
	{ r3 = FF800000; r2 = add(PC,00014FD0) }
	{ r2 = memw(r2-128); jump nn_reduction_float }
0001615C                                     00 C0 00 7F             ....

;; minimum_execute: 00016160
minimum_execute proc
	{ r2 = add(PC,00014FB0) }
	{ r2 = memw(r2-144); jump broadcast_elementwise_execute_f }

;; maximum_execute: 00016174
maximum_execute proc
	{ r2 = add(PC,00014F9C) }
	{ r2 = memw(r2-128); jump broadcast_elementwise_execute_f }

;; broadcast_elementwise_execute_f: 00016188
;;   Called from:
;;     00016168 (in minimum_execute)
;;     0001617C (in maximum_execute)
broadcast_elementwise_execute_f proc
	{ allocframe(+000000A8); memd(r29-48) = r25:r24; r25:r24 = combine(r1,r0) }
	{ memd(r29+136) = r23:r22; r3 = memw(r24+4) }
	{ memd(r29+152) = r19:r18; r18 = r2; r2 = 00000000 }
	{ memd(r29+144) = r21:r20 }
	{ memd(r29+160) = r17:r16; r23 = memw(r3) }
	{ r4 = memw(r24+8); r19 = memw(r3+4) }
	{ memd(r29+120) = r27:r26; r12 = memw(r23+4) }
	{ r7 = memw(r19); r0 = memw(r23); p0 = cmp.eq(r12,00000001) }
	{ r21 = memw(r23+8); r5 = memw(r19+4); p1 = cmp.eq(r0,00000001) }
	{ memw(r29+104) = r5; r9 = memw(r19+8); p2 = cmp.eq(r21,00000001); r6 = mux(p0,r5,r12) }
	{ r8 = memw(r23+12); r5 = mux(p1,r7,r0); r10 = p1 }
	{ memw(r29+72) = r7; r0 = memw(r19+12); p1 = cmp.eq(r8,00000001); r3 = mpyi(r5,r6) }
	{ r17 = memw(r4); r7 = mux(p2,r9,r21) }
	{ memw(r29+60) = r12; memw(r29+100) = r10; r10 = p2; r3 = mpyi(r3,r7) }
	{ memw(r29+88) = r6; memw(r29+68) = r5; r16 = mux(p1,r0,r8) }
	{ memw(r29+108) = r10; memw(r29+64) = r9; r3 = mpyi(r3,r16) }
	{ memw(r29+92) = r0; memw(r29+112) = r7 }
	{ memw(r29+84) = r2; r22 = asl(r3,00000002) }
	{ if (p0) jump:nt 00016238 }

l00016230:
	{ r2 = mpyi(r8,r21) }
	{ memw(r29+84) = r2 }

l00016238:
	{ r2 = r25; r1 = 000000BC; r0 = add(PC,00011551) }
	{ memw(r29+96) = r8; r27 = memw(r17+16); r4 = add(PC,00011560) }
	{ r26 = memw(r23+16); r20 = memw(r19+16) }
	{ memw(r29) = r24; call logmsg_function }
	{ if (!cmp.gtu(r22,r2.new)) jump:t 00016294; r2 = memw(r17+20); r1 = 000000BC }

l00016278:
	{ r2 = r25; r0 = add(PC,00000015) }
	{ r3 = add(PC,00011546) }

l00016288:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00016524 }

l00016294:
	{ r5 = memw(r23); r2 = memw(r19) }
	{ r7 = memw(r23+8); r8 = memw(r23+12); p0 = cmp.eq(r5,r2) }
	{ r12 = memw(r19+12); r6 = memw(r23+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+76) = r26; memw(r29+56) = r22 }
	{ memw(r29+80) = r20; if (p0) jump:nt 000162CC }

l000162C4:
	{ if (p0.new) jump:nt 000162CC; p0 = cmp.eq(r5,00000001) }

l000162C8:
	{ if (!p0.new) jump:nt 00016308; p0 = cmp.eq(r2,00000001) }

l000162CC:
	{ nop; if (p0.new) jump:nt 000162DC; p0 = cmp.eq(r6,r3) }

l000162D4:
	{ if (p0.new) jump:nt 000162DC; p0 = cmp.eq(r6,00000001) }

l000162D8:
	{ if (!p0.new) jump:nt 00016308; p0 = cmp.eq(r3,00000001) }

l000162DC:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 000162F0 }

l000162E4:
	{ if (p0.new) jump:nt 000162F0; p0 = cmp.eq(r7,00000001) }

l000162E8:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 00016308 }

l000162F0:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001633C }

l000162F8:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 0001633C }

l00016300:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 0001633C }

l00016308:
	{ memw(r29+28) = r12; r1 = 000000BC; r0 = add(PC,00011481) }
	{ memw(r29+12) = r8; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,000114B4) }
	{ memw(r29+4) = r6; memw(r29+16) = r2; r2 = r25 }
	{ memw(r29) = r5; jump 00016288 }

l0001633C:
	{ memw(r29+44) = r16; r23 = memd(r29+112); r0 = add(PC,0001144D) }
	{ r19 = memw(r29+68); r1 = 000000BC; r4 = add(PC,000114BC) }
	{ memw(r29+40) = r23; r20 = memd(r29+88) }
	{ memw(r29+32) = r19; memw(r29+36) = r20 }
	{ memw(r29+28) = r12; memw(r29+52) = r24 }
	{ memw(r29+24) = r9; memw(r29+48) = r25 }
	{ memw(r29+8) = r7; memw(r29+20) = r3 }
	{ memw(r29+4) = r6; memw(r29+16) = r2; r2 = r25 }
	{ memw(r29) = r5; memw(r29+12) = r8 }
	{ call logmsg_function }
	{ r4 = memw(r29+80); p0 = cmp.gt(r19,00000000) }
	{ r2 = memd(r29+56); r5 = memd(r29+76) }
	{ memw(r17) = r19; memw(r17+24) = r2 }
	{ memw(r17+8) = r23; memw(r17+4) = r20 }
	{ memw(r17+12) = r16; if (!p0) jump:nt 000164FC }

l000163A4:
	{ r9 = memw(r29+64); r2 = memw(r29+60); r1 = 00000000 }
	{ r0 = memd(r29+108); r6 = memd(r29+104); p0 = cmp.eq(r9,00000001); r2 = mpyi(r21,r2) }
	{ r8 = memw(r29+92); r7 = memw(r29+72); r3 = mpyi(r9,r6) }
	{ r0 = memw(r29+100); r13 = mux(p0,00000000,r8); p2 = r0; r9 = mpyi(r8,r9) }
	{ memw(r29+64) = r1; r7 = memd(r29+96); p1 = cmp.eq(r7,00000001); r3 = mpyi(r3,r8) }
	{ r25 = !cmp.eq(r7,00000001); r6 = 00000000; p2 = cmp.eq(r6,00000001); r12 = mux(p2,00000000,r7) }
	{ if (p2) r9 = add(r6,00000000); r26 = !cmp.eq(r8,00000001); p0 = r0; r2 = mpyi(r2,r7) }
	{ memw(r29+72) = r9; memw(r29+108) = r12; if (p0) r2 = add(r6,00000000); if (p1) r3 = add(r6,00000000) }
	{ memw(r29+60) = r2; memw(r29+104) = r13 }
	{ memw(r29+56) = r3 }

l00016420:
	{ memw(r29+76) = r5; r2 = memd(r29+88); r7:r6 = combine(00000000,r5); r3 = r4 }
	{ memw(r29+80) = r4; p0 = cmp.gt(r2,00000000) }
	{ if (!p0) jump:nt 000164DC }

l00016438:
	{ if (cmp.gt(r2.new,00000000)) jump:t 0001644C; r2 = memw(r29+112) }

l00016444:
	{ memw(r29+100) = r3; jump 000164BC }

l0001644C:
	{ r21 = r6; r23 = 00000000; r19 = r3 }
	{ memw(r29+96) = r6; memw(r29+92) = r7 }
	{ memw(r29+100) = r3 }

l0001645C:
	{ r22 = r16; r17 = r19; r24 = r27; r20 = r21 }
	{ if (!p0.new) jump:nt 000164A4; p0 = cmp.gt(r8,00000000) }

l0001646C:
	{ nop }
	{ nop; nop; nop; nop }

l00016480:
	{ r1 = memw(r17); r0 = memw(r20); callr r18; r22 = add(r22,FFFFFFFF) }
	{ memw(r24) = r0; r24 = add(r24,00000004); r17 = addasl(r17,r26,00000002); r20 = addasl(r20,r25,00000002) }
	{ if (!p0.new) jump:nt 00016480; p0 = cmp.eq(r14,00000000) }

l000164A0:
	{ r27 = addasl(r27,r16,00000002) }

l000164A4:
	{ r7 = memd(r29+104); r2 = memd(r29+108); r23 = add(r23,00000001) }
	{ r21 = addasl(r21,r2,00000002) }
	{ if (!cmp.eq(r2.new,r23)) jump:t 0001645C; r2 = memw(r29+112); r19 = addasl(r19,r7,00000002) }

l000164BC:
	{ r6 = memd(r29+96); r2 = memd(r29+84) }

l000164C0:
	{ r3 = memd(r29+100); r5 = memd(r29+72) }
	{ r2 = memd(r29+88); r7 = memd(r29+92); r6 = addasl(r6,r2,00000002) }
	{ r5 = memd(r29+76); r4 = memd(r29+80); r3 = addasl(r3,r5,00000002) }
	{ if (!cmp.eq(r7.new,r2)) jump:t 00016438; r7 = add(r7,00000001) }

l000164DC:
	{ r7 = memd(r29+56); r2 = memd(r29+60) }

l000164E0:
	{ r3 = memw(r29+64) }
	{ r2 = memd(r29+68); r3 = r3; r4 = addasl(r4,r7,00000002); r5 = addasl(r5,r2,00000002) }
	{ memw(r29+64) = r3; p0 = cmp.eq(r3,r2) }
	{ if (!p0) jump:nt 00016420 }

l000164FC:
	{ memb(r29) = r2.new; r2 = memw(r29+52); r0 = add(PC,0001128D) }
	{ r2 = memw(r29+48); r1 = 000000BC; r4 = add(PC,00000028) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00016524:
	{ r19:r18 = memd(r29+152); r17:r16 = memd(r29+160) }
	{ r23:r22 = memd(r29+136); r21:r20 = memd(r29+144) }
	{ r27:r26 = memd(r29+120); r25:r24 = memd(r29+128) }
	{ dealloc_return }

;; logmsg_function: 00016538
;;   Called from:
;;     00016080 (in minmax_check)
;;     00016124 (in minmax_check)
;;     00016260 (in broadcast_elementwise_execute_f)
;;     00016384 (in broadcast_elementwise_execute_f)
;;     0001651C (in broadcast_elementwise_execute_f)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00016558; r5 = memw(r2+16) }

l00016548:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l00016558:
	{ dealloc_return }
0001655C                                     00 C0 00 7F             ....

;; errlog_function: 00016560
;;   Called from:
;;     000160FC (in minmax_check)
;;     00016288 (in broadcast_elementwise_execute_f)
;;     00016758 (in nn_reduction_float)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }

;; fn00016564: 00016564
;;   Called from:
;;     00016100 (in minmax_check)
fn00016564 proc
	{ allocframe(00000008); r4 = r3 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }

;; nn_reduction_float: 0001657C
;;   Called from:
;;     00016050 (in min_execute)
;;     00016150 (in max_execute)
;;     00016568 (in errlog_function)
;;     00016568 (in fn00016564)
nn_reduction_float proc
	{ allocframe(00000070); r5 = 00000001 }
	{ r6 = memw(r0+4); r4 = memw(r0+8) }
	{ memd(r29+104) = r17:r16; r7 = memw(r0+16); r2 = r1; r16 = r2 }
	{ r4 = memw(r4); r8 = memw(r6) }
	{ memw(r29+24) = r3; memd(r29+88) = r21:r20; if (!p0.new) r13 = 00000001; p0 = cmp.eq(r7,00000003) }
	{ r28 = memw(r8+4); r15 = memw(r8+8); if (p0) r14 = 00000000 }
	{ r20 = memw(r8+12); r3 = memw(r8); if (p0) r11:r10 = combine(r15,r28) }
	{ r1 = memw(r4+16); r21 = memw(r8+16) }
	{ memd(r29+80) = r23:r22; memd(r29+96) = r19:r18; if (!p0) r18 = 00000001 }
	{ memd(r29+64) = r27:r26; memd(r29+72) = r25:r24; if (p0) r18 = add(r20,00000000) }
	{ if (p0) r9 = memw(r6+8); if (p0) jump:nt 000165EC }

l000165E0:
	{ r7:r6 = combine(00000001,00000001); r17 = 00000001; r11:r10 = combine(00000001,00000001) }
	{ jump 00016728 }

l000165EC:
	{ r7 = memw(r6+4); r17 = r3 }
	{ r6 = memw(r7+16); r8 = memw(r7+12) }
	{ r7 = memw(r9+16); p0 = cmp.eq(r8,00000000) }
	{ r7 = memw(r7); if (p0.new) r11:r10 = combine(r15,r28); if (!p0) r9 = add(r6,00000000); if (p0) jump:nt 00016668 }

l00016610:
	{ r18 = r20; r17 = r3; loop0(00016618,r8) }
	{ r12 = memw(r9++#4) }
	{ if (!cmp.gt(r12.new,00000003)) jump:t 00016630; r12 = add(r7,sub(0000007F,r12)) }

l00016628:
	{ r17 = 00000001; r11:r10 = combine(00000001,00000001) }

l00016630:
	{ if (p0.new) r18 = 00000001; p0 = cmp.eq(r12,00000000); if (p0.new) jump:t 0001665C }

l0001663C:
	{ if (p0.new) r11 = 00000001; p0 = cmp.eq(r12,00000001); if (p0.new) jump:t 0001665C }

l00016648:
	{ if (p0.new) r10 = 00000001; p0 = cmp.eq(r12,00000002); if (p0.new) jump:t 0001665C }

l00016654:
	{ if (p0.new) r17 = 00000001; p0 = cmp.eq(r12,00000003) }

l0001665C:
	{ nop; nop }
	{ r14 = r8 }

l00016668:
	{ if (cmp.eq(r8.new,00000002)) jump:t 00016680; r8 = memb(r0+32); r13:r12 = combine(r10,r17) }

l00016678:
	{ r13 = r10; r7:r6 = combine(r11,r18) }

l00016680:
	{ if (!p0.new) r13:r12 = combine(r10,r17); p0 = cmp.eq(r14,00000000); r9:r8 = combine(r11,r18); if (p0.new) jump:nt 000166E8 }

l00016690:
	{ r9:r8 = combine(r11,r18); loop0(00016698,r14) }
	{ r14 = memw(r6++#4) }
	{ if (!cmp.gt(r14.new,00000003)) jump:t 000166B4; r14 = add(r7,sub(0000007F,r14)) }

l000166A8:
	{ r12 = 00000000; r13 = 00000000; r9:r8 = combine(00000000,00000000) }

l000166B4:
	{ if (p0.new) r8 = 00000000; p0 = cmp.eq(r14,00000000); if (p0.new) jump:t 000166E0 }

l000166C0:
	{ if (p0.new) r9 = 00000000; p0 = cmp.eq(r14,00000001); if (p0.new) jump:t 000166E0 }

l000166CC:
	{ if (p0.new) r13 = 00000000; p0 = cmp.eq(r14,00000002); if (p0.new) jump:t 000166E0 }

l000166D8:
	{ if (p0.new) r12 = 00000000; p0 = cmp.eq(r14,00000003) }

l000166E0:
	{ nop; nop }

l000166E8:
	{ p3 = cmp.eq(r8,00000000); p0 = cmp.eq(r9,00000000); p2 = cmp.eq(r13,00000000); p1 = cmp.eq(r12,00000000) }
	{ r6 = mux(p1,00000001,r12); p1 = or(p1,p2) }
	{ r7 = mux(p1,00000001,r12); if (p2.new) r13 = 00000001; if (!p2) r6 = add(r13,00000000); p2 = or(p1,p0) }
	{ if (!p0) r6 = add(r9,00000000); if (!p2) r13 = add(r12,00000000); if (!p0) r7 = add(r6,00000000) }
	{ if (p3.new) r7:r6 = combine(r6,r8); if (!p3) r13 = add(r7,00000000); if (!p3) r5 = add(r13,00000000) }

l00016728:
	{ memw(r29+4) = r17; r9 = memw(r4+20); r8 = mpyi(r18,r11) }
	{ memw(r29+8) = r10; memw(r29+12) = r11; r8 = mpyi(r8,r10) }
	{ r8 = mpyi(r8,r17) }
	{ if (!cmp.gtu(r8.new,r9)) jump:t 0001676C; r8 = asl(r8,00000002) }

l00016750:
	{ r1 = 000000C6; r0 = add(PC,00000003) }
	{ call errlog_function; r3 = add(PC,0001106E) }
	{ r0 = FFFFFFFF; jump 000168E0 }

l0001676C:
	{ memw(r4+24) = r8; memw(r4+12) = r6; p0 = cmp.gt(r17,00000000); r0 = 00000000 }
	{ memw(r4) = r5; memw(r4+8) = r7; if (p0) r5 = 00000000 }
	{ memw(r4+4) = r13 }
	{ if (p0) memw(r29+32) = r28; if (p0) memw(r29+48) = r15; if (!p0) jump:nt 000168E0 }

l00016794:
	{ p0 = cmp.eq(r17,00000001); r4 = r18; p2 = cmp.eq(r10,00000001); p3 = cmp.eq(r11,00000001) }
	{ memw(r29+36) = r5; r23 = mux(p3,r15,00000001); r2 = mux(p0,r3,00000001); p1 = cmp.eq(r4,00000001) }
	{ memw(r29+40) = r2; memw(r29+20) = r4; r24 = mux(p1,r20,00000001); r3 = mux(p2,r28,00000001) }
	{ memw(r29+60) = r3 }

l000167C0:
	{ p0 = cmp.gt(r10,00000000); r2 = 00000000 }
	{ memw(r29+52) = r2; if (!p0) jump:nt 000168D0 }

l000167D0:
	{ p0 = cmp.gt(r11,00000000); r17 = 00000000; if (!p0.new) jump:nt 000168B8 }

l000167DC:
	{ memw(r29+16) = r1; r2 = r1; p0 = cmp.gt(r4,00000000); r19 = 00000000 }
	{ if (!p0) r1 = memw(r29+16); if (!p0) jump:nt 000168AC }

l000167F0:
	{ memw(r29+28) = r2; r2 = memd(r29+40) }
	{ r0 = memw(r29+24); p0 = cmp.gt(r2,00000000) }
	{ if (!p0) jump:nt 00016890 }

l00016800:
	{ r0 = memd(r29+24); r3 = 00000000 }

l00016804:
	{ if (cmp.gt(r2.new,00000000)) jump:t 00016818; r2 = memw(r29+60); r27 = 00000000 }

l00016814:
	{ memw(r29+44) = r3 }

l00016818:
	{ memw(r29+44) = r3; r2 = memd(r29+36) }
	{ r2 = add(r3,r2) }
	{ r3 = memw(r29+32) }
	{ memb(r29+14) = r2.new; r2 = mpyi(r2,r3) }

l0001682C:
	{ if (p0.new) r7 = memw(r29+48); if (!p0.new) jump:nt 00016878; p0 = cmp.gt(r15,00000000) }
	{ r3 = memd(r29+52); r2 = memd(r29+56); r22 = 00000000 }
	{ r2 += add(r27,r3) }
	{ r26 = mpyi(r2,r7) }

l00016844:
	{ p0 = cmp.gt(r24,00000000); r3:r2 = combine(r19,r26); r25 = r24; if (!p0.new) jump:nt 00016870 }

l00016848:
	{ p0 = cmp.gt(r24,00000000); r3:r2 = combine(r19,r26); r25 = r24 }

l00016854:
	{ r2 += add(r22,r17) }
	{ r3 += mpyi(r2,r20) }
	{ r18 = addasl(r21,r3,00000002) }

l00016860:
	{ r1 = memw(r18); r18 = add(r18,00000004); callr r16 }
	{ if (!cmp.eq(r25.new,00000000)) jump:t 00016860; r25 = add(r25,FFFFFFFF) }

l00016870:
	{ if (!cmp.eq(r22.new,r23)) jump:t 00016844; r22 = add(r22,00000001) }

l00016874:
	{ if (!cmp.eq(r22.new,r23)) jump:t 00016848 }

l0001687C:
	{ if (!cmp.eq(r27.new,r2)) jump:t 0001682C; r27 = add(r27,00000001) }

l00016888:
	{ if (!cmp.eq(r3.new,r2)) jump:t 00016804; r3 = add(r3,00000001) }

l00016890:
	{ r4 = memd(r29+20); r2 = memd(r29+28); r19 = add(r19,00000001) }

l00016894:
	{ r4 = memd(r29+20); r2 = memd(r29+28) }

l00016898:
	{ memw(r2) = r0; r2 = add(r2,00000004); p0 = cmp.eq(r19,r4) }
	{ if (p0) r1 = memw(r29+16); if (!p0) jump:nt 000167F0 }

l000168A8:
	{ r1 = addasl(r1,r4,00000002) }

l000168AC:
	{ r11 = memw(r29+12) }
	{ if (!cmp.eq(r17.new,r11)) jump:t 000167DC; r17 = add(r17,00000001) }

l000168B8:
	{ r10 = memw(r29+8); r2 = memw(r29+52) }

l000168BC:
	{ r10 = memw(r29+8) }

l000168C0:
	{ r2 = add(r2,00000001) }
	{ memw(r29+52) = r2; p0 = cmp.eq(r2,r10); if (!p0.new) jump:nt 000167D0 }

l000168D0:
	{ r2 = memd(r29+4); r3 = memd(r29+36) }
	{ r3 = add(r3,00000001) }
	{ memw(r29+36) = r3; r0 = -00000001; if (!p0.new) jump:nt 000167C0; p0 = cmp.eq(r3,r2) }

l000168E0:
	{ r19:r18 = memd(r29+96); r17:r16 = memd(r29+104) }
	{ r23:r22 = memd(r29+80); r21:r20 = memd(r29+88) }
	{ r27:r26 = memd(r29+64); r25:r24 = memd(r29+72) }
	{ dealloc_return }
000168F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; qsoftmax_execute_ref: 00016900
qsoftmax_execute_ref proc
	{ allocframe(+00000068); memd(r29-48) = r25:r24; r6 = 437F0000 }
	{ r24 = r0 }
	{ memd(r29+88) = r19:r18; r2 = memw(r24+4); r25 = r1 }
	{ r3 = memw(r24+8) }
	{ memd(r29+72) = r23:r22; memd(r29+96) = r17:r16 }
	{ r4 = memw(r2+4); r18 = memw(r2) }
	{ r5 = memw(r3+8); r2 = memw(r2+8) }
	{ r4 = memw(r4+16); r7 = memw(r18) }
	{ r17 = memw(r18+12); r2 = memw(r2+16) }
	{ r19 = memw(r3); r1 = memw(r3+4); r3 = mpyi(r7,r17) }
	{ r16 = memw(r4); r23 = memw(r18+4) }
	{ memd(r29+80) = r21:r20; r2 = memw(r2) }
	{ r20 = memw(r18+8) }
	{ memw(r29+28) = r1; r1 = r6; r0 = sfsub(r2,r16); r2 = mpyi(r3,r23) }
	{ memw(r29+36) = r24; memd(r29+56) = r27:r26 }
	{ memw(r29+48) = r7 }
	{ memw(r29+32) = r5; r26 = memw(r18+16) }
	{ r27 = memw(r19+16); call fn00009610; r22 = mpyi(r2,r20) }
	{ r2 = r25; r1 = 0000004A; r4 = add(PC,00010FF5) }
	{ memw(r29) = r24; r21 = r0; call logmsg_function }
	{ r5 = r19 }
	{ if (!cmp.gtu(r22,r2.new)) jump:t 000169BC; r2 = memw(r5+20) }

l000169A4:
	{ r2 = r25; r1 = 0000004B; r3 = add(PC,00000028) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00016C10 }

l000169BC:
	{ r2 = memd(r29+48); r3 = memw(r18) }
	{ memb(r5+1) = r4.new; r4 = memw(r18+4); r2 = mpyi(r23,r2) }
	{ memw(r5) = r3 }
	{ r6 = memw(r18+8); p0 = cmp.gt(r2,00000000) }
	{ memw(r29+24) = r25; memw(r5+8) = r6; if (p0) r20 = 7F800000 }
	{ r7 = memw(r18+12); if (p0) r18 = FF800000 }
	{ memw(r5+24) = r22; memw(r5+12) = r7; if (p0) r22 = 00000000 }
	{ if (p0) memw(r29+16) = r5; memw(r29+40) = r2; if (p0) jump:nt 00016A20 }

l00016A0C:
	{ r18 = FF800000; r20 = 7F800000 }
	{ jump 00016BC8 }

l00016A20:
	{ memb(r29+5) = r0.new; r0 = p0 }

l00016A28:
	{ if (p0.new) memw(r29+44) = r22; p0 = cmp.gt(r17,00000000) }
	{ memb(r29+12) = r0.new; if (p0) jump:nt 00016A58; r0 = p0 }

l00016A40:
	{ r0 = memw(r29+48) }
	{ if (p0.new) memw(r29+44) = r22; if (p0.new) r19 = 00000001; if (p0.new) jump:nt 00016A9C; p0 = r0 }

l00016A54:
	{ jump 00016B50 }

l00016A58:
	{ r25 = memb(r26); r19 = 00000001; r23 = r16 }
	{ r2 = r25; r3 = convert_w2sf(r25) }
	{ r23 += sfmpy(r21,r3) }

l00016A70:
	{ r1:r0 = combine(r23,r16); r2 = and(r2,000000FF) }
	{ r2 = convert_w2sf(r2) }
	{ call fn00009600; r0 += sfmpy(r21,r2) }
	{ r23 = r0; r2 = add(r26,r19); if (p0.new) jump:nt 00016A40; p0 = cmp.eq(r9,r11) }

l00016A90:
	{ r19 = add(r19,00000001) }
	{ r2 = memb(r2); jump 00016A70 }

l00016A9C:
	{ r24 = 00000000 }

l00016AA4:
	{ r3 = r16; r2 = and(r25,000000FF) }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r21,r2) }
	{ call fn00009780; r0 = sfsub(r3,r23) }
	{ r2 = add(r26,r19); if (p0.new) jump:nt 00016AD4; p0 = cmp.eq(r9,r11); r24 = sfadd(r24,r0) }

l00016AC8:
	{ r25 = memb(r2); r19 = add(r19,00000001) }
	{ jump 00016AA4 }

l00016AD4:
	{ memw(r29+44) = r22; r0 = 3F800000; r1 = r24 }
	{ call fn00009610 }
	{ r0 = memd(r29+48); r19 = 00000000; r24 = r0 }
	{ if (!p0.new) jump:nt 00016B50; p0 = r0 }

l00016AF4:
	{ r22 = 437F0000 }

l00016AFC:
	{ r2 = memb(r13+r19); r3 = r16 }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r21,r2) }
	{ call fn00009780; r0 = sfsub(r3,r23) }
	{ r25 = sfmpy(r24,r0) }
	{ r1:r0 = combine(r25,r18); call fn00009600 }
	{ r1:r0 = combine(r25,r20); r18 = r0; call fn000097B0 }
	{ r19 = r19; r20 = r0; r3 = add(r27,r19); r2 = sfmpy(r25,r22) }
	{ p0 = cmp.eq(r17,r19); r4 = convert_sf2uw(r2); p1 = sfcmp.gt(r2,r22) }
	{ if (p1) r4 = FFFFFFFF }
	{ memb(r3) = r4; if (!p0) jump:nt 00016AFC }

l00016B50:
	{ r2 = memd(r29+40); r22 = memd(r29+44); r27 = add(r27,r17); r26 = add(r26,r17) }
	{ if (!cmp.eq(r22.new,r2)) jump:t 00016A28; r22 = add(r22,00000001) }

l00016B68:
	{ if (!p0.new) jump:nt 00016BC8; p0 = r0 }

l00016B70:
	{ r2 = memd(r29+16); r0 = 3F800000; r1 = r18 }
	{ r16 = memw(r2+16); call fn00009610 }
	{ r3 = memd(r29+40); r2 = 3F000000 }
	{ r3 = memw(r29+48); loop1(00016B98,r3) }
	{ p0 = r3 }
	{ if (p0) r3 = add(r16,00000000); if (!p0) jump:nt 00016BBC }

l00016BA0:
	{ loop0(00016BA4,r17) }
	{ r4 = memb(r3); r5 = r20 }
	{ r4 = convert_w2sf(r4) }
	{ r5 += sfmpy(r0,r4) }
	{ r7 = sfadd(r5,r2) }
	{ memb(r3++#1) = r6.new; r6 = convert_sf2uw(r7) }

l00016BBC:
	{ nop; nop; r16 = add(r16,r17) }

l00016BC0:
	{ nop; nop }

l00016BC8:
	{ r6 = memd(r29+32); r7 = memd(r29+28); r4 = add(PC,00010DCE) }
	{ r5 = memd(r29+36); r2 = memd(r29+24); r1 = 00000085 }
	{ memw(r7+12) = 00000001; r0 = memw(r7+16) }
	{ memw(r7) = 00000001; memw(r7+8) = 00000001 }
	{ memw(r0) = r20; memw(r7+4) = FFFFFF81 }
	{ memw(r6+8) = 00000001; memw(r7+24) = 00000004 }
	{ memw(r6) = 00000001; r3 = memw(r6+16) }
	{ memw(r6+12) = 00000001; memw(r6+4) = 00000001 }
	{ memw(r6+24) = 00000004; memw(r3) = r18 }
	{ memw(r29) = r5; call logmsg_function }
	{ r0 = 00000000 }

l00016C10:
	{ r19:r18 = memd(r29+88); r17:r16 = memd(r29+96) }
	{ r23:r22 = memd(r29+72); r21:r20 = memd(r29+80) }
	{ r27:r26 = memd(r29+56); r25:r24 = memd(r29+64) }
	{ dealloc_return }

;; qsoftmax_check: 00016C24
qsoftmax_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00010CF8) }
	{ r17 = r0; r16 = r1; r1 = 0000008B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000003)) jump:t 00016C64; r2 = memw(r17+16); r1 = 0000008C }

l00016C50:
	{ r3 = add(PC,00000029) }
	{ r2 = r16; call errlog_function }

l00016C58:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00016C64:
	{ if (cmp.eq(r2.new,00000003)) jump:t 00016C7C; r2 = memw(r17+20); r1 = 0000008D }

l00016C74:
	{ jump 00016C58; r3 = add(PC,00000014) }

l00016C7C:
	{ r2 = r16; r1 = 0000008E; r4 = add(PC,00010CD8) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00016C9C
;;   Called from:
;;     00016988 (in qsoftmax_execute_ref)
;;     00016C04 (in qsoftmax_execute_ref)
;;     00016C38 (in qsoftmax_check)
;;     00016C8C (in qsoftmax_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00016CC0; r5 = memw(r2+16) }

l00016CAC:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000017) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00016CC0:
	{ dealloc_return }

;; errlog_function: 00016CC4
;;   Called from:
;;     000169B0 (in qsoftmax_execute_ref)
;;     00016C54 (in qsoftmax_check)
;;     00016CB4 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00010C3B) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00016CE8                         00 00 00 00 00 00 00 00         ........

;; nop_execute: 00016CF0
nop_execute proc
	{ allocframe(00000018); r2 = r1; r4 = add(PC,00010D31) }
	{ memd(r29+8) = r19:r18; memd(r29+16) = r17:r16; r16 = r0; r1 = 00000031 }
	{ memw(r29) = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00016D64; r2 = memw(r16+20) }

l00016D1C:
	{ r3 = memw(r16+4); r2 = memw(r16+8) }
	{ r2 = memw(r13+r17); r3 = memw(r21+r17) }
	{ r4 = memw(r3) }
	{ memb(r2+1) = r5.new; r5 = memw(r3+4) }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 00016D58; r6 = memw(r2+20) }

l00016D50:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l00016D58:
	{ r2 = memw(r16+20); r17 = add(r17,00000004) }
	{ if (cmp.gtu(r2,r18.new)) jump:t 00016D1C; r18 = add(r18,00000001) }

l00016D64:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16); r0 = 00000000 }

l00016D68:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }
	{ dealloc_return }

;; nop_check: 00016D70
nop_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00010C75) }
	{ r17 = r0; r16 = r1; r1 = 0000003B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+20) }
	{ if (cmp.eq(r3.new,r2)) jump:t 00016DA8; r3 = memw(r17+16) }

l00016D9C:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00016DA8:
	{ r2 = r16; r1 = 0000003D; r4 = add(PC,00010C64) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; nop_ctor: 00016DC4
nop_ctor proc
	{ allocframe(00000028); memd(r29+488) = r19:r18; r6 = add(PC,00010C0D) }
	{ r19:r18 = combine(r5,r0) }
	{ memd(r29+16) = r21:r20; memd(r29+32) = r17:r16; r21:r20 = combine(r2,r3); r17:r16 = combine(r4,r1) }
	{ r4 = r6; r2 = r18; r1 = 0000004B }
	{ memd(r29+8) = r23:r22; r22 = memd(r29+48) }
	{ r23 = memw(r29+52) }
	{ memw(r29) = r16; call logmsg_function }
	{ memw(r29+4) = r23; r1:r0 = combine(r16,r18); r3:r2 = combine(r20,r21); r5:r4 = combine(r19,r17) }
	{ memw(r29) = r22; call node_alloc_common }
	{ r17:r16 = memd(r29+32) }
	{ r21:r20 = memd(r29+16); r19:r18 = memd(r29+24) }
	{ dealloc_return; r23:r22 = memd(r29+8) }

;; nop_dtor: 00016E1C
nop_dtor proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00010BA4) }
	{ r17 = r0; r16 = r1; r1 = 00000059 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1:r0 = combine(r16,r17) }
	{ deallocframe; r17:r16 = memd(r29+8); jump node_free_common }

;; logmsg_function: 00016E44
;;   Called from:
;;     00016D08 (in nop_execute)
;;     00016D84 (in nop_check)
;;     00016DB4 (in nop_check)
;;     00016DF0 (in nop_ctor)
;;     00016E30 (in nop_dtor)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00016E68; r5 = memw(r2+16) }

l00016E54:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000017) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00016E68:
	{ dealloc_return }

l00016E6C:
	{ nop }

;; errlog_function: 00016E70
;;   Called from:
;;     00016E6C (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r3 = 00000000; r0 = add(PC,00010B37) }
	{ r5 = add(r29,00000010); r1 = 0000003C; r4 = add(PC,00010B7E) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; output_execute: 00016EA0
;;   Called from:
;;     00016E9C (in errlog_function)
output_execute proc
	{ allocframe(00000020); memd(r29+496) = r17:r16; r4 = add(PC,00010BF6) }
	{ r16 = r1; r1 = 00000034 }
	{ memd(r29+8) = r21:r20; memd(r29+16) = r19:r18; r18 = r0; r2 = r16 }
	{ memw(r29) = r18; call logmsg_function }
	{ r2 = memw(r16+32) }
	{ if (!cmp.eq(r3.new,r2)) jump:t 00016F04; r3 = memw(r18+16) }

l00016ED4:
	{ r2 = 00000000; r17 = 00000000 }
	{ r21:r20 = combine(00000000,00000000); r19 = 00000000 }

l00016EDC:
	{ r3 = memw(r16+24); r2 = memw(r18+4) }
	{ r2 = memw(r13+r20); r3 = add(r3,r21) }
	{ r4 = memw(r3+20) }
	{ if (!cmp.gtu(r5.new,r4)) jump:t 00016F1C; r5 = memw(r2+24) }

l00016EF8:
	{ memw(r29) = r19; r1 = 0000003A; r3 = add(PC,00000009) }
	{ jump 00016F10 }

l00016F04:
	{ r1 = 00000035; r3 = add(PC,00010BAB) }

l00016F10:
	{ r17 = -00000001; r2 = r16; call errlog_function }
	{ jump 00016F74 }

l00016F1C:
	{ r5 = memw(r2+4); r4 = memw(r2) }
	{ memw(r3) = r4; memw(r3+4) = r5 }
	{ r4 = memw(r2+8) }
	{ memw(r3+8) = r4 }
	{ r7 = memw(r2+12) }
	{ memw(r3+12) = r7 }
	{ r4 = memw(r2+24) }
	{ memw(r3+24) = r4 }
	{ r1 = memw(r2+16) }
	{ r2 = memw(r2+24); r0 = memw(r3+16); call fn00009560 }
	{ r2 = memw(r16); r21 = add(r21,00000020); r20 = add(r20,00000004) }
	{ if (cmp.gtu(r2,r19.new)) jump:t 00016EDC; r19 = add(r19,00000001) }

l00016F5C:
	{ memw(r29) = r2; r1 = 00000041; r4 = add(PC,00010B75) }
	{ r2 = r16; call logmsg_function }

l00016F74:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r17 }
	{ dealloc_return; r21:r20 = memd(r29+8) }

;; output_check: 00016F80
output_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00010AD3) }
	{ r17 = r0; r16 = r1; r1 = 00000048 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 00016FDC; r2 = memw(r17+16) }

l00016FA8:
	{ r3 = memw(r17+4) }
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 00016FDC; r4 = add(r4,00000001); r3 = add(r3,00000004) }

l00016FB0:
	{ if (!cmp.gtu(r2,r4.new)) jump:nt 00016FE0; r4 = add(r4,00000001) }

l00016FBC:
	{ if (!cmp.eq(r5.new,00000000)) jump:t 00016FB0 }

l00016FC4:
	{ r2 = r16; r1 = 0000004A; r3 = add(PC,0000002B) }
	{ call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00016FDC:
	{ r2 = r16; r1 = 0000004C; r4 = add(PC,00010AA2) }

l00016FE0:
	{ r2 = r16; r1 = 0000004C; r4 = add(PC,00000022) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00016FFC
;;   Called from:
;;     00016EBC (in output_execute)
;;     00016F6C (in output_execute)
;;     00016F94 (in output_check)
;;     00016FEC (in output_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00017020; r5 = memw(r2+16) }

l0001700C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000002F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00017020:
	{ dealloc_return }

;; errlog_function: 00017024
;;   Called from:
;;     00016F10 (in output_execute)
;;     00016FD0 (in output_check)
;;     00017014 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00010A13) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00017048                         00 00 00 00 00 00 00 00         ........

;; pprint_8_execute: 00017050
pprint_8_execute proc
	{ allocframe(+00000048); r4 = add(PC,00010AAF) }
	{ r5 = memw(r0+28); r2 = memw(r0+4) }
	{ memd(r29+56) = r19:r18; memd(r29+64) = r17:r16; r1 = 00000048; r16 = r1 }
	{ r7 = memw(r2); r3:r2 = combine(00000002,r16) }
	{ memd(r29+40) = r23:r22; memd(r29+48) = r21:r20 }
	{ memd(r29+32) = r25:r24; r17 = memw(r7+16) }
	{ r19 = memw(r7+8); r18 = memw(r7+12) }
	{ r21 = memw(r7); r20 = memw(r7+4) }
	{ memd(r29+24) = r27:r26; memw(r29+4) = r5 }
	{ memw(r29) = r0; call logmsg_function }
	{ memw(r29+12) = r18; r3:r2 = combine(00000001,r16); r4 = add(PC,00010A7F) }
	{ memw(r29) = r21; r1 = 00000048 }
	{ memw(r29+4) = r20; memw(r29+8) = r19 }
	{ call logmsg_function }
	{ r22 = 00000000; if (p0.new) jump:nt 0001712C; p0 = cmp.eq(r13,00000000) }

l000170C0:
	{ r23 = 00000000; if (p0.new) jump:nt 00017124; p0 = cmp.eq(r12,00000000) }

l000170C4:
	{ r23 = 00000000 }

l000170C8:
	{ r24 = 00000000; if (p0.new) jump:nt 0001711C; p0 = cmp.eq(r11,00000000) }

l000170CC:
	{ r24 = 00000000 }

l000170D0:
	{ r26 = 00000000; r25 = r17; if (p0.new) jump:nt 00017114; p0 = cmp.eq(r10,00000000) }

l000170DC:
	{ memb(r29+4) = r2.new; r2 = memb(r25++#1); r4 = add(PC,00010AA5) }
	{ memw(r29+4) = r23; r3:r2 = combine(00000001,r16) }
	{ memw(r29+8) = r24; memw(r29+12) = r26 }
	{ memw(r29) = r22; call logmsg_function }
	{ if (!cmp.eq(r26.new,r18)) jump:t 000170DC; r26 = add(r26,00000001) }

l00017114:
	{ if (!cmp.eq(r24.new,r19)) jump:t 000170D0; r24 = add(r24,00000001) }

l0001711C:
	{ if (!cmp.eq(r23.new,r20)) jump:t 000170C8; r23 = add(r23,00000001) }

l00017120:
	{ if (!cmp.eq(r23.new,r20)) jump:t 000170CC }

l00017124:
	{ if (!cmp.eq(r22.new,r21)) jump:t 000170C0; r22 = add(r22,00000001) }

l00017128:
	{ if (!cmp.eq(r22.new,r21)) jump:t 000170C4 }

l0001712C:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64); r0 = 00000000 }

l00017130:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }

l00017134:
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }

;; pprint_check: 00017144
pprint_check proc
	{ allocframe(00000000); r2 = r1 }
	{ if (cmp.eq(r3.new,00000001)) jump:t 00017164; r3 = memw(r0+16); r1 = 0000004E }

l00017158:
	{ r3 = add(PC,0000003E) }
	{ call errlog_function }

l00017160:
	{ dealloc_return; r0 = -00000001 }

l00017164:
	{ if (cmp.eq(r3.new,00000000)) jump:nt 0001717C; r3 = memw(r0+20); r1 = 0000004F }

l00017174:
	{ jump 00017160; r3 = add(PC,00000031) }

l0001717C:
	{ r1 = 00000050; r3 = 00000002; r4 = add(PC,000109F5) }
	{ call logmsg_function }
	{ dealloc_return; r0 = 00000000 }

;; pprint_32_execute: 00017194
pprint_32_execute proc
	{ allocframe(+00000048); r4 = add(PC,0001096B) }
	{ r5 = memw(r0+28); r2 = memw(r0+4) }
	{ memd(r29+56) = r19:r18; memd(r29+64) = r17:r16; r1 = 00000049; r16 = r1 }
	{ r7 = memw(r2); r3:r2 = combine(00000002,r16) }
	{ memd(r29+40) = r23:r22; memd(r29+48) = r21:r20 }
	{ memd(r29+32) = r25:r24; r17 = memw(r7+16) }
	{ r19 = memw(r7+8); r18 = memw(r7+12) }
	{ r21 = memw(r7); r20 = memw(r7+4) }
	{ memd(r29+24) = r27:r26; memw(r29+4) = r5 }
	{ memw(r29) = r0; call logmsg_function }
	{ memw(r29+12) = r18; r3:r2 = combine(00000001,r16); r4 = add(PC,0001093B) }
	{ memw(r29) = r21; r1 = 00000049 }
	{ memw(r29+4) = r20; memw(r29+8) = r19 }
	{ call logmsg_function }
	{ r22 = 00000000; if (p0.new) jump:nt 00017270; p0 = cmp.eq(r13,00000000) }

l00017204:
	{ r23 = 00000000; if (p0.new) jump:nt 00017268; p0 = cmp.eq(r12,00000000) }

l00017208:
	{ r23 = 00000000 }

l0001720C:
	{ r24 = 00000000; if (p0.new) jump:nt 00017260; p0 = cmp.eq(r11,00000000) }

l00017210:
	{ r24 = 00000000 }

l00017214:
	{ r26 = 00000000; r25 = r17; if (p0.new) jump:nt 00017258; p0 = cmp.eq(r10,00000000) }

l00017220:
	{ memb(r29+4) = r2.new; r2 = memw(r25++#4); r4 = add(PC,0001091C) }
	{ memw(r29+4) = r23; r3:r2 = combine(00000001,r16) }
	{ memw(r29+8) = r24; memw(r29+12) = r26 }
	{ memw(r29) = r22; call logmsg_function }
	{ if (!cmp.eq(r26.new,r18)) jump:t 00017220; r26 = add(r26,00000001) }

l00017258:
	{ if (!cmp.eq(r24.new,r19)) jump:t 00017214; r24 = add(r24,00000001) }

l00017260:
	{ if (!cmp.eq(r23.new,r20)) jump:t 0001720C; r23 = add(r23,00000001) }

l00017264:
	{ if (!cmp.eq(r23.new,r20)) jump:t 00017210 }

l00017268:
	{ if (!cmp.eq(r22.new,r21)) jump:t 00017204; r22 = add(r22,00000001) }

l0001726C:
	{ if (!cmp.eq(r22.new,r21)) jump:t 00017208 }

l00017270:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64); r0 = 00000000 }

l00017274:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }

l00017278:
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }
00017288                         00 40 00 7F 00 C0 00 7F         .@......

;; pprint_f_execute: 00017290
pprint_f_execute proc
	{ allocframe(+00000048); r4 = add(PC,0001086F) }
	{ r5 = memw(r0+28); r2 = memw(r0+4) }
	{ memd(r29+56) = r19:r18; memd(r29+64) = r17:r16; r1 = 0000004A; r16 = r1 }
	{ r7 = memw(r2); r3:r2 = combine(00000002,r16) }
	{ memd(r29+40) = r23:r22; memd(r29+48) = r21:r20 }
	{ memd(r29+32) = r25:r24; r17 = memw(r7+16) }
	{ r19 = memw(r7+8); r18 = memw(r7+12) }
	{ r21 = memw(r7); r20 = memw(r7+4) }
	{ memd(r29+24) = r27:r26; memw(r29+4) = r5 }
	{ memw(r29) = r0; call logmsg_function }
	{ memw(r29+12) = r18; r3:r2 = combine(00000001,r16); r4 = add(PC,0001083F) }
	{ memw(r29) = r21; r1 = 0000004A }
	{ memw(r29+4) = r20; memw(r29+8) = r19 }
	{ call logmsg_function }
	{ r22 = 00000000; if (p0.new) jump:nt 00017374; p0 = cmp.eq(r13,00000000) }

l00017300:
	{ r23 = 00000000; if (p0.new) jump:nt 0001736C; p0 = cmp.eq(r12,00000000) }

l00017304:
	{ r23 = 00000000 }

l00017308:
	{ r24 = 00000000; if (p0.new) jump:nt 00017364; p0 = cmp.eq(r11,00000000) }

l0001730C:
	{ r24 = 00000000 }

l00017310:
	{ r26 = 00000000; r25 = r17; if (p0.new) jump:nt 0001735C; p0 = cmp.eq(r10,00000000) }

l0001731C:
	{ r2 = memw(r25); r25 = add(r25,00000004); r4 = add(PC,0001080E) }
	{ memw(r29+4) = r23; memw(r29+8) = r24; r3:r2 = combine(00000001,r16); r1:r0 = convert_sf2df(r2) }
	{ memw(r29+12) = r26; memd(r29+16) = r1:r0; r1 = 0000004A }
	{ memw(r29) = r22; call logmsg_function }
	{ if (!cmp.eq(r26.new,r18)) jump:t 0001731C; r26 = add(r26,00000001) }

l0001735C:
	{ if (!cmp.eq(r24.new,r19)) jump:t 00017310; r24 = add(r24,00000001) }

l00017364:
	{ if (!cmp.eq(r23.new,r20)) jump:t 00017308; r23 = add(r23,00000001) }

l00017368:
	{ if (!cmp.eq(r23.new,r20)) jump:t 0001730C }

l0001736C:
	{ if (!cmp.eq(r22.new,r21)) jump:t 00017300; r22 = add(r22,00000001) }

l00017370:
	{ if (!cmp.eq(r22.new,r21)) jump:t 00017304 }

l00017374:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64); r0 = 00000000 }

l00017378:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }

l0001737C:
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }

;; logmsg_function: 0001738C
;;   Called from:
;;     00017090 (in pprint_8_execute)
;;     000170B4 (in pprint_8_execute)
;;     00017100 (in pprint_8_execute)
;;     0001718C (in pprint_check)
;;     000171D4 (in pprint_32_execute)
;;     000171F8 (in pprint_32_execute)
;;     00017244 (in pprint_32_execute)
;;     000172D0 (in pprint_f_execute)
;;     000172F4 (in pprint_f_execute)
;;     00017348 (in pprint_f_execute)
logmsg_function proc
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000173AC; r5 = memw(r2+16) }

l0001739C:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000000B) }
	{ memw(r29+4) = r6; call logv }

l000173AC:
	{ dealloc_return }

;; errlog_function: 000173B0
;;   Called from:
;;     0001715C (in pprint_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00010733) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
000173D4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; prefree_execute: 000173E0
prefree_execute proc
	{ r2 = r1; r1 = 00000031; r3 = add(PC,00010825) }
	{ allocframe(+00000000); call errlog_function }
	{ dealloc_return; r0 = -00000001 }
000173F8                         00 40 00 7F 00 C0 00 7F         .@......

;; prefree_check: 00017400
prefree_check proc
	{ r2 = r1; r1 = 00000036; r3 = add(PC,000107E1) }
	{ allocframe(+00000000); call errlog_function }
	{ dealloc_return; r0 = -00000001 }
00017418                         00 40 00 7F 00 C0 00 7F         .@......

;; prefree_ctor: 00017420
prefree_ctor proc
	{ allocframe(00000028); memd(r29+488) = r19:r18; r6 = add(PC,000107A9) }
	{ r19:r18 = combine(r5,r0) }
	{ memd(r29+16) = r21:r20; memd(r29+32) = r17:r16; r21:r20 = combine(r2,r3); r17:r16 = combine(r4,r1) }
	{ r4 = r6; r2 = r18; r1 = 00000043 }
	{ memd(r29+8) = r23:r22; r22 = memd(r29+48) }
	{ r23 = memw(r29+52) }
	{ memw(r29) = r16; call logmsg_function }
	{ memw(r29+4) = r23; r1:r0 = combine(r16,r18); r3:r2 = combine(r20,r21); r5:r4 = combine(r19,r17) }
	{ memw(r29) = r22; call node_alloc_common }
	{ r17:r16 = memd(r29+32) }
	{ r21:r20 = memd(r29+16); r19:r18 = memd(r29+24) }
	{ dealloc_return; r23:r22 = memd(r29+8) }
00017478                         00 40 00 7F 00 C0 00 7F         .@......

;; prefree_dtor: 00017480
prefree_dtor proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00010734) }
	{ r17 = r0; r16 = r1; r1 = 00000051 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1:r0 = combine(r16,r17) }
	{ deallocframe; r17:r16 = memd(r29+8); jump node_free_common }

;; logmsg_function: 000174A8
;;   Called from:
;;     0001744C (in prefree_ctor)
;;     00017494 (in prefree_dtor)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000174CC; r5 = memw(r2+16) }

l000174B8:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000023) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l000174CC:
	{ dealloc_return }

;; errlog_function: 000174D0
;;   Called from:
;;     000173EC (in prefree_execute)
;;     0001740C (in prefree_check)
;;     000174C0 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000106C7) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
000174F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; quantize_execute: 00017500
quantize_execute proc
	{ allocframe(+00000040); r4 = add(PC,00010805) }
	{ r5 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+16) = r27:r26; memd(r29+24) = r25:r24 }
	{ r7 = memw(r3+8); r6 = memw(r3) }
	{ memd(r29+48) = r19:r18; r3 = memw(r3+4) }
	{ r9 = memw(r6+4); r8 = memw(r6) }
	{ r13 = memw(r6+12); r12 = memw(r6+8) }
	{ memd(r29+56) = r17:r16; r16 = r1; r25 = convert_uw2sf(r8); r26 = convert_uw2sf(r9) }
	{ r19 = memw(r5); r3 = memw(r3+16); r18 = convert_uw2sf(r13); r27 = convert_uw2sf(r12) }
	{ r7 = memw(r7+16); r2 = r16; r1 = 00000049; r8 = sfmpy(r25,r26) }
	{ memd(r29+32) = r23:r22; r9 = memw(r5+4); r8 = sfmpy(r8,r27) }
	{ memd(r29+40) = r21:r20; r5 = memw(r5+8) }
	{ memw(r29+8) = r9 }
	{ memw(r29+12) = r5; r22 = memw(r6+16); r24 = sfmpy(r8,r18) }
	{ r17 = memw(r3); r23 = memw(r19+16) }
	{ memw(r29) = r0; r20 = memw(r7); call logmsg_function; r21 = convert_uw2sf(r24):chop }
	{ if (!cmp.gtu(r21,r2.new)) jump:t 000175AC; r2 = memw(r19+20); r6 = convert_sf2uw(r18); r7 = convert_sf2uw(r26) }

l00017594:
	{ r2 = r16; r1 = 0000004A; r3 = add(PC,0000000B) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000176BC }

l000175AC:
	{ r4 = 00000000; r2 = convert_sf2uw(r25); r3 = convert_sf2uw(r27) }
	{ memw(r19+24) = r21; r18 = r4 }
	{ memw(r19) = r2; memw(r19+8) = r3; r1:r0 = combine(r17,r18) }
	{ memw(r19+4) = r7; memw(r19+12) = r6 }
	{ call fn000097B0 }
	{ r16 = r0 }
	{ r17 = 437E0000; call fmaxf.1.0; r0 = sfsub(r20,r16) }
	{ r1 = r17; r19 = r0; call fn00009610 }
	{ r1:r0 = combine(r19,r17); r20 = r0; call fn00009610 }
	{ r16 = r0; r2 = D916872B; r4 = sfsub(r18,r16) }
	{ r3 = 3FEFF7CE; r4 = sfmpy(r4,r16) }
	{ call fn000097D0; r1:r0 = convert_sf2df(r4) }
	{ r3 = 437F0000; p0 = sfcmp.gt(r24,r18); r2 = convert_df2w(r1:r0):chop }
	{ r2 = sub(00000000,r2); if (p0) r19 = 00000001; if (p0) r25 = 00000100 }
	{ r2 = convert_w2sf(r2) }
	{ r18 = sfmpy(r20,r2) }
	{ r17 = r18 }
	{ if (!p0) jump:nt 00017688; r17 += sfmpy(r20,r3) }

l00017648:
	{ r2 = memw(r22) }
	{ r2 = sfsub(r2,r18) }
	{ call fn00009620; r0 = sfmpy(r16,r2) }
	{ r2 = 00000000; r3 = convert_uw2sf(r0):chop }
	{ if (p0.new) r2 = add(r3,00000000); if (p0.new) jump:nt 00017670; p0 = cmp.gt(r3,-00000001) }

l00017668:
	{ if (!p0.new) r2 = 000000FF; p0 = cmp.gt(r25,r3) }

l00017670:
	{ memb(r23++#1) = r2; r19 = add(r19,00000001); r22 = add(r22,00000004); r3 = convert_w2sf(r19) }
	{ if (p0.new) jump:nt 00017648; p0 = sfcmp.gt(r24,r3) }

l00017688:
	{ r3 = memd(r29+12); r4 = memd(r29+8); r0 = 00000000 }
	{ memw(r4+12) = FFFFFF81 }
	{ memw(r4+4) = 00000001; memw(r4+8) = 00000001 }
	{ memw(r3+8) = 00000001; memw(r4) = 00000001 }
	{ memw(r3+12) = 00000001; memw(r3) = 00000001 }
	{ memw(r3+4) = FFFFFF81 }
	{ r2 = memw(r4+16) }
	{ memw(r2) = r18 }
	{ r7 = memw(r3+16) }
	{ memw(r4+24) = 00000004; memw(r7) = r17 }
	{ memw(r3+24) = 00000004 }

l000176BC:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }

;; quantize_check: 000176D0
quantize_check proc
	{ jump quantize_check__merged; r0 = 00000000 }

;; quantize_check__merged: 000176D4
;;   Called from:
;;     000176D0 (in quantize_check)
;;     000178DC (in dequantize_check)
quantize_check__merged proc
	{ allocframe(00000018); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1); p0 = cmp.eq(r2,00000001) }
	{ if (p0) r1 = 000000A7; if (p0) jump:nt 00017700 }

l000176E8:
	{ memb(r29+2) = r0.new; r0 = p0; r4 = add(PC,000105EF) }
	{ r1 = 0000009E }

l00017700:
	{ r0 = p0; r4 = add(PC,00010569) }
	{ memw(r29+8) = r0 }
	{ memw(r29) = r17; r2 = r16 }
	{ call logmsg_function }
	{ r0 = memw(r29+8) }
	{ if (cmp.eq(r2.new,00000003)) jump:t 00017754; r2 = memw(r17+16); p1 = r0 }

l0001772C:
	{ r1 = 0000009F; if (!p1) jump:nt 00017748; r3 = add(PC,0000001D) }

l00017738:
	{ r1 = 000000A8; r3 = add(PC,0001054D) }

l00017744:
	{ r2 = r16; call errlog_function }

l00017748:
	{ r2 = r16 }

l0001774C:
	{ dealloc_return; r17:r16 = memd(r29+16); r0 = FFFFFFFF }

l00017754:
	{ r2 = memw(r17+20); if (p1) jump:nt 00017794 }

l0001775C:
	{ r1 = 000000A0; if (p0.new) jump:nt 00017780; p0 = cmp.eq(r2,00000003) }

l00017764:
	{ if (!p1) jump:nt 00017744; r3 = add(PC,00010530) }

l00017770:
	{ r1 = 000000A9; jump 00017744; r3 = add(PC,00010524) }

l00017780:
	{ if (!p1) r1 = 000000A1; if (p1) jump:nt 00017798 }

l00017788:
	{ jump 000177A4; r4 = add(PC,00010569) }

l00017794:
	{ if (!p0.new) jump:nt 00017770; p0 = cmp.eq(r2,00000001) }

l00017798:
	{ r1 = 000000AA; r4 = add(PC,0001050C) }

l000177A4:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+16) }

;; dequantize_execute: 000177B4
dequantize_execute proc
	{ allocframe(00000038); memd(r29+488) = r19:r18; r18 = r0 }
	{ memd(r29+48) = r17:r16; r7 = memw(r18+4); r17 = r1 }
	{ r6 = memw(r18+8) }
	{ memd(r29+32) = r21:r20; memd(r29+24) = r23:r22 }
	{ r5 = memw(r7+4); r4 = memw(r7+8) }
	{ memd(r29+8) = r27:r26; memd(r29+16) = r25:r24 }
	{ r3 = memw(r4+16); r2 = memw(r5+16) }
	{ r23 = memw(r6); r22 = memw(r7) }
	{ r2 = memw(r3); r19 = memw(r2) }
	{ call fmaxf.1.0; r0 = sfsub(r2,r19) }
	{ r1 = 437F0000; call fn00009610 }
	{ r3 = memw(r22+4); r2 = memw(r22); r5 = 40800000 }
	{ r7 = memw(r22+12); r6 = memw(r22+8); r4 = add(PC,000104B6) }
	{ r16 = r0; r1 = 00000075; r26 = convert_uw2sf(r3); r27 = convert_uw2sf(r2) }
	{ r20 = memw(r23+16); r2 = r17; r24 = convert_uw2sf(r7); r25 = convert_uw2sf(r6) }
	{ memw(r29) = r18; r22 = memw(r22+16); r3 = sfmpy(r27,r26) }
	{ r3 = sfmpy(r3,r25) }
	{ r21 = sfmpy(r3,r24) }
	{ r3 = sfmpy(r21,r5) }
	{ call logmsg_function; r18 = convert_uw2sf(r3):chop }
	{ if (!cmp.gtu(r18,r2.new)) jump:t 00017874; r2 = memw(r23+20); r0 = 00000000; r3 = convert_sf2uw(r27) }

l0001785C:
	{ r2 = r17; r1 = 00000076; r3 = add(PC,00000003) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000178C8 }

l00017874:
	{ r2 = 00000000; r5 = convert_sf2uw(r25); r4 = convert_sf2uw(r26) }
	{ memw(r23+4) = r4; memw(r23+24) = r18; if (p0.new) r2 = 00000001; p0 = sfcmp.gt(r21,r2) }
	{ memw(r23+8) = r5; memw(r23) = r3; r3 = convert_sf2uw(r24) }
	{ memw(r23+12) = r3; if (!p0) jump:nt 000178C8 }

l000178A0:
	{ r3 = memb(r22++#1); r2 = add(r2,00000001); r5 = r19; r4 = convert_w2sf(r2) }
	{ p0 = sfcmp.gt(r21,r4); r3 = convert_w2sf(r3) }
	{ r20 = add(r20,00000004); if (p0) jump:nt 000178A0; r5 += sfmpy(r16,r3) }

l000178C8:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }

l000178CC:
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }

;; dequantize_check: 000178DC
dequantize_check proc
	{ jump quantize_check__merged; r1 = 00000001 }

;; dequantize_i32_execute: 000178E0
dequantize_i32_execute proc
	{ allocframe(+00000030) }
	{ r6 = memw(r0+8); r7 = memw(r0+4) }
	{ memd(r29+32) = r19:r18; memd(r29+40) = r17:r16; r16 = r1 }
	{ r5 = memw(r7+4); r4 = memw(r7+8) }
	{ memd(r29+16) = r23:r22; memd(r29+24) = r21:r20 }
	{ r3 = memw(r4+16); r2 = memw(r5+16) }
	{ memd(r29) = r27:r26; memd(r29+8) = r25:r24 }
	{ r3 = memw(r3); r2 = memw(r2) }
	{ r20 = memw(r6); r18 = memw(r7) }
	{ call fmaxf.1.0; r0 = sfsub(r3,r2) }
	{ r3 = memw(r18+4); r2 = memw(r18); r5 = 40800000 }
	{ r6 = memw(r18+12); r4 = memw(r18+8); r7 = 2F800000 }
	{ r1 = 00000091; r23 = convert_uw2sf(r2); r21 = sfmpy(r0,r7) }
	{ r18 = memw(r18+16); r2 = r16; r25 = convert_uw2sf(r4); r24 = convert_uw2sf(r3) }
	{ r19 = memw(r20+16); r3 = sfmpy(r23,r24); r22 = convert_uw2sf(r6) }
	{ r3 = sfmpy(r3,r25); r4 = add(PC,000102F4) }
	{ r17 = sfmpy(r3,r22) }
	{ r3 = sfmpy(r17,r5) }
	{ call logmsg_function; r26 = convert_uw2sf(r3):chop }
	{ if (!cmp.gtu(r26,r2.new)) jump:t 00017998; r2 = memw(r20+20); r0 = 00000000; r3 = convert_sf2uw(r23) }

l00017980:
	{ r2 = r16; r1 = 00000092; r3 = add(PC,0000001F) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000179EC }

l00017998:
	{ r2 = 00000000; r5 = convert_sf2uw(r25); r4 = convert_sf2uw(r24) }
	{ memw(r20+4) = r4; memw(r20+24) = r26; if (p0.new) r2 = 00000001; p0 = sfcmp.gt(r17,r2) }
	{ memw(r20+8) = r5; memw(r20) = r3; r3 = convert_sf2uw(r22) }
	{ memw(r20+12) = r3; if (!p0) jump:nt 000179EC }

l000179C8:
	{ r3 = memw(r18++#4); r2 = add(r2,00000001); r4 = convert_w2sf(r2) }
	{ p0 = sfcmp.gt(r17,r4); r3 = convert_w2sf(r3) }
	{ memb(r19) = r3.new; r19 = add(r19,00000004); if (p0) jump:nt 000179C8; r3 = sfmpy(r21,r3) }

l000179EC:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }

l000179F0:
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ r27:r26 = memd(r29); r25:r24 = memd(r29+8) }
	{ dealloc_return }

;; logmsg_function: 00017A00
;;   Called from:
;;     00017574 (in quantize_execute)
;;     00017714 (in quantize_check__merged)
;;     000177A4 (in quantize_check__merged)
;;     00017840 (in dequantize_execute)
;;     00017964 (in dequantize_i32_execute)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00017A24; r5 = memw(r2+16) }

l00017A10:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000001A) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00017A24:
	{ dealloc_return }

;; errlog_function: 00017A28
;;   Called from:
;;     000175A0 (in quantize_execute)
;;     00017744 (in quantize_check__merged)
;;     00017868 (in dequantize_execute)
;;     0001798C (in dequantize_i32_execute)
;;     00017A18 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,000101FE) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; fmaxf.1.0: 00017A4C
;;   Called from:
;;     000175D4 (in quantize_execute)
;;     000177E4 (in dequantize_execute)
;;     0001790C (in dequantize_i32_execute)
fmaxf.1.0 proc
	{ r2 = 38D1B717 }
	{ r1:r0 = combine(r0,r2); jump fn00009600 }
00017A5C                                     00 00 00 00             ....

;; relu_execute: 00017A60
relu_execute proc
	{ allocframe(00000048); memd(r29+496) = r17:r16; r4 = add(PC,0001033E) }
	{ r17:r16 = combine(r1,r0) }
	{ memd(r29+40) = r23:r22; r2 = memw(r16+4); r1 = 00000050 }
	{ r3 = memw(r16+8) }
	{ memd(r29+32) = r25:r24; memd(r29+48) = r21:r20 }
	{ r25 = memw(r3); r22 = memw(r2) }
	{ r3 = memw(r3+4); r21 = memw(r3+8) }
	{ r6 = memw(r22+4); r5 = memw(r22) }
	{ memd(r29+56) = r19:r18; r19 = memw(r2+4) }
	{ r20 = memw(r2+8) }
	{ memw(r29+20) = r3; r7 = memw(r22+8); r2 = r17; r3 = mpyi(r6,r5) }
	{ r9 = memw(r20+16); r8 = memw(r22+12); r3 = mpyi(r3,r7) }
	{ memd(r29+24) = r27:r26; r12 = memw(r19+16) }
	{ r24 = memw(r25+16); r23 = memw(r22+16) }
	{ r18 = memw(r9); r27 = memw(r12) }
	{ memw(r29) = r16; call logmsg_function; r26 = mpyi(r3,r8) }
	{ r3 = memw(r19+16); r2 = memw(r20+16); r4 = add(PC,00010324) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = 00000053 }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r0 = 38D1B717; call fn00009600; r1 = sfsub(r18,r27) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = 00000000 }
	{ r2 = sfsub(r2,r27) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ if (!cmp.gtu(r26,r3.new)) jump:t 00017B64; r3 = memw(r25+20); p0 = cmp.eq(r26,00000000); r2 = convert_uw2sf(r0):chop }

l00017B4C:
	{ r2 = r17; r1 = 00000057; r3 = add(PC,0000002D) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00017C48 }

l00017B64:
	{ r4 = memw(r22+4); r3 = memw(r22) }
	{ memw(r25) = r3; memw(r25+4) = r4 }
	{ r6 = memw(r22+8) }
	{ memw(r25+8) = r6 }
	{ r7 = memw(r22+12) }
	{ memw(r25+24) = r26; memw(r25+12) = r7; if (p0) jump:nt 00017BB4 }

l00017B88:
	{ if (p0.new) r2 = FFFFFFFF; p1 = cmp.gt(r2,FFFFFFFF); p0 = cmp.gt(r2,000000FF); loop0(00017BA0,r26) }
	{ if (!p1) r2 = 00000000 }
	{ r4 = and(r2,000000FF) }
	{ r3 = memb(r23++#1) }
	{ if (!p0.new) r3 = add(r2,00000000); p0 = cmp.gtu(r3,r4) }
	{ memb(r24++#1) = r3; nop }

l00017BB4:
	{ r18 = memw(r29+20); r2 = add(r19,00000010) }
	{ r1:r0 = combine(r19,r18); call relu_execute.extracted_region }
	{ r3 = memw(r20+4); r2 = memw(r20) }
	{ memw(r21) = r2; memw(r21+4) = r3 }
	{ r2 = memw(r20+8) }
	{ memw(r21+8) = r2 }
	{ r7 = memw(r20+12) }
	{ memw(r21+12) = r7 }
	{ r2 = memw(r20+24) }
	{ if (!cmp.gtu(r2,r6.new)) jump:t 00017BF0; r6 = memw(r21+20) }

l00017BEC:
	{ r19 = add(r21,00000010) }

l00017BF0:
	{ memw(r21+24) = r2; r0 = memw(r21+16); r19 = add(r21,00000010) }
	{ r1 = memw(r20+16); r2 = memw(r20+24); call fn00009560 }
	{ r3 = memw(r18+16); r2 = memw(r19); r4 = add(PC,00010213) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = 00000064 }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = r17; r1 = 00000065; r4 = add(PC,00010197) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }

l00017C48:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }
00017C5C                                     00 C0 00 7F             ....

;; relu_check: 00017C60
relu_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00010170) }
	{ r17 = r0; r16 = r1; r1 = 00000098 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 00000099; r3 = add(PC,000100D5) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 00017CE8; r2 = memw(r17+16) }

l00017C94:
	{ r1 = 0000009A; r3 = add(PC,00000010) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 00017CE8; r2 = memw(r17+20) }

l00017CA8:
	{ r4 = memw(r17+4); r5 = 00000000 }

l00017CAC:
	{ if (cmp.gt(r5.new,00000002)) jump:nt 00017CF8; r5 = add(r5,00000001); r2 = add(r2,00000004) }

l00017CBC:
	{ if (!cmp.eq(r6.new,00000000)) jump:t 00017CD4; r6 = memw(r4++#4); r3 = add(PC,00000038) }

l00017CCC:
	{ r1 = 0000009C }
	{ r6 = memw(r17+8); r3 = add(PC,000100AB) }

l00017CD4:
	{ r6 = memw(r17+8); r3 = add(PC,0000002B) }

l00017CDC:
	{ if (!cmp.eq(r6.new,00000000)) jump:t 00017CAC; r6 = memw(r4+r2) }

l00017CE8:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00017CF8:
	{ r2 = r16; r1 = 0000009F; r4 = add(PC,000100EE) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
00017D18                         00 40 00 7F 00 C0 00 7F         .@......

;; reluX_execute: 00017D20
reluX_execute proc
	{ allocframe(00000040); memd(r29+496) = r17:r16; r4 = add(PC,0001007E) }
	{ r16 = r0 }
	{ memd(r29+24) = r25:r24; r3 = memw(r16+4) }
	{ memd(r29+48) = r19:r18; r5 = memw(r16+8); r1 = 00000081; r18 = r1 }
	{ memd(r29+16) = r27:r26; r25 = memw(r3); r2 = r18 }
	{ r19 = memw(r3+4); r6 = memw(r3+12) }
	{ r0 = memw(r25); r17 = memw(r3+8) }
	{ r8 = memw(r25+8); r7 = memw(r25+4) }
	{ r9 = memw(r25+12); r26 = memw(r5); r3 = mpyi(r7,r0) }
	{ r6 = memw(r6+16); r12 = memw(r17+16); r3 = mpyi(r3,r8) }
	{ r13 = memw(r19+16) }
	{ memd(r29+32) = r23:r22; memd(r29+40) = r21:r20 }
	{ r5 = memw(r5+4); r14 = memw(r5+8) }
	{ memw(r29+12) = r14 }
	{ memw(r29+8) = r5; r23 = memw(r25+16); r27 = mpyi(r3,r9) }
	{ r20 = memw(r13); r24 = memw(r26+16) }
	{ r21 = memw(r6); r22 = memw(r12) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 38D1B717; call fn00009600; r1 = sfsub(r22,r20) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r22 = r0; r2 = 00000000 }
	{ r2 = sfsub(r2,r20) }
	{ call fn00009620; r0 = sfmpy(r2,r22) }
	{ r2 = sfsub(r21,r20) }
	{ r20 = convert_uw2sf(r0):chop }
	{ call fn00009620; r0 = sfmpy(r2,r22) }
	{ if (!cmp.gtu(r27,r3.new)) jump:t 00017E20; r3 = memw(r26+20); p0 = cmp.eq(r27,00000000); r2 = convert_uw2sf(r0):chop }

l00017E08:
	{ r2 = r18; r1 = 00000086; r3 = add(PC,00000031) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00017ED8 }

l00017E20:
	{ r4 = memw(r25+4); r3 = memw(r25) }
	{ memw(r26) = r3; memw(r26+4) = r4; if (!p0) r4 = 00000000 }
	{ r6 = memw(r25+8) }
	{ memw(r26+8) = r6; r0 = memw(r29+8) }
	{ r7 = memw(r25+12); r21 = memw(r29+12) }
	{ memw(r26+24) = r27; memw(r26+12) = r7; if (p0) jump:nt 00017EA0 }

l00017E54:
	{ p0 = cmp.gt(r2,FFFFFFFF); p1 = cmp.gt(r20,FFFFFFFF); p3 = cmp.gt(r2,000000FF); p2 = cmp.gt(r20,000000FF) }
	{ if (p3) r2 = FFFFFFFF; if (p2) r20 = FFFFFFFF; loop0(00017E7C,r27) }
	{ r3 = mux(p1,r20,r4); if (!p0) r2 = add(r4,00000000) }
	{ r6 = and(r2,000000FF) }
	{ r4 = memb(r23++#1); r5 = and(r3,000000FF) }
	{ if (!p0.new) r4 = add(r3,00000000); p0 = cmp.gtu(r4,r5) }
	{ r7 = and(r4,000000FF) }
	{ if (!p0.new) r4 = add(r2,00000000); p0 = cmp.gtu(r6,r7) }
	{ memb(r24++#1) = r4; nop }

l00017EA0:
	{ r19 = add(r17,00000010); r1 = r19; r2 = add(r19,00000010); call relu_execute.extracted_region }
	{ r1:r0 = combine(r17,r21); r2 = r19; call relu_execute.extracted_region }
	{ r2 = r18; r1 = 00000091; r4 = add(PC,0000FF07) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }

l00017ED8:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }

;; reluX_check: 00017EEC
reluX_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000FE4E) }
	{ r17 = r0; r16 = r1; r1 = 000000A6 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 000000A7; r3 = add(PC,0000FE49) }
	{ if (!cmp.eq(r2.new,00000004)) jump:t 00017F8C; r2 = memw(r17+16) }

l00017F20:
	{ r1 = 000000A8; r3 = add(PC,00000004) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 00017F8C; r2 = memw(r17+20) }

l00017F34:
	{ jump 00017F44; r6 = r2 }

l00017F38:
	{ if (cmp.gt(r5.new,00000002)) jump:nt 00017F78; r5 = add(r5,00000001); r4 = add(r4,00000004) }

l00017F44:
	{ if (!cmp.eq(r7.new,00000000)) jump:t 00017F5C; r7 = memw(r6++#4); r3 = add(PC,0000FE2C) }

l00017F48:
	{ if (!cmp.eq(r7.new,00000000)) jump:t 00017F60; r7 = memw(r6++#4); r3 = add(PC,0000002C) }

l00017F58:
	{ r1 = 000000AA }

l00017F5C:
	{ r7 = memw(r17+8); r3 = add(PC,0000FE1F) }

l00017F60:
	{ r7 = memw(r17+8); r3 = add(PC,0000001F) }
	{ if (!cmp.eq(r7.new,00000000)) jump:t 00017F38; r7 = memw(r20+r4) }

l00017F74:
	{ r1 = 000000AB }

l00017F78:
	{ r1 = 000000AD; r3 = add(PC,0000FDF8) }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 00017F9C; r2 = memw(r2+12) }

l00017F8C:
	{ r2 = r16; call errlog_function }

l00017F90:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00017F9C:
	{ r2 = r16; r1 = 000000AE; r4 = add(PC,0000FDEB) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00017FBC
;;   Called from:
;;     00017ACC (in relu_execute)
;;     00017B00 (in relu_execute)
;;     00017C28 (in relu_execute)
;;     00017C3C (in relu_execute)
;;     00017C74 (in relu_check)
;;     00017D08 (in relu_check)
;;     00017DAC (in reluX_execute)
;;     00017ECC (in reluX_execute)
;;     00017F00 (in reluX_check)
;;     00017FAC (in reluX_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00017FE0; r5 = memw(r2+16) }

l00017FCC:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000018) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00017FE0:
	{ dealloc_return }

;; errlog_function: 00017FE4
;;   Called from:
;;     00017B58 (in relu_execute)
;;     00017CE8 (in relu_check)
;;     00017E14 (in reluX_execute)
;;     00017F8C (in reluX_check)
;;     00017FD4 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000FD3C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; relu_execute.extracted_region: 00018008
;;   Called from:
;;     00017BBC (in relu_execute)
;;     00017EA0 (in reluX_execute)
;;     00017EB0 (in reluX_execute)
relu_execute.extracted_region proc
	{ allocframe(+00000000) }
	{ r4 = memw(r1+4); r3 = memw(r1) }
	{ memw(r0) = r3; memw(r0+4) = r4 }
	{ r6 = memw(r1+8) }
	{ memw(r0+8) = r6 }
	{ r7 = memw(r1+12) }
	{ memw(r0+12) = r7 }
	{ r3 = memw(r1+24) }
	{ if (cmp.gtu(r3,r4.new)) jump:t 00018040; r4 = memw(r0+20) }

l00018034:
	{ r1 = memw(r2); r3 = memw(r1+24) }
	{ r2 = r3; call fn00009560 }

l00018040:
	{ dealloc_return }
00018044             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; autorequantize_execute: 00018050
autorequantize_execute proc
	{ allocframe(00000048); r5 = 2F800000 }
	{ memd(r29+64) = r17:r16; r17:r16 = combine(r1,r0); r4 = add(PC,0000FEF9) }
	{ memd(r29+40) = r23:r22; r2 = memw(r16+4); r1 = 0000004B }
	{ memd(r29+32) = r25:r24; r3 = memw(r16+8) }
	{ memd(r29+24) = r27:r26; r22 = memw(r2) }
	{ r25 = memw(r2+8); r24 = memw(r2+4) }
	{ r7 = memw(r22+4); r23 = memw(r3) }
	{ r2 = memw(r25+16); r6 = memw(r24+16) }
	{ r9 = memw(r22+8); r8 = memw(r22) }
	{ r12 = memw(r22+12); r0 = memw(r6); r7 = mpyi(r7,r8) }
	{ r13 = memw(r2); r2 = r17; r6 = mpyi(r7,r9) }
	{ r26 = memw(r3+4); r27 = memw(r3+8); r3 = sfsub(r13,r0) }
	{ memd(r29+48) = r21:r20; memd(r29+56) = r19:r18 }
	{ r19 = memw(r23+16); r18 = memw(r22+16) }
	{ memw(r29) = r16; r21 = sfmpy(r3,r5); r20 = mpyi(r6,r12) }
	{ call logmsg_function }
	{ r3 = memw(r24+16); r2 = memw(r25+16); r4 = add(PC,0000FE9A) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = 0000004E }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ if (!cmp.gtu(r20,r2.new)) jump:t 0001812C; r2 = memw(r23+20) }

l00018114:
	{ r2 = r17; r1 = 0000004F; r3 = add(PC,00000003) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001834C }

l0001812C:
	{ r3 = memw(r22+4); r2 = memw(r22); p0 = cmp.eq(r20,00000000) }
	{ memw(r23) = r2; memw(r23+4) = r3; r0 = p0 }
	{ r6 = memw(r22+8); if (p0) r2 = 4F000000 }
	{ memw(r23+8) = r6; if (p0) r24 = CF000000 }
	{ r7 = memw(r22+12) }
	{ memw(r23+24) = r20; memw(r23+12) = r7 }
	{ memw(r29+16) = r0; jump 000181CC; if (!p0) jump:nt 00018168 }
00018168                         FF 7F FF 07 A6 30 F2 2B         .....0.+
00018170 00 40 00 08 03 40 00 78 E4 7F F4 BF 25 C0 86 9B .@...@.x....%...
00018180 1E 61 4C 11 E7 7F E4 BF 04 E0 85 74 00 41 07 60 .aL........t.A.`
00018190 20 40 84 75 24 C0 86 9B 0E C0 20 5C 02 45 A2 D5  @.u$..... \.E..
000181A0 03 43 C5 D5 07 40 64 70 24 C0 86 9B 05 80 67 70 .C...@dp$.....gp
000181B0 00 C0 00 7F 03 43 C5 D5 02 C5 A2 D5 02 44 A2 D5 .....C.......D..
000181C0 03 C3 C4 D5 02 40 42 8B 18 C0 43 8B             .....@B...C.    

l000181CC:
	{ r22 = 00000000; r1 = sfmpy(r21,r2) }
	{ r0 = r22; call fn000097B0 }
	{ r23 = r0 }
	{ r0 = togglebit(r23,0000001E) }
	{ r24 = 437E0000; call fmaxf.1.0; r0 += sfmpy(r21,r24) }
	{ r1 = r24; r25 = r0; call fn00009610 }
	{ r0 = r24; r25 = r0; r1 = r25; call fn00009610 }
	{ r23 = r0; r2 = D916872B; r4 = sfsub(r22,r23) }
	{ r3 = 3FEFF7CE; r4 = sfmpy(r4,r23) }
	{ call fn000097D0; r1:r0 = convert_sf2df(r4) }
	{ p0 = cmp.gtu(r20,0000007F); r3 = 437F0000; r2 = convert_df2w(r1:r0):chop }
	{ if (p0) r4 = 4F000000; r2 = sub(00000000,r2) }
	{ r2 = convert_w2sf(r2) }
	{ r22 = sfmpy(r25,r2) }
	{ if (!p0) r0 = memw(r29+16); r24 = r22; if (p0) r1:r0 = combine(r21,r22) }
	{ if (p0) r3 = 3F000000; if (p0) jump:nt 000182A8; r24 += sfmpy(r25,r3) }

l00018274:
	{ if (p0.new) jump:nt 000182C8; p0 = r0 }

l0001827C:
	{ call fmaxf.1.0; r0 = sfsub(r24,r22) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ memw(r29) = r20; r3:r2 = combine(00000000,00000002); r1:r0 = combine(r19,r18); r5:r4 = combine(r0,r22) }
	{ call autorequantize_execute.extracted_region }
	{ jump 000182C8 }

l000182A8:
	{ r2 = sfmpy(r21,r23) }
	{ r3 += sfmpy(r2,r4) }
	{ call fn00009610; r21 = convert_uw2sf(r3):chop }
	{ r0 = r18; r4 = r20; r3:r2 = combine(r19,r21); r1 = convert_uw2sf(r0):chop }
	{ call quantize_asm }

l000182C8:
	{ memw(r26+12) = FFFFFF81; r2 = memw(r26+16); r4 = add(PC,0000FD61) }
	{ memw(r26+4) = FFFFFF81; memw(r26+8) = 00000001; r1 = 00000080 }
	{ memw(r2) = r22; memw(r26) = 00000001 }
	{ memw(r27+12) = FFFFFF81; memw(r26+24) = 00000004 }
	{ memw(r27+4) = FFFFFF81; r7 = memw(r27+16) }
	{ memw(r27+8) = 00000001; memw(r27) = 00000001 }
	{ memw(r27+24) = 00000004; memw(r7) = r24 }
	{ r3 = memw(r26+16); r2 = memw(r27+16) }
	{ r3 = memw(r3); r2 = memw(r2) }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = r17; r1 = 00000081; r4 = add(PC,0000FD1B) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }

l0001834C:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }

;; autorequantize_check: 00018360
autorequantize_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000FC72) }
	{ r17 = r0; r16 = r1; r1 = 00000107 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 00000108; r3 = add(PC,0000FAED) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 000183E8; r2 = memw(r17+16) }

l00018394:
	{ r1 = 00000109; r3 = add(PC,00000028) }
	{ if (!cmp.eq(r2.new,00000003)) jump:t 000183E8; r2 = memw(r17+20) }

l000183A8:
	{ r4 = memw(r17+4); r5 = 00000000 }

l000183AC:
	{ if (cmp.gt(r5.new,00000002)) jump:nt 000183F8; r5 = add(r5,00000001); r2 = add(r2,00000004) }

l000183BC:
	{ if (!cmp.eq(r6.new,00000000)) jump:t 000183D4; r6 = memw(r4++#4); r3 = add(PC,0000003A) }

l000183CC:
	{ r1 = 0000010B }
	{ r6 = memw(r17+8); r3 = add(PC,0000FC2D) }

l000183D4:
	{ r6 = memw(r17+8); r3 = add(PC,0000002D) }

l000183DC:
	{ if (!cmp.eq(r6.new,00000000)) jump:t 000183AC; r6 = memw(r4+r2) }

l000183E8:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l000183F8:
	{ r2 = r16; r1 = 0000010E; r4 = add(PC,0000FC11) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; requantize_execute: 00018418
requantize_execute proc
	{ allocframe(+00000050); r4 = add(PC,0000FB39) }
	{ memd(r29+72) = r17:r16; r6 = 2F800000; r17 = r0 }
	{ memd(r29+64) = r19:r18; r2 = memw(r17+4); r18 = r1 }
	{ memd(r29+48) = r23:r22; r1 = 0000009F }
	{ memd(r29+40) = r25:r24; r3 = memw(r17+8) }
	{ r23 = memw(r2+4); r24 = memw(r2) }
	{ r5 = memw(r2+12); r19 = memw(r2+8) }
	{ r9 = memw(r24+4); r8 = memw(r23+16) }
	{ r12 = memw(r24); r7 = memw(r19+16) }
	{ r2 = memw(r2+16); r5 = memw(r5+16); r9 = mpyi(r9,r12) }
	{ r7 = memw(r7); r13 = memw(r24+8) }
	{ memd(r29+32) = r27:r26; r8 = memw(r8) }
	{ r22 = memw(r3+8); r25 = memw(r3) }
	{ r3 = memw(r5); r27 = memw(r3+4); r5 = mpyi(r9,r13) }
	{ r14 = memw(r24+12); r15 = memw(r2+16); r2 = r18 }
	{ memd(r29+56) = r21:r20; memw(r29+24) = r3; r3 = sfsub(r7,r8) }
	{ r21 = memw(r24+16); r0 = memw(r15); r26 = mpyi(r5,r14) }
	{ r16 = memw(r25+16) }
	{ memw(r29) = r17; memw(r29+28) = r0; call logmsg_function; r20 = sfmpy(r3,r6) }
	{ r3 = memw(r23+16); r2 = memw(r19+16); r4 = add(PC,0000FAB2) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = 000000A2 }
	{ r2 = r18; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ if (!cmp.gtu(r26,r2.new)) jump:t 00018514; r2 = memw(r25+20); r4 = r26 }

l000184FC:
	{ r2 = r18; r1 = 000000A3; r3 = add(PC,0000001B) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 000186C8 }

l00018514:
	{ r3 = memw(r24+4); r2 = memw(r24); r23 = 00000000 }
	{ memw(r25) = r2; memw(r25+4) = r3; r27:r26 = combine(r20,r27) }
	{ r19 = r22; r0 = r23 }
	{ r1 = memw(r29+24); r7 = memw(r24+8); r20 = r4 }
	{ memw(r29+20) = r16; memw(r25+8) = r7 }
	{ r2 = memw(r24+12) }
	{ memw(r29+16) = r21 }
	{ memw(r25+24) = r20; memw(r25+12) = r2; call fn000097B0 }
	{ r2 = memd(r29+28); r22 = r0 }
	{ r24 = 437E0000; call fmaxf.1.0; r0 = sfsub(r2,r22) }
	{ r1 = r24; r25 = r0; call fn00009610 }
	{ r1:r0 = combine(r25,r24); r16 = r0; call fn00009610 }
	{ r2 = D916872B; r4 = sfsub(r23,r22) }
	{ r3 = 3FEFF7CE }
	{ r4 = sfmpy(r4,r0) }
	{ call fn000097D0; r1:r0 = convert_sf2df(r4) }
	{ p0 = cmp.gtu(r20,0000007F); r2 = convert_df2w(r1:r0):chop }
	{ r2 = 437F0000; r3 = sub(00000000,r2) }
	{ r3 = convert_w2sf(r3) }
	{ r22 = sfmpy(r16,r3) }
	{ r23 = r22 }
	{ if (p0) r16 = add(r27,00000000); if (p0) jump:nt 0001860C; r23 += sfmpy(r16,r2) }

l000185D4:
	{ r16 = r18; if (p0.new) jump:nt 0001864C; p0 = cmp.eq(r12,00000000) }

l000185DC:
	{ call fmaxf.1.0; r0 = sfsub(r23,r22) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r1 = memd(r29+20); r0 = memd(r29+16); r3:r2 = combine(r27,00000000); r5:r4 = combine(r0,r22) }
	{ memw(r29) = r20; call autorequantize_execute.extracted_region }
	{ jump 0001864C }

l0001860C:
	{ r0 = sfmpy(r16,r2); r1 = sfsub(r23,r22) }
	{ call fn00009610 }
	{ r3 = 4F000000; r2 = 3F000000 }
	{ r1:r0 = combine(r16,r22); r2 += sfmpy(r0,r3) }
	{ call fn00009610; r21 = convert_uw2sf(r2):chop }
	{ r3 = memd(r29+20); r4 = r20; r2 = r21; r1 = convert_uw2sf(r0):chop }
	{ r0 = memd(r29+16); r16 = r18; call quantize_asm }

l0001864C:
	{ memw(r26+12) = FFFFFF81; r2 = memw(r26+16); r4 = add(PC,0000F955) }
	{ memw(r26+4) = FFFFFF81; memw(r26+8) = 00000001; r1 = 000000C8 }
	{ memw(r2) = r22; memw(r26) = 00000001 }
	{ memw(r19+12) = FFFFFF81; memw(r26+24) = 00000004 }
	{ memw(r19+4) = 00000001; r7 = memw(r19+16) }
	{ memw(r19+8) = 00000001; memw(r19) = 00000001 }
	{ memw(r19+24) = 00000004; memw(r7) = r23 }
	{ r3 = memw(r26+16); r2 = memw(r19+16) }
	{ r3 = memw(r3); r2 = memw(r2) }
	{ r2 = r16; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = r16; r1 = 000000C9; r4 = add(PC,0000F913) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l000186C8:
	{ r19:r18 = memd(r29+64); r17:r16 = memd(r29+72) }
	{ r23:r22 = memd(r29+48); r21:r20 = memd(r29+56) }
	{ r27:r26 = memd(r29+32); r25:r24 = memd(r29+40) }
	{ dealloc_return }

;; requantize_check: 000186DC
requantize_check proc
	{ jump requantize_check__merged; r0 = 00000000 }

;; requantize_check__merged: 000186E0
;;   Called from:
;;     000186DC (in requantize_check)
;;     00018960 (in requantrange_check)
requantize_check__merged proc
	{ allocframe(00000028); p0 = cmp.eq(r2,00000001); r4 = add(PC,0000F839) }
	{ memd(r29+24) = r19:r18; memd(r29+32) = r17:r16; if (!p0) r1 = 00000114; r17:r16 = combine(r0,r1) }
	{ if (p0) r1 = 0000011D; r18 = add(PC,0000F83D) }
	{ memd(r29+8) = r23:r22; memd(r29+16) = r21:r20; if (p0) jump:nt 00018720 }

l0001870C:
	{ r21 = 00000115; r20 = 00000116; r19 = 00000117; r23:r22 = combine(00000005,00000003) }
	{ jump 00018740 }

l00018720:
	{ r19 = 00000120; r23:r22 = combine(00000003,00000002); r4 = add(PC,0000F72B) }
	{ r21 = 0000011E; r20 = 0000011F; r18 = add(PC,0000F758) }

l00018740:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,r23)) jump:t 0001876C; r2 = memw(r17+16); r1 = r21 }

l00018758:
	{ r3 = add(PC,00000015) }
	{ r2 = r16; call errlog_function }

l00018760:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 00018798 }

l0001876C:
	{ if (cmp.eq(r2.new,r22)) jump:t 00018784; r2 = memw(r17+20); r1 = r20 }

l0001877C:
	{ jump 00018760; r3 = add(PC,00000000) }

l00018784:
	{ memw(r29) = r17; r4 = r18; r1 = r19; r2 = r16 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00018798:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; requantrange_execute: 000187A4
requantrange_execute proc
	{ allocframe(00000038); r6 = 2F800000 }
	{ memd(r29+48) = r17:r16; r17:r16 = combine(r1,r0); r4 = add(PC,0000F6FA) }
	{ memd(r29+24) = r23:r22; r2 = memw(r16+4); r1 = 000000E5 }
	{ memd(r29+16) = r25:r24; r5 = memw(r16+8) }
	{ memd(r29+40) = r19:r18; r3 = memw(r2) }
	{ r24 = memw(r2+4); r23 = memw(r2+8) }
	{ memd(r29+32) = r21:r20 }
	{ r8 = memw(r3+4); r7 = memw(r23+16) }
	{ r9 = memw(r3); r2 = memw(r24+16) }
	{ r7 = memw(r7); r12 = memw(r3+8); r8 = mpyi(r8,r9) }
	{ r13 = memw(r3+12); r14 = memw(r2); r2 = r17 }
	{ r19 = memw(r5+4); r21 = memw(r3+16); r3 = sfsub(r7,r14) }
	{ memw(r29) = r16; r18 = memw(r5); r5 = mpyi(r8,r12) }
	{ r22 = mpyi(r5,r13) }
	{ call logmsg_function; r20 = sfmpy(r3,r6) }
	{ r3 = memw(r24+16); r2 = memw(r23+16); r4 = add(PC,0000F6A5) }
	{ r5 = memw(r3); r2 = memw(r2); r1 = 000000E8 }
	{ r2 = r17; r9:r8 = convert_sf2df(r5); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r4 = add(r22,FFFFFFFF); if (!p0.new) jump:nt 0001886C; p0 = cmp.eq(r14,00000000) }

l00018858:
	{ r3 = 4F000000; r2 = CF000000 }
	{ jump 000188CC }

l0001886C:
	{ r2 = 7FFFFFFF; r3 = 80000000 }
	{ r5 = memw(r21++#4); if (p0.new) jump:nt 00018888; p0 = cmp.gtu(r14,00000001) }

l00018884:
	{ jump 000188BC; r4 = r5 }

l00018888:
	{ r4 = memw(r21++#4); p0 = cmp.gtu(r4,00000001); r6 = add(r4,FFFFFFFF) }
	{ if (!p0) jump:nt 000188B4; loop0(0001889C,r6) }

l0001889C:
	{ r4 = memw(r21++#4); r6 = r4; r2 = min(r2,r5); r3 = max(r5,r3) }
	{ nop; r5 = r6 }

l000188B4:
	{ r2 = min(r2,r5); r3 = max(r5,r3) }

l000188BC:
	{ r2 = min(r2,r4); r5 = max(r4,r3) }
	{ r3 = convert_w2sf(r2) }
	{ r2 = convert_w2sf(r5) }

l000188CC:
	{ r5 = memw(r18+16); r6 = 00000000; r3 = sfmpy(r20,r3) }
	{ memw(r18+8) = 00000001; memw(r18+12) = 00000001; r1 = 000000FE; r7 = sfmin(r6,r3) }
	{ memw(r18) = 00000001; r2 = sfmpy(r20,r2); r4 = add(PC,0000F604) }
	{ memw(r18+4) = FFFFFF81 }
	{ memw(r18+24) = 00000004; memw(r5) = r7 }
	{ memw(r19+12) = 00000001; r3 = memw(r19+16) }
	{ memw(r19+4) = 00000001; memw(r19) = 00000001 }
	{ memw(r3) = r2; memw(r19+8) = 00000001 }
	{ memw(r19+24) = 00000004; r0 = memw(r19+16) }
	{ r5 = memw(r18+16) }
	{ r2 = memw(r0) }
	{ r3 = memw(r5); r7:r6 = convert_sf2df(r2) }
	{ memd(r29+8) = r7:r6; r2 = r17; r9:r8 = convert_sf2df(r3) }
	{ memd(r29) = r9:r8; call logmsg_function }
	{ r2 = r17; r1 = 000000FF; r4 = add(PC,0000F5D0) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ dealloc_return; r25:r24 = memd(r29+16) }

;; requantrange_check: 00018960
requantrange_check proc
	{ jump requantize_check__merged; r1 = 00000001 }

;; logmsg_function: 00018964
;;   Called from:
;;     000180D4 (in autorequantize_execute)
;;     00018104 (in autorequantize_execute)
;;     0001832C (in autorequantize_execute)
;;     00018340 (in autorequantize_execute)
;;     00018374 (in autorequantize_check)
;;     00018408 (in autorequantize_check)
;;     000184B4 (in requantize_execute)
;;     000184E8 (in requantize_execute)
;;     000186A8 (in requantize_execute)
;;     000186BC (in requantize_execute)
;;     00018740 (in requantize_check__merged)
;;     00018790 (in requantize_check__merged)
;;     00018818 (in requantrange_execute)
;;     0001884C (in requantrange_execute)
;;     0001892C (in requantrange_execute)
;;     00018944 (in requantrange_execute)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00018988; r5 = memw(r2+16) }

l00018974:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000003B) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00018988:
	{ dealloc_return }

l0001898C:
	{ nop }

;; errlog_function: 00018990
;;   Called from:
;;     00018120 (in autorequantize_execute)
;;     000183E8 (in autorequantize_check)
;;     00018508 (in requantize_execute)
;;     0001875C (in requantize_check__merged)
;;     0001898C (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000F49B) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; autorequantize_execute.extracted_region: 000189B4
;;   Called from:
;;     000182A0 (in autorequantize_execute)
;;     00018600 (in requantize_execute)
autorequantize_execute.extracted_region proc
	{ allocframe(+00000020) }
	{ memd(r29+8) = r21:r20; r6 = memd(r29+40); r20 = 00000000; r21 = togglebit(r4,0000001E) }
	{ memd(r29+24) = r17:r16 }
	{ memd(r29+16) = r19:r18; r19:r18 = combine(r1,r0); r17:r16 = combine(r5,r3) }
	{ memd(r29) = r23:r22; r22 = sub(r6,r2) }

l000189DC:
	{ r2 = memw(r18++#4); r3 = r21 }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r2,r16) }
	{ call fn00009620; r0 = sfmpy(r3,r17) }
	{ r22 = add(r22,FFFFFFFF); r2 = convert_uw2sf(r0):chop }
	{ p1 = cmp.eq(r22,00000000) }
	{ if (p2.new) r2 = FFFFFFFF; p0 = cmp.gt(r2,FFFFFFFF); p2 = cmp.gt(r2,000000FF) }
	{ if (!p0) r2 = add(r20,00000000) }
	{ memb(r19++#1) = r2; if (!p1) jump:nt 000189DC }

l00018A18:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }
	{ r23:r22 = memd(r29); r21:r20 = memd(r29+8) }
	{ dealloc_return }

;; fmaxf.1.0: 00018A24
;;   Called from:
;;     000181E8 (in autorequantize_execute)
;;     0001827C (in autorequantize_execute)
;;     00018560 (in requantize_execute)
;;     000185DC (in requantize_execute)
fmaxf.1.0 proc
	{ r2 = 38D1B717 }
	{ r1:r0 = combine(r0,r2); jump fn00009600 }
00018A34             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; supernode_execute_hvx: 00018A40
;;   Called from:
;;     000191B8 (in supernode_execute_hvx)
supernode_execute_hvx proc
	{ allocframe(+00000170); memd(r29-16) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+320) = r27:r26; r2 = memw(r17+4) }
	{ r27 = memb(r17+32); r3 = memw(r17+8) }
	{ r4 = memw(r2); r6 = memw(r17+8); p0 = cmp.eq(r27,00000000) }
	{ memd(r29+344) = r21:r20; r5 = memw(r2+4) }
	{ memd(r29+336) = r23:r22 }
	{ memd(r29+328) = r25:r24; r7 = memw(r2+24) }
	{ memw(r29+136) = r6; r6 = memw(r2+36) }
	{ r20 = memw(r2+20); r22 = memw(r2+12) }
	{ r21 = memw(r2+32); r25 = memw(r2+28) }
	{ r23 = memw(r2+16); r24 = memw(r2+8) }
	{ memw(r29+108) = r6; r2 = memw(r4+4) }
	{ r6 = memw(r3+4); r1 = memw(r3) }
	{ memw(r29+144) = r2; r2 = memw(r5+8) }
	{ memd(r29+352) = r19:r18; r0 = memw(r4+8) }
	{ memw(r29+80) = r1 }
	{ memw(r29+72) = r6; r6 = memw(r4) }
	{ memw(r29+132) = r2; r2 = memw(r7+4) }
	{ r19 = memw(r5); r18 = memw(r5+12) }
	{ r1 = memw(r7+8); r26 = memw(r5+4) }
	{ memw(r29+140) = r6; r3 = memw(r3+8) }
	{ r6 = memw(r4+12) }
	{ memw(r29+76) = r7; memw(r29+148) = r2; r2 = p0 }
	{ memw(r29+68) = r3; memw(r29+128) = r0 }
	{ memw(r29+116) = r2; memw(r29+96) = r6 }
	{ if (p0) jump:nt 00018B24 }

l00018AF0:
	{ if (p0.new) r2 = memw(r29-128); p0 = cmp.eq(r27,00000002); if (p0.new) jump:nt 00018B1C }

l00018AFC:
	{ memw(r29+120) = r1; r0 = 00000000; p0 = cmp.eq(r27,00000001); if (!p0.new) jump:nt 00018B2C }

l00018B08:
	{ r2 = memw(r29+128); r1 = memw(r29+120) }
	{ r0 = r1 }
	{ jump 00018B24; r0 += add(r2,FFFFFFFF) }

l00018B1C:
	{ r2 = sub(r2,r26) }
	{ r0 = add(r2,r1) }

l00018B24:
	{ memw(r29+120) = r1; call fn00009760 }

l00018B2C:
	{ if (p0.new) r2 = memw(r29-112); if (p0.new) r1 = memw(r29-108); p0 = cmp.eq(r27,00000002); if (p0.new) jump:nt 00018B7C }

l00018B3C:
	{ if (p0.new) r1 = memw(r29-108); if (p0.new) r2 = memw(r29-112); p0 = cmp.eq(r27,00000001); if (p0.new) jump:nt 00018B6C }

l00018B4C:
	{ memw(r29+124) = r16; r0 = memd(r29+116); r5 = 00000000; r16 = r0 }
	{ if (p0.new) r1 = memw(r29-108); if (p0.new) r0 = memw(r29-112); if (!p0.new) jump:nt 00018B90; p0 = r0 }

l00018B68:
	{ jump 00018B88 }

l00018B6C:
	{ memw(r29+124) = r16; r16 = r0; r0 = r1 }
	{ jump 00018B88; r0 += add(r2,FFFFFFFF) }

l00018B7C:
	{ memw(r29+124) = r16; r16 = r0; r2 = sub(r2,r19) }
	{ r0 = add(r2,r1) }

l00018B88:
	{ call fn00009760 }
	{ r5 = r0 }

l00018B90:
	{ r2 = memw(r29+140); r0 = add(r29,000000E8); r1 = 00000000; r4 = add(r18,0000001F) }
	{ memw(r29+100) = r18; r6 = memw(r25+16); r27 = add(r29,000000E8); r4 = and(r4,FFFFFFE0) }
	{ r2 = 00000050; r3 = mpyi(r18,r2) }
	{ memw(r29+104) = r4; memw(r29+56) = r6; r18 = r5; r3 = mpyi(r3,r16) }
	{ r3 = mpyi(r3,r5) }
	{ memw(r29+116) = r3; call fn000095F0 }
	{ memw(r29+232) = r17; r1 = 00000000; r0 = add(r27,00000034) }
	{ memw(r29+64) = r0; call fn00009740 }
	{ r0 = add(r29,00000098); r1 = 00000000; r25 = add(r29,00000098); r2 = 00000050 }
	{ call fn000095F0 }
	{ memw(r29+152) = r17; r1 = 00000000; r0 = add(r25,00000034); r2 = setbit(r25,00000004) }
	{ memw(r2) = 00000001; memw(r29+60) = r0; call fn00009740 }
	{ r3 = memw(r24+16); r2 = memw(r22+16); r24 = 437F0000 }
	{ r7 = memd(r29+108); r4 = memw(r21+16); r1 = r24 }
	{ r25 = memw(r3); r2 = memw(r2) }
	{ r6 = memw(r23+16); r5 = memw(r20+16); r0 = sfsub(r2,r25) }
	{ memw(r29+84) = r2; r7 = memw(r7+16) }
	{ r21 = memw(r5); r20 = memw(r6) }
	{ memw(r29+88) = r20 }
	{ memw(r29+92) = r21; r27 = memw(r4) }
	{ r23 = memw(r7); call fn00009610 }
	{ r22 = r0; r2 = sfsub(r21,r20) }
	{ r1:r0 = combine(r24,r2); call fn00009610 }
	{ r21 = r0; r20 = sfsub(r23,r27) }
	{ r1:r0 = combine(r24,r20); call fn00009610 }
	{ r22 = sfmpy(r22,r21) }
	{ r1 = r22; call fn00009610 }
	{ memw(r29+52) = r0; r1:r0 = combine(r22,r23); call fn00009610 }
	{ call fn000097E0 }
	{ r3 = memw(r29+136); r0 = r20; r2 = r0 }
	{ r20 = convert_uw2sf(r2):chop }
	{ r23 = memw(r3+4); call fmaxf.1.0 }
	{ r1:r0 = combine(r0,r24); call fn00009610 }
	{ r2 = 00000000 }
	{ r2 = sfsub(r2,r27) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r13 = memw(r29+120); r3 = memw(r29+132); r9 = asl(r19,00000001); r2 = mpyi(r26,r19) }
	{ r12 = memw(r29+96); r5 = 00000000; r4 = convert_uw2sf(r0):chop; r7 = add(00000003,mpyi(r18,r16)) }
	{ p2 = cmp.eq(r13,00000002); r0 = 00000001; r8 = add(r12,0000000F); r2 = add(0000000F,mpyi(r2,r3)) }
	{ r2 = and(r7,FFFFFFFC); p0 = cmp.eq(r12,00000003); r3 = and(r2,FFFFFFF0); r15 = max(r4,r5) }
	{ p3 = cmp.eq(r3,00000020); p1 = cmp.eq(r3,000000A0); r8 = and(r8,FFFFFFF0) }
	{ p1 = fastcorner9(p2,p1) }
	{ p2 = fastcorner9(p2,p3) }
	{ p1 = fastcorner9(p0,p1) }
	{ p2 = cmp.eq(r19,00000007); p0 = fastcorner9(p0,p2) }
	{ p2 = cmp.eq(r19,00000003); p1 = fastcorner9(p2,p1) }
	{ p2 = cmp.eq(r26,00000007); p0 = fastcorner9(p2,p0) }
	{ p2 = cmp.eq(r26,00000003); p1 = fastcorner9(p2,p1) }
	{ if (!p0.new) r7 = memw(r29-128); if (!p0.new) r14 = memw(r29-112); if (!p0.new) jump:nt 00018D60; p0 = or(p1,and(p0,p2)) }

l00018D48:
	{ memw(r29+112) = r18; r4 = r18; r5 = asl(r19,00000001); r3 = mpyi(r16,r3) }
	{ r4 += add(r5,00000002) }
	{ jump 00018DA8; r3 = mpyi(r3,r4) }

l00018D60:
	{ r5 = r18; r3 = r16; r6 = sub(r19,r14); r4 = sub(r26,r7) }
	{ memw(r29+112) = r18; r1 = memw(r29+148) }
	{ r4 = r7; r5 = add(r6,mpyi(r5,r1)); r3 = add(r4,mpyi(r3,r13)) }
	{ r7 = r5; r3 += lsr(r3,0000001F) }
	{ r4 += asr(r3,r0); r7 += lsr(r7,0000001F) }
	{ r7 = clrbit(r7,00000000); r4 = mpyi(r4,r8) }
	{ r7 = sub(r5,r7); r5 += add(r9,r14) }
	{ r1 = sub(r5,r7) }
	{ r3 = mpyi(r4,r1) }

l00018DA8:
	{ r27 = memw(r29+100); r7 = memw(r29+104); p1 = !cmp.eq(r26,00000001); r2 = asl(r2,00000002) }
	{ r1 = memd(r29+124); r5 = memd(r29+116); p3 = cmp.eq(r27,r7); p2 = !cmp.eq(r19,00000001) }
	{ r7 = memw(r1+4); r5 = add(r5,r27); p1 = or(p2,p1); r6 = mpyi(r2,r7) }
	{ r3 += add(r2,r6) }
	{ r1 = addasl(r3,r5,00000002) }
	{ if (cmp.eq(r4.new,00000002)) jump:t 00018E14; r4 = memb(r17+32); r21 = add(r1,00000580) }

l00018DEC:
	{ if (!p0.new) jump:nt 00018E20 }

l00018DF0:
	{ memw(r29+44) = r15; memw(r29+40) = r7; r24 = r13; p0 = and(p0,p0) }
	{ memw(r29+108) = r16; memw(r29+48) = r22; r18 = r12; r0 = p0 }
	{ memw(r29+36) = r0; memw(r29+32) = r20 }
	{ jump 00018E38 }

l00018E14:
	{ nop; if (p0) jump:nt 00018DF0 }

l00018E1C:
	{ memw(r29+44) = r15; r18 = r12; r24 = r13; r0 = p3 }

l00018E20:
	{ memw(r29+44) = r15; r18 = r12; r24 = r13 }

l00018E2C:
	{ memw(r29+40) = r7; memw(r29+36) = r0 }
	{ memw(r29+32) = r20; memw(r29+48) = r22 }
	{ memw(r29+108) = r16 }

l00018E38:
	{ r20 = memd(r29+84); r2 = memw(r17+28); r4 = add(PC,0000F6E7) }
	{ r16 = memw(r29+124); r1 = 000003D8 }
	{ memw(r29+4) = r2; r22 = memd(r29+88); r3:r2 = combine(00000002,r16) }
	{ memw(r29) = r17; call logmsg_function }
	{ r1 = 000003D9; r20 = r18; r9:r8 = convert_sf2df(r25); r7:r6 = convert_sf2df(r20) }
	{ r5 = memw(r29+128); r3:r2 = combine(00000002,r16); r4 = add(PC,0000F8EE) }
	{ memd(r29+24) = r7:r6; r18 = memw(r29+140) }
	{ memw(r29+8) = r5; r7 = memw(r29+144) }
	{ memw(r29+12) = r20; memd(r29+16) = r9:r8 }
	{ memw(r29) = r18; memw(r29+4) = r7 }
	{ call logmsg_function }
	{ r2 = memw(r29+92); r1 = 000003DA; r9:r8 = convert_sf2df(r22) }
	{ memd(r29+16) = r9:r8; r22 = memw(r29+132); r4 = add(PC,0000F8D6) }
	{ memw(r29+4) = r19; memw(r29+12) = r22; r3:r2 = combine(00000002,r16); r7:r6 = convert_sf2df(r2) }
	{ memw(r29+8) = r26; memd(r29+24) = r7:r6 }
	{ memw(r29) = r27; call logmsg_function }
	{ r1 = 000003DB; r3:r2 = combine(00000002,r16); r4 = add(PC,0000F6A3) }
	{ memw(r29+4) = r24; r5 = memw(r29+148) }
	{ memw(r29) = r5; call logmsg_function }
	{ r1 = 000003DC; r3:r2 = combine(00000002,r16); r4 = add(PC,0000F69A) }
	{ memb(r29) = r5.new; r5 = memb(r17+32); call logmsg_function }
	{ memw(r29+12) = r27; r7 = memw(r29+108); r4 = add(PC,00000013) }
	{ memw(r29) = r18; r3:r2 = combine(00000002,r16) }
	{ memw(r29+8) = r7; r5 = memd(r29+112); r1 = 000003DD }
	{ memw(r29+4) = r5 }
	{ call logmsg_function }
	{ r19 = memw(r29+136); r25 = r23; r4 = add(PC,0000F869) }
	{ r5 = memw(r19+8); r3:r2 = combine(00000002,r16); r1 = 000003DF; r7:r6 = convert_sf2df(r25) }
	{ memd(r29) = r7:r6; r8 = add(PC,0000F85D) }
	{ r9 = add(PC,0000F862) }
	{ if (!p0.new) r8 = add(r9,00000000); p0 = !cmp.eq(r5,00000000) }
	{ memw(r29+8) = r8; call logmsg_function }
	{ p0 = cmp.eq(r20,r22); r1 = 000003E0; r3 = add(PC,0000F646) }
	{ if (!p0) jump:nt 0001903C }

l00018F90:
	{ if (!cmp.gtu(r21,r2.new)) jump:t 00018FB0; r2 = memw(r16+8) }

l00018F9C:
	{ memw(r29+4) = r2; r1 = 000003E2; r3 = add(PC,0000003F) }
	{ memw(r29) = r21; r2 = r16 }
	{ jump 00019040 }

l00018FB0:
	{ r24 = memw(r29+116); r2 = memw(r29+80) }
	{ if (!cmp.gtu(r24,r4.new)) jump:t 00018FDC; r4 = memw(r2+20); r21 = r24 }

l00018FC8:
	{ memw(r29+4) = r24; r1 = 000003E5; r3 = add(PC,0000002D) }
	{ memw(r29) = r4; r2 = r16 }
	{ jump 00019040 }

l00018FDC:
	{ r2 = memw(r29+76); r1 = 000003E7; r3 = add(PC,0000F62F) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 0001903C; r2 = memw(r2) }

l00018FF8:
	{ r2 = memw(r29+76); r1 = 000003E8; r3 = add(PC,00000028) }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 0001903C; r2 = memw(r2+12) }

l00019010:
	{ r2 = memd(r29+72); r4 = 00000004; r3 = add(PC,00000021) }
	{ r1 = 000003E9 }
	{ if (cmp.gtu(r4,r2.new)) jump:t 0001903C; r2 = memw(r2+20) }

l00019028:
	{ r2 = memw(r29+68); r1 = 000003EA; r3 = add(PC,00000017) }
	{ if (cmp.gtu(r2.new,00000003)) jump:t 0001904C; r2 = memw(r2+20) }

l0001903C:
	{ r2 = r16 }

l00019040:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00019218 }

l0001904C:
	{ r0 = memw(r29+36) }
	{ if (p0.new) r1 = 000003EB; if (!p0.new) jump:nt 0001906C; p0 = r0 }

l0001905C:
	{ r3:r2 = combine(00000002,r16); r4 = add(PC,0000F76B) }
	{ call logmsg_function }

l0001906C:
	{ r20 = memd(r29+48); r2 = memd(r29+104); r1 = 000003EC }
	{ r22 = memw(r29+40); p0 = cmp.eq(r27,r2) }
	{ if (!p0) jump:nt 00019090 }

l00019080:
	{ r3:r2 = combine(00000002,r16); r4 = add(PC,0000F760) }
	{ call logmsg_function }

l00019090:
	{ r7 = memd(r29+108); r2 = memd(r29+80); p0 = cmp.gt(r27,00000000) }
	{ r5 = memd(r29+56); r3 = memd(r29+112) }
	{ memw(r2+4) = r3; memw(r2+24) = r21 }
	{ memw(r2) = r18 }
	{ memw(r2+8) = r7; r6 = memd(r29+52) }
	{ r7 = memw(r29+44) }
	{ memw(r2+12) = r27; if (p0) r2 = 3F000000; if (!p0) jump:nt 000190DC }

l000190BC:
	{ loop0(000190C0,r27) }
	{ r3 = memb(r5++#1); r4 = r2 }
	{ r3 = sub(r3,r7) }
	{ r3 = convert_w2sf(r3) }
	{ r4 += sfmpy(r6,r3) }
	{ memw(r22++#4) = r1.new; r1 = convert_uw2sf(r4):chop }

l000190DC:
	{ r1:r0 = combine(r20,r25); call fn00009610 }

l000190E0:
	{ r1:r0 = combine(r20,r25) }

l000190E4:
	{ call fn00009620 }
	{ r2 = r0; r0 = 00FF0000 }
	{ call fn00009760; r1 = convert_uw2sf(r2):chop }
	{ r3 = r0; r2 = add(r29,000000E8); r21 = add(PC,00000C38) }
	{ memw(r29+172) = r3; memw(r29+252) = r3; r1:r0 = combine(r21,r16); call nn_os_work_for_vector }
	{ r1:r0 = combine(r21,r16); r2 = add(r29,00000098); call nn_os_work_for_vector }
	{ r0 = memw(r29+64); call fn000096A0 }
	{ r0 = memw(r29+60); call fn000096A0 }
	{ if (!cmp.eq(r2.new,00000001)) jump:t 0001914C; r2 = memw(r16+52) }

l0001913C:
	{ r3:r2 = memd(r29+224) }
	{ r1:r0 = add(r1:r0,r3:r2) }
	{ r1:r0 = lsr(r1:r0,00000001) }
	{ memd(r17+48) = r1:r0 }

l0001914C:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 000191C4; r2 = memw(r19+8) }

l00019158:
	{ r3 = memw(r29+240) }
	{ r7 = memw(r29+32); r2 = max(r3,r2) }
	{ r2 = add(r2,r7) }
	{ r2 = convert_w2sf(r2) }
	{ r3 = sfmpy(r20,r2) }
	{ if (!p0.new) jump:nt 000191C4; p0 = sfcmp.gt(r3,r25) }

l00019178:
	{ r2 = memw(r19+4) }
	{ if (!p0.new) jump:nt 00019194; p0 = sfcmp.gt(r3,r2) }

l00019184:
	{ r2 = sfadd(r2,r2) }
	{ if (p0.new) jump:nt 00019184; p0 = sfcmp.gt(r3,r2) }

l00019190:
	{ memw(r19+4) = r2 }

l00019194:
	{ r5 = memw(r17+28); r7:r6 = convert_sf2df(r2); r4 = add(PC,0000F664) }
	{ memw(r29) = r5; r1 = 00000419; r3:r2 = combine(00000001,r16) }
	{ memd(r29+8) = r7:r6; call logmsg_function }
	{ r1:r0 = combine(r16,r17); call supernode_execute_hvx }
	{ jump 00019218 }

l000191C4:
	{ r5 = memd(r29+68); r3 = memd(r29+72); r4 = add(PC,0000F667) }
	{ r7 = memw(r29+108); r1 = 00000435 }
	{ memw(r3+12) = 00000001; r2 = memw(r3+16) }
	{ memw(r3+4) = 00000001; memw(r3+8) = 00000001 }
	{ memw(r2) = 00000000; memw(r3) = 00000001 }
	{ memw(r5) = 00000001; memw(r3+24) = 00000004 }
	{ memw(r5+8) = 00000001; r2 = memw(r5+16) }
	{ memw(r5+4) = 00000001; memw(r5+12) = 00000001 }
	{ memw(r5+24) = 00000004; memw(r2) = r25; r3:r2 = combine(00000002,r16) }
	{ memw(r29+12) = r27; r5 = memw(r29+112) }
	{ memw(r29+4) = r5; memw(r29+8) = r7 }
	{ memw(r29) = r18; call logmsg_function }
	{ r0 = 00000000 }

l00019218:
	{ r19:r18 = memd(r29+352); r17:r16 = memd(r29+360) }
	{ r23:r22 = memd(r29+336); r21:r20 = memd(r29+344) }
	{ r27:r26 = memd(r29+320); r25:r24 = memd(r29+328) }
	{ dealloc_return }

;; supernode_check_ref: 00019234
supernode_check_ref proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r4 = add(PC,0000F43F) }
	{ r17 = r0; r16 = r1; r1 = 0000043D }
	{ memd(r29+32) = r21:r20; memd(r29+40) = r19:r18; r3:r2 = combine(00000002,r16) }
	{ memd(r29+16) = r25:r24; memd(r29+24) = r23:r22 }
	{ memw(r29) = r17; memd(r29+8) = r27:r26 }
	{ call logmsg_function }
	{ r2 = memw(r17+16); r1 = 0000043E; r3 = add(PC,0000F42A) }
	{ if (cmp.gtu(r4.new,r2)) jump:t 00019418; r4 = 0000000A; p0 = cmp.gtu(r2,0000000B) }

l00019284:
	{ r1 = 0000043F; r3 = add(PC,0000000E) }
	{ if (!p0) r1 = 00000440; if (p0) jump:nt 00019418 }

l00019294:
	{ r3 = add(PC,0000F413) }
	{ if (!cmp.eq(r4.new,00000003)) jump:t 00019418; r4 = memw(r17+20) }

l000192A8:
	{ r1 = 00000441; r3 = add(PC,0000001D) }
	{ if (cmp.eq(r24.new,00000000)) jump:nt 00019418; r24 = memw(r17+4) }

l000192BC:
	{ r1 = 00000442; r3 = add(PC,00000015) }
	{ if (cmp.eq(r4.new,00000000)) jump:nt 00019418; r4 = memw(r17+8) }

l000192D0:
	{ r21 = 00000000; r5 = 00000000 }
	{ r6 = 00000000; r3 = r24; loop0(000192E0,r2) }
	{ if (cmp.eq(r7.new,00000000)) jump:nt 000192F4; r7 = memw(r3) }

l000192EC:
	{ r6 = r6; r3 = add(r3,00000004) }
	{ jump 00019314; r13 = r2 }

l000192F4:
	{ memw(r29) = r6; r1 = 00000445; r3 = add(PC,0000F3E6) }
	{ jump 00019418 }

l00019308:
	{ if (cmp.gtu(r5.new,00000002)) jump:nt 00019330; r5 = add(r5,00000001); r4 = add(r4,00000004) }

l0001930C:
	{ if (cmp.gtu(r5.new,00000002)) jump:nt 00019334; r5 = add(r5,00000001) }

l00019314:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 00019308; r2 = memw(r4) }

l00019318:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001930C }

l00019320:
	{ memw(r29) = r5; r1 = 0000044A; r3 = add(PC,0000000C) }
	{ jump 00019418 }

l00019330:
	{ r3 = memw(r24+20); r2 = memw(r24+16) }

l00019334:
	{ r3 = memw(r24+20) }

l00019338:
	{ r4 = memw(r24+4) }
	{ r3 = memw(r3+16); r2 = memw(r2+16) }
	{ r22 = memw(r4+8); r18 = memw(r4+16) }
	{ r2 = memw(r3); r20 = memw(r2) }
	{ r25 = memw(r4); r23 = memw(r4+4); r0 = sfsub(r2,r20) }
	{ r19 = memw(r4+12); call fmaxf.1.0 }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = 00000000 }
	{ r2 = sfsub(r2,r20) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r2 = r0; r0 = 0000000C }
	{ call fn00009500; r26 = convert_uw2sf(r2):chop }
	{ r1 = 00000460; r2 = mpyi(r23,r25); r3 = add(PC,0000F36B) }
	{ if (cmp.eq(r20.new,00000000)) jump:t 00019418; r20 = r0; p0 = cmp.gt(r26,000000FF); p2 = cmp.eq(r21,0000000B) }

l000193B0:
	{ if (p0) r25 = 000000FF; p1 = cmp.gt(r26,FFFFFFFF); r3 = add(r19,0000001F) }
	{ if (!p2) memw(r20+8) = 00000000; if (!p0) r25 = zxtb(r26); r21 = and(r3,FFFFFFE0); r23 = mpyi(r2,r22) }
	{ if (!p2) memw(r20+4) = 3F000000; if (!p1) r25 = 00000000; r22 = and(r4,FFFFFFF0) }
	{ if (!p2) jump:nt 000193F8; r1 = mpyi(r22,r21) }

l000193E4:
	{ memw(r20+8) = 00000001 }
	{ r2 = memw(r24+40) }
	{ r2 = memw(r2+16) }
	{ r2 = memw(r2) }
	{ memw(r20+4) = r2 }

l000193F8:
	{ memw(r17+40) = r20; r0 = 00000080; call fn00009550 }
	{ memw(r20) = r0; r1 = 0000046B; r3 = add(PC,0000F30A) }
	{ if (!p0.new) jump:nt 00019428; p0 = cmp.eq(r0,00000000) }

l00019418:
	{ r2 = r16; call errlog_function }
	{ r0 = FFFFFFFF; jump 0001947C }

l00019428:
	{ call nn_os_vector_acquire }
	{ r3 = memw(r16+4); r1:r0 = combine(r23,r18); r5:r4 = combine(r21,r22); r24 = r0 }
	{ memw(r29) = r25; r2 = r19; call pad2d }
	{ r3 = memw(r20); r0 = memw(r16+4); r1 = r22; r2 = r21 }
	{ call transpack }
	{ r0 = r24; call nn_os_vector_release }
	{ r1 = 00000471; r3:r2 = combine(00000002,r16); r4 = add(PC,0000F2DF) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001947C:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }

;; supernode_dtor: 00019490
supernode_dtor proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29) = r19:r18 }
	{ if (cmp.eq(r18.new,00000000)) jump:nt 000194B4; r18 = memw(r16+40) }

l000194A8:
	{ r0 = memw(r18) }
	{ r0 = r18; call fn00009510 }

l000194B4:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r1:r0 = combine(r17,r16) }
	{ deallocframe; jump node_free_common }

;; supernode_execute_ref: 000194C4
supernode_execute_ref proc
	{ allocframe(+000000B8) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+136) = r27:r26; memd(r29+160) = r21:r20 }
	{ r7 = memw(r2+8); r4 = memw(r2) }
	{ r20 = memw(r2); r26 = memb(r0+32) }
	{ memw(r29+64) = r7; memw(r29+92) = r4; p0 = cmp.eq(r26,00000000) }
	{ r7 = memw(r2+20); r4 = memw(r2+12) }
	{ memd(r29+168) = r19:r18; r5 = memw(r2+24) }
	{ memw(r29+56) = r4 }
	{ memw(r29+88) = r7; r4 = memw(r2+28) }
	{ r18 = memw(r2+4); r7 = memw(r2+4) }
	{ memw(r29+96) = r4; memd(r29+176) = r17:r16 }
	{ memw(r29+84) = r7; r4 = memw(r3+8) }
	{ r3 = memw(r3+4); r7 = memw(r3) }
	{ memd(r29+152) = r23:r22; r16 = memw(r20+8) }
	{ memw(r29+112) = r0; memd(r29+144) = r25:r24 }
	{ memw(r29+72) = r1; r0 = r16 }
	{ memw(r29+48) = r3; r17 = memw(r2+16) }
	{ r19 = memw(r20); r2 = memw(r18) }
	{ r21 = memw(r20+12); r23 = memw(r20+4) }
	{ r22 = memw(r18+4); r24 = memw(r18+12) }
	{ r3 = memw(r5+4); r1 = memw(r5+8) }
	{ memw(r29+128) = r2; memw(r29+124) = r7; r2 = p0 }
	{ r7 = memw(r18+8) }
	{ memw(r29+44) = r4; memw(r29+52) = r5 }
	{ memw(r29+80) = r2; memw(r29+100) = r7; if (!p0) r2 = sub(r16,r22) }
	{ if (p0) jump:nt 0001957C }

l00019550:
	{ if (p0.new) memw(r29+104) = r1; if (!p0.new) r27 = 00000000; p0 = cmp.eq(r26,00000002); if (p0.new) jump:nt 00019578 }

l00019560:
	{ if (p0.new) r1 = memw(r29+104); p0 = cmp.eq(r26,00000001); if (!p0.new) jump:nt 00019590 }

l0001956C:
	{ r0 = r1 }
	{ jump 0001957C; r0 += add(r16,FFFFFFFF) }

l00019578:
	{ r0 = add(r2,r1) }

l0001957C:
	{ memw(r29+104) = r1; r27 = r3; call fn00009760 }
	{ r27 = r0; r3 = r27 }

l00019590:
	{ if (p0.new) r26 = add(r23,00000000); p0 = cmp.eq(r26,00000002); r1:r0 = combine(r3,r3); if (p0.new) jump:nt 000195D0 }

l000195A0:
	{ if (p0.new) r26 = add(r23,00000000); p0 = cmp.eq(r26,00000001); if (p0.new) jump:nt 000195C8 }

l000195AC:
	{ r1 = memd(r29+80); r0 = 00000000; r23 = r3; r26 = r23 }
	{ if (!p0) r1:r0 = combine(r23,r26); if (!p0.new) jump:nt 000195E8; p0 = r1 }

l000195C4:
	{ jump 000195E4 }

l000195C8:
	{ jump 000195E0; r0 += add(r26,FFFFFFFF) }

l000195D0:
	{ r2 = memw(r29+128); r1 = r3 }
	{ r2 = sub(r26,r2) }
	{ r0 = add(r2,r3) }

l000195E0:
	{ r23 = r3 }

l000195E4:
	{ call fn00009760 }

l000195E8:
	{ r2 = memd(r29+56); r3 = memd(r29+64); r4 = r19 }
	{ r6 = memd(r29+88); r1 = memd(r29+92); r19 = 437F0000 }
	{ r2 = memw(r2+16); r3 = memw(r3+16) }
	{ memw(r29+68) = r4; r7 = memd(r29+84); r4 = mpyi(r27,r4) }
	{ r3 = memw(r1+16); r12 = memw(r3); r1 = r19 }
	{ r2 = memw(r2); r8 = memw(r29+96) }
	{ r13 = memw(r29+124); r5 = memw(r17+16) }
	{ r17 = memw(r3); r9 = memw(r8+16); r3 = mpyi(r4,r0) }
	{ r7 = memw(r7+16); r6 = memw(r6+16); r25 = mpyi(r3,r24) }
	{ memw(r29+80) = r0; r8 = memw(r29+72); r0 = sfsub(r2,r12) }
	{ r14 = memw(r18+16) }
	{ memw(r29+76) = r23; memw(r29+60) = r26; r26 = r0 }
	{ memw(r29+108) = r27; r27 = memw(r20+16) }
	{ memw(r29+56) = r9; r15 = memw(r13+16) }
	{ r9 = memw(r8+4) }
	{ memw(r29+36) = r15; memw(r29+64) = r12 }
	{ memw(r29+96) = r14 }
	{ memw(r29+40) = r9; r18 = memw(r6) }
	{ r23 = memw(r5) }
	{ memw(r29+28) = r3; r20 = memw(r7); call fn00009610 }
	{ memw(r29+84) = r0; r2 = sfsub(r18,r23) }
	{ memw(r29+88) = r2; r1:r0 = combine(r19,r2); call fn00009610 }
	{ r18 = r0; r2 = sfsub(r20,r17) }
	{ memw(r29+92) = r2; r1:r0 = combine(r19,r2); call fn00009610 }
	{ r2 = memw(r29+84) }
	{ memb(r29+8) = r18.new; r18 = sfmpy(r2,r18) }
	{ r1 = r18 }
	{ memw(r29+84) = r0; r1:r0 = combine(r18,r20); call fn00009610 }
	{ r0 = r26; r2 = r0 }
	{ memb(r29+6) = r2.new; call fmaxf.1.0; r2 = convert_uw2sf(r2):chop }
	{ r1:r0 = combine(r0,r19) }
	{ r2 = memd(r29+64); r18 = 00000000 }
	{ r2 = sfsub(r18,r2) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = memd(r29+88); r2 = r0 }
	{ memb(r29+22) = r2.new; call fmaxf.1.0; r2 = convert_uw2sf(r2):chop }
	{ r2 = 00000000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r20 = memw(r29+108); r2 = sfsub(r18,r23) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r0 = memd(r29+92); r2 = r0 }
	{ memb(r29+16) = r2.new; call fmaxf.1.0; r2 = convert_uw2sf(r2):chop }
	{ r2 = 00000000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r18,r17) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r19 = memd(r29+72); r17 = memd(r29+112); r4 = add(PC,0000EDC7) }
	{ r3:r2 = combine(00000002,r19); r1 = 000000EF; r18 = convert_uw2sf(r0):chop }
	{ memb(r29+1) = r5.new; r5 = memw(r17+28) }
	{ memw(r29) = r17 }
	{ memw(r29+12) = r21; r3:r2 = combine(00000002,r19); r4 = add(PC,0000EDC0) }
	{ r1 = 000000F0 }
	{ memw(r29+8) = r16; r5 = memd(r29+60); r23 = r19 }
	{ memw(r29+4) = r5; r19 = memd(r29+68) }
	{ memw(r29) = r19; call logmsg_function }
	{ r26 = memw(r29+100); r3:r2 = combine(00000002,r23); r4 = add(PC,0000EDB4) }
	{ memw(r29+12) = r26; r5 = memw(r29+128) }
	{ memw(r29+8) = r22; r1 = 000000F1 }
	{ memw(r29) = r24; memw(r29+4) = r5; call logmsg_function }
	{ r3:r2 = combine(00000002,r23); r4 = add(PC,0000EDA3) }
	{ r7 = memw(r29+104); r1 = 000000F2 }
	{ memw(r29+4) = r7; r5 = memd(r29+76) }
	{ memw(r29) = r5; call logmsg_function }
	{ r1 = 000000F3; r3:r2 = combine(00000002,r23); r4 = add(PC,0000ED9A) }
	{ memb(r29) = r5.new; r5 = memb(r17+32); call logmsg_function }
	{ memw(r29+12) = r24; r5 = memw(r29+80); r4 = add(PC,00000013) }
	{ memw(r29) = r19; memw(r29+4) = r5; r3:r2 = combine(00000002,r23) }
	{ memw(r29+8) = r20; r17 = r5; r1 = 000000F4 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r21,r26); r1 = 000000F5; r3 = add(PC,0000ED8E) }
	{ if (p0) r2 = memw(r23+8); if (p0) r3 = add(r25,r24); if (!p0) jump:nt 00019900 }
