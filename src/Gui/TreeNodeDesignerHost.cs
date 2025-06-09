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
using Reko.Core.Services;
using Reko.Gui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    public class TreeNodeDesignerHost : ITreeNodeDesignerHost
    {
        private readonly ITreeView tree;
        private Dictionary<object, TreeNodeDesigner> mpitemToDesigner;

        public TreeNodeDesignerHost(ITreeView tree, IServiceProvider services)
        {
            this.tree = tree;
            this.tree.AfterSelect += tree_AfterSelect;
            //this.tree.AfterExpand += tree_AfterExpand;
            this.tree.BeforeExpand += tree_BeforeExpand;
            this.Services = services;
            this.mpitemToDesigner = new Dictionary<object, TreeNodeDesigner>();
        }

        public object? SelectedObject
        {
            get { return GetSelectedObject(); }
            set { SetSelectedObject(value); }
        }

        public IServiceProvider Services { get; }

        public ITreeView TreeView => tree;

        public void AddComponents(IEnumerable? components)
        {
            if (components is null)
                return;
            var nodes = components
                .Cast<object>()
                .Select(o => CreateTreeNode(o, CreateDesigner(o)!, null));
            tree.Nodes.AddRange(nodes);
        }

        public void AddComponent(object? parent, object component)
        {
            AddComponents(parent, new[] { component });
        }

        public void AddComponents(object? parent, IEnumerable components)
        {
            TreeNodeDesigner? parentDes = GetDesigner(parent);
            if (parentDes is null)
            {
                Debug.Print("No designer for parent object {0}", parent ?? "(null)");
                AddComponents(components);
                return;
            }
            var nodes = components
                .Cast<object>()
                .Select(o => CreateTreeNode(o, CreateDesigner(o)!, parentDes));
            parentDes.TreeNode!.Nodes.AddRange(nodes);
        }

        public virtual void Clear()
        {
            tree.Nodes.Clear();
            this.mpitemToDesigner = new Dictionary<object, TreeNodeDesigner>();
        }

        public TreeNodeDesigner? GetDesigner(object? o)
        {
            if (o is null)
                return null;
            if (mpitemToDesigner.TryGetValue(o, out TreeNodeDesigner? des))
                return des;
            else
                return o as TreeNodeDesigner;
        }


        public void RemoveComponent(object component)
        {
            var des = GetDesigner(component);
            if (des is null)
                return;
            mpitemToDesigner.Remove(des.Component!);
            des.TreeNode!.Remove();
        }

        private TreeNodeDesigner? CreateDesigner(object? o)
        {
            if (o is null)
                return null;
            if (o is TreeNodeDesigner des)
            {
                if (des.Component is not null)
                {
                    o = des.Component;
                }
            }
            else
            {
                var attr = o.GetType().GetCustomAttributes(typeof(DesignerAttribute), true);
                if (attr.Length > 0)
                {
                    var svc = Services.RequireService<IPluginLoaderService>();
                    try
                    {
                        var desType = svc.GetType(
                            ((DesignerAttribute) attr[0]).DesignerTypeName);
                        if (desType is not null)
                            des = (TreeNodeDesigner) Activator.CreateInstance(desType)!;
                        else
                            des = new TreeNodeDesigner();
                    }
                    catch
                    {
                        des = new TreeNodeDesigner();
                    }
                }
                else
                {
                    des = new TreeNodeDesigner();
                }
            }
            if (des is not null)
            {
                mpitemToDesigner[o] = des;
            }
            return des;
        }

        private ITreeNode CreateTreeNode(object o, TreeNodeDesigner des, TreeNodeDesigner? parentDes)
        {
            var node = tree.CreateNode();
            node.Tag = des;
            node.Expand();
            des.Services = Services;
            des.Host = this;
            des.TreeNode = node;
            des.Component = o;
            des.Parent = parentDes;
            des.Initialize(o);

            return node;
        }

        private void tree_AfterSelect(object? sender, EventArgs e)
        {
            var des = GetSelectedDesigner();
            if (des is not null)
            {
                des.DoDefaultAction();
            }
        }

        private void tree_BeforeExpand(object? sender, TreeViewEventArgs e)
        {
            var des = (TreeNodeDesigner?) e.Node.Tag;
            des?.OnExpanded();
        }

        private void tree_AfterExpand(object? sender, TreeViewEventArgs e)
        {
            var des = (TreeNodeDesigner?) e.Node.Tag;
            des?.OnExpanded();
        }

        public TreeNodeDesigner? GetSelectedDesigner()
        {
            if (tree.SelectedNode is null)
                return null;
            return (TreeNodeDesigner?) tree.SelectedNode.Tag;
        }

        private object? GetSelectedObject()
        {
            var des = GetSelectedDesigner();
            if (des is null)
                return null;
            return des.Component;
        }

        private void SetSelectedObject(object? component)
        {
            if (component is null)
                return;
            if (!mpitemToDesigner.TryGetValue(component, out TreeNodeDesigner? des))
                return;
            tree.SelectedNode = des.TreeNode;
        }
    }
}
