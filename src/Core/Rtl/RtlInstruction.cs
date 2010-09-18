#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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
using System.IO;
using System.Text;

namespace Decompiler.Core.Rtl
{
    /// <summary>
    /// RtlInstructions are the low-level register-transfer instructions emitted by the Instruction rewriters
    /// </summary>
    public abstract class RtlInstruction
    {
        public RtlInstruction(Address addr, uint length)
        {
            this.Address = addr;
            this.Length = length;
        }

        public Address Address { get; private set; }
        
        public uint Length { get; private set; }

        public abstract void Accept(RtlInstructionVisitor visitor);

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }

        public virtual void Write(TextWriter writer)
        {
            writer.Write("{0}({1}) ", Address, Length);
        }
    }
}
