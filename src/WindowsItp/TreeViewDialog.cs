using Reko.Gui;
using Reko.Gui.Controls;
using Reko.UserInterfaces.WindowsForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class TreeViewDialog : Form
    {
        private TreeViewWrapper wrappedTree; 

        public TreeViewDialog()
        {
            InitializeComponent();
            wrappedTree = new TreeViewWrapper(this.treeView);
            ClearTree();
            EnableControls();
        }

        void ClearTree()
        {
            wrappedTree.Nodes.Clear();
            wrappedTree.ShowLines = false;
            wrappedTree.Nodes.Add("(No items)");
            lblResult.Text = "";
        }

        void EnableControls()
        {
            int n;
            if (txtItems.TextLength == 0 ||
                !int.TryParse(txtItems.Text, out n) ||
                n <= 0)
            {
                btnAllAtOnce.Enabled = false;
                btnOneByOne.Enabled = false;
            }
            else
            {
                btnAllAtOnce.Enabled = true;
                btnOneByOne.Enabled = true;
            }
        }

        private void AddAllAtOnce()
        {
            var nodes = CreateNodes();
            AddAllAtOnce(nodes);
        }

        private void AddAllAtOnce(ITreeNode[] nodes)
        {
            wrappedTree.Nodes.Clear();
            wrappedTree.Nodes.AddRange(nodes);
        }

        private void AddOneByOne()
        {
            var nodes = CreateNodes();
            wrappedTree.Nodes.Clear();
            for (int i = 0; i < nodes.Length; ++i)
            {
                wrappedTree.Nodes.Add(nodes[i]);
            }
        }

        [Designer(typeof(BrowseObjectDesigner))]
        public class BrowseObject
        {
            public int Index { get; set; }
        }

        public class BrowseObjectDesigner : TreeNodeDesigner
        {
            public override void Initialize(object obj)
            {
                var bo = (BrowseObject) obj;
                TreeNode.Text = "Node #" + bo.Index;
                TreeNode.ToolTipText = "This\nIs the text for\nNode #" + bo.Index;
                TreeNode.Tag = bo;
            }
        }

        private ITreeNode[] CreateNodesViaDesigner()
        {
            int n;
            if (!int.TryParse(txtItems.Text, out n) ||
                n <= 0)
                return Array.Empty<ITreeNode>();
            var objs = Enumerable.Range(0, n)
                .Select(i => new BrowseObject { Index = i });
            var nodes = objs.Select(o => new
                {
                    o,
                    dd = o.GetType()
                        .GetCustomAttributes(typeof(DesignerAttribute), true)
                })
                .Where(o => o.dd is not null && o.dd.Length > 0)
                .Select(o =>
                {
                    var tyName = ((DesignerAttribute) o.dd.First()).DesignerTypeName;
                    //throw new NotImplementedException("Services need to be introduced into WindowsItp");
                    //var des = Type.GetType(tyName, true).CreateInstance<TreeNodeDesigner>();
                    var node = wrappedTree.CreateNode();
                    //des.TreeNode = node;
                    //des.Initialize(o.o);
                    return node;
                })
                .ToArray();
            return nodes;
        }

        private ITreeNode[] CreateNodes()
        {
            int n;
            if (!int.TryParse(txtItems.Text, out n) ||
                n <= 0)
                return Array.Empty<ITreeNode>();
            return Enumerable.Range(0, n)
                            .Select(i => {
                                var node = wrappedTree.CreateNode();
                                node.Text = "Node #" + i;
                                node.ToolTipText = "This\nIs the text for\nNode #" + i;
                                node.Tag = new object[300];
                                return node;
                            })
                            .ToArray();
        }

        private void DisplayElapsedTime(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            lblResult.Text = string.Format("Elapsed time: {0}", sw.Elapsed);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTree();
        }

        private void txtItems_TextChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void btnOneByOne_Click(object sender, EventArgs e)
        {
            DisplayElapsedTime(AddOneByOne);
        }

        private void btnAllAtOnce_Click(object sender, EventArgs e)
        {
            DisplayElapsedTime(AddAllAtOnce);
        }

        private void btnWithDesigner_Click(object sender, EventArgs e)
        {
            DisplayElapsedTime(() => AddAllAtOnce(CreateNodesViaDesigner()));
        }
    }

    public static class Extomatic
    {
        public static T  CreateInstance<T>(this Type t, params object[] args)
        {
            return (T)Activator.CreateInstance(t, args);
        }
    }
}
