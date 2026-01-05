#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Operators;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.X86.Analysis
{
    /// <summary>
    /// Rewrites x86 strcpy chains. Expression coalescing should be done before.
    /// </summary>
    /// <remarks>
    /// Many x86 binaries contain strcpy(&lt;dst&gt;, &lt;src&gt;) compiled
    /// as scasb/movsd/movsb sequence:
    /// <code>
    ///     mov edi,&lt;dst&gt;
    ///     mov edx,&lt;src&gt;
    ///     or ecx,0FFh
    ///     xor eax,eax
    ///     repne scasb
    ///     not ecx
    ///     sub edi,ecx
    ///     mov esi,edi
    ///     mov eax,ecx
    ///     mov edi,edx
    ///     shr ecx,2h
    ///     rep movsd
    ///     mov ecx,eax
    ///     and ecx,3h
    ///     rep movsb
    ///     ret
    /// </code>
    /// </remarks>
    public class StrcpyChainRewriter : IAnalysis<SsaState>
    {
        private readonly AnalysisContext ctx;
        private readonly ExpressionEmitter m;
        private readonly ExpressionValueComparer cmp;

        public StrcpyChainRewriter(AnalysisContext ctx)
        {
            this.ctx = ctx;
            this.m = new();
            this.cmp = new();
        }

        public string Id => "strcpy-chain-rewriter";

        public string Description => "Rewite scasb/movsd/movsb sequence as strcpy";

        public (SsaState, bool) Transform(SsaState subject)
        {
            bool changed = false;
            foreach (var block in subject.Procedure.ControlGraph.Blocks)
            {
                var stms = block.Statements.ToArray();
                for (int i = 0; i < stms.Length - 1; i++)
                {
                    if (TryRewriteStrcpy(subject, stms[i], stms[i + 1]))
                    {
                        changed = true;
                        i++;
                    }
                }
            }
            return (subject, changed);
        }

        private bool TryRewriteStrcpy(
            SsaState ssa, Statement firstStm, Statement lastStm)
        {
            if (!AsMemcpy(firstStm, out var firstMemcpy))
                return false;
            if (!AsMemcpy(lastStm, out var lastMemcpy))
                return false;
            if (!IsAdjacent(firstMemcpy.Dst, lastMemcpy.Dst, firstMemcpy.Size))
                return false;
            if (!IsAdjacent(firstMemcpy.Src, lastMemcpy.Src, firstMemcpy.Size))
                return false;
            if (!IsStrSize(
                    ssa, firstMemcpy.Size, lastMemcpy.Size, firstMemcpy.Src))
                return false;
            firstStm.Instruction = new SideEffect(
                m.Fn(Strcpy(ssa), firstMemcpy.Dst, firstMemcpy.Src));
            ssa.RemoveUses(firstStm, firstMemcpy.Size);
            ssa.DeleteStatement(lastStm);
            return true;
        }

        private bool IsAdjacent(Expression first, Expression last, Expression size)
        {
            if (last is not BinaryExpression bin)
                return false;
            if (bin.Operator.Type != OperatorType.IAdd)
                return false;
            if (cmp.Equals(bin.Left, first) && cmp.Equals(bin.Right, size))
                return true;
            if (cmp.Equals(bin.Right, first) && cmp.Equals(bin.Left, size))
                return true;
            return false;
        }

        private bool IsStrSize(
            SsaState ssa, Expression firstSize, Expression lastSize,
            Expression str)
        {
            firstSize = GetDefiningExpression(ssa, firstSize);
            lastSize = GetDefiningExpression(ssa, lastSize);
            // if expression like (size >> log2(alignment)) * alignment
            if (!IsAlignment(firstSize, out var strSize1, out var alignment))
                return false;
            // if expression like (size & (n - 1))
            if (!IsModulo(lastSize, out var strSize2, out var n))
                return false;
            if (alignment != n)
                return false;
            if (!cmp.Equals(strSize1, strSize2))
                return false;
            var strSize = GetDefiningExpression(ssa, strSize1);
            if (!IsStrlenInc1(strSize, str))
                return false;
            return true;
        }

        private bool IsAlignment(
            Expression e,
            [MaybeNullWhen(false)] out Expression value,
            out long alignment)
        {
            value = null;
            alignment = 0;
            if (e is not BinaryExpression bin)
                return false;
            if (bin.Operator.Type != OperatorType.SMul &&
                bin.Operator.Type != OperatorType.UMul &&
                bin.Operator.Type != OperatorType.IMul)
                return false;
            if (bin.Right is not Constant cRight)
                return false;
            if (bin.Left is not BinaryExpression leftBin)
                return false;
            if (leftBin.Operator.Type != OperatorType.Shr)
                return false;
            if (leftBin.Right is not Constant cShr)
                return false;
            if ((1 << cShr.ToInt32()) != cRight.ToInt64())
                return false;
            value = leftBin.Left;
            alignment = cRight.ToInt64();
            return true;
        }

        private bool IsModulo(
            Expression e,
            [MaybeNullWhen(false)] out Expression value,
            out long n)
        {
            value = null;
            n = 0;
            if (e is not BinaryExpression bin)
                return false;
            if (bin.Operator.Type != OperatorType.And)
                return false;
            if (bin.Right is not Constant cAnd)
                return false;
            value = bin.Left;
            n = cAnd.ToInt64() + 1;
            return true;
        }

        private bool IsStrlenInc1(Expression e, Expression str)
        {
            if (e is not BinaryExpression bin)
                return false;
            if (bin.Operator.Type != OperatorType.IAdd)
                return false;
            if (!IsCallToIntrinsicProcedure(
                bin.Left, CommonOps.Strlen, out var app)
            )
                return false;
            if (!cmp.Equals(str, app.Arguments[0]))
                return false;
            if (bin.Right is not Constant c || !c.IsIntegerOne)
                return false;
            return true;
        }

        private Expression GetDefiningExpression(SsaState ssa, Expression e)
        {
            if (e is not Identifier id)
                return e;
            var defStm = ssa.Identifiers[id].DefStatement;
            if (defStm is null)
                return e;
            if (defStm.Instruction is not Assignment ass)
                return e;
            return ass.Src;
        }

        private bool AsMemcpy(
            Statement stm,
            [MaybeNullWhen(false)] out MemcpyArguments memcpy)
        {
            memcpy = null;
            if (stm.Instruction is not SideEffect side)
                return false;
            if (!IsCallToIntrinsicProcedure(
                side.Expression, CommonOps.Memcpy, out var app)
            )
                return false;
            memcpy = new(app.Arguments);
            return true;
        }

        private bool IsCallToIntrinsicProcedure(
            Expression e,
            IntrinsicProcedure intrinsic,
            [MaybeNullWhen(false)] out Application app)
        {
            app = null;
            if (e is not Application foundApp)
                return false;
            if (foundApp.Procedure is not ProcedureConstant pc)
                return false;
            if (pc.Procedure is not IntrinsicProcedure foundIntrinsic)
                return false;
            if (foundIntrinsic.Name != intrinsic.Name)
                return false;
            app = foundApp;
            return true;
        }

        private IntrinsicProcedure Strcpy(SsaState ssa)
        {
            return CommonOps.Strcpy.ResolvePointers(
                ssa.Procedure.Architecture.PointerType.BitSize);
        }

        private class MemcpyArguments
        {
            public readonly Expression Dst;
            public readonly Expression Src;
            public readonly Expression Size;

            public MemcpyArguments(Expression[] args)
            {
                (Dst, Src, Size) = (args[0], args[1], args[2]);
            }
        }
    }
}
