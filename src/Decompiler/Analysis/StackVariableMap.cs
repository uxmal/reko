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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Text;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Maps an identifier to the used width of that identifier.
	/// </summary>
	public class StorageWidthMap : DictionaryBase
	{
		public StorageWidthMap()
		{
		}

		public StorageWidthMap(StorageWidthMap orig)
		{
			foreach (DictionaryEntry de in orig)
			{
				InnerHashtable.Add(de.Key, de.Value);
			}
		}

		public int this[Storage id]
		{
			get { return (int) InnerHashtable[id]; }
			set { InnerHashtable[id] = value; }
		}

		public void Add(Storage id, int bitsize)
		{
			InnerHashtable.Add(id, bitsize);
		}

		public StorageWidthMap Clone()
		{
			return new StorageWidthMap(this);
		}

		public bool Contains(Storage id)
		{
			return InnerHashtable.Contains(id);
		}

		public ICollection Keys
		{
			get { return InnerHashtable.Keys; }
		}

		public void Remove(Storage id)
		{
			InnerHashtable.Remove(id);
		}
	}
}
