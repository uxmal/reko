using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine
{

    public class InstrDecoder<TDasm, TMnemonic, TInstr> : Decoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TInstr : MachineInstruction
    {
        private readonly InstrClass iclass;
        private readonly TMnemonic mnemonic;
        private readonly Mutator<TDasm>[] mutators;

        public InstrDecoder(InstrClass iclass, TMnemonic mnemonic, params Mutator<TDasm>[] mutators)
        {
            this.iclass = iclass;
            this.mnemonic = mnemonic;
            this.mutators = mutators;
        }

        public override TInstr Decode(uint wInstr, TDasm dasm)
        {
            DumpMaskedInstruction(wInstr, 0, this.mnemonic);
            foreach (var m in mutators)
            {
                if (!m(wInstr, dasm))
                    return dasm.CreateInvalidInstruction();
            }
            return dasm.MakeInstruction(this.iclass, this.mnemonic);
        }
    }

    public class WideInstrDecoder<TDasm, TMnemonic, TInstr> : WideDecoder<TDasm, TMnemonic, TInstr>
        where TDasm : DisassemblerBase<TInstr, TMnemonic>
        where TInstr : MachineInstruction
    {
        private readonly InstrClass iclass;
        private readonly TMnemonic mnemonic;
        private readonly WideMutator<TDasm>[] mutators;

        public WideInstrDecoder(InstrClass iclass, TMnemonic mnemonic, params WideMutator<TDasm>[] mutators)
        {
            this.iclass = iclass;
            this.mnemonic = mnemonic;
            this.mutators = mutators;
        }

        public override TInstr Decode(ulong ulInstr, TDasm dasm)
        {
            foreach (var m in mutators)
            {
                if (!m(ulInstr, dasm))
                    return dasm.CreateInvalidInstruction();
            }
            return dasm.MakeInstruction(this.iclass, this.mnemonic);
        }
    }
}
