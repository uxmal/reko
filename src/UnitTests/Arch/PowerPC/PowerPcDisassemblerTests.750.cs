using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.PowerPC
{
    /// <summary>
    /// Unit tests for disassembling the instructions of the PowerPC 750 family
    /// </summary>
    public partial class PowerPcDisassemblerTests
    {
        [Test]
        public void PPC750Dis_ps_merge11()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x11D39CE0, "ps_merge11\tf14,f19,f19");
        }

        [Test]
        public void PPC750Dis_ps_cmpo0()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x129D0040, "ps_cmpo0\tcr4,f29,f0");
        }

        [Test]
        public void PPC750Dis_ps_madd()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x1008333A, "ps_madd\tf0,f8,f12,f6");
        }

        [Test]
        public void PPC750Dis_ps_madds0()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x100B015C, "ps_madds0\tf0,f11,f5,f0");
        }

        [Test]
        public void PPC750Dis_ps_madds1()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x1009011E, "ps_madds1\tf0,f9,f4,f0");
        }

        [Test]
        public void PPC750Dis_ps_muls0_Rc()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10111819, "ps_muls0.\tf0,f17,f0");
        }

        [Test]
        public void PPC750Dis_psq_lx()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x1015000C, "psq_lx\tf0,r21,r0,00,00");
        }

        [Test]
        public void PPC750Dis_psq_stx()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x13E0180E, "psq_stx\tf31,r0,r3,01,07");
        }

        [Test]
        public void PPC750Dis_psq_stux()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x1023674E, "psq_stux\tf1,r3,r12,01,00");
        }

        [Test]
        public void PPC750Dis_ps_muls1()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x1018191A, "ps_muls1\tf0,f24,f4");
        }

        [Test]
        public void PPC750Dis_psq_lux()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x101D104D, "psq_lux\tf0,r29,r2,00,00");
        }

        [Test]
        public void PPC750Dis_psq_st()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0xF0A501F0, "psq_st\tf5,r5,+000001F0,01,02");
        }

        [Test]
        public void PPC750Dis_ps_neg()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10000050, "ps_neg\tf0,f0");
        }

        [Test]
        public void PPC750Dis_ps_mul()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10000072, "ps_mul\tf0,f0,f1");
        }

        [Test]
        public void PPC750Dis_ps_muls0()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10000318, "ps_muls0\tf0,f0,f12");
        }

        [Test]
        public void PPC750Dis_ps_div()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10020024, "ps_div\tf0,f2,f0");
        }

        [Test]
        public void PPC750Dis_ps_sum0()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10040014, "ps_sum0\tf0,f4,f0,f0");
        }


        [Test]
        public void PPC750Dis_ps_rsqrte()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10530034, "ps_rsqrte\tf2,f0");
        }

        [Test]
        public void PPC750Dis_ps_mr()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10200090, "ps_mr\tf1,f0");
        }

        [Test]
        public void PPC750Dis_ps_res()
        {
            Given_ProcessorModel_750cl();
            AssertCode(0x10320030, "ps_res\tf1,f0");
        }
    }
}
