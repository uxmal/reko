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
using Reko.Arch.Microchip.Common;
using Reko.Arch.Microchip.PIC16;
using Reko.Libraries.Microchip;
using System.Collections.Generic;
using System;

namespace Reko.UnitTests.Arch.Microchip.PIC16
{
    using static Common.Sample;

    [TestFixture]
    public class PIC16Registers_All_Tests
    {
        static PICCrownking db = PICCrownking.GetDB();

        private IEnumerable<PIC> GetSelectedPIC(InstructionSetID isID)
        {
            foreach (var spic in db.EnumPICList((p) => p.StartsWith("PIC16")))
            {
                var pic = db.GetPICAsXML(spic).ToObject<PIC>();
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
                    PICProcessorMode.GetMode(pic.Name).CreateRegisters();
                    Assert.IsNotNull(PIC16Registers.STATUS, $"Null status register for '{pic.Name}'");
                    Assert.IsNotNull(PIC16BasicRegisters.RP0, $"Null RP0 register for '{pic.Name}'");
                    Assert.IsNotNull(PIC16BasicRegisters.RP1, $"Null RP1 register for '{pic.Name}'");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"'{pic.Name}': wrong registers creation.", ex);
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
                    PICProcessorMode.GetMode(pic.Name).CreateRegisters();
                    Assert.IsNotNull(PIC16EnhancedRegisters.STATUS, $"Null status register for '{pic.Name}'");
                    Assert.IsNotNull(PIC16EnhancedRegisters.BSR, $"Null status register for '{pic.Name}'");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"'{pic.Name}': wrong registers creation.", ex);
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
                    PICProcessorMode.GetMode(pic.Name).CreateRegisters();
                    Assert.IsNotNull(PIC16FullRegisters.STATUS, $"Null status register for '{pic.Name}'");
                    Assert.IsNotNull(PIC16FullRegisters.BSR, $"Null status register for '{pic.Name}'");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"'{pic.Name}': wrong registers creation.", ex);
                }
            }
        }
    }

}
