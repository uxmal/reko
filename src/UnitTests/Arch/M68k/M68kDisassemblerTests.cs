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

using Decompiler.Core;
using Decompiler.Arch.M68k;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.M68k
{
    [TestFixture]
    public class M68kDisassemblerTests
    {
        private M68kDisassembler2 dasm;
        private M68kInstruction instr;

        private void DasmSingleInstruction(params byte[] bytes)
        {
            dasm = CreateDasm(bytes, 0x10000000);
            instr = dasm.Disassemble();
        }

        private M68kDisassembler2 CreateDasm(byte[] bytes, uint address)
        {
            Address addr = new Address(address);
            ProgramImage img = new ProgramImage(addr, bytes);
            return new M68kDisassembler2(img.CreateReader(addr));
        }

        private M68kDisassembler2 CreateDasm(params ushort[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[] { (byte)(w >> 8), (byte)w }).ToArray();
            return CreateDasm(bytes, 0x10000000);
        }

        [Test]
        public void MoveQ()
        {
            byte[] bytes = new byte[] {
                0x72, 0x01
            };
            DasmSingleInstruction(bytes);
            Assert.AreEqual("moveq\t#$+01,d1", instr.ToString());
        }


        [Test]
        public void AddQ()
        {
            byte[] bytes = new byte[] {
                0x5E, 0x92
            };
            DasmSingleInstruction(bytes);
            Assert.AreEqual( "addq.l\t#$+07,(a2)", instr.ToString());
        }

        [Test]
        public void Ori()
        {
            byte[] bytes = new byte[] {
                0x00, 0x00, 0x00, 0x12
            };
            DasmSingleInstruction(bytes);
            Assert.AreEqual("ori.b\t#$12,d0", instr.ToString());
        }

        [Test]
        public void OriCcr()
        {
            byte[] bytes = new byte[] {
                0x00, 0x3C, 0x00, 0x42
                };
            DasmSingleInstruction(bytes);
            Assert.AreEqual("ori.b\t#$42,ccr", instr.ToString());
        }

        [Test]
        public void OriSr()
        {
            byte[] bytes = new byte[] {
                0x00, 0x7C, 0x00, 0x42
                };
            DasmSingleInstruction(bytes);
            Assert.AreEqual("ori.w\t#$0042,sr", instr.ToString());
        }

        [Test]
        public void MoveW()
        {
            DasmSingleInstruction(0x30, 0x2F, 0x47, 0x11);
            Assert.AreEqual("move.w\t$4711(a7),d0", instr.ToString());
        }

        [Test]
        public void Lea()
        {
            DasmSingleInstruction(0x43, 0xEF, 0x00, 0x04);
            Assert.AreEqual("lea\t$0004(a7),a1", instr.ToString());
        }

        [Test]
        public void LslD()
        {
            DasmSingleInstruction(0xE5, 0x49, 0x00, 0x04);
            Assert.AreEqual("lsl.w\t#$+02,d1", instr.ToString());
        }

        [Test]
        public void AddaW()
        {
            DasmSingleInstruction(0xD2, 0xC1, 0x00, 0x04);
            Assert.AreEqual("adda.w\td1,a1", instr.ToString());
        }

        [Test]
        public void Addal()
        {
            dasm = CreateDasm(0xDBDC);
            Assert.AreEqual("adda.l\t(a4)+,a5", dasm.Disassemble().ToString());
        }

        [Test]
        public void MoveA()
        {
            DasmSingleInstruction(0x20, 0x51, 0x00, 0x04);
            Assert.AreEqual("movea.l\t(a1),a0", instr.ToString());
        }

        [Test]
        public void MoveM()
        {
            DasmSingleInstruction(0x48, 0xE7, 0x00, 0x04);
            Assert.AreEqual("movem.l\ta5,-(a7)", instr.ToString());
        }

        [Test]
        public void BraB()
        {
            DasmSingleInstruction(0x60, 0x1A);
            Assert.AreEqual("bra\t$1000001C", instr.ToString());
        }

        [Test]
        public void Bchg()
        {
            DasmSingleInstruction(0x01, 0x40);
            Assert.AreEqual("bchg\td0,d0", instr.ToString());
        }

        [Test]
        public void Dbf()
        {
            DasmSingleInstruction(0x51, 0xCA, 0xFF, 0xE4);
            Assert.AreEqual("dbf\td2,$0FFFFFE6", instr.ToString());
        }

        [Test]
        public void Moveb()
        {
            DasmSingleInstruction(0x14, 0x1A);
            Assert.AreEqual("move.b\t(a2)+,d2", instr.ToString());
        }

        [Test]
        public void ManyMoves()
        {
            dasm = CreateDasm(new byte[] { 0x20, 0x00, 0x20, 0x27, 0x20, 0x40, 0x20, 0x67, 0x20, 0x80, 0x21, 0x40, 0x00, 0x00 }, 0x10000000);
            Assert.AreEqual("move.l\td0,d0", dasm.Disassemble().ToString());
            Assert.AreEqual("move.l\t-(a7),d0", dasm.Disassemble().ToString());
            Assert.AreEqual("movea.l\td0,a0", dasm.Disassemble().ToString());
            Assert.AreEqual("movea.l\t-(a7),a0", dasm.Disassemble().ToString());
            Assert.AreEqual("move.l\td0,(a0)", dasm.Disassemble().ToString());
            Assert.AreEqual("move.l\td0,$0000(a0)", dasm.Disassemble().ToString());
        }

        [Test]
        public void AddB()
        {
            DasmSingleInstruction(0xD2, 0x02);
            Assert.AreEqual("add.b\td2,d1", instr.ToString());
        }

        [Test]
        public void Eor()
        {
            dasm = CreateDasm(0xB103, 0xB143, 0xB183);
            Assert.AreEqual("eor.b\td0,d3", dasm.Disassemble().ToString());
            Assert.AreEqual("eor.w\td0,d3", dasm.Disassemble().ToString());
            Assert.AreEqual("eor.l\td0,d3", dasm.Disassemble().ToString());
        }

        [Test]
        public void bcs()
        {
            dasm = CreateDasm(0x6572);
            Assert.AreEqual("bcs\t$00000074", dasm.Disassemble().ToString());
        }

    }
}
