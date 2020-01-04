#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    public class X86EmulatorTests
    {
        private IntelArchitecture arch;
        private X86Emulator emu;
        private SegmentMap segmentMap;
        private Dictionary<Address, ImportReference> importReferences;
        private IPlatform platform;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            importReferences = new Dictionary<Address, ImportReference>();
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

        private void Given_RegValue(RegisterStorage reg, uint value)
        {
            emu.WriteRegister(reg, value);
        }

        private void Given_Code(Action<X86Assembler> coder)
        {
            var asm = new X86Assembler(sc, new DefaultPlatform(sc, arch), Address.Ptr32(0x00100000), new List<ImageSymbol>());
            coder(asm);
            var program = asm.GetImage();
            this.segmentMap = program.SegmentMap;

            Given_Platform();

            var win32 = new Win32Emulator(program.SegmentMap, platform, importReferences);
            
            emu = new X86Emulator(arch, program.SegmentMap, win32);
            emu.InstructionPointer = program.ImageMap.BaseAddress;
            emu.WriteRegister(Registers.esp, (uint)program.ImageMap.BaseAddress.ToLinear() + 0x0FFC);
            emu.ExceptionRaised += delegate { throw new Exception(); };
        }

        private void Given_Platform()
        {
            var mockPlatform = new Mock<IPlatform>();
            mockPlatform.Setup(p => p.LookupProcedureByName(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ExternalProcedure("", new FunctionType()));
            this.platform = mockPlatform.Object;
        }

        [Test]
        public void X86Emu_Mov32()
        {
            Given_Code(m => { m.Mov(m.eax, m.ebx); });
            Given_RegValue(Registers.ebx, 4);
            emu.Start();

            Assert.AreEqual(4, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_Add32()
        {
            Given_Code(m => { m.Add(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 4);
            Given_RegValue(Registers.ebx, ~4u + 1u);
            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 + (1 << 6), emu.Flags, "Should set carry + zero flag");
        }

        [Test]
        public void X86Emu_Add32_ov()
        {
            Given_Code(m => { m.Add(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 0x80000000u);
            Given_RegValue(Registers.ebx, 0x80000000u);
            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 | (1 << 6) | (1 << 11), emu.Flags, "Should set carry + z + ov");
        }

        [Test]
        public void X86Emu_Sub32_ov()
        {
            Given_Code(m => {
                m.Mov(m.eax, ~0x7FFFFFFF);
                m.Mov(m.ebx, 0x00000001);
                m.Sub(m.eax, m.ebx); 
            });

            emu.Start();

            Assert.AreEqual(0x7FFFFFFFu, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(X86Emulator.Omask, emu.Flags, "Should set ov flag");
        }

        [Test]
        public void X86Emu_Sub32_cy()
        {
            Given_Code(m =>
            {
                m.Mov(m.eax, 0);
                m.Mov(m.ebx, 4);
                m.Sub(m.eax, m.ebx);
            });

            emu.Start();

            Assert.AreEqual(0xFFFFFFFCu, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(X86Emulator.Cmask, emu.Flags, "Should set C flag");
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
            emu.Start();

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
            emu.Start();

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
            emu.Start();

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
            emu.Start();

            Assert.AreEqual(0x12345620u, segmentMap.Segments.Values.First().MemoryArea.ReadLeUInt32(0));
        }

        [Test]
        public void X86Emu_Immediate_W16()
        {
            Given_Code(m =>
            {
                m.Mov(m.ax, 0x1234);
            });
            Given_RegValue(Registers.eax, 0xFFFFFFFF);
            emu.Start();

            Assert.AreEqual(0xFFFF1234u, emu.Registers[(int)Registers.eax.Domain]);
        }

        [Test]
        public void X86Emu_Xor()
        {
            Given_Code(m => { m.Xor(m.eax, m.eax); });
            Given_RegValue(Registers.eax, 0x1);

            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 << 6, emu.Flags, "Expected ZF set ");
        }

        [Test]
        public void X86Emu_or()
        {
            Given_Code(m => { m.Or(m.eax, m.eax); });
            Given_RegValue(Registers.eax, 0x1);

            emu.Start();

            Assert.AreEqual(1, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(0, emu.Flags, "Expected ZF clear ");
        }

        [Test]
        public void X86Emu_halt()
        {
            Given_Code(m => {
                m.Hlt();
                m.Mov(m.eax, 42);
            });

            emu.Start();

            Assert.AreNotEqual(42u, emu.Registers[Registers.eax.Number]);
        }


        [Test]
        public void X86Emu_jz()
        {
            Given_Code(m =>
            {
                m.Sub(m.eax, m.eax);
                m.Jz("z_flag_set");
                m.Hlt();
                m.Label("z_flag_set");
                m.Mov(m.eax, 42);
            });

            emu.Start();

            Assert.AreEqual(42u, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_pusha()
        {
            Given_Code(m =>
            {
                m.Pusha();
                m.Hlt();
                m.Dw(0);
                m.Dd(0); m.Dd(0); m.Dd(0); m.Dd(0); 
                m.Dd(0); m.Dd(0); m.Dd(0); m.Dd(0); 
            });
            emu.WriteRegister(Registers.esp, (uint)segmentMap.BaseAddress.ToLinear() + 0x24u);

            emu.Start();

            Assert.AreEqual(0x00100004u, emu.Registers[Registers.esp.Number]);
        }

        [Test]
        public void X86Emu_lea()
        {
            Given_Code(m =>
            {
                m.Lea(m.eax, m.MemDw(Registers.edx, Registers.edx, 4, null));
            });
            emu.WriteRegister(Registers.edx, 4);

            emu.Start();

            Assert.AreEqual(20u, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_adc()
        {
            Given_Code(m =>
            {
                m.Add(m.eax, 1);
                m.Adc(m.ebx, 0);
            });
            emu.WriteRegister(Registers.eax, 0xFFFFFFFF);
            emu.WriteRegister(Registers.ebx, 1);

            emu.Start();

            Assert.AreEqual(2u, emu.Registers[Registers.ebx.Number]);
        }

        [Test]
        public void X86Emu_inc()
        {
            Given_Code(m =>
            {
                m.Mov(m.eax, 0x7FFFFFFF);
                m.Inc(m.eax);
            });

            emu.Start();

            Assert.AreEqual(0x80000000u, emu.Registers[Registers.eax.Number]);
            Assert.IsTrue((emu.Flags & X86Emulator.Zmask) == 0);
            Assert.IsTrue((emu.Flags & X86Emulator.Omask) != 0);
        }

        [Test]
        public void X86Emu_add_signextendedImmByte()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, -0x4);
                m.Db(0x83,0xEE,0xFC);     // sub esi,-4
            });

            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.IsTrue((emu.Flags & X86Emulator.Zmask) != 0);
            Assert.IsTrue((emu.Flags & X86Emulator.Omask) == 0);
        }

        [Test]
        public void X86Emu_shl()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, 4);
                m.Shl(m.esi, 2);
            });

            emu.Start();

            Assert.AreEqual(16, emu.Registers[Registers.esi.Number]);
        }

        [Test]
        public void X86Emu_sub_with_adc()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, 0x0401000);
                m.Xor(m.ebx, m.ebx);
                m.Db(0x83, 0xEE, 0xFC);     // sub esi,-4
                m.Adc(m.ebx, m.ebx);
            });

            emu.Start();

            Assert.AreEqual(1, emu.Registers[Registers.ebx.Number]);
        }

        [Test]
        public void X86Emu_shr()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, 0x00040);
                m.Shr(m.esi, 4);
            });

            emu.Start();

            Assert.AreEqual(4, emu.Registers[Registers.esi.Number]);
        }

        [Test]
        public void X86Emu_rol()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, -0x0FFFFFFF);
                m.Rol(m.esi, 4);
            });

            emu.Start();

            Assert.AreEqual(0x1Fu, emu.Registers[Registers.esi.Number]);
        }

        [Test]
        public void X86Emu_xchg()
        {
            Given_Code(m =>
            {
                m.Mov(m.eax, 1);
                m.Mov(m.ebx, 2);
                m.Db(0x87, 0xC3);       // Xchg eax,ebx
            });

            emu.Start();

            Assert.AreEqual(2, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1, emu.Registers[Registers.ebx.Number]);
        }

        [Test]
        public void X86Emu_loop()
        {
            Given_Code(m =>
            {
                m.Mov(m.eax, 0);    // sum
                m.Mov(m.ecx, 4);
                m.Label("Lupe");
                m.Add(m.eax, m.ecx);
                m.Loop("Lupe");
            });

            emu.Start();

            Assert.AreEqual(10, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_regression_1()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, 8);
                m.Mov(m.ebx, -1);
                m.Sub(m.esi, -4);
                m.Adc(m.ebx, m.ebx);
            });
            emu.Start();
            Assert.AreEqual(1, emu.Flags & 1);
        }

        [Test]
        public void X86Emu_call()
        {
            Given_Code(m =>
            {
                m.Mov(m.esi, 0x00100000 + 5);           // 5

                m.Call(m.MemDw(Registers.esi, 4));      //3
                m.Hlt();                                // 1
                m.Dd(0x0010000D);                       // 4
                m.Mov(m.esi, -1);
                m.Hlt();

                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
                m.Dd(0);
            });

            emu.Registers[Registers.esp.Number] = 0x00100020;
            emu.Start();

            Assert.AreEqual(~0u, emu.Registers[Registers.esi.Number]);
        }

        [Test]
        public void X86Emu_repne_scasb()
        {
            Given_Code(m =>
            {
                m.Repne();
                m.Scasb();
                m.Hlt();
                m.Db(0);
                m.Db(Encoding.ASCII.GetBytes("Hello"));
                m.Db(0);
            });
            emu.Registers[Registers.edi.Number] = 0x00100004;
            emu.Registers[Registers.ecx.Number] = 0xFFFFFFFF;
            emu.Start();

            Assert.AreEqual(0x0010000A, emu.Registers[Registers.edi.Number]);

        }
    }
}
/*
 *    011111 => 100000 overflow
 *    111111 => 000000 no overflow
*/