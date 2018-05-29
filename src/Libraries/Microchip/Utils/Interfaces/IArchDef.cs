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
    /// This interface for arch definition.
    /// </summary>
    public interface IArchDef
    {
        /// <summary>
        /// Gets the description of the PIC architecture.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the program memory traits.
        /// </summary>
        IEnumerable<ITrait> ProgTraits { get; }

        /// <summary>
        /// Gets the data memory traits.
        /// </summary>
        IEnumerable<ITrait> DataTraits { get; }

        /// <summary>
        /// Gets address magic offset in the binary image for EEPROM content.
        /// </summary>
        uint? MagicOffset { get; }

        /// <summary>
        /// Gets the depth of the hardware stack.
        /// </summary>
        int? HWStackDepth { get; }

        /// <summary>
        /// Gets the number of memory banks.
        /// </summary>
        int? BankCount { get; }

    }

}
