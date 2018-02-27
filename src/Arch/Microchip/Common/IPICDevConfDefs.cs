#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// Interface for PIC Device Configuration Fuses definitions.
    /// </summary>
    public interface IPICDevConfDefs
    {
        /// <summary>
        /// Gets a Device Configuration Register by its name.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <returns>
        /// A <see cref="DevConfigRegister"/> instance or null.
        /// </returns>
        DevConfigRegister GetDCR(string name);

        /// <summary>
        /// Gets a Device Configuration Register by its memory address.
        /// </summary>
        /// <param name="addr">The memory address.</param>
        /// <returns>
        /// A <see cref="DevConfigRegister"/> instance or null.
        /// </returns>
        DevConfigRegister GetDCR(Address addr);

        /// <summary>
        /// Gets a Device Configuration Field by its name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>
        /// A <see cref="DevConfigField"/> instance or null.
        /// </returns>
        DevConfigField GetDCRField(string name);

        /// <summary>
        /// Renders the Device Configuration state given its value.
        /// </summary>
        /// <param name="dcr">The Device Configuration Register of interest.</param>
        /// <param name="value">The value assigned to this register.</param>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        string Render(DevConfigRegister dcr, int value);

        /// <summary>
        /// Renders the Device Configuration state at given address for given value.
        /// </summary>
        /// <param name="addr">The Device Configuration Register memory address.</param>
        /// <param name="value">The value assigned to this register.</param>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        string Render(Address addr, int value);

    }

}
