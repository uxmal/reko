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

using NUnit.Framework;
using Reko.Arch.Mos6502;
using Reko.Core;
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
        private ServiceContainer sc;
        private Mos6502ProcessorArchitecture arch = new Mos6502ProcessorArchitecture("mos6502");
        private Mos6502Emulator emu;

        public EmulatorTests()
        {
            this.sc = new ServiceContainer();
        }

       

        private void Given_Code(Action<Assembler> p)
        {
            var asm = new Assembler(sc, new DefaultPlatform(sc, arch), Address.Ptr16(0x0800), new List<ImageSymbol>());
            p(asm);
            var program = asm.GetImage();

            var envEmu = new DefaultPlatformEmulator();

            emu = (Mos6502Emulator) arch.CreateEmulator(program.SegmentMap, envEmu);
            emu.InstructionPointer = program.ImageMap.BaseAddress;
            emu.WriteRegister(Registers.s, 0xFF);
            emu.ExceptionRaised += (sender, e) => { throw e.Exception; };
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
    }
    /* ﻿
    0818 
    081B 99 F8 00 sta $00F8,y
    081E B9 FD 08 lda $08FD,y
    0821 99 33 03 sta $0333,y
    0824 88 dey 
    0825 D0 F1 bne #$818
    0827 A0 09 ldy #$09
    0829 B9 0C 08 lda $080C,y
    082C 99 FF 03 sta $03FF,y
    082F 88 dey 
    0830 D0 F7 bne #$829
    0832 A9 51 lda #$51
    0834 85 2D sta $2D
    0836 A9 55 lda #$55
    0838 85 2E sta $2E
    083A 4C 00 01 jmp $0100
    */
}
