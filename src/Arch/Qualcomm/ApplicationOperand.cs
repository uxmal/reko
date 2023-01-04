#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Qualcomm
{
    public class ApplicationOperand : AbstractMachineOperand
    {
        public ApplicationOperand(DataType width, Mnemonic mnemonic, params MachineOperand[] ops) : base(width)
        {
            this.Mnemonic = mnemonic;
            this.Operands = ops;
        }

        public Mnemonic Mnemonic { get; }
        public MachineOperand[] Operands { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (Mnemonic)
            {
            case Mnemonic.EQ:
                Operands[0].Render(renderer, options);
                renderer.WriteString("=");
                Operands[1].Render(renderer, options);
                break;
            case Mnemonic.LE:
                Operands[0].Render(renderer, options);
                renderer.WriteString("<=");
                Operands[1].Render(renderer, options);
                break;
            case Mnemonic.GE:
                Operands[0].Render(renderer, options);
                renderer.WriteString(">=");
                Operands[1].Render(renderer, options);
                break;
            case Mnemonic.NE:
                Operands[0].Render(renderer, options);
                renderer.WriteString("!=");
                Operands[1].Render(renderer, options);
                break;

            default:
                renderer.WriteMnemonic(Mnemonic.ToString().Replace("__", "."));
                var sep = "(";
                foreach (var op in Operands)
                {
                    renderer.WriteString(sep);
                    op.Render(renderer, options);
                    sep = ",";
                }
                renderer.WriteString(")");
                break;
            }
        }
    }
}
