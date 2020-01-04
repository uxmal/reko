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

using Reko.Core.Lib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Core.Lib
{
	[TestFixture]
	public class DfsIteratorTests
	{
        StringBuilder sb = new StringBuilder();

        [SetUp]
        public void Setup()
        {
            sb = new StringBuilder();
        }

        private DfsIterator<TreeNode> CreateTreeNodeIterator()
        {
            return new DfsIterator<TreeNode>(new TreeNodeGraph());
        }

        private DfsIterator<T> CreateGraphIterator<T>(DirectedGraph<T> graph) where T : class
        {
            return new DfsIterator<T>(graph);
        }


        [Test]
        public void Dfsi_Create()
        {
            TreeNode tree = new TreeNode("a");
            DumpPreOrderIterator(tree, CreateTreeNodeIterator());
            Assert.AreEqual("a", sb.ToString());
        }

        [Test]
        public void PreorderTwoLevelTree()
        {
            TreeNode tree = new TreeNode("a",
                new TreeNode("b"));
            DumpPreOrderIterator(tree, CreateTreeNodeIterator());
            Assert.AreEqual("a,b", sb.ToString());

        }

        [Test]
        public void PostorderTwoLevelTree()
        {
            TreeNode tree = new TreeNode("a",
                new TreeNode("b"),
                new TreeNode("c"));
            DumpPostOrderIterator(tree, CreateTreeNodeIterator());
            Assert.AreEqual("b,c,a", sb.ToString());
        }

        [Test]
        public void PreorderGraph()
        {
            DirectedGraphImpl<string> graph = new DirectedGraphImpl<string>();
            graph.AddNode("a");
            graph.AddNode("b");
            graph.AddNode("c");
            graph.AddEdge("a", "b");
            graph.AddEdge("b", "c");
            graph.AddEdge("c", "b");
            DumpPreOrderIterator("a", CreateGraphIterator<string>(graph));
            Assert.AreEqual("a,b,c", sb.ToString());
        }

        [Test]
        public void PostOrderGraph()
        {
            DirectedGraphImpl<string> graph = new DirectedGraphImpl<string>();
            graph.AddNode("a");
            graph.AddNode("b");
            graph.AddNode("c");
            graph.AddEdge("a", "b");
            graph.AddEdge("b", "c");
            graph.AddEdge("c", "b");
            DumpPostOrderIterator("a", CreateGraphIterator<string>(graph));
            Assert.AreEqual("c,b,a", sb.ToString());
        }

        [Test]
        public void PostOrderGraph2()
        {
            DirectedGraphImpl<string> graph = new DirectedGraphImpl<string>();
            graph.AddNode("a");
            graph.AddNode("b");
            graph.AddEdge("a", "a");
            graph.AddEdge("a", "b");
            DumpPostOrderIterator("a", CreateGraphIterator<string>(graph));
            Assert.AreEqual("b,a", sb.ToString());
        }

        private class TreeNode
        {
            public string Str;
            public TreeNode[] Children;

            public TreeNode(string str, params TreeNode[] children)
            {
                this.Str = str;
                this.Children = children;
            }

            public override string ToString()
            {
                return Str;
            }
        }

        private class TreeNodeGraph : DirectedGraph<TreeNode>
        {
            #region DirectedGraph<TreeNode> Members

            public ICollection<TreeNode> Predecessors(TreeNode node)
            {
                throw new NotImplementedException();
            }

            public ICollection<TreeNode> Successors(TreeNode node)
            {
                return node.Children;
            }

            public ICollection<TreeNode> Nodes
            {
                get { throw new NotImplementedException(); }
            }

            public void AddEdge(TreeNode nodeFrom, TreeNode nodeTo)
            {
                throw new NotImplementedException();
            }

            public void RemoveEdge(TreeNode nodeFrom, TreeNode nodeTo)
            {
                throw new NotImplementedException();
            }

            public bool ContainsEdge(TreeNode nodeFrom, TreeNode nodeTo)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private void DumpPreOrderIterator<T>(T tree, DfsIterator<T> iterator)
        {
            string sep = "";
            foreach (T node in iterator.PreOrder(tree))
            {
                sb.Append(sep);
                sep = ",";
                sb.Append(node.ToString());
            }
        }

        private void DumpPostOrderIterator<T>(T tree, DfsIterator<T> iterator)
        {
            string sep = "";
            foreach (T node in iterator.PostOrder(tree))
            {
                sb.Append(sep);
                sep = ",";
                sb.Append(node.ToString());
            }
        }

	}
}
