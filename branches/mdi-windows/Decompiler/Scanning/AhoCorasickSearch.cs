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
    public class AhoCorasickSearch<C> : StringSearch<C> 
        where C : IComparable<C>
    {

        private TreeNode _root;
        private C[][] _keywords;


        public AhoCorasickSearch(C[][] keywords) : base(null)
        {
            Keywords = keywords;
        }
        public AhoCorasickSearch() : base(null)
        { 
        }

        private void BuildTree()
        {
            _root = new TreeNode(null, default(C));
            _root.Failure = _root;
            foreach (C[] p in _keywords)
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
                    C c = nd.Char;

                    while (r != null && !r.ContainsTransition(c)) 
                        r = r.Failure;
                    if (r == null)
                        nd.Failure = _root;
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        foreach (C[] result in nd.Failure.Results)
                            nd.AddResult(result);
                    }

                    // add child nodes to BFS list 
                    foreach (TreeNode child in nd.Transitions)
                        newNodes.Add(child);
                }
                nodes = newNodes;
            }
        }

        private void AddPatternToTree(C[] p)
        {
            TreeNode nd = _root;
            foreach (C c in p)
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
        public C[][] Keywords
        {
            get { return _keywords; }
            set
            {
                _keywords = value;
                BuildTree();
            }
        }


        public override IEnumerator<int> GetMatchPositions(C[] text)
        {
            var ret = new List<object>();
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
                foreach (C[] found in ptr.Results)
                {
                    yield return index - found.Length + 1;
                }
            }
        }

        class TreeNode
        {
            private C _char;
            private TreeNode _parent;
            private TreeNode _failure;
            private List<C[]> _results;
            private Dictionary<C,TreeNode> _transHash;

            public TreeNode(TreeNode parent, C c)
            {
                _char = c; _parent = parent;
                _results = new List<C[]>();

                _transHash = new Dictionary<C,TreeNode>();
            }


            public void AddResult(C [] result)
            {
                if (_results.Contains(result)) return;
                _results.Add(result);
            }

            public void AddTransition(TreeNode node)
            {
                _transHash.Add(node.Char, node);
            }


            public TreeNode GetTransition(C c)
            {
                TreeNode  t;
                if (_transHash.TryGetValue(c, out t))
                    return t;
                else
                    return null;
            }


            public bool ContainsTransition(C c)
            {
                return GetTransition(c) != null;
            }


            public C Char
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


            public ICollection<C[]> Results
            {
                get { return _results; }
            }
        }

    }

}
