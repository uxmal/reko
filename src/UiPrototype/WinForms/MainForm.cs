using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UiPrototype.WinForms
{
    public partial class MainForm : Form
    {
        private IEnumerator<SearchHit> currentSearch;
        private System.Drawing.Font treeFont;
        private System.Drawing.Font userNodeFont;
        private Random random;

        public MainForm()
        {
            InitializeComponent();
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode is null)
                return;

            if (IsFnNode(treeView1.SelectedNode))
            {
                //var fv = new ProcedureView();
                //fv.EditorView.Model = fv.Model(
                //    fv.Line(
                //        fv.Span("// fn00112233", "cmt")),
                //    fv.Line(
                //        fv.Span("// Trashes - edx", "cmt")),
                //    fv.Line(
                //        fv.Span("void fn00112233(esi)")),
                //    fv.Line(
                //        fv.Span("{")),
                //    fv.Line(
                //        fv.Span("    "),
                //        fv.Span("while", "keyword"),
                //        fv.Span(" (edx_10 != 0)")),
                //    fv.Line(
                //        fv.Span("    {")), 
                //    fv.Line(
                //        fv.Span("        "),
                //        fv.Span("edx_10 = "),
                //        fv.Link("fn00114434"),
                //        fv.Span("(edx_10);")),
                //    fv.Line(
                //        fv.Span("    }")),
                //    fv.Line(
                //        fv.Span("}")));
                //fv.MdiParent = this;
                //fv.Show();
            }
            else
            {
                var mv = new MemoryView();
                mv.MdiParent = this;
                mv.Show();
            }
        }

        private bool IsFnNode(TreeNode treeNode)
        {
            return treeNode.ImageKey == "Code.ico";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.random = new Random((int)DateTime.Now.ToBinary());
            this.treeFont = treeView1.Font;
            this.userNodeFont = new Font(
                treeFont, FontStyle.Bold);

            var nodes = new TreeNode[] {
                BinaryNode("mainprog.exe",
                    FormatNode("PE image",
                        CodeSegmentNode(".text segment",
                            FnNode("fn00410000 (entry point)"),
                            FnNode("fn004100F0"),
                            FnNode("fn00410130"),
                            FnNode("fn00410270")),
                        DataSegmentNode(".data segment")),
                    FormatNode("MS-DOS stub image",
                        BinaryNode("0C00 - segment",
                            FnNode("fn0C00_0000"),
                            FnNode("fn0C00_0038")),
                        BinaryNode("0CF0 - segment",
                            FnNode("fn0CF0_0010"),
                            FnNode("fn0CF0_0284"),
                            FnNode("fn0CF0_02FC")))),
                BinaryNode("addin.dll",
                    FormatNode("PE image",
                        CodeSegmentNode(".text segment",
                            FnNode("fn00500000"))))
            };
            treeView1.ShowNodeToolTips = true;
            treeView1.Nodes.Clear();
            treeView1.Nodes.AddRange(nodes);
        }

        private TreeNode BinaryNode(string text, params TreeNode[] nodes)
        {
            var node = new TreeNode
            {
                Text = text,
                ImageKey = "Binary.ico",
                SelectedImageKey = "Binary.ico"
            };
            node.Nodes.AddRange(nodes);
            node.ExpandAll();
            return node;
        }

        private TreeNode FormatNode(string text, params TreeNode[] nodes)
        {
            var node = new TreeNode
            {
                Text = text,
                ImageKey = "Header.ico",
                SelectedImageKey = "Header.ico"
            };
            node.Nodes.AddRange(nodes);
            return node;
        }

        private TreeNode DataSegmentNode(string text, params TreeNode[] nodes)
        {
            var node = new TreeNode
            {
                Text = text,
                ImageKey = "Data.ico",
                SelectedImageKey = "Data.ico"
            };
            node.Nodes.AddRange(nodes);
            return node;
        }

        private TreeNode CodeSegmentNode(string text, params TreeNode[] nodes)
        {
            var node = new TreeNode
            {
                Text = text,
                ImageKey = "Code.ico",
                SelectedImageKey = "Code.ico"
            };
            node.Nodes.AddRange(nodes);
            return node;
        }

        private TreeNode FnNode(string text, params TreeNode[] nodes)
        {
            var nl = Environment.NewLine;
            bool userEdited = random.Next(3) == 0;
            var tt = new StringBuilder();
            tt.AppendLine(text);
            if (userEdited)
            {
                tt.AppendLine("User edited");
            }
            var node = new TreeNode
            {
                Text = text,
                ImageKey = "Code.ico",
                SelectedImageKey = "Code.ico",
                ToolTipText = tt.ToString(),
                NodeFont = userEdited ? userNodeFont : treeFont,
                ContextMenuStrip = ctxmProc,
            };
            node.Nodes.AddRange(nodes);
            return node;
        }

        private void stringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new PatternSearchDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    PopulateSearchResults(dlg.CreateStringSearcher());
                }
            }
        }

        private void PopulateSearchResults(StringSearcher stringSearcher)
        {
            currentSearch = stringSearcher.GetEnumerator();
            timerSearchResults.Start();
            listSearchResults.Items.Clear();
        }

        private void timerSearchResults_Tick(object sender, EventArgs e)
        {
            if (currentSearch is null)
            {
                timerSearchResults.Stop();
                return;
            }
            if (!currentSearch.MoveNext())
            {
                currentSearch.Dispose();
                currentSearch = null;
            }
            else
            {
                var hit = currentSearch.Current;
                var item = new ListViewItem
                {
                    Text = hit.AddressText,
                    SubItems = { hit.Name, hit.Description }
                };
                listSearchResults.Items.Add(item);
            }
        }

        private void uncannedBlocksOfCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentSearch = Enumerable.Range(0, 100)
                .Select(i => new SearchHit
                {
                    AddressText = (0x00410000 + i * 365).ToString("X8"),
                    Name = "",
                    Description = string.Format("{0:X8} - {1:X8}", (0x00410000 + i * 365), ((0x00410000 + i * 365) + 147)),
                })
                .GetEnumerator();
            timerSearchResults.Start();
            listSearchResults.Items.Clear();
        }
    }
}
