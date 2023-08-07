#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Reko.Core;
using Reko.Core.Output;
using Reko.Gui;
using Reko.UserInterfaces.WindowsForms.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System;
using Microsoft.Msagl.GraphViewerGdi;
using C2 = Microsoft.Msagl.Drawing.Color;
using Reko.Gui.Services;
using Reko.Gui.TextViewing;

namespace Reko.UserInterfaces.WindowsForms
{
    public class CfgGraphGenerator
    {
        private readonly Graph graph;
        private readonly HashSet<Block> visited;
        private readonly Graphics g;
        private readonly Font defaultFont;
        private readonly IUiPreferencesService uiPreferences;
        private readonly C2 fillColor = new C2(0xFF, 0xE0, 0xE0);

        public CfgGraphGenerator(Graph graph, IUiPreferencesService uiPreferences, Graphics g, Font defaultFont)
        {
            this.uiPreferences = uiPreferences;
            this.graph = graph;
            this.g = g;
            this.defaultFont = defaultFont;
            this.visited = new HashSet<Block>();
        }

        public static Graph Generate(IUiPreferencesService uiPreferences, Procedure proc, Graphics g, Font defaultFont)
        {
            Graph graph = new Graph();
            var cfgGen = new CfgGraphGenerator(graph, uiPreferences, g, defaultFont);
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
                Debug.Print("Node {0}", b.DisplayName);
                visited.Add(b);
                CreateGraphNode(b);
                foreach (var pred in b.Pred.Where(p => p != block.Procedure.EntryBlock))
                {
                    Debug.Print("Edge {0} - {1}", pred.DisplayName, b.DisplayName);
                    graph.AddEdge(pred.DisplayName, b.DisplayName);
                }
                foreach (var succ in b.Succ)
                {
                    q.Enqueue(succ);
                }
            }
        }

        private bool useTextEngine = false;

        private Node CreateGraphNode(Block b)
        {
            var nl = "\n    ";
            var model = GenerateTextModel(b);
            var stack = new StyleStack(uiPreferences);
            var layout = TextViewLayout.AllLines(model, g, defaultFont, stack);
            var blockNode = new CfgBlockNode {
                Block = b,
                TextModel = GenerateTextModel(b),
                 Layout = layout,
                 UiPreferences = uiPreferences,
            };
            var node = graph.AddNode(b.DisplayName);
            node.Attr.LabelMargin = 5;
            node.UserData = blockNode;
            if (useTextEngine)
            {
                node.Attr.Shape = Shape.DrawFromGeometry;
                node.DrawNodeDelegate = blockNode.DrawNode;
                node.NodeBoundaryDelegate = blockNode.GetNodeBoundary;
            }
            else
            {
                node.Attr.FillColor = new C2(0xFF, 0xE0, 0xE0);
                node.Label.FontName = "Lucida Console";
                node.Label.FontSize = 10f;
                node.Attr.FillColor = fillColor;
                node.LabelText =
                    b.DisplayName + nl +
                    string.Join(nl, b.Statements.Select(s => s.Instruction));
            }
            return node;
        }

        private ITextViewModel GenerateTextModel(Block b)
        {
            var tsf = new TextSpanFormatter();
            var fmt = new AbsynCodeFormatter(tsf);
            var procf = new ProcedureFormatter(b.Procedure, fmt);
            fmt.InnerFormatter.UseTabs = false;
            procf.WriteBlock(b, fmt);
            return tsf.GetModel();
        }
    }
}
