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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Arm.AArch32
{
    public class VectorMultipleRegisterOperand : AbstractMachineOperand
    {
        private readonly RegisterStorage[] registers;
        private readonly int iRegStart;

        public VectorMultipleRegisterOperand(PrimitiveType width, RegisterStorage[] registers, int iRegStart, int repeat, int increment) : base(width)
        {
            this.registers = registers;
            this.iRegStart = iRegStart;
            this.Repeat = repeat;
            this.Increment = increment;
            this.Index = -1;
        }

        public bool AllLanes { get; set; }
        public ArmVectorData ElementType { get; set; }

        public int Repeat { get; set; }
        public int Increment { get; set; }
        public int Index { get; set; }


        public IEnumerable<RegisterStorage> GetRegisters()
        {
            var iReg = iRegStart;
            for (int i = 0; i < Repeat; ++i)
            {
                var reg = this.registers[iReg & 0x1F];
                iReg += Increment;
                yield return reg;
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteChar('{');
            var sep = "";
            if (Repeat >= 3 && iRegStart + Repeat <= 0x20 && Increment == 1)
            {
                WriteName(registers[iRegStart], AllLanes, renderer, options);
                renderer.WriteChar('-');
                WriteName(registers[iRegStart + Repeat - 1], AllLanes, renderer, options);
            }
            else foreach (var reg in GetRegisters())
            {
                renderer.WriteString(sep);
                sep = ",";
                WriteName(reg, AllLanes, renderer, options);
            }
            renderer.WriteChar('}');
        }

        public void WriteName(RegisterStorage VectorRegister, bool allLanes, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(VectorRegister.Name);
            if (allLanes)
                renderer.WriteString("[]");
            if (Index >= 0)
            {
                renderer.WriteFormat("[{0}]", Index);
            }
        }
    }
}
