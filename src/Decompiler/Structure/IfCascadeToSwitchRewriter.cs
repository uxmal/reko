#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Reko.Structure.IfCascadeToSwitchRewriter;

namespace Reko.Structure
{
    public class IfCascadeToSwitchRewriter : IAbsynVisitor<IfCascadeToSwitchRewriter.Cascade>
    {
        private readonly List<AbsynStatement> body;
        private readonly ExpressionValueComparer cmp;

        public IfCascadeToSwitchRewriter(Procedure proc)
        {
            this.body = proc.Body!;
            this.cmp = new ExpressionValueComparer();
        }

        public class Cascade
        {
            public AbsynStatement stm;
            public List<CascadeNode>? nodes;

            public Cascade(AbsynStatement stm)
            {
                this.stm = stm;
            }

            public Cascade(AbsynStatement stm, List<CascadeNode>? nodes) : this(stm)
            {
                this.nodes = nodes;
            }
        }

        public void Transform()
        {
            Transform(body);
        }

        private void Transform(List<AbsynStatement> body)
        {
            for (int i = 0; i < body.Count; ++i)
            {
                body[i] = body[i].Accept(this).stm;
            }
        }

        public class CascadeNode
        {
            public Expression e;
            public List<Constant> constants; // An empty list denotes the the default case.
            public List<AbsynStatement> caseStatements;
            public List<AbsynStatement> nextStatements;

            public CascadeNode(Expression e, List<Constant> c, List<AbsynStatement> cs, List<AbsynStatement> ns)
            {
                this.e = e;
                this.constants = c;
                this.caseStatements = cs;
                this.nextStatements = ns;
            }

            public CascadeNode(Expression e, Constant c, List<AbsynStatement> cs, List<AbsynStatement> ns)
            {
                this.e = e;
                this.constants = new List<Constant> { c };
                this.caseStatements = cs;
                this.nextStatements = ns;
            }

            public CascadeNode(Expression e, List<AbsynStatement> cs)
                : this(e, new List<Constant>(), cs, new())
            {
            }
        }

        private List<CascadeNode>? IdentifyCascade(AbsynIf start)
        {
            var node = IdentifyCascadeNode(start);
            if (node is null)
                return null;
            var cascadeNodes = new List<CascadeNode> { node };
            var e = node.e; 
            while (node.nextStatements.Count == 1)
            {
                if (node.nextStatements[0] is not AbsynIf nextIf)
                    break;
                var nextNode = IdentifyCascadeNode(nextIf);
                if (nextNode is null || !cmp.Equals(e, nextNode.e))
                    break;
                cascadeNodes.Add(nextNode);
                node = nextNode;
            }
            if (cascadeNodes.Count < 2)
                return null;
            if (node!.nextStatements.Count > 0)
            {
                cascadeNodes.Add(new CascadeNode(e, node!.nextStatements));
            }
            return cascadeNodes;
        }

        private CascadeNode? IdentifyCascadeNode(AbsynIf absynIf)
        {
            if (absynIf.Condition is BinaryExpression bin)
            {
                if (bin.Right is Constant c)
                {
                    if (bin.Operator.Type == OperatorType.Eq)
                    {
                        return new(bin.Left, c, absynIf.Then, absynIf.Else);
                    }
                    else if (bin.Operator.Type == OperatorType.Ne)
                    {
                        return new(bin.Left, c, absynIf.Else, absynIf.Then);
                    }
                }
                else if (bin.Operator.Type == OperatorType.Cor)
                {
                    List<Constant> consts = new();
                    var (ee, cc) = EqualityCheck(bin.Left, OperatorType.Eq);
                    if (ee is null)
                        return null;
                    Expression e1 = ee;
                    consts.Add(cc!);
                    var er = bin.Right;
                    while (er is BinaryExpression b && 
                        b.Operator.Type == OperatorType.Cor)
                    {
                        (ee, cc) = EqualityCheck(b.Left, OperatorType.Eq);
                        if (ee is null || !cmp.Equals(e1, ee))
                            return null;
                        consts.Add(cc!);
                        er = b.Right;
                    }
                    (ee, cc) = EqualityCheck(er, OperatorType.Eq);
                    if (ee is null || !cmp.Equals(e1, ee))
                        return null;
                    consts.Add(cc!);
                    return new(e1, consts, absynIf.Then, absynIf.Else);
                }
                else if (bin.Operator.Type == OperatorType.Cand)
                {
                    List<Constant> consts = new();
                    var (ee, cc) = EqualityCheck(bin.Right, OperatorType.Ne);
                    if (ee is null)
                        return null;
                    Expression e1 = ee;
                    consts.Add(cc!);
                    var er = bin.Left;
                    while (er is BinaryExpression b &&
                        b.Operator.Type == OperatorType.Cand)
                    {
                        (ee, cc) = EqualityCheck(b.Right, OperatorType.Ne);
                        if (ee is null || !cmp.Equals(e1, ee))
                            return null;
                        consts.Add(cc!);
                        er = b.Left;
                    }
                    (ee, cc) = EqualityCheck(er, OperatorType.Ne);
                    if (ee is null || !cmp.Equals(e1, ee))
                        return null;
                    consts.Add(cc!);
                    consts.Reverse();

                    return new(e1, consts, absynIf.Else, absynIf.Then);
                }
            }

            return null;
        }

        private (Expression?, Constant?) EqualityCheck(Expression e, OperatorType op)
        {
            if (e is BinaryExpression bin &&
                bin.Operator.Type == op)
            {
                var l = bin.Left;
                if (bin.Right is Constant c)
                {
                    return (l, c);
                }
            }
            return (null, null);
        }

        private AbsynSwitch ReplaceCascadeWithSwitch(AbsynIf absynIf, List<CascadeNode> cascade)
        {
            var cases = cascade.SelectMany(GenerateCase).ToList();
            var sw = new AbsynSwitch(cascade[0].e, cases);
            return sw;
        }

        private List<AbsynStatement> GenerateCase(CascadeNode arg)
        {
            var items = new List<AbsynStatement>();
            if (arg.constants.Count == 0)
            {
                items.Add(new AbsynDefault());
            }
            else
            {
                foreach (var c in arg.constants)
                {
                    items.Add(new AbsynCase(c));
                }
            }
            items.AddRange(arg.caseStatements);
            switch (items[^1])
            {
            case AbsynCase:
            case AbsynBreak:
            case AbsynContinue:
            case AbsynDefault:
            case AbsynGoto:
            case AbsynLabel:
            case AbsynReturn:
                break;
            default:
                items.Add(new AbsynBreak());
                break;
            }
            return items;
        }

        public Cascade VisitAssignment(AbsynAssignment ass)
        {
            return new(ass);
        }

        public Cascade VisitBreak(AbsynBreak brk)
        {
            return new(brk);
        }

        public Cascade VisitCase(AbsynCase absynCase)
        {
            return new(absynCase);
        }

        public Cascade VisitCompoundAssignment(AbsynCompoundAssignment compound)
        {
            return new(compound);
        }

        public Cascade VisitContinue(AbsynContinue cont)
        {
            return new(cont);
        }

        public Cascade VisitDeclaration(AbsynDeclaration decl)
        {
            return new(decl);
        }

        public Cascade VisitDefault(AbsynDefault decl)
        {
            return new(decl);
        }

        public Cascade VisitDoWhile(AbsynDoWhile loop)
        {
            Transform(loop.Body);
            return new(loop);
        }

        public Cascade VisitFor(AbsynFor forLoop)
        {
            Transform(forLoop.Body);
            return new(forLoop);
        }

        public Cascade VisitGoto(AbsynGoto gotoStm)
        {
            return new(gotoStm);
        }

        public Cascade VisitIf(AbsynIf absynIf)
        {
            var cascade = IdentifyCascade(absynIf);
            if (cascade is null)
            {
                return new(absynIf);
            }
            else
                return new(ReplaceCascadeWithSwitch(absynIf, cascade));
        }

        public Cascade VisitLabel(AbsynLabel lbl)
        {
            return new(lbl);
        }

        public Cascade VisitLineComment(AbsynLineComment comment)
        {
            return new(comment);
        }

        public Cascade VisitReturn(AbsynReturn ret)
        {
            return new(ret);
        }

        public Cascade VisitSideEffect(AbsynSideEffect side)
        {
            return new(side);
        }

        public Cascade VisitSwitch(AbsynSwitch absynSwitch)
        {
            Transform(absynSwitch.Statements);
            return new(absynSwitch);
        }

        public Cascade VisitWhile(AbsynWhile loop)
        {
            Transform(loop.Body);
            return new(loop);
        }
    }
}
