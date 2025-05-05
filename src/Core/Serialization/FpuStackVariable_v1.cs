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
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// XML format for a FPU stack variable.
	/// </summary>
	public class FpuStackVariable_v1 : SerializedStorage
	{
        /// <summary>
        /// Size of the stack variable in bytes.
        /// </summary>
		[XmlAttribute("size")]
		public int ByteSize;

        /// <summary>
        /// Creates an uninitialized instance of <see cref="FpuStackVariable_v1"/>.
        /// </summary>
		public FpuStackVariable_v1()
		{
		}

        /// <inheritdoc/>
		public override Identifier Deserialize(ArgumentDeserializer sser)
		{
			return sser.Deserialize(this);
		}
	}
}
