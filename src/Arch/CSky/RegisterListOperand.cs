#region License
/* 
 * Copyright (C) 1999-2024 John K�ll�n.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace Reko.Arch.CSky
{
    public class RegisterListOperand : AbstractMachineOperand
    {
        private readonly uint registerList;
        private readonly RegisterStorage[] regs;

        public RegisterListOperand(uint registerList, RegisterStorage[] regs)
            : base(PrimitiveType.Byte)
        {
            this.registerList = registerList;
            this.regs = regs;
        }


        public IEnumerable<RegisterStorage> RegisterList
        {
            get
            {
                int i = 0;
                uint m = 1u << i;
                for (; i < regs.Length; ++i, m <<= 1)
                {
                    if ((registerList & m) != 0)
                        yield return regs[i];
                }
            }
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            int i = 0;
            uint m = 1u << i;
            int iRunStart = -1;
            var sep = "";
            for (; i < Registers.GpRegs.Length; ++i, m <<= 1)
            {
                if ((registerList & m) != 0)
                {
                    if (iRunStart < 0)
                    {
                        iRunStart = i;
                        renderer.WriteString(sep);
                        sep = ",";
                        renderer.WriteString(regs[i].Name);
                    }
                }
                else
                {
                    if (iRunStart >= 0 && iRunStart < i-1)
                    {
                        renderer.WriteChar('-');
                        renderer.WriteString(regs[i-1].Name);
                    }
                    iRunStart = -1;
                }
            }
            if (iRunStart >= 0 && iRunStart < i - 1)
            {
                renderer.WriteChar('-');
                renderer.WriteString(regs[i-1].Name);
            }
        }
    }
}