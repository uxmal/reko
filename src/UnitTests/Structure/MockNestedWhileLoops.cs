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
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Structure
{
    public class MockNestedWhileLoops : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier i = Declare(PrimitiveType.Int32, "i", Int32(0));

            Label("outer");
            BranchIf(Ge(i, 10), "done");

                Identifier j = Declare(PrimitiveType.Int32, "j", Int32(0));

                Label("inner");
                BranchIf(Ge(j, 10), "done_inner");

                MStore(Word32(0x1234), IAdd(Mem(PrimitiveType.Int32, Word32(0x1234)), j));
                    Assign(j,IAdd(j,1));
                    Goto("inner");
                Label("done_inner");
                Assign(i, IAdd(i, 1));
                Goto("outer");
            Label("done");
            Return();
        }
    }
}
