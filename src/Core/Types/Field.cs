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

namespace Reko.Core.Types;


/// <summary>
/// Represents a field in a structure or class.
/// </summary>
public abstract class Field
{
        /// <summary>
        /// Initializes a new instance of a subclass of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="type"></param>
    protected Field(DataType type)
    {
        this.DataType = type;
    }

        /// <summary>
        /// The data type of this field.
        /// </summary>
    public DataType DataType { get; set; }

        /// <summary>
        /// The name of this field.
        /// </summary>
    public abstract string Name { get; set; }
}
