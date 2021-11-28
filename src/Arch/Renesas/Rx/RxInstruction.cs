#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.Renesas.Rx
{
    public class RxInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();
        
        public PrimitiveType? DataType { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(renderer);
            RenderOperands(renderer, options);
        }

        private void RenderMnemonic(MachineInstructionRenderer renderer)
        {
            var sb = new StringBuilder(MnemonicAsString);
            if (DataType is not null)
            {
                sb.Append('.');
                string s;
                switch (DataType.BitSize)
                {
                case 3: s = "s"; break;
                case 32: s = "l"; break;
                case 16:
                    s = DataType.Domain == Domain.SignedInt ? "w" : "uw";
                    break;
                default:
                    s = DataType.Domain == Domain.SignedInt ? "b" : "ub";
                    break;
                }
                sb.Append(s);
            }
            renderer.WriteMnemonic(sb.ToString());
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (operand)
            {
            case Constant imm:
                string sImm;
                if (imm.DataType.Domain == Domain.SignedInt)
                {
                    int i = imm.ToInt32();
                    if (i < 0)
                    {
                        sImm = $"-{-i:X}";
                    }
                    else
                    {
                        sImm = i.ToString("X");
                    }
                }
                else if (imm.DataType.Domain == Domain.Real)
                {
                    sImm = imm.ToFloat().ToString();
                    renderer.WriteFormat("#{0}", sImm);
                    return;
                }
                else
                {
                    sImm = imm.ToUInt32().ToString("X");
                }
                renderer.WriteString(char.IsLetter(sImm[0]) ? "#0" : "#");
                renderer.WriteString(sImm);
                renderer.WriteChar('h');
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }
    }
}