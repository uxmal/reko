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
using Reko.Arch.Microchip.PIC18;
using System;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class MemoryMapperTests
    {

        private static PIC18Architecture arch;

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
                AddressRange physrange = rgn.PhysicalByteAddress;
                for (Address iv = virtualrange.Begin, ip = physrange.Begin; iv < virtualrange.End; iv += 1, ip += 1)
                    Assert.IsTrue(ip == map.RemapProgramAddr(iv), $"Wrong mapping of program address {iv} to {ip}");
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
                    for (Address iv = virtualrange.Begin, ip = physrange.Begin; iv < virtualrange.End; iv += 1, ip += 1)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == null) || (ipp == ip), $"Wrong mapping of data address {iv} to {ip}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Begin), $"Mismatch begin address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.End - 1), $"Mismatch end address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC18MemoryMapper_TraditionalTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18));
            IPICMemoryMapper map = arch.MemoryMapper;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18MemoryMapper_ExtendedTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18ExtdMemoryMapper_TraditionalTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            IPICMemoryMapper map = arch.MemoryMapper;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18ExtdMemoryMapper_ExtendedTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18EnhdMemoryMapper_TraditionalTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            IPICMemoryMapper map = arch.MemoryMapper;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

        [Test]
        public void PIC18EnhdMemoryMapper_ExtendedTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            CheckProgMap(map);
            CheckDataMap(map);
        }

    }
}
