#region License
/* 
* Copyright (C) 2021-2022 Sven Almgren.
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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core
{
    public class UserProcedure
    {
        public const int NoOrdinal = -1;
        
        public UserProcedure(Address address, string name)
        {
            Address = address;
            Name = name;
            Assume = new List<RegisterValue_v2>();
            Characteristics = new ProcedureCharacteristics();
        }

        public Address Address { get; set; }

        public string Name { get; set; }

        public bool Decompile { get; set; } = true;
        
        public List<RegisterValue_v2> Assume { get; set; }
        
        public string? CSignature { get; set; }
        
        public string? OutputFile { get; set; }

        public int Ordinal { get; set; } = NoOrdinal;
        
        public SerializedSignature? Signature { get; set; }

        public ProcedureCharacteristics Characteristics { get; set; }
    }
}
