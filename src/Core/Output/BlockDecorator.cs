#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// This class us used by ProcedureFormatter to render block-specific information
    /// before and after blocks are rendered. These will always be rendered as comments.
    /// </summary>
    public class BlockDecorator
    {
        /// <summary>
        /// If true, render the edges of the block graph.
        /// </summary>
        public bool ShowEdges { get; set; }

        /// <summary>
        /// Called before the block is rendered. Derived classes can override this
        /// to write additional information before the block is rendered.
        /// </summary>
        /// <param name="block">Block to render.</param>
        /// <param name="lines">A list of strings, to collect the output.</param>
        public virtual void BeforeBlock(Block block, List<string> lines)
        {
        }

        /// <summary>
        /// Called after the block is rendered. Derived classes can override this
        /// to write additional information before the block is rendered. By default,
        /// this method renders the graph edges after the block contents.
        /// </summary>
        /// <param name="block">Block to render.</param>
        /// <param name="lines">A list of strings, to collect the output.</param>
        public virtual void AfterBlock(Block block, List<string> lines)
        {
             WriteBlockGraphEdges(block, lines);
        }

        /// <summary>
        /// Render the basic block's successor edges.
        /// </summary>
        /// <param name="block">Basic block whose successor edges are to be rendered.</param>
        /// <param name="lines">A list of strings, to collect the output.</param>
        public virtual void WriteBlockGraphEdges(Block block, List<string> lines)
        {
            if (ShowEdges && block.Succ.Count > 0)
            {
                StringBuilder sb = new StringBuilder("succ: ");
                foreach (var s in block.Succ.Where(b => b is not null))
                    sb.AppendFormat(" {0}", s.DisplayName);
                lines.Add(sb.ToString());
            }
        }
    }
}
