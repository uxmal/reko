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

namespace Decompiler.UnitTests.Core
{
	using StringBuilder = System.Text.StringBuilder;

	[TestFixture]
	public class DirectedGraphTests
	{
		public DirectedGraphTests()
		{
		}

		private DirectedGraph CreateAcyclicGraph()
		{
			DirectedGraph gr = new DirectedGraph();
			gr.AddNode(0);
			gr.AddNode(1);
			gr.AddNode(2);

			gr.AddEdge(0, 1);
			gr.AddEdge(0, 2);
			gr.AddEdge(1, 2);
			gr.AddEdge(2, 1);
		
			return gr;
		}

		private string DumpGraph(DirectedGraph gr)
		{
			StringBuilder sb = new StringBuilder();
			foreach (int n in gr.Nodes)
			{
				sb.AppendFormat("({0} s:(", n);
				foreach (int i in gr.Successors(n))
				{
					sb.AppendFormat("{0} ", i);
				}
				sb.Append(") p:( ");
				foreach (int p in gr.Predecessors(n))
				{
					sb.AppendFormat("{0} ", p);
				}
				sb.Append(")) ");
			}
			return sb.ToString();
		}

		[Test]
		public void SimpleGraph()
		{
			DirectedGraph gr = CreateAcyclicGraph();
			string sExp = "(0 s:(1 2 ) p:( )) (1 s:(2 ) p:( 0 2 )) (2 s:(1 ) p:( 0 1 )) ";
			string s = DumpGraph(gr);

			Assert.AreEqual(sExp, s);
		}

		[Test]
		public void ModifyGraph()
		{
			DirectedGraph gr = CreateAcyclicGraph();

			gr.AddNode(3);
			gr.AddEdge(0, 3);
			gr.AddEdge(1, 3);

			gr.RemoveEdge(1, 2);

			string sExp = "(0 s:(1 2 3 ) p:( )) (1 s:(3 ) p:( 0 2 )) (2 s:(1 ) p:( 0 )) (3 s:() p:( 0 1 )) ";

			string s = DumpGraph(gr);
			Assert.AreEqual(sExp, s);

			gr.RemoveEdge(2, 1);
			gr.RemoveEdge(0, 1);
			gr.RemoveEdge(0, 2);

			sExp = "(0 s:(3 ) p:( )) (1 s:(3 ) p:( )) (2 s:() p:( )) (3 s:() p:( 0 1 )) ";

			s = DumpGraph(gr);
			Assert.AreEqual(sExp, s);
		}
	}
}
