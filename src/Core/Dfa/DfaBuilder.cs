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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Dfa
{
    /// <summary>
    /// Builds a DFA directly from a regular expression.
    /// </summary>
    public class DfaBuilder
    {
        private readonly Dictionary<IntermediateState, IntermediateState> Dstates;
        private readonly List<(int, int, int)> Dtran;
        private readonly NodeComparer nodeComparer;

        /// <summary>
        /// Constructs a DFA from the given pattern.
        /// </summary>
        /// <param name="pattern">Regular expression pattern.</param>
        public DfaBuilder(string pattern)
            : this(new PatternParser(pattern).Parse() ?? new TreeNode { Type = NodeType.EOS })
        {
        }

        /// <summary>
        /// Constructs a DFA from the given parse tree.
        /// </summary>
        /// <param name="node">Parse tree.</param>
        public DfaBuilder(TreeNode node)
        {
            this.ParseTree = node;
            this.nodeComparer = new NodeComparer();
            Dstates = new Dictionary<IntermediateState, IntermediateState>();
            Dtran = new List<(int, int, int)>();
        }

        /// <summary>
        /// The parse tree of the regular expression.
        /// </summary>
        public TreeNode ParseTree { get; private set; }

        /// <summary>
        /// DFA states.
        /// </summary>
        public State[]? States { get; set; }

        /// <summary>
        /// Transitions between states.
        /// </summary>
        public int[,]? Transitions { get; set; }

        private class IntermediateState
        {
            public IntermediateState(SortedSet<TreeNode> set, int number)
            {
                this.Number = number;
                this.Nodes = set;
                this.hash = Nodes.Aggregate(
                    0,
                    (h, n) => h ^ n.GetHashCode());
            }

            public readonly SortedSet<TreeNode> Nodes;
            public int Number;
            public bool Starts;
            public bool Accepts;
            private readonly int hash;

            public override bool Equals(object? obj)
            {
                if (obj is not IntermediateState that)
                    return false;
                if (Nodes.Count != that.Nodes.Count)
                    return false;
                var ea = Nodes.GetEnumerator();
                var eb = that.Nodes.GetEnumerator();
                while (ea.MoveNext() && eb.MoveNext())
                {
                    if (ea.Current != eb.Current)
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return hash;
            }

        }

        /// <summary>
        /// Builds the DFA automaton from the parse tree.
        /// </summary>
        public void BuildAutomaton()
        {
            BuildAutomaton(this.ParseTree);
        }

        private class NodeComparer : IComparer<TreeNode>
        {
            public int Compare(TreeNode? x, TreeNode? y)
            {
                return x!.Number - y!.Number;
            }
        }

        private IntermediateState CreateIntermediateState(ISet<TreeNode> nodeSet)
        {
            var state = new IntermediateState(
                new SortedSet<TreeNode>(nodeSet, nodeComparer),
                Dstates.Count);
            // DumpState(state);        // This is very verbose in the unit tests
            return state;
        }

        /// <summary>
        /// Builds the DFA automaton from the parse tree.
        /// </summary>
        /// <param name="tree">Parse tree.</param>
        internal void BuildAutomaton(TreeNode tree)
        {
            var unmarked = new Queue<IntermediateState>();
            var state = CreateIntermediateState(tree.FirstPos!);
            state.Starts = true;
            Dstates.Add(state, state);
            unmarked.Enqueue(state);
            while (unmarked.TryDequeue(out var T))
            {
                foreach (var p in T.Nodes.SelectMany(p => p.GetTransitionCharacters(), (p, c) => new { Node = p, Value = c }))
                {
                    var U = CreateIntermediateState(p.Node.FollowPos!);
                    Dstates.TryGetValue(U, out IntermediateState? dstate);
                    if (U.Nodes.Count() > 0 && dstate is null)
                    {
                        Dstates.Add(U, U);
                        unmarked.Enqueue(U);
                        dstate = U;
                    }
                    dstate!.Starts |= p.Node.Starts;
                    dstate.Accepts |= p.Node.FollowPos!.Any(n => n.Type == NodeType.EOS);
                    Dtran.Add((T.Number, (int) p.Value, dstate.Number));
                }
            }
            this.States = Dstates.Values
                .OrderBy(d => d.Number)
                .Select(d => new State { Number = d.Number, Starts = d.Starts, Accepts = d.Accepts })
                .ToArray();
            this.Transitions = Compact(Dtran);
        }

        [Conditional("DEBUG")]
        private static void DumpState(IntermediateState state)
        {
            Debug.Print("Created state {0}{1}{2}", state.Number, state.Starts ? " Starts" : "", state.Accepts ? " Accepts" : "");
            Debug.Print("    {0}", string.Join(",", state.Nodes.Select(n => n.Number.ToString())));
            Debug.Print("    {0}", string.Join(",", state.Nodes.Select(
                n => string.Format(
                    Char.IsControl((char)n.Value)
                        ? "\\x{0:X2}"
                        : "{0}",
                    Char.IsControl((char)n.Value)
                        ? (object)(int)n.Value
                        : (object)(char)n.Value))));
        }

        private int[,] Compact(List<(int, int, int)> Dtran)
        {
            var states = new int[Dstates.Count, 256];   //$TODO: alphabet size hard-wired to 256.
            foreach (var tuple in Dtran)
            {
                states[tuple.Item1, tuple.Item2] = tuple.Item3;
            }
            return states;
        }

        /// <summary>
        /// Builds node sets for the parse tree.
        /// </summary>
        public void BuildNodeSets()
        {
            BuildNodeSets(this.ParseTree);
        }

        /// <summary>
        /// Builds node sets for the parse tree.
        /// </summary>
        internal void BuildNodeSets(TreeNode node)
        {
            if (node is null)
                return;
            node.FollowPos = new HashSet<TreeNode>();
            switch (node.Type)
            {
            case NodeType.Epsilon:
                node.Nullable = true;
                node.FirstPos = new HashSet<TreeNode>();
                node.LastPos = new HashSet<TreeNode>();
                break;
            case NodeType.Cut:
                BuildNodeSets(node.Left!);
                BuildNodeSets(node.Right!);
                node.Nullable = node.Left!.Nullable && node.Right!.Nullable;
                var setcu = new HashSet<TreeNode>(node.Left.FirstPos!);
                if (node.Left.Nullable)
                {
                    setcu.UnionWith(node.Right!.FirstPos!);
                }
                node.FirstPos = setcu;
                setcu = new HashSet<TreeNode>(node.Right!.LastPos!);
                if (node.Right.Nullable)
                {
                    setcu.UnionWith(node.Left.LastPos!);
                }
                node.LastPos = setcu;
                foreach (var n in node.Right.FirstPos!)
                {
                    n.Starts = true;
                }
                foreach (var i in node.Left.LastPos!)
                {
                    i.FollowPos!.UnionWith(node.Right.FirstPos);
                }
                break;
            case NodeType.Star:
                BuildNodeSets(node.Left!);
                node.Nullable = true;
                node.FirstPos = new HashSet<TreeNode>(node.Left!.FirstPos!);
                node.LastPos = new HashSet<TreeNode>(node.Left.LastPos!);
                foreach (var i in node.Left.LastPos!)
                {
                    i.FollowPos!.UnionWith(node.FirstPos);
                }
                break;
            case NodeType.Plus:
                BuildNodeSets(node.Left!);
                node.Nullable = node.Left!.Nullable;
                node.FirstPos = new HashSet<TreeNode>(node.Left.FirstPos!);
                node.LastPos = new HashSet<TreeNode>(node.Left.LastPos!);
                foreach (var i in node.Left.LastPos!)
                {
                    i.FollowPos!.UnionWith(node.FirstPos);
                }

                break;
            case NodeType.Or:
                BuildNodeSets(node.Left!);
                BuildNodeSets(node.Right!);
                node.Nullable = node.Left!.Nullable | node.Right!.Nullable;

                var set = new HashSet<TreeNode>(node.Left.FirstPos!);
                set.UnionWith(node.Left.FirstPos!);
                node.FirstPos = set;

                set = new HashSet<TreeNode>(node.Left.LastPos!);
                set.UnionWith(node.Left.LastPos!);
                node.LastPos = set;
                break;
            case NodeType.Cat:
                BuildNodeSets(node.Left!);
                BuildNodeSets(node.Right!);
                node.Nullable = node.Left!.Nullable & node.Right!.Nullable;
                var setc = new HashSet<TreeNode>(node.Left.FirstPos!);
                if (node.Left.Nullable)
                {
                    setc.UnionWith(node.Right.FirstPos!);
                }
                node.FirstPos = setc;

                setc = new HashSet<TreeNode>(node.Right.LastPos!);
                if (node.Right.Nullable)
                {
                    setc.UnionWith(node.Left.LastPos!);
                }
                node.LastPos = setc;

                foreach (var i in node.Left.LastPos!)
                {
                    i.FollowPos!.UnionWith(node.Right.FirstPos!);
                }
                break;
            default:
                // A regular character.
                node.Nullable = false;
                node.FirstPos = new HashSet<TreeNode> { node };
                node.LastPos = new HashSet<TreeNode> { node };
                break;
            }
        }

        /// <summary>
        /// Extend the parse tree with an EOS node.
        /// </summary>
        public void ExtendWithEos()
        {
            int max = MaxSignificant(ParseTree) + 1;
            ParseTree = new TreeNode { Type = NodeType.Cat, Left = ParseTree, Right = new TreeNode { Type = NodeType.EOS, Number = max } };
        }

        private int MaxSignificant(TreeNode node)
        {
            int max = node.Number;
            if (node.Left is not null)
                max = Math.Max(max, MaxSignificant(node.Left));
            if (node.Right is not null)
                max = Math.Max(max, MaxSignificant(node.Right));
            return max;
        }
    }
}
