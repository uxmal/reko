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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    public class HeuristicBlock
    {
        public HeuristicBlock(Address address, string name)
        {
            this.Address = address;
            this.Name = name; // +"-" + (++cntr);
            this.Instructions = new List<MachineInstruction>();
            this.IsValid = true;
        }

        public Address Address { get; private set; }
        public string Name { get; private set; }
        public List<MachineInstruction> Instructions { get; private set; }
        public bool IsValid { get; set; }

        public Address GetEndAddress()
        {
            int iLast = Instructions.Count - 1;
            if (iLast < 0)
                return Address;
            var instr = Instructions[iLast];
            return instr.Address + instr.Length;
        }

        public override string ToString()
        {
            return string.Format("block({0})", Address);
        }
    }
}
