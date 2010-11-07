/* Aho-Corasick text search algorithm implementation
 * 
 * For more information visit
 *		- http://www.cs.uku.fi/~kilpelai/BSA05/lectures/slides04.pdf
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Class for searching string for one or multiple 
    /// keywords using efficient Aho-Corasick search algorithm
    /// </summary>
    public class AhoCorasickSearch<Symbol> : StringSearch<Symbol> 
        where Symbol : IComparable<Symbol>
    {
        private TreeNode _root;
        private Symbol[][] _keywords;

        public AhoCorasickSearch(Symbol[][] keywords) : base(null)
        {
            Keywords = keywords;
        }

        public AhoCorasickSearch() : base(null)
        { 
        }

        private void BuildTree()
        {
            _root = new TreeNode(null, default(Symbol));
            _root.Failure = _root;
            foreach (Symbol[] p in _keywords)
            {
                AddPatternToTree(p);
            }

            FindFailureFunctions();
        }

        private void FindFailureFunctions()
        {
            var nodes = new List<TreeNode>();
            // level 1 nodes - fail to root node
            foreach (TreeNode nd in _root.Transitions)
            {
                nd.Failure = _root;
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
                    Symbol c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) 
                        r = r.Failure;
                    if (r == null)
                        nd.Failure = _root;
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        foreach (Symbol[] result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
        }

        private void AddPatternToTree(Symbol[] p)
        {
            TreeNode nd = _root;
            foreach (Symbol c in p)
            {
                TreeNode ndNew = null;
                foreach (TreeNode trans in nd.Transitions)
                {
                    if (trans.Char.CompareTo(c) == 0) 
                    { ndNew = trans; break; }
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
        /// it requieres rebuilding of keyword tree)
        /// </summary>
        public Symbol[][] Keywords
        {
            get { return _keywords; }
            set
            {
                _keywords = value;
                BuildTree();
            }
        }

        public override IEnumerator<int> GetMatchPositions(Symbol[] text)
        {
            var ptr = _root;
            for (var index = 0; index < text.Length; ++index)
            {
                TreeNode trans = null;
                while (trans == null)
                {
                    trans = ptr.GetTransition(text[index]);
                    if (ptr == _root)
                        break;
                    if (trans == null)
                        ptr = ptr.Failure;
                }
                if (trans != null)
                    ptr = trans;
                foreach (Symbol[] found in ptr.Results)
                {
                    yield return index - found.Length + 1;
                }
            }
        }

        private class TreeNode
        {
            private Symbol _char;
            private TreeNode _parent;
            private TreeNode _failure;
            private List<Symbol[]> _results;
            private Dictionary<Symbol,TreeNode> _transHash;

            public TreeNode(TreeNode parent, Symbol c)
            {
                _char = c; _parent = parent;
                _results = new List<Symbol[]>();

                _transHash = new Dictionary<Symbol,TreeNode>();
            }


            public void AddResult(Symbol [] result)
            {
                if (_results.Contains(result)) return;
                _results.Add(result);
            }

            public void AddTransition(TreeNode node)
            {
                _transHash.Add(node.Char, node);
            }


            public TreeNode GetTransition(Symbol c)
            {
                TreeNode  t;
                if (_transHash.TryGetValue(c, out t))
                    return t;
                else
                    return null;
            }

            public bool ContainsTransition(Symbol c)
            {
                return GetTransition(c) != null;
            }


            public Symbol Char
            {
                get { return _char; }
            }


            public TreeNode Parent
            {
                get { return _parent; }
            }


            public TreeNode Failure
            {
                get { return _failure; }
                set { _failure = value; }
            }


            public ICollection<TreeNode> Transitions
            {
                get { return _transHash.Values; }
            }


            public ICollection<Symbol[]> Results
            {
                get { return _results; }
            }
        }
    }
}
