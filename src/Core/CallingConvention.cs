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
    public abstract class CallingConvention
    {
        public abstract CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams);

        public int Align(int n, int quantum)
        {
            int units = (n + quantum - 1) / quantum;
            return units * quantum;
        }
    }

    public class CallingConventionResult
    {
        public Storage Return;
        public Storage ImplicitThis;
        public List<Storage> Parameters;
        public int StackDelta;
        public int FpuStackDelta;

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
