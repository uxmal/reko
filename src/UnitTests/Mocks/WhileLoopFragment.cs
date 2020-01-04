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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class WhileLoopFragment : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            var i = Local(PrimitiveType.Int32, "i");
            var sum = Local(PrimitiveType.Int32, "sum");
            Assign(i, 0);
            Assign(sum, 0);

            Label("loopHeader");
            BranchIf(Ge(i, 100), "done");
                Assign(sum, IAdd(sum, i));
                Assign(i, IAdd(i, 1));
                Goto("loopHeader");

            Label("done");
            Return(sum);
        }
    }
}
