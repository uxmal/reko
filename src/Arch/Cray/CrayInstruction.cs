#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Cray
{
    public class CrayInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int)Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (Mnemonic)
            {
            case Mnemonic._and:
                Render3("&", renderer, options);
                return;
            case Mnemonic._fmul:
                Render3("*F", renderer, options);
                return;
            case Mnemonic._mov:
                RenderOperand(Operands[0], renderer, options);
                renderer.Tab();
                RenderOperand(Operands[1], renderer, options);
                if (Operands.Length == 2)
                    return;
                renderer.WriteString(",");
                RenderOperand(Operands[2], renderer, options);
                return;
            }
            renderer.WriteMnemonic(this.Mnemonic.ToString());
            RenderOperands(renderer, options);
        }

        private void Render3(string infix, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.Tab();
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(infix);
            RenderOperand(Operands[2], 
                renderer, options);
        }
    }
}
