using Microchip.Crownking;
using NUnit.Framework;
using Reko.Arch.Microchip.PIC18;
using Reko.Core.Types;
using System;
using System.Linq;

namespace Reko.UnitTests.Arch.Microchip.PIC18
{
    [TestFixture]
    public class PIC18ArchitectureTests
    {
        private static PIC pic;
        private PIC18Architecture arch;

        [Test]
        public void PIC18arch_CheckArchTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic);
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryMapper);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryMapper);

            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic);
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryMapper);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, arch.ExecMode);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryMapper);

            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic);
            Assert.NotNull(arch);

            arch.ExecMode = PICExecMode.Traditional;
            Assert.AreEqual(PICExecMode.Traditional, arch.ExecMode);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryMapper);

            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(PICExecMode.Extended, arch.ExecMode);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.Byte, arch.WordWidth);
            Assert.NotNull(arch.MemoryMapper);
        }

        [Test]
        public void PIC18arch_GetOpcodeNamesTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
        }

        [Test]
        public void PIC18arch_GetOpcodeNumberTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreEqual(Opcode.MOVWF, (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(Opcode.LFSR, (Opcode)arch.GetOpcodeNumber("LFSR"));

        }

        [Test]
        public void PIC18arch_FailGetRegisterFromNameTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));

        }

        [Test]
        public void PIC18arch_GetRegisterFromNameTests()
        {
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
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

            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
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

            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
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
            pic = PICSamples.GetSample(InstructionSetID.PIC18);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_EXTENDED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));

            pic = PICSamples.GetSample(InstructionSetID.PIC18_ENHANCED);
            arch = new PIC18Architecture(pic) { ExecMode = PICExecMode.Traditional };
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));
            arch.ExecMode = PICExecMode.Extended;
            Assert.AreSame(PIC18Registers.WREG, arch.GetRegister("WREG"));
            Assert.AreSame(PIC18Registers.STATUS, arch.GetRegister("STATUS"));
            Assert.AreSame(PIC18Registers.BSR, arch.GetRegister("BSR"));
            Assert.AreSame(PIC18Registers.PCL, arch.GetRegister("PCL"));
            Assert.AreSame(PIC18Registers.INDF0, arch.GetRegister("INDF0"));

        }

    }

}
