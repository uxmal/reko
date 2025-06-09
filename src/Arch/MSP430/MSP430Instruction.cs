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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Msp430
{
    public class Msp430Instruction : MachineInstruction
    {
        public Mnemonics Mnemonic;
        public PrimitiveType? dataWidth;
        public int repeatImm;
        public RegisterStorage? repeatReg;

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer);
            RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer)
        {
            if (repeatReg is not null)
            {
                renderer.WriteMnemonic("rpt");
                renderer.WriteString(" ");
                renderer.WriteString(repeatReg.Name);
                renderer.WriteString(" ");
            }
            else if (repeatImm > 1)
            {
                renderer.WriteMnemonic("rpt");
                renderer.WriteString(" #");
                renderer.WriteString(repeatImm.ToString());
                renderer.WriteString(" ");
            }
            var sb = new StringBuilder(Mnemonic.ToString());
            if (dataWidth is not null)
            {
                sb.AppendFormat(".{0}", dataWidth.BitSize == 8
                    ? "b"
                    : dataWidth.BitSize == 16
                        ? "w"
                        : "a");
            }
            renderer.WriteMnemonic(sb.ToString());
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (op is Address && (InstructionClass & InstrClass.Transfer) == 0)
            {
                renderer.WriteString("#");
            }
            if (op is Constant && Mnemonic != Mnemonics.call)
            {
                renderer.WriteString("#");
            }
            op.Render(renderer, options);
        }
    }
}