using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
{
    public partial class TreeViewDialog : Form
    {
        public TreeViewDialog()
        {
            InitializeComponent();
            ClearTree();
            EnableControls();
        }

        void ClearTree()
        {
            treeView.Nodes.Clear();
            treeView.ShowLines = false;
            treeView.Nodes.Add("(No items)");
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
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(nodes);
        }

        private void AddOneByOne()
        {
            var nodes = CreateNodes();
            treeView.Nodes.Clear();
            for (int i = 0; i < nodes.Length; ++i)
            {
                treeView.Nodes.Add(nodes[i]);
            }
        }

        private TreeNode[] CreateNodes()
        {
            int n;
            if (!int.TryParse(txtItems.Text, out n) ||
                n <= 0)
                return new TreeNode[0];
            return Enumerable.Range(0, n)
                            .Select(i => new TreeNode
                            {
                                Text = "Node #" + i,
                                ToolTipText = "This\nIs the text for\nNode #" + i,
                                Tag = new object[300]
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
    }
}
