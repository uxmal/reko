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
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    /// <summary>
    /// An implementation of <see cref="ICommandTarget"/> that does nothing.
    /// </summary>
    public class NullCommandTarget : ICommandTarget
    {
        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            return ValueTask.FromResult(false);
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            return false;
        }
    }
}
