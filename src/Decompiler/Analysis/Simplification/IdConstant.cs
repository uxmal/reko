/* 
 * Copyright (C) 1999-2007 John Källén.
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
using System;

namespace Decompiler.Analysis.Simplification
{
	/// <summary>
	/// Implements constant propagation.
	/// </summary>
	public class IdConstant
	{
		private SsaIdentifierCollection ssaIds;
		private SsaIdentifier sid;
		private Constant c;

		public IdConstant(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
		}

		public bool Match(Identifier id)
		{
			sid = ssaIds[id];
			if (sid.def == null)
				return false;
			Assignment ass = sid.def.Instruction as Assignment;
			if (ass == null)
				return false;
			c = ass.Src as Constant;
			return c != null;
		}

		public Expression Transform(Statement stm)
		{
			sid.uses.Remove(stm);
			return new Constant(c.DataType, c.Value);
		}
	}
}