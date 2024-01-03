#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    public class BitPositionOperand : AbstractMachineOperand
    {
        private MachineOperand op;

        public BitPositionOperand(uint v, MachineOperand op) 
            : base(PrimitiveType.Byte)
        {
            this.BitPosition = v;
            this.op = op;
        }

        public uint BitPosition { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            op.Render(renderer, options);
            renderer.WriteFormat(":{0}", BitPosition);
        }
    }
}