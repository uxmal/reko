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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    /// <summary>
    /// An algorithm derived from the sparse constant propagation algorithm.
    /// </summary>
    public class SparseValuePropagation
    {
        private readonly SsaState ssa;
        private readonly DecompilerEventListener listener;
        private readonly ExpressionValueComparer cmp;
        private readonly SparseEvaluationContext ctx;
        private readonly Evaluation.ExpressionSimplifier eval;

        public SparseValuePropagation(SsaState ssa, Program program, IDynamicLinker resolver, DecompilerEventListener listener)
        {
            this.ssa = ssa;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.ctx = new SparseEvaluationContext(ssa.Procedure.Architecture);
            this.eval = new Evaluation.ExpressionSimplifier(program.SegmentMap, ctx, listener);
        }

        public void Transform()
        {
            SetInitialValues();
            var wl = new WorkList<SsaIdentifier>();
            wl.AddRange(ssa.Identifiers.Where(sid => ctx.GetValue(sid.Identifier) != Constant.Unknown));
            while (wl.GetWorkItem(out var sid) && !listener.IsCanceled())
            {
                var oldValue = ctx.GetValue(sid.Identifier);
                if (oldValue == Constant.Invalid)
                    continue;
        
                var newValue = Evaluate(sid);
                if (!cmp.Equals(oldValue, newValue))
                {
                    ctx.SetValue(sid.Identifier, newValue);
                    foreach (var use in sid.Uses)
                    {
                        var uc = new InstructionUseCollector();
                        var uses = uc.CollectUses(use);
                        wl.AddRange(uses.Keys.Select(id => ssa.Identifiers[id]));
                    }
                }
            }
        }

        private void SetInitialValues()
        {
            foreach (var sid in ssa.Identifiers)
            {
                ctx.SetValue(sid.Identifier, Evaluate(sid));
            }
        }

        private Expression Evaluate(SsaIdentifier sid)
        {
            Expression e;
            switch (sid.DefStatement.Instruction)
            {
            case Assignment ass:
                e = ass.Src.Accept(eval);
                return e;
            case PhiAssignment phi:
                e = Constant.Unknown;
                foreach (var phiArg in phi.Src.Arguments)
                {
                    throw new NotImplementedException();
                }
                return Constant.Invalid;
            default:
                return Constant.Invalid;
            }
        }

        public void Write(TextWriter writer)
        {
            ctx.Write(writer);
        }

        private class SparseEvaluationContext : EvaluationContext
        {
            private readonly IProcessorArchitecture arch;
            private readonly Dictionary<Identifier, Expression> values;

            public SparseEvaluationContext(IProcessorArchitecture arch)
            {
                this.arch = arch;
                this.values = new Dictionary<Identifier, Expression>();
            }

            public Expression GetDefiningExpression(Identifier id)
            {
                return values[id];
            }

            public List<Statement> GetDefiningStatementClosure(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(Identifier id)
            {
                return values[id];
            }

            public Expression GetValue(MemoryAccess access, SegmentMap segmentMap)
            {
                return Constant.Invalid;
            }

            public Expression GetValue(SegmentedAccess access, SegmentMap segmentMap)
            {
                return Constant.Invalid;
            }

            public Expression GetValue(Application appl)
            {
                return Constant.Invalid;
            }

            public bool IsUsedInPhi(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression MakeSegmentedAddress(Constant seg, Constant off)
            {
                return arch.MakeSegmentedAddress(seg, off);
            }

            public void RemoveExpressionUse(Expression expr)
            {
                throw new NotImplementedException();
            }

            public void RemoveIdentifierUse(Identifier id)
            {
            }

            public void SetValue(Identifier id, Expression value)
            {
                values[id] = value;
            }

            public void SetValueEa(Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void SetValueEa(Expression basePointer, Expression ea, Expression value)
            {
                throw new NotImplementedException();
            }

            public void UseExpression(Expression expr)
            {
                throw new NotImplementedException();
            }

            public void Write(TextWriter w)
            {
                foreach (var de in values.OrderBy(k => k.Key.Name))
                {
                    w.WriteLine("{0}: {1}", de.Key, de.Value);
                }
            }
        }
    }
}
