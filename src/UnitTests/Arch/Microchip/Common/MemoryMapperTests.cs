using Microchip.Crownking;
using Microchip.MemoryMapper;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC18;
using System;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    [TestFixture]
    public class MemoryMapperTests
    {

        private static PIC18Architecture arch;

        [Test]
        public void PIC18MemoryMapper_TraditionalTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "No code");
            Assert.IsTrue(map.HasConfigFuse, "No configuration fuses");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    Assert.AreEqual(ip, map.RemapProgramAddr(iv), $"Mismap of program address 0x{iv:X} to 0x{ip:X}");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item1), $"Mismatch address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item2 - 1), $"Mismatch address for program region '{rgn.RegionName}'");
            }

            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "No SFR");
            Assert.IsTrue(map.HasGPR, "No GPR");
            Assert.AreEqual(PICMemoryMapper.NOPHYSICAL_MEM, map.RemapDataAddr(arch.PICDescriptor.DataSpace.EndAddr));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                    Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                    for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == ip) || (ipp == PICMemoryMapper.NOPHYSICAL_MEM), $"Mismap of data address 0x{iv:X} to 0x{ip:X}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item1), $"Mismatch address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item2 - 1), $"Mismatch address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC18MemoryMapper_ExtendedTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18, map.InstructionSetID);

            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "No code");
            Assert.IsTrue(map.HasConfigFuse, "No configuration fuses");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    Assert.AreEqual(ip, map.RemapProgramAddr(iv), $"Mismap of program address 0x{iv:X} to 0x{ip:X}");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item1), $"Mismatch address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item2 - 1), $"Mismatch address for program region '{rgn.RegionName}'");
            }

            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "No SFR");
            Assert.IsTrue(map.HasGPR, "No GPR");
            Assert.AreEqual(PICMemoryMapper.NOPHYSICAL_MEM, map.RemapDataAddr(arch.PICDescriptor.DataSpace.EndAddr));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                    Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                    for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == ip) || (ipp == PICMemoryMapper.NOPHYSICAL_MEM), $"Mismap of data address 0x{iv:X} to 0x{ip:X}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item1), $"Mismatch address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item2 - 1), $"Mismatch address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC18ExtdMemoryMapper_TraditionalTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "No code");
            Assert.IsTrue(map.HasConfigFuse, "No configuration fuses");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    Assert.AreEqual(ip, map.RemapProgramAddr(iv), $"Mismap of program address 0x{iv:X} to 0x{ip:X}");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item1), $"Mismatch address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item2 - 1), $"Mismatch address for program region '{rgn.RegionName}'");
            }

            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "No SFR");
            Assert.IsTrue(map.HasGPR, "No GPR");
            Assert.AreEqual(PICMemoryMapper.NOPHYSICAL_MEM, map.RemapDataAddr(arch.PICDescriptor.DataSpace.EndAddr));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                    Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                    for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == ip) || (ipp == PICMemoryMapper.NOPHYSICAL_MEM), $"Mismap of data address 0x{iv:X} to 0x{ip:X}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item1), $"Mismatch address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item2 - 1), $"Mismatch address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC18ExtdMemoryMapper_ExtendedTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, map.InstructionSetID);

            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "No code");
            Assert.IsTrue(map.HasConfigFuse, "No configuration fuses");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    Assert.AreEqual(ip, map.RemapProgramAddr(iv), $"Mismap of program address 0x{iv:X} to 0x{ip:X}");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item1), $"Mismatch address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item2 - 1), $"Mismatch address for program region '{rgn.RegionName}'");
            }

            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "No SFR");
            Assert.IsTrue(map.HasGPR, "No GPR");
            Assert.AreEqual(PICMemoryMapper.NOPHYSICAL_MEM, map.RemapDataAddr(arch.PICDescriptor.DataSpace.EndAddr));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                    Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                    for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == ip) || (ipp == PICMemoryMapper.NOPHYSICAL_MEM), $"Mismap of data address 0x{iv:X} to 0x{ip:X}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item1), $"Mismatch address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item2 - 1), $"Mismatch address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC18EnhdMemoryMapper_TraditionalTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "No code");
            Assert.IsTrue(map.HasConfigFuse, "No configuration fuses");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    Assert.AreEqual(ip, map.RemapProgramAddr(iv), $"Mismap of program address 0x{iv:X} to 0x{ip:X}");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item1), $"Mismatch address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item2 - 1), $"Mismatch address for program region '{rgn.RegionName}'");
            }

            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "No SFR");
            Assert.IsTrue(map.HasGPR, "No GPR");
            Assert.AreEqual(PICMemoryMapper.NOPHYSICAL_MEM, map.RemapDataAddr(arch.PICDescriptor.DataSpace.EndAddr));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                    Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                    for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == ip) || (ipp == PICMemoryMapper.NOPHYSICAL_MEM), $"Mismap of data address 0x{iv:X} to 0x{ip:X}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item1), $"Mismatch address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item2 - 1), $"Mismatch address for data region '{rgn.RegionName}'");
                }
            }
        }

        [Test]
        public void PIC18EnhdMemoryMapper_ExtendedTests()
        {
            arch = new PIC18Architecture(PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED));
            IPICMemoryMapper map = arch.MemoryMapper;
            map.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, map.ExecMode);
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, map.InstructionSetID);

            Assert.IsNotNull(map.ProgramRegions);
            Assert.IsTrue(map.HasCode, "No code");
            Assert.IsTrue(map.HasConfigFuse, "No configuration fuses");
            foreach (var rgn in map.ProgramRegions)
            {
                Assert.AreEqual(MemoryDomain.Prog, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetProgramRegion(rgn.RegionName));
                Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    Assert.AreEqual(ip, map.RemapProgramAddr(iv), $"Mismap of program address 0x{iv:X} to 0x{ip:X}");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item1), $"Mismatch address for program region '{rgn.RegionName}'");
                Assert.AreEqual(rgn, map.GetProgramRegion(virtualrange.Item2 - 1), $"Mismatch address for program region '{rgn.RegionName}'");
            }

            Assert.IsNotNull(map.DataRegions);
            Assert.IsTrue(map.HasSFR, "No SFR");
            Assert.IsTrue(map.HasGPR, "No GPR");
            Assert.AreEqual(PICMemoryMapper.NOPHYSICAL_MEM, map.RemapDataAddr(arch.PICDescriptor.DataSpace.EndAddr));
            foreach (var rgn in map.DataRegions)
            {
                Assert.AreEqual(MemoryDomain.Data, rgn.TypeOfMemory);
                Assert.IsNotNull(map.GetDataRegion(rgn.RegionName));
                if (rgn.SubtypeOfMemory != MemorySubDomain.NNMR)
                {
                    Assert.IsTrue(rgn.Size > 0, $"Invalid size for '{rgn.RegionName}' = {rgn.Size}");
                    Tuple<int, int> virtualrange = rgn.VirtualByteAddress;
                    Tuple<int, int> physrange = rgn.PhysicalByteAddress;
                    for (int iv = virtualrange.Item1, ip = physrange.Item1; iv < virtualrange.Item2; iv++, ip++)
                    {
                        var ipp = map.RemapDataAddr(iv);
                        Assert.IsTrue((ipp == ip) || (ipp == PICMemoryMapper.NOPHYSICAL_MEM), $"Mismap of data address 0x{iv:X} to 0x{ip:X}");
                    }
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item1), $"Mismatch address for data region '{rgn.RegionName}'");
                    Assert.AreEqual(rgn, map.GetDataRegion(virtualrange.Item2 - 1), $"Mismatch address for data region '{rgn.RegionName}'");
                }
            }
        }

    }
}
