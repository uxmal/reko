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

using System.IO;

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Models instruction with no effect on the architectural state.
    /// </summary>
    public sealed class RtlNop : RtlInstruction
    {
        /// <summary>
        /// Creates a no-op instruction.
        /// </summary>
        /// <param name="iclass">Instruction class for the instruction.</param>
        public RtlNop(InstrClass iclass = 0)
        {
            base.Class = InstrClass.Padding | InstrClass.Linear | iclass;
        }

        /// <inheritdoc/>
        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitNop(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(IRtlInstructionVisitor<T, C> visitor, C context)
        {
            return visitor.VisitNop(this, context);
        }

        /// <inheritdoc/>
        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("nop");
        }
    }
}
