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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis
{
    /// <summary>
    /// Represents the induced graph of phi functions in an SSA-converted
    /// procedure.
    /// </summary>
    public class PhiGraph : DirectedGraph<PhiAssignment>
    {
        private PhiAssignment[] phis;
        private SsaState ssa;

        public PhiGraph(SsaState ssa, IEnumerable<PhiAssignment> phis)
        {
            this.ssa = ssa;
            this.phis = phis.ToArray();
        }

        public ICollection<PhiAssignment> Nodes => phis;

        public void AddEdge(PhiAssignment nodeFrom, PhiAssignment nodeTo)
        {
            throw new NotSupportedException();
        }

        public bool ContainsEdge(PhiAssignment nodeFrom, PhiAssignment nodeTo)
        {
            throw new NotSupportedException();
        }

        public ICollection<PhiAssignment> Predecessors(PhiAssignment node)
        {
            throw new NotSupportedException();
        }

        public void RemoveEdge(PhiAssignment nodeFrom, PhiAssignment nodeTo)
        {
            throw new NotSupportedException();
        }

        public ICollection<PhiAssignment> Successors(PhiAssignment node)
        {
            return node.Src.Arguments
                .Select(a => a.Value)
                .OfType<Identifier>()
                .Select(i => ssa.Identifiers[i].DefStatement?.Instruction)
                .OfType<PhiAssignment>()
                .ToArray();
        }
    }
}
