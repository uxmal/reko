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

using System;
using System.Diagnostics;
using System.Collections;

namespace System.Collections
{
	public class Tree
	{
		public enum Color { Black, Red }

		public class Node
		{
			public Node Parent;
			public Node Left;
			public Node Right;
			public object Key;
			public object Value;
			public Color Color;
			public bool IsNil;

			public Node Sibling
			{
				get 
				{
					Node p = Parent; 
					return p.Left == this ? p.Right : p.Left;
				}
			}
		}

		private Node head;
		private int count;
		private IComparer cmp;

		public Tree()
		{
			Init(System.Collections.Comparer.Default);
		}

		public Tree(IComparer cmp)
		{
			Init(cmp);
		}

		private void Init(IComparer cmp)
		{
			this.head = new Node();
			this.count = 0;
			this.cmp = cmp;

			head.Parent = head.Left = head.Right = head;
			head.Color = Color.Black;
			head.IsNil = true;
		}

#if DEBUG
		private int CheckInvariantAux(Node n)
		{
			if (n.IsNil)
				return 0;

			// All pointers are sane.

			Debug.Assert(n.Left.IsNil || n.Left.Parent == n);
			Debug.Assert(n.Right.IsNil || n.Right.Parent == n);

			int left = CheckInvariantAux(n.Left);
			int right = CheckInvariantAux(n.Right);

			// Each path has the same number of black nodes.

			Debug.Assert(left == right);

			// No red node has a red ancestor.

			Debug.Assert(n.Color == Color.Black || n.Parent.Color == Color.Black);
			if (n.Color == Color.Black)
				++left;
			return left;
		}

		private void CheckInvariant()
		{
			// Root node is black.
			Debug.Assert(Root.Color == Color.Black);
			CheckInvariantAux(Root);
		}
#else
		private void CheckInvariant()
		{
		}
#endif

		public void Clear()
		{
			count = 0;
			Root.Parent = null;
			Root = head;
		}


		public int Count
		{
			get { return count; }
		}

		public Node Find(object key)
		{
			Node n = Root;
			while (!n.IsNil)
			{
				int cmp = this.cmp.Compare(key, n.Key);
				if (cmp < 0)
				{
					n = n.Left;
				}
				else if (cmp > 0)
				{
					n = n.Right;
				}
				else
				{
					break;
				}
			}
			return n;
		}

		public Node First
		{
			get { return Min(Root); }
		}

		public void Insert(object key, object val)
		{

			Node x = Root;
			Node p = head;
			while (!x.IsNil)
			{
				// We need to split 4-nodes as we descend the tree.

				if (Is4Node(x))
					Split(x, key, val);

				int cmp = this.cmp.Compare(key, x.Key);
				if (cmp == 0)
				{
					// Node already exists! Overwrite its value.
					x.Value = val;
					return;
				}
				p = x;
				x = cmp < 0 ? x.Left : x.Right;
			}

			x = new Node();
			x.Key = key;
			x.Value = val;
			x.Left = head;
			x.Right = head;
			x.Parent = p;

			if (p == head)
			{
				Root = x;
			}
			else if (this.cmp.Compare(key, p.Key) < 0)
				p.Left = x;
			else
				p.Right = x;
			Split(x, key, val);
			++count;

			CheckInvariant();
		}


		private bool Is4Node(Node x)
		{
			return x.Left.Color == Color.Red && x.Right.Color == Color.Red;
		}

		private bool Is2Node(Node x)
		{
			return x.Left.Color == Color.Black && x.Right.Color == Color.Black;
		}

		public Node Root
		{ 
			get { return head.Right; }
			set { head.Right = value; }
		}

		public Node Max(Node n)
		{
			while (!n.Right.IsNil)
			{
				n = n.Right;
			}
			return n;
		}

		public Node Min(Node n)
		{
			while (!n.Left.IsNil)
			{
				n = n.Left;
			}
			return n;
		}

		protected Node Predecessor(Node n)
		{
			return Max(n.Left);
		}

		public virtual void Remove(object key)
		{
			Node n = Find(key);
			if (n.IsNil)
				return;

			if (!n.Left.IsNil && !n.Right.IsNil)
			{
				// If the node has two children, we replace
				// the value of the node with the value of its 
				// successor, then delete the successor.

				Node s = Min(n.Right);
				n.Key = s.Key;
				n.Value = s.Value;

				n = s;			// s is now to be deleted.
			}

			// The node to be deleted is either a leaf or has only one child.

			Debug.Assert(n.Left.IsNil || n.Right.IsNil);
			if (n.Color == Color.Black)
			{
				// Deleting this node will muss up the black invariant
				// of the tree, so we fix up the tree to make the node red.

				RemoveFixup(n);
			}

			Debug.Assert(n.Color == Color.Red);

			// Now snip the item from the tree.

			Node kid = (n.Left.IsNil) ? n.Right : n.Left;
			if (n.Parent.Left == n)
			{
				n.Parent.Left = kid;
			}
			else
			{
				Debug.Assert(n.Parent.Right == n);
				n.Parent.Right = kid;
			}
			kid.Parent = n.Parent;


			--count;
			Root.Color = Color.Black;
			CheckInvariant();
		}

		private void RemoveFixup(Node n)
		{
			Debug.Assert(n.Color == Color.Black);

			// We wish to end up making n be red. In order to do so, we traverse the tree
			// coloring the current node X red. If this violates the Red-black invariant,
			// we must fix the tree.

			// X is the node we examine. T is the sibling of X. P is the parent of T and X.
			// L is the left child of T, and R is its right child.

			// As we descend the tree, we mark X red. We just came from P so we know it is red
			// also. T is black, since it is a child of the red P.

			// First, examine the root.

			Node r = Root;
			Node x, t, p, l;
			if (r == n)
			{
				n.Color = Color.Red;
				n.Left.Color = n.Right.Color = Color.Black;
				return;
			}
			if (Is2Node(r))
			{
				r.Color = Color.Red;
				x = cmp.Compare(n.Value, r.Value) < 0 ? r.Left : r.Right;
			}
			else
			{
				x = r;
			}

			// Now wander down the tree handling the special cases.

			do
			{
				// As we traverse down the tree, we continually encounter this situation 
				// until we reach the node to be deleted
				// X is Black, P is Red, T is Black

				Debug.Assert(x.Color == Color.Black);

				// We are going to color X Red, then recolor other nodes and possibly 
				// do rotation(s) based on the color of X’s and T’s children

				p = x.Parent;
				t = x.Sibling;
				l = t == p.Right ? t.Left : t.Right;
				r = t == p.Right ? t.Right : t.Left;
				if (Is2Node(x))
				{
					Debug.Assert(p.Color == Color.Red);
					Debug.Assert(t.Color == Color.Black);

					if (Is2Node(t))
					{
						p.Color = Color.Black;
						t.Color = Color.Red;
					}
					else if (l.Color == Color.Red)
					{
						// Rotate l around T, then around P.
						Rotate(t, l.Key);
						Debug.Assert(l.Parent == p);
						Rotate(p, l.Key);
						Debug.Assert(p.Parent == l);
						// Recolor p and x.
						Debug.Assert(p.Color == Color.Red);
						p.Color = Color.Black;
					}
					else
					{
						Debug.Assert(r.Color == Color.Red);

						// Rotate T around P.
						Rotate(p, t.Key);
						// Recolor X, P, T, R.
						Debug.Assert(p.Color == Color.Red);
						r.Color = Color.Black;
						p.Color = Color.Black;
					}
					x.Color = Color.Red;
				}
				else
				{
					// X has at least one red child, i.e.
					// it is at least a 3, and maybe even a 4-node.

					Debug.Assert(x.Left.Color == Color.Red || x.Right.Color == Color.Red);
					x = cmp.Compare(n.Key, x.Key) < 0 ? x.Left : x.Right;
					if (x.Color == Color.Red)
					{
						x = cmp.Compare(n.Key, x.Key) < 0 ? x.Left : x.Right;
					}
					else
					{
						t = x.Sibling;
						p = x.Parent;
						Rotate(p, t.Key);
						t.Color = Color.Red;
						p.Color = Color.Black;
					}
				}

			} while (x != n);
		}

		/// <summary>
		/// Rotates about the node <paramref>about</paramref>.
		/// </summary>
		/// <param name="about"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		/// 
		private Node Rotate(Node about, object key)
		{
			Node p = about.Parent;
			bool fLeft = (p.Left == about);
			Node c = about;
			Node gc;
			if (cmp.Compare(key, c.Key) < 0)
			{
				gc = c.Left; c.Left = gc.Right; gc.Right.Parent = c; gc.Right = c;
			}
			else
			{
				gc = c.Right; c.Right = gc.Left; gc.Left.Parent = c; gc.Left = c;
			}
			c.Parent = gc;

			if (fLeft)
				p.Left = gc; 
			else
				p.Right = gc;
			gc.Parent = p;
			return gc;
		}

		private Node Split(Node x, object key, object val)
		{
			x.Color = Color.Red;	// always maintain black invariant, but may break red-red invariant.
			x.Left.Color = Color.Black;
			x.Right.Color = Color.Black;
			Node p = x.Parent;
			Node g = p.Parent;
			if (p.Color == Color.Red)
			{
				// Red-red invariant broken. Rebalance the tree.

				g.Color = Color.Red;
				if (cmp.Compare(key, g.Key) < 0 == cmp.Compare(key, p.Key) < 0)
				{
					// zig-zig case.
					Rotate(g, p.Key);
					p.Color = Color.Black;
				}
				else
				{
					// zig-zag case.
					Rotate(p, x.Key);
					Rotate(g, x.Key);
					x.Color = Color.Black;
				}
				g.Color = Color.Red;
			}
			Root.Color = Color.Black;
			return x.Parent;
		}

		public Node Successor(Node n)
		{
			if (n.IsNil)
				return n;
			if (!n.Right.IsNil)
				return Min(n.Right);
			else
			{	// climb looking for right subtree
				Node p = n.Parent;
				for (; !p.IsNil && n == p.Right; n = p, p = n.Parent)
				{
				}
				return p;
			}		
		}

		public Node LowerBoundInclusive(object key)
		{
			Node p = head;
			Node n = Root;
			while (!n.IsNil)
			{
				if (cmp.Compare(key, n.Key) >= 0)
				{
					p = n;
					n = n.Right;
				}
				else
				{
					n = n.Left;
				}
			}
			return p;
		}

	}
}