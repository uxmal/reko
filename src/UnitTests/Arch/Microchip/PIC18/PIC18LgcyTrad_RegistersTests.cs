#region License
/* 
 * Copyright (C) 2017-2023 Christian Hostelet.
 * inspired by work of:
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

using Reko.Libraries.Microchip;
using NUnit.Framework;
using Reko.Arch.MicrochipPIC.Common;
using Reko.Arch.MicrochipPIC.PIC18;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Registers
{
    using Common;
    using Reko.Core;
    using static Common.Sample;

    [TestFixture]
    public class PIC18LgcyTrad_RegistersTests : PICRegistersTestsBase
    {
        private readonly BitRange lowByte = new BitRange(0, 8);
        private readonly BitRange highByte = new BitRange(8, 16);
        private readonly BitRange upperByte = new BitRange(16, 24);

        [OneTimeSetUp]
        public void OneSetup()
        {
            SetPICModel(PIC18LegacyName, PICExecMode.Traditional);
        }

        [Test]
        public void PIC18LgcyTrad_Registers_GetRegisterOfFSR0()
        {
            Assert.AreSame(PIC18Registers.FSR0L, arch.GetRegister(PIC18Registers.FSR0.Domain, lowByte));
            Assert.AreSame(PIC18Registers.FSR0H, arch.GetRegister(PIC18Registers.FSR0.Domain, highByte));
        }

        [Test]
        public void PIC18LgcyTrad_Registers_GetRegisterOfFSR1()
        {
            Assert.AreSame(PIC18Registers.FSR1L, arch.GetRegister(PIC18Registers.FSR1.Domain, lowByte));
            Assert.AreSame(PIC18Registers.FSR1H, arch.GetRegister(PIC18Registers.FSR1.Domain, highByte));
        }

        [Test]
        public void PIC18LgcyTrad_Registers_GetRegisterOfFSR2()
        {
            Assert.AreSame(PIC18Registers.FSR2L, arch.GetRegister(PIC18Registers.FSR2.Domain, lowByte));
            Assert.AreSame(PIC18Registers.FSR2H, arch.GetRegister(PIC18Registers.FSR2.Domain, highByte));
        }

        [Test]
        public void PIC18LgcyTrad_Registers_GetRegisterOfTOS()
        {
            Assert.AreSame(PIC18Registers.TOSL, arch.GetRegister(PIC18Registers.TOS.Domain, lowByte));
            Assert.AreSame(PIC18Registers.TOSH, arch.GetRegister(PIC18Registers.TOS.Domain, highByte));
            Assert.AreSame(PIC18Registers.TOSU, arch.GetRegister(PIC18Registers.TOS.Domain, upperByte));
        }

        [Test]
        public void PIC18LgcyTrad_Registers_GetRegisterOfPC()
        {
            Assert.AreSame(PICRegisters.PCL, arch.GetRegister(PIC18Registers.PCLAT.Domain, lowByte));
            Assert.AreSame(PICRegisters.PCLATH, arch.GetRegister(PIC18Registers.PCLAT.Domain, highByte));
            Assert.AreSame(PIC18Registers.PCLATU, arch.GetRegister(PIC18Registers.PCLAT.Domain, upperByte));
        }

        [Test]
        public void PIC18LgcyTrad_Registers_GetRegisterOfTBLPTR()
        {
            Assert.AreSame(PIC18Registers.TBLPTRL, arch.GetRegister(PIC18Registers.TBLPTR.Domain, lowByte));
            Assert.AreSame(PIC18Registers.TBLPTRH, arch.GetRegister(PIC18Registers.TBLPTR.Domain, highByte));
            Assert.AreSame(PIC18Registers.TBLPTRU, arch.GetRegister(PIC18Registers.TBLPTR.Domain, upperByte));
        }

        [Test]
        public void PIC18LgcyTrad_Registers_BitOffsetOfTOS()
        {
            Assert.AreEqual(0, PIC18Registers.TOSL.BitAddress);
            Assert.AreEqual(8, PIC18Registers.TOSH.BitAddress);
            Assert.AreEqual(16, PIC18Registers.TOSU.BitAddress);
        }

        [Test]
        public void PIC18LgcyTrad_Registers_BitOffsetOfPC()
        {
            Assert.AreEqual(0, PICRegisters.PCL.BitAddress);
            Assert.AreEqual(8, PICRegisters.PCLATH.BitAddress);
            Assert.AreEqual(16, PIC18Registers.PCLATU.BitAddress);
        }

    }

}
