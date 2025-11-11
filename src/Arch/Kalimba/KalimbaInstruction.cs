using Reko.Core.Machine;

namespace Reko.Arch.Kalimba;

public class KalimbaInstruction : MachineInstruction
{
    public Mnemonic Mnemonic { get; set; }
    public override int MnemonicAsInteger => (int) this.Mnemonic;

    public override string MnemonicAsString => this.Mnemonic.ToString();

    public KalimbaInstruction? MemAccess1 { get; set; }
    public KalimbaInstruction? MemAccess2 { get; set; }
    public CCode Condition { get;  set; }
    public uint? SignSelect { get; internal set; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        DoRender(renderer, options, ";");
    }

    protected void DoRender(
        MachineInstructionRenderer renderer,
        MachineInstructionRendererOptions options,
        string term)
    {
        if (Mnemonic == Mnemonic.Invalid)
        {
            renderer.WriteMnemonic("invalid");
            return;
        }
        if (Condition != CCode.Always)
        {
            renderer.WriteMnemonic("if");
            renderer.WriteFormat(" {0} ", this.Condition.ToString().ToLower());
        }
        switch (Mnemonic)
        {
        case Mnemonic.rts:
        case Mnemonic.rti:
            renderer.WriteMnemonic(MnemonicAsString);
            break;
        case Mnemonic.call:
        case Mnemonic.jump:
            renderer.WriteMnemonic(MnemonicAsString);
            renderer.WriteString(" ");
            RenderOperand(Operands[0], renderer, options);
            break;
        case Mnemonic.Load:
        case Mnemonic.Store:
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" = ");
            RenderOperand(Operands[1], renderer, options);
            break;
        case Mnemonic.fracmul:
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" = ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(" * ");
            RenderOperand(Operands[2], renderer, options);
            renderer.WriteString("  (frac)");
            break;
        case Mnemonic.smul:
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" = ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(" * ");
            RenderOperand(Operands[2], renderer, options);
            break;
        case Mnemonic.smulv:
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" = ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(" * ");
            RenderOperand(Operands[2], renderer, options);
            renderer.WriteString("  (sat)");
            break;
        case Mnemonic.maca:
            RenderMacX("+", renderer, options, term);
            break;
        case Mnemonic.macs:
            RenderMacX("-", renderer, options, term);
            break;
        default:
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" = ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteFormat(" {0} ", OperatorAsString());
            RenderOperand(Operands[2], renderer, options);

            if (MemAccess1 is not null)
            {
                renderer.WriteString(" ;; ");
                MemAccess1.DoRender(renderer, options, "");
                if (MemAccess2 is not null)
                {
                    renderer.WriteString(" ;; ");
                    MemAccess2.DoRender(renderer, options, "");
                }
            }
            break;
        }
        renderer.WriteString(term);
    }

    private void RenderMacX(string op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options, string term)
    {
        RenderOperand(Operands[0], renderer, options);
        renderer.WriteString(" = ");
        RenderOperand(Operands[0], renderer, options);
        renderer.WriteFormat(" {0} ", op);
        RenderOperand(Operands[1], renderer, options);
        renderer.WriteFormat(" * ", op);
        RenderOperand(Operands[2], renderer, options);
        renderer.WriteString(this.SignSelect!.Value switch
        {
        0b00 => " (uu)",
        0b01 => " (us)",
        0b10 => " (su)",
        0b11 => " (ss)",
        _ => ""
        });
    }

    private string OperatorAsString()
    {
        return Mnemonic switch
        {
            Mnemonic.add => "+",
            Mnemonic.and => "and",
            Mnemonic.ash => "ashift",
            Mnemonic.div => "/",
            Mnemonic.lsh => "lshift",
            Mnemonic.or => "or",
            Mnemonic.sub => "-",
            Mnemonic.xor => "xor",
            _ => throw new NotImplementedException($"{Mnemonic}")
        };
    }
}