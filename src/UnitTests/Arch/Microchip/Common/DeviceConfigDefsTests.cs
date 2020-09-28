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
using Reko.Core;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    using static Common.Sample;

    [TestFixture]
    public class DeviceConfigDefsTests
    {

        [Test]
        public void PIC16DevConf_Tests()
        {
            PICProcessorModel.GetModel(PIC16BasicName).CreateMemoryDescriptor();
            var dcr = PICMemoryDescriptor.GetDCR("CONFIG");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x2007), dcr.Address);

            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(dcr, 0xFFFF);
            Assert.AreEqual("FOSC=EXTRC, WDTE=ON, PWRTE=OFF, CP=OFF", s);

            var dcf = PICMemoryDescriptor.GetDCRField("WDTE");
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.IsNotNull(dcf);
            Assert.AreSame(dcr.Address, dcf.RegAddress);
        }

        [Test]
        public void PIC16EnhDevConf_Tests()
        {
            PICProcessorModel.GetModel(PIC16EnhancedName).CreateMemoryDescriptor();
            var dcr = PICMemoryDescriptor.GetDCR("CONFIG1");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x8007), dcr.Address);

            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(dcr, 0xFFFF);
            Assert.AreEqual("FOSC=ECH, WDTE=ON, PWRTE=OFF, MCLRE=ON, CP=OFF, BOREN=ON, CLKOUTEN=OFF, IESO=ON, FCMEN=ON", s);

            var dcf = PICMemoryDescriptor.GetDCRField("WDTE");
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.IsNotNull(dcf);
            Assert.AreSame(dcr.Address, dcf.RegAddress);
        }

        [Test]
        public void PIC16FullDevConf_Tests()
        {
            PICProcessorModel.GetModel(PIC16FullFeaturedName).CreateMemoryDescriptor();
            var dcr = PICMemoryDescriptor.GetDCR("CONFIG4");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x800A), dcr.Address);

            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(dcr, 0xFFFF);
            Assert.AreEqual("BBSIZE=BB512, BBEN=OFF, SAFEN=OFF, WRTAPP=OFF, WRTB=OFF, WRTC=OFF, WRTSAF=OFF, LVP=ON", s);

            var dcf = PICMemoryDescriptor.GetDCRField("WDTE");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x8009), dcf.RegAddress);
            Assert.AreEqual(5, dcf.BitPos);
            Assert.AreEqual(2, dcf.BitWidth);
        }

        [Test]
        public void PIC18DevConf_Tests()
        {
            PICProcessorModel.GetModel(PIC18LegacyName).CreateMemoryDescriptor();
            var dcr = PICMemoryDescriptor.GetDCR("CONFIG1H");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x300001), dcr.Address);

            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(dcr, 0xFF);
            Assert.AreEqual("OSC=RC, FSCM=ON, IESO=ON", s);

            var dcf = PICMemoryDescriptor.GetDCRField("WDT");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDT", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x300003), dcf.RegAddress);
            Assert.AreEqual(0, dcf.BitPos);
            Assert.AreEqual(1, dcf.BitWidth);

            dcf = PICMemoryDescriptor.GetDCRField("XINST");
            Assert.IsNull(dcf);
        }

        [Test]
        public void PIC18EggDevConf_Tests()
        {
            PICProcessorModel.GetModel(PIC18EggName).CreateMemoryDescriptor();
            var dcr = PICMemoryDescriptor.GetDCR("CONFIG2L");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x300002), dcr.Address);

            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(dcr, 0xFF);
            Assert.AreEqual("PWRT=OFF, BOR=BOHW, BORV=3", s);

            var dcf = PICMemoryDescriptor.GetDCRField("WDT");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDT", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x300003), dcf.RegAddress);
            Assert.AreEqual(0, dcf.BitPos);
            Assert.AreEqual(1, dcf.BitWidth);

            dcf = PICMemoryDescriptor.GetDCRField("XINST");
            Assert.IsNotNull(dcf);
        }

        [Test]
        public void PIC18EnhdDevConf_Tests()
        {
            PICProcessorModel.GetModel(PIC18EnhancedName).CreateMemoryDescriptor();
            var dcr = PICMemoryDescriptor.GetDCR("CONFIG2L");
            Assert.IsNotNull(dcr);
            Assert.AreEqual(Address.Ptr32(0x300002), dcr.Address);

            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(dcr, 0xFF);
            Assert.AreEqual("MCLRE=EXTMCLR, PWRTS=PWRT_OFF, MVECEN=ON, IVT1WAY=ON, LPBOREN=OFF, BOREN=SBORDIS", s);

            var dcf = PICMemoryDescriptor.GetDCRField("WDTE");
            Assert.IsNotNull(dcf);
            Assert.AreEqual("WDTE", dcf.Name);
            Assert.AreEqual(Address.Ptr32(0x300004), dcf.RegAddress);
            Assert.AreEqual(5, dcf.BitPos);
            Assert.AreEqual(2, dcf.BitWidth);

            dcf = PICMemoryDescriptor.GetDCRField("XINST");
            Assert.IsNotNull(dcf);
        }

    }
}
