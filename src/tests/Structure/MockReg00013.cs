#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion
 
public class Reg00013 : ProcedureMock
{
	protected override BuildBody()
	{
		Identifier ds_66 = Local(PrimitiveType.Selector, "ds_66");
	// Succ:  1
l1796_0003:		// block 1, pred: 0
	Assign(ds_66, Fn("fn05EB", Word16(0x0800)));
	BranchIf(Eq(SegLoadW(ds_66, Word16(0x541E)), 0x0000, "l0067"));
	// succ:  2 3
l1796_005C:		// block 2, pred: 1
	Assign(ds_66, Fn("fnC489", ds_66));
	// succ:  3
l1796_0067:		// block 3, pred: 2 1
	fn313C_C54C();
	// succ:  4
l1796_0078:		// block 4, pred: 3
	Store(SegMem(ds_66,Word16(0x53FE)), 0x0001);
	BranchIf(Fn("fn172D", ds_66),  "l009A");
	// succ:  6 7
l1796_0096_branch:		// block 6, pred: 4
	SideEffect(Fn("fn047A");
	Return();
	// succ:  11
Label("l009A");		// block 7, pred: 7 4
	BranchIf(Mem83[ds_66:0x81B0:word16] == 0x0000, "l00A8
	// succ:  7 9
l1796_00A1:		// block 8
l1796_00A8:		// block 9, pred: 7
	selector ds_107
	word16 bp_108 = fn1796_4AFB(bp_87, 0x5450, ds_89, &ds_107)
	store(Mem109[ds_107:0x5404:word16]) = 0x0000
	fn1796_04BA(ds_107)
	branch Mem109[ds_107:0x53B8:word16] < 0x0004 l1796_049E
	// succ:  12 10
l1796_049E:		// block 10, pred: 9
	word16 dx_119
	word16 bx_120
	fn1796_05D0(0xDCF2, fn1796_059F(0x0800), &dx_119, &bx_120)
	msdos_terminate(0x01)
	// succ:  11
fn1796_0003_exit:		// block 11, pred: 10 6
	// succ: 
l1796_00C1:		// block 12, pred: 9
	word16 bx_124 = Mem109[ds_107:0x53B8:word16]
	store(Mem126[ds_107:0x53BE:word16]) = bx_124 - 0x0001
	store(Mem129[ds_107:0x53C0:word16]) = bx_124 - 0x0002
	fn1796_0540(Mem129[ds_107:0x53C0:word16], ds_107)
	selector es_132 = Mem129[ds_107:0x53C2:word16]
	word16 di_133 = 0x0000
	word16 si_137 = 0x0000
	word16 cx_139 = 0x4000
	// succ:  13
l1796_00E5:		// block 13, pred: 12 14
	branch cx_139 == 0x0000 l1796_00E5_rep
	// succ:  15 14
l1796_00E5_rep:		// block 14, pred: 13
	store(Mem149[es_132:di_133:word32]) = Mem145[0x6C34:si_137:word32]
	cx_139 = cx_139 - 0x0001
	si_137 = si_137 + 0x0004
	di_133 = di_133 + 0x0004
	// succ:  13
l1796_00E8:		// block 15, pred: 13
	fn1796_0540(Mem145[0x0800:0x53BE:word16], 0x0800)
	selector es_156 = Mem145[0x0800:0x53C2:word16]
	word16 di_157 = 0x0000
	word16 si_161 = 0x0000
	word16 cx_163 = 0x4000
	// succ:  16
l1796_0102:		// block 16, pred: 15 17
	branch cx_163 == 0x0000 l1796_0102_rep
	// succ:  18 17
l1796_0102_rep:		// block 17, pred: 16
	store(Mem173[es_156:di_157:word32]) = Mem169[0x5D02:si_161:word32]
	cx_163 = cx_163 - 0x0001
	si_161 = si_161 + 0x0004
	di_157 = di_157 + 0x0004
	// succ:  16
l1796_0105:		// block 18, pred: 16
	store(Mem179[0x0800:0x7E52:word16]) = Mem169[0x0800:0x53BE:word16]
	word16 ax_180 = Mem179[0x0800:0x53C2:word16]
	store(Mem181[0x0800:0x7E50:word16]) = ax_180
	store(Mem182[0x0800:0x5380:word16]) = ax_180
	store(Mem186[0x0800:0x53BC:word16]) = Mem182[0x0800:0x53B8:word16] - 0x0002
	fn1796_0579()
	selector ds_187 = fn1796_2C30()
	branch Mem186[ds_187:0x54A6:word16] == 0x0000 l1796_014A
	// succ:  19 20
l1796_012E:		// block 19, pred: 18
	bp_108 = fn1796_2614(ds_187)
	store(Mem196[ds_187:0x6FF0:ui64]) = Mem186[ds_187:0x6FF0:ui64] - (int32) Mem186[ds_187:0x5418:word16] *s 0x0000F000
	// succ:  20
l1796_014A:		// block 20, pred: 19 18
	selector ds_205
	fn1796_1334(bp_108, ds_187, &ds_205)
	// succ:  21
l1796_014D:		// block 21, pred: 20 21
	branch Mem204[ds_205:0x81B0:word16] == 0x0000 || Mem204[ds_205:0x5404:word16] >= 0x0046 l1796_015B
	// succ:  21 23
l1796_0154:		// block 22
l1796_015B:		// block 23, pred: 21
	word16 bx_215
	selector ds_216
	fn313C_C54C(0x53E7, ds_205, &bx_215, &ds_216)
	// succ:  24
l1796_0169:		// block 24, pred: 23
	fn1796_A3AA(ds_216)
	fn1796_2900(ds_216)
	word16 bp_219
	word16 di_220
	word16 cx_221 = fn1796_2A90(ds_216, &bp_219, &di_220)
	word16 di_224
	selector es_225
	word16 bp_223
	fn1796_1370(fn1796_1760(cx_221, bp_219, di_220, ds_216), ds_216, &bp_223, &di_224, &es_225)
	// succ:  25
l1796_0178:		// block 25, pred: 24 51 42
	branch (Mem242[ds_216:0x919E:word16] & 0x01FF) == 0x0000 l1796_01E0
	// succ:  26 37
l1796_0182:		// block 26, pred: 25
	word16 bp_246
	word16 di_247
	word16 cx_248 = fn1796_7B17(ds_216, &bp_246, &di_247)
	branch Mem242[ds_216:0x0480:word16] == 0x0000 l1796_0199
	// succ:  28 27
l1796_0199:		// block 27, pred: 26
	store(Mem252[ds_216:0xC3CF:word16]) = 0x0000
	// succ:  30
l1796_018C:		// block 28, pred: 26
	cx_248 = fn2387_C010(di_247, ds_216, &bp_246, &di_247, &ds_216)
	// succ:  29
l1796_0197:		// block 29, pred: 28
	// succ:  30
l1796_019F:		// block 30, pred: 29 27
	branch Mem263[ds_216:0x542C:word16] != 0x0000 l1796_01B7
	// succ:  32 31
l1796_01B7:		// block 31, pred: 30
	store(Mem266[ds_216:0x540C:word32]) = 0x00000000
	bp_223 = fn2387_1100(bp_246, ds_216, &ds_216)
	// succ:  34
l1796_01A6:		// block 32, pred: 30
	selector ds_269
	word16 bp_270 = fn1796_02B9(cx_248, bp_246, di_247, ds_216, &ds_269)
	bp_223 = fn2387_1100(bp_270, ds_269, &ds_216)
	// succ:  33
l1796_01B4:		// block 33, pred: 32
	// succ:  34
l1796_01CB:		// block 34, pred: 33 31
	branch Mem275[ds_216:0xC3CF:word16] == 0x0000 l1796_01DA
	// succ:  35 36
l1796_01D4:		// block 35, pred: 34
	store(Mem278[ds_216:0x0412:word16]) = 0x0000
	// succ:  36
l1796_01DA:		// block 36, pred: 35 34
	store(Mem280[ds_216:0x5376:word16]) = 0x0000
	// succ:  37
l1796_01E0:		// block 37, pred: 36 25
	word16 di_285
	word32 ecx_287 = fn1796_19A2(bp_223, ds_216, &bp_223, &di_285, &ds_216)
	word16 ax_288 = fn1796_0C81(ds_216)
	word16 v25_290 = Mem283[ds_216:0x5404:word16]
	store(Mem291[ds_216:0x5404:word16]) = 0x0000
	word16 dx_293 = Mem291[ds_216:0x541A:word16]
	store(Mem295[ds_216:0x541A:word16]) = ax_288
	word16 ax_296 = ax_288 - dx_293
	store(Mem310[ds_216:0x5408:word16]) = SLICE(DPB(ax_296, (byte) (v25_290 - (word16) (ax_296 <u 0x0000)), 8, 8) *u 0x3865, word16, 16)
	word32 ecx_307 = DPB(ecx_287, 0x3865, 0, 16)
	branch (Mem310[ds_216:Mem310[ds_216:0x5A02:word16] + 0x0094:word16] & 0xFFFF) != 0x0000 || (Mem310[ds_216:Mem310[ds_216:0x5BD8:word16] + 0x0094:word16] & 0xFFFF) != 0x0000 l1796_0229
	// succ:  40 39
l1796_0229:		// block 39, pred: 37
	store(Mem326[ds_216:0x5408:word16]) = 0x0000
	// succ:  40
l1796_022F:		// block 40, pred: 39 37
	branch (Mem329[ds_216:0x919E:word16] & 0x02FF) != 0x0000 l1796_023F
	// succ:  41 42
l1796_0239:		// block 41, pred: 40
	store(Mem333[ds_216:0x5408:word16]) = 0x0044
	// succ:  42
l1796_023F:		// block 42, pred: 41 40
	branch (Mem334[ds_216:0x919E:word16] & 0x05FF) == 0x0400 l1796_0178
	// succ:  43 25
l1796_024C:		// block 43, pred: 42
	branch Mem334[ds_216:0x632E:word16] != 0x0000 || Mem334[ds_216:0xD10E:word16] == 0x0000 l1796_025D
	// succ:  45 46
l1796_0253:		// block 44
l1796_025A:		// block 45, pred: 43
	fn1796_B938(0x3865, bp_223, di_285, ds_216)
	// succ:  46
l1796_025D:		// block 46, pred: 45 43
	word16 bx_344
	word16 bp_345
	word16 di_346
	word32 ecx_348 = DPB(ecx_307, fn1796_2AF5(ds_216, &bx_344, &bp_345, &di_346), 0, 16)
	branch (Mem334[ds_216:0x6FCA:byte] & 0x10) == 0x00 l1796_026D
	// succ:  47 48
l1796_0267:		// block 47, pred: 46
	store(Mem352[ds_216:0x5376:word16]) = 0x0001
	// succ:  48
l1796_026D:		// block 48, pred: 47 46
	bp_223 = fn1796_7D05(ecx_348, bx_344, bp_345, di_346, ds_216)
	branch (Mem353[ds_216:0x0412:word16] & 0xFFFF) == 0x0000 l1796_027D
	// succ:  49 50
l1796_027A:		// block 49, pred: 48
	bp_223 = fn1796_03D1(bp_223, ds_216, &ds_216)
	// succ:  50
l1796_027D:		// block 50, pred: 49 48
	selector es_362
	fn313C_C721(ds_216, &es_362)
	// succ:  51
l1796_0288:		// block 51, pred: 50
	// succ:  25

Expression has been evaluated and has no value
>