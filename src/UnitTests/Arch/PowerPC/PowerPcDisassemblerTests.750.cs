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
        public void PPC750Dis_ps_madd()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1008333A, "ps_madd\tf0,f8,f6,f12");
        }

        [Test]
        public void PPC750Dis_ps_madds1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1009011E, "ps_madds1\tf0,f9,f0,f4");
        }

        [Test]
        public void PPC750Dis_ps_madds0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x100B015C, "ps_madds0\tf0,f11,f0,f5");
        }

        [Test]
        public void PPC750Dis_ps_muls0_Rc()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10111819, "ps_muls0.\tf0,f17,f0");
        }

        [Test]
        public void PPC750Dis_psq_lx()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1015000C, "psq_lx\tf0,r21,r0,00,00");
        }

        [Test]
        public void PPC750Dis_psq_stx()
        {
            Given_ProcessorModel_750();
            AssertCode(0x13E0180E, "psq_stx\tf31,r0,r3,01,07");
        }

        [Test]
        public void PPC750Dis_psq_stux()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1023674E, "psq_stux\tf1,r3,r12,01,00");
        }

        [Test]
        public void PPC750Dis_ps_muls1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1018191A, "ps_muls1\tf0,f24,f4");
        }

        [Test]
        public void PPC750Dis_psq_lux()
        {
            Given_ProcessorModel_750();
            AssertCode(0x101D104D, "psq_lux\tf0,r29,r2,00,00");
        }

        [Test]
        public void PPC750Dis_psq_st()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0A501F0, "psq_st\tf5,r5,+000001F0,01,02");
        }

        [Test]
        public void PPC750Dis_ps_neg()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10000050, "ps_neg\tf0,f0");
        }

        [Test]
        public void PPC750Dis_ps_mul()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10000072, "ps_mul\tf0,f0,f1");
        }

        [Test]
        public void PPC750Dis_ps_muls0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10000318, "ps_muls0\tf0,f0,f12");
        }

        [Test]
        public void PPC750Dis_ps_div()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10020024, "ps_div\tf0,f2,f0");
        }

        [Test]
        public void PPC750Dis_ps_sum0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10040014, "ps_sum0\tf0,f4,f0,f0");
        }


        [Test]
        public void PPC750Dis_ps_rsqrte()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10530034, "ps_rsqrte\tf2,f0");
        }

        [Test]
        public void PPC750Dis_ps_mr()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10200090, "ps_mr\tf1,f0");
        }

        // Unknown PowerPC VX instruction 10248036 04-036 (54)
        [Test]
        public void PPC750Dis_10248036()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10248036, "@@@");
        }

        // Unknown PowerPC VX instruction 10280004 04-004 (4)
        [Test]
        public void PPC750Dis_10280004()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10280004, "@@@");
        }

        // Unknown PowerPC VX instruction 10290044 04-044 (68)
        [Test]
        public void PPC750Dis_10290044()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10290044, "@@@");
        }

        // Unknown PowerPC VX instruction 10290084 04-084 (132)
        [Test]
        public void PPC750Dis_10290084()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10290084, "@@@");
        }

        // Unknown PowerPC VX instruction 102A804B 04-04B(75)
        [Test]
        public void PPC750Dis_102A804B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x102A804B, "@@@");
        }

        // Unknown PowerPC VX instruction 102B0044 04-044 (68)
        [Test]
        public void PPC750Dis_102B0044()
        {
            Given_ProcessorModel_750();
            AssertCode(0x102B0044, "@@@");
        }

        // Unknown PowerPC VX instruction 102F0044 04-044 (68)
        [Test]
        public void PPC750Dis_102F0044()
        {
            Given_ProcessorModel_750();
            AssertCode(0x102F0044, "@@@");
        }

        // Unknown PowerPC VX instruction 10310044 04-044 (68)
        [Test]
        public void PPC750Dis_10310044()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10310044, "@@@");
        }

        // Unknown PowerPC VX instruction 10311053 04-053 (83)
        [Test]
        public void PPC750Dis_10311053()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10311053, "@@@");
        }

        [Test]
        public void PPC750Dis_ps_res()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10320030, "ps_res\tf1,f0");
        }

        // Unknown PowerPC VX instruction 10350004 04-004 (4)
        [Test]
        public void PPC750Dis_10350004()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10350004, "@@@");
        }

        // Unknown PowerPC VX instruction 10050044 04-044 (68)
        [Test]
        public void PPC750Dis_10050044()
        {
            AssertCode(0x10050044, "@@@");
        }

        // Unknown PowerPC VX instruction 10680008 04-008 (8)
        [Test]
        public void PPC750Dis_10680008()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10680008, "@@@");
        }

        // Unknown PowerPC VX instruction 107EF2E3 04-2E3 (739)
        [Test]
        public void PPC750Dis_107EF2E3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x107EF2E3, "@@@");
        }

        // Unknown PowerPC VX instruction 108BC940 04-140 (320)
        [Test]
        public void PPC750Dis_108BC940()
        {
            Given_ProcessorModel_750();
            AssertCode(0x108BC940, "@@@");
        }

        // Unknown PowerPC VX instruction 108CF1C8 04-1C8(456)
        [Test]
        public void PPC750Dis_108CF1C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0x108CF1C8, "@@@");
        }

        // Unknown PowerPC VX instruction 108D0140 04-140 (320)
        [Test]
        public void PPC750Dis_108D0140()
        {
            Given_ProcessorModel_750();
            AssertCode(0x108D0140, "@@@");
        }

        // Unknown PowerPC VX instruction 108E1E09 04-609 (1545)
        [Test]
        public void PPC750Dis_108E1E09()
        {
            Given_ProcessorModel_750();
            AssertCode(0x108E1E09, "@@@");
        }

        // Unknown PowerPC VX instruction 10980010 04-010 (16)
        [Test]
        public void PPC750Dis_10980010()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10980010, "@@@");
        }

        // Unknown PowerPC VX instruction 10A0AD05 04-505 (1285)
        [Test]
        public void PPC750Dis_10A0AD05()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10A0AD05, "@@@");
        }

        // Unknown PowerPC VX instruction 10C314A0 04-4A0(1184)
        [Test]
        public void PPC750Dis_10C314A0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10C314A0, "@@@");
        }

        // Unknown PowerPC VX instruction 10EB6FD2 04-7D2 (2002)
        [Test]
        public void PPC750Dis_10EB6FD2()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10EB6FD2, "@@@");
        }

        // Unknown PowerPC VX instruction 10FB4492 04-492 (1170)
        [Test]
        public void PPC750Dis_10FB4492()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10FB4492, "@@@");
        }

        // Unknown PowerPC VX instruction 111705E3 04-5E3 (1507)
        [Test]
        public void PPC750Dis_111705E3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x111705E3, "@@@");
        }

        // Unknown PowerPC VX instruction 11200843 04-043 (67)
        [Test]
        public void PPC750Dis_11200843()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11200843, "@@@");
        }

        // Unknown PowerPC VX instruction 11328062 04-062 (98)
        [Test]
        public void PPC750Dis_11328062()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11328062, "@@@");
        }

        // Unknown PowerPC VX instruction 11441177 04-177 (375)
        [Test]
        public void PPC750Dis_11441177()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11441177, "@@@");
        }

        // Unknown PowerPC VX instruction 114F0DE3 04-5E3 (1507)
        [Test]
        public void PPC750Dis_114F0DE3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x114F0DE3, "@@@");
        }

        // Unknown PowerPC VX instruction 116B0004 04-004 (4)
        [Test]
        public void PPC750Dis_116B0004()
        {
            Given_ProcessorModel_750();
            AssertCode(0x116B0004, "@@@");
        }

        // Unknown PowerPC VX instruction 11811F67 04-767 (1895)
        [Test]
        public void PPC750Dis_11811F67()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11811F67, "@@@");
        }

        // Unknown PowerPC VX instruction 11890008 04-008 (8)
        [Test]
        public void PPC750Dis_11890008()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11890008, "@@@");
        }

        // Unknown PowerPC VX instruction 118C6420 04-420 (1056)
        [Test]
        public void PPC750Dis_118C6420()
        {
            Given_ProcessorModel_750();
            AssertCode(0x118C6420, "@@@");
        }

        // Unknown PowerPC VX instruction 11C44144 04-144 (324)
        [Test]
        public void PPC750Dis_11C44144()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11C44144, "@@@");
        }

        [Test]
        public void PPC750Dis_ps_merge11()
        {
            Given_ProcessorModel_750();
            AssertCode(0x11D39CE0, "ps_merge11\tf14,f19,f19");
        }

        // Unknown PowerPC VX instruction 1200B001 04-001 (1)
        [Test]
        public void PPC750Dis_1200B001()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1200B001, "@@@");
        }

        // Unknown PowerPC VX instruction 1200C812 04-012 (18)
        [Test]
        public void PPC750Dis_1200C812()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1200C812, "@@@");
        }

        // Unknown PowerPC VX instruction 120A09E0 04-1E0 (480)
        [Test]
        public void PPC750Dis_120A09E0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x120A09E0, "@@@");
        }

        // Unknown PowerPC VX instruction 1214124A 04-24A(586)
        [Test]
        public void PPC750Dis_1214124A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1214124A, "@@@");
        }

        // Unknown PowerPC VX instruction 122E9F43 04-743 (1859)
        [Test]
        public void PPC750Dis_122E9F43()
        {
            Given_ProcessorModel_750();
            AssertCode(0x122E9F43, "@@@");
        }

        // Unknown PowerPC VX instruction 12708A36 04-236 (566)
        [Test]
        public void PPC750Dis_12708A36()
        {
            Given_ProcessorModel_750();
            AssertCode(0x12708A36, "@@@");
        }

        // Unknown PowerPC VX instruction 128012B7 04-2B7(695)
        [Test]
        public void PPC750Dis_128012B7()
        {
            Given_ProcessorModel_750();
            AssertCode(0x128012B7, "@@@");
        }

        [Test]
        public void PPC750Dis_ps_cmpo0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x129D0040, "ps_cmpo0\tcr4,f29,f0");
        }

        // Unknown PowerPC VX instruction 12E005C9 04-5C9(1481)
        [Test]
        public void PPC750Dis_12E005C9()
        {
            Given_ProcessorModel_750();
            AssertCode(0x12E005C9, "@@@");
        }

        // Unknown PowerPC VX instruction 12EE1326 04-326 (806)
        [Test]
        public void PPC750Dis_12EE1326()
        {
            Given_ProcessorModel_750();
            AssertCode(0x12EE1326, "@@@");
        }

        // Unknown PowerPC VX instruction 131B1A12 04-212 (530)
        [Test]
        public void PPC750Dis_131B1A12()
        {
            Given_ProcessorModel_750();
            AssertCode(0x131B1A12, "@@@");
        }

        // Unknown PowerPC VX instruction 13201747 04-747 (1863)
        [Test]
        public void PPC750Dis_13201747()
        {
            Given_ProcessorModel_750();
            AssertCode(0x13201747, "@@@");
        }

        // Unknown PowerPC VX instruction 13204BE2 04-3E2 (994)
        [Test]
        public void PPC750Dis_13204BE2()
        {
            Given_ProcessorModel_750();
            AssertCode(0x13204BE2, "@@@");
        }

        // Unknown PowerPC VX instruction 13A97D87 04-587 (1415)
        [Test]
        public void PPC750Dis_13A97D87()
        {
            Given_ProcessorModel_750();
            AssertCode(0x13A97D87, "@@@");
        }

        // Unknown PowerPC VX instruction 13B6670B 04-70B(1803)
        [Test]
        public void PPC750Dis_13B6670B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x13B6670B, "@@@");
        }

        // Unknown PowerPC VX instruction 13D90008 04-008 (8)
        [Test]
        public void PPC750Dis_13D90008()
        {
            Given_ProcessorModel_750();
            AssertCode(0x13D90008, "@@@");
        }

        // Unknown PowerPC X instruction 4C0000FE 13-07F (127)
        [Test]
        public void PPC750Dis_4C0000FE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C0000FE, "@@@");
        }

        // Unknown PowerPC X instruction 4C001B5E 13-1AF(431)
        [Test]
        public void PPC750Dis_4C001B5E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C001B5E, "@@@");
        }

        // Unknown PowerPC X instruction 4C001C7E 13-23F (575)
        [Test]
        public void PPC750Dis_4C001C7E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C001C7E, "@@@");
        }

        // Unknown PowerPC X instruction 4C00424F 13-127 (295)
        [Test]
        public void PPC750Dis_4C00424F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C00424F, "@@@");
        }

        // Unknown PowerPC X instruction 4C004D4B 13-2A5(677)
        [Test]
        public void PPC750Dis_4C004D4B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C004D4B, "@@@");
        }

        // Unknown PowerPC X instruction 4C005061 13-030 (48)
        [Test]
        public void PPC750Dis_4C005061()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C005061, "@@@");
        }

        // Unknown PowerPC X instruction 4C005348 13-1A4(420)
        [Test]
        public void PPC750Dis_4C005348()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C005348, "@@@");
        }

        // Unknown PowerPC X instruction 4C111C40 13-220 (544)
        [Test]
        public void PPC750Dis_4C111C40()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C111C40, "@@@");
        }

        // Unknown PowerPC X instruction 4C119AB0 13-158 (344)
        [Test]
        public void PPC750Dis_4C119AB0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C119AB0, "@@@");
        }

        // Unknown PowerPC X instruction 4C204D55 13-2AA(682)
        [Test]
        public void PPC750Dis_4C204D55()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C204D55, "@@@");
        }

        // Unknown PowerPC X instruction 4C204F4E 13-3A7(935)
        [Test]
        public void PPC750Dis_4C204F4E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C204F4E, "@@@");
        }

        // Unknown PowerPC X instruction 4C24D37E 13-1BF(447)
        [Test]
        public void PPC750Dis_4C24D37E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C24D37E, "@@@");
        }

        // Unknown PowerPC X instruction 4C312069 13-034 (52)
        [Test]
        public void PPC750Dis_4C312069()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C312069, "@@@");
        }

        // Unknown PowerPC X instruction 4C322063 13-031 (49)
        [Test]
        public void PPC750Dis_4C322063()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C322063, "@@@");
        }

        // Unknown PowerPC X instruction 4C41494C 13-0A6(166)
        [Test]
        public void PPC750Dis_4C41494C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C41494C, "@@@");
        }

        // Unknown PowerPC X instruction 4C414D45 13-2A2(674)
        [Test]
        public void PPC750Dis_4C414D45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C414D45, "@@@");
        }

        // Unknown PowerPC X instruction 4C414E44 13-322 (802)
        [Test]
        public void PPC750Dis_4C414E44()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C414E44, "@@@");
        }

        // Unknown PowerPC X instruction 4C414E4B 13-325 (805)
        [Test]
        public void PPC750Dis_4C414E4B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C414E4B, "@@@");
        }

        // Unknown PowerPC X instruction 4C41505D 13-02E (46)
        [Test]
        public void PPC750Dis_4C41505D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C41505D, "@@@");
        }

        // Unknown PowerPC X instruction 4C41534D 13-1A6(422)
        [Test]
        public void PPC750Dis_4C41534D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C41534D, "@@@");
        }

        // Unknown PowerPC X instruction 4C415445 13-222 (546)
        [Test]
        public void PPC750Dis_4C415445()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C415445, "@@@");
        }

        // Unknown PowerPC X instruction 4C415446 13-223 (547)
        [Test]
        public void PPC750Dis_4C415446()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C415446, "@@@");
        }

        // Unknown PowerPC X instruction 4C41545F 13-22F (559)
        [Test]
        public void PPC750Dis_4C41545F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C41545F, "@@@");
        }

        // Unknown PowerPC X instruction 4C415645 13-322 (802)
        [Test]
        public void PPC750Dis_4C415645()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C415645, "@@@");
        }

        // Unknown PowerPC X instruction 4C424F4E 13-3A7(935)
        [Test]
        public void PPC750Dis_4C424F4E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C424F4E, "@@@");
        }

        // Unknown PowerPC X instruction 4C42616C 13-0B6 (182)
        [Test]
        public void PPC750Dis_4C42616C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C42616C, "@@@");
        }

        // Unknown PowerPC X instruction 4C436F70 13-3B8(952)
        [Test]
        public void PPC750Dis_4C436F70()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C436F70, "@@@");
        }

        // Unknown PowerPC X instruction 4C454400 13-200 (512)
        [Test]
        public void PPC750Dis_4C454400()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C454400, "@@@");
        }

        // Unknown PowerPC X instruction 4C45525F 13-12F (303)
        [Test]
        public void PPC750Dis_4C45525F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C45525F, "@@@");
        }

        // Unknown PowerPC X instruction 4C455445 13-222 (546)
        [Test]
        public void PPC750Dis_4C455445()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C455445, "@@@");
        }

        // Unknown PowerPC X instruction 4C455452 13-229 (553)
        [Test]
        public void PPC750Dis_4C455452()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C455452, "@@@");
        }

        // Unknown PowerPC X instruction 4C455645 13-322 (802)
        [Test]
        public void PPC750Dis_4C455645()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C455645, "@@@");
        }

        // Unknown PowerPC X instruction 4C495445 13-222 (546)
        [Test]
        public void PPC750Dis_4C495445()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C495445, "@@@");
        }

        // Unknown PowerPC X instruction 4C4B5F53 13-3A9(937)
        [Test]
        public void PPC750Dis_4C4B5F53()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4B5F53, "@@@");
        }

        // Unknown PowerPC X instruction 4C4C2055 13-02A(42)
        [Test]
        public void PPC750Dis_4C4C2055()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4C2055, "@@@");
        }

        // Unknown PowerPC X instruction 4C4C4552 13-2A9(681)
        [Test]
        public void PPC750Dis_4C4C4552()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4C4552, "@@@");
        }

        // Unknown PowerPC X instruction 4C4C4F57 13-3AB(939)
        [Test]
        public void PPC750Dis_4C4C4F57()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4C4F57, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F424D 13-126 (294)
        [Test]
        public void PPC750Dis_4C4F424D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4F424D, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5452 13-229 (553)
        [Test]
        public void PPC750Dis_4C4F5452()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4F5452, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5544 13-2A2(674)
        [Test]
        public void PPC750Dis_4C4F5544()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4F5544, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5645 13-322 (802)
        [Test]
        public void PPC750Dis_4C4F5645()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4F5645, "@@@");
        }

        // Unknown PowerPC X instruction 4C4F5F53 13-3A9(937)
        [Test]
        public void PPC750Dis_4C4F5F53()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C4F5F53, "@@@");
        }

        // Unknown PowerPC X instruction 4C542030 13-018 (24)
        [Test]
        public void PPC750Dis_4C542030()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C542030, "@@@");
        }

        // Unknown PowerPC X instruction 4C545F46 13-3A3(931)
        [Test]
        public void PPC750Dis_4C545F46()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C545F46, "@@@");
        }

        // Unknown PowerPC X instruction 4C545F48 13-3A4(932)
        [Test]
        public void PPC750Dis_4C545F48()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C545F48, "@@@");
        }

        // Unknown PowerPC X instruction 4C554520 13-290 (656)
        [Test]
        public void PPC750Dis_4C554520()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C554520, "@@@");
        }

        // Unknown PowerPC X instruction 4C55475D 13-3AE(942)
        [Test]
        public void PPC750Dis_4C55475D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C55475D, "@@@");
        }

        // Unknown PowerPC X instruction 4C595D2E 13-297 (663)
        [Test]
        public void PPC750Dis_4C595D2E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C595D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4C5D2E72 13-339 (825)
        [Test]
        public void PPC750Dis_4C5D2E72()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C5D2E72, "@@@");
        }

        // Unknown PowerPC X instruction 4C5F5041 13-020 (32)
        [Test]
        public void PPC750Dis_4C5F5041()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C5F5041, "@@@");
        }

        // Unknown PowerPC X instruction 4C616E64 13-332 (818)
        [Test]
        public void PPC750Dis_4C616E64()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C616E64, "@@@");
        }

        // Unknown PowerPC X instruction 4C617373 13-1B9(441)
        [Test]
        public void PPC750Dis_4C617373()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C617373, "@@@");
        }

        // Unknown PowerPC X instruction 4C61756E 13-2B7(695)
        [Test]
        public void PPC750Dis_4C61756E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C61756E, "@@@");
        }

        // Unknown PowerPC X instruction 4C656164 13-0B2 (178)
        [Test]
        public void PPC750Dis_4C656164()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C656164, "@@@");
        }

        // Unknown PowerPC X instruction 4C65616B 13-0B5 (181)
        [Test]
        public void PPC750Dis_4C65616B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C65616B, "@@@");
        }

        // Unknown PowerPC X instruction 4C656674 13-33A(826)
        [Test]
        public void PPC750Dis_4C656674()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C656674, "@@@");
        }

        // Unknown PowerPC X instruction 4C656E67 13-333 (819)
        [Test]
        public void PPC750Dis_4C656E67()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C656E67, "@@@");
        }

        // Unknown PowerPC X instruction 4C657474 13-23A(570)
        [Test]
        public void PPC750Dis_4C657474()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C657474, "@@@");
        }

        // Unknown PowerPC X instruction 4C65F67A 13-33D (829)
        [Test]
        public void PPC750Dis_4C65F67A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C65F67A, "@@@");
        }

        // Unknown PowerPC X instruction 4C696665 13-332 (818)
        [Test]
        public void PPC750Dis_4C696665()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C696665, "@@@");
        }

        // Unknown PowerPC X instruction 4C696674 13-33A(826)
        [Test]
        public void PPC750Dis_4C696674()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C696674, "@@@");
        }

        // Unknown PowerPC X instruction 4C696768 13-3B4(948)
        [Test]
        public void PPC750Dis_4C696768()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C696768, "@@@");
        }

        // Unknown PowerPC X instruction 4C696E65 13-332 (818)
        [Test]
        public void PPC750Dis_4C696E65()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C696E65, "@@@");
        }

        // Unknown PowerPC X instruction 4C69EECE 13-367 (871)
        [Test]
        public void PPC750Dis_4C69EECE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C69EECE, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6164 13-0B2 (178)
        [Test]
        public void PPC750Dis_4C6F6164()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C6F6164, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6361 13-1B0(432)
        [Test]
        public void PPC750Dis_4C6F6361()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C6F6361, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6F6B 13-3B5(949)
        [Test]
        public void PPC750Dis_4C6F6F6B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C6F6F6B, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F6F70 13-3B8(952)
        [Test]
        public void PPC750Dis_4C6F6F70()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C6F6F70, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F7365 13-1B2(434)
        [Test]
        public void PPC750Dis_4C6F7365()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C6F7365, "@@@");
        }

        // Unknown PowerPC X instruction 4C6F7665 13-332 (818)
        [Test]
        public void PPC750Dis_4C6F7665()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C6F7665, "@@@");
        }

        // Unknown PowerPC X instruction 4C8C1527 13-293 (659)
        [Test]
        public void PPC750Dis_4C8C1527()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C8C1527, "@@@");
        }

        // Unknown PowerPC X instruction 4C916FED 13-3F6 (1014)
        [Test]
        public void PPC750Dis_4C916FED()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C916FED, "@@@");
        }

        // Unknown PowerPC X instruction 4C9B684E 13-027 (39)
        [Test]
        public void PPC750Dis_4C9B684E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4C9B684E, "@@@");
        }

        // Unknown PowerPC X instruction 4CA03CBA 13-25D (605)
        [Test]
        public void PPC750Dis_4CA03CBA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4CA03CBA, "@@@");
        }

        // Unknown PowerPC X instruction 4CC710DA 13-06D (109)
        [Test]
        public void PPC750Dis_4CC710DA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4CC710DA, "@@@");
        }

        // Unknown PowerPC X instruction 4CECC21A 13-10D (269)
        [Test]
        public void PPC750Dis_4CECC21A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4CECC21A, "@@@");
        }

        // Unknown PowerPC X instruction 4D00157E 13-2BF(703)
        [Test]
        public void PPC750Dis_4D00157E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D00157E, "@@@");
        }

        // Unknown PowerPC X instruction 4D0202BD 13-15E (350)
        [Test]
        public void PPC750Dis_4D0202BD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D0202BD, "@@@");
        }

        // Unknown PowerPC X instruction 4D128DBE 13-2DF(735)
        [Test]
        public void PPC750Dis_4D128DBE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D128DBE, "@@@");
        }

        // Unknown PowerPC X instruction 4D1C71CD 13-0E6 (230)
        [Test]
        public void PPC750Dis_4D1C71CD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D1C71CD, "@@@");
        }

        // Unknown PowerPC X instruction 4D204752 13-3A9(937)
        [Test]
        public void PPC750Dis_4D204752()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D204752, "@@@");
        }

        // Unknown PowerPC X instruction 4D204E05 13-302 (770)
        [Test]
        public void PPC750Dis_4D204E05()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D204E05, "@@@");
        }

        // Unknown PowerPC X instruction 4D2F8C18 13-20C(524)
        [Test]
        public void PPC750Dis_4D2F8C18()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D2F8C18, "@@@");
        }

        // Unknown PowerPC X instruction 4D38DC45 13-222 (546)
        [Test]
        public void PPC750Dis_4D38DC45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D38DC45, "@@@");
        }

        // Unknown PowerPC X instruction 4D412041 13-020 (32)
        [Test]
        public void PPC750Dis_4D412041()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D412041, "@@@");
        }

        // Unknown PowerPC X instruction 4D414E00 13-300 (768)
        [Test]
        public void PPC750Dis_4D414E00()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D414E00, "@@@");
        }

        // Unknown PowerPC X instruction 4D424943 13-0A1(161)
        [Test]
        public void PPC750Dis_4D424943()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D424943, "@@@");
        }

        // Unknown PowerPC X instruction 4D424945 13-0A2(162)
        [Test]
        public void PPC750Dis_4D424945()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D424945, "@@@");
        }

        // Unknown PowerPC X instruction 4D434175 13-0BA(186)
        [Test]
        public void PPC750Dis_4D434175()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D434175, "@@@");
        }

        // Unknown PowerPC X instruction 4D435053 13-029 (41)
        [Test]
        public void PPC750Dis_4D435053()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D435053, "@@@");
        }

        // Unknown PowerPC X instruction 4D454348 13-1A4(420)
        [Test]
        public void PPC750Dis_4D454348()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D454348, "@@@");
        }

        // Unknown PowerPC X instruction 4D455441 13-220 (544)
        [Test]
        public void PPC750Dis_4D455441()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D455441, "@@@");
        }

        // Unknown PowerPC X instruction 4D455D2E 13-297 (663)
        [Test]
        public void PPC750Dis_4D455D2E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D455D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4D475220 13-110 (272)
        [Test]
        public void PPC750Dis_4D475220()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D475220, "@@@");
        }

        // Unknown PowerPC X instruction 4D47DF84 13-3C2(962)
        [Test]
        public void PPC750Dis_4D47DF84()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D47DF84, "@@@");
        }

        // Unknown PowerPC X instruction 4D49545F 13-22F (559)
        [Test]
        public void PPC750Dis_4D49545F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D49545F, "@@@");
        }

        // Unknown PowerPC X instruction 4D4D5D2E 13-297 (663)
        [Test]
        public void PPC750Dis_4D4D5D2E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4D5D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4D4E0054 13-02A(42)
        [Test]
        public void PPC750Dis_4D4E0054()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4E0054, "@@@");
        }

        // Unknown PowerPC X instruction 4D4E5533 13-299 (665)
        [Test]
        public void PPC750Dis_4D4E5533()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4E5533, "@@@");
        }

        // Unknown PowerPC X instruction 4D4E5534 13-29A(666)
        [Test]
        public void PPC750Dis_4D4E5534()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4E5534, "@@@");
        }

        // Unknown PowerPC X instruction 4D4F4B45 13-1A2(418)
        [Test]
        public void PPC750Dis_4D4F4B45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4F4B45, "@@@");
        }

        // Unknown PowerPC X instruction 4D4F5245 13-122 (290)
        [Test]
        public void PPC750Dis_4D4F5245()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4F5245, "@@@");
        }

        // Unknown PowerPC X instruction 4D4F5645 13-322 (802)
        [Test]
        public void PPC750Dis_4D4F5645()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D4F5645, "@@@");
        }

        // Unknown PowerPC X instruction 4D5188A7 13-053 (83)
        [Test]
        public void PPC750Dis_4D5188A7()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D5188A7, "@@@");
        }

        // Unknown PowerPC X instruction 4D535049 13-024 (36)
        [Test]
        public void PPC750Dis_4D535049()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D535049, "@@@");
        }

        // Unknown PowerPC X instruction 4D5D2E64 13-332 (818)
        [Test]
        public void PPC750Dis_4D5D2E64()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D5D2E64, "@@@");
        }

        // Unknown PowerPC X instruction 4D5D2E76 13-33B(827)
        [Test]
        public void PPC750Dis_4D5D2E76()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D5D2E76, "@@@");
        }

        // Unknown PowerPC X instruction 4D5F4A45 13-122 (290)
        [Test]
        public void PPC750Dis_4D5F4A45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D5F4A45, "@@@");
        }

        // Unknown PowerPC X instruction 4D61676E 13-3B7(951)
        [Test]
        public void PPC750Dis_4D61676E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D61676E, "@@@");
        }

        // Unknown PowerPC X instruction 4D617374 13-1BA(442)
        [Test]
        public void PPC750Dis_4D617374()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D617374, "@@@");
        }

        // Unknown PowerPC X instruction 4D617463 13-231 (561)
        [Test]
        public void PPC750Dis_4D617463()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D617463, "@@@");
        }

        // Unknown PowerPC X instruction 4D617849 13-024 (36)
        [Test]
        public void PPC750Dis_4D617849()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D617849, "@@@");
        }

        // Unknown PowerPC X instruction 4D617853 13-029 (41)
        [Test]
        public void PPC750Dis_4D617853()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D617853, "@@@");
        }

        // Unknown PowerPC X instruction 4D650046 13-023 (35)
        [Test]
        public void PPC750Dis_4D650046()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D650046, "@@@");
        }

        // Unknown PowerPC X instruction 4D656C65 13-232 (562)
        [Test]
        public void PPC750Dis_4D656C65()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D656C65, "@@@");
        }

        // Unknown PowerPC X instruction 4D656D6F 13-2B7(695)
        [Test]
        public void PPC750Dis_4D656D6F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D656D6F, "@@@");
        }

        // Unknown PowerPC X instruction 4D657472 13-239 (569)
        [Test]
        public void PPC750Dis_4D657472()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D657472, "@@@");
        }

        // Unknown PowerPC X instruction 4D696E64 13-332 (818)
        [Test]
        public void PPC750Dis_4D696E64()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D696E64, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F6465 13-232 (562)
        [Test]
        public void PPC750Dis_4D6F6465()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D6F6465, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F756E 13-2B7(695)
        [Test]
        public void PPC750Dis_4D6F756E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D6F756E, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F7665 13-332 (818)
        [Test]
        public void PPC750Dis_4D6F7665()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D6F7665, "@@@");
        }

        // Unknown PowerPC X instruction 4D6F7669 13-334 (820)
        [Test]
        public void PPC750Dis_4D6F7669()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D6F7669, "@@@");
        }

        // Unknown PowerPC X instruction 4D757369 13-1B4(436)
        [Test]
        public void PPC750Dis_4D757369()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D757369, "@@@");
        }

        // Unknown PowerPC X instruction 4D772FC0 13-3E0 (992)
        [Test]
        public void PPC750Dis_4D772FC0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4D772FC0, "@@@");
        }

        // Unknown PowerPC X instruction 4DABE0E1 13-070 (112)
        [Test]
        public void PPC750Dis_4DABE0E1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4DABE0E1, "@@@");
        }

        // Unknown PowerPC X instruction 4DDDA0AE 13-057 (87)
        [Test]
        public void PPC750Dis_4DDDA0AE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4DDDA0AE, "@@@");
        }

        // Unknown PowerPC X instruction 4DE96EA9 13-354 (852)
        [Test]
        public void PPC750Dis_4DE96EA9()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4DE96EA9, "@@@");
        }

        // Unknown PowerPC X instruction 4DF06BFB 13-1FD(509)
        [Test]
        public void PPC750Dis_4DF06BFB()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4DF06BFB, "@@@");
        }

        // Unknown PowerPC X instruction 4E004E50 13-328 (808)
        [Test]
        public void PPC750Dis_4E004E50()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E004E50, "@@@");
        }

        // Unknown PowerPC X instruction 4E0A0268 13-134 (308)
        [Test]
        public void PPC750Dis_4E0A0268()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E0A0268, "@@@");
        }

        // Unknown PowerPC X instruction 4E204E56 13-32B(811)
        [Test]
        public void PPC750Dis_4E204E56()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E204E56, "@@@");
        }

        // Unknown PowerPC X instruction 4E205341 13-1A0(416)
        [Test]
        public void PPC750Dis_4E205341()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E205341, "@@@");
        }

        // Unknown PowerPC X instruction 4E205549 13-2A4(676)
        [Test]
        public void PPC750Dis_4E205549()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E205549, "@@@");
        }

        // Unknown PowerPC X instruction 4E2101DE 13-0EF (239)
        [Test]
        public void PPC750Dis_4E2101DE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E2101DE, "@@@");
        }

        // Unknown PowerPC X instruction 4E412045 13-022 (34)
        [Test]
        public void PPC750Dis_4E412045()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E412045, "@@@");
        }

        // Unknown PowerPC X instruction 4E440050 13-028 (40)
        [Test]
        public void PPC750Dis_4E440050()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E440050, "@@@");
        }

        // Unknown PowerPC X instruction 4E443100 13-080 (128)
        [Test]
        public void PPC750Dis_4E443100()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E443100, "@@@");
        }

        // Unknown PowerPC X instruction 4E443300 13-180 (384)
        [Test]
        public void PPC750Dis_4E443300()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E443300, "@@@");
        }

        // Unknown PowerPC X instruction 4E445550 13-2A8(680)
        [Test]
        public void PPC750Dis_4E445550()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445550, "@@@");
        }

        // Unknown PowerPC X instruction 4E445D2E 13-297 (663)
        [Test]
        public void PPC750Dis_4E445D2E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F41 13-3A0(928)
        [Test]
        public void PPC750Dis_4E445F41()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F41, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F42 13-3A1(929)
        [Test]
        public void PPC750Dis_4E445F42()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F42, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F43 13-3A1(929)
        [Test]
        public void PPC750Dis_4E445F43()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F43, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F47 13-3A3(931)
        [Test]
        public void PPC750Dis_4E445F47()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F47, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F48 13-3A4(932)
        [Test]
        public void PPC750Dis_4E445F48()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F48, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4B 13-3A5(933)
        [Test]
        public void PPC750Dis_4E445F4B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F4B, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4C 13-3A6(934)
        [Test]
        public void PPC750Dis_4E445F4C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F4C, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4D 13-3A6(934)
        [Test]
        public void PPC750Dis_4E445F4D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F4D, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F4F 13-3A7(935)
        [Test]
        public void PPC750Dis_4E445F4F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F4F, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F50 13-3A8(936)
        [Test]
        public void PPC750Dis_4E445F50()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F50, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F53 13-3A9(937)
        [Test]
        public void PPC750Dis_4E445F53()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F53, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F54 13-3AA(938)
        [Test]
        public void PPC750Dis_4E445F54()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F54, "@@@");
        }

        // Unknown PowerPC X instruction 4E445F56 13-3AB(939)
        [Test]
        public void PPC750Dis_4E445F56()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E445F56, "@@@");
        }

        // Unknown PowerPC X instruction 4E450057 13-02B(43)
        [Test]
        public void PPC750Dis_4E450057()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E450057, "@@@");
        }

        // Unknown PowerPC X instruction 4E452030 13-018 (24)
        [Test]
        public void PPC750Dis_4E452030()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E452030, "@@@");
        }

        // Unknown PowerPC X instruction 4E455753 13-3A9(937)
        [Test]
        public void PPC750Dis_4E455753()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E455753, "@@@");
        }

        // Unknown PowerPC X instruction 4E46006C 13-036 (54)
        [Test]
        public void PPC750Dis_4E46006C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E46006C, "@@@");
        }

        // Unknown PowerPC X instruction 4E464952 13-0A9(169)
        [Test]
        public void PPC750Dis_4E464952()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E464952, "@@@");
        }

        // Unknown PowerPC X instruction 4E474230 13-118 (280)
        [Test]
        public void PPC750Dis_4E474230()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474230, "@@@");
        }

        // Unknown PowerPC X instruction 4E474231 13-118 (280)
        [Test]
        public void PPC750Dis_4E474231()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474231, "@@@");
        }

        // Unknown PowerPC X instruction 4E474232 13-119 (281)
        [Test]
        public void PPC750Dis_4E474232()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474232, "@@@");
        }

        // Unknown PowerPC X instruction 4E474233 13-119 (281)
        [Test]
        public void PPC750Dis_4E474233()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474233, "@@@");
        }

        // Unknown PowerPC X instruction 4E474234 13-11A(282)
        [Test]
        public void PPC750Dis_4E474234()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474234, "@@@");
        }

        // Unknown PowerPC X instruction 4E474235 13-11A(282)
        [Test]
        public void PPC750Dis_4E474235()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474235, "@@@");
        }

        // Unknown PowerPC X instruction 4E474236 13-11B(283)
        [Test]
        public void PPC750Dis_4E474236()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474236, "@@@");
        }

        // Unknown PowerPC X instruction 4E474237 13-11B(283)
        [Test]
        public void PPC750Dis_4E474237()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474237, "@@@");
        }

        // Unknown PowerPC X instruction 4E474238 13-11C(284)
        [Test]
        public void PPC750Dis_4E474238()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474238, "@@@");
        }

        // Unknown PowerPC X instruction 4E474239 13-11C(284)
        [Test]
        public void PPC750Dis_4E474239()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474239, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423A 13-11D (285)
        [Test]
        public void PPC750Dis_4E47423A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47423A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423B 13-11D (285)
        [Test]
        public void PPC750Dis_4E47423B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47423B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423C 13-11E (286)
        [Test]
        public void PPC750Dis_4E47423C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47423C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423D 13-11E (286)
        [Test]
        public void PPC750Dis_4E47423D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47423D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423E 13-11F (287)
        [Test]
        public void PPC750Dis_4E47423E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47423E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47423F 13-11F (287)
        [Test]
        public void PPC750Dis_4E47423F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47423F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474240 13-120 (288)
        [Test]
        public void PPC750Dis_4E474240()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474240, "@@@");
        }

        // Unknown PowerPC X instruction 4E474241 13-120 (288)
        [Test]
        public void PPC750Dis_4E474241()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474241, "@@@");
        }

        // Unknown PowerPC X instruction 4E474244 13-122 (290)
        [Test]
        public void PPC750Dis_4E474244()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474244, "@@@");
        }

        // Unknown PowerPC X instruction 4E474245 13-122 (290)
        [Test]
        public void PPC750Dis_4E474245()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474245, "@@@");
        }

        // Unknown PowerPC X instruction 4E474246 13-123 (291)
        [Test]
        public void PPC750Dis_4E474246()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474246, "@@@");
        }

        // Unknown PowerPC X instruction 4E474247 13-123 (291)
        [Test]
        public void PPC750Dis_4E474247()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474247, "@@@");
        }

        // Unknown PowerPC X instruction 4E474248 13-124 (292)
        [Test]
        public void PPC750Dis_4E474248()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474248, "@@@");
        }

        // Unknown PowerPC X instruction 4E474249 13-124 (292)
        [Test]
        public void PPC750Dis_4E474249()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474249, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424A 13-125 (293)
        [Test]
        public void PPC750Dis_4E47424A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47424A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424B 13-125 (293)
        [Test]
        public void PPC750Dis_4E47424B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47424B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424C 13-126 (294)
        [Test]
        public void PPC750Dis_4E47424C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47424C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424D 13-126 (294)
        [Test]
        public void PPC750Dis_4E47424D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47424D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424E 13-127 (295)
        [Test]
        public void PPC750Dis_4E47424E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47424E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47424F 13-127 (295)
        [Test]
        public void PPC750Dis_4E47424F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47424F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474250 13-128 (296)
        [Test]
        public void PPC750Dis_4E474250()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474250, "@@@");
        }

        // Unknown PowerPC X instruction 4E474251 13-128 (296)
        [Test]
        public void PPC750Dis_4E474251()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474251, "@@@");
        }

        // Unknown PowerPC X instruction 4E474252 13-129 (297)
        [Test]
        public void PPC750Dis_4E474252()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474252, "@@@");
        }

        // Unknown PowerPC X instruction 4E474253 13-129 (297)
        [Test]
        public void PPC750Dis_4E474253()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474253, "@@@");
        }

        // Unknown PowerPC X instruction 4E474254 13-12A(298)
        [Test]
        public void PPC750Dis_4E474254()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474254, "@@@");
        }

        // Unknown PowerPC X instruction 4E474255 13-12A(298)
        [Test]
        public void PPC750Dis_4E474255()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474255, "@@@");
        }

        // Unknown PowerPC X instruction 4E474256 13-12B(299)
        [Test]
        public void PPC750Dis_4E474256()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474256, "@@@");
        }

        // Unknown PowerPC X instruction 4E474257 13-12B(299)
        [Test]
        public void PPC750Dis_4E474257()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474257, "@@@");
        }

        // Unknown PowerPC X instruction 4E474258 13-12C(300)
        [Test]
        public void PPC750Dis_4E474258()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474258, "@@@");
        }

        // Unknown PowerPC X instruction 4E474259 13-12C(300)
        [Test]
        public void PPC750Dis_4E474259()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474259, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425A 13-12D (301)
        [Test]
        public void PPC750Dis_4E47425A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47425A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425B 13-12D (301)
        [Test]
        public void PPC750Dis_4E47425B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47425B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425C 13-12E (302)
        [Test]
        public void PPC750Dis_4E47425C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47425C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425D 13-12E (302)
        [Test]
        public void PPC750Dis_4E47425D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47425D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425E 13-12F (303)
        [Test]
        public void PPC750Dis_4E47425E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47425E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47425F 13-12F (303)
        [Test]
        public void PPC750Dis_4E47425F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47425F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474260 13-130 (304)
        [Test]
        public void PPC750Dis_4E474260()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474260, "@@@");
        }

        // Unknown PowerPC X instruction 4E474261 13-130 (304)
        [Test]
        public void PPC750Dis_4E474261()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474261, "@@@");
        }

        // Unknown PowerPC X instruction 4E474262 13-131 (305)
        [Test]
        public void PPC750Dis_4E474262()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474262, "@@@");
        }

        // Unknown PowerPC X instruction 4E474263 13-131 (305)
        [Test]
        public void PPC750Dis_4E474263()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474263, "@@@");
        }

        // Unknown PowerPC X instruction 4E474430 13-218 (536)
        [Test]
        public void PPC750Dis_4E474430()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474430, "@@@");
        }

        // Unknown PowerPC X instruction 4E474431 13-218 (536)
        [Test]
        public void PPC750Dis_4E474431()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474431, "@@@");
        }

        // Unknown PowerPC X instruction 4E474542 13-2A1(673)
        [Test]
        public void PPC750Dis_4E474542()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474542, "@@@");
        }

        // Unknown PowerPC X instruction 4E474A30 13-118 (280)
        [Test]
        public void PPC750Dis_4E474A30()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474A30, "@@@");
        }

        // Unknown PowerPC X instruction 4E474A31 13-118 (280)
        [Test]
        public void PPC750Dis_4E474A31()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474A31, "@@@");
        }

        // Unknown PowerPC X instruction 4E474A32 13-119 (281)
        [Test]
        public void PPC750Dis_4E474A32()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474A32, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D30 13-298 (664)
        [Test]
        public void PPC750Dis_4E474D30()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D30, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D31 13-298 (664)
        [Test]
        public void PPC750Dis_4E474D31()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D31, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D32 13-299 (665)
        [Test]
        public void PPC750Dis_4E474D32()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D32, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D33 13-299 (665)
        [Test]
        public void PPC750Dis_4E474D33()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D33, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D34 13-29A(666)
        [Test]
        public void PPC750Dis_4E474D34()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D34, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D35 13-29A(666)
        [Test]
        public void PPC750Dis_4E474D35()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D35, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D36 13-29B(667)
        [Test]
        public void PPC750Dis_4E474D36()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D36, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D37 13-29B(667)
        [Test]
        public void PPC750Dis_4E474D37()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D37, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D38 13-29C(668)
        [Test]
        public void PPC750Dis_4E474D38()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D38, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D39 13-29C(668)
        [Test]
        public void PPC750Dis_4E474D39()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D39, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3A 13-29D (669)
        [Test]
        public void PPC750Dis_4E474D3A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D3A, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3B 13-29D (669)
        [Test]
        public void PPC750Dis_4E474D3B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D3B, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3C 13-29E (670)
        [Test]
        public void PPC750Dis_4E474D3C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D3C, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3D 13-29E (670)
        [Test]
        public void PPC750Dis_4E474D3D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D3D, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3E 13-29F (671)
        [Test]
        public void PPC750Dis_4E474D3E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D3E, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D3F 13-29F (671)
        [Test]
        public void PPC750Dis_4E474D3F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D3F, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D40 13-2A0(672)
        [Test]
        public void PPC750Dis_4E474D40()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D40, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D41 13-2A0(672)
        [Test]
        public void PPC750Dis_4E474D41()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D41, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D42 13-2A1(673)
        [Test]
        public void PPC750Dis_4E474D42()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D42, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D43 13-2A1(673)
        [Test]
        public void PPC750Dis_4E474D43()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D43, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D44 13-2A2(674)
        [Test]
        public void PPC750Dis_4E474D44()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D44, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D45 13-2A2(674)
        [Test]
        public void PPC750Dis_4E474D45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D45, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D46 13-2A3(675)
        [Test]
        public void PPC750Dis_4E474D46()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D46, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D47 13-2A3(675)
        [Test]
        public void PPC750Dis_4E474D47()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D47, "@@@");
        }

        // Unknown PowerPC X instruction 4E474D48 13-2A4(676)
        [Test]
        public void PPC750Dis_4E474D48()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474D48, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E30 13-318 (792)
        [Test]
        public void PPC750Dis_4E474E30()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474E30, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E35 13-31A(794)
        [Test]
        public void PPC750Dis_4E474E35()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474E35, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E36 13-31B(795)
        [Test]
        public void PPC750Dis_4E474E36()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474E36, "@@@");
        }

        // Unknown PowerPC X instruction 4E474E37 13-31B(795)
        [Test]
        public void PPC750Dis_4E474E37()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E474E37, "@@@");
        }

        // Unknown PowerPC X instruction 4E475230 13-118 (280)
        [Test]
        public void PPC750Dis_4E475230()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475230, "@@@");
        }

        // Unknown PowerPC X instruction 4E475231 13-118 (280)
        [Test]
        public void PPC750Dis_4E475231()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475231, "@@@");
        }

        // Unknown PowerPC X instruction 4E475232 13-119 (281)
        [Test]
        public void PPC750Dis_4E475232()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475232, "@@@");
        }

        // Unknown PowerPC X instruction 4E475233 13-119 (281)
        [Test]
        public void PPC750Dis_4E475233()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475233, "@@@");
        }

        // Unknown PowerPC X instruction 4E475234 13-11A(282)
        [Test]
        public void PPC750Dis_4E475234()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475234, "@@@");
        }

        // Unknown PowerPC X instruction 4E475235 13-11A(282)
        [Test]
        public void PPC750Dis_4E475235()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475235, "@@@");
        }

        // Unknown PowerPC X instruction 4E475236 13-11B(283)
        [Test]
        public void PPC750Dis_4E475236()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475236, "@@@");
        }

        // Unknown PowerPC X instruction 4E475237 13-11B(283)
        [Test]
        public void PPC750Dis_4E475237()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475237, "@@@");
        }

        // Unknown PowerPC X instruction 4E475238 13-11C(284)
        [Test]
        public void PPC750Dis_4E475238()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475238, "@@@");
        }

        // Unknown PowerPC X instruction 4E475239 13-11C(284)
        [Test]
        public void PPC750Dis_4E475239()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475239, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523A 13-11D (285)
        [Test]
        public void PPC750Dis_4E47523A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47523A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523B 13-11D (285)
        [Test]
        public void PPC750Dis_4E47523B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47523B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523C 13-11E (286)
        [Test]
        public void PPC750Dis_4E47523C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47523C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523D 13-11E (286)
        [Test]
        public void PPC750Dis_4E47523D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47523D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523E 13-11F (287)
        [Test]
        public void PPC750Dis_4E47523E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47523E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47523F 13-11F (287)
        [Test]
        public void PPC750Dis_4E47523F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47523F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475240 13-120 (288)
        [Test]
        public void PPC750Dis_4E475240()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475240, "@@@");
        }

        // Unknown PowerPC X instruction 4E475241 13-120 (288)
        [Test]
        public void PPC750Dis_4E475241()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475241, "@@@");
        }

        // Unknown PowerPC X instruction 4E475244 13-122 (290)
        [Test]
        public void PPC750Dis_4E475244()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475244, "@@@");
        }

        // Unknown PowerPC X instruction 4E475245 13-122 (290)
        [Test]
        public void PPC750Dis_4E475245()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475245, "@@@");
        }

        // Unknown PowerPC X instruction 4E475246 13-123 (291)
        [Test]
        public void PPC750Dis_4E475246()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475246, "@@@");
        }

        // Unknown PowerPC X instruction 4E475247 13-123 (291)
        [Test]
        public void PPC750Dis_4E475247()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475247, "@@@");
        }

        // Unknown PowerPC X instruction 4E475248 13-124 (292)
        [Test]
        public void PPC750Dis_4E475248()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475248, "@@@");
        }

        // Unknown PowerPC X instruction 4E475249 13-124 (292)
        [Test]
        public void PPC750Dis_4E475249()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475249, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524A 13-125 (293)
        [Test]
        public void PPC750Dis_4E47524A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47524A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524B 13-125 (293)
        [Test]
        public void PPC750Dis_4E47524B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47524B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524C 13-126 (294)
        [Test]
        public void PPC750Dis_4E47524C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47524C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524D 13-126 (294)
        [Test]
        public void PPC750Dis_4E47524D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47524D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524E 13-127 (295)
        [Test]
        public void PPC750Dis_4E47524E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47524E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47524F 13-127 (295)
        [Test]
        public void PPC750Dis_4E47524F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47524F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475250 13-128 (296)
        [Test]
        public void PPC750Dis_4E475250()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475250, "@@@");
        }

        // Unknown PowerPC X instruction 4E475251 13-128 (296)
        [Test]
        public void PPC750Dis_4E475251()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475251, "@@@");
        }

        // Unknown PowerPC X instruction 4E475252 13-129 (297)
        [Test]
        public void PPC750Dis_4E475252()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475252, "@@@");
        }

        // Unknown PowerPC X instruction 4E475253 13-129 (297)
        [Test]
        public void PPC750Dis_4E475253()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475253, "@@@");
        }

        // Unknown PowerPC X instruction 4E475254 13-12A(298)
        [Test]
        public void PPC750Dis_4E475254()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475254, "@@@");
        }

        // Unknown PowerPC X instruction 4E475255 13-12A(298)
        [Test]
        public void PPC750Dis_4E475255()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475255, "@@@");
        }

        // Unknown PowerPC X instruction 4E475256 13-12B(299)
        [Test]
        public void PPC750Dis_4E475256()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475256, "@@@");
        }

        // Unknown PowerPC X instruction 4E475257 13-12B(299)
        [Test]
        public void PPC750Dis_4E475257()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475257, "@@@");
        }

        // Unknown PowerPC X instruction 4E475258 13-12C(300)
        [Test]
        public void PPC750Dis_4E475258()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475258, "@@@");
        }

        // Unknown PowerPC X instruction 4E475259 13-12C(300)
        [Test]
        public void PPC750Dis_4E475259()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475259, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525A 13-12D (301)
        [Test]
        public void PPC750Dis_4E47525A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47525A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525B 13-12D (301)
        [Test]
        public void PPC750Dis_4E47525B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47525B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525C 13-12E (302)
        [Test]
        public void PPC750Dis_4E47525C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47525C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525D 13-12E (302)
        [Test]
        public void PPC750Dis_4E47525D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47525D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525E 13-12F (303)
        [Test]
        public void PPC750Dis_4E47525E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47525E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47525F 13-12F (303)
        [Test]
        public void PPC750Dis_4E47525F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47525F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475260 13-130 (304)
        [Test]
        public void PPC750Dis_4E475260()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475260, "@@@");
        }

        // Unknown PowerPC X instruction 4E475261 13-130 (304)
        [Test]
        public void PPC750Dis_4E475261()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475261, "@@@");
        }

        // Unknown PowerPC X instruction 4E475262 13-131 (305)
        [Test]
        public void PPC750Dis_4E475262()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475262, "@@@");
        }

        // Unknown PowerPC X instruction 4E475263 13-131 (305)
        [Test]
        public void PPC750Dis_4E475263()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475263, "@@@");
        }

        // Unknown PowerPC X instruction 4E475264 13-132 (306)
        [Test]
        public void PPC750Dis_4E475264()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475264, "@@@");
        }

        // Unknown PowerPC X instruction 4E475265 13-132 (306)
        [Test]
        public void PPC750Dis_4E475265()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475265, "@@@");
        }

        // Unknown PowerPC X instruction 4E475266 13-133 (307)
        [Test]
        public void PPC750Dis_4E475266()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475266, "@@@");
        }

        // Unknown PowerPC X instruction 4E475267 13-133 (307)
        [Test]
        public void PPC750Dis_4E475267()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475267, "@@@");
        }

        // Unknown PowerPC X instruction 4E475268 13-134 (308)
        [Test]
        public void PPC750Dis_4E475268()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475268, "@@@");
        }

        // Unknown PowerPC X instruction 4E475269 13-134 (308)
        [Test]
        public void PPC750Dis_4E475269()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475269, "@@@");
        }

        // Unknown PowerPC X instruction 4E47526A 13-135 (309)
        [Test]
        public void PPC750Dis_4E47526A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47526A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47526B 13-135 (309)
        [Test]
        public void PPC750Dis_4E47526B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47526B, "@@@");
        }

        // Unknown PowerPC X instruction 4E475330 13-198 (408)
        [Test]
        public void PPC750Dis_4E475330()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475330, "@@@");
        }

        // Unknown PowerPC X instruction 4E475331 13-198 (408)
        [Test]
        public void PPC750Dis_4E475331()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475331, "@@@");
        }

        // Unknown PowerPC X instruction 4E475332 13-199 (409)
        [Test]
        public void PPC750Dis_4E475332()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475332, "@@@");
        }

        // Unknown PowerPC X instruction 4E475333 13-199 (409)
        [Test]
        public void PPC750Dis_4E475333()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475333, "@@@");
        }

        // Unknown PowerPC X instruction 4E475334 13-19A(410)
        [Test]
        public void PPC750Dis_4E475334()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475334, "@@@");
        }

        // Unknown PowerPC X instruction 4E475335 13-19A(410)
        [Test]
        public void PPC750Dis_4E475335()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475335, "@@@");
        }

        // Unknown PowerPC X instruction 4E475336 13-19B(411)
        [Test]
        public void PPC750Dis_4E475336()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475336, "@@@");
        }

        // Unknown PowerPC X instruction 4E475337 13-19B(411)
        [Test]
        public void PPC750Dis_4E475337()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475337, "@@@");
        }

        // Unknown PowerPC X instruction 4E475338 13-19C(412)
        [Test]
        public void PPC750Dis_4E475338()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475338, "@@@");
        }

        // Unknown PowerPC X instruction 4E475339 13-19C(412)
        [Test]
        public void PPC750Dis_4E475339()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475339, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533A 13-19D (413)
        [Test]
        public void PPC750Dis_4E47533A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47533A, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533B 13-19D (413)
        [Test]
        public void PPC750Dis_4E47533B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47533B, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533C 13-19E (414)
        [Test]
        public void PPC750Dis_4E47533C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47533C, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533D 13-19E (414)
        [Test]
        public void PPC750Dis_4E47533D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47533D, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533E 13-19F (415)
        [Test]
        public void PPC750Dis_4E47533E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47533E, "@@@");
        }

        // Unknown PowerPC X instruction 4E47533F 13-19F (415)
        [Test]
        public void PPC750Dis_4E47533F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E47533F, "@@@");
        }

        // Unknown PowerPC X instruction 4E475340 13-1A0(416)
        [Test]
        public void PPC750Dis_4E475340()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475340, "@@@");
        }

        // Unknown PowerPC X instruction 4E475341 13-1A0(416)
        [Test]
        public void PPC750Dis_4E475341()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475341, "@@@");
        }

        // Unknown PowerPC X instruction 4E475342 13-1A1(417)
        [Test]
        public void PPC750Dis_4E475342()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475342, "@@@");
        }

        // Unknown PowerPC X instruction 4E475430 13-218 (536)
        [Test]
        public void PPC750Dis_4E475430()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475430, "@@@");
        }

        // Unknown PowerPC X instruction 4E475431 13-218 (536)
        [Test]
        public void PPC750Dis_4E475431()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475431, "@@@");
        }

        // Unknown PowerPC X instruction 4E475432 13-219 (537)
        [Test]
        public void PPC750Dis_4E475432()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475432, "@@@");
        }

        // Unknown PowerPC X instruction 4E475433 13-219 (537)
        [Test]
        public void PPC750Dis_4E475433()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475433, "@@@");
        }

        // Unknown PowerPC X instruction 4E475434 13-21A(538)
        [Test]
        public void PPC750Dis_4E475434()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475434, "@@@");
        }

        // Unknown PowerPC X instruction 4E475435 13-21A(538)
        [Test]
        public void PPC750Dis_4E475435()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475435, "@@@");
        }

        // Unknown PowerPC X instruction 4E475830 13-018 (24)
        [Test]
        public void PPC750Dis_4E475830()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475830, "@@@");
        }

        // Unknown PowerPC X instruction 4E475831 13-018 (24)
        [Test]
        public void PPC750Dis_4E475831()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475831, "@@@");
        }

        // Unknown PowerPC X instruction 4E475832 13-019 (25)
        [Test]
        public void PPC750Dis_4E475832()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475832, "@@@");
        }

        // Unknown PowerPC X instruction 4E475D2E 13-297 (663)
        [Test]
        public void PPC750Dis_4E475D2E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E475D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4E4B544F 13-227 (551)
        [Test]
        public void PPC750Dis_4E4B544F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E4B544F, "@@@");
        }

        // Unknown PowerPC X instruction 4E4B5F4A 13-3A5(933)
        [Test]
        public void PPC750Dis_4E4B5F4A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E4B5F4A, "@@@");
        }

        // Unknown PowerPC X instruction 4E4B5F50 13-3A8(936)
        [Test]
        public void PPC750Dis_4E4B5F50()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E4B5F50, "@@@");
        }

        // Unknown PowerPC X instruction 4E4D4752 13-3A9(937)
        [Test]
        public void PPC750Dis_4E4D4752()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E4D4752, "@@@");
        }

        // Unknown PowerPC X instruction 4E4D8266 13-133 (307)
        [Test]
        public void PPC750Dis_4E4D8266()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E4D8266, "@@@");
        }

        // Unknown PowerPC X instruction 4E4F4E45 13-322 (802)
        [Test]
        public void PPC750Dis_4E4F4E45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E4F4E45, "@@@");
        }

        // Unknown PowerPC X instruction 4E504320 13-190 (400)
        [Test]
        public void PPC750Dis_4E504320()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E504320, "@@@");
        }

        // Unknown PowerPC X instruction 4E504330 13-198 (408)
        [Test]
        public void PPC750Dis_4E504330()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E504330, "@@@");
        }

        // Unknown PowerPC X instruction 4E504342 13-1A1(417)
        [Test]
        public void PPC750Dis_4E504342()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E504342, "@@@");
        }

        // Unknown PowerPC X instruction 4E504344 13-1A2(418)
        [Test]
        public void PPC750Dis_4E504344()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E504344, "@@@");
        }

        // Unknown PowerPC X instruction 4E504353 13-1A9(425)
        [Test]
        public void PPC750Dis_4E504353()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E504353, "@@@");
        }

        // Unknown PowerPC X instruction 4E50435F 13-1AF(431)
        [Test]
        public void PPC750Dis_4E50435F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E50435F, "@@@");
        }

        // Unknown PowerPC X instruction 4E50437C 13-1BE(446)
        [Test]
        public void PPC750Dis_4E50437C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E50437C, "@@@");
        }

        // Unknown PowerPC X instruction 4E534F4F 13-3A7(935)
        [Test]
        public void PPC750Dis_4E534F4F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E534F4F, "@@@");
        }

        // Unknown PowerPC X instruction 4E544552 13-2A9(681)
        [Test]
        public void PPC750Dis_4E544552()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E544552, "@@@");
        }

        // Unknown PowerPC X instruction 4E544630 13-318 (792)
        [Test]
        public void PPC750Dis_4E544630()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E544630, "@@@");
        }

        // Unknown PowerPC X instruction 4E544631 13-318 (792)
        [Test]
        public void PPC750Dis_4E544631()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E544631, "@@@");
        }

        // Unknown PowerPC X instruction 4E545200 13-100 (256)
        [Test]
        public void PPC750Dis_4E545200()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E545200, "@@@");
        }

        // Unknown PowerPC X instruction 4E54524F 13-127 (295)
        [Test]
        public void PPC750Dis_4E54524F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E54524F, "@@@");
        }

        // Unknown PowerPC X instruction 4E545D2E 13-297 (663)
        [Test]
        public void PPC750Dis_4E545D2E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E545D2E, "@@@");
        }

        // Unknown PowerPC X instruction 4E545F52 13-3A9(937)
        [Test]
        public void PPC750Dis_4E545F52()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E545F52, "@@@");
        }

        // Unknown PowerPC X instruction 4E553320 13-190 (400)
        [Test]
        public void PPC750Dis_4E553320()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E553320, "@@@");
        }

        // Unknown PowerPC X instruction 4E564559 13-2AC(684)
        [Test]
        public void PPC750Dis_4E564559()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E564559, "@@@");
        }

        // Unknown PowerPC X instruction 4E5AED51 13-2A8(680)
        [Test]
        public void PPC750Dis_4E5AED51()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E5AED51, "@@@");
        }

        // Unknown PowerPC X instruction 4E5F4249 13-124 (292)
        [Test]
        public void PPC750Dis_4E5F4249()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E5F4249, "@@@");
        }

        // Unknown PowerPC X instruction 4E5F504C 13-026 (38)
        [Test]
        public void PPC750Dis_4E5F504C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E5F504C, "@@@");
        }

        // Unknown PowerPC X instruction 4E6E6B28 13-194 (404)
        [Test]
        public void PPC750Dis_4E6E6B28()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E6E6B28, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F2046 13-023 (35)
        [Test]
        public void PPC750Dis_4E6F2046()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E6F2046, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F206F 13-037 (55)
        [Test]
        public void PPC750Dis_4E6F206F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E6F206F, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F6E2D 13-316 (790)
        [Test]
        public void PPC750Dis_4E6F6E2D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E6F6E2D, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F6E52 13-329 (809)
        [Test]
        public void PPC750Dis_4E6F6E52()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E6F6E52, "@@@");
        }

        // Unknown PowerPC X instruction 4E6F726D 13-136 (310)
        [Test]
        public void PPC750Dis_4E6F726D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E6F726D, "@@@");
        }

        // Unknown PowerPC X instruction 4E7610E8 13-074 (116)
        [Test]
        public void PPC750Dis_4E7610E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E7610E8, "@@@");
        }

        // Unknown PowerPC X instruction 4E7DAA4F 13-127 (295)
        [Test]
        public void PPC750Dis_4E7DAA4F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E7DAA4F, "@@@");
        }

        // Unknown PowerPC X instruction 4E96FD28 13-294 (660)
        [Test]
        public void PPC750Dis_4E96FD28()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E96FD28, "@@@");
        }

        // Unknown PowerPC X instruction 4E991995 13-0CA(202)
        [Test]
        public void PPC750Dis_4E991995()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4E991995, "@@@");
        }

        // Unknown PowerPC X instruction 4EAC546D 13-236 (566)
        [Test]
        public void PPC750Dis_4EAC546D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4EAC546D, "@@@");
        }

        // Unknown PowerPC X instruction 4EB6C4D1 13-268 (616)
        [Test]
        public void PPC750Dis_4EB6C4D1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4EB6C4D1, "@@@");
        }

        // Unknown PowerPC X instruction 4EC08723 13-391 (913)
        [Test]
        public void PPC750Dis_4EC08723()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4EC08723, "@@@");
        }

        // Unknown PowerPC X instruction 4EC0B831 13-018 (24)
        [Test]
        public void PPC750Dis_4EC0B831()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4EC0B831, "@@@");
        }

        // Unknown PowerPC X instruction 4ED09E2D 13-316 (790)
        [Test]
        public void PPC750Dis_4ED09E2D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4ED09E2D, "@@@");
        }

        // Unknown PowerPC X instruction 4ED66055 13-02A(42)
        [Test]
        public void PPC750Dis_4ED66055()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4ED66055, "@@@");
        }

        // Unknown PowerPC X instruction 4EEC4FD6 13-3EB(1003)
        [Test]
        public void PPC750Dis_4EEC4FD6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4EEC4FD6, "@@@");
        }

        // Unknown PowerPC X instruction 4F0F9915 13-08A(138)
        [Test]
        public void PPC750Dis_4F0F9915()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F0F9915, "@@@");
        }

        // Unknown PowerPC X instruction 4F202875 13-03A(58)
        [Test]
        public void PPC750Dis_4F202875()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F202875, "@@@");
        }

        // Unknown PowerPC X instruction 4F23937F 13-1BF(447)
        [Test]
        public void PPC750Dis_4F23937F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F23937F, "@@@");
        }

        // Unknown PowerPC X instruction 4F338754 13-3AA(938)
        [Test]
        public void PPC750Dis_4F338754()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F338754, "@@@");
        }

        // Unknown PowerPC X instruction 4F423A57 13-12B(299)
        [Test]
        public void PPC750Dis_4F423A57()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F423A57, "@@@");
        }

        // Unknown PowerPC X instruction 4F424F54 13-3AA(938)
        [Test]
        public void PPC750Dis_4F424F54()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F424F54, "@@@");
        }

        // Unknown PowerPC X instruction 4F42534F 13-1A7(423)
        [Test]
        public void PPC750Dis_4F42534F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F42534F, "@@@");
        }

        // Unknown PowerPC X instruction 4F434B00 13-180 (384)
        [Test]
        public void PPC750Dis_4F434B00()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F434B00, "@@@");
        }

        // Unknown PowerPC X instruction 4F442063 13-031 (49)
        [Test]
        public void PPC750Dis_4F442063()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F442063, "@@@");
        }

        // Unknown PowerPC X instruction 4F442074 13-03A(58)
        [Test]
        public void PPC750Dis_4F442074()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F442074, "@@@");
        }

        // Unknown PowerPC X instruction 4F44425A 13-12D (301)
        [Test]
        public void PPC750Dis_4F44425A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F44425A, "@@@");
        }

        // Unknown PowerPC X instruction 4F444445 13-222 (546)
        [Test]
        public void PPC750Dis_4F444445()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F444445, "@@@");
        }

        // Unknown PowerPC X instruction 4F4B455F 13-2AF(687)
        [Test]
        public void PPC750Dis_4F4B455F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4B455F, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C4445 13-222 (546)
        [Test]
        public void PPC750Dis_4F4C4445()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4C4445, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C4554 13-2AA(682)
        [Test]
        public void PPC750Dis_4F4C4554()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4C4554, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C4C45 13-222 (546)
        [Test]
        public void PPC750Dis_4F4C4C45()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4C4C45, "@@@");
        }

        // Unknown PowerPC X instruction 4F4C545F 13-22F (559)
        [Test]
        public void PPC750Dis_4F4C545F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4C545F, "@@@");
        }

        // Unknown PowerPC X instruction 4F4D4F4E 13-3A7(935)
        [Test]
        public void PPC750Dis_4F4D4F4E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4D4F4E, "@@@");
        }

        // Unknown PowerPC X instruction 4F4D5045 13-022 (34)
        [Test]
        public void PPC750Dis_4F4D5045()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4D5045, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E004E 13-027 (39)
        [Test]
        public void PPC750Dis_4F4E004E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E004E, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E0053 13-029 (41)
        [Test]
        public void PPC750Dis_4F4E0053()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E0053, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E2033 13-019 (25)
        [Test]
        public void PPC750Dis_4F4E2033()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E2033, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E2034 13-01A(26)
        [Test]
        public void PPC750Dis_4F4E2034()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E2034, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E2055 13-02A(42)
        [Test]
        public void PPC750Dis_4F4E2055()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E2055, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E4520 13-290 (656)
        [Test]
        public void PPC750Dis_4F4E4520()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E4520, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E4747 13-3A3(931)
        [Test]
        public void PPC750Dis_4F4E4747()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E4747, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E5645 13-322 (802)
        [Test]
        public void PPC750Dis_4F4E5645()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E5645, "@@@");
        }

        // Unknown PowerPC X instruction 4F4E5F5A 13-3AD(941)
        [Test]
        public void PPC750Dis_4F4E5F5A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4E5F5A, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F4400 13-200 (512)
        [Test]
        public void PPC750Dis_4F4F4400()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4F4400, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F4445 13-222 (546)
        [Test]
        public void PPC750Dis_4F4F4445()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4F4445, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F4B20 13-190 (400)
        [Test]
        public void PPC750Dis_4F4F4B20()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4F4B20, "@@@");
        }

        // Unknown PowerPC X instruction 4F4F5400 13-200 (512)
        [Test]
        public void PPC750Dis_4F4F5400()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F4F5400, "@@@");
        }

        // Unknown PowerPC X instruction 4F505F53 13-3A9(937)
        [Test]
        public void PPC750Dis_4F505F53()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F505F53, "@@@");
        }

        // Unknown PowerPC X instruction 4F505F57 13-3AB(939)
        [Test]
        public void PPC750Dis_4F505F57()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F505F57, "@@@");
        }

        // Unknown PowerPC X instruction 4F524500 13-280 (640)
        [Test]
        public void PPC750Dis_4F524500()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F524500, "@@@");
        }

        // Unknown PowerPC X instruction 4F52455F 13-2AF(687)
        [Test]
        public void PPC750Dis_4F52455F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F52455F, "@@@");
        }

        // Unknown PowerPC X instruction 4F524B53 13-1A9(425)
        [Test]
        public void PPC750Dis_4F524B53()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F524B53, "@@@");
        }

        // Unknown PowerPC X instruction 4F524D5F 13-2AF(687)
        [Test]
        public void PPC750Dis_4F524D5F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F524D5F, "@@@");
        }

        // Unknown PowerPC X instruction 4F525441 13-220 (544)
        [Test]
        public void PPC750Dis_4F525441()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F525441, "@@@");
        }

        // Unknown PowerPC X instruction 4F535320 13-190 (400)
        [Test]
        public void PPC750Dis_4F535320()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F535320, "@@@");
        }

        // Unknown PowerPC X instruction 4F535350 13-1A8(424)
        [Test]
        public void PPC750Dis_4F535350()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F535350, "@@@");
        }

        // Unknown PowerPC X instruction 4F542055 13-02A(42)
        [Test]
        public void PPC750Dis_4F542055()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F542055, "@@@");
        }

        // Unknown PowerPC X instruction 4F554400 13-200 (512)
        [Test]
        public void PPC750Dis_4F554400()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F554400, "@@@");
        }

        // Unknown PowerPC X instruction 4F554E44 13-322 (802)
        [Test]
        public void PPC750Dis_4F554E44()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F554E44, "@@@");
        }

        // Unknown PowerPC X instruction 4F57127D 13-13E (318)
        [Test]
        public void PPC750Dis_4F57127D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F57127D, "@@@");
        }

        // Unknown PowerPC X instruction 4F572055 13-02A(42)
        [Test]
        public void PPC750Dis_4F572055()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F572055, "@@@");
        }

        // Unknown PowerPC X instruction 4F592D8C 13-2C6(710)
        [Test]
        public void PPC750Dis_4F592D8C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F592D8C, "@@@");
        }

        // Unknown PowerPC X instruction 4F626A65 13-132 (306)
        [Test]
        public void PPC750Dis_4F626A65()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F626A65, "@@@");
        }

        // Unknown PowerPC X instruction 4F66426F 13-137 (311)
        [Test]
        public void PPC750Dis_4F66426F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F66426F, "@@@");
        }

        // Unknown PowerPC X instruction 4F66576F 13-3B7(951)
        [Test]
        public void PPC750Dis_4F66576F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F66576F, "@@@");
        }

        // Unknown PowerPC X instruction 4F666600 13-300 (768)
        [Test]
        public void PPC750Dis_4F666600()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F666600, "@@@");
        }

        // Unknown PowerPC X instruction 4F666673 13-339 (825)
        [Test]
        public void PPC750Dis_4F666673()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F666673, "@@@");
        }

        // Unknown PowerPC X instruction 4F6E0053 13-029 (41)
        [Test]
        public void PPC750Dis_4F6E0053()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F6E0053, "@@@");
        }

        // Unknown PowerPC X instruction 4F6E2053 13-029 (41)
        [Test]
        public void PPC750Dis_4F6E2053()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F6E2053, "@@@");
        }

        // Unknown PowerPC X instruction 4F707469 13-234 (564)
        [Test]
        public void PPC750Dis_4F707469()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F707469, "@@@");
        }

        // Unknown PowerPC X instruction 4F756368 13-1B4(436)
        [Test]
        public void PPC750Dis_4F756368()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F756368, "@@@");
        }

        // Unknown PowerPC X instruction 4F757400 13-200 (512)
        [Test]
        public void PPC750Dis_4F757400()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F757400, "@@@");
        }

        // Unknown PowerPC X instruction 4F75744F 13-227 (551)
        [Test]
        public void PPC750Dis_4F75744F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F75744F, "@@@");
        }

        // Unknown PowerPC X instruction 4F87DC71 13-238 (568)
        [Test]
        public void PPC750Dis_4F87DC71()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F87DC71, "@@@");
        }

        // Unknown PowerPC X instruction 4F8B6900 13-080 (128)
        [Test]
        public void PPC750Dis_4F8B6900()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F8B6900, "@@@");
        }

        // Unknown PowerPC X instruction 4F9C1BD2 13-1E9 (489)
        [Test]
        public void PPC750Dis_4F9C1BD2()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4F9C1BD2, "@@@");
        }

        // Unknown PowerPC X instruction 4FB921D9 13-0EC(236)
        [Test]
        public void PPC750Dis_4FB921D9()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4FB921D9, "@@@");
        }

        // Unknown PowerPC X instruction 4FE3D097 13-04B(75)
        [Test]
        public void PPC750Dis_4FE3D097()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4FE3D097, "@@@");
        }

        // Unknown PowerPC X instruction 4FF594AA 13-255 (597)
        [Test]
        public void PPC750Dis_4FF594AA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4FF594AA, "@@@");
        }

        // Unknown PowerPC X instruction 4FF6EFEB 13-3F5 (1013)
        [Test]
        public void PPC750Dis_4FF6EFEB()
        {
            Given_ProcessorModel_750();
            AssertCode(0x4FF6EFEB, "@@@");
        }

        // Unknown PowerPC X instruction 7C004A00 1F-100 (256)
        [Test]
        public void PPC750Dis_7C004A00()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C004A00, "@@@");
        }

        // Unknown PowerPC X instruction 7C004A2A 1F-115 (277)
        [Test]
        public void PPC750Dis_7C004A2A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C004A2A, "@@@");
        }

        // Unknown PowerPC X instruction 7C02014D 1F-0A6(166)
        [Test]
        public void PPC750Dis_7C02014D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C02014D, "@@@");
        }

        // Unknown PowerPC X instruction 7C025374 1F-1BA(442)
        [Test]
        public void PPC750Dis_7C025374()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C025374, "@@@");
        }

        // Unknown PowerPC X instruction 7C0B7C15 1F-20A(522)
        [Test]
        public void PPC750Dis_7C0B7C15()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C0B7C15, "@@@");
        }

        // Unknown PowerPC X instruction 7C0FEF59 1F-3AC(940)
        [Test]
        public void PPC750Dis_7C0FEF59()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C0FEF59, "@@@");
        }

        // Unknown PowerPC X instruction 7C1E7C27 1F-213 (531)
        [Test]
        public void PPC750Dis_7C1E7C27()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C1E7C27, "@@@");
        }

        // Unknown PowerPC X instruction 7C257300 1F-180 (384)
        [Test]
        public void PPC750Dis_7C257300()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C257300, "@@@");
        }

        // Unknown PowerPC X instruction 7C307C39 1F-21C(540)
        [Test]
        public void PPC750Dis_7C307C39()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C307C39, "@@@");
        }

        // Unknown PowerPC X instruction 7C3B01BA 1F-0DD(221)
        [Test]
        public void PPC750Dis_7C3B01BA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C3B01BA, "@@@");
        }

        // Unknown PowerPC X instruction 7C3DD424 1F-212 (530)
        [Test]
        public void PPC750Dis_7C3DD424()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C3DD424, "@@@");
        }

        // Unknown PowerPC X instruction 7C426F73 1F-3B9(953)
        [Test]
        public void PPC750Dis_7C426F73()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C426F73, "@@@");
        }

        // Unknown PowerPC X instruction 7C427C4B 1F-225 (549)
        [Test]
        public void PPC750Dis_7C427C4B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C427C4B, "@@@");
        }

        // Unknown PowerPC X instruction 7C43616D 1F-0B6 (182)
        [Test]
        public void PPC750Dis_7C43616D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C43616D, "@@@");
        }

        // Unknown PowerPC X instruction 7C486F6F 1F-3B7(951)
        [Test]
        public void PPC750Dis_7C486F6F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C486F6F, "@@@");
        }

        // Unknown PowerPC X instruction 7C486F72 1F-3B9(953)
        [Test]
        public void PPC750Dis_7C486F72()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C486F72, "@@@");
        }

        // Unknown PowerPC X instruction 7C546170 1F-0B8 (184)
        [Test]
        public void PPC750Dis_7C546170()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C546170, "@@@");
        }

        // Unknown PowerPC X instruction 7C546869 1F-034 (52)
        [Test]
        public void PPC750Dis_7C546869()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C546869, "@@@");
        }

        // Unknown PowerPC X instruction 7C547572 1F-2B9(697)
        [Test]
        public void PPC750Dis_7C547572()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C547572, "@@@");
        }

        // Unknown PowerPC X instruction 7C557C5E 1F-22F (559)
        [Test]
        public void PPC750Dis_7C557C5E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C557C5E, "@@@");
        }

        // Unknown PowerPC X instruction 7C566572 1F-2B9(697)
        [Test]
        public void PPC750Dis_7C566572()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C566572, "@@@");
        }

        // Unknown PowerPC X instruction 7C677C70 1F-238 (568)
        [Test]
        public void PPC750Dis_7C677C70()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C677C70, "@@@");
        }

        // Unknown PowerPC X instruction 7C6F0890 1F-048 (72)
        [Test]
        public void PPC750Dis_7C6F0890()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C6F0890, "@@@");
        }

        // Unknown PowerPC X instruction 7C706F73 1F-3B9(953)
        [Test]
        public void PPC750Dis_7C706F73()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C706F73, "@@@");
        }

        // Unknown PowerPC X instruction 7C766172 1F-0B9 (185)
        [Test]
        public void PPC750Dis_7C766172()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C766172, "@@@");
        }

        // Unknown PowerPC X instruction 7C78004C 1F-026 (38)
        [Test]
        public void PPC750Dis_7C78004C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C78004C, "@@@");
        }

        // Unknown PowerPC X instruction 7C79004C 1F-026 (38)
        [Test]
        public void PPC750Dis_7C79004C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C79004C, "@@@");
        }

        // Unknown PowerPC X instruction 7C797C82 1F-241 (577)
        [Test]
        public void PPC750Dis_7C797C82()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C797C82, "@@@");
        }

        // Unknown PowerPC X instruction 7C7A4E50 1F-328 (808)
        [Test]
        public void PPC750Dis_7C7A4E50()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C7A4E50, "@@@");
        }

        // Unknown PowerPC X instruction 7C7D7E7F 1F-33F (831)
        [Test]
        public void PPC750Dis_7C7D7E7F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C7D7E7F, "@@@");
        }

        // Unknown PowerPC X instruction 7C8274C1 1F-260 (608)
        [Test]
        public void PPC750Dis_7C8274C1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C8274C1, "@@@");
        }

        // Unknown PowerPC X instruction 7C8C7C95 1F-24A(586)
        [Test]
        public void PPC750Dis_7C8C7C95()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C8C7C95, "@@@");
        }

        // Unknown PowerPC X instruction 7C8CD8A2 1F-051 (81)
        [Test]
        public void PPC750Dis_7C8CD8A2()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C8CD8A2, "@@@");
        }

        // Unknown PowerPC X instruction 7C996F00 1F-380 (896)
        [Test]
        public void PPC750Dis_7C996F00()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C996F00, "@@@");
        }

        // Unknown PowerPC X instruction 7C9E7CA7 1F-253 (595)
        [Test]
        public void PPC750Dis_7C9E7CA7()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C9E7CA7, "@@@");
        }

        // Unknown PowerPC X instruction 7C9F4AD1 1F-168 (360)
        [Test]
        public void PPC750Dis_7C9F4AD1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7C9F4AD1, "@@@");
        }

        // Unknown PowerPC X instruction 7CB07CBA 1F-25D (605)
        [Test]
        public void PPC750Dis_7CB07CBA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CB07CBA, "@@@");
        }

        // Unknown PowerPC X instruction 7CC37CCC 1F-266 (614)
        [Test]
        public void PPC750Dis_7CC37CCC()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CC37CCC, "@@@");
        }

        // Unknown PowerPC X instruction 7CD57CDE 1F-26F (623)
        [Test]
        public void PPC750Dis_7CD57CDE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CD57CDE, "@@@");
        }

        // Unknown PowerPC X instruction 7CD846DF 1F-36F (879)
        [Test]
        public void PPC750Dis_7CD846DF()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CD846DF, "@@@");
        }

        // Unknown PowerPC X instruction 7CDF69CD 1F-0E6 (230)
        [Test]
        public void PPC750Dis_7CDF69CD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CDF69CD, "@@@");
        }

        // Unknown PowerPC X instruction 7CE87CF1 1F-278 (632)
        [Test]
        public void PPC750Dis_7CE87CF1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CE87CF1, "@@@");
        }

        // Unknown PowerPC X instruction 7CF8A16A 1F-0B5 (181)
        [Test]
        public void PPC750Dis_7CF8A16A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CF8A16A, "@@@");
        }

        // Unknown PowerPC X instruction 7CF97AF3 1F-179 (377)
        [Test]
        public void PPC750Dis_7CF97AF3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CF97AF3, "@@@");
        }

        // Unknown PowerPC X instruction 7CFA7D03 1F-281 (641)
        [Test]
        public void PPC750Dis_7CFA7D03()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CFA7D03, "@@@");
        }

        // Unknown PowerPC X instruction 7CFCAA55 1F-12A(298)
        [Test]
        public void PPC750Dis_7CFCAA55()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7CFCAA55, "@@@");
        }

        // Unknown PowerPC X instruction 7D001CDD 1F-26E (622)
        [Test]
        public void PPC750Dis_7D001CDD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D001CDD, "@@@");
        }

        // Unknown PowerPC X instruction 7D007B69 1F-1B4(436)
        [Test]
        public void PPC750Dis_7D007B69()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D007B69, "@@@");
        }

        // Unknown PowerPC X instruction 7D0C7D16 1F-28B(651)
        [Test]
        public void PPC750Dis_7D0C7D16()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D0C7D16, "@@@");
        }

        // Unknown PowerPC X instruction 7D167E88 1F-344 (836)
        [Test]
        public void PPC750Dis_7D167E88()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D167E88, "@@@");
        }

        // Unknown PowerPC X instruction 7D1DEFFD 1F-3FE(1022)
        [Test]
        public void PPC750Dis_7D1DEFFD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D1DEFFD, "@@@");
        }

        // Unknown PowerPC X instruction 7D1E6187 1F-0C3(195)
        [Test]
        public void PPC750Dis_7D1E6187()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D1E6187, "@@@");
        }

        // Unknown PowerPC X instruction 7D1F7D28 1F-294 (660)
        [Test]
        public void PPC750Dis_7D1F7D28()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D1F7D28, "@@@");
        }

        // Unknown PowerPC X instruction 7D218D40 1F-2A0(672)
        [Test]
        public void PPC750Dis_7D218D40()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D218D40, "@@@");
        }

        // Unknown PowerPC X instruction 7D22DB16 1F-18B(395)
        [Test]
        public void PPC750Dis_7D22DB16()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D22DB16, "@@@");
        }

        // Unknown PowerPC X instruction 7D25737B 1F-1BD(445)
        [Test]
        public void PPC750Dis_7D25737B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D25737B, "@@@");
        }

        // Unknown PowerPC X instruction 7D291B5F 1F-1AF(431)
        [Test]
        public void PPC750Dis_7D291B5F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D291B5F, "@@@");
        }

        // Unknown PowerPC X instruction 7D2E9B3D 1F-19E (414)
        [Test]
        public void PPC750Dis_7D2E9B3D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D2E9B3D, "@@@");
        }

        // Unknown PowerPC X instruction 7D314E3B 1F-31D (797)
        [Test]
        public void PPC750Dis_7D314E3B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D314E3B, "@@@");
        }

        // Unknown PowerPC X instruction 7D317D3A 1F-29D (669)
        [Test]
        public void PPC750Dis_7D317D3A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D317D3A, "@@@");
        }

        // Unknown PowerPC X instruction 7D3C75FD 1F-2FE(766)
        [Test]
        public void PPC750Dis_7D3C75FD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D3C75FD, "@@@");
        }

        // Unknown PowerPC X instruction 7D447D4D 1F-2A6(678)
        [Test]
        public void PPC750Dis_7D447D4D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D447D4D, "@@@");
        }

        // Unknown PowerPC X instruction 7D4638BE 1F-05F (95)
        [Test]
        public void PPC750Dis_7D4638BE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D4638BE, "@@@");
        }

        // Unknown PowerPC X instruction 7D55756C 1F-2B6(694)
        [Test]
        public void PPC750Dis_7D55756C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D55756C, "@@@");
        }

        // Unknown PowerPC X instruction 7D567D5F 1F-2AF(687)
        [Test]
        public void PPC750Dis_7D567D5F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D567D5F, "@@@");
        }

        // Unknown PowerPC X instruction 7D57ACA2 1F-251 (593)
        [Test]
        public void PPC750Dis_7D57ACA2()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D57ACA2, "@@@");
        }

        // Unknown PowerPC X instruction 7D697D72 1F-2B9(697)
        [Test]
        public void PPC750Dis_7D697D72()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D697D72, "@@@");
        }

        // Unknown PowerPC X instruction 7D78EBDD 1F-1EE(494)
        [Test]
        public void PPC750Dis_7D78EBDD()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D78EBDD, "@@@");
        }

        // Unknown PowerPC X instruction 7D7B7D84 1F-2C2(706)
        [Test]
        public void PPC750Dis_7D7B7D84()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D7B7D84, "@@@");
        }

        // Unknown PowerPC X instruction 7D8E7D97 1F-2CB(715)
        [Test]
        public void PPC750Dis_7D8E7D97()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7D8E7D97, "@@@");
        }

        // Unknown PowerPC X instruction 7DA07DA9 1F-2D4 (724)
        [Test]
        public void PPC750Dis_7DA07DA9()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DA07DA9, "@@@");
        }

        // Unknown PowerPC X instruction 7DAC1CEB 1F-275 (629)
        [Test]
        public void PPC750Dis_7DAC1CEB()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DAC1CEB, "@@@");
        }

        // Unknown PowerPC X instruction 7DADA77F 1F-3BF(959)
        [Test]
        public void PPC750Dis_7DADA77F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DADA77F, "@@@");
        }

        // Unknown PowerPC X instruction 7DAEBAF8 1F-17C(380)
        [Test]
        public void PPC750Dis_7DAEBAF8()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DAEBAF8, "@@@");
        }

        // Unknown PowerPC X instruction 7DB37DBC 1F-2DE(734)
        [Test]
        public void PPC750Dis_7DB37DBC()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DB37DBC, "@@@");
        }

        // Unknown PowerPC X instruction 7DB4A45E 1F-22F (559)
        [Test]
        public void PPC750Dis_7DB4A45E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DB4A45E, "@@@");
        }

        // Unknown PowerPC X instruction 7DBBFFFC 1F-3FE(1022)
        [Test]
        public void PPC750Dis_7DBBFFFC()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DBBFFFC, "@@@");
        }

        // Unknown PowerPC X instruction 7DC57DCE 1F-2E7 (743)
        [Test]
        public void PPC750Dis_7DC57DCE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DC57DCE, "@@@");
        }

        // Unknown PowerPC X instruction 7DCBDC46 1F-223 (547)
        [Test]
        public void PPC750Dis_7DCBDC46()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DCBDC46, "@@@");
        }

        // Unknown PowerPC X instruction 7DD7F79F 1F-3CF(975)
        [Test]
        public void PPC750Dis_7DD7F79F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DD7F79F, "@@@");
        }

        // Unknown PowerPC X instruction 7DD87DE1 1F-2F0 (752)
        [Test]
        public void PPC750Dis_7DD87DE1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DD87DE1, "@@@");
        }

        // Unknown PowerPC X instruction 7DD92685 1F-342 (834)
        [Test]
        public void PPC750Dis_7DD92685()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DD92685, "@@@");
        }

        // Unknown PowerPC X instruction 7DDBA387 1F-1C3(451)
        [Test]
        public void PPC750Dis_7DDBA387()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DDBA387, "@@@");
        }

        // Unknown PowerPC X instruction 7DDC6E1F 1F-30F (783)
        [Test]
        public void PPC750Dis_7DDC6E1F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DDC6E1F, "@@@");
        }

        // Unknown PowerPC X instruction 7DDF1B84 1F-1C2(450)
        [Test]
        public void PPC750Dis_7DDF1B84()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DDF1B84, "@@@");
        }

        // Unknown PowerPC X instruction 7DEA7DF4 1F-2FA(762)
        [Test]
        public void PPC750Dis_7DEA7DF4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DEA7DF4, "@@@");
        }

        // Unknown PowerPC X instruction 7DEE03D9 1F-1EC(492)
        [Test]
        public void PPC750Dis_7DEE03D9()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DEE03D9, "@@@");
        }

        // Unknown PowerPC X instruction 7DEF7B30 1F-198 (408)
        [Test]
        public void PPC750Dis_7DEF7B30()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DEF7B30, "@@@");
        }

        // Unknown PowerPC X instruction 7DF50207 1F-103 (259)
        [Test]
        public void PPC750Dis_7DF50207()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DF50207, "@@@");
        }

        // Unknown PowerPC X instruction 7DFA4EEE 1F-377 (887)
        [Test]
        public void PPC750Dis_7DFA4EEE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DFA4EEE, "@@@");
        }

        // Unknown PowerPC X instruction 7DFD7E06 1F-303 (771)
        [Test]
        public void PPC750Dis_7DFD7E06()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7DFD7E06, "@@@");
        }

        // Unknown PowerPC X instruction 7E0001A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7E0001A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E0001A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E0004A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7E0004A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E0004A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E0227CF 1F-3E7 (999)
        [Test]
        public void PPC750Dis_7E0227CF()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E0227CF, "@@@");
        }

        // Unknown PowerPC X instruction 7E0F7E19 1F-30C(780)
        [Test]
        public void PPC750Dis_7E0F7E19()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E0F7E19, "@@@");
        }

        // Unknown PowerPC X instruction 7E1306F0 1F-378 (888)
        [Test]
        public void PPC750Dis_7E1306F0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E1306F0, "@@@");
        }

        // Unknown PowerPC X instruction 7E17D383 1F-1C1(449)
        [Test]
        public void PPC750Dis_7E17D383()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E17D383, "@@@");
        }

        // Unknown PowerPC X instruction 7E2101A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7E2101A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E2101A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E2104A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7E2104A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E2104A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E227E2B 1F-315 (789)
        [Test]
        public void PPC750Dis_7E227E2B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E227E2B, "@@@");
        }

        // Unknown PowerPC X instruction 7E270CF2 1F-279 (633)
        [Test]
        public void PPC750Dis_7E270CF2()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E270CF2, "@@@");
        }

        // Unknown PowerPC X instruction 7E2B3219 1F-10C(268)
        [Test]
        public void PPC750Dis_7E2B3219()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E2B3219, "@@@");
        }

        // Unknown PowerPC X instruction 7E357E3E 1F-31F (799)
        [Test]
        public void PPC750Dis_7E357E3E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E357E3E, "@@@");
        }

        // Unknown PowerPC X instruction 7E4201A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7E4201A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E4201A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E4204A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7E4204A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E4204A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E477E51 1F-328 (808)
        [Test]
        public void PPC750Dis_7E477E51()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E477E51, "@@@");
        }

        // Unknown PowerPC X instruction 7E525D9E 1F-2CF(719)
        [Test]
        public void PPC750Dis_7E525D9E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E525D9E, "@@@");
        }

        // Unknown PowerPC X instruction 7E54C6E4 1F-372 (882)
        [Test]
        public void PPC750Dis_7E54C6E4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E54C6E4, "@@@");
        }

        // Unknown PowerPC X instruction 7E59EBDE 1F-1EF (495)
        [Test]
        public void PPC750Dis_7E59EBDE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E59EBDE, "@@@");
        }

        // Unknown PowerPC X instruction 7E5A7E63 1F-331 (817)
        [Test]
        public void PPC750Dis_7E5A7E63()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E5A7E63, "@@@");
        }

        // Unknown PowerPC X instruction 7E6301A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7E6301A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E6301A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E6304A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7E6304A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E6304A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E77854A 1F-2A5(677)
        [Test]
        public void PPC750Dis_7E77854A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E77854A, "@@@");
        }

        // Unknown PowerPC X instruction 7E7CEFDF 1F-3EF (1007)
        [Test]
        public void PPC750Dis_7E7CEFDF()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E7CEFDF, "@@@");
        }

        // Unknown PowerPC X instruction 7E7D874E 1F-3A7(935)
        [Test]
        public void PPC750Dis_7E7D874E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E7D874E, "@@@");
        }

        // Unknown PowerPC X instruction 7E7F7E88 1F-344 (836)
        [Test]
        public void PPC750Dis_7E7F7E88()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E7F7E88, "@@@");
        }

        // Unknown PowerPC X instruction 7E8401A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7E8401A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E8401A4, "@@@");
        }

        // Unknown PowerPC X instruction 7E8404A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7E8404A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E8404A6, "@@@");
        }

        // Unknown PowerPC X instruction 7E8E6656 1F-32B(811)
        [Test]
        public void PPC750Dis_7E8E6656()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E8E6656, "@@@");
        }

        // Unknown PowerPC X instruction 7E927E9B 1F-34D (845)
        [Test]
        public void PPC750Dis_7E927E9B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E927E9B, "@@@");
        }

        // Unknown PowerPC X instruction 7E967699 1F-34C(844)
        [Test]
        public void PPC750Dis_7E967699()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E967699, "@@@");
        }

        // Unknown PowerPC X instruction 7E9C1D16 1F-28B(651)
        [Test]
        public void PPC750Dis_7E9C1D16()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7E9C1D16, "@@@");
        }

        // Unknown PowerPC X instruction 7EA47EAE 1F-357 (855)
        [Test]
        public void PPC750Dis_7EA47EAE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EA47EAE, "@@@");
        }

        // Unknown PowerPC X instruction 7EA501A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7EA501A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EA501A4, "@@@");
        }

        // Unknown PowerPC X instruction 7EA504A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7EA504A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EA504A6, "@@@");
        }

        // Unknown PowerPC X instruction 7EB07297 1F-14B(331)
        [Test]
        public void PPC750Dis_7EB07297()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EB07297, "@@@");
        }

        // Unknown PowerPC X instruction 7EB77EC0 1F-360 (864)
        [Test]
        public void PPC750Dis_7EB77EC0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EB77EC0, "@@@");
        }

        // Unknown PowerPC X instruction 7EBCDAF5 1F-17A(378)
        [Test]
        public void PPC750Dis_7EBCDAF5()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EBCDAF5, "@@@");
        }

        // Unknown PowerPC X instruction 7EC601A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7EC601A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EC601A4, "@@@");
        }

        // Unknown PowerPC X instruction 7EC604A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7EC604A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EC604A6, "@@@");
        }

        // Unknown PowerPC X instruction 7ECA7ED3 1F-369 (873)
        [Test]
        public void PPC750Dis_7ECA7ED3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7ECA7ED3, "@@@");
        }

        // Unknown PowerPC X instruction 7EDC7EE6 1F-373 (883)
        [Test]
        public void PPC750Dis_7EDC7EE6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EDC7EE6, "@@@");
        }

        // Unknown PowerPC X instruction 7EE701A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7EE701A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EE701A4, "@@@");
        }

        // Unknown PowerPC X instruction 7EE704A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7EE704A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EE704A6, "@@@");
        }

        // Unknown PowerPC X instruction 7EEF7EF8 1F-37C(892)
        [Test]
        public void PPC750Dis_7EEF7EF8()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EEF7EF8, "@@@");
        }

        // Unknown PowerPC X instruction 7EF45BA5 1F-1D2 (466)
        [Test]
        public void PPC750Dis_7EF45BA5()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EF45BA5, "@@@");
        }

        // Unknown PowerPC X instruction 7EFBFA9F 1F-14F (335)
        [Test]
        public void PPC750Dis_7EFBFA9F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EFBFA9F, "@@@");
        }

        // Unknown PowerPC X instruction 7EFCDAC8 1F-164 (356)
        [Test]
        public void PPC750Dis_7EFCDAC8()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7EFCDAC8, "@@@");
        }

        // Unknown PowerPC X instruction 7F027F0B 1F-385 (901)
        [Test]
        public void PPC750Dis_7F027F0B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F027F0B, "@@@");
        }

        // Unknown PowerPC X instruction 7F0801A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7F0801A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F0801A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F0804A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7F0804A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F0804A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F157F1E 1F-38F (911)
        [Test]
        public void PPC750Dis_7F157F1E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F157F1E, "@@@");
        }

        // Unknown PowerPC X instruction 7F18B3EB 1F-1F5 (501)
        [Test]
        public void PPC750Dis_7F18B3EB()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F18B3EB, "@@@");
        }

        // Unknown PowerPC X instruction 7F277F31 1F-398 (920)
        [Test]
        public void PPC750Dis_7F277F31()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F277F31, "@@@");
        }

        // Unknown PowerPC X instruction 7F2901A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7F2901A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F2901A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F2904A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7F2904A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F2904A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F2BB4A1 1F-250 (592)
        [Test]
        public void PPC750Dis_7F2BB4A1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F2BB4A1, "@@@");
        }

        // Unknown PowerPC X instruction 7F3A7F43 1F-3A1(929)
        [Test]
        public void PPC750Dis_7F3A7F43()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F3A7F43, "@@@");
        }

        // Unknown PowerPC X instruction 7F4A01A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7F4A01A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F4A01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F4A04A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7F4A04A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F4A04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F4D7F56 1F-3AB(939)
        [Test]
        public void PPC750Dis_7F4D7F56()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F4D7F56, "@@@");
        }

        // Unknown PowerPC X instruction 7F577755 1F-3AA(938)
        [Test]
        public void PPC750Dis_7F577755()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F577755, "@@@");
        }

        // Unknown PowerPC X instruction 7F607F69 1F-3B4(948)
        [Test]
        public void PPC750Dis_7F607F69()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F607F69, "@@@");
        }

        // Unknown PowerPC X instruction 7F6B01A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7F6B01A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F6B01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F6B04A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7F6B04A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F6B04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F727F7C 1F-3BE(958)
        [Test]
        public void PPC750Dis_7F727F7C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F727F7C, "@@@");
        }

        // Unknown PowerPC X instruction 7F7A7996 1F-0CB(203)
        [Test]
        public void PPC750Dis_7F7A7996()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F7A7996, "@@@");
        }

        // Unknown PowerPC X instruction 7F857F8F 1F-3C7(967)
        [Test]
        public void PPC750Dis_7F857F8F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F857F8F, "@@@");
        }

        // Unknown PowerPC X instruction 7F8C01A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7F8C01A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F8C01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7F8C04A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7F8C04A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F8C04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7F987FA1 1F-3D0 (976)
        [Test]
        public void PPC750Dis_7F987FA1()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7F987FA1, "@@@");
        }

        // Unknown PowerPC X instruction 7FAD01A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7FAD01A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FAD01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7FAD04A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7FAD04A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FAD04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7FAFA1FE 1F-0FF(255)
        [Test]
        public void PPC750Dis_7FAFA1FE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FAFA1FE, "@@@");
        }

        // Unknown PowerPC X instruction 7FBE7FC7 1F-3E3 (995)
        [Test]
        public void PPC750Dis_7FBE7FC7()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FBE7FC7, "@@@");
        }

        // Unknown PowerPC X instruction 7FCE01A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7FCE01A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FCE01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7FCE04A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7FCE04A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FCE04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7FD07FDA 1F-3ED (1005)
        [Test]
        public void PPC750Dis_7FD07FDA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FD07FDA, "@@@");
        }

        // Unknown PowerPC X instruction 7FDDF80F 1F-007 (7)
        [Test]
        public void PPC750Dis_7FDDF80F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FDDF80F, "@@@");
        }

        // Unknown PowerPC X instruction 7FEF01A4 1F-0D2 (210)
        [Test]
        public void PPC750Dis_7FEF01A4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FEF01A4, "@@@");
        }

        // Unknown PowerPC X instruction 7FEF04A6 1F-253 (595)
        [Test]
        public void PPC750Dis_7FEF04A6()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FEF04A6, "@@@");
        }

        // Unknown PowerPC X instruction 7FF03AA9 1F-154 (340)
        [Test]
        public void PPC750Dis_7FF03AA9()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FF03AA9, "@@@");
        }

        // Unknown PowerPC X instruction 7FF5A686 1F-343 (835)
        [Test]
        public void PPC750Dis_7FF5A686()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FF5A686, "@@@");
        }

        // Unknown PowerPC X instruction 7FFF817B 1F-0BD(189)
        [Test]
        public void PPC750Dis_7FFF817B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x7FFF817B, "@@@");
        }
    } 
}
