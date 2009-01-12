/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Code;
using System;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Represents an edge between two identifiers that intefere in an
	/// interference graph.
	/// </summary>
	public class Interference : IComparable
	{
		private Identifier id1;
		private Identifier id2;

		public Interference(Identifier id1, Identifier id2)
		{
			if (id1.Number < id2.Number)
			{
				this.id1 = id1;
				this.id2 = id2;
			}
			else
			{
				this.id1 = id2;
				this.id2 = id1;
			}
		}

		public override bool Equals(object obj)
		{
			Interference i = obj as Interference;
			if (i == null)
				return false;
			return i.id1 == id1 && i.id2 == id2;
		}

		public override int GetHashCode()
		{
			return (id1.Number.GetHashCode() << 8) |
				id2.Number.GetHashCode();
		}

		public Identifier Identifier1
		{
			get { return id1; }
		}

		public Identifier Identifier2
		{
			get { return id2; }
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			Interference i = (Interference) obj;
			int d = id1.Number - i.id1.Number;
			if (d != 0)
				return d;
			return id2.Number - i.id2.Number;
		}

		#endregion
	}
}
