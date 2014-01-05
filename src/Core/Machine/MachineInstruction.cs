#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Machine
{
    /// <summary>
    /// Abstract base class for low-level machine instructions.
    /// </summary>
    public abstract class MachineInstruction
    {
        /// <summary>
        /// Returns any processor flags that are defined by the instruction when it is executed.
        /// Typically, ALU instructions will set flags to reflect the result of the operation.
        /// </summary>
        /// <returns></returns>
        public abstract uint DefCc();

        /// <summary>
        /// Returns any processor flags that are used by the instruction when it is executed. For instance, 
        /// conditional instructions and ADDC instructions will have a non-zero value here.
        /// </summary>
        /// <returns></returns>
        public abstract uint UseCc();
    }
}
