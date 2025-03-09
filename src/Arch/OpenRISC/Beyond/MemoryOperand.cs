using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Arch.OpenRISC.Beyond;

public class MemoryOperand : AbstractMachineOperand
{
    public MemoryOperand(RegisterStorage? baseReg, int offset)
        : base(PrimitiveType.Word32)    // disregarded, the size is goverend by the mnemonic/opcode
    {
        this.Base = baseReg;
        this.Offset = offset;
    }

    public RegisterStorage? Base { get; }
    public int Offset { get; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (Offset != 0)
        {
            if (Base is null)
            {
                var addr = Address.Ptr32((uint) Offset);
                renderer.WriteAddress($"0x{Offset:X4}", addr);
            }
            else
            {
                renderer.WriteFormat("{0}({1})", Offset, Base);
            }
        }
        else
        {
            Debug.Assert(Base is not null);
            renderer.WriteFormat("({0})", Base);
        }
    }
}