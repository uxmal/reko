#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Microsoft.Msagl.Drawing;
using Reko.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Gui.Windows
{
    public class CfgGraphGenerator
    {
        private Graph graph;
        private HashSet<Block> visited;

        public CfgGraphGenerator(Graph graph)
        {
            this.graph = graph;
            this.visited = new HashSet<Block>();
        }

        public static Graph Generate(Procedure proc)
        {
            Graph graph = new Graph();
            var cfgGen = new CfgGraphGenerator(graph);
            cfgGen.Traverse(proc.EntryBlock.Succ[0]);
            graph.Attr.LayerDirection = LayerDirection.TB;
            return graph;
        }

        public void Traverse(Block block)
        {
            var q = new Queue<Block>();
            q.Enqueue(block);
            while (q.Count > 0)
            {
                var b = q.Dequeue();
                if (visited.Contains(b))
                    continue;
                visited.Add(b);
                Debug.Print("Node {0}", b.Name);
                visited.Add(b);
                var n = Render(b);
                foreach (var pred in b.Pred.Where(p => p != block.Procedure.EntryBlock))
                {
                    Debug.Print("Edge {0} - {1}", pred.Name, b.Name);
                    graph.AddEdge(pred.Name, b.Name);
                }
                foreach (var succ in b.Succ)
                {
                    q.Enqueue(succ);
                }
            }
        }

        private Node Render(Block b)
        {
            var nl = "\n    ";
            var node = graph.AddNode(b.Name);
            node.Label.FontName = "Lucida Console";
            node.Label.FontSize = 10f;
            node.Attr.LabelMargin = 5;
            node.LabelText =
                b.Name + nl +
                string.Join(nl, b.Statements.Select(s => s.Instruction));
            return node;
        }
    }
}
