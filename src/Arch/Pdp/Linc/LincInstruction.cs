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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Pdp.Linc;

public class LincInstruction : MachineInstruction
{
    public Mnemonic Mnemonic { get; set; }
    public override int MnemonicAsInteger => (int) this.Mnemonic;
    public override string MnemonicAsString => this.Mnemonic.ToString();

    public bool Indirect { get; set; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteMnemonic(MnemonicAsString.ToUpperInvariant());
        if (this.Indirect)
            renderer.WriteString(" i");
        if (Operands.Length > 0)
        {
            if (!this.Indirect)
                renderer.WriteString("  ");
            foreach (var op in Operands)
            {
                renderer.WriteString(" ");
                RenderOperand(op, renderer, options);
            }
        }
    }

    protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (operand is Constant c)
        {
            var o = Convert.ToString(c.ToUInt16(), 8);
            if (c.DataType.BitSize > 8)
            {
                o = o.PadLeft(4, '0');
            }
            renderer.WriteString(o);
            return;
        }
        base.RenderOperand(operand, renderer, options);
    }
}
