/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Arch.M68k;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.M68k
{
    [TestFixture]
    public class M68kDisassemblerTests
    {
        private M68kInstruction instr;

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
        public void MoveA()
        {
            DasmSingleInstruction(0x20, 0x51, 0x00, 0x04);
            Assert.AreEqual("movea.l\t(a1),a0", instr.ToString());
        }

        [Test]
        public void MoveM()
        {
            DasmSingleInstruction(0x48, 0xE7, 0x00, 0x04);
            Assert.AreEqual("movem.l\t#$0004,-(a7)", instr.ToString());
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


        private void DasmSingleInstruction(params byte[] bytes)
        {
            M68kDisassembler dasm = CreateDasm(bytes, 0x10000000);
            instr = dasm.Disassemble();
        }

        private M68kDisassembler CreateDasm(byte[] bytes, uint address)
        {
            Address addr = new Address(address);
            ProgramImage img = new ProgramImage(addr, bytes);
            return new M68kDisassembler(img.CreateReader(addr));
        }
    }
}
