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

using Reko.Core.Expressions;
using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Represents a side effect in the RTL code. This is used to model
    /// instructions which are executed for their side effects.
    /// </summary>
    public sealed class RtlSideEffect : RtlInstruction
    {
        /// <summary>
        /// Constructs a side effect instruction.
        /// </summary>
        /// <param name="sideEffect">Expression evaluated for its side effect.</param>
        /// <param name="iclass">The <see cref="InstrClass"/> of this instruction.</param>
        public RtlSideEffect(Expression sideEffect, InstrClass iclass)
        {
            this.Expression = sideEffect;
            this.Class = iclass;
        }

        /// <summary>
        /// Expression to be evaluated for its side effect.
        /// </summary>
        public Expression Expression { get; }

        /// <inheritdoc/>
        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitSideEffect(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSideEffect(this, context);
        }

        /// <inheritdoc/>
        protected override void WriteInner(TextWriter writer)
        {
            writer.Write(Expression);
        }
    }
}
