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
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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

    public delegate void DragEventHandler(object sender, DragEventArgs e);
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public class DragEventArgs : EventArgs
    {
        public DragEventArgs(object data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            this.Data = data;
            this.KeyState = keyState;
            this.X = x;
            this.Y = y;
            this.AllowedEffect = allowedEffect;
            this.Effect = effect;
        }

        public object Data { get; }
        public int KeyState { get; }
        public int X { get; }
        public int Y { get; }
        public DragDropEffects AllowedEffect { get; }
        public DragDropEffects Effect { get; set; }
    }

    [Flags]
    public enum DragDropEffects
    {
        Scroll = int.MinValue,
        All = -2147483645,
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4
    }

    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            this.Button = button;
            this.Clicks = clicks;
            this.X = x;
            this.Y = y;
            this.Delta = delta;
        }

        public MouseButtons Button { get; }
        public int Clicks { get; }
        public int X { get; }
        public int Y { get; }
        public int Delta { get; }
    }

    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216
    }
}
