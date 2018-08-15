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
            AssertCode(0x13E0180E, "psq_stx\tf31,r0,r3,00,00");
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


        // Unknown PowerPC VMX instruction 180E8A9A 06-029 (41)
        [Test]
        public void PPC750Dis_180E8A9A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x180E8A9A, "@@@");
        }

        // Unknown PowerPC VMX instruction 1813EA1B 06-021 (33)
        [Test]
        public void PPC750Dis_1813EA1B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1813EA1B, "@@@");
        }

        // Unknown PowerPC VMX instruction 1815B554 06-055 (85)
        [Test]
        public void PPC750Dis_1815B554()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1815B554, "@@@");
        }

        // Unknown PowerPC VMX instruction 18181818 06-001 (1)
        [Test]
        public void PPC750Dis_18181818()
        {
            Given_ProcessorModel_750();
            AssertCode(0x18181818, "@@@");
        }


        // Unknown PowerPC VMX instruction 181D7012 06-001 (1)
        [Test]
        public void PPC750Dis_181D7012()
        {
            Given_ProcessorModel_750();
            AssertCode(0x181D7012, "@@@");
        }

        // Unknown PowerPC VMX instruction 185F1B1F 06-031 (49)
        [Test]
        public void PPC750Dis_185F1B1F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x185F1B1F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1869E45D 06-045 (69)
        [Test]
        public void PPC750Dis_1869E45D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1869E45D, "@@@");
        }

        // Unknown PowerPC VMX instruction 189E181B 06-001 (1)
        [Test]
        public void PPC750Dis_189E181B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x189E181B, "@@@");
        }

        // Unknown PowerPC VMX instruction 18A05DBB 06-05B(91)
        [Test]
        public void PPC750Dis_18A05DBB()
        {
            Given_ProcessorModel_750();
            AssertCode(0x18A05DBB, "@@@");
        }

        // Unknown PowerPC VMX instruction 18C735B4 06-05B(91)
        [Test]
        public void PPC750Dis_18C735B4()
        {
            Given_ProcessorModel_750();
            AssertCode(0x18C735B4, "@@@");
        }

        // Unknown PowerPC VMX instruction 18F5193F 06-013 (19)
        [Test]
        public void PPC750Dis_18F5193F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x18F5193F, "@@@");
        }

        // Unknown PowerPC VMX instruction 190031B3 06-01B(27)
        [Test]
        public void PPC750Dis_190031B3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x190031B3, "@@@");
        }

        // Unknown PowerPC VMX instruction 19189059 06-005 (5)
        [Test]
        public void PPC750Dis_19189059()
        {
            Given_ProcessorModel_750();
            AssertCode(0x19189059, "@@@");
        }

        // Unknown PowerPC VMX instruction 191AF858 06-005 (5)
        [Test]
        public void PPC750Dis_191AF858()
        {
            Given_ProcessorModel_750();
            AssertCode(0x191AF858, "@@@");
        }

        // Unknown PowerPC VMX instruction 191F191D 06-011 (17)
        [Test]
        public void PPC750Dis_191F191D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x191F191D, "@@@");
        }

        // Unknown PowerPC VMX instruction 1923017D 06-017 (23)
        [Test]
        public void PPC750Dis_1923017D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1923017D, "@@@");
        }

        // Unknown PowerPC VMX instruction 193A1938 06-013 (19)
        [Test]
        public void PPC750Dis_193A1938()
        {
            Given_ProcessorModel_750();
            AssertCode(0x193A1938, "@@@");
        }

        // Unknown PowerPC VMX instruction 193CB179 06-017 (23)
        [Test]
        public void PPC750Dis_193CB179()
        {
            Given_ProcessorModel_750();
            AssertCode(0x193CB179, "@@@");
        }

        // Unknown PowerPC VMX instruction 193E193C 06-013 (19)
        [Test]
        public void PPC750Dis_193E193C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x193E193C, "@@@");
        }

        // Unknown PowerPC VMX instruction 19487FC0 06-07C(124)
        [Test]
        public void PPC750Dis_19487FC0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x19487FC0, "@@@");
        }

        // Unknown PowerPC VMX instruction 194EE0B3 06-00B(11)
        [Test]
        public void PPC750Dis_194EE0B3()
        {
            Given_ProcessorModel_750();
            AssertCode(0x194EE0B3, "@@@");
        }

        // Unknown PowerPC VMX instruction 1954AC7D 06-047 (71)
        [Test]
        public void PPC750Dis_1954AC7D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1954AC7D, "@@@");
        }

        // Unknown PowerPC VMX instruction 195B1959 06-015 (21)
        [Test]
        public void PPC750Dis_195B1959()
        {
            Given_ProcessorModel_750();
            AssertCode(0x195B1959, "@@@");
        }

        // Unknown PowerPC VMX instruction 195F195F 06-015 (21)
        [Test]
        public void PPC750Dis_195F195F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x195F195F, "@@@");
        }

        // Unknown PowerPC VMX instruction 195F1B1F 06-031 (49)
        [Test]
        public void PPC750Dis_195F1B1F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x195F1B1F, "@@@");
        }

        // Unknown PowerPC VMX instruction 197A1978 06-017 (23)
        [Test]
        public void PPC750Dis_197A1978()
        {
            Given_ProcessorModel_750();
            AssertCode(0x197A1978, "@@@");
        }

        // Unknown PowerPC VMX instruction 197C1B1A 06-031 (49)
        [Test]
        public void PPC750Dis_197C1B1A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x197C1B1A, "@@@");
        }

        // Unknown PowerPC VMX instruction 197E1B1A 06-031 (49)
        [Test]
        public void PPC750Dis_197E1B1A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x197E1B1A, "@@@");
        }

        // Unknown PowerPC VMX instruction 197F9930 06-013 (19)
        [Test]
        public void PPC750Dis_197F9930()
        {
            Given_ProcessorModel_750();
            AssertCode(0x197F9930, "@@@");
        }

        // Unknown PowerPC VMX instruction 19938CB0 06-04B(75)
        [Test]
        public void PPC750Dis_19938CB0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x19938CB0, "@@@");
        }

        // Unknown PowerPC VMX instruction 19F2578B 06-078 (120)
        [Test]
        public void PPC750Dis_19F2578B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x19F2578B, "@@@");
        }

        // Unknown PowerPC VMX instruction 1A1B1213 06-021 (33)
        [Test]
        public void PPC750Dis_1A1B1213()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1A1B1213, "@@@");
        }

        // Unknown PowerPC VMX instruction 1A99F016 06-001 (1)
        [Test]
        public void PPC750Dis_1A99F016()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1A99F016, "@@@");
        }

        // Unknown PowerPC VMX instruction 1AAF8F8A 06-078 (120)
        [Test]
        public void PPC750Dis_1AAF8F8A()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1AAF8F8A, "@@@");
        }

        // Unknown PowerPC VMX instruction 1AC1CF8C 06-078 (120)
        [Test]
        public void PPC750Dis_1AC1CF8C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1AC1CF8C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1ACED55C 06-055 (85)
        [Test]
        public void PPC750Dis_1ACED55C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1ACED55C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1ADF189F 06-009 (9)
        [Test]
        public void PPC750Dis_1ADF189F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1ADF189F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1ADF1ADC 06-02D (45)
        [Test]
        public void PPC750Dis_1ADF1ADC()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1ADF1ADC, "@@@");
        }

        // Unknown PowerPC VMX instruction 1AEA229D 06-029 (41)
        [Test]
        public void PPC750Dis_1AEA229D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1AEA229D, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B08C471 06-047 (71)
        [Test]
        public void PPC750Dis_1B08C471()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B08C471, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B15978C 06-078 (120)
        [Test]
        public void PPC750Dis_1B15978C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B15978C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B1E1B3F 06-033 (51)
        [Test]
        public void PPC750Dis_1B1E1B3F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B1E1B3F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B1F185F 06-005 (5)
        [Test]
        public void PPC750Dis_1B1F185F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B1F185F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B3E1B3C 06-033 (51)
        [Test]
        public void PPC750Dis_1B3E1B3C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B3E1B3C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5C007B 06-007 (7)
        [Test]
        public void PPC750Dis_1B5C007B()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5C007B, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5C193E 06-013 (19)
        [Test]
        public void PPC750Dis_1B5C193E()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5C193E, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5D1BAE 06-03A(58)
        [Test]
        public void PPC750Dis_1B5D1BAE()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5D1BAE, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5E029F 06-029 (41)
        [Test]
        public void PPC750Dis_1B5E029F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5E029F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5E1B5C 06-035 (53)
        [Test]
        public void PPC750Dis_1B5E1B5C()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5E1B5C, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5F009F 06-009 (9)
        [Test]
        public void PPC750Dis_1B5F009F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5F009F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B5F1B5D 06-035 (53)
        [Test]
        public void PPC750Dis_1B5F1B5D()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B5F1B5D, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B77F19F 06-019 (25)
        [Test]
        public void PPC750Dis_1B77F19F()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B77F19F, "@@@");
        }

        // Unknown PowerPC VMX instruction 1B7DA7AA 06-07A(122)
        [Test]
        public void PPC750Dis_1B7DA7AA()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1B7DA7AA, "@@@");
        }

        // Unknown PowerPC VX instruction 100E0206 04-206 (518)
        [Test]
        public void PPC750Dis_100E0206()
        {
            Given_ProcessorModel_750();
            AssertCode(0x100E0206, "@@@");
        }

        [Test]
        public void PPC750Dis_ps_rsqrte()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10130034, "ps_rsqrte\tf1,f0");
        }

        [Test]
        public void PPC750Dis_ps_mr()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10200090, "ps_mr\tf1,f0");
        }

#if NYI
        // Unknown PowerPC VX instruction 102004A0 04-4A0(1184)
        [Test]
        public void PPC750Dis_102004A0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x102004A0, "@@@");
        }

        // Unknown PowerPC VX instruction 102104A0 04-4A0(1184)
        [Test]
        public void PPC750Dis_102104A0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x102104A0, "@@@");
        }

        // Unknown PowerPC VX instruction 10210CA0 04-4A0(1184)
        [Test]
        public void PPC750Dis_10210CA0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10210CA0, "@@@");
        }

           [Test]
        public void PPC750Dis_psq_stux()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1023674E, "psq_stux\tf1,r3,r12,01,06");
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

        // Unknown PowerPC VX instruction 10400C60 04-460 (1120)
        [Test]
        public void PPC750Dis_10400C60()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10400C60, "@@@");
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

        [Test]
        public void PPC750Dis_10E424A0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x10E424A0, "@@@");
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

        [Test]
        public void PPC750Dis_110844A0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x110844A0, "@@@");
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

        // Unknown PowerPC VX instruction 1295BC60 04-460 (1120)
        [Test]
        public void PPC750Dis_1295BC60()
        {
            Given_ProcessorModel_750();
            AssertCode(0x1295BC60, "@@@");
        }

        // Unknown PowerPC VX instruction 129D0040 04-040 (64)
        [Test]
        public void PPC750Dis_ps_cmpo0()
        {
            Given_ProcessorModel_750();
            AssertCode(0x129D0040, "ps_cmpo0\tcr4,f29,f0");
        }

        // Unknown PowerPC VX instruction 12D7AC60 04-460 (1120)
        [Test]
        public void PPC750Dis_12D7AC60()
        {
            Given_ProcessorModel_750();
            AssertCode(0x12D7AC60, "@@@");
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

        // Unknown PowerPC XX3 instruction F0030018 3C-003 (3)
        [Test]
        public void PPC750Dis_F0030018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0030018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0030020 3C-004 (4)
        [Test]
        public void PPC750Dis_F0030020()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0030020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0040010 3C-002 (2)
        [Test]
        public void PPC750Dis_F0040010()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0040010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0050028 3C-005 (5)
        [Test]
        public void PPC750Dis_F0050028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0050028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F00501C8 3C-039 (57)
        [Test]
        public void PPC750Dis_F00501C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF00501C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0060010 3C-002 (2)
        [Test]
        public void PPC750Dis_F0060010()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0060010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0066000 3C-C00(3072)
        [Test]
        public void PPC750Dis_F0066000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0066000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F01B7427 3C-E84(3716)
        [Test]
        public void PPC750Dis_F01B7427()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF01B7427, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0230028 3C-005 (5)
        [Test]
        public void PPC750Dis_F0230028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0230028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0238008 3C-1001 (4097)
        [Test]
        public void PPC750Dis_F0238008()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0238008, "@@@");
        }

        // Unknown PowerPC XX3 instruction F02501D0 3C-03A(58)
        [Test]
        public void PPC750Dis_F02501D0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF02501D0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F026E000 3C-1C00(7168)
        [Test]
        public void PPC750Dis_F026E000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF026E000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F03E7336 3C-E66(3686)
        [Test]
        public void PPC750Dis_F03E7336()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF03E7336, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0430010 3C-002 (2)
        [Test]
        public void PPC750Dis_F0430010()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0430010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0450020 3C-004 (4)
        [Test]
        public void PPC750Dis_F0450020()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0450020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F04501D8 3C-03B(59)
        [Test]
        public void PPC750Dis_F04501D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF04501D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0466000 3C-C00(3072)
        [Test]
        public void PPC750Dis_F0466000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0466000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F06501E0 3C-03C(60)
        [Test]
        public void PPC750Dis_F06501E0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF06501E0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F066E000 3C-1C00(7168)
        [Test]
        public void PPC750Dis_F066E000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF066E000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F069A9F1 3C-153E (5438)
        [Test]
        public void PPC750Dis_F069A9F1()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF069A9F1, "@@@");
        }

        // Unknown PowerPC XX3 instruction F07B197D 3C-32F (815)
        [Test]
        public void PPC750Dis_F07B197D()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF07B197D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F07F3BFF 3C-77F (1919)
        [Test]
        public void PPC750Dis_F07F3BFF()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF07F3BFF, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0830A26 3C-144 (324)
        [Test]
        public void PPC750Dis_F0830A26()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0830A26, "@@@");
        }

        // Unknown PowerPC XX3 instruction F08501E8 3C-03D (61)
        [Test]
        public void PPC750Dis_F08501E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF08501E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F08DAF16 3C-15E2 (5602)
        [Test]
        public void PPC750Dis_F08DAF16()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF08DAF16, "@@@");
        }

        // Unknown PowerPC XX3 instruction F09EF686 3C-1ED0 (7888)
        [Test]
        public void PPC750Dis_F09EF686()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF09EF686, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0A501F0 3C-03E (62)
        [Test]
        public void PPC750Dis_F0A501F0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0A501F0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0BDE0FB 3C-1C1F(7199)
        [Test]
        public void PPC750Dis_F0BDE0FB()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0BDE0FB, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0C501F8 3C-03F (63)
        [Test]
        public void PPC750Dis_F0C501F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0C501F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0DDAFA3 3C-15F4 (5620)
        [Test]
        public void PPC750Dis_F0DDAFA3()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0DDAFA3, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E1C36A 3C-186D (6253)
        [Test]
        public void PPC750Dis_F0E1C36A()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0E1C36A, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E1C3EB 3C-187D (6269)
        [Test]
        public void PPC750Dis_F0E1C3EB()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0E1C3EB, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E1DFAA 3C-1BF5(7157)
        [Test]
        public void PPC750Dis_F0E1DFAA()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0E1DFAA, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0E50200 3C-040 (64)
        [Test]
        public void PPC750Dis_F0E50200()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0E50200, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0EBE080 3C-1C10(7184)
        [Test]
        public void PPC750Dis_F0EBE080()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0EBE080, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0F1F2F3 3C-1E5E(7774)
        [Test]
        public void PPC750Dis_F0F1F2F3()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0F1F2F3, "@@@");
        }

        // Unknown PowerPC XX3 instruction F0FFF080 3C-1E10 (7696)
        [Test]
        public void PPC750Dis_F0FFF080()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF0FFF080, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1001A3F 3C-347 (839)
        [Test]
        public void PPC750Dis_F1001A3F()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1001A3F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1038028 3C-1005 (4101)
        [Test]
        public void PPC750Dis_F1038028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1038028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1050208 3C-041 (65)
        [Test]
        public void PPC750Dis_F1050208()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1050208, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1250210 3C-042 (66)
        [Test]
        public void PPC750Dis_F1250210()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1250210, "@@@");
        }

        // Unknown PowerPC XX3 instruction F12C8008 3C-1001 (4097)
        [Test]
        public void PPC750Dis_F12C8008()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF12C8008, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1318139 3C-1027 (4135)
        [Test]
        public void PPC750Dis_F1318139()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1318139, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1321A3F 3C-347 (839)
        [Test]
        public void PPC750Dis_F1321A3F()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1321A3F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F13E809D 3C-1013 (4115)
        [Test]
        public void PPC750Dis_F13E809D()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF13E809D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1450218 3C-043 (67)
        [Test]
        public void PPC750Dis_F1450218()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1450218, "@@@");
        }

        // Unknown PowerPC XX3 instruction F14C0010 3C-002 (2)
        [Test]
        public void PPC750Dis_F14C0010()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF14C0010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F15C3497 3C-692 (1682)
        [Test]
        public void PPC750Dis_F15C3497()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF15C3497, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1638038 3C-1007 (4103)
        [Test]
        public void PPC750Dis_F1638038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1638038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1650220 3C-044 (68)
        [Test]
        public void PPC750Dis_F1650220()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1650220, "@@@");
        }

        // Unknown PowerPC XX3 instruction F16C8018 3C-1003 (4099)
        [Test]
        public void PPC750Dis_F16C8018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF16C8018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1838008 3C-1001 (4097)
        [Test]
        public void PPC750Dis_F1838008()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1838008, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1850228 3C-045 (69)
        [Test]
        public void PPC750Dis_F1850228()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1850228, "@@@");
        }

        // Unknown PowerPC XX3 instruction F18A074E 3C-0E9 (233)
        [Test]
        public void PPC750Dis_F18A074E()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF18A074E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F18C0020 3C-004 (4)
        [Test]
        public void PPC750Dis_F18C0020()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF18C0020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F18FBC95 3C-1792 (6034)
        [Test]
        public void PPC750Dis_F18FBC95()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF18FBC95, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1A30010 3C-002 (2)
        [Test]
        public void PPC750Dis_F1A30010()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1A30010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1A38018 3C-1003 (4099)
        [Test]
        public void PPC750Dis_F1A38018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1A38018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1A50230 3C-046 (70)
        [Test]
        public void PPC750Dis_F1A50230()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1A50230, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1AC8028 3C-1005 (4101)
        [Test]
        public void PPC750Dis_F1AC8028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1AC8028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1AFFA26 3C-1F44 (8004)
        [Test]
        public void PPC750Dis_F1AFFA26()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1AFFA26, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1C100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F1C100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1C100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1C50010 3C-002 (2)
        [Test]
        public void PPC750Dis_F1C50010()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1C50010, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1C50238 3C-047 (71)
        [Test]
        public void PPC750Dis_F1C50238()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1C50238, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1CC0030 3C-006 (6)
        [Test]
        public void PPC750Dis_F1CC0030()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1CC0030, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1E100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F1E100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1E100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1E50018 3C-003 (3)
        [Test]
        public void PPC750Dis_F1E50018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1E50018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1E50240 3C-048 (72)
        [Test]
        public void PPC750Dis_F1E50240()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1E50240, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1EC8038 3C-1007 (4103)
        [Test]
        public void PPC750Dis_F1EC8038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1EC8038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F1F8167F 3C-2CF(719)
        [Test]
        public void PPC750Dis_F1F8167F()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF1F8167F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2006E00 3C-DC0(3520)
        [Test]
        public void PPC750Dis_F2006E00()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2006E00, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2010108 3C-021 (33)
        [Test]
        public void PPC750Dis_F2010108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2010108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2050248 3C-049 (73)
        [Test]
        public void PPC750Dis_F2050248()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2050248, "@@@");
        }

        // Unknown PowerPC XX3 instruction F21BB423 3C-1684 (5764)
        [Test]
        public void PPC750Dis_F21BB423()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF21BB423, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2210118 3C-023 (35)
        [Test]
        public void PPC750Dis_F2210118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2210118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2250250 3C-04A(74)
        [Test]
        public void PPC750Dis_F2250250()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2250250, "@@@");
        }

        // Unknown PowerPC XX3 instruction F22979B8 3C-F37(3895)
        [Test]
        public void PPC750Dis_F22979B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF22979B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F22D681F 3C-D03(3331)
        [Test]
        public void PPC750Dis_F22D681F()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF22D681F, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2410028 3C-005 (5)
        [Test]
        public void PPC750Dis_F2410028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2410028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F24100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F24100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF24100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2410128 3C-025 (37)
        [Test]
        public void PPC750Dis_F2410128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2410128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2450258 3C-04B(75)
        [Test]
        public void PPC750Dis_F2450258()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2450258, "@@@");
        }

        // Unknown PowerPC XX3 instruction F25567D1 3C-CFA(3322)
        [Test]
        public void PPC750Dis_F25567D1()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF25567D1, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2598ED7 3C-11DA(4570)
        [Test]
        public void PPC750Dis_F2598ED7()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2598ED7, "@@@");
        }

        // Unknown PowerPC XX3 instruction F25BF000 3C-1E00 (7680)
        [Test]
        public void PPC750Dis_F25BF000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF25BF000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2610038 3C-007 (7)
        [Test]
        public void PPC750Dis_F2610038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2610038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F26100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F26100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF26100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2610138 3C-027 (39)
        [Test]
        public void PPC750Dis_F2610138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2610138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2650260 3C-04C(76)
        [Test]
        public void PPC750Dis_F2650260()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2650260, "@@@");
        }

        // Unknown PowerPC XX3 instruction F274002E 3C-005 (5)
        [Test]
        public void PPC750Dis_F274002E()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF274002E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F27547BD 3C-8F7 (2295)
        [Test]
        public void PPC750Dis_F27547BD()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF27547BD, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2758092 3C-1012 (4114)
        [Test]
        public void PPC750Dis_F2758092()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2758092, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2786E68 3C-DCD(3533)
        [Test]
        public void PPC750Dis_F2786E68()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2786E68, "@@@");
        }

        // Unknown PowerPC XX3 instruction F279BC1E 3C-1783 (6019)
        [Test]
        public void PPC750Dis_F279BC1E()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF279BC1E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F27B7000 3C-E00(3584)
        [Test]
        public void PPC750Dis_F27B7000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF27B7000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2810098 3C-013 (19)
        [Test]
        public void PPC750Dis_F2810098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2810098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F28100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F28100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF28100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2810148 3C-029 (41)
        [Test]
        public void PPC750Dis_F2810148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2810148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2850268 3C-04D (77)
        [Test]
        public void PPC750Dis_F2850268()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2850268, "@@@");
        }

        // Unknown PowerPC XX3 instruction F288EA4C 3C-1D49 (7497)
        [Test]
        public void PPC750Dis_F288EA4C()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF288EA4C, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10058 3C-00B(11)
        [Test]
        public void PPC750Dis_F2A10058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10088 3C-011 (17)
        [Test]
        public void PPC750Dis_F2A10088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F2A100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10108 3C-021 (33)
        [Test]
        public void PPC750Dis_F2A10108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10118 3C-023 (35)
        [Test]
        public void PPC750Dis_F2A10118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A10158 3C-02B(43)
        [Test]
        public void PPC750Dis_F2A10158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2A50270 3C-04E (78)
        [Test]
        public void PPC750Dis_F2A50270()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2A50270, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2ADF96A 3C-1F2D (7981)
        [Test]
        public void PPC750Dis_F2ADF96A()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2ADF96A, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2B83D71 3C-7AE(1966)
        [Test]
        public void PPC750Dis_F2B83D71()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2B83D71, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2BE5F39 3C-BE7(3047)
        [Test]
        public void PPC750Dis_F2BE5F39()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2BE5F39, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10018 3C-003 (3)
        [Test]
        public void PPC750Dis_F2C10018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10068 3C-00D (13)
        [Test]
        public void PPC750Dis_F2C10068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10098 3C-013 (19)
        [Test]
        public void PPC750Dis_F2C10098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F2C100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10118 3C-023 (35)
        [Test]
        public void PPC750Dis_F2C10118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10128 3C-025 (37)
        [Test]
        public void PPC750Dis_F2C10128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C10168 3C-02D (45)
        [Test]
        public void PPC750Dis_F2C10168()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C10168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C102A8 3C-055 (85)
        [Test]
        public void PPC750Dis_F2C102A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C102A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2C50278 3C-04F (79)
        [Test]
        public void PPC750Dis_F2C50278()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2C50278, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2CB563B 3C-AC7(2759)
        [Test]
        public void PPC750Dis_F2CB563B()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2CB563B, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10028 3C-005 (5)
        [Test]
        public void PPC750Dis_F2E10028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10058 3C-00B(11)
        [Test]
        public void PPC750Dis_F2E10058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10078 3C-00F (15)
        [Test]
        public void PPC750Dis_F2E10078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F2E100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F2E100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F2E100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10128 3C-025 (37)
        [Test]
        public void PPC750Dis_F2E10128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10138 3C-027 (39)
        [Test]
        public void PPC750Dis_F2E10138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E10178 3C-02F (47)
        [Test]
        public void PPC750Dis_F2E10178()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E10178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E102B8 3C-057 (87)
        [Test]
        public void PPC750Dis_F2E102B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E102B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E102C8 3C-059 (89)
        [Test]
        public void PPC750Dis_F2E102C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E102C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E3F2E7 3C-1E5C(7772)
        [Test]
        public void PPC750Dis_F2E3F2E7()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E3F2E7, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E50280 3C-050 (80)
        [Test]
        public void PPC750Dis_F2E50280()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E50280, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2E7F278 3C-1E4F (7759)
        [Test]
        public void PPC750Dis_F2E7F278()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2E7F278, "@@@");
        }

        // Unknown PowerPC XX3 instruction F2FEBBDD 3C-177B(6011)
        [Test]
        public void PPC750Dis_F2FEBBDD()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF2FEBBDD, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3010038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3010068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3010088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F30100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F30100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F30100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010138 3C-027 (39)
        [Test]
        public void PPC750Dis_F3010138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010148 3C-029 (41)
        [Test]
        public void PPC750Dis_F3010148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010188 3C-031 (49)
        [Test]
        public void PPC750Dis_F3010188()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30101B8 3C-037 (55)
        [Test]
        public void PPC750Dis_F30101B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30102C8 3C-059 (89)
        [Test]
        public void PPC750Dis_F30102C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30102C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30102D8 3C-05B(91)
        [Test]
        public void PPC750Dis_F30102D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30102D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3010338 3C-067 (103)
        [Test]
        public void PPC750Dis_F3010338()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3010338, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3050288 3C-051 (81)
        [Test]
        public void PPC750Dis_F3050288()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3050288, "@@@");
        }

        // Unknown PowerPC XX3 instruction F30D8CC8 3C-1199 (4505)
        [Test]
        public void PPC750Dis_F30D8CC8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF30D8CC8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3121BB8 3C-377 (887)
        [Test]
        public void PPC750Dis_F3121BB8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3121BB8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F318A729 3C-14E5 (5349)
        [Test]
        public void PPC750Dis_F318A729()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF318A729, "@@@");
        }

        // Unknown PowerPC XX3 instruction F31BE000 3C-1C00(7168)
        [Test]
        public void PPC750Dis_F31BE000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF31BE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F31C473E 3C-8E7 (2279)
        [Test]
        public void PPC750Dis_F31C473E()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF31C473E, "@@@");
        }

        // Unknown PowerPC XX3 instruction F31C6030 3C-C06(3078)
        [Test]
        public void PPC750Dis_F31C6030()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF31C6030, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3210028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3210038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3210068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3210078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3210098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F32100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F32100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210108 3C-021 (33)
        [Test]
        public void PPC750Dis_F3210108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210148 3C-029 (41)
        [Test]
        public void PPC750Dis_F3210148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210158 3C-02B(43)
        [Test]
        public void PPC750Dis_F3210158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210198 3C-033 (51)
        [Test]
        public void PPC750Dis_F3210198()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32101C8 3C-039 (57)
        [Test]
        public void PPC750Dis_F32101C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32102D8 3C-05B(91)
        [Test]
        public void PPC750Dis_F32102D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32102D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32102E8 3C-05D (93)
        [Test]
        public void PPC750Dis_F32102E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32102E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210348 3C-069 (105)
        [Test]
        public void PPC750Dis_F3210348()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210348, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3210998 3C-133 (307)
        [Test]
        public void PPC750Dis_F3210998()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3210998, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3250290 3C-052 (82)
        [Test]
        public void PPC750Dis_F3250290()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3250290, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32B718D 3C-E31(3633)
        [Test]
        public void PPC750Dis_F32B718D()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32B718D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32BD4C3 3C-1A98(6808)
        [Test]
        public void PPC750Dis_F32BD4C3()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32BD4C3, "@@@");
        }

        // Unknown PowerPC XX3 instruction F32F039C 3C-073 (115)
        [Test]
        public void PPC750Dis_F32F039C()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF32F039C, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3385CB8 3C-B97(2967)
        [Test]
        public void PPC750Dis_F3385CB8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3385CB8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F33B6000 3C-C00(3072)
        [Test]
        public void PPC750Dis_F33B6000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF33B6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F33FC277 3C-184E (6222)
        [Test]
        public void PPC750Dis_F33FC277()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF33FC277, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410018 3C-003 (3)
        [Test]
        public void PPC750Dis_F3410018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3410028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3410038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410058 3C-00B(11)
        [Test]
        public void PPC750Dis_F3410058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3410078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3410088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3410098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F34100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F34100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F34100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410118 3C-023 (35)
        [Test]
        public void PPC750Dis_F3410118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410128 3C-025 (37)
        [Test]
        public void PPC750Dis_F3410128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410158 3C-02B(43)
        [Test]
        public void PPC750Dis_F3410158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410168 3C-02D (45)
        [Test]
        public void PPC750Dis_F3410168()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34101A8 3C-035 (53)
        [Test]
        public void PPC750Dis_F34101A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34101D8 3C-03B(59)
        [Test]
        public void PPC750Dis_F34101D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34101D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34102E8 3C-05D (93)
        [Test]
        public void PPC750Dis_F34102E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34102E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34102F8 3C-05F (95)
        [Test]
        public void PPC750Dis_F34102F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34102F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410358 3C-06B(107)
        [Test]
        public void PPC750Dis_F3410358()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410358, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3410738 3C-0E7 (231)
        [Test]
        public void PPC750Dis_F3410738()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3410738, "@@@");
        }

        // Unknown PowerPC XX3 instruction F34109A8 3C-135 (309)
        [Test]
        public void PPC750Dis_F34109A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF34109A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3450298 3C-053 (83)
        [Test]
        public void PPC750Dis_F3450298()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3450298, "@@@");
        }

        // Unknown PowerPC XX3 instruction F35BE000 3C-1C00(7168)
        [Test]
        public void PPC750Dis_F35BE000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF35BE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610018 3C-003 (3)
        [Test]
        public void PPC750Dis_F3610018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3610028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3610038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610058 3C-00B(11)
        [Test]
        public void PPC750Dis_F3610058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3610068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3610078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3610088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3610098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F36100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F36100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F36100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F36100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F36100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610108 3C-021 (33)
        [Test]
        public void PPC750Dis_F3610108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610128 3C-025 (37)
        [Test]
        public void PPC750Dis_F3610128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610138 3C-027 (39)
        [Test]
        public void PPC750Dis_F3610138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610158 3C-02B(43)
        [Test]
        public void PPC750Dis_F3610158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610168 3C-02D (45)
        [Test]
        public void PPC750Dis_F3610168()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610178 3C-02F (47)
        [Test]
        public void PPC750Dis_F3610178()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36101B8 3C-037 (55)
        [Test]
        public void PPC750Dis_F36101B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36101E8 3C-03D (61)
        [Test]
        public void PPC750Dis_F36101E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36101E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36102F8 3C-05F (95)
        [Test]
        public void PPC750Dis_F36102F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36102F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610308 3C-061 (97)
        [Test]
        public void PPC750Dis_F3610308()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610308, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610368 3C-06D (109)
        [Test]
        public void PPC750Dis_F3610368()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610368, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3610748 3C-0E9 (233)
        [Test]
        public void PPC750Dis_F3610748()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3610748, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36107C8 3C-0F9 (249)
        [Test]
        public void PPC750Dis_F36107C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36107C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36109B8 3C-137 (311)
        [Test]
        public void PPC750Dis_F36109B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36109B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F36502A0 3C-054 (84)
        [Test]
        public void PPC750Dis_F36502A0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF36502A0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F365DF2C 3C-1BE5(7141)
        [Test]
        public void PPC750Dis_F365DF2C()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF365DF2C, "@@@");
        }

        // Unknown PowerPC XX3 instruction F371286D 3C-50D (1293)
        [Test]
        public void PPC750Dis_F371286D()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF371286D, "@@@");
        }

        // Unknown PowerPC XX3 instruction F37B6000 3C-C00(3072)
        [Test]
        public void PPC750Dis_F37B6000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF37B6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810018 3C-003 (3)
        [Test]
        public void PPC750Dis_F3810018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3810028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3810038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810058 3C-00B(11)
        [Test]
        public void PPC750Dis_F3810058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3810068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3810078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3810088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3810098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F38100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F38100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F38100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F38100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F38100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F38100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810118 3C-023 (35)
        [Test]
        public void PPC750Dis_F3810118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810128 3C-025 (37)
        [Test]
        public void PPC750Dis_F3810128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810138 3C-027 (39)
        [Test]
        public void PPC750Dis_F3810138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810148 3C-029 (41)
        [Test]
        public void PPC750Dis_F3810148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810168 3C-02D (45)
        [Test]
        public void PPC750Dis_F3810168()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810178 3C-02F (47)
        [Test]
        public void PPC750Dis_F3810178()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810188 3C-031 (49)
        [Test]
        public void PPC750Dis_F3810188()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810198 3C-033 (51)
        [Test]
        public void PPC750Dis_F3810198()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101A8 3C-035 (53)
        [Test]
        public void PPC750Dis_F38101A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101C8 3C-039 (57)
        [Test]
        public void PPC750Dis_F38101C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101E8 3C-03D (61)
        [Test]
        public void PPC750Dis_F38101E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38101E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38101F8 3C-03F (63)
        [Test]
        public void PPC750Dis_F38101F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38101F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810308 3C-061 (97)
        [Test]
        public void PPC750Dis_F3810308()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810308, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810318 3C-063 (99)
        [Test]
        public void PPC750Dis_F3810318()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810318, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810378 3C-06F (111)
        [Test]
        public void PPC750Dis_F3810378()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810378, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3810758 3C-0EB(235)
        [Test]
        public void PPC750Dis_F3810758()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3810758, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38107D8 3C-0FB(251)
        [Test]
        public void PPC750Dis_F38107D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38107D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38109C8 3C-139 (313)
        [Test]
        public void PPC750Dis_F38109C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38109C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3842EEC 3C-5DD(1501)
        [Test]
        public void PPC750Dis_F3842EEC()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3842EEC, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38502A8 3C-055 (85)
        [Test]
        public void PPC750Dis_F38502A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38502A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F38704AC 3C-095 (149)
        [Test]
        public void PPC750Dis_F38704AC()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF38704AC, "@@@");
        }

        // Unknown PowerPC XX3 instruction F39BE000 3C-1C00(7168)
        [Test]
        public void PPC750Dis_F39BE000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF39BE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10018 3C-003 (3)
        [Test]
        public void PPC750Dis_F3A10018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3A10028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3A10038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10058 3C-00B(11)
        [Test]
        public void PPC750Dis_F3A10058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3A10068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3A10078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3A10088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3A10098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F3A100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F3A100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F3A100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F3A100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F3A100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F3A100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10108 3C-021 (33)
        [Test]
        public void PPC750Dis_F3A10108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10118 3C-023 (35)
        [Test]
        public void PPC750Dis_F3A10118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10128 3C-025 (37)
        [Test]
        public void PPC750Dis_F3A10128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10138 3C-027 (39)
        [Test]
        public void PPC750Dis_F3A10138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10148 3C-029 (41)
        [Test]
        public void PPC750Dis_F3A10148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10158 3C-02B(43)
        [Test]
        public void PPC750Dis_F3A10158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10178 3C-02F (47)
        [Test]
        public void PPC750Dis_F3A10178()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10188 3C-031 (49)
        [Test]
        public void PPC750Dis_F3A10188()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10198 3C-033 (51)
        [Test]
        public void PPC750Dis_F3A10198()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101A8 3C-035 (53)
        [Test]
        public void PPC750Dis_F3A101A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101B8 3C-037 (55)
        [Test]
        public void PPC750Dis_F3A101B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101D8 3C-03B(59)
        [Test]
        public void PPC750Dis_F3A101D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A101D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A101F8 3C-03F (63)
        [Test]
        public void PPC750Dis_F3A101F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A101F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10208 3C-041 (65)
        [Test]
        public void PPC750Dis_F3A10208()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10208, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10318 3C-063 (99)
        [Test]
        public void PPC750Dis_F3A10318()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10318, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10328 3C-065 (101)
        [Test]
        public void PPC750Dis_F3A10328()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10328, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10388 3C-071 (113)
        [Test]
        public void PPC750Dis_F3A10388()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10388, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10568 3C-0AD(173)
        [Test]
        public void PPC750Dis_F3A10568()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10568, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A10768 3C-0ED (237)
        [Test]
        public void PPC750Dis_F3A10768()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A10768, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A107E8 3C-0FD(253)
        [Test]
        public void PPC750Dis_F3A107E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A107E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A109D8 3C-13B(315)
        [Test]
        public void PPC750Dis_F3A109D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A109D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3A502B0 3C-056 (86)
        [Test]
        public void PPC750Dis_F3A502B0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3A502B0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3B5F7EC 3C-1EFD(7933)
        [Test]
        public void PPC750Dis_F3B5F7EC()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3B5F7EC, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3BB6000 3C-C00(3072)
        [Test]
        public void PPC750Dis_F3BB6000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3BB6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3BC2D54 3C-5AA(1450)
        [Test]
        public void PPC750Dis_F3BC2D54()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3BC2D54, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3BF8AA6 3C-1154 (4436)
        [Test]
        public void PPC750Dis_F3BF8AA6()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3BF8AA6, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10018 3C-003 (3)
        [Test]
        public void PPC750Dis_F3C10018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3C10028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3C10038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10058 3C-00B(11)
        [Test]
        public void PPC750Dis_F3C10058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3C10068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3C10078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3C10088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3C10098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F3C100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F3C100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F3C100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F3C100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F3C100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F3C100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10108 3C-021 (33)
        [Test]
        public void PPC750Dis_F3C10108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10118 3C-023 (35)
        [Test]
        public void PPC750Dis_F3C10118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10128 3C-025 (37)
        [Test]
        public void PPC750Dis_F3C10128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10138 3C-027 (39)
        [Test]
        public void PPC750Dis_F3C10138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10148 3C-029 (41)
        [Test]
        public void PPC750Dis_F3C10148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10158 3C-02B(43)
        [Test]
        public void PPC750Dis_F3C10158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10168 3C-02D (45)
        [Test]
        public void PPC750Dis_F3C10168()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10188 3C-031 (49)
        [Test]
        public void PPC750Dis_F3C10188()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10198 3C-033 (51)
        [Test]
        public void PPC750Dis_F3C10198()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101A8 3C-035 (53)
        [Test]
        public void PPC750Dis_F3C101A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101B8 3C-037 (55)
        [Test]
        public void PPC750Dis_F3C101B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101C8 3C-039 (57)
        [Test]
        public void PPC750Dis_F3C101C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C101E8 3C-03D (61)
        [Test]
        public void PPC750Dis_F3C101E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C101E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10208 3C-041 (65)
        [Test]
        public void PPC750Dis_F3C10208()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10208, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10218 3C-043 (67)
        [Test]
        public void PPC750Dis_F3C10218()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10218, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10328 3C-065 (101)
        [Test]
        public void PPC750Dis_F3C10328()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10328, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10338 3C-067 (103)
        [Test]
        public void PPC750Dis_F3C10338()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10338, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10398 3C-073 (115)
        [Test]
        public void PPC750Dis_F3C10398()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10398, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C103E8 3C-07D (125)
        [Test]
        public void PPC750Dis_F3C103E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C103E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10578 3C-0AF(175)
        [Test]
        public void PPC750Dis_F3C10578()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10578, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10668 3C-0CD(205)
        [Test]
        public void PPC750Dis_F3C10668()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10668, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C10778 3C-0EF (239)
        [Test]
        public void PPC750Dis_F3C10778()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C10778, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C107F8 3C-0FF(255)
        [Test]
        public void PPC750Dis_F3C107F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C107F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C109E8 3C-13D (317)
        [Test]
        public void PPC750Dis_F3C109E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C109E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C30020 3C-004 (4)
        [Test]
        public void PPC750Dis_F3C30020()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C30020, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3C502B8 3C-057 (87)
        [Test]
        public void PPC750Dis_F3C502B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3C502B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3DB35E9 3C-6BD(1725)
        [Test]
        public void PPC750Dis_F3DB35E9()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3DB35E9, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3DBE000 3C-1C00(7168)
        [Test]
        public void PPC750Dis_F3DBE000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3DBE000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3DC9B15 3C-1362 (4962)
        [Test]
        public void PPC750Dis_F3DC9B15()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3DC9B15, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10018 3C-003 (3)
        [Test]
        public void PPC750Dis_F3E10018()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10018, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10028 3C-005 (5)
        [Test]
        public void PPC750Dis_F3E10028()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10028, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10038 3C-007 (7)
        [Test]
        public void PPC750Dis_F3E10038()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10038, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10058 3C-00B(11)
        [Test]
        public void PPC750Dis_F3E10058()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10058, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10068 3C-00D (13)
        [Test]
        public void PPC750Dis_F3E10068()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10068, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10078 3C-00F (15)
        [Test]
        public void PPC750Dis_F3E10078()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10078, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10088 3C-011 (17)
        [Test]
        public void PPC750Dis_F3E10088()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10088, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10098 3C-013 (19)
        [Test]
        public void PPC750Dis_F3E10098()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10098, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100A8 3C-015 (21)
        [Test]
        public void PPC750Dis_F3E100A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E100A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100B8 3C-017 (23)
        [Test]
        public void PPC750Dis_F3E100B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E100B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100C8 3C-019 (25)
        [Test]
        public void PPC750Dis_F3E100C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E100C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100D8 3C-01B(27)
        [Test]
        public void PPC750Dis_F3E100D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E100D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100E8 3C-01D (29)
        [Test]
        public void PPC750Dis_F3E100E8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E100E8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E100F8 3C-01F (31)
        [Test]
        public void PPC750Dis_F3E100F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E100F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10108 3C-021 (33)
        [Test]
        public void PPC750Dis_F3E10108()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10108, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10118 3C-023 (35)
        [Test]
        public void PPC750Dis_F3E10118()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10118, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10128 3C-025 (37)
        [Test]
        public void PPC750Dis_F3E10128()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10128, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10138 3C-027 (39)
        [Test]
        public void PPC750Dis_F3E10138()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10138, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10148 3C-029 (41)
        [Test]
        public void PPC750Dis_F3E10148()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10148, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10158 3C-02B(43)
        [Test]
        public void PPC750Dis_F3E10158()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10158, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10168 3C-02D (45)
        [Test]
        public void PPC750Dis_F3E10168()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10168, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10178 3C-02F (47)
        [Test]
        public void PPC750Dis_F3E10178()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10178, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10188 3C-031 (49)
        [Test]
        public void PPC750Dis_F3E10188()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10188, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10198 3C-033 (51)
        [Test]
        public void PPC750Dis_F3E10198()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10198, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101A8 3C-035 (53)
        [Test]
        public void PPC750Dis_F3E101A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E101A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101B8 3C-037 (55)
        [Test]
        public void PPC750Dis_F3E101B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E101B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101C8 3C-039 (57)
        [Test]
        public void PPC750Dis_F3E101C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E101C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101D8 3C-03B(59)
        [Test]
        public void PPC750Dis_F3E101D8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E101D8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E101F8 3C-03F (63)
        [Test]
        public void PPC750Dis_F3E101F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E101F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10218 3C-043 (67)
        [Test]
        public void PPC750Dis_F3E10218()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10218, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10228 3C-045 (69)
        [Test]
        public void PPC750Dis_F3E10228()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10228, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10238 3C-047 (71)
        [Test]
        public void PPC750Dis_F3E10238()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10238, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10338 3C-067 (103)
        [Test]
        public void PPC750Dis_F3E10338()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10338, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10348 3C-069 (105)
        [Test]
        public void PPC750Dis_F3E10348()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10348, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10388 3C-071 (113)
        [Test]
        public void PPC750Dis_F3E10388()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10388, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E103A8 3C-075 (117)
        [Test]
        public void PPC750Dis_F3E103A8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E103A8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E103F8 3C-07F (127)
        [Test]
        public void PPC750Dis_F3E103F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E103F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E104B8 3C-097 (151)
        [Test]
        public void PPC750Dis_F3E104B8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E104B8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E104C8 3C-099 (153)
        [Test]
        public void PPC750Dis_F3E104C8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E104C8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10538 3C-0A7(167)
        [Test]
        public void PPC750Dis_F3E10538()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10538, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10588 3C-0B1 (177)
        [Test]
        public void PPC750Dis_F3E10588()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10588, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10678 3C-0CF(207)
        [Test]
        public void PPC750Dis_F3E10678()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10678, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10788 3C-0F1 (241)
        [Test]
        public void PPC750Dis_F3E10788()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10788, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10808 3C-101 (257)
        [Test]
        public void PPC750Dis_F3E10808()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10808, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E10928 3C-125 (293)
        [Test]
        public void PPC750Dis_F3E10928()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E10928, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E109F8 3C-13F (319)
        [Test]
        public void PPC750Dis_F3E109F8()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E109F8, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E30030 3C-006 (6)
        [Test]
        public void PPC750Dis_F3E30030()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E30030, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E502C0 3C-058 (88)
        [Test]
        public void PPC750Dis_F3E502C0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E502C0, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3E6F6B9 3C-1ED7 (7895)
        [Test]
        public void PPC750Dis_F3E6F6B9()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3E6F6B9, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3EBAE65 3C-15CC(5580)
        [Test]
        public void PPC750Dis_F3EBAE65()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3EBAE65, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3F55C48 3C-B89(2953)
        [Test]
        public void PPC750Dis_F3F55C48()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3F55C48, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3FB6000 3C-C00(3072)
        [Test]
        public void PPC750Dis_F3FB6000()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3FB6000, "@@@");
        }

        // Unknown PowerPC XX3 instruction F3FCA9D0 3C-153A(5434)
        [Test]
        public void PPC750Dis_F3FCA9D0()
        {
            Given_ProcessorModel_750();
            AssertCode(0xF3FCA9D0, "@@@");
        }
#endif        
    }
}
