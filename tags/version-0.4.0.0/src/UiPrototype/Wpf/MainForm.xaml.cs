using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Decompiler.UiPrototype.Wpf
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainForm : UserControl
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void treeView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void treeView1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public class Node
        {
            public string Text { get; set; }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var nodes = new Node[] {
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
            treeView1.Items.Clear();
            treeView1.ItemsSource = nodes;
        }


        private Node BinaryNode(string text, params Node[] nodes)
        {
            var node = new Node 
            {
                Text = text,
                //ImageKey = "Binary.ico",
                //SelectedImageKey = "Binary.ico"
            };
            //node.Items.AddRange(nodes);
            //node.ExpandAll();
            return node;
        }

        private Node FormatNode(string text, params Node[] nodes)
        {
            var node = new Node
            {
                Text = text,
                //ImageKey = "Header.ico",
                //SelectedImageKey = "Header.ico"
            };
            //node.Nodes.AddRange(nodes);
            return node;
        }

        private Node DataSegmentNode(string text, params Node[] nodes)
        {
            var node = new Node
            {
                Text = text,
                //ImageKey = "Data.ico",
                //SelectedImageKey = "Data.ico"
            };
            //node.Nodes.AddRange(nodes);
            return node;
        }

        private Node CodeSegmentNode(string text, params Node[] nodes)
        {
            var node = new Node
            {
                Text = text,
                //ImageKey = "Code.ico",
                //SelectedImageKey = "Code.ico"
            };
            //node.Nodes.AddRange(nodes);
            return node;
        }

        private Node FnNode(string text, params Node[] nodes)
        {
            var node = new Node
            {
                Text = text,
                //ImageKey = "Code.ico",
                //SelectedImageKey = "Code.ico"
            };
            //node.Nodes.AddRange(nodes);
            return node;
        }
    }
}
