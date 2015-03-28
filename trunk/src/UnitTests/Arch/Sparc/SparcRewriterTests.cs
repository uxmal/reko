#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.Sparc;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Sparc
{
    [TestFixture]
    class SparcRewriterTests : RewriterTestBase 
    {
        private SparcArchitecture arch = new SparcArchitecture(PrimitiveType.Word32);
        private Address baseAddr = Address.Ptr32(0x00100000);
        private SparcProcessorState state;
        private IRewriterHost host;
        private IEnumerable<RtlInstructionCluster> e;
        private MockRepository repository;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return e;
        }

        [SetUp]
        public void Setup()
        {
            state = (SparcProcessorState) arch.CreateProcessorState();
            repository = new MockRepository();
            host = repository.StrictMock<IRewriterHost>();
        }

        private void BuildTest(params uint[] words)
        {
            repository.ReplayAll();
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) (w >> 24),
                (byte) (w >> 16),
                (byte) (w >> 8),
                (byte) w
            }).ToArray();
            var image = new LoadedImage(LoadAddress, bytes);
            e = new SparcRewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        private void BuildTest(params SparcInstruction[] instrs)
        {
            var addr = LoadAddress;
            var exts = instrs
                .Select(i =>
                {
                    i.Address = addr;
                    i.Length = 4;
                    addr += 4;
                    return i;
                });
            e = new SparcRewriter(arch, exts.GetEnumerator(), state, new Frame(arch.WordWidth), host);
        }

        private SparcInstruction Instr(Opcode opcode, params object[] ops)
        {
            var instr = new SparcInstruction { Opcode = opcode };
            if (ops.Length > 0)
            {
                instr.Op1 = Op(ops[0]);
                if (ops.Length > 1)
                {
                    instr.Op2 = Op(ops[1]);
                    if (ops.Length > 2)
                    {
                        instr.Op3 = Op(ops[2]);
                    }
                }
            }
            return instr;
        }

        private MachineOperand Op(object o)
        {
            var reg = o as RegisterStorage;
            if (reg != null)
                return new RegisterOperand(reg);
            var c = o as Constant;
            if (c != null)
                return new ImmediateOperand(c);
            throw new NotImplementedException(string.Format("Unsupported: {0} ({1})", o, o.GetType().Name));
        }

        [Test]
        public void SparcRw_call()
        {
            BuildTest(0x7FFFFFFF);  // "call\t000FFFFC"
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|TD-|call 000FFFFC (0)");
        }

        [Test]
        public void SparcRw_addcc()
        {
            BuildTest(0x8A804004); // "addcc\t%g0,%g4,%g5"
            AssertCode(
                "0|00100000(4): 2 instructions",
                "1|L--|g5 = g1 + g4",
                "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_or_imm()
        {
            BuildTest(0xBE10E004);//"or\t%g3,0x00000004,%i7");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|i7 = g3 | 0x00000004");
        }

        [Test]
        public void SparcRw_and_neg()
        {
            BuildTest(0x86087FFE); // "and\t%g1,0xFFFFFFFE,%g3")
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g3 = g1 & 0xFFFFFFFE");
        }

        [Test]
        public void SparcRw_sll_imm()
        {
            BuildTest(0xAB2EA01F);// sll\t%i2,0x0000001F,%l5"
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|l5 = i2 << 0x0000001F");
        }

        [Test]
        public void SparcRw_sethi()
        {
            BuildTest(0x0B2AAAAA);  // sethi\t0x002AAAAA,%g5");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g5 = 0xAAAAA800");
        }

        [Test]
        [Ignore]
        public void SparcRw_taddcc()
        {
            BuildTest(0x8B006001);
            AssertCode("taddcc\t%g1,0x00000001,%g5");
        }

        [Test]
        public void SparcRw_mulscc()
        {
            host.Stub(h => h.EnsurePseudoProcedure("__mulscc", PrimitiveType.Int32, 2))
                .Return(new PseudoProcedure("__mulscc", PrimitiveType.Int32, 2));

            BuildTest(0x8B204009);  // mulscc  %g1,%o1,%g5
            AssertCode(
                "0|00100000(4): 2 instructions",
                "1|L--|g5 = __mulscc(g1, o1)",
                "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_umul()
        {
            BuildTest(0x8A504009); // umul %g1,%o1,%g5
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g5 = g1 *u o1");
        }

        [Test]
        public void SparcRw_smul()
        {
            BuildTest(0x8A584009);  // smul %g1,%o1,%g5
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g5 = g1 *s o1");
        }

        [Test]
        public void SparcRw_udivcc()
        {
            BuildTest(0x8AF04009);  // udivcc\t%g1,%o1,%g5
            AssertCode(
               "0|00100000(4): 2 instructions",
               "1|L--|g5 = g1 /u o1",
               "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_sdiv()
        {
            BuildTest(0x8A784009); // sdiv\t%g1,%o1,%g5
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g5 = g1 / o1");
        }

        [Test]
        [Ignore]
        public void SparcRw_save()
        {
            BuildTest(0x8BE04009);
            AssertCode("save\t%g1,%o1,%g5");
        }

        [Test]
        public void SparcRw_be()
        {
            BuildTest(
                0x02800004,     // be      00100010
                0x8A04C004);    // add   %l3,%g4,%g5"
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|TD-|if (Test(EQ,Z)) branch 00100010",
                "2|00100004(4): 1 instructions",
                "3|L--|g5 = l3 + g4");
        }

        [Test]
        public void SparcRw_be_a()
        {
            BuildTest(
                0x22800004,     // be,a    00100004
                0x8A04C004);    // add     %l3,%g4,%g5"
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|TDA|if (Test(EQ,Z)) branch 00100010",
                "2|00100004(4): 1 instructions",
                "3|L--|g5 = l3 + g4");
        }

        [Test]
        public void SparcRw_fbne()
        {
            BuildTest(0x03800001);  // fbne    00100004
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|TD-|if (Test(NE,LG)) branch 00100004");
        }

        [Test]
        public void SparcRw_jmpl_goto()
        {
            BuildTest(0x8FC07FF0);  // jmpl    %g1,-16,%g7
            AssertCode(
                "0|00100000(4): 2 instructions",
                "1|L--|g7 = 00100000",
                "2|TD-|goto g1 + -16");
        }

        [Test]
        public void SparcRw_jmpl_call()
        {
            BuildTest(0x9FC07FF0);  // jmpl    %g1,-16,%o7
            AssertCode(
                "0|00100000(4): 2 instructions",
                "1|L--|o7 = 00100000",
                "2|TD-|call g1 + -16 (0)");
        }

        [Test]
        public void SparcRw_ta()
        {
            host.Stub(h => h.EnsurePseudoProcedure("__syscall", VoidType.Instance, 1)).Return(new PseudoProcedure("__syscall", VoidType.Instance, 1));
            BuildTest(0x91D02999);  // ta\t%g1,0x00000019"
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|if (true) __syscall(0x00000019)");
        }

        [Test]
        public void SparcRw_fitos()
        {
            BuildTest(0x8BA0188A);  // fitos   %f10,%f5
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|f5 = (real32) f10");
        }

        [Test]
        public void SparcRw_ldsb()
        {
            BuildTest(0xC248A044); //ldsb\t[%g2+68],%g1");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g1 = (int32) Mem0[g2 + 68:int8]"); 
        }

        [Test]
        public void SparcRw_sth()
        {
            BuildTest(0xC230BFF0);// sth\t%g1,[%g2+68]
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[g2 + -16:word16] = (word16) g1"); 
        }

        [Test]
        public void SparcRw_sth_idx()
        {
            BuildTest(0xC230800C);//sth\t%g1,[%g2+%i4]");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[g2 + o4:word16] = (word16) g1");
        }

        [Test]
        public void SparcRw_sth_idx_g0()
        {
            BuildTest(0xC2308000);//sth\t%g1,[%g2+%g0]");
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|Mem0[g2:word16] = (word16) g1");
        }

        [Test]
        public void SparcRw_or_imm_g0()
        {
            BuildTest(
                Instr(Opcode.or, Registers.g0, Constant.Word32(3), Registers.g1));
            AssertCode(
                "0|00100000(4): 1 instructions",
                "1|L--|g1 = 0x00000000 | 0x00000003");      // Simplification happens later in the decompiler.
        }
    }
}
