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
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.TestCode
{
    public class Factorial
    {
        public static Program BuildSample()
        {
            var pb = new ProgramBuilder();
            pb.Add("fact", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                var r3 = m.Register("r3");
                var cc = m.Flags("cc");
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r2, r1);
                m.Assign(r1, 1);
                m.Assign(cc, m.Cond(m.ISub(r2, r1)));
                m.BranchIf(m.Test(ConditionCode.LE, cc), "m_done");

                m.Assign(sp, m.ISub(sp, 4));
                m.MStore(sp, r2);
                m.Assign(r1, m.ISub(r2, r1));
                m.Call("fact", 0);
                m.Assign(r2, m.Mem32(sp));
                m.Assign(sp, m.IAdd(sp, 4));
                m.Assign(r1, m.IMul(r1, r2));

                m.Label("m_done");
                m.Return();
            });
            pb.Add("main", m =>
            {
                var r1 = m.Register("r1");
                m.Assign(r1, 10);
                m.Call("fact", 0);
                m.MStore(m.Word32(0x400000), r1);
            });
            return pb.BuildProgram();
        }
    }
}
