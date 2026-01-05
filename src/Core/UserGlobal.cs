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

namespace Reko.Core
{
    /// <summary>
    /// Models a user-specified global varaible.
    /// </summary>
    public class UserGlobal
    {
        /// <summary>
        /// The address of the global variable.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// The name of the global variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the global variable.
        /// </summary>
        public SerializedType DataType { get; set; }

        /// <summary>
        /// Optional comment for the global variable.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Constructs a user global variable.
        /// </summary>
        /// <param name="address">Address of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="dataType">Data type of the variable.</param>
        public UserGlobal(Address address, string name, SerializedType dataType)
        {
            Address = address;
            Name = name;
            DataType = dataType;
        }

        /// <summary>
        /// Generates a default name for the global variable, based on its address.
        /// </summary>
        public static string GenerateDefaultName(Address address) => $"g_{address.ToLinear():X}";
    }
}
