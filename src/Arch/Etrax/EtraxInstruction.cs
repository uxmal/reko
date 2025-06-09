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
using System;
using System.Text;

namespace Reko.Arch.Etrax
{
    public class EtraxInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();
        public PrimitiveType? DataWidth { get; set; }

        /// <summary>
        /// Indicate what kind of swap is being requested.
        /// </summary>
        /// <remarks>
        /// This is only valid for the 'swap' instruction.</remarks>
        public uint SwapBits { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer);
            RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer)
        {
            var sb = new StringBuilder();
            if (Mnemonic == Mnemonic.swap)
            {
                if (SwapBits == 0x8)
                {
                    sb.Append("not");
                }
                else
                {
                    sb.Append(MnemonicAsString);
                    if ((SwapBits & 0x8) != 0) sb.Append('n');
                    if ((SwapBits & 0x4) != 0) sb.Append('w');
                    if ((SwapBits & 0x2) != 0) sb.Append('b');
                    if ((SwapBits & 0x1) != 0) sb.Append('r');
                }
            }
            else
            {
                sb.Append(MnemonicAsString);
            }
            if (this.DataWidth is not null)
            {
                sb.Append('.');
                sb.Append(SizeFormat(this.DataWidth));
            }
            renderer.WriteMnemonic(sb.ToString());
        }

        public static string SizeFormat(PrimitiveType scale)
        {
            switch (scale.BitSize)
            {
            case 8: return "b";
            case 16: return "w";
            case 32: return "d";
            }
            throw new NotImplementedException();
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (operand is Constant imm)
            {
                renderer.WriteString("0x");
            }
            base.RenderOperand(operand, renderer, options);
        }
    }
}