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
using System.Collections;

namespace Decompiler.Core.Types
{
	public class StructureField
	{
		public int Offset;
		public DataType DataType;
		private string name;

		public StructureField(int offset, DataType type)
		{
			this.Offset = offset; this.DataType = type;
		}

		public StructureField(int offset, DataType type, string name)
		{
			this.Offset = offset; this.DataType = type; this.name = name;
		}

		public StructureField Clone()
		{
			return new StructureField(Offset, DataType.Clone(), name);
		}

		public string Name
		{
			get 
			{ 
				if (name != null)
					return name;
				return string.Format("{0}{1:X4}", DataType.Prefix, Offset);
			}
		}
	}

	public class StructureFieldCollection : CollectionBase
	{
		public StructureField this[int i]
		{
			get { return (StructureField) InnerList[i]; }
			set { InnerList[i] = value; }
		}

		public void Add(int offset, DataType dt)
		{
			Add(new StructureField(offset, dt));
		}

		public void Add(int offset, DataType dt, string name)
		{
			Add(new StructureField(offset, dt, name));
		}

		public void Add(StructureField f)
		{
			int i;
			StructureField ff = null;
			for (i = 0; i < InnerList.Count; ++i)
			{
				ff = (StructureField) InnerList[i];
				if (f.Offset == ff.Offset)
				{
					if (f.DataType == ff.DataType)
						return;
				}
				if (f.Offset <= ff.Offset)
					break;
			}
			InnerList.Insert(i, f);
		}

		public StructureField [] ArrayCopy()
		{
			StructureField [] fs = new StructureField[InnerList.Count];
			for (int i = 0; i < fs.Length; ++i)
			{
				fs[i] = (StructureField) InnerList[i];
			}
			return fs;
		}

		public void Insert(int i, StructureField f)
		{
			InnerList.Insert(i, f);
		}

		public StructureField LowerBound(int offset)
		{
			StructureField fPrev = null;
			foreach (StructureField f in InnerList)
			{
				if (f.Offset <= offset)
					fPrev = f;
			}
			return fPrev;
		}

		/// <summary>
		/// Returns the structurefield exactly located at the specified offset.
		/// </summary>
		/// <param name="offset">Offset (in bytes) of the field to retrieve.</param>
		/// <returns>The requested StructureField if it exists at <paramref>offset</paramref>, otherwise null.</returns>
		public StructureField AtOffset(int offset)
		{
			foreach (StructureField f in InnerList)
			{
				if (f.Offset == offset)
					return f;
			}
			return null;
		}
	}
}
