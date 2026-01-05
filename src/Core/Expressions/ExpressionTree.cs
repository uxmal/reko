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

using Reko.Core.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Wraps a tree with the <see cref="DirectedGraph{T}" /> interface so that
    /// <see cref="DfsIterator{T}"/> can walk it.
    /// </summary>
    public class ExpressionTree : DirectedGraph<Expression>
    {
        /// <inheritdoc/>
        public ICollection<Expression> Nodes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public void AddEdge(Expression nodeFrom, Expression nodeTo)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ContainsEdge(Expression nodeFrom, Expression nodeTo)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ICollection<Expression> Predecessors(Expression node)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveEdge(Expression nodeFrom, Expression nodeTo)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ICollection<Expression> Successors(Expression node)
        {
            return node.Children.ToList();
        }
    }
}
