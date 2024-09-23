#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.PowerPC;
using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcEmulatorTests
    {
        private readonly ServiceContainer sc;
        private readonly PowerPcArchitecture arch;
        private Reko.Arch.PowerPC.PowerPcEmulator emu;

        public PowerPcEmulatorTests()
        {
            this.sc = new ServiceContainer();
            this.arch = new PowerPcBe64Architecture(sc, "ppc-be-64", new Dictionary<string, object>());
        }

        private void Given_Code(params uint [] uInstrs)
        {
            var writer = new BeImageWriter();
            foreach (uint uInstr in uInstrs)
            {
                writer.WriteBeUInt32(uInstr);
            }
            var mem = new ByteMemoryArea(Address.Ptr64(0x0010_0000), writer.ToArray());
            var seg = new ImageSegment("code", mem, AccessMode.ReadWriteExecute);
            var segmap = new SegmentMap(mem.BaseAddress, seg);
            var program = new Program(new ByteProgramMemory(segmap), arch, new DefaultPlatform(sc, arch));

            var envEmu = new DefaultPlatformEmulator();

            emu = (PowerPcEmulator) arch.CreateEmulator(segmap, envEmu);
            emu.InstructionPointer = program.ImageMap.BaseAddress;
            emu.ExceptionRaised += (sender, e) => { throw e.Exception; };
        }

        [Test]
        public void PPCEmu_rldicr()
        {
            Given_Code(0x798CCFE6);            // rldicr	r12,r12,39,3F

            emu.WriteRegister(arch.Registers[12], 0x12345678);
            emu.Start();

            Assert.AreEqual("F0000000002468AC", emu.ReadRegister(arch.Registers[12]).ToString("X"));
        }

        [Test]
        public void PPCEmu_rldicl()
        {
            Given_Code(0x79290040);              // rldicl	r9,r9,00,01

            emu.WriteRegister(arch.Registers[9], 0xA1234567_89ABCDEF);
            emu.Start();

            Assert.AreEqual("2123456789ABCDEF", emu.ReadRegister(arch.Registers[9]).ToString("X"));
        }

        /*
0x798C27E6<32>              // rldicr	r12,r12,24,3F
0x7929FFE6<32>              // rldicr	r9,r9,3F,3F
0x786A0620<32>              // rldicl	r10,r3,00,38
0x790A0620<32>              // rldicl	r10,r8,00,38
0x798CC7E6<32>              // rldicr	r12,r12,38,3F
0x798C2FE6<32>              // rldicr	r12,r12,25,3F
0x798CBFE6<32>              // rldicr	r12,r12,37,3F
0x798C4FE6<32>              // rldicr	r12,r12,29,3F
0x794B1FE6<32>              // rldicr	r11,r10,23,3F
0x798C47E6<32>              // rldicr	r12,r12,28,3F
0x798C3FE6<32>              // rldicr	r12,r12,27,3F
0x798C67E6<32>              // rldicr	r12,r12,2C,3F
0x798C2FE6<32>              // rldicr	r12,r12,25,3F
0x798CB7E6<32>              // rldicr	r12,r12,36,3F
0x798CC7E6<32>              // rldicr	r12,r12,38,3F
0x798CC7E6<32>              // rldicr	r12,r12,38,3F
0x794A0020<32>              // rldicl	r10,r10,00,20
0x796B0022<32>              // rldicl	r11,r11,20,20
0x796B0020<32>              // rldicl	r11,r11,00,20
0x79290040<32>              // rldicl	r9,r9,00,01
0x794A0040<32>              // rldicl	r10,r10,00,01
0x79680020<32>              // rldicl	r8,r11,00,20
0x79690020<32>              // rldicl	r9,r11,00,20
0x796B0020<32>              // rldicl	r11,r11,00,20
0x797DFFE6<32>              // rldicr	r29,r11,3F,3F
0x796B0020<32>              // rldicl	r11,r11,00,20
0x796B0020<32>              // rldicl	r11,r11,00,20
0x796B0020<32>              // rldicl	r11,r11,00,20
0x794AFFE6<32>              // rldicr	r10,r10,3F,3F
0x79290020<32>              // rldicl	r9,r9,00,20
0x786B0020<32>              // rldicl	r11,r3,00,20
0x788B0020<32>              // rldicl	r11,r4,00,20
0x792807E0<32>              // rldicl	r8,r9,00,3F
0x792807E0<32>              // rldicl	r8,r9,00,3F
        */
    }
}
