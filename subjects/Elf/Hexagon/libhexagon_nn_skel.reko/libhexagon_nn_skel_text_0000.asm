;;; Segment .text (00009840)

;; hexagon_nn_skel_invoke: 00009840
hexagon_nn_skel_invoke proc
	{ immext(#0x218C0); r2 = add(PC,#0x218D0); memd(r29-16) = r17:r16; allocframe(#0x78) }
	{ r17:r16 = combine(r0,r1); memd(r29+88) = r23:r22; memd(r29+80) = r25:r24 }
	{ r3 = extractu(r17,#0x5,#0x1D); r18 = #0x14; memd(r29+104) = r19:r18; memd(r29+96) = r21:r20 }
	{ memd(r29+72) = r27:r26 }
	{ immext(#0xFFFFFF80); r21 = memw(r2-96); immext(#0xFFFFFF80); r20 = memw(r2-80) }
	{ p0 = cmp.gtu(r3,#0x14); immext(#0xFFFFFF80); r19 = memw(r2-128) }
	{ if (p0) jump:nt 0000A644; immext(#0xFFFFFF80); r2 = memw(r2-112) }

l00009890:
	{ immext(#0xDC0); r4 = add(PC,#0xDD8) }
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
	{ r17:r16 = memd(r29+112); r19:r18 = memd(r29+104) }
	{ r21:r20 = memd(r29+96); r23:r22 = memd(r29+88) }
	{ r25:r24 = memd(r29+80); r27:r26 = memd(r29+72) }
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
	{ r2 = #0x80; immext(#0x60000000); r3 = #0x60000000; allocframe(#0x20) }
	{ r4 = #0x80; r16 = r0; memd(r29+24) = r17:r16; memd(r29+16) = r19:r18 }
	{ r1 = add(r16,#0x28) }
	{ call prefree; memd(r29+8) = r21:r20; memw(r16+48) = r4 }
	{ r17 = memw(r16) }

l0000A6EC:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 0000A7C0; if (p0.new) r0 = add(r16,#0x0) }

l0000A6F4:
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000A7B8 }

l0000A700:
	{ r3 = memw(r17+8) }

l0000A704:
	{ r3 = memw(r29+r20) }
	{ r4 = memw(r3+20); if (cmp.eq(r4.new,#0x0)) jump:nt 0000A7AC }

l0000A714:
	{ if (!cmp.eq(r3.new,#0x0)) jump:t 0000A7B0 }

l0000A71C:
	{ r2 = r18; r1:r0 = combine(r17,r16) }
	{ immext(#0x209C0); r2 = add(PC,#0x209EC); r19 = r0; r5:r4 = combine(#0x0,#0x0) }
	{ r1:r0 = combine(#0x0,r16) }
	{ r3:r2 = combine(#0x0,#0xD); immext(#0xFFFFFFC0); r6 = memw(r2-16) }
	{ r6 = memw(r6+52) }
	{ r6 = memw(r6+8); memw(r29+4) = #0xFFFFFF80 }
	{ callr r6; memw(r29) = #0x0 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000A794; if (!p0.new) r2 = memw(r17+8) }

l0000A75C:
	{ immext(#0x1AC00); r3 = add(PC,#0x1AC02); r1 = #0xF9; r2 = r16 }
	{ call errlog_function }
	{ immext(#0x1ABC0); r3 = add(PC,#0x1ABE1); r1 = #0x110; r2 = r16 }
	{ call errlog_function }
	{ immext(#0x1AA40); r3 = add(PC,#0x1AA48); jump 0000A8E0; r1 = #0x14C }

l0000A794:
	{ r2 = memw(r13+r20) }
	{ r2 = add(r2,#0x1C); memb(r0+2) = r2.new }
	{ memw(r0+36) = r7; memw(r19+36) = r0 }
	{ r2 = memw(r17+20) }

l0000A7AC:
	{ r20 = add(r20,#0x4); r18 = add(r18,#0x1); if (cmp.gtu(r2,r18.new)) jump:t 0000A700 }

l0000A7B0:
	{ r18 = add(r18,#0x1); if (cmp.gtu(r2,r18.new)) jump:t 0000A704 }

l0000A7B8:
	{ jump 0000A6EC; r17 = memw(r17+36) }

l0000A7BC:
	{ r17 = memw(r17+36) }

l0000A7C0:
	{ call allocate_and_free }
	{ immext(#0x1AA00); r3 = add(PC,#0x1AA16); r1 = #0x14E; p0 = cmp.eq(r0,#0x0) }
	{ if (!p0) jump:nt 0000A8E0; if (p0) r2 = memw(r16) }

l0000A7DC:
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 0000A800; if (p0.new) r0 = add(r16,#0x0); if (!p0.new) r3 = memw(r2+24) }

l0000A7E8:
	{ p0 = cmp.eq(r3,#0xD) }
	{ if (p0) r3 = memw(r2+8) }
	{ if (p0) r3 = memw(r3) }
	{ if (p0) memw(r3+16) = #0x0 }
	{ jump 0000A7DC; r2 = memw(r2+36) }

l0000A800:
	{ call check_allocations }
	{ immext(#0x1A9C0); r3 = add(PC,#0x1A9E1); r1 = #0x155; p0 = cmp.eq(r0,#0x0) }
	{ if (!p0) jump:nt 0000A8E0; if (p0) r0 = add(r16,#0x0) }

l0000A81C:
	{ call check_allocations }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000A848; r1 = #0xCA }

l0000A828:
	{ immext(#0x1A9C0); r3 = add(PC,#0x1A9EB) }

l0000A830:
	{ call errlog_function; r2 = r16 }

l0000A834:
	{ r2 = r16 }
	{ immext(#0x1A980); r3 = add(PC,#0x1A9BD); jump 0000A8E0; r1 = #0x156 }

l0000A848:
	{ r1 = #0xCB; r2 = memw(r16+44); if (cmp.eq(r2.new,#0x0)) jump:nt 0000A860 }

l0000A858:
	{ r3 = add(PC,#0x5); jump 0000A834 }

l0000A860:
	{ r17 = memw(r16+48) }
	{ call fn00009500; r0 = r17 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000A884; if (p0.new) r1 = #0xCD; memw(r16+44) = r0 }

l0000A878:
	{ immext(#0x1A980); r3 = add(PC,#0x1A9BA); jump 0000A830 }

l0000A884:
	{ immext(#0x1A980); r4 = add(PC,#0x1A9BF); r3:r2 = combine(#0x2,r16); memw(r29+4) = r0 }
	{ r1 = #0xD1; memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = r16; r2 = memw(r16+12); r3 = memw(r16+8) }
	{ r4 = add(r2,#0x7F) }
	{ r4 = and(r4,#0xFFFFFF80); memb(r3+1) = r4.new }
	{ r2 = sub(r7,r2); r4 = memw(r3+4) }
	{ call allocate_and_free; r2 = add(r2,r4); memb(r3+2) = r2.new }
	{ r3 = add(PC,#0x37); r1 = #0x158; p0 = cmp.eq(r0,#0x0) }
	{ if (p0) jump:nt 0000A8F4 }

l0000A8E0:
	{ call errlog_function; r2 = r16; r17 = #-0x1 }

l0000A8E8:
	{ r0 = r17; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); dealloc_return }

l0000A8F4:
	{ r18 = r16; r17 = #0x0; r0 = memw(r16); if (cmp.eq(r0.new,#0x0)) jump:nt 0000A8E8 }

l0000A908:
	{ r18 = add(r0,#0x24) }

l0000A90C:
	{ r0 = memw(r18); if (cmp.eq(r0.new,#0x0)) jump:nt 0000A8E8 }

l0000A918:
	{ if (!cmp.eq(r2.new,#0xD)) jump:nt 0000A90C }

l0000A920:
	{ r7 = memw(r0+36); memb(r18) = r7.new }
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
	{ immext(#0x1A840); r0 = add(PC,#0x1A875); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; allocate_and_free: 0000A964
;;   Called from:
;;     0000A7C0 (in allocate_graph_storage)
;;     0000A8C0 (in allocate_graph_storage)
allocate_and_free proc
	{ r17:r16 = combine(r0,#0x0); memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ r19 = memw(r17); memd(r29+24) = r19:r18 }
	{ memd(r29+16) = r21:r20 }
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 0000AAD4; if (!p0.new) r18 = add(r17,#0x28); memd(r29+8) = r23:r22 }

l0000A980:
	{ r1:r0 = combine(r18,r17); r2 = memw(r19+24); if (cmp.eq(r2.new,#0xD)) jump:t 0000A9C8 }

l0000A984:
	{ r2 = memw(r19+24); if (cmp.eq(r2.new,#0xD)) jump:t 0000A9CC }

l0000A990:
	{ r2 = memw(r19+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000AAD0 }

l0000A998:
	{ r3 = memw(r19+8) }

l0000A99C:
	{ r21 = memw(r31+r20<<#2) }
	{ r3 = memw(r21+20); if (cmp.eq(r3.new,#0x0)) jump:nt 0000AAC4 }

l0000A9AC:
	{ if (!cmp.eq(r4.new,#0x0)) jump:t 0000AAC8 }

l0000A9B4:
	{ r4 = memw(r18); if (cmp.eq(r4.new,#0x0)) jump:nt 0000AAE8 }

l0000A9C0:
	{ jump 0000AA30; r2 = and(r2,#0xFFFFFF80) }

l0000A9C8:
	{ r2 = memw(r19+8) }

l0000A9CC:
	{ r3 = memw(r2) }
	{ r2 = memw(r3+16); r3 = memw(r3+20) }
	{ call prefree }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000A9F4; if (!p0.new) r1 = #0x123 }

l0000A9E0:
	{ immext(#0x1A8C0); r3 = add(PC,#0x1A8F7) }

l0000A9E8:
	{ call errlog_function; r2 = r17; r16 = #-0x1 }
	{ jump 0000AAD4 }

l0000A9F4:
	{ immext(#0x1A8C0); r4 = add(PC,#0x1A8F1); r1 = #0x127; r2 = memw(r19+8) }
	{ r3:r2 = combine(#0x3,r17); r5 = memw(r2) }
	{ r6 = memw(r5+20) }
	{ r5 = memw(r5+16); memb(r29+1) = r5.new }
	{ memw(r29) = r6 }
	{ jump 0000AACC }

l0000AA24:
	{ r3 = r0; r4 = memw(r0) }
	{ p0 = cmp.eq(r4,#0x0); if (p0.new) jump:nt 0000AAE4; nop }

l0000AA30:
	{ r0 = r4 }
	{ r4 = memw(r0+8); if (cmp.gtu(r2,r4.new)) jump:t 0000AA24 }

l0000AA40:
	{ p0 = cmp.eq(r4,r2); if (!p0.new) jump:nt 0000AA58; if (!p0.new) r3 = sub(r4,r2) }

l0000AA48:
	{ call fn00009510; r2 = memw(r0); memb(r3) = r2.new }

l0000AA58:
	{ r2 = add(r22,r2); memb(r0+1) = r2.new }
	{ r3 = memw(r17+48); if (!cmp.gtu(r2,r3.new)) jump:t 0000AA8C }

l0000AA70:
	{ r4 = add(PC,#0x2A); r3:r2 = combine(#0x3,r17); memw(r17+48) = r2 }
	{ call logmsg_function; r1 = #0x66; r5 = memw(r0+4); memb(r29) = r5.new }

l0000AA8C:
	{ p0 = cmp.eq(r14,#0x0); if (!p0.new) jump:nt 0000AAA8; if (!p0.new) r1 = #0x135; memw(r21+16) = r22 }

l0000AA90:
	{ if (!p0.new) r1 = #0x135; memw(r21+16) = r22 }

l0000AA98:
	{ immext(#0x1A840); r3 = add(PC,#0x1A85D); jump 0000A9E8; r1 = #0x131 }

l0000AAA8:
	{ immext(#0x1A840); r4 = add(PC,#0x1A85A); r5 = memw(r21+20); memw(r29+4) = r22 }
	{ r3:r2 = combine(#0x3,r17); memw(r29) = r5 }
	{ call logmsg_function }
	{ r2 = memw(r19+20) }

l0000AAC4:
	{ r20 = add(r20,#0x1); if (cmp.gtu(r2,r20.new)) jump:t 0000A998 }

l0000AAC8:
	{ if (cmp.gtu(r2,r20.new)) jump:t 0000A99C }

l0000AACC:
	{ r19 = memw(r19+36); if (!cmp.eq(r19.new,#0x0)) jump:t 0000A980 }

l0000AAD0:
	{ if (!cmp.eq(r19.new,#0x0)) jump:t 0000A984 }

l0000AAD4:
	{ r0 = r16; r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }

l0000AAD8:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

l0000AAE4:
	{ jump 0000AA98; memw(r21+16) = #0x0 }

l0000AAE8:
	{ memw(r21+16) = #0x0 }

;; check_allocations: 0000AAEC
;;   Called from:
;;     0000A800 (in allocate_graph_storage)
;;     0000A81C (in allocate_graph_storage)
;;     0000AAE8 (in allocate_and_free)
check_allocations proc
	{ immext(#0x1A7C0); r4 = add(PC,#0x1A7C6); r1 = #0xB8; allocframe(#0x18) }
	{ r16 = r0; memd(r29+16) = r17:r16 }
	{ r3:r2 = combine(#0x3,r16); r5 = memw(r16+40) }
	{ r6 = memw(r5+8); r7 = memw(r5+4) }
	{ memw(r29+8) = r6; memw(r29+4) = r7 }
	{ call logmsg_function; memw(r29) = r5 }
	{ r2 = memw(r16+40); if (cmp.eq(r2.new,#0x0)) jump:nt 0000AB38 }

l0000AB24:
	{ r3 = add(r3,#0xFFFFFFFF); r4 = memw(r4); if (!cmp.eq(r4.new,#0x0)) jump:t 0000AB24 }

l0000AB34:
	{ if (p0.new) r2 = memw(r2+8) }

l0000AB38:
	{ immext(#0x1A700); r3 = add(PC,#0x1A739); r1 = #0xBE; r2 = r16 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+16); dealloc_return }
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
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r0 = memw(r16+40); if (cmp.eq(r0.new,#0x0)) jump:nt 0000ABC4 }

l0000ABB4:
	{ call fn00009510; r17 = memw(r0) }

l0000ABB8:
	{ r17 = memw(r0) }

l0000ABBC:
	{ r0 = r17; p0 = cmp.eq(r17,#0x0) }
	{ if (!p0) jump:nt 0000ABB4 }

l0000ABC4:
	{ r0 = memw(r16+12); memw(r16+40) = #0x0 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000ABD4; nop }

l0000ABD0:
	{ r17:r16 = memd(r29); dealloc_return }

l0000ABD4:
	{ jump fn00009510; r17:r16 = memd(r29); deallocframe }

;; logmsg_function: 0000ABDC
;;   Called from:
;;     0000A89C (in allocate_graph_storage)
;;     0000AA7C (in allocate_and_free)
;;     0000AABC (in allocate_and_free)
;;     0000AB10 (in check_allocations)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000ABFC }

l0000ABEC:
	{ r0 = add(PC,#0xD); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0000ABFC:
	{ dealloc_return }

;; prefree: 0000AC00
;;   Called from:
;;     0000A6DC (in allocate_graph_storage)
;;     0000A9D4 (in allocate_and_free)
prefree proc
	{ r3 = add(r3,#0x7F); r17:r16 = combine(r2,r1); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r18 = r0; r19 = and(r3,#0xFFFFFF80); r4 = memw(r16); memd(r29) = r19:r18 }

l0000AC18:
	{ p0 = cmp.eq(r4,#0x0); if (!p0.new) jump:nt 0000AC50; r2 = r4 }

l0000AC20:
	{ call fn00009500; r0 = #0xC }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000ACB4; if (p0.new) r17 = #0xFFFFFFFF }

l0000AC30:
	{ immext(#0x1A700); r3 = add(PC,#0x1A70D); r1 = #0xA0; r2 = r18 }
	{ call errlog_function }
	{ jump 0000ACC4 }

l0000AC48:
	{ r8 = r2; jump 0000AC18; r4 = memw(r2) }

l0000AC50:
	{ r3 = memw(r2+8); r4 = memw(r2+4) }
	{ r5 = add(r3,r4); if (cmp.gtu(r17,r5.new)) jump:t 0000AC48 }

l0000AC60:
	{ r3 = add(r3,r19); r17 = #0x0; r0 = memw(r2); memb(r2+2) = r3.new }
	{ if (!p0.new) r5 = memw(r0+4) }
	{ r4 = add(r4,r3); if (!cmp.eq(r4.new,r5)) jump:t 0000ACC4 }

l0000AC84:
	{ r3 = add(r4,r3); memb(r2+2) = r3.new }
	{ call fn00009510; memw(r2) = r3 }
	{ jump 0000ACC4 }
0000AC9C                                     05 51 13 F3             .Q..
0000ACA0 C2 E4 72 20 11 40 00 78 29 01 B3 78 0C 40 00 58 ..r .@.x)..x.@.X
0000ACB0 02 C3 82 A1                                     ....            

l0000ACB4:
	{ r17 = #0x0; memw(r0+4) = r17; memw(r0+8) = r19 }
	{ r2 = memw(r16) }
	{ memw(r0) = r2; memw(r16) = r0 }

l0000ACC4:
	{ r0 = r17; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; do_execute: 0000ACD0
;;   Called from:
;;     0000B060 (in hexagon_nn_execute_new)
;;     0000B120 (in hexagon_nn_execute)
do_execute proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ memd(r29+16) = r19:r18; memd(r29+8) = r21:r20 }
	{ memd(r29) = r23:r22; memw(r16+28) = r2 }
	{ memw(r16+20) = r1; memw(r16+24) = r3 }
	{ call nn_os_hvx_power_on; memw(r16+32) = r4 }
	{ call nn_os_vector_workers_acquire; r0 = r16 }
	{ call fn00009530 }
	{ r19:r18 = combine(r1,r0); r17 = memw(r16); if (cmp.eq(r17.new,#0x0)) jump:nt 0000AD58 }

l0000AD08:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 0000AD58; if (p0.new) r20 = #0x0 }

l0000AD0C:
	{ if (p0.new) r20 = #0x0 }

l0000AD10:
	{ call nn_os_get_perfcount; r0 = r16 }
	{ r23:r22 = combine(r1,r0); r1:r0 = combine(r16,r17); r2 = memw(r17) }
	{ r2 = memw(r2) }
	{ callr r2 }
	{ r20 = r0; if (!cmp.eq(r20.new,#0x0)) jump:nt 0000AD58 }

l0000AD38:
	{ r0 = r16 }
	{ r1:r0 = sub(r1:r0,r23:r22); r3:r2 = memd(r17+48); memw(r17+44) += #0x1 }
	{ r1:r0 = add(r3:r2,r1:r0) }
	{ jump 0000AD08; r17 = memw(r17+36); memd(r17+48) = r1:r0 }

l0000AD58:
	{ call fn00009530 }
	{ r3:r2 = sub(r1:r0,r19:r18); r0 = r16 }
	{ call nn_os_vector_workers_release; memd(r16+80) = r3:r2 }
	{ call nn_os_hvx_power_off; r0 = r16 }
	{ r0 = r20; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); r23:r22 = memd(r29) }
	{ dealloc_return }
0000AD84             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; hexagon_nn_init: 0000AD90
hexagon_nn_init proc
	{ r1:r0 = combine(#0x58,#0x1); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ call fn00009540; memd(r29) = r19:r18 }
	{ r17:r16 = combine(r0,#0x0) }
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 0000AE1C; if (!p0.new) memb(r17-28) = #0x1 }

l0000ADAC:
	{ r0 = #0x80; immext(#0x2000000); r1 = #0x2000000 }
	{ call fn00009550 }
	{ r18 = r0; memb(r17+1) = r18.new }
	{ immext(#0x2000000); if (!p0.new) memw(r17+33554440) = #0x0 }
	{ call fn00009510; r0 = r17 }
	{ jump 0000AE1C }
0000ADDC                                     00 40 02 00             .@..
0000ADE0 00 E0 00 7C AE F3 FF 5B 0C 40 40 10 00 60 91 74 ...|...[.@@..`.t
0000ADF0 0E C0 91 A1 8E 73 FF 5B 00 C0 72 70 EA FF FF 59 .....s.[..rp...Y
0000AE00 FF 7F 01 00 BF 47 51 3C 00 C8 51 3C A2 CE 00 5A .....GQ<..Q<...Z
0000AE10 10 C0 60 70 00 40 10 75 10 E0 11 74             ..`p.@.u...t    

l0000AE1C:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }
0000AE28                         00 40 00 7F 00 C0 00 7F         .@......

;; hexagon_nn_getlog: 0000AE30
hexagon_nn_getlog proc
	{ immext(#0x1A540); r3 = add(PC,#0x1A541); r4 = #0xE; allocframe(#0x10) }
	{ r17:r16 = combine(r1,r0); r18 = r2; memd(r29+8) = r17:r16; memd(r29) = r19:r18 }
	{ r2 = min(r4,r18); call fn00009560; r1:r0 = combine(r3,r17) }
	{ r2 = add(r17,add(r18,#0x3F)); p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 0000AE90; r0 = #0xFFFFFFFF }

l0000AE60:
	{ memb(r2) = #0x0 }
	{ r19 = memw(r16+56) }
	{ call fn00009570; r0 = r19 }
	{ r1:r0 = combine(r19,r17); r3 = r0; r2 = r18 }
	{ r2 = min(r2,r3); call fn00009560 }
	{ r0 = #0x0; r2 = memw(r16+56); memw(r16+64) = #0x0 }
	{ memb(r2) = #0x0 }

l0000AE90:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; hexagon_nn_snpprint: 0000AE98
hexagon_nn_snpprint proc
	{ immext(#0x1A4C0); r3 = add(PC,#0x1A4D9); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17:r16 = combine(r1,r2) }
	{ call fn00009580; r1:r0 = combine(r3,r17); r18 = r0; memd(r29) = r19:r18 }
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 0000AECC; r0 = #0xFFFFFFFF; if (!p0.new) r2 = add(r16,#0x0) }

l0000AEC0:
	{ call do_snpprint; r1:r0 = combine(r17,r18) }
	{ r0 = #0x0 }

l0000AECC:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; hexagon_nn_set_debug_level: 0000AED4
hexagon_nn_set_debug_level proc
	{ if (p0.new) r2 = #0xFFFFFFFF; r3 = #0x0; p0 = cmp.eq(r0,#0x0) }
	{ r3 = max(r1,r3); if (!p0) r2 = add(r3,#0x0) }
	{ r0 = r2; jumpr r31 }

;; hexagon_nn_prepare: 0000AEF0
hexagon_nn_prepare proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000AF14; allocframe(#0x8) }

l0000AEF8:
	{ immext(#0x1A480); r3 = add(PC,#0x1A49F); r2 = #0x0; r1 = #0x87 }
	{ call errlog_function; memw(r29) = #0x0 }
	{ r0 = #-0x1; dealloc_return }

l0000AF14:
	{ jump do_prepare; deallocframe }

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
	{ immext(#0x1A440); r0 = add(PC,#0x1A463); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; hexagon_nn_append_node: 0000AF40
hexagon_nn_append_node proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000AF60; r6 = r4; allocframe(#0x8) }

l0000AF48:
	{ immext(#0x1A440); r3 = add(PC,#0x1A44F); r2 = #0x0; r1 = #0x98 }
	{ jump 0000AF78; memw(r29) = #0x0 }

l0000AF60:
	{ r4 = memb(r0+36); if (cmp.eq(r4.new,#0x1)) jump:t 0000AF80 }

l0000AF6C:
	{ r3 = add(PC,#0x2); r1 = #0x9B; r2 = r0 }

l0000AF78:
	{ call errlog_function }
	{ r0 = #-0x1; dealloc_return }

l0000AF80:
	{ r4 = memd(r29+16); r7 = memd(r29+20) }
	{ call do_append_node; r5:r4 = combine(r7,r5); memw(r29+4) = r4; memw(r29) = r6 }
	{ dealloc_return }

;; hexagon_nn_append_const_node: 0000AF94
hexagon_nn_append_const_node proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000AFB4; allocframe(#0x8) }

l0000AF9C:
	{ immext(#0x1A3C0); r3 = add(PC,#0x1A3FB); r2 = #0x0; r1 = #0xB4 }
	{ jump 0000AFCC; memw(r29) = #0x0 }

l0000AFB4:
	{ r6 = memb(r0+36); if (cmp.eq(r6.new,#0x1)) jump:t 0000AFD4 }

l0000AFC0:
	{ r3 = add(PC,#0x2E); r1 = #0xB7; r2 = r0 }

l0000AFCC:
	{ call errlog_function }
	{ r0 = #-0x1; dealloc_return }

l0000AFD4:
	{ r7 = memw(r29+20); memb(r29+1) = r7.new }
	{ call do_append_const_node; memw(r29) = r6 }
	{ dealloc_return }

;; hexagon_nn_execute_new: 0000AFEC
hexagon_nn_execute_new proc
	{ memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ r17:r16 = combine(r0,r1); r19:r18 = combine(r3,r2); memd(r29+16) = r19:r18 }
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:nt 0000B01C; r20 = r4; memd(r29+8) = r21:r20 }

l0000B004:
	{ immext(#0x1A380); r3 = add(PC,#0x1A393); r2 = #0x0; r1 = #0xD5 }
	{ jump 0000B034; memw(r29) = #0x0 }

l0000B01C:
	{ r2 = memb(r17+36); if (cmp.eq(r2.new,#0x2)) jump:t 0000B044 }

l0000B028:
	{ r3 = add(PC,#0x2B); r1 = #0xD8; r2 = r17 }

l0000B034:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); dealloc_return }

l0000B044:
	{ call logmsg_function; r2 = r17; memw(r29+4) = r20; memw(r29) = r18 }
	{ r4 = r20; r3:r2 = combine(r19,r18); r1:r0 = combine(r16,r17) }
	{ r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ jump do_execute; r21:r20 = memd(r29+8); deallocframe }

;; logmsg_function: 0000B068
;;   Called from:
;;     0000B044 (in hexagon_nn_execute_new)
logmsg_function proc
	{ r4 = #0x2; allocframe(#0x8) }
	{ r3 = memw(r2+16); if (cmp.gtu(r4,r3.new)) jump:t 0000B098 }

l0000B078:
	{ r0 = add(PC,#0xB); r3 = #0x2; r5 = add(r29,#0x10) }
	{ immext(#0x1A340); r4 = add(PC,#0x1A362); r1 = #0xDA; r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0000B098:
	{ dealloc_return }

;; hexagon_nn_execute: 0000B09C
hexagon_nn_execute proc
	{ p0 = cmp.eq(r0,#0x0); memw(r29-52) = r4; allocframe(#0x60) }
	{ r6 = memd(r29+104); memw(r29+44) = r2 }
	{ r4 = memd(r29+124); memw(r29+48) = r3 }
	{ r3 = memw(r29+128); memw(r29+60) = r6 }
	{ memw(r29+40) = r1; memw(r29+24) = r4 }
	{ memw(r29+28) = r3; memw(r29+64) = r6 }
	{ memd(r29+88) = r17:r16; memd(r29+80) = r19:r18 }
	{ memd(r29+72) = r21:r20; memw(r29+32) = r3 }
	{ if (!p0) jump:nt 0000B0E8; memw(r29+56) = r5; if (p0) memw(r29) = #0x0 }

l0000B0D4:
	{ immext(#0x1A2C0); r3 = add(PC,#0x1A2C3); r2 = #0x0; r1 = #0xF9 }
	{ jump 0000B100 }

l0000B0E8:
	{ r2 = memb(r0+36); if (cmp.eq(r2.new,#0x2)) jump:t 0000B10C }

l0000B0F4:
	{ r3 = add(PC,#0x1F); r1 = #0xFC; r2 = r0 }

l0000B100:
	{ call errlog_function }
	{ jump 0000B144; r0 = #0xFFFFFFFF }

l0000B10C:
	{ r4 = #0x1; r3 = add(r29,#0x8); r2 = #0x1; r1 = add(r29,#0x28) }
	{ r16 = memd(r29+108); r17 = memd(r29+116) }
	{ r18 = memd(r29+112); r19 = memd(r29+120) }
	{ call do_execute; r20 = memw(r29+132) }
	{ r2 = memw(r29+8); memb(r16) = r2.new }
	{ r4 = memd(r29+16); memw(r18) = r5 }
	{ memw(r17) = r4; memw(r19) = r3 }
	{ r2 = memw(r29+32); memb(r20) = r2.new }

l0000B144:
	{ r17:r16 = memd(r29+88); r19:r18 = memd(r29+80) }

l0000B148:
	{ r21:r20 = memd(r29+72); dealloc_return }
0000B14C                                     00 C0 00 7F             ....

;; hexagon_nn_teardown: 0000B150
hexagon_nn_teardown proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000B174; allocframe(#0x8) }

l0000B158:
	{ immext(#0x1A200); r3 = add(PC,#0x1A23F); r2 = #0x0; r1 = #0x10B }
	{ call errlog_function; memw(r29) = #0x0 }
	{ r0 = #-0x1; dealloc_return }

l0000B174:
	{ jump do_teardown; deallocframe }

;; hexagon_nn_get_perfinfo: 0000B17C
hexagon_nn_get_perfinfo proc
	{ p0 = cmp.eq(r0,#0x0); r16 = r3; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ if (p0) jump:nt 0000B1AC; if (p0) memw(r29) = #0x0 }

l0000B190:
	{ call do_perfinfo_get }
	{ r2 = r0; r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); memb(r16) = r2.new }
	{ r0 = #-0x1; dealloc_return }

l0000B1AC:
	{ immext(#0x1A1C0); r3 = add(PC,#0x1A1EB); r2 = #0x0; r1 = #0x117 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

;; hexagon_nn_reset_perfinfo: 0000B1C8
hexagon_nn_reset_perfinfo proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000B1EC; allocframe(#0x8) }

l0000B1D0:
	{ immext(#0x1A1C0); r3 = add(PC,#0x1A1C7); r2 = #0x0; r1 = #0x124 }
	{ call errlog_function; memw(r29) = #0x0 }
	{ r0 = #-0x1; dealloc_return }

l0000B1EC:
	{ jump do_perfinfo_reset; deallocframe }

;; hexagon_nn_version: 0000B1F4
hexagon_nn_version proc
	{ r0 = #0x0; r2 = r0 }
	{ jumpr r31; memw(r2) = #0x5A }

;; hexagon_nn_last_execution_cycles: 0000B200
hexagon_nn_last_execution_cycles proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000B224; allocframe(#0x8) }

l0000B208:
	{ immext(#0x1A180); r3 = add(PC,#0x1A18F); r2 = #0x0; r1 = #0x134 }
	{ call errlog_function; memw(r29) = #0x0 }
	{ r0 = #-0x1; dealloc_return }

l0000B224:
	{ r0 = #0x0; r5:r4 = memd(r0+80) }
	{ r7:r6 = lsr(r5:r4,#0x20) }
	{ memw(r2) = r6; memw(r1) = r4 }
	{ dealloc_return }

;; hexagon_nn_GetHexagonBinaryVersion: 0000B238
hexagon_nn_GetHexagonBinaryVersion proc
	{ r0 = #0x0; r2 = r0 }
	{ jumpr r31; memw(r2) = #0x5A }

;; hexagon_nn_PrintLog: 0000B244
hexagon_nn_PrintLog proc
	{ r0 = #0x0; jumpr r31 }
0000B248                         00 40 00 7F 00 C0 00 7F         .@......

;; hexagon_nn_op_name_to_id: 0000B250
hexagon_nn_op_name_to_id proc
	{ immext(#0x1FEC0); r2 = add(PC,#0x1FEC0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17:r16 = combine(r0,r1); r18 = #0x0; memd(r29) = r19:r18 }
	{ immext(#0xFFFFFFC0); r19 = memw(r2-64) }

l0000B26C:
	{ call fn00009590; r0 = r17; r1 = memw(r19) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B28C }

l0000B278:
	{ r19 = add(r19,#0x4); r18 = r18 }
	{ if (!p0.new) jump:nt 0000B26C; jump 0000B290; p0 = cmp.gt(r18,#0x72); if (p0.new) r0 = #0xFFFFFFFF }

l0000B28C:
	{ r0 = #0x0; memw(r16) = r18 }

l0000B290:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; hexagon_nn_op_id_to_name: 0000B298
hexagon_nn_op_id_to_name proc
	{ p0 = cmp.gtu(r0,#0x72); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ if (p0) jump:nt 0000B2DC; r17:r16 = combine(r1,#0xFFFFFFFF); r18 = r2; memd(r29) = r19:r18 }

l0000B2AC:
	{ immext(#0x1FE40); r2 = add(PC,#0x1FE64) }
	{ immext(#0xFFFFFFC0); r2 = memw(r2-64) }
	{ r19 = memw(r14+r0<<#2) }
	{ call fn00009570; r0 = r19 }
	{ r2 = add(r0,#0x1); if (cmp.gtu(r2.new,r18)) jump:t 0000B2DC }

l0000B2D4:
	{ r1:r0 = combine(r19,r17); r16 = #0x0 }

l0000B2DC:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; hexagon_nn_disable_dcvs: 0000B2E8
hexagon_nn_disable_dcvs proc
	{ r0 = #0x0; r7 = #0x5; allocframe(#0x28) }
	{ r1 = add(r29,#0x0); r2 = add(r29,#0x0); memb(r29) = r7 }
	{ call fn000095B0; immext(#0x100); memuh(r2+264) = #0x0 }
	{ dealloc_return }
0000B30C                                     00 C0 00 7F             ....

;; hexagon_nn_set_powersave_level: 0000B310
hexagon_nn_set_powersave_level proc
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000B33C; r2 = #0x0; allocframe(#0x28) }

l0000B318:
	{ r7 = #0x5; r2 = add(r29,#0x0); r0 = #0x0; memb(r29) = r7.new }
	{ r1 = add(r29,#0x0); immext(#0x100); memuh(r2+264) = #0x0 }
	{ r2 = r0 }

l0000B33C:
	{ r0 = r2; dealloc_return }

;; hexagon_nn_config: 0000B340
hexagon_nn_config proc
	{ r3:r2 = combine(#0xFFFFFFFF,#0xFFFFFFFF); immext(#0x1000000); r0 = #0x1000000; r1 = #0x0 }
	{ call fn000095C0; allocframe(#0x0) }
	{ r0 = #0x0; dealloc_return }
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
	{ r16 = r2; memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ immext(#0x1A080); r2 = add(PC,#0x1A0A2); r22 = memw(r16+64); memd(r29+8) = r23:r22 }
	{ r17 = memw(r16+24); r7 = memw(r16+28) }
	{ r3 = add(r17,r22); r20 = r5; memd(r29+16) = r21:r20; memw(r29+4) = r1 }
	{ r21 = sub(r7,r22) }
	{ r19 = r4; r1:r0 = combine(r21,r3); memd(r29+24) = r19:r18; memw(r29) = r0 }
	{ call fn000095D0 }
	{ r3:r2 = combine(r20,r19); r18 = r0; if (!cmp.gtu(r21,r18.new)) jump:t 0000B3E8 }

l0000B3AC:
	{ r1:r0 = combine(r21,r17) }
	{ r0 += add(r18,r22); call fn000095E0 }
	{ r3 = add(r18,r22); r19 = r0; if (!cmp.gtu(r21,r19.new)) jump:t 0000B3E8 }

l0000B3C8:
	{ r2 = add(PC,#0xD); r20 = sub(r21,r19) }
	{ r17 += add(r19,r3) }
	{ call fn000095D0; r1:r0 = combine(r20,r17) }
	{  }
	{ r0 += add(r19,r18) }
	{ memw(r16+64) += r0 }

l0000B3E8:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }
0000B3F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; alloc_node: 0000B400
;;   Called from:
;;     000137EC (in hexagon_nn_const_ctor)
alloc_node proc
	{ r18 = r0; r0 = #0x38; memd(r29-24) = r19:r18; allocframe(#0x10) }
	{ call fn00009500; r17:r16 = combine(r1,r2); memd(r29+8) = r17:r16 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B444; if (p0.new) memw(r0+28) = r18 }

l0000B420:
	{ immext(#0x1FCC0); r2 = add(PC,#0x1FCF0) }
	{ immext(#0xFFFFFFC0); r2 = memw(r2-16) }
	{ r2 = memw(r7+r17<<#2); memb(r0+32) = r16 }
	{ memw(r0) = r2; memw(r0+24) = r17 }
	{ memw(r0+44) = #0x0; memw(r0+52) = #0x0 }
	{ memw(r0+48) = #0x0; memw(r0+40) = #0x0 }

l0000B444:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; node_alloc_common: 0000B44C
;;   Called from:
;;     000141F8 (in conv2d_ctor)
;;     00014FD8 (in matmul_ctor)
;;     00016E08 (in nop_ctor)
;;     00017464 (in prefree_ctor)
;;     0001D3C4 (in variable_ctor)
node_alloc_common proc
	{ r16 = r0; r0 = #0x38; memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ memd(r29+40) = r19:r18; memd(r29+32) = r21:r20 }
	{ r21:r20 = combine(r2,r3); r19:r18 = combine(r4,r5); r22 = r1; memd(r29+24) = r23:r22 }
	{ call fn00009500 }
	{ r1 = #0xA0; r17 = r0; if (cmp.eq(r17.new,#0x0)) jump:nt 0000B5E0 }

l0000B47C:
	{ r2 = add(PC,#0x18) }
	{ p0 = cmp.eq(r19,#0x0); memw(r17+28) = r22 }
	{ immext(#0xFFFFFFC0); r2 = memw(r2-16) }
	{ r2 = memw(r15+r21<<#2) }
	{ memw(r17) = r2; memw(r17+24) = r21 }
	{ r21 = memw(r29+68) }
	{ memb(r17+32) = r20; memw(r17+44) = #0xFFFFFF80 }
	{ memw(r17+48) = #0x0; memw(r17+16) = r19 }
	{ memw(r17+40) = #0x0; memw(r17+52) = #0x0 }
	{ if (!p0) jump:nt 0000B4B8 }

l0000B4B0:
	{ jump 0000B538; memw(r17+4) = #0x0; memw(r17+12) = #0x0 }

l0000B4B8:
	{ r0 = asl(r19,#0x3); call fn00009500 }
	{ r20 = r0; memb(r17+3) = r20.new }
	{ r1 = #0x57 }
	{ immext(#0x1A000); r3 = add(PC,#0x1A01A) }
	{ call errlog_function; r2 = r16 }
	{ immext(#0x19F40); r3 = add(PC,#0x19F68); jump 0000B5EC; r1 = #0xA4 }
0000B4F0 40 42 13 8C 08 F0 FF 5B 80 40 00 10 41 6B 00 7E @B.....[.@..Ak.~
0000B500 01 C0 91 A1 10 40 13 60 02 C2 9D 91 03 40 82 91 .....@.`.....@..
0000B510 0C E0 42 24 80 46 00 00 83 46 49 6A E2 7F FF 59 ..B$.F...FIj...Y
0000B520 21 CC 00 78 23 C0 82 91 47 40 82 9B 01 C3 94 A1 !..x#...G@......
0000B530 00 80 00 7F 10 C7 94 AB                         ........        

l0000B538:
	{ p0 = cmp.eq(r18,#0x0); r1:r0 = combine(#0x0,#0x0); memw(r17+20) = r18 }
	{ if (!p0) jump:nt 0000B550; memd(r29+16) = r1:r0; memd(r29+8) = r1:r0 }

l0000B548:
	{ jump 0000B5D0; memw(r17+8) = #0x0 }

l0000B550:
	{ r0 = asl(r18,#0x2); call fn00009500 }
	{ r19 = r0; memb(r17+2) = r19.new }
	{ r1 = #0x84 }
	{ r20 = #0x0; r22 = #0x0 }

l0000B56C:
	{ call tensor_alloc; r1 = #0x0; r0 = add(r29,#0x8) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000B5AC; if (p0.new) r1 = #0x8D; memw(r19) = r0 }

l0000B580:
	{ immext(#0x19F40); r3 = add(PC,#0x19F4E); jump 0000B594 }
0000B58C                                     7C 46 00 00             |F..
0000B590 03 D1 49 6A                                     ..Ij            

l0000B594:
	{ call errlog_function; r2 = r16 }
	{ immext(#0x19E80); r3 = add(PC,#0x19EBF); jump 0000B5EC; r1 = #0xA8 }

l0000B5AC:
	{ r22 = add(r22,#0x1); r19 = add(r19,#0x4); r2 = memw(r17+8); r3 = memw(r21++#8) }
	{ p0 = cmp.gtu(r18,r22) }
	{ r20 = add(r20,#0x4); r2 = memw(r13+r20) }
	{ if (p0) jump:nt 0000B56C; memw(r2+20) = r3 }

l0000B5D0:
	{ r0 = r17; r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ dealloc_return }

l0000B5E0:
	{ immext(#0x19E40); r3 = add(PC,#0x19E49); memw(r29) = r22 }

l0000B5EC:
	{ call errlog_function; r17 = #0x0; r2 = r16 }
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
	{ immext(#0x19E00); r0 = add(PC,#0x19E0F); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; node_free_common: 0000B628
;;   Called from:
;;     00016E3C (in nop_dtor)
;;     000174A0 (in prefree_dtor)
;;     000194BC (in supernode_dtor)
;;     0001D4C4 (in variable_dtor)
node_free_common proc
	{ r2 = r1; r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ memd(r29+8) = r19:r18; memw(r29) = r16 }
	{ call logmsg_function }
	{ r0 = memw(r16+4); if (cmp.eq(r0.new,#0x0)) jump:nt 0000B648 }

l0000B648:
	{ r0 = memw(r16+12); if (cmp.eq(r0.new,#0x0)) jump:nt 0000B654 }

l0000B654:
	{ r2 = memw(r16+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000B684 }

l0000B660:
	{ r2 = memw(r16+8) }
	{ r2 = memw(r13+r17) }
	{ memw(r2+16) = #0x0 }
	{ r2 = memw(r16+8) }
	{ call tensor_free; r0 = memw(r13+r17) }
	{ r17 = add(r17,#0x4); r2 = memw(r16+20) }
	{ r18 = add(r18,#0x1); if (cmp.gtu(r2,r18.new)) jump:t 0000B660 }

l0000B684:
	{ r0 = memw(r16+8); if (cmp.eq(r0.new,#0x0)) jump:nt 0000B690 }

l0000B688:
	{ if (cmp.eq(r0.new,#0x0)) jump:nt 0000B694 }

l0000B690:
	{ call fn00009510; r0 = r16 }

l0000B694:
	{ r0 = r16 }

l0000B698:
	{ r0 = #0x0; r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }
	{ dealloc_return }

;; logmsg_function: 0000B6A4
;;   Called from:
;;     0000B638 (in node_free_common)
logmsg_function proc
	{ r4 = #0x3; allocframe(#0x8) }
	{ r3 = memw(r2+16); if (cmp.gtu(r4,r3.new)) jump:t 0000B6D4 }

l0000B6B4:
	{ r0 = add(PC,#0x23); r3 = #0x3; r5 = add(r29,#0x10) }
	{ immext(#0x19D80); r4 = add(PC,#0x19DB3); r1 = #0xB0; r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0000B6D4:
	{ dealloc_return }
0000B6D8                         00 40 00 7F 00 C0 00 7F         .@......

;; do_append_node: 0000B6E0
;;   Called from:
;;     0000AF84 (in hexagon_nn_append_node)
do_append_node proc
	{ immext(#0x1FA00); r6 = add(PC,#0x1FA30); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17:r16 = combine(r1,r0); immext(#0xFFFFFFC0); r6 = memw(r6-16) }
	{ r7 = memw(r29+28); r6 = memw(r14+r2<<#2) }
	{ r6 = memw(r6+8); memw(r29+4) = r7 }
	{ r7 = memw(r29+24) }
	{ callr r6; memw(r29) = r7 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B738; if (p0.new) memw(r29) = r17 }

l0000B718:
	{ memw(r0+36) = #0xFFFFFF80 }
	{ r2 = memw(r16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000B72C }

l0000B728:
	{ r16 = add(r2,#0x24) }

l0000B72C:
	{ r0 = #0x0; r17:r16 = memd(r29+8); memw(r16) = r0 }
	{ dealloc_return }

l0000B738:
	{ immext(#0x19D40); r3 = add(PC,#0x19D47); r1 = #0xD5; r2 = r16 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; do_append_const_node: 0000B754
;;   Called from:
;;     0000AFE0 (in hexagon_nn_append_const_node)
do_append_const_node proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r6 = memw(r29+28) }
	{ r6 = memd(r29+24); memw(r29+4) = r6 }
	{ call hexagon_nn_const_ctor; memw(r29) = r6 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B794; if (p0.new) memw(r29) = r17 }

l0000B774:
	{ memw(r0+36) = #0xFFFFFF80 }
	{ r2 = memw(r16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000B788 }

l0000B784:
	{ r16 = add(r2,#0x24) }

l0000B788:
	{ r0 = #0x0; r17:r16 = memd(r29+8); memw(r16) = r0 }
	{ dealloc_return }

l0000B794:
	{ immext(#0x19CC0); r3 = add(PC,#0x19CEB); r1 = #0xFC; r2 = r16 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; do_teardown: 0000B7B0
;;   Called from:
;;     0000B174 (in hexagon_nn_teardown)
do_teardown proc
	{ call nn_os_workers_kill; r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = memw(r16); memb(r16-28) = #0x0 }

l0000B7C4:
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 0000B7FC; r1:r0 = combine(r16,r2) }

l0000B7CC:
	{ r3 = memw(r2); r17 = memw(r2+4) }
	{ r2 = memw(r3+12) }
	{ callr r2 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B7C4; r2 = r17 }

l0000B7E0:
	{ immext(#0x19C80); r3 = add(PC,#0x19CB6); r1 = #0x10E; r2 = r16 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29); dealloc_return }

l0000B7FC:
	{ call allocator_teardown; r0 = r16 }
	{ call fn00009510; r0 = memw(r16+4) }
	{ call fn00009510; r0 = r16 }
	{ r0 = #0x0; r17:r16 = memd(r29); dealloc_return }
0000B81C                                     00 00 00 00             ....

;; do_snpprint: 0000B820
;;   Called from:
;;     0000AEC0 (in hexagon_nn_snpprint)
do_snpprint proc
	{ immext(#0x19D00); r3 = add(PC,#0x19D26); memd(r29-24) = r19:r18; allocframe(#0x40) }
	{ r18 = r1; memd(r29+40) = r21:r20 }
	{ r4 = add(r18,add(r2,#0x3F)); r17 = add(r2,#0xFFFFFFFF); r16 = r0; memd(r29+56) = r17:r16 }
	{ r1:r0 = combine(r17,r18); memd(r29+32) = r23:r22; memb(r4) = #0x0 }
	{ r2 = memw(r16+12); r7 = memw(r16+16) }
	{ memw(r29+8) = r7; memw(r29) = r16 }
	{ r2 = r3; memw(r29+4) = r2 }
	{ call fn000095D0 }
	{ p0 = cmp.eq(r9,r0); if (p0.new) jump:nt 0000B990; if (!p0.new) r17 = sub(r17,r0) }

l0000B85C:
	{ r19 = #0x0; r18 = add(r18,r0); r20 = memw(r16) }

l0000B864:
	{ p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 0000B97C; r1:r0 = combine(r17,r18); if (!p0.new) r8 = memw(r20+16); if (!p0.new) r7 = memb(r20-32) }

l0000B874:
	{ immext(#0x1F880); r3 = add(PC,#0x1F89C); r4 = memw(r20+24); r5 = memw(r20+28) }
	{ immext(#0x1FE00); r6 = add(PC,#0x1FE08); immext(#0xFFFFFFC0); r3 = memw(r3-64) }
	{ immext(#0x19CC0); r2 = add(PC,#0x19CD8); r9 = memw(r20+20) }
	{ r6 = memw(r6+r7<<#2); memb(r29+7) = r6.new }
	{ memw(r29+24) = r7 }
	{ memw(r29+20) = r9; memw(r29+16) = r8 }
	{ memw(r29+12) = r3; memw(r29+8) = r4 }
	{ memw(r29+4) = r5; memw(r29) = r20 }
	{ call fn000095D0 }
	{ p0 = cmp.eq(r9,r0); if (p0.new) jump:nt 0000B990; if (!p0.new) r17 = sub(r17,r0) }

l0000B8C8:
	{ r18 = add(r18,r0); r2 = memw(r16+16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000B974 }

l0000B8D8:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000B938 }

l0000B8E0:
	{ immext(#0x19CC0); r2 = add(PC,#0x19CD2); r4 = memw(r20+4); r3 = memw(r20+12) }
	{ r1:r0 = combine(r17,r18); r5 = add(r3,r21); r3 = memw(r21+r21); r4 = memw(r13+r22) }
	{ r5 = memw(r5-4); memw(r29+12) = r3 }
	{ memw(r29+8) = r5; memw(r29+4) = r4 }
	{ call fn000095D0; memw(r29) = r23 }
	{ p0 = cmp.eq(r9,r0); if (p0.new) jump:nt 0000B990; if (!p0.new) r17 = sub(r17,r0) }

l0000B918:
	{ r22 = add(r22,#0x4); r18 = add(r18,r0); r21 = add(r21,#0x8); r2 = memw(r20+16) }
	{ r23 = add(r23,#0x1); if (cmp.gtu(r2,r23.new)) jump:t 0000B8E0 }

l0000B930:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000B978 }

l0000B938:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000B978 }

l0000B940:
	{ immext(#0x19C80); r2 = add(PC,#0x19C9C); r1:r0 = combine(r17,r18); r3 = memw(r20+8) }
	{ r3 = memw(r29+r21) }
	{ call fn000095D0; memw(r29+4) = r3; memw(r29) = r22 }
	{ p0 = cmp.eq(r9,r0); if (p0.new) jump:nt 0000B990; if (!p0.new) r17 = sub(r17,r0) }

l0000B964:
	{ r18 = add(r18,r0); r21 = add(r21,#0x4); r2 = memw(r20+20) }
	{ r22 = add(r22,#0x1); if (cmp.gtu(r2,r22.new)) jump:t 0000B940 }

l0000B974:
	{ jump 0000B864; r19 = r19; r20 = memw(r20+4) }

l0000B978:
	{ r19 = r19; r20 = memw(r20+4) }

l0000B97C:
	{ immext(#0x19C40); r2 = add(PC,#0x19C74); r1:r0 = combine(r17,r18); memw(r29) = r19 }
	{ call fn000095D0 }

l0000B990:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ dealloc_return }
0000B99C                                     00 00 00 00             ....

;; const_depth_extend_8: 0000B9A0
const_depth_extend_8 proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r3 = memw(r16+8); memd(r29+16) = r23:r22 }
	{ r21 = r2; memd(r29+24) = r21:r20 }
	{ memd(r29+32) = r19:r18 }
	{ r23 = memw(r3); memd(r29+8) = r25:r24 }
	{ memd(r29) = r27:r26 }
	{ r2 = memw(r23); r7 = memw(r23+8) }
	{ r4 = memw(r23+4); r18 = memw(r23+12) }
	{ r2 = mpyi(r7,r2); r24 = add(r18,r17); r19 = memw(r23+16) }
	{ r25 = mpyi(r2,r4) }
	{ immext(#0x80); r0 = add(#0x80,mpyi(r24,r25)); call fn00009500 }
	{ r2 = #0xFFFFFFFF; r20 = r0; if (cmp.eq(r20.new,#0x0)) jump:nt 0000BA4C }

l0000B9F4:
	{ r26 = r25; p0 = cmp.gt(r25,#0x0) }
	{ r22 = r20; r21 = and(r21,#0xFF) }

l0000BA00:
	{ call fn00009560; r2 = r18; r1:r0 = combine(r19,r22) }
	{ r0 = add(r22,r18); r1 = r21; r2 = r17; r19 = add(r19,r18) }
	{ call fn000095F0; r22 = add(r22,r24) }
	{ r26 = add(r26,#0xFFFFFFFF); if (!cmp.eq(r26.new,#0x0)) jump:t 0000BA00 }

l0000BA2C:
	{ call fn00009514; r0 = memw(r23+16) }
	{ r2 = #0x0; memw(r23+16) = r20; memw(r23+12) = r24 }
	{ r3 = memw(r16+8) }
	{ r3 = memw(r3) }
	{ memw(r3+20) = r17 }

l0000BA4C:
	{ r0 = r2; r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); r27:r26 = memd(r29) }
	{ dealloc_return }

;; const_width_extend_8: 0000BA64
;;   Called from:
;;     0000BE78 (in try_pad_bad_supernodes)
const_width_extend_8 proc
	{ r27 = r0; memd(r29-56) = r27:r26; allocframe(#0x38) }
	{ r22 = r2; r3 = memw(r27+8); memd(r29+24) = r23:r22 }
	{ r19 = r1; memd(r29+40) = r19:r18 }
	{ memd(r29+48) = r17:r16 }
	{ r23 = memw(r3); memd(r29+32) = r21:r20 }
	{ memd(r29+16) = r25:r24 }
	{ r21 = memw(r23+8); r16 = memw(r23) }
	{ r24 = add(r21,r19); r20 = memw(r23+4); r26 = memw(r23+12) }
	{ r3 = mpyi(r24,r16); r17 = memw(r23+16) }
	{ r25 = mpyi(r3,r20) }
	{ immext(#0x80); r0 = add(#0x80,mpyi(r25,r26)); call fn00009500 }
	{ r2 = #0xFFFFFFFF; r18 = r0; if (cmp.eq(r18.new,#0x0)) jump:nt 0000BB28 }

l0000BAC4:
	{ r19 = mpyi(r26,r19); r27 = mpyi(r20,r16); if (!cmp.gt(r27.new,#0x0)) jump:nt 0000BB04 }

l0000BAD4:
	{ r20 = r18; r22 = and(r22,#0xFF) }
	{ r21 = mpyi(r26,r21) }

l0000BADC:
	{ call fn00009560; r2 = r21; r1:r0 = combine(r17,r20) }
	{ r0 = add(r20,r21); r1 = r22; r2 = r19; r17 = add(r17,r21) }
	{ call fn000095F0; r20 = add(r20,r16) }
	{ r27 = add(r27,#0xFFFFFFFF); if (!cmp.eq(r27.new,#0x0)) jump:t 0000BADC }

l0000BB04:
	{ r16 = mpyi(r25,r26); call fn00009510; r0 = memw(r23+16) }

l0000BB08:
	{ call fn00009514; r0 = memw(r23+16) }

l0000BB10:
	{ r2 = #0x0; r3 = memd(r29+4); memw(r23+16) = r18 }
	{ memw(r23+8) = r24 }
	{ r3 = memw(r3+8) }
	{ r3 = memw(r3) }
	{ memw(r3+20) = r16 }

l0000BB28:
	{ r0 = r2; r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); r27:r26 = memd(r29+8) }
	{ dealloc_return }

;; check_same_inputs: 0000BB40
check_same_inputs proc
	{ r4 = memw(r1+16); if (cmp.gtu(r3,r4.new)) jump:t 0000BB90 }

l0000BB4C:
	{ r4 = memw(r2+16); if (cmp.gtu(r3,r4.new)) jump:t 0000BB94 }

l0000BB58:
	{ if (!p0.new) jumpr:nt r31 }
0000BB5C                                     00 41 03 60             .A.`
0000BB60 25 03 14 03 82 40 04 B0 83 C0 05 B0 E4 FF 82 97 %....@..........
0000BB70 E5 7F 83 97 10 C4 42 20 04 C0 82 91 05 40 83 91 ......B .....@..
0000BB80 0A C4 42 20 00 80 00 7F 83 20 82 20 C0 3F 00 48 ..B ..... . .?.H

l0000BB90:
	{ r0 = #0xFFFFFFFF }

l0000BB94:
	{ jumpr r31 }

;; try_pad_bad_supernodes: 0000BB98
;;   Called from:
;;     0000C640 (in do_prepare)
try_pad_bad_supernodes proc
	{ memd(r29-16) = r17:r16; allocframe(#0x60) }
	{ r17 = r0; r16 = memw(r1) }
	{ r19 = #0x0; memd(r29+80) = r19:r18; memd(r29+72) = r21:r20 }
	{ r2 = memw(r16+24) }
	{ p0 = cmp.eq(r2,#0x31); if (!p0) r1:r0 = combine(r16,r17); memd(r29+64) = r23:r22; memd(r29+56) = r25:r24 }
	{ if (!p0) jump:nt 0000BECC; memd(r29+48) = r27:r26 }

l0000BBC4:
	{ call find_first_consumer.1.2_i32_0 }
	{ r18 = r0; if (cmp.eq(r18.new,r16)) jump:nt 0000BECC }

l0000BBD4:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000BC14 }

l0000BBDC:
	{ r19 = #0x0 }

l0000BBE0:
	{ r20 = add(r20,#0x1); if (!cmp.gtu(r2,r20.new)) jump:nt 0000BC10 }

l0000BBEC:
	{ r2 = r20; r1:r0 = combine(r16,r17) }
	{ p0 = cmp.eq(r0,r10); if (!p0.new) jump:nt 0000BECC; if (p0.new) r2 = add(r20,#0x0); if (!p0) r1:r0 = combine(r16,r17) }

l0000BC00:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r10); if (p0.new) jump:nt 0000BBE0; if (p0.new) r2 = memw(r16+20) }

l0000BC0C:
	{ jump 0000BECC }

l0000BC10:
	{ r19 = #0x0; r2 = memw(r18+24) }

l0000BC14:
	{ if (!p0.new) jump:nt 0000BECC; p0 = cmp.eq(r2,#0x31) }

l0000BC1C:
	{ r2 = memw(r17); if (cmp.eq(r2.new,#0x0)) jump:nt 0000BECC }

l0000BC28:
	{ jump 0000BC38; r3 = memw(r4+56) }

l0000BC30:
	{ r5 = memw(r5+36); if (cmp.eq(r5.new,#0x0)) jump:nt 0000BECC }

l0000BC34:
	{ if (cmp.eq(r5.new,#0x0)) jump:nt 0000BED0 }

l0000BC38:
	{ r6 = memw(r5+28); if (!cmp.eq(r6.new,r3)) jump:t 0000BC30 }

l0000BC3C:
	{ if (!cmp.eq(r6.new,r3)) jump:t 0000BC34 }

l0000BC44:
	{ r5 = memw(r5) }
	{ r24 = memw(r5+12) }
	{ p0 = bitsclr(r24,#0x1F); if (p0.new) jump:nt 0000BECC; r5 = add(r24,#0x1F); if (!p0.new) r26 = add(r2,#0x0) }

l0000BC5C:
	{ r22 = and(r5,#0xFFFFFFE0); r4 = memw(r4+8) }
	{ jump 0000BC74; r20 = sub(r22,r24) }

l0000BC6C:
	{ r26 = memw(r26+36); if (cmp.eq(r26.new,#0x0)) jump:nt 0000BECC }

l0000BC70:
	{ if (cmp.eq(r26.new,#0x0)) jump:nt 0000BED0 }

l0000BC74:
	{ r5 = memw(r26+28); if (!cmp.eq(r5.new,r4)) jump:t 0000BC6C }

l0000BC78:
	{ if (!cmp.eq(r5.new,r4)) jump:t 0000BC70 }

l0000BC80:
	{ r25 = r2 }
	{ r25 = memw(r25+36); if (cmp.eq(r25.new,#0x0)) jump:nt 0000BECC }

l0000BC88:
	{ if (cmp.eq(r25.new,#0x0)) jump:nt 0000BED0 }

l0000BC90:
	{ if (!cmp.eq(r4.new,r3)) jump:t 0000BC88 }

l0000BC98:
	{ jump 0000BCA8; r4 = memw(r3+8) }

l0000BCA0:
	{ r21 = memw(r21+36); if (cmp.eq(r21.new,#0x0)) jump:nt 0000BECC }

l0000BCA4:
	{ if (cmp.eq(r21.new,#0x0)) jump:nt 0000BED0 }

l0000BCA8:
	{ r5 = memw(r21+28); if (!cmp.eq(r5.new,r4)) jump:t 0000BCA0 }

l0000BCAC:
	{ if (!cmp.eq(r5.new,r4)) jump:t 0000BCA4 }

l0000BCB4:
	{ r5 = memw(r3+32) }
	{ r4 = memw(r4+36); if (cmp.eq(r4.new,#0x0)) jump:nt 0000BECC }

l0000BCBC:
	{ if (cmp.eq(r4.new,#0x0)) jump:nt 0000BED0 }

l0000BCC4:
	{ if (!cmp.eq(r6.new,r5)) jump:t 0000BCBC }

l0000BCCC:
	{ r3 = memw(r3+40) }
	{ r2 = memw(r2+36); if (cmp.eq(r2.new,#0x0)) jump:nt 0000BECC }

l0000BCD4:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000BED0 }

l0000BCDC:
	{ if (!cmp.eq(r5.new,r3)) jump:t 0000BCD4 }

l0000BCE4:
	{ r0 = #0x17; r3 = memw(r4+8); r2 = memw(r2+8) }
	{ r3 = memw(r3) }
	{ r2 = memw(r2) }
	{ r2 = memw(r2+16); r3 = memw(r3+16) }
	{ r19 = memw(r3) }
	{ r2 = memw(r2) }
	{ r1 = sfsub(r2,r19); call fn00009600 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ immext(#0x0); r2 = #0x0 }
	{ r2 = sfsub(r2,r19) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ immext(#0x19900); r4 = add(PC,#0x1991F); r2 = convert_uw2sf(r0):chop; r5 = memw(r16+28) }
	{ r1 = #0x1C1; p1 = cmp.gt(r2,#0xFFFFFFFF); memw(r29+12) = r22 }
	{ p0 = cmp.gt(r2,#0xFF); if (p0.new) r3 = #0xFF; if (!p0.new) r3 = zxtb(r2); memw(r29+8) = r20 }
	{ if (!p1) r3 = #0x0; memw(r29+4) = r5; memw(r29) = r24 }
	{ r3:r2 = combine(#0x2,r17); memw(r29+28) = r3; memw(r29+16) = r3 }
	{ call logmsg_function }
	{ r19 = #0xFFFFFFFF; r2 = memw(r25+8) }
	{ r23 = memw(r2) }
	{ r7 = memw(r23); r6 = memw(r23+12) }
	{ r3 = add(r6,r20); r1 = memw(r23+8); r4 = memw(r23+4) }
	{ r2 = mpyi(r1,r7); r5 = memw(r23+16) }
	{ memw(r29+32) = r6; memw(r29+40) = r5 }
	{ r2 = mpyi(r2,r4); memw(r29+44) = r3 }
	{ immext(#0x80); r0 = add(#0x80,mpyi(r3,r2)); memw(r29+36) = r2 }
	{ call fn00009500 }
	{ r27 = r0; if (cmp.eq(r27.new,#0x0)) jump:t 0000BECC }

l0000BDB8:
	{ r2 = r20; r3 = memd(r29+44); r0 = memd(r29+36) }
	{ r1 = memd(r29+32); r5 = memd(r29+40) }
	{ r6 = mpyi(r3,r0); call try_pad_bad_supernodes.extracted_region; memb(r29+10) = r6.new }
	{ r0 = memw(r23+16) }
	{ r2 = memw(r29+44); memw(r23+16) = r27 }
	{ memw(r23+12) = r2 }
	{ r2 = memw(r25+8); r3 = memw(r29+40) }
	{ r2 = memw(r2) }
	{ memw(r2+20) = r3 }
	{ r7 = memw(r26+8) }
	{ r25 = memw(r7) }
	{ r2 = memw(r25+8); r5 = memw(r25+12) }
	{ r7 = memw(r25); r4 = memw(r25+4) }
	{ r2 = mpyi(r2,r7); r23 = add(r5,r20); r3 = memw(r25+16) }
	{ r2 = mpyi(r2,r4); memw(r29+36) = r5; memw(r29+44) = r3 }
	{ immext(#0x80); r0 = add(#0x80,mpyi(r23,r2)); memw(r29+40) = r2 }
	{ call fn00009500 }
	{ r3:r2 = combine(r23,r20); r27 = r0; if (cmp.eq(r27.new,#0x0)) jump:nt 0000BECC }

l0000BE40:
	{ r0 = memd(r29+40); r1 = memd(r29+36) }
	{ r5 = memw(r29+44) }
	{ r6 = mpyi(r23,r0); call try_pad_bad_supernodes.extracted_region; memb(r29+11) = r6.new }
	{ r0 = memw(r25+16) }
	{ r1:r0 = combine(r20,r21); memw(r25+16) = r27; memw(r25+12) = r23 }
	{ r3 = memw(r26+8); r2 = memw(r29+28) }
	{ r4 = memw(r29+44) }
	{ r3 = memw(r3) }
	{ call const_width_extend_8; memw(r3+20) = r4 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000BECC; if (p0.new) r2 = memw(r16+8) }

l0000BE88:
	{ r1:r0 = sxtw(r22) }
	{ r3:r2 = sxtw(r24); r19 = memw(r2) }
	{ r4 = memw(r19+20) }
	{ r7:r6 = mpyu(r4,r0) }
	{ r7 += mpyi(r4,r1) }
	{ call fn00009630; r1:r0 = combine(r7,r6) }
	{ immext(#0x197C0); r4 = add(PC,#0x197E1); r3:r2 = combine(#0x2,r17); memw(r19+20) = r0 }
	{ r1 = #0x1C6; r5 = memw(r16+28); r6 = memw(r18+28) }
	{ memw(r29+4) = r6 }
	{ call logmsg_function; r19 = #0x0; memw(r29) = r5 }

l0000BECC:
	{ r0 = r19; r25:r24 = memd(r29+56); r27:r26 = memd(r29+48) }

l0000BED0:
	{ r25:r24 = memd(r29+56); r27:r26 = memd(r29+48) }
	{ r17:r16 = memd(r29+88); r19:r18 = memd(r29+80) }
	{ r21:r20 = memd(r29+72); r23:r22 = memd(r29+64) }
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
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000BF04 }

l0000BEF4:
	{ r0 = add(PC,#0x5); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0000BF04:
	{ dealloc_return }

;; do_prepare: 0000BF08
;;   Called from:
;;     0000AF14 (in hexagon_nn_prepare)
do_prepare proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0xA8) }
	{ memd(r29+152) = r19:r18; memd(r29+144) = r21:r20 }
	{ memd(r29+136) = r23:r22; memd(r29+128) = r25:r24 }
	{ call nn_os_hvx_power_on; memd(r29+120) = r27:r26 }
	{ r1 = #0x1E5; r2 = memb(r16+36); if (cmp.eq(r2.new,#0x1)) jump:t 0000BF48 }

l0000BF34:
	{ r3 = add(PC,#0x6) }

l0000BF38:
	{ r2 = r16 }

l0000BF3C:
	{ call errlog_function }
	{ jump 0000C734; r0 = #0xFFFFFFFF }

l0000BF48:
	{ r17 = memw(r16); immext(#0x700); immext(#0x700); r21 = add(r29,#0x708); if (cmp.eq(r22.new,#0x0)) jump:nt 0000BF98 }

l0000BF5C:
	{ immext(#0xFFFFFFC0); r23 = #0xFFFFFFCC }

l0000BF64:
	{ r2 = memw(r17+24) }
	{ if (!p0.new) jump:nt 0000C0F4; p0 = cmp.eq(r2,#0x4D); if (p0.new) r1 = #0xD1 }

l0000BF74:
	{ immext(#0x19880); r4 = add(PC,#0x198B3); r3:r2 = combine(#0x9,r16) }
	{ call logmsg_function }
	{ call find_first_consumer.1.2_i32_0; r1:r0 = combine(r17,r16) }
	{ r18 = r0; if (cmp.eq(r18.new,r17)) jump:nt 0000C0F4 }

l0000BF98:
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000BFD8 }

l0000BFA4:
	{ r19 = add(r19,#0x1); if (!cmp.gtu(r2,r19.new)) jump:nt 0000BFD4 }

l0000BFB0:
	{ r2 = r19; r1:r0 = combine(r17,r16) }
	{ p0 = cmp.eq(r0,r10); if (!p0.new) jump:nt 0000C0F4; if (p0.new) r2 = add(r19,#0x0); if (!p0) r1:r0 = combine(r17,r16) }

l0000BFC4:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r10); if (p0.new) jump:nt 0000BFA4; if (p0.new) r2 = memw(r17+20) }

l0000BFD0:
	{ jump 0000C0F4 }

l0000BFD4:
	{ r2 = memw(r18+24) }

l0000BFD8:
	{ if (!p0.new) jump:nt 0000C0F4; p0 = cmp.eq(r2,#0x4B) }

l0000BFE0:
	{ r2 = memw(r17+16); if (cmp.gtu(r20,r2.new)) jump:t 0000C0F4 }

l0000BFEC:
	{ if (cmp.gtu(r20,r2.new)) jump:t 0000C0F8 }

l0000BFF4:
	{ r4 = memw(r17+12); r3 = memw(r18+12) }
	{ jump 0000C010; r3 = add(r3,#0x4); r4 = add(r4,#0x4) }

l0000C000:
	{ r4 = add(r4,#0x8); r3 = add(r3,#0x8); r2 = add(r2,#0x1); if (cmp.gt(r2.new,#0x2)) jump:nt 0000C02C }

l0000C010:
	{ r5 = memw(r4-4) }

l0000C014:
	{ r6 = memw(r3-4); if (!cmp.eq(r6.new,r5)) jump:nt 0000C0F4 }

l0000C020:
	{ r6 = memw(r3); if (cmp.eq(r6.new,r5)) jump:t 0000C000 }

l0000C02C:
	{ immext(#0x19800); r4 = add(PC,#0x19815); r3:r2 = combine(#0x9,r16); r1 = #0xD8 }
	{ call logmsg_function }
	{ immext(#0x1F0C0); r4 = add(PC,#0x1F0D0); r3 = setbit(r21,#0x4); r2 = memw(r18+8) }
	{ r5 = #0x3; r0 = r16 }
	{ r2 = #0x13; r6 = memw(r2) }
	{ r6 = memw(r6+20) }
	{ memw(r29+8) = r6; memw(r3) = #0x0 }
	{ r7 = memw(r18+8) }
	{ r1 = memw(r7+4) }
	{ r3 = memw(r1+20); memb(r29+4) = r3.new }
	{ r7 = memw(r18+8) }
	{ r3 = memw(r7+8) }
	{ r3 = memw(r3+20) }
	{ memw(r29+24) = r3; memw(r21+20) = #0x0 }
	{ r6 = memw(r13+r23) }
	{ r6 = memw(r6+76); r3 = memb(r18+32) }
	{ r4 = memw(r17+16); r1 = memw(r18+28) }
	{ r6 = memw(r6+8); r7 = memw(r17+12) }
	{ memw(r29+4) = r21 }
	{ callr r6; memw(r29) = r7 }
	{ r19 = r0; if (cmp.eq(r19.new,#0x0)) jump:nt 0000C0F4 }

l0000C0B0:
	{ memw(r22) = r19 }
	{ r7 = memw(r18+36); memb(r19+9) = r7.new }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1:r0 = combine(r16,r18); r2 = memw(r18) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ immext(#0x19780); r4 = add(PC,#0x19783); r3:r2 = combine(#0x3,r16); r1 = #0xEB }
	{ call logmsg_function; r5 = memw(r19+28); memb(r29) = r5.new }

l0000C0F4:
	{ r2 = memw(r22) }

l0000C0F8:
	{ r22 = add(r2,#0x24) }
	{ r17 = memw(r2+36); if (!cmp.eq(r17.new,#0x0)) jump:t 0000BF64 }

l0000C108:
	{ immext(#0x540); immext(#0x540); r21 = add(r29,#0x560); if (cmp.eq(r22.new,#0x0)) jump:nt 0000C168 }

l0000C118:
	{ r2 = memw(r17+24) }
	{ if (!p0.new) jump:nt 0000C29C; p0 = cmp.eq(r2,#0x4B); if (p0.new) r3 = memw(r17+12) }

l0000C128:
	{ r2 = memw(r3+36); if (!cmp.eq(r2.new,#0x0)) jump:t 0000C29C }

l0000C134:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000C2A0 }

l0000C13C:
	{ r3 = memw(r3+32) }
	{ r2 = memw(r2+36); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C29C }

l0000C144:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000C2A0 }

l0000C14C:
	{ if (!cmp.eq(r4.new,r3)) jump:t 0000C144 }

l0000C154:
	{ if (!cmp.eq(r2.new,#0x3)) jump:t 0000C2A0 }

l0000C15C:
	{ r1:r0 = combine(r17,r16) }
	{ r18 = r0; if (cmp.eq(r18.new,r17)) jump:nt 0000C29C }

l0000C168:
	{ r19 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C1A8 }

l0000C16C:
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C1AC }

l0000C178:
	{ r19 = add(r19,#0x1); if (!cmp.gtu(r2,r19.new)) jump:nt 0000C1A8 }

l0000C184:
	{ r2 = r19; r1:r0 = combine(r17,r16) }
	{ p0 = cmp.eq(r0,r10); if (!p0.new) jump:nt 0000C29C; if (p0.new) r2 = add(r19,#0x0); if (!p0) r1:r0 = combine(r17,r16) }

l0000C198:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r10); if (p0.new) jump:nt 0000C178; if (p0.new) r2 = memw(r17+20) }

l0000C1A4:
	{ jump 0000C29C }

l0000C1A8:
	{ r1 = #0x106; r2 = memw(r18+24); if (!cmp.eq(r2.new,#0x15)) jump:t 0000C29C }

l0000C1AC:
	{ r2 = memw(r18+24); if (!cmp.eq(r2.new,#0x15)) jump:t 0000C2A0 }

l0000C1B8:
	{ r4 = add(PC,#0x29); r3:r2 = combine(#0x9,r16) }
	{ call logmsg_function }
	{ immext(#0x1EF40); r6 = add(PC,#0x1EF4C); r3 = setbit(r20,#0x4); r2 = memw(r18+8) }
	{ r5:r4 = combine(#0x3,#0x4); r2 = memw(r2) }
	{ r2 = memw(r2+20) }
	{ r2 = #0x17; memw(r29+8) = r2; memw(r3) = #0x0 }
	{ r3 = memw(r18+12) }
	{ r0 = memw(r3) }
	{ r1 = memw(r3+4) }
	{ memd(r29+32) = r1:r0 }
	{ r7 = memw(r18+8) }
	{ r3 = memw(r7+4) }
	{ r3 = memw(r3+20) }
	{ memw(r29+16) = r3; memw(r20+12) = #0x0 }
	{ r7 = memw(r18+12) }
	{ r0 = memw(r7+8) }
	{ r1 = memw(r7+12) }
	{ memd(r29+40) = r1:r0 }
	{ r3 = memw(r18+8) }
	{ r3 = memw(r3+8) }
	{ r3 = memw(r3+20) }
	{ memw(r29+24) = r3; memw(r20+20) = #0x0 }
	{ r7 = memw(r18+12) }
	{ r0 = memw(r7+16) }
	{ r1 = memw(r7+20) }
	{ memd(r29+48) = r1:r0 }
	{ r3 = memw(r17+12) }
	{ r0 = memw(r3+32) }
	{ r1 = memw(r3+36) }
	{ memd(r29+56) = r1:r0 }
	{ r0 = r16; r6 = memw(r13+r23) }
	{ r6 = memw(r6+92); r3 = memb(r17+32) }
	{ r1 = memw(r18+28) }
	{ r6 = memw(r6+8); memw(r29+4) = r20 }
	{ callr r6; memw(r29) = r21 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000C29C; if (p0.new) memw(r17+36) = r0 }

l0000C270:
	{ r1:r0 = combine(r16,r18); r7 = memw(r18+36); memb(r0+9) = r7.new }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ immext(#0x19540); r4 = add(PC,#0x19569); r3:r2 = combine(#0x3,r16); r1 = #0x11A }
	{ call logmsg_function }

l0000C29C:
	{ r2 = memw(r22) }

l0000C2A0:
	{ r22 = add(r2,#0x24) }
	{ r17 = memw(r2+36); if (!cmp.eq(r17.new,#0x0)) jump:t 0000C118 }

l0000C2B0:
	{ r17 = memw(r16); if (cmp.eq(r17.new,#0x0)) jump:nt 0000C660 }

l0000C2BC:
	{ r25 = add(r29,#0x20) }

l0000C2C0:
	{ r1 = #0x133; r2 = memw(r17+24); if (!cmp.eq(r2.new,#0xF)) jump:t 0000C620 }

l0000C2D0:
	{ r4 = add(PC,#0x26); r3:r2 = combine(#0x9,r16) }
	{ call logmsg_function }
	{ call find_first_consumer.1.2_i32_0; r1:r0 = combine(r17,r16) }
	{ r18 = r0; if (cmp.eq(r18.new,r17)) jump:nt 0000C620 }

l0000C2F0:
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C330 }

l0000C2FC:
	{ r19 = add(r19,#0x1); if (!cmp.gtu(r2,r19.new)) jump:nt 0000C32C }

l0000C308:
	{ r2 = r19; r1:r0 = combine(r17,r16) }
	{ p0 = cmp.eq(r0,r10); if (!p0.new) jump:nt 0000C620; if (p0.new) r2 = add(r19,#0x0); if (!p0) r1:r0 = combine(r17,r16) }

l0000C31C:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r10); if (p0.new) jump:nt 0000C2FC; if (p0.new) r2 = memw(r17+20) }

l0000C328:
	{ jump 0000C620 }

l0000C32C:
	{ r2 = memw(r18+24); if (cmp.eq(r2.new,#0x13)) jump:t 0000C33C }

l0000C330:
	{ if (cmp.eq(r2.new,#0x13)) jump:t 0000C340 }

l0000C338:
	{ p0 = cmp.eq(r2,#0x4B) }

l0000C33C:
	{ immext(#0x19440); r4 = add(PC,#0x19444); r3:r2 = combine(#0x9,r16); r1 = #0x13B }

l0000C340:
	{ r4 = add(PC,#0x4); r3:r2 = combine(#0x9,r16); r1 = #0x13B }

l0000C34C:
	{ call logmsg_function }
	{ call find_first_consumer.1.2_i32_0; r1:r0 = combine(r18,r16) }
	{ r19 = r0; if (cmp.eq(r19.new,r18)) jump:nt 0000C620 }

l0000C364:
	{ r2 = memw(r18+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C3A4 }

l0000C370:
	{ r20 = add(r20,#0x1); if (!cmp.gtu(r2,r20.new)) jump:nt 0000C3A0 }

l0000C37C:
	{ r2 = r20; r1:r0 = combine(r18,r16) }
	{ p0 = cmp.eq(r0,r11); if (!p0.new) jump:nt 0000C620; if (p0.new) r2 = add(r20,#0x0); if (!p0) r1:r0 = combine(r18,r16) }

l0000C390:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r11); if (p0.new) jump:nt 0000C370; if (p0.new) r2 = memw(r18+20) }

l0000C39C:
	{ jump 0000C620 }

l0000C3A0:
	{ r2 = memw(r19+24) }

l0000C3A4:
	{ if (!p0.new) jump:nt 0000C620; p0 = cmp.eq(r2,#0x23); if (p0.new) r1 = #0x140 }

l0000C3B0:
	{ immext(#0x193C0); r4 = add(PC,#0x193DE); r3:r2 = combine(#0x9,r16) }
	{ call logmsg_function }
	{ call find_first_consumer.1.2_i32_0; r1:r0 = combine(r19,r16) }
	{ r20 = r0; if (cmp.eq(r20.new,r19)) jump:nt 0000C620 }

l0000C3D4:
	{ r2 = memw(r19+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C414 }

l0000C3E0:
	{ r21 = add(r21,#0x1); if (!cmp.gtu(r2,r21.new)) jump:nt 0000C410 }

l0000C3EC:
	{ r2 = r21; r1:r0 = combine(r19,r16) }
	{ p0 = cmp.eq(r0,r12); if (!p0.new) jump:nt 0000C620; if (p0.new) r2 = add(r21,#0x0); if (!p0) r1:r0 = combine(r19,r16) }

l0000C400:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r12); if (p0.new) jump:nt 0000C3E0; if (p0.new) r2 = memw(r19+20) }

l0000C40C:
	{ jump 0000C620 }

l0000C410:
	{ r2 = memw(r18+24); if (cmp.eq(r2.new,#0x13)) jump:t 0000C420 }

l0000C414:
	{ if (cmp.eq(r2.new,#0x13)) jump:t 0000C424 }

l0000C41C:
	{ p0 = cmp.eq(r2,#0x4B) }

l0000C420:
	{ immext(#0x19340); r4 = add(PC,#0x1937D); r3:r2 = combine(#0x9,r16); r1 = #0x145 }

l0000C424:
	{ r4 = add(PC,#0x3D); r3:r2 = combine(#0x9,r16); r1 = #0x145 }

l0000C430:
	{ call logmsg_function }
	{ call find_first_consumer.1.2_i32_0; r1:r0 = combine(r20,r16) }
	{ r21 = r0; if (cmp.eq(r21.new,r20)) jump:nt 0000C620 }

l0000C448:
	{ r2 = memw(r20+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C488 }

l0000C454:
	{ r22 = add(r22,#0x1); if (!cmp.gtu(r2,r22.new)) jump:nt 0000C484 }

l0000C460:
	{ r2 = r22; r1:r0 = combine(r20,r16) }
	{ p0 = cmp.eq(r0,r13); if (!p0.new) jump:nt 0000C620; if (p0.new) r2 = add(r22,#0x0); if (!p0) r1:r0 = combine(r20,r16) }

l0000C474:
	{ call find_last_consumer }
	{ p0 = cmp.eq(r0,r13); if (p0.new) jump:nt 0000C454; if (p0.new) r2 = memw(r20+20) }

l0000C480:
	{ jump 0000C620 }

l0000C484:
	{ r2 = memw(r21+24) }

l0000C488:
	{ r2 = setbit(r2,#0x2); if (!cmp.eq(r2.new,#0x17)) jump:nt 0000C620 }

l0000C494:
	{ r4 = add(PC,#0x1B); r3:r2 = combine(#0x9,r16); r1 = #0x14B }
	{ call logmsg_function }
	{ r2 = memw(r17+12) }
	{ r0 = memw(r2); r1 = memw(r2+4) }
	{ memd(r29+32) = r1:r0 }
	{ r7 = memw(r17+12) }
	{ r4 = memw(r7+8); r5 = memw(r7+12) }
	{ memd(r29+40) = r5:r4 }
	{ r2 = memw(r17+12) }
	{ r0 = memw(r2+16); r1 = memw(r2+20) }
	{ memd(r29+48) = r1:r0 }
	{ r7 = memw(r17+12) }
	{ r4 = memw(r7+24); r5 = memw(r7+28) }
	{ memd(r29+56) = r5:r4 }
	{ r2 = memw(r17+12) }
	{ r0 = memw(r2); r1 = memw(r2+4) }
	{ memd(r29+64) = r1:r0 }
	{ r7 = memw(r17+12) }
	{ r4 = memw(r7+8); r5 = memw(r7+12) }
	{ memd(r29+72) = r5:r4 }
	{ r2 = memw(r17+12) }
	{ r0 = memw(r2+16); r1 = memw(r2+20) }
	{ memd(r29+80) = r1:r0 }
	{ r7 = memw(r19+12) }
	{ r4 = memw(r7+8); r5 = memw(r7+12) }
	{ r4 = #0xA; memd(r29+88) = r5:r4 }
	{ r2 = memw(r19+12) }
	{ r0 = memw(r2); r1 = memw(r2+4) }
	{ memd(r29+96) = r1:r0 }
	{ r5 = memw(r19+12) }
	{ r6 = memw(r5+8); r7 = memw(r5+12) }
	{ memd(r29+104) = r7:r6 }
	{ r2 = memw(r21+24); if (!cmp.eq(r2.new,#0x17)) jump:t 0000C530 }

l0000C528:
	{ r0 = memw(r2+24); r1 = memw(r2+28) }
	{ memd(r29+112) = r1:r0 }

l0000C530:
	{ immext(#0x1EBC0); r6 = add(PC,#0x1EBE0); r3 = setbit(r24,#0x4); r2 = memw(r21+8) }
	{ r5 = #0x3; r2 = memw(r2) }
	{ r2 = #0x31; r7 = memw(r2+20); memb(r29+2) = r7.new }
	{ r1 = memw(r21+8) }
	{ r0 = memw(r1+4) }
	{ r0 = r16; r3 = memw(r0+20); memb(r29+4) = r3.new }
	{ r7 = memw(r21+8) }
	{ r3 = memw(r7+8) }
	{ r3 = memw(r3+20) }
	{ memw(r29+24) = r3; memw(r24+20) = #0xFFFFFF80 }
	{ r6 = memw(r13+r23) }
	{ r6 = memw(r6+196); r1 = memw(r21+28) }
	{ r3 = memb(r17+32) }
	{ r6 = memw(r6+8); memw(r29+4) = r24 }
	{ callr r6; memw(r29) = r25 }
	{ r22 = r0; if (cmp.eq(r22.new,#0x0)) jump:nt 0000C620 }

l0000C5AC:
	{ memw(r26) = r22 }
	{ r7 = memw(r21+36); memb(r22+9) = r7.new }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1:r0 = combine(r16,r18); r2 = memw(r18) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1:r0 = combine(r16,r19); r2 = memw(r19) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1:r0 = combine(r16,r20); r2 = memw(r20) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ r1:r0 = combine(r16,r21); r2 = memw(r21) }
	{ r2 = memw(r2+12) }
	{ callr r2 }
	{ immext(#0x19180); r4 = add(PC,#0x191B3); r3:r2 = combine(#0x3,r16); r1 = #0x176 }
	{ call logmsg_function; r5 = memw(r22+28); memb(r29) = r5.new }

l0000C620:
	{ r2 = memw(r26) }

l0000C624:
	{ r26 = add(r2,#0x24) }
	{ r17 = memw(r2+36); if (!cmp.eq(r17.new,#0x0)) jump:t 0000C2C0 }

l0000C634:
	{ r2 = memw(r17) }

l0000C638:
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 0000C65C; if (!p0.new) r1:r0 = combine(r17,r16) }

l0000C640:
	{ call try_pad_bad_supernodes }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000C748; if (!p0.new) r1 = #0x1CE; if (p0.new) r2 = memw(r17) }

l0000C650:
	{ r17 = add(r2,#0x24) }
	{ jump 0000C638; r2 = memw(r2+36) }

l0000C65C:
	{ call allocate_graph_storage; r0 = r16 }

l0000C660:
	{ r0 = r16 }

l0000C664:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000C734 }

l0000C668:
	{ r17 = memw(r16); if (cmp.eq(r17.new,#0x0)) jump:nt 0000C714 }

l0000C674:
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C700 }

l0000C67C:
	{ r3 = memw(r17+12) }

l0000C680:
	{ r4 = memw(r23+r18<<#3); if (cmp.eq(r4.new,#0x0)) jump:nt 0000C6DC }

l0000C68C:
	{ r2 = memw(r16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C758 }

l0000C698:
	{ r5 = memw(r3+4) }
	{ r2 = memw(r2+36); if (cmp.eq(r2.new,#0x0)) jump:nt 0000C754 }

l0000C6A0:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000C758 }

l0000C6A8:
	{ if (!cmp.eq(r3.new,r4)) jump:t 0000C6A0 }

l0000C6B0:
	{ if (cmp.gtu(r3.new,r5)) jump:t 0000C6D0 }

l0000C6B8:
	{ r3 = add(PC,#0x1A); r1 = #0x7E; memw(r29+4) = r4 }
	{ r2 = r16; memw(r29) = r5 }
	{ jump 0000BF3C }
0000C6CC                                     93 01 22 02             ..".

l0000C6D0:
	{ r2 = memw(r14+r5<<#2) }
	{ jump 0000C6F0; memw(r31+r18<<#2) = r2 }

l0000C6DC:
	{ immext(#0x19040); r4 = add(PC,#0x19058); r3:r2 = combine(#0x1,r16); r1 = #0x83 }
	{ call logmsg_function }

l0000C6F0:
	{ r2 = memw(r17+16) }
	{ r18 = add(r18,#0x1); if (cmp.gtu(r2,r18.new)) jump:t 0000C67C }

l0000C700:
	{ if (!cmp.eq(r17.new,#0x0)) jump:t 0000C674 }

l0000C708:
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:nt 0000C724; r1:r0 = combine(r16,r17); if (!p0.new) r2 = memw(r17) }

l0000C714:
	{ call nn_os_hvx_power_off; r0 = r16 }
	{ r0 = #0x0; jump 0000C734; memb(r16-28) = #0x2 }

l0000C724:
	{ r2 = memw(r2+4) }
	{ callr r2 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000C708; r17 = memw(r17+36) }

l0000C734:
	{ r17:r16 = memd(r29+160); r19:r18 = memd(r29+152) }
	{ r21:r20 = memd(r29+144); r23:r22 = memd(r29+136) }
	{ r25:r24 = memd(r29+128); r27:r26 = memd(r29+120) }
	{ dealloc_return }

l0000C748:
	{ immext(#0x19000); r3 = add(PC,#0x1901E); jump 0000BF38 }

l0000C754:
	{ immext(#0x18F80); r3 = add(PC,#0x18F88); r2 = memw(r17+28); memb(r29+1) = r2.new }

l0000C758:
	{ r3 = add(PC,#0x8); r2 = memw(r17+28); memb(r29+1) = r2.new }

l0000C768:
	{ r2 = r16 }
	{ jump 0000BF3C; memw(r29) = r4 }

;; errlog_function: 0000C774
;;   Called from:
;;     0000BF3C (in do_prepare)
errlog_function proc
	{ immext(#0x18EC0); r0 = add(PC,#0x18EC1); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; try_pad_bad_supernodes.extracted_region: 0000C798
;;   Called from:
;;     0000BDC4 (in try_pad_bad_supernodes)
;;     0000BE48 (in try_pad_bad_supernodes)
try_pad_bad_supernodes.extracted_region proc
	{ memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r17:r16 = combine(r2,r1); r19:r18 = combine(r4,r3); memd(r29+8) = r19:r18 }
	{ r21:r20 = combine(r0,r5); memd(r29) = r21:r20 }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0000C7DC }

l0000C7B4:
	{ call fn00009560; r2 = r16; r1:r0 = combine(r20,r19) }
	{ r0 = add(r19,r16); r2 = r17; r1 = #0x0; r20 = add(r20,r16) }
	{ call fn000095F0; r19 = add(r19,r18) }
	{ r21 = add(r21,#0xFFFFFFFF); if (!cmp.eq(r21.new,#0x0)) jump:t 0000C7B4 }

l0000C7DC:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }

l0000C7E0:
	{ r21:r20 = memd(r29); dealloc_return }

;; find_first_consumer.1.2_i32_0: 0000C7E4
;;   Called from:
;;     0000BBC4 (in try_pad_bad_supernodes)
;;     0000BF84 (in do_prepare)
;;     0000C2DC (in do_prepare)
;;     0000C350 (in do_prepare)
;;     0000C3C0 (in do_prepare)
;;     0000C434 (in do_prepare)
find_first_consumer.1.2_i32_0 proc
	{ immext(#0x240); r0 = #0x240; jump 0000C854 }
0000C7EC                                     00 00 00 00             ....

;; tensor_alloc: 0000C7F0
;;   Called from:
;;     0000B56C (in node_alloc_common)
tensor_alloc proc
	{ r18 = r0; r0 = #0x20; memd(r29-24) = r19:r18; allocframe(#0x10) }
	{ call fn00009500; r17 = r1; memd(r29+8) = r17:r16 }
	{ r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 0000C810 }

l0000C810:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 0000C838; if (p0.new) memw(r16+16) = #0x0 }

l0000C818:
	{ call fn00009550; immext(#0x80); r1:r0 = combine(r17,#0x80) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000C838; memw(r16+16) = r0 }

l0000C82C:
	{ call fn00009510; r16 = #0x0; r0 = r16 }
	{ jump 0000C850 }

l0000C838:
	{ r2 = memw(r18); r3 = memw(r18+12) }
	{ r7 = memw(r18+4); r4 = memw(r18+8) }
	{ memw(r16) = r2; memw(r16+12) = r3 }
	{ memw(r16+4) = r7; memw(r16+8) = r4 }
	{ memw(r16+24) = r17; memw(r16+20) = r17 }
	{ memw(r16+28) = r16 }

l0000C850:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }

l0000C854:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; tensor_dup: 0000C85C
;;   Called from:
;;     000137C0 (in hexagon_nn_const_ctor)
tensor_dup proc
	{ r18 = r0; r0 = #0x20; memd(r29-24) = r19:r18; allocframe(#0x10) }
	{ call fn00009500; r17 = memw(r18+24); memd(r29+8) = r17:r16 }
	{ r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 0000C87C }

l0000C87C:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 0000C8A8 }

l0000C880:
	{ call fn00009550; immext(#0x80); r1:r0 = combine(r17,#0x80) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000C89C; memw(r16+16) = r0 }

l0000C894:
	{ jump 0000C8B0; r2 = memw(r18+24) }

l0000C89C:
	{ call fn00009510; r16 = #0x0; r0 = r16 }
	{ jump 0000C8CC }

l0000C8A8:
	{ r0 = #0x0; r2 = #0x0; memw(r16+16) = #0x0 }

l0000C8B0:
	{ r3 = memw(r18); r4 = memw(r18+4) }
	{ r7 = memw(r18+8); r5 = memw(r18+12) }
	{ memw(r16) = r3; memw(r16+4) = r4 }
	{ memw(r16+8) = r7; memw(r16+12) = r5 }
	{ r1 = memw(r18+16); memw(r16+24) = r17 }
	{ memw(r16+20) = r17; memw(r16+28) = r16 }
	{ call fn00009560 }

l0000C8CC:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; tensor_free: 0000C8D8
;;   Called from:
;;     0000B670 (in node_free_common)
;;     00013944 (in const_dtor)
tensor_free proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r0 = memw(r16+16); if (cmp.eq(r0.new,#0x0)) jump:nt 0000C8EC }

l0000C8EC:
	{ immext(#0xFFFFCC00); r0 = r8; jump 0000C97C; r17:r16 = memd(r29); deallocframe }
0000C8F8                         00 00 00 00 00 00 00 00         ........

;; do_perfinfo_reset: 0000C900
;;   Called from:
;;     0000B1EC (in hexagon_nn_reset_perfinfo)
do_perfinfo_reset proc
	{ r2 = r0; allocframe(#0x0) }
	{ r5:r4 = combine(#0x0,#0x0); r3 = memw(r2); if (cmp.eq(r3.new,#0x0)) jump:nt 0000C91C }

l0000C910:
	{ memd(r3+48) = r5:r4 }

l0000C914:
	{ r3 = memw(r3+36); if (!cmp.eq(r3.new,#0x0)) jump:t 0000C910 }

l0000C91C:
	{ r0 = #0x5; r3 = and(r1,#0xFF); memw(r2+52) = r1 }

l0000C920:
	{ r3 = and(r1,#0xFF); memw(r2+52) = r1 }

l0000C924:
	{ r1 = setbit(r3,#0x10); call fn00009640 }
	{ r0 = #0x0; dealloc_return }

;; do_perfinfo_get: 0000C930
;;   Called from:
;;     0000B190 (in hexagon_nn_get_perfinfo)
do_perfinfo_get proc
	{ r4 = add(r1,#0x8); r0 = #0x0; r3 = memw(r0) }
	{ p0 = cmp.eq(r3,#0x0); if (p0.new) jump:nt 0000C96C; if (!p0.new) r0 = #0x0 }

l0000C940:
	{  }
	{ r0 = add(r0,#0x1); r5 = memw(r3+28); memb(r4-2) = r5.new }
	{ memw(r4-4) = r1 }
	{ r7:r6 = memd(r3+48) }
	{ r4 = add(r4,#0x10); memd(r4) = r7:r6 }
	{ r3 = memw(r3+36); if (!cmp.eq(r3.new,#0x0)) jump:t 0000C940 }

l0000C96C:
	{ jumpr r31 }

l0000C970:
	{ r0 = #-0x1; jumpr r31 }
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
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ r18 = r2; r19 = memw(r16); memd(r29+40) = r19:r18 }
	{ p0 = cmp.eq(r19,#0x0); if (p0.new) r0 = add(r17,#0x0); memd(r29+32) = r21:r20; memd(r29+24) = r23:r22 }
	{ if (!p0) jump:nt 0000C9A8; jump 0000CA2C; memd(r29+16) = r25:r24 }

l0000C9A8:
	{ r21 = #0x0; r0 = r17; r20 = memw(r17+28) }
	{ r2 = memw(r19+16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000CA1C }
	{ r24 = r0; r3 = memw(r19+12) }
	{ r4 = add(r3,r23) }
	{ r4 = memw(r4-4); if (cmp.eq(r4.new,r20)) jump:t 0000C9D8 }
	{ r0 = r24 }
	{ r3 = memw(r29+r23) }
	{ p0 = !cmp.eq(r3,r18); p1 = cmp.eq(r3,r18); if (p0.new) r0 = add(r24,#0x0); if (!p0.new) r0 = add(r19,#0x0) }
	{ if (!p1) jump:nt 0000CA10 }
	{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:nt 0000CA10 }
	{ r3 = memw(r19+28); r2 = memw(r17+28) }
	{ r2 = r16; memw(r29+12) = r2; memw(r29+8) = r17 }
	{ memw(r29+4) = r3; memw(r29) = r19 }
	{ call logmsg_function }
	{ r0 = r24; r2 = memw(r19+16) }
	{ r23 = add(r23,#0x8); r22 = add(r22,#0x1); if (cmp.gtu(r2,r22.new)) jump:t 0000C9BC }
	{ if (p0.new) r21 = #0x1; r19 = memw(r19+36); if (!cmp.eq(r19.new,#0x0)) jump:t 0000C9B4 }

l0000CA2C:
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); dealloc_return }
0000CA3C                                     00 C0 00 7F             ....

;; find_first_consumer: 0000CA40
find_first_consumer proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r18 = r2; r19 = memw(r17); memd(r29+32) = r19:r18 }
	{ memd(r29+24) = r21:r20 }
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 0000CAC4; memd(r29+16) = r23:r22 }

l0000CA5C:
	{ r21 = #0x0; r20 = memw(r16+28) }

l0000CA60:
	{ r2 = memw(r19+16); if (cmp.eq(r2.new,#0x0)) jump:nt 0000CAB4 }

l0000CA64:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0000CAB8 }

l0000CA6C:
	{ r3 = memw(r19+12) }

l0000CA70:
	{ r4 = add(r3,r23) }
	{ r4 = memw(r4-4); if (!cmp.eq(r4.new,r20)) jump:t 0000CAA8 }

l0000CA80:
	{ if (!cmp.eq(r3.new,r18)) jump:t 0000CAAC }

l0000CA88:
	{ jump 0000CAC8; if (!p0.new) r16 = add(r19,#0x0) }
0000CA90 82 07 B3 07 02 40 71 70 28 08 32 E8 0B 08 13 E8 .....@qp(.2.....
0000CAA0 1A C0 00 5A 82 C0 93 91                         ...Z....        

l0000CAA8:
	{ r23 = add(r23,#0x8); r22 = add(r22,#0x1); if (cmp.gtu(r2,r22.new)) jump:t 0000CA6C }

l0000CAAC:
	{ r22 = add(r22,#0x1); if (cmp.gtu(r2,r22.new)) jump:t 0000CA70 }

l0000CAB4:
	{ p0 = cmp.eq(r19,r16); if (p0.new) r21 = #0x1; r19 = memw(r19+36); if (!cmp.eq(r19.new,#0x0)) jump:t 0000CA60 }

l0000CAB8:
	{ if (p0.new) r21 = #0x1; r19 = memw(r19+36); if (!cmp.eq(r19.new,#0x0)) jump:t 0000CA64 }

l0000CAC4:
	{ r0 = r16; r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }

l0000CAC8:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }

l0000CACC:
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ dealloc_return }

;; logmsg_function: 0000CAD4
logmsg_function proc
	{ immext(#0x18DC0); r0 = add(PC,#0x18DC0); r3 = #0x0; allocframe(#0x8) }
	{ immext(#0x18DC0); r4 = add(PC,#0x18DCB); r1 = #0x3C; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }

;; nn_os_workers_kill: 0000CB00
;;   Called from:
;;     0000B7B0 (in do_teardown)
;;     0000CAFC (in logmsg_function)
nn_os_workers_kill proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ call qurt_pipe_send.1.1_i64_0; r0 = memw(r16+68) }
	{ call qurt_pipe_send.1.1_i64_0; r0 = memw(r16+72) }
	{ r0 = memw(r16+72); r17:r16 = memd(r29) }
	{ jump qurt_pipe_send.1.1_i64_0; deallocframe }

;; nn_os_work_for_vector: 0000CB28
;;   Called from:
;;     000118F4 (in avgpool_execute)
;;     000132B4 (in concat_execute)
;;     00015924 (in maxpool_execute)
;;     00019104 (in supernode_execute_hvx)
;;     00019114 (in supernode_execute_hvx)
nn_os_work_for_vector proc
	{ jump fn00009650; r3:r2 = combine(r2,r1); r0 = memw(r0+68) }

;; nn_os_work_for_scalar: 0000CB34
nn_os_work_for_scalar proc
	{ jump fn00009650; r3:r2 = combine(r2,r1); r0 = memw(r0+72) }

;; nn_os_vector_acquire: 0000CB40
;;   Called from:
;;     00014F04 (in matmul_check_ref)
;;     00019428 (in supernode_check_ref)
nn_os_vector_acquire proc
	{ call dspCV_hvx_lock.1.0_i8_2.1_i32_0; allocframe(#0x0) }
	{ r0 = #0x0; dealloc_return }

;; nn_os_vector_release: 0000CB4C
;;   Called from:
;;     00014F54 (in matmul_check_ref)
;;     00019458 (in supernode_check_ref)
nn_os_vector_release proc
	{ jump fn00009660 }

;; nn_os_workers_spawn: 0000CB50
nn_os_workers_spawn proc
	{ allocframe(#0xE8) }
	{ r2 = add(r29,#0x90); memd(r29+224) = r17:r16 }
	{ r16 = add(r2,#0x8); memd(r29+200) = r23:r22; memd(r29+192) = r25:r24 }
	{ r18 = r0; r0 = r16; memd(r29+216) = r19:r18; memd(r29+208) = r21:r20 }
	{ call qurt_sem_init_val.1.1_i16_0; memd(r29+184) = r27:r26 }
	{ r0 = #0x20; r19 = add(r29,#0x4) }
	{ call fn00009500; r17 = add(r18,#0x44); memw(r29+144) = r18; memb(r19+8) = #0x0 }
	{ r1 = add(r29,#0x4); r0 = r17; memw(r29+4) = r0; memw(r19+4) = #0xFFFFFF84 }
	{ call fn00009670 }
	{ call fn00009500; r18 = add(r18,#0x48); r0 = #0x20; memb(r19+8) = #0x0 }
	{ r1 = add(r29,#0x4); r0 = r18; memw(r29+4) = r0; memw(r19+4) = #0xFFFFFF84 }
	{ call fn00009670 }
	{ r22 = #0xFF; r23 = #0x100; r2 = add(r29,#0x10); r3 = add(r29,#0xA8) }
	{ immext(#0x80); r25:r24 = combine(#0x81,#0x0); immext(#0x2000); r27:r26 = combine(#0x2000,#0x1) }
	{ r19 = setbit(r3,#0x4); r21 = add(r2,#0x3C) }

l0000CBE4:
	{ r20 = add(r21,#0xFFFFFFE4); immext(#0xFFC0); r3 = #0xFFFE; memb(r21-28) = r24 }
	{ immext(#0x18D00); r1 = add(PC,#0x18D26); memb(r21-8) = r24; memb(r21-11) = r22 }
	{ r2 = #0x10; r0 = r20; memb(r21-12) = r24; memuh(r21-10) = r23 }
	{ memb(r21-7) = r22; memuh(r21-6) = r3 }
	{ memw(r21-4) = r24; memw(r21) = #0x0 }
	{ call fn00009680 }
	{ call fn00009500; r0 = #0x2000; memb(r21-13) = r24 }
	{ immext(#0x40); r2 = add(PC,#0x60); p0 = cmp.gt(r26,#0x1); memw(r21) = r0 }
	{ r3 = add(r29,#0x90); r1:r0 = combine(r20,r19); memw(r21-4) = r27; memuh(r21-10) = r25 }
	{ r4 = mux(p0,r18,r17) }
	{ call fn00009690; r4 = memw(r4); memb(r29+37) = r4.new }
	{ r25 = add(r25,#0x1); r19 = add(r19,#0x4); r0 = r16 }
	{ r21 = add(r21,#0x20); r26 = add(r26,#0x1); if (!cmp.eq(r26.new,#0x4)) jump:t 0000CBE4 }

l0000CC80:
	{ r17:r16 = memd(r29+224); r19:r18 = memd(r29+216) }
	{ r21:r20 = memd(r29+208); r23:r22 = memd(r29+200) }
	{ r25:r24 = memd(r29+192); r27:r26 = memd(r29+184) }
	{ dealloc_return }

;; qurt_worker: 0000CC94
qurt_worker proc
	{ memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r16 = memw(r0+4); r17 = memw(r0++#8) }
	{ call qurt_sem_add.1.1_i32_1; memd(r29) = r19:r18 }
	{ call fn000096B0; r0 = r16 }
	{ r19:r18 = combine(r1,r0) }
	{ r3:r2 = lsr(r19:r18,#0x20); p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 0000CCD8 }

l0000CCBC:
	{ r1:r0 = combine(r2,r17); callr r18 }
	{ call fn000096B0; r0 = r16 }
	{ r19:r18 = combine(r1,r0) }
	{ r3:r2 = lsr(r19:r18,#0x20); p0 = cmp.eq(r10,#0x0); if (!p0.new) jump:nt 0000CCBC }

l0000CCD8:
	{ r0 = #0x0; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ jump 000096C0; deallocframe }

;; nn_os_hvx_power_on: 0000CCE8
;;   Called from:
;;     0000ACE8 (in do_execute)
;;     0000BF1C (in do_prepare)
;;     00014EFC (in matmul_check_ref)
nn_os_hvx_power_on proc
	{ call fn000096D0; r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000CD00; if (!p0.new) r2 = add(r16,#0x0) }

l0000CCFC:
	{ call errlog_function }

l0000CD00:
	{ r17:r16 = memd(r29); dealloc_return }

;; errlog_function: 0000CD04
;;   Called from:
;;     0000CCFC (in nn_os_hvx_power_on)
errlog_function proc
	{ immext(#0x18C00); r0 = add(PC,#0x18C20); r3 = #0x0; allocframe(#0x8) }
	{ immext(#0x18C00); r4 = add(PC,#0x18C28); r1 = #0xC2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

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
	{ allocframe(#0x8) }
	{ r2 = memw(r0+20); memd(r29) = r17:r16 }
	{ p0 = cmp.eq(r2,#0x5); if (p0.new) jump:nt 0000CD54; nop }

l0000CD44:
	{ p0 = cmp.eq(r2,#0x0); if (!p0.new) jump:nt 0000CD60; if (!p0.new) r0 = #0x0 }

l0000CD4C:
	{ jump fn00009530; r17:r16 = memd(r29); deallocframe }

l0000CD54:
	{ call fn000096F0 }
	{ jump 00009700; r17:r16 = memd(r29); deallocframe }

l0000CD60:
	{ call fn00009710 }
	{ call fn00009710; r0 = #0x1; r16 = r0 }
	{ r17 = r0 }
	{ r1:r0 = combine(r17,r16); r17:r16 = memd(r29); dealloc_return }
0000CD78                         00 40 00 7F 00 C0 00 7F         .@......

;; nn_os_vector_workers_acquire: 0000CD80
;;   Called from:
;;     0000ACF0 (in do_execute)
nn_os_vector_workers_acquire proc
	{ immext(#0x1E380); r18 = add(PC,#0x1E390); memd(r29-24) = r19:r18; allocframe(#0x10) }
	{ r16 = r0; memd(r29+8) = r17:r16 }
	{ immext(#0xFFFFFFC0); r17 = memw(r18-48) }
	{ call qurt_sem_init_val.1.1_i16_0; r0 = r17 }
	{ immext(#0xFFFFFFC0); r18 = memw(r18-32) }
	{ immext(#0x1E8C0); r19 = add(PC,#0x1E8F8); call qurt_sem_init_val.1.1_i16_0; r0 = r18 }
	{ immext(#0x0); r2 = add(PC,#0x30); r3 = add(r19,#0x4); r0 = memw(r16+68) }
	{ call fn00009650 }
	{ call fn000096A0; r0 = r17 }
	{ call qurt_sem_add.1.1_i32_1; r0 = r18 }
	{ call dspCV_hvx_lock.1.0_i8_2.1_i32_0 }
	{ r17:r16 = memd(r29+8); memw(r19) = #0x0 }
	{ r19:r18 = memd(r29); dealloc_return }

;; worker_acquire: 0000CDE8
worker_acquire proc
	{ call dspCV_hvx_lock.1.0_i8_2.1_i32_0; r16 = r1; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ immext(#0x1E300); r17 = add(PC,#0x1E31C); memw(r16) = #0x0 }
	{ call qurt_sem_add.1.1_i32_1; immext(#0xFFFFFFC0); r0 = memw(r17-48) }
	{ immext(#0xFFFFFFC0); r0 = memw(r17-32); r17:r16 = memd(r29) }
	{ jump fn000096A0; deallocframe }

;; nn_os_vector_workers_release: 0000CE20
;;   Called from:
;;     0000AD64 (in do_execute)
nn_os_vector_workers_release proc
	{ immext(#0x1E2C0); r18 = add(PC,#0x1E2F0); memd(r29-24) = r19:r18; allocframe(#0x10) }
	{ r16 = r0; memd(r29+8) = r17:r16 }
	{ immext(#0xFFFFFFC0); r17 = memw(r18-48) }
	{ call qurt_sem_init_val.1.1_i16_0; r0 = r17 }
	{ immext(#0xFFFFFFC0); r18 = memw(r18-32) }
	{ call qurt_sem_init_val.1.1_i16_0; r0 = r18 }
	{ immext(#0x1E840); r4 = add(PC,#0x1E850); r0 = memw(r16+68) }
	{ immext(#0x0); r2 = add(PC,#0x2C); call fn00009650; r3 = add(r4,#0x4) }
	{ call fn000096A0; r0 = r17 }
	{ call qurt_sem_add.1.1_i32_1; r0 = r18; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ jump fn00009660; deallocframe }

;; worker_release: 0000CE88
worker_release proc
	{ call fn00009660; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ immext(#0x1E280); r16 = add(PC,#0x1E280) }
	{ call qurt_sem_add.1.1_i32_1; immext(#0xFFFFFFC0); r0 = memw(r16-48) }
	{ immext(#0xFFFFFFC0); r0 = memw(r16-32); r17:r16 = memd(r29) }
	{ jump fn000096A0; deallocframe }

;; dspCV_hvx_lock.1.0_i8_2.1_i32_0: 0000CEB8
;;   Called from:
;;     0000CB40 (in nn_os_vector_acquire)
;;     0000CDDC (in nn_os_vector_workers_acquire)
;;     0000CDE8 (in worker_acquire)
dspCV_hvx_lock.1.0_i8_2.1_i32_0 proc
	{ jump 00009720; r1:r0 = combine(#0x0,#0x2) }

;; qurt_sem_add.1.1_i32_1: 0000CEC0
;;   Called from:
;;     0000CCA0 (in qurt_worker)
;;     0000CDD4 (in nn_os_vector_workers_acquire)
;;     0000CE00 (in worker_acquire)
;;     0000CE74 (in nn_os_vector_workers_release)
;;     0000CE98 (in worker_release)
qurt_sem_add.1.1_i32_1 proc
	{ immext(#0xFFFFC840); r1 = #0xFFFFC841; jump fn0000CF80 }

;; qurt_sem_init_val.1.1_i16_0: 0000CEC8
;;   Called from:
;;     0000CB70 (in nn_os_workers_spawn)
;;     0000CD98 (in nn_os_vector_workers_acquire)
;;     0000CDA8 (in nn_os_vector_workers_acquire)
;;     0000CE38 (in nn_os_vector_workers_release)
;;     0000CE48 (in nn_os_vector_workers_release)
qurt_sem_init_val.1.1_i16_0 proc
	{ immext(#0xFFFFC840); r0 = #0xFFFFC840; jump fn0000CFA8 }

;; qurt_pipe_send.1.1_i64_0: 0000CED0
;;   Called from:
;;     0000CB08 (in nn_os_workers_kill)
;;     0000CB10 (in nn_os_workers_kill)
;;     0000CB20 (in nn_os_workers_kill)
qurt_pipe_send.1.1_i64_0 proc
	{ jump fn00009650; r3:r2 = combine(#0x0,#0x0) }
0000CED8                         00 00 00 00 00 00 00 00         ........

;; transpack: 0000CEE0
;;   Called from:
;;     00013EA0 (in conv2d_execute_hvx)
;;     00014F50 (in matmul_check_ref)
;;     00019454 (in supernode_check_ref)
transpack proc
	{ r4 = asl(r1,#0x3); p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0000CF74 }

l0000CEE8:
	{ r3 = add(r3,#0x3); r5 = #0x0 }

l0000CEEC:
	{ p0 = cmp.gt(r1,#0x0); if (!p0.new) jump:nt 0000CF68; r8 = add(r5,r2); r9 = add(r1,#0x3) }

l0000CEF8:
	{ r9 = lsr(r9,#0x2); r7:r6 = combine(#0x0,r3) }
	{ loop1(0000CF04,r9) }
	{ r14 = mpyi(r7,r2); r12 = add(r7,#0x2); r13 = add(r7,#0x3); r9 = r6 }
	{ r28 = mpyi(r12,r2); r10 = mpyi(r13,r2); r13:r12 = combine(r0,r0) }
	{ r12 += add(r8,r14); r13 += add(r14,r5); r15:r14 = combine(r0,r0) }
	{ r14 += add(r28,r5); r15 += add(r10,r5) }
	{ loop0(0000CF38,#0x10) }
	{ r28 = memb(r13++#1); memb(r9-3) = r28.new }
	{ memb(r9-2) = r10 }
	{ r28 = memb(r14++#1) }
	{ memb(r9-1) = r28 }
	{ r10 = memb(r15++#1) }
	{ r9 = add(r9,#0x4); memb(r9) = r10 }
	{ r7 = add(r7,#0x4); r6 = add(r6,#0x80); nop }

l0000CF68:
	{ r3 = addasl(r3,r4,#0x2); r5 = add(r5,#0x20); if (cmp.gtu(r2,r5.new)) jump:t 0000CEEC }

l0000CF74:
	{ jumpr r31 }

;; pad2d: 0000CF78
;;   Called from:
;;     0000CF68 (in transpack)
;;     00013E88 (in conv2d_execute_hvx)
;;     00014F3C (in matmul_check_ref)
;;     0001943C (in supernode_check_ref)
pad2d proc
	{ memd(r29-24) = r19:r18; allocframe(#0x30) }
	{ r17:r16 = combine(r5,r3); r19 = r4; memd(r29+40) = r17:r16 }

;; fn0000CF80: 0000CF80
;;   Called from:
;;     0000CEC0 (in qurt_sem_add.1.1_i32_1)
;;     0000CF7C (in pad2d)
fn0000CF80 proc
	{ r19 = r4; memd(r29+40) = r17:r16 }
	{ memd(r29+24) = r21:r20; memd(r29+8) = r25:r24 }
	{ r21:r20 = combine(r0,r1); r22 = r2; memd(r29+16) = r23:r22 }
	{ p0 = cmp.gt(r20,#0x0); if (p0.new) r23 = add(r16,#0x0); r18 = memw(r29+56); memd(r29) = r27:r26 }
	{ if (!p0) jump:nt fn0000CFE0; if (p0) r25 = add(r20,#0x0); if (p0) r24 = sub(r17,r22) }

;; fn0000CFA8: 0000CFA8
;;   Called from:
;;     0000CEC8 (in qurt_sem_init_val.1.1_i16_0)
fn0000CFA8 proc
	{ if (p0) r25 = add(r20,#0x0); if (p0) r24 = sub(r17,r22) }

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
	{ call vmemcpy_asm; r2 = r22; r1:r0 = combine(r21,r23) }
	{ r0 = add(r23,r22); r2 = r24; r1 = r18; r21 = add(r21,r22) }
	{ call vmemset_asm; r23 = add(r23,r17) }
	{ r25 = add(r25,#0xFFFFFFFF); if (!cmp.eq(r25.new,#0x0)) jump:t 0000CFB4 }

;; fn0000CFE0: 0000CFE0
;;   Called from:
;;     0000CFA4 (in fn0000CF80)
;;     0000CFD4 (in fn0000CFB0)
fn0000CFE0 proc
	{ r19 = sub(r19,r20); if (!cmp.gt(r19.new,#0x0)) jump:nt 0000CFFC }

l0000CFE8:
	{ call vmemset_asm; r1:r0 = combine(r18,r16); r2 = r17; r16 = add(r16,r17) }

l0000CFEC:
	{ r1:r0 = combine(r18,r16); r2 = r17; r16 = add(r16,r17) }

l0000CFF4:
	{ r19 = add(r19,#0xFFFFFFFF); if (!cmp.eq(r19.new,#0x0)) jump:t 0000CFE8 }

l0000CFFC:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }

l0000D000:
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); r27:r26 = memd(r29) }
	{ dealloc_return }

;; unpad2d: 0000D010
;;   Called from:
;;     00013FC0 (in conv2d_execute_hvx)
unpad2d proc
	{ memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r17:r16 = combine(r2,r0); r19:r18 = combine(r5,r3); memd(r29+8) = r19:r18; memd(r29) = r21:r20 }
	{ r21 = asl(r19,#0x2); r20 = r4; if (!cmp.gt(r20.new,#0x0)) jump:nt 0000D048 }

l0000D02C:
	{ r16 = addasl(r16,r17,#0x2); call vmemcpy_asm; r2 = r21; r1:r0 = combine(r16,r18) }

l0000D030:
	{ call fn00022964; r2 = r21; r1:r0 = combine(r16,r18) }

l0000D03C:
	{ r18 = addasl(r18,r19,#0x2); r20 = add(r20,#0xFFFFFFFF); if (!cmp.eq(r20.new,#0x0)) jump:t 0000D02C }

l0000D048:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }

l0000D04C:
	{ r21:r20 = memd(r29); dealloc_return }

;; unpad2d_bytes: 0000D050
;;   Called from:
;;     0001A744 (in supernode_execute_hvx_slice)
unpad2d_bytes proc
	{ memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r17:r16 = combine(r2,r0); r19:r18 = combine(r5,r3); memd(r29+8) = r19:r18; memd(r29) = r21:r20 }
	{ r20 = r4; if (!cmp.gt(r20.new,#0x0)) jump:nt 0000D080 }

l0000D068:
	{ call vmemcpy_asm; r1:r0 = combine(r16,r18); r2 = r19; r16 = add(r16,r17) }

l0000D06C:
	{ r1:r0 = combine(r16,r18); r2 = r19; r16 = add(r16,r17) }

l0000D074:
	{ r18 = add(r18,r19); r20 = add(r20,#0xFFFFFFFF); if (!cmp.eq(r20.new,#0x0)) jump:t 0000D068 }

l0000D080:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }

l0000D084:
	{ r21:r20 = memd(r29); dealloc_return }
0000D088                         00 00 00 00 00 00 00 00         ........
0000D090 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemm_cn: 0000D0A0
gemm_cn proc
	{ p0 = cmp.gt(r5,#0x0); if (!p0.new) jump:nt 0000D12C; memd(r29-16) = r17:r16; allocframe(#0x8) }

l0000D0A8:
	{ r8 = #0x0; r6 = memd(r29+16); r7 = memd(r29+20) }

l0000D0B0:
	{ r13 = mpyi(r8,r6); p0 = cmp.gt(r6,#0x0); if (!p0.new) jump:nt 0000D124; r12 = r8 }

l0000D0BC:
	{ loop1(0000D0CC,r6); r9 = r2 }
	{ r12 = add(r0,mpyi(r12,r7)) }
	{ r13 = addasl(r4,r13,#0x2) }
	{ p0 = cmp.gt(r7,#0x0); if (!p0.new) jump:nt 0000D118; r11 = add(r7,#0xFFFFFFFF); r14 = #0x0 }

l0000D0D8:
	{ r15:r14 = combine(r12,#0x0); r28 = add(r9,r6); p0 = cmp.gtu(r7,#0x1); r10 = memb(r9) }
	{ loop0(0000D0FC,r11); r10 = add(r10,r3); r16 = memb(r15++#1) }
	{ if (!p0) jump:nt 0000D114; r11 = add(r16,r1) }

l0000D0FC:
	{ r14 += mpyi(r10,r11); r28 = add(r28,r6); r16 = memb(r28); r17 = memb(r15++#1) }
	{ r10 = add(r16,r3); r11 = add(r17,r1) }

l0000D114:
	{ r14 += mpyi(r10,r11) }

l0000D118:
	{ r9 = add(r9,#0x1); nop; memw(r13++#4) = r14 }

l0000D124:
	{ r8 = add(r8,#0x1); if (!cmp.eq(r8.new,r5)) jump:t 0000D0B0 }

l0000D12C:
	{ r17:r16 = memd(r29); dealloc_return }

;; gemm_co: 0000D130
;;   Called from:
;;     0000D124 (in gemm_cn)
gemm_co proc
	{ allocframe(#0x18) }
	{ r6 = memd(r29+32); memd(r29+16) = r17:r16 }
	{ p0 = cmp.gt(r6,#0x0); if (p0.new) r12 = #0x0; memd(r29+8) = r19:r18; memd(r29) = r21:r20 }
	{ if (!p0) jump:nt 0000D2D8; if (p0) r9 = memw(r29+40); if (p0) r7 = memw(r29+36) }

l0000D150:
	{ r13 = mpyi(r3,r1); p0 = cmp.gt(r5,#0x0); r8 = memw(r29+44) }
	{ r13 = mpyi(r13,r7) }

l0000D160:
	{ p1 = !cmp.eq(r12,00000000); r15:r14 = combine(#0x0,r2) }
	{ p1 = or(p1,!p0); if (p1.new) jump:nt 0000D1B0; if (!p1.new) r14 = #0x0 }

l0000D174:
	{ loop1(0000D178,r5) }
	{  }
	{ if (p1.new) r10 = add(r14,#0x0); memw(r30+r14<<#2) = r13 }
	{ loop0(0000D194,r7); r15 = addasl(r9,r14,#0x2); r28 = r13 }
	{ r10 = add(r0,mpyi(r10,r7)) }
	{ r11 = memb(r10++#1) }
	{ r28 += mpyi(r11,r3) }
	{ nop; nop }
	{ r15:r14 = combine(#0x0,r2) }

l0000D1B0:
	{ r11 = add(r15,r12); r28 = #0x0; p1 = cmp.gt(r7,#0x0); if (p1.new) r10 = add(r14,#0x0) }
	{ if (!p1) jump:nt 0000D1E0; memw(r14+r11<<#2) = r28 }

l0000D1C8:
	{ loop0(0000D1D0,r7); r11 = addasl(r8,r11,#0x2) }
	{ r10 = add(r10,r6); r16 = memb(r10) }
	{ r28 += mpyi(r16,r1) }

l0000D1E0:
	{ r15 = add(r15,#0x1); r14 = add(r14,#0x1) }

l0000D1E4:
	{ r14 = add(r14,#0x1) }

l0000D1E8:
	{ if (!p2.new) jump:nt 0000D1B0; if (!p0) jump:nt 0000D2CC; p2 = cmp.eq(r15,#0x20); if (p2.new) r14 = #0x0 }

l0000D1F8:
	{ r10 = r12; r11 = r14; r15 = r2; r28 = #0x0 }
	{ r10 += mpyi(r14,r6); r11 = add(r0,mpyi(r11,r7)) }
	{ r19 = add(r10,r28); r16 = #0x0; if (p1) r17 = add(r15,#0x0); if (p1) r18 = add(r11,#0x0) }
	{ if (!p1) jump:nt 0000D244; memw(r15+r19<<#2) = r16 }
	{ loop0(0000D230,r7); r19 = addasl(r4,r19,#0x2) }
	{ r17 = add(r17,r6); r20 = memb(r17); r21 = memb(r18++#1) }
	{ r16 += mpyi(r20,r21) }
	{ r15 = add(r15,#0x1) }
	{ if (!p2.new) jump:nt 0000D210; p2 = cmp.eq(r28,#0x20) }
	{ r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r5)) jump:t 0000D1F8 }
	{ r15 = #0x0 }
	{ loop0(0000D2A0,#0xF); r28 = addasl(r9,r15,#0x2); r10 = r12; r11 = r14 }
	{ r10 += mpyi(r15,r6); r18 = memw(r11++#4); r16 = memw(r28) }
	{ r10 = addasl(r4,r10,#0x2) }
	{ r17 = memw(r10) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r19 = memw(r10+4) }
	{ r17 += add(r16,r18) }
	{ r18 = memw(r11++#4); r16 = memw(r28) }
	{ r15 = add(r15,#0x1) }
	{ r19 += add(r16,r18); if (!p1.new) jump:nt 0000D264; p1 = cmp.eq(r15,r5) }

l0000D2CC:
	{ r2 = add(r2,#0x20); r12 = add(r12,#0x20); if (cmp.gtu(r6,r12.new)) jump:t 0000D160 }

l0000D2D8:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }

l0000D2DC:
	{ r21:r20 = memd(r29); dealloc_return }

;; gemsuma_cn: 0000D2E0
gemsuma_cn proc
	{ p0 = cmp.gt(r1,#0x0); if (!p0.new) jump:nt 0000D334; r12 = add(r2,#0xFFFFFFFF); r6 = #0x0 }

l0000D2EC:
	{ loop1(0000D2F0,r1) }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0000D320; r8 = r6; r7 = #0x0 }

l0000D2FC:
	{ loop0(0000D314,r12); r7 = #0x0; p0 = cmp.gtu(r2,#0x1) }
	{ r8 = add(r0,mpyi(r8,r2)) }
	{ if (!p0) jump:nt 0000D31C; r9 = memb(r8++#1) }

l0000D314:
	{ r7 = add(r9,r7); r9 = memb(r8++#1) }

l0000D31C:
	{ r7 = add(r9,r7) }

l0000D320:
	{ r8 = r5; r6 = add(r6,#0x1) }
	{ r8 += mpyi(r7,r4); nop }

l0000D334:
	{ jumpr r31 }
0000D338                         00 40 00 7F 00 C0 00 7F         .@......

;; gemsumb_cn: 0000D340
gemsumb_cn proc
	{ r5 = #0x0; p0 = cmp.gt(r3,#0x0); r2 = #0x0; r6 = r0 }
	{ loop1(0000D350,#0x10) }
	{ if (!p0) jump:nt 0000D3B8; if (p0) r12 = add(r3,#0x3); memw(r30+r5<<#2) = r2 }

l0000D35C:
	{ r7 = addasl(r1,r5,#0x2); r9:r8 = combine(#0x0,r6) }
	{ r12 = lsr(r12,#0x2) }
	{ loop0(0000D36C,r12) }
	{ r12 = memb(r8-1) }
	{ r9 += mpyi(r12,r4) }
	{ r0 = memb(r8) }
	{ r13 += mpyi(r0,r4); r12 = mpyi(r0,r4) }
	{ r14 = mpyi(r14,r4) }
	{ r15 = r14 }
	{ r15 += add(r12,r9) }
	{ r9 = memb(r8+2) }
	{ r9 = mpyi(r9,r4) }
	{ r9 += add(r13,r14) }
	{ nop; memw(r7) = r9 }

l0000D3B8:
	{ nop; nop; r6 = add(r6,#0x4); r5 = r5 }
	{ jumpr r31 }

;; gemmpybbw_cn: 0000D3C8
gemmpybbw_cn proc
	{ p0 = cmp.gt(r3,#0x0); if (!p0.new) jumpr:nt r31 }
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
	{ loop1(0000D480,r3); p0 = cmp.gt(r3,#0x0); if (!p0.new) jump:nt 0000D4C8; r5 = #0x0 }

l0000D480:
	{ r3 = mpyi(r5,r4); r6 = addasl(r0,r5,#0x2); r7 = r1 }
	{ loop0(0000D4A0,#0xF); r3 = addasl(r2,r3,#0x2); r12 = memw(r7++#4); r8 = memw(r6) }
	{ r9 = memw(r3) }
	{ r13 = memw(r3+4) }
	{ r9 += add(r12,r8) }
	{ r8 = memw(r6); r12 = memw(r7++#4) }
	{ r5 = add(r5,#0x1) }
	{ r13 += add(r12,r8); nop }

l0000D4C8:
	{ jumpr r31 }

;; gemm_asm: 0000D4CC
;;   Called from:
;;     0000D4BC (in gemaddvvm_cn)
;;     00013FAC (in conv2d_execute_hvx)
gemm_asm proc
	{ memd(r29-16) = r17:r16; allocframe(#0x58) }
	{ r17 = r0; r16 = memd(r29+96) }
	{ r19 = r2; p0 = cmp.gt(r16,#0x0); memd(r29+72) = r19:r18; memd(r29+64) = r21:r20 }
	{ memd(r29+56) = r23:r22; memw(r29+36) = r1 }
	{ memd(r29+48) = r25:r24; memd(r29+40) = r27:r26 }
	{ memw(r29+12) = r3; memw(r29+32) = r4 }
	{ if (!p0) jump:nt 0000D5A4; if (p0) r25 = memw(r29+112); if (p0) r23 = memw(r29+104) }

l0000D4FC:
	{ r18 = #0x0; r21 = #0x0; r3 = memd(r29+36); r4 = memd(r29+12) }
	{ r22 = memd(r29+108); r2 = memd(r29+100) }
	{ r3 = mpyi(r4,r3); r7 = mpyi(r22,r2); r26 = combine(r25.l,r2.l); r4 = memw(r29+120) }
	{ r2 = mpyi(r3,r2); r4 = memd(r29+124); memw(r29+28) = r4 }
	{ r4 = asl(r22,#0x2); r24 = memw(r29+116); memw(r29+24) = r4 }
	{ memw(r29+20) = r7; memw(r29+16) = r4 }
	{ memw(r29+8) = r2 }

l0000D538:
	{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:nt 0000D54C; r1:r0 = combine(r23,r17); r3:r2 = combine(r24,r26) }

l0000D544:
	{ r5 = memd(r29+8); r4 = memd(r29+12) }
	{ call gemsuma_asm }

l0000D54C:
	{ r2 = memd(r29+28); r3 = memd(r29+36) }
	{ r20 = add(r2,r18); r2 = r25 }
	{ call gemsumb_asm; r1:r0 = combine(r20,r19) }
	{ r5:r4 = combine(r26,r16); r1:r0 = combine(r19,r17); r2 = memw(r29+32) }
	{ r27 = add(r2,r18) }
	{ call gemmpybbw_asm; r3:r2 = combine(r23,r27) }
	{ r3:r2 = combine(r23,r27); r1:r0 = combine(r20,r24); r4 = r16; r5 = memd(r29+24) }
	{ call gemaddvvm_asm; r6 = cmp.eq(r21,#0x0); memb(r29) = r6.new }
	{ r18 = add(r18,r2); r7 = memd(r29+20) }
	{ r19 = add(r19,r7); r21 = add(r21,r22); if (cmp.gtu(r16,r21.new)) jump:t 0000D538 }

l0000D5A4:
	{ r17:r16 = memd(r29+80); r19:r18 = memd(r29+72) }

l0000D5A8:
	{ r21:r20 = memd(r29+64); r23:r22 = memd(r29+56) }
	{ r25:r24 = memd(r29+48); r27:r26 = memd(r29+40) }
	{ dealloc_return }

;; im2col_co: 0000D5B8
;;   Called from:
;;     00013F7C (in conv2d_execute_hvx)
im2col_co proc
	{ r8 = #0x20; allocframe(#0xC0) }
	{ r7 = memw(r29+204); memd(r29+144) = r27:r26 }
	{ r12 = memw(r29+200); r9 = memw(r29+216) }
	{ r26 = mpyi(r7,r3); p0 = cmp.gt(r9,#0x0); memw(r29+120) = r5; memd(r29+176) = r19:r18 }
	{ r6 = add(#0xF,mpyi(r26,r12)); r16 = r4; memd(r29+184) = r17:r16 }
	{ r4 = memw(r29+212); memd(r29+152) = r25:r24 }
	{ r5 = and(r6,#0xFFFFFFF0); memd(r29+168) = r21:r20; memd(r29+160) = r23:r22 }
	{ r19 = max(r5,r8); if (p0) r9 = add(r2,#0x0); memw(r29+36) = r9; memw(r29+100) = r26 }
	{ memw(r29+56) = r0; memw(r29+8) = r4 }
	{ if (!p0) jump:nt 0000D954; if (p0) r0 = memw(r29+36); memw(r29+32) = r19 }

l0000D614:
	{ r6 = mpyi(r7,r12); r4 = mpyi(r26,r12); r24 = memw(r29+208); r13 = memw(r29+220) }
	{ r2 = add(#0xF,mpyi(r6,r3)); r14 = sub(r19,r4); r12 = memw(r29+8); memw(r29+112) = r12 }
	{ p0 = cmp.gt(r12,#0x0); p1 = cmp.gt(r14,#0x0); memw(r29+4) = r4; memw(r29+68) = r3 }
	{ r4 = mpyi(r3,r9); p0 = not(p0); r2 = and(r2,#0xFFFFFFF0); r6 = memw(r29+224) }
	{ r2 = max(r2,r8); p0 = or(p0,!p1); memw(r29+92) = r6; memw(r29+16) = r13 }
	{ r21 = mpyi(r4,r24); r23 = mpyi(r19,r0); r3 = sub(r7,r9); memw(r29+28) = r14 }
	{ r22 = mpyi(r0,r2); memw(r29+12) = r3; memw(r29+52) = r9 }
	{ r3 = add(r24,#0xFFFFFFFF); memw(r29+48) = r13 }
	{ r5 = sub(r7,r13); r7 = add(r24,r1); memw(r29+20) = r7; memw(r29+80) = r3 }
	{ r1 += add(r24,#0xFFFFFFFF); r4 = sub(r5,r9); r6 = #0x0; memw(r29+96) = r1 }
	{ r3 = mpyi(r12,r24); r1 = p0; memw(r29+72) = r1; memw(r29+88) = r7 }
	{ memw(r29+84) = r3; memw(r29+24) = r1 }
	{ memw(r29+44) = r4 }

l0000D6B0:
	{ r2 = mpyi(r6,r19); r8 = #0x0; r7 = memd(r29+112); memw(r29+40) = r6 }
	{ p0 = cmp.gt(r7,#0x0); memw(r29+116) = r2 }
	{ if (!p0) jump:nt 0000D904 }

l0000D6C8:
	{ r3 = memd(r29+44); r2 = memd(r29+40) }
	{ r3 = max(r3,r8); r2 = mpyi(r2,r24); r5 = memd(r29+20); r4 = memd(r29+48) }
	{ r4 = max(r4,r8); r1 = memd(r29+12); r7 = memd(r29+68) }
	{ r4 = mpyi(r7,r4); r3 = sub(r5,r3); r5 = memw(r29+16) }
	{ r3 = mpyi(r7,r3); r2 = sub(r2,r5) }
	{ r0 = add(r1,r2); r6 = sub(#0x0,r2); r1 = memw(r29+120) }
	{ r5 = max(r0,r8); r6 = max(r6,r8); r8 = #0x0; r4 = add(r1,r4) }
	{ r2 = max(r2,r8); r27 = mpyi(r6,r7); memb(r29+16) = r2.new }
	{ r2 = add(r1,r3); memw(r29+60) = r4; memw(r29+104) = r27 }
	{ r2 = add(r5,r6); memw(r29+76) = r2; memw(r29+108) = r0 }
	{ r2 = mpyi(r2,r7) }
	{ r20 = sub(r26,r2) }

l0000D73C:
	{ r2 = memw(r29+92) }
	{ r2 = mpyi(r8,r26); r4 = sub(r8,r2) }
	{ p0 = cmp.gt(r4,#-0x1); if (p0.new) jump:nt 0000D764; if (p0.new) r25 = add(r21,#0x0); if (p0.new) r3 = memw(r29+116) }

l0000D754:
	{ r2 = add(r2,r3); memw(r29+128) = r8 }
	{ jump 0000D7A0; memw(r29+136) = r2 }

l0000D764:
	{ r25 = r21; r17 = r4; r3 = memd(r29+116); r18 = memd(r29+120) }
	{ r18 += add(r2,r3); r19 = r4; r4 = add(r2,r3); memw(r29+128) = r8 }
	{ memw(r29+136) = r4 }

l0000D784:
	{ call vmemset_asm; r2 = r26; r1:r0 = combine(r16,r18); r18 = add(r18,r23) }
	{ r17 = add(r17,r24); if (!cmp.gt(r17.new,#-0x1)) jump:t 0000D784 }

l0000D7A0:
	{ r2 = #0x0; r1 = r24; r7 = memd(r29+84); memw(r29+124) = r4 }
	{ r17 = max(r4,r2); r19 = add(r4,r7); r2 = memw(r29+96) }
	{ r26 = min(r2,r19); r0 = sub(r17,r4) }
	{ call fn00009750 }
	{ if (!p0.new) jump:nt 0000D804; p0 = cmp.gt(r27,#0x0); memw(r29+132) = r0 }

l0000D7D0:
	{ if (!p0.new) jump:nt 0000D804; p0 = cmp.gt(r26,r17); if (p0.new) r3 = memw(r29-120); if (p0.new) r2 = memw(r29-124) }

l0000D7E0:
	{ r21 = r17; r18 = memd(r29+120) }
	{ r2 = mpyi(r2,r23) }
	{ r18 += add(r3,r2) }

l0000D7EC:
	{ call vmemset_asm; r2 = r27; r1:r0 = combine(r16,r18); r18 = add(r18,r23) }
	{ r21 = add(r21,r24); if (cmp.gtu(r26,r21.new)) jump:t 0000D7EC }

l0000D804:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0000D870; r1 = r24; r27 = r25 }

l0000D808:
	{ r1 = r24; r27 = r25 }

l0000D810:
	{ r2 = memw(r29+80) }
	{ r2 = sub(r2,r17) }
	{ call fn00009750; r0 = add(r2,r26) }
	{ r21 = r0; if (!cmp.gt(r21.new,#0x0)) jump:nt 0000D870 }

l0000D82C:
	{ r18 = memw(r29+64) }
	{ r2 = mpyi(r22,r2); r3 = memw(r29+52); r25 = memw(r29+60) }
	{ r6 = memw(r29+136); r7 = memw(r29+68) }
	{ r18 += mpyi(r17,r3); r25 += add(r6,r2); r3 = memw(r29+56) }
	{ r18 = add(r3,mpyi(r18,r7)) }

l0000D854:
	{ call vmemcpy_asm; r2 = r20; r1:r0 = combine(r18,r25); r25 = add(r25,r22) }
	{ r18 = add(r18,r27); r21 = add(r21,#0xFFFFFFFF); if (!cmp.eq(r21.new,#0x0)) jump:t 0000D854 }

l0000D870:
	{ r21 = memw(r29+108); if (!cmp.gt(r21.new,#0x0)) jump:nt 0000D8A8 }

l0000D874:
	{ if (!cmp.gt(r21.new,#0x0)) jump:nt 0000D8AC }

l0000D87C:
	{ p0 = cmp.gt(r26,r17); if (p0.new) r18 = memw(r29+76); if (p0.new) r2 = memw(r29-124) }
	{ r3 = memw(r29+136) }
	{ r2 = mpyi(r22,r2) }
	{ r18 += add(r3,r2) }

l0000D894:
	{ call vmemset_asm; r1:r0 = combine(r16,r18); r2 = r21; r18 = add(r18,r22) }
	{ r17 = add(r17,r24); if (cmp.gtu(r26,r17.new)) jump:t 0000D894 }

l0000D8A8:
	{ r1 = r24; r21 = r27; r2 = memw(r29+88); r26 = memw(r29+100) }

l0000D8AC:
	{ r21 = r27; r2 = memw(r29+88); r26 = memw(r29+100) }

l0000D8B8:
	{ p0 = cmp.gt(r2,r11); if (p0.new) jump:nt 0000D8F0 }

l0000D8BC:
	{ r17 = memd(r29+72); r2 = memd(r29+124) }
	{ call fn00009750; r0 = sub(r17,r2) }
	{ r2 = mpyi(r0,r23); r18 = memw(r29+120); r3 = memw(r29+136) }
	{ r18 += add(r3,r2) }

l0000D8D8:
	{ call vmemset_asm; r2 = r26; r1:r0 = combine(r16,r18); r18 = add(r18,r23) }
	{ r17 = add(r17,r24); if (cmp.gtu(r19,r17.new)) jump:t 0000D8D8 }

l0000D8F0:
	{ r8 = memw(r29+128); r2 = memw(r29+112) }

l0000D8F4:
	{ r2 = memw(r29+112) }

l0000D8F8:
	{ r8 = add(r8,#0x1); r27 = memw(r29+104); if (!cmp.eq(r8.new,r2)) jump:t 0000D73C }

l0000D904:
	{ r0 = memd(r29+24); r19 = memd(r29+28) }

l0000D908:
	{ p0 = r0; if (p0.new) jump:nt 0000D930 }

l0000D910:
	{ r17 = memd(r29+120); r2 = memd(r29+4) }
	{ r3 = memd(r29+116); r18 = memd(r29+8) }
	{ r17 += add(r3,r2) }

l0000D91C:
	{ call vmemset_asm; r1:r0 = combine(r16,r17); r2 = r19; r17 = add(r17,r23) }
	{ r18 = add(r18,#0xFFFFFFFF); if (!cmp.eq(r18.new,#0x0)) jump:t 0000D91C }

l0000D930:
	{ r6 = memd(r29+40); r5 = memd(r29+36) }

l0000D934:
	{ r6 = add(r6,#0x1); r2 = memd(r29+44); r7 = memd(r29+48) }
	{ r2 = add(r2,r24); r4 = sub(r7,r24); p0 = cmp.eq(r6,r5); r19 = memw(r29+32) }
	{ if (!p0) jump:nt 0000D6B0; memw(r29+44) = r2; memw(r29+48) = r4 }

l0000D954:
	{ r2 = memd(r29+36); r4 = memd(r29+8) }
	{ r3 = add(#0x7,mpyi(r2,r4)); r2 = mpyi(r2,r4) }
	{ r3 = and(r3,#0xFFFFFFF8) }
	{ r18 = sub(r3,r2); if (!cmp.gt(r18.new,#0x0)) jump:nt 0000D988 }

l0000D970:
	{ r17 = add(r3,mpyi(r17,r2)) }

l0000D974:
	{ call vmemset_asm; r1:r0 = combine(r16,r17); r2 = r19; r17 = add(r17,r19) }
	{ r18 = add(r18,#0xFFFFFFFF); if (!cmp.eq(r18.new,#0x0)) jump:t 0000D974 }

l0000D988:
	{ r17:r16 = memd(r29+184); r19:r18 = memd(r29+176) }

l0000D98C:
	{ r21:r20 = memd(r29+168); r23:r22 = memd(r29+160) }
	{ r25:r24 = memd(r29+152); r27:r26 = memd(r29+144) }
	{ dealloc_return }

;; im2col_cn: 0000D99C
im2col_cn proc
	{ allocframe(#0xB8) }
	{ r18 = r4; r7 = memw(r29+196); memd(r29+168) = r19:r18 }
	{ r9 = memw(r29+192); r12 = memw(r29+208) }
	{ r19 = mpyi(r7,r3); r16 = r5; r6 = memw(r29+204); memd(r29+176) = r17:r16 }
	{ r8 = add(#0xF,mpyi(r19,r9)); p0 = cmp.gt(r6,#0x0); memd(r29+160) = r21:r20; memd(r29+152) = r23:r22 }
	{ if (p0) r28 = sub(r7,r2); memd(r29+144) = r25:r24; memw(r29+92) = r9 }
	{ r17 = and(r8,#0xFFFFFFF0); memd(r29+136) = r27:r26; memw(r29+16) = r6 }
	{ memw(r29+72) = r16; memw(r29+40) = r0 }
	{ memw(r29+132) = r1; memw(r29+64) = r12 }
	{ if (!p0) jump:nt 0000DC30; if (p0) r13 = memw(r29-44); memw(r29+68) = r17 }

l0000DA00:
	{ r1 = mpyi(r7,r9); r14 = mpyi(r19,r9); r4 = memw(r29+216); r15 = memw(r29+200) }
	{ r20 = mpyi(r3,r2); r6 = mpyi(r4,r2); r5 = sub(r7,r13); memw(r29+96) = r15 }
	{ r8 = add(#0xF,mpyi(r1,r3)); r3 = #0x0; memw(r29+36) = r7; memw(r29+32) = r3 }
	{ r7 = sub(#0x0,r4); memw(r29+24) = r16; memw(r29+28) = r28 }
	{ r0 = lsr(r8,#0x4); r3 = sub(#0x0,r13); memw(r29+20) = r3; memw(r29+52) = r7 }
	{ r7 = sub(r17,r14); memw(r29+44) = r13; memw(r29+60) = r14 }
	{ r1 = mpyi(r12,r0); r3 = mpyi(r15,r2); memw(r29+8) = r3; memw(r29+56) = r7 }
	{ r2 = sub(r5,r2); r7 = sub(#0x0,r6); memw(r29+12) = r3 }
	{ r1:r0 = vaslw(r1:r0,#0x2); memw(r29+4) = r2; memw(r29+48) = r7 }
	{ memd(r29+80) = r1:r0 }

l0000DA74:
	{ if (!p0.new) jump:nt 0000DC00; p0 = cmp.gt(r12,#0x0); if (p0.new) r7 = memw(r29+24) }

l0000DA80:
	{ r2 = memw(r29+44); memb(r29+29) = r2.new }
	{ r2 = memd(r29+20); memw(r29+108) = r2 }
	{ r2 = mpyi(r2,r12); r7 = memw(r29+8) }
	{ r7 = #0x0; memw(r29+104) = r7 }
	{ memw(r29+76) = r2; memw(r29+100) = r7 }

l0000DAA0:
	{ if (!p0.new) jump:nt 0000DB9C; p0 = cmp.gt(r9,#0x0); if (p0.new) r17 = memw(r29+52); if (p0.new) r8 = memw(r29+28) }

l0000DAB0:
	{ r7 = #0x0; r2 = memd(r29+96); r3 = memd(r29+100) }
	{ r2 = mpyi(r3,r2); r6 = memd(r29+104); r4 = memd(r29+44) }
	{ r3 = max(r6,r7); r2 = sub(r2,r4); r5 = memd(r29+108); r1 = memd(r29+116) }
	{ r0 = max(r1,r7); r6 = sub(#0x0,r2); r2 = add(r8,r2); r1 = memw(r29+32) }
	{ r16 = max(r6,r7); r5 = max(r5,r7); r24 = r1; r6 = memw(r29+36) }
	{ r22 = max(r2,r7); r7 = mpyi(r1,r0); r2 = sub(r6,r5); r5 = memw(r29+40) }
	{ r2 = mpyi(r1,r2); r26 = mpyi(r22,r1); r23 = memw(r29+112); memb(r29+32) = r2.new }
	{ r7 = mpyi(r16,r1); r2 = add(r2,r3); r21 = memd(r29+92); memw(r29+124) = r7 }
	{ r24 = add(r5,mpyi(r24,r2)); r2 = sub(r6,r16); memw(r29+120) = r7 }
	{ r25 = sub(r2,r22) }
	{ r27 = mpyi(r25,r1) }

l0000DB30:
	{ p0 = cmp.gt(r9,#-0x1); if (p0.new) jump:nt 0000DB40; nop }

l0000DB38:
	{ r2 = memw(r29+132); if (cmp.gt(r2.new,r17)) jump:t 0000DB48 }

l0000DB40:
	{ r2 = r11; jump 0000DB88; r0 = r23; r1 = and(r18,#0xFF) }

l0000DB44:
	{ r0 = r23; r1 = and(r18,#0xFF) }

l0000DB48:
	{ p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt 0000DB58 }

l0000DB4C:
	{ call fn000095F0; r0 = r23; r1 = and(r18,#0xFF); r2 = memd(r29+120) }

l0000DB58:
	{ if (!p0.new) jump:nt 0000DB74; r1 = r24; p0 = cmp.gt(r25,#0x0); if (p0.new) r2 = memw(r29+124) }

l0000DB68:
	{ call vmemcpy_asm; r0 = add(r23,r2); r2 = r27 }

l0000DB74:
	{ p0 = cmp.gt(r14,#0x0); if (!p0.new) jump:nt 0000DB8C; r1 = and(r18,#0xFF); if (p0.new) r2 = memw(r29-128) }

l0000DB80:
	{ r0 = add(r23,r2); r2 = r26 }

l0000DB88:
	{ call fn000095F0 }

l0000DB8C:
	{ r24 = add(r24,r20); r17 = r17; r23 = add(r23,r19) }
	{ r21 = add(r21,#0xFFFFFFFF); if (!cmp.eq(r21.new,#0x0)) jump:t 0000DB30 }

l0000DB9C:
	{ r1 = and(r18,#0xFF); r2 = memd(r29+76); r21 = memd(r29+100) }

l0000DBA0:
	{ r2 = memd(r29+76); r21 = memd(r29+100) }
	{ r3 = add(r21,r2); r17 = memd(r29+68); r16 = memd(r29+72) }
	{ r3 = mpyi(r3,r17); r0 = r16; r4 = memd(r29+60); r2 = memd(r29+56) }
	{ r0 += add(r3,r4); call fn000095F0 }
	{ r21 = add(r21,#0x1); r2 = memd(r29+96); r3 = memd(r29+108) }
	{ r3 = add(r3,r2); r6 = memd(r29+112); r1:r0 = memd(r29+80) }
	{ r3 = addasl(r6,r0,#0x2); r12 = memw(r29+64); memw(r29+108) = r3 }
	{ p0 = cmp.eq(r21,r12); r7 = memw(r29+116); r9 = memw(r29+92) }
	{ r5 = sub(r7,r2); r6 = memd(r29+104); memw(r29+112) = r3 }
	{ r3 = add(r6,r2) }
	{ memw(r29+100) = r21; memw(r29+116) = r5 }
	{ if (!p0) jump:nt 0000DAA0; memw(r29+104) = r3 }

l0000DC00:
	{ r2 = memd(r29+24); r1:r0 = memd(r29+80) }
	{ r2 = addasl(r2,r1,#0x2); r7 = memd(r29+12); r4 = memd(r29+48) }
	{ r4 = add(r4,r7); r3 = memd(r29+20); r7 = memd(r29+16) }
	{ r3 = r3; r6 = memd(r29+52) }
	{ p0 = cmp.eq(r3,r7); r2 = memd(r29+96); memw(r29+24) = r2 }
	{ r4 = add(r6,r2); memw(r29+48) = r4; memw(r29+20) = r3 }
	{ if (!p0) jump:nt 0000DA74; memw(r29+52) = r4 }

l0000DC30:
	{ r1 = and(r18,#0xFF); r3 = memd(r29+16) }
	{ r2 = add(#0x7,mpyi(r12,r3)); r0 = mpyi(r12,r3) }
	{ r2 = and(r2,#0xFFFFFFF8) }
	{ r0 = add(r16,mpyi(r0,r17)); r2 = sub(r2,r0) }
	{ r2 = mpyi(r2,r17) }
	{ call fn000095F0; r17:r16 = memd(r29+176); r19:r18 = memd(r29+168) }
	{ r21:r20 = memd(r29+160); r23:r22 = memd(r29+152) }
	{ r25:r24 = memd(r29+144); r27:r26 = memd(r29+136) }
	{ dealloc_return }

;; im2col_slice_v0_co: 0000DC64
im2col_slice_v0_co proc
	{ memd(r29-40) = r23:r22; allocframe(#0xC8) }
	{ r23 = memw(r29+212); memd(r29+184) = r19:r18 }
	{ r27 = r5; r7 = memw(r29+208); memd(r29+152) = r27:r26 }
	{ r26 = memw(r29+224); memw(r29+104) = r7 }
	{ r19 = mpyi(r23,r3); r18 = memw(r29+220); memd(r29+160) = r25:r24 }
	{ r8 = mpyi(r19,r7); r17 = r2; memd(r29+192) = r17:r16; memw(r29+72) = r3 }
	{ r3 = add(#0x7,mpyi(r26,r18)); r7 = add(#0xF,mpyi(r19,r7)); r2 = memw(r29+232); r6 = memw(r29+240) }
	{ r20 = mpyi(r26,r18); memd(r29+176) = r21:r20; memw(r29+144) = r1 }
	{ r2 = and(r3,#0xFFFFFFF8); memw(r29+140) = r2; memw(r29+92) = r8 }
	{ r24 = memw(r29+236); memw(r29+44) = r26 }
	{ r3 = and(r7,#0xFFFFFFF0); r2 = sub(r2,r20) }
	{ r1:r0 = combine(r26,r24); r21 = r4; memw(r29+68) = r0 }
	{ r2 = sub(r3,r8); memw(r29+4) = r2; memw(r29+96) = r3 }
	{ r22 = add(r6,r24); memw(r29+88) = r2 }
	{ r25 = memw(r29+216) }
	{ r16 = memw(r29+228) }
	{ call fn00009750 }
	{ r2 = mpyi(r0,r26); p0 = cmp.gt(r20,r22); if (!p0.new) r7 = add(r18,#0xFFFFFFFF); memw(r29+40) = r0 }
	{ r20 = sub(r24,r2) }
	{ r2 = p0; memw(r29+36) = r20 }
	{ if (p0) jump:nt 0000DD24; jump 0000DD40; memw(r29+8) = r2 }

l0000DD24:
	{ call fn00009750; r18 = r0; r1:r0 = combine(r26,r22) }
	{ r7 = r0; r0 = r18 }
	{ r2 = mpyi(r7,r26) }
	{ r2 = sub(r22,r2) }
	{ memw(r29+12) = r2 }

l0000DD40:
	{ p0 = cmp.gt(r0,r7); if (p0.new) jump:nt 0000DFB0; if (!p0.new) r6 = add(r25,#0x0); memw(r29+32) = r7 }

l0000DD4C:
	{ r3 = mpyi(r0,r6); r4 = sub(r23,r16); r2 = memd(r29+104); r5 = memd(r29+72) }
	{ r2 = mpyi(r23,r2); r9 = sub(#0x0,r16); r1 = memw(r29+140); memw(r29+108) = r6 }
	{ r6 = mpyi(r6,r17); r2 = add(#0xF,mpyi(r2,r5)); r3 = sub(r3,r1); r8 = r0 }
	{ r25 = mpyi(r5,r17); r9 = sub(r23,r17); memw(r29+28) = r9; memw(r29+84) = r3 }
	{ r3 = mpyi(r17,r3); r4 = sub(r4,r17); memw(r29+60) = r9; memw(r29+24) = r6 }
	{ r2 = lsr(r2,#0x4); memw(r29+64) = r23; memw(r29+76) = r16 }
	{ memw(r29+20) = r4; memw(r29+80) = r3 }
	{ memw(r29+16) = r2 }
	{ if (p0.new) jump:nt 0000DDC8; r2 = r26; r9 = r20; p0 = cmp.eq(r8,r0) }

l0000DDB8:
	{ p0 = cmp.eq(r8,r7); r9 = #0x0; if (!p0.new) r2 = add(r26,#0x0) }
	{ if (p0) r2 = memw(r29+12) }

l0000DDC8:
	{ if (!p0.new) jump:nt 0000DF8C; p0 = cmp.gt(r2,r9); memw(r29+100) = r2; if (p0.new) memw(r29+52) = r8 }

l0000DDD8:
	{ r2 = sub(r2,r9); r4 = memd(r29+108); r5 = memd(r29+16) }
	{ r3 = mpyi(r4,r9); r6 = r4; r7 = memd(r29+20) }
	{ r2 = mpyi(r2,r5); r6 = add(r7,mpyi(r6,r9)); r5 = memw(r29+76); memw(r29+124) = r27 }
	{ r3 = sub(r5,r3); r7 = memw(r29+28); memw(r29+56) = r27 }
	{ r4 = add(r7,mpyi(r4,r9)); r2 = asl(r2,#0x4); memw(r29+128) = r6; memw(r29+120) = r3 }
	{ memw(r29+116) = r4; memw(r29+48) = r2 }

l0000DE18:
	{ r8 = #0x0; r2 = memw(r29+104); memw(r29+112) = r9 }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0000DF1C; if (p0.new) r7 = memw(r29+120); if (p0.new) r6 = memw(r29-128) }

l0000DE30:
	{ r2 = memd(r29+108); r3 = memd(r29+112) }
	{ r2 = mpyi(r3,r2); r3 = max(r6,r8); r4 = memd(r29+76); r1 = memd(r29+72) }
	{ r0 = max(r7,r8); r2 = sub(r2,r4); r7 = memd(r29+60); r16 = memd(r29+104) }
	{ r6 = sub(#0x0,r2); r2 = add(r7,r2); r5 = memd(r29+116); r4 = memd(r29+68) }
	{ r23 = max(r6,r8); r20 = max(r2,r8); r27 = r1; r6 = memw(r29+64) }
	{ r5 = max(r5,r8); r7 = mpyi(r1,r0); r2 = sub(r6,r3); r18 = memw(r29+84) }
	{ r2 = mpyi(r1,r2); r22 = mpyi(r20,r1); r26 = memw(r29+124); memb(r29+35) = r2.new }
	{ r7 = mpyi(r23,r1); r2 = add(r2,r5); memw(r29+136) = r7 }
	{ r27 = add(r4,mpyi(r27,r2)); r2 = sub(r6,r23); memw(r29+132) = r7 }
	{ r24 = sub(r2,r20) }
	{ r17 = mpyi(r24,r1) }

l0000DEAC:
	{ p0 = cmp.gt(r10,#-0x1); if (p0.new) jump:nt 0000DEBC; nop }

l0000DEB4:
	{ r2 = memw(r29+144); if (cmp.gt(r2.new,r18)) jump:t 0000DEC4 }

l0000DEBC:
	{ r2 = r11; jump 0000DF04; r1:r0 = combine(r21,r26) }

l0000DEC0:
	{ r1:r0 = combine(r21,r26) }

l0000DEC4:
	{ p0 = cmp.gt(r15,#0x0); if (!p0.new) jump:nt 0000DED4; if (!p0) r1:r0 = combine(r21,r26); if (p0.new) r2 = memw(r29-124) }

l0000DED0:
	{ call vmemset_asm }

l0000DED4:
	{ if (!p0.new) jump:nt 0000DEF0; p0 = cmp.gt(r24,#0x0); if (p0.new) r1 = add(r27,#0x0); if (p0.new) r2 = memw(r29-120) }

l0000DEE4:
	{ call vmemcpy_asm; r0 = add(r26,r2); r2 = r17 }

l0000DEF0:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0000DF08; if (p0.new) r1 = add(r21,#0x0); if (p0.new) r2 = memw(r29-116) }

l0000DEFC:
	{ r0 = add(r26,r2); r2 = r22 }

l0000DF04:
	{ call vmemset_asm }

l0000DF08:
	{ r27 = add(r27,r25); r18 = add(r18,#0x1); r26 = add(r26,r19) }
	{ r16 = add(r16,#0xFFFFFFFF); if (!cmp.eq(r16.new,#0x0)) jump:t 0000DEAC }

l0000DF1C:
	{ r1 = r21; r2 = memd(r29+92); r16 = memd(r29+124) }

l0000DF20:
	{ r2 = memd(r29+92); r16 = memd(r29+124) }

l0000DF24:
	{ call vmemset_asm; r0 = add(r16,r2); r2 = memw(r29+88) }
	{ r2 = memd(r29+108); r3 = memd(r29+116) }
	{ r3 = add(r3,r2); r7 = memw(r29+120); r6 = memw(r29+128) }
	{ r5 = sub(r7,r2); r3 = add(r6,r2); r9 = memw(r29+112); memw(r29+116) = r3 }
	{ r9 = add(r9,#0x1); r2 = memd(r29+100); r7 = memd(r29+96) }
	{ r16 = add(r16,r7); p0 = cmp.eq(r9,r2); memw(r29+120) = r5; memw(r29+128) = r3 }
	{ if (!p0) jump:nt 0000DE18; if (p0) r2 = memw(r29+48); memw(r29+124) = r16 }

l0000DF74:
	{ r27 = memw(r29+56); r26 = memw(r29+44) }
	{ r27 = add(r27,r2); r0 = memd(r29+40); r6 = memd(r29+24) }
	{ r20 = memd(r29+36); r7 = memd(r29+32) }
	{ r8 = memw(r29+52) }

l0000DF8C:
	{ r2 = memw(r29+80) }
	{ p0 = cmp.gt(r7,r8); r8 = add(r8,#0x1); r5 = memd(r29+108); r3 = memd(r29+84) }
	{ r2 = add(r2,r6); r3 = add(r3,r5); memb(r29+20) = r2.new }
	{ memw(r29+84) = r3 }

l0000DFB0:
	{ r0 = memw(r29+8) }
	{ p0 = r0; if (!p0.new) jump:nt 0000DFD8; if (!p0.new) r1:r0 = combine(r21,r27); if (!p0.new) r25:r24 = memd(r29+160) }

l0000DFC4:
	{ r17:r16 = memd(r29+192); r19:r18 = memd(r29+184) }
	{ r21:r20 = memd(r29+176); r23:r22 = memd(r29+168) }
	{ r25:r24 = memd(r29+160); r27:r26 = memd(r29+152) }
	{ dealloc_return }

l0000DFD8:
	{ r2 = memd(r29+96); r3 = memd(r29+4) }
	{ r2 = mpyi(r3,r2); r17:r16 = memd(r29+192); r19:r18 = memd(r29+184) }
	{ r21:r20 = memd(r29+176); r23:r22 = memd(r29+168) }
	{ r27:r26 = memd(r29+152) }
	{ jump vmemset_asm; deallocframe }

;; im2col_slice_co: 0000DFF4
im2col_slice_co proc
	{ allocframe(#0x88) }
	{ r21 = r2; r6 = memw(r29+144); memd(r29+112) = r21:r20 }
	{ r2 = memw(r29+164); memd(r29+88) = r27:r26 }
	{ r7 = memw(r29+168); memw(r29+48) = r2 }
	{ r2 = mpyi(r6,r3); r27 = memw(r29+148); memd(r29+96) = r25:r24 }
	{ r22 = r1; memd(r29+104) = r23:r22; memd(r29+120) = r19:r18 }
	{ r19 = memw(r29+160) }
	{ r2 = add(#0xF,mpyi(r2,r27)); r23 = memw(r29+172); r20 = memw(r29+176) }
	{ r1:r0 = combine(r19,r23); r2 = and(r2,#0xFFFFFFF0); memw(r29+80) = r0; memd(r29+128) = r17:r16 }
	{ r18 = add(r20,r23); r16 = r5; memw(r29+24) = r19 }
	{ memw(r29+72) = r6; memw(r29+84) = r3 }
	{ r17 = memw(r29+156); memw(r29+64) = r2 }
	{ memw(r29+76) = r4; memw(r29+68) = r7 }
	{ r24 = memw(r29+152) }
	{ call fn00009750 }
	{ r2 = mpyi(r19,r17); r25 = r0 }
	{ r3 = mpyi(r25,r19); memw(r29+20) = r25 }
	{ r26 = sub(r23,r3); r23 = r18 }
	{ p0 = cmp.gt(r2,r15); if (p0.new) jump:nt 0000E090; if (!p0) r1:r0 = combine(r19,r23); memw(r29+16) = r26 }

l0000E088:
	{ jump 0000E0A4; r17 = r17; memw(r29+40) = r19 }

l0000E090:
	{ call fn00009750 }
	{ r17 = r0 }
	{ r2 = mpyi(r17,r19) }
	{ r2 = sub(r23,r2) }
	{ memw(r29+40) = r2 }

l0000E0A4:
	{ r0 = r16; r2 = memd(r29+64); r1 = memd(r29+76) }
	{ r2 = mpyi(r2,r20); memw(r29+12) = r17 }
	{ call vmemset_asm }
	{ r3 = mpyi(r25,r24); if (p0.new) jump:nt 0000E224; r5 = r22; p0 = cmp.gt(r25,r17) }

l0000E0C8:
	{ r7 = mpyi(r24,r21); r4 = sub(r27,r21); r2 = memd(r29+72); r0 = memd(r29+84) }
	{ r2 = mpyi(r27,r2); r6 = r25; r23 = #0x0; r1 = memd(r29+68) }
	{ r2 = add(#0xF,mpyi(r2,r0)); r3 = sub(r3,r1); memb(r29+15) = r3.new }
	{ r2 = lsr(r2,#0x4); memw(r29+52) = r24; memw(r29+8) = r7 }
	{ memw(r29+44) = r4; memw(r29+56) = r3 }
	{ memw(r29+4) = r2 }
	{ p1 = cmp.eq(r6,r25); p0 = cmp.eq(r6,r17); r2 = memw(r29+40) }
	{ if (!p0) r2 = add(r19,#0x0); r4 = mux(p1,r26,#0x0) }
	{ if (p1) r2 = add(r19,#0x0) }
	{ p0 = cmp.gt(r2,r4); if (p0.new) jump:nt 0000E200; memw(r29+68) = r2; if (p0.new) memw(r29+36) = r16 }

l0000E128:
	{ r2 = sub(r2,r4); r22 = r16; r3 = memd(r29+4); memw(r29+32) = r6 }
	{ r2 = mpyi(r2,r3) }
	{ r2 = asl(r2,#0x4) }
	{ memw(r29+28) = r2 }

l0000E140:
	{ r2 = memd(r29+72); memw(r29+76) = r4 }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0000E1D4; if (p0.new) r25 = memw(r29+60); if (p0.new) r7 = memw(r29+48) }

l0000E150:
	{ r17 = #0x0; r2 = memd(r29+52); r3 = memd(r29+76) }
	{ r2 = mpyi(r3,r2); r26 = memw(r29+56); r20 = memw(r29+72) }
	{ r24 = sub(r2,r7); r2 = memw(r29+44) }
	{ r19 = sub(#0x0,r24); r18 = add(r2,r24) }

l0000E174:
	{ if (!p0.new) jump:nt 0000E1C0; p0 = cmp.gt(r25,#0xFFFFFFFF) }

l0000E17C:
	{ r3 = max(r23,r19); if (!p0.new) jump:nt 0000E1C0; p0 = cmp.gt(r5,r25) }

l0000E188:
	{ r2 = max(r23,r18); r4 = sub(r27,r3) }
	{ r2 = sub(r4,r2); if (!cmp.gt(r2.new,#0x0)) jump:nt 0000E1C0 }

l0000E19C:
	{ r0 = add(r3,r17); r6 = memd(r29+84); r3 = memd(r29+80) }
	{ r0 = add(r22,mpyi(r0,r6)); r1 = add(r4,r26); r16 = r5 }
	{ r2 = mpyi(r2,r6); r1 = add(r3,mpyi(r1,r6)) }
	{ call vmemcpy_asm }
	{ r5 = r16 }

l0000E1C0:
	{ r26 = add(r26,r21); r25 = add(r25,#0x1); r17 = add(r17,r27) }
	{ r20 = add(r20,#0xFFFFFFFF); if (!cmp.eq(r20.new,#0x0)) jump:t 0000E174 }

l0000E1D4:
	{ r4 = memd(r29+76); r2 = memd(r29+64) }

l0000E1D8:
	{ r22 = add(r22,r2); r7 = memd(r29+68) }
	{ r4 = add(r4,#0x1); if (!cmp.eq(r4.new,r7)) jump:t 0000E140 }

l0000E1E8:
	{ r16 = add(r16,r2); r19 = memw(r29+24); r25 = memw(r29+20) }
	{ r26 = memw(r29+16); r17 = memw(r29+12) }
	{ r6 = memd(r29+32); r7 = memd(r29+8) }

l0000E200:
	{ r2 = memw(r29+56) }
	{ p0 = cmp.gt(r17,r6); r6 = add(r6,#0x1); r4 = memd(r29+52); r3 = memd(r29+60) }
	{ r2 = add(r2,r7); r3 = add(r3,r4); memb(r29+14) = r2.new }
	{ memw(r29+60) = r3 }

l0000E224:
	{ r17:r16 = memd(r29+128); r19:r18 = memd(r29+120) }
	{ r21:r20 = memd(r29+112); r23:r22 = memd(r29+104) }
	{ r25:r24 = memd(r29+96); r27:r26 = memd(r29+88) }
	{ dealloc_return }

;; fast_im2col_co: 0000E238
;;   Called from:
;;     0001A564 (in supernode_execute_hvx_slice)
fast_im2col_co proc
	{ allocframe(#0x68) }
	{ r9 = memw(r29+124); r6 = memw(r29+112) }
	{ r6 = add(r6,r9); r17 = r3; r7 = memw(r29+128); memd(r29+96) = r17:r16 }
	{ r7 += add(r6,#0xFFFFFFFF); r19:r18 = combine(r5,r4); memd(r29+88) = r19:r18; memd(r29+72) = r23:r22 }
	{ p0 = cmp.gt(r9,r7); if (!p0.new) r3 = add(r17,#0xF); memd(r29+80) = r21:r20; memw(r29+36) = r7 }
	{ memd(r29+64) = r25:r24; memd(r29+56) = r27:r26 }
	{ if (p0) jump:nt 0000E414; if (!p0) r8 = memw(r29-124); memw(r29+52) = r1 }

l0000E280:
	{ r20 = mpyi(r17,r2); r4 = lsr(r3,#0x4); r12 = memw(r29+120); r5 = memw(r29+136) }
	{ r4 = mpyi(r4,r8); r6 = mpyi(r9,r12); r27 = and(r3,#0xFFFFFFF0); r13 = memw(r29+140) }
	{ r7 = mpyi(r12,r17); r15 = add(r5,r2); r26 = r20; r14 = memw(r29+144) }
	{ r21 = mpyi(r5,r17); r4 = asl(r4,#0x4); r5 = sub(r6,r13); r23 = sub(r27,r17) }
	{ r26 = add(r0,mpyi(r26,r5)); r3 = sub(r13,r6); r14 = memw(r29+52); memw(r29+28) = r14 }
	{ r22 = mpyi(r15,r17); r4 = sub(#0xFFFFFFFF,r3); memw(r29+4) = r4; memw(r29+32) = r12 }
	{ r2 = mpyi(r7,r2); r8 = sub(#0x0,r12); r16 = add(r13,r14); memw(r29+12) = r8 }
	{ memw(r29+20) = r13; memw(r29+8) = r8 }
	{ memw(r29+16) = r16; memw(r29+24) = r2 }

l0000E2FC:
	{ r2 = memw(r29+28); if (cmp.eq(r2.new,#0x0)) jump:nt 0000E36C }

l0000E308:
	{ if (p0.new) jump:nt 0000E324; p0 = cmp.gt(r12,#0x0); if (p0.new) r3 = memw(r29+20) }

l0000E314:
	{ memw(r29+40) = r4; memw(r29+44) = r5 }
	{ jump 0000E3E8; memw(r29+48) = r9 }
0000E320 19 C2 23 F3                                     ..#.            

l0000E324:
	{ r2 = memw(r29+16); if (cmp.gtu(r25,r2.new)) jump:nt 0000E314 }

l0000E330:
	{ r24 = r26; r3 = memw(r29+8); memw(r29+48) = r9 }
	{ r2 = sub(r4,r2) }
	{ r2 = maxu(r3,r2); memw(r29+44) = r5; memw(r29+40) = r4 }
	{ r16 = sub(#0x0,r2) }

l0000E34C:
	{ if (!p0.new) jump:nt 0000E35C; p0 = cmp.gt(r25,#0xFFFFFFFF) }

l0000E354:
	{ r2 = memw(r29+52); if (cmp.gt(r2.new,r25)) jump:t 0000E3B8 }

l0000E35C:
	{ call vmemset_asm; r2 = r22; r1:r0 = combine(r18,r19) }

l0000E360:
	{ r2 = r22; r1:r0 = combine(r18,r19) }

l0000E368:
	{ jump 0000E3D4 }

l0000E36C:
	{ r25:r24 = combine(r19,r26); r2 = memd(r29+12); memw(r29+40) = r4 }
	{ p0 = cmp.gt(r2,#0x0); r16 = r2; memw(r29+44) = r5 }
	{ if (!p0) jump:nt 0000E3E8; memw(r29+48) = r9 }

l0000E384:
	{ call vmemcpy_asm; r2 = r17; r1:r0 = combine(r24,r25) }
	{ call vmemset_asm; r0 = add(r25,r17); r2 = r23; r1 = r18 }
	{ r24 = add(r24,r17); r25 = add(r25,r27); r16 = add(r16,#0xFFFFFFFF); if (!cmp.eq(r16.new,#0x0)) jump:t 0000E384 }

l0000E3B0:
	{ jump 0000E3E8; r19 = add(r19,r2) }

l0000E3B8:
	{ call vmemset_asm; r2 = r21; r1:r0 = combine(r18,r19) }
	{ call vmemcpy_asm; r0 = add(r19,r21); r2 = r20; r1 = r24 }

l0000E3D4:
	{ r24 = add(r24,r20); r25 = add(r25,#0x1); r19 = add(r19,r22) }
	{ r16 = add(r16,#0xFFFFFFFF); if (!cmp.eq(r16.new,#0x0)) jump:t 0000E34C }

l0000E3E8:
	{ r9 = memw(r29+48); r2 = memw(r29+36) }

l0000E3EC:
	{ r2 = memw(r29+36) }
	{ p0 = cmp.gt(r2,r9); r9 = add(r9,#0x1); r7 = memw(r29+24); r12 = memw(r29+32) }
	{ r26 = add(r26,r7); r5 = memd(r29+44); r4 = memd(r29+40) }
	{ if (p0) jump:nt 0000E2FC; r5 = add(r5,r12); r4 = add(r4,r12) }

l0000E414:
	{ r17:r16 = memd(r29+96); r19:r18 = memd(r29+88) }
	{ r21:r20 = memd(r29+80); r23:r22 = memd(r29+72) }
	{ r25:r24 = memd(r29+64); r27:r26 = memd(r29+56) }
	{ dealloc_return }
0000E428                         00 00 00 00 00 00 00 00         ........

;; deconv_execute_ref: 0000E430
deconv_execute_ref proc
	{ allocframe(#0xB8) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+176) = r17:r16; memd(r29+152) = r23:r22 }
	{ r22 = memb(r0+32); r16 = memw(r2) }
	{ r5 = memw(r2+24); r7 = memw(r3) }
	{ p0 = cmp.eq(r22,#0x0); r17 = memw(r2+4); memd(r29+168) = r19:r18 }
	{ memd(r29+160) = r21:r20; memd(r29+144) = r25:r24 }
	{ r7 = memw(r16+4); memw(r29+104) = r7 }
	{ r19 = memw(r2+12); r20 = memw(r2+8) }
	{ r21 = memw(r2+20); r24 = memw(r2+16) }
	{ r2 = memw(r16+12); memd(r29+136) = r27:r26 }
	{ r0 = memw(r16+8); memw(r29+88) = r0 }
	{ r26 = memw(r16); memw(r29+92) = r7 }
	{ r7 = memw(r17+12); memw(r29+124) = r2 }
	{ r2 = memw(r17); r18 = memw(r5+8) }
	{ r27 = memw(r5+4); r4 = memw(r3+8) }
	{ r1 = p0; r3 = memw(r3+4); memw(r29+16) = r1 }
	{ memw(r29+100) = r7 }
	{ r7 = memw(r17+4); memw(r29+120) = r2 }
	{ r2 = memw(r17+8) }
	{ memw(r29+80) = r5; memw(r29+128) = r0 }
	{ memw(r29+76) = r4; memw(r29+72) = r3 }
	{ memw(r29+20) = r26; memw(r29+32) = r7 }
	{ memw(r29+84) = r2; memw(r29+108) = r1 }
	{ if (p0) jump:nt 0000E4E4 }

l0000E4BC:
	{ p0 = cmp.eq(r14,#0x2); if (p0.new) jump:nt 0000E4DC; if (p0.new) r3 = memw(r29+32); if (p0.new) r2 = memw(r29-128) }

l0000E4C8:
	{ p0 = cmp.eq(r14,#0x1); if (!p0.new) jump:nt 0000E4F0; r23 = #0x0; r0 = r18 }

l0000E4D0:
	{ r2 = memw(r29+128) }
	{ r0 += add(r2,#0xFFFFFFFF); jump 0000E4E4 }

l0000E4DC:
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r18) }

l0000E4E4:
	{ call fn00009760; r1 = r18 }
	{ r23 = r0 }

l0000E4F0:
	{ p0 = cmp.eq(r22,#0x2); if (!p0.new) r1 = add(r27,#0x0) }
	{ r0 = p0; if (p0) jump:nt 0000E53C; if (p0) r1 = add(r27,#0x0); memb(r29+29) = r0.new }

l0000E50C:
	{ if (!p0.new) r1 = add(r27,#0x0); if (p0.new) r2 = memw(r29+92) }
	{ r2 = #0x0; r0 = memw(r29+108); memb(r29+11) = r2.new }
	{ if (!p0.new) jump:nt 0000E554; if (p0.new) r0 = memw(r29+92) }

l0000E52C:
	{ jump 0000E548 }
0000E530 00 C0 61 70 E0 5F 02 E2 0A C0 00 58             ..ap._.....X    

l0000E53C:
	{ r2 = memd(r29+92); r3 = memd(r29+120) }
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r1) }

l0000E548:
	{ call fn00009760 }
	{ memw(r29+44) = r0 }
	{ immext(#0x437F0000); r20 = #0x437F0000; r2 = memw(r19+16); r3 = memw(r20+16) }

l0000E554:
	{ r20 = #0x0; r2 = memw(r19+16); r3 = memw(r20+16) }

l0000E55C:
	{ r6 = memw(r17+16); r4 = memw(r24+16) }
	{ r2 = memw(r2); r17 = memw(r3) }
	{ r5 = memw(r21+16); memw(r29+56) = r6 }
	{ r21 = sfsub(r2,r17); r7 = memw(r16+16); r6 = memd(r29+104) }
	{ r1:r0 = combine(r20,r21) }
	{ r16 = memw(r5); memw(r29+96) = r7 }
	{ r6 = memw(r6+16) }
	{ call fn00009610; r19 = memw(r4); memw(r29+52) = r6 }
	{ r24 = sfsub(r16,r19); r16 = r0; r25 = r20 }
	{ call fn00009610; r1:r0 = combine(r20,r24) }
	{ r16 = sfmpy(r16,r0); immext(#0x38D1B700); r3 = #0x38D1B717 }
	{ r1:r0 = combine(r21,r3); immext(#0x4F000000); r2 = #0x4F000000; r21 = r3 }
	{ r2 = sfmpy(r16,r2); call fn00009600; memb(r29+17) = r2.new }
	{ r1:r0 = combine(r0,r25); immext(#0x0); r20 = #0x0 }
	{ r2 = sfsub(r20,r17) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ call fn00009600; r1 = r24; r24 = r0; r0 = r21 }
	{ call fn00009610; r1:r0 = combine(r0,r25) }
	{ r2 = sfsub(r20,r19) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r19 = add(r18,#0xFFFFFFFF); r21 = r0; r2 = memd(r29+32) }
	{ r17 = sub(r18,r2) }

l0000E60C:
	{ r3 = r23; r0 = memd(r29+116) }
	{ p0 = r0; if (p0.new) jump:nt 0000E638; if (p0.new) r0 = add(r17,r3) }

l0000E61C:
	{ p0 = cmp.eq(r14,#0x1); if (p0.new) jump:nt 0000E638; if (p0.new) r0 = add(r19,r3) }

l0000E624:
	{ r0 = #0x0; r1 = memd(r29+108) }
	{ p0 = r1; if (!p0.new) jump:nt 0000E648; if (!p0) r1:r0 = combine(r18,r3) }

l0000E634:
	{ jump 0000E63C }

l0000E638:
	{ r1 = r18 }

l0000E63C:
	{ call fn00009760; r20 = r3 }
	{ r3 = r20 }

l0000E648:
	{ r23 = add(r3,#0x1); r2 = memw(r29+128); if (!cmp.eq(r2.new,r0)) jump:t 0000E60C }

l0000E658:
	{ r3 = memd(r29+120); memw(r29+64) = r3 }
	{ r23 = convert_uw2sf(r21):chop; r21 = convert_uw2sf(r24):chop; immext(#0xCF000000); r2 = #0xCF000000 }
	{ r2 = sfmpy(r16,r2); r17 = sub(r20,r3); r19 = memd(r29+44); memw(r29+112) = r20 }
	{ memw(r29+60) = r2 }

l0000E67C:
	{ r0 = memw(r29+116) }
	{ p0 = r0; if (p0.new) jump:nt 0000E6AC; if (!p0.new) r1:r0 = combine(r20,r19) }

l0000E68C:
	{ p0 = cmp.eq(r14,#0x1); if (p0.new) jump:nt 0000E6A4 }

l0000E690:
	{ r0 = #0x0; r1 = memd(r29+108) }
	{ p0 = r1; if (!p0.new) jump:nt 0000E6B8; if (!p0) r1:r0 = combine(r20,r19) }

l0000E6A0:
	{ jump 0000E6B4 }

l0000E6A4:
	{ r0 += add(r20,#0xFFFFFFFF); jump 0000E6B4 }

l0000E6AC:
	{ r0 = add(r17,r19); r1 = r20 }

l0000E6B4:
	{ call fn00009760 }

l0000E6B8:
	{ r19 = add(r19,#0x1); r2 = memw(r29+92); if (!cmp.eq(r2.new,r0)) jump:t 0000E67C }

l0000E6C8:
	{ r4 = add(PC,#0xA); r16 = memw(r29+88); r24 = memw(r29+16) }
	{ r1 = #0x8B; r22 = add(r19,#0xFFFFFFFF) }
	{ r2 = memw(r16+28); memb(r29+1) = r2.new }
	{ memw(r29) = r16 }
	{ call logmsg_function }
	{ immext(#0x172C0); r4 = add(PC,#0x172FC); r17 = memw(r29+124); r7 = memw(r29+128) }
	{ r1 = #0x8C; r2 = r24; memw(r29+12) = r17 }
	{ r3 = memd(r29+92); memw(r29+8) = r7 }
	{ memw(r29+4) = r3; memw(r29) = r26 }
	{ call logmsg_function }
	{ r1 = #0x8D; r2 = r24; r25 = memw(r29+84); r3 = memw(r29+32) }
	{ immext(#0x172C0); r4 = add(PC,#0x172D9); r27 = memw(r29+100); memw(r29+12) = r25 }
	{ r7 = memd(r29+120); memw(r29+8) = r3 }
	{ memw(r29+4) = r7; memw(r29) = r27 }
	{ call logmsg_function }
	{ immext(#0x172C0); r4 = add(PC,#0x172D1); r1 = #0x8E; memw(r29+4) = r18 }
	{ r2 = r24; memw(r29) = r20 }
	{ call logmsg_function }
	{ immext(#0x172C0); r4 = add(PC,#0x172C9); r1 = #0x8F; r2 = r24 }
	{ call logmsg_function; r16 = r24; r3 = memb(r16+32); memb(r29) = r3.new }
	{ r4 = add(PC,#0x3B); r24 = memw(r29+64); memw(r29+12) = r27 }
	{ r1 = #0x90; memw(r29+8) = r24; memw(r29) = r26 }
	{ r2 = r16; memw(r29+4) = r22 }
	{ call logmsg_function }
	{ r2 = mpyi(r26,r27); if (p0.new) jump:nt 0000E7D8; r12 = r22; p0 = cmp.eq(r17,r25) }

l0000E7BC:
	{ immext(#0x17280); r3 = add(PC,#0x172A6); r1 = #0x92 }
	{ r2 = r16 }

l0000E7CC:
	{ call errlog_function }
	{ jump 0000EAF0; r0 = #0xFFFFFFFF }

l0000E7D8:
	{ r9:r8 = combine(r24,r27); r3 = memw(r29+104); memw(r29+24) = r12 }
	{ r2 = mpyi(r2,r24); r4 = memw(r3+20) }
	{ r2 = mpyi(r2,r22) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r4)) jump:t 0000E810 }

l0000E7FC:
	{ r3 = add(PC,#0x3F); r1 = #0x94; memw(r29+4) = r2 }
	{ r2 = r16; memw(r29) = r4 }
	{ jump 0000E7CC }

l0000E810:
	{ r4 = memw(r29+80) }
	{ r3 = memw(r4); if (cmp.eq(r3.new,#0x1)) jump:t 0000E82C }

l0000E820:
	{ r3 = add(PC,#0x35); jump 0000E7CC; r1 = #0x96 }

l0000E82C:
	{ p0 = cmp.gt(r26,#0x0); r27 = r18; r3 = memw(r4+12); if (cmp.eq(r3.new,#0x1)) jump:t 0000E84C }

l0000E840:
	{ r3 = add(PC,#0x26); jump 0000E7CC; r1 = #0x97 }

l0000E84C:
	{ r3 = memd(r29+104); r7 = memd(r29+72) }
	{ r4 = memw(r29+60) }
	{ memw(r3+4) = r12; memw(r3+12) = r8 }
	{ memw(r3+8) = r9; memw(r3+24) = r2 }
	{ memw(r3) = r26 }
	{ r2 = memw(r7+16); r3 = memd(r29+76) }
	{ memw(r7+12) = #0x1; memw(r7) = #0x1 }
	{ memw(r7+4) = #0x1; memw(r7+8) = #0x1 }
	{ memw(r2) = r4; memw(r7+24) = #0x4 }
	{ r2 = memw(r3+16); memw(r3) = #0x1 }
	{ memw(r3+4) = #0x1; memw(r3+8) = #0x1 }
	{ r4 = memd(r29+68); memw(r3+12) = #0x1 }
	{ if (!p0) jump:nt 0000EAC8; memw(r2) = r4; memw(r3+24) = #0x4 }

l0000E894:
	{ r2 = r20; r3 = memd(r29+92); r6 = memd(r29+120) }
	{ r3 = add(r3,#0xFFFFFFFF); r7 = memw(r29+32); r5 = memw(r29+128) }
	{ r2 = add(r6,mpyi(r2,r3)); r5 = add(r5,#0xFFFFFFFF); r4 = r7; r18 = r7 }
	{ r4 += mpyi(r27,r5); r1 = mpyi(r7,r17); r5 = #0x0; memb(r29+7) = r5.new }
	{ r17 = mpyi(r17,r8); r4 = sub(r4,r9) }
	{ r3 = mpyi(r1,r8); r4 += lsr(r4,#0x1F); memb(r29+27) = r3.new }
	{ r7 = asr(r4,#0x1); r2 = asr(r2,#0x1); memb(r29+12) = r7.new }

l0000E8F0:
	{ if (!p0.new) jump:nt 0000EAAC; p0 = cmp.gt(r12,#0x0) }

l0000E8F8:
	{ r6 = #0x0; r2 = memd(r29+92); r3 = memd(r29+28) }
	{ r2 = mpyi(r3,r2); r7 = memd(r29+24); memw(r29+116) = r6 }
	{ r5 = mpyi(r3,r7); memw(r29+84) = r2 }
	{ memw(r29+40) = r5 }

l0000E914:
	{ if (!p0.new) jump:nt 0000EA98; p0 = cmp.gt(r9,#0x0); memw(r29+44) = r19 }

l0000E920:
	{ r7 = #0x0; r2 = memd(r29+40); r4 = memd(r29+116) }
	{ r2 = add(r4,r2); r3 = memd(r29+36); memw(r29+68) = r7 }
	{ r2 = mpyi(r2,r9); r3 = add(r4,r3) }
	{ memw(r29+88) = r3; memw(r29+60) = r2 }

l0000E93C:
	{ if (!p0.new) jump:nt 0000EA80; p0 = cmp.gt(r8,#0x0) }

l0000E944:
	{ r4 = memd(r29+68); r2 = memd(r29+60) }
	{ r2 = add(r4,r2); r3 = memd(r29+48); r5 = memd(r29+56) }
	{ r2 = mpyi(r2,r8); r19 = add(r4,r3); r4 = #0x0; r3 = memd(r29+52) }
	{ r2 = addasl(r3,r2,#0x2) }

l0000E960:
	{ r25 = r5; r26 = #0x0; r2 = memd(r29+120); memw(r29+72) = r2 }
	{ r16 = #0x0; p0 = cmp.gt(r2,#0x0) }
	{ if (!p0) jump:nt 0000EA60; memw(r29+76) = r4; memw(r29+80) = r5 }

l0000E97C:
	{ r1 = r20; r2 = memd(r29+116) }
	{ call fn00009770; r0 = sub(r2,r26) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000EA4C }

l0000E98C:
	{ r1 = r20; r2 = memd(r29+88) }
	{ call fn00009750; r0 = sub(r2,r26) }
	{ r2 = memw(r29+92); if (!cmp.gt(r2.new,r0)) jump:nt 0000EA4C }

l0000E9A4:
	{ if (p0.new) r24 = #0x0; if (p0.new) r20 = add(r25,#0x0) }
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 0000EA4C; if (p0.new) r2 = memw(r29+84); if (p0.new) r3 = memw(r29-128) }

l0000E9B8:
	{ r2 = add(r0,r2) }
	{ r2 = mpyi(r2,r3); memb(r29+26) = r2.new }

l0000E9C4:
	{ r22 = sub(r19,r24) }
	{ call fn00009770; r1:r0 = combine(r27,r22) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000EA40; if (!p0) r1:r0 = combine(r27,r22) }

l0000E9D8:
	{ call fn00009750 }
	{ r2 = memw(r29+128); if (!cmp.gt(r2.new,r0)) jump:nt 0000EA40 }

l0000E9E8:
	{ if (p0.new) r3 = memw(r29+124) }
	{ p0 = cmp.gt(r3,#0x0); if (!p0.new) jump:nt 0000EA40; if (p0.new) r8 = memw(r29+100) }

l0000E9F4:
	{ r6 = add(r3,#0xFFFFFFFF); p0 = cmp.gtu(r3,#0x1); r2 = memd(r29+104); r4 = memd(r29+96) }
	{ loop0(0000EA24,r6); r2 = add(r0,r2) }
	{ r2 = add(r4,mpyi(r2,r3)); r3 = add(r20,r8); r4 = memb(r20) }
	{ r4 = sub(r4,r23); r5 = memb(r2++#1) }
	{ if (!p0) jump:nt 0000EA3C; r5 = sub(r5,r21) }

l0000EA24:
	{ r16 += mpyi(r4,r5); r3 = add(r3,r8); r6 = memb(r3); r7 = memb(r2++#1) }
	{ r4 = sub(r6,r23); r5 = sub(r7,r21) }

l0000EA3C:
	{ r16 += mpyi(r4,r5) }

l0000EA40:
	{ r20 = add(r20,r17); r24 = add(r24,#0x1); if (!cmp.eq(r24.new,r18)) jump:t 0000E9C4 }

l0000EA4C:
	{ r2 = memd(r29+108); r7 = memd(r29+120) }

l0000EA50:
	{ r25 = add(r25,r2); r26 = add(r26,#0x1); r20 = memw(r29+112); if (!cmp.eq(r26.new,r7)) jump:t 0000E97C }

l0000EA60:
	{ r4 = memd(r29+76); r5 = memd(r29+80) }

l0000EA64:
	{ r4 = add(r4,#0x1); r5 = add(r5,#0x1); r2 = memw(r29+72); r8 = memw(r29+100) }
	{ p0 = cmp.eq(r4,r8); memw(r2++#4) = r16 }
	{ if (!p0) jump:nt 0000E960 }

l0000EA80:
	{ r2 = memw(r29+68); r9 = memw(r29+64) }
	{ r2 = add(r2,#0x1) }
	{ if (!p0.new) jump:nt 0000E93C; p0 = cmp.eq(r9,r2); memw(r29+68) = r2 }

l0000EA98:
	{ r3 = memd(r29+116); r19 = memd(r29+44) }
	{ r2 = add(r3,#0x2); r3 = add(r3,#0x1) }
	{ p0 = cmp.eq(r2,r11); if (!p0.new) jump:nt 0000E914; memw(r29+116) = r3 }

l0000EAAC:
	{ r2 = memw(r29+28); r26 = memw(r29+20) }
	{ r2 = add(r2,#0x1); r12 = memw(r29+24) }
	{ if (!p0.new) jump:nt 0000E8F0; p0 = cmp.eq(r2,r26); memw(r29+28) = r2 }

l0000EAC8:
	{ immext(#0x16FC0); r4 = add(PC,#0x16FEB); r1 = #0xD3; memw(r29+12) = r8 }
	{ r2 = memw(r29+16); memw(r29+8) = r9 }
	{ memw(r29+4) = r12; memw(r29) = r26 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0000EAF0:
	{ r17:r16 = memd(r29+176); r19:r18 = memd(r29+168) }
	{ r21:r20 = memd(r29+160); r23:r22 = memd(r29+152) }
	{ r25:r24 = memd(r29+144); r27:r26 = memd(r29+136) }
	{ dealloc_return }

;; deconv_check_ref: 0000EB04
deconv_check_ref proc
	{ immext(#0x16E40); r4 = add(PC,#0x16E67); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x202; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x7)) jump:t 0000EB40 }

l0000EB2C:
	{ r3 = add(PC,#0x1B); r1 = #0x203; r2 = memw(r17+28) }
	{ jump 0000EB54; memw(r29) = r2 }

l0000EB40:
	{ r1 = #0x204; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0000EB64 }

l0000EB50:
	{ r3 = add(PC,#0x13) }

l0000EB54:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0000EB64:
	{ immext(#0x16E40); r4 = add(PC,#0x16E52); r1 = #0x205; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

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
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000EBA8 }

l0000EB94:
	{ r0 = add(PC,#0x3F); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

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
	{ immext(#0x16D80); r0 = add(PC,#0x16D9F); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0000EBD4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; deconv_f_execute_ref: 0000EBE0
deconv_f_execute_ref proc
	{ r22 = r0; memd(r29-40) = r23:r22; allocframe(#0xB0) }
	{ r2 = memw(r22+4); memd(r29+168) = r17:r16 }
	{ r17 = memb(r22+32) }
	{ r3 = memw(r22+8); memd(r29+160) = r19:r18 }
	{ p0 = cmp.eq(r17,#0x0); r16 = memw(r2); r4 = memw(r2+8) }
	{ r21 = r1; r19 = memw(r2+4); memd(r29+152) = r21:r20 }
	{ memd(r29+128) = r27:r26 }
	{ r1 = p0; r0 = memw(r16+8); memd(r29+136) = r25:r24 }
	{ r24 = memw(r16); r2 = memw(r19+8) }
	{ r7 = memw(r19+4); r20 = memw(r3) }
	{ r26 = memw(r19+12); r18 = memw(r4+8) }
	{ r27 = memw(r4+4); memw(r29+88) = r2 }
	{ r2 = memw(r19); memw(r29+32) = r7 }
	{ r6 = memw(r16+12); r7 = memw(r16+4) }
	{ memw(r29+80) = r4; memw(r29+120) = r0 }
	{ memw(r29+20) = r24; memw(r29+116) = r2 }
	{ memw(r29+100) = r6; memw(r29+92) = r7 }
	{ if (p0) jump:nt 0000EC74; memw(r29+104) = r1 }

l0000EC50:
	{ p0 = cmp.eq(r9,#0x2); if (p0.new) jump:nt 0000EC68 }

l0000EC54:
	{ p0 = cmp.eq(r9,#0x1); if (!p0.new) jump:nt 0000EC80; r23 = #0x0 }

l0000EC5C:
	{ r0 = r18; r2 = memd(r29+120) }
	{ r0 += add(r2,#0xFFFFFFFF); jump 0000EC74 }

l0000EC68:
	{ r2 = memd(r29+120); r3 = memd(r29+32) }
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r18) }

l0000EC74:
	{ call fn00009760; r1 = r18 }
	{ r23 = r0 }

l0000EC80:
	{ p0 = cmp.eq(r17,#0x2) }
	{ r0 = p0; if (p0) jump:nt 0000ECC0; memb(r29+28) = r0.new }

l0000EC94:
	{ if (p0.new) r1 = add(r27,#0x0); if (p0.new) r2 = memw(r29+92) }
	{ r0 = #0x0; r1 = memd(r29+104) }
	{ p0 = r1; if (!p0.new) jump:nt 0000ECD4; if (p0.new) r1 = add(r27,#0x0); if (p0.new) r0 = memw(r29+92) }

l0000ECB0:
	{ jump 0000ECD0 }
0000ECB4             00 C0 61 70 E0 5F 02 E2 0C C0 00 58     ..ap._.....X

l0000ECC0:
	{ r1 = r27; r2 = memd(r29+116); r3 = memd(r29+92) }
	{ r2 = sub(r3,r2) }
	{ r0 = add(r2,r1) }

l0000ECD0:
	{ call fn00009760 }

l0000ECD4:
	{ r25 = r0; r2 = memw(r20+16); r7 = memd(r29+32) }
	{ r20 = sub(r18,r7); r16 = add(r18,#0xFFFFFFFF); r7 = memw(r16+16); memw(r29+84) = r20 }
	{ r2 = memw(r19+16); memw(r29+56) = r2 }
	{ memw(r29+52) = r2; memw(r29+96) = r7 }

l0000ECF0:
	{ r3 = r23; r0 = memd(r29+112) }
	{ p0 = r0; if (p0.new) jump:nt 0000ED1C; if (p0.new) r0 = add(r20,r3) }

l0000ED00:
	{ p0 = cmp.eq(r9,#0x1); if (p0.new) jump:nt 0000ED1C; if (p0.new) r0 = add(r16,r3) }

l0000ED08:
	{ r0 = #0x0; r1 = memd(r29+104) }
	{ p0 = r1; if (!p0.new) jump:nt 0000ED2C; if (!p0) r1:r0 = combine(r18,r3) }

l0000ED18:
	{ jump 0000ED20 }

l0000ED1C:
	{ r1 = r18 }

l0000ED20:
	{ call fn00009760; r19 = r3 }
	{ r3 = r19 }

l0000ED2C:
	{ r23 = add(r3,#0x1); r2 = memw(r29+120); if (!cmp.eq(r2.new,r0)) jump:t 0000ECF0 }

l0000ED3C:
	{ r20 = r25; r23 = r3; r2 = memd(r29+116) }
	{ r16 = sub(r19,r2); memw(r29+108) = r19 }

l0000ED4C:
	{ r0 = memw(r29+112) }
	{ p0 = r0; if (p0.new) jump:nt 0000ED7C; if (!p0.new) r1:r0 = combine(r19,r20) }

l0000ED5C:
	{ p0 = cmp.eq(r9,#0x1); if (p0.new) jump:nt 0000ED74 }

l0000ED60:
	{ r0 = #0x0; r1 = memd(r29+104) }
	{ p0 = r1; if (!p0.new) jump:nt 0000ED88; if (!p0) r1:r0 = combine(r19,r20) }

l0000ED70:
	{ jump 0000ED84 }

l0000ED74:
	{ r0 += add(r19,#0xFFFFFFFF); jump 0000ED84 }

l0000ED7C:
	{ r0 = add(r16,r20); r1 = r19 }

l0000ED84:
	{ call fn00009760 }

l0000ED88:
	{ r20 = add(r20,#0x1); r2 = memw(r29+92); if (!cmp.eq(r2.new,r0)) jump:t 0000ED4C }

l0000ED98:
	{ r4 = add(PC,#0x7); r2 = memw(r22+28) }
	{ r1 = #0x76; memw(r29+4) = r2 }
	{ r2 = r21; r17 = r20 }
	{ call logmsg_function; memw(r29+24) = r17; memw(r29) = r22 }
	{ r1 = #0x77; r25 = memw(r29+100); r7 = memw(r29+120) }
	{ immext(#0x16D80); r4 = add(PC,#0x16DB9); memw(r29+12) = r25; memw(r29) = r24 }
	{ r2 = r21; r3 = memd(r29+92); memw(r29+8) = r7 }
	{ memw(r29+4) = r3 }
	{ call logmsg_function }
	{ r1 = #0x78; r27 = memw(r29+88); r7 = memw(r29+32) }
	{ immext(#0x16D80); r4 = add(PC,#0x16DA6); memw(r29+12) = r27; memw(r29) = r26 }
	{ r2 = r21; r3 = memd(r29+116); memw(r29+8) = r7 }
	{ memw(r29+4) = r3 }
	{ call logmsg_function }
	{ immext(#0x16D80); r4 = add(PC,#0x16D9E); r1 = #0x79; memw(r29+4) = r18 }
	{ r2 = r21; memw(r29) = r19 }
	{ call logmsg_function }
	{ immext(#0x16D80); r4 = add(PC,#0x16D9A); r1 = #0x7A; r2 = r21 }
	{ call logmsg_function; r16 = r25; r3 = memb(r22+32); memb(r29) = r3.new }
	{ r4 = add(PC,#0xC); r1 = #0x7B; memw(r29+12) = r26 }
	{ r2 = r21; memw(r29) = r24 }
	{ memw(r29+8) = r23; memw(r29+4) = r17 }
	{ call logmsg_function }
	{ r2 = mpyi(r24,r26); if (p0.new) jump:nt 0000EE8C; p0 = cmp.eq(r16,r27); if (!p0.new) r1 = #0x7D }

l0000EE74:
	{ immext(#0x16D40); r3 = add(PC,#0x16D7B) }
	{ r2 = r21 }

l0000EE80:
	{ call errlog_function }
	{ jump 0000F148; r0 = #0xFFFFFFFF }

l0000EE8C:
	{ r7 = r23; r3 = memw(r29+84); memb(r29+16) = r7.new }
	{ r4 = memw(r3+20) }
	{ r2 = mpyi(r2,r17) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r4)) jump:t 0000EEC4 }

l0000EEB0:
	{ r3 = add(PC,#0x18); r1 = #0x7F; memw(r29+4) = r2 }
	{ r2 = r21; memw(r29) = r4 }
	{ jump 0000EE80 }

l0000EEC4:
	{ r4 = memw(r29+80) }
	{ r3 = memw(r4); if (cmp.eq(r3.new,#0x1)) jump:t 0000EEE0 }

l0000EED4:
	{ r3 = add(PC,#0xE); jump 0000EE80; r1 = #0x81 }

l0000EEE0:
	{ p0 = cmp.gt(r24,#0x0); r3 = memw(r4+12); if (cmp.eq(r3.new,#0x1)) jump:t 0000EEFC }

l0000EEF0:
	{ r3 = add(PC,#0x3); jump 0000EE80; r1 = #0x82 }

l0000EEFC:
	{ r3 = memd(r29+84); memw(r29+16) = r21 }
	{ memw(r3+24) = r2; memw(r3+4) = r17 }
	{ memw(r3) = r24; memw(r3+8) = r7 }
	{ if (!p0) jump:nt 0000F124; memw(r3+12) = r26 }

l0000EF14:
	{ r23 = mpyi(r16,r26); r2 = r19; r3 = memd(r29+92); r6 = memd(r29+116) }
	{ r3 = add(r3,#0xFFFFFFFF); r27 = memw(r29+32); r5 = memw(r29+120) }
	{ r2 = add(r6,mpyi(r2,r3)); r1 = mpyi(r27,r16); r4 = r27; r5 = add(r5,#0xFFFFFFFF) }
	{ r4 += mpyi(r18,r5); r2 = add(r2,sub(#0x41,r20)); r5 = #0x0; memb(r29+7) = r5.new }
	{ r4 = sub(r4,r7) }
	{ r3 = mpyi(r1,r26); r4 += lsr(r4,#0x1F); memb(r29+26) = r3.new }
	{ r2 = asr(r2,#0x1); memb(r29+12) = r6.new }

l0000EF70:
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:nt 0000F10C }

l0000EF74:
	{ r5 = #0x0; r2 = memd(r29+92); r3 = memd(r29+28) }
	{ r2 = mpyi(r3,r2); r6 = memd(r29+24); memw(r29+112) = r5 }
	{ r4 = mpyi(r3,r6); memw(r29+84) = r2 }
	{ memw(r29+40) = r4 }

l0000EF90:
	{ p0 = cmp.gt(r7,#0x0); if (!p0.new) jump:nt 0000F0F8; memw(r29+44) = r20 }

l0000EF98:
	{ r6 = #0x0; r2 = memd(r29+40); r4 = memd(r29+112) }
	{ r2 = add(r4,r2); r3 = memd(r29+36); memw(r29+68) = r6 }
	{ r2 = mpyi(r2,r7); r3 = add(r4,r3) }
	{ memw(r29+88) = r3; memw(r29+60) = r2 }

l0000EFB4:
	{ if (!p0.new) jump:nt 0000F0E8; p0 = cmp.gt(r26,#0x0) }

l0000EFBC:
	{ r4 = memd(r29+68); r2 = memd(r29+60) }
	{ r2 = add(r4,r2); r3 = memd(r29+48); r5 = memd(r29+52) }
	{ r2 = mpyi(r2,r26); r20 = add(r4,r3); r4 = #0x0; r3 = memd(r29+56) }
	{ r2 = addasl(r3,r2,#0x2) }

l0000EFD8:
	{ immext(#0x0); r24 = #0x0; r2 = memd(r29+116); memw(r29+72) = r2 }
	{ p0 = cmp.gt(r2,#0x0); r21 = #0x0; r17 = r5 }
	{ if (!p0) jump:nt 0000F0CC; memw(r29+76) = r4; memw(r29+80) = r5 }

l0000EFF4:
	{ r1 = r19; r2 = memd(r29+112) }
	{ call fn00009770; r0 = sub(r2,r21) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000F0B8 }

l0000F004:
	{ r1 = r19; r2 = memd(r29+88) }
	{ call fn00009750; r0 = sub(r2,r21) }
	{ r2 = memw(r29+92); if (!cmp.gt(r2.new,r0)) jump:nt 0000F0B8 }

l0000F01C:
	{ if (p0.new) r25 = #0x0 }
	{ if (!p0.new) jump:nt 0000F0B8; p0 = cmp.gt(r27,#0x0) }

l0000F028:
	{ r19 = r17; r2 = memd(r29+84); r3 = memd(r29+120) }
	{ r2 = add(r0,r2) }
	{ r2 = mpyi(r2,r3); memb(r29+25) = r2.new }

l0000F03C:
	{ r22 = sub(r20,r25) }
	{ call fn00009770; r1:r0 = combine(r18,r22) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0000F0AC; if (!p0) r1:r0 = combine(r18,r22) }

l0000F050:
	{ call fn00009750 }
	{ r2 = memw(r29+120); if (!cmp.gt(r2.new,r0)) jump:nt 0000F0AC }

l0000F060:
	{ r4 = addasl(r19,r26,#0x2); p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt 0000F0AC }

l0000F068:
	{ r6 = add(r16,#0xFFFFFFFF); p0 = cmp.gtu(r16,#0x1); r2 = memd(r29+100); r3 = memd(r29+96) }
	{ loop0(0000F094,r6); r2 = add(r0,r2) }
	{ r2 = mpyi(r2,r16) }
	{ r2 = addasl(r3,r2,#0x2); r3 = memw(r19) }
	{ r5 = add(r2,#0x4); r2 = memw(r2) }
	{ if (!p0) jump:nt 0000F0A8 }

l0000F094:
	{ r24 += sfmpy(r2,r3); r4 = addasl(r4,r26,#0x2); r2 = memw(r5); r3 = memw(r4) }
	{ r5 = add(r5,#0x4); nop }

l0000F0A8:
	{ r24 += sfmpy(r2,r3) }

l0000F0AC:
	{ r19 = addasl(r19,r23,#0x2); r25 = add(r25,#0x1); if (!cmp.eq(r25.new,r27)) jump:t 0000F03C }

l0000F0B8:
	{ r2 = memd(r29+104); r7 = memd(r29+116) }

l0000F0BC:
	{ r19 = memw(r29+108) }
	{ r17 = addasl(r17,r2,#0x2); r21 = add(r21,#0x1); if (!cmp.eq(r21.new,r7)) jump:t 0000EFF4 }

l0000F0CC:
	{ r2 = memd(r29+72); r4 = memd(r29+76) }

l0000F0D0:
	{ r4 = r4; r5 = memd(r29+80) }
	{ r5 = add(r5,#0x4); r2 = add(r2,#0x4); p0 = cmp.eq(r4,r26); memw(r2) = r24 }
	{ if (!p0) jump:nt 0000EFD8 }

l0000F0E8:
	{ r2 = memd(r29+68); r7 = memd(r29+64) }
	{ r2 = add(r2,#0x1) }
	{ p0 = cmp.eq(r7,r2); if (!p0.new) jump:nt 0000EFB4; memw(r29+68) = r2 }

l0000F0F8:
	{ r3 = memd(r29+112); r20 = memd(r29+44) }
	{ r2 = add(r3,#0x2); r3 = add(r3,#0x1) }
	{ p0 = cmp.eq(r2,r12); if (!p0.new) jump:nt 0000EF90; memw(r29+112) = r3 }

l0000F10C:
	{ r2 = memw(r29+28); r24 = memw(r29+20) }
	{ r2 = r2; r17 = memd(r29+24) }
	{ if (!p0.new) jump:nt 0000EF70; p0 = cmp.eq(r2,r24); memw(r29+28) = r2 }

l0000F124:
	{ immext(#0x16B00); r4 = add(PC,#0x16B1C); r1 = #0xB3; memw(r29+12) = r26 }
	{ r2 = memw(r29+16); memw(r29) = r24 }
	{ memw(r29+8) = r7; memw(r29+4) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0000F148:
	{ r17:r16 = memd(r29+168); r19:r18 = memd(r29+160) }
	{ r21:r20 = memd(r29+152); r23:r22 = memd(r29+144) }
	{ r25:r24 = memd(r29+136); r27:r26 = memd(r29+128) }
	{ dealloc_return }
0000F15C                                     00 C0 00 7F             ....

;; deconv_check_ref: 0000F160
deconv_check_ref proc
	{ immext(#0x16980); r4 = add(PC,#0x16998); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xBA; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0000F19C }

l0000F188:
	{ r3 = add(PC,#0xC); r1 = #0xBB; r2 = memw(r17+28) }
	{ jump 0000F1B0; memw(r29) = r2 }

l0000F19C:
	{ r1 = #0xBC; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0000F1C0 }

l0000F1AC:
	{ r3 = add(PC,#0x4) }

l0000F1B0:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0000F1C0:
	{ immext(#0x16980); r4 = add(PC,#0x16983); r1 = #0xBD; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

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
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000F204 }

l0000F1F0:
	{ r0 = add(PC,#0x2E); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0000F204:
	{ dealloc_return }

;; errlog_function: 0000F208
;;   Called from:
;;     0000EE80 (in deconv_f_execute_ref)
;;     0000F1B0 (in deconv_check_ref)
;;     0000F1F8 (in logmsg_function)
errlog_function proc
	{ immext(#0x168C0); r0 = add(PC,#0x168D2); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0000F22C                                     00 00 00 00             ....

;; logsoftmax_execute: 0000F230
logsoftmax_execute proc
	{ allocframe(#0x48) }
	{ r2 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+56) = r19:r18; memd(r29+64) = r17:r16 }
	{ r2 = r1; r5 = memw(r2); r19 = memw(r3) }
	{ memd(r29+48) = r21:r20; memd(r29+40) = r23:r22 }
	{ r7 = memw(r5+20); r4 = memw(r19+24) }
	{ p0 = cmp.gtu(r4,r7); if (!p0.new) r26 = #0x0; memd(r29+32) = r25:r24; memd(r29+24) = r27:r26 }
	{ if (p0) jump:nt 0000F3AC; if (p0) r1 = #0x39 }

l0000F264:
	{ r4 = memw(r19); r2 = memw(r19+4) }
	{ r2 = mpyi(r2,r4); r3 = memw(r19+8); memw(r29+8) = r2 }
	{ r22 = memw(r19+12) }
	{ memw(r29+12) = r5; memw(r29) = r4 }
	{ memw(r29+4) = r3 }
	{ r24 = mpyi(r2,r3); r25 = memw(r19+16); if (!cmp.gt(r24.new,#0x0)) jump:nt 0000F37C }

l0000F28C:
	{ r16 = #0x0; r2 = memd(r29+12) }
	{ r27 = memw(r2+16) }

l0000F294:
	{ p0 = cmp.gt(r22,#0x0); r2 = memw(r25) }
	{ r0 = p0; if (!p0) jump:nt 0000F2FC; memb(r29+4) = r0.new }

l0000F2AC:
	{ r3 = r20; r4 = r2; r21 = r2 }
	{ r21 = sfmax(r4,r21); r5 = add(r3,#0x4) }
	{ jump 0000F2C8 }
0000F2C0 04 C0 83 91 F8 C3 35 17                         ......5.        

l0000F2C8:
	{ r23 = r22; r17 = r16; r18 = r20; r0 = memd(r29+16) }
	{ p0 = r0; if (!p0.new) jump:nt 0000F2FC }

l0000F2DC:
	{ r0 = sfsub(r2,r21); call fn00009780 }
	{ r17 = sfadd(r17,r0); r3 = add(r18,#0x4); r23 = add(r23,#0xFFFFFFFF); if (cmp.eq(r23.new,#0x0)) jump:nt 0000F308 }

l0000F2F8:
	{ r10 = r3; jump 0000F2DC }

l0000F2FC:
	{ call fn00009790; r0 = r16 }
	{ jump 0000F368 }

l0000F308:
	{ call fn00009790; r0 = r17 }
	{ r2 = memw(r29+16) }
	{ p0 = r2; if (!p0.new) jump:nt 0000F368 }

l0000F31C:
	{ p0 = cmp.gtu(r22,#0x1); r3 = #0x0; r5 = r22 }
	{ loop0(0000F340,r5); r4 = add(r25,r3); r2 = add(r3,#0x4); r3 = add(r27,r3) }
	{ r4 = memw(r4) }
	{ r4 = sfsub(r4,r21); if (!p0) jump:nt 0000F360 }

l0000F340:
	{ r4 = sfsub(r4,r0); r5 = add(r25,r2); r6 = add(r2,#0x4); memb(r3) = r4.new }
	{ r2 = r6; r7 = memw(r5) }
	{ r4 = sfsub(r7,r21); nop }

l0000F360:
	{ r2 = sfsub(r4,r0); memb(r3) = r2.new }

l0000F368:
	{ r27 = addasl(r27,r22,#0x2); r20 = addasl(r20,r22,#0x2) }

l0000F36C:
	{ r20 = addasl(r20,r22,#0x2) }
	{ r25 = addasl(r25,r22,#0x2); r26 = add(r26,#0x1); if (!cmp.eq(r26.new,r24)) jump:t 0000F294 }

l0000F37C:
	{ r0 = #0x0; r3 = memd(r29+12); r2 = memd(r29) }

l0000F380:
	{ r3 = memd(r29+12); r2 = memd(r29) }

l0000F384:
	{ r7 = memd(r29+4); r6 = memd(r29+8) }
	{ memw(r3+8) = r7; memw(r3) = r2 }
	{ memw(r3+12) = r22; memw(r3+4) = r6 }
	{ r2 = memw(r19+24) }
	{ memw(r3+24) = r2 }

l0000F398:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }

l0000F3AC:
	{ immext(#0x16900); r3 = add(PC,#0x16924); call errlog_function }
	{ jump 0000F398; r0 = #0xFFFFFFFF }

;; logsoftmax_check: 0000F3C0
logsoftmax_check proc
	{ immext(#0x168C0); r4 = add(PC,#0x168C9); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x53; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x54; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x1)) jump:t 0000F400 }

l0000F3EC:
	{ r3 = add(PC,#0x3D) }
	{ call errlog_function; r2 = r16 }

l0000F3F4:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0000F400:
	{ r1 = #0x55; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0000F418 }

l0000F410:
	{ r3 = add(PC,#0x19); jump 0000F3F4 }

l0000F418:
	{ immext(#0x16880); r4 = add(PC,#0x1689C); r1 = #0x56; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0000F438
;;   Called from:
;;     0000F3D4 (in logsoftmax_check)
;;     0000F428 (in logsoftmax_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000F45C }

l0000F448:
	{ r0 = add(PC,#0x25); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0000F45C:
	{ dealloc_return }

;; errlog_function: 0000F460
;;   Called from:
;;     0000F3AC (in logsoftmax_execute)
;;     0000F3F0 (in logsoftmax_check)
;;     0000F450 (in logmsg_function)
errlog_function proc
	{ immext(#0x16800); r0 = add(PC,#0x16809); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0000F484             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; expanddims_execute: 0000F490
expanddims_execute proc
	{ allocframe(#0x0) }
	{ r4 = memw(r0+8); r2 = memw(r0+4) }
	{ r3 = memw(r2); r4 = memw(r4) }
	{ r7 = memw(r3); r5 = memw(r3+4) }
	{ memw(r4+4) = r5; memw(r4) = r7 }
	{ r2 = memw(r3+8); r0 = memw(r3+12) }
	{ memw(r4+12) = r0 }
	{ r2 = r1; memw(r4+8) = r2 }
	{ r5 = memw(r3+24) }
	{ r6 = memw(r4+20); if (cmp.gtu(r5,r6.new)) jump:t 0000F4CC }

l0000F4C0:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }
	{ r0 = #0x0; dealloc_return }

l0000F4CC:
	{ immext(#0x16880); r3 = add(PC,#0x1688B); call errlog_function; r1 = #0x30 }
	{ r0 = #-0x1; dealloc_return }

;; expanddims_check: 0000F4E0
expanddims_check proc
	{ immext(#0x16800); r4 = add(PC,#0x1681E); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x37; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16) }
	{ r3 = #0x4; if (cmp.gtu(r3.new,r2)) jump:t 0000F524 }

l0000F50C:
	{ r3 = add(PC,#0x13); r1 = #0x38 }
	{ call errlog_function; r2 = r16 }

l0000F518:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0000F524:
	{ r1 = #0x39; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0000F53C }

l0000F534:
	{ r3 = add(PC,#0x3A); jump 0000F518 }

l0000F53C:
	{ immext(#0x167C0); r4 = add(PC,#0x167FE); r1 = #0x3A; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0000F558
;;   Called from:
;;     0000F4F4 (in expanddims_check)
;;     0000F548 (in expanddims_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000F57C }

l0000F568:
	{ r0 = add(PC,#0x3A); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0000F57C:
	{ dealloc_return }

;; errlog_function: 0000F580
;;   Called from:
;;     0000F4CC (in expanddims_execute)
;;     0000F514 (in expanddims_check)
;;     0000F570 (in logmsg_function)
errlog_function proc
	{ immext(#0x16740); r0 = add(PC,#0x1675E); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0000F5A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; sslice_execute_4b: 0000F5B0
sslice_execute_4b proc
	{ r4 = #0x4; jump strided_slice_impl }

;; sslice_check: 0000F5B4
sslice_check proc
	{ immext(#0x167C0); r4 = add(PC,#0x167D3); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x70; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x71; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x4)) jump:t 0000F5E8 }

l0000F5E0:
	{ r3 = add(PC,#0x2); jump 0000F604 }

l0000F5E8:
	{ r1 = #0x72; r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0000F60C }

l0000F5FC:
	{ r3 = add(PC,#0x31) }
	{ call errlog_function; r2 = r16 }

l0000F604:
	{ r2 = r16 }

l0000F608:
	{ r0 = #0xFFFFFFFF }

l0000F60C:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; sslice_execute_1b: 0000F610
sslice_execute_1b proc
	{ r1 = #0x1; jump strided_slice_impl }

;; sslice_execute_q8: 0000F614
sslice_execute_q8 proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = memw(r16+4); r4 = memw(r16+8) }
	{ r3 = memw(r2+16); r2 = memw(r4+4) }
	{ r7 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r7 }
	{ r4 = memw(r3+8); r0 = memw(r3+12) }
	{ memw(r2+12) = r0; memw(r2+8) = r4 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 0000F64C }

l0000F644:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l0000F64C:
	{ r2 = memw(r16+8); r3 = memw(r16+4) }
	{ r3 = memw(r3+20); r2 = memw(r2+8) }
	{ r4 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 0000F67C }

l0000F674:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l0000F67C:
	{ r1:r0 = combine(r17,r16); r2 = #0x1; r17:r16 = memd(r29); deallocframe }
	{ jump strided_slice_impl }
0000F68C                                     00 C0 00 7F             ....

;; sslice_check_q8: 0000F690
sslice_check_q8 proc
	{ immext(#0x166C0); r4 = add(PC,#0x166F7); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x78; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x79; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x6)) jump:t 0000F6C4 }

l0000F6BC:
	{ r3 = add(PC,#0x26); jump 0000F6E0 }

l0000F6C4:
	{ r1 = #0x7A; r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0000F6E8 }

l0000F6D8:
	{ r3 = add(PC,#0x15) }
	{ call errlog_function; r2 = r16 }

l0000F6E0:
	{ r2 = r16 }

l0000F6E4:
	{ r0 = #0xFFFFFFFF }

l0000F6E8:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0000F6EC
;;   Called from:
;;     0000F5C8 (in sslice_check)
;;     0000F6A4 (in sslice_check_q8)
;;     0000F840 (in strided_slice_impl)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000F710 }

l0000F6FC:
	{ r0 = add(PC,#0x2D); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0000F710:
	{ dealloc_return }

;; errlog_function: 0000F714
;;   Called from:
;;     0000F600 (in sslice_check)
;;     0000F6DC (in sslice_check_q8)
;;     0000F704 (in logmsg_function)
;;     0000F810 (in strided_slice_impl)
errlog_function proc
	{ immext(#0x16640); r0 = add(PC,#0x16651); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; strided_slice_impl: 0000F738
;;   Called from:
;;     0000F5B0 (in sslice_execute_4b)
;;     0000F610 (in sslice_execute_1b)
;;     0000F688 (in sslice_execute_q8)
strided_slice_impl proc
	{ r19 = r0; memd(r29-24) = r19:r18; allocframe(#0x48) }
	{ r17 = r2; r3 = memw(r19+4); memd(r29+64) = r17:r16 }
	{ r21 = r1; memd(r29+48) = r21:r20 }
	{ memd(r29+32) = r25:r24 }
	{ r5 = memw(r3+8); memd(r29+40) = r23:r22 }
	{ r6 = memw(r3+4); r7 = memw(r3+12) }
	{ r22 = memw(r3); r0 = memw(r5+16) }
	{ r5 = memw(r6+16); r6 = memw(r7+16) }
	{ r4 = memw(r19+8) }
	{ r16 = memw(r5); r24 = memw(r0) }
	{ r2 = sub(#0xFFFFFFFF,r16); r20 = memw(r6); memd(r29+24) = r27:r26 }
	{ r1:r0 = combine(r20,r20); r3 = memw(r22+12); r23 = memw(r4) }
	{ r0 += add(r24,r2); r26 = memw(r22+4); r27 = memw(r22+8) }
	{ call fn00009750; memw(r29+20) = r3 }
	{ immext(#0x16600); r3 = add(PC,#0x16621); r18 = r0; r2 = memw(r23+20) }
	{ r25 = mpyi(r18,r17); r1 = #0x47; if (cmp.gtu(r25.new,r2)) jump:t 0000F810 }

l0000F7B0:
	{ r3 = add(PC,#0x17); p0 = cmp.gt(r26,#0x1); r1 = #0x48 }
	{ r2 = memw(r22); if (cmp.gt(r2.new,#0x1)) jump:t 0000F810 }

l0000F7C8:
	{ r3 = add(PC,#0x3F); r1 = #0x49 }
	{ if (p0) jump:nt 0000F810; if (!p0) r1 = #0x4A }

l0000F7D8:
	{ immext(#0x165C0); r3 = add(PC,#0x165EB); p0 = cmp.gt(r27,#0x1); if (!p0.new) r1 = #0x4B }
	{ if (p0) jump:nt 0000F810 }

l0000F7EC:
	{ immext(#0x165C0); r3 = add(PC,#0x165F7) }
	{ r2 = memw(r29+20); if (!cmp.gt(r2.new,r16)) jump:t 0000F810 }

l0000F800:
	{ r3 = add(PC,#0x31); r1 = #0x4C }
	{ r2 = memw(r29+20); if (!cmp.gtu(r24,r2.new)) jump:t 0000F81C }

l0000F810:
	{ call errlog_function; r2 = r21; r19 = #-0x1 }

l0000F814:
	{ r2 = r21; r19 = #-0x1 }

l0000F818:
	{ jump 0000F884 }

l0000F81C:
	{ immext(#0x165C0); r4 = add(PC,#0x165DA); r26 = memw(r22+16); r22 = memw(r23+16) }
	{ r1 = #0x4F; r2 = r21; memw(r29+12) = r20; memw(r29+8) = r24 }
	{ memw(r29+4) = r16 }
	{ call logmsg_function; r19 = #0x0; memw(r29) = r19 }
	{ p0 = cmp.gt(r18,#0x0); memw(r23+24) = r25; memw(r23) = #0x1 }
	{ memw(r23+4) = #0xFFFFFF81 }
	{ if (!p0) jump:nt 0000F884; memw(r23+8) = #0x1; memw(r23+12) = r18 }

l0000F864:
	{ r16 = add(r26,mpyi(r16,r17)); r20 = mpyi(r20,r17) }

l0000F86C:
	{ call fn00009560; r1:r0 = combine(r16,r22); r2 = r17; r22 = add(r22,r17) }
	{ r16 = add(r16,r20); r18 = add(r18,#0xFFFFFFFF); if (!cmp.eq(r18.new,#0x0)) jump:t 0000F86C }

l0000F884:
	{ r0 = r19; r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l0000F888:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }
0000F89C                                     00 00 00 00             ....

;; resizenear_f_execute: 0000F8A0
resizenear_f_execute proc
	{ allocframe(#0x68) }
	{ r7 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+80) = r21:r20; memd(r29+64) = r25:r24 }
	{ r4 = memw(r7+4); r21 = memw(r7) }
	{ r27 = r1; memd(r29+56) = r27:r26; memd(r29+88) = r19:r18 }
	{ r2 = memw(r4+16); r25 = memw(r21+8) }
	{ memd(r29+72) = r23:r22; memd(r29+96) = r17:r16 }
	{ r0 = convert_w2sf(r25); r24 = memw(r2+4); r20 = memw(r3) }
	{ r18 = memw(r21); r26 = memw(r21+4) }
	{ r1 = convert_w2sf(r24); call fn00009610; r23 = memw(r21+12); r19 = memw(r2) }
	{ r2 = convert_w2sf(r26); r16 = r0 }
	{ r1 = convert_w2sf(r19); call fn00009610; r0 = r2 }
	{ r2 = mpyi(r24,r19); r17 = asl(r23,#0x2); r1 = #0x44; r3 = memw(r20+20) }
	{ r2 = mpyi(r2,r18) }
	{ r22 = mpyi(r2,r17); if (!cmp.gtu(r22.new,r3)) jump:t 0000F934 }

l0000F920:
	{ r3 = add(PC,#0x22); r2 = r27 }
	{ call errlog_function }
	{ jump 0000FA44; r0 = #0xFFFFFFFF }

l0000F934:
	{ r3 = r26; r2 = r27; r26 = memw(r21+16); r21 = memw(r20+16) }
	{ immext(#0x16540); r4 = add(PC,#0x16548); r1 = #0x45; memw(r29+28) = r23 }
	{ memw(r29+48) = r0; memw(r29+24) = r24 }
	{ memw(r29+36) = r18; memw(r29+20) = r19 }
	{ memw(r29+32) = r3; memw(r29+16) = r18 }
	{ memw(r29+12) = r23; memw(r29+4) = r3 }
	{ memw(r29+8) = r25; memw(r29) = r18 }
	{ call logmsg_function }
	{ r0 = #0x0; r3 = r19; memw(r20+24) = r22; memw(r20) = r18 }
	{ p0 = cmp.gt(r18,#0x0); memw(r20+4) = r19; memw(r20+12) = r23 }
	{ memw(r20+8) = r24 }
	{ if (!p0) jump:nt 0000FA44 }

l0000F990:
	{ r2 = mpyi(r24,r23); r5 = #0x0; r4 = memd(r29+48) }
	{ r2 = asl(r2,#0x2) }
	{ memw(r29+44) = r2 }

l0000F9A0:
	{ p0 = cmp.gt(r3,#0x0); if (p0.new) jump:nt 0000F9AC; jump 0000FA34; if (p0.new) memw(r29+40) = r5 }

l0000F9AC:
	{ r20 = #0x0; r2 = memd(r29+32); memw(r29+40) = r5 }
	{ r2 = mpyi(r5,r2) }
	{ memw(r29+52) = r2 }
	{ r2 = convert_w2sf(r20); r27 = r21; p0 = cmp.gt(r24,#0x0) }
	{ r2 = sfmpy(r4,r2) }
	{ r2 = convert_sf2uw(r2); if (!p0) jump:nt 0000FA2C }
	{ r19 = r21; r22 = r3; r3 = memd(r29+52) }
	{ r3 = r25; r21 = #0x0; r2 = add(r2,r3) }
	{ r25 = mpyi(r2,r3); r18 = r3 }
	{ r2 = convert_w2sf(r21); r0 = r19 }
	{ r2 = sfmpy(r16,r2) }
	{ r3 = convert_sf2uw(r2); r2 = r17 }
	{ r3 = add(r3,r25) }
	{ r3 = mpyi(r3,r23) }
	{ r1 = addasl(r26,r3,#0x2); call fn00009560; r19 = add(r19,r17) }
	{ r21 = add(r21,#0x1); if (!cmp.eq(r21.new,r24)) jump:t 0000F9EC }
	{ r25 = r18; r3 = r22; r2 = memd(r29+44) }
	{ r21 = add(r21,r2); r4 = memd(r29+48) }
	{ r20 = add(r20,#0x1); if (!cmp.eq(r20.new,r3)) jump:t 0000F9BC }

l0000FA34:
	{ r5 = memd(r29+40); r2 = memd(r29+36) }
	{ r5 = add(r5,#0x1); if (!cmp.eq(r5.new,r2)) jump:t 0000F9A0 }

l0000FA44:
	{ r17:r16 = memd(r29+96); r19:r18 = memd(r29+88) }
	{ r21:r20 = memd(r29+80); r23:r22 = memd(r29+72) }
	{ r25:r24 = memd(r29+64); r27:r26 = memd(r29+56) }
	{ dealloc_return }
0000FA58                         00 40 00 7F 00 C0 00 7F         .@......

;; resizenear_f_check: 0000FA60
resizenear_f_check proc
	{ immext(#0x163C0); r4 = add(PC,#0x163D5); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x5B; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x5C; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0000FAA0 }

l0000FA8C:
	{ r3 = add(PC,#0x0) }
	{ call errlog_function; r2 = r16 }

l0000FA94:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0000FAA0:
	{ r1 = #0x5D; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0000FAB8 }

l0000FAB0:
	{ r3 = add(PC,#0x2B); jump 0000FA94 }

l0000FAB8:
	{ immext(#0x16380); r4 = add(PC,#0x163AF); r1 = #0x5E; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0000FAD8
;;   Called from:
;;     0000F970 (in resizenear_f_execute)
;;     0000FA74 (in resizenear_f_check)
;;     0000FAC8 (in resizenear_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0000FAFC }

l0000FAE8:
	{ r0 = add(PC,#0x31); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0000FAFC:
	{ dealloc_return }

;; errlog_function: 0000FB00
;;   Called from:
;;     0000F928 (in resizenear_f_execute)
;;     0000FA90 (in resizenear_f_check)
;;     0000FAF0 (in logmsg_function)
errlog_function proc
	{ immext(#0x16300); r0 = add(PC,#0x16315); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0000FB24             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; mirrorpad_f_execute: 0000FB30
mirrorpad_f_execute proc
	{ immext(#0x163C0); r4 = add(PC,#0x163F2); allocframe(#0xC0) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+152) = r25:r24; memd(r29+176) = r19:r18 }
	{ r24 = memw(r2+4); r2 = memw(r2) }
	{ r18 = memw(r3); memd(r29+144) = r27:r26 }
	{ r6 = memw(r24+16); r7 = memw(r2+16) }
	{ r5 = memw(r2); r25 = memw(r2+12) }
	{ r19 = memw(r2+8); r27 = memw(r2+4) }
	{ r17 = r1; r1 = #0x85; r2 = memw(r6+20); memd(r29+184) = r17:r16 }
	{ memd(r29+168) = r21:r20; memd(r29+160) = r23:r22 }
	{ r7 = memw(r18+16); memw(r29+124) = r7 }
	{ r2 = memw(r6+16); memw(r29+136) = r2 }
	{ r20 = memw(r6+24); r21 = memw(r6+28) }
	{ r16 = memw(r6+12); r26 = memw(r6+8) }
	{ r22 = memw(r6+4); r23 = memw(r6) }
	{ r2 = r17; memw(r29+12) = r25; memw(r29+140) = r2 }
	{ memw(r29+8) = r19; memw(r29+120) = r0 }
	{ memw(r29+4) = r27; memw(r29+132) = r5 }
	{ memw(r29+128) = r7; memw(r29) = r5 }
	{ call logmsg_function }
	{ immext(#0x16340); r3 = add(PC,#0x16379); r1 = #0x86 }
	{ r2 = memw(r24+12); if (!cmp.eq(r2.new,#0x2)) jump:t 0000FCC4 }

l0000FBD8:
	{ r3 = add(PC,#0x25); r1 = #0x87 }
	{ r2 = memw(r24+8); if (!cmp.eq(r2.new,#0x4)) jump:t 0000FCC4 }

l0000FBEC:
	{ r3 = add(PC,#0x1F); r1 = #0x88 }
	{ r2 = or(r21,r20); if (!cmp.eq(r2.new,#0x0)) jump:t 0000FCC4 }

l0000FC00:
	{ r3 = add(PC,#0x1D); r1 = #0x89 }
	{ r2 = or(r22,r23); if (!cmp.eq(r2.new,#0x0)) jump:t 0000FCC4 }

l0000FC14:
	{ p0 = cmp.gt(r19,r26); if (!p0.new) r2 = add(r17,#0x0); if (p0.new) memw(r29) = r26 }
	{ immext(#0x16340); r3 = add(PC,#0x16349); r1 = #0x8A; memw(r29+4) = r19 }
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
	{ jump 0000FF10; r0 = #0xFFFFFFFF }
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
	{ r17:r16 = memd(r29+184); r19:r18 = memd(r29+176) }
	{ r21:r20 = memd(r29+168); r23:r22 = memd(r29+160) }
	{ r25:r24 = memd(r29+152); r27:r26 = memd(r29+144) }
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
	{ immext(#0x15F00); r4 = add(PC,#0x15F17); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xA7; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memb(r17+32) }
	{ r2 = add(r2,#0xFFFFFFFD) }
	{ p0 = cmpb.gtu(r2,#0x1); if (!p0.new) jump:nt 0000FFF8; if (p0.new) r1 = #0xAA }

l0000FFE0:
	{ immext(#0x15EC0); r3 = add(PC,#0x15EF9) }
	{ call errlog_function; r2 = r16 }

l0000FFEC:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0000FFF8:
	{ r1 = #0xAC; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 00010010 }

l00010008:
	{ r3 = add(PC,#0x29); jump 0000FFEC }

l00010010:
	{ r1 = #0xAD; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00010028 }

l00010020:
	{ r3 = add(PC,#0x20); jump 0000FFEC }

l00010028:
	{ immext(#0x15EC0); r4 = add(PC,#0x15EE4); r1 = #0xAE; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00010048
;;   Called from:
;;     0000FBBC (in mirrorpad_f_execute)
;;     0000FFC4 (in mirrorpad_f_check)
;;     00010038 (in mirrorpad_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001006C }

l00010058:
	{ r0 = add(PC,#0x14); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001006C:
	{ dealloc_return }

;; errlog_function: 00010070
;;   Called from:
;;     0000FCC8 (in mirrorpad_f_execute)
;;     0000FFE8 (in mirrorpad_f_check)
;;     00010060 (in logmsg_function)
errlog_function proc
	{ immext(#0x15E00); r0 = add(PC,#0x15E38); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00010094             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; add_int32_execute: 000100A0
add_int32_execute proc
	{ r5 = r0; allocframe(#0xA0) }
	{ r2 = memw(r5+4); r3 = memw(r5+8) }
	{ memd(r29+112) = r27:r26; memd(r29+144) = r19:r18 }
	{ r26 = memw(r2); r19 = memw(r2+4) }
	{ r22 = #0x0; memd(r29+136) = r21:r20; memd(r29+128) = r23:r22 }
	{ r6 = memw(r26+4); r0 = memw(r26) }
	{ p0 = cmp.eq(r6,#0x1); p1 = cmp.eq(r0,#0x1); r8 = memw(r19); r4 = memw(r19+4) }
	{ r10 = p1; r12 = memw(r26+8) }
	{ r18 = mux(p0,r4,r6); p2 = cmp.eq(r12,#0x1); r9 = memw(r19+8); memw(r29+92) = r4 }
	{ r4 = mux(p1,r8,r0); r7 = memw(r26+12) }
	{ r2 = mpyi(r4,r18); p1 = cmp.eq(r7,#0x1); r0 = memw(r19+12) }
	{ r20 = mux(p2,r9,r12); memd(r29+120) = r25:r24; memw(r29+76) = r6 }
	{ r2 = mpyi(r2,r20); r21 = mux(p1,r0,r7) }
	{ r2 = mpyi(r2,r21); r6 = r1; r25 = memw(r3); memd(r29+152) = r17:r16 }
	{ r10 = p2; memw(r29+96) = r10; memw(r29+72) = r8 }
	{ memw(r29+68) = r9; memw(r29+84) = r10 }
	{ r16 = asl(r2,#0x2); memw(r29+104) = r4; memw(r29+80) = r0 }
	{ if (p0) jump:nt 0001014C }

l00010148:
	{ r22 = mpyi(r7,r12) }

l0001014C:
	{ immext(#0x15E80); r0 = add(PC,#0x15EA9); r1 = #0xBD; r2 = r6 }
	{ immext(#0x15E80); r4 = add(PC,#0x15EB8); r3 = memw(r19+16); memw(r29+64) = r12 }
	{ r24 = r6; r27 = r5; r23 = memw(r25+16); memw(r29+88) = r7 }
	{ r17 = memw(r26+16) }
	{ call logmsg_function; memw(r29+100) = r3; memw(r29) = r5 }
	{ r1 = #0xBD; r2 = memw(r25+20); if (!cmp.gtu(r16,r2.new)) jump:t 000101B4 }

l00010198:
	{ r0 = add(PC,#0x21); r2 = r24 }
	{ immext(#0x15E80); r3 = add(PC,#0x15E92) }

l000101A8:
	{ call errlog_function }
	{ jump 000103E0; r0 = #0xFFFFFFFF }

l000101B4:
	{ r2 = r24; r13 = memw(r19); r5 = memw(r26) }
	{ p0 = cmp.eq(r5,r13); r8 = memw(r26+12); r7 = memw(r26+8) }
	{ r6 = memw(r26+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+48) = r25; memw(r29+52) = r16 }
	{ memw(r29+56) = r17; memw(r29+60) = r27 }
	{ if (p0) jump:nt 000101FC }

l000101F0:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 000101FC }

l000101F4:
	{ if (!p0.new) jump:nt 00010238; p0 = cmp.eq(r13,#0x1) }

l000101FC:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 0001020C }

l00010200:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 0001020C }

l00010204:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 00010238; nop }

l0001020C:
	{ if (p0.new) jump:nt 00010220; p0 = cmp.eq(r7,r9) }

l00010214:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 00010220 }

l00010218:
	{ if (!p0.new) jump:nt 00010238; p0 = cmp.eq(r9,#0x1) }

l00010220:
	{ if (p0.new) jump:nt 0001026C; p0 = cmp.eq(r8,r12) }

l00010228:
	{ if (p0.new) jump:nt 0001026C; p0 = cmp.eq(r8,#0x1) }

l00010230:
	{ if (p0.new) jump:nt 0001026C; p0 = cmp.eq(r12,#0x1) }

l00010238:
	{ immext(#0x15D80); r0 = add(PC,#0x15DBD); r1 = #0xBD; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ immext(#0x15DC0); r3 = add(PC,#0x15DF0); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ memw(r29+12) = r8; memw(r29+4) = r6 }
	{ jump 000101A8; memw(r29) = r5 }

l0001026C:
	{ r19 = r2; r24 = memw(r29+104); memw(r29+44) = r21 }
	{ immext(#0x15D40); r0 = add(PC,#0x15D7D); memw(r29+36) = r18; memw(r29+20) = r3 }
	{ immext(#0x15DC0); r4 = add(PC,#0x15DEC); r1 = #0xBD; memw(r29+40) = r20 }
	{ memw(r29+32) = r24; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ memw(r29+12) = r8; memw(r29+8) = r7 }
	{ memw(r29+4) = r6; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r24,#0x0); if (p0.new) r14 = #0x0; r3 = memw(r29+48); r26 = memw(r29+100) }
	{ r25 = memw(r29+56) }
	{ r2 = memw(r29+52); memb(r3+6) = r2.new }
	{ memw(r3+4) = r18 }
	{ memw(r3+8) = r20; memw(r3+12) = r21 }
	{ if (!p0) jump:nt 000103B8; if (p0) r13 = memw(r29+80); if (p0) r12 = memw(r29+88) }

l000102E8:
	{ r6 = !cmp.eq(r13,00000001); r5 = memd(r29+68); r7 = memd(r29+92) }
	{ r9 = mpyi(r5,r7); r0 = memd(r29+84); r2 = memd(r29+76) }
	{ p2 = r0; p0 = cmp.eq(r5,#0x1); r3 = memd(r29+64); r4 = memd(r29+72) }
	{ r8 = mpyi(r3,r2); r9 = mpyi(r9,r13); p1 = cmp.eq(r4,#0x1); r0 = memw(r29+96) }
	{ r4 = mpyi(r13,r5); r2 = mux(p2,#0x0,r12); p2 = cmp.eq(r7,#0x1); if (p1) r9 = add(r14,#0x0) }
	{ r8 = mpyi(r8,r12); p0 = r0; r3 = mux(p0,#0x0,r13); r5 = !cmp.eq(r12,00000001) }
	{ r7 = #0x0; if (p2) r4 = add(r14,#0x0); if (p0) r8 = add(r14,#0x0) }

l00010340:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 000103A8; r13:r12 = combine(r25,r26); r14 = #0x0 }

l00010344:
	{ r13:r12 = combine(r25,r26); r14 = #0x0 }

l0001034C:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 00010398; r28 = r12; r15 = r13 }

l00010358:
	{ loop1(0001035C,r20) }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0001038C; r10 = r23; if (!p0) r1:r0 = combine(r15,r28) }

l00010368:
	{ loop0(0001036C,r21) }
	{ r0 = addasl(r0,r6,#0x2); r1 = addasl(r1,r5,#0x2); r11 = memw(r1); r16 = memw(r0) }
	{ r11 = add(r16,r11) }
	{ nop; memw(r10++#4) = r11 }
	{ r23 = addasl(r23,r21,#0x2) }

l0001038C:
	{ r28 = addasl(r28,r3,#0x2); r15 = addasl(r15,r2,#0x2); nop }

l00010398:
	{ r13 = addasl(r13,r22,#0x2); r12 = addasl(r12,r4,#0x2); r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r18)) jump:t 0001034C }

l000103A8:
	{ r25 = addasl(r25,r8,#0x2); r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 00010340 }

l000103AC:
	{ r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 00010344 }

l000103B8:
	{ immext(#0x15C00); r0 = add(PC,#0x15C3D); r2 = memw(r29+60); memb(r29) = r2.new }

l000103BC:
	{ r0 = add(PC,#0x3D); r2 = memw(r29+60); memb(r29) = r2.new }

l000103CC:
	{ r4 = add(PC,#0x18); r1 = #0xBD; r2 = r19 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l000103E0:
	{ r17:r16 = memd(r29+152); r19:r18 = memd(r29+144) }
	{ r21:r20 = memd(r29+136); r23:r22 = memd(r29+128) }
	{ r25:r24 = memd(r29+120); r27:r26 = memd(r29+112) }
	{ dealloc_return }

;; add_int32_check: 000103F4
add_int32_check proc
	{ immext(#0x15BC0); r4 = add(PC,#0x15BC6); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x37; r16 = r1 }
	{ immext(#0x15B80); r0 = add(PC,#0x15B99); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x38; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 00010444 }

l00010428:
	{ r0 = add(PC,#0x3D) }
	{ immext(#0x15B80); r3 = add(PC,#0x15B9A) }

l00010434:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00010444:
	{ r1 = #0x39; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00010464 }

l00010454:
	{ r0 = add(PC,#0x11) }
	{ immext(#0x15B40); r3 = add(PC,#0x15B7D); jump 00010434 }

l00010464:
	{ immext(#0x15B00); r0 = add(PC,#0x15B3D); r1 = #0x3A; r2 = r16 }
	{ immext(#0x15B40); r4 = add(PC,#0x15B75); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00010488
;;   Called from:
;;     00010180 (in add_int32_execute)
;;     000102B0 (in add_int32_execute)
;;     000103D8 (in add_int32_execute)
;;     00010414 (in add_int32_check)
;;     00010470 (in add_int32_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000104A8 }

l00010498:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l000104A8:
	{ dealloc_return }

;; errlog_function: 000104AC
;;   Called from:
;;     000101A8 (in add_int32_execute)
;;     00010434 (in add_int32_check)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; prod_f_execute: 000104D0
;;   Called from:
;;     000104CC (in close_check__merged)
prod_f_execute proc
	{ r28 = #0x1; allocframe(#0x30) }
	{ r2 = memw(r0+8); r8 = memw(r0+4) }
	{ r6 = memw(r0+16); memd(r29+40) = r17:r16 }
	{ r2 = r1; r7 = memw(r8); r14 = memw(r2) }
	{ p0 = cmp.eq(r6,#0x3); if (!p0.new) r17 = #0x1; memd(r29+32) = r19:r18; memd(r29+24) = r21:r20 }
	{ if (!p0) r1 = #0x1; if (!p0) r10 = #0x1; r15 = memw(r7); r3 = memw(r7+12) }
	{ if (p0) r18 = #0x0; if (p0) r13:r12 = combine(r3,r15); r4 = memw(r7+8); r5 = memw(r7+4) }
	{ r6 = memw(r7+16); r7 = memw(r14+16) }
	{ memd(r29+16) = r23:r22; memd(r29+8) = r25:r24 }
	{ if (p0) jump:nt 0001053C; memd(r29) = r27:r26 }

l00010530:
	{ r13:r12 = combine(#0x1,#0x1); r9:r8 = combine(#0x1,#0x1) }
	{ jump 0001067C }

l0001053C:
	{ r9:r8 = combine(r4,r5); r10 = memw(r8+8); r1 = memw(r8+4) }
	{ r10 = memw(r10+16); r11 = memw(r1+12) }
	{ r1 = memw(r1+16); r10 = memw(r10) }
	{ if (p0.new) jump:nt 000105B8; p0 = cmp.eq(r11,#0x0); if (!p0.new) r9:r8 = combine(r4,r5); if (!p0.new) r16 = add(r1,#0x0) }

l00010568:
	{ loop0(00010570,r11); r13:r12 = combine(r3,r15) }
	{ r17 = memw(r16++#4) }
	{ r17 = add(r10,sub(#0x7F,r17)); if (!cmp.gt(r17.new,#0x3)) jump:t 0001058C }

l00010580:
	{ r9:r8 = combine(#0x1,#0x1); r13 = #0x1; r12 = #0x1 }

l0001058C:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:t 000105AC; if (p0.new) r13 = #0x1 }

l00010594:
	{ p0 = cmp.eq(r9,#0x1); if (p0.new) jump:t 000105AC; if (p0.new) r9 = #0x1 }

l0001059C:
	{ p0 = cmp.eq(r9,#0x2); if (p0.new) jump:t 000105AC; if (p0.new) r8 = #0x1 }

l000105A4:
	{ p0 = cmp.eq(r17,#0x3); if (p0.new) r12 = #0x1 }

l000105AC:
	{ nop; nop }
	{ r18 = r11 }

l000105B8:
	{ p0 = cmp.eq(r18,#0x0); r11 = r9; r0 = memb(r0+32); if (cmp.eq(r0.new,#0x2)) jump:t 000105DC }

l000105CC:
	{ r17 = r8; r28 = r12; r1 = r13 }
	{ jump 0001067C }

l000105DC:
	{ if (!p0) r11 = add(r9,#0x0); r17:r16 = combine(r8,r12); r0 = r13 }
	{ if (p0) jump:nt 00010638; if (!p0) r0 = add(r13,#0x0); if (p0.new) r17:r16 = combine(r8,r12) }

l000105F4:
	{ loop0(000105F8,r18) }
	{ r18 = memw(r1++#4) }
	{ r18 = add(r10,sub(#0x7F,r18)); if (!cmp.gt(r18.new,#0x3)) jump:t 00010614 }

l00010608:
	{ r17 = #0x0; r0 = #0x0; r16 = #0x0 }
	{ jump 00010630 }

l00010614:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:t 00010630; if (p0.new) r0 = #0x0 }

l0001061C:
	{ p0 = cmp.eq(r10,#0x1); if (p0.new) jump:t 00010630; if (p0.new) r11 = #0x0 }

l00010624:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:t 00010630; if (p0.new) r17 = #0x0 }

l0001062C:
	{ p0 = cmp.eq(r18,#0x3); r16 = #-0x1 }

l00010630:
	{ nop; nop }

l00010638:
	{ p1 = cmp.eq(r16,#0x0); p2 = cmp.eq(r17,#0x0); p0 = cmp.eq(r11,#0x0); p3 = cmp.eq(r0,#0x0) }
	{ p1 = or(p1,p2); r1 = mux(p1,#0x1,r16) }
	{ p2 = or(p1,p0); if (!p2) r1 = add(r17,#0x0); r10 = mux(p1,#0x1,r16) }
	{ if (p3) jump:nt 0001067C; if (!p0) r10 = add(r1,#0x0); if (!p0) r1 = add(r11,#0x0); r17 = mux(p2,#0x1,r16) }

l0001066C:
	{ r28 = r17; r17 = r10; r10 = r1; r1 = r0 }

l0001067C:
	{ r0 = mpyi(r13,r9); p0 = cmp.gt(r12,#0x0); r11 = memw(r14+20) }
	{ r0 = mpyi(r0,r8) }
	{ r0 = mpyi(r0,r12) }
	{ r0 = asl(r0,#0x2); if (!cmp.gtu(r0.new,r11)) jump:t 000106B8 }

l0001069C:
	{ r0 = add(PC,#0x3A); r1 = #0xC6 }
	{ immext(#0x15A80); r3 = add(PC,#0x15A8D); call errlog_function }
	{ jump 00010814; r0 = #0xFFFFFFFF }

l000106B8:
	{ r0 = #0x0; memw(r14+12) = r1; memw(r14+24) = r0 }
	{ if (p0) r0 = #0x0; memw(r14+8) = r10; memw(r14+4) = r17 }
	{ if (!p0) jump:nt 00010814; immext(#0x3F800000); if (p0) r1 = #0x3F800000; memw(r14) = r28 }

l000106E0:
	{ p0 = cmp.eq(r12,#0x1); p3 = cmp.eq(r9,#0x1); p1 = cmp.eq(r13,#0x1); p2 = cmp.eq(r8,#0x1) }
	{ r2 = mux(p0,r15,#0x1); r28 = mux(p3,r4,#0x1); r14 = mux(p1,r3,#0x1) }
	{ r15 = mux(p2,r5,#0x1) }

l00010700:
	{ if (!p0.new) jump:nt 00010808; r10 = #0x0; p0 = cmp.gt(r8,#0x0) }

l00010704:
	{ r10 = #0x0; p0 = cmp.gt(r8,#0x0) }

l0001070C:
	{ if (!p0.new) jump:nt 00010800; r11 = #0x0; p0 = cmp.gt(r9,#0x0) }

l00010710:
	{ r11 = #0x0; p0 = cmp.gt(r9,#0x0) }

l00010718:
	{ if (!p0.new) jump:nt 000107F8; r17:r16 = combine(#0x0,r7); p0 = cmp.gt(r13,#0x0) }

l00010724:
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 000107E4; r18 = r1 }

l0001072C:
	{ r19:r18 = combine(#0x0,r1) }

l00010730:
	{ if (!p0.new) jump:nt 000107DC; r22 = add(r19,r0); p0 = cmp.gt(r15,#0x0) }

l00010734:
	{ r22 = add(r19,r0); p0 = cmp.gt(r15,#0x0) }

l0001073C:
	{ r21:r20 = combine(r10,#0x0) }
	{ r21 += mpyi(r22,r5) }

l00010744:
	{ if (!p0.new) jump:nt 000107D4; r24 = add(r21,r20); p0 = cmp.gt(r28,#0x0) }

l00010750:
	{ loop1(0001075C,r28); r23:r22 = combine(r11,#0x0) }
	{ r23 += mpyi(r24,r4) }
	{ if (!p0.new) jump:nt 000107C8; r25 = add(r23,r22); r24 = r17; p0 = cmp.gt(r14,#0x0) }

l0001076C:
	{ r26 = add(r14,#0xFFFFFFFF); p0 = cmp.gtu(r14,#0x1) }
	{ r24 += mpyi(r25,r3); if (p0) r27 = add(r26,#0xFFFFFFFF) }
	{ r25 = addasl(r6,r24,#0x2) }
	{ r24 = add(r25,#0x4) }
	{ if (p0) jump:nt 00010794; r25 = memw(r25) }

l0001078C:
	{ jump 000107C4; r24 = r25 }

l00010794:
	{ loop0(000107A8,r27); p0 = cmp.gtu(r26,#0x1); r26 = add(r24,#0x4); r24 = memw(r24) }
	{ if (!p0) jump:nt 000107C0 }

l000107A8:
	{ r18 = sfmpy(r18,r25); r27 = r24; r26 = add(r26,#0x4); r24 = memw(r26) }
	{ r25 = r27; nop }

l000107C0:
	{ r18 = sfmpy(r18,r25) }

l000107C4:
	{ r18 = sfmpy(r18,r24) }

l000107C8:
	{ r22 = add(r22,#0x1); nop; nop }

l000107D4:
	{ r20 = add(r20,#0x1); if (!cmp.eq(r20.new,r15)) jump:t 00010744 }

l000107DC:
	{ r19 = add(r19,#0x1); if (!cmp.eq(r19.new,r2)) jump:t 00010730 }

l000107E0:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00010734 }

l000107E4:
	{ r16 = add(r16,#0x4); r17 = r17; memw(r16) = r18 }

l000107E8:
	{ r17 = r17; memw(r16) = r18 }

l000107EC:
	{ if (!p0.new) jump:nt 00010724; p0 = cmp.eq(r17,r13) }

l000107F4:
	{ r7 = addasl(r7,r13,#0x2) }

l000107F8:
	{ r11 = add(r11,#0x1); if (!cmp.eq(r11.new,r9)) jump:t 00010718 }

l00010800:
	{ r10 = add(r10,#0x1); if (!cmp.eq(r10.new,r8)) jump:t 0001070C }

l00010804:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00010710 }

l00010808:
	{ r0 = add(r0,#0x1); if (!cmp.eq(r0.new,r12)) jump:t 00010700 }

l0001080C:
	{ if (!cmp.eq(r0.new,r12)) jump:t 00010704 }

l00010814:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); r27:r26 = memd(r29) }
	{ dealloc_return }
00010828                         00 40 00 7F 00 C0 00 7F         .@......

;; prod_f_check: 00010830
prod_f_check proc
	{ immext(#0x15880); r4 = add(PC,#0x158A7); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x37; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16) }
	{ r3 = #0x4; if (cmp.gtu(r3.new,r2)) jump:t 0001087C }

l0001085C:
	{ r0 = add(PC,#0x23); r1 = #0x38 }

l00010864:
	{ immext(#0x15840); r3 = add(PC,#0x1587F) }

l0001086C:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001087C:
	{ p0 = cmp.eq(r2,#0x0); if (!p0.new) jump:nt 00010890; r1 = #0x39 }

l00010884:
	{ immext(#0x15800); r0 = add(PC,#0x15837); jump 00010864 }

l00010890:
	{ r1 = #0x3A; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 000108B0 }

l000108A0:
	{ r0 = add(PC,#0x1F) }
	{ immext(#0x15840); r3 = add(PC,#0x1584E); jump 0001086C }

l000108B0:
	{ immext(#0x15840); r4 = add(PC,#0x15852); r1 = #0x3B; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 000108CC
;;   Called from:
;;     00010844 (in prod_f_check)
;;     000108BC (in prod_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000108F0 }

l000108DC:
	{ r0 = add(PC,#0x23); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l000108F0:
	{ dealloc_return }

;; errlog_function: 000108F4
;;   Called from:
;;     000106A4 (in prod_f_execute)
;;     0001086C (in prod_f_check)
;;     000108E4 (in logmsg_function)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

;; mul_int32_execute: 00010910
;;   Called from:
;;     000108FC (in errlog_function)
mul_int32_execute proc
	{ r5 = r0; allocframe(#0xA0) }
	{ r2 = memw(r5+4); r3 = memw(r5+8) }
	{ memd(r29+112) = r27:r26; memd(r29+144) = r19:r18 }
	{ r26 = memw(r2); r19 = memw(r2+4) }
	{ r22 = #0x0; memd(r29+136) = r21:r20; memd(r29+128) = r23:r22 }
	{ r6 = memw(r26+4); r0 = memw(r26) }
	{ p0 = cmp.eq(r6,#0x1); p1 = cmp.eq(r0,#0x1); r8 = memw(r19); r4 = memw(r19+4) }
	{ r10 = p1; r12 = memw(r26+8) }
	{ r18 = mux(p0,r4,r6); p2 = cmp.eq(r12,#0x1); r9 = memw(r19+8); memw(r29+92) = r4 }
	{ r4 = mux(p1,r8,r0); r7 = memw(r26+12) }
	{ r2 = mpyi(r4,r18); p1 = cmp.eq(r7,#0x1); r0 = memw(r19+12) }
	{ r20 = mux(p2,r9,r12); memd(r29+120) = r25:r24; memw(r29+76) = r6 }
	{ r2 = mpyi(r2,r20); r21 = mux(p1,r0,r7) }
	{ r2 = mpyi(r2,r21); r6 = r1; r25 = memw(r3); memd(r29+152) = r17:r16 }
	{ r10 = p2; memw(r29+96) = r10; memw(r29+72) = r8 }
	{ memw(r29+68) = r9; memw(r29+84) = r10 }
	{ r16 = asl(r2,#0x2); memw(r29+104) = r4; memw(r29+80) = r0 }
	{ if (p0) jump:nt 000109BC }

l000109B8:
	{ r22 = mpyi(r7,r12) }

l000109BC:
	{ immext(#0x157C0); r0 = add(PC,#0x157D7); r1 = #0xBD; r2 = r6 }
	{ immext(#0x157C0); r4 = add(PC,#0x157E6); r3 = memw(r19+16); memw(r29+64) = r12 }
	{ r24 = r6; r27 = r5; r23 = memw(r25+16); memw(r29+88) = r7 }
	{ r17 = memw(r26+16) }
	{ call logmsg_function; memw(r29+100) = r3; memw(r29) = r5 }
	{ r1 = #0xBD; r2 = memw(r25+20); if (!cmp.gtu(r16,r2.new)) jump:t 00010A24 }

l00010A08:
	{ r0 = add(PC,#0xF); r2 = r24 }
	{ immext(#0x157C0); r3 = add(PC,#0x157C0) }

l00010A18:
	{ call errlog_function }
	{ jump 00010C50; r0 = #0xFFFFFFFF }

l00010A24:
	{ r2 = r24; r13 = memw(r19); r5 = memw(r26) }
	{ p0 = cmp.eq(r5,r13); r8 = memw(r26+12); r7 = memw(r26+8) }
	{ r6 = memw(r26+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+48) = r25; memw(r29+52) = r16 }
	{ memw(r29+56) = r17; memw(r29+60) = r27 }
	{ if (p0) jump:nt 00010A6C }

l00010A60:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 00010A6C }

l00010A64:
	{ if (!p0.new) jump:nt 00010AA8; p0 = cmp.eq(r13,#0x1) }

l00010A6C:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 00010A7C }

l00010A70:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 00010A7C }

l00010A74:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 00010AA8; nop }

l00010A7C:
	{ if (p0.new) jump:nt 00010A90; p0 = cmp.eq(r7,r9) }

l00010A84:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 00010A90 }

l00010A88:
	{ if (!p0.new) jump:nt 00010AA8; p0 = cmp.eq(r9,#0x1) }

l00010A90:
	{ if (p0.new) jump:nt 00010ADC; p0 = cmp.eq(r8,r12) }

l00010A98:
	{ if (p0.new) jump:nt 00010ADC; p0 = cmp.eq(r8,#0x1) }

l00010AA0:
	{ if (p0.new) jump:nt 00010ADC; p0 = cmp.eq(r12,#0x1) }

l00010AA8:
	{ immext(#0x156C0); r0 = add(PC,#0x156EB); r1 = #0xBD; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ immext(#0x15700); r3 = add(PC,#0x1571E); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ memw(r29+12) = r8; memw(r29+4) = r6 }
	{ jump 00010A18; memw(r29) = r5 }

l00010ADC:
	{ r19 = r2; r24 = memw(r29+104); memw(r29+44) = r21 }
	{ immext(#0x15680); r0 = add(PC,#0x156AB); memw(r29+36) = r18; memw(r29+20) = r3 }
	{ immext(#0x15700); r4 = add(PC,#0x1571A); r1 = #0xBD; memw(r29+40) = r20 }
	{ memw(r29+32) = r24; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ memw(r29+12) = r8; memw(r29+8) = r7 }
	{ memw(r29+4) = r6; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r24,#0x0); if (p0.new) r14 = #0x0; r3 = memw(r29+48); r26 = memw(r29+100) }
	{ r25 = memw(r29+56) }
	{ r2 = memw(r29+52); memb(r3+6) = r2.new }
	{ memw(r3+4) = r18 }
	{ memw(r3+8) = r20; memw(r3+12) = r21 }
	{ if (!p0) jump:nt 00010C28; if (p0) r13 = memw(r29+80); if (p0) r12 = memw(r29+88) }

l00010B58:
	{ r6 = !cmp.eq(r13,00000001); r5 = memd(r29+68); r7 = memd(r29+92) }
	{ r9 = mpyi(r5,r7); r0 = memd(r29+84); r2 = memd(r29+76) }
	{ p2 = r0; p0 = cmp.eq(r5,#0x1); r3 = memd(r29+64); r4 = memd(r29+72) }
	{ r8 = mpyi(r3,r2); r9 = mpyi(r9,r13); p1 = cmp.eq(r4,#0x1); r0 = memw(r29+96) }
	{ r4 = mpyi(r13,r5); r2 = mux(p2,#0x0,r12); p2 = cmp.eq(r7,#0x1); if (p1) r9 = add(r14,#0x0) }
	{ r8 = mpyi(r8,r12); p0 = r0; r3 = mux(p0,#0x0,r13); r5 = !cmp.eq(r12,00000001) }
	{ r7 = #0x0; if (p2) r4 = add(r14,#0x0); if (p0) r8 = add(r14,#0x0) }

l00010BB0:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 00010C18; r13:r12 = combine(r25,r26); r14 = #0x0 }

l00010BB4:
	{ r13:r12 = combine(r25,r26); r14 = #0x0 }

l00010BBC:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 00010C08; r28 = r12; r15 = r13 }

l00010BC8:
	{ loop1(00010BCC,r20) }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 00010BFC; r10 = r23; if (!p0) r1:r0 = combine(r15,r28) }

l00010BD8:
	{ loop0(00010BDC,r21) }
	{ r0 = addasl(r0,r6,#0x2); r1 = addasl(r1,r5,#0x2); r11 = memw(r1); r16 = memw(r0) }
	{ r11 = mpyi(r16,r11) }
	{ nop; memw(r10++#4) = r11 }
	{ r23 = addasl(r23,r21,#0x2) }

l00010BFC:
	{ r28 = addasl(r28,r3,#0x2); r15 = addasl(r15,r2,#0x2); nop }

l00010C08:
	{ r13 = addasl(r13,r22,#0x2); r12 = addasl(r12,r4,#0x2); r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r18)) jump:t 00010BBC }

l00010C18:
	{ r25 = addasl(r25,r8,#0x2); r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 00010BB0 }

l00010C1C:
	{ r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 00010BB4 }

l00010C28:
	{ immext(#0x15540); r0 = add(PC,#0x1556B); r2 = memw(r29+60); memb(r29) = r2.new }

l00010C2C:
	{ r0 = add(PC,#0x2B); r2 = memw(r29+60); memb(r29) = r2.new }

l00010C3C:
	{ r4 = add(PC,#0x6); r1 = #0xBD; r2 = r19 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00010C50:
	{ r17:r16 = memd(r29+152); r19:r18 = memd(r29+144) }
	{ r21:r20 = memd(r29+136); r23:r22 = memd(r29+128) }
	{ r25:r24 = memd(r29+120); r27:r26 = memd(r29+112) }
	{ dealloc_return }

;; mul_int32_check: 00010C64
mul_int32_check proc
	{ immext(#0x154C0); r4 = add(PC,#0x154F4); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x37; r16 = r1 }
	{ immext(#0x154C0); r0 = add(PC,#0x154C7); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x38; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 00010CB4 }

l00010C98:
	{ r0 = add(PC,#0x2B) }
	{ immext(#0x154C0); r3 = add(PC,#0x154C8) }

l00010CA4:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00010CB4:
	{ r1 = #0x39; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00010CD4 }

l00010CC4:
	{ r0 = add(PC,#0x3F) }
	{ immext(#0x15480); r3 = add(PC,#0x154AB); jump 00010CA4 }

l00010CD4:
	{ immext(#0x15440); r0 = add(PC,#0x1546B); r1 = #0x3A; r2 = r16 }
	{ immext(#0x15480); r4 = add(PC,#0x154A3); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00010CF8
;;   Called from:
;;     000109F0 (in mul_int32_execute)
;;     00010B20 (in mul_int32_execute)
;;     00010C48 (in mul_int32_execute)
;;     00010C84 (in mul_int32_check)
;;     00010CE0 (in mul_int32_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00010D18 }

l00010D08:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00010D18:
	{ dealloc_return }

;; errlog_function: 00010D1C
;;   Called from:
;;     00010A18 (in mul_int32_execute)
;;     00010CA4 (in mul_int32_check)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; pack_execute: 00010D40
;;   Called from:
;;     00010D3C (in errlog_function)
pack_execute proc
	{ r7 = r1; r17 = r0; memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ r3 = memw(r17+4); memd(r29+8) = r27:r26 }
	{ r2 = memw(r17+8) }
	{ memd(r29+40) = r19:r18; memd(r29+32) = r21:r20 }
	{ r26 = memw(r3); r19 = memw(r2) }
	{ r20 = memw(r17+16); memd(r29+24) = r23:r22 }
	{ p0 = cmp.gt(r20,#0x1); if (p0.new) r4 = add(r3,#0x4); r2 = memw(r26+24) }
	{ r21 = memw(r26+8); memd(r29+16) = r25:r24 }
	{ r24 = r2; r25 = memw(r26); r23 = memw(r26+4) }
	{ if (!p0) jump:nt 00010E24; r5 = add(r20,#0xFFFFFFFF); r22 = memw(r26+12); r18 = memw(r19+16) }

l00010D98:
	{ loop0(00010DA0,r5); r24 = r2 }
	{ immext(#0x15500); r3 = add(PC,#0x15500); r5 = memw(r4) }
	{ r6 = memw(r5); if (cmp.eq(r6.new,r25)) jump:t 00010DBC }

l00010DB8:
	{ r1 = #0x42 }

l00010DBC:
	{ immext(#0x154C0); r3 = add(PC,#0x154E4); r6 = memw(r5+4); if (cmp.eq(r6.new,r23)) jump:t 00010DD4 }

l00010DD0:
	{ r1 = #0x43 }

l00010DD4:
	{ immext(#0x154C0); r3 = add(PC,#0x154CC); r6 = memw(r5+8); if (cmp.eq(r6.new,r21)) jump:t 00010DEC }

l00010DE8:
	{ r1 = #0x44 }

l00010DEC:
	{ immext(#0x15480); r3 = add(PC,#0x154B4); r6 = memw(r5+12); if (cmp.eq(r6.new,r22)) jump:t 00010E04 }

l00010E00:
	{ r1 = #0x45 }

l00010E04:
	{ immext(#0x15480); r3 = add(PC,#0x154A6); r5 = memw(r5+24); if (cmp.eq(r5.new,r2)) jump:t 00010E1C }

l00010E18:
	{ r1 = #0x46 }

l00010E1C:
	{ r4 = add(r4,#0x4); r24 = add(r24,r2) }

l00010E24:
	{ immext(#0x15480); r3 = add(PC,#0x1548F); r1 = #0x4A }
	{ r4 = memw(r19+20); if (cmp.gtu(r24,r4.new)) jump:t 00010EC0 }

l00010E3C:
	{ if (p0.new) r16 = #0x4; memw(r29+4) = r7 }
	{ r27 = r20 }

l00010E48:
	{ call fn00009560; r0 = r18; r1 = memw(r26+16) }
	{ r27 = add(r27,#0xFFFFFFFF); r3 = memw(r26+24); if (cmp.eq(r27.new,#0x0)) jump:nt 00010E74 }

l00010E64:
	{ r16 = add(r16,#0x4); r26 = memw(r13+r16) }
	{ jump 00010E48; r2 = memw(r26+24) }

l00010E74:
	{ p0 = cmp.eq(r14,#0x1); if (!p0.new) jump:nt 00010E98; if (p0.new) r22 = add(r20,#0x0) }

l00010E7C:
	{ r20 = r25 }

l00010E80:
	{ r0 = #0x0; memw(r19+24) = r24; memw(r19+4) = r23 }
	{ memw(r19) = r20; memw(r19+8) = r21 }
	{ jump 00010ECC; memw(r19+12) = r22 }

l00010E98:
	{ p0 = cmp.eq(r13,#0x1); if (p0.new) jump:t 00010E7C; if (p0.new) r21 = add(r20,#0x0) }

l00010EA0:
	{ p0 = cmp.eq(r15,#0x1); if (p0.new) jump:t 00010E7C; if (p0.new) r23 = add(r20,#0x0) }

l00010EA8:
	{ immext(#0x15400); r3 = add(PC,#0x15419); r1 = #0x56; r7 = memw(r29+4) }
	{ if (p0.new) jump:nt 00010E80; p0 = cmp.eq(r25,#0x1) }

l00010EC0:
	{ call errlog_function; r2 = r7 }
	{ r0 = #0xFFFFFFFF }

l00010ECC:
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); r27:r26 = memd(r29+8) }
	{ dealloc_return }

;; pack_check: 00010EE0
pack_check proc
	{ r16 = r1; r17 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x5F; r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x0)) jump:t 00010F0C }

l00010F04:
	{ r3 = add(PC,#0x9); jump 00010F28 }

l00010F0C:
	{ r1 = #0x60; r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00010F30 }

l00010F20:
	{ r3 = add(PC,#0x38) }
	{ call errlog_function; r2 = r16 }

l00010F28:
	{ r2 = r16 }

l00010F2C:
	{ r0 = #0xFFFFFFFF }

l00010F30:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00010F34
;;   Called from:
;;     00010EEC (in pack_check)
logmsg_function proc
	{ r4 = #0x2; allocframe(#0x8) }
	{ r3 = memw(r2+16); if (cmp.gtu(r4,r3.new)) jump:t 00010F64 }

l00010F44:
	{ r0 = add(PC,#0x19); r3 = #0x2; r5 = add(r29,#0x10) }
	{ immext(#0x15300); r4 = add(PC,#0x15327); r1 = #0x5E; r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l00010F64:
	{ dealloc_return }

;; errlog_function: 00010F68
;;   Called from:
;;     00010EC0 (in pack_execute)
;;     00010F24 (in pack_check)
errlog_function proc
	{ immext(#0x152C0); r0 = add(PC,#0x152F1); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00010F8C                                     00 00 00 00             ....

;; shape_execute: 00010F90
shape_execute proc
	{ immext(#0x15380); r4 = add(PC,#0x153B7); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r17:r16 = combine(r1,r0); r1 = #0x31 }
	{ r3 = memw(r16+4); memd(r29+8) = r19:r18 }
	{ r2 = r17; r5 = memw(r16+8) }
	{ r19 = memw(r3) }
	{ call logmsg_function; r18 = memw(r5); memw(r29) = r16 }
	{ r2 = memw(r18+20); if (cmp.gtu(r2.new,#0xF)) jump:t 00010FD8 }

l00010FC4:
	{ r3 = add(PC,#0x1F); r1 = #0x33; r2 = r17 }
	{ call errlog_function }
	{ jump 00011024; r0 = #0xFFFFFFFF }

l00010FD8:
	{ immext(#0x15380); r4 = add(PC,#0x15395); r2 = memw(r18+16); memw(r18+4) = #0x1 }
	{ r1 = #0x3B; memw(r18) = #0x1; memw(r18+12) = #0xFFFFFF84 }
	{ memw(r18+8) = #0x1 }
	{ r3 = memw(r19); memb(r2) = r3.new }
	{ memw(r7+4) = r6 }
	{ r3 = memw(r18+16); r2 = memw(r19+8) }
	{ r2 = r17; memw(r3+8) = r2 }
	{ r5 = memw(r18+16); r7 = memw(r19+12) }
	{ memw(r5+12) = r7; memw(r18+24) = #0x10 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }

l00011024:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }
	{ dealloc_return }
0001102C                                     00 C0 00 7F             ....

;; shape_check: 00011030
shape_check proc
	{ immext(#0x152C0); r4 = add(PC,#0x152CF); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x41; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x42; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x1)) jump:t 00011070 }

l0001105C:
	{ r3 = add(PC,#0x3E) }
	{ call errlog_function; r2 = r16 }

l00011064:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00011070:
	{ r1 = #0x43; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00011088 }

l00011080:
	{ r3 = add(PC,#0x29); jump 00011064 }

l00011088:
	{ immext(#0x15280); r4 = add(PC,#0x152AD); r1 = #0x44; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 000110A8
;;   Called from:
;;     00010FB0 (in shape_execute)
;;     00011018 (in shape_execute)
;;     00011044 (in shape_check)
;;     00011098 (in shape_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000110CC }

l000110B8:
	{ r0 = add(PC,#0x30); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l000110CC:
	{ dealloc_return }

;; errlog_function: 000110D0
;;   Called from:
;;     00010FCC (in shape_execute)
;;     00011060 (in shape_check)
;;     000110C0 (in logmsg_function)
errlog_function proc
	{ immext(#0x15200); r0 = add(PC,#0x15214); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
000110F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; and_int32_execute: 00011100
and_int32_execute proc
	{ immext(#0x4C0); r2 = add(PC,#0x4C8); jump broadcast_elementwise_execute_int32 }

;; logical_int32_check: 0001110C
logical_int32_check proc
	{ immext(#0x15340); r4 = add(PC,#0x15352); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x3E; r16 = r1 }
	{ immext(#0x15300); r0 = add(PC,#0x15321); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x3F; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001115C }

l00011140:
	{ r0 = add(PC,#0x5) }
	{ immext(#0x15300); r3 = add(PC,#0x1532D) }

l0001114C:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001115C:
	{ r1 = #0x40; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001117C }

l0001116C:
	{ r0 = add(PC,#0x19) }
	{ immext(#0x15300); r3 = add(PC,#0x15310); jump 0001114C }

l0001117C:
	{ immext(#0x152C0); r0 = add(PC,#0x152C5); r1 = #0x41; r2 = r16 }
	{ immext(#0x15300); r4 = add(PC,#0x15304); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; ior_int32_execute: 000111A4
ior_int32_execute proc
	{ immext(#0x400); r2 = add(PC,#0x41C); jump broadcast_elementwise_execute_int32 }

;; xor_int32_execute: 000111B0
xor_int32_execute proc
	{ immext(#0x3C0); r2 = add(PC,#0x3C8); jump broadcast_elementwise_execute_int32 }
000111BC                                     00 C0 00 7F             ....

;; broadcast_elementwise_execute_int32: 000111C0
;;   Called from:
;;     00011100 (in and_int32_execute)
;;     000111A4 (in ior_int32_execute)
;;     000111B0 (in xor_int32_execute)
broadcast_elementwise_execute_int32 proc
	{ r25:r24 = combine(r1,r0); memd(r29-48) = r25:r24; allocframe(#0xA8) }
	{ r3 = memw(r24+4); memd(r29+136) = r23:r22 }
	{ r2 = #0x0; r18 = r2; memd(r29+152) = r19:r18 }
	{ memd(r29+144) = r21:r20 }
	{ r23 = memw(r3); memd(r29+160) = r17:r16 }
	{ r19 = memw(r3+4); r4 = memw(r24+8) }
	{ r12 = memw(r23+4); memd(r29+120) = r27:r26 }
	{ p0 = cmp.eq(r12,#0x1); r0 = memw(r23); r7 = memw(r19) }
	{ p1 = cmp.eq(r0,#0x1); r5 = memw(r19+4); r21 = memw(r23+8) }
	{ r6 = mux(p0,r5,r12); p2 = cmp.eq(r21,#0x1); r9 = memw(r19+8); memw(r29+104) = r5 }
	{ r10 = p1; r5 = mux(p1,r7,r0); r8 = memw(r23+12) }
	{ r3 = mpyi(r5,r6); p1 = cmp.eq(r8,#0x1); r0 = memw(r19+12); memw(r29+72) = r7 }
	{ r7 = mux(p2,r9,r21); r17 = memw(r4) }
	{ r3 = mpyi(r3,r7); r10 = p2; memw(r29+100) = r10; memw(r29+60) = r12 }
	{ r16 = mux(p1,r0,r8); memw(r29+68) = r5; memw(r29+88) = r6 }
	{ r3 = mpyi(r3,r16); memw(r29+64) = r9; memw(r29+108) = r10 }
	{ memw(r29+112) = r7; memw(r29+92) = r0 }
	{ r22 = asl(r3,#0x2); memw(r29+84) = r2 }
	{ if (p0) jump:nt 00011270 }

l00011268:
	{ r2 = mpyi(r8,r21) }
	{ memw(r29+84) = r2 }

l00011270:
	{ immext(#0x15100); r0 = add(PC,#0x1510B); r1 = #0xBD; r2 = r25 }
	{ immext(#0x15100); r4 = add(PC,#0x1511A); r27 = memw(r17+16); memw(r29+96) = r8 }
	{ r20 = memw(r19+16); r26 = memw(r23+16) }
	{ call logmsg_function; memw(r29) = r24 }
	{ r1 = #0xBD; r2 = memw(r17+20); if (!cmp.gtu(r22,r2.new)) jump:t 000112CC }

l000112B0:
	{ r0 = add(PC,#0xF); r2 = r25 }
	{ immext(#0x15100); r3 = add(PC,#0x15100) }

l000112C0:
	{ call errlog_function }
	{ jump 00011564; r0 = #0xFFFFFFFF }

l000112CC:
	{ r2 = memw(r19); r5 = memw(r23) }
	{ p0 = cmp.eq(r5,r2); r8 = memw(r23+12); r7 = memw(r23+8) }
	{ r6 = memw(r23+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+56) = r22; memw(r29+76) = r26 }
	{ if (p0) jump:nt 00011304; memw(r29+80) = r20 }

l000112FC:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 00011304 }

l00011300:
	{ p0 = cmp.eq(r2,#0x1); if (!p0.new) jump:nt 00011340 }

l00011304:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 00011314; nop }

l0001130C:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 00011314 }

l00011310:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 00011340 }

l00011314:
	{ if (p0.new) jump:nt 00011328; p0 = cmp.eq(r7,r9) }

l0001131C:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 00011328 }

l00011320:
	{ if (!p0.new) jump:nt 00011340; p0 = cmp.eq(r9,#0x1) }

l00011328:
	{ if (p0.new) jump:nt 00011374; p0 = cmp.eq(r8,r12) }

l00011330:
	{ if (p0.new) jump:nt 00011374; p0 = cmp.eq(r8,#0x1) }

l00011338:
	{ if (p0.new) jump:nt 00011374; p0 = cmp.eq(r12,#0x1) }

l00011340:
	{ immext(#0x15000); r0 = add(PC,#0x1503B); r1 = #0xBD; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+12) = r8 }
	{ immext(#0x15040); r3 = add(PC,#0x1506E); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ r2 = r25; memw(r29+16) = r2; memw(r29+4) = r6 }
	{ jump 000112C0; memw(r29) = r5 }

l00011374:
	{ immext(#0x15000); r0 = add(PC,#0x15007); r23 = memd(r29+112); memw(r29+44) = r16 }
	{ immext(#0x15040); r4 = add(PC,#0x15076); r1 = #0xBD; r19 = memw(r29+68) }
	{ r20 = memd(r29+88); memw(r29+40) = r23 }
	{ memw(r29+36) = r20; memw(r29+32) = r19 }
	{ memw(r29+52) = r24; memw(r29+28) = r12 }
	{ memw(r29+48) = r25; memw(r29+24) = r9 }
	{ memw(r29+20) = r3; memw(r29+8) = r7 }
	{ r2 = r25; memw(r29+16) = r2; memw(r29+4) = r6 }
	{ memw(r29+12) = r8; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r19,#0x0); r4 = memw(r29+80) }
	{ r5 = memd(r29+76); r2 = memd(r29+56) }
	{ memw(r17+24) = r2; memw(r17) = r19 }
	{ memw(r17+4) = r20; memw(r17+8) = r23 }
	{ if (!p0) jump:nt 0001153C; memw(r17+12) = r16 }

l000113DC:
	{ r1 = #0x0; r2 = memw(r29+60); r9 = memw(r29+64) }
	{ r2 = mpyi(r21,r2); p0 = cmp.eq(r9,#0x1); r6 = memd(r29+104); r0 = memd(r29+108) }
	{ r3 = mpyi(r9,r6); r7 = memw(r29+72); r8 = memw(r29+92) }
	{ r9 = mpyi(r8,r9); p2 = r0; r13 = mux(p0,#0x0,r8); r0 = memw(r29+100) }
	{ r3 = mpyi(r3,r8); p1 = cmp.eq(r7,#0x1); r7 = memd(r29+96); memw(r29+64) = r1 }
	{ r12 = mux(p2,#0x0,r7); p2 = cmp.eq(r6,#0x1); r6 = #0x0; r25 = !cmp.eq(r7,00000001) }
	{ r2 = mpyi(r2,r7); p0 = r0; r26 = !cmp.eq(r8,00000001); if (p2) r9 = add(r6,#0x0) }
	{ if (p1) r3 = add(r6,#0x0); if (p0) r2 = add(r6,#0x0); memw(r29+108) = r12; memw(r29+72) = r9 }
	{ memw(r29+104) = r13; memw(r29+60) = r2 }
	{ memw(r29+56) = r3 }

l00011458:
	{ r3 = r4; r7:r6 = combine(#0x0,r5); r2 = memd(r29+88); memw(r29+76) = r5 }
	{ p0 = cmp.gt(r2,#0x0); memw(r29+80) = r4 }
	{ if (!p0) jump:nt 0001151C }

l00011470:
	{ r2 = memw(r29+112); if (cmp.gt(r2.new,#0x0)) jump:t 00011484 }

l0001147C:
	{ jump 000114FC; memw(r29+100) = r3 }

l00011484:
	{ r19 = r3; r23 = #0x0; r21 = r6 }
	{ memw(r29+92) = r7; memw(r29+96) = r6 }
	{ memw(r29+100) = r3 }

l00011494:
	{ r20 = r21; r24 = r27; r17 = r19; r22 = r16 }
	{ p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt 000114E4 }

l000114A4:
	{ nop; nop; nop }
	{ nop; nop; nop; nop }

l000114C0:
	{ r22 = add(r22,#0xFFFFFFFF); callr r18; r0 = memw(r20); r1 = memw(r17) }
	{ r20 = addasl(r20,r25,#0x2); r17 = addasl(r17,r26,#0x2); p0 = cmp.eq(r22,#0x0) }
	{ if (!p0) jump:nt 000114C0; memw(r24++#4) = r0 }

l000114E0:
	{ r27 = addasl(r27,r16,#0x2) }

l000114E4:
	{ r23 = add(r23,#0x1); r2 = memd(r29+108); r7 = memd(r29+104) }
	{ r21 = addasl(r21,r2,#0x2) }
	{ r19 = addasl(r19,r7,#0x2); r2 = memw(r29+112); if (!cmp.eq(r2.new,r23)) jump:t 00011494 }

l000114FC:
	{ r2 = memd(r29+84); r6 = memd(r29+96) }

l00011500:
	{ r5 = memd(r29+72); r3 = memd(r29+100) }
	{ r6 = addasl(r6,r2,#0x2); r7 = memd(r29+92); r2 = memd(r29+88) }
	{ r3 = addasl(r3,r5,#0x2); r4 = memd(r29+80); r5 = memd(r29+76) }
	{ r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r2)) jump:t 00011470 }

l0001151C:
	{ r2 = memd(r29+60); r7 = memd(r29+56) }

l00011520:
	{ r3 = memw(r29+64) }
	{ r5 = addasl(r5,r2,#0x2); r4 = addasl(r4,r7,#0x2); r3 = r3; r2 = memd(r29+68) }
	{ p0 = cmp.eq(r3,r2); memw(r29+64) = r3 }
	{ if (!p0) jump:nt 00011458 }

l0001153C:
	{ immext(#0x14E00); r0 = add(PC,#0x14E3F); r2 = memw(r29+52); memb(r29) = r2.new }
	{ r4 = add(PC,#0x1A); r1 = #0xBD; r2 = memw(r29+48) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00011564:
	{ r17:r16 = memd(r29+160); r19:r18 = memd(r29+152) }
	{ r21:r20 = memd(r29+144); r23:r22 = memd(r29+136) }
	{ r25:r24 = memd(r29+128); r27:r26 = memd(r29+120) }
	{ dealloc_return }

;; xor_helper: 00011578
xor_helper proc
	{ r0 = xor(r1,r0); jumpr r31 }

;; logmsg_function: 00011580
;;   Called from:
;;     0001112C (in logical_int32_check)
;;     0001118C (in logical_int32_check)
;;     00011298 (in broadcast_elementwise_execute_int32)
;;     000113BC (in broadcast_elementwise_execute_int32)
;;     0001155C (in broadcast_elementwise_execute_int32)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000115A0 }

l00011590:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l000115A0:
	{ dealloc_return }

;; errlog_function: 000115A4
;;   Called from:
;;     0001114C (in logical_int32_check)
;;     000112C0 (in broadcast_elementwise_execute_int32)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

;; ior_helper: 000115C0
;;   Called from:
;;     000115AC (in errlog_function)
ior_helper proc
	{ r0 = or(r1,r0); jumpr r31 }

;; and_helper: 000115C8
and_helper proc
	{ r0 = and(r1,r0); jumpr r31 }

;; avgpool_execute_asm: 000115D0
avgpool_execute_asm proc
	{ immext(#0x7C0); r2 = add(PC,#0x7D0); jump avgpool_execute }

;; avgpool_check: 000115DC
avgpool_check proc
	{ immext(#0x14F40); r4 = add(PC,#0x14F4A); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r1 = #0x186; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memd(r29+32) = r19:r18; memw(r29) = r17 }
	{ r1 = #0x187; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x5)) jump:t 00011620 }

l0001160C:
	{ r3 = add(PC,#0x37) }

l00011610:
	{ call errlog_function; r2 = r16 }

l00011614:
	{ r2 = r16 }
	{ jump 00011718; r0 = #0xFFFFFFFF }

l00011620:
	{ r1 = #0x18B; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0001163C }

l00011630:
	{ r3 = add(PC,#0x2A); jump 00011614; r1 = #0x188 }

l0001163C:
	{ immext(#0x14F00); r4 = add(PC,#0x14F2F); r3:r2 = combine(#0x3,r16); memw(r29) = r17 }
	{ call logmsg_function }
	{ r19:r18 = combine(#0x0,#0x0); r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 000116C4 }

l0001165C:
	{ r2 = memw(r17+4) }

l00011660:
	{ r2 = memw(r5+r18); if (!cmp.eq(r2.new,#0x0)) jump:t 0001167C }

l0001166C:
	{ r3 = add(PC,#0x0); r1 = #0x18E; memw(r29) = r19 }
	{ jump 00011610 }

l0001167C:
	{ immext(#0x14F40); r4 = add(PC,#0x14F72); r5 = memw(r2); r6 = memw(r2+4) }
	{ r1 = #0x180; r7 = memw(r2+8); r8 = memw(r2+12) }
	{ r9 = memw(r2+24); r2 = memw(r2+16) }
	{ r3:r2 = combine(#0x3,r16); memw(r29+24) = r2; memw(r29+12) = r7 }
	{ memw(r29+20) = r9; memw(r29+16) = r8 }
	{ memw(r29+8) = r6; memw(r29+4) = r5 }
	{ call logmsg_function; memw(r29) = r19 }
	{ r18 = add(r18,#0x4); r2 = memw(r17+16) }
	{ r19 = add(r19,#0x1); if (cmp.gtu(r2,r19.new)) jump:t 0001165C }

l000116C4:
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 000116FC }

l000116C8:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 00011700 }

l000116D0:
	{ r3 = memw(r17+8) }
	{ r3 = add(r3,#0x4); r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 000116FC }

l000116D8:
	{ r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 00011700 }

l000116E4:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 000116D8 }

l000116EC:
	{ r3 = add(PC,#0x16); r1 = #0x194; memw(r29) = r4 }
	{ jump 00011610 }

l000116FC:
	{ immext(#0x14EC0); r4 = add(PC,#0x14ED9); r3:r2 = combine(#0x2,r16); r1 = #0x197 }

l00011700:
	{ r4 = add(PC,#0x19); r3:r2 = combine(#0x2,r16); r1 = #0x197 }

l0001170C:
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l00011718:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ dealloc_return }

;; avgpool_execute_ref: 00011720
avgpool_execute_ref proc
	{ immext(#0x280); r2 = add(PC,#0x290); jump avgpool_execute }
0001172C                                     00 C0 00 7F             ....

;; avgpool_execute: 00011730
;;   Called from:
;;     000115D0 (in avgpool_execute_asm)
;;     00011720 (in avgpool_execute_ref)
avgpool_execute proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x88) }
	{ r3 = memw(r17+4); memd(r29+112) = r21:r20 }
	{ r21 = memb(r17+32); r4 = memw(r17+8) }
	{ memd(r29+88) = r27:r26; memd(r29+96) = r25:r24 }
	{ p0 = cmp.eq(r21,#0x0); r5 = memw(r3); r25 = memw(r3+16) }
	{ r2 = memw(r3+8); memw(r29+8) = r2 }
	{ r0 = p0; r26 = memw(r3+12) }
	{ r2 = memw(r3+4); memw(r29+24) = r2 }
	{ memd(r29+104) = r23:r22; memd(r29+120) = r19:r18 }
	{ r27 = memw(r4); memw(r29+12) = r2 }
	{ r23 = memw(r5); r2 = memw(r5+8) }
	{ r20 = memw(r5+4); r24 = memw(r5+12) }
	{ r1 = memw(r25+8); r22 = memw(r25+4) }
	{ r18 = memw(r26+4); r6 = memw(r4+4) }
	{ r7 = memw(r4+8) }
	{ memw(r29+16) = r6; memw(r29+20) = r7 }
	{ if (!p0) jump:nt 000117A4; memw(r29+28) = r0 }

l000117A0:
	{ r0 = r2; jump 000117C8 }

l000117A4:
	{ p0 = cmp.eq(r13,#0x2); if (p0.new) jump:nt 000117C4; if (p0.new) r2 = add(r1,r2); if (p0.new) r3 = memw(r26+8) }

l000117B0:
	{ p0 = cmp.eq(r13,#0x1); if (!p0.new) jump:nt 000117D0; r19 = #0x0; if (p0.new) r0 = add(r1,#0x0) }

l000117BC:
	{ r0 += add(r2,#0xFFFFFFFF); jump 000117C8 }

l000117C4:
	{ r0 = sub(r2,r3) }

l000117C8:
	{ call fn00009760 }
	{ r19 = r0 }

l000117D0:
	{ p0 = cmp.eq(r13,#0x2); if (p0.new) jump:nt 00011800; r2 = add(r22,r20); if (p0.new) r1 = add(r22,#0x0) }

l000117DC:
	{ p0 = cmp.eq(r13,#0x1); if (p0.new) jump:nt 000117F8; if (!p0) r1:r0 = combine(r22,r22) }

l000117E4:
	{ r21 = #0x0; r0 = memd(r29+28) }
	{ p0 = r0; if (!p0.new) jump:nt 0001180C; if (!p0) r1:r0 = combine(r22,r20) }

l000117F4:
	{ jump 00011804 }

l000117F8:
	{ r0 += add(r20,#0xFFFFFFFF); jump 00011804 }

l00011800:
	{ r0 = sub(r2,r18) }

l00011804:
	{ call fn00009760 }
	{ r21 = r0 }

l0001180C:
	{ r4 = add(r29,#0x20); r5 = add(r29,#0x38); r3:r2 = combine(#0x0,#0x0); memw(r29+32) = r17 }
	{ r20 = add(r4,#0x8); r22 = add(r5,#0x8); memw(r4+4) = #0xFFFFFF81 }
	{ r1:r0 = combine(#0x0,r20); memd(r29+64) = r3:r2; memd(r29+56) = r3:r2 }
	{ memd(r29+72) = r3:r2; memw(r4+20) = #0x0 }
	{ memw(r4+16) = #0x0; memw(r4+12) = #0x0 }
	{ call fn00009740; memw(r4+8) = #0x0; memw(r29+56) = r17 }
	{ call fn00009740; r1:r0 = combine(#0x0,r22) }
	{ immext(#0x14C80); r4 = add(PC,#0x14C80); r3:r2 = combine(#0x2,r16); r1 = #0x151 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r2 = memw(r26); if (!cmp.eq(r2.new,#0x1)) jump:t 0001187C }

l00011868:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 00011880 }

l00011870:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 00011880 }

l00011878:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001189C }

l0001187C:
	{ immext(#0x14C40); r3 = add(PC,#0x14C62); r1 = #0x156 }

l00011880:
	{ r3 = add(PC,#0x22); r1 = #0x156 }

l00011888:
	{ call errlog_function; r2 = r16 }

l0001188C:
	{ r2 = r16 }
	{ jump 0001199C; r0 = #0xFFFFFFFF }
00011898                         02 57 18 ED                     .W..    

l0001189C:
	{ r1 = #0x158; r4 = memw(r27+20) }
	{ r2 = mpyi(r2,r19) }
	{ r3 = mpyi(r2,r21); if (!cmp.gtu(r3.new,r4)) jump:t 000118BC }

l000118B4:
	{ r3 = add(PC,#0x6); jump 0001188C }

l000118BC:
	{ r1 = #0x159; r2 = memb(r17+32); if (!cmp.eq(r2.new,#0x0)) jump:t 000118D4 }

l000118CC:
	{ r3 = add(PC,#0x3C); jump 0001188C }

l000118D4:
	{ r2 = add(r29,#0x20); r18 = memw(r29+8); memw(r27+4) = r21 }
	{ r1:r0 = combine(r18,r16); memw(r27) = r23; memw(r27+12) = r24 }
	{ memw(r27+8) = r19; memw(r27+24) = r3 }
	{ call nn_os_work_for_vector }
	{ callr r18; r1 = add(r29,#0x38); r0 = r16 }
	{ call fn000096A0; r0 = r20 }
	{ r5 = memd(r29+12); r4 = memd(r29+16) }
	{ r2 = memw(r5) }
	{ r3 = memw(r5+4); memb(r4+1) = r3.new }
	{ r2 = memw(r5+8) }
	{ memw(r4+8) = r2 }
	{ r7 = memw(r5+12) }
	{ memw(r4+12) = r7 }
	{ r2 = memw(r5+24) }
	{ r6 = memw(r4+20); if (cmp.gtu(r2,r6.new)) jump:t 00011944 }

l0001193C:
	{ call fn00009560; r2 = memw(r5+24); r1 = memw(r5+16) }

l00011944:
	{ r4 = memd(r29+24); r5 = memd(r29+20) }
	{ r2 = memw(r4) }
	{ r3 = memw(r4+4); memb(r5+1) = r3.new }
	{ r2 = memw(r4+8) }
	{ memw(r5+8) = r2 }
	{ r7 = memw(r4+12) }
	{ memw(r5+12) = r7 }
	{ r2 = memw(r4+24) }
	{ r6 = memw(r5+20); if (cmp.gtu(r2,r6.new)) jump:t 00011980 }

l00011978:
	{ call fn00009560; r2 = memw(r4+24); r1 = memw(r4+16) }

l00011980:
	{ immext(#0x14B80); r4 = add(PC,#0x14B96); r3:r2 = combine(#0x2,r16); r1 = #0x165 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001199C:
	{ r17:r16 = memd(r29+128); r19:r18 = memd(r29+120) }
	{ r21:r20 = memd(r29+112); r23:r22 = memd(r29+104) }
	{ r25:r24 = memd(r29+96); r27:r26 = memd(r29+88) }
	{ dealloc_return }

;; avgpool_execute_slice_ref: 000119B0
avgpool_execute_slice_ref proc
	{ allocframe(#0xB8) }
	{ r2 = memw(r1); r3 = memw(r1+4) }
	{ memd(r29+168) = r19:r18; memd(r29+176) = r17:r16 }
	{ r7 = memw(r2+4); r19 = memb(r2+32) }
	{ r2 = memw(r2+8); memw(r29+24) = r3 }
	{ p0 = cmp.eq(r19,#0x0); r16 = memw(r7); r4 = memw(r7+16) }
	{ r3 = memw(r7+12); memd(r29+152) = r23:r22 }
	{ r5 = memw(r16+8) }
	{ memd(r29+160) = r21:r20; memd(r29+144) = r25:r24 }
	{ r0 = r5; r20 = r5; memd(r29+136) = r27:r26 }
	{ r1 = p0; r27 = memw(r2); memw(r29+4) = r1 }
	{ r17 = memw(r4+8); r21 = memw(r4+4) }
	{ if (!p0) r2 = add(r17,r20); r25 = memw(r16+12); r22 = memw(r16+4) }
	{ r23 = memw(r3+4); r24 = memw(r3+8) }
	{ r7 = memw(r16); memb(r29+5) = r7.new }
	{ memw(r29+116) = r1 }
	{ p0 = cmp.eq(r11,#0x2); if (p0.new) jump:nt 00011A38; if (!p0.new) r26 = #0x0; if (p0.new) r0 = sub(r2,r24) }

l00011A2C:
	{ p0 = cmp.eq(r11,#0x1); if (!p0.new) jump:nt 00011A44; r0 = r17 }

l00011A34:
	{ r0 += add(r20,#0xFFFFFFFF) }

l00011A38:
	{ call fn00009760; r1 = r17 }
	{ r26 = r0 }

l00011A44:
	{ p0 = cmp.eq(r11,#0x2); if (p0.new) jump:nt 00011A7C; if (p0.new) r1 = add(r21,#0x0) }

l00011A4C:
	{ p0 = cmp.eq(r11,#0x1); if (p0.new) jump:nt 00011A70; if (p0.new) r1 = add(r21,#0x0) }

l00011A54:
	{ r2 = #0x0; r0 = memw(r29+116); memb(r29+18) = r2.new }
	{ if (!p0.new) jump:nt 00011A90; if (!p0) r1:r0 = combine(r21,r22) }

l00011A6C:
	{ jump 00011A84 }

l00011A70:
	{ r0 = r1 }
	{ r0 += add(r22,#0xFFFFFFFF); jump 00011A84 }

l00011A7C:
	{ r2 = add(r1,r22) }
	{ r0 = sub(r2,r23) }

l00011A84:
	{ call fn00009760 }
	{ memw(r29+72) = r0 }
	{ r3 = sub(r23,r22); r2 = memw(r29+20); if (!cmp.gt(r2.new,#0x0)) jump:nt 00011D34 }

l00011A90:
	{ r2 = memw(r29+20); if (!cmp.gt(r2.new,#0x0)) jump:nt 00011D38 }

l00011A9C:
	{ r4 = add(r26,#0xFFFFFFFF); r5 = sub(r24,r20); r2 = memw(r29+72) }
	{ r4 = add(r5,mpyi(r4,r17)); r5 = mpyi(r25,r20); r2 = r2; r7 = memd(r29+24) }
	{ r2 = add(r3,mpyi(r2,r21)); r4 += lsr(r4,#0x1F); memw(r29+128) = r26; memw(r29+44) = r6 }
	{ r3 = mpyi(r7,r21); r2 += lsr(r2,#0x1F); r7 = sub(#0xFFFFFFFF,r22) }
	{ r6 = asl(r21,#0x1); r4 = asr(r4,#0x1); r7 = memw(r16+16); memw(r29+40) = r7 }
	{ r1 = asr(r2,#0x1); r7 = #0x0; r26 = memw(r27+16); memw(r29+108) = r7 }
	{ r5 = mpyi(r5,r22); memw(r29+112) = r5; memw(r29+52) = r22 }
	{ r6 = sub(#0xFFFFFFFF,r20); memw(r29+60) = r6; memw(r29+48) = r23 }
	{ r2 = add(r1,sub(#0x7F,r3)); r3 = sub(r3,r1); memw(r29+28) = r7; memw(r29+124) = r20 }
	{ r7 = add(r4,sub(#0x7F,r24)); r3 = sub(#0x0,r4); r2 = sub(r2,r23); memw(r29+12) = r3 }
	{ memw(r29+120) = r24; memw(r29+80) = r21 }
	{ memw(r29+96) = r6; memw(r29+16) = r5 }
	{ memw(r29+76) = r1; memw(r29+116) = r4 }
	{ memw(r29+36) = r7; memw(r29+32) = r3 }
	{ memw(r29+8) = r2 }

l00011B30:
	{ r2 = memw(r29+24) }
	{ r3 = memw(r29+72); if (!cmp.gt(r3.new,r2)) jump:t 00011D24 }

l00011B40:
	{ r7 = memd(r29+12); memw(r29+92) = r3 }
	{ r4 = memd(r29+112); r3 = memd(r29+72) }
	{ r2 = combine(r4.l,r2.l); r5 = memd(r29+28); memw(r29+88) = r7 }
	{ r1:r0 = combine(r4,r2); r7 = memd(r29+16); r6 = memd(r29+24) }
	{ r3 = mpyi(r5,r3); memb(r29+14) = r3.new }
	{ memd(r29+64) = r1:r0 }
	{ jump 00011B90; memw(r29+100) = r3 }

l00011B70:
	{ r6 = memd(r29+84); r5 = memd(r29+72) }
	{ p0 = cmp.gt(r5,r6); r2 = memd(r29+60); r3 = memd(r29+88) }
	{ r3 = add(r3,r2); r7 = memd(r29+92) }
	{ r4 = sub(r7,r2); memw(r29+88) = r3 }
	{ if (!p0) jump:nt 00011D24; memw(r29+92) = r4 }

l00011B90:
	{ r2 = r6 }
	{ r4 = add(r2,#0x2); r3 = memd(r29+80); r7 = memd(r29+76) }
	{ r3 = mpyi(r4,r3); r5 = memd(r29+108); memw(r29+84) = r4 }
	{ r3 = sub(r3,r7); r4 = memd(r29+112); r1:r0 = memd(r29+64) }
	{ r3 = add(r5,mpyi(r3,r4)) }
	{ l2fetch(r3,r1:r0) }
	{ r27 = #0x0; r3 = memw(r29+128); if (!cmp.gt(r3.new,#0x0)) jump:nt 00011B70 }

l00011BC4:
	{ r18 = #0x0; r3 = memd(r29+80); r4 = memd(r29+40) }
	{ r5 = memd(r29+92); r7 = memd(r29+56) }
	{ r3 = mpyi(r2,r3); r2 = add(r2,r7); r7 = memd(r29+88); r1 = memd(r29+48) }
	{ r4 = max(r5,r4); r5 = memd(r29+76); r16 = memd(r29+32) }
	{ r5 = max(r7,r6); r3 = sub(r3,r5); r4 = sub(#0xFFFFFFFF,r4); r7 = memw(r29+44) }
	{ r6 = max(r3,r6); r0 = add(r3,r1); r3 = memd(r29+52); r21 = memd(r29+36) }
	{ r23 = mpyi(r7,r2); r22 = sub(r4,r5); r7 = memw(r29+124) }
	{ r2 = min(r3,r0) }
	{ r1 = mpyi(r6,r7); r20 = sub(r2,r6); memb(r29+26) = r1.new }
	{ immext(#0x8000); r0 = #0x8000; r3 = memd(r29+116) }
	{ r7 = sub(r2,r3); r3 = memd(r29+120); r2 = memd(r29+124) }
	{ r19 = max(r7,r18); r3 = add(r7,r3) }
	{ r2 = min(r2,r3) }
	{ r24 = sub(r2,r19) }
	{ r1 = mpyi(r24,r20); call fn00009760 }
	{ r8 = mpyi(r24,r25); if (!p0.new) jump:nt 00011D0C; p0 = cmp.gt(r25,#0x0) }

l00011C54:
	{ r7 = max(r16,r18); r3 = mpyi(r27,r25); r2 = memd(r29+96); r4 = memd(r29+104) }
	{ r6 = add(r19,r4); r4 = #0x0; r1 = memd(r29+112) }
	{ r5 = max(r21,r2); r9 = mpyi(r6,r25); r2 = memw(r29+108) }
	{ r6 = sub(#0xFFFFFFFF,r5); r5 = sub(r1,r8); r8 = memw(r29+100) }
	{ r2 += add(r9,r8); r12 = sub(r6,r7); r6 = sub(r6,r7) }
	{ r7 = mpyi(r25,r12) }

l00011C90:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 00011CE4; r8 = #0x0 }

l00011C98:
	{ loop1(00011CA0,r22); r9:r8 = combine(r2,#0x0) }
	{ if (!p0.new) jump:nt 00011CD8; r14 = add(r6,#0xFFFFFFFF); r12 = add(r9,r4); p0 = cmp.gt(r24,#0x0) }

l00011CB0:
	{ loop0(00011CC4,r14); p0 = cmp.gtu(r6,#0x1) }
	{ if (!p0) jump:nt 00011CD0; r13 = add(r12,r25); r12 = memb(r12) }

l00011CC4:
	{ r8 = add(r12,r8); r13 = add(r13,r25); r12 = memb(r13) }

l00011CD0:
	{ r8 = add(r12,r8); r9 = add(r9,r7) }

l00011CD8:
	{ r9 = add(r9,r5); nop; nop }

l00011CE4:
	{ immext(#0x4000); r8 = add(#0x4000,mpyi(r8,r0)); r9 = add(r4,r3); r12 = r26 }
	{ r12 += add(r9,r23); r4 = add(r4,#0x1) }
	{ r8 = lsr(r8,#0xF); p0 = cmp.eq(r4,r25) }
	{ if (!p0) jump:nt 00011C90; memb(r12) = r8 }

l00011D0C:
	{ r16 = add(r16,r17); r21 = sub(r21,r17); r2 = memw(r29+128) }
	{ r27 = add(r27,#0x1); if (cmp.eq(r27.new,r2)) jump:nt 00011B70 }

l00011D24:
	{ r3 = memd(r29+28); r2 = memd(r29+20) }
	{ r3 = add(r3,#0x1) }
	{ p0 = cmp.eq(r3,r2); if (!p0.new) jump:nt 00011B30; memw(r29+28) = r3 }

l00011D34:
	{ r1 = #0x1; r2 = memd(r29+4); r17:r16 = memd(r29+176) }

l00011D38:
	{ r2 = memd(r29+4); r17:r16 = memd(r29+176) }

l00011D3C:
	{ r0 = add(r2,#0x8); r19:r18 = memd(r29+168); r21:r20 = memd(r29+160) }
	{ r23:r22 = memd(r29+152); r25:r24 = memd(r29+144) }
	{ jump 00009730; r27:r26 = memd(r29+136); deallocframe }

;; logmsg_function: 00011D58
;;   Called from:
;;     000115F0 (in avgpool_check)
;;     0001164C (in avgpool_check)
;;     000116B0 (in avgpool_check)
;;     0001170C (in avgpool_check)
;;     00011854 (in avgpool_execute)
;;     00011990 (in avgpool_execute)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00011D78 }

l00011D68:
	{ r0 = add(PC,#0x3); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l00011D78:
	{ dealloc_return }

;; errlog_function: 00011D7C
;;   Called from:
;;     00011610 (in avgpool_check)
;;     00011888 (in avgpool_execute)
errlog_function proc
	{ immext(#0x14700); r0 = add(PC,#0x1472B); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; avgpool_execute_slice_asm: 00011DA0
avgpool_execute_slice_asm proc
	{ allocframe(#0x98) }
	{ r2 = memw(r1); r3 = memw(r1+4) }
	{ memd(r29+136) = r19:r18; memd(r29+112) = r25:r24 }
	{ r7 = memw(r2+4); r18 = memb(r2+32) }
	{ r2 = memw(r2+8); memd(r29+144) = r17:r16 }
	{ p0 = cmp.eq(r18,#0x0); r24 = memw(r7); r4 = memw(r7+16) }
	{ r3 = memw(r7+12); memw(r29+28) = r3 }
	{ r17 = memw(r24+8) }
	{ memd(r29+128) = r21:r20; memd(r29+104) = r27:r26 }
	{ r0 = r17; r16 = memw(r2); r7 = memw(r24) }
	{ r2 = memw(r4+4); r26 = memw(r4+8) }
	{ r20 = memw(r24+12); r19 = memw(r24+4) }
	{ r1 = p0; memw(r29+12) = r1; memw(r29+24) = r7 }
	{ r2 = memw(r3+4); memw(r29+80) = r2 }
	{ r7 = memw(r3+8) }
	{ if (!p0) r2 = add(r26,r17); memd(r29+120) = r23:r22; memw(r29+48) = r2 }
	{ memw(r29+96) = r7; memw(r29+92) = r1 }
	{ if (p0) jump:nt 00011E34 }

l00011E14:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 00011E30; if (!p0.new) r0 = add(r26,#0x0); if (p0.new) r3 = memw(r29+96) }

l00011E20:
	{ p0 = cmp.eq(r10,#0x1); if (!p0.new) jump:nt 00011E40; r21 = #0x0 }

l00011E28:
	{ r0 += add(r17,#0xFFFFFFFF); jump 00011E34 }

l00011E30:
	{ r0 = sub(r2,r3) }

l00011E34:
	{ call fn00009760; r1 = r26 }
	{ r21 = r0 }

l00011E40:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 00011E70; nop }

l00011E48:
	{ p0 = cmp.eq(r10,#0x1); if (p0.new) jump:nt 00011E64; if (p0.new) r1 = memw(r29+80) }

l00011E50:
	{ r0 = #0x0; r1 = memd(r29+92) }
	{ p0 = r1; if (!p0.new) jump:nt 00011E80 }

l00011E5C:
	{ r0 = r11; jump 00011E7C; r1 = memw(r29+80) }

l00011E64:
	{ r0 = r1 }
	{ r0 += add(r19,#0xFFFFFFFF); jump 00011E7C }

l00011E70:
	{ r1 = memd(r29+80); r3 = memd(r29+48) }
	{ r2 = add(r1,r19) }
	{ r0 = sub(r2,r3) }

l00011E7C:
	{ call fn00009760 }

l00011E80:
	{ r7 = mpyi(r20,r17); r6 = mpyi(r21,r20); r2 = memw(r29+24); if (!cmp.gt(r2.new,#0x0)) jump:nt 00012050 }

l00011E94:
	{ r4 = add(r0,#0xFFFFFFFF); r3 = memd(r29+96); r5 = memd(r29+48) }
	{ r3 = sub(r3,r17); r5 = sub(r5,r19); r8 = and(r20,#0x7F); r1 = memw(r29+80) }
	{ r2 = add(r3,mpyi(r2,r26)); r9 = #0x0; r3 = memw(r16+16); r16 = memw(r24+16) }
	{ r4 = add(r5,mpyi(r4,r1)); r5 = mpyi(r0,r21); memw(r29+76) = r21; memw(r29+64) = r7 }
	{ r2 += lsr(r2,#0x1F); r1 = asl(r6,#0x1); memb(r29+13) = r1.new }
	{ r4 += lsr(r4,#0x1F); r1 = memd(r29+28); memw(r29+44) = r19 }
	{ r5 = mpyi(r7,r19); r2 = asr(r2,#0x1); memw(r29+20) = r5; memw(r29+68) = r0 }
	{ r6 = add(r3,mpyi(r6,r1)); r3 = asr(r4,#0x1); r2 = sub(#0x0,r2); memw(r29+92) = r8 }
	{ memw(r29+32) = r9; memw(r29+16) = r5 }
	{ memw(r29+36) = r6; memw(r29+72) = r3 }
	{ memw(r29+40) = r2 }

l00011F0C:
	{ r2 = memw(r29+28) }
	{ r3 = memw(r29+68); if (!cmp.gt(r3.new,r2)) jump:t 00012034 }

l00011F1C:
	{ r2 = combine(r3.l,r2.l); r7 = memd(r29+32); r5 = memd(r29+16) }
	{ r22 = mpyi(r5,r7); r1:r0 = combine(r3,r2); r4 = memw(r29+36); memb(r29+22) = r4.new }
	{ jump 00011F54 }

l00011F3C:
	{ r6 = memd(r29+84); r7 = memd(r29+68) }
	{ p0 = cmp.gt(r7,r6); r2 = memd(r29+52); r3 = memd(r29+88) }
	{ r3 = add(r3,r2) }
	{ if (!p0) jump:nt 00012034; memw(r29+88) = r3 }

l00011F54:
	{ r2 = r6 }
	{ r4 = add(r2,#0x2); r3 = memd(r29+80); r7 = memd(r29+72) }
	{ r3 = mpyi(r4,r3); r1:r0 = memd(r29+56); memw(r29+84) = r4 }
	{ r3 = sub(r3,r7); r4 = memw(r29+64) }
	{ r3 = add(r16,mpyi(r3,r4)) }
	{ l2fetch(r3,r1:r0) }
	{ r3 = memw(r29+76); if (!cmp.gt(r3.new,#0x0)) jump:nt 00011F3C }

l00011F84:
	{ r4 = memd(r29+48); r19 = memd(r29+76) }
	{ r2 = mpyi(r2,r3); r3 = #0x0; r24 = memw(r29+88); r21 = memw(r29+40) }
	{ r2 = sub(r2,r7); r7 = memw(r29+44) }
	{ r3 = max(r2,r3); r2 = add(r2,r4) }
	{ r2 = min(r7,r2); r18 = mpyi(r3,r17) }
	{ r25 = sub(r2,r3) }
	{ r23 = r26; r3 = #0x0; r2 = memd(r29+96) }
	{ r27 = max(r21,r3); immext(#0x8000); r0 = #0x8000; r2 = add(r2,r21) }
	{ r2 = min(r17,r2) }
	{ r26 = sub(r2,r27) }
	{ r1 = mpyi(r26,r25); call fn00009760 }
	{ r5:r4 = combine(r17,r25); r2 = add(r27,r18); r1 = r16 }
	{ r2 = mpyi(r2,r20) }
	{ r1 += add(r2,r22); r3 = memw(r29+92); if (!cmp.eq(r3.new,#0x0)) jump:t 0001200C }

l00011FF8:
	{ r3:r2 = combine(r26,r20); r0 = r24; memw(r29) = r0 }
	{ call avgpool_aligned_hvx }
	{ jump 0001201C }

l0001200C:
	{ r3:r2 = combine(r26,r20); r0 = r24; memw(r29) = r0 }
	{ call avgpool_nonaligned_hvx }

l0001201C:
	{ r26 = r23; r24 = add(r24,r20) }
	{ r21 = add(r21,r26); r19 = add(r19,#0xFFFFFFFF); if (cmp.eq(r19.new,#0x0)) jump:nt 00011F3C }

l00012034:
	{ r3 = memd(r29+32); r7 = memd(r29+24) }
	{ r3 = add(r3,#0x1); r2 = memd(r29+36); r4 = memd(r29+20) }
	{ p0 = cmp.eq(r3,r7); r2 = add(r2,r4); memw(r29+32) = r3 }
	{ if (!p0) jump:nt 00011F0C; memw(r29+36) = r2 }

l00012050:
	{ r1 = #0x1; r2 = memd(r29+12); r17:r16 = memd(r29+144) }
	{ r0 = add(r2,#0x8); r19:r18 = memd(r29+136); r21:r20 = memd(r29+128) }
	{ r23:r22 = memd(r29+120); r25:r24 = memd(r29+112) }
	{ jump 00009730; r27:r26 = memd(r29+104); deallocframe }
00012074             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; biasadd_8p8to32_execute: 00012080
biasadd_8p8to32_execute proc
	{ allocframe(#0x90) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+136) = r17:r16; memw(r29+92) = r1 }
	{ r5 = memw(r2+12); r4 = memw(r2+8) }
	{ r8 = memw(r2+16); r6 = memw(r2) }
	{ r1 = memw(r2+20); r17 = memw(r2+4) }
	{ r2 = memw(r4+16); r5 = memw(r5+16) }
	{ memd(r29+120) = r21:r20; memd(r29+96) = r27:r26 }
	{ r21 = memw(r2); r27 = memw(r5) }
	{ r0 = memw(r1+16); memw(r29+76) = r0 }
	{ memd(r29+128) = r19:r18 }
	{ r23 = sfsub(r27,r21); immext(#0x437F0000); r18 = #0x437F0000; memd(r29+112) = r23:r22 }
	{ r7 = memw(r8+16); r2 = memw(r3+8) }
	{ r1:r0 = combine(r18,r23); r19 = memw(r3); r22 = memw(r0) }
	{ r5 = memw(r3+4); memw(r29+72) = r2 }
	{ r2 = memw(r6+8) }
	{ memd(r29+104) = r25:r24; memw(r29+68) = r5 }
	{ r5 = memw(r6+4); memw(r29+84) = r2 }
	{ r2 = memw(r17+16); r26 = memw(r6+16) }
	{ r24 = memw(r6+12); r20 = memw(r6) }
	{ memw(r29+80) = r5; memw(r29+88) = r2 }
	{ r25 = memw(r19+16) }
	{ call fn00009610; r16 = memw(r7); memw(r29+64) = r22 }
	{ r22 = sfsub(r22,r16); r1 = r18; r18 = r0 }
	{ call fn00009610; r0 = r22 }
	{ immext(#0x14580); r3 = add(PC,#0x145AC); r1 = #0x66 }
	{ r2 = memw(r17+4); if (!cmp.eq(r2.new,#0x1)) jump:t 000121C4 }

l00012130:
	{ r3 = add(PC,#0x18); r1 = #0x67 }
	{ r2 = memw(r17); if (!cmp.eq(r2.new,#0x1)) jump:t 000121C4 }

l00012144:
	{ r3 = add(PC,#0x4); r1 = #0x68 }
	{ r2 = memw(r17+8); if (!cmp.eq(r2.new,#0x1)) jump:t 000121C4 }

l00012158:
	{ r4 = memw(r17+12); memw(r29+60) = r0 }
	{ if (p0.new) jump:nt 00012180; p0 = cmp.eq(r4,r24); if (!p0.new) r2 = memw(r29+92); if (p0.new) memw(r29+4) = r24 }

l0001216C:
	{ immext(#0x14540); r3 = add(PC,#0x14563); r1 = #0x6A; memw(r29) = r4 }
	{ jump 000121C8 }

l00012180:
	{ immext(#0x14540); r4 = add(PC,#0x14567); r2 = memd(r29+80); r3 = memd(r29+84) }
	{ r2 = mpyi(r20,r2) }
	{ r5 = mpyi(r2,r3); r2 = memw(r29+76) }
	{ r5 = mpyi(r5,r24); r3 = #0x2; r2 = memd(r29+92); memw(r29) = r2 }
	{ r17 = asl(r5,#0x2); call logmsg_function }
	{ immext(#0x14540); r3 = add(PC,#0x14555); r0 = clrbit(r21,#0x1E); r1 = #0x6E }
	{ r2 = memw(r19+20); if (!cmp.gtu(r17,r2.new)) jump:t 000121D4 }

l000121C4:
	{ r2 = memw(r29+92) }

l000121C8:
	{ call errlog_function }
	{ jump 00012470; r0 = #0xFFFFFFFF }

l000121D4:
	{ r1 = r27; r3 = memd(r29+80); r7 = memd(r29+84) }
	{ memw(r29+36) = r1; memw(r29+48) = r19 }
	{ memw(r19+24) = r17 }
	{ memw(r29+52) = r20 }
	{ memw(r19) = r20; memw(r19+4) = r3 }
	{ memw(r19+8) = r7; memw(r29+56) = r24 }
	{ call fn00009600; memw(r19+12) = r24 }
	{ r2 = clrbit(r16,#0x1E); r19 = r0; r20 = memd(r29+64) }
	{ call fn00009600; r1:r0 = combine(r20,r2) }
	{ call fn00009600; r1:r0 = combine(r0,r19) }
	{ immext(#0x48000000); r3 = #0x48000000; immext(#0xC8000000); r5 = #0xC8000000 }
	{ immext(#0x38D1B700); r2 = #0x38D1B717; immext(#0x2F800000); r4 = #0x2F800000 }
	{ r3 = sfmpy(r0,r3); r5 = sfmpy(r0,r5); r1:r0 = combine(r23,r2); memb(r29+10) = r5.new }
	{ memw(r29+32) = r3 }
	{ r19 = sfmpy(r2,r4); call fn00009600 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r17 = #0x0; memw(r29+44) = r17 }
	{ r2 = sfsub(r17,r21) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; r27 = r18; immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ p1 = cmp.gt(r2,#0xFFFFFFFF); r23 = #0x0; r1 = r22 }
	{ p0 = cmp.gt(r2,#0xFF); if (p0.new) r18 = #0xFF; if (!p0.new) r18 = zxtb(r2) }
	{ call fn00009600; if (!p1) r18 = add(r23,#0x0) }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = sfsub(r17,r16) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; r1:r0 = combine(r19,r27) }
	{ r17 = max(r2,r23); call fn00009610 }
	{ r24 = r19; r22 = r0; r1 = r19; r19 = memd(r29+60) }
	{ call fn00009610; r0 = r19 }
	{ r7:r6 = convert_sf2df(r20); r9:r8 = convert_sf2df(r16); r2 = memd(r29+36); r16 = memd(r29+92) }
	{ immext(#0x14400); r4 = add(PC,#0x14423); r15:r14 = convert_sf2df(r21); r1 = #0x8C }
	{ r13:r12 = convert_sf2df(r2); r3:r2 = combine(#0x9,r16); r20 = r0; memd(r29+24) = r7:r6 }
	{ memd(r29+16) = r9:r8; memd(r29+8) = r13:r12 }
	{ call logmsg_function; memd(r29) = r15:r14 }
	{ r7:r6 = convert_sf2df(r24); r9:r8 = convert_sf2df(r19); r3:r2 = combine(#0x9,r16); r1 = #0x8D }
	{ immext(#0x14400); r4 = add(PC,#0x14415); r13:r12 = convert_sf2df(r27); memd(r29+16) = r7:r6 }
	{ memd(r29+8) = r9:r8; memd(r29) = r13:r12 }
	{ call logmsg_function }
	{ r7:r6 = convert_sf2df(r20); r9:r8 = convert_sf2df(r22); r12 = memw(r29+68) }
	{ immext(#0x14400); r4 = add(PC,#0x14417); r5 = memd(r29+72); r3 = memd(r29+40) }
	{ r1 = #0x99; r0 = memw(r29+48) }
	{ r2 = memw(r12+16); memw(r12+12) = #0xFFFFFF81 }
	{ memw(r12+8) = #0x1; memw(r12+4) = #0xFFFFFF81 }
	{ memw(r12) = #0x1; memw(r2) = r3 }
	{ r2 = memw(r5+16); r13 = memw(r29+32) }
	{ memw(r5+4) = #0x1; memw(r5+8) = #0x1 }
	{ memw(r5) = #0x1; memw(r5+12) = #0x1 }
	{ r3:r2 = combine(#0x9,r16); r21 = memw(r29+52); memw(r2) = r13 }
	{ r19 = memw(r29+84); r27 = memw(r29+80) }
	{ r13 = memw(r29+44); r24 = memw(r29+56) }
	{ memw(r0) = r21; memw(r0+8) = r19 }
	{ memw(r0+4) = r27; memw(r0+24) = r13 }
	{ memw(r0+12) = r24; memw(r12+24) = #0x4 }
	{ memw(r5+24) = #0x4; memd(r29+24) = r7:r6 }
	{ memw(r29+16) = r17; memd(r29+8) = r9:r8 }
	{ call logmsg_function; memw(r29) = r18 }
	{ r2 = mpyi(r27,r21); r16 = r24 }
	{ r2 = mpyi(r2,r19) }
	{ p0 = cmp.eq(r2,#0x0); memw(r29+84) = r2 }
	{ if (p0) jump:nt 0001244C; nop }

l000123EC:
	{ r21 = r26; r27 = r16; r24 = r25; r19 = memw(r29+88) }
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 00012438 }

l00012400:
	{ r2 = memb(r19++#1); r3 = memb(r21++#1) }
	{ r2 = sub(r2,r17); r3 = sub(r3,r18) }
	{ r2 = convert_w2sf(r2); r3 = convert_w2sf(r3) }
	{ r0 = sfmpy(r20,r2) }
	{ r0 += sfmpy(r22,r3); call fn00009620; r27 = add(r27,#0xFFFFFFFF) }
	{ r2 = convert_uw2sf(r0):chop; if (!p0.new) jump:nt 00012400; p0 = cmp.eq(r27,#0x0); memw(r24++#4) = r2.new }

l00012438:
	{ r25 = addasl(r25,r16,#0x2); r26 = add(r26,r16); r2 = memw(r29+84) }

l0001243C:
	{ r26 = add(r26,r16); r2 = memw(r29+84) }

l00012444:
	{ r23 = add(r23,#0x1); if (!cmp.eq(r23.new,r2)) jump:t 000123EC }

l0001244C:
	{ immext(#0x14340); r4 = add(PC,#0x1434A); r2 = memw(r29+76); memb(r29) = r2.new }

l00012450:
	{ r4 = add(PC,#0xA); r2 = memw(r29+76); memb(r29) = r2.new }

l00012460:
	{ r1 = #0xAC }
	{ call logmsg_function; r2 = memw(r29+92) }
	{ r0 = #0x0 }

l00012470:
	{ r17:r16 = memd(r29+136); r19:r18 = memd(r29+128) }
	{ r21:r20 = memd(r29+120); r23:r22 = memd(r29+112) }
	{ r25:r24 = memd(r29+104); r27:r26 = memd(r29+96) }
	{ dealloc_return }

;; biasadd_check: 00012484
biasadd_check proc
	{ immext(#0x14180); r4 = add(PC,#0x141B5); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xB3; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x6)) jump:t 000124C8 }

l000124B0:
	{ r3 = add(PC,#0x26); r1 = #0xB4 }

l000124B8:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l000124C8:
	{ r2 = memw(r17+20) }
	{ p0 = cmp.eq(r2,#0x3); if (!p0.new) jump:nt 000124FC; if (!p0.new) r1 = #0xB5 }

l000124D4:
	{ loop0(000124E0,#0x6); r4 = #0x0; r2 = memw(r17+4) }
	{ r3 = r2 }
	{ r5 = memw(r3); if (!cmp.eq(r5.new,#0x0)) jump:t 00012508 }

l000124EC:
	{ r3 = add(PC,#0x16); r1 = #0xB8; memw(r29) = r4 }
	{ jump 000124B8 }

l000124FC:
	{ immext(#0x14140); r3 = add(PC,#0x1416D); jump 000124B8 }

l00012508:
	{ nop; r3 = add(r3,#0x4); r4 = r4 }
	{ r0 = #0x0; jump 00012520 }

l00012514:
	{ r2 = add(r2,#0x4); r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x5)) jump:nt 0001253C }

l00012518:
	{ r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x5)) jump:nt 00012540 }

l00012520:
	{ r3 = memw(r2); if (!cmp.eq(r3.new,#0x0)) jump:t 00012514 }

l00012524:
	{ if (!cmp.eq(r3.new,#0x0)) jump:t 00012518 }

l0001252C:
	{ r3 = add(PC,#0x2C); r1 = #0xBD; memw(r29) = r4 }
	{ jump 000124B8 }

l0001253C:
	{ immext(#0x14140); r4 = add(PC,#0x1416F); r3:r2 = combine(#0x2,r16); r1 = #0xC0 }

l00012540:
	{ r4 = add(PC,#0x2F); r3:r2 = combine(#0x2,r16); r1 = #0xC0 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

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
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001257C }

l0001256C:
	{ r0 = add(PC,#0x34); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0001257C:
	{ dealloc_return }

;; errlog_function: 00012580
;;   Called from:
;;     000121C8 (in biasadd_8p8to32_execute)
;;     000124B8 (in biasadd_check)
errlog_function proc
	{ immext(#0x14080); r0 = add(PC,#0x1409C); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
000125A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; check_execute: 000125B0
check_execute proc
	{ immext(#0x14240); r4 = add(PC,#0x1426C); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r17:r16 = combine(r0,r1); r1 = #0x36 }
	{ r2 = r16; r3 = memw(r17+4); memd(r29+8) = r19:r18 }
	{ r18 = memw(r3) }
	{ call logmsg_function; r19 = memw(r3+4); memw(r29) = r17 }
	{ immext(#0x14240); r3 = add(PC,#0x1425C); r1 = #0x38; r4 = memw(r18) }
	{ r2 = memw(r19); if (!cmp.eq(r2.new,r4)) jump:t 0001263C }

l000125F0:
	{ r3 = add(PC,#0x2B); r1 = #0x39; r4 = memw(r18+4) }
	{ r2 = memw(r19+4); if (!cmp.eq(r2.new,r4)) jump:t 0001263C }

l00012604:
	{ r3 = add(PC,#0x39); r1 = #0x3A; r4 = memw(r18+8) }
	{ r2 = memw(r19+8); if (!cmp.eq(r2.new,r4)) jump:t 0001263C }

l00012618:
	{ r3 = add(PC,#0x6); r1 = #0x3B; r4 = memw(r18+12) }
	{ r2 = memw(r19+12); if (!cmp.eq(r2.new,r4)) jump:t 0001263C }

l0001262C:
	{ r3 = add(PC,#0x13); r1 = #0x3C; r4 = memw(r18+24) }
	{ r2 = memw(r19+24); if (cmp.eq(r2.new,r4)) jump:t 00012644 }

l0001263C:
	{ r2 = r8; jump 00012664; memw(r29+4) = r2; memw(r29) = r4 }

l00012640:
	{ memw(r29+4) = r2; memw(r29) = r4 }

l00012644:
	{ call fn000097A0; r2 = r4; r0 = memw(r18+16); r1 = memw(r19+16) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00012670; if (p0.new) memw(r29) = r17 }

l00012658:
	{ immext(#0x14280); r3 = add(PC,#0x14282); r1 = #0x3E; r2 = r16 }

l00012664:
	{ call errlog_function }
	{ jump 00012688; r0 = #0xFFFFFFFF }

l00012670:
	{ immext(#0x14240); r4 = add(PC,#0x14278); r1 = #0x40; r2 = r16 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00012688:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }
	{ dealloc_return }

;; check_check: 00012690
check_check proc
	{ immext(#0x14100); r4 = add(PC,#0x14131); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x46; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x47; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 000126D0 }

l000126BC:
	{ r3 = add(PC,#0x20) }
	{ call errlog_function; r2 = r16 }

l000126C4:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l000126D0:
	{ r1 = #0x48; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 000126E8 }

l000126E0:
	{ r3 = add(PC,#0x12); jump 000126C4 }

l000126E8:
	{ immext(#0x14100); r4 = add(PC,#0x1411D); r1 = #0x49; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00012708
;;   Called from:
;;     000125D0 (in check_execute)
;;     00012680 (in check_execute)
;;     000126A4 (in check_check)
;;     000126F8 (in check_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001272C }

l00012718:
	{ r0 = add(PC,#0x12); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001272C:
	{ dealloc_return }

;; errlog_function: 00012730
;;   Called from:
;;     00012664 (in check_execute)
;;     000126C0 (in check_check)
;;     00012720 (in logmsg_function)
errlog_function proc
	{ immext(#0x14040); r0 = add(PC,#0x14076); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00012754             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; close_execute_f: 00012760
close_execute_f proc
	{ r4 = #0x3; r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x78) }
	{ r2 = memw(r17+4); memd(r29+104) = r19:r18 }
	{ r3 = memw(r17+16) }
	{ p0 = cmp.gtu(r4,r3); memd(r29+96) = r21:r20; memd(r29+88) = r23:r22 }
	{ r18 = memw(r2+4); r19 = memw(r2) }
	{ immext(#0x3D4CCCC0); if (p0) r24 = #0x3D4CCCCD; memd(r29+80) = r25:r24; memd(r29+72) = r27:r26 }
	{ r5 = memw(r18+24); r22 = memw(r18+16) }
	{ if (!p0) r2 = memw(r2+8) }
	{ if (!p0) r2 = memw(r2+16); r20 = memw(r19+16) }
	{ r21 = lsr(r5,#0x2); if (p0) jump:nt 000127AC }

l000127A8:
	{ r24 = memw(r2) }

l000127AC:
	{ immext(#0x14340); r4 = add(PC,#0x14377); r3:r2 = combine(#0x2,r16); r1 = #0x61 }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0x141C0); r3 = add(PC,#0x141C9); r1 = #0x64; r4 = memw(r19) }
	{ r2 = memw(r18); if (!cmp.eq(r2.new,r4)) jump:t 00012868 }

l000127E0:
	{ r3 = add(PC,#0x15); r1 = #0x65; r4 = memw(r19+4) }
	{ r2 = memw(r18+4); if (!cmp.eq(r2.new,r4)) jump:t 00012868 }

l000127F8:
	{ r3 = add(PC,#0x20); r1 = #0x66; r4 = memw(r19+8) }
	{ r2 = memw(r18+8); if (!cmp.eq(r2.new,r4)) jump:t 00012868 }

l00012810:
	{ r3 = add(PC,#0x2A); r1 = #0x67; r4 = memw(r19+12) }
	{ r2 = memw(r18+12); if (!cmp.eq(r2.new,r4)) jump:t 00012868 }

l00012828:
	{ r3 = add(PC,#0x34); r1 = #0x68; r4 = memw(r19+24) }
	{ p0 = cmp.eq(r21,#0x0); r25 = #0x0; r2 = memw(r18+24); if (!cmp.eq(r2.new,r4)) jump:t 00012868 }

l00012848:
	{ r26 = #0x0; if (!p0) r27 = add(r22,#0x0); if (!p0) memw(r29+68) = r26.new }
	{ if (p0.new) r19:r18 = combine(r26,r26); if (p0) r19:r18 = combine(r26,r26) }
	{ jump 000128C0; memw(r29+64) = r22; memw(r29+68) = r20 }

l00012868:
	{ r2 = r16; memw(r29+4) = r2; memw(r29) = r4 }

l00012870:
	{ call errlog_function }
	{ jump 00012940; r0 = #0xFFFFFFFF }
0001287C                                     0E 29 C7 7D             .).}
00012880 14 C0 9B 91 BE 76 FE 5B 00 D4 13 F5 92 77 FE 5B .....v.[.....w.[
00012890 13 40 60 70 00 D4 12 F5 9B 40 1B B0 96 40 16 B0 .@`p.....@...@..
000128A0 E2 00 0A 50 22 D4 02 EB 22 DF C2 8C 1A 5A 82 EB ...P"..."....Z..
000128B0 80 5A E2 C7 19 E0 17 74 37 40 17 B0 E4 F5 B2 21 .Z.....t7@.....!

l000128C0:
	{ r2 = sfsub(r19,r18) }
	{ r3 = sfmpy(r24,r2) }
	{ p0 = sfcmp.gt(r26,r3); if (!p0.new) jump:nt 0001292C; if (!p0.new) r1 = #0x7C; if (p0.new) memw(r29) = r17 }

l000128D8:
	{ r7:r6 = convert_sf2df(r3); r9:r8 = convert_sf2df(r2); r4 = memd(r29+64); r5 = memd(r29+68) }
	{ immext(#0x14240); r3 = add(PC,#0x14263); r13:r12 = convert_sf2df(r26); r1 = #0x7A }
	{ r4 = addasl(r4,r25,#0x2); r5 = addasl(r5,r25,#0x2) }
	{ r4 = memw(r4) }
	{ r5:r4 = convert_sf2df(r4); r2 = memw(r5); memd(r29+56) = r7:r6 }
	{ memd(r29+48) = r9:r8; memd(r29+24) = r5:r4 }
	{ r7:r6 = convert_sf2df(r2); r2 = r16; memw(r29+4) = r21 }
	{ memd(r29+40) = r13:r12; memd(r29+32) = r5:r4 }
	{ memd(r29+16) = r7:r6; memd(r29+8) = r7:r6 }
	{ jump 00012870; memw(r29) = r25 }

l0001292C:
	{ immext(#0x14180); r4 = add(PC,#0x141B8); r3:r2 = combine(#0x2,r16) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00012940:
	{ r17:r16 = memd(r29+112); r19:r18 = memd(r29+104) }
	{ r21:r20 = memd(r29+96); r23:r22 = memd(r29+88) }
	{ r25:r24 = memd(r29+80); r27:r26 = memd(r29+72) }
	{ dealloc_return }

;; close_check_f: 00012954
close_check_f proc
	{ immext(#0x14180); r4 = add(PC,#0x141A1); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xF6; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ r1 = #0xF7; r2 = memw(r17+16); if (cmp.gtu(r2.new,#0x1)) jump:t 00012998 }

l00012984:
	{ r3 = add(PC,#0x2D) }
	{ call errlog_function; r2 = r16 }

l0001298C:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00012998:
	{ r1 = #0xF8; r3 = #0x4; if (cmp.gtu(r3.new,r2)) jump:t 000129B0 }

l000129A8:
	{ r3 = add(PC,#0x9); jump 0001298C }

l000129B0:
	{ r1 = #0xF9; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 000129C8 }

l000129C0:
	{ r3 = add(PC,#0x7); jump 0001298C }

l000129C8:
	{ immext(#0x14140); r4 = add(PC,#0x14144); r3:r2 = combine(#0x2,r16); r1 = #0xFA }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; close_execute_i32: 000129E8
close_execute_i32 proc
	{ immext(#0x580); r2 = add(PC,#0x59C); jump close_execute }

;; close_check: 000129F4
close_check proc
	{ r0 = #0x0; jump close_check__merged }
000129F8                         00 40 00 7F 00 C0 00 7F         .@......

;; close_check__merged: 00012A00
;;   Called from:
;;     000129F4 (in close_check)
;;     00012D48 (in close_check_q)
close_check__merged proc
	{ immext(#0x13F00); r4 = add(PC,#0x13F14); p0 = cmp.eq(r2,#0x1); allocframe(#0x20) }
	{ immext(#0x140C0); r7 = add(PC,#0x140E9); r5 = #0xED; r6 = #0x6 }
	{ r8 = #0x2; r16 = r1; memd(r29+24) = r17:r16; memd(r29+16) = r19:r18 }
	{ r3:r2 = combine(#0x2,r16); if (!p0) r4 = add(r7,#0x0); immext(#0x100); r1 = mux(p0,#0x100,r5) }
	{ r19 = p0; r17 = r0; memb(r29+2) = r19.new }
	{ r18 = mux(p0,r6,r8); memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,r18)) jump:t 00012A74 }

l00012A5C:
	{ r3 = add(PC,#0x15); r2 = r16; r0 = memd(r29+8) }
	{ p0 = r0; jump 00012A9C; if (p0.new) r1 = #0x101; if (!p0.new) r1 = #0xEE }

l00012A74:
	{ r0 = memw(r29+8) }
	{ p1 = r0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 00012AB4 }

l00012A88:
	{ if (p1) r1 = #0x102; if (!p1) r1 = #0xEF }
	{ immext(#0x13E80); r3 = add(PC,#0x13EB3) }
	{ r2 = r16 }

l00012A9C:
	{ call errlog_function }
	{ jump 00012AE8; r0 = #0xFFFFFFFF }
00012AA8                         FA 44 00 00 83 4D 49 6A         .D...MIj
00012AB0 F8 FF FF 59                                     ...Y            

l00012AB4:
	{ if (p1) jump:nt 00012ACC }

l00012AB8:
	{ immext(#0x14040); r4 = add(PC,#0x14054); r3 = #0x2; r1 = #0xF0 }
	{ jump 00012ADC }

l00012ACC:
	{ immext(#0x13E80); r4 = add(PC,#0x13E8E); r3 = #0x2; r1 = #0x103 }

l00012ADC:
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r0 = #0x0 }

l00012AE8:
	{ r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ dealloc_return }

;; close_execute_u8: 00012AF0
close_execute_u8 proc
	{ immext(#0x380); r2 = add(PC,#0x39C); jump close_execute }
00012AFC                                     00 C0 00 7F             ....

;; close_execute_q_u8: 00012B00
close_execute_q_u8 proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x70) }
	{ r2 = memw(r17+4); memd(r29+80) = r23:r22 }
	{ memd(r29+72) = r25:r24 }
	{ immext(#0x437F0000); r19 = #0x437F0000; memd(r29+96) = r19:r18 }
	{ r3 = memw(r2+4); memd(r29+88) = r21:r20 }
	{ r1 = r19; r4 = memw(r2+8); r5 = memw(r2+20) }
	{ r6 = memw(r2+16); r3 = memw(r3+16) }
	{ r4 = memw(r4+16); r5 = memw(r5+16) }
	{ r6 = memw(r6+16) }
	{ r24 = memw(r4); r22 = memw(r3) }
	{ r25 = memw(r5); r23 = memw(r6) }
	{ r0 = sfsub(r24,r22); r21 = memw(r2); memd(r29+64) = r27:r26 }
	{ r18 = sfsub(r25,r23); call fn00009610; r20 = memw(r2+12) }
	{ call fn00009610; r1:r0 = combine(r19,r18); memw(r29+60) = r0 }
	{ immext(#0x13E00); r4 = add(PC,#0x13E0F); r3:r2 = combine(#0x2,r16); r1 = #0xD0 }
	{ r26 = memw(r21+16); memw(r29+56) = r0 }
	{ r27 = memw(r20+16); r19 = memw(r20+24) }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0x13E00); r3 = add(PC,#0x13E01); r1 = #0xD1; r4 = memw(r21) }
	{ r2 = memw(r20); if (!cmp.eq(r2.new,r4)) jump:t 00012C20 }

l00012BA8:
	{ r3 = add(PC,#0xD); r1 = #0xD2; r4 = memw(r21+4) }
	{ r2 = memw(r20+4); if (!cmp.eq(r2.new,r4)) jump:t 00012C20 }

l00012BC0:
	{ r3 = add(PC,#0x18); r1 = #0xD3; r4 = memw(r21+8) }
	{ r2 = memw(r20+8); if (!cmp.eq(r2.new,r4)) jump:t 00012C20 }

l00012BD8:
	{ r3 = add(PC,#0x22); r1 = #0xD4; r4 = memw(r21+12) }
	{ r2 = memw(r20+12); if (!cmp.eq(r2.new,r4)) jump:t 00012C20 }

l00012BF0:
	{ r3 = add(PC,#0x2C); r1 = #0xD5; r4 = memw(r21+24) }
	{ p0 = cmp.gt(r19,#0x0); r2 = memw(r20+24); if (!cmp.eq(r2.new,r4)) jump:t 00012C20 }

l00012C0C:
	{ memw(r29+44) = r25; memw(r29+48) = r24 }
	{ if (!p0) jump:nt 00012D18; jump 00012C3C; memw(r29+52) = r16 }

l00012C20:
	{ r2 = r16; memw(r29+4) = r2; memw(r29) = r4 }

l00012C28:
	{ call errlog_function }
	{ jump 00012D34; r0 = #0xFFFFFFFF }

l00012C34:
	{ r19 = r25; if (!cmp.gt(r19.new,r21)) jump:nt 00012D18 }

l00012C3C:
	{ r25:r24 = combine(r19,r23); r20 = r22; r19 = memb(r26); r16 = memb(r27) }

l00012C40:
	{ r20 = r22; r19 = memb(r26); r16 = memb(r27) }
	{ r7 = convert_w2sf(r16); r1 = r18; r4 = memd(r29+56); r2 = memd(r29+60) }
	{ r24 += sfmpy(r4,r7); r3 = convert_w2sf(r19) }
	{ r20 += sfmpy(r2,r3) }
	{ r0 = sfsub(r20,r24); call fn00009610 }
	{ r4 = clrbit(r0,#0x1E); r26 = add(r26,#0x1); immext(#0x99999980); r2 = #0x9999999A }
	{ r1:r0 = convert_sf2df(r4); r27 = add(r27,#0x1); immext(#0x3FA99980); r3 = #0x3FA99999 }
	{ p0 = dfcmp.gt(r1:r0,r3:r2); if (!p0.new) jump:nt 00012C34; if (!p0.new) r21 = add(r21,#0x1); if (p0.new) memw(r29+32) = r19 }

l00012C9C:
	{ r7:r6 = convert_sf2df(r23); r13:r12 = convert_sf2df(r22); r2 = memd(r29+44); memw(r29+36) = r16 }
	{ r5:r4 = convert_sf2df(r2); r1 = #0xE1; r16 = memw(r29+52) }
	{ immext(#0x13D80); r4 = add(PC,#0x13D84); r2 = memd(r29+48); memd(r29+24) = r5:r4 }
	{ r9:r8 = convert_sf2df(r2); r3:r2 = combine(#0x1,r16); memd(r29+16) = r7:r6; memd(r29) = r13:r12 }
	{ memd(r29+8) = r9:r8 }
	{ call logmsg_function }
	{ r7:r6 = convert_sf2df(r24); r9:r8 = convert_sf2df(r20); r3:r2 = combine(#0x1,r16); r1 = #0xE2 }
	{ immext(#0x13D80); r4 = add(PC,#0x13D8C); memw(r29+4) = r25; memd(r29+16) = r7:r6 }
	{ memd(r29+8) = r9:r8; memw(r29) = r21 }
	{ call logmsg_function }
	{ immext(#0x13D80); r3 = add(PC,#0x13D88); r1 = #0xE3; r2 = r16 }
	{ jump 00012C28 }

l00012D18:
	{ immext(#0x13D80); r4 = add(PC,#0x13D82); r3 = #0x2; r1 = #0xE7 }
	{ call logmsg_function; r2 = memd(r29+52); memw(r29) = r17 }
	{ r0 = #0x0 }

l00012D34:
	{ r17:r16 = memd(r29+104); r19:r18 = memd(r29+96) }
	{ r21:r20 = memd(r29+88); r23:r22 = memd(r29+80) }
	{ r25:r24 = memd(r29+72); r27:r26 = memd(r29+64) }
	{ dealloc_return }

;; close_check_q: 00012D48
close_check_q proc
	{ r1 = #0x1; jump close_check__merged }

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
;;     00013030 (in check_i32vals)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00012D6C }

l00012D5C:
	{ r0 = add(PC,#0x21); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

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
	{ immext(#0x13B80); r0 = add(PC,#0x13B89); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; close_execute: 00012D94
;;   Called from:
;;     000129E8 (in close_execute_i32)
;;     00012AF0 (in close_execute_u8)
close_execute proc
	{ immext(#0x13D00); r4 = add(PC,#0x13D38); memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ r17:r16 = combine(r0,r1) }
	{ r18 = r2; r1 = #0x3B; r5 = memw(r17+4); memd(r29+16) = r19:r18 }
	{ r3:r2 = combine(#0x2,r16) }
	{ r19 = memw(r5); memd(r29+8) = r21:r20 }
	{ r20 = memw(r5+4) }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0x13BC0); r3 = add(PC,#0x13BC9); r1 = #0x3D; r4 = memw(r19) }
	{ r2 = memw(r20); if (!cmp.eq(r2.new,r4)) jump:t 00012E30 }

l00012DDC:
	{ r3 = add(PC,#0x19); r1 = #0x3E; r4 = memw(r19+4) }
	{ r2 = memw(r20+4); if (!cmp.eq(r2.new,r4)) jump:t 00012E30 }

l00012DF0:
	{ r3 = add(PC,#0x28); r1 = #0x3F; r4 = memw(r19+8) }
	{ r2 = memw(r20+8); if (!cmp.eq(r2.new,r4)) jump:t 00012E30 }

l00012E04:
	{ r3 = add(PC,#0x36); r1 = #0x40; r4 = memw(r19+12) }
	{ r2 = memw(r20+12); if (!cmp.eq(r2.new,r4)) jump:t 00012E30 }

l00012E1C:
	{ r3 = add(PC,#0x0); r1 = #0x41; r4 = memw(r19+24) }
	{ r2 = memw(r20+24); if (cmp.eq(r2.new,r4)) jump:t 00012E38 }

l00012E30:
	{ r2 = r8; jump 00012E64; memw(r29+4) = r2; memw(r29) = r4 }

l00012E34:
	{ memw(r29+4) = r2; memw(r29) = r4 }

l00012E38:
	{ r3 = r4; r0 = r16; r1 = memw(r19+16); r2 = memw(r20+16) }
	{ callr r18 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00012E70; r1 = #0x45; r3:r2 = combine(#0x2,r16) }

l00012E54:
	{ immext(#0x13C00); r3 = add(PC,#0x13C38); r1 = #0x43; r2 = r16 }

l00012E64:
	{ call errlog_function }
	{ jump 00012E84; r0 = #0xFFFFFFFF }

l00012E70:
	{ immext(#0x13C40); r4 = add(PC,#0x13C74); memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00012E84:
	{ r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); dealloc_return }

;; check_u8vals: 00012E8C
check_u8vals proc
	{ r17:r16 = combine(r0,r3); memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ p0 = cmp.gt(r16,#0x0); r19:r18 = combine(#0x0,#0x1); memd(r29+40) = r19:r18 }
	{ r20 = r1; memd(r29+32) = r21:r20 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r25:r24 }
	{ if (!p0) jump:nt 00012F70 }

l00012EAC:
	{ p1 = cmp.gtu(r16,#0x1); r3 = r19; r2 = r16 }
	{ if (p1) jump:nt 00012ED8; r5 = add(r2,#0xFFFFFFFF); r4 = memb(r3++#1) }

l00012EC0:
	{ r2 = r4 }

l00012EC4:
	{ r2 = maxu(r18,r2); if (!p0) jump:nt 00012F70; r22 = #0x0; r18 = #0x0 }

l00012ED0:
	{ r21 = convert_uw2sf(r2); jump 00012F14 }

l00012ED8:
	{ loop0(00012EE8,r5); p1 = cmp.gtu(r2,#0x1); r2 = memb(r3++#1) }
	{ if (!p1) jump:nt 00012EFC }

l00012EE8:
	{ r18 = maxu(r18,r4); r5 = r2; r2 = memb(r3++#1) }
	{ r4 = r5; nop }

l00012EFC:
	{ r18 = maxu(r18,r4); jump 00012EC4 }

l00012F04:
	{ r20 = add(r20,#0x1); r19 = add(r19,#0x1); r22 = add(r22,#0x1); if (!cmp.gtu(r16,r22.new)) jump:nt 00012F70 }

l00012F14:
	{ r1 = r21; r23 = memb(r20); r24 = memb(r19) }

l00012F18:
	{ r23 = memb(r20); r24 = memb(r19) }
	{ r2 = convert_uw2sf(r24); r3 = convert_uw2sf(r23) }
	{ r0 = sfsub(r3,r2); call fn00009610 }
	{ r4 = clrbit(r0,#0x1E); immext(#0x99999980); r2 = #0x9999999A }
	{ r1:r0 = convert_sf2df(r4); immext(#0x3FA99980); r3 = #0x3FA99999 }
	{ p0 = dfcmp.gt(r1:r0,r3:r2); if (!p0.new) jump:nt 00012F04; if (p0.new) r1 = #0xA2; if (p0.new) memw(r29+12) = r24 }

l00012F58:
	{ immext(#0x13B40); r4 = add(PC,#0x13B55); r3:r2 = combine(#0x0,r17) }
	{ memw(r29+8) = r23; memw(r29+4) = r16 }
	{ call logmsg_function; r18 = #0x1; memw(r29) = r22 }

l00012F70:
	{ r0 = r18 }
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); dealloc_return }

;; check_i32vals: 00012F84
check_i32vals proc
	{ r20 = lsr(r3,#0x2); memd(r29-32) = r21:r20; allocframe(#0x30) }
	{ r17:r16 = combine(r2,r0); memd(r29+40) = r17:r16; memd(r29+16) = r23:r22 }
	{ r0 = #0x0; p0 = cmp.eq(r20,#0x0); r18 = r1; memd(r29+32) = r19:r18 }
	{ if (p0) jump:nt 00013038; if (!p0) r2 = add(r17,#0x0) }

l00012FA8:
	{ loop0(00012FAC,r20) }
	{ r3 = memw(r2++#4) }
	{ r3 = abs(r3) }
	{ r0 = maxu(r0,r3); nop }
	{ if (!p0) jump:nt 00012FDC; jump 00013038; r0 = #-0x1; r21 = #-0x1 }

l00012FC8:
	{ r18 = add(r18,#0x4); r17 = add(r17,#0x4); r21 = add(r21,#0x1); if (cmp.gtu(r20,r21.new)) jump:t 00012FE0 }

l00012FDC:
	{ r19 = convert_uw2sf(r0) }
	{ r1 = r19; r22 = memw(r18); r23 = memw(r17) }
	{ r2 = convert_w2sf(r23); r3 = convert_w2sf(r22) }
	{ r0 = sfsub(r3,r2); call fn00009610 }
	{ r4 = clrbit(r0,#0x1E); immext(#0x99999980); r2 = #0x9999999A }
	{ r1:r0 = convert_sf2df(r4); immext(#0x3FA99980); r3 = #0x3FA99999 }
	{ p0 = dfcmp.gt(r1:r0,r3:r2); if (!p0.new) jump:nt 00012FC8; if (p0.new) r1 = #0x8F; if (p0.new) memw(r29) = r21 }

l00013020:
	{ immext(#0x13A80); r4 = add(PC,#0x13A8D); r3:r2 = combine(#0x0,#0x0); memw(r29+12) = r23 }
	{ memw(r29+8) = r22; memw(r29+4) = r20 }
	{ call logmsg_function }
	{ r0 = #0x1 }

l00013038:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ dealloc_return }
00013044             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; concat_execute_asm: 00013050
concat_execute_asm proc
	{ immext(#0x5C0); r2 = add(PC,#0x5EC); jump concat_execute }

;; concat_check: 0001305C
concat_check proc
	{ immext(#0x13BC0); r4 = add(PC,#0x13BF7); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r1 = #0x123; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x124; immext(#0xAAAAAA80); r3 = #0xAAAAAAAB; r2 = memw(r17+16) }
	{ r4 = add(r2,#0xFFFFFFFF) }
	{ r3 = mpyu(r4,r3); r5 = r4 }
	{ r3 = lsr(r3,#0x1) }
	{ r5 -= mpyi(r3,#0x3) }
	{ r3 = add(PC,#0xF); r5 = memw(r17+24); r2 = memw(r17+28) }
	{ r2 = r16; memw(r29+8) = r2; memw(r29+4) = r5 }
	{ jump 00013138; memw(r29) = r4 }
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
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+16); dealloc_return }
00013144             EE 44 00 00 84 40 49 6A A1 65 00 78     .D...@Ij.e.x
00013150 02 C0 70 70 4E 42 00 5A 00 D1 9D A1 00 C0 00 78 ..ppNB.Z.......x
00013160 40 1F 14 3E                                     @..>            

;; concat_execute_ref: 00013164
concat_execute_ref proc
	{ immext(#0x2C0); r2 = add(PC,#0x2D0); jump concat_execute }

;; concat_execute: 00013170
;;   Called from:
;;     00013050 (in concat_execute_asm)
;;     00013164 (in concat_execute_ref)
concat_execute proc
	{ immext(#0xAAAAAA80); r3 = #0xAAAAAAAB; memd(r29-40) = r23:r22; allocframe(#0xA8) }
	{ r23 = r0 }
	{ r4 = memw(r23+16); memd(r29+152) = r19:r18 }
	{ r18 = add(r4,#0xFFFFFFFF); r27 = memw(r23+4); memd(r29+120) = r27:r26 }
	{ r3 = mpyu(r18,r3); r17 = r1; r5 = memw(r23+8); memd(r29+160) = r17:r16 }
	{ r25 = r2; r0 = memw(r27+4); memd(r29+128) = r25:r24 }
	{ immext(#0x13A00); r4 = add(PC,#0x13A1C); r1 = #0xE5; r2 = r17 }
	{ r22 = lsr(r3,#0x1); r24 = memw(r5); r26 = memw(r5+4) }
	{ r6 = addasl(r27,r22,#0x2); r16 = r22; r3 = memw(r0+4); memb(r29+10) = r3.new }
	{ r3 = memw(r0) }
	{ r6 = memw(r6+4); memw(r29+36) = r3 }
	{ r7 = memw(r13+r27) }
	{ r7 = memw(r7+16); r5 = memw(r5+8) }
	{ r3 = memw(r6+16); memd(r29+144) = r21:r20 }
	{ r5 = memw(r0+8); memw(r29+28) = r5 }
	{ r19 = memw(r27) }
	{ r20 = memw(r7); memw(r29+44) = r5 }
	{ r21 = memw(r3) }
	{ call logmsg_function; memw(r29) = r23 }
	{ r2 = memw(r19+16) }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x3)) jump:t 00013300 }

l00013214:
	{ r24 = #0x0; memw(r29+24) = r24 }
	{ r2 = #0x3; if (!cmp.gtu(r2.new,r18)) jump:t 0001331C }

l00013228:
	{ immext(#0x0); r2 = #0x0; r6 = memw(r29+28); memw(r26+12) = #0xFFFFFF81 }
	{ r2 = sfmin(r2,r21); r3 = add(r29,#0x30); memw(r26+8) = #0x1; memw(r26+4) = #0xFFFFFF81 }
	{ r21 = add(r3,#0x14); memw(r26) = #0x1; memw(r6+8) = #0x1 }
	{ r1:r0 = combine(#0x0,r21); r16 = add(r29,#0x54); memw(r6) = #0x1; memw(r6+4) = #0x1 }
	{ memw(r6+12) = #0xFFFFFF81 }
	{ r4 = memw(r26+16) }
	{ r4 = memd(r29+36); memw(r4) = r2 }
	{ r7 = memw(r6+16) }
	{ memw(r7) = r20; memw(r26+24) = #0x4 }
	{ memw(r6+24) = #0x4; memw(r5) = r4 }
	{ r7 = memd(r29+40); r4 = memd(r29+44) }
	{ memw(r5+4) = r7; memw(r5+8) = r4 }
	{ memw(r5+24) = r18 }
	{ memw(r5+12) = r19; memw(r29+48) = r23 }
	{ memw(r29+84) = r23; memw(r29+64) = r19 }
	{ memw(r29+56) = r2; memw(r29+92) = r2 }
	{ memw(r29+60) = r20; memw(r29+96) = r20 }
	{ memw(r29+100) = r19; memw(r3+4) = #0x1 }
	{ call fn00009740; memw(r16+4) = #0xFFFFFF80 }
	{ call fn00009740; r1 = #0x0; r0 = add(r16,#0x14) }
	{ call nn_os_work_for_vector; r2 = add(r29,#0x30); r1:r0 = combine(r25,r17) }
	{ callr r25; r1 = add(r29,#0x54); r0 = r17 }
	{ call fn000096A0; r0 = r21 }
	{ immext(#0x13940); r4 = add(PC,#0x13974); r1 = #0x112; r2 = r17 }
	{ call logmsg_function; memw(r29) = r23 }

l000132E8:
	{ r0 = r24; r27:r26 = memd(r29+120) }
	{ r17:r16 = memd(r29+160); r19:r18 = memd(r29+152) }
	{ r21:r20 = memd(r29+144); r23:r22 = memd(r29+136) }
	{ r25:r24 = memd(r29+128) }
	{ dealloc_return }

l00013300:
	{ immext(#0x138C0); r3 = add(PC,#0x138DD); r1 = #0xE6 }
	{ r2 = r17 }

l00013310:
	{ call errlog_function; r24 = #0xFFFFFFFF }
	{ jump 000132E8 }

l0001331C:
	{ r19:r18 = combine(#0x0,#0x0); r24 = add(r27,#0x4); r2 = memd(r29+44); r3 = memd(r29+40) }
	{ r26 = add(r16,r27); r16 = r27; memw(r29+8) = r26; memw(r29+16) = r23 }
	{ r2 = mpyi(r3,r2); memw(r29+12) = r25; memw(r29+20) = r17 }
	{ r16 += add(r7,#0x4); r23 = #0x0; r3 = memd(r29+36) }
	{ r2 = mpyi(r2,r3) }
	{ memw(r29+32) = r2 }

l00013354:
	{ r25 = memw(r24); r3 = memw(r29+44) }
	{ r2 = memw(r25+8); if (cmp.eq(r2.new,r3)) jump:t 00013378 }

l00013368:
	{ r3 = add(PC,#0x4); r1 = #0xE9 }
	{ jump 00013310; r2 = memd(r29+20); memw(r29) = r23 }

l00013374:
	{ r2 = memd(r29+20); memw(r29) = r23 }

l00013378:
	{ r2 = memw(r25+4) }
	{ r3 = memw(r29+40); if (cmp.eq(r3.new,r2)) jump:t 00013394 }

l00013388:
	{ r3 = add(PC,#0x3D); jump 00013374; r1 = #0xEC }

l00013394:
	{ r2 = memw(r25) }
	{ r3 = memw(r29+36); if (cmp.eq(r3.new,r2)) jump:t 000133B0 }

l000133A4:
	{ r3 = add(PC,#0x3B); jump 00013374; r1 = #0xEF }

l000133B0:
	{ r0 = r21; r17 = add(r27,#0x8); r23 = add(r23,#0x1); r2 = memw(r16++#4) }
	{ r2 = memw(r2+16) }
	{ call fn000097B0; r1 = memw(r2) }
	{ r21 = r0; r0 = r20; r2 = memw(r26++#4) }
	{ r2 = memw(r2+16) }
	{ call fn00009600; r1 = memw(r2) }
	{ r27 = r24; r20 = r0; r2 = memw(r25+12); r3 = memw(r29+32) }
	{ r24 = r17; r19 = add(r2,r19); p0 = cmp.gtu(r22,r23); if (!p0.new) r26 = memw(r29+8) }
	{ r18 += mpyi(r3,r2); if (p0) jump:nt 00013354; if (!p0) r24 = #0x0; if (!p0) r23 = memw(r29+16) }

l00013414:
	{ r1 = #0xF6; r5 = memd(r29+24); r17 = memd(r29+20) }
	{ r25 = memw(r29+12) }
	{ r2 = memw(r5+20); if (!cmp.gtu(r18,r2.new)) jump:t 00013228 }

l0001342C:
	{ r3 = add(PC,#0xE); jump 00013310 }

;; concat_execute_slice_ref: 00013434
concat_execute_slice_ref proc
	{ immext(#0x437F0000); r0 = #0x437F0000; allocframe(#0x48) }
	{ r4 = memw(r1+8); r2 = memw(r1+12) }
	{ r16 = memw(r1); memd(r29+64) = r17:r16 }
	{ memd(r29+40) = r23:r22 }
	{ r3 = memw(r16+16); memw(r29+4) = r1 }
	{ r1 = sfsub(r2,r4); r22 = memw(r1+16); r23 = memw(r1+4) }
	{ r17 = add(r3,#0xFFFFFFFF); memd(r29+32) = r25:r24; memd(r29+56) = r19:r18 }
	{ memd(r29+48) = r21:r20; memd(r29+24) = r27:r26 }
	{ call fn00009610; r24 = memw(r16+4); memw(r29+20) = r4 }
	{ r2 = #0x3; memw(r29+16) = r0 }
	{ p0 = cmp.gtu(r2,r9); if (p0) jump:nt 000135CC; if (!p0.new) r26 = add(r24,#0x0) }

l00013480:
	{ immext(#0xAAAAAA80); r3 = #0xAAAAAAAB; r2 = memw(r16+8) }
	{ r3 = mpyu(r17,r3); r16 = #0x0; r2 = memw(r2) }
	{ r18 = lsr(r3,#0x1); r4 = setbit(r3,#0x0); memb(r29+2) = r4.new }
	{ r27 = memw(r2+16) }
	{ memw(r29+12) = r3 }

l000134A8:
	{ r2 = r16; r26 = add(r26,#0x4); r21 = memw(r26+4) }
	{ r17 = add(r21,#0xC); r16 = r2; r3 = r2 }
	{ p0 = cmp.eq(r3,r15); if (!p0.new) jump:t 000135BC }

l000134C0:
	{ immext(#0x437F0000); r1 = #0x437F0000; r3 = memd(r29+12); r4 = memd(r29+8) }
	{ r2 = add(r4,r2); r4 = #0x0; r3 = add(r3,r2) }
	{ r3 = memw(r6+r3<<#2); r2 = memw(r14+r2<<#2) }
	{ r3 = memw(r3+16); r2 = memw(r2+16) }
	{ r3 = memw(r3) }
	{ r25 = sfmin(r4,r3); r2 = memw(r2) }
	{ r0 = sfsub(r2,r25); call fn00009610 }
	{ r20 = r0; r17 = add(r21,#0xC); r2 = memw(r21+8); r3 = memw(r21+4) }
	{ r5 = memw(r21); r4 = memw(r21+12) }
	{ r2 = mpyi(r3,r2); r3 = memw(r21+16) }
	{ r19 = mpyi(r2,r5) }
	{ r7 = combine(r4.l,r19.l) }
	{ r1:r0 = combine(r4,r7) }
	{ l2fetch(r3,r1:r0) }
	{ r1 = r20; r2 = memd(r29+20) }
	{ r0 = sfsub(r25,r2); call fn00009610 }
	{ immext(#0x41700000); r0 = #0x41700000; r2 = r0; r3 = memd(r29+16) }
	{ r25 = sfmpy(r3,r20); r20 = convert_uw2sf(r2):chop }
	{ call fn000097C0 }
	{ r2 = sfmpy(r25,r0); p0 = cmp.eq(r19,#0x0) }
	{ r5 = convert_uw2sf(r2):chop; if (p0) jump:nt 000135BC; if (!p0) r2 = add(r27,#0x0) }

l00013554:
	{ r4 = #0x7FFF; r7 = memw(r21+12); r3 = memw(r21+16) }
	{ loop0(00013568,r19); p0 = cmp.gt(r5,r4); if (!p0.new) r4 = sxth(r5) }
	{ p0 = cmp.eq(r7,#0x0); if (p0.new) jump:nt 000135B4; r7 = #0x0 }

l00013570:
	{ r6 = #0x0; r5 = r3 }

l00013574:
	{ r7 = #0xFF; r8 = memb(r5++#1) }
	{ r8 = add(r8,r20) }
	{ immext(#0x4000); r8 = add(#0x4000,mpyi(r8,r4)) }
	{ r8 = asr(r8,#0xF) }
	{ if (p0.new) jump:nt 000135A0; p0 = cmp.gt(r8,#0xFF); if (!p0.new) r7 = add(r8,#0x0) }

l00013598:
	{ p0 = cmp.gt(r8,#0xFFFFFFFF); if (!p0.new) r7 = #0x0 }

l000135A0:
	{ r8 = add(r2,r6); r6 = add(r6,#0x1) }
	{ memb(r8) = r7 }
	{ r7 = memw(r17); if (cmp.gtu(r7.new,r6)) jump:t 00013574 }

l000135B4:
	{ nop; r3 = add(r3,r7); r2 = add(r2,r22) }

l000135B8:
	{ r3 = add(r3,r7); r2 = add(r2,r22) }

l000135BC:
	{ p0 = cmp.gtu(r18,r16); r2 = memw(r17) }
	{ if (p0) jump:nt 000134A8; r27 = add(r27,r2) }

l000135CC:
	{ r1 = #0x1; r2 = memd(r29+4); r17:r16 = memd(r29+64) }
	{ r0 = add(r2,#0x14); r19:r18 = memd(r29+56); r21:r20 = memd(r29+48) }
	{ r23:r22 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ jump 00009730; r27:r26 = memd(r29+24); deallocframe }

;; logmsg_function: 000135F0
;;   Called from:
;;     00013070 (in concat_check)
;;     000131FC (in concat_execute)
;;     000132E0 (in concat_execute)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00013614 }

l00013600:
	{ r0 = add(PC,#0x2C); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00013614:
	{ dealloc_return }

;; errlog_function: 00013618
;;   Called from:
;;     00013138 (in concat_check)
;;     00013310 (in concat_execute)
;;     00013608 (in logmsg_function)
errlog_function proc
	{ immext(#0x13580); r0 = add(PC,#0x13590); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; concat_execute_slice_asm: 0001363C
concat_execute_slice_asm proc
	{ immext(#0x437F0000); r0 = #0x437F0000; allocframe(#0x48) }
	{ r4 = memw(r1+8); r2 = memw(r1+12) }
	{ r17 = memw(r1); memd(r29+64) = r17:r16 }
	{ r3 = memw(r1+16) }
	{ memd(r29+32) = r25:r24; memw(r29+16) = r3 }
	{ r1 = sfsub(r2,r4); r24 = memw(r1+4); memw(r29+8) = r1 }
	{ r3 = memw(r17+16) }
	{ r19 = add(r3,#0xFFFFFFFF); memd(r29+56) = r19:r18; memd(r29+48) = r21:r20 }
	{ memd(r29+40) = r23:r22; memd(r29+24) = r27:r26 }
	{ call fn00009610; r16 = memw(r17+4); memw(r29+20) = r4 }
	{ r2 = #0x3; memw(r29+12) = r0 }
	{ p0 = cmp.gtu(r2,r11); if (p0) jump:nt 00013770; if (!p0.new) r27 = #0x0; if (!p0.new) r25 = add(r16,#0x4) }

l00013690:
	{ immext(#0xAAAAAA80); r2 = #0xAAAAAAAB; r3 = memw(r17+8) }
	{ r2 = mpyu(r19,r2); r7 = memw(r3) }
	{ r19 = lsr(r2,#0x1); r20 = memw(r7+16) }
	{ r26 = asl(r19,#0x2); r16 = asl(r19,#0x3) }

l000136B0:
	{ r2 = and(r27,#0x1); r27 = add(r27,#0x1); r17 = memw(r25) }
	{ if (p0.new) jump:nt 000136D4; p0 = cmp.eq(r2,r24); if (p0.new) r3 = memw(r25+r16); if (p0.new) r2 = memw(r25+r26) }

l000136CC:
	{ jump 0001375C; r17 = add(r17,#0xC) }

l000136D4:
	{ r4 = #0x0; r21 = memw(r17+16) }
	{ immext(#0x437F0000); r1 = #0x437F0000; r2 = memw(r2+16); r3 = memw(r3+16) }
	{ r2 = memw(r2); r3 = memw(r3) }
	{ r23 = sfmin(r4,r2) }
	{ r0 = sfsub(r3,r23); call fn00009610 }
	{ r22 = r0; r2 = memw(r17+8); r3 = memw(r17+4) }
	{ r5 = memw(r17); r4 = memw(r17+12) }
	{ r2 = mpyi(r3,r2) }
	{ r18 = mpyi(r2,r5) }
	{ r7 = combine(r4.l,r18.l) }
	{ r1:r0 = combine(r4,r7) }
	{ l2fetch(r21,r1:r0) }
	{ r1 = r22; r2 = memd(r29+20) }
	{ r0 = sfsub(r23,r2); call fn00009610 }
	{ r6 = #0x7FFF; immext(#0x47000000); r3 = #0x47000000; r2 = memd(r29+12) }
	{ r7 = convert_uw2sf(r0):chop; r1:r0 = combine(r21,r20); r5 = memw(r29+16) }
	{ r2 = sfmpy(r2,r22) }
	{ r4 = sfmpy(r2,r3); r17 = add(r17,#0xC); r3 = sxth(r7); r2 = memw(r17+12) }
	{ r4 = convert_uw2sf(r4):chop; memw(r29) = r18 }
	{ r4 = min(r6,r4) }
	{ call memconvert_hvx; r4 = sxth(r4) }

l0001375C:
	{ r25 = add(r25,#0x4); p0 = cmp.gtu(r19,r27); r2 = memw(r17) }
	{ if (p0) jump:nt 000136B0; r20 = add(r20,r2) }

l00013770:
	{ r1 = #0x1; r2 = memd(r29+8); r17:r16 = memd(r29+64) }
	{ r0 = add(r2,#0x14); r19:r18 = memd(r29+56); r21:r20 = memd(r29+48) }
	{ r23:r22 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ jump 00009730; r27:r26 = memd(r29+24); deallocframe }
00013794             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; hexagon_nn_const_ctor: 000137A0
;;   Called from:
;;     0000B764 (in do_append_const_node)
hexagon_nn_const_ctor proc
	{ memw(r29-44) = r5; allocframe(#0x30) }
	{ r17:r16 = combine(r1,r0); r0 = add(r29,#0x0); memd(r29+40) = r17:r16 }
	{ memw(r29+8) = r4; memw(r29) = r2 }
	{ r4 = memd(r29+56); memd(r29+32) = r19:r18 }
	{ r3 = memd(r29+60); memw(r29+4) = r3 }
	{ memw(r29+16) = r4; memw(r29+24) = r3 }
	{ call tensor_dup; memw(r29+20) = r3 }
	{ r18 = r0; if (!cmp.eq(r18.new,#0x0)) jump:t 000137EC }

l000137D4:
	{ r3 = add(PC,#0x28); r1 = #0x54; r2 = r16 }
	{ call errlog_function; r17 = #0x0 }
	{ jump 00013828 }

l000137EC:
	{ call alloc_node; r1:r0 = combine(#0x3,r17); r2 = #0x0; r17 = #0x0 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 00013818; r2 = add(r18,#0x1C) }

l00013800:
	{ immext(#0x13500); r3 = add(PC,#0x1350B); r1 = #0x58; r2 = r16 }
	{ call errlog_function }
	{ jump 00013828 }

l00013818:
	{ r17 = r0; memw(r0+20) = #0x1; memw(r0+4) = #0x0 }
	{ memw(r0+16) = #0x0; memw(r0+12) = #0x0 }
	{ memw(r0+8) = r2 }

l00013828:
	{ r0 = r17; r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ dealloc_return }

;; errlog_function: 00013834
;;   Called from:
;;     000137E0 (in hexagon_nn_const_ctor)
;;     00013810 (in hexagon_nn_const_ctor)
;;     00013898 (in const_check)
errlog_function proc
	{ immext(#0x13480); r0 = add(PC,#0x134A9); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; const_execute: 00013858
const_execute proc
	{ r0 = #0x0; jumpr r31 }
0001385C                                     00 C0 00 7F             ....

;; const_check: 00013860
const_check proc
	{ immext(#0x134C0); r4 = add(PC,#0x134FE); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x36; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ r2 = memw(r17+4); if (cmp.eq(r2.new,#0x0)) jump:nt 000138A4 }

l0001388C:
	{ r4 = add(PC,#0x2D); r1 = #0x38; r3 = #0x0 }

l00013894:
	{ r2 = r16; memw(r29) = r4 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }
	{ r17:r16 = memd(r29+8); dealloc_return }

l000138A4:
	{ r2 = memw(r17+8); if (!cmp.eq(r2.new,#0x0)) jump:t 000138BC }

l000138B0:
	{ r4 = add(PC,#0x1E); r1 = #0x3B; r3 = #0x0 }
	{ jump 00013894 }

l000138BC:
	{ r1 = #0x40; r2 = memw(r2); if (!cmp.eq(r2.new,#0x0)) jump:t 000138D8 }

l000138CC:
	{ r4 = add(PC,#0x1D); r1 = #0x3E; r3 = #0x0 }
	{ jump 00013894 }

l000138D8:
	{ immext(#0x134C0); r4 = add(PC,#0x134E9); r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }
000138F4             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......

;; const_ctor: 00013900
const_ctor proc
	{ immext(#0x13400); r4 = add(PC,#0x1342E); r3:r2 = combine(#0x0,r0); r1 = #0x6D }
	{ call logmsg_function; allocframe(#0x0) }
	{ r0 = #0x0; dealloc_return }
0001391C                                     00 C0 00 7F             ....

;; const_dtor: 00013920
const_dtor proc
	{ immext(#0x133C0); r4 = add(PC,#0x133FB); r3:r2 = combine(#0x2,r1); allocframe(#0x10) }
	{ r1 = #0x73; r16 = r0; memd(r29+8) = r17:r16 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r2 = memw(r16+8) }
	{ call tensor_free; r0 = memw(r2) }
	{ call fn00009510; r0 = r16 }
	{ r0 = #0x0; r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001395C
;;   Called from:
;;     00013874 (in const_check)
;;     000138E8 (in const_check)
;;     00013910 (in const_ctor)
;;     00013938 (in const_dtor)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001397C }

l0001396C:
	{ r0 = add(PC,#0x35); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0001397C:
	{ dealloc_return }

;; conv2d_execute_hvx: 00013980
conv2d_execute_hvx proc
	{ r27 = r0; memd(r29-56) = r27:r26; allocframe(#0xB8) }
	{ r2 = memw(r27+4); memd(r29+160) = r21:r20 }
	{ r3 = memw(r27+8); memd(r29+152) = r23:r22 }
	{ r21 = memb(r27+32); r22 = memw(r2+4) }
	{ r20 = memw(r2); r5 = memw(r2+24) }
	{ p0 = cmp.eq(r21,#0x0); memd(r29+176) = r17:r16; memd(r29+168) = r19:r18 }
	{ r4 = memw(r3+8); memd(r29+144) = r25:r24 }
	{ r16 = memw(r3); r3 = memw(r3+4) }
	{ r19 = memw(r2+12); r23 = memw(r2+8) }
	{ r25 = memw(r2+20); r18 = memw(r2+16) }
	{ r2 = memw(r22+12); memw(r29+100) = r1 }
	{ r0 = memw(r20+8) }
	{ r7 = memw(r20+12); memw(r29+80) = r3 }
	{ r2 = memw(r22+8); memw(r29+116) = r2 }
	{ r17 = memw(r20+4); r3 = memw(r22+4) }
	{ r1 = memw(r5+8); r24 = memw(r5+4) }
	{ r6 = memw(r20); memw(r29+108) = r7 }
	{ r7 = memw(r22) }
	{ r2 = p0; memw(r29+92) = r2; memw(r29+88) = r5 }
	{ memw(r29+128) = r0; memw(r29+76) = r4 }
	{ memw(r29+104) = r6; memw(r29+124) = r7 }
	{ if (p0) jump:nt 00013A3C; memw(r29+84) = r2 }

l00013A08:
	{ p0 = cmp.eq(r13,#0x2); if (p0.new) jump:nt 00013A34; if (p0.new) r2 = memw(r29-128) }

l00013A10:
	{ r0 = #0x0; p0 = cmp.eq(r21,#0x1); memw(r29+112) = r1; memw(r29+120) = r3 }
	{ if (!p0) jump:nt 00013A44; if (p0) r2 = memw(r29-128); if (p0) r1 = memw(r29+112) }

l00013A28:
	{ r0 = r1; r3 = memd(r29+120) }
	{ r0 += add(r2,#0xFFFFFFFF); jump 00013A3C }

l00013A34:
	{ r2 = sub(r2,r3) }
	{ r0 = add(r2,r1) }

l00013A3C:
	{ call fn00009760; memw(r29+112) = r1; memw(r29+120) = r3 }

l00013A44:
	{ p0 = cmp.eq(r13,#0x2); if (p0.new) jump:nt 00013A80; if (p0.new) r1 = add(r24,#0x0); if (p0.new) r2 = memw(r29+124) }

l00013A50:
	{ p0 = cmp.eq(r13,#0x1); if (p0.new) jump:nt 00013A74; if (!p0.new) r26 = add(r24,#0x0); memw(r29+96) = r0 }

l00013A5C:
	{ r0 = #0x0; r1 = memd(r29+84) }
	{ p0 = r1; if (!p0.new) jump:nt 00013A94; if (!p0) r1:r0 = combine(r26,r17) }

l00013A6C:
	{ call fn00009760 }
	{ jump 00013A94 }

l00013A74:
	{ r1:r0 = combine(r24,r24) }
	{ r0 += add(r17,#0xFFFFFFFF); jump 00013A8C }

l00013A80:
	{ r2 = sub(r17,r2); memw(r29+96) = r0 }
	{ r0 = add(r2,r24) }

l00013A8C:
	{ call fn00009760; r26 = r24 }

l00013A94:
	{ immext(#0x437F0000); r19 = #0x437F0000; r2 = memw(r19+16); r3 = memw(r23+16) }
	{ r24 = r0; r6 = memw(r22+16); r4 = memw(r18+16) }
	{ r2 = memw(r2); r21 = memw(r3) }
	{ r5 = memw(r25+16); memw(r29+68) = r6 }
	{ r22 = sfsub(r2,r21); r7 = memw(r20+16); r6 = memw(r16+16) }
	{ r1:r0 = combine(r19,r22); memw(r29+48) = r26; memw(r29+84) = r7 }
	{ r18 = memw(r5); memw(r29+72) = r6 }
	{ call fn00009610; r25 = memw(r4) }
	{ r20 = sfsub(r18,r25); r23 = r0 }
	{ call fn00009610; r1:r0 = combine(r19,r20) }
	{ r4 = sfmpy(r23,r0); immext(#0xCF000000); r2 = #0xCF000000 }
	{ r2 = sfmpy(r4,r2); immext(#0x4F000000); r3 = #0x4F000000; r0 = r22 }
	{ r3 = sfmpy(r4,r3); memb(r29+15) = r3.new }
	{ memw(r29+56) = r2 }
	{ call fn00009610; r1:r0 = combine(r0,r19); immext(#0x0); r18 = #0x0 }
	{ r2 = sfsub(r18,r21) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = r0; r0 = r20 }
	{ r2 = convert_uw2sf(r2):chop; call fmaxf.1.0; memb(r29+13) = r2.new }
	{ r2 = #0x0 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = sfsub(r18,r25) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ immext(#0x13280); r4 = add(PC,#0x132A0); r22 = memw(r29+100); r5 = memw(r27+28) }
	{ r7 = convert_uw2sf(r0):chop; r1 = #0x207; r3:r2 = combine(#0x2,r22); memw(r29+4) = r5 }
	{ memw(r29+64) = r7; memw(r29) = r27 }
	{ call logmsg_function }
	{ immext(#0x13280); r4 = add(PC,#0x13292); r19 = memd(r29+108); r21 = memd(r29+104) }
	{ r3:r2 = combine(#0x2,r22); r5 = memw(r29+128); memw(r29+12) = r19 }
	{ r1 = #0x208; memw(r29+8) = r5; memw(r29+4) = r17 }
	{ call logmsg_function; memw(r29) = r21 }
	{ immext(#0x13280); r4 = add(PC,#0x13283); r25 = memw(r29+92); r5 = memw(r29+120) }
	{ r3:r2 = combine(#0x2,r22); memw(r29+12) = r25 }
	{ r1 = #0x209; r20 = memd(r29+124); memw(r29+8) = r5 }
	{ memw(r29+4) = r20 }
	{ r18 = memw(r29+116) }
	{ call logmsg_function; memw(r29) = r18 }
	{ immext(#0x13240); r4 = add(PC,#0x1326B); r1 = #0x20A; r5 = memw(r29+112) }
	{ r3:r2 = combine(#0x2,r22); memw(r29+4) = r5; memw(r29) = r26 }
	{ call logmsg_function }
	{ immext(#0x13240); r4 = add(PC,#0x1325F); r3:r2 = combine(#0x2,r22); r1 = #0x20B }
	{ call logmsg_function; r5 = memb(r27+32); memb(r29) = r5.new }
	{ r4 = add(PC,#0x15); memw(r29+12) = r18; memw(r29) = r21 }
	{ r3:r2 = combine(#0x2,r22); r23 = memw(r29+96); memw(r29+4) = r24 }
	{ r1 = #0x20C; memw(r29+8) = r23 }
	{ call logmsg_function }
	{ immext(#0x13240); r3 = add(PC,#0x13250); r1 = #0x20D; r9 = r17 }
	{ r2 = mpyi(r21,r18); r12 = r19; if (!cmp.eq(r12.new,r25)) jump:t 00013CEC }

l00013C58:
	{ r2 = mpyi(r2,r23) }
	{ r2 = mpyi(r2,r24) }
	{ r4 = asl(r2,#0x2); if (!cmp.gtu(r4.new,r5)) jump:t 00013C84 }

l00013C6C:
	{ r3 = add(PC,#0x21); r2 = memw(r27+28); memb(r29+2) = r2.new }
	{ r2 = r14; jump 00013CF0; memw(r29+4) = r4; memw(r29) = r5 }

l00013C84:
	{ r1 = #0x211; r19 = r23; r2 = memd(r29+88) }
	{ immext(#0x13200); r3 = add(PC,#0x1322B); r23 = r24 }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x1)) jump:t 00013CEC }

l00013CA4:
	{ r3 = add(PC,#0x28); r1 = #0x212; r2 = memw(r29+88) }
	{ r2 = memw(r2+12); if (!cmp.eq(r2.new,#0x1)) jump:t 00013CEC }

l00013CBC:
	{ r3 = add(PC,#0x21); r5 = #0x4; r2 = memd(r29+80) }
	{ r1 = #0x213 }
	{ r2 = memw(r2+20); if (cmp.gtu(r5,r2.new)) jump:t 00013CEC }

l00013CD4:
	{ r3 = add(PC,#0x17); r1 = #0x214; r2 = memw(r29+76) }
	{ r6 = add(#0x7,mpyi(r23,r19)); r2 = memw(r2+20); if (cmp.gtu(r2.new,#0x3)) jump:t 00013CFC }

l00013CEC:
	{ r2 = r22 }

l00013CF0:
	{ call errlog_function }
	{ jump 00014018; r0 = #0xFFFFFFFF }

l00013CFC:
	{ r1 = #0x20; r27 = and(r6,#0xFFFFFFF8); r2 = memd(r29+120); memw(r7+12) = r18 }
	{ r3 = mpyi(r2,r20); r15 = #0x0; memw(r7+8) = r19; memw(r7) = r21 }
	{ r5 = add(#0xF,mpyi(r3,r12)); r3 = mpyi(r3,r12); r17 = r22; memw(r7+4) = r23 }
	{ r6 = add(r18,#0x1F); r5 = and(r5,#0xFFFFFFF0); r7 = memd(r29+80); memw(r7+24) = r4 }
	{ r5 = max(r5,r1); r24 = and(r6,#0xFFFFFFE0); r8 = memw(r29+76); r2 = memw(r29+56) }
	{ r6 = asl(r24,#0x2); r13 = memw(r29+60) }
	{ r4 = memw(r7+16); memw(r7+4) = #0x1 }
	{ memw(r7+8) = #0x1; memw(r7+12) = #0x1 }
	{ r4 = asl(r27,#0x2); r0 = r5; memw(r7) = #0x1; memw(r4) = r2 }
	{ r28 = mpyi(r27,r0); r2 = mpyi(r0,r24); memw(r7+24) = #0x4; memw(r8+4) = #0xFFFFFF81 }
	{ r22 = r28; r7 = or(r2,#0x7F); r1 = memw(r8+16); memw(r8+8) = #0x1 }
	{ memw(r8+12) = #0xFFFFFF81; memw(r8) = #0x1 }
	{ r13 = #0xFF; memw(r1) = r13; memw(r8+24) = #0x4 }
	{ r5 = r7; r26 = memw(r17+4); memw(r29+32) = r5 }
	{ r8 = r26; r14 = memw(r29+64); memw(r29+60) = r3 }
	{ r2 = mpyi(r27,r24); r8 += add(r22,#0x7F); p0 = cmp.gt(r14,#0xFF); memw(r29+56) = r2 }
	{ r3 = asl(r2,#0x2); r8 = and(r8,#0xFFFFFF80); p1 = cmp.gt(r14,#0xFFFFFFFF); memw(r29+44) = r9 }
	{ r2 = add(#0x80,asl(r2,#0x4)); r8 = add(r8,#0x17F); memw(r29+92) = r8; memw(r29+64) = r3 }
	{ if (p0) r18 = add(r13,#0x0); r8 = and(r8,#0xFFFFFF80) }
	{ r8 += add(r4,#0x7F); if (!p0) r18 = zxtb(r14); memw(r29+88) = r8 }
	{ if (!p1) r18 = add(r15,#0x0); r0 = and(r8,#0xFFFFFF80) }
	{ r5 += add(r6,r0); r20 = addasl(r0,r24,#0x2); memw(r29+80) = r0 }
	{ r25 = and(r5,#0xFFFFFF80) }
	{ r3 = add(r25,r7); memw(r29+76) = r25 }
	{ r21 = and(r3,#0xFFFFFF80) }
	{ call fn000095F0; r1:r0 = combine(#0x0,r21) }
	{ immext(#0x13240); r4 = add(PC,#0x13250); r3:r2 = combine(#0x2,r17); memw(r29+4) = r26 }
	{ r1 = #0x23B; r16 = r26; memw(r29) = r22 }
	{ call logmsg_function }
	{ immext(#0x13240); r4 = add(PC,#0x1324B); r3:r2 = combine(#0x2,r17); r1 = #0x23C }
	{ r5 = memd(r29+56); memw(r29+4) = r20 }
	{ call logmsg_function; memw(r29) = r5 }
	{ immext(#0x13240); r4 = add(PC,#0x13243); r3:r2 = combine(#0x2,r17); r1 = #0x23D }
	{ r22 = r21; r5 = memd(r29+64); memw(r29+4) = r21 }
	{ r17 = memw(r29+128); memw(r29) = r5 }
	{ call logmsg_function }
	{ r3 = r20; r21 = memd(r29+32); r2 = memd(r29+116) }
	{ r5:r4 = combine(r24,r21); r1 = memd(r29+60); r0 = memd(r29+68) }
	{ call pad2d; memw(r29) = r18 }
	{ r3:r2 = combine(r25,r24); r1:r0 = combine(r21,r20); r26 = r24; r24 = r21 }
	{ call transpack }
	{ r9 = r19; r12 = r23; r2 = memd(r29+104); r7 = memd(r29+120) }
	{ r6 = sub(r7,r17); r4 = add(r12,#0xFFFFFFFF); r3 = memw(r29+108); r8 = memw(r29+112) }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 00013FE8; r19 = memd(r29+84); r20 = memd(r29+72) }

l00013EC8:
	{ r15 = mpyi(r12,r9); r2 = add(r9,#0xFFFFFFFF); r13 = memw(r29+44); r5 = memw(r29+124) }
	{ r5 = sub(r5,r13); r14 = sub(#0x0,r18); r0 = memd(r29+48); r1 = memd(r29+52) }
	{ r2 = add(r6,mpyi(r2,r8)); r6 = #0xFF; p1 = cmp.gt(r1,#0xFF); p0 = cmp.gt(r1,#0xFFFFFFFF) }
	{ r4 = add(r5,mpyi(r4,r0)); r5 = mpyi(r13,r17); memw(r29+64) = r15; memw(r29+60) = r14 }
	{ r2 += lsr(r2,#0x1F); if (!p1) r6 = zxtb(r1); r1 = memd(r29+116); r17 = memd(r29+104) }
	{ r5 = mpyi(r5,r3); r4 += lsr(r4,#0x1F); memb(r29+13) = r5.new }
	{ r2 = asr(r2,#0x1); r5 = #0x0 }
	{ if (!p0) r6 = add(r5,#0x0); memw(r29+56) = r0; memw(r29+40) = r2 }
	{ r6 = asr(r4,#0x1); r1 = sub(#0x0,r6); memw(r29+68) = r6 }
	{ memw(r29+48) = r1; memw(r29+36) = r6 }

l00013F40:
	{ r5 = r16; r0 = r19; r2 = memw(r29+36); memb(r29+6) = r2.new }
	{ r18 = r12; r6 = memd(r29+40); r1 = memd(r29+44) }
	{ r21 = r8; memw(r29+20) = r6; memw(r29+16) = r9 }
	{ r23 = r7; r4 = memd(r29+68) }
	{ r2 = memw(r29+128); memw(r29+12) = r12 }
	{ memw(r29+8) = r8 }
	{ r6 = memd(r29+124); memw(r29+4) = r7 }
	{ call im2col_co; memw(r29) = r6 }
	{ r5:r4 = combine(r27,r22); r2 = memd(r29+92); r1 = memd(r29+48) }
	{ r0 = r16; r7 = memd(r29+80); memw(r29+28) = r2 }
	{ r2 = memd(r29+88); memw(r29+24) = r7 }
	{ memw(r29+20) = r2; memw(r29+16) = r24 }
	{ r3 = memd(r29+60); r2 = memd(r29+76) }
	{ memw(r29+8) = r27; memw(r29+4) = r24 }
	{ call gemm_asm; memw(r29) = r26; memw(r29+12) = #0xFFFFFFA0 }
	{ r1:r0 = combine(r27,r22); r5 = memd(r29+116); r4 = memd(r29+64) }
	{ call unpad2d; r3:r2 = combine(r20,r26) }
	{ r12 = r18; r9:r8 = combine(r25,r21); r2 = memd(r29+56); r6 = memd(r29+52) }
	{ r19 = add(r19,r6); r7 = r23; r3 = memd(r29+108) }
	{ r20 = addasl(r20,r2,#0x2); r17 = add(r17,#0xFFFFFFFF); if (!cmp.eq(r17.new,#0x0)) jump:t 00013F40 }

l00013FE8:
	{ immext(#0x130C0); r4 = add(PC,#0x130C2); r2 = memw(r29+116); memb(r29+3) = r2.new }

l00013FEC:
	{ r4 = add(PC,#0x2); r2 = memw(r29+116); memb(r29+3) = r2.new }

l00013FFC:
	{ r1 = #0x259; r2 = memw(r29+100); memw(r29+8) = r9 }
	{ memw(r29+4) = r12; memw(r29) = r5 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00014018:
	{ r17:r16 = memd(r29+176); r19:r18 = memd(r29+168) }
	{ r21:r20 = memd(r29+160); r23:r22 = memd(r29+152) }
	{ r25:r24 = memd(r29+144); r27:r26 = memd(r29+136) }
	{ dealloc_return }

;; conv2d_check_ref: 0001402C
conv2d_check_ref proc
	{ immext(#0x12F00); r4 = add(PC,#0x12F07); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r1 = #0x271; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memd(r29+32) = r19:r18; memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x7)) jump:t 0001406C }

l00014058:
	{ r3 = add(PC,#0x37); r1 = #0x272; r2 = memw(r17+28) }
	{ jump 00014080; memw(r29) = r2 }

l0001406C:
	{ r1 = #0x273; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 00014090 }

l0001407C:
	{ r3 = add(PC,#0x2F) }

l00014080:
	{ call errlog_function; r2 = r16 }

l00014084:
	{ r2 = r16 }
	{ jump 000141A8; r0 = #0xFFFFFFFF }

l00014090:
	{ r1 = #0x274; r3 = memw(r17+4); if (!cmp.eq(r3.new,#0x0)) jump:t 000140A8 }

l000140A0:
	{ r3 = add(PC,#0x22); jump 00014084 }

l000140A8:
	{ r2 = memw(r17+8) }
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 000140D8; if (p0.new) r1 = #0x275; if (!p0.new) r4 = #0x0 }

l000140B8:
	{ loop0(000140BC,#0x7) }
	{ r5 = memw(r3); if (!cmp.eq(r5.new,#0x0)) jump:t 000140E4 }

l000140C8:
	{ r3 = add(PC,#0x13); r1 = #0x278; memw(r29) = r4 }
	{ jump 00014080 }

l000140D8:
	{ immext(#0x12E80); r3 = add(PC,#0x12EB2); jump 00014080 }

l000140E4:
	{ nop; r3 = add(r3,#0x4); r4 = r4 }
	{ r0 = #0x0; jump 000140FC }

l000140F0:
	{ r2 = add(r2,#0x4); r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x2)) jump:nt 00014118 }

l000140F4:
	{ r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x2)) jump:nt 0001411C }

l000140FC:
	{ r3 = memw(r2); if (!cmp.eq(r3.new,#0x0)) jump:t 000140F0 }

l00014100:
	{ if (!cmp.eq(r3.new,#0x0)) jump:t 000140F4 }

l00014108:
	{ r3 = add(PC,#0x21); r1 = #0x27D; memw(r29) = r4 }
	{ jump 00014080 }

l00014118:
	{ immext(#0x12E80); r4 = add(PC,#0x12E9C); r3:r2 = combine(#0x3,r16); r1 = #0x282 }

l0001411C:
	{ r4 = add(PC,#0x1C); r3:r2 = combine(#0x3,r16); r1 = #0x282 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r19:r18 = combine(#0x0,#0x0); r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 0001418C }

l0001413C:
	{ immext(#0x12EC0); r4 = add(PC,#0x12EDF); r1 = #0x26B; r2 = memw(r17+4) }

l00014140:
	{ r4 = add(PC,#0x1F); r1 = #0x26B; r2 = memw(r17+4) }

l0001414C:
	{ r2 = memw(r13+r18) }
	{ r5 = memw(r2+16); r6 = memw(r2) }
	{ r7 = memw(r2+4); r8 = memw(r2+8) }
	{ r9 = memw(r2+12); r2 = memw(r2+24) }
	{ memw(r29+24) = r5; memw(r29+8) = r7 }
	{ r3:r2 = combine(#0x3,r16); memw(r29+20) = r2; memw(r29+4) = r6 }
	{ memw(r29+16) = r9; memw(r29+12) = r8 }
	{ call logmsg_function; memw(r29) = r19 }
	{ r18 = add(r18,#0x4); r2 = memw(r17+16) }
	{ r19 = add(r19,#0x1); if (cmp.gtu(r2,r19.new)) jump:t 0001413C }

l0001418C:
	{ immext(#0x12E40); r4 = add(PC,#0x12E77); r3:r2 = combine(#0x2,r16); r1 = #0x286 }

l00014190:
	{ r4 = add(PC,#0x37); r3:r2 = combine(#0x2,r16); r1 = #0x286 }

l0001419C:
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l000141A8:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ dealloc_return }

;; conv2d_ctor: 000141B0
conv2d_ctor proc
	{ immext(#0x12D40); r6 = add(PC,#0x12D6C); memd(r29-24) = r19:r18; allocframe(#0x28) }
	{ r19:r18 = combine(r0,r4) }
	{ r17:r16 = combine(r3,r1); r21:r20 = combine(r2,r5); memd(r29+32) = r17:r16; memd(r29+16) = r21:r20 }
	{ r1 = #0x295; r3:r2 = combine(#0x2,r19); r4 = r6 }
	{ r22 = memd(r29+48); memd(r29+8) = r23:r22 }
	{ r23 = memw(r29+52) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r5:r4 = combine(r20,r18); r3:r2 = combine(r17,r21); r1:r0 = combine(r16,r19); memw(r29+4) = r23 }
	{ call node_alloc_common; memw(r29) = r22 }
	{ r17:r16 = memd(r29+32) }
	{ r19:r18 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r23:r22 = memd(r29+8); dealloc_return }

;; conv2d_execute_ref: 0001420C
conv2d_execute_ref proc
	{ allocframe(#0x88) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+96) = r25:r24; memd(r29+120) = r19:r18 }
	{ r25 = memb(r0+32); r18 = memw(r2) }
	{ p0 = cmp.eq(r25,#0x0); r23 = memw(r2+4); memd(r29+104) = r23:r22 }
	{ r5 = memw(r2+24); r7 = memw(r2+8) }
	{ memd(r29+112) = r21:r20; memd(r29+88) = r27:r26 }
	{ r4 = memw(r2+12) }
	{ r7 = memw(r3+8); memw(r29+60) = r7 }
	{ r20 = memw(r3); r3 = memw(r3+4) }
	{ r27 = memw(r18+8); memd(r29+128) = r17:r16 }
	{ r0 = r27; memw(r29+72) = r0; memw(r29+32) = r1 }
	{ r4 = memw(r2+20); memw(r29+64) = r4 }
	{ r24 = memw(r2+16); memw(r29+36) = r3 }
	{ r2 = memw(r23+8); r17 = memw(r18+4) }
	{ r19 = memw(r18+12); r16 = memw(r23+12) }
	{ r21 = memw(r23); r22 = memw(r23+4) }
	{ r1 = memw(r5+8); r3 = memw(r5+4) }
	{ r7 = memw(r18); memw(r29+24) = r7 }
	{ r2 = p0; memw(r29+80) = r2; memw(r29+40) = r5 }
	{ memw(r29+56) = r4; memw(r29+28) = r7 }
	{ if (p0) jump:nt 000142B0; if (!p0) r2 = sub(r27,r22); memw(r29+48) = r2 }

l0001428C:
	{ if (p0.new) jump:nt 000142B0; p0 = cmp.eq(r25,#0x2); if (p0.new) r0 = add(r2,r1) }

l00014298:
	{ if (!p0.new) jump:nt 000142C0; p0 = cmp.eq(r25,#0x1); r0 = #0x0; memw(r29+68) = r1 }

l000142A4:
	{ r1 = memw(r29+68) }
	{ r0 = r1 }
	{ r0 += add(r27,#0xFFFFFFFF) }

l000142B0:
	{ call fn00009760; r26 = r3; memw(r29+68) = r1 }
	{ r3 = r26 }

l000142C0:
	{ if (p0.new) jump:nt 000142FC; r2 = sub(r17,r21); p0 = cmp.eq(r25,#0x2); if (p0.new) memw(r29+76) = r0 }

l000142D0:
	{ if (p0.new) jump:nt 000142F4; p0 = cmp.eq(r25,#0x1); if (!p0.new) r25 = add(r3,#0x0); if (!p0) r1:r0 = combine(r3,r3) }

l000142E0:
	{ r0 = #0x0; r1 = memd(r29+48) }
	{ p0 = r1; if (!p0.new) jump:nt 0001430C; if (!p0) r1:r0 = combine(r25,r17) }

l000142F0:
	{ jump 00014308 }

l000142F4:
	{ r0 += add(r17,#0xFFFFFFFF); jump 00014304 }

l000142FC:
	{ r0 = add(r2,r3); r1 = r3; memw(r29+76) = r0 }

l00014304:
	{ r25 = r3 }

l00014308:
	{ call fn00009760 }

l0001430C:
	{ immext(#0x437F0000); r26 = #0x437F0000; r2 = memd(r29+64); r3 = memd(r29+60) }
	{ r6 = memw(r23+16); r5 = memd(r29+56) }
	{ r2 = memw(r2+16); r3 = memw(r3+16) }
	{ r6 = memw(r20+16); memw(r29+64) = r6 }
	{ r2 = memw(r2); memw(r29+52) = r20 }
	{ r20 = memw(r3); r4 = memw(r24+16) }
	{ r5 = memw(r5+16); memw(r29+48) = r0 }
	{ r24 = sfsub(r2,r20) }
	{ r1:r0 = combine(r26,r24); r25 = memw(r18+16); memw(r29+44) = r25 }
	{ r18 = memw(r5); memw(r29+60) = r6 }
	{ call fn00009610; r23 = memw(r4) }
	{ r2 = sfsub(r18,r23); r18 = r26; memw(r29+56) = r0 }
	{ call fn00009610; r1:r0 = combine(r26,r2); r26 = r2 }
	{ immext(#0xCF000000); r2 = #0xCF000000; r4 = memd(r29+56) }
	{ immext(#0x4F000000); r3 = #0x4F000000 }
	{ r4 = sfmpy(r4,r0); r0 = r24 }
	{ r3 = sfmpy(r4,r3); r2 = sfmpy(r4,r2) }
	{ memw(r29+56) = r3; memw(r29+20) = r2 }
	{ call fmaxf.1.0 }
	{ call fn00009610; r1:r0 = combine(r0,r18) }
	{ immext(#0x0); r3 = #0x0 }
	{ r2 = sfsub(r3,r20); r20 = r3 }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = r0; r0 = r26 }
	{ r24 = convert_uw2sf(r2):chop; call fmaxf.1.0 }
	{ call fn00009610; r1:r0 = combine(r0,r18) }
	{ r2 = sfsub(r20,r23) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ immext(#0x12A00); r4 = add(PC,#0x12A20); r26 = memw(r29+72); r20 = memw(r29+32) }
	{ r18 = convert_uw2sf(r0):chop; r1 = #0x174; r3:r2 = combine(#0x2,r20) }
	{ r5 = memw(r26+28); memb(r29+1) = r5.new }
	{ memw(r29) = r26 }
	{ immext(#0x12A00); r4 = add(PC,#0x12A12); r3:r2 = combine(#0x2,r20); memw(r29+12) = r19 }
	{ r1 = #0x175; memw(r29+4) = r17 }
	{ r23 = memw(r29+28); memw(r29+8) = r27 }
	{ call logmsg_function; memw(r29) = r23 }
	{ immext(#0x12A00); r4 = add(PC,#0x12A03); r2 = memd(r29+80); memw(r29+4) = r21 }
	{ r1 = #0x176; r3:r2 = combine(#0x2,r20); memw(r29+12) = r2 }
	{ memw(r29+8) = r22; memw(r29) = r16 }
	{ call logmsg_function }
	{ immext(#0x129C0); r4 = add(PC,#0x129FB); r3:r2 = combine(#0x2,r20); r7 = memw(r29+68) }
	{ r1 = #0x177 }
	{ r5 = memd(r29+44); memw(r29+4) = r7 }
	{ call logmsg_function; memw(r29) = r5 }
	{ immext(#0x129C0); r4 = add(PC,#0x129EF); r3:r2 = combine(#0x2,r20); r1 = #0x178 }
	{ r5 = memb(r26+32); r26 = memw(r29+48) }
	{ call logmsg_function; memw(r29) = r5 }
	{ r3:r2 = combine(#0x2,r20); memw(r29+12) = r16; memw(r29) = r23 }
	{ immext(#0x129C0); r4 = add(PC,#0x129D9); r20 = memw(r29+76); memw(r29+4) = r26 }
	{ r1 = #0x179; memw(r29+8) = r20 }
	{ call logmsg_function }
	{ immext(#0x129C0); r3 = add(PC,#0x129DC); r7:r6 = combine(r20,r23); r1 = #0x17A }
	{ r5 = r26; r2 = memw(r29+80); if (!cmp.eq(r2.new,r19)) jump:t 00014560 }

l000144CC:
	{ r3 = memw(r29+52) }
	{ r2 = mpyi(r2,r20); r4 = memw(r3+20) }
	{ r2 = mpyi(r2,r26) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r4)) jump:t 000144FC }

l000144E8:
	{ r3 = add(PC,#0x39); r1 = #0x17C; memw(r29+4) = r2 }
	{ r2 = memd(r29+32); memw(r29) = r4 }
	{ jump 00014564 }

l000144FC:
	{ r9:r8 = combine(r5,r3); r1 = #0x17E; r4 = memw(r29+40) }
	{ immext(#0x12980); r3 = add(PC,#0x129AF) }
	{ r4 = memw(r4); if (!cmp.eq(r4.new,#0x1)) jump:t 00014560 }

l0001451C:
	{ r3 = memw(r29+40) }
	{ immext(#0x12980); r3 = add(PC,#0x129A8); r4 = memw(r3+12) }
	{ p0 = cmp.eq(r4,#0x1); if (!p0.new) jump:nt 00014560 }

l00014530:
	{ r1 = #0x180; r5 = #0x4; r3 = memd(r29+36) }
	{ immext(#0x12980); r3 = add(PC,#0x129A1); r4 = memw(r3+20) }
	{ p0 = cmp.gtu(r5,r4); if (p0) jump:nt 00014560 }

l00014548:
	{ r1 = #0x181; r3 = memw(r29+24) }
	{ immext(#0x12980); r3 = add(PC,#0x12997); r4 = memw(r3+20) }
	{ p0 = cmp.gtu(r4,#0x3); if (p0.new) jump:nt 00014570 }

l00014560:
	{ r2 = memw(r29+32) }

l00014564:
	{ call errlog_function }
	{ jump 000147AC; r0 = #0xFFFFFFFF }

l00014570:
	{ r5 = r6; p0 = cmp.gt(r6,#0x0); r3 = memw(r29+36); memw(r8+4) = r9 }
	{ if (p0) r5 = add(r9,#0xFFFFFFFF); memw(r8+12) = r16; memw(r8+8) = r7 }
	{ memw(r8+24) = r2 }
	{ r8 = r7; memw(r8) = r6 }
	{ r2 = memw(r3+16); r1 = memd(r29+24) }
	{ memw(r3+12) = #0x1; memw(r3) = #0x1 }
	{ memw(r3+4) = #0x1; memw(r3+8) = #0x1 }
	{ r4 = memw(r29+20); memb(r2) = r4.new }
	{ memw(r3+24) = #0x4; memw(r1) = #0x1 }
	{ r2 = memw(r1+16); memw(r1+4) = #0x1 }
	{ memw(r1+8) = #0x1; memw(r1+12) = #0x1 }
	{ if (p0) r2 = sub(r22,r27); r4 = memw(r29+56); memb(r2) = r4.new }
	{ if (p0) r4 = sub(r21,r17); memw(r1+24) = #0x4 }
	{ r1 = mpyi(r22,r19); r6 = memd(r29+68); r7 = memd(r29+44) }
	{ r3 = add(r2,mpyi(r3,r6)); r5 = add(r4,mpyi(r5,r7)); r6 = #0x0; memb(r29+6) = r6.new }
	{ r4 = mpyi(r1,r16) }
	{ r3 += lsr(r3,#0x1F); r5 += lsr(r5,#0x1F) }
	{ r3 = asr(r3,#0x1); r7 = asr(r5,#0x1) }
	{ memw(r29+56) = r3; memw(r29+36) = r7 }

l0001460C:
	{ if (!p0.new) jump:nt 00014774; p0 = cmp.gt(r9,#0x0) }

l00014614:
	{ r6 = #0x0; r3 = memw(r29+24); memb(r29+13) = r6.new }
	{ r3 = mpyi(r3,r9); memb(r29+10) = r3.new }

l0001462C:
	{ if (!p0.new) jump:nt 0001475C; p0 = cmp.gt(r8,#0x0) }
	{ r1 = #0x0; r3 = memd(r29+44); r6 = memd(r29+52) }
	{ r3 = mpyi(r6,r3); r5 = memd(r29+40); memw(r29+80) = r1 }
	{ r5 = add(r6,r5); r6 = memw(r29+36) }
	{ r1 = mpyi(r5,r8); r13 = sub(r3,r6); memb(r29+18) = r1.new }

l00014658:
	{ p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt 00014744; r28 = #0x0; if (p0.new) r3 = memw(r29+72) }
	{ r6 = memd(r29+80); r5 = memd(r29+68) }
	{ r15 = memw(r29+64) }
	{ r5 = mpyi(r6,r5); r3 = add(r6,r3); r6 = memw(r29+56) }
	{ r3 = mpyi(r3,r16); r0 = sub(r5,r6); r5 = memw(r29+60) }
	{ r1 = addasl(r5,r3,#0x2) }

l00014688:
	{ r6 = r15; r3 = #0x0; r10 = #0x0; p0 = cmp.gt(r21,#0x0) }
	{ if (!p0) jump:nt 00014730 }

l0001469C:
	{ r5 = add(r3,r13); if (!cmp.gtu(r17,r5.new)) jump:nt 00014724 }

l000146A8:
	{ if (p0.new) r11 = add(r6,#0x0) }
	{ p0 = cmp.gt(r14,#0x0); if (!p0.new) jump:nt 00014724; r5 = add(r5,r7); if (p0.new) r8 = #0x0 }

l000146B8:
	{ loop1(000146C0,r22); r26 = mpyi(r5,r27) }
	{ r5 = add(r8,r0); if (!cmp.gtu(r27,r5.new)) jump:nt 00014718 }

l000146CC:
	{ if (p0.new) r14 = add(r19,#0xFFFFFFFF); if (p0.new) r23 = add(r5,r26) }
	{ p0 = cmp.gt(r11,#0x0); if (!p0.new) jump:nt 00014718; if (p0.new) r9 = add(r11,r16); if (p0.new) r20 = memb(r11) }

l000146E0:
	{ loop0(000146FC,r14); r23 = add(r25,mpyi(r23,r19)); r20 = sub(r20,r18); p0 = cmp.gtu(r19,#0x1) }
	{ r5 = memb(r23++#1) }
	{ if (!p0) jump:nt 00014714; r5 = sub(r5,r24) }

l000146FC:
	{ r10 += mpyi(r20,r5); r9 = add(r9,r16); r14 = memb(r9); r12 = memb(r23++#1) }
	{ r20 = sub(r14,r18); r5 = sub(r12,r24) }

l00014714:
	{ r10 += mpyi(r20,r5) }

l00014718:
	{ r8 = add(r8,#0x1); r11 = add(r11,r2); nop }

l00014724:
	{ r6 = add(r6,r4); r3 = add(r3,#0x1); if (!cmp.eq(r3.new,r21)) jump:t 0001469C }

l00014730:
	{ r28 = add(r28,#0x1); r15 = add(r15,#0x1); memw(r1++#4) = r10 }

l00014734:
	{ r15 = add(r15,#0x1); memw(r1++#4) = r10 }

l0001473C:
	{ if (!p0.new) jump:nt 00014688; p0 = cmp.eq(r28,r16) }

l00014744:
	{ r3 = memw(r29+80); r8 = memw(r29+76) }
	{ r3 = add(r3,#0x1) }
	{ if (!p0.new) jump:nt 00014658; p0 = cmp.eq(r3,r8); memw(r29+80) = r3 }

l0001475C:
	{ r3 = memw(r29+52); r9 = memw(r29+48) }
	{ r3 = add(r3,#0x1) }
	{ if (!p0.new) jump:nt 0001462C; p0 = cmp.eq(r3,r9); memw(r29+52) = r3 }

l00014774:
	{ r3 = memd(r29+24); r5 = memd(r29+28) }
	{ r3 = add(r3,#0x1) }
	{ p0 = cmp.eq(r3,r5); if (!p0.new) jump:nt 0001460C; memw(r29+24) = r3 }

l00014784:
	{ immext(#0x12740); r4 = add(PC,#0x12771); r3 = #0x2; memw(r29+12) = r16 }
	{ r1 = #0x1B4; r2 = memw(r29+32) }
	{ memw(r29+8) = r8; memw(r29+4) = r9 }
	{ call logmsg_function; memw(r29) = r5 }
	{ r0 = #0x0 }

l000147AC:
	{ r17:r16 = memd(r29+128); r19:r18 = memd(r29+120) }
	{ r21:r20 = memd(r29+112); r23:r22 = memd(r29+104) }
	{ r25:r24 = memd(r29+96); r27:r26 = memd(r29+88) }
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
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000147E0 }

l000147D0:
	{ r0 = add(PC,#0xC); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l000147E0:
	{ dealloc_return }

;; errlog_function: 000147E4
;;   Called from:
;;     00013CF0 (in conv2d_execute_hvx)
;;     00014080 (in conv2d_check_ref)
;;     00014564 (in conv2d_execute_ref)
errlog_function proc
	{ immext(#0x125C0); r0 = add(PC,#0x125F4); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; fmaxf.1.0: 00014808
;;   Called from:
;;     00013B2C (in conv2d_execute_hvx)
;;     0001438C (in conv2d_execute_ref)
;;     000143B8 (in conv2d_execute_ref)
fmaxf.1.0 proc
	{ immext(#0x38D1B700); r2 = #0x38D1B717 }
	{ jump fn00009600; r1:r0 = combine(r0,r2) }
00014818                         00 00 00 00 00 00 00 00         ........

;; flatten_execute: 00014820
flatten_execute proc
	{ immext(#0x12980); r4 = add(PC,#0x129B2); allocframe(#0x28) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ r16 = r1; r1 = #0x37; memd(r29+32) = r17:r16; memd(r29+24) = r19:r18 }
	{ r2 = r16; r17 = memw(r2) }
	{ memd(r29+16) = r21:r20; memd(r29+8) = r23:r22 }
	{ r18 = memw(r3); r19 = memw(r17+8) }
	{ r21 = memw(r17+4); r22 = memw(r17) }
	{ call logmsg_function; r20 = memw(r17+12); memw(r29) = r0 }
	{ r2 = memw(r17+24) }
	{ r3 = memw(r18+20); if (!cmp.gtu(r2,r3.new)) jump:t 00014878 }

l00014864:
	{ r3 = add(PC,#0x3C); r1 = #0x39; r2 = r16 }
	{ call errlog_function }
	{ jump 000148BC; r0 = #0xFFFFFFFF }

l00014878:
	{ r2 = mpyi(r21,r22); memw(r18) = #0x1; memw(r18+8) = #0x1 }
	{ r6 = mpyi(r2,r19); r0 = memw(r18+16); memw(r18+4) = #0x1 }
	{ r7 = mpyi(r6,r20); memb(r18+3) = r7.new }
	{ memw(r18+24) = r2 }
	{ call fn00009560; r1 = memw(r17+16); r2 = memw(r17+24) }
	{ immext(#0x128C0); r4 = add(PC,#0x128CA); r1 = #0x3F; r2 = r16 }
	{ call logmsg_function; r3 = memw(r17+24); memb(r29) = r3.new }

l000148BC:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; flatten_check: 000148C8
flatten_check proc
	{ immext(#0x128C0); r4 = add(PC,#0x128C1); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x5C; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0x12800); r3 = add(PC,#0x12824); r1 = #0x5D }
	{ r2 = memw(r17+16); if (cmp.gtu(r2.new,#0x2)) jump:t 0001493C }

l000148FC:
	{ r3 = add(PC,#0x1F); r1 = #0x5E }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x1)) jump:t 0001493C }

l00014910:
	{ r3 = add(PC,#0x16); r1 = #0x5F; r2 = memw(r17+4) }
	{ r2 = memw(r2); if (cmp.eq(r2.new,#0x0)) jump:nt 0001493C }

l00014928:
	{ r3 = add(PC,#0x9); r1 = #0x60; r2 = memw(r17+8) }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x0)) jump:t 0001494C }

l0001493C:
	{ call errlog_function; r2 = r16 }

l00014940:
	{ r2 = r16 }

l00014944:
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001494C:
	{ immext(#0x12840); r4 = add(PC,#0x1286D); r1 = #0x61; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; qflatten_execute: 0001496C
qflatten_execute proc
	{ immext(#0x127C0); r4 = add(PC,#0x127D5); memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x4C; r2 = memw(r17+4); memd(r29+24) = r19:r18 }
	{ r3 = memw(r17+8) }
	{ memd(r29+16) = r21:r20; memd(r29+8) = r23:r22 }
	{ r2 = r16; r18 = memw(r2); r19 = memw(r3) }
	{ r20 = memw(r18+8) }
	{ r22 = memw(r18+4); r23 = memw(r18) }
	{ call logmsg_function; r21 = memw(r18+12); memw(r29) = r17 }
	{ r2 = memw(r18+24) }
	{ r3 = memw(r19+20); if (!cmp.gtu(r2,r3.new)) jump:t 000149CC }

l000149B4:
	{ r3 = add(PC,#0x2C); r1 = #0x4E; r2 = r16 }
	{ call errlog_function }
	{ jump 00014A70; r0 = #0xFFFFFFFF }

l000149CC:
	{ r2 = mpyi(r22,r23); memw(r19) = #0x1; memw(r19+8) = #0x1 }
	{ r6 = mpyi(r2,r20); r0 = memw(r19+16); memw(r19+4) = #0x1 }
	{ r7 = mpyi(r6,r21); memb(r19+3) = r7.new }
	{ memw(r19+24) = r2 }
	{ call fn00009560; r1 = memw(r18+16); r2 = memw(r18+24) }
	{ r2 = memw(r17+8); r3 = memw(r17+4) }
	{ r3 = memw(r3+4); r2 = memw(r2+4) }
	{ r4 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 00014A24 }

l00014A1C:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l00014A24:
	{ r2 = memw(r17+8); r3 = memw(r17+4) }
	{ r3 = memw(r3+8); r2 = memw(r2+8) }
	{ r4 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 00014A54 }

l00014A4C:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l00014A54:
	{ immext(#0x12700); r4 = add(PC,#0x12716); r2 = memw(r18+24) }
	{ r1 = #0x56; r2 = r16; memw(r29) = r2 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00014A70:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }
00014A7C                                     00 C0 00 7F             ....

;; qflatten_check: 00014A80
qflatten_check proc
	{ immext(#0x12640); r4 = add(PC,#0x1266E); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x67; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x68; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 00014AC0 }

l00014AAC:
	{ r3 = add(PC,#0x20) }
	{ call errlog_function; r2 = r16 }

l00014AB4:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00014AC0:
	{ r1 = #0x69; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 00014AD8 }

l00014AD0:
	{ r3 = add(PC,#0xB); jump 00014AB4 }

l00014AD8:
	{ immext(#0x12640); r4 = add(PC,#0x1264F); r1 = #0x6A; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

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
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00014B1C }

l00014B08:
	{ r0 = add(PC,#0xD); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

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
	{ immext(#0x12580); r0 = add(PC,#0x125B1); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00014B44             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; input_execute: 00014B50
input_execute proc
	{ immext(#0x126C0); r4 = add(PC,#0x126FF); memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ r1 = #0x31; r16 = r1 }
	{ r2 = r16; r18 = r0; memd(r29+16) = r19:r18; memd(r29+8) = r21:r20 }
	{ call logmsg_function; memw(r29) = r18 }
	{ r2 = memw(r16+28) }
	{ r3 = memw(r18+20); if (!cmp.eq(r3.new,r2)) jump:t 00014BB0 }

l00014B84:
	{ r17 = #0x0; r2 = #0x0 }
	{ r19 = #0x0; r21:r20 = combine(#0x0,#0x0) }

l00014B8C:
	{ r3 = memw(r18+8); r4 = memw(r16+20) }
	{ r2 = add(r4,r20); r3 = memw(r29+r19) }
	{ r5 = memw(r2+20) }
	{ r6 = memw(r3+20); if (!cmp.gtu(r5,r6.new)) jump:t 00014BC8 }

l00014BA8:
	{ r3 = add(PC,#0x12); r11 = #0x3B; jump 00014BC0 }

l00014BB0:
	{ immext(#0x12680); r3 = add(PC,#0x126B8); r1 = #0x36 }
	{ call errlog_function; r2 = r16; r17 = #-0x1 }

l00014BC0:
	{ r2 = r16; r17 = #-0x1 }

l00014BC4:
	{ jump 00014C28 }

l00014BC8:
	{ r4 = memw(r5+r20); memb(r3) = r4.new }
	{ memw(r3+4) = r7 }
	{ r4 = memw(r2+8) }
	{ memw(r3+8) = r4 }
	{ r7 = memw(r2+12) }
	{ memw(r3+12) = r7 }
	{ r4 = memw(r2+20) }
	{ memw(r3+24) = r4 }
	{ r1 = memw(r2+16) }
	{ call vmemcpy_asm; r0 = memw(r3+16); r2 = memw(r2+20) }
	{ r19 = add(r19,#0x4); r20 = add(r20,#0x20); r2 = memw(r18+20) }
	{ r21 = add(r21,#0x1); if (cmp.gtu(r2,r21.new)) jump:t 00014B8C }

l00014C10:
	{ immext(#0x12640); r4 = add(PC,#0x12674); r1 = #0x40; memw(r29) = r2 }
	{ call logmsg_function; r2 = r16 }

l00014C28:
	{ r0 = r17; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); dealloc_return }

;; input_check: 00014C34
input_check proc
	{ immext(#0x125C0); r4 = add(PC,#0x125D3); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x46; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 00014C94 }

l00014C5C:
	{ r3 = memw(r17+8) }
	{ r3 = add(r3,#0x4); r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 00014C94 }

l00014C64:
	{ r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 00014C98 }

l00014C70:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 00014C64 }

l00014C78:
	{ r4 = add(PC,#0x2A); r1 = #0x4A; r3:r2 = combine(#0x0,r16) }
	{ call errlog_function; memw(r29) = r4 }
	{ r0 = #0xFFFFFFFF }
	{ r17:r16 = memd(r29+8); dealloc_return }

l00014C94:
	{ immext(#0x12580); r4 = add(PC,#0x125A4); r1 = #0x4D; r2 = r16 }

l00014C98:
	{ r4 = add(PC,#0x24); r1 = #0x4D; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00014CB4
;;   Called from:
;;     00014B6C (in input_execute)
;;     00014C20 (in input_execute)
;;     00014C48 (in input_check)
;;     00014CA4 (in input_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00014CD8 }

l00014CC4:
	{ r0 = add(PC,#0x2C); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00014CD8:
	{ dealloc_return }

;; errlog_function: 00014CDC
;;   Called from:
;;     00014BBC (in input_execute)
;;     00014C84 (in input_check)
;;     00014CCC (in logmsg_function)
errlog_function proc
	{ immext(#0x12500); r0 = add(PC,#0x12510); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; matmul_execute_asm: 00014D00
matmul_execute_asm proc
	{ immext(#0x700); r2 = add(PC,#0x724); jump matmul_execute }

;; matmul_check_ref: 00014D0C
matmul_check_ref proc
	{ immext(#0x126C0); r4 = add(PC,#0x126DD); memd(r29-16) = r17:r16; allocframe(#0x48) }
	{ r1 = #0x10F; r16 = r1; r17 = r0 }
	{ r3:r2 = combine(#0x2,r16); memd(r29+56) = r19:r18; memd(r29+48) = r21:r20 }
	{ memd(r29+40) = r23:r22; memd(r29+32) = r25:r24 }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0x126C0); r3 = add(PC,#0x126C9); r1 = #0x110 }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x6)) jump:t 00014EE0 }

l00014D50:
	{ r3 = add(PC,#0xB); r1 = #0x111 }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x3)) jump:t 00014EE0 }

l00014D64:
	{ r3 = add(PC,#0xE); r1 = #0x112 }
	{ r4 = memw(r17+4); if (cmp.eq(r4.new,#0x0)) jump:nt 00014EE0 }

l00014D78:
	{ r3 = add(PC,#0x6); r1 = #0x113; r2 = memw(r17+8) }
	{ loop0(00014D90,#0x6); p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 00014EE0; r5 = #0x0 }

l00014D90:
	{ r3 = memw(r4); if (cmp.eq(r3.new,#0x0)) jump:nt 00014DA4 }

l00014D9C:
	{ r4 = add(r4,#0x4); r5 = r5 }
	{ r0 = #0x0; jump 00014DC4 }

l00014DA4:
	{ immext(#0x12680); r3 = add(PC,#0x126A3); r1 = #0x116; memw(r29) = r5 }
	{ jump 00014EE0 }

l00014DB8:
	{ r2 = add(r2,#0x4); r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x2)) jump:nt 00014DE0 }

l00014DBC:
	{ r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x2)) jump:nt 00014DE4 }

l00014DC4:
	{ r3 = memw(r2); if (!cmp.eq(r3.new,#0x0)) jump:t 00014DB8 }

l00014DC8:
	{ if (!cmp.eq(r3.new,#0x0)) jump:t 00014DBC }

l00014DD0:
	{ r3 = add(PC,#0x9); r1 = #0x11B; memw(r29) = r4 }
	{ jump 00014EE0 }

l00014DE0:
	{ immext(#0x12680); r4 = add(PC,#0x12684); r3:r2 = combine(#0x3,r16); r1 = #0x120 }

l00014DE4:
	{ r4 = add(PC,#0x4); r3:r2 = combine(#0x3,r16); r1 = #0x120 }

l00014DF0:
	{ call logmsg_function; memw(r29) = r17 }
	{ r19:r18 = combine(#0x0,#0x0); r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 00014E58 }

l00014E04:
	{ immext(#0x12700); r4 = add(PC,#0x12723); r1 = #0x109; r2 = memw(r17+4) }

l00014E08:
	{ r4 = add(PC,#0x23); r1 = #0x109; r2 = memw(r17+4) }

l00014E14:
	{ r2 = memw(r13+r18) }
	{ r5 = memw(r2+16); r6 = memw(r2+24) }
	{ r7 = memw(r2+12); r8 = memw(r2+4) }
	{ r3:r2 = combine(#0x3,r16); r9 = memw(r2+8); r12 = memw(r2) }
	{ memw(r29+24) = r5; memw(r29+20) = r6 }
	{ memw(r29+16) = r7; memw(r29+12) = r9 }
	{ memw(r29+8) = r8; memw(r29+4) = r12 }
	{ call logmsg_function; memw(r29) = r19 }
	{ r18 = add(r18,#0x4); r2 = memw(r17+16) }
	{ r19 = add(r19,#0x1); if (cmp.gtu(r2,r19.new)) jump:t 00014E04 }

l00014E58:
	{ r2 = memw(r17+4) }

l00014E5C:
	{ r3 = memw(r2+20) }
	{ r4 = memw(r2+16); r7 = memw(r2+4) }
	{ r2 = memw(r4+16); r3 = memw(r3+16) }
	{ r18 = memw(r7+16); r20 = memw(r7+8) }
	{ r21 = memw(r2); r2 = memw(r3) }
	{ r0 = sfsub(r2,r21); r19 = memw(r7+12) }
	{ call fmaxf.1.0 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ immext(#0x0); r2 = #0x0 }
	{ r2 = sfsub(r2,r21) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = add(r20,#0xF); r4 = add(r19,#0x1F) }
	{ r5 = and(r2,#0xFFFFFFF0); r3:r2 = combine(#0x20,r0); r22 = and(r4,#0xFFFFFFE0); r0 = #0x80 }
	{ r23 = maxu(r5,r3); r24 = convert_uw2sf(r2):chop }
	{ r1 = mpyi(r23,r22); call fn00009550 }
	{ immext(#0x125C0); r3 = add(PC,#0x125D7); r1 = #0x13A; memw(r17+40) = r0 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 00014EEC; p1 = cmp.gt(r24,#0xFFFFFFFF) }

l00014EE0:
	{ call errlog_function; r2 = r16; r21 = #-0x1 }
	{ jump 00014F7C }

l00014EEC:
	{ p0 = cmp.gt(r24,#0xFF); r21 = #0x0; r0 = r16 }
	{ if (p0) r25 = #0xFF; if (!p0) r25 = zxtb(r24) }
	{ call nn_os_hvx_power_on; if (!p1) r25 = add(r21,#0x0) }
	{ call nn_os_vector_acquire }
	{ immext(#0x125C0); r4 = add(PC,#0x125C8); r3:r2 = combine(#0x2,r16); memw(r29+16) = r25 }
	{ r1 = #0x13E; memw(r29+4) = r19 }
	{ r24 = r0; memw(r29+12) = r22; memw(r29+8) = r23 }
	{ call logmsg_function; memw(r29) = r20 }
	{ r5:r4 = combine(r22,r23); r1:r0 = combine(r20,r18); r2 = r19; r3 = memw(r16+4) }
	{ call pad2d; memw(r29) = r25 }
	{ r2 = r22; r1 = r23; r0 = memw(r16+4); r3 = memw(r17+8) }
	{ call transpack }
	{ call nn_os_vector_release; r0 = r24 }
	{ call nn_os_hvx_power_off; r0 = r16 }
	{ immext(#0x12580); r4 = add(PC,#0x125AB); r3:r2 = combine(#0x2,r16); r1 = #0x143 }
	{ call logmsg_function; memw(r29) = r17 }

l00014F7C:
	{ r0 = r21 }
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); dealloc_return }

;; matmul_ctor: 00014F90
matmul_ctor proc
	{ immext(#0x12440); r6 = add(PC,#0x12442); memd(r29-24) = r19:r18; allocframe(#0x28) }
	{ r19:r18 = combine(r0,r4) }
	{ r17:r16 = combine(r3,r1); r21:r20 = combine(r2,r5); memd(r29+32) = r17:r16; memd(r29+16) = r21:r20 }
	{ r1 = #0x152; r3:r2 = combine(#0x2,r19); r4 = r6 }
	{ r22 = memd(r29+48); memd(r29+8) = r23:r22 }
	{ r23 = memw(r29+52) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r5:r4 = combine(r20,r18); r3:r2 = combine(r17,r21); r1:r0 = combine(r16,r19); memw(r29+4) = r23 }
	{ call node_alloc_common; memw(r29) = r22 }
	{ r17:r16 = memd(r29+32) }
	{ r19:r18 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r23:r22 = memd(r29+8); dealloc_return }

;; matmul_execute_ref: 00014FEC
matmul_execute_ref proc
	{ immext(#0x2C0); r2 = add(PC,#0x2F4); jump matmul_execute }

;; matmul_execute: 00014FF8
;;   Called from:
;;     00014D00 (in matmul_execute_asm)
;;     00014FEC (in matmul_execute_ref)
matmul_execute proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x78) }
	{ r3 = memw(r16+4); memw(r29+40) = r2 }
	{ r4 = memw(r16+8) }
	{ memw(r29+68) = r1; memd(r29+72) = r27:r26 }
	{ r7 = memw(r3); r2 = memw(r4+4) }
	{ r5 = memw(r3+12) }
	{ r2 = memw(r4); memw(r29+48) = r2 }
	{ r6 = memw(r3+8) }
	{ r2 = memw(r7+8); memw(r29+52) = r2 }
	{ r9 = memw(r3+4) }
	{ r2 = memw(r6+16); memw(r29+60) = r2 }
	{ r5 = memw(r5+16); r1 = memw(r4+8) }
	{ r4 = memw(r9); r8 = memw(r3+20) }
	{ r3 = memw(r3+16) }
	{ r4 = memw(r5); memw(r29+56) = r4 }
	{ r26 = memw(r2) }
	{ immext(#0x437F0000); r19 = #0x437F0000; memd(r29+104) = r19:r18; memd(r29+96) = r21:r20 }
	{ r8 = memw(r8+16) }
	{ r20 = sfsub(r4,r26); r1 = memw(r7+12); memw(r29+44) = r1 }
	{ r3 = memw(r3+16) }
	{ r1:r0 = combine(r19,r20); memw(r29+64) = r1; memd(r29+80) = r25:r24 }
	{ r18 = memw(r7+4); memd(r29+88) = r23:r22 }
	{ r17 = memw(r7); r25 = memw(r9+8) }
	{ r24 = memw(r9+4); r23 = memw(r9+12) }
	{ call fn00009610; r21 = memw(r8); r27 = memw(r3) }
	{ r21 = sfsub(r21,r27); r22 = r0 }
	{ call fn00009610; r1:r0 = combine(r19,r21) }
	{ r4 = sfmpy(r22,r0); immext(#0x4F000000); r3 = #0x4F000000 }
	{ immext(#0xCF000000); r2 = #0xCF000000; r0 = r20 }
	{ r3 = sfmpy(r4,r3); r22 = sfmpy(r4,r2); memb(r29+9) = r3.new }
	{ call fn00009610; r1:r0 = combine(r0,r19); immext(#0x0); r20 = #0x0 }
	{ r2 = sfsub(r20,r26); r26 = r24; r24 = r17 }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = r0; r0 = r21 }
	{ r19 = convert_uw2sf(r2):chop; call fmaxf.1.0 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = sfsub(r20,r27); r20 = r16; r16 = memd(r29+68) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ immext(#0x121C0); r4 = add(PC,#0x121EC); r3:r2 = combine(#0x2,r16); r1 = #0x72 }
	{ r21 = convert_uw2sf(r0):chop; call logmsg_function; memw(r29) = r20 }
	{ immext(#0x121C0); r4 = add(PC,#0x121E8); memw(r29+28) = r23; memw(r29+20) = r26 }
	{ r1 = #0x75; r2 = memw(r29+64) }
	{ r25 = memw(r29+56); memw(r29+24) = r25 }
	{ r17 = memw(r29+60); memw(r29+16) = r25 }
	{ r3:r2 = combine(#0x2,r16); memw(r29+12) = r2; memw(r29) = r24 }
	{ memw(r29+8) = r17; memw(r29+4) = r18 }
	{ call logmsg_function }
	{ immext(#0x121C0); r4 = add(PC,#0x121DE); r3:r2 = combine(#0x2,r16); memw(r29+12) = r23 }
	{ r1 = #0x77; memw(r29) = r24 }
	{ memw(r29+8) = r17; memw(r29+4) = r18 }
	{ call logmsg_function }
	{ immext(#0x121C0); r3 = add(PC,#0x121DF); r1 = #0x78; p0 = cmp.eq(r18,#0x1) }
	{ if (!p0) jump:nt 00015240; if (p0) r1 = #0x79 }

l000151A0:
	{ immext(#0x121C0); r3 = add(PC,#0x121C7); p0 = cmp.eq(r26,#0x1); if (p0.new) r1 = #0x7A }
	{ if (!p0) jump:nt 00015240 }

l000151B4:
	{ immext(#0x121C0); r3 = add(PC,#0x121C5); p0 = cmp.eq(r24,#0x1); if (p0.new) r1 = #0x7B }
	{ if (!p0) jump:nt 00015240 }

l000151C8:
	{ immext(#0x12180); r3 = add(PC,#0x121B1); r2 = mpyi(r24,r17); p0 = cmp.eq(r25,#0x1) }
	{ if (!p0) jump:nt 00015240 }

l000151DC:
	{ r1 = #0x7C; r3 = memw(r29+52) }
	{ r2 = mpyi(r2,r18) }
	{ immext(#0x12180); r3 = add(PC,#0x121A8); r4 = memw(r3+20) }
	{ r2 = mpyi(r2,r23) }
	{ r2 = asl(r2,#0x2); if (cmp.gtu(r2.new,r4)) jump:t 00015240 }

l00015204:
	{ r1 = #0x7D; r5 = #0x4; r3 = memd(r29+48) }
	{ immext(#0x12180); r3 = add(PC,#0x12195); p2 = cmp.gt(r19,#0xFFFFFFFF); r4 = memw(r3+20) }
	{ p0 = cmp.gtu(r5,r4); if (p0) jump:nt 00015240; p3 = cmp.gt(r21,#0xFFFFFFFF) }

l00015224:
	{ r1 = #0x7E; r5 = #0x0; r3 = memd(r29+44) }
	{ immext(#0x12180); r3 = add(PC,#0x12183); r4 = memw(r3+20) }
	{ p0 = cmp.gtu(r4,#0x3); if (p0.new) jump:nt 00015250; if (!p0) r1:r0 = combine(r16,r20) }

l00015240:
	{ call errlog_function; r2 = r16 }
	{ jump 000152C4; r0 = #0xFFFFFFFF }

l00015250:
	{ p0 = cmp.gt(r19,#0xFF); r3 = #0xFF; r7 = memd(r29+52); r6 = memd(r29+48) }
	{ memw(r7) = #0x1; memw(r7+12) = r23 }
	{ memw(r7+8) = r17; memw(r7+4) = #0x1 }
	{ if (p0) r2 = add(r3,#0x0); if (!p1) r3 = zxtb(r21); memw(r7+24) = r2 }
	{ if (!p0) r2 = zxtb(r19); if (!p3) r3 = add(r5,#0x0); r4 = memw(r6+16); r7 = memd(r29+44) }
	{ if (!p2) r2 = add(r5,#0x0); memw(r6+8) = #0x1; memw(r6+12) = #0x1 }
	{ memw(r6) = #0x1; memw(r6+4) = #0x1 }
	{ memw(r4) = r22 }
	{ r4 = memw(r7+16); memw(r7+8) = #0x1 }
	{ memw(r7+4) = #0x1; memw(r7) = #0x1 }
	{ r5 = memd(r29+36); memw(r7+12) = #0x1 }
	{ r4 = memd(r29+40); memw(r4) = r5 }
	{ callr r4; memw(r6+24) = #0x4; memw(r7+24) = #0x4 }
	{ immext(#0x12100); r4 = add(PC,#0x12111); r3:r2 = combine(#0x2,r16); r1 = #0x8C }
	{ call logmsg_function }
	{ r0 = #0x0 }

l000152C4:
	{ r17:r16 = memd(r29+112); r19:r18 = memd(r29+104) }
	{ r21:r20 = memd(r29+96); r23:r22 = memd(r29+88) }
	{ r25:r24 = memd(r29+80); r27:r26 = memd(r29+72) }
	{ dealloc_return }
000152D8                         00 40 00 7F 00 C0 00 7F         .@......

;; matmul_ref: 000152E0
matmul_ref proc
	{ immext(#0x11FC0); r4 = add(PC,#0x11FD1); allocframe(#0x38) }
	{ r6 = memw(r0+8); r5 = memw(r0+4) }
	{ r18 = r2; r17:r16 = combine(r1,r3); memd(r29+40) = r19:r18; memd(r29+48) = r17:r16 }
	{ r1 = #0xAD; r2 = memw(r5); r5 = memw(r5+4) }
	{ r6 = memw(r6); memd(r29+32) = r21:r20 }
	{ memd(r29+24) = r23:r22 }
	{ r19 = memw(r2+12); memd(r29+16) = r25:r24 }
	{ r20 = memw(r2+8); r23 = memw(r2+16) }
	{ r3:r2 = combine(#0x2,r17); r21 = memw(r5+16); r22 = memw(r5+12) }
	{ r24 = memw(r6+16); memw(r29+12) = r16 }
	{ memw(r29+8) = r18; memw(r29+4) = r19 }
	{ call logmsg_function; memw(r29) = r20 }
	{ p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 000153B4; r2 = #0x0 }

l0001533C:
	{ r5 = mpyi(r2,r22); p0 = cmp.eq(r14,#0x0); if (p0.new) jump:nt 000153AC }

l00015344:
	{ loop1(00015354,r22); r4 = r2; r3 = r21 }
	{ r4 = add(r23,mpyi(r4,r19)) }
	{ r5 = addasl(r24,r5,#0x2) }
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 000153A0; r12 = add(r19,#0xFFFFFFFF); r6 = #0x0 }

l00015360:
	{ r7:r6 = combine(r4,#0x0); r8 = add(r3,r22); p0 = cmp.gtu(r19,#0x1); r9 = memb(r3) }
	{ loop0(00015384,r12); r9 = sub(r9,r16); r13 = memb(r7++#1) }
	{ if (!p0) jump:nt 0001539C; r12 = sub(r13,r18) }

l00015384:
	{ r6 += mpyi(r9,r12); r8 = add(r8,r22); r13 = memb(r8); r14 = memb(r7++#1) }
	{ r9 = sub(r13,r16); r12 = sub(r14,r18) }

l0001539C:
	{ r6 += mpyi(r9,r12) }

l000153A0:
	{ r3 = add(r3,#0x1); nop; memw(r5++#4) = r6 }

l000153AC:
	{ r2 = add(r2,#0x1); if (!cmp.eq(r2.new,r20)) jump:t 0001533C }

l000153B4:
	{ immext(#0x11F00); r4 = add(PC,#0x11F2F); r3:r2 = combine(#0x2,r17); r1 = #0xBA }

l000153B8:
	{ r4 = add(PC,#0x2F); r3:r2 = combine(#0x2,r17); r1 = #0xBA }
	{ call logmsg_function; r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); dealloc_return }

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
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000153F8 }

l000153E8:
	{ r0 = add(PC,#0x31); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l000153F8:
	{ dealloc_return }
000153FC                                     00 C0 00 7F             ....

;; errlog_function: 00015400
;;   Called from:
;;     00014EE0 (in matmul_check_ref)
;;     00015240 (in matmul_execute)
errlog_function proc
	{ immext(#0x11E80); r0 = add(PC,#0x11E95); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; matmul_asm: 00015424
matmul_asm proc
	{ r21:r20 = combine(r0,r3); memd(r29-32) = r21:r20; allocframe(#0x50) }
	{ r4 = memw(r21+4); memd(r29+40) = r25:r24 }
	{ r5 = memw(r21+8) }
	{ r19 = r2; r16 = r1; memd(r29+72) = r17:r16; memd(r29+64) = r19:r18 }
	{ r6 = memw(r4); r5 = memw(r5) }
	{ r4 = memw(r4+4); memd(r29+48) = r23:r22 }
	{ r24 = memw(r6+8); r17 = memw(r6+16) }
	{ p0 = cmp.eq(r24,#0x1); if (p0.new) r1 = #0xDB; r23 = memw(r6+12); r18 = memw(r5+16) }
	{ if (!p0) jump:nt 000154F0; if (p0) r25 = add(r23,#0xF); r22 = memw(r4+12); memd(r29+32) = r27:r26 }

l00015470:
	{ immext(#0x120C0); r4 = add(PC,#0x120E5); memw(r29+20) = r20; memw(r29+4) = r23 }
	{ r24 = and(r25,#0xFFFFFFF0); r3:r2 = combine(#0x2,r16); memw(r29+16) = r19; memw(r29+28) = r16 }
	{ memw(r29+12) = r24; memw(r29+8) = #0x1 }
	{ call logmsg_function; memw(r29) = #0x1 }
	{ p0 = cmp.eq(r14,#0x0); if (p0.new) jump:nt 000155C0; r23 = #0xFFFFFE00; r2 = r25 }

l000154A4:
	{ r25 = r22; r27:r26 = combine(#0x0,#0x20); r16 = #0x0 }
	{ r23 &= asl(r2,#0x5); r19 = sub(#0x0,r19); r20 = sub(#0x0,r20) }

l000154BC:
	{ r5 = min(r26,r25); r1:r0 = combine(r19,r17); r4 = r18; r2 = memw(r21+8) }
	{ call gemvmpybbw_asm; r3 = r20; r2 = add(r2,r16); memw(r29) = r24 }
	{ r25 = add(r25,#0xFFFFFFE0); r16 = add(r16,r23); r18 = add(r18,#0x80) }
	{ r27 = add(r27,#0x20); if (cmp.gtu(r22,r27.new)) jump:t 000154BC }

l000154F0:
	{ immext(#0x11DC0); r4 = add(PC,#0x11DC1); r21 = memw(r4+16); memw(r29+12) = r20 }
	{ r3:r2 = combine(#0x2,r16); memw(r29) = r24 }
	{ r1 = #0xAD; memw(r29+8) = r19; memw(r29+4) = r23 }
	{ call logmsg_function }
	{ if (p0.new) jump:nt 00015594; r2 = #0x0; p0 = cmp.eq(r24,#0x0) }

l0001551C:
	{ r5 = mpyi(r2,r22); p0 = cmp.eq(r14,#0x0); if (p0.new) jump:nt 0001558C }

l00015524:
	{ loop1(00015534,r22); r4 = r2; r3 = r21 }
	{ r4 = add(r17,mpyi(r4,r23)) }
	{ r5 = addasl(r18,r5,#0x2) }
	{ p0 = cmp.eq(r15,#0x0); if (p0.new) jump:nt 00015580; r12 = add(r23,#0xFFFFFFFF); r6 = #0x0 }

l00015540:
	{ r7:r6 = combine(r4,#0x0); r8 = add(r3,r22); p0 = cmp.gtu(r23,#0x1); r9 = memb(r3) }
	{ loop0(00015564,r12); r9 = sub(r9,r20); r13 = memb(r7++#1) }
	{ if (!p0) jump:nt 0001557C; r12 = sub(r13,r19) }

l00015564:
	{ r6 += mpyi(r9,r12); r8 = add(r8,r22); r13 = memb(r8); r14 = memb(r7++#1) }
	{ r9 = sub(r13,r20); r12 = sub(r14,r19) }

l0001557C:
	{ r6 += mpyi(r9,r12) }

l00015580:
	{ r3 = add(r3,#0x1); nop; memw(r5++#4) = r6 }

l0001558C:
	{ r2 = add(r2,#0x1); if (!cmp.eq(r2.new,r24)) jump:t 0001551C }

l00015594:
	{ immext(#0x11D40); r4 = add(PC,#0x11D4F); r3:r2 = combine(#0x2,r16); r1 = #0xBA }

l00015598:
	{ r4 = add(PC,#0xF); r3:r2 = combine(#0x2,r16); r1 = #0xBA }

l000155A4:
	{ call logmsg_function }
	{ immext(#0x12000); r4 = add(PC,#0x12002); r3:r2 = combine(#0x2,r16); r1 = #0xEB }
	{ call logmsg_function; memw(r29+28) = r16 }

l000155C0:
	{ immext(#0x12000); r4 = add(PC,#0x1203B); r3 = #0x2; r1 = #0xEE }
	{ call logmsg_function; r2 = memd(r29+28); r17:r16 = memd(r29+72) }
	{ r19:r18 = memd(r29+64); r21:r20 = memd(r29+56) }
	{ r23:r22 = memd(r29+48); r25:r24 = memd(r29+40) }
	{ r27:r26 = memd(r29+32); dealloc_return }

;; fmaxf.1.0: 000155EC
;;   Called from:
;;     00014E78 (in matmul_check_ref)
;;     000150E8 (in matmul_execute)
fmaxf.1.0 proc
	{ immext(#0x38D1B700); r2 = #0x38D1B717 }
	{ jump fn00009600; r1:r0 = combine(r0,r2) }
000155FC                                     00 00 00 00             ....

;; maxpool_execute_asm: 00015600
maxpool_execute_asm proc
	{ immext(#0x780); r2 = add(PC,#0x794); jump maxpool_execute }

;; maxpool_check: 0001560C
maxpool_check proc
	{ immext(#0x12080); r4 = add(PC,#0x12087); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r1 = #0x16C; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memd(r29+32) = r19:r18; memw(r29) = r17 }
	{ r1 = #0x16D; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x5)) jump:t 00015650 }

l0001563C:
	{ r3 = add(PC,#0x34) }

l00015640:
	{ call errlog_function; r2 = r16 }

l00015644:
	{ r2 = r16 }
	{ jump 00015748; r0 = #0xFFFFFFFF }

l00015650:
	{ r1 = #0x171; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0001566C }

l00015660:
	{ r3 = add(PC,#0x27); jump 00015644; r1 = #0x16E }

l0001566C:
	{ immext(#0x12040); r4 = add(PC,#0x1206C); r3:r2 = combine(#0x3,r16); memw(r29) = r17 }
	{ call logmsg_function }
	{ r19:r18 = combine(#0x0,#0x0); r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 000156F4 }

l0001568C:
	{ r2 = memw(r17+4) }

l00015690:
	{ r2 = memw(r5+r18); if (!cmp.eq(r2.new,#0x0)) jump:t 000156AC }

l0001569C:
	{ r3 = add(PC,#0x3D); r1 = #0x174; memw(r29) = r19 }
	{ jump 00015640 }

l000156AC:
	{ immext(#0x12080); r4 = add(PC,#0x120AF); r5 = memw(r2); r6 = memw(r2+4) }
	{ r1 = #0x166; r7 = memw(r2+8); r8 = memw(r2+12) }
	{ r9 = memw(r2+24); r2 = memw(r2+16) }
	{ r3:r2 = combine(#0x3,r16); memw(r29+24) = r2; memw(r29+12) = r7 }
	{ memw(r29+20) = r9; memw(r29+16) = r8 }
	{ memw(r29+8) = r6; memw(r29+4) = r5 }
	{ call logmsg_function; memw(r29) = r19 }
	{ r18 = add(r18,#0x4); r2 = memw(r17+16) }
	{ r19 = add(r19,#0x1); if (cmp.gtu(r2,r19.new)) jump:t 0001568C }

l000156F4:
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0001572C }

l000156F8:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 00015730 }

l00015700:
	{ r3 = memw(r17+8) }
	{ r3 = add(r3,#0x4); r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 0001572C }

l00015708:
	{ r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 00015730 }

l00015714:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 00015708 }

l0001571C:
	{ r3 = add(PC,#0x13); r1 = #0x17A; memw(r29) = r4 }
	{ jump 00015640 }

l0001572C:
	{ immext(#0x12000); r4 = add(PC,#0x12016); r3:r2 = combine(#0x2,r16); r1 = #0x17D }

l00015730:
	{ r4 = add(PC,#0x16); r3:r2 = combine(#0x2,r16); r1 = #0x17D }

l0001573C:
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l00015748:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ dealloc_return }

;; maxpool_execute_ref: 00015750
maxpool_execute_ref proc
	{ immext(#0x280); r2 = add(PC,#0x290); jump maxpool_execute }
0001575C                                     00 C0 00 7F             ....

;; maxpool_execute: 00015760
;;   Called from:
;;     00015600 (in maxpool_execute_asm)
;;     00015750 (in maxpool_execute_ref)
maxpool_execute proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x88) }
	{ r3 = memw(r17+4); memd(r29+112) = r21:r20 }
	{ r21 = memb(r17+32); r4 = memw(r17+8) }
	{ memd(r29+88) = r27:r26; memd(r29+96) = r25:r24 }
	{ p0 = cmp.eq(r21,#0x0); r5 = memw(r3); r25 = memw(r3+16) }
	{ r2 = memw(r3+8); memw(r29+8) = r2 }
	{ r0 = p0; r26 = memw(r3+12) }
	{ r2 = memw(r3+4); memw(r29+24) = r2 }
	{ memd(r29+104) = r23:r22; memd(r29+120) = r19:r18 }
	{ r27 = memw(r4); memw(r29+12) = r2 }
	{ r23 = memw(r5); r2 = memw(r5+8) }
	{ r20 = memw(r5+4); r24 = memw(r5+12) }
	{ r1 = memw(r25+8); r22 = memw(r25+4) }
	{ r18 = memw(r26+4); r6 = memw(r4+4) }
	{ r7 = memw(r4+8) }
	{ memw(r29+16) = r6; memw(r29+20) = r7 }
	{ if (!p0) jump:nt 000157D4; memw(r29+28) = r0 }

l000157D0:
	{ r0 = r2; jump 000157F8 }

l000157D4:
	{ p0 = cmp.eq(r13,#0x2); if (p0.new) jump:nt 000157F4; if (p0.new) r2 = add(r1,r2); if (p0.new) r3 = memw(r26+8) }

l000157E0:
	{ p0 = cmp.eq(r13,#0x1); if (!p0.new) jump:nt 00015800; r19 = #0x0; if (p0.new) r0 = add(r1,#0x0) }

l000157EC:
	{ r0 += add(r2,#0xFFFFFFFF); jump 000157F8 }

l000157F4:
	{ r0 = sub(r2,r3) }

l000157F8:
	{ call fn00009760 }
	{ r19 = r0 }

l00015800:
	{ p0 = cmp.eq(r13,#0x2); if (p0.new) jump:nt 00015830; r2 = add(r22,r20); if (p0.new) r1 = add(r22,#0x0) }

l0001580C:
	{ p0 = cmp.eq(r13,#0x1); if (p0.new) jump:nt 00015828; if (!p0) r1:r0 = combine(r22,r22) }

l00015814:
	{ r21 = #0x0; r0 = memd(r29+28) }
	{ p0 = r0; if (!p0.new) jump:nt 0001583C; if (!p0) r1:r0 = combine(r22,r20) }

l00015824:
	{ jump 00015834 }

l00015828:
	{ r0 += add(r20,#0xFFFFFFFF); jump 00015834 }

l00015830:
	{ r0 = sub(r2,r18) }

l00015834:
	{ call fn00009760 }
	{ r21 = r0 }

l0001583C:
	{ r4 = add(r29,#0x20); r5 = add(r29,#0x38); r3:r2 = combine(#0x0,#0x0); memw(r29+32) = r17 }
	{ r20 = add(r4,#0x8); r22 = add(r5,#0x8); memw(r4+4) = #0xFFFFFF81 }
	{ r1:r0 = combine(#0x0,r20); memd(r29+64) = r3:r2; memd(r29+56) = r3:r2 }
	{ memd(r29+72) = r3:r2; memw(r4+20) = #0x0 }
	{ memw(r4+16) = #0x0; memw(r4+12) = #0x0 }
	{ call fn00009740; memw(r4+8) = #0x0; memw(r29+56) = r17 }
	{ call fn00009740; r1:r0 = combine(#0x0,r22) }
	{ immext(#0x11D80); r4 = add(PC,#0x11DBD); r3:r2 = combine(#0x2,r16); r1 = #0x137 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r2 = memw(r26); if (!cmp.eq(r2.new,#0x1)) jump:t 000158AC }

l00015898:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 000158B0 }

l000158A0:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 000158B0 }

l000158A8:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 000158CC }

l000158AC:
	{ immext(#0x11D80); r3 = add(PC,#0x11D9F); r1 = #0x13C }

l000158B0:
	{ r3 = add(PC,#0x1F); r1 = #0x13C }

l000158B8:
	{ call errlog_function; r2 = r16 }

l000158BC:
	{ r2 = r16 }
	{ jump 000159CC; r0 = #0xFFFFFFFF }
000158C8                         02 57 18 ED                     .W..    

l000158CC:
	{ r1 = #0x13E; r4 = memw(r27+20) }
	{ r2 = mpyi(r2,r19) }
	{ r3 = mpyi(r2,r21); if (!cmp.gtu(r3.new,r4)) jump:t 000158EC }

l000158E4:
	{ r3 = add(PC,#0x3); jump 000158BC }

l000158EC:
	{ r1 = #0x13F; r2 = memb(r17+32); if (!cmp.eq(r2.new,#0x0)) jump:t 00015904 }

l000158FC:
	{ r3 = add(PC,#0x39); jump 000158BC }

l00015904:
	{ r2 = add(r29,#0x20); r18 = memw(r29+8); memw(r27+4) = r21 }
	{ r1:r0 = combine(r18,r16); memw(r27) = r23; memw(r27+12) = r24 }
	{ memw(r27+8) = r19; memw(r27+24) = r3 }
	{ call nn_os_work_for_vector }
	{ callr r18; r1 = add(r29,#0x38); r0 = r16 }
	{ call fn000096A0; r0 = r20 }
	{ r5 = memd(r29+12); r4 = memd(r29+16) }
	{ r2 = memw(r5) }
	{ r3 = memw(r5+4); memb(r4+1) = r3.new }
	{ r2 = memw(r5+8) }
	{ memw(r4+8) = r2 }
	{ r7 = memw(r5+12) }
	{ memw(r4+12) = r7 }
	{ r2 = memw(r5+24) }
	{ r6 = memw(r4+20); if (cmp.gtu(r2,r6.new)) jump:t 00015974 }

l0001596C:
	{ call fn00009560; r2 = memw(r5+24); r1 = memw(r5+16) }

l00015974:
	{ r4 = memd(r29+24); r5 = memd(r29+20) }
	{ r2 = memw(r4) }
	{ r3 = memw(r4+4); memb(r5+1) = r3.new }
	{ r2 = memw(r4+8) }
	{ memw(r5+8) = r2 }
	{ r7 = memw(r4+12) }
	{ memw(r5+12) = r7 }
	{ r2 = memw(r4+24) }
	{ r6 = memw(r5+20); if (cmp.gtu(r2,r6.new)) jump:t 000159B0 }

l000159A8:
	{ call fn00009560; r2 = memw(r4+24); r1 = memw(r4+16) }

l000159B0:
	{ immext(#0x11CC0); r4 = add(PC,#0x11CD3); r3:r2 = combine(#0x2,r16); r1 = #0x14B }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l000159CC:
	{ r17:r16 = memd(r29+128); r19:r18 = memd(r29+120) }
	{ r21:r20 = memd(r29+112); r23:r22 = memd(r29+104) }
	{ r25:r24 = memd(r29+96); r27:r26 = memd(r29+88) }
	{ dealloc_return }

;; maxpool_execute_slice_ref: 000159E0
maxpool_execute_slice_ref proc
	{ allocframe(#0x98) }
	{ r2 = memw(r1); r3 = memw(r1+4) }
	{ memd(r29+136) = r19:r18; memd(r29+144) = r17:r16 }
	{ r7 = memw(r2+4); r18 = memb(r2+32) }
	{ r2 = memw(r2+8); memw(r29+16) = r3 }
	{ p0 = cmp.eq(r18,#0x0); r16 = memw(r7); r4 = memw(r7+16) }
	{ r3 = memw(r7+12); memd(r29+120) = r23:r22 }
	{ r5 = memw(r16+8) }
	{ memd(r29+112) = r25:r24; memd(r29+128) = r21:r20 }
	{ r0 = r5; r21 = r5; memd(r29+104) = r27:r26 }
	{ r1 = p0; r22 = memw(r4+4); memw(r29) = r1 }
	{ r19 = memw(r2); r17 = memw(r4+8) }
	{ if (!p0) r2 = add(r17,r21); r24 = memw(r16+12); r23 = memw(r16+4) }
	{ r25 = memw(r3+4); r26 = memw(r3+8) }
	{ r7 = memw(r16); memb(r29+3) = r7.new }
	{ if (p0) jump:nt 00015A64 }

l00015A4C:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 00015A64; if (p0.new) r0 = sub(r2,r26) }

l00015A54:
	{ p0 = cmp.eq(r10,#0x1); if (!p0.new) jump:nt 00015A70; r20 = #0x0; if (p0.new) r0 = add(r17,#0x0) }

l00015A60:
	{ r0 += add(r21,#0xFFFFFFFF) }

l00015A64:
	{ call fn00009760; r1 = r17 }
	{ r20 = r0 }

l00015A70:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 00015AA8; r2 = add(r22,r23); if (p0.new) r1 = add(r22,#0x0) }

l00015A7C:
	{ p0 = cmp.eq(r10,#0x1); if (p0.new) jump:nt 00015AA0; if (!p0) r1:r0 = combine(r22,r22) }

l00015A84:
	{ r2 = #0x0; r0 = memw(r29+96); memb(r29+17) = r2.new }
	{ if (!p0.new) jump:nt 00015AB8; if (!p0) r1:r0 = combine(r22,r23) }

l00015A9C:
	{ jump 00015AAC }

l00015AA0:
	{ r0 += add(r23,#0xFFFFFFFF); jump 00015AAC }

l00015AA8:
	{ r0 = sub(r2,r25) }

l00015AAC:
	{ call fn00009760 }
	{ memw(r29+68) = r0 }
	{ r3 = sub(r25,r23); r2 = memw(r29+12); if (!cmp.gt(r2.new,#0x0)) jump:nt 00015D28 }

l00015AB8:
	{ r2 = memw(r29+12); if (!cmp.gt(r2.new,#0x0)) jump:nt 00015D2C }

l00015AC4:
	{ r4 = sub(r26,r21); r6 = r20; r2 = memd(r29+68) }
	{ r6 = add(r4,mpyi(r6,r17)); r4 = sub(#0xFFFFFFFF,r21); r2 = r2; r5 = memd(r29+16) }
	{ r2 = add(r3,mpyi(r2,r22)); r1 = mpyi(r24,r21); r3 = #0x0 }
	{ r7 = mpyi(r5,r22); r6 += lsr(r6,#0x1F); r8 = sub(#0xFFFFFFFF,r23); memw(r29+52) = r8 }
	{ r2 += lsr(r2,#0x1F); r15 = asr(r6,#0x1) }
	{ r3 = #0x0; r4 = r21; memw(r29+48) = r3; memw(r29+96) = r4 }
	{ r5 = mpyi(r21,r24); r2 = asr(r2,#0x1); r9 = memw(r19+16) }
	{ r1 = sub(r7,r2); r8 = r26; memw(r29+32) = r8; memw(r29+64) = r1 }
	{ r6 = add(r15,sub(#0x7F,r26)); r12 = memw(r16+16) }
	{ r2 = add(r2,sub(#0x7F,r7)); r9 = #0x0; memw(r29+72) = r2; memw(r29+92) = r9 }
	{ r1 = sub(#0x0,r15); r2 = sub(r2,r25); memw(r29+8) = r1; memw(r29+44) = r23 }
	{ memw(r29+36) = r25; memw(r29+20) = r9 }
	{ memw(r29+28) = r6; memw(r29+24) = r1 }
	{ memw(r29+4) = r2 }

l00015B58:
	{ r2 = memw(r29+16) }
	{ r6 = memw(r29+68); if (!cmp.gt(r6.new,r2)) jump:t 00015D0C }

l00015B68:
	{ r2 = combine(r7.l,r22.l); r1 = memd(r29+8); memw(r29+88) = r6 }
	{ r6 = memw(r29+68); r13 = memw(r29+16) }
	{ r1:r0 = combine(r7,r2); r9 = memw(r29+20); memw(r29+84) = r1 }
	{ r6 = mpyi(r9,r6); memb(r29+10) = r6.new }
	{ memd(r29+56) = r1:r0 }

l00015B94:
	{ r2 = memd(r29+52); r6 = memd(r29+84) }
	{ r6 = add(r6,r2); r22 = memd(r29+76); r2 = memd(r29+88) }
	{ r2 -= asl(r22,#0x1); r13 = memw(r29+80); r7 = memw(r29+68) }
	{ p0 = cmp.gt(r7,r13); memw(r29+84) = r6; memw(r29+88) = r2 }
	{ if (!p0) jump:nt 00015D0C }

l00015BB8:
	{ r2 = r13 }
	{ r6 = add(r2,#0x2); r7 = memd(r29+72); r1:r0 = memd(r29+56) }
	{ r6 = mpyi(r6,r22); memw(r29+80) = r6 }
	{ r6 = sub(r6,r7); r7 = memw(r29+64) }
	{ r6 = add(r12,mpyi(r6,r7)) }
	{ l2fetch(r6,r1:r0) }
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 00015B94; if (p0.new) r13 = memw(r29+88); if (p0.new) r9 = memw(r29+40) }

l00015BE8:
	{ r6 = memd(r29+76); r7 = memd(r29+84) }
	{ r6 = mpyi(r2,r6); r0 = max(r7,r3); r2 = add(r2,r9); r7 = memw(r29+32) }
	{ r19 = mpyi(r2,r20); r14 = memw(r29+72); r2 = memw(r29+48) }
	{ r9 = max(r13,r7); r6 = sub(r6,r14); r2 = add(r2,r0); r7 = memd(r29+36) }
	{ r25 = max(r6,r3); r16 = mpyi(r4,r2); r6 = add(r6,r7); r0 = memd(r29+44) }
	{ r13 = #0x0; r9 = sub(#0xFFFFFFFF,r9); r23 = memd(r29+24); r1 = memd(r29+28) }
	{ r2 = min(r0,r6) }
	{ r7 = mpyi(r13,r17); if (!p0.new) jump:nt 00015CF8; r0 = add(r13,r19); p0 = cmp.gt(r24,#0x0) }

l00015C40:
	{ r14 = max(r23,r3); r21 = r24; r6 = memw(r29+96); r10 = memw(r29+92) }
	{ r7 = sub(r7,r15); r18 = add(r16,r14) }
	{ r28 = max(r1,r6); r0 = add(r10,mpyi(r0,r24)); r6 = #0x0 }
	{ r10 = max(r7,r3); r21 = add(r12,mpyi(r21,r18)); r28 = sub(#0xFFFFFFFF,r28); r7 = add(r7,r8) }
	{ r14 = min(r4,r7); r11 = sub(r28,r14) }

l00015C7C:
	{ if (!p0.new) jump:nt 00015CE4; r28 = sub(r9,r25); r18 = #0x0; p0 = cmp.gt(r2,r25) }

l00015C8C:
	{ loop1(00015C94,r28); r18 = #0x0; r7 = r21 }
	{ if (!p0.new) jump:nt 00015CD8; r26 = add(r11,#0xFFFFFFFF); r22 = and(r18,#0xFF); p0 = cmp.gt(r14,r10) }

l00015CA4:
	{ r27 = add(r7,r24); p1 = cmp.gtu(r11,#0x1); r28 = memb(r7) }
	{ loop0(00015CBC,r26); p0 = cmp.gtu(r28,r22) }
	{ if (!p1) jump:nt 00015CD4 }

l00015CBC:
	{ if (p0) r18 = add(r28,#0x0); r27 = add(r27,r24); r28 = memb(r27) }
	{ r22 = and(r18,#0xFF) }
	{ p0 = cmp.gtu(r28,r22); nop }

l00015CD4:
	{ if (p0) r18 = add(r28,#0x0) }

l00015CD8:
	{ r7 = add(r7,r5); nop; nop }

l00015CE4:
	{ r6 = add(r6,#0x1); r21 = add(r21,#0x1); memb(r0++#1) = r18 }
	{ if (!p0.new) jump:nt 00015C7C; p0 = cmp.eq(r6,r24) }

l00015CF8:
	{ r23 = add(r23,r17); r1 = sub(r1,r17); r13 = add(r13,#0x1); if (cmp.eq(r13.new,r20)) jump:nt 00015B94 }

l00015D0C:
	{ r7 = memd(r29+20); r1 = memd(r29+12) }
	{ r7 = add(r7,#0x1); r2 = memd(r29+44); r6 = memd(r29+48) }
	{ p0 = cmp.eq(r7,r1); r6 = add(r6,r2); memw(r29+20) = r7 }
	{ if (!p0) jump:nt 00015B58; memw(r29+48) = r6 }

l00015D28:
	{ r1 = #0x1; r2 = memd(r29); r17:r16 = memd(r29+144) }

l00015D2C:
	{ r2 = memd(r29); r17:r16 = memd(r29+144) }

l00015D30:
	{ r0 = add(r2,#0x8); r19:r18 = memd(r29+136); r21:r20 = memd(r29+128) }
	{ r23:r22 = memd(r29+120); r25:r24 = memd(r29+112) }
	{ jump 00009730; r27:r26 = memd(r29+104); deallocframe }

;; logmsg_function: 00015D4C
;;   Called from:
;;     00015620 (in maxpool_check)
;;     0001567C (in maxpool_check)
;;     000156E0 (in maxpool_check)
;;     0001573C (in maxpool_check)
;;     00015884 (in maxpool_execute)
;;     000159C0 (in maxpool_execute)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00015D6C }

l00015D5C:
	{ r0 = add(PC,#0x3C); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l00015D6C:
	{ dealloc_return }

;; errlog_function: 00015D70
;;   Called from:
;;     00015640 (in maxpool_check)
;;     000158B8 (in maxpool_execute)
errlog_function proc
	{ immext(#0x11880); r0 = add(PC,#0x118A4); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; maxpool_execute_slice_asm: 00015D94
maxpool_execute_slice_asm proc
	{ allocframe(#0x88) }
	{ r2 = memw(r1); r3 = memw(r1+4) }
	{ memd(r29+120) = r19:r18; memd(r29+96) = r25:r24 }
	{ r7 = memw(r2+4); r19 = memb(r2+32) }
	{ r2 = memw(r2+8); memw(r29+20) = r3 }
	{ p0 = cmp.eq(r19,#0x0); r24 = memw(r7); r4 = memw(r7+16) }
	{ r3 = memw(r7+12); memd(r29+128) = r17:r16 }
	{ r17 = memw(r24+8) }
	{ memd(r29+112) = r21:r20; memd(r29+88) = r27:r26 }
	{ r0 = r17; r16 = memw(r2); r2 = memw(r4+4) }
	{ r18 = memw(r4+8); r20 = memw(r24+12) }
	{ r21 = memw(r24+4); r27 = memw(r3+8) }
	{ r1 = p0; r7 = memw(r24); memw(r29+4) = r1 }
	{ r2 = memw(r3+4); memw(r29+72) = r2 }
	{ memd(r29+104) = r23:r22; memw(r29+16) = r7 }
	{ if (!p0) r2 = add(r18,r17); memw(r29+40) = r2; memw(r29+80) = r1 }
	{ if (p0) jump:nt 00015E18 }

l00015E04:
	{ p0 = cmp.eq(r11,#0x2); if (p0.new) jump:nt 00015E18; if (p0.new) r0 = sub(r2,r27) }

l00015E0C:
	{ p0 = cmp.eq(r11,#0x1); if (!p0.new) jump:nt 00015E24; r22 = #0x0; r0 = r18 }

l00015E14:
	{ r0 += add(r17,#0xFFFFFFFF) }

l00015E18:
	{ call fn00009760; r1 = r18 }
	{ r22 = r0 }

l00015E24:
	{ p0 = cmp.eq(r11,#0x2); if (p0.new) jump:nt 00015E54; nop }

l00015E2C:
	{ p0 = cmp.eq(r11,#0x1); if (p0.new) jump:nt 00015E48; if (p0.new) r1 = memw(r29+72) }

l00015E34:
	{ r0 = #0x0; r1 = memd(r29+80) }
	{ p0 = r1; if (!p0.new) jump:nt 00015E64 }

l00015E40:
	{ r0 = r13; jump 00015E60; r1 = memw(r29+72) }

l00015E48:
	{ r0 = r1 }
	{ r0 += add(r21,#0xFFFFFFFF); jump 00015E60 }

l00015E54:
	{ r1 = memd(r29+72); r3 = memd(r29+40) }
	{ r2 = add(r1,r21) }
	{ r0 = sub(r2,r3) }

l00015E60:
	{ call fn00009760 }

l00015E64:
	{ r4 = add(r0,#0xFFFFFFFF); r3 = sub(r27,r17); r2 = memw(r29+16); if (!cmp.gt(r2.new,#0x0)) jump:nt 0001600C }

l00015E78:
	{ r2 = add(r22,#0xFFFFFFFF); r5 = memd(r29+40); r1 = memd(r29+72) }
	{ r2 = add(r3,mpyi(r2,r18)); r7 = mpyi(r20,r17); r5 = sub(r5,r21); r26 = and(r20,#0x7F) }
	{ r4 = add(r5,mpyi(r4,r1)); r5 = mpyi(r0,r22); r8 = #0x0; r3 = memw(r16+16) }
	{ r2 += lsr(r2,#0x1F); r1 = asl(r6,#0x1); r23 = memw(r24+16); memb(r29+11) = r1.new }
	{ r2 = asr(r2,#0x1); r1 = memd(r29+20); memw(r29+36) = r21 }
	{ r5 = mpyi(r5,r20); r6 = add(r3,mpyi(r6,r1)); r2 = sub(#0x0,r2); memb(r29+3) = r5.new }
	{ memw(r29+68) = r22; memw(r29+56) = r7 }
	{ r3 = asr(r4,#0x1); memw(r29+60) = r0; memw(r29+8) = r5 }
	{ memw(r29+24) = r8; memw(r29+28) = r6 }
	{ memw(r29+64) = r3; memw(r29+32) = r2 }

l00015EE8:
	{ r2 = memw(r29+20) }
	{ r3 = memw(r29+60); if (!cmp.gt(r3.new,r2)) jump:t 00015FF0 }

l00015EF8:
	{ r2 = combine(r3.l,r2.l); r7 = memd(r29+24); r5 = memd(r29+8) }
	{ r19 = mpyi(r5,r7); r1:r0 = combine(r3,r2); r4 = memw(r29+28); memb(r29+20) = r4.new }
	{ jump 00015F30 }

l00015F18:
	{ r6 = memd(r29+76); r7 = memd(r29+60) }
	{ p0 = cmp.gt(r7,r6); r2 = memd(r29+44); r3 = memd(r29+80) }
	{ r3 = add(r3,r2) }
	{ if (!p0) jump:nt 00015FF0; memw(r29+80) = r3 }

l00015F30:
	{ r2 = r6 }
	{ r4 = add(r2,#0x2); r3 = memd(r29+72); r7 = memd(r29+64) }
	{ r3 = mpyi(r4,r3); r1:r0 = memd(r29+48); memw(r29+76) = r4 }
	{ r3 = sub(r3,r7); r4 = memw(r29+56) }
	{ r3 = add(r23,mpyi(r3,r4)) }
	{ l2fetch(r3,r1:r0) }
	{ r3 = memw(r29+68); if (!cmp.gt(r3.new,#0x0)) jump:nt 00015F18 }

l00015F60:
	{ r4 = memd(r29+40); r16 = memd(r29+68) }
	{ r2 = mpyi(r2,r3); r3 = #0x0; r24 = memw(r29+80); r22 = memw(r29+32) }
	{ r2 = sub(r2,r7); r7 = memw(r29+36) }
	{ r3 = max(r2,r3); r2 = add(r2,r4) }
	{ r2 = min(r7,r2); r21 = mpyi(r3,r17) }
	{ r25 = sub(r2,r3) }
	{ r3 = add(r27,r22); p0 = cmp.eq(r26,#0x0); r2 = #0x0; r1 = r23 }
	{ r2 = max(r22,r2); r3 = min(r17,r3); if (!p0) r0 = add(r24,#0x0) }
	{ r4 = add(r2,r21); r3 = sub(r3,r2); if (!p0) r2 = add(r20,#0x0); if (p0) r0 = add(r24,#0x0) }
	{ r7 = mpyi(r4,r20); if (p0.new) r5:r4 = combine(r17,r25); if (p0) r2 = add(r20,#0x0) }
	{ r1 += add(r7,r19); if (!p0) jump:nt 00015FD8; if (p0) r5:r4 = combine(r17,r25) }

l00015FD0:
	{ call maxpool_aligned_hvx }
	{ jump 00015FDC }

l00015FD8:
	{ call maxpool_nonaligned_hvx }

l00015FDC:
	{ r24 = add(r24,r20); r22 = add(r22,r18); r16 = add(r16,#0xFFFFFFFF); if (cmp.eq(r16.new,#0x0)) jump:nt 00015F18 }

l00015FF0:
	{ r3 = memd(r29+24); r7 = memd(r29+16) }
	{ r3 = add(r3,#0x1); r2 = memd(r29+28); r4 = memd(r29+12) }
	{ p0 = cmp.eq(r3,r7); r2 = add(r2,r4); memw(r29+24) = r3 }
	{ if (!p0) jump:nt 00015EE8; memw(r29+28) = r2 }

l0001600C:
	{ r1 = #0x1; r2 = memd(r29+4); r17:r16 = memd(r29+128) }
	{ r0 = add(r2,#0x8); r19:r18 = memd(r29+120); r21:r20 = memd(r29+112) }
	{ r23:r22 = memd(r29+104); r25:r24 = memd(r29+96) }
	{ jump 00009730; r27:r26 = memd(r29+88); deallocframe }
00016030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; min_execute: 00016040
min_execute proc
	{ immext(#0x150C0); r2 = add(PC,#0x150D0); immext(#0x7F800000); r3 = #0x7F800000 }
	{ jump nn_reduction_float; immext(#0xFFFFFFC0); r2 = memw(r2-16) }
0001605C                                     00 C0 00 7F             ....

;; minmax_check: 00016060
minmax_check proc
	{ immext(#0x11800); r4 = add(PC,#0x1182A); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x75; r16 = r1; r17 = r0 }
	{ immext(#0x117C0); r0 = add(PC,#0x117FA); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ immext(#0x11800); r3 = add(PC,#0x1181F); r1 = #0x76 }
	{ r2 = memw(r17+4); if (cmp.eq(r2.new,#0x0)) jump:nt 000160FC }

l0001609C:
	{ r3 = add(PC,#0x17); r1 = #0x77 }
	{ r4 = memw(r17+8); if (cmp.eq(r4.new,#0x0)) jump:nt 000160FC }

l000160B0:
	{ r3 = add(PC,#0x10); r1 = #0x78 }
	{ r2 = memw(r2); if (cmp.eq(r2.new,#0x0)) jump:nt 000160FC }

l000160C4:
	{ r3 = add(PC,#0x9); r1 = #0x79 }
	{ r2 = memw(r4); if (cmp.eq(r2.new,#0x0)) jump:nt 000160FC }

l000160D8:
	{ r3 = add(PC,#0x3); r1 = #0x7A }
	{ r2 = memw(r17+16); if (cmp.gtu(r2.new,#0x3)) jump:t 000160FC }

l000160EC:
	{ r3 = add(PC,#0x2F); r1 = #0x7B }
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00016114 }

l000160FC:
	{ immext(#0x11740); r0 = add(PC,#0x11772); call errlog_function; r2 = r16 }

l00016100:
	{ r0 = add(PC,#0x32); call fn00016564; r2 = r16 }

l0001610C:
	{ r0 = #-0x1; r17:r16 = memd(r29+8) }
	{ dealloc_return }

l00016114:
	{ immext(#0x11740); r0 = add(PC,#0x1175A); r1 = #0x7C; r2 = r16 }
	{ immext(#0x117C0); r4 = add(PC,#0x117C2); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }
0001613C                                     00 C0 00 7F             ....

;; max_execute: 00016140
max_execute proc
	{ immext(#0x14FC0); r2 = add(PC,#0x14FD0); immext(#0xFF800000); r3 = #0xFF800000 }
	{ jump nn_reduction_float; immext(#0xFFFFFFC0); r2 = memw(r2-64) }
0001615C                                     00 C0 00 7F             ....

;; minimum_execute: 00016160
minimum_execute proc
	{ immext(#0x14F80); r2 = add(PC,#0x14FB0) }
	{ jump broadcast_elementwise_execute_f; immext(#0xFFFFFFC0); r2 = memw(r2-16) }

;; maximum_execute: 00016174
maximum_execute proc
	{ immext(#0x14F80); r2 = add(PC,#0x14F9C) }
	{ jump broadcast_elementwise_execute_f; immext(#0xFFFFFFC0); r2 = memw(r2-64) }

;; broadcast_elementwise_execute_f: 00016188
;;   Called from:
;;     00016168 (in minimum_execute)
;;     0001617C (in maximum_execute)
broadcast_elementwise_execute_f proc
	{ r25:r24 = combine(r1,r0); memd(r29-48) = r25:r24; allocframe(#0xA8) }
	{ r3 = memw(r24+4); memd(r29+136) = r23:r22 }
	{ r2 = #0x0; r18 = r2; memd(r29+152) = r19:r18 }
	{ memd(r29+144) = r21:r20 }
	{ r23 = memw(r3); memd(r29+160) = r17:r16 }
	{ r19 = memw(r3+4); r4 = memw(r24+8) }
	{ r12 = memw(r23+4); memd(r29+120) = r27:r26 }
	{ p0 = cmp.eq(r12,#0x1); r0 = memw(r23); r7 = memw(r19) }
	{ p1 = cmp.eq(r0,#0x1); r5 = memw(r19+4); r21 = memw(r23+8) }
	{ r6 = mux(p0,r5,r12); p2 = cmp.eq(r21,#0x1); r9 = memw(r19+8); memw(r29+104) = r5 }
	{ r10 = p1; r5 = mux(p1,r7,r0); r8 = memw(r23+12) }
	{ r3 = mpyi(r5,r6); p1 = cmp.eq(r8,#0x1); r0 = memw(r19+12); memw(r29+72) = r7 }
	{ r7 = mux(p2,r9,r21); r17 = memw(r4) }
	{ r3 = mpyi(r3,r7); r10 = p2; memw(r29+100) = r10; memw(r29+60) = r12 }
	{ r16 = mux(p1,r0,r8); memw(r29+68) = r5; memw(r29+88) = r6 }
	{ r3 = mpyi(r3,r16); memw(r29+64) = r9; memw(r29+108) = r10 }
	{ memw(r29+112) = r7; memw(r29+92) = r0 }
	{ r22 = asl(r3,#0x2); memw(r29+84) = r2 }
	{ if (p0) jump:nt 00016238 }

l00016230:
	{ r2 = mpyi(r8,r21) }
	{ memw(r29+84) = r2 }

l00016238:
	{ immext(#0x11540); r0 = add(PC,#0x11551); r1 = #0xBC; r2 = r25 }
	{ immext(#0x11540); r4 = add(PC,#0x11560); r27 = memw(r17+16); memw(r29+96) = r8 }
	{ r20 = memw(r19+16); r26 = memw(r23+16) }
	{ call logmsg_function; memw(r29) = r24 }
	{ r1 = #0xBC; r2 = memw(r17+20); if (!cmp.gtu(r22,r2.new)) jump:t 00016294 }

l00016278:
	{ r0 = add(PC,#0x15); r2 = r25 }
	{ immext(#0x11540); r3 = add(PC,#0x11546) }

l00016288:
	{ call errlog_function }
	{ jump 00016524; r0 = #0xFFFFFFFF }

l00016294:
	{ r2 = memw(r19); r5 = memw(r23) }
	{ p0 = cmp.eq(r5,r2); r8 = memw(r23+12); r7 = memw(r23+8) }
	{ r6 = memw(r23+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+56) = r22; memw(r29+76) = r26 }
	{ if (p0) jump:nt 000162CC; memw(r29+80) = r20 }

l000162C4:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 000162CC }

l000162C8:
	{ p0 = cmp.eq(r2,#0x1); if (!p0.new) jump:nt 00016308 }

l000162CC:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 000162DC; nop }

l000162D4:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 000162DC }

l000162D8:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 00016308 }

l000162DC:
	{ if (p0.new) jump:nt 000162F0; p0 = cmp.eq(r7,r9) }

l000162E4:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 000162F0 }

l000162E8:
	{ if (!p0.new) jump:nt 00016308; p0 = cmp.eq(r9,#0x1) }

l000162F0:
	{ if (p0.new) jump:nt 0001633C; p0 = cmp.eq(r8,r12) }

l000162F8:
	{ if (p0.new) jump:nt 0001633C; p0 = cmp.eq(r8,#0x1) }

l00016300:
	{ if (p0.new) jump:nt 0001633C; p0 = cmp.eq(r12,#0x1) }

l00016308:
	{ immext(#0x11480); r0 = add(PC,#0x11481); r1 = #0xBC; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+12) = r8 }
	{ immext(#0x11480); r3 = add(PC,#0x114B4); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ r2 = r25; memw(r29+16) = r2; memw(r29+4) = r6 }
	{ jump 00016288; memw(r29) = r5 }

l0001633C:
	{ immext(#0x11440); r0 = add(PC,#0x1144D); r23 = memd(r29+112); memw(r29+44) = r16 }
	{ immext(#0x11480); r4 = add(PC,#0x114BC); r1 = #0xBC; r19 = memw(r29+68) }
	{ r20 = memd(r29+88); memw(r29+40) = r23 }
	{ memw(r29+36) = r20; memw(r29+32) = r19 }
	{ memw(r29+52) = r24; memw(r29+28) = r12 }
	{ memw(r29+48) = r25; memw(r29+24) = r9 }
	{ memw(r29+20) = r3; memw(r29+8) = r7 }
	{ r2 = r25; memw(r29+16) = r2; memw(r29+4) = r6 }
	{ memw(r29+12) = r8; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r19,#0x0); r4 = memw(r29+80) }
	{ r5 = memd(r29+76); r2 = memd(r29+56) }
	{ memw(r17+24) = r2; memw(r17) = r19 }
	{ memw(r17+4) = r20; memw(r17+8) = r23 }
	{ if (!p0) jump:nt 000164FC; memw(r17+12) = r16 }

l000163A4:
	{ r1 = #0x0; r2 = memw(r29+60); r9 = memw(r29+64) }
	{ r2 = mpyi(r21,r2); p0 = cmp.eq(r9,#0x1); r6 = memd(r29+104); r0 = memd(r29+108) }
	{ r3 = mpyi(r9,r6); r7 = memw(r29+72); r8 = memw(r29+92) }
	{ r9 = mpyi(r8,r9); p2 = r0; r13 = mux(p0,#0x0,r8); r0 = memw(r29+100) }
	{ r3 = mpyi(r3,r8); p1 = cmp.eq(r7,#0x1); r7 = memd(r29+96); memw(r29+64) = r1 }
	{ r12 = mux(p2,#0x0,r7); p2 = cmp.eq(r6,#0x1); r6 = #0x0; r25 = !cmp.eq(r7,00000001) }
	{ r2 = mpyi(r2,r7); p0 = r0; r26 = !cmp.eq(r8,00000001); if (p2) r9 = add(r6,#0x0) }
	{ if (p1) r3 = add(r6,#0x0); if (p0) r2 = add(r6,#0x0); memw(r29+108) = r12; memw(r29+72) = r9 }
	{ memw(r29+104) = r13; memw(r29+60) = r2 }
	{ memw(r29+56) = r3 }

l00016420:
	{ r3 = r4; r7:r6 = combine(#0x0,r5); r2 = memd(r29+88); memw(r29+76) = r5 }
	{ p0 = cmp.gt(r2,#0x0); memw(r29+80) = r4 }
	{ if (!p0) jump:nt 000164DC }

l00016438:
	{ r2 = memw(r29+112); if (cmp.gt(r2.new,#0x0)) jump:t 0001644C }

l00016444:
	{ jump 000164BC; memw(r29+100) = r3 }

l0001644C:
	{ r19 = r3; r23 = #0x0; r21 = r6 }
	{ memw(r29+92) = r7; memw(r29+96) = r6 }
	{ memw(r29+100) = r3 }

l0001645C:
	{ r20 = r21; r24 = r27; r17 = r19; r22 = r16 }
	{ p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt 000164A4 }

l0001646C:
	{ nop }
	{ nop; nop; nop; nop }

l00016480:
	{ r22 = add(r22,#0xFFFFFFFF); callr r18; r0 = memw(r20); r1 = memw(r17) }
	{ r20 = addasl(r20,r25,#0x2); r17 = addasl(r17,r26,#0x2); r24 = add(r24,#0x4); memw(r24) = r0 }
	{ p0 = cmp.eq(r14,#0x0); if (!p0.new) jump:nt 00016480 }

l000164A0:
	{ r27 = addasl(r27,r16,#0x2) }

l000164A4:
	{ r23 = add(r23,#0x1); r2 = memd(r29+108); r7 = memd(r29+104) }
	{ r21 = addasl(r21,r2,#0x2) }
	{ r19 = addasl(r19,r7,#0x2); r2 = memw(r29+112); if (!cmp.eq(r2.new,r23)) jump:t 0001645C }

l000164BC:
	{ r2 = memd(r29+84); r6 = memd(r29+96) }

l000164C0:
	{ r5 = memd(r29+72); r3 = memd(r29+100) }
	{ r6 = addasl(r6,r2,#0x2); r7 = memd(r29+92); r2 = memd(r29+88) }
	{ r3 = addasl(r3,r5,#0x2); r4 = memd(r29+80); r5 = memd(r29+76) }
	{ r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r2)) jump:t 00016438 }

l000164DC:
	{ r2 = memd(r29+60); r7 = memd(r29+56) }

l000164E0:
	{ r3 = memw(r29+64) }
	{ r5 = addasl(r5,r2,#0x2); r4 = addasl(r4,r7,#0x2); r3 = r3; r2 = memd(r29+68) }
	{ p0 = cmp.eq(r3,r2); memw(r29+64) = r3 }
	{ if (!p0) jump:nt 00016420 }

l000164FC:
	{ immext(#0x11280); r0 = add(PC,#0x1128D); r2 = memw(r29+52); memb(r29) = r2.new }
	{ r4 = add(PC,#0x28); r1 = #0xBC; r2 = memw(r29+48) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00016524:
	{ r17:r16 = memd(r29+160); r19:r18 = memd(r29+152) }
	{ r21:r20 = memd(r29+144); r23:r22 = memd(r29+136) }
	{ r25:r24 = memd(r29+128); r27:r26 = memd(r29+120) }
	{ dealloc_return }

;; logmsg_function: 00016538
;;   Called from:
;;     00016080 (in minmax_check)
;;     00016124 (in minmax_check)
;;     00016260 (in broadcast_elementwise_execute_f)
;;     00016384 (in broadcast_elementwise_execute_f)
;;     0001651C (in broadcast_elementwise_execute_f)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00016558 }

l00016548:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00016558:
	{ dealloc_return }
0001655C                                     00 C0 00 7F             ....

;; errlog_function: 00016560
;;   Called from:
;;     000160FC (in minmax_check)
;;     00016288 (in broadcast_elementwise_execute_f)
;;     00016758 (in nn_reduction_float)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }

;; fn00016564: 00016564
;;   Called from:
;;     00016100 (in minmax_check)
fn00016564 proc
	{ r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

;; nn_reduction_float: 0001657C
;;   Called from:
;;     00016050 (in min_execute)
;;     00016150 (in max_execute)
;;     00016568 (in errlog_function)
;;     00016568 (in fn00016564)
nn_reduction_float proc
	{ r5 = #0x1; allocframe(#0x70) }
	{ r4 = memw(r0+8); r6 = memw(r0+4) }
	{ r16 = r2; r2 = r1; r7 = memw(r0+16); memd(r29+104) = r17:r16 }
	{ r8 = memw(r6); r4 = memw(r4) }
	{ p0 = cmp.eq(r7,#0x3); if (!p0.new) r13 = #0x1; memd(r29+88) = r21:r20; memw(r29+24) = r3 }
	{ if (p0) r14 = #0x0; r15 = memw(r8+8); r28 = memw(r8+4) }
	{ if (p0) r11:r10 = combine(r15,r28); r3 = memw(r8); r20 = memw(r8+12) }
	{ r21 = memw(r8+16); r1 = memw(r4+16) }
	{ if (!p0) r18 = #0x1; memd(r29+96) = r19:r18; memd(r29+80) = r23:r22 }
	{ if (p0) r18 = add(r20,#0x0); memd(r29+72) = r25:r24; memd(r29+64) = r27:r26 }
	{ if (p0) jump:nt 000165EC; if (p0) r9 = memw(r6+8) }

l000165E0:
	{ r11:r10 = combine(#0x1,#0x1); r17 = #0x1; r7:r6 = combine(#0x1,#0x1) }
	{ jump 00016728 }

l000165EC:
	{ r17 = r3; r7 = memw(r6+4) }
	{ r8 = memw(r7+12); r6 = memw(r7+16) }
	{ p0 = cmp.eq(r8,#0x0); r7 = memw(r9+16) }
	{ if (p0) jump:nt 00016668; if (!p0) r9 = add(r6,#0x0); if (p0.new) r11:r10 = combine(r15,r28); r7 = memw(r7) }

l00016610:
	{ loop0(00016618,r8); r17 = r3; r18 = r20 }
	{ r12 = memw(r9++#4) }
	{ r12 = add(r7,sub(#0x7F,r12)); if (!cmp.gt(r12.new,#0x3)) jump:t 00016630 }

l00016628:
	{ r11:r10 = combine(#0x1,#0x1); r17 = #0x1 }

l00016630:
	{ if (p0.new) jump:t 0001665C; p0 = cmp.eq(r12,#0x0); if (p0.new) r18 = #0x1 }

l0001663C:
	{ if (p0.new) jump:t 0001665C; p0 = cmp.eq(r12,#0x1); if (p0.new) r11 = #0x1 }

l00016648:
	{ if (p0.new) jump:t 0001665C; p0 = cmp.eq(r12,#0x2); if (p0.new) r10 = #0x1 }

l00016654:
	{ p0 = cmp.eq(r12,#0x3); if (p0.new) r17 = #0x1 }

l0001665C:
	{ nop; nop }
	{ r14 = r8 }

l00016668:
	{ r13:r12 = combine(r10,r17); r8 = memb(r0+32); if (cmp.eq(r8.new,#0x2)) jump:t 00016680 }

l00016678:
	{ r7:r6 = combine(r11,r18); r13 = r10 }

l00016680:
	{ if (p0.new) jump:nt 000166E8; r9:r8 = combine(r11,r18); p0 = cmp.eq(r14,#0x0); if (!p0.new) r13:r12 = combine(r10,r17) }

l00016690:
	{ loop0(00016698,r14); r9:r8 = combine(r11,r18) }
	{ r14 = memw(r6++#4) }
	{ r14 = add(r7,sub(#0x7F,r14)); if (!cmp.gt(r14.new,#0x3)) jump:t 000166B4 }

l000166A8:
	{ r9:r8 = combine(#0x0,#0x0); r13 = #0x0; r12 = #0x0 }

l000166B4:
	{ if (p0.new) jump:t 000166E0; p0 = cmp.eq(r14,#0x0); if (p0.new) r8 = #0x0 }

l000166C0:
	{ if (p0.new) jump:t 000166E0; p0 = cmp.eq(r14,#0x1); if (p0.new) r9 = #0x0 }

l000166CC:
	{ if (p0.new) jump:t 000166E0; p0 = cmp.eq(r14,#0x2); if (p0.new) r13 = #0x0 }

l000166D8:
	{ p0 = cmp.eq(r14,#0x3); if (p0.new) r12 = #0x0 }

l000166E0:
	{ nop; nop }

l000166E8:
	{ p1 = cmp.eq(r12,#0x0); p2 = cmp.eq(r13,#0x0); p0 = cmp.eq(r9,#0x0); p3 = cmp.eq(r8,#0x0) }
	{ p1 = or(p1,p2); r6 = mux(p1,#0x1,r12) }
	{ p2 = or(p1,p0); if (!p2) r6 = add(r13,#0x0); if (p2.new) r13 = #0x1; r7 = mux(p1,#0x1,r12) }
	{ if (!p0) r7 = add(r6,#0x0); if (!p2) r13 = add(r12,#0x0); if (!p0) r6 = add(r9,#0x0) }
	{ if (!p3) r5 = add(r13,#0x0); if (!p3) r13 = add(r7,#0x0); if (p3.new) r7:r6 = combine(r6,r8) }

l00016728:
	{ r8 = mpyi(r18,r11); r9 = memw(r4+20); memw(r29+4) = r17 }
	{ r8 = mpyi(r8,r10); memw(r29+12) = r11; memw(r29+8) = r10 }
	{ r8 = mpyi(r8,r17) }
	{ r8 = asl(r8,#0x2); if (!cmp.gtu(r8.new,r9)) jump:t 0001676C }

l00016750:
	{ r0 = add(PC,#0x3); r1 = #0xC6 }
	{ immext(#0x11040); r3 = add(PC,#0x1106E); call errlog_function }
	{ jump 000168E0; r0 = #0xFFFFFFFF }

l0001676C:
	{ r0 = #0x0; p0 = cmp.gt(r17,#0x0); memw(r4+12) = r6; memw(r4+24) = r8 }
	{ if (p0) r5 = #0x0; memw(r4+8) = r7; memw(r4) = r5 }
	{ memw(r4+4) = r13 }
	{ if (!p0) jump:nt 000168E0; if (p0) memw(r29+48) = r15; if (p0) memw(r29+32) = r28 }

l00016794:
	{ p3 = cmp.eq(r11,#0x1); p2 = cmp.eq(r10,#0x1); r4 = r18; p0 = cmp.eq(r17,#0x1) }
	{ p1 = cmp.eq(r4,#0x1); r2 = mux(p0,r3,#0x1); r23 = mux(p3,r15,#0x1); memw(r29+36) = r5 }
	{ r3 = mux(p2,r28,#0x1); r24 = mux(p1,r20,#0x1); memw(r29+20) = r4; memw(r29+40) = r2 }
	{ memw(r29+60) = r3 }

l000167C0:
	{ r2 = #0x0; p0 = cmp.gt(r10,#0x0) }
	{ if (!p0) jump:nt 000168D0; memw(r29+52) = r2 }

l000167D0:
	{ if (!p0.new) jump:nt 000168B8; r17 = #0x0; p0 = cmp.gt(r11,#0x0) }

l000167DC:
	{ r19 = #0x0; p0 = cmp.gt(r4,#0x0); r2 = r1; memw(r29+16) = r1 }
	{ if (!p0) jump:nt 000168AC; if (!p0) r1 = memw(r29+16) }

l000167F0:
	{ r2 = memd(r29+40); memw(r29+28) = r2 }
	{ p0 = cmp.gt(r2,#0x0); r0 = memw(r29+24) }
	{ if (!p0) jump:nt 00016890 }

l00016800:
	{ r3 = #0x0; r0 = memd(r29+24) }

l00016804:
	{ r27 = #0x0; r2 = memw(r29+60); if (cmp.gt(r2.new,#0x0)) jump:t 00016818 }

l00016814:
	{ memw(r29+44) = r3 }

l00016818:
	{ r2 = memd(r29+36); memw(r29+44) = r3 }
	{ r2 = add(r3,r2) }
	{ r3 = memw(r29+32) }
	{ r2 = mpyi(r2,r3); memb(r29+14) = r2.new }

l0001682C:
	{ p0 = cmp.gt(r15,#0x0); if (!p0.new) jump:nt 00016878; if (p0.new) r7 = memw(r29+48) }
	{ r22 = #0x0; r2 = memd(r29+56); r3 = memd(r29+52) }
	{ r2 += add(r27,r3) }
	{ r26 = mpyi(r2,r7) }

l00016844:
	{ if (!p0.new) jump:nt 00016870; r25 = r24; r3:r2 = combine(r19,r26); p0 = cmp.gt(r24,#0x0) }

l00016848:
	{ r25 = r24; r3:r2 = combine(r19,r26); p0 = cmp.gt(r24,#0x0) }

l00016854:
	{ r2 += add(r22,r17) }
	{ r3 += mpyi(r2,r20) }
	{ r18 = addasl(r21,r3,#0x2) }

l00016860:
	{ callr r16; r18 = add(r18,#0x4); r1 = memw(r18) }
	{ r25 = add(r25,#0xFFFFFFFF); if (!cmp.eq(r25.new,#0x0)) jump:t 00016860 }

l00016870:
	{ r22 = add(r22,#0x1); if (!cmp.eq(r22.new,r23)) jump:t 00016844 }

l00016874:
	{ if (!cmp.eq(r22.new,r23)) jump:t 00016848 }

l0001687C:
	{ r27 = add(r27,#0x1); if (!cmp.eq(r27.new,r2)) jump:t 0001682C }

l00016888:
	{ r3 = add(r3,#0x1); if (!cmp.eq(r3.new,r2)) jump:t 00016804 }

l00016890:
	{ r19 = add(r19,#0x1); r2 = memd(r29+28); r4 = memd(r29+20) }

l00016894:
	{ r2 = memd(r29+28); r4 = memd(r29+20) }

l00016898:
	{ p0 = cmp.eq(r19,r4); r2 = add(r2,#0x4); memw(r2) = r0 }
	{ if (!p0) jump:nt 000167F0; if (p0) r1 = memw(r29+16) }

l000168A8:
	{ r1 = addasl(r1,r4,#0x2) }

l000168AC:
	{ r11 = memw(r29+12) }
	{ r17 = add(r17,#0x1); if (!cmp.eq(r17.new,r11)) jump:t 000167DC }

l000168B8:
	{ r2 = memw(r29+52); r10 = memw(r29+8) }

l000168BC:
	{ r10 = memw(r29+8) }

l000168C0:
	{ r2 = add(r2,#0x1) }
	{ if (!p0.new) jump:nt 000167D0; p0 = cmp.eq(r2,r10); memw(r29+52) = r2 }

l000168D0:
	{ r3 = memd(r29+36); r2 = memd(r29+4) }
	{ r3 = add(r3,#0x1) }
	{ p0 = cmp.eq(r3,r2); if (!p0.new) jump:nt 000167C0; r0 = #-0x1; memw(r29+36) = r3 }

l000168E0:
	{ r17:r16 = memd(r29+104); r19:r18 = memd(r29+96) }
	{ r21:r20 = memd(r29+88); r23:r22 = memd(r29+80) }
	{ r25:r24 = memd(r29+72); r27:r26 = memd(r29+64) }
	{ dealloc_return }
000168F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; qsoftmax_execute_ref: 00016900
qsoftmax_execute_ref proc
	{ immext(#0x437F0000); r6 = #0x437F0000; memd(r29-48) = r25:r24; allocframe(#0x68) }
	{ r24 = r0 }
	{ r25 = r1; r2 = memw(r24+4); memd(r29+88) = r19:r18 }
	{ r3 = memw(r24+8) }
	{ memd(r29+96) = r17:r16; memd(r29+72) = r23:r22 }
	{ r18 = memw(r2); r4 = memw(r2+4) }
	{ r2 = memw(r2+8); r5 = memw(r3+8) }
	{ r7 = memw(r18); r4 = memw(r4+16) }
	{ r2 = memw(r2+16); r17 = memw(r18+12) }
	{ r3 = mpyi(r7,r17); r1 = memw(r3+4); r19 = memw(r3) }
	{ r23 = memw(r18+4); r16 = memw(r4) }
	{ r2 = memw(r2); memd(r29+80) = r21:r20 }
	{ r20 = memw(r18+8) }
	{ r2 = mpyi(r3,r23); r0 = sfsub(r2,r16); r1 = r6; memw(r29+28) = r1 }
	{ memd(r29+56) = r27:r26; memw(r29+36) = r24 }
	{ memw(r29+48) = r7 }
	{ r26 = memw(r18+16); memw(r29+32) = r5 }
	{ r22 = mpyi(r2,r20); call fn00009610; r27 = memw(r19+16) }
	{ immext(#0x10FC0); r4 = add(PC,#0x10FF5); r1 = #0x4A; r2 = r25 }
	{ call logmsg_function; r21 = r0; memw(r29) = r24 }
	{ r5 = r19 }
	{ r2 = memw(r5+20); if (!cmp.gtu(r22,r2.new)) jump:t 000169BC }

l000169A4:
	{ r3 = add(PC,#0x28); r1 = #0x4B; r2 = r25 }
	{ call errlog_function }
	{ jump 00016C10; r0 = #0xFFFFFFFF }

l000169BC:
	{ r3 = memw(r18); r2 = memd(r29+48) }
	{ r2 = mpyi(r23,r2); r4 = memw(r18+4); memb(r5+1) = r4.new }
	{ memw(r5) = r3 }
	{ p0 = cmp.gt(r2,#0x0); r6 = memw(r18+8) }
	{ immext(#0x7F800000); if (p0) r20 = #0x7F800000; memw(r5+8) = r6; memw(r29+24) = r25 }
	{ immext(#0xFF800000); if (p0) r18 = #0xFF800000; r7 = memw(r18+12) }
	{ if (p0) r22 = #0x0; memw(r5+12) = r7; memw(r5+24) = r22 }
	{ if (p0) jump:nt 00016A20; memw(r29+40) = r2; if (p0) memw(r29+16) = r5 }

l00016A0C:
	{ immext(#0x7F800000); r20 = #0x7F800000; immext(#0xFF800000); r18 = #0xFF800000 }
	{ jump 00016BC8 }

l00016A20:
	{ r0 = p0; memb(r29+5) = r0.new }

l00016A28:
	{ p0 = cmp.gt(r17,#0x0); if (p0.new) memw(r29+44) = r22 }
	{ r0 = p0; if (p0) jump:nt 00016A58; memb(r29+12) = r0.new }

l00016A40:
	{ r0 = memw(r29+48) }
	{ p0 = r0; if (p0.new) jump:nt 00016A9C; if (p0.new) r19 = #0x1; if (p0.new) memw(r29+44) = r22 }

l00016A54:
	{ jump 00016B50 }

l00016A58:
	{ r23 = r16; r19 = #0x1; r25 = memb(r26) }
	{ r3 = convert_w2sf(r25); r2 = r25 }
	{ r23 += sfmpy(r21,r3) }

l00016A70:
	{ r2 = and(r2,#0xFF); r1:r0 = combine(r23,r16) }
	{ r2 = convert_w2sf(r2) }
	{ r0 += sfmpy(r21,r2); call fn00009600 }
	{ p0 = cmp.eq(r9,r11); if (p0.new) jump:nt 00016A40; r2 = add(r26,r19); r23 = r0 }

l00016A90:
	{ r19 = add(r19,#0x1) }
	{ jump 00016A70; r2 = memb(r2) }

l00016A9C:
	{ immext(#0x0); r24 = #0x0 }

l00016AA4:
	{ r2 = and(r25,#0xFF); r3 = r16 }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r21,r2) }
	{ r0 = sfsub(r3,r23); call fn00009780 }
	{ r24 = sfadd(r24,r0); p0 = cmp.eq(r9,r11); if (p0.new) jump:nt 00016AD4; r2 = add(r26,r19) }

l00016AC8:
	{ r19 = add(r19,#0x1); r25 = memb(r2) }
	{ jump 00016AA4 }

l00016AD4:
	{ r1 = r24; immext(#0x3F800000); r0 = #0x3F800000; memw(r29+44) = r22 }
	{ call fn00009610 }
	{ r24 = r0; r19 = #0x0; r0 = memd(r29+48) }
	{ p0 = r0; if (!p0.new) jump:nt 00016B50 }

l00016AF4:
	{ immext(#0x437F0000); r22 = #0x437F0000 }

l00016AFC:
	{ r3 = r16; r2 = memb(r13+r19) }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r21,r2) }
	{ r0 = sfsub(r3,r23); call fn00009780 }
	{ r25 = sfmpy(r24,r0) }
	{ call fn00009600; r1:r0 = combine(r25,r18) }
	{ call fn000097B0; r18 = r0; r1:r0 = combine(r25,r20) }
	{ r2 = sfmpy(r25,r22); r3 = add(r27,r19); r20 = r0; r19 = r19 }
	{ p1 = sfcmp.gt(r2,r22); r4 = convert_sf2uw(r2); p0 = cmp.eq(r17,r19) }
	{ if (p1) r4 = #0xFFFFFFFF }
	{ if (!p0) jump:nt 00016AFC; memb(r3) = r4 }

l00016B50:
	{ r26 = add(r26,r17); r27 = add(r27,r17); r22 = memd(r29+44); r2 = memd(r29+40) }
	{ r22 = add(r22,#0x1); if (!cmp.eq(r22.new,r2)) jump:t 00016A28 }

l00016B68:
	{ p0 = r0; if (!p0.new) jump:nt 00016BC8 }

l00016B70:
	{ r1 = r18; immext(#0x3F800000); r0 = #0x3F800000; r2 = memd(r29+16) }
	{ call fn00009610; r16 = memw(r2+16) }
	{ immext(#0x3F000000); r2 = #0x3F000000; r3 = memd(r29+40) }
	{ loop1(00016B98,r3); r3 = memw(r29+48) }
	{ p0 = r3 }
	{ if (!p0) jump:nt 00016BBC; if (p0) r3 = add(r16,#0x0) }

l00016BA0:
	{ loop0(00016BA4,r17) }
	{ r5 = r20; r4 = memb(r3) }
	{ r4 = convert_w2sf(r4) }
	{ r5 += sfmpy(r0,r4) }
	{ r7 = sfadd(r5,r2) }
	{ r6 = convert_sf2uw(r7); memb(r3++#1) = r6.new }

l00016BBC:
	{ r16 = add(r16,r17); nop; nop }

l00016BC0:
	{ nop; nop }

l00016BC8:
	{ immext(#0x10DC0); r4 = add(PC,#0x10DCE); r7 = memd(r29+28); r6 = memd(r29+32) }
	{ r1 = #0x85; r2 = memd(r29+24); r5 = memd(r29+36) }
	{ r0 = memw(r7+16); memw(r7+12) = #0x1 }
	{ memw(r7+8) = #0x1; memw(r7) = #0x1 }
	{ memw(r7+4) = #0xFFFFFF81; memw(r0) = r20 }
	{ memw(r7+24) = #0x4; memw(r6+8) = #0x1 }
	{ r3 = memw(r6+16); memw(r6) = #0x1 }
	{ memw(r6+4) = #0x1; memw(r6+12) = #0x1 }
	{ memw(r3) = r18; memw(r6+24) = #0x4 }
	{ call logmsg_function; memw(r29) = r5 }
	{ r0 = #0x0 }

l00016C10:
	{ r17:r16 = memd(r29+96); r19:r18 = memd(r29+88) }
	{ r21:r20 = memd(r29+80); r23:r22 = memd(r29+72) }
	{ r25:r24 = memd(r29+64); r27:r26 = memd(r29+56) }
	{ dealloc_return }

;; qsoftmax_check: 00016C24
qsoftmax_check proc
	{ immext(#0x10CC0); r4 = add(PC,#0x10CF8); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x8B; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x8C; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 00016C64 }

l00016C50:
	{ r3 = add(PC,#0x29) }
	{ call errlog_function; r2 = r16 }

l00016C58:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00016C64:
	{ r1 = #0x8D; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 00016C7C }

l00016C74:
	{ r3 = add(PC,#0x14); jump 00016C58 }

l00016C7C:
	{ immext(#0x10CC0); r4 = add(PC,#0x10CD8); r1 = #0x8E; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00016C9C
;;   Called from:
;;     00016988 (in qsoftmax_execute_ref)
;;     00016C04 (in qsoftmax_execute_ref)
;;     00016C38 (in qsoftmax_check)
;;     00016C8C (in qsoftmax_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00016CC0 }

l00016CAC:
	{ r0 = add(PC,#0x17); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00016CC0:
	{ dealloc_return }

;; errlog_function: 00016CC4
;;   Called from:
;;     000169B0 (in qsoftmax_execute_ref)
;;     00016C54 (in qsoftmax_check)
;;     00016CB4 (in logmsg_function)
errlog_function proc
	{ immext(#0x10C00); r0 = add(PC,#0x10C3B); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00016CE8                         00 00 00 00 00 00 00 00         ........

;; nop_execute: 00016CF0
nop_execute proc
	{ immext(#0x10D00); r4 = add(PC,#0x10D31); r2 = r1; allocframe(#0x18) }
	{ r1 = #0x31; r16 = r0; memd(r29+16) = r17:r16; memd(r29+8) = r19:r18 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r2 = memw(r16+20); if (cmp.eq(r2.new,#0x0)) jump:nt 00016D64 }

l00016D1C:
	{ r2 = memw(r16+8); r3 = memw(r16+4) }
	{ r3 = memw(r21+r17); r2 = memw(r13+r17) }
	{ r4 = memw(r3) }
	{ r5 = memw(r3+4); memb(r2+1) = r5.new }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 00016D58 }

l00016D50:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l00016D58:
	{ r17 = add(r17,#0x4); r2 = memw(r16+20) }
	{ r18 = add(r18,#0x1); if (cmp.gtu(r2,r18.new)) jump:t 00016D1C }

l00016D64:
	{ r0 = #0x0; r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }

l00016D68:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }
	{ dealloc_return }

;; nop_check: 00016D70
nop_check proc
	{ immext(#0x10C40); r4 = add(PC,#0x10C75); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x3B; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+20) }
	{ r3 = memw(r17+16); if (cmp.eq(r3.new,r2)) jump:t 00016DA8 }

l00016D9C:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00016DA8:
	{ immext(#0x10C40); r4 = add(PC,#0x10C64); r1 = #0x3D; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; nop_ctor: 00016DC4
nop_ctor proc
	{ immext(#0x10C00); r6 = add(PC,#0x10C0D); memd(r29-24) = r19:r18; allocframe(#0x28) }
	{ r19:r18 = combine(r5,r0) }
	{ r17:r16 = combine(r4,r1); r21:r20 = combine(r2,r3); memd(r29+32) = r17:r16; memd(r29+16) = r21:r20 }
	{ r1 = #0x4B; r2 = r18; r4 = r6 }
	{ r22 = memd(r29+48); memd(r29+8) = r23:r22 }
	{ r23 = memw(r29+52) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r5:r4 = combine(r19,r17); r3:r2 = combine(r20,r21); r1:r0 = combine(r16,r18); memw(r29+4) = r23 }
	{ call node_alloc_common; memw(r29) = r22 }
	{ r17:r16 = memd(r29+32) }
	{ r19:r18 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r23:r22 = memd(r29+8); dealloc_return }

;; nop_dtor: 00016E1C
nop_dtor proc
	{ immext(#0x10B80); r4 = add(PC,#0x10BA4); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x59; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1:r0 = combine(r16,r17) }
	{ jump node_free_common; r17:r16 = memd(r29+8); deallocframe }

;; logmsg_function: 00016E44
;;   Called from:
;;     00016D08 (in nop_execute)
;;     00016D84 (in nop_check)
;;     00016DB4 (in nop_check)
;;     00016DF0 (in nop_ctor)
;;     00016E30 (in nop_dtor)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00016E68 }

l00016E54:
	{ r0 = add(PC,#0x17); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00016E68:
	{ dealloc_return }

l00016E6C:
	{ nop }

;; errlog_function: 00016E70
;;   Called from:
;;     00016E6C (in logmsg_function)
errlog_function proc
	{ immext(#0x10B00); r0 = add(PC,#0x10B37); r3 = #0x0; allocframe(#0x8) }
	{ immext(#0x10B40); r4 = add(PC,#0x10B7E); r1 = #0x3C; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; output_execute: 00016EA0
;;   Called from:
;;     00016E9C (in errlog_function)
output_execute proc
	{ immext(#0x10BC0); r4 = add(PC,#0x10BF6); memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ r1 = #0x34; r16 = r1 }
	{ r2 = r16; r18 = r0; memd(r29+16) = r19:r18; memd(r29+8) = r21:r20 }
	{ call logmsg_function; memw(r29) = r18 }
	{ r2 = memw(r16+32) }
	{ r3 = memw(r18+16); if (!cmp.eq(r3.new,r2)) jump:t 00016F04 }

l00016ED4:
	{ r17 = #0x0; r2 = #0x0 }
	{ r19 = #0x0; r21:r20 = combine(#0x0,#0x0) }

l00016EDC:
	{ r2 = memw(r18+4); r3 = memw(r16+24) }
	{ r3 = add(r3,r21); r2 = memw(r13+r20) }
	{ r4 = memw(r3+20) }
	{ r5 = memw(r2+24); if (!cmp.gtu(r5.new,r4)) jump:t 00016F1C }

l00016EF8:
	{ r3 = add(PC,#0x9); r1 = #0x3A; memw(r29) = r19 }
	{ jump 00016F10 }

l00016F04:
	{ immext(#0x10B80); r3 = add(PC,#0x10BAB); r1 = #0x35 }

l00016F10:
	{ call errlog_function; r2 = r16; r17 = #-0x1 }
	{ jump 00016F74 }

l00016F1C:
	{ r4 = memw(r2); r5 = memw(r2+4) }
	{ memw(r3+4) = r5; memw(r3) = r4 }
	{ r4 = memw(r2+8) }
	{ memw(r3+8) = r4 }
	{ r7 = memw(r2+12) }
	{ memw(r3+12) = r7 }
	{ r4 = memw(r2+24) }
	{ memw(r3+24) = r4 }
	{ r1 = memw(r2+16) }
	{ call fn00009560; r0 = memw(r3+16); r2 = memw(r2+24) }
	{ r20 = add(r20,#0x4); r21 = add(r21,#0x20); r2 = memw(r16) }
	{ r19 = add(r19,#0x1); if (cmp.gtu(r2,r19.new)) jump:t 00016EDC }

l00016F5C:
	{ immext(#0x10B40); r4 = add(PC,#0x10B75); r1 = #0x41; memw(r29) = r2 }
	{ call logmsg_function; r2 = r16 }

l00016F74:
	{ r0 = r17; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); dealloc_return }

;; output_check: 00016F80
output_check proc
	{ immext(#0x10AC0); r4 = add(PC,#0x10AD3); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x48; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 00016FDC }

l00016FA8:
	{ r3 = memw(r17+4) }
	{ r3 = add(r3,#0x4); r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 00016FDC }

l00016FB0:
	{ r4 = add(r4,#0x1); if (!cmp.gtu(r2,r4.new)) jump:nt 00016FE0 }

l00016FBC:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 00016FB0 }

l00016FC4:
	{ r3 = add(PC,#0x2B); r1 = #0x4A; r2 = r16 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00016FDC:
	{ immext(#0x10A80); r4 = add(PC,#0x10AA2); r1 = #0x4C; r2 = r16 }

l00016FE0:
	{ r4 = add(PC,#0x22); r1 = #0x4C; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00016FFC
;;   Called from:
;;     00016EBC (in output_execute)
;;     00016F6C (in output_execute)
;;     00016F94 (in output_check)
;;     00016FEC (in output_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00017020 }

l0001700C:
	{ r0 = add(PC,#0x2F); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00017020:
	{ dealloc_return }

;; errlog_function: 00017024
;;   Called from:
;;     00016F10 (in output_execute)
;;     00016FD0 (in output_check)
;;     00017014 (in logmsg_function)
errlog_function proc
	{ immext(#0x10A00); r0 = add(PC,#0x10A13); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00017048                         00 00 00 00 00 00 00 00         ........

;; pprint_8_execute: 00017050
pprint_8_execute proc
	{ immext(#0x10A80); r4 = add(PC,#0x10AAF); allocframe(#0x48) }
	{ r2 = memw(r0+4); r5 = memw(r0+28) }
	{ r16 = r1; r1 = #0x48; memd(r29+64) = r17:r16; memd(r29+56) = r19:r18 }
	{ r3:r2 = combine(#0x2,r16); r7 = memw(r2) }
	{ memd(r29+48) = r21:r20; memd(r29+40) = r23:r22 }
	{ r17 = memw(r7+16); memd(r29+32) = r25:r24 }
	{ r18 = memw(r7+12); r19 = memw(r7+8) }
	{ r20 = memw(r7+4); r21 = memw(r7) }
	{ memw(r29+4) = r5; memd(r29+24) = r27:r26 }
	{ call logmsg_function; memw(r29) = r0 }
	{ immext(#0x10A40); r4 = add(PC,#0x10A7F); r3:r2 = combine(#0x1,r16); memw(r29+12) = r18 }
	{ r1 = #0x48; memw(r29) = r21 }
	{ memw(r29+8) = r19; memw(r29+4) = r20 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r13,#0x0); if (p0.new) jump:nt 0001712C; r22 = #0x0 }

l000170C0:
	{ p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 00017124; r23 = #0x0 }

l000170C4:
	{ r23 = #0x0 }

l000170C8:
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 0001711C; r24 = #0x0 }

l000170CC:
	{ r24 = #0x0 }

l000170D0:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 00017114; r25 = r17; r26 = #0x0 }

l000170DC:
	{ immext(#0x10A80); r4 = add(PC,#0x10AA5); r2 = memb(r25++#1); memb(r29+4) = r2.new }
	{ r3:r2 = combine(#0x1,r16); memw(r29+4) = r23 }
	{ memw(r29+12) = r26; memw(r29+8) = r24 }
	{ call logmsg_function; memw(r29) = r22 }
	{ r26 = add(r26,#0x1); if (!cmp.eq(r26.new,r18)) jump:t 000170DC }

l00017114:
	{ r24 = add(r24,#0x1); if (!cmp.eq(r24.new,r19)) jump:t 000170D0 }

l0001711C:
	{ r23 = add(r23,#0x1); if (!cmp.eq(r23.new,r20)) jump:t 000170C8 }

l00017120:
	{ if (!cmp.eq(r23.new,r20)) jump:t 000170CC }

l00017124:
	{ r22 = add(r22,#0x1); if (!cmp.eq(r22.new,r21)) jump:t 000170C0 }

l00017128:
	{ if (!cmp.eq(r22.new,r21)) jump:t 000170C4 }

l0001712C:
	{ r0 = #0x0; r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l00017130:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l00017134:
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }

;; pprint_check: 00017144
pprint_check proc
	{ r2 = r1; allocframe(#0x0) }
	{ r1 = #0x4E; r3 = memw(r0+16); if (cmp.eq(r3.new,#0x1)) jump:t 00017164 }

l00017158:
	{ r3 = add(PC,#0x3E) }
	{ call errlog_function }

l00017160:
	{ r0 = #-0x1; dealloc_return }

l00017164:
	{ r1 = #0x4F; r3 = memw(r0+20); if (cmp.eq(r3.new,#0x0)) jump:nt 0001717C }

l00017174:
	{ r3 = add(PC,#0x31); jump 00017160 }

l0001717C:
	{ immext(#0x109C0); r4 = add(PC,#0x109F5); r3 = #0x2; r1 = #0x50 }
	{ call logmsg_function }
	{ r0 = #0x0; dealloc_return }

;; pprint_32_execute: 00017194
pprint_32_execute proc
	{ immext(#0x10940); r4 = add(PC,#0x1096B); allocframe(#0x48) }
	{ r2 = memw(r0+4); r5 = memw(r0+28) }
	{ r16 = r1; r1 = #0x49; memd(r29+64) = r17:r16; memd(r29+56) = r19:r18 }
	{ r3:r2 = combine(#0x2,r16); r7 = memw(r2) }
	{ memd(r29+48) = r21:r20; memd(r29+40) = r23:r22 }
	{ r17 = memw(r7+16); memd(r29+32) = r25:r24 }
	{ r18 = memw(r7+12); r19 = memw(r7+8) }
	{ r20 = memw(r7+4); r21 = memw(r7) }
	{ memw(r29+4) = r5; memd(r29+24) = r27:r26 }
	{ call logmsg_function; memw(r29) = r0 }
	{ immext(#0x10900); r4 = add(PC,#0x1093B); r3:r2 = combine(#0x1,r16); memw(r29+12) = r18 }
	{ r1 = #0x49; memw(r29) = r21 }
	{ memw(r29+8) = r19; memw(r29+4) = r20 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r13,#0x0); if (p0.new) jump:nt 00017270; r22 = #0x0 }

l00017204:
	{ p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 00017268; r23 = #0x0 }

l00017208:
	{ r23 = #0x0 }

l0001720C:
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 00017260; r24 = #0x0 }

l00017210:
	{ r24 = #0x0 }

l00017214:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 00017258; r25 = r17; r26 = #0x0 }

l00017220:
	{ immext(#0x10900); r4 = add(PC,#0x1091C); r2 = memw(r25++#4); memb(r29+4) = r2.new }
	{ r3:r2 = combine(#0x1,r16); memw(r29+4) = r23 }
	{ memw(r29+12) = r26; memw(r29+8) = r24 }
	{ call logmsg_function; memw(r29) = r22 }
	{ r26 = add(r26,#0x1); if (!cmp.eq(r26.new,r18)) jump:t 00017220 }

l00017258:
	{ r24 = add(r24,#0x1); if (!cmp.eq(r24.new,r19)) jump:t 00017214 }

l00017260:
	{ r23 = add(r23,#0x1); if (!cmp.eq(r23.new,r20)) jump:t 0001720C }

l00017264:
	{ if (!cmp.eq(r23.new,r20)) jump:t 00017210 }

l00017268:
	{ r22 = add(r22,#0x1); if (!cmp.eq(r22.new,r21)) jump:t 00017204 }

l0001726C:
	{ if (!cmp.eq(r22.new,r21)) jump:t 00017208 }

l00017270:
	{ r0 = #0x0; r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l00017274:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l00017278:
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }
00017288                         00 40 00 7F 00 C0 00 7F         .@......

;; pprint_f_execute: 00017290
pprint_f_execute proc
	{ immext(#0x10840); r4 = add(PC,#0x1086F); allocframe(#0x48) }
	{ r2 = memw(r0+4); r5 = memw(r0+28) }
	{ r16 = r1; r1 = #0x4A; memd(r29+64) = r17:r16; memd(r29+56) = r19:r18 }
	{ r3:r2 = combine(#0x2,r16); r7 = memw(r2) }
	{ memd(r29+48) = r21:r20; memd(r29+40) = r23:r22 }
	{ r17 = memw(r7+16); memd(r29+32) = r25:r24 }
	{ r18 = memw(r7+12); r19 = memw(r7+8) }
	{ r20 = memw(r7+4); r21 = memw(r7) }
	{ memw(r29+4) = r5; memd(r29+24) = r27:r26 }
	{ call logmsg_function; memw(r29) = r0 }
	{ immext(#0x10800); r4 = add(PC,#0x1083F); r3:r2 = combine(#0x1,r16); memw(r29+12) = r18 }
	{ r1 = #0x4A; memw(r29) = r21 }
	{ memw(r29+8) = r19; memw(r29+4) = r20 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r13,#0x0); if (p0.new) jump:nt 00017374; r22 = #0x0 }

l00017300:
	{ p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 0001736C; r23 = #0x0 }

l00017304:
	{ r23 = #0x0 }

l00017308:
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 00017364; r24 = #0x0 }

l0001730C:
	{ r24 = #0x0 }

l00017310:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 0001735C; r25 = r17; r26 = #0x0 }

l0001731C:
	{ immext(#0x10800); r4 = add(PC,#0x1080E); r25 = add(r25,#0x4); r2 = memw(r25) }
	{ r1:r0 = convert_sf2df(r2); r3:r2 = combine(#0x1,r16); memw(r29+8) = r24; memw(r29+4) = r23 }
	{ r1 = #0x4A; memd(r29+16) = r1:r0; memw(r29+12) = r26 }
	{ call logmsg_function; memw(r29) = r22 }
	{ r26 = add(r26,#0x1); if (!cmp.eq(r26.new,r18)) jump:t 0001731C }

l0001735C:
	{ r24 = add(r24,#0x1); if (!cmp.eq(r24.new,r19)) jump:t 00017310 }

l00017364:
	{ r23 = add(r23,#0x1); if (!cmp.eq(r23.new,r20)) jump:t 00017308 }

l00017368:
	{ if (!cmp.eq(r23.new,r20)) jump:t 0001730C }

l0001736C:
	{ r22 = add(r22,#0x1); if (!cmp.eq(r22.new,r21)) jump:t 00017300 }

l00017370:
	{ if (!cmp.eq(r22.new,r21)) jump:t 00017304 }

l00017374:
	{ r0 = #0x0; r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l00017378:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }

l0001737C:
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
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
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000173AC }

l0001739C:
	{ r0 = add(PC,#0xB); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l000173AC:
	{ dealloc_return }

;; errlog_function: 000173B0
;;   Called from:
;;     0001715C (in pprint_check)
errlog_function proc
	{ immext(#0x10700); r0 = add(PC,#0x10733); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
000173D4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; prefree_execute: 000173E0
prefree_execute proc
	{ immext(#0x10800); r3 = add(PC,#0x10825); r1 = #0x31; r2 = r1 }
	{ call errlog_function; allocframe(#0x0) }
	{ r0 = #-0x1; dealloc_return }
000173F8                         00 40 00 7F 00 C0 00 7F         .@......

;; prefree_check: 00017400
prefree_check proc
	{ immext(#0x107C0); r3 = add(PC,#0x107E1); r1 = #0x36; r2 = r1 }
	{ call errlog_function; allocframe(#0x0) }
	{ r0 = #-0x1; dealloc_return }
00017418                         00 40 00 7F 00 C0 00 7F         .@......

;; prefree_ctor: 00017420
prefree_ctor proc
	{ immext(#0x10780); r6 = add(PC,#0x107A9); memd(r29-24) = r19:r18; allocframe(#0x28) }
	{ r19:r18 = combine(r5,r0) }
	{ r17:r16 = combine(r4,r1); r21:r20 = combine(r2,r3); memd(r29+32) = r17:r16; memd(r29+16) = r21:r20 }
	{ r1 = #0x43; r2 = r18; r4 = r6 }
	{ r22 = memd(r29+48); memd(r29+8) = r23:r22 }
	{ r23 = memw(r29+52) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r5:r4 = combine(r19,r17); r3:r2 = combine(r20,r21); r1:r0 = combine(r16,r18); memw(r29+4) = r23 }
	{ call node_alloc_common; memw(r29) = r22 }
	{ r17:r16 = memd(r29+32) }
	{ r19:r18 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r23:r22 = memd(r29+8); dealloc_return }
00017478                         00 40 00 7F 00 C0 00 7F         .@......

;; prefree_dtor: 00017480
prefree_dtor proc
	{ immext(#0x10700); r4 = add(PC,#0x10734); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x51; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1:r0 = combine(r16,r17) }
	{ jump node_free_common; r17:r16 = memd(r29+8); deallocframe }

;; logmsg_function: 000174A8
;;   Called from:
;;     0001744C (in prefree_ctor)
;;     00017494 (in prefree_dtor)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000174CC }

l000174B8:
	{ r0 = add(PC,#0x23); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l000174CC:
	{ dealloc_return }

;; errlog_function: 000174D0
;;   Called from:
;;     000173EC (in prefree_execute)
;;     0001740C (in prefree_check)
;;     000174C0 (in logmsg_function)
errlog_function proc
	{ immext(#0x106C0); r0 = add(PC,#0x106C7); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
000174F4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; quantize_execute: 00017500
quantize_execute proc
	{ immext(#0x10800); r4 = add(PC,#0x10805); allocframe(#0x40) }
	{ r3 = memw(r0+4); r5 = memw(r0+8) }
	{ memd(r29+24) = r25:r24; memd(r29+16) = r27:r26 }
	{ r6 = memw(r3); r7 = memw(r3+8) }
	{ r3 = memw(r3+4); memd(r29+48) = r19:r18 }
	{ r8 = memw(r6); r9 = memw(r6+4) }
	{ r12 = memw(r6+8); r13 = memw(r6+12) }
	{ r26 = convert_uw2sf(r9); r25 = convert_uw2sf(r8); r16 = r1; memd(r29+56) = r17:r16 }
	{ r27 = convert_uw2sf(r12); r18 = convert_uw2sf(r13); r3 = memw(r3+16); r19 = memw(r5) }
	{ r8 = sfmpy(r25,r26); r1 = #0x49; r2 = r16; r7 = memw(r7+16) }
	{ r8 = sfmpy(r8,r27); r9 = memw(r5+4); memd(r29+32) = r23:r22 }
	{ r5 = memw(r5+8); memd(r29+40) = r21:r20 }
	{ memw(r29+8) = r9 }
	{ r24 = sfmpy(r8,r18); r22 = memw(r6+16); memw(r29+12) = r5 }
	{ r23 = memw(r19+16); r17 = memw(r3) }
	{ r21 = convert_uw2sf(r24):chop; call logmsg_function; r20 = memw(r7); memw(r29) = r0 }
	{ r7 = convert_sf2uw(r26); r6 = convert_sf2uw(r18); r2 = memw(r19+20); if (!cmp.gtu(r21,r2.new)) jump:t 000175AC }

l00017594:
	{ r3 = add(PC,#0xB); r1 = #0x4A; r2 = r16 }
	{ call errlog_function }
	{ jump 000176BC; r0 = #0xFFFFFFFF }

l000175AC:
	{ r3 = convert_sf2uw(r27); r2 = convert_sf2uw(r25); immext(#0x0); r4 = #0x0 }
	{ r18 = r4; memw(r19+24) = r21 }
	{ r1:r0 = combine(r17,r18); memw(r19+8) = r3; memw(r19) = r2 }
	{ memw(r19+12) = r6; memw(r19+4) = r7 }
	{ call fn000097B0 }
	{ r16 = r0 }
	{ r0 = sfsub(r20,r16); call fmaxf.1.0; immext(#0x437E0000); r17 = #0x437E0000 }
	{ call fn00009610; r19 = r0; r1 = r17 }
	{ call fn00009610; r20 = r0; r1:r0 = combine(r19,r17) }
	{ r4 = sfsub(r18,r16); immext(#0xD9168700); r2 = #0xD916872B; r16 = r0 }
	{ r4 = sfmpy(r4,r16); immext(#0x3FEFF7C0); r3 = #0x3FEFF7CE }
	{ r1:r0 = convert_sf2df(r4); call fn000097D0 }
	{ r2 = convert_df2w(r1:r0):chop; p0 = sfcmp.gt(r24,r18); immext(#0x437F0000); r3 = #0x437F0000 }
	{ if (p0) r25 = #0x100; if (p0) r19 = #0x1; r2 = sub(#0x0,r2) }
	{ r2 = convert_w2sf(r2) }
	{ r18 = sfmpy(r20,r2) }
	{ r17 = r18 }
	{ r17 += sfmpy(r20,r3); if (!p0) jump:nt 00017688 }

l00017648:
	{ r2 = memw(r22) }
	{ r2 = sfsub(r2,r18) }
	{ r0 = sfmpy(r16,r2); call fn00009620 }
	{ r3 = convert_uw2sf(r0):chop; r2 = #0x0 }
	{ p0 = cmp.gt(r3,#-0x1); if (p0.new) jump:nt 00017670; if (p0.new) r2 = add(r3,#0x0) }

l00017668:
	{ p0 = cmp.gt(r25,r3); if (!p0.new) r2 = #0xFF }

l00017670:
	{ r3 = convert_w2sf(r19); r22 = add(r22,#0x4); r19 = add(r19,#0x1); memb(r23++#1) = r2 }
	{ p0 = sfcmp.gt(r24,r3); if (p0.new) jump:nt 00017648 }

l00017688:
	{ r0 = #0x0; r4 = memd(r29+8); r3 = memd(r29+12) }
	{ memw(r4+12) = #0xFFFFFF81 }
	{ memw(r4+8) = #0x1; memw(r4+4) = #0x1 }
	{ memw(r4) = #0x1; memw(r3+8) = #0x1 }
	{ memw(r3) = #0x1; memw(r3+12) = #0x1 }
	{ memw(r3+4) = #0xFFFFFF81 }
	{ r2 = memw(r4+16) }
	{ memw(r2) = r18 }
	{ r7 = memw(r3+16) }
	{ memw(r7) = r17; memw(r4+24) = #0x4 }
	{ memw(r3+24) = #0x4 }

l000176BC:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ r25:r24 = memd(r29+24); r27:r26 = memd(r29+16) }
	{ dealloc_return }

;; quantize_check: 000176D0
quantize_check proc
	{ r0 = #0x0; jump quantize_check__merged }

;; quantize_check__merged: 000176D4
;;   Called from:
;;     000176D0 (in quantize_check)
;;     000178DC (in dequantize_check)
quantize_check__merged proc
	{ p0 = cmp.eq(r2,#0x1); r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ if (p0) jump:nt 00017700; if (p0) r1 = #0xA7 }

l000176E8:
	{ immext(#0x105C0); r4 = add(PC,#0x105EF); r0 = p0; memb(r29+2) = r0.new }
	{ r1 = #0x9E }

l00017700:
	{ immext(#0x10540); r4 = add(PC,#0x10569); r0 = p0 }
	{ memw(r29+8) = r0 }
	{ r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = memw(r29+8) }
	{ p1 = r0; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 00017754 }

l0001772C:
	{ r3 = add(PC,#0x1D); if (!p1) jump:nt 00017748; r1 = #0x9F }

l00017738:
	{ immext(#0x10540); r3 = add(PC,#0x1054D); r1 = #0xA8 }

l00017744:
	{ call errlog_function; r2 = r16 }

l00017748:
	{ r2 = r16 }

l0001774C:
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+16); dealloc_return }

l00017754:
	{ if (p1) jump:nt 00017794; r2 = memw(r17+20) }

l0001775C:
	{ p0 = cmp.eq(r2,#0x3); if (p0.new) jump:nt 00017780; r1 = #0xA0 }

l00017764:
	{ immext(#0x10500); r3 = add(PC,#0x10530); if (!p1) jump:nt 00017744 }

l00017770:
	{ immext(#0x10500); r3 = add(PC,#0x10524); jump 00017744; r1 = #0xA9 }

l00017780:
	{ if (p1) jump:nt 00017798; if (!p1) r1 = #0xA1 }

l00017788:
	{ immext(#0x10540); r4 = add(PC,#0x10569); jump 000177A4 }

l00017794:
	{ p0 = cmp.eq(r2,#0x1); if (!p0.new) jump:nt 00017770 }

l00017798:
	{ immext(#0x10500); r4 = add(PC,#0x1050C); r1 = #0xAA }

l000177A4:
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+16); dealloc_return }

;; dequantize_execute: 000177B4
dequantize_execute proc
	{ r18 = r0; memd(r29-24) = r19:r18; allocframe(#0x38) }
	{ r17 = r1; r7 = memw(r18+4); memd(r29+48) = r17:r16 }
	{ r6 = memw(r18+8) }
	{ memd(r29+24) = r23:r22; memd(r29+32) = r21:r20 }
	{ r4 = memw(r7+8); r5 = memw(r7+4) }
	{ memd(r29+16) = r25:r24; memd(r29+8) = r27:r26 }
	{ r2 = memw(r5+16); r3 = memw(r4+16) }
	{ r22 = memw(r7); r23 = memw(r6) }
	{ r19 = memw(r2); r2 = memw(r3) }
	{ r0 = sfsub(r2,r19); call fmaxf.1.0 }
	{ call fn00009610; immext(#0x437F0000); r1 = #0x437F0000 }
	{ immext(#0x40800000); r5 = #0x40800000; r2 = memw(r22); r3 = memw(r22+4) }
	{ immext(#0x10480); r4 = add(PC,#0x104B6); r6 = memw(r22+8); r7 = memw(r22+12) }
	{ r27 = convert_uw2sf(r2); r26 = convert_uw2sf(r3); r1 = #0x75; r16 = r0 }
	{ r25 = convert_uw2sf(r6); r24 = convert_uw2sf(r7); r2 = r17; r20 = memw(r23+16) }
	{ r3 = sfmpy(r27,r26); r22 = memw(r22+16); memw(r29) = r18 }
	{ r3 = sfmpy(r3,r25) }
	{ r21 = sfmpy(r3,r24) }
	{ r3 = sfmpy(r21,r5) }
	{ r18 = convert_uw2sf(r3):chop; call logmsg_function }
	{ r3 = convert_sf2uw(r27); r0 = #0x0; r2 = memw(r23+20); if (!cmp.gtu(r18,r2.new)) jump:t 00017874 }

l0001785C:
	{ r3 = add(PC,#0x3); r1 = #0x76; r2 = r17 }
	{ call errlog_function }
	{ jump 000178C8; r0 = #0xFFFFFFFF }

l00017874:
	{ r4 = convert_sf2uw(r26); r5 = convert_sf2uw(r25); immext(#0x0); r2 = #0x0 }
	{ p0 = sfcmp.gt(r21,r2); if (p0.new) r2 = #0x1; memw(r23+24) = r18; memw(r23+4) = r4 }
	{ r3 = convert_sf2uw(r24); memw(r23) = r3; memw(r23+8) = r5 }
	{ if (!p0) jump:nt 000178C8; memw(r23+12) = r3 }

l000178A0:
	{ r4 = convert_w2sf(r2); r5 = r19; r2 = add(r2,#0x1); r3 = memb(r22++#1) }
	{ r3 = convert_w2sf(r3); p0 = sfcmp.gt(r21,r4) }
	{ r5 += sfmpy(r16,r3); if (p0) jump:nt 000178A0; r20 = add(r20,#0x4) }

l000178C8:
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }

l000178CC:
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); r27:r26 = memd(r29+8) }
	{ dealloc_return }

;; dequantize_check: 000178DC
dequantize_check proc
	{ r1 = #0x1; jump quantize_check__merged }

;; dequantize_i32_execute: 000178E0
dequantize_i32_execute proc
	{ allocframe(#0x30) }
	{ r7 = memw(r0+4); r6 = memw(r0+8) }
	{ r16 = r1; memd(r29+40) = r17:r16; memd(r29+32) = r19:r18 }
	{ r4 = memw(r7+8); r5 = memw(r7+4) }
	{ memd(r29+24) = r21:r20; memd(r29+16) = r23:r22 }
	{ r2 = memw(r5+16); r3 = memw(r4+16) }
	{ memd(r29+8) = r25:r24; memd(r29) = r27:r26 }
	{ r2 = memw(r2); r3 = memw(r3) }
	{ r18 = memw(r7); r20 = memw(r6) }
	{ r0 = sfsub(r3,r2); call fmaxf.1.0 }
	{ immext(#0x40800000); r5 = #0x40800000; r2 = memw(r18); r3 = memw(r18+4) }
	{ immext(#0x2F800000); r7 = #0x2F800000; r4 = memw(r18+8); r6 = memw(r18+12) }
	{ r21 = sfmpy(r0,r7); r23 = convert_uw2sf(r2); r1 = #0x91 }
	{ r24 = convert_uw2sf(r3); r25 = convert_uw2sf(r4); r2 = r16; r18 = memw(r18+16) }
	{ r22 = convert_uw2sf(r6); r3 = sfmpy(r23,r24); r19 = memw(r20+16) }
	{ immext(#0x102C0); r4 = add(PC,#0x102F4); r3 = sfmpy(r3,r25) }
	{ r17 = sfmpy(r3,r22) }
	{ r3 = sfmpy(r17,r5) }
	{ r26 = convert_uw2sf(r3):chop; call logmsg_function }
	{ r3 = convert_sf2uw(r23); r0 = #0x0; r2 = memw(r20+20); if (!cmp.gtu(r26,r2.new)) jump:t 00017998 }

l00017980:
	{ r3 = add(PC,#0x1F); r1 = #0x92; r2 = r16 }
	{ call errlog_function }
	{ jump 000179EC; r0 = #0xFFFFFFFF }

l00017998:
	{ r4 = convert_sf2uw(r24); r5 = convert_sf2uw(r25); immext(#0x0); r2 = #0x0 }
	{ p0 = sfcmp.gt(r17,r2); if (p0.new) r2 = #0x1; memw(r20+24) = r26; memw(r20+4) = r4 }
	{ r3 = convert_sf2uw(r22); memw(r20) = r3; memw(r20+8) = r5 }
	{ if (!p0) jump:nt 000179EC; memw(r20+12) = r3 }

l000179C8:
	{ r4 = convert_w2sf(r2); r2 = add(r2,#0x1); r3 = memw(r18++#4) }
	{ r3 = convert_w2sf(r3); p0 = sfcmp.gt(r17,r4) }
	{ r3 = sfmpy(r21,r3); if (p0) jump:nt 000179C8; r19 = add(r19,#0x4); memb(r19) = r3.new }

l000179EC:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }

l000179F0:
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); r27:r26 = memd(r29) }
	{ dealloc_return }

;; logmsg_function: 00017A00
;;   Called from:
;;     00017574 (in quantize_execute)
;;     00017714 (in quantize_check__merged)
;;     000177A4 (in quantize_check__merged)
;;     00017840 (in dequantize_execute)
;;     00017964 (in dequantize_i32_execute)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00017A24 }

l00017A10:
	{ r0 = add(PC,#0x1A); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

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
	{ immext(#0x101C0); r0 = add(PC,#0x101FE); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; fmaxf.1.0: 00017A4C
;;   Called from:
;;     000175D4 (in quantize_execute)
;;     000177E4 (in dequantize_execute)
;;     0001790C (in dequantize_i32_execute)
fmaxf.1.0 proc
	{ immext(#0x38D1B700); r2 = #0x38D1B717 }
	{ jump fn00009600; r1:r0 = combine(r0,r2) }
00017A5C                                     00 00 00 00             ....

;; relu_execute: 00017A60
relu_execute proc
	{ immext(#0x10300); r4 = add(PC,#0x1033E); memd(r29-16) = r17:r16; allocframe(#0x48) }
	{ r17:r16 = combine(r1,r0) }
	{ r1 = #0x50; r2 = memw(r16+4); memd(r29+40) = r23:r22 }
	{ r3 = memw(r16+8) }
	{ memd(r29+48) = r21:r20; memd(r29+32) = r25:r24 }
	{ r22 = memw(r2); r25 = memw(r3) }
	{ r21 = memw(r3+8); r3 = memw(r3+4) }
	{ r5 = memw(r22); r6 = memw(r22+4) }
	{ r19 = memw(r2+4); memd(r29+56) = r19:r18 }
	{ r20 = memw(r2+8) }
	{ r3 = mpyi(r6,r5); r2 = r17; r7 = memw(r22+8); memw(r29+20) = r3 }
	{ r3 = mpyi(r3,r7); r8 = memw(r22+12); r9 = memw(r20+16) }
	{ r12 = memw(r19+16); memd(r29+24) = r27:r26 }
	{ r23 = memw(r22+16); r24 = memw(r25+16) }
	{ r27 = memw(r12); r18 = memw(r9) }
	{ r26 = mpyi(r3,r8); call logmsg_function; memw(r29) = r16 }
	{ immext(#0x10300); r4 = add(PC,#0x10324); r2 = memw(r20+16); r3 = memw(r19+16) }
	{ r1 = #0x53; r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ r1 = sfsub(r18,r27); call fn00009600; immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ immext(#0x0); r2 = #0x0 }
	{ r2 = sfsub(r2,r27) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; p0 = cmp.eq(r26,#0x0); r3 = memw(r25+20); if (!cmp.gtu(r26,r3.new)) jump:t 00017B64 }

l00017B4C:
	{ r3 = add(PC,#0x2D); r1 = #0x57; r2 = r17 }
	{ call errlog_function }
	{ jump 00017C48; r0 = #0xFFFFFFFF }

l00017B64:
	{ r3 = memw(r22); r4 = memw(r22+4) }
	{ memw(r25+4) = r4; memw(r25) = r3 }
	{ r6 = memw(r22+8) }
	{ memw(r25+8) = r6 }
	{ r7 = memw(r22+12) }
	{ if (p0) jump:nt 00017BB4; memw(r25+12) = r7; memw(r25+24) = r26 }

l00017B88:
	{ loop0(00017BA0,r26); p0 = cmp.gt(r2,#0xFF); p1 = cmp.gt(r2,#0xFFFFFFFF); if (p0.new) r2 = #0xFFFFFFFF }
	{ if (!p1) r2 = #0x0 }
	{ r4 = and(r2,#0xFF) }
	{ r3 = memb(r23++#1) }
	{ p0 = cmp.gtu(r3,r4); if (!p0.new) r3 = add(r2,#0x0) }
	{ nop; memb(r24++#1) = r3 }

l00017BB4:
	{ r2 = add(r19,#0x10); r18 = memw(r29+20) }
	{ call relu_execute.extracted_region; r1:r0 = combine(r19,r18) }
	{ r2 = memw(r20); r3 = memw(r20+4) }
	{ memw(r21+4) = r3; memw(r21) = r2 }
	{ r2 = memw(r20+8) }
	{ memw(r21+8) = r2 }
	{ r7 = memw(r20+12) }
	{ memw(r21+12) = r7 }
	{ r2 = memw(r20+24) }
	{ r6 = memw(r21+20); if (!cmp.gtu(r2,r6.new)) jump:t 00017BF0 }

l00017BEC:
	{ r19 = add(r21,#0x10) }

l00017BF0:
	{ r19 = add(r21,#0x10); r0 = memw(r21+16); memw(r21+24) = r2 }
	{ call fn00009560; r2 = memw(r20+24); r1 = memw(r20+16) }
	{ immext(#0x10200); r4 = add(PC,#0x10213); r2 = memw(r19); r3 = memw(r18+16) }
	{ r1 = #0x64; r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ immext(#0x10180); r4 = add(PC,#0x10197); r1 = #0x65; r2 = r17 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }

l00017C48:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }
00017C5C                                     00 C0 00 7F             ....

;; relu_check: 00017C60
relu_check proc
	{ immext(#0x10140); r4 = add(PC,#0x10170); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x98; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0x100C0); r3 = add(PC,#0x100D5); r1 = #0x99 }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x3)) jump:t 00017CE8 }

l00017C94:
	{ r3 = add(PC,#0x10); r1 = #0x9A }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x3)) jump:t 00017CE8 }

l00017CA8:
	{ r5 = #0x0; r4 = memw(r17+4) }

l00017CAC:
	{ r2 = add(r2,#0x4); r5 = add(r5,#0x1); if (cmp.gt(r5.new,#0x2)) jump:nt 00017CF8 }

l00017CBC:
	{ r3 = add(PC,#0x38); r6 = memw(r4++#4); if (!cmp.eq(r6.new,#0x0)) jump:t 00017CD4 }

l00017CCC:
	{ r1 = #0x9C }
	{ immext(#0x10080); r3 = add(PC,#0x100AB); r6 = memw(r17+8) }

l00017CD4:
	{ r3 = add(PC,#0x2B); r6 = memw(r17+8) }

l00017CDC:
	{ r6 = memw(r4+r2); if (!cmp.eq(r6.new,#0x0)) jump:t 00017CAC }

l00017CE8:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00017CF8:
	{ immext(#0x100C0); r4 = add(PC,#0x100EE); r1 = #0x9F; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }
00017D18                         00 40 00 7F 00 C0 00 7F         .@......

;; reluX_execute: 00017D20
reluX_execute proc
	{ immext(#0x10040); r4 = add(PC,#0x1007E); memd(r29-16) = r17:r16; allocframe(#0x40) }
	{ r16 = r0 }
	{ r3 = memw(r16+4); memd(r29+24) = r25:r24 }
	{ r18 = r1; r1 = #0x81; r5 = memw(r16+8); memd(r29+48) = r19:r18 }
	{ r2 = r18; r25 = memw(r3); memd(r29+16) = r27:r26 }
	{ r6 = memw(r3+12); r19 = memw(r3+4) }
	{ r17 = memw(r3+8); r0 = memw(r25) }
	{ r7 = memw(r25+4); r8 = memw(r25+8) }
	{ r3 = mpyi(r7,r0); r26 = memw(r5); r9 = memw(r25+12) }
	{ r3 = mpyi(r3,r8); r12 = memw(r17+16); r6 = memw(r6+16) }
	{ r13 = memw(r19+16) }
	{ memd(r29+40) = r21:r20; memd(r29+32) = r23:r22 }
	{ r14 = memw(r5+8); r5 = memw(r5+4) }
	{ memw(r29+12) = r14 }
	{ r27 = mpyi(r3,r9); r23 = memw(r25+16); memw(r29+8) = r5 }
	{ r24 = memw(r26+16); r20 = memw(r13) }
	{ r22 = memw(r12); r21 = memw(r6) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r1 = sfsub(r22,r20); call fn00009600; immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = #0x0; r22 = r0 }
	{ r2 = sfsub(r2,r20) }
	{ r0 = sfmpy(r2,r22); call fn00009620 }
	{ r2 = sfsub(r21,r20) }
	{ r20 = convert_uw2sf(r0):chop }
	{ r0 = sfmpy(r2,r22); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; p0 = cmp.eq(r27,#0x0); r3 = memw(r26+20); if (!cmp.gtu(r27,r3.new)) jump:t 00017E20 }

l00017E08:
	{ r3 = add(PC,#0x31); r1 = #0x86; r2 = r18 }
	{ call errlog_function }
	{ jump 00017ED8; r0 = #0xFFFFFFFF }

l00017E20:
	{ r3 = memw(r25); r4 = memw(r25+4) }
	{ if (!p0) r4 = #0x0; memw(r26+4) = r4; memw(r26) = r3 }
	{ r6 = memw(r25+8) }
	{ r0 = memw(r29+8); memw(r26+8) = r6 }
	{ r21 = memw(r29+12); r7 = memw(r25+12) }
	{ if (p0) jump:nt 00017EA0; memw(r26+12) = r7; memw(r26+24) = r27 }

l00017E54:
	{ p2 = cmp.gt(r20,#0xFF); p3 = cmp.gt(r2,#0xFF); p1 = cmp.gt(r20,#0xFFFFFFFF); p0 = cmp.gt(r2,#0xFFFFFFFF) }
	{ loop0(00017E7C,r27); if (p2) r20 = #0xFFFFFFFF; if (p3) r2 = #0xFFFFFFFF }
	{ if (!p0) r2 = add(r4,#0x0); r3 = mux(p1,r20,r4) }
	{ r6 = and(r2,#0xFF) }
	{ r5 = and(r3,#0xFF); r4 = memb(r23++#1) }
	{ p0 = cmp.gtu(r4,r5); if (!p0.new) r4 = add(r3,#0x0) }
	{ r7 = and(r4,#0xFF) }
	{ p0 = cmp.gtu(r6,r7); if (!p0.new) r4 = add(r2,#0x0) }
	{ nop; memb(r24++#1) = r4 }

l00017EA0:
	{ call relu_execute.extracted_region; r2 = add(r19,#0x10); r1 = r19; r19 = add(r17,#0x10) }
	{ call relu_execute.extracted_region; r2 = r19; r1:r0 = combine(r17,r21) }
	{ immext(#0xFF00); r4 = add(PC,#0xFF07); r1 = #0x91; r2 = r18 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }

l00017ED8:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ r25:r24 = memd(r29+24); r27:r26 = memd(r29+16) }
	{ dealloc_return }

;; reluX_check: 00017EEC
reluX_check proc
	{ immext(#0xFE40); r4 = add(PC,#0xFE4E); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xA6; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0xFE40); r3 = add(PC,#0xFE49); r1 = #0xA7 }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x4)) jump:t 00017F8C }

l00017F20:
	{ r3 = add(PC,#0x4); r1 = #0xA8 }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x3)) jump:t 00017F8C }

l00017F34:
	{ r6 = r2; jump 00017F44 }

l00017F38:
	{ r4 = add(r4,#0x4); r5 = add(r5,#0x1); if (cmp.gt(r5.new,#0x2)) jump:nt 00017F78 }

l00017F44:
	{ immext(#0xFE00); r3 = add(PC,#0xFE2C); r7 = memw(r6++#4); if (!cmp.eq(r7.new,#0x0)) jump:t 00017F5C }

l00017F48:
	{ r3 = add(PC,#0x2C); r7 = memw(r6++#4); if (!cmp.eq(r7.new,#0x0)) jump:t 00017F60 }

l00017F58:
	{ r1 = #0xAA }

l00017F5C:
	{ immext(#0xFE00); r3 = add(PC,#0xFE1F); r7 = memw(r17+8) }

l00017F60:
	{ r3 = add(PC,#0x1F); r7 = memw(r17+8) }
	{ r7 = memw(r20+r4); if (!cmp.eq(r7.new,#0x0)) jump:t 00017F38 }

l00017F74:
	{ r1 = #0xAB }

l00017F78:
	{ immext(#0xFDC0); r3 = add(PC,#0xFDF8); r1 = #0xAD }
	{ r2 = memw(r2+12); if (!cmp.eq(r2.new,#0x0)) jump:t 00017F9C }

l00017F8C:
	{ call errlog_function; r2 = r16 }

l00017F90:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00017F9C:
	{ immext(#0xFDC0); r4 = add(PC,#0xFDEB); r1 = #0xAE; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

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
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00017FE0 }

l00017FCC:
	{ r0 = add(PC,#0x18); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

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
	{ immext(#0xFD00); r0 = add(PC,#0xFD3C); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; relu_execute.extracted_region: 00018008
;;   Called from:
;;     00017BBC (in relu_execute)
;;     00017EA0 (in reluX_execute)
;;     00017EB0 (in reluX_execute)
relu_execute.extracted_region proc
	{ allocframe(#0x0) }
	{ r3 = memw(r1); r4 = memw(r1+4) }
	{ memw(r0+4) = r4; memw(r0) = r3 }
	{ r6 = memw(r1+8) }
	{ memw(r0+8) = r6 }
	{ r7 = memw(r1+12) }
	{ memw(r0+12) = r7 }
	{ r3 = memw(r1+24) }
	{ r4 = memw(r0+20); if (cmp.gtu(r3,r4.new)) jump:t 00018040 }

l00018034:
	{ r3 = memw(r1+24); r1 = memw(r2) }
	{ call fn00009560; r2 = r3 }

l00018040:
	{ dealloc_return }
00018044             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; autorequantize_execute: 00018050
autorequantize_execute proc
	{ immext(#0x2F800000); r5 = #0x2F800000; allocframe(#0x48) }
	{ immext(#0xFEC0); r4 = add(PC,#0xFEF9); r17:r16 = combine(r1,r0); memd(r29+64) = r17:r16 }
	{ r1 = #0x4B; r2 = memw(r16+4); memd(r29+40) = r23:r22 }
	{ r3 = memw(r16+8); memd(r29+32) = r25:r24 }
	{ r22 = memw(r2); memd(r29+24) = r27:r26 }
	{ r24 = memw(r2+4); r25 = memw(r2+8) }
	{ r23 = memw(r3); r7 = memw(r22+4) }
	{ r6 = memw(r24+16); r2 = memw(r25+16) }
	{ r8 = memw(r22); r9 = memw(r22+8) }
	{ r7 = mpyi(r7,r8); r0 = memw(r6); r12 = memw(r22+12) }
	{ r6 = mpyi(r7,r9); r2 = r17; r13 = memw(r2) }
	{ r3 = sfsub(r13,r0); r27 = memw(r3+8); r26 = memw(r3+4) }
	{ memd(r29+56) = r19:r18; memd(r29+48) = r21:r20 }
	{ r18 = memw(r22+16); r19 = memw(r23+16) }
	{ r20 = mpyi(r6,r12); r21 = sfmpy(r3,r5); memw(r29) = r16 }
	{ call logmsg_function }
	{ immext(#0xFE80); r4 = add(PC,#0xFE9A); r2 = memw(r25+16); r3 = memw(r24+16) }
	{ r1 = #0x4E; r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ r2 = memw(r23+20); if (!cmp.gtu(r20,r2.new)) jump:t 0001812C }

l00018114:
	{ r3 = add(PC,#0x3); r1 = #0x4F; r2 = r17 }
	{ call errlog_function }
	{ jump 0001834C; r0 = #0xFFFFFFFF }

l0001812C:
	{ p0 = cmp.eq(r20,#0x0); r2 = memw(r22); r3 = memw(r22+4) }
	{ r0 = p0; memw(r23+4) = r3; memw(r23) = r2 }
	{ immext(#0x4F000000); if (p0) r2 = #0x4F000000; r6 = memw(r22+8) }
	{ immext(#0xCF000000); if (p0) r24 = #0xCF000000; memw(r23+8) = r6 }
	{ r7 = memw(r22+12) }
	{ memw(r23+12) = r7; memw(r23+24) = r20 }
	{ if (!p0) jump:nt 00018168; jump 000181CC; memw(r29+16) = r0 }

l00018168:
	{ immext(#0x7FFFFFC0); r2 = #0x7FFFFFFF; r6 = r18 }
	{ immext(#0x80000000); r3 = #0x80000000; r4 = add(r20,#0xFFFFFFFF); r5 = memw(r6++#4) }
	{ p0 = cmp.gtu(r12,#0x1); if (!p0.new) jump:t 000181BC; r7 = add(r4,#0xFFFFFFFF); if (!p0.new) r4 = add(r5,#0x0) }
	{ loop0(0001819C,r7); p0 = cmp.gtu(r4,#0x1); r4 = memw(r6++#4) }
	{ if (!p0) jump:nt 000181B4 }
	{ r2 = min(r2,r5); r3 = max(r5,r3); r7 = r4; r4 = memw(r6++#4) }
	{ r5 = r7; nop }
	{ r3 = max(r5,r3); r2 = min(r2,r5) }
	{ r2 = min(r2,r4); r3 = max(r4,r3) }
	{ r2 = convert_w2sf(r2); r24 = convert_w2sf(r3) }

l000181CC:
	{ r1 = sfmpy(r21,r2); immext(#0x0); r22 = #0x0 }
	{ call fn000097B0; r0 = r22 }
	{ r23 = r0 }
	{ r0 = togglebit(r23,#0x1E) }
	{ r0 += sfmpy(r21,r24); call fmaxf.1.0; immext(#0x437E0000); r24 = #0x437E0000 }
	{ call fn00009610; r25 = r0; r1 = r24 }
	{ call fn00009610; r1 = r25; r25 = r0; r0 = r24 }
	{ r4 = sfsub(r22,r23); immext(#0xD9168700); r2 = #0xD916872B; r23 = r0 }
	{ r4 = sfmpy(r4,r23); immext(#0x3FEFF7C0); r3 = #0x3FEFF7CE }
	{ r1:r0 = convert_sf2df(r4); call fn000097D0 }
	{ r2 = convert_df2w(r1:r0):chop; immext(#0x437F0000); r3 = #0x437F0000; p0 = cmp.gtu(r20,#0x7F) }
	{ r2 = sub(#0x0,r2); immext(#0x4F000000); if (p0) r4 = #0x4F000000 }
	{ r2 = convert_w2sf(r2) }
	{ r22 = sfmpy(r25,r2) }
	{ if (p0) r1:r0 = combine(r21,r22); r24 = r22; if (!p0) r0 = memw(r29+16) }
	{ r24 += sfmpy(r25,r3); if (p0) jump:nt 000182A8; immext(#0x3F000000); if (p0) r3 = #0x3F000000 }

l00018274:
	{ p0 = r0; if (p0.new) jump:nt 000182C8 }

l0001827C:
	{ r0 = sfsub(r24,r22); call fmaxf.1.0 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r5:r4 = combine(r0,r22); r1:r0 = combine(r19,r18); r3:r2 = combine(#0x0,#0x2); memw(r29) = r20 }
	{ call autorequantize_execute.extracted_region }
	{ jump 000182C8 }

l000182A8:
	{ r2 = sfmpy(r21,r23) }
	{ r3 += sfmpy(r2,r4) }
	{ r21 = convert_uw2sf(r3):chop; call fn00009610 }
	{ r1 = convert_uw2sf(r0):chop; r3:r2 = combine(r19,r21); r4 = r20; r0 = r18 }
	{ call quantize_asm }

l000182C8:
	{ immext(#0xFD40); r4 = add(PC,#0xFD61); r2 = memw(r26+16); memw(r26+12) = #0xFFFFFF81 }
	{ r1 = #0x80; memw(r26+8) = #0x1; memw(r26+4) = #0xFFFFFF81 }
	{ memw(r26) = #0x1; memw(r2) = r22 }
	{ memw(r26+24) = #0x4; memw(r27+12) = #0xFFFFFF81 }
	{ r7 = memw(r27+16); memw(r27+4) = #0xFFFFFF81 }
	{ memw(r27) = #0x1; memw(r27+8) = #0x1 }
	{ memw(r7) = r24; memw(r27+24) = #0x4 }
	{ r2 = memw(r27+16); r3 = memw(r26+16) }
	{ r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ immext(#0xFD00); r4 = add(PC,#0xFD1B); r1 = #0x81; r2 = r17 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }

l0001834C:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }

;; autorequantize_check: 00018360
autorequantize_check proc
	{ immext(#0xFC40); r4 = add(PC,#0xFC72); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x107; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0xFAC0); r3 = add(PC,#0xFAED); r1 = #0x108 }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x3)) jump:t 000183E8 }

l00018394:
	{ r3 = add(PC,#0x28); r1 = #0x109 }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x3)) jump:t 000183E8 }

l000183A8:
	{ r5 = #0x0; r4 = memw(r17+4) }

l000183AC:
	{ r2 = add(r2,#0x4); r5 = add(r5,#0x1); if (cmp.gt(r5.new,#0x2)) jump:nt 000183F8 }

l000183BC:
	{ r3 = add(PC,#0x3A); r6 = memw(r4++#4); if (!cmp.eq(r6.new,#0x0)) jump:t 000183D4 }

l000183CC:
	{ r1 = #0x10B }
	{ immext(#0xFC00); r3 = add(PC,#0xFC2D); r6 = memw(r17+8) }

l000183D4:
	{ r3 = add(PC,#0x2D); r6 = memw(r17+8) }

l000183DC:
	{ r6 = memw(r4+r2); if (!cmp.eq(r6.new,#0x0)) jump:t 000183AC }

l000183E8:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l000183F8:
	{ immext(#0xFC00); r4 = add(PC,#0xFC11); r1 = #0x10E; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; requantize_execute: 00018418
requantize_execute proc
	{ immext(#0xFB00); r4 = add(PC,#0xFB39); allocframe(#0x50) }
	{ r17 = r0; immext(#0x2F800000); r6 = #0x2F800000; memd(r29+72) = r17:r16 }
	{ r18 = r1; r2 = memw(r17+4); memd(r29+64) = r19:r18 }
	{ r1 = #0x9F; memd(r29+48) = r23:r22 }
	{ r3 = memw(r17+8); memd(r29+40) = r25:r24 }
	{ r24 = memw(r2); r23 = memw(r2+4) }
	{ r19 = memw(r2+8); r5 = memw(r2+12) }
	{ r8 = memw(r23+16); r9 = memw(r24+4) }
	{ r7 = memw(r19+16); r12 = memw(r24) }
	{ r9 = mpyi(r9,r12); r5 = memw(r5+16); r2 = memw(r2+16) }
	{ r13 = memw(r24+8); r7 = memw(r7) }
	{ r8 = memw(r8); memd(r29+32) = r27:r26 }
	{ r25 = memw(r3); r22 = memw(r3+8) }
	{ r5 = mpyi(r9,r13); r27 = memw(r3+4); r3 = memw(r5) }
	{ r2 = r18; r15 = memw(r2+16); r14 = memw(r24+12) }
	{ r3 = sfsub(r7,r8); memw(r29+24) = r3; memd(r29+56) = r21:r20 }
	{ r26 = mpyi(r5,r14); r0 = memw(r15); r21 = memw(r24+16) }
	{ r16 = memw(r25+16) }
	{ r20 = sfmpy(r3,r6); call logmsg_function; memw(r29+28) = r0; memw(r29) = r17 }
	{ immext(#0xFA80); r4 = add(PC,#0xFAB2); r2 = memw(r19+16); r3 = memw(r23+16) }
	{ r1 = #0xA2; r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r18 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ r4 = r26; r2 = memw(r25+20); if (!cmp.gtu(r26,r2.new)) jump:t 00018514 }

l000184FC:
	{ r3 = add(PC,#0x1B); r1 = #0xA3; r2 = r18 }
	{ call errlog_function }
	{ jump 000186C8; r0 = #0xFFFFFFFF }

l00018514:
	{ immext(#0x0); r23 = #0x0; r2 = memw(r24); r3 = memw(r24+4) }
	{ r27:r26 = combine(r20,r27); memw(r25+4) = r3; memw(r25) = r2 }
	{ r0 = r23; r19 = r22 }
	{ r20 = r4; r7 = memw(r24+8); r1 = memw(r29+24) }
	{ memw(r25+8) = r7; memw(r29+20) = r16 }
	{ r2 = memw(r24+12) }
	{ memw(r29+16) = r21 }
	{ call fn000097B0; memw(r25+12) = r2; memw(r25+24) = r20 }
	{ r22 = r0; r2 = memd(r29+28) }
	{ r0 = sfsub(r2,r22); call fmaxf.1.0; immext(#0x437E0000); r24 = #0x437E0000 }
	{ call fn00009610; r25 = r0; r1 = r24 }
	{ call fn00009610; r16 = r0; r1:r0 = combine(r25,r24) }
	{ r4 = sfsub(r23,r22); immext(#0xD9168700); r2 = #0xD916872B }
	{ immext(#0x3FEFF7C0); r3 = #0x3FEFF7CE }
	{ r4 = sfmpy(r4,r0) }
	{ r1:r0 = convert_sf2df(r4); call fn000097D0 }
	{ r2 = convert_df2w(r1:r0):chop; p0 = cmp.gtu(r20,#0x7F) }
	{ r3 = sub(#0x0,r2); immext(#0x437F0000); r2 = #0x437F0000 }
	{ r3 = convert_w2sf(r3) }
	{ r22 = sfmpy(r16,r3) }
	{ r23 = r22 }
	{ r23 += sfmpy(r16,r2); if (p0) jump:nt 0001860C; if (p0) r16 = add(r27,#0x0) }

l000185D4:
	{ p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 0001864C; r16 = r18 }

l000185DC:
	{ r0 = sfsub(r23,r22); call fmaxf.1.0 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r5:r4 = combine(r0,r22); r3:r2 = combine(r27,#0x0); r0 = memd(r29+16); r1 = memd(r29+20) }
	{ call autorequantize_execute.extracted_region; memw(r29) = r20 }
	{ jump 0001864C }

l0001860C:
	{ r1 = sfsub(r23,r22); r0 = sfmpy(r16,r2) }
	{ call fn00009610 }
	{ immext(#0x3F000000); r2 = #0x3F000000; immext(#0x4F000000); r3 = #0x4F000000 }
	{ r2 += sfmpy(r0,r3); r1:r0 = combine(r16,r22) }
	{ r21 = convert_uw2sf(r2):chop; call fn00009610 }
	{ r1 = convert_uw2sf(r0):chop; r2 = r21; r4 = r20; r3 = memd(r29+20) }
	{ call quantize_asm; r16 = r18; r0 = memd(r29+16) }

l0001864C:
	{ immext(#0xF940); r4 = add(PC,#0xF955); r2 = memw(r26+16); memw(r26+12) = #0xFFFFFF81 }
	{ r1 = #0xC8; memw(r26+8) = #0x1; memw(r26+4) = #0xFFFFFF81 }
	{ memw(r26) = #0x1; memw(r2) = r22 }
	{ memw(r26+24) = #0x4; memw(r19+12) = #0xFFFFFF81 }
	{ r7 = memw(r19+16); memw(r19+4) = #0x1 }
	{ memw(r19) = #0x1; memw(r19+8) = #0x1 }
	{ memw(r7) = r23; memw(r19+24) = #0x4 }
	{ r2 = memw(r19+16); r3 = memw(r26+16) }
	{ r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r16 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ immext(#0xF900); r4 = add(PC,#0xF913); r1 = #0xC9; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l000186C8:
	{ r17:r16 = memd(r29+72); r19:r18 = memd(r29+64) }
	{ r21:r20 = memd(r29+56); r23:r22 = memd(r29+48) }
	{ r25:r24 = memd(r29+40); r27:r26 = memd(r29+32) }
	{ dealloc_return }

;; requantize_check: 000186DC
requantize_check proc
	{ r0 = #0x0; jump requantize_check__merged }

;; requantize_check__merged: 000186E0
;;   Called from:
;;     000186DC (in requantize_check)
;;     00018960 (in requantrange_check)
requantize_check__merged proc
	{ immext(#0xF800); r4 = add(PC,#0xF839); p0 = cmp.eq(r2,#0x1); allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1); if (!p0) r1 = #0x114; memd(r29+32) = r17:r16; memd(r29+24) = r19:r18 }
	{ immext(#0xF800); r18 = add(PC,#0xF83D); if (p0) r1 = #0x11D }
	{ if (p0) jump:nt 00018720; memd(r29+16) = r21:r20; memd(r29+8) = r23:r22 }

l0001870C:
	{ r23:r22 = combine(#0x5,#0x3); r19 = #0x117; r20 = #0x116; r21 = #0x115 }
	{ jump 00018740 }

l00018720:
	{ immext(#0xF700); r4 = add(PC,#0xF72B); r23:r22 = combine(#0x3,#0x2); r19 = #0x120 }
	{ immext(#0xF740); r18 = add(PC,#0xF758); r20 = #0x11F; r21 = #0x11E }

l00018740:
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = r21; r2 = memw(r17+16); if (cmp.eq(r2.new,r23)) jump:t 0001876C }

l00018758:
	{ r3 = add(PC,#0x15) }
	{ call errlog_function; r2 = r16 }

l00018760:
	{ r2 = r16 }
	{ jump 00018798; r0 = #0xFFFFFFFF }

l0001876C:
	{ r1 = r20; r2 = memw(r17+20); if (cmp.eq(r2.new,r22)) jump:t 00018784 }

l0001877C:
	{ r3 = add(PC,#0x0); jump 00018760 }

l00018784:
	{ r2 = r16; r1 = r19; r4 = r18; memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00018798:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; requantrange_execute: 000187A4
requantrange_execute proc
	{ immext(#0x2F800000); r6 = #0x2F800000; allocframe(#0x38) }
	{ immext(#0xF6C0); r4 = add(PC,#0xF6FA); r17:r16 = combine(r1,r0); memd(r29+48) = r17:r16 }
	{ r1 = #0xE5; r2 = memw(r16+4); memd(r29+24) = r23:r22 }
	{ r5 = memw(r16+8); memd(r29+16) = r25:r24 }
	{ r3 = memw(r2); memd(r29+40) = r19:r18 }
	{ r23 = memw(r2+8); r24 = memw(r2+4) }
	{ memd(r29+32) = r21:r20 }
	{ r7 = memw(r23+16); r8 = memw(r3+4) }
	{ r2 = memw(r24+16); r9 = memw(r3) }
	{ r8 = mpyi(r8,r9); r12 = memw(r3+8); r7 = memw(r7) }
	{ r2 = r17; r14 = memw(r2); r13 = memw(r3+12) }
	{ r3 = sfsub(r7,r14); r21 = memw(r3+16); r19 = memw(r5+4) }
	{ r5 = mpyi(r8,r12); r18 = memw(r5); memw(r29) = r16 }
	{ r22 = mpyi(r5,r13) }
	{ r20 = sfmpy(r3,r6); call logmsg_function }
	{ immext(#0xF680); r4 = add(PC,#0xF6A5); r2 = memw(r23+16); r3 = memw(r24+16) }
	{ r1 = #0xE8; r2 = memw(r2); r5 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r5); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r14,#0x0); if (!p0.new) jump:nt 0001886C; r4 = add(r22,#0xFFFFFFFF) }

l00018858:
	{ immext(#0xCF000000); r2 = #0xCF000000; immext(#0x4F000000); r3 = #0x4F000000 }
	{ jump 000188CC }

l0001886C:
	{ immext(#0x80000000); r3 = #0x80000000; immext(#0x7FFFFFC0); r2 = #0x7FFFFFFF }
	{ p0 = cmp.gtu(r14,#0x1); if (p0.new) jump:nt 00018888; r5 = memw(r21++#4) }

l00018884:
	{ r4 = r5; jump 000188BC }

l00018888:
	{ r6 = add(r4,#0xFFFFFFFF); p0 = cmp.gtu(r4,#0x1); r4 = memw(r21++#4) }
	{ loop0(0001889C,r6); if (!p0) jump:nt 000188B4 }

l0001889C:
	{ r3 = max(r5,r3); r2 = min(r2,r5); r6 = r4; r4 = memw(r21++#4) }
	{ r5 = r6; nop }

l000188B4:
	{ r3 = max(r5,r3); r2 = min(r2,r5) }

l000188BC:
	{ r5 = max(r4,r3); r2 = min(r2,r4) }
	{ r3 = convert_w2sf(r2) }
	{ r2 = convert_w2sf(r5) }

l000188CC:
	{ r3 = sfmpy(r20,r3); r6 = #0x0; r5 = memw(r18+16) }
	{ r7 = sfmin(r6,r3); r1 = #0xFE; memw(r18+12) = #0x1; memw(r18+8) = #0x1 }
	{ immext(#0xF600); r4 = add(PC,#0xF604); r2 = sfmpy(r20,r2); memw(r18) = #0x1 }
	{ memw(r18+4) = #0xFFFFFF81 }
	{ memw(r5) = r7; memw(r18+24) = #0x4 }
	{ r3 = memw(r19+16); memw(r19+12) = #0x1 }
	{ memw(r19) = #0x1; memw(r19+4) = #0x1 }
	{ memw(r19+8) = #0x1; memw(r3) = r2 }
	{ r0 = memw(r19+16); memw(r19+24) = #0x4 }
	{ r5 = memw(r18+16) }
	{ r2 = memw(r0) }
	{ r7:r6 = convert_sf2df(r2); r3 = memw(r5) }
	{ r9:r8 = convert_sf2df(r3); r2 = r17; memd(r29+8) = r7:r6 }
	{ call logmsg_function; memd(r29) = r9:r8 }
	{ immext(#0xF5C0); r4 = add(PC,#0xF5D0); r1 = #0xFF; r2 = r17 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); dealloc_return }

;; requantrange_check: 00018960
requantrange_check proc
	{ r1 = #0x1; jump requantize_check__merged }

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
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00018988 }

l00018974:
	{ r0 = add(PC,#0x3B); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

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
	{ immext(#0xF480); r0 = add(PC,#0xF49B); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; autorequantize_execute.extracted_region: 000189B4
;;   Called from:
;;     000182A0 (in autorequantize_execute)
;;     00018600 (in requantize_execute)
autorequantize_execute.extracted_region proc
	{ allocframe(#0x20) }
	{ r21 = togglebit(r4,#0x1E); r20 = #0x0; r6 = memd(r29+40); memd(r29+8) = r21:r20 }
	{ memd(r29+24) = r17:r16 }
	{ r17:r16 = combine(r5,r3); r19:r18 = combine(r1,r0); memd(r29+16) = r19:r18 }
	{ r22 = sub(r6,r2); memd(r29) = r23:r22 }

l000189DC:
	{ r3 = r21; r2 = memw(r18++#4) }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r2,r16) }
	{ r0 = sfmpy(r3,r17); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; r22 = add(r22,#0xFFFFFFFF) }
	{ p1 = cmp.eq(r22,#0x0) }
	{ p2 = cmp.gt(r2,#0xFF); p0 = cmp.gt(r2,#0xFFFFFFFF); if (p2.new) r2 = #0xFFFFFFFF }
	{ if (!p0) r2 = add(r20,#0x0) }
	{ if (!p1) jump:nt 000189DC; memb(r19++#1) = r2 }

l00018A18:
	{ r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); r23:r22 = memd(r29) }
	{ dealloc_return }

;; fmaxf.1.0: 00018A24
;;   Called from:
;;     000181E8 (in autorequantize_execute)
;;     0001827C (in autorequantize_execute)
;;     00018560 (in requantize_execute)
;;     000185DC (in requantize_execute)
fmaxf.1.0 proc
	{ immext(#0x38D1B700); r2 = #0x38D1B717 }
	{ jump fn00009600; r1:r0 = combine(r0,r2) }
00018A34             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; supernode_execute_hvx: 00018A40
;;   Called from:
;;     000191B8 (in supernode_execute_hvx)
supernode_execute_hvx proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x170) }
	{ r2 = memw(r17+4); memd(r29+320) = r27:r26 }
	{ r3 = memw(r17+8); r27 = memb(r17+32) }
	{ p0 = cmp.eq(r27,#0x0); r6 = memw(r17+8); r4 = memw(r2) }
	{ r5 = memw(r2+4); memd(r29+344) = r21:r20 }
	{ memd(r29+336) = r23:r22 }
	{ r7 = memw(r2+24); memd(r29+328) = r25:r24 }
	{ r6 = memw(r2+36); memw(r29+136) = r6 }
	{ r22 = memw(r2+12); r20 = memw(r2+20) }
	{ r25 = memw(r2+28); r21 = memw(r2+32) }
	{ r24 = memw(r2+8); r23 = memw(r2+16) }
	{ r2 = memw(r4+4); memw(r29+108) = r6 }
	{ r1 = memw(r3); r6 = memw(r3+4) }
	{ r2 = memw(r5+8); memw(r29+144) = r2 }
	{ r0 = memw(r4+8); memd(r29+352) = r19:r18 }
	{ memw(r29+80) = r1 }
	{ r6 = memw(r4); memw(r29+72) = r6 }
	{ r2 = memw(r7+4); memw(r29+132) = r2 }
	{ r18 = memw(r5+12); r19 = memw(r5) }
	{ r26 = memw(r5+4); r1 = memw(r7+8) }
	{ r3 = memw(r3+8); memw(r29+140) = r6 }
	{ r6 = memw(r4+12) }
	{ r2 = p0; memw(r29+148) = r2; memw(r29+76) = r7 }
	{ memw(r29+128) = r0; memw(r29+68) = r3 }
	{ memw(r29+96) = r6; memw(r29+116) = r2 }
	{ if (p0) jump:nt 00018B24 }

l00018AF0:
	{ if (p0.new) jump:nt 00018B1C; p0 = cmp.eq(r27,#0x2); if (p0.new) r2 = memw(r29-128) }

l00018AFC:
	{ if (!p0.new) jump:nt 00018B2C; p0 = cmp.eq(r27,#0x1); r0 = #0x0; memw(r29+120) = r1 }

l00018B08:
	{ r1 = memw(r29+120); r2 = memw(r29+128) }
	{ r0 = r1 }
	{ r0 += add(r2,#0xFFFFFFFF); jump 00018B24 }

l00018B1C:
	{ r2 = sub(r2,r26) }
	{ r0 = add(r2,r1) }

l00018B24:
	{ call fn00009760; memw(r29+120) = r1 }

l00018B2C:
	{ if (p0.new) jump:nt 00018B7C; p0 = cmp.eq(r27,#0x2); if (p0.new) r1 = memw(r29-108); if (p0.new) r2 = memw(r29-112) }

l00018B3C:
	{ if (p0.new) jump:nt 00018B6C; p0 = cmp.eq(r27,#0x1); if (p0.new) r2 = memw(r29-112); if (p0.new) r1 = memw(r29-108) }

l00018B4C:
	{ r16 = r0; r5 = #0x0; r0 = memd(r29+116); memw(r29+124) = r16 }
	{ p0 = r0; if (!p0.new) jump:nt 00018B90; if (p0.new) r0 = memw(r29-112); if (p0.new) r1 = memw(r29-108) }

l00018B68:
	{ jump 00018B88 }

l00018B6C:
	{ r0 = r1; r16 = r0; memw(r29+124) = r16 }
	{ r0 += add(r2,#0xFFFFFFFF); jump 00018B88 }

l00018B7C:
	{ r2 = sub(r2,r19); r16 = r0; memw(r29+124) = r16 }
	{ r0 = add(r2,r1) }

l00018B88:
	{ call fn00009760 }
	{ r5 = r0 }

l00018B90:
	{ r4 = add(r18,#0x1F); r1 = #0x0; r0 = add(r29,#0xE8); r2 = memw(r29+140) }
	{ r4 = and(r4,#0xFFFFFFE0); r27 = add(r29,#0xE8); r6 = memw(r25+16); memw(r29+100) = r18 }
	{ r3 = mpyi(r18,r2); r2 = #0x50 }
	{ r3 = mpyi(r3,r16); r18 = r5; memw(r29+56) = r6; memw(r29+104) = r4 }
	{ r3 = mpyi(r3,r5) }
	{ call fn000095F0; memw(r29+116) = r3 }
	{ r0 = add(r27,#0x34); r1 = #0x0; memw(r29+232) = r17 }
	{ call fn00009740; memw(r29+64) = r0 }
	{ r2 = #0x50; r25 = add(r29,#0x98); r1 = #0x0; r0 = add(r29,#0x98) }
	{ call fn000095F0 }
	{ r2 = setbit(r25,#0x4); r0 = add(r25,#0x34); r1 = #0x0; memw(r29+152) = r17 }
	{ call fn00009740; memw(r29+60) = r0; memw(r2) = #0x1 }
	{ immext(#0x437F0000); r24 = #0x437F0000; r2 = memw(r22+16); r3 = memw(r24+16) }
	{ r1 = r24; r4 = memw(r21+16); r7 = memd(r29+108) }
	{ r2 = memw(r2); r25 = memw(r3) }
	{ r0 = sfsub(r2,r25); r5 = memw(r20+16); r6 = memw(r23+16) }
	{ r7 = memw(r7+16); memw(r29+84) = r2 }
	{ r20 = memw(r6); r21 = memw(r5) }
	{ memw(r29+88) = r20 }
	{ r27 = memw(r4); memw(r29+92) = r21 }
	{ call fn00009610; r23 = memw(r7) }
	{ r2 = sfsub(r21,r20); r22 = r0 }
	{ call fn00009610; r1:r0 = combine(r24,r2) }
	{ r20 = sfsub(r23,r27); r21 = r0 }
	{ call fn00009610; r1:r0 = combine(r24,r20) }
	{ r22 = sfmpy(r22,r21) }
	{ call fn00009610; r1 = r22 }
	{ call fn00009610; r1:r0 = combine(r22,r23); memw(r29+52) = r0 }
	{ call fn000097E0 }
	{ r2 = r0; r0 = r20; r3 = memw(r29+136) }
	{ r20 = convert_uw2sf(r2):chop }
	{ call fmaxf.1.0; r23 = memw(r3+4) }
	{ call fn00009610; r1:r0 = combine(r0,r24) }
	{ immext(#0x0); r2 = #0x0 }
	{ r2 = sfsub(r2,r27) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = mpyi(r26,r19); r9 = asl(r19,#0x1); r3 = memw(r29+132); r13 = memw(r29+120) }
	{ r7 = add(#0x3,mpyi(r18,r16)); r4 = convert_uw2sf(r0):chop; r5 = #0x0; r12 = memw(r29+96) }
	{ r2 = add(#0xF,mpyi(r2,r3)); r8 = add(r12,#0xF); r0 = #0x1; p2 = cmp.eq(r13,#0x2) }
	{ r15 = max(r4,r5); r3 = and(r2,#0xFFFFFFF0); p0 = cmp.eq(r12,#0x3); r2 = and(r7,#0xFFFFFFFC) }
	{ r8 = and(r8,#0xFFFFFFF0); p1 = cmp.eq(r3,#0xA0); p3 = cmp.eq(r3,#0x20) }
	{ p1 = fastcorner9(p2,p1) }
	{ p2 = fastcorner9(p2,p3) }
	{ p1 = fastcorner9(p0,p1) }
	{ p0 = fastcorner9(p0,p2); p2 = cmp.eq(r19,#0x7) }
	{ p1 = fastcorner9(p2,p1); p2 = cmp.eq(r19,#0x3) }
	{ p0 = fastcorner9(p2,p0); p2 = cmp.eq(r26,#0x7) }
	{ p1 = fastcorner9(p2,p1); p2 = cmp.eq(r26,#0x3) }
	{ p0 = or(p1,and(p0,p2)); if (!p0.new) jump:nt 00018D60; if (!p0.new) r14 = memw(r29-112); if (!p0.new) r7 = memw(r29-128) }

l00018D48:
	{ r3 = mpyi(r16,r3); r5 = asl(r19,#0x1); r4 = r18; memw(r29+112) = r18 }
	{ r4 += add(r5,#0x2) }
	{ r3 = mpyi(r3,r4); jump 00018DA8 }

l00018D60:
	{ r4 = sub(r26,r7); r6 = sub(r19,r14); r3 = r16; r5 = r18 }
	{ r1 = memw(r29+148); memw(r29+112) = r18 }
	{ r3 = add(r4,mpyi(r3,r13)); r5 = add(r6,mpyi(r5,r1)); r4 = r7 }
	{ r3 += lsr(r3,#0x1F); r7 = r5 }
	{ r7 += lsr(r7,#0x1F); r4 += asr(r3,r0) }
	{ r4 = mpyi(r4,r8); r7 = clrbit(r7,#0x0) }
	{ r5 += add(r9,r14); r7 = sub(r5,r7) }
	{ r1 = sub(r5,r7) }
	{ r3 = mpyi(r4,r1) }

l00018DA8:
	{ r2 = asl(r2,#0x2); p1 = !cmp.eq(r26,00000001); r7 = memw(r29+104); r27 = memw(r29+100) }
	{ p2 = !cmp.eq(r19,00000001); p3 = cmp.eq(r27,r7); r5 = memd(r29+116); r1 = memd(r29+124) }
	{ r6 = mpyi(r2,r7); p1 = or(p2,p1); r5 = add(r5,r27); r7 = memw(r1+4) }
	{ r3 += add(r2,r6) }
	{ r1 = addasl(r3,r5,#0x2) }
	{ r21 = add(r1,#0x580); r4 = memb(r17+32); if (cmp.eq(r4.new,#0x2)) jump:t 00018E14 }

l00018DEC:
	{ if (!p0.new) jump:nt 00018E20 }

l00018DF0:
	{ p0 = and(p0,p0); r24 = r13; memw(r29+40) = r7; memw(r29+44) = r15 }
	{ r0 = p0; r18 = r12; memw(r29+48) = r22; memw(r29+108) = r16 }
	{ memw(r29+32) = r20; memw(r29+36) = r0 }
	{ jump 00018E38 }

l00018E14:
	{ if (p0) jump:nt 00018DF0; nop }

l00018E1C:
	{ r0 = p3; r24 = r13; r18 = r12; memw(r29+44) = r15 }

l00018E20:
	{ r24 = r13; r18 = r12; memw(r29+44) = r15 }

l00018E2C:
	{ memw(r29+36) = r0; memw(r29+40) = r7 }
	{ memw(r29+48) = r22; memw(r29+32) = r20 }
	{ memw(r29+108) = r16 }

l00018E38:
	{ immext(#0xF6C0); r4 = add(PC,#0xF6E7); r2 = memw(r17+28); r20 = memd(r29+84) }
	{ r1 = #0x3D8; r16 = memw(r29+124) }
	{ r3:r2 = combine(#0x2,r16); r22 = memd(r29+88); memw(r29+4) = r2 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r7:r6 = convert_sf2df(r20); r9:r8 = convert_sf2df(r25); r20 = r18; r1 = #0x3D9 }
	{ immext(#0xF8C0); r4 = add(PC,#0xF8EE); r3:r2 = combine(#0x2,r16); r5 = memw(r29+128) }
	{ r18 = memw(r29+140); memd(r29+24) = r7:r6 }
	{ r7 = memw(r29+144); memw(r29+8) = r5 }
	{ memd(r29+16) = r9:r8; memw(r29+12) = r20 }
	{ memw(r29+4) = r7; memw(r29) = r18 }
	{ call logmsg_function }
	{ r9:r8 = convert_sf2df(r22); r1 = #0x3DA; r2 = memw(r29+92) }
	{ immext(#0xF8C0); r4 = add(PC,#0xF8D6); r22 = memw(r29+132); memd(r29+16) = r9:r8 }
	{ r7:r6 = convert_sf2df(r2); r3:r2 = combine(#0x2,r16); memw(r29+12) = r22; memw(r29+4) = r19 }
	{ memd(r29+24) = r7:r6; memw(r29+8) = r26 }
	{ call logmsg_function; memw(r29) = r27 }
	{ immext(#0xF680); r4 = add(PC,#0xF6A3); r3:r2 = combine(#0x2,r16); r1 = #0x3DB }
	{ r5 = memw(r29+148); memw(r29+4) = r24 }
	{ call logmsg_function; memw(r29) = r5 }
	{ immext(#0xF680); r4 = add(PC,#0xF69A); r3:r2 = combine(#0x2,r16); r1 = #0x3DC }
	{ call logmsg_function; r5 = memb(r17+32); memb(r29) = r5.new }
	{ r4 = add(PC,#0x13); r7 = memw(r29+108); memw(r29+12) = r27 }
	{ r3:r2 = combine(#0x2,r16); memw(r29) = r18 }
	{ r1 = #0x3DD; r5 = memd(r29+112); memw(r29+8) = r7 }
	{ memw(r29+4) = r5 }
	{ call logmsg_function }
	{ immext(#0xF840); r4 = add(PC,#0xF869); r25 = r23; r19 = memw(r29+136) }
	{ r7:r6 = convert_sf2df(r25); r1 = #0x3DF; r3:r2 = combine(#0x2,r16); r5 = memw(r19+8) }
	{ immext(#0xF840); r8 = add(PC,#0xF85D); memd(r29) = r7:r6 }
	{ immext(#0xF840); r9 = add(PC,#0xF862) }
	{ p0 = !cmp.eq(r5,00000000); if (!p0.new) r8 = add(r9,#0x0) }
	{ call logmsg_function; memw(r29+8) = r8 }
	{ immext(#0xF640); r3 = add(PC,#0xF646); r1 = #0x3E0; p0 = cmp.eq(r20,r22) }
	{ if (!p0) jump:nt 0001903C }

l00018F90:
	{ r2 = memw(r16+8); if (!cmp.gtu(r21,r2.new)) jump:t 00018FB0 }

l00018F9C:
	{ r3 = add(PC,#0x3F); r1 = #0x3E2; memw(r29+4) = r2 }
	{ r2 = r16; memw(r29) = r21 }
	{ jump 00019040 }

l00018FB0:
	{ r2 = memw(r29+80); r24 = memw(r29+116) }
	{ r21 = r24; r4 = memw(r2+20); if (!cmp.gtu(r24,r4.new)) jump:t 00018FDC }

l00018FC8:
	{ r3 = add(PC,#0x2D); r1 = #0x3E5; memw(r29+4) = r24 }
	{ r2 = r16; memw(r29) = r4 }
	{ jump 00019040 }

l00018FDC:
	{ immext(#0xF600); r3 = add(PC,#0xF62F); r1 = #0x3E7; r2 = memw(r29+76) }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x1)) jump:t 0001903C }

l00018FF8:
	{ r3 = add(PC,#0x28); r1 = #0x3E8; r2 = memw(r29+76) }
	{ r2 = memw(r2+12); if (!cmp.eq(r2.new,#0x1)) jump:t 0001903C }

l00019010:
	{ r3 = add(PC,#0x21); r4 = #0x4; r2 = memd(r29+72) }
	{ r1 = #0x3E9 }
	{ r2 = memw(r2+20); if (cmp.gtu(r4,r2.new)) jump:t 0001903C }

l00019028:
	{ r3 = add(PC,#0x17); r1 = #0x3EA; r2 = memw(r29+68) }
	{ r2 = memw(r2+20); if (cmp.gtu(r2.new,#0x3)) jump:t 0001904C }

l0001903C:
	{ r2 = r16 }

l00019040:
	{ call errlog_function }
	{ jump 00019218; r0 = #0xFFFFFFFF }

l0001904C:
	{ r0 = memw(r29+36) }
	{ p0 = r0; if (!p0.new) jump:nt 0001906C; if (p0.new) r1 = #0x3EB }

l0001905C:
	{ immext(#0xF740); r4 = add(PC,#0xF76B); r3:r2 = combine(#0x2,r16) }
	{ call logmsg_function }

l0001906C:
	{ r1 = #0x3EC; r2 = memd(r29+104); r20 = memd(r29+48) }
	{ p0 = cmp.eq(r27,r2); r22 = memw(r29+40) }
	{ if (!p0) jump:nt 00019090 }

l00019080:
	{ immext(#0xF740); r4 = add(PC,#0xF760); r3:r2 = combine(#0x2,r16) }
	{ call logmsg_function }

l00019090:
	{ p0 = cmp.gt(r27,#0x0); r2 = memd(r29+80); r7 = memd(r29+108) }
	{ r3 = memd(r29+112); r5 = memd(r29+56) }
	{ memw(r2+24) = r21; memw(r2+4) = r3 }
	{ memw(r2) = r18 }
	{ r6 = memd(r29+52); memw(r2+8) = r7 }
	{ r7 = memw(r29+44) }
	{ if (!p0) jump:nt 000190DC; immext(#0x3F000000); if (p0) r2 = #0x3F000000; memw(r2+12) = r27 }

l000190BC:
	{ loop0(000190C0,r27) }
	{ r4 = r2; r3 = memb(r5++#1) }
	{ r3 = sub(r3,r7) }
	{ r3 = convert_w2sf(r3) }
	{ r4 += sfmpy(r6,r3) }
	{ r1 = convert_uw2sf(r4):chop; memw(r22++#4) = r1.new }

l000190DC:
	{ call fn00009610; r1:r0 = combine(r20,r25) }

l000190E0:
	{ r1:r0 = combine(r20,r25) }

l000190E4:
	{ call fn00009620 }
	{ immext(#0xFF0000); r0 = #0xFF0000; r2 = r0 }
	{ r1 = convert_uw2sf(r2):chop; call fn00009760 }
	{ immext(#0xC00); r21 = add(PC,#0xC38); r2 = add(r29,#0xE8); r3 = r0 }
	{ call nn_os_work_for_vector; r1:r0 = combine(r21,r16); memw(r29+252) = r3; memw(r29+172) = r3 }
	{ call nn_os_work_for_vector; r2 = add(r29,#0x98); r1:r0 = combine(r21,r16) }
	{ call fn000096A0; r0 = memw(r29+64) }
	{ call fn000096A0; r0 = memw(r29+60) }
	{ r2 = memw(r16+52); if (!cmp.eq(r2.new,#0x1)) jump:t 0001914C }

l0001913C:
	{ r3:r2 = memd(r29+224) }
	{ r1:r0 = add(r1:r0,r3:r2) }
	{ r1:r0 = lsr(r1:r0,#0x1) }
	{ memd(r17+48) = r1:r0 }

l0001914C:
	{ r2 = memw(r19+8); if (!cmp.eq(r2.new,#0x0)) jump:t 000191C4 }

l00019158:
	{ r3 = memw(r29+240) }
	{ r2 = max(r3,r2); r7 = memw(r29+32) }
	{ r2 = add(r2,r7) }
	{ r2 = convert_w2sf(r2) }
	{ r3 = sfmpy(r20,r2) }
	{ p0 = sfcmp.gt(r3,r25); if (!p0.new) jump:nt 000191C4 }

l00019178:
	{ r2 = memw(r19+4) }
	{ p0 = sfcmp.gt(r3,r2); if (!p0.new) jump:nt 00019194 }

l00019184:
	{ r2 = sfadd(r2,r2) }
	{ p0 = sfcmp.gt(r3,r2); if (p0.new) jump:nt 00019184 }

l00019190:
	{ memw(r19+4) = r2 }

l00019194:
	{ immext(#0xF640); r4 = add(PC,#0xF664); r7:r6 = convert_sf2df(r2); r5 = memw(r17+28) }
	{ r3:r2 = combine(#0x1,r16); r1 = #0x419; memw(r29) = r5 }
	{ call logmsg_function; memd(r29+8) = r7:r6 }
	{ call supernode_execute_hvx; r1:r0 = combine(r16,r17) }
	{ jump 00019218 }

l000191C4:
	{ immext(#0xF640); r4 = add(PC,#0xF667); r3 = memd(r29+72); r5 = memd(r29+68) }
	{ r1 = #0x435; r7 = memw(r29+108) }
	{ r2 = memw(r3+16); memw(r3+12) = #0x1 }
	{ memw(r3+8) = #0x1; memw(r3+4) = #0x1 }
	{ memw(r3) = #0x1; memw(r2) = #0x0 }
	{ memw(r3+24) = #0x4; memw(r5) = #0x1 }
	{ r2 = memw(r5+16); memw(r5+8) = #0x1 }
	{ memw(r5+12) = #0x1; memw(r5+4) = #0x1 }
	{ r3:r2 = combine(#0x2,r16); memw(r2) = r25; memw(r5+24) = #0x4 }
	{ r5 = memw(r29+112); memw(r29+12) = r27 }
	{ memw(r29+8) = r7; memw(r29+4) = r5 }
	{ call logmsg_function; memw(r29) = r18 }
	{ r0 = #0x0 }

l00019218:
	{ r17:r16 = memd(r29+360); r19:r18 = memd(r29+352) }
	{ r21:r20 = memd(r29+344); r23:r22 = memd(r29+336) }
	{ r25:r24 = memd(r29+328); r27:r26 = memd(r29+320) }
	{ dealloc_return }

;; supernode_check_ref: 00019234
supernode_check_ref proc
	{ immext(#0xF400); r4 = add(PC,#0xF43F); memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ r1 = #0x43D; r16 = r1; r17 = r0 }
	{ r3:r2 = combine(#0x2,r16); memd(r29+40) = r19:r18; memd(r29+32) = r21:r20 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r25:r24 }
	{ memd(r29+8) = r27:r26; memw(r29) = r17 }
	{ call logmsg_function }
	{ immext(#0xF400); r3 = add(PC,#0xF42A); r1 = #0x43E; r2 = memw(r17+16) }
	{ p0 = cmp.gtu(r2,#0xB); r4 = #0xA; if (cmp.gtu(r4.new,r2)) jump:t 00019418 }

l00019284:
	{ r3 = add(PC,#0xE); r1 = #0x43F }
	{ if (p0) jump:nt 00019418; if (!p0) r1 = #0x440 }

l00019294:
	{ immext(#0xF400); r3 = add(PC,#0xF413) }
	{ r4 = memw(r17+20); if (!cmp.eq(r4.new,#0x3)) jump:t 00019418 }

l000192A8:
	{ r3 = add(PC,#0x1D); r1 = #0x441 }
	{ r24 = memw(r17+4); if (cmp.eq(r24.new,#0x0)) jump:nt 00019418 }

l000192BC:
	{ r3 = add(PC,#0x15); r1 = #0x442 }
	{ r4 = memw(r17+8); if (cmp.eq(r4.new,#0x0)) jump:nt 00019418 }

l000192D0:
	{ r5 = #0x0; r21 = #0x0 }
	{ loop0(000192E0,r2); r3 = r24; r6 = #0x0 }
	{ r7 = memw(r3); if (cmp.eq(r7.new,#0x0)) jump:nt 000192F4 }

l000192EC:
	{ r3 = add(r3,#0x4); r6 = r6 }
	{ r13 = r2; jump 00019314 }

l000192F4:
	{ immext(#0xF3C0); r3 = add(PC,#0xF3E6); r1 = #0x445; memw(r29) = r6 }
	{ jump 00019418 }

l00019308:
	{ r4 = add(r4,#0x4); r5 = add(r5,#0x1); if (cmp.gtu(r5.new,#0x2)) jump:nt 00019330 }

l0001930C:
	{ r5 = add(r5,#0x1); if (cmp.gtu(r5.new,#0x2)) jump:nt 00019334 }

l00019314:
	{ r2 = memw(r4); if (!cmp.eq(r2.new,#0x0)) jump:t 00019308 }

l00019318:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001930C }

l00019320:
	{ r3 = add(PC,#0xC); r1 = #0x44A; memw(r29) = r5 }
	{ jump 00019418 }

l00019330:
	{ r2 = memw(r24+16); r3 = memw(r24+20) }

l00019334:
	{ r3 = memw(r24+20) }

l00019338:
	{ r4 = memw(r24+4) }
	{ r2 = memw(r2+16); r3 = memw(r3+16) }
	{ r18 = memw(r4+16); r22 = memw(r4+8) }
	{ r20 = memw(r2); r2 = memw(r3) }
	{ r0 = sfsub(r2,r20); r23 = memw(r4+4); r25 = memw(r4) }
	{ call fmaxf.1.0; r19 = memw(r4+12) }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ immext(#0x0); r2 = #0x0 }
	{ r2 = sfsub(r2,r20) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r0 = #0xC; r2 = r0 }
	{ r26 = convert_uw2sf(r2):chop; call fn00009500 }
	{ immext(#0xF340); r3 = add(PC,#0xF36B); r2 = mpyi(r23,r25); r1 = #0x460 }
	{ p2 = cmp.eq(r21,#0xB); p0 = cmp.gt(r26,#0xFF); r20 = r0; if (cmp.eq(r20.new,#0x0)) jump:t 00019418 }

l000193B0:
	{ r3 = add(r19,#0x1F); p1 = cmp.gt(r26,#0xFFFFFFFF); if (p0) r25 = #0xFF }
	{ r23 = mpyi(r2,r22); r21 = and(r3,#0xFFFFFFE0); if (!p0) r25 = zxtb(r26); if (!p2) memw(r20+8) = #0x0 }
	{ r22 = and(r4,#0xFFFFFFF0); if (!p1) r25 = #0x0; immext(#0x3F000000); if (!p2) memw(r20+1056964612) = #0x0 }
	{ r1 = mpyi(r22,r21); if (!p2) jump:nt 000193F8 }

l000193E4:
	{ memw(r20+8) = #0x1 }
	{ r2 = memw(r24+40) }
	{ r2 = memw(r2+16) }
	{ r2 = memw(r2) }
	{ memw(r20+4) = r2 }

l000193F8:
	{ call fn00009550; r0 = #0x80; memw(r17+40) = r20 }
	{ immext(#0xF300); r3 = add(PC,#0xF30A); r1 = #0x46B; memw(r20) = r0 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 00019428 }

l00019418:
	{ call errlog_function; r2 = r16 }
	{ jump 0001947C; r0 = #0xFFFFFFFF }

l00019428:
	{ call nn_os_vector_acquire }
	{ r24 = r0; r5:r4 = combine(r21,r22); r1:r0 = combine(r23,r18); r3 = memw(r16+4) }
	{ call pad2d; r2 = r19; memw(r29) = r25 }
	{ r2 = r21; r1 = r22; r0 = memw(r16+4); r3 = memw(r20) }
	{ call transpack }
	{ call nn_os_vector_release; r0 = r24 }
	{ immext(#0xF2C0); r4 = add(PC,#0xF2DF); r3:r2 = combine(#0x2,r16); r1 = #0x471 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001947C:
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); r27:r26 = memd(r29+8) }
	{ dealloc_return }

;; supernode_dtor: 00019490
supernode_dtor proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ memd(r29) = r19:r18 }
	{ r18 = memw(r16+40); if (cmp.eq(r18.new,#0x0)) jump:nt 000194B4 }

l000194A8:
	{ r0 = memw(r18) }
	{ call fn00009510; r0 = r18 }

l000194B4:
	{ r1:r0 = combine(r17,r16); r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ jump node_free_common; deallocframe }

;; supernode_execute_ref: 000194C4
supernode_execute_ref proc
	{ allocframe(#0xB8) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+160) = r21:r20; memd(r29+136) = r27:r26 }
	{ r4 = memw(r2); r7 = memw(r2+8) }
	{ r26 = memb(r0+32); r20 = memw(r2) }
	{ p0 = cmp.eq(r26,#0x0); memw(r29+92) = r4; memw(r29+64) = r7 }
	{ r4 = memw(r2+12); r7 = memw(r2+20) }
	{ r5 = memw(r2+24); memd(r29+168) = r19:r18 }
	{ memw(r29+56) = r4 }
	{ r4 = memw(r2+28); memw(r29+88) = r7 }
	{ r7 = memw(r2+4); r18 = memw(r2+4) }
	{ memd(r29+176) = r17:r16; memw(r29+96) = r4 }
	{ r4 = memw(r3+8); memw(r29+84) = r7 }
	{ r7 = memw(r3); r3 = memw(r3+4) }
	{ r16 = memw(r20+8); memd(r29+152) = r23:r22 }
	{ memd(r29+144) = r25:r24; memw(r29+112) = r0 }
	{ r0 = r16; memw(r29+72) = r1 }
	{ r17 = memw(r2+16); memw(r29+48) = r3 }
	{ r2 = memw(r18); r19 = memw(r20) }
	{ r23 = memw(r20+4); r21 = memw(r20+12) }
	{ r24 = memw(r18+12); r22 = memw(r18+4) }
	{ r1 = memw(r5+8); r3 = memw(r5+4) }
	{ r2 = p0; memw(r29+124) = r7; memw(r29+128) = r2 }
	{ r7 = memw(r18+8) }
	{ memw(r29+52) = r5; memw(r29+44) = r4 }
	{ if (!p0) r2 = sub(r16,r22); memw(r29+100) = r7; memw(r29+80) = r2 }
	{ if (p0) jump:nt 0001957C }

l00019550:
	{ if (p0.new) jump:nt 00019578; p0 = cmp.eq(r26,#0x2); if (!p0.new) r27 = #0x0; if (p0.new) memw(r29+104) = r1 }

l00019560:
	{ if (!p0.new) jump:nt 00019590; p0 = cmp.eq(r26,#0x1); if (p0.new) r1 = memw(r29+104) }

l0001956C:
	{ r0 = r1 }
	{ r0 += add(r16,#0xFFFFFFFF); jump 0001957C }

l00019578:
	{ r0 = add(r2,r1) }

l0001957C:
	{ call fn00009760; r27 = r3; memw(r29+104) = r1 }
	{ r3 = r27; r27 = r0 }

l00019590:
	{ if (p0.new) jump:nt 000195D0; r1:r0 = combine(r3,r3); p0 = cmp.eq(r26,#0x2); if (p0.new) r26 = add(r23,#0x0) }

l000195A0:
	{ if (p0.new) jump:nt 000195C8; p0 = cmp.eq(r26,#0x1); if (p0.new) r26 = add(r23,#0x0) }

l000195AC:
	{ r26 = r23; r23 = r3; r0 = #0x0; r1 = memd(r29+80) }
	{ p0 = r1; if (!p0.new) jump:nt 000195E8; if (!p0) r1:r0 = combine(r23,r26) }

l000195C4:
	{ jump 000195E4 }

l000195C8:
	{ r0 += add(r26,#0xFFFFFFFF); jump 000195E0 }

l000195D0:
	{ r1 = r3; r2 = memw(r29+128) }
	{ r2 = sub(r26,r2) }
	{ r0 = add(r2,r3) }

l000195E0:
	{ r23 = r3 }

l000195E4:
	{ call fn00009760 }

l000195E8:
	{ r4 = r19; r3 = memd(r29+64); r2 = memd(r29+56) }
	{ immext(#0x437F0000); r19 = #0x437F0000; r1 = memd(r29+92); r6 = memd(r29+88) }
	{ r3 = memw(r3+16); r2 = memw(r2+16) }
	{ r4 = mpyi(r27,r4); r7 = memd(r29+84); memw(r29+68) = r4 }
	{ r1 = r19; r12 = memw(r3); r3 = memw(r1+16) }
	{ r8 = memw(r29+96); r2 = memw(r2) }
	{ r5 = memw(r17+16); r13 = memw(r29+124) }
	{ r3 = mpyi(r4,r0); r9 = memw(r8+16); r17 = memw(r3) }
	{ r25 = mpyi(r3,r24); r6 = memw(r6+16); r7 = memw(r7+16) }
	{ r0 = sfsub(r2,r12); r8 = memw(r29+72); memw(r29+80) = r0 }
	{ r14 = memw(r18+16) }
	{ r26 = r0; memw(r29+60) = r26; memw(r29+76) = r23 }
	{ r27 = memw(r20+16); memw(r29+108) = r27 }
	{ r15 = memw(r13+16); memw(r29+56) = r9 }
	{ r9 = memw(r8+4) }
	{ memw(r29+64) = r12; memw(r29+36) = r15 }
	{ memw(r29+96) = r14 }
	{ r18 = memw(r6); memw(r29+40) = r9 }
	{ r23 = memw(r5) }
	{ call fn00009610; r20 = memw(r7); memw(r29+28) = r3 }
	{ r2 = sfsub(r18,r23); memw(r29+84) = r0 }
	{ call fn00009610; r1:r0 = combine(r19,r2); memw(r29+88) = r2 }
	{ r2 = sfsub(r20,r17); r18 = r0 }
	{ call fn00009610; r1:r0 = combine(r19,r2); memw(r29+92) = r2 }
	{ r2 = memw(r29+84) }
	{ r18 = sfmpy(r2,r18); memb(r29+8) = r18.new }
	{ r1 = r18 }
	{ call fn00009610; r1:r0 = combine(r18,r20); memw(r29+84) = r0 }
	{ r2 = r0; r0 = r26 }
	{ r2 = convert_uw2sf(r2):chop; call fmaxf.1.0; memb(r29+6) = r2.new }
	{ r1:r0 = combine(r0,r19) }
	{ r18 = #0x0; r2 = memd(r29+64) }
	{ r2 = sfsub(r18,r2) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = r0; r0 = memd(r29+88) }
	{ r2 = convert_uw2sf(r2):chop; call fmaxf.1.0; memb(r29+22) = r2.new }
	{ r2 = #0x0 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = sfsub(r18,r23); r20 = memw(r29+108) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = r0; r0 = memd(r29+92) }
	{ r2 = convert_uw2sf(r2):chop; call fmaxf.1.0; memb(r29+16) = r2.new }
	{ r2 = #0x0 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = sfsub(r18,r17) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ immext(#0xEDC0); r4 = add(PC,#0xEDC7); r17 = memd(r29+112); r19 = memd(r29+72) }
	{ r18 = convert_uw2sf(r0):chop; r1 = #0xEF; r3:r2 = combine(#0x2,r19) }
	{ r5 = memw(r17+28); memb(r29+1) = r5.new }
	{ memw(r29) = r17 }
	{ immext(#0xEDC0); r4 = add(PC,#0xEDC0); r3:r2 = combine(#0x2,r19); memw(r29+12) = r21 }
	{ r1 = #0xF0 }
	{ r23 = r19; r5 = memd(r29+60); memw(r29+8) = r16 }
	{ r19 = memd(r29+68); memw(r29+4) = r5 }
	{ call logmsg_function; memw(r29) = r19 }
	{ immext(#0xED80); r4 = add(PC,#0xEDB4); r3:r2 = combine(#0x2,r23); r26 = memw(r29+100) }
	{ r5 = memw(r29+128); memw(r29+12) = r26 }
	{ r1 = #0xF1; memw(r29+8) = r22 }
	{ call logmsg_function; memw(r29+4) = r5; memw(r29) = r24 }
	{ immext(#0xED80); r4 = add(PC,#0xEDA3); r3:r2 = combine(#0x2,r23) }
	{ r1 = #0xF2; r7 = memw(r29+104) }
	{ r5 = memd(r29+76); memw(r29+4) = r7 }
	{ call logmsg_function; memw(r29) = r5 }
	{ immext(#0xED80); r4 = add(PC,#0xED9A); r3:r2 = combine(#0x2,r23); r1 = #0xF3 }
	{ call logmsg_function; r5 = memb(r17+32); memb(r29) = r5.new }
	{ r4 = add(PC,#0x13); r5 = memw(r29+80); memw(r29+12) = r24 }
	{ r3:r2 = combine(#0x2,r23); memw(r29+4) = r5; memw(r29) = r19 }
	{ r1 = #0xF4; r17 = r5; memw(r29+8) = r20 }
	{ call logmsg_function }
	{ immext(#0xED80); r3 = add(PC,#0xED8E); r1 = #0xF5; p0 = cmp.eq(r21,r26) }
	{ if (!p0) jump:nt 00019900; if (p0) r3 = add(r25,r24); if (p0) r2 = memw(r23+8) }
