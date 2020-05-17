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

        public Mnemonic Mnemonic { get; set; }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();
    }

    public enum Mnemonic
    {
        Invalid,

        add,
        sub,
        mul,
        div,

        and,
        or,
        xor,
        not,

    }
}
