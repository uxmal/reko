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
using Reko.Libraries.Microchip;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    using static Common.Sample;

    [TestFixture]
    public class MemoryMapTests
    {

        private void CheckProgMap()
        {
            foreach (var rgn in PICMemoryDescriptor.ProgramRegions)
            {
                Assert.AreEqual(PICMemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(PICMemoryDescriptor.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                AddressRange virtualrange = rgn.LogicalByteAddrRange;
                Assert.AreEqual(rgn, PICMemoryDescriptor.GetProgramRegion(virtualrange.Begin), $"Mismatch begin address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, PICMemoryDescriptor.GetProgramRegion(virtualrange.End - 1), $"Mismatch end address for program region '{rgn.RegionName}'");
            }
        }

        private void CheckDataMap()
        {
            foreach (var rgn in PICMemoryDescriptor.DataRegions)
            {
                Assert.AreEqual(PICMemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName(rgn.RegionName));
                if (rgn.SubtypeOfMemory != PICMemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    AddressRange virtualrange = rgn.LogicalByteAddrRange;
                    Assert.AreEqual(rgn, PICMemoryDescriptor.GetDataRegionByAddress(virtualrange.Begin), $"Mismatch begin address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, PICMemoryDescriptor.GetDataRegionByAddress(virtualrange.End - 1), $"Mismatch end address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC16MemoryMapper_TraditionalTests()
        {
            PICProcessorModel.GetModel(PIC16BasicName).CreateMemoryDescriptor();
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
        }

        [Test]
        public void PIC16MemoryMapper_EnhancedTests()
        {
            PICProcessorModel.GetModel(PIC16EnhancedName).CreateMemoryDescriptor();
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16_ENHANCED, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
        }

        [Test]
        public void PIC16MemoryMapper_EnhancedV1Tests()
        {
            PICProcessorModel.GetModel(PIC16FullFeaturedName).CreateMemoryDescriptor();
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16_FULLFEATURED, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
        }

        [Test]
        public void PIC18MemoryMapper_TraditionalTests()
        {
            PICProcessorModel.GetModel(PIC18LegacyName).CreateMemoryDescriptor();
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();

            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(PICMemoryDescriptor.GetDataRegionByName("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18MemoryMapper_ExtendedTests()
        {
            PICProcessorModel.GetModel(PIC18LegacyName).CreateMemoryDescriptor();
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(PICMemoryDescriptor.GetDataRegionByName("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18ExtdMemoryMapper_TraditionalTests()
        {
            PICProcessorModel.GetModel(PIC18EggName).CreateMemoryDescriptor();
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(PICMemoryDescriptor.GetDataRegionByName("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18ExtdMemoryMapper_ExtendedTests()
        {
            PICProcessorModel.GetModel(PIC18EggName).CreateMemoryDescriptor();
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("gpre"), "Missing 'gpre' data memory region for PIC18.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
        }

        [Test]
        public void PIC18EnhdMemoryMapper_TraditionalTests()
        {
            PICProcessorModel.GetModel(PIC18EnhancedName).CreateMemoryDescriptor();
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(PICMemoryDescriptor.GetDataRegionByName("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18EnhdMemoryMapper_ExtendedTests()
        {
            PICProcessorModel.GetModel(PIC18EnhancedName).CreateMemoryDescriptor();
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, PICMemoryDescriptor.InstructionSetID);

            CheckProgMap();
            CheckDataMap();
            Assert.IsNull(PICMemoryDescriptor.GetDataRegionByName("accessram"), "Unexpected 'accessram' data memory region for PIC18 extended.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("gpre"), "Missing 'gpre' data memory region for PIC18.");
            Assert.IsNotNull(PICMemoryDescriptor.GetDataRegionByName("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
        }

    }
}
