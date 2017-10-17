using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Arch.Msp430;
using NUnit.Framework;
using Reko.Core.Rtl;

namespace Reko.UnitTests.Arch.Msp430
{
    public class Msp430RewriterTests : RewriterTestBase
    {
        private Msp430Architecture arch;
        private MemoryArea image;

        public Msp430RewriterTests()
        {
            this.arch = new Msp430Architecture();
        }

        public override Address LoadAddress
        {
            get { return Address.Ptr16(0x0100); }
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        private void BuildTest(params byte[] bytes)
        {
            this.image = new MemoryArea(LoadAddress, bytes);
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            var rdr = image.CreateLeReader(image.BaseAddress);
            return arch.CreateRewriter(rdr, arch.CreateProcessorState(), binder, host);
        }

        [Test]
        public void Msp430Rw_mov()
        {
            BuildTest(0x3C, 0x40, 0xA0, 0xEE);	// mov.w	#EEA0,r12
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void Msp430Rw_xor()
        {
            BuildTest(0xA0, 0xEE, 0x3C, 0x90);	// xor.w	@r14,-6FC4(pc)
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_cmp()
        {
            BuildTest(0x3C, 0x90, 0xA0, 0xEE);	// cmp.w	#EEA0,r12
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jz()
        {
            BuildTest(0x07, 0x24);	// jz	0118
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_call()
        {
            BuildTest(0x8D, 0x12);	// call	r13
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_sub()
        {
            BuildTest(0x3D, 0x80, 0xA0, 0xEE);	// sub.w	#EEA0,r13
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_rra()
        {
            BuildTest(0x0D, 0x11);	// rra.w	r13
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_rrum()
        {
            BuildTest(0x5C, 0x03);	// rrum.w	r12
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_rrax()
        {
            BuildTest(0x4D, 0x18, 0x0C, 0x11);	// rpt #14 rrax.w	r12
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_add()
        {
            BuildTest(0x0D, 0x5C);	// add.w	r12,r13
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r13 = r13 + r12",
                "2|L--|VNZC = cond(r13)");
        }

        [Test]
        public void Msp430Rw_pushm()
        {
            BuildTest(0x1A, 0x15);	// pushm.w	#02,r10
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jnz()
        {
            BuildTest(0x22, 0x20);	// jnz	0190
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jc()
        {
            BuildTest(0x0B, 0x2C);	// jc	017A
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jnc()
        {
            BuildTest(0xF5, 0x2B);	// jnc	0164
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_popm()
        {
            BuildTest(0x19, 0x17);	// popm.w	#02,r9
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_addc()
        {
            BuildTest(0x6A, 0x64);	// addc.b	@r4,r10
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jge()
        {
            BuildTest(0x07, 0x34);	// jge	01FC
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jmp()
        {
            BuildTest(0xF8, 0x3F);	// jmp	01F6
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jl()
        {
            BuildTest(0x12, 0x38);	// jl	0272
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_and()
        {
            BuildTest(0xFE, 0xFF, 0xF9, 0x3F);	// and.b	@r15+,3FF9(r14)
            AssertCode(
                "0|L--|0100(4): 7 instructions",
                "1|L--|v3 = Mem0[r15:byte]",
                "2|L--|r15 = r15 + 1",
                "3|L--|v5 = Mem0[r14 + 16377:byte]",
                "4|L--|v5 = v5 & v3",
                "5|L--|Mem0[r14 + 16377:byte] = v5",
                "6|L--|V = false",
                "7|L--|NZC = cond(v5)");
        }

        [Test]
        public void Msp430Rw_subc()
        {
            BuildTest(0xB1, 0x79, 0x0E, 0x20);	// subc.w	@r9+,200E(sp)
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_bis()
        {
            BuildTest(0x0C, 0xDD);	// bis.w	r13,r12
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_bit()
        {
            BuildTest(0x66, 0xB1);	// bit.b	@sp,r6
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_bic()
        {
            BuildTest(0x16, 0xCB, 0x1C, 0x4A);	// bic.w	4A1C(r11),r6
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_rrc()
        {
            BuildTest(0x00, 0x10);	// rrc.w	pc
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_dadd()
        {
            BuildTest(0xB0, 0xA4, 0x3E, 0x40);	// dadd.w	@r4+,403E(pc)
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Msp430Rw_jn()
        {
            BuildTest(0x72, 0x31);	// jn	78E0
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|@@@");
        }
    }
}
