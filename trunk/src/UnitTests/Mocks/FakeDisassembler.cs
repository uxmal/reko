using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDisassembler : IDisassembler
    {
        private IEnumerator<MachineInstruction> instrs;
        private Address addr;

        public FakeDisassembler(Address a, IEnumerator<MachineInstruction> e)
        {
            this.addr = a;
            this.instrs = e;
        }

        public Address Address
        {
            get { return addr; } 
        }

        public MachineInstruction DisassembleInstruction()
        {
            if (!instrs.MoveNext())
                return null;
            
            addr = addr+ 1;
            return instrs.Current;
        }
    }
}
