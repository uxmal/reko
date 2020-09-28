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
 
using System;
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
        public bool ShowEdges { get; set; }

        public virtual void BeforeBlock(Block block, List<string> lines)
        {
        }

        public virtual void AfterBlock(Block block, List<string> lines)
        {
             WriteBlockGraphEdges(block, lines);
        }

        public virtual void WriteBlockGraphEdges(Block block, List<string> lines)
        {
            if (ShowEdges && block.Succ.Count > 0)
            {
                StringBuilder sb = new StringBuilder("succ: ");
                foreach (var s in block.Succ.Where(b => b != null))
                    sb.AppendFormat(" {0}", s.Name);
                lines.Add(sb.ToString());
            }
        }
    }
}
