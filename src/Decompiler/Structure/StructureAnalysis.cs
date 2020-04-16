#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Structure
{
    /// <summary>
    /// This class starts with the basic block control graph of a decompiled 
    /// procedure and converts it into high-level structured code.
    /// </summary>
    /// <remarks>
    /// Inspired by the algorithm described in:
    ///  Native x86 Decompilation using Semantics-Preserving Structural Analysis
    ///  and Iterative Control-Flow Structuring.
    /// </remarks>
    public class StructureAnalysis : IStructureAnalysis
    {
        private readonly Program program;
        private readonly Procedure proc;
        private DirectedGraph<Region> regionGraph;
        private Region entry;
        private DominatorGraph<Region> doms;
        private DominatorGraph<Region> postDoms;
        private Queue<Tuple<Region, ISet<Region>>> unresolvedCycles;
        private Queue<Region> unresolvedSwitches;
        private readonly DecompilerEventListener eventListener;

        public StructureAnalysis(DecompilerEventListener listener, Program program, Procedure proc)
        {
            this.eventListener = listener;
            this.program = program;
            this.proc = proc;
        }

        public void Structure()
        {
            var cfgc = new ControlFlowGraphCleaner(proc);
            cfgc.Transform();
            var ccc = new CompoundConditionCoalescer(proc);
            ccc.Transform();
            proc.Body = new List<AbsynStatement>();
            var reg = Execute();
            //$REVIEW: yeecch. Should return the statements, and 
            // caller decides what to do with'em. Probably 
            // return an abstract Procedure, rather than overloading
            // the IR procedure.
            proc.Body.AddRange(reg.Statements);

            // Post processing steps
            var flr = new ForLoopRewriter(proc);
            flr.Transform();
            var trrm = new TailReturnRemover(proc);
            trrm.Transform();
            var pp = new ProcedurePrettifier(proc);
            pp.Transform();
        }

        /// <summary>
        /// Executes the core of the analysis
        /// </summary>
        /// <remarks>
        /// The algorithm visits nodes in post-order in each iteration. This
        /// means that all descendants of a node will be visited (and
        /// hence had the chance to be reduced) before the node itself.
        /// The algorithm’s behavior when visiting node _n_
        /// depends on hether the region at _n_
        /// is acyclic (has no loop) or not. For an acyclic region, the 
        /// algorithm tries to match the subgraph
        /// at _n_to an acyclic schemas (3.2). If there is no match,
        /// and the region is a switch candidate, then it attempts to
        /// refine the region at _n_ into a switch region.
        ///             
        /// If _n_ is cyclic, the algorithm 
        /// compares the region at _n_ to the cyclic schemata.
        /// If this fails, it refines _n_ into a loop (3.6).
        /// 
        /// If both matching and refinement do not make progress, the
        /// current node _n_ is then skipped for the current iteration of
        /// the algorithm.  If there is an iteration in which all
        /// nodes are skipped, i.e., the algorithm makes no progress, then
        /// the algorithm employs a last resort refinement (3.7) to
        /// ensure that progress can be made in the next iteration.
        /// </remarks>
        public Region Execute()
        {
            var result = BuildRegionGraph(proc);
            this.regionGraph = result.Item1;
            this.entry = result.Item2;
            int iterations = 0;
            int oldCount;
            int newCount;
            do
            {
                if (eventListener.IsCanceled())
                    break;
                ++iterations;
                if (iterations > 1000)
                {
                    eventListener.Warn(
                        eventListener.CreateProcedureNavigator(program, proc),
                        "Structure analysis stopped making progress, quitting. Please report this issue at https://github.com/uxmal/reko");
                    DumpGraph();
                    break;
                }

                oldCount = regionGraph.Nodes.Count;
                this.doms = new DominatorGraph<Region>(this.regionGraph, result.Item2);
                this.unresolvedCycles = new Queue<Tuple<Region, ISet<Region>>>();
                this.unresolvedSwitches = new Queue<Region>();
                var postOrder = new DfsIterator<Region>(regionGraph).PostOrder(entry).ToList();

                bool didReduce = false;
                foreach (var n in postOrder)
                {
                    Probe();
                    didReduce = false;
                    do
                    {
                        if (eventListener.IsCanceled())
                            break;
                        didReduce = ReduceAcyclic(n);
                        if (!didReduce && IsCyclic(n))
                        {
                            didReduce = ReduceCyclic(n);
                        }
                    } while (didReduce);
                }
                newCount = regionGraph.Nodes.Count;
                if (newCount == oldCount && newCount > 1)
                {
                    // Didn't make any progress this round,
                    // try refining unstructured regions
                    ProcessUnresolvedRegions();
                }
            } while (regionGraph.Nodes.Count > 1);
            return entry;
        }

        /// <summary>
        /// Handy place to put breakpoints during debugging of structuring algorithm.
        /// </summary>
        [Conditional("DEBUG")]
        private void Probe()
        {
        }

        private DominatorGraph<Region> BuildPostDoms()
        {
            var revGraph = new ReverseGraph(regionGraph);
            var exitNode = new Region(new Block(proc, "DummyExitBlock")) { Type = RegionType.Tail };
            revGraph.Nodes.Add(exitNode);
            var tailRegions = regionGraph.Nodes.Where(n => n.Type == RegionType.Tail);
            foreach (var r in tailRegions)
            {
                revGraph.AddEdge(exitNode, r);
            }
            return new DominatorGraph<Region>(revGraph, exitNode);
        }

        /// <summary>
        /// Builds a graph of regions based on the basic blocks of the code.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static Tuple<DirectedGraph<Region>, Region> BuildRegionGraph(Procedure proc)
        {
            var btor = new Dictionary<Block, Region>();
            var regs = new DiGraph<Region>();
            var regionFactory = new RegionFactory();
            foreach (var b in proc.ControlGraph.Blocks)
            {
                if (b.Pred.Count == 0 && b != proc.EntryBlock ||
                    b == proc.ExitBlock)
                    continue;
                var reg = regionFactory.Create(b);
                btor.Add(b, reg);
                regs.AddNode(reg);
            }
            foreach (var b in proc.ControlGraph.Blocks)
            {
                if (b.Pred.Count == 0 && b != proc.EntryBlock)
                    continue;
                btor.TryGetValue(b, out var from);
                foreach (var s in b.Succ)
                {
                    if (s == proc.ExitBlock)
                        continue;
                    var to = btor[s];
                    regs.AddEdge(from, to);
                }
                if (from != null)
                {
                    if (regs.Successors(from).Count == 0)
                        from.Type = RegionType.Tail;
                }
            }

            foreach (var reg in regs.Nodes.ToList())
            {
                if (regs.Predecessors(reg).Count == 0 && reg != btor[proc.EntryBlock])
                {
                    regs.Nodes.Remove(reg);
                }
            }
            return new Tuple<DirectedGraph<Region>, Region>(regs, btor[proc.EntryBlock]);
        }

        /// <summary>
        /// Determines if n is the header of a cyclic set of regions.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private bool IsCyclic(Region n)
        {
            return regionGraph.Predecessors(n).Any(pred => pred == n || IsBackEdge(pred, n));
        }

        private bool IsBackEdge(Region a, Region b)
        {
            return doms.DominatesStrictly(b, a);
        }

        /// <summary>
        /// Attempts to match and reduce acyclic region.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>True if a reduction occurred</returns>
        public bool ReduceAcyclic(Region n)
        {
            bool didReduce = false;
            switch (n.Type)
            {
            case RegionType.Condition:
                didReduce = ReduceIfRegion(n);
                break;
            case RegionType.IncSwitch:
                didReduce = ReduceSwitchRegion(n);
                break;
            case RegionType.Linear:
                didReduce = ReduceSequence(n);
                break;
            case RegionType.Tail:
                didReduce = false;
                break;
            default:
                throw new NotImplementedException();
            }
            Probe();
            return didReduce;
        }

        private void EnqueueUnresolvedRegion(Region switchHead)
        {
            // Do not refine switch region if there are unresolved cycles
            if (unresolvedCycles.Count == 0)
                this.unresolvedSwitches.Enqueue(switchHead);
        }

        private void EnqueueUnresolvedRegion(Region head, ISet<Region> loop)
        {
            // Do not refine cycle if there are unresolved switches
            if (unresolvedSwitches.Count == 0)
                this.unresolvedCycles.Enqueue(Tuple.Create(head, loop));
        }

        public bool ProcessUnresolvedRegions()
        {
            if (unresolvedCycles.Count != 0)
            {
                var cycle = unresolvedCycles.Dequeue();
                if (RefineLoop(cycle.Item1, cycle.Item2))
                    return true;
            }
            if (unresolvedSwitches.Count != 0)
            {
                var switchHead = unresolvedSwitches.Dequeue();
                RefineIncSwitch(switchHead);
                return true;
            }
            var postOrder = new DfsIterator<Region>(regionGraph).PostOrder(entry).ToList();
            foreach (var n in postOrder)
            {
                if (VirtualizeReturn(n))
                    return true;
            }
            foreach (var n in postOrder)
            {
                if (CoalesceTailRegion(n, regionGraph.Nodes))
                    return true;
            }
            foreach (var n in postOrder)
            {
                if (LastResort(n))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Replace edge to return statement with just return statement
        /// </summary>
        private bool VirtualizeReturn(Region n)
        {
            VirtualEdge returnEdge = null;
            foreach (var s in regionGraph.Successors(n))
                if (s.IsReturn)
                    returnEdge = new VirtualEdge(n, s, VirtualEdgeType.Goto);
            if (returnEdge != null)
            {
                VirtualizeEdge(returnEdge);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Identifies if-then or if-then-else schemas in the region graph
        /// and reduces them out of the graph.
        /// </summary>
        /// <param name="n">The header of the possible if-region</param>
        /// <returns></returns>
        private bool ReduceIfRegion(Region n)
        {
            var ss = regionGraph.Successors(n).ToArray();
            var el = ss[0];
            var th = ss[1];
            var elS = LinearSuccessor(el);
            var thS = LinearSuccessor(th);
            if (elS == th)
            {
                if (RefinePredecessor(n, el))
                    return false;
                // Collapse (If else) into n.
                n.Statements.Add(new AbsynIf(n.Expression.Invert(), el.Statements));
                regionGraph.RemoveEdge(n, el);
                if (elS != null)
                    regionGraph.RemoveEdge(el, elS);
                RemoveRegion(el);
                n.Type = RegionType.Linear;
                n.Expression = null;
                return true;
            }
            else if (thS == el)
            {
                if (RefinePredecessor(n, th))
                    return false;
                // Collapse (if-then) into n
                n.Statements.Add(new AbsynIf(n.Expression, th.Statements));
                regionGraph.RemoveEdge(n, th);
                if (thS != null)
                    regionGraph.RemoveEdge(th, thS);
                RemoveRegion(th);
                n.Type = RegionType.Linear;
                n.Expression = null;
                return true;
            }
            else if (elS != null && elS == thS)
            {
                if (RefinePredecessor(n, th) |
                    RefinePredecessor(n, el))
                    return false;

                // Collapse (If then else) into n.
                n.Statements.Add(new AbsynIf(n.Expression.Invert(), el.Statements, th.Statements));
                regionGraph.RemoveEdge(n, el);
                regionGraph.RemoveEdge(n, th);
                regionGraph.RemoveEdge(el, elS);
                regionGraph.RemoveEdge(th, thS);
                RemoveRegion(th);
                RemoveRegion(el);
                regionGraph.AddEdge(n, elS);
                n.Type = RegionType.Linear;
                n.Expression = null;
                return true;
            }
            return false;
        }

        private bool ReduceSequence(Region n)
        {
            var s = regionGraph.Successors(n).First();
            if (regionGraph.Predecessors(s).Count == 1)
            {
                // Sequence!
                Debug.Print("Concatenated {0} and {1}", n.Block.Name, s.Block.Name);
                n.Type = s.Type;
                n.Expression = s.Expression;
                n.Statements.AddRange(s.Statements);
                regionGraph.RemoveEdge(n, s);
                ReplaceSuccessors(s, n);
                RemoveRegion(s);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Switch regions.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private bool ReduceSwitchRegion(Region n)
        {
            bool irregularEntries = AreIrregularEntries(n);
            var follow = GetSwitchFollow(n);
            if (!irregularEntries && (follow != null || AllCasesAreTails(n)))
            {
                return ReduceIncSwitch(n, follow);
            }

            // It's a switch region, but we are unable to collapse it.
            // Schedule it for refinement after the whole graph has been
            // traversed.
            EnqueueUnresolvedRegion(n);

            return false;
        }




#if NILZ
3.4  Switch Refinement
If the subgraph at node _n_ is acyclic but fails to match a
known schema, we try to refine the subgraph into a switch.
Regions that would match a switch schema in Table 3
but contain extra edges are switch candidates. A switch
candidate can fail to match the switch schema if it has
extra incoming edges or multiple successors. For instance,
the nodes in the IncSwitch [] box in Figure  4  would not
be identified as an IncSwitch [] region because there is an
extra incoming edge to the default case node.

We first refine switch candidates by ensuring that the
switch head is the only predecessor for each case node.
We remove any other incoming edge by virtualizing them.
The next step is to ensure there is a single successor of all
nodes in the switch. To find the successor, we first identify
the immediate post-dominator of the switch head. If this
node is the successor of any of the case nodes, we select
it as the switch successor. If not, we select the node that
(1) is a successor of a case node, (2) is not a case node
itself, and (3) has the highest number of incoming edges
from case nodes. After we have identified the successor,
we remove all outgoing edges from the case nodes to other
nodes by virtualizing them

After  refinement,  a  switch  candidate  is  usually  col-
lapsed to a IncSwitch[] region. For instance, a common
implementation strategy for switches is to redirect inputs
handled by the default case (e.g., x > 20) to a default
node, and use a jump table for the remaining cases (e.g.,
x in {0..20}.   This relationship is depicted in Figure
 4,
along with the corresponding region types. Because the
jump table only handles a few cases, it is recognized as an
IncSwitch[]. However, because the default node handles
all other cases, together they constitute a Switch[].

#endif

        /// <summary>Refines an incomplete switch statement</summary>
        /// <remarks>
        /// A switch candidate is refined by first virtualizing incoming
        /// edges to any node other than the switch head.
        /// The next step is to ensure there is a single successor of
        /// all nodes in the switch. The immediate post-dominator
        /// of the switch head is selected as the successor if it is the
        /// successor of any of the case nodes. Otherwise, the node
        /// that (1) is a successor of a case node, (2) is not a case
        /// node itself, and (3) has the highest number of incoming
        /// edges from case nodes is chosen as the successor. After
        /// the successor has been identified, any outgoing edge from
        /// the switch that does not go to the successor is virtualized.
        /// [Pavel Tomin's note. Virtualizing of all outgoing edges
        /// described by Schwartz causes incorrect refinement of switch
        /// with irregular case exits. Use loop lexical nodes definition
        /// method to find case body. Then virtualize any edge
        /// leaving the case body that does not go to the successor]
        /// After refinement, a switch candidate is usually collapsed
        /// to a IncSwitch[·] region. For instance, a common
        /// implementation strategy for switches is to redirect inputs
        /// handled by the default case (e.g., x > 20) to a default
        /// node, and use a jump table for the remaining cases (e.g.,
        /// x in [0,20]). This relationship is depicted in Figure 4,
        /// along with the corresponding region types. Because the
        /// jump table only handles a few cases, it is recognized as an
        /// IncSwitch[·]. However, because the default node handles
        /// all other cases, together they constitute a Switch[·].
        /// </remarks>
        private void RefineIncSwitch(Region n)
        {
            if (VirtualizeIrregularSwitchEntries(n))
                return;
            var follow = FindIrregularSwitchFollowRegion(n);
            var switchBody = FindSwitchBody(n, follow);
            if (VirtualizeIrregularSwitchExits(switchBody, follow))
                return;
            var switchNodes = switchBody.Values.Aggregate(
                (s, nodes) => { s.UnionWith(nodes); return s; });
            foreach (var node in switchNodes)
            {
                if (CoalesceTailRegion(node, switchNodes))
                    return;
            }
            LastResort(switchNodes);
        }

        private bool VirtualizeIrregularSwitchEntries(Region n)
        {
            bool virtualized = false;
            var vEdges = new List<VirtualEdge>();
            foreach (var s in regionGraph.Successors(n).Distinct())
            {
                foreach (var sp in regionGraph.Predecessors(s))
                {
                    if (sp != n)
                        vEdges.Add(new VirtualEdge(sp, s, VirtualEdgeType.Goto));
                }
            }
            foreach (var vEdge in vEdges)
            {
                virtualized = true;
                VirtualizeEdge(vEdge);
            }
            return virtualized;
        }

        /// <summary>
        ///  To find the successor, we first identify
        /// the immediate post-dominator of the switch head. If this
        /// node is the successor of any of the case nodes, we select
        /// it as the switch successor. If not, we select the node that
        /// (1) is a successor of a case node, (2) is not a case node
        /// itself, and (3) has the highest number of incoming edges
        /// from case nodes.
        /// </summary>
        private Region FindIrregularSwitchFollowRegion(Region n)
        {
            this.postDoms = BuildPostDoms();
            var immPDom = this.postDoms.ImmediateDominator(n);
            var caseNodes = regionGraph.Successors(n).ToHashSet();
            if (caseNodes.Any(s => regionGraph.Successors(s).Contains(immPDom)))
                return immPDom;

            int incoming(Region r)
            {
                return regionGraph.Predecessors(r)
                    .Where(p => caseNodes.Contains(p))
                    .Count();
            }
            var candidates = caseNodes.SelectMany(c => regionGraph.Successors(c))
                .Where(c => !caseNodes.Contains(c))
                .ToList();
            var best = candidates
                .Select(c => new {
                    Region = c,
                    Score = incoming(c)
                })
                .OrderByDescending(c => c.Score)
                .First();
            return best.Region;
        }

        private IDictionary<Region, ISet<Region>> FindSwitchBody(
            Region n, Region follow)
        {
            var caseNodesMap = new Dictionary<Region, ISet<Region>>();
            var caseEntries = regionGraph.Successors(n).ToHashSet();
            foreach (var c in caseEntries)
            {
                var caseSet = new HashSet<Region>() { c };
                caseNodesMap[c] = GetLexicalNodes(c, follow, caseSet);
            }
            return caseNodesMap;
        }

        /// <summary>
        /// After we have identified the successor of a switch, we remove 
        /// any edge leaving the case body that does not go to the successor by
        /// virtualizing them.
        /// </summary>
        private bool VirtualizeIrregularSwitchExits(
            IDictionary<Region, ISet<Region>> switchBody, Region follow)
        {
            foreach (var caseBody in switchBody.Values)
            {
                if (VirtualizeIrregularCaseExits(follow, caseBody))
                    return true;
            }
            return false;
        }

        private bool VirtualizeIrregularCaseExits(Region follow, ISet<Region> caseBody)
        {
            bool virtualized = false;
            var vEdges = new List<VirtualEdge>();
            foreach (var n in caseBody)
            {
                var leavingNodes = regionGraph.Successors(n).
                    Where(s => !caseBody.Contains(s) && s != follow);
                foreach (var s in leavingNodes)
                {
                    vEdges.Add(new VirtualEdge(n, s, VirtualEdgeType.Goto));
                }
            }
            foreach (var vEdge in vEdges)
            {
                virtualized = true;
                VirtualizeEdge(vEdge);
            }
            return virtualized;
        }

        private bool AreIrregularEntries(Region n)
        {
            foreach (var s in regionGraph.Successors(n))
            {
                if (regionGraph.Predecessors(s).Where(p => (p != n)).Any())
                    return true;
            }
            return false;
        }

        private Region GetSwitchFollow(Region n)
        {
            Region follow = null;
            foreach (var s in regionGraph.Successors(n))
            {
                var ss = LinearSuccessor(s);
                if (s.Type != RegionType.Tail)
                {
                    if (ss == null)
                        return null;
                    if (follow == null)
                        follow = ss;
                    else if (ss != follow)
                        return null;
                }
            }
            return follow;
        }

        private bool AllCasesAreTails(Region n)
        {
            return regionGraph.Successors(n).All(s => s.Type == RegionType.Tail);
        }

        private bool ReduceIncSwitch(Region n, Region follow)
        {
            //$REVIEW: workaround for when the datatype of n.Expression
            // is non-integral. What causes this?
            if (!(n.Expression.DataType is PrimitiveType pt))
            {
                eventListener.Warn(eventListener.CreateBlockNavigator(this.program, n.Block), "Non-integral switch expression");
                pt = PrimitiveType.CreateWord(n.Expression.DataType.BitSize);
            }
            var cases = CollectSwitchCases(n);
            var stms = new List<AbsynStatement>();
            foreach (var succ in cases.Keys)
            {
                foreach (int c in cases[succ])
                {
                    stms.Add(new AbsynCase(Constant.Create(pt, c)));
                }
                stms.AddRange(succ.Statements);
                if (succ.Type != RegionType.Tail)
                {
                    stms.Add(new AbsynBreak());
                }
                cases[succ].ForEach(c => regionGraph.RemoveEdge(n, succ));
                if (follow != null)
                {
                    regionGraph.RemoveEdge(succ, follow);
                }
                RemoveRegion(succ);
            }
            var sw = new AbsynSwitch(n.Expression, stms);
            n.Statements.Add(sw);
            n.Expression = null;
            if (follow != null)
            {
                n.Type = RegionType.Linear;
                regionGraph.AddEdge(n, follow);
            }
            else
            {
                n.Type = RegionType.Tail;
            }
            return true;
        }

        /// <summary>
        /// Collects the cases of a switch statement such that cases with
        /// the same destination region are collected in the same list
        /// </summary>
        /// <param name="n"></param>
        /// <returns>A mapping from Region to a list of the case values
        /// that jump to that region.</returns>
        private Dictionary<Region, List<int>> CollectSwitchCases(Region n)
        {
            var succs = regionGraph.Successors(n).ToArray();
            var cases = new Dictionary<Region, List<int>>();
            for (int i = 0; i < succs.Length; ++i)
            {
                if (!cases.ContainsKey(succs[i]))
                    cases.Add(succs[i], new List<int>());
                cases[succs[i]].Add(i);
            }
            return cases;
        }

        /// <summary>
        /// Finds all predecessors of <paramref name="s"/> that aren't 
        /// the structured predecessor <paramref name="n"/>. 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="s"></param>
        /// <returns>True if unstructured predecessors were found.
        /// </returns>
        private bool RefinePredecessor(Region n, Region s)
        {
            ISet<Region> unstructuredPreds = new HashSet<Region>(regionGraph.Predecessors(s).Where(p => p != n));
            if (unstructuredPreds.Count == 0)
                return false;
            return true;
        }

        private void RemoveRegion(Region n)
        {
            Debug.Print("Removing region {0} from graph", n.Block.Name);
            regionGraph.Nodes.Remove(n);
            Probe();
        }

        /// <summary>
        /// If <paramref name="n"/> is linear region, returns
        /// its sucessor. Otherwise returns null.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private Region LinearSuccessor(Region n)
        {
            if (n.Type != RegionType.Linear)
                return null;
            return SingleSuccessor(n);
        }

        /// <summary>
        /// If <paramref name="n"/> has a single successor, returns
        /// it. Otherwise returns null.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private Region SingleSuccessor(Region n)
        {
            var succ = regionGraph.Successors(n);
            if (succ.Count != 1)
                return null;
            return succ.First();
        }

        private Region SinglePredecessor(Region n)
        {
            var succ = regionGraph.Predecessors(n);
            if (succ.Count != 1)
                return null;
            return succ.First();
        }

        [Conditional("DEBUG")]
        private void DumpGraph()
        {
            foreach (var n in regionGraph.Nodes)
            {
                Debug.Print("Node: {0} ({1})", n.Block.Name, n.Type);
                Debug.Print("  Pred: {0}", string.Join(" ", regionGraph.Predecessors(n).Select(p => p.Block.Name)));
                var sb = new StringWriter();
                n.Write(sb);
                Debug.Write(sb.ToString());
                if (n.Expression != null)
                {
                    Debug.Print("    Condition: {0}", n.Expression);
                }
                Debug.Print("  Succ: {0}", string.Join(" ", regionGraph.Successors(n).Select(s => s.Block.Name)));
                Debug.WriteLine("");
            }
            Debug.WriteLine("");
            Debug.WriteLine("====");
        }

        private void ReplaceSuccessors(Region old, Region gnu)
        {
            var oldSuccs = regionGraph.Successors(old).ToList();
            foreach (var s in oldSuccs)
            {
                regionGraph.RemoveEdge(old, s);
                regionGraph.AddEdge(gnu, s);
            }
            Probe();

        }
#if NILZ
    3.3
Tail Regions and Edge Virtualization
When no subgraphs in the CFG match known schemas,
the algorithm is stuck and the CFG must be refined before
more  structure  can  be  recovered.   The  insight  behind
refinement is that removing an edge from the CFG may
allow a schema to match, and iterative refinement
refersto the repeated application of refinement until a match is
possible.  Of course, each edge in the CFG represents a
possible control flow, and we must represent this control
flow in some other way to preserve the program semantics.
We call removing the edge in a way that preserves control
flow _virtualizing_ the edge, since the decompiled program
behaves as if the edge was present, even though it is not.
Edges are virtualized collapsing the
source node of the edge into a tail region (see 2.1). Tail
regions explicitly denote that there should be a control
transfer at the end of the region. For instance, to virtualize
the edge (n1, n2)  we remove the edge from the CFG,
insert a fresh label _l_ at the start of n2, and collapse
n1 to a tail region that denotes there should be a
goto l statement at the end of region n1
. Tail regions can also be translated into break or
continue statements when used
inside a switch or loop. Because the tail region explicitly
represents the control flow of the virtualized edge, it is
safe to remove the edge from the graph and ignore it when
doing future pattern matches.
#endif
        /// <summary>
        /// Edges are virtualized by removing them from the 
        /// graph, then adding a label in the destination block
        /// and collapse the source region to a tail.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void VirtualizeEdge(VirtualEdge vEdge)
        {
            AbsynStatement stm;
            if (vEdge.To.IsReturn)
            {
                // Goto to a return statement => just a return statement.
                var ret = (AbsynReturn)vEdge.To.Statements[0];
                Expression v = ret.Value?.CloneExpression();
                stm = new AbsynReturn(v);
            }
            else
            {
                switch (vEdge.Type)
                {
                case VirtualEdgeType.Continue: stm = new AbsynContinue(); break;
                case VirtualEdgeType.Break: stm = new AbsynBreak(); break;
                case VirtualEdgeType.Goto:
                    stm = new AbsynGoto(vEdge.To.Block.Name);
                    if (vEdge.To.Statements.Count == 0 || !(vEdge.To.Statements[0] is AbsynLabel))
                    {
                        vEdge.To.Statements.Insert(0, new AbsynLabel(vEdge.To.Block.Name));
                    }
                    break;
                default:
                    throw new InvalidOperationException();
                }
            }
            CollapseToTailRegion(vEdge.From, vEdge.To, stm);
            regionGraph.RemoveEdge(vEdge.From, vEdge.To);
            if (regionGraph.Predecessors(vEdge.To).Count == 0 && vEdge.To != entry)
            {
                if (vEdge.To.IsReturn)
                    RemoveRegion(vEdge.To);
                else
                    eventListener.Error(
                        eventListener.CreateProcedureNavigator(program, proc),
                        string.Format(
                            "Removing edge ({0}, {1}) caused losing of some code blocks",
                            vEdge.From.Block.Name,
                            vEdge.To.Block.Name));

                Probe();
            }
        }

        /// <summary>
        /// Appends the statement <paramref name="stm"/> to the list
        /// of statements in the <paramref name="from"/> region.
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="stm"></param>
        public void CollapseToTailRegion(Region from, Region to, AbsynStatement stm)
        {
            if (from.Type == RegionType.Condition)
            {
                var succs = regionGraph.Successors(from).ToArray();
                if (succs[0] == to)
                {
                    from.Expression = from.Expression.Invert();
                }
                var ifStm = new AbsynIf
                {
                    Condition = from.Expression,
                    Then = { stm }
                };
                from.Statements.Add(ifStm);
                from.Expression = null;
                Probe();
                from.Type = RegionType.Linear;
            }
            else if (from.Type == RegionType.Linear)
            {
                from.Statements.Add(stm);
                from.Type = RegionType.Tail;
                Probe();
            }
            else
            {
                DumpGraph();
                throw new NotImplementedException(string.Format("Can't collapse {0} ({1}) => {2}", from.Block.Name, from.Type, to.Block.Name));
            }
        }

#if NILZ

3.5
    Cyclic Regions
If the subgraph at node _n_ is cyclic, we first test if it matches
a cyclic pattern. The first step is to identify any loops of
which_n_ is the loop header. It is possible for a node to be
the loop header of multiple loops.  For instance, nested
do-while loops share a common loop header. We identify
distinct loops at node n by finding back edges pointing to
n (see 2.1). Each back edge (nb; n)
defines a loop body consisting of the nodes that can reach
nb without going through the loop header,
n. 

The loop with the smallest loop body is reduced first. 
        This must happen before the
larger loops can match the cyclic region patterns, because
there is no schema for nested loops.

There are three types of loops.
While[] loops test the exit condition before executing
the loop body,  whereas DoWhile[] loops test the exit
condition after executing the loop body.  If the exit con-
dition occurs in the middle of the loop body, the region is a
natural loop. Natural loops do not represent one particular
C looping construct, but can be caused by code such as
while (1)
{
    body1; 
    if (e)
        break;
    body2;
}
Notice  that  our  schema  for  natural  loops  contains  no
outgoing edges from the loop. This is not a mistake, but is
required for semantics-preservation. Because
NatLoop[] regions are decompiled to
while (1)
{
...
}

which has no exits, the body of the loop must trigger any
loop exits. The loop exits are represented by
a tail region,  which corresponds to a goto , break,  or
continue in the decompiled output.  These tail regions
are added during loop refinement, which we discuss next.

#endif

        public bool ReduceCyclic(Region n)
        {
            bool didReduce = false;
            var loopNodes = new LoopFinder<Region>(regionGraph, n, doms).LoopNodes;
            Region[] succs;
            for (;;)
            {
                succs = regionGraph.Successors(n).ToArray();
                if (succs.Length != 1 || !ReduceSequence(n))
                    break;
                Probe();
                didReduce = true;
            }
            foreach (var s in succs)
            {
                if (s == n)
                {
                    AbsynStatement loopStm;
                    if (succs.Length == 1)
                    {
                        // Infinite loop.
                        loopStm = new AbsynWhile(Constant.True(), s.Statements);
                        n.Type = RegionType.Tail;
                    }
                    else
                    {
                        // DoWhile!
                        var exp = s == succs[0]
                            ? n.Expression.Invert()
                            : n.Expression;
                        loopStm = new AbsynDoWhile(s.Statements, exp);
                        n.Type = RegionType.Linear;
                    }
                    n.Statements = new List<AbsynStatement> { loopStm };
                    n.Expression = null;
                    regionGraph.RemoveEdge(n, s);
                    regionGraph.RemoveEdge(s, n);
            Probe();
                    return true;
                }
            }
            // Should be condition. Switches should not match a cyclic pattern
            if (n.Type != RegionType.Condition)
                return didReduce;
            foreach (var s in succs)
            {
                if (LinearSuccessor(s) == n && SinglePredecessor(s) == n)
                {
                    // While!
                    var exp = s == succs[0] 
                        ? n.Expression.Invert() 
                        : n.Expression;
                    if (n.Statements.Count == 0)
                    {
                        n.Statements.Add(new AbsynWhile(exp, s.Statements));
                    }
                    else
                    {
                        var loop = new AbsynWhile(Constant.True(), n.Statements);
                        loop.Body.Add(new AbsynIf(
                            exp.Invert(),
                            new List<AbsynStatement> {
                                new AbsynBreak()
                            }));
                        loop.Body.AddRange(s.Statements);
                        n.Statements = new List<AbsynStatement> { loop };
                    }
                    n.Type = RegionType.Linear;
                    n.Expression = null;
                    regionGraph.RemoveEdge(n, s);
                    regionGraph.RemoveEdge(s, n);
                    RemoveRegion(s);
            Probe();
                    return true;
                }
            }

            // It's a cyclic region, but we are unable to collapse it.
            // Schedule it for refinement after the whole graph has been 
            // traversed.
            EnqueueUnresolvedRegion(n, loopNodes);
            return didReduce;
        }
#if NILZ
3.6 Loop Refinement 
            
If any loops were detected with loop header _n_ that did not
match a loop schema, we start loop refinement.  Cyclic
regions may fail to match loop schemas because 1) there
are multiple entrances to the loop, 2) there are too many
exits from the loop, or 3) the loop body cannot be collapsed
(i.e., is a proper region).

The first step of loop refinement is to ensure the loop
has a single entrance (nodes with incoming edges from
outside the loop).  If there are multiple entrances to the
loop, we select the one with the most incoming edges, and
virtualize the other entrance edges.

The next step is to identify the type of loop we have.
If there is an exit edge from the loop header, we have
a While[] candidate.  If there is an outgoing edge from
the source of the loop’s back edge (see 2.1), we have
a DoWhile[] candidate.  Otherwise, we select any exit
edge and have a NatLoop []. The exit edge determines the
successor of the loop, i.e., the statement that is executed
immediately after the loop. The lexical successor in turn
determines which nodes are lexically contained in the loop.
Phoenix  virtualizes  any  edge  leaving  the  lexically
contained loop nodes other than the exit edge. Edges that
go to the loop header use the continue tail regions, while
edges that go to the loop successor use the break
regions. Any other virtualized edge becomes a goto.

In our first implementation, we considered the lexically
contained nodes to be the loop body defined by the loop’s
back edge. However, we found this definition introduced
goto statements when the original program had
break statements, as in Figure
 5(a). The puts("c") statement is not
in the loop body according to the standard definition
because it cannot reach the loop’s back edge, but it
is lexically contained in the loop. Obviously, to be able to use
the break statement, it must be lexically contained inside
the loop body, or there would be no loop to break out of.
Our observation is that the nodes lexically contained
in the loop should intuitively consist of the loop body
and any nodes that execute after the loop body but before the
successor.  More formally, this corresponds to the loop
body, and the nodes that are dominated by the loop header,
excluding any nodes reachable from the loop’s successor
without going through the loop header.   For example,
puts("c") in Figure  5(b)  is considered as a node that
executes between the loop body and the successor, and
thus Phoenix places it lexically inside the loop.  When
Phoenix uses the standard loop membership definition
used in structural analysis [31], Phoenix outputs
goto s, as in Figure  5(c)
. To quantify this, enabling the new loop
membership definition decreased the numbers of
goto's
Phoenix emitted by 45% (73 to 40) in our evaluation (4)

The last loop refinement step is to remove edges that
may prevent the loop body from being collapsed. This can
happen for instance because a goto was used in the input
program.  This step is only performed if the prior loop
refinement steps did not remove any edges during the latest
iteration of the algorithm. For this, we use the last resort
refinement on the loop body, which we describe below.
#endif
        private bool RefineLoop(Region head, ISet<Region> loopNodes)
        {
            head = EnsureSingleEntry(head, loopNodes);
            var (follow, latch) = DetermineFollowLatch(head, loopNodes);
            if (follow == null && latch == null)
                return false;
            var lexicalNodes = GetLexicalNodes(head, follow, loopNodes);
            var virtualized = VirtualizeIrregularExits(head, latch, follow, lexicalNodes);
            if (virtualized)
                return true;
            foreach (var n in lexicalNodes)
            {
                if (CoalesceTailRegion(n, lexicalNodes))
                    return true;
            }
            return LastResort(lexicalNodes);
        }

        private bool CoalesceTailRegion(Region n, ICollection<Region> regions)
        {
            var succs = regionGraph.Successors(n).ToArray();
            if (succs.Length == 2 && n.Type == RegionType.Condition)
            {
                var el = succs[0];
                var th = succs[1];

                if (succs[0].Type == RegionType.Tail && th.Type == RegionType.Tail &&
                    SinglePredecessor(el) == n && SinglePredecessor(th) == n)
                {
                    // Both successors are tails.
                    n.Statements.Add(new AbsynIf(n.Expression, th.Statements, el.Statements));
                    regionGraph.RemoveEdge(n, el);
                    regionGraph.RemoveEdge(n, th);
                    RemoveRegion(el);
                    RemoveRegion(th);
                    n.Expression = null;
                    n.Type = RegionType.Tail;
            Probe();
                    return true;
                }
                if (regions.Contains(el) && el.Type == RegionType.Tail && SinglePredecessor(el) == n)
                {
                    var e = n.Expression.Invert();
                    n.Statements.Add(new AbsynIf(e, el.Statements));
                    regionGraph.RemoveEdge(n, el);
                    RemoveRegion(el);
                    n.Expression = null;
                    n.Type = RegionType.Linear;
            Probe();
                    return true;
                }
                if (regions.Contains(th) && th.Type == RegionType.Tail && SinglePredecessor(th) == n)
                {
                    var e = n.Expression;
                    n.Statements.Add(new AbsynIf(e, th.Statements));
                    regionGraph.RemoveEdge(n, th);
                    RemoveRegion(th);
                    n.Expression = null;
                    n.Type = RegionType.Linear;
            Probe();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ensure the loop has a single entrance (nodes with 
        /// incoming edges from outside the loop).  If there are 
        /// multiple entrances to the loop, we select the one with 
        /// the most incoming edges, and virtualize the other 
        /// entrance edges.
        /// </summary>
        private Region EnsureSingleEntry(Region head, ISet<Region> loopNodes)
        {
            var cinMax = CountIncomingEdges(head, loopNodes);
            var headMax = head;
            foreach (var n in loopNodes)
            {
                var cin = CountIncomingEdges(n, loopNodes);
                if (cin > cinMax)
                {
                    cinMax = cin;
                    headMax = n;
                }
            }
            return head;   //$TODO: not implemented yet.
        }

        private int CountIncomingEdges(Region n, ISet<Region> loopNodes)
        {
            return
                regionGraph.Predecessors(n)
                    .Where(p => !loopNodes.Contains(p))
                    .Count();
        }

        private (Region follow, Region latch) DetermineFollowLatch(Region head, ISet<Region> loopNodes)
        {
            var headSucc = regionGraph.Successors(head).ToArray();
            if (headSucc.Length == 2)
            {
                // If the head is a Conditional node and one of the edges 
                // leaves the loop, the head of that edge is the follow 
                // node of the loop.
                Region follow = null;
                if (!loopNodes.Contains(headSucc[0]))
                {
                    follow = headSucc[0];
                }
                else if (!loopNodes.Contains(headSucc[1]))
                {
                    follow = headSucc[1];
                }
                if (follow != null)
                {
                    foreach (var latch in regionGraph.Predecessors(head))
                    {
                        if (IsBackEdge(latch, head) && LinearSuccessor(latch) == head)
                        {
                            return (follow, latch);
                        }
                    }
                }
            }
            foreach (var latch in regionGraph.Predecessors(head))
            {
                if (IsBackEdge(latch, head))
                {
                    var latchSuccs = regionGraph.Successors(latch).ToArray();
                    if (latchSuccs.Length == 2)
                    {
                        if (!loopNodes.Contains(latchSuccs[0]))
                            return (latchSuccs[0], latch);
                        if (!loopNodes.Contains(latchSuccs[1]))
                            return (latchSuccs[1], latch);
                    }
                }
            }
            return (null, null);
        }
        
        /// <summary>
        /// Nodes lexically contained consist of the loop body
        /// and any nodes that execute after the loop body but before the
        /// follow.  More formally, this corresponds to the loop
        /// body, and the nodes that are dominated by the loop header,
        /// excluding any nodes reachable from the loop’s successor
        /// without going through the loop header.   
        /// </summary>
        private ISet<Region> GetLexicalNodes(Region head, Region follow, ISet<Region> loopNodes)
        {
            var excluded = new HashSet<Region>();
            FindReachableRegions(follow, head, excluded);
            var lexNodes = new HashSet<Region>();
            var wl = new  WorkList<Region>(loopNodes);
            while (wl.GetWorkItem(out var item))
            {
                if (loopNodes.Contains(item))
                {
                    lexNodes.Add(item);
                    wl.AddRange(regionGraph.Successors(item).Where(s => !lexNodes.Contains(s)));
                }
                else if (doms.DominatesStrictly(head, item) && 
                    !excluded.Contains(item))
                {
                    lexNodes.Add(item);
                    wl.AddRange(regionGraph.Successors(item).Where(s => !lexNodes.Contains(s)));
                }
            }
            return lexNodes;
        }

        void FindReachableRegions(Region n, Region head, ISet<Region> regions)
        {
            regions.Add(n);
            foreach (var succ in regionGraph.Successors(n))
            {
                if (!regions.Contains(succ) && succ != head)
                    FindReachableRegions(succ, head, regions);
            }
        }

        public enum VirtualEdgeType
        {
            Goto,
            Break,
            Continue,
        }

        public enum LoopType
        {
            While,
            DoWhile,
        }

        private bool HasExitEdgeFrom(Region n, Region follow)
        {
            return regionGraph.Successors(n).Where(s => (s == follow)).Any();
        }

        private LoopType DetermineLoopType(Region header, Region latch, Region follow)
        {
            if (!HasExitEdgeFrom(latch, follow))
                return LoopType.While;
            if (!HasExitEdgeFrom(header, follow))
                return LoopType.DoWhile;
            if (header.Statements.Count > 0)
                return LoopType.DoWhile;
            return LoopType.While;
        }

        /// <summary>
        /// Virtualizes any edge leaving the lexically
        /// contained loop nodes other than the exit edge. Edges that
        /// go to the loop header use the continue tail regions, while
        /// edges that go to the loop successor use the break
        /// regions. Any other virtualized edge becomes a goto.
        /// </summary>
        /// <param name="header">The loop header</param>
        /// <param name="latch">The node that takes us back 
        /// to the top of the loop.</param>
        /// <param name="follow">The node that follows the loop.</param>
        /// <param name="lexicalNodes"></param>
        /// <returns>True if at least one edge was virtualized.</returns>
        private bool VirtualizeIrregularExits(Region header, Region latch, Region follow, ISet<Region> lexicalNodes)
        {
            bool didVirtualize = false;
            var loopType = DetermineLoopType(header, latch, follow);
            foreach (var n in lexicalNodes)
            {
                var vEdges = new List<VirtualEdge>();
                foreach (var s in regionGraph.Successors(n))
                {
                    if (s == header)
                    {
                        if (n != latch)
                            vEdges.Add(new VirtualEdge(n, s, VirtualEdgeType.Continue));
                    }
                    else if (!lexicalNodes.Contains(s))
                    {
                        if (s == follow)
                        {
                            if (loopType == LoopType.DoWhile && n != latch ||
                                loopType == LoopType.While && n != header)
                            {
                                vEdges.Add(new VirtualEdge(n, s, VirtualEdgeType.Break));
                            }
                        }
                        else
                            vEdges.Add(new VirtualEdge(n, s, VirtualEdgeType.Goto));
                    }
                }
                foreach (var edge in vEdges)
                {
                    didVirtualize = true;
                    VirtualizeEdge(edge);
                }
            }
            return didVirtualize;
        }

        /// <summary>
        /// If the algorithm does not collapse any nodes or perform any
        /// refinement during an iteration, we must remove an edge in
        /// the graph to allow it to make progress. We call this process
        /// the last resort refinement, because it has the lowest priority,
        /// and always allows progress to be made. Last resort refine-
        /// ment prefers to remove edges whose source does not domi-
        /// nate its target, nor whose target dominates its source. These
        /// edges can be thought of as cutting across the dominator
        /// tree. By removing them, we leave edges that reflect more
        /// structure because they reflect a dominator relationship
        /// </summary>
        private bool LastResort(Region n)
        {
            VirtualEdge vEdge = null;

            foreach (var s in regionGraph.Successors(n))
            {
                if (!doms.DominatesStrictly(n, s) && 
                    !doms.DominatesStrictly(s, n))
                {
                    vEdge = new VirtualEdge(n, s, VirtualEdgeType.Goto);
                    break;
                }
            }
            if (vEdge == null)
            {
                foreach (var s in regionGraph.Successors(n))
                {
                    if (!doms.DominatesStrictly(n, s))
                    {
                        vEdge = new VirtualEdge(n, s, VirtualEdgeType.Goto);
                        break;
                    }
                }
            }
            if (vEdge == null)
            {
                foreach (var p in regionGraph.Predecessors(n))
                {
                    if (!doms.DominatesStrictly(p, n))
                    {
                        vEdge = new VirtualEdge(p, n, VirtualEdgeType.Goto);
                        break;
                    }
                }
            }
            if (vEdge != null)
            {
                VirtualizeEdge(vEdge);
                return true;
            }
            else 
            {
                // Whoa, we're in trouble now....
                return false;
            }
        }

        private bool LastResort(ISet<Region> regions)
        {
            var vEdge = FindLastResortEdge( regions);
            if (vEdge != null)
            {
                VirtualizeEdge(vEdge);
                return true;
            }
            else
            {
                // Whoa, we're in trouble now....
                return false;
            }
        }

        private VirtualEdge FindLastResortEdge(ISet<Region> regions)
        {
            var edges = regions.SelectMany(
                n => regionGraph.Successors(n).
                Where(s => regions.Contains(s)).
                Select(s => new VirtualEdge(n, s, VirtualEdgeType.Goto)));

            foreach(var vEdge in edges)
                if (!doms.DominatesStrictly(vEdge.From, vEdge.To) &&
                    !doms.DominatesStrictly(vEdge.To, vEdge.From))
                    return vEdge;

            foreach (var vEdge in edges)
                if (!doms.DominatesStrictly(vEdge.From, vEdge.To))
                    return vEdge;

            return edges.FirstOrDefault();
        }

        public class VirtualEdge
        {
            public Region From;
            public Region To;
            public VirtualEdgeType Type;

            public VirtualEdge(Region from, Region to, VirtualEdgeType type)
            {
                this.From = from;
                this.To = to;
                this.Type = type;
            }
        }
    }
}
