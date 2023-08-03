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
using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mos6502
{
    [TestFixture]
    public class EmulatorTests
    {
        private const byte C = Mos6502Emulator.Cmask;
        private const byte N = Mos6502Emulator.Nmask;
        private const byte V = Mos6502Emulator.Vmask;
        private const byte NC = (byte) (N | C);
        private const byte VC = (byte) (V | C);

        private readonly ServiceContainer sc;
        private readonly Mos6502Architecture arch;
        private Mos6502Emulator emu;

        public EmulatorTests()
        {
            this.sc = new ServiceContainer();
            this.arch = new Mos6502Architecture(sc, "mos6502", new Dictionary<string, object>());
        }

        private void Given_Code(Action<Assembler> p)
        {
            var asm = new Assembler(sc, arch, Address.Ptr16(0x0800), new List<ImageSymbol>());
            p(asm);
            var program = asm.GetImage();
            program.SegmentMap.AddSegment(
                new ByteMemoryArea(Address.Ptr16(0), new byte[256]),
                "ZeroPage",
                AccessMode.ReadWriteExecute);
            program.SegmentMap.AddSegment(
                new ByteMemoryArea(Address.Ptr16(0x100), new byte[0x200]),
                "Low memory",
                AccessMode.ReadWriteExecute);

            var envEmu = new DefaultPlatformEmulator();

            emu = (Mos6502Emulator) arch.CreateEmulator(program.SegmentMap, envEmu);
            emu.InstructionPointer = program.ImageMap.BaseAddress;
            emu.WriteRegister(Registers.s, 0xFF);
            emu.ExceptionRaised += (sender, e) => { throw e.Exception; };
        }

        [Test]
        public void Emu6502_adc_carry()
        {
            Given_Code(m =>
            {
                m.Adc(m.i8(0x1));
            });
            emu.WriteRegister(Registers.a, 0x00);
            emu.WriteRegister(Registers.p, C);

            emu.Start();

            Assert.AreEqual(0x02, emu.ReadRegister(Registers.a));
            Assert.AreEqual(0x00, emu.ReadRegister(Registers.p));
        }
        [Test]
        public void Emu6502_adc_overflow()
        {
            Given_Code(m =>
            {
                m.Adc(m.i8(0x1));
            });
            emu.WriteRegister(Registers.a, 0x7F);
            emu.WriteRegister(Registers.p, 0);

            emu.Start();

            Assert.AreEqual(0x80, emu.ReadRegister(Registers.a));
            Assert.AreEqual(0xC0, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_asl_acc()
        {
            Given_Code(m =>
            {
                m.Asl(Registers.a);
            });
            emu.WriteRegister(Registers.a, 0xAA);
            emu.WriteRegister(Registers.p, 0);

            emu.Start();

            Assert.AreEqual(0x54, emu.ReadRegister(Registers.a));
            Assert.AreEqual(0x81, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_asl_acc_shift_out_0()
        {
            Given_Code(m =>
            {
                m.Asl(Registers.a);
            });
            emu.WriteRegister(Registers.a, 0x2A);
            emu.WriteRegister(Registers.p, VC);

            emu.Start();

            Assert.AreEqual(0x54, emu.ReadRegister(Registers.a));
            Assert.AreEqual(V, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_asl_zp()
        {
            Given_Code(m =>
            {
                m.Asl(m.zp(0xF0));
            });
            emu.WriteByte(0xF0, 0xAA);
            emu.WriteRegister(Registers.p, 0);

            emu.Start();

            emu.TryReadByte(0xF0, out var b);
            Assert.AreEqual(0x54, b);
            Assert.AreEqual(0x81, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_clc()
        {
            Given_Code(m =>
            {
                m.Clc();
            });
            emu.WriteRegister(Registers.p, C);

            emu.Start();

            Assert.AreEqual(0x00, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_cmp()
        {
            Given_Code(m =>
            {
                m.Cmp(m.i8(7));
            });
            emu.WriteRegister(Registers.a, 6);

            emu.Start();

            Assert.AreEqual(NC, emu.ReadRegister(Registers.p));
        }


        [Test]
        public void Emu6502_cpy()
        {
            Given_Code(m =>
            {
                m.Cpy(m.i8(0xF0));
            });
            emu.WriteByte(0xF0, 0xAA);
            emu.WriteRegister(Registers.y, 0);

            emu.Start();

            Assert.AreEqual(C, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_inc_zp()
        {
            Given_Code(m =>
            {
                m.Inc(m.zp(4));
            });
            emu.WriteByte(0x04, 0x42);
            emu.WriteRegister(Registers.p, 0xFF);

            emu.Start();

            emu.TryReadByte(0x04, out byte b);
            Assert.AreEqual(0x43, b);
            Assert.AreEqual(0x7D, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_ldy_imm()
        {
            Given_Code(m =>
            {
                m.Ldy(m.i8(0xC4)); // A0 C4 ldy #$C4
            });

            emu.Start();

            Assert.AreEqual(0xC4, emu.ReadRegister(Registers.y));
        }

        [Test]
        public void Emu6502_lda_abs_y()
        {
            Given_Code(m =>
            {
                m.Db(0x42);
                m.Ldy(m.i8(4));         // A0 04    ldy #$04
                m.Lda(m.ay(0x07FC));    // B9 3C 08 lda $083C,y
            });
            emu.InstructionPointer += 1;
            emu.Start();

            Assert.AreEqual(0x42, emu.ReadRegister(Registers.a));
        }

        [Test]
        public void Emu6502_lda_ind_y()
        {
            Given_Code(m =>
            {
                m.Lda(m.iy(0x42));
            });
            emu.WriteLeUInt16(0x42, 0x0034);
            emu.WriteLeUInt16(0x35, 0x11);
            emu.WriteRegister(Registers.y, 1);
            
            emu.Start();

            Assert.AreEqual(0x11, emu.ReadRegister(Registers.a));
        }

        [Test]
        public void Emu6502_ldx_zp()
        {
            Given_Code(m =>
            {
                m.Ldx(m.zp(4));         // A6 04    ldx $04
            });
            emu.WriteByte(0x04, 0x42);
            emu.Start();

            Assert.AreEqual(0x42, emu.ReadRegister(Registers.x));
        }

        [Test]
        public void Emu6502_sta_zpx()
        {
            Given_Code(m =>
            {
                m.Lda(m.i8(0x42));      // lda  #$42
                m.Ldx(m.i8(0x04));      // ldx  #$04
                m.Sta(m.zpX(0x04));     // sta  $04,x
            });
            emu.Start();
            emu.TryReadByte(0x0008, out var b);
            Assert.AreEqual(0x42, b);
        }

        [Test]
        public void Emu6502_sta_indirect_indexed()
        {
            Given_Code(m =>
            {
                m.Sta(m.iy(0xFC));      // sta ($FC),y
            });
            emu.WriteRegister(Registers.a, 0x42);
            emu.WriteRegister(Registers.y, 4);
            emu.WriteLeUInt16(0x00FC, 0x0080);
            emu.WriteByte(0x0084, 0x23);

            emu.Start();

            emu.TryReadByte(0x0084, out byte b);
            Assert.AreEqual(0x42, b);
        }


        [Test]
        public void Emu6502_dey()
        {
            Given_Code(m =>
            {
                m.Dey();                // 88 dey 
            });
            emu.Start();

            Assert.AreEqual(0xFF, emu.ReadRegister(Registers.y));
            Assert.AreEqual(Mos6502Emulator.Nmask, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_inx()
        {
            Given_Code(m =>
            {
                m.Ldx(m.i8(0xFF));
                m.Inx();                // E8      inx
            });
            emu.Start();

            Assert.AreEqual(0x00, emu.ReadRegister(Registers.x));
            Assert.AreEqual(Mos6502Emulator.Zmask, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_bne()
        {
            Given_Code(m =>
            {
                m.Ldx(m.i8(2));         // xx yy    ldx
                m.Dex();                // CA       dex
                m.Bne("skip");          // D0 xx    bne
                m.Ldx(m.i8(0x42));
                m.Label("skip");
                m.Nop();
            });
            emu.Start();

            Assert.AreEqual(0x01, emu.ReadRegister(Registers.x));
            Assert.AreEqual(0, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_bpl()
        {
            Given_Code(m =>
            {
                m.Ldx(m.i8(2));         // xx yy    ldx
                m.Dex();                // CA       dex
                m.Bpl("skip");          // D0 xx    bne
                m.Ldx(m.i8(0x42));
                m.Label("skip");
                m.Nop();
            });
            emu.Start();

            Assert.AreEqual(0x01, emu.ReadRegister(Registers.x));
            Assert.AreEqual(0, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_jmp()
        {
            Given_Code(m =>
            {
                m.Ldx(m.i8(2));         // xx yy    ldx
                m.Dex();                // CA       dex
                m.Jmp("skip");          // 4C xx xx jmp
                m.Ldx(m.i8(0x42));
                m.Label("skip");
                m.Nop();
            });
            emu.Start();

            Assert.AreEqual(0x01, emu.ReadRegister(Registers.x));
            Assert.AreEqual(0, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_jsr()
        {
            Given_Code(m =>
            {
                m.Ldx(m.i8(1));
                m.Jsr("subroutine");
                m.Ldx(m.i8(2));
                m.Jmp("done");
                m.Label("subroutine");
                m.Ldx(m.i8(3));
                m.Label("done");
                m.Nop();
            });
            emu.Start();

            Assert.AreEqual(0xFD, emu.ReadRegister(Registers.s));
            Assert.AreEqual(3, emu.ReadRegister(Registers.x));
            Assert.AreEqual(0x0804, emu.ReadLeUInt16(0x1FE));
        }

        [Test]
        public void Emu6502_rol()
        {
            Given_Code(m =>
            {
                m.Rol(m.zp(0x42));
            });
            emu.WriteByte(0x0042, 0xAA);
            emu.WriteRegister(Registers.p, C);

            emu.Start();

            emu.TryReadByte(0x42, out byte b);
            Assert.AreEqual(0x55, b);
            Assert.AreEqual(0x81, emu.ReadRegister(Registers.p));
        }

        [Test]
        public void Emu6502_rts()
        {
            Given_Code(m =>
            {
                m.Ldx(m.i8(1));
                m.Jsr("subroutine");
                m.Ldx(m.i8(3));
                m.Jmp("done");

                m.Label("subroutine");
                m.Ldx(m.i8(2));
                m.Rts();

                m.Label("done");
                m.Nop();
            });

            emu.Start();

            Assert.AreEqual(0x03, emu.ReadRegister(Registers.x));
        }

        [Test]
        public void Emu6502_sbc()
        {
            Given_Code(m =>
            {
                m.Sbc(m.zp(0xF0));
            });
            emu.WriteRegister(Registers.a, 0x05);
            emu.WriteRegister(Registers.p, C);
            emu.WriteByte(0xF0, 0x3);

            emu.Start();

            Assert.AreEqual(0x02, emu.ReadRegister(Registers.a));
        }

        [Test]
        public void Emu6502_sei()
        {
            Given_Code(m =>
            {
                m.Sei();
            });

            emu.Start();

            Assert.AreEqual(0x04, emu.ReadRegister(Registers.p));
        }
    }
    /* ﻿
    0818 
    083A 4C 00 01 jmp $0100
    ﻿0967 78 sei 
    0968 E6 01 inc $01
    096A 4C 16 08 jmp $0816
    */
}
