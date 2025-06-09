/*
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.netbeans.org/cddl.html
 * or //www.gnu.org/licenses/gpl-2.0.html.
 *
 * When distributing Covered Code, include this CDDL Header Notice in each file
 * and include the License file at http://www.netbeans.org/cddl.txt.
 * If applicable, add the following below the CDDL Header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted [year] [name of copyright owner]"
 *
 * The Original Software is GraphMaker. The Initial Developer of the Original
 * Software is Nathan L. Fiedler. Portions created by Nathan L. Fiedler
 * are Copyright (C) 1999-2008.  All Rights Reserved. 
 *
 * Contributor(s): Nathan L. Fiedler.
 * John Källén
 * 
 * Portions Copyrighted 2011 by Jason Wolfe.
 * Portions Copyrighted 2017-2025 by John Källén.
 * 
 */

using System;
using System.Collections.Generic;

namespace Reko.Core.Collections
{

    /**
     * This class implements a Fibonacci heap data structure. Much of the
     * code in this class is based on the algorithms in Chapter 21 of the
     * "Introduction to Algorithms" by Cormen, Leiserson, Rivest, and Stein.
     * The amortized running time of most of these methods is O(1), making
     * it a very fast data structure. Several have an actual running time
     * of O(1). removeMin() and delete() have O(log n) amortized running
     * times because they do the heap consolidation.
     *
     * <p><strong>Note that this implementation is not synchronized.</strong>
     * If multiple threads access a set concurrently, and at least one of the
     * threads modifies the set, it <em>must</em> be synchronized externally.
     * This is typically accomplished by synchronizing on some object that
     * naturally encapsulates the set.</p>
     *
     * @author  Nathan Fiedler
     */
    public class FibonacciHeap<TKey, TValue>
        where TKey : IComparable<TKey>
        where TValue : notnull
    {
        /** Points to the minimum node in the heap. */
        private Node? min;
        /** Number of nodes in the heap. If the type is ever widened,
         * (e.g. changed to long) then recalcuate the maximum degree
         * value used in the consolidate() method. */
        private int n;

        private Dictionary<TValue, Node> nodes;

        /// <summary>
        /// Creates a new fibonacci heap.
        /// </summary>
        public FibonacciHeap()
        {
            this.nodes = new Dictionary<TValue, Node>();
        }

        /// <summary>
        /// Number of items in the heap.
        /// </summary>
        public int Count => n;

        /**
         * Removes all elements from this heap.
         *
         * <p><em>Running time: O(1)</em></p>
         */
        public void Clear()
        {
            min = null;
            n = 0;
        }

        /**
         * Consolidates the trees in the heap by joining trees of equal
         * degree until there are no more trees of equal degree in the
         * root list.
         *
         * <p><em>Running time: O(log n) amortized</em></p>
         */
        private void Consolidate()
        {
            // The magic 45 comes from log base phi of Integer.MAX_VALUE,
            // which is the most elements we will ever hold, and log base
            // phi represents the largest degree of any root list node.
            Node?[] A = new Node[45];

            // For each root list node look for others of the same degree.
            Node start = min!;
            Node w = min!;
            do
            {
                Node x = w;
                // Because x might be moved, save its sibling now.
                Node nextW = w.right;
                int d = x.degree;
                while (A[d] is not null)
                {
                    // Make one of the nodes a child of the other.
                    Node y = A[d]!;
                    if (x.getKey.CompareTo(y.getKey) > 0)
                    {
                        Node temp = y;
                        y = x;
                        x = temp;
                    }
                    if (y == start)
                    {
                        // Because removeMin() arbitrarily assigned the min
                        // reference, we have to ensure we do not miss the
                        // end of the root node list.
                        start = start.right;
                    }
                    if (y == nextW)
                    {
                        // If we wrapped around we need to check for this case.
                        nextW = nextW.right;
                    }
                    // Node y disappears from root list.
                    y.Link(x);
                    // We've handled this degree, go to next one.
                    A[d] = null;
                    d++;
                }
                // Save this node for later when we might encounter another
                // of the same degree.
                A[d] = x;
                // Move forward through list.
                w = nextW;
            } while (w != start);

            // The node considered to be min may have been changed above.
            min = start;
            // Find the minimum key again.
            foreach (Node? a in A)
            {
                if (a is not null && a.getKey.CompareTo(min.getKey) < 0)
                {
                    min = a;
                }
            }
        }

        /// <summary>
        /// "Decrease the key".
        /// </summary>
        /// <param name="x"></param>
        /// <param name="k"></param>
        public void decreaseKey(TValue x, TKey k)
        {
            DecreaseKey(nodes[x], x, k);
        }

        /**
         * Decreases the key value for a heap node, given the new value
         * to take on. The structure of the heap may be changed, but will
         * not be consolidated.
         *
         * <p><em>Running time: O(1) amortized</em></p>
         *
         * @param  x  node to decrease the key of
         * @param  k  new key value for node x
         * @exception  IllegalArgumentException
         *             if k is larger than x.key value.
         */
        public void DecreaseKey(Node x, TValue newData, TKey k)
        {
            decreaseKey(x, newData, k, false);
        }

        /**
         * Decrease the key value of a node, or simply bubble it up to the
         * top of the heap in preparation for a delete operation.
         *
         * @param  x       node to decrease the key of.
         * @param  k       new key value for node x.
         * @param  delete  true if deleting node (in which case, k is ignored).
         */
        private void decreaseKey(Node x, TValue newData, TKey k, bool delete)
        {
            if (!delete && k.CompareTo(x.getKey) > 0)
            {
                throw new InvalidOperationException("Cannot increase key value.");
            }
            x.getKey = k;
            x.getData = newData;
            Node? y = x.parent;
            if (y is not null && (delete || k.CompareTo(y.getKey) < 0))
            {
                y.Cut(x, min!);
                y.CascadingCut(min!);
            }
            if (delete || k.CompareTo(min!.getKey) < 0)
            {
                min = x;
            }
        }

        /**
         * Deletes a node from the heap given the reference to the node.
         * The trees in the heap will be consolidated, if necessary.
         *
         * <p><em>Running time: O(log n) amortized</em></p>
         *
         * @param  x  node to remove from heap.
         */
        public void Delete(Node x)
        {
            // make x as small as possible
            decreaseKey(x, x.getData, default!, true);
            // remove the smallest, which decreases n also
            removeMin();
        }

        /**
         * Tests if the Fibonacci heap is empty or not. Returns true if
         * the heap is empty, false otherwise.
         *
         * <p><em>Running time: O(1)</em></p>
         *
         * @return  true if the heap is empty, false otherwise.
         */
        public bool IsEmpty => min is null;

        /**
         * Inserts a new data element into the heap. No heap consolidation
         * is performed at this time, the new node is simply inserted into
         * the root list of this heap.
         *
         * <p><em>Running time: O(1)</em></p>
         *
         * @param  x    data object to insert into heap.
         * @param  key  key value associated with data object.
         * @return newly created heap node.
         */
        public Node Insert(TValue x, TKey key)
        {
            Node node = new Node(x, key);
            nodes.Add(x, node);
            // concatenate node into min list
            if (min is not null)
            {
                node.right = min;
                node.left = min.left;
                min.left = node;
                node.left.right = node;
                if (key.CompareTo(min.getKey) < 0)
                {
                    min = node;
                }
            }
            else
            {
                min = node;
            }
            n++;
            return node;
        }

        /**
         * Returns the smallest element in the heap. This smallest element
         * is the one with the minimum key value.
         *
         * <p><em>Running time: O(1)</em></p>
         *
         * @return  heap node with the smallest key, or null if empty.
         */
        public Node? Minimum()
        {
            return min;
        }

        /**
         * Removes the smallest element from the heap. This will cause
         * the trees in the heap to be consolidated, if necessary.
         *
         * <p><em>Running time: O(log n) amortized</em></p>
         *
         * @return  data object with the smallest key.
         */
        public TValue removeMin()
        {
            Node? z = min;
            if (z is null)
                throw new InvalidOperationException("Empty");
            if (z.child is not null)
            {
                z.child.parent = null;
                // for each child of z do...
                for (Node x = z.child.right; x != z.child; x = x.right)
                {
                    // set parent[x] to null
                    x.parent = null;
                }
                // merge the children into root list
                Node minleft = min!.left;
                Node zchildleft = z.child.left;
                min.left = zchildleft;
                zchildleft.right = min;
                z.child.left = minleft;
                minleft.right = z.child;
            }
            // remove z from root list of heap
            z.left.right = z.right;
            z.right.left = z.left;
            if (z == z.right)
            {
                min = null;
            }
            else
            {
                min = z.right;
                Consolidate();
            }
            // decrement size of heap
            n--;
            return z.getData;
        }

        /**
         * Joins two Fibonacci heaps into a new one. No heap consolidation is
         * performed at this time. The two root lists are simply joined together.
         *
         * <p><em>Running time: O(1)</em></p>
         *
         * @param  H1  first heap
         * @param  H2  second heap
         * @return  new heap containing H1 and H2
         */
        public static FibonacciHeap<TKey, TValue> Union(FibonacciHeap<TKey, TValue> H1, FibonacciHeap<TKey, TValue> H2)
        {
            var H = new FibonacciHeap<TKey, TValue>();
            if (H1 is not null && H2 is not null)
            {
                H.min = H1.min;
                if (H.min is not null)
                {
                    if (H2.min is not null)
                    {
                        H.min.right.left = H2.min.left;
                        H2.min.left.right = H.min.right;
                        H.min.right = H2.min;
                        H2.min.left = H.min;
                        if (H2.min.getKey.CompareTo(H1.min!.getKey) < 0)
                        {
                            H.min = H2.min;
                        }
                    }
                }
                else
                {
                    H.min = H2.min;
                }
                H.n = H1.n + H2.n;
            }
            return H;
        }

        /// <summary>
        /// Gets the list of nodes.
        /// </summary>
        /// <returns></returns>
        public List<Node> NodeList()
        {
            var l = new List<Node>();
            if (min is not null)
                min.AddToList(l);
            return l;
        }

        /**
         * Implements a node of the Fibonacci heap. It holds the information
         * necessary for maintaining the structure of the heap. It acts as
         * an opaque handle for the data element, and serves as the key to
         * retrieving the data from the heap.
         *
         * @author  Nathan Fiedler
         */
        public class Node
        {
            /** Data object for this node, holds the key value. */
            public TValue getData;
            /** Key value for this node. */
            public TKey getKey;
            /** Parent node. */
            public Node? parent;
            /** First child node. */
            public Node? child;
            /** Right sibling node. */
            public Node right;
            /** Left sibling node. */
            public Node left;
            /** Number of children of this node. */
            public int degree;
            /** True if this node has had a child removed since this node was
             * added to its parent. */
            private bool mark;

            /**
             * Two-arg constructor which sets the data and key fields to the
             * passed arguments. It also initializes the right and left pointers,
             * making this a circular doubly-linked list.
             *
             * @param  data  data object to associate with this node
             * @param  key   key value for this data object
             */
            public Node(TValue data, TKey key)
            {
                this.getData = data;
                this.getKey = key;
                right = this;
                left = this;
            }

            /**
             * Performs a cascading cut operation. Cuts this from its parent
             * and then does the same for its parent, and so on up the tree.
             *
             * <p><em>Running time: O(log n)</em></p>
             *
             * @param  min  the minimum heap node, to which nodes will be added.
             */
            public void CascadingCut(Node min)
            {
                Node? z = parent;
                // if there's a parent...
                if (z is not null)
                {
                    if (mark)
                    {
                        // it's marked, cut it from parent
                        z.Cut(this, min);
                        // cut its parent as well
                        z.CascadingCut(min);
                    }
                    else
                    {
                        // if y is unmarked, set it marked
                        mark = true;
                    }
                }
            }

            /**
             * The reverse of the link operation: removes x from the child
             * list of this node.
             *
             * <p><em>Running time: O(1)</em></p>
             *
             * @param  x    child to be removed from this node's child list
             * @param  min  the minimum heap node, to which x is added.
             */
            public void Cut(Node x, Node min)
            {
                // remove x from childlist and decrement degree
                x.left.right = x.right;
                x.right.left = x.left;
                degree--;
                // reset child if necessary
                if (degree == 0)
                {
                    child = null;
                }
                else if (child == x)
                {
                    child = x.right;
                }
                // add x to root list of heap
                x.right = min;
                x.left = min.left;
                min.left = x;
                x.left.right = x;
                // set parent[x] to nil
                x.parent = null;
                // set mark[x] to false
                x.mark = false;
            }

            /**
             * Make this node a child of the given parent node. All linkages
             * are updated, the degree of the parent is incremented, and
             * mark is set to false.
             *
             * @param  parent  the new parent node.
             */
            public void Link(Node parent)
            {
                // Note: putting this code here in Node makes it 7x faster
                // because it doesn't have to use generated accessor methods,
                // which add a lot of time when called millions of times.
                // remove this from its circular list
                left.right = right;
                right.left = left;
                // make this a child of x
                this.parent = parent;
                if (parent.child is null)
                {
                    parent.child = this;
                    right = this;
                    left = this;
                }
                else
                {
                    left = parent.child;
                    right = parent.child.right;
                    parent.child.right = this;
                    right.left = this;
                }
                // increase degree[x]
                parent.degree++;
                // set mark false
                mark = false;
            }

            /// <summary>
            /// Add nodes to heap.
            /// </summary>
            /// <param name="l"></param>
            public void AddToList(List<Node> l)
            {
                Node cur = this;
                do
                {
                    l.Add(cur);
                    if (cur.child is not null) cur.child.AddToList(l);
                    cur = cur.right;
                } while (cur != this);
            }
        }
    }
}
