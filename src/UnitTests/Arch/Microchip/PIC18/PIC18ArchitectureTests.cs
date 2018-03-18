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
            var picMode = PICProcessorMode.Create("PICUnknown");
            Assert.IsNull(picMode);
            Assert.Throws<ArgumentNullException>(() => picMode = PICProcessorMode.Create(""));
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
            var picMode = PICProcessorMode.Create(PIC18LegacyName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18LegacyName, pic.Name);
            var arch = new PIC18Architecture("pic", picMode);
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, "pic");
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
            var picMode = PICProcessorMode.Create(PIC18EggName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18EggName, pic.Name);
            var arch = new PIC18Architecture("pic", picMode);
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, "pic");
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
            var picMode = PICProcessorMode.Create(PIC18EnhancedName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18EnhancedName, pic.Name);
            var arch = new PIC18Architecture("pic", picMode);
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, "pic");
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
            var picMode = PICProcessorMode.Create(PIC18LegacyName);
            var arch = new PIC18Architecture("pic", picMode);
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
            var picMode = PICProcessorMode.Create(PIC18EggName);
            var arch = new PIC18Architecture("pic", picMode);
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
            var picMode = PICProcessorMode.Create(PIC18EnhancedName);
            var arch = new PIC18Architecture("pic", picMode);
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
            var picMode = PICProcessorMode.Create(PIC18EggName);
            var arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
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
            var picMode = PICProcessorMode.Create(PIC18EggName);
            var arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
        }

        [Test]
        public void PIC18arch_FailGetRegisterFromNameTests()
        {
            var picMode = PICProcessorMode.Create(PIC18LegacyName);
            var arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

            picMode = PICProcessorMode.Create(PIC18EggName);
            arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

            picMode = PICProcessorMode.Create(PIC18EnhancedName);
            arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

        }

        [Test]
        public void PIC18arch_GetRegisterFromNameTests()
        {
            var picMode = PICProcessorMode.Create(PIC18EggName);
            var arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual("STATUS", arch.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR2L", arch.GetRegister("FSR2L").Name);
            Assert.AreEqual("FSR2H", arch.GetRegister("FSR2H").Name);
            Assert.AreEqual("FSR1L", arch.GetRegister("FSR1L").Name);
            Assert.AreEqual("FSR1H", arch.GetRegister("FSR1H").Name);
            Assert.AreEqual("FSR0L", arch.GetRegister("FSR0L").Name);
            Assert.AreEqual("FSR0H", arch.GetRegister("FSR0H").Name);
            Assert.AreEqual("BSR", arch.GetRegister("BSR").Name);
            Assert.AreEqual("TOSU", arch.GetRegister("TOSU").Name);
            Assert.AreEqual("TOSH", arch.GetRegister("TOSH").Name);
            Assert.AreEqual("TOSL", arch.GetRegister("TOSL").Name);
            Assert.AreEqual("WREG", arch.GetRegister("WREG").Name);
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual("STATUS", arch.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR2L", arch.GetRegister("FSR2L").Name);
            Assert.AreEqual("FSR2H", arch.GetRegister("FSR2H").Name);
            Assert.AreEqual("FSR1L", arch.GetRegister("FSR1L").Name);
            Assert.AreEqual("FSR1H", arch.GetRegister("FSR1H").Name);
            Assert.AreEqual("FSR0L", arch.GetRegister("FSR0L").Name);
            Assert.AreEqual("FSR0H", arch.GetRegister("FSR0H").Name);
            Assert.AreEqual("BSR", arch.GetRegister("BSR").Name);
            Assert.AreEqual("TOSU", arch.GetRegister("TOSU").Name);
            Assert.AreEqual("TOSH", arch.GetRegister("TOSH").Name);
            Assert.AreEqual("TOSL", arch.GetRegister("TOSL").Name);
            Assert.AreEqual("WREG", arch.GetRegister("WREG").Name);
        }

        [Test]
        public void PIC18arch_GetCoreRegisterTests()
        {
            var picMode = PICProcessorMode.Create(PIC18EggName);
            var arch = new PIC18Architecture("pic", picMode) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            Assert.AreSame(PIC18Registers.INDF2, arch.GetRegister("INDF2"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            Assert.AreSame(PIC18Registers.FSR1, arch.GetRegister("FSR1"));
        }

    }

}
