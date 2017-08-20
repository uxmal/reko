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
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.Environments.SysV.ArchSpecific
{
    // Based on algorithm described in
    // http://www.cygwin.com/ml/binutils/2003-06/msg00436.html

    public class MipsCallingConvention : CallingConvention
    {
        private int ir;
        private IProcessorArchitecture arch;

        public MipsCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            Storage ret = null;

            if (dtRet != null)
            {
                ret = this.GetReturnRegister(dtRet);
            }

            var parameters = new List<Storage>();
            int ir =  0;
            bool firstArgIntegral = false;

            for (int i = 0; i < dtParams.Count; ++i)
            {
                Storage param;
                var dtParam = dtParams[i];
                var prim = dtParam as PrimitiveType;
                if (prim != null && prim.Domain == Domain.Real && !firstArgIntegral)
                {
                    if ((ir % 2) != 0)
                        ++ir;
                    if (this.ir >= 4)
                    {
                        param = new StackArgumentStorage(-1, dtParam);
                    }
                    else
                    {
                        if (prim.Size == 4)
                        {
                            param = arch.GetRegister("f" + (this.ir + 12));
                            this.ir += 1;
                        }
                        else if (prim.Size == 8)
                        {
                            param = new SequenceStorage(
                            arch.GetRegister("f" + (ir + 12)),
                            arch.GetRegister("f" + (ir + 13)));
                            this.ir += 2;
                        }
                        else
                        {
                            throw new NotSupportedException(string.Format("Real type of size {0} not supported.", prim.Size));
                        }
                    }
                }
                else {
                    if (ir == 0)
                        firstArgIntegral = true;
                    if (dtParam.Size <= 4)
                    {
                        if (this.ir >= 4)
                        {
                            param = new StackArgumentStorage(-1, dtParam);
                        }
                        else
                        {
                            param = arch.GetRegister("r" + (ir + 4));
                            ++this.ir;
                        }
                    }
                    else if (dtParam.Size <= 8)
                    {
                        if ((ir & 1) != 0)
                            ++ir;
                        if (this.ir >= 4)
                        {
                            param = new StackArgumentStorage(-1, dtParam);
                        }
                        else
                        {
                            param = new SequenceStorage(
                                arch.GetRegister("r" + (ir + 4)),
                                arch.GetRegister("r" + (ir + 4)));
                            ir += 2;
                        }
                    }
                    else
                        throw new NotImplementedException();
                }
                parameters.Add(param);
            }
            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null, //$TODO
                Parameters = parameters,
                StackDelta = 0,
                FpuStackDelta = 0,
            };
        }

        public Storage GetReturnRegister(DataType dt)
        {
            int bitSize = dt.BitSize;
            var prim = dt as PrimitiveType;
            if (prim != null)
            {
                if (prim.Domain == Domain.Real)
                    return arch.GetRegister("f1");
            }
            return arch.GetRegister("r3");
        }
    }
}
