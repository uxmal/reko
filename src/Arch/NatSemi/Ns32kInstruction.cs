#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Types;

namespace Reko.Arch.NatSemi;

public class Ns32kInstruction : MachineInstruction
{
    public Mnemonic Mnemonic { get; set; }

    public override int MnemonicAsInteger => (int) Mnemonic;

    public override string MnemonicAsString => Mnemonic.ToString();

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        RenderMnemonic(renderer);
        RenderOperands(renderer, options);
    }

    private void RenderMnemonic(MachineInstructionRenderer renderer)
    {
        renderer.WriteMnemonic(MnemonicAsString);
    }

    protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (operand is Constant c)
        {
            if (c.DataType.Domain == Domain.SignedInt)
            {
                renderer.WriteFormat("{0}", c.ToInt32());
                return;
            }
            uint u = c.ToUInt32();
            if (u < 10)
            {
                renderer.WriteFormat("{0}", u);
                return;
            }
            renderer.WriteFormat("H'{0:X}", c.ToUInt32());
            return;
        }
        base.RenderOperand(operand, renderer, options);
    }
}