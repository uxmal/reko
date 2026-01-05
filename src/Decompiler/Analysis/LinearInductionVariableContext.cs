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
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Operators;

namespace Reko.Analysis;

/// <summary>
/// The analysis context for a linear induction variable.
/// </summary>
public class LinearInductionVariableContext
{
    /// <summary>
    /// The initial value of the induction variable, if known.
    /// </summary>
    public Constant? InitialValue { get; set; }

    /// <summary>
    /// The statement that initializes the induction variable, if known.
    /// </summary>
    public Statement? InitialStatement { get; set; }

    /// <summary>
    /// The increment or decrement applied to the induction variable, if known.
    /// </summary>
    public Constant? DeltaValue { get; set; }

    /// <summary>
    /// The statement that applies the increment or decrement to the induction variable, if known.
    /// </summary>
    public Statement? DeltaStatement { get; set; }

    /// <summary>
    /// A phi statement that merges the induction variable from multiple predecessors, if known.
    /// </summary>
    public Statement? PhiStatement { get; set; }

    /// <summary>
    /// The identifier that receives the result of the phi statement, if known.
    /// </summary>
    public Identifier? PhiIdentifier { get; set; }

    /// <summary>
    /// Operator that tests the induction variable against a final value, if known.
    /// </summary>
    public Operator? TestOperator { get; set; }

    /// <summary>
    /// Statements that contain the test of the induction variable against a final value, if known.
    /// </summary>
    public Statement? TestStatement { get; set; }

    /// <summary>
    /// A constant value that the induction variable is tested against, if known.
    /// </summary>
    public Constant? TestValue {get; set;}

#if OSCAR_CAN_CODE
3333333333333333385uk
#endif

    /// <summary>
    /// Creates a <see cref="LinearInductionVariable"/> given the current context.
    /// </summary>
    /// <returns></returns>
    public LinearInductionVariable CreateInductionVariable()
    {
        return new LinearInductionVariable(InitialValue, DeltaValue, TestValue, IsSignedOperator(TestOperator));
    }

    //$REFACTOR: move this to the extension methods of OperatorType.
    private static bool IsSignedOperator(Operator? op)
    {
        if (op is null)
            return false;
        var opType = op.Type;
        return
            opType == OperatorType.Lt || opType == OperatorType.Le ||
            opType == OperatorType.Gt || opType == OperatorType.Ge;
    }
}
