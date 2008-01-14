/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Lib;
using NUnit.Framework;
using System;
using System.Text;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class DfsIteratorTests
	{
		[Test]
		public void SingleNode()
		{
			DirectedGraph graph = new DirectedGraph();
			graph.AddNode(1);

			DfsIterator it = new DfsIterator(graph, 1);
			Assert.IsTrue(it.MoveNext());
			Assert.AreEqual(1, it.Current);
			Assert.IsFalse(it.MoveNext());

			Assert.AreEqual("1 ", Traverse(graph, 1));
		}

		[Test]
		public void DfsitFork()
		{
			DirectedGraph graph = new DirectedGraph();
			graph.AddNode(1);
			graph.AddNode(2);
			graph.AddNode(3);
			graph.AddEdge(1, 2);
			graph.AddEdge(1, 3);

			DfsIterator it = new DfsIterator(graph, 1);
			Assert.IsTrue(it.MoveNext());
			Assert.AreEqual(1, it.Current);
			Assert.IsTrue(it.MoveNext());
			Assert.AreEqual(2, it.Current);
			Assert.IsTrue(it.MoveNext());
			Assert.AreEqual(3, it.Current);
			Assert.IsFalse(it.MoveNext());

			Assert.AreEqual("1 2 3 ", Traverse(graph, 1));
		}

		[Test]
		public void DfsitDiamond()
		{
			DirectedGraph graph = new DirectedGraph();
			graph.AddNode(1);
			graph.AddNode(2);
			graph.AddNode(3);
			graph.AddNode(4);
			graph.AddEdge(1, 2);
			graph.AddEdge(1, 3);
			graph.AddEdge(2, 4);
			graph.AddEdge(3, 4);

			Assert.AreEqual("1 2 4 3 ", Traverse(graph, 1));

		}

		[Test]
		public void Linear()
		{
			DirectedGraph graph = new DirectedGraph();
			graph.AddNode(1);
			graph.AddNode(2);
			graph.AddNode(3);
			graph.AddNode(4);

			graph.AddEdge(1, 2);
			graph.AddEdge(2, 3);
			graph.AddEdge(3, 4);

			Assert.AreEqual("1 2 3 4 ", Traverse(graph, 1));
		
		}

		private string Traverse(DirectedGraph graph, object start)
		{
			StringBuilder sb = new StringBuilder();
			foreach (int n in new DfsIterator(graph, start))
			{
				sb.AppendFormat("{0} ", n);
			}
			return sb.ToString();
		}
	}
}
