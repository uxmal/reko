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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
    public class PostDominatorGraph
    {
        /// <summary>
        /// Finds the common post dominator of the current immediate post dominator
        /// and its successor's immediate post dominator
        /// </summary>
        /// <param name="curImmPDom"></param>
        /// <param name="succImmPDom"></param>
        /// <returns></returns>
        private StructureNode CommonPostDominator(StructureNode curImmPDom, StructureNode succImmPDom)
        {
            if (curImmPDom == null)
                return succImmPDom;
            if (succImmPDom == null)
                return curImmPDom;

            while (curImmPDom != null && succImmPDom != null && (curImmPDom != succImmPDom))
            {
                if (curImmPDom.RevOrder > succImmPDom.RevOrder)
                    succImmPDom = succImmPDom.ImmPostDominator;
                else
                    curImmPDom = curImmPDom.ImmPostDominator;
            }

            return curImmPDom;
        }

        /// <summary>Finds the immediate post dominator of each node in the graph.</summary>
        /// <remarks>
        /// Adapted version of the dominators algorithm by Hecht and Ullman; finds
        /// immediate post dominators only.
        /// Note: graph should be reducible
        /// </remarks>
        public void FindImmediatePostDominators(ProcedureStructure proc)
        {
            // traverse the nodes in order (i.e from the bottom up)
            for (int i = proc.RevOrdering.Count - 1; i >= 0; i--)
            {
                StructureNode curNode = proc.RevOrdering[i];
                foreach (StructureNode succNode in curNode.Succ)
                {
                    if (succNode.RevOrder > curNode.RevOrder)
                        curNode.ImmPostDominator = CommonPostDominator(curNode.ImmPostDominator, succNode);
                }
            }

            // make a second pass but consider the original CFG ordering this time
            foreach (StructureNode curNode in proc.Ordering)
            {
                if (curNode.Succ.Count > 1)
                {
                    foreach (StructureNode succNode in curNode.Succ)
                    {
                        curNode.ImmPostDominator = CommonPostDominator(curNode.ImmPostDominator, succNode);
                    }
                }
            }

            // one final pass to fix up nodes involved in a loop
            foreach (StructureNode curNode in proc.Ordering)
            {
                List<StructureNode> oEdges = curNode.Succ;
                if (curNode.Succ.Count > 1)
                {
                    foreach (StructureNode succNode in curNode.Succ)
                    {
                        if (curNode.HasBackEdgeTo(succNode) && curNode.Succ.Count > 1 &&
                             succNode.ImmPostDominator.Order < curNode.ImmPostDominator.Order)
                            curNode.ImmPostDominator = CommonPostDominator(succNode.ImmPostDominator, curNode.ImmPostDominator);
                        else
                            curNode.ImmPostDominator = CommonPostDominator(curNode.ImmPostDominator, succNode);
                    }
                }
            }
        }
    }
}
