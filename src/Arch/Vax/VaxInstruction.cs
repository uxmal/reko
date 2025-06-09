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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public class VaxInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; internal set; }

        public override int MnemonicAsInteger => (int)Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(this.Mnemonic.ToString());
            RenderOperands(renderer, options);
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (op)
            {
            case Constant:
                renderer.WriteChar('#');
                op.Render(renderer, options);
                return;
            case MemoryOperand mop when mop.Base is RegisterStorage rmop && rmop == Registers.pc:
                var addr = this.Address + this.Length;
                if (mop.Offset is not null)
                {
                    addr += mop.Offset.ToInt32();
                }
                if ((options.Flags & MachineInstructionRendererFlags.ResolvePcRelativeAddress) != 0)
                {
                    renderer.WriteAddress(addr.ToString(), addr);
                    renderer.AddAnnotation(op.ToString());
                }
                else
                {
                    op.Render(renderer, options);
                    renderer.AddAnnotation(addr.ToString());
                }
                return;
            default:
                op.Render(renderer, options);
                return;
            }
        }
    }
}
