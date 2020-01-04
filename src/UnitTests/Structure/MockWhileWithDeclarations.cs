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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Structure
{
    public class MockWhileWithDeclarations : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier i = Local32("i");
            Label("loopHeader");
            Identifier v = Declare(PrimitiveType.Byte, "v", Mem(PrimitiveType.Byte, i));
            Assign(i, IAdd(i, 1));
            BranchIf(Eq(v, 0x20), "exit_loop");

            MStore(Word32(0x00300000), v);
            Goto("loopHeader");
            Label("exit_loop");
            Return();
        }
    }
}
