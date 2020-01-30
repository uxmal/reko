using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    public class NativeMachineInstruction : MachineInstruction
    {
        public override int MnemonicAsInteger
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            base.Render(writer, options);
        }
    }
}
