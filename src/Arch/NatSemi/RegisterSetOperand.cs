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

namespace Reko.Arch.NatSemi;

public class RegisterSetOperand : AbstractMachineOperand
{
    public RegisterSetOperand(byte registerRet)
        : base(PrimitiveType.Word32)
    {
        this.registerSet = registerRet;
    }

    private int registerSet;

    public IEnumerable<RegisterStorage> GetRegisters()
    {
        int m = 1;
        for (int i = 0; i < 8; ++i, m <<= 1)
        {
            if ((registerSet & m) != 0)
            {
                yield return Registers.GpRegisters[i];
            }
        }
    }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        renderer.WriteChar('[');
        renderer.WriteString(string.Join(',', GetRegisters().Select(r => r.Name)));
        renderer.WriteChar(']');
    }
}
