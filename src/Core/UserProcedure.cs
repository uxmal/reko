#region License
/* 
* Copyright (C) 2021-2026 Sven Almgren.
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
using System.Collections.Generic;

namespace Reko.Core
{
    /// <summary>
    /// User-provided information about a procedure.
    /// </summary>
    public class UserProcedure
    {
        /// <summary>
        /// Indicates the lack of an ordinal.
        /// </summary>
        public const int NoOrdinal = -1;

        /// <summary>
        /// Constructs a new user procedure.
        /// </summary>
        /// <param name="address">Address of the procedure.</param>
        /// <param name="name">User-provided name.</param>
        public UserProcedure(Address address, string name)
        {
            Address = address;
            Name = name;
            Assume = new List<RegisterValue_v2>();
            Characteristics = new ProcedureCharacteristics();
        }

        /// <summary>
        /// The address of the procedure.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// The name of the procedure.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if user wishes to decompile the procedure.
        /// </summary>
        public bool Decompile { get; set; } = true;

        /// <summary>
        /// The registers in Assume are assumed to have specific values upon entry 
        /// to the procedure.
        /// </summary>
        public List<RegisterValue_v2> Assume { get; set; }

        /// <summary>
        /// The C signature of the procedure.
        /// </summary>
        public string? CSignature { get; set; }

        /// <summary>
        /// Optional specification of which file the decompiled code should be
        /// written to.
        /// </summary>
        public string? OutputFile { get; set; }

        /// <summary>
        /// Optional ordinal of the procedure. This is used when the procedure is
        /// imported from an external module.
        /// </summary>
        public int Ordinal { get; set; } = NoOrdinal;

        /// <summary>
        /// Optional reko signature of the procedure.
        /// </summary>
        public SerializedSignature? Signature { get; set; }

        /// <summary>
        /// Optional characteristics of the procedure.
        /// </summary>
        public ProcedureCharacteristics Characteristics { get; set; }
    }
}
