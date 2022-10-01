#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class ArgumentGuesserTests
    {
        private readonly ExternalProcedure externalProc;
        private FakeArchitecture arch;
        private Mock<IPlatform> platform;
        private Identifier fp;
        private RegisterStorage reg1;
        private RegisterStorage reg2;
        private RegisterStorage reg3;

        public ArgumentGuesserTests()
        {
            this.externalProc = new ExternalProcedure("external_proc", new FunctionType());
        }

        [SetUp]
        public void Setup()
        {
            this.arch = new FakeArchitecture(new ServiceContainer());
            this.reg1 = arch.GetRegister(1);
            this.reg2 = arch.GetRegister(2);
            this.reg3 = arch.GetRegister(3);
            this.platform = new Mock<IPlatform>();
            platform.Setup(p => p.Architecture).Returns(arch);
            platform.Setup(p => p.IsPossibleArgumentRegister(reg1)).Returns(true);
            platform.Setup(p => p.IsPossibleArgumentRegister(reg2)).Returns(true);
            platform.Setup(p => p.PossibleReturnValue(new[] { reg1 })).Returns(reg1);
        }

        private void Given_FramePointer(SsaProcedureBuilder m)
        {
            this.fp = m.Ssa.Identifiers.Add(m.Frame.FramePointer, null, null, false).Identifier;
        }

        private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder(arch);
            builder(m);
            var argGuesser = new ArgumentGuesser(platform.Object, m.Ssa, new FakeDecompilerEventListener());
            argGuesser.Transform();
            ProcedureCodeVerifier.AssertCode(m.Ssa.Procedure, sExpected);
            m.Ssa.Validate(s => { m.Ssa.Dump(true); Assert.Fail(s); });
        }

        [Test]
        public void Argg_SingleStackArgument()
        {
            var sExpected =
            #region Expected
@"
esp_1 = esp + 4<32>
v2_5 = 0x68<32>
external_proc(v2_5)
return
";
            #endregion

            RunTest(sExpected, m =>
            {
                var esp = m.Reg("esp", m.Architecture.StackRegister);
                var esp_1 = m.Reg("esp_1", m.Architecture.StackRegister);
                var esp_2 = m.Reg("esp_2", m.Architecture.StackRegister);

                m.Assign(esp_1, m.IAdd(esp, 4));
                m.MStore(m.IAdd(esp, 4), m.Word32(0x68));
                m.Call(externalProc, 4,
                    new[] { (esp.Storage, (Expression) m.IAdd(esp, 4)) },
                    new[] { (esp.Storage, esp_2) });
                m.Return();
            });
        }

        [Test]
        public void Argg_TwoStackArgument_UsingFramePointer()
        {
            var sExpected =
            #region Expected
@"
esp = fp
esp_1 = fp - 4<32>
v3_9 = 0x68<32>
esp_2 = fp - 8<32>
v2_8 = 0x65<32>
external_proc(v2_8, v3_9)
return
";
            #endregion

            RunTest(sExpected, m =>
            {
                Given_FramePointer(m);
                var esp = m.Reg("esp", m.Architecture.StackRegister);
                var esp_1 = m.Reg("esp_1", m.Architecture.StackRegister);
                var esp_2 = m.Reg("esp_2", m.Architecture.StackRegister);
                var esp_3 = m.Reg("esp_3", m.Architecture.StackRegister);

                m.Assign(esp, fp);
                m.Assign(esp_1, m.ISub(fp, 4));
                m.MStore(m.ISub(fp, 4), m.Word32(0x68));
                m.Assign(esp_2, m.ISub(fp, 8));
                m.MStore(m.ISub(fp, 8), m.Word32(0x65));
                m.Call(externalProc, 4,
                    new[] { (esp.Storage, (Expression) m.ISub(fp, 8)) },
                    new[] { (esp.Storage, esp_3) });
                m.Return();
            });
        }

        [Test]
        public void Argg_TwoStackArgument_UsingRegisters()
        {
            var sExpected =
            #region Expected
@"
esp = fp
r2_2 = 0x68<32>
r1_1 = 0x65<32>
r3_3 = 0x65<32>
external_proc(r1_1, r2_2)
return
";
            #endregion

            RunTest(sExpected, m =>
            {
                Given_FramePointer(m);
                var esp = m.Reg("esp", m.Architecture.StackRegister);
                var r1_1 = m.Reg("r1_1", reg1);
                var r2_2 = m.Reg("r2_2", reg2);
                var r3_3 = m.Reg("r3_3", reg3);
                var esp_4 = m.Reg("esp_4", m.Architecture.StackRegister);

                m.Assign(esp, fp);
                m.Assign(r2_2, m.Word32(0x68));
                m.Assign(r1_1, m.Word32(0x65));
                m.Assign(r3_3, m.Word32(0x65)); // not an arg.
                m.Call(externalProc, 4,
                    new[] { (esp.Storage, (Expression) m.ISub(esp, 8)) },
                    new[] { (esp.Storage, esp_4) });
                m.Return();
            });
        }

        [Test]
        public void Argg_ReturnValue_used()
        {
            var sExpected =
            #region Expected
@"
r1_1 = external_proc()
Mem2[0x123400<32>:word32] = r1_1
return
";
            #endregion

            RunTest(sExpected, m =>
            {
                var r1_1 = m.Reg("r1_1", reg1);
                var esp = m.Reg("esp", m.Architecture.StackRegister);
                m.Call(externalProc, 4,
                    new[] { (esp.Storage, (Expression) m.ISub(esp, 8)) },
                    new[] { (r1_1.Storage, r1_1) });
                m.MStore(m.Word32(0x00123400), r1_1);
                m.Return();
            });
        }

        [Test]
        public void Argg_ReturnValue_not_used()
        {
            var sExpected =
            #region Expected
@"
external_proc()
return
";
            #endregion

            RunTest(sExpected, m =>
            {
                var r1_1 = m.Reg("r1_1", reg1);
                var esp = m.Reg("esp", m.Architecture.StackRegister);
                m.Call(externalProc, 4,
                    new[] { (esp.Storage, (Expression) m.ISub(esp, 8)) },
                    new[] { (r1_1.Storage, r1_1) });
                m.Return();
            });
        }

        [Test]
        public void Argg_AvoidNonParameterCapture()
        {
            var sExpected =
            #region Expected
@"
def r3
loc_1 = r3
external_proc()
r3_3 = r3
return
";
            #endregion

            RunTest(sExpected, m =>
            {
                var r3 = m.Reg("r3", reg3);
                var loc_1 = m.Local32("loc_1", -4);
                var r1_2 = m.Reg("r3_2", reg1);
                var r3_3 = m.Reg("r3_3", reg3);
                var esp = m.Reg("esp", m.Architecture.StackRegister);

                m.AddDefToEntryBlock(r3);
                m.Assign(loc_1, r3);
                m.Call(externalProc, 4,
                    new[] { (esp.Storage, (Expression) m.ISub(esp, 8)) },
                    new[] { (r1_2.Storage, r1_2) });
                m.Assign(r3_3, r3);
                m.Return();
            });
        }
    }
}