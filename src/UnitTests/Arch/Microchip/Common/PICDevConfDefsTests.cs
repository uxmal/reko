#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Arch.Microchip.Common;
using NUnit.Framework;
using Reko.Core;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class PICDevConfDefsTests
    {

        [Test]
        public void PIC16DevConf_Tests()
        {
            IPICDevConfDefs defs = PICDevConfDefs.Create(PICSamples.GetSample(InstructionSetID.PIC16));
            var dcr = defs.GetDCR("CONFIG1");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x2007), dcr.Address);

            var s = defs.Render(dcr, 0xFFFF);
            Assert.AreEqual("FOSC=RC, WDTE=ON, PWRTE=OFF, CP=OFF, BOREN=ON", s);

            var dcf = defs.GetDCRField("WDTE");
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.IsNotNull(dcf);
            Assert.AreSame(dcr.Address, dcf.RegAddress);
        }

        [Test]
        public void PIC16EnhDevConf_Tests()
        {
            IPICDevConfDefs defs = PICDevConfDefs.Create(PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED));
            var dcr = defs.GetDCR("CONFIG1");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x8007), dcr.Address);

            var s = defs.Render(dcr, 0xFFFF);
            Assert.AreEqual("FOSC=ECH, WDTE=ON, PWRTE=OFF, MCLRE=ON, CP=OFF, BOREN=ON, CLKOUTEN=OFF, IESO=ON, FCMEN=ON", s);

            var dcf = defs.GetDCRField("WDTE");
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.IsNotNull(dcf);
            Assert.AreSame(dcr.Address, dcf.RegAddress);
        }

        [Test]
        public void PIC16EnhV1DevConf_Tests()
        {
            IPICDevConfDefs defs = PICDevConfDefs.Create(PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED_V1));
            var dcr = defs.GetDCR("CONFIG1");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x8007), dcr.Address);

            var s = defs.Render(dcr, 0xFFFF);
            Assert.AreEqual("FEXTOSC=ECH, RSTOSC=EXT1X, CLKOUTEN=OFF, CSWEN=ON, FCMEN=ON", s);

            var dcf = defs.GetDCRField("WDTE");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x8009), dcf.RegAddress);
            Assert.AreEqual(5, dcf.BitPos);
            Assert.AreEqual(2, dcf.BitWidth);
        }

        [Test]
        public void PIC18DevConf_Tests()
        {
            IPICDevConfDefs defs = PICDevConfDefs.Create(PICSamples.GetSample(InstructionSetID.PIC18));
            var dcr = defs.GetDCR("CONFIG1H");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x300001), dcr.Address);

            var s = defs.Render(dcr, 0xFF);
            Assert.AreEqual("OSC=RC, FSCM=ON, IESO=ON", s);

            var dcf = defs.GetDCRField("WDT");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDT", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x300003), dcf.RegAddress);
            Assert.AreEqual(0, dcf.BitPos);
            Assert.AreEqual(1, dcf.BitWidth);

            dcf = defs.GetDCRField("XINST");
            Assert.IsNull(dcf);
        }

        [Test]
        public void PIC18ExtdDevConf_Tests()
        {
            IPICDevConfDefs defs = PICDevConfDefs.Create(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            var dcr = defs.GetDCR("CONFIG1H");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x300001), dcr.Address);

            var s = defs.Render(dcr, 0xFF);
            Assert.AreEqual("FOSC=<invalid>, PCLKEN=OFF, FCMEN=ON, IESO=ON", s);

            var dcf = defs.GetDCRField("WDTEN");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDTEN", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x300003), dcf.RegAddress);
            Assert.AreEqual(0, dcf.BitPos);
            Assert.AreEqual(2, dcf.BitWidth);

            dcf = defs.GetDCRField("XINST");
            Assert.IsNotNull(dcf);
        }

        [Test]
        public void PIC18EnhdDevConf_Tests()
        {
            IPICDevConfDefs defs = PICDevConfDefs.Create(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            var dcr = defs.GetDCR("CONFIG1H");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x300001), dcr.Address);

            var s = defs.Render(dcr, 0xFF);
            Assert.AreEqual("CLKOUTEN=OFF, PR1WAY=ON, CSWEN=OFF, FCMEN=ON", s);

            var dcf = defs.GetDCRField("WDTE");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x300004), dcf.RegAddress);
            Assert.AreEqual(5, dcf.BitPos);
            Assert.AreEqual(2, dcf.BitWidth);

            dcf = defs.GetDCRField("XINST");
            Assert.IsNotNull(dcf);
        }

    }
}
