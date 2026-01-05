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

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Abstract base class for all RTL control transfer instructions.
    /// </summary>
    public abstract class RtlTransfer : RtlInstruction
    {
        /// <summary>
        /// Initializes an instance of <see cref="RtlTransfer"/>.
        /// </summary>
        /// <param name="target">Destination of the control transfer.</param>
        /// <param name="rtlClass">Instruction class of this control transfer instruction.</param>
        public RtlTransfer(Expression target, InstrClass rtlClass)
        {
            this.Target = target;
            this.Class = rtlClass;
        }

        /// <summary>
        /// The target of the control transfer instruction.
        /// </summary>
        public Expression Target { get; }
    }
}
