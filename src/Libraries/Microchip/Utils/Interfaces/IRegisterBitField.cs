#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// This interface provides access to a register bit field definition.
    /// </summary>
    public interface IRegisterBitField
    {
        /// <summary> Gets the name of the register's field. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of the field. </summary>
        string Description { get; }

        /// <summary> Gets the bit position of the field. </summary>
        byte BitPos { get; }

        /// <summary> Gets the bit width of the field. </summary>
        byte BitWidth { get; }

        /// <summary> Gets the bit mask of the field in the register. </summary>
        int BitMask { get; }

        /// <summary> Gets a value indicating whether this bit field is globally hidden. </summary>
        bool IsHidden { get; }

        /// <summary> Gets a value indicating whether this bit field is hidden to language tools. </summary>
        bool IsLangHidden { get; }

        /// <summary> Gets a value indicating whether this bit field is hidden to the MPLAB IDE. </summary>
        bool IsIDEHidden { get; }

    }

}
