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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Structure
{
    /// <summary>
    /// Generates Abstract syntax trees of high-level statements from the given StructureNodes. 
    /// </summary>
    [Obsolete("", true)]
    public class AbsynCodeGenerator
    {
        private Stack<StructureNode> followStack;
        private Queue<NodeEmitter> nodesToRender;
        private HashSet<StructureNode> visited;
        private HashSet<StructureNode> incomplete;

        public AbsynCodeGenerator()
        {
            followStack = new Stack<StructureNode>();
            nodesToRender = new Queue<NodeEmitter>();
            visited = new HashSet<StructureNode>();
            incomplete = new HashSet<StructureNode>();
        }

        public void DeferRendering(StructureNode predecessor, StructureNode node, AbsynStatementEmitter emitter)
        {
            foreach (NodeEmitter ne in nodesToRender)
            {
                if (ne.Node == node)
                    return;
            }
            nodesToRender.Enqueue(new NodeEmitter(predecessor, node, emitter));
        }


        private bool ShouldJumpFromSequentialNode(StructureNode node, StructureNode succ)
        {
            if (IsVisited(succ))
            {
                return (followStack.Count == 0 || followStack.Peek() != succ);
            }
            return false;
        }

        private bool NeedsLabel(StructureNode node)
        {
            if (node.ForceLabel)
                return true;

            if (node.InEdges.Count == 1)
                return false;

            foreach (StructureNode pred in node.InEdges)
            {
                if (IsVisited(pred) && !IncompleteNodes.Contains(pred))
                    continue;
                if (node.IsLoopHeader() && node.IsAncestorOf(pred))
                    continue;
                return true;
            }
            return false;
        }

        public void PushFollow(StructureNode followNode)
        {
            followStack.Push(followNode);
        }

        public StructureNode PopFollow()
        {
            return followStack.Pop();
        }

        private bool AllPredecessorsVisited(StructureNode node)
        {
            foreach (StructureNode pred in node.InEdges)
                if (!IsVisited(pred))
                    return false;
            return true;
        }

        public Expression BranchCondition(StructureNode node)
        {
            if (node.Instructions.Count == 0)
                throw new InvalidOperationException(string.Format("Node {0} must have at least one instruction.", node.Name));
            Branch branch = (Branch) node.Instructions.Last.Instruction;
            return branch.Condition;
        }

        private bool EndsWithReturnInstruction(StructureNode node)
        {
            if (node.Instructions.Count == 0)
                return false;
            return node.Instructions.Last.Instruction is ReturnInstruction;
        }

        public void EmitLinearBlockStatements(StructureNode node, AbsynStatementEmitter emitter)
        {
            foreach (Statement stm in node.Instructions)
            {
                if (stm.Instruction.IsControlFlow)
                    return;
                emitter.EmitStatement(stm);
            }
        }

        public HashSet<StructureNode> IncompleteNodes
        {
            get { return incomplete; }
        }

        public bool IsVisited(StructureNode succ)
        {
            return visited.Contains(succ);
        }

        public void IsVisited(StructureNode succ, bool flag)
        {
            if (flag)
                visited.Add(succ);
            else
                visited.Remove(succ);
        }

        private class NodeEmitter
        {
            public NodeEmitter(StructureNode node, AbsynStatementEmitter emitter)
            {
                this.Node = node;
                this.Predecessor = null;
                this.Emitter = emitter;
            }

            public NodeEmitter(StructureNode pred, StructureNode node, AbsynStatementEmitter emitter)
            {
                this.Predecessor = pred;
                this.Node = node;
                this.Emitter = emitter;
            }

            public StructureNode Node { get; private set; }

            public AbsynStatementEmitter Emitter { get; private set; }

            public StructureNode Predecessor { get; private set; }
        }

    }
}
