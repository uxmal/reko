/* 
 * Copyright (C) 1999-2009 John Källén.
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
        [Test]
        public void MoveQ()
        {
            byte[] bytes = new byte[] {
                0x72, 0x01
            };
            M68kDisassembler dasm = CreateDasm(bytes, 0x10000000);
            M68kInstruction instr = dasm.Disassemble();
            Assert.AreEqual("moveq\t#$+01,d1", instr.ToString());
        }

        [Test]
        public void AddQ()
        {
            byte[] bytes = new byte[] {
                0x5E, 0x92
            };
            M68kDisassembler dasm = CreateDasm(bytes, 0x10000000);
            M68kInstruction instr = dasm.Disassemble();
            Assert.AreEqual( "addq.l\t#$+07,(a2)", instr.ToString());
        }

        private M68kDisassembler CreateDasm(byte[] bytes, uint addr)
        {
            ProgramImage img = new ProgramImage(new Address(addr), bytes);
            return new M68kDisassembler(img.CreateReader(0));
        }
    }
}
