#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp11
{
    public class MemoryOperand : MachineOperand
    {
        public MemoryOperand(AddressMode mode, PrimitiveType type, RegisterStorage reg) : base(type)
        {
            Mode = mode;
            Register = reg;
        }

        public MemoryOperand(ushort absolute, PrimitiveType type) : base (type)
        {
            Mode = AddressMode.Absolute;
            EffectiveAddress = absolute;
        }

        public AddressMode Mode { get; set; }
        public RegisterStorage Register { get;set;}
        public bool PreDec { get; set; }
        public bool PostInc { get; set; }
        public ushort EffectiveAddress { get; set; }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            string fmt;
            switch (Mode)
            {
            case AddressMode.RegDef: fmt = "@{0}"; break;
            case AddressMode.AutoIncr: fmt = "({0})+"; break;
            case AddressMode.AutoIncrDef: fmt = "@({0})+"; break;
            case AddressMode.AutoDecr: fmt = "-({0})"; break;
            case AddressMode.AutoDecrDef: fmt = "@-({0})"; break;
            case AddressMode.Indexed: fmt = "{1:X4}({0})"; break;
            case AddressMode.IndexedDef: fmt = "@{1:X4}({0})"; break;
            //case AddressMode.Immediate : fmt = "#{1:X4}"; break;
            case AddressMode.Absolute: fmt = "@#{1:X4}"; break;
            default: throw new NotImplementedException(string.Format("Unknown mode {0}.", Mode));
            }
            writer.Write(string.Format(fmt, Register, EffectiveAddress));
        }
    }

    public enum AddressMode
    {
        Register,
        RegDef,
        AutoIncr,
        AutoDecr,
        Absolute,
        Indexed,
        Immediate,
        AutoIncrDef,
        AutoDecrDef,
        IndexedDef,
    }
}
