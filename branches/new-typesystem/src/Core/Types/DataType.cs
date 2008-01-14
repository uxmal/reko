/* 
 * Copyright (C) 1999-2008 John Källén.
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

using System;
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents concrete C-like data types inferred by the decompiler as part of the decompilation process.
	/// </summary>
	/// <remarks>
	/// The name 'DataType' is used to avoid conflicts with 'Type', which is part of the CLR.
	/// </remarks>
	public abstract class DataType
	{
		protected string name;

		public const int BitsPerByte = 8;

		protected DataType()
		{
		}

		protected DataType(string name)
		{
			this.name = name;
		}


		public abstract DataType Accept(DataTypeTransformer t);

		public abstract void Accept(IDataTypeVisitor v);

		public int BitSize
		{
			get { return Size * BitsPerByte; }			//$REVIEW: Wrong for 36-bit machines
		}

		public abstract DataType Clone();

		public virtual bool IsComplex
		{
			get { return false; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		

		public virtual string Prefix
		{
			get { return "t"; }
		}

		/// <summary>
		/// Size in bytes of the concrete datatype.
		/// </summary>
		public abstract int Size 
		{ 
			get; set;
		}

		protected void ThrowBadSize()
		{
			throw new InvalidOperationException(string.Format("Can't set size of {0}.", GetType().Name));
		}

		public sealed override string ToString()
		{
			StringWriter sw = new StringWriter();
			Write(sw);
			return sw.ToString();
		}

		public virtual void Write(TextWriter writer)
		{
			writer.Write(Name);
		}
	}
}
