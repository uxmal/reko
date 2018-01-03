using Microchip.Crownking;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Arch.Microchip
{
    [TestFixture]
    public class PICSamplesTests
    {
        [Test]
        public void GetSamples_Test()
        {
            foreach (InstructionSetID instrID in Enum.GetValues(typeof(InstructionSetID)))
            {
                if (instrID == InstructionSetID.UNDEFINED) continue;
                PIC pic = PICSamples.GetSample(instrID);
                Assert.NotNull(pic);
                Assert.AreEqual(instrID, pic.GetInstructionSetID);
            }
        }
    }

}
