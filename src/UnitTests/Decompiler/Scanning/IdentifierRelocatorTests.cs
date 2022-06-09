#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class IdentifierRelocatorTests
    {
        private ProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
        }

        private Instruction RunIdentifierRelocator(Instruction instr)
        {
            var frameNew = new Frame(default!, PrimitiveType.Word32);
            var replacer = new IdentifierRelocator(m.Frame, frameNew);
            return replacer.ReplaceIdentifiers(instr);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Ir_GlobalVariable()
        {
            var gbl_var = Identifier.Global("gbl_var", PrimitiveType.Int32);
            Instruction instr = m.Assign(gbl_var, 123);

            instr = RunIdentifierRelocator(instr);

            Assert.AreEqual("gbl_var = 123<i32>", instr.ToString());
        }
    }
}
