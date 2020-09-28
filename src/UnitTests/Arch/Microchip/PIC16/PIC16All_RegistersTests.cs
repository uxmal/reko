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
using Reko.Libraries.Microchip;
using System.Collections.Generic;
using System;

namespace Reko.UnitTests.Arch.Microchip.PIC16.Registers
{
    using Common;
    using static Common.Sample;

    [TestFixture]
    public class PIC16All_RegistersTests : PICRegistersTestsBase
    {
        static PICCrownking db = PICCrownking.GetDB();

        private IEnumerable<IPICDescriptor> GetSelectedPIC(InstructionSetID isID)
        {
            foreach (var spic in db.EnumPICList((p) => p.StartsWith("PIC16")))
            {
                var pic = db.GetPIC(spic);
                if (pic.GetInstructionSetID == isID)
                    yield return pic;
            }
        }

        [Test]
        public void PIC16Registers_Basic_All_Tests()
        {
            foreach (var pic in GetSelectedPIC(InstructionSetID.PIC16))
            {
                try
                {
                    PICProcessorModel.GetModel(pic.PICName).CreateRegisters();
                    Assert.IsNotNull(PICRegisters.STATUS, $"Null status register for '{pic.PICName}'");
                    Assert.IsNotNull(PIC16BasicRegisters.RP0, $"Null RP0 register for '{pic.PICName}'");
                    Assert.IsNotNull(PIC16BasicRegisters.RP1, $"Null RP1 register for '{pic.PICName}'");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"'{pic.PICName}': Wrong registers creation.", ex);
                }
            }
        }

        [Test]
        public void PIC16Registers_Enhd_All_Tests()
        {
            foreach (var pic in GetSelectedPIC(InstructionSetID.PIC16_ENHANCED))
            {
                try
                {
                    PICProcessorModel.GetModel(pic.PICName).CreateRegisters();
                    Assert.IsNotNull(PICRegisters.STATUS, $"Null status register for '{pic.PICName}'");
                    Assert.IsNotNull(PICRegisters.BSR, $"Null status register for '{pic.PICName}'");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"'{pic.PICName}': wrong registers creation.", ex);
                }
            }
        }

        [Test]
        public void PIC16Registers_Full_All_Tests()
        {
            foreach (var pic in GetSelectedPIC(InstructionSetID.PIC16_FULLFEATURED))
            {
                try
                {
                    PICProcessorModel.GetModel(pic.PICName).CreateRegisters();
                    Assert.IsNotNull(PICRegisters.STATUS, $"Null status register for '{pic.PICName}'");
                    Assert.IsNotNull(PICRegisters.BSR, $"Null status register for '{pic.PICName}'");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"'{pic.PICName}': wrong registers creation.", ex);
                }
            }
        }
    }

}
