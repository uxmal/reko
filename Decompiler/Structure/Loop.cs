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

using Decompiler.Core;
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    /// <summary>
    /// A loop is described by its header, its latch node, its members, and a follow node.
    /// </summary>
    public abstract class Loop
    {
        public Loop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes)
        {
            this.Header = header;
            this.Latch = latch;
            this.Nodes = loopNodes;
        }

        public Loop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow)
        {
            this.Header = header;
            this.Latch = latch;
            this.Nodes = loopNodes;
            this.Follow = follow;
        }

        public StructureNode Header { get; private set; }

        public StructureNode Latch { get;private set; }

        public StructureNode Follow { get; private set; }

        public HashSet<StructureNode> Nodes { get; private set; }

        public void GenerateCode(AbsynCodeGenerator codeGen, StructureNode node, StructureNode latchNode, AbsynStatementEmitter emitter)
        {
            if (Follow != null)
                codeGen.PushFollow(Follow);

            GenerateCodeInner(codeGen, node, emitter);
            if (Follow != null)
            {
                codeGen.PopFollow();
                codeGen.GenerateCode(Follow, latchNode, emitter);
            }
        }

        protected abstract void GenerateCodeInner(AbsynCodeGenerator codeGen, StructureNode node, AbsynStatementEmitter emitter);

        public void TagNodes(HashSet<StructureNode> loopNodes)
        {
            foreach (StructureNode node in loopNodes)
            {
                if (node.Loop == null)
                {
                    TagLoopNode(node);
                }
            }
        }

        private void TagLoopNode(StructureNode node)
        {
            node.Loop = this;
            if (node.Conditional == null)
                return;
            if (node.Conditional.Follow != null)
                return;
        }
    }

    /// <summary>
    /// Pre-tested loops correspond to 'while' loops.
    /// </summary>
    public class PreTestedLoop : Loop
    {
        public PreTestedLoop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow)
            : base(header, latch, loopNodes, follow)
        {
        }

        protected override void GenerateCodeInner(AbsynCodeGenerator codeGen, StructureNode node, AbsynStatementEmitter emitter)
        {
            codeGen.EmitLinearBlockStatements(node, emitter);
            var loopBody = new List<AbsynStatement>();
            var bodyNode = (node.Else == node.Loop.Follow)
                ? node.Then
                : node.Else;
            var bodyEmitter = new AbsynStatementEmitter(loopBody);
            codeGen.GenerateCode(bodyNode, node.Loop.Latch, bodyEmitter);
            bodyEmitter.StripDeclarations = true;
            codeGen.EmitLinearBlockStatements(node, bodyEmitter);

            emitter.EmitWhile(node, codeGen.BranchCondition(node), loopBody);
        }
    }

    /// <summary>
    /// Post-tested loops correspond to do/while or repeat/until loops.
    /// </summary>
    public class PostTestedLoop : Loop
    {
        public PostTestedLoop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow)
            : base(header, latch, loopNodes, follow)
        {
        }

        protected override void GenerateCodeInner(AbsynCodeGenerator codeGen, StructureNode node, AbsynStatementEmitter emitter)
        {
            List<AbsynStatement> loopBody = new List<AbsynStatement>();
            AbsynStatementEmitter bodyEmitter = new AbsynStatementEmitter(loopBody);
            if (node.IsLatchNode())
            {
                codeGen.EmitLinearBlockStatements(node, bodyEmitter);
            }
            else
            {
                if (node.Conditional != null)
                {
                    node.Conditional.GenerateCode(codeGen, node, Latch, bodyEmitter);
                }
                else
                {
                    codeGen.EmitLinearBlockStatements(node, bodyEmitter);
                    if (node.OutEdges.Count != 1)
                        throw new NotSupportedException(string.Format("Expected top of PostTestedLoop {0} to have only 1 out edge, but found {1} out edges.", node.Name, node.OutEdges.Count));
                    codeGen.GenerateCode(node.OutEdges[0], Latch, bodyEmitter);
                }
            }
            emitter.EmitDoWhile(loopBody, codeGen.BranchCondition(Latch));
        }
    }

    /// <summary>
    /// Testless loops don't have an exit in their header nor their latch node.
    /// Either exits need to be modelled with a break/goto/return (in C) or the loop is infinite.
    /// </summary>
    public class TestlessLoop : Loop
    {
        public TestlessLoop(StructureNode header, StructureNode latch, HashSet<StructureNode> loopNodes, StructureNode follow)
            : base(header, latch, loopNodes, follow)
        {
        }

        protected override void GenerateCodeInner(AbsynCodeGenerator codeGen, StructureNode node, AbsynStatementEmitter emitter)
        {
            List<AbsynStatement> loopBody = new List<AbsynStatement>();
            AbsynStatementEmitter bodyEmitter = new AbsynStatementEmitter(loopBody);
            if (node.Conditional != null)
            {
                node.Conditional.GenerateCode(codeGen, node, Latch, bodyEmitter);
            }
            else
            {
                codeGen.EmitLinearBlockStatements(node, bodyEmitter);
                if (node.OutEdges.Count != 1)
                    throw new NotSupportedException(string.Format("Expected top of PostTestedLoop {0} to have only 1 out edge, but found {1} out edges.", node.Name, node.OutEdges.Count));
                codeGen.GenerateCode(node.OutEdges[0], Latch, bodyEmitter);
            }
            emitter.EmitForever(node, loopBody);
        }
    }
}