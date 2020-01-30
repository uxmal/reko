using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.WindowsItp.Decoders
{
    public abstract class DecoderBuilder
    {
        public Decoder Mask(int pos, int length, params Decoder[] decoders)
        {
            return new MaskDecoder(new[] { new Bitfield(pos, length) }, decoders);
        }

        public abstract Decoder Instr(Mnemonic mnemonic, string format);
    }

    public class FormatDecoderBuilder : DecoderBuilder
    { 

        public override Decoder Instr(Mnemonic mnemonic, string format)
        {
            return new FormatDecoder(mnemonic, format);
        }
    }

    public class ThreadedDecoderBuilder : DecoderBuilder
    {
        public override Decoder Instr(Mnemonic mnemonic, string format)
        {
            var mutators = new List<Mutator<Disassembler>>();
            for (int i = 0; i < format.Length; ++i)
            {
                switch (format[i])
                {
                case 'r':
                    {
                        ++i;
                        int n = Disassembler.ReadDecimal(format, ref i);
                        mutators.Add(Disassembler.Reg(n));
                    }
                    break;
                default:
                    throw new NotImplementedException($"{format[i]}");
                }
            }
            return new ThreadedDecoder(mnemonic, mutators.ToArray());
        }
    }
}
