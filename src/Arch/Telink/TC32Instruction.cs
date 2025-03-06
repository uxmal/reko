using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Telink
{
    public class TC32Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int)Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer, options);
            RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(MnemonicAsString);
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (operand is Constant imm)
            {
                renderer.WriteFormat("#{0}", imm.ToUInt64());
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }
    }
}