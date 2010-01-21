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

using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
    /// <summary>Finds the immediate post dominator of each node in the graph.</summary>
    /// <remarks>
    /// Adapted version of the dominators algorithm by Hecht and Ullman; finds
    /// immediate post dominators only.
    /// Note: graph should be reducible
    /// </remarks>
    public class PostDominatorGraph
    {
        private ProcedureStructure proc;
        private StructureGraphAdapter graph;

        public PostDominatorGraph()
        { }

        public PostDominatorGraph(ProcedureStructure proc)
        {
            this.proc = proc;
            this.graph = new StructureGraphAdapter(proc.Nodes);
        }

        public void FindImmediatePostDominators()
        {
            // traverse the nodes in order (i.e from the bottom up)
            for (int i = proc.ReverseOrdering.Count - 1; i >= 0; i--)
            {
                StructureNode curNode = proc.ReverseOrdering[i];
                foreach (StructureNode succNode in curNode.OutEdges)
                {
                    if (succNode.RevOrder > curNode.RevOrder)
                        curNode.ImmPDom = CommonPostDominator(curNode.ImmPDom, succNode);
                }
            }

            // make a second pass but consider the original CFG ordering this time
            foreach (StructureNode curNode in proc.Ordering)
            {
                if (curNode.OutEdges.Count > 1)
                {
                    foreach (StructureNode succNode in curNode.OutEdges)
                    {
                        curNode.ImmPDom = CommonPostDominator(curNode.ImmPDom, succNode);
                    }
                }
            }

            // one final pass to fix up nodes involved in a loop
            foreach (StructureNode curNode in proc.Ordering)
            {
                List<StructureNode> oEdges = curNode.OutEdges;
                if (curNode.OutEdges.Count > 1)
                {
                    foreach (StructureNode succNode in curNode.OutEdges)
                    {
                        if (curNode.HasBackEdgeTo(succNode) && curNode.OutEdges.Count > 1 &&
                             succNode.ImmPDom.Order < curNode.ImmPDom.Order)
                            curNode.ImmPDom = CommonPostDominator(succNode.ImmPDom, curNode.ImmPDom);
                        else
                            curNode.ImmPDom = CommonPostDominator(curNode.ImmPDom, succNode);
                    }
                }
            }
        }
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
                    succImmPDom = succImmPDom.ImmPDom;
                else
                    curImmPDom = curImmPDom.ImmPDom;
            }

            return curImmPDom;
        }



    }
}
