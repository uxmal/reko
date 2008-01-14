/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System.Collections;

namespace Decompiler.Core
{
	/// <summary>
	/// Builds a function application from a call site and a callee.
	/// </summary>
	public class ApplicationBuilder
	{
		private Frame frame;

		public ApplicationBuilder(Frame f)
		{
			this.frame = f;
		}

		public Identifier Bind(Identifier id, CallSite cs)
		{
			return id.Storage.BindFormalArgumentToFrame(frame, cs);
		}

		/// <summary>
		/// Builds actual argument bindings from a callee.
		/// </summary>
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
		/// <param name="callee"></param>
		/// <returns></returns>
		public Instruction BuildApplication(CallSite cs, Expression callee, ProcedureSignature sigCallee)
		{
			if (sigCallee == null || !sigCallee.ArgumentsValid)
				return null;

			ArrayList actuals = new ArrayList();

			Expression idOut = null;
			if (sigCallee.ReturnValue != null)
			{
				idOut = Bind(sigCallee.ReturnValue, cs);
			}

			for (int i = 0; i < sigCallee.Arguments.Length; ++i)
			{
				Identifier formalArg = sigCallee.Arguments[i];
				Identifier actualArg = formalArg.Storage.BindFormalArgumentToFrame(frame, cs);
				if (formalArg.Storage is OutArgumentStorage)
				{
					actuals.Add(new UnaryExpression(UnaryOperator.addrOf, PrimitiveType.Pointer, actualArg));
				}
				else
				{
					actuals.Add(actualArg);
				}
			}
			
			Expression [] act = (Expression []) actuals.ToArray(typeof (Expression));
			Application appl = new Application(callee, (idOut == null ? null : idOut.DataType), act);

			if (idOut == null)
			{
				appl.DataType = PrimitiveType.Void;
				return new SideEffect(appl);
			}
			else
			{
				return new Assignment((Identifier) idOut, appl);
			}
		}
	}
}
