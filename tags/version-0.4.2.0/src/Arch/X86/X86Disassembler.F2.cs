using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.X86
{
    public partial class X86Disassembler
    {
        static Dictionary<byte, OpRec> CreateF2Oprecs()
        {
            return new Dictionary<byte, OpRec>
            {
                { 0x2C, new SingleByteOpRec(Opcode.cvttsd2si, "Gd,Wq")}
            };
        }
    }
}
