#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcDisassemblerTests
    {
        private PowerPcInstruction DisassembleX(uint op, uint rs, uint ra, uint rb, uint xo, uint rc)
        {
            uint w =
                (op << 26) |
                (rs << 21) |
                (ra << 16) |
                (rb << 11) |
                (xo << 1) |
                rc;
            LoadedImage img = new LoadedImage(new Address(0x00100000), new byte[4]);
            img.WriteBeUint32(0, w);
            return Disassemble(img);
        }

        private static PowerPcInstruction DisassembleWord(byte[] a)
        {
            LoadedImage img = new LoadedImage(new Address(0x00100000), a);
            return Disassemble(img);
        }

        private static PowerPcInstruction DisassembleWord(uint instr)
        {
            var bytes = new byte[4];
            new BeImageWriter(bytes).WriteBeUInt32(0, instr);
            var img = new LoadedImage(new Address(0x00100000), bytes);
            return Disassemble(img);
        }

        private static PowerPcInstruction Disassemble(LoadedImage img)
        {
            var arch = new PowerPcArchitecture(PrimitiveType.Word32);
            var dasm = new PowerPcDisassembler(arch, img.CreateReader(0U), arch.WordWidth);
            var instr = dasm.Disassemble();
            return instr;
        }

        [Test]
        public void PPCDis_IllegalOpcode()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 00, 00, 00, 00 });
            Assert.AreEqual(Opcode.illegal, instr.Opcode);
        }

        [Test]
        public void PPCDis_Ori()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 0x60, 0x1F, 0x44, 0x44 });
            Assert.AreEqual(Opcode.ori, instr.Opcode);
            Assert.AreEqual(3, instr.Operands);
            Assert.AreEqual("ori\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_Oris()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 0x64, 0x1F, 0x44, 0x44 });
            Assert.AreEqual("oris\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_Addi()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 0x38, 0x1F, 0xFF, 0xFC });
            Assert.AreEqual("addi\tr0,r31,-0004", instr.ToString());
        }

        [Test]
        public void PPCDis_Or()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 0);
            Assert.AreEqual("or\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_OrDot()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 1);
            Assert.AreEqual("or.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_b()
        {
            var instr = DisassembleWord(0x48000008);
            Assert.AreEqual("b\t$00100008", instr.ToString());
        }

        [Test]
        public void PPCDis_b_absolute()
        {
            var instr = DisassembleWord(0x4800000A);
            Assert.AreEqual("b\t$00000008", instr.ToString());
        }

        [Test]
        public void PPCDis_bl()
        {
            var instr = DisassembleWord(0x4BFFFFFD);
            Assert.AreEqual("bl\t$000FFFFC", instr.ToString());
        }

        [Test]
        public void PPCDis_bcxx()
        {
            var instr = DisassembleWord(0x4000FFF0);
            Assert.AreEqual("bc\t00,00,$000FFFF0", instr.ToString());
        }

        [Test]
        public void PPCDis_mtlr()
        {
            var instr = DisassembleWord(0x7C0803A6);
            Assert.AreEqual("mtlr\tr0", instr.ToString());
        }

        [Test]
        public void PPCDis_blr()
        {
            var instr = DisassembleWord(0x4e800020);
            Assert.AreEqual("blr", instr.ToString());
        }

        [Test]
        public void PPCDis_lwz()
        {
            var instr = DisassembleWord(0x803F0005);
            Assert.AreEqual("lwz\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stw()
        {
            var instr = DisassembleWord(0x903FFFF8);
            Assert.AreEqual("stw\tr1,-8(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stwu()
        {
            var instr = DisassembleWord(0x943F0005);
            Assert.AreEqual("stwu\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stb()
        {
            var instr = DisassembleWord(0x9A3FFE00);
            Assert.AreEqual("stb\tr17,-512(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_lhzx()
        {
            var instr = DisassembleWord(0x7F04022E);
            Assert.AreEqual("lhzx\tr24,r4,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzx()
        {
            var instr = DisassembleWord(0x7F0408AE);
            Assert.AreEqual("lbzx\tr24,r4,r1", instr.ToString());
        }

        [Test]
        public void PPCDis_mulli()
        {
            var instr = DisassembleWord(0x1F1F0003);
            Assert.AreEqual("mulli\tr24,r31,+0003", instr.ToString());
        }

        [Test]
        public void PPCDis_fadd()
        {
            var instr = DisassembleWord(0xFFF4402A);
            Assert.AreEqual("fadd\tf31,f20,f8", instr.ToString());
        }

        [Test]
        public void PPCDis_lbz()
        {
            var instr = DisassembleWord(0x88010203);
            Assert.AreEqual("lbz\tr0,515(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_stbux()
        {
            var instr = DisassembleWord(0x7CAA01EE);
            Assert.AreEqual("stbux\tr5,r10,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_stbu()
        {
            var instr = DisassembleWord(0x9C01FFEE);
            Assert.AreEqual("stbu\tr0,-18(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_lwarx()
        {
            var instr = DisassembleWord(0x7C720028);
            Assert.AreEqual("lwarx\tr3,r18,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzu()
        {
            var instr = DisassembleWord(0x8C0A0000);
            Assert.AreEqual("lbzu\tr0,0(r10)", instr.ToString());
        }
    }
}
