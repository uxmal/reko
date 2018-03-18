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
 
using Reko.Libraries.Microchip;
using NUnit.Framework;
using Reko.Arch.Microchip.Common;
using Reko.Arch.Microchip.PIC18;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC18
{
    using static Common.Sample;

    [TestFixture]
    public class PIC18ArchitectureTests
    {

        [Test]
        public void PIC18arch_WrongPIC()
        {
            var picMode = PICProcessorMode.GetMode("PICUnknown");
            Assert.IsNull(picMode);
            Assert.Throws<ArgumentNullException>(() => picMode = PICProcessorMode.GetMode(""));
        }

        [Test]
        public void PIC18arch_NullPICMode()
        {
            PIC18Architecture arch;
            Assert.Throws<ArgumentNullException>(() => arch = new PIC18Architecture("pic", null));
        }

        [Test]
        public void PIC18arch_CheckArchLegacyFromDBTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC18LegacyName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18LegacyName, pic.Name);
            var arch = picMode.CreateArchitecture();
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, picMode.ArchitectureID);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picMode.PICDescriptor, arch.PICDescriptor);
            Assert.NotNull(arch.MemoryDescriptor);
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            Assert.NotNull(arch.MemoryDescriptor);
        }

        [Test]
        public void PIC18arch_CheckArchEggFromDBTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC18EggName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18EggName, pic.Name);
            var arch = picMode.CreateArchitecture();
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, picMode.ArchitectureID);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picMode.PICDescriptor, arch.PICDescriptor);
            Assert.NotNull(arch.MemoryDescriptor);
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, arch.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            Assert.NotNull(arch.MemoryDescriptor);
        }

        [Test]
        public void PIC18arch_CheckArchEnhancedFromDBTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC18EnhancedName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18EnhancedName, pic.Name);

            var arch = picMode.CreateArchitecture();
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, picMode.ArchitectureID);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picMode.PICDescriptor, arch.PICDescriptor);
            Assert.NotNull(arch.MemoryDescriptor);
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, arch.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            Assert.NotNull(arch.MemoryDescriptor);
        }

        [Test]
        public void PIC18arch_CheckExecModeLegacyTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18LegacyName).CreateArchitecture();
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);
        }

        [Test]
        public void PIC18arch_CheckExecModeEggTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18EggName).CreateArchitecture();
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, arch.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

        }

        [Test]
        public void PIC18arch_CheckExecModeEnhancedTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18EnhancedName).CreateArchitecture();
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, arch.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);
        }

        [Test]
        public void PIC18arch_GetOpcodeNamesTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18EggName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
        }

        [Test]
        public void PIC18arch_GetOpcodeNumberTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18EggName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
        }

        [Test]
        public void PIC18arch_FailGetRegisterFromNameTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18LegacyName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));

            arch = PICProcessorMode.GetMode(PIC18EggName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));

            arch = PICProcessorMode.GetMode(PIC18EnhancedName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));

        }

        [Test]
        public void PIC18arch_GetRegisterFromNameTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18EggName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual("STATUS", PICRegisters.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR2L", PICRegisters.GetRegister("FSR2L").Name);
            Assert.AreEqual("FSR2H", PICRegisters.GetRegister("FSR2H").Name);
            Assert.AreEqual("FSR1L", PICRegisters.GetRegister("FSR1L").Name);
            Assert.AreEqual("FSR1H", PICRegisters.GetRegister("FSR1H").Name);
            Assert.AreEqual("FSR0L", PICRegisters.GetRegister("FSR0L").Name);
            Assert.AreEqual("FSR0H", PICRegisters.GetRegister("FSR0H").Name);
            Assert.AreEqual("BSR", PICRegisters.GetRegister("BSR").Name);
            Assert.AreEqual("TOSU", PICRegisters.GetRegister("TOSU").Name);
            Assert.AreEqual("TOSH", PICRegisters.GetRegister("TOSH").Name);
            Assert.AreEqual("TOSL", PICRegisters.GetRegister("TOSL").Name);
            Assert.AreEqual("WREG", PICRegisters.GetRegister("WREG").Name);
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual("STATUS", PICRegisters.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR2L", PICRegisters.GetRegister("FSR2L").Name);
            Assert.AreEqual("FSR2H", PICRegisters.GetRegister("FSR2H").Name);
            Assert.AreEqual("FSR1L", PICRegisters.GetRegister("FSR1L").Name);
            Assert.AreEqual("FSR1H", PICRegisters.GetRegister("FSR1H").Name);
            Assert.AreEqual("FSR0L", PICRegisters.GetRegister("FSR0L").Name);
            Assert.AreEqual("FSR0H", PICRegisters.GetRegister("FSR0H").Name);
            Assert.AreEqual("BSR", PICRegisters.GetRegister("BSR").Name);
            Assert.AreEqual("TOSU", PICRegisters.GetRegister("TOSU").Name);
            Assert.AreEqual("TOSH", PICRegisters.GetRegister("TOSH").Name);
            Assert.AreEqual("TOSL", PICRegisters.GetRegister("TOSL").Name);
            Assert.AreEqual("WREG", PICRegisters.GetRegister("WREG").Name);
        }

        [Test]
        public void PIC18arch_GetCoreRegisterTests()
        {
            var arch = PICProcessorMode.GetMode(PIC18EggName).CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreSame(PIC18Registers.WREG, PICRegisters.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, PICRegisters.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, PICRegisters.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, PICRegisters.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, PICRegisters.GetRegister("INDF0"));
            Assert.AreSame(PIC18Registers.INDF2, PICRegisters.GetRegister("INDF2"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.WREG, PICRegisters.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, PICRegisters.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, PICRegisters.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, PICRegisters.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, PICRegisters.GetRegister("INDF0"));
            Assert.AreSame(PIC18Registers.FSR1, PICRegisters.GetRegister("FSR1"));
        }

    }

}
