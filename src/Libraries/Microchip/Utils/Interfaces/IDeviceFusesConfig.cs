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

using System.Collections.Generic;

namespace Reko.Libraries.Microchip
{
    /// <summary>
    /// This interface provides access to a PIC device configuration register (configuration fuses).
    /// </summary>
    public interface IDeviceFusesConfig
    {
        /// <summary> Gets the memory address of the device configuration register. </summary>
        int Addr { get; }

        /// <summary> Gets the name of the device configuration register. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of the device configuration register. </summary>
        string Description { get; }

        /// <summary> Gets the bit width of the device configuration register. </summary>
        int BitWidth { get; }

        /// <summary> Gets the implemented bit mask of the device configuration register. </summary>
        int ImplMask { get; }

        /// <summary> Gets the access modes of the device configuration register's bits. </summary>
        string AccessBits { get; }

        /// <summary> Gets the default value of the device configuration register. </summary>
        int DefaultValue { get; }

        /// <summary> Gets a value indicating whether this register is hidden to language tools. </summary>
        bool IsLangHidden { get; }

        /// <summary> Enumerates the illegal settings for this configuration register. </summary>
        IEnumerable<IDeviceFusesIllegal> IllegalSettings { get; }

        /// <summary> Enumerates the bit fields of the configuration register. </summary>
        IEnumerable<IDeviceFusesField> ConfigFields { get; }

    }

    /// <summary>
    /// This interface provides conditions for illegal device configuration settings.
    /// </summary>
    public interface IDeviceFusesIllegal
    {
        /// <summary> Gets the "when" pattern of the illegal condition. </summary>
        string When { get; }

        /// <summary> Gets the textual description of the illegal condition. </summary>
        string Description { get; }

    }

    /// <summary>
    /// This interface provides access to a PIC device configuration fuses field.
    /// </summary>
    public interface IDeviceFusesField : IRegisterBitField
    {
        /// <summary> Enumerates the semantics of the settings for this configuration field. </summary>
        IEnumerable<IDeviceFusesSemantic> Semantics { get; }

    }

    /// <summary>
    /// This interface provides access to a semantic of a PIC device configuration field (fuses).
    /// </summary>
    public interface IDeviceFusesSemantic
    {
        /// <summary> Gets the name of the fuses field. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of the field configuration pattern. </summary>
        string Description { get; }

        /// <summary> Gets the 'when' condition for the field value (configuration pattern). </summary>
        string When { get; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden. </summary>
        bool IsHidden { get; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden to language tools. </summary>
        bool IsLangHidden { get; }

    }

}
