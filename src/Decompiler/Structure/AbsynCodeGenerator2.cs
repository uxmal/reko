using Decompiler.Core;
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    public class AbsynCodeGenerator2
    {
        private Stack<StructureNode> followStack;
        private Queue<NodeEmitter> nodesToRender;
        private HashSet<StructureNode> visited;

        public AbsynCodeGenerator2()
        {
            followStack = new Stack<StructureNode>();
            nodesToRender = new Queue<NodeEmitter>();
            visited = new HashSet<StructureNode>();
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

        public void EmitGotoAndForceLabel(StructureNode node, StructureNode succ, AbsynStatementEmitter emitter)
        {
            if (node == null)
                throw new InvalidOperationException("A goto must have a starting point.");

            if (node.Loop != null)
            {
                if (node.Loop.Follow == succ)
                {
                    emitter.EmitBreak();
                    return;
                }
                if (node.Loop.Header == succ)
                {
                    emitter.EmitContinue();
                    return;
                }
            }

            succ.ForceLabel = true;
            emitter.EmitGoto(succ);
        }




        public void GenerateCode(ProcedureStructure proc, List<AbsynStatement> stms)
        {
            nodesToRender.Enqueue(new NodeEmitter(proc.EntryNode, new AbsynStatementEmitter(stms)));
            while (nodesToRender.Count > 0)
            {
                NodeEmitter ne = nodesToRender.Dequeue();
                if (IsVisited(ne.Node))
                    EmitGotoAndForceLabel(ne.Predecessor, ne.Node, ne.Emitter);
                GenerateCode(ne.Node, null, ne.Emitter);
            }
        }

        public void GenerateCode(
            StructureNode node,
            StructureNode latchNode,
            AbsynStatementEmitter emitter)
        {
            if (followStack.Contains(node) && followStack.Peek() == node)
                return;
            if (IsVisited(node))
                return;
            visited.Add(node);

            if (NeedsLabel(node))
                GenerateLabel(node,emitter);

            switch (node.GetStructType())
            {
            case structType.Seq:
                EmitLinearBlockStatements(node, emitter);
                if (EndsWithReturnInstruction(node))
                {
                    emitter.EmitStatement(node.Instructions.Last);
                    return;
                }
                if (node.IsLatchNode())
                    break;

                if (node.OutEdges.Count == 1)
                {
                    StructureNode succ = node.OutEdges[0];
                    if (IsVisited(succ))
                        EmitGotoAndForceLabel(node, succ, emitter);
                    else
                        GenerateCode(succ, latchNode, emitter);
                }
                break;
            case structType.Cond:
                node.Conditional.GenerateCode(this, node, latchNode, emitter);
                break;
            case structType.Loop:
            case structType.LoopCond:
                node.Loop.GenerateCode(this, node, latchNode, emitter);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private bool NeedsLabel(StructureNode node)
        {
            if (node.ForceLabel)
                return true;
            foreach (StructureNode pred in node.InEdges)
            {
                if (IsVisited(pred))
                    continue;
                if (node.IsAncestorOf(pred))
                    continue;
                return true;
            }
            return false;
        }

        private void GenerateLabel(StructureNode node, AbsynStatementEmitter emitter)
        {
            emitter.EmitLabel(node);
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
            private AbsynStatementEmitter emitter;
            private StructureNode node;
            private StructureNode pred;

            public NodeEmitter(StructureNode node, AbsynStatementEmitter emitter)
            {
                this.node = node;
                this.pred = null;
                this.emitter = emitter;
            }

            public NodeEmitter(StructureNode pred, StructureNode node, AbsynStatementEmitter emitter)
            {
                this.pred = pred;
                this.node = node;
                this.emitter = emitter;
            }

            public StructureNode Node
            {
                get { return node; }
            }

            public AbsynStatementEmitter Emitter
            {
                get { return emitter; }
            }

            public StructureNode Predecessor
            {
                get { return pred; } 
            }
        }

    }
}
