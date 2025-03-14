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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using C2 = Microsoft.Msagl.Drawing.Color;

namespace Reko.UserInterfaces.WindowsForms
{
    using Microsoft.Msagl.Drawing;
    using Reko.Core.Graphs;

    public class CallGraphGenerator
    {
        private readonly Graph graph;
        private readonly HashSet<Procedure> visited;
        private readonly C2 fillColor = new C2(0xFF, 0xE0, 0xE0);

        public CallGraphGenerator(Graph graph)
        {
            this.graph = graph;
            this.visited = new HashSet<Procedure>();
        }

        public static Graph Generate(Program program)
        {
            Graph graph = new Graph();
            var cfgGen = new CallGraphGenerator(graph);
            foreach (var rootProc in program.Procedures.Values)
            {
                cfgGen.Traverse(program.CallGraph, rootProc);
            }
            graph.Attr.LayerDirection = LayerDirection.LR;
            return graph;
        }

        public void Traverse(CallGraph cgraph, Procedure rootProc)
        {
            var q = new Queue<Procedure>();
            q.Enqueue(rootProc);
            while (q.TryDequeue(out var proc))
            {
                if (visited.Contains(proc))
                    continue;
                visited.Add(proc);
                Debug.Print("Node {0}", proc.Name);
                visited.Add(proc);
                Render(proc);
                foreach (var pred in cgraph.CallerProcedures(proc).Where(p => p != rootProc))
                {
                    Debug.Print("Edge {0} - {1}", pred.Name, proc.Name);
                    graph.AddEdge(pred.Name, proc.Name);
                }
                foreach (var succ in cgraph.Callees(proc))
                {
                    q.Enqueue(succ);
                }
            }
        }

        private Node Render(Procedure proc)
        {
            var node = graph.AddNode(proc.Name);
            node.Label.FontName = "Lucida Console";
            node.Attr.FillColor = fillColor;
            node.Label.FontSize = 10f;
            node.Attr.LabelMargin = 5;
            node.LabelText = proc.Name;
            return node;
        }
    }
}
