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
using Reko.Core.Machine;
using System;
using System.Text;

namespace Reko.Arch.Avr.Avr32
{
    public class Avr32Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();
        public Avr32Condition Condition { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer, options);
            base.RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var sb = new StringBuilder(Mnemonic.ToString().Replace('_', '.'));
            if (Condition != Avr32Condition.al)
            {
                sb.Append(Condition.ToString());
            }
            renderer.WriteMnemonic(sb.ToString());
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (operand is SequenceStorage seq)
            {
                string sep = "";
                foreach (RegisterStorage reg in seq.Elements)
                {
                    renderer.WriteString(sep);
                    sep = ":";
                    renderer.WriteString(reg.Name);
                }
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }
    }
}
