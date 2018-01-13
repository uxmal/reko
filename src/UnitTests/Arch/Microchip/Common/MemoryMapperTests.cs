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

using Microchip.Crownking;
using Microchip.MemoryMapper;
using NUnit.Framework;
using Reko.Core;
using System;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class MemoryMapperTests
    {

        private void CheckProgMap(IPICMemoryMapper map)
        {
            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "Missing Code region");
            Assert.IsTrue(map.HasConfigFuse, "Missing Configuration Fuses region");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                AddressRange virtualrange = rgn.LogicalByteAddress;
                for (Address iv = virtualrange.Begin; iv < virtualrange.End; iv += 1)
                {
                    var ipp = map.RemapProgramAddr(iv);
                    Assert.IsTrue((ipp == null) || (map.GetProgramRegion(ipp) != null), $"Wrong (null) mapping of program address {iv}.");
                }
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Begin), $"Mismatch begin address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.End - 1), $"Mismatch end address for program region '{rgn.RegionName}'");
            }
        }

        private void CheckDataMap(IPICMemoryMapper map)
        {
            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "Missing SFR region");
            Assert.IsTrue(map.HasGPR, "Missing GPR region");
            Assert.IsNull(map.RemapDataAddr(Address.Ptr32(0x40000)));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    AddressRange virtualrange = rgn.LogicalByteAddress;
                    AddressRange physrange = rgn.PhysicalByteAddress;
                    for (Address iv = virtualrange.Begin; iv < virtualrange.End; iv += 1)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == null) || (map.GetDataRegion(ipp) != null), $"Wrong (null) mapping of data address {iv}.");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Begin), $"Mismatch begin address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.End - 1), $"Mismatch end address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC16MemoryMapper_TraditionalTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC16));
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC16MemoryMapper_EnhancedTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED));
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC16MemoryMapper_EnhancedV1Tests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED_V1));
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC16_ENHANCED_V1, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18MemoryMapper_TraditionalTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC18));
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18MemoryMapper_ExtendedTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC18));
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18ExtdMemoryMapper_TraditionalTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18ExtdMemoryMapper_ExtendedTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18EnhdMemoryMapper_TraditionalTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18EnhdMemoryMapper_ExtendedTests()
        {
            IPICMemoryMapper map = PICMemoryMapper.Create(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

    }
}
