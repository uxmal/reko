#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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
using Reko.Libraries.Microchip;

// As of today there are 3 flavors of PIC18 with potentially 2 execution modes:
//  - legacy (a.k.a. "pic18"), traditional execution mode
//  - extended (a.k.a. "egg"), traditional/extended execution modes
//  - enhanced (a.k.a. "cpu_pic18f_v6"), traditional/extended execution modes
// This means we have 5 different contexts to check.
// In addition the extended and enhanced PIC18 disassemblers use the legacy disassembler for legacy instructions.
//
namespace Reko.UnitTests.Arch.Microchip.PIC18.Disasm
{
    using Common;
    using static Common.Sample;

    // The only additional instruction for this PIC18 in traditional execution mode is 'CALLW'.
    // 
    [TestFixture]
    public class PIC18EggTrad_DisassemblerTests : DisassemblerTestsBase
    {
        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC18EggName, PICExecMode.Traditional);
        }

        [Test]
        public void PIC18EggTrad_Disasm_CheckSomeLgcy()
        {
            VerifyDisasm("ADDWF\t0x12,W,ACCESS", "", 0x2412);
            VerifyDisasm("CPFSEQ\t0x12,ACCESS", "", 0x6212);
            VerifyDisasm("MOVWF\t0x12,ACCESS", "", 0x6E12);
            VerifyDisasm("BCF\t0x03,0,ACCESS", "", 0x9003);
        }

        [Test]
        public void PIC18EggTrad_Disasm_CALLW()
        {
            VerifyDisasm("CALLW", "", 0x0014);
        }

        [Test]
        public void PIC18EggTrad_Disasm_Invalids()
        {
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0140);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0180);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x01E0);
            VerifyDisasm("invalid", "(MOVSFL) unsupported instruction", 0x0002, 0xF000, 0xF000);
            VerifyDisasm("invalid", "(MOVFFL) unsupported instruction", 0x006F, 0xF000, 0xF000);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0110);
            VerifyDisasm("invalid", "(MOVLB) too large value", 0x0120);
            VerifyDisasm("invalid", "(ADDFSR) not supported instruction", 0xE800);
            VerifyDisasm("invalid", "(ADDULNK) not supported instruction", 0xE8C0);
            VerifyDisasm("invalid", "(SUBFSR) not supported instruction", 0xE900);
            VerifyDisasm("invalid", "(SUBULNK) not supported instruction", 0xE9C0);
            VerifyDisasm("invalid", "(PUSHL) not supported instruction", 0xEA00);
            VerifyDisasm("invalid", "(ADDULNK) not supported instruction", 0xEB00, 0xF000);
            VerifyDisasm("invalid", "(MOVSS) not supported instruction", 0xEB80, 0xF000);
            VerifyDisasm("invalid", "(LFSR) too large value", 0xEE00, 0xF234);
        }

    }

}
