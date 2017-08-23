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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    public interface CallingConvention
    {
        void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams);
    }

    public abstract class ICallingConventionEmitter
    {
        public int StackDelta;
        public int FpuStackDelta;

        public int stackOffset;

        /// <summary>
        /// Indicate a floating point value is returned in an x87 FPU register.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="dt"></param>
        public abstract void FpuReturn(int depth, DataType dt);

        public abstract void LowLevelDetails(int stackAlignment, int initialStackOffset);

        /// <summary>
        /// Add a register parameter.
        /// </summary>
        /// <param name="stg"></param>
        public abstract void RegParam(RegisterStorage stg);

        /// <summary>
        /// Indicate a register is returned.
        /// </summary>
        /// <param name="stg"></param>
        public abstract void RegReturn(RegisterStorage stg);

        public abstract void ReverseParameters();
        /// <summary>
        /// Add a sequence parameter.
        /// </summary>
        /// <param name="stgHi"></param>
        /// <param name="stgLo"></param>
        public abstract void SequenceParam(RegisterStorage stgHi, RegisterStorage stgLo);

        /// <summary>
        /// Indicate that a sequence is returned.
        /// </summary>
        /// <param name="stgHi"></param>
        /// <param name="stgLo"></param>
        public abstract void SequenceReturn(RegisterStorage stgHi, RegisterStorage stgLo);

        public abstract void ImplicitThisRegister(Storage dtThis);

        public abstract void ImplicitThisStack(DataType dtThis);

        /// <summary>
        /// Add a stack parameter of the type dt.
        /// </summary>
        /// <param name="dt"></param>
        public abstract void StackParam(DataType dt);
    }

    public class CallingConventionEmitter : ICallingConventionEmitter
    {
        public Storage ImplicitThis;
        public Storage Return;
        public readonly List<Storage> Parameters;

        private int stackAlignment;

        public CallingConventionEmitter()
        {
            this.Parameters = new List<Storage>();
        }

        public static int Align(int n, int quantum)
        {
            int units = (n + quantum - 1) / quantum;
            return units * quantum;
        }

        public override void ImplicitThisRegister(Storage dtThis)
        {
            this.ImplicitThis = dtThis;
        }

        public override void ImplicitThisStack(DataType dt)
        {
            this.ImplicitThis = new StackArgumentStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
        }

        public override void LowLevelDetails(int stackAlignment, int initialStackOffset)
        {
            this.stackAlignment = stackAlignment;
            this.stackOffset = initialStackOffset;
        }

        public override void RegParam(RegisterStorage stg)
        {
            this.Parameters.Add(stg);
        }

        public override void RegReturn(RegisterStorage stg)
        {
            this.Return = stg;
        }

        public override void ReverseParameters()
        {
            this.Parameters.Reverse();
        }

        public override void SequenceParam(RegisterStorage stgHi, RegisterStorage stgLo)
        {
            this.Parameters.Add(new SequenceStorage(stgHi, stgLo));
        }

        public override void SequenceReturn(RegisterStorage stgHi, RegisterStorage stgLo)
        {
            this.Return = new SequenceStorage(stgHi, stgLo);
        }

        public override void StackParam(DataType dt)
        {
            var stg = new StackArgumentStorage(stackOffset, dt);
            stackOffset += Align(dt.Size, stackAlignment);
            Parameters.Add(stg);
        }

        public override void FpuReturn(int depth, DataType dt)
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
