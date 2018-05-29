#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// This interface provides access to the PIC definitions (architecture, memory regions, instruction set, etc...).
    /// </summary>
    public interface IPIC
    {
        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx)
        /// </summary>
        string Arch { get; }

        /// <summary>
        /// Gets the PIC description.
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// Gets the unique processor identifier. Used by development tools.
        /// </summary>
        int ProcID { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC is belonging to the PIC18 family.
        /// </summary>
        bool IsPIC18 { get; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// </summary>
        bool IsExtended { get; }

        /// <summary>
        /// Gets the instruction set identifier of this PIC as a value from the <see cref="InstructionSetID"/> enumeration.
        /// </summary>
        InstructionSetID GetInstructionSetID { get; }

        /// <summary>
        /// Gets the instruction set family name.
        /// </summary>
        string InstructionSetFamily { get; }

        /// <summary>
        /// Gets the PIC architecture main characteristics.
        /// </summary>
        IArchDef ArchDefinitions { get; }

    }

}
