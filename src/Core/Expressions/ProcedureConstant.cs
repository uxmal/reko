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

using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Wraps a reference to a <see cref="ProcedureBase"/> inside of an
    /// <see cref="Expression" />. It can be considered a pointer to
    /// the procedure's code.
    /// </summary>
	public class ProcedureConstant : AbstractExpression
	{
        private readonly FunctionType? sig;

        /// <summary>
        /// Creates a procedure constant.
        /// </summary>
        /// <param name="ptrType"></param>
        /// <param name="proc"></param>
		public ProcedureConstant(DataType ptrType, ProcedureBase proc) : base(ptrType)
		{
            this.Procedure = proc;
            this.sig = null;
            // Clone user-defined or platform-defined signatures so that they
            // can not be changed during Type Analysis
            if (proc.Signature is not null && proc.Signature.UserDefined)
            {
                this.sig = proc.Signature.Clone(shareIncompleteTypes: true);
            }
        }

        /// <summary>
        /// Creates a procedure constant with an overridden known signature <paramref name="sig" />,
        /// matching the actual parameters passed to the call.
        /// </summary>
        /// <remarks>
        /// This constructor is used in situations when the called function's
        /// signature differs from the actual invocation in the program code.
        /// The most typical case are calls to varargs procedures, where the 
        /// formal signature may have fewer parameters than the actual
        /// arguments used when calling it.
        /// </remarks>
        public ProcedureConstant(DataType ptrType, FunctionType sig, ProcedureBase proc) : base(ptrType)
        {
            this.Procedure = proc;
            this.sig = sig;
        }

        /// <summary>
        /// The <see cref="ProcedureBase"/> being referred to.
        /// </summary>
        public ProcedureBase Procedure { get; }

        /// <summary>
        /// The signature of the procedure.
        /// </summary>
        public FunctionType Signature
        {
            get => this.sig ?? Procedure.Signature;
        }

        /// <inheritdoc/>
        public override IEnumerable<Expression> Children
        {
            get { yield break; }
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitProcedureConstant(this, context);
        }

        /// <inheritdoc/>
        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitProcedureConstant(this);
        }

        /// <inheritdoc/>
		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitProcedureConstant(this);
		}

        /// <inheritdoc/>
		public override Expression CloneExpression()
		{
            return this;
		}
	}
}
