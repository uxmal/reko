/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class WhileLoopFragment : ProcedureMock
    {
        protected override void BuildBody()
        {
            Identifier i = Local(PrimitiveType.Int32, "i");
            Identifier sum = Local(PrimitiveType.Int32, "sum");
            Assign(i, 0);
            Assign(sum, 0);

            Label("loopHeader");
            BranchIf(Ge(i, 100), "done");
                Assign(sum, Add(sum, i));
                Assign(i, Add(i, 1));
                Jump("loopHeader");

            Label("done");
            Return(sum);
        }
    }
}
