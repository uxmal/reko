#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Structure
{
    /// <summary>
    /// This class is used by the StructureAnalysis class to keep track
    /// of information known about a region of statements. Initially
    /// there will be a region for each non-trivial or non-reachable
    /// basic block. These regions are subsequently coalesced into fewer
    /// regions containing more instructions, until there finally is 
    /// only one region with all the statements in a (mostly) structured
    /// format.
    /// </summary>
    public class Region
    {
        public Block Block { get; private set; }
        public RegionType Type { get; set; }
        public List<AbsynStatement> Statements { get; set; }
        public Expression Expression { get; set; }

        /// <summary>
        /// Return true if region consists of a single AbsynReturn statement.
        /// </summary>
        public bool IsReturn
        {
            get
            {
                return Statements.Count == 1 && Statements[0] is AbsynReturn;
            }
        }

        public Region(Block block) : this(block, new List<AbsynStatement>())
        {
        }

        public Region(Block block, IEnumerable<AbsynStatement> stmts)
        {
            this.Block = block;
            this.Statements = new List<AbsynStatement>(stmts);
        }

        public override string ToString()
        {
            return Block.Name;
        }

        public virtual void Write(StringWriter sb)
        {
            var f = new AbsynCodeFormatter(new TextFormatter(sb));
            foreach (var stm in Statements)
                stm.Accept(f);
        }
    }

    public enum RegionType
    {
        Linear,
        Condition,
        IncSwitch,
        Tail,
    }
}
