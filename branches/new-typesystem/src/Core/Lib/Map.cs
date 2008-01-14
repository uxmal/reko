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

namespace System.Collections
{
	/// <summary>
	/// Represents a collection of key-and-value pairs in sorted order. Lookups are just
	/// as efficient as with the SortedList class, while adding and removing elements are much faster.
	/// </summary>
	public class Map : IDictionary
	{
		private Tree tree;
		private KeyCollection keys;
		private ValueCollection values;

		public Map()
		{
			Init(Comparer.Default);
		}

		public Map(IComparer cmp)
		{
			Init(cmp);
		}

		public Map(IDictionary dict)
		{
			Init(Comparer.Default);
			CopyDictionary(dict);
		}

		public Map(IComparer cmp, IDictionary dict)
		{
			Init(cmp);
			CopyDictionary(dict);
		}

		public virtual object this[object key]
		{
			get 
			{ 
				if (key == null)
					throw new ArgumentNullException();
				Tree.Node n = tree.Find(key);
				if (n.IsNil)
					return null;
				else
					return n.Value;
			}
			set
			{
				if (key == null)
					throw new ArgumentNullException();
				tree.Insert(key, value);
			}
		}

		public virtual void Add(object key, object val)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (tree.Find(key).IsNil)
				tree.Insert(key, val);
			else
				throw new ArgumentException("An element with the key \'" + key + "\' already exists.");
		}

		public virtual void Clear()
		{
			tree.Clear();
		}

		private void CopyDictionary(IDictionary dict)
		{
			foreach (DictionaryEntry de in dict)
			{
				this[de.Key] = de.Value;
			}
			Debug.Assert(dict.Count == this.Count);
		}

		public virtual void CopyTo(Array arr, int i)
		{
			Tree.Node n = tree.First;
			while (!n.IsNil)
			{
				arr.SetValue(new DictionaryEntry(n.Key, n.Value), i++);
				n = tree.Successor(n);
			}
		}

		public virtual bool Contains(object key)
		{
			return !tree.Find(key).IsNil;
		}

		public virtual int Count
		{
			get { return tree.Count; }
		}

		private void Init(IComparer cmp)
		{
			tree = new Tree(cmp);
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new TreeEnumerator(tree.First, tree);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeEnumerator(tree.First, tree);
		}

		public IDictionaryEnumerator GetBoundedEnumerator(object min)
		{
			return new TreeEnumerator(tree.LowerBoundInclusive(min), tree);
		}

		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		public virtual ICollection Keys
		{
			get 
			{ 
				if (keys == null)
					keys = new KeyCollection(this);
				return keys;
			}
		}
		
		public object LowerBound(object key)
		{
			if (key == null)
				throw new ArgumentNullException();
			Tree.Node n = tree.LowerBoundInclusive(key);
			return (n.IsNil) ? null : n.Value;
		}

		public virtual void Remove(object key)
		{
			tree.Remove(key);
		}

		public virtual object SyncRoot
		{
			get { return this; }
		}

		public virtual ICollection Values
		{
			get 
			{ 
				if (values == null)
					values = new Map.ValueCollection(this);
				return values;
			}
		}

		private abstract class ListBase : ICollection
		{
			protected Map map;

			public ListBase(Map m)
			{
				this.map = m;
			}

			public abstract void CopyTo(Array a, int i);

			public virtual int Count
			{
				get { return map.Count; }
			}

			public virtual bool IsSynchronized
			{
				get { return map.IsSynchronized; }
			}

			public virtual object SyncRoot
			{
				get { return map.SyncRoot; }
			}

			public abstract IEnumerator GetEnumerator();
		}

		private class KeyCollection : ListBase
		{
			public KeyCollection(Map m) : base(m)
			{
			}
			
			public override void CopyTo(Array a, int i)
			{
				Tree.Node n = map.tree.First;
				while (!n.IsNil)
				{
					a.SetValue(n.Key, i++);
					n = map.tree.Successor(n);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return new KeyEnumerator(map.tree.First, map.tree);
			}
		}

		private class ValueCollection : ListBase
		{
			public ValueCollection(Map m) : base(m)
			{
			}

			public override void CopyTo(Array a, int i)
			{
				Tree.Node n = map.tree.First;
				while (!n.IsNil)
				{
					a.SetValue(n.Value, i++);
					n = map.tree.Successor(n);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return new ValueEnumerator(map.tree.First, map.tree);
			}
		}

		private class TreeEnumerator : IDictionaryEnumerator
		{
			protected Tree tree;
			private Tree.Node nCur;
			protected object key;
			protected object val;

			public TreeEnumerator(Tree.Node n, Tree t)
			{
				nCur = n;
				tree = t;
			}

			public virtual object Current
			{
				get 
				{ 
					if (key == null)
						InvalidOp();
					return new DictionaryEntry(key, val);
				}
			}

			public virtual DictionaryEntry Entry
			{
				get 
				{ 
					if (key == null)
						InvalidOp();
					return new DictionaryEntry(key, val);
				}
			}

			protected void InvalidOp()
			{
				throw new InvalidOperationException("You must call 'MoveNext' before accessing the current value of the enumerator.");
			}

			public virtual object Key
			{
				get 
				{
					if (key == null)
						InvalidOp();
					return key;
				}
			}

			public bool MoveNext()
			{
				if (nCur.IsNil)
				{
					key = null;
					return false;
				}

				key = nCur.Key;
				val = nCur.Value;
				nCur = tree.Successor(nCur);
				return true;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public virtual object Value
			{
				get 
				{ 
					if (key == null)
						InvalidOp();
					return val;
				}
			}

		}

		private class KeyEnumerator : TreeEnumerator
		{
			public KeyEnumerator(Tree.Node n, Tree t) : base(n, t)
			{
			}

			public override object Current
			{
				get 
				{
					if (key == null)
						InvalidOp();
					return key;
				}
			}
		}

		private class ValueEnumerator : TreeEnumerator
		{
			public ValueEnumerator(Tree.Node n, Tree t) : base(n, t)
			{
			}

			public override object Current
			{
				get 
				{
					if (key == null)
						InvalidOp();
					return val;
				}
			}
		}
	}
}
