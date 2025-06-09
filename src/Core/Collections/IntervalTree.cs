/////////////////////////////////////////////////////////////////////
// File Name               : IntervalTree.cs
//      Created            : 06 8 2012   22:38
//      Author             : Costin S
//
/////////////////////////////////////////////////////////////////////
#define TREE_WITH_PARENT_POINTERS

#pragma warning disable CS1572
#pragma warning disable CS1573

namespace Reko.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents an interval.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Interval<T> where T : IComparable<T>
    {
        /// <summary>
        /// The start of the interval.
        /// </summary>
        public T Start { get; }

        /// <summary>
        /// The end of the interval.
        /// </summary>
        public T End { get; }

        /// <summary>
        /// Constructs an interval.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <exception cref="ArgumentException"></exception>
        public Interval(T start, T end)
        {
            if (start.CompareTo(end) > 0)
            {
                throw new ArgumentException("The start value of the interval must be smaller than the end value. Null intervals are not allowed.");
            }

            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">Other object.</param>
        /// <returns>True if the other object is an <see cref="Interval{T}"/>
        /// with the same extent; otherwise false.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            if (obj is not Interval<T> that)
                return false;
            return this.Start.CompareTo(that.Start) ==0 &&
                   this.End.CompareTo(that.End)==0;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode() * 17;
        }

        /// <summary>
        /// Compares two intervals for equality.
        /// </summary>
        /// <param name="a">First interval.</param>
        /// <param name="b">Second interval.</param>
        /// <returns>True if the intervals are equal; false if not.
        /// </returns>
        public static bool operator == (Interval<T> a, Interval<T> b)
        {
            return a.Start.CompareTo(b.Start) == 0 &&
                   a.End.CompareTo(b.End) == 0;
        }

        /// <summary>
        /// Compares two intervals for inequality.
        /// </summary>
        /// <param name="a">First interval.</param>
        /// <param name="b">Second interval.</param>
        /// <returns>True if the intervals are not equal; false if they are equal.
        /// </returns>
        public static bool operator != (Interval<T> a, Interval<T> b)
        {
            return a.Start.CompareTo(b.Start) != 0 ||
                   a.End.CompareTo(b.End) != 0;
        }

        /// <summary>
        /// Determines if two intervals overlap (i.e. if this interval starts before the other ends and it finishes after the other starts)
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if the specified other is overlapping; otherwise, <c>false</c>.
        /// </returns>
        public bool OverlapsWith(Interval<T> other)
        {
            return this.Start.CompareTo(other.End) < 0 && this.End.CompareTo(other.Start) > 0;
        }

        /// <summary>
        /// Returns true if this interval covers the interval `that`.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool Covers(Interval<T> that)
        {
            return this.Start.CompareTo(that.Start) <= 0 && this.End.CompareTo(that.End) >= 0;
        }

        /// <summary>
        /// Returns a string representation of the interval.
        /// </summary>
        public override string ToString()
        {
            return $"[{this.Start}, {this.End}]";
        }

        /// <summary>
        /// Computs the intersection of this interval with the specified interval.
        /// </summary>
        /// <param name="that">Other interval to compute intersection with.</param>
        /// <returns>The intersection of the intervals, or an empty interval
        /// if there is no intersection.
        /// </returns>
        public Interval<T> Intersect(Interval<T> that)
        {
            T min = this.Start.CompareTo(that.Start) >= 0
                ? this.Start
                : that.Start;
            T max = this.End.CompareTo(that.End) <= 0
                ? this.End
                : that.End;
            if (min.CompareTo(max) > 0)
                max = min;
            return new Interval<T>(min, max);
        }

        /// <summary>
        /// Computes the difference between this interval and the specified interval.
        /// </summary>
        /// <param name="that">Other interval.</param>
        /// <returns>The difference between the intervals.
        /// </returns>
        public Interval<T> Except(Interval<T> that)
        {
            var newBegin = this.Start.CompareTo(that.End) > 0
                ? that.End
                : this.Start;

            var newEnd = this.End.CompareTo(that.Start) > 0
                ? that.Start
                : this.End;

            if (newBegin.CompareTo(newEnd) > 0)
                newEnd = newBegin;
            return new Interval<T>(newBegin, newEnd);
        }
    }

    /// <summary>
    /// Factory class for creating intervals.
    /// </summary>
    public static class Interval
    {
        /// <summary>
        /// Creates the specified interval.
        /// </summary>
        /// <typeparam name="T">Value type for the interval.</typeparam>
        /// <param name="begin">Start of the interval.</param>
        /// <param name="end">End of the interval.</param>
        /// <returns>An <see cref="Interval{T}"/> instance.
        /// </returns>
        public static Interval<T> Create<T>(T begin, T end)
            where T : IComparable<T>
        {
            return new Interval<T>(begin, end);
        }
    }

    /// <summary>
    /// Interval Tree class
    /// </summary>
    /// <typeparam name="T">Key Type.</typeparam>
    /// <typeparam name="TypeValue">Value type.</typeparam>
    public class IntervalTree<T, TypeValue> :
        IEnumerable<KeyValuePair<Interval<T>, TypeValue>>
        where T : IComparable<T>
    {
        private IntervalNode? root;
        private readonly IComparer<T> comparer;
        private readonly KeyValueComparer<T, TypeValue> keyvalueComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTree&lt;T, TypeValue&gt;"/> class.
        /// </summary>
        public IntervalTree()
        {
            this.comparer = ComparerUtil.GetComparer();
            this.keyvalueComparer = new KeyValueComparer<T, TypeValue>(this.comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTree&lt;T, TypeValue&gt;"/> class.
        /// </summary>
        /// <param name="elems">The elems.</param>
        public IntervalTree(IEnumerable<KeyValuePair<Interval<T>, TypeValue>> elems) : this()
        {
            if (elems is not null)
            {
                foreach (var elem in elems)
                {
                    Add(elem.Key, elem.Value);
                }
            }
        }

        /// <summary>
        /// Number of intervals in the tree.
        /// </summary>
        public int Count { get; private set; }

        #region Methods

        /// <summary>
        /// Adds the specified interval.
        /// If there is more than one interval starting at the same value, the
        /// intervalnode.Interval stores the start time and the maximum end
        /// time of all intervals starting at the same value. All end values
        /// (except the maximum end time/value which is stored in the interval
        /// node itself) are stored in the Range list in decreasing order.
        /// Note: this is okay for problems where intervals starting at the
        /// same time /value is not a frequent occurrence, however you can use
        /// other data structure for better performance depending on your
        /// problem needs.
        /// </summary>
        /// <param name="x">Start of the interval.</param>
        /// <param name="y">End of the interval.</param>
        /// <param name="value">Value to place at the interval.</param>
        public void Add(T x, T y, TypeValue value)
        {
            Add(new Interval<T>(x, y), value);
        }

        /// <summary>
        /// Adds the specified interval.
        /// If there is more than one interval starting at the same value, the
        /// intervalnode.Interval stores the start value and the maximum end
        /// value of all intervals starting at the same value. All end values
        /// (except the maximum end value which is stored in the interval node
        /// itself) are stored in the Range list in decreasing order.
        /// Note: this is okay for problems where intervals starting at the
        /// same value is not a frequent occurrence, however you can use other
        /// data structure for better performance depending on your problem
        /// needs
        /// </summary>
        /// <param name="interval">The interval to add.</param>
        /// <param name="value">The value to place at the interval.</param>
        public bool Add(Interval<T> interval, TypeValue value)
        {
            bool wasAdded = false;
            bool wasSuccessful = false;

            this.root = IntervalNode.Add(this.root, interval, value, ref wasAdded, ref wasSuccessful);
            if (this.root is not null)
            {
                IntervalNode.ComputeMax(this.root);
            }

            if (wasSuccessful)
            {
                this.Count++;
            }

            return wasSuccessful;
        }

        /// <summary>
        /// Deletes the specified interval.
        /// If the interval tree is used with unique intervals, this method
        /// removes the interval specified as an argument. If multiple
        /// identical intervals (starting at the same time and also ending
        /// at the same time) are allowed, this function will delete one of
        /// them (see procedure DeleteIntervalFromNodeWithRange for details).
        /// In this case, it is easy enough to either specify the (interval,
        /// value) pair to be deleted or enforce uniqueness by changing the
        /// Add procedure.
        /// </summary>
        /// <param name="arg">The arg.</param>
        public bool Delete(Interval<T> arg)
        {
            if (this.root is not null)
            {
                bool wasDeleted = false;
                bool wasSuccessful = false;

                this.root = IntervalNode.Delete(this.root, arg, ref wasDeleted, ref wasSuccessful);
                if (this.root is not null)
                {
                    IntervalNode.ComputeMax(this.root);
                }

                if (wasSuccessful)
                {
                    this.Count--;
                }
                return wasSuccessful;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Searches for all intervals overlapping the one specified.
        /// If multiple intervals starting at the same time/value are found to overlap the specified interval, they are returned in decreasing order of their End values.
        /// </summary>
        /// <param name="toFind">To find.</param>
        /// <param name="list">The list.</param>
        public void GetIntervalsOverlappingWith(Interval<T> toFind, ref List<KeyValuePair<Interval<T>, TypeValue>> list)
        {
            this.root?.GetIntervalsOverlappingWith(toFind, ref list);
        }

        /// <summary>
        /// Searches for all intervals overlapping the one specified.
        /// If multiple intervals starting at the same time/value are found to overlap the specified interval, they are returned in decreasing order of their End values.
        /// </summary>
        /// <param name="toFind">To find.</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsOverlappingWith(Interval<T> toFind)
        {
            if (this.root is not null)
            {
                return this.root.GetIntervalsOverlappingWith(toFind);
            }
            else
            {
                return Enumerable.Empty<KeyValuePair<Interval<T>, TypeValue>>();
            }
        }

        /// <summary>
        /// Returns all intervals beginning at the specified start value. 
        /// The multiple intervals start at the specified value, they are sorted based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns></returns>
        public List<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsStartingAt(T arg)
        {
            return IntervalNode.GetIntervalsStartingAt(this.root, arg)
                ?? new List<KeyValuePair<Interval<T>, TypeValue>>();
        }

#if TREE_WITH_PARENT_POINTERS

        /// <summary>
        /// Gets the collection of intervals (in ascending order of their Start values).
        /// Those intervals starting at the same time/value are sorted further based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        public IEnumerable<Interval<T>> Intervals
        {
            get
            {
                if (this.root is null)
                {
                    yield break;
                }

                var p = IntervalNode.FindMin(this.root);
                while (p is not null)
                {
                    foreach (var rangeNode in p.GetRangeReverse())
                    {
                        yield return rangeNode.Key;
                    }

                    yield return p.Interval;
                    p = p.Successor();
                }
            }
        }

        /// <summary>
        /// Gets the collection of values (ascending order)
        /// Those intervals starting at the same time/value are sorted further
        /// based on their End value (i.e. returned in ascending order of their
        /// End values)
        /// </summary>
        public IEnumerable<TypeValue> Values
        {
            get
            {
                if (this.root is null)
                {
                    yield break;
                }

                var p = IntervalNode.FindMin(this.root);
                while (p is not null)
                {
                    foreach (var rangeNode in p.GetRangeReverse())
                    {
                        yield return rangeNode.Value;
                    }

                    yield return p.Value;
                    p = p.Successor();
                }
            }
        }

        /// <summary>
        /// Gets the interval value pairs.
        /// Those intervals starting at the same time/value are sorted further based on their End value (i.e. returned in ascending order of their End values)
        /// </summary>
        public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> IntervalValuePairs
        {
            get
            {
                if (this.root is null)
                {
                    yield break;
                }

                var p = IntervalNode.FindMin(this.root);
                while (p is not null)
                {
                    foreach (var rangeNode in p.GetRangeReverse())
                    {
                        yield return rangeNode;
                    }

                    yield return new KeyValuePair<Interval<T>, TypeValue>(p.Interval, p.Value);
                    p = p.Successor();
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<Interval<T>, TypeValue>> GetEnumerator()
        {
            return IntervalValuePairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return IntervalValuePairs.GetEnumerator();
        }

#endif

        /// <summary>
        /// Tries to the get the value associated with the interval.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetInterval(Interval<T> data, [MaybeNullWhen(false)] out TypeValue value)
        {
            return this.TryGetIntervalImpl(this.root, data, out value);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.root = null;
            this.Count = 0;
        }

        /// <summary>
        /// Writes this instance to a TextWriter
        /// </summary>
        public void Write(TextWriter w)
        {
            this.Visit((node, level) =>
            {
                w.Write(new string(' ', 2 * level));
                w.Write(string.Format("{0}.{1}", node.Interval, node.Max));

                if (node.Range is not null)
                {
                    w.Write(" ... ");
                    foreach (var rangeNode in node.GetRange())
                    {
                        w.Write(string.Format("{0}  ", rangeNode.Key));
                    }
                }

                w.WriteLine();
            });
        }

        /// <summary>
        /// Dumps this instance to the debug output.
        /// </summary>
        [Conditional("DEBUG")]
        public void Dump()
        {
            var sw = new StringWriter();
            Write(sw);
            Debug.Write(sw);
        }

        /// <summary>
        /// Searches for interval starting at.
        /// </summary>
        /// <param name="subtree">The subtree.</param>
        /// <param name="data">The data.</param>
        /// <param name="value">The retrieved value.</param>
        /// <returns>True if a value was found; otherwise false.</returns>
        private bool TryGetIntervalImpl(IntervalNode? subtree, Interval<T> data, [MaybeNullWhen(false)] out TypeValue value)
        {
            if (subtree is not null)
            {
                int compareResult = data.Start.CompareTo(subtree.Interval.Start);

                if (compareResult < 0)
                {
                    return this.TryGetIntervalImpl(subtree.Left, data, out value);
                }
                else if (compareResult > 0)
                {
                    return this.TryGetIntervalImpl(subtree.Right, data, out value);
                }
                else
                {
                    if (data.End.CompareTo(subtree.Interval.End) == 0)
                    {
                        value = subtree.Value;
                        return true;
                    }
                    else if (subtree.Range is not null)
                    {
                        int kthIndex = subtree.Range.BinarySearch(new KeyValuePair<T, TypeValue>(data.End, default!), this.keyvalueComparer);
                        if (kthIndex >= 0)
                        {
                            value = subtree.Range[kthIndex].Value;
                            return true;
                        }
                    }
                }
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Visit_inorders the specified visitor. Defined for debugging purposes only
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        private void Visit(Action<IntervalNode,int> visitor)
        {
            this.root?.Visit(visitor, 0);
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// IntervalNode class. 
        /// </summary>
        private class IntervalNode
        {
            #region Fields

#if TREE_WITH_PARENT_POINTERS
            private IntervalNode? Parent;
#endif
            #endregion

            #region Properties

            public int Balance { get; private set; }
            public IntervalNode? Left { get; private set; }
            public IntervalNode? Right { get; private set; }
            public Interval<T> Interval { get; private set; }
            public TypeValue Value { get; private set; }
            public List<KeyValuePair<T, TypeValue>>? Range { get; private set; }
            public T Max { get; private set; }

            #endregion

            #region C'tor

            public IntervalNode(Interval<T> interval, TypeValue value)
            {
                this.Left = null;
                this.Right = null;
                this.Balance = 0;
                this.Interval = interval;
                this.Value = value;
                this.Max = interval.End;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Adds the specified elem.
            /// </summary>
            /// <param name="elem">The elem.</param>
            /// <param name="interval">The interval where it is to be added.</param>
            /// <param name="value">The data.</param>
            /// <param name="wasAdded">True if the data was added.</param>
            /// <param name="wasSuccessful">True if the data was added successfully.</param>
            /// <returns>A new interval node.</returns>
            public static IntervalNode Add(IntervalNode? elem, Interval<T> interval, TypeValue value, ref bool wasAdded, ref bool wasSuccessful)
            {
                if (elem is null)
                {
                    elem = new IntervalNode(interval, value);
                    wasAdded = true;
                    wasSuccessful = true;
                }
                else
                {
                    int compareResult = interval.Start.CompareTo(elem.Interval.Start);
                    IntervalNode? newChild = null;
                    if (compareResult < 0)
                    {
                        newChild = Add(elem.Left, interval, value, ref wasAdded, ref wasSuccessful);
                        if (elem.Left != newChild)
                        {
                            elem.Left = newChild;
#if TREE_WITH_PARENT_POINTERS
                            newChild.Parent = elem;
#endif
                        }

                        if (wasAdded)
                        {
                            elem.Balance--;

                            if (elem.Balance == 0)
                            {
                                wasAdded = false;
                            }
                            else if (elem.Balance == -2)
                            {
                                if (elem.Left.Balance == 1)
                                {
                                    int elemLeftRightBalance = elem.Left.Right!.Balance;

                                    elem.Left = RotateLeft(elem.Left);
                                    elem = RotateRight(elem);

                                    elem.Balance = 0;
                                    elem.Left!.Balance = elemLeftRightBalance == 1 ? -1 : 0;
                                    elem.Right!.Balance = elemLeftRightBalance == -1 ? 1 : 0;
                                }
                                else if (elem.Left.Balance == -1)
                                {
                                    elem = RotateRight(elem);
                                    elem.Balance = 0;
                                    elem.Right!.Balance = 0;
                                }
                                wasAdded = false;
                            }
                        }
                    }
                    else if (compareResult > 0)
                    {
                        newChild = Add(elem.Right, interval, value, ref wasAdded, ref wasSuccessful);
                        if (elem.Right != newChild)
                        {
                            elem.Right = newChild;
#if TREE_WITH_PARENT_POINTERS
                            newChild.Parent = elem;
#endif
                        }

                        if (wasAdded)
                        {
                            elem.Balance++;
                            if (elem.Balance == 0)
                            {
                                wasAdded = false;
                            }
                            else if (elem.Balance == 2)
                            {
                                if (elem.Right.Balance == -1)
                                {
                                    int elemRightLeftBalance = elem.Right.Left!.Balance;

                                    elem.Right = RotateRight(elem.Right);
                                    elem = RotateLeft(elem);

                                    elem.Balance = 0;
                                    elem.Left!.Balance = elemRightLeftBalance == 1 ? -1 : 0;
                                    elem.Right!.Balance = elemRightLeftBalance == -1 ? 1 : 0;
                                }

                                else if (elem.Right.Balance == 1)
                                {
                                    elem = RotateLeft(elem);

                                    elem.Balance = 0;
                                    elem.Left!.Balance = 0;
                                }
                                wasAdded = false;
                            }
                        }
                    }
                    else
                    {
                        //// if there are more than one interval starting at the same time/value, the intervalnode.Interval stores the start time and the maximum end time of all intervals starting at the same value.
                        //// all end values (except the maximum end time/value which is stored in the interval node itself) are stored in the Range list in decreasing order.
                        //// note: this is ok for problems where intervals starting at the same time /value is not a frequent occurrence, however you can use other data structure for better performance depending on your problem needs

                        elem.AddIntervalValuePair(interval, value);

                        wasSuccessful = true;
                    }
                    ComputeMax(elem);
                }
                return elem;
            }

            /// <summary>
            /// Computes the max.
            /// </summary>
            /// <param name="node">The node.</param>
            public static void ComputeMax(IntervalNode node)
            {
                T maxRange = node.Interval.End;

                if (node.Left is null && node.Right is null)
                {
                    node.Max = maxRange;
                }
                else if (node.Left is null)
                {
                    node.Max = (maxRange.CompareTo(node.Right!.Max) >= 0) ? maxRange : node.Right.Max;
                }
                else if (node.Right is null)
                {
                    node.Max = (maxRange.CompareTo(node.Left.Max) >= 0) ? maxRange : node.Left.Max;
                }
                else
                {
                    T leftMax = node.Left.Max;
                    T rightMax = node.Right.Max;

                    if (leftMax.CompareTo(rightMax) >= 0)
                    {
                        node.Max = maxRange.CompareTo(leftMax) >= 0 ? maxRange : leftMax;
                    }
                    else
                    {
                        node.Max = maxRange.CompareTo(rightMax) >= 0 ? maxRange : rightMax;
                    }
                }
            }

            /// <summary>
            /// Finds the min.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns></returns>
            public static IntervalNode? FindMin(IntervalNode? node)
            {
                while (node is not null && node.Left is not null)
                {
                    node = node.Left;
                }
                return node;
            }

            /// <summary>
            /// Finds the max.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns></returns>
            public static IntervalNode? FindMax(IntervalNode? node)
            {
                while (node is not null && node.Right is not null)
                {
                    node = node.Right;
                }
                return node;
            }

            /// <summary>
            /// Gets the range of intervals stored in this.Range (i.e. starting at the same value 'this.Interval.Start' as the interval stored in the node itself)
            /// The range intervals are sorted in the descending order of their End interval values
            /// </summary>
            /// <returns></returns>
            public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetRange()
            {
                if (this.Range is not null)
                {
                    foreach (var value in this.Range)
                    {
                        var kth = new Interval<T>(this.Interval.Start, value.Key);
                        yield return new KeyValuePair<Interval<T>, TypeValue>(kth, value.Value);
                    }
                }
                else
                {
                    yield break;
                }
            }

            /// <summary>
            /// Gets the range of intervals stored in this.Range (i.e. starting at the same value 'this.Interval.Start' as the interval stored in the node itself).
            /// The range intervals are sorted in the ascending order of their End interval values
            /// </summary>
            /// <returns></returns>
            public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetRangeReverse()
            {
                if (this.Range is not null && this.Range.Count > 0)
                {
                    int rangeCount = this.Range.Count;
                    for (int k = rangeCount - 1; k >= 0; k--)
                    {
                        var kth = new Interval<T>(this.Interval.Start, this.Range[k].Key);
                        yield return new KeyValuePair<Interval<T>, TypeValue>(kth, this.Range[k].Value);
                    }
                }
                else
                {
                    yield break;
                }
            }

#if TREE_WITH_PARENT_POINTERS

            /// <summary>
            /// Succeeds this instance.
            /// </summary>
            /// <returns></returns>
            public IntervalNode? Successor()
            {
                if (this.Right is not null)
                    return FindMin(this.Right);
                else
                {
                    var p = this;
                    while (p.Parent is not null && p.Parent.Right == p)
                    {
                        p = p.Parent;
                    }
                    return p.Parent;
                }
            }

            /// <summary>
            /// Precedes this instance.
            /// </summary>
            /// <returns></returns>
            public IntervalNode? Predecesor()
            {
                if (this.Left is not null)
                {
                    return FindMax(this.Left);
                }
                else
                {
                    var p = this;
                    while (p.Parent is not null && p.Parent.Left == p)
                    {
                        p = p.Parent;
                    }
                    return p.Parent;
                }
            }
#endif

            /// <summary>
            /// Deletes the specified node.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="arg">The arg.</param>
            /// <param name="wasDeleted"></param>
            /// <param name="wasSuccessful"></param>
            /// <returns></returns>
            public static IntervalNode? Delete(IntervalNode node, Interval<T> arg, ref bool wasDeleted, ref bool wasSuccessful)
            {
                int cmp = arg.Start.CompareTo(node.Interval.Start);
                IntervalNode? newChild = null;

                if (cmp < 0)
                {
                    if (node.Left is not null)
                    {
                        newChild = Delete(node.Left, arg, ref wasDeleted, ref wasSuccessful);
                        if (node.Left != newChild)
                        {
                            node.Left = newChild;
                        }

                        if (wasDeleted)
                        {
                            node.Balance++;
                        }
                    }
                }
                else if (cmp == 0)
                {
                    if (arg.End.CompareTo(node.Interval.End) == 0 && node.Range is null)
                    {
                        if (node.Left is not null && node.Right is not null)
                        {
                            var min = FindMin(node.Right);

                            var interval = node.Interval;
                            node.Swap(min!);

                            wasDeleted = false;

                            newChild = Delete(node.Right, interval, ref wasDeleted, ref wasSuccessful);
                            if (node.Right != newChild)
                            {
                                node.Right = newChild;
                            }

                            if (wasDeleted)
                            {
                                node.Balance--;
                            }
                        }
                        else if (node.Left is null)
                        {
                            wasDeleted = true;
                            wasSuccessful = true;

#if TREE_WITH_PARENT_POINTERS
                            if (node.Right is not null)
                            {
                                node.Right.Parent = node.Parent;
                            }
#endif
                            return node.Right;
                        }
                        else
                        {
                            wasDeleted = true;
                            wasSuccessful = true;
#if TREE_WITH_PARENT_POINTERS
                            if (node.Left is not null)
                            {
                                node.Left.Parent = node.Parent;
                            }
#endif
                            return node.Left;
                        }
                    }
                    else
                    {
                        wasSuccessful = node.DeleteIntervalFromNodeWithRange(arg);
                    }
                }
                else
                {
                    if (node.Right is not null)
                    {
                        newChild = Delete(node.Right, arg, ref wasDeleted, ref wasSuccessful);
                        if (node.Right != newChild)
                        {
                            node.Right = newChild;
                        }

                        if (wasDeleted)
                        {
                            node.Balance--;
                        }
                    }
                }

                ComputeMax(node);

                if (wasDeleted)
                {
                    if (node.Balance == 1 || node.Balance == -1)
                    {
                        wasDeleted = false;
                        return node;
                    }
                    else if (node.Balance == -2)
                    {
                        if (node.Left!.Balance == 1)
                        {
                            int leftRightBalance = node.Left.Right!.Balance;

                            node.Left = RotateLeft(node.Left);
                            node = RotateRight(node);

                            node.Balance = 0;
                            node.Left!.Balance = (leftRightBalance == 1) ? -1 : 0;
                            node.Right!.Balance = (leftRightBalance == -1) ? 1 : 0;
                        }
                        else if (node.Left.Balance == -1)
                        {
                            node = RotateRight(node);
                            node.Balance = 0;
                            node.Right!.Balance = 0;
                        }
                        else if (node.Left.Balance == 0)
                        {
                            node = RotateRight(node);
                            node.Balance = 1;
                            node.Right!.Balance = -1;

                            wasDeleted = false;
                        }
                    }
                    else if (node.Balance == 2)
                    {
                        if (node.Right!.Balance == -1)
                        {
                            int rightLeftBalance = node.Right.Left!.Balance;

                            node.Right = RotateRight(node.Right);
                            node = RotateLeft(node);

                            node.Balance = 0;
                            node.Left!.Balance = (rightLeftBalance == 1) ? -1 : 0;
                            node.Right!.Balance = (rightLeftBalance == -1) ? 1 : 0;
                        }
                        else if (node.Right.Balance == 1)
                        {
                            node = RotateLeft(node);
                            node.Balance = 0;
                            node.Left!.Balance = 0;
                        }
                        else if (node.Right.Balance == 0)
                        {
                            node = RotateLeft(node);
                            node.Balance = -1;
                            node.Left!.Balance = 1;

                            wasDeleted = false;
                        }
                    }
                }

                return node;
            }

            /// <summary>
            /// Returns all intervals beginning at the specified start value. The intervals are sorted based on their End value (i.e. returned in ascending order of their End values)
            /// </summary>
            /// <param name="subtree">The subtree.</param>
            /// <param name="start">The starting position.</param>
            /// <returns></returns>
            public static List<KeyValuePair<Interval<T>, TypeValue>>? GetIntervalsStartingAt(IntervalNode? subtree, T start)
            {
                if (subtree is not null)
                {
                    int compareResult = start.CompareTo(subtree.Interval.Start);
                    if (compareResult < 0)
                    {
                        return GetIntervalsStartingAt(subtree.Left, start);
                    }
                    else if (compareResult > 0)
                    {
                        return GetIntervalsStartingAt(subtree.Right, start);
                    }
                    else
                    {
                        var result = new List<KeyValuePair<Interval<T>, TypeValue>>();
                        if (subtree.Range is not null)
                        {
                            foreach (var kvp in subtree.GetRangeReverse())
                            {
                                result.Add(kvp);
                            }
                        }
                        result.Add(new KeyValuePair<Interval<T>, TypeValue>(subtree.Interval, subtree.Value));
                        return result;
                    }
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Searches for all intervals in this subtree that are overlapping the argument interval.
            /// If multiple intervals starting at the same time/value are found to overlap, they are returned in decreasing order of their End values.
            /// </summary>
            /// <param name="toFind">To find.</param>
            /// <param name="list">The list.</param>
            public void GetIntervalsOverlappingWith(Interval<T> toFind, ref List<KeyValuePair<Interval<T>, TypeValue>> list)
            {
                if (toFind.End.CompareTo(this.Interval.Start) <= 0)
                {
                    ////toFind ends before subtree.Data begins, prune the right subtree
                    this.Left?.GetIntervalsOverlappingWith(toFind, ref list);
                }
                else if (toFind.Start.CompareTo(this.Max) >= 0)
                {
                    ////toFind begins after the subtree.Max ends, prune the left subtree
                    this.Right?.GetIntervalsOverlappingWith(toFind, ref list);
                }
                else
                {
                    //// search the left subtree
                    this.Left?.GetIntervalsOverlappingWith(toFind, ref list);

                    if (this.Interval.OverlapsWith(toFind))
                    {
                        list ??= new List<KeyValuePair<Interval<T>, TypeValue>>();

                        list.Add(new KeyValuePair<Interval<T>, TypeValue>(this.Interval, this.Value));

                        ////the max value is stored in the node, if the node doesn't overlap then neither are the nodes in its range 
                        if (this.Range is not null && this.Range.Count > 0)
                        {
                            foreach (var kvp in this.GetRange())
                            {
                                if (kvp.Key.OverlapsWith(toFind))
                                {
                                    list ??= new List<KeyValuePair<Interval<T>, TypeValue>>();
                                    list.Add(kvp);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    //// search the right subtree
                    this.Right?.GetIntervalsOverlappingWith(toFind, ref list);
                }
            }

            /// <summary>
            /// Gets all intervals in this subtree that are overlapping the argument interval. 
            /// If multiple intervals starting at the same time/value are found to overlap, they are returned in decreasing order of their End values.
            /// </summary>
            /// <param name="toFind">To find.</param>
            /// <returns></returns>
            public IEnumerable<KeyValuePair<Interval<T>, TypeValue>> GetIntervalsOverlappingWith(Interval<T> toFind)
            {
                if (toFind.End.CompareTo(this.Interval.Start) <= 0)
                {
                    ////toFind ends before subtree.Data begins, prune the right subtree
                    if (this.Left is not null)
                    {
                        foreach (var value in this.Left.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }
                }
                else if (toFind.Start.CompareTo(this.Max) >= 0)
                {
                    ////toFind begins after the subtree.Max ends, prune the left subtree
                    if (this.Right is not null)
                    {
                        foreach (var value in this.Right.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }
                }
                else
                {
                    if (this.Left is not null)
                    {
                        foreach (var value in this.Left.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }

                    if (this.Interval.OverlapsWith(toFind))
                    {
                        yield return new KeyValuePair<Interval<T>, TypeValue>(this.Interval, this.Value);

                        if (this.Range is not null && this.Range.Count > 0)
                        {
                            foreach (var kvp in this.GetRange())
                            {
                                if (kvp.Key.OverlapsWith(toFind))
                                {
                                    yield return kvp;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (this.Right is not null)
                    {
                        foreach (var value in this.Right.GetIntervalsOverlappingWith(toFind))
                        {
                            yield return value;
                        }
                    }
                }
            }

            public void Visit(Action<IntervalNode, int> visitor, int level)
            {
                this.Left?.Visit(visitor, level + 1);

                visitor(this, level);

                this.Right?.Visit(visitor, level + 1);
            }

            /// <summary>
            /// Rotates lefts this instance.
            /// Assumes that this.Right is not null
            /// </summary>
            /// <returns></returns>
            private static IntervalNode RotateLeft(IntervalNode node)
            {
                var right = node.Right!;
                Debug.Assert(node.Right is not null);

                var rightLeft = right.Left;

                node.Right = rightLeft;
                ComputeMax(node);

#if TREE_WITH_PARENT_POINTERS
                var parent = node.Parent;
                if (rightLeft is not null)
                {
                    rightLeft.Parent = node;
                }
#endif
                right.Left = node;
                ComputeMax(right);

#if TREE_WITH_PARENT_POINTERS
                node.Parent = right;
                if (parent is not null)
                {
                    if (parent.Left == node)
                    {
                        parent.Left = right;
                    }
                    else
                    {
                        parent.Right = right;
                    }
                }
                right.Parent = parent;
#endif
                return right;
            }

            /// <summary>
            /// Rotates right this instance.
            /// Assumes that (this.Left is not null)
            /// </summary>
            /// <returns></returns>
            private static IntervalNode RotateRight(IntervalNode node)
            {
                var left = node.Left!;
                Debug.Assert(node.Left is not null);

                var leftRight = left.Right;
                node.Left = leftRight;
                ComputeMax(node);

#if TREE_WITH_PARENT_POINTERS
                var parent = node.Parent;
                if (leftRight is not null)
                {
                    leftRight.Parent = node;
                }
#endif
                left.Right = node;
                ComputeMax(left);

#if TREE_WITH_PARENT_POINTERS
                node.Parent = left;
                if (parent is not null)
                {
                    if (parent.Left == node)
                    {
                        parent.Left = left;
                    }
                    else
                    {
                        parent.Right = left;
                    }
                }
                left.Parent = parent;
#endif
                return left;
            }

            /// <summary>
            /// Deletes the specified interval from this node. 
            /// If the interval tree is used with unique intervals, this method removes the interval specified as an argument.
            /// If multiple identical intervals (starting at the same time and also ending at the same time) are allowed, this function will delete one of them. 
            /// In this case, it is easy enough to either specify the (interval, value) pair to be deleted or enforce uniqueness by changing the Add procedure.
            /// </summary>
            /// <param name="interval">The interval to be deleted.</param>
            /// <returns></returns>
            private bool DeleteIntervalFromNodeWithRange(Interval<T> interval)
            {
                if (this.Range is not null && this.Range.Count > 0)
                {
                    int rangeCount = this.Range.Count;
                    int intervalPosition = -1;

                    // find the exact interval to delete based on its End value.
                    if (interval.End.CompareTo(this.Interval.End) == 0)
                    {
                        intervalPosition = 0;
                    }
                    else if (rangeCount > 12)
                    {
                        var keyvalueComparer = new KeyValueComparer<T, TypeValue>(ComparerUtil.GetComparer());
                        int k = this.Range.BinarySearch(new KeyValuePair<T, TypeValue>(interval.End, default!), keyvalueComparer);
                        if (k >= 0)
                        {
                            intervalPosition = k + 1;
                        }
                    }
                    else
                    {
                        for (int k = 0; k < rangeCount; k++)
                        {
                            if (interval.End.CompareTo(this.Range[k].Key) == 0)
                            {
                                intervalPosition = k + 1;
                                break;
                            }
                        }
                    }

                    if (intervalPosition < 0)
                    {
                        return false;
                    }
                    else if (intervalPosition == 0)
                    {
                        this.Interval = new Interval<T>(this.Interval.Start, this.Range[0].Key);
                        this.Value = this.Range[0].Value;
                        this.Range.RemoveAt(0);
                    }
                    else if (intervalPosition > 0)
                    {
                        this.Range.RemoveAt(intervalPosition - 1);
                    }

                    if (this.Range.Count == 0)
                    {
                        this.Range = null;
                    }

                    return true;
                }
                else
                {
                    ////if interval end was not found in the range (or the node itself) or if the node doesnt have a range, return false
                    return false;
                }
            }

            private void Swap(IntervalNode node)
            {
                var dataInterval = this.Interval;
                var dataValue = this.Value;
                var dataRange = this.Range;

                this.Interval = node.Interval;
                this.Value = node.Value;
                this.Range = node.Range;

                node.Interval = dataInterval;
                node.Value = dataValue;
                node.Range = dataRange;
            }

            private void AddIntervalValuePair(Interval<T> interval, TypeValue value)
            {
                this.Range ??= new List<KeyValuePair<T, TypeValue>>();

                ////always store the max End value in the node.Data itself .. store the Range list in decreasing order
                if (interval.End.CompareTo(this.Interval.End) > 0)
                {
                    this.Range.Insert(0, new KeyValuePair<T, TypeValue>(this.Interval.End, this.Value));
                    this.Interval = interval;
                    this.Value = value;
                }
                else
                {
                    bool wasAdded = false;
                    for (int i = 0; i < this.Range.Count; i++)
                    {
                        if (interval.End.CompareTo(this.Range[i].Key) >= 0)
                        {
                            this.Range.Insert(i, new KeyValuePair<T, TypeValue>(interval.End, value));
                            wasAdded = true;
                            break;
                        }
                    }
                    if (!wasAdded)
                    {
                        this.Range.Add(new KeyValuePair<T, TypeValue>(interval.End, value));
                    }
                }
            }

            #endregion
        }

        private class KeyValueComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        {
            private readonly IComparer<TKey> keyComparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="IntervalTree&lt;T, TypeValue&gt;.KeyValueComparer&lt;TKey, TValue&gt;"/> class.
            /// </summary>
            /// <param name="keyComparer">The key comparer.</param>
            public KeyValueComparer(IComparer<TKey> keyComparer)
            {
                this.keyComparer = keyComparer;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// Value Condition Less than zero is less than y.Zerox equals y.Greater than zero is greater than y.
            /// </returns>
            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return 0 - this.keyComparer.Compare(x.Key, y.Key);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object? obj)
            {
                if (obj is KeyValueComparer<TKey, TValue> that)
                {
                    return object.Equals(this.keyComparer, that.keyComparer);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return this.keyComparer.GetHashCode();
            }
        }

        private static class ComparerUtil
        {
            public static IComparer<T> GetComparer()
            {
                if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(System.IComparable).IsAssignableFrom(typeof(T)))
                {
                    return Comparer<T>.Default;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("The type {0} cannot be compared. It must implement IComparable<T> or IComparable", typeof(T).FullName));
                }
            }
        }

        #endregion
    }
}