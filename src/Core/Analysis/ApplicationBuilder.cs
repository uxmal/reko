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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Core.Analysis
{
    /// <summary>
    /// Builds a function application from a call site and a callee.
    /// </summary>
    /// <remarks>
    /// The principal purpose of this class is to bind <see cref="Identifier"/>s 
    /// in the caller's frame to the <see cref="Storage"/>s  in the callee's
    /// procedure signature.
    /// 
    /// liveIn registers that are passed in and out are marked as 'out arguments'
    ///
    /// If there is only one 'out' argument, then it is returned as the return value of the
    /// function. If there are several return values:
    /// <list type="bullet">
    /// <item>If one and only one of them is a flag register, it becomes the return value.</item>  
    /// <item>If more registers are returned, they all become out registers and the function
    ///    is declared void, unless flags are returned.</item>  
    /// </list>
    /// </remarks>
    public abstract class ApplicationBuilder
    {
        /// <summary>
        /// The <see cref="FunctionType">function signature</see> for the called
        /// procedure.
        /// </summary>
        protected FunctionType? sigCallee;

        /// <summary>
        /// Creates an application builder that creates references
        /// to the identifiers in the caller's frame.
        /// </summary>
        /// <param name="site">The call site of the calling instruction.</param>
        public ApplicationBuilder(CallSite site)
        {
            Site = site;
        }


        /// <summary>
        /// The <see cref="CallSite"/> of the calling instruction.
        /// </summary>
        public CallSite Site { get; }

        /// <summary>
        /// Bind an input argument in the caller's frame to the corresponding
        /// <see cref="Storage"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ApplicationBuilder"/> is responsible for creating
        /// appropriate argument variables for each of the callee function's 
        /// input parameters. For instance, if the callee function has a 
        /// input register parameter "R1", an implementing class must ensure
        /// that the caller proceudre has a corresponding identifier for
        /// that register.
        /// </remarks>
        /// <param name="stg">The storage to bind in the caller's <see cref="Frame"/>.
        /// </param>
        /// <returns>The bound storage, or null if the argument couldn't be
        /// bound.
        /// </returns>
        public abstract Expression? BindInArg(Storage stg);


        /// <summary>
        /// Bind an input stack argument in the caller's frame to the corresponding
        /// <see cref="Storage"/>.
        /// <remarks>
        /// See <see cref="BindInArg(Storage)"/>for more details.</remarks>
        /// </summary>
        /// <param name="stg">The <see cref="StackStorage"/> to bind in the caller's <see cref="Frame"/>.
        /// </param>
        /// <param name="returnAdjustment">The number of storage units to adjust stack offsets.
        /// </param>
        /// <returns>The bound storage, or null if the argument couldn't be
        /// bound.
        /// </returns>
        public abstract Expression? BindInStackArg(StackStorage stg, int returnAdjustment);

        /// <summary>
        /// Binds an output argument in the caller's frame to the corresponding
        /// <see cref="Storage"/>.
        /// </summary>
        public abstract OutArgument BindOutArg(Storage stg);

        /// <summary>
        /// Binds the return value of the called procedure into a storage in 
        /// the calling procedure.
        /// </summary>
        /// <param name="stg">The <see cref="Storage"/> in which the called procedure returns
        /// its values. It is null if the called procedure doesn't return
        /// any data.</param>
        public abstract Expression? BindReturnValue(Storage? stg);

        /// <summary>
        /// Creates an <see cref="Application"/> expression, and a hosting
        /// instruction:
        ///     a = foo(b)
        /// or 
        ///     foo(b)
        /// depending on whether the <paramref name="sigCallee"/> returns a
        /// value or is of type 'void'.
        /// </summary>
        /// <param name="callee">The expression denoting the procedure to call.
        /// For direct calls, this is typically a <see cref="ProcedureConstant"/>,
        /// but for indirect calls it can be any <see cref="Expression"/>.
        /// </param>
        /// <param name="sigCallee">The <see cref="FunctionType"/> to use when 
        /// constructing the parameters of the application.
        /// </param>
        /// <param name="chr">Optional <see cref="ProcedureCharacteristics"/>
        /// that may influence the result of this method call.
        /// </param>
        /// <returns>The resulting <see cref="Assignment"/> or 
        /// <see cref="SideEffect"/> instruction.</returns>
        public Instruction CreateInstruction(
            Expression callee,
            FunctionType sigCallee,
            ProcedureCharacteristics? chr)
        {
            if (sigCallee is null || !sigCallee.ParametersValid)
                throw new InvalidOperationException("No signature available; application cannot be constructed.");
            this.sigCallee = sigCallee;
            var parameters = sigCallee.Parameters!;     // Since we checked ParametersValid, we are guaranteed there are parameters.
            Expression? expOut = null;
            DataType dtOut = VoidType.Instance;
            if (!sigCallee.HasVoidReturn)
            {
                var bindingOut = BindReturnValue(sigCallee.ReturnValue?.Storage);
                expOut = bindingOut;
                dtOut = sigCallee.ReturnValue!.DataType;
            }
            var actuals = BindArguments(parameters, sigCallee.IsVariadic, chr).ToArray();
            Expression appl = new Application(callee, dtOut, actuals);

            switch (expOut)
            {
            case Identifier idOut:
                {
                    if (idOut.Storage is not FlagGroupStorage)
                    {
                        int extraBits = idOut.DataType.BitSize - sigCallee.ReturnValue.DataType.BitSize;
                        if (extraBits > 0)
                        {
                            // The non-live bits of expOut can safely be replaced with anything,
                            // since they won't be used. Later stages of the decompilation
                            // process should remove the unused bits.
                            var dtUnused = PrimitiveType.CreateWord(extraBits);
                            var unused = Constant.Create(dtUnused, 0);
                            appl = new MkSequence(expOut.DataType, unused, appl);
                        }
                    }
                    return new Assignment(idOut, appl);
                }
            case MkSequence seq:
                return new Store(seq, appl);
            case null:
                return new SideEffect(appl);
            default:
                return new Store(expOut, appl);
            }
        }

        /// <summary>
        /// Bind the formal parameters of the signature to actual arguments in
        /// the frame of the calling procedure.
        /// </summary>
        /// <param name="parameters">The formal parameters of the callee 
        /// procedure.</param>
        /// <param name="isVariadic">True if the calle function is variadic (e.g. <c>printf</c>).</param>
        /// <param name="chr">The <see cref="ProcedureCharacteristics"/> of the called procedure, if any.</param>
        /// <returns>The resulting list of actual arguments.</returns>
        public virtual List<Expression> BindArguments(Identifier[] parameters, bool isVariadic, ProcedureCharacteristics? chr)
        {
            var actuals = new List<Expression>();
            for (int i = 0; i < parameters.Length; ++i)
            {
                var formalArg = parameters[i];
                if (formalArg.Storage is OutArgumentStorage outStg)
                {
                    var outArg = BindOutArg(outStg);
                    actuals.Add(outArg);
                }
                else
                {
                    var actualArg = BindInArg(formalArg.Storage);
                    //$REVIEW: what does null mean here? Forcing an error here generates
                    // regressions in the unit tests.
                    if (actualArg is not null)
                    {
                        if (actualArg.DataType.BitSize > formalArg.DataType.BitSize)
                        {
                            actualArg = new Slice(formalArg.DataType, actualArg, 0);
                        }
                        actuals.Add(actualArg);
                    }
                }
            }
            if (isVariadic)
            {
                return BindVariadicArguments(chr, actuals);
            }
            else
            {
                return actuals;
            }
        }

        /// <summary>
        /// Bind the variadic arguments of a function call.
        /// </summary>
        /// <param name="chr">The <see cref="ProcedureCharacteristics"/> for the
        /// variadic callee. This contains information about how to interpret
        /// format strings or other mechanisms to unpack the number of arguments.
        /// </param>
        /// <param name="actuals">The list of non-variadic actual arguments of the
        /// call.</param>
        /// <returns>A list consisting of all actual arguments including any variadic ones.
        /// </returns>
        public List<Expression> BindVariadicArguments(ProcedureCharacteristics? chr, List<Expression> actuals)
        {
            actuals.Add(Constant.Word32(0));
            Debug.Print($"{nameof(ApplicationBuilder)}: Varargs are not implemented yet.");
            return actuals;
        }
    }
}
