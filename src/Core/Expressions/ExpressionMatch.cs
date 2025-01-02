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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Expressions
{
    public class ExpressionMatch
    {
        private readonly Dictionary<string, Expression> capturedExpressions;
        private readonly Dictionary<string, Operator> capturedOperators;

        internal ExpressionMatch()
        {
            this.capturedExpressions = new Dictionary<string, Expression>();
            this.capturedOperators = new Dictionary<string, Operator>();
            Success = false;
        }

        public bool Success { get; internal set; }
        internal Expression? Pattern { get; set; }

        internal bool Capture(string? label, Expression e)
        {
            if (label is null)
                return true;
            if (capturedExpressions.TryGetValue(label, out var oldE))
            {
                //$TODO: Do something?
            }
            capturedExpressions[label] = e;
            return true;
        }

        internal bool Capture(string label, Operator op)
        {
            if (capturedOperators.TryGetValue(label, out var oldOp))
            {
                //$TODO: Do something?
            }
            capturedOperators[label] = op;
            return true;
        }

        public Expression? CapturedExpression(string? label)
        {
            if (string.IsNullOrEmpty(label) || !capturedExpressions.TryGetValue(label, out Expression? value))
                return null;
            return value;
        }

        public Operator? CapturedOperator(string? label)
        {
            if (string.IsNullOrEmpty(label) || !capturedOperators.TryGetValue(label, out Operator? value))
                return null;
            return value;
        }
    }
}
