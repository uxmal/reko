#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class IntraBlockDeadRegistersTests
    {
        private string testResult;

        private static string ToExpectedString(params string [] lines)
        {
            var stringlist = new List<string>();
            foreach (var i in lines)
                stringlist.Add("\t" + i);
            stringlist.Add ("");
            return String.Join (Environment.NewLine, stringlist.ToArray ());
        }

        private void RunTest(Action<ProcedureBuilder> m)
        {
            var builder = new ProcedureBuilder();
            m(builder);
            var ibdr = new IntraBlockDeadRegisters();
            var block = builder.Procedure.EntryBlock.Succ[0];
            ibdr.Apply(block);
            var sw = new StringWriter();
            block.WriteStatements(sw);
            sw.Flush();
            testResult = sw.ToString();
        }

        [Test(Description = "Assignment kills its dst.")]
        public void Ibdr_DeadAssign()
        {
            RunTest(m =>
            {
                var a = m.Frame.EnsureRegister(RegisterStorage.Reg32("a", 0));
                m.Assign(a, 2);
                m.Assign(a, 3);
            });
            string expected = ToExpectedString(
                "a = 3<32>"
            );
            Assert.AreEqual(expected, testResult);
        }

        [Test(Description = "Calls clear killed stuff.")]
        public void Ibdr_Call()
        {
            RunTest(m =>
            {
                var a = m.Frame.EnsureRegister(RegisterStorage.Reg32("a", 0));
                m.Assign(a, 2);
                m.Call("foo", 4);
                m.Assign(a, 3);
            });
            string expected = ToExpectedString(
                "a = 2<32>",
                "call <invalid> (retsize: 4;)",
                "a = 3<32>"
            );

            Assert.AreEqual(expected, testResult);
        }

        [Test(Description = "Flags used clear killed bits.")]
        public void Ibdr_FlagReg()
        {
            RunTest(m =>
            {
                var flags = RegisterStorage.Reg32("flags", 0x0A);
                var CN = m.Frame.EnsureFlagGroup(flags, 0x3, "CN", PrimitiveType.Byte);
                var C = m.Frame.EnsureFlagGroup(flags, 0x1, "C", PrimitiveType.Bool);
                var N = m.Frame.EnsureFlagGroup(flags, 0x2, "N", PrimitiveType.Bool);
                var Z = m.Frame.EnsureFlagGroup(flags, 0x4, "Z", PrimitiveType.Bool);
                
                var a = m.Frame.EnsureRegister(RegisterStorage.Reg32("a", 0));
                m.Assign(N, m.Cond(a));
                m.Assign(C, m.Cond(a));
                m.BranchIf(m.Test(ConditionCode.LE, CN), "foo");
            });
            string expected = ToExpectedString(
                "N = cond(a)",
                "C = cond(a)",
                "branch Test(LE,CN) foo"
            );
            Assert.AreEqual(expected, testResult);
        }

        [Test]
        public void Ibdr_RedundantFlagReg()
        {
            RunTest(m =>
            {
                var flags = RegisterStorage.Reg32("flags", 0xA);
                var CN = m.Frame.EnsureFlagGroup(flags, 0x3, "CN", PrimitiveType.Byte);
                var C = m.Frame.EnsureFlagGroup(flags, 0x1, "C", PrimitiveType.Bool);
                var N = m.Frame.EnsureFlagGroup(flags, 0x2, "N", PrimitiveType.Bool);
                var Z = m.Frame.EnsureFlagGroup(flags, 0x4, "Z", PrimitiveType.Bool);
                var a = m.Frame.EnsureRegister(RegisterStorage.Reg32("a", 0));
                m.Assign(a, m.IAdd(a, 3));
                m.Assign(N, m.Cond(a));
                m.Assign(a, m.IAdd(a, a));
                m.Assign(N, m.Cond(a));
                m.BranchIf(m.Test(ConditionCode.LE, CN), "foo");
            });
            string expected = ToExpectedString(
                "a = a + 3<32>",
                "a = a + a",
                "N = cond(a)",
                "branch Test(LE,CN) foo"
            );

            Assert.AreEqual(expected, testResult);
        }

        [Test(Description = "if a statement uses bl, then bx is live")]
        [Category(Categories.UnitTests)]
        public void Ibdr_PartialUse()
        {
            RunTest(m =>
            {
                var al = m.Frame.EnsureRegister(RegisterStorage.Reg8("al", 0));
                var bx = m.Frame.EnsureRegister(RegisterStorage.Reg16("bx", 3));
                var bl = m.Frame.EnsureRegister(RegisterStorage.Reg8("bl", 3));
                m.Assign(bx, m.Convert(al, al.DataType, PrimitiveType.Word16));
                m.SideEffect(m.Fn("foo", bl));
            });
            string sExp = ToExpectedString(
                "bx = CONVERT(al, byte, word16)",
                "foo(bl)");
            Assert.AreEqual(sExp, testResult);
        }
    }
}
