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

using NUnit.Framework;
using Reko.Arch.Microchip.Common;
using Reko.Arch.Microchip.PIC16;
using Reko.Arch.Microchip.PIC18;
using Reko.Core;
using Reko.Libraries.Microchip;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    using static Common.Sample;

    [TestFixture]
    public class MemoryMapTests
    {

        private void CheckProgMap(IMemoryMap map)
        {
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                AddressRange virtualrange = rgn.LogicalByteAddrRange;
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Begin), $"Mismatch begin address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.End - 1), $"Mismatch end address for program region '{rgn.RegionName}'");
            }
        }

        private void CheckDataMap(IMemoryMap map)
        {
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    AddressRange virtualrange = rgn.LogicalByteAddrRange;
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Begin), $"Mismatch begin address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.End - 1), $"Mismatch end address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC16MemoryMapper_TraditionalTests()
        {
            var arch = new PIC16Architecture("pic", PICProcessorMode.Create(PIC16BasicName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC16MemoryMapper_EnhancedTests()
        {
            var arch = new PIC16Architecture("pic", PICProcessorMode.Create(PIC16EnhancedName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC16MemoryMapper_EnhancedV1Tests()
        {
            var arch = new PIC16Architecture("pic", PICProcessorMode.Create(PIC16FullFeaturedName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16_FULLFEATURED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18MemoryMapper_TraditionalTests()
        {
            var arch = new PIC18Architecture("pic", PICProcessorMode.Create(PIC18LegacyName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);

            Assert.IsNotNull(map.GetDataRegion("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(map.GetDataRegion("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(map.GetDataRegion("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18MemoryMapper_ExtendedTests()
        {
            var arch = new PIC18Architecture("pic", PICProcessorMode.Create(PIC18LegacyName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
            Assert.IsNotNull(map.GetDataRegion("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(map.GetDataRegion("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(map.GetDataRegion("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18ExtdMemoryMapper_TraditionalTests()
        {
            var arch = new PIC18Architecture("pic", PICProcessorMode.Create(PIC18EggName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
            Assert.IsNotNull(map.GetDataRegion("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(map.GetDataRegion("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(map.GetDataRegion("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18ExtdMemoryMapper_ExtendedTests()
        {
            var arch = new PIC18Architecture("pic", PICProcessorMode.Create(PIC18EggName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
            Assert.IsNull(map.GetDataRegion("accessram"), "Unexpected 'accessram' data memory region for PIC18 extended.");
            Assert.IsNotNull(map.GetDataRegion("gpre"), "Missing 'gpre' data memory region for PIC18.");
            Assert.IsNotNull(map.GetDataRegion("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
        }

        [Test]
        public void PIC18EnhdMemoryMapper_TraditionalTests()
        {
            var arch = new PIC18Architecture("pic", PICProcessorMode.Create(PIC18EnhancedName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
            Assert.IsNotNull(map.GetDataRegion("accessram"), "Missing 'accessram' data memory region for PIC18.");
            Assert.IsNotNull(map.GetDataRegion("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
            Assert.IsNull(map.GetDataRegion("gpre"), "Unexpected 'gpre' data memory region for PIC18 traditional.");
        }

        [Test]
        public void PIC18EnhdMemoryMapper_ExtendedTests()
        {
            var arch = new PIC18Architecture("pic", PICProcessorMode.Create(PIC18EnhancedName));
            IMemoryMap map = arch.MemoryDescriptor.MemoryMap;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
            Assert.IsNull(map.GetDataRegion("accessram"), "Unexpected 'accessram' data memory region for PIC18 extended.");
            Assert.IsNotNull(map.GetDataRegion("gpre"), "Missing 'gpre' data memory region for PIC18.");
            Assert.IsNotNull(map.GetDataRegion("accesssfr"), "Missing 'accesssfr' data memory region for PIC18.");
        }

    }
}
