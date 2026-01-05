#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Types;

namespace Reko.Core
{
    /// <summary>
    /// A Global variable has an address, a type and a name.
    /// </summary>
    public class GlobalVariable : IAddressable
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="GlobalVariable"/> class.
        /// </summary>
        /// <param name="addr">Address of the global variable.</param>
        /// <param name="dt">Data type of the global variable.</param>
        /// <param name="name">The name of the global variable.</param>
        public GlobalVariable(Address addr, DataType dt, string name)
        {
            this.Address = addr;
            this.DataType = dt;
            this.Name = name;
        }

        /// <summary>
        /// The address of the global variable.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// The data type of the global variable.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// The name of the global variable.
        /// </summary>
        public string  Name { get; }
    }
}
