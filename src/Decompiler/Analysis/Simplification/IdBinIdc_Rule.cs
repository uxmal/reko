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

using Decompiler.Core.Code;
using Decompiler.Core;

using System;

namespace Decompiler.Analysis.Simplification
{
	/// <summary>
	/// If we find a = x OP CONST followed by b = a, then replace so that b = x OP CONST.
	/// </summary>
	public class IdBinIdc_Rule
	{
		private SsaIdentifierCollection ssaIds;
		private BinaryExpression bin;
		private SsaIdentifier sid;

		public IdBinIdc_Rule(SsaIdentifierCollection ssaIds)
		{
			this.ssaIds = ssaIds;
		}

		public bool Match(Identifier id)
		{
			sid = ssaIds[id];
			Statement s = sid.DefStatement;
			if (s == null)
				return false;
			if (sid.Uses.Count != 1)
				return false;
			Assignment ass = s.Instruction as Assignment;
			if (ass == null)
				return false;
			bin = ass.Src as BinaryExpression;
			if (bin == null)
				return false;
			return (bin.Left is Identifier) && (bin.Right is Constant);
		}

		public Expression Transform(Statement stm)
		{
			sid.Uses.Remove(stm);
			ExpressionUseAdder eua = new ExpressionUseAdder(stm, ssaIds);
			bin.Accept(eua);
			return bin;
		}
	}
}
