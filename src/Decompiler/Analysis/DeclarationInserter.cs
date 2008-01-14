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
using System;
using System.Collections;

namespace Decompiler.Analysis
{
	public class DeclarationInserter
	{
		private SsaIdentifierCollection ssaIds;
		private DominatorGraph doms;

		public DeclarationInserter(SsaIdentifierCollection ssaIds, DominatorGraph doms)
		{
			this.ssaIds = ssaIds;
			this.doms = doms;
		}

		public void InsertDeclaration(Web web)
		{
			ArrayList blocks = new ArrayList();
			foreach (SsaIdentifier sid in web.Members)
			{
				if (sid.def != null)
				{
					if (sid.def.Instruction is DefInstruction)
						blocks.Add(null);
					else
						blocks.Add(sid.def.block);
					foreach (Statement u in sid.uses)
					{
						blocks.Add(u.block);
					}
				}
			}

			Block dominator = doms.CommonDominator(blocks);

			if (dominator != null)
			{
				foreach (SsaIdentifier sid in web.Members)
				{
					if (sid.def != null && sid.def.block == dominator)
					{
						Assignment ass = sid.def.Instruction as Assignment;
						if (ass != null && ass.Dst == sid.id)
						{
							sid.def.Instruction = new Declaration(web.id, ass.Src);
						}
						else
						{
							int idx = dominator.Statements.IndexOf(sid.def);
							dominator.Statements.Insert(idx, new Declaration(web.id, null));
						}
						return;
					}
				}
				dominator.Statements.Insert(0, new Declaration(web.id, null));
			}
		}
	}
}
