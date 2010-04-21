/* 
 * Copyright (C) 1999-2010 John Källén.
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
	/// Implements copy propagation.
	/// </summary>
	public class IdCopyPropagationRule
	{
		private SsaIdentifierCollection ssaIds;
		private SsaIdentifier sid;
		private Identifier idNew;

		public IdCopyPropagationRule(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
		}

		public bool Match(Identifier id)
		{
			sid = ssaIds[id];
			if (sid.DefStatement == null)
				return false;
			Assignment ass = sid.DefStatement.Instruction as Assignment;
			if (ass == null)
				return false;
			idNew = ass.Src as Identifier;
			return idNew != null;

		}

		public Expression Transform(Statement stm)
		{
			sid.Uses.Remove(stm);
			ssaIds[idNew].Uses.Add(stm);
			return idNew;
		}
	}
}
