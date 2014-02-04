using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Controls
{
    public interface ITreeView
    {
        object SelectedItem { get; set; }
        bool ShowNodeToolTips { get; set; }
        bool ShowRootLines { get; set; }

        ITreeNodeCollection Nodes { get; }

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
        IList<ITreeNode> Nodes { get; }
        object Tag { get; set; }
        string Text { get; set; }
        string ToolTipText { get; set; }
    }
}
