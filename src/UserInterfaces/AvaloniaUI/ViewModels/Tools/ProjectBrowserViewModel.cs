#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Dock.Model.ReactiveUI.Controls;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    public class ProjectBrowserViewModel : Tool
    {
        public ProjectBrowserViewModel()
        {
            this.ProjectBrowser = new List<Node>
            {
                new Node("Arch"),
                new Node("Dependencies",
                    new Node("foo.dll"),
                    new Node("bar.dll")),
                new Node("EntryPoints",
                    new Node("_start"),
                    new Node("WinMain")),
            };
        }
        public Project? Project { get; set; }

        public List<Node> ProjectBrowser { get; set; }
    }

    public class Node
    {
        public Node(string name, params Node[] nodes)
        {
            this.Text = name;
            this.Nodes = new(nodes);
        }
            
        public string? Text { get; set; }
        public List<Node> Nodes { get; set; }
    }

}
