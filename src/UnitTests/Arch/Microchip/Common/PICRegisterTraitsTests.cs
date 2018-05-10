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
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class PICRegisterTraitsTests
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

        private static JoinedSFRDef BuildJoinedSFRDef(uint uAddr, string cname, uint nZWidth)
            => new JoinedSFRDef()
            {
                AddrFormatted = $"0x{uAddr:x}",
                CName = cname,
                Desc = "",
                NzWidthFormatted = $"{nZWidth}"
            };

        [Test]
        public void PICRegisterTraits_InvalidRegTest()
        {
            var trait = new PICRegisterTraits();
            Assert.NotNull(trait);
            Assert.AreEqual(8, trait.BitWidth);
            Assert.AreEqual(0xFF, trait.Impl);
            Assert.AreEqual("None", trait.Name);
            Assert.AreEqual("nnnnnnnn", trait.Access);
            Assert.AreEqual("uuuuuuuu", trait.POR);
            Assert.AreEqual("uuuuuuuu", trait.MCLR);
            Assert.IsFalse(trait.IsVolatile);
            Assert.IsFalse(trait.IsIndirect);
        }

        [Test]
        public void PICRegisterTraits_Reg8Test()
        {
            var trait = new PICRegisterTraits(BuildSFRDef(0, "Reg8", 8, 0x7F, "r-rrwnn", "10"));
            Assert.NotNull(trait);
            Assert.AreEqual(8, trait.BitWidth);
            Assert.AreEqual(0x7F, trait.Impl);
            Assert.AreEqual("Reg8", trait.Name);
            Assert.AreEqual("-r-rrwnn", trait.Access);
            Assert.AreEqual("00000010", trait.POR);
            Assert.AreEqual("uuuuuu10", trait.MCLR);
            Assert.IsFalse(trait.IsVolatile);
            Assert.IsFalse(trait.IsIndirect);
        }

        [Test]
        public void PICRegisterTraits_FSRTest()
        {
            var fsrl = new PICRegisterTraits(BuildSFRDef(0, "FSRL", 8, 0xFF, "nnnnnnnn", "00000000"));
            var fsrh = new PICRegisterTraits(BuildSFRDef(1, "FSRH", 8, 0x1F, "nnnnn", "00000"));
            var fsr = BuildJoinedSFRDef(0, "FSR", 16);
            var trait = new PICRegisterTraits(fsr, new List<PICRegisterTraits>() { fsrl, fsrh });
            Assert.NotNull(trait);
            Assert.AreEqual(16, trait.BitWidth);
            Assert.AreEqual(0x1FFF, trait.Impl);
            Assert.AreEqual("---nnnnnnnnnnnnn", trait.Access);
            Assert.AreEqual("0000000000000000", trait.POR);
            Assert.AreEqual("uuu0000000000000", trait.MCLR);
            Assert.IsFalse(trait.IsVolatile);
            Assert.IsFalse(trait.IsIndirect);
        }

        [Test]
        public void PICRegisterTraits_FlagsTest()
        {
            var sfrl = BuildSFRDef(0, "REGL", 8, 0x7F, "-nnnnnnn", "-0000000");
            sfrl.IsIndirect = true;
            var regl = new PICRegisterTraits(sfrl);
            var sfrh = BuildSFRDef(1, "REGH", 8, 0x1F, "nnnnn", "00000");
            sfrh.IsVolatile = true;
            var regh = new PICRegisterTraits(sfrh);
            var reg = BuildJoinedSFRDef(0, "REG", 16);
            var trait = new PICRegisterTraits(reg, new List<PICRegisterTraits>() { regl, regh });
            Assert.NotNull(trait);
            Assert.AreEqual(16, trait.BitWidth);
            Assert.AreEqual(0x1F7F, trait.Impl);
            Assert.AreEqual("---nnnnn-nnnnnnn", trait.Access);
            Assert.AreEqual("00000000-0000000", trait.POR);
            Assert.AreEqual("uuu00000-0000000", trait.MCLR);
            Assert.IsTrue(trait.IsVolatile);
            Assert.IsTrue(trait.IsIndirect);
        }

    }

}
