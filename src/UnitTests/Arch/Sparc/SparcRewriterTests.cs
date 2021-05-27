#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.Sparc;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Sparc
{
    [TestFixture]
    public class SparcRewriterTests : RewriterTestBase
    {
        private SparcArchitecture arch = new SparcArchitecture32(CreateServiceContainer(), "sparc", new Dictionary<string, object>());
        private Address baseAddr = Address.Ptr32(0x00100000);
        private SparcProcessorState state;
        private Mock<IRewriterHost> host;
        private SparcRewriter e;

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddr;

        protected override IRewriterHost CreateRewriterHost()
        {
            return host.Object;
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            if (e != null)
                return e;
            else
                return base.GetRtlStream(mem, binder, host);
        }

        [SetUp]
        public void Setup()
        {
            state = (SparcProcessorState)arch.CreateProcessorState();
            host = new Mock<IRewriterHost>();
            e = null;
        }

        private void Rewrite_UInt32s(params SparcInstruction[] instrs)
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
            e = new SparcRewriter(arch, exts.GetEnumerator(), state, new Frame(arch.WordWidth), host.Object);
        }

        private SparcInstruction Instr(Mnemonic mnemonic, params object[] ops)
        {
            var instr = new SparcInstruction
            {
                Mnemonic = mnemonic,
                InstructionClass = InstrClass.Linear,
                Operands = ops.Select(Op).ToArray()
            };
            return instr;
        }

        private MachineOperand Op(object o)
        {
            switch (o)
            {
            case RegisterStorage reg: return new RegisterOperand(reg);
            case Constant c: return new ImmediateOperand(c);
            default: throw new NotImplementedException(string.Format("Unsupported: {0} ({1})", o, o.GetType().Name));
            }
        }

        [Test]
        public void SparcRw_call()
        {
            Given_UInt32s(0x7FFFFFFF);  // "call\t000FFFFC"
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 000FFFFC (0)");
        }

        [Test]
        public void SparcRw_addcc()
        {
            Given_UInt32s(0x8A804004); // "addcc\t%g0,%g4,%g5"
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|g5 = g1 + g4",
                "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_or_imm()
        {
            Given_UInt32s(0xBE10E004);//"or\t%g3,0x00000004<32>,%i7");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|i7 = g3 | 4<32>");
        }

        [Test]
        public void SparcRw_and_neg()
        {
            Given_UInt32s(0x86087FFE); // "and\t%g1,0xFFFFFFFE<32>,%g3")
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g3 = g1 & 0xFFFFFFFE<32>");
        }

        [Test]
        public void SparcRw_sll_imm()
        {
            Given_UInt32s(0xAB2EA01F);// sll\t%i2,0x0000001F<32>,%l5"
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|l5 = i2 << 0x1F<32>");
        }

        [Test]
        public void SparcRw_sethi()
        {
            Given_UInt32s(0x0B2AAAAA);  // sethi\t0x002AAAAA,%g5");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = 0xAAAAA800<32>");
        }

        [Test]
        [Ignore("")]
        public void SparcRw_taddcc()
        {
            Given_UInt32s(0x8B006001);
            AssertCode("taddcc\t%g1,0x00000001<32>,%g5");
        }

        [Test]
        [Ignore("")]
        public void SparcRw_mulscc()
        {
            host.Setup(h => h.Intrinsic(
                "__mulscc",
                true,
                VoidType.Instance,
                It.IsNotNull<Expression[]>()))
                .Returns(new Application(
                     new ProcedureConstant(
                        PrimitiveType.Ptr32,
                        new IntrinsicProcedure("__mulscc", true, PrimitiveType.Int32, 2)),
                VoidType.Instance,
                Constant.Word32(0x19)));

            Given_UInt32s(0x8B204009);  // mulscc  %g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|g5 = __mulscc(g1, o1)",
                "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_umul()
        {
            Given_UInt32s(0x8A504009); // umul %g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = g1 *u o1");
        }

        [Test]
        public void SparcRw_smul()
        {
            Given_UInt32s(0x8A584009);  // smul %g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = g1 *s o1");
        }

        [Test]
        public void SparcRw_udivcc()
        {
            Given_UInt32s(0x8AF04009);  // udivcc\t%g1,%o1,%g5
            AssertCode(
               "0|L--|00100000(4): 2 instructions",
               "1|L--|g5 = g1 /u o1",
               "2|L--|NZVC = cond(g5)");
        }

        [Test]
        public void SparcRw_sdiv()
        {
            Given_UInt32s(0x8A784009); // sdiv\t%g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g5 = g1 / o1");
        }

        [Test]
        public void SparcRw_save()
        {
            Given_UInt32s(0x9DE3BEE8); // save\t%g1,%o1,%g5
            AssertCode(
                "0|L--|00100000(4): 10 instructions",
                "1|L--|v3 = sp + 0xFFFFFEE8<32>",
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
        public void SparcRw_save_g0()
        {
            Given_HexString("81E10F91");
            AssertCode(
                "0|L--|00100000(4): 8 instructions",
                "1|L--|i0 = o0",
                "2|L--|i1 = o1",
                "3|L--|i2 = o2",
                "4|L--|i3 = o3",
                "5|L--|i4 = o4",
                "6|L--|i5 = o5",
                "7|L--|i6 = sp",
                "8|L--|i7 = o7");
        }

        [Test]
        public void SparcRw_restore()
        {
            Given_UInt32s(0x81E80000); // restore\t%g0,%g0,%g0
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
            Given_UInt32s(
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
            Given_UInt32s(
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
            Given_UInt32s(0x03800001);  // fbne    00100004
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(NE,LG)) branch 00100004");
        }

        [Test]
        public void SparcRw_fsubs()
        {
            Given_HexString("B1B0445F");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f24 = f1 - f31");
        }

        [Test]
        public void SparcRw_jmpl_goto()
        {
            Given_UInt32s(0x8FC07FF0);  // jmpl    %g1,-16,%g7
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|g7 = 00100000",
                "2|TD-|goto g1 + -16<i32>");
        }

        [Test]
        public void SparcRw_jmpl_call()
        {
            Given_UInt32s(0x9FC07FF0);  // jmpl    %g1,-16,%o7
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|o7 = 00100000",
                "2|TD-|call g1 + -16<i32> (0)");
        }

        [Test]
        public void SparcRw_ta()
        {
            host.Setup(h => h.Intrinsic(
                IntrinsicProcedure.Syscall,
                false,
                VoidType.Instance,
                It.IsNotNull<Expression[] >()))
                .Returns(new Application(
                    new ProcedureConstant(
                        PrimitiveType.Ptr32,
                        new IntrinsicProcedure(IntrinsicProcedure.Syscall, false, VoidType.Instance, 1)),
                    VoidType.Instance,
                    Constant.Word32(0x19)));
            Given_UInt32s(0x91D02999);  // ta\t%g1,0x00000019<32>"
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (false) branch 00100004",
                "2|L--|__syscall(0x19<32>)");
        }

        [Test]
        public void SparcRw_fitoq()
        {
            Given_HexString("89BC0CCC");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q4 = CONVERT(f12, int32, real128)");
        }

        [Test]
        public void SparcRw_fitos()
        {
            Given_UInt32s(0x8BA0188A);  // fitos   %f10,%f5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f5 = CONVERT(f10, int32, real32)");
        }

        [Test]
        public void SparcRw_ldsb()
        {
            Given_UInt32s(0xC248A044); //ldsb\t[%g2+68],%g1");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g1 = CONVERT(Mem0[g2 + 68<i32>:int8], int8, int32)");
        }

        [Test]
        public void SparcRw_sth()
        {
            Given_UInt32s(0xC230BFF0);// sth\t%g1,[%g2+68]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[g2 + -16<i32>:word16] = SLICE(g1, word16, 0)");
        }

        [Test]
        public void SparcRw_sth_idx()
        {
            Given_UInt32s(0xC230800C);//sth\t%g1,[%g2+%i4]");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[g2 + o4:word16] = SLICE(g1, word16, 0)");
        }

        [Test]
        public void SparcRw_sth_idx_g0()
        {
            Given_UInt32s(0xC2308000);//sth\t%g1,[%g2+%g0]");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[g2:word16] = SLICE(g1, word16, 0)");
        }

        [Test]
        public void SparcRw_or_imm_g0()
        {
            Rewrite_UInt32s(
                Instr(Mnemonic.or, arch.Registers.g0, Constant.Word32(3), arch.Registers.IntegerRegisters[1]));
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|g1 = 0<32> | 3<32>");      // Simplification happens later in the decompiler.
        }

        [Test]
        public void SparcRw_subcc_g0()
        {
            Given_UInt32s(0x80a22003);   // subcc %o0, 3, %g0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|g0 = o0 - 3<32>",
                "2|L--|NZVC = cond(g0)");
        }

        [Test]
        public void SparcRw_andn()
        {
            Given_UInt32s(0x922A0009);   // andn %o0,%o1,%o1
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o1 = o0 & ~o1");
        }

        [Test]
        public void SparcRw_sra()
        {
            Given_UInt32s(0x913C3C03);// sra%l2,0x00000003<32>,%o0;
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o0 = l0 >> 3<32>");
        }

        [Test]
        public void SparcRw_subx()
        {
            Given_UInt32s(0x986060FF);  //  subx %g0,0xFFFFFFFF<32>,%o4
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o4 = g1 - 0xFF<32> - C");
        }

        [Test]
        public void SparcRw_addx()
        {
            Given_UInt32s(0x90402000);  // addx %g0,0<32>,%o0
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o0 = 0<32> + 0<32> + C");
        }

        [Test]
        public void SparcRw_xor()
        {
            Given_UInt32s(0x901A1A0A);  //  xor%o0,%o2,%o0
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|o0 = o0 ^ o2");
        }

        [Test]
        public void SparcRw_bpos()
        {
            Given_UInt32s(0x1CBFBFF1);  //  bpos 0001203C
            AssertCode(
                 "0|TD-|00100000(4): 1 instructions",
                 "1|TD-|if (Test(GE,N)) branch 000EFFC4");
        }

        [Test]
        public void SparcRw_ldd()
        {
            Given_UInt32s(0xd01be000); // ldd\t[%o7+0],%o0
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|o0 = Mem0[o7:word64]");
        }

        [Test]
        public void SparcRw_srl()
        {
            Given_UInt32s(0x8532E010); // srl %o3,0x00000010<32>,%g2
            AssertCode(
               "0|L--|00100000(4): 1 instructions",
               "1|L--|g2 = o3 >>u 0x10<32>");
        }

        [Test]
        public void SparcRw_fcmpd()
        {
            Given_UInt32s(0x81A90A47);	// fcmpd	%f4,%f38
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ELGU = cond(d19 - d2)");
        }

        [Test]
        public void SparcRw_fcmpq()
        {
            Given_UInt32s(0x81A90A65);	// fcmpq %f4,%f36
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ELGU = cond(q36 - q4)");
        }

        [Test]
        public void SparcRw_orn()
        {
            Given_HexString("A0340011");
            AssertCode(     // orn	%l0,%l1,%l0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|l0 = l0 | ~l1");
        }

        [Test]
        public void SparcRw_fcmped()
        {
            Given_HexString("81A88AC4");
            AssertCode(     // fcmped	%f2,%f4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ELGU = cond(d1 - d2)");
        }

        [Test]
        public void SparcRw_fbe()
        {
            Given_HexString("13800007");
            AssertCode(     // fbe	0010001C
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(EQ,E)) branch 0010001C");
        }

        [Test]
        public void SparcRw_fble()
        {
            Given_HexString("1B800016");
            AssertCode(     // fble	000107F4
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(LE,EL)) branch 00100058");
        }

        [Test]
        public void SparcRw_fmuld()
        {
            Given_HexString("85A08944");
            AssertCode(     // fmuld	%f2,%f4,%f2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d1 = d1 * d2");
        }

        [Test(Description = "Idiom used to retrieve the PC at the time of execution")]
        public void SparcRw_call_self()
        {
            //00010EA0 40000002 call fn00010EA8
            //00010EA4 2F000042 sethi 00000042,%l7
            //00010EA8 AE15E154 or %l7,00000154,%l7
            //00010EAC AE05C00F add %l7,%o7,%l7
            Given_HexString("40000002 2F000042 AE15E154 AE05C00F");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|o7 = 00100008",
                "2|L--|00100004(4): 1 instructions",
                "3|L--|l7 = 0x10800<32>",
                "4|L--|00100008(4): 1 instructions",
                "5|L--|l7 = l7 | 0x154<32>",
                "6|L--|0010000C(4): 1 instructions",
                "7|L--|l7 = l7 + o7");
        }



    }
}
