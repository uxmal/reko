using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.C166
{
    public class MemoryOperand : AbstractMachineOperand
    {
        public MemoryOperand(PrimitiveType width) : base(width)
        {
        }

        public int Offset { get; set; }
        public RegisterStorage? Base { get; set; }
        public bool Predecrement { get; set; }
        public bool Postincrement { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('[');
            if (Predecrement)
            {
                renderer.WriteChar('-');
            }
            if (Base is not null)
            {
                renderer.WriteString(Base.Name);
            }
            int offset = Offset;
            if (offset < 0)
            {
                renderer.WriteChar('-');
                offset = -offset;
            }
            else if (offset != 0 && Base is not null)
            {
                renderer.WriteChar('+');
            }
            if (offset != 0 || Base is null)
            {
                renderer.WriteFormat("0x{0:X}", offset);
            }
            if (Postincrement)
            {
                renderer.WriteChar('+');
            }
            renderer.WriteChar(']');
        }
    }
}