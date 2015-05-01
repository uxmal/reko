using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContextMenu = System.Windows.Forms.ContextMenu;
using DragEventHandler = System.Windows.Forms.DragEventHandler;
using MouseEventHandler = System.Windows.Forms.MouseEventHandler;

namespace Decompiler.Gui.Controls
{
    public interface ITreeView
    {
        event EventHandler AfterSelect;
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
        ContextMenu ContextMenu { get; set; }

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

        void Expand();
    }
}
