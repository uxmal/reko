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
using Reko.Arch.Maxim;
using Reko.Core;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch.Maxim;

public class MaxqDisassemblerTests : DisassemblerTestBase<MaxqInstruction>
{
    private readonly Address addr;
    private readonly MaxqArchitecture arch;

    public MaxqDisassemblerTests()
    {
        this.arch = new MaxqArchitecture(CreateServiceContainer(), "maxq", []);
        this.addr = Address.Ptr16(0x100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    private void AssertCode(string asm, string hexBytes)
    {
        byte[] bytes = BytePattern.FromHexBytes(hexBytes);
        var mem = Word16MemoryArea.CreateFromBeBytes(addr, bytes);
        var instr = base.Disassemble(mem);
        Assert.AreEqual(asm, instr.ToString());
    }

    [Test]
    public void MaxqDis_add_acc_a()
    {
        AssertCode("add	acc,a[3]", "ca39");
    }

    [Test]
    public void MaxqDis_add_acc_imm()
    {
        AssertCode("add	acc,#01", "4a01");
    }

    [Test]
    public void MaxqDis_add()
    {
        AssertCode("add	acc,a[2]", "ca29");
    }


    [Test]
    public void MaxqDis_add_acc_acc()
    {
        AssertCode("add	acc,acc", "ca0a");
    }

    [Test]
    public void MaxqDis_add_ap()
    {
        AssertCode("add	acc,a[ap]", "ca1a");
    }

    [Test]
    public void MaxqDis_addc_a()
    {
        AssertCode("addc\tacc,a[6]", "EA69");
    }

    [Test]
    public void MaxqDis_addc_imm()
    {
        AssertCode("addc	acc,#1A", "6a1a");
    }

    [Test]
    public void MaxqDis_and_imm()
    {
        AssertCode("and	acc,#02", "1a02");
    }

    [Test]
    public void MaxqDis_and()
    {
        AssertCode("and	acc,a[7]", "9a79");
    }

    [Test]
    public void MaxqDis_and_acc()
    {
        AssertCode("and	acc,#00", "1a00");
    }

    [Test]
    public void MaxqDis_cmp_m()
    {
        AssertCode("cmp	m3[0]", "f803");
    }

    [Test]
    public void MaxqDis_cmp_imm()
    {
        AssertCode("cmp	#11", "7811");
    }


    [Test]
    public void MaxqDis_cpl()
    {
        AssertCode("cpl	acc", "8a1a");
    }


    [Test]
    public void MaxqDis_lcall()
    {
        AssertCode("lcall	#86", "3d86");
    }


    [Test]
    public void MaxqDis_ldjnz()
    {
        AssertCode("ldjnz	lc[1],#3B", "5d3b");
    }


    [Test]
    public void MaxqDis_ljump()
    {
        AssertCode("ljump	#8E", "0c8e");
    }

    [Test]
    public void MaxqDis_ljump_ne()
    {
        AssertCode("ljump	ne,#49", "7c49");
    }

    [Test]
    public void MaxqDis_ljump_z()
    {
        AssertCode("ljump	z,#A9", "1ca9");
    }

    [Test]
    public void MaxqDis_ljump_e()
    {
        AssertCode("ljump	e,#9A", "3c9a");
    }


    [Test]
    public void MaxqDis_ljump_nc()
    {
        AssertCode("ljump	nc,#DC", "6cdc");
    }

    [Test]
    public void MaxqDis_ljump_c()
    {
        AssertCode("ljump	c,#C2", "2cc2");
    }


    [Test]
    public void MaxqDis_move_pfx_imm()
    {
        AssertCode("move	pfx[2],#00", "2b00");
    }

    [Test]
    [Ignore("Need PFX handling/stateful disassembly")]
    public void MaxqDis_move_sc()
    {
        AssertCode("move	sc,m7[1]", "2b00 8817");
    }

    [Test]
    public void MaxqDis_move_offs()
    {
        AssertCode("move	offs,#00", "3e00");
    }

    [Test]
    public void MaxqDis_move_dp()
    {
        AssertCode("move	dp[0],#00", "3f00");
    }


    [Test]
    public void MaxqDis_move()
    {
        AssertCode("move	a[0],#00", "0900");
    }

    [Test]
    public void MaxqDis_move_ap()
    {
        AssertCode("move	ap,#00", "0800");
    }

    [Test]
    public void MaxqDis_move_ic()
    {
        AssertCode("move	ic,m7[10]", "d8a7");
    }

    [Test]
    public void MaxqDis_move_apc()
    {
        AssertCode("move	apc,#00", "1800");
    }

    [Test]
    public void MaxqDis_move_sp()
    {
        AssertCode("move	sp,#00", "1d00");
    }

    [Test]
    public void MaxqDis_move_iv()
    {
        AssertCode("move	iv,#00", "2d00");
    }

    [Test]
    public void MaxqDis_move_preinc()
    {
        AssertCode("move	@++dp[1],#32", "5f32");
    }

    [Test]
    public void MaxqDis_move_lc()
    {
        AssertCode("move	lc[0],#A8", "6da8");
    }

    [Test]
    public void MaxqDis_move_postdec()
    {
        AssertCode("move	acc,@dp[1]--", "8a6f");
    }

    [Test]
    public void MaxqDis_move_bp_offs()
    {
        AssertCode("move	@bp[offs],#00", "0e00");
    }

    [Test]
    public void MaxqDis_move_a_grxl()
    {
        AssertCode("move	a[7],grxl", "f9ae");
    }


    [Test]
    [Ignore("Docs unclear")]
    public void MaxqDis_move_m()
    {
        AssertCode("move	m1[0],#18", "0118");
    }

    [Test]
    public void MaxqDis_move_grl()
    {
        AssertCode("move	grl,#49", "6e49");
    }

    [Test]
    public void MaxqDis_move_psf()
    {
        AssertCode("move	psf,#43", "4843");
    }

    [Test]
    public void MaxqDis_move_atdp()
    {
        AssertCode("move	@dp[1],#20", "4f20");
    }

    [Test]
    public void MaxqDis_move_acc()
    {
        AssertCode("move	acc,#64", "0a64");
    }

    [Test]
    public void MaxqDis_neg()
    {
        AssertCode("neg	acc", "8a9a");
    }

    [Test]
    public void MaxqDis_nop()
    {
        AssertCode("nop", "da3a");
    }


    [Test]
    public void MaxqDis_or_acc_imm()
    {
        AssertCode("or	acc,#AB", "2aab");
    }

    [Test]
    public void MaxqDis_or()
    {
        AssertCode("or	acc,a[6]", "aa69");
    }


    [Test]
    public void MaxqDis_or_grxl()
    {
        AssertCode("or	acc,grxl", "aaae");
    }

    [Test]
    public void MaxqDis_pop_a()
    {
        AssertCode("pop	a[0]", "890d");
    }

    [Test]
    public void MaxqDis_pop_dp()
    {
        AssertCode("pop	dp[0]", "bf0d");
    }

    [Test]
    public void MaxqDis_pop_preinc()
    {
        AssertCode("pop	@++dp[1]", "df0d");
    }

    [Test]
    public void MaxqDis_pop_offs()
    {
        AssertCode("pop	offs", "be0d");
    }

    [Test]
    public void MaxqDis_pop_bp()
    {
        AssertCode("pop	bp", "fe0d");
    }

    [Test]
    [Ignore("Docs unclear")]
    public void MaxqDis_pop_mcnt()
    {
        AssertCode("pop	mcnt", "820d");
    }

    [Test]
    public void MaxqDis_pop_atbp()
    {
        AssertCode("pop	@bp[offs]", "8e0d");
    }

    [Test]
    public void MaxqDis_pop_predec()
    {
        AssertCode("pop	@++dp[1]", "df0d");
    }


    [Test]
    public void MaxqDis_pop_lc()
    {
        AssertCode("pop	lc[1]", "fd0d");
    }

    [Test]
    public void MaxqDis_pop_ap()
    {
        AssertCode("pop	ap", "880d");
    }

    [Test]
    public void MaxqDis_pop_apc()
    {
        AssertCode("pop	apc", "980d");
    }

    [Test]
    public void MaxqDis_push_offs()
    {
        AssertCode("push	offs", "8d3e");
    }

    [Test]
    public void MaxqDis_push_psf()
    {
        AssertCode("push	psf", "8d48");
    }

    [Test]
    public void MaxqDis_push_apc()
    {
        AssertCode("push	apc", "8d18");
    }

    [Test]
    public void MaxqDis_push_ap()
    {
        AssertCode("push	ap", "8d08");
    }

    [Test]
    public void MaxqDis_push_a()
    {
        AssertCode("push	a[4]", "8d49");
    }

    [Test]
    public void MaxqDis_push_lc()
    {
        AssertCode("push	lc[1]", "8d7d");
    }

    [Test]
    public void MaxqDis_push_dp()
    {
        AssertCode("push	dp[0]", "8d3f");
    }

    [Test]
    public void MaxqDis_push_atbp()
    {
        AssertCode("push	@bp[offs]", "8d0e");
    }

    [Test]
    [Ignore("Docs unclear")]
    public void MaxqDis_push_mcnt()
    {
        AssertCode("push	mcnt", "8d02");
    }

    [Test]
    public void MaxqDis_push_m2()
    {
        AssertCode("push	m2[8]", "8d82");
    }

    [Test]
    public void MaxqDis_push_imm()
    {
        AssertCode("push	#00", "0d00");
    }

    [Test]
    public void MaxqDis_ret()
    {
        AssertCode("ret", "8c0d");
    }

    [Test]
    public void MaxqDis_reti()
    {
        AssertCode("reti", "8c8d");
    }


    [Test]
    public void MaxqDis_ret_c()
    {
        AssertCode("ret	c", "ac0d");
    }

    [Test]
    public void MaxqDis_rr()
    {
        AssertCode("rr	acc", "8aca");
    }

    [Test]
    public void MaxqDis_scall_a()
    {
        AssertCode("scall	a[7]", "bd79");
    }

    [Test]
    public void MaxqDis_scall_imm()
    {
        AssertCode("lcall	#65", "3d65");
    }

    [Test]
    public void MaxqDis_scall_m()
    {
        AssertCode("scall	m1[3]", "bd31");
    }

    [Test]
    public void MaxqDis_sdjnz()
    {
        AssertCode("ldjnz	lc[0],#FD", "4dfd");
    }

    [Test]
    public void MaxqDis_sjump_z()
    {
        AssertCode("ljump	z,#0E", "1c0e");
    }

    [Test]
    public void MaxqDis_sjump()
    {
        AssertCode("ljump	#05", "0c05");
    }

    [Test]
    public void MaxqDis_sjump_c()
    {
        AssertCode("ljump	c,#64", "2c64");
    }

    [Test]
    public void MaxqDis_sjump_nc()
    {
        AssertCode("ljump	nc,#61", "6c61");
    }

    [Test]
    public void MaxqDis_sjump_e()
    {
        AssertCode("ljump	e,#2C", "3c2c");
    }

    [Test]
    public void MaxqDis_sjump_ne()
    {
        AssertCode("ljump	ne,#06", "7c06");
    }

    [Test]
    public void MaxqDis_sjump_s()
    {
        AssertCode("ljump	s,#EC", "4cec");
    }

    [Test]
    public void MaxqDis_sla()
    {
        AssertCode("sla	acc", "8a2a");
    }

    [Test]
    public void MaxqDis_sla2()
    {
        AssertCode("sla2	acc", "8a3a");
    }

    [Test]
    public void MaxqDis_sla4()
    {
        AssertCode("sla4	acc", "8a6a");
    }

    [Test]
    public void MaxqDis_sr()
    {
        AssertCode("sr	acc", "8aaa");
    }

    [Test]
    public void MaxqDis_sra()
    {
        AssertCode("sra	acc", "8afa");
    }

    [Test]
    public void MaxqDis_sra2()
    {
        AssertCode("sra2	acc", "8aea");
    }

    [Test]
    public void MaxqDis_sra4()
    {
        AssertCode("sra4	acc", "8aba");
    }

    [Test]
    public void MaxqDis_sub_imm()
    {
        AssertCode("sub	acc,#01", "5a01");
    }

    [Test]
    public void MaxqDis_sub_a()
    {
        AssertCode("sub	acc,a[5]", "da59");
    }

    [Test]
    public void MaxqDis_sub_grl()
    {
        AssertCode("sub	acc,grl", "da6e");
    }

    [Test]
    public void MaxqDis_sub_m()
    {
        AssertCode("sub	acc,m1[9]", "da91");
    }

    [Test]
    public void MaxqDis_sub_grxl()
    {
        AssertCode("sub	acc,grxl", "daae");
    }

    [Test]
    [Ignore("Docs unclear")]
    public void MaxqDis_sub_ma()
    {
        AssertCode("sub	acc,ma", "da12");
    }

    [Test]
    [Ignore("Docs unclear")]

    public void MaxqDis_sub_mb()
    {
        AssertCode("sub	acc,mb", "da22");
    }

    [Test]
    [Ignore("Docs unclear")]
    public void MaxqDis_sub_mc0()
    {
        AssertCode("sub	acc,mc0", "da42");
    }

    [Test]
    public void MaxqDis_sub_dp()
    {
        AssertCode("sub	acc,dp[0]", "da3f");
    }

    [Test]
    public void MaxqDis_subb_m()
    {
        AssertCode("subb	acc,m0[0]", "fa00");
    }

    [Test]
    public void MaxqDis_subb_a()
    {
        AssertCode("subb	acc,a[0]", "fa09");
    }

    [Test]
    public void MaxqDis_subb_imm()
    {
        AssertCode("subb	acc,#00", "7a00");
    }

    [Test]
    public void MaxqDis_xch()
    {
        AssertCode("xch	acc", "8a8a");
    }

    [Test]
    public void MaxqDis_xor_imm()
    {
        AssertCode("xor	acc,#4D", "3a4d");
    }

    [Test]
    public void MaxqDis_xor_bit()
    {
        AssertCode("xor	c,acc.<2>", "ba2a");
    }

    [Test]
    public void MaxqDis_xor()
    {
        AssertCode("xor	acc,a[1]", "ba19");
    }
}
/*
00094	8a9f	unknown
0009f	8a9f	unknown
000b9	899f	unknown
000cd	8abf	unknown
000f4	8ddf	unknown
000f8	8dfe	unknown
00705	fffe	unknown
007a4	fffe	unknown
007eb	cccd	unknown
007ec	c16c	unknown
007ed	999a	unknown
007f1	cccd	unknown
007f2	c12c	unknown
007f3	999a	unknown
007f7	999a	unknown
007fb	999a	unknown
00805	999a	unknown
00870	3837	unknown
00878	3837	unknown
00883	bf4a	unknown
0091e	a80d	unknown
00952	a80d	unknown
00b11	a80d	unknown
00b1a	a80d	unknown
00b23	a80d	unknown
00b2c	a80d	unknown
00b35	a80d	unknown
00c63	f89f	unknown
00c6a	e99f	unknown
00c8f	ca9f	unknown
00c94	f99f	unknown
00d17	d99f	unknown
00f9a	ca4a	unknown
00f9b	ca8a	unknown
00fa3	ca4a	unknown
00fa4	ca8a	unknown
00fb5	ca4a	unknown
00fb6	ca8a	unknown
00fbe	ca4a	unknown
00fbf	ca8a	unknown
00fd7	ca4a	unknown
00fd8	ca8a	unknown
00fe0	ca4a	unknown
00fe1	ca8a	unknown
00ff2	ca4a	unknown
00ff3	ca8a	unknown
00ffb	ca4a	unknown
00ffc	ca8a	unknown
01013	ca4a	unknown
01014	ca8a	unknown
0101c	ca4a	unknown
0101d	ca8a	unknown
0102e	ca4a	unknown
0102f	ca8a	unknown
01037	ca4a	unknown
01038	ca8a	unknown
0104f	ca4a	unknown
01050	ca8a	unknown
01058	ca4a	unknown
01059	ca8a	unknown
0106a	ca4a	unknown
0106b	ca8a	unknown
01073	ca4a	unknown
01074	ca8a	unknown
0108a	ca4a	unknown
0108b	ca8a	unknown
01093	ca4a	unknown
01094	ca8a	unknown
010a5	ca4a	unknown
010a6	ca8a	unknown
010ae	ca4a	unknown
010af	ca8a	unknown
010c0	ca4a	unknown
010c1	ca8a	unknown
010c9	ca4a	unknown
010ca	ca8a	unknown
010db	ca4a	unknown
010dc	ca8a	unknown
010e4	ca4a	unknown
010e5	ca8a	unknown
0119f	8e9f	unknown
011aa	ca4a	unknown
011ab	ca8a	unknown
011b1	bf9f	unknown
011b2	e99f	unknown
011ba	bf9f	unknown
011e5	8e9f	unknown
011f0	ca4a	unknown
011f1	ca8a	unknown
011f7	bf9f	unknown
011f8	e99f	unknown
01202	bf9f	unknown
0122f	f99f	unknown
01235	f99f	unknown
0125b	ca4a	unknown
0125c	ca8a	unknown
01269	ca9f	unknown
012c0	8e9f	unknown
012cc	8e9f	unknown
012d5	8e9f	unknown
012e2	8e9f	unknown
012e7	8e9f	unknown
01439	ca4a	unknown
0143a	ca8a	unknown
0143d	ca9f	unknown
0147f	f99f	unknown
014db	ca9f	unknown
0153a	ca9f	unknown
01576	f99f	unknown
0157b	f99f	unknown
0158d	f89f	unknown
0159e	f89f	unknown
015a8	8d9f	unknown
015ab	8d9f	unknown
015ae	8d9f	unknown
015b1	8d9f	unknown
015b4	8d9f	unknown
015b8	8d9f	unknown
015bb	8d9f	unknown
015be	8d9f	unknown
015c1	8d9f	unknown
015c4	8d9f	unknown
015f7	ca4a	unknown
015f8	ca8a	unknown
0160f	ca4a	unknown
01610	ca8a	unknown
0164a	ca4a	unknown
0164b	ca8a	unknown
01662	ca4a	unknown
01663	ca8a	unknown
0171c	da9f	unknown
0173a	da9f	unknown
017e8	bf9f	unknown
017f4	8e9f	unknown
01813	8e9f	unknown
0181c	8e9f	unknown
01824	8e9f	unknown
01829	8e9f	unknown
0182e	8e9f	unknown
01833	8e9f	unknown
018fe	ca4a	unknown
018ff	ca8a	unknown
01907	ca4a	unknown
01908	ca8a	unknown
0191b	ca4a	unknown
0191c	ca8a	unknown
01924	ca4a	unknown
01925	ca8a	unknown
0196c	ca4a	unknown
0196d	ca8a	unknown
01975	ca4a	unknown
01976	ca8a	unknown
01989	ca4a	unknown
0198a	ca8a	unknown
01992	ca4a	unknown
01993	ca8a	unknown
019d9	ca4a	unknown
019da	ca8a	unknown
019e2	ca4a	unknown
019e3	ca8a	unknown
019f6	ca4a	unknown
019f7	ca8a	unknown
019ff	ca4a	unknown
01a00	ca8a	unknown
01a46	ca4a	unknown
01a47	ca8a	unknown
01a4f	ca4a	unknown
01a50	ca8a	unknown
01a63	ca4a	unknown
01a64	ca8a	unknown
01a6c	ca4a	unknown
01a6d	ca8a	unknown
01ab4	ca4a	unknown
01ab5	ca8a	unknown
01abd	ca4a	unknown
01abe	ca8a	unknown
01ad1	ca4a	unknown
01ad2	ca8a	unknown
01ada	ca4a	unknown
01adb	ca8a	unknown
01b1b	ca4a	unknown
01b1c	ca8a	unknown
01b24	ca4a	unknown
01b25	ca8a	unknown
01b38	ca4a	unknown
01b39	ca8a	unknown
01b41	ca4a	unknown
01b42	ca8a	unknown
01b72	ca4a	unknown
01b73	ca8a	unknown
01b7b	ca4a	unknown
01b7c	ca8a	unknown
01b8f	ca4a	unknown
01b90	ca8a	unknown
01b98	ca4a	unknown
01b99	ca8a	unknown
01bac	ca4a	unknown
01bad	ca8a	unknown
01bb5	ca4a	unknown
01bb6	ca8a	unknown
01bc9	ca4a	unknown
01bca	ca8a	unknown
01bd2	ca4a	unknown
01bd3	ca8a	unknown
01be5	ca4a	unknown
01be6	ca8a	unknown
01bee	ca4a	unknown
01bef	ca8a	unknown
01c02	ca4a	unknown
01c03	ca8a	unknown
01c0b	ca4a	unknown
01c0c	ca8a	unknown
01c1e	ca4a	unknown
01c1f	ca8a	unknown
01c27	ca4a	unknown
01c28	ca8a	unknown
01c3b	ca4a	unknown
01c3c	ca8a	unknown
01c44	ca4a	unknown
01c45	ca8a	unknown
01c56	ca4a	unknown
01c57	ca8a	unknown
01c5f	ca4a	unknown
01c60	ca8a	unknown
01c73	ca4a	unknown
01c74	ca8a	unknown
01c7c	ca4a	unknown
01c7d	ca8a	unknown
01c89	ca4a	unknown
01c8a	ca8a	unknown
01c92	ca4a	unknown
01c93	ca8a	unknown
01ca6	ca4a	unknown
01ca7	ca8a	unknown
01caf	ca4a	unknown
01cb0	ca8a	unknown
01cdc	ca4a	unknown
01ce8	ca9f	unknown
01d05	ca8a	unknown
01d09	ca8a	unknown
01d5c	ca5a	unknown
01d65	ca3a	unknown
01dde	ca6a	unknown
01df1	ca6a	unknown
01f08	da9f	unknown
01f42	ca4a	unknown
01f53	ca4a	unknown
01f55	ca4a	unknown
01f56	ca8a	unknown
01f5d	ca4a	unknown
01f5f	ca4a	unknown
01f60	ca8a	unknown
01f76	a80d	unknown
01f77	a80d	unknown
01f78	a80d	unknown
01f79	a80d	unknown
02078	a80d	unknown
02081	a80d	unknown
02143	8d9f	unknown
02149	a80d	unknown
02174	8d9f	unknown
02179	a80d	unknown
0217a	a80d	unknown
02191	8d9f	unknown
02196	a80d	unknown
02197	a80d	unknown
021ac	8d9f	unknown
021b1	a80d	unknown
021b2	a80d	unknown
02232	ca9f	unknown
02236	ca9f	unknown
0224a	8e9f	unknown
02270	bf9f	unknown
0227e	bf9f	unknown
0227f	e99f	unknown
0228a	ca4a	unknown
0228b	ca8a	unknown
0228e	bf9f	unknown
0228f	e99f	unknown
022a1	ca3a	unknown
022a8	bf9f	unknown
022a9	e99f	unknown
022b0	ca4a	unknown
022b1	ca8a	unknown
022b6	bf9f	unknown
022b7	e99f	unknown
022c2	ca4a	unknown
022c3	ca8a	unknown
022c9	bf9f	unknown
022ca	e99f	unknown
022d5	bf9f	unknown
022e2	bf9f	unknown
022e3	e99f	unknown
022f9	bf9f	unknown
022fa	e99f	unknown
0230d	bf9f	unknown
0230e	e99f	unknown
02318	ca3a	unknown
02319	ca4a	unknown
0231a	ca8a	unknown
0231d	bf9f	unknown
0231e	e99f	unknown
0232e	bf9f	unknown
0232f	e99f	unknown
02339	ca3a	unknown
0233a	ca4a	unknown
0233b	ca8a	unknown
02340	bf9f	unknown
02341	e99f	unknown
0234c	ca5a	unknown
0234f	bf9f	unknown
0235b	bf9f	unknown
0235c	e99f	unknown
0236d	ca4a	unknown
0236e	ca8a	unknown
02373	bf9f	unknown
02374	e99f	unknown
0237f	bf9f	unknown
0238c	bf9f	unknown
0238d	e99f	unknown
023a3	bf9f	unknown
023a4	e99f	unknown
023b7	bf9f	unknown
023b8	e99f	unknown
023c2	ca3a	unknown
023c3	ca4a	unknown
023c4	ca8a	unknown
023c7	bf9f	unknown
023c8	e99f	unknown
023d8	bf9f	unknown
023d9	e99f	unknown
023e3	ca3a	unknown
023e4	ca4a	unknown
023e5	ca8a	unknown
023ea	bf9f	unknown
023eb	e99f	unknown
023f6	ca5a	unknown
023f9	bf9f	unknown
02405	bf9f	unknown
02406	e99f	unknown
024a5	a80d	unknown
024ae	a80d	unknown
025d4	ca7a	unknown
0260a	ca7a	unknown
0273a	ca4a	unknown
0273b	ca8a	unknown
02787	899f	unknown
0278c	ca6a	unknown
02791	ca6a	unknown
02795	ca6a	unknown
0279b	ca6a	unknown
0279e	ca6a	unknown
027a3	ca6a	unknown
027a7	ca6a	unknown
027aa	ca6a	unknown
027ad	ca6a	unknown
027b2	ca6a	unknown
027b6	ca6a	unknown
027ba	ca6a	unknown
027bf	ca6a	unknown
027c3	ca6a	unknown
027c8	899f	unknown
027cd	ca6a	unknown
027d1	ca6a	unknown
027d5	ca6a	unknown
027d9	ca6a	unknown
027dd	ca6a	unknown
027e2	ca6a	unknown
027e5	ca6a	unknown
027e8	ca6a	unknown
027ed	ca6a	unknown
027f1	ca6a	unknown
027f4	ca6a	unknown
027f9	ca6a	unknown
027fd	ca6a	unknown
02803	ca6a	unknown
02822	ed9f	unknown
02827	8e9f	unknown
0282a	be9f	unknown
0282e	fe9f	unknown
02831	899f	unknown
02859	ed9f	unknown
0285e	8e9f	unknown
02861	be9f	unknown
02865	fe9f	unknown
02868	899f	unknown
02894	ed9f	unknown
02899	8e9f	unknown
0289c	be9f	unknown
028a0	fe9f	unknown
028a3	899f	unknown
028cf	ed9f	unknown
028d4	8e9f	unknown
028d7	be9f	unknown
028da	899f	unknown
02900	ed9f	unknown
02905	8e9f	unknown
02908	be9f	unknown
0290b	899f	unknown
02931	ed9f	unknown
02936	8e9f	unknown
02939	be9f	unknown
0293c	899f	unknown
02962	ed9f	unknown
02967	8e9f	unknown
0296a	be9f	unknown
02971	899f	unknown
0299d	ed9f	unknown
029a2	8e9f	unknown
029a5	be9f	unknown
029ac	899f	unknown
02a48	ca6a	unknown
02a49	e9bf	unknown
02a4a	ca6a	unknown
02a4c	ca6a	unknown
02a5c	89df	unknown
02a7f	8dfe	unknown
02a8e	b98f	unknown
02aaf	ca6a	unknown
02ab3	ca6a	unknown
02ab6	ca8a	unknown
02ac7	e98f	unknown
02acb	c99f	unknown
02ade	ca4a	unknown
02ae0	ca4a	unknown
02ae3	ca5a	unknown
02ae5	ca5a	unknown
02ae9	ca8a	unknown
02b06	899f	unknown
02b0c	899f	unknown
02bdb	ca4a	unknown
02bdc	ca8a	unknown
02c5c	da9f	unknown
02d51	a80d	unknown
02d5a	a80d	unknown
02da3	ca8a	unknown
02dad	ca8a	unknown
02dbd	ca8a	unknown
02ded	ca4a	unknown
02e57	ca8a	unknown
02e64	8e9f	unknown
02e82	ca4a	unknown
02e83	ca8a	unknown
02e8e	8e9f	unknown
02f76	ca4a	unknown
02f77	ca8a	unknown
0301e	ca4a	unknown
03036	ca4a	unknown
03037	ca8a	unknown
03060	ca4a	unknown
03061	ca8a	unknown
0325e	a80d	unknown
0326f	a80d	unknown
03280	a80d	unknown
03291	a80d	unknown
032cb	a80d	unknown
032db	a80d	unknown
032eb	a80d	unknown
032fb	a80d	unknown
03313	ca4a	unknown
03314	ca8a	unknown
03329	ca8a	unknown
0332d	ca8a	unknown
03341	ca5a	unknown
03348	ca3a	unknown
033ad	da9f	unknown
0346a	ca4a	unknown
034a6	ca4a	unknown
034ad	ca5a	unknown
034b4	ca4a	unknown
034bb	ca5a	unknown
034c2	ca4a	unknown
034c9	ca5a	unknown
034d0	ca4a	unknown
034d7	ca5a	unknown
03566	f99f	unknown
03580	ca4a	unknown
03587	ca5a	unknown
0358e	ca4a	unknown
03595	ca5a	unknown
0359c	ca4a	unknown
035a3	ca5a	unknown
035aa	ca4a	unknown
035b1	ca5a	unknown
03640	f99f	unknown
03653	ca4a	unknown
0369a	ca4a	unknown
036a5	e98f	unknown
036b8	ca4a	unknown
036bf	e99f	unknown
036c8	ca4a	unknown
036c9	ca8a	unknown
036cf	e99f	unknown
036ee	ca4a	unknown
036f5	ca5a	unknown
036fc	ca4a	unknown
03703	ca5a	unknown
0370a	ca4a	unknown
03711	ca5a	unknown
03718	ca4a	unknown
0371f	ca5a	unknown
03860	f99f	unknown
03888	f99f	unknown
038b0	f99f	unknown
038df	ca4a	unknown
03920	ca4a	unknown
0392b	e98f	unknown
0393e	ca4a	unknown
03945	e99f	unknown
0394e	ca4a	unknown
0394f	ca8a	unknown
03955	e99f	unknown
03964	ca4a	unknown
0396b	ca5a	unknown
03972	ca4a	unknown
03979	ca5a	unknown
03980	ca4a	unknown
03987	ca5a	unknown
0398e	ca4a	unknown
03995	ca5a	unknown
03a24	f99f	unknown
03ad4	ca4a	unknown
03add	ca4a	unknown
03ae5	ca4a	unknown
03b21	f8fb	unknown
03b25	f99f	unknown
03b7d	8e9f	unknown
03b85	bf9f	unknown
03b8c	8e9f	unknown
03b94	8e9f	unknown
03b9c	8e9f	unknown
03ba7	8e9f	unknown
03baf	8e9f	unknown
03bb7	8e9f	unknown
03bbf	8e9f	unknown
03bc7	8e9f	unknown
03bcf	8e9f	unknown
03bde	8e9f	unknown
03be4	8e9f	unknown
03bec	8e9f	unknown
03bf2	8e9f	unknown
03c05	8e9f	unknown
03c0a	8e9f	unknown
03c12	8e9f	unknown
03c1a	8e9f	unknown
03c21	8e9f	unknown
03c27	8e9f	unknown
03c2c	8e9f	unknown
03c34	8e9f	unknown
03c39	8e9f	unknown
03c50	a80d	unknown
03c57	ca4a	unknown
03c8a	da9f	unknown
03c96	ca6a	unknown
03c9e	da9f	unknown
03caa	ca9f	unknown
03cab	ca6a	unknown
03cea	ca9f	unknown
03ceb	ca6a	unknown
03d0d	da9f	unknown
03d1c	da9f	unknown
03d36	ca6a	unknown
03d41	ca6a	unknown
03d44	e99f	unknown
03d59	ca6a	unknown
03d5c	ca6a	unknown
03d64	ca6a	unknown
03d65	f8fb	unknown
03d69	ca6a	unknown
03d84	ca6a	unknown
03ddf	8e9f	unknown
03dec	8e9f	unknown
03df1	8e9f	unknown
03df6	8e9f	unknown
03e04	8e9f	unknown
03e11	8e9f	unknown
03e1e	8e9f	unknown
03e2b	8e9f	unknown
03e50	8e9f	unknown
03eba	ca4a	unknown
03ee6	da9f	unknown
03eea	ca4a	unknown
03ef7	ca4a	unknown
03f1a	ca4a	unknown
03f32	f99f	unknown
03f71	8e9f	unknown
03f86	8e9f	unknown
03f92	8e9f	unknown
03f99	ca4a	unknown
03faf	ca4a	unknown
03fbc	ca4a	unknown
03fff	a80d	unknown
0400f	a80d	unknown
0401f	a80d	unknown
0402f	a80d	unknown
0404f	ca4a	unknown
0405c	ca4a	unknown
0405d	ca8a	unknown
0409c	ca4a	unknown
040a9	ca4a	unknown
040c3	ca4a	unknown
04121	ca4a	unknown
04122	ca8a	unknown
04143	8e9f	unknown
0414d	8e9f	unknown
04154	8e9f	unknown
0416d	8e9f	unknown
041a7	8e9f	unknown
041ad	8e9f	unknown
041b4	ca5a	unknown
041b7	8e9f	unknown
041c1	8e9f	unknown
041c8	8e9f	unknown
041cd	8e9f	unknown
041d5	bf9f	unknown
041dc	8e9f	unknown
041e4	8e9f	unknown
041ec	8e9f	unknown
041f7	8e9f	unknown
041ff	8e9f	unknown
04207	8e9f	unknown
0420f	8e9f	unknown
04217	8e9f	unknown
0423a	ca8a	unknown
0423c	ca8a	unknown
0425c	8e9f	unknown
04264	ca2a	unknown
04267	8e9f	unknown
0427c	ca8a	unknown
0427e	ca8a	unknown
04308	ca8a	unknown
0430a	ca8a	unknown
043e6	ca8a	unknown
043e8	ca8a	unknown
0442b	ca8a	unknown
0442d	ca8a	unknown
04454	ca8a	unknown
04456	ca8a	unknown
04473	a80d	unknown
04474	a80d	unknown
04475	a80d	unknown
0448c	f99f	unknown
04491	f99f	unknown
04496	f99f	unknown
044cf	ca8a	unknown
044f3	ca4a	unknown
0459d	da9f	unknown
045a6	e99f	unknown
045ac	f8fb	unknown
045b0	f99f	unknown
0460a	ca7a	unknown
04650	ca4a	unknown
046a8	ca4a	unknown
047ce	a80d	unknown
04840	a80d	unknown
04841	a80d	unknown
04842	a80d	unknown
04858	f99f	unknown
048f0	f99f	unknown
04978	ca7a	unknown
049b0	ca4a	unknown
049f8	ca4a	unknown
04b0b	f99f	unknown
04b19	f99f	unknown
04b51	e99f	unknown
04bbe	ca8a	unknown
04dd9	ca6a	unknown
04de2	ca6a	unknown
04df5	ca6a	unknown
04dff	ca6a	unknown
04e32	ca6a	unknown
04e3b	ca6a	unknown
04e49	ca6a	unknown
04e53	ca6a	unknown
04eef	f99f	unknown
04ef4	f99f	unknown
04f0c	a80d	unknown
04f0d	a80d	unknown
04f0e	a80d	unknown
05430	8e9f	unknown
05438	8e9f	unknown
05440	8e9f	unknown
05448	8e9f	unknown
05450	8e9f	unknown
05458	8e9f	unknown
0546d	8e9f	unknown
05475	8e9f	unknown
0547d	8e9f	unknown
0548e	ca9f	unknown
0595b	a80d	unknown
05966	ca5a	unknown
05b00	ca5a	unknown
05b61	a80d	unknown
05b63	a80d	unknown
05b7b	a80d	unknown
05b83	a80d	unknown
05b95	a80d	unknown
05bae	a80d	unknown
05bdd	a80d	unknown
05d2a	a80d	unknown
05d2b	a80d	unknown
05d2c	a80d	unknown
05d2d	a80d	unknown
05da1	ca9f	unknown
05dc0	ca9f	unknown
05de3	f99f	unknown
05e44	f8fb	unknown
05ecf	f99f	unknown
05efe	f99f	unknown
05f1c	a80d	unknown
05f1d	a80d	unknown
05f1e	a80d	unknown
0615b	f8fb	unknown
06170	ca4a	unknown
06171	ca8a	unknown
06174	ca4a	unknown
06181	ca4a	unknown
06182	ca8a	unknown
0618b	ca4a	unknown
06318	ca4a	unknown
0631a	ca4a	unknown
0631c	ca4a	unknown
0632d	ca7a	unknown
0636f	a80d	unknown
06563	f89f	unknown
06569	f89f	unknown
0656f	f89f	unknown
06579	f89f	unknown
06671	ca6a	unknown
06673	ca6a	unknown
06677	ca6a	unknown
06679	ca6a	unknown
06693	8dfe	unknown
06696	b9af	unknown
0669a	99af	unknown
0669e	a9af	unknown
066b6	89df	unknown
066bb	99df	unknown
066c9	a18f	unknown
066fa	89df	unknown
06705	c2df	unknown
06706	f9df	unknown
06728	8dfe	unknown
0672d	b9af	unknown
06731	99af	unknown
06735	a9af	unknown
0674d	89df	unknown
06752	99df	unknown
06788	a18f	unknown
067b9	89df	unknown
067c4	c2df	unknown
067c5	f9df	unknown
067f7	8ddf	unknown
06800	d9df	unknown
068c4	c9bf	unknown
068ce	ca3a	unknown
068dc	c9bf	unknown
068e7	ca3a	unknown
06916	999f	unknown
06976	c99f	unknown
0698d	c99f	unknown
06999	999f	unknown
069aa	e9bf	unknown
069af	e9bf	unknown
069b8	8ddf	unknown
069d9	8ddf	unknown
06a20	8ddf	unknown
06a45	8ddf	unknown
06a4f	9fbf	unknown
06a5e	8ddf	unknown
06a81	8ddf	unknown
06ad9	999f	unknown
06b77	a9af	unknown
06b7a	a9af	unknown
06b7e	a9bf	unknown
06b86	daaf	unknown
06b88	89bf	unknown
06b8a	899f	unknown
06b90	daaf	unknown
06b92	89bf	unknown
06b96	899f	unknown
06b9b	89bf	unknown
06ba0	899f	unknown
06bb4	caaa	unknown
06bc1	caaa	unknown
06bcf	caaa	unknown
06c0f	caaa	unknown
06c13	ca9a	unknown
06c14	caaa	unknown
06d19	ca9f	unknown
06d47	ca9f	unknown
06d74	8e9f	unknown
06dca	ca4a	unknown
06dcb	ca8a	unknown
06dce	aa9f	unknown
06dd5	ca4a	unknown
06dd6	ca8a	unknown
06dd9	aa9f	unknown
06e09	ca4a	unknown
06e0a	ca8a	unknown
06e3b	ca4a	unknown
06e3c	ca8a	unknown
06e49	ca8a	unknown
06e4b	ca8a	unknown
06e82	ca4a	unknown
06e83	ca8a	unknown
06e8b	ca4a	unknown
06e8c	ca8a	unknown
06e9f	ca4a	unknown
06ea0	ca8a	unknown
06ea8	ca4a	unknown
06ea9	ca8a	unknown
06ebc	ca4a	unknown
06ebd	ca8a	unknown
06ec5	ca4a	unknown
06ec6	ca8a	unknown
06ed9	ca4a	unknown
06eda	ca8a	unknown
06ee2	ca4a	unknown
06ee3	ca8a	unknown
06ef5	ca4a	unknown
06ef6	ca8a	unknown
06efe	ca4a	unknown
06eff	ca8a	unknown
06f12	ca4a	unknown
06f13	ca8a	unknown
06f1b	ca4a	unknown
06f1c	ca8a	unknown
06f2e	ca4a	unknown
06f2f	ca8a	unknown
06f37	ca4a	unknown
06f38	ca8a	unknown
06f4b	ca4a	unknown
06f4c	ca8a	unknown
06f54	ca4a	unknown
06f55	ca8a	unknown
06f66	ca4a	unknown
06f67	ca8a	unknown
06f6f	ca4a	unknown
06f70	ca8a	unknown
06f83	ca4a	unknown
06f84	ca8a	unknown
06f8c	ca4a	unknown
06f8d	ca8a	unknown
06f99	ca4a	unknown
06f9a	ca8a	unknown
06fa2	ca4a	unknown
06fa3	ca8a	unknown
06fb6	ca4a	unknown
06fb7	ca8a	unknown
06fbf	ca4a	unknown
06fc0	ca8a	unknown
07083	ca4a	unknown
07084	ca8a	unknown
0708c	ca4a	unknown
0708d	ca8a	unknown
070a0	ca4a	unknown
070a1	ca8a	unknown
070a9	ca4a	unknown
070aa	ca8a	unknown
070f1	ca4a	unknown
070f2	ca8a	unknown
070fa	ca4a	unknown
070fb	ca8a	unknown
0710e	ca4a	unknown
0710f	ca8a	unknown
07117	ca4a	unknown
07118	ca8a	unknown
0715e	ca4a	unknown
0715f	ca8a	unknown
07167	ca4a	unknown
07168	ca8a	unknown
0717b	ca4a	unknown
0717c	ca8a	unknown
07184	ca4a	unknown
07185	ca8a	unknown
071cb	ca4a	unknown
071cc	ca8a	unknown
071d4	ca4a	unknown
071d5	ca8a	unknown
071e8	ca4a	unknown
071e9	ca8a	unknown
071f1	ca4a	unknown
071f2	ca8a	unknown
07239	ca4a	unknown
0723a	ca8a	unknown
07242	ca4a	unknown
07243	ca8a	unknown
07256	ca4a	unknown
07257	ca8a	unknown
0725f	ca4a	unknown
07260	ca8a	unknown
072a0	ca4a	unknown
072a1	ca8a	unknown
072a9	ca4a	unknown
072aa	ca8a	unknown
072bd	ca4a	unknown
072be	ca8a	unknown
072c6	ca4a	unknown
072c7	ca8a	unknown
0742f	ca4a	unknown
07430	ca8a	unknown
07443	ca4a	unknown
07444	ca8a	unknown
07466	ca4a	unknown
07467	ca8a	unknown
07517	ca8a	unknown
07519	ca8a	unknown
07657	bf9f	unknown
07663	8e9f	unknown
0766b	8e9f	unknown
07670	8e9f	unknown
07675	8e9f	unknown
0767a	8e9f	unknown
076da	f99f	unknown
0773c	f99f	unknown
077d9	ca4a	unknown
077e4	ca9f	unknown
077ee	ca4a	unknown
077f9	ca9f	unknown
07821	ca8a	unknown
07825	ca8a	unknown
07840	ca5a	unknown
07848	ca3a	unknown
07888	a80d	unknown
07889	a80d	unknown
0794d	f99f	unknown
079e2	ca4a	unknown
079e3	ca8a	unknown
079eb	ca4a	unknown
079ec	ca8a	unknown
079fd	ca4a	unknown
079fe	ca8a	unknown
07a06	ca4a	unknown
07a07	ca8a	unknown
07a1f	ca4a	unknown
07a20	ca8a	unknown
07a28	ca4a	unknown
07a29	ca8a	unknown
07a3a	ca4a	unknown
07a3b	ca8a	unknown
07a43	ca4a	unknown
07a44	ca8a	unknown
07a5b	ca4a	unknown
07a5c	ca8a	unknown
07a64	ca4a	unknown
07a65	ca8a	unknown
07a76	ca4a	unknown
07a77	ca8a	unknown
07a7f	ca4a	unknown
07a80	ca8a	unknown
07a97	ca4a	unknown
07a98	ca8a	unknown
07aa0	ca4a	unknown
07aa1	ca8a	unknown
07ab2	ca4a	unknown
07ab3	ca8a	unknown
07abb	ca4a	unknown
07abc	ca8a	unknown
07ad2	ca4a	unknown
07ad3	ca8a	unknown
07adb	ca4a	unknown
07adc	ca8a	unknown
07aed	ca4a	unknown
07aee	ca8a	unknown
07af6	ca4a	unknown
07af7	ca8a	unknown
07b08	ca4a	unknown
07b09	ca8a	unknown
07b11	ca4a	unknown
07b12	ca8a	unknown
07b23	ca4a	unknown
07b24	ca8a	unknown
07b2c	ca4a	unknown
07b2d	ca8a	unknown
07bc7	e99f	unknown
07c14	f89f	unknown
07c23	f89f	unknown
07c31	f89f	unknown
07c38	f89f	unknown
07c7a	f99f	unknown
07cab	f99f	unknown
07cb2	f89f	unknown
07cbe	da9f	unknown
07ccd	ca9f	unknown
07ce2	da9f	unknown
07cf1	ca9f	unknown
07cfc	da9f	unknown
07d0f	da9f	unknown
07d25	da9f	unknown
07d30	ca9f	unknown
07d3d	da9f	unknown
07d5c	da9f	unknown
07d6d	da9f	unknown
07d76	da9f	unknown
07dc3	f99f	unknown
07dda	da9f	unknown
07de9	ca9f	unknown
07dff	da9f	unknown
07e0e	ca9f	unknown
07e19	da9f	unknown
07e2c	da9f	unknown
07e42	da9f	unknown
07e4d	ca9f	unknown
07e5a	da9f	unknown
07e7a	da9f	unknown
07e8b	da9f	unknown
07e94	da9f	unknown
07eb8	f99f	unknown
07ed1	da9f	unknown
07ee0	ca9f	unknown
07f03	da9f	unknown
07f12	ca9f	unknown
07f2d	da9f	unknown
07f3c	da9f	unknown
07f51	da9f	unknown
07f60	ca9f	unknown
07f76	da9f	unknown
07f84	da9f	unknown
07f9f	da9f	unknown
07fae	ca9f	unknown
07fbe	da9f	unknown
07fcd	da9f	unknown
07fed	da9f	unknown
07ffc	ca9f	unknown
08007	da9f	unknown
08015	da9f	unknown
0803b	da9f	unknown
08046	ca9f	unknown
08053	da9f	unknown
08084	f89f	unknown
08090	da9f	unknown
0809f	ca9f	unknown
080b4	da9f	unknown
080c3	ca9f	unknown
080ce	da9f	unknown
080e1	da9f	unknown
080f7	da9f	unknown
08102	ca9f	unknown
0810f	da9f	unknown
0812e	da9f	unknown
08145	da9f	unknown
0814f	da9f	unknown
0816e	da9f	unknown
08177	da9f	unknown
0819b	f89f	unknown
081a7	da9f	unknown
081b6	ca9f	unknown
081c8	da9f	unknown
081d7	ca9f	unknown
081e2	da9f	unknown
081f1	da9f	unknown
08203	da9f	unknown
0820e	ca9f	unknown
0821b	da9f	unknown
08237	da9f	unknown
08245	da9f	unknown
0824e	da9f	unknown
08269	f89f	unknown
08275	da9f	unknown
08280	ca9f	unknown
082a0	da9f	unknown
082ab	ca9f	unknown
082bf	da9f	unknown
082d2	da9f	unknown
082e9	da9f	unknown
082f4	ca9f	unknown
082ff	da9f	unknown
08311	da9f	unknown
08333	da9f	unknown
0833a	ca9f	unknown
08347	da9f	unknown
08350	ca9f	unknown
08373	da9f	unknown
08385	da9f	unknown
0838e	da9f	unknown
083b1	f89f	unknown
083bd	da9f	unknown
083c7	ca9f	unknown
083e7	da9f	unknown
083f1	ca9f	unknown
08406	da9f	unknown
08419	da9f	unknown
08430	da9f	unknown
08439	ca9f	unknown
08444	da9f	unknown
08456	da9f	unknown
08475	da9f	unknown
0847c	ca9f	unknown
08488	da9f	unknown
08491	ca9f	unknown
084b4	da9f	unknown
084c6	da9f	unknown
084cf	da9f	unknown
084f2	da9f	unknown
08501	ca9f	unknown
08516	da9f	unknown
08525	ca9f	unknown
08530	da9f	unknown
08543	da9f	unknown
08559	da9f	unknown
08564	ca9f	unknown
08571	da9f	unknown
08590	da9f	unknown
085a1	da9f	unknown
085aa	da9f	unknown
085f7	f99f	unknown
086ef	da9f	unknown
08703	ca9f	unknown
0876f	da9f	unknown
08783	ca9f	unknown
087e3	da9f	unknown
087ec	da9f	unknown
087f1	da9f	unknown
087fc	da9f	unknown
0880e	da9f	unknown
08815	da9f	unknown
0882b	da9f	unknown
0883c	da9f	unknown
08843	da9f	unknown
08854	da9f	unknown
08e23	8dfe	unknown
08e32	89df	unknown
08e38	89df	unknown
08e47	a18f	unknown
08ef9	89df	unknown
08f03	89df	unknown
08f0d	89df	unknown
08f1a	c2df	unknown
08f1b	f9df	unknown
08f3d	8dfe	unknown
08f4e	89df	unknown
08f54	89df	unknown
09032	89df	unknown
0903c	89df	unknown
09046	89df	unknown
09053	c2df	unknown
09054	f9df	unknown
09072	99ef	unknown
09073	ca4a	unknown
09074	ca7a	unknown
09075	898f	unknown
09076	ca4a	unknown
09077	ca7a	unknown
09087	ca7a	unknown
090a0	99ef	unknown
090a1	ca4a	unknown
090a2	ca7a	unknown
090a3	898f	unknown
090a4	ca4a	unknown
090a5	ca7a	unknown
090bd	8dfe	unknown
090d3	e98f	unknown
090d6	ca6a	unknown
090da	ca6a	unknown
090de	ca6a	unknown
090e2	ca6a	unknown
090e4	8ddf	unknown
090eb	ca4a	unknown
090ec	ca7a	unknown
090ef	ca5a	unknown
090f0	ca7a	unknown
090f4	ca8a	unknown
090f7	b9df	unknown
09110	8dfe	unknown
09124	e98f	unknown
09127	ca6a	unknown
0912b	ca6a	unknown
0912f	ca6a	unknown
09134	ca6a	unknown
09150	8dfe	unknown
09166	e98f	unknown
09169	ca6a	unknown
0916d	ca6a	unknown
09171	ca6a	unknown
09178	ca6a	unknown
09180	8ddf	unknown
09187	ca4a	unknown
09188	ca7a	unknown
0918b	ca5a	unknown
0918c	ca7a	unknown
09190	ca8a	unknown
09193	b9df	unknown
091b6	f98f	unknown
091b7	e9ef	unknown
091ba	ca6a	unknown
091bd	ca6a	unknown
091c2	ca6a	unknown
091c5	ca6a	unknown
091d3	8dfe	unknown
091dc	d9bf	unknown
09208	919f	unknown
09242	ca6a	unknown
0924b	ca6a	unknown
09280	ca6a	unknown
09426	8ddf	unknown
0944f	8ddf	unknown
09478	ca6a	unknown
0947a	ca6a	unknown
09483	8ddf	unknown
094b0	999f	unknown
0953f	ee9f	unknown
0956f	8ddf	unknown
09574	e99f	unknown
096a7	ca6a	unknown
096aa	ca6a	unknown
098cf	e99f	unknown
099a9	ca6a	unknown
099ac	ca6a	unknown
09adf	e99f	unknown
09b84	e99f	unknown
09ba0	e99f	unknown
09bdb	e99f	unknown
09c05	e99f	unknown
09c17	a19f	unknown
09daa	f8fb	unknown
09dae	f8fb	unknown
09db4	f8fb	unknown
09dbc	f8fb	unknown
09dc0	f8fb	unknown
09dc6	f8fb	unknown
09dd1	8a5a	unknown
09dea	8a5a	unknown
09dec	8a5a	unknown
09ded	8a5a	unknown
09e09	8a5a	unknown
09e42	f99f	unknown
09e56	f8fb	unknown
09edb	f8fb	unknown
09f0a	f8fb	unknown
09fa9	a80d	unknown
09faa	a80d	unknown
09fe4	bf9f	unknown
0a094	ca7a	unknown
0a0a6	f8fb	unknown
0a0cc	ca4a	unknown
0a170	f8fb	unknown
0a1b3	f8fb	unknown
0a1ef	a80d	unknown
0a317	ca5a	unknown
0a31e	ca5a	unknown
0a326	ca5a	unknown
0a35f	ca8a	unknown
0a361	ca8a	unknown
0a376	ca8a	unknown
0a378	ca8a	unknown
0a381	ca8a	unknown
0a383	ca8a	unknown
0a399	ca8a	unknown
0a39b	ca8a	unknown
0a3ba	ca5a	unknown
0a3c1	ca5a	unknown
0a3c9	ca5a	unknown
0a3f9	ca8a	unknown
0a3fb	ca8a	unknown
0a419	ca8a	unknown
0a41b	ca8a	unknown
0a431	ca4a	unknown
0a432	ca8a	unknown
0a460	ca5a	unknown
0a464	ca4a	unknown
0a467	ca5a	unknown
0a46b	ca4a	unknown
0a46e	ca4a	unknown
0a470	ca4a	unknown
0a474	ca4a	unknown
0a47c	ca4a	unknown
0a47f	ca4a	unknown
0a484	ca4a	unknown
0a488	ca4a	unknown
0a48a	ca7a	unknown
0a48b	f8fb	unknown
0a490	ca7a	unknown
0a4b1	ca8a	unknown
0a4b3	ca8a	unknown
0a4bc	8ada	unknown
0a4be	ca8a	unknown
0a4c0	ca8a	unknown
0a4c5	8ada	unknown
0a4c8	ca4a	unknown
0a4ca	ca4a	unknown
0a4d5	ca4a	unknown
0a4df	ca4a	unknown
0a4e0	ca8a	unknown
0a4fb	ca5a	unknown
0a4ff	ca4a	unknown
0a502	ca4a	unknown
0a507	ca5a	unknown
0a50b	ca4a	unknown
0a50f	ca4a	unknown
0a511	ca4a	unknown
0a515	ca4a	unknown
0a51f	ca4a	unknown
0a522	ca7a	unknown
0a528	ca7a	unknown
0a536	ca4a	unknown
0a54b	8a5a	unknown
0a566	8ada	unknown
0a569	ca4a	unknown
0a570	8ada	unknown
0a579	ca4a	unknown
0a57a	ca8a	unknown
0a599	f8fb	unknown
0a59c	f8fb	unknown
0a5d1	f8fb	unknown
0a5e2	ca7a	unknown
0a5e6	ca8a	unknown
0a5e8	ca8a	unknown
0a5f5	ca7a	unknown
0a60a	ca7a	unknown
0a61d	ca4a	unknown
0a61e	ca8a	unknown
0a632	ca7a	unknown
0a636	ca8a	unknown
0a638	ca8a	unknown
0a645	ca7a	unknown
0a65a	ca7a	unknown
0a662	ca4a	unknown
0a663	ca8a	unknown
0a670	ca5a	unknown
0a72d	ca5a	unknown
0a734	f8fb	unknown
0a739	ca8a	unknown
0a766	ca8a	unknown
0a77e	a80d	unknown
0a8f1	a80d	unknown
0a92a	f8fb	unknown
0a98a	ca5a	unknown
0a991	f8fb	unknown
0a996	ca8a	unknown
0a9c3	ca8a	unknown
0a9db	a80d	unknown
0aa00	ca5a	unknown
0aa21	ca4a	unknown
0aa22	ca8a	unknown
0aa30	a80d	unknown
0aa46	ca5a	unknown
0aa63	a80d	unknown
0aa8c	a80d	unknown
0aa8d	a80d	unknown
0aa8e	a80d	unknown
0aa8f	a80d	unknown
*/