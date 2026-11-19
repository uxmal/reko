using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.ControlData.CDC6600;

public class CDC6600Bundle : MachineInstruction
{
    public override int MnemonicAsInteger => 0;

    public override string MnemonicAsString => "";

    public CDC6600Instruction[] Instructions { get; set; } = [];
  
    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        string sep = "";
        foreach (var instr in Instructions)
        {
            renderer.WriteString(sep);
            instr.Render(renderer, options);
            sep = ", ";
        }
    }
}
