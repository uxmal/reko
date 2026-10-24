#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using NUnit.Framework;
using Reko.Arch.Pdp;
using Reko.Core;

namespace Reko.UnitTests.Arch.Pdp.Linc;

public class LincRewriterTests : RewriterTestBase
{
    private readonly LincArchitecture arch;
    private readonly Address addrLoad;

    public LincRewriterTests()
    {
        this.arch = new LincArchitecture(CreateServiceContainer(), "linc", []);
        addrLoad = LincArchitecture.Ptr12(0x10);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addrLoad;

    [Test]
    public void LincRw_ada()
    {
        Given_OctalBytes4("1103");      // ADA   0003
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = AC + Mem0[R3:word12]");
    }

    [Test]
    public void LincRw_ada_0()
    {
        Given_OctalBytes4("1100 1234");      // ADA   1234
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = AC + Mem0[0o1234<p12>:word12]");
    }

    [Test]
    public void LincRw_ada_0_i()
    {
        Given_OctalBytes4("1100 1234");      // ADA   1234
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = AC + Mem0[0o1234<p12>:word12]");
    }


    [Test]
    public void LincRw_ada_i()
    {
        Given_OctalBytes4("1123");      // ADA i 0003
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|R3 = R3 + 1<12>",
            "2|L--|AC = AC + Mem0[R3:word12]");
    }

    [Test]
    public void LincRw_add()
    {
        Given_OctalBytes4("3123");      // ADD   1123
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = AC + Mem0[0o1123<p12>:word12]");
    }

    [Test]
    public void LincRw_adm()
    {
        Given_OctalBytes4("1165");      // ADM i 0005
        AssertCode(
            "0|L--|0020(1): 4 instructions",
            "1|L--|R5 = R5 + 1<12>",
            "2|L--|v4 = Mem0[R5:word12]",
            "3|L--|AC = AC + v4",
            "4|L--|Mem0[R5:word12] = v4");
    }

    [Test]
    public void LincRw_apo()
    {
        Given_OctalBytes4("0451");      // APO
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (AC >= 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_apo_i()
    {
        Given_OctalBytes4("0471");      // APO i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (AC < 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_atr()
    {
        Given_OctalBytes4("0014");      // ATR
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__transfer_to_relays(AC)");
    }

    [Test]
    public void LincRw_axo()
    {
        Given_OctalBytes4("0001");      // AXO
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__write_to_xob(AC)");
    }

    [Test]
    public void LincRw_aze()
    {
        Given_OctalBytes4("0450");      // AZE
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (AC == 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_aze_i()
    {
        Given_OctalBytes4("0470");      // AZE i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (AC != 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_bcl()
    {
        Given_OctalBytes4("1544");      // BCL   0004
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = AC & ~Mem0[R4:word12]");
    }

    [Test]
    public void LincRw_bco()
    {
        Given_OctalBytes4("1644");      // BCO   0004
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = AC ^ Mem0[R4:word12]");
    }

    [Test]
    public void LincRw_bse()
    {
        Given_OctalBytes4("1620 4242");      // BSE i 4242
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = AC | 0x8A2<12>");
    }

    [Test]
    public void LincRw_chk()
    {
        Given_OctalBytes4("0707 3500");      // CHK   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __check_one_tape_block(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_chk_i()
    {
        Given_OctalBytes4("0737 4501");      // CHK i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __check_one_tape_block(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_clr()
    {
        Given_OctalBytes4("0011");      // CLR
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|AC = 0<12>",
            "2|L--|MQ = 0<12>",
            "3|L--|L = 0<12>");
    }

    [Test]
    public void LincRw_com()
    {
        Given_OctalBytes4("0017");      // COM
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = ~AC");
    }

    [Test]
    public void LincRw_dis()
    {
        Given_OctalBytes4("0147");      // DIS   0007
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__display(Mem0[R7:word12], AC)");
    }
    [Test]
    public void LincRw_dis_i()
    {
        Given_OctalBytes4("0167");      // DIS i 0007
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|R7 = R7 + 1<12>",
            "2|L--|__display(Mem0[R7:word12], AC)");
    }

    [Test]
    public void LincRw_djr()
    {
        // PDP-12
        Given_OctalBytes4("0006");      // DJR
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__disable_jump_return()");
    }

    [Test]
    public void LincRw_dsc()
    {
        Given_OctalBytes4("1747");      // DSC   0007
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|__display_character(R1, AC, Mem0[R7:word12])",
            "2|L--|R1 = R1 + 8<12>");
    }

    [Test]
    public void LincRw_dsc_i()
    {
        Given_OctalBytes4("1767");      // DSC i 0007
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|R7 = R7 + 1<12>",
            "2|L--|__display_character(R1, AC, Mem0[R7:word12])",
            "3|L--|R1 = R1 + 8<12>");
    }

    [Test]
    public void LincRw_flo()
    {
        Given_OctalBytes4("0454");      // FLO
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (Test(OV,F)) branch 0022");
    }

    [Test]
    public void LincRw_flo_i()
    {
        Given_OctalBytes4("0474");      // FLO i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (Test(NO,F)) branch 0022");
    }

    [Test]
    public void LincRw_halt()
    {
        Given_OctalBytes4("0000");      // HALT
        AssertCode(
            "0|H--|0020(1): 1 instructions",
            "1|H--|__halt()");
    }

    [Test]
    public void LincRw_ibz()
    {
        Given_OctalBytes4("0453");      // IBZ
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (__is_tape_in_interblock_zone()) branch 0022");
    }

    [Test]
    public void LincRw_ibz_i()
    {
        Given_OctalBytes4("0473");      // IBZ i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (!__is_tape_in_interblock_zone()) branch 0022");
    }

    [Test]
    public void LincRw_iof()
    {
        // PDP-12
        Given_OctalBytes4("0500 6002");      // IOF
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|__interrupts_off()");
    }

    [Test]
    public void LincRw_ion()
    {
        // PDP-12
        Given_OctalBytes4("0500 6001");      // ION
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|__interrupts_on()");
    }

    [Test]
    public void LincRw_kst()
    {
        Given_OctalBytes4("0415");      // KST
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (__is_key_struck()) branch 0022");
    }

    [Test]
    public void LincRw_kst_i()
    {
        Given_OctalBytes4("0435");      // KST i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (!__is_key_struck()) branch 0022");
    }

    [Test]
    public void LincRw_lam()
    {
        Given_OctalBytes4("1233");      // LAM i 0013
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|R11 = R11 + 1<12>",
            "2|L--|AC = __addc<word12,word12>(AC, Mem0[R11:word12], L)",
            "3|L--|L = cond(AC)");
    }

    [Test]
    public void LincRw_lda()
    {
        Given_OctalBytes4("1020 1234");      // LDA i 1234
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = Mem0[0x29C<12>:word12]");
    }

    [Test]
    public void LincRw_ldf()
    {
        // PDP-12
        Given_OctalBytes4("0644");      // LDF   4
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__load_data_field_register(4<i8>)");
    }

    [Test]
    public void LincRw_ldf_i()
    {
        Given_OctalBytes4("0677");      // LDF   37
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__load_data_field_register(31<i8>)");
    }

    [Test]
    public void LincRw_ldh()
    {
        Given_OctalBytes4("1303");      // LDH   0003
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = CONVERT(load_halfword(Mem0[R3:word12]), word6, word12)");
    }

    [Test]
    public void LincRw_ldh_0()
    {
        Given_OctalBytes4("1300 0370 ");      // LDH i 0370
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = CONVERT(load_halfword(Mem0[0o0370<p12>:word12]), word6, word12)");
    }

    [Test]
    public void LincRw_ldh_i_0()
    {
        Given_OctalBytes4("1320 6527");      // LDH i 4370
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = 0x35<12>");
    }

    [Test]
    public void LincRw_lif()
    {
        Given_OctalBytes4("0637");      // LIF   37
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__load_linc_instruction_field_buffer()");
    }

    [Test]
    public void LincRw_lmr()
    {
        Given_OctalBytes4("0500 6151");      // LMR
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|__load_maintenance_register()");
    }

    [Test]
    public void LincRw_lsw()
    {
        Given_OctalBytes4("0517");      // LSW
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __load_left_switches()");
    }

    [Test]
    public void LincRw_lze()
    {
        Given_OctalBytes4("0452");      // LZE
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (L == 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_lzei()
    {
        Given_OctalBytes4("0472");      // LZE i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (L != 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_mtb()
    {
        Given_OctalBytes4("0703 3500");      // MTB   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __move_toward_block(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_mtb_i()
    {
        Given_OctalBytes4("0733 4501");      // MTB i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __move_toward_block(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_mul()
    {
        Given_OctalBytes4("1240 1234");      // MUL   1234
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC_MQ = AC *24 Mem0[0o1234<p12>:word12]");
    }

    [Test]
    public void LincRw_mul_i()
    {
        Given_OctalBytes4("1260 1234");      // MUL i 1234
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC_MQ = AC *24 0x29C<12>");
    }

    [Test]
    public void LincRw_nop()
    {
        Given_OctalBytes4("0016");      // NOP
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void LincRw_pdp()
    {
        Given_OctalBytes4("0002");      // PDP
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__switch_to_PDP8_mode()");
    }

    [Test]
    public void LincRw_qac()
    {
        Given_OctalBytes4("0005");      // QAC
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = CONVERT(SLICE(MQ, ui11, 1), ui11, word12)");
    }

    [Test]
    public void LincRw_qlz()
    {
        Given_OctalBytes4("0455");      // QLZ
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if ((MQ & 1<12>) == 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_qlz_i()
    {
        Given_OctalBytes4("0475");      // QLZ i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if ((MQ & 1<12>) != 0<12>) branch 0022");
    }

    [Test]
    public void LincRw_rcg()
    {
        Given_OctalBytes4("0700 3500");      // RCG   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __read_and_check_consecutive(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_rcg_i()
    {
        Given_OctalBytes4("0730 4501");      // RCG i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __read_and_check_consecutive(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_rdc()
    {
        Given_OctalBytes4("0701 3500");      // RDC   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __check_one_tape_block(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_rde()
    {
        Given_OctalBytes4("0702 3500");      // RDE   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __check_one_tape_block(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_rde_i()
    {
        Given_OctalBytes4("0732 4501");      // RDE i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __check_one_tape_block(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_rdf()
    {
        // PDP-12
        Given_OctalBytes4("0500 6214");      // RDF
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|v3 = __read_data_field()",
            "2|L--|AC = AC | SEQ(SLICE(0<12>, word6, 6), v3, SLICE(0<12>, word3, 0))");
    }

    [Test]
    public void LincRw_rib()
    {
        // PDP-12
        Given_OctalBytes4("0500 6234");      // RIB
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = __read_interrupt_buffer()");
    }

    [Test]
    public void LincRw_rif()
    {
        // PDP-12
        Given_OctalBytes4("0500 6224");      // RIF
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|AC = __read_instruction_field()");
    }

    [Test]
    public void LincRw_rmf()
    {
        // PDP-12
        Given_OctalBytes4("0500 6244");      // RMF
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|__restore_memory_field()");
    }

    [Test]
    public void LincRw_rol()
    {
        Given_OctalBytes4("0241");      // ROL   1
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __rol<word12,int8>(AC, 1<i8>)");
    }

    [Test]
    public void LincRw_rol_i()
    {
        Given_OctalBytes4("0261");      // ROL i 1
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|v5 = AC",
            "2|L--|AC = __rcl<word12,int8>(v5, 1<i8>, L)",
            "3|L--|L = cond((v5 & 0x800<12>) != 0<12>)");
    }

    [Test]
    public void LincRw_ror()
    {
        Given_OctalBytes4("0301");      // ROR   1
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __ror<word12,int8>(AC, 1<i8>)");
    }

    [Test]
    public void LincRw_ror_i()
    {
        Given_OctalBytes4("0321");      // ROR i 1
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|v5 = AC",
            "2|L--|AC = __rcr<word12,int8>(v5, 1<i8>, L)",
            "3|L--|L = cond((v5 & 1<12>) != 0<12>)");
    }

    [Test]
    public void LincRw_rsw()
    {
        Given_OctalBytes4("0516");      // RSW
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __load_right_switches()");
    }

    [Test]
    public void LincRw_rta()
    {
        Given_OctalBytes4("0015");      // RTA
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|v3 = __load_relays()",
            "2|L--|AC = SEQ(SLICE(AC, word6, 6), v3)");
    }

    [Test]
    public void LincRw_sae()
    {
        Given_OctalBytes4("1444");      // SAE   0004
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (AC == Mem0[R4:word12]) branch 0022");
    }

    [Test]
    public void LincRw_sae_0_i()
    {
        Given_OctalBytes4("1440 i 1234");      // SAE i 1234
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|T--|if (AC == Mem0[0o1234<p12>:word12]) branch 0023");
    }

    [Test]
    public void LincRw_sam()
    {
        Given_OctalBytes4("0104");      // SAM   4
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __sample_channel(4<i8>)");
    }

    [Test]
    public void LincRw_sam_i()
    {
        Given_OctalBytes4("0137");      // SAM   37
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __sample_channel(31<i8>)");
    }

    [Test]
    public void LincRw_scr()
    {
        Given_OctalBytes4("0343");      // SCR   3
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC_MQ = AC_MQ >> 3<i8>");
    }

    [Test]
    public void LincRw_scr_i()
    {
        Given_OctalBytes4("0363");      // SCR i 3
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|AC_MQ = AC_MQ >> 3<i8>",
            "2|L--|L = (MQ & 0x800<12>) != 0<12>");
    }

    [Test]
    public void LincRw_set()
    {
        Given_OctalBytes4("0043 0004");      // SET   0000 0004
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|R3 = Mem0[0o0004<p12>:word12]");
    }

    [Test]
    public void LincRw_set_i()
    {
        Given_OctalBytes4("0063 0024");      // SET i 0000 0024
        AssertCode(
            "0|L--|0020(2): 1 instructions",
            "1|L--|R3 = 0x14<12>");
    }

    [Test]
    public void LincRw_shd()
    {
        Given_OctalBytes4("1420 5500");      // SHD i 5500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|v4 = SLICE(AC, word6, 0)",   
            "2|T--|if (v4 != 0<6>) branch 0023");
    }

    [Test]
    public void LincRw_skp()
    {
        Given_OctalBytes4("0456");      // SKP
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|goto 0022");
    }

    [Test]
    public void LincRw_skp_i()
    {
        Given_OctalBytes4("0476");      // SKP i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|goto 0022");
    }

    [Test]
    public void LincRw_sns()
    {
        Given_OctalBytes4("0440");      // SNS   0
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (__sense_switch(0<i8>)) branch 0022");
    }

    [Test]
    public void LincRw_sns_i()
    {
        Given_OctalBytes4("0462");      // SNS i 2
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (!__sense_switch(2<i8>)) branch 0022");
    }

    [Test]
    public void LincRw_sro()
    {
        Given_OctalBytes4("1525");      // SRO i 0005
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|R5 = R5 + 1<12>",
            "2|L--|v4 = (Mem0[R5:word12] & 1<12>) == 0<12>",
            "3|T--|if (!v4) branch 0022");
    }

    [Test]
    public void LincRw_sta()
    {
        Given_OctalBytes4("1043");      // STA   0003
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|Mem0[R3:word12] = AC");
    }

    [Test]
    public void LincRw_stc()
    {
        Given_OctalBytes4("4124");      // STC   0124
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|Mem0[0o0124<p12>:word12] = AC",
            "2|L--|AC = 0<12>");
    }

    [Test]
    public void LincRw_std()
    {
        Given_OctalBytes4("0416");      // STD
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (__is_tape_done()) branch 0022");
    }

    [Test]
    public void LincRw_std_i()
    {
        Given_OctalBytes4("0436");      // STD i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (!__is_tape_done()) branch 0022");
    }

    [Test]
    public void LincRw_sth()
    {
        Given_OctalBytes4("1367");      // STH i 0007
        AssertCode(
            "0|L--|0020(1): 3 instructions",
            "1|L--|R7 = R7 + 1<12>",
            "2|L--|v5 = SLICE(AC, word6, 0)",
            "3|L--|__store_halfword(Mem0[R7:word12], v5)");
    }

    [Test]
    public void LincRw_sxl()
    {
        Given_OctalBytes4("0403");      // SXL   3
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (__is_external_level_high(3<i8>)) branch 0022");
    }

    [Test]
    public void LincRw_sxl_i()
    {
        Given_OctalBytes4("0423");      // SXL i 3
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (!__is_external_level_high(3<i8>)) branch 0022");
    }

    [Test]
    public void LincRw_tac()
    {
        Given_OctalBytes4("0003");      // TAC
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = TAC");
    }

    [Test]
    public void LincRw_tma()
    {
        Given_OctalBytes4("0023");      // TMA
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|__write_tma_setup_register(AC)");
    }

    [Test]
    public void LincRw_twc()
    {
        Given_OctalBytes4("0417");      // TWC
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (__is_tape_word_complete()) branch 0022");
    }

    [Test]
    public void LincRw_twc_i()
    {
        Given_OctalBytes4("0437");      // TWC i
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (!__is_tape_word_complete()) branch 0022");
    }

    [Test]
    public void LincRw_wcg()
    {
        Given_OctalBytes4("0705 3500");      // WCG   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __write_and_check_consecutive(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_wcg_i()
    {
        Given_OctalBytes4("0735 4501");      // WCG i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __write_and_check_consecutive(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_wrc()
    {
        Given_OctalBytes4("0704 3500");      // WRC   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __write_and_check(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_wrc_i()
    {
        Given_OctalBytes4("0734 4501");      // WRC i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __write_and_check(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_wri()
    {
        Given_OctalBytes4("0706 3500");      // WRI   0 3|500
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __write_tape(false, 0<12>, 3<12>, 0x140<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_wri_i()
    {
        Given_OctalBytes4("0736 4501");      // WRI i 1 4|501
        AssertCode(
            "0|L--|0020(2): 2 instructions",
            "1|L--|AC = __write_tape(true, 1<12>, 4<12>, 0x141<12>)",
            "2|L--|TAC = AC");
    }

    [Test]
    public void LincRw_xoa()
    {
        Given_OctalBytes4("0021");      // XOA
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|L--|AC = __read_from_xob()");
    }

    [Test]
    public void LincRw_xsk()
    {
        Given_OctalBytes4("0206");      // XSK   0006
        AssertCode(
            "0|L--|0020(1): 1 instructions",
            "1|T--|if (R6 == 0x3FF<12>) branch 0022");
    }

    [Test]
    public void LincRw_xsk_i()
    {
        Given_OctalBytes4("0226");      // XSK i 0006
        AssertCode(
            "0|L--|0020(1): 2 instructions",
            "1|L--|R6 = R6 + 1<i12>",
            "2|T--|if (R6 == 0x3FF<12>) branch 0022");
    }
}
