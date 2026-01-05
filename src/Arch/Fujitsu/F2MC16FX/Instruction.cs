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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    public class Instruction : MachineInstruction
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
            switch (operand)
            {
            case Constant imm:
                renderer.WriteChar('#');
                RenderUnsignedConstant(imm, renderer);
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }

        internal static void RenderSignedConstant(
            Constant value, 
            MachineInstructionRenderer renderer)
        {
            var v = value.ToInt32();
            var sValue = Math.Abs(v).ToString("X");
            renderer.WriteChar(v >= 0 ? '+' : '-');
            if (!char.IsDigit(sValue[0]))
                renderer.WriteChar('0');
            renderer.WriteString(sValue);
            renderer.WriteChar('h');
        }

        internal static void RenderUnsignedConstant(
            Constant value,
            MachineInstructionRenderer renderer)
        {
            var v = value.ToUInt32();
            var sValue = v.ToString("X");
            if (!char.IsDigit(sValue[0]))
                renderer.WriteChar('0');
            renderer.WriteString(sValue);
            renderer.WriteChar('h');
        }
    }
}
