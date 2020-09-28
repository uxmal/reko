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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    /// <summary>
    /// A class implementing this interface is expected to translate the data 
    /// types of a function type (i.e. the parameters and the return type) 
    /// into Storages (stack accesses, FPU stack accesses, registers or 
    /// register pairs. 
    /// </summary>
    public interface CallingConvention
    {

        void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams);

        /// <summary>
        /// Can <paramref name="stg"/> be used as a parameter in this calling convention?
        /// </summary>
        bool IsArgument(Storage stg);

        /// <summary>
        /// Can <paramref name="stg" /> be used to return a value in this calling convention?
        /// </summary>
        /// <param name="stg"></param>
        bool IsOutArgument(Storage stg);
    }

    public interface ICallingConventionEmitter
    {
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
        void LowLevelDetails(int stackAlignment, int initialStackOffset);

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
        void SequenceParam(RegisterStorage stgHi, RegisterStorage stgLo);
        void SequenceParam(SequenceStorage seq);

        /// <summary>
        /// Indicate that a sequence is returned.
        /// </summary>
        void SequenceReturn(RegisterStorage stgHi, RegisterStorage stgLo);
        void SequenceReturn(SequenceStorage seq);

        /// <summary>
        /// Indicate that the implicit "this" pointer is passed in a register.
        /// </summary>
        /// <param name="dtThis"></param>
        void ImplicitThisRegister(Storage dtThis);

        /// <summary>
        /// Indicate that the implicit "this" pointer is passed on the stack.
        /// </summary>
        void ImplicitThisStack(DataType dtThis);

        /// <summary>
        /// Add a stack parameter of the type dt.
        /// </summary>
        /// <param name="dt"></param>
        void StackParam(DataType dt);
        void StackReturn(DataType dtRet);
    }

    public class CallingConventionEmitter : ICallingConventionEmitter
    {
        private int stackAlignment;
        private int stackOffset;

        public CallingConventionEmitter()
        {
            this.Parameters = new List<Storage>();
        }

        public Storage ImplicitThis { get; private set; }
        public Storage Return { get; private set; }
        public List<Storage> Parameters { get; private set; }
        public int StackDelta { get; private set; }
        public int FpuStackDelta { get; private set; }

        public static int Align(int n, int quantum)
        {
            int units = (n + quantum - 1) / quantum;
            return units * quantum;
        }

        public void CalleeCleanup()
        {
            this.StackDelta = this.stackOffset;
        }

        public void CallerCleanup(int retAddressOnStack)
        {
            this.StackDelta = retAddressOnStack;
        }

        public void ImplicitThisRegister(Storage dtThis)
        {
            this.ImplicitThis = dtThis;
        }

        public void ImplicitThisStack(DataType dt)
        {
            this.ImplicitThis = new StackArgumentStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
        }

        public void LowLevelDetails(int stackAlignment, int initialStackOffset)
        {
            this.stackAlignment = stackAlignment;
            this.stackOffset = initialStackOffset;
        }

        public void RegParam(RegisterStorage stg)
        {
            this.Parameters.Add(stg);
        }

        public void RegReturn(RegisterStorage stg)
        {
            this.Return = stg;
        }

        public void ReverseParameters()
        {
            this.Parameters.Reverse();
        }

        public void SequenceParam(RegisterStorage stgHi, RegisterStorage stgLo)
        {
            this.Parameters.Add(new SequenceStorage(
                PrimitiveType.CreateWord(stgHi.DataType.BitSize + stgLo.DataType.BitSize),
                stgHi,
                stgLo));
        }

        public void SequenceParam(SequenceStorage seq)
        {
            this.Parameters.Add(seq);
        }

        public void SequenceReturn(RegisterStorage stgHi, RegisterStorage stgLo)
        {
            this.Return = new SequenceStorage(
                PrimitiveType.CreateWord(stgHi.DataType.BitSize + stgLo.DataType.BitSize),
                stgHi,
                stgLo);
        }

        public void SequenceReturn(SequenceStorage seq)
        {
            this.Return = seq;
        }
        public void StackParam(DataType dt)
        {
            var stg = new StackArgumentStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
            Parameters.Add(stg);
        }

        public void StackReturn(DataType dt)
        {
            var stg = new StackArgumentStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
            this.Return = stg;
        }

        public void FpuReturn(int depth, DataType dt)
        {
            this.Return = new FpuStackStorage(0, dt);
            this.FpuStackDelta = 1;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Stk: {0} ", StackDelta);
            if (FpuStackDelta != 0)
            {
                sb.AppendFormat("Fpu: {0} ", FpuStackDelta);
            }
            if (this.Return != null)
            {
                sb.AppendFormat("{0} ", this.Return);
            }
            else
            {
                sb.AppendFormat("void ");
            }
            if (this.ImplicitThis != null)
            {
                sb.AppendFormat("[this {0}] ", this.ImplicitThis);
            }
            sb.Append("(");
            var sep = "";
            foreach (var dt in this.Parameters)
            {
                sb.AppendFormat("{0}{1}", sep, dt);
                sep = ", ";
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
