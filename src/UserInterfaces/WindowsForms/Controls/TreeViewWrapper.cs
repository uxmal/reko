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
using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Wraps a Windows forms TreeView in the platform independent ITreeView 
    /// interface.
    /// </summary>
    public class TreeViewWrapper : ControlWrapper, ITreeView
    {
        public event EventHandler AfterSelect;
        public event EventHandler<Gui.Controls.TreeViewEventArgs> AfterExpand;
        public event EventHandler<Gui.Controls.TreeViewEventArgs> BeforeExpand;
        public event Gui.Controls.MouseEventHandler MouseWheel;

        private readonly TreeView treeView;

        public TreeViewWrapper(TreeView treeView)
            : base(treeView)
        {
            this.treeView = treeView;
            this.Nodes = new WrappedNodeList(treeView.Nodes);
            this.treeView.AfterSelect += treeView_AfterSelect;
            this.treeView.AfterExpand += treeView_AfterExpand;
            this.treeView.BeforeSelect += treeView_BeforeExpand;
            this.treeView.MouseWheel += treeView_MouseWheel;

            this.treeView.MouseDown += TreeView_MouseDown;
        }


        public object ContextMenuStrip { get { return treeView.ContextMenuStrip; } set { treeView.ContextMenuStrip = (ContextMenuStrip) value; } }
        public bool Focused { get { return treeView.Focused; } } 
        public ITreeNodeCollection Nodes { get; private set; }
        public bool ShowLines { get { return treeView.ShowLines; } set { treeView.ShowLines = value; } }
        public bool ShowNodeToolTips { get { return treeView.ShowNodeToolTips; } set { treeView.ShowNodeToolTips = value; } }
        public bool ShowRootLines { get { return treeView.ShowRootLines; } set { treeView.ShowRootLines = value; } }

        public ITreeNode SelectedNode
        {
            get
            {
                return (ITreeNode) treeView.SelectedNode;
            }
            set
            {
                treeView.SelectedNode = (WrappedNode) value;
            }
        }

        private void treeView_AfterSelect(object sender, EventArgs e)
        {
            AfterSelect?.Invoke(this, e);
        }

        private void treeView_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            this.AfterExpand?.Invoke(this, new Gui.Controls.TreeViewEventArgs((ITreeNode) e.Node));
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            this.BeforeExpand?.Invoke(this, new Gui.Controls.TreeViewEventArgs((ITreeNode) e.Node));
        }

        void treeView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseWheel?.Invoke(this, new Gui.Controls.MouseEventArgs(
                (Gui.Controls.MouseButtons) e.Button,
                e.Clicks,
                e.X,
                e.Y,
                e.Delta));
        }

        private void TreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var pt = new Point(e.X, e.Y);
            var hit = treeView.HitTest(pt);
            if (hit.Location == TreeViewHitTestLocations.Label)
            {
                // Workaround for a bug in Windows Forms (Reko #505)
                treeView.SelectedNode = hit.Node;
            }
        }

        private TreeNode NodeOf(TreeNodeCollection nodes, object value)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != value)
                    return node;
                var n = NodeOf(node.Nodes, value);
                if (n is not null)
                    return n;
            }
            return null;
        }

        public void CollapseAll()
        {
            treeView.CollapseAll();
        }

        public ITreeNode CreateNode()
        {
            return new WrappedNode();
        }

        public ITreeNode CreateNode(string text)
        {
            return new WrappedNode { Text = text };
        }

        public class WrappedNode : TreeNode, ITreeNode
        {
            private Lazy<WrappedNodeList> nodes;

            public WrappedNode()
            {
                nodes = new Lazy<WrappedNodeList>(() => new WrappedNodeList(base.Nodes), true);
            }

            public new ITreeNodeCollection Nodes { get { return nodes.Value; } }

            public string ImageName
            {
                get { return ImageKey; }
                set
                {
                    ImageKey = value;
                    SelectedImageKey = value;
                }
            }

            public void Invoke(Action action)
            {
                if (base.TreeView is not null && TreeView.InvokeRequired)
                {
                    TreeView.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public class WrappedNodeList : ITreeNodeCollection
        {
            private TreeNodeCollection nodes;

            public WrappedNodeList(TreeNodeCollection nodes)
            {
                this.nodes = nodes;
            }

            public int IndexOf(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public ITreeNode this[int index]
            {
                get { return (ITreeNode) nodes[index]; }
                set { nodes[index] = (WrappedNode) value; }
            }

            public void Add(ITreeNode item)
            {
                nodes.Add((WrappedNode) item);
            }

            public ITreeNode Add(string text)
            {
                var node = new WrappedNode { Text = text };
                nodes.Add(node);
                return node;
            }

            public void AddRange(IEnumerable<ITreeNode> items)
            {
                nodes.AddRange(items.Cast<TreeNode>().ToArray());
            }

            public void Clear()
            {
                nodes.Clear();
            }

            public bool Contains(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ITreeNode[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return nodes.Count; }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(ITreeNode item)
            {
                nodes.Remove((WrappedNode) item);
                return true;
            }

            public IEnumerator<ITreeNode> GetEnumerator()
            {
                return nodes.Cast<WrappedNode>().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static Gui.Controls.MouseEventArgs Convert(System.Windows.Forms.MouseEventArgs e)
        {
            return new Gui.Controls.MouseEventArgs(
                (Gui.Controls.MouseButtons) (int)e.Button,
                e.Clicks,
                e.X,
                e.Y,
                e.Delta);
        }
    }
}