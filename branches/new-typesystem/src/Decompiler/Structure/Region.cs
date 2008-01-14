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
using System;

namespace Decompiler.Structure
{
	/// <summary>
	/// Base class for all types of structures in our code.
	/// </summary>
	public class Region
	{
		public Region()
		{
		}
	}

	public class BlockRegion : Region
	{
		private StatementList stms;

		public BlockRegion(StatementList stms)
		{
			this.stms = stms;
		}
	}

	public class IfRegion : Region
	{
		public IfRegion(StatementList c, StatementList t, StatementList e)
		{
		}
	}

	public class WhileRegion : Region
	{
		public WhileRegion(StatementList head)
		{
		}
	}
}
