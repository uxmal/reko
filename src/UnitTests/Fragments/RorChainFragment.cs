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

using Reko.Arch.Z80;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Fragments
{
    public class RorChainFragment : ProcedureBuilder
    {
        public RorChainFragment() : base(new Z80ProcessorArchitecture("z80")) { }

        protected override void BuildBody()
        {
            var sp = Frame.EnsureRegister(Registers.sp);
            var a = Frame.EnsureRegister(Registers.a);
            var c = Frame.EnsureRegister(Registers.c);
            var h = Frame.EnsureRegister(Registers.h);
            var l = Frame.EnsureRegister(Registers.l);
            var C = Frame.EnsureFlagGroup(Architecture.GetFlagGroup("C"));
            var Z = Frame.EnsureFlagGroup(Architecture.GetFlagGroup("Z"));
            var SZC = Frame.EnsureFlagGroup(Architecture.GetFlagGroup("SZC"));
            var SZP = Frame.EnsureFlagGroup(Architecture.GetFlagGroup("SZP"));
            var rorc = new PseudoProcedure(
                PseudoProcedure.RorC,
                PrimitiveType.Byte,
                3);
            Assign(sp, Frame.FramePointer);
            Label("m1Loop");
            Assign(a, h);
            Assign(a, Or(a, a));
            Assign(SZC, Cond(a));
            Assign(C, Constant.False());
            Assign(a, Shr(a, Constant.Byte(1)));
            Assign(C, Cond(a));
            Assign(h, a);
            Assign(a, l);
            Assign(a, Fn(rorc, a, Constant.Byte(1), C));
            Assign(C, Cond(a));
            Assign(l, a);
            Assign(c, ISub(c, 1));
            Assign(SZP, Cond(c));
            BranchIf(Test(ConditionCode.NE, Z), "m1Loop");

            Label("m2Done");
            MStore(Word32(0x1000), l);
            MStore(Word32(0x1001), h);
            Return();
        }
    }
}
