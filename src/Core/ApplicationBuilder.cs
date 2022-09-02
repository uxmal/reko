#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

namespace Reko.Core
{
    /// <summary>
    /// Builds a function application from a call site and a callee.
    /// </summary>
    /// <summary>
    /// <remarks>
    /// liveIn registers that are passed in and out are marked as 'out arguments'
    ///
    /// If there is only one 'out' argument, then it is returned as the return value of the
    /// function. If there are several return values:
    ///   If one and only one of them is a flag register, it becomes the return value.
    ///   If more registers are returned, they all become out registers and the function
    ///    is declared void, unless flags are returned.
    /// </remarks>
    public abstract class ApplicationBuilder 
	{
        protected CallSite site;
        protected Expression callee;
        protected FunctionType? sigCallee;

        /// <summary>
        /// Creates an application builder that creates references
        /// to the identifiers in the frame.
        /// </summary>
        /// <param name="site">The call site of the calling instruction.</param>
        /// <param name="callee">The procedure being called.</param>
        /// <param name="sigCallee">The signature of the procedure being called.</param>
        public ApplicationBuilder(CallSite site, Expression callee)
        {
            this.site = site;
            this.callee = callee;
        }

        public abstract Expression? BindInArg(Storage stg);
        public abstract Expression? BindInStackArg(StackStorage stg, int returnAdjustment);
        public abstract OutArgument BindOutArg(Storage stg);
        public abstract Expression? BindReturnValue(Storage? stg);

        /// <summary>
        /// Creates an instruction:
        ///     a = foo(b)
        /// or 
        ///     foo(b)
        ///  depending on whether the signature returns a value or is of
        /// type 'void'
        /// </summary>
        /// <returns></returns>
        public Instruction CreateInstruction(
            FunctionType sigCallee,
            ProcedureCharacteristics? chr)
        {
            if (sigCallee == null || !sigCallee.ParametersValid)
                throw new InvalidOperationException("No signature available; application cannot be constructed.");
            this.sigCallee = sigCallee;
            var parameters = sigCallee.Parameters!;     // Since we checked ParametersValid, we are guaranteed there are parameters.
            Expression? expOut = null;
            DataType dtOut = VoidType.Instance;
            if (!sigCallee.HasVoidReturn)
            {
                expOut = BindReturnValue(sigCallee.ReturnValue?.Storage);
                dtOut = sigCallee.ReturnValue!.DataType;
            }
            var actuals = BindArguments(parameters, sigCallee.IsVariadic, chr).ToArray();
            Expression appl = new Application(callee, dtOut, actuals);

            //$TODO: refactor to 'switch' statement.
            if (expOut == null)
            {
                return new SideEffect(appl);
            }
            else if (expOut is Identifier idOut)
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
                return new Assignment(idOut, appl);
            }
            else if (expOut is MkSequence seq)
            {
                return new Store(seq, appl);
            }
            else
            {
                return new Store(expOut, appl);
            }
        }

        /// <summary>
        /// Bind the formal parameters of the signature to actual arguments in
        /// the frame of the calling procedure.
        /// </summary>
        /// <param name="sigCallee"></param>
        /// <param name="chr"></param>
        /// <returns></returns>
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

        public List<Expression> BindVariadicArguments(ProcedureCharacteristics? chr, List<Expression> actuals)
        {
            actuals.Add(Constant.Word32(0));
            Debug.Print($"{nameof(ApplicationBuilder)}: Varargs are not implemented yet.");
            return actuals;
        }
    }
}
