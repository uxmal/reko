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

using Reko.Core.Expressions;

namespace Reko.Core
{
    /// <summary>
    /// Represents a user-specified register value.
    /// </summary>
    public class UserRegisterValue
    {
        /// <summary>
        /// Constructns a user-specified register value.
        /// </summary>
        /// <param name="register">Register name.</param>
        /// <param name="value">Register value.</param>
        public UserRegisterValue(Storage register, Constant value)
        {
            Register = register;
            Value = value;
        }

        /// <summary>
        /// The register to which the value is assigned.
        /// </summary>
        public Storage Register { get; }

        /// <summary>
        /// The value to assign to the register.
        /// </summary>
        public Constant Value { get; }
    }
}
