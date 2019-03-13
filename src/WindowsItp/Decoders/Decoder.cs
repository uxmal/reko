using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.WindowsItp.Decoders
{
    public abstract class Decoder
    {
        public abstract TestInstruction Decode(uint wInstr, Disassembler dasm);
    }

    public class MaskDecoder : Decoder
    {
        private readonly Bitfield[] bitfields;
        private readonly Decoder[] decoders;

        public MaskDecoder(Bitfield[] bitfields, Decoder [] decoders)
        {
            this.bitfields = bitfields;
            this.decoders = decoders;
        }

        public override TestInstruction Decode(uint wInstr, Disassembler dasm)
        {
            var code = Bitfield.ReadFields(bitfields, wInstr);
            return decoders[code].Decode(wInstr, dasm);
        }
    }

    public class FormatDecoder : Decoder
    {
        private readonly Opcode opcode;
        private readonly string format;

        public FormatDecoder(Opcode opcode, string format)
        {
            this.opcode = opcode;
            this.format = format;
        }

        public override TestInstruction Decode(uint wInstr, Disassembler dasm)
        {
            return dasm.Decode(wInstr, opcode, format);
        }
    }

    public class ThreadedDecoder : Decoder
    {
        private readonly Opcode opcode;
        private readonly Mutator<Disassembler>[] mutators;

        public ThreadedDecoder(Opcode opcode, params Mutator<Disassembler> [] mutators)
        {
            this.opcode = opcode;
            this.mutators = mutators;
        }
         
        public override TestInstruction Decode(uint wInstr, Disassembler dasm)
        {
            foreach (var mutator in mutators)
            {
                if (!mutator(wInstr, dasm))
                    return dasm.Invalid();
            }
            return dasm.MakeInstruction();
        }
    }
}
