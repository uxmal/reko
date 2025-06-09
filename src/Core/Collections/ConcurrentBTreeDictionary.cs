#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted by the keys
    /// and are accessible by key and by index. It's intended to be a drop-in
    /// replacement for <see cref="System.Collections.Generic.SortedList{TKey, TValue}"/>.
    /// </summary>
    /// <remarks>
    /// This class implemements most of the same API as <see cref="System.Collections.Generic.SortedList{TKey, TValue}"/>
    /// but with much better performance. Where n random insertions into a 
    /// SortedList have a complexity of O(n^2), the ConcurrentBTreeDictionary is 
    /// organized as a B+tree, and this has a complexity of O(n log n). 
    /// In addition, benchmark measurements show that ConcurrentBTreeDictionary is
    /// about twice as fast as SortedDictionary (which also is O(n log n))
    /// and in addition provides the IndexOf functionality from SortedList
    /// that SortedDictionary lacks.
    /// </remarks>
    public class ConcurrentBTreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static readonly TraceSwitch trace = new TraceSwitch("ConcurrentBTreeDictionary", "Traces the concurrent BTree dictionary") { Level = TraceLevel.Error };

        private Node? root;
        private int version;
        private readonly int InternalNodeChildren;
        private readonly int LeafNodeChildren;
        private readonly KeyCollection keyCollection;
        private readonly ValueCollection valueCollection;

        /// <summary>
        /// Creates an empty concurrent BTree dictionary.
        /// </summary>
        public ConcurrentBTreeDictionary() :
            this(Comparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Creates an empty concurrent BTree dictionary with the specified comparer.
        /// </summary>
        /// <param name="cmp">Comparer used to compare elements.</param>
        public ConcurrentBTreeDictionary(IComparer<TKey> cmp)
        {
            this.Comparer = cmp ?? throw new ArgumentNullException(nameof(cmp));
            this.version = 0;
            this.InternalNodeChildren = 16;
            this.LeafNodeChildren = InternalNodeChildren - 1;
            this.keyCollection = new KeyCollection(this);
            this.valueCollection = new ValueCollection(this);
        }

        /// <summary>
        /// Creates a copy from another concurrent btree dictionary.
        /// </summary>
        /// <param name="that"></param>
        public ConcurrentBTreeDictionary(ConcurrentBTreeDictionary<TKey, TValue> that)
        {
            this.Comparer = that.Comparer;
            this.version = 0;
            this.InternalNodeChildren = that.InternalNodeChildren;
            this.LeafNodeChildren = that.LeafNodeChildren;
            this.keyCollection = new KeyCollection(this);
            this.valueCollection = new ValueCollection(this);
            this.root = that.root;  // Snapshot the old tree.
        }

        /// <summary>
        /// Creates a concurrent BTree dictionary from another dictionary.
        /// </summary>
        /// <param name="entries"> Dictionary to copy from.</param>
        public ConcurrentBTreeDictionary(IDictionary<TKey, TValue> entries) :
            this()
        {
            if (entries is null)
                throw new ArgumentNullException(nameof(entries));
            Populate(entries);
        }

        /// <summary>
        /// Creates a concurrent BTree dictionary from another dictionary, using the specified comparer.
        /// </summary>
        /// <param name="entries"> Dictionary to copy from.</param>
        /// <param name="comparer">Comparer used to compare elements.</param>
        public ConcurrentBTreeDictionary(IDictionary<TKey, TValue> entries, IComparer<TKey> comparer) :
            this(comparer)
        {
            if (entries is null)
                throw new ArgumentNullException(nameof(entries));
            Populate(entries);
        }

        private abstract class Node
        {
            public readonly TKey[] keys;
            public readonly int count;       // # of direct children
            public readonly int totalCount;  // # of recursively reachable children.

            protected Node(TKey[] keys, int count, int totalCount)
            {
                this.keys = keys;
                this.count = count;
                this.totalCount = totalCount;
            }

            public abstract (Node, Node?) Put(TKey key, TValue value, bool setting, ConcurrentBTreeDictionary<TKey, TValue> tree);

            public abstract (TValue, bool) Get(TKey key, ConcurrentBTreeDictionary<TKey, TValue> tree);

            /// <summary>
            /// Removes the key from the node.
            /// </summary>
            /// <param name="key">Key to remove</param>
            /// <param name="tree">The ConcurrentBTreeDictionary</param>
            /// <returns>A new node if <paramref name="key"/> was found and deleted, otherwise null.</returns>
            public abstract Node? Remove(TKey key, ConcurrentBTreeDictionary<TKey, TValue> tree);

            public override string ToString()
            {
                return $"{GetType().Name}: {count} items; keys: {string.Join(",", keys.Take(count))}.";
            }
        }

        /// <summary>
        /// In a B+Tree, the values are held in the leaf nodes of the data structure.
        /// </summary>
        private class LeafNode : Node
        {
            public readonly TValue[] values;

            public LeafNode(TKey[] keys, TValue[] values, int count, int totalCount) :
                base(keys, count, totalCount)
            {
                this.values = values;
            }

            public override (TValue, bool) Get(TKey key, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 0, count, key, tree.Comparer);
                if (idx < 0)
                    return (default(TValue)!, false);
                return (values[idx], true);
            }

            public override (Node, Node?) Put(TKey key, TValue value, bool setting, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 0, count, key, tree.Comparer);
                if (idx >= 0)
                {
                    if (!setting)
                        throw new ArgumentException("Duplicate key.");
                    var nKeys = new TKey[tree.LeafNodeChildren];
                    var nValues = new TValue[tree.LeafNodeChildren];
                    Array.Copy(this.keys, 0, nKeys, 0, this.count);
                    Array.Copy(this.values, 0, nValues, 0, this.count);
                    nValues[idx] = value;
                    var newNode = new LeafNode(nKeys, nValues, this.count, this.totalCount);
                    return (newNode, null);
                }
                else
                {
                    return Insert(~idx, key, value, tree);
                }
            }

            public override Node? Remove(TKey key, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 0, count, key, tree.Comparer);
                if (idx >= 0)
                {
                    var nKeys = new TKey[this.keys.Length];
                    var nValues = new TValue[this.values.Length];
                    var nCount = this.count - 1;
                    if (idx > 0)
                    {
                        Array.Copy(keys, 0, nKeys, 0, idx);
                        Array.Copy(values, 0, nValues, 0, idx);
                    }
                    Array.Copy(keys, idx + 1, nKeys, idx, nCount - idx);
                    Array.Copy(values, idx + 1, nValues, idx, nCount - idx);
                    var newNode = new LeafNode(nKeys, nValues, nCount, nCount);
                    return newNode;
                }
                return null;
            }

            private (Node, Node?) Insert(int idx, TKey key, TValue value, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                if (count == keys.Length)
                {
                    return SplitAndInsert(key, value, tree);
                }
                var nKeys = new TKey[tree.LeafNodeChildren];
                var nValues = new TValue[tree.LeafNodeChildren];
                Array.Copy(keys, 0, nKeys, 0, idx);
                Array.Copy(values, 0, nValues, 0, idx);
                if (idx < count)
                {
                    // Leave a hole at position idx.
                    Array.Copy(keys, idx, nKeys, idx + 1, count - idx);
                    Array.Copy(values, idx, nValues, idx + 1, count - idx);
                }
                nKeys[idx] = key;
                nValues[idx] = value;
                var newNode = new LeafNode(nKeys, nValues, this.count + 1, this.totalCount + 1);
                return (newNode, null);
            }

            /// <summary>
            /// Splits this node into subnodes by creating a new "left" and 
            /// a new "right" node and adds the (key,value) to the appropriate subnode.
            /// </summary>
            private (Node, Node) SplitAndInsert(TKey key, TValue value, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                var iSplit = (this.count + 1) / 2;
                var leftCount = iSplit;
                var rightCount = this.count - iSplit;
                var lKeys = new TKey[tree.LeafNodeChildren];
                var rKeys = new TKey[tree.LeafNodeChildren];
                var lValues = new TValue[tree.LeafNodeChildren];
                var rValues = new TValue[tree.LeafNodeChildren];
                Array.Copy(this.keys, 0, lKeys, 0, leftCount);
                Array.Copy(this.keys, iSplit, rKeys, 0, rightCount);
                Array.Copy(this.values, 0, lValues, 0, leftCount);
                Array.Copy(this.values, iSplit, rValues, 0, rightCount);
                TKey[] nKeys;
                TValue[] nValues;
                int count;
                if (tree.Comparer.Compare(rKeys[0], key) < 0)
                {
                    nKeys = rKeys;
                    nValues = rValues;
                    count = rightCount;
                    ++rightCount;
                }
                else
                {
                    nKeys = lKeys;
                    nValues = lValues;
                    count = leftCount;
                    ++leftCount;
                }

                // Find the place where the item would be if it had been present.
                int idx = Array.BinarySearch(nKeys, 0, count, key, tree.Comparer);
                if (idx >= 0)
                    throw new ArgumentException("Duplicate key.");
                idx = ~idx;
                if (idx < count)
                {
                    // Make a 'hole' if the place is not at the end of the items.
                    Array.Copy(nKeys, idx, nKeys, idx + 1, count - idx);
                    Array.Copy(nValues, idx, nValues, idx + 1, count - idx);
                }
                nKeys[idx] = key;
                nValues[idx] = value;

                var left = new LeafNode(lKeys, lValues, leftCount, leftCount);
                var right = new LeafNode(rKeys, rValues, rightCount, rightCount);
                return (left, right);
            }
        }

        /// <summary>
        /// In a B+tree, the internals node only contain links to other nodes.
        /// </summary>
        private class InternalNode : Node
        {
            public readonly Node[] nodes;

            public InternalNode(TKey[] keys, Node[] nodes, int count, int totalCount) :
                base(keys, count, totalCount)
            {
                this.nodes = nodes;
            }

            public override (TValue, bool) Get(TKey key, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 1, count - 1, key, tree.Comparer);
                if (idx >= 0)
                    return nodes[idx].Get(key, tree);
                else
                {
                    var iPos = (~idx) - 1;
                    return nodes[iPos].Get(key, tree);
                }
            }

            public override (Node, Node?) Put(TKey key, TValue value, bool setting, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 1, count - 1, key, tree.Comparer);
                int iPos = (idx >= 0)
                    ? idx
                    : (~idx) - 1;
                var subnode = nodes[iPos];
                var (leftNode, rightNode) = subnode.Put(key, value, setting, tree);
                if (rightNode is null)
                {
                    var nKeys = new TKey[tree.InternalNodeChildren];
                    var nNodes = new Node[tree.InternalNodeChildren];
                    Array.Copy(this.keys, 0, nKeys, 0, this.count);
                    Array.Copy(this.nodes, 0, nNodes, 0, this.count);
                    nNodes[iPos] = leftNode;
                    var newNode = new InternalNode(nKeys, nNodes, this.count, SumNodeCounts(nNodes, this.count));
                    return (newNode, null);
                }
                else
                {
                    return Insert(iPos + 1, rightNode.keys[0], leftNode, rightNode, tree);
                }
            }

            public override Node? Remove(TKey key, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 1, count - 1, key, tree.Comparer);
                int iPos;
                if (idx >= 0)
                {
                    iPos = idx;
                }
                else
                {
                    iPos = (~idx) - 1;
                }
                var removed = nodes[iPos].Remove(key, tree);
                if (removed is not null)
                {
                    var nKeys = new TKey[tree.InternalNodeChildren];
                    var nNodes = new Node[tree.InternalNodeChildren];
                    Array.Copy(this.keys, 0, nKeys, 0, this.count);
                    Array.Copy(this.nodes, 0, nNodes, 0, this.count);
                    nNodes[iPos] = removed;
                    removed = new InternalNode(nKeys, nNodes, this.count, this.totalCount - 1);
                }
                return removed;
            }

            private (Node, Node?) Insert(int idx, TKey key, Node leftNode, Node node, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                if (count == keys.Length)
                {
                    return SplitAndInsert(key, idx, leftNode, node, tree);
                }
                var nKeys = new TKey[tree.InternalNodeChildren];
                var nNodes = new Node[tree.InternalNodeChildren];
                Array.Copy(this.keys, 0, nKeys, 0, idx);
                Array.Copy(this.nodes, 0, nNodes, 0, idx);

                if (idx < count)
                {
                    // Leave a 'hole' at position idx.
                    Array.Copy(this.keys, idx, nKeys, idx + 1, count - idx);
                    Array.Copy(this.nodes, idx, nNodes, idx + 1, count - idx);
                }
                nNodes[idx - 1] = leftNode;
                nKeys[idx] = key;
                nNodes[idx] = node;
                int nCount = this.count + 1;
                var newNode = new InternalNode(nKeys, nNodes, this.count + 1, SumNodeCounts(nNodes, nCount));
                return (newNode, null);
            }

            /// <summary>
            /// Make a pair of new nodes by partitioning the subnodes of the current node.
            /// </summary>
            private (Node, Node) SplitAndInsert(TKey key, int iLeft, Node leftNode, Node node, ConcurrentBTreeDictionary<TKey, TValue> tree)
            {
                var iSplit = (this.count + 1) / 2;
                var lKeys = new TKey[tree.InternalNodeChildren];
                var rKeys = new TKey[tree.InternalNodeChildren];
                var lNodes = new Node[tree.InternalNodeChildren];
                var rNodes = new Node[tree.InternalNodeChildren];
                var leftCount = iSplit;
                var rightCount = this.count - iSplit;
                Array.Copy(this.keys, 0, lKeys, 0, leftCount);
                Array.Copy(this.keys, iSplit, rKeys, 0, rightCount);
                Array.Copy(this.nodes, 0, lNodes, 0, leftCount);
                Array.Copy(this.nodes, iSplit, rNodes, 0, rightCount);
                if (iLeft-1 < iSplit)
                {
                    lNodes[iLeft-1] = leftNode;
                }
                else
                {
                    rNodes[iLeft - (iSplit+1)] = leftNode;
                }
                TKey[] nKeys;
                Node[] nNodes;
                int count;
                if (tree.Comparer.Compare(rKeys[0], key) < 0)
                {
                    nKeys = rKeys;
                    nNodes = rNodes;
                    count = rightCount;
                    ++rightCount;
                }
                else
                {
                    nKeys = lKeys;
                    nNodes = lNodes;
                    count = leftCount;
                    ++leftCount;
                }

                int idx = Array.BinarySearch(nKeys, 1, count - 1, key, tree.Comparer);
                if (idx >= 0)
                    throw new ArgumentException("Duplicate key.");
                idx = ~idx;
                if (idx < count)
                {
                    // Leave a 'hole' at position idx.
                    Array.Copy(nKeys, idx, nKeys, idx + 1, count - idx);
                    Array.Copy(nNodes, idx, nNodes, idx + 1, count - idx);
                }
                nKeys[idx] = key;
                nNodes[idx] = node;

                var left = new InternalNode(lKeys, lNodes, leftCount, SumNodeCounts(lNodes, leftCount));
                var right = new InternalNode(rKeys, rNodes, rightCount, SumNodeCounts(rNodes, rightCount));
                return (left, right);
            }

            private static int SumNodeCounts(Node[] nodes, int count)
            {
                int n = 0;
                for (int i = 0; i < count; ++i)
                    n += nodes[i].totalCount;
                return n;
            }
        }

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                if (root is null)
                    throw new KeyNotFoundException();
                var (value, found) = root.Get(key, this);
                if (!found)
                    throw new KeyNotFoundException();
                return value;
            }

            set
            {
                EnsureRoot();
                var (left, right) = root!.Put(key, value, true, this);
                if (right is not null)
                    root = NewInternalRoot(left, right);
                else
                    root = left;
                ++version;
                Validate(root);
            }
        }

        /// <summary>
        /// Comparer used to compare items.
        /// </summary>
        public IComparer<TKey> Comparer { get; }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <inheritdoc/>
        public KeyCollection Keys => keyCollection;

        /// <inheritdoc/>
        public ValueCollection Values => valueCollection;

        /// <inheritdoc/>
        public int Count => root is not null ? root.totalCount : 0;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            EnsureRoot();
            var (left, right) = root!.Put(key, value, false, this);
            Node newRoot;
            if (right is not null)
                newRoot = NewInternalRoot(left, right);
            else
                newRoot = left;
            root = newRoot;
            ++version;
            Validate(root);
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            ++version;
            root = null;
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (root is null)
                return false;
            var (value, found) = root.Get(item.Key, this);
            return found && object.Equals(item.Value, value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            if (root is null)
                return false;
            var (_, found) = root.Get(key, this);
            return found;
        }

        /// <inheritdoc/>
        public bool ContainsValue(TValue value)
        {
            return this.Any(e => e.Value!.Equals(value));
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (this.Count > array.Length - arrayIndex) throw new ArgumentException();
            int iDst = arrayIndex;
            foreach (var item in this)
            {
                array[iDst++] = item;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (root is null)
                yield break;
            // Get the leftmost leaf node.
            Node node;
            var stack = new Stack<(Node, int)>();
            for (node = root; node is InternalNode intern; node = intern.nodes[0])
                stack.Push((intern, 0));
            stack.Push((node, 0));
            while (stack.Count > 0)
            {
                // Invariant: the top of the stack is a leaf node.

                var (nLeaf, _) = stack.Pop();
                var leaf = (LeafNode)nLeaf;
                for (int i = 0; i < leaf.count; ++i)
                {
                    yield return new KeyValuePair<TKey, TValue>(leaf.keys[i], leaf.values[i]);
                }
                while (stack.Count > 0)
                {
                    // Find a parent who is not exhausted. Then find the deepest leaf node.
                    var (nParent, iParent) = stack.Pop();
                    var parent = (InternalNode)nParent;
                    ++iParent;
                    if (iParent < parent.count)
                    {
                        stack.Push((parent, iParent));
                        for (node = parent.nodes[iParent]; node is InternalNode intern; node = intern.nodes[0])
                            stack.Push((intern, 0));
                        stack.Push((node, 0));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determine the 0-based index of <paramref name="key"/>.
        /// </summary>
        /// <returns>
        /// A non-negative number if the key is found. If the key is 
        /// not found, returns the one's complement of the index the key would
        /// have had if it were present in the collection.
        /// </returns>
        public int IndexOfKey(TKey key)
        {
            if (root is null)
                return ~0;
            int totalBefore = 0;
            Node node = root;
            int i;
            while (node is InternalNode intern)
            {
                for (i = 1; i < intern.count; ++i)
                {
                    int c = Comparer.Compare(intern.keys[i], key);
                    if (c <= 0)
                    {
                        totalBefore += intern.nodes[i - 1].totalCount;
                    }
                    else
                    {
                        node = intern.nodes[i - 1];
                        break;
                    }
                }
                if (i == intern.count)
                {
                    // Key was larger than all nodes.
                    node = intern.nodes[i - 1];
                }
            }
            // Should have reached a leaf node.
            var leaf = (LeafNode)node;
            for (i = 0; i < leaf.count; ++i)
            {
                var c = Comparer.Compare(leaf.keys[i], key);
                if (c == 0)
                    return totalBefore + i;
                if (c > 0)
                    break;
            }
            return ~(totalBefore + i);
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            if (root is null)
                return false;
            var newRoot = root.Remove(key, this);
            if (newRoot is not null)
            {
                root = newRoot;
                ++this.version;
                return true;
            }
            else
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [NotNullWhen(returnValue: true)] out TValue value)
        {
            if (root is null)
            {
                value = default!;
                return false;
            }
            bool found;
            (value, found) = root.Get(key, this);
            return found;
        }

        /// <summary>
        /// Find the first key that is greater than or equal to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key whose lower bound is to be found.</param>
        /// <param name="value">Lower bound retrieved.</param>
        /// <returns>True if a lower bound could be found; otherwise false.
        /// </returns>
        public bool TryGetLowerBound(TKey key, [MaybeNullWhen(returnValue: false)] out TValue value)
        {
            var snapshot = this.root;
            var cmp = this.Comparer;

            value = default!;
            bool set = false;
            if (snapshot is null)
                return set;

            int lo = 0;
            int hi = snapshot.totalCount - 1;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                var kv = GetEntry(snapshot, mid);
                int c = cmp.Compare(kv.Key, key);
                if (c == 0)
                {
                    value = kv.Value;
                    return true;
                }
                if (c < 0)
                {
                    value = kv.Value;
                    set = true;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Find the first key that is less than or equal to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key whose upper bound is to be found.</param>
        /// <param name="value">Upper bound retrieved.</param>
        /// <returns>True if a upper bound could be found; otherwise false.
        /// </returns>
        public bool TryGetUpperBound(TKey key, [MaybeNullWhen(returnValue: false)] out TValue value)
        {
            var snapshot = this.root;
            var cmp = this.Comparer;

            value = default!;
            bool set = false;
            if (snapshot is null)
                return set;

            int lo = 0;
            int hi = snapshot.totalCount - 1;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                var kv = GetEntry(snapshot, mid);
                int c = cmp.Compare(kv.Key, key);
                if (c == 0)
                {
                    value = kv.Value;
                    return true;
                }
                if (c > 0)
                {
                    value = kv.Value;
                    set = true;
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Find the first key that is greater than or equal to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key whose lower bound is to be found.</param>
        /// <param name="closestKey">Lower bound retrieved.</param>
        /// <returns>True if a lower bound could be found; otherwise false.
        /// </returns>
        public bool TryGetLowerBoundKey(TKey key, out TKey closestKey)
        {
            var snapshot = this.root;
            var cmp = this.Comparer;

            closestKey = default!;
            bool set = false;
            if (snapshot is null)
                return set;

            int lo = 0;
            int hi = snapshot.totalCount - 1;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                var kv = GetEntry(snapshot, mid);
                int c = cmp.Compare(kv.Key, key);
                if (c == 0)
                {
                    closestKey = kv.Key;
                    return true;
                }
                if (c < 0)
                {
                    closestKey = kv.Key;
                    set = true;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Find the first key that is less than or equal to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key whose upper bound is to be found.</param>
        /// <param name="closestKey">Upper bound retrieved.</param>
        /// <returns>True if a upper bound could be found; otherwise false.
        /// </returns>
        public bool TryGetUpperBoundKey(TKey key, out TKey closestKey)
        {
            var snapshot = this.root;
            var cmp = this.Comparer;

            closestKey = default!;
            bool set = false;
            if (snapshot is null)
                return set;

            int lo = 0;
            int hi = snapshot.totalCount - 1;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                var kv = GetEntry(snapshot, mid);
                int c = cmp.Compare(kv.Key, key);
                if (c == 0)
                {
                    closestKey = kv.Key;
                    return true;
                }
                if (c < 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    closestKey = kv.Key;
                    set = true;
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Find the index of the first key that is greater than or equal to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key whose lower bound is to be found.</param>
        /// <param name="closestIndex">Lower bound retrieved.</param>
        /// <returns>True if a lower bound could be found; otherwise false.
        /// </returns>
        public bool TryGetLowerBoundIndex(TKey key, out int closestIndex)
        {
            var snapshot = this.root;
            var cmp = this.Comparer;

            closestIndex = -1;
            bool set = false;
            if (snapshot is null)
                return set;

            int lo = 0;
            int hi = snapshot.totalCount - 1;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                var kv = GetEntry(snapshot, mid);
                int c = cmp.Compare(kv.Key, key);
                if (c == 0)
                {
                    closestIndex = mid;
                    return true;
                }
                if (c < 0)
                {
                    lo = mid + 1;
                    closestIndex = mid;
                    set = true;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }

        /// <summary>
        /// Find the index of the first key that is less than or equal to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key whose upper bound is to be found.</param>
        /// <param name="closestIndex">Upper bound retrieved.</param>
        /// <returns>True if a upper bound could be found; otherwise false.
        /// </returns>
        public bool TryGetUpperBoundIndex(TKey key, out int closestIndex)
        {
            var snapshot = this.root;
            var cmp = this.Comparer;

            closestIndex = -1;
            bool set = false;
            if (snapshot is null)
                return set;

            int lo = 0;
            int hi = snapshot.totalCount - 1;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                var kv = GetEntry(snapshot, mid);
                int c = cmp.Compare(kv.Key, key);
                if (c == 0)
                {
                    closestIndex = mid;
                    return true;
                }
                if (c < 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    closestIndex = mid;
                    set = true;
                    hi = mid - 1;
                }
            }
            return set;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EnsureRoot()
        {
            if (root is not null)
                return;
            root = new LeafNode(
                new TKey[LeafNodeChildren],
                new TValue[LeafNodeChildren],
                0, 0);
        }

        private static KeyValuePair<TKey, TValue> GetEntry(Node? root, int index)
        {
            if (root is not null && 0 <= index && index < root.totalCount)
            {
                Node node = root;
                int itemsLeft = index;
                while (node is InternalNode intern)
                {
                    for (int i = 0; i < intern.count; ++i)
                    {
                        var subNode = intern.nodes[i];
                        if (itemsLeft < subNode.totalCount)
                        {
                            node = subNode;
                            break;
                        }
                        itemsLeft -= subNode.totalCount;
                    }
                }
                var leaf = (LeafNode)node;
                return new KeyValuePair<TKey, TValue>(leaf.keys[itemsLeft], leaf.values[itemsLeft]);
            }
            else
                throw new ArgumentOutOfRangeException("Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        private InternalNode NewInternalRoot(Node left, Node right)
        {
            var intern = new InternalNode(
                new TKey[InternalNodeChildren],
                new Node[InternalNodeChildren],
                2,
                left.totalCount + right.totalCount);
            intern.keys[0] = left.keys[0];
            intern.keys[1] = right.keys[0];
            intern.nodes[0] = left;
            intern.nodes[1] = right;
            return intern;
        }

        private void Populate(IDictionary<TKey, TValue> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry.Key, entry.Value);
            }
        }

        #region Debugging code 

        /// <summary>
        /// Debugging code.
        /// </summary>
        [Conditional("DEBUG")]
        public void Dump()
        {
            if (root is null)
                Debug.Print("(empty)");
            Dump(root!, 0);
        }

        /// <summary>
        /// Debugging code.
        /// </summary>
        [Conditional("DEBUG")]
        private void Dump(Node n, int depth)
        {
            var prefix = new string(' ', depth);
            switch (n)
            {
            case InternalNode inode:
                for (int i = 0; i < inode.count; ++i)
                {
                    Debug.Print("{0}{1}: total nodes: {2}", prefix, inode.keys[i], inode.nodes[i].totalCount);
                    Dump(inode.nodes[i], depth + 4);
                }
                break;
            case LeafNode leaf:
                for (int i = 0; i < leaf.count; ++i)
                {
                    Debug.Print("{0}{1}: {2}", prefix, leaf.keys[i], leaf.values[i]);
                }
                break;
            default:
                Debug.Print("{0}huh?", prefix);
                break;
            }
        }

        [Conditional("DEBUG")]
        private void Validate(Node node)
        {
            if (!trace.TraceVerbose)
                return;
            if (node is LeafNode leaf)
            {
                if (leaf.totalCount != leaf.count)
                {
                    Dump();
                    throw new InvalidOperationException($"Leaf node {leaf} has mismatched counts.");
                }
            }
            else if (node is InternalNode intern)
            {
                int sum = 0;
                for (int i = 0; i < intern.count; ++i)
                {
                    Validate(intern.nodes[i]);
                    sum += intern.nodes[i].totalCount;
                }
                if (sum != intern.totalCount)
                {
                    Dump();
                    Console.WriteLine("# of nodes: {0}", this.Count);
                    throw new InvalidOperationException($"Internal node {intern} has mismatched counts; expected {sum} but had {intern.totalCount}.");
                }
            }
        }
        #endregion

        /// <summary>
        /// Abstract collection used for projections.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Collection<T> : ICollection<T>
        {
            /// <summary>
            /// Root of the B-tree.
            /// </summary>
            protected readonly ConcurrentBTreeDictionary<TKey, TValue> btree;

            /// <summary>
            /// Initializes the collection.
            /// </summary>
            /// <param name="btree">Root of the btree.</param>
            protected Collection(ConcurrentBTreeDictionary<TKey, TValue> btree)
            {
                this.btree = btree;
            }

            /// <inheritdoc/>
            public int Count => btree.Count;

            /// <inheritdoc/>
            public bool IsReadOnly => true;

            /// <inheritdoc/>
            public abstract T this[int index] { get; }

            /// <inheritdoc/>
            public void Add(T item)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc/>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc/>
            public abstract bool Contains(T item);

            /// <inheritdoc/>
            public abstract void CopyTo(T[] array, int arrayIndex);

            /// <inheritdoc/>
            public abstract IEnumerator<T> GetEnumerator();

            /// <inheritdoc/>
            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Projection of the keys in the B-tree.
        /// </summary>
        public class KeyCollection : Collection<TKey>
        {
            internal KeyCollection(ConcurrentBTreeDictionary<TKey, TValue> btree) :
                base(btree)
            {
            }

            /// <inheritdoc/>
            public override TKey this[int index] => GetEntry(btree.root, index).Key;

            /// <inheritdoc/>
            public override bool Contains(TKey item) => btree.ContainsKey(item);

            /// <inheritdoc/>
            public override void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array is null) throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (btree.Count > array.Length - arrayIndex) throw new ArgumentException();
                var iDst = arrayIndex;
                foreach (var item in btree)
                {
                    array[iDst++] = item.Key;
                }
            }

            /// <inheritdoc/>
            public int IndexOf(TKey item) => btree.IndexOfKey(item);

            /// <inheritdoc/>
            public override IEnumerator<TKey> GetEnumerator() => btree.Select(e => e.Key).GetEnumerator();
        }

        /// <summary>
        /// Value project of the B-Tree entries.
        /// </summary>
        public class ValueCollection : Collection<TValue>
        {
            internal ValueCollection(ConcurrentBTreeDictionary<TKey, TValue> btree) :
                base(btree)
            {
            }

            /// <inheritdoc/>
            public override TValue this[int index] => GetEntry(btree.root, index).Value;

            /// <inheritdoc/>
            public override bool Contains(TValue item) => btree.ContainsValue(item);

            /// <inheritdoc/>
            public override void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array is null) throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (btree.Count > array.Length - arrayIndex) throw new ArgumentException();
                var iDst = arrayIndex;
                foreach (var item in btree)
                {
                    array[iDst] = item.Value;
                }
            }

            /// <inheritdoc/>
            public override IEnumerator<TValue> GetEnumerator() => btree.Select(e => e.Value).GetEnumerator();
        }
    }
}