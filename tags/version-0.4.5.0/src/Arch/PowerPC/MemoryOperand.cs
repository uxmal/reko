#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class MemoryOperand : MachineOperand
    {
        public MemoryOperand(PrimitiveType size, RegisterStorage reg, Constant offset) : base(size)
        {
            this.BaseRegister = reg;
            this.Offset = offset;
        }

        public RegisterStorage BaseRegister { get; private set; }
        public Constant Offset { get; private set; } 

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            writer.Write(string.Format("{0}({1})", Offset, BaseRegister));
        }
    }
}
