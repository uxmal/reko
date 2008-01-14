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

using Decompiler.Core.Code;
using System;
using System.Collections;

namespace Decompiler.Core
{
	public class StatementList : CollectionBase
	{
		private Block block;

		public StatementList(Block block)
		{
			this.block = block;
		}

		public int Add(Instruction instr)
		{
			return List.Add(new Statement(instr, block));
		}

		public int Add(Statement stm)
		{
			return List.Add(stm);
		}

		public Statement this[int i]
		{
			get { return (Statement) List[i]; }
			set { List[i] = value; }
		}

		public object [] CopyToArray()
		{
			return (object []) InnerList.ToArray();
		}

		public int IndexOf(Statement stm)
		{
			return List.IndexOf(stm);
		}

		public void Insert(int at, Instruction instr)
		{
			List.Insert(at, new Statement(instr, block));
		}

		public void Insert(int at, Statement stm)
		{
			List.Insert(at, stm);
		}

		public Statement Last
		{
			get 
			{ 
				int i = List.Count - 1;
				return i >= 0 ? (Statement) InnerList[i] : null;
			}
		}

		public void Remove(Statement stm)
		{
			List.Remove(stm);
		}
	}
}
