#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    class X86EmulatorTests
    {
        private IntelArchitecture arch;
        private X86Emulator emu;
        private LoadedImage image;

        [SetUp]
        public void Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.Protected32);
        }

        private void Given_RegValue(IntelRegister reg, uint value)
        {
            emu.WriteRegister(reg, value);
        }

        private void Given_Code(Action<IntelAssembler> coder)
        {
            var asm = new IntelAssembler(arch, new Address(0x00100000), new List<EntryPoint>());
            coder(asm);
            var lr = asm.GetImage();
            this.image = lr.Image;
            emu = new X86Emulator(arch, lr.Image);
            emu.InstructionPointer = lr.Image.BaseAddress;
            emu.ExceptionRaised += delegate { throw new Exception(); };
        }

        [Test]
        public void X86Emu_Mov32()
        {
            Given_Code(m => { m.Mov(m.eax, m.ebx); });
            Given_RegValue(Registers.ebx, 4);
            emu.Run();

            Assert.AreEqual(4, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_Add32()
        {
            Given_Code(m => { m.Add(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 4);
            Given_RegValue(Registers.ebx, ~4u + 1u);
            emu.Run();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 + (1 << 6), emu.Flags, "Should set carry + zero flag");
        }

        [Test]
        public void X86Emu_Add32_ov()
        {
            Given_Code(m => { m.Add(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 0x80000000u);
            Given_RegValue(Registers.ebx, 0x80000000u);
            emu.Run();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 | (1 << 6) | (1 << 11), emu.Flags, "Should set carry + z + ov");
        }

        [Test]
        public void X86Emu_Sub32_ov()
        {
            Given_Code(m => { m.Sub(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 0x80000000u);
            Given_RegValue(Registers.ebx, 0x00000001u);
            emu.Run();

            Assert.AreEqual(0x7FFFFFFFu, emu.Registers[Registers.eax.Number], string.Format("{0:X}", emu.Registers[Registers.eax.Number] ));
            Assert.AreEqual(1  | (1 << 11), emu.Flags, "Should set carry + z + ov flag");
        }


        [Test]
        public void X86Emu_ReadDirect_W32()
        {
            Given_Code(m => {
                m.Label("datablob");
                m.Dd(0x12345678);
                m.Mov(m.eax, m.MemDw("datablob")); 
            });
            emu.InstructionPointer += 4;
            emu.Run();

            Assert.AreEqual(0x12345678u, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_ReadIndexed_W32()
        {
            Given_Code(m =>
            {
                m.Label("datablob");
                m.Dd(0x12345678);
                m.Mov(m.eax, m.MemDw(Registers.ebx, -0x10));
            });
            Given_RegValue(Registers.ebx, 0x00100010);
            emu.InstructionPointer += 4;
            emu.Run();

            Assert.AreEqual(0x12345678u, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_Immediate_W32()
        {
            Given_Code(m =>
            {
                m.Mov(m.eax, 0x1234);
            });
            Given_RegValue(Registers.eax, 0xFFFFFFFF);
            emu.Run();

            Assert.AreEqual(0x00001234u, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_Write_Byte()
        {
            Given_Code(m =>
            {
                m.Label("datablob");
                m.Dd(0x12345678);
                m.Mov(m.MemB(0x00100000), m.ah);
            });
            Given_RegValue(Registers.eax, 0x40302010);
            emu.InstructionPointer += 4;
            emu.Run();

            Assert.AreEqual(0x12345620u, image.ReadLeUInt32(0));
        }

        [Test]
        public void X86Emu_Immediate_W16()
        {
            Given_Code(m =>
            {
                m.Mov(m.ax, 0x1234);
            });
            Given_RegValue(Registers.eax, 0xFFFFFFFF);
            emu.Run();

            Assert.AreEqual(0xFFFF1234u, emu.Registers[Registers.eax.Number]);
        }
    }
}
