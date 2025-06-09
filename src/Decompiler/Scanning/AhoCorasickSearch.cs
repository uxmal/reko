#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Scanning
{
        /// <summary>
        /// Interface containing all methods to be implemented
        /// by string search algorithm
        /// </summary>
        public interface IStringSearchAlgorithm<TSymbol>
        {
            #region Methods & Properties

            /// <summary>
            /// List of keywords to search for
            /// </summary>
            TSymbol[][] Keywords { get; set; }


            /// <summary>
            /// Searches passed text and returns all occurrences of any keyword
            /// </summary>
            /// <param name="text">Text to search</param>
            /// <returns>Array of occurrences</returns>
            StringSearchResult<TSymbol> [] FindAll(string text);

            /// <summary>
            /// Searches passed text and returns first occurrence of any keyword
            /// </summary>
            /// <param name="text">Text to search</param>
            /// <returns>First occurrence of any keyword (or StringSearchResult.Empty if text doesn't contain any keyword)</returns>
            StringSearchResult<TSymbol> FindFirst(string text);

            /// <summary>
            /// Searches passed text and returns true if text contains any keyword
            /// </summary>
            /// <param name="text">Text to search</param>
            /// <returns>True when text contains any keyword</returns>
            bool ContainsAny(string text);

            #endregion
        }

        /// <summary>
        /// Structure containing results of search 
        /// (keyword and position in original text)
        /// </summary>
        public struct StringSearchResult<TSymbol>
        {
            #region Members

            private int _index;
            private TSymbol[] _keyword;

            /// <summary>
            /// Initialize string search result
            /// </summary>
            /// <param name="index">Index in text</param>
            /// <param name="keyword">Found keyword</param>
            public StringSearchResult(int index, TSymbol[] keyword)
            {
                _index = index; _keyword = keyword;
            }


            /// <summary>
            /// Returns index of found keyword in original text
            /// </summary>
            public int Index
            {
                get { return _index; }
            }


            /// <summary>
            /// Returns keyword found by this result
            /// </summary>
            public TSymbol[] Keyword
            {
                get { return _keyword; }
            }


            /// <summary>
            /// Returns empty search result
            /// </summary>
            public static StringSearchResult<TSymbol> Empty
            {
                get { return new StringSearchResult<TSymbol>(-1, Array.Empty<TSymbol>()); }
            }

            #endregion
        }


        /// <summary>
        /// Class for searching string for one or multiple 
        /// keywords using efficient Aho-Corasick search algorithm
        /// </summary>
        public class AhoCorasickSearch2<TSymbol> : StringSearch<TSymbol>
            where TSymbol: IComparable<TSymbol>

        {
            #region Objects

            /// <summary>
            /// Tree node representing character and its 
            /// transition and failure function
            /// </summary>
            class TreeNode
            {
                #region Constructor & Methods

                /// <summary>
                /// Initialize tree node with specified character
                /// </summary>
                /// <param name="parent">Parent node</param>
                /// <param name="c">Character</param>
                public TreeNode(TreeNode parent, TSymbol c)
                {
                    _char = c; _parent = parent;
                    _results = new List<TSymbol[]>();
                    _resultsAr = Array.Empty<TSymbol[]>();

                    _transitionsAr = Array.Empty<TreeNode>();
                    _transHash = new Hashtable();
                }


                /// <summary>
                /// Adds pattern ending in this node
                /// </summary>
                /// <param name="result">Pattern</param>
                public void AddResult(TSymbol[] result)
                {
                    if (_results.Contains(result)) return;
                    _results.Add(result);
                    _resultsAr = _results.ToArray();
                }

                /// <summary>
                /// Adds trabsition node
                /// </summary>
                /// <param name="node">Node</param>
                public void AddTransition(TreeNode node)
                {
                    _transHash.Add(node.Char, node);
                    TreeNode[] ar = new TreeNode[_transHash.Values.Count];
                    _transHash.Values.CopyTo(ar, 0);
                    _transitionsAr = ar;
                }


                /// <summary>
                /// Returns transition to specified character (if exists)
                /// </summary>
                /// <param name="c">Character</param>
                /// <returns>Returns TreeNode or null</returns>
                public TreeNode GetTransition(TSymbol c)
                {
                    return (TreeNode)_transHash[c];
                }


                /// <summary>
                /// Returns true if node contains transition to specified character
                /// </summary>
                /// <param name="c">Character</param>
                /// <returns>True if transition exists</returns>
                public bool ContainsTransition(TSymbol c)
                {
                    return GetTransition(c) is not null;
                }

                #endregion
                #region Properties

                private readonly TSymbol _char;
                private readonly TreeNode _parent;
                private TreeNode _failure;
                private readonly List<TSymbol[]> _results;
                private TreeNode[] _transitionsAr;
                private TSymbol[][] _resultsAr;
                private readonly Hashtable _transHash;

                /// <summary>
                /// Character
                /// </summary>
                public TSymbol Char
                {
                    get { return _char; }
                }


                /// <summary>
                /// Parent tree node
                /// </summary>
                public TreeNode Parent
                {
                    get { return _parent; }
                }


                /// <summary>
                /// Failure function - descendant node
                /// </summary>
                public TreeNode Failure
                {
                    get { return _failure; }
                    set { _failure = value; }
                }


                /// <summary>
                /// Transition function - list of descendant nodes
                /// </summary>
                public TreeNode[] Transitions
                {
                    get { return _transitionsAr; }
                }


                /// <summary>
                /// Returns list of patterns ending by this letter
                /// </summary>
                public TSymbol[][] Results
                {
                    get { return _resultsAr; }
                }

                #endregion
            }

            #endregion
            #region Local fields

            private TreeNode _root;

            /// <summary>
            /// Keywords to search for
            /// </summary>
            private TSymbol[][] _keywords;

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize search algorithm (Build keyword tree)
            /// </summary>
            /// <param name="keywords">Keywords to search for</param>
            public AhoCorasickSearch2(TSymbol[][] keywords,
                bool scannedMemory,
                bool unscannedMemory) : base(null, scannedMemory, unscannedMemory)
            {
                Keywords = keywords;
            }


            #endregion
            #region Implementation

            /// <summary>
            /// Build tree from specified keywords
            /// </summary>
            void BuildTree()
            {
                // Build keyword tree and transition function
                _root = new TreeNode(null, default);
                foreach (var p in _keywords)
                {
                    // add pattern to tree
                    TreeNode nd = _root;
                    foreach (TSymbol c in p)
                    {
                        TreeNode ndNew = null;
                        foreach (TreeNode trans in nd.Transitions)
                        {
                            if (trans.Char.CompareTo(c) == 0) {
                                ndNew = trans; 
                                break; 
                            }
                        }
                        if (ndNew is null)
                        {
                            ndNew = new TreeNode(nd, c);
                            nd.AddTransition(ndNew);
                        }
                        nd = ndNew;
                    }
                    nd.AddResult(p);
                }

                // Find failure functions
                var nodes = new List<TreeNode>();
                // level 1 nodes - fail to root node
                foreach (TreeNode nd in _root.Transitions)
                {
                    nd.Failure = _root;
                    foreach (TreeNode trans in nd.Transitions) 
                        nodes.Add(trans);
                }
                // other nodes - using BFS
                while (nodes.Count != 0)
                {
                    var newNodes = new List<TreeNode>();
                    foreach (TreeNode nd in nodes)
                    {
                        var r = nd.Parent.Failure;
                        var c = nd.Char;

                        while (r is not null && !r.ContainsTransition(c)) r = r.Failure;
                        if (r is null)
                            nd.Failure = _root;
                        else
                        {
                            nd.Failure = r.GetTransition(c);
                            foreach (var result in nd.Failure.Results)
                                nd.AddResult(result);
                        }

                        // add child nodes to BFS list 
                        foreach (TreeNode child in nd.Transitions)
                            newNodes.Add(child);
                    }
                    nodes = newNodes;
                }
                _root.Failure = _root;
            }


            #endregion
            #region Methods & Properties

            /// <summary>
            /// Keywords to search for (setting this property is slow, because
            /// it requieres rebuilding of keyword tree)
            /// </summary>
            public TSymbol[][] Keywords
            {
                get { return _keywords; }
                set
                {
                    _keywords = value;
                    BuildTree();
                }
            }

            public override IEnumerable<int> GetMatchPositions(TSymbol[] stringToSearch)
            {
               return FindAll(stringToSearch).Select(s => s.Index);
            }

            /// <summary>
            /// Searches passed text and returns all occurrences of any keyword
            /// </summary>
            /// <param name="text">Text to search</param>
            /// <returns>Array of occurrences</returns>
            public StringSearchResult<TSymbol>[] FindAll(TSymbol[] text)
            {
                var ret = new List<StringSearchResult<TSymbol>>();
                TreeNode ptr = _root;
                int index = 0;

                while (index < text.Length)
                {
                    TreeNode trans = null;
                    while (trans is null)
                    {
                        trans = ptr.GetTransition(text[index]);
                        if (ptr == _root) 
                            break;
                        if (trans is null)
                            ptr = ptr.Failure;
                    }
                    if (trans is not null) ptr = trans;

                    foreach (var found in ptr.Results)
                        ret.Add(new StringSearchResult<TSymbol>(index - found.Length + 1, found));
                    index++;
                }
                return ret.ToArray();
            }


            /// <summary>
            /// Searches passed text and returns first occurrence of any keyword
            /// </summary>
            /// <param name="text">Text to search</param>
            /// <returns>First occurrence of any keyword (or StringSearchResult.Empty if text doesn't contain any keyword)</returns>
            public StringSearchResult<TSymbol> FindFirst(TSymbol[] text)
            {
                TreeNode ptr = _root;
                int index = 0;

                while (index < text.Length)
                {
                    TreeNode trans = null;
                    while (trans is null)
                    {
                        trans = ptr.GetTransition(text[index]);
                        if (ptr == _root) break;
                        if (trans is null) ptr = ptr.Failure;
                    }
                    if (trans is not null) ptr = trans;

                    foreach (var found in ptr.Results)
                        return new StringSearchResult<TSymbol>(index - found.Length + 1, found);
                    index++;
                }
                return StringSearchResult<TSymbol>.Empty;
            }


            /// <summary>
            /// Searches passed text and returns true if text contains any keyword
            /// </summary>
            /// <param name="text">Text to search</param>
            /// <returns>True when text contains any keyword</returns>
            public bool ContainsAny(TSymbol[] text)
            {
                TreeNode ptr = _root;
                int index = 0;

                while (index < text.Length)
                {
                    TreeNode trans = null;
                    while (trans is null)
                    {
                        trans = ptr.GetTransition(text[index]);
                        if (ptr == _root) break;
                        if (trans is null) ptr = ptr.Failure;
                    }
                    if (trans is not null) ptr = trans;

                    if (ptr.Results.Length > 0) return true;
                    index++;
                }
                return false;
            }

            #endregion
        }

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

        public AhoCorasickSearch(TSymbol[][] keywords, bool scannedMemory, bool unscannedMemory) 
            : base(null, scannedMemory, unscannedMemory)
        {
            Keywords = keywords;
        }

        public AhoCorasickSearch()
            : base(null, true, true)
        { 
        }

        private void BuildTree()
        {
            root = new TreeNode(null, default);
            root.Failure = null;
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

                    while (r is not null && !r.ContainsTransition(c)) 
                        r = r.Failure;
                    if (r is null)
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

                if (ndNew is null)
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

        /// <summary>
        /// Find all places where any one of the patterns is found in the text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override IEnumerable<int> GetMatchPositions(TSymbol[] text)
        {
            var ptr = root;
            for (var index = 0; index < text.Length; ++index)
            {
                TreeNode trans = null;
                while (trans is null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == root)
                        break;
                    if (trans is null)
                        ptr = ptr.Failure;
                }
                if (trans is not null)
                    ptr = trans;
                foreach (TSymbol[] found in ptr.Results)
                {
                    yield return index - found.Length + 1;
                }
            }
        }

        private class TreeNode
        {
            private readonly List<TSymbol[]> results;
            private readonly Dictionary<TSymbol,TreeNode> hash;

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
                if (hash.TryGetValue(c, out TreeNode t))
                    return t;
                else
                    return null;
            }

            public bool ContainsTransition(TSymbol c)
            {
                return GetTransition(c) is not null;
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

    // Matching with *-wildcards via Aho-Corasick. Matching with ?-wildcards via convolutions.
    //mport java.util.Arrays;

    public class AhoCorasick
    {

        const int ALPHABET_SIZE = 26;

        Node[] nodes;
        int nodeCount;

        private class Node
        {
            public int parent;
            public char charFromParent;
            public int suffLink = -1;
            public int[] children = new int[ALPHABET_SIZE];
            public int[] transitions = new int[ALPHABET_SIZE];
            public bool leaf;

            public Node()
            {
                Array.Fill(children, -1);
                Array.Fill(transitions, -1);
            }
        }

        public AhoCorasick(int maxNodes)
        {
            nodes = new Node[maxNodes];
            // create root
            nodes[0] = new Node();
            nodes[0].suffLink = 0;
            nodes[0].parent = -1;
            nodeCount = 1;
        }

        public void addString(String s)
        {
            int cur = 0;
            foreach (char ch in s)
            {
                int c = ch;
                if (nodes[cur].children[c] == -1)
                {
                    nodes[nodeCount] = new Node
                    {
                        parent = cur,
                        charFromParent = ch,
                    };
                    nodes[cur].children[c] = nodeCount++;
                }
                cur = nodes[cur].children[c];
            }
            nodes[cur].leaf = true;
        }

        public int suffLink(int nodeIndex)
        {
            Node node = nodes[nodeIndex];
            if (node.suffLink == -1)
                node.suffLink = node.parent == 0 ? 0 : transition(suffLink(node.parent), node.charFromParent);
            return node.suffLink;
        }

        public int transition(int nodeIndex, char ch)
        {
            int c = ch;
            Node node = nodes[nodeIndex];
            if (node.transitions[c] == -1)
                node.transitions[c] = node.children[c] != -1 ? node.children[c] : (nodeIndex == 0 ? 0 : transition(suffLink(nodeIndex), ch));
            return node.transitions[c];
        }

        // Usage example
        public static void main(String[] args)
        {
            var ahoCorasick = new AhoCorasick(1000);
            ahoCorasick.addString("bc");
            ahoCorasick.addString("abc");

            String s = "tabc";
            int node = 0;
            foreach (char ch in s)
            {
                node = ahoCorasick.transition(node, ch);
            }
            Console.Out.WriteLine(ahoCorasick.nodes[node].leaf);
        }
    }

}
