/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
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
    ///   If one and only one of them is a flag register, it becomes the return value
    ///     ingelsta.
    ///   If more registers are returned, they all become out registers and the function
    ///    is declared void, unless flags are returned.
    /// </remarks>
    public class ApplicationBuilder
	{
        private Identifier idOut;
        private Application appl;
        private Frame frame;

        public ApplicationBuilder(Frame frame, CallSite cs, Expression callee, ProcedureSignature sigCallee)
        {
            this.frame = frame;
			if (sigCallee == null || !sigCallee.ArgumentsValid)
				throw new InvalidOperationException("No signature available; application cannot be constructed.");


            FindReturnValue(cs, sigCallee);
            List<Expression> actuals = BindArguments(frame, cs, sigCallee);
			appl = new Application(
                callee, 
                (idOut == null ? PrimitiveType.Void : idOut.DataType),
                actuals.ToArray());
        }

        private List<Expression> BindArguments(Frame frame, CallSite cs, ProcedureSignature sigCallee)
        {
            List<Expression> actuals = new List<Expression>();
            for (int i = 0; i < sigCallee.FormalArguments.Length; ++i)
            {
                Identifier formalArg = sigCallee.FormalArguments[i];
                Identifier actualArg = formalArg.Storage.BindFormalArgumentToFrame(frame, cs);
                if (formalArg.Storage is OutArgumentStorage)
                {
                    actuals.Add(new UnaryExpression(UnaryOperator.addrOf, frame.FramePointer.DataType, actualArg));
                }
                else
                {
                    actuals.Add(actualArg);
                }
            }
            return actuals;
        }

        private void FindReturnValue(CallSite cs, ProcedureSignature sigCallee)
        {
            idOut = null;
            if (sigCallee.ReturnValue != null)
            {
                idOut = Bind(sigCallee.ReturnValue, cs);
            }

        }

		public Identifier Bind(Identifier id, CallSite cs)
		{
			return id.Storage.BindFormalArgumentToFrame(frame, cs);
		}

        public Instruction CreateInstruction()
        {
			if (idOut == null)
			{
                return new SideEffect(appl);
			}
			else
			{
                return new Assignment(idOut, appl);
			}
        }
    }
}
