#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Expressions;
using System;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Represents an edge between two identifiers that intefere in an
	/// interference graph.
	/// </summary>
	public class Interference : IComparable<Interference>
	{
		public Interference(Identifier id1, Identifier id2)
		{
			if (id1.Number < id2.Number)
			{
				this.Identifier1 = id1;
				this.Identifier2 = id2;
			}
			else
			{
				this.Identifier1 = id2;
				this.Identifier2 = id1;
			}
		}

        public Identifier Identifier1 { get; private set; }
        public Identifier Identifier2 { get; private set; }

		public override bool Equals(object obj)
		{
			Interference i = obj as Interference;
			if (i == null)
				return false;
			return i.Identifier1 == Identifier1 && i.Identifier2 == Identifier2;
		}

		public override int GetHashCode()
		{
			return (Identifier1.Number.GetHashCode() << 8) |
				Identifier2.Number.GetHashCode();
		}

		#region IComparable Members

		public int CompareTo(Interference i)
		{
			int d = Identifier1.Number - i.Identifier1.Number;
			if (d != 0)
				return d;
			return Identifier2.Number - i.Identifier2.Number;
		}

		#endregion
	}
}
