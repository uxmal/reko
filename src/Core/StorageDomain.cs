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

namespace Reko.Core;

/// <summary>
/// All data in an analyzed program is stored in some kind of storage. 
/// The different types of storage are represented by this enumeration.
/// </summary>
/// <remarks>
/// Some  storage domains are subdivided further. Specifically, the 
/// <see cref="Register"/> domain is subdivided into each register in
/// the given architecture. The net effect is that sub-ranges of one 
/// register cannot alias sub-ranges of another register. Stack values
/// only have a single domain <see cref="Stack"/>, which makes it possible
/// for stack variables to alias each other.
/// </remarks>
public enum StorageDomain
{
    /// <summary>
    /// Default, invalid value.
    /// </summary>
    None = -1,

    /// <summary>
    /// Refers to a register in the architecture. The actual value of the domain
    /// is the architectural register number, which varies from processor to processor.
    /// </summary>
    /// <remarks>
    /// Few architectures have 4096 general purpose registers (fingers xD)
    /// </remarks>
    Register = 0,

    /// <summary>
    /// Refers to a memory space.
    /// </summary>
    Memory = 4096,          

    /// <summary>
    /// Refers to a register in an FPU stack.
    /// </summary>
    FpuStack = 4098,

    /// <summary>
    /// Global variable within a memory space
    /// </summary>
    Global = 8191,

    /// <summary>
    /// Space for system / control registers (1 million should be enough)
    /// </summary>
    SystemRegister = 8192,

    /// <summary>
    /// Things that look like registers but are in fact other identifiers,
    /// such as enumeration values.
    /// </summary>
    PseudoRegister = (1 << 20),

    /// <summary>
    /// Space for local variables in a procedure.
    /// </summary>
    Stack = (1 << 30),

    /// <summary>
    /// Refers to a temporary variable. StorageDomains for temporary variables
    /// are allocated starting at this value.
    /// </summary>
    Temporary = (1 << 31),
}
