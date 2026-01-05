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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
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

            case Mnemonic._and: Render3("&", renderer, options); return;
            case Mnemonic._andnot: Render3("#", "&", renderer, options); return;
            case Mnemonic._clz: Render2("Z", renderer, options); return;
            case Mnemonic._dlsl: Render4(",", "<", renderer, options); return;
            case Mnemonic._iadd: Render3("+", renderer, options); return;
            case Mnemonic._imul: Render3("*", renderer, options); return;
            case Mnemonic._isub: Render3("-", renderer, options); return;
            case Mnemonic._fmul: Render3("*F", renderer, options); return;
            case Mnemonic._lmask: Render2("<", renderer, options); return;
            case Mnemonic._lsl: Render2("<", renderer, options); return;
            case Mnemonic._lsr: Render2(">", renderer, options); return;
            case Mnemonic._load: RenderLoad(renderer, options); return;
            case Mnemonic._load_inc: RenderLoadInc(renderer, options); return;
            case Mnemonic._mov:
            case Mnemonic._movz:
                RenderOperand(Operands[0], renderer, options);
                renderer.WriteString(" ");
                RenderOperand(Operands[1], renderer, options);
                if (Operands.Length == 2)
                    return;
                renderer.WriteString(",");
                RenderOperand(Operands[2], renderer, options);
                return;
            case Mnemonic._movlo: Render3(":", renderer, options); return;
            case Mnemonic._movhi: Render3(":", renderer, options); return;
            case Mnemonic._neg: Render2("-", renderer, options); return;
            case Mnemonic._vor: Render3("!", renderer, options); return;
            case Mnemonic._vmov: Render2("", renderer, options); return;
            case Mnemonic._popcnt: Render2("P", renderer, options); return;
            case Mnemonic._rmask: Render2(">", renderer, options); return;
            case Mnemonic._store: RenderStore(renderer, options); return;
            case Mnemonic._store_inc: RenderStoreInc(renderer, options); return;
            case Mnemonic._viadd: Render3("+", renderer, options); return;
            case Mnemonic._xor: Render3("\\", renderer, options); return;
            }
            renderer.WriteMnemonic(this.Mnemonic.ToString());
            var sep = ' ';
            foreach (var operand in Operands)
            {
                renderer.WriteChar(sep);
                sep = ',';
                RenderOperand(operand, renderer, options);
            }
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (operand)
            {
            case Address addr:
                var sAddr = Convert.ToString(addr.ToUInt32(), 8)
                    .PadLeft(12, '0');
                renderer.WriteAddress(sAddr, addr);
                break;
            case Constant imm:
                var sValue = Convert.ToString((int) imm.ToUInt64(), 8)
                    .PadLeft(6, '0');
                renderer.WriteString(sValue);
                break;
            default:
                base.RenderOperand(operand, renderer, options);
                break;
            }
        }
   
        private void Render2(string infix, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" ");
            renderer.WriteString(infix);
            RenderOperand(Operands[1], renderer, options);
        }

        private void Render3(string infix, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(infix);
            RenderOperand(Operands[2], renderer, options);
        }

        private void Render3(string prefix, string infix, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" ");
            renderer.WriteString(prefix);
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(infix);
            RenderOperand(Operands[2], renderer, options);
        }

        private void Render4(string infix1, string infix2, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(infix1);
            RenderOperand(Operands[2], renderer, options);
            renderer.WriteString(infix2);
            RenderOperand(Operands[3], renderer, options);
        }

        private void RenderLoad(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" ");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(",");
            if (Operands.Length == 3)
            {
                RenderOperand(Operands[2], renderer, options);
            }
        }

        private void RenderLoadInc(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteString(" ,");
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteString(",");
            RenderOperand(Operands[2], renderer, options);
        }

        private void RenderStore(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderOperand(Operands[0], renderer, options);
            if (Operands.Length == 2)
            {
                renderer.WriteString(", ");
                RenderOperand(Operands[1], renderer, options);
            }
            else
            {
                renderer.WriteString(",");
                RenderOperand(Operands[1], renderer, options);
                renderer.WriteString(" ");
                RenderOperand(Operands[2], renderer, options);
            }
        }

        private void RenderStoreInc(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar(',');
            RenderOperand(Operands[0], renderer, options);
            renderer.WriteChar(',');
            RenderOperand(Operands[1], renderer, options);
            renderer.WriteChar(' ');
            RenderOperand(Operands[2], renderer, options);
        }
    }
}
