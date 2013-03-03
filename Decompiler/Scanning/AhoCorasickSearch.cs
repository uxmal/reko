#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
 .
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

namespace Decompiler.Scanning
{
    /// <summary>
    /// Class for searching string for one or multiple 
    /// keywords using efficient Aho-Corasick search algorithm
    /// </summary>
    /// For more information visit
    /// http://www.cs.uku.fi/~kilpelai/BSA05/lectures/slides04.pdf
    /// </remarks>
    public class AhoCorasickSearch<TSymbol> : StringSearch<TSymbol> 
        where TSymbol : IComparable<TSymbol>
    {
        private TreeNode root;
        private TSymbol[][] keywords;

        public AhoCorasickSearch(TSymbol[][] keywords) : base(null)
        {
            Keywords = keywords;
        }

        public AhoCorasickSearch() : base(null)
        { 
        }

        private void BuildTree()
        {
            root = new TreeNode(null, default(TSymbol));
            root.Failure = root;
            foreach (TSymbol[] p in keywords)
            {
                AddPatternToTree(p);
            }
            FindFailureFunctions();
        }

        private void FindFailureFunctions()
        {
            var nodes = new List<TreeNode>();
            // level 1 nodes - fail to root node
            foreach (TreeNode nd in root.Transitions)
            {
                nd.Failure = root;
                foreach (TreeNode child in nd.Transitions) 
                    nodes.Add(child);
            }
            // other nodes - using BFS
            while (nodes.Count != 0)
            {
                var newNodes = new List<TreeNode>();
                foreach (TreeNode nd in nodes)
                {
                    TreeNode r = nd.Parent.Failure;
                    TSymbol c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) 
                        r = r.Failure;
                    if (r == null)
                        nd.Failure = root;
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        foreach (TSymbol[] result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
        }

        private void AddPatternToTree(TSymbol[] p)
        {
            TreeNode nd = root;
            foreach (TSymbol c in p)
            {
                TreeNode ndNew = null;
                foreach (TreeNode trans in nd.Transitions)
                {
                    if (trans.Char.CompareTo(c) == 0) 
                    { 
                        ndNew = trans; 
                        break; 
                    }
                }

                if (ndNew == null)
                {
                    ndNew = new TreeNode(nd, c);
                    nd.AddTransition(ndNew);
                }
                nd = ndNew;
            }
            nd.AddResult(p);
        }


        /// <summary>
        /// Keywords to search for (setting this property is slow, because
        /// it requires rebuilding of keyword tree)
        /// </summary>
        public TSymbol[][] Keywords
        {
            get { return keywords; }
            set
            {
                keywords = value;
                BuildTree();
            }
        }

        public override IEnumerator<int> GetMatchPositions(TSymbol[] text)
        {
            var ptr = root;
            for (var index = 0; index < text.Length; ++index)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == root)
                        break;
                    if (trans == null)
                        ptr = ptr.Failure;
                }
                if (trans != null)
                    ptr = trans;
                foreach (TSymbol[] found in ptr.Results)
                {
                    yield return index - found.Length + 1;
                }
            }
        }

        private class TreeNode
        {
            private List<TSymbol[]> results;
            private Dictionary<TSymbol,TreeNode> hash;

            public TreeNode(TreeNode parent, TSymbol c)
            {
                this.Char = c; 
                this.Parent = parent;
                this.results = new List<TSymbol[]>();

                hash = new Dictionary<TSymbol,TreeNode>();
            }

            public TSymbol Char { get; private set; }
            public TreeNode Parent { get; private set; }
            public TreeNode Failure { get; set; }


            public void AddResult(TSymbol [] result)
            {
                if (results.Contains(result)) return;
                results.Add(result);
            }

            public void AddTransition(TreeNode node)
            {
                hash.Add(node.Char, node);
            }

            public TreeNode GetTransition(TSymbol c)
            {
                TreeNode  t;
                if (hash.TryGetValue(c, out t))
                    return t;
                else
                    return null;
            }

            public bool ContainsTransition(TSymbol c)
            {
                return GetTransition(c) != null;
            }

            public ICollection<TreeNode> Transitions
            {
                get { return hash.Values; }
            }


            public ICollection<TSymbol[]> Results
            {
                get { return results; }
            }
        }
    }
}
