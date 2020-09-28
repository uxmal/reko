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

namespace Reko.UnitTests.Fragments
{
    public static class EvenOdd
    {
        /// <summary>
        /// Mutually recursive functions that jump into each other's bodies.
        /// </summary>
        /// <returns></returns>
        public static Program BuildProgram()
        {
            var pb = new ProgramBuilder();

            var m = new ProcedureBuilder("even");
            var r1 = m.Register("r1");
            m.BranchIf(m.Eq0(r1), "done");
            m.Assign(r1, m.ISub(r1, 1));
            m.Call("odd", 4);
            m.Assign(r1, m.Not(r1));
            m.Return();

            m.Label("done");
            m.Assign(r1, true);
            m.Return();
            pb.Add(m);

            m = new ProcedureBuilder("odd");
            r1 = m.Register("r1");
            m.BranchIf(m.Eq(r1, 1), "done");
            m.Assign(r1, m.ISub(r1, 1));
            m.Call("event", 4);
            m.Assign(r1, m.Not(r1));
            m.Return();

            m.Label("done");
            m.Assign(r1, true);
            m.Return();
            pb.Add(m);

            return pb.BuildProgram();
        }
    }
}
