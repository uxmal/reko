/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    ///<summary>
    /// Tags this node and all its children within the case defined by (head,follow)
    /// as belonging to the case. If a node visited in this traversal is already with
    /// a case then it is left untouched.
    ///</summary>
    public class CaseFinder
    {
        private StructureNode head;
        private StructureNode follow;
        private HashedSet<StructureNode> visited = new HashedSet<StructureNode>();

        public CaseFinder(StructureNode head, StructureNode follow)
        {
            this.head = head;
            this.follow = follow;
        }

        /// <summary>
        /// Recursively associates the <paramref name="node"/> and its descendants with
        /// the case statement defined by head. The recursion ends when the follow node 
        /// of the head node is encountered.
        /// </summary>
        /// <param name="node"></param>
        public void SetCaseHead(StructureNode node)
        {
            Debug.Assert(node.CaseHead == null);
            visited.Add(node);

            // don't tag this node if it is the case header under investigation 
            if (node != head)
                node.CaseHead = head;

            // if this is a nested case header, then its member nodes will already have been
            // tagged so skip straight to its follow
            if (node.BlockType == bbType.nway && node != head)
            {
                if (!visited.Contains(node.Conditional.Follow) &&
                    node.Conditional.Follow != follow)
                {
                    SetCaseHead(node.Conditional.Follow);
                }
            }
            else
            {
                // traverse each child of this node that:
                //   i) isn't on a back-edge,
                //  ii) hasn't already been traversed in a case tagging traversal and,
                // iii) isn't the follow node.
                foreach (StructureNode succ in node.OutEdges)
                {
                    if (!node.HasBackEdgeTo(succ) &&
                        !visited.Contains(succ) &&
                        succ != follow)
                    {
                        SetCaseHead(succ);
                    }
                }
            }
        }
    }
}
