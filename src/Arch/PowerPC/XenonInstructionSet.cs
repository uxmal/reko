#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Reko.Core.Machine;
using static Reko.Arch.PowerPC.PowerPcDisassembler;

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;

    // https://github.com/yui-konnu/PPC-Altivec-IDA/blob/master/plugin.cpp
    // https://github.com/xenia-project/xenia/blob/master/src/xenia/cpu/ppc/ppc_opcode_lookup_gen.cc
    // http://biallas.net/doc/vmx128/vmx128.txt
    public class XenonInstructionSet : InstructionSet
    {

        public override Decoder Ext4Decoder()
        {
            var baseDecoder = base.Ext4Decoder();
            /*
               VD128, VS128:  5 lower bits of a VMX128 vector register 
                              number
               VDh:	          upper 2 bits of VD128
                              (so register number is (VDh << 5 | VD128))
               VA128:         same as VD128
               A:             bit 6 of VA128
               a:             bit 5 of VA128
                              (so register number is (A<<6 | a<<5 | VA128))
               VB128:         same as VD128
               VBh:           same as VDh
               VC128:         3 bits of a VMX128 vector register number
                              (you can only use vr0-vr7 here)
               RA, RB:        general purpose register number
               UIMM:          unsigned immediate value
               SIMM:          signed immediate value
               PERMh:         upper 3 bits of a permutation
               PERMl:         lower 5 bits of a permutation
               x, y, z:       unknown immediate values*/
            var decoder = Mask(27, 1, "  Xenon OP4",
                    /*
                    0           6         11        16        21            28  30
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 1 0 0 0|VDh|1 1|    lvewx128 vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 0 0 0|VDh|1 1|    lvlx128       vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 1 0 0|VDh|1 1|    lvrx128       vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 1 0 0 0 0 0|VDh|1 1|    lvlxl128      vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 1 0 0 1 0 0|VDh|1 1|    lvrxl128      vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 0 0 0 0|VDh|1 1|    lvsl128       vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 0 1 0 0|VDh|1 1|    lvsr128       vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 1 1 0 0|VDh|1 1|    lvx128        vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 1 0 1 1 0 0|VDh|1 1|    lvxl128       vr(VD128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 1 1 0 0 0 0|VDh|1 1|    stvewx128     vr(VS128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 0 0 0|VDh|1 1|    stvlx128      vr(VS128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 0 0 0|VDh|1 1|    lvlxl128      vr(VS128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 1 0 0|VDh|1 1|    stvrx128       vr(VS128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 1 0 0|VDh|1 1|    stvrxl128     vr(VS128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 0 1 1 1 0 0|VDh|1 1|    stvx128       vr(VS128), r(RA), r(RB)
                    |0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 1 1 1 1 0 0|VDh|1 1|    stvxl128      vr(VS128), r(RA), r(RB)
                    */
                 Select(30, 2, u => u != 3,
                    baseDecoder,
                    Sparse(21, 7, baseDecoder,
                        (0b0000000, Instr(Mnemonic.lvsl128, Wd, r2, r3)),
                        (0b0000100, Instr(Mnemonic.lvsr128, Wd, r2, r3)),
                        (0b0001000, Instr(Mnemonic.lvewx128, Wd, r2, r3)),
                        (0b0001100, Instr(Mnemonic.lvx128, Wd, r2, r3)),
                        (0b0011000, Instr(Mnemonic.stvewx128, Wd, r2, r3)),
                        (0b0011100, Instr(Mnemonic.stvx128, Wd, r2, r3)),
                        (0b0101100, Instr(Mnemonic.lvxl128, Wd, r2, r3)),
                        (0b0111100, Instr(Mnemonic.stvxl128, Wd, r2, r3)),
                        (0b1000000, Instr(Mnemonic.lvlx128, Wd, r2, r3)),
                        (0b1000100, Instr(Mnemonic.lvrx128, Wd, r2, r3)),
                        (0b1010000, Instr(Mnemonic.stvlx128, Wd, r2, r3)),
                        (0b1010100, Instr(Mnemonic.stvrx128, Wd, r2, r3)),
                        (0b1100000, Instr(Mnemonic.lvlxl128, Wd, r2, r3)),
                        (0b1100100, Instr(Mnemonic.lvrxl128, Wd, r2, r3)),
                        (0b1110000, Instr(Mnemonic.lvlxl128, Wd, r2, r3)),
                        (0b1110100, Instr(Mnemonic.stvrxl128, Wd, r2, r3)))),

            /*
            |0 0 0 1 0 0|  VD128  |  VA128  |  VB128  |A|  SHB  |a|1|VDh|VBh|    vsldoi128     vr(VD128), vr(VA128), vr(VB128), SHB
            */
            Instr(Mnemonic.vsldoi128, Wd, Wa, Wb, u6_4));
            return decoder;
        }

        /*
 	{	vmx128_vsldoi128,	"vsldoi128",	VX128_5(4, 16),		VX128_5_MASK,	{ VD128, VA128, VB128, SHB },			""	},
	{	vmx128_lvsl128,		"lvsl128",		VX128_1(4, 3),		VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_lvsr128,		"lvsr128",		VX128_1(4, 67),		VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_lvewx128,	"lvewx128",		VX128_1(4, 131),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_lvx128,		"lvx128",		VX128_1(4, 195),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_stvewx128,	"stvewx128",	VX128_1(4, 387),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
	{	vmx128_stvx128,		"stvx128",		VX128_1(4, 451),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
	{	vmx128_lvxl128,		"lvxl128",		VX128_1(4, 707),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_stvxl128,	"stvxl128",		VX128_1(4, 963),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
	{	vmx128_lvlx128,		"lvlx128",		VX128_1(4, 1027),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_lvrx128,		"lvrx128",		VX128_1(4, 1091),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_stvlx128,	"stvlx128",		VX128_1(4, 1283),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
	{	vmx128_stvrx128,	"stvrx128",		VX128_1(4, 1347),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
	{	vmx128_lvlxl128,	"lvlxl128",		VX128_1(4, 1539),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_lvrxl128,	"lvrxl128",		VX128_1(4, 1603),	VX128_1_MASK,	{ VD128, RA, RB },						""	},
	{	vmx128_stvlxl128,	"stvlxl128",	VX128_1(4, 1795),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
	{	vmx128_stvrxl128,	"stvrxl128",	VX128_1(4, 1859),	VX128_1_MASK,	{ VS128, RA, RB },						""	},
    */


        public override Decoder Ext5Decoder()
        {
            var decoder = Mask(27, 1, "  Xenon OP5",
                //  0           6         11        16        21
                // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|    vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
                Mask(22, 1, "  bit 27=0 (BE)",
                    Instr(Mnemonic.vperm128, Wd,Wa,Wb,Wc),
                    Mask(23, 3, "  not-vperm128",
                        Instr(Mnemonic.vpkshss128, Wd,Wa,Wb),      // 1 0 0 0 | a | 0 | VDh | VBh | vpkshss128    vr(VD128), vr(VA128), vr(VB128)
                        Instr(Mnemonic.vpkshus128, Wd,Wa,Wb),      // 1 0 0 1 | a | 0 | VDh | VBh | vpkshus128    vr(VD128), vr(VA128), vr(VB128)
                        Instr(Mnemonic.vpkswss128, Wd,Wa,Wb),      // 1 0 1 0 | a | 0 | VDh | VBh | vpkswss128    vr(VD128), vr(VA128), vr(VB128)
                        Instr(Mnemonic.vpkswus128, Wd,Wa,Wb),      // 1 0 1 1 | a | 0 | VDh | VBh | vpkswus128    vr(VD128), vr(VA128), vr(VB128)
                        
                        Instr(Mnemonic.vpkuhum128, Wd,Wa,Wb),      // 1 1 0 0 | a | 0 | VDh | VBh | vpkuhum128    vr(VD128), vr(VA128), vr(VB128)
                        Instr(Mnemonic.vpkuhus128, Wd,Wa,Wb),      // 1 1 0 1 | a | 0 | VDh | VBh | vpkuhus128    vr(VD128), vr(VA128), vr(VB128)
                        Instr(Mnemonic.vpkuwum128, Wd,Wa,Wb),      // 1 1 1 0 | a | 0 | VDh | VBh | vpkuwum128    vr(VD128), vr(VA128), vr(VB128)
                        Instr(Mnemonic.vpkuwus128, Wd,Wa,Wb))),    // 1 1 1 1 | a | 0 | VDh | VBh | vpkuwus128    vr(VD128), vr(VA128), vr(VB128)
                Mask(22, 4, "  bit 27=1 (BE)",
                       Instr(Mnemonic.vaddfp128,   Wd,Wa,Wb),             // 0 0 0 0 | a | 1 | VDh | VBh | vaddfp128     vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vsubfp128,   Wd,Wa,Wb),             // 0 0 0 1 | a | 1 | VDh | VBh | vsubfp128     vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vmulfp128,   Wd,Wa,Wb),             // 0 0 1 0 | a | 1 | VDh | VBh | vmulfp128     vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vmaddfp128,  Wd,Wa,Wb,Wd),          // 0 0 1 1 | a | 1 | VDh | VBh | vmaddfp128    vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)

                       Instr(Mnemonic.vmaddcfp128, Wd,Wa,Wb,Wd),          // 0 1 0 0 | a | 1 | VDh | VBh | vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)
                       Instr(Mnemonic.vnmsubfp128, Wd,Wa,Wb,Wd),          // 0 1 0 1 | a | 1 | VDh | VBh | vnmsubfp128   vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
                       Instr(Mnemonic.vmsub3fp128, Wd,Wa,Wb),             // 0 1 1 0 | a | 1 | VDh | VBh | vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vmsub4fp128, Wd,Wa,Wb),             // 0 1 1 1 | a | 1 | VDh | VBh | vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)
                       
                       Instr(Mnemonic.vand128,     Wd,Wa,Wb),             // 1 0 0 0 | a | 1 | VDh | VBh | vand128       vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vandc128,    Wd,Wa,Wb),             // 1 0 1 0 | a | 1 | VDh | VBh | vandc128      vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vnor128,     Wd,Wa,Wb),             // 1 0 1 0 | a | 1 | VDh | VBh | vnor128       vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vor128,      Wd,Wa,Wb),             // 1 0 1 1 | a | 1 | VDh | VBh | vor128        vr(VD128), vr(VA128), vr(VB128)
                       
                       Instr(Mnemonic.vxor128,     Wd,Wa,Wb),             // 1 1 0 0 | a | 1 | VDh | VBh | vxor128       vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vsel128,     Wd,Wa,Wb,Wd),          // 1 1 0 1 | a | 1 | VDh | VBh | vsel128       vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
                       Instr(Mnemonic.vslo128,     Wd,Wa,Wb),             // 1 1 1 0 | a | 1 | VDh | VBh | vslo128       vr(VD128), vr(VA128), vr(VB128)
                       Instr(Mnemonic.vsro128,     Wd,Wa,Wb)));           // 1 1 1 1 | a | 1 | VDh | VBh | vslo128       vr(VD128), vr(VA128), vr(VB128)
            return decoder;
        }

    /*
	{	vmx128_vperm128,	"vperm128",		VX128_2(5, 0),		VX128_2_MASK,	{ VD128, VA128, VB128, VC128 },			""	},
	{	vmx128_vaddfp128,	"vaddfp128",	VX128(5, 16),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vsubfp128,	"vsubfp128",	VX128(5,  80),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vmulfp128,	"vmulfp128",	VX128(5, 144),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vmaddfp128,	"vmaddfp128",	VX128(5, 208),		VX128_MASK,		{ VD128, VA128, VB128, VS128 },			""	},
	{	vmx128_vmaddcfp128,	"vmaddcfp128",	VX128(5, 272),		VX128_MASK,		{ VD128, VA128, VS128, VB128 },			""	},
	{	vmx128_vnmsubfp128,	"vnmsubfp128",	VX128(5, 336),		VX128_MASK,		{ VD128, VA128, VB128, VS128 },			""	},
	{	vmx128_vmsum3fp128,	"vmsum3fp128",	VX128(5, 400),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vmsum4fp128,	"vmsum4fp128",	VX128(5, 464),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vpkshss128,	"vpkshss128",	VX128(5, 512),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vand128,		"vand128",		VX128(5, 528),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vpkshus128,	"vpkshus128",	VX128(5, 576),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vandc128,	"vandc128",		VX128(5, 592),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vpkswss128,	"vpkswss128",	VX128(5, 640),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vnor128,		"vnor128",		VX128(5, 656),		VX128_MASK,		{ VD128, VA128, VB128 },				""  },
	{	vmx128_vpkswus128,	"vpkswus128",	VX128(5, 704),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vor128,		"vor128",		VX128(5, 720),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vpkuhum128,	"vpkuhum128",	VX128(5, 768),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vxor128,		"vxor128",		VX128(5, 784),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vpkuhus128,	"vpkuhus128",	VX128(5, 832),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vsel128,		"vsel128",		VX128(5, 848),		VX128_MASK,		{ VD128, VA128, VB128, VS128 },			""	},
	{	vmx128_vpkuwum128,	"vpkuwum128",	VX128(5, 896),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vslo128,		"vslo128",		VX128(5, 912),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vpkuwus128,	"vpkuwus128",	VX128(5, 960),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
	{	vmx128_vsro128,		"vsro128",		VX128(5, 976),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
*/
        public override Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction> Ext6Decoder()
        {
            /*
            return new VMXDecoder(0x7F, new Dictionary<uint, Decoder>
            {
                { 0x00, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x02, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x04, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x06, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x40, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x42, Instr(Mnemonic.vcmpeqfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x44, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x46, Instr(Mnemonic.vcmpeqfp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|    vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x08, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0A, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0C, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x0E, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x48, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4A, Instr(Mnemonic.vcmpgefp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4C, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x4E, Instr(Mnemonic.vcmpgefp128, CC,Wd,Wa,Wb) },     // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|    vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x10, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x12, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x14, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x16, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x50, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x52, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x54, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x56, Instr(Mnemonic.vcmpgtfp128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|    vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x18, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1A, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1C, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x1E, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x58, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5A, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5C, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x5E, Instr(Mnemonic.vcmpbfp128, Wd,Wa,Wb) },        // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|    vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x0D, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x0F, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x4D, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x4F, Instr(Mnemonic.vslw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)

                { 0x1D, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x1F, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x5D, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)
                { 0x5F, Instr(Mnemonic.vsrw128, Wd,Wa,Wb) },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)

                { 0x20, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x22, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x24, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x26, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x60, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x62, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x64, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
                { 0x66, Instr(Mnemonic.vcmpequw128, Wd,Wa,Wb) },       // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|    vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)

                { 0x23, Instr(Mnemonic.vcfpsxws128, Wd,Wb,s16_5) },    // |0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |0 1 0 0 0 1 1|VDh|VBh|    vcfpsxws128   vr(VD128), vr(VB128), SIMM

                { 0x28, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x2A, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x68, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6A, Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|    vmaxfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x2B, Instr(Mnemonic.vcsxwfp128, Wd,Wb,u16_5) },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 0 1 1|VDh|VBh|    vcsxwfp128    vr(VD128), vr(VB128), SIMM

                { 0x2C, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x2E, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6C, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)
                { 0x6E, Instr(Mnemonic.vminfp128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|    vminfp128     vr(VD128), vr(VA128), vr(VB128)

                { 0x30, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x32, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x70, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x72, Instr(Mnemonic.vmrghw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)

                { 0x34, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x36, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x74, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                { 0x76, Instr(Mnemonic.vmrglw128, Wd,Wa,Wb) },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)

                { 0x37, Instr(Mnemonic.vrfin128, Wd,Wb) },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 1 1 1|VDh|VBh|    vrfin128      vr(VD128), vr(VB128)

                { 0x3F, Instr(Mnemonic.vrfiz128, Wd,Wb) },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 1 1|VDh|VBh|    vrfiz128      vr(VD128), vr(VB128)

                { 0x61, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x65, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x69, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
                { 0x6D, Instr(Mnemonic.vpkd3d128, Wd,Wb,u18_3,u16_2,u6_2) },   // |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z

                { 0x63, Instr(Mnemonic.vrefp128, Wd,Wb) },             // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 0 1 1|VDh|VBh|    vrefp128      vr(VD128), vr(VB128)

                { 0x67, Instr(Mnemonic.vrsqrtefp128, Wd,Wb) },         // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 1 1 1|VDh|VBh|    vrsqrtefp128  vr(VD128), vr(VB128)

                { 0x6B, Instr(Mnemonic.vexptefp128, Wd,Wb) },          // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 0 1 1|VDh|VBh|    vexptefp128   vr(VD128), vr(VB128)

                { 0x6F, Instr(Mnemonic.vlogefp128, Wd,Wb) },           // |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 1 1 1|VDh|VBh|    vlogefp128    vr(VD128), vr(VB128)

                { 0x77, Instr(Mnemonic.vspltisw128, Wd,Wb,s16_5) },

                { 0x71, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x75, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x79, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                { 0x7D, Instr(Mnemonic.vrlimi128, Wd,Wb,u16_5,u14_2) },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z

                { 0x73, Instr(Mnemonic.vspltw128, Wd,Wb,u16_5) },      // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|    vspltw128     vr(VD128), vr(VB128), UIMM

                { 0x7F, Instr(Mnemonic.vupkd3d128, Wd,Wb,u16_5) },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|    vupkd3d128    vr(VD128), vr(VB128), UIMM
            });
            */
            var vpermwi = Instr(Mnemonic.vpermwi128, Wd, Wb, u(BeFields((23, 3), (11, 5))));
            var vpkd3d128 = Nyi("vpkd3d128"); //    vr(VD128), vr(VB128), x, y, z
            var vrlimi128 = Nyi("vrlimi128");   //     vr(VD128), vr(VB128), UIMM, z

             var decoder = Mask(27, 1, "  Ext6 - Xenon",
                Mask(22, 5, " .....0",
                    Instr(Mnemonic.vcmpeqfp128, C6, Wd,Wa,Wb),
                    Instr(Mnemonic.vcmpeqfp128, C6,Wd,Wa,Wb),
                    Instr(Mnemonic.vcmpeqfp128, C6,Wd,Wa,Wb),
                    Instr(Mnemonic.vcmpeqfp128, C6,Wd,Wa,Wb),

                    Instr(Mnemonic.vcmpgefp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpgefp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpgefp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpgefp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)

                    Instr(Mnemonic.vcmpgtfp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpgtfp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpgtfp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpgtfp128, C6,Wd,Wa,Wb), //   vr(VD128), vr(VA128), vr(VB128)(R == 0)

                    Instr(Mnemonic.vcmpbfp128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpbfp128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpbfp128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpbfp128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)

                    Instr(Mnemonic.vcmpequw128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpequw128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpequw128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)
                    Instr(Mnemonic.vcmpequw128, C6,Wd,Wa,Wb), //    vr(VD128), vr(VA128), vr(VB128)(R == 0)

                    Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb),
                    Instr(Mnemonic.vmaxfp128, Wd,Wa,Wb),
                    Instr(Mnemonic.vminfp128, Wd,Wa,Wb),
                    Instr(Mnemonic.vminfp128, Wd,Wa,Wb),

                    Instr(Mnemonic.vmrghw128, Wd,Wa,Wb),
                    Instr(Mnemonic.vmrghw128, Wd,Wa,Wb),
                    Instr(Mnemonic.vmrglw128, Wd,Wa,Wb),
                    Instr(Mnemonic.vmrglw128, Wd,Wa,Wb),

                    Instr(Mnemonic.vupkhsb128, Wd,Wb),  //    vr(VD128), vr(VB128)(R == 0)
                    invalid,
                    Instr(Mnemonic.vupklsb128, Wd,Wb),  //    vr(VD128), vr(VB128)(R == 0)
                    invalid),

                Mask(22, 1, "  .....1",
                    Mask(23, 3,
                        invalid,
                        invalid,
                        invalid,
                        Instr(Mnemonic.vslw128, Wd,Wa,Wb),           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)

                        invalid,
                        Instr(Mnemonic.vsraw128, Wd,Wa,Wb),           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                        invalid,
                        Instr(Mnemonic.vsrw128, Wd,Wa,Wb)),           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                    Mask(23, 3,
                       Select(11, 5, u => u == 0,
                            Instr(Mnemonic.vrefp128    , Wd,Wb),
                            Instr(Mnemonic.vcfpsxws128 , Wd,Wb, s16_5)),
                       Select(11, 5, u => u == 0,
                            Instr(Mnemonic.vrsqrtefp128, Wd,Wb),
                            Instr(Mnemonic.vcfpuxws128, Wd,Wb, u16_5)),
                       Select(11, 5, u => u == 0,
                            Instr(Mnemonic.vexptefp128 , Wd,Wb),
                            Instr(Mnemonic.vcsxwfp128, Wd,Wb, s16_5)),
                       Select(11, 5, u => u == 0,
                            Instr(Mnemonic.vlogefp128, Wd, Wb),
                            Instr(Mnemonic.vcuxwfp128  , Wd,Wb, u16_5)),
                            
                       Select(11, 5, u => u == 0,
                            Instr(Mnemonic.vrfim128, Wd, Wb),
                            Instr(Mnemonic.vspltw128, Wd, Wb, u16_5)),
                       Select(11, 5, u => u == 0,
                            Instr(Mnemonic.vrfin128, Wd, Wb),
                            Instr(Mnemonic.vspltisw128, Wd, Wb, s16_5)),
                       Instr(Mnemonic.vrfip128, Wd,Wb),
                       Select(11, 5, u => u == 0,
                           Instr(Mnemonic.vrfiz128, Wd,Wb),
                /* 1 1 1 */Instr(Mnemonic.vupkd3d128, Wd, Wb, u16_5)))));

            return decoder;
        }

        public override Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction> Ext3BDecoder()
        {
            var decoder = Sparse(26, 5, invalid, "  Xenon Ext3B",
                (0b10010, Instr(Mnemonic.fdivs, C,f1,f2,f3)),       // A I 154 PPC fdivs[.] Floating Divide Single
                (0b10100, Instr(Mnemonic.fsubs, C, f1, f2, f3)),    // A I 153 PPC fsubs[.] Floating Subtract Single
                (0b10101, Instr(Mnemonic.fadds, C, f1, f2, f3)),    // A I 153 PPC fadds[.] Floating Add Single
                (0b10110, Instr(Mnemonic.fsqrts, C, f1, f3)),       // A I 155 PPC fsqrts[.] Floating Square Root Single
                (0b11000, Instr(Mnemonic.fres, C, f1, f3)),         // A I 155 PPC fres[.] Floating Reciprocal Estimate Single
                (0b11001, Instr(Mnemonic.fmuls, C, f1, f2, f4)),    // A I 154 PPC fmuls[.] Floating Multiply Single
                (0b11010, Instr(Mnemonic.frsqrtes, C, f1, f3)),     // A I 156 v2.02 frsqrtes[.] Floating Reciprocal Square Root Estimate Single
                (0b11100, Instr(Mnemonic.fmsubs, C,f1,f2,f4,f3)),   // A I 159 PPC fmsubs[.] Floating Multiply-Subtract Single
                (0b11101, Instr(Mnemonic.fmadds, C,f1,f2,f4,f3)),   // A I 158 PPC fmadds[.] Floating Multiply-Add Single
                (0b11110, Instr(Mnemonic.fnmsubs, C,f1,f2,f3,f4)),  // A I 159 PPC fnmsubs[.] Floating Negative Multiply-Subtract Single
                (0b11111, Instr(Mnemonic.fnmadds, C,f1,f2,f3,f4))); // A I 159 PPC fnmadds[.] Floating Negative Multiply-Add Single
            return decoder;
        }

        public override Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction> Ext3CDecoder()
        {
            return invalid;
        }

        /*

        |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0 1|a|1|VDh|VBh|    vsraw128      vr(VD128), vr(VA128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vsrw128       vr(VD128), vr(VA128), vr(VB128)

        |0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |0 1 0 0 0 1 1|VDh|VBh|    vcfpsxws128   vr(VD128), vr(VB128), SIMM
        |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 0 1 1 1|VDh|VBh|    vcfpuxws128   vr(VD128), vr(VB128), UIMM
        |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 0 1 1|VDh|VBh|    vcsxwfp128    vr(VD128), vr(VB128), SIMM
        |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 1 1 1|VDh|VBh|    vcuxwfp128    vr(VD128), vr(VB128), UIMM
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 1 1 1|VDh|VBh|    vrfin128      vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 0 1 1|VDh|VBh|    vrfip128      vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 1 1|VDh|VBh|    vrfiz128      vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 0 1 1|VDh|VBh|    vrfim128      vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 0 1 1|VDh|VBh|    vrefp128      vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 1 1 1|VDh|VBh|    vrsqrtefp128  vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 0 1 1|VDh|VBh|    vexptefp128   vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 1 1 1|VDh|VBh|    vlogefp128    vr(VD128), vr(VB128)
        |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|    vspltw128     vr(VD128), vr(VB128), UIMM
        |0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |1 1 1 0 1 1 1|VDh|VBh|    vspltisw128   vr(VD128), vr(VB128), SIMM
        |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|    vupkd3d128    vr(VD128), vr(VB128), UIMM

        |0 0 0 1 1 0|  VD128  |  PERMl  |  VB128  |0|1|PERMh|0|1|VDh|VBh|    vpermwi128    vr(VD128), vr(VB128), (PERMh << 5 | PERMl)
        |0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|    vpkd3d128     vr(VD128), vr(VB128), x, y, z
        |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
        |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 1 1|a|1|VDh|VBh|    vsro128       vr(VD128), vr(VA128), vr(VB128)
        */

    }



    /*
        {	vmx128_vpermwi128,	"vpermwi128",	VX128_P(6, 528),	VX128_P_MASK,	{ VD128, VB128, VPERM128 },				""	},
        {	vmx128_vcfpsxws128,	"vcfpsxws128",	VX128_3(6, 560),	VX128_3_MASK,	{ VD128, VB128, SIMM },					""	},
        {	vmx128_vcfpuxws128,	"vcfpuxws128",	VX128_3(6, 624),	VX128_3_MASK,	{ VD128, VB128, UIMM },					""	},
        {	vmx128_vcsxwfp128,	"vcsxwfp128",	VX128_3(6, 688),	VX128_3_MASK,	{ VD128, VB128, SIMM },					""	},
        {	vmx128_vcuxwfp128,	"vcuxwfp128",	VX128_3(6, 752),	VX128_3_MASK,	{ VD128, VB128, UIMM },					""	},
        {	vmx128_vrfim128,	"vrfim128",		VX128_3(6, 816),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vrfin128,	"vrfin128",		VX128_3(6, 880),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vrfip128,	"vrfip128",		VX128_3(6, 944),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vrfiz128,	"vrfiz128",		VX128_3(6, 1008),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vpkd3d128,	"vpkd3d128",	VX128_4(6, 1552),	VX128_4_MASK,	{ VD128, VB128, VD3D0, VD3D1, VD3D2},	""	},
        {	vmx128_vrefp128,	"vrefp128",		VX128_3(6, 1584),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vrsqrtefp128,"vrsqrtefp128",	VX128_3(6, 1648),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vexptefp128,	"vexptefp128",	VX128_3(6, 1712),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vlogefp128,	"vlogefp128",	VX128_3(6, 1776),	VX128_3_MASK,	{ VD128, VB128 },						""	},
        {	vmx128_vrlimi128,	"vrlimi128",	VX128_4(6, 1808),	VX128_4_MASK,	{ VD128, VB128, UIMM, VD3D2},			""	},
        {	vmx128_vspltw128,	"vspltw128",	VX128_3(6, 1840),	VX128_3_MASK,	{ VD128, VB128, UIMM },					""	},
        {	vmx128_vspltisw128,	"vspltisw128",	VX128_3(6, 1904),	VX128_3_MASK,	{ VD128, VB128, SIMM },					""	},
        {	vmx128_vupkd3d128,	"vupkd3d128",	VX128_3(6, 2032),	VX128_3_MASK,	{ VD128, VB128, UIMM },					""	},
        {	vmx128_vcmpeqfp128,	"vcmpeqfp128",	VX128(6, 0),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpeqfp128c,"vcmpeqfp128.",	VX128(6, 64),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vrlw128,		"vrlw128",		VX128(6, 80),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpgefp128,	"vcmpgefp128",	VX128(6, 128),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpgefp128c,"vcmpgefp128.",	VX128(6, 192),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vslw128,		"vslw128",		VX128(6, 208),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpgtfp128,	"vcmpgtfp128",	VX128(6, 256),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpgtfp128c,"vcmpgtfp128.",	VX128(6, 320),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vsraw128,	"vsraw128",		VX128(6, 336),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpbfp128,	"vcmpbfp128",	VX128(6, 384),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpbfp128c,	"vcmpbfp128.",	VX128(6, 448),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vsrw128,		"vsrw128",		VX128(6, 464),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpequw128,	"vcmpequw128",	VX128(6, 512),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vcmpequw128c,"vcmpequw128.",	VX128(6, 576),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vmaxfp128,	"vmaxfp128",	VX128(6, 640),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vminfp128,	"vminfp128",	VX128(6, 704),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vmrghw128,	"vmrghw128",	VX128(6, 768),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vmrglw128,	"vmrglw128",	VX128(6, 832),		VX128_MASK,		{ VD128, VA128, VB128 },				""	},
        {	vmx128_vupkhsb128,	"vupkhsb128",	VX128(6, 896),		VX128_MASK,		{ VD128, VB128 },						""	},
        {	vmx128_vupklsb128,	"vupklsb128",	VX128(6, 960),		VX128_MASK,		{ VD128, VB128 },						""  },
        */
}
