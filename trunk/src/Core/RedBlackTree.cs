/* 
 * Copyright (C) 1999-2007 John Källén.
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

namespace Decomp.Core
{
	public class RedBlackTree : IDictionary
	{
		private Node _Myhead;	// pointer to head node
		private int  _Mysize;	// number of elements

		protected enum Color
		{
			_Red, _Black
		}

		protected class Node
		{
			// base class for _Tree_ptr to hold allocator _Alnod

			public Node(Node _Larg, Node _Parg, Node _Rarg,
				object _Val, char _Carg)
			{
				_Left = _Larg;
				_Parent = _Parg;
				_Right = _Rarg;
				_Myval = _Val;
				_Color = _Carg;
				_Isnil = false;
			}

			Node _Left;	// left subtree, or smallest element if head
			Node _Parent;	// parent, or root of tree if head
			Node _Right;	// right subtree, or largest element if head
			object _Key;
			object _Myval;	// the stored value, unused if head
			Color _Color;	// _Red or _Black, _Black if head
			bool _Isnil;	// true only if head (also nil) node
		}

		/*
	typedef _Tree<_Traits> _Myt;
	typedef _Tree_val<_Traits> _Mybase;
	typedef typename _Traits::key_type key_type;
	typedef typename _Traits::key_compare key_compare;
	typedef typename _Traits::value_compare value_compare;
	typedef typename _Traits::value_type value_type;
	typedef typename _Traits::allocator_type allocator_type;
	typedef typename _Traits::_ITptr _ITptr;
	typedef typename _Traits::_IReft _IReft;

protected:
	typedef typename _Tree_nod<_Traits>::_Genptr _Genptr;
	typedef typename _Tree_nod<_Traits>::_Node _Node;
*/


		protected class Enumerator : IDictionaryEnumerator
		{	
			private Node _Ptr;
			private object current;
			private RedBlackTree tree;

			public Enumerator(RedBlackTree tree)
			{
				this.tree = tree;
				_Ptr = null;
			}

			public Enumerator(Node n)
			{
				_Ptr = n;
			}

			public virtual object Current
			{
				get 
				{
					if (current == null)
						throw new InvalidOperationException("Enumerator is positioned before the first element or after the last element.");
					return current;
				}
			}
		
			public virtual bool MoveNext()
			{
				_Inc();
			}

			public virtual bool MovePrev()
			{
				_Dec();
			}

			public virtual void Reset()
			{
				
			}

			private void _Dec()
			{	// move to node with next smaller value
				if (_Ptr._IsNil)
					_Ptr = _Ptr._Right;	// end() ==> rightmost
				else if (!_Isnil(_Ptr._Left))
					_Ptr = _Max(_Ptr._Left);	// ==> largest of left subtree
				else
				{	// climb looking for left subtree
					Node _Pnode;
					while (!_Isnil(_Pnode = _Ptr._Parent)&&
						_Ptr == _Pnode._Left)
					{
						_Ptr = _Pnode;	// ==> parent while left subtree
					}
					if (!_Isnil(_Pnode))
						_Ptr = _Pnode;	// ==> parent if not head
				}
			}

			private void _Inc()
			{	// move to node with next larger value
				if (_Isnil(_Ptr))
					return;	// end() shouldn't be incremented, don't move
				else if (!_Isnil(_Ptr._Right))
					_Ptr = _Min(_Ptr._Right);	// ==> smallest of right subtree
				else
				{	// climb looking for right subtree
					Node _Pnode;
					while (!_Isnil(_Pnode = _Ptr._Parent)
						&& _Ptr == _Pnode._Right)
						_Ptr = _Pnode;	// ==> parent while right subtree
					_Ptr = _Pnode;	// ==> parent (head if end())
				}
			}

			public Node _Mynode()
			{
				return (_Ptr);
			}

		}


		public RedBlackTree()
		{	
			_Init();
		}

		public RedBlackTree(IComparer comp)
		{
			comp = parg;
			_Init();
		}

		public RedBlackTree(IDictionary dict)
		{
			_Init();
			foreach (DictionaryEntry de in dict)
			{
				insert(de.Key, de.Value);
			}
		}


		public virtual IEnumerator GetEnumerator()
		{
			return new Enumerator(_Lmost());
		}


		public int Count
		{
			get { return (_Mysize);	}
		}

		public virtual void Add(object key, object _Val)
		{	// try to insert node with value _Val
			Node _Trynode = _Root;
			Node _Wherenode = _Myhead;
			bool _Addleft = true;	// add to left of head if tree empty
			while (!_Isnil(_Trynode))
			{	// look for leaf to insert before (_Addleft) or after
				_Wherenode = _Trynode;
				_Addleft = this.comp(this._Kfn(_Val), _Key(_Trynode));
				_Trynode = _Addleft ? _Trynode._Left : _Trynode._Right;
			}

			if (this._Multi)
			{
				return (_Pairib(_Insert(_Addleft, _Wherenode, _Val), true));
			}
			else
			{	// insert only if unique
				iterator _Where = iterator(_Wherenode);
				if (!_Addleft)
					;	// need to test if insert after is okay
				else if (_Where == begin())
					return (_Pairib(_Insert(true, _Wherenode, _Val), true));
				else
					--_Where;	// need to test if insert before is okay

				if (this.comp(_Key(_Where._Mynode()), this._Kfn(_Val)))
					return (_Pairib(_Insert(_Addleft, _Wherenode, _Val), true));
				else
					return (_Pairib(_Where, false));
			}
		}

		public virtual bool IsSynchronized
		{
			get 
			{
				return false;		//$
			}
		}

		public virtual object SyncRoot
		{
			get { return null; } //$
		}

		public virtual bool IsReadonly
		{
			get { return false; } //$
		}

		public virtual ICollection Keys
		{
			get { return null; }
		}

		public virtual ICollection Values
		{
			get { return null; }
		}

/*
		public void insert(iterator _Where, object _Val)
		{	// try to insert node with value _Val using _Where as a hint
			iterator _Next;

			if (Count == 0)
				return (_Insert(true, _Myhead, _Val));	// insert into empty tree
			else if (this._Multi)
			{	// insert even if duplicate
				if (_Where == begin())
				{	// insert at beginning if before first element
					if (!this.comp(_Key(_Where._Mynode()), this._Kfn(_Val)))
						return (_Insert(true, _Where._Mynode(), _Val));
				}
				else if (_Where == end())
				{	// insert at end if after last element
					if (!this.comp(this._Kfn(_Val), _Key(_Rmost())))
						return (_Insert(false, _Rmost(), _Val));
				}
				else if (!this.comp(_Key(_Where._Mynode()), this._Kfn(_Val))
					&& !this.comp(this._Kfn(_Val),
					_Key((--(_Next = _Where))._Mynode())))
				{	// insert before _Where
					if (_Isnil(_Right(_Next._Mynode())))
						return (_Insert(false, _Next._Mynode(), _Val));
					else
						return (_Insert(true, _Where._Mynode(), _Val));
				}
				else if (!this.comp(this._Kfn(_Val), _Key(_Where._Mynode()))
					&& (++(_Next = _Where) == end()
					|| !this.comp(_Key(_Next._Mynode()),
					this._Kfn(_Val))))
				{	// insert after _Where
					if (_Isnil(_Right(_Where._Mynode())))
						return (_Insert(false, _Where._Mynode(), _Val));
					else
						return (_Insert(true, _Next._Mynode(), _Val));
				}
			}
			else
			{	// insert only if unique
				if (_Where == begin())
				{	// insert at beginning if before first element
					if (this.comp(this._Kfn(_Val), _Key(_Where._Mynode())))
						return (_Insert(true, _Where._Mynode(), _Val));
				}
				else if (_Where == end())
				{	// insert at end if after last element
					if (this.comp(_Key(_Rmost()), this._Kfn(_Val)))
						return (_Insert(false, _Rmost(), _Val));
				}
				else if (this.comp(this._Kfn(_Val), _Key(_Where._Mynode()))
					&& this.comp(_Key((--(_Next = _Where))._Mynode()),
					this._Kfn(_Val)))
				{	// insert before _Where
					if (_Isnil(_Right(_Next._Mynode())))
						return (_Insert(false, _Next._Mynode(), _Val));
					else
						return (_Insert(true, _Where._Mynode(), _Val));
				}
				else if (this.comp(_Key(_Where._Mynode()), this._Kfn(_Val))
					&& (++(_Next = _Where) == end()
					|| this.comp(this._Kfn(_Val),
					_Key(_Next._Mynode()))))
				{	// insert after _Where
					if (_Isnil(_Where._Mynode()._Right))
						return (_Insert(false, _Where._Mynode(), _Val));
					else
						return (_Insert(true, _Next._Mynode(), _Val));
				}
			}

			return (insert(_Val).first);	// try usual insert if all else fails
		}

*/
 /*
		public iterator erase(Node _Where)
		{	// erase element at _Where
			if (_Isnil(_Where._Mynode()))
				_THROW(out_of_range, "invalid map/set<T> iterator");
			Node _Fixnode;	// the node to recolor as needed
			Node _Fixnodeparent;	// parent of _Fixnode (which may be nil)
			Node _Erasednode = _Where;	// node to erase
			Node _Pnode = _Erasednode;
			++_Where;	// save successor iterator for return

			if (_Isnil(_Pnode._Left))
				_Fixnode = _Pnode._Right;	// must stitch up right subtree
			else if (_Isnil(_Pnode._Right))
				_Fixnode = _Pnode._Left;	// must stitch up left subtree
			else
			{	// two subtrees, must lift successor node to replace erased
				_Pnode = _Where;	// _Pnode is successor node
				_Fixnode = _Pnode._Right;	// _Fixnode is its only subtree
			}

			if (_Pnode == _Erasednode)
			{	// at most one subtree, relink it
				_Fixnodeparent = _Erasednode._Parent;
				if (!_Isnil(_Fixnode))
					_Fixnode._Parent = _Fixnodeparent;	// link up

				if (_Root() == _Erasednode)
					_Root() = _Fixnode;	// link down from root
				else if (_Fixnodeparent._Left == _Erasednode)
					_Fixnodeparent._Left = _Fixnode;	// link down to left
				else
					_Fixnodeparent._Right = _Fixnode;	// link down to right

				if (_Lmost() == _Erasednode)
					_Lmost() = _Isnil(_Fixnode)
						? _Fixnodeparent	// smallest is parent of erased node
						: _Min(_Fixnode);	// smallest in relinked subtree

				if (_Rmost() == _Erasednode)
					_Rmost() = _Isnil(_Fixnode)
						? _Fixnodeparent	// largest is parent of erased node
						: _Max(_Fixnode);	// largest in relinked subtree
			}
			else
			{	// erased has two subtrees, _Pnode is successor to erased
				_Erasednode._Left._Parent = _Pnode;	// link left up
				_Pnode._Left = _Erasednode._Left;	// link successor down

				if (_Pnode == _Erasednode._Right)
					_Fixnodeparent = _Pnode;	// successor is next to erased
				else
				{	// successor further down, link in place of erased
					_Fixnodeparent = _Pnode._Parent;	// parent is successor's
					if (!_Isnil(_Fixnode))
						_Fixnode._Parent = _Fixnodeparent;	// link fix up
					_Fixnodeparent._Left = _Fixnode;	// link fix down
					_Pnode._Right = _Erasednode._Right;	// link successor down
					_Erasednode._Right._Parent = _Pnode;	// link right up
				}

				if (_Root() == _Erasednode)
					_Root() = _Pnode;	// link down from root
				else if (_Erasednode._Parent._Left == _Erasednode)
					_Erasednode._Parent._Left = _Pnode;	// link down to left
				else
					_Right(_Erasednode._Parent) = _Pnode;	// link down to right

				_Pnode._Parent = _Erasednode._Parent;	// link successor up
				swap(_Color(_Pnode), _Color(_Erasednode));	// recolor it
			}

			if (_Color(_Erasednode) == _Black)
			{	// erasing black link, must recolor/rebalance tree
				for (; _Fixnode != _Root() && _Color(_Fixnode) == _Black;
					_Fixnodeparent = _Fixnode._Parent)
					if (_Fixnode == _Fixnodeparent._Left)
					{	// fixup left subtree
						_Pnode = _Fixnodeparent._Right;
						if (_Color(_Pnode) == _Red)
						{	// rotate red up from right subtree
							_Color(_Pnode) = _Black;
							_Color(_Fixnodeparent) = _Red;
							_Lrotate(_Fixnodeparent);
							_Pnode = _Fixnodeparent._Right;
						}

						if (_Isnil(_Pnode))
							_Fixnode = _Fixnodeparent;	// shouldn't happen
						else if (_Color(_Pnode._Left) == _Black
							&& _Color(_Pnode._Right) == _Black)
						{	// redden right subtree with black children
							_Color(_Pnode) = _Red;
							_Fixnode = _Fixnodeparent;
						}
						else
						{	// must rearrange right subtree
							if (_Color(_Pnode._Right) == _Black)
							{	// rotate red up from left sub-subtree
								_Color(_Pnode._Left) = _Black;
								_Color(_Pnode) = _Red;
								_Rrotate(_Pnode);
								_Pnode = _Fixnodeparent._Right;
							}

							_Color(_Pnode) = _Color(_Fixnodeparent);
							_Color(_Fixnodeparent) = _Black;
							_Color(_Pnode._Right) = _Black;
							_Lrotate(_Fixnodeparent);
							break;	// tree now recolored/rebalanced
						}
					}
					else
					{	// fixup right subtree
						_Pnode = _Fixnodeparent._Left;
						if (_Color(_Pnode) == _Red)
						{	// rotate red up from left subtree
							_Color(_Pnode) = _Black;
							_Color(_Fixnodeparent) = _Red;
							_Rrotate(_Fixnodeparent);
							_Pnode = _Fixnodeparent._Left;
						}
						if (_Isnil(_Pnode))
							_Fixnode = _Fixnodeparent;	// shouldn't happen
						else if (_Color(_Pnode._Right) == _Black
							&& _Color(_Pnode._Left) == _Black)
						{	// redden left subtree with black children
							_Color(_Pnode) = _Red;
							_Fixnode = _Fixnodeparent;
						}
						else
						{	// must rearrange left subtree
							if (_Color(_Pnode._Left) == _Black)
							{	// rotate red up from right sub-subtree
								_Color(_Pnode._Right) = _Black;
								_Color(_Pnode) = _Red;
								_Lrotate(_Pnode);
								_Pnode = _Fixnodeparent._Left;
							}

							_Color(_Pnode) = _Color(_Fixnodeparent);
							_Color(_Fixnodeparent) = _Black;
							_Color(_Pnode._Left) = _Black;
							_Rrotate(_Fixnodeparent);
							break;	// tree now recolored/rebalanced
						}
					}

				_Color(_Fixnode) = _Black;	// ensure stopping node is black
			}

			this._Alnod.destroy(_Erasednode);	// destroy, free erased node
			this._Alnod.deallocate(_Erasednode, 1);

			if (0 < _Mysize)
				--_Mysize;

			return (_Where);	// return successor iterator
		}
*/

		public void Clear()
		{
			_Mysize = 0;
			
			_Root = _Myhead;
			_Mysize = 0;
			_Lmost = _Myhead;
			_Rmost = _Myhead;
		}

		public void CopyTo(System.Array a, int i)
		{
			foreach (DictionaryEntry de in this)
			{
				a.SetValue(de, i++);
			}
		}

		public virtual bool Contains(object key)
		{
			return false;	//$
		}
		IEnumerator Find(object key)
		{	// find an element in mutable sequence that matches _Keyval
			iterator _Where = lower_bound(_Keyval);
			return (_Where == end() || this.comp(_Keyval, _Key(_Where._Mynode()))
				? end() : _Where);
		}

		IEnumerator lower_bound(object _Keyval)
		{	// find leftmost node not less than _Keyval in mutable tree
			return new Enumerator(_Lbound(_Keyval));
		}


		public IEnumerator upper_bound(object _Keyval)
		{	// find leftmost node greater than _Keyval in mutable tree
			return new Enumerator(_Ubound(_Keyval));
		}

		protected void _Copy(RedBlackTree _Right)
		{	// copy entire tree from _Right
			_Root = _Copy(_Right._Root, _Myhead);
			_Mysize = _Right.Count;
			if (!_Isnil(_Root))
			{	// nonempty tree, look for new smallest and largest
				_Lmost = _Min(_Root);
				_Rmost = _Max(_Root);
			}
			else
			{
				_Lmost = _Myhead;
				_Rmost = _Myhead;	// empty tree
			}
		}

		protected Node _Copy(Node _Rootnode, Node _Wherenode)
		{	// copy entire subtree, recursively
			Node _Newroot = _Myhead;	// point at nil node

			if (!_Isnil(_Rootnode))
			{	// copy a node, then any subtrees
				Node _Pnode = _Buynode(_Myhead, _Wherenode, _Myhead,
					_Myval(_Rootnode), _Color(_Rootnode));
				if (_Isnil(_Newroot))
					_Newroot = _Pnode;	// memorize new root

				_Pnode._Left = _Copy(_Rootnode._Left, _Pnode);
				_Pnode._Right = _Copy(_Rootnode._Right, _Pnode);
			}
			return (_Newroot);	// return newlyructed tree
		}


		protected void _Init()
		{	// create head/nil node and make tree empty
			_Myhead = _Buynode();
			_Isnil(_Myhead) = true;
			_Root = _Myhead;
			_Lmost = _Myhead;
			_Rmost = _Myhead;
			_Mysize = 0;
		}
	
		protected Enumerator _Insert(bool _Addleft, Node _Wherenode, object _Val)
		{	// add node with value next to _Wherenode, to left if _Addnode
			Node _Newnode = _Buynode(_Myhead, _Wherenode, _Myhead,	_Val, _Red);
			++_Mysize;

			if (_Wherenode == _Myhead)
			{	// first node in tree, just set head values
				_Root() = _Newnode;
				_Lmost() = _Newnode;
				_Rmost() = _Newnode;
			}
			else if (_Addleft)
			{	// add to left of _Wherenode
				_Wherenode._Left = _Newnode;
				if (_Wherenode == _Lmost())
					_Lmost() = _Newnode;
			}
			else
			{	// add to right of _Wherenode
				_Wherenode._Right = _Newnode;
				if (_Wherenode == _Rmost())
					_Rmost() = _Newnode;
			}

			for (Node _Pnode = _Newnode; _Color(_Pnode._Parent) == _Red; )
				if (_Pnode._Parent == _Pnode._Parent._Parent._Left)
				{	// fixup red-red in left subtree
					_Wherenode = _Right(_Pnode._Parent._Parent);
					if (_Color(_Wherenode) == _Red)
					{	// parent has two red children, blacken both
						_Color(_Pnode._Parent) = _Black;
						_Color(_Wherenode) = _Black;
						_Color(_Pnode._Parent._Parent) = _Red;
						_Pnode = _Pnode._Parent._Parent;
					}
					else
					{	// parent has red and black children
						if (_Pnode == _Right(_Pnode._Parent))
						{	// rotate right child to left
							_Pnode = _Pnode._Parent;
							_Lrotate(_Pnode);
						}
						_Color(_Pnode._Parent) = _Black;	// propagate red up
						_Color(_Pnode._Parent._Parent) = _Red;
						_Rrotate(_Pnode._Parent._Parent);
					}
				}
				else
				{	// fixup red-red in right subtree
					_Wherenode = _Pnode._Parent._Parent._Left;
					if (_Color(_Wherenode) == _Red)
					{	// parent has two red children, blacken both
						_Color(_Pnode._Parent) = _Black;
						_Color(_Wherenode) = _Black;
						_Color(_Pnode._Parent._Parent) = _Red;
						_Pnode = _Pnode._Parent._Parent;
					}
					else
					{	// parent has red and black children
						if (_Pnode == _Pnode._Parent._Left)
						{	// rotate left child to right
							_Pnode = _Pnode._Parent;
							_Rrotate(_Pnode);
						}
						_Color(_Pnode._Parent) = _Black;	// propagate red up
						_Color(_Pnode._Parent._Parent) = _Red;
						_Lrotate(_Pnode._Parent._Parent);
					}
				}

			_Root._Color = Color._Black;	// root is always black
			return (iterator(_Newnode));
		}

		protected Node _Lbound(object _Keyval)
		{	// find leftmost node not less than _Keyval
			Node _Pnode = _Root();
			Node _Wherenode = _Myhead;	// end() if search fails

			while (!_Isnil(_Pnode))
				if (this.comp(_Key(_Pnode), _Keyval))
					_Pnode = _Pnode._Right;	// descend right subtree
				else
				{	// _Pnode not less than _Keyval, remember it
					_Wherenode = _Pnode;
					_Pnode = _Pnode._Left;	// descend left subtree
				}

			return (_Wherenode);	// return best remembered candidate
		}

		private Node _Lmost	// return leftmost node in mutable tree
		{	
			get { return _Myhead._Left; }
			set { _Myhead._Left = value; }
		}


		protected void _Lrotate(Node _Wherenode)
		{	// promote right node to root of subtree
			Node _Pnode = _Wherenode._Right;
			_Wherenode._Right = _Pnode._Left;

			if (!_Isnil(_Pnode._Left))
				_Pnode._Left._Parent = _Wherenode;
			_Pnode._Parent = _Wherenode._Parent;

			if (_Wherenode == _Root())
				_Root() = _Pnode;
			else if (_Wherenode == _Wherenode._Parent._Left)
				_Wherenode._Parent._Left = _Pnode;
			else
				_Wherenode._Right._Parent = _Pnode;

			_Pnode._Left = _Wherenode;
			_Wherenode._Parent = _Pnode;
		}

		protected static Node _Max(Node _Pnode)
		{	// return rightmost node in subtree at _Pnode
			while (!_Isnil(_Pnode._Right))
				_Pnode = _Pnode._Right;
			return (_Pnode);
		}

		protected static Node _Min(Node _Pnode)
		{	// return leftmost node in subtree at _Pnode
			while (!_Isnil(_Pnode._Left))
				_Pnode = _Pnode._Left;
			return (_Pnode);
		}

		private Node _Rmost
		{	// return rightmost node in mutable tree
			get {return _Myhead._Right; }
			set { _Myhead._Right = value; }
		}

		private Node _Root
		{
			get { return _Myhead._Parent; }
			set { _Myhead._Parent = value; }
		}


		private void _Rrotate(Node _Wherenode)
		{	// promote left node to root of subtree
			Node _Pnode = _Wherenode._Left;
			_Wherenode._Left = _Pnode._Right;

			if (!_Isnil(_Pnode._Right))
				_Pnode._Right._Parent = _Wherenode;
			_Pnode._Parent = _Wherenode._Parent;

			if (_Wherenode == _Root)
				_Root() = _Pnode;
			else if (_Wherenode == _Wherenode._Parent._Right)
				_Wherenode._Parent._Right = _Pnode;
			else
				_Wherenode._Parent._Left = _Pnode;

			_Pnode._Right = _Wherenode;
			_Wherenode._Parent = _Pnode;
		}

		private Node _Ubound(object _Keyval)
		{	// find leftmost node greater than _Keyval
			Node _Pnode = _Root();
			Node _Wherenode = _Myhead;	// end() if search fails

			while (!_Isnil(_Pnode))
				if (this.comp(_Keyval, _Key(_Pnode)))
				{	// _Pnode greater than _Keyval, remember it
					_Wherenode = _Pnode;
					_Pnode = _Pnode._Left;	// descend left subtree
				}
				else
					_Pnode = _Pnode._Right;	// descend right subtree

			return (_Wherenode);	// return best remembered candidate
		}

		private Node _Buynode()
		{	
			Node n = new Node();
			n._Color = Color._Black;
			n._Isnil = false;
		}

		private Node _Buynode(Node _Larg, Node _Parg, Node _Rarg,
			DictionaryEntry _Val, Color _Carg)
		{
			return new  _Node(_Larg, _Parg, _Rarg, _Val, _Carg);
		}
	}
}

