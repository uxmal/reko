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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.Gameboy
{
    public class GameboyInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(MnemonicAsString);
            base.RenderOperands(renderer, options);
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (operand is ImmediateOperand imm)
            {
                var number = imm.Value.ToUInt32();
                if (imm.Width.Domain == Domain.SignedInt)
                {
                    int sNumber = (short) number;
                    if (sNumber < 0)
                    {
                        renderer.WriteChar('-');
                        sNumber = -sNumber;
                    }
                    RenderIntelHexNumber(sNumber, renderer);
                }
                else
                {
                    RenderIntelHexNumber((int)number, renderer);
                }
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }

        public static void RenderIntelHexNumber(int number, MachineInstructionRenderer renderer)
        {
            var sOffset = number.ToString("X");
            if ("ABCDEF".Contains(sOffset[0]))
                renderer.WriteChar('0');
            renderer.WriteString(sOffset);
            renderer.WriteChar('h');
        }
    }
}
