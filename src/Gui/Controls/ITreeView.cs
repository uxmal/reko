#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using DragEventHandler = System.Windows.Forms.DragEventHandler;
using MouseEventHandler = System.Windows.Forms.MouseEventHandler;

namespace Reko.Gui.Controls
{
    public interface ITreeView : IControl
    {
        event EventHandler AfterSelect;
        event EventHandler<TreeViewEventArgs> AfterExpand;
        event EventHandler<TreeViewEventArgs> BeforeExpand;

        event DragEventHandler DragEnter;
        event DragEventHandler DragOver;
        event DragEventHandler DragDrop;
        event EventHandler DragLeave;
        event MouseEventHandler MouseWheel;

        bool Focused { get; }
        ITreeNode SelectedNode { get; set; }
        bool ShowNodeToolTips { get; set; }
        bool ShowRootLines { get; set; }

        ITreeNodeCollection Nodes { get; }
        object ContextMenu { get; set; }

        void CollapseAll();
        ITreeNode CreateNode();
        ITreeNode CreateNode(string text);
    }

    public interface ITreeNodeCollection : IList<ITreeNode>
    {
        ITreeNode Add(string text);
        void AddRange(IEnumerable<ITreeNode> nodes);
    }

    public interface ITreeNode
    {
        ITreeNodeCollection Nodes { get; }
        string ImageName { get; set; }
        object Tag { get; set; }
        string Text { get; set; }
        string ToolTipText { get; set; }

        void Collapse();
        void Expand();
        void Invoke(Action action);
        void Remove();
    }
}
