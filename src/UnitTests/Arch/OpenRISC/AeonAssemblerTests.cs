#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.OpenRISC;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonAssemblerTests
    {
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.program = null;
        }

        private void Asm(string asmSrc)
        {
            var arch = new AeonArchitecture(new ServiceContainer(), "aeon", new Dictionary<string, object>());
            var asm = arch.CreateAssembler(null);
            var program = asm.AssembleFragment(Address.Ptr32(0x10_0000), asmSrc);
            var bmem = (ByteMemoryArea) program.SegmentMap.Segments.Values.First().MemoryArea;
            this.program = program;
        }

        private void AssertAsm(string asmSrc, string hexBytesExpected)
        {
            var arch = new AeonArchitecture(new ServiceContainer(), "aeon", new Dictionary<string, object>());
            var asm = arch.CreateAssembler(null);
            var program = asm.AssembleFragment(Address.Ptr32(0x10_0000), asmSrc);
            var bmem = (ByteMemoryArea) program.SegmentMap.Segments.Values.First().MemoryArea;
            var bytesExpected = hexBytesExpected.Replace(" ", "");
            var sActual = string.Join("", bmem.Bytes.Select(b => $"{b:X2}"));
            Assert.AreEqual(bytesExpected, sActual);
        }

        private void RoundTrip(string asmSrc, string asmExpected)
        {
            var arch = new AeonArchitecture(new ServiceContainer(), "aeon", new Dictionary<string, object>());
            var asm = arch.CreateAssembler(null);
            var program = asm.AssembleFragment(Address.Ptr32(0x10_0000), asmSrc);
            var bmem = (ByteMemoryArea) program.SegmentMap.Segments.Values.First().MemoryArea;
            var dasm = arch.CreateDisassembler(bmem.CreateBeReader(0));
            var nl = Environment.NewLine;
            var sResult =
                nl +
                string.Join(nl, dasm.Select(i => $"{i.Address} {i}"))
                    .Replace('\t', ' ');
            Assert.AreEqual(asmExpected, sResult);
        }


        [Test]
        public void AeonAsm_bg_andi()
        {
            // confirmed with source
            AssertAsm("bg.andi\tr3,r3,0x8000", "C4 63 80 00");
        }

        [Test]
        public void AeonAsm_bg_beqi()
        {
            AssertAsm("bg.beqi\tr10,0x1,0x00100095", "D1 41 04 AA");
        }

        [Test]
        public void AeonAsm_bg_lbz()
        {
            AssertAsm("bg.lbz\tr6,0x1EEC(r7)", "F0 C7 1E EC");
        }

        [Test]
        public void AeonAsm_bg_movhi()
        {
            AssertAsm("bg.movhi\tr7,0xA020", "C0 F4 04 01");
        }

        [Test]
        public void AeonAsm_bg_ori()
        {
            AssertAsm("bg.ori\tr5,r7,0x2400", "C8 A7 24 00");
        }

        [Test]
        public void AeonAsm_bg_sb()
        {
            AssertAsm("bg.sb\t0x36D8(r10),r7", "F8 EA 36 D8");
        }

        [Test]
        public void AeonAsm_bn_bnei()
        {
            AssertAsm("bn.bnei\tr6,0x0,0x00100017", "20 C0 5E");
        }

        [Test]
        public void AeonAsm_bn_j()
        {
            AssertAsm("bn.j\t0x000FF17E", "2F F1 7E");
        }

        [Test]
        public void AeonAsm_bn_xor()
        {
            AssertAsm("bn.xor\tr7,r4,r3", "44 E4 1E");
        }

        [Test]
        public void AeonAsm_bt_addi__()
        {
            AssertAsm("bt.addi\tr1,-0x4", "9C 3C");
        }

        [Test]
        public void AeonAsm_labels()
        {
            Asm(@"
    bn.xor r2,r2,r2
label:
        ");
            var label = this.program.ImageSymbols.Values.First(s => s.Name == "label");
            Assert.AreEqual(0x10_0003u, label.Address.ToUInt32());
        }

        [Test]
        public void AeonAsm_Relocate_RT_AEON_BN_DISP8_2()
        {
            RoundTrip(@"
    bg.ori r1,r0,3
label:
        bg.sb (r3),r1
        bt.addi r3,1
        bt.addi r1,-1
        bn.bnei r1,0,label
        ",
        @"
00100000 bg.ori r1,r0,0x3
00100004 bg.sb? (r3),r1
00100008 bt.addi? r3,0x1
0010000A bt.addi? r1,-0x1
0010000C bn.bnei? r1,0x0,00100004");
        }

        [Test]
        public void AeonAsm_Relocate_RT_AEON_BN_DISP18()
        {
            RoundTrip(@"
// Comment
label:
        bt.addi r3,1
        bn.j label
        ",
        @"
00100000 bt.addi? r3,0x1
00100002 bn.j?? 00100000");
        }

        [Test]
        public void AeonAsm_Relocate_RT_AEON_BG_DISP13_3()
        {
            RoundTrip(@"
// Comment
label:
        bt.addi r3,1
        bg.beqi r3,0x0,label
        ",
        @"
00100000 bt.addi? r3,0x1
00100002 bg.beqi? r3,0x0,00100000");
        }

        [Test]
        public void AeonAsm_Relocate_32bitconstant()
        {
            // Test for a relocation being split across two
            // locations.
            RoundTrip(@"
    bg.movhi r1,hi(variable)
    bg.ori r1,r1,lo(variable)
    bt.jr r2
variable:
        .word 0x42
",
            @"
00100000 bg.movhi r1,0x10000A@hi
00100004 bg.ori r1,r1,0x10000A@lo
00100008 bt.jr r2
0010000A bn.nop");
        }

        [Test]
        public void AeonAsm_Relocate_Load()
        {
            RoundTrip(@"
    bg.movhi r1,hi(variable)
    bg.lwz r3,lo(variable)(r1)
    bt.jr r2
    .half 0x42 // Padding to align
variable:
    .word 0x42
",
            @"
00100000 bg.movhi r1,0x10000C@hi
00100004 bg.lwz r3,0x10000C@lo(r1)
00100008 bt.jr r2
0010000A Nyi 000000
0010000D Nyi 000000");
        }

        [Test]
        public void AeonAsm_Equ()
        {
            RoundTrip(@"
TWO .equ 2
    bg.ori r1,r0,TWO
    bt.jr r2
", @"
00100000 bg.ori r1,r0,0x2
00100004 bt.jr r2");
        }

        [Test]
        public void AeonAsm_hi_lo_of_equ()
        {
            RoundTrip(@"
BIGNUM .equ 0x12345678
    bg.movhi r1,hi(BIGNUM)
    bg.ori   r1,r1,lo(BIGNUM)

    bt.jr r2
", @"
00100000 bg.movhi r1,0x12345678@hi
00100004 bg.ori r1,r1,0x12345678@lo
00100008 bt.jr r2");
        }

        [Test]
        public void AeonAsm_align()
        {
            AssertAsm(@"
    bt.jr r2
    .align 4
    bt.jr r2
",
"844900008449");
        }

        [Test]
        public void AeonAsm_byte_string()
        {
            AssertAsm(@"
NUL .equ 0
    .byte ""Hello world!"",NUL
",
"48656C6C6F20776F726C642100");
        }
    }
}
