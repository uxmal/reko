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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class BranchTableOperand : AbstractMachineOperand
    {
        public BranchTableOperand(uint[] targets, uint defaultTarget)
            : base(VoidType.Instance)
        {
            this.Targets = targets;
            this.DefaultTarget = defaultTarget;
        }

        public uint[] Targets { get; }
        public uint DefaultTarget { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var sep = "";
            foreach (var target in Targets)
            {
                renderer.WriteString(sep);
                sep = " ";
                renderer.WriteFormat("0x{0:X}", target);
            }
            renderer.WriteFormat("0x{0:X}", DefaultTarget);
        }
    }
}
