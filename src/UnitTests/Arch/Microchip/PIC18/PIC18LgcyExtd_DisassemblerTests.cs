#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work of:
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
using Reko.Arch.MicrochipPIC.Common;

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

    [TestFixture]
    public class PIC18LgcyExtd_DisassemblerTests : DisassemblerTestsBase
    {

        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC18LegacyName, PICExecMode.Extended);
        }

        [Test]
        public void PIC18LgcyExtd_Disasm_CantBeExtend()
        {
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
        }

        [Test]
        public void PIC18LgcyExtd_Disasm_StillInTradMode()
        {
            VerifyDisasm("ADDWF\t0x12,W,ACCESS", "", 0x2412);
            VerifyDisasm("CPFSEQ\t0x12,ACCESS", "", 0x6212);
            VerifyDisasm("MOVWF\t0x12,ACCESS", "", 0x6E12);
            VerifyDisasm("BCF\t0x03,0,ACCESS", "", 0x9003);
        }
    }

}
