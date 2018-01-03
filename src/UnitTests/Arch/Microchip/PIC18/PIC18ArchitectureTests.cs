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
        private static PIC pic = PICSamples.GetSample(InstructionSetID.PIC18);
        private PIC18Architecture arch;

        [Test]
        public void PIC18arch_CheckArch()
        {

            arch = new PIC18Architecture(pic);
            Assert.NotNull(arch);
            Assert.AreEqual(arch.PICDescriptor, pic);
            Assert.AreEqual(arch.Name, pic.Name);
            Assert.AreEqual(PrimitiveType.UInt16, arch.WordWidth);
        }

        [Test]
        public void PIC18arch_GetOpcodeNames()
        {
            arch = new PIC18Architecture(pic);
            Assert.AreEqual(
                "ADDFSR,ADDLW,ADDULNK,ADDWF,ADDWFC,ANDLW",
                string.Join(",", arch.GetOpcodeNames().Keys.Take(6)));
        }

        [Test]
        public void PIC18arch_GetOpcodeNumber()
        {
            arch = new PIC18Architecture(pic);
            Assert.AreEqual(
                Opcode.MOVWF,
                (Opcode)arch.GetOpcodeNumber("MOVWF"));
            Assert.AreEqual(
                Opcode.LFSR,
                (Opcode)arch.GetOpcodeNumber("LFSR"));
        }

        [Test]
        public void PIC18arch_FailGetRegisterFromName()
        {
            arch = new PIC18Architecture(pic);
            Assert.Throws<ArgumentException>(() => arch.GetRegister("invalidregistername"));
        }

        [Test]
        public void PIC18arch_GetRegisterFromName()
        {
            arch = new PIC18Architecture(pic);
            Assert.AreEqual("STATUS", arch.GetRegister("STATUS").Name);
            Assert.AreEqual("FSR2L", arch.GetRegister("FSR2L").Name);
            Assert.AreEqual("FSR2H", arch.GetRegister("FSR2H").Name);
            Assert.AreEqual("BSR", arch.GetRegister("BSR").Name);
            Assert.AreEqual("TOSU", arch.GetRegister("TOSU").Name);
            Assert.AreEqual("TOSH", arch.GetRegister("TOSH").Name);
            Assert.AreEqual("TOSL", arch.GetRegister("TOSL").Name);
        }

        [Test]
        public void PIC18arch_GetRegisterFromNumber()
        {
            arch = new PIC18Architecture(pic);
            Assert.AreEqual("STATUS", arch.GetRegister(0x3FD8).Name);
        }

    }

}
