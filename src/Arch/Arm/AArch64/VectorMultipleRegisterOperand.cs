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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch64
{
    public class VectorMultipleRegisterOperand : AbstractMachineOperand
    {
        private readonly RegisterStorage[] registers;
        private readonly int iRegStart;

        public VectorMultipleRegisterOperand(PrimitiveType width, RegisterStorage[] registers, int iRegStart, int repeat) : base(width)
        {
            this.registers = registers;
            this.iRegStart = iRegStart;
            this.Repeat = repeat;
            this.Index = -1;
        }

        public VectorData ElementType { get; set; }

        public int Repeat { get; set; }
        public int Index { get; set; }

        public IEnumerable<RegisterStorage> GetRegisters()
        {
            for (int i = 0; i < Repeat; ++i)
            {
                var reg = this.registers[(i + iRegStart) & 0x1F];
                yield return reg;
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('{');
            var sep = "";
            if (Repeat >= 3 && iRegStart + Repeat <= 0x20)
            {
                VectorRegisterOperand.WriteName(DataType.BitSize, registers[iRegStart], ElementType, Index, renderer);
                renderer.WriteChar('-');
                VectorRegisterOperand.WriteName(DataType.BitSize, registers[iRegStart + Repeat - 1 ], ElementType, Index, renderer);
            } 
            else foreach (var reg in GetRegisters())
            {
                renderer.WriteString(sep);
                sep = ",";
                VectorRegisterOperand.WriteName(DataType.BitSize, reg, ElementType, Index, renderer);
            }
            renderer.WriteChar('}');
            if (Index >= 0)
            {
                renderer.WriteFormat("[{0}]", Index);
            }
        }
    }
}
