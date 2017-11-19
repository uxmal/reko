#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

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
    public class ApplicationBuilder : StorageVisitor<Expression>
	{
        private IProcessorArchitecture arch;
        private Frame frame;
        private CallSite site;
        private Expression callee;
        private FunctionType sigCallee;
        private bool ensureVariables;

        /// <summary>
        /// Creates an application builder.
        /// </summary>
        /// <param name="arch">The processor architecture to use.</param>
        /// <param name="frame">The Frame of the calling procedure.</param>
        /// <param name="site">The call site of the calling instruction.</param>
        /// <param name="callee">The procedure being called.</param>
        /// <param name="sigCallee">The signature of the procedure being called.</param>
        /// <param name="ensureVariables">If true, creates variables in the <paramref name="frame"/> if needed.</param>
        public ApplicationBuilder(
            IProcessorArchitecture arch, 
            Frame frame,
            CallSite site,
            Expression callee,
            FunctionType sigCallee,
            bool ensureVariables)
        {
            this.arch = arch;
            this.site = site;
            this.frame = frame;
            this.callee = callee;
            this.sigCallee = sigCallee;
            this.ensureVariables = ensureVariables;
        }

        public virtual List<Expression> BindArguments(Frame frame, FunctionType sigCallee)
        {
            if (sigCallee == null || !sigCallee.ParametersValid)
                throw new InvalidOperationException("No signature available; application cannot be constructed.");
            this.sigCallee = sigCallee;
            var actuals = new List<Expression>();
            for (int i = 0; i < sigCallee.Parameters.Length; ++i)
            {
                var formalArg = sigCallee.Parameters[i];
                var actualArg = formalArg.Storage.Accept(this);
                if (formalArg.Storage is OutArgumentStorage)
                {
                    actuals.Add(new OutArgument(frame.FramePointer.DataType, actualArg));
                }
                else
                {
                    actuals.Add(actualArg);
                }
            }
            return actuals;
        }

        public Identifier BindReturnValue()
        {
            if (sigCallee.HasVoidReturn)
                return null;
            return (Identifier) Bind(sigCallee.ReturnValue);
        }

		public Expression Bind(Identifier id)
		{
            if (id == null)
                return null;
            return id.Storage.Accept(this);
		}

        /// <summary>
        /// Creates an instruction:
        ///     a = foo(b)
        /// or 
        ///     foo(b)
        ///  depending on whether the signature returns a value or is of
        /// type 'void'
        /// </summary>
        /// <returns></returns>
        public Instruction CreateInstruction()
        {
            var idOut = BindReturnValue();
            var dtOut = sigCallee.HasVoidReturn
                ? VoidType.Instance
                : sigCallee.ReturnValue.DataType;
            var actuals = BindArguments(frame, sigCallee);
            Expression appl = new Application(
                callee,
                dtOut,
                actuals.ToArray());

			if (idOut == null)
			{
                return new SideEffect(appl);
			}
			else
			{
                if (idOut.DataType.Size > sigCallee.ReturnValue.DataType.Size)
                {
                    appl = new DepositBits(idOut, appl, 0);
                }
                return new Assignment(idOut, appl);
			}
        }

        #region StorageVisitor<Expression> Members

        public Expression VisitFlagGroupStorage(FlagGroupStorage grf)
        {
            return frame.EnsureFlagGroup(grf.FlagRegister, grf.FlagGroupBits, grf.Name, grf.DataType);
        }

        public Expression VisitFlagRegister(FlagRegister freg)
        {
            throw new NotSupportedException();
        }

        public Expression VisitFpuStackStorage(FpuStackStorage fpu)
        {
            return frame.EnsureFpuStackVariable(fpu.FpuStackOffset - site.FpuStackDepthBefore, fpu.DataType);
        }

        public Expression VisitMemoryStorage(MemoryStorage global)
        {
            throw new NotSupportedException(string.Format("A {0} can't be used as a formal parameter.", global.GetType().FullName));
        }

        public Expression VisitStackLocalStorage(StackLocalStorage local)
        {
            throw new NotSupportedException(string.Format("A {0} can't be used as a formal parameter.", local.GetType().FullName));
        }

        public Expression VisitOutArgumentStorage(OutArgumentStorage arg)
        {
            return arg.OriginalIdentifier.Storage.Accept(this);
        }

        public Expression VisitRegisterStorage(RegisterStorage reg)
        {
			return frame.EnsureRegister(reg);
        }

        public Expression VisitSequenceStorage(SequenceStorage seq)
        {
            var h = seq.Head.Accept(this);
            var t = seq.Tail.Accept(this);
            var idHead = h as Identifier;
            var idTail = t as Identifier;
            if (idHead != null && idTail != null)
                return frame.EnsureSequence(idHead.Storage, idTail.Storage, PrimitiveType.CreateWord(idHead.DataType.Size + idTail.DataType.Size));
            throw new NotImplementedException("Handle case when stack parameter is passed.");
        }

        public Expression VisitStackArgumentStorage(StackArgumentStorage stack)
        {
            if (ensureVariables)
                return frame.EnsureStackVariable(
                    stack.StackOffset - site.StackDepthOnEntry,
                    stack.DataType);
            else 
                return arch.CreateStackAccess(
                    frame,
                    stack.StackOffset - site.SizeOfReturnAddressOnStack,
                    stack.DataType);

        }

        public Expression VisitTemporaryStorage(TemporaryStorage temp)
        {
            throw new NotSupportedException(string.Format("A {0} can't be used as a formal parameter.", temp.GetType().FullName));
        }

        #endregion
    }
}
