using Reko.Core.Machine;

namespace Reko.Arch.Telink
{
    public class TC32Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int)Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}