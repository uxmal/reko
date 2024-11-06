#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core.Machine;
using Reko.Core.Types;
using System.Text;

namespace Reko.Arch.M16C;

public class M16CInstruction : MachineInstruction
{
    public Mnemonic Mnemonic { get; set; }

    public override int MnemonicAsInteger => (int) Mnemonic;

    public override string MnemonicAsString => Mnemonic.ToString();

    public SizeSuffix SizeSuffix { get;  set; }

    public InstrSuffix InstrSuffix { get; set; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        RenderMnemonic(renderer);
        RenderOperands(renderer, options);
    }

    private void RenderMnemonic(MachineInstructionRenderer renderer)
    {
        var sb = new StringBuilder();
        sb.Append(Mnemonic.ToString());
        sb.Append(this.SizeSuffix switch
        {
            SizeSuffix.B => ".b",
            SizeSuffix.W => ".w",
            SizeSuffix.L => ".l",
            SizeSuffix.A => ".a",
            _ => ""
        });
        sb.Append(this.InstrSuffix switch
        {
            InstrSuffix.Q => ":q",
            InstrSuffix.S => ":s",
            InstrSuffix.Z => ":z",
            _ => ""
        });
        renderer.WriteMnemonic(sb.ToString());
    }

    protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        switch (operand)
        {
        case ImmediateOperand imm:
            renderer.WriteChar('#');
            var s = imm.Value.DataType.Domain == Domain.SignedInt
                ? FormatSignedValue(imm.Value.ToInt64(), null)
                : FormatUnsignedValue(imm.Value.ToUInt64(), null);
            renderer.WriteString(s);
            return;
        }
        base.RenderOperand(operand, renderer, options);
    }

    internal static string FormatSignedValue(long n, string? format)
    {
        var sb = new StringBuilder();
        int iLeadingHexDigit = 0;
        if (n < 0)
        {
            n = -n;
            sb.Append('-');
            iLeadingHexDigit = 1;
        }
        sb.AppendFormat(format ?? "{0:X}", n);
        if (!char.IsDigit(sb[iLeadingHexDigit]))
        {
            sb.Insert(iLeadingHexDigit, '0');
        }
        sb.Append('h');
        return sb.ToString();
    }

    internal static string FormatUnsignedValue(ulong n, string? format)
    {
        var sb = new StringBuilder();
        sb.AppendFormat(format ?? "{0:X}", n);
        if (!char.IsDigit(sb[0]))
        {
            sb.Insert(0, '0');
        }
        sb.Append('h');
        return sb.ToString();
    }
}

public enum SizeSuffix
{
    None,
    A,
    B,
    W,
    L
}

public enum InstrSuffix
{
    None,
    Q,
    S,
    Z,
}
