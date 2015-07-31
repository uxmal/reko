#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Structure.Schwartz
{
    // Based on:
    // Native x86 Decompilation using Semantics-Preserving Structural Analysis
    // and Iterative Control-Flow Structuring.

    public class ProcedureStructurer
    {
        private DirectedGraph<Region> regionGraph;
        private DirectedGraph<Region> newGraph;
        private  DominatorGraph<Region> doms;
        private Queue<Tuple<Region, ISet<Region>>> unresolvedCycles;

        public ProcedureStructurer()
        {

        }

        public Region Execute(Procedure proc)
        {
#if NILZ
        We focus on the novel aspects of our algorithm in this
paper and refer readers interested in any structural analysis
details elided to standard sources 
         
Like vanilla structural analysis, our algorithm visits
nodes in post-order in each iteration.   Intuitively,  this
means that all descendants of a node will be visited (and
hence had the chance to be reduced) before the node itself.
The algorithm’s behavior when visiting node _n_
depends on hether the region at _n_
is acyclic (has no loop) or not. For an acyclic region, the 
algorithm tries to match the subgraph
at _n_to an acyclic schemas (3.2). If there is no match,
and the region is a switch candidate, then it attempts to
refine the region at _n_ into a switch region (3.4).
            
If _n_ is cyclic, the algorithm 
compares the region at _n_ to the cyclic schemas (3.5)
If this fails, it refines _n_ into a loop (3.6).
If both matching and refinement do not make progress, the
current node _n_ is then skipped for the current iteration of
the algorithm.  If there is an iteration in which all
nodes are skipped, i.e., the algorithm makes no progress, then
the algorithm employs a last resort refinement (3.7) to
ensure that progress can be made in the next iteration.
#endif
            var result = BuildRegionGraph(proc);
            this.regionGraph = result.Item1;
            var entry = result.Item2;
            DumpGraph();
            newGraph = new DiGraph<Region>();
            do
            {
                this.doms = new DominatorGraph<Region>(this.regionGraph, result.Item2);
                this.unresolvedCycles = new Queue<Tuple<Region, ISet<Region>>>(); 
                var postOrder = new DfsIterator<Region>(regionGraph).PostOrder(entry).ToList();
                Debug.Print("Iterating....");
                DumpGraph();
                foreach (var n in postOrder) 
                {
                    entry = Dfs(n);
                }
                ProcessUnresolvedCycles();
            } while (regionGraph.Nodes.Count > 1);
            return entry;
        }

        private Region Dfs(Region n)
        {
            Debug.Print("DFS: visiting node {0}", n.Block.Name);
            if (!IsCyclic(n))
            {
                return MatchAcyclic(n);
            }
            else
            {
                return MatchCyclic(n);
            }
        }

        public void ProcessUnresolvedCycles()
        {
            if (unresolvedCycles.Count == 0)
                return;
            var cycle = unresolvedCycles.Dequeue();
            RefineLoop(cycle.Item1, cycle.Item2);
        }

        /// <summary>
        /// Builds a graph of regions based on the basic blocks of the code.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public Tuple<DirectedGraph<Region>, Region> BuildRegionGraph(Procedure proc)
        {
            var btor = new Dictionary<Block, Region>();
            var regs = new DiGraph<Region>();
            var regionFactory = new RegionFactory();
            foreach (var b in proc.ControlGraph.Blocks)
            {
                var reg = regionFactory.Create(b);
                btor.Add(b, reg);
                regs.AddNode(reg);
            }
            foreach (var b in proc.ControlGraph.Blocks)
            {
                foreach (var s in b.Succ)
                {
                    var from = btor[b];
                    var to = btor[s];
                    regs.AddEdge(from, to);
                }
            }
            return new Tuple<DirectedGraph<Region>, Region>(regs, btor[proc.EntryBlock]);
        }

        private bool IsCyclic(Region n)
        {
            return regionGraph.Predecessors(n).Any(pred => pred == n || IsBackEdge(pred, n));
        }

        private bool IsBackEdge(Region a, Region b)
        {
            return doms.DominatesStrictly(b, a);
        }


#if NILZ
3.2
    Acyclic Regions
The acyclic region types supported by Phoenix correspond
to the acyclic control flow operators in C: sequences, ifs,
and switches.  The schemas for these regions are shown in
Table 3. For example, the Seq[n1; ... nk] contains _k_ regions 
that  always  execute  in  the  listed  sequence.
IfThenElse[c, n, nt, nf] denotes that _nt_ is executed after
_n_ when condition c holds, and otherwise
_nf_ is executed.

Our  schemas  match  both  shape  and  the  boolean
predicates that guard execution of each node, to ensure
semantics preservation.  These conditions are implicitly
described using meta-variables in Table 3, such as c and 
!c.  The intuition is that shape alone is not enough to
distinguish  which  control  structure  should  be  used  in
decompilation. For instance, a switch for cases x = 2
and x = 3 can have the diamond shape of an if-then-else, but
we would not want to mistake a switch for an if-then-else
because the semantics of if-then-else requires the outgoing
conditions to be inverses.
#endif
        /// <summary>
        /// Attempts to match an acyclic region.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Region MatchAcyclic(Region n)
        {
            for (; ; )
            {
                if (n.Type == RegionType.Condition)
                {
                    var ss = regionGraph.Successors(n).ToArray();
                    var el = ss[0];
                    var th = ss[1];
                    var elS = SingleSuccessor(el);
                    var thS = SingleSuccessor(th);
                    if (elS == th || el.Type == RegionType.Tail)
                    {
                        // Collapse (If then) into n.
                        n.Statements.Add(new AbsynIf(n.Expression.Invert(), el.Statements));
                        regionGraph.RemoveEdge(n, el);
                        if (elS != null)
                            regionGraph.RemoveEdge(el, elS);
                        regionGraph.Nodes.Remove(el);
                        n.Type = RegionType.Linear;
                        continue;
                    }
                    if (elS != null && elS == thS)
                    {
                        // Collapse (If then else) into n.
                        n.Statements.Add(new AbsynIf(n.Expression, th.Statements, el.Statements));
                        regionGraph.RemoveEdge(n, el);
                        regionGraph.RemoveEdge(n, th);
                        regionGraph.RemoveEdge(el, elS);
                        regionGraph.RemoveEdge(th, thS);
                        regionGraph.Nodes.Remove(th);
                        regionGraph.Nodes.Remove(el);
                        regionGraph.AddEdge(n, elS);
                        n.Type = RegionType.Linear;
                        continue;
                    }
                    return n;
                }

                if (this.regionGraph.Successors(n).Count == 1)
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
                        regionGraph.Nodes.Remove(s);
                        continue;
                    }
                }
                return n;
            }
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

        [Conditional("DEBUG")]
        private void DumpGraph()
        {
            foreach (var n in regionGraph.Nodes)
            {
                Debug.Print("Node: {0}", n.Block.Name);
                Debug.Print("  Pred: {0}", string.Join(" ", regionGraph.Predecessors(n).Select(p => p.Block.Name)));
                var sb = new StringWriter();
                n.Write(sb);
                Debug.Write(sb.ToString());
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
        public void VirtualizeEdge(Region from, Region to, VirtualEdgeType type)
        {
            AbsynStatement stm;
            switch (type)
            {
            case VirtualEdgeType.Continue: stm = new AbsynContinue(); break;
            case VirtualEdgeType.Break: stm = new AbsynBreak(); break;
            case VirtualEdgeType.Goto:
                stm = new AbsynGoto(to.Block.Name);
                if (to.Statements.Count > 0 && !(to.Statements[0] is AbsynLabel))
                {
                    to.Statements.Insert(0, new AbsynLabel(to.Block.Name));
                }
                break;
            default:
                throw new InvalidOperationException();
            }
            CollapseToTailRegion(from, to, stm);
            regionGraph.RemoveEdge(from, to);

        }

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
                from.Type = RegionType.Linear;
            }
            else if (from.Type == RegionType.Linear)
            {
                from.Statements.Add(stm);
                from.Type = RegionType.Tail;
            }
            else
                throw new NotImplementedException();
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
loop exits. In Phoenix, the loop exits are represented by
a tail region,  which corresponds to a goto , break,  or
continue in the decompiled output.  These tail regions
are added during loop refinement, which we discuss next.

#endif

        public Region MatchCyclic(Region n)
        {
            var loopNodes = new LoopFinder<Region>(regionGraph, n, doms).LoopNodes;
            var succs = regionGraph.Successors(n).ToArray();
            foreach (var s in succs)
            {
                if (s == n)
                {
                    // DoWhile!
                    var loop = new AbsynDoWhile(s.Statements, s.Expression);
                    n.Statements = new List<AbsynStatement> { loop };
                    n.Type = RegionType.Linear;
                    n.Expression = null;
                    regionGraph.RemoveEdge(n, s);
                    regionGraph.RemoveEdge(s, n);
                    DumpGraph();
                    return n;
                }
            }
            foreach (var s in succs)
            {
                var ss = SingleSuccessor(s);
                if (ss != null && ss == n)
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
                            exp,
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
                    regionGraph.Nodes.Remove(s);
                    DumpGraph();
                    return n;
                }
            }

            // It's a cyclic region, but we are unable to collapse it. Schedule it for refinement after the whole graph has been traversed.
            this.unresolvedCycles.Enqueue(Tuple.Create(n, loopNodes));
            // RefineLoop(n, loopNodes);
            //this.unresolvedCycles.Add(Tuple.Create(n, loopNodes));
            return n;
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
        private void RefineLoop(Region head, ISet<Region> loopNodes)
        {
           head = EnsureSingleEntry(head, loopNodes);
           var follow = DetermineFollowRegion(head, loopNodes);
           var lexicalNodes = GetLexicalNodes(head, follow, loopNodes);
           var virtualized = VirtualizeIrregularExits(head, follow, lexicalNodes);
           if (virtualized)
               return;
           LastResort(lexicalNodes);
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

        private Region DetermineFollowRegion(Region head, ISet<Region> loopNodes)
        {
            var headSucc = regionGraph.Successors(head).ToArray();
            if (headSucc.Length == 2)
            {
                if (!loopNodes.Contains(headSucc[0]))
                    return headSucc[0];
                if (!loopNodes.Contains(headSucc[1]))
                    return headSucc[1];
            }
            throw new NotImplementedException();
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
            Region item;
            while (wl.GetWorkItem(out item))
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
            lexNodes.Remove(head);
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

        /// <summary>
        /// Virtualizes any edge leaving the lexically
        /// contained loop nodes other than the exit edge. Edges that
        /// go to the loop header use the continue tail regions, while
        /// edges that go to the loop successor use the break
        /// regions. Any other virtualized edge becomes a goto.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="follow"></param>
        /// <param name="lexicalNodes"></param>
        /// <returns>True if at least one edge was virtualized.</returns>
        private bool VirtualizeIrregularExits(Region header, Region follow, ISet<Region> lexicalNodes)
        {
            bool didVirtualize = false;
            foreach (var n in lexicalNodes)
            {
                var vEdges = new List<Tuple<Region, Region, VirtualEdgeType>>();
                foreach (var s in regionGraph.Successors(n))
                {
                    if (!lexicalNodes.Contains(s))
                    {
                        VirtualEdgeType vType;
                        if (s == header)
                            continue; // vType = VirtualEdgeType.Continue;
                        else if (s == follow)
                            vType = VirtualEdgeType.Break;
                        else
                            vType = VirtualEdgeType.Goto;

                        vEdges.Add(Tuple.Create(n, s, vType));
                    }
                }
                foreach (var edge in vEdges)
                {
                    didVirtualize = true;
                    VirtualizeEdge(edge.Item1, edge.Item2, edge.Item3);
                }
            }
            return didVirtualize;
        }
#if NILZ
3.7 Last Resort Refinement
If the algorithm does not collapse any nodes or perform any
refinement during an iteration, Phoenix removes an edge in
the graph to allow it to make progress. We call this process
the last resort refinement, because it has the lowest priority,
and always allows progress to be made. Last resort refine-
ment prefers to remove edges whose source does not domi-
nate its target, nor whose target dominates its source. These
edges can be thought of as cutting across the dominator
tree. By removing them, we leave edges that reflect more
structure because they reflect a dominator relationship
#endif
        private void LastResort(IEnumerable<Region> nodes)
        {
            foreach (var n in nodes)
            {
                foreach (var s in regionGraph.Successors(n))
                {
                    if (!doms.DominatesStrictly(n, s))
                        VirtualizeEdge(n, s, VirtualEdgeType.Goto);
                }
            }
        }
    }
}