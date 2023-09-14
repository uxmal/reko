#region License
/* 
 * Copyright (C) 2017-2023 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.MicrochipPIC.Common;
using Reko.Arch.MicrochipPIC.PIC18;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Reko.Core.Lib;

namespace Reko.UnitTests.Arch.Microchip.PIC18
{
    using static Common.Sample;

    [TestFixture]
    public class PIC18ArchitectureTests
    {

        private PICArchitecture GetArch(string picName, PICExecMode mode = PICExecMode.Traditional)
        {
            var arch = new PICArchitecture(new ServiceContainer(), "pic", new Dictionary<string, object>()) { Options = new PICArchitectureOptions(picName, mode) };
            Assert.NotNull(arch);
            arch.CreatePICProcessorModel();
            return arch;
        }

        [Test]
        public void PIC18arch_WrongPIC()
        {
            var picModel = PICProcessorModel.GetModel("PICUnknown");
            Assert.IsNull(picModel);
            picModel = PICProcessorModel.GetModel("");
            Assert.IsNotNull(picModel);
            Assert.AreEqual(PICProcessorModel.DefaultPICName, picModel.PICName);
        }

        [Test]
        public void PIC18arch_CheckArchLegacyFromDBTests()
        {
            var picModel = PICProcessorModel.GetModel(PIC18LegacyName);
            Assert.NotNull(picModel);
            Assert.NotNull(picModel.PICDescriptor);
            var pic = picModel.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18LegacyName, pic.PICName);

            var arch = GetArch(PIC18LegacyName);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picModel.PICDescriptor, arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC18arch_CheckArchEggFromDBTests()
        {
            var picModel = PICProcessorModel.GetModel(PIC18EggName);
            Assert.NotNull(picModel);
            Assert.NotNull(picModel.PICDescriptor);
            var pic = picModel.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18_EXTENDED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18EggName, pic.PICName);

            var arch = GetArch(PIC18EggName);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picModel.PICDescriptor, arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, PICMemoryDescriptor.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void PIC18arch_CheckArchEnhancedFromDBTests()
        {
            var picModel = PICProcessorModel.GetModel(PIC18EnhancedName);
            Assert.NotNull(picModel);
            Assert.NotNull(picModel.PICDescriptor);
            var pic = picModel.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC18_ENHANCED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC18EnhancedName, pic.PICName);


            var arch = GetArch(PIC18EnhancedName);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picModel.PICDescriptor, arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, PICMemoryDescriptor.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC18arch_CheckExecModeLegacyTests()
        {
            var arch = GetArch(PIC18LegacyName);

            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC18arch_CheckExecModeEggTests()
        {
            var arch = GetArch(PIC18EggName);

            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC18arch_CheckExecModeEnhancedTests()
        {
            var arch = GetArch(PIC18EnhancedName);

            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, PICMemoryDescriptor.ExecMode);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC18arch_GetOpcodeNamesTests()
        {
            var arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetMnemonicNames().Keys.Take(6)));
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetMnemonicNames().Keys.Take(6)));
        }

        [Test]
        public void PIC18arch_GetOpcodeNumberTests()
        {
            var arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(Mnemonic.MOVWF, (Mnemonic)arch.GetMnemonicNumber("MOVWF"));
            Assert.AreEqual(Mnemonic.LFSR, (Mnemonic)arch.GetMnemonicNumber("LFSR"));
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Mnemonic.MOVWF, (Mnemonic)arch.GetMnemonicNumber("MOVWF"));
            Assert.AreEqual(Mnemonic.LFSR, (Mnemonic)arch.GetMnemonicNumber("LFSR"));
        }

        [Test]
        public void PIC18arch_FailGetRegisterFromNameTests()
        {
            var arch = GetArch(PIC18LegacyName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.IsNull(arch.GetRegister("invalidregistername"));
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.IsNull(arch.GetRegister("invalidregistername"));

            arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.IsNull(arch.GetRegister("invalidregistername"));
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.IsNull(arch.GetRegister("invalidregistername"));
             
            arch = GetArch(PIC18EnhancedName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.IsNull(arch.GetRegister("invalidregistername"));
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.IsNull(arch.GetRegister("invalidregistername"));
        }

        [Test]
        public void PIC18arch_GetRegisterFromNameTests()
        {
            var arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
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

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
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
            var arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.AreSame(PICRegisters.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PICRegisters.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PICRegisters.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PICRegisters.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            Assert.AreSame(PIC18Registers.INDF2, arch.GetRegister("INDF2"));

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PICRegisters.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PICRegisters.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PICRegisters.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PICRegisters.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            Assert.AreSame(PIC18Registers.FSR1, arch.GetRegister("FSR1"));
        }

        [Test]
        public void PIC18arch_GetSubregisterTests()
        {
            var arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            var lowByte = new BitRange(0, 8);
            var highByte = new BitRange(8, 16);
            var word = new BitRange(0, 16);
            Assert.AreSame(PIC18Registers.FSR0L, arch.GetRegister(PIC18Registers.FSR0.Domain, lowByte));
            Assert.AreSame(PIC18Registers.FSR0H, arch.GetRegister(PIC18Registers.FSR0.Domain, highByte));
            Assert.AreSame(PIC18Registers.FSR0, arch.GetRegister(PIC18Registers.FSR0.Domain, word));

            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.FSR1L, arch.GetRegister(PIC18Registers.FSR1.Domain, lowByte));
            Assert.AreSame(PIC18Registers.FSR1H, arch.GetRegister(PIC18Registers.FSR1.Domain, highByte));
            Assert.AreSame(PIC18Registers.FSR2L, arch.GetRegister(PICRegisters.GetRegister("FSR2").Domain, lowByte));
            Assert.AreSame(PIC18Registers.FSR2H, arch.GetRegister(PICRegisters.GetRegister("FSR2").Domain, highByte));
            Assert.AreSame(PIC18Registers.FSR2, arch.GetRegister(PICRegisters.GetRegister("FSR2").Domain, word));
        }

        [Test]
        public void PIC18arch_GetParentRegisterTests()
        {
            var arch = GetArch(PIC18EggName);
            PICMemoryDescriptor.ExecMode = PICExecMode.Traditional;
            Assert.IsNull(arch.GetParentRegister(PIC18Registers.TOS));
            Assert.AreSame(PIC18Registers.TOS, arch.GetParentRegister(PIC18Registers.TOSL));
            Assert.AreSame(PIC18Registers.TOS, arch.GetParentRegister(PIC18Registers.TOSU));
            PICMemoryDescriptor.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.TOS, arch.GetParentRegister(PIC18Registers.TOSH));
            Assert.IsNull(arch.GetParentRegister(PIC18Registers.WREG));
        }

    }

}
