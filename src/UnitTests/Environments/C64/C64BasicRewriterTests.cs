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
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Environments.C64;
using Reko.UnitTests.Arch;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using Reko.Arch.Mos6502;

namespace Reko.UnitTests.Environments.C64
{
    [TestFixture]
    public class C64BasicRewriterTests : RewriterTestBase
    {
        private BasicProcessor m;
        private C64Basic arch;
        private Mos6502Architecture arch6502;
        private SortedList<ushort, C64BasicInstruction> lines;
        private Mock<ArchTestBase.RewriterHost> host;

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr16(0x800);

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var addr = Address.Ptr16(10);
            return arch.CreateRewriter(
                arch.CreateImageReader(mem, addr),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        protected override IRewriterHost CreateRewriterHost()
        {
            return host.Object;
        }

        private class BasicProcessor
        {
            private SortedList<ushort, C64BasicInstruction> lines;

            public BasicProcessor(SortedList<ushort, C64BasicInstruction> lines)
            {
                this.lines = lines;
            }

            public void Add(ushort lineNumber, params object[] instrs)
            {
                var tokens = Tokenize(instrs);
                var line = new C64BasicInstruction
                {
                    Address = Address.Ptr16(lineNumber),
                    Line = tokens,
                };
                lines.Add(lineNumber, line);
            }

            private byte[] Tokenize(object[] instrs)
            {
                MemoryStream stm = new MemoryStream();
                foreach (var instr in instrs)
                {
                    if (instr is Token)
                        stm.WriteByte((byte)(Token)instr);
                    else if (instr is string)
                    {
                        var bytes = Encoding.ASCII.GetBytes((string)instr);
                        stm.Write(bytes, 0, bytes.Length);
                    }
                    else
                        throw new NotImplementedException();
                }
                return stm.ToArray();
            }

            public object Clr() { return Token.CLR; }
            public object End() { return Token.END; }

            internal object Sys() { return Token.SYS; }
        }

        [SetUp]
        public void Setup()
        {
            lines = new SortedList<ushort, C64BasicInstruction>();
            arch = new C64Basic(lines);
            arch6502 = new Mos6502Architecture("m6502");
            m = new BasicProcessor(lines);
            host = new Mock<RewriterTestBase.RewriterHost>(arch) { CallBase = true };
            host.Setup(h => h.GetArchitecture("m6502"))
                .Returns(arch6502);
            base.Given_MemoryArea(new MemoryArea(Address.Ptr16(0x10), new byte[10]));
        }

        [Test]
        public void C64brw_End()
        {
            m.Add(10, m.End());
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__End()");
        }

        [Test]
        public void C64brw_Sys()
        {
            m.Add(10, m.Sys(), " 2064");
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|callx m6502 0810 (2)");
        }

        [Test]
        public void C64brw_Clr()
        {
            m.Add(10, m.Clr());
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__Clr()");
        }

        [Test]
        public void C64brw_LetInt()
        {
            m.Add(10, "AB", Token.eq, "8");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|AB_r = 8");
        }

        [Test]
        public void C64brw_Print_Chr_s()
        {
            m.Add(10, Token.PRINT, " ", Token.CHR_s, "(147)");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__PrintLine(__Chr(147))");
        }

        [Test]
        public void C64brw_Print_Print()
        {
            m.Add(10, Token.PRINT, " 3:", Token.PRINT);
            AssertCode(
                "0|L--|000A(1): 2 instructions",
                "1|L--|__PrintLine(3)",
                "2|L--|__PrintEmptyLine()");
        }

        [Test]
        public void C64brw_Poke()
        {
            m.Add(10, Token.POKE, " ", "51231,123");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__Poke(-14305, 123)");
        }

        [Test]
        public void C64brw_Get()
        {
            m.Add(10, Token.GET, "A$");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__Get(out A_s)");
        }

        [Test]
        public void C64brw_IfThen_Line()
        {
            m.Add(10, Token.IF, "A", Token.THEN,"10");
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|if (A_r) branch 000A");
        }

        [Test]
        public void C64brw_IfThen_Command()
        {
            m.Add(10, Token.IF, "A", Token.THEN, "A ", Token.eq, " 3");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|if (A_r) A_r = 3");
        }

        [Test]
        public void C64brw_IfGoto()
        {
            m.Add(10, Token.IF, "A", Token.GOTO, "10");
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|if (A_r) branch 000A");
        }

        [Test]
        public void C64brw_Regression_1()
        {
            m.Add(10, Token.GET, "AN$:", Token.IF, "AN$", Token.eq, "\"\"", Token.GOTO, "370");
            AssertCode(
                "0|T--|000A(1): 2 instructions",
                "1|L--|__Get(out AN_s)",
                "2|T--|if (AN_s == \"\") branch 0172");
        }

        [Test]
        public void C64brw_Goto()
        {
            m.Add(10, Token.GOTO, "32");
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|goto 0020");
        }

        [Test]
        public void C64brw_Gosub()
        {
            m.Add(10, Token.GOSUB, "32");
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|call 0020 (2)");
        }

        [Test]
        public void C64brw_Return()
        {
            m.Add(10, Token.RETURN);
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void C64brw_Print_Tab()
        {
            m.Add(10, Token.PRINT,Token.TAB_lp,"5);\"HELLO\"");
            AssertCode(
                "0|L--|000A(1): 2 instructions",
                "1|L--|__PrintTab(5)",
                "2|L--|__PrintLine(\"HELLO\")");
        }

        [Test]
        public void C64brw_Input()
        {
            m.Add(10, Token.INPUT, "\"FOO?\";A$");
            AssertCode(
                "0|L--|000A(1): 2 instructions",
                "1|L--|__Print(\"FOO?\")",
                "2|L--|__Input(out A_s)");
        }

        [Test]
        public void C64brw_Open()
        {
            m.Add(10, Token.OPEN,"8,1,1,\"FOO,SEQ,R\"");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__Open(8, 1, 1, \"FOO,SEQ,R\")");
        }

        [Test]
        public void C64brw_InputHash()
        {
            m.Add(10, Token.INPUT_hash, "8,A$,B%");
            AssertCode(
                "0|L--|000A(1): 2 instructions",
                "1|L--|__InputStm(8, out A_s)",
                "2|L--|__InputStm(8, out B_i)");
        }

        [Test]
        public void C64brw_array()
        {
            m.Add(10, "B", Token.eq, "A(1)");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|B_r = A_r[1]");
        }


        [Test]
        public void C64brw_Close()
        {
            m.Add(10, Token.CLOSE, "3");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|__Close(3)");
        }

        [Test]
        public void C64brw_Plus()
        {
            m.Add(10, "A$", Token.eq, "A$", Token.add, "\"+\"");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|A_s = A_s + \"+\"");
        }

        [Test]
        public void C64brw_Regression_2()
        {
            m.Add(10, Token.PRINT,":",Token.PRINT,"\"FOO: \";CM$(MA):",Token.PRINT);
            AssertCode(
                "0|L--|000A(1): 4 instructions",
                "1|L--|__PrintEmptyLine()",
                "2|L--|__Print(\"FOO: \")",
                "3|L--|__PrintLine(CM_s[MA_r])",
                "4|L--|__PrintEmptyLine()");
        }

        [Test]
        public void C64brw_Print_hash()
        {
            m.Add(10, Token.PRINT_hash, "2,C$,A$");
            AssertCode(
                "0|L--|000A(1): 2 instructions",
                "1|L--|__PrintStm(2, C_s)",
                "2|L--|__PrintStm(2, A_s)");
        }

        [Test]
        public void C64brw_Let_Array()
        {
            m.Add(10, "CA$(3)",Token.eq,"M$(I)");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|CA_s[3] = M_s[I_r]");
        }

        [Test]
        public void C64brw_gt()
        {
            m.Add(10, Token.IF, "S",Token.gt,"3",Token.GOTO, "123");
            AssertCode(
                "0|T--|000A(1): 1 instructions",
                "1|T--|if (S_r > 3) branch 007B");
        }

        [Test]
        public void C64brw_mul()
        {
            m.Add(10, "S", Token.eq, "S", Token.mul,"3");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|S_r = S_r * 3");
        }

        [Test]
        public void C64brw_neg()
        {
            m.Add(10, "S", Token.eq, Token.sub, "3");
            AssertCode(
                "0|L--|000A(1): 1 instructions",
                "1|L--|S_r = -3");
        }
    }
}
