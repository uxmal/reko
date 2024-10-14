#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System;

namespace Reko.Analysis
{
	/// <summary>
	/// Represents an edge between two identifiers that intefere in an
	/// interference graph.
	/// </summary>
	public readonly struct Interference : IComparable<Interference>
	{
		public Interference(Identifier id1, Identifier id2)
		{
			if (string.Compare(id1.Name, id2.Name) < 0)
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

        public Identifier Identifier1 { get; }
        public Identifier Identifier2 { get; }

		public override bool Equals(object? obj)
		{
            return
                obj is Interference i &&
                i.Identifier1 == Identifier1 && i.Identifier2 == Identifier2;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Identifier1.Name, Identifier2.Name);
		}

		#region IComparable Members

		public int CompareTo(Interference that)
		{
            int d = string.Compare(Identifier1.Name, that.Identifier1.Name);
			if (d != 0)
				return d;
            return string.Compare(Identifier2.Name, that.Identifier2.Name);
		}

		#endregion
	}
}
