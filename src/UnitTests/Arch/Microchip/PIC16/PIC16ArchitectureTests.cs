using Reko.Libraries.Microchip;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC16;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC16
{
    [TestFixture]
    public class PIC16ArchitectureTests
    {
        private static PIC pic;
        private PIC16Architecture arch;

        [Test]
        public void PIC16arch_WrongPIC()
        {
            arch = new PIC16Architecture() { CPUModel = "PICUnknown" };
            Assert.Throws<InvalidOperationException>(() => { var pic = arch.PICDescriptor; });
        }

        [Test]
        public void PIC16arch_CheckArchFromDBTests()
        {
            arch = new PIC16Architecture();
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, "pic16");
            arch.CPUModel = "PIC16F84A";
            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            pic = arch.PICDescriptor;
            Assert.AreEqual(arch.CPUModel, pic.Name);

            arch = new PIC16Architecture("pic16");
            Assert.NotNull(arch);
            Assert.AreEqual(arch.Name, "pic16");
            arch.CPUModel = "PIC16F1827";
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.NotNull(arch.PICDescriptor);
            pic = arch.PICDescriptor;
            Assert.AreEqual(arch.CPUModel, pic.Name);
        }

        [Test]
        public void PIC16arch_CheckArchTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC16);
            arch = new PIC16Architecture(pic);
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.CPUModel, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.CPUModel, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            pic = PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED);
            arch = new PIC16Architecture(pic);
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.CPUModel, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.CPUModel, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            pic = PICSamples.GetSample(InstructionSetID.PIC16_FULLFEATURED);
            arch = new PIC16Architecture(pic);
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.CPUModel, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.CPUModel, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryDescriptor);
        }

        [Test]
        public void PIC16arch_GetOpcodeNamesTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC16);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(
                "__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(
                "__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_FULLFEATURED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(
                "__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "__IDLOCS,ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
        }

        [Test]
        public void PIC16arch_GetOpcodeNumberTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC16);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_FULLFEATURED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.BRW, (Opcode)arch.GetOpcodeNumber("BRW"));

        }

        [Test]
        public void PIC16arch_FailGetRegisterFromNameTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC16);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_FULLFEATURED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

        }

        [Test]
        public void PIC16arch_GetRegisterFromNameTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC16);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
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

            pic = PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
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

            pic = PICSamples.GetSample(InstructionSetID.PIC16_FULLFEATURED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
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
        public void PIC16arch_GetCoreRegisterTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC16);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC16Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC16Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC16Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC16Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC16Registers.INDF0, arch.GetRegister("INDF0"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC16Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC16Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC16Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC16Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC16Registers.INDF0, arch.GetRegister("INDF0"));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_ENHANCED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC16Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC16Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC16Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC16Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC16Registers.INDF0, arch.GetRegister("INDF0"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC16Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC16Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC16Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC16Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC16Registers.INDF0, arch.GetRegister("INDF0"));

            pic = PICSamples.GetSample(InstructionSetID.PIC16_FULLFEATURED);
            arch = new PIC16Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC16Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC16Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC16Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC16Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC16Registers.INDF0, arch.GetRegister("INDF0"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC16Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC16Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC16Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC16Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC16Registers.INDF0, arch.GetRegister("INDF0"));

        }

    }

}
