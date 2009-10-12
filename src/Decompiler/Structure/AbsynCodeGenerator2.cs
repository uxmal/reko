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


        public void GenerateCode(ProcedureStructure proc, List<AbsynStatement> stms)
        {
            nodesToRender.Enqueue(new NodeEmitter(proc.EntryNode, new AbsynStatementEmitter(stms)));
            while (nodesToRender.Count > 0)
            {
                NodeEmitter ne = nodesToRender.Dequeue();
                GenerateCode(ne.Node, null, ne.Emitter);
            }
        }

        public void GenerateCode(
            StructureNode node,
            StructureNode latchNode,
            AbsynStatementEmitter emitter)
        {
            if (followStack.Contains(node))
                return;

            switch (node.GetStructType())
            {
            case structType.Seq:
                EmitLinearBlockStatements(node, emitter);
                if (EndsWithReturnInstruction(node))
                {
                    emitter.EmitStatement(node.Instructions.Last);
                    return;
                }
                if (node.OutEdges.Count == 1)
                {
                    GenerateCode(node.OutEdges[0], latchNode, emitter);
                }
                break;
            case structType.Cond:
                EmitLinearBlockStatements(node, emitter);
                if (node.CondFollow == null)
                    throw new NotSupportedException("Null condfollow");
                this.followStack.Push(node.CondFollow);
                node.Conditional.GenerateCode(this, node, latchNode, emitter);
                this.followStack.Pop();
                GenerateCode(node.CondFollow, latchNode, emitter);
                break;
            default:
                throw new NotImplementedException();
            }
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

        private void EmitLinearBlockStatements(StructureNode node, AbsynStatementEmitter emitter)
        {
            foreach (Statement stm in node.Instructions)
            {
                if (stm.Instruction.IsControlFlow)
                    return;
                emitter.EmitStatement(stm);
            }
        }


        private class NodeEmitter
        {
            private AbsynStatementEmitter emitter;
            private StructureNode node;

            public NodeEmitter(StructureNode node, AbsynStatementEmitter emitter)
            {
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
        }

        internal void EmitGotoAndLabel(StructureNode node, StructureNode succ, AbsynStatementEmitter emitSwitchBranches)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
