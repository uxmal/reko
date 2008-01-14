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
	public class BlockList : CollectionBase
	{
		public BlockList()
		{
		}

		public BlockList(int c)
		{
			for (int i = 0; i < c; ++i)
			{
				Add(null);
			}
		}

		public BlockList(ICollection c)
		{
			foreach (Block b in c)
			{
				Add(b);
			}
		}

		public int Add(Block b)
		{
			return InnerList.Add(b);
		}

		public Block this[int i]
		{
			get { return (Block) InnerList[i]; }
			set { InnerList[i] = value; }
		}

		public bool Contains(Block b)
		{
			return InnerList.Contains(b);
		}

		public void CopyTo(Array a)
		{
			InnerList.CopyTo(a);
		}
		public int IndexOf(Block b)
		{
			return InnerList.IndexOf(b);
		}

		public void Remove(Block b)
		{
			InnerList.Remove(b);
		}
	}
}

