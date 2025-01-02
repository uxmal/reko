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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.CompactRisc
{
    public class Cr16Instruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer, options);
            RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(Mnemonic.ToString());
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (operand)
            {
            case ImmediateOperand imm:
                if (imm.Width.Domain == Domain.SignedInt)
                    renderer.WriteFormat("${0}", imm.Value.ToInt32());
                else 
                    renderer.WriteFormat("${0:X}", imm.Value.ToUInt32());
                return;
            case SequenceStorage seq:
                renderer.WriteChar('(');
                RenderRegisterPair(seq, renderer);
                renderer.WriteChar(')');
                return;
            default:
                base.RenderOperand(operand, renderer, options);
                break;
            }
        }

        internal static void RenderRegisterPair(SequenceStorage seq, MachineInstructionRenderer renderer)
        {
            renderer.WriteFormat("{0},{1}", seq.Elements[0], seq.Elements[1]);
        }
    }
}
