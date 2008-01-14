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
	public class ArrayType : DataType
	{
		private DataType elType;
		private int length;

		public ArrayType(DataType elType, int length)
		{
			this.elType = elType;
			this.length = length;
		}

		public override DataType Accept(DataTypeTransformer t)
		{
			return t.TransformArrayType(this);
		}

		public override void Accept(IDataTypeVisitor v)
		{
			v.VisitArray(this);
		}

		public override DataType Clone()
		{
			return new ArrayType(ElementType, Length);
		}

		/// <summary>
		/// DataType of each element in the array.
		/// </summary>
		public DataType ElementType
		{
			get { return elType; }
			set { elType = value; }
		}

		/// <summary>
		/// Number of elements. 0 means unknown number of elements.
		/// </summary>
		public int Length
		{
			get { return length; }
		}

		public override bool IsComplex
		{
			get { return true; }
		}

		public override string Prefix
		{
			get { return "a"; }
		}

		public override int Size
		{
			get { return ElementType.Size * Length; }
			set { ThrowBadSize(); }
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("(arr ");
			ElementType.Write(writer);
			if (Length != 0)
			{
				writer.Write(" {0}", Length);
			}
			writer.Write(")");
		}
	}
}
