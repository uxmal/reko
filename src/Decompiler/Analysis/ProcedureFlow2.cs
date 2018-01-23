#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Analysis
{
    public class ProcedureFlow2
    {
        public ProcedureFlow2()
        {
            this.Preserved = new HashSet<Storage>();
            this.Trashed = new HashSet<Storage>();
            this.Constants = new Dictionary<Storage, Constant>();
        }

        /// <summary>
        /// Locations that are preserved by a procedure.
        /// </summary>
        public HashSet<Storage> Preserved { get; private set; }
        /// <summary>
        /// Locations that are trashed by a procedure.
        /// </summary>
        public HashSet<Storage> Trashed { get; private set; }
        /// <summary>
        /// Locations that have a constant value at the end of a procedure.
        /// </summary>
        public Dictionary<Storage, Constant> Constants { get; private set; }
    }
}
