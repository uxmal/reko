using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Reko.Arch.OpenRISC.Beyond;

public class BeyondInstruction : MachineInstruction
{
    private static readonly Dictionary<Mnemonic, string> mnemonicStrings;

    public Mnemonic Mnemonic { get; set; }
    public override int MnemonicAsInteger => (int) Mnemonic;

    public override string MnemonicAsString => Mnemonic.ToString();

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        RenderMnemonic(renderer);
        base.RenderOperands(renderer, options);
    }

    private void RenderMnemonic(MachineInstructionRenderer renderer)
    {
        if (!mnemonicStrings.TryGetValue(Mnemonic, out string? mnemonicString))
        {
            mnemonicString = MnemonicAsString;
        }
        renderer.WriteMnemonic(mnemonicString);
    }

    protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (operand is Constant c)
        {
            renderer.BeginOperand();
            if (c.DataType.Domain == Core.Types.Domain.SignedInt)
            {
                renderer.WriteString(c.ToInt32().ToString());
            }
            else
            {
                renderer.WriteFormat("0x{0:X}", c.ToUInt32());
            }
            renderer.EndOperand();
            return;
        }
        base.RenderOperand(operand, renderer, options);
    }

    static BeyondInstruction()
    {
        mnemonicStrings = Enum.GetValues<Mnemonic>()
            .ToDictionary(e => e, e => Enum.GetName(e)!.Replace('_', '.'));
    }
}