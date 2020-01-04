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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Wraps a tree with the DirectedGraph interface so that
    /// DfsIterator can walk it.
    /// </summary>
    public class ExpressionTree : DirectedGraph<Expression>
    {
        public ExpressionTree()
        {
        }

        public ICollection<Expression> Nodes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AddEdge(Expression nodeFrom, Expression nodeTo)
        {
            throw new NotImplementedException();
        }

        public bool ContainsEdge(Expression nodeFrom, Expression nodeTo)
        {
            throw new NotImplementedException();
        }

        public ICollection<Expression> Predecessors(Expression node)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdge(Expression nodeFrom, Expression nodeTo)
        {
            throw new NotImplementedException();
        }

        public ICollection<Expression> Successors(Expression node)
        {
            return node.Children.ToList();
        }
    }
}
