using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.WindowsItp.Decoders
{
    public class TestInstruction : MachineInstruction
    {
        public TestInstruction()
        { }

        public Opcode Opcode { get; set; }

        public override int OpcodeAsInteger => (int) Opcode;
    }
}
