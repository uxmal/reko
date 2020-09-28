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
    public class PIC16Basic_RegistersTests
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PIC16Basic_RegistersTests()
        {
            PICProcessorModel.GetModel(PIC16BasicName).CreateRegisters();
        }

        [Test]
        public void PIC16Basic_BitOffsetOfRegisters()
        {
            Assert.AreEqual(0, PICRegisters.WREG.BitAddress);
            Assert.AreEqual(0, PICRegisters.PCL.BitAddress);
            Assert.AreEqual(0, PICRegisters.STATUS.BitAddress);
            Assert.AreEqual(0, PIC16Registers.INTCON.BitAddress);
            Assert.AreEqual(0, PIC16BasicRegisters.INDF.BitAddress);
        }

    }

}
