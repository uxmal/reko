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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Structure
{
    public class ProcedureStructureBuilder
    {
        private List<StructureNode> nodeList;			// head of the linked list of nodes

        public ProcedureStructureBuilder()
        {
            this.nodeList = new List<StructureNode>();
        }

        public void BuildNodes(Procedure proc, Dictionary<Block, StructureNode> blockNodes)
        {
            int bId = 0;
            foreach (Block b in proc.RpoBlocks)
            {
                StructureNode cfgNode = new StructureNode(b, ++bId);
                nodeList.Add(cfgNode);
                blockNodes.Add(b, cfgNode);
            }
            if (!blockNodes.ContainsKey(proc.ExitBlock))
            {
                StructureNode cfgNode = new StructureNode(proc.ExitBlock, ++bId);
                nodeList.Add(cfgNode);
                blockNodes.Add(proc.ExitBlock, cfgNode);
            }
        }

        private void DebugBuildNodes()
        {
            foreach (StructureNode newNode in nodeList)
            {
                List<Statement> ins = newNode.Instructions;
                Debug.Write("Block #" + newNode.Ident() + " is of type " + newNode.BlockType);
                Debug.WriteLine(" and contains:");

                for (int i = 0; i < ins.Count; i++)
                {
                    Console.Error.WriteLine("\t");
                    Console.Error.WriteLine(ins[i].ToString());
                }
            }
        }


        // define the edges between these nodes as well as removing any redundant nodes
        public void DefineEdges(Procedure proc, Dictionary<Block, StructureNode> blockNodes)
        {
            //build the edges
            foreach (StructureNode curNode in nodeList)
            {
                foreach (Block s in curNode.Block.Succ)
                {
                    curNode.AddEdgeTo(blockNodes[s]);
                }
            }

            //tag the nodes that are reachable from the head of a procedure.
            blockNodes[proc.EntryBlock].DfsTag();

            DebugDefineEdges();
        }

        private void DebugDefineEdges()
        {
            foreach (StructureNode curNode in nodeList)
            {
                List<StructureNode> oEdges = curNode.OutEdges;
                List<StructureNode> iEdges = curNode.InEdges;
                Console.Error.Write("Node #" + curNode.Ident() + ": ");
                if (oEdges.Count > 0)
                {
                    Console.Error.Write(" outedges = {");
                    for (int i = 0; i < oEdges.Count; i++)
                        Console.Error.Write(oEdges[i].Ident() + " ");
                    Console.Error.Write("}");
                }

                if (iEdges.Count > 0)
                {
                    Console.Error.Write(" inedges = {");
                    for (int i = 0; i < iEdges.Count; i++)
                        Console.Error.Write(iEdges[i].Ident() + " ");
                    Console.Error.Write("}");
                }
                Console.Error.WriteLine();
            }
        }

        // build the headers for each cfg within the program
        public ProcedureStructure DefineCfgs(Procedure proc, Dictionary<Block, StructureNode> blocks)
        {
            List<StructureNode> nodes = new List<StructureNode>();
            foreach (StructureNode node in blocks.Values)
            {
                nodes.Add(node);
            }

            ProcedureStructure newProc = new ProcedureStructure(proc, nodes);
            StructureNode curNode = blocks[proc.EntryBlock];
            newProc.EntryNode = curNode;
            newProc.ExitNode = blocks[proc.ExitBlock];
            return newProc;
        }


        // Build the sequence of derived graphs for each procedure
        public void BuildDerivedSequences(ProcedureStructure curProc)
        {
            DerivedSequenceBuilder d = new DerivedSequenceBuilder();
            d.BuildDerivedSequence(curProc);
        }

        // Display the sequence of derived graphs for each procedure
        public void DisplayDerivedSequences(ProcedureStructure curProc)
        {
            DisplayDerivedSequence(curProc);
        }







        //********************************************************************************
        // Post-processing structuring routines
        //********************************************************************************





        private void DisplayDerivedSequence(ProcedureStructure proc)
        {
            Console.Out.WriteLine("Derived sequence intervals for procedure {0}", proc.Name);

            for (int i = 0; i < proc.DerivedGraphs.Count; i++)
            {
                DerivedGraph curGraph = proc.DerivedGraphs[i];
                Console.Out.WriteLine();
                Console.Out.WriteLine("\nDerived graph #{0}:", i);
                curGraph.DisplayIntervals();
            }

            // Indicate whether or not the graph was reducible
            if (proc.DerivedGraphs[proc.DerivedGraphs.Count - 1].Count == 1)
                Console.Out.WriteLine("The graph is reducible.");
            else
                Console.Out.WriteLine("The graph is not reducible.");

        }




        // Give each node its DFS generated orderings.
        public void SetTimeStamps(ProcedureStructure curProc)
        {
            //do the time stamping used for loop structuring 
            int time = 1;
            List<StructureNode> order = curProc.Ordering;

            // set the parenthesis for the nodes as well as setting
            // the post-order ordering between the nodes
            curProc.EntryNode.SetLoopStamps(ref time, order);

            // set the reverse parenthesis for the nodes
            time = 1;
            curProc.EntryNode.SetRevLoopStamps(ref time);

            // do the ordering of nodes within the reverse graph 
            List<StructureNode> rorder = curProc.ReverseOrdering;
            Debug.Assert(curProc.ExitNode != null);
            curProc.ExitNode.SetRevOrder(rorder);
        }
    }

}
