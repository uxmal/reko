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
using Reko.Arch.MicrochipPIC.Common;
using Reko.Arch.MicrochipPIC.PIC16;

namespace Reko.UnitTests.Arch.Microchip.PIC16
{
    using static Common.Sample;

    [TestFixture]
    public class PIC16Full_RegistersTests
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PIC16Full_RegistersTests()
        {
            PICProcessorModel.GetModel(PIC16FullFeaturedName).CreateRegisters();
        }

        [Test]
        public void PIC16Full_BitOffsetOfRegisters()
        {
            Assert.AreEqual(0, PICRegisters.WREG.BitAddress);
            Assert.AreEqual(0, PICRegisters.PCL.BitAddress);
            Assert.AreEqual(0, PICRegisters.STATUS.BitAddress);
            Assert.AreEqual(0, PICRegisters.BSR.BitAddress);
            Assert.AreEqual(0, PIC16FullRegisters.FSR0L.BitAddress);
            Assert.AreEqual(8, PIC16FullRegisters.FSR0H.BitAddress);
            Assert.AreEqual(0, PIC16FullRegisters.INDF0.BitAddress);
        }

        [Test]
        public void PIC16Full_GetSubRegisterOfFSR0()
        {
            Assert.AreSame(PIC16FullRegisters.FSR0L, PICRegisters.GetSubregister(PIC16FullRegisters.FSR0, 0, 8));
            Assert.AreSame(PIC16FullRegisters.FSR0H, PICRegisters.GetSubregister(PIC16FullRegisters.FSR0, 8, 8));
        }

        [Test]
        public void PIC16Full_GetSubRegisterOfFSR1()
        {
            Assert.AreSame(PIC16FullRegisters.FSR1L, PICRegisters.GetSubregister(PIC16FullRegisters.FSR1, 0, 8));
            Assert.AreSame(PIC16FullRegisters.FSR1H, PICRegisters.GetSubregister(PIC16FullRegisters.FSR1, 8, 8));
        }

    }

}
