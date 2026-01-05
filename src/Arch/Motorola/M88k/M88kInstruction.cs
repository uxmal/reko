#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Text;

namespace Reko.Arch.Motorola.M88k;

public class M88kInstruction : MachineInstruction
{
    public Mnemonic Mnemonic { get; set; }
    public override int MnemonicAsInteger => (int) Mnemonic;

    public override string MnemonicAsString => Mnemonic.ToString();

    /// <summary>
    /// Bit mask indicating the sizes of floating point operands.
    /// </summary>
    /// <remarks>
    public uint? FloatSizes { get; set; }

    /// <summary>
    /// True if the instruction is accessing user-space variant (.usr suffix)
    /// </summary>
    public bool UserSpace { get; set; }

    /// <summary>
    /// The address of the next instruction immediately following this one.
    /// </summary>
    public Address NextAddress => this.Address + this.Length;

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        RenderMnemonic(renderer);
        base.RenderOperands(renderer, options);
    }

    private void RenderMnemonic(MachineInstructionRenderer renderer)
    {
        var sb = new StringBuilder(this.Mnemonic.ToString());
        sb.Replace('_', '.');
        if (this.FloatSizes.HasValue)
        {
            sb.Append((this.FloatSizes.Value) switch
            {
                0b000000 => "",
                0b000001 => ".s",
                0b000010 => ".d",
                0b000101 => ".ss",
                0b000110 => ".ds",
                0b001001 => ".sd",
                0b001010 => ".dd",
                0b010000 => ".s",
                0b010101 => ".sss",
                0b010110 => ".dss",
                0b011001 => ".sds",
                0b011010 => ".dds",
                0b100000 => ".d",
                0b100101 => ".ssd",
                0b100110 => ".dsd",
                0b101001 => ".dds",
                0b101010 => ".ddd",
                _ => $"***UNEXPECTED{Convert.ToString(this.FloatSizes.Value, 2)}",
            });
        }
        if (this.UserSpace)
            sb.Append(".usr");
        renderer.WriteMnemonic(sb.ToString());
    }

    protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (operand is Constant c)
        {
            var uimm = c.ToUInt32();
            if (uimm < 10)
            {
                renderer.WriteString(uimm.ToString());
            }
            else
            {
                renderer.WriteString($"0x{uimm:X}");
            }
            return;
        }
        base.RenderOperand(operand, renderer, options);
    }
}