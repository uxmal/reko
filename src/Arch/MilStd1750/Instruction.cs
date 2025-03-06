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

namespace Reko.Arch.MilStd1750
{
    public class Instruction : MachineInstruction
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
            var sMnemonic = Mnemonic.ToString();
            if (sMnemonic.StartsWith("xio_"))
                sMnemonic = sMnemonic.Substring(4);
            renderer.WriteMnemonic(sMnemonic);
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (operand is Constant imm)
            {
                renderer.WriteChar('#');
                WriteHex(imm.ToUInt16(), renderer);
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }

        public static void WriteHex(ushort n, MachineInstructionRenderer renderer)
        {
            if (n > 9)
            {
                renderer.WriteFormat("0x{0:X}", n);
            }
            else
            {
                renderer.WriteFormat("{0:X}", n);
            }
        }
    }
}