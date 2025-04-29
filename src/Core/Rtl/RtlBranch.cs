#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Expressions;
using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Models a branch instruction in the RTL.
    /// </summary>
    public sealed class RtlBranch : RtlTransfer
    {
        /// <summary>
        /// Constructs an RTL branch instruction.
        /// </summary>
        /// <param name="condition">Predicate of the branch condition.</param>
        /// <param name="target">Target of the branch if the predicate evaluates to 'true'.</param>
        /// <param name="rtlClass"><see cref="InstrClass"/> of the branch.</param>
        public RtlBranch(Expression condition, Expression target, InstrClass rtlClass) 
            : base(target, rtlClass)
        {
            this.Condition = condition;
        }

        /// <summary>
        /// Predicate of the branch instruction.
        /// </summary>
        public Expression Condition { get; }

        /// <inheritdoc/>
        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitBranch(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitBranch(this, context);
        }

        /// <inheritdoc/>
        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("if (");
            writer.Write(Condition);
            writer.Write(") ");
            writer.Write("branch {0}", Target);
        }
    }
}
