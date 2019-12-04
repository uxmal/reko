using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine
{

    public class InstrDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TInstr : MachineInstruction
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
    {
        private readonly InstrClass iclass;
        private readonly TMnemonic mnemonic;
        private readonly Mutator<TDasm>[] mutators;

        public InstrDecoder(InstrClass iclass, TMnemonic mnemonic, params Mutator<TDasm> [] mutators)
        {
            this.iclass = iclass;
            this.mnemonic = mnemonic;
            this.mutators = mutators;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            foreach (var m in mutators)
            {
                if (!m(wInstr, dasm))
                    return dasm.CreateInvalidInstruction();
            }
            return dasm.MakeInstruction(this.iclass, this.mnemonic);
        }
    }
}
