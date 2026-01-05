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
using Reko.Arch.Pdp.Memory;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Pdp.Pdp1;

class Pdp1RewriterTests : RewriterTestBase
{
    private readonly Pdp1Architecture arch;

    public Pdp1RewriterTests()
    {
        this.arch = new Pdp1Architecture(CreateServiceContainer(), "pdp1", new Dictionary<string, object>());
        this.LoadAddress = Pdp10Architecture.Ptr18(0x00100);
    }

    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress { get; }

    private void Given_OctalWord(string octalWord)
    {
        uint word = (uint) Pdp10Architecture.OctalStringToWord(octalWord);
        Given_MemoryArea(new Word18MemoryArea(LoadAddress, new[] { word }));
    }

    [Test]
    public void Pdp1Rw_add()
    {
        Given_OctalWord("400040");
        AssertCode(       // add\t000040
            "0|L--|000400(1): 3 instructions",
            "1|L--|v6 = Mem0[0o000040<p18>:word18]",
            "2|L--|acc = acc + v6",
            "3|L--|V = cond(acc)");
    }

    [Test]
    public void Pdp1Rw_add_i()
    {
        Given_OctalWord("410010");
        AssertCode(       // add.i\t000010
            "0|L--|000400(1): 4 instructions",
            "1|L--|v5 = Mem0[0o000010<p18>:ptr18]",
            "2|L--|v6 = Mem0[v5:word18]",
            "3|L--|acc = acc + v6",
            "4|L--|V = cond(acc)");
    }

    [Test]
    public void Pdp1Rw_and()
    {
        Given_OctalWord("027000");
        AssertCode(       // and\t007000
            "0|L--|000400(1): 2 instructions",
            "1|L--|v4 = Mem0[0o007000<p18>:word18]",
            "2|L--|acc = acc & v4");
    }

    [Test]
    public void Pdp1Rw_and_i()
    {
        Given_OctalWord("036000");
        AssertCode(       // and.i\t006000
            "0|L--|000400(1): 3 instructions",
            "1|L--|v4 = Mem0[0o006000<p18>:word18]",
            "2|L--|v4 = Mem0[v4:word18]",
            "3|L--|acc = acc & v4");
    }

    [Test]
    public void Pdp1Rw_cal()
    {
        Given_OctalWord("164000");
        AssertCode(       // cal\t004000
            "0|L--|000400(1): 1 instructions",
            "1|L--|__call_subroutine()"

);
    }

    [Test]
    public void Pdp1Rw_cma()
    {
        Given_OctalWord("761000");
        AssertCode(       // cma
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = ~acc");
    }

    [Test]
    public void Pdp1Rw_dac()
    {
        Given_OctalWord("244000");
        AssertCode(       // dac\t004000
            "0|L--|000400(1): 1 instructions",
            "1|L--|Mem0[0o004000<p18>:word18] = acc");
    }

    [Test]
    public void Pdp1Rw_dac_i()
    {
        Given_OctalWord("250050");
        AssertCode(       // dac.i\t000050
            "0|L--|000400(1): 2 instructions",
            "1|L--|v4 = Mem0[0o000050<p18>:word18]",
            "2|L--|Mem0[v4:word18] = acc");
    }

    [Test]
    public void Pdp1Rw_dap()
    {
        Given_OctalWord("260000");
        AssertCode(       // dap\t000000
            "0|L--|000400(1): 4 instructions",
            "1|L--|v4 = SLICE(acc, word12, 0)",
            "2|L--|v5 = Mem0[0o000000<p18>:word18]",
            "3|L--|v5 = SEQ(SLICE(v5, word6, 12), v4)",
            "4|L--|Mem0[0o000000<p18>:word18] = v5");
    }

    [Test]
    public void Pdp1Rw_dap_i()
    {
        Given_OctalWord("272000");
        AssertCode(       // dap.i\t002000
            "0|L--|000400(1): 5 instructions",
            "1|L--|v4 = SLICE(acc, word12, 0)",
            "2|L--|v6 = Mem0[0o002000<p18>:ptr18]",
            "3|L--|v5 = Mem0[v6:word18]",
            "4|L--|v5 = SEQ(SLICE(v5, word6, 12), v4)",
            "5|L--|Mem0[v6:word18] = v5");
    }

    [Test]
    public void Pdp1Rw_dip()
    {
        Given_OctalWord("302222");
        AssertCode(       // dip\t002222
            "0|L--|000400(1): 4 instructions",
            "1|L--|v4 = SLICE(acc, word12, 0)",
            "2|L--|v5 = Mem0[0o002222<p18>:word18]",
            "3|L--|v5 = SEQ(v4, SLICE(v5, word12, 0))",
            "4|L--|Mem0[0o002222<p18>:word18] = v5");
    }

    [Test]
    public void Pdp1Rw_dip_i()
    {
        Given_OctalWord("316666");
        AssertCode(       // dip.i\t006666
             "0|L--|000400(1): 5 instructions",
            "1|L--|v4 = SLICE(acc, word12, 0)",
            "2|L--|v6 = Mem0[0o006666<p18>:ptr18]",
            "3|L--|v5 = Mem0[v6:word18]",
            "4|L--|v5 = SEQ(v4, SLICE(v5, word12, 0))",
            "5|L--|Mem0[v6:word18] = v5");
    }

    [Test]
    public void Pdp1Rw_dio()
    {
        Given_OctalWord("320660");
        AssertCode(       // dio\t000660
            "0|L--|000400(1): 1 instructions",
            "1|L--|Mem0[0o000660<p18>:word18] = io");
    }

    [Test]
    public void Pdp1Rw_dio_i()
    {
        Given_OctalWord("330550");
        AssertCode(       // dio.i\t000550
            "0|L--|000400(1): 2 instructions",
            "1|L--|v4 = Mem0[0o000550<p18>:word18]",
            "2|L--|Mem0[v4:word18] = io");
    }

    [Test]
    public void Pdp1Rw_div()
    {
        Given_OctalWord("560030");
        AssertCode(       // div\t000030
            "0|L--|000400(1): 3 instructions",
            "1|L--|v5 = Mem0[0o000030<p18>:word18]",
            "2|L--|acc = acc_io /18 v5",
            "3|L--|io = acc_io %s v5");
    }

    [Test]
    public void Pdp1Rw_div_i()
    {
        Given_OctalWord("570050");
        AssertCode(       // div.i\t000050
            "0|L--|000400(1): 4 instructions",
            "1|L--|v5 = Mem0[0o000050<p18>:word18]",
            "2|L--|v5 = Mem0[v5:word18]",
            "3|L--|acc = acc_io /18 v5",
            "4|L--|io = acc_io %s v5");
    }

    [Test]
    public void Pdp1Rw_dzm()
    {
        Given_OctalWord("344444");
        AssertCode(       // dzm\t004444
            "0|L--|000400(1): 1 instructions",
            "1|L--|Mem0[0o004444<p18>:ptr18] = 0x000000<p18>");
    }

    [Test]
    public void Pdp1Rw_dzm_i()
    {
        Given_OctalWord("350055");
        AssertCode(       // dzm.i\t000055
            "0|L--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o000055<p18>:word18]",
            "2|L--|Mem0[v3:ptr18] = 0x000000<p18>");
    }

    [Test]
    public void Pdp1Rw_esm()
    {
        Given_OctalWord("720055");
        AssertCode(       // esm
            "0|L--|000400(1): 1 instructions",
            "1|L--|__enter_sequence_break_mode()");
    }

    [Test]
    public void Pdp1Rw_idx()
    {
        Given_OctalWord("444000");
        AssertCode(       // idx\t004000
            "0|L--|000400(1): 3 instructions",
            "1|L--|acc = Mem0[0o004000<p18>:word18]",
            "2|L--|acc = acc + 1<18>",
            "3|L--|Mem0[0o004000<p18>:word18] = acc");
    }

    [Test]
    public void Pdp1Rw_idx_i()
    {
        Given_OctalWord("457000");
        AssertCode(       // idx.i\t007000
            "0|L--|000400(1): 4 instructions",
            "1|L--|v3 = Mem0[0o007000<p18>:word18]",
            "2|L--|acc = Mem0[v3:word18]",
            "3|L--|acc = acc + 1<18>",
            "4|L--|Mem0[v3:word18] = acc");
    }

    [Test]
    public void Pdp1Rw_ior()
    {
        Given_OctalWord("043000");
        AssertCode(       // ior\t003000
            "0|L--|000400(1): 2 instructions",
            "1|L--|v4 = Mem0[0o003000<p18>:word18]",
            "2|L--|acc = acc | v4");
    }

    [Test]
    public void Pdp1Rw_ior_i()
    {
        Given_OctalWord("053000");
        AssertCode(       // ior.i\t003000
            "0|L--|000400(1): 3 instructions",
            "1|L--|v4 = Mem0[0o003000<p18>:word18]",
            "2|L--|v4 = Mem0[v4:word18]",
            "3|L--|acc = acc | v4");
    }

    [Test]
    public void Pdp1Rw_isp()
    {
        Given_OctalWord("467000");
        AssertCode(       // isp\t007000
            "0|T--|000400(1): 4 instructions",
            "1|L--|acc = Mem0[0o007000<p18>:word18]",
            "2|L--|acc = acc + 1<18>",
            "3|L--|Mem0[0o007000<p18>:word18] = acc",
            "4|T--|if (acc > 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_isp_i()
    {
        Given_OctalWord("472220");
        AssertCode(       // isp.i\t002220
            "0|T--|000400(1): 5 instructions",
            "1|L--|v4 = Mem0[0o002220<p18>:word18]",
            "2|L--|acc = Mem0[v4:word18]",
            "3|L--|acc = acc + 1<18>",
            "4|L--|Mem0[v4:word18] = acc",
            "5|T--|if (acc > 0<18>) branch 000402");
    }


    [Test]
    public void Pdp1Rw_jda()
    {
        Given_OctalWord("171234");
        AssertCode(       // jda\t001234
            "0|T--|000400(1): 3 instructions",
            "1|L--|Mem0[0o001234<p18>:word18] = acc",
            "2|L--|acc = 000400",
            "3|T--|goto 001235");
    }


    [Test]
    public void Pdp1Rw_jmp()
    {
        Given_OctalWord("606000");
        AssertCode(       // jmp\t006000
            "0|T--|000400(1): 1 instructions",
            "1|T--|goto 006000");
    }

    [Test]
    public void Pdp1Rw_jmp_i()
    {
        Given_OctalWord("611234");
        AssertCode(       // jmp.i\t001234
            "0|T--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o001234<p18>:word18]",
            "2|T--|goto v3");
    }

    [Test]
    public void Pdp1Rw_jsp()
    {
        Given_OctalWord("624444");
        AssertCode(       // jsp\t004444
            "0|T--|000400(1): 2 instructions",
            "1|L--|acc = 000401",
            "2|T--|call 004444 (0)");
    }

    [Test]
    public void Pdp1Rw_jsp_i()
    {
        Given_OctalWord("634242");
        AssertCode(       // jsp.i\t004242
            "0|T--|000400(1): 3 instructions",
            "1|L--|v3 = Mem0[0o004242<p18>:ptr18]",
            "2|L--|acc = 000401",
            "3|T--|call v3 (0)");
    }

    [Test]
    public void Pdp1Rw_lac()
    {
        Given_OctalWord("204242");
        AssertCode(       // lac\t004242
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = Mem0[0o004242<p18>:word18]");
    }

    [Test]
    public void Pdp1Rw_lac_i()
    {
        Given_OctalWord("210300");
        AssertCode(       // lac.i\t000300
            "0|L--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o000300<p18>:word18]",
            "2|L--|acc = Mem0[v3:word18]");
    }

    [Test]
    public void Pdp1Rw_lap()
    {
        Given_OctalWord("760100");
        AssertCode(       // lap
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = acc | 0o000401<p18>");
    }

    [Test]
    public void Pdp1Rw_lat()
    {
        Given_OctalWord("762000");
        AssertCode(       // lat
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = __load_test_word()");
    }

    [Test]
    public void Pdp1Rw_hlt()
    {
        Given_OctalWord("760400");
        AssertCode(       // hlt
            "0|H--|000400(1): 1 instructions",
            "1|H--|__halt()");
    }

    [Test]
    public void Pdp1Rw_cla()
    {
        Given_OctalWord("760200");
        AssertCode(       // cla
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = 0<18>");
    }

    [Test]
    public void Pdp1Rw_clf()
    {
        Given_OctalWord("760004");
        AssertCode(       // clf\t04
            "0|L--|000400(1): 1 instructions",
            "1|L--|__clear_program_flag(4<8>)");
    }

    [Test]
    public void Pdp1Rw_cli()
    {
        Given_OctalWord("764000");
        AssertCode(       // cli
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = 0<18>");
    }


    [Test]
    public void Pdp1Rw_nop()
    {
        Given_OctalWord("760000");
        AssertCode(       // nop
             "0|L--|000400(1): 1 instructions",
             "1|L--|nop");
    }

    [Test]
    public void Pdp1Rw_ppa()
    {
        Given_OctalWord("720005");
        AssertCode(       // ppa
            "0|L--|000400(1): 1 instructions",
            "1|L--|__punch_performated_tape_alphanumeric(io)");
    }

    [Test]
    public void Pdp1Rw_ppb()
    {
        Given_OctalWord("720006");
        AssertCode(       // ppb
            "0|L--|000400(1): 1 instructions",
            "1|L--|__punch_performated_tape_binary(io)");
    }

    [Test]
    public void Pdp1Rw_cbs()
    {
        Given_OctalWord("720056");
        AssertCode(       // cbs
            "0|L--|000400(1): 1 instructions",
            "1|L--|__clear_sequence_break_system()");
    }

    [Test]
    public void Pdp1Rw_cks()
    {
        Given_OctalWord("720033");
        AssertCode(       // cks
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = __check_IO_status()");
    }

    [Test]
    public void Pdp1Rw_law_neg()
    {
        Given_OctalWord("711234");
        AssertCode(       // law
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = 0x3FD64<18>");
    }

    [Test]
    public void Pdp1Rw_law_pos()
    {
        Given_OctalWord("701234");
        AssertCode(       // law\t001234
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = 0x29C<18>");
    }

    [Test]
    public void Pdp1Rw_lio()
    {
        Given_OctalWord("226666");
        AssertCode(       // lio\t006666
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = Mem0[0o006666<p18>:word18]");
    }

    [Test]
    public void Pdp1Rw_lio_i()
    {
        Given_OctalWord("234321`");
        AssertCode(       // lio.i\t004321
            "0|L--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o004321<p18>:word18]",
            "2|L--|io = Mem0[v3:word18]");
    }

    [Test]
    public void Pdp1Rw_mul()
    {
        Given_OctalWord("540030");
        AssertCode(       // mul\t000030
            "0|L--|000400(1): 2 instructions",
            "1|L--|v4 = Mem0[0o000030<p18>:word18]",
            "2|L--|acc_io = acc *36 v4");
    }

    [Test]
    public void Pdp1Rw_mul_i()
    {
        Given_OctalWord("550030");
        AssertCode(       // mul.i\t000030
            "0|L--|000400(1): 3 instructions",
            "1|L--|v4 = Mem0[0o000030<p18>:word18]",
            "2|L--|v4 = Mem0[v4:word18]",
            "3|L--|acc_io = acc *36 v4");
    }

    [Test]
    public void Pdp1Rw_ral()
    {
        Given_OctalWord("661100");
        AssertCode(       // ral\t01
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = __rol<word18,byte>(acc, 1<8>)");
    }

    [Test]
    public void Pdp1Rw_rar()
    {
        Given_OctalWord("671777");
        AssertCode(       // rar\t09
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = __ror<word18,byte>(acc, 9<8>)");
    }

    [Test]
    public void Pdp1Rw_rcl()
    {
        Given_OctalWord("663300");
        AssertCode(       // rcl\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc_io = __rol<word36,byte>(acc_io, 2<8>)");
    }

    [Test]
    public void Pdp1Rw_rcr()
    {
        Given_OctalWord("673300");
        AssertCode(       // rcr\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc_io = __ror<word36,byte>(acc_io, 2<8>)");
    }

    [Test]
    public void Pdp1Rw_ril()
    {
        Given_OctalWord("662020");
        AssertCode(       // ril\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = __rol<word18,byte>(io, 1<8>)");
    }

    [Test]
    public void Pdp1Rw_rir()
    {
        Given_OctalWord("672000");
        AssertCode(       // rir\t01
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = __ror<word18,byte>(io, 0<8>)");
    }

    [Test]
    public void Pdp1Rw_rpa()
    {
        Given_OctalWord("720001");
        AssertCode(       // rpa
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = __read_performated_tape_alphanumeric()");
    }

    [Test]
    public void Pdp1Rw_rpb()
    {
        Given_OctalWord("720002");
        AssertCode(       // rpb
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = __read_performated_tape_binary()");
    }

    [Test]
    public void Pdp1Rw_rrb()
    {
        Given_OctalWord("720030");
        AssertCode(       // rrb
            "0|L--|000400(1): 1 instructions",
            "1|L--|__read_reader_buffer()");
    }

    [Test]
    public void Pdp1Rw_sad()
    {
        Given_OctalWord("501212");
        AssertCode(       // sad\t001212
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc != Mem0[0o001212<p18>:word18]) branch 000402");
    }

    [Test]
    public void Pdp1Rw_sad_i()
    {
        Given_OctalWord("511234");
        AssertCode(       // sad.i\t001234
            "0|T--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o001234<p18>:word18]",
            "2|T--|if (acc != Mem0[v3:word18]) branch 000402");
    }

    [Test]
    public void Pdp1Rw_sal()
    {
        Given_OctalWord("665500");
        AssertCode(       // sal\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = acc << 2<8>");
    }

    [Test]
    public void Pdp1Rw_sar()
    {
        Given_OctalWord("675500");
        AssertCode(       // sar\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc = acc >>u 2<8>");
    }

    [Test]
    public void Pdp1Rw_sas()
    {
        Given_OctalWord("521212");
        AssertCode(       // sas\t001212
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc == Mem0[0o001212<p18>:word18]) branch 000402");
    }

    [Test]
    public void Pdp1Rw_sas_i()
    {
        Given_OctalWord("534242");
        AssertCode(       // sas.i\t004242
            "0|T--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o004242<p18>:word18]",
            "2|T--|if (acc == Mem0[v3:word18]) branch 000402");
    }

    [Test]
    public void Pdp1Rw_scl()
    {
        Given_OctalWord("667700");
        AssertCode(       // scl\t03
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc_io = acc_io << 3<8>");
    }

    [Test]
    public void Pdp1Rw_scr()
    {
        Given_OctalWord("677700");
        AssertCode(       // scr\t03
            "0|L--|000400(1): 1 instructions",
            "1|L--|acc_io = acc_io >>u 3<8>");
    }

    [Test]
    public void Pdp1Rw_sil()
    {
        Given_OctalWord("666600");
        AssertCode(       // sil\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = io << 2<8>");
    }

    [Test]
    public void Pdp1Rw_sir()
    {
        Given_OctalWord("676600");
        AssertCode(       // sir\t02
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = io >>u 2<8>");
    }

    [Test]
    public void Pdp1Rw_sma()
    {
        Given_OctalWord("640400");
        AssertCode(       // sma
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc < 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_sma_i()
    {
        Given_OctalWord("650400");
        AssertCode(       // sma.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc >= 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_spa()
    {
        Given_OctalWord("640200");
        AssertCode(       // spa
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc >= 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_spa_i()
    {
        Given_OctalWord("650200");
        AssertCode(       // spa.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc < 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_spi()
    {
        Given_OctalWord("642000");
        AssertCode(       // spi
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (io >= 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_spi_i()
    {
        Given_OctalWord("652000");
        AssertCode(       // spi.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (io < 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_stf()
    {
        Given_OctalWord("760013");
        AssertCode(       // stf\t03
            "0|L--|000400(1): 1 instructions",
            "1|L--|__set_program_flag(3<8>)");
    }

    [Test]
    public void Pdp1Rw_sub()
    {
        Given_OctalWord("420030");
        AssertCode(       // sub\t000030
            "0|L--|000400(1): 3 instructions",
            "1|L--|v6 = Mem0[0o000030<p18>:word18]",
            "2|L--|acc = acc - v6",
            "3|L--|V = cond(acc)");
    }

    [Test]
    public void Pdp1Rw_sub_i()
    {
        Given_OctalWord("430030");
        AssertCode(       // sub.i\t000030
            "0|L--|000400(1): 4 instructions",
            "1|L--|v5 = Mem0[0o000030<p18>:ptr18]",
            "2|L--|v6 = Mem0[v5:word18]",
            "3|L--|acc = acc - v6",
            "4|L--|V = cond(acc)");
    }

    [Test]
    public void Pdp1Rw_sza()
    {
        Given_OctalWord("640100");
        AssertCode(       // sza
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc == 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_sza_i()
    {
        Given_OctalWord("650100");
        AssertCode(       // sza.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (acc != 0<18>) branch 000402");
    }

    [Test]
    public void Pdp1Rw_szf()
    {
        Given_OctalWord("640007");
        AssertCode(       // szf
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (!__get_sense_switch(7<8>)) branch 000402");
    }

    [Test]
    public void Pdp1Rw_szf_i()
    {
        Given_OctalWord("650000");
        AssertCode(       // szf.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (__get_sense_switch(0<8>)) branch 000402");
    }

    [Test]
    public void Pdp1Rw_szo()
    {
        Given_OctalWord("641000");
        AssertCode(       // szo
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (Test(NO,V)) branch 000402");
    }

    [Test]
    public void Pdp1Rw_szo_i()
    {
        Given_OctalWord("651000");
        AssertCode(       // szo.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (Test(OV,V)) branch 000402");
    }

    [Test]
    public void Pdp1Rw_szs()
    {
        Given_OctalWord("640000");
        AssertCode(       // szs
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (!__get_sense_switch(0<8>)) branch 000402");
    }

    [Test]
    public void Pdp1Rw_szs_i()
    {
        Given_OctalWord("650000");
        AssertCode(       // szs.i
            "0|T--|000400(1): 1 instructions",
            "1|T--|if (__get_sense_switch(0<8>)) branch 000402");
    }

    [Test]
    public void Pdp1Rw_tyi()
    {
        Given_OctalWord("720004");
        AssertCode(       // tyi
            "0|L--|000400(1): 1 instructions",
            "1|L--|io = __type_in()");
    }

    [Test]
    public void Pdp1Rw_tyo()
    {
        Given_OctalWord("730003");
        AssertCode(       // tyo
            "0|L--|000400(1): 1 instructions",
            "1|L--|__type_out(io)");
    }

    [Test]
    public void Pdp1Rw_xct()
    {
        Given_OctalWord("104242");
        AssertCode(       // xct\t004242
            "0|L--|000400(1): 1 instructions",
            "1|L--|__execute(0o004242<p18>)");
    }

    [Test]
    public void Pdp1Rw_xct_i()
    {
        Given_OctalWord("114242");
        AssertCode(       // xct.i\t004242
            "0|L--|000400(1): 2 instructions",
            "1|L--|v3 = Mem0[0o004242<p18>:word18]",
            "2|L--|__execute(v3)");
    }

    [Test]
    public void Pdp1Rw_xor()
    {
        Given_OctalWord("060100");
        AssertCode(       // xor\t000100
            "0|L--|000400(1): 2 instructions",
            "1|L--|v4 = Mem0[0o000100<p18>:word18]",
            "2|L--|acc = acc ^ v4");
    }

    [Test]
    public void Pdp1Rw_xor_i()
    {
        Given_OctalWord("075000");
        AssertCode(       // xor.i\t005000
            "0|L--|000400(1): 3 instructions",
            "1|L--|v4 = Mem0[0o005000<p18>:word18]",
            "2|L--|v4 = Mem0[v4:word18]",
            "3|L--|acc = acc ^ v4");
    }
}
