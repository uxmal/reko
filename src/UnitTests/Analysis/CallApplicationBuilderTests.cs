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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{

    [TestFixture]
    public class CallApplicationBuilderTests
    {
        private readonly RegisterStorage reg2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
        private readonly RegisterStorage reg3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word32);

        [Test]
        public void Cab_Sequence()
        {
            var m = new SsaProcedureBuilder(nameof(Cab_Sequence));
            var r2_1 = m.Reg("r2_1", reg2);
            var r3_2 = m.Reg("r3_2", reg3);
            var r2_r3_4 = m.SeqId("r2_r3_4", PrimitiveType.Word64, reg2, reg3);
            m.Assign(r2_1, 0x0001234);
            m.Assign(r3_2, 0x5678ABCD);
            m.Assign(r2_r3_4, m.Seq(r2_1, r3_2));
            var sigCallee = FunctionType.Action(
                    new Identifier("r2_r3", PrimitiveType.Word64,
                        new SequenceStorage(PrimitiveType.Word64, reg2, reg3)));
            var callee = new ProcedureConstant(
                PrimitiveType.Ptr32,
                new ExternalProcedure("callee", sigCallee));
            var stmCall = m.Call(callee, 0,
                new Identifier[] { r2_r3_4 },
                new Identifier[] { });
            m.Return();

            var cab = new CallApplicationBuilder(
                m.Ssa,
                stmCall,
                (CallInstruction) stmCall.Instruction,
                callee);
            var instr = cab.CreateInstruction(sigCallee, null);

            Assert.AreEqual("callee(r2_r3_4)", instr.ToString());
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        // If for some reason we haven't made a sequence of the register elements,
        // create a SEQ in the application
        [Test]
        public void Cab_Sequence_MkSequence()
        {
            var m = new SsaProcedureBuilder(nameof(Cab_Sequence));
            var r2_1 = m.Reg("r2_1", reg2);
            var r3_2 = m.Reg("r3_2", reg3);
            m.Assign(r2_1, 0x0001234);
            m.Assign(r3_2, 0x5678ABCD);
            var sigCallee = FunctionType.Action(
                    new Identifier("r2_r3", PrimitiveType.Word64,
                        new SequenceStorage(PrimitiveType.Word64, reg2, reg3)));
            var callee = new ProcedureConstant(
                PrimitiveType.Ptr32,
                new ExternalProcedure("callee", sigCallee));
            var stmCall = m.Call(callee, 0,
                new Identifier[] { r3_2, r2_1 },
                new Identifier[] { });
            m.Return();

            var cab = new CallApplicationBuilder(
                m.Ssa,
                stmCall,
                (CallInstruction) stmCall.Instruction,
                callee);
            var instr = cab.CreateInstruction(sigCallee, null);

            Assert.AreEqual("callee(SEQ(r2_1, r3_2))", instr.ToString());
            m.Ssa.Validate(s => Assert.Fail(s)); 
        }
    }
}
