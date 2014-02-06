using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDisassembler : IDisassembler, IEnumerator<MachineInstruction>
    {
        private IEnumerator<MachineInstruction> instrs;
        private Address addr;
        private MachineInstruction instr;

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

        public MachineInstruction Current { get { return instr; } }

        object System.Collections.IEnumerator.Current { get { return instr; } }

        public void Dispose() { }

        public void Reset() { throw new NotSupportedException(); }

        public bool MoveNext()
        {
            if (!instrs.MoveNext())
                return false;
            instr = instrs.Current;
            instr.Address = addr;
            instr.Length = 4;
            addr += 4;
            return true;
        }
    }
}
