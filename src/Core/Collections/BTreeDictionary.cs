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
    /// SortedList have a complexity of O(n^2), the BtreeDictionary is 
    /// organized as a B+tree, and this has a complexity of O(n log n). 
    /// In addition, benchmark measurements show that BTreeDictionary is
    /// about twice as fast as SortedDictionary (which also is O(n log n))
    /// and in addition provides the IndexOf functionality from SortedList
    /// that SortedDictionary lacks.
    /// </remarks>
    public class BTreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Node? root;
        private int version;
        private readonly int InternalNodeChildren;
        private readonly int LeafNodeChildren;
        private readonly KeyCollection keyCollection;
        private readonly ValueCollection valueCollection;

        /// <summary>
        /// Constructs an empty <see cref="Collections.BTreeDictionary{TKey, TValue}"/>.
        /// </summary>
        public BTreeDictionary() :
            this(Comparer<TKey>.Default) {
        }

        /// <summary>
        /// Constructs an empty <see cref="Collections.BTreeDictionary{TKey, TValue}"/>
        /// using the given <see cref="IComparer{TKey}"/> to use for comparing keys.
        /// </summary>
        public BTreeDictionary(IComparer<TKey> cmp)
        {
            this.Comparer = cmp ?? throw new ArgumentNullException(nameof(cmp));
            this.version = 0;
            this.InternalNodeChildren = 16;
            this.LeafNodeChildren = InternalNodeChildren - 1;
            this.keyCollection = new KeyCollection(this);
            this.valueCollection = new ValueCollection(this);
        }

        /// <summary>
        /// Creates a <see cref="Collections.BTreeDictionary{TKey, TValue}"/> and
        /// populates it with values from the given <paramref name="entries"/> dictionary.
        /// </summary>
        /// <param name="entries">Initial values.</param>
        public BTreeDictionary(IDictionary<TKey,TValue> entries) :
            this()
        {
            if (entries is null)
                throw new ArgumentNullException(nameof(entries));
            Populate(entries);
        }

        /// <summary>
        /// Creates a <see cref="Collections.BTreeDictionary{TKey, TValue}"/>
        /// with the given <see cref="IComparer{TKey}"/> and
        /// populates it with values from the given <paramref name="entries"/> dictionary.
        /// </summary>
        /// <param name="entries">Initial values.</param>
        /// <param name="comparer">Comparer to use.</param>
        public BTreeDictionary(IDictionary<TKey,TValue> entries, IComparer<TKey> comparer) :
            this(comparer)
        {
            if (entries is null)
                throw new ArgumentNullException(nameof(entries));
            Populate(entries);
        }

        private abstract class Node
        {
            public int count;       // # of direct children
            public int totalCount;  // # of recursively reachable children.
            public TKey[] keys;

            public Node(TKey[] keys)
            {
                this.keys = keys;
            }

            public abstract (Node, Node?) Put(TKey key, TValue value, bool setting, BTreeDictionary<TKey, TValue> tree);

            public abstract (TValue, bool) Get(TKey key, BTreeDictionary<TKey, TValue> tree);

            public abstract bool Remove(TKey key, BTreeDictionary<TKey, TValue> tree);

            public override string ToString()
            {
                return $"{GetType().Name}: {count} items; keys: {string.Join(",",keys)}.";
            }
        }

        /// <summary>
        /// In a B+Tree, the values are held in the leaf nodes of the data structure.
        /// </summary>
        private class LeafNode : Node 
        {
            public LeafNode? nextLeaf;   // leaves are threaded together for ease of enumeration.
            public TValue[] values;

            public LeafNode(int children) : base(new TKey[children])
            {
                this.values = new TValue[children];
            }

            public override (TValue, bool) Get(TKey key, BTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 0, count, key, tree.Comparer);
                if (idx < 0)
                    return (default(TValue)!, false);
                return (values[idx], true);
            }

            public override (Node, Node?) Put(TKey key, TValue value, bool setting, BTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 0, count, key, tree.Comparer);
                if (idx >= 0)
                {
                    if (!setting)
                        throw new ArgumentException("Duplicate key.");
                    values[idx] = value;
                    return (this, null);
                }
                else
                {
                    return Insert(~idx, key, value, tree);
                }
            }

            public override bool Remove(TKey key, BTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 0, count, key, tree.Comparer);
                if (idx >= 0)
                {
                    --count;
                    --totalCount;
                    Array.Copy(keys, idx + 1, keys, idx, count - idx);
                    Array.Copy(values, idx + 1, values, idx, count - idx);
                    keys[count] = default!;
                    values[count] = default!;
                    return true;
                }
                return false;
            }

            private (Node, Node?) Insert(int idx, TKey key, TValue value, BTreeDictionary<TKey, TValue> tree)
            {
                if (count == keys.Length)
                {
                    var newRight = SplitAndInsert(key, value, tree);
                    return (this, newRight);
                }
                else if (idx < count)
                {
                    // Make a hole
                    Array.Copy(keys, idx, keys, idx + 1, count - idx);
                    Array.Copy(values, idx, values, idx + 1, count - idx);
                }
                keys[idx] = key;
                values[idx] = value;
                ++this.count;
                ++this.totalCount;
                return (this, null);
            }

            /// <summary>
            /// Splits this node into subnodes by creating a new "right" node
            /// and adds the (key,value) to the appropriate subnode.
            /// </summary>
            private Node SplitAndInsert(TKey key, TValue value, BTreeDictionary<TKey, TValue> tree)
            {
                var iSplit = (count + 1) / 2;
                var right = new LeafNode(tree.LeafNodeChildren);
                right.count = count - iSplit;
                this.count = iSplit;
                right.totalCount = right.count;
                this.totalCount = this.count;
                Array.Copy(this.keys, iSplit, right.keys, 0, right.count);
                Array.Clear(this.keys, iSplit, right.count);
                Array.Copy(this.values, iSplit, right.values, 0, right.count);
                Array.Clear(this.values, iSplit, right.count);
                right.nextLeaf = this.nextLeaf;
                this.nextLeaf = right;
                if (tree.Comparer.Compare(right.keys[0], key) < 0)
                    right.Put(key, value, false, tree);
                else
                    this.Put(key, value, false, tree);
                return right;
            }
        }

        /// <summary>
        /// In a B+tree, the internals node only contain links to other nodes.
        /// </summary>
        private class InternalNode : Node
        {
            public Node[] nodes;

            public InternalNode(int children) : base(new TKey[children])
            {
                this.nodes = new Node[children];
            }

            public override (TValue, bool) Get(TKey key, BTreeDictionary<TKey, TValue> tree)
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

            public override (Node, Node?) Put(TKey key, TValue value, bool setting, BTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 1, count-1, key, tree.Comparer);
                int iPos = (idx >= 0)
                    ? idx
                    : (~idx) - 1;
                var subnode = nodes[iPos];
                var (leftNode, rightNode) = subnode.Put(key, value, setting, tree);
                if (rightNode is null)
                {
                    this.totalCount = SumNodeCounts(this.nodes, this.count);
                    return (leftNode, null);
                }
                else
                {
                    return Insert(iPos + 1, rightNode.keys[0], rightNode, tree);
                }
            }

            public Node AddNode(TKey key, Node node, BTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 1, count-1, key, tree.Comparer);
                if (idx >= 0)
                    throw new ArgumentException("Duplicate key.");
                return Insert(~idx, node.keys[0], node, tree).Item1;
            }

            public override bool Remove(TKey key, BTreeDictionary<TKey, TValue> tree)
            {
                int idx = Array.BinarySearch(keys, 1, count - 1, key, tree.Comparer);
                bool removed;
                if (idx >= 0)
                {
                    removed = nodes[idx].Remove(key, tree);
                }
                else
                {
                    var iPos = (~idx) - 1;
                    removed = nodes[iPos].Remove(key, tree);
                }
                if (removed)
                {
                    --this.totalCount;
                }
                return removed;
            }

            private (Node, Node?) Insert(int idx, TKey key, Node node, BTreeDictionary<TKey, TValue> tree)
            {
                if (count == keys.Length)
                {
                    var newRight = SplitAndInsert(key, node, tree);
                    return (this, newRight);
                }
                if (idx < count)
                {
                    Array.Copy(keys, idx, keys, idx + 1, count - idx);
                    Array.Copy(nodes, idx, nodes, idx + 1, count - idx);
                }
                keys[idx] = key;
                nodes[idx] = node;
                ++this.count;
                this.totalCount = SumNodeCounts(this.nodes, this.count);
                return (this, null);
            }

            private Node SplitAndInsert(TKey key, Node node, BTreeDictionary<TKey, TValue> tree)
            {
                var iSplit = (count + 1) / 2;
                var right = new InternalNode(tree.InternalNodeChildren);
                right.count = count - iSplit;
                this.count = iSplit;
                Array.Copy(this.keys, iSplit, right.keys, 0, right.count);
                Array.Clear(this.keys, iSplit, right.count);
                Array.Copy(this.nodes, iSplit, right.nodes, 0, right.count);
                Array.Clear(this.nodes, iSplit, right.count);
                if (tree.Comparer.Compare(right.keys[0], key) < 0)
                {
                    right.AddNode(key, node, tree);
                    this.totalCount = SumNodeCounts(this.nodes, this.count);
                }
                else
                {
                    this.AddNode(key, node, tree);
                    right.totalCount = SumNodeCounts(right.nodes, right.count);
                }
                return right;
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
                ++version;
                // Validate(root);
            }
        }

        /// <summary>
        /// <see cref="IComparer{TKey}"/> instance used to compare keys.
        /// </summary>
        public IComparer<TKey> Comparer { get; }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// The keys of this collection.
        /// </summary>
        public KeyCollection Keys => keyCollection;

        /// <summary>
        /// The values of this collection.
        /// </summary>
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
            if (right is not null)
                root = NewInternalRoot(left, right);
            ++version;
            // Validate(root);
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

        /// <summary>
        /// Returns true if the BTreeDictionary contains the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value whose presence is to be tested.</param>
        /// <returns>True if the value is present, false otherwise.</returns>
        /// <remarks>This method is O(n) and will be slow on large collections.</remarks>
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
            for (node = root; node is InternalNode intern; node = intern.nodes[0])
                ;
            var leaf = (LeafNode?)node;
            int myVersion = this.version;
            while (leaf is not null)
            {
                for (int i = 0; i < leaf.count; ++i)
                {
                    if (myVersion != this.version)
                        throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                    yield return new KeyValuePair<TKey, TValue>(leaf.keys[i], leaf.values[i]);
                }
                leaf = leaf.nextLeaf;
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
            if (root.Remove(key, this))
            {
                ++this.version;
                return true;
            }
            else
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.Remove(KeyValuePair<TKey, TValue> item)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EnsureRoot()
        {
            if (root is not null)
                return;
            root = new LeafNode(LeafNodeChildren);
        }

        private KeyValuePair<TKey,TValue> GetEntry(int index)
        {
            if (root is not null && 0 <= index && index < this.Count)
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
            var intern = new InternalNode(InternalNodeChildren);
            intern.count = 2;
            intern.totalCount = left.totalCount + right.totalCount;
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
            if (node is LeafNode leaf)
            {
                if (leaf.totalCount != leaf.count)
                    throw new InvalidOperationException($"Leaf node {leaf} has mismatched counts.");
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
        /// Abstract collection for items in the BTreeDictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class Collection<T> : ICollection<T>
        {
            /// <summary>
            /// BTree nodes for this level.
            /// </summary>
            protected readonly BTreeDictionary<TKey, TValue> btree;

            /// <summary>
            /// Initializes the btree nodes for this collection.
            /// </summary>
            /// <param name="btree"></param>
            protected Collection(BTreeDictionary<TKey, TValue> btree)
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
        /// Collection of keys in the BTreeDictionary.
        /// </summary>
        public class KeyCollection : Collection<TKey>
        {
            internal KeyCollection(BTreeDictionary<TKey, TValue> btree) : 
                base(btree)
            {
            }

            /// <inheritdoc/>
            public override TKey this[int index] => btree.GetEntry(index).Key;

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

            /// <summary>
            /// Index of the given key.
            /// </summary>
            /// <param name="item">Key whose index we are seeking.</param>
            /// <returns>The index of the key, or -1 if it isn't present.</returns>
            public int IndexOf(TKey item) => btree.IndexOfKey(item);

            /// <inheritdoc/>
            public override IEnumerator<TKey> GetEnumerator() => btree.Select(e => e.Key).GetEnumerator();
        }

        /// <summary>
        /// A collection of <typeparamref name="TValue"/> items.
        /// </summary>
        public class ValueCollection : Collection<TValue>
        {
            internal ValueCollection(BTreeDictionary<TKey, TValue> btree) :
                base(btree)
            {
            }

            /// <summary>
            /// Gets the entry at index <paramref name="index"/>.
            /// </summary>
            /// <param name="index">Index of the entry to retrieve.</param>
            /// <returns>The retrieved value.</returns>
            public override TValue this[int index] => btree.GetEntry(index).Value;

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
                    ++iDst;
                }
            }

            /// <inheritdoc/>
            public override IEnumerator<TValue> GetEnumerator() => btree.Select(e => e.Value).GetEnumerator();
        }
    }
}
