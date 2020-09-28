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
using Reko.Libraries.Microchip;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class PICRegisterContentTests
    {
        internal class DummySFRDef : ISFRRegister
        {
            public DummySFRDef() { }

            public int ByteWidth { get; set; }
            public uint ImplMask { get; set; }
            public string AccessBits { get; set; }
            public string MCLR { get; set; }
            public string POR { get; set; }
            public bool IsIndirect { get; set; }
            public bool IsVolatile { get; set; }
            public bool IsHidden { get; set; }
            public bool IsLangHidden { get; set; }
            public bool IsIDEHidden { get; set; }
            public string NMMRID { get; set; }
            public bool IsNMMR => throw new System.NotImplementedException();
            public IEnumerable<ISFRBitField> BitFields => throw new System.NotImplementedException();
            public uint Addr { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public byte BitWidth { get; set; }
        }


        private static ISFRRegister BuildSFRDef(uint uAddr, string cname, byte nZWidth, uint Impl, string sAccess, string sPOR)
            => new DummySFRDef()
            {
                Addr = uAddr,
                Name = cname,
                Description = "",
                ImplMask = Impl,
                BitWidth = nZWidth,
                AccessBits = sAccess,
                MCLR = sPOR,
                POR = sPOR
            };

        private static PICRegisterTraits GetTraits(ISFRRegister sfr)
            => new PICRegisterTraits(sfr);


        [Test]
        public void PICRegisterContent_FullReg8Test()
        {
            var reg = new PICRegisterContent(GetTraits(BuildSFRDef(0, "Reg8", 8, 0xFF, new string('n', 8), "")));
            Assert.NotNull(reg);
            Assert.AreEqual(0U, reg.ResetValue);
            Assert.AreEqual(0U, reg.ActualValue);
            reg.ActualValue = 0x55;
            Assert.AreEqual(0x55U, reg.ActualValue);
            reg.ActualValue = 0xAA;
            Assert.AreEqual(0xAAU, reg.ActualValue);
            reg.ActualValue = 0xAA55;
            Assert.AreEqual(0x55U, reg.ActualValue);
        }

        [Test]
        public void PICRegisterContent__RRRWWW_Test()
        {
            var reg = new PICRegisterContent(GetTraits(BuildSFRDef(0, "Reg8", 8, 0x7E, "-rrrwww-", "-010001-")));
            Assert.NotNull(reg);
            Assert.AreEqual(0x22U, reg.ResetValue);
            Assert.AreEqual(0x20U, reg.ActualValue);
            reg.ActualValue = 0xFF;
            Assert.AreEqual(0x00U, reg.ActualValue);
        }

        [Test]
        public void PICRegisterContent__NNNWWW_Test()
        {
            var reg = new PICRegisterContent(GetTraits(BuildSFRDef(0, "Reg8", 8, 0x7E, "-nnnwww-", "-010001-")));
            Assert.NotNull(reg);
            Assert.AreEqual(0x22U, reg.ResetValue);
            Assert.AreEqual(0x20U, reg.ActualValue);
            reg.ActualValue = 0xFF;
            Assert.AreEqual(0x70U, reg.ActualValue);
        }

    }


}
