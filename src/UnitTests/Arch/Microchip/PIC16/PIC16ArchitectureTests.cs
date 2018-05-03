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
using Reko.Arch.MicrochipPIC.Common;
using Reko.Arch.MicrochipPIC.PIC16;
using Reko.Libraries.Microchip;
using System;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC16
{

    using static Common.Sample;

    [TestFixture]
    public class PIC16ArchitectureTests
    {

        private PICArchitecture GetArch(string picName)
        {
            var arch = new PICArchitecture("pic") { Options = new PICArchitectureOptions(picName, PICExecMode.Traditional) };
            Assert.NotNull(arch);
            arch.CreatePICProcessorModel();
            return arch;
        }

        [Test]
        public void PIC16arch_WrongPICMode()
        {
            var picMode = PICProcessorMode.GetMode("PICUnknown");
            Assert.IsNull(picMode);
            Assert.Throws<ArgumentNullException>(() => picMode = PICProcessorMode.GetMode(""));
        }

        [Test]
        public void PIC16arch_CheckBasicPICFromDBTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16BasicName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC16, pic.GetInstructionSetID);
            Assert.AreEqual(PIC16BasicName, pic.Name);

            var arch = GetArch(PIC16BasicName);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picMode.PICDescriptor, arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC16arch_CheckEnhancedPICFromDBTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16EnhancedName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC16_ENHANCED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC16EnhancedName, pic.Name);

            var arch = GetArch(PIC16EnhancedName);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picMode.PICDescriptor, arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC16arch_CheckFullPICFromDBTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16FullFeaturedName);
            Assert.NotNull(picMode);
            Assert.NotNull(picMode.PICDescriptor);
            var pic = picMode.PICDescriptor;
            Assert.AreEqual(InstructionSetID.PIC16_FULLFEATURED, pic.GetInstructionSetID);
            Assert.AreEqual(PIC16FullFeaturedName, pic.Name);

            var arch = GetArch(PIC16FullFeaturedName);
            Assert.NotNull(arch.PICDescriptor);
            Assert.AreEqual(picMode.PICDescriptor, arch.PICDescriptor);
            Assert.IsTrue(PICMemoryDescriptor.IsValid);
        }

        [Test]
        public void PIC16arch_GetOpcodeNamesTests()
        {
            var arch = GetArch(PIC16BasicName);
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));

            arch = GetArch(PIC16EnhancedName);
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));

            arch = GetArch(PIC16FullFeaturedName);
            Assert.AreEqual(
                "__CONFIG,__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
        }

        [Test]
        public void PIC16arch_GetOpcodeNumberTests()
        {
            var arch = GetArch(PIC16BasicName);
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.ADDWF, (Opcode)arch.GetOpcodeNumber("ADDWF"));

            arch = GetArch(PIC16EnhancedName);
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.ADDWF, (Opcode)arch.GetOpcodeNumber("ADDWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));

            arch = GetArch(PIC16FullFeaturedName);
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.ADDWF, (Opcode)arch.GetOpcodeNumber("ADDWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));
            Assert.AreEqual(Opcode.RESET, (Opcode)arch.GetOpcodeNumber("RESET"));

        }

        [Test]
        public void PIC16arch_FailGetRegisterFromNameTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16BasicName);
            picMode.CreateRegisters();
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));

            picMode = PICProcessorMode.GetMode(PIC16EnhancedName);
            picMode.CreateRegisters();
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));

            picMode = PICProcessorMode.GetMode(PIC16FullFeaturedName);
            picMode.CreateRegisters();
            Assert.Throws<ArgumentException>(() => PICRegisters.GetRegister("invalidregistername"));

        }

        [Test]
        public void PIC16arch_GetBasicRegisterFromNameTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16BasicName);
            picMode.CreateRegisters();
            Assert.AreEqual("STATUS", PICRegisters.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR", PICRegisters.GetRegister("FSR").Name);
            Assert.AreEqual("INDF", PICRegisters.GetRegister("INDF").Name);
            Assert.AreEqual("INTCON", PICRegisters.GetRegister("INTCON").Name);
            Assert.AreEqual("WREG", PICRegisters.GetRegister("WREG").Name);
            Assert.AreEqual("PCL", PICRegisters.GetRegister("PCL").Name);
            Assert.AreEqual("PCLATH", PICRegisters.GetRegister("PCLATH").Name);
            Assert.AreEqual("RP", PICRegisters.GetBitField("RP").Name);
        }

        [Test]
        public void PIC16arch_GetEnhancedRegisterFromNameTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16EnhancedName);
            picMode.CreateRegisters();
            Assert.AreEqual("STATUS", PICRegisters.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR1L", PICRegisters.GetRegister("FSR1L").Name);
            Assert.AreEqual("FSR1H", PICRegisters.GetRegister("FSR1H").Name);
            Assert.AreEqual("FSR0L", PICRegisters.GetRegister("FSR0L").Name);
            Assert.AreEqual("FSR0H", PICRegisters.GetRegister("FSR0H").Name);
            Assert.AreEqual("FSR0", PICRegisters.GetRegister("FSR0").Name);
            Assert.AreEqual("FSR1", PICRegisters.GetRegister("FSR1").Name);
            Assert.AreEqual("INDF0", PICRegisters.GetRegister("INDF0").Name);
            Assert.AreEqual("INDF1", PICRegisters.GetRegister("INDF1").Name);
            Assert.AreEqual("BSR", PICRegisters.GetRegister("BSR").Name);
            Assert.AreEqual("WREG", PICRegisters.GetRegister("WREG").Name);
            Assert.AreEqual("PCL", PICRegisters.GetRegister("PCL").Name);
            Assert.AreEqual("PCLATH", PICRegisters.GetRegister("PCLATH").Name);
        }

        [Test]
        public void PIC16arch_GetFullFeaturedRegisterFromNameTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16FullFeaturedName);
            picMode.CreateRegisters();
            Assert.AreEqual("STATUS", PICRegisters.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR1L", PICRegisters.GetRegister("FSR1L").Name);
            Assert.AreEqual("FSR1H", PICRegisters.GetRegister("FSR1H").Name);
            Assert.AreEqual("FSR0L", PICRegisters.GetRegister("FSR0L").Name);
            Assert.AreEqual("FSR0H", PICRegisters.GetRegister("FSR0H").Name);
            Assert.AreEqual("FSR0", PICRegisters.GetRegister("FSR0").Name);
            Assert.AreEqual("FSR1", PICRegisters.GetRegister("FSR1").Name);
            Assert.AreEqual("INDF0", PICRegisters.GetRegister("INDF0").Name);
            Assert.AreEqual("INDF1", PICRegisters.GetRegister("INDF1").Name);
            Assert.AreEqual("BSR", PICRegisters.GetRegister("BSR").Name);
            Assert.AreEqual("WREG", PICRegisters.GetRegister("WREG").Name);
            Assert.AreEqual("PCL", PICRegisters.GetRegister("PCL").Name);
            Assert.AreEqual("PCLATH", PICRegisters.GetRegister("PCLATH").Name);
        }

        [Test]
        public void PIC16arch_GetBasicCoreRegisterTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16BasicName);
            picMode.CreateRegisters();
            Assert.AreSame(PICRegisters.WREG, PICRegisters.GetRegister("WREG"));
            Assert.AreSame(PICRegisters.STATUS, PICRegisters.GetRegister("STATUS"));
            Assert.AreSame(PICRegisters.PCL, PICRegisters.GetRegister("PCL"));
            Assert.AreSame(PIC16BasicRegisters.INDF, PICRegisters.GetRegister("INDF"));
            Assert.AreSame(PIC16BasicRegisters.RP0, PICRegisters.GetBitField("RP0"));
            Assert.AreSame(PIC16BasicRegisters.RP1, PICRegisters.GetBitField("RP1"));
        }

        [Test]
        public void PIC16arch_GetEnhancedCoreRegisterTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16EnhancedName);
            picMode.CreateRegisters();
            Assert.AreSame(PICRegisters.WREG, PICRegisters.GetRegister("WREG"));
            Assert.AreSame(PICRegisters.STATUS, PICRegisters.GetRegister("STATUS"));
            Assert.AreSame(PICRegisters.PCL, PICRegisters.GetRegister("PCL"));
            Assert.AreSame(PICRegisters.BSR, PICRegisters.GetRegister("BSR"));
            Assert.AreSame(PIC16EnhancedRegisters.INDF0, PICRegisters.GetRegister("INDF0"));
        }

        [Test]
        public void PIC16arch_GetFullFeaturedCoreRegisterTests()
        {
            var picMode = PICProcessorMode.GetMode(PIC16FullFeaturedName);
            picMode.CreateRegisters();
            Assert.AreSame(PICRegisters.WREG, PICRegisters.GetRegister("WREG"));
            Assert.AreSame(PICRegisters.STATUS, PICRegisters.GetRegister("STATUS"));
            Assert.AreSame(PICRegisters.PCL, PICRegisters.GetRegister("PCL"));
            Assert.AreSame(PICRegisters.BSR, PICRegisters.GetRegister("BSR"));
            Assert.AreSame(PIC16FullRegisters.INDF0, PICRegisters.GetRegister("INDF0"));
            Assert.AreSame(PIC16FullRegisters.INDF1, PICRegisters.GetRegister("INDF1"));
        }

    }

}
