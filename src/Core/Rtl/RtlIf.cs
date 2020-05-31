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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Models conditionally executed statements, like those that appear in 
    /// the ARM architecture.
    /// </summary>
    public sealed class RtlIf : RtlInstruction
    {
        /// <summary>
        /// Builds an RTL If instruction, which executes the specified statement
        /// only if the condition is satisfied. The annulled flag is used to model
        /// processor architectures like SPARC, where there is a delay slot after a
        /// branch, and annullment allows the delay slot to not be executed if the
        /// branch is not taken.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="instr"></param>
        /// <param name="annulled"></param>
        public RtlIf(Expression condition, RtlInstruction instr)
        {
            this.Condition = condition;
            this.Instruction = instr;
            this.Class = instr.Class | InstrClass.Conditional;
        }

        public Expression Condition { get; }

        /// <summary>
        /// The conditionally executed RTL instruction.
        /// </summary>
        public RtlInstruction Instruction { get; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitIf(this);
        }

        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("if ({0}) ", Condition);
            Instruction.Write(writer);
        }
    }
}
