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
using Reko.Core.Types;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Machine
{
    /// <summary>
    /// A class implementing this interface is expected to translate the data 
    /// types of a function type (i.e. the parameters and the return type) 
    /// into Storages (stack accesses, FPU stack accesses, registers or 
    /// register pairs. 
    /// </summary>
    public interface ICallingConvention
    {
        /// <summary>
        /// The name of the calling convention.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Generates a procedure signature for the calling convention. The signature is
        /// collected from the <see cref="ICallingConventionBuilder"/> after the call.
        /// </summary>
        /// <param name="ccr"><see cref="ICallingConventionBuilder"/> instance.</param>
        /// <param name="retAddressOnStack">Size of the return address on the stack.</param>
        /// <param name="dtRet">Return type.</param>
        /// <param name="dtThis">Optional type of C++ <c>this</c> pointer.</param>
        /// <param name="dtParams">Parameter types.</param>
        void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams);

        /// <summary>
        /// Can <paramref name="stg"/> be used as a parameter in this calling convention?
        /// </summary>
        bool IsArgument(Storage stg);

        /// <summary>
        /// Can <paramref name="stg" /> be used to return a value in this calling convention?
        /// </summary>
        /// <param name="stg"></param>
        bool IsOutArgument(Storage stg);

        /// <summary>
        /// Used to order input arguments according to the calling convention's ABI.
        /// </summary>
        IComparer<Identifier>? InArgumentComparer { get; }

        /// <summary>
        /// Used to order output arguments according to the calling convention's ABI.
        /// </summary>
        IComparer<Identifier>? OutArgumentComparer { get; }
    }

    /// <summary>
    /// Interface for a builder that builds a calling convention.
    /// </summary>
    public interface ICallingConventionBuilder
    {
        /// <summary>
        /// Allocate a stack slot of size <paramref name="dt"/>, without
        /// generating a parameter. Caller usually does this when building
        /// heterogenous sequences of register/stack storages.
        /// </summary>
        /// <param name="dt">Size of the stack slot to allocate.</param>
        /// <returns>Freshly allocated stack slot storage.</returns>
        Storage AllocateStackSlot(DataType dt);

        /// <summary>
        /// Indicate that the callee will clean up all pushed arguments off 
        /// the stack.
        /// </summary>
        void CalleeCleanup();

        /// <summary>
        /// Indicate that the callee will only clean up the return address off
        /// the stack.
        /// </summary>
        void CallerCleanup(int retAddressOnStack);

        /// <summary>
        /// Indicate a floating point value is returned in an x87 FPU register.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="dt"></param>
        void FpuReturn(int depth, DataType dt);

        /// <summary>
        /// Provides the stack alignment (minimum allocation unit on the
        /// stack) and the initial stack offset of the first parameter
        /// passed on the stack.
        /// </summary>
        void LowLevelDetails(int stackAlignment, int parameterStackSaveOffset);

        /// <summary>
        /// Add a parameter.
        /// </summary>
        void Param(Storage stg);


        /// <summary>
        /// Add a register parameter.
        /// </summary>
        void RegParam(RegisterStorage stg);

        /// <summary>
        /// Indicate a register is returned.
        /// </summary>
        void RegReturn(RegisterStorage stg);

        /// <summary>
        /// Reverses the order of the parameters. Used in calling conventions
        /// where arguments are passed left-to-right.
        /// </summary>
        void ReverseParameters();

        /// <summary>
        /// Add a sequence parameter.
        /// </summary>
        /// <param name="stgs">Sequence storage used to pass a parameter value.</param>
        void SequenceParam(params Storage[] stgs);

        /// <summary>
        /// Add a sequence parameter.
        /// </summary>
        /// <param name="seq">Sequence storage used to pass a parameter value.</param>
        void SequenceParam(SequenceStorage seq);

        /// <summary>
        /// Indicate that a sequence of storages is returned.
        /// </summary>
        /// <param name="stgs">An ordered sequence of starges to use for returning 
        /// a value.</param>
        void SequenceReturn(params Storage[] stgs);

        /// <summary>
        /// Indicate that a sequence of storages is returned.
        /// </summary>
        /// <param name="seq">An ordered sequence of starges to use for returning 
        /// a value.</param>
        void SequenceReturn(SequenceStorage seq);

        /// <summary>
        /// Indicate that the implicit "this" pointer is passed in a register.
        /// </summary>
        /// <param name="dtThis">The data type of the "this" pointer.</param>
        void ImplicitThisRegister(Storage dtThis);

        /// <summary>
        /// Indicate that the implicit "this" pointer is passed on the stack.
        /// </summary>
        /// <param name="dtThis">The data type of the "this" pointer.</param>
        void ImplicitThisStack(DataType dtThis);

        /// <summary>
        /// Add a stack parameter of the type dt.
        /// </summary>
        /// <param name="dt">The data type of the stack parameter.</param>
        void StackParam(DataType dt);

        /// <summary>
        /// Indicate that the procedure writes its return value to the stack.
        /// </summary>
        /// <param name="dtRet">The data type of the stack-based return value.</param>
        void StackReturn(DataType dtRet);
    }

    /// <summary>
    /// A class implemeting <see cref="ICallingConventionBuilder"/>.
    /// </summary>
    /// <summary>
    /// Implements the <see cref="ICallingConventionBuilder"/> interface.
    /// </summary>
    public class CallingConventionBuilder : ICallingConventionBuilder
    {
        private int stackAlignment;
        private int stackOffset;

        /// <summary>
        /// Constructs a <see cref="CallingConventionBuilder"/> instance.
        /// </summary>
        public CallingConventionBuilder()
        {
            Parameters = [];
        }

        /// <summary>
        /// The <see cref="Storage"/> used to pass the hidden "this" pointer, if any.
        /// </summary>
        public Storage? ImplicitThis { get; private set; }

        /// <summary>
        /// The <see cref="Storage"/> used to return a value.
        /// </summary>
        public Storage? Return { get; private set; }

        /// <summary>
        /// The <see cref="Storage"/>s used pass parameters into a procedure."/>
        /// </summary>
        public List<Storage> Parameters { get; private set; }

        /// <summary>
        /// The net effect on the stack pointer after calling this procedudre.
        /// </summary>
        public int StackDelta { get; private set; }

        /// <summary>
        /// The net effect on the FPU stack pointer after calling this procedure
        /// (for machines that have FPU stacks).
        /// </summary>
        public int FpuStackDelta { get; private set; }

        /// <summary>
        /// Aligns the given size up to the next higher multiple of the given quantum.
        /// </summary>
        /// <param name="n">Value to align.</param>
        /// <param name="quantum">Desierd alignment.</param>
        /// <returns>Returns an aligned value, which is either equal to the original
        /// value <paramref name="n"/> or greater by at most <paramref name="quantum"/> - 1.</returns>
        public static int Align(int n, int quantum)
        {
            int units = (n + quantum - 1) / quantum;
            return units * quantum;
        }

        /// <inheritdoc />
        public void CalleeCleanup()
        {
            StackDelta = stackOffset;
        }

        /// <inheritdoc />
        public void CallerCleanup(int retAddressOnStack)
        {
            StackDelta = retAddressOnStack;
        }

        /// <inheritdoc />
        public void ImplicitThisRegister(Storage dtThis)
        {
            ImplicitThis = dtThis;
        }

        void ICallingConventionBuilder.ImplicitThisStack(DataType dt)
        {
            ImplicitThis = new StackStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
        }

        /// <inheritdoc />
        public void LowLevelDetails(int stackAlignment, int initialStackOffset)
        {
            this.stackAlignment = stackAlignment;
            stackOffset = initialStackOffset;
        }

        /// <inheritdoc />
        public void Param(Storage stg)
        {
            Parameters.Add(stg);
        }

        /// <inheritdoc />
        public void RegParam(RegisterStorage stg)
        {
            Parameters.Add(stg);
        }

        /// <inheritdoc />
        public void RegReturn(RegisterStorage stg)
        {
            Return = stg;
        }

        /// <inheritdoc />
        public void ReverseParameters()
        {
            Parameters.Reverse();
        }

        /// <inheritdoc />
        public void SequenceParam(params Storage[] stgs)
        {
            Parameters.Add(new SequenceStorage(stgs));
        }

        /// <inheritdoc />
        public void SequenceParam(SequenceStorage seq)
        {
            Parameters.Add(seq);
        }

        /// <inheritdoc />
        public void SequenceReturn(params Storage[] stgs)
        {
            Return = new SequenceStorage(stgs);
        }

        /// <inheritdoc />
        public void SequenceReturn(SequenceStorage seq)
        {
            Return = seq;
        }

        /// <inheritdoc />
        public Storage AllocateStackSlot(DataType dt)
        {
            var stg = new StackStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
            return stg;
        }

        /// <inheritdoc />
        public void StackParam(DataType dt)
        {
            var stg = AllocateStackSlot(dt);
            Parameters.Add(stg);
        }

        /// <inheritdoc />
        public void StackReturn(DataType dt)
        {
            var stg = new StackStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
            Return = stg;
        }

        /// <inheritdoc />
        public void FpuReturn(int depth, DataType dt)
        {
            Return = new FpuStackStorage(depth, dt);
            FpuStackDelta = 1;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Stk: {0} ", StackDelta);
            if (FpuStackDelta != 0)
            {
                sb.AppendFormat("Fpu: {0} ", FpuStackDelta);
            }
            if (Return is not null)
            {
                sb.AppendFormat("{0} ", Return);
            }
            else
            {
                sb.AppendFormat("void ");
            }
            if (ImplicitThis is not null)
            {
                sb.AppendFormat("[this {0}] ", ImplicitThis);
            }
            sb.Append('(');
            var sep = "";
            foreach (var dt in Parameters)
            {
                sb.AppendFormat("{0}{1}", sep, dt);
                sep = ", ";
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}
