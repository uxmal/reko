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
using System.Collections;

namespace System.Collections
{
	/// <summary>
	/// Represents a set of items.
	/// </summary>
	public class Set : ICollection
	{
		private Tree tree;

		public Set()
		{
			tree = new Tree(Comparer.Default);
		}

		public Set(IComparer cmp)
		{
			tree = new Tree(cmp);
		}

		public bool this[object key]
		{
			get 
			{
				if (key == null)
					throw new ArgumentNullException();
				return !tree.Find(key).IsNil;
			}
			set 
			{
				if (key == null)
					throw new ArgumentNullException();
				if (value)
					tree.Insert(key, key);
				else
					tree.Remove(key);
			}
		}

		public void Add(object key)
		{
			if (key == null)
				throw new ArgumentNullException();
			if (tree.Find(key).IsNil)
				tree.Insert(key, key);
			else
				throw new ArgumentException("An element with the key \'" + key + "\' already exists.");
		}

		public void CopyTo(Array a, int i)
		{
			Tree.Node n = tree.First;
			while (!n.IsNil)
			{
				a.SetValue(n.Key, i++);
				n = tree.Successor(n);
			}
		}

		public int Count
		{
			get { return tree.Count; }
		}

		public bool IsEqual(Set s)
		{
			if (s == null)
				return false;
			Tree.Node i = tree.First;
			Tree.Node j = s.tree.First;
			while (!i.IsNil && !j.IsNil)
			{
				if (!i.Key.Equals(j.Key))
					return false;
				i = tree.Successor(i);
				j = tree.Successor(j);
			}
			return i.IsNil && j.IsNil;
		}

		public IEnumerator GetEnumerator()
		{
			return new SetEnumerator(tree.First, tree);
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		
		private class SetEnumerator : IEnumerator
		{
			private Tree.Node nCur;
			private Tree tree;
			private object item;
			private bool fStarted;

			public SetEnumerator(Tree.Node n, Tree tree)
			{
				this.nCur = n;
				this.tree = tree;
			}

			public object Current
			{
				get 
				{
					if (!fStarted)
						InvalidOp();
					return item;
				}
			}

			private void InvalidOp()
			{
				throw new InvalidOperationException("You must call 'MoveNext' before accessing the current value of the enumerator.");
			}

			public bool MoveNext()
			{
				if (nCur.IsNil)
				{
					fStarted = false;
					return false;
				}
				item = nCur.Key;
				nCur = tree.Successor(nCur);
				fStarted = true;
				return true;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

	}
}
