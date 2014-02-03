
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

using Decompiler.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Wraps a Windows forms TreeView in the platform independent ITreeView interface.
    /// </summary>
    public class TreeViewWrapper : ITreeView
    {
        private TreeView treeView;

        public TreeViewWrapper(TreeView treeView)
        {
            this.treeView = treeView;
        }

        public object SelectedItem
        {
            get
            {
                if (treeView.SelectedNode != null)
                    return treeView.SelectedNode.Tag;
                return null;
            }
            set
            {
                if (value != null)
                {
                    var node = NodeOf(treeView.Nodes, value);
                    if (node != null)
                        treeView.SelectedNode = node;
                }
                else 
                    treeView.SelectedNode = null;
            }
        }

        private TreeNode NodeOf(TreeNodeCollection nodes, object value)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != value)
                    return node;
                var n = NodeOf(node.Nodes, value);
                if (n != null)
                    return n;
            }
            return null;
        }

    }
}
