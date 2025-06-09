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
    /// Models a microcode goto, with an optional condition. This allows the expression of free-form
    /// control flow in RTL code. 
    /// </summary>
    /// <remarks>
    /// Micro-gotos are restricted to offsets within the same <see cref="RtlInstructionCluster"/>.
    /// If you want to jump outside of the cluster, you may only jump to the start of a rewritten
    /// instruction using the <see cref="RtlGoto"/> instruction.
    /// </remarks>
    public sealed class RtlMicroGoto : RtlInstruction
    {
        /// <summary>
        /// Constructs an unconditional <see cref="RtlMicroGoto"/> instance.
        /// </summary>
        /// <param name="indexTarget">Index into the RTL instructions of the current 
        /// RTL cluster.</param>
        public RtlMicroGoto(int indexTarget)
        {
            this.Target = indexTarget;
            this.Class = InstrClass.Transfer;
        }


        /// <summary>
        /// Constructs a conditional <see cref="RtlMicroGoto"/> instance.
        /// </summary>
        /// <param name="condition">Predicate for the conditonal jump</param>
        /// <param name="indexTarget">Index into the RTL instructions of the current 
        /// RTL cluster.</param>
        public RtlMicroGoto(Expression? condition, int indexTarget)
        {
            this.Condition = condition;
            this.Target = indexTarget;
            this.Class = InstrClass.ConditionalTransfer;
        }

        /// <summary>
        /// If not null, this micro-goto is predicated on this condition. If
        /// null, the micro-goto is unconditional.
        /// </summary>
        public Expression? Condition { get; }

        /// <summary>
        /// Name of the microLabelName to jump to. 
        /// The target must be an index into the <see cref="RtlInstruction"/>s of the 
        /// current <see cref="RtlInstructionCluster"/>.
        /// the current RtlInstructionCluster.
        /// </summary>
        public int Target { get; }

        /// <inheritdoc/>
        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitMicroGoto(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitMicroGoto(this, context);
        }

        /// <inheritdoc/>
        protected override void WriteInner(TextWriter writer)
        {
            if (Condition is not null)
            {
                writer.Write("if ({0}) micro_goto {1}", Condition, Target);
            }
            else
            {
                writer.Write("micro_goto {0}", Target);
            }
        }
    }
}
