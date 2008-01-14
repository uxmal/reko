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
using System.Diagnostics;
using System.IO;

namespace Decompiler.Core.Absyn
{
	/// <summary>
	/// Represents a compound statement.
	/// </summary>
	public class AbsynCompoundStatement : AbsynStatement
	{
		private AbsynStatementList stms;

		public AbsynCompoundStatement(AbsynStatementList stms)
		{
			this.stms = stms;
		}

		public override void Accept(IAbsynVisitor visitor)
		{
			visitor.VisitCompoundStatement(this);
		}

		public AbsynStatementList Statements
		{
			get { return stms; }
		}
	}

	public class AbsynStatementList : CollectionBase
	{
		public void Add(AbsynStatement stm)
		{
			InnerList.Add(stm);
		}

		public AbsynStatement this[int i]
		{
			get { return (AbsynStatement) InnerList[i]; }
			set { InnerList[i] = value; }
		}

		public void CopyTo(AbsynStatementList dst)
		{
			for (int i = 0; i < Count; ++i)
			{
				dst.InnerList.Add(this.InnerList[i]);
			}
		}

#if DEBUG
		public void Dump()
		{
			foreach (AbsynStatement stm in InnerList)
			{
				Debug.WriteLine(stm.ToString());
			}
		}
#else
		public void Dump()
		{
		}
#endif

		public void Insert(int at, AbsynStatement stm)
		{
			InnerList.Insert(at, stm);
		}

		public AbsynStatement MakeAbsynStatement()
		{
			if (Count > 1)
			{
				return new AbsynCompoundStatement(this);
			}
			else if (Count == 1)
				return this[0];
			else 
				return null;
		}
	}
}
