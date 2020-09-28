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
    public class PICRegisterAccessTests
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


        private static ISFRRegister BuildSFRDef(uint uAddr, string cname, byte nZWidth, uint Impl, string sAccess, string sMCLR)
            => new DummySFRDef() {
                    Addr = uAddr,
                    Name = cname,
                    Description ="",
                    ImplMask = Impl,
                    BitWidth = nZWidth,
                    AccessBits = sAccess,
                    MCLR = sMCLR,
                    POR  = sMCLR
                };

        private static PICRegisterTraits GetTraits(ISFRRegister sfr)
            => new PICRegisterTraits(sfr);

        private static PICRegisterAccessMasks GetAccessMask(byte nZWidth, uint Impl, string sAccess, string sMCLR)
            => PICRegisterAccessMasks.Create(new PICRegisterTraits(BuildSFRDef(0, "Reg", nZWidth, Impl, sAccess, sMCLR)));

        private static PICRegisterAccessMasks GetFullMask(byte nzWidth)
            => PICRegisterAccessMasks.Create(
                new PICRegisterTraits(
                    BuildSFRDef(0, "FullReg", nzWidth, (1U << (int)nzWidth) - 1U, new string('n', (int)nzWidth), new string('0', (int)nzWidth))));

        [Test]
        public void PICRegisterAccess_FullReg8Test()
        {
            var regam = GetFullMask(8);
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask);
            Assert.AreEqual(0x00U, regam.ReadOrMask);
            Assert.AreEqual(0xFFU, regam.ReadAndMask);
            Assert.AreEqual(0x00U, regam.WriteOrMask);
            Assert.AreEqual(0xFFU, regam.WriteAndMask);
            Assert.AreEqual(0x00U, regam.ResetValue);
        }

        [Test]
        public void PICRegisterAccess_FullReg12Test()
        {
            var regam = GetFullMask(12);
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFFU, regam.ImplementedMask);
            Assert.AreEqual(0x000U, regam.ReadOrMask);
            Assert.AreEqual(0xFFFU, regam.ReadAndMask);
            Assert.AreEqual(0x000U, regam.WriteOrMask);
            Assert.AreEqual(0xFFFU, regam.WriteAndMask);
            Assert.AreEqual(0x000U, regam.ResetValue);
        }

        [Test]
        public void PICRegisterAccess_FullReg16Test()
        {
            var regam = GetFullMask(16);
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFFFU, regam.ImplementedMask);
            Assert.AreEqual(0x0000U, regam.ReadOrMask);
            Assert.AreEqual(0xFFFFU, regam.ReadAndMask);
            Assert.AreEqual(0x0000U, regam.WriteOrMask);
            Assert.AreEqual(0xFFFFU, regam.WriteAndMask);
            Assert.AreEqual(0x0000U, regam.ResetValue);
        }

        [Test]
        public void PICRegisterAccess_FullReg24Test()
        {
            var regam = GetFullMask(24);
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFFFFFU, regam.ImplementedMask);
            Assert.AreEqual(0x000000U, regam.ReadOrMask);
            Assert.AreEqual(0xFFFFFFU, regam.ReadAndMask);
            Assert.AreEqual(0x000000U, regam.WriteOrMask);
            Assert.AreEqual(0xFFFFFFU, regam.WriteAndMask);
            Assert.AreEqual(0x000000U, regam.ResetValue);
        }

        [Test]
        public void PICRegisterAccess_RX2PPSTest()
        {
            var regam = GetAccessMask(8, 0x1F, "---nnnnn", "---01111");
            Assert.NotNull(regam);
            Assert.AreEqual(0x1FU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0x1FU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x1FU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x0FU, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_SSP2CLKPPSTest()
        {
            var regam = GetAccessMask(8, 0x1F, "nnnnn", "01001");
            Assert.NotNull(regam);
            Assert.AreEqual(0x1FU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0x1FU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x1FU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x09U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_SSP2BUFTest()
        {
            var regam = GetAccessMask(8, 0xFF, "nnnnnnnn", "xxxxxxxx");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0xFFU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_SSP2STATTest()
        {
            var regam = GetAccessMask(8, 0xFF, "nnrrrrrr", "00000000");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0xC0U, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_SSP2CON1Test()
        {
            var regam = GetAccessMask(8, 0xFF, "rrnnnnnn", "00000000");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x3FU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_SSP2CON2Test()
        {
            var regam = GetAccessMask(8, 0xFF, "nrnnnnnn", "00000000");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0xBFU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_SSP2CON3Test()
        {
            var regam = GetAccessMask(8, 0xFF, "rnnnnnnn", "00000000");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x7FU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_TX2STATest()
        {
            var regam = GetAccessMask(8, 0xFF, "nnnnnnrn", "00000010");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0xFDU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x02U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_BAUD2CONest()
        {
            var regam = GetAccessMask(8, 0xDB, "cr-nn-nn", "01-00-00");
            Assert.NotNull(regam);
            Assert.AreEqual(0xDBU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xDBU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x9BU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x40U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_IPR0Test()
        {
            var regam = GetAccessMask(8, 0x37, "nn-nnn", "11-111");
            Assert.NotNull(regam);
            Assert.AreEqual(0x37U, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0x37U, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x37U, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x37U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_NVMCON1Test()
        {
            var regam = GetAccessMask(8, 0xDF, "nn-snnss", "00-0x000");
            Assert.NotNull(regam);
            Assert.AreEqual(0xDFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0xDFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0xDFU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_NVMCON2Test()
        {
            var regam = GetAccessMask(8, 0xFF, "wwwwwwww", "00000000");
            Assert.NotNull(regam);
            Assert.AreEqual(0xFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x00U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0x00U, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x00U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0xFFU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x00U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

        [Test]
        public void PICRegisterAccess_FSR1Test()
        {
            var regam = GetAccessMask(16, 0xFFF, "----nnnnnnnnnnnn", "----000000000000");
            Assert.NotNull(regam);
            Assert.AreEqual(0x0FFFU, regam.ImplementedMask, $"{nameof(regam.ImplementedMask)}");
            Assert.AreEqual(0x0000U, regam.ReadOrMask, $"{nameof(regam.ReadOrMask)}");
            Assert.AreEqual(0x0FFFU, regam.ReadAndMask, $"{nameof(regam.ReadAndMask)}");
            Assert.AreEqual(0x0000U, regam.WriteOrMask, $"{nameof(regam.WriteOrMask)}");
            Assert.AreEqual(0x0FFFU, regam.WriteAndMask, $"{nameof(regam.WriteAndMask)}");
            Assert.AreEqual(0x0000U, regam.ResetValue, $"{nameof(regam.ResetValue)}");
        }

    }

}
