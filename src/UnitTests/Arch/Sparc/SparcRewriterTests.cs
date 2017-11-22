﻿#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Sparc
{
    [TestFixture]
    public class SparcRewriterTests : RewriterTestBase
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

        protected override IRewriterHost CreateRewriterHost()
        {
            return host;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return e;
        }

        [SetUp]
        public void Setup()
        {
            state = (SparcProcessorState)arch.CreateProcessorState();
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
            var image = new MemoryArea(LoadAddress, bytes);
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
                "0|T--|00100000(4): 1 instructions",
                "1|TD-|call 000FFFFC (0)");
        }

        [Test]
        public void SparcRw_addcc()
        {
            BuildTest(0x8A804004); // "addcc\t%g0,%g4,%g5"
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|g5 = g1 + g4",
                "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_or_imm()
        {
            BuildTest(0xBE10E004);//"or\t%g3,0x00000004,%i7");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|i7 = g3 | 0x00000004");
        }

        [Test]
        public void SparcRw_and_neg()
        {
            BuildTest(0x86087FFE); // "and\t%g1,0xFFFFFFFE,%g3")
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g3 = g1 & 0xFFFFFFFE");
        }

        [Test]
        public void SparcRw_sll_imm()
        {
            BuildTest(0xAB2EA01F);// sll\t%i2,0x0000001F,%l5"
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|l5 = i2 << 0x0000001F");
        }

        [Test]
        public void SparcRw_sethi()
        {
            BuildTest(0x0B2AAAAA);  // sethi\t0x002AAAAA,%g5");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = 0xAAAAA800");
        }

        [Test]
        [Ignore("")]
        public void SparcRw_taddcc()
        {
            BuildTest(0x8B006001);
            AssertCode("taddcc\t%g1,0x00000001,%g5");
        }

        [Test]
        [Ignore("")]
        public void SparcRw_mulscc()
        {
            host.Stub(h => h.PseudoProcedure(
                Arg<string>.Is.Equal("__mulscc"),
                Arg<DataType>.Is.Equal(VoidType.Instance),
                Arg<Expression[]>.Is.NotNull))
                .Return(new Application(
                     new ProcedureConstant(
                        PrimitiveType.Pointer32,
                        new PseudoProcedure("__mulscc", PrimitiveType.Int32, 2)),
                VoidType.Instance,
                Constant.Word32(0x19)));

            host.Stub(h => h.EnsurePseudoProcedure("__mulscc", PrimitiveType.Int32, 2))
                .Return(new PseudoProcedure("__mulscc", PrimitiveType.Int32, 2));

            BuildTest(0x8B204009);  // mulscc  %g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|g5 = __mulscc(g1, o1)",
                "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_umul()
        {
            BuildTest(0x8A504009); // umul %g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = g1 *u o1");
        }

        [Test]
        public void SparcRw_smul()
        {
            BuildTest(0x8A584009);  // smul %g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = g1 *s o1");
        }

        [Test]
        public void SparcRw_udivcc()
        {
            BuildTest(0x8AF04009);  // udivcc\t%g1,%o1,%g5
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|g5 = g1 /u o1",
               "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_sdiv()
        {
            BuildTest(0x8A784009); // sdiv\t%g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = g1 / o1");
        }

        [Test]
        public void SparcRw_save()
        {
            BuildTest(0x9DE3BEE8); // save\t%g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 10 instructions",
                "1|L--|v3 = sp + 0xFFFFFEE8",
                "2|L--|i0 = o0",
                "3|L--|i1 = o1",
                "4|L--|i2 = o2",
                "5|L--|i3 = o3",
                "6|L--|i4 = o4",
                "7|L--|i5 = o5",
                "8|L--|i6 = sp",
                "9|L--|i7 = o7",
                "10|L--|sp = v3");
        }

        [Test]
        public void SparcRw_restore()
        {
            BuildTest(0x81E80000); // restore\t%g0,%g0,%g0
            AssertCode(
                "0|L--|00100000(4): 8 instructions",
                "1|L--|o0 = i0",
                "2|L--|o1 = i1",
                "3|L--|o2 = i2",
                "4|L--|o3 = i3",
                "5|L--|o4 = i4",
                "6|L--|o5 = i5",
                "7|L--|sp = i6",
                "8|L--|o7 = i7");
        }

        [Test]
        public void SparcRw_be()
        {
            BuildTest(
                0x02800004,     // be      00100010
                0x8A04C004);    // add   %l3,%g4,%g5"
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(EQ,Z)) branch 00100010",
                "2|L--|00100004(4): 1 instructions",
                "3|L--|g5 = l3 + g4");
        }

        [Test]
        public void SparcRw_be_a()
        {
            BuildTest(
                0x22800004,     // be,a    00100004
                0x8A04C004);    // add     %l3,%g4,%g5"
            AssertCode(
                "0|TDA|00100000(4): 1 instructions",
                "1|TDA|if (Test(EQ,Z)) branch 00100010",
                "2|L--|00100004(4): 1 instructions",
                "3|L--|g5 = l3 + g4");
        }

        [Test]
        public void SparcRw_fbne()
        {
            BuildTest(0x03800001);  // fbne    00100004
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(NE,LG)) branch 00100004");
        }

        [Test]
        public void SparcRw_jmpl_goto()
        {
            BuildTest(0x8FC07FF0);  // jmpl    %g1,-16,%g7
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|g7 = 00100000",
                "2|TD-|goto g1 + -16");
        }

        [Test]
        public void SparcRw_jmpl_call()
        {
            BuildTest(0x9FC07FF0);  // jmpl    %g1,-16,%o7
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|o7 = 00100000",
                "2|TD-|call g1 + -16 (0)");
        }

        [Test]
        public void SparcRw_ta()
        {
            host.Stub(h => h.PseudoProcedure(
                Arg<string>.Is.Equal(PseudoProcedure.Syscall),
                Arg<DataType>.Is.Equal(VoidType.Instance),
                Arg<Expression[] >.Is.NotNull))
                .Return(new Application(
                    new ProcedureConstant(
                        PrimitiveType.Pointer32,
                        new PseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, 1)),
                    VoidType.Instance,
                    Constant.Word32(0x19)));
            BuildTest(0x91D02999);  // ta\t%g1,0x00000019"
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (false) branch 00100004",
                "2|L--|__syscall(0x00000019)");
        }

        [Test]
        public void SparcRw_fitos()
        {
            BuildTest(0x8BA0188A);  // fitos   %f10,%f5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f5 = (real32) f10");
        }

        [Test]
        public void SparcRw_ldsb()
        {
            BuildTest(0xC248A044); //ldsb\t[%g2+68],%g1");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g1 = (int32) Mem0[g2 + 68:int8]");
        }

        [Test]
        public void SparcRw_sth()
        {
            BuildTest(0xC230BFF0);// sth\t%g1,[%g2+68]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[g2 + -16:word16] = (word16) g1");
        }

        [Test]
        public void SparcRw_sth_idx()
        {
            BuildTest(0xC230800C);//sth\t%g1,[%g2+%i4]");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[g2 + o4:word16] = (word16) g1");
        }

        [Test]
        public void SparcRw_sth_idx_g0()
        {
            BuildTest(0xC2308000);//sth\t%g1,[%g2+%g0]");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[g2:word16] = (word16) g1");
        }

        [Test]
        public void SparcRw_or_imm_g0()
        {
            BuildTest(
                Instr(Opcode.or, Registers.g0, Constant.Word32(3), Registers.g1));
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g1 = 0x00000000 | 0x00000003");      // Simplification happens later in the decompiler.
        }

        [Test]
        public void SparcRw_subcc_g0()
        {
            BuildTest(0x80a22003);   // subcc %o0, 3, %g0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|g0 = o0 - 0x00000003",
                "2|L--|NZVC = cond(g0)");
        }

        [Test]
        public void SparcRw_andn()
        {
            BuildTest(0x922A0009);   // andn %o0,%o1,%o1
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o1 = o0 & ~o1");
        }

        [Test]
        public void SparcRw_sra()
        {
            BuildTest(0x913C3C03);// sra%l2,0x00000003,%o0;
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o0 = l0 >> 0x00000003");
        }

        [Test]
        public void SparcRw_subx()
        {
            BuildTest(0x986060FF);  //  subx %g0,0xFFFFFFFF,%o4
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o4 = g1 - 0x000000FF - C");
        }

        [Test]
        public void SparcRw_addx()
        {
            BuildTest(0x90402000);  // addx %g0,0x00000000,%o0
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o0 = 0x00000000 + 0x00000000 + C");
        }

        [Test]
        public void SparcRw_xor()
        {
            BuildTest(0x901A1A0A);  //  xor%o0,%o2,%o0
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o0 = o0 ^ o2");
        }

        [Test]
        public void SparcRw_bpos()
        {
            BuildTest(0x1CBFBFF1);  //  bpos 0001203C
            AssertCode(
                 "0|TD-|00100000(4): 1 instructions",
                 "1|TD-|if (Test(GE,N)) branch 000EFFC4");
        }

        [Test]
        public void SparcRw_ldd()
        {
            BuildTest(0xd01be000); // ldd\t[%o7+0],%o0
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|o0 = Mem0[o7:word64]");
        }

        [Test]
        public void SparcRw_srl()
        {
            BuildTest(0x8532E010); // srl %o3,0x00000010,%g2
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|g2 = o3 >>u 0x00000010");
        }
    }
}
