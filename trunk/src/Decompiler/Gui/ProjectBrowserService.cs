#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Decompiler.Gui
{
    public class ProjectBrowserService : IProjectBrowserService, ITreeNodeDesignerHost
    {
        private ITreeView tree;
        private Dictionary<object, TreeNodeDesigner> mpitemToDesigner;

        public ProjectBrowserService(IServiceProvider services, ITreeView treeView)
        {
            this.Services = services;
            this.tree = treeView;
        }

        public IServiceProvider Services { get; private set; }

        public void Load(Project project)
        {
            this.mpitemToDesigner = new Dictionary<object, TreeNodeDesigner>();
            if (project == null)
            {
                tree.ShowRootLines = false;
                tree.ShowNodeToolTips = false;
                tree.Nodes.Clear();
                tree.Nodes.Add(tree.CreateNode("(No project loaded)"));
                return;
            }
            AddComponents(project.InputFiles);
            tree.ShowNodeToolTips = true;
        }

        public void AddComponents(IEnumerable components)
        {
            List<ITreeNode> nodes = new List<ITreeNode>();
            foreach (object o in components)
            {
                if (o == null)
                    continue;
                var attr = o.GetType().GetCustomAttributes(typeof(DesignerAttribute), true);
                if (attr.Length == 0)
                    continue;   //$Consider a simple designer where the text is component.ToString()?
                var desType = Type.GetType(
                    ((DesignerAttribute) attr[0]).DesignerTypeName,
                    true);
                var des = (TreeNodeDesigner) Activator.CreateInstance(desType);
                var node = tree.CreateNode();
                node.Tag = o;
                des.Services = Services;
                des.Host = this;
                des.TreeNode = node;
                des.Initialize(o);
                nodes.Add(node);
            }
            tree.Nodes.AddRange(nodes);
        }

        public void AddComponents(object parent, IEnumerable components)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent(object component)
        {
            throw new NotImplementedException();
        }
    }
}
