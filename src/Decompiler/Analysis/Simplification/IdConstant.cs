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
using Decompiler.Typing;
using System;

namespace Decompiler.Analysis.Simplification
{
	/// <summary>
	/// Implements constant propagation.
	/// </summary>
	public class IdConstant
	{
		private SsaIdentifierCollection ssaIds;
		private Unifier unifier;
		private SsaIdentifier sid;
		private Constant cSrc;
		private Identifier idDst;

		public IdConstant(SsaIdentifierCollection ssaIds, Unifier u)
		{
			this.ssaIds = ssaIds;
			this.unifier = u;
		}

		public bool Match(Identifier id)
		{
			sid = ssaIds[id];
			if (sid.def == null)
				return false;
			Assignment ass = sid.def.Instruction as Assignment;
			if (ass == null)
				return false;
			cSrc = ass.Src as Constant;
			idDst = id;
			return cSrc != null;
		}

		public Expression Transform(Statement stm)
		{
			sid.uses.Remove(stm);
			DataType dt = unifier.Unify(cSrc.DataType, idDst.DataType);
			if (dt is PrimitiveType)
				return new Constant(dt, cSrc.Value);
			throw new NotSupportedException(string.Format("Resulting type is {0}, which isn't supported yet.", dt));
		}
	}
}