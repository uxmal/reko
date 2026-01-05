#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
    /// This class is used by the <see cref="StructureAnalysis"/> class to keep track
    /// of information known about a region of statements. Initially
    /// there will be a region for each non-trivial or non-reachable
    /// basic block. These regions are subsequently coalesced into fewer
    /// regions containing more instructions, until there finally is 
    /// only one region with all the statements in a (mostly) structured
    /// format.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The basic block that this region represents.
        /// </summary>
        public Block Block { get; private set; }

        /// <summary>
        /// The type of the region.
        /// </summary>
        public RegionType Type { get; set; }

        /// <summary>
        /// The ordered list of statements in this region.
        /// </summary>
        public List<AbsynStatement> Statements { get; set; }

        /// <summary>
        /// Optional predicate expression, used in conditional constructs
        /// like if-then-else, while, etc.
        /// </summary>
        public Expression? Expression { get; set; }

        /// <summary>
        /// True if this region is a switch pad.
        /// </summary>
        public bool IsSwitchPad { get; set; }

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

        /// <summary>
        /// Constructs an empty region for the given block.
        /// </summary>
        /// <param name="block">Basic block for the region.</param>
        public Region(Block block) : this(block, new List<AbsynStatement>())
        {
        }

        /// <summary>
        /// Constructs a region for the given block and statements.
        /// </summary>
        /// <param name="block">Basic block for the region.</param>
        /// <param name="stmts"><see cref="AbsynStatement"/> for the region.</param>
        public Region(Block block, IEnumerable<AbsynStatement> stmts)
        {
            this.Block = block;
            this.Statements = new List<AbsynStatement>(stmts);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Block.DisplayName + (IsSwitchPad ? "_$sw" : "");
        }

        /// <summary>
        /// Renders the txtual representation of the region to the given
        /// <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">Text writer to write to.</param>
        public void Write(TextWriter writer)
        {
            var f = new AbsynCodeFormatter(new TextFormatter(writer));
            foreach (var stm in Statements)
            {
                stm.Accept(f);
            }
        }
    }

    /// <summary>
    /// Classification of regions.
    /// </summary>
    public enum RegionType
    {
        /// <summary>
        /// A linear sequence of statements.
        /// </summary>
        Linear,

        /// <summary>
        /// A conditional region, such as an if-then-else or a while loop.
        /// </summary>
        Condition,

        /// <summary>
        /// An incomplete switch statement (having no guard statement).
        /// </summary>
        IncSwitch,

        /// <summary>
        /// A tail region, which is a region that is terminated by a return statement.
        /// </summary>
        Tail,
    }
}
