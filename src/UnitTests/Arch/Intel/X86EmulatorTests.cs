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

        private void Given_Win32Code(Action<X86Assembler> coder)
        {
            var asm = new X86Assembler(sc, new DefaultPlatform(sc, arch), Address.Ptr32(0x00100000), new List<ImageSymbol>());
            coder(asm);
            var program = asm.GetImage();
            this.segmentMap = program.SegmentMap;

            Given_Platform();

            var win32 = new Win32Emulator(program.SegmentMap, platform, importReferences);
            
            emu = (X86Emulator) arch.CreateEmulator(program.SegmentMap, win32);
            emu.InstructionPointer = program.ImageMap.BaseAddress;
            emu.WriteRegister(Registers.esp, (uint)program.ImageMap.BaseAddress.ToLinear() + 0x0FFC);
            emu.ExceptionRaised += delegate { throw new Exception(); };
        }

        private void Given_MsdosCode(Action<X86Assembler> coder)
        {
            arch = new X86ArchitectureReal("x86-real-16");
            var asm = new X86Assembler(sc, new DefaultPlatform(sc, arch), Address.SegPtr(0x07F0, 0), new List<ImageSymbol>());
            asm.Segment("PSP");
            asm.Repeat(0x100, m => m.Db(0));
            asm.Segment("Code");
            coder(asm);
            asm.Align(0x2000);  // make room for a stack.
            var program = asm.GetImage();
            this.segmentMap = program.SegmentMap;

            Given_Platform();

            var msdos = platform.CreateEmulator(program.SegmentMap, importReferences);

            emu = (X86Emulator) arch.CreateEmulator(program.SegmentMap, msdos);
            emu.InstructionPointer = Address.SegPtr(0x800, 0);
            emu.WriteRegister(Registers.cs, 0x0800);
            emu.WriteRegister(Registers.ds, 0x0800);
            emu.WriteRegister(Registers.es, 0x0800);
            emu.WriteRegister(Registers.ss, 0x0800);
            emu.WriteRegister(Registers.sp, 0x0FFE);
            emu.ExceptionRaised += delegate { throw new Exception(); };
        }

        private void Given_Platform()
        {
            var mockPlatform = new Mock<IPlatform>();
            var emu = new Mock<IPlatformEmulator>();
            mockPlatform.Setup(p => p.LookupProcedureByName(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ExternalProcedure("", new FunctionType()));
            mockPlatform.Setup(p => p.CreateEmulator(
                It.IsAny<SegmentMap>(),
                It.IsAny<Dictionary<Address,ImportReference>>()))
                .Returns(emu.Object);
            this.platform = mockPlatform.Object;
        }

        // Calculate a segmented real mode address.
        private ulong Lin(uint seg, uint off)
        {
            return (seg << 4) + off;
        }


        [Test]
        public void X86Emu_Mov32()
        {
            Given_Win32Code(m => { m.Mov(m.eax, m.ebx); });
            Given_RegValue(Registers.ebx, 4);
            emu.Start();

            Assert.AreEqual(4, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_Add32()
        {
            Given_Win32Code(m => { m.Add(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 4);
            Given_RegValue(Registers.ebx, ~4u + 1u);
            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 + (1 << 6), emu.Flags, "Should set carry + zero flag");
        }

        [Test]
        public void X86Emu_Add32_ov()
        {
            Given_Win32Code(m => { m.Add(m.eax, m.ebx); });

            Given_RegValue(Registers.eax, 0x80000000u);
            Given_RegValue(Registers.ebx, 0x80000000u);
            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 | (1 << 6) | (1 << 11), emu.Flags, "Should set carry + z + ov");
        }



        [Test]
        public void X86Emu_Sub32_ov()
        {
            Given_Win32Code(m => {
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
            Given_Win32Code(m =>
            {
                m.Mov(m.eax, 0);
                m.Mov(m.ebx, 4);
                m.Sub(m.eax, m.ebx);
            });

            emu.Start();

            Assert.AreEqual(0xFFFFFFFCu, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(X86Emulator.Cmask|X86Emulator.Smask, emu.Flags, "Should set C, S flags");
        }

        [Test]
        public void X86Emu_ReadDirect_W32()
        {
            Given_Win32Code(m => {
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m => { m.Xor(m.eax, m.eax); });
            Given_RegValue(Registers.eax, 0x1);

            emu.Start();

            Assert.AreEqual(0, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(1 << 6, emu.Flags, "Expected ZF set ");
        }

        [Test]
        public void X86Emu_or()
        {
            Given_Win32Code(m => {
                m.Mov(m.eax, m.Imm(0x80000000));
                m.Or(m.eax, m.Imm(1));
            });

            emu.Start();

            Assert.AreEqual(0x80000001, emu.Registers[Registers.eax.Number]);
            Assert.AreEqual(X86Emulator.Smask, emu.Flags, "Expected ZF clear, SF set ");
        }

        [Test]
        public void X86Emu_halt()
        {
            Given_Win32Code(m => {
                m.Hlt();
                m.Mov(m.eax, 42);
            });

            emu.Start();

            Assert.AreNotEqual(42u, emu.Registers[Registers.eax.Number]);
        }


        [Test]
        public void X86Emu_jz()
        {
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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
            Given_Win32Code(m =>
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

        [Test]
        public void X86Emu_cld()
        {
            Given_MsdosCode(m =>
            {
                m.Cld();
                m.Hlt();
            });
            emu.Registers[Registers.eflags.Number] = 0xFFFFFFFF;
            emu.Start();

            Assert.AreEqual(0xFFFFFBFF, emu.Registers[Registers.eflags.Number]);
        }

        [Test]
        public void X86Emu_real_push()
        {
            Given_MsdosCode(m =>
            {
                m.Push(m.es);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual(0x0FFC, emu.Registers[Registers.esp.Number]);
            Assert.AreEqual(0x800u, emu.ReadLeUInt16(Lin(0x800, 0xFFC)));
        }


        [Test]
        public void X86Emu_mov_16Bit()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.ax, m.cs);
                m.Hlt();
            });

            emu.WriteRegister(Registers.eax, 0xFFFFFFFF);
            emu.Start();

            Assert.AreEqual(0xFFFF0800, emu.Registers[Registers.eax.Number]);
        }

        [Test]
        public void X86Emu_add_mem_16Bit()
        {
            Given_MsdosCode(m =>
            {
                m.Db(0x01, 0x06, 0x38, 0x00);   // add ds:[0038],ax
                m.Hlt();
            });

            var addr = Lin(0x800, 0x38);
            emu.WriteLeUInt16(addr, 0x4000);
            emu.WriteRegister(Registers.eax, 0x1C000);

            emu.Start();

            Assert.AreEqual(0x0000, emu.ReadLeUInt16(addr));
            Assert.AreEqual(X86Emulator.Zmask, emu.Flags & X86Emulator.Zmask);
            Assert.AreEqual(X86Emulator.Cmask, emu.Flags & X86Emulator.Cmask);
        }

        [Test]
        public void X86Emu_movsw_16bit()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.si, 0x40);
                m.Mov(m.di, 0x80);
                m.Mov(m.cx, 0x3);
                m.Rep();
                m.Movsw();
                m.Hlt();
            });

            emu.WriteLeUInt16(Lin(0x800, 0x40), 1);
            emu.WriteLeUInt16(Lin(0x800, 0x42), 2);
            emu.WriteLeUInt16(Lin(0x800, 0x44), 3);
            emu.WriteLeUInt16(Lin(0x800, 0x46), 4); // won't get copied.

            emu.Start();

            Assert.AreEqual(1, emu.ReadLeUInt16(Lin(0x800, 0x80)));
            Assert.AreEqual(2, emu.ReadLeUInt16(Lin(0x800, 0x82)));
            Assert.AreEqual(3, emu.ReadLeUInt16(Lin(0x800, 0x84)));
            Assert.AreEqual(0, emu.ReadLeUInt16(Lin(0x800, 0x86))); // won't get copied.
        }

        [Test]
        public void X86Emu_lodsw_16bit()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.si, 0x40);
                m.Lodsw();
                m.Hlt();
            });

            emu.WriteRegister(Registers.eax, 0xFFFFFFFF);
            emu.WriteLeUInt16(Lin(0x800, 0x40), 0x4242);

            emu.Start();

            Assert.AreEqual(0xFFFF4242, emu.ReadRegister(Registers.eax));
        }

        [Test]
        public void X86Emu_lodsb_16bit()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.si, 0x40);
                m.Lodsb();
                m.Hlt();
            });

            emu.WriteRegister(Registers.eax, 0xFFFFFFFF);
            emu.WriteLeUInt16(Lin(0x800, 0x40), 0x4242);

            emu.Start();

            Assert.AreEqual(0xFFFFFF42, emu.ReadRegister(Registers.eax));
        }

        [Test]
        public void X86Emu_jmp_far_16bit()
        {
            Given_MsdosCode(m =>
            {
                m.JmpF(Address.SegPtr(0x7FF, 0x16));
                m.Hlt();
                m.Mov(m.ax, 0x4242);
                m.Hlt();
            });

            emu.WriteRegister(Registers.eax, 0xFFFFFFFF);

            emu.Start();

            Assert.AreEqual(0xFFFF4242, emu.ReadRegister(Registers.eax));
        }

        [Test]
        public void X86Emu_js_16bit()
        {
            Given_MsdosCode(m =>
            {
                m.Jns("not_signed");
                m.Mov(m.ax, 0x4711);
                m.Hlt();
                m.Label("not_signed");
                m.Mov(m.ax, 0x4242);
                m.Hlt();
            });

            emu.WriteRegister(Registers.eax, 0xFFFFFFFF);

            emu.Start();

            Assert.AreEqual(0xFFFF4242, emu.ReadRegister(Registers.eax));
        }

        [Test]
        public void X86Emu_dec_16bit()
        {
            Given_MsdosCode(m =>
            {
                m.Dec(m.dx);
                m.Hlt();
            });
            emu.Start();
            Assert.AreEqual(X86Emulator.Smask, emu.Flags & X86Emulator.Smask);
        }

        [Test]
        public void X86emu_call_far()
        {
            Given_MsdosCode(m =>
            {
                m.Db(0x9A, 0x06, 0x00, 0x00, 0x08); // call far fn
                m.Hlt();

                m.Label("fn");
                m.Mov(m.ax, 0x42);
                m.Hlt();
            });
            emu.Start();
            Assert.AreEqual(0x42, emu.ReadRegister(Registers.ax));
        }

        [Test]
        public void X86emu_call_far_ret()
        {
            Given_MsdosCode(m =>
            {
                m.Db(0x9A, 0x09, 0x00, 0x00, 0x08); // call far fn
                m.Mov(m.ax, 0x42);
                m.Hlt();

                m.Label("fn");
                m.Retf();
            });
            emu.Start();
            Assert.AreEqual(0x42, emu.ReadRegister(Registers.ax));
        }

        [Test]
        public void X86emu_call_near_ret()
        {
            Given_MsdosCode(m =>
            {
                m.Call("fn"); // call near fn
                m.Mov(m.ax, 0x42);
                m.Hlt();

                m.Label("fn");
                m.Ret();
            });
            emu.Start();
            Assert.AreEqual(0x42, emu.ReadRegister(Registers.ax));
        }

        [Test]
        public void X86Emu_Shl16()
        {
            Given_MsdosCode(m =>
            {
                m.Shl(m.si, 3);
                m.Hlt();
            });

            emu.WriteRegister(Registers.esi, 0xFFFF0001);
            emu.Start();
            Assert.AreEqual(0xFFFF0008, emu.ReadRegister(Registers.esi));
        }

        [Test]
        public void X86Emu_loop16()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.ax, 0);    // sum
                m.Mov(m.cx, 4);
                m.Label("Lupe");
                m.Add(m.ax, m.cx);
                m.Loop("Lupe");
            });

            emu.WriteRegister(Registers.ecx, 0xFFFFFFFF);
            emu.Start();

            Assert.AreEqual(10, emu.Registers[Registers.ax.Number]);
        }

        [Test]
        public void X86Emu_stc()
        {
            Given_MsdosCode(m =>
            {
                m.Stc();
                m.Hlt();
            });

            emu.Start();

            Assert.IsTrue((emu.Flags & X86Emulator.Cmask) != 0);
        }

        [Test]
        public void X86Emu_rcl16()
        {
            Given_MsdosCode(m =>
            {
                m.Stc();
                m.Mov(m.eax, 0x7FF0AAAA);
                m.Rcl(m.ax, 1);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual("7FF05555", emu.ReadRegister(Registers.eax).ToString("X8"));
            Assert.IsTrue((emu.Flags & X86Emulator.Cmask) != 0);
        }

        [Test]
        public void X86Emu_rcl8()
        {
            Given_MsdosCode(m =>
            {
                m.Stc();
                m.Mov(m.eax, 0x7FF08000);
                m.Rcl(m.ah, 1);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual("7FF00100", emu.ReadRegister(Registers.eax).ToString("X8"));
            Assert.IsTrue((emu.Flags & X86Emulator.Cmask) != 0);
        }

        [Test]
        public void X86Emu_sar16()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.eax, 0x7FF0AAAA);
                m.Sar(m.ax, 3);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual("7FF0F555", emu.ReadRegister(Registers.eax).ToString("X8"));
            Assert.AreEqual(X86Emulator.Smask, emu.Flags);
        }

        [Test]
        public void X86Emu_shr16()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.cx, 3);
                m.Mov(m.eax, 0x33333333);
                m.Shr(m.ax, m.cl);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual("33330666", emu.ReadRegister(Registers.eax).ToString("X8"));
            Assert.AreEqual(0, emu.Flags);
        }

        [Test]
        public void X86Emu_shl16_flags()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.eax, 0x33333333);
                m.Shl(m.ax, 2);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual("3333CCCC", emu.ReadRegister(Registers.eax).ToString("X8"));
            Assert.AreEqual(X86Emulator.Smask, emu.Flags);
        }

        [Test]
        public void X86Emu_stosb16()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.ax, 0x1234);
                m.Mov(m.edi, 0x28880020);
                m.Stosb();
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual(52, emu.ReadLeUInt16(Lin(0x800, 0x0020)));
        }

        [Test]
        public void X86Emu_shr_16()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.bp, 0xFFFF);
                m.Shr(m.bp, 1);
                m.Hlt();
            });

            emu.Start();

            Assert.AreEqual(0x7FFF, emu.ReadRegister(Registers.bp));
            Assert.AreEqual(X86Emulator.Cmask, emu.Flags);
        }

        [Test]
        public void X86emu_X86Emu_indexed_memory_16()
        {
            Given_MsdosCode(m =>
            {
                m.Mov(m.bx, 0xFFF0);
                m.Mov(m.di, 0x0020);
                m.Db(0x8A, 0x01); // 8A01          MOV AL,[BX+DI]
                m.Hlt();
            });

            emu.WriteByte(Lin(0x0800, 0x0010), 0x42);

            emu.Start();

            Assert.AreEqual(0x42, emu.ReadRegister(Registers.al));
        }
    }
}
/*
;; fn1D54_056D: 1D54:056D
;;   Called from:
;;     1800:5AB9 (in fn0800_0003)
;;     1800:5ABE (in fn0800_0003)
;;     1800:5AC7 (in fn0800_0003)
;;     1800:5ACC (in fn0800_0003)
;;     1800:5ADD (in fn0800_0003)
;;     1800:5AE4 (in fn0800_0003)
;;     1800:5AEB (in fn0800_0003)
;;     1800:5AF6 (in fn0800_0003)
;;     1800:5AFF (in fn0800_0003)
;;     1800:5B15 (in fn0800_0003)
;;     1800:5B26 (in fn0800_0003)
;;     1800:5B2E (in fn0800_0003)
;;     1800:5B38 (in fn0800_0003)
;;     1800:5B41 (in fn0800_0003)
fn1D54_056D proc
	shr	bp,01
	dec	dl
	jnz	0578

l1D54_0573:
	lodsw
	mov	bp,ax
	mov	dl,10

l1D54_0578:
	ret

l1D54_0579:
	call	056D
	rcl	bh,01
	call	056D
	jc	0597

l1D54_0583:
	mov	dh,02
	mov	cl,03

l1D54_0587:
	call	056D
	jc	0595

l1D54_058C:
	call	056D
	rcl	bh,01
	shl	dh,01
	loop	0587

l1D54_0595:
	sub	bh,dh

l1D54_0597:
	mov	dh,02
	mov	cl,04

l1D54_059B:
	inc	dh
	call	056D
	jc	05B2

l1D54_05A2:
	loop	059B

l1D54_05A4:
	call	056D
	jnc	05B6

l1D54_05A9:
	inc	dh
	call	056D
	jnc	05B2

l1D54_05B0:
	inc	dh

l1D54_05B2:
	mov	cl,dh
	jmp	05E0

l1D54_05B6:
	call	056D
	jc	05CB

l1D54_05BB:
	mov	cl,03
	mov	dh,00

l1D54_05BF:
	call	056D
	rcl	dh,01
	loop	05BF

l1D54_05C6:
	add	dh,09
	jmp	05B2

l1D54_05CB:
	lodsb
	mov	cl,al
	add	cx,11
	jmp	05E0

l1D54_05D3:
	mov	cl,03

l1D54_05D5:
	call	056D
	rcl	bh,01
	loop	05D5

l1D54_05DC:
	dec	bh

l1D54_05DE:
	mov	cl,02

l1D54_05E0:
	mov	al,es:[bx+di]
	stosb
	loop	05E0





fn1D54_05E6:
	call	056D
	jnc	05EE

l1D54_05EB:
	movsb
	jmp	05E6


l1D54_05EE:
	call	056D
	lodsb
	mov	bh,FF
	mov	bl,al
	jc	0579

l1D54_05F8:
	call	056D
	jc	05D3

l1D54_05FD:
	cmp	bh,bl
	jnz	05DE

l1D54_0601:
	call	056D
	jnc	062D

l1D54_0606:
	mov	cl,04
	push	di
	shr	di,cl
	mov	ax,es
	add	ax,di
	sub	ah,02
	mov	es,ax
	pop	di
	and	di,000F
	add	di,2000
	push	si
	shr	si,cl
	mov	ax,ds
	add	ax,si
	mov	ds,ax
	pop	si
	and	si,000F
	jmp	05E6

l1D54_062D:
	pop	bp
	push	cs
	pop	ds
	mov	si,004A
	mov	cx,008A

l1D54_0636:
	lodsw
	or	ax,ax
	js	0644

l1D54_063B:
	add	ax,bp
	mov	es,ax
	lodsw
	mov	bx,ax
	jmp	064A

l1D54_0644:
	shl	ax,01
	sar	ax,01
	add	bx,ax

l1D54_064A:
	add	es:[bx],bp
	loop	0636

l1D54_064F:
	pop	es
	pop	ds
	add	bp,1555
	mov	ss,bp
	mov	sp,0000
	xor	bp,bp
	xor	di,di
	xor	si,si
	xor	dx,dx
	xor	bx,bx
	xor	ax,ax
	jmp	far 0800:2268
1D54:066B                                  00 00 00 00 00            .....
1D54:0670 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1D54:0750 00 00 00 00 00 00 00 00 29 DB 00 08 F0 07 00 00 ........).......
1D54:0760 00 00 00 00                                     ....           
*/

