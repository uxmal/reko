#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Types;
using Reko.Typing;
using System.Collections.Generic;

namespace Reko.Analysis
{
    /// <summary>
    /// A <see cref="LocalTypeDescender"/> is a subclass of 
    /// <see cref="ExpressionTypeDescender"/> that only involves
    /// expressions and variables of a single SSA procedure.
    /// </summary>
    public class LocalTypeDescender : ExpressionTypeDescender
    {
        public LocalTypeDescender(IReadOnlyProgram program, TypeStore store, TypeFactory factory)
            : base(program, store, factory)
        {
            this.TypeVariables = new Dictionary<Expression,TypeVariable>();
        }

        public Dictionary<Expression, TypeVariable> TypeVariables { get; }

        public void BuildEquivalenceClasses(SsaState ssa)
        {
            foreach (var sid in ssa.Identifiers)
            {
                if (sid.DefStatement?.Instruction is PhiAssignment phi)
                {
                    var tv = TypeVar(phi.Dst);
                    foreach (var (_, v) in phi.Src.Arguments)
                    {
                        store.MergeClasses(tv, TypeVar(v));
                    }
                }
            }
        }

        public void MergeDataTypes()
        {
            var eqTypes = new Dictionary<EquivalenceClass, DataType>();
            foreach (var (_, tv) in TypeVariables)
            {
                if (eqTypes.TryGetValue(tv.Class, out var dt))
                {
                    eqTypes[tv.Class] = unifier.Unify(dt, tv.DataType) ?? dt;
                }
                else
                {
                    eqTypes[tv.Class] = tv.DataType;
                }
            }
            foreach (var (eq, dt) in eqTypes)
            {
                eq.DataType = dt;
            }
            foreach (var (_, tv) in TypeVariables)
            {
                tv.DataType = tv.Class.DataType;
            }
        }

        protected override TypeVariable TypeVar(Expression exp)
        {
            if (TypeVariables.TryGetValue(exp, out var tv))
                return tv;
            tv = store.CreateTypeVariable(factory);
            tv.DataType = exp.DataType;
            TypeVariables.Add(exp, tv);
            return tv;
        }

        public void Visit(Expression exp)
        {
            var tv = TypeVar(exp);
            exp.Accept(this, tv);
        }
    }
}
