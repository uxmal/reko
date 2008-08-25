using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Lib
{
    /// <summary>
    /// Implements a Disjoint-set Data Structure.
    /// </summary>
    public class DisjointPartition<T> where T : class
    {
        private Dictionary<T, Set> items;

        public DisjointPartition()
        {
            items = new Dictionary<T, Set>();
        }

        public void Add(T item)
        {
            items.Add(item, new Set(item));
        }

        public void Union(T x, T y)
        {
            Set xRoot = FindSet(items[x]);
            Set yRoot = FindSet(items[y]);
            if (xRoot.rank > yRoot.rank)
            {
                yRoot.parent = xRoot;
            }
            else if (xRoot.rank < yRoot.rank)
            {
                xRoot.parent = yRoot;
            }
            else if (xRoot != yRoot)
            {
                yRoot.parent = xRoot;
                ++xRoot.rank;
            }
        }

        public T Find(T item)
        {
            Set x = items[item];
            return FindSet(items[item]).item;
        }

        private Set FindSet(Set x)
        {
            if (x.parent == x)
                return x;
            else
            {
                x.parent = FindSet(x.parent);
                return x.parent;
            }
        }


        private class Set
        {
            public Set(T item) { this.item = item;  parent = this; rank = 0; }
            public T item;
            public Set parent;
            public int rank;
        }
    }
}

