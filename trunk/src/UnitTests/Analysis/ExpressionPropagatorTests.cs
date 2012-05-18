#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Analysis;
using Decompiler.Evaluation;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class ExpressionPropagatorTests
    {
        [Test]
        public void TestCondition()
        {
            var p = new ProgramBuilder();
            var proc = p.Add("main", (m) =>
            {
                m.Label("foo");
                m.BranchCc(ConditionCode.EQ, "foo");
                m.Return();
            });

            var ctx = new SymbolicEvaluationContext(new IntelArchitecture(ProcessorMode.ProtectedFlat), proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx);
            var ep = new ExpressionPropagator(null, simplifier, ctx, new ProgramDataFlow());

            var newInstr = proc.EntryBlock.Succ[0].Statements[0].Instruction.Accept(ep);
            Assert.AreEqual("branch Test(EQ,Z) foo", newInstr.ToString());
        }
    

        [Test]
        public void ConditionOf()
        {
            var p = new ProgramBuilder();
            var proc = p.Add("main", (m) =>
            {
                var szo = m.Frame.EnsureFlagGroup(0x7, "SZO", PrimitiveType.Byte);
                var ebx = m.Frame.EnsureRegister(new RegisterStorage("ebx", 0, PrimitiveType.Word32));
                var v4 = m.Frame.CreateTemporary(PrimitiveType.Word16);

                m.Assign(v4, m.Add(m.LoadW(ebx), 1));
                m.Store(ebx, v4);
                m.Assign(szo, m.Cond(v4));
                m.Return();
            });

            var ctx = new SymbolicEvaluationContext(new IntelArchitecture(ProcessorMode.ProtectedFlat), proc.Frame);
            var simplifier = new ExpressionSimplifier(ctx);
            var ep = new ExpressionPropagator(null, simplifier, ctx, new ProgramDataFlow());

            var newInstr = proc.EntryBlock.Succ[0].Statements[2].Instruction.Accept(ep);
            Assert.AreEqual("SZO = cond(v4)", newInstr.ToString());


        }
    }
}
