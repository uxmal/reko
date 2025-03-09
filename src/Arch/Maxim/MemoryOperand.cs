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
using Reko.Core.Code;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Maxim;

public class MemoryOperand : AbstractMachineOperand
{

    private MemoryOperand(PrimitiveType dt, RegisterStorage? baseRegister, int offset, IncrementMode mode)
        : base(dt)
    {
        this.Base = baseRegister;
        this.Offset = offset;
        this.Increment = mode;
    }

    public static MemoryOperand Create(PrimitiveType dt, RegisterStorage? baseRegister, int offset, IncrementMode mode)
    {
        var op = new MemoryOperand(dt, baseRegister, offset, mode);
        return op;
    }

    public RegisterStorage? Base { get; }
    public int Offset { get; }

    public IncrementMode Increment { get; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        throw new NotImplementedException();
    }

}
