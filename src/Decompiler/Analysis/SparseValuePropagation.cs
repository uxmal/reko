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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Collections;
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
    /// <remarks>
    /// The algorithm works as follows:
    /// Evaluate expressions involving constants only and assign
    ///   the value(c) to variable on LHS
    ///  If an expression can not be evaluated at compile time, assign
    ///     it the BOTTOM value.
    ///  Else(for expression contains variables)
    ///     assign TOP
    ///  Initialize worklist WL with SSA edges whose def is not TOP
    /// 
    /// Iterate until WL is empty:
    ///     Take an SSA edge E out of WL
    ///     Take meet of the value at def end and the use end of E for
    ///        the variable defined at def end
    ///     If the meet value is different from use value, replace the
    ///        use by the meet
    ///     Take special care of phi functions.
    ///     Recompute the def d at the use end of E
    ///     If the recomputed value is lower than the stored value, add
    ///         all SSA edges originating at d to the WL
    ///         
    /// This is a linear algorithm for most sane graphs.
    /// </remarks>
    public class SparseValuePropagation
    {
        private readonly SsaState ssa;
        private readonly DecompilerEventListener listener;
        private readonly ExpressionValueComparer cmp;
        private readonly SparseEvaluationContext ctx;
        private readonly Evaluation.ExpressionSimplifier eval;

        public SparseValuePropagation(
            SsaState ssa,
            Program program,
            DecompilerEventListener listener)
        {
            this.ssa = ssa;
            this.listener = listener;
            this.cmp = new ExpressionValueComparer();
            this.ctx = new SparseEvaluationContext(ssa);
            this.eval = new Evaluation.ExpressionSimplifier(program.SegmentMap, ctx, listener);
        }

        public void Transform()
        {
            SetInitialValues();
            var wl = new WorkList<SsaIdentifier>();
            wl.AddRange(ssa.Identifiers);
            while (wl.TryGetWorkItem(out var sid) && !listener.IsCanceled())
            {
                var oldValue = ctx.GetValue(sid.Identifier);
                if (oldValue is InvalidConstant)
                    continue;
        
                var newValue = Evaluate(sid);
                if (!cmp.Equals(oldValue, newValue))
                {
                    ctx.SetValue(sid.Identifier, newValue!);
                    foreach (var use in sid.Uses)
                    {
                        var uc = new InstructionUseCollector();
                        var uses = uc.CollectUses(use);
                        wl.AddRange(uses.Keys.Select(id => ssa.Identifiers[id]));
                    }
                }
            }
            PerformReplacements();
        }

        private void SetInitialValues()
        {
            foreach (var sid in ssa.Identifiers)
            {
                var value = EvaluateInitial(sid);
                if (value is { })
                {
                    ctx.SetValue(sid.Identifier, value);
                }
            }
        }

        private Expression? EvaluateInitial(SsaIdentifier sid)
        {
            if (sid.DefStatement.Instruction is PhiAssignment)
                return null;
            return Evaluate(sid);
        }

        private Expression? Evaluate(SsaIdentifier sid)
        {
            Expression? e;
            switch (sid.DefStatement.Instruction)
            {
            case Assignment ass:
                (e, _) = ass.Src.Accept(eval);
                return e;
            case PhiAssignment phi:
                e = null;
                foreach (var phiArg in phi.Src.Arguments)
                {
                    var p = ctx.GetValue((Identifier)phiArg.Value);
                    e = Meet(e, p);
                    if (e is InvalidConstant)
                        return e;
                }
                return e;
            default:
                return InvalidConstant.Create(sid.Identifier.DataType);
            }
        }

        private void PerformReplacements()
        {
            var ssam = new SsaMutator(ssa);
            foreach (var sid in ssa.Identifiers)
            {
                var value = ctx.GetValue(sid.Identifier);
                if (value is null || value is InvalidConstant)
                    continue;
                switch (sid.DefStatement.Instruction)
                {
                case AliasAssignment:
                    ssam.ReplaceAssigment(sid, new AliasAssignment(sid.Identifier, value));
                    break;
                case Assignment:
                    ssam.ReplaceAssigment(sid, new Assignment(sid.Identifier, value));
                    break;
                case PhiAssignment:
                    ssam.ReplaceAssigment(sid, new Assignment(sid.Identifier, value));
                    break;
                }
            }
        }

        private Expression? Meet(Expression? e, Expression p)
        {
            if (e is null)
                return p;
            if (e is InvalidConstant)
                return e;
            if (p is null)
                return e;
            if (p is InvalidConstant)
                return p;
            if (!cmp.Equals(e, p))
            {
                return InvalidConstant.Create(e.DataType);
            }
            return p;
        }

        public void Write(TextWriter writer)
        {
            ctx.Write(writer);
        }

        private class SparseEvaluationContext : EvaluationContext
        {
            private readonly SsaState ssa;
            private readonly IProcessorArchitecture arch;
            private readonly Dictionary<Identifier, Expression> values;

            public SparseEvaluationContext(SsaState ssa)
            {
                this.ssa = ssa;
                this.arch = ssa.Procedure.Architecture;
                this.values = new Dictionary<Identifier, Expression>();
            }

            public EndianServices Endianness => arch.Endianness;

            public int MemoryGranularity => arch.MemoryGranularity;

            public Expression? GetDefiningExpression(Identifier id)
            {
                return ssa.Identifiers[id].GetDefiningExpression();
            }

            public List<Statement> GetDefiningStatementClosure(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression GetValue(Identifier id)
            {
                return values.TryGetValue(id, out var result)
                    ? result
                    : null!;
            }

            public Expression GetValue(MemoryAccess access, IReadOnlySegmentMap segmentMap)
            {
                return InvalidConstant.Create(access.DataType);
            }

            public Expression GetValue(SegmentedAccess access, IReadOnlySegmentMap segmentMap)
            {
                return InvalidConstant.Create(access.DataType);
            }

            public Expression GetValue(Application appl)
            {
                return InvalidConstant.Create(appl.DataType);
            }

            public bool IsUsedInPhi(Identifier id)
            {
                throw new NotImplementedException();
            }

            public Expression MakeSegmentedAddress(Constant seg, Constant off)
            {
                return arch.MakeSegmentedAddress(seg, off);
            }

            public Constant ReinterpretAsFloat(Constant rawBits)
            {
                return arch.ReinterpretAsFloat(rawBits);
            }

            public void RemoveExpressionUse(Expression expr)
            {
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
