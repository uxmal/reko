#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.OpenRISC;
using Reko.Arch.OpenRISC.Aeon;
using Reko.Core;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class AeonEmulatorTests : EmulatorTestBase
    {
        public AeonEmulatorTests()
            : base(new AeonArchitecture(new ServiceContainer(), "aeon", new()), Address.Ptr32(0x10_000))
        {
        }

        [Test]
        public void AeonEmu_addi()
        {
            base.Given_Code(
                "FCC40002" // bg.addi	r6,r4,0x2
                );

            Emulator.WriteRegister(Registers.GpRegisters[4], 0x1);
            Emulator.WriteRegister(Registers.GpRegisters[6], 0x42000042);
            Emulator.Start();

            Assert.AreEqual(3u, Emulator.ReadRegister(Registers.GpRegisters[6]));
        }

        [Test]
        public void AeonEmu_addi_r0()
        {
            Given_Code(
                "FCC0829F" // bg.addi	r6,r0,-0x7D61
                );

            Emulator.WriteRegister(Registers.GpRegisters[6], 0x42000042);
            Emulator.Start();

            Assert.AreEqual(0xFFFF829Fu, Emulator.ReadRegister(Registers.GpRegisters[6]));
        }

        [Test]
        public void AeonEmu_beq()
        {
            Given_Code(
                "D4 83 00 42", // bg.beq  r4,r3,00010008
                "FC C0 00 04", // bg.addi r6,r0,0x4
                "FC A6 00 00"  // bg.addi r5,r6,0x0
                );

            Emulator.WriteRegister(Registers.GpRegisters[3], 0x42);
            Emulator.WriteRegister(Registers.GpRegisters[4], 0x42);
            Emulator.WriteRegister(Registers.GpRegisters[6], 0x42);
            Emulator.Start();

            Assert.AreEqual(0x42u, Emulator.ReadRegister(Registers.GpRegisters[5]));
        }
    }
}
