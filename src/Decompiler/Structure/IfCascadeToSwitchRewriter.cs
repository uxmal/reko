#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Operators;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Structure
{
    /// <summary>
    /// Transforms a cascade of <see cref="AbsynIf"/> statements into a single
    /// <see cref="AbsynSwitch"/> statement.
    /// </summary>
    public class IfCascadeToSwitchRewriter : IAbsynVisitor<IfCascadeToSwitchRewriter.Cascade>
    {
        private readonly List<AbsynStatement> body;
        private readonly ExpressionValueComparer cmp;

        /// <summary>
        /// Constructs an instance of the <see cref="IfCascadeToSwitchRewriter"/> class.
        /// </summary>
        /// <param name="proc">Procedure being rewritten.</param>
        public IfCascadeToSwitchRewriter(Procedure proc)
        {
            this.body = proc.Body!;
            this.cmp = new ExpressionValueComparer();
        }

        /// <summary>
        /// Models a cascade of <see cref="AbsynIf"/> statements.
        /// </summary>
        public class Cascade
        {
            /// <summary>
            /// Initial statement.
            /// </summary>
            public AbsynStatement stm;

            /// <summary>
            /// Cascade nodes.
            /// </summary>
            public List<CascadeNode>? nodes;

            /// <summary>
            /// Constructs a new instance of the <see cref="Cascade"/> class.
            /// </summary>
            /// <param name="stm">Initial statement.</param>
            public Cascade(AbsynStatement stm)
            {
                this.stm = stm;
            }

            /// <summary>
            /// Constructs a new instance of the <see cref="Cascade"/> class.
            /// </summary>
            /// <param name="stm">Initial statement.</param>
            /// <param name="nodes">List of cascade nodes.</param>
            public Cascade(AbsynStatement stm, List<CascadeNode>? nodes) : this(stm)
            {
                this.nodes = nodes;
            }
        }

        /// <summary>
        /// Transforms the procedure body by replacing cascades of if statements
        /// with a switch statement.
        /// </summary>
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

        /// <summary>
        /// Represends a node in a cascade of <see cref="AbsynIf"/> statements.
        /// </summary>
        public class CascadeNode
        {
            /// <summary>
            /// Predicate expression of the cascade node.
            /// </summary>
            public Expression e;
            /// <summary>
            /// Represents a collection of constants.
            /// </summary>
            /// <remarks>An empty list denotes the default case.</remarks>
            public List<Constant> constants;

            /// <summary>
            /// Collected case statements for this cascade node.
            /// </summary>
            public List<AbsynStatement> caseStatements;

            /// <summary>
            /// Collected next statements for this cascade node.
            /// </summary>
            public List<AbsynStatement> nextStatements;

            /// <summary>
            /// Constructs a new instance of the <see cref="CascadeNode"/> class.
            /// </summary>
            /// <param name="e">Predicate expression.</param>
            /// <param name="c">List of constants.</param>
            /// <param name="cs">List of cases.</param>
            /// <param name="ns">List of successor statements.</param>
            public CascadeNode(Expression e, List<Constant> c, List<AbsynStatement> cs, List<AbsynStatement> ns)
            {
                this.e = e;
                this.constants = c;
                this.caseStatements = cs;
                this.nextStatements = ns;
            }

            /// <summary>
            /// Constructs a new instance of the <see cref="CascadeNode"/> class.
            /// </summary>
            /// <param name="e">Predicate expression.</param>
            /// <param name="c">Single constant value.</param>
            /// <param name="cs">List of cases.</param>
            /// <param name="ns">List of successor statements.</param>
            public CascadeNode(Expression e, Constant c, List<AbsynStatement> cs, List<AbsynStatement> ns)
            {
                this.e = e;
                this.constants = [c];
                this.caseStatements = cs;
                this.nextStatements = ns;
            }

            /// <summary>
            /// Constructs a new instance of the <see cref="CascadeNode"/> class.
            /// </summary>
            /// <param name="e">Predicate expression.</param>
            /// <param name="cs">Case statements.
            /// </param>
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

        /// <inheritdoc/>
        public Cascade VisitAssignment(AbsynAssignment ass)
        {
            return new(ass);
        }

        /// <inheritdoc/>
        public Cascade VisitBreak(AbsynBreak brk)
        {
            return new(brk);
        }

        /// <inheritdoc/>
        public Cascade VisitCase(AbsynCase absynCase)
        {
            return new(absynCase);
        }

        /// <inheritdoc/>
        public Cascade VisitCompoundAssignment(AbsynCompoundAssignment compound)
        {
            return new(compound);
        }

        /// <inheritdoc/>
        public Cascade VisitContinue(AbsynContinue cont)
        {
            return new(cont);
        }

        /// <inheritdoc/>
        public Cascade VisitDeclaration(AbsynDeclaration decl)
        {
            return new(decl);
        }

        /// <inheritdoc/>
        public Cascade VisitDefault(AbsynDefault decl)
        {
            return new(decl);
        }

        /// <inheritdoc/>
        public Cascade VisitDoWhile(AbsynDoWhile loop)
        {
            Transform(loop.Body);
            return new(loop);
        }

        /// <inheritdoc/>
        public Cascade VisitFor(AbsynFor forLoop)
        {
            Transform(forLoop.Body);
            return new(forLoop);
        }

        /// <inheritdoc/>
        public Cascade VisitGoto(AbsynGoto gotoStm)
        {
            return new(gotoStm);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Cascade VisitLabel(AbsynLabel lbl)
        {
            return new(lbl);
        }

        /// <inheritdoc/>
        public Cascade VisitLineComment(AbsynLineComment comment)
        {
            return new(comment);
        }

        /// <inheritdoc/>
        public Cascade VisitReturn(AbsynReturn ret)
        {
            return new(ret);
        }

        /// <inheritdoc/>
        public Cascade VisitSideEffect(AbsynSideEffect side)
        {
            return new(side);
        }

        /// <inheritdoc/>
        public Cascade VisitSwitch(AbsynSwitch absynSwitch)
        {
            Transform(absynSwitch.Statements);
            return new(absynSwitch);
        }

        /// <inheritdoc/>
        public Cascade VisitWhile(AbsynWhile loop)
        {
            Transform(loop.Body);
            return new(loop);
        }
    }
}
