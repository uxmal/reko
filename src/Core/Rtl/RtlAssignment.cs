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
    /// Models an assignment instruction in RTL.
    /// </summary>
    public sealed class RtlAssignment : RtlInstruction
    {
        /// <summary>
        /// Constructs an assignment instruction.
        /// </summary>
        /// <param name="dst">Destination of the assignment.</param>
        /// <param name="src">Source of the assignment.</param>
        public RtlAssignment(Expression dst, Expression src)
        {
            this.Dst = dst;
            this.Src = src;
            this.Class = InstrClass.Linear;
        }

        /// <summary>
        /// Gets the destination of the assignment.
        /// </summary>
        public Expression Dst { get; }

        /// <summary>
        /// Gets the source of the assignment.
        /// </summary>
        public Expression Src { get; }

        /// <inheritdoc/>
        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        /// <inheritdoc/>
        public override T Accept<T,C>(IRtlInstructionVisitor<T,C> visitor, C context)
        {
            return visitor.VisitAssignment(this, context);
        }

        /// <inheritdoc/>
        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("{0} = {1}", Dst, Src);
        }
    }
}
