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

#pragma warning disable IDE1006

using Reko.Core.Expressions;
using Reko.Core.Memory;

namespace Reko.Core
{
    /// <summary>
    /// An EvaluationContext is used by the SymbolicEvaluator to provide a 
    /// statement context for the evaluation.
    /// </summary>
    /// <remarks>
    /// For instance, it might be interesting to find the expression currently
    /// bound to an identifier, to see if a simplification could be made. The
    /// statements
    /// <code>
    ///     a = constant
    ///     b = a + 3
    /// </code>
    /// can be merged to
    /// <code>
    ///     b = (constant + 3)
    /// </code>
    /// if we know that a == constant.
    /// </remarks>
    public interface EvaluationContext
    {
        /// <summary>
        /// Gets the endianness of the program being analyzed.
        /// </summary>
        EndianServices Endianness { get; }

        /// <summary>
        /// The size of a memory storage unit in bits. On most modern 
        /// architectures this is 8, but some architectures differ.
        /// </summary>
        int MemoryGranularity { get; }

        /// <summary>
        /// Gets the symbolic value of the identifier <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Expression? GetValue(Identifier id);

        /// <summary>
        /// Retrieves the value located at the address obtained by evaluating
        /// the <paramref name="access"/> expression.
        /// </summary>
        /// <param name="access">Memory access expression.</param>
        /// <param name="memory">The memory of the program
        /// being analyzed.</param>
        /// <returns>An expression at the memory address, or an instance
        /// of <see cref="InvalidConstant"/> if the address couldn't be resolved.
        /// </returns>
        Expression GetValue(MemoryAccess access, IMemory memory);


        /// <summary>
        /// Evaluates the given application, which might return in a simplification
        /// depending on the arguments of the call.
        /// </summary>
        /// <param name="appl">An application to evaluate.</param>
        /// <returns>The evaluated value.</returns>
        Expression GetValue(Application appl);
        
        /// <summary>
        /// Given an identifier, finds the expression that defined it.
        /// </summary>
        /// <param name="id">Identifier whose definition is to be found.</param>
        /// <returns>The defining expression, or null if it can't be found.
        /// </returns>
        Expression? GetDefiningExpression(Identifier id);

        /// <summary>
        /// Indicate that all identifiers in the expression is used in the current statement.
        /// </summary>
        /// <param name="expr">Expression whose identifiers are to be marked as used.
        /// </param>
        void UseExpression(Expression expr);

        /// <summary>
        /// Indicate that all identifiers in the expression is no longer used in the current statement.
        /// </summary>
        /// <param name="expr">Expression whose identifiers are to be marked as unused.
        /// </param>
        void RemoveExpressionUse(Expression expr);

        /// <summary>
        /// Record that the identifier <paramref name="id"/> has the given value.
        /// </summary>
        /// <param name="id">Identifier whose value is to be recorded.</param>
        /// <param name="value">The value to record.
        /// </param>
        void SetValue(Identifier id, Expression value);

        /// <summary>
        /// Given the effective address <paramref name="ea"/>, set the value of the memory
        /// at that address to <paramref name="value"/>.
        /// </summary>
        /// <param name="ea">Effective address.</param>
        /// <param name="value">Value to wrote to that address.</param>
        void SetValueEa(Expression ea, Expression value);

        /// <summary>
        /// Set the value at the memory address specified by <paramref name="basePointer"/>
        /// and <paramref name="ea"/> to the <paramref name="value"/>.
        /// </summary>
        /// <param name="basePointer"></param>
        /// <param name="ea">Effective address of the address whose value is to be set.</param>
        /// <param name="value">Value to set.</param>
        void SetValueEa(Expression basePointer, Expression ea, Expression value);

        /// <summary>
        /// Determines whether the identifier <paramref name="id"/> is used in a
        /// <see cref="PhiFunction"/>.
        /// </summary>
        /// <param name="id">Identifier to test.</param>
        /// <returns>True if the identifier is used in a <see cref="PhiFunction"/>;
        /// otherwise false.
        /// </returns>
        bool IsUsedInPhi(Identifier id);

        /// <summary>
        /// Given two constants, creates a segmented address expression.
        /// </summary>
        /// <param name="selector">Segment selector.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>A segmented address expression.</returns>
        Expression MakeSegmentedAddress(Constant selector, Constant offset);

        /// <summary>
        /// Reinterprets a string of raw bits as a floating point number appropriate
        /// for the current architecture.
        /// </summary>
        /// <param name="rawBits">Raw bits to be interpreted.</param>
        /// <returns></returns>
        Constant ReinterpretAsFloat(Constant rawBits);
    }
}
