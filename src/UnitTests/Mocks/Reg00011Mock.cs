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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Text;

namespace Reko.UnitTests.Mocks
{
	public class Reg00011Mock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			Identifier ds = Local16("ds");
			MStore(Word16(0x8416), IAdd(LoadW(0x8416), 1));
			Identifier loc02_26 = Local16("loc02_26");
			Assign(loc02_26, LoadW(0x53BA));
			// succ:  2


			// LocalsOut: loc02(16)l1B96_2DA8:		// block 2, pred: 1 7 6 3
			Label("block2");
			Identifier cx_37 = Local16("cx_37");
			Identifier bx_38 = Local16("bx_38 ");
			Identifier ax_36 = Local16("ax_36");
			BranchIf(Fn("fn1B96_3CA2", AddrOf(PrimitiveType.Ptr32, ax_36), AddrOf(PrimitiveType.Ptr32, cx_37), AddrOf(PrimitiveType.Ptr32, bx_38)), "block7");
			// succ:  3 7

			// DataOut: eax ecx edx esi ax cx bx sp bp di es cs ss ds fs
			// LocalsOut: loc02(16)l1B96_2DAD:		// block 3, pred: 2
			BranchIf(Fn("__bt", LoadW(0x8418), ax_36), "block2");
			// succ:  4 2""

			// DataOut: eax ecx edx esi ax cx bx sp bp di es cs ss ds fs
			// LocalsOut: loc02(16)l1B96_2DB4:		// block 4, pred: 3
			Identifier fs_41 = Local16("fs_41");
            Assign(fs_41, Mem16(Word16(0x7E50)));
			BranchIf(Ne(LoadW(0x7E50), LoadW(0x53C2)), "block6");
			// succ:  5 6

			// DataOut: eax ecx edx esi ax cx bx sp bp di es cs ss ds fs
			// LocalsOut: loc02(16)l1B96_2DC2:		// block 5, pred: 4
			Identifier dx_49 = Local16("dx_49");
			Identifier bx_50 = Local16("bx_50");
			SideEffect(Fn("fn1B96_0540", LoadW(0x7E52), ds, AddrOf(PrimitiveType.Ptr32, dx_49), AddrOf(PrimitiveType.Ptr32, bx_50)));
			// succ:  6

			// DataOut: eax ecx edx esi ax cx sp bp di es cs ss ds fs
			// LocalsOut: loc02(16)l1B96_2DCF:		// block 6, pred: 5 4
			Label("block6");
			SideEffect(Fn(Mem16(IAdd(UMul(ax_36, 0x0002), 0x841A))));
			Identifier loc04_63 = Local16("loc04_63");
			Assign(loc04_63, LoadW(0x0048));
			MStore(Word16(0x004A), LoadW(0x004A));
			MStore(Word16(0x0048), loc04_63);
			MStore(Word16(0x8410), Word16(0x0000));
            BranchIf(Eq(LoadW(0x8410), Word16(0)), "block2");
			// succ:  7 2

			// DataOut: eax ecx edx esi cx sp bp di es cs ss ds fs
			// LocalsOut: loc02(16)l1B96_2DF4:		// block 7, pred: 6 2
			Label("block7");
			Identifier ax_80 = Local16("ax_80");
			Assign(ax_80, LoadW(0x840E));
			MStore(Word16(0x8414), Or(LoadW(0x8414), ax_80));
			BranchIf(Ne(ax_80, 0x0000), "block2");
			// succ:  8 2

			// DataOut: eax ecx edx esi cx sp bp di es cs ss ds fs
			// LocalsOut: loc02(16)l1B96_2E00:		// block 8, pred: 7
			Identifier ax_88 = Local16("ax_88");
			Identifier dx_86 = Local16("dx_86");
			Identifier bx_87 = Local16("bx_87");
			Assign(ax_88, Fn("fn1B96_0540", loc02_26, ds));
			Identifier v19_89 = Local16("v19_89");
			Assign(v19_89, ISub(LoadW(0x8416), 0x0001));
			MStore(Word16(0x8416), v19_89);
			BranchIf(Ne(v19_89, 0x0000), "block11");
			// succ:  9 11

			// DataOut: eax ecx edx esi ax dx bx sp bp di al dl dh es cs ss ds fsl1B96_2E0A:		// block 9, pred: 8
            MStore(Word16(0x8414), Word16(0x0000));
            BranchIf(Eq(LoadW(0x8414), Word16(0x0000)), "block11");
			// succ:  11 10
			// DataOut: eax ecx edx esi ax dx bx sp bp di al dl dh es cs ss ds fsl1B96_2E15_branch:		// block 10, pred: 9
			Identifier ax_96 = Local16("ax_96");
			Identifier bx_97 = Local16("bx_97");
			SideEffect(Fn("fn1B96_02A9", ds, AddrOf(PrimitiveType.Ptr32, ax_96)));
			Return(ax_96);
			// DataOut: eax ecx edx esi ax dx bx sp bp di al dl dh es cs ss fsl1B96_2E19:		// block 11, pred: 9 8
			Label("block11");
			Return(ax_88);
		}

        private MemoryAccess LoadW(ushort i)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, Constant.Word16(i), PrimitiveType.Word16);
        }
	}
}
