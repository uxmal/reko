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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.IO;

namespace Reko.Scanning;

/// <summary>
/// Abstract base class for work items that can be processed by the <see cref="Scanner"/>
/// class.
/// </summary>
public abstract class WorkItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItem"/> class with the specified address.
    /// </summary>
    /// <param name="addr">Address associated with the work item.</param>
    protected WorkItem(Address addr)
    {
        this.Address = addr;
    }

    /// <summary>
    /// The address associated with this work item.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Processes the work item.
    /// </summary>
    public abstract void Process();
}
