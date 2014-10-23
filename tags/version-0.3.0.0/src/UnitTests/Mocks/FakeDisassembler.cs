using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDisassembler : DisassemblerBase<MachineInstruction>
    {
        private IEnumerator<MachineInstruction> instrs;
        private Address addr;
        private MachineInstruction instr;

        public FakeDisassembler(Address a, IEnumerator<MachineInstruction> e)
        {
            this.addr = a;
            this.instrs = e;
        }

        public override MachineInstruction Current { get { return instr; } }

        public override bool MoveNext()
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
