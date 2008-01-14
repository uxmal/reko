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

namespace Decompiler.Core
{
	/// <summary>
	/// Represents a table of jumps or calls. Initially, it is empty.
	/// item, but further analysis may grow this size.
	/// </summary>
	public class ImageMapVectorTable : ImageMapItem
	{
		private bool fCallTable;
		private ArrayList addresses;
		private MapAddressToRegister registerUsed = new MapAddressToRegister();

		public ImageMapVectorTable(bool fCallTable)
		{
			this.fCallTable = fCallTable;
			this.addresses = new ArrayList();
		}

		public ImageMapVectorTable(bool isCallTable, Address [] vector, int size) 
		{
			this.fCallTable = isCallTable;
			this.addresses = new ArrayList(vector);
		}

		public ArrayList Addresses { get { return addresses; } }

		public bool IsCallTable
		{
			get { return fCallTable; }
		}

		public MapAddressToRegister RegisterUsed
		{
			get { return registerUsed; }
		}
	}

	public class MapAddressToRegister : DictionaryBase
	{
		public int this[Address addr]
		{
			get 
			{
				object o = InnerHashtable[addr];
				if (o != null)
					return (int) o;
				else 
					return -1;
			}
			set { InnerHashtable[addr] = value; }
		}
	}
}
