#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Models a microcode goto, with an optional condition. This allows the expression of free-form
    /// control flow in RTL code. 
    /// </summary>
    /// <remarks>
    /// Micro-gotos are restricted to labels within the same <see cref="RtlInstructionCluster"/>.
    /// If you want to jump outside of the cluster, you may only jump to the start of a rewritten
    /// instruction using the <see cref="RtlGoto"/> instruction.
    /// </remarks>
    public class RtlMicroGoto : RtlInstruction
    {
        public RtlMicroGoto(object microTarget)
        {
            Debug.Assert(microTarget is string || microTarget is RtlLocation);
            this.Target = microTarget;
            this.Class = InstrClass.Transfer;
        }

        public RtlMicroGoto(Expression? condition, object microTarget)
        {
            Debug.Assert(microTarget is string || microTarget is RtlLocation);
            this.Condition = condition;
            this.Target = microTarget;
            this.Class = InstrClass.ConditionalTransfer;
        }

        /// <summary>
        /// If not null, this micro-goto is predicated on this condition. If
        /// null, the micro-goto is unconditional.
        /// </summary>
        public Expression? Condition { get; }

        /// <summary>
        /// Name of the microLabelName to jump to. The micro label must be inside
        /// the current RtlInstructionCluster.
        /// </summary>
        /// <remarks>
        /// The target can either be a symbolic string or an RtlLocation. Symbolic 
        /// strings are translated to RtlLocations during scanning.
        /// </remarks>
        public object Target { get; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitMicroGoto(this);
        }

        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitMicroGoto(this, context);
        }
        protected override void WriteInner(TextWriter writer)
        {
            if (Condition != null)
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
