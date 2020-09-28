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
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Fragments
{
    public class FCmpFragment : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            var m = this;
            var real80 = PrimitiveType.Real80;
            var real96 = PrimitiveType.Real96;
            var int32 = PrimitiveType.Int32;
            var fp0 = m.Register(new RegisterStorage("fp0", 32, 0, real80));
            var a6 = m.Register(new RegisterStorage("a6", 14, 0, PrimitiveType.Ptr32));
            var flagReg = new RegisterStorage("flags", 42, 0, PrimitiveType.Word32);
            var flags = m.Frame.EnsureFlagGroup(flagReg, 0xF, "NZCV", PrimitiveType.Byte);

            m.Assign(fp0, m.Cast(real80, m.Mem(int32, m.ISub(a6, 0x10))));
            m.Assign(
                flags,
                m.Cond(
                    m.FSub(
                        m.Cast(real96, fp0),
                        m.Mem(real96, m.IAdd(a6, 0x08)))));
            m.BranchIf(m.Test(ConditionCode.EQ, flags), "l1");
            m.Return();
        }
    }
}
