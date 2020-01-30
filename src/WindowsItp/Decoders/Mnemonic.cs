using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.WindowsItp.Decoders
{
    public enum Mnemonic
    {
        Invalid,

        add,
        sub,
        mul,
        div,

        and,
        or,
        not,
        xor,
    }
}
