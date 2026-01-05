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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Machine;

/// <summary>
/// Specifies the type of automatic increment or decrement operation to apply, such as pre-increment, post-increment,
/// pre-decrement, or post-decrement.
/// </summary>
public enum AutoIncrement
{
    /// <summary>
    /// No automatic increment or decrement operation is applied.
    /// </summary>
    None,

    /// <summary>
    /// Base register is decremented after the memory access.
    /// </summary>
    PostDecrement,

    /// <summary>
    /// Base register is incremented after the memory access.
    /// </summary>
    PostIncrement,

    /// <summary>
    /// Base register is decremented before the memory access.
    /// </summary>
    PreDecrement,

    /// <summary>
    /// Base register is incremented before the memory access.
    /// </summary>
    PreIncrement
}
