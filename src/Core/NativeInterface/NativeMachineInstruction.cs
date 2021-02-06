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
        public override int MnemonicAsInteger => throw new NotImplementedException();
        public override string MnemonicAsString => throw new NotImplementedException();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            base.DoRender(renderer, options);
        }
    }
}
