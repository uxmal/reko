#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
#endregion

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Inserts declaration statements for local variables so they appear
    /// as close to all their uses and definitions. If a single definition
    /// dominates all others, then it is enough to use one definition.
    /// </summary>
	public class DeclarationInserter
	{
		private readonly BlockDominatorGraph doms;

		public DeclarationInserter(BlockDominatorGraph doms)
		{
			this.doms = doms;
		}

		public void InsertDeclaration(Web web)
		{
            var blocks = new HashSet<Block>();
			foreach (SsaIdentifier sid in web.Members)
			{
				if (sid.DefStatement != null)
				{
                    if (IsImplicitDeclaration(sid.DefStatement))
                        return;
                    blocks.Add(sid.DefStatement.Block);
					foreach (Statement u in sid.Uses)
					{
						blocks.Add(u.Block);
					}
				}
			}

			Block? dominator = doms.CommonDominator(blocks);

			if (dominator != null)
			{
				foreach (SsaIdentifier sid in web.Members)
				{
					if (sid.DefStatement != null && sid.DefStatement.Block == dominator)
					{
                        if (sid.DefStatement.Instruction is Assignment ass &&
                            ass.Dst == sid.Identifier)
                        {
                            sid.DefStatement.Instruction = new Declaration(web.Identifier!, ass.Src);
                        }
                        else
                        {
                            int idx = dominator.Statements.IndexOf(sid.DefStatement);
                            dominator.Statements.Insert(
                                idx,
                                sid.DefStatement.Address,
                                new Declaration(web.Identifier!, null));
                        }
                        return;
					}
				}
				dominator.Statements.Insert(
                    0,
                    dominator.Address,
                    new Declaration(web.Identifier!, null));
			}
		}

        static private bool IsImplicitDeclaration(Statement stm)
        {
            if (stm.Instruction is not DefInstruction def)
                return false;
            // Do not add declarations for global variables
            if (def.Identifier.Storage is GlobalStorage)
                return true;
            // Do not add declarations for parameters of procedure
            var sig = stm.Block.Procedure.Signature;
            if (
                sig.ParametersValid &&
                sig.Parameters!.Any(p => p.Name == def.Identifier.Name))
            {
                return true;
            }
            return false;
        }
    }
}
