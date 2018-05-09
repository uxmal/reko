#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class PICRegisterContentTests
    {

        private static SFRDef BuildSFRDef(uint uAddr, string cname, uint nZWidth, uint Impl, string sAccess, string sPOR)
            => new SFRDef()
            {
                AddrFormatted = $"0x{uAddr:x}",
                CName = cname,
                Desc = "",
                ImplFormatted = $"0x{Impl:X}",
                NzWidthFormatted = $"{nZWidth}",
                Access = sAccess,
                MCLR = sPOR,
                POR = sPOR
            };

        private static PICRegisterTraits GetTraits(SFRDef sfr)
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

    }


}
