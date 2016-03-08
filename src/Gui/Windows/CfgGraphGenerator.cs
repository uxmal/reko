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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using System.Windows.Forms;
using System.ComponentModel.Design;
using Reko.Core;

namespace Reko.Gui.Windows
{
    public class CfgGraphGenerator : IWindowPane, ICommandTarget
    {
        private GViewer graphViewer;
        private IServiceProvider services;

        public Control CreateControl()
        {
            graphViewer = new GViewer() { Dock = DockStyle.Fill };
            Graph graph = new Graph();

            var nl = "\n";
            var a = graph.AddNode("A");
            a.Label.FontName = "Lucida Console";
            a.Label.FontSize = 10f;
            a.Attr.LabelMargin = 5;
            a.LabelText = "A:" + nl + "dec cx" + nl + "jnz A";
            var b = graph.AddNode("B");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "A");

            graph.AddEdge("B", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("D", "E");
            graph.AddEdge("C", "E");

            graph.Attr.LayerDirection = LayerDirection.TB;
            graphViewer.Graph = graph; // throws exception
            graphViewer.ToolBarIsVisible = false;

            graphViewer.KeyDown += GraphViewer_KeyDown;
            return graphViewer;
        }

        public void SetSite(IServiceProvider services)
        {
            this.services = services;
        }

        public void Close()
        {
            if (graphViewer != null)
                graphViewer.Dispose();
        }

            private void GraphViewer_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.A)
                {
                    graphViewer.ZoomF *= 1.1;
                    e.Handled = true;
                }

                if (e.KeyCode == Keys.Z)
                {
                    graphViewer.ZoomF /= 1.1;
                    e.Handled = true;
                }
            }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            return false;
        }

        public static Graph Generate(Procedure proc)
        {
                     Graph graph = new Graph();

            var nl = "\n";
            var a = graph.AddNode("A");
            a.Label.FontName = "Lucida Console";
            a.Label.FontSize = 10f;
            a.Attr.LabelMargin = 5;
            a.LabelText = "A:" + nl + "dec cx" + nl + "jnz A";
            var b = graph.AddNode("B");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "A");

            graph.AddEdge("B", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("D", "E");
            graph.AddEdge("C", "E");

            graph.Attr.LayerDirection = LayerDirection.TB;
            return graph;
        }
    }
}
