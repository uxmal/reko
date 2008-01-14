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
	/// Represents a memory structure type, consisting of several Fields.
	/// </summary>
	public class StructureType : DataType
	{
		private int size;
		private bool isSegment;
		private StructureFieldCollection fields;

		public StructureType() : base(null)
		{
			this.fields = new StructureFieldCollection();
			this.size = 0;
		}

		public StructureType(string name, int size, StructureField field) : base(name)
		{
			this.fields = new StructureFieldCollection();
			this.size = size; 
			Fields.Add(field);
		}

		public StructureType(string name, int size) : base(name)
		{
			this.fields = new StructureFieldCollection();
			this.size = size; 
		}

		public override DataType Accept(DataTypeTransformer t)
		{
			return t.TransformStructure(this);
		}

		public override void Accept(IDataTypeVisitor v)
		{
			v.VisitStructure(this);
		}

		public override DataType Clone()
		{
			StructureType s = new StructureType(name, size);
			foreach (StructureField f in Fields)
			{
				s.Fields.Add(f.Clone());
			}
			return s;
		}


		public StructureFieldCollection Fields
		{
			get { return fields; }
		}

		public override bool IsComplex
		{
			get { return true; }
		}

		/// <summary>
		/// If true, the structure is an Intel-style segment. In particular, segments must never be converted to 
		/// primitive types.
		/// </summary>
		public bool IsSegment
		{
			get { return isSegment; }
			set { isSegment = value; }
		}

		public bool IsEmpty
		{
			get { return size == 0 && fields.Count == 0; }
		}

		public DataType Simplify()
		{
			if (Fields.Count == 1 && !IsSegment)
			{
				StructureField f = Fields[0];
				if (f.Offset == 0 && (Size == 0 || Size == f.DataType.Size))
				{
					return f.DataType;
				}
			}
			return this;
		}

		public override int Size
		{
			get { return size; }
			set { size = value; }
		}


		public override void Write(System.IO.TextWriter writer)
		{
			writer.Write("({0}", IsSegment ? "segment" : "struct");
			if (Name != null)
			{
				writer.Write(" \"{0}\"", Name);
			}
			if (Size != 0)
			{
				writer.Write(" {0:X}", Size);
			}

			if (Fields != null)
			{
				foreach (StructureField f in Fields)
				{
					writer.Write(" ({0:X} ", f.Offset);
					f.DataType.Write(writer);
					writer.Write(" {0})", f.Name);
				}
			}
			writer.Write(")");
		}
	}
}
